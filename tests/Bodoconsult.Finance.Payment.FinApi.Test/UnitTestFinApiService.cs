using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Bodoconsult.Finance.Payment.FinApi.Model;
using Bodoconsult.Finance.Payment.FinApi.Services;
using Bodoconsult.Finance.Payment.FinApi.Test.Helpers;
using NUnit.Framework;

namespace Bodoconsult.Finance.Payment.FinApi.Test
{
    [TestFixture]
    public class UnitTestFinApiService
    {
        private FinApiContact _contact;

        private FinApiService _service;


        /// <summary>
        /// Setup before each unit test
        /// </summary>
        [SetUp]
        public void Init()
        {

            // https://live.finapi.io
            // https://sandbox.finapi.io

            _contact = new FinApiContact
            {
                Url = TestHelper.GetUrl(),
                UserName = TestHelper.GetUsername(),
                Password = TestHelper.GetPassword(),
                ClientId = TestHelper.GetClientId(),
                ClientSecret = TestHelper.GetClientSecret(),
                Bankcode = "70169619",
                AccountNr = "42374",
                Owner = ""
            };


            _service = new FinApiService {PaymentContact = _contact};
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



        ///// <summary>
        ///// Add a bank connection
        ///// </summary>
        //[Test]
        //public void AddBankConnection()
        //{

        //    //var bank = new BankData
        //    //{
        //    //    Blz = "70169619",
        //    //};

        //    var bank = new BankData
        //    {
        //        Blz = "67390000",
        //    };


        //    service.AddBankConnection(bank);

        //}


        /// <summary>
        /// Get transaction for an account
        /// </summary>
        [Test]
        public void GetTransactions()
        {


            var until = DateTime.Now;
            var from = DateTime.Now.AddDays(-60);

            var results = _service.GetHistory(from, until);


            foreach (var result in results)
            {
                //Debug.Print(account.ToJson());
                Debug.Print($"{result.Name} A: {result.Amount} Fee: {result.FeeAmount} E: {result.EntryDate}");
            }

            Assert.IsTrue(results.Any());
        }


        /// <summary>
        /// Get transaction for an account
        /// </summary>
        [Test]
        public void GetTransactionsToMt940AndPdf()
        {

            var fileNameMt940 = @"D:\temp\BodoMoneyTransfer\MT940.sta";
            var fileNamePdf = fileNameMt940.Replace(".sta", ".pdf");

            if (File.Exists(fileNamePdf)) File.Delete(fileNamePdf);
            if (File.Exists(fileNameMt940)) File.Delete(fileNameMt940);

            var until = DateTime.Now;
            var from = DateTime.Now.AddDays(-10);

            _service.GetHistoryToMt940File(from, until, fileNameMt940, true);



            Assert.IsTrue(File.Exists(fileNameMt940));
            Assert.IsTrue(File.Exists(fileNamePdf));

        }



        [Test]
        public void GetDepot()
        {

            _contact.AccountNr = "619004637";

            var results = _service.GetDepot();


            foreach (var result in results)
            {
                Debug.Print($"{result.Isin} {result.PricePerUnitAmount} {result.Wkn}");
            }
        }

        [Test]
        public void GetBalance()
        {


            var result = _service.GetBalance();

            Debug.Print($"AccountNr: {_contact.AccountNr} Balance: {result}");
            Assert.IsTrue(result > 0);
        }



        [Test]
        public void IsAccount()
        {


            var result = _service.IsAccount("DE23701696190000042374");

            Debug.Print($"IsAccount: {result}");
            Assert.IsTrue(result);
        }


        [Test]
        public void GetOwner()
        {

            var result = _service.GetOwner();

            Debug.Print($"Owner: {result}");
            Assert.IsFalse(string.IsNullOrEmpty(result));
        }
    }
}