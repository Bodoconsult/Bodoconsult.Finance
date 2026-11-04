// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.Finance.Payment.Base.Models;

namespace Bodoconsult.Finance.Payment.Base;

/// <summary>
/// Show a payment message
/// </summary>
/// <param name="message">Payment message</param>
public delegate void ShowPaymentMessageDelegate(PaymentMessage message);