// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.Finance.Payment.Base.Interfaces;

namespace Bodoconsult.Finance.Payment.Banking.Interfaces;

/// <summary>
/// Base interface for banking accounts
/// </summary>
public interface IBankingContact: IPaymentContact
{
    /// <summary>
    /// Account name given by the bank internal system
    /// </summary>
    string AccountName { get; set; }

    /// <summary>
    /// Account number
    /// </summary>
    string AccountNr { get; set; }

    /// <summary>
    /// BIC
    /// </summary>
    string Bic { get; set; }
 
    /// <summary>
    /// IBAN
    /// </summary>
    string Iban { get; set; }

    /// <summary>
    /// Name of the contact (see HomeBankingAdministrator)
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Passphrase for opening key files (see HomeBankingAdministrator). If a <see cref="Pin"/> is provided the passphrase is ignored
    /// </summary>
    string Passphrase { get; set; }

    /// <summary>
    /// PIN for the account
    /// </summary>
    string Pin { get; set; }
}