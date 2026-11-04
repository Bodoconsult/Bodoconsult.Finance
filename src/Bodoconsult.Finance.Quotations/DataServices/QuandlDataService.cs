// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System;
using System.Linq;
using Bodoconsult.Finance.Quotations.Model;

namespace Bodoconsult.Finance.Quotations.DataServices;

/// <summary>
/// Get data from Quandl
/// </summary>
public class QuandlDataService : BaseDataService
{
    /// <summary>
    /// Get current quotation
    /// </summary>
    public override void GetCurrentQuotation()
    {

        var date = DateTime.Today.AddDays(-1);

        GetHistory(date.AddDays(-7), date);

        foreach (var item in SecuritiesItems.Where(x => x.Token.StartsWith("QUANDL:")))
        {

            if (item.History==null) continue;

            var q = item.History.FirstOrDefault(x => x.Date == date) ?? item.History.FirstOrDefault();

            if (q != null)
            {
                item.Quotation = q.Quotation;
            }

        }

        //foreach (var item in SecuritiesItems)
        //{

        //    //try
        //    //{

        //        SetMessage("Kurse von Quandl holen");

        //        var erg =
        //            GetUrl(string.Format("http://www.quandl.com/api/v3/datasets/{0}/data.csv?start_date={1}", item.Token.Replace("QUANDL:", string.Empty), ));
        //        var quote = erg.Split('\n');
        //        foreach (var s in quote)
        //        {

        //            if (string.IsNullOrEmpty(s)) continue;
        //            var kurs = s.Replace("\r", string.Empty).Replace("\string.Empty, string.Empty).Split(',');


        //            try
        //            {
        //                item.Quotation = Convert.ToDouble(kurs[1].Replace(".", ","));
        //            }
        //            catch
        //            {
        //                // ignored
        //            }
        //        }
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    SetError(ex);
        //    //}
        //}
    }

    /// <summary>
    /// Get the quotation history for a time period
    /// </summary>
    /// <param name="from">time period from</param>
    /// <param name="to">time period to</param>
    public override void GetHistory(DateTime from, DateTime to)
    {

        foreach (var item in SecuritiesItems.Where(x => x.Token.StartsWith("QUANDL:")))
        {

            var history = item.History;

            try
            {
                var url =
                    $"http://www.quandl.com/api/v3/datasets/{item.Token.Replace("QUANDL:", string.Empty)}/data.csv?start_date={@from:yyyy-MM-dd}";

                string[] quote  = [];

                try
                {
                    var s = GetUrl(url);

                    if (s == null)
                    {
                        continue;
                    }

                    quote = s.Split(["\n"], StringSplitOptions.RemoveEmptyEntries);
                }
                catch (Exception ex)
                {
                    SetError(ex);
                }

                if (quote.Length == 0)
                {
                    continue;
                }

                foreach (var s in quote)
                {
                    var k = s.Split(',');

                    try
                    {

                        var d = k[0];
                        d = $"{d.Substring(8, 2)}.{d.Substring(5, 2)}.{d.Substring(0, 4)}";

                        var kd = new QuotationItem
                        {
                            Date = Convert.ToDateTime(d),
                            Quotation = Convert.ToDouble(k[1].Replace(".", ","))
                        };

                        if (kd.Date>=from && kd.Date<=to)
                        {
                            history.Add(kd);
                        }
                    }
                    catch (Exception ex)
                    {
                        SetError(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                SetError(ex);
            }
        }
    }
}