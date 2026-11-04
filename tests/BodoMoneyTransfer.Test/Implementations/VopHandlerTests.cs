// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.Finance.Payment.Banking.Dummy;
using Bodoconsult.Finance.Payment.Banking.Ebics;
using BodoMoneyTransfer.Dal.Implementations;
using BodoMoneyTransfer.Dal.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Bodoconsult.Finance.Payment.Banking.Enums;

namespace BodoMoneyTransfer.Test.Implementations;

[TestFixture]
internal class VopHandlerTests
{
    private List<Payment> GetPayments()
    {
        var data = new List<Payment>
        {
            new()
            {
                PayerOrPayee = "Payee",
                Iban = "IBAN",
                Bic = "GENODEF1ASG",
                Description = null,
                Total = 999,
                Currency = "EUR",
                Guid = Guid.NewGuid(),
                ExecOnSuccess = null,
                ExecOnFailure = null,
                ExecOnVpoCloseMatch = "SQL"
            },
            new()
            {
                PayerOrPayee = "Payee2",
                Iban = "IBAN2",
                Bic = "GENODEF1ASG",
                Description = null,
                Total = 888,
                Currency = "EUR",
                Guid = Guid.NewGuid(),
                ExecOnSuccess = null,
                ExecOnFailure = null,
                ExecOnVpoCloseMatch = "SQL"
            }
        };

        return data;
    }

    private List<VopProcessingData> GetVopProcessingDatas(List<Payment> payments, string state)
    {
        var result = new List<VopProcessingData>();

        var i = 0;

        foreach (var dataItem in payments)
        {
            i++;

            var vopItem = VopHandler.MapPaymentToVopProcessingData(dataItem);
            vopItem.CreditorName = $"Blubb{i}";
            vopItem.DataDigest = $"BlubbHash{i}";
            vopItem.Message = $"Hallo{i}";
            vopItem.TransactionState = TransactionState.DeliveredToBank;
            vopItem.OrderId = $"OrderId{i}";
            vopItem.OrderType = "SCT";
            vopItem.State = state;
            result.Add(vopItem);
        }

        return result;
    }

    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var payments = GetPayments();

        var bankingService = new DummyBankingService();

        var workload = new List<WorkloadTask>();

        var options = new Options
        {
            ArchivePath = @"d:\temp\BodoMoneyTransfer\Archive",
            OriginalArchivePath = @"d:\temp\BodoMoneyTransfer\Originals",
            QuotationFileName = @"d:\temp\BodoMoneyTransfer\QuotationFile.json",
            ErrorMailer = "Test@Test.de"
        };

        var dataHandler = new DummyDataHandler(options, workload, payments);

        // Act  
        var voph = new VopHandler(payments, bankingService, dataHandler);

