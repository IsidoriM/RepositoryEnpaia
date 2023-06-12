// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Utilities.ModGetDati
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using System;
using System.Collections.Generic;
using System.Data;
using TFI.DAL.ConnectorDB;
using TFI.OCM.Amministrativo;

namespace TFI.DAL.Utilities
{
  public class ModGetDati
  {
    public static Decimal TOTALE_ALTREIMPRESE_AD = 2316735.12M;
    public static Decimal TOTSOC = 985977.93M;
    public static Decimal TOTALE_BONIFICA_AD = 338696.61M;
    public static Decimal TOTALE_ALTREIMPRESE = 58078805.98M;
    public static Decimal TOTALE_BONIFICA = 8467415.43M;
    public static Decimal TOTALE_CONSORZI_AD = 2229414.55M;
    public static Decimal TOTALE_CONSORZI = 55735363.77M;
    private Utile Utils = new Utile();
    private clsWRITE_DB clsWRITE_DB = new clsWRITE_DB();

    public string Module_Get_CAUSALE_MOVIMENTO(DataLayer objDataAccess, string TIPMOV) => objDataAccess.Get1ValueFromSQL("SELECT CODCAU FROM TIPMOVCAU WHERE TIPMOV = " + DBMethods.DoublePeakForSql(TIPMOV) + " AND CURRENT_DATE BETWEEN DATINI AND DATFIN", CommandType.Text).ToString().Trim();

    public string Module_Get_NumeroSapMovimento(int NUMERO_RIF, string CODCAU, int ANNO)
    {
      if (ANNO < 2003)
        return CODCAU.PadLeft(2, '0') + "/0000/0" + NUMERO_RIF.ToString().Trim();
      return CODCAU.PadLeft(2, '0') + "/" + ANNO.ToString() + "/" + NUMERO_RIF.ToString().Trim();
    }

    public string Module_Get_NumeroRicevuta(DataLayer objDataAccess, string TIPMOV)
    {
      DataTable dataTable1 = new DataTable();
      DateTime dataSistema = this.Utils.Module_GetDataSistema();
      string strSQL = "SELECT VALUE(MAX( DECIMAL( VALUE( SUBSTR(NUMRIC, 6), '0'))), 0) AS NUMRIC " + " FROM DENTES ";
      string str1 = TIPMOV.Trim();
      int year;
      if (!(str1 == "NU"))
      {
        if (str1 == "DP" || str1 == "AR")
        {
          string str2 = strSQL;
          year = dataSistema.Year;
          string str3 = year.ToString();
          strSQL = str2 + " WHERE YEAR(DATCHI) = " + str3;
        }
      }
      else
      {
        string str4 = strSQL;
        year = dataSistema.Year;
        string str5 = year.ToString();
        strSQL = str4 + " WHERE YEAR(DATCONMOV) = " + str5;
      }
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL);
      int num = dataTable2.Rows.Count != 0 ? Convert.ToInt32(dataTable2.Rows[0]["NUMRIC"]) + 1 : 1;
      year = dataSistema.Year;
      return year.ToString() + "/" + num.ToString();
    }

    public DataTable Module_GetDatiCompleti_RAPLAV(
      DataLayer objDataAccess,
      int CODPOS,
      int MAT,
      int PRORAP)
    {
      DataTable dataTable = new DataTable();
      string strSQL = "SELECT CODPOS, (SELECT RAGSOC FROM AZI WHERE AZI.CODPOS = RAPLAV.CODPOS)  AS RAGSOC, MAT, PRORAP, " + " DATDEC, " + " VALUE(CODCAUCES, 0) AS CODCAUCE, " + " CASE DATCES WHEN '2100-12-31' THEN NULL " + " WHEN '9999-12-31' THEN NULL " + " ELSE DATCES END DATCES, " + " (SELECT DENCES FROM CAUCES WHERE CAUCES.CODCAUCES = VALUE(RAPLAV.CODCAUCES, 0)) AS DENCES, " + " DATDENCES, DATASS, DATDEN, DATPRE, DATPRO, " + " NUMPRO, MATFON, PROFON, TIPFONDO, ULTAGG, UTEAGG, " + " CODPOSFOR, RAGSOCFOR, PARIVAFOR, CODFISFOR, INDFOR, TELFOR, " + " CODDUGFOR, (SELECT DENDUG FROM DUG WHERE DUG.CODDUG = VALUE(RAPLAV.CODDUGFOR, 0)) AS DENDUGFOR, " + " NUMCIVFOR, DENLOCFOR, CAPFOR, SIGPROFOR, " + " CODCOMFOR, " + " DATLIQ, DATPAG,DATSAP, DATINPS, CODPOSDA, CODPOSA " + " FROM RAPLAV " + " WHERE CODPOS = " + CODPOS.ToString() + " AND MAT= " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString();
      return objDataAccess.GetDataTable(strSQL);
    }

