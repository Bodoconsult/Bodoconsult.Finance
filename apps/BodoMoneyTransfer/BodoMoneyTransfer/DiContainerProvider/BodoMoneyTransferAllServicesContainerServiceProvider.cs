// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Factories;
using Bodoconsult.Database.Interfaces;
using Bodoconsult.Database.SqlClient;
using BodoMoneyTransfer.Dal.App;
using BodoMoneyTransfer.Dal.Implementations;
using BodoMoneyTransfer.Dal.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.Versioning;
using DiContainer = Bodoconsult.App.Abstractions.DependencyInjection.DiContainer;

namespace BodoMoneyTransfer.DiContainerProvider;

/// <summary>
/// Load all specific BodoMoneyTransfer services to DI container. Intended mainly for production
/// </summary>
[SupportedOSPlatform("windows10.0.17763")]
public class BodoMoneyTransferAllServicesContainerServiceProvider : IDiContainerServiceProvider
{
    //private readonly string _benchmarkFileName = Path.Combine("C:\\ProgramData\\BodoMoneyTransfer", "BodoMoneyTransfer_Benchmark.csv");
    private readonly IAppGlobals _appGlobals;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appGlobals">Current app globals</param>
    public BodoMoneyTransferAllServicesContainerServiceProvider(IAppGlobals appGlobals)
    {
        ArgumentNullException.ThrowIfNull(appGlobals);
        AppStartParameter = appGlobals.AppStartParameter;
        _appGlobals = appGlobals;
    }

    /// <summary>
    /// Current app start parameter
    /// </summary>
    public IAppStartParameter AppStartParameter { get; }

    /// <summary>
    /// Add DI container services to a DI container
    /// </summary>
    /// <param name="diContainer">Current DI container</param>
    public void AddServices(DiContainer diContainer)
    {
        if (_appGlobals is not IBodoMoneyTransferAppGlobals globals)
        {
            throw new ArgumentException("AppGlobals is not IBodoMoneyTransferAppGlobals");
        }

        // Factories to create tower instance related objects (should be singletons)
        diContainer.AddSingletonInstance(globals.LogDataFactory);
        diContainer.AddSingleton<IAppLoggerProxyFactory, AppLoggerProxyFactory>();

        //// benchmark
        //var benchProxy = AppBenchProxy.CreateAppBenchProxy(_benchmarkFileName, globals.LogDataFactory);
        //diContainer.AddSingletonInstance(benchProxy);

        //// General app management
        //diContainer.AddSingleton<IGeneralAppManagementService, GeneralAppManagementService>();
        //diContainer.AddSingleton<IGeneralAppManagementManager, GeneralAppManagementManager>();

        // Services
        //diContainer.AddSingleton();

        // ReSharper disable once SuggestVarOrType_SimpleTypes
        IConnManager db = SqlClientConnManager.GetConnManager(globals.AppStartParameter.DefaultConnectionString);
        diContainer.AddSingleton(db);
        diContainer.AddSingleton<IDataHandler, DataHandler>();
        diContainer.AddSingleton<IWorkloadHandlerFactory, WorkloadHandlerFactory>();
        diContainer.AddSingleton<IWorkloadProcessorFactory, WorkloadProcessorFactory>();
        diContainer.AddSingleton<IPaymentHandler, PaymentHandler>();
        
        // Load all other services required for the app now
        diContainer.AddSingleton<IApplicationService, BodoMoneyTransferService>();
    }

    /// <summary>
    /// Late bind DI container references to avoid circular DI references
    /// </summary>
    /// <param name="diContainer"></param>
    public void LateBindObjects(DiContainer diContainer)
    {
        var appLogger = diContainer.Get<IAppLoggerProxy>();

        //appLogger.LogInformation($"Benchmark starts logging to {_benchmarkFileName}...");

        // Set logger to current logger factory
        var loggerFactory = diContainer.Get<ILoggerFactory>();
        appLogger.UpdateILoggerFactory(loggerFactory);
    }
}