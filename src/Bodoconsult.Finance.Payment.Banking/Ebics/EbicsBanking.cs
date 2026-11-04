// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.Finance.Payment.Banking.Base;
using Bodoconsult.Finance.Payment.Banking.Models;
using Bodoconsult.Finance.Payment.Base.Models;
using DDBAC.Net;
using DDBAC.Net.Camt;
using DDBAC.Net.Ebics;
using DDBAC.Net.Swift;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using Bodoconsult.App.Extensions;

namespace Bodoconsult.Finance.Payment.Banking.Ebics;

/// <summary>
/// Banking class for EBICS based banking
/// </summary>
[SupportedOSPlatform("windows")]
public class EbicsBanking : BaseBanking
{
    private readonly DdbacEbics _ddbacEbics = new();

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="productName">FinTS product name (ZKA registration number)</param>
    /// <param name="productVersion">FinTS product version</param>
    public EbicsBanking(string productName, string productVersion)
    {
        _ddbacEbics.ProductName = productName;
        _ddbacEbics.ProductVersion = productVersion;

        UserName = null;
        AccountNr = null;
        Passphrase = null;
        Contact = null;
        ErrorMessage = null;
        Error = 0;
    }

    /// <summary>
    /// Open connection to the bank via Kontonummer
    /// </summary>
    public override PaymentMessage OpenAccount()
    {
        if (Error != 0)
        {
            return null;
        }

        try
        {
            _ddbacEbics.OpenContact(Contact, Passphrase);
            _ddbacEbics.SelectAccount(AccountNr);

            return null;
        }
        catch (Exception ex)
        {
            return NoHbciError(ex);
        }
    }

    /// <summary>
    /// Open connection to the bank via IBAN
    /// </summary>
    public override PaymentMessage OpenAccountSepa()
    {
        if (Error != 0)
        {
            return null;
        }

        try
        {
            _ddbacEbics.OpenContact(Contact, Passphrase);
            _ddbacEbics.SelectAccountIban(Iban);

            return null;
        }
        catch (Exception ex)
        {
            return NoHbciError(ex);
        }
    }

    /// <summary>
    /// Transfer multiple transactions for one account using VOP opt-out option
    /// </summary>
    /// <param name="transferData">Transactions to transfer</param>
    /// <param name="fromBic">BIC the money is transferred from</param>
    /// <returns>A payment message indicating the success of the transfers</returns>
    public override PaymentMessage TransferMultipleTransactionsVopOptOut(IEnumerable<BankingOrder> transferData, string fromBic)
    {
        if (Error != 0 || _ddbacEbics.Account == null)
        {
            var hbciMessage = EmptyMessage();
            hbciMessage.ErrorCode = 9000;
            hbciMessage.ErrorCode1 = Error;
            Error = 77;
            return hbciMessage;
        }

        //try
        //{
        //var hbciMessage = EmptyMessage();
        //if (Error != 0)
        //{
        //    hbciMessage.ErrorCode = 9000;
        //    hbciMessage.ErrorCode1 = Error;
        //    return hbciMessage;
        //}


        foreach (var t in transferData)
        {

            var msg = CreateSepaMessage(t, out var ta, out var aSepaMessage);

            if (msg != null)
            {
                return msg;
            }

            var xml = aSepaMessage.GenerateXml();
            var message = _ddbacEbics.UploadData(xml, "CCT");
            ta.Result = message;

            //var xml = aSepaMessage.GenerateXml();

            //var fileName = GetTempFileName();
            //if (fileName == null)
            //{
            //    var hbciMessage = EmptyMessage();
            //    hbciMessage.ErrorCode = 9000;
            //    hbciMessage.ErrorCode1 = Error;
            //    Error = 78;
            //    return hbciMessage;
            //}

            //Debug.Print(fileName);

            ////#if DEBUG
            ////            fileName = "C:\\Temp\\sepa.xml";
            ////#endif
            //if (File.Exists(fileName))
            //{
            //    File.Delete(fileName);
            //}

            //// Not working: Encoding.UTF8 (which adds BOM)
            //Encoding utf8WithoutBom = new UTF8Encoding(false);
            //File.WriteAllText(fileName, xml, utf8WithoutBom);

            ////aSepaMessage.Save(fileName);
            //var message = _ddbacEbics.UploadData(fileName, "CCT");

            //ta.Result = message;

            //if (!File.Exists(fileName))
            //{
            //    continue;
            //}

            //try
            //{
            //    File.Delete(fileName);
            //}
            //catch
            //{
            //    // ignored
            //}
        }
        return EmptyMessage();
        //}
        //catch (Exception ex)
        //{
        //    var msg = new PaymentMessage { Message = ex.Message, ErrorCode = 9999, ErrorCode1 = 9998 };
        //    return msg;
        //}
    }


