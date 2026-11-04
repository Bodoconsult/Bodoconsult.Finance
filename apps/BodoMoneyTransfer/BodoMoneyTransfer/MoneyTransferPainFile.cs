namespace BodoMoneyTransfer
{
    //class MoneyTransferPainFile
    //{
    //    private AdapterConnManager _db;

    //    public string Connection { set { _db = ConnManagerFactory.GetConnManager(value, "System.Data.SqlClient"); } }

    //    public string ErrorMailer;



    //    public void Start()
    //    {

    //        var lo = new SqlCommand
    //        {
    //            CommandType = CommandType.StoredProcedure,
    //            CommandText = "dbo.spTransactions_DTA_Absender"
    //        };

    //        var dt = _db.GetDataTable(lo).AsEnumerable().ToList();
    //        foreach (var r in dt)
    //        {
    //            CreatePainFile(r["IBAN"].ToString(), Convert.ToDateTime(r["Datum"].ToString()), r["Name"].ToString(), r["BIC"].ToString());
    //        }

    //    }

    //    private void CreatePainFile(string iban, DateTime datum, string name, string bic)
    //    {

    //        var lo = new SqlCommand { CommandType = CommandType.StoredProcedure, CommandText = "dbo.spTransactions_DTA_Sepa" };

    //        lo.Parameters.Add("@MyIBAN", SqlDbType.VarChar);
    //        lo.Parameters[0].Value = iban;

    //        lo.Parameters.Add("@MyDatum", SqlDbType.DateTime);
    //        lo.Parameters[0].Value = datum;

    //        var dt = _db.GetDataTable(lo).AsEnumerable().ToList();
    //        var ta = new List<HbciDebit>();

    //        foreach (var r in dt)
    //        {
    //            if (string.IsNullOrEmpty(r["A_HisIban"].ToString()))
    //            {
    //                continue;
    //            }

    //            var wert = Convert.ToDouble(r["A_Amount"].ToString());

    //            if (wert < 0.01) continue;

    //            var t = new HbciDebit
    //            {
    //                ZahlungspflichtigerNameEins1 = r["A_HisName"].ToString(),
    //                Kontonummer1 = r["A_HisIban"].ToString().Replace(" ", "").Replace(".", ""),
    //                Kreditinstitutcode1 = r["A_HisBIC"].ToString().Replace(" ", "").Replace(".", ""),
    //                Verwendungszweck = r["A_Purpose"].ToString(),
    //                Wert1 = Convert.ToDecimal(wert)
    //            };
    //            ta.Add(t);
    //        }

            
    //        // ToDo: Filename
    //        var x = new PainFile
    //            {
    //                MyBic = bic,
    //                MyIban = iban,
    //                MyName = name,
    //                Transactions = ta,
    //                TransferDate = datum,
    //                FileName = "blabla"
    //            };

    //        x.CreateFile();

    //    }
    //}
}
