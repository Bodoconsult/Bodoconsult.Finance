// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.Finance.Quotations.DataServices;
using Bodoconsult.Finance.Quotations.Model;
using Bodoconsult.Finance.Quotations.Test.Helpers;
using NUnit.Framework;
using System.Diagnostics;

// ReSharper disable InconsistentNaming

namespace Bodoconsult.Finance.Quotations.Test.DataServices;

[TestFixture]
public class BaseDataServiceTests
{
    private readonly TestDataService _service = new();

    public BaseDataServiceTests()
    {
        _service.StatusMessageDelegate = TestHelper.SetState;
    }

    [SetUp]
    public void TestSetup()
    {

    }

    [Test]
    public void TestGetTicker_Success()
    {
        // Arrange
        var wp = new SecuritiesItem { Wkn = "A0HG2Q", Isin = "DE000A0HG2Q2" };
        _service.SecuritiesItems.Add(wp);

        // Act
        _service.GetToken();

        // Assert
        Assert.IsTrue(!string.IsNullOrEmpty( _service.SecuritiesItems[0].Token));
    }



    //[Test]
    //public void TestStartRequest()
    //{

    //    var wp = new SecuritiesItem { Wkn = "580265", Isin = "IE0005895655" };
    //    _service.Add(wp);

    //    //wp = new SecuritiesItem { Wkn = "982280", Isin = "CH0013283368" };
    //    //_service.Add(wp);


    //    wp = new SecuritiesItem { Wkn = "A0HG2Q", Isin = "DE000A0HG2Q2", Yahoo = "2465789.DU" };
    //    _service.Add(wp);

    //    //wp = new Bodoconsult.Wertpapierkurse.Wertpapier {Wkn = "A0J207", Isin = "DE000A0J2078", Yahoo = "2664350.DE"};
    //    //k.Add(wp);



    //    //wp = new Bodoconsult.Wertpapierkurse.Wertpapier {Wkn = "LYX0AB", Isin = "FR0010312124", Yahoo = "LYYC.DE"};
    //    //k.Add(wp);	

    //    _service.StartRequest();


    //    foreach (var x in _service.Securities)
    //    {
    //        var s = string.Format("ISIN: {0}: {1}", x.Isin, x.Quotation);

    //        SetStatus(s);

    //        Assert.IsTrue(x.Quotation > 0);
    //    }
    //}


    //[Test]
    //public void TestGetTickers()
    //{

    //    var wp = new SecuritiesItem { Wkn = "999999", Isin = "DE000A12UL56" };
    //    _service.Add(wp);

    //    //wp = new SecuritiesItem { Wkn = "982280", Isin = "CH0013283368" };
    //    //_service.Add(wp);


    //    var wp1 = new SecuritiesItem { Wkn = "A0HG2Q", Isin = "DE000A0HG2Q2" };
    //    _service.Add(wp1);

    //    //wp = new Bodoconsult.Wertpapierkurse.Wertpapier {Wkn = "A0J207", Isin = "DE000A0J2078", Yahoo = "2664350.DE"};
    //    //k.Add(wp);



    //    //wp = new Bodoconsult.Wertpapierkurse.Wertpapier {Wkn = "LYX0AB", Isin = "FR0010312124", Yahoo = "LYYC.DE"};
    //    //k.Add(wp);	

    //    //_service.GetTickers();


    //    //foreach (var x in _service.Securities)
    //    //{
    //    //    var s = string.Format("ISIN: {0}: {1}", x.Isin, x.Quotation);

    //    //    SetStatus(s);

    //    //    Assert.IsTrue(x.Quotation > 0);
    //    //}

    //    Assert.IsTrue(string.IsNullOrEmpty(wp.Yahoo));
    //    Assert.IsTrue(string.IsNullOrEmpty(wp1.Yahoo));
    //}


    //[Test]
    //public void TestStartRequest_Indices()
    //{

    //    var wp = new SecuritiesItem { Yahoo = "^GSPC" };
    //    _service.Add(wp);

    //    wp = new SecuritiesItem { Yahoo = "^GDAXI" };
    //    _service.Add(wp);

    //    wp = new SecuritiesItem { Yahoo = "^STOXX50E" };
    //    _service.Add(wp);

    //    wp = new SecuritiesItem { Yahoo = "^NDX" };
    //    _service.Add(wp);

    //    wp = new SecuritiesItem { Yahoo = "^GREXP" };
    //    _service.Add(wp);

    //    //wp = new SecuritiesItem { Yahoo = "^GDAXHI" };
    //    //_service.Add(wp);

    //    wp = new SecuritiesItem { Yahoo = "^FTSE" };
    //    _service.Add(wp);


    //    wp = new SecuritiesItem { Yahoo = "^SSMI" };
    //    _service.Add(wp);


    //    wp = new SecuritiesItem { Yahoo = "^HSI" };
    //    _service.Add(wp);


    //    wp = new SecuritiesItem { Yahoo = "^N225" };
    //    _service.Add(wp);










    //    //wp = new SecuritiesItem { Wkn = "982280", Isin = "CH0013283368" };
    //    //_service.Add(wp);


    //    //wp = new SecuritiesItem { Wkn = "A0HG2Q", Isin = "DE000A0HG2Q2", Yahoo = "2465789.DU" };
    //    //_service.Add(wp);

    //    //wp = new Bodoconsult.Wertpapierkurse.Wertpapier {Wkn = "A0J207", Isin = "DE000A0J2078", Yahoo = "2664350.DE"};
    //    //k.Add(wp);



    //    //wp = new Bodoconsult.Wertpapierkurse.Wertpapier {Wkn = "LYX0AB", Isin = "FR0010312124", Yahoo = "LYYC.DE"};
    //    //k.Add(wp);	

    //    _service.StartRequest();


    //    foreach (var x in _service.Securities)
    //    {
    //        var s = string.Format("ISIN: {0}: {1}", x.Isin, x.Quotation);

    //        SetStatus(s);

    //        Assert.IsTrue(x.Quotation > 0);
    //    }
    //}


    //[Test]
    //public void TestGetHistoricalData()
    //{
    //    var data = _service.GetHistoricalData("^GSPC", DateTime.Now.AddYears(-15), DateTime.Now);

    //    Assert.IsTrue(data.Count>0);
    //}


    //[Test]
    //public void TestGetHistoricalData_FromFile()
    //{
    //    var data = _service.GetHistoricalData("^GSPC", @"Q:\Rexp.csv");

    //    Assert.IsTrue(data.Count > 0);
    //}


    private static void SetStatus(string msg)
    {

        Debug.Print(msg);
    }

}