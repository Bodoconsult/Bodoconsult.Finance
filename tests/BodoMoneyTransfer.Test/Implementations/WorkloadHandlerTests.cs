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
using Bodoconsult.Database.SqlClient;
using Bodoconsult.Finance.Payment.Base.Helpers;
using BodoMoneyTransfer.Dal.App;

namespace BodoMoneyTransfer.Test.Implementations;

[SupportedOSPlatform("windows")]
[TestFixture]
internal class WorkloadHandlerTests
{
    private readonly WorkloadHandler _workloadHandler;
    private readonly Globals _appGlobals = Globals.Instance;

    public WorkloadHandlerTests()
    {
        var credentials = TestAppGlobals.Instance.TestCredentials;
        ArgumentNullException.ThrowIfNull(credentials);
        var contact = credentials.EbicsContacts[0];

        IPaymentService banking = new EbicsBankingService(credentials.ProductName, PaymentAppHelper.GetProductVersion());
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

        var db = new SqlClientConnManager(Globals.Instance.AppStartParameter.DefaultConnectionString);

        var dataHandler = new DataHandler(Globals.Instance, db);

        _workloadHandler = new WorkloadHandler(dataHandler, banking, workload, _appGlobals);
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