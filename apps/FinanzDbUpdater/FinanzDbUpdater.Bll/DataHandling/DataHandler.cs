// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.Database.Interfaces;
using Bodoconsult.Finance.Quotations.Model;
using FinanzDbUpdater.Bll.Interfaces;
using FinanzDbUpdater.Bll.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace FinanzDbUpdater.Bll.DataHandling;

/// <summary>
/// Current implementation of data handling
/// </summary>
public sealed class DataHandler : IDataHandler
{
    private readonly IConnManager _db;
    private readonly IFinanzDbUpdaterAppGlobals _appGlobals;
    private readonly string _errorMailAddress;
    private readonly IAppLoggerProxy _logger;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="db">Current DB conn manager</param>
    /// <param name="appGlobals">Current app globals</param>
    public DataHandler(IConnManager db, IAppGlobals appGlobals)
    {
        _db = db;

        if (appGlobals is not IFinanzDbUpdaterAppGlobals globals)
        {
            throw new ArgumentException("AppGlobals is not IFinanzDbUpdaterAppGlobals");
        }
        _appGlobals = globals;

        _errorMailAddress = _appGlobals.AppStartParameter.ErrorMailAddress;

        ArgumentNullException.ThrowIfNull(_appGlobals.Logger);
        _logger = _appGlobals.Logger;
    }

    private void SetError(string message, Exception exception)
    {
        _logger.LogError(message, exception);
        _appGlobals.StatusMessageDelegate?.Invoke(message);
    }

    /// <summary>
    /// Prepare database
    /// </summary>
    public void InitTheDatabasePreparation()
    {
        Exec("EXEC dbo.VorbereitenKursabfrage");
    }

    /// <summary>
    /// Get securities for quotation feed
    /// </summary>
    /// <returns>List with securities items</returns>
    public IList<SecuritiesItem> GetSecuritiesForQuotationFeed()
    {
        var result = new List<SecuritiesItem>();

        var dt = _db.GetDataReader("EXEC dbo.GetWPForKursabfrage");
        while (dt.Read())
        {
            var wp = new SecuritiesItem
            {
                Isin = dt["wp_isin"].ToString()?.Trim(),
                Wkn = dt["wp_wkn"].ToString()?.Trim(),
                Token = dt["wp_yahooquery"].ToString()?.Trim()
            };

            result.Add(wp);
        }

        return result;
    }

    /// <summary>
    /// Get securities for EURIBOR quotation feed
    /// </summary>
    /// <returns>List with securities items</returns>
    public IList<SecuritiesItem> GetSecuritiesForQuotationFeedEuribor()
    {
        var result = new List<SecuritiesItem>();

        var dt = _db.GetDataReader("EXEC dbo.GetWPForKursabfrage 2");
        while (dt.Read())
        {
            var wp = new SecuritiesItem
            {
                Isin = dt["wp_isin"].ToString()?.Trim(),
                Wkn = dt["wp_wkn"].ToString()?.Trim(),
                Token = dt["wp_yahooquery"].ToString()?.Trim()
            };

            result.Add(wp);
        }

        return result;
    }

    /// <summary>
    /// Get securities in a flexible manner for quotation feed
    /// </summary>
    /// <returns>List with securities items</returns>
    public IList<SecuritiesItem> GetSecuritiesForQuotationFeedFlexible(string sql)
    {
        var result = new List<SecuritiesItem>();

        var dt = _db.GetDataReader(sql);
        while (dt.Read())
        {
            var wp = new SecuritiesItem
            {
                Isin = dt["wp_isin"].ToString()?.Trim(),
                Wkn = dt["wp_wkn"].ToString()?.Trim(),
                Token = dt["wp_yahooquery"].ToString()?.Trim()
            };

            result.Add(wp);
        }

        return result;
    }

