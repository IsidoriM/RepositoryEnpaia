// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Amministrativo.AnnullamentiDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Data;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.Amministrativo;

namespace TFI.DAL.Amministrativo
{
  public class AnnullamentiDAL
  {
    public static List<TipiMovimento> GetTipiMovimento()
    {
      try
      {
        DataTable dataTable = new DataLayer().GetDataTable("SELECT TIPMOV, DENTIPMOV FROM TIPMOV WHERE TIPMOV IN ('NU', 'DP', 'AR') ORDER BY TIPMOV");
        if (dataTable.Rows.Count <= 0)
          return (List<TipiMovimento>) null;
        List<TipiMovimento> tipiMovimento = new List<TipiMovimento>();
        foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          tipiMovimento.Add(new TipiMovimento()
          {
            Codice = row["TIPMOV"].ToString(),
            Denominazione = row["DENTIPMOV"].ToString()
          });
        return tipiMovimento;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static string GetRagioneSociale(string codPos)
    {
      try
      {
        DataLayer dataLayer = new DataLayer();
        iDB2Parameter parameter = dataLayer.CreateParameter("codPos_param", iDB2DbType.iDB2VarChar, 50, ParameterDirection.Input, codPos);
        string strSQL = "SELECT RAGSOC FROM azi WHERE CODPOS = @codPos_param";
        DataTable tableWithParameters = dataLayer.GetDataTableWithParameters(strSQL, parameter);
        return tableWithParameters.Rows.Count > 0 ? (!DBNull.Value.Equals(tableWithParameters.Rows[0]["RAGSOC"]) ? tableWithParameters.Rows[0]["RAGSOC"].ToString() : string.Empty) : string.Empty;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static List<AnnullamentoDenunceOCM> CaricaDati(
      string codPos,
      string anno_da,
      string mese_da,
      string anno_a,
      string mese_a,
      string tipMov)
    {
      try
      {
        anno_da = anno_da.Trim();
        anno_a = anno_a.Trim();
        DataLayer dataLayer = new DataLayer();
        Utile utile = new Utile();
        string empty = string.Empty;
        string str1 = string.Empty;
        string str2 = string.Empty;
        DataTable dataTable1 = utile.CREA_DT_AUTORIZZAZIONI_ENPAIA(ref empty);
        string str3 = "SELECT DENTES.*, " + " (SELECT RAGSOC FROM AZI WHERE AZI.CODPOS = DENTES.CODPOS) AS RAGSOC, " + " (SELECT DENTIPUTE FROM TIPUTE WHERE CODTIPUTE = DENTES.UTEAPE) AS UTEAPE, " + " (SELECT DENTIPUTE FROM TIPUTE WHERE CODTIPUTE = DENTES.UTECHI) AS UTECHI, " + " (SELECT DENTIPMOV FROM TIPMOV WHERE TIPMOV=DENTES.TIPMOV) AS DENTIPMOV, " + " (CASE DENTES.STADEN WHEN 'N' THEN 'NON VALIDA' WHEN 'A' THEN 'ACQUISITA' ELSE 'NON VALIDA' END) AS DENSTADEN, " + " (SELECT DENMODPAG FROM MODPAG WHERE CODMODPAG = DENTES.CODMODPAG) AS DENMODPAG " + " FROM DENTES ";
        if (codPos == string.Empty)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable1.Rows)
          {
            if ((!DBNull.Value.Equals(row["ABILITATO"]) ? row["ABILITATO"].ToString() : string.Empty) == "NO")
            {
              string str4 = !DBNull.Value.Equals(row["POSIZIONE"]) ? row["POSIZIONE"].ToString() : string.Empty;
              str1 = str1 + "," + str4;
            }
          }
          if (str1.Trim() != string.Empty)
          {
            string str5 = str1.Substring(1);
            str2 = str2 + " AND DENTES.CODPOS NOT IN (" + str5 + ") ";
          }
        }
        if (anno_da != string.Empty && mese_da == "0")
          str2 = str2 + " AND TRIM(CHAR(RIGHT('0000' || DENTES.ANNDEN , 4))) >= " + DBMethods.DoublePeakForSql(anno_da);
        if (anno_da == string.Empty && mese_da != "0")
          str2 = str2 + " AND TRIM(CHAR(RIGHT('00' || DENTES.MESDEN , 2))) >= " + DBMethods.DoublePeakForSql(mese_da.PadLeft(2, '0'));
        if (anno_da != string.Empty && mese_da != "0")
          str2 = str2 + " AND TRIM(CHAR(RIGHT('0000' || DENTES.ANNDEN , 4))) || TRIM(CHAR(RIGHT('00' || DENTES.MESDEN , 2))) >= " + DBMethods.DoublePeakForSql(anno_da + mese_da.PadLeft(2, '0'));
        if (anno_a != string.Empty && mese_a == "0")
          str2 = str2 + " AND TRIM(CHAR(RIGHT('0000' || DENTES.ANNDEN , 4))) <= " + DBMethods.DoublePeakForSql(anno_a);
        if (anno_a == string.Empty && mese_a != "0")
          str2 = str2 + " AND TRIM(CHAR(RIGHT('00' || DENTES.MESDEN , 2))) <= " + DBMethods.DoublePeakForSql(mese_a.PadLeft(2, '0'));
        if (anno_a != string.Empty && mese_a != "0")
          str2 = str2 + " AND TRIM(CHAR(RIGHT('0000' || DENTES.ANNDEN , 4))) || TRIM(CHAR(RIGHT('00' || DENTES.MESDEN , 2))) <= " + DBMethods.DoublePeakForSql(anno_a + mese_a.PadLeft(2, '0'));
        if (anno_da == string.Empty && mese_da == "0" && anno_a == string.Empty && mese_a == "0")
          str2 = str2 + " AND TRIM(CHAR(RIGHT('0000' || DENTES.ANNDEN , 4))) || TRIM(CHAR(RIGHT('00' || DENTES.MESDEN , 2))) >= '200212' " + " AND TRIM(CHAR(RIGHT('0000' || DENTES.ANNDEN , 4))) || TRIM(CHAR(RIGHT('00' || DENTES.MESDEN , 2))) <= '999912' ";
        string str6 = str2 + " AND DENTES.MESDEN <> 0";
        if (codPos.Trim() != string.Empty)
          str6 = str6 + " AND DENTES.CODPOS = " + codPos;
        if (tipMov != string.Empty)
          str6 = !(tipMov == "SAN") ? str6 + " AND DENTES.TIPMOV = '" + tipMov.Trim() + "'" : str6 + " AND DENTES.NUMSANANN IS NULL AND NUMSAN IS NOT NULL";
        string str7 = str6 + " AND DENTES.DATMOVANN IS NULL";
        if (str7 != string.Empty)
          str7 = " WHERE " + str7.Substring(4);
        string strSQL = str3 + str7 + " ORDER BY DENTES.CODPOS, DENTES.ANNDEN, DENTES.MESDEN";
        DataTable dataTable2 = dataLayer.GetDataTable(strSQL);
        if (dataTable2.Rows.Count <= 0)
          return (List<AnnullamentoDenunceOCM>) null;
        List<AnnullamentoDenunceOCM> annullamentoDenunceOcmList = new List<AnnullamentoDenunceOCM>();
        foreach (DataRow row in (InternalDataCollectionBase) dataTable2.Rows)
        {
          AnnullamentoDenunceOCM annullamentoDenunceOcm = new AnnullamentoDenunceOCM()
          {
            CodPos = !DBNull.Value.Equals(row["CODPOS"]) ? Convert.ToInt32(row["CODPOS"]) : 0,
            RagSoc = !DBNull.Value.Equals(row["RAGSOC"]) ? row["RAGSOC"].ToString() : string.Empty,
            AnnDen = !DBNull.Value.Equals(row["ANNDEN"]) ? Convert.ToInt32(row["ANNDEN"]) : 0,
            Mese = !DBNull.Value.Equals(row["MESDEN"]) ? Convert.ToInt32(row["MESDEN"]) : 0
          };
          annullamentoDenunceOcm.MesDen = annullamentoDenunceOcm.Mese != 0 ? Utile.GetMesi()[annullamentoDenunceOcm.Mese] : string.Empty;
          annullamentoDenunceOcm.DenTipMov = !DBNull.Value.Equals(row["DENTIPMOV"]) ? row["DENTIPMOV"].ToString() : string.Empty;
          annullamentoDenunceOcm.ProDen = !DBNull.Value.Equals(row["PRODEN"]) ? Convert.ToInt32(row["PRODEN"]) : 0;
          annullamentoDenunceOcm.NumMov = !DBNull.Value.Equals(row["NUMMOV"]) ? row["NUMMOV"].ToString() : string.Empty;
          annullamentoDenunceOcm.ImpAbb = !DBNull.Value.Equals(row["IMPABB"]) ? Convert.ToDecimal(row["IMPABB"]) : 0M;
          annullamentoDenunceOcm.ImpCon = !DBNull.Value.Equals(row["IMPCON"]) ? Convert.ToDecimal(row["IMPCON"]) : 0M;
          annullamentoDenunceOcm.NumSan = !DBNull.Value.Equals(row["NUMSAN"]) ? row["NUMSAN"].ToString() : string.Empty;
          annullamentoDenunceOcm.NumSanAnn = !DBNull.Value.Equals(row["NUMSANANN"]) ? row["NUMSANANN"].ToString() : string.Empty;
          annullamentoDenunceOcm.ImpSanDet = !DBNull.Value.Equals(row["IMPSANDET"]) ? Convert.ToDecimal(row["IMPSANDET"]) : 0M;
          annullamentoDenunceOcm.ImpAddRec = !DBNull.Value.Equals(row["IMPADDREC"]) ? Convert.ToDecimal(row["IMPADDREC"]) : 0M;
          annullamentoDenunceOcm.ImpAssCon = !DBNull.Value.Equals(row["IMPASSCON"]) ? Convert.ToDecimal(row["IMPASSCON"]) : 0M;
          annullamentoDenunceOcm.ImpConDel = !DBNull.Value.Equals(row["IMPCONDEL"]) ? Convert.ToDecimal(row["IMPCONDEL"]) : 0M;
          annullamentoDenunceOcm.ImpAddRecDel = !DBNull.Value.Equals(row["IMPADDRECDEL"]) ? Convert.ToDecimal(row["IMPADDRECDEL"]) : 0M;
          annullamentoDenunceOcm.ImpSanRet = !DBNull.Value.Equals(row["IMPSANRET"]) ? Convert.ToDecimal(row["IMPSANRET"]) : 0M;
          annullamentoDenunceOcm.ImpDis = !DBNull.Value.Equals(row["IMPDIS"]) ? Convert.ToDecimal(row["IMPDIS"]) : 0M;
          annullamentoDenunceOcm.UltAgg = !DBNull.Value.Equals(row["ULTAGG"]) ? row["ULTAGG"].ToString() : string.Empty;
          annullamentoDenunceOcm.TipMov = !DBNull.Value.Equals(row["TIPMOV"]) ? row["TIPMOV"].ToString() : string.Empty;
          annullamentoDenunceOcm.CodCauMov = !DBNull.Value.Equals(row["CODCAUMOV"]) ? row["CODCAUMOV"].ToString() : string.Empty;
          annullamentoDenunceOcm.CodCauSan = !DBNull.Value.Equals(row["CODCAUSAN"]) ? row["CODCAUSAN"].ToString() : string.Empty;
          annullamentoDenunceOcmList.Add(annullamentoDenunceOcm);
        }
        return annullamentoDenunceOcmList;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static List<ImportiFromDENTES> GetDataFromDentes(
      AnnullamentoDenunceOCM denuncia)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string str = "SELECT * FROM DENTES WHERE CODPOS = " + denuncia.CodPos.ToString().Trim() + " AND ANNDEN = " + denuncia.AnnDen.ToString().Trim() + " AND MESDEN = " + denuncia.Mese.ToString().Trim();
        if (!string.IsNullOrEmpty(denuncia.NumMov))
          str = str + " AND NUMMOV = '" + denuncia.NumMov.Trim() + "'";
        string strSQL = str + " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL ORDER BY TIPMOV";
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count <= 0)
          return (List<ImportiFromDENTES>) null;
        List<ImportiFromDENTES> dataFromDentes = new List<ImportiFromDENTES>();
        foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          dataFromDentes.Add(new ImportiFromDENTES()
          {
            ImpDis = !DBNull.Value.Equals(row["IMPDIS"]) ? Convert.ToDecimal(row["IMPDIS"]) : 0M,
            ImpAbb = !DBNull.Value.Equals(row["IMPABB"]) ? Convert.ToDecimal(row["IMPABB"]) : 0M,
            ImpCon = !DBNull.Value.Equals(row["IMPCON"]) ? Convert.ToDecimal(row["IMPCON"]) : 0M,
            NumSan = !DBNull.Value.Equals(row["NUMSAN"]) ? row["NUMSAN"].ToString() : string.Empty,
            NumSanAnn = !DBNull.Value.Equals(row["NUMSANANN"]) ? row["NUMSANANN"].ToString() : string.Empty,
            ImpSanDet = !DBNull.Value.Equals(row["IMPSANDET"]) ? Convert.ToDecimal(row["IMPSANDET"]) : 0M,
            ImpAddRec = !DBNull.Value.Equals(row["IMPADDREC"]) ? Convert.ToDecimal(row["IMPADDREC"]) : 0M,
            ImpAssCon = !DBNull.Value.Equals(row["IMPASSCON"]) ? Convert.ToDecimal(row["IMPASSCON"]) : 0M,
            ImpConDel = !DBNull.Value.Equals(row["IMPCONDEL"]) ? Convert.ToDecimal(row["IMPCONDEL"]) : 0M,
            ImpAddRecDel = !DBNull.Value.Equals(row["IMPADDRECDEL"]) ? Convert.ToDecimal(row["IMPADDRECDEL"]) : 0M,
            ImpSanRet = !DBNull.Value.Equals(row["IMPSANRET"]) ? Convert.ToDecimal(row["IMPSANRET"]) : 0M,
            TipMov = !DBNull.Value.Equals(row["TIPMOV"]) ? row["TIPMOV"].ToString() : string.Empty,
            CodPos = !DBNull.Value.Equals(row["CODPOS"]) ? Convert.ToInt32(row["CODPOS"]) : 0,
            AnnDen = !DBNull.Value.Equals(row["ANNDEN"]) ? Convert.ToInt32(row["ANNDEN"]) : 0,
            Mese = !DBNull.Value.Equals(row["MESDEN"]) ? Convert.ToInt32(row["MESDEN"]) : 0,
            ProDen = !DBNull.Value.Equals(row["PRODEN"]) ? Convert.ToInt32(row["PRODEN"]) : 0
          });
        return dataFromDentes;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static bool MovimentoAnnullamento(DataLayer db, string filtroSql, string oldTimeStamp)
    {
      try
      {
        string str = db.GetDataTable("SELECT ULTAGG FROM DENTES " + filtroSql).Rows[0]["ULTAGG"].ToString();
        if (oldTimeStamp.Trim() != str)
          return false;
        string strSQL1 = "DELETE FROM DENDET" + filtroSql + " AND EXISTS(SELECT * FROM DENTES " + filtroSql + " AND STADEN='N' " + " AND DENDET.CODPOS = DENTES.CODPOS " + " AND DENDET.ANNDEN = DENTES.ANNDEN" + " AND DENDET.MESDEN = DENTES.MESDEN" + " AND DENDET.PRODEN = DENTES.PRODEN)";
        if (db.WriteTransactionData(strSQL1, CommandType.Text))
        {
          string strSQL2 = "DELETE FROM DENTES" + filtroSql + " AND STADEN='N' ";
          if (db.WriteTransactionData(strSQL2, CommandType.Text))
          {
            string strSQL3 = " UPDATE MODPREDET SET PRODEN = NULL, PRODENDET = NULL " + " " + filtroSql + " AND EXISTS ( SELECT * FROM MODPRE " + filtroSql + " AND CODSTAPRE = 0 " + " AND MODPREDET.CODPOS = MODPRE.CODPOS " + " AND MODPREDET.MAT = MODPRE.MAT" + " AND MODPREDET.PRORAP = MODPRE.PRORAP" + " AND MODPREDET.PROMOD = MODPRE.PROMOD)";
            return db.WriteTransactionData(strSQL3, CommandType.Text);
          }
        }
        return false;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static List<DENDET_Data> GetDataForDelete_1(
      DataLayer db,
      int codPos,
      int annDen,
      int mesDen,
      int proDen)
    {
      try
      {
        string strSQL = "SELECT * FROM DENDET" + " WHERE CODPOS = " + codPos.ToString() + " AND ANNDEN = " + annDen.ToString() + " AND MESDEN = " + mesDen.ToString() + " AND PRODEN = " + proDen.ToString() + " AND DATMOVANN Is NULL AND NUMMOVANN IS NULL" + " AND (ESIRET IS NULL OR ESIRET = 'N')";
        DataTable dataTable = db.GetDataTable(strSQL);
        if (dataTable.Rows.Count <= 0)
          return (List<DENDET_Data>) null;
        List<DENDET_Data> dataForDelete1 = new List<DENDET_Data>();
        foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          dataForDelete1.Add(new DENDET_Data()
          {
            CodPos = !DBNull.Value.Equals(row["CODPOS"]) ? Convert.ToInt32(row["CODPOS"]) : 0,
            Mat = !DBNull.Value.Equals(row["MAT"]) ? Convert.ToInt32(row["MAT"]) : 0,
            ProRap = !DBNull.Value.Equals(row["PRORAP"]) ? Convert.ToInt32(row["PRORAP"]) : 0,
            AnnCom = !DBNull.Value.Equals(row["ANNCOM"]) ? Convert.ToInt32(row["ANNCOM"]) : 0,
            ImpRet = !DBNull.Value.Equals(row["IMPRET"]) ? Convert.ToDecimal(row["IMPRET"]) : 0M,
            ImpOcc = !DBNull.Value.Equals(row["IMPOCC"]) ? (Decimal) Convert.ToInt32(row["IMPOCC"]) : 0M,
            ImpFig = !DBNull.Value.Equals(row["IMPFIG"]) ? (Decimal) Convert.ToInt32(row["IMPFIG"]) : 0M
          });
        return dataForDelete1;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static List<DENDET_Data> GetDataForDelete_2(
      DataLayer db,
      int codPos,
      int annDen,
      int mesDen,
      int proDen)
    {
      try
      {
        string strSQL = "SELECT DISTINCT CODPOS, ANNDEN, MESDEN, PRODEN " + " FROM DENDET WHERE IMPCONDEL <> 0 " + " AND CODPOS= " + codPos.ToString() + " AND ANNDEN= " + annDen.ToString() + " AND MESDEN= " + mesDen.ToString() + " AND PRODEN= " + proDen.ToString() + " AND ANNRET IS NOT NULL";
        DataTable dataTable = db.GetDataTable(strSQL);
        if (dataTable.Rows.Count <= 0)
          return (List<DENDET_Data>) null;
        List<DENDET_Data> dataForDelete2 = new List<DENDET_Data>();
        foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          dataForDelete2.Add(new DENDET_Data()
          {
            CodPos = !DBNull.Value.Equals(row["CODPOS"]) ? Convert.ToInt32(row["CODPOS"]) : 0,
            AnnDen = !DBNull.Value.Equals(row["ANNDEN"]) ? Convert.ToInt32(row["ANNDEN"]) : 0,
            MesDen = !DBNull.Value.Equals(row["MESDEN"]) ? Convert.ToInt32(row["MESDEN"]) : 0,
            ProDen = !DBNull.Value.Equals(row["PRODEN"]) ? Convert.ToInt32(row["PRODEN"]) : 0
          });
        return dataForDelete2;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static string GetFieldFirstValue_TipMov(DataLayer db, string codCau)
    {
      try
      {
        string strSQL = "SELECT TIPMOV FROM TIPMOVCAU WHERE CODCAU = '" + codCau + "'";
        DataTable dataTable = db.GetDataTable(strSQL);
        if (dataTable.Rows.Count <= 0)
          return (string) null;
        List<string> stringList = new List<string>();
        foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          stringList.Add(!DBNull.Value.Equals(row["TIPMOV"]) ? row["TIPMOV"].ToString() : string.Empty);
        return stringList[0];
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static string GetFieldFirstValue_CodCauAnn(
      DataLayer db,
      string tipMov,
      string dataSistema)
    {
      try
      {
        string strSQL = "SELECT CODCAU FROM TIPMOVCAU WHERE TIPMOV = " + DBMethods.DoublePeakForSql(tipMov) + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataSistema)) + " BETWEEN DATINI AND DATFIN";
        DataTable dataTable = db.GetDataTable(strSQL);
        if (dataTable.Rows.Count <= 0)
          return (string) null;
        List<string> stringList = new List<string>();
        foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          stringList.Add(!DBNull.Value.Equals(row["CODCAU"]) ? row["CODCAU"].ToString() : string.Empty);
        return stringList[0];
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static string GetDataSistema(DataLayer db)
    {
      try
      {
        string strSQL = "SELECT CURRENT_DATE AS DATASISTEMA FROM DBUNICONET.TIPIND";
        DataTable dataTable = db.GetDataTable(strSQL);
        if (dataTable.Rows.Count <= 0)
          return (string) null;
        List<string> stringList = new List<string>();
        foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          stringList.Add(!DBNull.Value.Equals(row["DATASISTEMA"]) ? row["DATASISTEMA"].ToString() : string.Empty);
        return stringList[0];
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static string GetFieldFirstValue_NumSanAnn(
      DataLayer db,
      int codPos,
      int annDen,
      int mesDen,
      int proDen)
    {
      try
      {
        string strSQL = " SELECT NUMSANANN FROM DENTES WHERE CODPOS = " + codPos.ToString() + " AND ANNDEN = " + annDen.ToString() + " AND MESDEN = " + mesDen.ToString() + " AND PRODEN = " + proDen.ToString() + " AND TIPMOV ='NU'";
        DataTable dataTable = db.GetDataTable(strSQL);
        if (dataTable.Rows.Count <= 0)
          return (string) null;
        List<string> stringList = new List<string>();
        foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          stringList.Add(!DBNull.Value.Equals(row["NUMSANANN"]) ? row["NUMSANANN"].ToString() : string.Empty);
        return stringList[0];
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static bool AnnullamentoRettifiche_1(
      DataLayer db,
      int codPos,
      int annDen,
      int mesDen,
      int proDen)
    {
      try
      {
        string strSQL1 = "DELETE FROM DENDET WHERE CODPOS=" + codPos.ToString() + " AND ANNDEN=" + annDen.ToString() + " AND MESDEN=" + mesDen.ToString() + " AND PRODEN=" + proDen.ToString() + " AND TIPMOV='RT' " + " AND NUMMOV IS NULL";
        if (!db.WriteTransactionData(strSQL1, CommandType.Text))
          return false;
        string strSQL2 = "UPDATE DENDET SET ESIRET=NULL WHERE CODPOS=" + codPos.ToString() + " AND ANNDEN=" + annDen.ToString() + " AND MESDEN=" + mesDen.ToString() + " AND PRODEN=" + proDen.ToString() + " AND ESIRET='S' " + " AND NUMGRURET IS NULL";
        return db.WriteTransactionData(strSQL2, CommandType.Text);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static bool AnnullamentoRettifiche_2(
      DataLayer db,
      int codPos,
      int annDen,
      int mesDen,
      int proDen,
      int i,
      string numeroMovimentoSap,
      string dataSistema,
      ref string tipoAnno,
      ref Decimal addizionale,
      clsWRITE_DB clsWRITE_DB,
      int annoBilancio,
      ref Decimal impConRet,
      ref Decimal impAddRecRet,
      ref Decimal impAbbRet,
      ref Decimal impAssConRet,
      ref string codCauAnn,
      ref string partitaSanzione,
      ref Decimal progMovSanzione,
      ref string numSanAnn,
      ModContabilita modContabilita,
      TFI.OCM.Utente.Utente utente,
      ref string tipMov,
      ref Decimal impSanDet,
      ref Decimal impAbb,
      ref Decimal impAddRec,
      ref Decimal impAssCon)
    {
      string MSGErrore = "";
      try
      {
        string str1 = "SELECT DISTINCT VALUE(ANNCOM, 9999) AS ANNCOM, CODCAUSAN " + " FROM DENDET WHERE CODPOS=" + codPos.ToString() + " AND ANNDEN=" + annDen.ToString() + " AND MESDEN=" + mesDen.ToString() + " AND PRODEN=" + proDen.ToString() + " AND TIPMOV='RT' ";
        string strSQL1 = (i != 1 ? str1 + " AND IMPCONDEL>0" : str1 + " AND IMPCONDEL<0") + " AND NUMMOVANN = " + DBMethods.DoublePeakForSql(numeroMovimentoSap);
        DataTable dataTable1 = db.GetDataTable(strSQL1);
        tipoAnno = string.Empty;
        if (dataTable1 != null)
        {
          foreach (DataRow row1 in (InternalDataCollectionBase) dataTable1.Rows)
          {
            int num = !DBNull.Value.Equals(row1["ANNCOM"]) ? Convert.ToInt32(row1["ANNCOM"]) : 0;
            string CODCAUSAN = !DBNull.Value.Equals(row1["CODCAUSAN"]) ? row1["CODCAUSAN"].ToString() : string.Empty;
            addizionale = Convert.ToDecimal(clsWRITE_DB.Module_GetValorePargen(db, 5, dataSistema, codPos));
            string str2 = "SELECT VALUE(SUM(IMPCONDEL),0.0) AS IMPCONRET, " + " ROUND(VALUE(SUM(IMPCONDEL), 0.0) * " + addizionale.ToString().Replace(",", ".") + " / 100 , 2) AS IMPADDRECRET, " + " VALUE(SUM(IMPASSCONDEL),0.0) AS IMPASSCONRET, " + " VALUE(SUM(IMPABBDEL),0.0) AS IMPABBRET " + " FROM DENDET WHERE CODPOS=" + codPos.ToString() + " AND ANNDEN = " + annDen.ToString() + " AND MESDEN = " + mesDen.ToString() + " AND PRODEN = " + proDen.ToString() + " AND TIPMOV = 'RT' ";
            string strSQL2 = (i != 1 ? str2 + " AND IMPCONDEL>0" : str2 + " AND IMPCONDEL<0") + " AND NUMMOVANN = " + DBMethods.DoublePeakForSql(numeroMovimentoSap) + string.Format(" AND VALUE(ANNCOM, 9999) = {0}", (object) num);
            tipoAnno = num != 9999 ? clsWRITE_DB.Module_GetTipoAnno(num, annoBilancio) : clsWRITE_DB.Module_GetTipoAnno(annDen, annoBilancio);
            addizionale = 0M;
            DataTable dataTable2 = db.GetDataTable(strSQL2);
            if (dataTable2.Rows.Count > 0)
            {
              impConRet = !DBNull.Value.Equals(dataTable2.Rows[0]["IMPCONRET"]) ? Convert.ToDecimal(dataTable2.Rows[0]["IMPCONRET"]) : 0M;
              impAddRecRet = !DBNull.Value.Equals(dataTable2.Rows[0]["IMPADDRECRET"]) ? Convert.ToDecimal(dataTable2.Rows[0]["IMPADDRECRET"]) : 0M;
              impAbbRet = !DBNull.Value.Equals(dataTable2.Rows[0]["IMPABBRET"]) ? Convert.ToDecimal(dataTable2.Rows[0]["IMPABBRET"]) : 0M;
              impAssConRet = !DBNull.Value.Equals(dataTable2.Rows[0]["IMPASSCONRET"]) ? Convert.ToDecimal(dataTable2.Rows[0]["IMPASSCONRET"]) : 0M;
              if (impConRet != 0M && i == 2)
              {
                string strSQL3 = "SELECT SUM(DENDET.IMPSANDET) AS IMPSANDET, DENDET.CODCAUSAN, RETTES.NUMSAN " + " FROM DENDET, RETTES " + " WHERE DENDET.ANNRET = RETTES.ANNRET " + " AND DENDET.PRORET = RETTES.PRORET " + " AND DENDET.PRORETTES = RETTES.PRORETTES " + " AND DENDET.CODPOS=" + codPos.ToString() + " AND DENDET.ANNDEN=" + annDen.ToString() + " AND DENDET.MESDEN=" + mesDen.ToString() + " AND DENDET.PRODEN=" + proDen.ToString() + " AND DENDET.NUMMOVANN = " + DBMethods.DoublePeakForSql(numeroMovimentoSap) + " AND RETTES.NUMSAN IS NOT NULL " + " AND VALUE(DENDET.ANNCOM, 9999) = " + num.ToString() + " AND DENDET.TIPMOV = 'RT' " + " GROUP BY DENDET.CODCAUSAN, RETTES.NUMSAN ";
                foreach (DataRow row2 in (InternalDataCollectionBase) db.GetDataTable(strSQL3).Rows)
                {
                  string strSQL4 = string.Format("SELECT TIPMOV FROM TIPMOVCAU WHERE CODCAU='{0}'", row2["CODCAUSAN"]);
                  DataTable dataTable3 = db.GetDataTable(strSQL4);
                  if (dataTable3.Rows.Count > 0)
                  {
                    codCauAnn = !DBNull.Value.Equals(dataTable3.Rows[0]["TIPMOV"]) ? dataTable3.Rows[0]["TIPMOV"].ToString() : string.Empty;
                    codCauAnn = "ANN_" + codCauAnn;
                  }
                  string strSQL5 = "SELECT VALUE(CODCAU,0) AS CODCAU FROM TIPMOVCAU WHERE TIPMOV='" + codCauAnn + "'";
                  DataTable dataTable4 = db.GetDataTable(strSQL5);
                  if (dataTable4.Rows.Count > 0)
                    codCauAnn = !DBNull.Value.Equals(dataTable4.Rows[0]["CODCAU"]) ? dataTable4.Rows[0]["CODCAU"].ToString() : string.Empty;
                  partitaSanzione = "";
                  progMovSanzione = 0M;
                  numSanAnn = modContabilita.WRITE_CONTABILITA_ANNULLAMENTO_SANZIONI(db, utente, ref MSGErrore, codPos, annDen, mesDen, proDen, tipoAnno, codCauAnn, annoBilancio.ToString(), tipMov, ref impSanDet, ref impAbb, ref impAddRec, ref impAssCon, ref partitaSanzione, ref progMovSanzione, CODCAUSAN, num);
                  if (!string.IsNullOrEmpty(MSGErrore))
                    return false;
                }
              }
            }
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        throw;
      }
    }
  }
}
