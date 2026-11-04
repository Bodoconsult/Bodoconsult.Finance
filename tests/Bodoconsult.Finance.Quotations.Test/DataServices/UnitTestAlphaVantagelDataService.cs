

//// ReSharper disable InconsistentNaming

//namespace Bodoconsult.Finance.Quotations.Test;

////[TestFixture]
////public class UnitTestAlphaVantageDataService
////{

////    private readonly IDataService _service = new AlphaVantageDataService();

////    private readonly DateTime _from = DateTime.Now.AddDays(-90);
////    private readonly DateTime _until = DateTime.Now.AddDays(-1);

////    public UnitTestAlphaVantageDataService()
////    {
////        _service.StatusChange += SetStatus;
////    }


////    [SetUp]
////    public void TestSetup()
////    {

////    }


////    [Test]
////    public void TestGetHistory_DiverseWp()
////    {

////        // Arrange


////        var wp = new SecuritiesItem { Wkn = "A0HG2Q", Isin = "DE000A0HG2Q2", Token = "ALPHA:MBH3.FRK" };
////        _service.SecuritiesItems.Add(wp);

////        wp = new SecuritiesItem { Wkn = "99999", Isin = "DE000A0J2078", Token = "ALPHA:IPRP.LON" };
////        _service.SecuritiesItems.Add(wp); ;

////        // Act
////        _service.GetHistory(_from, _until);

////        // Assert
////        Assert.IsTrue(_service.SecuritiesItems[0].History.Count>0);
////        Assert.IsTrue(_service.SecuritiesItems[1].History.Count>0);

////    }


////    [Test]
////    public void TestGetCurrentQuotation_DiverseWp()
////    {

////        // Arrange

////        var wp = new SecuritiesItem { Wkn = "A0HG2Q", Isin = "DE000A0HG2Q2", Token = "ALPHA:MBH3.FRK" };
////        _service.SecuritiesItems.Add(wp);

////        wp = new SecuritiesItem { Wkn = "99999", Isin = "DE000A0J2078", Token = "ALPHA:IPRP.LON" };
////        _service.SecuritiesItems.Add(wp); ;

////        // Act
////        _service.GetCurrentQuotation();

////        // Assert
////        Assert.IsTrue(_service.SecuritiesItems[0].Quotation<0.00001);
////        Assert.IsTrue(_service.SecuritiesItems[1].Quotation > 0);

////    }




////    static void SetStatus(string msg)
////    {

////        Debug.Print(msg);
////    }

////}