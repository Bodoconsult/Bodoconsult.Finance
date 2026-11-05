// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using System.Text;
using Bodoconsult.Finance.Payment.Banking.Base;
using Bodoconsult.Finance.Payment.Banking.Models;
using Bodoconsult.Finance.Payment.Banking.Services;
using Bodoconsult.Finance.Payment.Base.Models;
using DDBAC.Net;
using DDBAC.Net.FinTS;
using DDBAC.Net.Swift;

namespace Bodoconsult.Finance.Payment.Banking.Hbci;

/// <summary>
/// Banking class for HBCI based banking
/// </summary>
[SupportedOSPlatform("windows")]
public class HbciBanking : BaseBanking
{
    private readonly Ddbac _ddbac = new();

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="productName">Product name as registered with the Bankenverband</param>
    /// <param name="productVersion">Current product version</param>
    public HbciBanking(string productName, string productVersion)
    {
        _ddbac.ProductName = productName;
        _ddbac.ProductVersion = productVersion;

        UserName = null;
        AccountNr = null;
        Passphrase = null;
        Contact = null;
        ErrorMessage = null;
        Error = 0;
        _ddbac.Init();
    }

    /// <summary>
    /// Open connection to the bank
    /// </summary>
    public override PaymentMessage OpenAccount()
    {

        //throw new NotSupportedException("Only SEPA accounts support!");

        try
        {
            if (Error != 0)
            {
                return null;
            }

            // Neuen Dialog öffnen
            _ddbac.NewDialog(Contact, Passphrase);

            _ddbac.SelectAccount(AccountNr);

            // 
            if (_ddbac.Dialog == null)
            {
                ErrorMessage = "Opening accout not possible. No connection possible: Account blocked? Wrong PIN?";
                Error = 18;
                return HbciError(ErrorMessage);
            }

            var oMessage = _ddbac.Dialog.BeginDialog(BACDialogTypes.bacDialogStandard, _ddbac.CustomerId, null);
            _ddbac.ReportResult(oMessage, "Open account");
            if (_ddbac.Dialog.State != BACDialogStates.bacDialogReady)
            {
                _ddbac.ReleaseDialog();
            }

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
        try
        {
            if (Error != 0)
            {
                return null;
            }

            // Neuen Dialog öffnen
            _ddbac.NewDialog(Contact, Passphrase);
            _ddbac.SelectAccountIban(Iban);

            // 
            if (_ddbac.Dialog == null)
            {
                ErrorMessage = "Opening accout not possible. No connection possible: Account blocked? Wrong PIN?";
                Error = 18;
                return HbciError(ErrorMessage);
            }

            // Specify Version a Language
            if (_ddbac.HbciVersion != 0)
            {
                _ddbac.Dialog.HBCIVersion = _ddbac.HbciVersion;
            }
            _ddbac.Dialog.Language = _ddbac.Language;

            var oMessage = _ddbac.Dialog.BeginDialog(BACDialogTypes.bacDialogStandard, _ddbac.CustomerId, null);
            _ddbac.ReportResult(oMessage, "Open SEPA account");
            if (_ddbac.Dialog is { HBCIResultCode: >= 9000 })
            {
                // Fehler beim Anmelden, Dialog zurücksetzten
                _ddbac.ReleaseDialog();
            }
            return null;
        }
        catch (Exception ex)
        {
            return NoHbciError(ex);
        }
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
        var accountStatement = new AccountStatement();

        if (_ddbac.Account is null)
        {
            Error = 4711;
            return accountStatement;
        }

        if (Error != 0)
        {
            return accountStatement;
        }

        var aParam = new AcctMvmtsSpecifiedPeriodCamt.Parameter(_ddbac.Account.FindOptimalBPDSegment(AcctMvmtsSpecifiedPeriodCamt.Parameter.HBCICode));
        var sCamtDescriptor = aParam.AcctMvmtsSpecifiedPeriodCamt.Supported_Camt_DataFormats[0] as string;

        // Auftrag erstellen
        var aOrder = new AcctMvmtsSpecifiedPeriodCamt(_ddbac.Account);

        aOrder.InsertOrderingAccount(_ddbac.Account);

        aOrder.CamtDescriptors.New.CamtDescriptor =
            sCamtDescriptor; // "urn:iso:std:iso:20022:tech:xsd:camt.052.001.02";

        // optionale Parameter übergeben
        aOrder.All_Acct = false;
        aOrder.StartDate = startDate;
        aOrder.EndDate = endDate;


        // Auftrag ausführen
        var fSuccess = ExecuteOrder(aOrder);
        if (!fSuccess)
        {
            return accountStatement;
        }

        var response = aOrder.getResponse;

        // Antwort der Bank auswerten
        if (response == null)
        {
            return accountStatement;
        }

        // Konto
        accountStatement.AccountNumber = response.BookedTrans.AccountIdentificationCode;

        // Anfangssaldo
        accountStatement.OpeningBalanceAmount = response.BookedTrans.OpeningBalanceAmount;
        accountStatement.OpeningBalanceDate = response.BookedTrans.OpeningBalanceDate;
        accountStatement.OpeningBalanceCurrency = response.BookedTrans.OpeningBalanceCurrencyCode;

        // Endsaldo
        accountStatement.ClosingBalanceAmount = response.BookedTrans.ClosingBalanceAmount;
        accountStatement.ClosingBalanceDate = response.BookedTrans.ClosingBalanceDate;
        accountStatement.ClosingBalanceCurrency = response.BookedTrans.ClosingBalanceCurrencyCode;

        // gebuchte Aufträge eintragen
        foreach (var aLine in response.BookedTrans.StatementLines)
        {
            WriteStatementLine(aLine, accountStatement);
        }

        // eventuell nicht gebuchte Aufträge eintragen
        if (response.NonbookedTrans == null
            || response.NonbookedTrans.StatementLines.Count <= 0)
        {
            return accountStatement;
        }

        // vorgemerkte Aufträge eintragen
        foreach (var aLine in response.NonbookedTrans.StatementLines)
        {
            WriteStatementLine(aLine, accountStatement);
        }

        return accountStatement;
    }

    /// <summary>
    /// Eine Auszugszeile in die Liste eintragen
    /// </summary>
    /// <param name="aLine">Account statement line</param>
    /// <param name="erg">List of account statement items</param>
    private static void WriteStatementLine(BACSwiftStatementLine aLine, AccountStatement erg)
    {
        var aItem = new AccountStatementLine
        {
            EntryDate = aLine.EntryDate,
            ValidityDate = aLine.ValidityDate,
            BusinessTransactionText = aLine.BusinessTransactionText,
            CurrencyCode = aLine.OriginalCurrencyCode,
            OriginalAmount = (double)aLine.OriginalAmount,
            AccountNo = aLine.AccountNumber,
            Amount = aLine.Amount,
            Transactionid = aLine.TransactionId,
            TransactionType = aLine.TransactionType,
            PrimaNota = aLine.PrimaNoteNumber,
            Name = string.IsNullOrEmpty(aLine.Name1 + aLine.Name2) ? "Bank" : aLine.Name1 + aLine.Name2
        };


        // Das Wertstellungsdatum wird nunmehr über das neue Property
        // RealValidityDate ausgelesen und dargestellt. Dies ist erforderlich, da als
        // Wertstellungsdatum auch nicht existierende Tage wie der 30. Februar
        // auftauchen können.
        var sRealValidityDate = "        ";

        if (aLine.RealValidityDate != null)
        {
            sRealValidityDate = aLine.RealValidityDate.Substring(4, 2);
            sRealValidityDate += ".";
            sRealValidityDate += aLine.RealValidityDate.Substring(2, 2);
            sRealValidityDate += ".";
            sRealValidityDate += aLine.RealValidityDate.Substring(0, 2);

            if (sRealValidityDate.StartsWith("30.02."))
            {
                sRealValidityDate = $"28.02.{aLine.RealValidityDate.Substring(0, 2)}";
            }
        }
        aItem.ValidityDate = Convert.ToDateTime(sRealValidityDate);


        // Verwendungszweck
        var x = new StringBuilder();

        foreach (var sPurpose in aLine.PurposeLines)
        {
            x.Append(sPurpose);
        }

        aItem.Purpose = x.ToString();

        // IBAN und BIC
        var iban = FindIban(aLine.IBAN, aLine.Purpose);
        var bic = FindBic(aLine.BIC, aLine.Purpose);

        iban ??= aLine.AccountNumber;
        bic ??= aLine.BankCode;

        aItem.AccountNo = iban;
        aItem.BankCode = bic;


        erg.Items.Add(aItem);
    }

    /// <summary>
    /// Find the BIC
    /// </summary>
    /// <param name="bic">String with a BIC in it</param>
    /// <param name="purpose">Purpose</param>
    /// <returns>BIC</returns>
    protected static string FindBic(string bic, string purpose)
    {
        if (!string.IsNullOrEmpty(bic) || string.IsNullOrEmpty(purpose)) return bic;

        purpose = purpose.Replace("\r\n", string.Empty);

        var i = purpose.IndexOf("BIC: ", StringComparison.OrdinalIgnoreCase);

        if (i < 0)
        {
            return bic;
        }

        var j = purpose.IndexOf(" ", i + 5, StringComparison.OrdinalIgnoreCase);

        if (j == -1)
        {
            j = purpose.Length;
        }

        return purpose.Substring(i + 5, j - i - 5);
    }

    /// <summary>
    /// Find a IBAN
    /// </summary>
    /// <param name="iban">String with an IBAN</param>
    /// <param name="purpose">Purpose</param>
    /// <returns>IBAN</returns>
    protected static string FindIban(string iban, string purpose)
    {
        if (!string.IsNullOrEmpty(iban) || string.IsNullOrEmpty(purpose))
        {
            return iban;
        }

        purpose = purpose.Replace("\r\n", string.Empty);

        var i = purpose.IndexOf("IBAN: ", StringComparison.OrdinalIgnoreCase);

        if (i < 0) return iban;

        var j = purpose.IndexOf(" ", i + 6, StringComparison.OrdinalIgnoreCase);
        var x = purpose.IndexOf("BIC:", i + 6, StringComparison.OrdinalIgnoreCase);

        if (x < j) j = x;

        if (j == -1) j = purpose.Length;

        return purpose.Substring(i + 6, j - i - 6);

    }

    /// <summary>
    /// Execute an order
    /// </summary>
    /// <param name="aOrder">Order to execute</param>
    /// <returns>True if the order was successful else false</returns>
    public bool ExecuteOrder(BACTransactionBase aOrder)
    {
        var aStart = DateTime.Now;
        var sErrorText = string.Empty;

        var fSuccess = false;

        try
        {

            //var aTicket = BACBanking.Ticket ?? new BACTicket();

            //if (CurrentDialog == null)
            //{
            //    CurrentDialog = new BACDialog(frmMain.SelectedContact, null, frmMain.SecurityMedium, aTicket);
            //}
            //_ddbac.Dialog.ParentHandle = aParent.Handle;

            //if (!Properties.Settings.Default.CloseDialog)
            //{
            //    // If the dialog is opened in advance, it will not be opened in ExecuteOrder and the dialog
            //    // will not be ended in ExecuteOrder
            //    if (_ddbac.Dialog.State != BACDialogStates.bacDialogReady)
            //    {
            //        _ddbac.Dialog.BeginDialog(BACDialogTypes.bacDialogStandard, SelectedAccount.CustomerID, null);
            //        CheckIfBeginDialogWasSuccessful();
            //    }
            //}

            _ddbac.Dialog.ExecuteOrder(aOrder);
            //if (Properties.Settings.Default.CloseDialog)
            //{
            //    // Dialog was opend in Execute order
            //    CheckIfBeginDialogWasSuccessful();
            //}

            //fWasLastTransactionAsynchron = CurrentDialog.WasLastTransactionAsynchron;
            //if (fWasLastTransactionAsynchron)
            //{
            //    sLastTransactionOrderReference = CurrentDialog.LastTransactionOrderReference;
            //}

            if (string.IsNullOrEmpty(aOrder.ErrorMessage))
            {
                fSuccess = _ddbac.Dialog.WasLastTransactionSuccessful;
            }

            if (fSuccess)
            {
                if (_ddbac.Dialog.HasMessageBeenSent() == false)
                {
                    fSuccess = false;
                    throw new Exception("Critical! HasMessageBeenSent is FALSE on Success! Do not release this to customers.");
                }
            }

            if (fSuccess == false)
            {
                sErrorText = aOrder.ErrorMessage;
            }
        }
        catch (Exception aException)
        {
            sErrorText = aException.Message;

            if (string.IsNullOrEmpty(aOrder.ErrorMessage))
            {
                aOrder.ErrorMessage = sErrorText;
            }

            // ErrorCustomerText may contain a better message for the customer
            var aBacException = aException as BACException;
            if (aBacException != null && !string.IsNullOrEmpty(aBacException.ErrorCustomerText))
            {
                //sErrorText += "\r\nErrorCustomerText:\r\n";
                sErrorText = aBacException.ErrorCustomerText;
            }
        }


        var sText = string.Empty;
        if (fSuccess)
        {
            sText = "Auftrag erfolgreich ausgeführt. ";
        }
        else
        {
            // Falls der Fehlertext nicht leer ist und der Fehler nicht von der Bank kommt, handelt es sich um ein Problem bei der Verarbeitung.
            if (!string.IsNullOrEmpty(sErrorText) && aOrder.ContainsBankResult() == false)
            {
                if (aOrder.LastMessage != null)
                {
                    // In LastMessage ist die letzte verarbeitete Nachricht enthalten. Sollte die Exception vor dem Senden des Auftrags geworfen werden,
                    // ist LastMessage leer.
                    // Tritt die Exception nach dem Senden auf, enthält LastMessage den Auftrag.
                    // Falls die Nachricht komplett empfangen wurde und die Rückmeldung der Bank ausgewertet werden konnte, enthält ResultSeverity
                    // den Status der Nachricht.
                    if (aOrder.LastMessage.ResultSeverity == BACMessageStatus.Success || aOrder.LastMessage.ResultSeverity == BACMessageStatus.Warning)
                    {
                        // Die Rückmeldung des Auftrags war erfolgreich.
                        aOrder.ErrorMessage +=
                            $"\r\n\r\nDer Auftrag wurde von der Bank angenommen. {aOrder.LastMessage.ResultDisplayString}. Bei der Verarbeitung der Antwort trat jedoch ein Fehler auf.";
                    }
                    else if (aOrder.LastMessage.ResultSeverity == BACMessageStatus.Error)
                    {
                        // Die Rückmeldung des Auftrags enthält einen Fehler
                        aOrder.ErrorMessage +=
                            $"\r\n\r\nDer Auftrag wurde NICHT von der Bank angenommen. {aOrder.LastMessage.ResultDisplayString}. Bei der Verarbeitung der Antwort trat ein Fehler auf.";
                    }
                    else
                    {
                        // Der Fehler trat bei der Auswertung der Antwort auf, der Status des Auftrags ist unklar.
                        aOrder.ErrorMessage += "\r\n\r\nDer Auftrag wurde an die Bank übertragen, es ist aber unklar ob der Auftrag angenommen wurde.";
                    }
                }
                else
                {
                    // Falls ein Fehler aufgetreten ist, dann kann es sein, dass der Auftrag an die Bank gesendet wurde
                    // und der Fehler erst beim Verarbeiten der Antwort aufgetreten ist. Zum Beispiel Verbindung zum
                    // Server abgebrochen, Antwort des Server konnte nicht verarbeitet werden. Die Bank hat den Auftrag aber
                    // möglicherweise angenommen. Die folgenden Abfrage liefert true, wenn der Auftrag abgesendet wurde.
                    if (aOrder.WasTransactionIncomplete())
                    {
                        aOrder.ErrorMessage += "\r\n\r\nDer Auftrag wurde an die Bank übertragen, es ist aber unklar ob der Auftrag angenommen wurde.";
                    }
                }
            }
            sText = "Fehler bei der Bearbeitung des Auftrags. ";
        }
        sText += $" Dauer: {(int)DateTime.Now.Subtract(aStart).TotalMilliseconds} ms";

        return fSuccess;
    }

    /// <summary>
    /// Get account statement and save it as MT940 file with sta extension. 
    /// Extension is added automatically with not existing
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="targetFile"></param>
    public override bool GetAccountStatementToMt940File(DateTime startDate, DateTime endDate, string targetFile)
    {
        return GetAccountStatementToMt940File(startDate, endDate, targetFile, false);
    }


    /// <summary>
    /// Get account statement and save it as MT940 file with sta extension. 
    /// Additonally create PDF file with account statement
    /// Extension is added automatically with not existing
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="targetFile"></param>
    /// <param name="pdf"></param>
    public override bool GetAccountStatementToMt940File(DateTime startDate, DateTime endDate, string targetFile, bool pdf)
    {

        if (Error != 0)
        {
            return true;
        }

        if (_ddbac.Account == null)
        {
            return true;
        }

        if (!targetFile.EndsWith(".sta"))
        {
            targetFile += ".sta";
        }

        var statement = GetAccountStatement(startDate, endDate);

        var service = new Mt940Service { TargetFileName = targetFile };

        var file = service.DataFile;
        file.AccountNumber = statement.AccountNumber;
        file.OpeningBalanceAmount = statement.OpeningBalanceAmount;
        file.OpeningBalanceDate = statement.OpeningBalanceDate;
        file.ClosingBalanceAmount = statement.ClosingBalanceAmount;
        file.ClosingBalanceDate = statement.ClosingBalanceDate;
        file.AvailableBalanceAmount = statement.AvailableBalanceAmount;
        file.AvailableBalanceDate = statement.AvailableBalanceDate;

        file.Items = statement.Items;

        service.SaveMt940File();

        if (!pdf)
        {
            return false;
        }

        var pdfFile = targetFile.Replace(".sta", ".pdf");

        var header = new AccountStatementHeader
        {
            AccountOwner = _ddbac.Account.NameEins,
            AccountNr = AccountNr,
            StartDate = startDate,
            EndDate = endDate,

        };

        CreateAccountStatementAsPdf(statement, pdfFile, header);

        return false;
    }


    /// <summary>
    /// Get all official account statement files from the bank
    /// </summary>
    /// <returns>List with account statement files</returns>
    public override IList<AccountStatementFile> GetOfficialAccountStatementFiles()
    {
        // Auftrag erstellen
        var result = new List<AccountStatementFile>();


        var aParam = new AcctStatement.Parameter(
            _ddbac.Account.FindOptimalBPDSegment(AcctStatement.Parameter.HBCICode));
        //if (aParam.AcctStatement_Param.AcctStatementNo_Allowed.HasValue &&
        //    aParam.AcctStatement_Param.AcctStatementNo_Allowed.Value)
        //{
        //    lblAcctStatementNo.Visible = true;
        //    txtAccountNumber.Visible = true;
        //}
        //else
        //{
        //    lblAcctStatementNo.Visible = false;
        //    txtAccountNumber.Visible = false;
        //}

        var receiptRequired = false;
        if (aParam.AcctStatement_Param.ReceiptRequired.HasValue)
        {
            receiptRequired = aParam.AcctStatement_Param.ReceiptRequired.Value;
        }



        var aOrder = new AcctStatementList(_ddbac.Account);

        aOrder.InsertOrderingAccount(_ddbac.Account);

        // Auftrag ausführen
        var fSuccess = _ddbac.ExecuteOrder(aOrder);
        if (fSuccess)
        {
            foreach (AcctStatementList.Response response in aOrder)
            {

                if (response.RequestPossible != true)
                {
                    continue;
                }

                var af = new AccountStatementFile
                {
                    BankCode = _ddbac.Account.BankCode,
                    AccountNr = _ddbac.Account.AccountNumber,
                    AccountStatementType = AccountStatementType.Pdf,
                    Year = Convert.ToInt32(response.Year),
                    Number = Convert.ToInt32(response.AcctStatementNo),
                    RequestPossible = response.RequestPossible,
                    CreationDateTime = response.Date,
                    CreationType = response.ResponseSegment.GetDEString("Erstellart", 1),
                    ReceiptRequired = receiptRequired
                };

                switch (response.ResponseSegment.GetDEString("Quittierung", 1))
                {
                    case "0":
                        af.ReceiptHandling = ReceiptHandling.NotNecessary;
                        break;
                    case "1":
                        af.ReceiptHandling = ReceiptHandling.ReceiptDone;
                        break;
                    case "2":
                        af.ReceiptHandling = ReceiptHandling.ReceiptOpen;
                        break;
                }


                result.Add(af);
            }

            if (aOrder.WarningMessage.Length > 0)
            {
                ShowHbciMessage(new PaymentMessage { ErrorCode = 3000, Message = aOrder.WarningMessage });
            }
            else if (aOrder.ErrorMessage.Length > 0)
            {
                ShowHbciMessage(new PaymentMessage { ErrorCode = 9000, Message = aOrder.ErrorMessage });
            }
        }

        return result;
    }


    /// <summary>
    /// Get a concrete account statement file
    /// </summary>
    /// <param name="fileData">Current metadata of an account statement file</param>
    /// <param name="sendReceipt">Send a receipt if possible and necessary</param>
    /// <param name="fileName">File name to save the file in</param>
    /// <returns>file data</returns>
    public override bool GetOfficialAccountStatementFile(AccountStatementFile fileData, bool sendReceipt, string fileName)
    {

        switch (fileData.AccountStatementType)
        {
            case AccountStatementType.Mt940:
                return GetOfficalMt940(fileData, sendReceipt, fileName);
            case AccountStatementType.Pdf:
                return GetOfficalPdf(fileData, sendReceipt, fileName);
            default:
                throw new ArgumentOutOfRangeException("accountStatementType", fileData.AccountStatementType, null);
        }
    }

    private bool GetOfficalMt940(AccountStatementFile fileData, bool sendReceipt, string fileName)
    {
        // Auftrag erstellen HKEKA
        var aOrder = new AcctStatement(_ddbac.Account) { AcctStatementNo = fileData.Number, Year = fileData.Year };

        aOrder.InsertOrderingAccount(_ddbac.Account);

        aOrder.AcctStatement_Format = 1;


        // Auftrag ausführen
        var fSuccess = _ddbac.ExecuteOrder(aOrder);
        if (fSuccess)
        {
            // Antwort der Bank auswerten
            if (aOrder.getResponse != null)
            {
                var aHIEKA = aOrder.getResponse;
                var sData = aHIEKA.BookedTrans;


                if (aHIEKA.AcctStatement_Format == 1)
                {
                    var aSwift = new BACSwiftStatement();
                    aSwift.Load(Tools.HexStringToBytes(sData));

                    // Save MT-940
                    aSwift.SaveCopyAs(fileName);
                }
                //else if (aHIEKA.AcctStatement_Format == 3) // PDF
                //{
                //    saveFileDialog.Filter = "PDF (*.pdf)|*.pdf|Alle Dateien (*.*)|*.*";
                //    if (sData != null)
                //    {
                //        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                //        {
                //            using (FileStream file = File.Create(saveFileDialog.FileName))
                //            {
                //                byte[] data = _ddbac.Net.Tools.HexStringToBytes(sData);
                //                file.Write(data, 0, data.Length);

                //                file.Close();
                //            }
                //        }
                //    }
                //}

                //else
                //{
                //    splitContainer.Visible = false;

                //    // ISO Format
                //    MessageBox.Show(this, "ISO 8583 Anzeige fehlt", frmMain.Form.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}


                if (aOrder.WarningMessage.Length > 0)
                {
                    ShowHbciMessage(new PaymentMessage { ErrorCode = 3001, Message = aOrder.WarningMessage });
                }


                //// HKEKA
                //if (m_fReceiptRequired && !String.IsNullOrEmpty(aHIEKA.ReceiptCode))
                //{
                //    if (MessageBox.Show(this, "Der Bankserver erwartet einen Quittierung und ein Quittierungscode ist vorhanden. Soll der Kontoauszug jetzt quittiert werden?", frmMain.Form.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                //    {
                //        Receipt aReceipt = new Receipt(aOrderingAccount);

                //        aReceipt.ReferencingOrder = aHIEKA.ReceiptCode;

                //        fSuccess = frmMain.Form.ExecuteOrder(this, aReceipt);
                //        if (fSuccess)
                //        {
                //            MessageBox.Show(this, aReceipt.SuccessMessage, frmMain.Form.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //        }
                //        else
                //        {
                //            MessageBox.Show(this, aReceipt.ErrorMessage, frmMain.Form.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //        }
                //    }
                //}
            }
        }
        else
        {
            if (aOrder.WarningMessage.Length > 0)
            {
                ShowHbciMessage(new PaymentMessage { ErrorCode = 3001, Message = aOrder.WarningMessage });
            }
            else if (aOrder.ErrorMessage.Length > 0)
            {
                ShowHbciMessage(new PaymentMessage { ErrorCode = 9001, Message = aOrder.ErrorMessage });
            }
            return false;
        }

        return true;
    }


    private bool GetOfficalPdf(AccountStatementFile fileData, bool sendReceipt, string fileName)
    {

        // Auftrag erstellen HKEKA
        var aOrder = new AcctStatement(_ddbac.Account) { AcctStatementNo = fileData.Number, Year = fileData.Year };

        aOrder.InsertOrderingAccount(_ddbac.Account);

        aOrder.AcctStatement_Format = 3;


        // Auftrag ausführen
        var fSuccess = _ddbac.ExecuteOrder(aOrder);
        if (fSuccess)
        {
            // Antwort der Bank auswerten
            if (aOrder.getResponse == null)
            {
                return true;
            }

            var aHIEKA = aOrder.getResponse;
            var sData = aHIEKA.BookedTrans;

            if (sData != null)
            {
                using var file = File.Create(fileName);
                var data = Tools.HexStringToBytes(sData);
                file.Write(data, 0, data.Length);
                file.Close();
            }


            //else
            //{
            //    splitContainer.Visible = false;

            //    // ISO Format
            //    MessageBox.Show(this, "ISO 8583 Anzeige fehlt", frmMain.Form.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}


            if (aOrder.WarningMessage.Length > 0)
            {
                ShowHbciMessage(new PaymentMessage { ErrorCode = 3001, Message = aOrder.WarningMessage });
            }



            if (!fileData.ReceiptRequired || !sendReceipt || string.IsNullOrEmpty(aHIEKA.ReceiptCode))
                return true;

            // HKEKA: send receipt
            var aReceipt = new Receipt(_ddbac.Account) { ReferencingOrder = aHIEKA.ReceiptCode };

            fSuccess = _ddbac.ExecuteOrder(aReceipt);
            if (fSuccess)
            {
                if (aOrder.WarningMessage.Length > 0)
                {
                    ShowHbciMessage(new PaymentMessage { ErrorCode = 3001, Message = aOrder.WarningMessage });
                }

                ShowHbciMessage(new PaymentMessage { ErrorCode = 0, Message = aOrder.SuccessMessage });
            }
            else
            {
                ShowHbciMessage(new PaymentMessage { ErrorCode = 9001, Message = aReceipt.ErrorMessage });
            }
        }
        else
        {
            if (aOrder.WarningMessage.Length > 0)
            {
                ShowHbciMessage(new PaymentMessage { ErrorCode = 3001, Message = aOrder.WarningMessage });
            }
            else if (aOrder.ErrorMessage.Length > 0)
            {
                ShowHbciMessage(new PaymentMessage { ErrorCode = 9001, Message = aOrder.ErrorMessage });
            }
            return false;
        }

        return true;


        //var aOrder = new AcctStatementPDF(_ddbac.Account);
        //aOrder.InsertOrderingAccount(_ddbac.Account);
        //aOrder.AcctStatementNo = number;
        //aOrder.Year = year;

        //var fSuccess = false;


        //var aParamPdf =
        //    new AcctStatementPDF.Parameter(
        //        _ddbac.Account.FindOptimalBPDSegment(AcctStatementPDF.Parameter.HBCICode));


        ////if (aParamPdf.AcctStatementPDF_Param.AcctStatementNo_Allowed.HasValue &&
        ////    aParamPdf.AcctStatementPDF_Param.AcctStatementNo_Allowed.Value)
        ////{
        ////    lblAcctStatementNo.Visible = true;
        ////    txtAccountNumber.Visible = true;
        ////}
        ////else
        ////{
        ////    lblAcctStatementNo.Visible = false;
        ////    txtAccountNumber.Visible = false;
        ////}

        //var receiptRequired = false;
        //if (aParamPdf.AcctStatementPDF_Param.ReceiptRequired.HasValue)
        //    receiptRequired = aParamPdf.AcctStatementPDF_Param.ReceiptRequired.Value;


        //// Auftrag ausführen
        //fSuccess = _ddbac.ExecuteOrder(aOrder);


        //if (fSuccess)
        //{
        //    // Antwort der Bank auswerten
        //    if (aOrder.getResponse != null)
        //    {
        //        var aHIEKP = aOrder.getResponse;
        //        var sDataPdf = aHIEKP.BookedTrans.ToString();


        //        if (sDataPdf != null)
        //        {
        //            using (var file = File.Create(fileName))
        //            {
        //                byte[] data;
        //                aHIEKP.BookedTrans.GetData(out data);

        //                try
        //                {
        //                    var result = Encoding.UTF8.GetString(data);
        //                    data = Convert.FromBase64String(result);
        //                }
        //                catch (FormatException ex)
        //                {
        //                    throw new BACException(ex.Message);
        //                }

        //                file.Write(data, 0, data.Length);
        //                file.Close();
        //            }
        //        }
        //    }


        //    if (aOrder.WarningMessage.Length > 0)
        //    {
        //        ShowHbciMessage(new PaymentMessage {ErrorCode = 3001, Message = aOrder.WarningMessage});
        //    }


        //    //// HKEKA
        //    //if (m_fReceiptRequired && !String.IsNullOrEmpty(aHIEKA.ReceiptCode))
        //    //{
        //    //    if (MessageBox.Show(this, "Der Bankserver erwartet einen Quittierung und ein Quittierungscode ist vorhanden. Soll der Kontoauszug jetzt quittiert werden?", frmMain.Form.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
        //    //    {
        //    //        Receipt aReceipt = new Receipt(aOrderingAccount);

        //    //        aReceipt.ReferencingOrder = aHIEKA.ReceiptCode;

        //    //        fSuccess = frmMain.Form.ExecuteOrder(this, aReceipt);
        //    //        if (fSuccess)
        //    //        {
        //    //            MessageBox.Show(this, aReceipt.SuccessMessage, frmMain.Form.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    //        }
        //    //        else
        //    //        {
        //    //            MessageBox.Show(this, aReceipt.ErrorMessage, frmMain.Form.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    //        }
        //    //    }
        //    //}
        //}

        //else
        //{
        //    if (aOrder.WarningMessage.Length > 0)
        //    {
        //        ShowHbciMessage(new PaymentMessage { ErrorCode = 3001, Message = aOrder.WarningMessage });
        //    }
        //    else if (aOrder.ErrorMessage.Length > 0)
        //    {
        //        ShowHbciMessage(new PaymentMessage { ErrorCode = 9001, Message = aOrder.ErrorMessage });
        //    }
        //    return false;
        //}

        //return true;
    }


    ///// <summary>
    ///// Einzelüberweisung senden
    ///// </summary>
    ///// <param name="transferData">Überweisungsdaten</param>
    ///// <returns></returns>
    //public PaymentMessage Transfer(HbciTransfer transferData)
    //{



    //    try
    //    {
    //        var hbciMessage = EmptyMessage();

    //        if (Error != 0) return hbciMessage;


    //        var oHkueb = _ddbac.CreateTransactionSegment("HKUEB");
    //        _ddbac.SetAuftraggeberKontoverbindung(oHkueb);

    //        // Hier wird die Kontoverbindung des Empfängers eingestellt. Für das
    //        // Empfängerkonto können wir kein Unterkontomerkmal wissen, und geben deshalb
    //        // auch keines an.

    //        oHkueb["EmpfaengerKontoverbindung1", "Kontonummer1"] = transferData.Kontonummer1;
    //        oHkueb["EmpfaengerKontoverbindung1", "Laenderkennzeichen1"] = 280;
    //        oHkueb["EmpfaengerKontoverbindung1", "Kreditinstitutcode1"] = transferData.Kreditinstitutcode1;

    //        oHkueb["EmpfaengerNameEins1", 1] = transferData.EmpfaengerNameEins1;
    //        oHkueb["Zahlungsbetrag1", "Wert1"] = String.Format("{0}",transferData.Wert1);
    //        oHkueb["Zahlungsbetrag1", "Waehrung1"] = "EUR";
    //        oHkueb["Textschluessel1", 1] = 51;
    //        oHkueb["Textschluesselergaenzung1", 1] = string.Empty;

    //        var vz = new string[2];

    //        vz[0] = transferData.Verwendungszweck.Substring(0, 27);

    //        if (transferData.Verwendungszweck.Length > 27) vz[1] = transferData.Verwendungszweck.Substring(27, 27);

    //        oHkueb["Verwendungszweck1", "Zeile1"] = vz[0];
    //        oHkueb["Verwendungszweck1", "Zeile2"] = vz[1];

    //        var oMessage = _ddbac.Dialog.ExecuteSegment(oHkueb);
    //        hbciMessage = _ddbac.ReportResult(oMessage);

    //        return hbciMessage;
    //    }
    //    catch (Exception ex)
    //    {
    //        return NoHbciError(ex);
    //    }

    //}

    ///// <summary>
    ///// Sammelüberweisung senden
    ///// </summary>
    ///// <param name="transferData">Zu übertragende Überweisungen</param>
    ///// <returns></returns>
    //public PaymentMessage TransferMultiple(HbciTransfer[] transferData)
    //{
    //    return TransferMultiple(transferData, null);
    //}




    ///// <summary>
    ///// Sammelüberweisung senden
    ///// </summary>
    ///// <param name="transferData">Zu übertragende Überweisungen</param>
    ///// <param name="fileName">Dateiname, unter dem die DTAUS-Datei gespeichert werden soll</param>
    ///// <returns></returns>
    //public PaymentMessage TransferMultiple(HbciTransfer[] transferData, string fileName)
    //{

    //    //try
    //    //{



    //    var hbciMessage = EmptyMessage();
    //    if (Error != 0) return hbciMessage;

    //    var aDtaus = new BACDTAUSFile
    //    {
    //        MyName = UserName,
    //        Type = "GK",
    //        MyBankCode = _ddbac.BankCode,
    //        MyAccountNumber = Account,
    //        EuroCurrency = true
    //    };

    //    foreach (var t in transferData)
    //    {

    //        var aDtRec = aDtaus.RecordAdd(0);
    //        aDtRec.Amount = t.Wert1;
    //        aDtRec.EuroCurrency = true;
    //        aDtRec.HisAccountNumber = t.Kontonummer1;
    //        aDtRec.HisBankCode = t.Kreditinstitutcode1;

    //        var empf = t.EmpfaengerNameEins1;

    //        if (empf.Length > 27) empf = empf.Substring(0, 27);
    //        aDtRec.HisName = empf.Replace("ß", "ss");
    //        aDtRec.Purpose = _ddbac.Get27Lines(t.Verwendungszweck.Replace("ß","ss"));
    //        aDtRec.TextKey = 51;
    //    }

    //    aDtaus.RecordSort();

    //    if (fileName != null) aDtaus.SaveAs(fileName);

    //    var oHksub = _ddbac.CreateTransactionSegment("HKSUB");
    //    _ddbac.SetAuftraggeberKontoverbindung(oHksub);

    //    oHksub["DTADatensatz1", 1] = aDtaus;

    //    var oMessage = _ddbac.Dialog.ExecuteSegment(oHksub);
    //    hbciMessage = _ddbac.ReportResult(oMessage);

    //    return hbciMessage;
    //    //}
    //    //catch (Exception Ex)
    //    //{
    //    //    return NoHBCIError(Ex);
    //    //}
    //}

    /// <summary>
    /// Sammelüberweisung senden
    /// </summary>
    /// <param name="transferData">Zu übertragende Überweisungen</param>
    /// <param name="fromBic"> </param>
    /// <returns></returns>
    public override PaymentMessage TransferMultipleTransactionsVopOptOut(IEnumerable<BankingOrder> transferData, string fromBic)
    {

        //try
        //{
        var hbciMessage = EmptyMessage();
        if (Error != 0)
        {
            hbciMessage.ErrorCode = 9000;
            hbciMessage.ErrorCode1 = Error;
            return hbciMessage;
        }

        foreach (var t in transferData)
        {

            // Auftragssegment erstellen (Einzelüberweisung)
            var oHkccs = _ddbac.CreateTransactionSegment("HKCCS"); // HKCCS = SEPA Einzelüberweisung
            _ddbac.SetSepaAuftraggeberKontoverbindung(oHkccs);

            // Sepa pain Message erstellen 
            // Auftrag und Absender einstellen

            var fromAccount = _ddbac.Account;

            if (fromAccount == null)
            {
                return new PaymentMessage { ErrorCode = 111111, Message = "No account found" };
            }


            //fromAccount.BIC = fromBic;

            var ta = t;

            var aSepaMessage = new BACSepaMessage
            {
                Segment = oHkccs,
                FromAccount = fromAccount
            };


            // Neue Order erstellen (eine für Einzelauftrag, mehrere für Sammelüberweisungen)
            var aSepaOrder = aSepaMessage.Orders.Add();

            var aToAccount = new BACAccount
            {
                CountryCode = string.Empty,
                NameEins = ClearSepaString(ta.PayerOrPayee),
                IBAN = ta.Iban.ToUpper(),
                BIC = ta.Bic
            };

            aSepaOrder.ToAccount = aToAccount;
            aSepaOrder.Amount = ta.Total;
            aSepaOrder.CurrencyCode = (string.IsNullOrEmpty(ta.Currency)) ? "EUR" : ta.Currency;
            aSepaOrder.Purpose = ClearSepaString(ta.Description);


            // Optional: Gegen das Schema verifizieren
            var sErrorReason = string.Empty;
            aSepaMessage.Verify(ref sErrorReason);
            if (!string.IsNullOrEmpty(sErrorReason))
            {
                var msg = new PaymentMessage { Message = sErrorReason, ErrorCode = 9999, ErrorCode1 = 9999 };
                ta.Result = msg;
                continue;
            }

            // ?? Wirft Fehler: warum????
            //oHkccs.Verify();

            var oMessage = _ddbac.Dialog.ExecuteSegment(oHkccs);
            hbciMessage = _ddbac.ReportResult(oMessage, oHkccs, "Sepa transfer");
            hbciMessage.ErrorCode1 = Error;
            ta.Result = hbciMessage;

        }

        /*
         * 
         * Error validating XML: Der Inhalt des Elements '{urn:sepade:xsd:pain.001.001.02}ChrgBr' ist gemäß dem Inhaltsmodell des übergeordneten Elements '{urn:sepade:xsd:pain.001.001.02}PmtInf' nicht gültig.
Erwartet: {urn:sepade:xsd:pain.001.001.02}DbtrAgt.
         */


        ////            var oMessage = _ddbac.Dialog.ExecuteSegment(oHkccs) as BACMessage;
        ////            hbciMessage = _ddbac.ReportResult(oMessage, oHkccs);

        ////            var hbciMessage = EmptyMessage();
        ////            if (Error != 0) return hbciMessage;

        ////            // Auftragssegment erstellen (Einzelüberweisung)
        ////            var oHkccs = _ddbac.CreateTransactionSegment("HKCCS"); // HKCCS = SEPA Einzelüberweisung



        ////            // Sepa pain Message erstellen 
        ////            // Auftrag und Absender einstellen

        ////            var fromAccount = _ddbac.GetAccount();
        ////            fromAccount.BIC = fromBic;


        ////            var aSepaMessage = new BACSepaMessage { Segment = oHkccs, FromAccount = fromAccount };





        ////            foreach (var t in transferData)
        ////            {

        ////                // Neue Order erstellen (eine für Einzelauftrag, mehrere für Sammelüberweisungen)
        ////                var aSepaOrder = aSepaMessage.Orders.Add();

        ////                var aToAccount = new BACAccount
        ////                {
        ////                    CountryCode = string.Empty,
        ////                    NameEins = t.EmpfaengerNameEins1,
        ////                    //AccountNumber = string.Empty,
        ////                    //BankCode = string.Empty,
        ////                    IBAN = t.Iban,
        ////                    BIC = t.Bic
        ////                };


        ////                //switch (t.Iban.Substring(0,2).ToLower())
        ////                //{
        ////                //    case "IT":
        ////                //        aToAccount.CountryCode = "380";
        ////                //        break;
        ////                //    case "at":
        ////                //        aToAccount.CountryCode = "040";
        ////                //        break;
        ////                //    default:
        ////                //        aToAccount.CountryCode = "280";
        ////                //        break;


        ////                //}


        ////                aSepaOrder.ToAccount = aToAccount;
        ////                aSepaOrder.Amount = t.Wert1;
        ////                aSepaOrder.CurrencyCode = (String.IsNullOrEmpty(t.Waehrung1)) ? "EUR" : t.Waehrung1;
        ////                aSepaOrder.Purpose = t.Verwendungszweck.Replace(".", string.Empty).Replace("-", string.Empty);

        ////            }




        ////            // Optional: Gegen das Schema verifizieren
        ////            var sErrorReason = string.Empty;
        ////            aSepaMessage.Verify(ref sErrorReason);
        ////            if (!string.IsNullOrEmpty(sErrorReason))
        ////            {
        ////                throw new Exception(sErrorReason);
        ////            }


        ////            /*
        ////             * 
        ////             * Error validating XML: Der Inhalt des Elements '{urn:sepade:xsd:pain.001.001.02}ChrgBr' ist gemäß dem Inhaltsmodell des übergeordneten Elements '{urn:sepade:xsd:pain.001.001.02}PmtInf' nicht gültig.
        ////Erwartet: {urn:sepade:xsd:pain.001.001.02}DbtrAgt.
        ////             */


        ////            var oMessage = _ddbac.Dialog.ExecuteSegment(oHkccs) as BACMessage;
        ////            hbciMessage = _ddbac.ReportResult(oMessage, oHkccs);

        return EmptyMessage();
        //}
        //catch (Exception ex)
        //{
        //    var msg = new PaymentMessage { Message = ex.Message, ErrorCode = 9999, ErrorCode1 = 9998 };
        //    return msg;
        //}
    }

    //        /// <summary>
    //        /// Sammelüberweisung senden
    //        /// </summary>
    //        /// <param name="transferData">Zu übertragende Überweisungen</param>
    //        /// <param name="fromBic"> </param>
    //        /// <returns></returns>
    //        public HbciMessage TransferMultipleSepa(HbciTransferSepa[] transferData, string fromBic)
    //        {

    //            var hbciMessage = EmptyMessage();
    //            if (Error != 0) return hbciMessage;

    //            // Auftragssegment erstellen (Einzelüberweisung)
    //            var oHkccs = _ddbac.CreateTransactionSegment("HKCCS"); // HKCCS = SEPA Einzelüberweisung



    //            // Sepa pain Message erstellen 
    //            // Auftrag und Absender einstellen

    //            var fromAccount = _ddbac.GetAccount();
    //            fromAccount.BIC = fromBic;

    //            foreach (var t in transferData)
    //            {

    //                var aSepaMessage = new BACSepaMessage { Segment = oHkccs, FromAccount = fromAccount };


    //                // Neue Order erstellen (eine für Einzelauftrag, mehrere für Sammelüberweisungen)
    //                var aSepaOrder = aSepaMessage.Orders.Add();

    //                var aToAccount = new BACAccount
    //                {
    //                    CountryCode = string.Empty,
    //                    NameEins = t.EmpfaengerNameEins1,
    //                    IBAN = t.Iban,
    //                    BIC = t.Bic
    //                };

    //                aSepaOrder.ToAccount = aToAccount;
    //                aSepaOrder.Amount = t.Wert1;
    //                aSepaOrder.CurrencyCode = (String.IsNullOrEmpty(t.Waehrung1)) ? "EUR" : t.Waehrung1;
    //                aSepaOrder.Purpose = t.Verwendungszweck.Replace(".", string.Empty).Replace("-", string.Empty);

    //                // Optional: Gegen das Schema verifizieren
    //                var sErrorReason = string.Empty;
    //                aSepaMessage.Verify(ref sErrorReason);
    //                if (!string.IsNullOrEmpty(sErrorReason))
    //                {
    //                    throw new Exception(sErrorReason);
    //                }

    //                var oMessage = _ddbac.Dialog.ExecuteSegment(oHkccs) as BACMessage;
    //                hbciMessage = _ddbac.ReportResult(oMessage, oHkccs);

    //                // Bei Fehler hier Abbruch
    //                if (hbciMessage.ErrorCode > 4000)
    //                {
    //                    return hbciMessage;
    //                }

    //            }







    //            /*
    //             * 
    //             * Error validating XML: Der Inhalt des Elements '{urn:sepade:xsd:pain.001.001.02}ChrgBr' ist gemäß dem Inhaltsmodell des übergeordneten Elements '{urn:sepade:xsd:pain.001.001.02}PmtInf' nicht gültig.
    //Erwartet: {urn:sepade:xsd:pain.001.001.02}DbtrAgt.
    //             */


    //            ////            var oMessage = _ddbac.Dialog.ExecuteSegment(oHkccs) as BACMessage;
    //            ////            hbciMessage = _ddbac.ReportResult(oMessage, oHkccs);

    //            ////            var hbciMessage = EmptyMessage();
    //            ////            if (Error != 0) return hbciMessage;

    //            ////            // Auftragssegment erstellen (Einzelüberweisung)
    //            ////            var oHkccs = _ddbac.CreateTransactionSegment("HKCCS"); // HKCCS = SEPA Einzelüberweisung



    //            ////            // Sepa pain Message erstellen 
    //            ////            // Auftrag und Absender einstellen

    //            ////            var fromAccount = _ddbac.GetAccount();
    //            ////            fromAccount.BIC = fromBic;


    //            ////            var aSepaMessage = new BACSepaMessage { Segment = oHkccs, FromAccount = fromAccount };





    //            ////            foreach (var t in transferData)
    //            ////            {

    //            ////                // Neue Order erstellen (eine für Einzelauftrag, mehrere für Sammelüberweisungen)
    //            ////                var aSepaOrder = aSepaMessage.Orders.Add();

    //            ////                var aToAccount = new BACAccount
    //            ////                {
    //            ////                    CountryCode = string.Empty,
    //            ////                    NameEins = t.EmpfaengerNameEins1,
    //            ////                    //AccountNumber = string.Empty,
    //            ////                    //BankCode = string.Empty,
    //            ////                    IBAN = t.Iban,
    //            ////                    BIC = t.Bic
    //            ////                };


    //            ////                //switch (t.Iban.Substring(0,2).ToLower())
    //            ////                //{
    //            ////                //    case "IT":
    //            ////                //        aToAccount.CountryCode = "380";
    //            ////                //        break;
    //            ////                //    case "at":
    //            ////                //        aToAccount.CountryCode = "040";
    //            ////                //        break;
    //            ////                //    default:
    //            ////                //        aToAccount.CountryCode = "280";
    //            ////                //        break;


    //            ////                //}


    //            ////                aSepaOrder.ToAccount = aToAccount;
    //            ////                aSepaOrder.Amount = t.Wert1;
    //            ////                aSepaOrder.CurrencyCode = (String.IsNullOrEmpty(t.Waehrung1)) ? "EUR" : t.Waehrung1;
    //            ////                aSepaOrder.Purpose = t.Verwendungszweck.Replace(".", string.Empty).Replace("-", string.Empty);

    //            ////            }




    //            ////            // Optional: Gegen das Schema verifizieren
    //            ////            var sErrorReason = string.Empty;
    //            ////            aSepaMessage.Verify(ref sErrorReason);
    //            ////            if (!string.IsNullOrEmpty(sErrorReason))
    //            ////            {
    //            ////                throw new Exception(sErrorReason);
    //            ////            }


    //            ////            /*
    //            ////             * 
    //            ////             * Error validating XML: Der Inhalt des Elements '{urn:sepade:xsd:pain.001.001.02}ChrgBr' ist gemäß dem Inhaltsmodell des übergeordneten Elements '{urn:sepade:xsd:pain.001.001.02}PmtInf' nicht gültig.
    //            ////Erwartet: {urn:sepade:xsd:pain.001.001.02}DbtrAgt.
    //            ////             */


    //            ////            var oMessage = _ddbac.Dialog.ExecuteSegment(oHkccs) as BACMessage;
    //            ////            hbciMessage = _ddbac.ReportResult(oMessage, oHkccs);

    //            return hbciMessage;
    //        }



    ///// <summary>
    ///// Einzellastschrift einreichen
    ///// </summary>
    ///// <returns></returns>
    //public PaymentMessage Debit(HbciDebit debitTransaction)
    //{
    //    //try
    //    //{
    //    var hbciMessage = EmptyMessage();
    //    if (Error != 0) return hbciMessage;

    //    var oHklas = _ddbac.CreateTransactionSegment("HKLAS");
    //    _ddbac.SetAuftraggeberKontoverbindung(oHklas);

    //    // Hier wird die Kontoverbindung des Zahlungspflichtigen eingestellt. Für den
    //    // Zahlungspflichtigen können wir kein Unterkontomerkmal wissen, und geben
    //    // deshalb auch keines an.

    //    oHklas["ZahlungspflichtigerKontoverbindung1", "Kontonummer1"] = debitTransaction.Kontonummer1;
    //    oHklas["ZahlungspflichtigerKontoverbindung1", "Laenderkennzeichen1"] = 280;
    //    oHklas["ZahlungspflichtigerKontoverbindung1", "Kreditinstitutcode1"] = debitTransaction.Kreditinstitutcode1;

    //    oHklas["ZahlungspflichtigerNameEins1", 1] = debitTransaction.ZahlungspflichtigerNameEins1;
    //    oHklas["Zahlungsbetrag1", "Wert1"] = debitTransaction.Wert1;
    //    oHklas["Zahlungsbetrag1", "Waehrung1"] = "EUR";
    //    oHklas["Textschluessel1", 1] = "04";

    //    var vz = new string[2];
    //    vz[0] = debitTransaction.Verwendungszweck.Substring(0, 27);
    //    if (debitTransaction.Verwendungszweck.Length > 27) vz[1] = debitTransaction.Verwendungszweck.Substring(27, 27);

    //    oHklas["Verwendungszweck1", "Zeile1"] = vz[0];
    //    oHklas["Verwendungszweck1", "Zeile2"] = vz[1];

    //    var oMessage = _ddbac.Dialog.ExecuteSegment(oHklas);
    //    hbciMessage = _ddbac.ReportResult(oMessage);

    //    return hbciMessage;
    //    //}
    //    //catch (Exception Ex)
    //    //{
    //    //    return NoHBCIError(Ex);
    //    //}
    //}

    ///// <summary>
    ///// Sammelüberweisung senden
    ///// </summary>
    ///// <param name="transferData">Zu übertragende Überweisungen</param>
    ///// <returns></returns>
    //public PaymentMessage DebitMultiple(HbciDebit[] transferData)
    //{
    //    return DebitMultiple(transferData, null);
    //}

    ///// <summary>
    ///// Sammellast senden
    ///// </summary>
    ///// <param name="transferData">Zu übertragende Lastschriften</param>
    ///// <param name="fileName">Dateiname, unter dem die DTAUS-Datei gespeichert werden soll</param>
    ///// <returns></returns>
    //public PaymentMessage DebitMultiple(HbciDebit[] transferData, string fileName)
    //{

    //    //try
    //    //{
    //    var hbciMessage = EmptyMessage();
    //    if (Error != 0) return hbciMessage;


    //    var aDtaus = new BACDTAUSFile
    //    {
    //        MyName = UserName,
    //        Type = "LK",
    //        MyBankCode = _ddbac.BankCode,
    //        MyAccountNumber = Account,
    //        EuroCurrency = true
    //    };



    //    foreach (var t in transferData)
    //    {

    //        var aDtRec = aDtaus.RecordAdd(0);
    //        aDtRec.Amount = t.Wert1;
    //        aDtRec.EuroCurrency = true;
    //        aDtRec.HisAccountNumber = t.Kontonummer1;
    //        aDtRec.HisBankCode = t.Kreditinstitutcode1;

    //        var empf = t.ZahlungspflichtigerNameEins1;

    //        if (empf.Length > 27) empf = empf.Substring(0, 27);
    //        aDtRec.HisName = empf;
    //        aDtRec.Purpose = _ddbac.Get27Lines(t.Verwendungszweck);
    //        aDtRec.TextKey = 5;
    //    }

    //    aDtaus.RecordSort();

    //    if (fileName != null) aDtaus.SaveAs(fileName);

    //    var oHksla = _ddbac.CreateTransactionSegment("HKSLA");
    //    _ddbac.SetAuftraggeberKontoverbindung(oHksla);

    //    oHksla["DTADatensatz1", 1] = aDtaus;

    //    var oMessage = _ddbac.Dialog.ExecuteSegment(oHksla);
    //    hbciMessage = _ddbac.ReportResult(oMessage);

    //    return hbciMessage;
    //    //}
    //    //catch (Exception Ex)
    //    //{
    //    //    return NoHBCIError(Ex);
    //    //}
    //}


    //        /// <summary>
    ///// Einzelüberweisung senden
    ///// </summary>
    ///// <param name="transferData">Überweisungsdaten</param>
    ///// <returns></returns>
    //public HbciMessage TransferSepa(HbciTransferSepa transferData)
    //{




    //}

    /// <summary>
    /// Quit banking module
    /// </summary>
    public override void Quit()
    {
        _ddbac.ReleaseDialog();
    }

    /// <summary>
    /// Get the list of securities items in a safekeeping account
    /// </summary>
    /// <returns>list of securities items</returns>
    public override IList<SafekeepingItem> GetDepot()
    {
        var supported = _ddbac.IsGvSupported("HKWPD");
        if (!supported)
        {
            return null;
        }

        var items = new List<SafekeepingItem>();

        var version = _ddbac.HbciVersion;

        Debug.Print(version.ToString());

        var oHkwpd = _ddbac.CreateTransactionSegment("HKWPD");

        oHkwpd["Depot1", "Depotnummer1"] = _ddbac.Account.AccountNumber;
        oHkwpd["Depot1", "Unterkontomerkmal1"] = _ddbac.Account.SubAccountNumber;
        oHkwpd["Depot1", "Laenderkennzeichen1"] = _ddbac.Account.CountryCode;
        oHkwpd["Depot1", "Kreditinstitutcode1"] = _ddbac.Account.BankCode;

        // Bei der Verarbeitung des HKWPD Segments kann der HBCI Server einen
        // Aufsetzpunkt zurückmelden. Aus diesem Grund kann es erforderlich sein,
        // dass das Auftragssegment wiederholt übermittelt werden muss.
        var sCursor = string.Empty;
        var oDataObject = new BACDataObject();
        var fHaveSomething = false;

        while (true)
        {
            var oMessage = _ddbac.Dialog.ExecuteSegment(oHkwpd);
            _ddbac.ReportResult(oMessage, "Open securities account");

            // Die Antwortdaten werden zur Ausgabe hinzugefügt.

            var oTxn = oMessage.Transactions[0];
            if (oTxn.ResponseSegments.Count == 1)
            {
                var oHiwpd = oTxn.ResponseSegments[0];

                if (oHiwpd["Depotaufstellung1", 1] is BACDataObject oBlob)
                {
                    fHaveSomething = true;

                    // Die empfangenen Daten werden jetzt an das als Sammelbecken
                    // agierende Datenobjekt angehängt.
                    oDataObject.PutString(oDataObject.GetString() + oBlob.GetString());

                }
            }
            // Falls kein Aufsetzpunkt gemeldet wurde, sind wir jetzt fertig.
            // Ansonsten wird die Auftragsnachricht mit einem Aufsetzpunkt
            // wiederholt.
            if (sCursor.Length == 0)
            {
                break;
            }

            oHkwpd["Aufsetzpunkt1", 1] = sCursor;
        }

        if (!fHaveSomething)
        {
            return items;
        }

        var oMt571 = new BACSwiftStatementOfHoldings();
        oDataObject.WriteToObject(oMt571);

        foreach (var oEntry in oMt571.StatementOfHoldingsEntries)
        {
            var item = new SafekeepingItem
            {
                Wkn = oEntry.WKN,
                Isin = oEntry.ISIN,
                Quantity = oEntry.SecuritiesQuantity,
                PricePerUnitCurrencyCode = oEntry.PricePerUnitCurrencyCode,
                PricePerUnitAmount = oEntry.PricePerUnitAmount
            };
            var iPos = oEntry.IdentificationOfSecurities.IndexOf("\r\n", StringComparison.Ordinal);
            item.Title = iPos != -1 ? oEntry.IdentificationOfSecurities.Substring(0, iPos) : oEntry.IdentificationOfSecurities;

            items.Add(item);
        }

        return items;
    }

    //private string	Pad(string s, int len)
    //{
    //    if (s.Length > len) return s.Substring(0,len);
    //    while (s.Length < len)
    //        s += " ";
    //    return s;
    //}


    /// <inheritdoc />
    /// <summary>
    /// Get the balance of an account
    /// </summary>
    /// <returns>balance amount</returns>
    public override decimal GetBalance()
    {
        if (_ddbac.Account is null)
        {
            Error = 4711;
            return 0;
        }

        try
        {
            var hksal = _ddbac.CreateTransactionSegment("HKSAL");
            _ddbac.SetAuftraggeberKontoverbindung(hksal);

            // Die Kontowährung gibt es seit HBCI 2.2 nicht mehr. Für ältere Versionen
            // ist sie dennoch erforderlich. Existiert das Feld Kontowaehrung1 im
            // Segment nicht, so wird der gesetzte Wert von den DDBAC einfach ignoriert.
            hksal["Kontowaehrung1", 1] = _ddbac.Account.Currency;
            hksal["AlleKonten1", 1] = false;

            var oMessage = _ddbac.Dialog.ExecuteSegment(hksal);
            _ddbac.ReportResult(oMessage, "Get balance");

            var oHisal = oMessage.Transactions[0].ResponseSegments[0];

            var sSollHabenKennzeichen = oHisal["SaldoGebucht1", "SollHabenKennzeichen1"] as string;

            var value = Convert.ToDecimal(Ddbac.FormatAmount(oHisal["SaldoGebucht1", "Wert1"]));
            return sSollHabenKennzeichen == "C" ? value : -value;
        }
        catch (Exception ex)
        {
            Debug.Print(ex.ToString());
            return 0;
        }
    }

    /// <summary>
    /// Get the all balances of the contact
    /// </summary>
    /// <returns>balance amount</returns>
    public override IList<AccountBalanceData> GetContactBalances()
    {
        var result = new List<AccountBalanceData>();

        try
        {
            // Kontostand abrufen
            var aOrder = new AcctBal(_ddbac.Account);

            aOrder.InsertOrderingAccount(_ddbac.Account);

            // Parameter für alle Konten oder nur für ausgewähltes
            aOrder.All_Acct = true;

            // Beispiel für den Zugriff auf die zugehörigen Bankparameter
            var aParam = new AcctBal.Parameter(_ddbac.Account.FindOptimalBPDSegment(AcctBal.Parameter.HBCICode));

            // Auftrag ausführen
            var oMessage = _ddbac.Dialog.ExecuteOrder(aOrder);

            // Für jeden zurückgemeldeten Saldo:
            foreach (AcctBal.Response aResponse in aOrder)
            {
                var data = new AccountBalanceData
                {
                    AccountName = aResponse.AcctName,
                    AccountNo = aResponse.OrderingCustAcct.AcctNo,
                    Overdraft = aResponse.Overdraft.Value ?? 0,
                    AvailableFunds = aResponse.AvailableFunds.Value ?? 0,
                    AlreadyDrawnOnAmount = aResponse.AlreadyDrawnOn_Amt.Value ?? 0,
                    OverdraftLimit = aResponse.OverdraftLim.Value ?? 0,
                    Amount = (aResponse.Current_ValutaBal.DebitCreditFlag == "D" ? -1 : 1) * (aResponse.Current_ValutaBal.AmountValue ?? 0),
                    AmountIncludingPendingTransactions = (aResponse.IncludingPendingTrans_Bal.DebitCreditFlag == "D" ? -1 : 1) * (aResponse.IncludingPendingTrans_Bal.AmountValue ?? 0)
                };
                //if (aResponse.PawnableNextMonth != null)
                //    txtResult.Text += "Pfändbar..........: " + aResponse.PawnableNextMonth.Value + " " + aResponse.PawnableNextMonth.Currency + "\r\n";
                //if (aResponse.Current_ValutaBal.Date.HasValue)
                //    txtResult.Text += "SaldoGebucht......: " +
                //        aResponse.Current_ValutaBal.Date.Value.ToShortDateString() + " " +
                //        ((aResponse.Current_ValutaBal.DebitCreditFlag == "D") ? "-" : string.Empty) + string.Format("{0:n}", aResponse.Current_ValutaBal.AmountValue) + " " +
                //        aResponse.Current_ValutaBal.AmountCurrency + "\r\n";
                //else
                //    txtResult.Text += "SaldoGebucht......: \r\n";
                //if (aResponse.IncludingPendingTrans_Bal.Date.HasValue)
                //    txtResult.Text += "SaldoNichtGebucht.: " + aResponse.IncludingPendingTrans_Bal.Date.Value.ToShortDateString() + " " + string.Format("{0:n}", aResponse.IncludingPendingTrans_Bal.AmountValue) + " " + aResponse.IncludingPendingTrans_Bal.AmountCurrency + "\r\n";

                //txtResult.Text += "\r\n";

                result.Add(data);
            }
        }
        catch (Exception ex)
        {
            Debug.Print(ex.ToString());
        }

        return result;
    }

    /// <summary>
    /// Get the owner of the account 
    /// </summary>
    /// <returns>Account owner's name</returns>
    public override string GetOwner()
    {
        return _ddbac.Account.SepaName.Trim();
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
    /// Get all banking contacts on the current system for the current user
    /// </summary>
    /// <returns></returns>
    public override IList<BankingAccount> GetBankingAccounts(string contactName, string password)
    {
        return _ddbac.GetAccounts(contactName, password);
    }

    /// <summary>
    /// Show the administration window for HBCI accounts
    /// </summary>
    public override void ShowAdministrationWindow()
    {
        _ddbac.Banking.RunContactsCPL();
    }
}