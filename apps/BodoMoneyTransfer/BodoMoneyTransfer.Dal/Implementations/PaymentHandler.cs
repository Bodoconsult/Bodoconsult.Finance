// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.Finance.Payment.Base.Models;
using BodoMoneyTransfer.Dal.Helper;
using BodoMoneyTransfer.Dal.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using BodoMoneyTransfer.Dal.Delegates;

namespace BodoMoneyTransfer.Dal.Implementations;

[SupportedOSPlatform("windows")]
public class PaymentHandler : IPaymentHandler
{
    //private readonly FinApiContact _finApiContact;
    //private FinApiServiceFactory _finApiServiceFactory;
    //private FinApiService _finApiService;
    private readonly IAppLoggerProxy _logger;
    private readonly IWorkloadProcessorFactory _workloadProcessorFactory;
    private List<PaymentMessage> ErrorMessages { get; } = [];

    public IBodoMoneyTransferAppGlobals AppGlobals { get; }

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataHandler">Current data handler</param>
    /// <param name="appGlobals">Current app globals</param>
    /// <param name="workloadProcessorFactory">Current workload processor factory</param>
    public PaymentHandler(IDataHandler dataHandler, IAppGlobals appGlobals, IWorkloadProcessorFactory workloadProcessorFactory)
    {
        if (appGlobals is not IBodoMoneyTransferAppGlobals globals)
        {
            throw new ArgumentException("appGlobals is not IBodoMoneyTransferAppGlobals");
        }

        AppGlobals = globals;
        AppGlobals.SetImportantMessageDelegate = SetImportantMessage;
        DataHandler = dataHandler;
        _logger = appGlobals.Logger;
        _workloadProcessorFactory = workloadProcessorFactory;

        SendMailDelagate = dataHandler.SendMail;

        //_finApiContact = new FinApiContact
        //{
        //    Url = PasswordHandler.Decrypt(DataHandler.GetSetting("KID_Url")),
        //    ClientId = PasswordHandler.Decrypt(DataHandler.GetSetting("KID_ClientID")),
        //    ClientSecret = PasswordHandler.Decrypt(DataHandler.GetSetting("KID_ClientSecret")),
        //    UserName = PasswordHandler.Decrypt(DataHandler.GetSetting("KID_UserName")),
        //    Password = PasswordHandler.Decrypt(DataHandler.GetSetting("KID_Password"))
        //};

        AppGlobals.UseKidForAccountInfos = true;

        //if (!string.IsNullOrEmpty(_finApiContact.Url)) return;
        AppGlobals.UseKidForAccountInfos = false;
    }

    /// <summary>
    /// Current data handler
    /// </summary>
    public IDataHandler DataHandler { get; }

    /// <summary>
    /// Delegate to send an email
    /// </summary>
    public SendMailDelegate SendMailDelagate { get; set; }

    /// <summary>
    /// Get all jobs for payments to be done from different target databases
    /// </summary>
    public void StartJobCollector()
    {
        SendState("JobCollector gestartet...");

        var jobs = DataHandler.GetJobTasks();
        foreach (var job in jobs)
        {
            try
            {
                DataHandler.Exec(job.Sql);
            }
            catch (Exception e)
            {
                SendMail($"StartJobCollector:{job.Sql}", e.Message);
            }
        }

        SendState("JobCollector fertig!");
    }

    /// <summary>
    /// Send a test mail: mail is used to in form admin users regarding errors
    /// </summary>
    public void SendTestMail()
    {
        SendMail("SendTestMail", "<h1>Testmail</h1><p>This is a simple test mail.</p>");
    }

