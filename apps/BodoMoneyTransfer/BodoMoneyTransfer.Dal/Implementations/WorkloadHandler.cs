// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.Finance.Payment.Banking.Enums;
using Bodoconsult.Finance.Payment.Banking.Interfaces;
using Bodoconsult.Finance.Payment.Banking.Models;
using Bodoconsult.Finance.Payment.Base.Helpers;
using Bodoconsult.Finance.Payment.Base.Interfaces;
using Bodoconsult.Finance.Payment.Base.Models;
using BodoMoneyTransfer.Dal.Interfaces;
using BodoMoneyTransfer.Dal.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using Bodoconsult.Finance.Payment.Base;

namespace BodoMoneyTransfer.Dal.Implementations;

/// <summary>
/// Handles a single banking workload
/// </summary>
[SupportedOSPlatform("windows")]
public class WorkloadHandler : IWorkloadHandler
{
    private readonly IDataHandler _dataHandler;
    private readonly WorkloadTask _workload;
    private readonly IAppLoggerProxy _logger;
    private readonly IBodoMoneyTransferAppGlobals _appGlobals;
    private readonly ShowPaymentMessageDelegate _setImportantMessageDelegate;
    
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataHandler">Database connection handler</param>
    /// <param name="paymentService">Payment service used for payment transactions</param>
    /// <param name="workload">Current workload</param>
    /// <param name="appGlobals">Current options</param>
    public WorkloadHandler(IDataHandler dataHandler, IPaymentService paymentService, WorkloadTask workload, IAppGlobals appGlobals)
    {
        _dataHandler = dataHandler;
        PaymentService = paymentService;
        _workload = workload;

        if (appGlobals is not IBodoMoneyTransferAppGlobals globals)
        {
            throw new ArgumentException("AppGlobals is not IBodoMoneyTransferAppGlobals");
        }

        ArgumentNullException.ThrowIfNull(globals.SetImportantMessageDelegate);
        _setImportantMessageDelegate = globals.SetImportantMessageDelegate;

        _appGlobals = globals;
        _logger = globals.Logger;

        //Logger.Info("Hallo");
    }

    /// <summary>
    /// Payment service used for payment transactions
    /// </summary>
    public IPaymentService PaymentService { get; }

