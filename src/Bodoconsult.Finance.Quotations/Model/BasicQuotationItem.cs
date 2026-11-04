// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.Finance.Quotations.Model;

/// <summary>
/// Data class representing import data from a JSON file with current quotations
/// </summary>
public class BasicQuotationItem
{
    /// <summary>
    /// ISIN of the security
    /// </summary>
    public string Isin { get; set; }



    /// <summary>
    /// Current quotation 
    /// </summary>
    public double Quotation { get; set; }

}