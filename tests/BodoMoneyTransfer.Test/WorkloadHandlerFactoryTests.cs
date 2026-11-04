// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.Finance.Payment.Banking.Hbci;
using BodoMoneyTransfer.Dal.Implementations;
using BodoMoneyTransfer.Dal.Interfaces;
using BodoMoneyTransfer.Dal.Model;
using BodoMoneyTransfer.Test.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;

namespace BodoMoneyTransfer.Test;

[SupportedOSPlatform("windows10.0.17763")]
[TestFixture]
public class WorkloadHandlerFactoryTests
{
    private IDataHandler _dataHandler;
    private readonly IAppLoggerProxy _appLoggerProxy = TestHelper.GetFakeAppLoggerProxy();
    private IOptions _options;
    private IWorkloadHandlerFactory _workloadHandlerFactory;

    [SetUp]
    public void SetUp()
    {
        var payments = new List<Payment>( );

        var workload = new List<WorkloadTask>();

        _options = new Options
        {
            ArchivePath = @"d:\temp\BodoMoneyTransfer\Archive",
            OriginalArchivePath = @"d:\temp\BodoMoneyTransfer\Originals",
            QuotationFileName = @"d:\temp\BodoMoneyTransfer\QuotationFile.json",
            ErrorMailer = "Test@Test.de"
        };

        _options.StatusMessageDelegate = ShowMessage;

        _dataHandler = new DummyDataHandler(_options, workload, payments);
       
        ////Globals.ImportantMessageChange += GlobalsOnImportantMessageChange;
        ////Globals.SendMailDelagate = SendMailDelagate;
        //_workloadHandler = new WorkloadHandler();
    }

    //private void GlobalsOnImportantMessageChange(PaymentMessage error)
    //{
    //    throw new NotImplementedException();
    //}

    //private void SendMailDelagate(string subject, string body)
    //{
    //    Debug.Print(subject+" "+body);
    //}

    [Test]
    public void TestGetBalanceWithoutFinApi()
    {
        // ReSharper disable once UseObjectOrCollectionInitializer
        var workload = new WorkloadTask
        {
            Iban = "DE37701696190120042374",
            Id = "9E397D34-F81F-46D0-83FC-160B3378804E",
            AccountStatementsLive = true,
            BankCode = "70169619",
            Password = "5lUtNwEAhpA=",
            Contact = "RL_57029",
            Provider = BankingServiceProvider.Hbci,

            //AccountNr = "120042734";
            AccountNr = "42734"
        };


        _workloadHandlerFactory = new WorkloadHandlerFactory(
            //null, 
            _dataHandler, _options, _appLoggerProxy);


        var workloadHandler = _workloadHandlerFactory.CreateInstance(workload);

        Assert.That(workloadHandler.PaymentService.GetType() == typeof(HbciBankingService));

        var result = workloadHandler.GetBalance();


        Assert.That(result> decimal.MinValue);

        Debug.Print($"Balance: {result}");
    }

    [Test]
    public void TestGetAccountStatementWithoutFinApi()
    {
        // ReSharper disable once UseObjectOrCollectionInitializer
        var workload = new WorkloadTask
        {
            Iban = "DE37701696190120042374",
            Id = "9E397D34-F81F-46D0-83FC-160B3378804E",
            AccountStatementsLive = true,
            BankCode = "70169619",
            Password = "5lUtNwEAhpA=",
            Contact = "RL_57029",
            Provider = BankingServiceProvider.Hbci,

            //AccountNr = "120042734";
            AccountNr = "42734",
        };

        _workloadHandlerFactory = new WorkloadHandlerFactory(
            //null, 
            _dataHandler, _options, _appLoggerProxy);

        var workloadHandler = _workloadHandlerFactory.CreateInstance(workload);

        Assert.That(workloadHandler.PaymentService.GetType() == typeof(HbciBankingService));

        var from = DateTime.Now.AddDays(-89);
        var until = DateTime.Now;


        var result = workloadHandler.GetAccountStatements(from, until);


        Assert.That(result > 0);

        Debug.Print($"Number of account statement records: {result}");
    }


    [Test]
    public void TestGetAccountStatementWithoutFinApiKeyFile()
    {
        // ReSharper disable once UseObjectOrCollectionInitializer
        var workload = new WorkloadTask
        {
            Iban = "DE29765600600000098922",
            Id = "9E397D34-F81F-46D0-83FC-160B3378804E",
            AccountStatementsLive = true,
            BankCode = "76560060",
            Password = "H8VVMRqeQzqWFlcgu6R1PIi1xJGbFRuy",
            Contact = "Rothenburg_98922File",
            Provider = BankingServiceProvider.Hbci,

            //AccountNr = "120042734";
            AccountNr = "98922",
        };

        _workloadHandlerFactory = new WorkloadHandlerFactory(
            //null, 
            _dataHandler, _options, _appLoggerProxy);


        var workloadHandler = _workloadHandlerFactory.CreateInstance(workload);

        Assert.That(workloadHandler.PaymentService.GetType() == typeof(HbciBankingService));

        var from = DateTime.Now.AddDays(-89);
        var until = DateTime.Now;


        var result = workloadHandler.GetAccountStatements(from, until);


        Assert.That(result > 0);

        Debug.Print($"Number of account statement records: {result}");
    }

