// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.Finance.Payment.Base.Helpers;

/// <summary>
/// Enum for the results of a IBAN validation
/// </summary>
public enum IbanValidationResult
{
    /// <summary>
    /// Fails due to value missing
    /// </summary>
    ValueMissing,

    /// <summary>
    /// Fails due to being too short
    /// </summary>
    ValueTooSmall,

    /// <summary>
    /// Fails due to being too long
    /// </summary>
    ValueTooBig,

    /// <summary>
    /// Fails due to module 97 check
    /// </summary>
    ValueFailsModule97Check,

    /// <summary>
    /// IBAN is valid
    /// </summary>
    IsValid
}