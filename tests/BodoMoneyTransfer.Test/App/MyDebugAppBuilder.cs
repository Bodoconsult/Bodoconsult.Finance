// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App;
using Bodoconsult.App.Abstractions.Interfaces;
using BodoMoneyTransfer.Dal.Interfaces;
using System;

namespace BodoMoneyTransfer.Test.App;

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