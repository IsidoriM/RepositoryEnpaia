using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using IBM.Data.DB2.iSeries;
using iTextSharp.text;
using iTextSharp.text.pdf;
using log4net;
using OCM.TFI.OCM.AziendaConsulente;
using OCM.TFI.OCM.Registrazione;
using OCM.TFI.OCM.Stampa;
using OCM.TFI.OCM.Utilities;
using TFI.BLL.Utilities.Pdf;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.AziendaConsulente;
using Utilities;
using Font = iTextSharp.text.Font;
using Image = iTextSharp.text.Image;
using Paragraph = iTextSharp.text.Paragraph;

namespace TFI.BLL.Utilities.Pdf;

public class PdfService
{
    private static readonly ILog _logger = LogManager.GetLogger("RollingFile");
    public static string FontName = "Arial";
    public static Font FontBold = FontFactory.GetFont(FontName, 11, Font.BOLD, new Color(0, 0, 0));
    public static Font FontGreen = FontFactory.GetFont(FontName, 11, Font.NORMAL, new Color(9, 113, 50));
    public BaseFont bfVerdana = BaseFont.CreateFont(PathFileHelper.FontPath, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);

    public BaseFont bfVerdanaBlack =
        BaseFont.CreateFont(PathFileHelper.FontPathBlack, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);

    public const int PdfPageSize = 20;

    public byte[] CreaPdfRicevutaRegistrazioneAzienda(DatiPdfRegistrazioneAzienda datiPdf, ref string errorMsg)
    {
        using var ms = new MemoryStream();
        using (var existingFileStream = new FileStream(PathFileHelper.TemplateRegistrazioneAzienda, FileMode.Open))
        {
            var pdfReader = new PdfReader(existingFileStream);
            var stamper = new PdfStamper(pdfReader, ms);

            var form = stamper.AcroFields;

            form.SetField("ORA", datiPdf.Protocollo.DataProtocollo.StandardizeTime());
            form.SetField("PARIVA", datiPdf.DatiAzienda.PartitaIva);
            form.SetField("NUMPROT", datiPdf.Protocollo.NumeroProtocollo);
            form.SetField("CODFIS", datiPdf.DatiAzienda.CodiceFiscale);
            form.SetField("DATA", datiPdf.Protocollo.DataProtocollo.StandardizeDateString(StandardUse.Readable));
            form.SetField("RAGSOC", datiPdf.DatiAzienda.RagioneSociale);

            stamper.FormFlattening = true;

            stamper.Close();
            pdfReader.Close();
            return ms.GetBuffer();
        }
    }

    public static byte[] CreaPdfRicevutaDipa(DatiPdfDatiTotaliDenuncia datiPdfDatiTotaliDenuncia, ref string errorMsg)
    {
        try
        {
            var dataLayer = new DataLayer();
            var datiTotaliDenuncia = datiPdfDatiTotaliDenuncia.DatiTotaliDenuncia;
            var dataToFillRicevutaFromDb = GetDataToFillRicevutaFromDb();
            var ricevutaDipaPdfFields = GetRicevutaDipaPdfFields();
            
            string fileNameExisting = PathFileHelper.PathTemplateRicevutaDipa;
            using var ms = new MemoryStream();
            using (var existingFileStream = new FileStream(fileNameExisting, FileMode.Open))
            {
                var pdfReader = new PdfReader(existingFileStream);
                var stamper = new PdfStamper(pdfReader, ms);
                var form = stamper.AcroFields;

                SetPdfAcrofields();

                stamper.FormFlattening = true;

                stamper.Close();
                pdfReader.Close();
                return ms.GetBuffer();

                void SetPdfAcrofields()
                {
                    form.SetField("protocollo", ricevutaDipaPdfFields.ProtocolloField);
                    form.SetField("protnum", ricevutaDipaPdfFields.NumeroProtocolloField);
                    form.SetField("protdat", ricevutaDipaPdfFields.DataProtocolloField);
                    form.SetField("PR", ricevutaDipaPdfFields.PrRicevutaField);
                    form.SetField("PERIODO", ricevutaDipaPdfFields.PeriodoDipaField);
                    form.SetField("DATA", ricevutaDipaPdfFields.DataDipaRicevutaField);
                    form.SetField("ORA", ricevutaDipaPdfFields.OraDipaRicevutaField);
                    form.SetField("NOMINATIVO", ricevutaDipaPdfFields.NominativoDipaRicevutaField);
                    form.SetField("NUMPROG", ricevutaDipaPdfFields.NumeroProgressivoRicevutaField);
                    form.SetField("STATO", ricevutaDipaPdfFields.StatoRicevutaField);
                    form.SetField("TOTDOV", ricevutaDipaPdfFields.TotaleDovutoRicevutaField);
                    form.SetField("TOTPAG", ricevutaDipaPdfFields.TotaleDaPagareRicevutaField);
                }
            }

            DataRow GetDataToFillRicevutaFromDb()
            {
                var codPosParameter = dataLayer.CreateParameter(DbParameters.CodicePosizione, iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, datiTotaliDenuncia.CodPos);
                var annDenParameter = dataLayer.CreateParameter(DbParameters.AnnoDenuncia, iDB2DbType.iDB2Decimal, 4, ParameterDirection.Input, datiTotaliDenuncia.Anno.ToString());
                var mesDenParameter = dataLayer.CreateParameter(DbParameters.MeseDenuncia, iDB2DbType.iDB2Decimal, 2, ParameterDirection.Input, datiTotaliDenuncia.Mese.ToString());
                var proDenParameter = dataLayer.CreateParameter(DbParameters.ProgressivoDenuncia, iDB2DbType.iDB2Decimal, 3, ParameterDirection.Input, datiTotaliDenuncia.ProDen.ToString());

                var getDataToFillRicevutaSqlQuery = "SELECT DATSANANN, SANSOTSOG, NUMRIC, DATCHI, TIPMOV, IMPDIS, IMPDEC, IMPSANDET, CODMODPAG , DATCONMOV, " +
                    "(SELECT MESE FROM MESI WHERE CODMES = DENTES.MESDEN) AS DENMESE, (SELECT RAGSOC FROM AZI WHERE DENTES.CODPOS = CODPOS) AS RAGSOC FROM DENTES " +
                    $"WHERE CODPOS = {DbParameters.CodicePosizione} AND ANNDEN = {DbParameters.AnnoDenuncia} AND MESDEN = {DbParameters.MeseDenuncia} AND PRODEN = {DbParameters.ProgressivoDenuncia}";
                var dataToFillRicevutaResultSet = dataLayer
                    .GetDataTableWithParameters(getDataToFillRicevutaSqlQuery, codPosParameter, annDenParameter, mesDenParameter, proDenParameter);

                if (dataToFillRicevutaResultSet.Rows.Count <= 0)
                    throw new Exception("Dati per compilazione PDF non risultano presenti nel DataBase");

                return dataToFillRicevutaResultSet.Rows[0];
            }

            RicevutaDipaPdfFields GetRicevutaDipaPdfFields()
            {
                var datChi = dataToFillRicevutaFromDb.ElementAt("DATCHI").Split(' ');
                var ragSoc = dataToFillRicevutaFromDb.ElementAt("RAGSOC");
                var impDec = dataToFillRicevutaFromDb.ElementAt("IMPDEC");
                var totale = dataToFillRicevutaFromDb.DecimalElementAt("IMPDIS");

                var prRicevutaDipa = "Protocollo ENPAIA";
                var numeroProtocollo = datiPdfDatiTotaliDenuncia.Protocollo.ProtocolloCompleto.Split(';')[1];
                var periodoDipa = $"{dataToFillRicevutaFromDb.ElementAt("DENMESE")} {datiTotaliDenuncia.Anno}";
                var dataDipa = datChi[0];
                var oraDipa = datChi[1];
                var nominativoDipa = $"{datiTotaliDenuncia.CodPos} - {ragSoc}";
                var numProgRicevuta = dataToFillRicevutaFromDb.ElementAt("NUMRIC");
                var statoRicevuta = GetStatoRicevuta();
                var totaleDovutoRicevuta = $"€ {totale}";
                var totaleDaPagareRicevuta = GetTotaleDaPagareRicevuta();

                return new RicevutaDipaPdfFields
                {
                    ProtocolloField = datiPdfDatiTotaliDenuncia.Protocollo.ProtocolloCompleto,
                    NumeroProtocolloField = $"Num. {numeroProtocollo}",
                    DataProtocolloField = $"Data {datiPdfDatiTotaliDenuncia.Protocollo.DataProtocollo}",
                    PrRicevutaField = prRicevutaDipa,
                    PeriodoDipaField = periodoDipa,
                    DataDipaRicevutaField = dataDipa,
                    OraDipaRicevutaField = oraDipa,
                    NominativoDipaRicevutaField = nominativoDipa,
                    NumeroProgressivoRicevutaField = numProgRicevuta,
                    StatoRicevutaField = statoRicevuta,
                    TotaleDovutoRicevutaField = totaleDovutoRicevuta,
                    TotaleDaPagareRicevutaField = totaleDaPagareRicevuta
                };

                string GetStatoRicevuta()
                {
                    if (dataToFillRicevutaFromDb.ElementAt("CODMODPAG") == "")
                        return "Acquisito senza dichiarazione di pagamento";
                    else
                        return "Acquisito con dichiarazione di pagamento";
                }

                string GetTotaleDaPagareRicevuta()
                {
                    var totaleDaPagare = string.Empty;
                    if (dataToFillRicevutaFromDb.ElementAt("SANSOTSOG") == "S" || dataToFillRicevutaFromDb.ElementAt("DATSANANN") != "")
                        totaleDaPagare = (totale - decimal.Parse(impDec)).ToString();
                    else
                        totaleDaPagare = (totale + (dataToFillRicevutaFromDb.DecimalElementAt("IMPDIS") - decimal.Parse(impDec))).ToString();
                    return $"€ {totaleDaPagare}";
                }
            }
        }
        catch (Exception ex)
        {
            errorMsg += "Errore nella generazione del PDF della ricevuta";
            _logger.Info($"[TFI.BLL] : PdfService - Generazione Ricevuta Dipa - E' stata generata un'eccezione in data: {DateTime.Now}. Messaggio: {ex.Message}");
            throw ex;
        }
    }