    public DataTable Module_GetDatiCompleti_STORDL(
      DataLayer objDataAccess,
      int CODPOS,
      int MAT,
      int PRORAP,
      string ErroreMSG,
      string strData = "",
      string strData_Per_MinimoContrattuale = "",
      string strOrdinamento = "DESC",
      string strTABELLABK = "")
    {
      DataTable dataTable1 = new DataTable();
      this.Utils.Module_GetDataSistema().ToString();
      string strDataNascita = objDataAccess.Get1ValueFromSQL("SELECT DATNAS FROM ISCT WHERE MAT = " + MAT.ToString(), CommandType.Text);
      objDataAccess.Get1ValueFromSQL("SELECT DATDEC FROM RAPLAV WHERE CODPOS = " + CODPOS.ToString() + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString(), CommandType.Text);
      string str1 = "SELECT " + strTABELLABK + "STORDL.CODPOS, " + strTABELLABK + "STORDL.MAT, " + strTABELLABK + "STORDL.PRORAP, " + " DATINI, " + " CASE DATFIN WHEN '2100-12-31' THEN NULL " + " WHEN '9999-12-31' THEN NULL " + " ELSE DATFIN END DATFIN, " + " CHAR(" + strTABELLABK + "STORDL.DATSCATER) AS DATSCATER, " + " " + strTABELLABK + "STORDL.TIPRAP, (SELECT DENTIPRAP FROM TIPRAP WHERE TIPRAP=" + strTABELLABK + "STORDL.TIPRAP) AS DENTIPRAP, " + " " + strTABELLABK + "STORDL.CODLIV, '' as DENLIV, " + " '' AS DENCON, " + " VALUE(" + strTABELLABK + "STORDL.CODCON, 0) AS CODCON, 0 AS PROCON, " + " VALUE(" + strTABELLABK + "STORDL.CODLOC, 0) AS CODLOC, 0 AS PROLOC, " + " 0.00 AS MINCON, " + " 0.00 AS MINCON_OGGI, " + " 0 AS CODQUACON, '' AS DENQUA, " + " '' AS TIPSPE, " + " '' AS DENTIPSPE, " + " VALUE(" + strTABELLABK + "STORDL.TRAECO,0.00) AS TRAECO, " + " " + strTABELLABK + "STORDL.NUMMEN, " + " 0.00 AS IMPAGG12, " + " 0.00 AS IMPAGG13, " + " 0.00 AS IMPAGG14, " + " " + strTABELLABK + "STORDL.MESMEN14, " + " 0.00 AS IMPAGG15, " + " " + strTABELLABK + "STORDL.MESMEN15, " + " 0.00 AS IMPAGG16, " + " " + strTABELLABK + "STORDL.MESMEN16, " + " VALUE(" + strTABELLABK + "STORDL.PERAPP, 0.00) AS PERAPP, " + " VALUE(" + strTABELLABK + "STORDL.PERPAR, 0.00) AS PERPAR, " + " FAP, " + " CODGRUASS, " + " '' AS DENGRUASS, " + " 0.00 AS ALIQUOTA, " + " VALUE(" + strTABELLABK + "STORDL.NUMSCAMAT,0) AS NUMSCA, " + " " + strTABELLABK + "STORDL.DATULTSCA AS DATSCA, " + " " + strTABELLABK + "STORDL.DATNEWSCA AS DATPROSCA, " + " VALUE(" + strTABELLABK + "STORDL.IMPSCAMAT,0.00) AS IMPSCA, " + " 0.00 AS RETTOT, " + " ABBPRE, " + " ASSCON, ALIQTFR, ALIQFP, ALIQINF, DTTFR, DTINF, DTFP, " + " 0 AS PROPAR, " + " '' AS GENNAIO, " + " '' AS FEBBRAIO, " + " '' AS MARZO, " + " '' AS APRILE, " + " '' AS MAGGIO, " + " '' AS GIUGNO, " + " '' AS LUGLIO, " + " '' AS AGOSTO, " + " '' AS SETTEMBRE, " + " '' AS OTTOBRE, " + " '' AS NOVEMBRE, " + " '' AS DICEMBRE, " + " " + strTABELLABK + "STORDL.ULTAGG, " + strTABELLABK + "STORDL.UTEAGG" + " FROM " + strTABELLABK + "STORDL " + " WHERE CODPOS = '" + CODPOS.ToString() + "' " + " AND MAT= '" + MAT.ToString() + "' " + " AND PRORAP = '" + PRORAP.ToString() + "' ";
      if (!string.IsNullOrEmpty(strData))
        str1 = str1 + " AND DATINI = " + DBMethods.Db2Date(strData);
      string strSQL1 = str1 + " ORDER BY " + strTABELLABK + "STORDL.DATINI " + strOrdinamento;
      DataTable dataTable2 = objDataAccess.GetDataTable(strSQL1);
      DataTable dataTable3 = new DataTable();
      for (int index1 = 0; index1 <= dataTable2.Rows.Count - 1; ++index1)
      {
        if (string.IsNullOrEmpty(dataTable2.Rows[index1]["CODGRUASS"].ToString()))
          dataTable2.Rows[index1]["CODGRUASS"] = (object) 0;
        if (string.IsNullOrEmpty(dataTable2.Rows[index1]["TIPRAP"].ToString()))
          dataTable2.Rows[index1]["TIPRAP"] = (object) 0;
        int index2;
        switch (Convert.ToInt32(dataTable2.Rows[index1]["TIPRAP"]))
        {
          case 3:
          case 4:
          case 8:
          case 10:
          case 11:
          case 16:
          case 17:
            if (Convert.ToInt32(dataTable2.Rows[index1]["TIPRAP"]) == 11 | Convert.ToInt32(dataTable2.Rows[index1]["TIPRAP"]) == 17 | Convert.ToInt32(dataTable2.Rows[index1]["TIPRAP"]) == 16)
            {
              string strSQL2 = "SELECT PARMES FROM " + strTABELLABK + "PARTIMM WHERE CODPOS = '" + CODPOS.ToString() + "'  AND MAT = '" + MAT.ToString() + "' AND PRORAP = '" + PRORAP.ToString() + "' AND DATINI = '" + DBMethods.Db2Date(dataTable2.Rows[index1]["DATINI"].ToString()) + "' ";
              dataTable3 = objDataAccess.GetDataTable(strSQL2);
              for (index2 = 0; index2 <= dataTable3.Rows.Count - 1; ++index2)
              {
                switch (dataTable3.Rows[index2]["PARMES"].ToString().Trim())
                {
                  case "1":
                    dataTable2.Rows[index1]["GENNAIO"] = (object) "S";
                    break;
                  case "10":
                    dataTable2.Rows[index1]["OTTOBRE"] = (object) "S";
                    break;
                  case "11":
                    dataTable2.Rows[index1]["NOVEMBRE"] = (object) "S";
                    break;
                  case "12":
                    dataTable2.Rows[index1]["DICEMBRE"] = (object) "S";
                    break;
                  case "2":
                    dataTable2.Rows[index1]["FEBBRAIO"] = (object) "S";
                    break;
                  case "3":
                    dataTable2.Rows[index1]["MARZO"] = (object) "S";
                    break;
                  case "4":
                    dataTable2.Rows[index1]["APRILE"] = (object) "S";
                    break;
                  case "5":
                    dataTable2.Rows[index1]["MAGGIO"] = (object) "S";
                    break;
                  case "6":
                    dataTable2.Rows[index1]["GIUGNO"] = (object) "S";
                    break;
                  case "7":
                    dataTable2.Rows[index1]["LUGLIO"] = (object) "S";
                    break;
                  case "8":
                    dataTable2.Rows[index1]["AGOSTO"] = (object) "S";
                    break;
                  case "9":
                    dataTable2.Rows[index1]["SETTEMBRE"] = (object) "S";
                    break;
                }
              }
              break;
            }
            break;
        }
        for (index2 = 12; index2 <= 16; ++index2)
        {
          string strSQL3 = "SELECT VALUE(IMPAGG, 0.00) AS IMPAGG FROM " + strTABELLABK + " IMPAGG WHERE CODPOS = '" + CODPOS.ToString() + "' AND MAT = '" + MAT.ToString() + "' AND PRORAP = '" + PRORAP.ToString() + "' AND DATINI = '" + DBMethods.Db2Date(dataTable2.Rows[index1]["DATINI"].ToString()) + "' AND MENAGG = '" + index2.ToString() + "' ";
          dataTable3.Clear();
          dataTable3 = objDataAccess.GetDataTable(strSQL3);
          switch (dataTable3.Rows.Count)
          {
            case 0:
              dataTable2.Rows[index1]["IMPAGG" + index2.ToString()] = (object) 0.0;
              break;
            case 1:
              dataTable2.Rows[index1]["IMPAGG" + index2.ToString()] = dataTable3.Rows[0]["IMPAGG"];
              break;
            default:
              ErroreMSG = "Attenzione... verificare incongruenza di dati nella tabella IMPAGG. Sono presenti più record per CODPOS = " + CODPOS.ToString() + " MAT = " + MAT.ToString() + " MENAGG = " + index2.ToString() + " alla data " + dataTable2.Rows[index1]["DATINI"]?.ToString();
              return dataTable2;
          }
        }
        if (!string.IsNullOrEmpty(dataTable2.Rows[index1]["DATSCA"].ToString()))
          dataTable2.Rows[index1]["DATSCA"] = (object) Convert.ToDateTime(dataTable2.Rows[index1]["DATSCA"]);
        if (!string.IsNullOrEmpty(dataTable2.Rows[index1]["DATPROSCA"].ToString()))
          dataTable2.Rows[index1]["DATPROSCA"] = (object) Convert.ToDateTime(dataTable2.Rows[index1]["DATPROSCA"]);
        if (!string.IsNullOrEmpty(dataTable2.Rows[index1]["DATSCATER"].ToString()))
          dataTable2.Rows[index1]["DATSCATER"] = (object) this.Utils.Module_CheckData_9999_12_31(dataTable2.Rows[index1]["DATSCATER"].ToString());
        dataTable3 = Convert.ToInt32(dataTable2.Rows[index1]["CODLOC"]) != 0 ? (!(strData_Per_MinimoContrattuale == "") ? this.Module_GetDatiContratto_Locale(objDataAccess, Convert.ToInt32(dataTable2.Rows[index1]["CODLOC"]), strData_Per_MinimoContrattuale) : this.Module_GetDatiContratto_Locale(objDataAccess, Convert.ToInt32(dataTable2.Rows[index1]["CODLOC"]), dataTable2.Rows[index1]["DATINI"].ToString())) : (!(strData_Per_MinimoContrattuale == "") ? this.Module_GetDatiContratto_Riferimento(objDataAccess, Convert.ToInt32(dataTable2.Rows[index1]["CODCON"]), strData_Per_MinimoContrattuale) : this.Module_GetDatiContratto_Riferimento(objDataAccess, Convert.ToInt32(dataTable2.Rows[index1]["CODCON"]), dataTable2.Rows[index1]["DATINI"].ToString()));
        if (dataTable3.Rows.Count > 0)
        {
          dataTable2.Rows[index1]["DENQUA"] = (object) dataTable3.Rows[0]["DENQUA"].ToString().Trim();
          dataTable2.Rows[index1]["DENCON"] = (object) dataTable3.Rows[0]["DENCON"].ToString().Trim();
          dataTable2.Rows[index1]["PROCON"] = (object) dataTable3.Rows[0]["PROCON"].ToString().Trim();
          dataTable2.Rows[index1]["CODLOC"] = (object) dataTable3.Rows[0]["CODLOC"].ToString().Trim();
          dataTable2.Rows[index1]["PROLOC"] = (object) dataTable3.Rows[0]["PROLOC"].ToString().Trim();
          dataTable2.Rows[index1]["CODQUACON"] = (object) dataTable3.Rows[0]["CODQUACON"].ToString().Trim();
          dataTable2.Rows[index1]["TIPSPE"] = (object) dataTable3.Rows[0]["TIPSPE"].ToString().Trim();
          string str2 = dataTable3.Rows[0]["TIPSPE"].ToString().Trim();
          if (!(str2 == "S"))
          {
            if (!(str2 == "M"))
            {
              if (str2 == "A")
                dataTable2.Rows[index1]["DENTIPSPE"] = (object) "CONTRATTO AZIENDALE";
            }
            else
              dataTable2.Rows[index1]["DENTIPSPE"] = (object) "AREA MINIMALE";
          }
          else
            dataTable2.Rows[index1]["DENTIPSPE"] = (object) "CONTRATTO MECCANIZZATO";
          string strSQL4 = "SELECT DENLIV FROM CONLIV WHERE CODCON = '" + dataTable2.Rows[index1]["CODCON"]?.ToString() + "'" + " AND PROCON = '" + dataTable2.Rows[index1]["PROCON"]?.ToString() + "' " + " AND CODLIV = '" + dataTable2.Rows[index1]["CODLIV"]?.ToString() + "' ";
          dataTable3.Clear();
          DataTable dataTable4 = objDataAccess.GetDataTable(strSQL4);
          if (dataTable4.Rows.Count > 0)
            dataTable2.Rows[index1]["DENLIV"] = dataTable4.Rows[0]["DENLIV"];
          dataTable2.Rows[index1]["MINCON"] = !(strData_Per_MinimoContrattuale == "") ? (object) this.Module_GetMinimoContrattuale(objDataAccess, Convert.ToInt32(dataTable2.Rows[index1]["CODCON"]), Convert.ToInt32(dataTable2.Rows[index1]["PROCON"]), Convert.ToInt32(dataTable2.Rows[index1]["CODLOC"]), Convert.ToInt32(dataTable2.Rows[index1]["PROLOC"]), Convert.ToInt32(dataTable2.Rows[index1]["CODLIV"]), strData_Per_MinimoContrattuale, Convert.ToDecimal(dataTable2.Rows[index1]["PERAPP"]), Convert.ToDecimal(dataTable2.Rows[index1]["PERPAR"])) : (object) this.Module_GetMinimoContrattuale(objDataAccess, Convert.ToInt32(dataTable2.Rows[index1]["CODCON"]), Convert.ToInt32(dataTable2.Rows[index1]["PROCON"]), Convert.ToInt32(dataTable2.Rows[index1]["CODLOC"]), Convert.ToInt32(dataTable2.Rows[index1]["PROLOC"]), Convert.ToInt32(dataTable2.Rows[index1]["CODLIV"]), dataTable2.Rows[index1]["DATINI"].ToString(), Convert.ToDecimal(dataTable2.Rows[index1]["PERAPP"]), Convert.ToDecimal(dataTable2.Rows[index1]["PERPAR"]));
          string str3 = dataTable2.Rows[index1]["TIPSPE"].ToString().Trim();
          if (!(str3 == "S"))
          {
            if (!(str3 == "M"))
            {
              if (str3 == "A")
              {
                dataTable2.Rows[index1]["MINCON"] = (object) 0.0;
                dataTable2.Rows[index1]["RETTOT"] = (object) (Convert.ToDecimal(dataTable2.Rows[index1]["TRAECO"]) + Convert.ToDecimal(dataTable2.Rows[index1]["IMPAGG12"]) + Convert.ToDecimal(dataTable2.Rows[index1]["IMPAGG13"]) + Convert.ToDecimal(dataTable2.Rows[index1]["IMPAGG14"]) + Convert.ToDecimal(dataTable2.Rows[index1]["IMPAGG15"]) + Convert.ToDecimal(dataTable2.Rows[index1]["IMPAGG16"]));
              }
            }
            else
              dataTable2.Rows[index1]["RETTOT"] = (object) (Convert.ToDecimal(dataTable2.Rows[index1]["TRAECO"]) + Convert.ToDecimal(dataTable2.Rows[index1]["IMPAGG12"]) + Convert.ToDecimal(dataTable2.Rows[index1]["IMPAGG13"]) + Convert.ToDecimal(dataTable2.Rows[index1]["IMPAGG14"]) + Convert.ToDecimal(dataTable2.Rows[index1]["IMPAGG15"]) + Convert.ToDecimal(dataTable2.Rows[index1]["IMPAGG16"]));
          }
          else
          {
            dataTable2.Rows[index1]["TRAECO"] = (object) 0.0;
            dataTable2.Rows[index1]["RETTOT"] = (object) (Convert.ToDecimal(dataTable2.Rows[index1]["MINCON"]) + Convert.ToDecimal(dataTable2.Rows[index1]["IMPSCA"]) + Convert.ToDecimal(dataTable2.Rows[index1]["IMPAGG12"]) + Convert.ToDecimal(dataTable2.Rows[index1]["IMPAGG13"]) + Convert.ToDecimal(dataTable2.Rows[index1]["IMPAGG14"]) + Convert.ToDecimal(dataTable2.Rows[index1]["IMPAGG15"]) + Convert.ToDecimal(dataTable2.Rows[index1]["IMPAGG16"]));
          }
          if (string.IsNullOrEmpty(dataTable2.Rows[index1]["CODGRUASS"].ToString()))
            dataTable2.Rows[index1]["CODGRUASS"] = (object) 0;
          string str4 = "Select VALUE(SUM(ALIQUOTA), 0.00) AS ALIQUOTA " + " FROM ALIFORASS " + " WHERE ALIFORASS.CODGRUASS = '" + dataTable2.Rows[index1]["CODGRUASS"]?.ToString() + "' " + " AND ALIFORASS.CODQUACON='" + dataTable2.Rows[index1]["CODQUACON"]?.ToString() + "' " + " AND '" + DBMethods.Db2Date(dataTable2.Rows[index1]["DATINI"].ToString()) + "'  BETWEEN ALIFORASS.DATINI AND VALUE(ALIFORASS.DATFIN,'9999-12-31') ";
          if (this.Utils.Module_Check_65Anni(Convert.ToDateTime(dataTable2.Rows[index1]["DATINI"]), strDataNascita))
            str4 += " AND ALIFORASS.CODFORASS IN (SELECT CODFORASS FROM FORASS WHERE CATFORASS <> 'PREV') ";
          string strSQL5 = str4 + " AND ALIFORASS.CODFORASS NOT IN (SELECT CODFORASS FROM FORASS WHERE CATFORASS = 'FAP') ";
          dataTable4.Clear();
          DataTable dataTable5 = objDataAccess.GetDataTable(strSQL5);
          dataTable2.Rows[index1]["ALIQUOTA"] = dataTable5.Rows[0]["ALIQUOTA"];
          if (dataTable2.Rows[index1]["FAP"] == (object) "S")
          {
            string strSQL6 = "SELECT VALFAP FROM CODFAP WHERE " + DBMethods.Db2Date(dataTable2.Rows[index1]["DATINI"].ToString()) + " BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31')";
            dataTable5.Clear();
            dataTable5 = objDataAccess.GetDataTable(strSQL6);
            if (dataTable5.Rows.Count > 0)
              dataTable2.Rows[index1]["ALIQUOTA"] = (object) (Convert.ToDecimal(dataTable2.Rows[index1]["ALIQUOTA"]) + Convert.ToDecimal(dataTable5.Rows[0]["VALFAP"]));
          }
          string strSQL7 = " SELECT GRUASS.CODGRUASS, GRUASS.DENGRUASS FROM GRUASS " + " WHERE GRUASS.CODGRUASS = '" + dataTable2.Rows[index1]["CODGRUASS"]?.ToString() + "' ";
          dataTable5.Clear();
          dataTable3 = objDataAccess.GetDataTable(strSQL7);
          if (dataTable3.Rows.Count > 0)
          {
            dataTable2.Rows[index1]["CODGRUASS"] = dataTable3.Rows[0]["CODGRUASS"];
            dataTable2.Rows[index1]["DENGRUASS"] = dataTable3.Rows[0]["DENGRUASS"];
          }
        }
      }
      return dataTable2;
    }

