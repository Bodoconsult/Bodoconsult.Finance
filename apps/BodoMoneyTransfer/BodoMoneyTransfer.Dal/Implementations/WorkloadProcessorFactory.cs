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
    private readonly IAppLoggerProxy _logger;
    private readonly IOptions _options;
    private readonly IWorkloadHandlerFactory _workloadHandlerFactory;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataHandler">Current data handler</param>
    /// <param name="options">Current banking options</param>
    /// <param name="logger">Current app logger</param>
    /// <param name="workloadHandlerFactory">Current factory for workload handlers</param>
    public WorkloadProcessorFactory(
        //FinApiService finApiService,
        IDataHandler dataHandler, IOptions options, IAppLoggerProxy logger, IWorkloadHandlerFactory workloadHandlerFactory)
    {
        //_finApiService = finApiService;
        _dataHandler = dataHandler;
        _options = options;
        _logger = logger;
        _workloadHandlerFactory = workloadHandlerFactory;
    }

    /// <summary>
    /// Get a <see cref="IWorkloadProcessor"/>
    /// </summary>
    /// <returns><see cref="IWorkloadProcessor"/> instance</returns>
    public IWorkloadProcessor CreateInstance()
    {
        return new WorkloadProcessor(_dataHandler, _options, _logger, _workloadHandlerFactory);
    }
}