    /// <summary>
    /// Transfer multiple transactions for one account using VOP
    /// </summary>
    /// <param name="transferData">Transactions to transfer</param>
    /// <param name="fromBic">BIC the money is transferred from</param>
    /// <returns>A payment message indicating the success of the transfers</returns>
    public override PaymentMessage TransferMultipleTransactionsVop(IEnumerable<BankingOrder> transferData, string fromBic)
    {
        if (Error != 0 || _ddbacEbics.Account == null)
        {
            var hbciMessage = EmptyMessage();
            hbciMessage.ErrorCode = 9000;
            hbciMessage.ErrorCode1 = Error;
            Error = 77;
            return hbciMessage;
        }

        //try
        //{
        //var hbciMessage = EmptyMessage();
        //if (Error != 0)
        //{
        //    hbciMessage.ErrorCode = 9000;
        //    hbciMessage.ErrorCode1 = Error;
        //    return hbciMessage;
        //}


        foreach (var t in transferData)
        {

            var msg = CreateSepaMessage(t, out var ta, out var aSepaMessage);

            if (msg != null)
            {
                return msg;
            }

            var xml = aSepaMessage.GenerateXml();
            var message = _ddbacEbics.UploadData1(xml, "CTV");
            ta.Result = message;

        }
        return EmptyMessage();
        //}
        //catch (Exception ex)
        //{
        //    var msg = new PaymentMessage { Message = ex.Message, ErrorCode = 9999, ErrorCode1 = 9998 };
        //    return msg;
        //}
    }

    private PaymentMessage CreateSepaMessage(BankingOrder t, out BankingOrder ta, out BACSepaMessage aSepaMessage)
    {
        // Auftragssegment erstellen (Einzelüberweisung)
        var oHkccs = _ddbacEbics.CreateTransactionSegment("HKCCS"); // HKCCS = SEPA Einzelüberweisung
        _ddbacEbics.SetSepaAuftraggeberKontoverbindung(oHkccs);

        // Sepa pain Message erstellen 
        // Auftrag und Absender einstellen

        var fromAccount = _ddbacEbics.Account;

        if (fromAccount == null)
        {
            ta = null;
            aSepaMessage = null;
            return new PaymentMessage { ErrorCode = 9000, Message = "No account found" };
        }

        //fromAccount.BIC = fromBic;

        ta = t;

        aSepaMessage = new BACSepaMessage
        {
            Segment = oHkccs,
            FromAccount = fromAccount,
            ExecutionDate = DateTime.Today
        };

        // Neue Order erstellen (eine für Einzelauftrag, mehrere für Sammelüberweisungen)
        var aSepaOrder = aSepaMessage.Orders.Add();

        var endToEndId = ta.Guid.ToPlainString();
        //var endToEndId = $"BMT12345";

        aSepaOrder.EndToEndId = endToEndId;

        var aToAccount = new BACAccount
        {
            CountryCode = string.Empty,
            SepaName = ClearSepaString(ta.PayerOrPayee),
            IBAN = ta.Iban.ToUpper(),
            BIC = ta.Bic,
        };

        if (!string.IsNullOrEmpty(ta.Bic))
        {
            aToAccount.BIC = ta.Bic;
        }

        aSepaOrder.ToAccount = aToAccount;
        aSepaOrder.Amount = ta.Total;
        aSepaOrder.CurrencyCode = string.IsNullOrEmpty(ta.Currency) ? "EUR" : ta.Currency;
        aSepaOrder.Purpose = ClearSepaString(ta.Description);

        // Gegen das Schema verifizieren
        var sErrorReason = string.Empty;
        aSepaMessage.Verify(ref sErrorReason);
        if (string.IsNullOrEmpty(sErrorReason))
        {
            return null;
        }
        var msg = new PaymentMessage { Message = sErrorReason, ErrorCode = 9999, ErrorCode1 = 9999 };
        ta.Result = msg;
        return msg;

    }

