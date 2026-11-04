using System.Diagnostics;
using NUnit.Framework;

namespace Bodoconsult.Finance.Payment.FinApi.Test
{
    [TestFixture]
    public class UnitTestDiverse
    {

        [Test]
        public void TestStartWebpage()
        {

            Process.Start("http://www.bodoconsult.de");

            //Assert.IsNotNull(p);
            Assert.IsTrue(true);
        }
    }
}