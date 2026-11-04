// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;

namespace FinanzDbUpdater.Bll.Model;

/// <summary>
/// Data of a valuation date
/// </summary>
public sealed class ValuationDate
{
    /// <summary>
    /// Validation date GUID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Validation date
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Owner
    /// </summary>
    public string Owner { get; set; }

    /// <summary>
    /// BodoFileTransfer account ID
    /// </summary>
    public int BftAccountId { get; set; }
    
    /// <summary>
    /// Is the path existing
    /// </summary>
    public bool IsPathExisting { get; set; }

    /// <summary>
    /// Valuation schema
    /// </summary>
    public string ValuationSchema { get; set; }

    /// <summary>
    /// Valuation file path
    /// </summary>
    public string ValuationFilePath { get; set; }
}