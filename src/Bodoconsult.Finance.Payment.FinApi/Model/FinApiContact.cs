namespace Bodoconsult.Finance.Payment.FinApi.Model
{
    public class FinApiContact: IPaymentContact
    {
        public string ToReadableString()
        {
            var s = $"FinApiContact: {Url}: {UserName}: BLZ {Bankcode} AccountNr {AccountNr}";


            return s;
        }

        /// <inheritdoc />
        public string ContactType => "FINAPI";

        /// <summary>
        /// Account owner in clear text
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// KID REST api address
        /// </summary>
        public string Url;

        /// <summary>
        /// KID REST api client ID
        /// </summary> 
        public string ClientId;

        /// <summary>
        /// KID REST api client secret
        /// </summary>
        public string ClientSecret;

        /// <summary>
        /// KID REST username
        /// </summary>
        public string UserName;

        /// <summary>
        /// KID REST password
        /// </summary>
        public string Password;

        /// <summary>
        /// Bank code (BLZ)
        /// </summary>
        public string Bankcode;

        /// <summary>
        /// Bank account number
        /// </summary>
        public string AccountNr;


        /// <summary>
        /// International bank account number (IBAN)
        /// </summary>
        public string Iban { get; set; }
    }
}
