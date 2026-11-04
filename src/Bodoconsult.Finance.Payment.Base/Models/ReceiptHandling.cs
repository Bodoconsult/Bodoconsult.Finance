// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


namespace Bodoconsult.Finance.Payment.Base.Models;

/// <summary>
/// Receipt status of an account statement file
/// </summary>
public enum ReceiptHandling
{
    /// <summary>
    /// Not necessary
    /// </summary>
    NotNecessary,

    /// <summary>
    /// Receipt open
    /// </summary>
    ReceiptOpen,

    /// <summary>
    /// Receipt done
    /// </summary>
    ReceiptDone
}