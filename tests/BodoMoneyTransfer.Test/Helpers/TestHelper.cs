// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.DataProtection;
using Bodoconsult.App.Logging;
using BodoMoneyTransfer.Dal.App;
using System.Diagnostics;
using System.IO;

namespace BodoMoneyTransfer.Test.Helpers;

internal static class TestHelper
{
    public static string ConnectionString { get; } = "Data Source=192.168.10.121;Initial Catalog=BodoMoneyTransfer;Trusted_Connection=True;TrustServerCertificate=True";

    static TestHelper()
    {
        AppHelper.LoadPasswordData();
    }
    
    public static string Decrypt(string encrypted)
    {
        return PasswordHandler.Decrypt(encrypted);
    }

    public static string Encrypt(string unencrypted)
    {
        return PasswordHandler.Encrypt(unencrypted);
    }
    

    public static string GetHbciPin()
    {
        var s = File.ReadAllText(@"D:\Daten\Projekte\_work\Data\HBCI_PIN.txt");
        return Decrypt(s);
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
        var s = File.ReadAllText(@"D:\Daten\Projekte\_work\Data\FinApi_ClientID.txt");
        return Decrypt(s);

        //return "6ee5ba70-86d8-47d9-af3c-d0a9ad32a2ef";
    }

    public static string GetClientSecret()
    {
        var s = File.ReadAllText(@"D:\Daten\Projekte\_work\Data\FinApi_ClientSecret.txt");
        return Decrypt(s);

        //return "77533417-730f-43f1-aae2-a722bebce05b";
    }


    public static string GetUrl()
    {
        return "https://live.finapi.io";
        //return "https://sandbox.finapi.io";
    }

    /// <summary>
    /// Get a fake app logger proxy
    /// </summary>
    /// <returns>Fake app logger</returns>
    public static IAppLoggerProxy GetFakeAppLoggerProxy()
    {
        return new AppLoggerProxy(new FakeLoggerFactory(), new LogDataFactory());
    }

    /// <summary>
    /// Set a state message in the UI
    /// </summary>
    /// <param name="msg">Message to show</param>
    public static void SetState(string msg)
    {
        Debug.Print(msg);
    }


    //public static FinApiService GetFinApiService()
    //{
    //    var contact = new FinApiContact
    //    {
    //        Url = GetUrl(),
    //        UserName = GetUsername(),
    //        Password = GetPassword(),
    //        ClientId = GetClientId(),
    //        ClientSecret = GetClientSecret(),
    //        //Bankcode = bankCode,
    //        //AccountNr = accountNr,
    //        //Owner = string.Empty
    //    };

    //    var finApiServiceFactory = new FinApiServiceFactory
    //    {
    //        ProductName = Globals.ProductName,
    //        ProductVersion = Globals.ProductVersion,

    //    };

    //    finApiServiceFactory.ImportantMessageChange += Globals.SetImportantMessage;
    //    finApiServiceFactory.StatusChange += Globals.SetStatus;

    //    return (FinApiService) finApiServiceFactory.Create(contact);
    //}

}