    public DataTable Module_GetDatiCompleti_ISCT(DataLayer objDataAccess, int MAT)
    {
      DataTable dataTable = new DataTable();
      string strSQL = "SELECT MAT, COG, NOM, DATNAS, " + " CODCOM AS CODCOMNAS, (SELECT DENCOM FROM CODCOM WHERE CODCOM.CODCOM = VALUE(ISCT.CODCOM,'@@@@@')) AS DENCOMNAS, " + " (SELECT SIGPRO FROM CODCOM WHERE CODCOM.CODCOM = VALUE(ISCT.CODCOM,'@@@@@')) AS SIGPRONAS, " + " CODFIS, SES, DATCHIISC, COUCHIISC, STACIV, (SELECT DENSTACIV FROM STACIV WHERE STACIV.CODSTACIV = VALUE(ISCT.STACIV ,0)) AS DENSTACIV, " + " TITSTU, (SELECT DENTITSTU FROM TITSTU WHERE TITSTU.CODTITSTU = VALUE(ISCT.TITSTU ,0)) AS DENTITSTU, ULTAGG, UTEAGG" + " FROM ISCT " + " WHERE MAT = " + MAT.ToString();
      return objDataAccess.GetDataTable(strSQL);
    }

    public DataTable Module_GetDatiCompleti_ISCD(
      DataLayer objDataAccess,
      int MAT,
      string Data = "")
    {
      DataTable dataTable = new DataTable();
      string str = "SELECT DATINI, " + " IND, URL, CO, EMAILCERT, CELL, NUMCIV, CODDUG, " + " (SELECT DENDUG FROM DUG WHERE DUG.CODDUG = ISCD.CODDUG) as DENDUG, " + " DENLOC, CAP, SIGPRO, DENSTAEST, " + " CODCOM, " + " TEL1, TEL2,FAX, EMAIL" + " FROM ISCD" + " WHERE MAT= " + MAT.ToString();
      string strSQL = !(Data != "") ? str + " ORDER BY DATINI" : str + " AND DATINI <= " + DBMethods.Db2Date(Data) + " ORDER BY DATINI DESC  FETCH FIRST 1 ROWS ONLY";
      return objDataAccess.GetDataTable(strSQL);
    }

