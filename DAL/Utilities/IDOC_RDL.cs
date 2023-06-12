// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Utilities.IDOC_RDL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using System;
using System.Data;
using TFI.DAL.ConnectorDB;

namespace TFI.DAL.Utilities
{
  public class IDOC_RDL
  {
    private Utile Utils = new Utile();
    public DataTable objDtCONTIDOC;
    private int IDOC_CONT;
    private string SEGNUM = "";

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
      string strSQL1 = "SELECT * FROM IDOCCAMPI WHERE IDTAB = " + IDTAB.ToString() + " ORDER BY ORDINE";
      dataTable1.Clear();
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL1);
      this.SEGNUM = this.Module_GetSEGNUM(objDataAccess, this.IDOC_CONT);
      for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
      {
        str2 = str2 + ", " + dataTable2.Rows[index]["CAMPO"]?.ToString();
        string upper1 = dataTable2.Rows[index]["CAMPO"].ToString().Trim().ToUpper();
        if (!(upper1 == "MANDT"))
        {
          if (upper1 == "DOCNUM")
            DOCNUM = this.GetFormato(dataTable2.Rows[index]["FORMATO"].ToString(), this.IDOC_CONT.ToString(), Convert.ToInt32(dataTable2.Rows[index]["LUNGHEZZA"]));
        }
        else
          MANDT = this.GetFormato(dataTable2.Rows[index]["FORMATO"].ToString(), dataTable2.Rows[index]["VALORE"].ToString(), Convert.ToInt32(dataTable2.Rows[index]["LUNGHEZZA"]));
        string upper2 = dataTable2.Rows[index]["VALORE"].ToString().Trim().ToUpper();
        if (!(upper2 == "[CONTATORE]"))
        {
          str3 = upper2 == "IDOCVALORI" ? str3 + ", " + this.GetStrValues(objDataAccess, Convert.ToInt32(dataTable2.Rows[index]["IDCAMPI"]), ref ROW_DATI, VAR_E1PITYP) : str3 + ", " + this.GetFormato(dataTable2.Rows[index]["FORMATO"].ToString(), dataTable2.Rows[index]["VALORE"].ToString(), Convert.ToInt32(dataTable2.Rows[index]["LUNGHEZZA"]));
        }
        else
        {
          string upper3 = dataTable2.Rows[index]["CAMPO"].ToString().Trim().ToUpper();
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
          str3 = str3 + ", " + this.GetFormato(dataTable2.Rows[index]["FORMATO"].ToString(), str1.ToString(), Convert.ToInt32(dataTable2.Rows[index]["LUNGHEZZA"]));
        }
      }
      string str4 = str2 + ", CONT_NUM ";
      string str5 = str3 + ", " + this.IDOC_CONT.ToString() + " ";
      string strSQL2 = "INSERT INTO " + TABSAP + " (" + str4.Substring(2) + ")" + (" VALUES (" + str5.Substring(2) + ")");
      objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
    }

    public DataTable GET_IDOC_DATI_E1PITYPE(
      DataLayer objDataAccess,
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
      string strSQL1 = "";
      DataTable dataTable = new DataTable();
      string str1 = Convert.ToString(this.Utils.Module_GetDataSistema());
      switch (VAR_E1PITYP.ToUpper().Trim())
      {
        case "0000":
        case "0001":
        case "0002":
        case "0003":
        case "0302":
          if (FASE == "VARIAZIONE INQUADRAMENTO" || FASE == "TRASFERIMENTO" || FASE == "ANNULLAMENTO TRASFERIMENTO")
          {
            string str2 = " SELECT ISCT.MAT, ";
            string str3 = string.IsNullOrEmpty(DATBEGDA) ? str2 + " '' AS DATINI, " + " '' AS DATBEGDA, " : str2 + DBMethods.Db2Date(DATBEGDA.Trim()) + " AS DATINI, " + DBMethods.Db2Date(DATBEGDA.Trim()) + " AS DATBEGDA, ";
            string strSQL2 = (string.IsNullOrEmpty(DATENDDA) ? str3 + " '' AS DATFIN, " + " '' AS DATENDDA, " : str3 + DBMethods.Db2Date(DATENDDA.Trim()) + " AS DATFIN, " + DBMethods.Db2Date(DATENDDA.Trim()) + " AS DATENDDA, ") + " '" + MASSN + "' AS MASSN, " + " '" + MASSG + "' AS MASSG, " + " '" + PLVAR + "' AS PLVAR, " + " '" + FLGCANC + "' AS FLGCANC, '' AS SUBTY, " + " CURRENT_DATE AS ULTAGG " + " FROM ISCT WHERE ISCT.MAT=" + MAT.ToString();
            idocDatiE1Pitype = objDataAccess.GetDataTable(strSQL2);
            break;
          }
          string strSQL3 = " SELECT TRIM(VALUE(ISCT.COG,'')) || ' ' || TRIM(VALUE(ISCT.NOM,'')) " + " AS COGNOM1, '' AS COGNOM2, ISCT.MAT, ISCT.COG, ISCT.NOM, ISCT.CODFIS, ISCT.DATNAS, " + " ISCT.DATISC AS DATINI, " + DBMethods.Db2Date(DATENDDA) + " AS DATFIN, " + " ISCT.DATISC AS DATBEGDA, " + DBMethods.Db2Date(DATENDDA) + " AS DATENDDA, " + " ISCT.ULTAGG, CASE ISCT.SES WHEN 'F' THEN '2' WHEN 'M' THEN '1' " + " ELSE ' ' END AS SEX, '" + FLGCANC + "' AS FLGCANC, " + " (SELECT DENCOM FROM CODCOM WHERE CODCOM=ISCT.CODCOM) AS DENCOMNAS, " + " (SELECT SIGPRO FROM CODCOM WHERE CODCOM=ISCT.CODCOM) AS SIGPRONAS, " + " (SELECT CASE SIGPRO WHEN 'EE' THEN 'ZZ' ELSE 'IT' END FROM CODCOM " + "  WHERE CODCOM=ISCT.CODCOM) AS DENSTANAS, '' AS SUBTY, '' AS STATO_LUNGO," + " ISCT.CODCOM AS COMUNE, " + " '' AS CODREGNAS, " + " '" + MASSN + "' AS MASSN, " + " '" + MASSG + "' AS MASSG, " + " '" + PLVAR + "' AS PLVAR, " + " '' AS DATINISTO " + " FROM ISCT WHERE ISCT.MAT=" + MAT.ToString() + " ORDER BY ISCT.MAT ";
          idocDatiE1Pitype = objDataAccess.GetDataTable(strSQL3);
          for (int index = 0; index <= idocDatiE1Pitype.Rows.Count - 1; ++index)
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
              string strSQL4 = " SELECT CODREG FROM CODREG WHERE CODREG IN(SELECT CODREG " + " FROM SIGPRO WHERE SIGPRO IN (SELECT SIGPRO FROM CODCOM " + " WHERE CODCOM=" + DBMethods.DoublePeakForSql(idocDatiE1Pitype.Rows[index]["COMUNE"].ToString().Trim()) + "))";
              idocDatiE1Pitype.Rows[index]["CODREGNAS"] = (object) objDataAccess.Get1ValueFromSQL(strSQL4, CommandType.Text);
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
          string str4 = "SELECT ISCD.MAT, ISCD.IND, ISCD.CO, ISCD.NUMCIV, ISCD.DATINI, date( " + DBMethods.Db2Date(DATENDDA) + ") AS DATFIN ,";
          string strSQL5 = (!(FASE == "CANCELLAZIONE") ? str4 + " ISCD.DATINI AS DATBEGDA, date( " + DBMethods.Db2Date(DATENDDA) + ") AS DATENDDA , " : str4 + " '1900-01-01' AS DATBEGDA, date(9999-12-31) AS DATENDDA , ") + " (SELECT DENDUG FROM DUG WHERE CODDUG=ISCD.CODDUG) AS DENDUG, '" + FLGCANC + "' AS FLGCANC, " + " ISCD.DENLOC, ISCD.DENSTAEST, ISCD.CAP, ISCD.SIGPRO, 'TEL2' AS LBLTEL2, 'TEL3' AS LBLTEL3, ISCD.TEL1, '' AS IND1, '' AS IND2, " + " '' AS DENCOM, ISCD.CODCOM, " + " '" + PLVAR + "' AS PLVAR, '1' AS SUBTY, " + " ISCD.TEL2, 'FAX1' AS LBLFAX1, ISCD.FAX, ISCD.ULTAGG FROM ISCD WHERE MAT = " + MAT.ToString() + " " + " ORDER BY DATINI ";
          idocDatiE1Pitype = objDataAccess.GetDataTable(strSQL5);
          for (int index = idocDatiE1Pitype.Rows.Count - 1; index >= 0; index += -1)
          {
            if (idocDatiE1Pitype.Rows[index]["CODCOM"].ToString().Trim() != "")
            {
              string strSQL6 = "SELECT VALUE(DENCOM,'') AS DENCOM FROM CODCOM WHERE CODCOM = " + DBMethods.DoublePeakForSql(idocDatiE1Pitype.Rows[index]["CODCOM"].ToString().Trim());
              string str5 = objDataAccess.Get1ValueFromSQL(strSQL6, CommandType.Text);
              idocDatiE1Pitype.Rows[index]["DENCOM"] = (object) str5.Trim();
            }
            idocDatiE1Pitype.Rows[index]["DATFIN"] = index == idocDatiE1Pitype.Rows.Count - 1 ? (object) "9999-12-31" : (object) Convert.ToDateTime(idocDatiE1Pitype.Rows[index + 1]["DATINI"]).AddDays(-1.0);
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
          int num1;
          if (FLGCANC == "D")
          {
            string str7 = " SELECT TRIM(CHAR(RAPLAV.CODPOS)) AS CODPOS, RAPLAV.MAT, RAPLAV.PRORAP, RAPLAV.DATDEC, RAPLAV.DATASS, RAPLAV.DATCES, " + " STORDL.TIPRAP, STORDL.CODCON, STORDL.CODGRUASS, VALUE(STORDL.CODLOC, 0) AS CODLOC, STORDL.CODGRUASS, 0 AS CODCONLOC, " + " '" + PLVAR + "' AS PLVAR, '' AS SUBTY, " + " (SELECT DISTINCT VALUE(CODQUACON, 0) AS CODQUACON FROM CONRIF WHERE CODCON=STORDL.CODCON) AS CODQUACON, " + " STORDL.DATINI AS DATINIAPP, STORDL.DATFIN AS DATFINAPP, " + " '1900-01-01' AS DATBEGDA, '9999-12-31' AS DATENDDA, " + " CASE VALUE(STORDL.PERPAR, '')  WHEN 0 THEN '' ELSE 'X' END AS FLGPARTIM, '' AS FLGOBBPRV, '' AS FLGOBBTFR, 'A' AS STATDIP, ISCT.DATNAS," + " '' AS PERCTRASF, '' AS TIPCON, STORDL.PERPAR, " + " STORDL.NUMMEN, STORDL.CODLIV, STORDL.DATSCATER, '" + FLGCANC + "' AS FLGCANC, STORDL.DATFIN, STORDL.DATINI, STORDL.ULTAGG FROM RAPLAV, STORDL, ISCT" + " WHERE RAPLAV.CODPOS=STORDL.CODPOS AND RAPLAV.MAT=STORDL.MAT AND RAPLAV.PRORAP=STORDL.PRORAP AND RAPLAV.MAT=ISCT.MAT";
            string str8 = FASE == "VARIAZIONE INQUADRAMENTO" || FASE == "TRASFERIMENTO" || FASE == "ANNULLAMENTO TRASFERIMENTO" ? str7 + " AND RAPLAV.MAT = " + MAT.ToString() : str7 + " AND RAPLAV.CODPOS = '" + CODPOS.ToString() + "' " + " AND RAPLAV.MAT = '" + MAT.ToString() + "' " + " AND RAPLAV.PRORAP = '" + PRORAP.ToString() + "' ";
            string strSQL7 = !(flgWEB == "S") ? str8 + " ORDER BY CODPOS, MAT, PRORAP, DATINI " : str8 + " ORDER BY DATINI DESC FETCH FIRST 1 ROWS ONLY ";
            idocDatiE1Pitype = objDataAccess.GetDataTable(strSQL7);
            for (int index = 0; index <= idocDatiE1Pitype.Rows.Count - 1; ++index)
            {
              if (FASE == "TRASFERIMENTO")
              {
                string strSQL8 = " SELECT VALUE(PERTRA, 0.0) FROM TRARAP WHERE CODPOSTRA = " + idocDatiE1Pitype.Rows[index][nameof (CODPOS)]?.ToString() + " AND MATTRA = " + idocDatiE1Pitype.Rows[index][nameof (MAT)]?.ToString() + " AND PRORAPTRA= " + idocDatiE1Pitype.Rows[index][nameof (PRORAP)]?.ToString() + " ORDER BY DATTRA DESC FETCH FIRST ROWS ONLY ";
                Decimal num2 = string.IsNullOrEmpty(objDataAccess.Get1ValueFromSQL(strSQL8, CommandType.Text)) ? 100.0M : Convert.ToDecimal(objDataAccess.Get1ValueFromSQL(strSQL8, CommandType.Text));
                if (Convert.ToDouble(num2) > 0.0)
                  idocDatiE1Pitype.Rows[index][nameof (PERCTRASF)] = (object) num2;
              }
              idocDatiE1Pitype.Rows[index][nameof (CODPOS)] = (object) ("A" + idocDatiE1Pitype.Rows[index][nameof (CODPOS)].ToString().Trim().PadLeft(6, '0'));
              idocDatiE1Pitype.Rows[index]["STATDIP"] = !(idocDatiE1Pitype.Rows[index]["DATCES"].ToString().Trim() != "" & idocDatiE1Pitype.Rows[index]["DATCES"].ToString().Trim() != "9999-12-31") ? (object) "A" : (!(idocDatiE1Pitype.Rows[index]["DATCES"].ToString().Trim() == idocDatiE1Pitype.Rows[index]["DATFIN"].ToString().Trim()) ? (object) "A" : (object) "C");
              int int32_1;
              if (idocDatiE1Pitype.Rows[index]["CODLOC"].ToString() != "0")
              {
                string strSQL9 = " SELECT VALUE(PROCON,0) FROM CONLOC" + " WHERE CODLOC = '" + idocDatiE1Pitype.Rows[index]["CODLOC"]?.ToString() + "' " + " AND DATDEC <= '" + DBMethods.Db2Date(idocDatiE1Pitype.Rows[index]["DATINI"].ToString()) + "' " + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
                int32_1 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL9, CommandType.Text));
                string strSQL10 = " SELECT VALUE(PROLOC,0) FROM CONLOC" + " WHERE CODLOC = '" + idocDatiE1Pitype.Rows[index]["CODLOC"]?.ToString() + "' " + " AND DATDEC <= '" + DBMethods.Db2Date(idocDatiE1Pitype.Rows[index]["DATINI"].ToString()) + "' " + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
                int int32_2 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL10, CommandType.Text));
                string strSQL11 = " SELECT VALUE(CODTIPCON,0) FROM CONLOC WHERE" + " CODCON = '" + idocDatiE1Pitype.Rows[index]["CODCON"]?.ToString() + "' " + " AND PROCON = '" + int32_1.ToString() + "' " + " AND CODLOC = '" + idocDatiE1Pitype.Rows[index]["CODLOC"]?.ToString() + "' " + " AND PROLOC = '" + int32_2.ToString() + "' ";
                idocDatiE1Pitype.Rows[index]["TIPCON"] = (object) objDataAccess.Get1ValueFromSQL(strSQL11, CommandType.Text);
              }
              else
              {
                string strSQL12 = "SELECT VALUE(PROCON,0) FROM CONRIF " + " WHERE CODCON = '" + idocDatiE1Pitype.Rows[index]["CODCON"]?.ToString() + "' " + " AND DATDEC <= '" + DBMethods.Db2Date(idocDatiE1Pitype.Rows[index]["DATINI"].ToString()) + "' " + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
                int32_1 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL12, CommandType.Text));
                string strSQL13 = " SELECT VALUE(CODTIPCON,0) FROM CONRIF WHERE" + " CODCON = '" + idocDatiE1Pitype.Rows[index]["CODCON"]?.ToString() + "' " + " AND PROCON = '" + int32_1.ToString() + "' ";
                idocDatiE1Pitype.Rows[index]["TIPCON"] = (object) objDataAccess.Get1ValueFromSQL(strSQL13, CommandType.Text);
              }
              if (idocDatiE1Pitype.Rows[index]["CODQUACON"].ToString().Trim() != "0" & idocDatiE1Pitype.Rows[index]["CODQUACON"].ToString().Trim() != "")
              {
                string strSQL14 = " SELECT VALUE(COUNT(*),0) FROM ALIFORASS WHERE" + " CODGRUASS = '" + idocDatiE1Pitype.Rows[index]["CODGRUASS"]?.ToString() + "' " + " AND CODQUACON = '" + idocDatiE1Pitype.Rows[index]["CODQUACON"]?.ToString() + "' " + " AND CODFORASS IN(SELECT CODFORASS FROM FORASS" + " WHERE CATFORASS IN('PREV'))";
                num1 = 0;
                if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL14, CommandType.Text)) > 0)
                {
                  if (idocDatiE1Pitype.Rows[index]["DATFIN"].ToString() == "9999-12-31")
                  {
                    if (!this.Utils.Module_Check_65Anni(Convert.ToDateTime(str1), idocDatiE1Pitype.Rows[index]["DATNAS"].ToString()))
                      idocDatiE1Pitype.Rows[index]["FLGOBBPRV"] = (object) "X";
                  }
                  else if (!this.Utils.Module_Check_65Anni(Convert.ToDateTime(idocDatiE1Pitype.Rows[index]["DATFIN"]), idocDatiE1Pitype.Rows[index]["DATNAS"].ToString()))
                    idocDatiE1Pitype.Rows[index]["FLGOBBPRV"] = (object) "X";
                }
                string strSQL15 = " SELECT VALUE(COUNT(*),0) FROM ALIFORASS WHERE" + " CODGRUASS = " + idocDatiE1Pitype.Rows[index]["CODGRUASS"]?.ToString() + " AND CODQUACON = " + idocDatiE1Pitype.Rows[index]["CODQUACON"]?.ToString() + " AND CODFORASS IN(SELECT CODFORASS FROM FORASS" + " WHERE CATFORASS IN('TFR'))";
                num1 = 0;
                if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL15, CommandType.Text)) > 0)
                  idocDatiE1Pitype.Rows[index]["FLGOBBTFR"] = (object) "X";
              }
            }
            break;
          }
          string str9 = " SELECT TRIM(CHAR(RAPLAV.CODPOS)) AS CODPOS, RAPLAV.MAT, RAPLAV.PRORAP, RAPLAV.DATDEC, RAPLAV.DATASS, RAPLAV.DATCES, " + " STORDL.TIPRAP, STORDL.CODCON, VALUE(STORDL.CODLOC, 0) AS CODLOC, STORDL.CODGRUASS, 0 AS CODCONLOC, '" + FLGCANC + "' AS FLGCANC, " + " (SELECT DISTINCT VALUE(CODQUACON, 0) AS CODQUACON FROM CONRIF WHERE CODCON=STORDL.CODCON) AS CODQUACON, " + " STORDL.DATINI AS DATINIAPP, STORDL.DATFIN AS DATFINAPP, " + " '1900-01-01' AS DATBEGDA, '9999-12-31' AS DATENDDA, " + " CASE VALUE(STORDL.PERPAR, '')  WHEN 0 THEN '' ELSE 'X' END AS FLGPARTIM, '' AS FLGOBBPRV, '' AS FLGOBBTFR, 'A' AS STATDIP, ISCT.DATNAS," + " '" + PLVAR + "' AS PLVAR, '' AS TIPCON, '' AS SUBTY, " + " '' AS PERCTRASF, STORDL.PERPAR, " + " STORDL.NUMMEN, STORDL.CODLIV, STORDL.DATSCATER, STORDL.DATFIN, STORDL.DATINI, STORDL.ULTAGG FROM RAPLAV, STORDL, ISCT " + " WHERE RAPLAV.CODPOS=STORDL.CODPOS AND RAPLAV.MAT=STORDL.MAT AND RAPLAV.PRORAP=STORDL.PRORAP AND RAPLAV.MAT=ISCT.MAT";
          string str10 = FASE == "VARIAZIONE INQUADRAMENTO" || FASE == "TRASFERIMENTO" || FASE == "ANNULLAMENTO TRASFERIMENTO" ? str9 + " AND RAPLAV.MAT = " + MAT.ToString() : str9 + " AND RAPLAV.CODPOS = " + CODPOS.ToString() + " AND RAPLAV.MAT = " + MAT.ToString() + " AND RAPLAV.PRORAP = " + PRORAP.ToString();
          string strSQL16 = !(flgWEB == "S") ? str10 + " ORDER BY CODPOS, MAT, PRORAP, DATINI " : str10 + " ORDER BY DATINI DESC FETCH FIRST 1 ROWS ONLY ";
          idocDatiE1Pitype = objDataAccess.GetDataTable(strSQL16);
          for (int index = idocDatiE1Pitype.Rows.Count - 1; index >= 0; index += -1)
          {
            if (FASE == "TRASFERIMENTO")
            {
              string strSQL17 = " SELECT VALUE(PERTRA, 0.0) FROM TRARAP WHERE CODPOSTRA = " + idocDatiE1Pitype.Rows[index][nameof (CODPOS)]?.ToString() + " AND MATTRA = " + idocDatiE1Pitype.Rows[index][nameof (MAT)]?.ToString() + " AND PRORAPTRA= " + idocDatiE1Pitype.Rows[index][nameof (PRORAP)]?.ToString() + " ORDER BY DATTRA DESC FETCH FIRST ROWS ONLY ";
              Decimal num3 = string.IsNullOrEmpty(objDataAccess.Get1ValueFromSQL(strSQL17, CommandType.Text)) ? 100.0M : Convert.ToDecimal(objDataAccess.Get1ValueFromSQL(strSQL17, CommandType.Text));
              if (Convert.ToDouble(num3) > 0.0)
                idocDatiE1Pitype.Rows[index][nameof (PERCTRASF)] = (object) num3;
            }
            idocDatiE1Pitype.Rows[index][nameof (CODPOS)] = (object) ("A" + idocDatiE1Pitype.Rows[index][nameof (CODPOS)].ToString().Trim().PadLeft(6, '0'));
            idocDatiE1Pitype.Rows[index]["STATDIP"] = !(idocDatiE1Pitype.Rows[index]["DATCES"].ToString().Trim() != "" & idocDatiE1Pitype.Rows[index]["DATCES"].ToString().Trim() != "9999-12-31") ? (object) "A" : (!(idocDatiE1Pitype.Rows[index]["DATCES"].ToString().Trim() == idocDatiE1Pitype.Rows[index]["DATFIN"].ToString().Trim()) ? (object) "A" : (object) "C");
            idocDatiE1Pitype.Rows[index]["CODCONLOC"] = !(idocDatiE1Pitype.Rows[index]["CODLOC"].ToString().Trim() != "0") ? idocDatiE1Pitype.Rows[index]["CODCON"] : idocDatiE1Pitype.Rows[index]["CODLOC"];
            if (idocDatiE1Pitype.Rows[index]["CODQUACON"].ToString().Trim() == "")
              idocDatiE1Pitype.Rows[index]["CODQUACON"] = (object) "0";
            int int32_3;
            if (idocDatiE1Pitype.Rows[index]["CODLOC"].ToString() != "0")
            {
              string strSQL18 = " SELECT VALUE(PROCON,0) FROM CONLOC" + " WHERE CODLOC = '" + idocDatiE1Pitype.Rows[index]["CODLOC"]?.ToString() + "' " + " AND DATDEC <= '" + DBMethods.Db2Date(idocDatiE1Pitype.Rows[index]["DATINI"].ToString()) + "' " + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
              int32_3 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL18, CommandType.Text));
              string strSQL19 = " SELECT VALUE(PROLOC,0) FROM CONLOC" + " WHERE CODLOC = '" + idocDatiE1Pitype.Rows[index]["CODLOC"]?.ToString() + "' " + " AND DATDEC <= '" + DBMethods.Db2Date(idocDatiE1Pitype.Rows[index]["DATINI"].ToString()) + "' " + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
              int int32_4 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL19, CommandType.Text));
              string strSQL20 = " SELECT VALUE(CODTIPCON,0) FROM CONLOC WHERE" + " CODCON = '" + idocDatiE1Pitype.Rows[index]["CODCON"]?.ToString() + "' " + " AND PROCON = '" + int32_3.ToString() + "' " + " AND CODLOC = '" + idocDatiE1Pitype.Rows[index]["CODLOC"]?.ToString() + "' " + " AND PROLOC = '" + int32_4.ToString() + "' ";
              idocDatiE1Pitype.Rows[index]["TIPCON"] = (object) objDataAccess.Get1ValueFromSQL(strSQL20, CommandType.Text);
            }
            else
            {
              string strSQL21 = "SELECT VALUE(PROCON,0) FROM CONRIF " + " WHERE CODCON = '" + idocDatiE1Pitype.Rows[index]["CODCON"]?.ToString() + "' " + " AND DATDEC <= '" + DBMethods.Db2Date(idocDatiE1Pitype.Rows[index]["DATINI"].ToString()) + "' " + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
              int32_3 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL21, CommandType.Text));
              string strSQL22 = " SELECT VALUE(CODTIPCON,0) FROM CONRIF WHERE" + " CODCON = '" + idocDatiE1Pitype.Rows[index]["CODCON"]?.ToString() + "' " + " AND PROCON = " + int32_3.ToString();
              idocDatiE1Pitype.Rows[index]["TIPCON"] = (object) objDataAccess.Get1ValueFromSQL(strSQL22, CommandType.Text);
            }
            if (idocDatiE1Pitype.Rows[index]["CODQUACON"].ToString().Trim() != "0" & idocDatiE1Pitype.Rows[index]["CODQUACON"].ToString().Trim() != "")
            {
              string strSQL23 = " SELECT VALUE(COUNT(*),0) FROM ALIFORASS WHERE" + " CODGRUASS = '" + idocDatiE1Pitype.Rows[index]["CODGRUASS"]?.ToString() + "' " + " AND CODQUACON = '" + idocDatiE1Pitype.Rows[index]["CODQUACON"]?.ToString() + "' " + " AND CODFORASS IN(SELECT CODFORASS FROM FORASS" + " WHERE CATFORASS IN('PREV'))";
              num1 = 0;
              if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL23, CommandType.Text)) > 0 && idocDatiE1Pitype.Rows[index]["DATFIN"].ToString() == "9999-12-31")
              {
                if (idocDatiE1Pitype.Rows[index]["DATFIN"].ToString() == "9999-12-31")
                {
                  if (!this.Utils.Module_Check_65Anni(Convert.ToDateTime(str1), idocDatiE1Pitype.Rows[index]["DATNAS"].ToString()))
                    idocDatiE1Pitype.Rows[index]["FLGOBBPRV"] = (object) "X";
                }
                else if (!this.Utils.Module_Check_65Anni(Convert.ToDateTime(idocDatiE1Pitype.Rows[index]["DATFIN"]), idocDatiE1Pitype.Rows[index]["DATNAS"].ToString()))
                  idocDatiE1Pitype.Rows[index]["FLGOBBPRV"] = (object) "X";
              }
              string strSQL24 = " SELECT VALUE(COUNT(*),0) FROM ALIFORASS WHERE" + " CODGRUASS = '" + idocDatiE1Pitype.Rows[index]["CODGRUASS"]?.ToString() + "' " + " AND CODQUACON = '" + idocDatiE1Pitype.Rows[index]["CODQUACON"]?.ToString() + "' " + " AND CODFORASS IN(SELECT CODFORASS FROM FORASS" + " WHERE CATFORASS IN('TFR'))";
              num1 = 0;
              if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL24, CommandType.Text)) > 0)
                idocDatiE1Pitype.Rows[index]["FLGOBBTFR"] = (object) "X";
            }
          }
          break;
        case "9003":
          string strSQL25 = "SELECT MAT, TRIM(CHAR(CODPOS)) AS CODPOS, PRORAP, DATINISOS AS DATINI, " + " DATFINSOS AS DATFIN, TRIM(CHAR(PERAZI)) AS PERAZI, " + " '1900-01-01' AS DATBEGDA, '9999-12-31' AS DATENDDA, '' AS SUBTY, " + " (SELECT RAGSOC FROM AZI WHERE CODPOS=SOSRAP.CODPOS) AS RAGSOC, " + " '" + PLVAR + "' AS PLVAR, " + " '" + FLGCANC + "' AS FLGCANC, " + " PROSOS, CODSOS, ULTAGG, UTEAGG " + " FROM SOSRAP WHERE CODPOS = " + CODPOS.ToString() + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString() + " AND PROSOS = " + PROSOS.ToString();
          idocDatiE1Pitype = objDataAccess.GetDataTable(strSQL25);
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
            string str11 = TIPMOV.Trim();
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
          idocDatiE1Pitype = objDataAccess.GetDataTable(strSQL1);
          for (int index = 0; index <= idocDatiE1Pitype.Rows.Count - 1; ++index)
          {
            string strSQL26 = "SELECT VALUE(PERPAR, 0.00) AS PERPAR FROM STORDL " + " WHERE CODPOS = " + idocDatiE1Pitype.Rows[index][nameof (CODPOS)]?.ToString() + " AND MAT = " + idocDatiE1Pitype.Rows[index][nameof (MAT)]?.ToString() + " AND PRORAP = " + idocDatiE1Pitype.Rows[index][nameof (PRORAP)]?.ToString() + " AND DATINI <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(idocDatiE1Pitype.Rows[index]["DAL"].ToString())) + " AND DATFIN >= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(idocDatiE1Pitype.Rows[index]["AL"].ToString())) + " ORDER BY DATINI DESC FETCH FIRST 1 ROWS ONLY";
            string str15 = objDataAccess.Get1ValueFromSQL(strSQL26, CommandType.Text);
            Decimal num4 = str15 != string.Empty ? Convert.ToDecimal(str15) : 0M;
            if (num4 != 0.0M)
              idocDatiE1Pitype.Rows[index]["PERPAR"] = (object) num4;
            if (TIPMOV.ToString() == "RT")
            {
              string strSQL27 = " SELECT DISTINCT VALUE(PROMOD,0) FROM MODPREDET WHERE" + " CODPOS = " + idocDatiE1Pitype.Rows[index][nameof (CODPOS)]?.ToString() + " AND MAT = " + idocDatiE1Pitype.Rows[index][nameof (MAT)]?.ToString() + " AND PRORAP = " + idocDatiE1Pitype.Rows[index][nameof (PRORAP)]?.ToString() + " AND ANNDEN = " + idocDatiE1Pitype.Rows[index][nameof (ANNDEN)]?.ToString() + " AND PRODEN = " + idocDatiE1Pitype.Rows[index][nameof (PRODEN)]?.ToString() + " AND PROMOD = ( SELECT MAX(PROMOD) FROM MODPREDET" + " WHERE CODPOS = " + idocDatiE1Pitype.Rows[index][nameof (CODPOS)]?.ToString() + " AND MAT = " + idocDatiE1Pitype.Rows[index][nameof (MAT)]?.ToString() + " AND PRORAP = " + idocDatiE1Pitype.Rows[index][nameof (PRORAP)]?.ToString() + " AND ANNDEN = " + idocDatiE1Pitype.Rows[index][nameof (ANNDEN)]?.ToString() + " AND PRODEN = " + idocDatiE1Pitype.Rows[index][nameof (PRODEN)]?.ToString() + " )";
              int int32 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL27, CommandType.Text));
              if (int32 > 0)
              {
                string strSQL28 = " SELECT VALUE(COUNT(*),0) AS CONT FROM MODPRE WHERE" + " CODPOS = " + idocDatiE1Pitype.Rows[index][nameof (CODPOS)]?.ToString() + " AND MAT = " + idocDatiE1Pitype.Rows[index][nameof (MAT)]?.ToString() + " AND PRORAP = " + idocDatiE1Pitype.Rows[index][nameof (PRORAP)]?.ToString() + " AND PROMOD = " + int32.ToString() + " AND CODSTAPRE = 4 AND DATANN IS NULL";
                dataTable.Clear();
                dataTable = objDataAccess.GetDataTable(strSQL28);
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
            Decimal num5 = !DBNull.Value.Equals(idocDatiE1Pitype.Rows[index]["IMPRET"]) ? Convert.ToDecimal(idocDatiE1Pitype.Rows[index]["IMPRET"]) : 0M;
            Decimal num6 = !DBNull.Value.Equals(idocDatiE1Pitype.Rows[index]["IMPFIG"]) ? Convert.ToDecimal(idocDatiE1Pitype.Rows[index]["IMPFIG"]) : 0M;
            Decimal num7 = !DBNull.Value.Equals(idocDatiE1Pitype.Rows[index]["IMPOCC"]) ? Convert.ToDecimal(idocDatiE1Pitype.Rows[index]["IMPOCC"]) : 0M;
            idocDatiE1Pitype.Rows[index]["IMPUTILE"] = (object) (num5 + num6 - num7);
          }
          break;
        case "9005":
          string strSQL29;
          if (FASE == "PREV")
            strSQL29 = " SELECT ANNCOM, CODPOS, MAT, PRORAP, DAL, AL, DATBEGDA, DATENDDA, STATUSPREV, " + " PLVAR, RAGSOC, SUBTY, FLGCANC, " + " MAX(ULTAGG) AS ULTAGG, SUM(IMPRET) AS IMPRET, SUM(IMPOCC) AS IMPOCC, 0.0 AS IMPUTILE " + " FROM " + " ( " + " SELECT MODPREDET.ANNCOM, TRIM(CHAR(MODPREDET.CODPOS)) AS CODPOS, " + " MODPREDET.MAT, MODPREDET.PRORAP, " + " MODPREDET.ANNCOM || '-01-01' AS DAL, " + " MODPREDET.ANNCOM || '-12-31' AS AL, " + " '1900-01-01' AS DATBEGDA, '9999-12-31' AS DATENDDA, 'C' AS STATUSPREV, " + " '" + PLVAR + "' AS PLVAR, " + " (SELECT RAGSOC FROM AZI WHERE CODPOS=MODPREDET.CODPOS) AS RAGSOC, " + "  '' AS SUBTY, '" + FLGCANC + "' AS FLGCANC, MODPREDET.ULTAGG, " + " CASE VALUE(CHAR(MODPREDET.IMPRETPRV), '') WHEN '' THEN MODPREDET.IMPRET ELSE MODPREDET.IMPRETPRV END AS IMPRET," + " CASE VALUE(CHAR(MODPREDET.IMPOCCPRV), '') WHEN '' THEN MODPREDET.IMPOCC ELSE MODPREDET.IMPOCCPRV  END AS IMPOCC" + " FROM MODPREDET " + " WHERE MODPREDET.TIPMOV = 'AR' " + " AND MODPREDET.CODPOS=" + CODPOS.ToString() + " AND MODPREDET.MAT = " + MAT.ToString() + " AND MODPREDET.PRORAP = " + PRORAP.ToString() + " AND MODPREDET.PROMOD = " + PROMOD.ToString() + " ) AS TABELLA " + " GROUP BY " + " ANNCOM, CODPOS, MAT, PRORAP, DAL, AL, DATBEGDA, DATENDDA, STATUSPREV, " + " PLVAR, RAGSOC, SUBTY, FLGCANC " + " ORDER BY MAT, DAL";
          else if (FASE == "ANTE2003")
            strSQL29 = " SELECT ANNDEN AS ANNCOM, CODPOS, MAT, PRORAP, DAL, AL, DATBEGDA, DATENDDA, STATUSPREV, " + " PLVAR, RAGSOC, SUBTY, FLGCANC, " + " MAX(ULTAGG) AS ULTAGG, SUM(IMPRET) AS IMPRET, SUM(IMPOCC) AS IMPOCC, 0.0 AS IMPUTILE " + " FROM " + " ( " + " SELECT DENDET.ANNDEN, TRIM(CHAR(DENDET.CODPOS)) AS CODPOS, " + " DENDET.MAT, DENDET.PRORAP, " + " DENDET.ANNDEN || '-01-01' AS DAL, " + " DENDET.ANNDEN || '-12-31' AS AL, " + " '1900-01-01' AS DATBEGDA, '9999-12-31' AS DATENDDA, '' AS STATUSPREV, " + " '" + PLVAR + "' AS PLVAR, " + " (SELECT RAGSOC FROM AZI WHERE CODPOS=DENDET.CODPOS) AS RAGSOC, " + "  '' AS SUBTY, '" + FLGCANC + "' AS FLGCANC, DENDET.ULTAGG, " + " CASE VALUE(DENDET.NUMMOVANN, '') WHEN '' THEN DENDET.IMPRET ELSE 0 END AS IMPRET, " + " CASE VALUE(DENDET.NUMMOVANN, '') WHEN '' THEN DENDET.IMPOCC ELSE 0 END AS IMPOCC " + " FROM DENDET, DENTES " + " WHERE " + " DENTES.CODPOS = DENDET.CODPOS " + " AND DENTES.ANNDEN = DENDET.ANNDEN " + " AND DENTES.MESDEN = DENDET.MESDEN " + " AND DENTES.PRODEN = DENDET.PRODEN " + " AND VALUE(DENDET.ESIRET, '') <> 'S' " + " AND DENDET.CODPOS=" + CODPOS.ToString() + " AND DENDET.ANNDEN= " + ANNDEN.ToString() + " AND DENDET.MESDEN= " + MESDEN.ToString() + " AND DENDET.PRODEN = " + PRODEN.ToString() + " AND DENDET.MAT= " + MAT.ToString() + " AND DENDET.PRODENDET = " + PRODENDET.ToString() + " AND DENDET.PRORAP = " + PRORAP.ToString() + " ) AS TABELLA " + " GROUP BY " + " ANNDEN, CODPOS, MAT, PRORAP, DAL, AL, DATBEGDA, DATENDDA, STATUSPREV, " + " PLVAR, RAGSOC, SUBTY, FLGCANC " + " ORDER BY MAT, DAL";
          else if (FASE == "ANTE2003_ANN")
          {
            strSQL29 = " SELECT ANNDEN AS ANNCOM, CODPOS, MAT, PRORAP, DAL, AL, DATBEGDA, DATENDDA, STATUSPREV, " + " PLVAR, RAGSOC, SUBTY, FLGCANC, " + " MAX(ULTAGG) AS ULTAGG, SUM(IMPRET) AS IMPRET, SUM(IMPOCC) AS IMPOCC, 0.0 AS IMPUTILE " + " FROM " + " ( " + " SELECT DENDET.ANNDEN, TRIM(CHAR(DENDET.CODPOS)) AS CODPOS, " + " DENDET.MAT, DENDET.PRORAP, " + " DENDET.ANNDEN || '-01-01' AS DAL, " + " DENDET.ANNDEN || '-12-31' AS AL, " + " '1900-01-01' AS DATBEGDA, '9999-12-31' AS DATENDDA, '' AS STATUSPREV, " + " '" + PLVAR + "' AS PLVAR, " + " (SELECT RAGSOC FROM AZI WHERE CODPOS=DENDET.CODPOS) AS RAGSOC, " + "  '' AS SUBTY, '" + FLGCANC + "' AS FLGCANC, DENDET.ULTAGG, " + " 0 AS IMPRET, " + " 0 AS IMPOCC " + " FROM DENDET, DENTES " + " WHERE " + " DENTES.CODPOS = DENDET.CODPOS " + " AND DENTES.ANNDEN = DENDET.ANNDEN " + " AND DENTES.MESDEN = DENDET.MESDEN " + " AND DENTES.PRODEN = DENDET.PRODEN " + " AND VALUE(DENDET.ESIRET, '') <> 'S' " + " AND DENDET.CODPOS=" + CODPOS.ToString() + " AND DENDET.ANNDEN= " + ANNDEN.ToString() + " AND DENDET.MESDEN= " + MESDEN.ToString() + " AND DENDET.PRODEN = " + PRODEN.ToString() + " AND DENDET.MAT= " + MAT.ToString() + " AND DENDET.PRODENDET = " + PRODENDET.ToString() + " AND DENDET.PRORAP = " + PRORAP.ToString() + " ) AS TABELLA " + " GROUP BY " + " ANNDEN, CODPOS, MAT, PRORAP, DAL, AL, DATBEGDA, DATENDDA, STATUSPREV, " + " PLVAR, RAGSOC, SUBTY, FLGCANC " + " ORDER BY MAT, DAL";
          }
          else
          {
            string str16 = " SELECT ANNCOM, CODPOS, MAT, PRORAP, DAL, AL, DATBEGDA, DATENDDA, STATUSPREV, " + " PLVAR, RAGSOC, SUBTY, FLGCANC, " + " MAX(ULTAGG) AS ULTAGG, SUM(IMPRET) AS IMPRET, SUM(IMPOCC) AS IMPOCC, 0.0 AS IMPUTILE " + " FROM " + " ( " + " SELECT DENDET.ANNCOM, TRIM(CHAR(DENDET.CODPOS)) AS CODPOS, " + " DENDET.MAT, DENDET.PRORAP, " + " DENDET.ANNCOM || '-01-01' AS DAL, " + " DENDET.ANNCOM || '-12-31' AS AL, " + " '1900-01-01' AS DATBEGDA, '9999-12-31' AS DATENDDA, '' AS STATUSPREV, " + " '" + PLVAR + "' AS PLVAR, " + " (SELECT RAGSOC FROM AZI WHERE CODPOS=DENDET.CODPOS) AS RAGSOC, " + "  '' AS SUBTY, '" + FLGCANC + "' AS FLGCANC, DENDET.ULTAGG, " + " CASE VALUE(DENDET.NUMMOVANN, '') WHEN '' THEN DENDET.IMPRET ELSE 0 END AS IMPRET, " + " CASE VALUE(DENDET.NUMMOVANN, '') WHEN '' THEN DENDET.IMPOCC ELSE 0 END AS IMPOCC " + " FROM DENDET, DENTES " + " WHERE " + " DENTES.CODPOS = DENDET.CODPOS " + " AND DENTES.ANNDEN = DENDET.ANNDEN " + " AND DENTES.MESDEN = DENDET.MESDEN " + " AND DENTES.PRODEN = DENDET.PRODEN " + " AND DENTES.TIPMOV = 'AR' " + " AND VALUE(DENDET.ESIRET, '') <> 'S' " + " AND DENTES.NUMMOV IS NOT NULL ";
            strSQL29 = (!(TIPMOV.Trim() == "RT") ? (PRODENDET == 0 ? str16 + " AND DENDET.CODPOS=" + CODPOS.ToString() + " AND DENDET.MAT IN (SELECT DISTINCT MAT " + " FROM DENDET WHERE CODPOS= " + CODPOS.ToString() + " AND ANNDEN= " + ANNDEN.ToString() + " AND MESDEN= " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + ") " + " AND DENDET.ANNCOM IN (SELECT DISTINCT ANNCOM " + " FROM DENDET WHERE CODPOS= " + CODPOS.ToString() + " AND ANNDEN= " + ANNDEN.ToString() + " AND MESDEN= " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + ") " : str16 + " AND DENDET.CODPOS=" + CODPOS.ToString() + " AND DENDET.ANNDEN= " + ANNDEN.ToString() + " AND DENDET.MESDEN= " + MESDEN.ToString() + " AND DENDET.PRODEN = " + PRODEN.ToString() + " AND DENDET.MAT= " + MAT.ToString() + " AND DENDET.PRODENDET = " + PRODENDET.ToString() + " AND DENDET.PRORAP = " + PRORAP.ToString() + " AND DENDET.TIPMOV = 'AR'") : str16 + " AND DENDET.ANNCOM IN ( SELECT DISTINCT ANNCOM " + " FROM DENDET WHERE DENDET.CODPOS=" + CODPOS.ToString() + " AND DENDET.ANNDEN= " + ANNDEN.ToString() + " AND DENDET.MESDEN= " + MESDEN.ToString() + " AND DENDET.PRODEN = " + PRODEN.ToString() + " AND DENDET.MAT= " + MAT.ToString() + " AND DENDET.PRODENDET = " + PRODENDET.ToString() + " )" + " AND DENDET.MAT IN (SELECT DISTINCT MAT " + " FROM DENDET WHERE DENDET.CODPOS=" + CODPOS.ToString() + " AND DENDET.ANNDEN= " + ANNDEN.ToString() + " AND DENDET.MESDEN= " + MESDEN.ToString() + " AND DENDET.PRODEN = " + PRODEN.ToString() + " AND DENDET.MAT= " + MAT.ToString() + " AND DENDET.PRODENDET = " + PRODENDET.ToString() + " )" + " AND DENDET.CODPOS=" + CODPOS.ToString() + " AND DENDET.ANNDEN= " + ANNDEN.ToString() + " AND DENDET.MESDEN= " + MESDEN.ToString() + " AND DENDET.PRODEN = " + PRODEN.ToString() + " AND DENDET.MAT= " + MAT.ToString() + " AND DENDET.PRODENDET = " + PRODENDET.ToString()) + " ) AS TABELLA " + " GROUP BY " + " ANNCOM, CODPOS, MAT, PRORAP, DAL, AL, DATBEGDA, DATENDDA, STATUSPREV, " + " PLVAR, RAGSOC, SUBTY, FLGCANC " + " ORDER BY MAT, DAL";
          }
          idocDatiE1Pitype = objDataAccess.GetDataTable(strSQL29);
          for (int index = 0; index <= idocDatiE1Pitype.Rows.Count - 1; ++index)
          {
            if (idocDatiE1Pitype.Rows[index]["RAGSOC"].ToString().Trim().Length > 29)
              idocDatiE1Pitype.Rows[index]["RAGSOC"] = (object) idocDatiE1Pitype.Rows[index]["RAGSOC"].ToString().Trim().Substring(0, 29);
            idocDatiE1Pitype.Rows[index][nameof (CODPOS)] = (object) ("A" + idocDatiE1Pitype.Rows[index][nameof (CODPOS)].ToString().Trim().PadLeft(6, '0'));
            idocDatiE1Pitype.Rows[index]["IMPUTILE"] = (object) (Convert.ToDecimal(idocDatiE1Pitype.Rows[index]["IMPRET"]) - Convert.ToDecimal(idocDatiE1Pitype.Rows[index]["IMPOCC"]));
            string strSQL30 = "SELECT VALUE(CHAR(DATDEC), '') AS DATDEC, VALUE(CHAR(DATCES), '') AS DATCES" + " FROM RAPLAV WHERE CODPOS = " + CODPOS.ToString() + " AND MAT = " + idocDatiE1Pitype.Rows[index][nameof (MAT)]?.ToString() + " AND PRORAP = " + idocDatiE1Pitype.Rows[index][nameof (PRORAP)]?.ToString();
            dataTable.Clear();
            dataTable = objDataAccess.GetDataTable(strSQL30);
            if (dataTable.Rows.Count > 0)
            {
              if (Convert.ToDateTime(dataTable.Rows[0][nameof (DATDEC)].ToString()).Year.ToString() == idocDatiE1Pitype.Rows[index]["ANNCOM"].ToString())
                idocDatiE1Pitype.Rows[index]["DAL"] = (object) DBMethods.Db2Date(dataTable.Rows[0][nameof (DATDEC)].ToString());
              if (dataTable.Rows[0]["DATCES"].ToString().Trim() != "")
                idocDatiE1Pitype.Rows[index]["AL"] = (object) DBMethods.Db2Date(dataTable.Rows[0]["DATCES"].ToString());
            }
          }
          break;
        case "9101":
          string strSQL31 = " SELECT TRIM(CHAR(CODPOS)) AS CODPOS, MAT, PRORAP, DATINIPRE, INDDENDIP, DATSCAPRE, IMPINDSOS, GIOINDSOS, " + " '" + PLVAR + "' AS PLVAR, '' AS SUBTY, " + " '1900-01-01' AS DATBEGDA, " + " '9999-12-31' AS DATENDDA, ULTAGG, " + " '" + FLGCANC + "' AS FLGCANC " + " FROM MODPRE " + " WHERE CODPOS=" + CODPOS.ToString() + " AND MAT=" + MAT.ToString() + " AND PRORAP=" + PRORAP.ToString() + " AND PROMOD=" + PROMOD.ToString();
          idocDatiE1Pitype = objDataAccess.GetDataTable(strSQL31);
          for (int index = 0; index <= idocDatiE1Pitype.Rows.Count - 1; ++index)
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
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL);
      for (int index = 0; index <= DT.Rows.Count - 1; ++index)
      {
        DataRow row = DT.Rows[index];
        this.WRITE_EDID4(objDataAccess, 3, dataTable2.Rows[0]["TABSAP"].ToString().Trim(), VAR_E1PITYP, ref row);
        if (!CANCELLAZIONE)
        {
          this.WRITE_EDID4(objDataAccess, Convert.ToInt32(dataTable2.Rows[0]["IDTAB"]), dataTable2.Rows[0]["TABSAP"].ToString(), VAR_E1PITYP, ref row);
        }
        else
        {
          string str = VAR_E1PITYP.ToUpper().Trim();
          if (str == "0016" || str == "9001" || str == "9003" || str == "9004")
            this.WRITE_EDID4(objDataAccess, Convert.ToInt32(dataTable2.Rows[0]["IDTAB"]), dataTable2.Rows[0]["TABSAP"].ToString(), VAR_E1PITYP, ref row);
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
      DataTable dataTable4 = objDataAccess.GetDataTable(strSQL1);
      for (int index1 = 0; index1 <= dataTable4.Rows.Count - 1; ++index1)
      {
        string str3 = "";
        string str4 = "";
        string strSQL2 = "SELECT * FROM IDOCCAMPI WHERE IDTAB = " + dataTable4.Rows[index1]["IDTAB"]?.ToString() + " ORDER BY ORDINE";
        dataTable2.Clear();
        dataTable2 = objDataAccess.GetDataTable(strSQL2);
        this.SEGNUM = this.Module_GetSEGNUM(objDataAccess, this.IDOC_CONT);
        for (int index2 = 0; index2 <= dataTable2.Rows.Count - 1; ++index2)
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
      string strSQL1 = " SELECT VALUE(CHAR(CURRENT_TIMESTAMP),'') FROM MENU";
      string str = objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text);
      int num = random.Next();
      string strSQL2 = "INSERT INTO COUNT_IDOC(ULTAGG, RND)" + " VALUES ('" + str + "', " + num.ToString() + ") ";
      objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
      string strSQL3 = " SELECT CONT FROM COUNT_IDOC WHERE ULTAGG = '" + str + "'" + " AND RND = " + num.ToString();
      return objDataAccess.Get1ValueFromSQL(strSQL3, CommandType.Text);
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
      string strSQL1 = "SELECT PADRE, HLEVEL FROM IDOCTAB WHERE IDTAB = " + IDTAB.ToString();
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL1);
      string str = dataTable2.Rows[0]["PADRE"].ToString();
      HLEVEL = dataTable2.Rows[0][nameof (HLEVEL)].ToString();
      string strSQL2 = "SELECT SEGNUM FROM EDID4 WHERE CONT_NUM = " + this.IDOC_CONT.ToString() + " AND MANDT = " + MANDT + " AND DOCNUM = " + DOCNUM + " AND SEGNAM = " + DBMethods.DoublePeakForSql(str);
      PSGNUM = objDataAccess.Get1ValueFromSQL(strSQL2, CommandType.Text);
    }

    private string Module_GetSEGNUM(DataLayer objDataAccess, int CONT)
    {
      DataTable dataTable = new DataTable();
      string strSQL = "SELECT VALUE(MAX(SEGNUM),0) + 1 AS SEGNUM FROM EDID4 WHERE CONT_NUM = " + CONT.ToString();
      return objDataAccess.Get1ValueFromSQL(strSQL, CommandType.Text);
    }

    private string GetStrValues(
      DataLayer objDataAccess,
      int IDCAMPI,
      ref DataRow ROW_DATI,
      string INFTYP)
    {
      DataTable dataTable1 = new DataTable();
      string str1 = "";
      string strSQL = "SELECT CAMPOSAP, TIPO, LUNGHEZZA, VALORE, VALDEFAULT, VALDTDATI, FORMATO FROM IDOCVALORI WHERE IDCAMPI = " + IDCAMPI.ToString() + " ORDER BY ORDINE";
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL);
      for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
      {
        string str2 = !(dataTable2.Rows[index]["CAMPOSAP"].ToString().Trim().ToUpper() == "INFTY") ? (!(dataTable2.Rows[index]["VALORE"].ToString().Trim().ToUpper() == "DTDATI") ? dataTable2.Rows[index]["VALDEFAULT"].ToString().Trim() : (!(dataTable2.Rows[index]["VALDTDATI"].ToString().Trim() == "") ? dataTable2.Rows[index]["VALDTDATI"].ToString().Trim() : dataTable2.Rows[index]["VALDEFAULT"].ToString().Trim())) : INFTYP;
        str1 += this.GetFormato(dataTable2.Rows[index]["FORMATO"].ToString(), str2.ToString(), Convert.ToInt32(dataTable2.Rows[index]["LUNGHEZZA"]), false);
      }
      return "'" + str1.Replace("'", "''") + "'";
    }

    private string GetFormato(string FORMATO, string TESTO, int LUNGHEZZA, bool USE_DOUBLEPEAK = true)
    {
      TESTO = TESTO.Trim();
      switch (FORMATO.Trim().ToUpper())
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
          if (TESTO != "" && TESTO != "000000")
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
          if (TESTO != "" & TESTO != "99991231" & TESTO != "18000101" && TESTO != "00000000")
          {
            TESTO = DBMethods.Db2Date(TESTO);
            TESTO = TESTO.Replace("-", "");
          }
          return TESTO.PadRight(LUNGHEZZA, ' ');
        default:
          return !USE_DOUBLEPEAK ? TESTO.ToString().Trim().PadRight(LUNGHEZZA, ' ') : DBMethods.DoublePeakForSql(TESTO.ToString().Trim().PadRight(LUNGHEZZA, ' '));
      }
    }

    public void Aggiorna_IDOC(DataLayer objDataAccess)
    {
      if (this.objDtCONTIDOC == null)
        return;
      for (short index = 0; (int) index <= this.objDtCONTIDOC.Rows.Count - 1; ++index)
      {
        string strSQL = "UPDATE EDIDC SET STATUS = 64 WHERE STATUS = 00 AND CONT_NUM = " + this.objDtCONTIDOC.Rows[(int) index]["CONTIDOC"]?.ToString();
        objDataAccess.WriteTransactionData(strSQL, CommandType.Text);
      }
    }

    public void Module_AggiornaStordl(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int CODPOS,
      int MAT,
      int PRORAP,
      string DATINI = "")
    {
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      string strData1 = "";
      string strData2 = "";
      string strData3 = "";
      string str1 = "";
      string str2 = "";
      string str3 = "";
      Decimal num1 = 0.0M;
      Decimal num2 = 0.0M;
      Decimal num3 = 0.0M;
      string str4 = "";
      int num4 = 0;
      string str5 = "";
      string str6 = "";
      DataTable dataTable3 = new DataTable();
      int num5 = 0;
      string strDataNascita = objDataAccess.Get1ValueFromSQL("SELECT DATNAS FROM ISCT WHERE MAT = " + MAT.ToString(), CommandType.Text);
      string str7 = "SELECT * FROM STORDL WHERE" + " CODPOS = '" + CODPOS.ToString() + "' " + " AND MAT = '" + MAT.ToString() + "' AND PRORAP = '" + PRORAP.ToString() + "' ";
      string strSQL1 = string.IsNullOrEmpty(DATINI) ? str7 + " ORDER BY DATINI ASC " : str7 + " AND DATINI = '" + DBMethods.Db2Date(DATINI.Replace("'", "")) + "' ";
      DataTable dataTable4 = objDataAccess.GetDataTable(strSQL1);
      for (int index1 = 0; index1 <= dataTable4.Rows.Count - 1; ++index1)
      {
        if (!string.IsNullOrEmpty(DATINI))
        {
          string strSQL2 = "SELECT DTTFR, DTFP, DTINF FROM STORDL WHERE" + " CODPOS = '" + CODPOS.ToString() + "' " + " AND MAT = '" + MAT.ToString() + "' AND PRORAP = '" + PRORAP.ToString() + "' " + " AND DATINI < '" + DBMethods.Db2Date(DATINI.Replace("'", "")) + "' " + " ORDER BY DATINI DESC";
          dataTable2.Clear();
          dataTable2 = objDataAccess.GetDataTable(strSQL2);
          if (dataTable2.Rows.Count > 0)
          {
            str1 = string.IsNullOrEmpty(dataTable2.Rows[0]["DTTFR"].ToString()) ? "" : dataTable2.Rows[0]["DTTFR"].ToString();
            str2 = string.IsNullOrEmpty(dataTable2.Rows[0]["DTINF"].ToString()) ? "" : dataTable2.Rows[0]["DTINF"].ToString();
            str3 = string.IsNullOrEmpty(dataTable2.Rows[0]["DTFP"].ToString()) ? "" : dataTable2.Rows[0]["DTFP"].ToString();
          }
        }
        int num6 = Convert.ToInt32(dataTable4.Rows[index1]["CODCON"]);
        int num7 = !string.IsNullOrEmpty(dataTable4.Rows[index1]["CODLOC"].ToString()) ? Convert.ToInt32(dataTable4.Rows[index1]["CODLOC"]) : 0;
        Decimal num8 = Convert.ToDecimal(dataTable4.Rows[index1]["CODGRUASS"]);
        string str8 = dataTable4.Rows[index1]["FAP"].ToString().Trim();
        DataTable dataTable5;
        int num9;
        int num10;
        if (num7 == 0)
        {
          string strSQL3 = "SELECT CODQUACON, PROCON FROM CONRIF " + " WHERE CODCON = " + num6.ToString() + " AND DATDEC <= '" + DBMethods.Db2Date(dataTable4.Rows[index1][nameof (DATINI)].ToString()) + "' " + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
          dataTable2.Clear();
          dataTable5 = objDataAccess.GetDataTable(strSQL3);
          if (dataTable5.Rows.Count > 0)
          {
            num9 = Convert.ToInt32(dataTable5.Rows[0]["CODQUACON"]);
            num10 = Convert.ToInt32(dataTable5.Rows[0]["PROCON"]);
          }
          else
          {
            num9 = 0;
            num10 = 0;
          }
        }
        else
        {
          string strSQL4 = "SELECT PROCON FROM CONRIF WHERE CODCON = " + num6.ToString() + " AND DATDEC <= '" + DBMethods.Db2Date(dataTable4.Rows[index1][nameof (DATINI)].ToString()) + "' " + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
          dataTable2.Clear();
          dataTable5 = objDataAccess.GetDataTable(strSQL4);
          num10 = dataTable5.Rows.Count <= 0 ? 0 : Convert.ToInt32(dataTable5.Rows[0]["PROCON"]);
          string strSQL5 = "SELECT CODQUACON FROM QUACON WHERE CODQUACON IN (SELECT CODQUACON FROM CONRIF WHERE CODCON = " + num6.ToString() + ")";
          num9 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL5, CommandType.Text));
        }
        string str9 = "SELECT VALUE(SUM(ALIQUOTA), 0.00) AS ALIQUOTA " + " FROM ALIFORASS " + " WHERE ALIFORASS.CODGRUASS = " + dataTable4.Rows[index1]["CODGRUASS"]?.ToString() + " AND ALIFORASS.CODQUACON=" + num9.ToString() + " AND '" + DBMethods.Db2Date(dataTable4.Rows[index1][nameof (DATINI)].ToString()) + "' BETWEEN ALIFORASS.DATINI AND VALUE(ALIFORASS.DATFIN,'9999-12-31') ";
        if (this.Utils.Module_Check_65Anni(Convert.ToDateTime(dataTable4.Rows[index1][nameof (DATINI)]), strDataNascita))
          str9 += " AND ALIFORASS.CODFORASS IN (SELECT CODFORASS FROM FORASS WHERE CATFORASS <> 'PREV') ";
        string strSQL6 = str9 + " AND ALIFORASS.CODFORASS NOT IN (SELECT CODFORASS FROM FORASS WHERE CATFORASS = 'FAP') ";
        dataTable3.Clear();
        dataTable3 = objDataAccess.GetDataTable(strSQL6);
        dataTable4.Rows[index1]["ALIQUOTA"] = dataTable3.Rows[0]["ALIQUOTA"];
        Decimal num11 = (Decimal) dataTable4.Rows[index1]["ALIQUOTA"];
        Decimal num12;
        if (str8 == "S")
        {
          string strSQL7 = "SELECT VALFAP FROM CODFAP WHERE " + DBMethods.Db2Date(dataTable4.Rows[index1][nameof (DATINI)].ToString()) + " BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31')";
          num12 = Convert.ToDecimal(objDataAccess.Get1ValueFromSQL(strSQL7, CommandType.Text));
        }
        else
          num12 = 0M;
        string strSQL8 = "SELECT DENLIV FROM CONLIV WHERE CODCON = " + num6.ToString() + " AND PROCON = " + num10.ToString() + " AND CODLIV = " + dataTable4.Rows[index1]["CODLIV"]?.ToString();
        string str10 = objDataAccess.Get1ValueFromSQL(strSQL8, CommandType.Text);
        string strSQL9 = " SELECT * FROM (SELECT (SELECT CATFORASS FROM FORASS WHERE CODFORASS = A.CODFORASS) AS CAT, A.ALIQUOTA " + " FROM ALIFORASS A WHERE A.CODGRUASS =  " + num8.ToString() + " AND A.CODQUACON = " + num9.ToString() + " ) AS TAB WHERE TAB.CAT <> 'FAP'";
        dataTable5.Clear();
        dataTable2 = objDataAccess.GetDataTable(strSQL9);
        for (int index2 = 0; index2 <= dataTable2.Rows.Count - 1; ++index2)
        {
          string str11 = dataTable2.Rows[index2]["CAT"].ToString().Trim();
          if (!(str11 == "TFR"))
          {
            if (!(str11 == "PREV"))
            {
              if (!(str11 == "INF"))
                throw new Exception("Caso non gestito " + dataTable2.Rows[index2]["CAT"]?.ToString());
              num2 = (Decimal) dataTable2.Rows[index2]["ALIQUOTA"];
            }
            else
              num3 = (Decimal) dataTable2.Rows[index2]["ALIQUOTA"];
          }
          else
            num1 = (Decimal) dataTable2.Rows[index2]["ALIQUOTA"];
        }
        for (int index3 = 0; index3 <= 2; ++index3)
        {
          switch (index3)
          {
            case 0:
              str6 = "TFR";
              break;
            case 1:
              str6 = "PREV";
              break;
            case 2:
              str6 = "INF";
              break;
          }
          string strSQL10 = " SELECT COUNT(*) FROM ALIFORASS A, FORASS B WHERE A.CODGRUASS =  " + num8.ToString() + " AND A.CODQUACON = " + num9.ToString() + " AND A.CODFORASS = B.CODFORASS AND B.CATFORASS = " + DBMethods.DoublePeakForSql(str6);
          if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL10, CommandType.Text)) > 0)
          {
            switch (index3)
            {
              case 0:
                strData1 = !((Decimal) num5 == num8) ? (!(str1 == "") ? str1 : dataTable4.Rows[index1][nameof (DATINI)].ToString()) : (!(str2 == "") ? str1 : dataTable4.Rows[index1][nameof (DATINI)].ToString());
                continue;
              case 1:
                strData3 = !((Decimal) num5 == num8) ? (!(str3 == "") ? str3 : dataTable4.Rows[index1][nameof (DATINI)].ToString()) : (!(str3 == "") ? str2 : dataTable4.Rows[index1][nameof (DATINI)].ToString());
                continue;
              case 2:
                strData2 = !((Decimal) num5 == num8) ? (!(str2 == "") ? str2 : dataTable4.Rows[index1][nameof (DATINI)].ToString()) : (!(str2 == "") ? str2 : dataTable4.Rows[index1][nameof (DATINI)].ToString());
                continue;
              default:
                continue;
            }
          }
        }
        string str12 = " UPDATE STORDL SET ";
        string str13 = !(strData1.Trim() != "") ? str12 + " DTTFR = NULL, " + " ALIQTFR = NULL, " : str12 + " DTTFR  = '" + DBMethods.Db2Date(strData1) + "', " + " ALIQTFR  = " + num1.ToString().Replace(",", ".") + ", ";
        string str14 = !(strData2.Trim() != "") ? str13 + " DTINF  = NULL, " + " ALIQINF  = NULL, " : str13 + " DTINF  = '" + DBMethods.Db2Date(strData2) + "', " + " ALIQINF  = " + num2.ToString().Replace(",", ".") + ", ";
        string str15 = !(strData3.Trim() != "") ? str14 + " DTFP  = NULL, " + " ALIQFP  = NULL, " : str14 + " DTFP  = '" + DBMethods.Db2Date(strData3) + "', " + " ALIQFP  = " + num3.ToString().Replace(",", ".") + ", ";
        string str16 = (!(str8 == "S") ? str15 + " ALIFAP = NULL, " : str15 + " ALIFAP  = " + num12.ToString().Replace(",", ".") + ", ") + " ALIQUOTA  = " + num11.ToString().Replace(",", ".") + ", " + " CODQUACON  = " + num9.ToString() + ", ";
        string strSQL11 = (!string.IsNullOrEmpty(str10) ? str16 + " DENLIV  = " + DBMethods.DoublePeakForSql(str10.Trim()) : str16 + " DENLIV  = NULL ") + " WHERE CODPOS = " + CODPOS.ToString() + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString() + " AND DATINI = '" + DBMethods.Db2Date(dataTable4.Rows[index1][nameof (DATINI)].ToString()) + "' ";
        objDataAccess.WriteTransactionData(strSQL11, CommandType.Text);
        num5 = Convert.ToInt32(num8);
        str1 = strData1;
        str2 = strData2;
        str3 = strData3;
        strData1 = "";
        strData2 = "";
        strData3 = "";
        num1 = 0.0M;
        num2 = 0.0M;
        num3 = 0.0M;
        str4 = "";
        num12 = 0.0M;
        num8 = 0.0M;
        num11 = 0.0M;
        num9 = 0;
        num6 = 0;
        num4 = 0;
        str5 = "";
        str6 = "";
        num10 = 0;
      }
    }

    public void AGGIORNA_RAPLAV_INPS(
      DataLayer objDataAccess,
      TFI.OCM.Utente.Utente u,
      int CODPOS,
      int MAT,
      int PRORAP)
    {
      if (CODPOS == 0)
      {
        string strSQL = "UPDATE RAPLAV SET DATINPS = NULL, TIPOPE = 'M', " + " ULTAGG = CURRENT_TIMESTAMP, UTEAGG = " + DBMethods.DoublePeakForSql(u.Username) + " WHERE MAT=" + MAT.ToString() + " AND CURRENT_DATE BETWEEN DATDEC AND VALUE(DATCES,'9999-12-31')";
        objDataAccess.WriteTransactionData(strSQL, CommandType.Text);
      }
      else
      {
        string strSQL = "UPDATE RAPLAV SET DATINPS = NULL, TIPOPE = 'M', " + " ULTAGG = CURRENT_TIMESTAMP, UTEAGG = " + DBMethods.DoublePeakForSql(u.Username) + " WHERE CODPOS = " + CODPOS.ToString() + " AND MAT=" + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString();
        objDataAccess.WriteTransactionData(strSQL, CommandType.Text);
      }
    }
  }
}
