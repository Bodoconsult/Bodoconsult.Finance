// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using Bodoconsult.Pdf.PdfSharp;
using Bodoconsult.Pdf.Stylesets;
using MigraDoc.DocumentObjectModel;

namespace Bodoconsult.Finance.Payment.Base.Models;

/// <summary>
/// Create an account statement as PDF from MT940 data
/// </summary>
[SupportedOSPlatform("windows")]
public class PdfAccountStatement
{
    private readonly AccountStatementHeader _headerData;
    private readonly IList<AccountStatementLine> _data;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="data">Account statement data to show in the account statement</param>
    /// <param name="headerData">Header data for the account statement</param>
    /// <param name="fileName">Target file name</param>
    public PdfAccountStatement(IList<AccountStatementLine> data, AccountStatementHeader headerData, string fileName)
    {
        FileName = fileName;
        _data = data;
        StatementRowsPerPage = 11;
        StatementRowsPerPageFirstPage = 10;
        _headerData = headerData;
    }

    /// <summary>
    /// Filename for the account statement file
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Number of statement rows per page
    /// </summary>
    public int StatementRowsPerPage { get; set; }

    /// <summary>
    /// Number of statement rows on the first page
    /// </summary>
    public int StatementRowsPerPageFirstPage { get; set; }

    /// <summary>
    /// Create the account statement file
    /// </summary>
    public void CreateFile()
    {
        var styleset = new DefaultStyleSet
        {
            DocumentMetaData =
            {
                HeaderText = $"Kontoauszug {_headerData.AccountNr}",
                FooterText = "Seite <<page>> von <<pages>>"
            }
        };

        styleset.CreatePageSetup();

        styleset.PageSetup.LeftMargin = 2.0;
        styleset.PageSetup.TopMargin = 1.5;
        styleset.PageSetup.RightMargin = 1.3;
        styleset.PageSetup.BottomMargin = 1.2;


        var pdf = new PdfCreator(styleset);

        pdf.SetDocInfo($"Kontoauszug {_headerData.AccountNr}",
            $"Kontoauszug {_headerData.AccountNr}",
            _headerData.AccountOwner);

            AddStyles(pdf);

        pdf.SetHeader();
        pdf.SetFooter();

        pdf.CreateContentSection();
        
        var title = $"{_headerData.AccountOwner}: Kontoauszug für Konto {_headerData.AccountNr}";
        pdf.AddParagraph(title, "heading1");

        // Anzahl Seiten anzeigen
        var p = new Paragraph();
        p.AddText("Anzahl Seiten: ");
        p.AddNumPagesField();
        p.Style = "Reference";
        pdf.AddParagraph(p);

        // Neuer Kontostand
        pdf.AddParagraph(
            $"Auswertungszeitraum: {_headerData.StartDate:dd.MM.yyyy} - {_headerData.EndDate:dd.MM.yyyy}", "Reference");

        // Alter Kontostand
        pdf.AddParagraph(
            $"alter Kontostand am {_headerData.StartBalanceDate:dd.MM.yyyy}:       {_headerData.StartBalanceAmount} {_headerData.StartBalanceCurrency} {(_headerData.StartBalanceAmount >= 0 ? "H" : "S")}", "Reference");


        // Neuer Kontostand
        pdf.AddParagraph(
            $"neuer Kontostand am {_headerData.EndBalanceDate:dd.MM.yyyy}:       {_headerData.EndBalanceAmount} {_headerData.EndBalanceCurrency} {(_headerData.EndBalanceAmount >= 0 ? "H" : "S")}", "Reference");


        if (_data is { Count: > 0 })
        {
            var page = 1;
            var rowCount = 1;

            // Add data to PDF
            foreach (var accountStatement in _data.OrderBy(x => x.EntryDate))
            {
                if (rowCount == 1)
                {
                    // Table head
                    pdf.TableStart("NormalTable");
                    pdf.TableAddColumn(ParagraphAlignment.Left, 0.085 * pdf.Width);
                    pdf.TableAddColumn(ParagraphAlignment.Right, 0.085 * pdf.Width);
                    pdf.TableAddColumn(ParagraphAlignment.Left, 0.59 * pdf.Width);
                    pdf.TableAddColumn(ParagraphAlignment.Right, 0.12 * pdf.Width);
                    pdf.TableAddColumn(ParagraphAlignment.Right, 0.12 * pdf.Width);

                    pdf.TableAddRow();

                    pdf.TableSetContent(0, 0, "Bu-Tag");
                    pdf.TableSetContent(1, 0, "Wert");
                    pdf.TableSetContent(2, 0, "Vorgang");
                    pdf.TableSetContent(3, 0, "Soll");
                    pdf.TableSetContent(4, 0, "Haben");
                }

                pdf.TableAddRow();

                var cell = pdf.Table.Rows[rowCount].Cells[0];
                cell.AddParagraph(accountStatement.EntryDate.ToString("dd.MM.yy"));

                cell = pdf.Table.Rows[rowCount].Cells[1];
                cell.AddParagraph(accountStatement.ValidityDate.ToString("dd.MM.yy"));

                cell = pdf.Table.Rows[rowCount].Cells[2];

                var purpose = accountStatement.Purpose;

                purpose = purpose?.Replace("\r\n", " ");

                var vorgang =
                    $"{accountStatement.BusinessTransactionText} PN: {accountStatement.BusinessTransactionCode}\r\n{accountStatement.Name}\r\n{purpose}";

                cell.AddParagraph(vorgang);


                cell = pdf.Table.Rows[rowCount].Cells[3];
                cell.AddParagraph(accountStatement.Amount < 0 ? accountStatement.Amount.ToString("#,##0.00 S") : string.Empty);

                cell = pdf.Table.Rows[rowCount].Cells[4];
                cell.AddParagraph(accountStatement.Amount >= 0 ? accountStatement.Amount.ToString("#,##0.00 H") : string.Empty);

                rowCount++;
                if (rowCount <= (page == 1 ? StatementRowsPerPageFirstPage : StatementRowsPerPage))
                {
                    continue;
                }

                rowCount = 1;
                page++;
                pdf.AddPageBreak();
            }
        }

        pdf.AddParagraph("-- Ende Kontoauszug --");
        pdf.RenderToPdf(FileName, false);
        pdf.Dispose();
    }

