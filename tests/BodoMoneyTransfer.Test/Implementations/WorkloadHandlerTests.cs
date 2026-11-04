// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.Finance.Payment.Banking.Ebics;
using Bodoconsult.Finance.Payment.Base.Interfaces;
using BodoMoneyTransfer.Dal.Implementations;
using BodoMoneyTransfer.Dal.Model;
using BodoMoneyTransfer.Test.Helpers;
using NUnit.Framework;
using System.Diagnostics;
using System.Runtime.Versioning;
using BaseTests.Apps;

namespace BodoMoneyTransfer.Test.Implementations;

[SupportedOSPlatform("windows")]
[TestFixture]
internal class WorkloadHandlerTests
{
    private readonly WorkloadHandler _workloadHandler;
    private readonly IAppLoggerProxy _appLoggerProxy = TestHelper.GetFakeAppLoggerProxy();

    public WorkloadHandlerTests()
    {
        var credentials = TestAppGlobals.Instance.TestCredentials;
        ArgumentNullException.ThrowIfNull(credentials);
        var contact = credentials.EbicsContacts[0];

        IPaymentService banking = new EbicsBankingService();
        var bankingContact = new EbicsBankingContact
        {
            Name = contact.Name,
            Passphrase = contact.Passphrase,
            Iban = contact.Iban
        };

        banking.StatusMessageDelegate = _banking_StatusChange;
        banking.PaymentContact = bankingContact;
        banking.OpenContact();

        var workload = new WorkloadTask();

        var options = new Options
        {
            ArchivePath = @"d:\temp\BodoMoneyTransfer\Archive",
            OriginalArchivePath = @"d:\temp\BodoMoneyTransfer\Originals",
            QuotationFileName = @"d:\temp\BodoMoneyTransfer\QuotationFile.json",
            ErrorMailer = "Test@Test.de",
            ConnectionString = TestHelper.ConnectionString
        };

        var dataHandler = new DataHandler(options, _appLoggerProxy);

        _workloadHandler = new WorkloadHandler(dataHandler, banking, workload, options, _appLoggerProxy);
    }

    private void _banking_StatusChange(string msg)
    {
        Debug.Print(msg);
    }

    [Test]
    public void CheckVopMoneyTransfers_TransferWaiting_Success()
    {
        // Arrange 

        // Act  
        _workloadHandler.CheckVopMoneyTransfers();

        // Assert
    }
}