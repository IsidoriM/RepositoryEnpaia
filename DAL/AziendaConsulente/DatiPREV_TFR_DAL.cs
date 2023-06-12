// Decompiled with JetBrains decompiler
// Type: TFI.DAL.AziendaConsulente.DatiPREV_TFR_DAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using System;
using System.Collections.Generic;
using System.Data;
using TFI.CRYPTO.Crypto;
using TFI.DAL.ConnectorDB;
using TFI.OCM.AziendaConsulente;

namespace TFI.DAL.AziendaConsulente
{
  public class DatiPREV_TFR_DAL
  {
    public List<DatiPREV_TFR_OCM.PrevdaConf> SearchPrev(
      string mat,
      string cog,
      string nom,
      string periodo1,
      string periodo2,
      string stato,
      string codpos)
    {
      try
      {
        DataLayer dataLayer = new DataLayer();
        string strSQL = "SELECT DISTINCT ISCT.MAT, ISCT.COG, ISCT.NOM, RAPLAV.PROTCES AS STRPROT, RAPLAV.DATPROTCES AS DATPROT,\r\n                                           TRANSLATE(CHAR(raplav.datdec, EUR),'/','.') AS DATDEC,\r\n                                           TRANSLATE(CHAR(raplav.datces, EUR),'/','.') AS DATCES,\r\n                                           TRANSLATE(CHAR(raplav.datass, EUR),'/','.') AS DATASS,\r\n                                           CAUCES.DENCES, RAPLAV.CODPOS, RAPLAV.PRORAP, MODPRE.PROMOD,\r\n                                           CASE MODPRE.CODSTAPRE WHEN 2 THEN 'IN LAVORAZIONE' WHEN 3 THEN 'IN LAVORAZIONE' WHEN 4 THEN 'IN LAVORAZIONE' ELSE DENSTAPRE END AS STATO\r\n                                FROM\r\n                                       MODPRE INNER JOIN RAPLAV ON MODPRE.CODPOS = RAPLAV.CODPOS\r\n                                       AND MODPRE.MAT = RAPLAV.MAT AND MODPRE.PRORAP = RAPLAV.PRORAP\r\n                                       INNER JOIN CAUCES ON RAPLAV.CODCAUCES = CAUCES.CODCAUCES\r\n                                       INNER JOIN CODSTAPRE ON MODPRE.CODSTAPRE = CODSTAPRE.CODSTAPRE\r\n                                       INNER JOIN ISCT ON RAPLAV.MAT = ISCT.MAT\r\n                             WHERE \r\n                                     RAPLAV.CODPOS = '" + codpos + "' AND RAPLAV.PROTCES IS NOT NULL AND RAPLAV.DATPROTCES IS NOT NULL ";
        if (!string.IsNullOrEmpty(mat))
          strSQL = strSQL + " AND ISCT.MAT= '" + mat + "' ";
        if (!string.IsNullOrEmpty(cog))
          strSQL = strSQL + " AND ISCT.COG= '" + cog + "' ";
        if (!string.IsNullOrEmpty(nom))
          strSQL = strSQL + " AND ISCT.NOM= '" + nom + "' ";
        if (!string.IsNullOrEmpty(periodo1))
          strSQL = strSQL + " AND RAPLAV.DATCES >= '" + periodo1 + "' ";
        if (!string.IsNullOrEmpty(periodo2))
          strSQL = strSQL + " AND RAPLAV.DATCES <= '" + periodo2 + "' ";
        if (!string.IsNullOrEmpty(stato))
        {
          if (stato.ToUpper() == "DA TRASMETTER")
            strSQL += " AND MODPRE.CODSTAPRE='0' ";
          else if (stato.ToUpper() == "TRASMESSO=")
            strSQL += " AND MODPRE.CODSTAPRE='1' ";
          else if (stato.ToUpper() == "IN LAVORAZIONE")
            strSQL += " AND MODPRE.CODSTAPRE='2' ";
          else if (stato.ToUpper() == "PAGATO")
            strSQL += " AND MODPRE.CODSTAPRE='5' ";
        }
        List<DatiPREV_TFR_OCM.PrevdaConf> prevdaConfList = new List<DatiPREV_TFR_OCM.PrevdaConf>();
        string Err = "";
        DataSet dataSet = new DataSet();
        foreach (DataRow row in (InternalDataCollectionBase) dataLayer.GetDataSet(strSQL, ref Err).Tables[0].Rows)
        {
          DatiPREV_TFR_OCM.PrevdaConf prevdaConf = new DatiPREV_TFR_OCM.PrevdaConf()
          {
            Matricola = row["MAT"].ToString(),
            Cognome = row["COG"].ToString().Trim(),
            Nome = row["NOM"].ToString().Trim(),
            DataIsc = row["DATDEC"].ToString(),
            DataCess = row["DATCES"].ToString(),
            Causale = row["DENCES"].ToString(),
            Prorap = row["PRORAP"].ToString(),
            Promod = row["PROMOD"].ToString(),
            Strprot = row["STRPROT"].ToString(),
            Datprot = row["DATPROT"].ToString()
          };
          prevdaConfList.Add(prevdaConf);
        }
        return prevdaConfList;
      }
      catch (Exception ex)
      {
        return (List<DatiPREV_TFR_OCM.PrevdaConf>) null;
      }
    }

