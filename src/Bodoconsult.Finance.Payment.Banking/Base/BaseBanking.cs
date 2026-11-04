// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.Finance.Payment.Banking.Ebics;
using Bodoconsult.Finance.Payment.Banking.Hbci;
using Bodoconsult.Finance.Payment.Banking.Interfaces;
using Bodoconsult.Finance.Payment.Banking.Models;
using Bodoconsult.Finance.Payment.Base;
using Bodoconsult.Finance.Payment.Base.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;

namespace Bodoconsult.Finance.Payment.Banking.Base;

/// <summary>
/// Base classs for <see cref="IBanking"/> derived classes
/// </summary>
[SupportedOSPlatform("windows")]
public class BaseBanking: IBanking
{
    /// <summary>
    /// Error code or 0
    /// </summary>
    public int Error { get; internal set; }

    /// <summary>
    /// Message of the last error
    /// </summary>
    public string ErrorMessage { get; internal set; }

    /// <summary>
    /// Banking account name according to Homebanking Administrator app
    /// </summary>
    public string Contact { get; set; }

    /// <summary>
    /// Passphrase for opening key files. If a <see cref="Pin"/> is provided the passphrase is ignored
    /// </summary>
    public string Passphrase { internal get; set; }

    /// <summary>
    /// Bank internal account number
    /// </summary>
    public string AccountNr { get; set; }

    /// <summary>
    /// PIN for the account
    /// </summary>
    public string Pin { get; set; }

    /// <summary>
    /// IBAN international banking account code
    /// </summary>
    public string Iban { get; set; }

    /// <summary>
    /// Banking username
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// HBCI message received delegate
    /// </summary>
    public ShowPaymentMessageDelegate OnHbciMessage { get; set; }

    /// <summary>
    /// Open connection to the bank via Kontonummer
    /// </summary>
    public virtual PaymentMessage OpenAccount()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Open connection to the bank via IBAN
    /// </summary>
    public virtual PaymentMessage OpenAccountSepa()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Get the balance of an account
    /// </summary>
    /// <returns>balance amount</returns>
    public virtual decimal GetBalance()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Get the all balances of the contact
    /// </summary>
    /// <returns>balance amount</returns>
    public virtual IList<AccountBalanceData> GetContactBalances()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Get the owner of the account 
    /// </summary>
    /// <returns>Account owner's name</returns>
    public virtual string GetOwner()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Get account statement for the last year (or at least as long as possible)
    /// </summary>
    public virtual AccountStatement GetAccountStatement()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Get the account statment for a defined period
    /// </summary>
    /// <param name="startDate">start date</param>
    /// <param name="endDate">end date</param>
    /// <returns>list of account statement rows</returns>
    public virtual AccountStatement GetAccountStatement(DateTime startDate, DateTime endDate)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Get account statement and save it as MT940 file with sta extension. 
    /// Additonally create PDF file with account statement
    /// Extension is added automatically with not existing
    /// </summary>
    /// <param name="startDate">start date</param>
    /// <param name="endDate">end date</param>
    /// <param name="targetFile">full path of the target PDF file</param>
    public virtual bool GetAccountStatementToMt940File(DateTime startDate, DateTime endDate, string targetFile)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Get account statement and save it as MT940 file with sta extension. 
    /// Additonally create PDF file with account statement
    /// Extension is added automatically with not existing
    /// </summary>
    /// <param name="startDate">start date</param>
    /// <param name="endDate">end date</param>
    /// <param name="targetFile">full path of the target PDF file</param>
    /// <param name="pdf">create PDF: true or false</param>
    public virtual bool GetAccountStatementToMt940File(DateTime startDate, DateTime endDate, string targetFile, bool pdf)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Load an MT940 file as accoutn statement
    /// </summary>
    /// <param name="targetFile">Path of the MT940 file</param>
    /// <param name="header">Account statement header</param>
    /// <returns>True of success else false</returns>
    public virtual bool LoadMt940FileAsCreateAccountStatement(string targetFile, AccountStatementHeader header)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Transfer multiple transactions for one account using VOP opt-out option
    /// </summary>
    /// <param name="transferData">Transactions to transfer</param>
    /// <param name="fromBic">BIC the money is transferred from</param>
    /// <returns>A payment message indicating the success of the transfers</returns>
    public virtual PaymentMessage TransferMultipleTransactionsVopOptOut(IEnumerable<BankingOrder> transferData, string fromBic)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Transfer multiple transactions for one account using VOP
    /// </summary>
    /// <param name="transferData">Transactions to transfer</param>
    /// <param name="fromBic">BIC the money is transferred from</param>
    /// <returns>A payment message indicating the success of the transfers</returns>
    public virtual PaymentMessage TransferMultipleTransactionsVop(IEnumerable<BankingOrder> transferData, string fromBic)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Download a PTK message
    /// </summary>
    /// <returns>PTK message</returns>
    public virtual string DownloadPtk()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Download Auftragsart VPZ (VOP). Get Orderid, State and CreditorName
    /// </summary>
    /// <param name="result">List with VOP processing relevante data</param>
    /// <returns>Payment message</returns>
    public virtual PaymentMessage DownloadVpz(List<VopProcessingData> result)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Download Auftragsart HVD (VEU)
    /// </summary>
    public virtual PaymentMessage DownloadHvd(VopProcessingData data)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Download Auftragsart HVU (VEU)
    /// </summary>
    /// <param name="result">List of order data</param>
    /// <returns>Payment message</returns>
    public virtual PaymentMessage DownloadHvu(List<VopProcessingData> result)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Send HVE to bank to confirm money transfer
    /// </summary>
    /// <param name="data">Order data</param>
    /// <returns>Payment message</returns>
    public virtual PaymentMessage UploadHve(VopProcessingData data)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Send HVS to bank to cancel (Storno) money transfer
    /// </summary>
    /// <param name="data">Order data</param>
    /// <returns>Payment message</returns>
    public virtual PaymentMessage UploadHvs(VopProcessingData data)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Transfer received HBCI messages
    /// </summary>
    /// <param name="msg">Payment message received from HBCI</param>
    public void ShowHbciMessage(PaymentMessage msg)
    {
        OnHbciMessage?.Invoke(msg);
    }

