// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using BodoMoneyTransfer.Dal.Interfaces;
using BodoMoneyTransfer.Dal.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Bodoconsult.Finance.Payment.Banking.Enums;
using Bodoconsult.Finance.Payment.Base.Models;

namespace BodoMoneyTransfer.Dal.Implementations;

/// <summary>
/// Dummy data handler (without real database connection)
/// </summary>
public class DummyDataHandler: IDataHandler
{
    private readonly IList<WorkloadTask> _workload;
    private readonly IList<Payment> _payments;
    private IOptions _options;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="options"></param>
    /// <param name="workload"></param>
    /// <param name="payments"></param>
    public DummyDataHandler(IOptions options, IList<WorkloadTask> workload, IList<Payment> payments)
    {
        _options = options;
        _workload = workload;
        _payments = payments;
    }

    /// <summary>
    /// Was <see cref="SetTransactionStatus"/> fired?
    /// </summary>
    public bool WasSetTransactionStatusFired { get; set; }

    /// <summary>
    /// Was <see cref="HandleVpoResult"/> fired?
    /// </summary>
    public bool WasHandleVpoResultFired { get; set; }

    /// <summary>
    /// Was <see cref="DeleteTransaction"/> fired?
    /// </summary>
    public bool WasDeleteTransactionFired { get; set; }

    /// <summary>
    /// Was <see cref="MoveTransactionToArchive"/> fired?
    /// </summary>
    public bool WasMoveTransactionToArchiveFired { get; set; }

    /// <summary>
    /// Execute a non data returning SqlCommand against the database
    /// </summary>
    /// <param name="cmd">Command to execute</param>
    public void Exec(SqlCommand cmd)
    {
        // Do nothing
    }

    /// <summary>
    /// Execute a SQL statement
    /// </summary>
    /// <param name="sql">SQL statement to execute</param>
    public void Exec(string sql)
    {
        // Do nothing;
    }

    /// <summary>
    /// Execute a  and return a string value delivered by the statement
    /// </summary>
    /// <param name="sql">SQL statement to execute</param>
    /// <returns>Result string</returns>
    public string ExecWithResult(string sql)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get all jobs
    /// </summary>
    /// <returns>List with all job tasks</returns>
    public IList<JobTask> GetJobTasks()
    {
        return new List<JobTask>();
    }

    /// <summary>
    /// Get all open paymnts for an account
    /// </summary>
    /// <param name="account">Account number</param>
    /// <param name="iban">IBAN of the account</param>
    /// <param name="type">type (U = Überweisung)</param>
    /// <param name="state">Current transaction state requested</param>
    /// <returns>List with open payments</returns>
    public IList<Payment> GetPaymentsForAnAccount(string account, string iban, char type, TransactionState state)
    {
        return _payments;
    }

    ///// <summary>
    ///// Get all payment tasks
    ///// </summary>
    ///// <returns>List with all PaymentTask objects with open transaction</returns>
    //public IList<PaymentTask> GetPaymentTasks()
    //{
    //    throw new NotImplementedException();
    //}

    /// <summary>
    /// Get a setting from database
    /// </summary>
    /// <param name="key">Key value of the setting</param>
    /// <returns>Value of the setting</returns>
    public string GetSetting(string key)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get the current workload 
    /// </summary>
    /// <param name="sql"></param>
    /// <returns>List with current workload data</returns>
    public IList<WorkloadTask> GetWorkload(string sql)
    {
        return _workload;
    }

    /// <summary>
    /// Move a payment transaction to the archive
    /// </summary>
    /// <param name="tGuid">Guid of the transaction</param>
    public void MoveTransactionToArchive(Guid tGuid)
    {
        // Do nothing
        WasMoveTransactionToArchiveFired = true;
    }

    /// <summary>
    /// Reset the balance in the database to zero
    /// </summary>
    public void ResetBalance()
    {
        // Do nothing
    }

    /// <summary>
    /// Create BodoMoneyTransfer-Transfer for an orginal account statement 
    /// </summary>
    /// <param name="id">Account ID</param>
    /// <param name="afFileName">filename of the accoutn statement</param>
    /// <param name="text">clear text to provide for the mail body</param>
    public void TransferOriginalAccountStatement(string id, string afFileName, string text)
    {
        // Do nothing
    }

    /// <summary>
    /// Update the balance for an account
    /// </summary>
    /// <param name="id">ID in the foreign database</param>
    /// <param name="sql">SQL to run against the foreign database</param>
    /// <param name="asId">Account statement ID in database BodoMoneyTransfer</param>
    /// <param name="balance">current balance amount</param>
    public void UpdateBalance(string id, string sql, string asId, decimal balance)
    {
        // Do nothing
    }

    /// <summary>
    /// Update the owner data for an account
    /// </summary>
    /// <param name="id">Account statement ID in database BodoMoneyTransfer</param>
    /// <param name="owner">Account owner name from the bank</param>
    public void UpdateOwner(string id, string owner)
    {
        // Do nothing
    }

    /// <summary>
    /// Send an error mail to the admin
    /// </summary>
    /// <param name="subject">Subject of the mail</param>
    /// <param name="body">Mail body</param>
    public void SendMail(string subject, string body)
    {
        Debug.Print($"Mail: {subject}: {body}");
    }

    /// <summary>
    /// Get the contact names for the workload
    /// </summary>
    /// <returns>List with contact names</returns>
    public IList<string> GetContactsForWorkload()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Set the status of a money transfer transcation for VOP
    /// </summary>
    /// <param name="transactionId">UID of the transaction</param>
    /// <param name="state">Status to set</param>
    public void SetTransactionStatus(Guid transactionId, TransactionState state)
    {
        // Do nothing
        WasSetTransactionStatusFired = true;
    }

    /// <summary>
    /// Handle the VOP result for a close match
    /// </summary>
    /// <param name="sql">SQL to execute</param>
    /// <param name="iban">IBAN of the payee</param>
    /// <param name="creditorName">Name of the payee delivered by the bank</param>
    public void HandleVpoResult(string sql, string iban, string creditorName)
    {
        // Do nothing
        WasHandleVpoResultFired = true;
    }

    /// <summary>
    /// Delete a payment transaction
    /// </summary>
    /// <param name="transactionId">UID of the transaction</param>
    public void DeleteTransaction(Guid transactionId)
    {
        // Do nothing
        WasDeleteTransactionFired = true;
    }

    /// <summary>
    /// Create an account statement row
    /// </summary>
    /// <param name="h">Account statement line</param>
    /// <param name="workload">Current workload</param>
    public void CreateAccountStatementRow(AccountStatementLine h, WorkloadTask workload)
    {
        // Do nothing
    }
}