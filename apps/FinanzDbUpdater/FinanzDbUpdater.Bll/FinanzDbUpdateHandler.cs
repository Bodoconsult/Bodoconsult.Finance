// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Delegates;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.Finance.Portfolio;
using Bodoconsult.Finance.Quotations.DataServices;
using Bodoconsult.Finance.Quotations.Interfaces;
using FinanzDbUpdater.Bll.Interfaces;
using FinanzDbUpdater.Bll.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanzDbUpdater.Bll;

/// <summary>
/// Rahmenklasse für Datenaufbereitung
/// </summary>
public sealed class FinanzDbUpdateHandler : IDisposable, IFinanzDbUpdateHandler
{
    /// <summary>
    /// Logo used for the simulation report header
    /// </summary>
    public string LogoSimulationReport { get;  }

    /// <summary>
    /// JSON file with quotations from external sources
    /// </summary>
    public string QuotationJsonFile { get;  }

    /// <summary>
    /// JSON file containing Depot data with quoatations
    /// </summary>
    public string DepotJsonFile { get;  }

    // Logging mit Log4net
    // private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);



    public StatusMessageDelegate StatusMessageDelegate { get; set; }

    public HandleExceptionDelegate HandleExceptionDelegate { get; set; }

    private readonly IDataHandler _db;
    private IFinanzDbUpdaterAppGlobals _globals;
    private readonly IQuotationHandler _quotationHandler;

    //public string Connection
    //{
    //    set =>
    //        _db = new DataHandler(value)
    //        {
    //            StatusHandler = SetStatus,
    //            ErrorHandler = ErrorHandler,
    //        };
    //}


    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appGlobals"></param>
    /// <param name="quotationHandler">Current quotation handler</param>
    /// <exception cref="ArgumentException"></exception>
    public FinanzDbUpdateHandler(IAppGlobals appGlobals, IQuotationHandler quotationHandler, IDataHandler db)
    {
        if (appGlobals is not IFinanzDbUpdaterAppGlobals globals)
        {
            throw new ArgumentException("AppGlobals is not IFinanzDbUpdaterAppGlobals");
        }

        _globals = globals;

        _quotationHandler = quotationHandler;
        _quotationHandler.StatusMessageDelegate += SetStatus;

        _db = db;

        LogoSimulationReport = _globals.LogoSimulationReport;
        QuotationJsonFile = _globals.QuotationJsonFile;
        DepotJsonFile = _globals.DepotJsonFile;
    }

    /// <summary>
    /// Date from
    /// </summary>
    public DateTime From { get; set; } = DateTime.Now.AddMonths(-1);

    /// <summary>
    /// Date until
    /// </summary>
    public DateTime Until { get; set; }= DateTime.Now.AddDays(-1);

    /// <summary>
    /// Prepare the database
    /// </summary>
    public void PrepareTheDatabase()
    {
        _db.InitTheDatabasePreparation();
    }

    /// <summary>
    /// Update the quotation data
    /// </summary>
    public void UpdateQuotationData()
    {
        try
        {
            // Wertpapiere
            SetStatus("Wertpapier-Kurse aus dem Internet holen...");
            GetQuotationsForSecurities();
        }
        catch (Exception ex)
        {
            SetError("UpdateQuotationData:GetQuotationsForSecurities", ex);
            _db.SendMail("UpdateQuotationData:GetQuotationsForSecurities", ex.Message);
        }

#if DEBUG
        foreach (var item in _quotationHandler.SecuritiesItems.OrderBy(x => x.Wkn))
        {

            Debug.Print($"{item.Wkn}: {item.Quotation} ");
        }
#endif

        try
        {
            SetStatus("EURIBOR-Daten aus dem Internet von Bundesbank holen...");
            GetQuotationsForEuribor();
        }
        catch (Exception ex)
        {
            _db.SendMail("UpdateQuotationData:QuotationHistory", ex.Message);
            SetError("UpdateQuotationData:GetQuotationsForEuribor", ex);
        }

        try
        {
            SetStatus("Kursdaten für Indizes aus dem Internet  holen...");
            QuotationHistory();
        }
        catch (Exception ex)
        {
            _db.SendMail("UpdateQuotationData:QuotationHistory", ex.Message);
            SetError("UpdateQuotationData:QuotationHistory", ex);
        }
    }

    private void GetQuotationsForSecurities()
    {
        _quotationHandler.SecuritiesItems.Clear();

        var data = _db.GetSecuritiesForQuotationFeed();

        foreach (var wp in data)
        {
            _quotationHandler.SecuritiesItems.Add(wp);
        }

        if (!string.IsNullOrEmpty(DepotJsonFile))
        {
            _quotationHandler.StartGetCurrentQuotations(DataProviderType.Safekeepings, DepotJsonFile);
        }
        _quotationHandler.StartGetCurrentQuotations(DataProviderType.Json, QuotationJsonFile);
        //quotationHandler.StartGetCurrentQuotations(DataProviderType.Msn);
        _quotationHandler.StartGetCurrentQuotations(DataProviderType.Ariva);

        //quotationHandler.GetWkn();
        //quotationHandler.GetIsin();
        //quotationHandler.GetTickers();

        //quotationHandler.StartRequest();
        //quotationHandler.ReadFromXml();

        //Parallel.ForEach(quotationHandler.Securities.Where(wp => wp.Changed), wp => UpdateWertpapierStammdaten(wp.Wkn, wp.Isin, wp.Yahoo));


        Parallel.ForEach(_quotationHandler.SecuritiesItems.Where(x => x.Quotation > 0), wp =>
        {
            //if (!(wp.Kurs > 0)) continue;
            var s = $"{wp.Wkn}: {wp.Isin}:{wp.Quotation}";
            SetStatus(s);

            _db.UpdateQuotation(wp.Isin, wp.Quotation);
        });
    }

