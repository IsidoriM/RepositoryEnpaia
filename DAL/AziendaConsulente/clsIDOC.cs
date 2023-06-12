// Decompiled with JetBrains decompiler
// Type: TFI.DAL.AziendaConsulente.clsIDOC
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using System;
using System.Data;
using System.Web;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;

namespace TFI.DAL.AziendaConsulente
{
  public class clsIDOC
  {
    private DataLayer DL = new DataLayer();
    public DataTable DT_IDOCTAB = new DataTable();
    public DataTable DT_IDOCCAMPI = new DataTable();
    public DataTable DT_IDOCVALORI = new DataTable();
    public string strUserCode;
    public int IDOC_CONT;
    public string SEGNUM = "";
    public DataTable objDtCONTIDOC;

    private void WRITE_EDID4(
      DataLayer objDataAccess,
      int IDTAB,
      string TABSAP,
      string VAR_E1PITYP,
      ref DataRow ROW_DATI)
    {
      string DOCNUM = "";
      string str1 = "";
      string MANDT = "";
      string PSGNUM = "";
      string HLEVEL = "";
      DataTable dataTable1 = new DataTable();
      string str2 = "";
      string str3 = "";
      string strSQL = "SELECT * FROM IDOCCAMPI WHERE IDTAB = " + IDTAB.ToString() + " ORDER BY ORDINE";
      dataTable1.Clear();
      DataTable dataTable2 = new DataTable();
      DataTable dataTable3 = this.DL.GetDataTable(strSQL);
      this.SEGNUM = this.Module_GetSEGNUM(objDataAccess, this.IDOC_CONT);
      int num = dataTable3.Rows.Count - 1;
      for (int index = 0; index <= num; ++index)
      {
        str2 = str2 + ", " + dataTable3.Rows[index]["CAMPO"]?.ToString();
        string upper1 = dataTable3.Rows[index]["CAMPO"].ToString().Trim().ToUpper();
        if (!(upper1 == "MANDT"))
        {
          if (upper1 == "DOCNUM")
            DOCNUM = this.GetFormato(dataTable3.Rows[index]["FORMATO"].ToString(), this.IDOC_CONT.ToString(), Convert.ToInt32(dataTable3.Rows[index]["LUNGHEZZA"]));
        }
        else
          MANDT = this.GetFormato(dataTable3.Rows[index]["FORMATO"].ToString(), dataTable3.Rows[index]["VALORE"].ToString(), Convert.ToInt32(dataTable3.Rows[index]["LUNGHEZZA"]));
        string upper2 = dataTable3.Rows[index]["VALORE"].ToString().Trim().ToUpper();
        if (!(upper2 == "[CONTATORE]"))
        {
          str3 = upper2 == "IDOCVALORI" ? str3 + ", " + this.GetStrValues(objDataAccess, Convert.ToInt32(dataTable3.Rows[index]["LUNGHEZZA"]), ref ROW_DATI, VAR_E1PITYP) : str3 + ", " + this.GetFormato(dataTable3.Rows[index]["FORMATO"].ToString(), dataTable3.Rows[index]["VALORE"].ToString(), Convert.ToInt32(dataTable3.Rows[index]["LUNGHEZZA"]));
        }
        else
        {
          string upper3 = dataTable3.Rows[index]["CAMPO"].ToString().Trim().ToUpper();
          if (!(upper3 == "SEGNUM"))
          {
            if (!(upper3 == "CONT"))
            {
              if (!(upper3 == "PSGNUM"))
              {
                if (!(upper3 == "HLEVEL"))
                {
                  if (upper3 == "DOCNUM")
                    str1 = this.IDOC_CONT.ToString();
                }
                else
                  str1 = HLEVEL;
              }
              else
              {
                this.Module_GetPSGNUM_HLEVEL(objDataAccess, IDTAB, MANDT, DOCNUM, ref HLEVEL, ref PSGNUM);
                str1 = PSGNUM;
              }
            }
            else
              str1 = this.IDOC_CONT.ToString();
          }
          else
            str1 = this.SEGNUM;
          str3 = str3 + ", " + this.GetFormato(dataTable3.Rows[index]["FORMATO"].ToString(), str1.ToString(), Convert.ToInt32(dataTable3.Rows[index]["LUNGHEZZA"]));
        }
      }
      string str4 = str2 + ", CONT_NUM ";
      string str5 = str3 + ", " + this.IDOC_CONT.ToString() + " ";
      string str6 = "INSERT INTO " + TABSAP + " (" + str4.Substring(2) + ")";
      string str7 = " VALUES (" + str5.Substring(2) + ")";
      objDataAccess.WriteTransactionData(str6 + str7, CommandType.Text);
    }

