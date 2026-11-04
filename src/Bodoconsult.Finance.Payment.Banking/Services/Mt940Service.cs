// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Versioning;
using System.Text;
using Bodoconsult.App.Helpers;
using Bodoconsult.Finance.Payment.Base.Models;
using DDBAC.Net.Swift;

namespace Bodoconsult.Finance.Payment.Banking.Services;

/// <summary>
/// MT940 service creating MT940 files
/// </summary>
[SupportedOSPlatform("windows")]
public class Mt940Service
{
    private static readonly CultureInfo AmericanCulture = new("en-US");
    private readonly Mt940File _file = new();

    //public Mt940Service()
    //{

    //}

    /// <summary>
    /// Target filename
    /// </summary>
    public string TargetFileName { get; set; }

    /// <summary>
    /// Data file
    /// </summary>
    public Mt940File DataFile => _file;

    /// <summary>
    /// Save as MT940 file
    /// </summary>
    /// <returns></returns>
    public bool SaveMt940File()
    {
        FileSystemHelper.DeleteFile(TargetFileName);

        CreateFile();

        return false;
    }

    /// <summary>
    /// Returns a date string in MT940 style YYMMDD
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static string DateFormatter(DateTime date)
    {
        return date.ToString("yyMMdd");
    }

    /// <summary>
    /// Returns short date time string MMDD for validity date i.e.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static string DateFormatterShort(DateTime date)
    {
        return date.ToString("MMdd");
    }

    /// <summary>
    /// Returns a date with time string in MT940 style YYMMDDHHmm
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static string DateTimeFormatter(DateTime date)
    {
        return date.ToString("yyMMddhhmm");
    }

    /// <summary>
    /// Formatting a number in a SWIFT number
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string DecimalFormatter(decimal value)
    {
        var erg = value.ToString("0.00", AmericanCulture).Replace(",", string.Empty).Replace(".", ",");
        return erg;
    }

    /// <summary>
    /// Write the content into the MT940 file
    /// </summary>
    public void CreateFile()
    {
        TextWriter tw = new StreamWriter(TargetFileName, true, Encoding.GetEncoding("Windows-1252"));
        tw.Write(GetContent());
        tw.Close();
    }

    /// <summary>
    /// Create the content of the MT940 file
    /// </summary>
    /// <returns></returns>
    private string GetContent()
    {
        var erg = new StringBuilder();

        erg.Append(GetHeader());

        GetItems(erg);

        erg.Append(GetFooter());

        return erg.ToString();
    }

    private void GetItems(StringBuilder erg)
    {
        if (DataFile.Items == null || DataFile.Items.Count == 0)
        {
            return;
        }

        foreach (var item in DataFile.Items)
        {
            erg .Append(
                $"\r\n:61:{DateFormatter(item.ValidityDate)}{DateFormatterShort(item.EntryDate)}{((item.Amount >= 0) ? "C" : "D")}{string.Empty}{DecimalFormatter(Math.Abs(item.Amount))}{((string.IsNullOrEmpty(item.TransactionType)) ? "NMSC" : item.TransactionType)}");

            var zwerg =
                $":86:{item.BusinessTransactionCode}?00{item.BusinessTransactionText}?10{item.PrimaNota}{Get27Lines(item.Purpose, 20, 27)}";

            if (!string.IsNullOrEmpty(item.BankCode))
            {
                zwerg += $"?30{item.BankCode}";
                zwerg += $"?31{item.AccountNo}";
            }


            zwerg += Get27Lines(item.Name, 32, 27);

            if (!string.IsNullOrEmpty(item.TextKeySupplement))
            {
                zwerg += $"?34{item.TextKeySupplement}";
            }


            erg.Append( $"\r\n{Get6x65Lines(zwerg)}");
        }
    }

