// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.Database.Interfaces;
using Bodoconsult.Finance.Payment.Banking.Enums;
using Bodoconsult.Finance.Payment.Base.Models;
using BodoMoneyTransfer.Dal.Helper;
using BodoMoneyTransfer.Dal.Interfaces;
using BodoMoneyTransfer.Dal.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace BodoMoneyTransfer.Dal.Implementations;

/// <summary>
/// Current implementation of <see cref="IDataHandler"/>
/// </summary>
public class DataHandler : IDataHandler
{
    private readonly IAppLoggerProxy _logger;
    private readonly IConnManager _db;
    private readonly string _errorMailer;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appGlobals">Current app logger</param>
    /// <param name="db">Current DB connection manager</param>
    public DataHandler(IAppGlobals appGlobals, IConnManager db)
    {
        _db = db;
        _logger = appGlobals.Logger;
        _errorMailer = appGlobals.AppStartParameter.ErrorMailAddress;
    }

    /// <summary>
    /// Get all jobs
    /// </summary>
    /// <returns>List with all job tasks</returns>
    public IList<JobTask> GetJobTasks()
    {
        var erg = new List<JobTask>();

        var dt = _db.GetDataTable("EXEC dbo.spJobCollector_Start");
        foreach (DataRow r in dt.Rows)
        {
            var jt = new JobTask { Sql = r["JC_SQL"].ToString() };
            erg.Add(jt);
        }

        return erg;
    }

    /// <summary>
    /// Get a setting from database
    /// </summary>
    /// <param name="key">Key value of the setting</param>
    /// <returns>Value of the setting</returns>
    public string GetSetting(string key)
    {
        return _db.ExecWithResult($"SELECT [Value] FROm dbo.Settings WHERE sKey='{key}'");
    }

    /// <summary>
    /// Execute a SQL statement
    /// </summary>
    /// <param name="sql">SQL statement to execute</param>
    public void Exec(string sql)
    {
        _db.Exec(sql);
    }

    ///// <summary>
    ///// Get all payment tasks
    ///// </summary>
    ///// <returns>List with all PaymentTask objects with open transaction</returns>
    //public IList<PaymentTask> GetPaymentTasks()
    //{

    //    var erg = new List<PaymentTask>();

    //    var lo = new SqlCommand
    //    {
    //        CommandType = CommandType.StoredProcedure,
    //        CommandText = "[dbo].[spTransactions_Contacts]"
    //    };

    //    var dt = _db.GetDataTable(lo);

    //    foreach (DataRow row in dt.Rows)
    //    {

    //        var pt = new PaymentTask()
    //        {
    //            Password = PasswordHandler.Decrypt(row["A_Passphrase"].ToString()),
    //            Contact = row["A_Contact"].ToString(),
    //            Iban = row["A_MyIban"].ToString(),
    //            //Bic = row["A_MyBic"].ToString(),
    //            AccountName = row["A_MyName"].ToString()
    //        };

    //        erg.Add(pt);
    //    }


    //    return erg;
    //}


    /// <summary>
    /// Execute a  and return a string value delivered by the statement
    /// </summary>
    /// <param name="sql">SQL statement to execute</param>
    /// <returns>Result string</returns>
    public string ExecWithResult(string sql)
    {
        return _db.ExecWithResult(sql);
    }

