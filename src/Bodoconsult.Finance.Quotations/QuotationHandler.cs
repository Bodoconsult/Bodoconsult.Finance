// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Delegates;
using Bodoconsult.Finance.Quotations.DataServices;
using Bodoconsult.Finance.Quotations.Factories;
using Bodoconsult.Finance.Quotations.Interfaces;
using Bodoconsult.Finance.Quotations.Model;
using System;
using System.Collections.Generic;

namespace Bodoconsult.Finance.Quotations;

/// <summary>
/// Handles the collection of quotations from diverse sources
/// </summary>
public class QuotationHandler : IQuotationHandler
{
    /// <summary>
    /// Send a status message to the UI
    /// </summary>
    public StatusMessageDelegate StatusMessageDelegate { get; set; }

    /// <summary>
    /// Send an error message to the UI
    /// </summary>
    public HandleExceptionDelegate HandleExceptionDelegate { get; set; }

    /// <summary>
    /// Send a state message to the UI
    /// </summary>
    /// <param name="msg">Message to send</param>
    public void SetMessage(string msg)
    {
        StatusMessageDelegate?.Invoke(msg);
    }

    /// <summary>
    /// Handle an exception
    /// </summary>
    /// <param name="ex">Exception to handle</param>
    public void SetError(Exception ex)
    {
        HandleExceptionDelegate?.Invoke(ex);
    }

    /// <summary>
    /// Items to get data for
    /// </summary>
    public List<SecuritiesItem> SecuritiesItems { get; } = new();

    /// <summary>
    /// Get current quotations
    /// </summary>
    /// <param name="dataProvider">type of data provider</param>
    public void StartGetCurrentQuotations(DataProviderType dataProvider)
    {
        var service = DataServiceFactory.GetDataService(dataProvider, SecuritiesItems, null);
        service.GetCurrentQuotation();
    }

    /// <summary>
    /// Get current quotations with a filename as parameter
    /// </summary>
    /// <param name="dataProvider">type of data provider</param>
    /// <param name="fileName">filename to load the quotations from</param>
    public void StartGetCurrentQuotations(DataProviderType dataProvider, string fileName)
    {
        var service = DataServiceFactory.GetDataService(dataProvider, SecuritiesItems, fileName);

        //foreach (var item in SecuritiesItems)
        //{
        //    Debug.Print($"{item.Wkn}: {item.Quotation}");
        //}
        
        service.GetCurrentQuotation();
    }

    /// <summary>
    /// Get a data service
    /// </summary>
    /// <param name="dataProvider">Type of data provider</param>
    /// <param name="fileName">File name to quoation data if needed for the data provider</param>
    /// <returns>Quotation data service</returns>
    public IDataService GetDataService(DataProviderType dataProvider, string fileName)
    {
        IDataService service;
        switch (dataProvider)
        {
            case DataProviderType.Msn:
                service = new MsnDataService();
                break;
            case DataProviderType.Ariva:
                service = new ArivaDataService();
                break;
            case DataProviderType.Bundesbank:
                service = new BundesbankDataService();
                break;
            case DataProviderType.Quandl:
                service = new QuandlDataService();
                break;
            case DataProviderType.Safekeepings:
                service = new SafekeepingDataService();
                ((SafekeepingDataService)service).JsonFile = fileName;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dataProvider), dataProvider, null);
        }

        service.SecuritiesItems = SecuritiesItems;

        return service;
    }

    /// <summary>
    /// Load quotation history 
    /// </summary>
    /// <param name="dataProvider">type of data provider</param>
    /// <param name="from">start date</param>
    /// <param name="until">end date</param>
    public void StartGetHistory(DataProviderType dataProvider, DateTime from, DateTime until)
    {
        var service = DataServiceFactory.GetDataService(dataProvider, SecuritiesItems, null);
        service.GetHistory(from, until);
    }

    /// <summary>
    /// Clear the securities in the quotation handler
    /// </summary>
    public void ClearSecurities()
    {
        SecuritiesItems.Clear();
    }
}