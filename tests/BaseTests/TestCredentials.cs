// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.Finance.Payment.Banking.Ebics;
using Bodoconsult.Finance.Payment.Banking.Hbci;

namespace BaseTests;

/// <summary>
/// Credentials to be used for unit testing
/// </summary>
public class TestCredentials
{
    /// <summary>
    /// Alpha Vantage API token
    /// </summary>
    public string AlphaVantageApiToken { get; set; } = string.Empty;

    /// <summary>
    /// HBCI contacts for testing
    /// </summary>
    public List<HbciBankingContact> HbciContacts { get; } = [];

    /// <summary>
    /// EBICS contacts for testing
    /// </summary>
    public List<EbicsBankingContact> EbicsContacts { get; } = [];

    /// <summary>
    /// FINTS-Registriernummer as product name since 1.8.2019
    /// </summary>
    public string ProductName { get; set; } = string.Empty;
}