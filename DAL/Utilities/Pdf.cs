// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Utilities.Pdf
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using TFI.DAL.ConnectorDB;
using TFI.OCM.Amministrativo;

namespace TFI.DAL.Utilities
{
  public class Pdf
  {
    public static string BasePath;
    private clsSediinNet2010.clsSediinNet2010 objNet = new clsSediinNet2010.clsSediinNet2010();

    public static bool CreaStampaRicevutaDipaProtocollo(
      int CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      string strProtocollo,
      string strPath)
    {
      DataLayer dataLayer = new DataLayer();
      DataTable dataTable1 = new DataTable();
      try
      {
        string strSQL1 = "SELECT DATCONMOV FROM DENTES" + " WHERE ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND CODPOS = " + CODPOS.ToString();
        string str1 = dataLayer.Get1ValueFromSQL(strSQL1, CommandType.Text) ?? "";
        string strSQL2 = " SELECT AZI.RAGSOC" + " FROM AZI LEFT JOIN INDSED ON AZI.CODPOS = INDSED.CODPOS" + " WHERE INDSED.TIPIND=1 " + " AND INDSED.DATINI <= " + Pdf.Mod_DB2Date(Convert.ToDateTime(str1)) + " AND AZI.CODPOS= " + CODPOS.ToString() + " AND INDSED.DATCOM = (SELECT MAX(DATCOM) FROM INDSED WHERE " + " INDSED.CODPOS = " + CODPOS.ToString() + " AND TIPIND = 1 AND INDSED.DATINI <= " + Pdf.Mod_DB2Date(Convert.ToDateTime(str1)) + ")" + " ORDER BY AZI.CODPOS, INDSED.DATCOM DESC FETCH FIRST 1 ROWS ONLY";
        string str2 = dataLayer.Get1ValueFromSQL(strSQL2, CommandType.Text) ?? "";
        string strSQL3 = "SELECT DATSANANN, SANSOTSOG, NUMRIC, DATCHI, TIPMOV, IMPDIS, IMPDEC, IMPSANDET, CODMODPAG, " + " (SELECT MESE FROM MESI WHERE CODMES = DENTES.MESDEN) AS DENMESE" + " FROM DENTES" + " WHERE ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND CODPOS = " + CODPOS.ToString();
        DataTable dataTable2 = dataLayer.GetDataTable(strSQL3);
        string str3 = dataTable2.Rows[0]["DENMESE"].ToString().Trim() + " " + ANNDEN.ToString();
        string str4 = dataTable2.Rows[0]["IMPDEC"].ToString().Trim();
        string[] strArray1 = dataTable2.Rows[0]["DATCHI"].ToString().Split(' ');
        string str5 = strArray1[0];
        string str6 = strArray1[1];
        string[] strArray2 = strProtocollo.Split(';');
        string str7 = strArray2[strArray2.Length - 1].Split(' ')[0];
        string path = strPath;
        PdfReader reader = new PdfReader(!(dataTable2.Rows[0]["TIPMOV"].ToString().Trim() == "AR") ? AppContext.BaseDirectory + "StampePDF\\Moduli\\TEMPLATE_RICEVUTA_DIPA.pdf" : AppContext.BaseDirectory + "StampePDF\\Moduli\\TEMPLATE_RICEVUTA_ARRETRATI.pdf");
        PdfStamper pdfStamper = new PdfStamper(reader, (Stream) new FileStream(path, FileMode.Create));
        AcroFields acroFields1 = pdfStamper.AcroFields;
        acroFields1.SetField("protocollo", strProtocollo);
        acroFields1.SetField("PR", "Protocollo ENPAIA");
        acroFields1.SetField("protnum", "Num. " + strArray2[1]);
        acroFields1.SetField("protdat", "Data " + str7);
        acroFields1.SetField("PERIODO", str3);
        acroFields1.SetField("DATA", str5);
        acroFields1.SetField("ORA", str6);
        acroFields1.SetField("NOMINATIVO", CODPOS.ToString() + " - " + str2);
        acroFields1.SetField("NUMPROG", dataTable2.Rows[0]["NUMRIC"].ToString().Trim());
        if (dataTable2.Rows[0]["CODMODPAG"].ToString() == "")
          acroFields1.SetField("STATO", "Acquisito senza dichiarazione di pagamento");
        else
          acroFields1.SetField("STATO", "Acquisito con dichiarazione di pagamento");
        string str8 = Convert.ToDecimal(dataTable2.Rows[0]["IMPDIS"]).ToString();
        string str9;
        Decimal num;
        if (dataTable2.Rows[0]["SANSOTSOG"].ToString().Trim() == "S")
          str9 = (Convert.ToDecimal(str8) - Convert.ToDecimal(str4)).ToString();
        else if (dataTable2.Rows[0]["DATSANANN"].ToString().Trim() != "")
        {
          str9 = (Convert.ToDecimal(str8) - Convert.ToDecimal(str4)).ToString();
        }
        else
        {
          num = Convert.ToDecimal(str8) + Convert.ToDecimal(dataTable2.Rows[0]["IMPSANDET"]) - Convert.ToDecimal(str4);
          str9 = num.ToString();
        }
        AcroFields acroFields2 = acroFields1;
        num = Convert.ToDecimal(str8);
        string str10 = "€ " + num.ToString("#,##0.#0");
        acroFields2.SetField("TOTDOV", str10);
        AcroFields acroFields3 = acroFields1;
        num = Convert.ToDecimal(str9);
        string str11 = "€ " + num.ToString("#,##0.#0");
        acroFields3.SetField("TOTPAG", str11);
        pdfStamper.FormFlattening = false;
        pdfStamper.Close();
        reader.Close();
        return true;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public void MODULE_STAMPA_DOCUMENTO_DENUNCIA(
      int CODPOS,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      DataTable DTSTAMPE = null,
      bool DOCUMENTO_ORIGINALE = false,
      string strAZIONE = "",
      string strPathAzione = "")
    {
      DataTable dataTable1 = new DataTable();
      string str1 = "";
      DataLayer dataLayer = new DataLayer();
      try
      {
        string str2 = this.PROT_PRES_GET_TEMP_PATH_APPLICATION() + "\\";
        string str3 = !(Application.StartupPath.ToString().Substring(Application.StartupPath.ToString().Length - 1) == "\\") ? Application.StartupPath + "\\" : Application.StartupPath;
        if (DTSTAMPE != null)
        {
          if (DTSTAMPE.Columns.Count == 4)
          {
            string strPath = str2 + "TEMP_ENPAIANET_NU_" + DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(".", "_").Replace(":", "") + ".pdf";
            if (strAZIONE == "GENERA")
              this.CreaStampaNotifica(ref DTSTAMPE, strPathAzione, "GENERA");
            else
              this.CreaStampaNotifica(ref DTSTAMPE, strPath);
          }
          else if (DTSTAMPE.Rows.Count > 1)
          {
            for (int index = 0; index <= DTSTAMPE.Rows.Count - 1; ++index)
            {
              if (str1.Trim() != DTSTAMPE.Rows[index]["TIPMOV"].ToString().Trim())
              {
                string str4 = DTSTAMPE.Rows[index]["TIPMOV"].ToString().Trim();
                if (!(str4 == "DP"))
                {
                  if (!(str4 == "NU"))
                  {
                    if (str4 == "AR")
                    {
                      string strPath = str2 + "TEMP_ENPAIANET_AR_" + DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(".", "_").Replace(":", "") + ".pdf";
                      string strSQL = "SELECT VALUE(CHAR(NUMMOVANN),'') AS NUMMOVANN FROM DENTES " + " WHERE CODPOS = " + DTSTAMPE.Rows[index][nameof (CODPOS)]?.ToString() + " AND ANNDEN = " + DTSTAMPE.Rows[index][nameof (ANNDEN)]?.ToString() + " AND MESDEN = " + DTSTAMPE.Rows[index][nameof (MESDEN)]?.ToString() + " AND PRODEN = " + DTSTAMPE.Rows[index][nameof (PRODEN)]?.ToString() + " AND TIPMOV='AR'";
                      if (dataLayer.GetDataTable(strSQL).Rows[0]["NUMMOVANN"].ToString() == "")
                        this.CreaStampaArretrati(ref DTSTAMPE, strPath);
                      else
                        this.CreaStampaArretratoAnnullato(ref DTSTAMPE, strPath);
                    }
                    else
                    {
                      this.objNet.MsgBoxExm("Nessuna stampa disponibile per questo movimento");
                      break;
                    }
                  }
                  else
                  {
                    string strPath = str2 + "TEMP_ENPAIANET_NU_" + DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(".", "_").Replace(":", "") + ".pdf";
                    this.CreaStampaNotifica(ref DTSTAMPE, strPath);
                  }
                }
                else
                {
                  string strPath = str2 + "TEMP_ENPAIANET_DP_" + DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(".", "_").Replace(":", "") + ".pdf";
                  string strSQL = "SELECT VALUE(CHAR(NUMMOVANN),'') AS NUMMOVANN FROM DENTES " + " WHERE CODPOS = " + DTSTAMPE.Rows[index][nameof (CODPOS)]?.ToString() + " AND ANNDEN = " + DTSTAMPE.Rows[index][nameof (ANNDEN)]?.ToString() + " AND MESDEN = " + DTSTAMPE.Rows[index][nameof (MESDEN)]?.ToString() + " AND PRODEN = " + DTSTAMPE.Rows[index][nameof (PRODEN)]?.ToString() + " AND TIPMOV='DP'";
                  if (dataLayer.GetDataTable(strSQL).Rows[0]["NUMMOVANN"].ToString() == "")
                    this.CreaStampaDipa(ref DTSTAMPE, strPath);
                  else
                    this.CreaStampaDipaAnnullato(ref DTSTAMPE, strPath);
                }
              }
              str1 = DTSTAMPE.Rows[index]["TIPMOV"].ToString().Trim();
            }
          }
          else
          {
            string str5 = DTSTAMPE.Rows[0]["TIPMOV"].ToString().Trim();
            if (!(str5 == "DP"))
            {
              if (!(str5 == "NU"))
              {
                if (str5 == "AR")
                {
                  string strPath = str2 + "TEMP_ENPAIANET_AR_" + DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(".", "_").Replace(":", "") + ".pdf";
                  string strSQL = "SELECT VALUE(CHAR(NUMMOVANN),'') AS NUMMOVANN FROM DENTES " + " WHERE CODPOS = " + DTSTAMPE.Rows[0][nameof (CODPOS)]?.ToString() + " AND ANNDEN = " + DTSTAMPE.Rows[0][nameof (ANNDEN)]?.ToString() + " AND MESDEN = " + DTSTAMPE.Rows[0][nameof (MESDEN)]?.ToString() + " AND PRODEN = " + DTSTAMPE.Rows[0][nameof (PRODEN)]?.ToString() + " AND TIPMOV='AR'";
                  if (dataLayer.GetDataTable(strSQL).Rows[0]["NUMMOVANN"].ToString() == "")
                    this.CreaStampaArretrati(ref DTSTAMPE, strPath);
                  else
                    this.CreaStampaArretratoAnnullato(ref DTSTAMPE, strPath);
                }
                else
                  this.objNet.MsgBoxExm("Nessuna stampa disponibile per questo movimento");
              }
              else
              {
                string strPath = str2 + "TEMP_ENPAIANET_NU_" + DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(".", "_").Replace(":", "") + ".pdf";
                this.CreaStampaNotifica(ref DTSTAMPE, strPath);
              }
            }
            else
            {
              string strPath = str2 + "TEMP_ENPAIANET_DP_" + DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(".", "_").Replace(":", "") + ".pdf";
              string strSQL = "SELECT VALUE(CHAR(NUMMOVANN),'') AS NUMMOVANN FROM DENTES " + " WHERE CODPOS = " + DTSTAMPE.Rows[0][nameof (CODPOS)]?.ToString() + " AND MESDEN = " + DTSTAMPE.Rows[0][nameof (MESDEN)]?.ToString() + " AND PRODEN = " + DTSTAMPE.Rows[0][nameof (PRODEN)]?.ToString() + " AND ANNDEN = " + DTSTAMPE.Rows[0][nameof (ANNDEN)]?.ToString() + " AND TIPMOV='DP'";
              if (dataLayer.GetDataTable(strSQL).Rows[0]["NUMMOVANN"].ToString() == "")
                this.CreaStampaDipa(ref DTSTAMPE, strPath);
              else
                this.CreaStampaDipaAnnullato(ref DTSTAMPE, strPath);
            }
          }
        }
        else
        {
          string strSQL1 = "SELECT DATCHI, NUMMOV, NUMMOVANN, NUMSAN, NUMSANANN, TIPMOV, VALUE(ESIRET, '') AS ESIRET FROM DENTES " + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString();
          DataTable dataTable2 = dataLayer.GetDataTable(strSQL1);
          if (dataTable2.Rows[0]["ESIRET"].ToString().Trim() == "S" && !DOCUMENTO_ORIGINALE)
          {
            this.objNet.MsgBoxExm("Movimento rettificato. Impossibile stampare");
          }
          else
          {
            DTSTAMPE = new DataTable();
            DTSTAMPE.Columns.Add(new DataColumn()
            {
              ColumnName = nameof (CODPOS)
            });
            DTSTAMPE.Columns.Add(new DataColumn()
            {
              ColumnName = nameof (ANNDEN)
            });
            DTSTAMPE.Columns.Add(new DataColumn()
            {
              ColumnName = nameof (MESDEN)
            });
            DTSTAMPE.Columns.Add(new DataColumn()
            {
              ColumnName = nameof (PRODEN)
            });
            DTSTAMPE.Columns.Add(new DataColumn()
            {
              ColumnName = "TIPMOV"
            });
            DataRow row = DTSTAMPE.NewRow();
            row[nameof (CODPOS)] = (object) CODPOS;
            row[nameof (ANNDEN)] = (object) ANNDEN;
            row[nameof (MESDEN)] = (object) MESDEN;
            row[nameof (PRODEN)] = (object) PRODEN;
            row["TIPMOV"] = (object) dataTable2.Rows[0]["TIPMOV"].ToString().Trim();
            DTSTAMPE.Rows.Add(row);
            string str6 = dataTable2.Rows[0]["TIPMOV"].ToString().Trim();
            if (!(str6 == "DP"))
            {
              if (!(str6 == "NU"))
              {
                if (str6 == "AR")
                {
                  if (dataTable2.Rows[0]["NUMMOV"].ToString().Trim() == "")
                  {
                    this.objNet.MsgBoxExm("Arretrato non contabilizzato. Impossibile stampare");
                    return;
                  }
                  string strPath = str2 + "TEMP_ENPAIANET_AR_" + DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(".", "_").Replace(":", "") + ".pdf";
                  string strSQL2 = "SELECT VALUE(CHAR(NUMMOVANN),'') AS NUMMOVANN FROM DENTES " + " WHERE CODPOS = " + DTSTAMPE.Rows[0][nameof (CODPOS)]?.ToString() + " AND ANNDEN = " + DTSTAMPE.Rows[0][nameof (ANNDEN)]?.ToString() + " AND MESDEN = " + DTSTAMPE.Rows[0][nameof (MESDEN)]?.ToString() + " AND PRODEN = " + DTSTAMPE.Rows[0][nameof (PRODEN)]?.ToString() + " AND TIPMOV='AR'";
                  if (dataLayer.GetDataTable(strSQL2).Rows[0]["NUMMOVANN"].ToString() == "")
                    this.CreaStampaArretrati(ref DTSTAMPE, strPath);
                  else
                    this.CreaStampaArretratoAnnullato(ref DTSTAMPE, strPath);
                }
                else
                {
                  this.objNet.MsgBoxExm("Nessuna stampa disponibile per questo movimento");
                  return;
                }
              }
              else
              {
                if (dataTable2.Rows[0]["NUMMOV"].ToString().Trim() == "")
                {
                  this.objNet.MsgBoxExm("Notifica non contabilizzata. Impossibile stampare");
                  return;
                }
                string strPath = str2 + "TEMP_ENPAIANET_NU_" + DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(".", "_").Replace(":", "") + ".pdf";
                this.CreaStampaNotifica(ref DTSTAMPE, strPath);
              }
            }
            else
            {
              if (dataTable2.Rows[0]["DATCHI"].ToString().Trim() == "")
              {
                this.objNet.MsgBoxExm("Dipa in fase di compilazione. Impossibile stampare");
                return;
              }
              string strPath = str2 + "TEMP_ENPAIANET_DP_" + DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(".", "_").Replace(":", "") + ".pdf";
              string strSQL3 = "SELECT VALUE(CHAR(NUMMOVANN),'') AS NUMMOVANN FROM DENTES " + " WHERE CODPOS = " + DTSTAMPE.Rows[0][nameof (CODPOS)]?.ToString() + " AND ANNDEN = " + DTSTAMPE.Rows[0][nameof (ANNDEN)]?.ToString() + " AND MESDEN = " + DTSTAMPE.Rows[0][nameof (MESDEN)]?.ToString() + " AND PRODEN = " + DTSTAMPE.Rows[0][nameof (PRODEN)]?.ToString() + " AND TIPMOV='DP'";
              if (dataLayer.GetDataTable(strSQL3).Rows[0]["NUMMOVANN"].ToString() == "")
                this.CreaStampaDipa(ref DTSTAMPE, strPath);
              else
                this.CreaStampaDipaAnnullato(ref DTSTAMPE, strPath);
            }
            string strPath1 = str2 + "TEMP_ENPAIANET_SAN_" + DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(".", "_").Replace(":", "") + ".pdf";
            if (dataTable2.Rows[0]["NUMSANANN"].ToString().Trim() != "" && dataTable2.Rows[0]["TIPMOV"].ToString().Trim() == "NU")
              this.CreaStampaSanzioneAnnullataNU(ref DTSTAMPE, strPath1);
            if (!(dataTable2.Rows[0]["NUMSAN"].ToString().Trim() != "") || !(dataTable2.Rows[0]["TIPMOV"].ToString().Trim() != "NU") || this.objNet.MsgBoxYesNo("Si desidera effettuare la stampa della sanzione per questa denuncia?") != DialogResult.Yes)
              return;
            string str7 = dataTable2.Rows[0]["TIPMOV"].ToString().Trim();
            if (!(str7 == "AR"))
            {
              if (!(str7 == "DP"))
                return;
              this.CreaStampaSanzioneDipa(ref DTSTAMPE, strPath1);
            }
            else
              this.CreaStampaSanzioneArretrati(ref DTSTAMPE, strPath1);
          }
        }
      }
      catch (Exception ex)
      {
        StackTrace stackTrace = new StackTrace(ex);
        stackTrace.GetFrame(stackTrace.FrameCount - 1).ToString();
        this.Module_WriteError(nameof (MODULE_STAMPA_DOCUMENTO_DENUNCIA), stackTrace.GetFrame(stackTrace.FrameCount - 1).ToString(), ex.Message);
      }
    }

    public string PROT_PRES_GET_TEMP_PATH_APPLICATION()
    {
      string str = Path.GetTempPath();
      FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
      string companyName = versionInfo.CompanyName;
      string productName = versionInfo.ProductName;
      string productVersion = versionInfo.ProductVersion;
      if (str.Substring(str.Length - 1) == "\\")
        str = str.Substring(0, str.Length - 1);
      string path = str + "\\" + productName + "\\tmp";
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
      return path;
    }

    public void CreaStampaNotifica(ref DataTable dtNotifica, string strPath, string strAzione = "")
    {
      Document document = new Document(PageSize.A4.Rotate(), 10f, 20f, 5f, 5f);
      PdfWriter instance = PdfWriter.GetInstance(document, (Stream) new FileStream(strPath, FileMode.Create));
      int num1 = 1;
      int num2 = 0;
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataLayer dl = new DataLayer();
      int cntNumPag = 1;
      string str1 = this.EstraiFilePGM_File("VERDANAB");
      string str2 = this.EstraiFilePGM_File("VERDANA");
      document.Open();
      for (int index1 = 0; index1 <= dtNotifica.Rows.Count - 1; ++index1)
      {
        PdfContentByte directContent = instance.DirectContent;
        string strSQL1 = " SELECT DENTES.SANSOTSOG, DENTES.IMPADDREC, DENTES.DATSANANN, DENTES.DATSAN, DENTES.IMPABB, DENTES.IMPASSCON, " + " DENTES.NUMSAN, DENTES.DATCONMOV, DENTES.NUMMOV, DENTES.CODCAUSAN, " + " DENTES.IMPCON AS IMPCONTOT, DENTES.IMPSANDET AS IMPSANTOT, " + " DENTES.DATMOVANN, DENTES.NUMMOVANN, DENTES.NUMSANANN " + " FROM DENTES " + " WHERE DENTES.CODPOS = " + dtNotifica.Rows[index1]["CODPOS"]?.ToString() + "  AND DENTES.ANNDEN = " + dtNotifica.Rows[index1]["ANNDEN"]?.ToString() + "  AND DENTES.MESDEN = " + dtNotifica.Rows[index1]["MESDEN"]?.ToString() + "  AND DENTES.PRODEN = " + dtNotifica.Rows[index1]["PRODEN"]?.ToString();
        DataTable dataTable3 = dl.GetDataTable(strSQL1);
        BaseFont.CreateFont(str1, "Cp1252", false);
        if (index1 > 0)
        {
          cntNumPag = 1;
          num2 = 0;
          num1 = 1;
          document.NewPage();
        }
        dataTable3.Rows[0]["SANSOTSOG"].ToString().Trim();
        string IMPSANTOT = !(dataTable3.Rows[0]["NUMSAN"].ToString() == "") ? (!((dataTable3.Rows[0]["NUMMOVANN"].ToString() ?? "") == "") ? dataTable3.Rows[0]["IMPSANTOT"].ToString() : (!(dataTable3.Rows[0]["NUMSANANN"].ToString() != "") ? dataTable3.Rows[0]["IMPSANTOT"].ToString() : "0,00")) : "0,00";
        this.ScriviIntestazioneNotifica(dl, ref directContent, ref document, Convert.ToInt32(dtNotifica.Rows[index1]["CODPOS"]), dataTable3.Rows[0]["DATCONMOV"]?.ToString() ?? "", str2, str1);
        this.ScriviPieDiPaginaNotifica(dl, ref directContent, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["NUMSANANN"].ToString(), str2);
        Cell Cell = new Cell();
        iTextSharp.text.Table tabTestata = new iTextSharp.text.Table(10, 1);
        tabTestata.WidthPercentage = 100f;
        tabTestata.AutoFillEmptyCells = true;
        tabTestata.BorderWidth = 0.0f;
        tabTestata.BorderColor = Color.WHITE;
        tabTestata.Cellpadding = 2f;
        int[] widths1 = new int[10]
        {
          8,
          20,
          5,
          1,
          7,
          11,
          1,
          7,
          1,
          7
        };
        tabTestata.SetWidths(widths1);
        Cell CellTotali = new Cell();
        iTextSharp.text.Table TabTotali = new iTextSharp.text.Table(4, 7);
        TabTotali.WidthPercentage = 45f;
        TabTotali.AutoFillEmptyCells = true;
        TabTotali.Alignment = 2;
        TabTotali.BorderWidth = 0.0f;
        TabTotali.BorderColor = Color.WHITE;
        int[] widths2 = new int[4]{ 30, 45, 1, 22 };
        TabTotali.SetWidths(widths2);
        iTextSharp.text.Font FontTestata = new iTextSharp.text.Font(BaseFont.CreateFont(str1, "Cp1252", false), 8.25f, 0);
        this.ScriviTestataNotifica(ref tabTestata, ref Cell, ref FontTestata);
        string strSQL2 = " SELECT DENDET.MAT, DENDET.NUMSAN, DENDET.PRODEN, DENDET.ALIQUOTA, " + " DENDET.ANNDEN,DENDET.MESDEN, SUM(DENDET.IMPCON) AS IMPCON, " + " SUM(DENDET.IMPSANDET) AS IMPSAN, SUM(DENDET.IMPRET) AS IMPRET," + " DENDET.CODPOS, " + " ISCT.COG, ISCT.NOM " + " FROM DENDET " + " INNER JOIN ISCT ON " + " ISCT.MAT = DENDET.MAT " + " WHERE DENDET.CODPOS = " + dtNotifica.Rows[index1]["CODPOS"]?.ToString() + "  AND DENDET.ANNDEN = " + dtNotifica.Rows[index1]["ANNDEN"]?.ToString() + "  AND DENDET.MESDEN = " + dtNotifica.Rows[index1]["MESDEN"]?.ToString() + "  AND DENDET.PRODEN = " + dtNotifica.Rows[index1]["PRODEN"]?.ToString() + "  AND DENDET.TIPMOV = 'NU'" + " GROUP BY DENDET.MAT, DENDET.CODPOS, DENDET.PRODEN, DENDET.MESDEN, DENDET.NUMSAN," + " DENDET.ANNDEN, DENDET.ALIQUOTA, " + " ISCT.COG, ISCT.NOM ORDER BY ISCT.COG";
        DataTable dataTable4 = dl.GetDataTable(strSQL2);
        BaseFont font = BaseFont.CreateFont(str2, "Cp1252", false);
        iTextSharp.text.Font FontDettaglio = new iTextSharp.text.Font(font, 8.25f, 0);
        iTextSharp.text.Font FontNascondi = new iTextSharp.text.Font(font, 8.25f, 0, Color.WHITE);
        int TotPag = !((Decimal) (dataTable4.Rows.Count - dataTable4.Rows.Count / 18 * 18) <= 9M) ? dataTable4.Rows.Count / 18 + 2 : dataTable4.Rows.Count / 18 + 1;
        this.scriviNumPagNotifica(ref directContent, cntNumPag, TotPag, str2);
        for (int index2 = 0; index2 <= dataTable4.Rows.Count - 1; ++index2)
        {
          this.ScriviSanzioneNotifica(ref directContent, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["DATSANANN"].ToString(), dataTable3.Rows[0]["DATCONMOV"].ToString(), dataTable3.Rows[0]["NUMMOV"].ToString(), dataTable3.Rows[0]["DATSAN"].ToString(), dataTable3.Rows[0]["NUMSAN"].ToString(), dataTable4.Rows[index2]["MESDEN"].ToString(), dataTable4.Rows[index2]["ANNDEN"].ToString(), Convert.ToInt32(dataTable4.Rows[index2]["CODPOS"]), dataTable3.Rows[0]["DATMOVANN"].ToString(), dataTable3.Rows[0]["NUMMOVANN"].ToString(), dataTable3.Rows[0]["NUMSANANN"].ToString(), str2);
          if (num1 == 19)
          {
            tabTestata.Offset = 108f;
            document.Add((IElement) tabTestata);
            tabTestata.DeleteAllRows();
            document.NewPage();
            this.ScriviIntestazioneNotifica(dl, ref directContent, ref document, Convert.ToInt32(dtNotifica.Rows[index1]["CODPOS"]), dataTable3.Rows[0]["DATCONMOV"]?.ToString() ?? "", str2, str1);
            this.ScriviPieDiPaginaNotifica(dl, ref directContent, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["NUMSANANN"].ToString(), str2);
            this.ScriviTestataNotifica(ref tabTestata, ref Cell, ref FontTestata);
            ++cntNumPag;
            num2 = index2;
            num1 = 1;
            this.scriviNumPagNotifica(ref directContent, cntNumPag, TotPag, str2);
          }
          string str3 = dataTable4.Rows[index2]["COG"].ToString().Trim() + " " + dataTable4.Rows[index2]["NOM"].ToString().Trim();
          string str4 = dataTable4.Rows[index2]["MESDEN"].ToString().Trim() + "/" + dataTable4.Rows[index2]["ANNDEN"].ToString().Trim();
          Decimal num3 = Convert.ToDecimal(dataTable4.Rows[index2]["IMPRET"]);
          string str5 = num3.ToString("#,##0.#0");
          num3 = Convert.ToDecimal(dataTable4.Rows[index2]["IMPCON"]);
          string str6 = num3.ToString("#,##0.#0");
          string str7;
          if (dataTable4.Rows[index2]["NUMSAN"].ToString() == "")
          {
            str7 = "0,00";
          }
          else
          {
            num3 = Convert.ToDecimal(dataTable4.Rows[index2]["IMPSAN"]);
            str7 = num3.ToString("#,##0.#0");
          }
          string str8 = dataTable4.Rows[index2]["ALIQUOTA"].ToString().Trim() + " %";
          Cell aCell1 = new Cell((IElement) new Phrase(dataTable4.Rows[index2]["MAT"].ToString().Trim(), FontDettaglio));
          tabTestata.AddCell(aCell1, 1 + index2 - num2, 0);
          aCell1.HorizontalAlignment = 1;
          aCell1.VerticalAlignment = 5;
          aCell1.BorderWidth = 0.0f;
          aCell1.BorderWidthBottom = 0.5f;
          Cell aCell2 = new Cell((IElement) new Phrase(str3.Trim(), FontDettaglio));
          tabTestata.AddCell(aCell2, 1 + index2 - num2, 1);
          aCell2.HorizontalAlignment = 0;
          aCell2.VerticalAlignment = 5;
          aCell2.BorderWidth = 0.0f;
          aCell2.BorderWidthBottom = 0.5f;
          Cell aCell3 = new Cell((IElement) new Phrase(str4, FontDettaglio));
          tabTestata.AddCell(aCell3, 1 + index2 - num2, 2);
          aCell3.HorizontalAlignment = 1;
          aCell3.VerticalAlignment = 5;
          aCell3.BorderWidth = 0.0f;
          aCell3.BorderWidthBottom = 0.5f;
          Cell aCell4 = new Cell((IElement) new Phrase("€", FontDettaglio));
          tabTestata.AddCell(aCell4, 1 + index2 - num2, 3);
          aCell4.HorizontalAlignment = 2;
          aCell4.VerticalAlignment = 5;
          aCell4.BorderWidth = 0.0f;
          aCell4.BorderWidthBottom = 0.5f;
          Cell aCell5 = new Cell((IElement) new Phrase(str5, FontDettaglio));
          tabTestata.AddCell(aCell5, 1 + index2 - num2, 4);
          aCell5.HorizontalAlignment = 2;
          aCell5.VerticalAlignment = 5;
          aCell5.BorderWidth = 0.0f;
          aCell5.BorderWidthBottom = 0.5f;
          Cell aCell6 = new Cell((IElement) new Phrase(str8, FontDettaglio));
          tabTestata.AddCell(aCell6, 1 + index2 - num2, 5);
          aCell6.HorizontalAlignment = 1;
          aCell6.VerticalAlignment = 5;
          aCell6.BorderWidth = 0.0f;
          aCell6.BorderWidthBottom = 0.5f;
          Cell aCell7 = new Cell((IElement) new Phrase("€", FontDettaglio));
          tabTestata.AddCell(aCell7, 1 + index2 - num2, 6);
          aCell7.HorizontalAlignment = 2;
          aCell7.VerticalAlignment = 5;
          aCell7.BorderWidth = 0.0f;
          aCell7.BorderWidthBottom = 0.5f;
          Cell aCell8 = new Cell((IElement) new Phrase(str6, FontDettaglio));
          tabTestata.AddCell(aCell8, 1 + index2 - num2, 7);
          aCell8.HorizontalAlignment = 2;
          aCell8.VerticalAlignment = 5;
          aCell8.BorderWidth = 0.0f;
          aCell8.BorderWidthBottom = 0.5f;
          Cell aCell9 = new Cell((IElement) new Phrase("€", FontDettaglio));
          tabTestata.AddCell(aCell9, 1 + index2 - num2, 8);
          aCell9.HorizontalAlignment = 2;
          aCell9.VerticalAlignment = 5;
          aCell9.BorderWidth = 0.0f;
          aCell9.BorderWidthBottom = 0.5f;
          Cell = new Cell((IElement) new Phrase(str7, FontDettaglio));
          tabTestata.AddCell(Cell, 1 + index2 - num2, 9);
          Cell.HorizontalAlignment = 2;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          ++num1;
        }
        tabTestata.Offset = 108f;
        document.Add((IElement) tabTestata);
        if (num1 > 10)
        {
          document.NewPage();
          this.ScriviIntestazioneNotifica(dl, ref directContent, ref document, Convert.ToInt32(dtNotifica.Rows[index1]["CODPOS"]), dataTable3.Rows[0]["DATCONMOV"]?.ToString() ?? "", str2, str1);
          this.ScriviPieDiPaginaNotifica(dl, ref directContent, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["NUMSANANN"].ToString(), str2);
          this.scriviNumPagNotifica(ref directContent, cntNumPag + 1, TotPag, str2);
          this.ScriviTotaliNotifica(ref TabTotali, ref CellTotali, ref FontDettaglio, ref FontNascondi, ref FontTestata, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["NUMSANANN"].ToString(), dataTable3.Rows[0]["IMPADDREC"].ToString(), dataTable3.Rows[0]["IMPABB"].ToString(), dataTable3.Rows[0]["IMPASSCON"].ToString(), dataTable3.Rows[0]["IMPCONTOT"].ToString(), IMPSANTOT, dataTable3.Rows[0]["NUMSAN"].ToString(), ref directContent, dataTable3.Rows[0]["NUMMOVANN"]?.ToString() ?? "", str1);
          TabTotali.Offset = 108f;
          document.Add((IElement) TabTotali);
        }
        else
        {
          this.ScriviTotaliNotifica(ref TabTotali, ref CellTotali, ref FontDettaglio, ref FontNascondi, ref FontTestata, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["NUMSANANN"].ToString(), dataTable3.Rows[0]["IMPADDREC"].ToString(), dataTable3.Rows[0]["IMPABB"].ToString(), dataTable3.Rows[0]["IMPASSCON"].ToString(), dataTable3.Rows[0]["IMPCONTOT"].ToString(), IMPSANTOT, dataTable3.Rows[0]["NUMSAN"].ToString(), ref directContent, dataTable3.Rows[0]["NUMMOVANN"]?.ToString() ?? "", str1);
          TabTotali.Offset = 10f;
          document.Add((IElement) TabTotali);
        }
        Convert.ToDecimal(0.0);
      }
      document.Close();
      instance.Close();
      if (!(strAzione != "GENERA"))
        return;
      Process.Start(strPath);
    }

    public void CreaStampaDipaAnnullato(ref DataTable DTDIPA, string strPath)
    {
      Document document = new Document(PageSize.A4.Rotate(), 10f, 20f, 5f, 5f);
      PdfWriter instance = PdfWriter.GetInstance(document, (Stream) new FileStream(strPath, FileMode.Create));
      int num1 = 1;
      int num2 = 0;
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      int cntNumPag = 1;
      document.Open();
      for (int index1 = 0; index1 <= DTDIPA.Rows.Count - 1; ++index1)
      {
        if (DTDIPA.Rows[index1]["TIPMOV"].ToString().Trim() == "DP")
        {
          PdfContentByte directContent = instance.DirectContent;
          string strSQL1 = " SELECT DENTES.SANSOTSOG, DENTES.IMPADDREC, DENTES.DATSANANN, DENTES.DATSAN, DENTES.IMPABB, DENTES.IMPASSCON, " + " DENTES.NUMSAN, DENTES.DATCONMOV, DENTES.NUMMOV, DENTES.CODCAUSAN, " + " DENTES.IMPCON AS IMPCONTOT, DENTES.IMPSANDET AS IMPSANTOT, " + " DENTES.DATMOVANN, DENTES.NUMMOVANN, DENTES.NUMSANANN " + " FROM DENTES " + " WHERE DENTES.CODPOS = " + DTDIPA.Rows[index1]["CODPOS"]?.ToString() + "  AND DENTES.MESDEN = " + DTDIPA.Rows[index1]["MESDEN"]?.ToString() + "  AND DENTES.PRODEN = " + DTDIPA.Rows[index1]["PRODEN"]?.ToString() + "  AND DENTES.ANNDEN = " + DTDIPA.Rows[index1]["ANNDEN"]?.ToString() + "  AND DENTES.TIPMOV = 'DP' ";
          DataTable dataTable3 = dataLayer.GetDataTable(strSQL1);
          BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
          if (index1 > 0)
          {
            cntNumPag = 1;
            num2 = 0;
            num1 = 1;
            document.NewPage();
          }
          dataTable3.Rows[0]["SANSOTSOG"].ToString().Trim();
          string IMPSANTOT = !(dataTable3.Rows[0]["NUMSAN"].ToString() == "") ? (!((dataTable3.Rows[0]["NUMMOVANN"].ToString() ?? "") == "") ? Convert.ToString(dataTable3.Rows[0]["IMPSANTOT"]) : (!(dataTable3.Rows[0]["NUMSANANN"].ToString() != "") ? Convert.ToString(dataTable3.Rows[0]["IMPSANTOT"]) : "0,00")) : "0,00";
          this.ScriviIntestazioneDipaAnnullato(ref directContent, ref document, Convert.ToInt32(DTDIPA.Rows[index1]["CODPOS"]), dataTable3.Rows[0]["DATCONMOV"]?.ToString() ?? "");
          this.ScriviPieDiPaginaDipaAnnullato(ref directContent, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["NUMSANANN"].ToString());
          Cell Cell = new Cell();
          iTextSharp.text.Table tabTestata = new iTextSharp.text.Table(10, 1);
          tabTestata.WidthPercentage = 100f;
          tabTestata.AutoFillEmptyCells = true;
          tabTestata.BorderWidth = 0.0f;
          tabTestata.BorderColor = Color.WHITE;
          tabTestata.Cellpadding = 2f;
          int[] widths1 = new int[10]
          {
            8,
            36,
            12,
            3,
            8,
            11,
            3,
            8,
            3,
            8
          };
          tabTestata.SetWidths(widths1);
          Cell CellTotali = new Cell();
          iTextSharp.text.Table TabTotali = new iTextSharp.text.Table(4, 7);
          TabTotali.WidthPercentage = 45f;
          TabTotali.AutoFillEmptyCells = true;
          TabTotali.Alignment = 2;
          TabTotali.BorderWidth = 0.0f;
          TabTotali.BorderColor = Color.WHITE;
          int[] widths2 = new int[4]{ 30, 45, 1, 22 };
          TabTotali.SetWidths(widths2);
          iTextSharp.text.Font FontTestata = new iTextSharp.text.Font(BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false), 8.25f, 0);
          this.ScriviTestataDipaAnnullato(ref tabTestata, ref Cell, ref FontTestata);
          string strSQL2 = " SELECT DENDET.MAT, DENDET.NUMSAN, DENDET.PRODEN, DENDET.ALIQUOTA, " + " DENDET.ANNDEN,DENDET.MESDEN, SUM(DENDET.IMPCON) AS IMPCON, " + " SUM(DENDET.IMPSANDET) AS IMPSAN, SUM(DENDET.IMPRET) AS IMPRET," + " DENDET.CODPOS, " + " ISCT.COG, ISCT.NOM " + " FROM DENDET " + " INNER JOIN ISCT ON " + " ISCT.MAT = DENDET.MAT " + " WHERE DENDET.CODPOS = " + DTDIPA.Rows[index1]["CODPOS"]?.ToString() + " AND DENDET.ANNDEN = " + DTDIPA.Rows[index1]["ANNDEN"]?.ToString() + " AND DENDET.MESDEN = " + DTDIPA.Rows[index1]["MESDEN"]?.ToString() + " AND DENDET.PRODEN = " + DTDIPA.Rows[index1]["PRODEN"]?.ToString() + " AND DENDET.TIPMOV = 'DP'" + " GROUP BY DENDET.MAT, DENDET.CODPOS, DENDET.PRODEN, DENDET.MESDEN, DENDET.NUMSAN," + " DENDET.ANNDEN, DENDET.ALIQUOTA, " + " ISCT.COG, ISCT.NOM ORDER BY ISCT.COG";
          DataTable dataTable4 = dataLayer.GetDataTable(strSQL2);
          BaseFont font = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
          iTextSharp.text.Font FontDettaglio = new iTextSharp.text.Font(font, 8.25f, 0);
          iTextSharp.text.Font FontNascondi = new iTextSharp.text.Font(font, 8.25f, 0, Color.WHITE);
          int TotPag = !((Decimal) (dataTable4.Rows.Count - dataTable4.Rows.Count / 18 * 18) <= 9M) ? dataTable4.Rows.Count / 18 + 2 : dataTable4.Rows.Count / 18 + 1;
          this.scriviNumPagDipaAnnullato(ref directContent, cntNumPag, TotPag);
          for (int index2 = 0; index2 <= dataTable4.Rows.Count - 1; ++index2)
          {
            this.ScriviSanzioneDipaAnnullato(ref directContent, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["DATSANANN"].ToString(), dataTable3.Rows[0]["DATCONMOV"].ToString(), dataTable3.Rows[0]["NUMMOV"].ToString(), dataTable3.Rows[0]["DATSAN"].ToString(), dataTable3.Rows[0]["NUMSAN"].ToString(), dataTable4.Rows[index2]["MESDEN"].ToString(), dataTable4.Rows[index2]["ANNDEN"].ToString(), Convert.ToInt32(dataTable4.Rows[index2]["CODPOS"]), dataTable3.Rows[0]["DATMOVANN"].ToString(), dataTable3.Rows[0]["NUMMOVANN"].ToString(), dataTable3.Rows[0]["NUMSANANN"].ToString());
            if (num1 == 19)
            {
              tabTestata.Offset = 120f;
              document.Add((IElement) tabTestata);
              tabTestata.DeleteAllRows();
              document.NewPage();
              this.ScriviIntestazioneDipaAnnullato(ref directContent, ref document, Convert.ToInt32(DTDIPA.Rows[index1]["CODPOS"]), dataTable3.Rows[0]["DATCONMOV"]?.ToString() ?? "");
              this.ScriviPieDiPaginaDipaAnnullato(ref directContent, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["NUMSANANN"].ToString());
              this.ScriviTestataDipaAnnullato(ref tabTestata, ref Cell, ref FontTestata);
              ++cntNumPag;
              num2 = index2;
              num1 = 1;
              this.scriviNumPagDipaAnnullato(ref directContent, cntNumPag, TotPag);
            }
            string str1 = dataTable4.Rows[index2]["COG"].ToString().Trim() + " " + dataTable4.Rows[index2]["NOM"].ToString().Trim();
            string str2 = dataTable4.Rows[index2]["MESDEN"].ToString().Trim() + "/" + dataTable4.Rows[index2]["ANNDEN"].ToString().Trim();
            Decimal num3 = Convert.ToDecimal(dataTable4.Rows[index2]["IMPRET"]);
            string str3 = num3.ToString("#,##0.#0");
            num3 = Convert.ToDecimal(dataTable4.Rows[index2]["IMPCON"]);
            string str4 = num3.ToString("#,##0.#0");
            string str5;
            if (dataTable4.Rows[index2]["NUMSAN"].ToString() == "")
            {
              str5 = "0,00";
            }
            else
            {
              num3 = Convert.ToDecimal(dataTable4.Rows[index2]["IMPSAN"]);
              str5 = num3.ToString("#,##0.#0");
            }
            string str6 = dataTable4.Rows[index2]["ALIQUOTA"].ToString().Trim() + " %";
            Cell aCell1 = new Cell((IElement) new Phrase(dataTable4.Rows[index2]["MAT"].ToString().Trim(), FontDettaglio));
            tabTestata.AddCell(aCell1, 1 + index2 - num2, 0);
            aCell1.HorizontalAlignment = 1;
            aCell1.VerticalAlignment = 5;
            aCell1.BorderWidth = 0.0f;
            aCell1.BorderWidthBottom = 0.5f;
            Cell aCell2 = new Cell((IElement) new Phrase(str1, FontDettaglio));
            tabTestata.AddCell(aCell2, 1 + index2 - num2, 1);
            aCell2.HorizontalAlignment = 0;
            aCell2.VerticalAlignment = 5;
            aCell2.BorderWidth = 0.0f;
            aCell2.BorderWidthBottom = 0.5f;
            Cell aCell3 = new Cell((IElement) new Phrase(str2, FontDettaglio));
            tabTestata.AddCell(aCell3, 1 + index2 - num2, 2);
            aCell3.HorizontalAlignment = 1;
            aCell3.VerticalAlignment = 5;
            aCell3.BorderWidth = 0.0f;
            aCell3.BorderWidthBottom = 0.5f;
            Cell aCell4 = new Cell((IElement) new Phrase("€", FontDettaglio));
            tabTestata.AddCell(aCell4, 1 + index2 - num2, 3);
            aCell4.HorizontalAlignment = 2;
            aCell4.VerticalAlignment = 5;
            aCell4.BorderWidth = 0.0f;
            aCell4.BorderWidthBottom = 0.5f;
            Cell aCell5 = new Cell((IElement) new Phrase(str3, FontDettaglio));
            tabTestata.AddCell(aCell5, 1 + index2 - num2, 4);
            aCell5.HorizontalAlignment = 2;
            aCell5.VerticalAlignment = 5;
            aCell5.BorderWidth = 0.0f;
            aCell5.BorderWidthBottom = 0.5f;
            Cell aCell6 = new Cell((IElement) new Phrase(str6, FontDettaglio));
            tabTestata.AddCell(aCell6, 1 + index2 - num2, 5);
            aCell6.HorizontalAlignment = 1;
            aCell6.VerticalAlignment = 5;
            aCell6.BorderWidth = 0.0f;
            aCell6.BorderWidthBottom = 0.5f;
            Cell aCell7 = new Cell((IElement) new Phrase("€", FontDettaglio));
            tabTestata.AddCell(aCell7, 1 + index2 - num2, 6);
            aCell7.HorizontalAlignment = 2;
            aCell7.VerticalAlignment = 5;
            aCell7.BorderWidth = 0.0f;
            aCell7.BorderWidthBottom = 0.5f;
            Cell aCell8 = new Cell((IElement) new Phrase(str4, FontDettaglio));
            tabTestata.AddCell(aCell8, 1 + index2 - num2, 7);
            aCell8.HorizontalAlignment = 2;
            aCell8.VerticalAlignment = 5;
            aCell8.BorderWidth = 0.0f;
            aCell8.BorderWidthBottom = 0.5f;
            Cell aCell9 = new Cell((IElement) new Phrase("€", FontDettaglio));
            tabTestata.AddCell(aCell9, 1 + index2 - num2, 8);
            aCell9.HorizontalAlignment = 2;
            aCell9.VerticalAlignment = 5;
            aCell9.BorderWidth = 0.0f;
            aCell9.BorderWidthBottom = 0.5f;
            Cell = new Cell((IElement) new Phrase(str5, FontDettaglio));
            tabTestata.AddCell(Cell, 1 + index2 - num2, 9);
            Cell.HorizontalAlignment = 2;
            Cell.VerticalAlignment = 5;
            Cell.BorderWidth = 0.0f;
            Cell.BorderWidthBottom = 0.5f;
            ++num1;
          }
          tabTestata.Offset = 120f;
          document.Add((IElement) tabTestata);
          if (num1 > 10)
          {
            document.NewPage();
            this.ScriviIntestazioneDipaAnnullato(ref directContent, ref document, Convert.ToInt32(DTDIPA.Rows[index1]["CODPOS"]), dataTable3.Rows[0]["DATCONMOV"]?.ToString() ?? "");
            this.ScriviPieDiPaginaDipaAnnullato(ref directContent, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["NUMSANANN"].ToString());
            this.scriviNumPagDipaAnnullato(ref directContent, cntNumPag + 1, TotPag);
            this.ScriviTotaliDipaAnnullato(ref TabTotali, ref CellTotali, ref FontDettaglio, ref FontNascondi, ref FontTestata, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["NUMSANANN"].ToString(), dataTable3.Rows[0]["IMPADDREC"].ToString(), dataTable3.Rows[0]["IMPABB"].ToString(), dataTable3.Rows[0]["IMPASSCON"].ToString(), dataTable3.Rows[0]["IMPCONTOT"].ToString(), IMPSANTOT, dataTable3.Rows[0]["NUMSAN"].ToString(), ref directContent, dataTable3.Rows[0]["NUMMOVANN"]?.ToString() ?? "");
            TabTotali.Offset = 120f;
            document.Add((IElement) TabTotali);
          }
          else
          {
            this.ScriviTotaliDipaAnnullato(ref TabTotali, ref CellTotali, ref FontDettaglio, ref FontNascondi, ref FontTestata, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["NUMSANANN"].ToString(), dataTable3.Rows[0]["IMPADDREC"].ToString(), dataTable3.Rows[0]["IMPABB"].ToString(), dataTable3.Rows[0]["IMPASSCON"].ToString(), dataTable3.Rows[0]["IMPCONTOT"].ToString(), IMPSANTOT, dataTable3.Rows[0]["NUMSAN"].ToString(), ref directContent, dataTable3.Rows[0]["NUMMOVANN"]?.ToString() ?? "");
            TabTotali.Offset = 20f;
            document.Add((IElement) TabTotali);
          }
          Convert.ToDecimal(0.0);
        }
      }
      document.Close();
      instance.Close();
      Process.Start(strPath);
    }

    public string EstraiFilePGM_File(string CHIAVE, string EST = ".ttf")
    {
      int ordinal = 0;
      DataTable dataTable = new DataTable();
      DataTableReader dataReader = new DataLayer().GetDataTable("SELECT FILE FROM FILE_PGM WHERE CHIAVE = " + this.objNet.DoublePeakForSql(CHIAVE.ToUpper())).CreateDataReader();
      dataReader.Read();
      byte[] buffer = new byte[dataReader.GetBytes(ordinal, 0L, (byte[]) null, 0, int.MaxValue) - 1L + 1L];
      dataReader.GetBytes(ordinal, 0L, buffer, 0, buffer.Length);
      dataReader.Close();
      string path = Path.GetTempFileName().Replace(".tmp", EST);
      FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
      fileStream.Write(buffer, 0, buffer.Length);
      fileStream.Close();
      return path;
    }

    private void ScriviIntestazioneDipaAnnullato(
      ref PdfContentByte cb,
      ref Document document,
      int CODPOS,
      string DATCONMOV)
    {
      DataTable dataTable1 = new DataTable();
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
      BaseFont font2 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      DataLayer dataLayer = new DataLayer();
      Image instance = Image.GetInstance(this.EstraiFilePGM_Byte("CNT_LOGO"));
      instance.ScaleAbsolute(103f, 51f);
      instance.SetAbsolutePosition(82f, 545f);
      document.Add((IElement) instance);
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(1, "FONDAZIONE E.N.P.A.I.A.", 395f, 580f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(1, "ENTE NAZIONALE DI PREVIDENZA PER GLI ADDETTI E PER GLI", 395f, 570f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(1, "IMPIEGATI IN AGRICOLTURA", 395f, 560f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Viale Beethoven, 48 - 00144 ROMA", 395f, 550f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Call Center 800.010270 - Fax 06/5914444 - 06/5458385", 395f, 540f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Internet: www.enpaia.it       Email: info@enpaia.it", 395f, 530f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Ufficio contributi e riscossione", 133f, 542f, 0.0f);
      cb.EndText();
      string strSQL1 = " SELECT  AZI.RAGSOC, AZI.CODPOS, DUG.DENDUG, INDSED.IND, INDSED.NUMCIV, " + " INDSED.CAP, INDSED.DENLOC, INDSED.SIGPRO, INDSED.DENSTAEST, " + " INDSED.CODCOM, '' AS DENCOM " + " FROM  AZI " + " INNER JOIN INDSED ON " + " INDSED.CODPOS = AZI.CODPOS " + " LEFT JOIN DUG ON INDSED.CODDUG = DUG.CODDUG " + " WHERE INDSED.TIPIND=1 " + " AND INDSED.DATINI <= " + this.Module_DB2Date(Convert.ToDateTime(DATCONMOV)) + " AND AZI.CODPOS= " + CODPOS.ToString() + " AND INDSED.DATCOM = (SELECT MAX(DATCOM) FROM INDSED WHERE " + " INDSED.CODPOS = " + CODPOS.ToString() + " AND TIPIND = 1 AND INDSED.DATINI <= " + this.Module_DB2Date(Convert.ToDateTime(DATCONMOV)) + ") " + " ORDER BY AZI.CODPOS, INDSED.DATCOM DESC FETCH FIRST 1 ROWS ONLY";
      DataTable dataTable2 = dataLayer.GetDataTable(strSQL1);
      if (dataTable2.Rows.Count <= 0)
        return;
      if (dataTable2.Rows[0]["CODCOM"].ToString().Trim() != "")
      {
        string strSQL2 = "SELECT VALUE(DENCOM,'') AS DENCOM FROM CODCOM WHERE CODCOM = " + this.objNet.DoublePeakForSql(dataTable2.Rows[0]["CODCOM"].ToString().Trim());
        string str = dataLayer.GetDataTable(strSQL2).ToString();
        dataTable2.Rows[0]["DENCOM"] = (object) str;
      }
      else
        dataTable2.Rows[0]["DENCOM"] = (object) "";
      string text1 = dataTable2.Rows[0]["RAGSOC"].ToString().Trim();
      string text2;
      if (DBNull.Value.Equals(dataTable2.Rows[0]["NUMCIV"]) | dataTable2.Rows[0]["NUMCIV"].ToString().Trim() == "")
        text2 = dataTable2.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable2.Rows[0]["IND"].ToString().Trim();
      else
        text2 = dataTable2.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable2.Rows[0]["IND"].ToString().Trim() + ", " + dataTable2.Rows[0]["NUMCIV"].ToString().Trim();
      string text3;
      if (!DBNull.Value.Equals(dataTable2.Rows[0]["DENSTAEST"]))
        text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENSTAEST"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
      else if (dataTable2.Rows[0]["DENLOC"].ToString().Trim() != dataTable2.Rows[0]["DENCOM"].ToString().Trim())
      {
        if (dataTable2.Rows[0]["DENLOC"].ToString().Trim() == "")
          text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENCOM"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
        else
          text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENLOC"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
      }
      else
        text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENCOM"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
      BaseFont font3 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.BeginText();
      cb.SetFontAndSize(font3, 8.25f);
      cb.ShowTextAligned(0, text1, 565f, 552f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font3, 8.25f);
      cb.ShowTextAligned(0, text2, 565f, 542f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font3, 8.25f);
      cb.ShowTextAligned(0, text3, 565f, 532f, 0.0f);
      cb.EndText();
    }

    private void ScriviPieDiPaginaDipaAnnullato(
      ref PdfContentByte cb,
      string CODCAUSAN,
      string NUMSANANN)
    {
      DataTable dataTable1 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      cb.BeginText();
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.SetFontAndSize(font1, 5.25f);
      cb.ShowTextAligned(0, "Legenda causali:", 10f, 71f, 0.0f);
      cb.EndText();
      int y = 64;
      string str1 = "SELECT DISTINCT CODCAU, VALUE(DESCAUREP, ' ') AS DESC FROM TIPMOVCAU ";
      string strSQL = !(CODCAUSAN == "") ? str1 + " WHERE TIPMOV IN('ANN_DP', 'ANN_SAN_MD', 'ANN_SAN_RD') ORDER BY CODCAU ASC" : str1 + " WHERE TIPMOV IN('ANN_DP') ORDER BY CODCAU ASC";
      DataTable dataTable2 = dataLayer.GetDataTable(strSQL);
      if (dataTable2.Rows.Count > 0)
      {
        for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
        {
          if (dataTable2.Rows[index]["DESC"].ToString() != "")
          {
            string str2 = dataTable2.Rows[index]["CODCAU"]?.ToString() + " " + dataTable2.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[index]["DESC"].ToString().Length - 1);
            cb.BeginText();
            BaseFont font2 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
            cb.SetFontAndSize(font2, 5.25f);
            cb.ShowTextAligned(0, str2.Trim(), 10f, (float) y, 0.0f);
            cb.EndText();
          }
          else
          {
            string text = "";
            cb.BeginText();
            BaseFont font3 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
            cb.SetFontAndSize(font3, 5.25f);
            cb.ShowTextAligned(0, text, 10f, (float) y, 0.0f);
            cb.EndText();
          }
          y -= 7;
        }
      }
      else
      {
        for (int index = 0; index <= 5; ++index)
        {
          cb.BeginText();
          BaseFont font4 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
          cb.SetFontAndSize(font4, 5.25f);
          cb.ShowTextAligned(0, "", 10f, (float) y, 0.0f);
          cb.EndText();
          y -= 7;
        }
      }
      cb.BeginText();
      BaseFont font5 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.SetFontAndSize(font5, 8.25f);
      cb.ShowTextAligned(1, "Per informazioni telefonare al Call Center 800.010270 - 800.313231", 420f, 10f, 0.0f);
      cb.EndText();
    }

    private void ScriviTestataDipaAnnullato(
      ref iTextSharp.text.Table tabTestata,
      ref Cell Cell,
      ref iTextSharp.text.Font FontTestata)
    {
      Cell = new Cell((IElement) new Phrase("Matricola", FontTestata));
      Cell.HorizontalAlignment = 1;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 0);
      Cell = new Cell((IElement) new Phrase("Cognome e Nome", FontTestata));
      Cell.HorizontalAlignment = 0;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 1);
      Cell = new Cell((IElement) new Phrase("Periodo", FontTestata));
      Cell.HorizontalAlignment = 1;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 2);
      Cell = new Cell((IElement) new Phrase("Retribuzione", FontTestata));
      Cell.HorizontalAlignment = 2;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 3);
      Cell = new Cell((IElement) new Phrase("Aliquota", FontTestata));
      Cell.HorizontalAlignment = 1;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 5);
      Cell = new Cell((IElement) new Phrase("Contributo", FontTestata));
      Cell.HorizontalAlignment = 2;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.Colspan = 2;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 6);
      Cell = new Cell((IElement) new Phrase("Sanzione", FontTestata));
      Cell.HorizontalAlignment = 2;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.Colspan = 2;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 8);
    }

    private void scriviNumPagDipaAnnullato(ref PdfContentByte cb, int cntNumPag, int TotPag)
    {
      string text = "Pag. " + cntNumPag.ToString() + " di " + TotPag.ToString();
      cb.BeginText();
      BaseFont font = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.SetFontAndSize(font, 7.25f);
      cb.ShowTextAligned(1, text, 800f, 10f, 0.0f);
      cb.EndText();
    }

    private void ScriviSanzioneDipaAnnullato(
      ref PdfContentByte cb,
      string CODCAUSAN,
      string DATSANANN,
      string DATCONMOV,
      string NUMMOV,
      string DATSAN,
      string NUMSAN,
      string MESDEN,
      string ANNDEN,
      int CODPOS,
      string DATMOVANN,
      string NUMMOVANN,
      string NUMSANANN)
    {
      DateTimeFormatInfo dateTimeFormat = new CultureInfo("it-IT", false).DateTimeFormat;
      BaseFont font = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      string str1 = !(DATCONMOV != "") ? "" : Convert.ToString(Convert.ToDateTime(DATCONMOV));
      string str2 = !(DATMOVANN != "") ? "" : Convert.ToString(Convert.ToDateTime(DATMOVANN));
      string str3 = !(DATSAN != "") ? "" : Convert.ToString(Convert.ToDateTime(DATSAN));
      string text1;
      if (NUMSAN == "")
        text1 = "Nota di annullamento n. " + NUMMOVANN.Trim() + " emessa il " + str2 + " riferita alla Autodenuncia Contributi di " + dateTimeFormat.MonthNames[Convert.ToInt32(MESDEN) - 1].ToString().ToUpper() + " " + ANNDEN;
      else
        text1 = "Nota di annullamento n. " + NUMMOVANN.Trim() + " e Nota di annullamento n. " + NUMSANANN.Trim() + " emesse il " + str2 + " riferita alla Autodenuncia Contributi di " + dateTimeFormat.MonthNames[Convert.ToInt32(MESDEN) - 1].ToString().ToUpper() + " " + ANNDEN;
      string text2 = "Posizione assicurativa " + CODPOS.ToString();
      cb.BeginText();
      cb.SetFontAndSize(font, 8.25f);
      cb.ShowTextAligned(0, text1, 10f, 500f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font, 8.25f);
      cb.ShowTextAligned(0, text2, 10f, 490f, 0.0f);
      cb.EndText();
    }

    private void ScriviTotaliDipaAnnullato(
      ref iTextSharp.text.Table TabTotali,
      ref Cell CellTotali,
      ref iTextSharp.text.Font FontDettaglio,
      ref iTextSharp.text.Font FontNascondi,
      ref iTextSharp.text.Font FontTestata,
      string CODCAU,
      string NUMSANANN,
      string IMPADDREC,
      string IMPABB,
      string IMPASSCON,
      string IMPCONTOT,
      string IMPSANTOT,
      string NUMSAN,
      ref PdfContentByte cb,
      string NUMMOVANN)
    {
      string str1 = "";
      DataTable dataTable1 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 3, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 4, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 5, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 6, 0);
      CellTotali = new Cell((IElement) new Phrase("Importo contributo annullato", FontDettaglio));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 1);
      CellTotali = new Cell((IElement) new Phrase("Importo addizionale annullato", FontDettaglio));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 1);
      CellTotali = new Cell((IElement) new Phrase("Assistenza contrattuale annullata", FontDettaglio));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 1);
      CellTotali = new Cell((IElement) new Phrase("Abbonamento periodico annullato", FontDettaglio));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 3, 1);
      if (CODCAU == "")
      {
        str1 = "Sanzioni annullate";
      }
      else
      {
        string strSQL = "SELECT VALUE(TASSO, 0.00) AS TASSO, TIPMOV, DESCAUREP AS DESC FROM TIPMOVCAU " + " WHERE CODCAU ='" + CODCAU + "' AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
        dataTable1.Clear();
        DataTable dataTable2 = dataLayer.GetDataTable(strSQL);
        if (dataTable2.Rows.Count > 0)
        {
          for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
          {
            string str2 = dataTable2.Rows[index]["TIPMOV"].ToString().Trim();
            if (!(str2 == "SAN_MD"))
            {
              if (str2 == "SAN_RD")
                str1 = !(dataTable2.Rows[index]["DESC"].ToString() != "") ? "" : dataTable2.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[index]["DESC"].ToString().Length - 1) + " annullate";
            }
            else
              str1 = !(dataTable2.Rows[index]["DESC"].ToString() != "") ? "" : dataTable2.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[index]["DESC"].ToString().Length - 1) + " annullate";
          }
        }
        else
          str1 = "";
      }
      CellTotali = new Cell((IElement) new Phrase(str1, FontDettaglio));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 4, 1);
      CellTotali = new Cell((IElement) new Phrase(".....", FontNascondi));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 6;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 5, 1);
      CellTotali = new Cell((IElement) new Phrase("Importo complessivo annullato", FontTestata));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 6, 1);
      CellTotali = new Cell((IElement) new Phrase("€", FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 2);
      CellTotali = new Cell((IElement) new Phrase("€", FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 2);
      CellTotali = new Cell((IElement) new Phrase("€", FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 2);
      CellTotali = new Cell((IElement) new Phrase("€", FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 3, 2);
      CellTotali = new Cell((IElement) new Phrase("€", FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 6;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 4, 2);
      CellTotali = new Cell();
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 5, 2);
      CellTotali = new Cell((IElement) new Phrase("€", FontTestata));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 6, 2);
      Decimal num;
      if (NUMSAN == "")
        IMPSANTOT = "0,00";
      else if (NUMMOVANN == "")
      {
        if (NUMSANANN != "")
        {
          IMPSANTOT = "0,00";
        }
        else
        {
          num = Convert.ToDecimal(IMPSANTOT);
          IMPSANTOT = num.ToString("#,##0.#0");
        }
      }
      else
      {
        num = Convert.ToDecimal(IMPSANTOT);
        IMPSANTOT = num.ToString("#,##0.#0");
      }
      num = Convert.ToDecimal(IMPABB);
      IMPABB = num.ToString("#,##0.#0");
      string str3 = !(NUMSAN == "") ? (!(NUMMOVANN == "") ? Convert.ToString(Convert.ToDecimal(IMPCONTOT) + Convert.ToDecimal(IMPADDREC) + Convert.ToDecimal(IMPASSCON) + Convert.ToDecimal(IMPABB) + Convert.ToDecimal(IMPSANTOT)) : (!(NUMSANANN != "") ? Convert.ToString(Convert.ToDecimal(IMPCONTOT) + Convert.ToDecimal(IMPADDREC) + Convert.ToDecimal(IMPASSCON) + Convert.ToDecimal(IMPABB) + Convert.ToDecimal(IMPSANTOT)) : Convert.ToString(Convert.ToDecimal(IMPCONTOT) + Convert.ToDecimal(IMPADDREC) + Convert.ToDecimal(IMPASSCON) + Convert.ToDecimal(IMPABB)))) : Convert.ToString(Convert.ToDecimal(IMPCONTOT) + Convert.ToDecimal(IMPADDREC) + Convert.ToDecimal(IMPASSCON) + Convert.ToDecimal(IMPABB));
      ref Cell local1 = ref CellTotali;
      num = Convert.ToDecimal(IMPCONTOT);
      Cell cell1 = new Cell((IElement) new Phrase(num.ToString("#,##0.#0"), FontDettaglio));
      local1 = cell1;
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 3);
      ref Cell local2 = ref CellTotali;
      num = Convert.ToDecimal(IMPADDREC);
      Cell cell2 = new Cell((IElement) new Phrase(num.ToString("#,##0.#0"), FontDettaglio));
      local2 = cell2;
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 3);
      ref Cell local3 = ref CellTotali;
      num = Convert.ToDecimal(IMPASSCON);
      Cell cell3 = new Cell((IElement) new Phrase(num.ToString("#,##0.#0"), FontDettaglio));
      local3 = cell3;
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 3);
      CellTotali = new Cell((IElement) new Phrase(IMPABB, FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 3, 3);
      CellTotali = new Cell((IElement) new Phrase(IMPSANTOT, FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 6;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 4, 3);
      CellTotali = new Cell((IElement) new Phrase("....", FontNascondi));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 6;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 5, 3);
      ref Cell local4 = ref CellTotali;
      num = Convert.ToDecimal(str3);
      Cell cell4 = new Cell((IElement) new Phrase(num.ToString("#,##0.#0"), FontTestata));
      local4 = cell4;
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 6, 3);
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
      iTextSharp.text.Font font2 = new iTextSharp.text.Font(font1, 7.75f, 1);
      iTextSharp.text.Font font3 = new iTextSharp.text.Font(font1, 7.75f, 0, Color.WHITE);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 7, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 8, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 9, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 10, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 11, 0);
    }

    public byte[] EstraiFilePGM_Byte(string CHIAVE)
    {
      DataLayer dataLayer = new DataLayer();
      int ordinal = 0;
      string strQuery = "SELECT FILE FROM FILE_PGM WHERE CHIAVE = " + DBMethods.DoublePeakForSql(CHIAVE);
      iDB2DataReader dataReaderFromQuery = dataLayer.GetDataReaderFromQuery(strQuery, CommandType.Text);
      dataReaderFromQuery.Read();
      byte[] buffer = new byte[dataReaderFromQuery.GetBytes(ordinal, 0L, (byte[]) null, 0, int.MaxValue) - 1L + 1L];
      dataReaderFromQuery.GetBytes(ordinal, 0L, buffer, 0, buffer.Length);
      dataReaderFromQuery.Close();
      return buffer;
    }

    public string Module_DB2Date(DateTime Data, bool ConApiciFinali = true, bool IsTimeStamp = false)
    {
      string str = Data.Year.ToString("0000") + "-" + Data.Month.ToString("00") + "-" + Data.Day.ToString("00");
      if (IsTimeStamp)
        str = str + "-" + Data.Hour.ToString().PadLeft(2, Convert.ToChar("0")) + "." + Data.Minute.ToString().PadLeft(2, Convert.ToChar("0")) + "." + Data.Second.ToString().PadLeft(2, Convert.ToChar("0")) + ".037500";
      return ConApiciFinali ? "'" + str + "'" : str;
    }

    private void ScriviIntestazioneNotifica(
      DataLayer dl,
      ref PdfContentByte cb,
      ref Document document,
      int CODPOS,
      string DATCONMOV,
      string PATH_FONT,
      string PATH_FONTB)
    {
      DataTable dataTable1 = new DataTable();
      BaseFont.CreateFont(PATH_FONTB, "Cp1252", false);
      BaseFont.CreateFont(PATH_FONT, "Cp1252", false);
      Image instance = Image.GetInstance(this.EstraiFilePGM_Byte("CNT_LOGO"));
      instance.ScaleAbsolute(540f, 110f);
      instance.SetAbsolutePosition(15f, 505f);
      document.Add((IElement) instance);
      string strSQL1 = " SELECT  AZI.RAGSOC, AZI.CODPOS, DUG.DENDUG, INDSED.IND, INDSED.NUMCIV, " + " INDSED.CAP, INDSED.DENLOC, INDSED.SIGPRO, INDSED.DENSTAEST, " + " INDSED.CODCOM, '' AS DENCOM " + " FROM  AZI " + " INNER JOIN INDSED ON " + " INDSED.CODPOS = AZI.CODPOS " + " LEFT JOIN DUG ON INDSED.CODDUG = DUG.CODDUG " + " WHERE INDSED.TIPIND=1 " + " AND INDSED.DATINI <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATCONMOV)) + " AND AZI.CODPOS= " + CODPOS.ToString() + " AND INDSED.DATCOM = (SELECT MAX(DATCOM) FROM INDSED WHERE " + " INDSED.CODPOS = " + CODPOS.ToString() + " AND TIPIND = 1 AND INDSED.DATINI <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATCONMOV)) + ") " + " ORDER BY AZI.CODPOS, INDSED.DATCOM DESC FETCH FIRST 1 ROWS ONLY";
      DataTable dataTable2 = dl.GetDataTable(strSQL1);
      if (dataTable2.Rows.Count <= 0)
        return;
      if (dataTable2.Rows[0]["CODCOM"].ToString().Trim() != "")
      {
        string strSQL2 = "SELECT VALUE(DENCOM,'') AS DENCOM FROM CODCOM WHERE CODCOM = " + this.objNet.DoublePeakForSql(dataTable2.Rows[0]["CODCOM"].ToString().Trim());
        string str = dl.GetDataTable(strSQL2).Rows[0]["DENCOM"].ToString();
        dataTable2.Rows[0]["DENCOM"] = (object) str;
      }
      else
        dataTable2.Rows[0]["DENCOM"] = (object) "";
      string text1 = dataTable2.Rows[0]["RAGSOC"].ToString().Trim();
      string text2;
      if (DBNull.Value.Equals(dataTable2.Rows[0]["NUMCIV"]) | dataTable2.Rows[0]["NUMCIV"].ToString().Trim() == "")
        text2 = dataTable2.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable2.Rows[0]["IND"].ToString().Trim();
      else
        text2 = dataTable2.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable2.Rows[0]["IND"].ToString().Trim() + ", " + dataTable2.Rows[0]["NUMCIV"].ToString().Trim();
      string text3;
      if (!DBNull.Value.Equals(dataTable2.Rows[0]["DENSTAEST"]))
        text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENSTAEST"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
      else if (dataTable2.Rows[0]["DENLOC"].ToString().Trim() != dataTable2.Rows[0]["DENCOM"].ToString().Trim())
      {
        if (dataTable2.Rows[0]["DENLOC"].ToString().Trim() == "")
          text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENCOM"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
        else
          text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENLOC"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
      }
      else
        text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENCOM"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
      BaseFont font = BaseFont.CreateFont(PATH_FONT, "Cp1252", false);
      cb.BeginText();
      cb.SetFontAndSize(font, 8.25f);
      cb.ShowTextAligned(0, text1, 565f, 552f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font, 8.25f);
      cb.ShowTextAligned(0, text2, 565f, 542f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font, 8.25f);
      cb.ShowTextAligned(0, text3, 565f, 532f, 0.0f);
      cb.EndText();
    }

    private void ScriviPieDiPaginaNotifica(
      DataLayer dl,
      ref PdfContentByte cb,
      string CODCAUSAN,
      string NUMSANANN,
      string PATH_FONT)
    {
      DataTable dataTable1 = new DataTable();
      cb.BeginText();
      BaseFont font1 = BaseFont.CreateFont(PATH_FONT, "Cp1252", false);
      cb.SetFontAndSize(font1, 5.25f);
      cb.ShowTextAligned(0, "Legenda causali:", 10f, 71f, 0.0f);
      cb.EndText();
      int y = 64;
      string str1 = "SELECT DISTINCT CODCAU, VALUE(DESCAUREP, ' ') AS DESC FROM TIPMOVCAU ";
      string strSQL = !(CODCAUSAN == "") ? (!(NUMSANANN != "") ? str1 + " WHERE TIPMOV IN('NU', 'SAN_MD', 'ANN_SAN_MD', 'ANN_NU', 'SAN_RD','ANN_SAN_RD') ORDER BY CODCAU ASC" : str1 + " WHERE TIPMOV IN('NU', 'SAN_MD', 'ANN_SAN_MD', 'ANN_NU', 'SAN_RD','ANN_SAN_RD') ORDER BY CODCAU ASC") : str1 + " WHERE TIPMOV IN('NU', 'ANN_NU') ORDER BY CODCAU ASC";
      DataTable dataTable2 = dl.GetDataTable(strSQL);
      if (dataTable2.Rows.Count > 0)
      {
        for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
        {
          if (dataTable2.Rows[index]["DESC"].ToString() != "")
          {
            string str2 = dataTable2.Rows[index]["CODCAU"]?.ToString() + " " + dataTable2.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[index]["DESC"].ToString().Length - 1);
            cb.BeginText();
            BaseFont font2 = BaseFont.CreateFont(PATH_FONT, "Cp1252", false);
            cb.SetFontAndSize(font2, 5.25f);
            cb.ShowTextAligned(0, str2.Trim(), 10f, (float) y, 0.0f);
            cb.EndText();
          }
          else
          {
            string text = "";
            cb.BeginText();
            BaseFont font3 = BaseFont.CreateFont(PATH_FONT, "Cp1252", false);
            cb.SetFontAndSize(font3, 5.25f);
            cb.ShowTextAligned(0, text, 10f, (float) y, 0.0f);
            cb.EndText();
          }
          y -= 7;
        }
      }
      else
      {
        for (int index = 0; index <= 5; ++index)
        {
          cb.BeginText();
          BaseFont font4 = BaseFont.CreateFont(PATH_FONT, "Cp1252", false);
          cb.SetFontAndSize(font4, 5.25f);
          cb.ShowTextAligned(0, "", 10f, (float) y, 0.0f);
          cb.EndText();
          y -= 7;
        }
      }
      cb.BeginText();
      BaseFont font5 = BaseFont.CreateFont(PATH_FONT, "Cp1252", false);
      cb.SetFontAndSize(font5, 8.25f);
      cb.ShowTextAligned(1, "Per informazioni telefonare al Call Center 800.010270 - 800.313231", 420f, 10f, 0.0f);
      cb.EndText();
    }

    private void scriviNumPagNotifica(
      ref PdfContentByte cb,
      int cntNumPag,
      int TotPag,
      string PATH_FONT)
    {
      string text = "Pag. " + cntNumPag.ToString() + " di " + TotPag.ToString();
      cb.BeginText();
      BaseFont font = BaseFont.CreateFont(PATH_FONT, "Cp1252", false);
      cb.SetFontAndSize(font, 7.25f);
      cb.ShowTextAligned(1, text, 800f, 10f, 0.0f);
      cb.EndText();
    }

    private void ScriviTotaliNotifica(
      ref iTextSharp.text.Table TabTotali,
      ref Cell CellTotali,
      ref iTextSharp.text.Font FontDettaglio,
      ref iTextSharp.text.Font FontNascondi,
      ref iTextSharp.text.Font FontTestata,
      string CODCAU,
      string NUMSANANN,
      string IMPADDREC,
      string IMPABB,
      string IMPASSCON,
      string IMPCONTOT,
      string IMPSANTOT,
      string NUMSAN,
      ref PdfContentByte cb,
      string NUMMOVANN,
      string PATH_FONTB)
    {
      string str1 = "";
      DataTable dataTable1 = new DataTable();
      IMPCONTOT = IMPCONTOT == "" ? "0,00" : IMPCONTOT;
      IMPADDREC = IMPADDREC == "" ? "0,00" : IMPADDREC;
      DataLayer dataLayer = new DataLayer();
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 3, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 4, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 5, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 6, 0);
      CellTotali = !(NUMMOVANN == "") ? new Cell((IElement) new Phrase("Importo contributo annullato", FontDettaglio)) : new Cell((IElement) new Phrase("Importo contributo", FontDettaglio));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 1);
      string str2 = !(NUMSANANN != "" & NUMMOVANN == "") ? (!(NUMMOVANN == "") ? "Importo addizionale annullato" : "Importo addizionale") : "Importo addizionale";
      CellTotali = new Cell((IElement) new Phrase(str2, FontDettaglio));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 1);
      CellTotali = !(NUMMOVANN == "") ? new Cell((IElement) new Phrase("Assistenza contrattuale annullata", FontDettaglio)) : new Cell((IElement) new Phrase("Assistenza contrattuale", FontDettaglio));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 1);
      CellTotali = !(NUMMOVANN == "") ? new Cell((IElement) new Phrase("Abbonamento periodico annullato", FontDettaglio)) : new Cell((IElement) new Phrase("Abbonamento periodico", FontDettaglio));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 3, 1);
      if (CODCAU == "")
        str1 = !(NUMMOVANN == "") ? "Sanzioni annullate" : "Sanzioni";
      else if (NUMSANANN != "" & NUMMOVANN == "")
      {
        str1 = "Sanzioni";
      }
      else
      {
        string strSQL = "SELECT VALUE(TASSO, 0.00) AS TASSO, TIPMOV, DESCAUREP AS DESC FROM TIPMOVCAU " + " WHERE CODCAU ='" + CODCAU + "' AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
        dataTable1.Clear();
        DataTable dataTable2 = dataLayer.GetDataTable(strSQL);
        if (dataTable2.Rows.Count > 0)
        {
          for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
          {
            string str3 = dataTable2.Rows[index]["TIPMOV"].ToString().Trim();
            if (!(str3 == "SAN_MD"))
            {
              if (str3 == "SAN_RD")
              {
                if (dataTable2.Rows[0]["DESC"].ToString() != "")
                {
                  if (NUMMOVANN == "")
                    str1 = dataTable2.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[index]["DESC"].ToString().Length - 1) + " al tasso del " + dataTable2.Rows[index]["TASSO"]?.ToString() + " % annuo";
                  else
                    str1 = dataTable2.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[index]["DESC"].ToString().Length - 1) + " annullate";
                }
                else
                  str1 = "";
              }
            }
            else if (dataTable2.Rows[index]["DESC"].ToString() != "")
            {
              if (NUMMOVANN == "")
                str1 = dataTable2.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[index]["DESC"].ToString().Length - 1) + " al tasso del " + dataTable2.Rows[index]["TASSO"]?.ToString() + " % annuo";
              else
                str1 = dataTable2.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[index]["DESC"].ToString().Length - 1) + " annullate";
            }
            else
              str1 = "";
          }
        }
        else
          str1 = "";
      }
      CellTotali = new Cell((IElement) new Phrase(str1, FontDettaglio));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 4, 1);
      CellTotali = new Cell((IElement) new Phrase(".....", FontNascondi));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 6;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 5, 1);
      CellTotali = !(NUMMOVANN == "") ? new Cell((IElement) new Phrase("Importo complessivo annullato", FontTestata)) : new Cell((IElement) new Phrase("Importo complessivo", FontTestata));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 6, 1);
      CellTotali = new Cell((IElement) new Phrase("€", FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 2);
      CellTotali = new Cell((IElement) new Phrase("€", FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 2);
      CellTotali = new Cell((IElement) new Phrase("€", FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 2);
      CellTotali = new Cell((IElement) new Phrase("€", FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 3, 2);
      CellTotali = new Cell((IElement) new Phrase("€", FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 6;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 4, 2);
      CellTotali = new Cell();
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 5, 2);
      CellTotali = new Cell((IElement) new Phrase("€", FontTestata));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 6, 2);
      Decimal num;
      if (NUMSAN == "")
        IMPSANTOT = "0,00";
      else if (NUMMOVANN == "")
      {
        if (NUMSANANN != "")
        {
          IMPSANTOT = "0,00";
        }
        else
        {
          num = Convert.ToDecimal(IMPSANTOT);
          IMPSANTOT = num.ToString("#,##0.#0");
        }
      }
      else
      {
        num = Convert.ToDecimal(IMPSANTOT);
        IMPSANTOT = num.ToString("#,##0.#0");
      }
      num = Convert.ToDecimal(IMPABB);
      IMPABB = num.ToString("#,##0.#0");
      string str4 = !(NUMSAN == "") ? (!(NUMMOVANN == "") ? Convert.ToString(Convert.ToDecimal(IMPCONTOT) + Convert.ToDecimal(IMPADDREC) + Convert.ToDecimal(IMPASSCON) + Convert.ToDecimal(IMPABB) + Convert.ToDecimal(IMPSANTOT)) : (!(NUMSANANN != "") ? Convert.ToString(Convert.ToDecimal(IMPCONTOT) + Convert.ToDecimal(IMPADDREC) + Convert.ToDecimal(IMPASSCON) + Convert.ToDecimal(IMPABB) + Convert.ToDecimal(IMPSANTOT)) : Convert.ToString(Convert.ToDecimal(IMPCONTOT) + Convert.ToDecimal(IMPADDREC) + Convert.ToDecimal(IMPASSCON) + Convert.ToDecimal(IMPABB)))) : Convert.ToString(Convert.ToDecimal(IMPCONTOT) + Convert.ToDecimal(IMPADDREC) + Convert.ToDecimal(IMPASSCON) + Convert.ToDecimal(IMPABB));
      ref Cell local1 = ref CellTotali;
      num = Convert.ToDecimal(IMPCONTOT);
      Cell cell1 = new Cell((IElement) new Phrase(num.ToString("#,##0.#0"), FontDettaglio));
      local1 = cell1;
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 3);
      ref Cell local2 = ref CellTotali;
      num = Convert.ToDecimal(IMPADDREC);
      Cell cell2 = new Cell((IElement) new Phrase(num.ToString("#,##0.#0"), FontDettaglio));
      local2 = cell2;
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 3);
      ref Cell local3 = ref CellTotali;
      num = Convert.ToDecimal(IMPASSCON);
      Cell cell3 = new Cell((IElement) new Phrase(num.ToString("#,##0.#0"), FontDettaglio));
      local3 = cell3;
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 3);
      CellTotali = new Cell((IElement) new Phrase(IMPABB, FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 3, 3);
      CellTotali = new Cell((IElement) new Phrase(IMPSANTOT, FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 6;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 4, 3);
      CellTotali = new Cell((IElement) new Phrase("....", FontNascondi));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 6;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 5, 3);
      ref Cell local4 = ref CellTotali;
      num = Convert.ToDecimal(str4);
      Cell cell4 = new Cell((IElement) new Phrase(num.ToString("#,##0.#0"), FontTestata));
      local4 = cell4;
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 6, 3);
      BaseFont font1 = BaseFont.CreateFont(PATH_FONTB, "Cp1252", false);
      iTextSharp.text.Font font2 = new iTextSharp.text.Font(font1, 7.75f, 1);
      iTextSharp.text.Font font3 = new iTextSharp.text.Font(font1, 7.75f, 0, Color.WHITE);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 7, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 8, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 9, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 10, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 11, 0);
      if (!(NUMMOVANN == ""))
        return;
      CellTotali = new Cell((IElement) new Phrase("Da versare entro 30 giorni dalla data di emissione tramite gli allegati M.Av.", font2));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 6;
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 12, 0);
    }

    private void ScriviTestataNotifica(ref iTextSharp.text.Table tabTestata, ref Cell Cell, ref iTextSharp.text.Font FontTestata)
    {
      Cell = new Cell((IElement) new Phrase("Matricola", FontTestata));
      Cell.HorizontalAlignment = 1;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 0);
      Cell = new Cell((IElement) new Phrase("Cognome e Nome", FontTestata));
      Cell.HorizontalAlignment = 0;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 1);
      Cell = new Cell((IElement) new Phrase("Periodo", FontTestata));
      Cell.HorizontalAlignment = 1;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 2);
      Cell = new Cell((IElement) new Phrase("Retribuzione", FontTestata));
      Cell.HorizontalAlignment = 2;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 3);
      Cell = new Cell((IElement) new Phrase("Aliquota", FontTestata));
      Cell.HorizontalAlignment = 1;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 5);
      Cell = new Cell((IElement) new Phrase("Contributi", FontTestata));
      Cell.HorizontalAlignment = 2;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.Colspan = 2;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 6);
      Cell = new Cell((IElement) new Phrase("Sanzione", FontTestata));
      Cell.HorizontalAlignment = 2;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.Colspan = 2;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 8);
    }

    private void ScriviSanzioneNotifica(
      ref PdfContentByte cb,
      string CODCAUSAN,
      string DATSANANN,
      string DATCONMOV,
      string NUMMOV,
      string DATSAN,
      string NUMSAN,
      string MESDEN,
      string ANNDEN,
      int CODPOS,
      string DATMOVANN,
      string NUMMOVANN,
      string NUMSANANN,
      string PATH_FONT)
    {
      string text1 = "";
      DateTimeFormatInfo dateTimeFormat = new CultureInfo("it-IT", false).DateTimeFormat;
      BaseFont font = BaseFont.CreateFont(PATH_FONT, "Cp1252", false);
      string str1 = !(DATCONMOV != "") ? "" : Convert.ToString(Convert.ToDateTime(DATCONMOV));
      string str2 = !(DATMOVANN != "") ? "" : Convert.ToString(Convert.ToDateTime(DATMOVANN));
      string str3 = !(DATSAN != "") ? "" : Convert.ToString(Convert.ToDateTime(DATSAN));
      string text2;
      if (DATMOVANN == "")
      {
        if (NUMSAN == "")
          text2 = "Nota d'ufficio n. " + NUMMOV.Trim() + " emessa il " + str1 + " relativa al periodo " + dateTimeFormat.MonthNames[Convert.ToInt32(MESDEN) - 1].ToString().ToUpper() + " " + ANNDEN;
        else if (DATSANANN == "")
          text2 = "Nota d'ufficio n. " + NUMMOV.Trim() + " e Nota sanzione n. " + NUMSAN.Trim() + " emesse il " + str3 + " relative al periodo " + dateTimeFormat.MonthNames[Convert.ToInt32(MESDEN) - 1].ToString().ToUpper() + " " + ANNDEN;
        else
          text2 = "Nota d'ufficio n. " + NUMMOV.Trim() + " emessa il " + str1 + " relativa al periodo " + dateTimeFormat.MonthNames[Convert.ToInt32(MESDEN) - 1].ToString().ToUpper() + " " + ANNDEN;
      }
      else if (NUMSAN == "")
      {
        text2 = "Nota di annullamento n. " + NUMMOVANN.Trim() + " emessa il " + str2;
        text1 = "relativa a Nota d'ufficio n. " + NUMMOV.Trim() + " emessa il " + str1 + " riferita al periodo " + dateTimeFormat.MonthNames[Convert.ToInt32(MESDEN) - 1].ToString().ToUpper() + " " + ANNDEN;
      }
      else
      {
        text2 = "Nota di annullamento n. " + NUMMOVANN.Trim() + " e Nota di annullamento n. " + NUMSANANN.Trim() + " emesse il " + str2;
        text1 = "relative a Nota d'ufficio n. " + NUMMOV.Trim() + " e Nota di sanzione  n. " + NUMSAN.Trim() + " " + " riferite a " + dateTimeFormat.MonthNames[Convert.ToInt32(MESDEN) - 1].ToString().ToUpper() + " " + ANNDEN;
      }
      string text3 = "Posizione assicurativa " + CODPOS.ToString();
      cb.BeginText();
      cb.SetFontAndSize(font, 8.25f);
      cb.ShowTextAligned(0, text2, 10f, 500f, 0.0f);
      cb.EndText();
      if (text1 != "")
      {
        cb.BeginText();
        cb.SetFontAndSize(font, 8.25f);
        cb.ShowTextAligned(0, text1, 10f, 490f, 0.0f);
        cb.EndText();
        cb.BeginText();
        cb.SetFontAndSize(font, 8.25f);
        cb.ShowTextAligned(0, text3, 10f, 480f, 0.0f);
        cb.EndText();
      }
      else
      {
        cb.BeginText();
        cb.SetFontAndSize(font, 8.25f);
        cb.ShowTextAligned(0, text3, 10f, 490f, 0.0f);
        cb.EndText();
      }
    }

    public void CreaStampaDipa(ref DataTable dtDipa, string strPath)
    {
      Document document = new Document(PageSize.A4.Rotate(), 15f, 15f, 15f, 15f);
      PdfWriter instance = PdfWriter.GetInstance(document, (Stream) new FileStream(strPath, FileMode.Create));
      int num1 = 1;
      int num2 = 0;
      DataTable dataTable1 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      DataTable dataTable2 = new DataTable();
      int cntNumPag = 1;
      string RAGSOC = "";
      string TEL1 = "";
      string EMAIL = "";
      string ABB = "";
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      BaseFont font2 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
      BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAI"), "Cp1252", false);
      document.Open();
      for (int index1 = 0; index1 <= dtDipa.Rows.Count - 1; ++index1)
      {
        iTextSharp.text.Table tabTotali = new iTextSharp.text.Table(3, 1);
        iTextSharp.text.Table tabFooter = new iTextSharp.text.Table(7, 4);
        iTextSharp.text.Table tabFooter2 = new iTextSharp.text.Table(2, 6);
        if (index1 > 0)
        {
          cntNumPag = 1;
          num2 = 0;
          num1 = 1;
          document.NewPage();
        }
        int int32_1 = Convert.ToInt32(dtDipa.Rows[index1]["PRODEN"]);
        int int32_2 = Convert.ToInt32(dtDipa.Rows[index1]["CODPOS"]);
        int int32_3 = Convert.ToInt32(dtDipa.Rows[index1]["ANNDEN"]);
        int int32_4 = Convert.ToInt32(dtDipa.Rows[index1]["MESDEN"]);
        string strSQL1 = "SELECT DATCONMOV FROM DENTES" + " WHERE ANNDEN = " + int32_3.ToString() + " AND MESDEN = " + int32_4.ToString() + " AND PRODEN = " + int32_1.ToString() + " AND CODPOS = " + int32_2.ToString();
        string strData = dataLayer.GetDataTable(strSQL1).Rows[0]["DATCONMOV"]?.ToString() ?? "";
        DataTable dataTable3 = new DataTable();
        string strSQL2 = " SELECT AZI.RAGSOC, AZISTO.ABB, INDSED.TEL1, INDSED.EMAIL" + " FROM   AZI LEFT JOIN INDSED  ON  AZI.CODPOS =  INDSED.CODPOS" + " LEFT JOIN  AZISTO ON  AZI.CODPOS =  AZISTO.CODPOS" + " WHERE INDSED.TIPIND=1 " + " AND INDSED.DATINI <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + " AND AZI.CODPOS= " + int32_2.ToString() + " AND INDSED.DATCOM = (SELECT MAX(DATCOM) FROM INDSED WHERE " + " INDSED.CODPOS = " + int32_2.ToString() + " AND TIPIND = 1 AND INDSED.DATINI <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + ")" + " ORDER BY AZI.CODPOS, INDSED.DATCOM DESC FETCH FIRST 1 ROWS ONLY";
        DataTable dataTable4 = dataLayer.GetDataTable(strSQL2);
        if (dataTable4.Rows.Count > 0)
        {
          RAGSOC = dataTable4.Rows[0]["RAGSOC"].ToString().Trim();
          TEL1 = dataTable4.Rows[0]["TEL1"].ToString().Trim();
          EMAIL = dataTable4.Rows[0]["EMAIL"].ToString().Trim();
          ABB = dataTable4.Rows[0]["ABB"].ToString().Trim();
        }
        PdfContentByte directContent = instance.DirectContent;
        string strSQL3 = "SELECT NUMMOVANN, DATMOVANN, DATSANANN FROM DENTES" + " WHERE ANNDEN = " + int32_3.ToString() + " AND MESDEN = " + int32_4.ToString() + " AND PRODEN = " + int32_1.ToString() + " AND CODPOS = " + int32_2.ToString();
        DataTable dataTable5 = dataLayer.GetDataTable(strSQL3);
        BaseFont font3 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
        string DATSANANN = !DBNull.Value.Equals(dataTable5.Rows[0]["DATSANANN"]) ? dataTable5.Rows[0]["DATSANANN"].ToString() : string.Empty;
        string NUMMOVANN = !DBNull.Value.Equals(dataTable5.Rows[0]["NUMMOVANN"]) ? dataTable5.Rows[0]["NUMMOVANN"].ToString() : string.Empty;
        string str1 = !DBNull.Value.Equals(dataTable5.Rows[0]["DATMOVANN"]) ? dataTable5.Rows[0]["DATMOVANN"].ToString() : string.Empty;
        if (str1.ToString() != "")
        {
          directContent.BeginText();
          directContent.SetFontAndSize(font3, 11f);
          directContent.ShowTextAligned(1, "DOCUMENTO ANNULLATO", 300f, 575f, 0.0f);
          directContent.EndText();
        }
        this.ScriviIntestazioneDipa(ref directContent, ref document, int32_2, RAGSOC, TEL1, EMAIL, ABB, int32_3, int32_4);
        this.ScriviPieDiPaginaDipa(ref directContent);
        Cell Cell = new Cell();
        iTextSharp.text.Table tabTestata = new iTextSharp.text.Table(17, 2);
        tabTestata.AutoFillEmptyCells = true;
        tabTestata.WidthPercentage = 100f;
        tabTestata.BorderWidth = 0.0f;
        tabTestata.Cellspacing = 0.0f;
        tabTestata.Cellpadding = 2f;
        int[] widths = new int[17]
        {
          5,
          22,
          6,
          4,
          4,
          8,
          5,
          5,
          2,
          7,
          2,
          7,
          2,
          7,
          6,
          4,
          4
        };
        tabTestata.SetWidths(widths);
        iTextSharp.text.Font fontRigaTest = new iTextSharp.text.Font(font2, 6.75f, 0);
        iTextSharp.text.Font font4 = new iTextSharp.text.Font(font1, 7.75f, 0);
        iTextSharp.text.Font font5 = new iTextSharp.text.Font(font1, 7.75f, 0, Color.WHITE);
        this.ScriviTestataDipa(ref tabTestata, ref Cell, ref fontRigaTest);
        string strSQL4 = "SELECT ISCT.MAT, TRIM(ISCT.COG) || ' ' || TRIM(ISCT.NOM) AS NOME, DENDET.IMPRET," + " DENDET.ANNDEN, DENDET.MESDEN, DENDET.CODCON, DENDET.IMPABB, DENDET.IMPASSCON, " + " DENDET.IMPOCC, DENDET.IMPFIG, DENDET.CODLIV, DENDET.CODQUACON, DENDET.DAL, " + " DENDET.AL, " + " ( " + " SELECT DENLIV FROM CONLIV WHERE " + " CONLIV.CODCON = DENDET.CODCON " + " AND CONLIV.PROCON = DENDET.PROCON " + " AND  CONLIV.CODLIV = DENDET.CODLIV " + " ) AS LIVELLO, " + " DENDET.PERPAR, DENDET.ALIQUOTA" + " FROM ISCT RIGHT JOIN DENDET ON ISCT.MAT = DENDET.MAT" + " WHERE DENDET.ANNDEN = " + int32_3.ToString() + " AND DENDET.MESDEN = " + int32_4.ToString() + " AND DENDET.PRODEN = " + int32_1.ToString() + " AND DENDET.CODPOS = " + int32_2.ToString() + " AND DENDET.TIPMOV = 'DP'" + " ORDER BY ISCT.COG, DENDET.MAT, DENDET.DAL";
        DataTable dataTable6 = dataLayer.GetDataTable(strSQL4);
        int TotPag = !((Decimal) (dataTable6.Rows.Count - dataTable6.Rows.Count / 20 * 20) <= 6M) ? dataTable6.Rows.Count / 20 + 2 : dataTable6.Rows.Count / 20 + 1;
        this.scriviNumPagDipa(ref directContent, cntNumPag, TotPag);
        for (int index2 = 0; index2 <= dataTable6.Rows.Count - 1; ++index2)
        {
          if (num1 == 21)
          {
            tabTestata.Offset = 140f;
            document.Add((IElement) tabTestata);
            tabTestata.DeleteAllRows();
            document.NewPage();
            if (str1.ToString() != "")
            {
              directContent.BeginText();
              directContent.SetFontAndSize(font3, 11f);
              directContent.ShowTextAligned(1, "DOCUMENTO ANNULLATO", 300f, 575f, 0.0f);
              directContent.EndText();
            }
            this.ScriviIntestazioneDipa(ref directContent, ref document, int32_2, RAGSOC, TEL1, EMAIL, ABB, int32_3, int32_4);
            this.ScriviPieDiPaginaDipa(ref directContent);
            this.ScriviTestataDipa(ref tabTestata, ref Cell, ref fontRigaTest);
            ++cntNumPag;
            num2 = index2;
            num1 = 1;
            this.scriviNumPagDipa(ref directContent, cntNumPag, TotPag);
          }
          string str2 = dataTable6.Rows[index2]["DAL"].ToString().Trim().Substring(0, 5);
          string str3 = dataTable6.Rows[index2]["AL"].ToString().Trim().Substring(0, 5);
          string str4 = Convert.ToDecimal(dataTable6.Rows[index2]["IMPRET"]).ToString("#,##0.#0");
          string str5 = Convert.ToDecimal(dataTable6.Rows[index2]["IMPOCC"]).ToString("#,##0.#0");
          string str6 = Convert.ToDecimal(dataTable6.Rows[index2]["IMPFIG"]).ToString("#,##0.#0");
          Cell = new Cell((IElement) new Phrase(dataTable6.Rows[index2]["MAT"].ToString().Trim(), font4));
          Cell.VerticalAlignment = 5;
          Cell.HorizontalAlignment = 0;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 2 + index2 - num2, 0);
          Cell = new Cell((IElement) new Phrase(dataTable6.Rows[index2]["NOME"].ToString().Trim(), font4));
          Cell.HorizontalAlignment = 0;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 2 + index2 - num2, 1);
          Cell = !(dataTable6.Rows[index2]["CODQUACON"].ToString() == "1") ? new Cell((IElement) new Phrase("I", font4)) : new Cell((IElement) new Phrase("D", font4));
          Cell.HorizontalAlignment = 1;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 2 + index2 - num2, 2);
          Cell = new Cell((IElement) new Phrase(str2, font4));
          Cell.HorizontalAlignment = 1;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 2 + index2 - num2, 3);
          Cell = new Cell((IElement) new Phrase(str3, font4));
          Cell.HorizontalAlignment = 1;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 2 + index2 - num2, 4);
          Cell = new Cell((IElement) new Phrase(dataTable6.Rows[index2]["LIVELLO"].ToString().Trim(), font4));
          Cell.HorizontalAlignment = 1;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 2 + index2 - num2, 5);
          Cell = new Cell((IElement) new Phrase(dataTable6.Rows[index2]["CODCON"].ToString().Trim(), font4));
          Cell.HorizontalAlignment = 1;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 2 + index2 - num2, 6);
          Cell = new Cell((IElement) new Phrase(dataTable6.Rows[index2]["PERPAR"].ToString().Trim(), font4));
          Cell.HorizontalAlignment = 1;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 2 + index2 - num2, 7);
          Cell = new Cell((IElement) new Phrase("€", font4));
          Cell.HorizontalAlignment = 2;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 2 + index2 - num2, 8);
          Cell = new Cell((IElement) new Phrase(str4, font4));
          Cell.HorizontalAlignment = 2;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 2 + index2 - num2, 9);
          Cell = new Cell((IElement) new Phrase("€", font4));
          Cell.HorizontalAlignment = 2;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 2 + index2 - num2, 10);
          Cell = new Cell((IElement) new Phrase(str5, font4));
          Cell.HorizontalAlignment = 2;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 2 + index2 - num2, 11);
          Cell = new Cell((IElement) new Phrase("€", font4));
          Cell.HorizontalAlignment = 2;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 2 + index2 - num2, 12);
          Cell = new Cell((IElement) new Phrase(str6, font4));
          Cell.HorizontalAlignment = 2;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 2 + index2 - num2, 13);
          Cell = new Cell((IElement) new Phrase(dataTable6.Rows[index2]["ALIQUOTA"].ToString(), font4));
          Cell.HorizontalAlignment = 1;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 2 + index2 - num2, 14);
          Cell = !(dataTable6.Rows[index2]["IMPABB"].ToString() != "0") ? new Cell((IElement) new Phrase("N", font4)) : new Cell((IElement) new Phrase("S", font4));
          Cell.HorizontalAlignment = 1;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 2 + index2 - num2, 15);
          Cell = !(dataTable6.Rows[index2]["IMPASSCON"].ToString() != "0") ? new Cell((IElement) new Phrase("N", font4)) : new Cell((IElement) new Phrase("S", font4));
          Cell.HorizontalAlignment = 1;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 2 + index2 - num2, 16);
          ++num1;
        }
        tabTestata.Offset = 140f;
        document.Add((IElement) tabTestata);
        if (NUMMOVANN == "")
        {
          if (num1 > 7)
          {
            document.NewPage();
            this.ScriviIntestazioneDipa(ref directContent, ref document, int32_2, RAGSOC, TEL1, EMAIL, ABB, int32_3, int32_4);
            this.ScriviPieDiPaginaDipa(ref directContent);
            this.ScriviTestataDipa(ref tabTestata, ref Cell, ref fontRigaTest);
            this.scriviNumPagDipa(ref directContent, cntNumPag + 1, TotPag);
            this.ScriviTotaliDipa(ref document, ref tabFooter, ref tabFooter2, int32_3, int32_4, int32_1, int32_2, DATSANANN, NUMMOVANN, ref tabTotali);
            tabFooter.Offset = 160f;
            document.Add((IElement) tabFooter);
            tabFooter2.Offset = 20f;
            document.Add((IElement) tabFooter2);
          }
          else
          {
            this.ScriviTotaliDipa(ref document, ref tabFooter, ref tabFooter2, int32_3, int32_4, int32_1, int32_2, DATSANANN, NUMMOVANN, ref tabTotali);
            tabFooter.Offset = 20f;
            document.Add((IElement) tabFooter);
            tabFooter2.Offset = 20f;
            document.Add((IElement) tabFooter2);
          }
        }
        else if (num1 > 14)
        {
          document.NewPage();
          directContent.BeginText();
          directContent.SetFontAndSize(font3, 11f);
          directContent.ShowTextAligned(1, "DOCUMENTO ANNULLATO", 300f, 575f, 0.0f);
          directContent.EndText();
          this.ScriviIntestazioneDipa(ref directContent, ref document, int32_2, RAGSOC, TEL1, EMAIL, ABB, int32_3, int32_4);
          this.ScriviPieDiPaginaDipa(ref directContent);
          this.ScriviTestataDipa(ref tabTestata, ref Cell, ref fontRigaTest);
          this.scriviNumPagDipa(ref directContent, cntNumPag + 1, TotPag);
          this.ScriviTotaliDipa(ref document, ref tabFooter, ref tabFooter2, int32_3, int32_4, int32_1, int32_2, DATSANANN, NUMMOVANN, ref tabTotali);
          tabTotali.Offset = 160f;
          document.Add((IElement) tabTotali);
        }
        else
        {
          this.ScriviTotaliDipa(ref document, ref tabFooter, ref tabFooter2, int32_3, int32_4, int32_1, int32_2, DATSANANN, NUMMOVANN, ref tabTotali);
          tabTotali.Offset = 20f;
          document.Add((IElement) tabTotali);
        }
      }
      document.Close();
      instance.Close();
      Process.Start(strPath);
    }

    private void ScriviIntestazioneDipa(
      ref PdfContentByte cb,
      ref Document document,
      int CODPOS,
      string RAGSOC,
      string TEL1,
      string EMAIL,
      string ABB,
      int ANNDEN,
      int MESDEN)
    {
      string text = "";
      switch (MESDEN)
      {
        case 1:
          text = "Gennaio";
          break;
        case 2:
          text = "Febbraio";
          break;
        case 3:
          text = "Marzo";
          break;
        case 4:
          text = "Aprile";
          break;
        case 5:
          text = "Maggio";
          break;
        case 6:
          text = "Giugno";
          break;
        case 7:
          text = "Luglio";
          break;
        case 8:
          text = "Agosto";
          break;
        case 9:
          text = "Settembre";
          break;
        case 10:
          text = "Ottobre";
          break;
        case 11:
          text = "Novembre";
          break;
        case 12:
          text = "Dicembre";
          break;
      }
      Graphic graphic = new Graphic();
      graphic.Rectangle(15f, 562f, 90f, 13f);
      graphic.Stroke();
      document.Add((IElement) graphic);
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      BaseFont font2 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
      BaseFont font3 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAI"), "Cp1252", false);
      cb.BeginText();
      cb.SetFontAndSize(font2, 9f);
      cb.ShowTextAligned(0, "Mod. DIPA/01-3", 17f, 565f, 0.0f);
      cb.EndText();
      Image instance = Image.GetInstance(this.EstraiFilePGM_Byte("CNT_LOGO"));
      instance.ScaleAbsolute(103f, 51f);
      instance.SetAbsolutePosition(110f, 530f);
      document.Add((IElement) instance);
      cb.BeginText();
      cb.SetFontAndSize(font3, 9f);
      cb.ShowTextAligned(0, "Alla", 510f, 565f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 9f);
      cb.ShowTextAligned(0, "Fondazione ENPAIA - Ente Nazionale di Previdenza", 530f, 565f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 9f);
      cb.ShowTextAligned(0, "per gli Addetti e per gli Impiegati in Agricoltura", 530f, 555f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(0, "Viale Beethoven, 48 - 00144 Roma", 530f, 540f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(0, "Tel. 06/54581 - Call Center 800.010270 - Fax 06/5914444", 530f, 530f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(0, "Internet: www.enpaia.it                  Email: info@enpaia.it", 530f, 520f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(0, "Denuncia e pagamento del mese di __________________ Anno _________", 17f, 495f, 0.0f);
      cb.ShowTextAligned(0, text, 210f, 497f, 0.0f);
      cb.ShowTextAligned(0, Convert.ToString(ANNDEN), 325f, 497f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(0, "Denominazione _____________________________________________________________________", 52f, 460f, 0.0f);
      cb.ShowTextAligned(0, RAGSOC, 135f, 462f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(0, "Posizione assicurativa ENPAIA ______________", 580f, 460f, 0.0f);
      cb.ShowTextAligned(0, Convert.ToString(CODPOS), 730f, 462f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(0, "Telefono __________________________________ E-mail __________________________________", 52f, 445f, 0.0f);
      cb.ShowTextAligned(0, TEL1, 105f, 447f, 0.0f);
      cb.ShowTextAligned(0, EMAIL, 330f, 447f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(0, "Abbonamento P.A. Azienda", 550f, 445f, 0.0f);
      if (ABB == "S")
        cb.ShowTextAligned(0, "X", 693f, 445f, 0.0f);
      cb.EndText();
      graphic.Rectangle(690f, 442f, 13f, 13f);
      graphic.Stroke();
      document.Add((IElement) graphic);
      graphic.Rectangle(15f, 438f, 810f, 40f);
      graphic.Stroke();
      document.Add((IElement) graphic);
    }

    private void ScriviPieDiPaginaDipa(ref PdfContentByte cb) => BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);

    private void ScriviTestataDipa(ref iTextSharp.text.Table tabTestata, ref Cell Cell, ref iTextSharp.text.Font fontRigaTest)
    {
      Cell = new Cell((IElement) new Phrase("Matricola", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 0;
      Cell.BorderWidth = 0.0f;
      tabTestata.AddCell(Cell, 0, 0);
      Cell = new Cell((IElement) new Phrase("Cognome e Nome", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 0;
      Cell.BorderWidth = 0.0f;
      tabTestata.AddCell(Cell, 0, 1);
      Cell = new Cell((IElement) new Phrase("Qualifica", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      tabTestata.AddCell(Cell, 0, 2);
      Cell = new Cell((IElement) new Phrase("Data Variazione", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 3);
      Cell = new Cell((IElement) new Phrase("Livello", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      tabTestata.AddCell(Cell, 0, 5);
      Cell = new Cell((IElement) new Phrase("Cod. Contratto", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      tabTestata.AddCell(Cell, 0, 6);
      Cell = new Cell((IElement) new Phrase("Prest. %", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      tabTestata.AddCell(Cell, 0, 7);
      Cell = new Cell((IElement) new Phrase("Retribuzione Imponibile", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 2;
      Cell.BorderWidth = 0.0f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 8);
      Cell = new Cell((IElement) new Phrase("di cui Occasionali", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 2;
      Cell.BorderWidth = 0.0f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 10);
      Cell = new Cell((IElement) new Phrase("Figurativa", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 2;
      Cell.BorderWidth = 0.0f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 12);
      Cell = new Cell((IElement) new Phrase("Aliquota", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      tabTestata.AddCell(Cell, 0, 14);
      Cell = new Cell((IElement) new Phrase("Abb. P.A.", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      tabTestata.AddCell(Cell, 0, 15);
      Cell = new Cell((IElement) new Phrase("Ass. Contr.", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      tabTestata.AddCell(Cell, 0, 16);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 0);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 1);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 2);
      Cell = new Cell((IElement) new Phrase("(Dal)", fontRigaTest));
      Cell.VerticalAlignment = 4;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 3);
      Cell = new Cell((IElement) new Phrase("(Al)", fontRigaTest));
      Cell.VerticalAlignment = 4;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 4);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 5);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 6);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 7);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 8);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 9);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 10);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 11);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 12);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 13);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 14);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 15);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 16);
    }

    private void scriviNumPagDipa(ref PdfContentByte cb, int cntNumPag, int TotPag)
    {
      string text = "Pag. " + cntNumPag.ToString() + " di " + TotPag.ToString();
      cb.BeginText();
      BaseFont font = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.SetFontAndSize(font, 7.25f);
      cb.ShowTextAligned(1, text, 800f, 10f, 0.0f);
      cb.EndText();
    }

    private void ScriviTotaliDipa(
      ref Document document,
      ref iTextSharp.text.Table tabFooter,
      ref iTextSharp.text.Table tabFooter2,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      int CODPOS,
      string DATSANANN,
      string NUMMOVANN,
      ref iTextSharp.text.Table tabTotali)
    {
      Decimal num1 = 0M;
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      string str1 = "";
      DataLayer dataLayer = new DataLayer();
      string str2 = !(NUMMOVANN == "") ? "Addizionale annullato" : "Addizionale";
      string strSQL1 = "SELECT DENTES.SANSOTSOG, DENTES.IMPCON, DENTES.DATVER, DENTES.IMPVER, DENTES.UFFPOS, DENTES.CITDIC, DENTES.PRODIC," + " DENTES.IBAN, DENTES.ABIDIC, DENTES.CABDIC, VALUE(DENTES.IMPADDREC, 0.0) AS IMPADDREC, VALUE(DENTES.IMPASSCON, 0.0) AS IMPASSCONAZI," + " DENTES.IMPABB AS IMPABBAZI, DENTES.IMPDEC, DENTES.ANNDEN, VALUE(DENTES.IMPSANDET, 0.0) AS IMPSANDET, DENTES.CODMODPAG," + " DENTES.CODCAUSAN" + " FROM  DENTES " + " WHERE ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND CODPOS = " + CODPOS.ToString();
      DataTable dataTable3 = dataLayer.GetDataTable(strSQL1);
      string str3 = dataTable3.Rows[0]["CODCAUSAN"]?.ToString() ?? "";
      Decimal num2 = Convert.ToDecimal(dataTable3.Rows[0]["IMPDEC"]);
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      iTextSharp.text.Font font2 = new iTextSharp.text.Font(font1, 7.75f, 1);
      iTextSharp.text.Font font3 = new iTextSharp.text.Font(font1, 7.75f, 4);
      iTextSharp.text.Font font4 = new iTextSharp.text.Font(font1, 6.75f, 0);
      if (NUMMOVANN == "")
      {
        iTextSharp.text.Table table = new iTextSharp.text.Table(1, 3);
        table.WidthPercentage = 100f;
        table.BorderWidth = 0.0f;
        table.Cellspacing = 0.0f;
        table.Cellpadding = 1f;
        table.AutoFillEmptyCells = true;
        iTextSharp.text.Font font5 = new iTextSharp.text.Font(font1, 7.75f, 1, new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue));
        iTextSharp.text.Font font6 = new iTextSharp.text.Font(font1, 7.75f, 1);
        Cell aCell1 = new Cell((IElement) new Phrase("Il/La sottoscritto/a ________________________________________________________", font2));
        aCell1.BorderWidth = 0.0f;
        table.AddCell(aCell1, 0, 0);
        iTextSharp.text.Font font7 = new iTextSharp.text.Font(font1, 7.75f, 0);
        Cell aCell2 = new Cell((IElement) new Phrase("dichiara ai sensi della legge n.15 del 31 gennaio 1968 e successive modifiche ed integrazioni," + " sotto la propria responsabilità civile e penale, che le informazioni e i dati contenuti nel presente modulo" + " sono rispondenti al vero e si impegna a comunicare entro 30 giorni qualsiasi variazione riguardante" + " le situazioni dichiarate.", font7));
        aCell2.BorderWidth = 0.0f;
        table.AddCell(aCell2, 1, 0);
        Cell aCell3 = new Cell((IElement) new Phrase("      Data ____/____/______     Timbro della Ditta e Firma __________________________", font7));
        aCell3.BorderWidth = 0.0f;
        table.AddCell(aCell3, 2, 0);
        int[] widths1 = new int[7]{ 5, 2, 53, 2, 25, 6, 7 };
        tabFooter.AutoFillEmptyCells = true;
        tabFooter.WidthPercentage = 100f;
        tabFooter.Cellpadding = 2f;
        tabFooter.Cellspacing = 0.0f;
        tabFooter.SetWidths(widths1);
        tabFooter.Offset = 20f;
        tabFooter.BorderWidth = 0.0f;
        Cell aCell4 = new Cell("");
        aCell4.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell4, 0, 0);
        Cell aCell5 = new Cell("");
        aCell5.GrayFill = 0.3f;
        aCell5.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell5, 0, 1);
        Cell aCell6 = new Cell((IElement) new Phrase("Dichiarazione di responsabilità", font5));
        aCell6.HorizontalAlignment = 1;
        aCell6.GrayFill = 0.3f;
        aCell6.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell6, 0, 2);
        Cell aCell7 = new Cell("");
        aCell7.GrayFill = 0.3f;
        aCell7.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell7, 0, 3);
        Cell aCell8 = new Cell("");
        aCell8.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell8, 0, 4);
        Cell aCell9 = new Cell("");
        aCell9.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell9, 0, 5);
        Cell aCell10 = new Cell("");
        aCell10.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell10, 0, 6);
        Cell aCell11 = new Cell("");
        aCell11.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell11, 1, 0);
        Cell aCell12 = new Cell("");
        aCell12.GrayFill = 0.3f;
        aCell12.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell12, 1, 1);
        Cell aCell13 = new Cell((IElement) table);
        aCell13.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell13, 1, 2);
        Cell aCell14 = new Cell("");
        aCell14.GrayFill = 0.3f;
        aCell14.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell14, 1, 3);
        if (str3 == "")
          str1 = "Sanzioni";
        else if (DATSANANN != "")
        {
          str1 = "Sanzioni";
        }
        else
        {
          string strSQL2 = "SELECT VALUE(TASSO, 0.00) AS TASSO, TIPMOV, DESCAUREP AS DESC FROM TIPMOVCAU " + " WHERE CODCAU ='" + str3 + "' AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
          dataTable1.Clear();
          DataTable dataTable4 = dataLayer.GetDataTable(strSQL2);
          if (dataTable4.Rows.Count > 0)
          {
            for (int index = 0; index <= dataTable4.Rows.Count - 1; ++index)
            {
              string str4 = dataTable4.Rows[index]["TIPMOV"].ToString().Trim();
              if (!(str4 == "SAN_MD"))
              {
                if (str4 == "SAN_RD")
                  str1 = !(dataTable4.Rows[index]["DESC"].ToString() != "") ? "" : dataTable4.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable4.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable4.Rows[index]["DESC"].ToString().Length - 1);
              }
              else
                str1 = !(dataTable4.Rows[index]["DESC"].ToString() != "") ? "" : dataTable4.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable4.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable4.Rows[index]["DESC"].ToString().Length - 1);
            }
          }
          else
            str1 = "";
        }
        Cell aCell15 = new Cell((IElement) new Paragraph(new Phrase("Totale generale CTR", font7)));
        aCell15.Add((object) new Paragraph(new Phrase(str2, font7)));
        aCell15.Add((object) new Paragraph(new Phrase("Assistenza Contrattuale", font7)));
        aCell15.Add((object) new Paragraph(new Phrase("Abbonamenti P.A.", font7)));
        aCell15.Add((object) new Paragraph(new Phrase(str1, font7)));
        aCell15.Add((object) new Paragraph(new Phrase("Importo decurtato", font7)));
        aCell15.Add((object) new Paragraph(new Phrase("TOTALE dovuto", font6)));
        aCell15.Add((object) new Paragraph(new Phrase("TOTALE da pagare", font6)));
        aCell15.HorizontalAlignment = 2;
        aCell15.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell15, 1, 4);
        font4 = new iTextSharp.text.Font(font1, 6.75f, 0, new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue));
        Cell aCell16 = new Cell((IElement) new Paragraph(new Phrase("€", font7)));
        aCell16.Add((object) new Paragraph(new Phrase("€", font7)));
        aCell16.Add((object) new Paragraph(new Phrase("€", font7)));
        aCell16.Add((object) new Paragraph(new Phrase("€", font7)));
        aCell16.Add((object) new Paragraph(new Phrase("€", font7)));
        aCell16.Add((object) new Paragraph(new Phrase("€", font7)));
        aCell16.Add((object) new Paragraph(new Phrase("€", font6)));
        aCell16.Add((object) new Paragraph(new Phrase("€", font6)));
        aCell16.HorizontalAlignment = 2;
        aCell16.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell16, 1, 5);
        Cell aCell17;
        Decimal num3;
        if (dataTable3.Rows.Count > 0)
        {
          Decimal num4 = num1 + Convert.ToDecimal(dataTable3.Rows[0]["IMPCON"]) + Convert.ToDecimal(dataTable3.Rows[0]["IMPADDREC"]) + Convert.ToDecimal(dataTable3.Rows[0]["IMPASSCONAZI"]) + Convert.ToDecimal(dataTable3.Rows[0]["IMPABBAZI"]);
          if (num2 > 0M)
            num4 -= num2;
          Decimal num5 = !(dataTable3.Rows[0]["SANSOTSOG"].ToString().Trim() == "S") ? (!(DATSANANN.ToString() == "") ? num4 + 0M : num4 + Convert.ToDecimal(dataTable3.Rows[0]["IMPSANDET"])) : num4 + 0M;
          string str5 = !(DATSANANN.ToString() == "") ? "0,00" : Convert.ToDecimal(dataTable3.Rows[0]["IMPSANDET"]).ToString("#,##0.#0");
          aCell17 = new Cell((IElement) new Paragraph(new Phrase(Convert.ToDecimal(dataTable3.Rows[0]["IMPCON"]).ToString("#,##0.#0"), font7)));
          Cell cell1 = aCell17;
          num3 = Convert.ToDecimal(dataTable3.Rows[0]["IMPADDREC"]);
          Paragraph o1 = new Paragraph(new Phrase(num3.ToString("#,##0.#0"), font7));
          cell1.Add((object) o1);
          Cell cell2 = aCell17;
          num3 = Convert.ToDecimal(dataTable3.Rows[0]["IMPASSCONAZI"]);
          Paragraph o2 = new Paragraph(new Phrase(num3.ToString("#,##0.#0"), font7));
          cell2.Add((object) o2);
          Cell cell3 = aCell17;
          num3 = Convert.ToDecimal(dataTable3.Rows[0]["IMPABBAZI"]);
          Paragraph o3 = new Paragraph(new Phrase(num3.ToString("#,##0.#0"), font7));
          cell3.Add((object) o3);
          if (dataTable3.Rows[0]["SANSOTSOG"].ToString().Trim() == "S")
            aCell17.Add((object) new Paragraph(new Phrase("0,00", font7)));
          else
            aCell17.Add((object) new Paragraph(new Phrase(str5, font7)));
          aCell17.Add((object) new Paragraph(new Phrase(num2.ToString("#,##0.#0"), font7)));
          Decimal num6 = num5 + num2;
          aCell17.Add((object) new Paragraph(new Phrase(num6.ToString("#,##0.#0"), font6)));
          aCell17.Add((object) new Paragraph(new Phrase(num5.ToString("#,##0.#0"), font6)));
        }
        else
        {
          aCell17 = new Cell((IElement) new Paragraph(new Phrase("0,00", font7)));
          aCell17.Add((object) new Paragraph(new Phrase("0,00", font7)));
          aCell17.Add((object) new Paragraph(new Phrase("0,00", font7)));
          aCell17.Add((object) new Paragraph(new Phrase("0,00", font7)));
          aCell17.Add((object) new Paragraph(new Phrase("0,00", font7)));
          aCell17.Add((object) new Paragraph(new Phrase("0,00", font7)));
          aCell17.Add((object) new Paragraph(new Phrase("0,00", font7)));
          aCell17.Add((object) new Paragraph(new Phrase("0,00", font7)));
        }
        aCell17.HorizontalAlignment = 2;
        aCell17.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell17, 1, 6);
        Cell aCell18 = new Cell("");
        aCell18.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell18, 2, 0);
        Cell aCell19 = new Cell("");
        aCell19.GrayFill = 0.3f;
        aCell19.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell19, 2, 1);
        Cell aCell20 = new Cell("");
        aCell20.GrayFill = 0.3f;
        aCell20.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell20, 2, 2);
        Cell aCell21 = new Cell("");
        aCell21.GrayFill = 0.3f;
        aCell21.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell21, 2, 3);
        Cell aCell22 = new Cell("");
        aCell22.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell22, 2, 4);
        Cell aCell23 = new Cell("");
        aCell23.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell23, 2, 5);
        Cell aCell24 = new Cell("");
        aCell24.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell24, 2, 6);
        Cell aCell25 = new Cell("");
        aCell25.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell25, 3, 0);
        Cell aCell26 = new Cell("");
        aCell26.GrayFill = 0.3f;
        aCell26.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell26, 3, 1);
        Cell aCell27 = new Cell("");
        aCell27.GrayFill = 0.3f;
        aCell27.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell27, 3, 2);
        Cell aCell28 = new Cell("");
        aCell28.GrayFill = 0.3f;
        aCell28.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell28, 3, 3);
        Cell aCell29 = new Cell("");
        aCell29.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell29, 3, 4);
        Cell aCell30 = new Cell("");
        aCell30.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell30, 3, 5);
        Cell aCell31 = new Cell("");
        aCell31.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell31, 3, 6);
        int[] widths2 = new int[2]{ 20, 80 };
        tabFooter2.AutoFillEmptyCells = true;
        tabFooter2.WidthPercentage = 100f;
        tabFooter2.Cellpadding = 2f;
        tabFooter2.Cellspacing = 0.0f;
        tabFooter2.BorderWidth = 0.0f;
        tabFooter2.SetWidths(widths2);
        Cell aCell32 = new Cell((IElement) new Phrase("Riferimenti del versamento", font3));
        aCell32.BorderWidth = 0.0f;
        tabFooter2.AddCell(aCell32, 0, 0);
        Cell aCell33;
        if (dataTable3.Rows[0]["DATVER"].ToString() != "")
        {
          string[] strArray = new string[8]
          {
            "data operazione:  ",
            dataTable3.Rows[0]["DATVER"].ToString().Substring(0, 2),
            "-",
            dataTable3.Rows[0]["DATVER"].ToString().Substring(3, 2),
            "-",
            dataTable3.Rows[0]["DATVER"].ToString().Substring(6, 4),
            "        Importo del versamento:  € ",
            null
          };
          num3 = Convert.ToDecimal(dataTable3.Rows[0]["IMPVER"]);
          strArray[7] = num3.ToString("#,##0.#0");
          aCell33 = new Cell((IElement) new Phrase(string.Concat(strArray), font7));
        }
        else
          aCell33 = new Cell((IElement) new Phrase("data operazione:  ___ ___ ______  Importo del versamento:  ________________", font7));
        aCell33.BorderWidth = 0.0f;
        tabFooter2.AddCell(aCell33, 0, 1);
        Cell aCell34 = !(dataTable3.Rows[0]["CODMODPAG"].ToString() == "2") ? new Cell((IElement) new Phrase("", font7)) : new Cell((IElement) new Phrase("[X]  C/C Postale", font7));
        aCell34.BorderWidth = 0.0f;
        tabFooter2.AddCell(aCell34, 1, 0);
        Cell aCell35;
        if (dataTable3.Rows[0]["CODMODPAG"].ToString() == "2")
          aCell35 = new Cell((IElement) new Phrase("Ufficio Postale:  " + (dataTable3.Rows[0]["UFFPOS"].ToString() + new string(' ', 51)).Substring(0, 51) + "  Città:  " + (dataTable3.Rows[0]["CITDIC"].ToString() + new string(' ', 35)).Substring(0, 35) + " Prov.: " + (dataTable3.Rows[0]["PRODIC"].ToString() + new string(' ', 5)).Substring(0, 5), font7));
        else
          aCell35 = new Cell((IElement) new Phrase("", font7));
        aCell35.BorderWidth = 0.0f;
        tabFooter2.AddCell(aCell35, 1, 1);
        Cell aCell36 = !(dataTable3.Rows[0]["CODMODPAG"].ToString() == "3") ? new Cell((IElement) new Phrase("[  ]  Bonifico Bancario", font7)) : new Cell((IElement) new Phrase("[X]  Bonifico Bancario", font7));
        aCell36.BorderWidth = 0.0f;
        tabFooter2.AddCell(aCell36, 2, 0);
        Cell aCell37 = Convert.ToInt32(dataTable3.Rows[0][nameof (ANNDEN)]) <= 2007 ? (!(dataTable3.Rows[0]["CODMODPAG"].ToString() == "3") ? new Cell((IElement) new Phrase("IBAN Azienda:  ___________________________________", font7)) : new Cell((IElement) new Phrase("IBAN Azienda:  " + dataTable3.Rows[0]["IBAN"].ToString().Trim(), font7))) : (!(dataTable3.Rows[0]["CODMODPAG"].ToString() == "3") ? new Cell((IElement) new Phrase("IBAN Azienda:  ___________________________________", font7)) : new Cell((IElement) new Phrase("IBAN Azienda:  " + dataTable3.Rows[0]["IBAN"].ToString().Trim(), font7)));
        aCell37.BorderWidth = 0.0f;
        tabFooter2.AddCell(aCell37, 2, 1);
        Cell aCell38 = !(dataTable3.Rows[0]["CODMODPAG"].ToString() == "4") ? new Cell((IElement) new Phrase("[  ]  Versamento compensato totalmente da credito precedente", font7)) : new Cell((IElement) new Phrase("[X]  Versamento compensato totalmente da credito precedente", font7));
        aCell38.BorderWidth = 0.0f;
        aCell38.Colspan = 2;
        tabFooter2.AddCell(aCell38, 3, 0);
        Cell aCell39 = !(dataTable3.Rows[0]["CODMODPAG"].ToString() == "6") ? new Cell((IElement) new Phrase("[  ]  Versamento differito (con applicazioni delle sanzioni previste)", font7)) : new Cell((IElement) new Phrase("[X]  Versamento differito (con applicazioni delle sanzioni previste)", font7));
        aCell39.BorderWidth = 0.0f;
        aCell39.Colspan = 2;
        tabFooter2.AddCell(aCell39, 4, 0);
        Cell aCell40 = !(dataTable3.Rows[0]["CODMODPAG"].ToString() == "5") ? new Cell((IElement) new Phrase("[  ]  Ritardato versamento per finanziamenti pubblici tardivamente erogati (delibera CdA n°38/98)", font7)) : new Cell((IElement) new Phrase("[X]  Ritardato versamento per finanziamenti pubblici tardivamente erogati (delibera CdA n°38/98)", font7));
        aCell40.BorderWidth = 0.0f;
        aCell40.Colspan = 2;
        tabFooter2.AddCell(aCell40, 5, 0);
      }
      else
      {
        iTextSharp.text.Font font8 = new iTextSharp.text.Font(font1, 7.75f, 1, new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue));
        iTextSharp.text.Font font9 = new iTextSharp.text.Font(font1, 7.75f, 1);
        int[] widths = new int[3]{ 87, 3, 10 };
        tabTotali.WidthPercentage = 100f;
        tabTotali.Cellpadding = 2f;
        tabTotali.Cellspacing = 0.0f;
        tabTotali.SetWidths(widths);
        tabTotali.Offset = 20f;
        tabTotali.BorderWidth = 0.0f;
        if (str3 == "")
          str1 = "Sanzioni annullate";
        else if (DATSANANN != "")
        {
          str1 = "Sanzioni annullate";
        }
        else
        {
          string strSQL3 = "SELECT VALUE(TASSO, 0.00) AS TASSO, TIPMOV, DESCAUREP AS DESC FROM TIPMOVCAU " + " WHERE CODCAU ='" + str3 + "' AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
          dataTable1.Clear();
          DataTable dataTable5 = dataLayer.GetDataTable(strSQL3);
          if (dataTable5.Rows.Count > 0)
          {
            string str6 = dataTable5.Rows[0]["TIPMOV"].ToString().Trim();
            if (!(str6 == "SAN_MD"))
            {
              if (str6 == "SAN_RD")
                str1 = !(dataTable5.Rows[0]["DESC"].ToString() != "") ? "" : dataTable5.Rows[0]["DESC"].ToString().Substring(0, 1) + dataTable5.Rows[0]["DESC"].ToString().ToLower().Substring(1, dataTable5.Rows[0]["DESC"].ToString().Length - 1) + " annullate";
            }
            else
              str1 = !(dataTable5.Rows[0]["DESC"].ToString() != "") ? "" : dataTable5.Rows[0]["DESC"].ToString().Substring(0, 1) + dataTable5.Rows[0]["DESC"].ToString().ToLower().Substring(1, dataTable5.Rows[0]["DESC"].ToString().Length - 1) + " annullate";
          }
          else
            str1 = "";
        }
        Cell aCell41 = new Cell((IElement) new Paragraph(new Phrase("Totale generale CTR annullato", font2)));
        aCell41.Add((object) new Paragraph(new Phrase(str2, font2)));
        aCell41.Add((object) new Paragraph(new Phrase("Assistenza Contrattuale annullata", font2)));
        aCell41.Add((object) new Paragraph(new Phrase("Abbonamenti P.A. annullati", font2)));
        aCell41.Add((object) new Paragraph(new Phrase(str1, font2)));
        aCell41.Add((object) new Paragraph(new Phrase("TOTALE dovuto annullato", font9)));
        aCell41.HorizontalAlignment = 2;
        aCell41.BorderWidth = 0.0f;
        tabTotali.AddCell(aCell41, 0, 0);
        font4 = new iTextSharp.text.Font(font1, 6.75f, 0, new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue));
        Cell aCell42 = new Cell((IElement) new Paragraph(new Phrase("€", font2)));
        aCell42.Add((object) new Paragraph(new Phrase("€", font2)));
        aCell42.Add((object) new Paragraph(new Phrase("€", font2)));
        aCell42.Add((object) new Paragraph(new Phrase("€", font2)));
        aCell42.Add((object) new Paragraph(new Phrase("€", font2)));
        aCell42.Add((object) new Paragraph(new Phrase("€", font9)));
        aCell42.HorizontalAlignment = 2;
        aCell42.BorderWidth = 0.0f;
        tabTotali.AddCell(aCell42, 0, 1);
        Cell aCell43;
        if (dataTable3.Rows.Count > 0)
        {
          Decimal num7 = num1 + Convert.ToDecimal(dataTable3.Rows[0]["IMPCON"]) + Convert.ToDecimal(dataTable3.Rows[0]["IMPADDREC"]) + Convert.ToDecimal(dataTable3.Rows[0]["IMPASSCONAZI"]) + Convert.ToDecimal(dataTable3.Rows[0]["IMPABBAZI"]);
          Decimal num8 = !(dataTable3.Rows[0]["SANSOTSOG"].ToString().Trim() == "S") ? (!(DATSANANN.ToString() == "") ? num7 + 0M : num7 + Convert.ToDecimal(dataTable3.Rows[0]["IMPSANDET"])) : num7 + 0M;
          string str7 = !(DATSANANN.ToString() == "") ? "0,00" : Convert.ToDecimal(dataTable3.Rows[0]["IMPSANDET"]).ToString("#,##0.#0");
          aCell43 = new Cell((IElement) new Paragraph(new Phrase(Convert.ToDecimal(dataTable3.Rows[0]["IMPCON"]).ToString("#,##0.#0"), font2)));
          Cell cell4 = aCell43;
          Decimal num9 = Convert.ToDecimal(dataTable3.Rows[0]["IMPADDREC"]);
          Paragraph o4 = new Paragraph(new Phrase(num9.ToString("#,##0.#0"), font2));
          cell4.Add((object) o4);
          Cell cell5 = aCell43;
          num9 = Convert.ToDecimal(dataTable3.Rows[0]["IMPASSCONAZI"]);
          Paragraph o5 = new Paragraph(new Phrase(num9.ToString("#,##0.#0"), font2));
          cell5.Add((object) o5);
          Cell cell6 = aCell43;
          num9 = Convert.ToDecimal(dataTable3.Rows[0]["IMPABBAZI"]);
          Paragraph o6 = new Paragraph(new Phrase(num9.ToString("#,##0.#0"), font2));
          cell6.Add((object) o6);
          if (dataTable3.Rows[0]["SANSOTSOG"].ToString().Trim() == "S")
            aCell43.Add((object) new Paragraph(new Phrase("0,00", font2)));
          else
            aCell43.Add((object) new Paragraph(new Phrase(str7, font2)));
          aCell43.Add((object) new Paragraph(new Phrase(num8.ToString("#,##0.#0"), font9)));
        }
        else
        {
          aCell43 = new Cell((IElement) new Paragraph(new Phrase("0,00", font2)));
          aCell43.Add((object) new Paragraph(new Phrase("0,00", font2)));
          aCell43.Add((object) new Paragraph(new Phrase("0,00", font2)));
          aCell43.Add((object) new Paragraph(new Phrase("0,00", font2)));
          aCell43.Add((object) new Paragraph(new Phrase("0,00", font2)));
          aCell43.Add((object) new Paragraph(new Phrase("0,00", font2)));
        }
        aCell43.HorizontalAlignment = 2;
        aCell43.BorderWidth = 0.0f;
        tabTotali.AddCell(aCell43, 0, 2);
      }
    }

    public void CreaStampaArretrati(ref DataTable dtArretrati, string strPath)
    {
      Document document = new Document(PageSize.A4.Rotate(), 15f, 15f, 15f, 15f);
      PdfWriter instance = PdfWriter.GetInstance(document, (Stream) new FileStream(strPath, FileMode.Create));
      int num1 = 1;
      int num2 = 0;
      DataTable dataTable1 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      int cntNumPag = 1;
      DataTable dataTable2 = new DataTable();
      string RAGSOC = "";
      string TEL1 = "";
      string EMAIL = "";
      string ABB = "";
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      BaseFont font2 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
      BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAI"), "Cp1252", false);
      document.Open();
      for (int index1 = 0; index1 <= dtArretrati.Rows.Count - 1; ++index1)
      {
        int int32_1 = Convert.ToInt32(dtArretrati.Rows[index1]["PRODEN"]);
        int int32_2 = Convert.ToInt32(dtArretrati.Rows[index1]["CODPOS"]);
        int int32_3 = Convert.ToInt32(dtArretrati.Rows[index1]["ANNDEN"]);
        int int32_4 = Convert.ToInt32(dtArretrati.Rows[index1]["MESDEN"]);
        if (index1 > 0)
        {
          cntNumPag = 1;
          num2 = 0;
          num1 = 1;
          document.NewPage();
        }
        string strSQL1 = "SELECT DATCONMOV FROM DENTES" + " WHERE ANNDEN = " + int32_3.ToString() + " AND MESDEN = " + int32_4.ToString() + " AND PRODEN = " + int32_1.ToString() + " AND CODPOS = " + int32_2.ToString();
        string strData = (!DBNull.Value.Equals(dataLayer.GetDataTable(strSQL1).Rows[0]["DATCONMOV"]) ? dataLayer.GetDataTable(strSQL1).Rows[0]["DATCONMOV"].ToString() : string.Empty) ?? "";
        DataTable dataTable3 = new DataTable();
        string strSQL2 = " SELECT AZI.RAGSOC, AZISTO.ABB, INDSED.TEL1, INDSED.EMAIL" + " FROM   AZI LEFT JOIN INDSED  ON  AZI.CODPOS =  INDSED.CODPOS" + " LEFT JOIN  AZISTO ON  AZI.CODPOS =  AZISTO.CODPOS" + " WHERE INDSED.TIPIND=1 " + " AND INDSED.DATINI <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + " AND AZI.CODPOS= " + int32_2.ToString() + " AND INDSED.DATCOM = (SELECT MAX(DATCOM) FROM INDSED WHERE " + " INDSED.CODPOS = " + int32_2.ToString() + " AND TIPIND = 1 AND INDSED.DATINI <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + ")" + " ORDER BY AZI.CODPOS, INDSED.DATCOM DESC FETCH FIRST 1 ROWS ONLY";
        DataTable dataTable4 = dataLayer.GetDataTable(strSQL2);
        if (dataTable4.Rows.Count > 0)
        {
          RAGSOC = dataTable4.Rows[0]["RAGSOC"].ToString().Trim();
          TEL1 = dataTable4.Rows[0]["TEL1"].ToString().Trim();
          EMAIL = dataTable4.Rows[0]["EMAIL"].ToString().Trim();
          ABB = dataTable4.Rows[0]["ABB"].ToString().Trim();
        }
        string strSQL3 = "SELECT DATCHI FROM DENTES" + " WHERE ANNDEN = " + int32_3.ToString() + " AND MESDEN = " + int32_4.ToString() + " AND PRODEN = " + int32_1.ToString() + " AND CODPOS = " + int32_2.ToString();
        string DATCHI = dataLayer.GetDataTable(strSQL3).Rows[0]["DATCHI"]?.ToString() ?? "";
        if (DATCHI != "")
          DATCHI = DATCHI.Substring(0, 10);
        PdfContentByte directContent = instance.DirectContent;
        BaseFont font3 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
        string strSQL4 = "SELECT NUMMOVANN, DATMOVANN, DATSANANN, NUMMOV FROM DENTES" + " WHERE ANNDEN = " + int32_3.ToString() + " AND MESDEN = " + int32_4.ToString() + " AND PRODEN = " + int32_1.ToString() + " AND CODPOS = " + int32_2.ToString();
        DataTable dataTable5 = dataLayer.GetDataTable(strSQL4);
        string str1 = dataTable5.Rows[0]["DATMOVANN"].ToString();
        string DATSANANN = dataTable5.Rows[0]["DATSANANN"].ToString();
        string NUMMOVANN = dataTable5.Rows[0]["NUMMOVANN"].ToString();
        string NUMMOV = dataTable5.Rows[0]["NUMMOV"].ToString();
        if (str1.ToString() != "")
        {
          directContent.BeginText();
          directContent.SetFontAndSize(font3, 11f);
          directContent.ShowTextAligned(1, "DOCUMENTO ANNULLATO", 300f, 575f, 0.0f);
          directContent.EndText();
        }
        this.ScriviIntestazioneArretrati(ref directContent, ref document, int32_2, RAGSOC, TEL1, EMAIL, ABB, int32_3, int32_4, DATCHI, NUMMOV);
        this.ScriviPieDiPaginaArretrati(ref directContent);
        iTextSharp.text.Table tabFooter = new iTextSharp.text.Table(7, 4);
        iTextSharp.text.Table tabFooter2 = new iTextSharp.text.Table(2, 6);
        iTextSharp.text.Table tabTotali = new iTextSharp.text.Table(3, 1);
        Cell Cell = new Cell();
        iTextSharp.text.Table tabTestata = new iTextSharp.text.Table(9, 1);
        tabTestata.AutoFillEmptyCells = true;
        tabTestata.WidthPercentage = 100f;
        tabTestata.BorderWidth = 0.0f;
        tabTestata.Cellspacing = 0.0f;
        tabTestata.Cellpadding = 2f;
        int[] widths = new int[9]
        {
          6,
          36,
          12,
          4,
          10,
          4,
          10,
          10,
          8
        };
        tabTestata.SetWidths(widths);
        iTextSharp.text.Font fontRigaTest = new iTextSharp.text.Font(font2, 6.75f, 0);
        iTextSharp.text.Font font4 = new iTextSharp.text.Font(font1, 7.75f, 0);
        this.ScriviTestataArretrati(ref tabTestata, ref Cell, ref fontRigaTest);
        string strSQL5 = "SELECT ISCT.MAT, TRIM(ISCT.COG) || ' ' || TRIM(ISCT.NOM) AS NOME, DENDET.IMPRET," + " DENDET.ANNDEN, DENDET.MESDEN, DENDET.CODCON, DENDET.IMPABB, DENDET.IMPASSCON, " + " DENDET.IMPOCC, DENDET.IMPFIG, DENDET.CODLIV, DENDET.CODQUACON, DENDET.DAL, " + " DENDET.AL, CONLIV.DENLIV AS LIVELLO," + " DENDET.ALIQUOTA, DENDET.ANNCOM" + " FROM ISCT RIGHT JOIN DENDET ON ISCT.MAT = DENDET.MAT" + " INNER JOIN CONLIV ON DENDET.CODCON=CONLIV.CODCON" + " AND DENDET.PROCON = CONLIV.PROCON" + " AND DENDET.CODLIV = CONLIV.CODLIV" + " WHERE DENDET.ANNDEN = " + int32_3.ToString() + " AND DENDET.MESDEN = " + int32_4.ToString() + " AND DENDET.PRODEN = " + int32_1.ToString() + " AND DENDET.CODPOS = " + int32_2.ToString() + " AND DENDET.TIPMOV = 'AR'" + " ORDER BY ISCT.COG, DENDET.MAT, DENDET.DAL";
        DataTable dataTable6 = dataLayer.GetDataTable(strSQL5);
        int TotPag = !((Decimal) (dataTable6.Rows.Count - dataTable6.Rows.Count / 21 * 21) <= 6M) ? dataTable6.Rows.Count / 21 + 2 : dataTable6.Rows.Count / 21 + 1;
        this.scriviNumPagArretrati(ref directContent, cntNumPag, TotPag);
        for (int index2 = 0; index2 <= dataTable6.Rows.Count - 1; ++index2)
        {
          if (num1 == 22)
          {
            tabTestata.Offset = 140f;
            document.Add((IElement) tabTestata);
            tabTestata.DeleteAllRows();
            document.NewPage();
            if (str1.ToString() != "")
            {
              directContent.BeginText();
              directContent.SetFontAndSize(font3, 11f);
              directContent.ShowTextAligned(1, "DOCUMENTO ANNULLATO", 300f, 575f, 0.0f);
              directContent.EndText();
            }
            this.ScriviIntestazioneArretrati(ref directContent, ref document, int32_2, RAGSOC, TEL1, EMAIL, ABB, int32_3, int32_4, DATCHI, NUMMOV);
            this.ScriviPieDiPaginaArretrati(ref directContent);
            this.ScriviTestataArretrati(ref tabTestata, ref Cell, ref fontRigaTest);
            ++cntNumPag;
            num2 = index2;
            num1 = 1;
            this.scriviNumPagArretrati(ref directContent, cntNumPag, TotPag);
          }
          dataTable6.Rows[index2]["DAL"].ToString().Substring(0, 5);
          dataTable6.Rows[index2]["AL"].ToString().Substring(0, 5);
          string str2 = Convert.ToDecimal(dataTable6.Rows[index2]["IMPRET"]).ToString("#,##0.#0");
          string str3 = Convert.ToDecimal(dataTable6.Rows[index2]["IMPOCC"]).ToString("#,##0.#0");
          Convert.ToDecimal(dataTable6.Rows[index2]["IMPFIG"]).ToString("#,##0.#0");
          Cell = new Cell((IElement) new Phrase(dataTable6.Rows[index2]["MAT"].ToString().Trim(), font4));
          Cell.VerticalAlignment = 5;
          Cell.HorizontalAlignment = 0;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 0);
          Cell = new Cell((IElement) new Phrase(dataTable6.Rows[index2]["NOME"].ToString().Trim(), font4));
          Cell.HorizontalAlignment = 0;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 1);
          Cell = new Cell((IElement) new Phrase(dataTable6.Rows[index2]["ANNCOM"].ToString().Trim(), font4));
          Cell.HorizontalAlignment = 1;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 2);
          Cell = new Cell((IElement) new Phrase("€", font4));
          Cell.HorizontalAlignment = 2;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 3);
          Cell = new Cell((IElement) new Phrase(str2, font4));
          Cell.HorizontalAlignment = 2;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 4);
          Cell = new Cell((IElement) new Phrase("€", font4));
          Cell.HorizontalAlignment = 2;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 5);
          Cell = new Cell((IElement) new Phrase(str3, font4));
          Cell.HorizontalAlignment = 2;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 6);
          Cell = new Cell((IElement) new Phrase(dataTable6.Rows[index2]["ALIQUOTA"].ToString().Trim(), font4));
          Cell.HorizontalAlignment = 2;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 7);
          Cell = !(dataTable6.Rows[index2]["CODQUACON"].ToString() == "1") ? new Cell((IElement) new Phrase("I", font4)) : new Cell((IElement) new Phrase("D", font4));
          Cell.HorizontalAlignment = 1;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 8);
          ++num1;
        }
        tabTestata.Offset = 140f;
        document.Add((IElement) tabTestata);
        if (NUMMOVANN == "")
        {
          if (num1 > 7)
          {
            document.NewPage();
            this.ScriviIntestazioneArretrati(ref directContent, ref document, int32_2, RAGSOC, TEL1, EMAIL, ABB, int32_3, int32_4, DATCHI, NUMMOV);
            this.ScriviPieDiPaginaArretrati(ref directContent);
            this.ScriviTestataArretrati(ref tabTestata, ref Cell, ref fontRigaTest);
            this.scriviNumPagArretrati(ref directContent, cntNumPag + 1, TotPag);
            this.ScriviTotaliArretrati(ref document, ref tabFooter, ref tabFooter2, int32_3, int32_4, int32_1, int32_2, DATSANANN, NUMMOVANN, ref tabTotali);
            tabFooter.Offset = 160f;
            document.Add((IElement) tabFooter);
            tabFooter2.Offset = 20f;
            document.Add((IElement) tabFooter2);
          }
          else
          {
            this.ScriviTotaliArretrati(ref document, ref tabFooter, ref tabFooter2, int32_3, int32_4, int32_1, int32_2, DATSANANN, NUMMOVANN, ref tabTotali);
            tabFooter.Offset = 20f;
            document.Add((IElement) tabFooter);
            tabFooter2.Offset = 20f;
            document.Add((IElement) tabFooter2);
          }
        }
        else if (num1 > 14)
        {
          document.NewPage();
          directContent.BeginText();
          directContent.SetFontAndSize(font3, 11f);
          directContent.ShowTextAligned(1, "DOCUMENTO ANNULLATO", 300f, 575f, 0.0f);
          directContent.EndText();
          this.ScriviIntestazioneArretrati(ref directContent, ref document, int32_2, RAGSOC, TEL1, EMAIL, ABB, int32_3, int32_4, DATCHI, NUMMOV);
          this.ScriviPieDiPaginaArretrati(ref directContent);
          this.ScriviTestataArretrati(ref tabTestata, ref Cell, ref fontRigaTest);
          this.scriviNumPagArretrati(ref directContent, cntNumPag + 1, TotPag);
          this.ScriviTotaliArretrati(ref document, ref tabFooter, ref tabFooter2, int32_3, int32_4, int32_1, int32_2, DATSANANN, NUMMOVANN, ref tabTotali);
          tabTotali.Offset = 160f;
          document.Add((IElement) tabTotali);
        }
        else
        {
          this.ScriviTotaliArretrati(ref document, ref tabFooter, ref tabFooter2, int32_3, int32_4, int32_1, int32_2, DATSANANN, NUMMOVANN, ref tabTotali);
          tabTotali.Offset = 20f;
          document.Add((IElement) tabTotali);
        }
      }
      document.Close();
      instance.Close();
      Process.Start(strPath);
    }

    private void ScriviIntestazioneArretrati(
      ref PdfContentByte cb,
      ref Document document,
      int CODPOS,
      string RAGSOC,
      string TEL1,
      string EMAIL,
      string ABB,
      int ANNDEN,
      int MESDEN,
      string DATCHI,
      string NUMMOV)
    {
      string str = "";
      switch (MESDEN)
      {
        case 1:
          str = "Gennaio";
          break;
        case 2:
          str = "Febbraio";
          break;
        case 3:
          str = "Marzo";
          break;
        case 4:
          str = "Aprile";
          break;
        case 5:
          str = "Maggio";
          break;
        case 6:
          str = "Giugno";
          break;
        case 7:
          str = "Luglio";
          break;
        case 8:
          str = "Agosto";
          break;
        case 9:
          str = "Settembre";
          break;
        case 10:
          str = "Ottobre";
          break;
        case 11:
          str = "Novembre";
          break;
        case 12:
          str = "Dicembre";
          break;
      }
      Graphic graphic = new Graphic();
      graphic.Rectangle(15f, 562f, 90f, 13f);
      graphic.Stroke();
      document.Add((IElement) graphic);
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      BaseFont font2 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
      BaseFont font3 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAI"), "Cp1252", false);
      cb.BeginText();
      cb.SetFontAndSize(font2, 9f);
      cb.ShowTextAligned(0, "Mod. ARR/01", 17f, 565f, 0.0f);
      cb.EndText();
      Image instance = Image.GetInstance(this.EstraiFilePGM_Byte("CNT_LOGO"));
      instance.ScaleAbsolute(103f, 51f);
      instance.SetAbsolutePosition(110f, 530f);
      document.Add((IElement) instance);
      cb.BeginText();
      cb.SetFontAndSize(font3, 9f);
      cb.ShowTextAligned(0, "Alla", 510f, 565f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 9f);
      cb.ShowTextAligned(0, "Fondazione ENPAIA - Ente Nazionale di Previdenza", 530f, 565f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 9f);
      cb.ShowTextAligned(0, "per gli Addetti e per gli Impiegati in Agricoltura", 530f, 555f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(0, "Viale Beethoven, 48 - 00144 Roma", 530f, 540f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(0, "Tel. 06/54581 - Call Center 800.010270 - Fax 06/5914444", 530f, 530f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(0, "Internet: www.enpaia.it                  Email: info@enpaia.it", 530f, 520f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(0, "Denuncia arretrati n. " + NUMMOV, 17f, 495f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(0, "Denominazione _____________________________________________________________________", 52f, 460f, 0.0f);
      cb.ShowTextAligned(0, RAGSOC, 135f, 462f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(0, "Posizione assicurativa ENPAIA ______________", 580f, 460f, 0.0f);
      cb.ShowTextAligned(0, Convert.ToString(CODPOS), 730f, 462f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(0, "Telefono __________________________________ E-mail __________________________________", 52f, 445f, 0.0f);
      cb.ShowTextAligned(0, TEL1, 105f, 447f, 0.0f);
      cb.ShowTextAligned(0, EMAIL, 330f, 447f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(0, "Abbonamento P.A. Azienda", 550f, 445f, 0.0f);
      if (ABB == "S")
        cb.ShowTextAligned(0, "X", 693f, 445f, 0.0f);
      cb.EndText();
      graphic.Rectangle(690f, 442f, 13f, 13f);
      graphic.Stroke();
      document.Add((IElement) graphic);
      graphic.Rectangle(15f, 438f, 810f, 40f);
      graphic.Stroke();
      document.Add((IElement) graphic);
    }

    private void ScriviPieDiPaginaArretrati(ref PdfContentByte cb) => BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);

    public void ScriviTestataArretrati(ref iTextSharp.text.Table tabTestata, ref Cell Cell, ref iTextSharp.text.Font fontRigaTest)
    {
      Cell = new Cell((IElement) new Phrase("Matricola", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 0;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 0);
      Cell = new Cell((IElement) new Phrase("Cognome e Nome", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 0;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 1);
      Cell = new Cell((IElement) new Phrase("Anno Competenza", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 0;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 2);
      Cell = new Cell((IElement) new Phrase("Retribuzione Imponibile", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 2;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 3);
      Cell = new Cell((IElement) new Phrase("di cui Occasionali", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 2;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 5);
      Cell = new Cell((IElement) new Phrase("Aliquota %", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 2;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 7);
      Cell = new Cell((IElement) new Phrase("Qual.", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 8);
    }

    private void scriviNumPagArretrati(ref PdfContentByte cb, int cntNumPag, int TotPag)
    {
      string text = "Pag. " + cntNumPag.ToString() + " di " + TotPag.ToString();
      cb.BeginText();
      BaseFont font = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.SetFontAndSize(font, 7.25f);
      cb.ShowTextAligned(1, text, 800f, 10f, 0.0f);
      cb.EndText();
    }

    private void ScriviTotaliArretrati(
      ref Document document,
      ref iTextSharp.text.Table tabFooter,
      ref iTextSharp.text.Table tabFooter2,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      int CODPOS,
      string DATSANANN,
      string NUMMOVANN,
      ref iTextSharp.text.Table tabTotali)
    {
      Decimal num1 = 0M;
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      string str1 = "";
      string str2 = "";
      DataLayer dataLayer = new DataLayer();
      string str3 = !(NUMMOVANN == "") ? "Addizionale annullato" : "Addizionale";
      string strSQL1 = "SELECT DENTES.IMPCON, DENTES.DATVER, DENTES.IMPVER, DENTES.UFFPOS, DENTES.CITDIC, DENTES.PRODIC," + " DENTES.IBAN, DENTES.ABIDIC, DENTES.ANNDEN, DENTES.CABDIC, VALUE(DENTES.IMPADDREC, 0.0) AS IMPADDREC, VALUE(DENTES.IMPASSCON, 0.0) AS IMPASSCONAZI," + " DENTES.IMPABB AS IMPABBAZI, VALUE(DENTES.IMPSANDET, 0.0) AS IMPSANDET, DENTES.CODMODPAG, DENTES.SANSOTSOG," + " DENTES.CODCAUSAN" + " FROM  DENTES " + " WHERE ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND CODPOS = " + CODPOS.ToString();
      DataTable dataTable3 = dataLayer.GetDataTable(strSQL1);
      string str4 = dataTable3.Rows[0]["CODCAUSAN"]?.ToString() ?? "";
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      iTextSharp.text.Font font2 = new iTextSharp.text.Font(font1, 7.75f, 1);
      iTextSharp.text.Font font3 = new iTextSharp.text.Font(font1, 7.75f, 4);
      iTextSharp.text.Font font4 = new iTextSharp.text.Font(font1, 6.75f, 0);
      if (NUMMOVANN == "")
      {
        iTextSharp.text.Table table = new iTextSharp.text.Table(1, 3);
        table.WidthPercentage = 100f;
        table.BorderWidth = 0.0f;
        table.Cellspacing = 0.0f;
        table.Cellpadding = 1f;
        table.AutoFillEmptyCells = true;
        Cell aCell1 = new Cell((IElement) new Phrase("Il/La sottoscritto/a ________________________________________________________", font2));
        aCell1.BorderWidth = 0.0f;
        table.AddCell(aCell1, 0, 0);
        iTextSharp.text.Font font5 = new iTextSharp.text.Font(font1, 7.75f, 0);
        Cell aCell2 = new Cell((IElement) new Phrase("dichiara ai sensi della legge n.15 del 31 gennaio 1968 e successive modifiche ed integrazioni," + " sotto la propria responsabilità civile e penale, che le informazioni e i dati contenuti nel presente modulo" + " sono rispondenti al vero e si impegna a comunicare entro 30 giorni qualsiasi variazione riguardante" + " le situazioni dichiarate.", font5));
        aCell2.BorderWidth = 0.0f;
        table.AddCell(aCell2, 1, 0);
        Cell aCell3 = new Cell((IElement) new Phrase("      Data ____/____/______     Timbro della Ditta e Firma __________________________", font5));
        aCell3.BorderWidth = 0.0f;
        table.AddCell(aCell3, 2, 0);
        iTextSharp.text.Font font6 = new iTextSharp.text.Font(font1, 7.75f, 1, new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue));
        int[] widths1 = new int[7]{ 5, 2, 53, 2, 25, 6, 7 };
        tabFooter.AutoFillEmptyCells = true;
        tabFooter.WidthPercentage = 100f;
        tabFooter.Cellpadding = 2f;
        tabFooter.Cellspacing = 0.0f;
        tabFooter.SetWidths(widths1);
        tabFooter.Offset = 20f;
        tabFooter.BorderWidth = 0.0f;
        Cell aCell4 = new Cell("");
        aCell4.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell4, 0, 0);
        Cell aCell5 = new Cell("");
        aCell5.GrayFill = 0.3f;
        aCell5.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell5, 0, 1);
        Cell aCell6 = new Cell((IElement) new Phrase("Dichiarazione di responsabilità", font6));
        aCell6.HorizontalAlignment = 1;
        aCell6.GrayFill = 0.3f;
        aCell6.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell6, 0, 2);
        Cell aCell7 = new Cell("");
        aCell7.GrayFill = 0.3f;
        aCell7.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell7, 0, 3);
        Cell aCell8 = new Cell("");
        aCell8.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell8, 0, 4);
        Cell aCell9 = new Cell("");
        aCell9.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell9, 0, 5);
        Cell aCell10 = new Cell("");
        aCell10.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell10, 0, 6);
        Cell aCell11 = new Cell("");
        aCell11.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell11, 1, 0);
        Cell aCell12 = new Cell("");
        aCell12.GrayFill = 0.3f;
        aCell12.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell12, 1, 1);
        Cell aCell13 = new Cell((IElement) table);
        aCell13.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell13, 1, 2);
        Cell aCell14 = new Cell("");
        aCell14.GrayFill = 0.3f;
        aCell14.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell14, 1, 3);
        if (str4 == "")
          str1 = "Sanzioni";
        else if (DATSANANN != "")
        {
          str1 = "Sanzioni";
        }
        else
        {
          string strSQL2 = "SELECT VALUE(TASSO, 0.00) AS TASSO, TIPMOV, DESCAUREP AS DESC FROM TIPMOVCAU " + " WHERE CODCAU ='" + str4 + "' AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
          dataTable1.Clear();
          DataTable dataTable4 = dataLayer.GetDataTable(strSQL2);
          if (dataTable4.Rows.Count > 0)
          {
            for (int index = 0; index <= dataTable4.Rows.Count - 1; ++index)
            {
              string str5 = dataTable4.Rows[index]["TIPMOV"].ToString().Trim();
              if (!(str5 == "SAN_MD"))
              {
                if (str5 == "SAN_RD")
                {
                  if (dataTable4.Rows[index]["DESC"].ToString() != "")
                  {
                    str1 = dataTable4.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable4.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable4.Rows[index]["DESC"].ToString().Length - 1);
                    str2 = " al tasso del " + dataTable4.Rows[index]["TASSO"]?.ToString() + " % annuo";
                  }
                  else
                    str1 = "";
                }
              }
              else if (dataTable4.Rows[index]["DESC"].ToString() != "")
              {
                str1 = dataTable4.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable4.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable4.Rows[index]["DESC"].ToString().Length - 1);
                str2 = " al tasso del " + dataTable4.Rows[index]["TASSO"]?.ToString() + " % annuo";
              }
              else
                str1 = "";
            }
          }
          else
            str1 = "";
        }
        Cell aCell15 = new Cell((IElement) new Paragraph(new Phrase("Totale generale CTR", font5)));
        aCell15.Add((object) new Paragraph(new Phrase(str3, font5)));
        aCell15.Add((object) new Paragraph(new Phrase("Assistenza Contrattuale", font5)));
        aCell15.Add((object) new Paragraph(new Phrase("Abbonamenti P.A.", font5)));
        aCell15.Add((object) new Paragraph(new Phrase(str1, font5)));
        if (str2 != "")
          aCell15.Add((object) new Paragraph(new Phrase(str2, font5)));
        aCell15.Add((object) new Paragraph(new Phrase("TOTALE dovuto", font5)));
        aCell15.HorizontalAlignment = 2;
        aCell15.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell15, 1, 4);
        iTextSharp.text.Font font7 = new iTextSharp.text.Font(font1, 6.75f, 0, new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue));
        Cell aCell16 = new Cell((IElement) new Paragraph(new Phrase("€", font5)));
        aCell16.Add((object) new Paragraph(new Phrase("€", font5)));
        aCell16.Add((object) new Paragraph(new Phrase("€", font5)));
        aCell16.Add((object) new Paragraph(new Phrase("€", font5)));
        aCell16.Add((object) new Paragraph(new Phrase("€", font5)));
        if (str2 != "")
          aCell16.Add((object) new Paragraph(new Phrase("€", font5)));
        aCell16.Add((object) new Paragraph(new Phrase("€", font5)));
        aCell16.HorizontalAlignment = 2;
        aCell16.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell16, 1, 5);
        Cell aCell17;
        Decimal num2;
        if (dataTable3.Rows.Count > 0)
        {
          Decimal num3 = num1 + Convert.ToDecimal(dataTable3.Rows[0]["IMPCON"]) + Convert.ToDecimal(dataTable3.Rows[0]["IMPADDREC"]) + Convert.ToDecimal(dataTable3.Rows[0]["IMPASSCONAZI"]) + Convert.ToDecimal(dataTable3.Rows[0]["IMPABBAZI"]);
          Decimal num4 = !(dataTable3.Rows[0]["SANSOTSOG"].ToString().Trim() == "S") ? (!(DATSANANN.ToString() == "") ? num3 + 0M : num3 + Convert.ToDecimal(dataTable3.Rows[0]["IMPSANDET"])) : num3 + 0M;
          string str6 = !(DATSANANN.ToString() == "") ? "0,00" : Convert.ToDecimal(dataTable3.Rows[0]["IMPSANDET"]).ToString("#,##0.#0");
          aCell17 = new Cell((IElement) new Paragraph(new Phrase(Convert.ToDecimal(dataTable3.Rows[0]["IMPCON"]).ToString("#,##0.#0"), font5)));
          aCell17.Add((object) new Paragraph(new Phrase(Convert.ToDecimal(dataTable3.Rows[0]["IMPADDREC"]).ToString("#,##0.#0"), font5)));
          Cell cell1 = aCell17;
          num2 = Convert.ToDecimal(dataTable3.Rows[0]["IMPASSCONAZI"]);
          Paragraph o1 = new Paragraph(new Phrase(num2.ToString("#,##0.#0"), font5));
          cell1.Add((object) o1);
          Cell cell2 = aCell17;
          num2 = Convert.ToDecimal(dataTable3.Rows[0]["IMPABBAZI"]);
          Paragraph o2 = new Paragraph(new Phrase(num2.ToString("#,##0.#0"), font5));
          cell2.Add((object) o2);
          if (str2 != "")
            aCell17.Add((object) new Paragraph(new Phrase("0,00", font7)));
          if (dataTable3.Rows[0]["SANSOTSOG"].ToString().Trim() == "S")
            aCell17.Add((object) new Paragraph(new Phrase("0,00", font5)));
          else
            aCell17.Add((object) new Paragraph(new Phrase(str6, font5)));
          aCell17.Add((object) new Paragraph(new Phrase(num4.ToString("#,##0.#0"), font5)));
        }
        else
        {
          aCell17 = new Cell((IElement) new Paragraph(new Phrase("0,00", font5)));
          aCell17.Add((object) new Paragraph(new Phrase("0,00", font5)));
          aCell17.Add((object) new Paragraph(new Phrase("0,00", font5)));
          aCell17.Add((object) new Paragraph(new Phrase("0,00", font5)));
          if (str2 != "")
            aCell17.Add((object) new Paragraph(new Phrase("0,00", font7)));
          aCell17.Add((object) new Paragraph(new Phrase("0,00", font5)));
          aCell17.Add((object) new Paragraph(new Phrase("0,00", font5)));
        }
        aCell17.HorizontalAlignment = 2;
        aCell17.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell17, 1, 6);
        Cell aCell18 = new Cell("");
        aCell18.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell18, 2, 0);
        Cell aCell19 = new Cell("");
        aCell19.GrayFill = 0.3f;
        aCell19.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell19, 2, 1);
        Cell aCell20 = new Cell("");
        aCell20.GrayFill = 0.3f;
        aCell20.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell20, 2, 2);
        Cell aCell21 = new Cell("");
        aCell21.GrayFill = 0.3f;
        aCell21.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell21, 2, 3);
        Cell aCell22 = new Cell("");
        aCell22.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell22, 2, 4);
        Cell aCell23 = new Cell("");
        aCell23.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell23, 2, 5);
        Cell aCell24 = new Cell("");
        aCell24.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell24, 2, 6);
        Cell aCell25 = new Cell("");
        aCell25.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell25, 3, 0);
        Cell aCell26 = new Cell("");
        aCell26.GrayFill = 0.3f;
        aCell26.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell26, 3, 1);
        Cell aCell27 = new Cell("");
        aCell27.GrayFill = 0.3f;
        aCell27.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell27, 3, 2);
        Cell aCell28 = new Cell("");
        aCell28.GrayFill = 0.3f;
        aCell28.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell28, 3, 3);
        Cell aCell29 = new Cell("");
        aCell29.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell29, 3, 4);
        Cell aCell30 = new Cell("");
        aCell30.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell30, 3, 5);
        Cell aCell31 = new Cell("");
        aCell31.BorderWidth = 0.0f;
        tabFooter.AddCell(aCell31, 3, 6);
        int[] widths2 = new int[2]{ 20, 80 };
        tabFooter2.AutoFillEmptyCells = true;
        tabFooter2.WidthPercentage = 100f;
        tabFooter2.Cellpadding = 2f;
        tabFooter2.Cellspacing = 0.0f;
        tabFooter2.Offset = 20f;
        tabFooter2.BorderWidth = 0.0f;
        tabFooter2.SetWidths(widths2);
        Cell aCell32 = new Cell((IElement) new Phrase("Riferimenti del versamento", font3));
        aCell32.BorderWidth = 0.0f;
        tabFooter2.AddCell(aCell32, 0, 0);
        Cell aCell33;
        if (!DBNull.Value.Equals(dataTable3.Rows[0]["DATVER"]))
        {
          string[] strArray = new string[8]
          {
            "data operazione:  ",
            dataTable3.Rows[0]["DATVER"].ToString().Substring(0, 2),
            "-",
            dataTable3.Rows[0]["DATVER"].ToString().Substring(3, 2),
            "-",
            dataTable3.Rows[0]["DATVER"].ToString().Substring(6, 4),
            "        Importo del versamento:  € ",
            null
          };
          num2 = Convert.ToDecimal(dataTable3.Rows[0]["IMPVER"]);
          strArray[7] = num2.ToString("#,##0.#0");
          aCell33 = new Cell((IElement) new Phrase(string.Concat(strArray), font5));
        }
        else
          aCell33 = new Cell((IElement) new Phrase("data operazione:  ___ ___ ______  Importo del versamento:  ________________", font5));
        aCell33.BorderWidth = 0.0f;
        tabFooter2.AddCell(aCell33, 0, 1);
        Cell aCell34 = !(dataTable3.Rows[0]["CODMODPAG"].ToString() == "2") ? new Cell((IElement) new Phrase("[  ]  C/C Postale", font5)) : new Cell((IElement) new Phrase("[X]  C/C Postale", font5));
        aCell34.BorderWidth = 0.0f;
        tabFooter2.AddCell(aCell34, 1, 0);
        Cell aCell35;
        if (dataTable3.Rows[0]["CODMODPAG"].ToString() == "2")
          aCell35 = new Cell((IElement) new Phrase("Ufficio Postale:  " + (dataTable3.Rows[0]["UFFPOS"].ToString() + new string(' ', 51)).Substring(0, 51) + "  Città:  " + (dataTable3.Rows[0]["CITDIC"].ToString() + new string(' ', 35)).Substring(0, 35) + " Prov.: " + (dataTable3.Rows[0]["PRODIC"].ToString() + new string(' ', 5)).Substring(0, 5), font5));
        else
          aCell35 = new Cell((IElement) new Phrase("Ufficio Postale:  ___________________________________________________  Città:  ___________________________________ Prov.: _____", font5));
        aCell35.BorderWidth = 0.0f;
        tabFooter2.AddCell(aCell35, 1, 1);
        Cell aCell36 = !(dataTable3.Rows[0]["CODMODPAG"].ToString() == "3") ? new Cell((IElement) new Phrase("[  ]  Bonifico Bancario", font5)) : new Cell((IElement) new Phrase("[X]  Bonifico Bancario", font5));
        aCell36.BorderWidth = 0.0f;
        tabFooter2.AddCell(aCell36, 2, 0);
        Cell aCell37;
        if (Convert.ToInt32(dataTable3.Rows[0][nameof (ANNDEN)]) > 2007)
        {
          if (dataTable3.Rows[0]["CODMODPAG"].ToString() == "3")
            aCell37 = new Cell((IElement) new Phrase("Banca mittente:  " + (dataTable3.Rows[0]["UFFPOS"].ToString() + new string(' ', 34)).Substring(0, 34) + "  IBAN:  " + (dataTable3.Rows[0]["IBAN"].ToString() + new string(' ', 27)).Substring(0, 27) + " Città:  " + (dataTable3.Rows[0]["CITDIC"].ToString() + new string(' ', 33)).Substring(0, 33) + " Prov.: " + (dataTable3.Rows[0]["PRODIC"].ToString() + new string(' ', 5)).Substring(0, 5), font5));
          else
            aCell37 = new Cell((IElement) new Phrase("Banca mittente:  ___________________________________  IBAN:  ________________________  Città:  ________________________________ Prov.: _____", font5));
        }
        else if (dataTable3.Rows[0]["CODMODPAG"].ToString() == "3")
          aCell37 = new Cell((IElement) new Phrase("Banca mittente:  " + (dataTable3.Rows[0]["UFFPOS"].ToString() + new string(' ', 34)).Substring(0, 34) + "  ABI:  " + (dataTable3.Rows[0]["ABIDIC"].ToString() + new string(' ', 8)).Substring(0, 8) + "  CAB:  " + (dataTable3.Rows[0]["CABDIC"].ToString() + new string(' ', 8)).Substring(0, 8) + "  Città:  " + (dataTable3.Rows[0]["CITDIC"].ToString() + new string(' ', 33)).Substring(0, 33) + " Prov.: " + (dataTable3.Rows[0]["PRODIC"].ToString() + new string(' ', 5)).Substring(0, 5), font5));
        else
          aCell37 = new Cell((IElement) new Phrase("Banca mittente:  ___________________________________  ABI:  ________  CAB:  ________  Città:  ________________________________ Prov.: _____", font5));
        aCell37.BorderWidth = 0.0f;
        tabFooter2.AddCell(aCell37, 2, 1);
        Cell aCell38 = !(dataTable3.Rows[0]["CODMODPAG"].ToString() == "4") ? new Cell((IElement) new Phrase("[  ]  Versamento compensato totalmente da credito precedente", font5)) : new Cell((IElement) new Phrase("[X]  Versamento compensato totalmente da credito precedente", font5));
        aCell38.BorderWidth = 0.0f;
        aCell38.Colspan = 2;
        tabFooter2.AddCell(aCell38, 3, 0);
        Cell aCell39 = !(dataTable3.Rows[0]["CODMODPAG"].ToString() == "5") ? new Cell((IElement) new Phrase("[  ]  Versamento differito (con applicazioni delle sanzioni previste)", font5)) : new Cell((IElement) new Phrase("[X]  Versamento differito (con applicazioni delle sanzioni previste)", font5));
        aCell39.BorderWidth = 0.0f;
        aCell39.Colspan = 2;
        tabFooter2.AddCell(aCell39, 4, 0);
        Cell aCell40 = !(dataTable3.Rows[0]["CODMODPAG"].ToString() == "6") ? new Cell((IElement) new Phrase("[  ]  Ritardato versamento per finanziamenti pubblici tardivamente erogati (delibera CdA n°38/98)", font5)) : new Cell((IElement) new Phrase("[X]  Ritardato versamento per finanziamenti pubblici tardivamente erogati (delibera CdA n°38/98)", font5));
        aCell40.BorderWidth = 0.0f;
        aCell40.Colspan = 2;
        tabFooter2.AddCell(aCell40, 5, 0);
      }
      else
      {
        iTextSharp.text.Font font8 = new iTextSharp.text.Font(font1, 7.75f, 1, new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue));
        iTextSharp.text.Font font9 = new iTextSharp.text.Font(font1, 7.75f, 1);
        int[] widths = new int[3]{ 87, 3, 10 };
        tabTotali.WidthPercentage = 100f;
        tabTotali.Cellpadding = 2f;
        tabTotali.Cellspacing = 0.0f;
        tabTotali.SetWidths(widths);
        tabTotali.Offset = 20f;
        tabTotali.BorderWidth = 0.0f;
        if (str4 == "")
          str1 = "Sanzioni annullate";
        else if (DATSANANN != "")
        {
          str1 = "Sanzioni annullate";
        }
        else
        {
          string strSQL3 = "SELECT VALUE(TASSO, 0.00) AS TASSO, TIPMOV, DESCAUREP AS DESC FROM TIPMOVCAU " + " WHERE CODCAU ='" + str4 + "' AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
          dataTable1.Clear();
          DataTable dataTable5 = dataLayer.GetDataTable(strSQL3);
          if (dataTable5.Rows.Count > 0)
          {
            string str7 = dataTable5.Rows[0]["TIPMOV"].ToString().Trim();
            if (!(str7 == "SAN_MD"))
            {
              if (str7 == "SAN_RD")
                str1 = !(dataTable5.Rows[0]["DESC"].ToString() != "") ? "" : dataTable5.Rows[0]["DESC"].ToString().Substring(0, 1) + dataTable5.Rows[0]["DESC"].ToString().ToLower().Substring(1, dataTable5.Rows[0]["DESC"].ToString().Length - 1) + " annullate";
            }
            else
              str1 = !(dataTable5.Rows[0]["DESC"].ToString() != "") ? "" : dataTable5.Rows[0]["DESC"].ToString().Substring(0, 1) + dataTable5.Rows[0]["DESC"].ToString().ToLower().Substring(1, dataTable5.Rows[0]["DESC"].ToString().Length - 1) + " annullate";
          }
          else
            str1 = "";
        }
        Cell aCell41 = new Cell((IElement) new Paragraph(new Phrase("Totale generale CTR annullato", font2)));
        aCell41.Add((object) new Paragraph(new Phrase(str3, font2)));
        aCell41.Add((object) new Paragraph(new Phrase("Assistenza Contrattuale annullata", font2)));
        aCell41.Add((object) new Paragraph(new Phrase("Abbonamenti P.A. annullati", font2)));
        aCell41.Add((object) new Paragraph(new Phrase(str1, font2)));
        aCell41.Add((object) new Paragraph(new Phrase("TOTALE dovuto annullato", font9)));
        aCell41.HorizontalAlignment = 2;
        aCell41.BorderWidth = 0.0f;
        tabTotali.AddCell(aCell41, 0, 0);
        font4 = new iTextSharp.text.Font(font1, 6.75f, 0, new Color((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue));
        Cell aCell42 = new Cell((IElement) new Paragraph(new Phrase("€", font2)));
        aCell42.Add((object) new Paragraph(new Phrase("€", font2)));
        aCell42.Add((object) new Paragraph(new Phrase("€", font2)));
        aCell42.Add((object) new Paragraph(new Phrase("€", font2)));
        aCell42.Add((object) new Paragraph(new Phrase("€", font2)));
        aCell42.Add((object) new Paragraph(new Phrase("€", font9)));
        aCell42.HorizontalAlignment = 2;
        aCell42.BorderWidth = 0.0f;
        tabTotali.AddCell(aCell42, 0, 1);
        Cell aCell43;
        if (dataTable3.Rows.Count > 0)
        {
          Decimal num5 = num1 + Convert.ToDecimal(dataTable3.Rows[0]["IMPCON"]) + Convert.ToDecimal(dataTable3.Rows[0]["IMPADDREC"]) + Convert.ToDecimal(dataTable3.Rows[0]["IMPASSCONAZI"]) + Convert.ToDecimal(dataTable3.Rows[0]["IMPABBAZI"]);
          Decimal num6 = !(dataTable3.Rows[0]["SANSOTSOG"].ToString().Trim() == "S") ? (!(DATSANANN.ToString() == "") ? num5 + 0M : num5 + Convert.ToDecimal(dataTable3.Rows[0]["IMPSANDET"])) : num5 + 0M;
          string str8 = !(DATSANANN.ToString() == "") ? "0,00" : Convert.ToDecimal(dataTable3.Rows[0]["IMPSANDET"]).ToString("#,##0.#0");
          aCell43 = new Cell((IElement) new Paragraph(new Phrase(Convert.ToDecimal(dataTable3.Rows[0]["IMPCON"]).ToString("#,##0.#0"), font2)));
          aCell43.Add((object) new Paragraph(new Phrase(Convert.ToDecimal(dataTable3.Rows[0]["IMPADDREC"]).ToString("#,##0.#0"), font2)));
          Cell cell3 = aCell43;
          Decimal num7 = Convert.ToDecimal(dataTable3.Rows[0]["IMPASSCONAZI"]);
          Paragraph o3 = new Paragraph(new Phrase(num7.ToString("#,##0.#0"), font2));
          cell3.Add((object) o3);
          Cell cell4 = aCell43;
          num7 = Convert.ToDecimal(dataTable3.Rows[0]["IMPABBAZI"]);
          Paragraph o4 = new Paragraph(new Phrase(num7.ToString("#,##0.#0"), font2));
          cell4.Add((object) o4);
          if (dataTable3.Rows[0]["SANSOTSOG"].ToString().Trim() == "S")
            aCell43.Add((object) new Paragraph(new Phrase("0,00", font2)));
          else
            aCell43.Add((object) new Paragraph(new Phrase(str8, font2)));
          aCell43.Add((object) new Paragraph(new Phrase(num6.ToString("#,##0.#0"), font9)));
        }
        else
        {
          aCell43 = new Cell((IElement) new Paragraph(new Phrase("0,00", font2)));
          aCell43.Add((object) new Paragraph(new Phrase("0,00", font2)));
          aCell43.Add((object) new Paragraph(new Phrase("0,00", font2)));
          aCell43.Add((object) new Paragraph(new Phrase("0,00", font2)));
          aCell43.Add((object) new Paragraph(new Phrase("0,00", font2)));
          aCell43.Add((object) new Paragraph(new Phrase("0,00", font2)));
        }
        aCell43.HorizontalAlignment = 2;
        aCell43.BorderWidth = 0.0f;
        tabTotali.AddCell(aCell43, 0, 2);
      }
    }

    public void CreaStampaArretratoAnnullato(ref DataTable DTARR, string strPath)
    {
      Document document = new Document(PageSize.A4.Rotate(), 10f, 20f, 5f, 5f);
      PdfWriter instance = PdfWriter.GetInstance(document, (Stream) new FileStream(strPath, FileMode.Create));
      int num1 = 1;
      int num2 = 0;
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      int cntNumPag = 1;
      document.Open();
      for (int index1 = 0; index1 <= DTARR.Rows.Count - 1; ++index1)
      {
        if (DTARR.Rows[index1]["TIPMOV"].ToString().Trim() == "AR")
        {
          PdfContentByte directContent = instance.DirectContent;
          string strSQL1 = " SELECT DENTES.SANSOTSOG, DENTES.IMPADDREC, DENTES.DATSANANN, DENTES.DATSAN, DENTES.IMPABB, DENTES.IMPASSCON, " + " DENTES.NUMSAN, DENTES.DATCONMOV, DENTES.NUMMOV, DENTES.CODCAUSAN, " + " DENTES.IMPCON AS IMPCONTOT, DENTES.IMPSANDET AS IMPSANTOT, " + " DENTES.DATMOVANN, DENTES.NUMMOVANN, DENTES.NUMSANANN " + " FROM DENTES " + " WHERE DENTES.CODPOS = " + DTARR.Rows[index1]["CODPOS"]?.ToString() + "  AND DENTES.ANNDEN = " + DTARR.Rows[index1]["ANNDEN"]?.ToString() + "  AND DENTES.MESDEN = " + DTARR.Rows[index1]["MESDEN"]?.ToString() + "  AND DENTES.PRODEN = " + DTARR.Rows[index1]["PRODEN"]?.ToString();
          DataTable dataTable3 = dataLayer.GetDataTable(strSQL1);
          BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
          if (index1 > 0)
          {
            cntNumPag = 1;
            num2 = 0;
            num1 = 1;
            document.NewPage();
          }
          dataTable3.Rows[0]["SANSOTSOG"].ToString().Trim();
          string IMPSANTOT = !(dataTable3.Rows[0]["NUMSAN"].ToString() == "") ? (!((dataTable3.Rows[0]["NUMMOVANN"].ToString() ?? "") == "") ? Convert.ToString(dataTable3.Rows[0]["IMPSANTOT"]) : (!(dataTable3.Rows[0]["NUMSANANN"].ToString() != "") ? Convert.ToString(dataTable3.Rows[0]["IMPSANTOT"]) : "0,00")) : "0,00";
          this.ScriviIntestazioneArretratoAnnullato(ref directContent, ref document, Convert.ToInt32(DTARR.Rows[index1]["CODPOS"]), dataTable3.Rows[0]["DATCONMOV"]?.ToString() ?? "");
          this.ScriviPieDiPaginaArretratoAnnullato(ref directContent, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["NUMSANANN"].ToString());
          Cell Cell = new Cell();
          iTextSharp.text.Table tabTestata = new iTextSharp.text.Table(10, 1);
          tabTestata.WidthPercentage = 100f;
          tabTestata.AutoFillEmptyCells = true;
          tabTestata.BorderWidth = 0.0f;
          tabTestata.BorderColor = Color.WHITE;
          tabTestata.Cellpadding = 2f;
          int[] widths1 = new int[10]
          {
            8,
            36,
            12,
            3,
            8,
            11,
            3,
            8,
            3,
            8
          };
          tabTestata.SetWidths(widths1);
          Cell CellTotali = new Cell();
          iTextSharp.text.Table TabTotali = new iTextSharp.text.Table(4, 7);
          TabTotali.WidthPercentage = 45f;
          TabTotali.AutoFillEmptyCells = true;
          TabTotali.Alignment = 2;
          TabTotali.BorderWidth = 0.0f;
          TabTotali.BorderColor = Color.WHITE;
          int[] widths2 = new int[4]{ 30, 45, 1, 22 };
          TabTotali.SetWidths(widths2);
          iTextSharp.text.Font FontTestata = new iTextSharp.text.Font(BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false), 8.25f, 0);
          this.ScriviTestataArretratoAnnullato(ref tabTestata, ref Cell, ref FontTestata);
          string strSQL2 = " SELECT DENDET.MAT, DENDET.NUMSAN, DENDET.PRODEN, DENDET.ALIQUOTA, " + " DENDET.ANNDEN,DENDET.MESDEN, SUM(DENDET.IMPCON) AS IMPCON, " + " SUM(DENDET.IMPSANDET) AS IMPSAN, SUM(DENDET.IMPRET) AS IMPRET," + " DENDET.CODPOS, " + " ISCT.COG, ISCT.NOM " + " FROM DENDET " + " INNER JOIN ISCT ON " + " ISCT.MAT = DENDET.MAT " + " WHERE DENDET.CODPOS = " + DTARR.Rows[index1]["CODPOS"]?.ToString() + " AND DENDET.ANNDEN = " + DTARR.Rows[index1]["ANNDEN"]?.ToString() + " AND DENDET.MESDEN = " + DTARR.Rows[index1]["MESDEN"]?.ToString() + " AND DENDET.PRODEN = " + DTARR.Rows[index1]["PRODEN"]?.ToString() + " AND DENDET.TIPMOV = 'AR'" + " GROUP BY DENDET.MAT, DENDET.CODPOS, DENDET.PRODEN, DENDET.MESDEN, DENDET.NUMSAN," + " DENDET.ANNDEN, DENDET.ALIQUOTA, " + " ISCT.COG, ISCT.NOM ORDER BY ISCT.COG";
          DataTable dataTable4 = dataLayer.GetDataTable(strSQL2);
          BaseFont font = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
          iTextSharp.text.Font FontDettaglio = new iTextSharp.text.Font(font, 8.25f, 0);
          iTextSharp.text.Font FontNascondi = new iTextSharp.text.Font(font, 8.25f, 0, Color.WHITE);
          int TotPag = !((Decimal) (dataTable4.Rows.Count - dataTable4.Rows.Count / 18 * 18) <= 9M) ? dataTable4.Rows.Count / 18 + 2 : dataTable4.Rows.Count / 18 + 1;
          this.scriviNumPagArretratoAnnullato(ref directContent, cntNumPag, TotPag);
          for (int index2 = 0; index2 <= dataTable4.Rows.Count - 1; ++index2)
          {
            this.ScriviSanzioneArretratoAnnullato(ref directContent, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["DATSANANN"].ToString(), dataTable3.Rows[0]["DATCONMOV"].ToString(), dataTable3.Rows[0]["NUMMOV"].ToString(), dataTable3.Rows[0]["DATSAN"].ToString(), dataTable3.Rows[0]["NUMSAN"].ToString(), Convert.ToString(dataTable4.Rows[index2]["MESDEN"]), Convert.ToString(dataTable4.Rows[index2]["ANNDEN"]), Convert.ToInt32(dataTable4.Rows[index2]["CODPOS"]), dataTable3.Rows[0]["DATMOVANN"].ToString(), dataTable3.Rows[0]["NUMMOVANN"].ToString(), dataTable3.Rows[0]["NUMSANANN"].ToString());
            if (num1 == 19)
            {
              tabTestata.Offset = 120f;
              document.Add((IElement) tabTestata);
              tabTestata.DeleteAllRows();
              document.NewPage();
              this.ScriviIntestazioneArretratoAnnullato(ref directContent, ref document, Convert.ToInt32(DTARR.Rows[index1]["CODPOS"]), dataTable3.Rows[0]["DATCONMOV"]?.ToString() ?? "");
              this.ScriviPieDiPaginaArretratoAnnullato(ref directContent, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["NUMSANANN"].ToString());
              this.ScriviTestataArretratoAnnullato(ref tabTestata, ref Cell, ref FontTestata);
              ++cntNumPag;
              num2 = index2;
              num1 = 1;
              this.scriviNumPagArretratoAnnullato(ref directContent, cntNumPag, TotPag);
            }
            string str1 = dataTable4.Rows[index2]["COG"].ToString().Trim() + " " + dataTable4.Rows[index2]["NOM"].ToString().Trim();
            string str2 = dataTable4.Rows[index2]["MESDEN"].ToString().Trim() + "/" + dataTable4.Rows[index2]["ANNDEN"].ToString().Trim();
            Decimal num3 = Convert.ToDecimal(dataTable4.Rows[index2]["IMPRET"]);
            string str3 = num3.ToString("#,##0.#0");
            num3 = Convert.ToDecimal(dataTable4.Rows[index2]["IMPCON"]);
            string str4 = num3.ToString("#,##0.#0");
            string str5;
            if (dataTable4.Rows[index2]["NUMSAN"].ToString() == "")
            {
              str5 = "0,00";
            }
            else
            {
              num3 = Convert.ToDecimal(dataTable4.Rows[index2]["IMPSAN"]);
              str5 = num3.ToString("#,##0.#0");
            }
            string str6 = dataTable4.Rows[index2]["ALIQUOTA"].ToString().Trim() + " %";
            Cell aCell1 = new Cell((IElement) new Phrase(dataTable4.Rows[index2]["MAT"].ToString().Trim(), FontDettaglio));
            tabTestata.AddCell(aCell1, 1 + index2 - num2, 0);
            aCell1.HorizontalAlignment = 1;
            aCell1.VerticalAlignment = 5;
            aCell1.BorderWidth = 0.0f;
            aCell1.BorderWidthBottom = 0.5f;
            Cell aCell2 = new Cell((IElement) new Phrase(str1, FontDettaglio));
            tabTestata.AddCell(aCell2, 1 + index2 - num2, 1);
            aCell2.HorizontalAlignment = 0;
            aCell2.VerticalAlignment = 5;
            aCell2.BorderWidth = 0.0f;
            aCell2.BorderWidthBottom = 0.5f;
            Cell aCell3 = new Cell((IElement) new Phrase(str2, FontDettaglio));
            tabTestata.AddCell(aCell3, 1 + index2 - num2, 2);
            aCell3.HorizontalAlignment = 1;
            aCell3.VerticalAlignment = 5;
            aCell3.BorderWidth = 0.0f;
            aCell3.BorderWidthBottom = 0.5f;
            Cell aCell4 = new Cell((IElement) new Phrase("€", FontDettaglio));
            tabTestata.AddCell(aCell4, 1 + index2 - num2, 3);
            aCell4.HorizontalAlignment = 2;
            aCell4.VerticalAlignment = 5;
            aCell4.BorderWidth = 0.0f;
            aCell4.BorderWidthBottom = 0.5f;
            Cell aCell5 = new Cell((IElement) new Phrase(str3, FontDettaglio));
            tabTestata.AddCell(aCell5, 1 + index2 - num2, 4);
            aCell5.HorizontalAlignment = 2;
            aCell5.VerticalAlignment = 5;
            aCell5.BorderWidth = 0.0f;
            aCell5.BorderWidthBottom = 0.5f;
            Cell aCell6 = new Cell((IElement) new Phrase(str6, FontDettaglio));
            tabTestata.AddCell(aCell6, 1 + index2 - num2, 5);
            aCell6.HorizontalAlignment = 1;
            aCell6.VerticalAlignment = 5;
            aCell6.BorderWidth = 0.0f;
            aCell6.BorderWidthBottom = 0.5f;
            Cell aCell7 = new Cell((IElement) new Phrase("€", FontDettaglio));
            tabTestata.AddCell(aCell7, 1 + index2 - num2, 6);
            aCell7.HorizontalAlignment = 2;
            aCell7.VerticalAlignment = 5;
            aCell7.BorderWidth = 0.0f;
            aCell7.BorderWidthBottom = 0.5f;
            Cell aCell8 = new Cell((IElement) new Phrase(str4, FontDettaglio));
            tabTestata.AddCell(aCell8, 1 + index2 - num2, 7);
            aCell8.HorizontalAlignment = 2;
            aCell8.VerticalAlignment = 5;
            aCell8.BorderWidth = 0.0f;
            aCell8.BorderWidthBottom = 0.5f;
            Cell aCell9 = new Cell((IElement) new Phrase("€", FontDettaglio));
            tabTestata.AddCell(aCell9, 1 + index2 - num2, 8);
            aCell9.HorizontalAlignment = 2;
            aCell9.VerticalAlignment = 5;
            aCell9.BorderWidth = 0.0f;
            aCell9.BorderWidthBottom = 0.5f;
            Cell = new Cell((IElement) new Phrase(str5, FontDettaglio));
            tabTestata.AddCell(Cell, 1 + index2 - num2, 9);
            Cell.HorizontalAlignment = 2;
            Cell.VerticalAlignment = 5;
            Cell.BorderWidth = 0.0f;
            Cell.BorderWidthBottom = 0.5f;
            ++num1;
          }
          tabTestata.Offset = 120f;
          document.Add((IElement) tabTestata);
          if (num1 > 10)
          {
            document.NewPage();
            this.ScriviIntestazioneArretratoAnnullato(ref directContent, ref document, Convert.ToInt32(DTARR.Rows[index1]["CODPOS"]), dataTable3.Rows[0]["DATCONMOV"]?.ToString() ?? "");
            this.ScriviPieDiPaginaArretratoAnnullato(ref directContent, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["NUMSANANN"].ToString());
            this.scriviNumPagArretratoAnnullato(ref directContent, cntNumPag + 1, TotPag);
            this.ScriviTotaliArretratoAnnullato(ref TabTotali, ref CellTotali, ref FontDettaglio, ref FontNascondi, ref FontTestata, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["NUMSANANN"].ToString(), dataTable3.Rows[0]["IMPADDREC"].ToString(), dataTable3.Rows[0]["IMPABB"].ToString(), dataTable3.Rows[0]["IMPASSCON"].ToString(), dataTable3.Rows[0]["IMPCONTOT"].ToString(), IMPSANTOT, dataTable3.Rows[0]["NUMSAN"].ToString(), ref directContent, dataTable3.Rows[0]["NUMMOVANN"]?.ToString() ?? "");
            TabTotali.Offset = 120f;
            document.Add((IElement) TabTotali);
          }
          else
          {
            this.ScriviTotaliArretratoAnnullato(ref TabTotali, ref CellTotali, ref FontDettaglio, ref FontNascondi, ref FontTestata, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["NUMSANANN"].ToString(), dataTable3.Rows[0]["IMPADDREC"].ToString(), dataTable3.Rows[0]["IMPABB"].ToString(), dataTable3.Rows[0]["IMPASSCON"].ToString(), dataTable3.Rows[0]["IMPCONTOT"].ToString(), IMPSANTOT, dataTable3.Rows[0]["NUMSAN"].ToString(), ref directContent, dataTable3.Rows[0]["NUMMOVANN"]?.ToString() ?? "");
            TabTotali.Offset = 20f;
            document.Add((IElement) TabTotali);
          }
          Convert.ToDecimal(0.0);
        }
      }
      document.Close();
      instance.Close();
      Process.Start(strPath);
    }

    private void ScriviIntestazioneArretratoAnnullato(
      ref PdfContentByte cb,
      ref Document document,
      int CODPOS,
      string DATCONMOV)
    {
      DataTable dataTable1 = new DataTable();
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
      BaseFont font2 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      DataLayer dataLayer = new DataLayer();
      Image instance = Image.GetInstance(this.EstraiFilePGM_Byte("CNT_LOGO"));
      instance.ScaleAbsolute(103f, 51f);
      instance.SetAbsolutePosition(82f, 545f);
      document.Add((IElement) instance);
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(1, "FONDAZIONE E.N.P.A.I.A.", 395f, 580f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(1, "ENTE NAZIONALE DI PREVIDENZA PER GLI ADDETTI E PER GLI", 395f, 570f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(1, "IMPIEGATI IN AGRICOLTURA", 395f, 560f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Viale Beethoven, 48 - 00144 ROMA", 395f, 550f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Call Center 800.010270 - Fax 06/5914444 - 06/5458385", 395f, 540f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Internet: www.enpaia.it       Email: info@enpaia.it", 395f, 530f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Ufficio contributi e riscossione", 133f, 542f, 0.0f);
      cb.EndText();
      string strSQL1 = " SELECT  AZI.RAGSOC, AZI.CODPOS, DUG.DENDUG, INDSED.IND, INDSED.NUMCIV, " + " INDSED.CAP, INDSED.DENLOC, INDSED.SIGPRO, INDSED.DENSTAEST, " + " INDSED.CODCOM, '' AS DENCOM " + " FROM  AZI " + " INNER JOIN INDSED ON " + " INDSED.CODPOS = AZI.CODPOS " + " LEFT JOIN DUG ON INDSED.CODDUG = DUG.CODDUG " + " WHERE INDSED.TIPIND=1 " + " AND INDSED.DATINI <= " + this.Module_DB2Date(Convert.ToDateTime(DATCONMOV)) + " AND AZI.CODPOS= " + CODPOS.ToString() + " AND INDSED.DATCOM = (SELECT MAX(DATCOM) FROM INDSED WHERE " + " INDSED.CODPOS = " + CODPOS.ToString() + " AND TIPIND = 1 AND INDSED.DATINI <= " + this.Module_DB2Date(Convert.ToDateTime(DATCONMOV)) + ") " + " ORDER BY AZI.CODPOS, INDSED.DATCOM DESC FETCH FIRST 1 ROWS ONLY";
      DataTable dataTable2 = dataLayer.GetDataTable(strSQL1);
      if (dataTable2.Rows.Count <= 0)
        return;
      if (dataTable2.Rows[0]["CODCOM"].ToString().Trim() != "")
      {
        string strSQL2 = "SELECT VALUE(DENCOM,'') AS DENCOM FROM CODCOM WHERE CODCOM = " + DBMethods.DoublePeakForSql(dataTable2.Rows[0]["CODCOM"].ToString().Trim());
        string str = dataLayer.GetDataTable(strSQL2).Rows[0]["DENCOM"].ToString();
        dataTable2.Rows[0]["DENCOM"] = (object) str;
      }
      else
        dataTable2.Rows[0]["DENCOM"] = (object) "";
      string text1 = dataTable2.Rows[0]["RAGSOC"].ToString().Trim();
      string text2;
      if (DBNull.Value.Equals(dataTable2.Rows[0]["NUMCIV"]) | dataTable2.Rows[0]["NUMCIV"].ToString().Trim() == "")
        text2 = dataTable2.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable2.Rows[0]["IND"].ToString().Trim();
      else
        text2 = dataTable2.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable2.Rows[0]["IND"].ToString().Trim() + ", " + dataTable2.Rows[0]["NUMCIV"].ToString().Trim();
      string text3;
      if (!DBNull.Value.Equals(dataTable2.Rows[0]["DENSTAEST"]))
        text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENSTAEST"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
      else if (dataTable2.Rows[0]["DENLOC"].ToString().Trim() != dataTable2.Rows[0]["DENCOM"].ToString().Trim())
      {
        if (dataTable2.Rows[0]["DENLOC"].ToString().Trim() == "")
          text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENCOM"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
        else
          text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENLOC"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
      }
      else
        text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENCOM"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
      BaseFont font3 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.BeginText();
      cb.SetFontAndSize(font3, 8.25f);
      cb.ShowTextAligned(0, text1, 565f, 552f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font3, 8.25f);
      cb.ShowTextAligned(0, text2, 565f, 542f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font3, 8.25f);
      cb.ShowTextAligned(0, text3, 565f, 532f, 0.0f);
      cb.EndText();
    }

    private void ScriviPieDiPaginaArretratoAnnullato(
      ref PdfContentByte cb,
      string CODCAUSAN,
      string NUMSANANN)
    {
      DataTable dataTable1 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      cb.BeginText();
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.SetFontAndSize(font1, 5.25f);
      cb.ShowTextAligned(0, "Legenda causali:", 10f, 71f, 0.0f);
      cb.EndText();
      int y = 64;
      string str1 = "SELECT DISTINCT CODCAU, VALUE(DESCAUREP, ' ') AS DESC FROM TIPMOVCAU ";
      string strSQL = !(CODCAUSAN == "") ? str1 + " WHERE TIPMOV IN('ANN_AR', 'ANN_SAN_MD', 'ANN_SAN_RD') ORDER BY CODCAU ASC" : str1 + " WHERE TIPMOV IN('ANN_AR') ORDER BY CODCAU ASC";
      DataTable dataTable2 = dataLayer.GetDataTable(strSQL);
      if (dataTable2.Rows.Count > 0)
      {
        for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
        {
          if (dataTable2.Rows[index]["DESC"].ToString() != "")
          {
            string str2 = dataTable2.Rows[index]["CODCAU"]?.ToString() + " " + dataTable2.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[index]["DESC"].ToString().Length - 1);
            cb.BeginText();
            BaseFont font2 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
            cb.SetFontAndSize(font2, 5.25f);
            cb.ShowTextAligned(0, str2.Trim(), 10f, (float) y, 0.0f);
            cb.EndText();
          }
          else
          {
            string text = "";
            cb.BeginText();
            BaseFont font3 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
            cb.SetFontAndSize(font3, 5.25f);
            cb.ShowTextAligned(0, text, 10f, (float) y, 0.0f);
            cb.EndText();
          }
          y -= 7;
        }
      }
      else
      {
        for (int index = 0; index <= 5; ++index)
        {
          cb.BeginText();
          BaseFont font4 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
          cb.SetFontAndSize(font4, 5.25f);
          cb.ShowTextAligned(0, "", 10f, (float) y, 0.0f);
          cb.EndText();
          y -= 7;
        }
      }
      cb.BeginText();
      BaseFont font5 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.SetFontAndSize(font5, 8.25f);
      cb.ShowTextAligned(1, "Per informazioni telefonare al Call Center 800.010270 - 800.313231", 420f, 10f, 0.0f);
      cb.EndText();
    }

    private void ScriviTestataArretratoAnnullato(
      ref iTextSharp.text.Table tabTestata,
      ref Cell Cell,
      ref iTextSharp.text.Font FontTestata)
    {
      Cell = new Cell((IElement) new Phrase("Matricola", FontTestata));
      Cell.HorizontalAlignment = 1;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 0);
      Cell = new Cell((IElement) new Phrase("Cognome e Nome", FontTestata));
      Cell.HorizontalAlignment = 0;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 1);
      Cell = new Cell((IElement) new Phrase("Periodo", FontTestata));
      Cell.HorizontalAlignment = 1;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 2);
      Cell = new Cell((IElement) new Phrase("Retribuzione", FontTestata));
      Cell.HorizontalAlignment = 2;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 3);
      Cell = new Cell((IElement) new Phrase("Aliquota", FontTestata));
      Cell.HorizontalAlignment = 1;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 5);
      Cell = new Cell((IElement) new Phrase("Contributo", FontTestata));
      Cell.HorizontalAlignment = 2;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.Colspan = 2;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 6);
      Cell = new Cell((IElement) new Phrase("Sanzione", FontTestata));
      Cell.HorizontalAlignment = 2;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.Colspan = 2;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 8);
    }

    private void ScriviSanzioneArretratoAnnullato(
      ref PdfContentByte cb,
      string CODCAUSAN,
      string DATSANANN,
      string DATCONMOV,
      string NUMMOV,
      string DATSAN,
      string NUMSAN,
      string MESDEN,
      string ANNDEN,
      int CODPOS,
      string DATMOVANN,
      string NUMMOVANN,
      string NUMSANANN)
    {
      DateTimeFormatInfo dateTimeFormat = new CultureInfo("it-IT", false).DateTimeFormat;
      BaseFont font = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      string str1 = !(DATCONMOV != "") ? "" : Convert.ToString(Convert.ToDateTime(DATCONMOV));
      string str2 = !(DATMOVANN != "") ? "" : Convert.ToString(Convert.ToDateTime(DATMOVANN));
      string str3 = !(DATSAN != "") ? "" : Convert.ToString(Convert.ToDateTime(DATSAN));
      string text1;
      if (NUMSAN == "")
        text1 = "Nota di annullamento n. " + NUMMOVANN.Trim() + " emessa il " + str2 + " riferita alla denuncia di arretrati del " + DATCONMOV.Trim().Substring(0, 10);
      else
        text1 = "Nota di annullamento n. " + NUMMOVANN.Trim() + " e Nota di annullamento n. " + NUMSANANN.Trim() + " emesse il " + str2 + " riferita alla denuncia di arretrati del " + DATCONMOV.Trim().Substring(0, 10);
      string text2 = "Posizione assicurativa " + CODPOS.ToString();
      cb.BeginText();
      cb.SetFontAndSize(font, 8.25f);
      cb.ShowTextAligned(0, text1, 10f, 500f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font, 8.25f);
      cb.ShowTextAligned(0, text2, 10f, 490f, 0.0f);
      cb.EndText();
    }

    private void scriviNumPagArretratoAnnullato(ref PdfContentByte cb, int cntNumPag, int TotPag)
    {
      string text = "Pag. " + cntNumPag.ToString() + " di " + TotPag.ToString();
      cb.BeginText();
      BaseFont font = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.SetFontAndSize(font, 7.25f);
      cb.ShowTextAligned(1, text, 800f, 10f, 0.0f);
      cb.EndText();
    }

    private void ScriviTotaliArretratoAnnullato(
      ref iTextSharp.text.Table TabTotali,
      ref Cell CellTotali,
      ref iTextSharp.text.Font FontDettaglio,
      ref iTextSharp.text.Font FontNascondi,
      ref iTextSharp.text.Font FontTestata,
      string CODCAU,
      string NUMSANANN,
      string IMPADDREC,
      string IMPABB,
      string IMPASSCON,
      string IMPCONTOT,
      string IMPSANTOT,
      string NUMSAN,
      ref PdfContentByte cb,
      string NUMMOVANN)
    {
      string str1 = "";
      DataTable dataTable1 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 3, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 4, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 5, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 6, 0);
      CellTotali = new Cell((IElement) new Phrase("Importo contributo annullato", FontDettaglio));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 1);
      CellTotali = new Cell((IElement) new Phrase("Importo addizionale annullato", FontDettaglio));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 1);
      CellTotali = new Cell((IElement) new Phrase("Assistenza contrattuale annullata", FontDettaglio));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 1);
      CellTotali = new Cell((IElement) new Phrase("Abbonamento periodico annullato", FontDettaglio));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 3, 1);
      if (CODCAU == "")
      {
        str1 = "Sanzioni annullate";
      }
      else
      {
        string strSQL = "SELECT VALUE(TASSO, 0.00) AS TASSO, TIPMOV, DESCAUREP AS DESC FROM TIPMOVCAU " + " WHERE CODCAU ='" + CODCAU + "' AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
        dataTable1.Clear();
        DataTable dataTable2 = dataLayer.GetDataTable(strSQL);
        if (dataTable2.Rows.Count > 0)
        {
          for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
          {
            string str2 = dataTable2.Rows[index]["TIPMOV"].ToString().Trim();
            if (!(str2 == "SAN_MD"))
            {
              if (str2 == "SAN_RD")
                str1 = !(dataTable2.Rows[index]["DESC"].ToString() != "") ? "" : dataTable2.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[index]["DESC"].ToString().Length - 1) + " annullate";
            }
            else
              str1 = !(dataTable2.Rows[index]["DESC"].ToString() != "") ? "" : dataTable2.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[index]["DESC"].ToString().Length - 1) + " annullate";
          }
        }
        else
          str1 = "";
      }
      CellTotali = new Cell((IElement) new Phrase(str1, FontDettaglio));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 4, 1);
      CellTotali = new Cell((IElement) new Phrase(".....", FontNascondi));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 6;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 5, 1);
      CellTotali = new Cell((IElement) new Phrase("Importo complessivo annullato", FontTestata));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 6, 1);
      CellTotali = new Cell((IElement) new Phrase("€", FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 2);
      CellTotali = new Cell((IElement) new Phrase("€", FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 2);
      CellTotali = new Cell((IElement) new Phrase("€", FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 2);
      CellTotali = new Cell((IElement) new Phrase("€", FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 3, 2);
      CellTotali = new Cell((IElement) new Phrase("€", FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 6;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 4, 2);
      CellTotali = new Cell();
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 5, 2);
      CellTotali = new Cell((IElement) new Phrase("€", FontTestata));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 6, 2);
      Decimal num;
      if (NUMSAN == "")
        IMPSANTOT = "0,00";
      else if (NUMMOVANN == "")
      {
        if (NUMSANANN != "")
        {
          IMPSANTOT = "0,00";
        }
        else
        {
          num = Convert.ToDecimal(IMPSANTOT);
          IMPSANTOT = num.ToString("#,##0.#0");
        }
      }
      else
      {
        num = Convert.ToDecimal(IMPSANTOT);
        IMPSANTOT = num.ToString("#,##0.#0");
      }
      num = Convert.ToDecimal(IMPABB);
      IMPABB = num.ToString("#,##0.#0");
      string str3 = !(NUMSAN == "") ? (!(NUMMOVANN == "") ? Convert.ToString(Convert.ToDecimal(IMPCONTOT) + Convert.ToDecimal(IMPADDREC) + Convert.ToDecimal(IMPASSCON) + Convert.ToDecimal(IMPABB) + Convert.ToDecimal(IMPSANTOT)) : (!(NUMSANANN != "") ? Convert.ToString(Convert.ToDecimal(IMPCONTOT) + Convert.ToDecimal(IMPADDREC) + Convert.ToDecimal(IMPASSCON) + Convert.ToDecimal(IMPABB) + Convert.ToDecimal(IMPSANTOT)) : Convert.ToString(Convert.ToDecimal(IMPCONTOT) + Convert.ToDecimal(IMPADDREC) + Convert.ToDecimal(IMPASSCON) + Convert.ToDecimal(IMPABB)))) : Convert.ToString(Convert.ToDecimal(IMPCONTOT) + Convert.ToDecimal(IMPADDREC) + Convert.ToDecimal(IMPASSCON) + Convert.ToDecimal(IMPABB));
      ref Cell local1 = ref CellTotali;
      num = Convert.ToDecimal(IMPCONTOT);
      Cell cell1 = new Cell((IElement) new Phrase(num.ToString("#,##0.#0"), FontDettaglio));
      local1 = cell1;
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 3);
      ref Cell local2 = ref CellTotali;
      num = Convert.ToDecimal(IMPADDREC);
      Cell cell2 = new Cell((IElement) new Phrase(num.ToString("#,##0.#0"), FontDettaglio));
      local2 = cell2;
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 3);
      ref Cell local3 = ref CellTotali;
      num = Convert.ToDecimal(IMPASSCON);
      Cell cell3 = new Cell((IElement) new Phrase(num.ToString("#,##0.#0"), FontDettaglio));
      local3 = cell3;
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 3);
      CellTotali = new Cell((IElement) new Phrase(IMPABB, FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 3, 3);
      CellTotali = new Cell((IElement) new Phrase(IMPSANTOT, FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 6;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 4, 3);
      CellTotali = new Cell((IElement) new Phrase("....", FontNascondi));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 6;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 5, 3);
      ref Cell local4 = ref CellTotali;
      num = Convert.ToDecimal(str3);
      Cell cell4 = new Cell((IElement) new Phrase(num.ToString("#,##0.#0"), FontTestata));
      local4 = cell4;
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 6, 3);
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
      iTextSharp.text.Font font2 = new iTextSharp.text.Font(font1, 7.75f, 1);
      iTextSharp.text.Font font3 = new iTextSharp.text.Font(font1, 7.75f, 0, Color.WHITE);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 7, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 8, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 9, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 10, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 11, 0);
    }

    public void CreaStampaSanzioneAnnullataNU(ref DataTable dtNotifica, string strPath)
    {
      Document document = new Document(PageSize.A4.Rotate(), 10f, 20f, 5f, 5f);
      PdfWriter instance = PdfWriter.GetInstance(document, (Stream) new FileStream(strPath, FileMode.Create));
      int num1 = 1;
      int num2 = 0;
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      int cntNumPag = 1;
      document.Open();
      for (int index1 = 0; index1 <= dtNotifica.Rows.Count - 1; ++index1)
      {
        PdfContentByte directContent = instance.DirectContent;
        string strSQL1 = " SELECT DENTES.SANSOTSOG, DENTES.IMPADDREC, DENTES.DATSANANN, DENTES.DATSAN, DENTES.IMPABB, DENTES.IMPASSCON, " + " DENTES.NUMSAN, DENTES.DATCONMOV, DENTES.NUMMOV, DENTES.CODCAUSAN, " + " DENTES.IMPCON AS IMPCONTOT, DENTES.IMPSANDET AS IMPSANTOT, " + " DENTES.DATMOVANN, DENTES.NUMMOVANN, DENTES.NUMSANANN " + " FROM DENTES " + " WHERE DENTES.CODPOS = " + dtNotifica.Rows[index1]["CODPOS"]?.ToString() + "  AND DENTES.ANNDEN = " + dtNotifica.Rows[index1]["ANNDEN"]?.ToString() + "  AND DENTES.MESDEN = " + dtNotifica.Rows[index1]["MESDEN"]?.ToString() + "  AND DENTES.PRODEN = " + dtNotifica.Rows[index1]["PRODEN"]?.ToString();
        DataTable dataTable3 = dataLayer.GetDataTable(strSQL1);
        BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
        if (index1 > 0)
        {
          cntNumPag = 1;
          num2 = 0;
          num1 = 1;
          document.NewPage();
        }
        dataTable3.Rows[0]["SANSOTSOG"].ToString().Trim();
        string IMPSANTOT = !(dataTable3.Rows[0]["NUMSAN"].ToString() == "") ? dataTable3.Rows[0]["IMPSANTOT"].ToString() : "0,00";
        this.ScriviIntestazioneSanzioneAnnullataNU(ref directContent, ref document, Convert.ToInt32(dtNotifica.Rows[index1]["CODPOS"]), dataTable3.Rows[0]["DATCONMOV"]?.ToString() ?? "");
        this.ScriviPieDiPaginaSanzioneAnnullataNU(ref directContent, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["DATSANANN"].ToString());
        Cell Cell = new Cell();
        iTextSharp.text.Table tabTestata = new iTextSharp.text.Table(10, 1);
        tabTestata.WidthPercentage = 100f;
        tabTestata.AutoFillEmptyCells = true;
        tabTestata.BorderWidth = 0.0f;
        tabTestata.BorderColor = Color.WHITE;
        tabTestata.Cellpadding = 2f;
        int[] widths1 = new int[10]
        {
          8,
          20,
          5,
          1,
          7,
          11,
          1,
          7,
          1,
          7
        };
        tabTestata.SetWidths(widths1);
        Cell CellTotali = new Cell();
        iTextSharp.text.Table TabTotali = new iTextSharp.text.Table(4, 3);
        TabTotali.WidthPercentage = 45f;
        TabTotali.AutoFillEmptyCells = true;
        TabTotali.Alignment = 2;
        TabTotali.BorderWidth = 0.0f;
        TabTotali.BorderColor = Color.WHITE;
        int[] widths2 = new int[4]{ 30, 45, 1, 22 };
        TabTotali.SetWidths(widths2);
        iTextSharp.text.Font FontTestata = new iTextSharp.text.Font(BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false), 8.25f, 0);
        this.ScriviTestataSanzioneAnnullataNU(ref tabTestata, ref Cell, ref FontTestata);
        string strSQL2 = " SELECT DENDET.MAT, DENDET.NUMSAN, DENDET.PRODEN, DENDET.ALIQUOTA, " + " DENDET.ANNDEN,DENDET.MESDEN, SUM(DENDET.IMPCON) AS IMPCON, " + " SUM(DENDET.IMPSANDET) AS IMPSAN, SUM(DENDET.IMPRET) AS IMPRET," + " DENDET.CODPOS, " + " ISCT.COG, ISCT.NOM " + " FROM DENDET " + " INNER JOIN ISCT ON " + " ISCT.MAT = DENDET.MAT " + " WHERE DENDET.CODPOS = " + dtNotifica.Rows[index1]["CODPOS"]?.ToString() + "  AND DENDET.ANNDEN = " + dtNotifica.Rows[index1]["ANNDEN"]?.ToString() + "  AND DENDET.MESDEN = " + dtNotifica.Rows[index1]["MESDEN"]?.ToString() + "  AND DENDET.PRODEN = " + dtNotifica.Rows[index1]["PRODEN"]?.ToString() + "  AND DENDET.TIPMOV = 'NU'" + " GROUP BY DENDET.MAT, DENDET.CODPOS, DENDET.PRODEN, DENDET.MESDEN, DENDET.NUMSAN," + " DENDET.ANNDEN, DENDET.ALIQUOTA, " + " ISCT.COG, ISCT.NOM ORDER BY ISCT.COG";
        DataTable dataTable4 = dataLayer.GetDataTable(strSQL2);
        BaseFont font = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
        iTextSharp.text.Font FontDettaglio = new iTextSharp.text.Font(font, 8.25f, 0);
        iTextSharp.text.Font FontNascondi = new iTextSharp.text.Font(font, 8.25f, 0, Color.WHITE);
        int TotPag = !((Decimal) (dataTable4.Rows.Count - dataTable4.Rows.Count / 18 * 18) <= 9M) ? dataTable4.Rows.Count / 18 + 2 : dataTable4.Rows.Count / 18 + 1;
        this.scriviNumPagSanzioneAnnullataNU(ref directContent, cntNumPag, TotPag);
        for (int index2 = 0; index2 <= dataTable4.Rows.Count - 1; ++index2)
        {
          this.ScriviSanzioneAnnullataNU(ref directContent, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["DATSANANN"].ToString(), dataTable3.Rows[0]["DATCONMOV"].ToString(), dataTable3.Rows[0]["NUMMOV"].ToString(), dataTable3.Rows[0]["DATSAN"].ToString(), dataTable3.Rows[0]["NUMSAN"].ToString(), dataTable4.Rows[index2]["MESDEN"].ToString(), dataTable4.Rows[index2]["ANNDEN"].ToString(), Convert.ToInt32(dataTable4.Rows[index2]["CODPOS"]), dataTable3.Rows[0]["NUMSANANN"].ToString());
          if (num1 == 19)
          {
            tabTestata.Offset = 108f;
            document.Add((IElement) tabTestata);
            tabTestata.DeleteAllRows();
            document.NewPage();
            this.ScriviIntestazioneSanzioneAnnullataNU(ref directContent, ref document, Convert.ToInt32(dtNotifica.Rows[index1]["CODPOS"]), dataTable3.Rows[0]["DATCONMOV"]?.ToString() ?? "");
            this.ScriviPieDiPaginaSanzioneAnnullataNU(ref directContent, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["DATSANANN"].ToString());
            this.ScriviTestataSanzioneAnnullataNU(ref tabTestata, ref Cell, ref FontTestata);
            ++cntNumPag;
            num2 = index2;
            num1 = 1;
            this.scriviNumPagSanzioneAnnullataNU(ref directContent, cntNumPag, TotPag);
          }
          string str1 = dataTable4.Rows[index2]["COG"].ToString().Trim() + " " + dataTable4.Rows[index2]["NOM"].ToString().Trim();
          string str2 = dataTable4.Rows[index2]["MESDEN"].ToString().Trim() + "/" + dataTable4.Rows[index2]["ANNDEN"].ToString().Trim();
          Decimal num3 = Convert.ToDecimal(dataTable4.Rows[index2]["IMPRET"]);
          string str3 = num3.ToString("#,##0.#0");
          num3 = Convert.ToDecimal(dataTable4.Rows[index2]["IMPCON"]);
          string str4 = num3.ToString("#,##0.#0");
          string str5;
          if (dataTable4.Rows[index2]["NUMSAN"].ToString() == "")
          {
            str5 = "0,00";
          }
          else
          {
            num3 = Convert.ToDecimal(dataTable4.Rows[index2]["IMPSAN"]);
            str5 = num3.ToString("#,##0.#0");
          }
          string str6 = dataTable4.Rows[index2]["ALIQUOTA"].ToString().Trim() + " %";
          Cell aCell1 = new Cell((IElement) new Phrase(dataTable4.Rows[index2]["MAT"].ToString().Trim(), FontDettaglio));
          aCell1.HorizontalAlignment = 1;
          aCell1.VerticalAlignment = 5;
          aCell1.BorderWidth = 0.0f;
          aCell1.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(aCell1, 1 + index2 - num2, 0);
          Cell aCell2 = new Cell((IElement) new Phrase(str1.Trim().Substring(0, 35), FontDettaglio));
          aCell2.HorizontalAlignment = 0;
          aCell2.VerticalAlignment = 5;
          aCell2.BorderWidth = 0.0f;
          aCell2.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(aCell2, 1 + index2 - num2, 1);
          Cell aCell3 = new Cell((IElement) new Phrase(str2, FontDettaglio));
          aCell3.HorizontalAlignment = 1;
          aCell3.VerticalAlignment = 5;
          aCell3.BorderWidth = 0.0f;
          aCell3.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(aCell3, 1 + index2 - num2, 2);
          Cell aCell4 = new Cell((IElement) new Phrase("€", FontDettaglio));
          aCell4.HorizontalAlignment = 2;
          aCell4.VerticalAlignment = 5;
          aCell4.BorderWidth = 0.0f;
          aCell4.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(aCell4, 1 + index2 - num2, 3);
          Cell aCell5 = new Cell((IElement) new Phrase(str3, FontDettaglio));
          aCell5.HorizontalAlignment = 2;
          aCell5.VerticalAlignment = 5;
          aCell5.BorderWidth = 0.0f;
          aCell5.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(aCell5, 1 + index2 - num2, 4);
          Cell aCell6 = new Cell((IElement) new Phrase(str6, FontDettaglio));
          aCell6.HorizontalAlignment = 1;
          aCell6.VerticalAlignment = 5;
          aCell6.BorderWidth = 0.0f;
          aCell6.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(aCell6, 1 + index2 - num2, 5);
          Cell aCell7 = new Cell((IElement) new Phrase("€", FontDettaglio));
          aCell7.HorizontalAlignment = 2;
          aCell7.VerticalAlignment = 5;
          aCell7.BorderWidth = 0.0f;
          aCell7.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(aCell7, 1 + index2 - num2, 6);
          Cell aCell8 = new Cell((IElement) new Phrase(str4, FontDettaglio));
          aCell8.HorizontalAlignment = 2;
          aCell8.VerticalAlignment = 5;
          aCell8.BorderWidth = 0.0f;
          aCell8.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(aCell8, 1 + index2 - num2, 7);
          Cell aCell9 = new Cell((IElement) new Phrase("€", FontDettaglio));
          aCell9.HorizontalAlignment = 2;
          aCell9.VerticalAlignment = 5;
          aCell9.BorderWidth = 0.0f;
          aCell9.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(aCell9, 1 + index2 - num2, 8);
          Cell = new Cell((IElement) new Phrase(str5, FontDettaglio));
          Cell.HorizontalAlignment = 2;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 9);
          ++num1;
        }
        tabTestata.Offset = 108f;
        document.Add((IElement) tabTestata);
        if (num1 > 10)
        {
          document.NewPage();
          this.ScriviIntestazioneSanzioneAnnullataNU(ref directContent, ref document, Convert.ToInt32(dtNotifica.Rows[index1]["CODPOS"]), dataTable3.Rows[0]["DATCONMOV"]?.ToString() ?? "");
          this.ScriviPieDiPaginaSanzioneAnnullataNU(ref directContent, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["DATSANANN"].ToString());
          this.scriviNumPagSanzioneAnnullataNU(ref directContent, cntNumPag + 1, TotPag);
          this.ScriviTotaliSanzioneAnnullataNU(ref TabTotali, ref CellTotali, ref FontDettaglio, ref FontNascondi, ref FontTestata, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["DATSANANN"].ToString(), IMPSANTOT, dataTable3.Rows[0]["NUMSAN"].ToString(), ref directContent);
          TabTotali.Offset = 108f;
          document.Add((IElement) TabTotali);
        }
        else
        {
          this.ScriviTotaliSanzioneAnnullataNU(ref TabTotali, ref CellTotali, ref FontDettaglio, ref FontNascondi, ref FontTestata, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["DATSANANN"].ToString(), IMPSANTOT, dataTable3.Rows[0]["NUMSAN"].ToString(), ref directContent);
          TabTotali.Offset = 10f;
          document.Add((IElement) TabTotali);
        }
        Convert.ToDecimal(0.0);
      }
      document.Close();
      instance.Close();
      Process.Start(strPath);
    }

    private void ScriviIntestazioneSanzioneAnnullataNU(
      ref PdfContentByte cb,
      ref Document document,
      int CODPOS,
      string DATCONMOV)
    {
      DataTable dataTable1 = new DataTable();
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
      BaseFont font2 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      DataLayer dataLayer = new DataLayer();
      Image instance = Image.GetInstance(this.EstraiFilePGM_Byte("CNT_LOGO"));
      instance.ScaleAbsolute(103f, 51f);
      instance.SetAbsolutePosition(82f, 545f);
      document.Add((IElement) instance);
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(1, "FONDAZIONE E.N.P.A.I.A.", 395f, 580f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(1, "ENTE NAZIONALE DI PREVIDENZA PER GLI ADDETTI E PER GLI", 395f, 570f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(1, "IMPIEGATI IN AGRICOLTURA", 395f, 560f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Viale Beethoven, 48 - 00144 ROMA", 395f, 550f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Call Center 800.010270 - Fax 06/5914444 - 06/5458385", 395f, 540f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Internet: www.enpaia.it       Email: info@enpaia.it", 395f, 530f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Ufficio contributi e riscossione", 133f, 542f, 0.0f);
      cb.EndText();
      string strSQL1 = " SELECT  AZI.RAGSOC, AZI.CODPOS, DUG.DENDUG, INDSED.IND, INDSED.NUMCIV, " + " INDSED.CAP, INDSED.DENLOC, INDSED.SIGPRO, INDSED.DENSTAEST, " + " INDSED.CODCOM, '' AS DENCOM " + " FROM  AZI " + " INNER JOIN INDSED ON " + " INDSED.CODPOS = AZI.CODPOS " + " LEFT JOIN DUG ON INDSED.CODDUG = DUG.CODDUG " + " WHERE INDSED.TIPIND=1 " + " AND INDSED.DATINI <= " + this.Module_DB2Date(Convert.ToDateTime(DATCONMOV)) + " AND AZI.CODPOS= " + CODPOS.ToString() + " AND INDSED.DATCOM = (SELECT MAX(DATCOM) FROM INDSED WHERE " + " INDSED.CODPOS = " + CODPOS.ToString() + " AND TIPIND = 1 AND INDSED.DATINI <= " + this.Module_DB2Date(Convert.ToDateTime(DATCONMOV)) + ") " + " ORDER BY AZI.CODPOS, INDSED.DATCOM DESC FETCH FIRST 1 ROWS ONLY";
      DataTable dataTable2 = dataLayer.GetDataTable(strSQL1);
      if (dataTable2.Rows.Count <= 0)
        return;
      if (dataTable2.Rows[0]["CODCOM"].ToString().Trim() != "")
      {
        string strSQL2 = "SELECT VALUE(DENCOM,'') AS DENCOM FROM CODCOM WHERE CODCOM = " + DBMethods.DoublePeakForSql(dataTable2.Rows[0]["CODCOM"].ToString().Trim());
        string str = dataLayer.GetDataTable(strSQL2).Rows[0]["DENCOM"].ToString();
        dataTable2.Rows[0]["DENCOM"] = (object) str;
      }
      else
        dataTable2.Rows[0]["DENCOM"] = (object) "";
      string text1 = dataTable2.Rows[0]["RAGSOC"].ToString().Trim();
      string text2;
      if (DBNull.Value.Equals(dataTable2.Rows[0]["NUMCIV"]) | dataTable2.Rows[0]["NUMCIV"].ToString().Trim() == "")
        text2 = dataTable2.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable2.Rows[0]["IND"].ToString().Trim();
      else
        text2 = dataTable2.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable2.Rows[0]["IND"].ToString().Trim() + ", " + dataTable2.Rows[0]["NUMCIV"].ToString().Trim();
      string text3;
      if (!DBNull.Value.Equals(dataTable2.Rows[0]["DENSTAEST"]))
        text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENSTAEST"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
      else if (dataTable2.Rows[0]["DENLOC"].ToString().Trim() != dataTable2.Rows[0]["DENCOM"].ToString().Trim())
      {
        if (dataTable2.Rows[0]["DENLOC"].ToString().Trim() == "")
          text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENCOM"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
        else
          text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENLOC"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
      }
      else
        text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENCOM"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
      BaseFont font3 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.BeginText();
      cb.SetFontAndSize(font3, 8.25f);
      cb.ShowTextAligned(0, text1, 565f, 552f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font3, 8.25f);
      cb.ShowTextAligned(0, text2, 565f, 542f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font3, 8.25f);
      cb.ShowTextAligned(0, text3, 565f, 532f, 0.0f);
      cb.EndText();
    }

    private void ScriviPieDiPaginaSanzioneAnnullataNU(
      ref PdfContentByte cb,
      string CODCAUSAN,
      string DATSANANN)
    {
      DataTable dataTable1 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      cb.BeginText();
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.SetFontAndSize(font1, 5.25f);
      cb.ShowTextAligned(0, "Legenda causali:", 10f, 71f, 0.0f);
      cb.EndText();
      int y = 64;
      string strSQL = "SELECT DISTINCT CODCAU, VALUE(DESCAUREP, ' ') AS DESC FROM TIPMOVCAU " + " WHERE TIPMOV IN('SAN_MD', 'ANN_SAN_MD', 'SAN_RD', 'ANN_SAN_RD') ORDER BY CODCAU ASC";
      DataTable dataTable2 = dataLayer.GetDataTable(strSQL);
      if (dataTable2.Rows.Count > 0)
      {
        for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
        {
          if (dataTable2.Rows[index]["DESC"].ToString() != "")
          {
            string str = dataTable2.Rows[index]["CODCAU"]?.ToString() + " " + dataTable2.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[index]["DESC"].ToString().Length - 1);
            cb.BeginText();
            BaseFont font2 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
            cb.SetFontAndSize(font2, 5.25f);
            cb.ShowTextAligned(0, str.Trim(), 10f, (float) y, 0.0f);
            cb.EndText();
          }
          else
          {
            string text = "";
            cb.BeginText();
            BaseFont font3 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
            cb.SetFontAndSize(font3, 5.25f);
            cb.ShowTextAligned(0, text, 10f, (float) y, 0.0f);
            cb.EndText();
          }
          y -= 7;
        }
      }
      else
      {
        for (int index = 0; index <= 5; ++index)
        {
          cb.BeginText();
          BaseFont font4 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
          cb.SetFontAndSize(font4, 5.25f);
          cb.ShowTextAligned(0, "", 10f, (float) y, 0.0f);
          cb.EndText();
          y -= 7;
        }
      }
      cb.BeginText();
      BaseFont font5 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.SetFontAndSize(font5, 8.25f);
      cb.ShowTextAligned(1, "Per informazioni telefonare al Call Center 800.010270 - 800.313231", 420f, 10f, 0.0f);
      cb.EndText();
    }

    private void ScriviTestataSanzioneAnnullataNU(
      ref iTextSharp.text.Table tabTestata,
      ref Cell Cell,
      ref iTextSharp.text.Font FontTestata)
    {
      Cell = new Cell((IElement) new Phrase("Matricola", FontTestata));
      Cell.HorizontalAlignment = 1;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 0);
      Cell = new Cell((IElement) new Phrase("Cognome e Nome", FontTestata));
      Cell.HorizontalAlignment = 0;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 1);
      Cell = new Cell((IElement) new Phrase("Periodo", FontTestata));
      Cell.HorizontalAlignment = 1;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 2);
      Cell = new Cell((IElement) new Phrase("Retribuzione", FontTestata));
      Cell.HorizontalAlignment = 2;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 3);
      Cell = new Cell((IElement) new Phrase("Aliquota", FontTestata));
      Cell.HorizontalAlignment = 1;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 5);
      Cell = new Cell((IElement) new Phrase("Contributi", FontTestata));
      Cell.HorizontalAlignment = 2;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 6);
      Cell = new Cell((IElement) new Phrase("Sanzione", FontTestata));
      Cell.HorizontalAlignment = 2;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 8);
    }

    private void scriviNumPagSanzioneAnnullataNU(ref PdfContentByte cb, int cntNumPag, int TotPag)
    {
      string text = "Pag. " + cntNumPag.ToString() + " di " + TotPag.ToString();
      cb.BeginText();
      BaseFont font = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.SetFontAndSize(font, 7.25f);
      cb.ShowTextAligned(1, text, 800f, 10f, 0.0f);
      cb.EndText();
    }

    private void ScriviSanzioneAnnullataNU(
      ref PdfContentByte cb,
      string CODCAUSAN,
      string DATSANANN,
      string DATCONMOV,
      string NUMMOV,
      string DATSAN,
      string NUMSAN,
      string MESDEN,
      string ANNDEN,
      int CODPOS,
      string NUMSANANN)
    {
      DateTimeFormatInfo dateTimeFormat = new CultureInfo("it-IT", false).DateTimeFormat;
      BaseFont font = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      string str1 = !(DATSANANN != "") ? "" : Convert.ToString(Convert.ToDateTime(DATSANANN));
      string str2 = !(DATSAN != "") ? "" : Convert.ToString(Convert.ToDateTime(DATSAN));
      string text1 = "Nota di annullamento n. " + NUMSANANN.Trim() + " emessa il " + str1;
      string text2 = "relativa a Nota di sanzione n. " + NUMSAN.Trim() + " emessa il " + str2 + " riferita al periodo " + dateTimeFormat.MonthNames[Convert.ToInt32(MESDEN) - 1].ToString().ToUpper() + " " + ANNDEN;
      string text3 = "Posizione assicurativa " + CODPOS.ToString();
      cb.BeginText();
      cb.SetFontAndSize(font, 8.25f);
      cb.ShowTextAligned(0, text1, 10f, 500f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font, 8.25f);
      cb.ShowTextAligned(0, text2, 10f, 490f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font, 8.25f);
      cb.ShowTextAligned(0, text3, 10f, 480f, 0.0f);
      cb.EndText();
    }

    private void ScriviTotaliSanzioneAnnullataNU(
      ref iTextSharp.text.Table TabTotali,
      ref Cell CellTotali,
      ref iTextSharp.text.Font FontDettaglio,
      ref iTextSharp.text.Font FontNascondi,
      ref iTextSharp.text.Font FontTestata,
      string CODCAU,
      string DATSANANN,
      string IMPSANTOT,
      string NUMSAN,
      ref PdfContentByte cb)
    {
      string str1 = "";
      DataTable dataTable1 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      if (CODCAU == "")
      {
        str1 = "Sanzioni annullate";
      }
      else
      {
        string strSQL = "SELECT VALUE(TASSO, 0.00) AS TASSO, TIPMOV, DESCAUREP AS DESC FROM TIPMOVCAU " + " WHERE CODCAU ='" + CODCAU + "' AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
        DataTable dataTable2 = dataLayer.GetDataTable(strSQL);
        if (dataTable2.Rows.Count > 0)
        {
          for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
          {
            string str2 = dataTable2.Rows[index]["TIPMOV"].ToString().Trim();
            if (!(str2 == "SAN_MD"))
            {
              if (str2 == "SAN_RD")
              {
                if (dataTable2.Rows[index]["DESC"].ToString() != "")
                  str1 = dataTable2.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[index]["DESC"].ToString().Length - 1) + " al tasso del " + dataTable2.Rows[index]["TASSO"]?.ToString() + " % annuo";
                else
                  str1 = "";
              }
            }
            else if (dataTable2.Rows[index]["DESC"].ToString() != "")
              str1 = dataTable2.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[index]["DESC"].ToString().Length - 1) + " al tasso del " + dataTable2.Rows[index]["TASSO"]?.ToString() + " % annuo";
            else
              str1 = "";
          }
        }
        else
          str1 = "";
      }
      CellTotali = new Cell((IElement) new Phrase(str1, FontDettaglio));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 1);
      CellTotali = new Cell((IElement) new Phrase(".....", FontNascondi));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 6;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 1);
      CellTotali = new Cell((IElement) new Phrase("Importo complessivo annullato", FontTestata));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 1);
      CellTotali = new Cell((IElement) new Phrase("€", FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 6;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 2);
      CellTotali = new Cell();
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 2);
      CellTotali = new Cell((IElement) new Phrase("€", FontTestata));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 2);
      IMPSANTOT = !(NUMSAN == "") ? Convert.ToDecimal(IMPSANTOT).ToString("#,##0.#0") : "0,00";
      CellTotali = new Cell((IElement) new Phrase(IMPSANTOT, FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 6;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 3);
      CellTotali = new Cell((IElement) new Phrase("....", FontNascondi));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 6;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 3);
      CellTotali = new Cell((IElement) new Phrase(IMPSANTOT, FontTestata));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 3);
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
      iTextSharp.text.Font font2 = new iTextSharp.text.Font(font1, 7.75f, 1);
      iTextSharp.text.Font font3 = new iTextSharp.text.Font(font1, 7.75f, 0, Color.WHITE);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 3, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 4, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 5, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 6, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 7, 0);
    }

    public void CreaStampaSanzioneArretrati(ref DataTable dtSanzione, string strPath)
    {
      Document document = new Document(PageSize.A4.Rotate(), 15f, 15f, 15f, 15f);
      PdfWriter instance = PdfWriter.GetInstance(document, (Stream) new FileStream(strPath, FileMode.Create));
      int index1 = 0;
      int num1 = 1;
      int num2 = 0;
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataTable dataTable3 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      string NUMSAN = "";
      string DATSAN = "";
      string IMPSANDET = "";
      string SANSOTSOG = "";
      string DATCHI = "";
      DateTimeFormatInfo dateTimeFormat = new CultureInfo("it-IT", false).DateTimeFormat;
      int cntNumPag = 1;
      iTextSharp.text.Table tabFooter = new iTextSharp.text.Table(3, 1);
      Cell CellFooter = new Cell();
      tabFooter.WidthPercentage = 35f;
      tabFooter.AutoFillEmptyCells = true;
      tabFooter.Alignment = 2;
      tabFooter.BorderWidth = 0.0f;
      tabFooter.BorderColor = Color.WHITE;
      int[] widths1 = new int[3]{ 59, 19, 22 };
      tabFooter.SetWidths(widths1);
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      BaseFont font2 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
      BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAI"), "Cp1252", false);
      int int32_1 = Convert.ToInt32(dtSanzione.Rows[0]["PRODEN"]);
      int int32_2 = Convert.ToInt32(dtSanzione.Rows[0]["CODPOS"]);
      int int32_3 = Convert.ToInt32(dtSanzione.Rows[0]["ANNDEN"]);
      int int32_4 = Convert.ToInt32(dtSanzione.Rows[0]["MESDEN"]);
      string PERIODO = dateTimeFormat.MonthNames[int32_4 - 1].ToString().ToUpper() + " " + int32_3.ToString();
      document.Open();
      string strSQL1 = "SELECT SANSOTSOG, DATSANANN, NUMSAN, DATSAN, IMPDIS, IMPSANDET, NUMSANANN, CODCAUSAN, DATCHI, DATCONMOV" + " FROM  DENTES " + " WHERE ANNDEN = " + int32_3.ToString() + " AND MESDEN = " + int32_4.ToString() + " AND PRODEN = " + int32_1.ToString() + " AND CODPOS = " + int32_2.ToString();
      DataTable dataTable4 = dataLayer.GetDataTable(strSQL1);
      if (dataTable4.Rows.Count > 0)
      {
        NUMSAN = dataTable4.Rows[0]["NUMSAN"].ToString().Trim();
        IMPSANDET = Convert.ToString(dataTable4.Rows[0]["IMPSANDET"]);
        if (!DBNull.Value.Equals(dataTable4.Rows[0]["DATSAN"]))
        {
          DATSAN = dataTable4.Rows[0]["DATSAN"].ToString();
          Convert.ToDateTime(DATSAN);
        }
        Convert.ToString(Convert.ToDecimal(dataTable4.Rows[0]["IMPDIS"]) + Convert.ToDecimal(dataTable4.Rows[0]["IMPSANDET"]));
        SANSOTSOG = dataTable4.Rows[0]["SANSOTSOG"].ToString().Trim();
        DATCHI = dataTable4.Rows[0]["DATCHI"]?.ToString() ?? "";
      }
      string CODCAUSAN = dataTable4.Rows[0]["CODCAUSAN"].ToString();
      PdfContentByte directContent = instance.DirectContent;
      BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
      string str1 = dataTable4.Rows[0]["DATSANANN"].ToString();
      string DATSANANN = !(str1 != "") ? "" : Convert.ToDateTime(str1).ToString();
      string NUMSANANN = dataTable4.Rows[0]["NUMSANANN"].ToString();
      this.ScriviIntestazioneSanzioneArretrati(ref directContent, ref document, Convert.ToInt32(dtSanzione.Rows[index1]["CODPOS"]), PERIODO, NUMSAN, DATSAN, DATSANANN, NUMSANANN, CODCAUSAN, DATCHI, dataTable4.Rows[0]["DATCONMOV"]?.ToString() ?? "");
      this.ScriviPieDiPaginaSanzioneArretrati(ref directContent);
      Cell Cell = new Cell();
      iTextSharp.text.Table tabTestata = new iTextSharp.text.Table(14, 2);
      tabTestata.AutoFillEmptyCells = true;
      tabTestata.WidthPercentage = 100f;
      tabTestata.BorderWidth = 0.0f;
      tabTestata.Cellspacing = 0.0f;
      tabTestata.Cellpadding = 2f;
      int[] widths2 = new int[14]
      {
        7,
        20,
        6,
        4,
        4,
        4,
        8,
        9,
        4,
        8,
        9,
        5,
        4,
        8
      };
      tabTestata.SetWidths(widths2);
      iTextSharp.text.Font fontRigaTest = new iTextSharp.text.Font(font2, 6.75f, 0);
      iTextSharp.text.Font font3 = new iTextSharp.text.Font(font1, 7.75f, 0);
      this.ScriviTestataSanzioneArretrati(ref tabTestata, ref Cell, ref fontRigaTest);
      string strSQL2 = " SELECT DENDET.CODPOS," + " TRIM(ISCT.COG) || ' ' || TRIM(ISCT.NOM) AS COGNOM," + " DENDET.MAT," + " DENDET.ANNDEN," + " DENDET.MESDEN," + " DENDET.PRODEN," + " DENDET.IMPRET," + " DENDET.IMPCON," + " DENDET.ALIQUOTA," + " DENDET.DATINISAN," + " DENDET.DATFINSAN," + " DENDET.DAL," + " DENDET.AL," + " DENDET.IMPSANDET AS IMPSAN," + " DENDET.ANNCOM," + " DENDET.TASSAN" + " FROM DENDET" + " INNER JOIN ISCT ON" + " ISCT.MAT = DENDET.MAT" + " WHERE DENDET.ANNDEN = " + int32_3.ToString() + " AND DENDET.MESDEN = " + int32_4.ToString() + " AND DENDET.PRODEN = " + int32_1.ToString() + " AND DENDET.CODPOS = " + int32_2.ToString() + " AND DENDET.TIPMOV = 'AR'" + " ORDER BY ISCT.COG, ISCT.NOM, DENDET.MAT, DENDET.DAL";
      DataTable dataTable5 = dataLayer.GetDataTable(strSQL2);
      int TotPag = !((Decimal) (dataTable5.Rows.Count - dataTable5.Rows.Count / 18 * 18) <= 11M) ? dataTable5.Rows.Count / 18 + 2 : dataTable5.Rows.Count / 18 + 1;
      this.scriviNumPagSanzioneArretrati(ref directContent, cntNumPag, TotPag);
      for (int index2 = 0; index2 <= dataTable5.Rows.Count - 1; ++index2)
      {
        if (num1 == 19)
        {
          tabTestata.Offset = 105f;
          document.Add((IElement) tabTestata);
          tabTestata.DeleteAllRows();
          document.NewPage();
          this.ScriviIntestazioneSanzioneArretrati(ref directContent, ref document, Convert.ToInt32(dtSanzione.Rows[index1]["CODPOS"]), PERIODO, NUMSAN, DATSAN, DATSANANN, NUMSANANN, CODCAUSAN, DATCHI, dataTable4.Rows[index1]["DATCONMOV"]?.ToString() ?? "");
          this.ScriviPieDiPaginaSanzioneArretrati(ref directContent);
          this.ScriviTestataSanzioneArretrati(ref tabTestata, ref Cell, ref fontRigaTest);
          ++cntNumPag;
          num2 = index2;
          num1 = 1;
          this.scriviNumPagSanzioneArretrati(ref directContent, cntNumPag, TotPag);
        }
        string str2 = dataTable5.Rows[index2]["DAL"].ToString().Substring(0, 5);
        string str3 = dataTable5.Rows[index2]["AL"].ToString().Substring(0, 5);
        Decimal num3 = Convert.ToDecimal(dataTable5.Rows[index2]["IMPRET"]);
        string str4 = num3.ToString("#,##0.#0");
        num3 = Convert.ToDecimal(dataTable5.Rows[index2]["IMPCON"]);
        string str5 = num3.ToString("#,##0.#0");
        num3 = Convert.ToDecimal(dataTable5.Rows[index2]["IMPSAN"]);
        string str6 = num3.ToString("#,##0.#0");
        Cell aCell1 = new Cell((IElement) new Phrase(dataTable5.Rows[index2]["MAT"].ToString().Trim(), font3));
        aCell1.VerticalAlignment = 5;
        aCell1.HorizontalAlignment = 1;
        aCell1.BorderWidth = 0.0f;
        aCell1.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(aCell1, 2 + index2 - num2, 0);
        Cell aCell2 = new Cell((IElement) new Phrase(dataTable5.Rows[index2]["COGNOM"].ToString().Trim().Substring(0, 27), font3));
        aCell2.HorizontalAlignment = 0;
        aCell2.BorderWidth = 0.0f;
        aCell2.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(aCell2, 2 + index2 - num2, 1);
        Cell aCell3 = new Cell((IElement) new Phrase(dataTable5.Rows[index2]["ANNCOM"].ToString().Trim(), font3));
        aCell3.HorizontalAlignment = 1;
        aCell3.BorderWidth = 0.0f;
        aCell3.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(aCell3, 2 + index2 - num2, 2);
        Cell aCell4 = new Cell((IElement) new Phrase(str2, font3));
        aCell4.HorizontalAlignment = 1;
        aCell4.BorderWidth = 0.0f;
        aCell4.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(aCell4, 2 + index2 - num2, 3);
        Cell aCell5 = new Cell((IElement) new Phrase(str3, font3));
        aCell5.HorizontalAlignment = 1;
        aCell5.BorderWidth = 0.0f;
        aCell5.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(aCell5, 2 + index2 - num2, 4);
        Cell aCell6 = new Cell((IElement) new Phrase("€ ", font3));
        aCell6.HorizontalAlignment = 2;
        aCell6.BorderWidth = 0.0f;
        aCell6.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(aCell6, 2 + index2 - num2, 5);
        Cell aCell7 = new Cell((IElement) new Phrase(str4, font3));
        aCell7.HorizontalAlignment = 2;
        aCell7.BorderWidth = 0.0f;
        aCell7.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(aCell7, 2 + index2 - num2, 6);
        Cell aCell8 = new Cell((IElement) new Phrase(dataTable5.Rows[index2]["ALIQUOTA"]?.ToString() ?? "", font3));
        aCell8.HorizontalAlignment = 1;
        aCell8.BorderWidth = 0.0f;
        aCell8.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(aCell8, 2 + index2 - num2, 7);
        Cell aCell9 = new Cell((IElement) new Phrase("€ ", font3));
        aCell9.HorizontalAlignment = 1;
        aCell9.BorderWidth = 0.0f;
        aCell9.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(aCell9, 2 + index2 - num2, 8);
        Cell aCell10 = new Cell((IElement) new Phrase(str5, font3));
        aCell10.HorizontalAlignment = 2;
        aCell10.BorderWidth = 0.0f;
        aCell10.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(aCell10, 2 + index2 - num2, 9);
        if (dataTable5.Rows[index2]["DATFINSAN"].ToString().Trim() != "" | dataTable5.Rows[index2]["DATINISAN"].ToString().Trim() != "")
        {
          Cell aCell11 = new Cell((IElement) new Phrase(Convert.ToDateTime(dataTable5.Rows[index2]["DATFINSAN"]).Subtract(Convert.ToDateTime(dataTable5.Rows[index2]["DATINISAN"])).Days.ToString(), font3));
          aCell11.HorizontalAlignment = 1;
          aCell11.BorderWidth = 0.0f;
          aCell11.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(aCell11, 2 + index2 - num2, 10);
        }
        else
        {
          Cell aCell12 = new Cell((IElement) new Phrase("", font3));
          aCell12.HorizontalAlignment = 1;
          aCell12.BorderWidth = 0.0f;
          aCell12.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(aCell12, 2 + index2 - num2, 10);
        }
        Cell aCell13 = new Cell((IElement) new Phrase(dataTable5.Rows[index2]["TASSAN"]?.ToString() + " %", font3));
        aCell13.HorizontalAlignment = 1;
        aCell13.BorderWidth = 0.0f;
        aCell13.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(aCell13, 2 + index2 - num2, 11);
        Cell aCell14 = new Cell((IElement) new Phrase("€ ", font3));
        aCell14.HorizontalAlignment = 2;
        aCell14.BorderWidth = 0.0f;
        aCell14.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(aCell14, 2 + index2 - num2, 12);
        Cell = new Cell((IElement) new Phrase(str6, font3));
        Cell.HorizontalAlignment = 2;
        Cell.BorderWidth = 0.0f;
        Cell.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(Cell, 2 + index2 - num2, 13);
        ++num1;
      }
      tabTestata.Offset = 105f;
      document.Add((IElement) tabTestata);
      if (num1 > 12)
      {
        document.NewPage();
        this.ScriviIntestazioneSanzioneArretrati(ref directContent, ref document, Convert.ToInt32(dtSanzione.Rows[index1]["CODPOS"]), PERIODO, NUMSAN, DATSAN, DATSANANN, NUMSANANN, CODCAUSAN, DATCHI, dataTable4.Rows[0]["DATCONMOV"]?.ToString() ?? "");
        this.ScriviPieDiPaginaSanzioneArretrati(ref directContent);
        this.ScriviTestataSanzioneArretrati(ref tabTestata, ref Cell, ref fontRigaTest);
        this.scriviNumPagSanzioneArretrati(ref directContent, cntNumPag + 1, TotPag);
        this.ScriviTotaliSanzioneArretrati(ref document, ref tabFooter, ref CellFooter, IMPSANDET, SANSOTSOG, CODCAUSAN, ref directContent, DATSANANN);
        tabFooter.Offset = 120f;
        document.Add((IElement) tabFooter);
      }
      else
      {
        this.ScriviTotaliSanzioneArretrati(ref document, ref tabFooter, ref CellFooter, IMPSANDET, SANSOTSOG, CODCAUSAN, ref directContent, DATSANANN);
        tabFooter.Offset = 20f;
        document.Add((IElement) tabFooter);
      }
      document.Close();
      instance.Close();
      Process.Start(strPath);
    }

    private void ScriviIntestazioneSanzioneArretrati(
      ref PdfContentByte cb,
      ref Document document,
      int CODPOS,
      string PERIODO,
      string NUMSAN,
      string DATSAN,
      string DATSANANN,
      string NUMSANANN,
      string CODCAUSAN,
      string DATCHI,
      string DATCONMOV)
    {
      DataTable dataTable1 = new DataTable();
      string str1 = "";
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
      BaseFont font2 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      DataLayer dataLayer = new DataLayer();
      Image instance = Image.GetInstance(this.EstraiFilePGM_Byte("CNT_LOGO"));
      instance.ScaleAbsolute(103f, 51f);
      instance.SetAbsolutePosition(82f, 545f);
      document.Add((IElement) instance);
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(1, "FONDAZIONE E.N.P.A.I.A.", 395f, 580f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(1, "ENTE NAZIONALE DI PREVIDENZA PER GLI ADDETTI E PER GLI", 395f, 570f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(1, "IMPIEGATI IN AGRICOLTURA", 395f, 560f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Viale Beethoven, 48 - 00144 ROMA", 395f, 550f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Call Center 800.010270 - Fax 06/5914444 - 06/5458385", 395f, 540f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Internet: www.enpaia.it       Email: info@enpaia.it", 395f, 530f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Ufficio contributi e riscossione", 133f, 542f, 0.0f);
      cb.EndText();
      string strSQL1 = " SELECT  AZI.RAGSOC, AZI.CODPOS, DUG.DENDUG, INDSED.IND, INDSED.NUMCIV, " + " INDSED.CAP, INDSED.DENLOC, INDSED.SIGPRO, INDSED.DENSTAEST, " + " INDSED.CODCOM, '' AS DENCOM " + " FROM  AZI " + " INNER JOIN INDSED ON " + " INDSED.CODPOS = AZI.CODPOS " + " LEFT JOIN DUG ON INDSED.CODDUG = DUG.CODDUG " + " WHERE INDSED.TIPIND=1 " + " AND INDSED.DATINI <= " + this.Module_DB2Date(Convert.ToDateTime(DATCONMOV)) + " AND AZI.CODPOS= " + CODPOS.ToString() + " AND INDSED.DATCOM = (SELECT MAX(DATCOM) FROM INDSED WHERE " + " INDSED.CODPOS = " + CODPOS.ToString() + " AND TIPIND = 1 AND INDSED.DATINI <= " + this.Module_DB2Date(Convert.ToDateTime(DATCONMOV)) + ")" + " ORDER BY AZI.CODPOS, INDSED.DATCOM DESC FETCH FIRST 1 ROWS ONLY";
      DataTable dataTable2 = dataLayer.GetDataTable(strSQL1);
      if (dataTable2.Rows.Count > 0)
      {
        if (dataTable2.Rows[0]["CODCOM"].ToString().Trim() != "")
        {
          string strSQL2 = "SELECT VALUE(DENCOM,'') AS DENCOM FROM CODCOM WHERE CODCOM = " + this.objNet.DoublePeakForSql(dataTable2.Rows[0]["CODCOM"].ToString().Trim());
          string str2 = dataLayer.GetDataTable(strSQL2).Rows[0]["DENCOM"].ToString();
          dataTable2.Rows[0]["DENCOM"] = (object) str2;
        }
        else
          dataTable2.Rows[0]["DENCOM"] = (object) "";
        string text1 = dataTable2.Rows[0]["RAGSOC"].ToString().Trim();
        string text2;
        if (DBNull.Value.Equals(dataTable2.Rows[0]["NUMCIV"]) | dataTable2.Rows[0]["NUMCIV"].ToString().Trim() == "")
          text2 = dataTable2.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable2.Rows[0]["IND"].ToString().Trim();
        else
          text2 = dataTable2.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable2.Rows[0]["IND"].ToString().Trim() + ", " + dataTable2.Rows[0]["NUMCIV"].ToString().Trim();
        string text3;
        if (!DBNull.Value.Equals(dataTable2.Rows[0]["DENSTAEST"]))
          text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENSTAEST"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
        else if (dataTable2.Rows[0]["DENLOC"].ToString().Trim() != dataTable2.Rows[0]["DENCOM"].ToString().Trim())
        {
          if (dataTable2.Rows[0]["DENLOC"].ToString().Trim() == "")
            text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENCOM"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
          else
            text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENLOC"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
        }
        else
          text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENCOM"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
        BaseFont font3 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
        cb.BeginText();
        cb.SetFontAndSize(font3, 8.25f);
        cb.ShowTextAligned(0, text1, 565f, 552f, 0.0f);
        cb.EndText();
        cb.BeginText();
        cb.SetFontAndSize(font3, 8.25f);
        cb.ShowTextAligned(0, text2, 565f, 542f, 0.0f);
        cb.EndText();
        cb.BeginText();
        cb.SetFontAndSize(font3, 8.25f);
        cb.ShowTextAligned(0, text3, 565f, 532f, 0.0f);
        cb.EndText();
      }
      string strSQL3 = "SELECT TIPMOV FROM TIPMOVCAU " + " WHERE CODCAU ='" + CODCAUSAN + "' AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
      string str3 = dataLayer.GetDataTable(strSQL3).Rows[0]["TIPMOV"]?.ToString() ?? "";
      if (str3.Trim() == "SAN_RD" | str3.Trim() == "ANN_SAN_RD")
        str1 = "ritardata";
      else if (str3.Trim() == "SAN_MD" | str3.Trim() == "ANN_SAN_MD")
        str1 = "mancata";
      cb.BeginText();
      cb.SetFontAndSize(font2, 7.75f);
      string text;
      if (DATSANANN == "")
        text = "Nota sanzione per " + str1 + " denuncia arretrati n. " + NUMSAN + " emessa il " + DATSAN + " ";
      else
        text = "Nota di annullamento n. " + NUMSANANN.Trim() + " emessa il " + DATSANANN + " relativa a Nota sanzione per " + str1 + " denuncia arretrati n. " + NUMSAN.Trim() + " emessa il " + DATSAN;
      cb.ShowTextAligned(0, text, 17f, 495f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 7.75f);
      cb.ShowTextAligned(0, "Posizione assicurativa " + CODPOS.ToString(), 17f, 480f, 0.0f);
      cb.EndText();
    }

    private void ScriviPieDiPaginaSanzioneArretrati(ref PdfContentByte cb)
    {
      DataTable dataTable1 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      cb.BeginText();
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.SetFontAndSize(font1, 5.25f);
      cb.ShowTextAligned(0, "Legenda causali:", 10f, 71f, 0.0f);
      cb.EndText();
      int y = 64;
      string strSQL = "SELECT DISTINCT CODCAU, VALUE(DESCAUREP, ' ') AS DESC FROM TIPMOVCAU WHERE TIPMOV IN('SAN_RD','SAN_MD','ANN_SAN_RD','ANN_SAN_MD') ORDER BY CODCAU ASC";
      DataTable dataTable2 = dataLayer.GetDataTable(strSQL);
      if (dataTable2.Rows.Count > 0)
      {
        for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
        {
          if (dataTable2.Rows[index]["DESC"].ToString() != "")
          {
            string str = dataTable2.Rows[index]["CODCAU"]?.ToString() + " " + dataTable2.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[index]["DESC"].ToString().Length - 1);
            cb.BeginText();
            BaseFont font2 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
            cb.SetFontAndSize(font2, 5.25f);
            cb.ShowTextAligned(0, str.Trim(), 10f, (float) y, 0.0f);
            cb.EndText();
          }
          else
          {
            string text = "";
            cb.BeginText();
            BaseFont font3 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
            cb.SetFontAndSize(font3, 5.25f);
            cb.ShowTextAligned(0, text, 10f, (float) y, 0.0f);
            cb.EndText();
          }
          y -= 7;
        }
      }
      else
      {
        for (int index = 0; index <= 5; ++index)
        {
          cb.BeginText();
          BaseFont font4 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
          cb.SetFontAndSize(font4, 5.25f);
          cb.ShowTextAligned(0, "", 10f, (float) y, 0.0f);
          cb.EndText();
          y -= 7;
        }
      }
      cb.BeginText();
      BaseFont font5 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.SetFontAndSize(font5, 8.25f);
      cb.ShowTextAligned(1, "Per informazioni telefonare al Call Center 800.010270 - 800.313231", 420f, 10f, 0.0f);
      cb.EndText();
    }

    private void ScriviTestataSanzioneArretrati(
      ref iTextSharp.text.Table tabTestata,
      ref Cell Cell,
      ref iTextSharp.text.Font fontRigaTest)
    {
      Cell = new Cell((IElement) new Phrase("Matricola", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      tabTestata.AddCell(Cell, 0, 0);
      Cell = new Cell((IElement) new Phrase("Cognome e Nome", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 0;
      Cell.BorderWidth = 0.0f;
      tabTestata.AddCell(Cell, 0, 1);
      Cell = new Cell((IElement) new Phrase("Anno", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      tabTestata.AddCell(Cell, 0, 2);
      Cell = new Cell((IElement) new Phrase("Periodo", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 3);
      Cell = new Cell((IElement) new Phrase("Retr. imponibile", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 2;
      Cell.BorderWidth = 0.0f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 5);
      Cell = new Cell((IElement) new Phrase("Aliquota", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      tabTestata.AddCell(Cell, 0, 7);
      Cell = new Cell((IElement) new Phrase("Contributi", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 2;
      Cell.BorderWidth = 0.0f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 8);
      Cell = new Cell((IElement) new Phrase("GG.Ritardo", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      tabTestata.AddCell(Cell, 0, 10);
      Cell = new Cell((IElement) new Phrase("Tasso", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      tabTestata.AddCell(Cell, 0, 11);
      Cell = new Cell((IElement) new Phrase("Importo Sanzione", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 2;
      Cell.BorderWidth = 0.0f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 12);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 0);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 1);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 2);
      Cell = new Cell((IElement) new Phrase("(Dal)", fontRigaTest));
      Cell.VerticalAlignment = 4;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 3);
      Cell = new Cell((IElement) new Phrase("(Al)", fontRigaTest));
      Cell.VerticalAlignment = 4;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 4);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 5);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 6);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 7);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 8);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 9);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 10);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 11);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 12);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 13);
    }

    private void scriviNumPagSanzioneArretrati(ref PdfContentByte cb, int cntNumPag, int TotPag)
    {
      string text = "Pag. " + cntNumPag.ToString() + " di " + TotPag.ToString();
      cb.BeginText();
      BaseFont font = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.SetFontAndSize(font, 7.25f);
      cb.ShowTextAligned(1, text, 800f, 10f, 0.0f);
      cb.EndText();
    }

    private void ScriviTotaliSanzioneArretrati(
      ref Document document,
      ref iTextSharp.text.Table tabFooter,
      ref Cell CellFooter,
      string IMPSANDET,
      string SANSOTSOG,
      string CODCAUSAN,
      ref PdfContentByte cb,
      string DATSANANN)
    {
      DataTable dataTable1 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      string strSQL = "SELECT VALUE(TASSO, 0.00) AS TASSO, TIPMOV, DESCAUREP AS DESC FROM TIPMOVCAU " + " WHERE CODCAU ='" + CODCAUSAN + "' AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
      dataTable1.Clear();
      DataTable dataTable2 = dataLayer.GetDataTable(strSQL);
      string str1 = dataTable2.Rows.Count <= 0 ? "" : (!(dataTable2.Rows[0]["DESC"].ToString() != "") ? "" : (!(DATSANANN == "") ? dataTable2.Rows[0]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[0]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[0]["DESC"].ToString().Length - 1) + " annullate" : dataTable2.Rows[0]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[0]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[0]["DESC"].ToString().Length - 1)));
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      iTextSharp.text.Font font2 = new iTextSharp.text.Font(font1, 7.75f, 0);
      iTextSharp.text.Font font3 = new iTextSharp.text.Font(font1, 7.75f, 1);
      iTextSharp.text.Font font4 = new iTextSharp.text.Font(font1, 7.75f, 0, Color.WHITE);
      CellFooter = new Cell((IElement) new Phrase(str1, font2));
      CellFooter.HorizontalAlignment = 0;
      CellFooter.VerticalAlignment = 6;
      CellFooter.BorderWidth = 0.0f;
      tabFooter.AddCell(CellFooter, 0, 0);
      CellFooter = new Cell((IElement) new Phrase("€", font2));
      CellFooter.HorizontalAlignment = 2;
      CellFooter.VerticalAlignment = 6;
      CellFooter.BorderWidth = 0.0f;
      tabFooter.AddCell(CellFooter, 0, 1);
      string str2 = !(SANSOTSOG.ToString().Trim() == "S") ? Convert.ToDecimal(IMPSANDET).ToString("#,##0.#0") : "0,00";
      CellFooter = new Cell((IElement) new Phrase(str2, font3));
      CellFooter.HorizontalAlignment = 2;
      CellFooter.VerticalAlignment = 6;
      CellFooter.BorderWidth = 0.0f;
      tabFooter.AddCell(CellFooter, 0, 2);
      CellFooter = new Cell((IElement) new Phrase("0", font4));
      CellFooter.BorderWidth = 0.0f;
      CellFooter.Colspan = 3;
      tabFooter.AddCell(CellFooter, 1, 0);
      CellFooter = new Cell((IElement) new Phrase("0", font4));
      CellFooter.BorderWidth = 0.0f;
      CellFooter.Colspan = 3;
      tabFooter.AddCell(CellFooter, 2, 0);
      CellFooter = new Cell((IElement) new Phrase("0", font4));
      CellFooter.BorderWidth = 0.0f;
      CellFooter.Colspan = 3;
      tabFooter.AddCell(CellFooter, 3, 0);
      CellFooter = new Cell((IElement) new Phrase("0", font4));
      CellFooter.BorderWidth = 0.0f;
      CellFooter.Colspan = 3;
      tabFooter.AddCell(CellFooter, 4, 0);
      CellFooter = new Cell((IElement) new Phrase("0", font4));
      CellFooter.BorderWidth = 0.0f;
      CellFooter.Colspan = 3;
      tabFooter.AddCell(CellFooter, 5, 0);
      iTextSharp.text.Font font5 = new iTextSharp.text.Font(BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false), 6f, 0);
      if (!(DATSANANN == ""))
        return;
      CellFooter = new Cell((IElement) new Phrase("Da versare entro 30 giorni dalla data di emissione tramite gli allegati M.Av.", font5));
      CellFooter.HorizontalAlignment = 2;
      CellFooter.VerticalAlignment = 6;
      CellFooter.BorderWidth = 0.0f;
      CellFooter.Colspan = 3;
      tabFooter.AddCell(CellFooter, 6, 0);
    }

    public void CreaStampaSanzioneDipa(ref DataTable dtSanzione, string strPath)
    {
      Document document = new Document(PageSize.A4.Rotate(), 15f, 15f, 15f, 15f);
      PdfWriter instance = PdfWriter.GetInstance(document, (Stream) new FileStream(strPath, FileMode.Create));
      int index1 = 0;
      int num1 = 1;
      int num2 = 0;
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataTable dataTable3 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      string NUMSAN = "";
      string DATSAN = "";
      string IMPSANDET = "";
      string SANSOTSOG = "";
      string DATCHI = "";
      DateTimeFormatInfo dateTimeFormat = new CultureInfo("it-IT", false).DateTimeFormat;
      int cntNumPag = 1;
      iTextSharp.text.Table tabFooter = new iTextSharp.text.Table(3, 1);
      Cell CellFooter = new Cell();
      tabFooter.WidthPercentage = 35f;
      tabFooter.AutoFillEmptyCells = true;
      tabFooter.Alignment = 2;
      tabFooter.BorderWidth = 0.0f;
      tabFooter.BorderColor = Color.WHITE;
      int[] widths1 = new int[3]{ 59, 19, 22 };
      tabFooter.SetWidths(widths1);
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      BaseFont font2 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
      BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAI"), "Cp1252", false);
      int int32_1 = Convert.ToInt32(dtSanzione.Rows[0]["PRODEN"]);
      int int32_2 = Convert.ToInt32(dtSanzione.Rows[0]["CODPOS"]);
      int int32_3 = Convert.ToInt32(dtSanzione.Rows[0]["ANNDEN"]);
      int int32_4 = Convert.ToInt32(dtSanzione.Rows[0]["MESDEN"]);
      string PERIODO = dateTimeFormat.MonthNames[int32_4 - 1].ToString().ToUpper() + " " + int32_3.ToString();
      document.Open();
      string strSQL1 = "SELECT SANSOTSOG, DATSANANN, NUMSAN, DATSAN, IMPDIS, IMPSANDET, NUMSANANN, CODCAUSAN, DATCHI, DATCONMOV" + " FROM  DENTES " + " WHERE ANNDEN = " + int32_3.ToString() + " AND MESDEN = " + int32_4.ToString() + " AND PRODEN = " + int32_1.ToString() + " AND CODPOS = " + int32_2.ToString();
      DataTable dataTable4 = dataLayer.GetDataTable(strSQL1);
      if (dataTable4.Rows.Count > 0)
      {
        NUMSAN = dataTable4.Rows[0]["NUMSAN"].ToString().Trim();
        IMPSANDET = dataTable4.Rows[0]["IMPSANDET"].ToString();
        if (!DBNull.Value.Equals(dataTable4.Rows[0]["DATSAN"]))
        {
          DATSAN = Convert.ToString(dataTable4.Rows[0]["DATSAN"]);
          Convert.ToDateTime(DATSAN);
        }
        Convert.ToString(Convert.ToDecimal(dataTable4.Rows[0]["IMPDIS"]) + Convert.ToDecimal(dataTable4.Rows[0]["IMPSANDET"]));
        SANSOTSOG = dataTable4.Rows[0]["SANSOTSOG"].ToString().Trim();
        DATCHI = dataTable4.Rows[0]["DATCHI"]?.ToString() ?? "";
      }
      string CODCAUSAN = dataTable4.Rows[0]["CODCAUSAN"].ToString();
      PdfContentByte directContent = instance.DirectContent;
      BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
      string str1 = dataTable4.Rows[0]["DATSANANN"].ToString();
      string DATSANANN = !(str1 != "") ? "" : Convert.ToString(str1);
      string NUMSANANN = dataTable4.Rows[0]["NUMSANANN"].ToString();
      this.ScriviIntestazioneSanzioneDipa(ref directContent, ref document, Convert.ToInt32(dtSanzione.Rows[index1]["CODPOS"]), PERIODO, NUMSAN, DATSAN, DATSANANN, NUMSANANN, CODCAUSAN, DATCHI, dataTable4.Rows[0]["DATCONMOV"]?.ToString() ?? "");
      this.ScriviPieDiPaginaSanzioneDipa(ref directContent);
      Cell Cell = new Cell();
      iTextSharp.text.Table tabTestata = new iTextSharp.text.Table(13, 2);
      tabTestata.AutoFillEmptyCells = true;
      tabTestata.WidthPercentage = 100f;
      tabTestata.BorderWidth = 0.0f;
      tabTestata.Cellspacing = 0.0f;
      tabTestata.Cellpadding = 2f;
      int[] widths2 = new int[13]
      {
        7,
        25,
        5,
        5,
        4,
        8,
        8,
        4,
        8,
        9,
        5,
        4,
        8
      };
      tabTestata.SetWidths(widths2);
      iTextSharp.text.Font fontRigaTest = new iTextSharp.text.Font(font2, 6.75f, 0);
      iTextSharp.text.Font font3 = new iTextSharp.text.Font(font1, 7.75f, 0);
      this.ScriviTestataSanzioneDipa(ref tabTestata, ref Cell, ref fontRigaTest);
      string strSQL2 = " SELECT DENDET.CODPOS," + " TRIM(ISCT.COG) || ' ' || TRIM(ISCT.NOM) AS COGNOM," + " DENDET.MAT," + " DENDET.ANNDEN," + " DENDET.MESDEN," + " DENDET.PRODEN," + " DENDET.IMPRET," + " DENDET.IMPCON," + " DENDET.ALIQUOTA," + " DENDET.DAL," + " DENDET.AL," + " DENDET.DATINISAN," + " DENDET.DATFINSAN," + " DENDET.IMPSANDET AS IMPSAN," + " DENDET.TASSAN" + " FROM DENDET" + " INNER JOIN ISCT ON" + " ISCT.MAT = DENDET.MAT" + " WHERE DENDET.ANNDEN = " + int32_3.ToString() + " AND DENDET.MESDEN = " + int32_4.ToString() + " AND DENDET.PRODEN = " + int32_1.ToString() + " AND DENDET.CODPOS = " + int32_2.ToString() + " AND DENDET.TIPMOV = 'DP'" + " ORDER BY ISCT.COG, ISCT.NOM, DENDET.MAT, DENDET.DAL";
      DataTable dataTable5 = dataLayer.GetDataTable(strSQL2);
      int TotPag = !((Decimal) (dataTable5.Rows.Count - dataTable5.Rows.Count / 14 * 14) <= 11M) ? dataTable5.Rows.Count / 18 + 2 : dataTable5.Rows.Count / 18 + 1;
      this.scriviNumPagSanzioneDipa(ref directContent, cntNumPag, TotPag);
      for (int index2 = 0; index2 <= dataTable5.Rows.Count - 1; ++index2)
      {
        if (num1 == 19)
        {
          tabTestata.Offset = 105f;
          document.Add((IElement) tabTestata);
          tabTestata.DeleteAllRows();
          document.NewPage();
          this.ScriviIntestazioneSanzioneDipa(ref directContent, ref document, Convert.ToInt32(dtSanzione.Rows[index1]["CODPOS"]), PERIODO, NUMSAN, DATSAN, DATSANANN, NUMSANANN, CODCAUSAN, DATCHI, dataTable4.Rows[0]["DATCONMOV"]?.ToString() ?? "");
          this.ScriviPieDiPaginaSanzioneDipa(ref directContent);
          this.ScriviTestataSanzioneDipa(ref tabTestata, ref Cell, ref fontRigaTest);
          ++cntNumPag;
          num2 = index2;
          num1 = 1;
          this.scriviNumPagSanzioneDipa(ref directContent, cntNumPag, TotPag);
        }
        string str2 = dataTable5.Rows[index2]["DAL"].ToString().Substring(0, 5);
        string str3 = dataTable5.Rows[index2]["AL"].ToString().Substring(0, 5);
        Decimal num3 = Convert.ToDecimal(dataTable5.Rows[index2]["IMPRET"]);
        string str4 = num3.ToString("#,##0.#0");
        num3 = Convert.ToDecimal(dataTable5.Rows[index2]["IMPCON"]);
        string str5 = num3.ToString("#,##0.#0");
        num3 = Convert.ToDecimal(dataTable5.Rows[index2]["IMPSAN"]);
        string str6 = num3.ToString("#,##0.#0");
        Cell aCell1 = new Cell((IElement) new Phrase(dataTable5.Rows[index2]["MAT"].ToString().Trim(), font3));
        aCell1.VerticalAlignment = 5;
        aCell1.HorizontalAlignment = 1;
        aCell1.BorderWidth = 0.0f;
        aCell1.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(aCell1, 2 + index2 - num2, 0);
        Cell aCell2 = new Cell((IElement) new Phrase(dataTable5.Rows[index2]["COGNOM"].ToString().Trim().Substring(0, 32), font3));
        aCell2.HorizontalAlignment = 0;
        aCell2.BorderWidth = 0.0f;
        aCell2.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(aCell2, 2 + index2 - num2, 1);
        Cell aCell3 = new Cell((IElement) new Phrase(str2, font3));
        aCell3.HorizontalAlignment = 1;
        aCell3.BorderWidth = 0.0f;
        aCell3.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(aCell3, 2 + index2 - num2, 2);
        Cell aCell4 = new Cell((IElement) new Phrase(str3, font3));
        aCell4.HorizontalAlignment = 1;
        aCell4.BorderWidth = 0.0f;
        aCell4.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(aCell4, 2 + index2 - num2, 3);
        Cell aCell5 = new Cell((IElement) new Phrase("€ ", font3));
        aCell5.HorizontalAlignment = 2;
        aCell5.BorderWidth = 0.0f;
        aCell5.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(aCell5, 2 + index2 - num2, 4);
        Cell aCell6 = new Cell((IElement) new Phrase(str4, font3));
        aCell6.HorizontalAlignment = 2;
        aCell6.BorderWidth = 0.0f;
        aCell6.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(aCell6, 2 + index2 - num2, 5);
        Cell aCell7 = new Cell((IElement) new Phrase(dataTable5.Rows[index2]["ALIQUOTA"]?.ToString() ?? "", font3));
        aCell7.HorizontalAlignment = 1;
        aCell7.BorderWidth = 0.0f;
        aCell7.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(aCell7, 2 + index2 - num2, 6);
        Cell aCell8 = new Cell((IElement) new Phrase("€ ", font3));
        aCell8.HorizontalAlignment = 1;
        aCell8.BorderWidth = 0.0f;
        aCell8.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(aCell8, 2 + index2 - num2, 7);
        Cell aCell9 = new Cell((IElement) new Phrase(str5, font3));
        aCell9.HorizontalAlignment = 2;
        aCell9.BorderWidth = 0.0f;
        aCell9.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(aCell9, 2 + index2 - num2, 8);
        if (dataTable5.Rows[index2]["DATFINSAN"].ToString().Trim() != "" | dataTable5.Rows[index2]["DATINISAN"].ToString().Trim() != "")
        {
          Cell aCell10 = new Cell((IElement) new Phrase(Convert.ToDateTime(dataTable5.Rows[index2]["DATFINSAN"]).Subtract(Convert.ToDateTime(dataTable5.Rows[index2]["DATINISAN"])).Days.ToString(), font3));
          aCell10.HorizontalAlignment = 1;
          aCell10.BorderWidth = 0.0f;
          aCell10.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(aCell10, 2 + index2 - num2, 9);
        }
        else
        {
          Cell aCell11 = new Cell((IElement) new Phrase("", font3));
          aCell11.HorizontalAlignment = 1;
          aCell11.BorderWidth = 0.0f;
          aCell11.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(aCell11, 2 + index2 - num2, 9);
        }
        Cell aCell12 = new Cell((IElement) new Phrase(dataTable5.Rows[index2]["TASSAN"]?.ToString() + " %", font3));
        aCell12.HorizontalAlignment = 1;
        aCell12.BorderWidth = 0.0f;
        aCell12.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(aCell12, 2 + index2 - num2, 10);
        Cell aCell13 = new Cell((IElement) new Phrase("€ ", font3));
        aCell13.HorizontalAlignment = 2;
        aCell13.BorderWidth = 0.0f;
        aCell13.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(aCell13, 2 + index2 - num2, 11);
        Cell = new Cell((IElement) new Phrase(str6, font3));
        Cell.HorizontalAlignment = 2;
        Cell.BorderWidth = 0.0f;
        Cell.BorderWidthBottom = 0.5f;
        tabTestata.AddCell(Cell, 2 + index2 - num2, 12);
        ++num1;
      }
      tabTestata.Offset = 105f;
      document.Add((IElement) tabTestata);
      if (num1 > 12)
      {
        document.NewPage();
        this.ScriviIntestazioneSanzioneDipa(ref directContent, ref document, Convert.ToInt32(dtSanzione.Rows[index1]["CODPOS"]), PERIODO, NUMSAN, DATSAN, DATSANANN, NUMSANANN, CODCAUSAN, DATCHI, dataTable4.Rows[0]["DATCONMOV"]?.ToString() ?? "");
        this.ScriviPieDiPaginaSanzioneDipa(ref directContent);
        this.ScriviTestataSanzioneDipa(ref tabTestata, ref Cell, ref fontRigaTest);
        this.scriviNumPagSanzioneDipa(ref directContent, cntNumPag + 1, TotPag);
        this.ScriviTotaliSanzioneDipa(ref document, ref tabFooter, ref CellFooter, IMPSANDET, SANSOTSOG, CODCAUSAN, ref directContent, DATSANANN);
        tabFooter.Offset = 120f;
        document.Add((IElement) tabFooter);
      }
      else
      {
        this.ScriviTotaliSanzioneDipa(ref document, ref tabFooter, ref CellFooter, IMPSANDET, SANSOTSOG, CODCAUSAN, ref directContent, DATSANANN);
        tabFooter.Offset = 20f;
        document.Add((IElement) tabFooter);
      }
      document.Close();
      instance.Close();
      Process.Start(strPath);
    }

    private void ScriviIntestazioneSanzioneDipa(
      ref PdfContentByte cb,
      ref Document document,
      int CODPOS,
      string PERIODO,
      string NUMSAN,
      string DATSAN,
      string DATSANANN,
      string NUMSANANN,
      string CODCAUSAN,
      string DATCHI,
      string DATCONMOV)
    {
      DataTable dataTable1 = new DataTable();
      string str1 = "";
      DataLayer dataLayer = new DataLayer();
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
      BaseFont font2 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      Image instance = Image.GetInstance(this.EstraiFilePGM_Byte("CNT_LOGO"));
      instance.ScaleAbsolute(103f, 51f);
      instance.SetAbsolutePosition(82f, 545f);
      document.Add((IElement) instance);
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(1, "FONDAZIONE E.N.P.A.I.A.", 395f, 580f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(1, "ENTE NAZIONALE DI PREVIDENZA PER GLI ADDETTI E PER GLI", 395f, 570f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(1, "IMPIEGATI IN AGRICOLTURA", 395f, 560f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Viale Beethoven, 48 - 00144 ROMA", 395f, 550f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Call Center 800.010270 - Fax 06/5914444 - 06/5458385", 395f, 540f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Internet: www.enpaia.it       Email: info@enpaia.it", 395f, 530f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Ufficio contributi e riscossione", 133f, 542f, 0.0f);
      cb.EndText();
      string strSQL1 = " SELECT  AZI.RAGSOC, AZI.CODPOS, DUG.DENDUG, INDSED.IND, INDSED.NUMCIV, " + " INDSED.CAP, INDSED.DENLOC, INDSED.SIGPRO, INDSED.DENSTAEST, " + " INDSED.CODCOM, '' AS DENCOM " + " FROM  AZI " + " INNER JOIN INDSED ON " + " INDSED.CODPOS = AZI.CODPOS " + " LEFT JOIN DUG ON INDSED.CODDUG = DUG.CODDUG " + " WHERE INDSED.TIPIND=1 " + " AND INDSED.DATINI <= " + this.Module_DB2Date(Convert.ToDateTime(DATCONMOV)) + " AND AZI.CODPOS= " + CODPOS.ToString() + " AND INDSED.DATCOM = (SELECT MAX(DATCOM) FROM INDSED WHERE " + " INDSED.CODPOS = " + CODPOS.ToString() + " AND TIPIND = 1 AND INDSED.DATINI <= " + this.Module_DB2Date(Convert.ToDateTime(DATCONMOV)) + ")" + " ORDER BY AZI.CODPOS, INDSED.DATCOM DESC FETCH FIRST 1 ROWS ONLY";
      DataTable dataTable2 = dataLayer.GetDataTable(strSQL1);
      if (dataTable2.Rows.Count > 0)
      {
        if (dataTable2.Rows[0]["CODCOM"].ToString().Trim() != "")
        {
          string strSQL2 = "SELECT VALUE(DENCOM,'') AS DENCOM FROM CODCOM WHERE CODCOM = " + this.objNet.DoublePeakForSql(dataTable2.Rows[0]["CODCOM"].ToString().Trim());
          string str2 = dataLayer.GetDataTable(strSQL2).Rows[0]["DENCOM"].ToString();
          dataTable2.Rows[0]["DENCOM"] = (object) str2;
        }
        else
          dataTable2.Rows[0]["DENCOM"] = (object) "";
        string text1 = dataTable2.Rows[0]["RAGSOC"].ToString().Trim();
        string text2;
        if (DBNull.Value.Equals(dataTable2.Rows[0]["NUMCIV"]) | dataTable2.Rows[0]["NUMCIV"].ToString().Trim() == "")
          text2 = dataTable2.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable2.Rows[0]["IND"].ToString().Trim();
        else
          text2 = dataTable2.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable2.Rows[0]["IND"].ToString().Trim() + ", " + dataTable2.Rows[0]["NUMCIV"].ToString().Trim();
        string text3;
        if (!DBNull.Value.Equals(dataTable2.Rows[0]["DENSTAEST"]))
          text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENSTAEST"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
        else if (dataTable2.Rows[0]["DENLOC"].ToString().Trim() != dataTable2.Rows[0]["DENCOM"].ToString().Trim())
        {
          if (dataTable2.Rows[0]["DENLOC"].ToString().Trim() == "")
            text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENCOM"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
          else
            text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENLOC"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
        }
        else
          text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENCOM"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
        BaseFont font3 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
        cb.BeginText();
        cb.SetFontAndSize(font3, 8.25f);
        cb.ShowTextAligned(0, text1, 565f, 552f, 0.0f);
        cb.EndText();
        cb.BeginText();
        cb.SetFontAndSize(font3, 8.25f);
        cb.ShowTextAligned(0, text2, 565f, 542f, 0.0f);
        cb.EndText();
        cb.BeginText();
        cb.SetFontAndSize(font3, 8.25f);
        cb.ShowTextAligned(0, text3, 565f, 532f, 0.0f);
        cb.EndText();
      }
      string strSQL3 = "SELECT TIPMOV FROM TIPMOVCAU " + " WHERE CODCAU ='" + CODCAUSAN + "' AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
      string str3 = dataLayer.GetDataTable(strSQL3).Rows[0]["TIPMOVCAU"]?.ToString() ?? "";
      if (str3.Trim() == "SAN_RD" | str3.Trim() == "ANN_SAN_RD")
        str1 = "ritardata";
      else if (str3.Trim() == "SAN_MD" | str3.Trim() == "ANN_SAN_MD")
        str1 = "mancata";
      cb.BeginText();
      cb.SetFontAndSize(font2, 7.75f);
      string text;
      if (DATSANANN == "")
        text = "Nota sanzione per " + str1 + " denuncia n. " + NUMSAN + " emessa il " + DATSAN + " ";
      else
        text = "Nota di annullamento n. " + NUMSANANN.Trim() + " emessa il " + DATSANANN + " relativa a Nota sanzione per " + str1 + " denuncia n. " + NUMSAN.Trim() + " emessa il " + DATSAN;
      cb.ShowTextAligned(0, text, 17f, 495f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 7.75f);
      cb.ShowTextAligned(0, "Posizione assicurativa " + CODPOS.ToString(), 17f, 480f, 0.0f);
      cb.EndText();
    }

    private void ScriviPieDiPaginaSanzioneDipa(ref PdfContentByte cb)
    {
      DataTable dataTable1 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      cb.BeginText();
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.SetFontAndSize(font1, 5.25f);
      cb.ShowTextAligned(0, "Legenda causali:", 10f, 71f, 0.0f);
      cb.EndText();
      int y = 64;
      string strSQL = "SELECT DISTINCT CODCAU, VALUE(DESCAUREP, ' ') AS DESC FROM TIPMOVCAU WHERE TIPMOV IN('SAN_RD','SAN_MD','ANN_SAN_RD','ANN_SAN_MD') ORDER BY CODCAU ASC";
      DataTable dataTable2 = dataLayer.GetDataTable(strSQL);
      if (dataTable2.Rows.Count > 0)
      {
        for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
        {
          if (dataTable2.Rows[index]["DESC"].ToString() != "")
          {
            string str = dataTable2.Rows[index]["CODCAU"]?.ToString() + " " + dataTable2.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[index]["DESC"].ToString().Length - 1);
            cb.BeginText();
            BaseFont font2 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
            cb.SetFontAndSize(font2, 5.25f);
            cb.ShowTextAligned(0, str.Trim(), 10f, (float) y, 0.0f);
            cb.EndText();
          }
          else
          {
            string text = "";
            cb.BeginText();
            BaseFont font3 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
            cb.SetFontAndSize(font3, 5.25f);
            cb.ShowTextAligned(0, text, 10f, (float) y, 0.0f);
            cb.EndText();
          }
          y -= 7;
        }
      }
      else
      {
        for (int index = 0; index <= 5; ++index)
        {
          cb.BeginText();
          BaseFont font4 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
          cb.SetFontAndSize(font4, 5.25f);
          cb.ShowTextAligned(0, "", 10f, (float) y, 0.0f);
          cb.EndText();
          y -= 7;
        }
      }
      cb.BeginText();
      BaseFont font5 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.SetFontAndSize(font5, 8.25f);
      cb.ShowTextAligned(1, "Per informazioni telefonare al Call Center 800.010270 - 800.313231", 420f, 10f, 0.0f);
      cb.EndText();
    }

    private void ScriviTestataSanzioneDipa(
      ref iTextSharp.text.Table tabTestata,
      ref Cell Cell,
      ref iTextSharp.text.Font fontRigaTest)
    {
      Cell = new Cell((IElement) new Phrase("Matricola", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      tabTestata.AddCell(Cell, 0, 0);
      Cell = new Cell((IElement) new Phrase("Cognome e Nome", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 0;
      Cell.BorderWidth = 0.0f;
      tabTestata.AddCell(Cell, 0, 1);
      Cell = new Cell((IElement) new Phrase("Periodo", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 2);
      Cell = new Cell((IElement) new Phrase("Retr. imponibile", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 2;
      Cell.BorderWidth = 0.0f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 4);
      Cell = new Cell((IElement) new Phrase("Aliquota", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      tabTestata.AddCell(Cell, 0, 6);
      Cell = new Cell((IElement) new Phrase("Contributi", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 2;
      Cell.BorderWidth = 0.0f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 7);
      Cell = new Cell((IElement) new Phrase("GG. Ritardo", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      tabTestata.AddCell(Cell, 0, 9);
      Cell = new Cell((IElement) new Phrase("Tasso", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      tabTestata.AddCell(Cell, 0, 10);
      Cell = new Cell((IElement) new Phrase("Importo Sanzione", fontRigaTest));
      Cell.VerticalAlignment = 5;
      Cell.HorizontalAlignment = 2;
      Cell.BorderWidth = 0.0f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 11);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 0);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 1);
      Cell = new Cell((IElement) new Phrase("(Dal)", fontRigaTest));
      Cell.VerticalAlignment = 4;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 2);
      Cell = new Cell((IElement) new Phrase("(Al)", fontRigaTest));
      Cell.VerticalAlignment = 4;
      Cell.HorizontalAlignment = 1;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 3);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 4);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 5);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 6);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 7);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 8);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 9);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 10);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 11);
      Cell = new Cell("");
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 1, 12);
    }

    private void scriviNumPagSanzioneDipa(ref PdfContentByte cb, int cntNumPag, int TotPag)
    {
      string text = "Pag. " + cntNumPag.ToString() + " di " + TotPag.ToString();
      cb.BeginText();
      BaseFont font = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.SetFontAndSize(font, 7.25f);
      cb.ShowTextAligned(1, text, 800f, 10f, 0.0f);
      cb.EndText();
    }

    private void ScriviTotaliSanzioneDipa(
      ref Document document,
      ref iTextSharp.text.Table tabFooter,
      ref Cell CellFooter,
      string IMPSANDET,
      string SANSOTSOG,
      string CODCAUSAN,
      ref PdfContentByte cb,
      string DATSANANN)
    {
      DataTable dataTable1 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      string strSQL = "SELECT VALUE(TASSO, 0.00) AS TASSO, TIPMOV, DESCAUREP AS DESC FROM TIPMOVCAU " + " WHERE CODCAU ='" + CODCAUSAN + "' AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
      dataTable1.Clear();
      DataTable dataTable2 = dataLayer.GetDataTable(strSQL);
      string str1 = dataTable2.Rows.Count <= 0 ? "" : (!(dataTable2.Rows[0]["DESC"].ToString() != "") ? "" : (!(DATSANANN == "") ? dataTable2.Rows[0]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[0]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[0]["DESC"].ToString().Length - 1) + " annullate" : dataTable2.Rows[0]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[0]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[0]["DESC"].ToString().Length - 1)));
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      iTextSharp.text.Font font2 = new iTextSharp.text.Font(font1, 7.75f, 0);
      iTextSharp.text.Font font3 = new iTextSharp.text.Font(font1, 7.75f, 1);
      iTextSharp.text.Font font4 = new iTextSharp.text.Font(font1, 7.75f, 0, Color.WHITE);
      CellFooter = new Cell((IElement) new Phrase(str1, font2));
      CellFooter.HorizontalAlignment = 0;
      CellFooter.VerticalAlignment = 6;
      CellFooter.BorderWidth = 0.0f;
      tabFooter.AddCell(CellFooter, 0, 0);
      CellFooter = new Cell((IElement) new Phrase("€", font2));
      CellFooter.HorizontalAlignment = 2;
      CellFooter.VerticalAlignment = 6;
      CellFooter.BorderWidth = 0.0f;
      tabFooter.AddCell(CellFooter, 0, 1);
      string str2 = !(SANSOTSOG.ToString().Trim() == "S") ? Convert.ToDecimal(IMPSANDET).ToString("#,##0.#0") : "0,00";
      CellFooter = new Cell((IElement) new Phrase(str2, font3));
      CellFooter.HorizontalAlignment = 2;
      CellFooter.VerticalAlignment = 6;
      CellFooter.BorderWidth = 0.0f;
      tabFooter.AddCell(CellFooter, 0, 2);
      CellFooter = new Cell((IElement) new Phrase("0", font4));
      CellFooter.BorderWidth = 0.0f;
      CellFooter.Colspan = 3;
      tabFooter.AddCell(CellFooter, 1, 0);
      CellFooter = new Cell((IElement) new Phrase("0", font4));
      CellFooter.BorderWidth = 0.0f;
      CellFooter.Colspan = 3;
      tabFooter.AddCell(CellFooter, 2, 0);
      CellFooter = new Cell((IElement) new Phrase("0", font4));
      CellFooter.BorderWidth = 0.0f;
      CellFooter.Colspan = 3;
      tabFooter.AddCell(CellFooter, 3, 0);
      CellFooter = new Cell((IElement) new Phrase("0", font4));
      CellFooter.BorderWidth = 0.0f;
      CellFooter.Colspan = 3;
      tabFooter.AddCell(CellFooter, 4, 0);
      CellFooter = new Cell((IElement) new Phrase("0", font4));
      CellFooter.BorderWidth = 0.0f;
      CellFooter.Colspan = 3;
      tabFooter.AddCell(CellFooter, 5, 0);
      iTextSharp.text.Font font5 = new iTextSharp.text.Font(BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false), 6f, 0);
      if (!(DATSANANN == ""))
        return;
      CellFooter = new Cell((IElement) new Phrase("Da versare entro 30 giorni dalla data di emissione tramite gli allegati M.Av.", font5));
      CellFooter.HorizontalAlignment = 2;
      CellFooter.VerticalAlignment = 6;
      CellFooter.BorderWidth = 0.0f;
      CellFooter.Colspan = 3;
      tabFooter.AddCell(CellFooter, 6, 0);
    }

    public void Module_WriteError(
      string strClassName,
      string strEvent,
      string strErrore,
      int intErr = 0,
      bool blnNoMessage = false,
      string strSQL_ERR = "")
    {
      IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
      this.objNet.strPcName = Dns.GetHostName();
      this.objNet.strIPAddress = ((IPAddress) hostEntry.AddressList.GetValue(0)).ToString();
      iDB2Connection iDb2Connection = new iDB2Connection(this.objNet.strConnection);
      try
      {
        iDb2Connection.Open();
        iDB2Command iDb2Command = new iDB2Command();
        string str1 = (!(strSQL_ERR != "") ? "INSERT INTO ERRORI (CODERR, DATA, ORA, DESERR, USERAPP, IP, NOMEPC, FORM, AMBIENTE) " : "INSERT INTO ERRORI (CODERR, DATA, ORA, DESERR, USERAPP, IP, NOMEPC, FORM, AMBIENTE, STACKTRACE) ") + "VALUES ( (SELECT VALUE (MAX(CODERR), 0 ) + 1 FROM ERRORI ) , CURRENT_DATE, CURRENT_TIME, " + this.objNet.DoublePeakForSql(strErrore) + "," + this.objNet.DoublePeakForSql(this.objNet.strUserCode) + "," + this.objNet.DoublePeakForSql(this.objNet.strIPAddress) + "," + this.objNet.DoublePeakForSql(this.objNet.strPcName) + "," + this.objNet.DoublePeakForSql(strClassName) + ", ";
        string str2 = !(strSQL_ERR != "") ? str1 + " 'I') " : str1 + " 'I', " + this.objNet.DoublePeakForSql(strSQL_ERR) + ") ";
        iDb2Command.Connection = iDb2Connection;
        iDb2Command.CommandType = CommandType.Text;
        iDb2Command.CommandText = str2;
        iDb2Command.ExecuteNonQuery();
        if (blnNoMessage)
          return;
        if (Application.StartupPath.ToUpper().IndexOf("PROTSRV01") > 0)
          this.objNet.MsgBoxError("Attenzione... si è verificato un errore. Contattare il CED");
        else
          this.objNet.MsgBoxError(strErrore);
      }
      catch (Exception ex)
      {
        this.objNet.MsgBoxError(ex.Message);
      }
      finally
      {
        iDb2Connection.Close();
      }
    }

    public void MODULE_STAMPA_DOCUMENTO_RETTIFICA(List<DENDET_Data> list_stampeRet, bool CONT = false)
    {
      DataTable dataTable1 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      DataTable dataTable2 = new ListtoDataTableConverter().ToDataTable<DENDET_Data>(list_stampeRet);
      try
      {
        Utile.GetDataSistema();
        string str1 = !(Application.StartupPath.ToString().Substring(Application.StartupPath.ToString().Length - 1) == "\\") ? Application.StartupPath + "\\" : Application.StartupPath;
        string str2 = this.PROT_PRES_GET_TEMP_PATH_APPLICATION() + "\\TEMP_ENPAIANET_RT_" + DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(".", "_").Replace(":", "") + ".pdf";
        if (dataTable2.Columns.Count == 3)
          this.CreaStampaRettifica(ref dataTable2, str2);
        else
          this.CreaStampaRettificaAnnullata(ref dataTable2, str2);
        if (!CONT)
          return;
        string strSQL = "UPDATE RETTES SET NOMFILPDF = " + DBMethods.DoublePeakForSql(this.FTP_Upload(18, str2, Path.GetFileName(str2), folderNow: true)) + ", " + " CODLINE = RIGHT('000000' || TRIM(CHAR(CODPOS)), 6) || SUBSTRING(TRIM(NUMMOV), 6, 2) || SUBSTRING(TRIM(NUMMOV), 1, 2) || RIGHT('000' || SUBSTRING(TRIM(NUMMOV), 9, 3), 3) " + " WHERE ANNRET=" + dataTable2.Rows[0]["ANNRET"]?.ToString() + " AND PRORET = " + dataTable2.Rows[0]["PRORET"]?.ToString() + " AND PRORETTES = " + dataTable2.Rows[0]["PRORETTES"]?.ToString();
        dataLayer.WriteData(strSQL, CommandType.Text);
      }
      catch (Exception ex)
      {
        StackTrace stackTrace = new StackTrace(ex);
        stackTrace.GetFrame(stackTrace.FrameCount - 1).ToString();
        this.Module_WriteError(nameof (MODULE_STAMPA_DOCUMENTO_RETTIFICA), stackTrace.GetFrame(stackTrace.FrameCount - 1).ToString(), ex.Message);
      }
    }

    public void CreaStampaRettifica(ref DataTable dtRettifica, string strPath)
    {
      Document document = new Document(PageSize.A4.Rotate(), 10f, 20f, 5f, 5f);
      PdfWriter instance = PdfWriter.GetInstance(document, (Stream) new FileStream(strPath, FileMode.Create));
      int num1 = 1;
      int num2 = 0;
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      int cntNumPag = 1;
      string FLGSAN = "N";
      document.Open();
      for (int index1 = 0; index1 <= dtRettifica.Rows.Count - 1; ++index1)
      {
        PdfContentByte directContent = instance.DirectContent;
        string strSQL1 = "SELECT RETTES.CODPOS, RETTES.IMPADDDEL, RETTES.SANSOTSOG, " + " RETTES.DATSAN, RETTES.IMPABB, RETTES.IMPASSCON," + " RETTES.NUMSAN, RETTES.DATCONMOV, RETTES.NUMMOV, RETTES.CODCAUSAN," + " RETTES.IMPCONDEL AS IMPCONTOT, RETTES.IMPSANDET AS IMPSANTOT," + " RETTES.TIPIMP, DAL_AL.DAL, DAL_AL.AL" + " FROM RETTES INNER JOIN " + "(SELECT MIN(DAL) AS DAL, MAX(AL) AS AL, ANNRET, PRORET, PRORETTES" + " FROM DENDET" + " GROUP BY ANNRET, PRORET, PRORETTES)" + " DAL_AL ON RETTES.ANNRET = DAL_AL.ANNRET " + " AND RETTES.PRORETTES = DAL_AL.PRORETTES " + " AND RETTES.PRORET = DAL_AL.PRORET " + " WHERE RETTES.ANNRET = " + dtRettifica.Rows[index1]["ANNRET"]?.ToString() + " AND RETTES.PRORETTES = " + dtRettifica.Rows[index1]["PRORETTES"]?.ToString() + " AND RETTES.PRORET = " + dtRettifica.Rows[index1]["PRORET"]?.ToString();
        DataTable dataTable3 = dataLayer.GetDataTable(strSQL1);
        BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
        if (index1 > 0)
        {
          cntNumPag = 1;
          num2 = 0;
          num1 = 1;
          document.NewPage();
        }
        string TIPIMP = dataTable3.Rows[0]["TIPIMP"].ToString().Trim();
        string str1 = dataTable3.Rows[0]["SANSOTSOG"].ToString().Trim();
        string IMPSANTOT = !(dataTable3.Rows[0]["NUMSAN"].ToString() == "") ? dataTable3.Rows[0]["IMPSANTOT"].ToString() : "0,00";
        if (TIPIMP == "+" && Convert.ToInt32(IMPSANTOT) > 0 && str1 != "S")
          FLGSAN = "S";
        this.ScriviIntestazioneRettifica(ref directContent, ref document, Convert.ToInt32(dataTable3.Rows[0]["CODPOS"]), dataTable3.Rows[0]["DATCONMOV"]?.ToString() ?? "");
        this.ScriviPieDiPaginaRettifica(ref directContent, dataTable3.Rows[0]["CODCAUSAN"].ToString());
        Cell Cell = new Cell();
        iTextSharp.text.Table tabTestata = new iTextSharp.text.Table(15, 1);
        tabTestata.WidthPercentage = 100f;
        tabTestata.AutoFillEmptyCells = true;
        tabTestata.BorderWidth = 0.0f;
        tabTestata.BorderColor = Color.WHITE;
        tabTestata.Cellpadding = 2f;
        int[] widths1 = new int[15]
        {
          8,
          19,
          5,
          4,
          7,
          5,
          6,
          4,
          7,
          10,
          2,
          6,
          9,
          2,
          6
        };
        tabTestata.SetWidths(widths1);
        Cell CellTotali = new Cell();
        iTextSharp.text.Table TabTotali = new iTextSharp.text.Table(4, 9);
        TabTotali.WidthPercentage = 45f;
        TabTotali.AutoFillEmptyCells = true;
        TabTotali.Alignment = 2;
        TabTotali.BorderWidth = 0.0f;
        TabTotali.BorderColor = Color.WHITE;
        int[] widths2 = new int[4]{ 30, 45, 3, 22 };
        TabTotali.SetWidths(widths2);
        iTextSharp.text.Font FontTestata = new iTextSharp.text.Font(BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false), 8.25f, 0);
        this.ScriviTestataRettifica(ref tabTestata, ref Cell, ref FontTestata, FLGSAN);
        string strSQL2 = "SELECT DENDET.MAT, DENDET.PRODEN, DENDET.ALIQUOTA, DENDET.ANNDEN, DENDET.MESDEN," + " DENDET.IMPRET, DENDET.ANNCOM, DENDET.NUMSAN, DENDET.IMPRETPRE, DENDET.IMPRETDEL, DENDET.IMPCONDEL, DENDET.IMPSANDET, " + " (SELECT TIPMOV FROM DENTES WHERE CODPOS = DENDET.CODPOS AND ANNDEN = DENDET.ANNDEN AND MESDEN = DENDET.MESDEN AND PRODEN = DENDET.PRODEN ) AS DENTES_TIPMOV," + " DENDET.CODPOS, ISCT.COG, ISCT.NOM, DENDET.DAL, DENDET.AL, DENDET.TASSAN" + " FROM DENDET INNER JOIN ISCT ON ISCT.MAT = DENDET.MAT" + " WHERE DENDET.ANNRET = " + dtRettifica.Rows[index1]["ANNRET"]?.ToString() + " And DENDET.PRORET = " + dtRettifica.Rows[index1]["PRORET"]?.ToString() + " And DENDET.PRORETTES = " + dtRettifica.Rows[index1]["PRORETTES"]?.ToString() + " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL" + " AND IMPCONDEL <> 0" + " ORDER BY ISCT.COG, ISCT.NOM, DENDET.DAL";
        DataTable dataTable4 = dataLayer.GetDataTable(strSQL2);
        BaseFont font = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
        iTextSharp.text.Font FontDettaglio = new iTextSharp.text.Font(font, 8.25f, 0);
        iTextSharp.text.Font FontNascondi = new iTextSharp.text.Font(font, 8.25f, 0, Color.WHITE);
        int TotPag = !((Decimal) (dataTable4.Rows.Count - dataTable4.Rows.Count / 18 * 18) <= 15M) ? dataTable4.Rows.Count / 18 + 2 : dataTable4.Rows.Count / 18 + 1;
        this.scriviNumPagRettifica(ref directContent, cntNumPag, TotPag);
        for (int index2 = 0; index2 <= dataTable4.Rows.Count - 1; ++index2)
        {
          if (Math.Round(Convert.ToDecimal(dataTable4.Rows[index2]["IMPRETDEL"]) * Convert.ToDecimal(dataTable4.Rows[index2]["ALIQUOTA"]) / Convert.ToDecimal(100.0), 2) != Convert.ToDecimal(dataTable4.Rows[index2]["IMPCONDEL"]))
            dataTable4.Rows[index2]["ALIQUOTA"] = (object) (Convert.ToDecimal(dataTable4.Rows[index2]["IMPCONDEL"]) * -1M / (Convert.ToDecimal((double) dataTable4.Rows[index2]["IMPRETPRE"]) * 100M));
          if (Convert.ToInt32(dataTable4.Rows[index2]["ALIQUOTA"]) < 0)
            dataTable4.Rows[index2]["ALIQUOTA"] = (object) (Convert.ToDecimal(dataTable4.Rows[index2]["ALIQUOTA"]) * -1M);
          dataTable4.Rows[index2]["ALIQUOTA"] = (object) Math.Round(Convert.ToDecimal(dataTable4.Rows[index2]["ALIQUOTA"]), 2);
          this.ScriviSanzioneRettifica(ref directContent, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["DATCONMOV"].ToString(), dataTable3.Rows[0]["NUMMOV"].ToString(), dataTable3.Rows[0]["DATSAN"].ToString(), dataTable3.Rows[0]["NUMSAN"].ToString(), dataTable4.Rows[index2]["MESDEN"].ToString(), dataTable4.Rows[index2]["ANNDEN"].ToString(), Convert.ToInt32(dataTable4.Rows[index2]["CODPOS"]), dataTable3.Rows[0]["DAL"].ToString(), dataTable3.Rows[0]["AL"].ToString());
          if (num1 == 19)
          {
            tabTestata.Offset = 108f;
            document.Add((IElement) tabTestata);
            tabTestata.DeleteAllRows();
            document.NewPage();
            this.ScriviIntestazioneRettifica(ref directContent, ref document, Convert.ToInt32(dataTable3.Rows[0]["CODPOS"]), dataTable3.Rows[0]["DATCONMOV"]?.ToString() ?? "");
            this.ScriviPieDiPaginaRettifica(ref directContent, dataTable3.Rows[0]["CODCAUSAN"].ToString());
            this.ScriviTestataRettifica(ref tabTestata, ref Cell, ref FontTestata, FLGSAN);
            ++cntNumPag;
            num2 = index2;
            num1 = 1;
            this.scriviNumPagRettifica(ref directContent, cntNumPag, TotPag);
          }
          string str2 = dataTable4.Rows[index2]["COG"].ToString().Trim() + " " + dataTable4.Rows[index2]["NOM"].ToString().Trim();
          string str3 = !(dataTable4.Rows[index2]["DENTES_TIPMOV"].ToString().Trim() == "AR") ? dataTable4.Rows[index2]["MESDEN"].ToString().Trim() + "/" + dataTable4.Rows[index2]["ANNDEN"].ToString().Trim() : dataTable4.Rows[index2]["ANNCOM"].ToString().Trim();
          Decimal num3 = Convert.ToDecimal(dataTable4.Rows[index2]["IMPRET"]);
          string str4 = num3.ToString("#,##0.#0");
          num3 = Convert.ToDecimal(dataTable4.Rows[index2]["IMPRETPRE"]);
          string str5 = num3.ToString("#,##0.#0");
          num3 = Convert.ToDecimal(dataTable4.Rows[index2]["IMPRETDEL"]);
          string str6 = num3.ToString("#,##0.#0");
          num3 = Convert.ToDecimal(dataTable4.Rows[index2]["IMPCONDEL"]);
          string str7 = num3.ToString("#,##0.#0");
          string str8 = dataTable4.Rows[index2]["DAL"].ToString().Trim();
          string str9 = str8.Substring(0, 2) + "/" + str8.Substring(3, 2);
          string str10 = dataTable4.Rows[index2]["AL"].ToString().Trim();
          string str11 = str10.Substring(0, 2) + "/" + str10.Substring(3, 2);
          string str12;
          if (dataTable4.Rows[index2]["NUMSAN"].ToString() == "")
          {
            str12 = "0,00";
          }
          else
          {
            num3 = Convert.ToDecimal(dataTable4.Rows[index2]["IMPSANDET"]);
            str12 = num3.ToString("#,##0.#0");
          }
          string str13 = dataTable4.Rows[index2]["ALIQUOTA"].ToString().Trim() + " %";
          string str14 = !(dataTable4.Rows[index2]["TASSAN"].ToString().Trim() == "") ? dataTable4.Rows[index2]["TASSAN"].ToString().Trim() + " %" : "0,00 %";
          Cell = new Cell((IElement) new Phrase(dataTable4.Rows[index2]["MAT"].ToString().Trim(), FontDettaglio));
          Cell.HorizontalAlignment = 1;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 0);
          Cell = new Cell((IElement) new Phrase(str2.Trim().Substring(0, 28), FontDettaglio));
          Cell.HorizontalAlignment = 0;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 1);
          Cell = new Cell((IElement) new Phrase(str3, FontDettaglio));
          Cell.HorizontalAlignment = 1;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 2);
          Cell = new Cell((IElement) new Phrase("€", FontDettaglio));
          Cell.HorizontalAlignment = 2;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 3);
          Cell = new Cell((IElement) new Phrase(str4, FontDettaglio));
          Cell.HorizontalAlignment = 2;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 4);
          Cell = new Cell((IElement) new Phrase("€", FontDettaglio));
          Cell.HorizontalAlignment = 2;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 5);
          Cell = new Cell((IElement) new Phrase(str5, FontDettaglio));
          Cell.HorizontalAlignment = 2;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 6);
          Cell = new Cell((IElement) new Phrase("€", FontDettaglio));
          Cell.HorizontalAlignment = 2;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 7);
          Cell = new Cell((IElement) new Phrase(str6, FontDettaglio));
          Cell.HorizontalAlignment = 2;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 8);
          Cell = new Cell((IElement) new Phrase(str13, FontDettaglio));
          Cell.HorizontalAlignment = 1;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 9);
          Cell = new Cell((IElement) new Phrase("€", FontDettaglio));
          Cell.HorizontalAlignment = 2;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 10);
          Cell = new Cell((IElement) new Phrase(str7, FontDettaglio));
          Cell.HorizontalAlignment = 2;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 11);
          Cell = !(FLGSAN == "S") ? new Cell((IElement) new Phrase("", FontDettaglio)) : new Cell((IElement) new Phrase(str14, FontDettaglio));
          Cell.HorizontalAlignment = 1;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 12);
          Cell = !(FLGSAN == "S") ? new Cell((IElement) new Phrase("", FontDettaglio)) : new Cell((IElement) new Phrase("€", FontDettaglio));
          Cell.HorizontalAlignment = 2;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 13);
          Cell = !(FLGSAN == "S") ? new Cell((IElement) new Phrase("", FontDettaglio)) : new Cell((IElement) new Phrase(str12, FontDettaglio));
          Cell.HorizontalAlignment = 2;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index2 - num2, 14);
          ++num1;
        }
        tabTestata.Offset = 112f;
        document.Add((IElement) tabTestata);
        if (num1 > 15)
        {
          document.NewPage();
          this.ScriviIntestazioneRettifica(ref directContent, ref document, Convert.ToInt32(dataTable3.Rows[0]["CODPOS"]), dataTable3.Rows[0]["DATCONMOV"]?.ToString() ?? "");
          this.ScriviPieDiPaginaRettifica(ref directContent, dataTable3.Rows[0]["CODCAUSAN"].ToString());
          this.scriviNumPagRettifica(ref directContent, cntNumPag + 1, TotPag);
          this.ScriviTotaliRettifica(ref TabTotali, ref CellTotali, ref FontDettaglio, ref FontNascondi, ref FontTestata, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["IMPADDDEL"].ToString(), dataTable3.Rows[0]["IMPCONTOT"].ToString(), IMPSANTOT, ref directContent, FLGSAN, TIPIMP);
          TabTotali.Offset = 108f;
          document.Add((IElement) TabTotali);
        }
        else
        {
          this.ScriviTotaliRettifica(ref TabTotali, ref CellTotali, ref FontDettaglio, ref FontNascondi, ref FontTestata, dataTable3.Rows[0]["CODCAUSAN"].ToString(), dataTable3.Rows[0]["IMPADDDEL"].ToString(), dataTable3.Rows[0]["IMPCONTOT"].ToString(), IMPSANTOT, ref directContent, FLGSAN, TIPIMP);
          TabTotali.Offset = 25f;
          document.Add((IElement) TabTotali);
        }
        Convert.ToDecimal(0.0);
      }
      document.Close();
      instance.Close();
      Process.Start(strPath);
    }

    private void ScriviIntestazioneRettifica(
      ref PdfContentByte cb,
      ref Document document,
      int CODPOS,
      string DATCONMOV)
    {
      DataTable dataTable1 = new DataTable();
      BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
      BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      DataLayer dataLayer = new DataLayer();
      Image instance = Image.GetInstance(this.EstraiFilePGM_Byte("CNT_LOGO"));
      instance.ScaleAbsolute(540f, 110f);
      instance.SetAbsolutePosition(15f, 505f);
      document.Add((IElement) instance);
      string strSQL1 = " SELECT  AZI.RAGSOC, AZI.CODPOS, DUG.DENDUG, INDSED.IND, INDSED.NUMCIV, " + " INDSED.CAP, INDSED.DENLOC, INDSED.SIGPRO, INDSED.DENSTAEST, " + " INDSED.CODCOM, '' AS DENCOM " + " FROM  AZI " + " INNER JOIN INDSED ON " + " INDSED.CODPOS = AZI.CODPOS " + " LEFT JOIN DUG ON INDSED.CODDUG = DUG.CODDUG " + " WHERE INDSED.TIPIND=1 " + " AND INDSED.DATINI <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATCONMOV)) + " AND AZI.CODPOS= " + CODPOS.ToString() + " AND INDSED.DATCOM = (SELECT MAX(DATCOM) FROM INDSED WHERE " + " INDSED.CODPOS = " + CODPOS.ToString() + " AND TIPIND = 1 AND INDSED.DATINI <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATCONMOV)) + ") " + " ORDER BY AZI.CODPOS, INDSED.DATCOM DESC FETCH FIRST 1 ROWS ONLY";
      DataTable dataTable2 = dataLayer.GetDataTable(strSQL1);
      if (dataTable2.Rows.Count <= 0)
        return;
      if (dataTable2.Rows[0]["CODCOM"].ToString().Trim() != "")
      {
        string strSQL2 = "SELECT VALUE(DENCOM,'') AS DENCOM FROM CODCOM WHERE CODCOM = " + this.objNet.DoublePeakForSql(dataTable2.Rows[0]["CODCOM"].ToString().Trim());
        string str = dataLayer.GetDataTable(strSQL2).Rows[0]["DENCOM"].ToString();
        dataTable2.Rows[0]["DENCOM"] = (object) str;
      }
      else
        dataTable2.Rows[0]["DENCOM"] = (object) "";
      string text1 = dataTable2.Rows[0]["RAGSOC"].ToString().Trim();
      string text2;
      if (DBNull.Value.Equals((object) dataTable2.Rows[0]["NUMCIV"].ToString().Trim()) | dataTable2.Rows[0]["NUMCIV"].ToString().Trim() == "")
        text2 = dataTable2.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable2.Rows[0]["IND"].ToString().Trim();
      else
        text2 = dataTable2.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable2.Rows[0]["IND"].ToString().Trim() + ", " + dataTable2.Rows[0]["NUMCIV"].ToString().Trim();
      string text3;
      if (!DBNull.Value.Equals(dataTable2.Rows[0]["DENSTAEST"]))
        text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENSTAEST"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
      else if (dataTable2.Rows[0]["DENLOC"].ToString().Trim() != dataTable2.Rows[0]["DENCOM"].ToString().Trim())
      {
        if (dataTable2.Rows[0]["DENLOC"].ToString().Trim() == "")
          text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENCOM"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
        else
          text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENLOC"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
      }
      else
        text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENCOM"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
      BaseFont font = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.BeginText();
      cb.SetFontAndSize(font, 8.25f);
      cb.ShowTextAligned(0, text1, 565f, 552f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font, 8.25f);
      cb.ShowTextAligned(0, text2, 565f, 542f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font, 8.25f);
      cb.ShowTextAligned(0, text3, 565f, 532f, 0.0f);
      cb.EndText();
    }

    private void ScriviPieDiPaginaRettifica(ref PdfContentByte cb, string CODCAUSAN)
    {
      DataTable dataTable1 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      cb.BeginText();
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.SetFontAndSize(font1, 5.25f);
      cb.ShowTextAligned(0, "Legenda causali:", 10f, 71f, 0.0f);
      cb.EndText();
      int y = 64;
      string str1 = "SELECT DISTINCT CODCAU, VALUE(DESCAUREP, ' ') AS DESC FROM TIPMOVCAU ";
      string strSQL = !(CODCAUSAN == "") ? str1 + " WHERE TIPMOV IN ('RT_POS', 'RT_NEG', 'SAN_RT_MD', 'SAN_RT_RD') ORDER BY CODCAU ASC" : str1 + " WHERE TIPMOV IN ('RT_POS', 'RT_NEG') ORDER BY CODCAU ASC";
      DataTable dataTable2 = dataLayer.GetDataTable(strSQL);
      if (dataTable2.Rows.Count > 0)
      {
        for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
        {
          if (dataTable2.Rows[index]["DESC"].ToString() != "")
          {
            string str2 = dataTable2.Rows[index]["CODCAU"]?.ToString() + " " + dataTable2.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[index]["DESC"].ToString().Length - 1);
            cb.BeginText();
            BaseFont font2 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
            cb.SetFontAndSize(font2, 5.25f);
            cb.ShowTextAligned(0, str2.Trim(), 10f, (float) y, 0.0f);
            cb.EndText();
          }
          else
          {
            string text = "";
            cb.BeginText();
            BaseFont font3 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
            cb.SetFontAndSize(font3, 5.25f);
            cb.ShowTextAligned(0, text, 10f, (float) y, 0.0f);
            cb.EndText();
          }
          y -= 7;
        }
      }
      else
      {
        for (int index = 0; index <= 5; ++index)
        {
          cb.BeginText();
          BaseFont font4 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
          cb.SetFontAndSize(font4, 5.25f);
          cb.ShowTextAligned(0, "", 10f, (float) y, 0.0f);
          cb.EndText();
          y -= 7;
        }
      }
      cb.BeginText();
      BaseFont font5 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.SetFontAndSize(font5, 8.25f);
      cb.ShowTextAligned(1, "Per informazioni telefonare al Call Center 800.010270 - 800.313231", 420f, 10f, 0.0f);
      cb.EndText();
    }

    private void ScriviTestataRettifica(
      ref iTextSharp.text.Table tabTestata,
      ref Cell Cell,
      ref iTextSharp.text.Font FontTestata,
      string FLGSAN)
    {
      Cell = new Cell((IElement) new Phrase("Matricola", FontTestata));
      Cell.HorizontalAlignment = 1;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 0);
      Cell = new Cell((IElement) new Phrase("Cognome e Nome", FontTestata));
      Cell.HorizontalAlignment = 0;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 1);
      Cell = new Cell((IElement) new Phrase("Periodo", FontTestata));
      Cell.HorizontalAlignment = 1;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 2);
      Cell = new Cell((IElement) new Phrase("Retribuzione attuale", FontTestata));
      Cell.HorizontalAlignment = 2;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 3);
      Cell = new Cell((IElement) new Phrase("Retribuzione precedente", FontTestata));
      Cell.HorizontalAlignment = 2;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 5);
      Cell = new Cell((IElement) new Phrase("Differenza", FontTestata));
      Cell.HorizontalAlignment = 2;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 7);
      Cell = new Cell((IElement) new Phrase("Aliquota", FontTestata));
      Cell.HorizontalAlignment = 1;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 9);
      Cell = new Cell((IElement) new Phrase("Contributi", FontTestata));
      Cell.HorizontalAlignment = 2;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.Colspan = 2;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 10);
      Cell = !(FLGSAN == "S") ? new Cell((IElement) new Phrase("", FontTestata)) : new Cell((IElement) new Phrase("Tasso Sanzione", FontTestata));
      Cell.HorizontalAlignment = 1;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 12);
      Cell = !(FLGSAN == "S") ? new Cell((IElement) new Phrase("", FontTestata)) : new Cell((IElement) new Phrase("Sanzione", FontTestata));
      Cell.HorizontalAlignment = 2;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.Colspan = 2;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 13);
    }

    private void scriviNumPagRettifica(ref PdfContentByte cb, int cntNumPag, int TotPag)
    {
      string text = "Pag. " + cntNumPag.ToString() + " di " + TotPag.ToString();
      cb.BeginText();
      BaseFont font = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.SetFontAndSize(font, 7.25f);
      cb.ShowTextAligned(1, text, 800f, 10f, 0.0f);
      cb.EndText();
    }

    private void ScriviSanzioneRettifica(
      ref PdfContentByte cb,
      string CODCAUSAN,
      string DATCONMOV,
      string NUMMOV,
      string DATSAN,
      string NUMSAN,
      string MESDEN,
      string ANNDEN,
      int CODPOS,
      string DAL,
      string AL)
    {
      string text1 = "";
      DateTimeFormatInfo dateTimeFormat = new CultureInfo("it-IT", false).DateTimeFormat;
      BaseFont font = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      string str1 = !(DATCONMOV != "") ? "" : Convert.ToDateTime(DATCONMOV).ToString();
      string str2 = !(DATSAN != "") ? "" : Convert.ToDateTime(DATSAN).ToString();
      string str3 = dateTimeFormat.MonthNames[Convert.ToInt32(DAL.Substring(3, 2)) - 1].ToString().ToUpper() + " " + DAL.Substring(6, 4);
      string str4 = dateTimeFormat.MonthNames[Convert.ToInt32(AL.Substring(3, 2)) - 1].ToString().ToUpper() + " " + AL.Substring(6, 4);
      string text2;
      if (NUMSAN == "")
        text2 = "Nota di rettifica n. " + NUMMOV.Trim() + " emessa il " + str1 + " relativa al periodo " + str3 + IIf(str3 == str4, (object) "", (object) (" / " + str4))?.ToString();
      else
        text2 = "Nota di rettifica n. " + NUMMOV.Trim() + " e Nota sanzione n. " + NUMSAN.Trim() + " emesse il " + str2 + " relative al periodo " + str3 + IIf(str3 == str4, (object) "", (object) (" / " + str4))?.ToString();
      string text3 = "Posizione assicurativa " + CODPOS.ToString();
      cb.BeginText();
      cb.SetFontAndSize(font, 8.25f);
      cb.ShowTextAligned(0, text2, 10f, 500f, 0.0f);
      cb.EndText();
      if (text1 != "")
      {
        cb.BeginText();
        cb.SetFontAndSize(font, 8.25f);
        cb.ShowTextAligned(0, text1, 10f, 490f, 0.0f);
        cb.EndText();
        cb.BeginText();
        cb.SetFontAndSize(font, 8.25f);
        cb.ShowTextAligned(0, text3, 10f, 480f, 0.0f);
        cb.EndText();
      }
      else
      {
        cb.BeginText();
        cb.SetFontAndSize(font, 8.25f);
        cb.ShowTextAligned(0, text3, 10f, 490f, 0.0f);
        cb.EndText();
      }

      static object IIf(bool expression, object truePart, object falsePart) => !expression ? falsePart : truePart;
    }

    private void ScriviTotaliRettifica(
      ref iTextSharp.text.Table TabTotali,
      ref Cell CellTotali,
      ref iTextSharp.text.Font FontDettaglio,
      ref iTextSharp.text.Font FontNascondi,
      ref iTextSharp.text.Font FontTestata,
      string CODCAU,
      string IMPADDDEL,
      string IMPCONTOT,
      string IMPSANTOT,
      ref PdfContentByte cb,
      string FLGSAN,
      string TIPIMP)
    {
      string str1 = "";
      DataTable dataTable1 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 3, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 4, 0);
      CellTotali = new Cell((IElement) new Phrase("Importo contributo", FontDettaglio));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 1);
      string str2 = "Importo addizionale";
      CellTotali = new Cell((IElement) new Phrase(str2, FontDettaglio));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 1);
      if (CODCAU == "")
      {
        str1 = "Sanzioni";
      }
      else
      {
        string strSQL = "SELECT VALUE(TASSO, 0.00) AS TASSO, TIPMOV, DESCAUREP AS DESC FROM TIPMOVCAU " + " WHERE CODCAU ='" + CODCAU + "' AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
        dataTable1.Clear();
        DataTable dataTable2 = dataLayer.GetDataTable(strSQL);
        if (dataTable2.Rows.Count > 0)
        {
          for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
          {
            string str3 = dataTable2.Rows[index]["TIPMOV"].ToString().Trim();
            if (!(str3 == "SAN_RT_MD"))
            {
              if (str3 == "SAN_RT_RD")
                str1 = !(dataTable2.Rows[index]["DESC"].ToString() != "") ? "" : dataTable2.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[index]["DESC"].ToString().Length - 1);
            }
            else
              str1 = !(dataTable2.Rows[index]["DESC"].ToString() != "") ? "" : dataTable2.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[index]["DESC"].ToString().Length - 1);
          }
        }
        else
          str1 = "";
      }
      CellTotali = !(FLGSAN == "S") ? new Cell((IElement) new Phrase("", FontDettaglio)) : new Cell((IElement) new Phrase(str1, FontDettaglio));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 1);
      CellTotali = new Cell((IElement) new Phrase(".....", FontNascondi));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 6;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 3, 1);
      CellTotali = !(TIPIMP == "+") ? new Cell((IElement) new Phrase("Importo a credito", FontTestata)) : new Cell((IElement) new Phrase("Importo a debito", FontTestata));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 4, 1);
      CellTotali = new Cell((IElement) new Phrase("€", FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 2);
      CellTotali = new Cell((IElement) new Phrase("€", FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 2);
      CellTotali = !(FLGSAN == "S") ? new Cell((IElement) new Phrase("", FontDettaglio)) : new Cell((IElement) new Phrase("€", FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 2);
      CellTotali = new Cell();
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 3, 2);
      CellTotali = new Cell((IElement) new Phrase("€", FontTestata));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 4, 2);
      string str4 = (Convert.ToDecimal(IMPCONTOT) + Convert.ToDecimal(IMPADDDEL)).ToString();
      if (FLGSAN == "S")
        str4 = (Convert.ToDecimal(str4) + Convert.ToDecimal(IMPSANTOT)).ToString();
      Decimal num = Convert.ToDecimal(IMPCONTOT);
      IMPCONTOT = num.ToString("#,##0.#0");
      num = Convert.ToDecimal(IMPADDDEL);
      IMPADDDEL = num.ToString("#,##0.#0");
      CellTotali = new Cell((IElement) new Phrase(IMPCONTOT, FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 3);
      CellTotali = new Cell((IElement) new Phrase(IMPADDDEL, FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 3);
      if (FLGSAN == "S")
      {
        ref Cell local = ref CellTotali;
        num = Convert.ToDecimal(IMPSANTOT);
        Cell cell = new Cell((IElement) new Phrase(num.ToString("#,##0.#0"), FontDettaglio));
        local = cell;
      }
      else
        CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 3);
      CellTotali = new Cell((IElement) new Phrase("....", FontNascondi));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 6;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 3, 3);
      ref Cell local1 = ref CellTotali;
      num = Convert.ToDecimal(str4);
      Cell cell1 = new Cell((IElement) new Phrase(num.ToString("#,##0.#0"), FontTestata));
      local1 = cell1;
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 4, 3);
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAI"), "Cp1252", false);
      BaseFont font2 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
      iTextSharp.text.Font font3 = new iTextSharp.text.Font(font1, 8.25f, 0);
      iTextSharp.text.Font font4 = new iTextSharp.text.Font(font2, 8.25f, 0);
      BaseFont font5 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
      iTextSharp.text.Font font6 = new iTextSharp.text.Font(font5, 7.75f, 1);
      iTextSharp.text.Font font7 = new iTextSharp.text.Font(font5, 7.75f, 0, Color.WHITE);
      if (TIPIMP == "+")
      {
        CellTotali = new Cell((IElement) new Phrase("0", font7));
        CellTotali.BorderWidth = 0.0f;
        CellTotali.Colspan = 4;
        TabTotali.AddCell(CellTotali, 5, 0);
        CellTotali = new Cell((IElement) new Phrase("Per il pagamento:", font3));
        CellTotali.HorizontalAlignment = 0;
        CellTotali.BorderWidth = 0.0f;
        CellTotali.Colspan = 4;
        TabTotali.AddCell(CellTotali, 6, 0);
        CellTotali = new Cell((IElement) new Phrase("IBAN: IT71Y0569603211000036000X17", font4));
        CellTotali.HorizontalAlignment = 0;
        CellTotali.BorderWidth = 0.0f;
        CellTotali.Colspan = 4;
        TabTotali.AddCell(CellTotali, 7, 0);
        CellTotali = new Cell((IElement) new Phrase("(citare n° posizione assicurativa nella causale)", font3));
        CellTotali.HorizontalAlignment = 0;
        CellTotali.BorderWidth = 0.0f;
        CellTotali.Colspan = 4;
        TabTotali.AddCell(CellTotali, 8, 0);
      }
      else
      {
        CellTotali = new Cell((IElement) new Phrase("0", font7));
        CellTotali.BorderWidth = 0.0f;
        CellTotali.Colspan = 4;
        TabTotali.AddCell(CellTotali, 5, 0);
        CellTotali = new Cell((IElement) new Phrase("", font3));
        CellTotali.HorizontalAlignment = 0;
        CellTotali.BorderWidth = 0.0f;
        CellTotali.Colspan = 4;
        TabTotali.AddCell(CellTotali, 6, 0);
        CellTotali = new Cell((IElement) new Phrase("", font3));
        CellTotali.HorizontalAlignment = 0;
        CellTotali.BorderWidth = 0.0f;
        CellTotali.Colspan = 4;
        TabTotali.AddCell(CellTotali, 7, 0);
        CellTotali = new Cell((IElement) new Phrase("", font3));
        CellTotali.HorizontalAlignment = 0;
        CellTotali.BorderWidth = 0.0f;
        CellTotali.Colspan = 4;
        TabTotali.AddCell(CellTotali, 8, 0);
      }
      CellTotali = new Cell((IElement) new Phrase("0", font7));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 9, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font7));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 10, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font7));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 11, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font7));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 12, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font7));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 13, 0);
      CellTotali = !(FLGSAN == "S") ? new Cell((IElement) new Phrase("", font7)) : new Cell((IElement) new Phrase("Da versare entro 30 giorni dalla data di emissione tramite gli allegati M.Av.", font6));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 6;
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 14, 0);
    }

    public void CreaStampaRettificaAnnullata(ref DataTable dtRettifica, string strPath)
    {
      Document document = new Document(PageSize.A4.Rotate(), 10f, 20f, 5f, 5f);
      PdfWriter instance = PdfWriter.GetInstance(document, (Stream) new FileStream(strPath, FileMode.Create));
      int num1 = 1;
      int num2 = 0;
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataTable dataTable3 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      Decimal IMPADDDEL = 0M;
      Decimal num3 = 0M;
      Decimal num4 = 0M;
      int cntNumPag = 1;
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "CODPOS"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "ANNDEN"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "MESDEN"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "PRODEN"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "MAT"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "COG"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "NOM"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "DAL"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "AL"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "IMPRET"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "IMPRETPRE"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "IMPRETDEL"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "IMPCONDEL"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "IMPSANDET"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "NUMMOVANN"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "DATMOVANN"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "NUMSANANN"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "DATSANANN"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "CODCAUSAN"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "ALIQUOTA"
      });
      dataTable1.Columns.Add(new DataColumn()
      {
        ColumnName = "TASSAN"
      });
      document.Open();
      for (int index1 = 0; index1 <= dtRettifica.Rows.Count - 1; ++index1)
      {
        PdfContentByte directContent = instance.DirectContent;
        BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
        if (index1 > 0)
        {
          cntNumPag = 1;
          num2 = 0;
          num1 = 1;
          document.NewPage();
        }
        string strSQL1 = " SELECT DENDET.MAT, SUM(DENDET.IMPRET) AS IMPRET, SUM(DENDET.IMPRETPRE) AS IMPRETPRE," + " SUM(DENDET.IMPRETDEL) AS IMPRETDEL, SUM(DENDET.IMPCONDEL) AS IMPCONDEL," + " SUM(DENDET.IMPSANDET) AS IMPSANDET" + " FROM DENDET WHERE DENDET.CODPOS = " + dtRettifica.Rows[index1]["CODPOS"]?.ToString() + " AND DENDET.ANNDEN = " + dtRettifica.Rows[index1]["ANNDEN"]?.ToString() + " AND DENDET.MESDEN = " + dtRettifica.Rows[index1]["MESDEN"]?.ToString() + " AND DENDET.PRODEN = " + dtRettifica.Rows[index1]["PRODEN"]?.ToString() + " AND DENDET.TIPMOV = 'RT'" + " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NOT NULL" + " AND IMPCONDEL <> 0" + " GROUP BY DENDET.MAT";
        dataTable2.Clear();
        dataTable2 = dataLayer.GetDataTable(strSQL1);
        string strSQL2 = " SELECT DENDET.MAT, DENDET.PRODEN, DENDET.ALIQUOTA, DENDET.ANNDEN," + " DENDET.MESDEN, RETTES.NUMSAN, DENDET.CODPOS, ISCT.COG, ISCT.NOM," + " DENDET.DAL, DENDET.AL, DENDET.TASSAN," + " DENDET.DATMOVANN, DENDET.CODCAUSAN, RETTES.NUMMOV, DENDET.NUMMOVANN, " + " DENDET.DATSANANN, DENDET.NUMSANANN " + " FROM DENDET INNER JOIN ISCT ON ISCT.MAT = DENDET.MAT" + " INNER JOIN RETTES ON RETTES.PRORET = DENDET.PRORET" + " AND RETTES.PRORETTES = DENDET.PRORETTES" + " AND RETTES.ANNRET = DENDET.ANNRET" + " WHERE DENDET.CODPOS = " + dtRettifica.Rows[index1]["CODPOS"]?.ToString() + " AND DENDET.ANNDEN = " + dtRettifica.Rows[index1]["ANNDEN"]?.ToString() + " AND DENDET.MESDEN = " + dtRettifica.Rows[index1]["MESDEN"]?.ToString() + " AND DENDET.PRODEN = " + dtRettifica.Rows[index1]["PRODEN"]?.ToString() + " AND DENDET.TIPMOV = 'RT'" + " AND RETTES.NUMMOV IS NOT NULL AND DENDET.NUMMOVANN IS NOT NULL" + " AND DENDET.IMPCONDEL <> 0" + " ORDER BY ISCT.COG, ISCT.NOM, DENDET.DAL";
        dataTable3.Clear();
        dataTable3 = dataLayer.GetDataTable(strSQL2);
        for (int index2 = 0; index2 <= dataTable2.Rows.Count - 1; ++index2)
        {
          for (int index3 = 0; index3 <= dataTable3.Rows.Count - 1; ++index3)
          {
            if (Convert.ToInt32(dataTable3.Rows[index3]["MAT"]) == Convert.ToInt32(dataTable2.Rows[index2]["MAT"]))
            {
              DataRow row = dataTable1.NewRow();
              dataTable1.Rows.Add(row);
              dataTable1.Rows[dataTable1.Rows.Count - 1]["CODPOS"] = dataTable3.Rows[index3]["CODPOS"];
              dataTable1.Rows[dataTable1.Rows.Count - 1]["ANNDEN"] = dataTable3.Rows[index3]["ANNDEN"];
              dataTable1.Rows[dataTable1.Rows.Count - 1]["MESDEN"] = dataTable3.Rows[index3]["MESDEN"];
              dataTable1.Rows[dataTable1.Rows.Count - 1]["PRODEN"] = dataTable3.Rows[index3]["PRODEN"];
              dataTable1.Rows[dataTable1.Rows.Count - 1]["MAT"] = dataTable3.Rows[index3]["MAT"];
              dataTable1.Rows[dataTable1.Rows.Count - 1]["COG"] = dataTable3.Rows[index3]["COG"];
              dataTable1.Rows[dataTable1.Rows.Count - 1]["NOM"] = dataTable3.Rows[index3]["NOM"];
              dataTable1.Rows[dataTable1.Rows.Count - 1]["DAL"] = dataTable3.Rows[index3]["DAL"];
              dataTable1.Rows[dataTable1.Rows.Count - 1]["AL"] = dataTable3.Rows[index3]["AL"];
              dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPRET"] = dataTable2.Rows[index2]["IMPRET"];
              dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPRETPRE"] = dataTable2.Rows[index2]["IMPRETPRE"];
              dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPRETDEL"] = dataTable2.Rows[index2]["IMPRETDEL"];
              dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPCONDEL"] = dataTable2.Rows[index2]["IMPCONDEL"];
              dataTable1.Rows[dataTable1.Rows.Count - 1]["IMPSANDET"] = dataTable2.Rows[index2]["IMPSANDET"];
              dataTable1.Rows[dataTable1.Rows.Count - 1]["NUMMOVANN"] = dataTable3.Rows[index3]["NUMMOVANN"];
              dataTable1.Rows[dataTable1.Rows.Count - 1]["DATMOVANN"] = dataTable3.Rows[index3]["DATMOVANN"];
              dataTable1.Rows[dataTable1.Rows.Count - 1]["NUMSANANN"] = dataTable3.Rows[index3]["NUMSANANN"];
              dataTable1.Rows[dataTable1.Rows.Count - 1]["DATSANANN"] = dataTable3.Rows[index3]["DATSANANN"];
              dataTable1.Rows[dataTable1.Rows.Count - 1]["CODCAUSAN"] = dataTable3.Rows[index3]["CODCAUSAN"];
              dataTable1.Rows[dataTable1.Rows.Count - 1]["ALIQUOTA"] = (object) "";
              dataTable1.Rows[dataTable1.Rows.Count - 1]["TASSAN"] = (object) "";
              break;
            }
          }
        }
        this.ScriviIntestazioneRettificaAnnullata(ref directContent, ref document, Convert.ToInt32(dataTable1.Rows[0]["CODPOS"]), dataTable1.Rows[0]["DATMOVANN"]?.ToString() ?? "");
        this.ScriviPieDiPaginaRettificaAnnullata(ref directContent, dataTable1.Rows[0]["CODCAUSAN"].ToString());
        Cell Cell = new Cell();
        iTextSharp.text.Table tabTestata = new iTextSharp.text.Table(15, 1);
        tabTestata.WidthPercentage = 100f;
        tabTestata.AutoFillEmptyCells = true;
        tabTestata.BorderWidth = 0.0f;
        tabTestata.BorderColor = Color.WHITE;
        tabTestata.Cellpadding = 2f;
        int[] widths1 = new int[15]
        {
          8,
          19,
          5,
          4,
          7,
          5,
          6,
          4,
          7,
          10,
          2,
          6,
          9,
          2,
          6
        };
        tabTestata.SetWidths(widths1);
        Cell CellTotali = new Cell();
        iTextSharp.text.Table TabTotali = new iTextSharp.text.Table(4, 5);
        TabTotali.WidthPercentage = 45f;
        TabTotali.AutoFillEmptyCells = true;
        TabTotali.Alignment = 2;
        TabTotali.BorderWidth = 0.0f;
        TabTotali.BorderColor = Color.WHITE;
        int[] widths2 = new int[4]{ 30, 45, 3, 22 };
        TabTotali.SetWidths(widths2);
        iTextSharp.text.Font FontTestata = new iTextSharp.text.Font(BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false), 8.25f, 0);
        this.ScriviTestataRettificaAnnullata(ref tabTestata, ref Cell, ref FontTestata);
        BaseFont font = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
        iTextSharp.text.Font FontDettaglio = new iTextSharp.text.Font(font, 8.25f, 0);
        iTextSharp.text.Font FontNascondi = new iTextSharp.text.Font(font, 8.25f, 0, Color.WHITE);
        int TotPag = !((Decimal) (dataTable1.Rows.Count - dataTable1.Rows.Count / 18 * 18) <= 9M) ? dataTable1.Rows.Count / 18 + 2 : dataTable1.Rows.Count / 18 + 1;
        this.scriviNumPagRettificaAnnullata(ref directContent, cntNumPag, TotPag);
        for (int index4 = 0; index4 <= dataTable1.Rows.Count - 1; ++index4)
        {
          this.ScriviSanzioneRettificaAnnullata(ref directContent, dataTable1.Rows[0]["CODCAUSAN"].ToString(), dataTable1.Rows[0]["DATMOVANN"].ToString(), dataTable1.Rows[0]["NUMMOVANN"].ToString(), dataTable1.Rows[0]["DATSANANN"].ToString(), dataTable1.Rows[0]["NUMSANANN"].ToString(), dataTable1.Rows[index4]["MESDEN"].ToString(), dataTable1.Rows[index4]["ANNDEN"].ToString(), Convert.ToInt32(dataTable1.Rows[index4]["CODPOS"]), dataTable1.Rows[0]["DAL"].ToString(), dataTable1.Rows[0]["AL"].ToString());
          if (num1 == 19)
          {
            tabTestata.Offset = 108f;
            document.Add((IElement) tabTestata);
            tabTestata.DeleteAllRows();
            document.NewPage();
            this.ScriviIntestazioneRettificaAnnullata(ref directContent, ref document, Convert.ToInt32(dataTable1.Rows[0]["CODPOS"]), dataTable1.Rows[0]["DATCONMOV"]?.ToString() ?? "");
            this.ScriviPieDiPaginaRettificaAnnullata(ref directContent, dataTable1.Rows[0]["CODCAUSAN"].ToString());
            this.ScriviTestataRettificaAnnullata(ref tabTestata, ref Cell, ref FontTestata);
            ++cntNumPag;
            num2 = index4;
            num1 = 1;
            this.scriviNumPagRettificaAnnullata(ref directContent, cntNumPag, TotPag);
          }
          string str1 = dataTable1.Rows[index4]["COG"].ToString().Trim() + " " + dataTable1.Rows[index4]["NOM"].ToString().Trim();
          string str2 = dataTable1.Rows[index4]["MESDEN"].ToString().Trim() + "/" + dataTable1.Rows[index4]["ANNDEN"].ToString().Trim();
          Decimal num5 = Convert.ToDecimal(dataTable1.Rows[index4]["IMPRET"]);
          string str3 = num5.ToString("#,##0.#0");
          num5 = Convert.ToDecimal(dataTable1.Rows[index4]["IMPRETPRE"]);
          string str4 = num5.ToString("#,##0.#0");
          num5 = Convert.ToDecimal(dataTable1.Rows[index4]["IMPRETDEL"]);
          string str5 = num5.ToString("#,##0.#0");
          num5 = Convert.ToDecimal(dataTable1.Rows[index4]["IMPCONDEL"]);
          string str6 = num5.ToString("#,##0.#0");
          string str7 = dataTable1.Rows[index4]["DAL"].ToString().Trim();
          string str8 = str7.Substring(0, 2) + "/" + str7.Substring(3, 2);
          string str9 = dataTable1.Rows[index4]["AL"].ToString().Trim();
          string str10 = str9.Substring(0, 2) + "/" + str9.Substring(3, 2);
          string str11;
          if (dataTable1.Rows[index4]["NUMSANANN"].ToString() == "")
          {
            str11 = "0,00";
          }
          else
          {
            num5 = Convert.ToDecimal(dataTable1.Rows[index4]["IMPSANDET"]);
            str11 = num5.ToString("#,##0.#0");
          }
          string str12 = "0,00 %";
          string str13 = !(dataTable1.Rows[index4]["TASSAN"].ToString().Trim() == "") ? dataTable1.Rows[index4]["TASSAN"].ToString().Trim() + " %" : "0,00 %";
          Cell = new Cell((IElement) new Phrase(dataTable1.Rows[index4]["MAT"].ToString().Trim(), FontDettaglio));
          Cell.HorizontalAlignment = 1;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index4 - num2, 0);
          Cell = new Cell((IElement) new Phrase(str1.Trim(), FontDettaglio));
          Cell.HorizontalAlignment = 0;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index4 - num2, 1);
          Cell = new Cell((IElement) new Phrase(str2, FontDettaglio));
          Cell.HorizontalAlignment = 1;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index4 - num2, 2);
          Cell = new Cell((IElement) new Phrase("€", FontDettaglio));
          Cell.HorizontalAlignment = 2;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index4 - num2, 3);
          Cell = new Cell((IElement) new Phrase(str3, FontDettaglio));
          Cell.HorizontalAlignment = 2;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index4 - num2, 4);
          Cell = new Cell((IElement) new Phrase("€", FontDettaglio));
          Cell.HorizontalAlignment = 2;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index4 - num2, 5);
          Cell = new Cell((IElement) new Phrase(str4, FontDettaglio));
          Cell.HorizontalAlignment = 2;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index4 - num2, 6);
          Cell = new Cell((IElement) new Phrase("€", FontDettaglio));
          Cell.HorizontalAlignment = 2;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index4 - num2, 7);
          Cell = new Cell((IElement) new Phrase(str5, FontDettaglio));
          Cell.HorizontalAlignment = 2;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index4 - num2, 8);
          Cell = new Cell((IElement) new Phrase(str12, FontDettaglio));
          Cell.HorizontalAlignment = 1;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index4 - num2, 9);
          Cell = new Cell((IElement) new Phrase("€", FontDettaglio));
          Cell.HorizontalAlignment = 2;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index4 - num2, 10);
          Cell = new Cell((IElement) new Phrase(str6, FontDettaglio));
          Cell.HorizontalAlignment = 2;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index4 - num2, 11);
          Cell = new Cell((IElement) new Phrase(str13, FontDettaglio));
          Cell.HorizontalAlignment = 1;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index4 - num2, 12);
          Cell = new Cell((IElement) new Phrase("€", FontDettaglio));
          Cell.HorizontalAlignment = 2;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index4 - num2, 13);
          Cell = new Cell((IElement) new Phrase(str11, FontDettaglio));
          Cell.HorizontalAlignment = 2;
          Cell.VerticalAlignment = 5;
          Cell.BorderWidth = 0.0f;
          Cell.BorderWidthBottom = 0.5f;
          tabTestata.AddCell(Cell, 1 + index4 - num2, 14);
          num3 = Convert.ToDecimal(num3) + Convert.ToDecimal(str11);
          num4 = Convert.ToDecimal(num4) + Convert.ToDecimal(str6);
          string strSQL3 = "SELECT COUNT(*) FROM PARGENPOS WHERE CODPOS = " + dataTable1.Rows[index4]["CODPOS"]?.ToString() + " AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
          Decimal num6;
          if (Convert.ToInt32(dataLayer.GetDataTable(strSQL3).Rows[0][0]) == 0)
          {
            string strSQL4 = "SELECT VALORE FROM PARGENDET WHERE CODPAR = 5" + " AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
            num6 = Convert.ToDecimal(dataLayer.GetDataTable(strSQL4).Rows[0]["VALORE"]);
          }
          else
          {
            string strSQL5 = "SELECT VALORE FROM PARGENPOS WHERE CODPOS = " + dataTable1.Rows[index4]["CODPOS"]?.ToString() + " AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
            num6 = Convert.ToDecimal(dataLayer.GetDataTable(strSQL5).Rows[0]["VALORE"]);
          }
          IMPADDDEL += Convert.ToDecimal(str6) * num6 / 100M;
          Convert.ToDecimal(0.0);
          ++num1;
        }
        tabTestata.Offset = 108f;
        document.Add((IElement) tabTestata);
        if (num1 > 10)
        {
          document.NewPage();
          this.ScriviIntestazioneRettificaAnnullata(ref directContent, ref document, Convert.ToInt32(dataTable1.Rows[0]["CODPOS"]), dataTable1.Rows[0]["DATCONMOV"]?.ToString() ?? "");
          this.ScriviPieDiPaginaRettificaAnnullata(ref directContent, dataTable1.Rows[0]["CODCAUSAN"].ToString());
          this.scriviNumPagRettificaAnnullata(ref directContent, cntNumPag + 1, TotPag);
          this.ScriviTotaliRettificaAnnullata(ref TabTotali, ref CellTotali, ref FontDettaglio, ref FontNascondi, ref FontTestata, dataTable1.Rows[0]["CODCAUSAN"].ToString(), num4.ToString(), num3.ToString(), IMPADDDEL, ref directContent);
          TabTotali.Offset = 108f;
          document.Add((IElement) TabTotali);
        }
        else
        {
          this.ScriviTotaliRettificaAnnullata(ref TabTotali, ref CellTotali, ref FontDettaglio, ref FontNascondi, ref FontTestata, dataTable1.Rows[0]["CODCAUSAN"].ToString(), num4.ToString(), num3.ToString(), IMPADDDEL, ref directContent);
          TabTotali.Offset = 10f;
          document.Add((IElement) TabTotali);
        }
        Convert.ToDecimal(0.0);
      }
      document.Close();
      instance.Close();
      Process.Start(strPath);
    }

    private void ScriviIntestazioneRettificaAnnullata(
      ref PdfContentByte cb,
      ref Document document,
      int CODPOS,
      string DATCONMOV)
    {
      DataTable dataTable1 = new DataTable();
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
      BaseFont font2 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      DataLayer dataLayer = new DataLayer();
      Image instance = Image.GetInstance(this.EstraiFilePGM_Byte("CNT_LOGO"));
      instance.ScaleAbsolute(103f, 51f);
      instance.SetAbsolutePosition(82f, 545f);
      document.Add((IElement) instance);
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(1, "FONDAZIONE E.N.P.A.I.A.", 395f, 580f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(1, "ENTE NAZIONALE DI PREVIDENZA PER GLI ADDETTI E PER GLI", 395f, 570f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font1, 9f);
      cb.ShowTextAligned(1, "IMPIEGATI IN AGRICOLTURA", 395f, 560f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Viale Beethoven, 48 - 00144 ROMA", 395f, 550f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Call Center 800.010270 - Fax 06/5914444 - 06/5458385", 395f, 540f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Internet: www.enpaia.it       Email: info@enpaia.it", 395f, 530f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font2, 8.25f);
      cb.ShowTextAligned(1, "Ufficio contributi e riscossione", 133f, 542f, 0.0f);
      cb.EndText();
      string strSQL1 = " SELECT  AZI.RAGSOC, AZI.CODPOS, DUG.DENDUG, INDSED.IND, INDSED.NUMCIV, " + " INDSED.CAP, INDSED.DENLOC, INDSED.SIGPRO, INDSED.DENSTAEST, " + " INDSED.CODCOM, '' AS DENCOM " + " FROM  AZI " + " INNER JOIN INDSED ON " + " INDSED.CODPOS = AZI.CODPOS " + " LEFT JOIN DUG ON INDSED.CODDUG = DUG.CODDUG " + " WHERE INDSED.TIPIND=1 " + " AND INDSED.DATINI <= '" + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATCONMOV)) + "'" + " AND AZI.CODPOS= " + CODPOS.ToString() + " AND INDSED.DATCOM = (SELECT MAX(DATCOM) FROM INDSED WHERE " + " INDSED.CODPOS = " + CODPOS.ToString() + " AND TIPIND = 1 AND INDSED.DATINI <= '" + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATCONMOV)) + "') " + " ORDER BY AZI.CODPOS, INDSED.DATCOM DESC FETCH FIRST 1 ROWS ONLY";
      DataTable dataTable2 = dataLayer.GetDataTable(strSQL1);
      if (dataTable2.Rows.Count <= 0)
        return;
      if (dataTable2.Rows[0]["CODCOM"].ToString().Trim() != "")
      {
        string strSQL2 = "SELECT VALUE(DENCOM,'') AS DENCOM FROM CODCOM WHERE CODCOM = " + DBMethods.DoublePeakForSql(dataTable2.Rows[0]["CODCOM"].ToString().Trim());
        string str = dataLayer.GetDataTable(strSQL2).Rows[0]["DENCOM"].ToString();
        dataTable2.Rows[0]["DENCOM"] = (object) str;
      }
      else
        dataTable2.Rows[0]["DENCOM"] = (object) "";
      string text1 = dataTable2.Rows[0]["RAGSOC"].ToString().Trim();
      string text2;
      if (DBNull.Value.Equals(dataTable2.Rows[0]["NUMCIV"]) | dataTable2.Rows[0]["NUMCIV"].ToString().Trim() == "")
        text2 = dataTable2.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable2.Rows[0]["IND"].ToString().Trim();
      else
        text2 = dataTable2.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable2.Rows[0]["IND"].ToString().Trim() + ", " + dataTable2.Rows[0]["NUMCIV"].ToString().Trim();
      string text3;
      if (!DBNull.Value.Equals(dataTable2.Rows[0]["DENSTAEST"]))
        text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENSTAEST"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
      else if (dataTable2.Rows[0]["DENLOC"].ToString().Trim() != dataTable2.Rows[0]["DENCOM"].ToString().Trim())
      {
        if (dataTable2.Rows[0]["DENLOC"].ToString().Trim() == "")
          text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENCOM"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
        else
          text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENLOC"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
      }
      else
        text3 = dataTable2.Rows[0]["CAP"].ToString().Trim() + " - " + dataTable2.Rows[0]["DENCOM"].ToString().Trim() + " (" + dataTable2.Rows[0]["SIGPRO"].ToString().Trim() + ")";
      BaseFont font3 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.BeginText();
      cb.SetFontAndSize(font3, 8.25f);
      cb.ShowTextAligned(0, text1, 565f, 552f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font3, 8.25f);
      cb.ShowTextAligned(0, text2, 565f, 542f, 0.0f);
      cb.EndText();
      cb.BeginText();
      cb.SetFontAndSize(font3, 8.25f);
      cb.ShowTextAligned(0, text3, 565f, 532f, 0.0f);
      cb.EndText();
    }

    private void ScriviPieDiPaginaRettificaAnnullata(ref PdfContentByte cb, string CODCAUSAN)
    {
      DataTable dataTable1 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      cb.BeginText();
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.SetFontAndSize(font1, 5.25f);
      cb.ShowTextAligned(0, "Legenda causali:", 10f, 71f, 0.0f);
      cb.EndText();
      int y = 64;
      string str1 = "SELECT DISTINCT CODCAU, VALUE(DESCAUREP, ' ') AS DESC FROM TIPMOVCAU ";
      string strSQL = !(CODCAUSAN == "") ? str1 + " WHERE TIPMOV IN ('ANN_RT_POS', 'ANN_RT_NEG', 'ANN_SAN_MD', 'ANN_SAN_RD') ORDER BY CODCAU ASC" : str1 + " WHERE TIPMOV IN ('ANN_RT_POS', 'ANN_RT_NEG') ORDER BY CODCAU ASC";
      DataTable dataTable2 = dataLayer.GetDataTable(strSQL);
      if (dataTable2.Rows.Count > 0)
      {
        for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
        {
          if (dataTable2.Rows[index]["DESC"].ToString() != "")
          {
            string str2 = dataTable2.Rows[index]["CODCAU"]?.ToString() + " " + dataTable2.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[index]["DESC"].ToString().Length - 1);
            cb.BeginText();
            BaseFont font2 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
            cb.SetFontAndSize(font2, 5.25f);
            cb.ShowTextAligned(0, str2.Trim(), 10f, (float) y, 0.0f);
            cb.EndText();
          }
          else
          {
            string text = "";
            cb.BeginText();
            BaseFont font3 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
            cb.SetFontAndSize(font3, 5.25f);
            cb.ShowTextAligned(0, text, 10f, (float) y, 0.0f);
            cb.EndText();
          }
          y -= 7;
        }
      }
      else
      {
        for (int index = 0; index <= 5; ++index)
        {
          cb.BeginText();
          BaseFont font4 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
          cb.SetFontAndSize(font4, 5.25f);
          cb.ShowTextAligned(0, "", 10f, (float) y, 0.0f);
          cb.EndText();
          y -= 7;
        }
      }
      cb.BeginText();
      BaseFont font5 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.SetFontAndSize(font5, 8.25f);
      cb.ShowTextAligned(1, "Per informazioni telefonare al Call Center 800.010270 - 800.313231", 420f, 10f, 0.0f);
      cb.EndText();
    }

    private void ScriviTestataRettificaAnnullata(
      ref iTextSharp.text.Table tabTestata,
      ref Cell Cell,
      ref iTextSharp.text.Font FontTestata)
    {
      Cell = new Cell((IElement) new Phrase("Matricola", FontTestata));
      Cell.HorizontalAlignment = 1;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 0);
      Cell = new Cell((IElement) new Phrase("Cognome e Nome", FontTestata));
      Cell.HorizontalAlignment = 0;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 1);
      Cell = new Cell((IElement) new Phrase("Periodo", FontTestata));
      Cell.HorizontalAlignment = 1;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 2);
      Cell = new Cell((IElement) new Phrase("Retribuzione attuale", FontTestata));
      Cell.HorizontalAlignment = 2;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 3);
      Cell = new Cell((IElement) new Phrase("Retribuzione precedente", FontTestata));
      Cell.HorizontalAlignment = 2;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 5);
      Cell = new Cell((IElement) new Phrase("Differenza", FontTestata));
      Cell.HorizontalAlignment = 2;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      Cell.Colspan = 2;
      tabTestata.AddCell(Cell, 0, 7);
      Cell = new Cell((IElement) new Phrase("Aliquota", FontTestata));
      Cell.HorizontalAlignment = 1;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 9);
      Cell = new Cell((IElement) new Phrase("Contributi", FontTestata));
      Cell.HorizontalAlignment = 2;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.Colspan = 2;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 10);
      Cell = new Cell((IElement) new Phrase("Tasso Sanzione", FontTestata));
      Cell.HorizontalAlignment = 1;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 12);
      Cell = new Cell((IElement) new Phrase("Sanzione", FontTestata));
      Cell.HorizontalAlignment = 2;
      Cell.VerticalAlignment = 5;
      Cell.BorderWidth = 0.0f;
      Cell.Colspan = 2;
      Cell.BorderWidthBottom = 0.5f;
      tabTestata.AddCell(Cell, 0, 13);
    }

    private void scriviNumPagRettificaAnnullata(ref PdfContentByte cb, int cntNumPag, int TotPag)
    {
      string text = "Pag. " + cntNumPag.ToString() + " di " + TotPag.ToString();
      cb.BeginText();
      BaseFont font = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      cb.SetFontAndSize(font, 7.25f);
      cb.ShowTextAligned(1, text, 800f, 10f, 0.0f);
      cb.EndText();
    }

    private void ScriviSanzioneRettificaAnnullata(
      ref PdfContentByte cb,
      string CODCAUSAN,
      string DATCONMOV,
      string NUMMOV,
      string DATSAN,
      string NUMSAN,
      string MESDEN,
      string ANNDEN,
      int CODPOS,
      string DAL,
      string AL)
    {
      string text1 = "";
      DateTimeFormatInfo dateTimeFormat = new CultureInfo("it-IT", false).DateTimeFormat;
      BaseFont font = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANA"), "Cp1252", false);
      string str1 = !(DATCONMOV != "") ? "" : Convert.ToDateTime(DATCONMOV).ToString();
      string str2 = !(DATSAN != "") ? "" : Convert.ToDateTime(DATSAN).ToString();
      string str3 = dateTimeFormat.MonthNames[Convert.ToInt32(DAL.Substring(3, 2)) - 1].ToString().ToUpper() + " " + DAL.Substring(6, 4);
      string str4 = dateTimeFormat.MonthNames[Convert.ToInt32(AL.Substring(3, 2)) - 1].ToString().ToUpper() + " " + AL.Substring(6, 4);
      string text2;
      if (NUMSAN == "")
        text2 = "Nota di rettifica n. " + NUMMOV.Trim() + " emessa il " + str1 + " relativa al periodo " + str3 + IIf(str3 == str4, (object) "", (object) (" / " + str4))?.ToString();
      else
        text2 = "Nota di rettifica n. " + NUMMOV.Trim() + " e Nota sanzione n. " + NUMSAN.Trim() + " emesse il " + str2 + " relative al periodo " + str3 + IIf(str3 == str4, (object) "", (object) (" / " + str4))?.ToString();
      string text3 = "Posizione assicurativa " + CODPOS.ToString();
      cb.BeginText();
      cb.SetFontAndSize(font, 8.25f);
      cb.ShowTextAligned(0, text2, 10f, 500f, 0.0f);
      cb.EndText();
      if (text1 != "")
      {
        cb.BeginText();
        cb.SetFontAndSize(font, 8.25f);
        cb.ShowTextAligned(0, text1, 10f, 490f, 0.0f);
        cb.EndText();
        cb.BeginText();
        cb.SetFontAndSize(font, 8.25f);
        cb.ShowTextAligned(0, text3, 10f, 480f, 0.0f);
        cb.EndText();
      }
      else
      {
        cb.BeginText();
        cb.SetFontAndSize(font, 8.25f);
        cb.ShowTextAligned(0, text3, 10f, 490f, 0.0f);
        cb.EndText();
      }

      static object IIf(bool expression, object truePart, object falsePart) => !expression ? falsePart : truePart;
    }

    private void ScriviTotaliRettificaAnnullata(
      ref iTextSharp.text.Table TabTotali,
      ref Cell CellTotali,
      ref iTextSharp.text.Font FontDettaglio,
      ref iTextSharp.text.Font FontNascondi,
      ref iTextSharp.text.Font FontTestata,
      string CODCAU,
      string IMPCONTOT,
      string IMPSANTOT,
      Decimal IMPADDDEL,
      ref PdfContentByte cb)
    {
      string str1 = "";
      DataTable dataTable1 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 3, 0);
      CellTotali = new Cell((IElement) new Phrase("", FontDettaglio));
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 4, 0);
      CellTotali = new Cell((IElement) new Phrase("Importo contributo", FontDettaglio));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 1);
      string str2 = "Importo addizionale";
      CellTotali = new Cell((IElement) new Phrase(str2, FontDettaglio));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 1);
      if (CODCAU == "")
      {
        str1 = "Sanzioni";
      }
      else
      {
        string strSQL = "SELECT VALUE(TASSO, 0.00) AS TASSO, TIPMOV, DESCAUREP AS DESC FROM TIPMOVCAU " + " WHERE CODCAU ='" + CODCAU + "' AND CURRENT_DATE BETWEEN DATINI AND DATFIN";
        dataTable1.Clear();
        DataTable dataTable2 = dataLayer.GetDataTable(strSQL);
        if (dataTable2.Rows.Count > 0)
        {
          for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
          {
            string str3 = dataTable2.Rows[index]["TIPMOV"].ToString().Trim();
            if (!(str3 == "SAN_MD"))
            {
              if (str3 == "SAN_RD")
                str1 = !(dataTable2.Rows[index]["DESC"].ToString() != "") ? "" : dataTable2.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[index]["DESC"].ToString().Length - 1);
            }
            else
              str1 = !(dataTable2.Rows[index]["DESC"].ToString() != "") ? "" : dataTable2.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[index]["DESC"].ToString().Length - 1);
          }
        }
        else
          str1 = "";
      }
      CellTotali = new Cell((IElement) new Phrase(str1, FontDettaglio));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 1);
      CellTotali = new Cell((IElement) new Phrase(".....", FontNascondi));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 6;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 3, 1);
      CellTotali = new Cell((IElement) new Phrase("Importo complessivo", FontTestata));
      CellTotali.HorizontalAlignment = 0;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 4, 1);
      CellTotali = new Cell((IElement) new Phrase("€", FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 2);
      CellTotali = new Cell((IElement) new Phrase("€", FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 2);
      CellTotali = new Cell((IElement) new Phrase("€", FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 2);
      CellTotali = new Cell();
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 3, 2);
      CellTotali = new Cell((IElement) new Phrase("€", FontTestata));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 4, 2);
      string str4 = (Convert.ToDecimal((Convert.ToDecimal(IMPCONTOT) + Convert.ToDecimal(IMPADDDEL)).ToString()) + Convert.ToDecimal(IMPSANTOT)).ToString();
      IMPCONTOT = Convert.ToDecimal(IMPCONTOT).ToString("#,##0.#0");
      string str5 = Convert.ToDecimal(IMPADDDEL).ToString("#,##0.#0");
      CellTotali = new Cell((IElement) new Phrase(IMPCONTOT, FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 0, 3);
      CellTotali = new Cell((IElement) new Phrase(str5, FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 1, 3);
      CellTotali = new Cell((IElement) new Phrase(Convert.ToDecimal(IMPSANTOT).ToString("#,##0.#0"), FontDettaglio));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 2, 3);
      CellTotali = new Cell((IElement) new Phrase("....", FontNascondi));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 6;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 3, 3);
      CellTotali = new Cell((IElement) new Phrase(Convert.ToDecimal(str4).ToString("#,##0.#0"), FontTestata));
      CellTotali.HorizontalAlignment = 2;
      CellTotali.VerticalAlignment = 5;
      CellTotali.BorderWidth = 0.0f;
      TabTotali.AddCell(CellTotali, 4, 3);
      BaseFont font1 = BaseFont.CreateFont(this.EstraiFilePGM_File("VERDANAB"), "Cp1252", false);
      iTextSharp.text.Font font2 = new iTextSharp.text.Font(font1, 7.75f, 1);
      iTextSharp.text.Font font3 = new iTextSharp.text.Font(font1, 7.75f, 0, Color.WHITE);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 5, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 6, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 7, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 8, 0);
      CellTotali = new Cell((IElement) new Phrase("0", font3));
      CellTotali.BorderWidth = 0.0f;
      CellTotali.Colspan = 4;
      TabTotali.AddCell(CellTotali, 9, 0);
    }

    public string FTP_Upload(
      int codiceFTP,
      string pathFileOrigine,
      string nomeFileFTP,
      string subFolderFTP = "",
      bool folderNow = false)
    {
      WebClient webClient = new WebClient();
      DataTable dataTable1 = new DataTable();
      string userName = "";
      string password = "";
      DataLayer dataLayer = new DataLayer();
      if (codiceFTP == 0)
      {
        webClient.Credentials = (ICredentials) new NetworkCredential(userName, password);
        webClient.UploadFile(nomeFileFTP, pathFileOrigine);
        return nomeFileFTP;
      }
      string str = subFolderFTP;
      string strSQL = "SELECT current_date as DATA_SISTEMA, FTPIND.* FROM FTPIND" + " WHERE CODFTP = " + codiceFTP.ToString();
      DataTable dataTable2 = dataLayer == null ? dataLayer.GetDataTable(strSQL) : dataLayer.GetDataTable(strSQL);
      if (folderNow)
      {
        string[] strArray = new string[6];
        strArray[0] = str;
        DateTime dateTime = Convert.ToDateTime(dataTable2.Rows[0]["DATA_SISTEMA"]);
        strArray[1] = dateTime.Year.ToString();
        strArray[2] = "/";
        Dictionary<int, string> mesi = Utile.GetMesi();
        dateTime = Convert.ToDateTime(dataTable2.Rows[0]["DATA_SISTEMA"]);
        int int32 = Convert.ToInt32(dateTime.Month);
        strArray[3] = mesi[int32].ToUpper();
        strArray[4] = "/";
        dateTime = Convert.ToDateTime(dataTable2.Rows[0]["DATA_SISTEMA"]);
        strArray[5] = dateTime.Day.ToString().PadLeft(2, '0');
        str = string.Concat(strArray);
      }
      webClient.Credentials = (ICredentials) new NetworkCredential(userName, password);
      if (str != "")
      {
        webClient.UploadFile(dataTable2.Rows[0]["PATHFTP"].ToString().Trim() + str + "/" + nomeFileFTP, pathFileOrigine);
        return dataTable2.Rows[0]["PATHFTP"].ToString().Trim() + str + "/" + nomeFileFTP;
      }
      webClient.UploadFile(dataTable2.Rows[0]["PATHFTP"].ToString().Trim() + nomeFileFTP, pathFileOrigine);
      return dataTable2.Rows[0]["PATHFTP"].ToString().Trim() + nomeFileFTP;
    }

    private static string Mod_DB2Date(DateTime Data, bool ConApiciFinali = true, bool IsTimeStamp = false)
    {
      string str = Data.Year.ToString("0000") + "-" + Data.Month.ToString("00") + "-" + Data.Day.ToString("00");
      if (IsTimeStamp)
        str = str + "-" + Data.Hour.ToString().PadLeft(2, Convert.ToChar("0")) + "." + Data.Minute.ToString().PadLeft(2, Convert.ToChar("0")) + "." + Data.Second.ToString().PadLeft(2, Convert.ToChar("0")) + ".037500";
      return ConApiciFinali ? "'" + str + "'" : str;
    }

    private string Mod_DoublePeakForSql(string strValue)
    {
      strValue = strValue.Replace("'", "''");
      strValue = "'" + strValue.Trim() + "'";
      return strValue;
    }

    private void ScriviPieDiPaginaSanzioneArretrati(DataLayer db, ref PdfContentByte cb)
    {
      DataTable dataTable1 = new DataTable();
      cb.BeginText();
      BaseFont font1 = BaseFont.CreateFont(Pdf.BasePath + "StampePdf\\Font\\Lato-Regular.ttf", "Cp1252", false);
      cb.SetFontAndSize(font1, 5.25f);
      cb.ShowTextAligned(0, "Legenda causali:", 10f, 71f, 0.0f);
      cb.EndText();
      int y = 64;
      string strSQL = "SELECT DISTINCT CODCAU, VALUE(DESCAUREP, ' ') AS DESC FROM TIPMOVCAU WHERE TIPMOV IN('SAN_RD','SAN_MD','ANN_SAN_RD','ANN_SAN_MD') ORDER BY CODCAU ASC";
      DataTable dataTable2 = db.GetDataTable(strSQL);
      if (dataTable2.Rows.Count > 0)
      {
        int num = dataTable2.Rows.Count - 1;
        for (int index = 0; index <= num; ++index)
        {
          if (dataTable2.Rows[index]["DESC"].ToString() != "")
          {
            string str = dataTable2.Rows[index]["CODCAU"]?.ToString() + " " + dataTable2.Rows[index]["DESC"].ToString().Substring(0, 1) + dataTable2.Rows[index]["DESC"].ToString().ToLower().Substring(1, dataTable2.Rows[index]["DESC"].ToString().Length - 1);
            cb.BeginText();
            BaseFont font2 = BaseFont.CreateFont(Pdf.BasePath + "StampePdf\\Font\\Lato-Regular.ttf", "Cp1252", false);
            cb.SetFontAndSize(font2, 5.25f);
            cb.ShowTextAligned(0, str.Trim(), 10f, (float) y, 0.0f);
            cb.EndText();
          }
          else
          {
            string text = "";
            cb.BeginText();
            BaseFont font3 = BaseFont.CreateFont(Pdf.BasePath + "StampePdf\\Font\\Lato-Regular.ttf", "Cp1252", false);
            cb.SetFontAndSize(font3, 5.25f);
            cb.ShowTextAligned(0, text, 10f, (float) y, 0.0f);
            cb.EndText();
          }
          y -= 7;
        }
      }
      else
      {
        for (int index = 0; index <= 5; ++index)
        {
          cb.BeginText();
          BaseFont font4 = BaseFont.CreateFont(Pdf.BasePath + "StampePdf\\Font\\Lato-Regular.ttf", "Cp1252", false);
          cb.SetFontAndSize(font4, 5.25f);
          cb.ShowTextAligned(0, "", 10f, (float) y, 0.0f);
          cb.EndText();
          y -= 7;
        }
      }
      cb.BeginText();
      BaseFont font5 = BaseFont.CreateFont(Pdf.BasePath + "StampePdf\\Font\\Lato-Regular.ttf", "Cp1252", false);
      cb.SetFontAndSize(font5, 8.25f);
      cb.ShowTextAligned(1, "Per informazioni telefonare al Call Center 800.010270 - 800.313231", 420f, 10f, 0.0f);
      cb.EndText();
    }
  }
}
