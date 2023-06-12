// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Amministrativo.modIDOC_Aziende
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using System;
using System.Data;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;

namespace TFI.DAL.Amministrativo
{
  public class modIDOC_Aziende
  {
    private DataLayer objdataccess = new DataLayer();
    private DataTable objDtCONTIDOC_AZI = new DataTable();
    private int IDOC_CONT_AZI;
    private string SEGNUM = "";

    public void WRITE_EDID4_AZI(
      ref iDB2Connection Conn,
      ref iDB2Command cmd,
      ref iDB2DataAdapter da,
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
      DataTable dataTable2 = this.objdataccess.GetDataTable(strSQL);
      this.SEGNUM = this.Module_GetSEGNUM_AZI(ref Conn, ref cmd, ref da, this.IDOC_CONT_AZI);
      int num = dataTable2.Rows.Count - 1;
      for (int index = 0; index <= num; ++index)
      {
        str2 = Convert.ToString(str2 + (", ", dataTable2.Rows[index]["CAMPO"]).ToString());
        string str4 = dataTable2.Rows[index]["CAMPO"].ToString().Trim().ToUpper() ?? "";
        if (!(str4 == "MANDT"))
        {
          if (str4 == "DOCNUM")
            DOCNUM = this.GetFormato_AZI(dataTable2.Rows[index]["FORMATO"].ToString(), this.IDOC_CONT_AZI.ToString(), Convert.ToInt32(dataTable2.Rows[index]["LUNGHEZZA"]));
        }
        else
          MANDT = this.GetFormato_AZI(dataTable2.Rows[index]["FORMATO"].ToString(), dataTable2.Rows[index]["VALORE"].ToString(), Convert.ToInt32(dataTable2.Rows[index]["LUNGHEZZA"]));
        string str5 = dataTable2.Rows[index]["VALORE"].ToString().Trim().ToUpper() ?? "";
        if (!(str5 == "[CONTATORE]"))
        {
          str3 = str5 == "IDOCVALORI" ? str3 + ", " + this.GetStrValues_AZI(ref Conn, ref cmd, ref da, Convert.ToInt32(dataTable2.Rows[index]["IDCAMPI"]), ref ROW_DATI, VAR_E1PITYP) : str3 + ", " + this.GetFormato_AZI(dataTable2.Rows[index]["FORMATO"].ToString(), dataTable2.Rows[index]["VALORE"].ToString(), Convert.ToInt32(dataTable2.Rows[index]["LUNGHEZZA"]));
        }
        else
        {
          string str6 = dataTable2.Rows[index]["CAMPO"].ToString().Trim().ToUpper() ?? "";
          if (!(str6 == "SEGNUM"))
          {
            if (!(str6 == "CONT"))
            {
              if (!(str6 == "PSGNUM"))
              {
                if (!(str6 == "HLEVEL"))
                {
                  if (str6 == "DOCNUM")
                    str1 = this.IDOC_CONT_AZI.ToString();
                }
                else
                  str1 = HLEVEL;
              }
              else
              {
                this.Module_GetPSGNUM_HLEVEL_AZI(ref Conn, ref cmd, ref da, IDTAB, MANDT, DOCNUM, ref HLEVEL, ref PSGNUM);
                str1 = PSGNUM;
              }
            }
            else
              str1 = this.IDOC_CONT_AZI.ToString();
          }
          else
            str1 = this.SEGNUM;
          str3 = str3 + ", " + this.GetFormato_AZI(dataTable2.Rows[index]["FORMATO"].ToString(), str1.ToString(), Convert.ToInt32(dataTable2.Rows[index]["LUNGHEZZA"]));
        }
      }
      string str7 = str2 + ", CONT_NUM ";
      string str8 = str3 + ", " + this.IDOC_CONT_AZI.ToString() + " ";
      this.objdataccess.WriteTransactionData("INSERT INTO " + TABSAP + " (" + str7.Substring(2) + ")" + (" VALUES (" + str8.Substring(2) + ")"), CommandType.Text);
    }

