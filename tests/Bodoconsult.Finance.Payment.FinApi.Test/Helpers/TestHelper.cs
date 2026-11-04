using System.IO;

namespace Bodoconsult.Finance.Payment.FinApi.Test.Helpers
{
    internal class TestHelper
    {

        public static string Decrypt(string encrypted)
        {
            return Bodoconsult.Console.PasswordHandler.Decrypt(encrypted);
        }


        public static string Encrypt(string unencrypted)
        {
            return Bodoconsult.Console.PasswordHandler.Encrypt(unencrypted);
        }



        public static string GetUsername()
        {
            var s = File.ReadAllText(@"D:\Daten\Projekte\_work\Data\FinApi_User.txt");
            return Decrypt(s);
        }

        public static string GetPassword()
        {
            var s = File.ReadAllText(@"D:\Daten\Projekte\_work\Data\FinApi_Password.txt");
            return Decrypt(s);
        }

        public static string GetClientId()
        {
            //var s = File.ReadAllText(@"D:\Daten\Projekte\_work\Data\FinApi_ClientID.txt");
            //return Decrypt(s);

            return "6ee5ba70-86d8-47d9-af3c-d0a9ad32a2ef";
        }

        public static string GetClientSecret()
        {
            //var s = File.ReadAllText(@"D:\Daten\Projekte\_work\Data\FinApi_ClientSecret.txt");
            //return Decrypt(s);

            return "77533417-730f-43f1-aae2-a722bebce05b";
        }


        public static string GetUrl()
        {
          // return "https://live.finapi.io";
            return "https://sandbox.finapi.io";
        }
    }
}
