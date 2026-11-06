// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App;
using Bodoconsult.App.Abstractions.Delegates;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.Finance.Payment.Base;
using Bodoconsult.Finance.Payment.Base.Models;
using BodoMoneyTransfer.Dal.Interfaces;
using BodoMoneyTransfer.Dal.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BodoMoneyTransfer.Dal.App;

/// <summary>
/// App global values
/// </summary>
public class Globals : IBodoMoneyTransferAppGlobals 
{
    #region Singleton factory

    // Thread-safe implementation of singleton pattern
    private static Lazy<Globals> _instance;

    /// <summary>
    /// Get a singleton instance of 
    /// </summary>
    /// <returns></returns>
    public static Globals Instance
    {
        get
        {
            try
            {
                _instance ??= new Lazy<Globals>(() => new Globals());
                return _instance.Value;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    #endregion

    /// <summary>
    /// Default ctor
    /// </summary>
    private Globals()
    {
        AccountStatementDateFrom = DateTime.Now.AddMonths(-2);
        AccountStatementDateUntil = DateTime.Now;

        var date = DateTime.Now.AddMonths(-1);

        ArchiveAccountStatementDateFrom = new DateTime(date.Year, date.Month, 1);
        ArchiveAccountStatementDateUntil = ArchiveAccountStatementDateFrom.AddMonths(1).AddDays(-1);
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        //throw new NotImplementedException();
    }

    /// <summary>
    /// This event is set if the application is started only as singleton
    /// </summary>
    public EventWaitHandle EventWaitHandle { get; set; }

    /// <summary>
    /// App start parameter
    /// </summary>
    public IAppStartParameter AppStartParameter { get; set; } = new AppStartParameter();

    /// <summary>
    /// Current log data entry factory
    /// </summary>
    public ILogDataFactory LogDataFactory { get; set; } = new LogDataFactory();

    /// <summary>
    /// Current logging config
    /// </summary>
    public LoggingConfig LoggingConfig { get; set; } = new();

    /// <summary>
    /// Current app logger. Use this instance only if no DI container is available. Nonetheless, use DiContainer.Get&lt;IAppLoggerProxy&gt; to fetch the default app logger from DI container. Don't forget to load it during DI setup!
    /// </summary>
    public IAppLoggerProxy Logger { get; set; }

    /// <summary>
    /// Current dependency injection (DI) container
    /// </summary>
    public Bodoconsult.App.Abstractions.DependencyInjection.DiContainer DiContainer { get; set; } = new();

    /// <summary>
    /// Delegate called if a fatale app exception has been raised and a message to the UI has to be sent before app terminates
    /// </summary>
    public HandleFatalExceptionDelegate HandleFatalExceptionDelegate { get; set; }

    /// <summary>
    /// Current app storage connection check instance or null
    /// </summary>
    public IAppStorageConnectionCheck AppStorageConnectionCheck { get; set; }

    /// <summary>
    /// Current status message delegate
    /// </summary>
    public StatusMessageDelegate StatusMessageDelegate { get; set; } = Console.WriteLine;

    /// <summary>
    /// Current license management delegate
    /// </summary>
    public LicenseMissingDelegate LicenseMissingDelegate { get; set; }

    /// <summary>
    /// Delegate to handle I18N translations
    /// </summary>
    public TranslateDelegate TranslateDelegate { get; set; }

    /// <summary>
    /// Delegate to handle I18N translations with parameters to fill in translated text
    /// </summary>
    public TranslateWithParamsDelegate TranslateWithParamsDelegate { get; set; }

    /// <summary>
    /// Externally registered product name
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// Externally registered version
    /// </summary>
    public string ProductVersion { get; set; }

    /// <summary>
    /// The current configuration loaded from appsettings.json
    /// </summary>
    public IConfigurationRoot ConfigurationRoot { get; set; }


    /// <summary>
    /// A delegate sending an important payment message to UI or logging
    /// </summary>
    public ShowPaymentMessageDelegate SetImportantMessageDelegate { get; set; }

    /// <summary>
    /// Path to archive the PDF account statements created by BodoMoneyTransfer
    /// </summary>
    public string ArchivePath { get; set; }

    /// <summary>
    /// Path to archive the orginal PDF account statements from the bank
    /// </summary>
    public string OriginalArchivePath { get; set; }

    /// <summary>
    /// Should transaction transfer run
    /// </summary>
    public bool IsRunTransactions { get; set; } = true;

    /// <summary>
    /// Should account statements be fetched
    /// </summary>
    public bool IsGetAccountStatement { get; set; } = true;

    /// <summary>
    /// Should account statements be archived as MT940
    /// </summary>
    public bool IsArchiveAccountStatements { get; set; } = true;

    /// <summary>
    /// Should the original PDFs of the account statements be fetched from the server
    /// </summary>
    public bool IsArchiveOriginalAccountStatements { get; set; } = true;

    /// <summary>
    /// Should the safekeeping items be fetched from the server
    /// </summary>
    public bool IsGetSafekeepingItems { get; set; } = true;

    /// <summary>
    /// Name of the file to write the quotation of safekeepings as JSON in
    /// </summary>
    public string QuotationFileName { get; set; }

    /// <summary>
    /// All safekeeping items
    /// </summary>
    public IList<SafekeepingItem> SafekeepingItems { get; } = new List<SafekeepingItem>();

    /// <summary>
    /// Use Kontoinformationsdienst (KID) for account infos. Default: false
    /// </summary>
    public bool UseKidForAccountInfos { get; set; }

    /// <summary>
    /// Start date for live account statements
    /// </summary>
    public DateTime AccountStatementDateUntil { get; set; }

    /// <summary>
    /// End date for live account statements
    /// </summary>
    public DateTime AccountStatementDateFrom { get; set; }

    /// <summary>
    /// Start date for live account statements
    /// </summary>
    public DateTime ArchiveAccountStatementDateUntil { get; set; }

    /// <summary>
    /// End date for live account statements
    /// </summary>
    public DateTime ArchiveAccountStatementDateFrom { get; set; }
}