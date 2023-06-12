using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using TFI.DAL.ConnectorDB;
using TFI.OCM.AziendaConsulente;
using Utilities;

namespace TFI.BLL.Utilities.Pdf;

public class PdfArretratoService : PdfService
{
    public byte[] CreaStampaArretrati(List<DenunciaArretrati> arretrati, Azienda.PosAss posizioneAssicurativaAzienda, int anno, int mese, int proDen)
    {
        var numberPage = Convert.ToInt32(Math.Ceiling(arretrati.Count / Convert.ToDecimal(PdfPageSize)) + 1M);


        using var memoryStream = new MemoryStream();
        var document = new Document(PageSize.A4.Rotate(), 15, 15, 15, 15);
        var pdfWriter = PdfWriter.GetInstance(document, memoryStream);

        var movAnn = GetAnnullamento(anno, mese, proDen, posizioneAssicurativaAzienda.CODPOSPA);
        var numMov = GetNumMov(anno, mese, proDen, posizioneAssicurativaAzienda.CODPOSPA);
        var abb = GetAbb(anno, mese, proDen, posizioneAssicurativaAzienda.CODPOSPA);

        document.Open();
        for (var page = 1; page < numberPage; page++)
        {
            if (!string.IsNullOrWhiteSpace(movAnn.DatMovAnn))
                SetIntestazioneAnnullamento(pdfWriter);

            ScriviIntestazione();
            var actualRecords = arretrati.Skip(PdfPageSize * (page - 1)).Take(PdfPageSize).ToList();
            WriteTablePage();
            WritePieDiPagina(pdfWriter, document, page, numberPage);

            document.NewPage();

            void WriteTablePage()
            {
                document.Add(CreaIntestazioneTabella());

                foreach (var record in actualRecords)
                {
                    var recordTabella = new Table(9, 1);
                    recordTabella.SetWidths(new[] { 6, 36, 12, 4, 10, 4, 10, 10, 8 });
                    recordTabella.AutoFillEmptyCells = true;
                    recordTabella.WidthPercentage = 100;
                    recordTabella.BorderWidth = 0;
                    recordTabella.Cellspacing = 0;
                    recordTabella.Cellpadding = 2;
                    WriteRecordArretrato(recordTabella, record);
                    document.Add(recordTabella);
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(movAnn.DatMovAnn))
            SetIntestazioneAnnullamento(pdfWriter);

        ScriviIntestazione();
        ScriviTotali(document, anno, mese, proDen, posizioneAssicurativaAzienda.CODPOSPA, movAnn.NumMovAnn, movAnn.DatSanAnn);
        pdfWriter.PageEvent = new PdfFooter();
        WritePieDiPagina(pdfWriter, document, numberPage, numberPage);

        document.Close();
        pdfWriter.Close();

        return memoryStream.GetBuffer();

        void ScriviIntestazione()
        {
            var pdfContentByte = pdfWriter.DirectContent;

            Image img = Image.GetInstance(PathFileHelper.PathLogoEnpaia);
            img.ScaleAbsolute(845, 75);
            img.SetAbsolutePosition(0, 510);
            document.Add(img);

            pdfContentByte.BeginText();
            pdfContentByte.SetFontAndSize(bfVerdana, 9);
            pdfContentByte.ShowTextAligned(0, $"Denuncia arretrati n. {numMov}", 17, 495, 0);
            pdfContentByte.EndText();

            pdfContentByte.BeginText();
            pdfContentByte.SetFontAndSize(bfVerdana, 9);
            pdfContentByte.ShowTextAligned(0,
                "Denominazione _____________________________________________________________________", 52, 460, 0);
            pdfContentByte.ShowTextAligned(0, posizioneAssicurativaAzienda.RagioneSocialePA, 135, 462, 0);
            pdfContentByte.EndText();

            pdfContentByte.BeginText();
            pdfContentByte.SetFontAndSize(bfVerdana, 9);
            pdfContentByte.ShowTextAligned(0, "Posizione assicurativa ENPAIA ______________", 590, 460, 0);
            pdfContentByte.ShowTextAligned(0, posizioneAssicurativaAzienda.CODPOSPA, 720, 462, 0);
            pdfContentByte.EndText();

            pdfContentByte.BeginText();
            pdfContentByte.SetFontAndSize(bfVerdana, 9);
            pdfContentByte.ShowTextAligned(0,
                "Telefono __________________________________ E-mail __________________________________", 52, 445, 0);
            pdfContentByte.ShowTextAligned(0, posizioneAssicurativaAzienda.Telefono1PA, 105, 447, 0);
            pdfContentByte.ShowTextAligned(0, posizioneAssicurativaAzienda.EmailPA, 250, 447, 0);
            pdfContentByte.EndText();

            pdfContentByte.BeginText();
            pdfContentByte.SetFontAndSize(bfVerdana, 9);
            pdfContentByte.ShowTextAligned(0, "Abbonamento P.A. Azienda", 590, 445, 0);
            if (abb == "S")
                pdfContentByte.ShowTextAligned(0, "X", 733, 445, 0);
            pdfContentByte.EndText();

            var grx = new Graphic();
            grx.Rectangle(730, 442, 13, 13);
            grx.Stroke();
            document.Add(grx);

            grx.Rectangle(15, 438, 810, 40);
            grx.Stroke();
            document.Add(grx);
        }
    }

    private Table CreaIntestazioneTabella()
    {
        var testataTabella = new Table(9, 1);
        testataTabella.SetWidths(new[] { 6, 36, 12, 4, 10, 4, 10, 10, 8 });
        testataTabella.Offset = 140;
        testataTabella.AutoFillEmptyCells = true;
        testataTabella.WidthPercentage = 100;
        testataTabella.BorderWidth = 0;
        testataTabella.Cellspacing = 0;
        testataTabella.Cellpadding = 2;
        var fontRigaTest = new Font(bfVerdanaBlack, 6.75F, Font.NORMAL);

        var cell = new Cell(new Phrase("Matricola", fontRigaTest));
        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.HorizontalAlignment = Element.ALIGN_LEFT;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        testataTabella.AddCell(cell, 0, 0);

        cell = new Cell(new Phrase("Cognome e Nome", fontRigaTest));
        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.HorizontalAlignment = Element.ALIGN_LEFT;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        testataTabella.AddCell(cell, 0, 1);


        cell = new Cell(new Phrase("Anno Competenza", fontRigaTest));
        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.HorizontalAlignment = Element.ALIGN_LEFT;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        testataTabella.AddCell(cell, 0, 2);


        cell = new Cell(new Phrase("Retribuzione Imponibile", fontRigaTest));
        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        cell.Colspan = 2;
        testataTabella.AddCell(cell, 0, 3);


        cell = new Cell(new Phrase("di cui Occasionali", fontRigaTest));
        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        cell.Colspan = 2;
        testataTabella.AddCell(cell, 0, 5);


        cell = new Cell(new Phrase("Aliquota %", fontRigaTest));
        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        testataTabella.AddCell(cell, 0, 7);


        cell = new Cell(new Phrase("Qual.", fontRigaTest));
        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        testataTabella.AddCell(cell, 0, 8);

        return testataTabella;
    }

    private void WriteRecordArretrato(Table recordTable, DenunciaArretrati record)
    {
        var fontRigaDettaglio = new Font(bfVerdana, 7.75F, Font.NORMAL);

        var cell = new Cell(new Phrase(record.mat.ToString().Trim(), fontRigaDettaglio))
        {
            VerticalAlignment = Element.ALIGN_MIDDLE,
            HorizontalAlignment = Element.ALIGN_LEFT,
            BorderWidth = 0,
            BorderWidthBottom = 0.5F
        };
        recordTable.AddCell(cell, 0, 0);

        cell = new Cell(new Phrase(record.nome.Trim().Substring(0, Math.Min(record.nome.Trim().Length, 22)),
            fontRigaDettaglio))
        {
            HorizontalAlignment = Element.ALIGN_LEFT,
            BorderWidth = 0,
            BorderWidthBottom = 0.5F
        };
        recordTable.AddCell(cell, 0, 1);

        cell = new Cell(new Phrase(record.anncom.ToString(), fontRigaDettaglio))
        {
            HorizontalAlignment = Element.ALIGN_CENTER,
            BorderWidth = 0,
            BorderWidthBottom = 0.5F
        };
        recordTable.AddCell(cell, 0, 2);

        cell = new Cell(new Phrase("€", fontRigaDettaglio))
        {
            HorizontalAlignment = Element.ALIGN_RIGHT,
            BorderWidth = 0,
            BorderWidthBottom = 0.5F
        };
        recordTable.AddCell(cell, 0, 3);

        cell = new Cell(new Phrase(record.impret, fontRigaDettaglio))
        {
            HorizontalAlignment = Element.ALIGN_RIGHT,
            BorderWidth = 0,
            BorderWidthBottom = 0.5F
        };
        recordTable.AddCell(cell, 0, 4);

        cell = new Cell(new Phrase("€", fontRigaDettaglio))
        {
            HorizontalAlignment = Element.ALIGN_RIGHT,
            BorderWidth = 0,
            BorderWidthBottom = 0.5F
        };
        recordTable.AddCell(cell, 0, 5);

        cell = new Cell(new Phrase(record.impocc, fontRigaDettaglio))
        {
            HorizontalAlignment = Element.ALIGN_RIGHT,
            BorderWidth = 0,
            BorderWidthBottom = 0.5F
        };
        recordTable.AddCell(cell, 0, 6);

        cell = new Cell(new Phrase(record.aliquota, fontRigaDettaglio))
        {
            HorizontalAlignment = Element.ALIGN_RIGHT,
            BorderWidth = 0,
            BorderWidthBottom = 0.5F
        };
        recordTable.AddCell(cell, 0, 7);

        cell = record.codquacon == 1
            ? new Cell(new Phrase("D", fontRigaDettaglio))
            : new Cell(new Phrase("I", fontRigaDettaglio));
        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        recordTable.AddCell(cell, 0, 8);
    }

    private void ScriviTotali(Document document, int annDen, int mesDen, int proDen, string codPos, string numMovAnn, string datSanAnn)
    {
        var objDataAccess = new DataLayer();

        var tabTotali = new Table(3, 1);
        var tabFooter = new Table(7, 4);
        var tabFooter2 = new Table(2, 6);

        decimal impTot = 0;
        string impSanTot;
        var addizionale = numMovAnn == "" ? "Addizionale" : "Addizionale annullato";

        var dtSanzioni = new DataTable();

        var totDescSan = "";
        var totDescSan2 = "";
        // decimal impDec = 0;
        // decimal impTotDec = 0;


        var strSql =
            "SELECT DENTES.SANSOTSOG, DENTES.IMPCON, DENTES.DATVER, DENTES.IMPVER, DENTES.UFFPOS, DENTES.CITDIC, DENTES.PRODIC, DENTES.IBAN, DENTES.ABIDIC, DENTES.CABDIC, VALUE(DENTES.IMPADDREC, 0.0) AS IMPADDREC, VALUE(DENTES.IMPASSCON, 0.0) AS IMPASSCONAZI, DENTES.IMPABB AS IMPABBAZI, DENTES.IMPDEC, DENTES.ANNDEN, VALUE(DENTES.IMPSANDET, 0.0) AS IMPSANDET, DENTES.CODMODPAG, DENTES.CODCAUSAN" +
            $" FROM  DENTES  WHERE ANNDEN = {annDen} AND MESDEN = {mesDen} AND PRODEN = {proDen} AND CODPOS = {codPos}";
        var dtTotali = objDataAccess.GetDataTable(strSql);

        var codCau = dtTotali.Rows[0]["CODCAUSAN"].ToString();

        // IMPDEC = dtTotali.Rows[0].DecimalElementAt("IMPDEC").Value;
        Cell cell;
        iTextSharp.text.Font fontFooterBox = new iTextSharp.text.Font(bfVerdana, 7.75F, Font.BOLD);
        iTextSharp.text.Font fontFooterBoxU = new iTextSharp.text.Font(bfVerdana, 7.75F, Font.UNDERLINE);
        iTextSharp.text.Font fontFooterBoxSmall = new iTextSharp.text.Font(bfVerdana, 6.75F, Font.NORMAL);

        if (numMovAnn == "")
        {
            Table tabDettFooter = new Table(1, 3)
            {
                WidthPercentage = 100,
                BorderWidth = 0,
                Cellspacing = 0,
                Cellpadding = 1,
                AutoFillEmptyCells = true
            };

            var fontFooter = new Font(bfVerdana, 7.75F, Font.BOLD, Color.WHITE);
            // Font fontFooterB = new Font(bfVerdana, 7.75F, Font.BOLD);

            cell = new Cell(new Phrase("Il/La sottoscritto/a ________________________________________________________",
                fontFooterBox));
            cell.BorderWidth = 0;
            tabDettFooter.AddCell(cell, 0, 0);

            fontFooterBox = new Font(bfVerdana, 7.75F, Font.NORMAL);

            var strAppo =
                @"dichiara ai sensi della legge n.15 del 31 gennaio 1968 e successive modifiche ed integrazioni, 
                    sotto la propria responsabilità civile e penale, che le informazioni e i dati contenuti nel presente modulo 
                    sono rispondenti al vero e si impegna a comunicare entro 30 giorni qualsiasi variazione riguardante le situazioni dichiarate.";

            cell = new Cell(new Phrase(strAppo, fontFooterBox));
            cell.BorderWidth = 0;
            tabDettFooter.AddCell(cell, 1, 0);

            strAppo = "      Data ____/____/______     Timbro della Ditta e Firma __________________________";
            cell = new Cell(new Phrase(strAppo, fontFooterBox));
            cell.BorderWidth = 0;
            tabDettFooter.AddCell(cell, 2, 0);

            var colonne2 = new int[7];
            colonne2[0] = 5;
            colonne2[1] = 2;
            colonne2[2] = 53;
            colonne2[3] = 2;
            colonne2[4] = 25;
            colonne2[5] = 6;
            colonne2[6] = 7;

            tabFooter.AutoFillEmptyCells = true;
            tabFooter.WidthPercentage = 100;
            tabFooter.Cellpadding = 2;
            tabFooter.Cellspacing = 0;
            tabFooter.SetWidths(colonne2);
            tabFooter.Offset = 20;
            tabFooter.BorderWidth = 0;

            cell = new Cell("");
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 0, 0);

            cell = new Cell("");
            cell.GrayFill = 0.3F;
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 0, 1);

            cell = new Cell(new Phrase("Dichiarazione di responsabilità", fontFooter));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.GrayFill = 0.3F;
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 0, 2);

            cell = new Cell("");
            cell.GrayFill = 0.3F;
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 0, 3);

