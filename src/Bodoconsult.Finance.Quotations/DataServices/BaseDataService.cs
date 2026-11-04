// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Bodoconsult.App.Abstractions.Delegates;
using Bodoconsult.Finance.Quotations.Interfaces;
using Bodoconsult.Finance.Quotations.Model;

namespace Bodoconsult.Finance.Quotations.DataServices;

/// <summary>
/// Base data service class provides central functionality
/// </summary>
public abstract class BaseDataService : IDataService
{
    private readonly HttpClient _client = new() { DefaultRequestVersion = HttpVersion.Version20 };

    /// <summary>
    /// Default ctor
    /// </summary>
    protected BaseDataService()
    {
        SecuritiesItems = new List<SecuritiesItem>();
    }
    
    /// <summary>
    /// Send a status message to the UI
    /// </summary>
    public StatusMessageDelegate StatusMessageDelegate { get; set; }

    /// <summary>
    /// Send an error message to the UI
    /// </summary>
    public HandleExceptionDelegate HandleExceptionDelegate { get; set; }

    /// <summary>
    /// Send a status message
    /// </summary>
    /// <param name="msg"></param>
    public void SetMessage(string msg)
    {
        StatusMessageDelegate?.Invoke(msg);
    }

    /// <summary>
    /// Send an error message
    /// </summary>
    /// <param name="ex"></param>
    public void SetError(Exception ex)
    {
        HandleExceptionDelegate?.Invoke(ex);
    }

    /// <summary>
    /// Items to get data for
    /// </summary>
    /// <returns>List with security items</returns>
    public IList<SecuritiesItem> SecuritiesItems { get; set; }

    /// <summary>
    /// Get current quotation
    /// </summary>
    public virtual void GetCurrentQuotation()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get the quotation history for a time period
    /// </summary>
    /// <param name="from">time period from</param>
    /// <param name="to">time period to</param>
    public virtual void GetHistory(DateTime from, DateTime to)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get the ISIN for the SecurityItem (WKN must be available already)
    /// </summary>
    public void GetIsin()
    {
        foreach (var item in SecuritiesItems)
        {
            // http://www.bing.com/search?q=DE0002635265

            var url = $"http://www.bing.com/search?q={item.Wkn}";

            var r = GetUrl(url);

            var i = r.IndexOf("http://www.finanzen.net", StringComparison.OrdinalIgnoreCase);
            var j = r.IndexOf('"', i);


            var bing = r.Substring(i, j - i);

            r = GetUrl(bing);
            i = r.IndexOf("<title>", StringComparison.OrdinalIgnoreCase);
            j = r.IndexOf("</title>", StringComparison.OrdinalIgnoreCase);

            var title = r.Substring(i, j - i);

            i = title.IndexOf('|');
            i = title.IndexOf('|', i + 1);
            j = title.IndexOf('|', i + 1);

            if (j < 0)
            {
                j = title.Length;
            }

            item.Isin = title.Substring(i + 1, j - i - 1).Trim();
        }
    }

    /// <summary>
    /// Get the WKN for the SecurityItem (ISIN must be available already)
    /// </summary>
    public void GetWkn()
    {
        foreach (var item in SecuritiesItems)
        {

            // http://www.bing.com/search?q=DE0002635265

            var url = $"http://www.bing.com/search?q={item.Isin}";

            var r = GetUrl(url);

            var i = r.IndexOf("http://www.finanzen.net", StringComparison.Ordinal);
            var j = r.IndexOf('"', i);


            var bing = r.Substring(i, j - i);

            r = GetUrl(bing);
            i = r.IndexOf("<title>", StringComparison.OrdinalIgnoreCase);
            j = r.IndexOf("</title>", StringComparison.OrdinalIgnoreCase);

            var title = r.Substring(i, j - i);

            i = title.IndexOf('|');
            //i = title.IndexOf("|", i+1, StringComparison.OrdinalIgnoreCase);
            j = title.IndexOf('|', i + 1);

            item.Wkn = title.Substring(i + 1, j - i - 1).Trim();
        }
    }

    /// <summary>
    /// Get the token for the SecurityItem via WKN or ISIN
    /// </summary>
    public void GetToken()
    {
        foreach (var item in SecuritiesItems)
        {
            if (!string.IsNullOrEmpty(item.Token))
            {
                continue;
            }

            var erg = GetTokenInternally(item.Wkn);
            item.Token = erg == null ? null : "YAHOO:" + erg;

            if (!string.IsNullOrEmpty(item.Token))
            {
                item.Changed = true;
                return;
            }

            erg = GetTokenInternally(item.Isin);
            item.Token = erg == null ? null : "YAHOO:" + erg;

            if (!string.IsNullOrEmpty(item.Token))
            {
                item.Changed = true;
            }
        }
    }

