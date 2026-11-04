// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using System;
using System.Runtime.Versioning;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Extensions;
using Bodoconsult.App.Interfaces;
using Bodoconsult.App.Windows.ConsoleTools;
using FinanzDbUpdater.Bll.App;

namespace FinanzDbUpdater;

/// <summary>
/// Console starter for BodoWebMailer
/// </summary>
[SupportedOSPlatform("windows10.0.17763")]
public class FinanzDbUpdaterConsoleStarter : BaseConsoleStarter
{
    private readonly Globals _globals;

    private readonly IAppLoggerProxy _loggerProxy;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="args">Current command line args</param>
    /// <param name="isTestEnvironment">Is running in test environment</param>
    public FinanzDbUpdaterConsoleStarter(string[] args, bool isTestEnvironment = false) : base($"FinanzDbUpdater{(isTestEnvironment ? "Test" : string.Empty)}", args)
    {
        MainAppRunnerDelegate = StartInternal;

        _globals = Globals.Instance;
        _globals.LoggingConfig.AddDefaultLoggerProviderConfiguratorsForConsoleApp();
        _globals.StatusMessageDelegate = SetStatus;

        _loggerProxy = _globals.Logger;
    }
    private int StartInternal(IConsoleStarter consoleStarter)
    {
        if (Args is null || Args.Length == 0)
        {
            return RunMainTask(consoleStarter);
        }

        //switch (Args[0])
        //{
        //    case "0":
        //        return RunMainTask(consoleStarter);
        //    case "/u":
        //    case "--u":
        //        var options = _globals.Options;
        //        options.IsArchiveAccountStatements = false;
        //        options.IsArchiveOriginalAccountStatements = false;
        //        options.IsGetSafekeepingItems = false;
        //        options.IsGetAccountStatement = false;
        //        return RunMainTask(consoleStarter);
        //    case "/c":
        //    case "--c":
        //        EncryptSingleValue();
        //        return 0;
        //    default:
        //        return 0;
        //}

        return 0;
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
        IAppBuilder builder = new FinanzDbUpdaterAppBuilder(_globals);
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
        // See FinanzDbUpdaterService.StartApplication() for the relevante parts of app start
        builder.StartApplication();

        return 0;
    }
}