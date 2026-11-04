// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Factories;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.Versioning;
using Bodoconsult.App.Abstractions.DependencyInjection;
using Bodoconsult.Database.Interfaces;
using Bodoconsult.Database.SqlClient;
using Bodoconsult.Finance.Quotations;
using Bodoconsult.Finance.Quotations.Interfaces;
using FinanzDbUpdater.Bll;
using FinanzDbUpdater.Bll.Interfaces;
using FinanzDbUpdater.Bll.App;
using FinanzDbUpdater.Bll.DataHandling;

namespace FinanzDbUpdater.DiContainerProvider;

/// <summary>
/// Load all specific FinanzDbUpdater services to DI container. Intended mainly for production
/// </summary>
[SupportedOSPlatform("windows10.0.17763")]
public class FinanzDbUpdaterAllServicesContainerServiceProvider : IDiContainerServiceProvider
{
    //private readonly string _benchmarkFileName = Path.Combine("C:\\ProgramData\\FinanzDbUpdater", "FinanzDbUpdater_Benchmark.csv");
    private readonly IAppGlobals _appGlobals;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appGlobals">Current app globals</param>
    public FinanzDbUpdaterAllServicesContainerServiceProvider(IAppGlobals appGlobals)
    {
        ArgumentNullException.ThrowIfNull(appGlobals);
        AppStartParameter = appGlobals.AppStartParameter;
        _appGlobals = appGlobals;
    }

    /// <summary>
    /// Current app start parameter
    /// </summary>
    public IAppStartParameter AppStartParameter { get; }

    /// <summary>
    /// Add DI container services to a DI container
    /// </summary>
    /// <param name="diContainer">Current DI container</param>
    public void AddServices(DiContainer diContainer)
    {
        if (_appGlobals is not IFinanzDbUpdaterAppGlobals globals)
        {
            throw new ArgumentException("AppGlobals is not IFinanzDbUpdaterAppGlobals");
        }

        // Factories to create tower instance related objects (should be singletons)
        diContainer.AddSingletonInstance(globals.LogDataFactory);
        diContainer.AddSingleton<IAppLoggerProxyFactory, AppLoggerProxyFactory>();

        //// benchmark
        //var benchProxy = AppBenchProxy.CreateAppBenchProxy(_benchmarkFileName, globals.LogDataFactory);
        //diContainer.AddSingletonInstance(benchProxy);

        //// General app management
        //diContainer.AddSingleton<IGeneralAppManagementService, GeneralAppManagementService>();
        //diContainer.AddSingleton<IGeneralAppManagementManager, GeneralAppManagementManager>();

        // Services
        var db = new SqlClientConnManager(globals.AppStartParameter.DefaultConnectionString)
        {
            SendStatus = SendStatus
        };
        db.SetCommandTimeout(600000);
        diContainer.AddSingleton((IConnManager)db);

        diContainer.AddSingleton<IDataHandler, DataHandler>();
        diContainer.AddSingleton<IQuotationHandler, QuotationHandler>();
        diContainer.AddSingleton<IFinanzDbUpdateHandler , FinanzDbUpdateHandler >();

        // Load all other services required for the app now
        diContainer.AddSingleton<IApplicationService, FinanzDbUpdaterService>();
    }

    private void SendStatus(string message)
    {
        _appGlobals.StatusMessageDelegate?.Invoke(message);
        _appGlobals.Logger?.LogInformation(message);
    }

    /// <summary>
    /// Late bind DI container references to avoid circular DI references
    /// </summary>
    /// <param name="diContainer"></param>
    public void LateBindObjects(DiContainer diContainer)
    {
        var appLogger = diContainer.Get<IAppLoggerProxy>();

        //appLogger.LogInformation($"Benchmark starts logging to {_benchmarkFileName}...");

        // Set logger to current logger factory
        var loggerFactory = diContainer.Get<ILoggerFactory>();
        appLogger.UpdateILoggerFactory(loggerFactory);
    }
}