    public DataTable GET_IDOC_DATI_E1PITYPE(
      DataLayer dataLayer,
      string VAR_E1PITYP,
      int CODPOS,
      int MAT,
      int PRORAP,
      int PROSOS,
      string MASSN,
      string MASSG,
      string DATENDDA,
      string DATBEGDA,
      string PERCTRASF,
      string DATDEC,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      string FLGCANC,
      string FASE,
      string TIPMOV,
      string NUMRETTES,
      string PLVAR = "",
      int PROMOD = 0,
      string flgWEB = "N",
      int PRODENDET = 0)
    {
      DataTable idocDatiE1Pitype = new DataTable();
      DataTable dataTable = new DataTable();
      string strSQL1 = "";
      switch (VAR_E1PITYP.ToUpper().Trim() ?? "")
      {
        case "0000":
        case "0001":
        case "0002":
        case "0003":
        case "0302":
          string str1 = FASE ?? "";
          if (str1 == "VARIAZIONE INQUADRAMENTO" || str1 == "TRASFERIMENTO" || str1 == "ANNULLAMENTO TRASFERIMENTO")
          {
            string str2 = " SELECT ISCT.MAT, ";
            string str3 = string.IsNullOrEmpty(DATBEGDA.Trim()) ? str2 + " '' AS DATINI, " + " '' AS DATBEGDA, " : str2 + DBMethods.Db2Date(DATBEGDA) + " AS DATINI, " + DBMethods.Db2Date(DATBEGDA) + " AS DATBEGDA, ";
            idocDatiE1Pitype = this.DL.GetDataTable((string.IsNullOrEmpty(DATENDDA.Trim()) ? str3 + " '' AS DATFIN, " + " '' AS DATENDDA, " : str3 + DBMethods.Db2Date(DATENDDA) + " AS DATFIN, " + DBMethods.Db2Date(DATENDDA) + " AS DATENDDA, ") + " '" + MASSN + "' AS MASSN, " + " '" + MASSG + "' AS MASSG, " + " '" + PLVAR + "' AS PLVAR, " + " '" + FLGCANC + "' AS FLGCANC, '' AS SUBTY, " + " CURRENT_DATE AS ULTAGG " + " FROM ISCT WHERE ISCT.MAT=" + MAT.ToString());
            break;
          }
          idocDatiE1Pitype = this.DL.GetDataTable(" SELECT TRIM(VALUE(ISCT.COG,'')) || ' ' || TRIM(VALUE(ISCT.NOM,'')) " + " AS COGNOM1, '' AS COGNOM2, ISCT.MAT, ISCT.COG, ISCT.NOM, ISCT.CODFIS, ISCT.DATNAS, " + " ISCT.DATISC AS DATINI, '" + DBMethods.Db2Date(DATENDDA) + "' AS DATFIN, " + " ISCT.DATISC AS DATBEGDA, '" + DBMethods.Db2Date(DATENDDA) + "' AS DATENDDA, " + " ISCT.ULTAGG, CASE ISCT.SES WHEN 'F' THEN '2' WHEN 'M' THEN '1' " + " ELSE ' ' END AS SEX, '" + FLGCANC + "' AS FLGCANC, " + " (SELECT DENCOM FROM CODCOM WHERE CODCOM=ISCT.CODCOM) AS DENCOMNAS, " + " (SELECT SIGPRO FROM CODCOM WHERE CODCOM=ISCT.CODCOM) AS SIGPRONAS, " + " (SELECT CASE SIGPRO WHEN 'EE' THEN 'ZZ' ELSE 'IT' END FROM CODCOM " + "  WHERE CODCOM=ISCT.CODCOM) AS DENSTANAS, '' AS SUBTY, '' AS STATO_LUNGO," + " ISCT.CODCOM AS COMUNE, " + " '' AS CODREGNAS, " + " '" + MASSN + "' AS MASSN, " + " '" + MASSG + "' AS MASSG, " + " '" + PLVAR + "' AS PLVAR, " + " '' AS DATINISTO " + " FROM ISCT WHERE ISCT.MAT= " + MAT.ToString() + " ORDER BY ISCT.MAT ");
          int num1 = idocDatiE1Pitype.Rows.Count - 1;
          for (int index = 0; index <= num1; ++index)
          {
            if (idocDatiE1Pitype.Rows[index]["DENCOMNAS"].ToString().Trim() != "")
            {
              if (idocDatiE1Pitype.Rows[index]["DENCOMNAS"].ToString().Trim().Length > 24)
              {
                idocDatiE1Pitype.Rows[index]["STATO_LUNGO"] = (object) idocDatiE1Pitype.Rows[index]["DENCOMNAS"].ToString().Trim();
                idocDatiE1Pitype.Rows[index]["DENCOMNAS"] = (object) "";
              }
              else
              {
                idocDatiE1Pitype.Rows[index]["DENCOMNAS"] = (object) idocDatiE1Pitype.Rows[index]["DENCOMNAS"].ToString().Trim();
                idocDatiE1Pitype.Rows[index]["STATO_LUNGO"] = (object) "";
              }
            }
            if (idocDatiE1Pitype.Rows[index]["DENSTANAS"].ToString().Trim() == "ZZ")
            {
              idocDatiE1Pitype.Rows[index]["CODREGNAS"] = (object) "ESTE";
            }
            else
            {
              string strSQL2 = " SELECT CODREG FROM CODREG WHERE CODREG IN(SELECT CODREG " + " FROM SIGPRO WHERE SIGPRO IN (SELECT SIGPRO FROM CODCOM " + " WHERE CODCOM=" + DBMethods.DoublePeakForSql(idocDatiE1Pitype.Rows[index]["COMUNE"].ToString().Trim()) + "))";
              idocDatiE1Pitype.Rows[index]["CODREGNAS"] = (object) dataLayer.Get1ValueFromSQL(strSQL2, CommandType.Text);
              idocDatiE1Pitype.Rows[index]["CODREGNAS"] = (object) idocDatiE1Pitype.Rows[index]["CODREGNAS"].ToString().Trim().PadLeft(4, '0');
            }
            if (idocDatiE1Pitype.Rows[index]["COGNOM1"].ToString().Trim().Length > 30)
            {
              idocDatiE1Pitype.Rows[index]["COGNOM1"] = (object) idocDatiE1Pitype.Rows[index]["COGNOM1"].ToString().Trim().Substring(0, 30);
              idocDatiE1Pitype.Rows[index]["COGNOM2"] = (object) idocDatiE1Pitype.Rows[index]["COGNOM1"].ToString().Trim();
            }
            else
            {
              idocDatiE1Pitype.Rows[index]["COGNOM1"] = (object) idocDatiE1Pitype.Rows[index]["COGNOM1"].ToString().Trim();
              idocDatiE1Pitype.Rows[index]["COGNOM2"] = (object) idocDatiE1Pitype.Rows[index]["COGNOM1"].ToString().Trim();
            }
          }
          break;
        case "0006":
          string str4 = "SELECT ISCD.MAT, ISCD.IND, ISCD.NUMCIV, ISCD.DATINI, date(" + DBMethods.Db2Date(DATENDDA) + ") AS DATFIN ,";
          idocDatiE1Pitype = this.DL.GetDataTable((!(FASE == "CANCELLAZIONE") ? str4 + " ISCD.DATINI AS DATBEGDA, date( " + DBMethods.Db2Date(DATENDDA) + ") AS DATENDDA , " : str4 + " date(1900-01-01) AS DATBEGDA, date(9999-12-31) AS DATENDDA , ") + " (SELECT DENDUG FROM DUG WHERE CODDUG=ISCD.CODDUG) AS DENDUG, '" + FLGCANC + "' AS FLGCANC, " + " ISCD.DENLOC, ISCD.DENSTAEST, ISCD.CAP, ISCD.SIGPRO, 'TEL2' AS LBLTEL2, 'TEL3' AS LBLTEL3, ISCD.TEL1, '' AS IND1, '' AS IND2, " + " '' AS DENCOM, ISCD.CODCOM, " + " '" + PLVAR + "' AS PLVAR, '1' AS SUBTY, " + " ISCD.TEL2, 'FAX1' AS LBLFAX1, ISCD.FAX, ISCD.ULTAGG , ISCD.CO FROM ISCD WHERE MAT = " + MAT.ToString() + " " + " ORDER BY DATINI ");
          for (int index = idocDatiE1Pitype.Rows.Count - 1; index >= 0; --index)
          {
            if (idocDatiE1Pitype.Rows[index]["CODCOM"].ToString().Trim() != "")
            {
              string strSQL3 = "SELECT VALUE(DENCOM,'') AS DENCOM FROM CODCOM WHERE CODCOM = " + DBMethods.DoublePeakForSql(idocDatiE1Pitype.Rows[index]["CODCOM"].ToString().Trim());
              string str5 = Convert.ToString(dataLayer.Get1ValueFromSQL(strSQL3, CommandType.Text));
              idocDatiE1Pitype.Rows[index]["DENCOM"] = (object) str5.Trim();
            }
            if (index != idocDatiE1Pitype.Rows.Count - 1)
              Convert.ToDateTime(idocDatiE1Pitype.Rows[index + 1]["DATINI"]).AddDays(-1.0);
            else
              idocDatiE1Pitype.Rows[index]["DATFIN"] = (object) "9999-12-31";
            string str6;
            if (idocDatiE1Pitype.Rows[index]["NUMCIV"].ToString().Trim() == "")
              str6 = idocDatiE1Pitype.Rows[index]["DENDUG"].ToString().Trim() + " " + idocDatiE1Pitype.Rows[index]["IND"].ToString().Trim();
            else
              str6 = idocDatiE1Pitype.Rows[index]["DENDUG"].ToString().Trim() + " " + idocDatiE1Pitype.Rows[index]["IND"].ToString().Trim() + ", " + idocDatiE1Pitype.Rows[index]["NUMCIV"].ToString().Trim();
            idocDatiE1Pitype.Rows[index]["IND1"] = str6.Trim().Length <= 59 ? (object) str6 : (object) str6.Substring(0, 60);
            idocDatiE1Pitype.Rows[index]["DENSTAEST"] = !(idocDatiE1Pitype.Rows[index]["DENSTAEST"].ToString().Trim() != "") ? (object) "IT" : (object) "ZZ";
            if (idocDatiE1Pitype.Rows[index]["TEL1"].ToString().Trim() == "")
              idocDatiE1Pitype.Rows[index]["LBLTEL2"] = (object) "";
            if (idocDatiE1Pitype.Rows[index]["TEL2"].ToString().Trim() == "")
              idocDatiE1Pitype.Rows[index]["LBLTEL3"] = (object) "";
            if (idocDatiE1Pitype.Rows[index]["FAX"].ToString().Trim() == "")
              idocDatiE1Pitype.Rows[index]["LBLFAX1"] = (object) "";
          }
          break;
        case "0016":
        case "9001":
          int num2;
          if (FLGCANC == "D")
          {
            string str7 = " SELECT TRIM(CHAR(RAPLAV.CODPOS)) AS CODPOS, RAPLAV.MAT, RAPLAV.PRORAP, RAPLAV.DATDEC, RAPLAV.DATASS, RAPLAV.DATCES, " + " STORDL.TIPRAP, STORDL.CODCON, STORDL.CODGRUASS, VALUE(STORDL.CODLOC, 0) AS CODLOC, STORDL.CODGRUASS, 0 AS CODCONLOC, " + " '" + PLVAR + "' AS PLVAR, '' AS SUBTY, " + " (SELECT DISTINCT VALUE(CODQUACON, 0) AS CODQUACON FROM CONRIF WHERE CODCON=STORDL.CODCON) AS CODQUACON, " + " STORDL.DATINI AS DATINIAPP, STORDL.DATFIN AS DATFINAPP, " + " '1900-01-01' AS DATBEGDA, '9999-12-31' AS DATENDDA, " + " CASE VALUE(STORDL.PERPAR, '')  WHEN 0 THEN '' ELSE 'X' END AS FLGPARTIM, '' AS FLGOBBPRV, '' AS FLGOBBTFR, 'A' AS STATDIP, ISCT.DATNAS," + " '' AS PERCTRASF, '' AS TIPCON, " + " STORDL.NUMMEN, STORDL.CODLIV, STORDL.DATSCATER, '" + FLGCANC + "' AS FLGCANC, STORDL.DATFIN, STORDL.DATINI, STORDL.ULTAGG FROM RAPLAV, STORDL, ISCT" + " WHERE RAPLAV.CODPOS=STORDL.CODPOS AND RAPLAV.MAT=STORDL.MAT AND RAPLAV.PRORAP=STORDL.PRORAP AND RAPLAV.MAT=ISCT.MAT";
            string str8 = FASE ?? "";
            idocDatiE1Pitype = this.DL.GetDataTable((str8 == "VARIAZIONE INQUADRAMENTO" || str8 == "TRASFERIMENTO" || str8 == "ANNULLAMENTO TRASFERIMENTO" ? str7 + " AND RAPLAV.MAT = " + MAT.ToString() : str7 + " AND RAPLAV.CODPOS = " + CODPOS.ToString() + " AND RAPLAV.MAT = " + MAT.ToString() + " AND RAPLAV.PRORAP = " + PRORAP.ToString()) + " ORDER BY CODPOS, MAT, PRORAP, DATINI ");
            int num3 = idocDatiE1Pitype.Rows.Count - 1;
            for (int index = 0; index <= num3; ++index)
            {
              if ((FASE ?? "") == "TRASFERIMENTO")
              {
                string strSQL4 = " SELECT VALUE(PERTRA, 0.0) FROM TRARAP WHERE CODPOSTRA = " + idocDatiE1Pitype.Rows[index][nameof (CODPOS)]?.ToString() + " AND MATTRA = " + idocDatiE1Pitype.Rows[index][nameof (MAT)]?.ToString() + " AND PRORAPTRA= " + idocDatiE1Pitype.Rows[index][nameof (PRORAP)]?.ToString() + " ORDER BY DATTRA DESC FETCH FIRST ROWS ONLY ";
                Decimal num4 = Convert.ToDecimal(dataLayer.Get1ValueFromSQL(strSQL4, CommandType.Text));
                if ((double) num4 > 0.0)
                  idocDatiE1Pitype.Rows[index][nameof (PERCTRASF)] = (object) num4;
              }
              idocDatiE1Pitype.Rows[index][nameof (CODPOS)] = (object) ("A" + idocDatiE1Pitype.Rows[index][nameof (CODPOS)].ToString().Trim().PadLeft(6, '0'));
              idocDatiE1Pitype.Rows[index]["STATDIP"] = !(idocDatiE1Pitype.Rows[index]["DATCES"].ToString().Trim() != "" & idocDatiE1Pitype.Rows[index]["DATCES"].ToString().Trim() != "9999-12-31") ? (object) "A" : (!(idocDatiE1Pitype.Rows[index]["DATCES"].ToString().Trim() == idocDatiE1Pitype.Rows[index]["DATFIN"].ToString().Trim()) ? (object) "A" : (object) "C");
              if (idocDatiE1Pitype.Rows[index]["CODLOC"].ToString() != "0")
              {
                string strSQL5 = " SELECT VALUE(PROCON,0) FROM CONLOC" + " WHERE CODLOC = " + idocDatiE1Pitype.Rows[index]["CODLOC"]?.ToString() + " AND DATDEC <= '" + DBMethods.Db2Date(idocDatiE1Pitype.Rows[index]["DATINI"].ToString()) + "' ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
                int int32_1 = Convert.ToInt32(dataLayer.Get1ValueFromSQL(strSQL5, CommandType.Text));
                string strSQL6 = " SELECT VALUE(PROLOC,0) FROM CONLOC" + " WHERE CODLOC = " + idocDatiE1Pitype.Rows[index]["CODLOC"]?.ToString() + " AND DATDEC <= '" + DBMethods.Db2Date(idocDatiE1Pitype.Rows[index]["DATINI"].ToString()) + "' ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
                int int32_2 = Convert.ToInt32(dataLayer.Get1ValueFromSQL(strSQL6, CommandType.Text));
                string strSQL7 = " SELECT VALUE(CODTIPCON,0) FROM CONLOC WHERE" + " CODCON = " + idocDatiE1Pitype.Rows[index]["CODCON"]?.ToString() + " AND PROCON = " + int32_1.ToString() + " AND CODLOC = " + idocDatiE1Pitype.Rows[index]["CODLOC"]?.ToString() + " AND PROLOC = " + int32_2.ToString();
                idocDatiE1Pitype.Rows[index]["TIPCON"] = (object) dataLayer.Get1ValueFromSQL(strSQL7, CommandType.Text);
              }
              else
              {
                string strSQL8 = "SELECT VALUE(PROCON,0) FROM CONRIF " + " WHERE CODCON = " + idocDatiE1Pitype.Rows[index]["CODCON"]?.ToString() + " AND DATDEC <= '" + DBMethods.Db2Date(idocDatiE1Pitype.Rows[index]["DATINI"].ToString()) + "'" + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
                int int32 = Convert.ToInt32(dataLayer.Get1ValueFromSQL(strSQL8, CommandType.Text));
                string strSQL9 = " SELECT VALUE(CODTIPCON,0) FROM CONRIF WHERE" + " CODCON = " + idocDatiE1Pitype.Rows[index]["CODCON"]?.ToString() + " AND PROCON = " + int32.ToString();
                idocDatiE1Pitype.Rows[index]["TIPCON"] = (object) dataLayer.Get1ValueFromSQL(strSQL9, CommandType.Text);
              }
              if (idocDatiE1Pitype.Rows[index]["CODQUACON"].ToString().Trim() != "0" & idocDatiE1Pitype.Rows[index]["CODQUACON"].ToString().Trim() != "")
              {
                string strSQL10 = " SELECT VALUE(COUNT(*),0) FROM ALIFORASS WHERE" + " CODGRUASS = " + idocDatiE1Pitype.Rows[index]["CODGRUASS"]?.ToString() + " AND CODQUACON = " + idocDatiE1Pitype.Rows[index]["CODQUACON"]?.ToString() + " AND CODFORASS IN(SELECT CODFORASS FROM FORASS" + " WHERE CATFORASS IN('PREV'))";
                num2 = 0;
                if (Convert.ToInt32(dataLayer.Get1ValueFromSQL(strSQL10, CommandType.Text)) > 0)
                  idocDatiE1Pitype.Rows[index]["FLGOBBPRV"] = !(idocDatiE1Pitype.Rows[index]["DATFIN"].ToString() == "9999-12-31") ? (object) "X" : (object) "X";
                string strSQL11 = " SELECT VALUE(COUNT(*),0) FROM ALIFORASS WHERE" + " CODGRUASS = " + idocDatiE1Pitype.Rows[index]["CODGRUASS"]?.ToString() + " AND CODQUACON = " + idocDatiE1Pitype.Rows[index]["CODQUACON"]?.ToString() + " AND CODFORASS IN(SELECT CODFORASS FROM FORASS" + " WHERE CATFORASS IN('TFR'))";
                num2 = 0;
                if (Convert.ToInt32(dataLayer.Get1ValueFromSQL(strSQL11, CommandType.Text)) > 0)
                  idocDatiE1Pitype.Rows[index]["FLGOBBTFR"] = (object) "X";
              }
            }
            break;
          }
          string str9 = " SELECT TRIM(CHAR(RAPLAV.CODPOS)) AS CODPOS, RAPLAV.MAT, RAPLAV.PRORAP, RAPLAV.DATDEC, RAPLAV.DATASS, RAPLAV.DATCES, " + " STORDL.TIPRAP, STORDL.CODCON, VALUE(STORDL.CODLOC, 0) AS CODLOC, STORDL.CODGRUASS, 0 AS CODCONLOC, '" + FLGCANC + "' AS FLGCANC, " + " (SELECT DISTINCT VALUE(CODQUACON, 0) AS CODQUACON FROM CONRIF WHERE CODCON=STORDL.CODCON) AS CODQUACON, " + " STORDL.DATINI AS DATINIAPP, STORDL.DATFIN AS DATFINAPP, " + " '1900-01-01' AS DATBEGDA, '9999-12-31' AS DATENDDA, " + " CASE VALUE(STORDL.PERPAR, '')  WHEN 0 THEN '' ELSE 'X' END AS FLGPARTIM, '' AS FLGOBBPRV, '' AS FLGOBBTFR, 'A' AS STATDIP, ISCT.DATNAS," + " '" + PLVAR + "' AS PLVAR, '' AS TIPCON, '' AS SUBTY, " + " '' AS PERCTRASF, " + " STORDL.NUMMEN, STORDL.CODLIV, STORDL.DATSCATER, STORDL.DATFIN, STORDL.DATINI, STORDL.ULTAGG FROM RAPLAV, STORDL, ISCT " + " WHERE RAPLAV.CODPOS=STORDL.CODPOS AND RAPLAV.MAT=STORDL.MAT AND RAPLAV.PRORAP=STORDL.PRORAP AND RAPLAV.MAT=ISCT.MAT";
          string str10 = FASE ?? "";
          idocDatiE1Pitype = this.DL.GetDataTable((str10 == "VARIAZIONE INQUADRAMENTO" || str10 == "TRASFERIMENTO" || str10 == "ANNULLAMENTO TRASFERIMENTO" ? str9 + " AND RAPLAV.MAT = " + MAT.ToString() : str9 + " AND RAPLAV.CODPOS = " + CODPOS.ToString() + " AND RAPLAV.MAT = " + MAT.ToString() + " AND RAPLAV.PRORAP = " + PRORAP.ToString()) + " ORDER BY CODPOS, MAT, PRORAP, DATINI ");
          for (int index = idocDatiE1Pitype.Rows.Count - 1; index >= 0; --index)
          {
            if ((FASE ?? "") == "TRASFERIMENTO")
            {
              string strSQL12 = " SELECT VALUE(PERTRA, 0.0) FROM TRARAP WHERE CODPOSTRA = " + idocDatiE1Pitype.Rows[index][nameof (CODPOS)]?.ToString() + " AND MATTRA = " + idocDatiE1Pitype.Rows[index][nameof (MAT)]?.ToString() + " AND PRORAPTRA= " + idocDatiE1Pitype.Rows[index][nameof (PRORAP)]?.ToString() + " ORDER BY DATTRA DESC FETCH FIRST ROWS ONLY ";
              Decimal num5 = Convert.ToDecimal(dataLayer.Get1ValueFromSQL(strSQL12, CommandType.Text));
              if ((double) num5 > 0.0)
                idocDatiE1Pitype.Rows[index][nameof (PERCTRASF)] = (object) num5;
            }
            idocDatiE1Pitype.Rows[index][nameof (CODPOS)] = (object) ("A" + idocDatiE1Pitype.Rows[index][nameof (CODPOS)].ToString().Trim().PadLeft(6, '0'));
            idocDatiE1Pitype.Rows[index]["STATDIP"] = !(idocDatiE1Pitype.Rows[index]["DATCES"].ToString().Trim() != "" & idocDatiE1Pitype.Rows[index]["DATCES"].ToString().Trim() != "9999-12-31") ? (object) "A" : (!(idocDatiE1Pitype.Rows[index]["DATCES"].ToString().Trim() == idocDatiE1Pitype.Rows[index]["DATFIN"].ToString().Trim()) ? (object) "A" : (object) "C");
            idocDatiE1Pitype.Rows[index]["CODCONLOC"] = !(idocDatiE1Pitype.Rows[index]["CODLOC"].ToString().Trim() != "0") ? idocDatiE1Pitype.Rows[index]["CODCON"] : idocDatiE1Pitype.Rows[index]["CODLOC"];
            if (idocDatiE1Pitype.Rows[index]["CODQUACON"].ToString().Trim() == "")
              idocDatiE1Pitype.Rows[index]["CODQUACON"] = (object) "0";
            if (idocDatiE1Pitype.Rows[index]["CODLOC"].ToString() != "0")
            {
              string strSQL13 = " SELECT VALUE(PROCON,0) FROM CONLOC" + " WHERE CODLOC = " + idocDatiE1Pitype.Rows[index]["CODLOC"]?.ToString() + " AND DATDEC <= '" + DBMethods.Db2Date(idocDatiE1Pitype.Rows[index]["DATINI"].ToString()) + "'" + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
              string int32_3 = dataLayer.Get1ValueFromSQL(strSQL13, CommandType.Text);
              string strSQL14 = " SELECT VALUE(PROLOC,0) FROM CONLOC" + " WHERE CODLOC = " + idocDatiE1Pitype.Rows[index]["CODLOC"]?.ToString() + " AND DATDEC <=  '" + DBMethods.Db2Date(idocDatiE1Pitype.Rows[index]["DATINI"].ToString()) + "'" + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
              string int32_4 = dataLayer.Get1ValueFromSQL(strSQL14, CommandType.Text);
              string strSQL15 = " SELECT VALUE(CODTIPCON,0) FROM CONLOC WHERE" + " CODCON = " + idocDatiE1Pitype.Rows[index]["CODCON"]?.ToString() + " AND PROCON = " + int32_3.ToString() + " AND CODLOC = " + idocDatiE1Pitype.Rows[index]["CODLOC"]?.ToString() + " AND PROLOC = " + int32_4.ToString();
              idocDatiE1Pitype.Rows[index]["TIPCON"] = (object) dataLayer.Get1ValueFromSQL(strSQL15, CommandType.Text);
            }
            else
            {
              string strSQL16 = "SELECT VALUE(PROCON,0) FROM CONRIF " + " WHERE CODCON = " + idocDatiE1Pitype.Rows[index]["CODCON"]?.ToString() + " AND DATDEC <= '" + DBMethods.Db2Date(idocDatiE1Pitype.Rows[index]["DATINI"].ToString()) + "'" + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
              int int32 = Convert.ToInt32(dataLayer.Get1ValueFromSQL(strSQL16, CommandType.Text));
              string strSQL17 = " SELECT VALUE(CODTIPCON,0) FROM CONRIF WHERE" + " CODCON = " + idocDatiE1Pitype.Rows[index]["CODCON"]?.ToString() + " AND PROCON = " + int32.ToString();
              idocDatiE1Pitype.Rows[index]["TIPCON"] = (object) dataLayer.Get1ValueFromSQL(strSQL17, CommandType.Text);
            }
            if (idocDatiE1Pitype.Rows[index]["CODQUACON"].ToString().Trim() != "0" & idocDatiE1Pitype.Rows[index]["CODQUACON"].ToString().Trim() != "")
            {
              string strSQL18 = " SELECT VALUE(COUNT(*),0) FROM ALIFORASS WHERE" + " CODGRUASS = " + idocDatiE1Pitype.Rows[index]["CODGRUASS"]?.ToString() + " AND CODQUACON = " + idocDatiE1Pitype.Rows[index]["CODQUACON"]?.ToString() + " AND CODFORASS IN(SELECT CODFORASS FROM FORASS" + " WHERE CATFORASS IN('PREV'))";
              num2 = 0;
              if (Convert.ToInt32(dataLayer.Get1ValueFromSQL(strSQL18, CommandType.Text)) > 0)
                idocDatiE1Pitype.Rows[index]["FLGOBBPRV"] = !(idocDatiE1Pitype.Rows[index]["DATFIN"].ToString() == "9999-12-31") ? (object) "X" : (object) "X";
              string strSQL19 = " SELECT VALUE(COUNT(*),0) FROM ALIFORASS WHERE" + " CODGRUASS = " + idocDatiE1Pitype.Rows[index]["CODGRUASS"]?.ToString() + " AND CODQUACON = " + idocDatiE1Pitype.Rows[index]["CODQUACON"]?.ToString() + " AND CODFORASS IN(SELECT CODFORASS FROM FORASS" + " WHERE CATFORASS IN('TFR'))";
              num2 = 0;
              if (Convert.ToInt32(dataLayer.Get1ValueFromSQL(strSQL19, CommandType.Text)) > 0)
                idocDatiE1Pitype.Rows[index]["FLGOBBTFR"] = (object) "X";
            }
          }
          break;
        case "9003":
          idocDatiE1Pitype = this.DL.GetDataTable("SELECT MAT, TRIM(CHAR(CODPOS)) AS CODPOS, PRORAP, DATINISOS AS DATINI, " + " DATFINSOS AS DATFIN, TRIM(CHAR(PERAZI)) AS PERAZI, " + " '1900-01-01' AS DATBEGDA, '9999-12-31' AS DATENDDA, '' AS SUBTY, " + " (SELECT RAGSOC FROM AZI WHERE CODPOS=SOSRAP.CODPOS) AS RAGSOC, " + " '" + PLVAR + "' AS PLVAR, " + " '" + FLGCANC + "' AS FLGCANC, " + " PROSOS, CODSOS, ULTAGG, UTEAGG " + " FROM SOSRAP WHERE CODPOS = " + CODPOS.ToString() + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString() + " AND PROSOS = " + PROSOS.ToString());
          idocDatiE1Pitype.Rows[0][nameof (CODPOS)] = (object) ("A" + idocDatiE1Pitype.Rows[0][nameof (CODPOS)].ToString().Trim().PadLeft(6, '0'));
          if (idocDatiE1Pitype.Rows[0]["PERAZI"].ToString().Trim() != "")
          {
            if (idocDatiE1Pitype.Rows[0]["RAGSOC"].ToString().Trim().Length > 29)
              idocDatiE1Pitype.Rows[0]["RAGSOC"] = (object) idocDatiE1Pitype.Rows[0]["RAGSOC"].ToString().Trim().Substring(0, 29);
            if (idocDatiE1Pitype.Rows[0]["PERAZI"].ToString().Trim().Substring(0, 1) == ".")
            {
              idocDatiE1Pitype.Rows[0]["PERAZI"] = (object) ("0" + idocDatiE1Pitype.Rows[0]["PERAZI"].ToString().Trim());
              break;
            }
            break;
          }
          break;
        case "9004":
          if (FASE == "PREV")
          {
            strSQL1 = " SELECT TRIM(CHAR(CODPOS)) AS CODPOS, " + " ANNDEN, MESDEN, VALUE(PRODEN,0) AS PRODEN, VALUE(PRODENDET,0) AS PRODENDET, MAT, PRORAP, " + " DAL, AL, " + " DAL AS DATINI, AL AS DATFIN, '' AS SUBTY, " + " '1900-01-01' AS DATBEGDA, '9999-12-31' AS DATENDDA, " + " '" + PLVAR + "' AS PLVAR, 'C' AS STATUSPREV, " + " (SELECT RAGSOC FROM AZI WHERE CODPOS=MODPREDET.CODPOS) AS RAGSOC, " + " IMPCON, IMPFIG, IMPOCC, IMPRET,  " + " 0.0 AS IMPUTILE," + " IMPCONPRV, IMPFIGPRV, IMPOCCPRV, IMPRETPRV, " + " '" + FLGCANC + "' AS FLGCANC, " + " PERFAP, 0.00 AS PERPAR, ULTAGG " + " FROM MODPREDET " + " WHERE CODPOS = " + CODPOS.ToString() + " AND PRORAP = " + PRORAP.ToString() + " AND MAT = " + MAT.ToString() + " AND PROMOD = " + PROMOD.ToString() + " AND TIPMOV <> 'AR' " + " ORDER BY DAL";
          }
          else
          {
            string str11 = TIPMOV.Trim() ?? "";
            if (!(str11 == "DP") && !(str11 == "NU"))
            {
              if (str11 == "RT")
              {
                string str12 = " SELECT TRIM(CHAR(DENDET.CODPOS)) AS CODPOS, DENDET.ANNDEN, DENDET.MESDEN, DENDET.PRODEN, PRODENDET, MAT, PRORAP, " + " DAL, AL, '' AS STATUSPREV, '' AS SUBTY, " + " DAL AS DATINI, AL AS DATFIN, " + " '1900-01-01' AS DATBEGDA, '9999-12-31' AS DATENDDA, " + " '" + PLVAR + "' AS PLVAR, " + " (SELECT RAGSOC FROM AZI WHERE CODPOS=DENDET.CODPOS) AS RAGSOC, ";
                strSQL1 = (!(FASE == "ANNULLAMENTO") ? str12 + " DENDET.IMPCON, DENDET.IMPFIG, DENDET.IMPOCC, DENDET.IMPRET, " + " 0.0 AS IMPUTILE," + " '' AS FLGCANC, " : str12 + " 'D' AS FLGCANC, " + " 0 AS IMPCON, 0 AS IMPFIG, 0 AS IMPOCC, 0 AS IMPRET, " + " 0 AS IMPUTILE,") + " PERFAP, 0.00 AS PERPAR, DENDET.ULTAGG " + " FROM DENDET, DENTES " + " WHERE DENTES.CODPOS = DENDET.CODPOS " + " AND DENTES.ANNDEN = DENDET.ANNDEN " + " AND DENTES.MESDEN = DENDET.MESDEN " + " AND DENTES.PRODEN = DENDET.PRODEN " + " AND DENTES.TIPMOV <> 'AR' " + " AND DENDET.CODPOS=" + CODPOS.ToString() + " AND DENDET.ANNDEN = " + ANNDEN.ToString() + " AND DENDET.MESDEN = " + MESDEN.ToString() + " AND DENDET.PRODEN = " + PRODEN.ToString() + " AND DENDET.MAT = " + MAT.ToString() + " AND DENDET.PRODENDET = " + PRODENDET.ToString() + " AND DENDET.PRORAP = " + PRORAP.ToString() + " ORDER BY MAT, DAL";
              }
            }
            else
            {
              string str13 = " SELECT TRIM(CHAR(CODPOS)) AS CODPOS, ANNDEN, MESDEN, PRODEN, PRODENDET, MAT, PRORAP, " + " DAL, AL, '' AS SUBTY, " + " DAL AS DATINI, AL AS DATFIN, " + " '1900-01-01' AS DATBEGDA, '9999-12-31' AS DATENDDA, " + " '" + PLVAR + "' AS PLVAR, '' AS STATUSPREV, " + " (SELECT RAGSOC FROM AZI WHERE CODPOS=DENDET.CODPOS) AS RAGSOC, ";
              string str14 = (!(FASE == "ANNULLAMENTO") ? str13 + " IMPCON, IMPFIG, IMPOCC, IMPRET, " + " 0.0 AS IMPUTILE," : str13 + " 0 AS IMPCON, 0 AS IMPFIG, 0 AS IMPOCC, 0 AS IMPRET, " + " 0 AS IMPUTILE, ") + " '" + FLGCANC + "' AS FLGCANC, " + " PERFAP, 0.00 AS PERPAR, ULTAGG " + " FROM DENDET " + " WHERE CODPOS=" + CODPOS.ToString() + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString();
              if (MAT != 0)
                str14 = str14 + " AND MAT = " + MAT.ToString();
              if (PRODENDET != 0)
                str14 = str14 + " AND PRODENDET = " + PRODENDET.ToString();
              if (PRORAP != 0)
                str14 = str14 + " AND PRORAP= " + PRORAP.ToString();
              strSQL1 = str14 + " ORDER BY MAT, DAL";
            }
          }
          idocDatiE1Pitype = this.DL.GetDataTable(strSQL1);
          int num6 = idocDatiE1Pitype.Rows.Count - 1;
          for (int index = 0; index <= num6; ++index)
          {
            if (idocDatiE1Pitype.Rows[index]["RAGSOC"].ToString().Trim().Length > 29)
              idocDatiE1Pitype.Rows[index]["RAGSOC"] = (object) idocDatiE1Pitype.Rows[index]["RAGSOC"].ToString().Trim().Substring(0, 29);
            string strSQL20 = "SELECT VALUE(PERPAR, 0.00) AS PERPAR FROM STORDL " + " WHERE CODPOS = " + idocDatiE1Pitype.Rows[index][nameof (CODPOS)]?.ToString() + " AND MAT = " + idocDatiE1Pitype.Rows[index][nameof (MAT)]?.ToString() + " AND PRORAP = " + idocDatiE1Pitype.Rows[index][nameof (PRORAP)]?.ToString() + " AND DATINI <= '" + DBMethods.Db2Date(idocDatiE1Pitype.Rows[index]["DAL"].ToString()) + "'" + " AND DATFIN >= '" + DBMethods.Db2Date(idocDatiE1Pitype.Rows[index]["AL"].ToString()) + "'" + " ORDER BY DATINI DESC FETCH FIRST 1 ROWS ONLY";
            Decimal num7 = Convert.ToDecimal(dataLayer.Get1ValueFromSQL(strSQL20, CommandType.Text));
            if ((double) num7 != 0.0)
              idocDatiE1Pitype.Rows[index]["PERPAR"] = (object) num7;
            if (TIPMOV.ToString() == "RT")
            {
              string strSQL21 = " SELECT DISTINCT VALUE(PROMOD,0) FROM MODPREDET WHERE" + " CODPOS = " + idocDatiE1Pitype.Rows[index][nameof (CODPOS)]?.ToString() + " AND MAT = " + idocDatiE1Pitype.Rows[index][nameof (MAT)]?.ToString() + " AND PRORAP = " + idocDatiE1Pitype.Rows[index][nameof (PRORAP)]?.ToString() + " AND ANNDEN = " + idocDatiE1Pitype.Rows[index][nameof (ANNDEN)]?.ToString() + " AND PRODEN = " + idocDatiE1Pitype.Rows[index][nameof (PRODEN)]?.ToString() + " AND PROMOD = ( SELECT MAX(PROMOD) FROM MODPREDET" + " WHERE CODPOS = " + idocDatiE1Pitype.Rows[index][nameof (CODPOS)]?.ToString() + " AND MAT = " + idocDatiE1Pitype.Rows[index][nameof (MAT)]?.ToString() + " AND PRORAP = " + idocDatiE1Pitype.Rows[index][nameof (PRORAP)]?.ToString() + " AND ANNDEN = " + idocDatiE1Pitype.Rows[index][nameof (ANNDEN)]?.ToString() + " AND PRODEN = " + idocDatiE1Pitype.Rows[index][nameof (PRODEN)]?.ToString() + " )";
              int int32 = Convert.ToInt32(dataLayer.Get1ValueFromSQL(strSQL21, CommandType.Text));
              if (int32 > 0)
              {
                string strSQL22 = " SELECT VALUE(COUNT(*),0) AS CONT FROM MODPRE WHERE" + " CODPOS = " + idocDatiE1Pitype.Rows[index][nameof (CODPOS)]?.ToString() + " AND MAT = " + idocDatiE1Pitype.Rows[index][nameof (MAT)]?.ToString() + " AND PRORAP = " + idocDatiE1Pitype.Rows[index][nameof (PRORAP)]?.ToString() + " AND PROMOD = " + int32.ToString() + " AND CODSTAPRE = 4 AND DATANN IS NULL";
                dataTable.Clear();
                dataTable = this.DL.GetDataTable(strSQL22);
                if (Convert.ToInt32(dataTable.Rows[0]["CONT"]) > 0)
                  idocDatiE1Pitype.Rows[index]["STATUSPREV"] = (object) "C";
              }
            }
            idocDatiE1Pitype.Rows[index][nameof (CODPOS)] = (object) ("A" + idocDatiE1Pitype.Rows[index][nameof (CODPOS)].ToString().Trim().PadLeft(6, '0'));
            if (FASE == "PREV")
            {
              if (idocDatiE1Pitype.Rows[index]["IMPRETPRV"].ToString().Trim() != "")
                idocDatiE1Pitype.Rows[index]["IMPRET"] = idocDatiE1Pitype.Rows[index]["IMPRETPRV"];
              if (idocDatiE1Pitype.Rows[index]["IMPFIGPRV"].ToString().Trim() != "")
                idocDatiE1Pitype.Rows[index]["IMPFIG"] = idocDatiE1Pitype.Rows[index]["IMPFIGPRV"];
              if (idocDatiE1Pitype.Rows[index]["IMPOCCPRV"].ToString().Trim() != "")
                idocDatiE1Pitype.Rows[index]["IMPOCC"] = idocDatiE1Pitype.Rows[index]["IMPOCCPRV"];
              if (idocDatiE1Pitype.Rows[index]["IMPCONPRV"].ToString().Trim() != "")
                idocDatiE1Pitype.Rows[index]["IMPCON"] = idocDatiE1Pitype.Rows[index]["IMPCONPRV"];
            }
            idocDatiE1Pitype.Rows[index]["IMPUTILE"] = (object) (Convert.ToInt32(idocDatiE1Pitype.Rows[index]["IMPRET"]) + Convert.ToInt32(idocDatiE1Pitype.Rows[index]["IMPFIG"]) - Convert.ToInt32(idocDatiE1Pitype.Rows[index]["IMPOCC"]));
          }
          break;
        case "9005":
          string strSQL23;
          if (FASE == "PREV")
            strSQL23 = " SELECT ANNCOM, CODPOS, MAT, PRORAP, DAL, AL, DATBEGDA, DATENDDA, STATUSPREV, " + " PLVAR, RAGSOC, SUBTY, FLGCANC, " + " MAX(ULTAGG) AS ULTAGG, SUM(IMPRET) AS IMPRET, SUM(IMPOCC) AS IMPOCC, 0.0 AS IMPUTILE " + " FROM " + " ( " + " SELECT MODPREDET.ANNCOM, TRIM(CHAR(MODPREDET.CODPOS)) AS CODPOS, " + " MODPREDET.MAT, MODPREDET.PRORAP, " + " MODPREDET.ANNCOM || '-01-01' AS DAL, " + " MODPREDET.ANNCOM || '-12-31' AS AL, " + " '1900-01-01' AS DATBEGDA, '9999-12-31' AS DATENDDA, 'C' AS STATUSPREV, " + " '" + PLVAR + "' AS PLVAR, " + " (SELECT RAGSOC FROM AZI WHERE CODPOS=MODPREDET.CODPOS) AS RAGSOC, " + "  '' AS SUBTY, '" + FLGCANC + "' AS FLGCANC, MODPREDET.ULTAGG, " + " CASE VALUE(CHAR(MODPREDET.IMPRETPRV), '') WHEN '' THEN MODPREDET.IMPRET ELSE MODPREDET.IMPRETPRV END AS IMPRET," + " CASE VALUE(CHAR(MODPREDET.IMPOCCPRV), '') WHEN '' THEN MODPREDET.IMPOCC ELSE MODPREDET.IMPOCCPRV  END AS IMPOCC" + " FROM MODPREDET " + " WHERE MODPREDET.TIPMOV = 'AR' " + " AND MODPREDET.CODPOS=" + CODPOS.ToString() + " AND MODPREDET.MAT = " + MAT.ToString() + " AND MODPREDET.PRORAP = " + PRORAP.ToString() + " AND MODPREDET.PROMOD = " + PROMOD.ToString() + " ) AS TABELLA " + " GROUP BY " + " ANNCOM, CODPOS, MAT, PRORAP, DAL, AL, DATBEGDA, DATENDDA, STATUSPREV, " + " PLVAR, RAGSOC, SUBTY, FLGCANC " + " ORDER BY MAT, DAL";
          else if (FASE == "ANTE2003")
            strSQL23 = " SELECT ANNDEN AS ANNCOM, CODPOS, MAT, PRORAP, DAL, AL, DATBEGDA, DATENDDA, STATUSPREV, " + " PLVAR, RAGSOC, SUBTY, FLGCANC, " + " MAX(ULTAGG) AS ULTAGG, SUM(IMPRET) AS IMPRET, SUM(IMPOCC) AS IMPOCC, 0.0 AS IMPUTILE " + " FROM " + " ( " + " SELECT DENDET.ANNDEN, TRIM(CHAR(DENDET.CODPOS)) AS CODPOS, " + " DENDET.MAT, DENDET.PRORAP, " + " DENDET.ANNDEN || '-01-01' AS DAL, " + " DENDET.ANNDEN || '-12-31' AS AL, " + " '1900-01-01' AS DATBEGDA, '9999-12-31' AS DATENDDA, '' AS STATUSPREV, " + " '" + PLVAR + "' AS PLVAR, " + " (SELECT RAGSOC FROM AZI WHERE CODPOS=DENDET.CODPOS) AS RAGSOC, " + "  '' AS SUBTY, '" + FLGCANC + "' AS FLGCANC, DENDET.ULTAGG, " + " CASE VALUE(DENDET.NUMMOVANN, '') WHEN '' THEN DENDET.IMPRET ELSE 0 END AS IMPRET, " + " CASE VALUE(DENDET.NUMMOVANN, '') WHEN '' THEN DENDET.IMPOCC ELSE 0 END AS IMPOCC " + " FROM DENDET, DENTES " + " WHERE " + " DENTES.CODPOS = DENDET.CODPOS " + " AND DENTES.ANNDEN = DENDET.ANNDEN " + " AND DENTES.MESDEN = DENDET.MESDEN " + " AND DENTES.PRODEN = DENDET.PRODEN " + " AND VALUE(DENDET.ESIRET, '') <> 'S' " + " AND DENDET.CODPOS=" + CODPOS.ToString() + " AND DENDET.ANNDEN= " + ANNDEN.ToString() + " AND DENDET.MESDEN= " + MESDEN.ToString() + " AND DENDET.PRODEN = " + PRODEN.ToString() + " AND DENDET.MAT= " + MAT.ToString() + " AND DENDET.PRODENDET = " + PRODENDET.ToString() + " AND DENDET.PRORAP = " + PRORAP.ToString() + " ) AS TABELLA " + " GROUP BY " + " ANNDEN, CODPOS, MAT, PRORAP, DAL, AL, DATBEGDA, DATENDDA, STATUSPREV, " + " PLVAR, RAGSOC, SUBTY, FLGCANC " + " ORDER BY MAT, DAL";
          else if (FASE == "ANTE2003_ANN")
          {
            strSQL23 = " SELECT ANNDEN AS ANNCOM, CODPOS, MAT, PRORAP, DAL, AL, DATBEGDA, DATENDDA, STATUSPREV, " + " PLVAR, RAGSOC, SUBTY, FLGCANC, " + " MAX(ULTAGG) AS ULTAGG, SUM(IMPRET) AS IMPRET, SUM(IMPOCC) AS IMPOCC, 0.0 AS IMPUTILE " + " FROM " + " ( " + " SELECT DENDET.ANNDEN, TRIM(CHAR(DENDET.CODPOS)) AS CODPOS, " + " DENDET.MAT, DENDET.PRORAP, " + " DENDET.ANNDEN || '-01-01' AS DAL, " + " DENDET.ANNDEN || '-12-31' AS AL, " + " '1900-01-01' AS DATBEGDA, '9999-12-31' AS DATENDDA, '' AS STATUSPREV, " + " '" + PLVAR + "' AS PLVAR, " + " (SELECT RAGSOC FROM AZI WHERE CODPOS=DENDET.CODPOS) AS RAGSOC, " + "  '' AS SUBTY, '" + FLGCANC + "' AS FLGCANC, DENDET.ULTAGG, " + " 0 AS IMPRET, " + " 0 AS IMPOCC " + " FROM DENDET, DENTES " + " WHERE " + " DENTES.CODPOS = DENDET.CODPOS " + " AND DENTES.ANNDEN = DENDET.ANNDEN " + " AND DENTES.MESDEN = DENDET.MESDEN " + " AND DENTES.PRODEN = DENDET.PRODEN " + " AND VALUE(DENDET.ESIRET, '') <> 'S' " + " AND DENDET.CODPOS=" + CODPOS.ToString() + " AND DENDET.ANNDEN= " + ANNDEN.ToString() + " AND DENDET.MESDEN= " + MESDEN.ToString() + " AND DENDET.PRODEN = " + PRODEN.ToString() + " AND DENDET.MAT= " + MAT.ToString() + " AND DENDET.PRODENDET = " + PRODENDET.ToString() + " AND DENDET.PRORAP = " + PRORAP.ToString() + " ) AS TABELLA " + " GROUP BY " + " ANNDEN, CODPOS, MAT, PRORAP, DAL, AL, DATBEGDA, DATENDDA, STATUSPREV, " + " PLVAR, RAGSOC, SUBTY, FLGCANC " + " ORDER BY MAT, DAL";
          }
          else
          {
            string str15 = " SELECT ANNCOM, CODPOS, MAT, PRORAP, DAL, AL, DATBEGDA, DATENDDA, STATUSPREV, " + " PLVAR, RAGSOC, SUBTY, FLGCANC, " + " MAX(ULTAGG) AS ULTAGG, SUM(IMPRET) AS IMPRET, SUM(IMPOCC) AS IMPOCC, 0.0 AS IMPUTILE " + " FROM " + " ( " + " SELECT DENDET.ANNCOM, TRIM(CHAR(DENDET.CODPOS)) AS CODPOS, " + " DENDET.MAT, DENDET.PRORAP, " + " DENDET.ANNCOM || '-01-01' AS DAL, " + " DENDET.ANNCOM || '-12-31' AS AL, " + " '1900-01-01' AS DATBEGDA, '9999-12-31' AS DATENDDA, '' AS STATUSPREV, " + " '" + PLVAR + "' AS PLVAR, " + " (SELECT RAGSOC FROM AZI WHERE CODPOS=DENDET.CODPOS) AS RAGSOC, " + "  '' AS SUBTY, '" + FLGCANC + "' AS FLGCANC, DENDET.ULTAGG, " + " CASE VALUE(DENDET.NUMMOVANN, '') WHEN '' THEN DENDET.IMPRET ELSE 0 END AS IMPRET, " + " CASE VALUE(DENDET.NUMMOVANN, '') WHEN '' THEN DENDET.IMPOCC ELSE 0 END AS IMPOCC " + " FROM DENDET, DENTES " + " WHERE " + " DENTES.CODPOS = DENDET.CODPOS " + " AND DENTES.ANNDEN = DENDET.ANNDEN " + " AND DENTES.MESDEN = DENDET.MESDEN " + " AND DENTES.PRODEN = DENDET.PRODEN " + " AND DENTES.TIPMOV = 'AR' " + " AND VALUE(DENDET.ESIRET, '') <> 'S' " + " AND DENTES.NUMMOV IS NOT NULL ";
            strSQL23 = (!(TIPMOV.Trim() == "RT") ? (PRODENDET == 0 ? str15 + " AND DENDET.CODPOS=" + CODPOS.ToString() + " AND DENDET.MAT IN (SELECT DISTINCT MAT " + " FROM DENDET WHERE CODPOS= " + CODPOS.ToString() + " AND ANNDEN= " + ANNDEN.ToString() + " AND MESDEN= " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + ") " + " AND DENDET.ANNCOM IN (SELECT DISTINCT ANNCOM " + " FROM DENDET WHERE CODPOS= " + CODPOS.ToString() + " AND ANNDEN= " + ANNDEN.ToString() + " AND MESDEN= " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + ") " : str15 + " AND DENDET.CODPOS=" + CODPOS.ToString() + " AND DENDET.ANNDEN= " + ANNDEN.ToString() + " AND DENDET.MESDEN= " + MESDEN.ToString() + " AND DENDET.PRODEN = " + PRODEN.ToString() + " AND DENDET.MAT= " + MAT.ToString() + " AND DENDET.PRODENDET = " + PRODENDET.ToString() + " AND DENDET.PRORAP = " + PRORAP.ToString() + " AND DENDET.TIPMOV = 'AR'") : str15 + " AND DENDET.ANNCOM IN ( SELECT DISTINCT ANNCOM " + " FROM DENDET WHERE DENDET.CODPOS=" + CODPOS.ToString() + " AND DENDET.ANNDEN= " + ANNDEN.ToString() + " AND DENDET.MESDEN= " + MESDEN.ToString() + " AND DENDET.PRODEN = " + PRODEN.ToString() + " AND DENDET.MAT= " + MAT.ToString() + " AND DENDET.PRODENDET = " + PRODENDET.ToString() + " )" + " AND DENDET.MAT IN (SELECT DISTINCT MAT " + " FROM DENDET WHERE DENDET.CODPOS=" + CODPOS.ToString() + " AND DENDET.ANNDEN= " + ANNDEN.ToString() + " AND DENDET.MESDEN= " + MESDEN.ToString() + " AND DENDET.PRODEN = " + PRODEN.ToString() + " AND DENDET.MAT= " + MAT.ToString() + " AND DENDET.PRODENDET = " + PRODENDET.ToString() + " )" + " AND DENDET.CODPOS=" + CODPOS.ToString() + " AND DENDET.ANNDEN= " + ANNDEN.ToString() + " AND DENDET.MESDEN= " + MESDEN.ToString() + " AND DENDET.PRODEN = " + PRODEN.ToString() + " AND DENDET.MAT= " + MAT.ToString() + " AND DENDET.PRODENDET = " + PRODENDET.ToString()) + " ) AS TABELLA " + " GROUP BY " + " ANNCOM, CODPOS, MAT, PRORAP, DAL, AL, DATBEGDA, DATENDDA, STATUSPREV, " + " PLVAR, RAGSOC, SUBTY, FLGCANC " + " ORDER BY MAT, DAL";
          }
          idocDatiE1Pitype = this.DL.GetDataTable(strSQL23);
          int num8 = idocDatiE1Pitype.Rows.Count - 1;
          for (int index = 0; index <= num8; ++index)
          {
            if (idocDatiE1Pitype.Rows[index]["RAGSOC"].ToString().Trim().Length > 29)
              idocDatiE1Pitype.Rows[index]["RAGSOC"] = (object) idocDatiE1Pitype.Rows[index]["RAGSOC"].ToString().Trim().Substring(0, 29);
            idocDatiE1Pitype.Rows[index][nameof (CODPOS)] = (object) ("A" + idocDatiE1Pitype.Rows[index][nameof (CODPOS)].ToString().Trim().PadLeft(6, '0'));
            idocDatiE1Pitype.Rows[index]["IMPUTILE"] = (object) (Convert.ToInt32(idocDatiE1Pitype.Rows[index]["IMPRET"]) - Convert.ToInt32(idocDatiE1Pitype.Rows[index]["IMPOCC"]));
            string strSQL24 = "SELECT VALUE(CHAR(DATDEC), '') AS DATDEC, VALUE(CHAR(DATCES), '') AS DATCES" + " FROM RAPLAV WHERE CODPOS = " + CODPOS.ToString() + " AND MAT = " + idocDatiE1Pitype.Rows[index][nameof (MAT)]?.ToString() + " AND PRORAP = " + idocDatiE1Pitype.Rows[index][nameof (PRORAP)]?.ToString();
            dataTable.Clear();
            dataTable = this.DL.GetDataTable(strSQL24);
            if (dataTable.Rows.Count > 0)
            {
              if (Convert.ToDateTime(dataTable.Rows[0][nameof (DATDEC)]).Year == Convert.ToInt32(idocDatiE1Pitype.Rows[index]["ANNCOM"]))
                idocDatiE1Pitype.Rows[index]["DAL"] = (object) DBMethods.Db2Date(dataTable.Rows[0][nameof (DATDEC)].ToString());
              if (dataTable.Rows[0]["DATCES"].ToString().Trim() != "")
                idocDatiE1Pitype.Rows[index]["AL"] = (object) DBMethods.Db2Date(dataTable.Rows[0]["DATCES"].ToString());
            }
          }
          break;
        case "9101":
          idocDatiE1Pitype = this.DL.GetDataTable(" SELECT TRIM(CHAR(CODPOS)) AS CODPOS, MAT, PRORAP, DATINIPRE, DATSCAPRE, IMPINDSOS, GIOINDSOS, " + " '" + PLVAR + "' AS PLVAR, '' AS SUBTY, " + " '1900-01-01' AS DATBEGDA, " + " '9999-12-31' AS DATENDDA, ULTAGG, " + " '" + FLGCANC + "' AS FLGCANC " + " FROM MODPRE " + " WHERE CODPOS=" + CODPOS.ToString() + " AND MAT=" + MAT.ToString() + " AND PRORAP=" + PRORAP.ToString() + " AND PROMOD=" + PROMOD.ToString());
          int num9 = idocDatiE1Pitype.Rows.Count - 1;
          for (int index = 0; index <= num9; ++index)
            idocDatiE1Pitype.Rows[index][nameof (CODPOS)] = (object) ("A" + idocDatiE1Pitype.Rows[index][nameof (CODPOS)].ToString().Trim().PadLeft(6, '0'));
          break;
      }
      return idocDatiE1Pitype;
    }

