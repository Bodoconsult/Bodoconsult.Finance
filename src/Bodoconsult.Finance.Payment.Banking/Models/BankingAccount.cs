// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.Finance.Payment.Banking.Models;

/// <summary>
/// HBCI banking account
/// </summary>
public class BankingAccount
{
    /// <summary>
    /// Name of the HBCI contact the account is related to
    /// </summary>
    public string Contact { get; set; }

    /// <summary>
    /// Account number
    /// </summary>
    public string AccountNumber { get; set; }


    /// <summary>
    /// IBAN of the account
    /// </summary>
    public string Iban { get; set; }

}