    /// <summary>
    /// Download a PTK message
    /// </summary>
    /// <returns>PTK message</returns>
    public override string DownloadPtk()
    {
        var startDate = DateTime.Today.AddDays(-1);
        var endDate = startDate.AddHours(23).AddMinutes(59).AddSeconds(59);

        if (_ddbacEbics.Account == null)
        {
            Error = 77;
            return string.Empty;
        }

        var downParam = new BACStandardOrderParams
        {
            OrderType = "PTK",
            OrderAttribute = "DZHNN",
        };

        var erg = _ddbacEbics.DownloadData(startDate, endDate, downParam);

        return erg ?? string.Empty;
    }

    /// <summary>
    /// Download Auftragsart VPZ (VOP)
    /// </summary>
    /// <param name="result">List with VOP processing relevante data</param>
    /// <returns>Payment message</returns>
    public override PaymentMessage DownloadVpz(List<VopProcessingData> result)
    {
        var pm = new PaymentMessage();

        if (_ddbacEbics.Account == null)
        {
            Error = 77;
            pm.ErrorCode = 77;
            pm.Message = "No account found";
            return pm;
        }

        var startDate = DateTime.Today.AddDays(-1);
        var endDate = startDate.AddHours(23).AddMinutes(59).AddSeconds(59);

        // Get PAIN.002 XML
        var downParam = new BACStandardOrderParams3
        {
            OrderType = "BTD",
            ServiceName = "REP",
            Scope = "DE",
            ServiceOption = "VOP",
            MsgName = "pain.002",
            ContainerType = "ZIP",
            StartDate = startDate,
            EndDate = endDate
        };

        var erg = _ddbacEbics.DownloadByteData(downParam);

        if (erg.Count == 0)
        {
            pm.ErrorCode = 78;
            pm.Message = "No data returned";
            return pm;
        }

        var reports = GetSepaReport(erg);

        foreach (var report in reports)
        {
            CheckSepaReport(report, result);

        }

        return pm;
    }

    private void CheckSepaReport(BACSepaReport report, List<VopProcessingData> result)
    {
        foreach (var order in report.Orders)
        {
            var vop = result.FirstOrDefault( x=> x.EndToEndId == order.OrgnlEndToEndId);

            if (vop == null)
            {
                continue;
            }

            vop.State = order.TxSts;
            vop.CreditorName = order.StsRsnAddtlInf;
        }
    }

    private List<BACSepaReport> GetSepaReport(List<byte[]> erg)
    {
        var result = new List<BACSepaReport>();

        foreach (var unzippedData in erg)
        {
            var dataObject = new BACDataObject();
            dataObject.PutData(unzippedData);

            var sepaReport = new BACSepaReport();
            sepaReport.Load(dataObject);

            result.Add(sepaReport);
        }

        return result;
    }

