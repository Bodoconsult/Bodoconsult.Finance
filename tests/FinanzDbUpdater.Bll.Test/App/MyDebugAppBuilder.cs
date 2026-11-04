// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App;
using Bodoconsult.App.Abstractions.Interfaces;
using FinanzDbUpdater.Bll.Interfaces;
using System;

namespace BaseTests.Apps;

public class MyDebugAppBuilder : BaseDebugAppBuilder
{
    public MyDebugAppBuilder(IAppGlobals appGlobals) : base(appGlobals)
    {
    }

    /// <summary>
    /// Process the configuration from <see cref="IAppStartParameter.ConfigFile"/>
    /// </summary>
    public override void ProcessConfiguration()
    {
        // Load basic config
        base.ProcessConfiguration();

        ArgumentNullException.ThrowIfNull(AppStartProvider?.AppConfigurationProvider);

        // Now get the root configuration element
        var root = AppStartProvider.AppConfigurationProvider.Configuration;

        if (root is null)
        {
            return;
        }

        var section = root.GetSection("FinanzDbUpdater");

        if (AppGlobals is not IFinanzDbUpdaterAppGlobals globals)
        {
            throw new ArgumentException("AppGlobals is not IFinanzDbUpdaterAppGlobals");
        }

        globals.ArchivePath = DefaultAppStartProvider.ReadStringProperty(section, "ArchivePath", globals.ArchivePath);
        globals.DepotJsonFile = DefaultAppStartProvider.ReadStringProperty(section, "DepotJsonFile", globals.DepotJsonFile);
        globals.LogoSimulationReport = DefaultAppStartProvider.ReadStringProperty(section, "LogoSimulationReport", globals.LogoSimulationReport);
        globals.QuotationJsonFile = DefaultAppStartProvider.ReadStringProperty(section, "QuotationFile ", globals.QuotationJsonFile);
    }
}