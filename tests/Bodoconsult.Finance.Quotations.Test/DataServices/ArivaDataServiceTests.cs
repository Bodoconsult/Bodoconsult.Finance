// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using Bodoconsult.Finance.Quotations.DataServices;
using Bodoconsult.Finance.Quotations.Interfaces;
using Bodoconsult.Finance.Quotations.Model;
using Bodoconsult.Finance.Quotations.Test.Helpers;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace Bodoconsult.Finance.Quotations.Test.DataServices;

[TestFixture]
public class ArivaDataServiceTests
{
    private readonly IDataService _service = new ArivaDataService();
    private readonly DateTime _from = DateTime.Now.AddDays(-90);
    private readonly DateTime _until = DateTime.Now.AddDays(-1);

    public ArivaDataServiceTests()
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
        var wp = new SecuritiesItem { Wkn = "A0HG2Q", Isin = "DE000A0HG2Q2", Token = "2465789.DU" };
        _service.SecuritiesItems.Add(wp);

        wp = new SecuritiesItem { Wkn = "99999", Isin = "DE000A0J2078", Token = "^GSPC" };
        _service.SecuritiesItems.Add(wp);

        // Act
        Assert.That(() =>
        {
            _service.GetHistory(_from, _until);
        }, Throws.TypeOf<NotImplementedException>());
    }


    [Test]
    public void TestGetCurrentQuotation_DiverseWp()
    {
        // Arrange
        var wp = new SecuritiesItem { Wkn = "710000", Isin = "DE0007100000", Token = "DAI" };
        _service.SecuritiesItems.Add(wp);

        //wp = new SecuritiesItem { Wkn = "99999", Isin = "DE000A0J2078", Token = "^GSPC" };
        //_service.SecuritiesItems.Add(wp);

        //var wp = new SecuritiesItem { Wkn = "911950", Isin = "IE0032895942", Token = "911950" };
        //_service.SecuritiesItems.Add(wp);


        //: 

        // Act
        _service.GetCurrentQuotation();

        // Assert
        Assert.IsTrue(_service.SecuritiesItems[0].Quotation > 0);
        //Assert.IsTrue(_service.SecuritiesItems[1].Quotation > 0);
    }
}