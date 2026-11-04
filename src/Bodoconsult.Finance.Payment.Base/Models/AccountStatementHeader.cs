// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System;

namespace Bodoconsult.Finance.Payment.Base.Models;

/// <summary>
/// Class contains additional header data for an account statement
/// </summary>
public class AccountStatementHeader
{
    /// <summary>
    /// Start balance date
    /// </summary>
    public DateTime StartBalanceDate { get; set; }

    /// <summary>
    /// Start balance amount
    /// </summary>
    public decimal StartBalanceAmount { get; set; }

    /// <summary>
    /// Start balance currency
    /// </summary>
    public string StartBalanceCurrency { get; set; }

    /// <summary>
    /// End balance date
    /// </summary>
    public DateTime EndBalanceDate { get; set; }

    /// <summary>
    /// End balance amount
    /// </summary>
    public decimal EndBalanceAmount { get; set; }

    /// <summary>
    /// End balance currency
    /// </summary>
    public string EndBalanceCurrency { get; set; }

    /// <summary>
    /// The account number (not IBAN) of the account
    /// </summary>
    public string AccountNr { get; set; }

    /// <summary>
    /// The start date of the account statement period
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// The end date of the account statement period
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// The bank identifier (Bankleitzahl, not the BIC)
    /// </summary>
    public string BankIdentifier { get; set; }

    /// <summary>
    /// The name of the account owner
    /// </summary>
    public string AccountOwner { get; set; }
}