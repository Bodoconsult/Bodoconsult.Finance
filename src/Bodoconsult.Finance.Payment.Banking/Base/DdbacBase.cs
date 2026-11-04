// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.Finance.Payment.Base;
using DDBAC.Net;

namespace Bodoconsult.Finance.Payment.Banking.Base;

/// <summary>
/// Base class for DDBAC based banking implementations
/// </summary>
public class DdbacBase
{
    /// <summary>
    ///  Current error number
    /// </summary>
    public int Error { get; protected set; }

    /// <summary>
    /// FINTS-Registriernummer als Produktbezeichnung ab 1.8.2019
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// ProductVersion (Länge 5 Zeichen)
    /// </summary>
    public string ProductVersion { get; set; }

    /// <summary>
    /// Current banking instance
    /// </summary>
    public BACBanking Banking { get; protected set; } = new();

    /// <summary>
    /// Delegate fired if a HBCI message was received
    /// </summary>
    public ShowPaymentMessageDelegate OnHbciMessage { get; set; }

    /// <summary>
    /// Current contact
    /// </summary>
    public BACContact Contact;

    /// <summary>
    /// Current account
    /// </summary>
    public BACAccount Account;

    /// <summary>
    /// Select a banking account by IBAN
    /// </summary>
    /// <param name="iban">IBAN of the account</param>
    public void SelectAccountIban(string iban)
    {
        try
        {
            if (Error != 0)
            {
                return;
            }

            foreach (var account in Contact.Accounts)
            {
                if (account.IBAN != iban)
                {
                    continue;
                }
                Account = account;
                return;
            }

            if (Account == null)
            {
                Error = -13;
            }

            //foreach (BACSegment b in m_oCustomer.AccountData.Segments)
            //{
            //    var s = b.GetXML();

            //    if (b["Kontoverbindung1", "Kontonummer1"].ToString() != iban) continue;
            //    m_oHIUPD = b;
            //    break;
            //}

            //var c = (BACCustomer)_mOCustomer;
            //foreach (BACAccount x in c.Accounts)
            //{

            //    //                   Debug.Print("{0} {1}", x.IBAN, iban);

            //    if (x.IBAN != iban) continue;
            //    _mAccount = x;
            //    Hiupd = (BACSegment)x.Segment;
            //    break;
            //}

            //var s = m_Account.IsSepa();

        }
        catch
        {
            Error = -13;
        }
    }

    /// <summary>
    ///  Customer ID
    /// </summary>
    public string CustomerId => Account == null ? string.Empty : Account.CustomerID;

    /// <summary>
    /// Select an account by its account number
    /// </summary>
    /// <param name="accountNumber">Account number (Kontonummer)</param>
    public void SelectAccount(string accountNumber)
    {
        try
        {
            if (Error != 0)
            {
                return;
            }

            foreach (var account in Contact.Accounts)
            {
                if (account.AccountNumber != accountNumber)
                {
                    continue;
                }
                Account = account;
                return;
            }
        }
        catch
        {
            Error = -14;
        }
    }

    /// <summary>
    /// Reset the internal error code
    /// </summary>
    public void ResetError()
    {
        Error = 0;
    }
}