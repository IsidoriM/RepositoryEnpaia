// Decompiled with JetBrains decompiler
// Type: TFI.DAL.AziendaConsulente.TotaleArretratiDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;
using log4net;
using OCM.TFI.OCM.AziendaConsulente;
using OCM.TFI.OCM.Protocollo;
using OCM.TFI.OCM.Utilities;
using TFI.CRYPTO.Crypto;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.AziendaConsulente;
using Utilities;
using static Utilities.DbParameters;

namespace TFI.DAL.AziendaConsulente
{
  public class TotaleArretratiDAL
  {
    private DataLayer objDataAccess;
    private DateTimeFormatInfo objDTFI = new CultureInfo("it-IT", false).DateTimeFormat;
    private DataView objDvPercen;
    private DataView objDvFap;
    private DataSet objDs;
    private string strStaDen;
    private TotaleArretrati listaTotali = new TotaleArretrati();
    private static readonly ILog _log = LogManager.GetLogger("RollingFile");


    public TotaleArretrati Page_Load(
      string codPos,
      int hdnAnno,
      int hdnMese,
      int hdnProden,
      string ret = "")
    {
      this.listaTotali.hdnAnno = hdnAnno;
      this.listaTotali.hdnMese = hdnMese;
      this.listaTotali.hdnProden = hdnProden;
      this.listaTotali.btnDettaglioVisibile = true;
      this.listaTotali.lblCreditiVisibile = true;
      this.listaTotali.txtCreditiVisibile = true;
      this.listaTotali.lblTotaleVisibile = true;
      this.listaTotali.lblTotPagareVisibile = true;
      this.listaTotali.lblAddizionale = "0,00";
      this.listaTotali.lblContributi = "0,00";
      this.listaTotali.lblTotContributi = "0,00";
      this.listaTotali.lblTotPagare = "0,00";
      this.listaTotali.lblTotDovuto = "0,00";
      this.listaTotali.lbltotpag = "0,00";
      this.listaTotali.lblDecurtato = "0,00";
      this.listaTotali.txtCrediti = "0,00";
      if (this.listaTotali.hdnMav == "ok")
      {
        this.listaTotali.btnStampa_MAV = true;
        this.listaTotali.btnMav = false;
      }
      else
      {
        this.listaTotali.btnStampa_MAV = false;
        this.listaTotali.btnMav = true;
      }
      this.LoadData(ret);
      this.InitPage();
      if (this.listaTotali.hdnMav == "ok")
      {
        this.listaTotali.txtDataVersamento = DateTime.Today.GetDateTimeFormats((IFormatProvider) this.objDTFI)[0];
        this.listaTotali.txtImportoVersato = this.listaTotali.lblTotPagare;
        this.listaTotali.btnStampa_MAV = true;
        this.listaTotali.btnMav = false;
      }
      return this.listaTotali;
    }

