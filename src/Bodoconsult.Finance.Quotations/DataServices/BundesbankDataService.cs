// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System;
using System.Collections.Generic;
using Bodoconsult.Finance.Quotations.Model;

namespace Bodoconsult.Finance.Quotations.DataServices;

/// <summary>
/// Get data form Deutsche Bundesbank
/// 
/// Known Tokens:
///     BBK:EURIBOR    
/// 
/// </summary>
public class BundesbankDataService : BaseDataService
{
    /// <summary>
    /// Get the quotation history for a time period
    /// </summary>
    /// <param name="from">time period from</param>
    /// <param name="to">time period to</param>
    /// <returns></returns>
    public override void GetHistory(DateTime from, DateTime to)
    {
        foreach (var item in SecuritiesItems)
        {
            switch (item.Token.ToLower())
            {
                case "bbk:euribor":
                    GetEuriborHistory(item.History , from, to);
                    break;
            }
        }
    }

    private void GetEuriborHistory(List<QuotationItem> history, DateTime @from, DateTime to)
    {
        SetMessage("Fetch Euribor data from Bundesbank...");

        const string url = "https://api.statistiken.bundesbank.de/rest/download/BBIG1/M.D0.EUR.MMKT.EURIBOR.M12.AVE.MA?format=csv&lang=de";
        var erg = GetUrl(url);
        var result = erg.Split('\n');

        SetMessage("Process Euribor data...");
        foreach (var s in result)
        {
            var x = s.Split(';');

            if (!DateTime.TryParse(x[0], out var datum))
            {
                continue;
            }

            if (!double.TryParse(x[1], out var zins))
            {
                continue;
            }

            if (datum < @from || datum > to)
            {
                continue;
            }

            var kd = new QuotationItem { Date = datum, Quotation = zins };
            history.Add(kd);
        }
    }
}