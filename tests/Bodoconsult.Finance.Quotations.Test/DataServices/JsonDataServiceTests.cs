// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bodoconsult.Finance.Quotations.DataServices;
using Bodoconsult.Finance.Quotations.Helpers;
using Bodoconsult.Finance.Quotations.Model;
using Bodoconsult.Finance.Quotations.Test.Helpers;
using NUnit.Framework;

namespace Bodoconsult.Finance.Quotations.Test.DataServices;

[TestFixture]
public class JsonDataServiceTests
{
    private readonly JsonDataService _service = new();

    public JsonDataServiceTests()
    {
        _service.StatusMessageDelegate = TestHelper.SetState;
        _service.JsonFile = Path.Combine(TestHelper.TestDataPath, @"quotation.json");
    }

    [SetUp]
    public void TestSetup()
    {

    }

    [Test]
    public void TestCreateSampleJson()
    {
        var data = new List<BasicQuotationItem>();

        var item = new BasicQuotationItem
        {
            Isin = "CH0013283368",
            Quotation = 6.97
        };

        data.Add(item);

        item = new BasicQuotationItem
        {
            Isin = "XS0410448459",
            Quotation = 25.12
        };

        data.Add(item);

        JsonHelper.SaveAsFile(_service.JsonFile, data);
    }


    [Test]
    public void TestGetCurrentQuotation()
    {
        var si = new List<SecuritiesItem>();

        _service.SecuritiesItems = si;

        _service.GetCurrentQuotation();

        Assert.IsTrue(si.Count > 0);
        Assert.IsTrue(si.Count == 2);
        Assert.IsTrue(si.Count(x => x.Quotation > 0) == 2);
    }
}