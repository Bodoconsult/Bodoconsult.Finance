// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.Finance.Payment.Base.Models;

/// <summary>
/// A securities item in a safekeeping account
/// </summary>
public class SafekeepingItem
{
    /// <summary>
    /// German Wertpapierkennnummer WKN
    /// </summary>
    public string Wkn { get; set; }

    /// <summary>
    /// international ISIN
    /// </summary>
    public string Isin { get; set; }

    /// <summary>
    /// Title of the securities item
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Quantity of the item in the safekeeping account
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Price per unit of the item
    /// </summary>
    public decimal PricePerUnitAmount { get; set; }

    /// <summary>
    /// Currency code of the price per unit
    /// </summary>
    public string PricePerUnitCurrencyCode { get; set; }
}