    private void GetQuotationsForEuribor()
    {
        _quotationHandler.SecuritiesItems.Clear();

        var data = _db.GetSecuritiesForQuotationFeedEuribor();

        foreach (var wp in data)
        {
            _quotationHandler.SecuritiesItems.Add(wp);
        }

        _quotationHandler.StartGetHistory(DataProviderType.Bundesbank, From, Until);

        if (!_quotationHandler.SecuritiesItems.Any())
        {
            return;
        }

        //Euribor-Daten
        Parallel.ForEach(_quotationHandler.SecuritiesItems[0].History, kd =>
        {
            _db.UpdateEuribor(kd.Date, kd.Quotation);
        });

        _db.UpdateEuriborData();
    }

    /// <summary>
    /// Kurshistorie holen
    /// </summary>
    public void QuotationHistory(string sql = "EXEC dbo.GetWPForKursabfrage 1")
    {
        _quotationHandler.ClearSecurities();

        var tasks = new List<Task>();

        try
        {
            var wpData = _db.GetSecuritiesForQuotationFeedFlexible(sql);

            foreach (var wp in wpData)
            {
                _quotationHandler.SecuritiesItems.Add(wp);
            }

            _quotationHandler.StartGetHistory(DataProviderType.Quandl, From, Until);
        }
        catch (Exception ex)
        {
            SetError("Quotation feed", ex);
        }

        if (_quotationHandler == null)
        {
            return;
        }

        foreach (var item in _quotationHandler.SecuritiesItems.Where(x => x.History != null))
        {

            var isin = item.Isin;
            var erg = item.History;
            var data = erg.ToList();

            var task = new TaskFactory().StartNew(() =>
            {
                try
                {
                    SetStatus($"Schreibe Daten für {isin}");

                    foreach (var dataItem in data)
                    {
                        _db.UpdateQuotationHistory(isin, dataItem.Date, dataItem.Quotation);
                    }

                    _db.UpdateQuotationHistoryPrepareImport(isin, From);

                    SetStatus($"Daten für {isin} fertig geschrieben");
                }
                catch (Exception e)
                {
                    SetError($"Kursabfrage:{isin}", e);
                }

            });

            tasks.Add(task);
        }

        Task.WaitAll(tasks.ToArray());
    }

    public void PrepareDataFull()
    {
        SetStatus("Wertpapierkurse aufbereiten");
        _db.UpdateSecuritiesQuotations();

        SetStatus("Benchmarks für die User aufbereiten");
        _db.UpdateBenchmarks();

        SetStatus("Vermögensdaten aufbereiten");
        _db.UpdateWealth();

        SetStatus("Externe Daten aufbereiten");
        _db.StartExternalJobs();

        SetStatus("Liquiditätsdaten aufbereiten");
        _db.UpdateLiquidity();
    }

    /// <summary>
    /// Berechne Risken der einzelnen Depots
    /// </summary>
    public void CalculateDepotRisk()
    {
        SetStatus("Risiko für Depots berechnen");

        var dt = _db.GetOwnersDataForRiskMetrics();

        Parallel.ForEach(dt, r =>
        {
            SetStatus($"Depotrisiko für {r.Name} ermitteln..");
            CalculateDepotRisk(r.Name);
        });

        //foreach (var r in dt)
        //{
        //    SetStatus("Depotrisiko für " + r["Owner"] + " ermitteln..");
        //    ErmittleDepotRisiko(r["Owner"].ToString());
        //}
    }

    /// <summary>
    /// Risikoberechnung für ein Depot
    /// </summary>
    /// <param name="owner"></param>
    public void CalculateDepotRisk(string owner)
    {
        //try
        //{

        //Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
        //Thread.CurrentThread.Priority = ThreadPriority.Highest;

        SetStatus($"Risiko für Depot von {owner} berechnen...");

        var dt = _db.GetBasicRiskDataForOwner(owner);
        if (dt.Rows.Count == 0)
        {
            return;
        }

        var rc = new RiskCalculator();
        rc.LoadVectorA(dt);

        rc.LoadCovarianceMatrix(_db.GetCovarianceMatrixForOwner(owner));

        rc.Calculate();

        _db.UpdateOwnerRisk(owner, rc.Risk);

        //}
        //catch (Exception ex)
        //{
        //    SetError("ErmittleDepotRisiko", ex);
        //}
    }
        