    /// <summary>
    /// Quit banking module
    /// </summary>
    public virtual void Quit()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Leere HBCI-Nachricht initialisieren
    /// </summary>
    /// <returns></returns>
    internal static PaymentMessage EmptyMessage()
    {
        return new PaymentMessage { ErrorCode = 0, Message = "keine besonderen Vorkommnisse" };
    }

    /// <summary>
    /// Show an not HBCI related error
    /// </summary>
    /// <param name="ex">Current exception</param>
    /// <param name="msg">Additional message</param>
    /// <returns>Payment message</returns>
    internal static PaymentMessage NoHbciError(Exception ex, string msg = null)
    {
        return new PaymentMessage { ErrorCode = -99, Message = $"C#-Fehler: {msg}: {ex.Source} {ex.Message} "};
    }

    /// <summary>
    /// Show an HBCI related error
    /// </summary>
    /// <param name="ex">Current exception</param>
    /// <param name="msg">Additional message</param>
    /// <returns>Payment message</returns>
    internal static PaymentMessage HbciError(Exception ex, string msg = null)
    {
        return new PaymentMessage { ErrorCode = 9000, Message = $"HBCI-Fehler: {msg}: {ex.Source} {ex.Message} " };
    }

    /// <summary>
    /// Show an HBCI related error
    /// </summary>
    /// <param name="msg">Additional message</param>
    /// <returns>Payment message</returns>
    internal static PaymentMessage HbciError(string msg = null)
    {
        return new PaymentMessage { ErrorCode = 9000, Message = $"HBCI-Fehler: {msg}" };
    }

    /// <summary>
    /// Get all banking contacts on the current computer
    /// </summary>
    /// <returns></returns>
    public IList<string> GetBankingContacts()
    {
        return Ddbac.GetBankingContacts();
    }

    /// <summary>
    /// Get all banking contacts on the current system for the current user
    /// </summary>
    /// <returns></returns>
    public virtual IList<BankingAccount> GetBankingAccounts(string contactName, string password)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Show the administration window for HBCI accounts
    /// </summary>
    public virtual void ShowAdministrationWindow()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Show the window for new HBCI contact
    /// </summary>
    public void ShowNewContactWindow()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Get the list of securities items in a safekeeping account
    /// </summary>
    /// <returns>list of securities items</returns>
    public virtual IList<SafekeepingItem> GetDepot()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Get all official account statement files from the bank
    /// </summary>
    /// <returns>List with account statement files</returns>
    public virtual IList<AccountStatementFile> GetOfficialAccountStatementFiles()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Get a concrete account statement file
    /// </summary>
    /// <param name="fileData">Current metadata of an account statement file</param>
    /// <param name="sendReceipt">Send a receipt if possible and necessary</param>
    /// <param name="fileName">File name to save the file in</param>
    /// <returns>file data</returns>
    public virtual bool GetOfficialAccountStatementFile(AccountStatementFile fileData, bool sendReceipt, string fileName)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Clear a SEPA string from unallowed chars
    /// </summary>
    /// <param name="value">String to check</param>
    /// <returns>Checked string</returns>
    protected static string ClearSepaString(string value)
    {
        value =
            value.Replace("Ö", "oe", StringComparison.OrdinalIgnoreCase)
                .Replace("ö", "oe", StringComparison.OrdinalIgnoreCase)
                .Replace("Ä", "Ae", StringComparison.OrdinalIgnoreCase)
                .Replace("ä", "ae", StringComparison.OrdinalIgnoreCase)
                .Replace("Ü", "Ue", StringComparison.OrdinalIgnoreCase)
                .Replace("ü", "ue", StringComparison.OrdinalIgnoreCase)
                .Replace("ß", "ss", StringComparison.OrdinalIgnoreCase)
                .Replace(".", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);

        return value;
    }

    /// <summary>
    /// Create a PDF file from an account statement file
    /// </summary>
    /// <param name="statement">Current account statement</param>
    /// <param name="pdfFile">Name of the PDF file to the accoutn statement in</param>
    /// <param name="header">Account statement header</param>
    protected static void CreateAccountStatementAsPdf(AccountStatement statement, string pdfFile, AccountStatementHeader header)
    {
        if (File.Exists(pdfFile))
        {
            File.Delete(pdfFile);
        }

        var data = statement.Items;

        header.StartBalanceDate = statement.OpeningBalanceDate;
        header.StartBalanceAmount = statement.OpeningBalanceAmount;
        header.StartBalanceCurrency = statement.OpeningBalanceCurrency;
        header.EndBalanceDate = statement.ClosingBalanceDate;
        header.EndBalanceAmount = statement.ClosingBalanceAmount;
        header.EndBalanceCurrency = statement.ClosingBalanceCurrency;

        var pdf = new PdfAccountStatement(data, header, pdfFile);
        pdf.CreateFile();
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // TODO release managed resources here
        }
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}