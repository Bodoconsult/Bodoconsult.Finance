using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bodoconsult.Finance.Payment.Base;
using Bodoconsult.Finance.Payment.Base.Interfaces;
using Bodoconsult.Finance.Payment.Base.Models;
using Bodoconsult.Finance.Payment.FinApi.Client;
using Bodoconsult.Finance.Payment.FinApi.Model;

namespace Bodoconsult.Finance.Payment.FinApi.Services
{
    /// <summary>
    /// FinApi payment service
    /// </summary>
    public class FinApiService : IPaymentService
    {

        private FinApiContact _paymentContact;

        private FinApiUserDataService _service;

        /// <inheritdoc />
        public bool OpenContact()
        {
            // do nothing
            return true;
        }

        public void CloseContact()
        {
            // do nothing
        }

        public PaymentMessage CreatePayment(IPaymentOrder order)
        {
            throw new NotImplementedException();
        }

        public PaymentMessage CreatePayment(IPaymentOrder order, string cancelUrl, string returnUrl)
        {
            throw new NotImplementedException();
        }

        public PaymentMessage ConfirmPayment(string token, string payerId)
        {
            throw new NotImplementedException();
        }

        public PaymentMessage Transfer(IEnumerable<IPaymentOrder> transactions)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Get all found accounts
        /// </summary>
        /// <returns></returns>
        public IList<Account> GetAccounts()
        {
            return _service.Accounts;
        }


        public decimal GetBalance()
        {
            //try
            //{
                return _service.GetBalance(_paymentContact.Bankcode, _paymentContact.AccountNr);
            //}
            //catch (ApiException e)
            //{
            //    HandleApiException(e);

            //}
            //catch (Exception e)
            //{
            //    HandleException(e);
            //}

            //return decimal.MinValue;
        }

        /// <summary>
        /// Is there a certain account
        /// </summary>
        /// <param name="iban">IBAN of the account to check existence for</param>
        /// <returns>true if the account exists</returns>
        public bool IsAccount(string iban)
        {

            try
            {
                return _service.Accounts.Any(x => x.Iban == iban);
            }
            catch (ApiException e)
            {
                HandleApiException(e);

            }
            catch (Exception e)
            {
                HandleException(e);
            }

            return false;
        }


        public string GetOwner()
        {
            try
            {
                return _service.GetOwner(_paymentContact.Bankcode, _paymentContact.AccountNr);
            }
            catch (ApiException e)
            {
                HandleApiException(e);

            }
            catch (Exception e)
            {
                HandleException(e);
            }

            return null;

        }

        public IList<AccountStatement> GetHistory(DateTime startDate, DateTime endDate)
        {
            var erg = new List<AccountStatement>();

            try
            {

                var data = _service.GetTransactions(_paymentContact.Bankcode, _paymentContact.AccountNr,
                    startDate, endDate);

                foreach (var transaction in data)
                {

                    var st = new AccountStatement
                    {
                        AccountNo = transaction.CounterpartAccountNumber,
                        BankCode = transaction.CounterpartBlz,
                        EntryDate = Convert.ToDateTime(transaction.BankBookingDate),
                        ValidityDate = Convert.ToDateTime(transaction.ValueDate),
                        BusinessTransactionCode = transaction.TypeCodeZka,
                        Name = transaction.CounterpartName,
                        Transactionid = transaction.Id.ToString(),
                        Purpose = transaction.Purpose,
                        CurrencyCode = "EUR",
                        PrimaNota = transaction.Primanota,
                        BusinessTransactionText = transaction.Type.Replace("Ue", "Ü").Replace("Abschluß", "ABSCHLUSS")
                    };

                    if (transaction.Amount == null) continue;

                    st.Amount = (decimal)transaction.Amount;
                    var orig = transaction.OriginalAmount ?? 0;

                    if (Math.Abs(orig) < (decimal)0.0001) orig = st.Amount;


                    st.FeeAmount = Math.Abs(orig) - Math.Abs(st.Amount);

                    erg.Add(st);
                }
            }
            catch (ApiException e)
            {
                HandleApiException(e);
            }
            catch (Exception e)
            {
                HandleException(e);
            }

            return erg;

        }

        public void GetHistoryToMt940File(DateTime startDate, DateTime endDate, string targetFileName)
        {
            GetHistoryToMt940File(startDate, endDate, targetFileName, false);
        }

