//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using HtmlAgilityPack;

//namespace Bodoconsult.Finance.Quotations
//{


//    public class QuotationInternet : QuotationBase
//    {

//        /// <summary>
//        /// Kurs von Yahoo abholen
//        /// </summary>
//        public void StartYahooRequest()
//        {

//            var search = Securities.Where(wp => !string.IsNullOrEmpty(wp.Yahoo) && wp.Yahoo!="??NoData??").Aggregate(string.Empty, (current, wp) => current + (wp.Yahoo + "+"));

//            try
//            {

//                SetMessage("Kurse von Yahoo holen");

//                var erg = GetUrl(string.Format("http://download.finance.yahoo.com/d/quotes.csv?s={0}&f=snl1", search));
//                var quote = erg.Split('\n');
//                foreach (var s in quote)
//                {

//                    if (string.IsNullOrEmpty(s)) continue;
//                    var kurs = s.Replace("\r", string.Empty).Replace("\string.Empty, string.Empty).Split(',');

//                    var x = Securities.First(wp => wp.Yahoo == kurs[0].Trim());
//                    x.Quotation = Convert.ToDouble(kurs[2].Replace(".", ","));

//                }
//            }
//            catch (Exception ex)
//            {
//                SetError(ex);
//            }
//        }





//        /// <summary>
//        /// Kurse von Msn abholen
//        /// </summary>
//        public void StartMsnRequest()
//        {
//            try
//            {

//                foreach (var wp in Securities.Where(wp => !string.IsNullOrEmpty(wp.Wkn) && wp.Yahoo != "??NoData??" && wp.Quotation <= 0))
//                {
//                    SetMessage(string.Format("Noch fehlende Kurse von MSN holen: {0}", wp.Isin));

//                    // 1. Versuch mit Wkn
//                    var x = GetMsnKurs(wp.Wkn);
//                    if (x > 0)
//                    {
//                        wp.Quotation = x;
//                        continue;

//                    }
//                    // 2. Versuch mit ISIN
//                    x = GetMsnKurs(wp.Isin);
//                    if (x > 0)
//                    {
//                        wp.Quotation = Math.Round(x,5);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                SetError(ex);
//            }
//        }


//        public void StartArivaRequest()
//        {
//            try
//            {
//                var sleep = 0;

//                foreach (var wp in Securities.Where(wp => !string.IsNullOrEmpty(wp.Wkn) && wp.Yahoo!="??NoData??" && wp.Quotation <= 0))
//                {
//                    SetMessage(string.Format("Noch fehlende Kurse von Ariva holen: {0}", wp.Isin));

//                    Thread.Sleep(sleep);

//                    // 1. Versuch mit Wkn
//                    var x = GetArivaKurs(wp.Wkn);
//                    if (x > 0)
//                    {
//                        wp.Quotation = x;
//                        continue;

//                    }
//                    // 2. Versuch mit ISIN
//                    x = GetArivaKurs(wp.Isin);
//                    if (x > 0)
//                    {
//                        wp.Quotation = x;
//                    }

//                    sleep += 1000;
//                }
//            }
//            catch (Exception ex)
//            {
//                SetError(ex);
//            }

//        }

//        /// <summary>
//        /// Kurse über alle verfügbaren Wege abrufen
//        /// </summary>
//        public override void StartRequest()
//        {
//            StartYahooRequest();
//            StartMsnRequest();
//            StartArivaRequest();
//        }



//        private double GetMsnKurs(string wertpapierKennung)
//        {
//            if (string.IsNullOrEmpty(wertpapierKennung)) return 0;

//            var result = GetUrl(string.Format("http://moneycentral.msn.com/investor/external/excel/quotes.asp?SYMBOL={0}", "DE:" + wertpapierKennung));

//            var d = new HtmlDocument();
//            d.LoadHtml(result);
//            foreach (var link in d.DocumentNode.SelectNodes("//td"))
//            {
//                var att = link.Attributes["class"];
//                if (att == null || att.Value != "xl5219310") continue;
//                var k = link.InnerText;
//                if (string.IsNullOrEmpty(k)) continue;
//                try
//                {
//                    return Math.Round(Convert.ToDouble(k.Replace('.', ',')),5);
//                }
//                catch (Exception ex)
//                {
//                    SetError(ex);
//                    return 0;
//                }
//            }

//            return 0;
//        }


//        private double GetArivaKurs(string wertpapierKennung)
//        {
//            try
//            {

//                if (string.IsNullOrEmpty(wertpapierKennung)) return 0;

//                var result = GetUrl(string.Format("http://www.ariva.de/{0}", wertpapierKennung));

//                if (string.IsNullOrEmpty(result)) return 0;

//                const string startTag = @"<table class=string.Emptylinestring.Empty>";
//                const string secondTag = @"<td style=string.Empty";

