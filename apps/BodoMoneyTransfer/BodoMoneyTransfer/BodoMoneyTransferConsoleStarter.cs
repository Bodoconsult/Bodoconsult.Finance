// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.DataProtection;
using Bodoconsult.App.Extensions;
using Bodoconsult.App.Helpers;
using Bodoconsult.App.Interfaces;
using Bodoconsult.App.Windows.ConsoleTools;
using Bodoconsult.App.Windows.System;
using Bodoconsult.Finance.Payment.Base.Helpers;
using BodoMoneyTransfer.Dal.App;

namespace BodoMoneyTransfer;

/// <summary>
/// Console starter for BodoWebMailer
/// </summary>
[SupportedOSPlatform("windows10.0.17763")]
public class BodoMoneyTransferConsoleStarter : BaseConsoleStarter
{
    private readonly Globals _globals;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="args">Current command line args</param>
    /// <param name="isTestEnvironment">Is running in test environment</param>
    public BodoMoneyTransferConsoleStarter(string[] args, bool isTestEnvironment = false) : base($"BodoMoneyTransfer{(isTestEnvironment ? "Test" : string.Empty)}", args)
    {
        MainAppRunnerDelegate = StartInternal;

        AppHelper.LoadPasswordData();

        _globals = Globals.Instance;
        _globals.StatusMessageDelegate = SetStatus;
        _globals.LoggingConfig.AddDefaultLoggerProviderConfiguratorsForConsoleApp();

        ArgumentNullException.ThrowIfNull(AppPath);
        CredentialsImportFileName = Path.Combine(AppPath, "workload.json");

        ArgumentNullException.ThrowIfNull(DataPath);
        CredentialsFileName = Path.Combine(DataPath, "data2.dat");
    }

    /// <summary>
    /// Filename of the workload storage file
    /// </summary>
    public string CredentialsFileName { get; }

    /// <summary>
    /// Filename of the workload storage import file. To be deleted after import
    /// </summary>
    public string CredentialsImportFileName { get; }

    /// <summary>
    /// Current workload
    /// </summary>
    public Workload Workload { get; private set; }

    private int StartInternal(IConsoleStarter consoleStarter)
    {
        if (Args is null || Args.Length == 0)
        {
            return RunMainTask(consoleStarter);
        }

        switch (Args[0])
        {
            case "0":
                return RunMainTask(consoleStarter);
            case "/u":
            case "--u":
                _globals.IsArchiveAccountStatements = false;
                _globals.IsArchiveOriginalAccountStatements = false;
                _globals.IsGetSafekeepingItems = false;
                _globals.IsGetAccountStatement = false;
                return RunMainTask(consoleStarter);
            case "/c":
            case "--c":
                EncryptSingleValue();
                return 0;
            default:
                return 0;
        }
    }

    /// <summary>
    /// Encrypt a single value. Public only for unit testing
    /// </summary>
    public static void EncryptSingleValue()
    {
        var value = ConsoleHelper.AskUserForStringValue("Please insert value to encrypt for usage with BodoMoneyTransfer:");

        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        var crypted = PasswordHandler.Encrypt(value);

        Debug.Print(crypted);

        Clipboard.SetText(crypted);

        ConsoleHelper.WaitUntilAnyKeyPressed("Crypted value was copied to clipboard!");
    }

    /// <summary>
    /// Run the main application mask
    /// </summary>
    /// <returns>Error code or 0 in case of successful</returns>
    public int RunMainTask(IConsoleStarter consoleStarter)
    {
        // Set additional app start parameters as required
        var param = _globals.AppStartParameter;
        param.SoftwareTeam = "Robert Leisner";

        _globals.AppStartParameter.Args = Args;

        // Now start app buiding process
        IAppBuilder builder = new BodoMoneyTransferAppBuilder(_globals);
#if !DEBUG
        AppDomain.CurrentDomain.UnhandledException += builder.CurrentDomainOnUnhandledException;
#endif

        // Load basic app metadata
        builder.LoadBasicSettings();

        // Process the config file
        builder.ProcessConfiguration();

        // Now load the globally needed settings
        builder.LoadGlobalSettings();

        // Write first log entry with default logger
        ArgumentNullException.ThrowIfNull(_globals.Logger);
        _globals.Logger.LogInformation($"{param.AppName} {param.AppVersion} starts...");
        _globals.StatusMessageDelegate = SetStatus;

        Console.WriteLine("Logging started...");

        // App is ready now for doing something
        Console.WriteLine($"Connection string loaded: {param.DefaultConnectionString}");

        // Prepare the DI container package
        builder.LoadDiContainerServiceProviderPackage();
        builder.RegisterDiServices();
        builder.FinalizeDiContainerSetup();

        // Now finally start the app and wait
        // See BodoMoneyTransferService.StartApplication() for the relevante parts of app start
        builder.StartApplication();

        return 0;
    }

    /// <summary>
    /// Load the workload
    /// </summary>
    public void LoadWorkload()
    {
        Workload = LoadCredentials<Workload>("Workload", CredentialsFileName, CredentialsImportFileName);

        if (Workload is null)
        {
            return;
        }

        _globals.ProductName = Workload.ProductName;
        _globals.ProductVersion = PaymentAppHelper.GetProductVersion();
    }
}