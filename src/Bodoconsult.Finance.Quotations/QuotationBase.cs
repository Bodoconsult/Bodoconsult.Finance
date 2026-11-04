//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Xml;

//namespace Bodoconsult.Finance.Quotations
//{

//    public delegate void ErrorHandler(Exception ex);
//    public delegate void StatusHandler(string msg);

//    public class QuotationBase
//    {



//        public string KurseXml { get; set; }

//        public List<SecuritiesItem> Securities { get; set; }

//        public QuotationBase()
//        {
//            Securities = new List<SecuritiesItem>();
//        }


//        /// <summary>
//        /// Wertpapier hinzufügen
//        /// </summary>
//        /// <param name="item">a concrete securities item</param>
//        public void Add(SecuritiesItem item)
//        {
//            SetMessage(string.Format("Wertpapier {0}/{1} hinzugefügt...", item.Isin, item.Wkn));
//            Securities.Add(item);

//        }

//        /// <summary>
//        /// Hole Wertpapierkurse von Quelle
//        /// </summary>
//        public virtual void StartRequest()
//        {


//        }


//        public virtual void GetTickers()
//        {


//        }

//        public virtual void GetWkn()
//        {


//        }

//        public virtual void GetIsin()
//        {


//        }


//        public virtual List<QuotationItem> GetEuribor()
//        {

//            return null;
//        }


//        public virtual List<QuotationItem> GetHistoricalData(string kennung, DateTime from, DateTime until)
//        {

//            return null;
//        }






//        public void ReadFromXml()
//        {

//            if (string.IsNullOrEmpty(KurseXml)) return;

//            if (File.Exists(KurseXml) == false) return;

//            var d = new XmlDocument();
//            d.Load(KurseXml);

//            var last = d.GetElementsByTagName("wertpapier");
//            for (var i = 0; i < last.Count; i++)
//            {
//                try
//                {
//                    var isin = last[i].Attributes["isin"].InnerText;
//                    var kurs = Convert.ToDouble(last[i].InnerText.Replace(".", ","));

//                    var x = Securities.First(wp => wp.Isin == isin);
//                    x.Quotation = kurs;
//                }
//                catch (Exception ex)
//                {

//                    SetError(ex);
//                }

//            }

//        }

//        /// <summary>
//        /// ´Get historical quotation data from a CSV file
//        /// </summary>
//        /// <param name="kennung"></param>
//        /// <param name="csvFileName">filename of the CSV file</param>
//        /// <param name="dateColumn">column number of the date column (based on 1)</param>
//        /// <param name="quotationColumn">column number of the quotation column (based on 1)</param>
//        /// <returns></returns>
//        public virtual List<QuotationItem> GetHistoricalData(string kennung, string csvFileName, int dateColumn=1, int quotationColumn=2)
//        {
//            var erg = new List<QuotationItem>();

//            var s = File.ReadAllText(csvFileName, Encoding.UTF8);

//            var zeilen = s.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

//            // ReSharper disable once LoopCanBeConvertedToQuery
//            foreach (var zeile in zeilen)
//            {

//                var fields = zeile.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);

//                var dateString = fields[dateColumn-1];
//                var valueString = fields[quotationColumn-1];

//                var k = new QuotationItem
//                {
//                    Date = Convert.ToDateTime(dateString),
//                    Quotation = double.Parse(valueString)
//                };

//                erg.Add(k);
//            }

//            return erg;
//        }
//    }
//}