    /// <summary>
    /// Move a payment transaction to the archive
    /// </summary>
    /// <param name="tGuid">Guid of the transaction</param>
    public void MoveTransactionToArchive(Guid tGuid)
    {
        var lo = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = "[dbo].[spTransactions_MoveToArchive_Sepa]"
        };
        lo.Parameters.Add("@A_ID", SqlDbType.UniqueIdentifier);
        lo.Parameters[0].Value = tGuid;
        _db.Exec(lo);
    }

    /// <summary>
    /// Execute a non data returning SqlCommand against the database
    /// </summary>
    /// <param name="cmd">Command to execute</param>
    public void Exec(SqlCommand cmd)
    {
        _db.Exec(cmd);
    }

    /// <summary>
    /// Get all open paymnts for an account
    /// </summary>
    /// <param name="account">Account number</param>
    /// <param name="iban">IBAN of the account</param>
    /// <param name="type">type (U = Überweisung)</param>
    /// <param name="state">Current transaction state requested: Default: 0</param>
    /// <returns>List with open payments</returns>
    public IList<Payment> GetPaymentsForAnAccount(string account, string iban, char type, TransactionState state)
    {
        var erg = new List<Payment>();

        _logger.LogInformation($"{account} {iban}");

        var lo = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = "dbo.spTransactions_ContactCurrentTransactions"
        };
        lo.Parameters.Add("@HBCI_Contact", SqlDbType.VarChar);
        lo.Parameters[0].Value = account;
        lo.Parameters.Add("@MyIban", SqlDbType.VarChar);
        lo.Parameters[1].Value = iban;
        lo.Parameters.Add("@Typ", SqlDbType.Char);
        lo.Parameters[2].Value = type;
        lo.Parameters.Add("@State", SqlDbType.TinyInt);
        lo.Parameters[3].Value = state;

        List<DataRow> dt;

        try
        {
            dt = _db.GetDataTable(lo).AsEnumerable().ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            dt = null;
        }

        if (dt == null)
        {
            return erg;
        }

        foreach (var r in dt)
        {
            try
            {
                var p = new Payment
                {
                    PayerOrPayee = r["A_HisName"].ToString(),
                    Iban = r["A_HisIban"].ToString()?.ToUpperInvariant(),
                    Bic = r["A_HisBic"].ToString()?.ToUpperInvariant(),
                    Description = r["A_Purpose"].ToString(),
                    Total = Convert.ToDecimal(r["A_Amount"].ToString()),
                    Currency = "EUR",
                    Guid = new Guid(r["A_ID"].ToString() ?? Guid.NewGuid().ToString()),
                    ExecOnSuccess = r["A_SQL_Success"].ToString(),
                    ExecOnFailure = r["A_SQL_Error"].ToString(),
                    ExecOnVpoCloseMatch = r["A_SQL_VopCheck"].ToString(),
                    TransactionState = (TransactionState)Convert.ToInt32(r["A_Status"].ToString())
                };

                erg.Add(p);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        return erg;
    }

    /// <summary>
    /// Get the current workload 
    /// </summary>
    /// <param name="sql">SQL providing all workloads</param>
    /// <returns>List with current workload data</returns>
    public IList<WorkloadTask> GetWorkload(string sql)
    {
        var erg = new List<WorkloadTask>();

        var dt = _db.GetDataTable(sql);
        foreach (DataRow r in dt.Rows)
        {
            var a = new WorkloadTask
            {
                Id = r["AS_ID"].ToString(),
                Contact = r["AS_Contact"].ToString(),
                Password = r["AS_Passphrase"].ToString(),
                Provider = ProviderHelper.MapToBankingServiceProvider(r["AS_Type"].ToString().ToUpper()),
                Signature = r["AS_Signature"].ToString(),
                SqlStatementRows = r["AS_SQL"].ToString(),
                SqlBalance = r["AS_SQLBalance"].ToString(),
                AccountNr = r["AS_AccountNr"].ToString().Trim(),
                BankCode = r["AS_BankCode"].ToString().Trim(),
                Iban = r["AS_IBAN"].ToString(),
                BftAccountId = Convert.ToInt32(r["AS_BftAccountId"].ToString()),
                Owner = r["AS_Owner"].ToString(),

                AccountStatementsLive = Convert.ToBoolean(r["AS_LiveData"].ToString()),
                AccountStatementsHistoricalData = Convert.ToBoolean(r["AS_HistoricalData"].ToString()),
                AccountStatementsOfficialHistoricalData = Convert.ToBoolean(r["AS_OfficialHistoricalData"].ToString()),
                SafeKeepingItems = Convert.ToBoolean(r["AS_SafeKeepings"].ToString()),
                RunTransfers = Convert.ToBoolean(r["AS_RunTransfers"].ToString()),
                VopOptOut = Convert.ToBoolean(r["AS_VopOptOut"].ToString()),
            };

            erg.Add(a);
        }

        return erg;
    }

    /// <summary>
    /// Update the owner data for an account
    /// </summary>
    /// <param name="id">Account statement ID in database BodoMoneyTransfer</param>
    /// <param name="owner">Account owner name from the bank</param>
    public void UpdateOwner(string id, string owner)
    {
        var olo = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = "dbo.spAccountStatements_UpdateOwner"
        };
        olo.Parameters.Add("@Owner", SqlDbType.VarChar);
        olo.Parameters[0].Value = owner;
        olo.Parameters.Add("@ASID", SqlDbType.VarChar);
        olo.Parameters[1].Value = id;
        _db.Exec(olo);
    }

    /// <summary>
    /// Reset the balance in the database to zero
    /// </summary>
    public void ResetBalance()
    {
        _db.Exec("EXEC dbo.spAccountStatements_ResetBalance");
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
        SqlCommand lo;

        if (!string.IsNullOrEmpty(sql))
        {
            try
            {
                lo = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = sql
                };
                lo.Parameters.Add("@Value", SqlDbType.Float);
                lo.Parameters[0].Value = balance;
                lo.Parameters.Add("@OGUID", SqlDbType.VarChar);
                lo.Parameters[1].Value = id;
                _db.Exec(lo);
            }
            catch (Exception e)
            {
                _logger.LogError($"Update balance not possible: {sql}", e);
            }
        }

        lo = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = "spAccountStatements_UpdateBalance"
        };
        lo.Parameters.Add("@Balance", SqlDbType.Money);
        lo.Parameters[0].Value = balance;
        lo.Parameters.Add("@AS_ID", SqlDbType.UniqueIdentifier);
        lo.Parameters[1].Value = new Guid(asId);
        _db.Exec(lo);
    }

    /// <summary>
    /// Create BodoMoneyTransfer-Transfer for an orginal account statement 
    /// </summary>
    /// <param name="id">Account ID</param>
    /// <param name="afFileName">filename of the accoutn statement</param>
    /// <param name="text">clear text to provide for the mail body</param>
    public void TransferOriginalAccountStatement(string id, string afFileName, string text)
    {
        var sql = $"EXEC dbo.[spBFT_TransferOriginalAccountStatement] '{id}', '{afFileName}', '{text}'";
        _db.Exec(sql);
    }

    /// <summary>
    /// Send an error mail to the admin
    /// </summary>
    /// <param name="subject">Subject of the mail</param>
    /// <param name="body">Mail body</param>
    public void SendMail(string subject, string body)
    {
        _logger.LogError($"{subject}:{body}");

        var lo = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = "dbo.[spMAIL_New]"
        };
        lo.Parameters.Add("@To", SqlDbType.VarChar);
        lo.Parameters[0].Value = _errorMailer;
        lo.Parameters.Add("@Subject", SqlDbType.VarChar);
        lo.Parameters[1].Value = $"BodoMoneyTransfer:PaymentHandler:{subject}";
        lo.Parameters.Add("@Msg", SqlDbType.VarChar);
        lo.Parameters[2].Value = body;

