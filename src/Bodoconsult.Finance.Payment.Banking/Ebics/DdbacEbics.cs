// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.Finance.Payment.Banking.Base;
using Bodoconsult.Finance.Payment.Base.Models;
using DDBAC.Net;
using DDBAC.Net.Ebics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Bodoconsult.Finance.Payment.Banking.Ebics;

/// <summary>
/// Ddbac helper class for EBICS
/// </summary>
public class DdbacEbics : DdbacBase
{
    /// <summary>
    /// Current security medium
    /// </summary>
    public IBACSecurity SecurityMedium;

    /// <summary>
    /// Open a contact
    /// </summary>
    /// <param name="contactName">name of the banking contact to open</param>
    /// <param name="passphrase"></param>
    public void OpenContact(string contactName, string passphrase)
    {

        SendState("Opening contact '{0}'...", contactName);

        var aContacts = new BACContacts();
        aContacts.Populate(string.Empty);

        SendState("Populating contacts done!");

        foreach (var contact in aContacts)
        {
            if (!string.Equals(contact.Name, contactName, StringComparison.CurrentCultureIgnoreCase))
            {
                continue;
            }

            Contact = contact;
            SecurityMedium = Contact.Authenticate(new BACSecureString(passphrase));
            break;
        }

        SendState("Opening contact done!");
    }

    /// <summary>
    /// Diese Funktion erzeugt ein Segment Objekt für einen gegebenen Geschäftsvorfall. Dabei wird die neueste GV Version die sowohl
    /// vom HBCI Zielsystem als auch von der aufrufenden Anwendung unterstützt werden ermittelt. Wird keine Schnittmenge gefunden,
    /// (meist dann der Fall, wenn der gewünschte Geschäftsvorfall nicht vom HBCI Zielsystem angeboten wird) so wird eine
    /// entsprechende Fehlermeldung ausgelöst.
    /// </summary>
    /// <param name="sSegmentType">sSegmentType gibt den gewünschten Geschäftsvorfall, z.B. "HKUEB" oder "HKSAL" an</param>
    /// <returns>Segment</returns>
    public BACSegment CreateTransactionSegment(string sSegmentType)
    {
        var oSegment = Account.FindOptimalBPDSegment(sSegmentType, 0);

        // Fehlermeldung falls kein BPD Segment vorhanden
        if (oSegment == null)
        {
            throw new Exception("Dieser Geschäftsvorfall wird vom HBCI Banksystem nicht unterstützt.");
        }

        var lSegmentVersion = oSegment.Version;

        // Jetzt ist die Segmentkennung und die optimale Segmentversion bekannt,
        // so dass das Auftragssegment kreiert werden kann.
        oSegment = Banking.NewSegment(sSegmentType, lSegmentVersion);

        return oSegment;
    }

    /// <summary>
    /// Setzt die Kontoverbindungsdaten des Auftraggebers in ein
    /// Auftragssegment. Dabei wird der HISPAS berücksichtigt und die
    /// nationale Kontoverbindung wird nur eingetragen, wenn es erlaubt ist.
    /// </summary>
    /// <param name="oSegment"></param>
    public void SetSepaAuftraggeberKontoverbindung(BACSegment oSegment)
    {
        oSegment["AuftraggeberKontoverbindung1", "IBAN1"] = Account.IBAN;
        oSegment["AuftraggeberKontoverbindung1", "BIC1"] = Account.BIC;
    }

    ///// <summary>
    ///// Transfer data to the bank via webrequest
    ///// </summary>
    ///// <param name="dataStream">stream with data to transfer to the bank</param>
    ///// <param name="orderType">order type (must be permitted by the bank)</param>
    ///// <param name="orderAttribute">attributes for the request. Default: OZHNN</param>
    ///// <returns>a message with information regarding success of the transaction</returns>
    //public PaymentMessage UploadData(Stream dataStream, string orderType, string orderAttribute = "OZHNN")
    //{
    //    try
    //    {

    //        SendStatus("Send upload web request to bank...");

    //        var uploadOrder = new BACUploadOrder();

    //        var upParam = new BACStandardOrderParams
    //        {
    //            OrderType = orderType,
    //            OrderAttribute = orderAttribute
    //        };

    //        //dataStream.Position = 0;

    //        var orderData = new BACRequestOrderData { Data = dataStream };


    //        BACResponse response = uploadOrder.SendRequest(Contact, SecurityMedium, upParam, orderData);
    //        var returnState = response.CheckResponse();

    //        if (ReturnCodes.RETURNCODE_OK == returnState)
    //        {
    //            SendStatus("Web request successful");