    public void WRITE_IDOC_E1PITYP(
      DataLayer objDataAccess,
      string VAR_E1PITYP,
      DataTable DT,
      bool CANCELLAZIONE)
    {
      DataTable dataTable1 = new DataTable();
      string strSQL = "SELECT IDTAB, TABSAP FROM IDOCTAB WHERE INFTYP = '" + VAR_E1PITYP + "' AND VALUE(USATO, 'S') = 'S'";
      dataTable1.Clear();
      DataTable dataTable2 = this.DL.GetDataTable(strSQL);
      int num = DT.Rows.Count - 1;
      for (int index = 0; index <= num; ++index)
      {
        DataRow row1 = DT.Rows[index];
        this.WRITE_EDID4(objDataAccess, 3, dataTable2.Rows[0]["TABSAP"].ToString().Trim(), VAR_E1PITYP, ref row1);
        if (!CANCELLAZIONE)
        {
          DataRow row2 = DT.Rows[index];
          this.WRITE_EDID4(objDataAccess, Convert.ToInt32(dataTable2.Rows[0]["IDTAB"]), dataTable2.Rows[0]["TABSAP"].ToString(), VAR_E1PITYP, ref row2);
        }
        else
        {
          string str = VAR_E1PITYP.ToUpper().Trim() ?? "";
          if (str == "0016" || str == "9001" || str == "9003" || str == "9004")
          {
            DataRow row3 = DT.Rows[index];
            this.WRITE_EDID4(objDataAccess, Convert.ToInt32(dataTable2.Rows[0]["IDTAB"]), dataTable2.Rows[0]["TABSAP"].ToString(), VAR_E1PITYP, ref row3);
          }
        }
      }
    }