    /// <summary>
    /// Download Auftragsart HVD (VEU)
    /// </summary>
    public override PaymentMessage DownloadHvd(VopProcessingData data)
    {
        var request = new BACHVDRequest();

        var pm = new PaymentMessage();

        BACHVDResponseOrderData response = null;

        try
        {
            request.OrderParams.HVDPartnerID = data.PartnerId;
            request.OrderParams.HVDOrderType = data.OrderType;
            request.OrderParams.HVDOrderID = data.OrderId;

            response = request.SendRequest(_ddbacEbics.Contact, _ddbacEbics.SecurityMedium);

            data.DataDigest = response.DataDigest;

            pm.ErrorCode = 0;
            pm.ErrorCode1 = 0;
            pm.Message = "HVD request successful";

        }
        catch (Exception ex)
        {
            pm.ErrorCode = 9090;
            pm.ErrorCode1 = Convert.ToInt32(response?.ReturnCode);
            pm.Message = ex.ToString();
        }

        return pm;
    }

    /// <summary>
    /// Download Auftragsart HVU (VEU)
    /// </summary>
    /// <param name="result">List of order data</param>
    /// <returns>Payment message</returns>
    public override PaymentMessage DownloadHvu(List<VopProcessingData> result)
    {
        var pm = new PaymentMessage();
        BACHVUResponseOrderData response = null;

        try
        {
            var request = new BACHVURequest();

            //if (!String.IsNullOrEmpty(tb_ordertypes.Text))
            //{
            //    foreach (string orderType in tb_ordertypes.Text.Split(' '))
            //    {
            //        request.OrderParams.OrderTypes.Add(orderType);
            //    }
            //}

            response = request.SendRequest(_ddbacEbics.Contact, _ddbacEbics.SecurityMedium);

            if (response == null)
            {
                pm.ErrorCode = 9091;
                pm.Message = "HVU request failed";
                return pm;
            }

            pm.ErrorCode = response.ReturnCode == "0" ? 0 : 9093;
            pm.ErrorCode1 = Convert.ToInt32(response.ReturnCode);
            pm.Message = $"RC {response.ReturnCode} TRC{response.TechnicalReturnCode} {response.TechnicalReportText}";

            foreach (var orderdetail in response.HVUOrderDetails)
            {
                // Find the order now and add required data
                var hvu = result.FirstOrDefault(x => x.OrderId == orderdetail.OrderID);

                if (hvu == null)
                {
                    continue;
                }

                hvu.OrderType = orderdetail.OrderType;
                hvu.PartnerId = orderdetail.OriginatorInfo.PartnerID;

                //var hvu = new VopProcessingData
                //{
                //    OrderId = orderdetail.OrderID,
                //    OrderType = orderdetail.OrderType,
                //    PartnerId = orderdetail.OriginatorInfo.PartnerID
                //};
                //result.Add(hvu);
            }
        }
        catch (Exception ex)
        {
            pm.ErrorCode = 9092;
            pm.ErrorCode1 = Convert.ToInt32(response?.ReturnCode);
            pm.Message = ex.ToString();
        }

        return pm;
    }

    /// <summary>
    /// Send HVE to bank to confirm money transfer
    /// </summary>
    /// <param name="data">Order data</param>
    /// <returns>Payment message</returns>
    public override PaymentMessage UploadHve(VopProcessingData data)
    {
        var request = new BACHVERequest();
        var pm = new PaymentMessage();
        BACUploadResponse response = null;

        try
        {
            request.OrderParams.HVEPartnerID = data.PartnerId;
            request.OrderParams.HVEOrderType = data.OrderType;
            request.OrderParams.HVEOrderID = data.OrderId;

            if (data.DataDigest != null)
            {
                request.OrderData.DataHash = Convert.FromBase64String(data.DataDigest);
            }
            else
            {
                pm.ErrorCode = 0;
                pm.ErrorCode1 = 9005;
                pm.Message = "HVE request: no order data available";
                return pm;
            }

            response = request.SendRequest(_ddbacEbics.Contact, _ddbacEbics.SecurityMedium);

            if (response.ReturnCode == "0")
            {
                pm.ErrorCode = 0;
                pm.Message = "HVE request sucessful: elektronische Unterschrift wurde hinzugefügt";
                return pm;
            }

            pm.ErrorCode = response.ReturnCode == "0" ? 0 : 9095;
            pm.ErrorCode1 = Convert.ToInt32(response.ReturnCode);
            pm.Message = $"HVE request: RC {response.ReturnCode} TRC{response.TechnicalReturnCode} {response.TechnicalReportText}";
        }
        catch (Exception err)
        {
            pm.ErrorCode = 9094;

            if (response == null)
            {
                pm.Message = err.ToString();
            }
            else
            {
                pm.ErrorCode1 = Convert.ToInt32(response.ReturnCode);
                pm.Message = $"RC {response.ReturnCode} TRC{response.TechnicalReturnCode} {response.TechnicalReportText} {err}";
            }
        }

        return pm;
    }

