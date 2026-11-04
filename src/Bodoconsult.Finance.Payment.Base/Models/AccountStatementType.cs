// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.Finance.Payment.Base.Models;

/// <summary>
/// Type of account statement
/// </summary>
public enum AccountStatementType
{
    /// <summary>
    /// MT940 file
    /// </summary>
    Mt940,

    /// <summary>
    /// PDF file
    /// </summary>
    Pdf
}