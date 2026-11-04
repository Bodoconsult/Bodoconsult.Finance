// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.Finance.Payment.Base.Interfaces;

/// <summary>
/// Factory interface for banking services
/// </summary>
public interface IPaymentServiceFactory
{
    /// <summary>
    /// Create a payment service
    /// </summary>
    /// <param name="contact">Name of a banking contact</param>
    /// <returns><see cref="IPaymentService"/> instance</returns>
    IPaymentService CreateInstance(IPaymentContact contact);

    /// <summary>
    /// FINTS-Registriernummer as product name since 1.8.2019
    /// </summary>
    string ProductName { get; set; } 

    /// <summary>
    /// ProductVersion (Länge 5 Zeichen)
    /// </summary>
    string ProductVersion { get; set; } 
}