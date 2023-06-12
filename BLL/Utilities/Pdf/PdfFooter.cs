using iTextSharp.text;
using iTextSharp.text.pdf;

namespace TFI.BLL.Utilities.Pdf;

public class PdfFooter : PdfPageEventHelper
{
    private readonly Font _font = FontFactory.GetFont("Arial", 11, Font.NORMAL, new Color(0, 0, 0));
    public override void OnOpenDocument(PdfWriter writer, Document document)
    {
        base.OnOpenDocument(writer, document);
        var tabFot = new PdfPTable(new float[] { 1F })
        {
            SpacingAfter = 10F,
            TotalWidth = 300F
        };
        var cell = new PdfPCell(new Phrase(""))
        {
            Border = Rectangle.NO_BORDER
        };
        tabFot.AddCell(cell);
        tabFot.WriteSelectedRows(0, -1, 150, document.Top, writer.DirectContent);
    }
    public override void OnEndPage(PdfWriter writer, Document document)
    {
        var footer1 = new Phrase("Fondazione E.N.P.A.I.A.", PdfService.FontBold);
        var footer2 = new Phrase("Ente Nazionale di Previdenza per gli Addetti e gli Impiegati in Agricoltura", PdfService.FontGreen);
        var footer3 = new Phrase("Viale Beethoven, 48 - 00144 Roma T +39 06 54 581 F +39 06 59 26781 | info@enpaia.it | enpaia.it", _font);
        var footer4 = new Phrase("C.F. 02070800582 | P.IVA 01028511002", _font);
        
        base.OnEndPage(writer, document);
        var tabFot = new PdfPTable(new float[] { 1F })
        {
            TotalWidth = 600F
        };

        var cell1 = CreateCell(footer1);
        tabFot.AddCell(cell1);
        var cell2 = CreateCell(footer2);
        tabFot.AddCell(cell2);
        var cell3 = CreateCell(footer3);
        tabFot.AddCell(cell3);
        var cell4= CreateCell(footer4);
        tabFot.AddCell(cell4);
        tabFot.WriteSelectedRows(0, -1, 50, document.Bottom + 50, writer.DirectContent);
        
        PdfPCell CreateCell(Phrase phrase)
        {
            return new PdfPCell(phrase)
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                BorderWidth = 0.0f
            };
        }
    }
}