// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Bodoconsult.Finance.Quotations.Helpers;
using Bodoconsult.Finance.Payment.Base.Models;

namespace Bodoconsult.Finance.Quotations.DataServices;

/// <inheritdoc />
/// <summary>
/// Get data from a safekeeping account as json file via WKN
/// </summary>
public class SafekeepingDataService : BaseDataService
{
    /// <summary>
    /// JSON file containing depot data as <see cref="SafekeepingItem"/>
    /// </summary>
    public string JsonFile { get; set; }

    /// <summary>
    /// Get current quotation
    /// </summary>
    /// <returns></returns>
    public override void GetCurrentQuotation()
    {
        try
        {
            var erg = JsonHelper.LoadJsonFile<List<SafekeepingItem>>(JsonFile);

            foreach (var item in SecuritiesItems.Where(x => !string.IsNullOrEmpty(x.Wkn) && x.Wkn != "999999" && x.Quotation < 0.000001))
            {
                var result = erg.FirstOrDefault(x => x.Wkn == item.Wkn);

                if (result == null) continue;
                item.Quotation = (double) result.PricePerUnitAmount;
            }
        }
        catch (Exception ex)
        {
            SetError(ex);
        }
    }
}