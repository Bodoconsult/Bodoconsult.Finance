// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.Finance.Payment.Base.Interfaces;

/// <summary>
/// Payment order item
/// </summary>
public interface IPaymentOrderItem
{
    /// <summary>
    /// Product name
    /// </summary>
    string ProductName { get; set; }

    /// <summary>
    /// Product code
    /// </summary>
    string ProductCode { get; set; }

    /// <summary>
    /// Quantity
    /// </summary>
    decimal Quantity { get; set; }

    /// <summary>
    /// Price
    /// </summary>
    decimal Price { get; set; }

    /// <summary>
    /// Tax amount
    /// </summary>
    decimal Tax { get; set; }

    /// <summary>
    /// Tax rate
    /// </summary>
    decimal TaxRate { get; set; }

    /// <summary>
    /// Total amount
    /// </summary>
    decimal Total { get; }
}