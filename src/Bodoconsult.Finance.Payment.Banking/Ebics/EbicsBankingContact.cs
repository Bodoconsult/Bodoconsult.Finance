using Bodoconsult.Finance.Payment.Banking.Interfaces;

namespace Bodoconsult.Finance.Payment.Banking.Ebics;

/// <summary>
/// Class for providing general EBICS banking information
/// </summary>
public class EbicsBankingContact : IBankingContact
{
    /// <summary>
    /// Type description of the contact (HBCI, EBICS, PAYPAL, FINAPI, ...)
    /// </summary>
    public string ContactType => "EBICS";
    
    /// <summary>
    /// Name of the contact (see HomeBankingAdministrator)
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Passphrase for opening key files (see HomeBankingAdministrator). If a <see cref="IBankingContact.Pin"/> is provided the passphrase is ignored
    /// </summary>
    public string Passphrase { get; set; }

    /// <summary>
    /// Account name given by the bank internal system
    /// </summary>
    public string AccountName { get; set; }

    /// <summary>
    /// Bank internal account number
    /// </summary>
    public string AccountNr { get; set; }

    /// <summary>
    /// IBAN international banking account code
    /// </summary>
    public string Iban { get; set; }

    /// <summary>
    /// Bank Identifier Code of the account's bank
    /// </summary>
    public string Bic { get; set; }

    /// <summary>
    /// PIN for the account
    /// </summary>
    public string Pin { get; set; }
    
    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public string ToReadableString()
    {
        return $"EBICS-Contact: {Name} / {AccountNr} / {Iban}";
    }
}