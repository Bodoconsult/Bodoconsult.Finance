// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System;
using System.Collections.Generic;
using Bodoconsult.Finance.Quotations.DataServices;
using Bodoconsult.Finance.Quotations.Interfaces;
using Bodoconsult.Finance.Quotations.Model;

namespace Bodoconsult.Finance.Quotations.Factories;

/// <summary>
/// Factory for data services
/// </summary>
public class DataServiceFactory
{
    /// <summary>
    /// Get a data service
    /// </summary>
    /// <param name="dataProvider">Type of data provider</param>
    /// <param name="securitiesItems">List of security items to connect with the data service</param>
    /// <param name="fileName">File name to quoation data if needed for the data provider</param>
    /// <returns></returns>
    public static IDataService GetDataService(DataProviderType dataProvider, IList<SecuritiesItem> securitiesItems,  string fileName="")
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
            case DataProviderType.Json:
                service = new JsonDataService();
                ((JsonDataService)service).JsonFile = fileName;
                break;
            case DataProviderType.Safekeepings:
                service = new SafekeepingDataService();
                ((SafekeepingDataService)service).JsonFile = fileName;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dataProvider), dataProvider, null);
        }

        service.SecuritiesItems = securitiesItems;

        return service;
    }

}