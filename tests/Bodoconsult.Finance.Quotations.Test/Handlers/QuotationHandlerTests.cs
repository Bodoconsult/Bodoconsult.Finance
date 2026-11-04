// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Bodoconsult.Finance.Quotations.Test.Handlers;

[TestFixture]
public class QuotationHandlerTests
{
    private readonly DateTime _from = DateTime.Now.AddDays(-90);
    private readonly DateTime _until = DateTime.Now.AddDays(-1);

    ///// <summary>
    ///// Constructor to initialize class
    ///// </summary>
    //public UnitTestQuotationHandler()
    //{

    //}


    ///// <summary>
    ///// Runs in front of each test method
    ///// </summary>
    //[SetUp]
    //public void Setup()
    //{


    //}

    ///// <summary>
    ///// Cleanup aft test methods
    ///// </summary>
    //[TestCleanup]
    //public void Cleanup()
    //{


    //}


    //[Test]
    //public void TestQuotationHandler_StartGetHistory()
    //{
    //    //Arrange
    //    var h = new QuotationHandler();
    //    h.StatusChange += SetStatus;
    //    h.ErrorHandler += ErrorMessage;

    //    var wp = new SecuritiesItem { Wkn = "A0HG2Q", Isin = "DE000A0HG2Q2", Token = "YAHOO:2465789.DU" };
    //    h.SecuritiesItems.Add(wp);

    //    wp = new SecuritiesItem { Wkn = "99999", Isin = "DE000A0J2078", Token = "YAHOO:^GSPC" };
    //    h.SecuritiesItems.Add(wp);

    //    wp = new SecuritiesItem { Wkn = "605283", Isin = "DE0006052830", Token = "YAHOO:MBH3.DE" };
    //    h.SecuritiesItems.Add(wp);

    //    // Act
    //    h.StartGetHistory(DataProviderType.Yahoo, _from, _until);



    //    //Assert
    //    Assert.IsTrue(h.SecuritiesItems[0].History == null);
    //    Assert.IsTrue(h.SecuritiesItems[1].History.Count > 0);
    //}


    //[Test]
    //public void TestQuotationHandler_StartGetCurrentQuotations()
    //{
    //    //Arrange
    //    var h = new QuotationHandler();
    //    h.StatusChange += SetStatus;
    //    h.ErrorHandler += ErrorMessage;

    //    var wp = new SecuritiesItem { Wkn = "A0HG2Q", Isin = "DE000A0HG2Q2", Token = "YAHOO:2465789.DU" };
    //    h.SecuritiesItems.Add(wp);

    //    wp = new SecuritiesItem { Wkn = "99999", Isin = "DE000A0J2078", Token = "YAHOO:^GSPC" };
    //    h.SecuritiesItems.Add(wp);

    //    wp = new SecuritiesItem { Wkn = "605283", Isin = "DE0006052830", Token = "YAHOO:MBH3.DE" };
    //    h.SecuritiesItems.Add(wp);

    //    // Act
    //    h.StartGetHistory(DataProviderType.Yahoo, _from, _until);




    //    //Assert
    //    Assert.IsTrue(h.SecuritiesItems[0].History == null);
    //    Assert.IsTrue(h.SecuritiesItems[1].History.Count > 0);
    //}

    private void ErrorMessage(Exception ex)
    {
        Debug.Print(ex.Message);
    }

    private void SetStatus(string msg)
    {
        Debug.Print(msg);
    }
}