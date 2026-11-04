// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.Finance.Payment.Base.Interfaces;

/// <summary>
/// Basic interface for payment contacts
/// </summary>
public interface IPaymentContact
{
    /// <summary>
    /// Type description of the contact (HBCI, EBICS, PAYPAL, FINAPI, ...)
    /// </summary>
    string ContactType { get; }

    /// <summary>
    /// Generate a readable information
    /// </summary>
    /// <returns>Readable contact information</returns>
    string ToReadableString();
}