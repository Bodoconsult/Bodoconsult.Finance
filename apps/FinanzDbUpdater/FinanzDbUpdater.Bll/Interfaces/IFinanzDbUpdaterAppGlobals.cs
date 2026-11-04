// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Abstractions.Delegates;
using Bodoconsult.App.Abstractions.Interfaces;

namespace FinanzDbUpdater.Bll.Interfaces;

/// <summary>
/// App globals for FinanzDbUpdater
/// </summary>
public interface IFinanzDbUpdaterAppGlobals: IAppGlobals
{
    /// <summary>
    /// Path to archive
    /// </summary>
    public string ArchivePath { get; set; }

    /// <summary>
    /// Full path to the Depot data file
    /// </summary>
    public string DepotJsonFile { get; set; }

    /// <summary>
    /// Full path to the logo for the simulation report
    /// </summary>
    public string LogoSimulationReport { get; set; }

    /// <summary>
    /// Full path to the file with the quotation data from the bank
    /// </summary>
    public string QuotationJsonFile { get; set; }

    /// <summary>
    /// Current delegate to handle exceptions
    /// </summary>
    public HandleExceptionDelegate HandleExceptionDelegate { get; set; }
}