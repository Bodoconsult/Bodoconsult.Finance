// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System;
using System.Collections.Generic;
using Bodoconsult.App.Abstractions.Delegates;
using Bodoconsult.Finance.Quotations.Model;

namespace Bodoconsult.Finance.Quotations.Interfaces;

/// <summary>
/// Data service interface for getting stock quotations and similar data from diverse sources
/// </summary>
public interface IDataService
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
    /// Send a status message
    /// </summary>
    /// <param name="msg"></param>
    void SetMessage(string msg);

    /// <summary>
    /// Send an error message
    /// </summary>
    /// <param name="ex"></param>
    void SetError(Exception ex);

    /// <summary>
    /// Current items to fetch data for
    /// </summary>
    /// <returns>List with security items</returns>
    IList<SecuritiesItem> SecuritiesItems { get; set; }

    /// <summary>
    /// Get current quotation
    /// </summary>
    void GetCurrentQuotation();

    /// <summary>
    /// Get the quotation history for a time period
    /// </summary>
    /// <param name="from">time period from</param>
    /// <param name="to">time period to</param>
    void GetHistory(DateTime from, DateTime to);

    /// <summary>
    /// Get the ISIN for the SecurityItem (WKN must be available already)
    /// </summary>
    void GetIsin();

    /// <summary>
    /// Get the WKN for the SecurityItem (ISIN must be available already)
    /// </summary>
    void GetWkn();

    /// <summary>
    /// Get the token for the SecurityItem via WKN or ISIN
    /// </summary>
    void GetToken();
}