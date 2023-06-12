// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Utilities.modIDOC_TabelleDiServizio
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using System;
using System.Data;
using TFI.DAL.ConnectorDB;

namespace TFI.DAL.Utilities
{
  public class modIDOC_TabelleDiServizio
  {
    public DataTable objDtCONTIDOC_SERV;
    private int IDOC_CONT_SERV;
    private string SEGNUM = "";

    private void WRITE_EDID4_TdS(
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
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL);
      this.SEGNUM = this.Module_GetSEGNUM_TdS(objDataAccess, this.IDOC_CONT_SERV);
      for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
      {
        str2 = str2 + ", " + dataTable2.Rows[index]["CAMPO"]?.ToString();
        string upper1 = dataTable2.Rows[index]["CAMPO"].ToString().Trim().ToUpper();
        if (!(upper1 == "MANDT"))
        {
          if (upper1 == "DOCNUM")
            DOCNUM = this.GetFormato_TdS(dataTable2.Rows[index]["FORMATO"].ToString(), this.IDOC_CONT_SERV.ToString(), Convert.ToInt32(dataTable2.Rows[index]["LUNGHEZZA"]));
        }
        else
          MANDT = this.GetFormato_TdS(dataTable2.Rows[index]["FORMATO"].ToString(), dataTable2.Rows[index]["VALORE"].ToString(), Convert.ToInt32(dataTable2.Rows[index]["LUNGHEZZA"]));
        string upper2 = dataTable2.Rows[index]["VALORE"].ToString().Trim().ToUpper();
        if (!(upper2 == "[CONTATORE]"))
        {
          str3 = upper2 == "IDOCVALORI" ? str3 + ", " + this.GetStrValues_TdS(objDataAccess, Convert.ToInt32(dataTable2.Rows[index]["IDCAMPI"]), ref ROW_DATI, VAR_E1PITYP) : str3 + ", " + this.GetFormato_TdS(dataTable2.Rows[index]["FORMATO"].ToString(), dataTable2.Rows[index]["VALORE"].ToString(), Convert.ToInt32(dataTable2.Rows[index]["LUNGHEZZA"]));
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
                    str1 = this.IDOC_CONT_SERV.ToString();
                }
                else
                  str1 = HLEVEL;
              }
              else
              {
                this.Module_GetPSGNUM_HLEVEL_TdS(objDataAccess, IDTAB, MANDT, DOCNUM, ref HLEVEL, ref PSGNUM);
                str1 = PSGNUM;
              }
            }
            else
              str1 = this.IDOC_CONT_SERV.ToString();
          }
          else
            str1 = this.SEGNUM;
          str3 = str3 + ", " + this.GetFormato_TdS(dataTable2.Rows[index]["FORMATO"].ToString(), str1.ToString(), Convert.ToInt32(dataTable2.Rows[index]["LUNGHEZZA"]));
        }
      }
      string str4 = str2 + ", CONT_NUM ";
      string str5 = str3 + ", " + this.IDOC_CONT_SERV.ToString() + " ";
      string str6 = "INSERT INTO " + TABSAP + " (" + str4.Substring(2) + ")";
      string str7 = " VALUES (" + str5.Substring(2) + ")";
      objDataAccess.WriteTransactionData(str6 + str7, CommandType.Text);
    }

    public void WRITE_IDOC_SOS_TdS(DataLayer objDataAccess, int CODSOS, string OPERA)
    {
      DataTable dataTable1 = new DataTable();
      string strSQL = " SELECT CODSOS, DENSOS, '" + OPERA + "' AS OPERA " + " FROM CODSOS " + " WHERE CODSOS = " + CODSOS.ToString();
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL);
      this.WRITE_IDOC_TESTATA_TdS(objDataAccess, dataTable2.Rows[0]);
      this.WRITE_IDOC_E1PITYP_TdS(objDataAccess, "ZE1CODSOS", dataTable2.Rows[0]);
    }

    public void WRITE_IDOC_NATGIU_TdS(DataLayer objDataAccess, int NATGIU, string OPERA)
    {
      DataTable dataTable1 = new DataTable();
      string strSQL = " SELECT CASE LENGTH(TRIM(CHAR(NATGIU))) WHEN 1 THEN '0' || TRIM(CHAR(NATGIU)) ELSE TRIM(CHAR(NATGIU)) END AS NATGIU, DENNATGIU, '" + OPERA + "' AS OPERA FROM NATGIU " + " WHERE NATGIU = " + NATGIU.ToString();
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL);
      this.WRITE_IDOC_TESTATA_TdS(objDataAccess, dataTable2.Rows[0]);
      this.WRITE_IDOC_E1PITYP_TdS(objDataAccess, "ZE1NATGIU", dataTable2.Rows[0]);
    }

    public void WRITE_IDOC_REG_TdS(DataLayer objDataAccess, int CODREG, string OPERA)
    {
      DataTable dataTable1 = new DataTable();
      string strSQL = " SELECT CODREG, DENREG, '' AS DENREGBRV, '" + OPERA + "' AS OPERA " + " FROM CODREG " + " WHERE CODREG = " + CODREG.ToString();
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL);
      if (dataTable2.Rows[0]["DENREG"].ToString().Trim().Length > 3)
        dataTable2.Rows[0]["DENREGBRV"] = (object) dataTable2.Rows[0]["DENREG"].ToString().Trim().Substring(0, 3);
      this.WRITE_IDOC_TESTATA_TdS(objDataAccess, dataTable2.Rows[0]);
      this.WRITE_IDOC_E1PITYP_TdS(objDataAccess, "ZE1T5ITNS", dataTable2.Rows[0]);
    }

    public void WRITE_IDOC_TIPRAP_TdS(DataLayer objDataAccess, int TIPRAP, string OPERA)
    {
      DataTable dataTable1 = new DataTable();
      string strSQL = " SELECT TIPRAP, DENTIPRAP, '' AS FLGTER, '" + OPERA + "' AS OPERA " + " FROM TIPRAP " + " WHERE TIPRAP = " + TIPRAP.ToString();
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL);
      if (Convert.ToInt32(dataTable2.Rows[0][nameof (TIPRAP)].ToString().Trim()) == 2 | Convert.ToInt32(dataTable2.Rows[0][nameof (TIPRAP)].ToString().Trim()) == 4)
        dataTable2.Rows[0]["FLGTER"] = (object) "X";
      this.WRITE_IDOC_TESTATA_TdS(objDataAccess, dataTable2.Rows[0]);
      this.WRITE_IDOC_E1PITYP_TdS(objDataAccess, "ZE1T547V", dataTable2.Rows[0]);
    }

    public void WRITE_IDOC_PROV_TdS(DataLayer objDataAccess, string SIGPRO, string OPERA)
    {
      DataTable dataTable1 = new DataTable();
      string strSQL = " SELECT SIGPRO, DENPRO, '" + OPERA + "' AS OPERA " + " FROM SIGPRO " + " WHERE SIGPRO = '" + SIGPRO + "'";
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL);
      this.WRITE_IDOC_TESTATA_TdS(objDataAccess, dataTable2.Rows[0]);
      this.WRITE_IDOC_E1PITYP_TdS(objDataAccess, "ZE1T005S", dataTable2.Rows[0]);
    }

    public void WRITE_IDOC_CODSTALEG_TdS(DataLayer objDataAccess, string CODSTALEG, string OPERA)
    {
      DataTable dataTable1 = new DataTable();
      string strSQL = " SELECT CODSTALEG, DENSTALEG, '" + OPERA + "' AS OPERA " + " FROM CODSTALEG " + " WHERE CODSTALEG = '" + CODSTALEG + "'";
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL);
      this.WRITE_IDOC_TESTATA_TdS(objDataAccess, dataTable2.Rows[0]);
      this.WRITE_IDOC_E1PITYP_TdS(objDataAccess, "ZE1CONCORS", dataTable2.Rows[0]);
    }

    public void WRITE_IDOC_CODTIT_TdS(DataLayer objDataAccess, string CODTIT, string OPERA)
    {
      DataTable dataTable1 = new DataTable();
      string strSQL = " SELECT CODTIT, DENTIT, '" + OPERA + "' AS OPERA " + " FROM CODTIT " + " WHERE CODTIT = '" + CODTIT + "'";
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL);
      this.WRITE_IDOC_TESTATA_TdS(objDataAccess, dataTable2.Rows[0]);
      this.WRITE_IDOC_E1PITYP_TdS(objDataAccess, "?????????", dataTable2.Rows[0]);
    }

    public void WRITE_IDOC_LOC_TdS(DataLayer objDataAccess, string CODCOM, string OPERA)
    {
      DataTable dataTable1 = new DataTable();
      string strSQL1 = " SELECT CODCOM.CODCOM, CODCOM.DENCOM , CODCOM.SIGPRO, CODSTA, " + " CODREG.DENREG, '' AS DENREGBRV, '" + OPERA + "' AS OPERA, '00000' AS CAP " + " FROM CODCOM LEFT JOIN SIGPRO ON " + " CODCOM.SIGPRO = SIGPRO.SIGPRO LEFT JOIN CODREG ON " + " SIGPRO.CODREG = CODREG.CODREG " + " WHERE CODCOM = '" + CODCOM + "'";
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL1);
      if (dataTable2.Rows[0]["CODSTA"].ToString().Trim() != "IT")
        dataTable2.Rows[0]["DENREGBRV"] = (object) "EEE";
      else if (dataTable2.Rows[0]["DENREG"].ToString().Trim().Length > 3)
        dataTable2.Rows[0]["DENREGBRV"] = (object) dataTable2.Rows[0]["DENREG"].ToString().Trim().Substring(0, 3);
      this.WRITE_IDOC_TESTATA_TdS(objDataAccess, dataTable2.Rows[0]);
      this.WRITE_IDOC_E1PITYP_TdS(objDataAccess, "ZE1T5ITM4", dataTable2.Rows[0]);
      string strSQL2 = " SELECT CODCOM, DENCOM, '" + OPERA + "' AS OPERA,  ULTAGG AS DATINI " + " FROM CODCOM " + " WHERE CODCOM = '" + CODCOM + "'";
      dataTable2.Clear();
      DataTable dataTable3 = objDataAccess.GetDataTable(strSQL2);
      dataTable3.Rows[0]["DENCOM"] = (object) ("Tassa comunale di " + dataTable3.Rows[0]["DENCOM"].ToString().Trim());
      dataTable3.Rows[0]["DATINI"] = (object) dataTable3.Rows[0]["DATINI"].ToString().Substring(0, 10);
      this.WRITE_IDOC_E1PITYP_TdS(objDataAccess, "ZE1T5ITM5", dataTable3.Rows[0]);
      string strSQL3 = " SELECT CODCOM, '" + OPERA + "' AS OPERA,  ULTAGG AS DATINI " + " FROM CODCOM " + " WHERE CODCOM = '" + CODCOM + "'";
      dataTable3.Clear();
      DataTable dataTable4 = objDataAccess.GetDataTable(strSQL3);
      dataTable4.Rows[0]["DATINI"] = (object) (Convert.ToDateTime(dataTable4.Rows[0]["DATINI"]).Year.ToString() + "-01-01");
      this.WRITE_IDOC_E1PITYP_TdS(objDataAccess, "ZE1T5ITM6", dataTable4.Rows[0]);
    }

    public void WRITE_IDOC_E1PITYP_TdS(
      DataLayer objDataAccess,
      string VAR_E1PITYP,
      DataRow ROW_DATI)
    {
      DataTable dataTable1 = new DataTable();
      string strSQL = "SELECT IDTAB, TABSAP FROM IDOCTAB WHERE INFTYP = '" + VAR_E1PITYP + "' AND VALUE(USATO, 'S') = 'S'";
      dataTable1.Clear();
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL);
      this.WRITE_EDID4_TdS(objDataAccess, Convert.ToInt32(dataTable2.Rows[0]["IDTAB"]), dataTable2.Rows[0]["TABSAP"].ToString(), VAR_E1PITYP, ref ROW_DATI);
    }

    public void WRITE_IDOC_TB_CONTRATTI_TdS(
      DataLayer objDataAccess,
      string CODCON,
      string PROCON,
      string OPERA)
    {
      DataTable dataTable1 = new DataTable();
      string strSQL = " SELECT TB_CONTRATTI.CODCON AS CODICE, TB_CONTRATTI.PROCON AS PROGRESSIVO, TB_CONTRATTO_LIVELLI.CODLIV," + " 1 AS TIPCON, TB_CONTRATTI.DATINI, TB_CONTRATTI.DATFIN," + " TB_CONTRATTI.ASSCON, TB_CONTRATTI.DENCON, TB_CONTRATTI.MAXSCA, TB_CONTRATTI.PERSCA," + " TB_CONTRATTI.NUMMEN," + " TB_CONTRATTI.RIVAUT, '" + OPERA + "' AS OPERA " + " FROM TB_CONTRATTI, TB_CONTRATTO_LIVELLI WHERE" + " TB_CONTRATTO_LIVELLI.CODCON = TB_CONTRATTI.CODCON" + " AND TB_CONTRATTO_LIVELLI.PROCON = TB_CONTRATTI.PROCON" + " AND TB_CONTRATTI.CODCON = " + CODCON + " AND TB_CONTRATTI.PROCON = " + PROCON + " ORDER BY TB_CONTRATTI.CODCON, TB_CONTRATTI.PROCON, TB_CONTRATTO_LIVELLI.CODLIV";
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL);
      for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
      {
        dataTable2.Rows[index]["IMPMIN"] = (object) 0;
        dataTable2.Rows[index]["RIVAUT"] = !(dataTable2.Rows[index]["RIVAUT"].ToString().Trim() == "S") ? (object) "" : (object) "X";
        dataTable2.Rows[index]["ASSCON"] = !(dataTable2.Rows[index]["ASSCON"].ToString().Trim() == "S") ? (object) "" : (object) "X";
        if (index == 0)
          this.WRITE_IDOC_TESTATA_TdS(objDataAccess, dataTable2.Rows[index]);
        this.WRITE_IDOC_E1PITYP_TdS(objDataAccess, "ZE1HRCCNL", dataTable2.Rows[index]);
      }
    }

    public void WRITE_IDOC_CONLOC_TdS(
      DataLayer objDataAccess,
      string CODLOC,
      string PROLOC,
      string CODCON,
      string PROCON,
      string OPERA)
    {
      DataTable dataTable1 = new DataTable();
      string strSQL1 = " SELECT CONLOC.CODCON, CONLOC.PROCON, " + " CONLOC.CODLOC AS CODICE, CONLOC.PROLOC AS PROGRESSIVO, CONLIV.CODLIV, " + " 2 AS TIPCON, CONLOC.DATAPP, CONLOC.DATINI, CONLOC.DATFIN," + " CONLOC.ASSCON, CONLOC.DENCON, CONLOC.MAXSCA, CONLOC.DATDEC, CONLOC.PERSCA," + " CONLOC.NUMMEN, CONLOC.M14, CONLOC.M15, CONLOC.M16," + " CONLOC.RIVAUT, CONLOC.TIPSPE, 0 AS CODQUACON, 0.0 AS IMPMIN, '" + OPERA + "' AS OPERA " + " FROM CONLOC, CONLIV WHERE" + " CONLIV.CODCON = CONLOC.CODCON" + " AND CONLIV.PROCON = CONLOC.PROCON" + " AND CONLOC.CODCON = " + CODCON + " AND CONLOC.PROCON = " + PROCON + " AND CONLOC.CODLOC = " + CODLOC + " AND CONLOC.PROLOC = " + PROLOC + " ORDER BY CONLOC.CODCON, CONLOC.PROCON, CONLOC.CODLOC, CONLOC.PROLOC, CONLIV.CODLIV";
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL1);
      for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
      {
        string strSQL2 = " SELECT VALUE(CODQUACON, 0) FROM CONRIF WHERE" + " CODCON = " + dataTable2.Rows[index][nameof (CODCON)]?.ToString() + " AND PROCON = " + dataTable2.Rows[index][nameof (PROCON)]?.ToString();
        int int32 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL2, CommandType.Text));
        dataTable2.Rows[index]["CODQUACON"] = (object) int32;
        dataTable2.Rows[index]["IMPMIN"] = (object) 0;
        dataTable2.Rows[index]["RIVAUT"] = !(dataTable2.Rows[index]["RIVAUT"].ToString().Trim() == "S") ? (object) "" : (object) "X";
        dataTable2.Rows[index]["ASSCON"] = !(dataTable2.Rows[index]["ASSCON"].ToString().Trim() == "S") ? (object) "" : (object) "X";
        if (index == 0)
          this.WRITE_IDOC_TESTATA_TdS(objDataAccess, dataTable2.Rows[index]);
        this.WRITE_IDOC_E1PITYP_TdS(objDataAccess, "ZE1HRCCNL", dataTable2.Rows[index]);
      }
    }

    public void WRITE_IDOC_TESTATA_TdS(DataLayer objDataAccess, DataRow ROW_DATI)
    {
      string DOCNUM = "";
      string str1 = "";
      string MANDT = "";
      string PSGNUM = "";
      string HLEVEL = "";
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      DataTable dataTable3 = new DataTable();
      this.IDOC_CONT_SERV = Convert.ToInt32(this.Module_GetCONT_TdS(objDataAccess));
      if (this.objDtCONTIDOC_SERV == null)
      {
        this.objDtCONTIDOC_SERV = new DataTable();
        this.objDtCONTIDOC_SERV.Columns.Add("CONTIDOC");
      }
      this.objDtCONTIDOC_SERV.Rows.Add(this.objDtCONTIDOC_SERV.NewRow());
      this.objDtCONTIDOC_SERV.Rows[this.objDtCONTIDOC_SERV.Rows.Count - 1]["CONTIDOC"] = (object) this.IDOC_CONT_SERV;
      this.SEGNUM = "";
      string strSQL1 = "SELECT * FROM IDOCTAB WHERE IDTAB = 30 AND VALUE(USATO, 'S') = 'S' ORDER BY ORDINE";
      dataTable1.Clear();
      DataTable dataTable4 = objDataAccess.GetDataTable(strSQL1);
      for (int index1 = 0; index1 <= dataTable4.Rows.Count - 1; ++index1)
      {
        string str2 = "";
        string str3 = "";
        string strSQL2 = "SELECT * FROM IDOCCAMPI WHERE IDTAB = " + dataTable4.Rows[index1]["IDTAB"]?.ToString() + " ORDER BY ORDINE";
        dataTable2.Clear();
        dataTable2 = objDataAccess.GetDataTable(strSQL2);
        this.SEGNUM = this.Module_GetSEGNUM_TdS(objDataAccess, this.IDOC_CONT_SERV);
        for (int index2 = 0; index2 <= dataTable2.Rows.Count - 1; ++index2)
        {
          str2 = str2 + ", " + dataTable2.Rows[index2]["CAMPO"]?.ToString();
          string upper1 = dataTable2.Rows[index2]["CAMPO"].ToString().Trim().ToUpper();
          if (!(upper1 == "MANDT"))
          {
            if (upper1 == "DOCNUM")
              DOCNUM = this.GetFormato_TdS(dataTable2.Rows[index2]["FORMATO"].ToString(), this.IDOC_CONT_SERV.ToString(), Convert.ToInt32(dataTable2.Rows[index2]["LUNGHEZZA"]));
          }
          else
            MANDT = this.GetFormato_TdS(dataTable2.Rows[index2]["FORMATO"].ToString(), dataTable2.Rows[index2]["VALORE"].ToString(), Convert.ToInt32(dataTable2.Rows[index2]["LUNGHEZZA"]));
          string upper2 = dataTable2.Rows[index2]["VALORE"].ToString().Trim().ToUpper();
          if (!(upper2 == "[CONTATORE]"))
          {
            str3 = upper2 == "IDOCVALORI" ? str3 + ", " + this.GetStrValues_TdS(objDataAccess, Convert.ToInt32(dataTable2.Rows[index2]["IDCAMPI"]), ref ROW_DATI, dataTable4.Rows[index1]["INFTYP"].ToString().Trim()) : str3 + ", " + this.GetFormato_TdS(dataTable2.Rows[index2]["FORMATO"].ToString(), dataTable2.Rows[index2]["VALORE"].ToString(), Convert.ToInt32(dataTable2.Rows[index2]["LUNGHEZZA"]));
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
                      str1 = this.IDOC_CONT_SERV.ToString();
                  }
                  else
                    str1 = HLEVEL;
                }
                else
                {
                  this.Module_GetPSGNUM_HLEVEL_TdS(objDataAccess, Convert.ToInt32(dataTable4.Rows[index1]["IDTAB"]), MANDT, DOCNUM, ref HLEVEL, ref PSGNUM);
                  str1 = PSGNUM;
                }
              }
              else
                str1 = this.IDOC_CONT_SERV.ToString();
            }
            else
              str1 = this.SEGNUM;
            str3 = str3 + ", " + this.GetFormato_TdS(dataTable2.Rows[index2]["FORMATO"].ToString(), str1.ToString(), Convert.ToInt32(dataTable2.Rows[index2]["LUNGHEZZA"]));
          }
        }
        string str4 = str2 + ", CONT_NUM ";
        string str5 = str3 + ", " + this.IDOC_CONT_SERV.ToString() + " ";
        string str6 = "INSERT INTO " + dataTable4.Rows[index1]["TABSAP"].ToString().Trim() + " (" + str4.Substring(2) + ")";
        string str7 = " VALUES (" + str5.Substring(2) + ")";
        objDataAccess.WriteTransactionData(str6 + str7, CommandType.Text);
      }
    }

    private string Module_GetCONT_TdS(DataLayer objDataAccess)
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

    private void Module_GetPSGNUM_HLEVEL_TdS(
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
      string strSQL2 = "SELECT SEGNUM FROM EDID4 WHERE CONT_NUM = " + DBMethods.DoublePeakForSql(this.IDOC_CONT_SERV.ToString().Trim()) + " AND MANDT = " + MANDT + " AND DOCNUM = " + DOCNUM + " AND SEGNAM = " + DBMethods.DoublePeakForSql(str);
      PSGNUM = objDataAccess.Get1ValueFromSQL(strSQL2, CommandType.Text);
    }

    private string Module_GetSEGNUM_TdS(DataLayer objDataAccess, int CONT)
    {
      DataTable dataTable = new DataTable();
      string strSQL = "SELECT VALUE(MAX(SEGNUM),0) + 1 AS SEGNUM FROM EDID4 WHERE CONT_NUM = " + CONT.ToString();
      return objDataAccess.Get1ValueFromSQL(strSQL, CommandType.Text);
    }

    private string GetStrValues_TdS(
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
        string str2 = !(dataTable2.Rows[index]["CAMPOSAP"].ToString().Trim().ToUpper() == "INFTY") ? (!(dataTable2.Rows[index]["VALORE"].ToString().Trim().ToUpper() == "DTDATI") ? dataTable2.Rows[index]["VALDEFAULT"].ToString().Trim() : (!(ROW_DATI[dataTable2.Rows[index]["VALDTDATI"].ToString()].ToString().Trim() == "") ? ROW_DATI[dataTable2.Rows[index]["VALDTDATI"].ToString()].ToString().Trim() : dataTable2.Rows[index]["VALDEFAULT"].ToString().Trim())) : INFTYP;
        str1 += this.GetFormato_TdS(dataTable2.Rows[index]["FORMATO"].ToString(), str2.ToString(), Convert.ToInt32(dataTable2.Rows[index]["LUNGHEZZA"]), false);
      }
      return "'" + str1.Replace("'", "''") + "'";
    }

    private string GetFormato_TdS(
      string FORMATO,
      string TESTO,
      int LUNGHEZZA,
      bool USE_DOUBLEPEAK = true)
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
          if (TESTO != "")
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
          if (TESTO != "" & TESTO != "99991231" & TESTO != "18000101")
          {
            TESTO = DBMethods.Db2Date(TESTO);
            TESTO = TESTO.Replace("-", "");
          }
          return TESTO.PadRight(LUNGHEZZA, ' ');
        default:
          return !USE_DOUBLEPEAK ? TESTO.ToString().Trim().PadRight(LUNGHEZZA, ' ') : DBMethods.DoublePeakForSql(TESTO.ToString().Trim().PadRight(LUNGHEZZA, ' '));
      }
    }

    public void Aggiorna_IDOC_SERV(DataLayer objDataAccess)
    {
      for (short index = 0; (int) index <= this.objDtCONTIDOC_SERV.Rows.Count - 1; ++index)
      {
        string strSQL = "UPDATE EDIDC SET STATUS = 64 WHERE STATUS = 00 AND CONT_NUM = '" + this.objDtCONTIDOC_SERV.Rows[(int) index]["CONTIDOC"]?.ToString() + "'";
        objDataAccess.WriteTransactionData(strSQL, CommandType.Text);
      }
    }
  }
}
