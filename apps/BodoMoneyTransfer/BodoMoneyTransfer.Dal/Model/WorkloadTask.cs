namespace BodoMoneyTransfer.Dal.Model;

/// <summary>
/// Describes a task with workload like getting account statement data and make money transfers
/// </summary>
public class WorkloadTask
{
    /// <summary>
    /// ID of the account statement task
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Bank code (Bankleitzahl BLZ)
    /// </summary>
    public string BankCode { get; set; }

    /// <summary>
    /// Account number (Kontonummer)
    /// </summary>
    public string AccountNr { get; set; }


    public string Iban { get; set; }


    /// <summary>
    /// Current banking service provider to use for account statements and transactions
    /// </summary>
    public BankingServiceProvider Provider { get; set; }


    /// <summary>
    /// ID of the bank connection at the service provider
    /// </summary>
    public int BankConnectionId { get; set; }

    /// <summary>
    /// ID of the account at the service provider
    /// </summary>
    public int AccountId { get; set; }

    /// <summary>
    /// Name of the HBCI / EBICS password
    /// </summary>
    public string Contact { get; set; }

    /// <summary>
    /// Encrypted password for use with HBCI / EBICS
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// SQL statement to save account statement rows
    /// </summary>
    public string SqlStatementRows { get; set; }

    /// <summary>
    /// SQL statement to save balance
    /// </summary>
    public string SqlBalance { get; set; }

    /// <summary>
    /// BodoMoneyTransfer account ID
    /// </summary>
    public int BftAccountId { get; set; }

    /// <summary>
    /// Use opt-out option fror verification-of-payee (VOP) process
    /// </summary>
    public bool VopOptOut { get; set; }

    /// <summary>
    /// Account Owner
    /// </summary>
    public string Owner { get; set; }

    /// <summary>
    /// Signature
    /// </summary>
    public string Signature { get; set; }

    /// <summary>
    /// Get live account statement
    /// </summary>
    public bool AccountStatementsLive { get; set; }
    /// <summary>
    /// Get historical account statements as MT940
    /// </summary>
    public bool AccountStatementsHistoricalData { get; set; }

    /// <summary>
    /// Get official account statement PDFs
    /// </summary>
    public bool AccountStatementsOfficialHistoricalData { get; set; }

    /// <summary>
    /// Get safekeeping items
    /// </summary>
    public bool SafeKeepingItems { get; set; }

    /// <summary>
    /// Run transfers
    /// </summary>
    public bool RunTransfers { get; set; }

    /// <summary>
    /// Current balance
    /// </summary>
    public decimal Balance { get; set; }
}