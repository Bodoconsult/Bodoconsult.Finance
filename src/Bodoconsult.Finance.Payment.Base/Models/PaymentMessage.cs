// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


namespace Bodoconsult.Finance.Payment.Base.Models;

/// <summary>
/// Payment message with error code and error message to transport as result of a payment transaction
/// </summary>
public class PaymentMessage
{
    /// <summary>
    /// HBCI-Fehlermeldung
    /// 0XXX -> Erfolgsmeldungen 
    ///3XXX -> Warnungen und/oder Hinweise 
    ///9XXX -> Fehlermeldungen 
    ///Die zweite Ziffer gibt dann einen Hinweis auf die mögliche Ursache:
    ///X0XX -> Meldung zum Status (z.b. 0020 Auftrag verarbeitet) 
    ///X1XX -> Meldung zum Syntax (falsche HBCI Version o.ä.) 
    ///X2XX -> Die Aussage gilt nur für einen speziellen Geschäftsvorfall 
    ///X3XX -> Meldungen die das Sicherheitsverfahren betreffen 
    ///X4XX -> Im Moment nur reserviert (Sicherheitsmedien) 
    ///X8XX -> Sonstige Meldungen (z.B die besagte Abbruchmeldung) 
    ///X9XX -> Individuelle Meldungen vom Rechenzentrum 
    /// </summary>
    public int ErrorCode;

    /// <summary>
    /// HbciBanking class error codes
    /// </summary>
    public int ErrorCode1;

    /// <summary>
    /// Payment message or null
    /// </summary>
    public string Message;
    
    /// <summary>
    /// Transaction cursor
    /// </summary>
    public string Cursor;

    /// <summary>
    /// Transaction ID
    /// </summary>
    public string TransactionId;

    /// <summary>
    /// Approval URL
    /// </summary>
    public string ApprovalUrl;

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return
            $"HBCI error code {ErrorCode}: Internal code {ErrorCode1}: {Message}{(string.IsNullOrEmpty(Cursor) ? string.Empty : $" Cursor {Cursor}")}{(string.IsNullOrEmpty(TransactionId) ? string.Empty : $" TransactionId {TransactionId}")}{(string.IsNullOrEmpty(ApprovalUrl) ? string.Empty : $" ApprovalUrl{ApprovalUrl}")}";
    }
}