namespace BodoMoneyTransfer.Dal.Model;

/// <summary>
/// Represents an account with payments to transfer to the bank
/// </summary>
public class PaymentTask
{
    public PaymentTask()
    {
        PaymentType = "U";
    }

    /// <summary>
    /// Current banking service provider
    /// </summary>
    public BankingServiceProvider Type { get; set; }

    /// <summary>
    /// Name of the HBCI / EBICS password
    /// </summary>
    public string Contact { get; set; }

    /// <summary>
    /// Encrypted password for use with HBCI / EBICS
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// IBAN of the account
    /// </summary>
    public string Iban { get; set; }

    /// <summary>
    /// Account owner's name
    /// </summary>
    public string AccountName { get; set; }


    /// <summary>
    /// Is the payment a debit payment: U (Überweisung) or L (Lastschrift)
    /// </summary>
    public string PaymentType { get; set; }
}