    public DataTable Module_GetDatiContratto_Riferimento(
      DataLayer objDataAccess,
      int CODCON,
      string strData)
    {
      DataTable dataTable = new DataTable();
      string strSQL = "SELECT CONRIF.*, (SELECT DENQUA FROM QUACON WHERE CODQUACON = CONRIF.CODQUACON) AS DENQUA, 0 AS CODLOC, 0 AS PROLOC FROM CONRIF " + " WHERE CODCON = " + CODCON.ToString() + " AND DATDEC <= '" + DBMethods.Db2Date(strData) + "' " + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
      return objDataAccess.GetDataTable(strSQL);
    }

    public DataTable Module_GetDatiContratto_Locale(
      DataLayer objDataAccess,
      int CODLOC,
      string strData)
    {
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      string strSQL1 = "SELECT CONLOC.*, 0 AS CODQUACON, '' as DENQUA FROM CONLOC " + " WHERE CODLOC = '" + CODLOC.ToString() + "' " + " AND DATDEC <= '" + DBMethods.Db2Date(strData) + "' " + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
      DataTable dataTable3 = objDataAccess.GetDataTable(strSQL1);
      if (dataTable3.Rows.Count > 0)
      {
        string strSQL2 = "SELECT PROCON FROM CONRIF WHERE CODCON = '" + dataTable3.Rows[0]["CODCON"]?.ToString() + "' " + " AND DATDEC <= '" + DBMethods.Db2Date(strData) + "' " + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
        DataTable dataTable4 = objDataAccess.GetDataTable(strSQL2);
        if (dataTable4.Rows.Count > 0)
          dataTable3.Rows[0]["PROCON"] = dataTable4.Rows[0]["PROCON"];
        string strSQL3 = "SELECT CODQUACON, DENQUA FROM QUACON WHERE CODQUACON IN (SELECT CODQUACON FROM CONRIF WHERE CODCON = '" + dataTable3.Rows[0]["CODCON"]?.ToString() + "')";
        dataTable4.Clear();
        DataTable dataTable5 = objDataAccess.GetDataTable(strSQL3);
        if (dataTable5.Rows.Count > 0)
        {
          dataTable3.Rows[0]["CODQUACON"] = dataTable5.Rows[0]["CODQUACON"];
          dataTable3.Rows[0]["DENQUA"] = dataTable5.Rows[0]["DENQUA"];
        }
      }
      return dataTable3;
    }

