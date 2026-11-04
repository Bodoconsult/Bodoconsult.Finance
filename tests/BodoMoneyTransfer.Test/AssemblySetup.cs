// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Extensions;
using BodoMoneyTransfer.Dal.App;
using BodoMoneyTransfer.Dal.Interfaces;
using NUnit.Framework;
using System;

namespace BodoMoneyTransfer.Test;

/// <summary>
/// Setup for the assembly for all tests
/// </summary>
[SetUpFixture]
public static class AssemblySetup
{
    /// <summary>
    /// At startup of the assembly
    /// </summary>
    [OneTimeSetUp]
    public static void AssemblyStartUp()
    {
        AppHelper.LoadPasswordData();

        var globals = (IBodoMoneyTransferAppGlobals)App.Globals.Instance;

        var temp = System.IO.Path.GetTempPath();

        globals.Options.ArchivePath = System.IO.Path.Combine(temp, "BodoMoneyTransfer\\Archive");
        globals.Options.ErrorMailer = "test@bodoconsult.de";
        globals.Options.OriginalArchivePath = System.IO.Path.Combine(temp, "BodoMoneyTransfer\\Originals");
        globals.Options.QuotationFileName = System.IO.Path.Combine(temp, "BodoMoneyTransfer\\QuotationFile.json");

        globals.LoggingConfig.AddDefaultLoggerProviderConfiguratorsForUiApp();

        // Set additional app start parameters as required
        var param = globals.AppStartParameter;
        param.AppName = "WinAppTests: Demo app";
        param.SoftwareTeam = "Robert Leisner";
        //param.LogoRessourcePath = "WinFormsConsoleApp1.Resources.logo.jpg";
        param.AppFolderName = "WinAppTests";

        // Now start the app building process
        var builder = new MyDebugAppBuilder(globals);
#if !DEBUG
        AppDomain.CurrentDomain.UnhandledException += builder.CurrentDomainOnUnhandledException;
#endif

        // Load basic app metadata
        builder.LoadBasicSettings();

        // Process the config file
        builder.ProcessConfiguration();

        // Now load the globally needed settings
        builder.LoadGlobalSettings();

        ArgumentNullException.ThrowIfNull(globals.Logger);
        globals.Logger.LogInformation("Starting tests...");
    }
}