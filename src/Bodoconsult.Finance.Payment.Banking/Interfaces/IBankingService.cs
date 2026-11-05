// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.Finance.Payment.Banking.Ebics;
using Bodoconsult.Finance.Payment.Banking.Hbci;
using Bodoconsult.Finance.Payment.Banking.Models;
using Bodoconsult.Finance.Payment.Base.Interfaces;
using Bodoconsult.Finance.Payment.Base.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bodoconsult.Finance.Payment.Banking.Interfaces;

/// <summary>
/// Interface for banking services
/// </summary>
public interface IBankingService: IPaymentService
{
    /// <summary>
    /// FINTS-Registriernummer as product name since 1.8.2019
    /// </summary>
    string ProductName { get; }

    /// <summary>
    /// Product version (max. length 5 chars)
    /// </summary>
    [MaxLength(5)]
    string ProductVersion { get; }

    /// <summary>
    /// Load an MT940 data file and create a PDF account statement for it
    /// </summary>
    /// <param name="targetFile">MT940 file</param>
    /// <param name="header"></param>
    /// <returns></returns>
    bool LoadMt940FileAsCreateAccountStatement(string targetFile, AccountStatementHeader header);

    /// <summary>
    /// Load an MT940 data file and create a PDF account statement for it
    /// </summary>
    /// <param name="targetFile">MT940 file</param>
    /// <returns></returns>
    bool LoadMt940FileAsCreateAccountStatement(string targetFile);

    /// <summary>
    /// Get all banking contacts on the current system for the current user
    /// </summary>
    /// <returns></returns>
    IList<string> GetBankingContacts();

    /// <summary>
    /// Get all banking contacts on the current system for the current user
    /// </summary>
    /// <returns></returns>
    IList<BankingAccount> GetBankingAccounts(string contactName, string password);

    /// <summary>
    /// Open the administration window for HBCI contacts
    /// </summary>
    void ShowAdministrationWindow();

    /// <summary>
    /// Open the new HBCI dialog
    /// </summary>
    void ShowNewContactWindow();

    /// <summary>
    /// Get the all balances of the contact
    /// </summary>
    /// <returns>balance amount</returns>
    IList<AccountBalanceData> GetContactBalances();

    /// <summary>
    /// Download Auftragsart VPZ (VOP). Get Orderid, State and CreditorName
    /// </summary>
    /// <param name="result">List with VOP processing relevante data</param>
    /// <returns>True on success else false</returns>
    public bool DownloadVpz(List<VopProcessingData> result);

    /// <summary>
    /// Download Auftragsart HVD (VEU)
    /// </summary>
    /// <param name="data">Order data</param>
    /// <returns>True on success else false</returns>
    public bool DownloadHvd(VopProcessingData data);

    /// <summary>
    /// Download Auftragsart HVU (VEU)
    /// </summary>
    /// <param name="result">List of order data</param>
    /// <returns>True on success else false</returns>
    public bool DownloadHvu(List<VopProcessingData> result);

    /// <summary>
    /// Send HVE to bank to confirm money transfer
    /// </summary>
    /// <param name="data">Order data</param>
    /// <returns>True on success else false</returns>
    public bool UploadHve(VopProcessingData data);

    /// <summary>
    /// Send HVS to bank to cancel (Storno) money transfer
    /// </summary>
    /// <param name="data">Order data</param>
    /// <returns>True on success else false</returns>
    public bool UploadHvs(VopProcessingData data);
}