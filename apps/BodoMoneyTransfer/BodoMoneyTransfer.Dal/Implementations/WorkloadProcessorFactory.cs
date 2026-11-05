// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using System.Runtime.Versioning;
using Bodoconsult.App.Abstractions.Interfaces;
using BodoMoneyTransfer.Dal.Interfaces;

namespace BodoMoneyTransfer.Dal.Implementations;

/// <summary>
/// Factory for <see cref="WorkloadHandler"/> objects
/// </summary>
[SupportedOSPlatform("windows10.0.17763")]
public class WorkloadProcessorFactory : IWorkloadProcessorFactory
{
    //private readonly FinApiService _finApiService;
    private readonly IDataHandler _dataHandler;
    private readonly IAppGlobals _appGlobals;
    private readonly IWorkloadHandlerFactory _workloadHandlerFactory;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataHandler">Current data handler</param>
    /// <param name="appGlobals">Current banking options</param>
    /// <param name="workloadHandlerFactory">Current factory for workload handlers</param>
    public WorkloadProcessorFactory(
        //FinApiService finApiService,
        IDataHandler dataHandler, IAppGlobals appGlobals, IWorkloadHandlerFactory workloadHandlerFactory)
    {
        //_finApiService = finApiService;
        _dataHandler = dataHandler;
        _appGlobals = appGlobals;
        _workloadHandlerFactory = workloadHandlerFactory;
    }

    /// <summary>
    /// Get a <see cref="IWorkloadProcessor"/>
    /// </summary>
    /// <returns><see cref="IWorkloadProcessor"/> instance</returns>
    public IWorkloadProcessor CreateInstance()
    {
        return new WorkloadProcessor(_dataHandler, _appGlobals, _workloadHandlerFactory);
    }
}