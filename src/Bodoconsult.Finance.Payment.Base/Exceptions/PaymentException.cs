// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;

namespace Bodoconsult.Finance.Payment.Base.Exceptions;

/// <summary>
/// Payment related exception
/// </summary>
public class PaymentException : ApplicationException
{
    /// <summary>
    /// Ctor providing a message
    /// </summary>
    /// <param name="message">Payment related message</param>
    public PaymentException(string message) : base(message) { }

    /// <summary>
    /// Ctor providing a message
    /// </summary>
    /// <param name="message">Payment related message</param>
    /// <param name="inner">Inner exception</param>
    public PaymentException(string message, Exception inner) : base(message, inner) { }
}