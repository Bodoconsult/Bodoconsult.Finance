using Bodoconsult.Finance.Payment.FinApi.Services;
using Bodoconsult.Finance.Payment.FinApi.Test.Helpers;
using NUnit.Framework;

namespace Bodoconsult.Finance.Payment.FinApi.Test
{
    [TestFixture]
    public class UnitTestFinApiClientDataService
    {

        private string _url;
        private string _clientId;
        private string _clientSecret;

        private string UserName;
        private string Password;
        private const string EMail = "robert.leisner@bodoconsult.de";
        private const string Phone = "081064173";

        private FinApiClientDataService service;


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

            service = new FinApiClientDataService(_url, _clientId, _clientSecret);
        }

        /// <summary>
        /// Clean up after each unit test
        /// </summary>
        [TearDown]
        public void Cleanup()
        {

        }

        /// <summary>
        /// Test an instance of ClientConfigurationApi
        /// </summary>
        [Test]
        public void AddUser()
        {
            service.AddUser(UserName, Password, EMail, Phone);

        }

    }
}