    private static string Get6x65Lines(string zwerg)
    {
        var erg = string.Empty;

        var daten = zwerg.Split('?');
        var length = 0;

        var start = true;

        foreach (var s in daten)
        {
            length += s.Length + 1;
            if (length > 64)
            {
                length = s.Length + 1;
                erg += "\r\n";
            }

            erg += ((start) ? string.Empty : "?") + s;
            start = false;
        }

        return erg;
    }

    /// <summary>
    /// Divide purpose in 27 chars long pieces for MT940-Segment :86
    /// </summary>
    /// <param name="purpose"></param>
    /// <param name="startValue"></param>
    /// <param name="lineLength"></param>
    /// <returns></returns>
    private static string Get27Lines(string purpose, int startValue, int lineLength)
    {
        var i = startValue;

        if (string.IsNullOrEmpty(purpose))
        {
            return string.Empty;
        }

        purpose = purpose.Replace("Ä", "Ae")
            .Replace("ä", "ae")
            .Replace("Ü", "Ue")
            .Replace("ü", "ue")
            .Replace("Ö", "Oe")
            .Replace("ö", "oe")
            .Replace("ß", "ss").Replace("\r\n", string.Empty);

        var length = purpose.Length;

        if (length < lineLength + 1)
        {
            return $"?{i}{purpose}";
        }

        var erg = string.Empty;
        var j = 0;
        while (j < length - 1)
        {

            var number = (j + lineLength <= length - 1) ? lineLength : length - j;

            erg += $"?{i}{purpose.Substring(j, number)}";


            i++;
            if (i > lineLength + 2) break;
            j += lineLength;
        }

        return erg;
    }

    private string GetHeader()
    {
        var erg = string.Empty;

        erg += $":20:{DataFile.References}\r\n";
        erg += $":25:{DataFile.AccountNumber}\r\n";
        erg += $":28C:{DataFile.StatementNumber}\r\n";

        erg += string.Format(":60F:{3}{2}{1}{0}",
            DecimalFormatter(Math.Abs(DataFile.OpeningBalanceAmount)),
            DataFile.OpeningBalanceCurrency,
            DateFormatter(DataFile.OpeningBalanceDate),
            (DataFile.OpeningBalanceAmount >= 0) ? "C" : "D"
        );


        return erg;
    }


