namespace BodoMoneyTransfer.Dal.Model;

/// <summary>
/// Banking service provider (PSD2 compatible)
/// </summary>
public enum BankingServiceProvider
{
    Unknown,
    Hbci,
    Ebics,
    FinApi
}