// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using Bodoconsult.App.Abstractions.Delegates;
using Bodoconsult.Finance.Payment.Base;
using Bodoconsult.Finance.Payment.Base.Interfaces;
using Bodoconsult.Finance.Payment.Base.Models;

namespace BodoMoneyTransfer.Dal.Services;

/// <summary>
/// FinApi payment service
/// </summary>
public class FinApiService : IPaymentService
{
    /// <summary>
    /// Current status message handler
    /// </summary>
    public StatusMessageDelegate StatusMessageDelegate { get; set; }

    /// <summary>
    /// Current status message handler for important messages
    /// </summary>
    public ShowPaymentMessageDelegate ImportantMessageChange { get; set; }

    /// <inheritdoc />
    public bool OpenContact()
    {
        // do nothing
        return true;
    }

    public void CloseContact()
    {
        // do nothing
    }

    public PaymentMessage CreatePayment(IPaymentOrder order)
    {
        throw new NotImplementedException();
    }

    public PaymentMessage CreatePayment(IPaymentOrder order, string cancelUrl, string returnUrl)
    {
        throw new NotImplementedException();
    }

    public PaymentMessage ConfirmPayment(string token, string payerId)
    {
        throw new NotImplementedException();
    }

    public PaymentMessage TransferVopOutOut(IEnumerable<IPaymentOrder> transactions)
    {
        throw new NotImplementedException();
    }

    public PaymentMessage TransferVop(IEnumerable<IPaymentOrder> transactions)
    {
        throw new NotImplementedException();
    }


    ///// <summary>
    ///// Get all found accounts
    ///// </summary>
    ///// <returns></returns>
    //public IList<Account> GetAccounts()
    //{
    //    throw new NotImplementedException();
    //}


    public decimal GetBalance()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Is there a certain account
    /// </summary>
    /// <param name="iban">IBAN of the account to check existence for</param>
    /// <returns>true if the account exists</returns>
    public bool IsAccount(string iban)
    {

        throw new NotImplementedException();
    }


    public string GetOwner()
    {
        throw new NotImplementedException();

    }

    public AccountStatement GetHistory(DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException();

    }

    public void GetHistoryToMt940File(DateTime startDate, DateTime endDate, string targetFileName)
    {
        GetHistoryToMt940File(startDate, endDate, targetFileName, false);
    }

    public void GetHistoryToMt940File(DateTime startDate, DateTime endDate, string targetFileName, bool createPdfAccountStatement)
    {

        throw new NotImplementedException();
    }



    public IList<AccountStatementFile> GetOfficialAccountStatementFiles()
    {
        throw new NotImplementedException();
    }

    public bool GetOfficialAccountStatementFile(AccountStatementFile fileData, bool sendReceipt, string fileName)
    {
        throw new NotImplementedException();
    }

    public IList<SafekeepingItem> GetDepot()
    {
        throw new NotImplementedException();

    }

    public string DownloadPtk()
    {
        throw new NotImplementedException();
    }

    private void HandleException(Exception e)
    {
        var pm = new PaymentMessage
        {
            ErrorCode = 9010,
            Message = $"Payment contact needs Webform approval: {e.Message}: {PaymentContact.ToReadableString()}"
        };
        SetImportantMessage(pm);

        throw new Exception("9010", e);
    }



    public IPaymentContact PaymentContact
    {
        get => null;
        set
        {
                


        }
    }

    /// <summary>
    /// Statusanzeige
    /// </summary>
    /// <param name="msg"></param>
    private void SetStatus(string msg)
    {
        StatusMessageDelegate?.Invoke(msg);
    }


    /// <summary>
    /// Statusanzeige
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="values"></param>
    private void SetStatus(string msg, params object[] values)
    {
        StatusMessageDelegate?.Invoke(string.Format(msg, values));
    }

    /// <summary>
    /// Statusanzeige
    /// </summary>
    /// <param name="message"></param>
    private void SetImportantMessage(PaymentMessage message)
    {
        ImportantMessageChange?.Invoke(message);
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        // TODO release managed resources here
    }
}