    public void WRITE_IDOC_TESTATA(
      DataLayer objDataAccess,
      DataRow ROW_DATI,
      bool SCRIVI_SOLO_EPILOGI = false)
    {
      string DOCNUM = "";
      string str1 = "";
      string MANDT = "";
      string PSGNUM = "";
      string HLEVEL = "";
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataTable dataTable3 = new DataTable();
      string str2;
      if (SCRIVI_SOLO_EPILOGI)
      {
        str2 = " IDTAB = 2 ";
      }
      else
      {
        str2 = " IDTAB IN (1, 2) ";
        this.IDOC_CONT = Convert.ToInt32(this.Module_GetCONT(objDataAccess));
        if (this.objDtCONTIDOC == null)
        {
          this.objDtCONTIDOC = new DataTable();
          this.objDtCONTIDOC.Columns.Add("CONTIDOC");
        }
        this.objDtCONTIDOC.Rows.Add(this.objDtCONTIDOC.NewRow());
        this.objDtCONTIDOC.Rows[this.objDtCONTIDOC.Rows.Count - 1]["CONTIDOC"] = (object) this.IDOC_CONT;
        this.SEGNUM = "";
      }
      string strSQL1 = "SELECT * FROM IDOCTAB WHERE " + str2 + " AND VALUE(USATO, 'S') = 'S' ORDER BY ORDINE";
      dataTable1.Clear();
      DataTable dataTable4 = this.DL.GetDataTable(strSQL1);
      int num1 = dataTable4.Rows.Count - 1;
      for (int index1 = 0; index1 <= num1; ++index1)
      {
        string str3 = "";
        string str4 = "";
        string strSQL2 = "SELECT * FROM IDOCCAMPI WHERE IDTAB = " + dataTable4.Rows[index1]["IDTAB"]?.ToString() + " ORDER BY ORDINE";
        dataTable2.Clear();
        dataTable2 = this.DL.GetDataTable(strSQL2);
        this.SEGNUM = this.Module_GetSEGNUM(objDataAccess, this.IDOC_CONT);
        int num2 = dataTable2.Rows.Count - 1;
        for (int index2 = 0; index2 <= num2; ++index2)
        {
          str3 = str3 + ", " + dataTable2.Rows[index2]["CAMPO"]?.ToString();
          string upper1 = dataTable2.Rows[index2]["CAMPO"].ToString().Trim().ToUpper();
          if (!(upper1 == "MANDT"))
          {
            if (upper1 == "DOCNUM")
              DOCNUM = this.GetFormato(dataTable2.Rows[index2]["FORMATO"].ToString(), this.IDOC_CONT.ToString(), Convert.ToInt32(dataTable2.Rows[index2]["LUNGHEZZA"]));
          }
          else
            MANDT = this.GetFormato(dataTable2.Rows[index2]["FORMATO"].ToString(), dataTable2.Rows[index2]["VALORE"].ToString(), Convert.ToInt32(dataTable2.Rows[index2]["LUNGHEZZA"]));
          string upper2 = dataTable2.Rows[index2]["VALORE"].ToString().Trim().ToUpper();
          if (!(upper2 == "[CONTATORE]"))
          {
            str4 = upper2 == "IDOCVALORI" ? str4 + ", " + this.GetStrValues(objDataAccess, Convert.ToInt32(dataTable2.Rows[index2]["IDCAMPI"]), ref ROW_DATI, dataTable4.Rows[index1]["INFTYP"].ToString().Trim()) : str4 + ", " + this.GetFormato(dataTable2.Rows[index2]["FORMATO"].ToString(), dataTable2.Rows[index2]["VALORE"].ToString(), Convert.ToInt32(dataTable2.Rows[index2]["LUNGHEZZA"]));
          }
          else
          {
            string upper3 = dataTable2.Rows[index2]["CAMPO"].ToString().Trim().ToUpper();
            if (!(upper3 == "SEGNUM"))
            {
              if (!(upper3 == "CONT"))
              {
                if (!(upper3 == "PSGNUM"))
                {
                  if (!(upper3 == "HLEVEL"))
                  {
                    if (upper3 == "DOCNUM")
                      str1 = this.IDOC_CONT.ToString();
                  }
                  else
                    str1 = HLEVEL;
                }
                else
                {
                  this.Module_GetPSGNUM_HLEVEL(objDataAccess, Convert.ToInt32(dataTable4.Rows[index1]["IDTAB"]), MANDT, DOCNUM, ref HLEVEL, ref PSGNUM);
                  str1 = PSGNUM;
                }
              }
              else
                str1 = this.IDOC_CONT.ToString();
            }
            else
              str1 = this.SEGNUM;
            str4 = str4 + ", " + this.GetFormato(dataTable2.Rows[index2]["FORMATO"].ToString(), str1.ToString(), Convert.ToInt32(dataTable2.Rows[index2]["LUNGHEZZA"]));
          }
        }
        string str5 = str3 + ", CONT_NUM ";
        string str6 = str4 + ", " + this.IDOC_CONT.ToString() + " ";
        string str7 = "INSERT INTO " + dataTable4.Rows[index1]["TABSAP"].ToString().Trim() + " (" + str5.Substring(2) + ")";
        string str8 = " VALUES (" + str6.Substring(2) + ")";
        objDataAccess.WriteTransactionData(str7 + str8, CommandType.Text);
      }
    }

