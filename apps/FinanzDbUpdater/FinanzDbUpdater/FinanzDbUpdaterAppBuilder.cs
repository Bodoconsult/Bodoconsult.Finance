// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using System;
using System.Runtime.Versioning;
using Bodoconsult.App;
using Bodoconsult.App.Abstractions.Interfaces;
using FinanzDbUpdater.Bll.Interfaces;
using FinanzDbUpdater.DiContainerProvider;

namespace FinanzDbUpdater;

[SupportedOSPlatform("windows10.0.17763")]
public class FinanzDbUpdaterAppBuilder : BaseAppBuilder
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appGlobals">Global app settings</param>
    public FinanzDbUpdaterAppBuilder(IAppGlobals appGlobals) : base(appGlobals)
    { }

    /// <summary>
    /// Load the <see cref="IAppBuilder.DiContainerServiceProviderPackage"/>
    /// </summary>
    public override void LoadDiContainerServiceProviderPackage()
    {
        var factory = new FinanzDbUpdaterProductionDiContainerServiceProviderPackageFactory(AppGlobals);
        DiContainerServiceProviderPackage = factory.CreateInstance();
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