            cell = new Cell("");
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 0, 4);

            cell = new Cell("");
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 0, 5);

            cell = new Cell("");
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 0, 6);

            cell = new Cell("");
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 1, 0);

            cell = new Cell("");
            cell.GrayFill = 0.3F;
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 1, 1);

            cell = new Cell(tabDettFooter);
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 1, 2);

            cell = new Cell("");
            cell.GrayFill = 0.3F;
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 1, 3);

            if (codCau == "")
                totDescSan = "Sanzioni";
            else if (datSanAnn != "")
                totDescSan = "Sanzioni";
            else
            {
                strSql = "SELECT VALUE(TASSO, 0.00) AS TASSO, TIPMOV, DESCAUREP AS DESC FROM TIPMOVCAU ";
                strSql += " WHERE CODCAU ='" + codCau + "' AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
                dtSanzioni.Clear();
                dtSanzioni = objDataAccess.GetDataTable(strSql);
                if (dtSanzioni.Rows.Count > 0)
                {
                    for (var kk = 0; kk <= dtSanzioni.Rows.Count - 1; kk++)
                    {
                        switch (dtSanzioni.Rows[kk]["TIPMOV"].ToString().Trim())
                        {
                            case "SAN_MD":
                            {
                                if (dtSanzioni.Rows[kk]["DESC"].ToString() != "")
                                {
                                    totDescSan = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dtSanzioni.Rows[kk]["DESC"].ToString());
                                    // TotDescSan = dtSanzioni.Rows[kk]["DESC"].ToString().Substring(0, 1) +
                                    //              dtSanzioni.Rows[kk]["DESC"].ToString().ToLower().Substring(1,
                                    //                  dtSanzioni.Rows[kk]["DESC"].ToString().Length - 1);
                                    totDescSan2 = $"al tasso del {dtSanzioni.Rows[kk]["TASSO"]} % annuo";
                                }
                                else
                                    totDescSan = "";

                                break;
                            }

                            case "SAN_RD":
                            {
                                if (dtSanzioni.Rows[kk]["DESC"].ToString() != "")
                                {
                                    totDescSan =
                                        CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dtSanzioni.Rows[kk]["DESC"].ToString());
                                    // TotDescSan = dtSanzioni.Rows[kk]["DESC"].ToString().Substring(0, 1) +
                                    //              dtSanzioni.Rows[kk]["DESC"].ToString().ToLower().Substring(1,
                                    //                  dtSanzioni.Rows[kk]["DESC"].ToString().Length - 1);
                                    totDescSan2 = $"al tasso del {dtSanzioni.Rows[kk]["TASSO"]} % annuo";
                                }
                                else
                                    totDescSan = "";

                                break;
                            }
                        }
                    }
                }
                else
                    totDescSan = "";
            }

            cell = new Cell(new Paragraph(new Phrase("Totale generale CTR", fontFooterBox)));
            cell.Add(new Paragraph(new Phrase(addizionale, fontFooterBox)));
            cell.Add(new Paragraph(new Phrase("Assistenza Contrattuale", fontFooterBox)));
            cell.Add(new Paragraph(new Phrase("Abbonamenti P.A.", fontFooterBox)));
            cell.Add(new Paragraph(new Phrase(totDescSan, fontFooterBox)));
            if (!string.IsNullOrWhiteSpace(totDescSan2))
                cell.Add(new Paragraph(new Phrase(totDescSan2, fontFooterBox)));


            //Cell.Add(new Paragraph(new Phrase("Importo decurtato", fontFooterBox)));
            cell.Add(new Paragraph(new Phrase("TOTALE dovuto", fontFooterBox)));
            //Cell.Add(new Paragraph(new Phrase("TOTALE da pagare", fontFooterB)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 1, 4);

            cell = new Cell(new Paragraph(new Phrase("€", fontFooterBox)));
            cell.Add(new Paragraph(new Phrase("€", fontFooterBox)));
            cell.Add(new Paragraph(new Phrase("€", fontFooterBox)));
            cell.Add(new Paragraph(new Phrase("€", fontFooterBox)));
            cell.Add(new Paragraph(new Phrase("€", fontFooterBox)));
            if (!string.IsNullOrWhiteSpace(totDescSan2))
                cell.Add(new Paragraph(new Phrase("€", fontFooterBox)));

            cell.Add(new Paragraph(new Phrase("€", fontFooterBox)));

            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 1, 5);

            if (dtTotali.Rows.Count > 0)
            {
                impTot += Convert.ToDecimal(dtTotali.Rows[0]["IMPCON"]);
                impTot += Convert.ToDecimal(dtTotali.Rows[0]["IMPADDREC"]);
                impTot += Convert.ToDecimal(dtTotali.Rows[0]["IMPASSCONAZI"]);
                impTot += Convert.ToDecimal(dtTotali.Rows[0]["IMPABBAZI"]);

                // if (IMPDEC > 0)
                // IMPTOT = IMPTOT - IMPDEC;

                if (dtTotali.Rows[0]["SANSOTSOG"].ToString().Trim() == "S")
                    impTot += 0;
                else if (datSanAnn.ToString() == "")
                    impTot += Convert.ToDecimal(dtTotali.Rows[0]["IMPSANDET"]);
                else
                    impTot += 0;

                if (datSanAnn.ToString() == "")
                    impSanTot = Convert.ToDecimal(dtTotali.Rows[0]["IMPSANDET"]).ToString("#,##0.#0");
                else
                    impSanTot = "0,00";

                cell = new Cell(new Paragraph(new Phrase(Convert.ToDecimal(dtTotali.Rows[0]["IMPCON"]).ToString("#,##0.#0"), fontFooterBox)));
                cell.Add(new Paragraph(new Phrase(Convert.ToDecimal(dtTotali.Rows[0]["IMPADDREC"]).ToString("#,##0.#0"), fontFooterBox)));
                cell.Add(new Paragraph(new Phrase(Convert.ToDecimal(dtTotali.Rows[0]["IMPASSCONAZI"]).ToString("#,##0.#0"), fontFooterBox)));
                cell.Add(new Paragraph(new Phrase(Convert.ToDecimal(dtTotali.Rows[0]["IMPABBAZI"]).ToString("#,##0.#0"), fontFooterBox)));
                if (!string.IsNullOrWhiteSpace(totDescSan2))
                    cell.Add(new Paragraph(new Phrase("0,00", fontFooterBoxSmall)));
                if (dtTotali.Rows[0]["SANSOTSOG"].ToString().Trim() == "S")
                    cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
                else
                    cell.Add(new Paragraph(new Phrase(impSanTot, fontFooterBox)));

                // Cell.Add(new Paragraph(new Phrase(IMPDEC.ToString("#,##0.#0"), fontFooterBox)));
                // IMPTOT_DEC = (IMPTOT + IMPDEC);
                // Cell.Add(new Paragraph(new Phrase(IMPTOT_DEC.ToString("#,##0.#0"), fontFooterB)));
                cell.Add(new Paragraph(new Phrase(impTot.ToString("#,##0.#0"), fontFooterBox)));
            }
            else
            {
                cell = new Cell(new Paragraph(new Phrase("0,00", fontFooterBox)));
                cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
                cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
                cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
                if (!string.IsNullOrWhiteSpace(totDescSan2))
                    cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
                cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
                cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
            }

            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 1, 6);

            cell = new Cell("");
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 2, 0);

            cell = new Cell("");
            cell.GrayFill = 0.3F;
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 2, 1);

            cell = new Cell("");
            cell.GrayFill = 0.3F;
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 2, 2);

            cell = new Cell("");
            cell.GrayFill = 0.3F;
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 2, 3);

            cell = new Cell("");
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 2, 4);

            cell = new Cell("");
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 2, 5);

            cell = new Cell("");
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 2, 6);

            cell = new Cell("");
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 3, 0);

            cell = new Cell("");
            cell.GrayFill = 0.3F;
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 3, 1);

            cell = new Cell("");
            cell.GrayFill = 0.3F;
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 3, 2);

            cell = new Cell("");
            cell.GrayFill = 0.3F;
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 3, 3);

            cell = new Cell("");
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 3, 4);

            cell = new Cell("");
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 3, 5);

            cell = new Cell("");
            cell.BorderWidth = 0;
            tabFooter.AddCell(cell, 3, 6);
            
            // Secondo Footer ----------------------------------------------------------------------
            var colonne3 = new int[2];
            colonne3[0] = 20;
            colonne3[1] = 80;
            tabFooter2.AutoFillEmptyCells = true;
            tabFooter2.WidthPercentage = 100;
            tabFooter2.Cellpadding = 2;
            tabFooter2.Cellspacing = 0;
            tabFooter2.BorderWidth = 0;
            tabFooter2.Offset = 20;
            tabFooter2.SetWidths(colonne3);

            cell = new Cell(new Phrase("Riferimenti del versamento", fontFooterBoxU));
            cell.BorderWidth = 0;
            tabFooter2.AddCell(cell, 0, 0);

            if (dtTotali.Rows[0]["DATVER"].ToString() != "")
            {
                string giorno = dtTotali.Rows[0]["DATVER"].ToString().Substring(0, 2);
                string mese = dtTotali.Rows[0]["DATVER"].ToString().Substring(3, 2);
                string anno = dtTotali.Rows[0]["DATVER"].ToString().Substring(6, 4);
                cell = new Cell(new Phrase(
                    "data operazione:  " + giorno + "-" + mese + "-" + anno + "        Importo del versamento:  € " +
                    Convert.ToDecimal(dtTotali.Rows[0]["IMPVER"]).ToString("#,##0.#0"), fontFooterBox));
            }
            else
                cell = new Cell(new Phrase("data operazione:  ___ ___ ______  Importo del versamento:  ________________", fontFooterBox));
            cell.BorderWidth = 0;
            tabFooter2.AddCell(cell, 0, 1);

            if (dtTotali.Rows[0]["CODMODPAG"].ToString() == "2")
                cell = new Cell(new Phrase("[X]  C/C Postale", fontFooterBox));
            else
                cell = new Cell(new Phrase("", fontFooterBox));
            cell.BorderWidth = 0;
            tabFooter2.AddCell(cell, 1, 0);

            if (dtTotali.Rows[0]["CODMODPAG"].ToString() == "2")
                cell = new Cell(new Phrase(
                    "Ufficio Postale:  " + dtTotali.Rows[0]["UFFPOS"].ToString().PadLeft(51, ' ') +
                    "  Città:  " + dtTotali.Rows[0]["CITDIC"].ToString().PadLeft(35, ' ') + " Prov.: " +
                    dtTotali.Rows[0]["PRODIC"].ToString().PadLeft(5, ' '), fontFooterBox));
            else
                cell = new Cell(new Phrase("Ufficio Postale: ___________________________________________________ Città: ___________________________________ Prov.: _____", fontFooterBox));
            cell.BorderWidth = 0;
            tabFooter2.AddCell(cell, 1, 1);

            if (dtTotali.Rows[0]["CODMODPAG"].ToString() == "3")
                cell = new Cell(new Phrase("[X]  Bonifico Bancario", fontFooterBox));
            else
                cell = new Cell(new Phrase("[  ]  Bonifico Bancario", fontFooterBox));
            cell.BorderWidth = 0;
            tabFooter2.AddCell(cell, 2, 0);

            if (dtTotali.Rows[0].IntElementAt("ANNDEN") > 2007)
            {
                if (dtTotali.Rows[0]["CODMODPAG"].ToString() == "3")
                    cell = new Cell(new Phrase("IBAN Azienda:  " + dtTotali.Rows[0]["IBAN"].ToString().Trim(), fontFooterBox));
                else
                    cell = new Cell(new Phrase("IBAN Azienda:  ___________________________________", fontFooterBox));
            }
            else if (dtTotali.Rows[0]["CODMODPAG"].ToString() == "3")
                cell = new Cell(new Phrase("IBAN Azienda:  " + dtTotali.Rows[0]["IBAN"].ToString().Trim(), fontFooterBox));
            else
                cell = new Cell(new Phrase("IBAN Azienda:  ___________________________________", fontFooterBox));
            cell.BorderWidth = 0;
            tabFooter2.AddCell(cell, 2, 1);

            if (dtTotali.Rows[0]["CODMODPAG"].ToString() == "4")
                cell = new Cell(new Phrase("[X]  Versamento compensato totalmente da credito precedente", fontFooterBox));
            else
                cell = new Cell(new Phrase("[  ]  Versamento compensato totalmente da credito precedente", fontFooterBox));
            cell.BorderWidth = 0;
            cell.Colspan = 2;
            tabFooter2.AddCell(cell, 3, 0);

            if (dtTotali.Rows[0]["CODMODPAG"].ToString() == "5")
                cell = new Cell(new Phrase("[X]  Versamento differito (con applicazioni delle sanzioni previste)", fontFooterBox));
            else
                cell = new Cell(new Phrase("[  ]  Versamento differito (con applicazioni delle sanzioni previste)", fontFooterBox));
            cell.BorderWidth = 0;
            cell.Colspan = 2;
            tabFooter2.AddCell(cell, 4, 0);

            if (dtTotali.Rows[0]["CODMODPAG"].ToString() == "6")
                cell = new Cell(new Phrase("[X]  Ritardato versamento per finanziamenti pubblici tardivamente erogati (delibera CdA n°38/98)", fontFooterBox));
            else
                cell = new Cell(new Phrase("[  ]  Ritardato versamento per finanziamenti pubblici tardivamente erogati (delibera CdA n°38/98)", fontFooterBox));
            cell.BorderWidth = 0;
            cell.Colspan = 2;
            tabFooter2.AddCell(cell, 5, 0);
        }
        else
        {
            Font fontFooterB = new Font(bfVerdana, 7.75F, Font.BOLD);

            int[] colonne2 = new int[3];
            colonne2[0] = 87;
            colonne2[1] = 3;
            colonne2[2] = 10;

            tabTotali.WidthPercentage = 100;
            tabTotali.Cellpadding = 2;
            tabTotali.Cellspacing = 0;
            tabTotali.SetWidths(colonne2);
            tabTotali.Offset = 20;
            tabTotali.BorderWidth = 0;

            if (codCau == "")
                totDescSan = "Sanzioni annullate";
            else if (datSanAnn != "")
                totDescSan = "Sanzioni annullate";
            else
            {
                strSql = $"SELECT VALUE(TASSO, 0.00) AS TASSO, TIPMOV, DESCAUREP AS DESC FROM TIPMOVCAU  WHERE CODCAU ='{codCau}' AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
                dtSanzioni.Clear();
                dtSanzioni = objDataAccess.GetDataTable(strSql);

                if (dtSanzioni.Rows.Count > 0)
                {
                    switch (dtSanzioni.Rows[0]["TIPMOV"].ToString().Trim())
                    {
                        case "SAN_MD":
                        {
                            if (dtSanzioni.Rows[0]["DESC"].ToString() != "")
                                totDescSan = $"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dtSanzioni.Rows[0]["DESC"].ToString())} annullate";
                                // TotDescSan = dtSanzioni.Rows[0]["DESC"].ToString().Substring(0, 1) +
                                //              dtSanzioni.Rows[0]["DESC"].ToString().ToLower().Substring(1,
                                //                  dtSanzioni.Rows[0]["DESC"].ToString().Length - 1) + " annullate";
                            else
                                totDescSan = "";
                            break;
                        }

                        case "SAN_RD":
                        {
                            if (dtSanzioni.Rows[0]["DESC"].ToString() != "")
                                totDescSan = $"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dtSanzioni.Rows[0]["DESC"].ToString())} annullate";
                                // TotDescSan = dtSanzioni.Rows[0]["DESC"].ToString().Substring(0, 1) +
                                //              dtSanzioni.Rows[0]["DESC"].ToString().ToLower().Substring(1,
                                //              dtSanzioni.Rows[0]["DESC"].ToString().Length - 1) + " annullate";
                            else
                                totDescSan = "";
                            break;
                        }
                    }
                }
                else
                    totDescSan = "";
            }

            cell = new Cell(new Paragraph(new Phrase("Totale generale CTR annullato", fontFooterBox)));
            cell.Add(new Paragraph(new Phrase(addizionale, fontFooterBox)));
            cell.Add(new Paragraph(new Phrase("Assistenza Contrattuale annullata", fontFooterBox)));
            cell.Add(new Paragraph(new Phrase("Abbonamenti P.A. annullati", fontFooterBox)));
            cell.Add(new Paragraph(new Phrase(totDescSan, fontFooterBox)));
            cell.Add(new Paragraph(new Phrase("TOTALE dovuto annullato", fontFooterB)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.BorderWidth = 0;
            tabTotali.AddCell(cell, 0, 0);

            cell = new Cell(new Paragraph(new Phrase("€", fontFooterBox)));
            cell.Add(new Paragraph(new Phrase("€", fontFooterBox)));
            cell.Add(new Paragraph(new Phrase("€", fontFooterBox)));
            cell.Add(new Paragraph(new Phrase("€", fontFooterBox)));
            cell.Add(new Paragraph(new Phrase("€", fontFooterBox)));
            cell.Add(new Paragraph(new Phrase("€", fontFooterB)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.BorderWidth = 0;
            tabTotali.AddCell(cell, 0, 1);

            if (dtTotali.Rows.Count > 0)
            {
                impTot += Convert.ToDecimal(dtTotali.Rows[0]["IMPCON"]);
                impTot += Convert.ToDecimal(dtTotali.Rows[0]["IMPADDREC"]);
                impTot += Convert.ToDecimal(dtTotali.Rows[0]["IMPASSCONAZI"]);
                impTot += Convert.ToDecimal(dtTotali.Rows[0]["IMPABBAZI"]);

                if (dtTotali.Rows[0]["SANSOTSOG"].ToString().Trim() == "S")
                    impTot += 0;
                else if (datSanAnn == "")
                    impTot += Convert.ToDecimal(dtTotali.Rows[0]["IMPSANDET"]);
                else
                    impTot += 0;

                impSanTot = datSanAnn == "" ? Convert.ToDecimal(dtTotali.Rows[0]["IMPSANDET"]).ToString("#,##0.#0") : "0,00";

                cell = new Cell(new Paragraph(new Phrase(Convert.ToDecimal(dtTotali.Rows[0]["IMPCON"]).ToString("#,##0.#0"), fontFooterBox)));
                cell.Add(new Paragraph(new Phrase(Convert.ToDecimal(dtTotali.Rows[0]["IMPADDREC"]).ToString("#,##0.#0"), fontFooterBox)));
                cell.Add(new Paragraph(new Phrase(Convert.ToDecimal(dtTotali.Rows[0]["IMPASSCONAZI"]).ToString("#,##0.#0"), fontFooterBox)));
                cell.Add(new Paragraph(new Phrase(Convert.ToDecimal(dtTotali.Rows[0]["IMPABBAZI"]).ToString("#,##0.#0"), fontFooterBox)));
                if (dtTotali.Rows[0]["SANSOTSOG"].ToString().Trim() == "S") 
                    cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
                else
                    cell.Add(new Paragraph(new Phrase(impSanTot, fontFooterBox)));
                cell.Add(new Paragraph(new Phrase(impTot.ToString("#,##0.#0"), fontFooterB)));
            }
            else
            {
                cell = new Cell(new Paragraph(new Phrase("0,00", fontFooterBox)));
                cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
                cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
                cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
                cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
                cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
            }

            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.BorderWidth = 0;
            tabTotali.AddCell(cell, 0, 2);
        }

        tabTotali.Offset = 140;
        tabFooter.Offset = 20;
        tabFooter2.Offset = 20;

        document.Add(tabTotali);
        document.Add(tabFooter);
        document.Add(tabFooter2);
    }
}