    //            return new PaymentMessage
    //            {
    //                ErrorCode = 0,
    //                Message = "EBICS: file transfer successfull!",
    //                TransactionId = response.TransactionID
    //            };

    //        }

    //        var message = response.TechnicalReportText != null ? $"{response.TechnicalReportText} ({response.TechnicalReturnCode})" : response.ReturnCode;

    //        var errorText = Properties.Resources.ResourceManager.GetObject($"_{response.ReturnCode}");
    //        if (errorText != null)
    //        {
    //            message = $"{errorText} ({message})";
    //        }

    //        SendStatus("Web request NOT successfull: {0}", message);

    //        return new PaymentMessage
    //        {
    //            ErrorCode = 9000,
    //            Message = $"EBICS: File was transferred to bank successfully but quitted with a message: \"{message}",
    //            TransactionId = response.TransactionID
    //        };

    //    }
    //    catch (Exception err)
    //    {
    //        return new PaymentMessage
    //        {
    //            ErrorCode = 9900,
    //            Message = $"EBICS: Error during file transfer: \"{err.Message}"
    //        };
    //    }
    //}

    /// <summary>
    /// Transfer data to the bank via webrequest
    /// </summary>
    /// <param name="fileName">path of a file with data to transfer to the bank</param>
    /// <param name="orderType">order type (must be permitted by the bank)</param>
    /// <param name="orderAttribute">attributes for the request. Default: OZHNN</param>
    /// <returns>a message with information regarding success of the transaction</returns>
    public PaymentMessage UploadData1(string fileName, string orderType, string orderAttribute = "OZHNN")
    {
        try
        {

            var uploadOrder = new BACUploadOrder();

            var upParam = new BACStandardOrderParams
            {
                OrderType = orderType,
                OrderAttribute = orderAttribute
            };

            SendState("Send upload web request to bank...");

            using var dataStream = File.OpenRead(fileName);
            var orderData = new BACRequestOrderData { Data = dataStream };

            BACResponse response = uploadOrder.SendRequest(Contact, SecurityMedium, upParam, orderData);
            var returnState = response.CheckResponse();

            if (ReturnCodes.RETURNCODE_OK == returnState)
            {
                SendState("Web request successful");

                return new PaymentMessage
                {
                    ErrorCode = 0,
                    Message = "EBICS: file transfer succesfull!",
                    TransactionId = response.TransactionID
                };
            }

            var message = response.TechnicalReportText != null
                ? $"{response.TechnicalReportText} ({response.TechnicalReturnCode})"
                : response.ReturnCode;

            var errorText = Properties.Resources.ResourceManager.GetObject($"_{response.ReturnCode}");
            if (errorText != null)
            {
                message = $"{errorText} ({message})";
            }

            SendState("Web request NOT successfull: {0}", message);

            return new PaymentMessage
            {
                ErrorCode = 9000,
                Message = $"EBICS: File was transferred to bank successfully but quitted with the following message: \"{message}",
                TransactionId = response.TransactionID
            };
        }
        catch (Exception err)
        {
            return new PaymentMessage
            {
                ErrorCode = 9900,
                Message = $"EBICS: Error during file transfer: \"{err.Message}"
            };
        }
    }

    /// <summary>
    /// Transfer data to the bank via webrequest
    /// </summary>
    /// <param name="content">path of a file with data to transfer to the bank</param>
    /// <param name="orderType">order type (must be permitted by the bank)</param>
    /// <param name="orderAttribute">attributes for the request. Default: OZHNN</param>
    /// <returns>a message with information regarding success of the transaction</returns>
    public PaymentMessage UploadData(string content, string orderType, string orderAttribute = "OZHNN")
    {
        try
        {
            var uploadOrder = new BACUploadOrder();

            var upParam = new BACStandardOrderParams
            {
                OrderType = orderType,
                OrderAttribute = orderAttribute
            };

            SendState("Send upload web request to bank...");

            // Not working: Encoding.UTF8 (which adds BOM)
            var utf8WithoutBom = new UTF8Encoding(false);
            var bytes = utf8WithoutBom.GetBytes(content);


            using var dataStream = new MemoryStream(bytes);
            var orderData = new BACRequestOrderData { Data = dataStream };

            BACResponse response = uploadOrder.SendRequest(Contact, SecurityMedium, upParam, orderData);
            var returnState = response.CheckResponse();

            if (ReturnCodes.RETURNCODE_OK == returnState)
            {
                SendState("Web request successful");

                return new PaymentMessage
                {
                    ErrorCode = 0,
                    Message = "EBICS: file transfer successfull!",
                    TransactionId = response.TransactionID
                };
            }

            var message = response.TechnicalReportText != null
                ? $"{response.TechnicalReportText} ({response.TechnicalReturnCode})"
                : response.ReturnCode;

            var errorText = Properties.Resources.ResourceManager.GetObject($"_{response.ReturnCode}");
            if (errorText != null)
            {
                message = $"{errorText} ({message})";
            }

            SendState("Web request NOT successfull: {0}", message);

            return new PaymentMessage
            {
                ErrorCode = 9000,
                Message =
                    $"EBICS: File was transferred to bank successfully but quitted with the following message: \"{message}",
                TransactionId = response.TransactionID
            };
        }
        catch (Exception err)
        {
            return new PaymentMessage
            {
                ErrorCode = 9900,
                Message = $"EBICS: Error during file transfer: \"{err.Message}"
            };
        }
    }

