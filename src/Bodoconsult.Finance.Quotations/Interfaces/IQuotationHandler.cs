// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using System;
using System.Collections.Generic;
using Bodoconsult.App.Abstractions.Delegates;
using Bodoconsult.Finance.Quotations.DataServices;
using Bodoconsult.Finance.Quotations.Model;

namespace Bodoconsult.Finance.Quotations.Interfaces;

/// <summary>
/// Interface for handling the collection of quotations from diverse sources
/// </summary>
public interface IQuotationHandler
{
    /// <summary>
    /// Send a status message to the UI
    /// </summary>
    StatusMessageDelegate StatusMessageDelegate { get; set; }

    /// <summary>
    /// Send an error message to the UI
    /// </summary>
    HandleExceptionDelegate HandleExceptionDelegate { get; set; }

    /// <summary>
    /// Items to get data for
    /// </summary>
    List<SecuritiesItem> SecuritiesItems { get; }

    /// <summary>
    /// Send a state message to the UI
    /// </summary>
    /// <param name="msg">Message to send</param>
    void SetMessage(string msg);

    /// <summary>
    /// Handle an exception
    /// </summary>
    /// <param name="ex">Exception to handle</param>
    void SetError(Exception ex);

    /// <summary>
    /// Get current quotations
    /// </summary>
    /// <param name="dataProvider">type of data provider</param>
    void StartGetCurrentQuotations(DataProviderType dataProvider);

    /// <summary>
    /// Get current quotations with a filename as parameter
    /// </summary>
    /// <param name="dataProvider">type of data provider</param>
    /// <param name="fileName">filename to load the quotations from</param>
    void StartGetCurrentQuotations(DataProviderType dataProvider, string fileName);

    /// <summary>
    /// Get a data service
    /// </summary>
    /// <param name="dataProvider">Type of data provider</param>
    /// <param name="fileName">File name to quoation data if needed for the data provider</param>
    /// <returns>Quotation data service</returns>
    IDataService GetDataService(DataProviderType dataProvider, string fileName);

    /// <summary>
    /// Load quotation history 
    /// </summary>
    /// <param name="dataProvider">type of data provider</param>
    /// <param name="from">start date</param>
    /// <param name="until">end date</param>
    void StartGetHistory(DataProviderType dataProvider, DateTime from, DateTime until);

    /// <summary>
    /// Clear the securities in the quotation handler
    /// </summary>
    void ClearSecurities();
}