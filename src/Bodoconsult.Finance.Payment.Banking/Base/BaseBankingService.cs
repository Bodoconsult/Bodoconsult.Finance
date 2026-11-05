// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using Bodoconsult.App.Abstractions.Delegates;
using Bodoconsult.Finance.Payment.Banking.Ebics;
using Bodoconsult.Finance.Payment.Banking.Hbci;
using Bodoconsult.Finance.Payment.Banking.Interfaces;
using Bodoconsult.Finance.Payment.Banking.Models;
using Bodoconsult.Finance.Payment.Base;
using Bodoconsult.Finance.Payment.Base.Interfaces;
using Bodoconsult.Finance.Payment.Base.Models;

namespace Bodoconsult.Finance.Payment.Banking.Base;

/// <summary>
/// Base class for banking services
/// </summary>
public class BaseBankingService : IBankingService
{
    private IBankingContact _contact;

    /// <summary>
    /// Current <see cref="IBanking"/> instance
    /// </summary>
    protected IBanking Banking;

    /// <summary>
    /// Current <see cref="IPaymentContact"/> object to work with
    /// </summary>
    public virtual IPaymentContact PaymentContact
    {
        get => _contact;
        set => _contact = (IBankingContact)value;
    }

    /// <summary>
    /// Current status message handler
    /// </summary>
    public StatusMessageDelegate StatusMessageDelegate { get; set; }

    /// <summary>
    /// Current status message handler for important messages
    /// </summary>
    public ShowPaymentMessageDelegate ImportantMessageChange { get; set; }

    /// <summary>
    /// FINTS-Registriernummer as product name since 1.8.2019
    /// </summary>
    public string ProductName { get; protected set; }

    /// <summary>
    /// Product version (max. length 5 chars)
    /// </summary>
    public string ProductVersion { get; protected set; }

    /// <summary>
    /// Open the payment contact
    /// </summary>
    /// <returns>true on success else false</returns>
    public bool OpenContact()
    {
        Banking?.Quit();
        GetBanking();

        if (Banking == null)
        {
            throw new Exception("Banking contact not initialized!");
        }

        Banking.OnHbciMessage = OnPaymentMessage;
        Banking.Contact = _contact.Name;
        Banking.Passphrase = _contact.Passphrase;
        Banking.AccountNr = _contact.AccountNr;
        Banking.Iban = _contact.Iban;
        Banking.UserName = _contact.AccountName;

        PaymentMessage pm;

        if (string.IsNullOrEmpty(_contact.Iban))
        {
            SetStatus("Open HBCI contact {0} for {1}", _contact.Name, _contact.AccountNr);
            pm = Banking.OpenAccount();
        }
        else
        {
            SetStatus("Open HBCI contact {0} for {1}", _contact.Name, _contact.Iban);
            pm = Banking.OpenAccountSepa();
        }

        if (pm == null)
        {
            return true;
        }

        if (pm.ErrorCode >= 9000)
        {
            SetImportantMessage(pm);
            return false;
        }

        SetStatus(pm.Message);
        return true;
    }

    /// <summary>
    /// Get the banking instance
    /// </summary>
    public virtual void GetBanking()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Close the payment contact
    /// </summary>
    public void CloseContact()
    {
        Banking.Quit();
    }


    /// <summary>
    /// Transfer payment transactions using VOP opt-out
    /// </summary>
    /// <param name="transactions">list of transaction to transfer to the bank or webservice</param>
    /// <returns><see cref="PaymentMessage"/> object with information regarding the success of the transaction</returns>
    public PaymentMessage TransferVopOutOut(IEnumerable<IPaymentOrder> transactions)
    {
        SetStatus(string.IsNullOrEmpty(_contact.Iban)
            ? $"Bank transfers for {_contact.AccountNr}"
            : $"Bank transfers for {_contact.Iban}");

        // Bic mit übergeben, solange Bic nicht von Bank geliefert wird
        return Banking.TransferMultipleTransactionsVopOptOut((IEnumerable<BankingOrder>)transactions, _contact.Bic);
    }

    /// <summary>
    /// Transfer payment transactions using VOP name check
    /// </summary>
    /// <param name="transactions">list of transaction to transfer to the bank or webservice</param>
    /// <returns><see cref="PaymentMessage"/> object with information regarding the success of the transaction</returns>
    public virtual PaymentMessage TransferVop(IEnumerable<IPaymentOrder> transactions)
    {
        return Banking.TransferMultipleTransactionsVop((IEnumerable<BankingOrder>)transactions, _contact.Iban);
    }

    /// <summary>
    /// Get the balance of an account
    /// </summary>
    /// <returns>balance amount</returns>
    public virtual decimal GetBalance()
    {
        return Banking.GetBalance();
    }