    private static void AddStyles(PdfCreator pdf)
    {
        if (pdf == null)
        {
            return;
        }

        var style = pdf.AddStyle("Normal");
        style.Font.Name = "Arial";
        style.Font.Size = 9;


        style = pdf.AddStyle("Heading1");
        style.Font.Name = "Arial Black";
        style.Font.Size = 14;
        style.Font.Color = Colors.Black;
        style.ParagraphFormat.PageBreakBefore = true;
        style.ParagraphFormat.KeepTogether = true;
        style.ParagraphFormat.KeepWithNext = true;
        style.ParagraphFormat.SpaceAfter = 2;
        style.ParagraphFormat.SpaceBefore = 4;
        style.ParagraphFormat.Alignment = ParagraphAlignment.Left;

        style = pdf.AddStyle("Heading2");
        style.Font.Name = "Arial Black";
        style.Font.Size = 12;
        style.Font.Color = Colors.Black;
        style.ParagraphFormat.KeepTogether = true;
        style.ParagraphFormat.KeepWithNext = true;
        style.ParagraphFormat.SpaceAfter = 1;
        style.ParagraphFormat.SpaceBefore = 2;
        style.ParagraphFormat.Alignment = ParagraphAlignment.Left;

        style = pdf.AddStyle("Heading3");
        style.Font.Name = "Arial";
        style.Font.Size = 12;
        style.Font.Color = Colors.Black;
        style.ParagraphFormat.PageBreakBefore = true;
        style.ParagraphFormat.KeepTogether = true;
        style.ParagraphFormat.KeepWithNext = true;
        style.ParagraphFormat.SpaceAfter = 2;
        style.ParagraphFormat.SpaceBefore = 4;
        style.ParagraphFormat.Alignment = ParagraphAlignment.Left;

        style = pdf.AddStyle("Heading4");
        style.Font.Name = "Arial Black";
        style.Font.Size = 11;
        style.Font.Color = Colors.Black;
        style.ParagraphFormat.PageBreakBefore = true;
        style.ParagraphFormat.KeepTogether = true;
        style.ParagraphFormat.KeepWithNext = true;
        style.ParagraphFormat.SpaceAfter = 2;
        style.ParagraphFormat.SpaceBefore = 4;
        style.ParagraphFormat.Alignment = ParagraphAlignment.Left;


        // Spezielles Format für Tabellenbasis (nicht ändern!)
        style = pdf.AddStyle("NormalTable");
        style.Font.Name = "Arial Narrow";
        style.Font.Size = 3;
        style.ParagraphFormat.SpaceBefore = 0;
        style.ParagraphFormat.SpaceAfter = 0;
        style.ParagraphFormat.PageBreakBefore = false;
        style.ParagraphFormat.Alignment = ParagraphAlignment.Center;
        style.ParagraphFormat.LeftIndent = 0;
        style.ParagraphFormat.RightIndent = 0;

        // Spezielles Format für Tabellenbasis (nicht ändern!)
        style = pdf.AddStyle("NormalTable1");
        style.Font.Name = "Arial Narrow";
        style.Font.Size = 3;
        style.ParagraphFormat.SpaceBefore = Unit.FromCentimeter(1.5);
        style.ParagraphFormat.SpaceAfter = Unit.FromCentimeter(0.5);
        style.ParagraphFormat.PageBreakBefore = false;
        style.ParagraphFormat.Alignment = ParagraphAlignment.Center;

        // Footer
        style = pdf.AddStyle("Footer");
        style.Font.Name = "Arial Narrow";
        style.Font.Size = 8;
        style.Font.Color = Colors.Black;

        // Reference
        style = pdf.AddStyle("Reference");
        style.Font.Name = "Arial";
        style.Font.Size = 12;
        style.Font.Color = Colors.Black;
        style.ParagraphFormat.Alignment = ParagraphAlignment.Left;
        style.ParagraphFormat.SpaceAfter = 2;
        style.ParagraphFormat.SpaceBefore = 4;
    }
}