//                var start = result.IndexOf(startTag, StringComparison.Ordinal);

//                start = result.IndexOf(secondTag, start + 1, StringComparison.Ordinal);

//                start = result.IndexOf(">", start + 1, StringComparison.Ordinal);

//                var ende = result.IndexOf("&nbsp;", start + 1, StringComparison.Ordinal);

//                var s = result.Substring(start + 1, ende - start - 1);


//                return Math.Round(Convert.ToDouble(s),5);
//            }
//            catch (Exception)
//            {
//                return 0;

//            }



//        }


//        public override List<QuotationItem> GetEuribor()
//        {
//            SetMessage("Euribor-Daten von Bundesbank holen");

//            const string url = "http://www.bundesbank.de/cae/servlet/CsvDownload?tsId=BBK01.ST0343&its_csvFormat=de&mode=its";
//            var erg = GetUrl(url);
//            var result = erg.Split('\n');

//            var euribor = new List<QuotationItem>();


//            SetMessage("Euribor-Daten verarbeiten");
//            foreach (var s in result)
//            {
//                var x = s.Split(';');
//                DateTime datum;
//                if (!DateTime.TryParse(x[0], out datum)) continue;
//                double zins;
//                if (!double.TryParse(x[1], out zins)) continue;

//                var kd = new QuotationItem { Date = datum, Quotation = zins };

//                euribor.Add(kd);
//            }

//            return euribor;
//        }


//        public override List<QuotationItem> GetHistoricalData(string kennung, DateTime from, DateTime until)
//        {
//            var history = new List<QuotationItem>();

//            try
//            {
//                var url = string.Format("http://ichart.yahoo.com/table.csv?s={0}&a={1}&b={2}&c={3}&d={5}&e={4}&f={6}&g=d&ignore=.csv", kennung,
//                    //Convert.ToInt16(from.Month-1).ToString("00"),
//                    //from.Day.ToString("00"),
//                    //from.Year.ToString( "0000"),
//                    4,
//                    1,
//                    1984,

//                    Convert.ToInt16(until.Month - 1).ToString("00"),
//                    until.Day.ToString("00"),
//                    until.Year.ToString("0000")
//                    );

//                string[] quote = null;

//                try
//                {
//                    var s = GetUrl(url);
//                    quote = s.Split('\n');
//                }
//                catch (Exception ex)
//                {
//                    SetError(ex);
//                }

//                if (quote == null) return null;



//                foreach (var s in quote)
//                {
//                    var k = s.Split(',');

//                    try
//                    {

//                        var d = k[0];
//                        d = d.Substring(8, 2) + "." + d.Substring(5, 2) + "." + d.Substring(0, 4);

//                        var kd = new QuotationItem
//                                     {
//                                         Date = Convert.ToDateTime(d),
//                                         Quotation = Convert.ToDouble(k[4].Replace(".", ","))
//                                     };
//                        history.Add(kd);
//                    }
//                    catch (Exception ex)
//                    {
//                        SetError(ex);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                SetError(ex);

//            }


//            return history;
//        }


//        public override void GetTickers()
//        {
//            foreach (var wp in Securities.Where(wp => string.IsNullOrEmpty(wp.Yahoo)))
//            {

//                var s = string.Format("Ticker für {0} abholen", wp.Isin);
//                SetMessage(s);

//                wp.Yahoo = HoleWertpapierId(wp.Isin);

//                if (!string.IsNullOrEmpty(wp.Yahoo))
//                {
//                    wp.Changed = true;
//                    continue;
//                }

//                wp.Yahoo = HoleWertpapierId(wp.Wkn);
//                if (!string.IsNullOrEmpty(wp.Yahoo)) wp.Changed = true;
//            }

//            SetMessage("Ticker sind abgeholt!");
//        }


//        public override void GetWkn()
//        {
//            foreach (var wp in Securities.Where(wp => string.IsNullOrEmpty(wp.Wkn)))
//            {
//                wp.Wkn = GetWkn(wp.Isin);
//                if (!string.IsNullOrEmpty(wp.Wkn)) wp.Changed = true;
//            }
//        }

//        public override void GetIsin()
//        {
//            foreach (var wp in Securities.Where(wp => string.IsNullOrEmpty(wp.Isin)))
//            {
//                wp.Isin = GetIsin(wp.Wkn);
//                if (!string.IsNullOrEmpty(wp.Isin)) wp.Changed = true;
//            }

//        }

//        /// <summary>
//        /// WKN über Bing und Finanzen.net abrufen
//        /// </summary>
//        /// <param name="isin"></param>
//        /// <returns></returns>
//        public string GetWkn(string isin)
//        {
//            // http://www.bing.com/search?q=DE0002635265

//            var url = string.Format("http://www.bing.com/search?q={0}", isin);