    private string GetFooter()
    {
        var erg = $"\r\n{string.Format(":62F:{3}{2}{1}{0}",
            DecimalFormatter(Math.Abs(DataFile.ClosingBalanceAmount)),
            DataFile.ClosingBalanceCurrency,
            DateFormatter(DataFile.ClosingBalanceDate),
            (DataFile.ClosingBalanceAmount >= 0) ? "C" : "D"
        )}";


        return erg;
    }

    /// <summary>
    /// Save as MT940 file and PDF file
    /// </summary>
    /// <returns>False on success else true</returns>
    public bool SaveMt940FileAndPdf()
    {
        SaveMt940File();

        // Arrange
        var pdf = TargetFileName.Replace(".sta", ".pdf");

        FileSystemHelper.DeleteFile(pdf);

        var header = new AccountStatementHeader
        {
            AccountOwner = _file.AccountOwner,
            AccountNr = _file.AccountNumber,
            StartDate = _file.OpeningBalanceDate,
            EndDate = _file.ClosingBalanceDate,
            BankIdentifier = _file.AccountNumber
        };

        // Act

        var erg = LoadMt940FileAndCreateAccountStatement(TargetFileName, header);
        return false;
    }

    /// <summary>
    /// Load a MT940 file and create an account statement
    /// </summary>
    /// <param name="targetFile">Target filename</param>
    /// <param name="header">Account statement header</param>
    /// <returns>False on success else true</returns>
    public static bool LoadMt940FileAndCreateAccountStatement(string targetFile, AccountStatementHeader header)
    {
        if (!File.Exists(targetFile))
        {
            return true;
        }

        var oMt940 = new BACSwiftStatement();

        oMt940.Load(targetFile);

        header ??= new AccountStatementHeader
        {
            AccountNr = oMt940.AccountIdentificationCode,
            AccountOwner = "Unknown",
            BankIdentifier = oMt940.BankCode,
        };

        var pdfFile = targetFile.Replace(".sta", ".pdf");
        CreateAccountStatementAsPdf(ref oMt940, pdfFile, header);
        return false;
    }

    /// <summary>
    /// Create a PDF file from an MT940 account statement file
    /// </summary>
    /// <param name="oMt940">MT940 data</param>
    /// <param name="pdfFile">Filename of the PDF file</param>
    /// <param name="header">Account statement header</param>
    protected static void CreateAccountStatementAsPdf(ref BACSwiftStatement oMt940, string pdfFile, AccountStatementHeader header)
    {
        FileSystemHelper.DeleteFile(pdfFile);

        var data = GetAccountStatementData(oMt940);

        header.StartBalanceDate = oMt940.OpeningBalanceDate;
        header.StartBalanceAmount = oMt940.OpeningBalanceAmount;
        header.StartBalanceCurrency = oMt940.ClosingBalanceCurrencyCode;
        header.EndBalanceDate = oMt940.ClosingBalanceDate;
        header.EndBalanceAmount = oMt940.ClosingBalanceAmount;
        header.EndBalanceCurrency = oMt940.ClosingBalanceCurrencyCode;

        var pdf = new PdfAccountStatement(data, header, pdfFile);
        pdf.CreateFile();
    }

    /// <summary>
    /// Get account statement lines out of MT940 data
    /// </summary>
    /// <param name="oMt940">MT940 data</param>
    /// <returns>List of account statement lines</returns>
    public static IList<AccountStatementLine> GetAccountStatementData(BACSwiftStatement oMt940)
    {
        var erg = new List<AccountStatementLine>();

        try
        {
            foreach (var oLine in oMt940.StatementLines)
            {
                var h = new AccountStatementLine();


                // Das Wertstellungsdatum wird nunmehr über das neue Property
                // RealValidityDate ausgelesen und dargestellt. Dies ist erforderlich, da als
                // Wertstellungsdatum auch nicht existierende Tage wie der 30. Februar
                // auftauchen können.
                var sRealValidityDate = "        ";

                if (oLine.RealValidityDate != null)
                {
                    sRealValidityDate = oLine.RealValidityDate.Substring(4, 2);
                    sRealValidityDate += ".";
                    sRealValidityDate += oLine.RealValidityDate.Substring(2, 2);
                    sRealValidityDate += ".";
                    sRealValidityDate += oLine.RealValidityDate.Substring(0, 2);

                    if (sRealValidityDate.StartsWith("30.02."))
                    {
                        sRealValidityDate = $"28.02.{oLine.RealValidityDate.Substring(0, 2)}";
                    }
                }
                h.ValidityDate = Convert.ToDateTime(sRealValidityDate);

                h.Name = (string.IsNullOrEmpty(oLine.Name)) ? "Bank" : oLine.Name;
                    
                h.EntryDate = Convert.ToDateTime(CheckDate(oLine.EntryDate, h.ValidityDate));
                h.BusinessTransactionText = oLine.BusinessTransactionText.Trim();
                h.BusinessTransactionCode = oLine.BusinessTransactionCode.ToString().PadLeft(3).Replace(" ", "0");
                h.Amount = Convert.ToDecimal(FormatAmount(oLine.Amount));
                h.CurrencyCode = oLine.CurrencyCode;
                h.PrimaNota = oLine.PrimaNoteNumber;
                h.TransactionType = oLine.TransactionType;
                h.TextKeySupplement = oLine.TextKeySupplement;
                h.Transactionid = oLine.TransactionId;

                var iban = FindIban(oLine.IBAN, oLine.Purpose);
                var bic = FindBic(oLine.BIC, oLine.Purpose);

                h.AccountNo = iban;
                h.BankCode = bic;

                h.AccountNo ??= oLine.AccountNumber;
                h.BankCode ??= oLine.BankCode;

                //Debug.Print(bic);
                //Debug.Print(oLine.IBAN);

                if (oLine.OriginalCurrencyCode != null)
                {
                    h.OriginalCurrencyCode = oLine.OriginalCurrencyCode;
                    h.OriginalAmount = Convert.ToDouble(FormatAmount(oLine.OriginalAmount));
                }
                if (oLine.Purpose != null)
                {
                    h.Purpose += oLine.Purpose;

                    //string sPurpose = oLine.Purpose;
                    //int iPos;
                    //    while ((iPos = sPurpose.IndexOf("\r\n")) >= 0)
                    //    {
                    //        _H.Purpose += " " + sPurpose.Substring(0, iPos) + "\r\n";
                    //        sPurpose = sPurpose.Substring(iPos + 2);

                    //    }
                    //}
                }

                erg.Add(h);
                //                    i++;
            }
        }
        catch
        {
            // ignored
        }

        return erg;
    }

    /// <summary>
    /// Find the BIC in a purpose
    /// </summary>
    /// <param name="bic">Given BIC</param>
    /// <param name="purpose">Purpose to check for other BIC</param>
    /// <returns>BIC</returns>
    protected static string FindBic(string bic, string purpose)
    {
        if (!string.IsNullOrEmpty(bic) || string.IsNullOrEmpty(purpose)) return bic;

        purpose = purpose.Replace("\r\n", string.Empty);

        var i = purpose.IndexOf("BIC: ", StringComparison.OrdinalIgnoreCase);

        if (i < 0) return bic;

        var j = purpose.IndexOf(" ", i + 5, StringComparison.OrdinalIgnoreCase);

        if (j == -1) j = purpose.Length;

        return purpose.Substring(i + 5, j - i - 5);
    }

    /// <summary>
    /// Find the IBAN in a purpose
    /// </summary>
    /// <param name="iban">Given IBAN</param>
    /// <param name="purpose">Purpose to check for other IBAN</param>
    /// <returns>IBAN</returns>
    protected static string FindIban(string iban, string purpose)
    {
        if (!string.IsNullOrEmpty(iban) || string.IsNullOrEmpty(purpose))
        {
            return iban;
        }

        purpose = purpose.Replace("\r\n", string.Empty);

        var i = purpose.IndexOf("IBAN: ", StringComparison.OrdinalIgnoreCase);

        if (i < 0)
        {
            return iban;
        }

        var j = purpose.IndexOf(" ", i + 6, StringComparison.OrdinalIgnoreCase);
        var x = purpose.IndexOf("BIC:", i + 6, StringComparison.OrdinalIgnoreCase);

        if (x < j)
        {
            j = x;
        }

        if (j == -1)
        {
            j = purpose.Length;
        }

        return purpose.Substring(i + 6, j - i - 6);
    }

    /// <summary>
    /// Entry date and validity date must be in the same year
    /// </summary>
    /// <param name="date"></param>
    /// <param name="validityDate"></param>
    /// <returns></returns>
    protected static string CheckDate(DateTime date, DateTime validityDate)
    {
        if (date.Year != validityDate.Year)
        {
            date = new DateTime(validityDate.Year, date.Month, date.Day);
        }

        return FormatDate(date);
    }

    /// <summary>
    /// Format obhect as date string
    /// </summary>
    /// <param name="date">Date as object</param>
    /// <returns>Date as string</returns>
    public static string FormatDate(object date)
    {
        if (date is DateTime dt)
        {
            return dt.ToShortDateString();
        }
        return date.ToString();
    }

    /// <summary>
    /// Format an amount
    /// </summary>
    /// <param name="amount">Object with the amount</param>
    /// <returns></returns>
    public static string FormatAmount(object amount)
    {
        var val = (decimal)amount;
        return val.ToString("n", NumberFormatInfo.CurrentInfo);
    }
}