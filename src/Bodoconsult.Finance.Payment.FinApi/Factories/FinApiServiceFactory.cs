using System;
using Bodoconsult.Finance.Payment.Base;
using Bodoconsult.Finance.Payment.Base.Interfaces;
using Bodoconsult.Finance.Payment.Base.Models;
using Bodoconsult.Finance.Payment.FinApi.Services;

namespace Bodoconsult.Finance.Payment.FinApi.Factories
{
    /// <summary>
    /// Factory for FinApi service. 
    /// </summary>
    /// <remarks>Works with only one instance of the service due to the structure of FinApi. See https://sandbox.finanpi.io for details on FinApi.</remarks>
    public class FinApiServiceFactory : IPaymentServiceFactory
    {


        /// <summary>
        /// FINTS-Registiernummer als Produktbezeichnung ab 1.8.2019
        /// </summary>
        public string ProductName { get; set; } = "DDD7CE97F50B1DFEB50594A5F";

        /// <summary>
        /// Produktversion (Länge 5 Zeichen)
        /// </summary>
        public string ProductVersion { get; set; } = "1.0";


        /// <summary>
        /// Current status message handler for important messages
        /// </summary>
        public event ShowMessage ImportantMessageChange;


        /// <summary>
        /// Current status message handler
        /// </summary>
        public event StatusHandler StatusChange;

        private FinApiService _service;

        public IPaymentService Create(IPaymentContact contact)
        {

            if (_service != null) return _service;

            try
            {
                _service = new FinApiService();
                if (StatusChange != null) _service.StatusChange += StatusChange;
                if (ImportantMessageChange != null) _service.ImportantMessageChange += ImportantMessageChange;
                _service.PaymentContact = contact;
            }
            catch (Exception e)
            {

                var pm = new PaymentMessage
                {
                    ErrorCode = 9000,
                    Message = e.Message
                };

                ImportantMessageChange?.Invoke(pm);

                return null;
            }
            return _service;
        }
    }
}