    private string Module_GetCONT(DataLayer objDataAccess)
    {
      DataTable dataTable = new DataTable();
      Random random = new Random();
      string strSQL1 = " SELECT VALUE(CHAR(CURRENT_TIMESTAMP),'') FROM MENU ";
      string str = Convert.ToString(objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text));
      int num = random.Next();
      string strSQL2 = "INSERT INTO COUNT_IDOC(ULTAGG, RND)" + " VALUES ('" + str + "', " + num.ToString() + ") ";
      objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
      string strSQL3 = " SELECT CONT FROM COUNT_IDOC WHERE ULTAGG = '" + str + "'" + " AND RND = " + num.ToString();
      return Convert.ToString(objDataAccess.Get1ValueFromSQL(strSQL3, CommandType.Text));
    }

    private void Module_GetPSGNUM_HLEVEL(
      DataLayer objDataAccess,
      int IDTAB,
      string MANDT,
      string DOCNUM,
      ref string HLEVEL,
      ref string PSGNUM)
    {
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = this.DL.GetDataTable("SELECT PADRE, HLEVEL FROM IDOCTAB WHERE IDTAB = " + IDTAB.ToString());
      string str = dataTable2.Rows[0]["PADRE"].ToString();
      HLEVEL = dataTable2.Rows[0][nameof (HLEVEL)].ToString();
      string strSQL = "SELECT SEGNUM FROM EDID4 WHERE CONT_NUM = " + this.IDOC_CONT.ToString() + " AND MANDT = " + MANDT + " AND DOCNUM = " + DOCNUM + " AND SEGNAM = " + DBMethods.DoublePeakForSql(str);
      PSGNUM = Convert.ToString(objDataAccess.Get1ValueFromSQL(strSQL, CommandType.Text));
    }