//            var r = GetUrl(url);

//            var i = r.IndexOf("http://www.finanzen.net", StringComparison.Ordinal);
//            var j = r.IndexOf("\string.Empty, i, StringComparison.Ordinal);


//            var bing = r.Substring(i, j - i);

//            r = GetUrl(bing);
//            i = r.IndexOf("<title>", StringComparison.Ordinal);
//            j = r.IndexOf("</title>", StringComparison.Ordinal);

//            var title = r.Substring(i, j - i);

//            i = title.IndexOf("|", StringComparison.Ordinal);
//            //i = title.IndexOf("|", i+1, StringComparison.Ordinal);
//            j = title.IndexOf("|", i+1,  StringComparison.Ordinal);

//            var wkn = title.Substring(i+1, j - i-1).Trim();

//            return wkn;
//        }

//        /// <summary>
//        /// ISIN über Bing und Finanzen.net abrufen
//        /// </summary>
//        /// <param name="wkn"></param>
//        /// <returns></returns>
//        public string GetIsin(string wkn)
//        {
//            // http://www.bing.com/search?q=DE0002635265

//            var url = string.Format("http://www.bing.com/search?q={0}", wkn);

//            var r = GetUrl(url);

//            var i = r.IndexOf("http://www.finanzen.net", StringComparison.Ordinal);
//            var j = r.IndexOf("\string.Empty, i, StringComparison.Ordinal);


//            var bing = r.Substring(i, j - i);

//            r = GetUrl(bing);
//            i = r.IndexOf("<title>", StringComparison.Ordinal);
//            j = r.IndexOf("</title>", StringComparison.Ordinal);

//            var title = r.Substring(i, j - i);

//            i = title.IndexOf("|", StringComparison.Ordinal);
//            i = title.IndexOf("|", i+1, StringComparison.Ordinal);
//            j = title.IndexOf("|", i + 1, StringComparison.Ordinal);

//            if (j < 0) j = title.Length;

//            var isin = title.Substring(i + 1, j - i - 1).Trim();

//            return isin;
//        }


//        /// <summary>
//        /// Ermittle Ticker für Yahoo-Kursabfrage
//        /// </summary>
//        /// <param name="isin">ISIN oder WKN</param>
//        /// <returns></returns>
//        private string HoleWertpapierId(string isin)
//        {
//            try
//            {
//                //string url = string.Format("http://de.finance.yahoo.com/q?s={0}EUR", ISIN);
//                //StreamReader response = new StreamReader(this.getWebStream(url));
//                //string r = response.ReadToEnd();

//                //if (r.Contains("Ihre Suche") == false)
//                //{
//                //    return ISIN + "EUR";
//                //}

//                var url = string.Format("http://de.finsearch.yahoo.com/de/index.php?s=de_sort&nm={0}&tp=*&r=*&sub=Suchen", isin);

//                var r = GetUrl(url);
//                var i = r.IndexOf("/q?s=", StringComparison.Ordinal);
//                string result = null;
//                while (i != -1)
//                {
//                    var z = r.IndexOf(@string.Emptystring.Empty, i, StringComparison.Ordinal);
//                    result = result + r.Substring(i, z - i).Replace("/q?s=", string.Empty) + ",";
//                    i = r.IndexOf("/q?s=", z, StringComparison.Ordinal);
//                }


//                var s = result.Split(',');

//                // XETRA
//                foreach (var x in s.Where(x => x.EndsWith(".DE")))
//                {
//                    return x;
//                }

//                // Frankfurt
//                foreach (var x in s.Where(x => x.EndsWith(".F")))
//                {
//                    return x;
//                }

//                // Stuttgart
//                foreach (var x in s.Where(x => x.EndsWith(".SG")))
//                {
//                    return x;
//                }

//                // Hamburg
//                foreach (var x in s.Where(x => x.EndsWith(".HM")))
//                {
//                    return x;
//                }

//                // München
//                foreach (var x in s.Where(x => x.EndsWith(".MU")))
//                {
//                    return x;
//                }

//                // Berlin
//                foreach (var x in s.Where(x => x.EndsWith(".BE")))
//                {
//                    return x;
//                }

//                // Düsseldorf
//                foreach (var x in s.Where(x => x.EndsWith(".DU")))
//                {
//                    return x;
//                }

//                // Paris
//                foreach (var x in s.Where(x => x.EndsWith(".PA")))
//                {
//                    return x;
//                }

//                // Mailand
//                foreach (var x in s.Where(x => x.EndsWith(".MI")))
//                {
//                    return x;
//                }

//                return result;

//            }
//            catch (Exception ex)
//            {
//                SetError(ex);
//                return null;
//            }
//        }


//        public void Clear()
//        {
//            Securities.Clear();
//        }
//    }
//}
