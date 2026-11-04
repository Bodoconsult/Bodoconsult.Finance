//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.Diagnostics;
//using System.Linq;
//using Bodoconsult.Database;
//using Bodoconsult.Finance.Payment.Banking.Hbci;
//using Bodoconsult.Finance.Payment.Banking.Hbci.Models;
//using Bodoconsult.Finance.Payment.Base;
//using Bodoconsult.Finance.Payment.Base.Helpers;
//using Bodoconsult.Finance.Payment.Base.Models;

//namespace BodoMoneyTransfer
//{

//    class MoneyTransferSepa
//    {

//        public string ErrorMailer { get; set; }

//        private AdapterConnManager _db;

//        public string Connection { set { _db = ConnManagerFactory.GetConnManager(value, "System.Data.SqlClient"); } }


//        private readonly List<MoneyTransferContactSepa> _moneyTransContacts = new List<MoneyTransferContactSepa>();

//        //private readonly HbciBanking _banking = new HbciBanking();

//        //public HbciBanking Banking
//        //{
//        //    get { return _banking; }
//        //}

//        public event StatusHandler StatusChange;


//        //public MoneyTransferSepa()
//        //{
            
//        //}

//        /// <summary>
//        /// 
//        /// </summary>
//        public void GetContacts()
//        {
//            SetStatus("Erforderliche HBCI-Kontakte ermitteln");
//            var dt = _db.GetDataTable("EXEC dbo.spTransactions_Contacts_Sepa").AsEnumerable().ToList();
//            foreach (var m in from DataRow r in dt select new MoneyTransferContactSepa(this)
//                                                                   {
//                                                                       Name = r["A_HBCI_Contact"].ToString(),
//                                                                       Passphrase = r["A_HBCI_Passphrase"].ToString(),
//                                                                       Iban = r["A_MyIban"].ToString(),
//                                                                       AccountName = r["A_MyName"].ToString(),
//                                                                       Bic = r["A_MyBic"].ToString(),
//                                                                       ErrorMailer = ErrorMailer,
//                                                                   })
//            {
//                _moneyTransContacts.Add(m);
//            }

//        }




//        public void StartJobCollector()
//        {
//            SetStatus("JobCollector gestartet...");
//            var dt = _db.GetDataTable("EXEC dbo.spJobCollector_Start");
//            foreach (DataRow r in dt.Rows)
//            {
//                _db.Exec(r["JC_SQL"].ToString(), false);
//            }

//            SetStatus("JobCollector fertig!");
//        }

//        public void RunTransferTransactions()
//        {
//            foreach (var m in _moneyTransContacts)
//            {
//                SetStatus("Überweisungen für " + m.Name + " Konto " + m.Iban);
//                m.RunTransferTransactions();
//            }


//        }


//        //public void RunTransferTransactionsSepa()
//        //{
//        //    foreach (var m in _moneyTransContacts)
//        //    {
//        //        SetStatus("SEPA-Überweisungen für " + m.Name + " Konto " + m.Iban);
//        //        m.RunTransferTransactions();
//        //    }


//        //}


//        public void RunDebitTransactions()
//        {
//            foreach (var m in _moneyTransContacts)
//            {
//                m.RunDebitTransactions();            
//            }
//        }


//        //public void RunAccountStatements()
//        //{

//        //    var dv = DateTime.Now.AddMonths(-2);
//        //    var db = DateTime.Now;




//        //    var lo = new SqlCommand
//        //    {
//        //        CommandType = CommandType.StoredProcedure,
//        //        CommandText = "dbo.spAccountStatements_GetStatements"
//        //    };

//        //    var dt = _db.GetDataTable(lo).AsEnumerable().ToList();

//        //    foreach (var r in dt)
//        //    {
//        //        var banking = new HbciBanking();
//        //        banking.OnHbciMessage += OnPaymentMessage;

//        //        banking.Contact = r["AS_HBCI_Contact"].ToString();
//        //        banking.Passphrase = r["AS_HBCI_Passphrase"].ToString();
//        //        banking.Account = r["AS_AccountNr"].ToString();
//        //        banking.OpenAccount();



//        //        SetStatus("Kontoauszug für " + r["AS_AccountNr"]);


//        //        var _AS = banking.GetAccountStatement(dv, db);
//        //        if (_AS == null)
//        //        {
//        //            SetStatus(string.Format("{0} Items", 0));
//        //            continue;
//        //        }

