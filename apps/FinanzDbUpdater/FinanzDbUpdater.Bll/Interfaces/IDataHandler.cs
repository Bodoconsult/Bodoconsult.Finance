// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Bodoconsult.Finance.Quotations.Model;
using FinanzDbUpdater.Bll.Model;

namespace FinanzDbUpdater.Bll.Interfaces;

/// <summary>
/// Interface for data handling implementations
/// </summary>
public interface IDataHandler: IDisposable
{
    /// <summary>
    /// Init the database for preparation
    /// </summary>
    void InitTheDatabasePreparation();

    /// <summary>
    /// Get securities for quotation feed
    /// </summary>
    /// <returns>List with securities items</returns>
    IList<SecuritiesItem> GetSecuritiesForQuotationFeed();

    /// <summary>
    /// Get securities for EURIBOR quotation feed
    /// </summary>
    /// <returns>List with securities items</returns>
    IList<SecuritiesItem> GetSecuritiesForQuotationFeedEuribor();

    /// <summary>
    /// Get securities in a flexible manner for quotation feed
    /// </summary>
    /// <returns>List with securities items</returns>
    IList<SecuritiesItem> GetSecuritiesForQuotationFeedFlexible(string sql);

    /// <summary>
    /// Update the quotation for a secuity by its ISIN
    /// </summary>
    /// <param name="isin">ISIN</param>
    /// <param name="quotation">Quotation</param>
    void UpdateQuotation(string isin, double quotation);
    void UpdateEuribor(DateTime date, double quotation);
    void UpdateEuriborData();
    void UpdateQuotationHistory(string isin, DateTime date, double quotation);
    void UpdateQuotationHistoryPrepareImport(string isin, DateTime from);

    /// <summary>
    /// Update the quotations of the securities
    /// </summary>
    void UpdateSecuritiesQuotations();

    /// <summary>
    /// Update the benchmarks for the depots
    /// </summary>
    void UpdateBenchmarks();

    /// <summary>
    /// Update the wealth of all depots
    /// </summary>
    void UpdateWealth();

    /// <summary>
    /// Execute a SQL statement
    /// </summary>
    /// <param name="sql">SQL statement to execute</param>
    void Exec(string sql);

    /// <summary>
    /// Execute a command
    /// </summary>
    /// <param name="cmd">SQL command to execute</param>
    void Exec(DbCommand cmd);

    /// <summary>
    /// Start the external jobs for data collection
    /// </summary>
    void StartExternalJobs();

    /// <summary>
    /// Update liquidity
    /// </summary>
    void UpdateLiquidity();
    IList<OwnerData> GetOwnersDataForRiskMetrics();
    DataTable GetBasicRiskDataForOwner(string owner);

    /// <summary>
    /// Get the covariance matrix for an asset owner
    /// </summary>
    /// <param name="owner">Requested asset owner</param>
    /// <returns>Covariance matrix for the owner</returns>
    DataTable GetCovarianceMatrixForOwner(string owner);

    /// <summary>
    /// Update the risk for an asset owner
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="risk"></param>
    void UpdateOwnerRisk(string owner, double risk);

    IList<OwnerData> GetOwnersDataForMcs();
    IList<OwnerData> GetOwnersDataForTimePathMcs();
    void SaveMcsResult(string owner, double median, double mean, double standardDeviation);

    /// <summary>
    /// Send an error mail to the admin
    /// </summary>
    /// <param name="subject">subject of the mail</param>
    /// <param name="message">message body of the mail</param>
    void SendMail(string subject, string message);

    /// <summary>
    /// Send an error mail to the admin
    /// </summary>
    /// <param name="to">Receiver email address</param>
    /// <param name="subject">subject of the mail</param>
    /// <param name="message">message body of the mail</param>
    void SendMail(string to, string subject, string message);

    /// <summary>
    /// Update the filepath for the valuation file for a valuation date
    /// </summary>
    /// <param name="stichtagId">Valuation date ID</param>
    /// <param name="path">Valuation file path</param>
    void UpdateValuationDateFilePath(string stichtagId, string path);

    /// <summary>
    /// Get valuation dates
    /// </summary>
    /// <returns>List with valuation dates</returns>
    IList<ValuationDate> GetValuationDates();


    void RegisterBewertungForBft(string path, string title, string keyword, int accountId);
    IList<OwnerData> GetOwnersForMailVersand();
    IList<AssetValueHtmlCreator> GetBestandswerteForOwner(string owner);
    IList<MovementHtmlCreator> GetBewegungenForAnlageobjekt(int anlageobjektId, DateTime date);
    void AufbereitenKurz();
}