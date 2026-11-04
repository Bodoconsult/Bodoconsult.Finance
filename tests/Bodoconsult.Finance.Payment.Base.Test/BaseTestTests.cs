// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System;
using System.IO;
using BaseTests;
using Bodoconsult.App.Helpers;
using Bodoconsult.Finance.Payment.Banking.Ebics;
using Bodoconsult.Finance.Payment.Banking.Hbci;
using NUnit.Framework;

namespace Bodoconsult.Finance.Payment.Base.Test;

[TestFixture]
public class BaseTestTests
{
    //[SetUp]
    //public void Setup()
    //{
    //}

    [Test]
    public void SerializeAsFile_SampleTestCredentials_Serialized()
    {
        // Arrange
        var filePath = Path.Combine(Path.GetTempPath(), "cred.json");

        var cred = new TestCredentials
        {
            AlphaVantageApiToken = Guid.NewGuid().ToString()
        };

        cred.HbciContacts.Add(new HbciBankingContact
        {
            Name = "testAccount",
            AccountNr = "12345",
            Bic= "GENODEF1ZOR",
            Iban = "DE23123456666776676778877666",

        });

        cred.EbicsContacts.Add(new EbicsBankingContact
        {
            Name = "testAccount",
            AccountNr = "12345",
            Bic = "GENODEF1ZOR",
            Iban = "DE23123456666776676778877666",

        });

        // Act
        SimpleJsonHelper.SerializeAsFile(filePath, cred);

        // Assert
        Assert.That(File.Exists(filePath));
    }
}