    /// <summary>
    /// Get the owner of the account 
    /// </summary>
    /// <returns>Account owner's name</returns>
    public string GetOwner()
    {
        return Banking.GetOwner();
    }

    /// <summary>
    /// Create a payment 
    /// </summary>
    /// <param name="order">transaction data</param>
    /// <returns><see cref="PaymentMessage"/> object with information regarding the success of the transaction</returns>
    public PaymentMessage CreatePayment(IPaymentOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Create a payment 
    /// </summary>
    /// <param name="order">transaction data</param>
    /// <param name="cancelUrl">url for cancelling the transaction</param>
    /// <param name="returnUrl">url for return target from the transaction</param>
    /// <returns><see cref="PaymentMessage"/> object with information regarding the success of the transaction</returns>
    public PaymentMessage CreatePayment(IPaymentOrder order, string cancelUrl, string returnUrl)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Confirm a payment 
    /// </summary>
    /// <param name="token">token of the transaction</param>
    /// <param name="payerId">ID of the payer</param>
    /// <returns><see cref="PaymentMessage"/> object with information regarding the success of the transaction</returns>
    public PaymentMessage ConfirmPayment(string token, string payerId)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Get an account statement 
    /// </summary>
    /// <param name="startDate">start date</param>
    /// <param name="endDate">end date</param>
    /// <returns></returns>
    public AccountStatement GetHistory(DateTime startDate, DateTime endDate)
    {
        return Banking.GetAccountStatement(startDate, endDate);
    }


    /// <summary>
    /// Hbci-Nachricht anzeigen
    /// </summary>
    /// <param name="error"></param>
    internal void OnPaymentMessage(PaymentMessage error)
    {
        SetStatus($"{error.ErrorCode} {error.Message}");

        // Nur wichtige Nachrichten weiterreichen
        if (error.ErrorCode <= 1000 && error.ErrorCode1 <= 0)
        {
            return;
        }

        if (error.ErrorCode != 3060)
        {
            SetImportantMessage(error);
        }
    }

    /// <summary>
    /// Statusanzeige
    /// </summary>
    /// <param name="msg"></param>
    private void SetStatus(string msg)
    {
        var x = StatusMessageDelegate;
        x?.Invoke(msg);
    }

    /// <summary>
    /// Statusanzeige
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="values"></param>
    private void SetStatus(string msg, params object[] values)
    {
        var x = StatusMessageDelegate;
        x?.Invoke(string.Format(msg, values));
    }

    /// <summary>
    /// Statusanzeige
    /// </summary>
    /// <param name="message"></param>
    private void SetImportantMessage(PaymentMessage message)
    {
        ImportantMessageChange?.Invoke(message);
    }

    /// <summary>
    /// Get account statement as MT 940
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="targetFileName">path to save MT940 file (.sta extension will be added if not existing)</param>
    public void GetHistoryToMt940File(DateTime startDate, DateTime endDate, string targetFileName)
    {
        Banking.GetAccountStatementToMt940File(startDate, endDate, targetFileName);
    }

    /// <summary>
    /// Get account statement as MT 940
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="targetFileName">path to save MT940 file (.sta extension will be added if not existing)</param>
    /// <param name="createPdfAccountStatement">Create additionally PDF account statement</param>
    public void GetHistoryToMt940File(DateTime startDate, DateTime endDate, string targetFileName, bool createPdfAccountStatement)
    {
        Banking.GetAccountStatementToMt940File(startDate, endDate, targetFileName, createPdfAccountStatement);
    }

    /// <summary>
    /// Load an MT940 data file and create a PDF account statement for it
    /// </summary>
    /// <param name="targetFile">MT940 file</param>
    /// <param name="header"></param>
    /// <returns></returns>
    public bool LoadMt940FileAsCreateAccountStatement(string targetFile, AccountStatementHeader header)
    {
        return Banking.LoadMt940FileAsCreateAccountStatement(targetFile, header);
    }

    /// <summary>
    /// Load an MT940 data file and create a PDF account statement for it
    /// </summary>
    /// <param name="targetFile">MT940 file</param>
    /// <returns></returns>
    public bool LoadMt940FileAsCreateAccountStatement(string targetFile)
    {
        return Banking.LoadMt940FileAsCreateAccountStatement(targetFile, null);
    }

    /// <summary>
    /// Get all banking contacts on the current system for the current user
    /// </summary>
    /// <returns></returns>
    public IList<string> GetBankingContacts()
    {
        if (Banking == null)
        {
            GetBanking();
        }

        return Banking == null ? 
            new List<string>() : 
            Banking.GetBankingContacts();
    }

    /// <summary>
    /// Get all banking contacts on the current system for the current user
    /// </summary>
    /// <returns></returns>
    public IList<BankingAccount> GetBankingAccounts(string contactName, string password)
    {
        if (Banking == null)
        {
            GetBanking();
        }

        return Banking == null ?
            new List<BankingAccount>() : 
            Banking.GetBankingAccounts(contactName, password);
    }


    /// <summary>
    /// Open the administration window for HBCI contacts
    /// </summary>
    public void ShowAdministrationWindow()
    {
        Banking?.Quit();
        GetBanking();

        if (Banking == null)
        {
            throw new Exception("Banking contact not initialized!");
        }
        Banking.ShowAdministrationWindow();
    }

    /// <summary>
    /// Open the new HBCI dialog
    /// </summary>
    public void ShowNewContactWindow()
    {
        Banking?.Quit();
        GetBanking();

        if (Banking == null)
        {
            throw new Exception("Banking contact not initialized!");
        }
        Banking.ShowNewContactWindow();
    }

    /// <summary>
    /// Get the all balances of the contact
    /// </summary>
    /// <returns>balance amount</returns>
    public IList<AccountBalanceData> GetContactBalances()
    {
        return Banking.GetContactBalances();
    }

    /// <summary>
    /// Download Auftragsart VPZ (VOP). Get Orderid, State and CreditorName
    /// </summary>
    /// <param name="result">List with VOP processing relevante data</param>
    /// <returns>True on success else false</returns>
    public bool DownloadVpz(List<VopProcessingData> result)
    {
        var pm = Banking.DownloadVpz(result);

        if (pm.ErrorCode == 0)
        {
            return true;
        }
        SetImportantMessage(pm);
        return false;
    }


    /// <summary>
    /// Download Auftragsart HVD (VEU)
    /// </summary>
    /// <param name="data">Order data</param>
    /// <returns>True on success else false</returns>
    public bool DownloadHvd(VopProcessingData data)
    {
        var pm = Banking.DownloadHvd(data);

        if (pm.ErrorCode == 0)
        {
            return true;
        }
        SetImportantMessage(pm);
        return false;
    }

    /// <summary>
    /// Download Auftragsart HVU (VEU)
    /// </summary>
    /// <param name="result">List of order data</param>
    /// <returns>True on success else false</returns>
    public bool DownloadHvu(List<VopProcessingData> result)
    {
        var pm = Banking.DownloadHvu(result);

        if (pm.ErrorCode == 0)
        {
            return true;
        }
        SetImportantMessage(pm);
        return false;
    }

    /// <summary>
    /// Send HVE to bank to confirm money transfer
    /// </summary>
    /// <param name="data">Order data</param>
    /// <returns>True on success else false</returns>
    public bool UploadHve(VopProcessingData data)
    {
        var pm = Banking.UploadHve(data);

        if (pm.ErrorCode == 0)
        {
            return true;
        }
        SetImportantMessage(pm);
        return false;
    }

    /// <summary>
    /// Send HVS to bank to cancel (Storno) money transfer
    /// </summary>
    /// <param name="data">Order data</param>
    /// <returns>True on success else false</returns>
    public bool UploadHvs(VopProcessingData data)
    {
        var pm = Banking.UploadHvs(data);

        if (pm.ErrorCode == 0)
        {
            return true;
        }
        SetImportantMessage(pm);
        return false;
    }

    /// <summary>
    /// Get the list of securities items in a safekeeping account
    /// </summary>
    /// <returns>list of securities items</returns>
    public IList<SafekeepingItem> GetDepot()
    {
        return Banking.GetDepot();
    }

    /// <summary>
    /// Download a PTK message
    /// </summary>
    /// <returns>PTK message</returns>
    public string DownloadPtk()
    {
        return Banking.DownloadPtk();
    }


    /// <summary>
    /// Get all official account statement files from the bank
    /// </summary>
    /// <returns></returns>
    public IList<AccountStatementFile> GetOfficialAccountStatementFiles()
    {
        return Banking.GetOfficialAccountStatementFiles();
    }


    /// <summary>
    /// Get a concrete account statement file
    /// </summary>
    /// <param name="fileData">Metadata for the account statement file </param>
    /// <param name="sendReceipt">Send a receipt if possible and necessary</param>
    /// <param name="filename">File name to save the file in</param>
    /// <returns>file data</returns>
    public bool GetOfficialAccountStatementFile(AccountStatementFile fileData,
        bool sendReceipt, string filename)
    {
        return Banking.GetOfficialAccountStatementFile(fileData, sendReceipt, filename);
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        CloseContact();
        Banking.Dispose();
    }
}