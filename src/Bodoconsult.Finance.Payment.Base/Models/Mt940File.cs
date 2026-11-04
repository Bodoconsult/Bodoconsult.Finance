// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;

namespace Bodoconsult.Finance.Payment.Base.Models;

/// <summary>
/// Class represents all required data for a MT940 file
/// </summary>
public class Mt940File
{
    /// <summary>
    /// References
    /// </summary>
    public string References { get; set; } = "STARTUMS";

    /// <summary>
    /// Account owner name
    /// </summary>
    public string AccountOwner { get; set; }

    /// <summary>
    /// Account number
    /// </summary>
    public string AccountNumber { get; set; }

    /// <summary>
    /// Statement number
    /// </summary>
    public string StatementNumber { get; set; } = "0";

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
    /// Account statement lines
    /// </summary>
    public IList<AccountStatementLine> Items { get; set; }

}