    private void LoadData(string ret = "")
    {
      var utente = HttpContext.Current.Session["utente"] as OCM.Utente.Utente;
      var strSql = string.Empty;
      decimal decSanzione = 0M;
      try
      {
        objDataAccess = new DataLayer();
        strSql = "SELECT VALUE(VALORE, '0') FROM PARGEN A INNER JOIN PARGENDET B ON A.CODPAR = B.CODPAR WHERE A.DENPAR = 'ADDIZIONALE'";
        var strAddizionale = objDataAccess.Get1ValueFromSQL(strSql, CommandType.Text);
        var decAddizionale = Convert.ToDecimal(strAddizionale);
        listaTotali.Label5 = $"Addizionale del {decAddizionale} % €";
        // this.listaTotali.Label5 = "Addizionale del " + Convert.ToDecimal(objDataAccess.Get1ValueFromSQL("SELECT VALUE(VALORE, '0') FROM PARGEN A " + "INNER JOIN PARGENDET B ON A.CODPAR = B.CODPAR " + "WHERE A.DENPAR = 'ADDIZIONALE'", CommandType.Text)).ToString() + "% € ";
        strSql =
          "SELECT IMPDEC, IMPCON, IMPADDREC, IMPDEC, TIPMOV, IMPSANDET, STADEN, VALUE(CODMODPAG, 0) AS CODMODPAG, " +
          "DATAPE, DATEROARR, IMPVER, DATVER, UFFPOS, CITDIC, PROTRIC, FLGRIC, PRODIC, IBAN, NUMSANANN, SANSOTSOG, " +
          "VALUE(ESIRET, 'N') AS ESIRET, NOTE, IMPCONDEL, IMPADDRECDEL, IMPABBDEL, IMPASSCONDEL, VALUE(IMPSANRET, 0) AS IMPSANRET, NUMSAN, CODLINE," +
          " DATCHI, NUMRIC, CODMODPAG FROM DENTES " +
          $"WHERE CODPOS = {utente.CodPosizione}" +
          $" AND ANNDEN = {listaTotali.hdnAnno} " +
          $"AND MESDEN = {listaTotali.hdnMese} " +
          $"AND PRODEN = {listaTotali.hdnProden}";
        DataTable dataTable = objDataAccess.GetDataTable(strSql);
        // DataTable dataTable = objDataAccess.GetDataTable(
        //   "SELECT IMPDEC, IMPCON, IMPADDREC, IMPDEC, TIPMOV, IMPSANDET, STADEN, VALUE(CODMODPAG, 0) AS CODMODPAG, DATAPE, DATEROARR, IMPVER, DATVER, UFFPOS, CITDIC, PROTRIC, FLGRIC, PRODIC, IBAN, NUMSANANN, SANSOTSOG, VALUE(ESIRET, 'N') AS ESIRET, NOTE, IMPCONDEL, IMPADDRECDEL, IMPABBDEL, IMPASSCONDEL, VALUE(IMPSANRET, 0) AS IMPSANRET, NUMSAN, CODLINE FROM DENTES " +
        //   "WHERE CODPOS = " + utente.CodPosizione + " AND ANNDEN = " + this.listaTotali.hdnAnno.ToString() + " AND MESDEN = " + this.listaTotali.hdnMese.ToString() + " And PRODEN = " + this.listaTotali.hdnProden.ToString());
        if (dataTable.Rows.Count > 0)
        {
          listaTotali.DataChiusura = dataTable.Rows[0].RawDateElementAt("DATCHI", StandardUse.Readable);
          listaTotali.Protocollo = new Protocollo(dataTable.Rows[0].ElementAt("PROTRIC"))
          {
            Success = !string.IsNullOrWhiteSpace(dataTable.Rows[0].ElementAt("PROTRIC")),
            NumeroProgressivoProtoccollo = dataTable.Rows[0].ElementAt("NUMRIC")
          };
          listaTotali.ModalitaPagamento = dataTable.Rows[0].IntElementAt("CODMODPAG");
          
          listaTotali.hdnTipMov = dataTable.Rows[0]["TIPMOV"].ToString().Trim();
          listaTotali.hdnProt = dataTable.Rows[0]["PROTRIC"].ToString().Trim();
          listaTotali.hdnFLGRIC = dataTable.Rows[0]["FLGRIC"].ToString().Trim();
          listaTotali.strStaDen = dataTable.Rows[0]["STADEN"].ToString().Trim();
          listaTotali.lblDataDenuncia = dataTable.Rows[0]["DATAPE"].ToString().Substring(0, 10);
          if (DBNull.Value.Equals(dataTable.Rows[0]["NUMSANANN"]) && !DBNull.Value.Equals(dataTable.Rows[0]["NUMSAN"]))
            decSanzione = Convert.ToDecimal(dataTable.Rows[0]["IMPSANDET"]);
          decimal num2;
          if (ret == "S" && dataTable.Rows[0]["ESIRET"].ToString() == "S")
          {
            this.listaTotali.tbIntestazionePagamentoVisibile = false;
            this.listaTotali.btnDettaglioVisibile = false;
            this.listaTotali.lblCreditiVisibile = false;
            this.listaTotali.txtCreditiVisibile = false;
            this.listaTotali.lblTotaleVisibile = false;
            this.listaTotali.lblTotPagareVisibile = false;
            this.listaTotali.tbPagamentoVisibile = false;
            this.listaTotali.lblContributi = (Convert.ToDecimal(dataTable.Rows[0]["IMPCON"]) + Convert.ToDecimal(dataTable.Rows[0]["IMPCONDEL"])).ToString();
            this.listaTotali.lblAddizionale = (Convert.ToDecimal(dataTable.Rows[0]["IMPADDREC"]) + Convert.ToDecimal(dataTable.Rows[0]["IMPADDRECDEL"])).ToString();
            this.listaTotali.lblTotContributi = (Convert.ToDecimal(dataTable.Rows[0]["IMPADDREC"]) + Convert.ToDecimal(dataTable.Rows[0]["IMPADDRECDEL"]) + Convert.ToDecimal(dataTable.Rows[0]["IMPCON"]) + Convert.ToDecimal(dataTable.Rows[0]["IMPCONDEL"])).ToString();
            // decimal num3 = decSanzione + Convert.ToDecimal(dataTable.Rows[0]["IMPSANRET"]);
            decSanzione += Convert.ToDecimal(dataTable.Rows[0]["IMPSANRET"]);
            // TotaleArretrati listaTotali = this.listaTotali;
            // num2 = Convert.ToDecimal(num3);
            // string str = num2.ToString();
            // listaTotali.lblTotSanzione = str;
            listaTotali.lblTotSanzione = decSanzione.ToString();
          }
          else
          {
            listaTotali.lblContributi = Convert.ToDecimal(dataTable.Rows[0]["IMPCON"]).ToString();
            listaTotali.lblAddizionale = Convert.ToDecimal(dataTable.Rows[0]["IMPADDREC"]).ToString();
            TotaleArretrati listaTotali1 = listaTotali;
            var decTotContributi = Convert.ToDecimal(dataTable.Rows[0]["IMPCON"]) + Convert.ToDecimal(dataTable.Rows[0]["IMPADDREC"]);
            string strTotContributi = decTotContributi.ToString();
            listaTotali1.lblTotContributi = strTotContributi;
            listaTotali.lblTotSanzione = "0,00";
            TotaleArretrati listaTotali2 = listaTotali;
            // num2 = Convert.ToDecimal(decSanzione);
            // string str2 = num2.ToString();
            listaTotali2.lblTotSanzione = decSanzione.ToString();
          }
          TotaleArretrati listaTotali3 = listaTotali;
          num2 = Convert.ToDecimal(listaTotali.lblTotContributi);
          string str3 = num2.ToString().Replace(".", "");
          listaTotali3.lblTotDovuto = str3;
          TotaleArretrati listaTotali4 = listaTotali;
          num2 = Convert.ToDecimal(listaTotali.lblTotDovuto) + Convert.ToDecimal(this.listaTotali.lblTotSanzione);
          string str4 = num2.ToString();
          listaTotali4.lblTotPagare = str4;
          TotaleArretrati listaTotali5 = listaTotali;
          num2 = Convert.ToDecimal(listaTotali.lblTotDovuto) + Convert.ToDecimal(this.listaTotali.lblTotSanzione);
          string str5 = num2.ToString();
          listaTotali5.lbltotpag = str5;
          if (!DBNull.Value.Equals(dataTable.Rows[0]["IMPVER"])) 
            listaTotali.txtImportoVersato = dataTable.Rows[0]["IMPVER"].ToString();
          if (!DBNull.Value.Equals(dataTable.Rows[0]["DATVER"]))
          {
            listaTotali.txtDataVersamento = dataTable.Rows[0]["DATVER"].ToString().Substring(0, 10);
            listaTotali.txtNote = dataTable.Rows[0]["NOTE"]?.ToString() ?? "";
          }
          if (Convert.ToInt32(dataTable.Rows[0]["CODMODPAG"]) != 0 && 
              (!(ret == "S") || !(dataTable.Rows[0]["ESIRET"].ToString() == "S")) && 
              dataTable.Rows[0]["CODMODPAG"].ToString() == "1" 
              && !DBNull.Value.Equals(dataTable.Rows[0]["CODLINE"]))
          {
            listaTotali.btnStampa_MAV = true;
            listaTotali.btnMav = false;
            listaTotali.hdnMav = "S";
          }
          if (dataTable.Rows[0]["STADEN"].ToString() == "A")
          {
            listaTotali.strStaDen2 = dataTable.Rows[0]["STADEN"].ToString();
            AbilitaVersamento();
            var crediti = Convert.ToDecimal(dataTable.Rows[0]["IMPDEC"]);
            if (crediti > 0) listaTotali.lblTotPagare = (Convert.ToDecimal(listaTotali.lblTotPagare) - crediti).ToString();
            listaTotali.txtCrediti = dataTable.Rows[0]["IMPDEC"].ToString();
          }
        }

        strSql = "SELECT CHAR(ULTAGG) AS ULTAGG FROM DENTES " +
                 $"WHERE CODPOS = {utente.CodPosizione} " +
                 $"AND ANNDEN = {listaTotali.hdnAnno} " +
                 $"AND MESDEN = {listaTotali.hdnMese} " +
                 $"AND PRODEN = {listaTotali.hdnProden}";
        HttpContext.Current.Session["Timestamp"] = objDataAccess.Get1ValueFromSQL(strSql, CommandType.Text);
        // string str6 = $"SELECT CHAR(ULTAGG) AS ULTAGG FROM DENTES WHERE CODPOS = {utente.CodPosizione}";
        // int num4 = this.listaTotali.hdnAnno;
        // string str7 = num4.ToString();
        // string str8 = str6 + " AND ANNDEN = " + str7;
        // num4 = this.listaTotali.hdnMese;
        // string str9 = num4.ToString();
        // string str10 = str8 + " AND MESDEN = " + str9;
        // num4 = this.listaTotali.hdnProden;
        // string str11 = num4.ToString();
        // HttpContext.Current.Session["Timestamp"] = objDataAccess.Get1ValueFromSQL(str10 + " AND PRODEN = " + str11, CommandType.Text);
        if (HttpContext.Current.Session["Timestamp"] != null)
          return;
        HttpContext.Current.Session["Timestamp"] = "0";
      }
      catch (Exception ex)
      {
        HttpContext.Current.Session["LastException"] = ex;
      }
    }

