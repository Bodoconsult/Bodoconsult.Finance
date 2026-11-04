// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.Finance.Payment.Banking.Ebics;
using Bodoconsult.Finance.Payment.Banking.Factories;
using Bodoconsult.Finance.Payment.Banking.Hbci;
using Bodoconsult.Finance.Payment.Base.Interfaces;
using BodoMoneyTransfer.Dal.Interfaces;
using BodoMoneyTransfer.Dal.Model;
using System.Runtime.Versioning;
using Bodoconsult.App.DataProtection;

namespace BodoMoneyTransfer.Dal.Implementations;

/// <summary>
/// Factory for <see cref="WorkloadHandler"/> objects
/// </summary>
[SupportedOSPlatform("windows10.0.17763")]
public class WorkloadHandlerFactory : IWorkloadHandlerFactory
{
    //private readonly FinApiService _finApiService;
    private readonly IDataHandler _dataHandler;
    private readonly IAppLoggerProxy _logger;
    private readonly IOptions _options;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataHandler">Current data handler</param>
    /// <param name="options">Current banking options</param>
    /// <param name="logger">Current app logger</param>
    public WorkloadHandlerFactory(
        //FinApiService finApiService,
        IDataHandler dataHandler, IOptions options, IAppLoggerProxy logger)
    {
        //_finApiService = finApiService;
        _dataHandler = dataHandler;
        _options = options;
        _logger = logger;
    }

    /// <summary>
    /// Get a <see cref="IWorkloadHandler"/>
    /// </summary>
    /// <param name="workload">Current workload to handle</param>
    /// <returns></returns>
    public IWorkloadHandler CreateInstance(WorkloadTask workload)
    {

        //bool isAccount = false;

        // Load the provider used for transactions
        var paymentService = GetPaymentService(workload);

        var msg = $"{paymentService.PaymentContact.ContactType}: working on {workload.BankCode} / {workload.AccountNr}";
        _logger.LogInformation(msg);
        _options.StatusMessageDelegate.Invoke(msg);

        var wh = new WorkloadHandler(_dataHandler, paymentService, workload, _options, _logger);

        return wh;
    }
    
    /// <summary>
    /// Get the request payment service
    /// </summary>
    /// <param name="workload"></param>
    /// <returns></returns>
    public IPaymentService GetPaymentService(WorkloadTask workload)
    {
        IPaymentContact contact;

        IPaymentServiceFactory factory;

        switch (workload.Provider)
        {
            case BankingServiceProvider.Hbci:

                contact = new HbciBankingContact
                {
                    Name = workload.Contact,
                    Passphrase = PasswordHandler.Decrypt(workload.Password),
                    AccountNr = workload.AccountNr,
                    Iban = workload.Iban
                };

                factory = new HbciBankingServiceFactory
                {
                    ProductName = _options.ProductName,
                    ProductVersion = _options.ProductVersion
                };
                break;

            case BankingServiceProvider.Ebics:

                contact = new EbicsBankingContact
                {
                    Name = workload.Contact,
                    Passphrase = PasswordHandler.Decrypt(workload.Password),
                    AccountNr = workload.AccountNr,
                    Iban = workload.Iban
                };

                factory = new EbicsBankingServiceFactory
                {
                    ProductName = _options.ProductName,
                    ProductVersion = _options.ProductVersion
                };
                break;

            case BankingServiceProvider.FinApi:
            //break;
            case BankingServiceProvider.Unknown:
            default:
                return null;
        }

        var service = factory.CreateInstance(contact);
        service.StatusMessageDelegate = _options.StatusMessageDelegate;
        service.ImportantMessageChange = _options.SetImportantMessageDelegate;
        return service;
    }
}