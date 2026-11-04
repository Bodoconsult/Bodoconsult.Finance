// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.Finance.Payment.Base.Models;
using BodoMoneyTransfer.Dal.Interfaces;
using BodoMoneyTransfer.Dal.Model;
using System;
using System.Runtime.Versioning;
using Bodoconsult.App.Abstractions.Interfaces;

namespace BodoMoneyTransfer.Dal.Implementations;

[SupportedOSPlatform("windows10.0.17763")]
public class WorkloadProcessor : IWorkloadProcessor
{
    private readonly IWorkloadHandlerFactory _workloadHandlerFactory;
    private readonly IDataHandler _dataHandler;
    private readonly IOptions _options;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataHandler">Current data handöer instance</param>
    /// <param name="options">Banking options</param>
    /// <param name="logger">Current app logger</param>
    /// <param name="workloadHandlerFactory">Current workload handler factory</param>
    public WorkloadProcessor(
        //FinApiService finApiService,
        IDataHandler dataHandler, IOptions options, IAppLoggerProxy logger, IWorkloadHandlerFactory workloadHandlerFactory)
    {
        _workloadHandlerFactory = workloadHandlerFactory;
        //_workloadHandlerFactory = new WorkloadHandlerFactory(
        //    //finApiService,
        //    dataHandler, options, logger);

        _dataHandler = dataHandler;

        _options = options;
    }

    /// <summary>
    /// Process all workloads
    /// </summary>
    public void ProcessAll()
    {
        // Get all required contacts and process them separately
        // to avoid trouble if the contact has to be opened again due to PSD2 security requirements
        var dt = _dataHandler.GetContactsForWorkload();
        foreach (var r in dt)
        {
            _options.StatusMessageDelegate.Invoke($"Processing contact {r} started...");

            ProcessWorkloadForAContact(r);

            _options.StatusMessageDelegate.Invoke($"Processing contact {r} finished");
        }
    }

    private void ProcessWorkloadForAContact(string contactName)
    {
        var dt = _dataHandler.GetWorkload($"EXEC dbo.spContacts_GetWorkloadForContact '{contactName}';");
        foreach (var r in dt)
        {
            try
            {
                ProcessWorkload(r);
            }
            catch (Exception e)
            {
                var pm = new PaymentMessage
                {
                    ErrorCode = 9999,
                    Message = $"{e.Message} {e.StackTrace}"
                };

                _options.SetImportantMessageDelegate.Invoke(pm);
                return;
            }
        }
    }


    private void ProcessWorkload(WorkloadTask workload)
    {

        var wh = _workloadHandlerFactory.CreateInstance(workload);

        // wh.GetOwner();

        wh.GetBalance();

        if (_options.IsRunTransactions)
        {
            wh.RunMoneyTransfers();
        }

        if (_options.IsGetAccountStatement)
        {
            wh.GetAccountStatements(_options.AccountStatementDateFrom, _options.AccountStatementDateUntil);
        }

        if (_options.IsArchiveAccountStatements)
        {
            wh.ArchiveAccountStatementsAsMt940(_options.ArchiveAccountStatementDateFrom, _options.ArchiveAccountStatementDateUntil);
        }

        if (_options.IsArchiveOriginalAccountStatements)
        {
            wh.ArchiveOriginalAccountStatementPdfs();
        }

        if (_options.IsGetSafekeepingItems)
        {
            wh.GetSafeKeepingItems();
        }

        wh.Dispose();
    }
}