//        //        SetStatus(string.Format("{0} Items", _AS.Length));
//        //        foreach (var h in _AS)
//        //        {
//        //            lo = new SqlCommand
//        //                     {
//        //                         CommandType = CommandType.StoredProcedure,
//        //                         CommandText = r["AS_SQL"].ToString()
//        //                     };
//        //            lo.Parameters.Add("@AccountNr", SqlDbType.VarChar);
//        //            lo.Parameters[0].Value = r["AS_AccountNr"].ToString();
//        //            lo.Parameters.Add("@BankCode", SqlDbType.VarChar);
//        //            lo.Parameters[1].Value = r["AS_BankCode"].ToString();
//        //            lo.Parameters.Add("@ValidityDate", SqlDbType.VarChar);
//        //            lo.Parameters[2].Value = h.ValidityDate;
//        //            lo.Parameters.Add("@EntryDate", SqlDbType.VarChar);
//        //            lo.Parameters[3].Value = h.EntryDate;
//        //            lo.Parameters.Add("@Sender", SqlDbType.VarChar);
//        //            lo.Parameters[4].IsNullable = true;
//        //            lo.Parameters[4].Value = "" + h.Name;
//        //            lo.Parameters.Add("@Amount", SqlDbType.Float);
//        //            lo.Parameters[5].Value = h.Amount;
//        //            lo.Parameters.Add("@Text", SqlDbType.VarChar);
//        //            lo.Parameters[6].IsNullable = true;
//        //            lo.Parameters[6].Value = string.Format("{0} {1}", CleanString(h.BusinessTransactionText), CleanString(h.Purpose)).Trim();

//        //            lo.Parameters.Add("@OGUID", SqlDbType.VarChar);
//        //            lo.Parameters[7].Value = r["AS_ID"].ToString();
//        //            _db.Exec(lo);
//        //        }

//        //        banking.Quit();
//        //    }

//        //}

//        //private static string CleanString(string value)
//        //{
//        //    if (string.IsNullOrEmpty(value)) return "";

//        //    return value.Trim();
//        //}


//        internal void OnPaymentMessage(PaymentMessage error)
//        {
//            SetStatus(string.Format("{0} {1}", error.ErrorCode, error.Message));
//        }


//        /// <summary>
//        /// Statusanzeige
//        /// </summary>
//        /// <param name="msg"></param>
//        private void SetStatus(string msg)
//        {
//            if (StatusChange != null) StatusChange(msg);
//        }


//        class MoneyTransferContactSepa
//        {
//            private readonly MoneyTransferSepa _mt;

//            public string ErrorMailer { private get; set; }

//            public MoneyTransferContactSepa(MoneyTransferSepa m)
//            {
//                _mt = m;
//            }


//            public string Name { get; set; }

//            public string Passphrase { private get; set; }

//            public string AccountName { private get; set; }

//            public string Iban { get; set; }

//            public string Bic { private get; set; }


//            //public BodoConsult.dbconn.AdapterConnManager DBConn { get; set; }


//            public void RunTransferTransactions()
//            {

//                var lo = new SqlCommand
//                {
//                    CommandType = CommandType.StoredProcedure,
//                    CommandText = "dbo.spTransactions_ContactCurrentTransactions_Sepa"
//                };
//                lo.Parameters.Add("@HBCI_Contact", SqlDbType.VarChar);
//                lo.Parameters[0].Value = Name;
//                lo.Parameters.Add("@MyIban", SqlDbType.VarChar);
//                lo.Parameters[1].Value = Iban;
//                lo.Parameters.Add("@Typ", SqlDbType.VarChar);
//                lo.Parameters[2].Value = "U";

//                var dt = _mt._db.GetDataTable(lo).AsEnumerable().ToList();

//                if (dt.Count <= 0) return;

//                var ta = new List<BankingOrder>();
//                //var i = 0;


//                foreach (var transaction in from ueberweisung in dt
//                                select new BankingOrder
//                                    {
//                                        PayerOrPayee = ueberweisung["A_HisName"].ToString(),
//                                        Iban = ueberweisung["A_HisIban"].ToString(),
//                                        Bic = ueberweisung["A_HisBic"].ToString(),
//                                        Description = ueberweisung["A_Purpose"].ToString(),
//                                        Total = Convert.ToDecimal(ueberweisung["A_Amount"].ToString()),
//                                        Currency = "EUR",
//                                        Guid = ueberweisung["A_ID"].ToString(),
//                                        ExecOnSuccess = ueberweisung["A_SQL_Success"].ToString(),
//                                        ExecOnFailure = ueberweisung["A_SQL_Error"].ToString()
//                                    })
//                {

//                    if (IbanValidator.Validate(transaction.Iban) != IbanValidationResult.IsValid)
//                    {

//                        var mail = string.Format("{0} ({1}): IBAN ungültig", transaction.PayerOrPayee, transaction.Iban);

