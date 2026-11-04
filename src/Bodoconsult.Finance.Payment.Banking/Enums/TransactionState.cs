// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

namespace Bodoconsult.Finance.Payment.Banking.Enums;

/// <summary>
/// The current state of a money transfer transaction
/// </summary>
public enum TransactionState
{
    /// <summary>
    /// Unhandled
    /// </summary>
    Unhandled,

    /// <summary>
    /// DeliveredToBank
    /// </summary>
    DeliveredToBank,

    /// <summary>
    /// Successful
    /// </summary>
    Successful,

    /// <summary>
    /// Storno
    /// </summary>
    Storno
}