    /// <summary>
    /// Send HVS to bank to cancel (Storno) money transfer
    /// </summary>
    /// <param name="data">Order data</param>
    /// <returns>Payment message</returns>
    public override PaymentMessage UploadHvs(VopProcessingData data)
    {
        var request = new BACHVSRequest();
        var pm = new PaymentMessage();
        BACUploadResponse response = null;

        try
        {
            request.OrderParams.HVSPartnerID = data.PartnerId;
            request.OrderParams.HVSOrderType = data.OrderType;
            request.OrderParams.HVSOrderID = data.OrderId;

            if (data.DataDigest != null)
            {
                request.OrderData.DataHash = Convert.FromBase64String(data.DataDigest);
            }
            else
            {
                //MessageBox.Show(this, "Keine Auftragsdaten vorhanden!", frmMain.Form.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                pm.ErrorCode = 0;
                pm.ErrorCode1 = 9005;
                pm.Message = "HVS request: no order data available";
                return pm;
            }

            response = request.SendRequest(_ddbacEbics.Contact, _ddbacEbics.SecurityMedium);

            if (response == null || response.ReturnCode == "0")
            {
                pm.ErrorCode = 0;
                pm.Message = "HVS request sucessful";
                return pm;
            }
            pm.ErrorCode = 9094;
            pm.ErrorCode1 = Convert.ToInt32(response.ReturnCode);
            pm.Message = $"HVS request: RC {response.ReturnCode} TRC{response.TechnicalReturnCode} {response.TechnicalReportText}";
        }
        catch (Exception err)
        {
            pm.ErrorCode = 9094;

            if (response == null)
            {
                pm.Message = err.ToString();
            }
            else
            {
                pm.ErrorCode1 = Convert.ToInt32(response.ReturnCode);
                pm.Message = $"HVS request: RC{response.ReturnCode} TRC{response.TechnicalReturnCode} {response.TechnicalReportText} {err}";
            }
        }

        return pm;
    }

    /// <summary>
    /// Get account statement for the last year (or at least as long as possible)
    /// </summary>
    public override AccountStatement GetAccountStatement()
    {
        return GetAccountStatement(DateTime.Now.AddYears(-1), DateTime.Now);
    }

