// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using BaseTests.Apps;
using Bodoconsult.Finance.Quotations.DataServices;
using Bodoconsult.Finance.Quotations.Interfaces;
using Bodoconsult.Finance.Quotations.Model;
using Bodoconsult.Finance.Quotations.Test.Helpers;
using NUnit.Framework;
using System;

// ReSharper disable InconsistentNaming

namespace Bodoconsult.Finance.Quotations.Test.DataServices;

[TestFixture]
public class AlphaVantageDataServiceTests
{
    private readonly IDataService _service = new AlphaVantageDataService();
    private readonly DateTime _from = DateTime.Now.AddDays(-90);
    private readonly DateTime _until = DateTime.Now.AddDays(-1);

    public AlphaVantageDataServiceTests()
    {   
        var cred = TestAppGlobals.Instance.TestCredentials;
        ArgumentNullException.ThrowIfNull(cred);

        var apiToken = cred.AlphaVantageApiToken;
        ArgumentNullException.ThrowIfNull(apiToken);

        _service.StatusMessageDelegate = TestHelper.SetState;
        ((AlphaVantageDataService) _service).ApiToken = apiToken;
    }

    //[SetUp]
    //public void TestSetup()
    //{ }

    [Test]
    public void TestGetHistory_DiverseWp()
    {
        // Arrange
        var wp = new SecuritiesItem { Wkn = string.Empty, Isin = string.Empty, Token = "ALPHA:MBH3.FRK" };
        _service.SecuritiesItems.Add(wp);

        wp = new SecuritiesItem { Wkn = string.Empty, Isin = string.Empty, Token = "ALPHA:IPRP.LON" };
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
        var wp = new SecuritiesItem { Wkn = string.Empty, Isin = string.Empty, Token = "ALPHA:MBH3.FRK" };
        _service.SecuritiesItems.Add(wp);

        wp = new SecuritiesItem { Wkn = string.Empty, Isin = string.Empty, Token = "ALPHA:IPRP.LON" };
        _service.SecuritiesItems.Add(wp); ;

        // Act
        _service.GetCurrentQuotation();

        // Assert
        Assert.IsTrue(_service.SecuritiesItems[0].Quotation>0);
        Assert.IsTrue(_service.SecuritiesItems[1].Quotation > 0);
    }
}