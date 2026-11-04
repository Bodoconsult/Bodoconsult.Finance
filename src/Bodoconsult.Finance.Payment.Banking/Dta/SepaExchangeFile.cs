//namespace Bodoconsult.Finance.Payment.Banking.Dta;

////    public class SepaExchangeFile : IPaymentExchangeFile
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
////        public string Standardtext { set { _standardtext = value + " "; } }

////        public string[] TableNames { get; private set; }




////        public string TableName { private get; set; }

////        string _fileName;
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

////                    var schemaTable = _conn.GetOleDbSchemaTable(
////                            OleDbSchemaGuid.Tables,
////                            new object[] { null, null, null, "TABLE" });

////                    var v = new DataView(schemaTable) { Sort = "Table_Name", RowFilter = "Table_Name is not null" };
////                    schemaTable = v.ToTable();

////                    TableNames = new string[schemaTable.Rows.Count];
////                    var i = 0;
////                    foreach (DataRow r in schemaTable.Rows)
////                    {
////                        TableNames[i] = r["Table_name"].ToString();
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


////        DtaStructure _dtas;

////        public SepaExchangeFile()
////        {
////            TableName = null;
////            ReceiverName = null;
////            ReceiverAccount = null;
////            ReceiverBankCode = null;
////            TransferDate = DateTime.MinValue;
////        }

////        public DtaStructure Structure { get { return _dtas; } set { _dtas = value; } }


////        public void ReadSource()
////        {
////            if (_error != 0) return;

////            try
////            {
////                var cmd = new OleDbCommand { CommandText = "select * from [" + TableName + "]", Connection = _conn };

////                var da = new OleDbDataAdapter { SelectCommand = cmd };
////                _dt.Clear();
////                da.Fill(_dt);

////                //var row = _dt.Rows[_dt.Rows.Count-1];

////            }
////            catch
////            {

////                _error = 2;
////                throw;
////            }
////        }

////        /// <summary>
////        /// Lastschriftdatei erzeugen
////        /// </summary>
////        public void CreateFile()
////        {

////            if (_error != 0) return;

////            //try
////            //{
////            var ta = new List<BankingOrder>();



////            foreach (DataRow ud in _dt.Rows)
////            {
////                if (string.IsNullOrEmpty(ud[_dtas.AccountNo].ToString()))
////                {
////                    continue;
////                }

////                try
////                {

////                    var wert = Convert.ToDouble(ud[_dtas.Amount]);

////                    if (wert < 0.01) continue;

////                    var t = new BankingOrder()
////                    {
////                        PayerOrPayee = ud[_dtas.Name].ToString(),
////                        Iban = ud[_dtas.AccountNo].ToString().Replace(" ", string.Empty).Replace(".", string.Empty),
////                        Bic = ud[_dtas.BankCode].ToString().Replace(" ", string.Empty).Replace(".", string.Empty),
////                        Description = _standardtext + ud[_dtas.Remark],
////                        Total = Convert.ToDecimal(wert)
////                    };
////                    ta.Add(t);
////                }
////                catch
////                {

////                    var msg = "Fehler in Buchungssatz:\r\nZahlungspflichtiger: " + ud[_dtas.Name] +
////                              "\r\nKontonummer: " + ud[_dtas.AccountNo].ToString().Replace(" ", string.Empty).Replace(".", string.Empty) +
////                              "\r\nKreditinstitutcode: " + ud[_dtas.BankCode].ToString().Replace(" ", string.Empty).Replace(".", string.Empty) +
////                              "\r\nVerwendungszweck:" + _standardtext + ud[_dtas.Remark] +
////                              "\r\nWert" + Convert.ToDecimal(ud[_dtas.Amount].ToString());
////                    throw (new PaymentException(msg));
////                }
////            }

////            if (TransferDate == DateTime.MinValue) TransferDate = DateTime.Now;



////            var x = new PainFile
////                {
////                    MyBic = ReceiverBankCode,
////                    MyIban = ReceiverAccount,
////                    MyName = ClearSepaString(ReceiverName),
////                    TransferDate = TransferDate,
////                    Transactions = ta,
////                    FileName = _target
////                };
////            x.CreateFile();


////            //var aFromAccount = new BACAccount
////            //{
////            //    CountryCode = string.Empty,
////            //    NameEins = ClearSepaString(ReceiverName),
////            //    IBAN = ReceiverAccount,
////            //    BIC = ReceiverBankCode

////            //};

////            //    //var oHkccs = aFromAccount.FindOptimalBPDSegment("HKCCS", 0);