    private string Module_GetSEGNUM(DataLayer objDataAccess, int CONT)
    {
      DataTable dataTable = new DataTable();
      string strSQL = "SELECT VALUE(MAX(SEGNUM),0) + 1 AS SEGNUM FROM EDID4 WHERE CONT_NUM = " + CONT.ToString();
      return Convert.ToString(objDataAccess.Get1ValueFromSQL(strSQL, CommandType.Text));
    }

    private string GetStrValues(
      DataLayer objDataAccess,
      int IDCAMPI,
      ref DataRow ROW_DATI,
      string INFTYP)
    {
      DataTable dataTable1 = new DataTable();
      string str1 = "";
      DataTable dataTable2 = this.DL.GetDataTable("SELECT CAMPOSAP, TIPO, LUNGHEZZA, VALORE, VALDEFAULT, VALDTDATI, FORMATO FROM IDOCVALORI WHERE IDCAMPI = " + IDCAMPI.ToString() + " ORDER BY ORDINE");
      int num = dataTable2.Rows.Count - 1;
      for (int index = 0; index <= num; ++index)
      {
        string str2 = !(dataTable2.Rows[index]["CAMPOSAP"].ToString().Trim().ToUpper() == "INFTY") ? (!(dataTable2.Rows[index]["VALORE"].ToString().Trim().ToUpper() == "DTDATI") ? dataTable2.Rows[index]["VALDEFAULT"].ToString().Trim() : (!(dataTable2.Rows[index]["VALDTDATI"].ToString().Trim() == "") ? dataTable2.Rows[index]["VALDTDATI"].ToString().Trim() : dataTable2.Rows[index]["VALDEFAULT"].ToString().Trim())) : INFTYP;
        str1 += this.GetFormato(dataTable2.Rows[index]["FORMATO"].ToString(), str2.ToString(), Convert.ToInt32(dataTable2.Rows[index]["LUNGHEZZA"]), false);
      }
      return "'" + str1.Replace("'", "''") + "'";
    }

