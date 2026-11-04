// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using System;
using Bodoconsult.App.Abstractions.Delegates;

namespace FinanzDbUpdater.Bll.Interfaces;

/// <summary>
/// Interface for main business logic class of FinanzDbUpdater
/// </summary>
public interface IFinanzDbUpdateHandler : IDisposable
{
    /// <summary>
    /// Logo used for the simulation report header
    /// </summary>
    string LogoSimulationReport { get; }

    /// <summary>
    /// JSON file with quotations from external sources
    /// </summary>
    string QuotationJsonFile { get; }

    /// <summary>
    /// JSON file containing Depot data with quoatations
    /// </summary>
    string DepotJsonFile { get; }

    /// <summary>
    /// Current delegate for sending status message to UI
    /// </summary>
    StatusMessageDelegate StatusMessageDelegate { get; set; }

    /// <summary>
    /// Current delegate for exception handling on the UI
    /// </summary>
    HandleExceptionDelegate HandleExceptionDelegate { get; set; }

    /// <summary>
    /// Date from
    /// </summary>
    DateTime From { get; set; }

    /// <summary>
    /// Date until
    /// </summary>
    DateTime Until { get; set; }

    /// <summary>
    /// Prepare the database
    /// </summary>
    void PrepareTheDatabase();

    /// <summary>
    /// Update the quotation data
    /// </summary>
    void UpdateQuotationData();

    /// <summary>
    /// Fetch quotation history
    /// </summary>
    void QuotationHistory(string sql = "EXEC dbo.GetWPForKursabfrage 1");

    /// <summary>
    /// Prepare the data in the database (full mode)
    /// </summary>
    void PrepareDataFull();

    /// <summary>
    /// Calcute the risk of the depots
    /// </summary>
    void CalculateDepotRisk();

    /// <summary>
    /// Calcute the risk of the depot of an owver
    /// </summary>
    /// <param name="owner">Depot owner</param>
    void CalculateDepotRisk(string owner);

    void SetError(string msg, Exception ex);

    /// <summary>
    /// Check if Bewertungen files for the Stichtag exists
    /// </summary>
    void CheckStichtage();

    /// <summary>
    /// Send account statement info for all registered owner
    /// </summary>
    void SendMail();

    /// <summary>
    /// Prepare the data in the database (short mode)
    /// </summary>
    void PrepareDataShort();
}