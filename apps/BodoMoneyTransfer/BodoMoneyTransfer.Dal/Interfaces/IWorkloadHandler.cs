// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using Bodoconsult.Finance.Payment.Base.Interfaces;
using Bodoconsult.Finance.Payment.Base.Models;
using BodoMoneyTransfer.Dal.Implementations;

namespace BodoMoneyTransfer.Dal.Interfaces;

/// <inheritdoc />
/// <summary>
/// Interface for workload handler handling a single banking workload
/// </summary>
public interface IWorkloadHandler : IDisposable
{
    /// <summary>
    /// Payment service used for payment transactions
    /// </summary>
    IPaymentService PaymentService { get; }

    /// <summary>
    /// Archive the account statements for a certain time period (normally last month)
    /// </summary>
    /// <param name="dv">Date from</param>
    /// <param name="db">Date until</param>
    string ArchiveAccountStatementsAsMt940(DateTime dv, DateTime db);

    /// <summary>
    /// Archive the original PDFs from the bank
    /// </summary>
    void ArchiveOriginalAccountStatementPdfs();

    /// <summary>
    /// Get live account statement
    /// </summary>
    /// <param name="dv">date from</param>
    /// <param name="db">date until</param>
    int GetAccountStatements(DateTime dv, DateTime db);

    /// <summary>
    /// Get the balance for the account
    /// </summary>
    decimal GetBalance();

    /// <summary>
    /// Get the owner for the account
    /// </summary>
    void GetOwner();

    /// <summary>
    /// Add all safekeeping items to <see cref="PaymentHandler"/>
    /// </summary>
    void GetSafeKeepingItems();

    /// <summary>
    /// Run money transfer (currently only SEPA-Überweisungen with VOP or VOPOptOut)
    /// </summary>
    void RunMoneyTransfers();

    /// <summary>
    /// All safekeeping items
    /// </summary>
    IList<SafekeepingItem> SafekeepingItems { get; }

    /// <summary>
    /// Check waiting VOP money transfers
    /// </summary>
    void CheckVopMoneyTransfers();
}