// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System;
using System.Collections.Generic;
using Bodoconsult.App.Abstractions.Delegates;
using Bodoconsult.Finance.Payment.Base.Models;

namespace Bodoconsult.Finance.Payment.Base.Interfaces;

/// <summary>
/// Top level interface for the payment handling
/// </summary>
public interface IPaymentService: IDisposable
{
    /// <summary>
    /// Current <see cref="IPaymentContact"/> object to work with
    /// </summary>
    IPaymentContact PaymentContact { get; set; }

    /// <summary>
    /// Current status message handler delegate
    /// </summary>
    StatusMessageDelegate StatusMessageDelegate { get; set; }

    /// <summary>
    /// Current status message handler for important messages
    /// </summary>
    ShowPaymentMessageDelegate ImportantMessageChange { get; set; }

    /// <summary>
    /// Open the payment contact
    /// </summary>
    /// <returns>true on success else false</returns>
    bool OpenContact();

    /// <summary>
    /// Close the payment contact
    /// </summary>
    void CloseContact();

    /// <summary>
    /// Create a payment 
    /// </summary>
    /// <param name="order">transaction data</param>
    /// <returns><see cref="PaymentMessage"/> object with information regarding the success of the transaction</returns>
    PaymentMessage CreatePayment(IPaymentOrder order);

    /// <summary>
    /// Create a payment 
    /// </summary>
    /// <param name="order">transaction data</param>
    /// <param name="cancelUrl">url for cancelling the transaction</param>
    /// <param name="returnUrl">url for return target from the transaction</param>
    /// <returns><see cref="PaymentMessage"/> object with information regarding the success of the transaction</returns>
    PaymentMessage CreatePayment(IPaymentOrder order, string cancelUrl, string returnUrl);

    /// <summary>
    /// Confirm a payment 
    /// </summary>
    /// <param name="token">token of the transaction</param>
    /// <param name="payerId">ID of the payer</param>
    /// <returns><see cref="PaymentMessage"/> object with information regarding the success of the transaction</returns>
    PaymentMessage ConfirmPayment(string token, string payerId);

    /// <summary>
    /// Transfer payment transactions using VOP opt-out
    /// </summary>
    /// <param name="transactions">list of transaction to transfer to the bank or webservice</param>
    /// <returns><see cref="PaymentMessage"/> object with information regarding the success of the transaction</returns>
    PaymentMessage TransferVopOutOut(IEnumerable<IPaymentOrder> transactions);

    /// <summary>
    /// Transfer payment transactions using VOP name check
    /// </summary>
    /// <param name="transactions">list of transaction to transfer to the bank or webservice</param>
    /// <returns><see cref="PaymentMessage"/> object with information regarding the success of the transaction</returns>
    PaymentMessage TransferVop(IEnumerable<IPaymentOrder> transactions);

    /// <summary>
    /// Get the balance of an account
    /// </summary>
    /// <returns>balance amount</returns>
    decimal GetBalance();

    /// <summary>
    /// Get the owner of the account 
    /// </summary>
    /// <returns>Account owner's name</returns>
    string GetOwner();


    /// <summary>
    /// Get an account statement 
    /// </summary>
    /// <param name="startDate">start date</param>
    /// <param name="endDate">end date</param>
    /// <returns></returns>
    AccountStatement GetHistory(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Get account statement as MT 940
    /// </summary>
    /// <param name="startDate">start date</param>
    /// <param name="endDate">end date</param>
    /// <param name="targetFileName">path to save MT940 file (.sta extension will be added if not existing)</param>
    void GetHistoryToMt940File(DateTime startDate, DateTime endDate, string targetFileName);


    /// <summary>
    /// Get account statement as MT 940
    /// </summary>
    /// <param name="startDate">start date</param>
    /// <param name="endDate">end date</param>
    /// <param name="targetFileName">path to save MT940 file (.sta extension will be added if not existing)</param>
    /// <param name="createPdfAccountStatement">Create additionally PDF account statement</param>
    void GetHistoryToMt940File(DateTime startDate, DateTime endDate, string targetFileName,
        bool createPdfAccountStatement);


    /// <summary>
    /// Get all official account statement files from the bank
    /// </summary>
    /// <returns></returns>
    IList<AccountStatementFile> GetOfficialAccountStatementFiles();

    /// <summary>
    /// Get a concrete account statement file
    /// </summary>
    /// <param name="fileData">Metadata for the account statement file </param>
    /// <param name="sendReceipt">Send a receipt if possible and necessary</param>
    /// <param name="fileName">File name to save the file in</param>
    /// <returns>file data</returns>
    bool GetOfficialAccountStatementFile(AccountStatementFile fileData, bool sendReceipt, string fileName);


    /// <summary>
    /// Get the list of securities items in a safekeeping account
    /// </summary>
    /// <returns>list of securities items</returns>
    IList<SafekeepingItem> GetDepot();

    /// <summary>
    /// Download a PTK message
    /// </summary>
    /// <returns>PTK message</returns>
    string DownloadPtk();
}