    private string GetTokenInternally(string wknOrIsin)
    {
        try
        {
            //string url = string.Format("http://de.finance.yahoo.com/q?s={0}EUR", ISIN);
            //StreamReader response = new StreamReader(this.getWebStream(url));
            //string r = response.ReadToEnd();

            //if (r.Contains("Ihre Suche") == false)
            //{
            //    return ISIN + "EUR";
            //}

            // http://de.finsearch.yahoo.com/de/index.php?s=de_sort&nm=A0HG2Q&tp=*&r=*&sub=Suchen

            var url =
                $"http://de.finsearch.yahoo.com/de/index.php?s=de_sort&nm={wknOrIsin}&tp=*&r=*&sub=Suchen";

            var r = GetUrl(url);
            var i = r.IndexOf("/q?s=", StringComparison.OrdinalIgnoreCase);
            var result = new StringBuilder();
            while (i != -1)
            {
                var z = r.IndexOf('"', i);
                result.Append(
                    $"{r.Substring(i, z - i).Replace("/q?s=", string.Empty, StringComparison.OrdinalIgnoreCase)},");
                i = r.IndexOf("/q?s=", z, StringComparison.OrdinalIgnoreCase);
            }


            var s = result.ToString().Split(',');

            // XETRA
            foreach (var x in s.Where(x => x.EndsWith(".DE", StringComparison.OrdinalIgnoreCase)))
            {
                return x;
            }

            // Frankfurt
            foreach (var x in s.Where(x => x.EndsWith(".F", StringComparison.OrdinalIgnoreCase)))
            {
                return x;
            }

            // Stuttgart
            foreach (var x in s.Where(x => x.EndsWith(".SG", StringComparison.OrdinalIgnoreCase)))
            {
                return x;
            }

            // Hamburg
            foreach (var x in s.Where(x => x.EndsWith(".HM", StringComparison.OrdinalIgnoreCase)))
            {
                return x;
            }

            // München
            foreach (var x in s.Where(x => x.EndsWith(".MU", StringComparison.OrdinalIgnoreCase)))
            {
                return x;
            }

            // Berlin
            foreach (var x in s.Where(x => x.EndsWith(".BE", StringComparison.OrdinalIgnoreCase)))
            {
                return x;
            }

            // Düsseldorf
            foreach (var x in s.Where(x => x.EndsWith(".DU", StringComparison.OrdinalIgnoreCase)))
            {
                return x;
            }

            // Paris
            foreach (var x in s.Where(x => x.EndsWith(".PA", StringComparison.OrdinalIgnoreCase)))
            {
                return x;
            }

            // Mailand
            foreach (var x in s.Where(x => x.EndsWith(".MI", StringComparison.OrdinalIgnoreCase)))
            {
                return x;
            }

            return result.ToString();

        }
        catch (Exception ex)
        {
            SetError(ex);
            return null;
        }
    }

    /// <summary>
    /// Load the content of a web URL
    /// </summary>
    /// <param name="url">URL to load</param>
    /// <returns>URL content</returns>
    protected string GetUrl(string url)
    {
        //try
        //{
        //var response = new StreamReader(GetWebStream(url));

        //return response.ReadToEnd();

        return GetWebStream(url);

        //}
        //catch (Exception ex)
        //{
        //    var s = string.Format("ERROR:GetUrl:{0}:{1}", url, ex.Message);
        //    SetMessage(s);
        //    return null;
        //}

    }


    /// <summary>
    /// helper method for Agents that need to communicate with their source over http/https
    /// </summary>
    /// <param name="url">url that should be opened</param>
    /// <returns>URL content</returns>
    protected string GetWebStream(string url)
    {
        try
        {
            //ServicePointManager.ServerCertificateValidationCallback += AcceptAllCertifications;

            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var myUri = new Uri(url);

            var content = _client.GetStringAsync(myUri);

            content.Wait(5000);

            var result = content.Result;

            return result;


            //var request = (HttpWebRequest)WebRequest.Create(myUri);
            //request.AllowAutoRedirect = true;

            ////allows for validation of SSL certificates 


            //var response = (HttpWebResponse)request.GetResponse();
            //var dataStream = response.GetResponseStream();

            //if (dataStream == null) return null;
            //var reader = new StreamReader(dataStream);
            //var responseFromServer = reader.ReadToEnd();
            //return responseFromServer;
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("(404)"))
            {
                return null;
            }

            var s = $"ERROR:GetWebStream:{url}:{ex.Message}";
            SetMessage(s);
            return null;
        }
    }

    //private static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
    //{
    //    return true;
    //}
}