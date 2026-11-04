using System;
using System.Diagnostics;
using System.Linq;
using Bodoconsult.Finance.Payment.FinApi.Model;
using Bodoconsult.Finance.Payment.FinApi.Services;
using Bodoconsult.Finance.Payment.FinApi.Test.Helpers;
using NUnit.Framework;

namespace Bodoconsult.Finance.Payment.FinApi.Test
{
    [TestFixture]
    public class UnitTestFinApiUserDataService
    {

        private string _url;
        private string _clientId;
        private string _clientSecret;

        private string UserName ;
        private string Password ;
        private const string EMail = "robert.leisner@bodoconsult.de";
        private const string Phone = "081064173";

        private FinApiUserDataService service;


        /// <summary>
        /// Setup before each unit test
        /// </summary>
        [SetUp]
        public void Init()
        {

            // https://live.finapi.io
            // https://sandbox.finapi.io

            _url = TestHelper.GetUrl();
            _clientId = TestHelper.GetClientId();
            _clientSecret = TestHelper.GetClientSecret();
            UserName = TestHelper.GetUsername();
            Password = TestHelper.GetPassword();

            service = new FinApiUserDataService(_url, _clientId, _clientSecret, UserName, Password);
        }

        /// <summary>
        /// Clean up after each unit test
        /// </summary>
        [TearDown]
        public void Cleanup()
        {

        }



        [Test]
        public void GetUserData()
        {

            //service.GetUserData();
        }



        /// <summary>
        /// Add a bank connection
        /// </summary>
        [Test]
        public void AddBankConnection()
        {

            //var bank = new BankData
            //{
            //    Blz = "70169619",
            //    UserName = TestHelper.GetUsername(),
            //    Password = TestHelper.GetPassword(),
            //    BankUser = "",
            //    BankPin = ""
            //};

            //var bank = new BankData
            //{
            //    Blz = "67390000",
            //    UserName = TestHelper.GetUsername(),
            //    Password = TestHelper.GetPassword(),
            //    BankUser = "",
            //    BankPin = ""
            //};

            var bank = new BankData
            {
                Blz = "76069601",
                UserName = TestHelper.GetUsername(),
                Password = TestHelper.GetPassword(),
                BankUser = "",
                BankPin = ""
            };

            service.AddBankConnection(bank);

        }


        /// <summary>
        /// Get all bank connections
        /// </summary>
        [Test]
        public void GetBankConnections()
        {

            var count = service.GetBankConnections();
            Assert.IsTrue(count>0);

        }






        /// <summary>
        /// Get all bank connections
        /// </summary>
        [Test]
        public void GetAccounts()
        {

            var count = service.GetBankConnections();
            Assert.IsTrue(count > 0);

            count = service.GetAccounts();
            Assert.IsTrue(count > 0);

            foreach (var account in service.Accounts.OrderBy(x=> x.AccountHolderName).ThenBy(x=> x.AccountNumber))
            {
                //Debug.Print(account.ToJson());
                Debug.Print($"{account.AccountHolderName} {account.AccountName} {account.AccountNumber}");
            }
        }

        /// <summary>
        /// Get transaction for an account
        /// </summary>
        [Test]
        public void GetTransactions()
        {

            var count = service.GetBankConnections();
            Assert.IsTrue(count > 0);

            count = service.GetAccounts();
            Assert.IsTrue(count > 0);

            var until = DateTime.Now;
            var from = DateTime.Now.AddDays(-30);

            var results = service.GetTransactions("70169619", "96636", from, until);

            
            foreach (var result in results)
            {
                //Debug.Print(account.ToJson());
                Debug.Print($"{result.CounterpartName} {result.Amount} {result.BankBookingDate}");

                Debug.Print(result.ToJson());
            }


        }






        [Test]
        public void GetDailyBalances()
        {

            var count = service.GetBankConnections();
            Assert.IsTrue(count > 0);

            count = service.GetAccounts();
            Assert.IsTrue(count > 0);

            var until = DateTime.Now;
            var from = DateTime.Now.AddDays(-50);

            var results = service.GetBalancesByDate("70169619", "96636", from, until);


            foreach (var result in results)
            {
                //Debug.Print(account.ToJson());
                Debug.Print($"{result.Date} {result.Balance}");
            }

            Assert.IsTrue(results.Any());

        }


        [Test]
        public void GetDepot()
        {

            var count = service.GetBankConnections();
            Assert.IsTrue(count > 0);

            count = service.GetAccounts();
            Assert.IsTrue(count > 0);

            var results = service.GetDepot("70169619", "619004637");


            foreach (var result in results)
            {
                Debug.Print($"{result.Name} {result.Quote} {result.Wkn}");
            }
        }

        [Test]
        public void GetBalance()
        {
            //service.SetUserToAotomatcUpdate();


            var count = service.GetBankConnections(true);
            Assert.IsTrue(count > 0);

            count = service.GetAccounts();
            Assert.IsTrue(count > 0);

            var until = DateTime.Now;
            var from = DateTime.Now.AddDays(-10);

            var result = service.GetBalance("70169619", "96636");

            Debug.Print($"Balance: {result}");
            Assert.IsTrue(result>0);
        }

        [Test]
        public void UpdateBankConnections()
        {

            var count = service.GetBankConnections();
            Assert.IsTrue(count > 0);

            count = service.GetAccounts();
            Assert.IsTrue(count > 0);

            var until = DateTime.Now;
            var from = DateTime.Now.AddDays(-10);

            var result = service.UpdateBankConnections();

            Assert.IsTrue(result);
        }


        [Test]
        public void UpdateBankConnectionsFromUi()
        {

            var count = service.GetBankConnections();
            Assert.IsTrue(count > 0);

            //count = service.GetAccounts();
            //Assert.IsTrue(count > 0);

            var until = DateTime.Now;
            var from = DateTime.Now.AddDays(-10);

            var result = service.UpdateBankConnectionsFromUi();

            Assert.IsTrue(result);

            count = service.GetAccounts();
            Assert.IsTrue(count > 0);

            foreach (var account in service.Accounts.OrderBy(x => x.AccountHolderName).ThenBy(x => x.AccountNumber))
            {
                //Debug.Print(account.ToJson());
                Debug.Print($"{account.AccountHolderName} {account.AccountName} {account.AccountNumber}");
            }
        }



        [Test]
        public void GetOwner()
        {

            var count = service.GetBankConnections();
            Assert.IsTrue(count > 0);

            count = service.GetAccounts();
            Assert.IsTrue(count > 0);

            var until = DateTime.Now;
            var from = DateTime.Now.AddDays(-10);

            var result = service.GetOwner("70169619", "619004637");

            Debug.Print($"Balance: {result}");
            Assert.IsFalse(string.IsNullOrEmpty(result));
        }
    }
}
