// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Bodoconsult.Finance.Payment.Base.Helpers;

/// <summary>
/// IBAN validator helper class
/// </summary>
public static class IbanValidator
{
    /// <summary>
    /// Validate an IBAN
    /// </summary>
    /// <param name="iban">IBAN to check for being valid</param>
    /// <returns></returns>
    public static IbanValidationResult Validate(string iban)
    {
        // Check if value is missing
        if (string.IsNullOrEmpty(iban))
        {
            return IbanValidationResult.ValueMissing;
        }

        if (iban.Length < 2)
        {
            return IbanValidationResult.ValueTooSmall;
        }

        var countryCode = iban[..2].ToUpper();
        var lengthForCountryCode = Lengths[countryCode];
        // Check length.
        if (iban.Length < lengthForCountryCode)
        {
            return IbanValidationResult.ValueTooSmall;
        }

        if (iban.Length > lengthForCountryCode)
        {
            return IbanValidationResult.ValueTooBig;
        }

        iban = iban.ToUpperInvariant();
        var newIban = iban[4..] + iban[..4];

        newIban = Regex.Replace(newIban, @"\D", match => (match.Value[0] - 55).ToString(CultureInfo.InvariantCulture));

        var remainder = BigInteger.Parse(newIban) % 97;

        return remainder != 1 ? IbanValidationResult.ValueFailsModule97Check : IbanValidationResult.IsValid;
    }

    private static readonly Dictionary<string, int> Lengths = new()
    {
        {"AL", 28},
        {"AD", 24},
        {"AT", 20},
        {"AZ", 28},
        {"BE", 16},
        {"BH", 22},
        {"BA", 20},
        {"BR", 29},
        {"BG", 22},
        {"CR", 21},
        {"HR", 21},
        {"CY", 28},
        {"CZ", 24},
        {"DK", 18},
        {"DO", 28},
        {"EE", 20},
        {"FO", 18},
        {"FI", 18},
        {"FR", 27},
        {"GE", 22},
        {"DE", 22},
        {"GI", 23},
        {"GR", 27},
        {"GL", 18},
        {"GT", 28},
        {"HU", 28},
        {"IS", 26},
        {"IE", 22},
        {"IL", 23},
        {"IT", 27},
        {"KZ", 20},
        {"KW", 30},
        {"LV", 21},
        {"LB", 28},
        {"LI", 21},
        {"LT", 20},
        {"LU", 20},
        {"MK", 19},
        {"MT", 31},
        {"MR", 27},
        {"MU", 30},
        {"MC", 27},
        {"MD", 24},
        {"ME", 22},
        {"NL", 18},
        {"NO", 15},
        {"PK", 24},
        {"PS", 29},
        {"PL", 28},
        {"PT", 25},
        {"RO", 24},
        {"SM", 27},
        {"SA", 24},
        {"RS", 22},
        {"SK", 24},
        {"SI", 19},
        {"ES", 24},
        {"SE", 24},
        {"CH", 21},
        {"TN", 24},
        {"TR", 26},
        {"AE", 23},
        {"GB", 22},
        {"VG", 24}
    };
}

/// <summary>
/// Payment app helper
/// </summary>
public static class PaymentAppHelper
{
    /// <summary>
    /// Get the product version out of the assemblies file version
    /// </summary>
    /// <returns>Product version string</returns>
    public static string GetProductVersion()
    {
        // Set product version
        var assembly = Assembly.GetExecutingAssembly();
        var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

        if (fvi.FileVersion is null)
        {
            return "1.0";
        }

        var versiondata = fvi.FileVersion.Split(["."], StringSplitOptions.RemoveEmptyEntries);

        return $"{versiondata[0]}.{versiondata[1]}";
    }
}