    /// <summary>
    /// Transfer data to the bank via webrequest
    /// </summary>
    /// <param name="content">path of a file with data to transfer to the bank</param>
    /// <param name="orderType">order type (must be permitted by the bank)</param>
    /// <param name="upParam"></param>
    /// <returns>a message with information regarding success of the transaction</returns>
    public PaymentMessage UploadData(string content, string orderType, IBACOrderParams upParam)
    {
        try
        {
            var uploadOrder = new BACUploadOrder();

            SendState("Send upload web request to bank...");

            // Not working: Encoding.UTF8 (which adds BOM)
            var utf8WithoutBom = new UTF8Encoding(false);
            var bytes = utf8WithoutBom.GetBytes(content);


            using var dataStream = new MemoryStream(bytes);
            var orderData = new BACRequestOrderData { Data = dataStream };

            BACResponse response = uploadOrder.SendRequest(Contact, SecurityMedium, upParam, orderData);
            var returnState = response.CheckResponse();

            if (ReturnCodes.RETURNCODE_OK == returnState)
            {
                SendState("Web request successful");

                return new PaymentMessage
                {
                    ErrorCode = 0,
                    Message = "EBICS: file transfer succesfull!",
                    TransactionId = response.TransactionID
                };
            }

            var message = response.TechnicalReportText != null
                ? $"{response.TechnicalReportText} ({response.TechnicalReturnCode})"
                : response.ReturnCode;

            var errorText = Properties.Resources.ResourceManager.GetObject($"_{response.ReturnCode}");
            if (errorText != null)
            {
                message = $"{errorText} ({message})";
            }

            SendState("Web request NOT successfull: {0}", message);

            return new PaymentMessage
            {
                ErrorCode = 9000,
                Message = $"EBICS: File was transferred to bank successfully but quitted with the following message: \"{message}",
                TransactionId = response.TransactionID
            };
        }
        catch (Exception err)
        {
            return new PaymentMessage
            {
                ErrorCode = 9900,
                Message = $"EBICS: Error during file transfer: \"{err.Message}"
            };
        }
    }

    internal string DownloadData(DateTime start, DateTime end, IBACOrderParams downParam)
    {
        //throw new NotImplementedException();

        //try
        //{

        //var downParam = new BACStandardOrderParams3
        //{
        //    OrderType = "BTD",
        //    ServiceName = "REP",
        //    Scope = "DE",
        //    ServiceOption = "VOP",
        //    MsgName = "pain.002",
        //    ContainerType = "ZIP"
        //};

        //BACResponse response = downloadOrder.SendRequest(contact, security, downParam, null);

        //var downParam = new BACStandardOrderParams
        //{
        //    OrderType = orderType,
        //    OrderAttribute = orderAttribute,
        //    StartDate = start,
        //    EndDate = end
        //};

        var downloadOrder = new BACDownloadOrder();

        SendState("Send download web request to bank...");

        var response = downloadOrder.SendRequest(Contact, SecurityMedium, downParam, null);

        if (response.CheckResponse() == ReturnCodes.RETURNCODE_OK)
        {
            SendState("Web request successful!");
            return response.Data;
        }

        var message = new StringBuilder();

        message.Append("Web request not successful: ");

        if (response.TechnicalReportText != null)
        {
            message.Append($"TechnicalReport: {response.TechnicalReportText} ({response.TechnicalReturnCode})");
        }

        var errorText = Properties.Resources.ResourceManager.GetObject($"_{response.ReturnCode}");
        if (errorText != null)
        {
            message.Append($"Error: {errorText}");
        }
        SendState(message.ToString());

        return null;

        //    throw new Exception(message);
        ////}
        ////catch (Exception err)
        ////{
        ////    throw new Exception(err.Message);
        ////}
    }

