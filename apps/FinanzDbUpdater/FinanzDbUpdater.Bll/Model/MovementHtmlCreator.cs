// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Text;
using FinanzDbUpdater.Bll.Interfaces;

namespace FinanzDbUpdater.Bll.Model;

/// <summary>
/// <see cref="IHtmlCreator"/> implementation for account movements
/// </summary>
public sealed class MovementHtmlCreator: IHtmlCreator 
{
    /// <summary>
    /// Movement date
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Value
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Receiver or sender
    /// </summary>
    public string ReceiverSender { get; set; }

    /// <summary>
    /// Description (purpose)
    /// </summary>
    public string Description { get; set; }
    
    /// <summary>
    /// Create a partial string with HTML
    /// </summary>
    /// <returns>A string with HTML code</returns>
    public StringBuilder ToHtmlString()
    {
        var result = new StringBuilder();

        result.AppendLine($"<tr><td width=\"10%\">{Date:dd.MM.yyyy}</td><td width=\"12%\" class=\"right\"><b>{Value:#,##0.00}</b></td><td><b>{ReceiverSender}</b></br>{Description}</td></tr>");

        return result;
    }
}