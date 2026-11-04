// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

namespace Bodoconsult.Finance.Payment.Banking.Hbci;

/// <summary>
/// Account balance data
/// </summary>
public class AccountBalanceData
{
    /// <summary>
    /// Account name
    /// </summary>
    public string AccountName { get; set; }

    /// <summary>
    /// Account name
    /// </summary>
    public string AccountNo { get; set; }

    /// <summary>
    /// Overdraft value
    /// </summary>
    public decimal Overdraft { get; set; }

    /// <summary>
    /// Available funds
    /// </summary>
    public decimal AvailableFunds { get; set; }

    /// <summary>
    /// Already drawn amount
    /// </summary>
    public decimal AlreadyDrawnOnAmount { get; set; }

    /// <summary>
    /// Overdraft limit
    /// </summary>
    public decimal OverdraftLimit { get; set; }

    /// <summary>
    /// Current balance
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Current amount including pending transactions
    /// </summary>
    public decimal AmountIncludingPendingTransactions { get; set; }
}