    private void InitPage()
    {
      var utente = HttpContext.Current.Session["utente"] as OCM.Utente.Utente;
      var strStaDen = listaTotali.strStaDen;
      if (strStaDen != "N")
      {
        if (strStaDen != "A")
        {
          if (strStaDen == "AP")
          {
            listaTotali.txtNoteReadonly = true;
            listaTotali.tbTotaliRows5Visible = true;
            listaTotali.btnConfermaTotaliVisible = false;
            listaTotali.btnCreditiVisible = false;
            listaTotali.txtCreditiReadonly = true;
            listaTotali.txtDataVersamentoReadonly = true;
            listaTotali.txtImportoVersatoReadonly = true;
            listaTotali.tbIntestazionePagamentoVisibile = true;
            listaTotali.tbPagamentoVisibile = true;
          }
        }
        else
        {
          listaTotali.tbIntestazionePagamentoVisibile = true;
          listaTotali.tbPagamentoVisibile = true;
          listaTotali.btnCreditiVisible = false;
          listaTotali.txtCreditiReadonly = true;
          listaTotali.txtNoteReadonly = true;
          listaTotali.tbTotaliRows5Visible = true;
          listaTotali.btnConfermaTotaliVisible = false;
        }
      }
      else
      {
        this.listaTotali.tbTotaliRows5Visible = false;
        this.listaTotali.btnConfermaTotaliVisible = true;
        listaTotali.btnCreditiVisible = true;

      }
      int num = utente.Tipo != "E" ? 1 : 0;
    }

