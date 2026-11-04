// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;

namespace BodoMoneyTransfer.Dal.Interfaces;

/// <summary>
/// Interface for BodoMoneyTransfer specific app globals
/// </summary>
public interface IBodoMoneyTransferAppGlobals : IAppGlobals
{
    /// <summary>
    /// Current banking options
    /// </summary>
    IOptions Options { get; set; }

    /// <summary>
    /// Path to archive the PDF account statements created by BodoMoneyTransfer
    /// </summary>
    string ArchivePath { get; set; }

    /// <summary>
    /// Receiver for error mails
    /// </summary>
    string ErrorMailer { get; set; }

    /// <summary>
    /// Path to archive the orginal PDF account statements from the bank
    /// </summary>
    string OriginalArchivePath { get; set; }


    /// <summary>
    /// Path to a file for quotations to store in
    /// </summary>
    string QuotationFile { get; set; }
}