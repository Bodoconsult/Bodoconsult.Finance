// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.Finance.Payment.Base.Helpers;
using BodoMoneyTransfer.Dal.App;
using BodoMoneyTransfer.Dal.Helper;
using NUnit.Framework;

namespace BodoMoneyTransfer.Test;

[TestFixture]
internal class PaymentAppHelperTests
{
    //[SetUp]
    //public void Setup()
    //{
    //}

    [Test]
    public void GetProductVersion()
    {
        // Arrange

        // Act
        var version = PaymentAppHelper.GetProductVersion();

        // Assert
        Assert.That(version, Is.Not.Null);
        Debug.Print(version);
    }
   
}