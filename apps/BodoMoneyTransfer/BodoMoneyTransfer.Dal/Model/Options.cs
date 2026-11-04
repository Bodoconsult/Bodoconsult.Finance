// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.Finance.Payment.Base;
using Bodoconsult.Finance.Payment.Base.Models;
using BodoMoneyTransfer.Dal.Interfaces;
using System;
using System.Collections.Generic;
using Bodoconsult.App.Abstractions.Delegates;

namespace BodoMoneyTransfer.Dal.Model;

/// <summary>
/// Banking options
/// </summary>
public class Options : IOptions
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public Options()
    {
        AccountStatementDateFrom = DateTime.Now.AddMonths(-2);
        AccountStatementDateUntil = DateTime.Now;

        var date = DateTime.Now.AddMonths(-1);

        ArchiveAccountStatementDateFrom = new DateTime(date.Year, date.Month, 1);
        ArchiveAccountStatementDateUntil = ArchiveAccountStatementDateFrom.AddMonths(1).AddDays(-1);
    }

    /// <summary>
    /// FINTS-Registriernummer as product name since 1.8.2019
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// Product version (max. length 5 chars)
    /// </summary>
    public string ProductVersion { get; set; }

    /// <summary>
    /// A delegate for "status message" method calls getting a string an input parameter
    /// </summary>
    public StatusMessageDelegate StatusMessageDelegate { get; set; }

    /// <summary>
    /// A delegate sending an important payment message to UI or logging
    /// </summary>
    public ShowPaymentMessageDelegate SetImportantMessageDelegate { get; set; }

    /// <summary>
    /// Path to archive account statements
    /// </summary>
    public string ArchivePath { get; set; }

    /// <summary>
    /// Path to archive orginal account statements
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
    /// Name of the file to write the quotation of safekeepings in
    /// </summary>
    public string QuotationFileName { get; set; }

    /// <summary>
    /// Database connection string (ADO.NET)
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Mail address for error mails
    /// </summary>
    public string ErrorMailer { get; set; }
    
    /// <summary>
    /// All safekeeping items
    /// </summary>
    public IList<SafekeepingItem> SafekeepingItems { get; } = new List<SafekeepingItem>();

    /// <summary>
    /// Use Kontoinformationsdienst (KID) for account infos. Default: false
    /// </summary>
    public bool UseKidForAccountInfos { get; set; }
    
    ///// <summary>
    ///// Error messages during workload processing
    ///// </summary>
    //public IList<string> Errors { get; } = new List<string>();


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