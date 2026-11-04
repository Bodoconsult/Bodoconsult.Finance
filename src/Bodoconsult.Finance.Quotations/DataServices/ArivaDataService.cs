// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System;
using System.Globalization;
using System.Linq;

namespace Bodoconsult.Finance.Quotations.DataServices;

/// <summary>
/// Get data from Ariva via WKN
/// </summary>
public class ArivaDataService : BaseDataService
{
    /// <summary>
    /// Get current quotation
    /// </summary>
    /// <returns></returns>
    public override void GetCurrentQuotation()
    {

        // http://www.ariva.de/710000

        try
        {

            foreach (var item in SecuritiesItems.Where(x => !string.IsNullOrEmpty(x.Wkn) && x.Wkn!="999999" && x.Quotation < 0.000001))
            {

                //var css = GetUrl("http://www.freeprivacypolicy.com/public/cookie-consent/3.0.0/cookie-consent.js");

                //var fileName = "D:\\tmp\\cookie.js";

                //if (File.Exists(fileName)) File.Delete(fileName);
                //File.WriteAllText(fileName,css);

                var result = GetUrl($"http://www.ariva.de/{item.Wkn}");

                if (string.IsNullOrEmpty(result)) continue;

                const string startTag = "<span itemprop=\"price\" content=\"";

                const string secondTag = "</span>";



                var start = result.IndexOf(startTag, StringComparison.Ordinal) + startTag.Length;

                start = result.IndexOf(">", start, StringComparison.Ordinal)+1;

                var ende = result.IndexOf(secondTag, start + 1, StringComparison.Ordinal);
                var count = ende - start;

                var s = result.Substring(start , count).Replace("\t",string.Empty).Replace("\n",string.Empty).Replace(",",".");

                try
                {
                    item.Quotation = Math.Round(double.Parse(s, new CultureInfo("en-us")), 5);
                }
                catch
                {
                    // ignored
                }
            }
        }
        catch (Exception ex)
        {
            SetError(ex);
        }
    }
}