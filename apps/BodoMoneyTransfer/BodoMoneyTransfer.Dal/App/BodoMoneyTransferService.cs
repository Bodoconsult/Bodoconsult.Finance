// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using System;
using System.Diagnostics;
using System.Threading;
using Bodoconsult.App.Abstractions.Delegates;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Helpers;
using BodoMoneyTransfer.Dal.Interfaces;

namespace BodoMoneyTransfer.Dal.App;

/// <summary>
/// Current implementation of <see cref="IApplicationService"/>
/// </summary>
public class BodoMoneyTransferService : IApplicationService
{
    private bool _isStopped;
    private bool _isStarting;
    private readonly IAppLoggerProxy _appLogger;

    /// <summary>
    /// Default ctor
    /// </summary>
    public BodoMoneyTransferService(IAppLoggerProxy appLogger,
        IAppGlobals appGlobals)
    {
        _appLogger = appLogger;
        AppGlobals = appGlobals;
        Offline = false;
    }

    /// <summary>
    /// Request application stop delegate
    /// </summary>
    public RequestApplicationStopDelegate RequestApplicationStopDelegate { get; set; }

    /// <summary>
    /// Current app globals
    /// </summary>
    public IAppGlobals AppGlobals { get; }

    /// <summary>
    /// Application status offline true / false
    /// </summary>
    public bool Offline { get; set; }

    /// <summary>
    /// Register required services like GRPC clients etc.
    /// </summary>
    public void RegisterServices()
    {
        // Do nothing in this demo
    }

    /// <summary>
    /// Start the application
    /// </summary>
    /// <param name="cancellationToken"></param>
    public void StartApplication(CancellationToken? cancellationToken)
    {
        _isStarting = true;

        if (_isStopped)
        {
            return;
        }

        _isStarting = false;

        // Do start your workload here
        Start();

        // Stop app now
        if (RequestApplicationStopDelegate is null)
        {
            return;
        }

        // Fire app stop now if workload is done
        AsyncHelper.FireAndForget(RequestApplicationStopDelegate.Invoke);
    }

    private void Start()
    {
        var ph = AppGlobals.DiContainer.Get<IPaymentHandler>();

        var args = AppGlobals.AppStartParameter.Args;

        if (args?.Length > 0)
        {
            switch (args[ 0].ToLowerInvariant())
            {
                case "/t":
                case "--t":
                    ph.SendTestMail();
                    return;
            }
        }

        ph.AppGlobals.UseKidForAccountInfos = false;

        ph.StartJobCollector();
        ph.ProcessAllWorkloads();
        ph.SaveSafeKeepingsFile();

        ph.SendErrorMails();

        Status("BodoMoneyTransfer done!");
    }

    public void Status(string msg)
    {
        if (msg.Contains("Error"))
        {
            _appLogger.LogError(msg);
        }
        else
        {
            _appLogger.LogInformation(msg);
        }
        AppGlobals.StatusMessageDelegate?.Invoke(msg);
    }

    /// <summary>
    /// Suspend the app
    /// </summary>
    public void SuspendApplication()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Restart the app if it is in suspend state
    /// </summary>
    public void RestartApplication()
    {
        throw new NotSupportedException();
    }

    //private void OnLicenseMissingEvent(object sender, LicenseMissingArgs e)
    //{
    //    var msg = $"License is missing: {e.ErrorMessage}. Application will stop";
    //    _appLogger.LogError(msg);
    //    StopApplication();

    //    LicenseMissingDelegate?.Invoke(msg);
    //}

    /// <summary>
    /// Stop the application
    /// </summary>
    public void StopApplication()
    {
        _appLogger.LogWarning("Stopping application starts...");

        // Start process running? If yes leave here
        if (_isStarting)
        {
            return;
        }

        // Do not stop more than one time
        if (_isStopped)
        {
            return;
        }

        _isStopped = true;

        //// Do all needed to stop youe app correctly
        //var di = AppGlobals.DiContainer;

        //try
        //{
        //    // Stop performance logging
        //    var perflog = di.Get<IPerformanceLoggerManager>();
        //    perflog?.StopLogging();
        //}
        //catch (Exception e)
        //{
        //    Debug.Print(e.Message);
        //    //_appLogger.LogError($"Performance logging could not be stopped", new object[]{e});
        //}

        //var gms = di.Get<IGeneralAppManagementManager>();
        //var request = new EmptyBusinessTransactionRequestData();

        //DefaultBusinessTransactionReply result;

        //// Create log dump on app stop
        //try
        //{

        //    // ToDo: fill request with useful information for logging
        //    result = gms.CreateLogDump(request);

        //    //if (result != null)
        //    //{
        //    _appLogger?.LogWarning($"CreateLogDump: error code {result.ErrorCode}: {result.Message}");
        //    //}
        //}
        //catch
        //{
        //    // Do nothing
        //}s

        // Stop logging now
        try
        {
            if (_appLogger != null)
            {
                _appLogger.StopLogging();
                _appLogger.Dispose();
            }

        }
        catch
        {
            // Do nothing
        }
    }

    /// <summary>
    /// Current <see cref="IApplicationService.LicenseMissingDelegate"/>
    /// </summary>
    public LicenseMissingDelegate LicenseMissingDelegate { get; set; }
}