    /// <summary>
    /// Download data as list of byte arrays
    /// </summary>
    /// <param name="downParam">Download parameters</param>
    /// <returns></returns>
    public List<byte[]> DownloadByteData(IBACOrderParams downParam)
    {
        //throw new NotImplementedException();

        try
        {

            //var downParam = new BACStandardOrderParams3
            //{
            //    OrderType = "BTD",
            //    ServiceName = "REP",
            //    Scope = "DE",
            //    ServiceOption = "VOP",
            //    MsgName = "pain.002",
            //    ContainerType = "ZIP"
            //};

            //BACResponse response = downloadOrder.SendRequest(contact, security, downParam, null);

            //var downParam = new BACStandardOrderParams
            //{
            //    OrderType = orderType,
            //    OrderAttribute = orderAttribute,
            //    StartDate = start,
            //    EndDate = end
            //};

            var downloadOrder = new BACDownloadOrder();

            SendState("Send download web request to bank...");

            var response = downloadOrder.SendRequest(Contact, SecurityMedium, downParam, null);

            if (response.CheckResponse() == ReturnCodes.RETURNCODE_OK)
            {
                SendState("Web request successful!");
                return response.UnzippedDatas;
            }

            var message = new StringBuilder();

            message.Append("Web request not successful: ");

            if (response.TechnicalReportText != null)
            {
                message.Append($"TechnicalReport: {response.TechnicalReportText} ({response.TechnicalReturnCode})");
            }

            var errorText = Properties.Resources.ResourceManager.GetObject($"_{response.ReturnCode}");
            if (errorText != null)
            {
                message.Append($"Error: {errorText}");
            }
            SendState(message.ToString());

            return [];

            //    throw new Exception(message);
        }
        catch (Exception err)
        {
            throw new Exception(err.Message);
        }
    }

    /// <summary>
    /// Download statement data
    /// </summary>
    /// <param name="start">Start date</param>
    /// <param name="end">End date</param>
    /// <param name="downParam">Download parameters</param>
    /// <returns>Statement data as string</returns>
    public string DownloadDataStatements(DateTime start, DateTime end, IBACOrderParams downParam)
    {
        //throw new NotImplementedException();

        //try
        //{

            var downloadOrder = new BACDownloadOrder();

            BACMonitor.Trace(BACMonitorTraceLevel.Debug, "Test", "EBICS");

            SendState("Send download web request to bank...");

            var response = downloadOrder.SendRequest(Contact, SecurityMedium, downParam, null);

            if (response.CheckResponse() == ReturnCodes.RETURNCODE_OK)
            {
#if DEBUG
            File.WriteAllText(@"C:\temp\bsag.txt", response.Data);
#endif
            var result = response.Data;

                var i = result.IndexOf("<", StringComparison.InvariantCultureIgnoreCase);

                result = result[i..];

                i = result.LastIndexOf("</Document>", StringComparison.InvariantCultureIgnoreCase);

                result = result[..(i + 11)];

                Debug.Print(result);

                SendState("Web request successful!");
                return result;
            }

            var message = "Web request not successful: ";

            if (response.TechnicalReportText != null)
            {
                message += $"TechnicalReport: {response.TechnicalReportText} ({response.TechnicalReturnCode})";
            }

            var errorText = Properties.Resources.ResourceManager.GetObject($"_{response.ReturnCode}");
            if (errorText != null)
            {
                message += $"{errorText} ({message})";
            }
            SendState(message);

            return null;

            //    throw new Exception(message);
        //}
        //catch (Exception err)
        //{
        //    throw new Exception(err.Message);
        //}
    }

    /// <summary>
    /// Send a state message
    /// </summary>
    /// <param name="message">State message to send</param>
    public void SendState(string message)
    {
        var pm = new PaymentMessage
        {
            ErrorCode = 0,
            Message = message
        };

        OnHbciMessage?.Invoke(pm);
    }

    /// <summary>
    /// Send a state message
    /// </summary>
    /// <param name="message">State message to send</param>
    /// <param name="args">Additonal arguments</param>
    public void SendState(string message, params object[] args)
    {
        var pm = new PaymentMessage
        {
            ErrorCode = 0,
            Message = string.Format(message, args)
        };

        OnHbciMessage?.Invoke(pm);
    }

    /// <summary>
    /// Show a received HBCI message
    /// </summary>
    /// <param name="message">Current message to show</param>
    public void ShowHbciMessage(PaymentMessage message)
    {
        OnHbciMessage?.Invoke(message);
    }

    /// <summary>
    /// Release the banking dialog
    /// </summary>
    public void ReleaseDialog()
    {
        Contact = null;
        Banking = null;
    }
}