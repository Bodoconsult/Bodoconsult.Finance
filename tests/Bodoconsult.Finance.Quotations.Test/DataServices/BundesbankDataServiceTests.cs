// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.Finance.Quotations.DataServices;
using Bodoconsult.Finance.Quotations.Interfaces;
using Bodoconsult.Finance.Quotations.Model;
using Bodoconsult.Finance.Quotations.Test.Helpers;
using NUnit.Framework;
using System;

// ReSharper disable InconsistentNaming

namespace Bodoconsult.Finance.Quotations.Test.DataServices;

[TestFixture]
public class BundesbankDataServiceTests
{
    private readonly IDataService _service = new BundesbankDataService();
    private readonly DateTime From = DateTime.Now.AddDays(-90);
    private readonly DateTime To = DateTime.Now.AddDays(-1);

    public BundesbankDataServiceTests()
    {
        _service.StatusMessageDelegate = TestHelper.SetState;
    }

    [SetUp]
    public void TestSetup()
    {

    }

    [Test]
    public void TestGetHistory_Euribor()
    {
        // Arrange
        var wp = new SecuritiesItem { Wkn = "999999", Isin = "EURIBOR", Token = "BBK:EURIBOR" };
        _service.SecuritiesItems.Add(wp);

        // Act
        _service.GetHistory(From, To);

        // Assert
        Assert.IsTrue(wp.History.Count > 0);
    }
}