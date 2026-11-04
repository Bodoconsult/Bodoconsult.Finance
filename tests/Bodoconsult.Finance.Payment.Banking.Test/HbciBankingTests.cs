// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using BaseTests;
using BaseTests.Apps;
using Bodoconsult.App.Helpers;
using Bodoconsult.Finance.Payment.Banking.Hbci;
using Bodoconsult.Finance.Payment.Banking.Test.Helpers;
using Bodoconsult.Finance.Payment.Base.Models;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;

namespace Bodoconsult.Finance.Payment.Banking.Test;

[SupportedOSPlatform("windows")]
[TestFixture]
public class HbciBankingTests
{
    private HbciBankingService _banking;
    private readonly TestCredentials _credentials;
    private readonly string _path = Path.GetTempPath();

    public HbciBankingTests()
    {
        ArgumentNullException.ThrowIfNull(TestAppGlobals.Instance.TestCredentials);
        _credentials = TestAppGlobals.Instance.TestCredentials;
    }

    [SetUp]
    public void SetUp()
    {
        _banking = new HbciBankingService();
        _banking.ProductName = _credentials.ProductName;

        var bankingContact = _credentials.HbciContacts[0];

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
    public void TestGetBalance()
    {
        var erg = _banking.GetBalance();

        Debug.Print("Saldo: {0}", erg);

        Assert.That(erg > 0 || erg < 0);
    }

    [Test]
    public void GetContactBalances_ValidSetup_ReturnsListWithAccounts()
    {
        var erg = _banking.GetContactBalances();
        Assert.That(erg.Count, Is.Not.Null);

        foreach (var data in erg)
        {
            Debug.Print($"AccountName {data.AccountName} AccountNo {data.AccountNo} Saldo: {data.Amount} SaldoUngebucht: {data.AmountIncludingPendingTransactions}");
        }
    }

    //[Test]
    //public void TestTransfer()
    //{
    //    var order = new BankingOrder
    //    {
    //        PayerOrPayee = "Robert Leisner",
    //        Iban = "DE36673900000070376300",
    //        Bic = "GENODE61WTH",
    //        Description = "Umbuchung",
    //        Total = Convert.ToDecimal("29"),
    //        Currency = "EUR",
    //        Guid = Guid.NewGuid(),
    //        ExecOnSuccess = null,
    //        ExecOnFailure = null
    //    };

    //    var orders = new List<BankingOrder> { order };


    //    var erg = _banking.TransferVopOutOut(orders);

    //    Assert.That(erg.ErrorCode < 3000);


    //    //var erg = _banking.GetBalance();

    //    //Debug.Print("Saldo: {0}", erg);

    //    //Assert.That(erg > 0 || erg < 0);
    //}


    [Test]
    public void TestGetAccountStatement()
    {
        // Arrange
        var month = DateTime.Now.Month;
        var year = DateTime.Now.Year;

        //var month = 12;
        //var year = 2015;

        var startDate = new DateTime(year, month, 1).AddMonths(-1);
        var endDate = new DateTime(year, month, 1).AddMinutes(-1);

        //endDate = DateTime.Now;

        // Act         
        var erg = _banking.GetHistory(startDate, endDate);

        // Assert
        Assert.That(erg, Is.Not.Null);

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
        var target = $@"{_path}\mt940.sta";

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
        var target = $@"{_path}\mt940.sta";
        var pdf = $@"{_path}\mt940.pdf";

        FileSystemHelper.DeleteFile(target);
        FileSystemHelper.DeleteFile(pdf);

        // Act
        _banking.GetHistoryToMt940File(startDate, endDate, target, true);

        // Assert
        Assert.That(File.Exists(target));
        Assert.That(File.Exists(pdf));
    }



    [Test]
    public void TestLoadMt940FileAsCreateAccountStatement1()
    {
        // Arrange
        var target = $@"{_path}mt940.sta";

        var pdf = target.Replace(".sta", ".pdf");

        if (File.Exists(pdf)) File.Delete(pdf);

        var header = new AccountStatementHeader
        {
            AccountOwner = "Rothenburg_98922",
            AccountNr = "20098922",
            StartDate = new DateTime(2015, 8, 1),
            EndDate = new DateTime(2015, 8, 31),
            BankIdentifier = "DE4536626727277181"
        };

        // Act
        var erg = _banking.LoadMt940FileAsCreateAccountStatement(target, header);

        // Assert
        Assert.That(erg == false);
        Assert.That(File.Exists(pdf));
    }

    [Test]
    public void TestGetBankingContacts()
    {
        //Arrange

        // Act
        var erg = _banking.GetBankingContacts();

        //Assert
        Assert.That(erg, Is.Not.Null);
        Assert.That(erg.Count, Is.Not.Zero);
    }


    [Test]
    public void TestGetBankingAccounts()
    {
        //Arrange
        var contact = _credentials.HbciContacts[0];

        // Act
        var erg = _banking.GetBankingAccounts(contact.Name, contact.Passphrase);

        //Assert
        Assert.That(erg, Is.Not.Null);
        Assert.That(erg.Count, Is.Not.Zero);
    }

    [Test]
    public void TestGetOfficialAccountStatementFiles()
    {
        //Arrange

        // Act
        var erg = _banking.GetOfficialAccountStatementFiles();

        //Assert
        Assert.That(erg, Is.Not.Null);
        Assert.That(erg.Count, Is.Not.Zero);

        foreach (var af in erg)
        {
            Debug.Print("\r\n\r\n{0} {1} {2} \r\n{3} \r\n{4} \r\n{5}\r\n{6}", af.Year, af.Number, af.AccountStatementType, af.ReceiptHandling, af.RequestPossible, af.CreationType, af.FileName);
        }
    }


    [Test]
    public void TestGetOfficialAccountStatementFile()
    {
        //Arrange
        var erg = _banking.GetOfficialAccountStatementFiles();
        
        Assert.That(erg, Is.Not.Null);
        Assert.That(erg.Count, Is.Not.Zero);

        foreach (var af in erg.Where(x => x.RequestPossible == true).OrderBy(x => x.Number))
        {

            var fileName = Path.Combine(_path, af.FileName);

            FileSystemHelper.DeleteFile(fileName);

            Debug.Print("\r\n\r\n{0} {1} {2} \r\n{3} \r\n{4} \r\n{5}", af.Year, af.Number, af.AccountStatementType, af.ReceiptHandling, af.RequestPossible, af.CreationType);

            // Act
            var data = _banking.GetOfficialAccountStatementFile(af, false, fileName);

            //Assert
            Assert.That(data);
            Assert.That(File.Exists(fileName));
        }
    }
}