// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App;
using Bodoconsult.App.Abstractions.Interfaces;
using BodoMoneyTransfer.DiContainerProvider;
using System;
using System.Runtime.Versioning;
using BodoMoneyTransfer.Dal.Interfaces;

namespace BodoMoneyTransfer;

[SupportedOSPlatform("windows10.0.17763")]
public class BodoMoneyTransferAppBuilder : BaseAppBuilder
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appGlobals">Global app settings</param>
    public BodoMoneyTransferAppBuilder(IAppGlobals appGlobals) : base(appGlobals)
    { }

    /// <summary>
    /// Load the <see cref="IAppBuilder.DiContainerServiceProviderPackage"/>
    /// </summary>
    public override void LoadDiContainerServiceProviderPackage()
    {
        var factory = new BodoMoneyTransferProductionDiContainerServiceProviderPackageFactory(AppGlobals);
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

        var section = root.GetSection("BodoMoneyTransfer");

        if (AppGlobals is not IBodoMoneyTransferAppGlobals globals)
        {
            throw new ArgumentException("AppGlobals is not IBodoMoneyTransferAppGlobals");
        }

        globals.ArchivePath = DefaultAppStartProvider.ReadStringProperty(section, "ArchivePath", globals.ArchivePath);
        globals.OriginalArchivePath = DefaultAppStartProvider.ReadStringProperty(section, "OriginalArchivePath", globals.OriginalArchivePath);
        globals.QuotationFileName = DefaultAppStartProvider.ReadStringProperty(section, "QuotationFile", globals.QuotationFileName);
    }
}