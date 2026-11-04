using Bodoconsult.Finance.Payment.Banking.Enums;
using System;

namespace BodoMoneyTransfer.Dal.Model;

/// <summary>
/// Data of a payment transaction
/// </summary>
public class Payment
{
    public string PayerOrPayee { get; set; }

    /// <summary>
    /// IBAN of the payee
    /// </summary>
    public string Iban { get; set; }
    public string Bic { get; set; }
    public string Description { get; set; }
    public decimal Total { get; set; }
    public string Currency { get; set; }
    public Guid Guid { get; set; }
    public string ExecOnSuccess { get; set; }
    public string ExecOnFailure { get; set; }

    /// <summary>
    /// SQL to execute on VOP close match result
    /// </summary>
    public string ExecOnVpoCloseMatch { get; set; }

    /// <summary>
    /// Current transaction state
    /// </summary>
    public TransactionState TransactionState { get; set; }

}