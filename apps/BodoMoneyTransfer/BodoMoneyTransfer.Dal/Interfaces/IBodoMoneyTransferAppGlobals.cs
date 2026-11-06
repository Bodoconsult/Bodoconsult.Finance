// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.Finance.Payment.Base;
using Bodoconsult.Finance.Payment.Base.Models;
using System;
using System.Collections.Generic;

namespace BodoMoneyTransfer.Dal.Interfaces;

/// <summary>
/// Interface for BodoMoneyTransfer specific app globals
/// </summary>
public interface IBodoMoneyTransferAppGlobals : IAppGlobals
{
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
    /// Path to archive the PDF account statements created by BodoMoneyTransfer
    /// </summary>
    string ArchivePath { get; set; }

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
    /// Path to archive the orginal PDF account statements from the bank
    /// </summary>
    string OriginalArchivePath { get; set; }

    /// <summary>
    /// Name of the file to write the quotation of safekeepings as JSON in
    /// </summary>
    string QuotationFileName { get; set; }

    /// <summary>
    /// All safekeeping items
    /// </summary>
    IList<SafekeepingItem> SafekeepingItems { get; }

    /// <summary>
    /// Use Kontoinformationsdienst (KID) for account infos
    /// </summary>
    bool UseKidForAccountInfos { get; set; }
}