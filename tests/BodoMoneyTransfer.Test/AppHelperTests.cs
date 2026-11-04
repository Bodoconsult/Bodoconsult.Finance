// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using BodoMoneyTransfer.Dal.App;
using NUnit.Framework;

namespace BodoMoneyTransfer.Test;

[TestFixture]
internal class AppHelperTests
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
        var version = AppHelper.GetProductVersion();

        // Assert
        Assert.That(version, Is.Not.Null);
        Debug.Print(version);
    }
}