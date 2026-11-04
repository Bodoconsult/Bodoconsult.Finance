//using BodoMoneyTransfer.Dal.Implementations;
//using BodoMoneyTransfer.Dal.Model;
//using System.Runtime.Versioning;

//namespace BodoMoneyTransfer.Dal.App;

//[SupportedOSPlatform("windows")]
//public static class AppStarter
//{

//    public static void Start(Options options, string arg)
//    {

//        // Database handler
//        var dh = new DataHandler(options);
        

//        var ph = new PaymentHandler(dh, options);
        

//        // Command line args
//        if (!string.IsNullOrEmpty(arg))
//        {
//            switch (arg.ToLower())
//            {
//                case "/t":
//                    ph.SendTestMail();
//                    //ShowMessage("Test mail sent!");
//                    //ShowMessage("Application is closing...!");
//                    return;
//                case "/u":
//                    options.IsArchiveAccountStatements = false;
//                    options.IsArchiveOriginalAccountStatements = false;
//                    options.IsGetSafekeepingItems = false;
//                    options.IsGetAccountStatement = false;
//                    break;
//            }
//        }


//        ph.Options.UseKidForAccountInfos = false;

//        ph.StartJobCollector();
//        ph.ProcessAllWorkloads();

//        ph.SendErrorMails();
//    }

//}