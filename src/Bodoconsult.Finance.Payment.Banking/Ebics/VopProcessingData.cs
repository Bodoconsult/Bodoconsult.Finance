// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System;
using Bodoconsult.Finance.Payment.Banking.Enums;

namespace Bodoconsult.Finance.Payment.Banking.Ebics;

/// <summary>
/// Data required or filled by the VOP process
/// </summary>
public class VopProcessingData
{
    /// <summary>
    /// UID of the money transfer transaction
    /// </summary>
    public Guid TransactionId { get; set; }

    /// <summary>
    /// End-to-end-ID
    /// </summary>
    public string EndToEndId { get; set; }

    /// <summary>
    /// IBAN of the payee
    /// </summary>
    public string Iban { get; set; }

    /// <summary>
    /// Current state of the money transfer
    /// </summary>
    public string State { get; set; }

    /// <summary>
    /// Partner ID
    /// </summary>
    public string PartnerId { get; set; }

    /// <summary>
    /// Order type in the system of the bank
    /// </summary>
    public string OrderType { get; set; }

    /// <summary>
    /// Order ID in the system of the bank
    /// </summary>
    public string OrderId { get; set; }

    /// <summary>
    /// Hashcode of the order data 
    /// </summary>
    public string DataDigest { get; set; }

    /// <summary>
    /// Current transaction state
    /// </summary>
    public TransactionState TransactionState { get; set; }

    /// <summary>
    /// The name of the creditor given by the bank
    /// </summary>
    public string CreditorName { get; set; }

    /// <summary>
    /// Message to the transaction author
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// SQL to execute on VOP close match result
    /// </summary>
    public string ExecOnVpoCloseMatch { get; set; }

    /// <summary>
    /// Was the answer the bank sent successful
    /// </summary>
    public bool WasBankAnswerSuccessful { get; set; }
}