    public List<DatiPREV_TFR_OCM.PrevdaConf> SearchPrevConfDAL(string codpos)
    {
      try
      {
        DataLayer dataLayer = new DataLayer();
        string strSQL = "SELECT DISTINCT ISCT.MAT, ISCT.COG, ISCT.NOM, RAPLAV.PROTCES AS STRPROT, RAPLAV.DATPROTCES AS DATPROT, \r\n                                   TRANSLATE(CHAR(raplav.datdec, EUR),'/','.') AS DATDEC, \r\n                                   TRANSLATE(CHAR(raplav.datces, EUR),'/','.') AS DATCES, \r\n                                   TRANSLATE(CHAR(raplav.datass, EUR),'/','.') AS DATASS, \r\n                                   CAUCES.DENCES, RAPLAV.CODPOS, RAPLAV.PRORAP, MODPRE.PROMOD, \r\n                                   CASE MODPRE.CODSTAPRE WHEN 2 THEN 'IN LAVORAZIONE' WHEN 3 THEN 'IN LAVORAZIONE' WHEN 4 THEN 'IN LAVORAZIONE' ELSE DENSTAPRE END AS STATO \r\n                                FROM MODPRE INNER JOIN RAPLAV ON MODPRE.CODPOS = RAPLAV.CODPOS \r\n                                   AND MODPRE.MAT = RAPLAV.MAT AND MODPRE.PRORAP = RAPLAV.PRORAP \r\n                                   INNER JOIN CAUCES ON RAPLAV.CODCAUCES = CAUCES.CODCAUCES \r\n                                   INNER JOIN CODSTAPRE ON MODPRE.CODSTAPRE = CODSTAPRE.CODSTAPRE \r\n                                   INNER JOIN ISCT ON RAPLAV.MAT = ISCT.MAT \r\n                                WHERE RAPLAV.CODPOS = '" + codpos + "' AND MODPRE.CODSTAPRE = 0 AND DATANN IS NULL ";
        List<DatiPREV_TFR_OCM.PrevdaConf> prevdaConfList = new List<DatiPREV_TFR_OCM.PrevdaConf>();
        string Err = "";
        DataSet dataSet = new DataSet();
        foreach (DataRow row in (InternalDataCollectionBase) dataLayer.GetDataSet(strSQL, ref Err).Tables[0].Rows)
        {
          DatiPREV_TFR_OCM.PrevdaConf prevdaConf = new DatiPREV_TFR_OCM.PrevdaConf()
          {
            Matricola = row["MAT"].ToString(),
            Cognome = row["COG"].ToString().Trim(),
            Nome = row["NOM"].ToString().Trim(),
            DataIsc = row["DATDEC"].ToString(),
            DataCess = row["DATCES"].ToString(),
            Causale = row["DENCES"].ToString(),
            Prorap = row["PRORAP"].ToString(),
            Promod = row["PROMOD"].ToString(),
            Strprot = row["STRPROT"].ToString(),
            Datprot = row["DATPROT"].ToString()
          };
          prevdaConfList.Add(prevdaConf);
        }
        return prevdaConfList;
      }
      catch (FormatException ex)
      {
        return (List<DatiPREV_TFR_OCM.PrevdaConf>) null;
      }
    }