//                        lo = new SqlCommand
//                        {
//                            CommandType = CommandType.StoredProcedure,
//                            CommandText = "dbo.[spMAIL_New]"
//                        };
//                        lo.Parameters.Add("@To", SqlDbType.VarChar);
//                        lo.Parameters[0].Value = ErrorMailer;
//                        lo.Parameters.Add("@Subject", SqlDbType.VarChar);
//                        lo.Parameters[1].Value = "BodoMoneyTransfer: IBAN ungültig";
//                        lo.Parameters.Add("@Msg", SqlDbType.VarChar);
//                        lo.Parameters[2].Value = mail;
//                        _mt._db.Exec(lo);

//                        continue;
//                    }

//                    ta.Add(transaction);
//                    //i++;
//                }

//                var banking = new HbciBanking
//                {
//                    Contact = Name,
//                    Passphrase = Passphrase,
//                    Iban = Iban,
//                    UserName = AccountName
//                };
//                banking.OnHbciMessage += OnPaymentMessage;
//                banking.OpenAccountSepa();

//                // Bic mit übergeben, solange Bic nicht von Bank geliefert wird
//                var message = banking.TransferMultipleSepa(ta, Bic);
   
//                foreach(var t in ta)
//                {

//                    // erfolgreich
//                    if (message.ErrorCode >= 0 && message.ErrorCode < 4000)
//                    {
//                        var msg = string.Format("SEPA-DebitTransaction {0}: {1} // {2} // {3}", 
//                            t.Guid,
//                            banking.Error
//                            , message.ErrorCode
//                            , message.Message);


//                        Trace.TraceInformation(msg);

//                        lo = new SqlCommand
//                        {
//                            CommandType = CommandType.StoredProcedure,
//                            CommandText = "[dbo].[spTransactions_MoveToArchive_Sepa]"
//                        };
//                        lo.Parameters.Add("@A_ID", SqlDbType.VarChar);
//                        lo.Parameters[0].Value = t.Guid;
//                        _mt._db.Exec(lo);

//                        if (string.IsNullOrEmpty(t.ExecOnSuccess) == false) _mt._db.Exec(t.ExecOnSuccess, false);

//                    }
//                    else  // mit Fehlern
//                    {
//                        var msg = string.Format("SEPA-DebitTransaction {0}: {1} // {2} // {3}",
//                            t.Guid,
//                            banking.Error, 
//                            message.ErrorCode, 
//                            message.Message);

//                        Trace.TraceError(msg);

//                        if (!string.IsNullOrEmpty(msg))
//                        {
//                            lo = new SqlCommand
//                            {
//                                CommandType = CommandType.StoredProcedure,
//                                CommandText = "dbo.[spMAIL_New]"
//                            };
//                            lo.Parameters.Add("@To", SqlDbType.VarChar);
//                            lo.Parameters[0].Value = ErrorMailer;
//                            lo.Parameters.Add("@Subject", SqlDbType.VarChar);
//                            lo.Parameters[1].Value = "BodoMoneyTransfer: SEPA-Überweisungen fehlgeschlagen";
//                            lo.Parameters.Add("@Msg", SqlDbType.VarChar);
//                            lo.Parameters[2].Value = msg;
//                            _mt._db.Exec(lo);
//                        }

//                        if (string.IsNullOrEmpty(t.ExecOnFailure) == false) _mt._db.Exec(t.ExecOnFailure, false);
//                    }
//                }

//                /*

//                if (_banking.Error == 0 && message.ErrorCode >= 0 && message.ErrorCode < 4000)
//                {

//                    foreach (var ud in dt)
//                    {
//                        var msg = string.Format("SEPA-DebitTransaction {0}: {1} // {2} // {3}", ud["A_ID"],
//                                                _banking.Error, message.ErrorCode, message.Message);
//                        Trace.TraceInformation(msg);

//                        lo = new SqlCommand
//                        {
//                            CommandType = CommandType.StoredProcedure,
//                            CommandText = "[dbo].[spTransactions_MoveToArchive_Sepa]"
//                        };
//                        lo.Parameters.Add("@A_ID", SqlDbType.VarChar);
//                        lo.Parameters[0].Value = ud["A_ID"].ToString();
//                        _mt._db.Exec(lo);

//                        if (string.IsNullOrEmpty(ud["A_SQL_Success"].ToString()) == false) _mt._db.Exec(ud["A_SQL_Success"].ToString(), false);
//                    }
//                }
//                //// Spezielle Warnung auf Änderungen an HBCI
//                //else if (_Message.ErrorCode == 3060)
//                //{
//                //    // Mail versenden

//                 //    foreach (DataRow _UD in _DT.Rows)
//                //    {

//                 //    }
//                //}
//                else
//                {

//                    var mail = "";

//                    foreach (var ud in dt)
//                    {
//                        var msg = string.Format("SEPA-DebitTransaction {0}: {1} // {2} // {3}", ud["A_ID"],
//                                 _banking.Error, message.ErrorCode, message.Message);