    /// <summary>
    /// Run all the workload 
    /// </summary>
    public void ProcessAllWorkloads()
    {
        try
        {
            // *****************************
            // Preparation
            // *****************************

            // Reset the balances of the accounts
            SendState("Reset balances...");
            DataHandler.ResetBalance();

            // Archiv bereinigen (wegen DS-GVO)
            SendState("Clean transaction archive...");
            DataHandler.Exec("EXEC dbo.Transaction_CleanArchive");


            //if (UseKidForAccountInfos)
            //{

            //    SendState("Load KID connection...");
            //    //_finApiServiceFactory = new FinApiServiceFactory
            //    //{
            //    //    ProductName = Globals.ProductName,
            //    //    ProductVersion = Globals.ProductVersion,

            //    //};

            //    //_finApiServiceFactory.ImportantMessageChange += Globals.SetImportantMessage;
            //    //_finApiServiceFactory.StatusChange += Globals.SetStatus;

            //    //_finApiService = (FinApiService)_finApiServiceFactory.Create(_finApiContact);
            //}
            //else
            //{
            //    //_finApiServiceFactory = null;
            //    //_finApiService = null;
            //}

            // *****************************
            // Loop of workloads
            // *****************************

            SendState("Process workloads...");
            var wp = _workloadProcessorFactory.CreateInstance();
            wp.ProcessAll();

            // *****************************
            // Finalize
            // *****************************
        }
        catch (Exception e)
        {
            var pm = new PaymentMessage
            {
                ErrorCode = 9998,
                Message = $"{e.Message} {e.StackTrace}"
            };

            SetImportantMessage(pm);
        }
    }

    /// <summary>
    /// Save the safekeeping items to a JSON file
    /// </summary>
    public void SaveSafeKeepingsFile()
    {
        SendState("Save safekeepings...");

        if (string.IsNullOrEmpty(AppGlobals.QuotationFileName))
        {
            AppGlobals.Logger?.LogWarning("No valid path given for AppGlobals.QuotationFileName");
            return;
        }

        if (AppGlobals.SafekeepingItems.Count <= 0)
        {
            return;
        }

        if (File.Exists(AppGlobals.QuotationFileName))
        {
            File.Delete(AppGlobals.QuotationFileName);
        }

        JsonHelper.SaveAsFile(AppGlobals.QuotationFileName, AppGlobals.SafekeepingItems);
    }

    /// <summary>
    /// Send all error messages in one mail
    /// </summary>
    public void SendErrorMails()
    {
        if (ErrorMessages.Count == 0)
        {
            return;
        }

        var msg = new StringBuilder();

        foreach (var message in ErrorMessages.OrderByDescending(x => x.ErrorCode))
        {
            msg.Append($"<h1>Banking-Message {message.ErrorCode1} // {message.ErrorCode}</h1><p>{message.Message}</p>");

            if (!string.IsNullOrEmpty(message.ApprovalUrl))
            {
                msg.Append($"<p>ApprovalUrl: {message.ApprovalUrl}</p>");
            }
        }

        SendMail("BodoMoneyTransfer: Errors", msg.ToString());
    }


    public void SetImportantMessage(PaymentMessage message)
    {
        var msg = $"Banking-Message {message.ErrorCode1}: {message.ErrorCode} // {message.Message} ";

        if (!string.IsNullOrEmpty(message.ApprovalUrl))
        {
            msg += $" // ApprovalUrl: {message.ApprovalUrl}";
        }

        if (message.ErrorCode < 4000)
        {
            if (message.ErrorCode != 3060)
            {
                _logger.LogInformation(msg);
            }
        }
        else
        {
            _logger.LogError(msg);
            ErrorMessages.Add(message);
        }

        SendState(message.ToString());
    }

    /// <summary>
    /// Send an error mail to the admin
    /// </summary>
    /// <param name="subject">Subject of the mail</param>
    /// <param name="body">Mail body</param>
    public void SendMail(string subject, string body)
    {
        var del = SendMailDelagate;
        del?.Invoke(subject, body);
    }

    private void SendState(string message)
    {
        AppGlobals.StatusMessageDelegate?.Invoke(message);
    }
}