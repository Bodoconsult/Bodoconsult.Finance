// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System;

namespace Bodoconsult.Finance.Payment.Base.Models;

/// <summary>
/// Class contains data of a row of an account statement
/// </summary>
public struct AccountStatementLine
{
    /// <summary>
    /// Name
    /// </summary>
    public string Name;

    /// <summary>
    /// Validity date
    /// </summary>
    public DateTime ValidityDate;

    /// <summary>
    /// Entry date
    /// </summary>
    public DateTime EntryDate;

    /// <summary>
    /// Business transaction text
    /// </summary>
    public string BusinessTransactionText;

    /// <summary>
    /// Amount
    /// </summary>
    public decimal Amount;

    /// <summary>
    /// Fee amount
    /// </summary>
    public decimal FeeAmount;

    /// <summary>
    /// Currency code
    /// </summary>
    public string CurrencyCode;

    /// <summary>
    /// Original amount in currency
    /// </summary>
    public double OriginalAmount;

    /// <summary>
    /// Original currency code
    /// </summary>
    public string OriginalCurrencyCode;

    /// <summary>
    /// Purpose
    /// </summary>
    public string Purpose;

    /// <summary>
    /// Transaction ID
    /// </summary>
    public string Transactionid;

    /// <summary>
    /// Business transaction code
    /// </summary>
    public string BusinessTransactionCode;

    /// <summary>
    /// Bank code
    /// </summary>
    public string BankCode;

    /// <summary>
    /// Account number
    /// </summary>
    public string AccountNo;

    /// <summary>
    /// Primanota
    /// </summary>
    public string PrimaNota;

    /// <summary>
    /// Transaction type
    /// </summary>
    public string TransactionType;

    /// <summary>
    /// Text key supplement
    /// </summary>
    public string TextKeySupplement;
}