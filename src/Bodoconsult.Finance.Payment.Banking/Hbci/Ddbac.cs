// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

/* ====================================================================
 * THIS SOFTWARE IS PROVIDED BY DATADESIGN AG 'AS IS' AND ANY
 * EXPRESSED OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
 * PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL DATADESIGN AG OR
 * ITS CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 * ====================================================================
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Bodoconsult.Finance.Payment.Banking.Base;
using Bodoconsult.Finance.Payment.Banking.Models;
using Bodoconsult.Finance.Payment.Base.Models;
using DDBAC.Net;

namespace Bodoconsult.Finance.Payment.Banking.Hbci;

/// <summary>
/// DDBAC with HBCI base class
/// </summary>
public class Ddbac : DdbacBase
{
    private bool _online;

    /// <summary>
    /// Current HBCI version
    /// </summary>
    public int HbciVersion { get; set; }

    /// <summary>
    /// Dialog language
    /// </summary>
    public BACDialogLanguages Language { get; set; } = BACDialogLanguages.bacDialogLanguageDefault;




    /// <summary>
    /// Current dialog
    /// </summary>
    public BACDialog Dialog { get; private set; }

    /// <summary>
    /// Init the dialog
    /// </summary>
    public void Init()
    {
        //#if !DDBACNET
        //            // Hier wird die Syntax für alle HIKAZ Segmente so geändert, dass die
        //            // empfangenen Daten nicht durch das BACSwiftStatement Objekt geparst
        //            // werden, sondern erst einmal einfach in einem BACDataObject Objekt
        //            // gesammelt werden.
        //            // Dies ist notwenig, weil einige Kreditinstitute entgegen der Spezifikation
        //            // bei Kontoumsätzen mit Aufsetzpunkten in einer Nachricht teilweise
        //            // unvollständiges MT-??? zurückliefern und der interne Parser einen
        //            // Fehler meldet.
        //            BACSyntaxSegment aSyntaxSegment;
        //            BACSyntaxElement aSyntaxElement;

        //            for (int i = 2; i < 99; i++)
        //            {
        //                aSyntaxSegment = m_Syntax.FindSegment("HIKAZ", i);
        //                if (aSyntaxSegment == null) break;
        //                aSyntaxElement = aSyntaxSegment.SyntaxElements.FindElementByName("UmsaetzeGebucht");
        //                aSyntaxElement.ObjectClass = "DataDesign.BACDataObject";
        //                m_Syntax.Add(aSyntaxSegment);

        //                aSyntaxSegment = m_Syntax.FindSegment("HIWPD", i);
        //                if (aSyntaxSegment == null) break;
        //                aSyntaxElement = aSyntaxSegment.SyntaxElements.FindElementByName("Depotaufstellung");
        //                aSyntaxElement.ObjectClass = "DataDesign.BACDataObject";
        //                m_Syntax.Add(aSyntaxSegment);
        //            }
        //#endif
    }

    /// <summary>
    /// Abort dialog
    /// </summary>
    public void AbortDialog()
    {
        Dialog = null;
    }

    /// <summary>
    /// Release dialog
    /// </summary>
    public void ReleaseDialog()
    {
        Contact = null;
        Dialog = null;
        _online = false;
        _mFDialogHasBeenAborted = false;
        Banking = new BACBanking();
    }

    /// <summary>
    /// Get all banking contacts on the current computer
    /// </summary>
    /// <returns></returns>
    public static IList<string> GetBankingContacts()
    {
        var aContacts = new BACContacts();
        aContacts.Populate(string.Empty);
        return aContacts.Select(oContact => oContact.Name).ToList();
    }

    /// <summary>
    /// Create a new dialog
    /// </summary>
    /// <param name="contact">Contact name</param>
    /// <param name="passphrase">Passphrase</param>
    public void NewDialog(string contact, string passphrase)
    {
        var aContacts = new BACContacts();
        aContacts.Populate();

        var checkValue = contact.ToLowerInvariant();

        foreach (var oContact in aContacts)
        {
            if (!oContact.Name.Equals(checkValue, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            Contact = oContact;

            break;
        }

        Dialog = Contact.NewDialog(new BACSecureString(passphrase));
        Dialog.ProductName = ProductName;
        Dialog.ProductVersion = ProductVersion;

        if (Dialog != null)
        {
            Dialog.OnStateChange += Dialog_OnStateChange;
        }
    }

    /// <summary>
    /// Format an date object as date string
    /// </summary>
    /// <param name="date">Object containing a date</param>
    /// <returns>Date as string</returns>
    public static string FormatDate(object date)
    {
        return date is not DateTime dt ? date.ToString() : dt.ToShortDateString();
    }
    /// <summary>
    /// Format an amount object as amount string
    /// </summary>
    /// <param name="amount">Amount object</param>
    /// <returns>Amount string</returns>
    public static string FormatAmount(object amount)
    {
        var val = (decimal)amount;
        return val.ToString("n", NumberFormatInfo.CurrentInfo);
    }

    /// <summary>
    /// Checks if the nation Kontonummer is allowed
    /// </summary>
    /// <returns>True if the nation Kontonummer is allowed else false</returns>
    public bool IsNationaleKontoverbindungErlaubt()
    {
        foreach (var aBpdSegment in Contact.BankData.Segments)
        {
            if (aBpdSegment.Type == "HISPAS")
            {
                return (bool)aBpdSegment["Parameter1", "NationaleKontoverbindungErlaubt1"];
            }
        }
        return true;
    }

    /// <summary>
    /// Setzt die Kontoverbindungsdaten des Auftraggebers in ein
    /// Auftragssegment. Dabei wird der HISPAS berücksichtigt und die
    /// nationale Kontoverbindung wird nur eingetragen, wenn es erlaubt ist.
    /// </summary>
    /// <param name="oSegment">Given segment</param>
    public void SetSepaAuftraggeberKontoverbindung(BACSegment oSegment)
    {
        if (IsNationaleKontoverbindungErlaubt())
        {
            oSegment["AuftraggeberKontoverbindung1", "Kontonummer1"] = Account.AccountNumber;
            oSegment["AuftraggeberKontoverbindung1", "Unterkontomerkmal1"] = Account.SubAccountNumber;
            oSegment["AuftraggeberKontoverbindung1", "Laenderkennzeichen1"] = Account.CountryCode;
            oSegment["AuftraggeberKontoverbindung1", "Kreditinstitutcode1"] = Account.BankCode;
        }
        oSegment["AuftraggeberKontoverbindung1", "IBAN1"] = Account.IBAN;
        oSegment["AuftraggeberKontoverbindung1", "BIC1"] = Account.BIC;
    }

    /// <summary>
    /// Setzt die Kontoverbindungsdaten des Auftraggebers in ein
    /// Auftragssegment. Die korrekten Kontodaten werden dabei aus dem
    /// HIUPD Segment des Kontos gelesen.
    /// </summary>
    /// <param name="oSegment">Given segment</param>
    public void SetAuftraggeberKontoverbindung(BACSegment oSegment)
    {
        /*
        Alle Kontodetails aus den UPD werden kopiert.Obwohl das GD 
        "Unterkontomerkmal" erst mit HBCI 2.2 eingeführt wurde, wird es hier
         dennoch immer kopiert.Liegt dieses GD nicht vor, so wird einfach ein
         leerer Wert kopiert, der später beim Segmentaufbau in der DDBAC
         ignoriert wird.
        */
        oSegment["AuftraggeberKontoverbindung1", "Kontonummer1"] = Account.AccountNumber;
        oSegment["AuftraggeberKontoverbindung1", "Unterkontomerkmal1"] = Account.SubAccountNumber;
        oSegment["AuftraggeberKontoverbindung1", "Laenderkennzeichen1"] = Account.CountryCode;
        oSegment["AuftraggeberKontoverbindung1", "Kreditinstitutcode1"] = Account.BankCode;

        if (!Account.IsSepa())
        {
            return;
        }
        oSegment["AuftraggeberKontoverbindung1", "IBAN1"] = Account.IBAN;
        oSegment["AuftraggeberKontoverbindung1", "BIC1"] = Account.BIC;
    }

    /// <summary>
    /// Diese Funktion erzeugt ein Segment Objekt für einen gegebenen 
    /// Geschäftsvorfall. Dabei wird die neueste GV Version die sowohl 
    /// vom HBCI Zielsystem als auch von der aufrufenden Anwendung 
    /// unterstützt werden ermittelt. Wird keine Schnittmenge gefunden, 
    /// (meist dann der Fall, wenn der gewünschte Geschäftsvorfall 
    /// nicht vom HBCI Zielsystem angeboten wird) so wird eine 
    /// entsprechende Fehlermeldung ausgelöst.
    /// </summary>
    /// <param name="sSegmentType">sSegmentType gibt den gewünschten Geschäftsvorfall, z.B. "HKUEB" oder "HKSAL" an.</param>
    /// <returns>Segment to send to bank</returns>
    public BACSegment CreateTransactionSegment(string sSegmentType)
    {
        if (Account == null)
        {
            throw new Exception("Der Account ist null.");
        }

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

    /*
    ' *****************************************************************************
    '   Function:   ReportResult()
    '
    '   Comments:   Diese Funktion zeigt dem Kunden die Rückmeldungen aus der
    '               abgewickelten HBCI-Nachricht an. Für Auftragsnachrichten muss
    '               dieser Funktion eine Referenz auf das Auftragssegment
    '               übergeben werden. Bei administrativen Nachrichten ist dies
    '               nicht erforderlich.
    '
    '               Im einfachsten Fall wird lediglich die bereits von der DDBAC
    '               im BACDialog Objekt bereitgestellte Rückmeldung in der
    '               Statuszeile angezeigt.
    '
    '               Trat bei der Verarbeitung ein Fehler auf, oder handelte es sich
    '               um eine explizit bestätigte Transaktion (kein Abholauftrag),
    '               dann werden die relevanten Rückmeldungen in einer MsgBox zur
    '               Bestätigung angezeigt.
    ' *****************************************************************************
    */
    //public string ReportResult(BACMessage oMsg, BACSegment oSegment)
    //{
    //    // Die DDBAC stellt die Rückmeldung mit dem höchsten Rückmeldecode aus
    //    // dem HIRMG in den Properties HBCIResultCode und HBCIResult des BACDialog
    //    // Objekts direkt zur Verfügung. Interessant ist hier vor allem der
    //    // Rückmeldecode, der als Anhaltspunkt für den Gesamterfolg der Nachricht
    //    // genommen werden kann.
    //    string sCursor = string.Empty;
    //    int iResultCode = Dialog.HBCIResultCode;
    //    string sResultText = Dialog.HBCIResult;

    //    // Anzuzeigender Rückmeldetext und ein möglichst sinnvoller Rückmeldecode
    //    // werden ermittelt.

    //    sResultText = FormatResultDisplayString(oMsg, ref sCursor, ref iResultCode, oSegment);

    //    // Hier wird der maximale Rückmeldecode dazu genutzt, ein entsprechendes
    //    // Icon für die später generierte MsgBox auszuwählen.
    //    //MessageBox.Show (
    //    MessageBoxIcon nIcon;

    //    if (iResultCode < 3000)
    //        nIcon = MessageBoxIcon.Information;
    //    else if (iResultCode < 9000)
    //        nIcon = MessageBoxIcon.Exclamation;
    //    else
    //        nIcon = MessageBoxIcon.Error;

    //    // Eine Meldung zur Kundenanzeige wird generiert. Die aufgerufene
    //    // Funktion liefert einen leeren String, wenn es nichts wichtiges zu
    //    // berichten gibt. Dies ist z.B. bei einem erfolgreichen Abruf eines
    //    // Kontostandes der Fall. Falls eine Kundenanzeige generiert wurde, wird
    //    // diese in einer MsgBox dargestellt.

    //    if (sResultText.Length > 0)
    //    {
    //        String sRemainingSignatures = m_oContact["RemainingSignatures"];
    //        if (sRemainingSignatures != null && sRemainingSignatures.Length > 0)
    //        {
    //            sResultText += "\r\n\r\nNoch verfügbare Signaturen mit diesem Medium: " + sRemainingSignatures;
    //        }
    //        MessageBox.Show(sResultText, "HBCI.NET", MessageBoxButtons.OK, nIcon);
    //    }
    //    return sCursor;
    //}


    /// <summary>
    /// Report a result
    /// </summary>
    /// <param name="oMsg">BACMessage</param>
    /// <param name="oSegment">Current segment</param>
    /// <param name="message">Message</param>
    /// <returns>Payment message</returns>
    public PaymentMessage ReportResult(BACMessage oMsg, BACSegment oSegment, string message)
    {
        // Die DDBAC stellt die Rückmeldung mit dem höchsten Rückmeldecode aus
        // dem HIRMG in den Properties HBCIResultCode und HBCIResult des BACDialog
        // Objekts direkt zur Verfügung. Interessant ist hier vor allem der
        // Rückmeldecode, der als Anhaltspunkt für den Gesamterfolg der Nachricht
        // genommen werden kann.

        var e = new PaymentMessage
        {
            ErrorCode = Dialog.HBCIResultCode,
            Message = $"{message}: {Dialog.HBCIResult}"
        };


        // Anzuzeigender Rückmeldetext und ein möglichst sinnvoller Rückmeldecode
        // werden ermittelt.
        var s = FormatResultDisplayString(oMsg, ref e.Cursor, ref e.ErrorCode, oSegment).Trim();

        if (!string.IsNullOrEmpty(s))
        {
            e.Message += $" {s}";
        }

        //// Hier wird der maximale Rückmeldecode dazu genutzt, ein entsprechendes
        //// Icon für die später generierte MsgBox auszuwählen.
        ////MessageBox.Show (
        //MessageBoxIcon	nIcon;

        //if (iResultCode < 3000)
        //    nIcon = MessageBoxIcon.Information;
        //else if (iResultCode < 9000)
        //    nIcon = MessageBoxIcon.Exclamation;
        //else
        //    nIcon = MessageBoxIcon.Error;


        ShowHbciMessage(e);


        return e;
    }

    /// <summary>
    /// Report a result
    /// </summary>
    /// <param name="oMsg">BACMessage</param>
    /// <param name="message">Message</param>
    /// <returns>Payment message</returns>
    public PaymentMessage ReportResult(BACMessage oMsg, string message)
    {
        return ReportResult(oMsg, null, message);
    }

    /*
    ' *****************************************************************************
    '   Function:   FormatResultDisplayString()
    '
    '   Comments:   Diese Funktion generiert eine textuelle Gesamtmeldung für eine
    '               abgewickelte HBCI-Nachricht, die für eine direkte
    '               Kundenanzeige geeignet ist. Der zurückgelieferte String ist
    '               eine einzeilige Verkettung aller relevanten Rückmeldetexte.
    '               Gibt es in der abgewickelten Nachricht keine zur Anzeige
    '               relevanten Rückmeldetexte, so wird einer leerer Srting
    '               zurückgegeben.
    '
    '               Für Auftragsnachrichten muss dieser Funktion eine Referenz auf
    '               das Auftragssegment übergeben werden. Die Rückmeldung zum
    '               Auftragssegment wird dann gesondert behandelt.
    ' *****************************************************************************
    */
    private string FormatResultDisplayString(BACMessage oMsg, ref string sCursor, ref int iStatusCode, BACSegment oSegment)
    {
        string sDisplayString;

        sCursor = string.Empty;
        iStatusCode = 0;

        // Falls keine Referenz zu einem Auftragssegment übergeben wurde, wird
        // angenommen, dass es sich um eine administrative Nachricht handelt. In
        // diesem Fall wird die gesamte Nachricht betrachtet ohne besondere
        // Berücksichtigung einer einzelnen Transaktion.
        if (oSegment == null)
        {
            return ResultTextsFromMessage(oMsg, ref iStatusCode);
        }

        // Es handelt sich um eine Auftragsnachricht. Der für den Kunden wichtige
        // Auftrag wird durch das übergebene Auftragssegment referenziert. Das
        // heißt, wir müssen zuerst die Transaktion in der das übergebene
        // Auftragssegment enthalten ist suchen. Dies sollte normalerweise auch
        // das einzige BACTransaction Objekt in der Transactions collection sein.

        BACTransaction oTxn = null;

        foreach (var oTxnItem in oMsg.Transactions)
        {
            if (oTxnItem.OrderSegment != null)
            {
                oTxn = oTxnItem;
                break;
            }
        }
        if (oTxn == null)
        {
            iStatusCode = 9000;
            return "Transaktionssegment nicht vorhanden";
        }

        // Bei der Auswertung der Rückmeldung wird zwischen Abholaufträgen und
        // Transaktionsaufträgen unterschieden. Bei einem vollständig erfolgreichen
        // Abholauftrag ist die genaue Erfolgsmeldung der Rückmeldung irrelevant,
        // dem Benutzer interessieren nur die abgeholten Daten selbst. Bei einem
        // Transaktionsauftrag ist die genaue Formulierung der Erfolgsmeldung
        // wichtig, da hier evtl. wichtige Informationen für den Kunden enthalten
        // sind. Ein Abholauftrag wird hier ganz einfach durch das Vorhandensein
        // von Antwortsegmenten erkannt.

        var fAbholauftrag = oTxn.ResponseSegments.Count > 0;

        // Ist ein Rückmeldesegment vorhanden, dann reichen die in diesem Segment
        // vorhandenen Informationen um eine Sinnvolle Klartextmeldung für den
        // Kunden zu generieren. Bei Abholaufträgen werden Erfolgsmeldungen durch
        // setzen eines entsprechenden unteren Grenzwertes unterdrückt.

        if (oTxn.Acknowledgement != null)
        {
            if (fAbholauftrag)
            {
                sDisplayString = ResultTextsFromHirmx(oTxn.Acknowledgement, 3000, ref sCursor, ref iStatusCode);
            }
            else
            {
                var sDummyCursor = string.Empty;
                sDisplayString = ResultTextsFromHirmx(oTxn.Acknowledgement, 0, ref sDummyCursor, ref iStatusCode);
            }

            if (iStatusCode >= 9000)
            {
                iStatusCode = 9050;
            }
            else if (iStatusCode >= 3000)
            {
                iStatusCode = 3060;
            }
            else
            {
                iStatusCode = 10;
            }
        }
        else
        {
            // Ist kein Rückmeldesegment zu diesem Auftrag vorhanden, so ist ein
            // schwerwiegenderes Problem aufgetreten. In diesem Fall finden sich die
            // relevanten informationen in den administrativen Rückmeldesegmenten.
            // Das heißt, die Nachricht kann wie eine administrative Nachricht
            // ausgewertet werden.

            sDisplayString = ResultTextsFromMessage(oMsg, ref iStatusCode);
        }
        return sDisplayString;
    }

    /*
    ' *****************************************************************************
    '   Function:   ResultTextsFromMessage()
    '
    '   Comments:   Generiert eine formatierte textuelle Gesamtmeldung aus einer
    '               abgewickelten HBCI Nachricht. Die generierte Gesamtmeldung ist
    '               für die direkte Kundenanzeige gedacht. In dieser Funktion
    '               werden alle Rückmeldesegmente gleichwertig behandelt, eine
    '               Fokussierung auf eine bestimmte Transaktion findet nicht
    '               statt. Aus diesem Grund, ist diese Funktion vor allem für
    '               administrative HBCI Nachrichten, wie z.B. Dialoginitialisierung
    '               oder Dialogbeendigung, geeignet.
    '
    '               Der nThreshold Parameter ermöglicht die Angabe eines
    '               Mindestwertes für den Rückmeldecode. Ist der Wert eines
    '               Rückmeldecodes kleiner als der hier angegebene Wert, dann wird
    '               diese Rückmeldung nicht berücksichtigt. Für administrative
    '               HBCI Nachrichten sind detaillierte Erfolgsmeldungen und
    '               Warnungen nicht von Kundeninteresse. Deshalb wird dieser
    '               Parameter mit der Fehlerkategorie 9000 vorbelegt. Dadurch
    '               werden nur echte Fehlermeldungen generiert.
    '
    '               Es ist möglich, dass diese Funktion einen leeren String als
    '               Ergebnis liefert. Dies kann als erfolgreiche Abwicklung der
    '               Nachricht interpretiert werden.
    ' *****************************************************************************
    */
    private string ResultTextsFromMessage(BACMessage oMsg, ref int iStatusCode)
    {
        return ResultTextsFromMessage(oMsg, ref iStatusCode, 9000);
    }
    private string ResultTextsFromMessage(BACMessage oMsg, ref int iStatusCode, int iThreshold)
    {
        string sText;
        var sDummyCursor = string.Empty;
        var iDummyStatusCode = 0;

        if (oMsg == null)
        {
            return string.Empty;
        }

        // Zuerst wird der Rückmeldung zur Gesamtnachricht unsere Aufmerksamkeit
        // gewidmet. Laut HBCI muss diese Rückmeldung immer vorhanden sein. Wir
        // sind hier dennoch auf den Fall vorbereitet, dass kein HIRMG vorhanden
        // ist, was in jedem Fall auch als Fehler gewertet wird.

        if (oMsg.Acknowledgement != null)
        {
            sText = ResultTextsFromHirmx(oMsg.Acknowledgement, iThreshold, ref sDummyCursor, ref iStatusCode);
        }
        else
        {
            sText = "Keine Rückmeldung vom Kreditinstitut!";
            if (iStatusCode < 9000)
            {
                iStatusCode = 9000;
            }
        }

        if (iStatusCode == 0)
        {
            iStatusCode = 10;
        }

        // Als nächstes werden die möglichen Rückmeldungen zu den übertragenen
        // Signaturen betrachtet.
        string sMsg;
        foreach (var oSig in oMsg.Signatures)
        {
            if (oSig.SignatureHeaderAcknowledgement != null)
            {
                sMsg = ResultTextsFromHirmx(oSig.SignatureHeaderAcknowledgement, iThreshold, ref sDummyCursor, ref iDummyStatusCode);
                sText = AppendResultText(sText, sMsg);
            }
            if (oSig.SignatureValueAcknowledgement != null)
            {
                sMsg = ResultTextsFromHirmx(oSig.SignatureValueAcknowledgement, iThreshold, ref sDummyCursor, ref iDummyStatusCode);
                sText = AppendResultText(sText, sMsg);
            }
        }

        // Auch die Transaktionen können Rückmeldungen enthalten.
        foreach (var oTxn in oMsg.Transactions)
        {
            if (oTxn.Acknowledgement != null)
            {
                sMsg = ResultTextsFromHirmx(oTxn.Acknowledgement, iThreshold, ref sDummyCursor, ref iDummyStatusCode);
                sText = AppendResultText(sText, sMsg);
            }
        }


        // Zuletzt kümmern wir uns um die Rückmeldungen, die aus irgendeinem Grund
        // in der OrphanSegments collection gelandet sind.
        foreach (var oSeg in oMsg.OrphanResponseSegments)
        {
            if (oSeg.Type == "HIRMS")
            {
                sMsg = ResultTextsFromHirmx(oSeg, iThreshold, ref sDummyCursor, ref iDummyStatusCode);
                sText = AppendResultText(sText, sMsg);
            }
        }
        return sText;
    }

    /*
    ' *****************************************************************************
    '   Function:   ResultTextsFromHIRMX()
    '
    '   Comments:   Extrahiert und formatiert die Rückmeldetexte des gegebenen
    '               Rückmeldesegments. Die formatierte Gesamtmeldung wird als
    '               String zurückgeliefert. Diese Funktion kann sowohl für HIRMG
    '               als auch für HIRMS Segmente genutzt werden.
    '
    '               Der nThreshold Parameter ermöglicht die Angabe eines
    '               Mindestwertes für den Rückmeldecode. Ist der Wert eines
    '               Rückmeldecodes kleiner als der hier angegebene Wert, dann wird
    '               diese Rückmeldung nicht berücksichtigt.
    '
    '               Ist in der Rückmeldung ein Aufsetzpunkt enthalten, so wird
    '               dieser in den Parameter sCursor eingestellt. Wird kein
    '               Aufsetzpunkt gefunden, so wird in sCursor ein Nullstring
    '               eingestellt
    '
    '               Es ist möglich, dass diese Funktion einen leeren String als
    '               Ergebnis liefert.
    ' *****************************************************************************
    */
    private string ResultTextsFromHirmx(BACSegment oHirmx, int iThreshold, ref string cursor, ref int iStatusCode)
    {
        int i;
        var sResultText = string.Empty;

        cursor = string.Empty;

        // Ein einzelnes Rückmeldesegment kann bis zu 99 Rückmeldungen enthalten.
        // Hier wird über alle möglichen Rückmeldungen iteriert.

        for (i = 2; i < 100; i++)
        {

            // Der Rückmeldecode wird extrahiert. Dieser wird als Variant
            // ausgelesen, damit erkannt werden kann ob tatsächlich ein Wert
            // vorliegt. Liegt kein Wert vor, dann wurde die letzte Rückmeldung
            // bereits verarbeitet und die Schleife kann vorzeitig beendet
            // werden.

            var vtRueckmeldecode = oHirmx[i, 1];
            if (vtRueckmeldecode == null)
            {
                break;
            }

            var iRueckmeldecode = Convert.ToInt32(vtRueckmeldecode);

            // Entspricht die Rückmeldung einem Aufsetzpunkt, so wird dieser
            // extrahiert und die Rückmeldung ansonsten ignoriert.

            if (oHirmx.Type == "HIRMS" && oHirmx.RefSeqNo != 0 &&
                iRueckmeldecode == 3040 && oHirmx[i, 4] != null)
            {
                cursor = oHirmx[i, 4] as string;
            }

            // Ist der Wert des Rückmeldecodes kleiner als der gewünschte
            // Grenzwert, so wird diese Rückmeldung ignoriert. Ausserdem werden
            // die Rückmeldungen 3060 (Teilweise liegen Warnungen vor) und 9050
            // (Teilweise liegen Fehler vor) gefiltert. Diese beiden Codes treten
            // in einer Nachricht nur im HIRMG zusammen mit einem genaueren HIRMS
            // auf. Wir überlassen die Fehlerbeschreibung daher dem HIRMS.

            else if (iRueckmeldecode >= iThreshold &&
                     iRueckmeldecode != 3060 &&
                     iRueckmeldecode != 9050)
            {
                // Aus dem Rückmeldetext und dem Rückmeldecode wird eine
                // Klartextmeldung aufgebaut und an den gesammelte Meldetext
                // angehängt. Der Rückmeldecode wird immer vierstellig
                // angegeben.
                var sText = oHirmx[i, 3] as string;

                for (var j = 0; j < 10; j++)
                {
                    if (oHirmx[i, 4 + j] is string { Length: > 0 } sParameter)
                    {
                        sText += ",";
                        sText += sParameter;
                    }
                    else
                    {
                        break;
                    }
                }

                sResultText = AppendResultText(sResultText, FormatResultText(sText, iRueckmeldecode));

                // In nStatusCode wird der höchste berücksichtigte Rückmeldecode
                // ermittelt.

                if (iRueckmeldecode > iStatusCode)
                {
                    iStatusCode = iRueckmeldecode;
                }
            }
        }
        return sResultText;
    }

    /*
    ' *****************************************************************************
    '   Function:   AppendResultText()
    '
    '   Comments:   Hängt den gegebenen formatierten Rückmeldetext an die
    '               Gesamtmeldung an. Wird als Rückmeldetext ein leerer String
    '               übergeben, so macht diese Funktion gar nichts. Die
    '               Rückmeldetexte werden in der Gesamtmeldung durch ";"
    '               voneinander getrennt.
    ' *****************************************************************************
    */
    private string AppendResultText(string sText, string sResultText)
    {
        if (sResultText.Length <= 0)
        {
            return sText;
        }
        sText = sText.Length > 0 ? $"{sText}; {sResultText}" : sResultText;
        return sText;
    }

    /*
    ' *****************************************************************************
    '   Function:   FormatResultText()
    '
    '   Comments:   Erzeugt einen formatierten Rückmeldetext aus dem rohen
    '               Rückmeldetext und dem Rückmeldecode. Der formatierte
    '               Rückmeldetext ist für die direkte Kundenanzeige gedacht.
    ' *****************************************************************************
    */
    private string FormatResultText(string sResultText, int iResultCode)
    {
        // Aus dem Rückmeldetext und dem Rückmeldecode wird eine formatierte
        // Klartextmeldung aufgebaut. Der Rückmeldecode wird immer vierstellig
        // angegeben.
        var sText = $"{sResultText} ({iResultCode:0000})";

        return sText;
    }

    private bool _mFDialogHasBeenAborted;

    /// <summary>
    /// Execute f a new state is set for the dialog
    /// </summary>
    /// <param name="nNewState">ID of the new state</param>
    public void Dialog_OnStateChange(int nNewState)
    {
        if (nNewState == (int)BACDialogStates.bacDialogAborted)
        {
            // Cannot delete DDBAC Dialog directly, while in DDBAC Dialog Callback
            _mFDialogHasBeenAborted = true;
        }
        if (nNewState == (int)BACDialogStates.bacDialogReady)
        {
            _online = true;
        }
        if (nNewState == (int)BACDialogStates.bacDialogIdle)
        {
            _online = false;
        }
    }

    /// <summary>
    /// Dialog has been aborted
    /// </summary>
    public bool DialogHasBeenAborted => _mFDialogHasBeenAborted;

    /// <summary>
    /// Is the dialog online?
    /// </summary>
    public bool Online => _online;

    /// <summary>
    /// Get the BPD segemnt
    /// </summary>
    /// <param name="sType">Requested type of segment</param>
    /// <returns>Banking segment to sent to bank</returns>
    public BACSegment GetBpdSegment(string sType)
    {
        foreach (var oSegment in Contact.BankData.Segments)
        {
            if (oSegment.Type == sType)
                return oSegment;
        }
        return null;
    }

    ///// <summary>
    ///// Is the GV supported
    ///// </summary>
    ///// <param name="sSegments">Segments as string</param>
    ///// <returns>True if the GV is supported else false</returns>
    //public bool IsGvSupported(string sSegments)
    //{
    //    if (_online == false)
    //    {
    //        return false;
    //    }

    //    if (Account == null)
    //    {
    //        return false;
    //    }

    //    while (sSegments.Length > 0)
    //    {
    //        var iPos = sSegments.IndexOf(",", StringComparison.Ordinal);
    //        string sType;
    //        if (iPos == -1)
    //        {
    //            sType = sSegments;
    //            sSegments = string.Empty;
    //        }
    //        else
    //        {
    //            sType = sSegments[..iPos];
    //            sSegments = sSegments[(iPos + 1)..];
    //        }

    //        if (Account.FindOptimalBPDSegment(sType, 0) != null)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    /// <summary>
    /// Show a received HBCI message
    /// </summary>
    /// <param name="message">Current message to show</param>
    public void ShowHbciMessage(PaymentMessage message)
    {
        OnHbciMessage?.Invoke(message);
    }

    /// <summary>
    /// Get all accounts
    /// </summary>
    /// <param name="contact">Account name</param>
    /// <param name="passphrase">Passphrase</param>
    /// <returns>List with bankin accounts</returns>
    public IList<BankingAccount> GetAccounts(string contact, string passphrase)
    {
        NewDialog(contact, passphrase);

        var erg = new List<BankingAccount>();

        if (Contact == null)
        {
            return erg;
        }

        try
        {
            erg.AddRange(Contact.Accounts.Select(account => new BankingAccount
            {
                Contact = Contact.Name,
                AccountNumber = account.AccountNumber,
                Iban = account.IBAN
            }));
        }
        catch
        {
            // ignored
        }
        ReleaseDialog();
        return erg;

    }

    /// <summary>
    /// Checks if a Geschäftsvorfall is supported
    /// </summary>
    /// <param name="sSegments">Segments as string</param>
    /// <returns>True if the GV is supported</returns>
    public bool IsGvSupported(string sSegments)
    {

        if (Account == null)
        {
            return false;
        }

        while (sSegments.Length > 0)
        {
            var iPos = sSegments.IndexOf(',');
            string sType;
            if (iPos == -1)
            {
                sType = sSegments;
                sSegments = string.Empty;
            }
            else
            {
                sType = sSegments[..iPos];
                sSegments = sSegments[(iPos + 1)..];
            }

            if (Account.FindOptimalBPDSegment(sType, 0) != null)
            {
                return true;
            }
        }
        return false;
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

            var aDialog = Dialog;
            aDialog.ExecuteOrder(aOrder);

            //if (aDialog.BeginDialogResult != null)
            //{
            //    // True if return code 3072 was found
            //    if (aDialog.BeginDialogResult.MustChangeContactData())
            //    {
            //        String s1 = aDialog.BeginDialogResult.NewUserID();
            //        String s2 = aDialog.BeginDialogResult.NewCustomerID();
            //        String s3 = aDialog.BeginDialogResult.NewLoginName();

            //        // Application must inform the user an change the contact data accordingly
            //    }
            //}

            //fWasLastTransactionAsynchron = aDialog.WasLastTransactionAsynchron;
            //if (fWasLastTransactionAsynchron)
            //{
            //    sLastTransactionOrderReference = aDialog.LastTransactionOrderReference;
            //}

            if (string.IsNullOrEmpty(aOrder.ErrorMessage))
            {
                fSuccess = aDialog.WasLastTransactionSuccessful;
            }

            if (fSuccess)
            {
                if (aDialog.HasMessageBeenSent() == false)
                {
                    fSuccess = false;
                    //throw new Exception("Critical! HasMessageBeenSent is FALSE on Success! Do not release this to customers.");
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
            if (aException is BACException aBacException && !string.IsNullOrEmpty(aBacException.ErrorCustomerText))
            {
                //sErrorText += "\r\nErrorCustomerText:\r\n";
                sErrorText = aBacException.ErrorCustomerText;
            }
        }


        if (fSuccess)
        {
            return true;
        }

        // Falls der Fehlertext nicht leer ist und der Fehler nicht von der Bank kommt, handelt es sich um ein Problem bei der Verarbeitung.
        if (string.IsNullOrEmpty(sErrorText) || aOrder.ContainsBankResult())
        {
            return false;
        }

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

        return false;
    }
}