    public DatiPREV_TFR_OCM ProspTFR(string codPos)
    {
      DatiPREV_TFR_OCM datiPrevTfrOcm = new DatiPREV_TFR_OCM();
      try
      {
        DataLayer dataLayer = new DataLayer();
        string strSQL1 = "SELECT DISTINCT RAGSOC,CODPOS,CODFIS,PARIVA FROM AZI WHERE CODPOS = '" + codPos + "' ";
        string strSQL2 = "SELECT DISTINCT IND,NUMCIV,CAP,SIGPRO,DENLOC FROM INDSED WHERE CODPOS='" + codPos + "' AND TIPIND='1' AND CURRENT_DATE BETWEEN DATINI AND DATFIN ";
        string Err = "";
        DataSet dataSet1 = new DataSet();
        DataSet dataSet2 = dataLayer.GetDataSet(strSQL1, ref Err);
        DataSet dataSet3 = new DataSet();
        DataSet dataSet4 = dataLayer.GetDataSet(strSQL2, ref Err);
        datiPrevTfrOcm.prospLiqTfr.Posizione = dataSet2.Tables[0].Rows[0]["CODPOS"].ToString().Trim();
        datiPrevTfrOcm.prospLiqTfr.RagSoc = dataSet2.Tables[0].Rows[0]["RAGSOC"].ToString().Trim();
        datiPrevTfrOcm.prospLiqTfr.CodFis = dataSet2.Tables[0].Rows[0]["CODFIS"].ToString().Trim();
        datiPrevTfrOcm.prospLiqTfr.ParIVA = dataSet2.Tables[0].Rows[0]["PARIVA"].ToString().Trim();
        datiPrevTfrOcm.prospLiqTfr.Cap = dataSet4.Tables[0].Rows[0]["CAP"].ToString().Trim();
        datiPrevTfrOcm.prospLiqTfr.Comune = dataSet4.Tables[0].Rows[0]["DENLOC"].ToString().Trim();
        datiPrevTfrOcm.prospLiqTfr.Prov = dataSet4.Tables[0].Rows[0]["SIGPRO"].ToString().Trim();
        datiPrevTfrOcm.prospLiqTfr.IndSede = dataSet4.Tables[0].Rows[0]["IND"].ToString().Trim() + " " + dataSet4.Tables[0].Rows[0]["NUMCIV"].ToString().Trim();
        string strSQL3 = " SELECT DISTINCT NUMELE,ID,ANNOELE,LETLIQ.MAT,ISCT.COG,ISCT.NOM,NOMEFILE,TIPLIQ FROM LETLIQ INNER JOIN ISCT ON LETLIQ.MAT=ISCT.MAT WHERE CODPOS='" + codPos + "' AND TIPDEST='A' AND WEB='S' ORDER BY ANNOELE DESC ";
        DataSet dataSet5 = new DataSet();
        foreach (DataRow row in (InternalDataCollectionBase) dataLayer.GetDataSet(strSQL3, ref Err).Tables[0].Rows)
        {
          DatiPREV_TFR_OCM.ListProspLiqTfr listProspLiqTfr = new DatiPREV_TFR_OCM.ListProspLiqTfr()
          {
            matricola = row["MAT"].ToString().Trim(),
            Cognome = row["COG"].ToString().Trim().ToUpper(),
            Nome = row["NOM"].ToString().Trim().ToUpper(),
            anno = row["ANNOELE"].ToString().Trim(),
            numElenco = row["NUMELE"].ToString().Trim(),
            path = Cypher.CryptPassword(row["NOMEFILE"].ToString().Trim())
          };
          datiPrevTfrOcm.listProspLiqTfrs.Add(listProspLiqTfr);
        }
        return datiPrevTfrOcm;
      }
      catch (Exception ex)
      {
        return (DatiPREV_TFR_OCM) null;
      }
    }
  }
}