    /// <summary>
    /// Update the quotation for a secuity by its ISIN
    /// </summary>
    /// <param name="isin">ISIN</param>
    /// <param name="quotation">Quotation</param>
    public void UpdateQuotation(string isin, double quotation)
    {
        var lo = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = "dbo.UpdateWPKursISIN"
        };
        lo.Parameters.Add("@ISIN", SqlDbType.VarChar);
        lo.Parameters[0].Value = isin;
        lo.Parameters.Add("@K", SqlDbType.Float);
        lo.Parameters[1].Value = quotation;
        Exec(lo);
    }

    public void UpdateEuribor(DateTime date, double quotation)
    {
        var lo = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = "dbo.spEuribor_Update"
        };
        lo.Parameters.Add("@Datum", SqlDbType.Date);
        lo.Parameters.Add("@Zinssatz", SqlDbType.Money);
        lo.Parameters[0].Value = date;
        lo.Parameters[1].Value = quotation;
        Exec(lo);
    }

    public void UpdateEuriborData()
    {
        var lo1 = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = "dbo.UpdateWpEuribor"
        };

        Exec(lo1);
    }

    public void UpdateQuotationHistory(string isin, DateTime date, double quotation)
    {
        var lo = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = "dbo.UpdateWPKursHistorie"
        };
        lo.Parameters.Add("@ISIN", SqlDbType.VarChar);
        lo.Parameters[0].Value = isin;
        lo.Parameters.Add("@D", SqlDbType.DateTime);
        lo.Parameters[1].Value = date;
        lo.Parameters.Add("@K", SqlDbType.Float);
        lo.Parameters[2].Value = quotation;
        Exec(lo);
    }

    public void UpdateQuotationHistoryPrepareImport(string isin, DateTime from)
    {
        var lo1 = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = "dbo.UpdateWPHistoriePrepareImport"
        };
        lo1.Parameters.Add("@ISIN", SqlDbType.VarChar);
        lo1.Parameters[0].Value = isin;
        lo1.Parameters.Add("@Date", SqlDbType.DateTime);
        lo1.Parameters[1].Value = from;
        Exec(lo1);
    }

    /// <summary>
    /// Update the quotations of the securities
    /// </summary>
    public void UpdateSecuritiesQuotations()
    {
        Exec("EXEC dbo.UpdateWPKurse");
    }

    /// <summary>
    /// Update the benchmarks for the depots
    /// </summary>
    public void UpdateBenchmarks()
    {
        Exec("exec dbo.UpdateBenchmarks");
    }

    /// <summary>
    /// Update the wealth of all depots
    /// </summary>
    public void UpdateWealth()
    {
        //Exec("EXEC dbo.Aktualisieren", false);

        Exec("EXEC dbo.UpdateCustomer");

        Exec("EXEC dbo.UpdateBewegungen");

        Exec("EXEC dbo.Update_Bestandswerte");

        Exec("EXEC dbo.UpdatePortfolioStrukturen ");

        Exec("exec spUpdateSteuer");

        Exec("exec UpdateInternerZinsfuß");

        Exec("exec updateLiquidität");

        Exec("exec UpdateAssetAllokation");

        Exec("exec dbo.UpdateAnteilsbewertung");

        Exec("exec dbo.UpdateAnteilsbuchhaltung");

        // Do it again...
        Exec("exec dbo.UpdateAnteilsbewertung");

        Exec("exec dbo.UpdateAnteilsbuchhaltung");


        Exec("exec dbo.UpdatePortfolioRisiko 3");

        Exec("exec dbo.UpdatePortfolioRisiko 12");

        Exec("exec dbo.UpdatePortfolioRisiko 120");

        Exec("exec UpdateFinancialPlanning");

        Exec("exec UpdateVermögensstand");

        //Exec("", false);

        //Exec("", false);

        //Exec("", false);

        //Exec("", false);
    }

    /// <summary>
    /// Execute a SQL statement
    /// </summary>
    /// <param name="sql">SQL statement to execute</param>
    public void Exec(string sql)
    {
        try
        {
            _db.Exec(sql);
        }
        catch (Exception e)
        {
            SetError($"Exec:Sql:{sql}", e);
        }
    }

    /// <summary>
    /// Execute a command
    /// </summary>
    /// <param name="cmd">SQL command to execute</param>
    public void Exec(DbCommand cmd)
    {
        try
        {
            _db.Exec(cmd);
        }
        catch (Exception e)
        {
            SetError($"Exec:Sql:{cmd.CommandText}", e);
        }
    }

    /// <summary>
    /// Start the external jobs for data collection
    /// </summary>
    public void StartExternalJobs()
    {
        Exec("EXEC dbo.spSQL_Jobs_Start");
    }

    /// <summary>
    /// Update liquidity
    /// </summary>
    public void UpdateLiquidity()
    {
        Exec("EXEC dbo.UpdateLiquidität");
    }


    public IList<OwnerData> GetOwnersDataForRiskMetrics()
    {

        var result = new List<OwnerData>();

        var dt = _db.GetDataTable("exec dbo.GetOwner");

        foreach (DataRow row in dt.Rows)
        {
            var o = new OwnerData { Name = row["Owner"].ToString() };
            result.Add(o);
        }

        return result;
    }

    public DataTable GetBasicRiskDataForOwner(string owner)
    {
        var lo = new SqlCommand { CommandType = CommandType.StoredProcedure, CommandText = "dbo.GetDepotAnteile" };
        lo.Parameters.Add("@o", SqlDbType.VarChar);
        lo.Parameters[0].Value = owner;

        return _db.GetDataTable(lo);
    }

    public DataTable GetCovarianceMatrixForOwner(string owner)
    {
        var kov = new SqlCommand { CommandType = CommandType.StoredProcedure, CommandText = "[dbo].[GetKovarianzDepot2]" };
        kov.Parameters.Add("@Owner", SqlDbType.VarChar);
        kov.Parameters[0].Value = owner;
        return _db.GetDataTable(kov);
    }

    public void UpdateOwnerRisk(string owner, double risk)
    {
        var lo = new SqlCommand { CommandType = CommandType.StoredProcedure, CommandText = "[dbo].[SetDepotRisiko]" };
        lo.Parameters.Add("@Owner", SqlDbType.VarChar);
        lo.Parameters[0].Value = owner;
        lo.Parameters.Add("@Risk", SqlDbType.Real);
        lo.Parameters[1].Value = risk;
        Exec(lo);
    }


    public IList<OwnerData> GetOwnersDataForMcs()
    {

        var result = new List<OwnerData>();

        var dt = _db.GetDataTable("exec dbo.spMonteCarlo_Start");

        foreach (DataRow row in dt.Rows)
        {
            var o = new OwnerData
            {
                Name = row["Owner"].ToString(),
                McsNumberOfRuns = Convert.ToInt32(row["Anzahl"].ToString())
            };
            result.Add(o);
        }

        return result;
    }


    public IList<OwnerData> GetOwnersDataForTimePathMcs()
    {

        var result = new List<OwnerData>();

        var dt = _db.GetDataTable("exec dbo.spMonteCarlo_Start");

        foreach (DataRow row in dt.Rows)
        {
            var o = new OwnerData
            {
                Name = row["Owner"].ToString(),
                McsNumberOfRuns = Convert.ToInt32(row["Anzahl"].ToString()),
                McsMasterFile = row["AN_MCS_MasterFile"].ToString(),
                McsNumberOfPeriods = Convert.ToInt32(row["AN_MCS_NumberOfPeriods"].ToString())
            };
            result.Add(o);
        }

        return result;
    }

    //public IEnumerable<IDistributionInputData> GetSimulationInputData(string owner)
    //{
    //    var result = new List<IDistributionInputData>();

    //    var dt = _db.GetDataTable($"EXEC dbo.spMonteCarloSimulation '{owner}'").AsEnumerable().ToList();
    //    foreach (var r in dt)
    //    {
    //        switch (r["Verteilung"].ToString())
    //        {
    //            case "NOR":
    //                var norm = new NormalDistributionInput
    //                {
    //                    Mean = Convert.ToDouble(r["P1"].ToString()),
    //                    StandardDeviation = Convert.ToDouble(r["P2"].ToString()),
    //                    Percentage = Convert.ToDouble(r["Anteil"].ToString())
    //                };
    //                result.Add(norm);
    //                break;
    //            case "TRI":
    //                var tri = new TriangularDistributionInput
    //                {
    //                    Alpha = Convert.ToDouble(r["P1"].ToString()),
    //                    Beta = Convert.ToDouble(r["P2"].ToString()),
    //                    Gamma = Convert.ToDouble(r["P3"].ToString()),
    //                    Percentage = Convert.ToDouble(r["Anteil"].ToString())
    //                };
    //                result.Add(tri);


    //                break;
    //            default:
    //                var log = new LogNormalDistributionInput
    //                {
    //                    Mean = Convert.ToDouble(r["P1"].ToString()),
    //                    StandardDeviation = Convert.ToDouble(r["P2"].ToString()),
    //                    Percentage = Convert.ToDouble(r["Anteil"].ToString())
    //                };
    //                result.Add(log);
    //                break;
    //        }
    //    }
    //    return result;
    //}


    public void SaveMcsResult(string owner, double median, double mean, double standardDeviation)
    {
        var lo = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = "dbo.spMonteCarlo_ErgebnisSpeichern"
        };
        lo.Parameters.Add("@Owner", SqlDbType.VarChar);
        lo.Parameters[0].Value = owner;
        lo.Parameters.Add("@Median", SqlDbType.Real);
        lo.Parameters[1].Value = median;
        lo.Parameters.Add("@Average", SqlDbType.Real);
        lo.Parameters[2].Value = mean;
        lo.Parameters.Add("@Risk", SqlDbType.Real);
        lo.Parameters[3].Value = standardDeviation;
        Exec(lo);
    }

    //public List<IDistributionInputData> GetPortfolioForTimePathMcs(string owner)
    //{
    //    var portfolio = new List<IDistributionInputData>();

    //    var dt = _db.GetDataTable($"EXEC dbo.spMonteCarloSimulation '{owner}'").AsEnumerable().ToList();
    //    foreach (var r in dt)
    //    {
    //        switch (r["Verteilung"].ToString())
    //        {
    //            case "NOR":
    //                var norm = new NormalDistributionInput
    //                {
    //                    Mean = Convert.ToDouble(r["P1"].ToString()),
    //                    StandardDeviation = Convert.ToDouble(r["P2"].ToString()),
    //                    Percentage = Convert.ToDouble(r["Anteil"].ToString())
    //                };
    //                portfolio.Add(norm);
    //                break;

    //            case "TRI":
    //                var tri = new TriangularDistributionInput
    //                {
    //                    Alpha = Convert.ToDouble(r["P1"].ToString()),
    //                    Beta = Convert.ToDouble(r["P2"].ToString()),
    //                    Gamma = Convert.ToDouble(r["P3"].ToString()),
    //                    Percentage = Convert.ToDouble(r["Anteil"].ToString())
    //                };
    //                portfolio.Add(tri);


    //                break;

    //            default:
    //                var log = new LogNormalDistributionInput
    //                {
    //                    Mean = Convert.ToDouble(r["P1"].ToString()),
    //                    StandardDeviation = Convert.ToDouble(r["P2"].ToString()),
    //                    Percentage = Convert.ToDouble(r["Anteil"].ToString())
    //                };
    //                portfolio.Add(log);
    //                break;
    //        }
    //    }

    //    return portfolio;
    //}

    /// <summary>
    /// Send an error mail to the admin
    /// </summary>
    /// <param name="subject">subject of the mail</param>
    /// <param name="message">message body of the mail</param>
    public void SendMail(string subject, string message)
    {
        if (string.IsNullOrEmpty(_errorMailAddress))
        {
            return;
        }
        var lo = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = "dbo.[spMAIL_New]"
        };
        lo.Parameters.Add("@To", SqlDbType.VarChar);
        lo.Parameters[0].Value = _errorMailAddress;
        lo.Parameters.Add("@Subject", SqlDbType.VarChar);
        lo.Parameters[1].Value = subject;
        lo.Parameters.Add("@Msg", SqlDbType.VarChar);
        lo.Parameters[2].Value = message;
        Exec(lo);
    }

    /// <summary>
    /// Send an error mail to the admin
    /// </summary>
    /// <param name="to">Receiver email address</param>
    /// <param name="subject">subject of the mail</param>
    /// <param name="message">message body of the mail</param>
    public void SendMail(string to, string subject, string message)
    {

#if DEBUG
        to = "robert.leisner@bodoconsult.de";
        //Debug.WriteLine("MailTo: "+to);
        //Debug.WriteLine("Subject: "+subject);
        //Debug.WriteLine("Body: " + message);
        //return;
#endif

        var lo = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = "dbo.[spMAIL_New]"
        };
        lo.Parameters.Add("@To", SqlDbType.VarChar);
        lo.Parameters[0].Value = to;
        lo.Parameters.Add("@Subject", SqlDbType.VarChar);
        lo.Parameters[1].Value = subject;
        lo.Parameters.Add("@Msg", SqlDbType.VarChar);
        lo.Parameters[2].Value = message;
        Exec(lo);
    }

    /// <summary>
    /// Update the filepath for the valuation file for a valuation date
    /// </summary>
    /// <param name="stichtagId">Valuation date ID</param>
    /// <param name="path">Valuation file path</param>
    public void UpdateValuationDateFilePath(string stichtagId, string path)
    {
        var sql = $"EXEC dbo.UpdateStichtagBewertungPfad '{stichtagId}', '{path}'";

        Debug.Print(sql);

        try
        {
            Exec(sql);
        }
        catch (Exception e)
        {
            SetError($"CheckStichtage:Sql:{path}", e);
        }
    }

    /// <summary>
    /// Get valuation dates
    /// </summary>
    /// <returns>List with valuation dates</returns>
    public IList<ValuationDate> GetValuationDates()
    {

        var result = new List<ValuationDate>();

        var dt = _db.GetDataTable("exec dbo.GetStichtageBewertungPfad").AsEnumerable();
        foreach (var r in dt)
        {

            var st = new ValuationDate
            {
                Id = new Guid(r["ST_ID"].ToString() ?? string.Empty),
                Date = Convert.ToDateTime(r["ST_Datum"].ToString()),
                BftAccountId = Convert.ToInt32(r["AN_BftAccountId"].ToString()),
                Owner = r["AN_Name"].ToString(),
                ValuationSchema = r["AN_BewertungSchema"].ToString(),
                ValuationFilePath = r["AN_BewertungPfad"].ToString(),
                IsPathExisting = Convert.ToBoolean(r["IsPath"].ToString())
            };

            result.Add(st);
        }

        return result;
    }


    public void RegisterBewertungForBft(string path, string title, string keyword, int accountId)
    {
        var lo = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = "[dbo].[spBFT_Files_New]"
        };
        lo.Parameters.Add("@Path", SqlDbType.VarChar);
        lo.Parameters[0].Value = path;
        lo.Parameters.Add("@Title", SqlDbType.VarChar);
        lo.Parameters[1].Value = title;
        lo.Parameters.Add("@Keyword", SqlDbType.VarChar);
        lo.Parameters[2].Value = keyword;
        lo.Parameters.Add("@AccountId", SqlDbType.VarChar);
        lo.Parameters[3].Value = accountId;
        Exec(lo);
    }

    public IList<OwnerData> GetOwnersForMailVersand()
    {
        var result = new List<OwnerData>();

        var dt = _db.GetDataTable("exec dbo.GetAnlegerMailVersandLiquiditaet");

        foreach (DataRow row in dt.Rows)
        {
            var o = new OwnerData
            {
                Name = row["AN_Username"].ToString(),
                Fullname = row["AN_Name"].ToString(),
                Email = row["AN_Email"].ToString()?.ToLowerInvariant(),
            };
            result.Add(o);
        }

        return result;
    }


    public IList<AssetValueHtmlCreator> GetBestandswerteForOwner(string owner)
    {
        var result = new List<AssetValueHtmlCreator>();

        var dt = _db.GetDataTable($"exec dbo.GetAnlegerLiquiditaetBestandswerte '{owner}'");

        foreach (DataRow row in dt.Rows)
        {
            var o = new AssetValueHtmlCreator
            {
                AssetName = row["AO_Name"].ToString(),
                AssetId = Convert.ToInt32(row["AO_ID"].ToString()),
                Value = Convert.ToDouble(row["AO_Bestandswert"].ToString()),
            };
            result.Add(o);
        }

        return result;
    }


    public IList<MovementHtmlCreator> GetBewegungenForAnlageobjekt(int anlageobjektId, DateTime date)
    {

        var result = new List<MovementHtmlCreator>();

        var lo = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = "[dbo].[GetAnlegerLiquiditaetBewegungen]"
        };
        lo.Parameters.Add("@AO_ID", SqlDbType.Int);
        lo.Parameters[0].Value = anlageobjektId;
        lo.Parameters.Add("@Datum", SqlDbType.DateTime);
        lo.Parameters[1].Value = date;

        var dt = _db.GetDataTable(lo);

        foreach (DataRow row in dt.Rows)
        {
            var o = new MovementHtmlCreator
            {
                Date = Convert.ToDateTime(row["B_Datum"].ToString()),
                Value = Convert.ToDouble(row["B_Betrag"].ToString()),
                ReceiverSender = row["B_Absender"].ToString(),
                Description = row["B_Bemerkung"].ToString(),
            };
            result.Add(o);
        }

        return result;
    }

    public void AufbereitenKurz()
    {
        Exec("EXEC dbo.AktualisierenKurz");
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        // Do nothing currently
    }
}