    private void btnCrediti()
    {
      this.listaTotali.lblDecurtato = Convert.ToDecimal(this.listaTotali.txtCrediti).ToString();
      if (Convert.ToDecimal(this.listaTotali.lblDecurtato) <= Convert.ToDecimal(this.listaTotali.lbltotpag))
        this.listaTotali.lblTotPagare = (Convert.ToDecimal(this.listaTotali.lbltotpag) - Convert.ToDecimal(this.listaTotali.lblDecurtato)).ToString();
      else
        this.listaTotali.lblTotPagare = "0,00";
    }

    public void btnConfermaTotali(
      string codPos,
      int hdnAnno,
      int hdnMese,
      int hdnProden,
      string TEXTAREA1,
      string txtCrediti,
      ref string ErrorMSG,
      ref string SuccessMSG)
    {
      if (!this.VerificaArretrati(codPos, hdnAnno, hdnMese, hdnProden, ref ErrorMSG, ref SuccessMSG))
        return;
      this.ConfermaDenuncia(codPos, hdnAnno, hdnMese, hdnProden, TEXTAREA1, txtCrediti, ref ErrorMSG, ref SuccessMSG);
    }

    private bool ConfermaDenuncia(
      string codPos,
      int hdnAnno,
      int hdnMese,
      int hdnProden,
      string TEXTAREA1,
      string txtCrediti,
      ref string ErrorMSG,
      ref string SuccessMSG)
    {
      var utente = HttpContext.Current.Session["utente"] as OCM.Utente.Utente;
      // clsIDOC clsIdoc = (clsIDOC) null;
      string str1 = null;
      bool flag = false;
      bool blnCommit = false;
      List<ParametriGenerali> listaParametriGenerali = new List<ParametriGenerali>();
      string PARTITA_MOV = "";
      int PROGMOV_MOV = 0;
      string CODCAUS = "";
      try
      {
        objDataAccess = new DataLayer();
        var strSql = "SELECT CHAR(ULTAGG) AS TIMESTAMP, CHAR(DATAPE) AS TMSTAPE, DENTES.* FROM DENTES " + 
                     $"WHERE CODPOS = {utente.CodPosizione} AND ANNDEN = {hdnAnno} AND MESDEN = {hdnMese} AND PRODEN = {hdnProden}";
        DataTable dataTable1 = objDataAccess.GetDataTable(strSql);
        if (dataTable1.Rows.Count > 0)
        {
          if ((dataTable1.Rows[0]["TIMESTAMP"].ToString() ?? "0") == HttpContext.Current.Session["Timestamp"].ToString())
          {
            if (VerificaArretrati(codPos, hdnAnno, hdnMese, hdnProden, ref ErrorMSG, ref SuccessMSG))
            {
              objDataAccess.StartTransaction();
              flag = true;
              string DATORARIC = objDataAccess.Get1ValueFromSQL("SELECT CHAR(CURRENT_TIMESTAMP) FROM DENTES FETCH FIRST 1 ROWS ONLY", CommandType.Text);
              string DATCHI = !(utente.Tipo.Trim() == "A") ? dataTable1.Rows[0]["TMSTAPE"].ToString() : DATORARIC;
              blnCommit = WriteDIPA_SalvataggioTotale.WRITE_CONFERMA_DENUNCIA(this.objDataAccess, utente, Convert.ToInt32(utente.CodPosizione), hdnAnno, hdnMese, hdnProden, "AR", DATCHI, utente.Tipo.ToString().Trim(), "A", Convert.ToDecimal(txtCrediti), DATORARIC, dataTable1.Rows[0]["RICSANUTE"]?.ToString() ?? "", TEXTAREA1); 
              if (blnCommit)
              {
                blnCommit = objDataAccess.WriteTransactionData("DELETE FROM DENDET WHERE IMPRET = 0 AND CODPOS = " + utente.CodPosizione + " AND ANNDEN = " + hdnAnno.ToString() + " AND MESDEN = " + hdnMese.ToString() + " AND PRODEN = " + hdnProden.ToString(), CommandType.Text);
                blnCommit = objDataAccess.WriteTransactionData("UPDATE DENTES SET NUMRIGDET = (SELECT COUNT(*) FROM DENDET WHERE CODPOS = " + utente.CodPosizione + " AND ANNDEN = " + hdnAnno.ToString() + " AND MESDEN = " + hdnMese.ToString() + " AND PRODEN = " + hdnProden.ToString() + ") WHERE CODPOS = " + utente.CodPosizione + " AND ANNDEN = " + hdnAnno.ToString() + " AND MESDEN = " + hdnMese.ToString() + " AND PRODEN = " + hdnProden.ToString(), CommandType.Text);
                if (blnCommit)
                {
                  blnCommit = WriteDIPA_SalvataggioTotale.WRITE_INSERT_ALISOS(objDataAccess, utente, Convert.ToInt32(utente.CodPosizione), hdnAnno, hdnMese, hdnProden);
                  if (blnCommit)
                  {
                    str1 = objDataAccess.Get1ValueFromSQL("SELECT VALORE FROM PARGENDET WHERE CODPAR = 11 AND current_date BETWEEN DATINI AND DATFIN", CommandType.Text);
                    if (str1.Trim() == "S")
                    {
                      blnCommit = WriteDIPA_SalvataggioTotale.WRITE_CONTABILITA_DENUNCIA(objDataAccess, utente, listaParametriGenerali, Convert.ToInt32(utente.CodPosizione), hdnAnno, hdnMese, hdnProden, dataTable1.Rows[0]["TIPMOV"].ToString().Trim(), Convert.ToDecimal(dataTable1.Rows[0]["IMPDIS"]), Convert.ToDecimal(dataTable1.Rows[0]["IMPABB"]), Convert.ToDecimal(dataTable1.Rows[0]["IMPADDREC"]), Convert.ToDecimal(dataTable1.Rows[0]["IMPASSCON"]), PARTITA_MOV, PROGMOV_MOV, CODCAUS);
                      if (blnCommit)
                      {
                        dataTable1 = objDataAccess.GetDataTable("SELECT COG, NOM, A.MAT, A.PRORAP FROM ISCT INNER JOIN MODPRE A ON ISCT.MAT = A.MAT INNER JOIN DENDET B ON A.CODPOS = B.CODPOS " + "AND A.MAT = B.MAT AND A.PRORAP = B.PRORAP WHERE A.CODSTAPRE = 4 AND A.DATANN IS NULL AND B.CODPOS = " + utente.CodPosizione + " AND B.ANNDEN = " + hdnAnno.ToString() + " AND B.MESDEN = " + hdnMese.ToString() + " AND B.PRODEN = " + hdnProden.ToString());
                        int num = dataTable1.Rows.Count - 1;
                        for (int index = 0; index <= num; ++index)
                        {
                          string str2 = objDataAccess.Get1ValueFromSQL("SELECT VALUE(MAX(PROSCA), 0) + 1 FROM SCADEN", CommandType.Text);
                          string denominazione = utente.Denominazione;
                          string str3 = denominazione.Substring(denominazione.IndexOf("-") + 1).Trim();
                          blnCommit = this.objDataAccess.WriteTransactionData("INSERT INTO SCADEN (PROSCA, CODPOS, MAT, RAGSOC, COG, NOM, PRORAP, DATINS, CODUTEINS, CODDOC, NOTE, DATSCA," + " DATRIS, CODUTERIS, DATANN, CODUTEANN, ULTAGG, UTEAGG) VALUES (" + str2 + ", " + utente.CodPosizione + ", " + dataTable1.Rows[index]["MAT"]?.ToString() + ", " + DBMethods.DoublePeakForSql(str3) + ", " + DBMethods.DoublePeakForSql(dataTable1.Rows[index]["COG"].ToString()) + ", " + DBMethods.DoublePeakForSql(dataTable1.Rows[index]["NOM"].ToString()) + ", Null, current_date, " + DBMethods.DoublePeakForSql(utente.CodPosizione.ToString()) + ", 4, Null, '9999-12-31', Null, Null, Null, Null, current_timestamp, " + DBMethods.DoublePeakForSql(utente.CodPosizione.Trim()) + ")", CommandType.Text);
                          if (!blnCommit)
                            break;
                        }
                      }
                    }
                    // if (blnCommit)
                    // {
                    //   iDB2DataAdapter iDb2DataAdapter = new iDB2DataAdapter();
                    //   // clsIdoc = new clsIDOC();
                    //   // clsIdoc.strUserCode = utente.CodPosizione.Trim();
                    //   DataTable dataTable2 = new DataTable();
                    //   DataTable idocDatiE1Pitype = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9005", Convert.ToInt32(utente.CodPosizione), 0, 0, 0, "", "", "", "", "", "", hdnAnno, hdnMese, hdnProden, "", "", "AR", "");
                    //   int num = idocDatiE1Pitype.Rows.Count - 1;
                    //   for (int index = 0; index <= num; ++index)
                    //   {
                    //     clsIdoc.WRITE_IDOC_TESTATA(this.objDataAccess, idocDatiE1Pitype.Rows[index]);
                    //     DataTable DT = idocDatiE1Pitype.Clone();
                    //     DT.ImportRow(idocDatiE1Pitype.Rows[index]);
                    //     clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "9005", DT, false);
                    //   }
                    // }
                  }
                }
              }
              if (blnCommit)
              {
                string strSQL = "SELECT * FROM DENDET" + " WHERE CODPOS = " + utente.CodPosizione + " AND ANNDEN = " + hdnAnno.ToString() + " AND MESDEN = " + hdnMese.ToString() + " AND PRODEN = " + hdnProden.ToString();
                dataTable1.Clear();
                DataTable dataTable3 = this.objDataAccess.GetDataTable(strSQL);
                int num = dataTable3.Rows.Count - 1;
                for (int index = 0; index <= num; ++index)
                {
                  blnCommit = WriteDIPA_SalvataggioTotale.AGGIORNA_RETANN(this.objDataAccess, utente, dataTable3.Rows[index]["CODPOS"].ToString(), dataTable3.Rows[index]["MAT"].ToString(), Convert.ToInt32(dataTable3.Rows[index]["PRORAP"]), Convert.ToInt32(dataTable3.Rows[index]["ANNCOM"]), Convert.ToDecimal(dataTable3.Rows[index]["IMPRET"]), Convert.ToDecimal(dataTable3.Rows[index]["IMPOCC"]), Convert.ToDecimal(dataTable3.Rows[index]["IMPFIG"]), "+", true);
                  if (!blnCommit)
                    break;
                }
              }
              // if (blnCommit)
              //   clsIdoc.Aggiorna_IDOC(ref this.objDataAccess);
              if (!string.IsNullOrEmpty(str1) && str1.Trim() == "S")
                blnCommit = !(this.objDataAccess.Get1ValueFromSQL("SELECT COUNT(*) FROM MOVIMSAP WHERE PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOV) + " AND PROGMOV = " + PROGMOV_MOV.ToString() + " AND CODCAUS = " + DBMethods.DoublePeakForSql(CODCAUS.Trim().PadLeft(2, '0')), CommandType.Text) == "0");
              blnCommit = this.objDataAccess.WriteTransactionData("UPDATE DENTES SET FLGRIC = 'N'" + " WHERE CODPOS = " + utente.CodPosizione + " AND ANNDEN = " + hdnAnno.ToString() + " AND MESDEN = " + hdnMese.ToString() + " AND PRODEN = " + hdnProden.ToString(), CommandType.Text);
              this.objDataAccess.EndTransaction(blnCommit);
              if (blnCommit)
              {
                HttpContext.Current.Session["tbArretrati"] = null;
                strStaDen = "A";
                SuccessMSG = "Conferma dati ad Enpaia effettuata.";
              }
              else
                ErrorMSG = "Si sono verificati dei problemi nella conferma dei dati!";
              LoadData();
            }
            else
            {
              ErrorMSG = "Impossibile salvare! Sono state apportate variazioni ai trattamenti economici del periodo.";
              return false;
            }
          }
          else
          {
            ErrorMSG = "Impossibile salvare! I dati sono stati modificati da un altro utente.";
            return false;
          }
        }
      }
      catch (Exception ex)
      {
        if (flag)
        {
          blnCommit = false;
          objDataAccess.EndTransaction(false);
        }
        HttpContext.Current.Session["LastException"] = (object) ex;
        ErrorMSG = ex.ToString();
      }
      return blnCommit;
    }

    private bool VerificaArretrati(
      string codPos,
      int hdnAnno,
      int hdnMese,
      int hdnProden,
      ref string ErrorMSG,
      ref string SuccessMSG)
    {
      TFI.OCM.Utente.Utente utente = (TFI.OCM.Utente.Utente) HttpContext.Current.Session["Utente"];
      bool flag1 = false;
      this.objDataAccess = new DataLayer();
      DataTable dataTable1 = this.objDataAccess.GetDataTable("SELECT * FROM DENDET WHERE CODPOS = " + utente.CodPosizione + " AND ANNDEN = " + hdnAnno.ToString() + " AND MESDEN = " + hdnMese.ToString() + " AND PRODEN = " + hdnProden.ToString() + " ORDER BY MAT, DAL");
      GeneraDenunciaDAL generaDenunciaDal = new GeneraDenunciaDAL();
      if (flag1)
      {
        string strDataDal = "01/01/" + dataTable1.Rows[0]["ANNCOM"]?.ToString();
        string strDataAl = "31/12" + dataTable1.Rows[0]["ANNCOM"]?.ToString();
        List<ParametriGenerali> listaParametriGen = (List<ParametriGenerali>) null;
        HttpContext.Current.Session["tbDipa"] = (object) generaDenunciaDal.GeneraDenunciaArr(strDataDal, strDataAl, ref listaParametriGen, codPos);
      }
      DataTable dataTable2 = new DataTable();
      DataTable DtSessione = (DataTable) HttpContext.Current.Session["tbArretrati"];
      bool flag2;
      if (!generaDenunciaDal.VerificaDenuncia(ref dataTable1, ref DtSessione))
      {
        ErrorMSG = "Attenzione! I dati relativi al periodo della denuncia sono stati modificati";
        flag2 = false;
      }
      else
        flag2 = true;
      return flag2;
    }

    private Decimal GetPercent(
      Decimal decMat,
      string strDatini,
      string str65Anni,
      ref object objCatForass)
    {
      Decimal percent = 0M;
      string[,] strArray = new string[4, 3];
      string str = "MAT = " + decMat.ToString() + " AND DATINI <= '" + strDatini + "' AND DATFIN > = '" + strDatini + "'";
      if (str65Anni == "S")
        str += " AND CATFORASS <> 'PRE'";
      this.objDvPercen.RowFilter = str;
      if (this.objDvPercen.Count > 0)
      {
        string[,] instance = (string[,]) Array.CreateInstance(typeof (string), this.objDvPercen.Count, 3);
        int num = this.objDvPercen.Count - 1;
        for (short index = 0; (int) index <= num; ++index)
        {
          instance.SetValue((object) Convert.ToString(this.objDvPercen[(int) index]["CATFORASS"]), (int) index, 0);
          instance.SetValue((object) Convert.ToString(this.objDvPercen[(int) index]["CODFORASS"]), (int) index, 1);
          instance.SetValue((object) Convert.ToString(this.objDvPercen[(int) index]["PERCEN"]), (int) index, 2);
          percent += Convert.ToDecimal(this.objDvPercen[(int) index]["PERCEN"]);
        }
        objCatForass = (object) instance;
      }
      this.objDvPercen.RowFilter = "";
      return percent;
    }

    private void AbilitaVersamento()
    {
      listaTotali.btnConfermaTotaliVisible = false;
      listaTotali.btnCreditiVisible = false;
      listaTotali.btnStampaArretratoVisible = true;
      listaTotali.btnStampaRicevutaVisible = true;
    }

    private void btnStampaRicevuta(ref string ErrorMSG, ref string SuccessMSG)
    {
      TFI.OCM.Utente.Utente utente = (TFI.OCM.Utente.Utente) HttpContext.Current.Session["Utente"];
      SqlConnection objConnection = new SqlConnection(Cypher.DeCryptPassword(ConfigurationManager.AppSettings["ConnProtocollo"]));
      SqlCommand objCommand = new SqlCommand();
      SqlDataAdapter objDataAdapter = new SqlDataAdapter();
      string str1 = (string) null;
      ClsProtocolloDll clsProtocolloDll = new ClsProtocolloDll();
      string str2 = (string) null;
      string DATPRO = (string) null;
      DataTable dtTable = new DataTable();
      try
      {
        if (this.listaTotali.hdnProt.ToString() == "")
        {
          string str3 = clsProtocolloDll.GeneraProtocolloLettera(utente.CodPosizione, utente.Denominazione.ToString().Substring(Convert.ToInt32(utente.Denominazione.ToString().Trim().IndexOf("-") + 1)), "", "", "", "", "AZ", "9.6", 3233012, 4792886, "RICEVUTA ARRETRATO - POS : " + utente.CodPosizione + " - PERIODO : " + this.listaTotali.hdnAnno.ToString() + "/" + this.listaTotali.hdnMese.ToString(), utente.CodPosizione.ToString(), ref str2, ref DATPRO, "P");
          this.objDataAccess = new DataLayer();
          this.objDataAccess.StartTransaction();
          bool blnCommit = this.objDataAccess.WriteTransactionData("UPDATE DENTES SET PROTRIC = " + DBMethods.DoublePeakForSql(str3) + ", " + " DATPROTRIC = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATPRO)) + ", " + " NUMPROTRIC = " + DBMethods.DoublePeakForSql(str2) + ", FLGRIC = 'S'" + " WHERE CODPOS = " + utente.CodPosizione + " AND ANNDEN = " + this.listaTotali.hdnAnno.ToString() + " AND MESDEN = " + this.listaTotali.hdnMese.ToString() + " AND PRODEN = " + this.listaTotali.hdnProden.ToString(), CommandType.Text);
          if (blnCommit)
          {
            Pdf.BasePath = HttpContext.Current.Request.PhysicalApplicationPath + "bin\\";
            str1 = clsProtocolloDll.PROT_PRES_GET_TEMP_PATH_APPLICATION() + "\\ARRETRATI_" + utente.CodPosizione + "_" + this.listaTotali.hdnAnno.ToString() + "_" + this.listaTotali.hdnMese.ToString() + "_" + this.listaTotali.hdnProden.ToString() + "_" + this.listaTotali.hdnTipMov + ".pdf";
            blnCommit = Pdf.CreaStampaRicevutaDipaProtocollo(Convert.ToInt32(utente.CodPosizione), this.listaTotali.hdnAnno, this.listaTotali.hdnMese, this.listaTotali.hdnProden, str3, str1);
          }
          if (blnCommit)
            blnCommit = clsProtocolloDll.Module_AllegaLetteraProtocollo(str1, str3, ref str2);
          this.objDataAccess.EndTransaction(blnCommit);
          if (!blnCommit)
            ;
        }
        else
        {
          string strSql = "SELECT A.SOTTOCARTELLA AS CARTELLA , A.NOMEFILE AS [NOME FILE] FROM TBALLEGATI A, TBPROTOCOLLIALLEGATI B WHERE A.IDALLEGATO=B.IDALLEGATO AND B.IDPROTOCOLLO =" + this.listaTotali.hdnProt.Split(';')[0];
          DataTable dataTable = clsProtocolloDll.SqlClient_GetDataTable(strSql, CommandType.Text, (string) null, ref objConnection, ref dtTable, ref objDataAdapter, ref objCommand);
          clsProtocolloDll.FTP_Download("ftp://192.168.1.6/SCANSIONI/" + dataTable.Rows[0]["CARTELLA"]?.ToString() + dataTable.Rows[0]["NOME FILE"]?.ToString());
        }
      }
      catch (Exception ex)
      {
        ErrorMSG = "Attenzione! Si è verificato il seguente errore: " + ex.Message;
      }
    }

    private string ReplaceSpecialChars(ref string strInput)
    {
      if (!string.IsNullOrEmpty(strInput.Trim()))
      {
        if (strInput.LastIndexOf("*") == strInput.Length - 1)
        {
          strInput = strInput.Substring(0, strInput.LastIndexOf("*")).Trim();
          if (strInput.Substring(strInput.Length - 1, 1) != " ")
            strInput += ".";
        }
        strInput = strInput.Replace("\\", "");
        strInput = strInput.Replace("|", "");
        strInput = strInput.Replace("#", "");
        strInput = strInput.Replace("?", "");
        strInput = strInput.Replace("!", "");
        strInput = strInput.Replace("$", "");
        strInput = strInput.Replace("^", "");
        strInput = strInput.Replace("*", ".");
        strInput = strInput.Replace("&", "E");
        strInput = strInput.Replace("<", "");
        strInput = strInput.Replace(">", "");
        strInput = strInput.Replace("à", "A'");
        strInput = strInput.Replace("À", "A'");
        strInput = strInput.Replace("Á", "A'");
        strInput = strInput.Replace("è", "E'");
        strInput = strInput.Replace("È", "E'");
        strInput = strInput.Replace("É", "E'");
        strInput = strInput.Replace("é", "E'");
        strInput = strInput.Replace("ì", "I'");
        strInput = strInput.Replace("Ì", "I'");
        strInput = strInput.Replace("Í", "I'");
        strInput = strInput.Replace("ò", "O'");
        strInput = strInput.Replace("Ò", "O'");
        strInput = strInput.Replace("Ó", "O'");
        strInput = strInput.Replace("ù", "U'");
        strInput = strInput.Replace("°", " ");
      }
      return strInput;
    }

    public DettaglioArretrato GetDettaglioArretrato(int anno, int mese, int proDen, int annCom)
    {
      listaTotali.hdnAnno = anno;
      listaTotali.hdnMese = mese;
      listaTotali.hdnProden = proDen;
      
      LoadData();
      if (string.IsNullOrWhiteSpace(listaTotali.lblTotDovuto) || string.IsNullOrWhiteSpace(listaTotali.lblTotPagare))
        return null;
      string staDen;
      
      if (listaTotali.hdnTipMov == "NU")
        staDen = "Notifica di Ufficio";
      else if (listaTotali.strStaDen == "A")
        staDen = listaTotali.ModalitaPagamento <= 0
          ? "Acquisita senza estremi di pagamento"
          : "Acquisita con estremi di pagamento";
      else
        staDen = "Non Confermata";
      
      return new DettaglioArretrato()
      {
        AnnoCompetenza = annCom.ToString(),
        AnnoDenuncia = anno.ToString(),
        MeseDenuncia = Utile.GetMesi()[mese],
        ProgressivoDenuncia = proDen.ToString(),
        TotaleDovuto = listaTotali.lblTotDovuto,
        TotaleDaPagare = listaTotali.lblTotPagare,
        DataChiusura = listaTotali.DataChiusura,
        Protocollo = listaTotali.Protocollo,
        StatoDenuncia = staDen
      };
    }
    
    public bool UpdateDentesProtocollo(Protocollo protocollo, int anno, int mese, int proDen, string codPos)
    {
      var dataLayer = new DataLayer();
      var codPosParam = dataLayer.CreateParameter(CodicePosizione, iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codPos);
      var annoParam = dataLayer.CreateParameter(AnnoDenuncia, iDB2DbType.iDB2Decimal, 4, ParameterDirection.Input, anno.ToString());
      var meseParam = dataLayer.CreateParameter(MeseDenuncia, iDB2DbType.iDB2Decimal, 2, ParameterDirection.Input, mese.ToString());
      var proDenParam = dataLayer.CreateParameter(ProgressivoDenuncia, iDB2DbType.iDB2Decimal, 3, ParameterDirection.Input, proDen.ToString());

      var aggiornamentoDentes = objDataAccess.WriteDataWithParameters(
        $"UPDATE DENTES SET PROTRIC = '{protocollo.ProtocolloCompleto}', DATPROTRIC = '{protocollo.DataProtocollo.StandardizeDateString(StandardUse.Internal)}'" +
        $" WHERE CODPOS = {CodicePosizione} AND ANNDEN = {AnnoDenuncia} AND MESDEN ={MeseDenuncia}" +
        $" AND PRODEN = {ProgressivoDenuncia}", CommandType.Text, codPosParam, annoParam, meseParam, proDenParam);
      if (!aggiornamentoDentes) 
        _log.Error($"[TFI - TotaleArretratiDAL] - Aggiornamento Dentes protocollo ricevuta arretrato - Data: {DateTime.Now} -" +
                   $" Messaggio: {dataLayer.objException?.Message} - StackTrace: {dataLayer.objException?.StackTrace}");
      return aggiornamentoDentes;
    }
  }
}
