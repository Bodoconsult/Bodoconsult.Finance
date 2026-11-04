// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System;

namespace Bodoconsult.Finance.Payment.Base.Models;

/// <summary>
/// Represents general data of an account statement file delivered by the bank
/// </summary>
public class AccountStatementFile
{

    /// <summary>
    /// Bankcode (Bankleitzahl, BLZ)
    /// </summary>
    public string BankCode { get; set; }

    /// <summary>
    /// Bank internal account number
    /// </summary>
    public string AccountNr { get; set; }

    /// <summary>
    /// Type of account statement file like MT940, PDF
    /// </summary>
    public AccountStatementType AccountStatementType { get; set; }

    /// <summary>
    /// Status of the receipt handling
    /// </summary>
    public ReceiptHandling ReceiptHandling { get; set; }

    /// <summary>
    /// Year of the account statement
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Running number of the account statement file in the <see cref="Year"/>
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Date of creation
    /// </summary>
    public DateTime? CreationDateTime { get; set; }

    /// <summary>
    /// Erstellart
    /// </summary>
    public string CreationType { get; set; }

    /// <summary>
    /// Request possible
    /// </summary>
    public bool? RequestPossible { get; set; }


    /// <summary>
    /// Receipt required
    /// </summary>
    public bool ReceiptRequired { get; set; }

    /// <summary>
    /// Get default file name of the account statement file
    /// </summary>
    public string FileName
    {
        get
        {
            var ext = AccountStatementType == AccountStatementType.Pdf ? "pdf" : "sta";
            return $"{BankCode}_{AccountNr}_Kontoauszug_{Year}_{Number}.{ext}";
        }
    }

}