        // Assert
        Assert.That(voph.BankingService, Is.EqualTo(bankingService));
        Assert.That(voph.Payments, Is.EqualTo(payments));
        Assert.That(voph.VopPayments.Count, Is.EqualTo(payments.Count));
    }


    [Test]
    public void MapPaymentToVopProcessingData_ValidPayment_ReturnsData()
    {
        // Arrange 
        var payments = GetPayments();

        var payment1 = payments[0];

        // Act  
        var result = VopHandler.MapPaymentToVopProcessingData(payment1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.EndToEndId, Is.Not.Null);
        Assert.That(result.Iban, Is.EqualTo(payment1.Iban));
        Assert.That(result.TransactionId, Is.EqualTo(payment1.Guid));
        Assert.That(result.ExecOnVpoCloseMatch, Is.EqualTo(payment1.ExecOnVpoCloseMatch));
    }

    [Test]
    public void DownloadVpz_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var payments = GetPayments();
        var vop = GetVopProcessingDatas(payments, VopHandler.EbicsStateMatch);

        var bankingService = new DummyBankingService
        {
            VopData = vop
        };

        var workload = new List<WorkloadTask>();

        var options = new Options
        {
            ArchivePath = @"d:\temp\BodoMoneyTransfer\Archive",
            OriginalArchivePath = @"d:\temp\BodoMoneyTransfer\Originals",
            QuotationFileName = @"d:\temp\BodoMoneyTransfer\QuotationFile.json",
            ErrorMailer = "Test@Test.de"
        };

        var dataHandler = new DummyDataHandler(options, workload, payments);

        var voph = new VopHandler(payments, bankingService, dataHandler);

        // Act  
        voph.DownloadVpz();

        // Assert
        var vopData1 = voph.VopPayments[0];

        Assert.That(vopData1.OrderId, Is.Not.Null);
        Assert.That(vopData1.State, Is.Not.Null);
        Assert.That(vopData1.CreditorName, Is.Not.Null);
    }

    [Test]
    public void CheckStateForPayments_ValidSetupMatch_PropsSetCorrectly()
    {
        // Arrange 
        var payments = GetPayments();
        var vop = GetVopProcessingDatas(payments, VopHandler.EbicsStateMatch);

        var bankingService = new DummyBankingService
        {
            VopData = vop
        };

        var workload = new List<WorkloadTask>();

        var options = new Options
        {
            ArchivePath = @"d:\temp\BodoMoneyTransfer\Archive",
            OriginalArchivePath = @"d:\temp\BodoMoneyTransfer\Originals",
            QuotationFileName = @"d:\temp\BodoMoneyTransfer\QuotationFile.json",
            ErrorMailer = "Test@Test.de"
        };

        var dataHandler = new DummyDataHandler(options, workload, payments);

        var voph = new VopHandler(payments, bankingService, dataHandler);
        voph.DownloadVpz();


        // Act  
        voph.CheckStateForPayments();

        // Assert
        var vopData1 = voph.VopPayments[0];

        Assert.That(vopData1.TransactionState, Is.EqualTo(TransactionState.Successful));
    }

    [Test]
    public void CheckStateForPayments_ValidSetupCloseMatch_PropsSetCorrectly()
    {
        // Arrange 
        var payments = GetPayments();
        var vop = GetVopProcessingDatas(payments, VopHandler.EbicsStateCloseMatch);

        var bankingService = new DummyBankingService
        {
            VopData = vop
        };

        var workload = new List<WorkloadTask>();

        var options = new Options
        {
            ArchivePath = @"d:\temp\BodoMoneyTransfer\Archive",
            OriginalArchivePath = @"d:\temp\BodoMoneyTransfer\Originals",
            QuotationFileName = @"d:\temp\BodoMoneyTransfer\QuotationFile.json",
            ErrorMailer = "Test@Test.de"
        };

        var dataHandler = new DummyDataHandler(options, workload, payments);

        var voph = new VopHandler(payments, bankingService, dataHandler);
        voph.DownloadVpz();


        // Act  
        voph.CheckStateForPayments();

        // Assert
        var vopData1 = voph.VopPayments[0];

        Assert.That(vopData1.TransactionState, Is.EqualTo(TransactionState.Storno));
    }

    [Test]
    public void CheckStateForPayments_ValidSetupNoMatch_PropsSetCorrectly()
    {
        // Arrange 
        var payments = GetPayments();
        var vop = GetVopProcessingDatas(payments, VopHandler.EbicsStateNoMatch);

        var bankingService = new DummyBankingService
        {
            VopData = vop
        };

        var workload = new List<WorkloadTask>();

        var options = new Options
        {
            ArchivePath = @"d:\temp\BodoMoneyTransfer\Archive",
            OriginalArchivePath = @"d:\temp\BodoMoneyTransfer\Originals",
            QuotationFileName = @"d:\temp\BodoMoneyTransfer\QuotationFile.json",
            ErrorMailer = "Test@Test.de"
        };

        var dataHandler = new DummyDataHandler(options, workload, payments);

        var voph = new VopHandler(payments, bankingService, dataHandler);
        voph.DownloadVpz();


        // Act  
        voph.CheckStateForPayments();

        // Assert
        var vopData1 = voph.VopPayments[0];

        Assert.That(vopData1.TransactionState, Is.EqualTo(TransactionState.Storno));
    }

    [Test]
    public void PrepareAnswerToBank_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var payments = GetPayments();
        var vop = GetVopProcessingDatas(payments, VopHandler.EbicsStateMatch);

        var bankingService = new DummyBankingService
        {
            VopData = vop
        };

        var workload = new List<WorkloadTask>();

        var options = new Options
        {
            ArchivePath = @"d:\temp\BodoMoneyTransfer\Archive",
            OriginalArchivePath = @"d:\temp\BodoMoneyTransfer\Originals",
            QuotationFileName = @"d:\temp\BodoMoneyTransfer\QuotationFile.json",
            ErrorMailer = "Test@Test.de"
        };

        var dataHandler = new DummyDataHandler(options, workload, payments);

        var voph = new VopHandler(payments, bankingService, dataHandler);
        voph.DownloadVpz();
        voph.CheckStateForPayments();

        // Act  
        voph.PrepareAnswerToBank();

        // Assert
        var vopData1 = voph.VopPayments[0];

        Assert.That(vopData1.DataDigest, Is.Not.Null);
    }

    [Test]
    public void SendAllAnswersToBank_ValidSetupMatch_PropsSetCorrectly()
    {
        // Arrange 
        var payments = GetPayments();
        var vop = GetVopProcessingDatas(payments, VopHandler.EbicsStateMatch);

        var bankingService = new DummyBankingService
        {
            VopData = vop
        };

        var workload = new List<WorkloadTask>();

        var options = new Options
        {
            ArchivePath = @"d:\temp\BodoMoneyTransfer\Archive",
            OriginalArchivePath = @"d:\temp\BodoMoneyTransfer\Originals",
            QuotationFileName = @"d:\temp\BodoMoneyTransfer\QuotationFile.json",
            ErrorMailer = "Test@Test.de"
        };

        var dataHandler = new DummyDataHandler(options, workload, payments);

        var voph = new VopHandler(payments, bankingService, dataHandler);
        voph.DownloadVpz();
        voph.CheckStateForPayments();
        voph.PrepareAnswerToBank();

        // Act  
        voph.SendAllAnswersToBank();

        // Assert
        Assert.That(bankingService.WasUploadHveFired, Is.True);
        Assert.That(bankingService.WasUploadHvsFired, Is.False);
    }

    [Test]
    public void SendAllAnswersToBank_ValidSetupCloseMatch_PropsSetCorrectly()
    {
        // Arrange 
        var payments = GetPayments();
        var vop = GetVopProcessingDatas(payments, VopHandler.EbicsStateCloseMatch);

        var bankingService = new DummyBankingService
        {
            VopData = vop
        };

        var workload = new List<WorkloadTask>();

        var options = new Options
        {
            ArchivePath = @"d:\temp\BodoMoneyTransfer\Archive",
            OriginalArchivePath = @"d:\temp\BodoMoneyTransfer\Originals",
            QuotationFileName = @"d:\temp\BodoMoneyTransfer\QuotationFile.json",
            ErrorMailer = "Test@Test.de"
        };

        var dataHandler = new DummyDataHandler(options, workload, payments);

        var voph = new VopHandler(payments, bankingService, dataHandler);
        voph.DownloadVpz();
        voph.CheckStateForPayments();
        voph.PrepareAnswerToBank();

        // Act  
        voph.SendAllAnswersToBank();

        // Assert
        var vopData1 = voph.VopPayments[0];

        Assert.That(bankingService.WasUploadHveFired, Is.False);
        Assert.That(bankingService.WasUploadHvsFired, Is.True);
    }

    [Test]
    public void HandleAllVopResultsOnOurSide_ValidSetupMatch_CorrectActionsStarted()
    {
        // Arrange 
        var payments = GetPayments();
        var vop = GetVopProcessingDatas(payments, VopHandler.EbicsStateMatch);

        var bankingService = new DummyBankingService
        {
            VopData = vop
        };

        var workload = new List<WorkloadTask>();

        var options = new Options
        {
            ArchivePath = @"d:\temp\BodoMoneyTransfer\Archive",
            OriginalArchivePath = @"d:\temp\BodoMoneyTransfer\Originals",
            QuotationFileName = @"d:\temp\BodoMoneyTransfer\QuotationFile.json",
            ErrorMailer = "Test@Test.de"
        };

        var dataHandler = new DummyDataHandler(options, workload, payments);

        var voph = new VopHandler(payments, bankingService, dataHandler);
        voph.DownloadVpz();
        voph.CheckStateForPayments();
        voph.PrepareAnswerToBank();
        voph.SendAllAnswersToBank();

        // Act  
        voph.HandleAllVopResultsOnOurSide();

        // Assert
        Assert.That(bankingService.WasUploadHveFired, Is.True);
        Assert.That(bankingService.WasUploadHvsFired, Is.False);
        Assert.That(dataHandler.WasSetTransactionStatusFired, Is.True);
        Assert.That(dataHandler.WasMoveTransactionToArchiveFired, Is.True);
        Assert.That(dataHandler.WasDeleteTransactionFired, Is.False);
        Assert.That(dataHandler.WasHandleVpoResultFired, Is.False);
    }

    [Test]
    public void HandleAllVopResultsOnOurSide_ValidSetupCloseMatch_CorrectActionsStarted()
    {
        // Arrange 
        var payments = GetPayments();
        var vop = GetVopProcessingDatas(payments, VopHandler.EbicsStateCloseMatch);

        var bankingService = new DummyBankingService
        {
            VopData = vop
        };

        var workload = new List<WorkloadTask>();

        var options = new Options
        {
            ArchivePath = @"d:\temp\BodoMoneyTransfer\Archive",
            OriginalArchivePath = @"d:\temp\BodoMoneyTransfer\Originals",
            QuotationFileName = @"d:\temp\BodoMoneyTransfer\QuotationFile.json",
            ErrorMailer = "Test@Test.de"
        };

        var dataHandler = new DummyDataHandler(options, workload, payments);

        var voph = new VopHandler(payments, bankingService, dataHandler);
        voph.DownloadVpz();
        voph.CheckStateForPayments();
        voph.PrepareAnswerToBank();
        voph.SendAllAnswersToBank();

        // Act  
        voph.HandleAllVopResultsOnOurSide();

        // Assert
        Assert.That(bankingService.WasUploadHveFired, Is.False);
        Assert.That(bankingService.WasUploadHvsFired, Is.True);
        Assert.That(dataHandler.WasSetTransactionStatusFired, Is.False);
        Assert.That(dataHandler.WasMoveTransactionToArchiveFired, Is.False);
        Assert.That(dataHandler.WasDeleteTransactionFired, Is.True);
        Assert.That(dataHandler.WasHandleVpoResultFired, Is.True);
    }
}