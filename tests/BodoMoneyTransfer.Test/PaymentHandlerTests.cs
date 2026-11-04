// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using BodoMoneyTransfer.Dal.Implementations;
using BodoMoneyTransfer.Dal.Interfaces;
using NUnit.Framework;
using System;
using System.Runtime.Versioning;
using Bodoconsult.App.Abstractions.Interfaces;
using BodoMoneyTransfer.Dal.App;
using BodoMoneyTransfer.Test.Helpers;

namespace BodoMoneyTransfer.Test;

[TestFixture]
[SupportedOSPlatform("windows10.0.17763")]
public class PaymentHandlerTests
{
    private PaymentHandler _handler;
    private readonly IAppLoggerProxy _logger = TestHelper.GetFakeAppLoggerProxy();
    private IDataHandler _dataHandler;
    private IWorkloadProcessorFactory _workloadProcessorFactory;

    [SetUp]
    public void SetUp()
    {

        // Options
        var options = Globals.Instance.Options;

        _dataHandler = new DataHandler(options, _logger);
        IWorkloadHandlerFactory workloadProcessorFactory = new WorkloadHandlerFactory(_dataHandler, options, _logger);
        _workloadProcessorFactory = new WorkloadProcessorFactory(_dataHandler, options, _logger, workloadProcessorFactory);
        _handler = new PaymentHandler(_dataHandler, options, _logger, _workloadProcessorFactory);
        options.StatusMessageDelegate = ShowMessage;
    }

    //[Test]
    //public void TestArchiveAccountStatements()
    //{
    //    //try
    //    //{
    //        var erg = _handler.ArchiveAccountStatements();
    //        Assert.IsFalse(erg);
    //    //}
    //    //catch (Exception ex)
    //    //{
    //    //    Assert.Fail("Expected no exception, but got: " + ex.Message);
    //    //}
    //}

    //[Test]
    //public void TestArchiveOriginalAccountStatements()
    //{
    //    //try
    //    //{
    //    var erg = _handler.ArchiveOriginalAccountStatements();
    //    Assert.IsFalse(erg);
    //    //}
    //    //catch (Exception ex)
    //    //{
    //    //    Assert.Fail("Expected no exception, but got: " + ex.Message);
    //    //}
    //}


    //[Test]
    //public void TestAccountStatements()
    //{
    //    //try
    //    //{
    //    _handler.GetAccountStatements();
    //    Assert.IsFalse(false);
    //    //}
    //    //catch (Exception ex)
    //    //{
    //    //    Assert.Fail("Expected no exception, but got: " + ex.Message);
    //    //}
    //}


    //[Test]
    //public void TestGetSafekeepingItems()
    //{
    //    //try
    //    //{
    //        const string fileName = @"C:\tmp\wptest.json";

    //         _handler.GetSafekeepingItems(fileName);
    //        Assert.That(File.Exists(fileName));
    //    //}
    //    //catch (Exception ex)
    //    //{
    //    //    Assert.Fail("Expected no exception, but got: " + ex.Message);
    //    //}
    //}



    //[Test]
    //public void TestAppendTransactionsWrongIban()
    //{

    //    var transaction = new List<BankingOrder>();

    //    var bo = new BankingOrder
    //    {
    //        Iban = "DE4711",
    //        Bic = "GENODEF1ZOR",
    //        Guid = Guid.NewGuid().ToString(),
    //        PayerOrPayee = "Test",
    //        Description = "Testüberweisung",
    //        Total = (decimal)109.11,
    //        Currency = "EUR"
    //    };

    //    transaction.Add(bo);

    //    var result = new List<BankingOrder>();

    //    _handler.AppendTransactions(transaction, result, "DE37701696190000096636", 1000);

    //    Assert.That(result.Count == 0);
    //}


    //[Test]
    //public void TestAppendTransactionsIbansEqual()
    //{

    //    var transaction = new List<BankingOrder>();

    //    var bo = new BankingOrder
    //    {
    //        Iban = "DE37701696190000096636",
    //        Bic = "GENODEF1ZOR",
    //        Guid = Guid.NewGuid().ToString(),
    //        PayerOrPayee = "Test",
    //        Description = "Testüberweisung",
    //        Total = (decimal)109.11,
    //        Currency = "EUR"
    //    };

    //    transaction.Add(bo);

    //    var result = new List<BankingOrder>();

    //    _handler.AppendTransactions(transaction, result, "DE37701696190000096636", 1000);

    //    Assert.That(result.Count == 0);
    //}


    //[Test]
    //public void TestAppendTransactionsBalanceToLow()
    //{

    //    var transaction = new List<BankingOrder>();

    //    var bo = new BankingOrder
    //    {
    //        Iban = "DE37701696190000717436",
    //        Bic = "GENODEF1ZOR",
    //        Guid = Guid.NewGuid().ToString(),
    //        PayerOrPayee = "Test",
    //        Description = "Testüberweisung1",
    //        Total = (decimal)109.11,
    //        Currency = "EUR"
    //    };

    //    transaction.Add(bo);


    //    bo = new BankingOrder
    //    {
    //        Iban = "DE37701696190000717436",
    //        Bic = "GENODEF1ZOR",
    //        Guid = Guid.NewGuid().ToString(),
    //        PayerOrPayee = "Test",
    //        Description = "Testüberweisung2",
    //        Total = (decimal)80.11,
    //        Currency = "EUR"
    //    };

    //    transaction.Add(bo);

    //    var result = new List<BankingOrder>();

    //    _handler.AppendTransactions(transaction, result, "DE37701696190000096636", 100);

    //    Assert.That(result.Count == 1);
    //}


    private static void ShowMessage(string msg)
    {
        Console.WriteLine(msg);
    }
}