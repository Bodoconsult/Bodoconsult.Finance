// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.Finance.Payment.Banking.Ebics;
using Bodoconsult.Finance.Payment.Banking.Enums;
using Bodoconsult.Finance.Payment.Banking.Interfaces;
using BodoMoneyTransfer.Dal.Interfaces;
using BodoMoneyTransfer.Dal.Model;
using System.Collections.Generic;
using Bodoconsult.App.Extensions;

namespace BodoMoneyTransfer.Dal.Implementations;

/// <summary>
/// This class handles open money transfers needing a VOP check
/// </summary>
public class VopHandler
{

    private readonly IDataHandler _dataHandler;

    public const string EbicsStateMatch = "RCVC";
    public const string EbicsStateCloseMatch = "RVMC";
    public const string EbicsStateNoMatch = "RVNM";

    public VopHandler(List<Payment> payments, IBankingService bankingService, IDataHandler dataHandler)
    {
        Payments = payments;
        BankingService = bankingService;
        _dataHandler = dataHandler;

        foreach (var payment in Payments)
        {
            var data = MapPaymentToVopProcessingData(payment);
            VopPayments.Add(data);
        }
    }

    /// <summary>
    /// Map <see cref="Payment"/> to <see cref="VopProcessingData"/>
    /// </summary>
    /// <param name="payment">Payment</param>
    /// <returns>VOP processing data</returns>
    public static VopProcessingData MapPaymentToVopProcessingData(Payment payment)
    {
        return new VopProcessingData
        {
            TransactionId = payment.Guid,
            EndToEndId = payment.Guid.ToPlainString(),
            ExecOnVpoCloseMatch = payment.ExecOnVpoCloseMatch,
            Iban = payment.Iban
        };
    }

    /// <summary>
    /// Current banking service
    /// </summary>
    public IBankingService BankingService { get; }

    /// <summary>
    ///  Payments waiting for VOP
    /// </summary>
    public List<Payment> Payments { get; } 

    /// <summary>
    /// Current VOP payment data
    /// </summary>
    public List<VopProcessingData> VopPayments { get; } = [];



    public void DownloadVpz()
    {
        // Get the VPZ payment status report from the bank
        // Missing data in VopPayements are added
        BankingService.DownloadVpz(VopPayments);
    }

    public void CheckStateForPayments()
    {
        foreach (var payment in VopPayments)
        {
            CheckPaymentState(payment);
        }
    }

    public void CheckPaymentState(VopProcessingData payment)
    {
        // Exact match
        if (string.IsNullOrEmpty(payment.State) || payment.State == EbicsStateMatch)
        {
            payment.TransactionState = TransactionState.Successful;
        }
        // Close match
        else if (payment.State == EbicsStateCloseMatch)
        {
            payment.TransactionState = TransactionState.Storno;
        }
        // No match
        else if (payment.State == EbicsStateNoMatch)
        {
            payment.TransactionState = TransactionState.Storno;
        }
        // Other reasons for storno
        else
        {
            payment.TransactionState = TransactionState.Storno;
        }
    }

    /// <summary>
    /// Prepare the data needed for answers to bank
    /// </summary>
    public void PrepareAnswerToBank()
    {
        BankingService.DownloadHvu(VopPayments);

        foreach (var payment in VopPayments)
        {
            BankingService.DownloadHvd(payment);
        }
    }

    /// <summary>
    /// Send all answers HVE and HVS to bank for all payments
    /// </summary>
    public void SendAllAnswersToBank()
    {
        foreach (var payment in VopPayments)
        {
            SendAnswerToBankForPayment(payment);
        }
    }

    /// <summary>
    /// Send all answers HVE and HVS to bank for a single payment. Public only for unit testing
    /// </summary>
    public void SendAnswerToBankForPayment(VopProcessingData payment)
    {
        // Success : send command HVE with VEU to bank
        if (payment.TransactionState == TransactionState.Successful)
        {
            payment.WasBankAnswerSuccessful = BankingService.UploadHve(payment);
            return;
        }

        // Storno: send storno command HVS to bank
        if (payment.TransactionState == TransactionState.Storno)
        {
            payment.WasBankAnswerSuccessful = BankingService.UploadHvs(payment);
        }
    }

    /// <summary>
    /// Handle Vop results for all payments on our side
    /// </summary>
    public void HandleAllVopResultsOnOurSide()
    {
        foreach (var payment in VopPayments)
        {
            HandleVopResultOnOurSide(payment);
        }
    }

    /// <summary>
    /// Handle Vop results for single payment on our side
    /// </summary>
    public void HandleVopResultOnOurSide(VopProcessingData payment)
    {

        if (payment.TransactionState == TransactionState.Successful)
        {
            // Set state
            _dataHandler.SetTransactionStatus(payment.TransactionId, TransactionState.Successful);

            // Migrate transaction to archive
            _dataHandler.MoveTransactionToArchive(payment.TransactionId);

            return;
        }

        if (payment.TransactionState != TransactionState.Storno)
        {
            return;
        }

        // Close match
        if (payment.State == EbicsStateCloseMatch)
        {
            // Run VOP check SQL
            _dataHandler.HandleVpoResult(payment.ExecOnVpoCloseMatch, payment.Iban, payment.CreditorName);
        }

        // Inform user
        _dataHandler.SendMail("VOP check banking payment:", $"Transaction {payment.TransactionId}: payee name for IBAN {payment.Iban} should be >>{payment.CreditorName}<<");

        // Delete transaction
        _dataHandler.DeleteTransaction(payment.TransactionId);
    }
}