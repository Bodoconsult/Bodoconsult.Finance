//namespace Bodoconsult.Finance.Payment.Banking.Dta;

////    public class DtaExchangeFile : IPaymentExchangeFile
////    {

////        public DtaMode TransactionMode { get; set; }

////        OleDbConnection _conn;
////        readonly DataTable _dt = new DataTable();
////        int _error;

////        public string ReceiverBankCode { private get; set; }

////        public string ReceiverAccount { private get; set; }

////        public string ReceiverName { private get; set; }

////        public DateTime TransferDate { private get; set; }


////        string _standardtext = string.Empty;
////        public string Standardtext { set { _standardtext = value+" "; } }

////        public string[] TableNames { get; private set; }


////        public string TableName { private get; set; }

////        string _fileName;


////        public void Dispose()
////        {
////            try
////            {
////                _conn.Open();
////            }

////// ReSharper disable EmptyGeneralCatchClause
////            catch
////// ReSharper restore EmptyGeneralCatchClause
////            {
                
                
////            }

////        }

////        /// <summary>
////        /// Vollständiger Pfad zur Excel-Datei
////        /// </summary>
////        public string FileName
////        {
////            get { return _fileName; }


////            set
////            {
////                _fileName = value;
////                _conn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + value + @";Extended Properties=string.EmptyExcel 12.0;HDR=YESstring.Empty;");
////                try
////                {
////                    _conn.Open();

////                    var schemaTable  = _conn.GetOleDbSchemaTable(
////                            OleDbSchemaGuid.Tables,
////                            new object[] { null, null, null, "TABLE" });

////                    var v = new DataView(schemaTable) {Sort = "Table_Name", RowFilter = "Table_Name is not null"};
////                    schemaTable = v.ToTable();

////                   TableNames = new string[schemaTable.Rows.Count];
////                    var i = 0;
////                    foreach (DataRow r in schemaTable.Rows)
////                    {
////                        TableNames[i]=r["Table_name"].ToString();
////                        i++;
////                    }

////                }
////                catch
////                {
////                    _error = 1;
////                    throw;
////                }
////            }
////        }

////        string _target;
////        public string TargetDtaFile { set { _target = value; } }


////        public DtaExchangeFile()
////        {
////            TableName = null;
////            ReceiverName = null;
////            ReceiverAccount = null;
////            ReceiverBankCode = null;
////            TransferDate = DateTime.MinValue;
////        }

////        public DtaStructure Structure { get; set; }


////        public void ReadSource()
////        {
////            if (_error != 0) return;

////            //try
////            //{
////                var cmd = new OleDbCommand {CommandText = "select * from [" + TableName + "]", Connection = _conn};

////                var da = new OleDbDataAdapter {SelectCommand = cmd};
////                _dt.Clear();
////                da.Fill(_dt);

////                //var row = _dt.Rows[_dt.Rows.Count-1];

////            //}
////            //catch 
////            //{

////            //    _error = 2;
////            //    throw;
////            //}
////        }

////        /// <summary>
////        /// Lastschriftdatei erzeugen
////        /// </summary>
////        public void CreateFile()
////        {

////            if (_error != 0) return;

////            try
////            {
////                var ta = new List<BankingOrder>();
////                var i = 0;


////                foreach (var t in from DataRow ud in _dt.Rows 
////                                  where !string.IsNullOrEmpty(ud[Structure.AccountNo].ToString()) 
////                                  let wert = Convert.ToDouble(ud[Structure.Amount]) 
////                                  where !(wert <0.01) select new BankingOrder
////                                        {
////                                            PayerOrPayee = ud[Structure.Name].ToString(),
////                                            Iban =
////                                                ud[Structure.AccountNo].ToString().Replace(" ", string.Empty).Replace(".", string.Empty),
////                                            Bic =
////                                                ud[Structure.BankCode].ToString().Replace(" ", string.Empty).Replace(".", string.Empty),
////                                            Description = _standardtext + ud[Structure.Remark],
////                                           Total = Convert.ToDecimal(wert)
////                                        })
////                {
////                    ta[i] = t;
////                    i++;
////                }

////                if (TransferDate == DateTime.MinValue) TransferDate = DateTime.Now;

////                var aDtaus = new BACDTAUSFile
////                                 {
////                                     MyName = ReceiverName,
////                                     Type = (TransactionMode== DtaMode.Lastschrift) ? "LK": "GK",
////                                     MyBankCode = ReceiverBankCode,
////                                     MyAccountNumber = ReceiverAccount,
////                                     //EuroCurrency = true,
////                                     ExecutionDate = TransferDate
////                                 };



////                foreach (var t in ta)
////                {

////                    try
////                    {
////                        if (string.IsNullOrEmpty(t.Iban)) continue;
////                        var aDtRec = aDtaus.AddRecord();
////                        aDtRec.Amount = t.Total;
////                        //aDtRec.EuroCurrency = true;
////                        aDtRec.HisAccountNumber = t.Iban;
////                        aDtRec.HisBankCode = t.Bic;

////                        var empf = t.PayerOrPayee;

////                        if (empf.Length > 27) empf = empf.Substring(0, 27);
////                        aDtRec.HisName = empf;
////                        aDtRec.Purpose = Get27Lines(t.Description.Replace("??BR????BR??", "\r\n").Replace("??BR??", "\r\n"));
////                        aDtRec.TextKey = (TransactionMode == DtaMode.Lastschrift) ? "5" : "51";
////                    }
////                    catch
////                    {

////                        var msg = "Fehler in Buchungssatz:\r\nZahlungspflichtiger: " + t.PayerOrPayee +
////                            "\r\nKontonummer: " + t.Iban +
////                            "\r\nKreditinstitutcode: " + t.Bic +
////                            "\r\nVerwendungszweck:" + t.Description +
////                            "\r\nWert" + t.Total;
////                        throw (new PaymentException(msg));                  

////                    }
////                }

////                //aDtaus.RecordSort;

////                aDtaus.Validate();

////                if (_target == null) return;

////                if (File.Exists(_target)) File.Delete(_target);
////                aDtaus.SaveAs(_target);
////            }
////            catch (Exception ex)
////            {
////                _error = 3;
////                throw new Exception("Dta.CreateDta", ex);
////            }
////        }


////        private static string Get27Lines(string sValue)
////        {
            
////            string erg = null;

////            if (sValue.Length <= 27) return sValue;

////            var i = Convert.ToInt32(sValue.Length / 27) + 1;
////            if (i > 14) i = 14;

////            for (var n = 0; n < i; n++)
////            {
////                var anz = sValue.Length - n * 27;
////                if (anz > 27) anz = 27;
////                erg += sValue.Substring(n * 27, anz) + "\r\n";
////            }

////            return erg;

////        }

////        public void Quit()
////        {
////            _conn.Close();
////        }
////    }