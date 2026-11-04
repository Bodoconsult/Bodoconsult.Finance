// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System;

namespace Bodoconsult.Finance.Quotations.Model;

/// <summary>
/// Keeps quotation for certain date
/// </summary>
public class QuotationItem
{
    /// <summary>
    /// Date
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Quotation at <see cref="Date"/>
    /// </summary>
    public double Quotation { get; set; }
}