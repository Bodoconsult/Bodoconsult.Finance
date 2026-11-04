// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System;
using System.Linq;
using HtmlAgilityPack;

namespace Bodoconsult.Finance.Quotations.DataServices;

/// <summary>
/// Get data from Msn via WKN
/// </summary>
public class MsnDataService : BaseDataService
{
    /// <summary>
    /// Get current quotation
    /// </summary>
    public override void GetCurrentQuotation()
    {

        // http://moneycentral.msn.com/investor/external/excel/quotes.asp?SYMBOL=DE:580265

        try
        {

            foreach (var item in SecuritiesItems.Where(x => !string.IsNullOrEmpty(x.Wkn) && x.Quotation<0.000001))
            {
                var result = GetUrl(
                    $"http://moneycentral.msn.com/investor/external/excel/quotes.asp?SYMBOL={"DE:" + item.Wkn}");

                var d = new HtmlDocument();
                d.LoadHtml(result);
                foreach (var link in d.DocumentNode.SelectNodes("//td"))
                {
                    var att = link.Attributes["class"];
                    if (att == null || att.Value != "xl5219310") continue;
                    var k = link.InnerText;
                    if (string.IsNullOrEmpty(k)) continue;
                    try
                    {
                        item.Quotation = Math.Round(Convert.ToDouble(k.Replace('.', ',')), 5);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }
        catch (Exception ex)
        {
            SetError(ex);
        }
    }

}