// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.Finance.Payment.Banking.Ebics;
using Bodoconsult.Finance.Payment.Base.Interfaces;
using System;
using System.Runtime.Versioning;

namespace Bodoconsult.Finance.Payment.Banking.Factories;

/// <summary>
/// Factory for EBICS banking service
/// </summary>
[SupportedOSPlatform("windows")]
public class EbicsBankingServiceFactory : IPaymentServiceFactory
{
    /// <summary>
    /// FINTS-Registriernummer as product name since 1.8.2019
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// ProductVersion (Länge 5 Zeichen)
    /// </summary>
    public string ProductVersion { get; set; } = "1.0";

    /// <summary>
    /// Create a payment service
    /// </summary>
    /// <param name="contact">Name of a banking contact</param>
    /// <returns><see cref="IPaymentService"/> instance</returns>
    public IPaymentService CreateInstance(IPaymentContact contact)
    {
        if (string.IsNullOrEmpty(ProductName))
        {
            ArgumentException.ThrowIfNullOrEmpty(ProductName);
        }

        if (contact.GetType() != typeof(EbicsBankingContact))
        {
            throw new Exception($"Contact has wrong type '{contact.GetType().Name}' instead of 'BankingContact'");
        }

        var service = new EbicsBankingService(ProductName, ProductVersion)
        {
            PaymentContact = contact
        };
        var result = service.OpenContact();

        return !result ? null : service;
    }
}