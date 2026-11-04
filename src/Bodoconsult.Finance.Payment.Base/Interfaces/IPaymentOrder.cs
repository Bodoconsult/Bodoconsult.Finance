// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using Bodoconsult.Finance.Payment.Base.Models;

namespace Bodoconsult.Finance.Payment.Base.Interfaces;

/// <summary>
/// Payment order
/// </summary>
public interface IPaymentOrder
{
    /// <summary>
    /// List of all order items
    /// </summary>
    IList<IPaymentOrderItem> PaymentOrderItems { get; }

    /// <summary>
    /// Subtotal for order
    /// </summary>
    decimal SubTotal { get; }

    /// <summary>
    /// Total order amount
    /// </summary>
    /// <returns></returns>
    decimal Total { get; }

    /// <summary>
    /// Total tax
    /// </summary>
    /// <returns></returns>
    decimal Tax { get; }

    /// <summary>
    /// Description for invoice number
    /// </summary>
    string Description { get; set; }

    /// <summary>
    /// Invoice number
    /// </summary>
    string InvoiceNumber { get; set; }

    /// <summary>
    /// Add a paypal order item
    /// </summary>
    /// <param name="productName">our product name</param>
    /// <param name="productCode">our product code</param>
    /// <param name="price">price for total quantity</param>
    /// <param name="quantity">quantity</param>
    /// <param name="tax">tax</param>
    void AddOrderItem(string productName,
        string productCode,
        decimal price,
        decimal quantity,
        decimal tax);

    /// <summary>
    /// Add an order item
    /// </summary>
    /// <param name="item">Order item</param>
    void AddOrderItem(IPaymentOrderItem item);

    /// <summary>
    /// Result of payment transaction after running the transaction
    /// </summary>
    PaymentMessage Result { get; set; }

    /// <summary>
    /// GUID of the payment order
    /// </summary>
    Guid Guid { get; set; }

    /// <summary>
    /// SQL to be executed on the current connection in case of successful processing of the order
    /// </summary>
    string ExecOnSuccess { get; set; }

    /// <summary>
    /// SQL to be executed on the current connection in case of an error processing the order
    /// </summary>
    string ExecOnFailure { get; set; }
}