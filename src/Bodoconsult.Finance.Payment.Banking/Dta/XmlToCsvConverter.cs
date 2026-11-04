// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Globalization;
using System.IO;
using System.Text;
using DDBAC.Net;

namespace Bodoconsult.Finance.Payment.Banking.Dta;

/// <summary>
/// Converter for XML to CSV conversion
/// </summary>
public class XmlToCsvConverter
{
    /// <summary>
    /// Filename of the source XML file
    /// </summary>
    public string SourceFileName;
    
    /// <summary>
    /// Filename of the target CSV file
    /// </summary>
    public string TargetFileName;

    /// <summary>
    /// Convert the XML file into a CSV file
    /// </summary>
    public void Convert()
    {
        if (string.IsNullOrEmpty(SourceFileName))
        {
            return;
        }

        var aSepaMessage = new BACSepaMessage();
        aSepaMessage.Load(SourceFileName);

        var s = new StringBuilder();

        foreach(var r in aSepaMessage.Orders)
        {
            var sr = $"{r.ToAccount.NameEins};{r.ToAccount.BIC};{r.ToAccount.IBAN};{r.Amount.ToString(CultureInfo.InvariantCulture).Replace(".", ",")};{r.Purpose.Replace("\r\n", "??BR??")}";
            s.AppendLine(sr);
        }

        var sw = new StreamWriter(TargetFileName, false, Encoding.GetEncoding("utf-8"));
        sw.Write(s);
        sw.Close();
    }
}