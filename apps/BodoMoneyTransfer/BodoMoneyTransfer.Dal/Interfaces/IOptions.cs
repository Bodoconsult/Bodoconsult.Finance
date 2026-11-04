// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.Finance.Payment.Base;
using Bodoconsult.Finance.Payment.Base.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Bodoconsult.App.Abstractions.Delegates;

namespace BodoMoneyTransfer.Dal.Interfaces;

/// <summary>
/// Banking options
/// </summary>
public interface IOptions
{
    /// <summary>
    /// FINTS-Registriernummer as product name since 1.8.2019
    /// </summary>
    string ProductName { get; set; }

    /// <summary>
    /// Product version (max. length 5 chars)
    /// </summary>
    [MaxLength(5)]
    string ProductVersion { get; set; }

    /// <summary>
    /// A delegate for "status message" method calls getting a string an input parameter
    /// </summary>
    StatusMessageDelegate StatusMessageDelegate { get; set; }

    /// <summary>
    /// A delegate sending an important payment message to UI or logging
    /// </summary>
    ShowPaymentMessageDelegate SetImportantMessageDelegate { get; set; }

    /// <summary>
    /// End date for live account statements
    /// </summary>
    DateTime AccountStatementDateFrom { get; set; }

    /// <summary>
    /// Start date for live account statements
    /// </summary>
    DateTime AccountStatementDateUntil { get; set; }

    /// <summary>
    /// End date for live account statements
    /// </summary>
    DateTime ArchiveAccountStatementDateFrom { get; set; }

    /// <summary>
    /// Start date for live account statements
    /// </summary>
    DateTime ArchiveAccountStatementDateUntil { get; set; }
        
    /// <summary>
    /// Path to archive account statements
    /// </summary>
    string ArchivePath { get; set; }
        
    /// <summary>
    /// Mail address for error mails
    /// </summary>
    string ErrorMailer { get; set; }

    ///// <summary>
    ///// Error messages during workload processing
    ///// </summary>

    //IList<string> Errors { get; }

    /// <summary>
    /// Should account statements be archived as MT940
    /// </summary>
    bool IsArchiveAccountStatements { get; set; }
        
    /// <summary>
    /// Should the original PDFs of the account statements be fetched from the server
    /// </summary>
    bool IsArchiveOriginalAccountStatements { get; set; }

    /// <summary>
    /// Should account statements be fetched
    /// </summary>
    bool IsGetAccountStatement { get; set; }

    /// <summary>
    /// Should the safekeeping items be fetched from the server
    /// </summary>
    bool IsGetSafekeepingItems { get; set; }

    /// <summary>
    /// Should transaction transfer run
    /// </summary>
    bool IsRunTransactions { get; set; }

    /// <summary>
    /// Path to archive orginal account statements
    /// </summary>
    string OriginalArchivePath { get; set; }

    /// <summary>
    /// Name of the file to write the quotation of safekeepings in
    /// </summary>
    string QuotationFileName { get; set; }

    /// <summary>
    /// Database connection string (ADO.NET)
    /// </summary>
    string ConnectionString { get; set; }

    /// <summary>
    /// All safekeeping items
    /// </summary>
    IList<SafekeepingItem> SafekeepingItems { get; }

    /// <summary>
    /// Use Kontoinformationsdienst (KID) for account infos
    /// </summary>
    bool UseKidForAccountInfos { get; set; }
}