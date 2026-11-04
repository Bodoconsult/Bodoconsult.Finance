// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.IO;
using Bodoconsult.Finance.Quotations.DataServices;
using Bodoconsult.Finance.Quotations.Model;
using Bodoconsult.Finance.Quotations.Test.Helpers;
using NUnit.Framework;

namespace Bodoconsult.Finance.Quotations.Test.DataServices;

[TestFixture]
public class DepotDataServiceTests
{
	private readonly SafekeepingDataService _service = new();
	private readonly string _fileName = Path.Combine(TestHelper.TestDataPath, @"depot.json");

	public DepotDataServiceTests()
	{
		_service.StatusMessageDelegate = TestHelper.SetState;
	}

	[SetUp]
	public void TestSetup()
	{

	}

	[Test]
	public void TestGetCurrentQuotation_DiverseWp()
	{

		_service.JsonFile = _fileName;

		// Arrange
		var wp = new SecuritiesItem { Wkn = "263530", Isin = string.Empty, Token = string.Empty };
		_service.SecuritiesItems.Add(wp);

		wp = new SecuritiesItem { Wkn = "710000", Isin = string.Empty, Token = string.Empty };
		_service.SecuritiesItems.Add(wp);

		wp = new SecuritiesItem { Wkn = "605283", Isin = string.Empty, Token = string.Empty };
		_service.SecuritiesItems.Add(wp);

		// Act
		_service.GetCurrentQuotation();

		// Assert
		Assert.IsTrue(_service.SecuritiesItems[0].Quotation > 0);
		Assert.IsTrue(_service.SecuritiesItems[1].Quotation < 0.00001);
		Assert.IsTrue(_service.SecuritiesItems[2].Quotation > 0);

	}
}