// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.Finance.Payment.Banking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Bodoconsult.App.Abstractions.Delegates;
using Bodoconsult.Finance.Payment.Banking.Ebics;
using Bodoconsult.Finance.Payment.Banking.Hbci;
using Bodoconsult.Finance.Payment.Banking.Models;
using Bodoconsult.Finance.Payment.Base;
using Bodoconsult.Finance.Payment.Base.Interfaces;
using Bodoconsult.Finance.Payment.Base.Models;

namespace Bodoconsult.Finance.Payment.Banking.Dummy;

/// <summary>
/// Dummy implementation for <see cref="IBankingService"/> for unit testing
/// </summary>
public class DummyBankingService: IBankingService
{
    /// <summary>
    /// Current <see cref="IPaymentContact"/> object to work with
    /// </summary>
    public IPaymentContact PaymentContact { get; set; }

    /// <summary>
    /// Current status message handler
    /// </summary>
    public StatusMessageDelegate StatusMessageDelegate { get; set; }

    /// <summary>
    /// Current status message handler for important messages
    /// </summary>
    public ShowPaymentMessageDelegate ImportantMessageChange { get; set; }

    /// <summary>
    /// Was <see cref="UploadHve"/> fired?
    /// </summary>
    public bool WasUploadHveFired { get; set; }

    /// <summary>
    /// Was <see cref="UploadHvs"/> fired?
    /// </summary>
    public bool WasUploadHvsFired { get; set; }

