// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using BaseTests;
using BaseTests.Apps;
using Bodoconsult.Finance.Payment.Banking.Hbci;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using Bodoconsult.Finance.Payment.Banking.Test.Helpers;

// ReSharper disable InconsistentNaming

namespace Bodoconsult.Finance.Payment.Banking.Test;

[SupportedOSPlatform("windows")]
[TestFixture]
public class HbciBankingServiceDepotTests
{
    private HbciBankingService _banking;
    private readonly TestCredentials _credentials;

    public HbciBankingServiceDepotTests()
    {
        _credentials = TestAppGlobals.Instance.TestCredentials;
        ArgumentNullException.ThrowIfNull(_credentials);
	}

    [SetUp]
    public void SetUp()
    {
        _banking = new HbciBankingService();

        var bankingContact = _credentials.HbciContacts[2];

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
    public void TestGetDepot()
    {
        // Arrange

        // Act         
        var erg = _banking.GetDepot();

        // Assert
       Assert.That(erg, Is.Not.Null);
        Assert.That(erg.Count, Is.Not.Zero);

        foreach (var item in erg)
        {
            Debug.Print("{0}  {1}  {2} {3} {4} {5}", item.Wkn, item.Isin, item.Title, item.Quantity,
                item.PricePerUnitAmount, item.PricePerUnitCurrencyCode);
        }
    }
}