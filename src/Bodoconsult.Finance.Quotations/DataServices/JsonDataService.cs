// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System;
using System.Collections.Generic;
using System.Linq;
using Bodoconsult.Finance.Quotations.Helpers;
using Bodoconsult.Finance.Quotations.Model;

namespace Bodoconsult.Finance.Quotations.DataServices;

/// <inheritdoc />
/// <summary>
/// Get quoattion data from as a json file via WKN
/// </summary>
public class JsonDataService : BaseDataService
{
    /// <summary>
    /// JSON file containing quotation data as <see cref="QuotationItem"/>
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

            var erg = JsonHelper.LoadJsonFile<List<BasicQuotationItem>>(JsonFile);

            foreach (var item in erg)
            {

                var si = SecuritiesItems.FirstOrDefault(x => x.Isin == item.Isin);
                if (si != null)
                {
                    si.Quotation = item.Quotation;
                    continue;
                }

                si = new SecuritiesItem
                {
                    Isin = item.Isin,
                    Quotation = item.Quotation
                };

                SecuritiesItems.Add(si);

            }

        }
        catch (Exception ex)
        {
            SetError(ex);
        }
    }

}