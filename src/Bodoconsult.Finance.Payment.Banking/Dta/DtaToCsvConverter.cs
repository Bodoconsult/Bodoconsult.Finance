// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Globalization;
using System.IO;
using System.Text;
using DDBAC.Net.DTAUS;

namespace Bodoconsult.Finance.Payment.Banking.Dta;

/// <summary>
/// Converter for DTA to CSV conversion
/// </summary>
public class DtaToCsvConverter
{
    /// <summary>
    /// Filename of the source DTA file
    /// </summary>
    public string SourceFileName;

    /// <summary>
    /// Filename of the target CSV file
    /// </summary>
    public string TargetFileName;

    /// <summary>
    /// Convert the DTA file into a CSV file
    /// </summary>
    public void Convert()
    {

        if (string.IsNullOrEmpty(SourceFileName))
        {
            return;
        }

        var aDtaus = new BACDTAUSFile();
        aDtaus.Load(SourceFileName);

        var s = new StringBuilder();

        var count = aDtaus.RecordCount;

        for (var i = 0; i < count; i++)
        {
            var r = aDtaus[i];

            var sr =
                $"{r.HisName};{r.HisAccountNumber};{r.HisBankCode};{r.Amount.ToString(CultureInfo.InvariantCulture).Replace(".", ",")};{r.Purpose.Replace("\r\n", "??BR??")}";


            s.AppendLine(sr);
        }

        var sw = new StreamWriter(TargetFileName,false, Encoding.GetEncoding("utf-8"));
        sw.Write(s);
        sw.Close();
    }
}