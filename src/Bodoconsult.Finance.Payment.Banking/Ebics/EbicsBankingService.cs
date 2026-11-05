// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Runtime.Versioning;
using System.Text;
using Bodoconsult.Finance.Payment.Banking.Base;

namespace Bodoconsult.Finance.Payment.Banking.Ebics;

/// <summary>
/// Service class for EBICS
/// </summary>
[SupportedOSPlatform("windows")]
public class EbicsBankingService : BaseBankingService
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public EbicsBankingService(string productName, string productVersion)
    {
        ProductName = productName;
        ProductVersion = productVersion;

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        GetBanking();
    }

    /// <summary>
    /// Get the banking instance
    /// </summary>
    public sealed override void GetBanking()
    {
        if (string.IsNullOrEmpty(ProductName))
        {
            throw new Exception("FinTS product name (ZKA registration number) missing!");
        }

        if (string.IsNullOrEmpty(ProductVersion))
        {
            throw new Exception("FinTS product version missing!");
        }

        Banking = new EbicsBanking(ProductName, ProductVersion);
        //Banking.OnHbciMessage += OnPaymentMessage;
    }
}