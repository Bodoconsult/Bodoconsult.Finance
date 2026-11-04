using System.Diagnostics;
using Bodoconsult.Finance.Payment.FinApi.Test.Helpers;
using NUnit.Framework;

namespace Bodoconsult.Finance.Payment.FinApi.Test
{
    [TestFixture]
    public class UnitTestTestHelper
    {

        [Test]
        public void TestCrypt()
        {

            const string value = "https://live.finapi.io";

            var encrypted = Bodoconsult.Console.PasswordHandler.Encrypt(value);

            Debug.Print(encrypted);

            var result = Bodoconsult.Console.PasswordHandler.Decrypt(encrypted);

            Debug.Print(result);
            Assert.IsTrue(result==value);

        }

        [Test]
        public void TestCrypt1()
        {

            const string value = "Test";

            var encrypted = TestHelper.Encrypt(value);

            Debug.Print(encrypted);

            var result = TestHelper.Decrypt(encrypted);

            Assert.IsTrue(result == value);

        }

        [Test]
        public void TestGetUsername()
        {

            var result = TestHelper.GetUsername();

            Assert.IsNotNull(result);

        }

        [Test]
        public void TestGetPassword()
        {

            var result = TestHelper.GetPassword();

            Assert.IsNotNull(result);

        }

        [Test]
        public void TestGetClientId()
        {

            var result = TestHelper.GetClientId();

            Assert.IsNotNull(result);

        }

        [Test]
        public void TestGetClientSecret()
        {

            var result = TestHelper.GetClientSecret();

            Assert.IsNotNull(result);

        }



    }
}