    private string GetFormato(string FORMATO, string TESTO, int LUNGHEZZA, bool USE_DOUBLEPEAK = true)
    {
      TESTO = TESTO.Trim();
      switch (FORMATO.Trim().ToUpper() ?? "")
      {
        case "0 DX":
          return !USE_DOUBLEPEAK ? TESTO.ToString().Trim().PadRight(LUNGHEZZA, '0') : DBMethods.DoublePeakForSql(TESTO.ToString().Trim().PadRight(LUNGHEZZA, '0'));
        case "0 SX":
          return !USE_DOUBLEPEAK ? TESTO.ToString().Trim().PadLeft(LUNGHEZZA, '0') : DBMethods.DoublePeakForSql(TESTO.ToString().Trim().PadLeft(LUNGHEZZA, '0'));
        case "BLANK DX":
          return !USE_DOUBLEPEAK ? TESTO.ToString().Trim().PadRight(LUNGHEZZA, ' ') : DBMethods.DoublePeakForSql(TESTO.ToString().Trim().PadRight(LUNGHEZZA, ' '));
        case "BLANK SX":
          return !USE_DOUBLEPEAK ? TESTO.ToString().Trim().PadLeft(LUNGHEZZA, ' ') : DBMethods.DoublePeakForSql(TESTO.ToString().Trim().PadLeft(LUNGHEZZA, ' '));
        case "HHMMSS":
          if (!string.IsNullOrEmpty(TESTO) && TESTO != "000000")
          {
            TESTO = TESTO.Replace(".", "");
            TESTO = TESTO.Split(' ')[1];
          }
          return TESTO.PadRight(LUNGHEZZA, ' ');
        case "SUBQUERY":
          return TESTO;
        case "VALUTA":
          TESTO = TESTO.Replace(",", ".");
          return TESTO.ToString().Trim().PadRight(LUNGHEZZA, ' ');
        case "YYYYMMDD":
          if (!string.IsNullOrEmpty(TESTO) & TESTO != "99991231" & TESTO != "18000101" && TESTO != "00000000")
          {
            TESTO = DBMethods.Db2Date(TESTO);
            TESTO = TESTO.Replace("-", "");
          }
          return TESTO.PadRight(LUNGHEZZA, ' ');
        default:
          return !USE_DOUBLEPEAK ? TESTO.ToString().Trim().PadRight(LUNGHEZZA, ' ') : DBMethods.DoublePeakForSql(TESTO.ToString().Trim().PadRight(LUNGHEZZA, ' '));
      }
    }