    /// <summary>
    /// Get the account statment for a defined period
    /// </summary>
    /// <param name="startDate">start date</param>
    /// <param name="endDate">end date</param>
    /// <returns>list of account statement rows</returns>
    public override AccountStatement GetAccountStatement(DateTime startDate, DateTime endDate)
    {

        var downParam = new BACStandardOrderParams3
        {
            OrderType = "BTD",
            ServiceName = "EOP",
            Scope = "DE",
            MsgName = "camt.053",
            ContainerType = "ZIP",
            StartDate = startDate,
            EndDate = endDate,
        };

        var erg = _ddbacEbics.DownloadDataStatements(startDate, endDate, downParam);

        //var dataObject = new BACDataObject();
        //dataObject.ReadFromObject(erg);

        Debug.Print(erg);

        Encoding utf8WithoutBom = new UTF8Encoding(false);

        var bytes = utf8WithoutBom.GetBytes(erg);

        var statement = new BACCamtStatement();
        statement.ParseCamt(bytes, 0, bytes.Length);

        //if (File.Exists("d:\\TMP\\C53.zip")) File.Delete("d:\\TMP\\C53.zip");

        //using (var fileStream = File.Create("d:\\TMP\\C53.zip"))
        //{
        //    //erg.Seek(0, SeekOrigin.Begin);
        //    erg.CopyTo(fileStream);
        //}

        var reader = new StreamReader(erg, Encoding.GetEncoding("ibm852"));
        var text = reader.ReadToEnd();

        Debug.Print(text);

        var data = text.Split(["\r\n-\r\n"], StringSplitOptions.RemoveEmptyEntries);

        foreach (var mt940String in data)
        {
            //Debug.Print(mt940String);


            var array = Encoding.GetEncoding("ibm852").GetBytes(mt940String);

            var mt940 = new BACSwiftStatement();


            mt940.Load(array);

            //mt940.SaveCopyAs($@"D:\tmp\amt940_{mt940.AccountNumber}.sta");



            foreach (var x in mt940.StatementLines)
            {
                Debug.Print($"{mt940.AccountNumber} : {x.AccountNumber}: {x.Amount}");
            }
        }

        return null;
    }

    /// <summary>
    /// Get account statement and save it as MT940 file with sta extension. 
    /// Additonally create PDF file with account statement
    /// Extension is added automatically with not existing
    /// </summary>
    /// <param name="startDate">start date</param>
    /// <param name="endDate">end date</param>
    /// <param name="targetFile">full path of the target PDF file</param>
    /// <param name="pdf">create PDF: true or false</param>
    public override bool GetAccountStatementToMt940File(DateTime startDate, DateTime endDate, string targetFile, bool pdf)
    {
        throw new NotImplementedException();


        //if (Error != 0) return true;

        //if (!targetFile.EndsWith(".sta")) targetFile += ".sta";


        //var mt940 = GetMt940(startDate, endDate);
        //if (mt940 == null) return true;

        //mt940.SaveCopyAs(targetFile);

        //// DDBAC-Version: no entry date
        ////oMt940.SaveAs(targetFile);

        //if (!pdf) return false;

        //var pdfFile = targetFile.Replace(".sta", ".pdf");

        //var header = new AccountStatementHeader
        //{
        //    AccountOwner = _ddbacEbics.Account.NameEins,
        //    AccountNr = AccountNr,
        //    StartDate = startDate,
        //    EndDate = endDate
        //};

        //CreateAccountStatementAsPdf(ref mt940, pdfFile, header);

        //return false;
    }

    /// <summary>
    /// Quit banking module
    /// </summary>
    public override void Quit()
    {
        _ddbacEbics.ReleaseDialog();
    }

    /// <summary>
    /// Get the balance of an account
    /// </summary>
    /// <returns>balance amount</returns>
    public override decimal GetBalance()
    {
        return decimal.MinValue;

        ////if (_ddbacEbics.Account == null)
        ////{
        ////    Error = 77;
        ////    return 0;
        ////}

        ////try
        ////{
        ////    var startDate = DateTime.Now.AddMonths(-2);
        ////    var endDate = DateTime.Now;

        ////    var erg = _ddbacEbics.DownloadData(startDate, endDate, "STA");


        ////    var mt940 = new BACSwiftStatementClass();
        ////    mt940.Load(erg);

        ////    mt940.SaveCopyAs("D:\\tmp\\mt940.txt");

        ////    return mt940.ClosingBalanceAmount;
        ////}
        ////catch
        ////{
        ////    return 0;
        ////}
    }


    /// <summary>
    /// Get the owner of the account 
    /// </summary>
    /// <returns>Account owner's name</returns>
    public override string GetOwner()
    {
        return _ddbacEbics.Account.SepaName.Trim();
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Quit();
        }
    }

    /// <summary>
    /// Show the administration window for HBCI accounts
    /// </summary>
    public override void ShowAdministrationWindow()
    {
        _ddbacEbics.Banking.RunContactsCPL();
    }
}