using BodoMoneyTransfer.Dal.Model;

namespace BodoMoneyTransfer.Dal.Helper;

/// <summary>
/// Helper class for <see cref="BankingServiceProvider"/>
/// </summary>
public class ProviderHelper
{
    /// <summary>
    /// Map a string to <see cref="BankingServiceProvider"/>
    /// </summary>
    /// <param name="providerName">Name of the provider: HBCI, EBICS or FinApi</param>
    /// <returns></returns>
    public static BankingServiceProvider MapToBankingServiceProvider(string providerName)
    {
        var erg = BankingServiceProvider.Unknown;

        switch (providerName.ToLower())
        {
            case "hbci":
                erg= BankingServiceProvider.Hbci;
                break;
            case "finapi":
                erg = BankingServiceProvider.FinApi;
                break;
            case "ebics":
                erg = BankingServiceProvider.Ebics;
                break;
        }

        return erg;
    }

}