    public void WRITE_IDOC_ANAG_IND_AZI(
      ref iDB2Connection Conn,
      ref iDB2Command cmd,
      ref iDB2DataAdapter da,
      int CODPOS,
      ref DataTable DT_SEDI_OLD)
    {
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataTable dataTable3 = new DataTable();
      DataTable dataTable4 = new DataTable();
      int num1 = 0;
      bool flag = false;
      string strSQL1 = " SELECT TRIM(CHAR(AZI.CODPOS)) AS CODPOS, INDSED.TIPIND, AZI.CODFIS, AZI.PARIVA, AZI.RAGSOC AS RAGSOCAPP,  '005' AS MSGFN, " + " '' AS RAGSOC, '' AS RAGSOC1, '' AS RAGSOC2, '' AS RAGSOCTRIM," + " '' AS IND2, INDSED.IND AS IND1, '' AS INDAPP, INDSED.DENSTAEST, INDSED.DENLOC," + " VALUE(CAP, '00000') AS CAP, INDSED.TEL1 AS TEL, INDSED.FAX, INDSED.SIGPRO, INDSED.EMAIL, INDSED.EMAILCERT, '' EMAILDEF, " + " 'SEDE' AS KTOKD, INDSED.NUMCIV," + " INDSED.CODCOM, '' AS DENCOM, " + " 'IT' AS CODSTATO," + " '' AS NATGIU, '' AS CODATTCAM, '' AS CODRAPLEG," + " DUG.DENDUG, CODREG.DENREG " + " FROM AZI INNER JOIN INDSED ON " + " INDSED.CODPOS = AZI.CODPOS LEFT JOIN DUG ON " + " INDSED.CODDUG = DUG.CODDUG LEFT JOIN SIGPRO ON " + " INDSED.SIGPRO = SIGPRO.SIGPRO LEFT JOIN CODREG ON " + " SIGPRO.CODREG = CODREG.CODREG " + " INNER JOIN " + " (SELECT TIPIND, MAX(DATINI) AS DATINI FROM INDSED WHERE CODPOS=" + CODPOS.ToString() + " GROUP BY TIPIND) AS IND " + " ON INDSED.TIPIND = IND.TIPIND AND INDSED.DATINI = IND.DATINI " + " WHERE AZI.CODPOS= " + CODPOS.ToString() + " ORDER BY INDSED.TIPIND ";
      DataTable dataTable5 = this.objdataccess.GetDataTable(strSQL1);
      int num2 = dataTable5.Rows.Count - 1;
      string str1;
      for (int index = 0; index <= num2; ++index)
      {
        if (!string.IsNullOrEmpty(dataTable5.Rows[index]["CODCOM"].ToString().Trim()))
        {
          string str2 = this.objdataccess.Get1ValueFromSQL("SELECT VALUE(DENCOM,'') AS DENCOM FROM CODCOM WHERE CODCOM = '" + DBMethods.DoublePeakForSql(dataTable5.Rows[index]["CODCOM"].ToString().Trim()) + "' ", CommandType.Text);
          dataTable5.Rows[index]["DENCOM"] = (object) str2.Trim();
        }
        if (!string.IsNullOrEmpty(dataTable5.Rows[index]["DENSTAEST"].ToString().Trim()))
        {
          string strSQL2 = "SELECT VALUE(CHAR(CODSTA),'') FROM CODCOM WHERE DENCOM = '" + DBMethods.DoublePeakForSql(dataTable5.Rows[index]["DENSTAEST"].ToString().Trim()) + "' ";
          dataTable5.Rows[index]["CODSTATO"] = (object) this.objdataccess.Get1ValueFromSQL(strSQL2, CommandType.Text);
          dataTable5.Rows[index]["DENREG"] = (object) "EEE";
          dataTable5.Rows[index]["SIGPRO"] = (object) "ZZ";
          dataTable5.Rows[index]["DENLOC"] = dataTable5.Rows[index]["DENSTAEST"];
        }
        else if (!string.IsNullOrEmpty(dataTable5.Rows[index]["DENREG"].ToString().Trim()))
          dataTable5.Rows[index]["DENREG"] = (object) dataTable5.Rows[index]["DENREG"].ToString().Substring(0, 3);
        if (int.Parse(dataTable5.Rows[index]["TIPIND"].ToString()) >= 10)
          throw new Exception("Attenzione... si è verificato un errore nella lettura del campo TIPIND: valore letto = " + dataTable5.Rows[index]["TIPIND"].ToString().Trim());
        dataTable5.Rows[index][nameof (CODPOS)] = (object) ("90" + dataTable5.Rows[index]["TIPIND"].ToString().Trim() + "A" + dataTable5.Rows[index][nameof (CODPOS)].ToString().Trim().PadLeft(6, '0'));
        if (dataTable5.Rows[index]["RAGSOCAPP"].ToString().Trim().Length <= 35)
          dataTable5.Rows[index]["RAGSOC"] = (object) dataTable5.Rows[index]["RAGSOCAPP"].ToString().Trim();
        else if (dataTable5.Rows[index]["RAGSOCAPP"].ToString().Trim().Length > 35 & dataTable5.Rows[index]["RAGSOCAPP"].ToString().Trim().Length <= 71)
        {
          dataTable5.Rows[index]["RAGSOC"] = (object) dataTable5.Rows[index]["RAGSOCAPP"].ToString().Trim().Substring(0, 35);
          dataTable5.Rows[index]["RAGSOC1"] = (object) dataTable5.Rows[index]["RAGSOCAPP"].ToString().Trim().Substring(35);
        }
        else
        {
          dataTable5.Rows[index]["RAGSOC"] = (object) dataTable5.Rows[index]["RAGSOCAPP"].ToString().Trim().Substring(0, 35);
          dataTable5.Rows[index]["RAGSOC1"] = (object) dataTable5.Rows[index]["RAGSOCAPP"].ToString().Trim().Substring(35, 35);
          dataTable5.Rows[index]["RAGSOC2"] = dataTable5.Rows[index]["RAGSOCAPP"].ToString().Trim().Length <= 100 ? (object) dataTable5.Rows[index]["RAGSOCAPP"].ToString().Trim().Substring(70) : (object) dataTable5.Rows[index]["RAGSOCAPP"].ToString().Trim().Substring(70, 30);
        }
        if (string.IsNullOrEmpty(dataTable5.Rows[index]["NUMCIV"].ToString().Trim()))
          dataTable5.Rows[index]["INDAPP"] = (object) (dataTable5.Rows[index]["DENDUG"].ToString().Trim() + " " + dataTable5.Rows[index]["IND1"].ToString().Trim());
        else
          dataTable5.Rows[index]["INDAPP"] = (object) (dataTable5.Rows[index]["DENDUG"].ToString().Trim() + " " + dataTable5.Rows[index]["IND1"].ToString().Trim() + ", " + dataTable5.Rows[index]["NUMCIV"].ToString().Trim());
        if (dataTable5.Rows[index]["INDAPP"].ToString().Trim().Length <= 35)
        {
          dataTable5.Rows[index]["IND1"] = (object) dataTable5.Rows[index]["INDAPP"].ToString().Trim();
          dataTable5.Rows[index]["IND2"] = dataTable5.Rows[index]["DENLOC"];
        }
        else if (dataTable5.Rows[index]["INDAPP"].ToString().Trim().Length > 35)
        {
          dataTable5.Rows[index]["IND1"] = (object) dataTable5.Rows[index]["INDAPP"].ToString().Trim().Substring(0, 35);
          if (dataTable5.Rows[index]["INDAPP"].ToString().Trim().Length < 71)
          {
            dataTable5.Rows[index]["IND2"] = (object) dataTable5.Rows[index]["INDAPP"].ToString().Trim().Substring(35);
            DataRow row = dataTable5.Rows[index];
            row["IND2"] = (object) (row["IND2"]?.ToString() + dataTable5.Rows[index]["IND2"].ToString() + dataTable5.Rows[index]["DENLOC"].ToString());
          }
          else
          {
            dataTable5.Rows[index]["IND2"] = (object) dataTable5.Rows[index]["INDAPP"].ToString().Trim().Substring(35, 35);
            DataRow row = dataTable5.Rows[index];
            row["IND2"] = (object) (row["IND2"]?.ToString() + dataTable5.Rows[index]["IND2"].ToString() + dataTable5.Rows[index]["DENLOC"].ToString());
          }
        }
        dataTable5.Rows[index]["EMAILDEF"] = string.IsNullOrEmpty(dataTable5.Rows[index]["EMAILCERT"].ToString().Trim()) ? (object) dataTable5.Rows[index]["EMAIL"].ToString().Trim().ToLower() : (object) dataTable5.Rows[index]["EMAILCERT"].ToString().Trim().ToLower();
        this.WRITE_IDOC_TESTATA_AZI(ref Conn, ref cmd, ref da, dataTable5.Rows[index]);
        this.WRITE_IDOC_E1PITYP_AZI(ref Conn, ref cmd, ref da, "E1KNA1M", dataTable5.Rows[index]);
        str1 = "";
      }
      DataTable dataTable6 = this.objdataccess.GetDataTable(" SELECT TRIM(CHAR(AZIRAP.CODPOS)) AS CODPOS, AZIRAP.CODFIS, '' AS PARIVA, TRIM(AZIRAP.COG) || ' ' || TRIM(AZIRAP.NOM) AS RAGSOCAPP, '005' AS MSGFN," + " '' AS RAGSOC, '' AS RAGSOC1, '' AS RAGSOC2, '' AS RAGSOCTRIM, AZIRAP.EMAIL, AZIRAP.EMAILCERT, '' EMAILDEF, " + " 'IT' AS CODSTATO," + " '' AS IND2, AZIRAP.IND AS IND1, '' AS INDAPP, AZIRAP.DENSTAEST, AZIRAP.DENLOC," + " VALUE(AZIRAP.CAP, '00000') AS CAP, AZIRAP.TEL1 AS TEL, AZIRAP.FAX, AZIRAP.SIGPRO," + " '' AS CODRAPLEG, 'RAPL' AS KTOKD, AZIRAP.NUMCIV, AZIRAP.CODFUNRAP, " + " '' AS NATGIU, '' AS CODATTCAM, " + " AZIRAP.CODCOMRES AS CODCOM, '' AS DENCOM, " + " DUG.DENDUG, CODREG.DENREG" + " FROM AZIRAP LEFT JOIN DUG ON" + " AZIRAP.CODDUG = DUG.CODDUG LEFT JOIN SIGPRO ON" + " AZIRAP.SIGPRO = SIGPRO.SIGPRO LEFT JOIN CODREG ON" + " SIGPRO.CODREG = CODREG.CODREG" + " WHERE AZIRAP.CODPOS= " + CODPOS.ToString() + " AND AZIRAP.CODFUNRAP = 5" + " ORDER BY AZIRAP.DATINI ASC");
      int num3 = dataTable6.Rows.Count - 1;
      for (int index = 0; index <= num3; ++index)
      {
        if (!string.IsNullOrEmpty(dataTable6.Rows[index]["CODCOM"].ToString().Trim()))
        {
          string str3 = this.objdataccess.Get1ValueFromSQL("SELECT VALUE(DENCOM,'') AS DENCOM FROM CODCOM WHERE CODCOM = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index]["CODCOM"].ToString().Trim()), CommandType.Text).ToString();
          dataTable6.Rows[index]["DENCOM"] = (object) str3.Trim();
        }
        ++num1;
        flag = true;
        dataTable6.Rows[index][nameof (CODPOS)] = (object) ("8" + num1.ToString().PadLeft(2, '0') + "A" + dataTable6.Rows[index][nameof (CODPOS)].ToString().Trim().PadLeft(6, '0'));
        if (!string.IsNullOrEmpty(dataTable6.Rows[index]["DENSTAEST"].ToString().Trim()))
        {
          string strSQL3 = "SELECT VALUE(CHAR(CODSTA),'') FROM CODCOM WHERE DENCOM = " + DBMethods.DoublePeakForSql(dataTable6.Rows[index]["DENSTAEST"].ToString().Trim());
          dataTable6.Rows[index]["CODSTATO"] = (object) this.objdataccess.Get1ValueFromSQL(strSQL3, CommandType.Text);
          dataTable6.Rows[index]["DENREG"] = (object) "EEE";
          dataTable6.Rows[index]["SIGPRO"] = (object) "ZZ";
          dataTable6.Rows[index]["DENLOC"] = dataTable6.Rows[index]["DENSTAEST"];
        }
        else if (!string.IsNullOrEmpty(dataTable6.Rows[index]["DENREG"].ToString().Trim()))
          dataTable6.Rows[index]["DENREG"] = (object) dataTable6.Rows[index]["DENREG"].ToString().Substring(0, 3);
        if (dataTable6.Rows[index]["RAGSOCAPP"].ToString().Trim().Length <= 35)
          dataTable6.Rows[index]["RAGSOC"] = (object) dataTable6.Rows[index]["RAGSOCAPP"].ToString().Trim();
        else if (dataTable6.Rows[index]["RAGSOCAPP"].ToString().Trim().Length > 35 & dataTable6.Rows[index]["RAGSOCAPP"].ToString().Trim().Length <= 71)
        {
          dataTable6.Rows[index]["RAGSOC"] = (object) dataTable6.Rows[index]["RAGSOCAPP"].ToString().Trim().Substring(0, 35);
          dataTable6.Rows[index]["RAGSOC1"] = (object) dataTable6.Rows[index]["RAGSOCAPP"].ToString().Trim().Substring(35);
        }
        else
        {
          dataTable6.Rows[index]["RAGSOC"] = (object) dataTable6.Rows[index]["RAGSOCAPP"].ToString().Trim().Substring(0, 35);
          dataTable6.Rows[index]["RAGSOC1"] = (object) dataTable6.Rows[index]["RAGSOCAPP"].ToString().Trim().Substring(35, 35);
          dataTable6.Rows[index]["RAGSOC2"] = dataTable6.Rows[index]["RAGSOCAPP"].ToString().Trim().Length <= 100 ? (object) dataTable6.Rows[index]["RAGSOCAPP"].ToString().Trim().Substring(70) : (object) dataTable6.Rows[index]["RAGSOCAPP"].ToString().Trim().Substring(70, 30);
        }
        if (string.IsNullOrEmpty(dataTable6.Rows[index]["NUMCIV"].ToString().Trim()))
          dataTable6.Rows[index]["INDAPP"] = (object) (dataTable6.Rows[index]["DENDUG"].ToString().Trim() + " " + dataTable6.Rows[index]["IND1"].ToString().Trim());
        else
          dataTable6.Rows[index]["INDAPP"] = (object) (dataTable6.Rows[index]["DENDUG"].ToString().Trim() + " " + dataTable6.Rows[index]["IND1"].ToString().Trim() + ", " + dataTable6.Rows[index]["NUMCIV"].ToString().Trim());
        if (dataTable6.Rows[index]["INDAPP"].ToString().Trim().Length <= 35)
        {
          dataTable6.Rows[index]["IND1"] = (object) dataTable6.Rows[index]["INDAPP"].ToString().Trim();
          dataTable6.Rows[index]["IND2"] = dataTable6.Rows[index]["DENLOC"];
        }
        else if (dataTable6.Rows[index]["INDAPP"].ToString().Trim().Length > 35)
        {
          dataTable6.Rows[index]["IND1"] = (object) dataTable6.Rows[index]["INDAPP"].ToString().Trim().Substring(0, 35);
          if (dataTable6.Rows[index]["INDAPP"].ToString().Trim().Length <= 71)
          {
            dataTable6.Rows[index]["IND2"] = (object) dataTable6.Rows[index]["INDAPP"].ToString().Trim().Substring(35);
            dataTable6.Rows[index]["IND2"] = dataTable6.Rows[index]["IND2"];
          }
          else
          {
            dataTable6.Rows[index]["IND2"] = (object) dataTable6.Rows[index]["INDAPP"].ToString().Trim().Substring(35, 35);
            dataTable6.Rows[index]["IND2"] = dataTable6.Rows[index]["IND2"];
          }
        }
        dataTable6.Rows[index]["EMAILDEF"] = string.IsNullOrEmpty(dataTable6.Rows[index]["EMAILCERT"].ToString().Trim()) ? (object) dataTable6.Rows[index]["EMAIL"].ToString().Trim().ToLower() : (object) dataTable6.Rows[index]["EMAILCERT"].ToString().Trim().ToLower();
        this.WRITE_IDOC_TESTATA_AZI(ref Conn, ref cmd, ref da, dataTable6.Rows[index]);
        this.WRITE_IDOC_E1PITYP_AZI(ref Conn, ref cmd, ref da, "E1KNA1M", dataTable6.Rows[index]);
        str1 = "";
      }
      DataTable dataTable7 = this.objdataccess.GetDataTable(" SELECT TRIM(CHAR(AZI.CODPOS)) AS CODPOS, AZI.CODFIS, AZI.PARIVA, AZI.RAGSOC AS RAGSOCAPP, '005' AS MSGFN," + " '' AS RAGSOC, '' AS RAGSOC1, '' AS RAGSOC2, '' AS RAGSOCTRIM," + " '' AS IND2, INDSED.IND AS IND1, '' AS INDAPP, INDSED.DENSTAEST, INDSED.DENLOC, INDSED.EMAIL, INDSED.EMAILCERT, '' EMAILDEF, " + " VALUE(CAP, '00000') AS CAP, INDSED.TEL1 AS TEL, INDSED.FAX, INDSED.SIGPRO," + " '' AS CODRAPLEG, " + " 'IT' AS CODSTATO," + " INDSED.CODCOM, '' AS DENCOM, " + " CASE LENGTH(TRIM(CHAR(AZI.NATGIU))) " + " WHEN 1 THEN '0' || TRIM(CHAR(AZI.NATGIU)) " + " ELSE TRIM(CHAR(AZI.NATGIU)) END AS NATGIU, " + " '99' AS CODATTCAM, 'AZIS' AS KTOKD, INDSED.NUMCIV," + " DUG.DENDUG, CODREG.DENREG" + " FROM AZI INNER JOIN INDSED ON" + " INDSED.CODPOS = AZI.CODPOS LEFT JOIN AZIATT ON" + " AZIATT.CODPOS = AZI.CODPOS LEFT JOIN DUG ON" + " INDSED.CODDUG = DUG.CODDUG LEFT JOIN SIGPRO ON" + " INDSED.SIGPRO = SIGPRO.SIGPRO LEFT JOIN CODREG ON" + " SIGPRO.CODREG = CODREG.CODREG" + " WHERE AZI.CODPOS= " + CODPOS.ToString() + " AND INDSED.DATINI <= CURRENT_DATE" + " AND INDSED.TIPIND = 1" + " AND INDSED.DATCOM = ( SELECT MAX(DATCOM) FROM INDSED" + " WHERE INDSED.CODPOS = " + CODPOS.ToString() + " AND INDSED.DATINI <= CURRENT_DATE AND INDSED.TIPIND = 1)" + " ORDER BY INDSED.DATCOM DESC FETCH FIRST 1 ROWS ONLY");
      if (dataTable7.Rows.Count > 0)
      {
        if (!string.IsNullOrEmpty(dataTable7.Rows[0]["CODCOM"].ToString().Trim()))
        {
          string str4 = "SELECT VALUE(DENCOM,'') AS DENCOM FROM CODCOM WHERE CODCOM = " + DBMethods.DoublePeakForSql(dataTable7.Rows[0]["CODCOM"].ToString().Trim());
          string str5 = this.objdataccess.Get1ValueFromSQL(strSQL1, CommandType.Text).ToString();
          dataTable7.Rows[0]["DENCOM"] = (object) str5.Trim();
        }
        if (flag)
          dataTable7.Rows[0]["CODRAPLEG"] = (object) ("8" + num1.ToString().PadLeft(2, '0') + "A" + dataTable7.Rows[0][nameof (CODPOS)].ToString().Trim().PadLeft(6, '0'));
        if (!string.IsNullOrEmpty(dataTable7.Rows[0]["DENSTAEST"].ToString().Trim()))
        {
          string strSQL4 = "SELECT VALUE(CHAR(CODSTA),'') FROM CODCOM WHERE DENCOM = " + DBMethods.DoublePeakForSql(dataTable7.Rows[0]["DENSTAEST"].ToString().Trim());
          dataTable7.Rows[0]["CODSTATO"] = (object) this.objdataccess.Get1ValueFromSQL(strSQL4, CommandType.Text).ToString();
          dataTable7.Rows[0]["DENREG"] = (object) "EEE";
          dataTable7.Rows[0]["SIGPRO"] = (object) "ZZ";
          dataTable7.Rows[0]["DENLOC"] = dataTable7.Rows[0]["DENSTAEST"];
        }
        else if (!string.IsNullOrEmpty(dataTable7.Rows[0]["DENREG"].ToString().Trim()))
          dataTable7.Rows[0]["DENREG"] = (object) dataTable7.Rows[0]["DENREG"].ToString().Substring(0, 3);
        dataTable7.Rows[0][nameof (CODPOS)] = (object) ("A" + dataTable7.Rows[0][nameof (CODPOS)].ToString().Trim().PadLeft(6, '0'));
        dataTable7.Rows[0]["RAGSOCTRIM"] = (object) dataTable7.Rows[0]["RAGSOCAPP"].ToString().Trim().Replace(" ", "");
        if (dataTable7.Rows[0]["RAGSOCTRIM"].ToString().Trim().Length > 10)
          dataTable7.Rows[0]["RAGSOCTRIM"] = (object) dataTable7.Rows[0]["RAGSOCTRIM"].ToString().Trim().Substring(0, 10);
        if (dataTable7.Rows[0]["RAGSOCAPP"].ToString().Trim().Length <= 35)
          dataTable7.Rows[0]["RAGSOC"] = (object) dataTable7.Rows[0]["RAGSOCAPP"].ToString().Trim();
        else if (dataTable7.Rows[0]["RAGSOCAPP"].ToString().Trim().Length > 35 & dataTable7.Rows[0]["RAGSOCAPP"].ToString().Trim().Length <= 71)
        {
          dataTable7.Rows[0]["RAGSOC"] = (object) dataTable7.Rows[0]["RAGSOCAPP"].ToString().Trim().Substring(0, 35);
          dataTable7.Rows[0]["RAGSOC1"] = (object) dataTable7.Rows[0]["RAGSOCAPP"].ToString().Trim().Substring(35);
        }
        else
        {
          dataTable7.Rows[0]["RAGSOC"] = (object) dataTable7.Rows[0]["RAGSOCAPP"].ToString().Trim().Substring(0, 35);
          dataTable7.Rows[0]["RAGSOC1"] = (object) dataTable7.Rows[0]["RAGSOCAPP"].ToString().Trim().Substring(35, 35);
          dataTable7.Rows[0]["RAGSOC2"] = dataTable7.Rows[0]["RAGSOCAPP"].ToString().Trim().Length <= 100 ? (object) dataTable7.Rows[0]["RAGSOCAPP"].ToString().Trim().Substring(70) : (object) dataTable7.Rows[0]["RAGSOCAPP"].ToString().Trim().Substring(70, 30);
        }
        if (string.IsNullOrEmpty(dataTable7.Rows[0]["NUMCIV"].ToString().Trim()))
          dataTable7.Rows[0]["INDAPP"] = (object) (dataTable7.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable7.Rows[0]["IND1"].ToString().Trim());
        else
          dataTable7.Rows[0]["INDAPP"] = (object) (dataTable7.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable7.Rows[0]["IND1"].ToString().Trim() + ", " + dataTable7.Rows[0]["NUMCIV"].ToString().Trim());
        if (dataTable7.Rows[0]["INDAPP"].ToString().Trim().Length <= 35)
        {
          dataTable7.Rows[0]["IND1"] = (object) dataTable7.Rows[0]["INDAPP"].ToString().Trim();
          dataTable7.Rows[0]["IND2"] = dataTable7.Rows[0]["DENLOC"];
        }
        else if (dataTable7.Rows[0]["INDAPP"].ToString().Trim().Length > 35)
        {
          dataTable7.Rows[0]["IND1"] = (object) dataTable7.Rows[0]["INDAPP"].ToString().Trim().Substring(0, 35);
          if (dataTable7.Rows[0]["INDAPP"].ToString().Trim().Length < 71)
          {
            dataTable7.Rows[0]["IND2"] = (object) dataTable7.Rows[0]["INDAPP"].ToString().Trim().Substring(35);
            dataTable7.Rows[0]["IND2"] = dataTable7.Rows[0]["IND2"];
          }
          else
          {
            dataTable7.Rows[0]["IND2"] = (object) dataTable7.Rows[0]["INDAPP"].ToString().Trim().Substring(35, 35);
            dataTable7.Rows[0]["IND2"] = dataTable7.Rows[0]["IND2"];
          }
        }
        dataTable7.Rows[0]["EMAILDEF"] = string.IsNullOrEmpty(dataTable7.Rows[0]["EMAILCERT"].ToString().Trim()) ? (object) dataTable7.Rows[0]["EMAIL"].ToString().Trim().ToLower() : (object) dataTable7.Rows[0]["EMAILCERT"].ToString().Trim().ToLower();
        this.WRITE_IDOC_TESTATA_AZI(ref Conn, ref cmd, ref da, dataTable7.Rows[0]);
        this.WRITE_IDOC_E1PITYP_AZI(ref Conn, ref cmd, ref da, "E1KNA1M", dataTable7.Rows[0]);
        str1 = "";
      }
      if (DT_SEDI_OLD.Rows.Count > 0)
      {
        for (int index = 0; index < DT_SEDI_OLD.Rows.Count; ++index)
        {
          DT_SEDI_OLD.Rows[index][nameof (CODPOS)] = (object) ("A" + DT_SEDI_OLD.Rows[index][nameof (CODPOS)].ToString().Trim().PadLeft(6, '0'));
          DT_SEDI_OLD.Rows[index]["CODPOSSED"] = (object) ("90" + DT_SEDI_OLD.Rows[index]["TIPIND"].ToString().Trim() + DT_SEDI_OLD.Rows[index][nameof (CODPOS)].ToString().Trim().PadLeft(6, '0'));
          this.WRITE_IDOC_E1PITYP_AZI(ref Conn, ref cmd, ref da, "ZE1KNZA", DT_SEDI_OLD.Rows[index]);
        }
      }
      string str6 = " SELECT TRIM(CHAR(AZI.CODPOS)) AS CODPOS, '' AS CODPOSSED, 'I' AS OPERA, INDSED.TIPIND , '005' AS MSGFN " + " FROM AZI INNER JOIN INDSED ON" + " INDSED.CODPOS = AZI.CODPOS LEFT JOIN DUG ON" + " INDSED.CODDUG = DUG.CODDUG LEFT JOIN SIGPRO ON" + " INDSED.SIGPRO = SIGPRO.SIGPRO LEFT JOIN CODREG ON" + " SIGPRO.CODREG = CODREG.CODREG" + " INNER JOIN" + " (SELECT TIPIND, MAX(DATINI) AS DATINI FROM INDSED WHERE CODPOS = " + CODPOS.ToString() + " GROUP BY TIPIND) AS IND" + " ON INDSED.TIPIND = IND.TIPIND AND INDSED.DATINI = IND.DATINI" + " WHERE AZI.CODPOS= " + CODPOS.ToString() + " ORDER BY INDSED.TIPIND ";
      DataTable dataTable8 = this.objdataccess.GetDataTable(strSQL1);
      for (int index = 0; index < dataTable8.Rows.Count; ++index)
      {
        dataTable8.Rows[index][nameof (CODPOS)] = (object) ("A" + dataTable8.Rows[index][nameof (CODPOS)].ToString().Trim().PadLeft(6, '0'));
        if (dataTable8.Rows[index].Table.Columns.Contains("CODPOSSED"))
          dataTable8.Rows[index]["CODPOSSED"] = (object) ("90" + dataTable8.Rows[index]["TIPIND"].ToString().Trim() + dataTable8.Rows[index][nameof (CODPOS)].ToString().Trim().PadLeft(6, '0'));
        this.WRITE_IDOC_E1PITYP_AZI(ref Conn, ref cmd, ref da, "ZE1KNZA", dataTable8.Rows[index]);
      }
      string strSQL5 = " SELECT TRIM(CHAR(AZISTO.CODSTACON)) AS CODSTACON, '005' AS MSGFN FROM AZISTO " + " WHERE AZISTO.CODPOS = " + CODPOS.ToString() + " ORDER BY DATINI DESC";
      dataTable8.Clear();
      DataTable dataTable9 = this.objdataccess.GetDataTable(strSQL5);
      if (dataTable9.Rows.Count <= 0)
        return;
      if (Convert.ToDecimal(dataTable9.Rows[0]["CODSTACON"].ToString()) > 0M)
        dataTable9.Rows[0]["CODSTACON"] = (object) "99.999";
      this.WRITE_IDOC_E1PITYP_AZI(ref Conn, ref cmd, ref da, "E1KNB1M", dataTable9.Rows[0]);
    }

    public void WRITE_IDOC_CUR_AZI(
      ref iDB2Connection Conn,
      ref iDB2Command cmd,
      ref iDB2DataAdapter da,
      int CODPOS)
    {
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataTable dataTable3 = new DataTable();
      DataTable dataTable4 = new DataTable();
      bool flag = false;
      int num1 = 0;
      string strSQL1 = " SELECT CODFUNRAP FROM AZIRAP" + " WHERE CODPOS= " + CODPOS.ToString() + " AND CODFUNRAP = 5" + " ORDER BY DATINI ASC";
      int num2 = this.objdataccess.GetDataTable(strSQL1).Rows.Count - 1;
      for (int index = 0; index <= num2; ++index)
      {
        ++num1;
        flag = true;
      }
      DataTable dataTable5 = this.objdataccess.GetDataTable(" SELECT TRIM(CHAR(AZI.CODPOS)) AS CODPOS, AZI.CODFIS, AZI.PARIVA, AZI.RAGSOC AS RAGSOCAPP, '005' AS MSGFN," + " '' AS RAGSOC, '' AS RAGSOC1, '' AS RAGSOC2, '' AS RAGSOCTRIM," + " '' AS IND2, INDSED.IND AS IND1, '' AS INDAPP, INDSED.DENSTAEST, INDSED.DENLOC," + " '' AS DENCOM, INDSED.CODCOM, " + " VALUE(CAP, '00000') AS CAP, INDSED.TEL1 AS TEL, INDSED.FAX, INDSED.SIGPRO," + " '' AS CODRAPLEG, " + " 'IT' AS CODSTATO," + " AZI.NATGIU, '99' AS CODATTCAM, 'AZIS' AS KTOKD, INDSED.NUMCIV," + " DUG.DENDUG, CODREG.DENREG" + " FROM AZI INNER JOIN INDSED ON" + " INDSED.CODPOS = AZI.CODPOS LEFT JOIN AZIATT ON" + " AZIATT.CODPOS = AZI.CODPOS RIGHT JOIN DUG ON" + " INDSED.CODDUG = DUG.CODDUG LEFT JOIN SIGPRO ON" + " INDSED.SIGPRO = SIGPRO.SIGPRO LEFT JOIN CODREG ON" + " SIGPRO.CODREG = CODREG.CODREG" + " WHERE AZI.CODPOS= " + CODPOS.ToString() + " AND INDSED.DATINI <= CURRENT_DATE" + " AND INDSED.TIPIND = 1" + " AND INDSED.DATCOM = ( SELECT MAX(DATCOM) FROM INDSED" + " WHERE INDSED.CODPOS = " + CODPOS.ToString() + " AND INDSED.DATINI <= CURRENT_DATE AND INDSED.TIPIND = 1)" + " ORDER BY INDSED.DATCOM DESC FETCH FIRST 1 ROWS ONLY");
      if (dataTable5.Rows.Count <= 0)
        return;
      if (!string.IsNullOrEmpty(dataTable5.Rows[0]["CODCOM"].ToString().Trim()))
      {
        string str1 = "SELECT VALUE(DENCOM,'') AS DENCOM FROM CODCOM WHERE CODCOM = " + DBMethods.DoublePeakForSql(dataTable5.Rows[0]["CODCOM"].ToString().Trim());
        string str2 = this.objdataccess.Get1ValueFromSQL(strSQL1, CommandType.Text).ToString();
        dataTable5.Rows[0]["DENCOM"] = (object) str2.Trim();
      }
      if (flag)
        dataTable5.Rows[0]["CODRAPLEG"] = (object) ("8" + num1.ToString().PadLeft(2, '0') + "A" + dataTable5.Rows[0][nameof (CODPOS)].ToString().Trim().PadLeft(6, '0'));
      dataTable5.Rows[0][nameof (CODPOS)] = (object) ("A" + dataTable5.Rows[0][nameof (CODPOS)].ToString().Trim().PadLeft(6, '0'));
      if (!string.IsNullOrEmpty(dataTable5.Rows[0]["DENSTAEST"].ToString().Trim()))
      {
        strSQL1 = "SELECT VALUE(CHAR(CODSTA),'') FROM CODCOM WHERE DENCOM = " + DBMethods.DoublePeakForSql(dataTable5.Rows[0]["DENSTAEST"].ToString().Trim());
        dataTable5.Rows[0]["CODSTATO"] = (object) this.objdataccess.Get1ValueFromSQL(strSQL1, CommandType.Text).ToString();
        dataTable5.Rows[0]["DENREG"] = (object) "EEE";
        dataTable5.Rows[0]["SIGPRO"] = (object) "ZZ";
        dataTable5.Rows[0]["DENLOC"] = dataTable5.Rows[0]["DENSTAEST"];
      }
      else if (!string.IsNullOrEmpty(dataTable5.Rows[0]["DENREG"].ToString().Trim()))
        dataTable5.Rows[0]["DENREG"] = (object) dataTable5.Rows[0]["DENREG"].ToString().Substring(0, 3);
      dataTable5.Rows[0]["RAGSOCTRIM"] = (object) dataTable5.Rows[0]["RAGSOCAPP"].ToString().Trim().Replace(" ", "");
      if (dataTable5.Rows[0]["RAGSOCTRIM"].ToString().Trim().Length > 10)
        dataTable5.Rows[0]["RAGSOCTRIM"] = (object) dataTable5.Rows[0]["RAGSOCTRIM"].ToString().Trim().Substring(0, 10);
      if (dataTable5.Rows[0]["RAGSOCAPP"].ToString().Trim().Length <= 35)
        dataTable5.Rows[0]["RAGSOC"] = (object) dataTable5.Rows[0]["RAGSOCAPP"].ToString().Trim();
      else if (dataTable5.Rows[0]["RAGSOCAPP"].ToString().Trim().Length > 35 & dataTable5.Rows[0]["RAGSOCAPP"].ToString().Trim().Length <= 71)
      {
        dataTable5.Rows[0]["RAGSOC"] = (object) dataTable5.Rows[0]["RAGSOCAPP"].ToString().Trim().Substring(0, 35);
        dataTable5.Rows[0]["RAGSOC1"] = (object) dataTable5.Rows[0]["RAGSOCAPP"].ToString().Trim().Substring(35);
      }
      else
      {
        dataTable5.Rows[0]["RAGSOC"] = (object) dataTable5.Rows[0]["RAGSOCAPP"].ToString().Trim().Substring(0, 35);
        dataTable5.Rows[0]["RAGSOC1"] = (object) dataTable5.Rows[0]["RAGSOCAPP"].ToString().Trim().Substring(35, 35);
        dataTable5.Rows[0]["RAGSOC2"] = dataTable5.Rows[0]["RAGSOCAPP"].ToString().Trim().Length <= 100 ? (object) dataTable5.Rows[0]["RAGSOCAPP"].ToString().Trim().Substring(70) : (object) dataTable5.Rows[0]["RAGSOCAPP"].ToString().Trim().Substring(70, 30);
      }
      if (string.IsNullOrEmpty(dataTable5.Rows[0]["NUMCIV"].ToString().Trim()))
        dataTable5.Rows[0]["INDAPP"] = (object) (dataTable5.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable5.Rows[0]["IND1"].ToString().Trim());
      else
        dataTable5.Rows[0]["INDAPP"] = (object) (dataTable5.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable5.Rows[0]["IND1"].ToString().Trim() + ", " + dataTable5.Rows[0]["NUMCIV"].ToString().Trim());
      if (dataTable5.Rows[0]["INDAPP"].ToString().Trim().Length <= 35)
        dataTable5.Rows[0]["IND1"] = (object) dataTable5.Rows[0]["INDAPP"].ToString().Trim();
      else if (dataTable5.Rows[0]["INDAPP"].ToString().Trim().Length > 35)
      {
        dataTable5.Rows[0]["IND1"] = (object) dataTable5.Rows[0]["INDAPP"].ToString().Trim().Substring(0, 35);
        dataTable5.Rows[0]["IND2"] = dataTable5.Rows[0]["INDAPP"].ToString().Trim().Length >= 71 ? (object) dataTable5.Rows[0]["INDAPP"].ToString().Trim().Substring(35, 35) : (object) dataTable5.Rows[0]["INDAPP"].ToString().Trim().Substring(35);
      }
      this.WRITE_IDOC_TESTATA_AZI(ref Conn, ref cmd, ref da, dataTable5.Rows[0]);
      this.WRITE_IDOC_E1PITYP_AZI(ref Conn, ref cmd, ref da, "E1KNA1M", dataTable5.Rows[0]);
      string str3 = "";
      string str4 = " SELECT TRIM(CHAR(CODPOS)) AS CODPOS, '' AS INDAPP, IND AS IND1, '' AS IND2, '005' AS MSGFN, VALUE(DATFIN, '9999-12-31') AS DATFIN, ";
      string strSQL2 = strSQL1 + " DENSTAEST, DENLOC, '' AS IND, ";
      string str5 = str4 + " CODCOM, '' AS DENCOM, " + " TRIM(COG) || ' ' || (NOM) AS COGNOM, NOM, " + " (" + " SELECT DENDUG FROM DUG WHERE CURAZILEG.CODDUG = DUG.CODDUG" + " ) AS DENDUG, '' AS DTOPERA, CODTIT AS ABTNR, " + " VALUE(CAP, '00000') AS CAP, TEL1 AS TEL, FAX, NUMCIV, 'IT' AS CODSTATO " + " FROM CURAZILEG " + " WHERE CODPOS = " + CODPOS.ToString() + " AND DATFIN IS NULL ORDER BY DATINI, DATFIN";
      DataTable dataTable6 = this.objdataccess.GetDataTable(strSQL2);
      if (dataTable6.Rows.Count > 0)
      {
        if (!string.IsNullOrEmpty(dataTable6.Rows[0]["CODCOM"].ToString().Trim()))
        {
          string str6 = this.objdataccess.Get1ValueFromSQL("SELECT VALUE(DENCOM,'') AS DENCOM FROM CODCOM WHERE CODCOM = " + DBMethods.DoublePeakForSql(dataTable6.Rows[0]["CODCOM"].ToString().Trim()), CommandType.Text);
          dataTable6.Rows[0]["DENCOM"] = (object) str6.Trim();
        }
        dataTable6.Rows[0][nameof (CODPOS)] = (object) ("A" + dataTable6.Rows[0][nameof (CODPOS)].ToString().Trim().PadLeft(6, '0'));
        if (string.IsNullOrEmpty(dataTable6.Rows[0]["NUMCIV"].ToString().Trim()))
          dataTable6.Rows[0]["INDAPP"] = (object) (dataTable6.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable6.Rows[0]["IND1"].ToString().Trim());
        else
          dataTable6.Rows[0]["INDAPP"] = (object) (dataTable6.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable6.Rows[0]["IND1"].ToString().Trim() + ", " + dataTable6.Rows[0]["NUMCIV"].ToString().Trim());
        if (dataTable6.Rows[0]["INDAPP"].ToString().Trim().Length <= 60)
          dataTable6.Rows[0]["IND1"] = (object) dataTable6.Rows[0]["INDAPP"].ToString().Trim();
        else if (dataTable6.Rows[0]["INDAPP"].ToString().Trim().Length > 60)
        {
          dataTable6.Rows[0]["IND1"] = (object) dataTable6.Rows[0]["INDAPP"].ToString().Trim().Substring(0, 60);
          dataTable6.Rows[0]["IND2"] = dataTable6.Rows[0]["INDAPP"].ToString().Trim().Length >= 90 ? (object) dataTable6.Rows[0]["INDAPP"].ToString().Trim().Substring(60, 35) : (object) dataTable6.Rows[0]["INDAPP"].ToString().Trim().Substring(60);
        }
        if (!string.IsNullOrEmpty(dataTable6.Rows[0]["DENSTAEST"].ToString().Trim()))
        {
          string strSQL3 = "SELECT VALUE(CHAR(CODSTA),'') FROM CODCOM WHERE DENCOM = " + DBMethods.DoublePeakForSql(dataTable6.Rows[0]["DENSTAEST"].ToString().Trim());
          dataTable6.Rows[0]["CODSTATO"] = (object) this.objdataccess.Get1ValueFromSQL(strSQL3, CommandType.Text);
          dataTable6.Rows[0]["DENLOC"] = dataTable6.Rows[0]["DENSTAEST"];
        }
        this.WRITE_IDOC_E1PITYP_AZI(ref Conn, ref cmd, ref da, "E1KNA11", dataTable6.Rows[0]);
        this.WRITE_IDOC_E1PITYP_AZI(ref Conn, ref cmd, ref da, "ZE1SADR", dataTable6.Rows[0]);
        string strSQL4 = " SELECT TRIM(CHAR(AZISTO.CODSTACON)) AS CODSTACON , '005' AS MSGFN FROM AZISTO " + " WHERE AZISTO.CODPOS = " + CODPOS.ToString() + " ORDER BY DATINI DESC";
        dataTable3.Clear();
        DataTable dataTable7 = this.objdataccess.GetDataTable(strSQL4);
        if (dataTable7.Rows.Count > 0)
        {
          if (Convert.ToDecimal(dataTable7.Rows[0]["CODSTACON"].ToString()) > 0M)
            dataTable7.Rows[0]["CODSTACON"] = (object) "99.999";
          this.WRITE_IDOC_E1PITYP_AZI(ref Conn, ref cmd, ref da, "E1KNB1M", dataTable7.Rows[0]);
        }
        this.WRITE_IDOC_E1PITYP_AZI(ref Conn, ref cmd, ref da, "E1KNVKM", dataTable6.Rows[0]);
      }
      else
      {
        string strSQL5 = " SELECT TRIM(CHAR(AZISTO.CODSTACON)) AS CODSTACON , '005' AS MSGFN FROM AZISTO " + " WHERE AZISTO.CODPOS = " + CODPOS.ToString() + " ORDER BY DATINI DESC";
        dataTable3.Clear();
        DataTable dataTable8 = this.objdataccess.GetDataTable(strSQL5);
        if (dataTable8.Rows.Count > 0)
        {
          if (Convert.ToDecimal(dataTable8.Rows[0]["CODSTACON"].ToString()) > 0M)
            dataTable8.Rows[0]["CODSTACON"] = (object) "99.999";
          this.WRITE_IDOC_E1PITYP_AZI(ref Conn, ref cmd, ref da, "E1KNB1M", dataTable8.Rows[0]);
        }
        str3 = "";
        DataTable dataTable9 = this.objdataccess.GetDataTable(" SELECT TRIM(CHAR(CODPOS)) AS CODPOS, '' AS INDAPP, IND AS IND1, '' AS IND2, '003' AS MSGFN, VALUE(DATFIN, '9999-12-31') AS DATFIN, " + " DENSTAEST, DENLOC, '' AS IND, " + " CODCOM, '' AS DENCOM, " + " TRIM(COG) || ' ' || (NOM) AS COGNOM, NOM, " + " (" + " SELECT DENDUG FROM DUG WHERE CURAZILEG.CODDUG = DUG.CODDUG" + " ) AS DENDUG, '' AS DTOPERA, CODTIT AS ABTNR, " + " VALUE(CAP, '00000') AS CAP, TEL1 AS TEL, FAX, NUMCIV, 'IT' AS CODSTATO " + " FROM CURAZILEG " + " WHERE CODPOS = " + CODPOS.ToString() + " AND DATFIN IS NOT NULL ORDER BY DATFIN DESC");
        if (dataTable9.Rows.Count <= 0)
          return;
        if (!string.IsNullOrEmpty(dataTable9.Rows[0]["CODCOM"].ToString().Trim()))
        {
          string str7 = this.objdataccess.Get1ValueFromSQL("SELECT VALUE(DENCOM,'') AS DENCOM FROM CODCOM WHERE CODCOM = " + DBMethods.DoublePeakForSql(dataTable9.Rows[0]["CODCOM"].ToString().Trim()), CommandType.Text).ToString();
          dataTable9.Rows[0]["DENCOM"] = (object) str7.Trim();
        }
        this.WRITE_IDOC_E1PITYP_AZI(ref Conn, ref cmd, ref da, "E1KNVKM", dataTable9.Rows[0]);
      }
    }

    public void WRITE_IDOC_E1PITYP_AZI(
      ref iDB2Connection Conn,
      ref iDB2Command cmd,
      ref iDB2DataAdapter da,
      string VAR_E1PITYP,
      DataRow ROW_DATI)
    {
      DataTable dataTable1 = new DataTable();
      cmd.Connection = Conn;
      string strSQL = "SELECT IDTAB, TABSAP FROM IDOCTAB WHERE INFTYP = '" + VAR_E1PITYP + "' AND VALUE(USATO, 'S') = 'S'";
      dataTable1.Clear();
      DataTable dataTable2 = this.objdataccess.GetDataTable(strSQL);
      this.WRITE_EDID4_AZI(ref Conn, ref cmd, ref da, Convert.ToInt32(dataTable2.Rows[0]["IDTAB"]), Convert.ToString(dataTable2.Rows[0]["TABSAP"]), VAR_E1PITYP, ref ROW_DATI);
    }

    public void WRITE_IDOC_TESTATA_AZI(
      ref iDB2Connection Conn,
      ref iDB2Command cmd,
      ref iDB2DataAdapter da,
      DataRow ROW_DATI)
    {
      string DOCNUM = "";
      string str1 = "";
      string MANDT = "";
      string PSGNUM = "";
      string HLEVEL = "";
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataTable dataTable3 = new DataTable();
      cmd.Connection = Conn;
      this.IDOC_CONT_AZI = Convert.ToInt32(this.Module_GetCONT_AZI(ref Conn, ref cmd, ref da));
      if (this.objDtCONTIDOC_AZI == null)
      {
        this.objDtCONTIDOC_AZI = new DataTable();
        this.objDtCONTIDOC_AZI.Columns.Add("CONTIDOC");
      }
      if (!this.objDtCONTIDOC_AZI.Columns.Contains("CONTIDOC"))
        this.objDtCONTIDOC_AZI.Columns.Add("CONTIDOC");
      this.objDtCONTIDOC_AZI.Rows.Add(this.objDtCONTIDOC_AZI.NewRow());
      this.objDtCONTIDOC_AZI.Rows[this.objDtCONTIDOC_AZI.Rows.Count - 1]["CONTIDOC"] = (object) this.IDOC_CONT_AZI;
      this.SEGNUM = "";
      string strSQL1 = "SELECT * FROM IDOCTAB WHERE IDTAB = 15 AND VALUE(USATO, 'S') = 'S' ORDER BY ORDINE";
      dataTable1.Clear();
      DataTable dataTable4 = this.objdataccess.GetDataTable(strSQL1);
      int num1 = dataTable4.Rows.Count - 1;
      for (int index1 = 0; index1 <= num1; ++index1)
      {
        string str2 = "";
        string str3 = "";
        string strSQL2 = "SELECT * FROM IDOCCAMPI WHERE IDTAB = '" + dataTable4.Rows[index1]["IDTAB"].ToString() + "' ORDER BY ORDINE";
        dataTable2.Clear();
        dataTable2 = this.objdataccess.GetDataTable(strSQL2);
        this.SEGNUM = this.Module_GetSEGNUM_AZI(ref Conn, ref cmd, ref da, this.IDOC_CONT_AZI);
        int num2 = dataTable2.Rows.Count - 1;
        for (int index2 = 0; index2 <= num2; ++index2)
        {
          str2 = str2 + ", " + dataTable2.Rows[index2]["CAMPO"].ToString();
          string str4 = dataTable2.Rows[index2]["CAMPO"].ToString().Trim().ToUpper() ?? "";
          if (!(str4 == "MANDT"))
          {
            if (str4 == "DOCNUM")
              DOCNUM = this.GetFormato_AZI(dataTable2.Rows[index2]["FORMATO"].ToString(), this.IDOC_CONT_AZI.ToString(), Convert.ToInt32(dataTable2.Rows[index2]["LUNGHEZZA"]));
          }
          else
            MANDT = this.GetFormato_AZI(dataTable2.Rows[index2]["FORMATO"].ToString(), dataTable2.Rows[index2]["VALORE"].ToString(), Convert.ToInt32(dataTable2.Rows[index2]["LUNGHEZZA"]));
          string str5 = dataTable2.Rows[index2]["VALORE"].ToString().Trim().ToUpper() ?? "";
          if (!(str5 == "[CONTATORE]"))
          {
            str3 = str5 == "IDOCVALORI" ? str3 + ", " + this.GetStrValues_AZI(ref Conn, ref cmd, ref da, Convert.ToInt32(dataTable2.Rows[index2]["IDCAMPI"]), ref ROW_DATI, dataTable4.Rows[index1]["INFTYP"].ToString().Trim()) : str3 + ", " + this.GetFormato_AZI(dataTable2.Rows[index2]["FORMATO"].ToString(), dataTable2.Rows[index2]["VALORE"].ToString(), Convert.ToInt32(dataTable2.Rows[index2]["LUNGHEZZA"]));
          }
          else
          {
            string str6 = dataTable2.Rows[index2]["CAMPO"].ToString().Trim().ToUpper() ?? "";
            if (!(str6 == "SEGNUM"))
            {
              if (!(str6 == "CONT"))
              {
                if (!(str6 == "PSGNUM"))
                {
                  if (!(str6 == "HLEVEL"))
                  {
                    if (str6 == "DOCNUM")
                      str1 = this.IDOC_CONT_AZI.ToString();
                  }
                  else
                    str1 = HLEVEL;
                }
                else
                {
                  this.Module_GetPSGNUM_HLEVEL_AZI(ref Conn, ref cmd, ref da, Convert.ToInt32(dataTable4.Rows[index1]["IDTAB"]), MANDT, DOCNUM, ref HLEVEL, ref PSGNUM);
                  str1 = PSGNUM;
                }
              }
              else
                str1 = this.IDOC_CONT_AZI.ToString();
            }
            else
              str1 = this.SEGNUM;
            str3 = str3 + ", " + this.GetFormato_AZI(dataTable2.Rows[index2]["FORMATO"].ToString(), str1.ToString(), Convert.ToInt32(dataTable2.Rows[index2]["LUNGHEZZA"]));
          }
        }
        string str7 = str2 + ", CONT_NUM ";
        string str8 = str3 + ", " + this.IDOC_CONT_AZI.ToString() + " ";
        this.objdataccess.WriteTransactionData("INSERT INTO " + dataTable4.Rows[index1]["TABSAP"].ToString().Trim() + " (" + str7.Substring(2) + ")" + (" VALUES (" + str8.Substring(2) + ")"), CommandType.Text);
      }
    }

    public string Module_GetCONT_AZI(
      ref iDB2Connection Conn,
      ref iDB2Command cmd,
      ref iDB2DataAdapter da)
    {
      DataTable dataTable = new DataTable();
      Random random = new Random();
      string str = this.objdataccess.Get1ValueFromSQL(" SELECT VALUE(CHAR(CURRENT_TIMESTAMP),'') FROM MENU", CommandType.Text).ToString();
      int num = random.Next();
      this.objdataccess.WriteData("INSERT INTO COUNT_IDOC(ULTAGG, RND)" + " VALUES ('" + str + "', " + num.ToString() + ") ", CommandType.Text);
      return this.objdataccess.Get1ValueFromSQL(" SELECT CONT FROM COUNT_IDOC WHERE ULTAGG = '" + str + "'" + " AND RND = " + num.ToString(), CommandType.Text).ToString();
    }

    public void Module_GetPSGNUM_HLEVEL_AZI(
      ref iDB2Connection Conn,
      ref iDB2Command cmd,
      ref iDB2DataAdapter da,
      int IDTAB,
      string MANDT,
      string DOCNUM,
      ref string HLEVEL,
      ref string PSGNUM)
    {
      DataTable dataTable1 = new DataTable();
      string strSQL = "SELECT PADRE, HLEVEL FROM IDOCTAB WHERE IDTAB = " + IDTAB.ToString();
      DataTable dataTable2 = this.objdataccess.GetDataTable(strSQL);
      string str1 = dataTable2.Rows[0]["PADRE"].ToString().Trim();
      HLEVEL = dataTable2.Rows[0][nameof (HLEVEL)].ToString().Trim();
      string str2 = "SELECT SEGNUM FROM EDID4 WHERE CONT_NUM = " + this.IDOC_CONT_AZI.ToString() + " AND MANDT = " + MANDT + " AND DOCNUM = " + DOCNUM + " AND SEGNAM = " + DBMethods.DoublePeakForSql(str1);
      PSGNUM = this.objdataccess.Get1ValueFromSQL(strSQL, CommandType.Text);
    }

    public string Module_GetSEGNUM_AZI(
      ref iDB2Connection Conn,
      ref iDB2Command cmd,
      ref iDB2DataAdapter da,
      int CONT)
    {
      return this.objdataccess.Get1ValueFromSQL("SELECT VALUE(MAX(SEGNUM),0) + 1 AS SEGNUM FROM EDID4 WHERE CONT_NUM = '" + CONT.ToString() + "' ", CommandType.Text);
    }

    public string GetStrValues_AZI(
      ref iDB2Connection Conn,
      ref iDB2Command cmd,
      ref iDB2DataAdapter da,
      int IDCAMPI,
      ref DataRow ROW_DATI,
      string INFTYP)
    {
      DataTable dataTable1 = new DataTable();
      string str1 = "";
      DataTable dataTable2 = this.objdataccess.GetDataTable("SELECT CAMPOSAP, TIPO, LUNGHEZZA, VALORE, VALDEFAULT, VALDTDATI, FORMATO FROM IDOCVALORI WHERE IDCAMPI = " + IDCAMPI.ToString() + " ORDER BY ORDINE");
      int num = dataTable2.Rows.Count - 1;
      for (int index = 0; index <= num; ++index)
      {
        string str2 = !(dataTable2.Rows[index]["CAMPOSAP"].ToString().Trim().ToUpper() == "INFTY") ? (!((dataTable2.Rows[index]["VALORE"].ToString().Trim().ToUpper() ?? "") == "DTDATI") ? dataTable2.Rows[index]["VALDEFAULT"].ToString().Trim() : (!string.IsNullOrEmpty(dataTable2.Rows[index]["VALDTDATI"].ToString().Trim()) ? dataTable2.Rows[index]["VALDTDATI"].ToString().Trim() : dataTable2.Rows[index]["VALDEFAULT"].ToString().Trim())) : INFTYP;
        str1 += this.GetFormato_AZI(dataTable2.Rows[index]["FORMATO"].ToString(), str2.ToString(), Convert.ToInt32(dataTable2.Rows[index]["LUNGHEZZA"]), false);
      }
      return "'" + str1.Replace("'", "''") + "'";
    }

    public string GetFormato_AZI(string FORMATO, string TESTO, int LUNGHEZZA, bool USE_DOUBLEPEAK = true)
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
          if (!string.IsNullOrEmpty(TESTO))
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
          if (!string.IsNullOrEmpty(TESTO) & TESTO != "99991231" & TESTO != "18000101")
          {
            TESTO = DBMethods.Db2Date(TESTO);
            TESTO = TESTO.Replace("-", "");
          }
          return TESTO.PadRight(LUNGHEZZA, ' ');
        default:
          return !USE_DOUBLEPEAK ? TESTO.ToString().Trim().PadRight(LUNGHEZZA, ' ') : DBMethods.DoublePeakForSql(TESTO.ToString().Trim().PadRight(LUNGHEZZA, ' '));
      }
    }

    public void Aggiorna_IDOC_AZI(ref iDB2Command cmd)
    {
      short num = (short) (this.objDtCONTIDOC_AZI.Rows.Count - 1);
      for (short index = 0; (int) index <= (int) num; ++index)
        this.objdataccess.WriteTransactionData("UPDATE EDIDC SET STATUS = 64 WHERE STATUS = 00 AND CONT_NUM = '" + this.objDtCONTIDOC_AZI.Rows[(int) index]["CONTIDOC"]?.ToString() + "' ", CommandType.Text);
    }
  }
}