    [Test]
    public void TestGetAccountStatementAsPdfWithoutFinApi()
    {
        // ReSharper disable once UseObjectOrCollectionInitializer
        var workload = new WorkloadTask
        {
            Iban = "DE37701696190120042374",
            Id = "9E397D34-F81F-46D0-83FC-160B3378804E",
            AccountStatementsLive = true,
            AccountStatementsHistoricalData = true,
            BankCode = "70169619",
            Password = "5lUtNwEAhpA=",
            Contact = "RL_57029",
            Provider = BankingServiceProvider.Hbci,

            //AccountNr = "120042734";
            AccountNr = "42734",
        };

        _workloadHandlerFactory = new WorkloadHandlerFactory(
            //null, 
            _dataHandler, _options, _appLoggerProxy);

        var workloadHandler = _workloadHandlerFactory.CreateInstance(workload);

        Assert.That(workloadHandler.PaymentService.GetType() == typeof(HbciBankingService));

        var from = DateTime.Now.AddDays(-35);
        var until = DateTime.Now;


        var result = workloadHandler.ArchiveAccountStatementsAsMt940(from, until);

        Assert.That(File.Exists(result));
    }

    //[Test]
    //public void TestGetBalanceWithFinApiNoAccount()
    //{


    //    // ReSharper disable once UseObjectOrCollectionInitializer
    //    var workload = new WorkloadTask
    //    {
    //       Iban = "DE37701696190120042374",
    //        //Iban = "DE23701696190000042374",
    //        Id = "9E397D34-F81F-46D0-83FC-160B3378804E",
    //        AccountStatementsLive = true,
    //        BankCode = "70169619",
    //        Password = "5lUtNwEAhpA=",
    //        Contact = "RL_57029",
    //        Provider = BankingServiceProvider.Hbci,

    //        AccountNr = "120042734",
    //        //AccountNr = "42734",
    //    };


    //    var service = TestHelper.GetFinApiService();

    //    _workloadHandlerFactory = new WorkloadHandlerFactory(service, _dataHandler, _options, true);


    //    var workloadHandler = _workloadHandlerFactory.GetWorkloadHandler(workload);

    //    Assert.That(workloadHandler.AccountInfoService.GetType()== typeof(HbciBankingService));
    //    Assert.That(workloadHandler.AccountInfoServiceDirect.GetType() == typeof(HbciBankingService));
    //    Assert.That(workloadHandler.PaymentService.GetType() == typeof(HbciBankingService));

    //    var result = workloadHandler.GetBalance();


    //    Assert.That(result > decimal.MinValue);

    //    Debug.Print($"Balance: {result}");

    //}


    //[Test]
    //public void TestGetBalanceWithFinApiAccount()
    //{



    //    // ReSharper disable once UseObjectOrCollectionInitializer
    //    var workload = new WorkloadTask
    //    {
    //        //Iban = "DE37701696190120042374",
    //        Iban = "DE23701696190000042374",
    //        Id = "9E397D34-F81F-46D0-83FC-160B3378804E",
    //        AccountStatementsLive = true,
    //        BankCode = "70169619",
    //        Password = "5lUtNwEAhpA=",
    //        Contact = "RL_57029",
    //        Provider = BankingServiceProvider.Hbci,

    //        //AccountNr = "120042734",
    //        AccountNr = "42374",
    //    };


    //    var service = TestHelper.GetFinApiService();

    //    var check = service.IsAccount(workload.Iban);

    //    Assert.That(check);

    //    _workloadHandlerFactory = new WorkloadHandlerFactory(service, _dataHandler, _options, true);


    //    var workloadHandler = _workloadHandlerFactory.GetWorkloadHandler(workload);

    //    Assert.That(workloadHandler.AccountInfoService.GetType() == typeof(FinApiService));
    //    Assert.That(workloadHandler.AccountInfoServiceDirect.GetType() == typeof(HbciBankingService));
    //    Assert.That(workloadHandler.PaymentService.GetType() == typeof(HbciBankingService));

    //    var result = workloadHandler.GetBalance();


    //    Assert.That(result > decimal.MinValue);

    //    Debug.Print($"Balance: {result}");

    //}

    private static void ShowMessage(string msg)
    {
        Console.WriteLine(msg);
    }
}