////            ////var aSepaMessage = new BACSepaMessage
////            ////    {
////            ////        Segment = oHkccs,
////            ////        FromAccount = aFromAccount,
////            ////        ExecutionDate = TransferDate
////            ////    };


////            //var dir = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
////            //if (dir == null) return;

////            //var aSepaMessage = new BACSepaMessage();
////            //aSepaMessage.Load(Path.Combine(dir, @"sepa_pain_001_003_03.txt"));
////            //aSepaMessage.FromAccount = aFromAccount;
////            //aSepaMessage.ExecutionDate = TransferDate;
////            //aSepaMessage.Orders.Clear();


////            //foreach (var t in ta)
////            //{

////            //    //try
////            //    //{
////            //    if (String.IsNullOrEmpty(t.Kontonummer1)) continue;

////            //    var aSepaOrder = aSepaMessage.Orders.Add();

////            //    var empf = t.ZahlungspflichtigerNameEins1;

////            //    if (empf.Length > 27) empf = empf.Substring(0, 27);

////            //    if (IbanValidator.Validate(t.Kontonummer1) != IbanValidationResult.IsValid)
////            //    {
////            //        throw new Exception(String.Format("IBAN {0} für Empfänger {1} ungültig!", t.Kontonummer1, t.ZahlungspflichtigerNameEins1));
////            //    }

////            //    var aToAccount = new BACAccount
////            //    {
////            //        CountryCode = string.Empty,
////            //        NameEins = ClearSepaString(empf),
////            //        IBAN = t.Kontonummer1,
////            //        BIC = t.Kreditinstitutcode1
////            //    };

////            //    aSepaOrder.ToAccount = aToAccount;
////            //    aSepaOrder.Amount = Math.Round(t.Wert1, 2);
////            //    aSepaOrder.CurrencyCode = (String.IsNullOrEmpty(t.Waehrung1)) ? "EUR" : t.Waehrung1;
////            //    aSepaOrder.Purpose = ClearSepaString(t.Verwendungszweck.Replace("??BR????BR??", "\r\n").Replace("??BR??", "\r\n"));


////            //    //}
////            //    //catch
////            //    //{

////            //    //    var msg = "Fehler in Buchungssatz:\r\nZahlungspflichtiger: " + t.ZahlungspflichtigerNameEins1 +
////            //    //        "\r\nKontonummer: " + t.Kontonummer1 +
////            //    //        "\r\nKreditinstitutcode: " + t.Kreditinstitutcode1 +
////            //    //        "\r\nVerwendungszweck:" + t.Verwendungszweck +
////            //    //        "\r\nWert" + t.Wert1;
////            //    //    throw (new BuchungException(msg));

////            //    //}
////            //}

////            //var sErrorReason = string.Empty;


////            //aSepaMessage.Verify(ref sErrorReason);
////            //if (!string.IsNullOrEmpty(sErrorReason))
////            //{
////            //    throw new Exception(sErrorReason);
////            //}

////            //if (_target == null) return;

////            //if (File.Exists(_target)) File.Delete(_target);
////            //aSepaMessage.SaveAs(_target);
////            //}
////            //catch (Exception ex)
////            //{
////            //    _error = 3;
////            //    throw;
////            //}
////        }

////        private static string ClearSepaString(string value)
////        {

////            value =
////                value.Replace("Ö", "oe")
////                     .Replace("ö", "oe")
////                     .Replace("Ä", "Ae")
////                     .Replace("ä", "ae")
////                     .Replace("Ü", "Ue")
////                     .Replace("ü", "ue")
////                     .Replace("ß", "ss")
////                     .Replace(".", string.Empty)
////                     .Replace("-", string.Empty);

////            return value;
////        }

////        //private static string Get27Lines(string sValue)
////        //{

////        //    string erg = null;

////        //    if (sValue.Length <= 27) return sValue;

////        //    var i = Convert.ToInt32(sValue.Length / 27) + 1;
////        //    if (i > 14) i = 14;

////        //    for (var n = 0; n < i; n++)
////        //    {
////        //        var anz = sValue.Length - n * 27;
////        //        if (anz > 27) anz = 27;
////        //        erg += sValue.Substring(n * 27, anz) + "\r\n";
////        //    }

////        //    return erg;

////        //}

////        public void Quit()
////        {
////            _conn.Close();
////        }

////        public void Dispose()
////        {
////            try
////            {
////                _conn.Open();
////            }

////// ReSharper disable EmptyGeneralCatchClause
////            catch
////            // ReSharper restore EmptyGeneralCatchClause
////            {


////            }

////        }
////    }