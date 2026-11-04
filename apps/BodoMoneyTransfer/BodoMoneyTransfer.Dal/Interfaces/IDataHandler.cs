// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.Finance.Payment.Banking.Enums;
using Bodoconsult.Finance.Payment.Base.Models;
using BodoMoneyTransfer.Dal.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace BodoMoneyTransfer.Dal.Interfaces;

/// <summary>
/// Interface for data handlers for connecting database
/// </summary>
public interface IDataHandler
{
    /// <summary>
    /// Execute a non data returning SqlCommand against the database
    /// </summary>
    /// <param name="cmd">Command to execute</param>
    void Exec(SqlCommand cmd);

    /// <summary>
    /// Execute a SQL statement
    /// </summary>
    /// <param name="sql">SQL statement to execute</param>
    void Exec(string sql);

    /// <summary>
    /// Execute a  and return a string value delivered by the statement
    /// </summary>
    /// <param name="sql">SQL statement to execute</param>
    /// <returns>Result string</returns>
    string ExecWithResult(string sql);

    /// <summary>
    /// Get all jobs
    /// </summary>
    /// <returns>List with all job tasks</returns>
    IList<JobTask> GetJobTasks();

    /// <summary>
    /// Get all open paymnts for an account
    /// </summary>
    /// <param name="account">Account number</param>
    /// <param name="iban">IBAN of the account</param>
    /// <param name="type">type (U = Überweisung)</param>
    /// <param name="state">Current transaction state requested</param>
    /// <returns>List with open payments</returns>
    IList<Payment> GetPaymentsForAnAccount(string account, string iban, char type, TransactionState state);


    ///// <summary>
    ///// Get all payment tasks
    ///// </summary>
    ///// <returns>List with all PaymentTask objects with open transaction</returns>
    //IList<PaymentTask> GetPaymentTasks();

    /// <summary>
    /// Get a setting from database
    /// </summary>
    /// <param name="key">Key value of the setting</param>
    /// <returns>Value of the setting</returns>
    string GetSetting(string key);

    /// <summary>
    /// Get the current workload 
    /// </summary>
    /// <param name="sql">SQL providing all workloads</param>
    /// <returns>List with current workload data</returns>
    IList<WorkloadTask> GetWorkload(string sql);

    /// <summary>
    /// Move a payment transaction to the archive
    /// </summary>
    /// <param name="tGuid">Guid of the transaction</param>
    void MoveTransactionToArchive(Guid tGuid);

    /// <summary>
    /// Reset the balance in the database to zero
    /// </summary>
    void ResetBalance();

    /// <summary>
    /// Create BodoMoneyTransfer-Transfer for an orginal account statement 
    /// </summary>
    /// <param name="id">Account ID</param>
    /// <param name="afFileName">filename of the accoutn statement</param>
    /// <param name="text">clear text to provide for the mail body</param>
    void TransferOriginalAccountStatement(string id, string afFileName, string text);

    /// <summary>
    /// Update the balance for an account
    /// </summary>
    /// <param name="id">ID in the foreign database</param>
    /// <param name="sql">SQL to run against the foreign database</param>
    /// <param name="asId">Account statement ID in database BodoMoneyTransfer</param>
    /// <param name="balance">current balance amount</param>
    void UpdateBalance(string id, string sql, string asId, decimal balance);

    /// <summary>
    /// Update the owner data for an account
    /// </summary>
    /// <param name="id">Account statement ID in database BodoMoneyTransfer</param>
    /// <param name="owner">Account owner name from the bank</param>
    void UpdateOwner(string id, string owner);

    /// <summary>
    /// Send an error mail to the admin
    /// </summary>
    /// <param name="subject">Subject of the mail</param>
    /// <param name="body">Mail body</param>
    void SendMail(string subject, string body);

    /// <summary>
    /// Get the contact names for the workload
    /// </summary>
    /// <returns>List with contact names</returns>
    IList<string> GetContactsForWorkload();

    /// <summary>
    /// Set the status of a money transfer transcation for VOP
    /// </summary>
    /// <param name="transactionId">UID of the transaction</param>
    /// <param name="state">Status to set</param>
    void SetTransactionStatus(Guid transactionId, TransactionState state);

    /// <summary>
    /// Handle the VOP result for a close match
    /// </summary>
    /// <param name="sql">SQL to execute</param>
    /// <param name="iban">IBAN of the payee</param>
    /// <param name="creditorName">Name of the payee delivered by the bank</param>
    void HandleVpoResult(string sql, string iban, string creditorName);

    /// <summary>
    /// Delete a payment transaction
    /// </summary>
    /// <param name="transactionId">UID of the transaction</param>
    void DeleteTransaction(Guid transactionId);

    /// <summary>
    /// Create an account statement row
    /// </summary>
    /// <param name="h">Account statement line</param>
    /// <param name="workload">Current workload</param>
    void CreateAccountStatementRow(AccountStatementLine h, WorkloadTask workload);
}