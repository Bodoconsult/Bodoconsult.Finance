// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Runtime.Versioning;

namespace BodoMoneyTransfer;

[SupportedOSPlatform("windows10.0.17763")]
public class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        var starter = new BodoMoneyTransferConsoleStarter(args);
        starter.LoadWorkload();
        return starter.Start();


        //var log4NetConfig = new XmlDocument();
        //log4NetConfig.Load(File.OpenRead("log4net.config"));

        //var repo = LogManager.CreateRepository(
        //    Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

        //XmlConfigurator.Configure(repo, log4NetConfig["log4net"]);

        //Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

        //Globals1.LoadAppSettings();
        //var settings = Globals1.CurrentAppSettings;

        //// Logging mit Log4net
        //ShowMessage("BodoMoneyTransfer started");

        //// Set product version
        //var assembly = Assembly.GetExecutingAssembly();
        //var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
        //var versiondata = fvi.FileVersion.Split(["."], StringSplitOptions.RemoveEmptyEntries);

        //var version = $"{versiondata[0]}.{versiondata[1]}";

        //Globals1.ProductVersion = version;
        //Globals1.ProductName = "blubb";

        //// set other globals
        //Globals1.StatusMessageDelegate += ShowMessage;

        //// Options
        //var options = new Options
        //{
        //    ArchivePath = settings.ArchivePath,
        //    OriginalArchivePath = settings.OriginalArchivePath,
        //    QuotationFileName = settings.QuotationFile,
        //    ConnectionString = settings.ConnectionString,
        //    ErrorMailer = settings.ErrorMailer,
                
        //};

        //if (args.Length == 0)
        //{
        //    AppStarter.Start(options, null);
        //    return;
        //}

        //AppStarter.Start(options, args[0]);


        ////#if DEBUG
        ////            Console.ReadLine();
        ////#endif

    }


    //private static void ShowMessage(string msg)
    //{
    //    //Logger.Info(msg);
    //    Console.WriteLine(msg);
    //}
}