    public bool AGGIORNA_RAPLAV_INPS(string CODPOS, string MAT, string PRORAP)
    {
      TFI.OCM.Utente.Utente utente = (TFI.OCM.Utente.Utente) HttpContext.Current.Session["utente"];
      return Convert.ToInt32(CODPOS) != 0 ? this.DL.WriteTransactionData("UPDATE RAPLAV SET DATINPS = NULL, TIPOPE = 'M', " + " ULTAGG = CURRENT_TIMESTAMP, UTEAGG = " + DBMethods.DoublePeakForSql(utente.CodPosizione) + " WHERE CODPOS = " + CODPOS + " AND MAT=" + MAT + " AND PRORAP = " + PRORAP, CommandType.Text) : this.DL.WriteTransactionData("UPDATE RAPLAV SET DATINPS = NULL, TIPOPE = 'M', " + " ULTAGG = CURRENT_TIMESTAMP, UTEAGG = " + DBMethods.DoublePeakForSql(utente.CodPosizione) + " WHERE MAT=" + MAT + " AND CURRENT_DATE BETWEEN DATDEC AND VALUE(DATCES,'9999-12-31')", CommandType.Text);
    }

    public void Aggiorna_IDOC(ref DataLayer cmd)
    {
      int num = this.objDtCONTIDOC.Rows.Count - 1;
      for (short index = 0; (int) index <= num; ++index)
      {
        string str = "UPDATE EDIDC SET STATUS = 64 WHERE STATUS = 00 AND CONT_NUM = '" + this.objDtCONTIDOC.Rows[(int) index]["CONTIDOC"].ToString() + "' ";
        cmd.objCommand.CommandText = str;
        cmd.objCommand.ExecuteNonQuery();
      }
    }

    public bool Check_65Anni(DateTime datDataConfronto, string strDataNascita) => !string.IsNullOrEmpty(strDataNascita.Trim()) && DateTime.Compare(DateTime.Parse(strDataNascita).AddYears(65), datDataConfronto) <= 0;
  }
}