//                        mail = mail + msg + "<br/>\r\n";
//                        Trace.TraceError(msg);

//                        if (string.IsNullOrEmpty(ud["A_SQL_Error"].ToString()) == false) _mt._db.Exec(ud["A_SQL_Error"].ToString(), false);
//                    }


//                    if (!string.IsNullOrEmpty(mail))
//                    {
//                        lo = new SqlCommand
//                                 {
//                                     CommandType = CommandType.StoredProcedure,
//                                     CommandText = "dbo.[spMAIL_New]"
//                                 };
//                        lo.Parameters.Add("@To", SqlDbType.VarChar);
//                        lo.Parameters[0].Value = ErrorMailer;
//                        lo.Parameters.Add("@Subject", SqlDbType.VarChar);
//                        lo.Parameters[1].Value = "BodoMoneyTransfer: SEPA-Überweisungen fehlgeschlagen";
//                        lo.Parameters.Add("@Msg", SqlDbType.VarChar);
//                        lo.Parameters[2].Value = mail;
//                        _mt._db.Exec(lo);
//                    }

//                }
//                */


//                banking.Quit();


//            }

//            public void RunDebitTransactions()
//            {
//                //var lo = new SqlCommand();
//                //lo.CommandType = CommandType.StoredProcedure;
//                //lo.CommandText = "dbo.spTransactions_ContactCurrentTransactions";
//                //lo.Parameters.Add("@HBCI_Contact", SqlDbType.VarChar);
//                //lo.Parameters[0].Value = Name;
//                //lo.Parameters.Add("@MyAccount", SqlDbType.VarChar);
//                //lo.Parameters[1].Value = Account;
//                //lo.Parameters.Add("@Typ", SqlDbType.VarChar);
//                //lo.Parameters[2].Value = "L";

//                //var dt = _MT._db.GetDataTable(lo);
//                //if (dt.Rows.Count > 0)
//                //{

//                //    HbciDebit[] _TA = new HbciDebit[dt.Rows.Count];

//                //    int i = 0;


//                //    foreach (DataRow ud in dt.Rows)
//                //    {
//                //        var t = new HbciDebit
//                //        {
//                //            ZahlungspflichtigerNameEins1 = ud["A_HisName"].ToString(),
//                //            Kontonummer1 = ud["A_HisAccount"].ToString(),
//                //            Kreditinstitutcode1 = ud["A_HisBankCode"].ToString(),
//                //            Verwendungszweck = ud["A_Purpose"].ToString(),
//                //            Wert1 = Convert.ToDecimal(ud["A_Amount"].ToString())
//                //        };


//                //        _TA[i] = t;
//                //        i++;
//                //    }



//                //    var b = new HbciBanking();
//                //    b.OnHbciMessage += _B_OnHBCIMessage;
//                //    b.Passphrase = Passphrase;
//                //    b.Account = Account;
//                //    b.Contact = Name;
//                //    b.UserName = AccountName;
//                //    b.OpenAccount();

//                //    var message = b.DebitMultiple(_TA);
//                //    b.Quit();

//                //    // erfolgreich
//                //    if (b.Error == 0 && message.ErrorCode >= 0 && message.ErrorCode < 4000)
//                //    {
//                //        foreach (DataRow ud in dt.Rows)
//                //        {
//                //            lo = new SqlCommand
//                //            {
//                //                CommandType = CommandType.StoredProcedure,
//                //                CommandText = "[dbo].[spTransactions_MoveToArchive]"
//                //            };
//                //            lo.Parameters.Add("@A_ID", SqlDbType.VarChar);
//                //            lo.Parameters[0].Value = ud["A_ID"].ToString();
//                //            _MT._db.Exec(lo);

//                //            if (string.IsNullOrEmpty(ud["A_SQL_Success"].ToString()) == false) _MT._db.Exec(ud["A_SQL_Success"].ToString(), false);
//                //        }
//                //    }
//                //    //// Spezielle Warnung auf Änderungen an HBCI
//                //    //else if (_Message.ErrorCode == 3060)
//                //    //{
//                //    //    // Mail versenden

//                //    //    foreach (DataRow _UD in _DT.Rows)
//                //    //    {

//                //    //    }
//                //    //}
//                //    else
//                //    {
//                //        foreach (DataRow ud in dt.Rows)
//                //        {
//                //            if (string.IsNullOrEmpty(ud["A_SQL_Error"].ToString()) == false) _MT._db.Exec(ud["A_SQL_Error"].ToString(), false);
//                //        }
//                //    }

//                //}

//            }





//            private void OnPaymentMessage(PaymentMessage error)
//            {
//                _mt.SetStatus(string.Format("{0} {1}", error.ErrorCode, error.Message));
//            }

//        }

//    }
//}
