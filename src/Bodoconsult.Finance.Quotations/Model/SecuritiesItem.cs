// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Collections.Generic;

namespace Bodoconsult.Finance.Quotations.Model;

/// <summary>
/// Represents a security with its data
/// </summary>
public class SecuritiesItem
{
    /// <summary>
    /// Wertpapierkennummer
    /// </summary>
    public string Wkn { get; set; }

    /// <summary>
    /// International security identification number
    /// </summary>
    public string Isin { get; set; }

    /// <summary>
    /// Token identifying service provider and security: Example: YAHOO:^GSPC for data provider Yahoo and S+P 500 index
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Current quotation
    /// </summary>
    public double Quotation { get; set; }

    /// <summary>
    /// Quotation
    /// </summary>
    public List<QuotationItem> History { get; } = [];

    /// <summary>
    /// Data changed?
    /// </summary>
    public bool Changed { get; set; }
}