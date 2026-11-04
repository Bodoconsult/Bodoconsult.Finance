// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.Finance.Payment.Banking.Ebics;
using Bodoconsult.Finance.Payment.Banking.Hbci;
using Bodoconsult.Finance.Payment.Banking.Models;
using Bodoconsult.Finance.Payment.Base;
using Bodoconsult.Finance.Payment.Base.Models;
using System;
using System.Collections.Generic;

namespace Bodoconsult.Finance.Payment.Banking.Interfaces;

/// <summary>
/// Interface for basic banking services
/// </summary>
public interface IBanking: IDisposable
{
    /// <summary>
    /// Error code or 0
    /// </summary>
    int Error { get; }

    /// <summary>
    /// Message of the last error
    /// </summary>
    string ErrorMessage { get; }

    /// <summary>
    /// Banking account name according to Homebanking Administrator app
    /// </summary>
    string Contact { get; set; }

    /// <summary>
    /// Passphrase for opening key files. If a <see cref="Pin"/> is provided the passphrase is ignored
    /// </summary>
    string Passphrase { set; }

    /// <summary>
    /// Bank internal account number
    /// </summary>
    string AccountNr { get; set; }

    /// <summary>
    /// PIN for the account
    /// </summary>
    string Pin { get; set; }

    /// <summary>
    /// IBAN international banking account code
    /// </summary>
    string Iban { get; set; }

    /// <summary>
    /// Banking username
    /// </summary>
    string UserName { get; set; }
    
    /// <summary>
    /// HBCI message received delegate
    /// </summary>
    ShowPaymentMessageDelegate OnHbciMessage { get; set; }

    /// <summary>
    /// Verbindung zur Bank öffnen
    /// </summary>
    PaymentMessage OpenAccount();

    /// <summary>
    /// Verbindung zur Bank öffnen
    /// </summary>
    PaymentMessage OpenAccountSepa();

    /// <summary>
    /// Get the available balance of an account
    /// </summary>
    /// <returns>balance amount</returns>
    decimal GetBalance();

    /// <summary>
    /// Get the all balances of the contact
    /// </summary>
    /// <returns>balance amount</returns>
    IList<AccountBalanceData> GetContactBalances();

    /// <summary>
    /// Get the owner of the account 
    /// </summary>
    /// <returns>Account owner's name</returns>
    string GetOwner();

    /// <summary>
    /// Get account statement for the last year (or at least as long as possible)
    /// </summary>
    AccountStatement GetAccountStatement();

    /// <summary>
    /// Get account statement
    /// </summary>
    /// <param name="startDate">start date</param>
    /// <param name="endDate">start</param>
    AccountStatement GetAccountStatement(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Get account statement and save it as MT940 file with sta extension. 
    /// Extension is added automatically with not existing
    /// </summary>
    /// <param name="startDate">start date</param>
    /// <param name="endDate">start</param>
    /// <param name="targetFile">full path of target MT940 file</param>
    bool GetAccountStatementToMt940File(DateTime startDate, DateTime endDate, string targetFile);

    /// <summary>
    /// Get account statement and save it as MT940 file with sta extension. 
    /// Additonally create PDF file with account statement
    /// Extension is added automatically with not existing
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="targetFile"></param>
    /// <param name="pdf"></param>
    bool GetAccountStatementToMt940File(DateTime startDate, DateTime endDate, string targetFile, bool pdf);

    /// <summary>
    /// Load an MT940 file as accoutn statement
    /// </summary>
    /// <param name="targetFile">Path of the MT940 file</param>
    /// <param name="header">Account statement header</param>
    /// <returns>True of success else false</returns>
    bool LoadMt940FileAsCreateAccountStatement(string targetFile, AccountStatementHeader header);

    /// <summary>
    /// Transfer multiple transactions for one account using VOP opt-out option
    /// </summary>
    /// <param name="transferData">Transactions to transfer</param>
    /// <param name="fromBic">BIC the money is transferred from</param>
    /// <returns>A payment message indicating the success of the transfers</returns>
    PaymentMessage TransferMultipleTransactionsVopOptOut(IEnumerable<BankingOrder> transferData, string fromBic);

    /// <summary>
    /// Transfer multiple transactions for one account using VOP
    /// </summary>
    /// <param name="transferData">Transactions to transfer</param>
    /// <param name="fromBic">BIC the money is transferred from</param>
    /// <returns>A payment message indicating the success of the transfers</returns>
    PaymentMessage TransferMultipleTransactionsVop(IEnumerable<BankingOrder> transferData, string fromBic);

    /// <summary>
    /// Download a PTK message
    /// </summary>
    /// <returns>PTK message</returns>
    public string DownloadPtk();

    /// <summary>
    /// Download Auftragsart VPZ (VOP). Get Orderid, State and CreditorName
    /// </summary>
    /// <param name="result">List with VOP processing relevante data</param>
    /// <returns>Payment message</returns>
    public PaymentMessage DownloadVpz(List<VopProcessingData> result);

    /// <summary>
    /// Download Auftragsart HVD (VEU)
    /// </summary>
    public PaymentMessage DownloadHvd(VopProcessingData data);

    /// <summary>
    /// Download Auftragsart HVU (VEU)
    /// </summary>
    /// <param name="result">List of order data</param>
    /// <returns>Payment message</returns>
    public PaymentMessage DownloadHvu(List<VopProcessingData> result);

    /// <summary>
    /// Send HVE to bank to confirm money transfer
    /// </summary>
    /// <param name="data">Order data</param>
    /// <returns>Payment message</returns>
    public PaymentMessage UploadHve(VopProcessingData data);

    /// <summary>
    /// Send HVS to bank to cancel (Storno) money transfer
    /// </summary>
    /// <param name="data">Order data</param>
    /// <returns>Payment message</returns>
    public PaymentMessage UploadHvs(VopProcessingData data);

    /// <summary>
    /// Transfer received HBCI messages
    /// </summary>
    /// <param name="msg">Payment message received from HBCI</param>
    void ShowHbciMessage(PaymentMessage msg);

    /// <summary>
    /// Quit banking module
    /// </summary>
    void Quit();

    /// <summary>
    /// Get all banking contacts on the current computer
    /// </summary>
    /// <returns></returns>
    IList<string> GetBankingContacts();

    /// <summary>
    /// Get all banking contacts on the current system for the current user
    /// </summary>
    /// <returns></returns>
    IList<BankingAccount> GetBankingAccounts(string contactName, string password);

    /// <summary>
    /// Show the administration window for HBCI accounts
    /// </summary>
    void ShowAdministrationWindow();

    /// <summary>
    /// Show the window for new HBCI contact
    /// </summary>
    void ShowNewContactWindow();

    /// <summary>
    /// Get the list of securities items in a safekeeping account
    /// </summary>
    /// <returns>list of securities items</returns>
    IList<SafekeepingItem> GetDepot();

    /// <summary>
    /// Get all official account statement files from the bank
    /// </summary>
    /// <returns></returns>
    IList<AccountStatementFile> GetOfficialAccountStatementFiles();

    /// <summary>
    /// Get a concrete account statement file
    /// </summary>
    /// <param name="fileData">Current metadata of an account statement file</param>
    /// <param name="sendReceipt">Send a receipt if possible and necessary</param>
    /// <param name="fileName">File name to save the file in</param>
    /// <returns>file data</returns>
    bool GetOfficialAccountStatementFile(AccountStatementFile fileData, bool sendReceipt, string fileName);
}