    public byte[] StampaDipa(DatiNuovaDenuncia datiDenuncia, string codicePosizione)
    {
        var records = datiDenuncia.ListaReport;
        var numberPage = Convert.ToInt32(Math.Ceiling(records.Count / Convert.ToDecimal(PdfPageSize)) + 1M);
        

        using var memoryStream = new MemoryStream();
        var document = new Document(PageSize.A4.Rotate(), 15, 15, 15, 15);
        var pdfWriter = PdfWriter.GetInstance(document, memoryStream);
        
        var movAnn = GetAnnullamento(int.Parse(datiDenuncia.Anno), datiDenuncia.IntMese, datiDenuncia.ProDen,
            codicePosizione);
        
        document.Open();
        for (var page = 1; page < numberPage; page++)
        {
            if (!string.IsNullOrWhiteSpace(movAnn.DatMovAnn))
                SetIntestazioneAnnullamento(pdfWriter);
            
            ScriviIntestazione();
            var actualRecords = records.Skip(PdfPageSize * (page - 1)).Take(PdfPageSize).ToList();
            WriteTablePage();
            WritePieDiPagina(pdfWriter, document, page, numberPage);
            
            document.NewPage();

            void WriteTablePage()
            {
                document.Add(CreaIntestazioneTabella());

                foreach (var record in actualRecords)
                {
                    var recordTabella = new Table(17, 2);
                    recordTabella.SetWidths(new int[] {5, 22, 6, 4, 4, 8, 5, 5, 2, 7, 2, 7, 2, 7, 6, 4, 4});
                    recordTabella.WidthPercentage = 100;
                    recordTabella.BorderWidth = 0;
                    recordTabella.Cellspacing = 0;
                    recordTabella.Cellpadding = 2;
                    WriteRecordDipa(recordTabella, record);
                    document.Add(recordTabella);
                }
            }
        }
        if (!string.IsNullOrWhiteSpace(movAnn.DatMovAnn))
            SetIntestazioneAnnullamento(pdfWriter);

        ScriviIntestazione();
        ScriviTotali(document, int.Parse(datiDenuncia.Anno), datiDenuncia.IntMese, datiDenuncia.ProDen, codicePosizione, movAnn.NumMovAnn, movAnn.DatSanAnn);
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
            pdfContentByte.ShowTextAligned(0, "Denuncia e pagamento del mese di __________________ Anno _________", 17, 495, 0);
            pdfContentByte.ShowTextAligned(0, datiDenuncia.StrMese, 165, 497, 0);
            pdfContentByte.ShowTextAligned(0, datiDenuncia.Anno, 250, 497, 0);
            pdfContentByte.EndText();

            pdfContentByte.BeginText();
            pdfContentByte.SetFontAndSize(bfVerdana, 9);
            pdfContentByte.ShowTextAligned(0,
                "Denominazione _____________________________________________________________________", 52, 460, 0);
            pdfContentByte.ShowTextAligned(0, datiDenuncia.DatiAnagrafici.RagioneSociale, 135, 462, 0);
            pdfContentByte.EndText();

            pdfContentByte.BeginText();
            pdfContentByte.SetFontAndSize(bfVerdana, 9);
            pdfContentByte.ShowTextAligned(0, "Posizione assicurativa ENPAIA ______________", 590, 460, 0);
            pdfContentByte.ShowTextAligned(0, codicePosizione, 720, 462, 0);
            pdfContentByte.EndText();

            pdfContentByte.BeginText();
            pdfContentByte.SetFontAndSize(bfVerdana, 9);
            pdfContentByte.ShowTextAligned(0,
                "Telefono __________________________________ E-mail __________________________________", 52, 445, 0);
            pdfContentByte.ShowTextAligned(0, datiDenuncia.DatiSedeLegale.Telefono, 105, 447, 0);
            pdfContentByte.ShowTextAligned(0, datiDenuncia.DatiSedeLegale.EMail, 250, 447, 0);
            pdfContentByte.EndText();

            pdfContentByte.BeginText();
            pdfContentByte.SetFontAndSize(bfVerdana, 9);
            pdfContentByte.ShowTextAligned(0, "Abbonamento P.A. Azienda", 550, 445, 0);
            if (datiDenuncia.DatiAnagrafici.Abb == 'S')
                pdfContentByte.ShowTextAligned(0, "X", 693, 445, 0);
            pdfContentByte.EndText();

            var grx = new Graphic();
            grx.Rectangle(690, 442, 13, 13);
            grx.Stroke();
            document.Add(grx);

            grx.Rectangle(15, 438, 810, 40);
            grx.Stroke();
            document.Add(grx);
        }
    }

    public void SetIntestazioneAnnullamento(PdfWriter pdfWriter)
    {
        var bfAnnullamento =
            BaseFont.CreateFont(PathFileHelper.FontPathBlack, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        var pdfContentByte = pdfWriter.DirectContent;
        pdfContentByte.BeginText();
        pdfContentByte.SetFontAndSize(bfAnnullamento, 6.5F);
        pdfContentByte.ShowTextAligned(1, "DOCUMENTO ANNULLATO", 300, 575, 0);
        pdfContentByte.EndText();
    }
    public DettaglioAnnullamentoDenuncia GetAnnullamento(int ANNDEN, int MESDEN, int PRODEN, string CODPOS)
    {
        var objDataAccess = new DataLayer();
        var movAnnSqlQuery =
            $"SELECT NUMMOVANN, DATMOVANN, DATSANANN FROM DENTES WHERE ANNDEN = {ANNDEN} AND MESDEN = {MESDEN} AND PRODEN = {PRODEN}" +
            $" AND CODPOS = {CODPOS}";
        var movAnn = objDataAccess.GetDataTable(movAnnSqlQuery);
        var NUMMOVANN = movAnn.Rows[0].ElementAt("NUMMOVANN");
        var DATMOVANN = movAnn.Rows[0].ElementAt("DATMOVANN");
        var DATSANANN = movAnn.Rows[0].ElementAt("DATSANANN");
        return new() { NumMovAnn = NUMMOVANN, DatMovAnn = DATMOVANN, DatSanAnn = DATSANANN};
    }

    public string GetNumMov(int annDen, int mesDen, int proDen, string codPos)
    {
        var objDataAccess = new DataLayer();
        var numMovSqlQuery =
            $"SELECT NUMMOV FROM DENTES WHERE ANNDEN = {annDen} AND MESDEN = {mesDen} AND PRODEN = {proDen} AND CODPOS = {codPos}";
        return objDataAccess.Get1ValueFromSQL(numMovSqlQuery, CommandType.Text);
    }

    public string GetAbb(int annDen, int mesDen, int proDen, string codPos)
    {
        var objDataAccess = new DataLayer();
        var abbSqlQuery =
            $"SELECT ABB FROM AZISTO WHERE CODPOS = {codPos} AND {DBMethods.DoublePeakForSql(DBMethods.Db2Date(string.Format("01/{0}/{1}", Utils.FixMese(mesDen), annDen)))} <= VALUE(DATFIN, '9999-12-31') ORDER BY DATFIN DESC";
        return objDataAccess.Get1ValueFromSQL(abbSqlQuery, CommandType.Text);
    }
    private void ScriviTotali(Document document, int ANNDEN, int MESDEN, int PRODEN, string CODPOS, string NUMMOVANN, string DATSANANN)
    {
        var objDataAccess = new DataLayer();

        Table tabTotali = new Table(3, 1);
        Table tabFooter = new Table(7, 4);
        Table tabFooter2 = new Table(2, 6);
        decimal IMPTOT = 0;
        string IMPSANTOT;
        string addizionale;

        DataTable dtSanzioni = new DataTable();
        DataTable dtTotali = new DataTable();

        string strSQl;
        string CODCAU = "";
        string TotDescSan = "";
        string TotDescSan2 = "";
        decimal IMPDEC = 0;
        Int32 KK = 0;
        decimal IMPTOT_DEC = 0;

        addizionale = NUMMOVANN == "" ? "Addizionale" : "Addizionale annullato";
        
        strSQl =
            "SELECT DENTES.SANSOTSOG, DENTES.IMPCON, DENTES.DATVER, DENTES.IMPVER, DENTES.UFFPOS, DENTES.CITDIC, DENTES.PRODIC,";
        strSQl +=
            " DENTES.IBAN, DENTES.ABIDIC, DENTES.CABDIC, VALUE(DENTES.IMPADDREC, 0.0) AS IMPADDREC, VALUE(DENTES.IMPASSCON, 0.0) AS IMPASSCONAZI,";
        strSQl +=
            " DENTES.IMPABB AS IMPABBAZI, DENTES.IMPDEC, DENTES.ANNDEN, VALUE(DENTES.IMPSANDET, 0.0) AS IMPSANDET, DENTES.CODMODPAG,";
        strSQl += " DENTES.CODCAUSAN";
        strSQl += " FROM  DENTES ";
        strSQl += " WHERE ANNDEN = " + ANNDEN;
        strSQl += " AND MESDEN = " + MESDEN;
        strSQl += " AND PRODEN = " + PRODEN;
        strSQl += " AND CODPOS = " + CODPOS;
        dtTotali = objDataAccess.GetDataTable(strSQl);
        
        CODCAU = "" + dtTotali.Rows[0]["CODCAUSAN"];
        IMPDEC = dtTotali.Rows[0].DecimalElementAt("IMPDEC").Value;
        
        Cell Cell;
        iTextSharp.text.Font fontFooterBox = new iTextSharp.text.Font(bfVerdana, 7.75F, Font.BOLD);
        iTextSharp.text.Font fontFooterBoxU = new iTextSharp.text.Font(bfVerdana, 7.75F, Font.UNDERLINE);
        iTextSharp.text.Font fontFooterBoxSmall = new iTextSharp.text.Font(bfVerdana, 6.75F, Font.NORMAL);
        
        if (NUMMOVANN == "")
        {
            Table tabDettFooter = new Table(1, 3);
            tabDettFooter.WidthPercentage = 100;
            tabDettFooter.BorderWidth = 0;
            tabDettFooter.Cellspacing = 0;
            tabDettFooter.Cellpadding = 1;
            tabDettFooter.AutoFillEmptyCells = true;
            
            iTextSharp.text.Font fontFooter =
                new iTextSharp.text.Font(bfVerdana, 7.75F, Font.BOLD, new Color(255, 255, 255));
            iTextSharp.text.Font fontFooterB = new iTextSharp.text.Font(bfVerdana, 7.75F, Font.BOLD);

            Cell = new Cell(new Phrase("Il/La sottoscritto/a ________________________________________________________",
                fontFooterBox));
            Cell.BorderWidth = 0;
            tabDettFooter.AddCell(Cell, 0, 0);
            
            fontFooterBox = new iTextSharp.text.Font(bfVerdana, 7.75F, Font.NORMAL);
            
            string strAppo =
                "dichiara ai sensi della legge n.15 del 31 gennaio 1968 e successive modifiche ed integrazioni,";
            strAppo +=
                " sotto la propria responsabilità civile e penale, che le informazioni e i dati contenuti nel presente modulo";
            strAppo +=
                " sono rispondenti al vero e si impegna a comunicare entro 30 giorni qualsiasi variazione riguardante";
            strAppo += " le situazioni dichiarate.";

            Cell = new Cell(new Phrase(strAppo, fontFooterBox));
            Cell.BorderWidth = 0;
            tabDettFooter.AddCell(Cell, 1, 0);
            
            strAppo = "      Data ____/____/______     Timbro della Ditta e Firma __________________________";
            Cell = new Cell(new Phrase(strAppo, fontFooterBox));
            Cell.BorderWidth = 0;
            tabDettFooter.AddCell(Cell, 2, 0);

            int[] colonne2 = new int[7];
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
            
            Cell = new Cell("");
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 0, 0);
            
            Cell = new Cell("");
            Cell.GrayFill = 0.3F;
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 0, 1);
            
            Cell = new Cell(new Phrase("Dichiarazione di responsabilità", fontFooter));
            Cell.HorizontalAlignment = Element.ALIGN_CENTER;
            Cell.GrayFill = 0.3F;
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 0, 2);
            
            Cell = new Cell("");
            Cell.GrayFill = 0.3F;
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 0, 3);
            
            Cell = new Cell("");
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 0, 4);

            Cell = new Cell("");
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 0, 5);

            Cell = new Cell("");
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 0, 6);
            
            Cell = new Cell("");
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 1, 0);
            
            Cell = new Cell("");
            Cell.GrayFill = 0.3F;
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 1, 1);
            
            Cell = new Cell(tabDettFooter);
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 1, 2);
            
            Cell = new Cell("");
            Cell.GrayFill = 0.3F;
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 1, 3);


            if (CODCAU == "")
                TotDescSan = "Sanzioni";
            else if (DATSANANN != "")
                TotDescSan = "Sanzioni";
            else
            {
                strSQl = "SELECT VALUE(TASSO, 0.00) AS TASSO, TIPMOV, DESCAUREP AS DESC FROM TIPMOVCAU ";
                strSQl += " WHERE CODCAU ='" + CODCAU + "' AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
                dtSanzioni.Clear();
                dtSanzioni = objDataAccess.GetDataTable(strSQl);
                if (dtSanzioni.Rows.Count > 0)
                {
                    for (KK = 0; KK <= dtSanzioni.Rows.Count - 1; KK++)
                    {
                        switch (dtSanzioni.Rows[KK]["TIPMOV"].ToString().Trim())
                        {
                            case "SAN_MD":
                            {
                                if (dtSanzioni.Rows[KK]["DESC"].ToString() != "")
                                    TotDescSan = dtSanzioni.Rows[KK]["DESC"].ToString().Substring(0, 1) +
                                                 dtSanzioni.Rows[KK]["DESC"].ToString().ToLower().Substring(1,
                                                     dtSanzioni.Rows[KK]["DESC"].ToString().Length - 1);
                                else
                                    TotDescSan = "";
                                break;
                            }

                            case "SAN_RD":
                            {
                                if (dtSanzioni.Rows[KK]["DESC"].ToString() != "")
                                    TotDescSan = dtSanzioni.Rows[KK]["DESC"].ToString().Substring(0, 1) +
                                                 dtSanzioni.Rows[KK]["DESC"].ToString().ToLower().Substring(1,
                                                     dtSanzioni.Rows[KK]["DESC"].ToString().Length - 1);
                                else
                                    TotDescSan = "";
                                break;
                            }
                        }
                    }
                }
                else
                    TotDescSan = "";
            }
            
            Cell = new Cell(new Paragraph(new Phrase("Totale generale CTR", fontFooterBox)));
            Cell.Add(new Paragraph(new Phrase(addizionale, fontFooterBox))); // Addizionale
            Cell.Add(new Paragraph(new Phrase("Assistenza Contrattuale", fontFooterBox)));
            Cell.Add(new Paragraph(new Phrase("Abbonamenti P.A.", fontFooterBox)));
            Cell.Add(new Paragraph(new Phrase(TotDescSan, fontFooterBox)));
            Cell.Add(new Paragraph(new Phrase("Importo decurtato", fontFooterBox)));
            Cell.Add(new Paragraph(new Phrase("TOTALE dovuto", fontFooterB)));
            Cell.Add(new Paragraph(new Phrase("TOTALE da pagare", fontFooterB)));
            Cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 1, 4);

            Cell = new Cell(new Paragraph(new Phrase("€", fontFooterBox)));
            Cell.Add(new Paragraph(new Phrase("€", fontFooterBox)));
            Cell.Add(new Paragraph(new Phrase("€", fontFooterBox)));
            Cell.Add(new Paragraph(new Phrase("€", fontFooterBox)));
            Cell.Add(new Paragraph(new Phrase("€", fontFooterBox)));
            Cell.Add(new Paragraph(new Phrase("€", fontFooterBox)));
            Cell.Add(new Paragraph(new Phrase("€", fontFooterB)));
            Cell.Add(new Paragraph(new Phrase("€", fontFooterB)));
            Cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 1, 5);

            if (dtTotali.Rows.Count > 0)
            {
                IMPTOT += Convert.ToDecimal(dtTotali.Rows[0]["IMPCON"]);
                IMPTOT += Convert.ToDecimal(dtTotali.Rows[0]["IMPADDREC"]);
                IMPTOT += Convert.ToDecimal(dtTotali.Rows[0]["IMPASSCONAZI"]);
                IMPTOT += Convert.ToDecimal(dtTotali.Rows[0]["IMPABBAZI"]);

                if (IMPDEC > 0)
                    IMPTOT = IMPTOT - IMPDEC;

                if (dtTotali.Rows[0]["SANSOTSOG"].ToString().Trim() == "S")
                    IMPTOT += 0;
                else if (DATSANANN.ToString() == "")
                    IMPTOT += Convert.ToDecimal(dtTotali.Rows[0]["IMPSANDET"]);
                else
                    IMPTOT += 0;

                if (DATSANANN.ToString() == "")
                    IMPSANTOT = Convert.ToDecimal(dtTotali.Rows[0]["IMPSANDET"]).ToString("#,##0.#0");
                else
                    IMPSANTOT = "0,00";

                Cell = new Cell(new Paragraph(
                    new Phrase(Convert.ToDecimal(dtTotali.Rows[0]["IMPCON"]).ToString("#,##0.#0"), fontFooterBox)));
                Cell.Add(new Paragraph(new Phrase(Convert.ToDecimal(dtTotali.Rows[0]["IMPADDREC"]).ToString("#,##0.#0"),
                    fontFooterBox)));
                Cell.Add(new Paragraph(new Phrase(
                    Convert.ToDecimal(dtTotali.Rows[0]["IMPASSCONAZI"]).ToString("#,##0.#0"), fontFooterBox)));
                Cell.Add(new Paragraph(new Phrase(Convert.ToDecimal(dtTotali.Rows[0]["IMPABBAZI"]).ToString("#,##0.#0"),
                    fontFooterBox)));

                if (dtTotali.Rows[0]["SANSOTSOG"].ToString().Trim() == "S")
                    Cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
                else
                    Cell.Add(new Paragraph(new Phrase(IMPSANTOT, fontFooterBox)));
                Cell.Add(new Paragraph(new Phrase(IMPDEC.ToString("#,##0.#0"), fontFooterBox)));
                IMPTOT_DEC = (IMPTOT + IMPDEC);
                Cell.Add(new Paragraph(new Phrase(IMPTOT_DEC.ToString("#,##0.#0"), fontFooterB)));
                Cell.Add(new Paragraph(new Phrase(IMPTOT.ToString("#,##0.#0"), fontFooterB)));
            }
            else
            {
                Cell = new Cell(new Paragraph(new Phrase("0,00", fontFooterBox)));
                Cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
                Cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
                Cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
                Cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
                Cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
                Cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
                Cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
            }
            
            Cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 1, 6);

            Cell = new Cell("");
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 2, 0);

            Cell = new Cell("");
            Cell.GrayFill = 0.3F;
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 2, 1);
            
            Cell = new Cell("");
            Cell.GrayFill = 0.3F;
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 2, 2);

            Cell = new Cell("");
            Cell.GrayFill = 0.3F;
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 2, 3);
            
            Cell = new Cell("");
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 2, 4);

            Cell = new Cell("");
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 2, 5);

            Cell = new Cell("");
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 2, 6);

            Cell = new Cell("");
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 3, 0);

            Cell = new Cell("");
            Cell.GrayFill = 0.3F;
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 3, 1);

            Cell = new Cell("");
            Cell.GrayFill = 0.3F;
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 3, 2);

            Cell = new Cell("");
            Cell.GrayFill = 0.3F;
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 3, 3);
            
            Cell = new Cell("");
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 3, 4);

            Cell = new Cell("");
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 3, 5);

            Cell = new Cell("");
            Cell.BorderWidth = 0;
            tabFooter.AddCell(Cell, 3, 6);
            
            // Secondo Footer ----------------------------------------------------------------------
            int[] colonne3 = new int[2];
            colonne3[0] = 20;
            colonne3[1] = 80;
            tabFooter2.AutoFillEmptyCells = true;
            tabFooter2.WidthPercentage = 100;
            tabFooter2.Cellpadding = 2;
            tabFooter2.Cellspacing = 0;
            tabFooter2.BorderWidth = 0;
            tabFooter2.SetWidths(colonne3);
            
            Cell = new Cell(new Phrase("Riferimenti del versamento", fontFooterBoxU));
            Cell.BorderWidth = 0;
            tabFooter2.AddCell(Cell, 0, 0);
            
            if (dtTotali.Rows[0]["DATVER"].ToString() != "")
            {
                string GIORNO = dtTotali.Rows[0]["DATVER"].ToString().Substring(0, 2);
                string MESE = dtTotali.Rows[0]["DATVER"].ToString().Substring(3, 2);
                string ANNO = dtTotali.Rows[0]["DATVER"].ToString().Substring(6, 4);
                Cell = new Cell(new Phrase(
                    "data operazione:  " + GIORNO + "-" + MESE + "-" + ANNO + "        Importo del versamento:  € " +
                    Convert.ToDecimal(dtTotali.Rows[0]["IMPVER"]).ToString("#,##0.#0"), fontFooterBox));
            }
            else
                Cell = new Cell(new Phrase(
                    "data operazione:  ___ ___ ______  Importo del versamento:  ________________", fontFooterBox));

            Cell.BorderWidth = 0;
            tabFooter2.AddCell(Cell, 0, 1);
            
            if (dtTotali.Rows[0]["CODMODPAG"].ToString() == "2")
                Cell = new Cell(new Phrase("[X]  C/C Postale", fontFooterBox));
            else
                Cell = new Cell(new Phrase("", fontFooterBox));

            Cell.BorderWidth = 0;
            tabFooter2.AddCell(Cell, 1, 0);
            
            if (dtTotali.Rows[0]["CODMODPAG"].ToString() == "2")
                Cell = new Cell(new Phrase(
                    "Ufficio Postale:  " + dtTotali.Rows[0]["UFFPOS"].ToString().PadLeft(51, ' ') +
                    "  Città:  " + dtTotali.Rows[0]["CITDIC"].ToString().PadLeft(35, ' ') + " Prov.: " +
                    dtTotali.Rows[0]["PRODIC"].ToString().PadLeft(5, ' '), fontFooterBox));
            else
                Cell = new Cell(new Phrase("", fontFooterBox));
            Cell.BorderWidth = 0;
            tabFooter2.AddCell(Cell, 1, 1);

            if (dtTotali.Rows[0]["CODMODPAG"].ToString() == "3")
                Cell = new Cell(new Phrase("[X]  Bonifico Bancario", fontFooterBox));
            else
                Cell = new Cell(new Phrase("[  ]  Bonifico Bancario", fontFooterBox));
            Cell.BorderWidth = 0;
            tabFooter2.AddCell(Cell, 2, 0);

            if (dtTotali.Rows[0].IntElementAt("ANNDEN") > 2007)
            {
                if (dtTotali.Rows[0]["CODMODPAG"].ToString() == "3")
                    Cell = new Cell(new Phrase("IBAN Azienda:  " + dtTotali.Rows[0]["IBAN"].ToString().Trim(),
                        fontFooterBox));
                else
                    Cell = new Cell(new Phrase("IBAN Azienda:  ___________________________________", fontFooterBox));
            }
            else if (dtTotali.Rows[0]["CODMODPAG"].ToString() == "3")
                Cell = new Cell(new Phrase("IBAN Azienda:  " + dtTotali.Rows[0]["IBAN"].ToString().Trim(),
                    fontFooterBox));
            else
                Cell = new Cell(new Phrase("IBAN Azienda:  ___________________________________", fontFooterBox));
            Cell.BorderWidth = 0;
            tabFooter2.AddCell(Cell, 2, 1);

            if (dtTotali.Rows[0]["CODMODPAG"].ToString() == "4")
                Cell = new Cell(
                    new Phrase("[X]  Versamento compensato totalmente da credito precedente", fontFooterBox));
            else
                Cell = new Cell(new Phrase("[  ]  Versamento compensato totalmente da credito precedente",
                    fontFooterBox));
            Cell.BorderWidth = 0;
            Cell.Colspan = 2;
            tabFooter2.AddCell(Cell, 3, 0);

            if (dtTotali.Rows[0]["CODMODPAG"].ToString() == "6")
                Cell = new Cell(new Phrase("[X]  Versamento differito (con applicazioni delle sanzioni previste)",
                    fontFooterBox));
            else
                Cell = new Cell(new Phrase("[  ]  Versamento differito (con applicazioni delle sanzioni previste)",
                    fontFooterBox));
            Cell.BorderWidth = 0;
            Cell.Colspan = 2;
            tabFooter2.AddCell(Cell, 4, 0);

            if (dtTotali.Rows[0]["CODMODPAG"].ToString() == "5")
                Cell = new Cell(new Phrase(
                    "[X]  Ritardato versamento per finanziamenti pubblici tardivamente erogati (delibera CdA n°38/98)",
                    fontFooterBox));
            else
                Cell = new Cell(new Phrase(
                    "[  ]  Ritardato versamento per finanziamenti pubblici tardivamente erogati (delibera CdA n°38/98)",
                    fontFooterBox));
            Cell.BorderWidth = 0;
            Cell.Colspan = 2;
            tabFooter2.AddCell(Cell, 5, 0);
        }
        else
        {
            iTextSharp.text.Font fontFooterB = new iTextSharp.text.Font(bfVerdana, 7.75F, Font.BOLD);
            
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
            
            if (CODCAU == "")
                TotDescSan = "Sanzioni annullate";
            else if (DATSANANN != "")
                TotDescSan = "Sanzioni annullate";
            else
            {
                strSQl = "SELECT VALUE(TASSO, 0.00) AS TASSO, TIPMOV, DESCAUREP AS DESC FROM TIPMOVCAU ";
                strSQl += " WHERE CODCAU ='" + CODCAU + "' AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
                dtSanzioni.Clear();
                dtSanzioni = objDataAccess.GetDataTable(strSQl);

                if (dtSanzioni.Rows.Count > 0)
                {
                    switch (dtSanzioni.Rows[0]["TIPMOV"].ToString().Trim())
                    {
                        case "SAN_MD":
                        {
                            if (dtSanzioni.Rows[0]["DESC"].ToString() != "")
                                TotDescSan = dtSanzioni.Rows[0]["DESC"].ToString().Substring(0, 1) +
                                             dtSanzioni.Rows[0]["DESC"].ToString().ToLower().Substring(1,
                                                 dtSanzioni.Rows[0]["DESC"].ToString().Length - 1) + " annullate";
                            else
                                TotDescSan = "";
                            break;
                        }

                        case "SAN_RD":
                        {
                            if (dtSanzioni.Rows[0]["DESC"].ToString() != "")
                                TotDescSan = dtSanzioni.Rows[0]["DESC"].ToString().Substring(0, 1) +
                                             dtSanzioni.Rows[0]["DESC"].ToString().ToLower().Substring(1,
                                                 dtSanzioni.Rows[0]["DESC"].ToString().Length - 1) + " annullate";
                            else
                                TotDescSan = "";
                            break;
                        }
                    }
                }
                else
                    TotDescSan = "";
            }
            
            Cell = new Cell(new Paragraph(new Phrase("Totale generale CTR annullato", fontFooterBox)));
            Cell.Add(new Paragraph(new Phrase(addizionale, fontFooterBox))); // Addizionale
            Cell.Add(new Paragraph(new Phrase("Assistenza Contrattuale annullata", fontFooterBox)));
            Cell.Add(new Paragraph(new Phrase("Abbonamenti P.A. annullati", fontFooterBox)));
            Cell.Add(new Paragraph(new Phrase(TotDescSan, fontFooterBox)));
            Cell.Add(new Paragraph(new Phrase("TOTALE dovuto annullato", fontFooterB)));
            Cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            Cell.BorderWidth = 0;
            tabTotali.AddCell(Cell, 0, 0);

            Cell = new Cell(new Paragraph(new Phrase("€", fontFooterBox)));
            Cell.Add(new Paragraph(new Phrase("€", fontFooterBox)));
            Cell.Add(new Paragraph(new Phrase("€", fontFooterBox)));
            Cell.Add(new Paragraph(new Phrase("€", fontFooterBox)));
            Cell.Add(new Paragraph(new Phrase("€", fontFooterBox)));
            Cell.Add(new Paragraph(new Phrase("€", fontFooterB)));
            Cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            Cell.BorderWidth = 0;
            tabTotali.AddCell(Cell, 0, 1);
            
            if (dtTotali.Rows.Count > 0)
            {
                IMPTOT += Convert.ToDecimal(dtTotali.Rows[0]["IMPCON"]);
                IMPTOT += Convert.ToDecimal(dtTotali.Rows[0]["IMPADDREC"]);
                IMPTOT += Convert.ToDecimal(dtTotali.Rows[0]["IMPASSCONAZI"]);
                IMPTOT += Convert.ToDecimal(dtTotali.Rows[0]["IMPABBAZI"]);

                if (dtTotali.Rows[0]["SANSOTSOG"].ToString().Trim() == "S")
                    IMPTOT += 0;
                else if (DATSANANN.ToString() == "")
                    IMPTOT += Convert.ToDecimal(dtTotali.Rows[0]["IMPSANDET"]);
                else
                    IMPTOT += 0;

                if (DATSANANN == "")
                    IMPSANTOT = Convert.ToDecimal(dtTotali.Rows[0]["IMPSANDET"]).ToString("#,##0.#0");
                else
                    IMPSANTOT = "0,00";

                Cell = new Cell(new Paragraph(
                    new Phrase(Convert.ToDecimal(dtTotali.Rows[0]["IMPCON"]).ToString("#,##0.#0"), fontFooterBox)));
                Cell.Add(new Paragraph(new Phrase(Convert.ToDecimal(dtTotali.Rows[0]["IMPADDREC"]).ToString("#,##0.#0"),
                    fontFooterBox)));
                Cell.Add(new Paragraph(new Phrase(
                    Convert.ToDecimal(dtTotali.Rows[0]["IMPASSCONAZI"]).ToString("#,##0.#0"), fontFooterBox)));
                Cell.Add(new Paragraph(new Phrase(Convert.ToDecimal(dtTotali.Rows[0]["IMPABBAZI"]).ToString("#,##0.#0"),
                    fontFooterBox)));
                if (dtTotali.Rows[0]["SANSOTSOG"].ToString().Trim() == "S")
                    Cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
                else
                    Cell.Add(new Paragraph(new Phrase(IMPSANTOT, fontFooterBox)));

                Cell.Add(new Paragraph(new Phrase(IMPTOT.ToString("#,##0.#0"), fontFooterB)));
            }
            else
            {
                Cell = new Cell(new Paragraph(new Phrase("0,00", fontFooterBox)));
                Cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
                Cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
                Cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
                Cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
                Cell.Add(new Paragraph(new Phrase("0,00", fontFooterBox)));
            }
            Cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            Cell.BorderWidth = 0;
            tabTotali.AddCell(Cell, 0, 2);
        }
        tabTotali.Offset = 140;
        tabFooter.Offset = 20;
        tabFooter2.Offset = 20;

        document.Add(tabTotali);
        document.Add(tabFooter);
        document.Add(tabFooter2);
    }
    
    public void WritePieDiPagina(PdfWriter writer, Document document, int page, int totalPage)
    {
        var pdfContentByte = writer.DirectContent;
        pdfContentByte.BeginText();
        pdfContentByte.SetFontAndSize(bfVerdana, 6.5F);
        pdfContentByte.ShowTextAligned(0,
            $"Pag. {page} di {totalPage}", document.PageSize.Width - 50, 20, 0);
        pdfContentByte.EndText();
    }
    
    private Table CreaIntestazioneTabella()
    {
        var testataTabella = new Table(17, 2);
        testataTabella.SetWidths(new int[] {5, 22, 6, 4, 4, 8, 5, 5, 2, 7, 2, 7, 2, 7, 6, 4, 4});
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
        testataTabella.AddCell(cell, 0, 0);

        cell = new Cell(new Phrase("Cognome e Nome", fontRigaTest));
        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.HorizontalAlignment = Element.ALIGN_LEFT;
        cell.BorderWidth = 0;
        testataTabella.AddCell(cell, 0, 1);


        cell = new Cell(new Phrase("Qualifica", fontRigaTest));
        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.BorderWidth = 0;
        testataTabella.AddCell(cell, 0, 2);


        cell = new Cell(new Phrase("Data Variazione", fontRigaTest));
        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.BorderWidth = 0;
        cell.Colspan = 2;
        testataTabella.AddCell(cell, 0, 3);


        cell = new Cell(new Phrase("Livello", fontRigaTest));
        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.BorderWidth = 0;
        testataTabella.AddCell(cell, 0, 5);


        cell = new Cell(new Phrase("Cod. Contratto", fontRigaTest));
        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.BorderWidth = 0;
        testataTabella.AddCell(cell, 0, 6);


        cell = new Cell(new Phrase("Prest. %", fontRigaTest));
        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.BorderWidth = 0;
        testataTabella.AddCell(cell, 0, 7);


        cell = new Cell(new Phrase("Retribuzione Imponibile", fontRigaTest));
        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        cell.BorderWidth = 0;
        cell.Colspan = 2;
        testataTabella.AddCell(cell, 0, 8);


        cell = new Cell(new Phrase("di cui Occasionali", fontRigaTest));
        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        cell.BorderWidth = 0;
        cell.Colspan = 2;
        testataTabella.AddCell(cell, 0, 10);


        cell = new Cell(new Phrase("Figurativa", fontRigaTest));
        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        cell.BorderWidth = 0;
        cell.Colspan = 2;
        testataTabella.AddCell(cell, 0, 12);


        cell = new Cell(new Phrase("Aliquota", fontRigaTest));
        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.BorderWidth = 0;
        testataTabella.AddCell(cell, 0, 14);


        cell = new Cell(new Phrase("Abb. P.A.", fontRigaTest));
        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.BorderWidth = 0;
        testataTabella.AddCell(cell, 0, 15);


        cell = new Cell(new Phrase("Ass. Contr.", fontRigaTest));
        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.BorderWidth = 0;
        testataTabella.AddCell(cell, 0, 16);


        cell = new Cell("");
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        testataTabella.AddCell(cell, 1, 0);


        cell = new Cell("");
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        testataTabella.AddCell(cell, 1, 1);


        cell = new Cell("");
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        testataTabella.AddCell(cell, 1, 2);


        cell = new Cell(new Phrase("(Dal)", fontRigaTest));
        cell.VerticalAlignment = Element.ALIGN_TOP;
        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        testataTabella.AddCell(cell, 1, 3);


        cell = new Cell(new Phrase("(Al)", fontRigaTest));
        cell.VerticalAlignment = Element.ALIGN_TOP;
        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        testataTabella.AddCell(cell, 1, 4);


        cell = new Cell("");
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        testataTabella.AddCell(cell, 1, 5);


        cell = new Cell("");
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        testataTabella.AddCell(cell, 1, 6);


        cell = new Cell("");
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        testataTabella.AddCell(cell, 1, 7);


        cell = new Cell("");
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        testataTabella.AddCell(cell, 1, 8);


        cell = new Cell("");
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        testataTabella.AddCell(cell, 1, 9);


        cell = new Cell("");
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        testataTabella.AddCell(cell, 1, 10);


        cell = new Cell("");
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        testataTabella.AddCell(cell, 1, 11);


        cell = new Cell("");
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        testataTabella.AddCell(cell, 1, 12);


        cell = new Cell("");
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        testataTabella.AddCell(cell, 1, 13);


        cell = new Cell("");
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        testataTabella.AddCell(cell, 1, 14);


        cell = new Cell("");
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        testataTabella.AddCell(cell, 1, 15);


        cell = new Cell("");
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        testataTabella.AddCell(cell, 1, 16);
        return testataTabella;
    }
    
    private void WriteRecordDipa(Table recordTable, RetribuzioneRDL record)
    {
        var fontRigaDettaglio = new Font(bfVerdana, 7.75F, Font.NORMAL);

        var cell = new Cell(new Phrase(record.Mat.ToString().Trim(), fontRigaDettaglio));

        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        cell.HorizontalAlignment = Element.ALIGN_LEFT;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        recordTable.AddCell(cell, 0, 0);

        cell = new Cell(new Phrase(
            record.Nome.Trim().Substring(0, record.Nome.Trim().Length < 22 ? record.Nome.Trim().Length : 22),
            fontRigaDettaglio));
        cell.HorizontalAlignment = Element.ALIGN_LEFT;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        recordTable.AddCell(cell, 0, 1);

        if (record.CodQuaCon == 1)
        {
            cell = new Cell(new Phrase("D", fontRigaDettaglio));
        }
        else
        {
            cell = new Cell(new Phrase("I", fontRigaDettaglio));
        }

        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        recordTable.AddCell(cell, 0, 2);

        cell = new Cell(new Phrase(record.Dal.Substring(0, 5), fontRigaDettaglio));
        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        recordTable.AddCell(cell, 0, 3);

        cell = new Cell(new Phrase(record.Al.Substring(0, 5), fontRigaDettaglio));
        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        recordTable.AddCell(cell, 0, 4);

        cell = new Cell(new Phrase(record.Livello.Trim(), fontRigaDettaglio));
        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        recordTable.AddCell(cell, 0, 5);

        cell = new Cell(new Phrase(record.CodCon.ToString().Trim(), fontRigaDettaglio));
        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        recordTable.AddCell(cell, 0, 6);

        cell = new Cell(new Phrase(record.PerPar.ToString().Trim(), fontRigaDettaglio));
        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        recordTable.AddCell(cell, 0, 7);

        cell = new Cell(new Phrase("€", fontRigaDettaglio));
        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        recordTable.AddCell(cell, 0, 8);

        cell = new Cell(new Phrase(record.ImpRet.ToString(), fontRigaDettaglio));
        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        recordTable.AddCell(cell, 0, 9);

        cell = new Cell(new Phrase("€", fontRigaDettaglio));
        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        recordTable.AddCell(cell, 0, 10);

        cell = new Cell(new Phrase(record.ImpOcc.ToString(), fontRigaDettaglio));
        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;

        recordTable.AddCell(cell, 0, 11);

        cell = new Cell(new Phrase("€", fontRigaDettaglio));
        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        recordTable.AddCell(cell, 0, 12);

        cell = new Cell(new Phrase(record.ImpFig.ToString(), fontRigaDettaglio));
        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        recordTable.AddCell(cell, 0, 13);

        cell = new Cell(new Phrase(record.Aliquota.ToString(), fontRigaDettaglio));
        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        recordTable.AddCell(cell, 0, 14);

        if (record.ImpAbb.ToString() != "0")
        {
            cell = new Cell(new Phrase("S", fontRigaDettaglio));
        }
        else
        {
            cell = new Cell(new Phrase("N", fontRigaDettaglio));
        }

        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        recordTable.AddCell(cell, 0, 15);

        if (record.ImpAssCon.ToString() != "0")
        {
            cell = new Cell(new Phrase("S", fontRigaDettaglio));
        }
        else
        {
            cell = new Cell(new Phrase("N", fontRigaDettaglio));
        }

        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.BorderWidth = 0;
        cell.BorderWidthBottom = 0.5F;
        recordTable.AddCell(cell, 0, 16);
    }

    public byte[] CreaPdfRicevutaArretrato(DettaglioArretrato dettaglioArretrato, string nominativo)
    {
        string fileNameExisting = PathFileHelper.TemplateRicevutaArretrato;
        using var ms = new MemoryStream();
        using (var existingFileStream = new FileStream(fileNameExisting, FileMode.Open))
        {
            var pdfReader = new PdfReader(existingFileStream);
            var stamper = new PdfStamper(pdfReader, ms);
            var form = stamper.AcroFields;

            SetPdfAcrofields();

            stamper.FormFlattening = true;

            stamper.Close();
            pdfReader.Close();
            return ms.GetBuffer();

            void SetPdfAcrofields()
            {
                form.SetField("protnum", $"Num. {dettaglioArretrato.Protocollo.ProtocolloCompleto.Split(';')[1]}");
                form.SetField("protdat", $"Data {dettaglioArretrato.Protocollo.DataProtocollo.StandardizeDateString(StandardUse.Readable)}");
                form.SetField("PR", "Protocollo ENPAIA");
                form.SetField("PERIODO", $"{dettaglioArretrato.MeseDenuncia} {dettaglioArretrato.AnnoDenuncia}");
                form.SetField("DATA", dettaglioArretrato.DataChiusura.Value.Date.StandardizeDateString(StandardUse.Readable));
                form.SetField("ORA", dettaglioArretrato.DataChiusura.Value.StandardizeTime());
                form.SetField("NOMINATIVO", nominativo);
                form.SetField("NUMPROG", dettaglioArretrato.Protocollo.NumeroProgressivoProtoccollo);
                form.SetField("STATO", dettaglioArretrato.StatoDenuncia);
                form.SetField("TOTDOV", $"€ {dettaglioArretrato.TotaleDovuto}");
                form.SetField("TOTPAG", $"€ {dettaglioArretrato.TotaleDaPagare}");
            }
        }
    }
}