        public void GetHistoryToMt940File(DateTime startDate, DateTime endDate, string targetFileName, bool createPdfAccountStatement)
        {

            try
            {
                var balances = _service.GetBalancesByDate(_paymentContact.Bankcode, _paymentContact.AccountNr,
                    startDate, endDate).OrderBy(x => x.Date).ToList();

                var min = Convert.ToDecimal(balances[0].Balance);
                var max = Convert.ToDecimal(balances[balances.Count - 1].Balance);



                var service = new Mt940Service { TargetFileName = targetFileName };

                var file = service.DataFile;
                file.AccountNumber = $"{_paymentContact.Bankcode}/{_paymentContact.AccountNr}" ;
                file.OpeningBalanceAmount = min;
                file.OpeningBalanceDate = startDate;
                file.ClosingBalanceAmount = max;
                file.ClosingBalanceDate = endDate;
                file.AvailableBalanceAmount = max;
                file.AvailableBalanceDate = endDate;

                file.Items = GetHistory(startDate, endDate);

                service.SaveMt940File();


                // DDBAC-Version: no entry date
                //oMt940.SaveAs(targetFile);

                if (!createPdfAccountStatement) return;

                var pdfFile = targetFileName.Replace(".sta", ".pdf");

                var header = new AccountStatementHeader
                {
                    AccountOwner = _paymentContact.Owner,
                    AccountNr = _paymentContact.AccountNr,
                    StartDate = startDate,
                    EndDate = endDate
                };

                if (File.Exists(pdfFile)) File.Delete(pdfFile);

                header.StartBalanceDate = DateTime.Today;
                header.StartBalanceAmount = min;
                header.StartBalanceCurrency = "EUR";
                header.EndBalanceDate = DateTime.Today;
                header.EndBalanceAmount = max;
                header.EndBalanceCurrency = "EUR";


                var pdf = new PdfAccountStatement(file.Items, header, pdfFile);
                pdf.CreateFile();

            }
            catch (ApiException e)
            {
                HandleApiException(e);
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }



        public IList<AccountStatementFile> GetOfficialAccountStatementFiles()
        {
            throw new NotImplementedException();
        }

        public bool GetOfficialAccountStatementFile(AccountStatementFile fileData, bool sendReceipt, string fileName)
        {
            throw new NotImplementedException();
        }

        public IList<SafekeepingItem> GetDepot()
        {
            var result = new List<SafekeepingItem>();

            try
            {

                var data = _service.GetDepot(_paymentContact.Bankcode, _paymentContact.AccountNr);

                foreach (var item in data)
                {
                    var si = new SafekeepingItem();
                    if (item.Quote != null) si.PricePerUnitAmount = (decimal)item.Quote;
                    si.Isin = item.Isin;
                    si.Wkn = item.Wkn;
                    si.Title = item.Name;
                    si.PricePerUnitCurrencyCode = item.QuoteCurrency;

                    result.Add(si);
                }
            }
            catch (ApiException e)
            {
                HandleApiException(e);
            }
            catch (Exception e)
            {
                HandleException(e);
            }

            return result;

        }

        private void HandleException(Exception e)
        {
            var pm = new PaymentMessage
            {
                ErrorCode = 9010,
                Message = $"Payment contact needs Webform approval: {e.Message}: {PaymentContact.ToReadableString()}"
            };
            SetImportantMessage(pm);

            throw new Exception("9010", e);
        }

        private void HandleApiException(ApiException e)
        {
            var pm = new PaymentMessage { ErrorCode = 9015 };

            switch (e.ErrorCode)
            {
                case 451:   // Webform required
                    pm.ErrorCode1 = 451;
                    pm.ApprovalUrl = e.Url;
                    pm.Message = $"Payment contact {PaymentContact.ToReadableString()} needs Webform approval:   {e.Url}";
                    break;
                default:
                    pm.Message = $"Payment contact {PaymentContact.ToReadableString()} needs Webform approval: : {e.Message} {e.Url}";
                    break;
            }

            SetImportantMessage(pm);


        }

        public IPaymentContact PaymentContact
        {
            get => _paymentContact;
            set
            {
                _paymentContact = (FinApiContact)value;

                try
                {
                    _service = new FinApiUserDataService(_paymentContact.Url,
                        _paymentContact.ClientId, _paymentContact.ClientSecret,
                        _paymentContact.UserName, _paymentContact.Password);

                    _service.GetBankConnections();
                    _service.GetAccounts();
                }
                catch (ApiException e)
                {
                    HandleApiException(e);
                }
                catch (Exception e)
                {
                    HandleException(e);
                }


            }
        }

        /// <inheritdoc />
        public event StatusHandler StatusChange;

        /// <inheritdoc />
        public event ShowMessage ImportantMessageChange;


        /// <summary>
        /// Statusanzeige
        /// </summary>
        /// <param name="msg"></param>
        private void SetStatus(string msg)
        {
            var x = StatusChange;
            x?.Invoke(msg);
        }


        /// <summary>
        /// Statusanzeige
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="values"></param>
        private void SetStatus(string msg, params object[] values)
        {
            var x = StatusChange;
            x?.Invoke(string.Format(msg, values));
        }

        /// <summary>
        /// Statusanzeige
        /// </summary>
        /// <param name="message"></param>
        private void SetImportantMessage(PaymentMessage message)
        {
            var x = ImportantMessageChange;
            x?.Invoke(message);
        }

    }
}