#if DEBUG
        Debug.Print($"{subject}:{body}");
#else
            _db.Exec(lo);
#endif

    }

    /// <summary>
    /// Get the contact names for the workload
    /// </summary>
    /// <returns>List with contact names</returns>
    public IList<string> GetContactsForWorkload()
    {
        var erg = new List<string>();
        
        var dt = _db.GetDataTable("EXEC dbo.spContacts_GetContactsForWorkload");
        foreach (DataRow r in dt.Rows)
        {

            erg.Add(r["AS_Contact"].ToString());
        }

        return erg;
    }

    /// <summary>
    /// Set the status of a money transfer transcation for VOP
    /// </summary>
    /// <param name="transactionId">UID of the transaction</param>
    /// <param name="state">Status to set</param>
    public void SetTransactionStatus(Guid transactionId, TransactionState state)
    {
        var lo = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = "[dbo].[spTransactions_SetStatus]"
        };
        lo.Parameters.Add("@A_ID", SqlDbType.UniqueIdentifier);
        lo.Parameters[0].Value = transactionId;
        lo.Parameters.Add("@Status", SqlDbType.TinyInt);
        lo.Parameters[1].Value = state;
        _db.Exec(lo);
    }

    /// <summary>
    /// Handle the VOP result for a close match
    /// </summary>
    /// <param name="sql">SQL to execute</param>
    /// <param name="iban">IBAN of the payee</param>
    /// <param name="creditorName">Name of the payee delivered by the bank</param>
    public void HandleVpoResult(string sql, string iban, string creditorName)
    {
        var lo = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = sql
        };
        lo.Parameters.Add("@IBAN", SqlDbType.VarChar);
        lo.Parameters[0].Value = iban;
        lo.Parameters.Add("@HisName", SqlDbType.VarChar);
        lo.Parameters[1].Value = creditorName;
        _db.Exec(lo);
    }

    /// <summary>
    /// Delete a payment transaction
    /// </summary>
    /// <param name="transactionId">UID of the transaction</param>
    public void DeleteTransaction(Guid transactionId)
    {
        var lo = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = "dbo.spTransactions_Delete"
        };
        lo.Parameters.Add("@A_ID", SqlDbType.UniqueIdentifier);
        lo.Parameters[0].Value = transactionId;
        _db.Exec(lo);
    }

    /// <summary>
    /// Create an account statement row
    /// </summary>
    /// <param name="h">Account statement line</param>
    /// <param name="workload">Current workload</param>
    public void CreateAccountStatementRow(AccountStatementLine h, WorkloadTask workload)
    {
        var lo = new SqlCommand
        {
            CommandType = CommandType.StoredProcedure,
            CommandText = workload.SqlStatementRows
        };
        lo.Parameters.Add("@AccountNr", SqlDbType.VarChar);
        lo.Parameters[0].Value = workload.AccountNr;
        lo.Parameters.Add("@BankCode", SqlDbType.VarChar);
        lo.Parameters[1].Value = workload.BankCode;
        lo.Parameters.Add("@ValidityDate", SqlDbType.VarChar);
        lo.Parameters[2].Value = h.ValidityDate;
        lo.Parameters.Add("@EntryDate", SqlDbType.VarChar);
        lo.Parameters[3].Value = h.EntryDate;
        lo.Parameters.Add("@Sender", SqlDbType.VarChar);
        lo.Parameters[4].IsNullable = true;

        var sender = string.IsNullOrEmpty($"{h.Name}".Trim()) ? "Bank" : $"{h.Name}";

        lo.Parameters[4].Value = sender;
        lo.Parameters.Add("@Amount", SqlDbType.Float);
        lo.Parameters[5].Value = h.Amount;
        lo.Parameters.Add("@Text", SqlDbType.VarChar);
        lo.Parameters[6].IsNullable = true;
        lo.Parameters[6].Value = $"{CleanString(h.BusinessTransactionText)} {CleanString(h.Purpose)}".Trim();
        lo.Parameters.Add("@FeeAmount", SqlDbType.Float);
        lo.Parameters[7].Value = h.FeeAmount;
        lo.Parameters.Add("@OGUID", SqlDbType.VarChar);
        lo.Parameters[8].Value = workload.Id;
        Exec(lo);
    }

    private static string CleanString(string value)
    {
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return value.Trim();
    }
}