    /// <summary>
    /// All safekeeping items
    /// </summary>
    public IList<SafekeepingItem> SafekeepingItems { get; } = new List<SafekeepingItem>();

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        PaymentService.Dispose();
    }

    /// <summary>
    /// Check waiting VOP money transfers
    /// </summary>
    public void CheckVopMoneyTransfers()
    {
        if (PaymentService is not IBankingService bankingService)
        {
            return;
        }

        var contact = (IBankingContact)PaymentService.PaymentContact;

        var payments = _dataHandler.GetPaymentsForAnAccount(contact.Name, contact.Iban, 'U', TransactionState.DeliveredToBank).ToList();

        var voph = new VopHandler(payments, bankingService, _dataHandler);
        voph.DownloadVpz();
        voph.CheckStateForPayments();
        voph.PrepareAnswerToBank();
        voph.SendAllAnswersToBank();
        voph.HandleAllVopResultsOnOurSide();
    }

    /// <summary>
    /// Run money transfer (currently only SEPA-Überweisungen)
    /// </summary>
    public void RunMoneyTransfers()
    {
        if (!_workload.RunTransfers)
        {
            // SetStatus($"     -> MONEY TRANSFER {_workload.BankCode} / {_workload.AccountNr} NOT REQUIRED");
            return;
        }

        SetStatus($"     -> MONEY TRANSFER {_workload.BankCode} / {_workload.AccountNr}");

        try
        {
            if (GetTransactions(out var ta))
            {
                return;
            }

            // VOP process
            if (_workload.VopOptOut)
            {
                TransferVopOptOut(ta);
            }
            else
            {
                throw new NotImplementedException();
                //TransferVop(ta);
            }
        }
        catch (Exception ex)
        {
            //var msg = $"Contact: {_workload.Contact} IBAN: {_workload.Iban}: ";
            //Logger.Error(msg, ex);
            //_dataHandler.SendMail("RunBankingTransfersForAnAccount:MainError", msg + ex.Message);

            var pm = new PaymentMessage
            {
                ErrorCode = 9901,
                Message = $"RunBankingTransfersForAnAccount:MainError:{_workload.Contact} // {_workload.Iban}:Main error: {ex.Message}"
            };

            _logger.LogError(pm.Message);
            _setImportantMessageDelegate.Invoke(pm);
        }
    }

    /// <summary>
    ///  Use VOP opt-out option for money transfer
    /// </summary>
    /// <param name="ta">List of transactions</param>
    /// <exception cref="Exception">HBCI exceptions</exception>
    private void TransferVopOptOut(List<BankingOrder> ta)
    {
        var erg = PaymentService.TransferVopOutOut(ta);

        if (erg.ErrorCode is < 0 or >= 4000)
        {
            throw new Exception(string.Format("HbciError:ErrorCode{1}/{2}:{0}", erg.Message, erg.ErrorCode, erg.ErrorCode1));
        }

        foreach (var t in ta)
        {
            var message = t.Result;
            // erfolgreich
            if (message.ErrorCode is >= 0 and < 4000)
            {
                var msg =
                    $"SEPA-TransferTransaction {t.Guid}: {message.ErrorCode1} // {message.ErrorCode} // {message.Message}";

                if (!string.IsNullOrEmpty(msg))
                {
                    _logger.LogDebug(msg);
                }

                _dataHandler.MoveTransactionToArchive(t.Guid);

                if (string.IsNullOrEmpty(t.ExecOnSuccess) == false)
                {
                    _dataHandler.Exec(t.ExecOnSuccess);
                }
            }
            else // mit Fehlern
            {
                var msg =
                    $"SEPA-TransferTransaction {t.Guid}: {message.ErrorCode1} // {message.ErrorCode} // {message.Message}";

                if (!string.IsNullOrEmpty(msg))
                {
                    var pm = new PaymentMessage
                    {
                        ErrorCode = 9900,
                        Message =
                            $"RunBankingTransfersForAnAccount:{PaymentService.PaymentContact.ToReadableString()}:Payment errors: {msg}"
                    };

                    _logger.LogError(pm.Message);
                    _setImportantMessageDelegate.Invoke(pm);
                }

                if (string.IsNullOrEmpty(t.ExecOnFailure) == false)
                {
                    _dataHandler.Exec(t.ExecOnFailure);
                }
            }
        }
    }

    /// <summary>
    ///  Use VOP for money transfer
    /// </summary>
    /// <param name="ta">List of transactions</param>
    /// <exception cref="Exception">HBCI exceptions</exception>
    private void TransferVop(List<BankingOrder> ta)
    {
        var erg = PaymentService.TransferVop(ta);

        if (erg.ErrorCode is < 0 or >= 4000)
        {
            throw new Exception(string.Format("HbciError:ErrorCode{1}/{2}:{0}", erg.Message, erg.ErrorCode, erg.ErrorCode1));
        }

        foreach (var t in ta)
        {
            var message = t.Result;
            // erfolgreich
            if (message.ErrorCode is >= 0 and < 4000)
            {
                var msg =
                    $"SEPA-TransferTransaction {t.Guid}: {message.ErrorCode1} // {message.ErrorCode} // {message.Message}";

                if (!string.IsNullOrEmpty(msg))
                {
                    _logger.LogDebug(msg);
                }

                _dataHandler.SetTransactionStatus(t.Guid, TransactionState.DeliveredToBank);
            }
            else // mit Fehlern
            {
                var msg =
                    $"SEPA-TransferTransaction {t.Guid}: {message.ErrorCode1} // {message.ErrorCode} // {message.Message}";

                if (!string.IsNullOrEmpty(msg))
                {
                    var pm = new PaymentMessage
                    {
                        ErrorCode = 9900,
                        Message =
                            $"RunBankingTransfersForAnAccount:{PaymentService.PaymentContact.ToReadableString()}:Payment errors: {msg}"
                    };

                    _logger.LogError(pm.Message);
                    _setImportantMessageDelegate.Invoke(pm);
                }

                if (string.IsNullOrEmpty(t.ExecOnFailure) == false)
                {
                    _dataHandler.Exec(t.ExecOnFailure);
                }
            }
        }
    }

    private bool GetTransactions(out List<BankingOrder> ta)
    {
        var contact = (IBankingContact)PaymentService.PaymentContact;

        var balance = GetBalance(contact); // = _workload.Balance;

        var dt = _dataHandler.GetPaymentsForAnAccount(contact.Name, contact.Iban, 'U', TransactionState.Unhandled);

        if (dt.Count <= 0)
        {
            ta = null;
            return true;
        }

        //var i = 0;

        var transactions = from ueberweisung in dt
                           select new BankingOrder
                           {
                               PayerOrPayee = ueberweisung.PayerOrPayee,
                               Iban = ueberweisung.Iban,
                               Bic = ueberweisung.Bic,
                               Description = ueberweisung.Description,
                               Total = ueberweisung.Total,
                               Currency = "EUR",
                               Guid = ueberweisung.Guid,
                               ExecOnSuccess = ueberweisung.ExecOnSuccess,
                               ExecOnFailure = ueberweisung.ExecOnFailure
                           };

        ta = [];
        AppendTransactions(transactions, ta, contact.Iban.ToUpper().Trim(), balance);

        return ta.Count == 0;
    }

    private decimal GetBalance(IBankingContact contact)
    {
        decimal balance;
        try
        {
            balance = Convert.ToDecimal(_dataHandler.ExecWithResult($"EXEC dbo.spAccountStatements_GetBalance '{contact.Iban}'"));
        }
        catch
        {
            balance = 0;
        }

        return balance;
    }

    /// <summary>
    /// Check and append transaction to the current batch
    /// </summary>
    /// <param name="iban">Current account's IBAN</param>
    /// <param name="sourceTransactions">Current transactions to check</param>
    /// <param name="targetTransactions">Remaining current transactions after checking</param>
    /// <param name="balance">Current account's balance</param>
    /// <returns></returns>
    internal string AppendTransactions(IEnumerable<BankingOrder> sourceTransactions, List<BankingOrder> targetTransactions, string iban, decimal balance)
    {

        var mail = new StringBuilder();
        foreach (var transaction in sourceTransactions)
        {
            // IBAN is not valid
            if (IbanValidator.Validate(transaction.Iban) != IbanValidationResult.IsValid)
            {
                mail.AppendLine($"{transaction.PayerOrPayee} ({transaction.Iban}): IBAN invalid: {transaction.Guid}");
                continue;
            }

            // IBAN sender equals IBAN receiver
            if (iban == transaction.Iban.ToUpper().Trim())
            {
                mail.AppendLine($"IBAN payer {iban} equals IBAN receiver {transaction.Iban}: Payer and receiver are the same: {transaction.Guid}");
                continue;
            }

            if (balance <= transaction.Total)
            {
                mail.AppendLine($"Balance too low: {balance}: Money transfer not possible: IBAN2 {transaction.Iban} Guid {transaction.Guid}");
                continue;
            }

            // Reduce balance only in case of transfer is possible
            balance -= transaction.Total;

            targetTransactions.Add(transaction);
            //i++;
        }

        // send error mails
        if (mail.Length==0)
        {
            return null;
        }

        //var subject = $"RunBankingTransfersForAnAccount:Payment errors:IBAN {iban}: ";
        //_dataHandler.SendMail(subject, mail);

        var s = mail.ToString();

        var pm = new PaymentMessage
        {
            ErrorCode = 9902,
            Message =
                $"RunBankingTransfersForAnAccount:{PaymentService.PaymentContact.ToReadableString()}:Payment errors:IBAN {iban}:{s}"
        };

        _logger.LogError(pm.Message);
        _setImportantMessageDelegate.Invoke(pm);

        return s;
    }

    /// <summary>
    /// Get live account statement
    /// </summary>
    /// <param name="dv">date from</param>
    /// <param name="db">date until</param>
    public int GetAccountStatements(DateTime dv, DateTime db)
    {

        if (!_workload.AccountStatementsLive)
        {
            return -1;
        }

        SetStatus($"     -> LIVE {_workload.BankCode} / {_workload.AccountNr}");

        PaymentService.OpenContact();

        IList<AccountStatementLine> erg = new List<AccountStatementLine>();
        //try
        //{
        erg = PaymentService.GetHistory(dv, db).Items;
        //}
        //catch (Exception ex2)
        //{
        //    if (!ex2.Message.ToLower().Contains("illegal padding size"))
        //    {

        //        var pm = new PaymentMessage
        //        {
        //            ErrorCode = 9903,
        //            Message =
        //                $"GetAccountStatements:{PaymentService.PaymentContact.ToReadableString()}:{ex2.Message}"
        //        };

        //        Logger.Error(pm.Message);
        //        Globals.SetImportantMessage(pm);

        //    }
        //}

        try
        {
            if (erg == null)
            {
                return -1;
            }

            SetStatus($"Got {erg.Count} Items");
            foreach (var h in erg)
            {
                _dataHandler.CreateAccountStatementRow(h, _workload);

                Debug.Print(h.Amount.ToString(CultureInfo.InvariantCulture));

                //Trace.TraceInformation(h.Amount.ToString()); 
            }

            return erg.Count;
        }
        catch (Exception ex)
        {
            var pm = new PaymentMessage
            {
                ErrorCode = 9904,
                Message =
                    $"GetAccountStatements:{PaymentService.PaymentContact.ToReadableString()}:{ex.Message}"
            };

            _logger.LogError(pm.Message);
            _setImportantMessageDelegate.Invoke(pm);
        }

        return -1;
    }

    /// <summary>
    /// Archive the account statements for a certain time period (normally last month)
    /// </summary>
    /// <param name="dv">Date from</param>
    /// <param name="db">Date until</param>
    public string ArchiveAccountStatementsAsMt940(DateTime dv, DateTime db)
    {
        if (!_workload.AccountStatementsHistoricalData)
        {
            return null;
        }

        try
        {
            SetStatus($"     -> MT940 {_workload.BankCode} / {_workload.AccountNr}");

            var path = Path.Combine(_appGlobals.ArchivePath, $"{_workload.BankCode}_{_workload.AccountNr}");


            //SetStatus($"     -> MT940 {path}");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fileName = Path.Combine(path, string.Format("{1}_Kontoauszug_{0}.sta", dv.Year * 100 + dv.Month, _workload.AccountNr));

            var pdf = fileName.Replace(".sta", ".pdf");

            //SetStatus($"     -> MT940 {fileName}");

            if (File.Exists(fileName) && File.Exists(pdf))
            {
                return fileName;
            }

            PaymentService.GetHistoryToMt940File(dv, db, fileName, true);

            SetStatus($"Saved MT940 data to {fileName}");

            return fileName;
        }
        catch (Exception e)
        {
            var pm = new PaymentMessage
            {
                ErrorCode = 9904,
                Message =
                    $"ArchiveAccountStatementsAsMt940:{PaymentService.PaymentContact.ToReadableString()}:{e.Message}"
            };

            _logger.LogError(pm.Message);
            _setImportantMessageDelegate.Invoke(pm);
            return null;
        }


    }

    /// <summary>
    /// Archive the original PDFs from the bank
    /// </summary>
    public void ArchiveOriginalAccountStatementPdfs()
    {

        if (!_workload.AccountStatementsOfficialHistoricalData)
        {
            return;
        }

        SetStatus($"     -> PDF {_workload.BankCode} / {_workload.AccountNr}");

        var account = _workload.AccountNr.Trim();
        var bc = _workload.BankCode.Trim();

        var path = Path.Combine(_appGlobals.OriginalArchivePath, $"{bc}_{account}");

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var results = PaymentService.GetOfficialAccountStatementFiles();

        SetStatus($"Saved {results.Count} PDF files to {path}");

        var sendReceipt = true;

#if DEBUG
        sendReceipt = false;
#endif

        foreach (var af in results)
        {
            var afFileName = Path.Combine(path, af.FileName);

            if (File.Exists(afFileName))
            {
                continue;
            }


            try
            {
                PaymentService.GetOfficialAccountStatementFile(af, sendReceipt, afFileName);
            }
            catch (Exception e)
            {

                var pm = new PaymentMessage
                {
                    ErrorCode = 9906,
                    Message =
                        $"GetArchiveOriginalAccountStatement:{PaymentService.PaymentContact.ToReadableString()}:{e.Message}"
                };

                _logger.LogError(pm.ToString());
                _setImportantMessageDelegate.Invoke(pm);
                continue;
            }

            if (_workload.BftAccountId == 0)
            {
                continue;
            }

            _dataHandler.TransferOriginalAccountStatement(_workload.Id, afFileName, $"Original-Kontoauszug im PDF-Format für BLZ {_workload.BankCode} Konto {_workload.AccountNr}");
        }
    }

    /// <summary>
    /// Add all safekeeping items to <see cref="PaymentHandler"/>
    /// </summary>
    public void GetSafeKeepingItems()
    {
        if (!_workload.SafeKeepingItems)
        {
            return;
        }

        SetStatus($"     -> Safekeepings {_workload.BankCode} / {_workload.AccountNr}");

        IList<SafekeepingItem> erg = null;

        try
        {
            erg = PaymentService.GetDepot();

        }
        catch (Exception ex2)
        {
            if (!ex2.Message.ToLower().Contains("illegal padding size"))
            {
                var pm = new PaymentMessage
                {
                    ErrorCode = 9907,
                    Message =
                        $"GetDepot:{PaymentService.PaymentContact.ToReadableString()}:{ex2.Message}"
                };

                _logger.LogError(pm.ToString());
                _setImportantMessageDelegate.Invoke(pm);
            }
        }

        if (erg == null)
        {
            return;
        }

        foreach (var h in erg.Where(x => _appGlobals.SafekeepingItems.All(y => y.Wkn != x.Wkn)))
        {
            _appGlobals.SafekeepingItems.Add(h);
        }
    }

    /// <summary>
    /// Get the balance for the account
    /// </summary>
    public decimal GetBalance()
    {
        SetStatus($"     -> Balance {_workload.BankCode} / {_workload.AccountNr}");

        var balance = PaymentService.GetBalance();

        var delta = Math.Abs(Math.Abs(balance) - Math.Abs(decimal.MinValue));

        if (delta < (decimal)0.001)
        {
            return decimal.MinValue;
        }

        _workload.Balance = balance;

        _dataHandler.UpdateBalance(_workload.Id, _workload.SqlBalance, _workload.Id, balance);

        return balance;
    }
    
    /// <summary>
    /// Get the owner for the account
    /// </summary>
    public void GetOwner() 
    {
        if (!string.IsNullOrEmpty(_workload.Owner))
        {
            return;
        }

        var owner = PaymentService.GetOwner();

        if (string.IsNullOrEmpty(owner))
        {
            return;
        }

        _workload.Owner = owner;
        _dataHandler.UpdateOwner(_workload.Id, owner);
    }



    /// <summary>
    /// Send a status message
    /// </summary>
    /// <param name="msg">message to be sent</param>
    private void SetStatus(string msg)
    {
        _logger.LogInformation(msg);
        _appGlobals.StatusMessageDelegate.Invoke(msg);
    }
}