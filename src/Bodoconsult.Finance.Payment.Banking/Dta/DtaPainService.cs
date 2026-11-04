//// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


//using System;
//using System.Collections.Generic;
//using System.Runtime.Versioning;
//using Bodoconsult.Finance.Payment.Banking.Hbci;
//using Bodoconsult.Finance.Payment.Banking.Models;
//using Bodoconsult.Finance.Payment.Base;
//using Bodoconsult.Finance.Payment.Base.Interfaces;
//using Bodoconsult.Finance.Payment.Base.Models;

//namespace Bodoconsult.Finance.Payment.Banking.Dta;

//[SupportedOSPlatform("windows")]
//public class DtaPainService: IPaymentService
//{
//    public bool OpenContact()
//    {
//        throw new NotImplementedException();
//    }

//    public void CloseContact()
//    {
//        throw new NotImplementedException();
//    }

//    public PaymentMessage CreatePayment(IPaymentOrder order)
//    {
//        throw new NotImplementedException();
//    }

//    public PaymentMessage CreatePayment(IPaymentOrder order, string cancelUrl, string returnUrl)
//    {
//        throw new NotImplementedException();
//    }

//    public PaymentMessage ConfirmPayment(string token, string payerId)
//    {
//        throw new NotImplementedException();
//    }

//    public PaymentMessage TransferVopOutOut(IEnumerable<IPaymentOrder> transactions)
//    {
//        // ToDo: Filename
//        var x = new PainFile
//        {
//            MyBic = _contact.Bic,
//            MyIban = _contact.Iban,
//            MyName = _contact.Name,
//            Transactions = (IEnumerable<BankingOrder>) transactions,
//            TransferDate = DateTime.Now,
//            FileName = "blabla"
//        };

//        x.CreateFile();

//        return new PaymentMessage();
//    }

//    /// <summary>
//    /// Not implemented
//    /// </summary>
//    /// <returns></returns>
//    public decimal GetBalance()
//    {
//        throw new NotImplementedException();
//    }

//    public string GetOwner()
//    {
//        throw new NotImplementedException();
//    }

//    public IList<AccountStatement> GetHistory(DateTime startDate, DateTime endDate)
//    {
//        throw new NotImplementedException();
//    }

//    public void GetHistoryToMt940File(DateTime startDate, DateTime endDate, string targetFileName)
//    {
//        throw new NotImplementedException();
//    }

//    public void GetHistoryToMt940File(DateTime startDate, DateTime endDate, string targetFileName, bool createPdfAccountStatement)
//    {
//        throw new NotImplementedException();
//    }

//    public IList<AccountStatementFile> GetOfficialAccountStatementFiles()
//    {
//        throw new NotImplementedException();
//    }

//    public bool GetOfficialAccountStatementFile(AccountStatementFile fileData, bool sendReceipt, string fileName)
//    {
//        throw new NotImplementedException();
//    }

//    public IList<SafekeepingItem> GetDepot()
//    {
//        throw new NotImplementedException();
//    }

//    public void DownloadPtk()
//    {
//        throw new NotImplementedException();
//    }

//    private HbciBankingContact _contact;


//    public IPaymentContact PaymentContact
//    {
//        get => _contact;
//        set => _contact = (HbciBankingContact)value;
//    }

//    /// <summary>
//    /// Current status message handler
//    /// </summary>
//    public event StatusHandlerDelegate StatusChange;

//    /// <summary>
//    /// Current status message handler for important messages
//    /// </summary>
//    public event ShowMessageDelegate ImportantMessageChange;

//    /// <summary>
//    /// Statusanzeige
//    /// </summary>
//    /// <param name="msg"></param>
//    private void SetStatus(string msg)
//    {
//        var x = StatusChange;
//        x?.Invoke(msg);
//    }

//}