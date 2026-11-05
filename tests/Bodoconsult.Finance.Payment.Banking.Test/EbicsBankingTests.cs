// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using BaseTests;
using BaseTests.Apps;
using Bodoconsult.App.Helpers;
using Bodoconsult.Finance.Payment.Banking.Ebics;
using Bodoconsult.Finance.Payment.Banking.Hbci;
using Bodoconsult.Finance.Payment.Banking.Models;
using Bodoconsult.Finance.Payment.Banking.Test.Helpers;
using Bodoconsult.Finance.Payment.Base.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;

namespace Bodoconsult.Finance.Payment.Banking.Test;

[SupportedOSPlatform("windows")]
[TestFixture]
public class EbicsBankingTests
{
    private EbicsBankingService _banking;
    private readonly TestCredentials _credentials;
    private readonly HbciBankingContact _hbciContact;
    private readonly string _path = Path.GetTempPath();

    public EbicsBankingTests()
    {
        _credentials = TestAppGlobals.Instance.TestCredentials;
        ArgumentNullException.ThrowIfNull(_credentials);

        _hbciContact = _credentials.HbciContacts[0];
    }

    [SetUp]
    public void SetUp()
    {
        var service =  new EbicsBankingService(_credentials.ProductName, PaymentAppHelper.GetProductVersion());
        _banking = service;

        var bankingContact = _credentials.EbicsContacts[0];

        _banking.StatusMessageDelegate = TestHelper.SetState;
        _banking.PaymentContact = bankingContact;
        _banking.OpenContact();
    }

    [TearDown]
    public void TearDown()
    {
        _banking.CloseContact();
        _banking = null;
    }

    [Test]
    public void TestRunTransfer()
    {
        var orders = new List<BankingOrder>();

        // Arrange
        var order = new BankingOrder
        {
            Iban = _hbciContact.Iban,
            Bic = _hbciContact.Bic,
            Currency = "EUR",
            Description = "Testumbuchung",
            Total = DateTime.Now.Hour*10+DateTime.Now.Minute,
            PayerOrPayee = _hbciContact.AccountName,
            Guid = Guid.NewGuid(),
        };

        orders.Add(order);

        // Act         
        var erg = _banking.TransferVopOutOut(orders);

        // Assert
        Assert.That(erg.ErrorCode == 0);
    }

    [Test]
    public void TestGetBalance()
    {
        var erg = _banking.GetBalance();

        Debug.Print("Saldo: {0}", erg);

        Assert.That(erg>0 || erg<0);
    }

    [Test]
    public void TestGetAccountStatement()
    {
        var erg = _banking.GetHistory(DateTime.Now.AddMonths(-1), DateTime.Now.AddDays(-1));

        Assert.That(erg, Is.Not.Null);
        Assert.That(erg.Items.Any());

        foreach (var r in erg.Items)
        {
            Debug.Print("{0} {1} {2}", r.EntryDate, r.Name, r.Amount);
        }
    }

    [Test]
    public void TestGetAccountStatementToMt940File()
    {
        // Arrange
        var month = DateTime.Now.Month;
        var year = DateTime.Now.Year;

        //var month = 12;
        //var year = 2015;

        var startDate = new DateTime(year, month, 1).AddMonths(-1);
        var endDate = new DateTime(year, month, 1).AddMinutes(-1);
        var target = $@"{_path}\mt940ebics.sta";

        FileSystemHelper.DeleteFile(target);

        // Act         
        _banking.GetHistoryToMt940File(startDate, endDate, target, false);

        // Assert
        Assert.That(File.Exists(target));
    }

    [Test]
    public void TestGetAccountStatementToMt940FileAndPdf()
    {
        // Arrange

        var month = DateTime.Now.Month;
        var year = DateTime.Now.Year;

        //var month = 12;
        //var year = 2015;

        var startDate = new DateTime(year, month, 1).AddMonths(-1);
        var endDate = new DateTime(year, month, 1).AddMinutes(-1);
        var target = $@"{_path}\mt940ebics.sta";
        var pdf = $@"{_path}\mt940ebics.pdf";

        FileSystemHelper.DeleteFile(target);
        FileSystemHelper.DeleteFile(pdf);

        // Act
        _banking.GetHistoryToMt940File(startDate, endDate, target, true);

        // Assert
        Assert.That(File.Exists(target));
        Assert.That(File.Exists(pdf));
    }

    //[Test]
    //public void TestGetAccountStatement()
    //{
    //    var erg = _banking.GetHistory(DateTime.Now.AddMonths(-3), DateTime.Now);

    //   Assert.That(erg, Is.Not.Null);
    //    Assert.That(erg.Any());

    //}

    [Test]
    public void DownloadPtk_ValidSetup_ReturnsData()
    {
        // Arrange 

        // Act  
        var result = _banking.DownloadPtk();

        // Assert
    }

    [Test]
    public void DownloadVpz_ValidAccount_FillsDataInList()
    {
        // Arrange 
        List<VopProcessingData> data = [];

        // Act  
        var result = _banking.DownloadVpz(data);

        // Assert
        Assert.That(result);
    }


    //[Test]
    //public void DownloadPtk_ValidSetup_ReturnsData()
    //{
    //    // Arrange 
    //    // Act  
    //    var result = _banking.G();
    //    // Assert
    //}
}