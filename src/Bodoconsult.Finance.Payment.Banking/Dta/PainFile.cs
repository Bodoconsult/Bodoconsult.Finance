//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Reflection;
//using Bodoconsult.Finance.Payment.Banking.Models;
//using Bodoconsult.Finance.Payment.Base.Exceptions;
//using Bodoconsult.Finance.Payment.Base.Helpers;
//using DDBAC.Net;

//namespace Bodoconsult.Finance.Payment.Banking.Dta;

//public class PainFile
//{

//    public string FileName;

//    public IEnumerable<BankingOrder> Transactions;

//    public string MyIban;

//    public string MyName;

//    public string MyBic;

//    public DateTime TransferDate;


//    public void CreateFile()
//    {

//        if (IbanValidator.Validate(MyIban) != IbanValidationResult.IsValid)
//        {
//            throw new Exception("IBAN des Absenders ungültig!");
//        }

//        if (TransferDate == DateTime.MinValue) TransferDate = DateTime.Now;

//        var aFromAccount = new BACAccount
//        {
//            CountryCode = string.Empty,
//            NameEins = ClearSepaString(MyName),
//            IBAN = MyIban,
//            BIC = MyName

//        };


//        var dir = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
//        if (dir == null) return;

//        var aSepaMessage = new BACSepaMessage();
//        aSepaMessage.Load(Path.Combine(dir, @"sepa_pain_001_003_03.txt"));
//        aSepaMessage.FromAccount = aFromAccount;
//        aSepaMessage.ExecutionDate = TransferDate;

//        for(var i = 0; i<=aSepaMessage.Orders.Count-1; i++)
//        {
//            aSepaMessage.Orders.Remove(i);
//        }
            

//        // Todo: Export der Daten

//        foreach (var t in Transactions)
//        {

//            try
//            {
//                if (string.IsNullOrEmpty(t.Iban)) continue;

//                var aSepaOrder = aSepaMessage.Orders.Add();

//                var empf = t.PayerOrPayee;

//                if (empf.Length > 27) empf = empf.Substring(0, 27);

//                if (IbanValidator.Validate(t.Iban) != IbanValidationResult.IsValid)
//                {
//                    throw new Exception($"IBAN {t.Iban} für Empfänger {t.PayerOrPayee} ungültig!");
//                }

//                var aToAccount = new BACAccount
//                {
//                    CountryCode = string.Empty,
//                    NameEins = ClearSepaString(empf),
//                    IBAN = t.Iban,
//                    BIC = t.Bic
//                };

//                aSepaOrder.ToAccount = aToAccount;
//                aSepaOrder.Amount = Math.Round(t.Total, 2);
//                aSepaOrder.CurrencyCode = (string.IsNullOrEmpty(t.Currency)) ? "EUR" : t.Currency;
//                aSepaOrder.Purpose = ClearSepaString(t.Description.Replace("??BR????BR??", "\r\n").Replace("??BR??", "\r\n"));
//            }
//            catch
//            {
//                var msg =
//                    $"Fehler in Buchungssatz:\r\nZahlungspflichtiger: {t.PayerOrPayee}\r\nKontonummer: {t.Iban}\r\nKreditinstitutcode: {t.Bic}\r\nVerwendungszweck:{t.Description}\r\nWert{t.Total}";
//                throw (new PaymentException(msg));
//            }
//        }

//        var sErrorReason = string.Empty;

//        aSepaMessage.Verify(ref sErrorReason);
//        if (!string.IsNullOrEmpty(sErrorReason))
//        {
//            throw new Exception(sErrorReason);
//        }

//        if (FileName == null)
//        {
//            return;
//        }

//        if (File.Exists(FileName))
//        {
//            File.Delete(FileName);
//        }
//        aSepaMessage.Save(FileName);
//    }


//    private static string ClearSepaString(string value)
//    {

//        value =
//            value.Replace("Ö", "oe")
//                .Replace("ö", "oe")
//                .Replace("Ä", "Ae")
//                .Replace("ä", "ae")
//                .Replace("Ü", "Ue")
//                .Replace("ü", "ue")
//                .Replace("ß", "ss")
//                .Replace(".", string.Empty)
//                .Replace("-", string.Empty);

//        return value;
//    }
//}