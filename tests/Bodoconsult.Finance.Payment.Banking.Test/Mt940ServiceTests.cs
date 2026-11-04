// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;
using Bodoconsult.Finance.Payment.Banking.Services;
using Bodoconsult.Finance.Payment.Base.Models;
using NUnit.Framework;

namespace Bodoconsult.Finance.Payment.Banking.Test;

[SupportedOSPlatform("windows")]
[TestFixture]
public class Mt940ServiceTests
{
    private const string TargetFileName = @"C:\test\mt949_paypal.sta";
        
    private Mt940Service _service;


    /// <summary>
    /// Constructor to initialize class
    /// </summary>
    public Mt940ServiceTests()
    {

    }


    /// <summary>
    /// Runs in front of each test method
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _service = new Mt940Service {TargetFileName = TargetFileName};

    }

    /// <summary>
    /// Cleanup aft test methods
    /// </summary>
    [TearDown]
    public void Cleanup()
    {


    }


    [Test]
    public void TestSaveMt940File()
    {
        //Arrange
        var file = _service.DataFile;
        file.AccountNumber = "70169619/96636";
        file.OpeningBalanceAmount = (decimal) 1000.01;
        file.OpeningBalanceDate = new DateTime(2015,1,1);
        file.ClosingBalanceAmount = 1250;
        file.ClosingBalanceDate = new DateTime(2015, 1, 31);
        file.AvailableBalanceAmount = (decimal) 1250.01;
        file.AvailableBalanceDate = new DateTime(2015, 1, 31);


        var data = new List<AccountStatementLine>();

        var ka = new AccountStatementLine
        {
            EntryDate = new DateTime(2015, 1, 2),
            ValidityDate = new DateTime(2015, 1, 2),
            Amount = (decimal) -100.15,
            BusinessTransactionCode = "105",
            BusinessTransactionText = "Überweisung",
            Purpose = "Test",
            Name = "Testabsender"
        };

        data.Add(ka);


        ka = new AccountStatementLine
        {
            EntryDate = new DateTime(2015, 1, 3),
            ValidityDate = new DateTime(2015, 1, 3),
            Amount = (decimal) 200.29,
            BusinessTransactionCode = "166",
            BusinessTransactionText = "Gutschrift",
            Purpose = "Testüberweisung von einem Paypal-Konto nach Hamburg und zurück über Berlin, damit alles wichtiger ist",
            Name = "Testempfaenger"
        };

        data.Add(ka);


        file.Items = data;



        // Act
        var erg = _service.SaveMt940File();


        //Assert
        Assert.That(erg==false);
        Assert.That(File.Exists(TargetFileName));
    }

    [Test]
    public void TestSaveMt940FileAndPdf()
    {
        //Arrange
        var file = _service.DataFile;
        file.AccountNumber = "70169619/96636";
        file.OpeningBalanceAmount = (decimal)1000.01;
        file.OpeningBalanceDate = new DateTime(2015, 1, 1);
        file.ClosingBalanceAmount = 1250;
        file.ClosingBalanceDate = new DateTime(2015, 1, 31);
        file.AvailableBalanceAmount = (decimal)1250.01;
        file.AvailableBalanceDate = new DateTime(2015, 1, 31);
        file.AccountOwner = "Hallo";

        var data = new List<AccountStatementLine>();

        var ka = new AccountStatementLine
        {
            EntryDate = new DateTime(2015, 1, 2),
            ValidityDate = new DateTime(2015, 1, 2),
            Amount = (decimal)-100.15,
            BusinessTransactionCode = "105",
            BusinessTransactionText = "Überweisung",
            Purpose = "Test",
            Name = "Testabsender"
        };

        data.Add(ka);


        ka = new AccountStatementLine
        {
            EntryDate = new DateTime(2015, 1, 3),
            ValidityDate = new DateTime(2015, 1, 3),
            Amount = (decimal)200.29,
            BusinessTransactionCode = "166",
            BusinessTransactionText = "Gutschrift",
            Purpose = "Testüberweisung von einem Paypal-Konto nach Hamburg und zurück über Berlin, damit alles wichtiger ist",
            Name = "Testempfaenger"
        };

        data.Add(ka);


        file.Items = data;



        // Act
        var erg = _service.SaveMt940FileAndPdf();


        //Assert
        Assert.That(erg == false);
        Assert.That(File.Exists(TargetFileName));
    }

}