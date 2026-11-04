// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System;
using System.Collections.Generic;

namespace Bodoconsult.Finance.Payment.Base.Models;

/// <summary>
/// Represents an account statement delivered by the bank
/// </summary>
public class AccountStatement
{
    /// <summary>
    /// Opening balance amount
    /// </summary>
    public decimal OpeningBalanceAmount { get; set; }

    /// <summary>
    /// Opening balance date
    /// </summary>
    public DateTime OpeningBalanceDate { get; set; }

    /// <summary>
    /// Opening balance currency
    /// </summary>
    public string OpeningBalanceCurrency { get; set; } = "EUR";

    /// <summary>
    /// Closing balance amount
    /// </summary>
    public decimal ClosingBalanceAmount { get; set; }

    /// <summary>
    /// Closing balance date
    /// </summary>
    public DateTime ClosingBalanceDate { get; set; }

    /// <summary>
    /// Closing balance currency
    /// </summary>
    public string ClosingBalanceCurrency { get; set; } = "EUR";

    /// <summary>
    /// Available balance amount
    /// </summary>
    public decimal AvailableBalanceAmount { get; set; }

    /// <summary>
    /// Available balance date
    /// </summary>
    public DateTime AvailableBalanceDate { get; set; }

    /// <summary>
    /// Available balance currency
    /// </summary>
    public string AvailableBalanceCurrency { get; set; } = "EUR";


    /// <summary>
    /// Account statement items
    /// </summary>
    public List<AccountStatementLine> Items { get; } = [];

    /// <summary>
    /// Account number
    /// </summary>
    public string AccountNumber { get; set; }
}