    public Decimal Module_GetMinimoContrattuale(
      DataLayer objDataAccess,
      int CODCON,
      int PROCON,
      int CODLOC,
      int PROLOC,
      int CODLIV,
      string strData,
      Decimal PERAPP,
      Decimal PERPAR)
    {
      string strSQL1 = "SELECT VALUE(SUM(IMPVOCRET), 0) AS EMOLUMENTI FROM CONRET " + " WHERE CODCON = '" + CODCON.ToString() + "' " + " AND PROCON = '" + PROCON.ToString() + "' " + " AND CODLIV = '" + CODLIV.ToString() + "' " + " AND CODVOCRET <> 4 " + " AND DATAPPINI <= '" + DBMethods.Db2Date(strData) + "' " + " AND DATAPPFIN >= '" + DBMethods.Db2Date(strData) + "' ";
      Decimal minimoContrattuale = Convert.ToDecimal(objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text));
      if (CODLOC > 0)
      {
        string strSQL2 = "SELECT VALUE(SUM(IMPVOCRET), 0) AS EMOLUMENTI FROM LOCRET " + " WHERE CODLOC = '" + CODLOC.ToString() + "' " + " AND PROLOC = '" + PROLOC.ToString() + "' " + " AND CODLIV ='" + CODLIV.ToString() + "' " + " AND CODVOCRET <> 4 " + " AND DATAPPINI <= '" + DBMethods.Db2Date(strData) + "' " + " AND DATAPPFIN >= '" + DBMethods.Db2Date(strData) + "' ";
        minimoContrattuale += Convert.ToDecimal(objDataAccess.Get1ValueFromSQL(strSQL2, CommandType.Text));
      }
      if (PERAPP > 0M)
        minimoContrattuale = minimoContrattuale * PERAPP / 100M;
      if (PERPAR > 0M)
        minimoContrattuale = minimoContrattuale * PERPAR / 100M;
      return minimoContrattuale;
    }

    public void Module_Carica_DUG(
      DataLayer objDataAccess,
      GestioneRapportiLavoroIscrittiOCM rdl,
      string strRic = "",
      bool ConPrimaVoceVuota = false)
    {
      DataTable dataTable = new DataTable();
      List<GestioneRapportiLavoroIscrittiOCM.Indirizzi> indirizziList = new List<GestioneRapportiLavoroIscrittiOCM.Indirizzi>();
      foreach (DataRow row in (InternalDataCollectionBase) objDataAccess.GetDataTable("SELECT CODDUG, DENDUG FROM DUG " + strRic + " ORDER BY DENDUG").Rows)
      {
        GestioneRapportiLavoroIscrittiOCM.Indirizzi indirizzi = new GestioneRapportiLavoroIscrittiOCM.Indirizzi()
        {
          dendug = row["DENDUG"].ToString(),
          coddug = row["CODDUG"].ToString()
        };
        indirizziList.Add(indirizzi);
      }
      rdl.listaIndirizzi = indirizziList;
    }

    public void Module_Carica_TIPRAP(
      DataLayer objDataAccess,
      GestioneRapportiLavoroIscrittiOCM rdl,
      string strRic = "")
    {
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = objDataAccess.GetDataTable("SELECT TIPRAP, DENTIPRAP FROM TIPRAP " + strRic + " ORDER BY DENTIPRAP");
      List<GestioneRapportiLavoroIscrittiOCM.TipoRapLav> tipoRapLavList = new List<GestioneRapportiLavoroIscrittiOCM.TipoRapLav>();
      foreach (DataRow row in (InternalDataCollectionBase) dataTable2.Rows)
      {
        GestioneRapportiLavoroIscrittiOCM.TipoRapLav tipoRapLav = new GestioneRapportiLavoroIscrittiOCM.TipoRapLav()
        {
          tiprap = row["TIPRAP"].ToString(),
          dentiprap = row["DENTIPRAP"].ToString()
        };
        tipoRapLavList.Add(tipoRapLav);
      }
      rdl.listTipRap = tipoRapLavList;
    }

    public Decimal MODULE_GENERA_SANZIONE(
      DataLayer objDataAccess,
      ref Decimal PERMAXSOGLIA,
      Decimal IMPORTO_RETRIBUZIONE,
      ref Decimal TASSO,
      Decimal ALIQUOTA,
      string TIPMOVSAN,
      string DATINISAN,
      string DATFINSAN,
      ref string CODCAUSAN,
      string DATRETT = "",
      int ANNO = 0)
    {
      DataTable dataTable1 = new DataTable();
      Decimal num1 = 0.0M;
      if (!string.IsNullOrEmpty(TIPMOVSAN))
      {
        CODCAUSAN = objDataAccess.Get1ValueFromSQL("SELECT CODCAU FROM TIPMOVCAU WHERE TIPMOV = " + DBMethods.DoublePeakForSql(TIPMOVSAN) + " AND CURRENT_DATE BETWEEN DATINI AND DATFIN", CommandType.Text);
        Decimal days = (Decimal) Convert.ToDateTime(DATFINSAN).Subtract(Convert.ToDateTime(DATINISAN)).Days;
        string str = "SELECT VALUE(GIORNI, 0) AS GIORNI, VALUE(TASSO, 0.0) AS TASSO, VALUE(PERMAXSOGLIA, 0.0) AS PERMAXSOGLIA ";
        string strSQL;
        if (string.IsNullOrEmpty(DATRETT))
          strSQL = str + " FROM TIPMOVCAU WHERE TIPMOV = '" + TIPMOVSAN + "' AND " + DBMethods.Db2Date(DATINISAN) + " BETWEEN DATINI AND DATFIN ";
        else
          strSQL = str + " FROM TIPMOVCAU WHERE TIPMOV = '" + TIPMOVSAN + "' AND " + DBMethods.Db2Date(DATRETT) + " BETWEEN DATINI AND DATFIN ";
        dataTable1.Clear();
        DataTable dataTable2 = objDataAccess.GetDataTable(strSQL);
        if (dataTable2.Rows.Count > 0)
        {
          if (TIPMOVSAN == "SAN_RT_MD")
            days += (Decimal) dataTable2.Rows[0]["GIORNI"];
          TASSO = (Decimal) dataTable2.Rows[0][nameof (TASSO)];
          PERMAXSOGLIA = (Decimal) dataTable2.Rows[0][nameof (PERMAXSOGLIA)];
        }
        Decimal num2;
        if (ANNO == 0)
        {
          num2 = IMPORTO_RETRIBUZIONE * ALIQUOTA / 100M * days * TASSO / 36500M;
          num1 = Convert.ToDecimal(IMPORTO_RETRIBUZIONE) * ALIQUOTA / 100M * days * TASSO / 36500M;
        }
        else
        {
          num2 = IMPORTO_RETRIBUZIONE * ALIQUOTA / 100M * days * TASSO / 36500M;
          num1 = Convert.ToDecimal(IMPORTO_RETRIBUZIONE) * ALIQUOTA / 100M * days * TASSO / 36500M;
        }
      }
      else
        num1 = 0.0M;
      return num1;
    }

    public int Module_GetAnnoBilancio(
      DataLayer objDataAccess,
      bool BILANCIO_A_DATA_SISTEMA,
      int ANNDEN = 0)
    {
      DataTable dataTable = new DataTable();
      string strDataDecorrenza = this.Utils.Module_GetDataSistema().ToString();
      if (BILANCIO_A_DATA_SISTEMA)
        return Convert.ToInt32(this.clsWRITE_DB.Module_GetValorePargen(objDataAccess, 2, strDataDecorrenza) ?? throw new Exception("Contattare l'Amministratore del Sistema per impostazione bilancio"));
      if (ANNDEN == Convert.ToDateTime(strDataDecorrenza).Year)
        return ANNDEN;
      return Convert.ToInt32(this.clsWRITE_DB.Module_GetValorePargen(objDataAccess, 2, strDataDecorrenza) ?? throw new Exception("Contattare l'Amministratore del Sistema per impostazione bilancio"));
    }

    public string Module_GetTipoAnno(int ANNDEN, int ANNBIL)
    {
      if (ANNDEN == ANNBIL)
        return "AC";
      if (ANNBIL < ANNDEN)
        throw new Exception("Contattare l'Amministratore del Sistema. Anno di bilancio inferiore all'anno della distinta");
      return "AP";
    }

    public bool Module_RDL_IsCessato(DataLayer objDataAccess, int CODPOS, int MAT, int PRORAP)
    {
      string strSQL = "SELECT VALUE(DATCES, '9999-12-31') AS DATCES FROM RAPLAV WHERE CODPOS = " + CODPOS.ToString() + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString();
      objDataAccess.GetDataTable(strSQL);
      return true;
    }

    public void Module_AggiornaRaplav(DataLayer objDataAccess, int CODPOS, int MAT, int PRORAP)
    {
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = new DataTable();
      string strData1 = "";
      string strData2 = "";
      string strData3 = "";
      Decimal num1 = 0.0M;
      Decimal num2 = 0.0M;
      Decimal num3 = 0.0M;
      string str1 = "";
      Decimal num4 = 0.0M;
      Decimal num5 = 0.0M;
      Decimal num6 = 0.0M;
      int num7 = 0;
      int num8 = 0;
      int num9 = 0;
      int num10 = 0;
      string str2 = "";
      int num11 = 0;
      string str3 = "";
      string str4 = "";
      string str5 = "";
      string ErroreMSG = "";
      DataTable datiCompletiStordl = this.Module_GetDatiCompleti_STORDL(objDataAccess, CODPOS, MAT, PRORAP, ErroreMSG);
      if (datiCompletiStordl.Rows.Count > 0)
      {
        str3 = datiCompletiStordl.Rows[0]["ABBPRE"].ToString();
        str4 = datiCompletiStordl.Rows[0]["ASSCON"].ToString();
        num8 = Convert.ToInt32(datiCompletiStordl.Rows[0]["CODCON"]);
        num9 = Convert.ToInt32(datiCompletiStordl.Rows[0]["CODLOC"]);
        num5 = Convert.ToDecimal(datiCompletiStordl.Rows[0]["CODGRUASS"]);
        str1 = datiCompletiStordl.Rows[0]["FAP"].ToString().Trim();
        if (str1 == "S")
        {
          string strSQL = "SELECT VALFAP FROM CODFAP WHERE '" + DBMethods.Db2Date(datiCompletiStordl.Rows[0]["DATINI"].ToString()) + "' BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31')";
          num4 = Convert.ToDecimal(objDataAccess.Get1ValueFromSQL(strSQL, CommandType.Text));
          num6 = Convert.ToDecimal(datiCompletiStordl.Rows[0]["ALIQUOTA"]) - num4;
        }
        else
          num6 = (Decimal) datiCompletiStordl.Rows[0]["ALIQUOTA"];
        num11 = Convert.ToInt32(datiCompletiStordl.Rows[0]["TIPRAP"]);
        str2 = datiCompletiStordl.Rows[0]["DENLIV"].ToString().Trim();
        num10 = Convert.ToInt32(datiCompletiStordl.Rows[0]["CODLIV"]);
        num7 = Convert.ToInt32(datiCompletiStordl.Rows[0]["CODQUACON"]);
        string strSQL1 = " SELECT * FROM (SELECT (SELECT CATFORASS FROM FORASS WHERE CODFORASS = A.CODFORASS) AS CAT, A.ALIQUOTA " + " FROM ALIFORASS A WHERE A.CODGRUASS =  '" + num5.ToString() + "' AND A.CODQUACON = '" + num7.ToString() + "' " + " ) AS TAB WHERE TAB.CAT <> 'FAP'";
        DataTable dataTable3 = objDataAccess.GetDataTable(strSQL1);
        int num12 = dataTable3.Rows.Count - 1;
        for (int index = 0; index <= num12; ++index)
        {
          string str6 = dataTable3.Rows[index]["CAT"].ToString().Trim();
          if (!(str6 == "TFR"))
          {
            if (!(str6 == "PREV"))
            {
              if (!(str6 == "INF"))
                throw new Exception("Caso non gestito " + dataTable3.Rows[index]["CAT"]?.ToString());
              num2 = Convert.ToDecimal(dataTable3.Rows[index]["ALIQUOTA"]);
            }
            else
              num3 = Convert.ToDecimal(dataTable3.Rows[index]["ALIQUOTA"]);
          }
          else
            num1 = Convert.ToDecimal(dataTable3.Rows[index]["ALIQUOTA"]);
        }
      }
      for (int index1 = 0; index1 <= 2; ++index1)
      {
        switch (index1)
        {
          case 0:
            str5 = "TFR";
            break;
          case 1:
            str5 = "PREV";
            break;
          case 2:
            str5 = "INF";
            break;
        }
        int num13 = datiCompletiStordl.Rows.Count - 1;
        for (int index2 = 0; index2 <= num13; ++index2)
        {
          int num14 = 0;
          string strSQL2 = "SELECT CODQUACON FROM CONRIF " + " WHERE CODCON = '" + datiCompletiStordl.Rows[index2]["CODCON"]?.ToString() + "' " + " AND DATDEC <= '" + DBMethods.Db2Date(datiCompletiStordl.Rows[index2]["DATINI"].ToString()) + "' " + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
          num14 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL2, CommandType.Text));
          string strSQL3 = " SELECT COUNT(*) FROM ALIFORASS A, FORASS B WHERE A.CODGRUASS =  '" + datiCompletiStordl.Rows[index2]["CODGRUASS"]?.ToString() + "' " + " AND A.CODQUACON = '" + num14.ToString() + "' " + " AND A.CODFORASS = B.CODFORASS AND B.CATFORASS = " + DBMethods.DoublePeakForSql(str5) + " ";
          if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL3, CommandType.Text)) > 0)
          {
            switch (index1)
            {
              case 0:
                strData1 = datiCompletiStordl.Rows[index2]["DATINI"].ToString();
                break;
              case 1:
                strData3 = datiCompletiStordl.Rows[index2]["DATINI"].ToString();
                break;
              case 2:
                strData2 = datiCompletiStordl.Rows[index2]["DATINI"].ToString();
                break;
            }
          }
          else
            break;
        }
      }
      string str7 = " UPDATE RAPLAV SET ";
      string str8 = string.IsNullOrEmpty(strData1.Trim()) ? str7 + " DTTFR = NULL, " + " ALIQTFR = NULL, " : str7 + " DTTFR  = '" + DBMethods.Db2Date(strData1) + "', " + " ALIQTFR = '" + num1.ToString().Replace(",", ".") + "', ";
      string str9 = string.IsNullOrEmpty(strData2.Trim()) ? str8 + " DTINF  = NULL, " + " ALIQINF  = NULL, " : str8 + " DTINF  = '" + DBMethods.Db2Date(strData2) + "', " + " ALIQINF  = '" + num2.ToString().Replace(",", ".") + "', ";
      string str10 = (string.IsNullOrEmpty(strData3.Trim()) ? str9 + " DTFP  = NULL, " + " ALIQFP  = NULL, " : str9 + " DTFP  = '" + DBMethods.Db2Date(strData3) + "', " + " ALIQFP  = '" + num3.ToString().Replace(",", ".") + "', ") + " FAP  = " + DBMethods.DoublePeakForSql(str1) + ", ";
      string strSQL4 = (!(str1 == "S") ? str10 + " ALIFAP = NULL, " : str10 + " ALIFAP  = '" + num4.ToString().Replace(",", ".") + "', ") + " ALIQUOTA  = '" + num6.ToString().Replace(",", ".") + "', " + " CODGRUASS  = '" + num5.ToString() + "', " + " CODQUACON  = '" + num7.ToString() + "', " + " CODCON  = '" + num8.ToString() + "', " + " CODLOC  = '" + num9.ToString() + "', " + " CODLIV  = '" + num10.ToString() + "', " + " DENLIV  = " + DBMethods.DoublePeakForSql(str2) + ", " + " TIPRAP  = '" + num11.ToString() + "', " + " ABBPRE  = " + DBMethods.DoublePeakForSql(str3) + ", FLGAPP = 'M', " + " ASSCON  = " + DBMethods.DoublePeakForSql(str4) + " WHERE CODPOS = '" + CODPOS.ToString() + "' " + " AND MAT = '" + MAT.ToString() + "' " + " AND PRORAP = '" + PRORAP.ToString() + "' ";
      objDataAccess.WriteTransactionData(strSQL4, CommandType.Text);
    }

    public void Module_Carica_TITSTU(
      DataLayer objDataAccess,
      GestioneRapportiLavoroIscrittiOCM rdl,
      string strRic = "",
      bool ConPrimaVoceVuota = false)
    {
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = objDataAccess.GetDataTable("SELECT CODTITSTU, DENTITSTU FROM TITSTU " + strRic + " ORDER BY ORDINE");
      List<GestioneRapportiLavoroIscrittiOCM.TitoliStudio> titoliStudioList = new List<GestioneRapportiLavoroIscrittiOCM.TitoliStudio>();
      foreach (DataRow row in (InternalDataCollectionBase) dataTable2.Rows)
      {
        GestioneRapportiLavoroIscrittiOCM.TitoliStudio titoliStudio = new GestioneRapportiLavoroIscrittiOCM.TitoliStudio()
        {
          dentistu = row["DENTITSTU"].ToString(),
          codtitstu = row["CODTITSTU"].ToString()
        };
        titoliStudioList.Add(titoliStudio);
      }
      rdl.listTitoli = titoliStudioList;
    }

    public void Module_GetListaAliquoteContributive(
      DataLayer objDataAccess,
      GestioneRapportiLavoroIscrittiOCM rdl,
      int CODQUACON,
      int CODGRUASS,
      string STRDATA,
      string STRGRUCON,
      bool BLN65ANNI,
      string FAP)
    {
      Decimal num1 = 0.00M;
      DataTable dataTable1 = new DataTable();
      string str1 = (string) null;
      if (string.IsNullOrEmpty(STRDATA))
        return;
      try
      {
        if (FAP == "S")
        {
          string strSQL = "SELECT VALFAP FROM CODFAP WHERE " + DBMethods.Db2Date(STRDATA) + " BETWEEN DATINI AND  VALUE(DATFIN, '9999-12-31')";
          dataTable1 = objDataAccess.GetDataTable(strSQL);
          str1 = dataTable1.Rows.Count <= 0 ? "0,00" : dataTable1.Rows[0]["VALFAP"].ToString();
        }
        string str2 = "SELECT DISTINCT ALIFORASS.CODFORASS,ALIFORASS.ALIQUOTA," + " (" + " SELECT DENFORASS FROM FORASS WHERE FORASS.CODFORASS=ALIFORASS.CODFORASS" + " ) AS DENFORASS" + " FROM ALIFORASS WHERE" + " CODQUACON =  '" + CODQUACON.ToString() + "' " + " AND CODGRUASS= '" + CODGRUASS.ToString() + "' " + " AND '" + DBMethods.Db2Date(STRDATA) + "' BETWEEN DATINI AND DATFIN";
        if (BLN65ANNI)
          str2 += " AND ALIFORASS.CODFORASS IN (SELECT CODFORASS FROM FORASS WHERE CATFORASS<>'PREV') ";
        string strSQL1 = str2 + " AND ALIFORASS.CODFORASS NOT IN (SELECT CODFORASS FROM FORASS WHERE CATFORASS='FAP') ";
        dataTable1.Rows.Clear();
        DataTable dataTable2 = objDataAccess.GetDataTable(strSQL1);
        int num2 = dataTable2.Rows.Count - 1;
        List<GestioneRapportiLavoroIscrittiOCM.ListAliquota> listAliquotaList = new List<GestioneRapportiLavoroIscrittiOCM.ListAliquota>();
        foreach (DataRow row in (InternalDataCollectionBase) dataTable2.Rows)
        {
          GestioneRapportiLavoroIscrittiOCM.ListAliquota listAliquota = new GestioneRapportiLavoroIscrittiOCM.ListAliquota()
          {
            DENFORASS = row["DENFORASS"].ToString(),
            CODFORASS = row["CODFORASS"].ToString(),
            ALIQUOTA = row["ALIQUOTA"].ToString()
          };
          listAliquotaList.Add(listAliquota);
        }
        rdl.ListAliq = listAliquotaList;
        int index;
        for (index = 0; index <= num2; ++index)
          num1 += Convert.ToDecimal(dataTable2.Rows[index]["ALIQUOTA"]);
        if (FAP == "S")
        {
          string str3 = Convert.ToString(dataTable2.Rows[index][nameof (FAP)].ToString()).Replace(".", ",");
          num1 += Convert.ToDecimal(str3);
        }
        rdl.datiRetributivi.TOTALIQ = num1.ToString();
      }
      catch (Exception ex)
      {
        throw new Exception("Errore nel caricamento lista aliquote");
      }
    }

    public string getDatiAnagrafici(GestioneRapportiLavoroIscrittiOCM rdl) => "" + rdl.iscritti.nome.ToUpper().Trim() + "@" + rdl.iscritti.cognome.ToUpper().Trim() + "@" + rdl.iscritti.codfis.ToUpper().Trim() + "@" + rdl.iscritti.datnas + "@" + rdl.iscritti.sesso + "@" + rdl.iscritti.comuneN + "@" + rdl.iscritti.provN + "@" + rdl.iscritti.statoEsN + "@" + rdl.iscritti.titoloStudio + "@";

    public string getDatiRecapiti(GestioneRapportiLavoroIscrittiOCM rdl) => "" + rdl.iscritti.dug + "@" + rdl.iscritti.indirizzo + "@" + rdl.iscritti.numciv + "@" + rdl.iscritti.comune + "@" + rdl.iscritti.localita + "@" + rdl.iscritti.prov + "@" + rdl.iscritti.cap + "@" + rdl.iscritti.statoEs + "@" + rdl.iscritti.tel + "@" + rdl.iscritti.tel2 + "@" + rdl.iscritti.fax + "@" + rdl.iscritti.email + "@" + rdl.iscritti.url + "@" + rdl.iscritti.co + "@" + rdl.iscritti.cellulare + "@" + rdl.iscritti.pec + "@";
  }
}
