// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Text;
using FinanzDbUpdater.Bll.Interfaces;

namespace FinanzDbUpdater.Bll.Model;

/// <summary>
/// Implementation of <see cref="IHtmlCreator"/> for asset value
/// </summary>
public sealed class AssetValueHtmlCreator: IHtmlCreator
{
    /// <summary>
    /// Asset ID
    /// </summary>
    public int AssetId { get; set; }

    /// <summary>
    /// Asset name
    /// </summary>
    public string AssetName { get; set; }

    /// <summary>
    /// Value amount
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Create a partial string with HTML
    /// </summary>
    /// <returns>A string with HTML code</returns>
    public StringBuilder ToHtmlString()
    {
        var result = new StringBuilder();

        result.AppendLine($"<tr><td width=\"35%\">{AssetName}</td><td width=\"15%\" class=\"right\">{Value:#,##0.00 EUR}</td><td>&nbsp</td></tr>");

        return result;
    }
}