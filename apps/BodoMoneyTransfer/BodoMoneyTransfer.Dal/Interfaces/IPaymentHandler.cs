// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.Finance.Payment.Base.Models;
using BodoMoneyTransfer.Dal.Delegates;

namespace BodoMoneyTransfer.Dal.Interfaces;

/// <summary>
/// Interface for exchanging payment data with banks
/// </summary>
public interface IPaymentHandler
{
    /// <summary>
    /// Current app globals
    /// </summary>
    IBodoMoneyTransferAppGlobals AppGlobals { get; }

    /// <summary>
    /// Current data handler
    /// </summary>
    IDataHandler DataHandler { get; }

    /// <summary>
    /// Delegate to send an email
    /// </summary>
    SendMailDelegate SendMailDelagate { get; set; }

    /// <summary>
    /// Get all jobs for payments to be done from different target databases
    /// </summary>
    void StartJobCollector();

    /// <summary>
    /// Send a test mail: mail is used to in form admin users regarding errors
    /// </summary>
    void SendTestMail();

    /// <summary>
    /// Run all the workload 
    /// </summary>
    void ProcessAllWorkloads();

    /// <summary>
    /// Save the safekeeping items to a JSON file
    /// </summary>
    void SaveSafeKeepingsFile();

    /// <summary>
    /// Send all error mails
    /// </summary>
    void SendErrorMails();

    /// <summary>
    /// Send an important message
    /// </summary>
    /// <param name="message">Message to send</param>
    void SetImportantMessage(PaymentMessage message);
}