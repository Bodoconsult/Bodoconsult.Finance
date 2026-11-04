// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Bodoconsult.Finance.Quotations.DataServices;

//https://www.alphavantage.co/query?function=SYMBOL_SEARCH&keywords=hermle&apikey=demo


//0P0000UDDH.FRK

//    https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol=0P0000UDDH.FRK&apikey=WHJSODGETDT2ICOD

//https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol=MBH3.FRK&apikey=WHJSODGETDT2ICOD

//https://www.alphavantage.co/query?function=TIME_SERIES_MONTHLY_ADJUSTED&symbol=0P0000UDDH.FRK&apikey=WHJSODGETDT2ICOD

//https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol=IPRP.LON&apikey=WHJSODGETDT2ICOD

//IPRP.LON


/// <summary>
/// Data service using Alpha Vantage
/// </summary>
public class AlphaVantageDataService : BaseDataService
{
    /// <summary>
    /// Alpha Vantage API token
    /// </summary>
    public string ApiToken { get; set; } 

    /// <summary>
    /// Get current quotation
    /// </summary>
    /// <returns></returns>
    public override void GetCurrentQuotation()
    {
        var search = SecuritiesItems
            .Where(wp => !string.IsNullOrEmpty(wp.Token) && wp.Token.StartsWith("ALPHA:") && wp.Token != "??NoData??");

        //try
        //{

        SetMessage("Kurse von AlphaVantage holen");

        var from = DateTime.Now.AddDays(-15);
        var to = DateTime.Now;

        foreach (var searchItem in search)
        {
            Debug.Print(searchItem.Token.Replace("ALPHA:", string.Empty));

            var url =
                $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={searchItem.Token.Replace("ALPHA:", string.Empty)}&apikey={ApiToken}&datatype=csv";


            var s = GetUrl(url);

            var data = s.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);



            var numbers = data[1].Split([','], StringSplitOptions.RemoveEmptyEntries);

            searchItem.Quotation = Convert.ToDouble(numbers[4], CultureInfo.InvariantCulture);
        }
    }
}