// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using Bodoconsult.Finance.Payment.Base.Interfaces;
using Bodoconsult.Finance.Payment.Base.Models;

namespace Bodoconsult.Finance.Payment.Banking.Models;

/// <summary>
/// Banking payment order
/// </summary>
public class BankingOrder: IPaymentOrder
{
    /// <summary>
    /// IBAN
    /// </summary>
    public string Iban { get; set; }

    /// <summary>
    /// BIC
    /// </summary>
    public string Bic { get; set; }

    /// <summary>
    /// Payer or payee
    /// </summary>
    public string PayerOrPayee { get; set; }

    /// <summary>
    /// Currency
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    /// Payment order items
    /// </summary>
    public IList<IPaymentOrderItem> PaymentOrderItems { get; set; }

    /// <summary>
    /// Subtotal amount
    /// </summary>
    public decimal SubTotal { get; set; }

    /// <summary>
    /// Total amount
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Tay amount
    /// </summary>
    public decimal Tax { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Invoice number
    /// </summary>
    public string InvoiceNumber { get; set; }

    /// <summary>
    /// Add a payment order item
    /// </summary>
    /// <param name="productName">Product name</param>
    /// <param name="productCode">Product code</param>
    /// <param name="price">Price per unit</param>
    /// <param name="quantity">Quantity</param>
    /// <param name="tax">Tay rate</param>
    public void AddOrderItem(string productName, string productCode, decimal price, decimal quantity, decimal tax)
    {

    }

    /// <summary>
    /// Add a payment order item
    /// </summary>
    /// <param name="item">Payment order item</param>
    public void AddOrderItem(IPaymentOrderItem item)
    {

    }

    /// <summary>
    /// Payment transaction result
    /// </summary>
    public PaymentMessage Result { get; set; }

    /// <summary>
    /// Current UID of the order
    /// </summary>
    public Guid Guid { get; set; }

    /// <summary>
    /// SQL executed on transfer success. Only relevante for VOP-opt-out transfers
    /// </summary>
    public string ExecOnSuccess { get; set; }

    /// <summary>
    /// SQL executed on transfer error. Only relevante for VOP-opt-out transfers
    /// </summary>
    public string ExecOnFailure { get; set; }
}