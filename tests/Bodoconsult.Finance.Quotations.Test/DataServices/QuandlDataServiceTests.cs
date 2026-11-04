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
public class QuandlDataServiceTests
{
    private readonly IDataService _service = new QuandlDataService();
    private readonly DateTime _from = DateTime.Now.AddDays(-90);
    private readonly DateTime _until = DateTime.Now.AddDays(-1);

    public QuandlDataServiceTests()
    {
        _service.StatusMessageDelegate = TestHelper.SetState;
    }

    [SetUp]
    public void TestSetup()
    {

    }

    [Test]
    public void TestGetHistory_DiverseWp()
    {
        // Arrange
        var wp = new SecuritiesItem { Wkn = string.Empty, Isin = string.Empty, Token = "QUANDL:BUNDESBANK/BBK01_WT5511" };
        _service.SecuritiesItems.Add(wp);

        wp = new SecuritiesItem { Wkn = string.Empty, Isin = string.Empty, Token = "QUANDL:NASDAQOMX/NQEURO8600" };
        _service.SecuritiesItems.Add(wp); ;

        // Act
        _service.GetHistory(_from, _until);

        // Assert
        Assert.IsTrue(_service.SecuritiesItems[0].History.Count>0);
        Assert.IsTrue(_service.SecuritiesItems[1].History.Count>0);
    }

    [Test]
    public void TestGetCurrentQuotation_DiverseWp()
    {
        // Arrange
        var wp = new SecuritiesItem { Wkn = "A0HG2Q", Isin = "DE000A0HG2Q2", Token = "QUANDL:BUNDESBANK/BBK01_WT5511" };
        _service.SecuritiesItems.Add(wp);

        wp = new SecuritiesItem { Wkn = "99999", Isin = "DE000A0J2078", Token = "QUANDL:NASDAQOMX/NQEURO8600" };
        _service.SecuritiesItems.Add(wp); ;

        // Act
        _service.GetCurrentQuotation();

        // Assert
        Assert.IsTrue(_service.SecuritiesItems[0].Quotation<0.00001);
        Assert.IsTrue(_service.SecuritiesItems[1].Quotation > 0);
    }
}