    /// <summary>
    /// Statusanzeige
    /// </summary>
    /// <param name="msg"></param>
    private void SetStatus(string msg)
    {
        StatusMessageDelegate?.Invoke(msg);
    }

    public void SetError(string msg, Exception ex)
    {
        HandleExceptionDelegate?.Invoke(new Exception(msg, ex));
    }

    /// <summary>
    /// Check if Bewertungen files for the Stichtag exists
    /// </summary>
    public void CheckStichtage()
    {
        var stichtage = _db.GetValuationDates();

        //var dt = _db.GetDataTable("exec dbo.GetStichtageBewertungPfad").AsEnumerable();
        foreach (var st in stichtage)
        {
            var date = st.Date;
            var accountId = st.BftAccountId;
            var name = st.Owner;

            var path = string.Format(st.ValuationSchema, date.ToString("yyyyMMdd"));
            path = System.IO.Path.Combine(st.ValuationFilePath, path);

            //Debug.Print(path);
            if (!System.IO.File.Exists(path)) continue;

            //Debug.Print(path);

            if (!st.IsPathExisting)
            {
                _db.UpdateValuationDateFilePath(st.Id.ToString(), path);
            }

            if (accountId <= 0)
            {
                continue;
            }

            try
            {
                var title = $"Anteilsbewertung {name} Stichtag: {date:dd.MM.yyyy}";

                const string keyword = "Anteilsbewertung";

                _db.RegisterBewertungForBft(path, title, keyword, st.BftAccountId);
            }
            catch (Exception e)
            {
                SetError($"CheckStichtage:Btf:{path}", e);
            }
        }
    }

    /// <summary>
    /// Send account statement info for all registered owner
    /// </summary>
    public void SendMail()
    {
        var owners = _db.GetOwnersForMailVersand();

        var date = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

        // Create content
        foreach (var owner in owners)
        {
            var s = GetMailBody(owner, date);

            Debug.Print("\r\n");
            Debug.Print("\r\n");
            Debug.Print(s);

            owner.Data = s;
        }

        // Get all mail receivers
        var mailto = new List<OwnerData>();

        foreach (var owner in owners)
        {
            var addresses = owner.Email.Split([';'], StringSplitOptions.RemoveEmptyEntries);

            foreach (var address in addresses)
            {
                var od = new OwnerData
                {
                    Email = address,
                    Name = owner.Name,
                    Fullname = owner.Fullname,
                    Data = owner.Data
                };

                mailto.Add(od);
            }
        }
            
        var receivers = mailto.GroupBy(x => x.Email);

        foreach (var receiver in receivers)
        {
            var toc = new StringBuilder();
            toc.AppendLine("<h1>Inhaltsverzeichnis</h1>");
            //toc.AppendLine("<p>&nbsp;</p>");
            var body = new StringBuilder();

            var i = 0;

            var parts = mailto.Where(x => x.Email == receiver.Key).ToList();

            var createToc = parts.Count > 1;

            foreach (var part in parts)
            {
                toc.AppendLine($"<p class=\"toc\"><a href=\"#ID{i}\">{part.Fullname}</a></p>");
                body.Append($"<div><a name=\"ID{i}\" />\r\n{part.Data}\r\n</div>");
                i++;
            }

            toc.AppendLine("<p>&nbsp;</p>");

            if (createToc)
            {
                toc.Append(body);
                _db.SendMail(receiver.Key, "Liquiditätsstatus", toc.ToString());
            }
            else
            {
                _db.SendMail(receiver.Key, "Liquiditätsstatus", body.ToString());
            }
        }
    }

    private string GetMailBody(OwnerData owner, string date)
    {

        var result = new StringBuilder();

        var werte = _db.GetBestandswerteForOwner(owner.Name);

        result.AppendLine($"<h1>Liquiditätsstatus {owner.Fullname}</h1>");

        // bestandswerte
        result.AppendLine($"<h2>Kontenstand {date}</h2>");
        result.AppendLine("<table width=\"100%\">");

        foreach (var wert in werte)
        {

            result.Append(wert.ToHtmlString());

        }

        result.AppendLine("</table>");
        result.AppendLine(string.Empty);

        var fetchDate = DateTime.Now.AddDays(-4).Date;

        // Kontobewegungen
        foreach (var wert in werte)
        {

            result.AppendLine($"<h2>Kontobewegungen {wert.AssetName}</h2>");


            var rows = _db.GetBewegungenForAnlageobjekt(wert.AssetId, fetchDate).OrderByDescending(x => x.Date).ThenBy(x => x.Value).ToList();

            if (rows.Any())
            {
                result.AppendLine("<table width=\"100%\">");
                foreach (var row in rows)
                {
                    result.Append(row.ToHtmlString());
                }
                result.AppendLine("</table>");
            }
            else
            {
                result.AppendLine($"<p>Keine Bewegungen seit {fetchDate:dd.MM.yyyy}.</p>");
            }

            result.AppendLine(string.Empty);
        }

        result.AppendLine("<p>&nbsp;</p>");

        return result.ToString();
    }


    public void Dispose()
    {
        _db.Dispose();
    }

    public void PrepareDataShort()
    {
        _db.AufbereitenKurz();
    }
}