    /// <summary>
    /// Open the payment contact
    /// </summary>
    /// <returns>true on success else false</returns>
    public bool OpenContact()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Close the payment contact
    /// </summary>
    public void CloseContact()
    {
        throw new NotImplementedException();
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
    /// Transfer payment transactions using VOP opt-out
    /// </summary>
    /// <param name="transactions">list of transaction to transfer to the bank or webservice</param>
    /// <returns><see cref="PaymentMessage"/> object with information regarding the success of the transaction</returns>
    public PaymentMessage TransferVopOutOut(IEnumerable<IPaymentOrder> transactions)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Transfer payment transactions using VOP name check
    /// </summary>
    /// <param name="transactions">list of transaction to transfer to the bank or webservice</param>
    /// <returns><see cref="PaymentMessage"/> object with information regarding the success of the transaction</returns>
    public PaymentMessage TransferVop(IEnumerable<IPaymentOrder> transactions)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get the balance of an account
    /// </summary>
    /// <returns>balance amount</returns>
    public decimal GetBalance()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get the owner of the account 
    /// </summary>
    /// <returns>Account owner's name</returns>
    public string GetOwner()
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get account statement as MT 940
    /// </summary>
    /// <param name="startDate">start date</param>
    /// <param name="endDate">end date</param>
    /// <param name="targetFileName">path to save MT940 file (.sta extension will be added if not existing)</param>
    public void GetHistoryToMt940File(DateTime startDate, DateTime endDate, string targetFileName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get account statement as MT 940
    /// </summary>
    /// <param name="startDate">start date</param>
    /// <param name="endDate">end date</param>
    /// <param name="targetFileName">path to save MT940 file (.sta extension will be added if not existing)</param>
    /// <param name="createPdfAccountStatement">Create additionally PDF account statement</param>
    public void GetHistoryToMt940File(DateTime startDate, DateTime endDate, string targetFileName, bool createPdfAccountStatement)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get all official account statement files from the bank
    /// </summary>
    /// <returns></returns>
    public IList<AccountStatementFile> GetOfficialAccountStatementFiles()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get a concrete account statement file
    /// </summary>
    /// <param name="fileData">Metadata for the account statement file </param>
    /// <param name="sendReceipt">Send a receipt if possible and necessary</param>
    /// <param name="fileName">File name to save the file in</param>
    /// <returns>file data</returns>
    public bool GetOfficialAccountStatementFile(AccountStatementFile fileData, bool sendReceipt, string fileName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get the list of securities items in a safekeeping account
    /// </summary>
    /// <returns>list of securities items</returns>
    public IList<SafekeepingItem> GetDepot()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Download a PTK message
    /// </summary>
    /// <returns>PTK message</returns>
    public string DownloadPtk()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Load an MT940 data file and create a PDF account statement for it
    /// </summary>
    /// <param name="targetFile">MT940 file</param>
    /// <param name="header"></param>
    /// <returns></returns>
    public bool LoadMt940FileAsCreateAccountStatement(string targetFile, AccountStatementHeader header)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Load an MT940 data file and create a PDF account statement for it
    /// </summary>
    /// <param name="targetFile">MT940 file</param>
    /// <returns></returns>
    public bool LoadMt940FileAsCreateAccountStatement(string targetFile)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get all banking contacts on the current system for the current user
    /// </summary>
    /// <returns></returns>
    public IList<string> GetBankingContacts()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get all banking contacts on the current system for the current user
    /// </summary>
    /// <returns></returns>
    public IList<BankingAccount> GetBankingAccounts(string contactName, string password)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Open the administration window for HBCI contacts
    /// </summary>
    public void ShowAdministrationWindow()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Open the new HBCI dialog
    /// </summary>
    public void ShowNewContactWindow()
    {
        throw new NotImplementedException();
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
    /// VopData for <see cref="DownloadVpz"/>
    /// </summary>
    public List<VopProcessingData> VopData { get; set; }

    /// <summary>
    /// Get the all balances of the contact
    /// </summary>
    /// <returns>balance amount</returns>
    public IList<AccountBalanceData> GetContactBalances()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Download Auftragsart VPZ (VOP). Get Orderid, State and CreditorName
    /// </summary>
    /// <param name="result">List with VOP processing relevante data</param>
    /// <returns>True on success else false</returns>
    public bool DownloadVpz(List<VopProcessingData> result)
    {
        foreach (var vopProcessingData in result)
        {
            var vopData = VopData.FirstOrDefault(x => x.EndToEndId == vopProcessingData.EndToEndId);

            if (vopData == null)
            {
                continue;
            }

            vopProcessingData.OrderId = vopData.OrderId;
            vopProcessingData.CreditorName = vopData.CreditorName;
            vopProcessingData.State = vopData.State;
        }

        return true;
    }

    /// <summary>
    /// Download Auftragsart HVD (VEU)
    /// </summary>
    /// <param name="data">Order data</param>
    /// <returns>True on success else false</returns>
    public bool DownloadHvd(VopProcessingData data)
    {
        var vopData = VopData.FirstOrDefault(x => x.EndToEndId == data.EndToEndId);

        if (vopData == null)
        {
            return false;
        }

        data.DataDigest = vopData.DataDigest;

        return true;
    }

    /// <summary>
    /// Download Auftragsart HVU (VEU)
    /// </summary>
    /// <param name="result">List of order data</param>
    /// <returns>True on success else false</returns>
    public bool DownloadHvu(List<VopProcessingData> result)
    {
        foreach (var vopProcessingData in result)
        {
            var vopData = VopData.FirstOrDefault(x => x.EndToEndId == vopProcessingData.EndToEndId);

            if (vopData == null)
            {
                continue;
            }

            vopProcessingData.OrderType = vopData.OrderType;
            vopProcessingData.PartnerId = vopData.PartnerId;
        }

        return true;
    }

    /// <summary>
    /// Send HVE to bank to confirm money transfer
    /// </summary>
    /// <param name="data">Order data</param>
    /// <returns>True on success else false</returns>
    public bool UploadHve(VopProcessingData data)
    {
        // Do nothing
        WasUploadHveFired = true;
        return true;
    }

    /// <summary>
    /// Send HVS to bank to cancel (Storno) money transfer
    /// </summary>
    /// <param name="data">Order data</param>
    /// <returns>True on success else false</returns>
    public bool UploadHvs(VopProcessingData data)
    {
        // Do nothing
        WasUploadHvsFired = true;
        return true;
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        // TODO release managed resources here
    }
}