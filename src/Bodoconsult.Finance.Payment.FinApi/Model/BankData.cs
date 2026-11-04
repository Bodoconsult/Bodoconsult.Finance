namespace Bodoconsult.Finance.Payment.FinApi.Model
{
    /// <summary>
    /// Required bank data for creating a new bank connection
    /// </summary>
    public class BankData
    {

        /// <summary>
        /// Bankleitzahl (BLZ)
        /// </summary>
        public string Blz { get; set; }

        /// <summary>
        /// Login username to the online banking system of a bank
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Login password to the online banking system of a bank
        /// </summary>
        public string Password { get; set; }


        /// <summary>
        /// Banking pin
        /// </summary>
        public string BankPin { get; set; }

        /// <summary>
        /// Bank user
        /// </summary>
        public string BankUser { get; set; }
    }
}