// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Amministrativo.NotificheUfficioDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using System;
using System.Collections.Generic;
using System.Data;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.Amministrativo;

namespace TFI.DAL.Amministrativo
{
  public class NotificheUfficioDAL
  {
    public static string GetDataDecorrenza(string codPos)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = "SELECT MIN(DATDEC) AS DATA_DEC FROM RAPLAV WHERE CODPOS= " + codPos;
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        return !DBNull.Value.Equals(dataTable.Rows[0]["DATA_DEC"]) ? dataTable.Rows[0]["DATA_DEC"].ToString().Trim() : string.Empty;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static void GetData(
      string dataA,
      string dataDa,
      int anno,
      int mese,
      string msk_codPos_da,
      string msk_codPos_a,
      string stringCodPos,
      List<NotificheUfficioOCM> listaNotifiche)
    {
      string empty = string.Empty;
      DataLayer dataLayer = new DataLayer();
      Utile utile = new Utile();
      try
      {
        DataTable dataTable1 = utile.CREA_DT_AUTORIZZAZIONI_ENPAIA(ref empty);
        string str1 = " SELECT DISTINCT RAPLAV.CODPOS, AZI.RAGSOC FROM RAPLAV " + " INNER JOIN AZI ON RAPLAV.CODPOS=AZI.CODPOS" + " WHERE AZI.NATGIU NOT IN (10) AND RAPLAV.DATDEC <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataA)) + " AND VALUE(RAPLAV.DATCES,'9999-12-31') >= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataDa)) + " AND" + string.Format(" RAPLAV.CODPOS NOT IN (SELECT CODPOS FROM DENTES WHERE ANNDEN={0} AND MESDEN={1}", (object) anno, (object) mese) + " AND TIPMOV = 'DP' AND DATCHI IS NOT NULL AND DATMOVANN IS NULL) " + string.Format(" AND RAPLAV.CODPOS NOT IN(SELECT CODPOS FROM DENTES WHERE ANNDEN= {0} AND MESDEN={1}", (object) anno, (object) mese) + " AND TIPMOV = 'NU' AND DATCONMOV IS NOT NULL AND DATMOVANN IS NULL)";
        if (!string.IsNullOrEmpty(msk_codPos_da))
          str1 = str1 + " AND RAPLAV.CODPOS>= " + msk_codPos_da;
        if (!string.IsNullOrEmpty(msk_codPos_a))
          str1 = str1 + " AND RAPLAV.CODPOS<= " + msk_codPos_a;
        for (int index = 0; index <= dataTable1.Rows.Count - 1; ++index)
        {
          if ((!DBNull.Value.Equals(dataTable1.Rows[index]["ABILITATO"]) ? dataTable1.Rows[index]["ABILITATO"].ToString() : string.Empty) == "NO")
          {
            string str2 = !DBNull.Value.Equals(dataTable1.Rows[index]["POSIZIONE"]) ? dataTable1.Rows[index]["POSIZIONE"].ToString() : string.Empty;
            stringCodPos = stringCodPos + "," + str2;
          }
        }
        if (stringCodPos.ToString().Trim() != string.Empty)
        {
          stringCodPos = stringCodPos.Substring(1);
          str1 = str1 + " AND RAPLAV.CODPOS NOT IN (" + stringCodPos + ") ";
        }
        string strSQL = str1 + " ORDER BY RAPLAV.CODPOS";
        DataTable dataTable2 = dataLayer.GetDataTable(strSQL);
        stringCodPos = "";
        if (dataTable2.Rows.Count <= 0)
          return;
        foreach (DataRow row in (InternalDataCollectionBase) dataTable2.Rows)
          listaNotifiche.Add(new NotificheUfficioOCM()
          {
            CodPos = !DBNull.Value.Equals(row["CODPOS"]) ? Convert.ToInt32(row["CODPOS"]) : 0,
            RagSoc = !DBNull.Value.Equals(row["RAGSOC"]) ? row["RAGSOC"].ToString() : string.Empty,
            Anno = anno,
            Mese = Utile.GetMesi()[mese].ToUpper(),
            NumMese = mese,
            TipIsc = "O"
          });
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static string GetCausaleMovimento(string tipMov)
    {
      try
      {
        return new DataLayer().Get1ValueFromSQL("SELECT CODCAU FROM TIPMOVCAU WHERE TIPMOV = " + DBMethods.DoublePeakForSql(tipMov) + " AND CURRENT_DATE BETWEEN DATINI AND DATFIN", CommandType.Text);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static List<TFI.OCM.Amministrativo.CercaNotifiche> CercaNotifiche()
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = " SELECT DENTES.CODPOS, AZI.RAGSOC, DENTES.ANNDEN, DENTES.MESDEN, DENTES.PRODEN," + " (SELECT MESE FROM MESI WHERE CODMES = DENTES.MESDEN) AS DENMESE,  " + " VALUE(DENTES.IMPDIS, 0) AS IMPDIS, " + " CASE VALUE(SANSOTSOG, '') WHEN 'S' THEN 0 ELSE IMPSANDET END AS IMPSANDET, " + " VALUE(DENTES.IMPCON, 0) AS IMPCON, DENTES.IMPADDREC, DENTES.IMPABB, DENTES.DATSCA, DENTES.SANSOTSOG, " + " DENTES.IMPASSCON, DENTES.CODCAUSAN, '' AS CANCELLATE_DAL_DIPA " + " FROM DENTES INNER JOIN AZI ON DENTES.CODPOS = AZI.CODPOS " + " WHERE DENTES.TIPMOV = 'NU' AND DENTES.DATCONMOV IS NULL AND DATMOVANN IS NULL";
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count <= 0)
          return (List<TFI.OCM.Amministrativo.CercaNotifiche>) null;
        List<TFI.OCM.Amministrativo.CercaNotifiche> cercaNotificheList = new List<TFI.OCM.Amministrativo.CercaNotifiche>();
        foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
        {
          TFI.OCM.Amministrativo.CercaNotifiche cercaNotifiche = new TFI.OCM.Amministrativo.CercaNotifiche();
          cercaNotifiche.CodPos = !DBNull.Value.Equals(row["CODPOS"]) ? Convert.ToInt32(row["CODPOS"]) : 0;
          cercaNotifiche.RagSoc = !DBNull.Value.Equals(row["RAGSOC"]) ? row["RAGSOC"].ToString() : string.Empty;
          cercaNotifiche.AnnDen = !DBNull.Value.Equals(row["ANNDEN"]) ? Convert.ToInt32(row["ANNDEN"]) : 0;
          cercaNotifiche.MesDen = !DBNull.Value.Equals(row["MESDEN"]) ? Convert.ToInt32(row["MESDEN"]) : 0;
          cercaNotifiche.ProDen = !DBNull.Value.Equals(row["PRODEN"]) ? Convert.ToInt32(row["PRODEN"]) : 0;
          cercaNotifiche.DenMese = !DBNull.Value.Equals(row["DENMESE"]) ? row["DENMESE"].ToString() : string.Empty;
          cercaNotifiche.ImpDis = !DBNull.Value.Equals(row["IMPDIS"]) ? Convert.ToDecimal(row["IMPDIS"]) : 0M;
          cercaNotifiche.ImpSanDet = !DBNull.Value.Equals(row["IMPSANDET"]) ? Convert.ToDecimal(row["IMPSANDET"]) : 0M;
          cercaNotifiche.ImpCon = !DBNull.Value.Equals(row["IMPCON"]) ? Convert.ToDecimal(row["IMPCON"]) : 0M;
          cercaNotifiche.ImpAbb = !DBNull.Value.Equals(row["IMPABB"]) ? Convert.ToDecimal(row["IMPABB"]) : 0M;
          cercaNotifiche.DatSca = !DBNull.Value.Equals(row["DATSCA"]) ? row["DATSCA"].ToString() : string.Empty;
          cercaNotifiche.SanSotSog = !DBNull.Value.Equals(row["SANSOTSOG"]) ? row["SANSOTSOG"].ToString() : string.Empty;
          cercaNotifiche.ImpAssCon = !DBNull.Value.Equals(row["IMPASSCON"]) ? Convert.ToDecimal(row["IMPASSCON"]) : 0M;
          cercaNotifiche.CodCauSan = !DBNull.Value.Equals(row["CODCAUSAN"]) ? row["CODCAUSAN"].ToString() : string.Empty;
          cercaNotifiche.CancellateDalDIPA = !DBNull.Value.Equals(row["CANCELLATE_DAL_DIPA"]) ? row["CANCELLATE_DAL_DIPA"].ToString() : string.Empty;
          cercaNotificheList.Add(cercaNotifiche);
        }
        return cercaNotificheList;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static string Module_GetNewTimeStamp(DataLayer db)
    {
      try
      {
        string strSQL = "SELECT CURRENT_TIMESTAMP AS NEWTIMESTAMP FROM DBUNICONET.TIPIND";
        return db.Get1ValueFromSQL(strSQL, CommandType.Text);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static bool ChecK_Dipa_Confermato(DataLayer db, int CODPOS, int ANNO, int MESE)
    {
      int num = 0;
      try
      {
        string strSQL1 = " SELECT COUNT(*) AS DIPA FROM DENTES" + " WHERE CODPOS= " + CODPOS.ToString() + " AND ANNDEN = " + ANNO.ToString() + " AND MESDEN =" + MESE.ToString() + " AND DATCHI IS NOT NULL AND DATMOVANN IS NULL AND TIPMOV = 'DP' ";
        DataTable dataTable1 = db.GetDataTable(strSQL1);
        if (dataTable1.Rows.Count > 0)
          num = Convert.ToInt32(dataTable1.Rows[0]["DIPA"]);
        if (num != 0)
          return true;
        string strSQL2 = " SELECT COUNT(*) AS DIPA FROM DENTES" + " WHERE CODPOS= " + CODPOS.ToString() + " AND ANNDEN = " + ANNO.ToString() + " AND MESDEN =" + MESE.ToString() + " AND TIPMOV = 'NU' " + " AND NUMMOV IS NOT NULL " + " AND DATMOVANN IS NULL ";
        DataTable dataTable2 = db.GetDataTable(strSQL2);
        if (dataTable2.Rows.Count > 0)
          num = Convert.ToInt32(dataTable2.Rows[0]["DIPA"]);
        if (num != 0)
          return true;
        string strSQL3 = "DELETE FROM DENDETALI WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN=" + ANNO.ToString() + " AND MESDEN=" + MESE.ToString() + " AND PRODEN IN (SELECT PRODEN FROM DENTES WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN=" + ANNO.ToString() + " AND MESDEN=" + MESE.ToString() + " AND DATCHI IS NULL" + " AND DATMOVANN IS NULL" + " AND TIPMOV = 'DP')";
        db.WriteTransactionData(strSQL3, CommandType.Text);
        string strSQL4 = "DELETE FROM DENDETSOS WHERE CODPOS=" + CODPOS.ToString() + " AND ANNDEN=" + ANNO.ToString() + " AND MESDEN=" + MESE.ToString() + " AND PRODEN IN (SELECT PRODEN FROM DENTES WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN=" + ANNO.ToString() + " AND MESDEN=" + MESE.ToString() + " AND DATCHI IS NULL" + " AND DATMOVANN IS NULL" + " AND TIPMOV = 'DP')";
        db.WriteTransactionData(strSQL4, CommandType.Text);
        string strSQL5 = "DELETE FROM DENDET WHERE CODPOS=" + CODPOS.ToString() + " AND ANNDEN=" + ANNO.ToString() + " AND MESDEN=" + MESE.ToString() + " AND PRODEN IN (SELECT PRODEN FROM DENTES WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN=" + ANNO.ToString() + " AND MESDEN=" + MESE.ToString() + " AND DATCHI IS NULL" + " AND DATMOVANN IS NULL" + " AND TIPMOV = 'DP')";
        db.WriteTransactionData(strSQL5, CommandType.Text);
        string strSQL6 = "DELETE FROM DENTES WHERE CODPOS=" + CODPOS.ToString() + " AND ANNDEN=" + ANNO.ToString() + " AND MESDEN=" + MESE.ToString() + " AND DATCHI IS NULL" + " AND DATMOVANN IS NULL" + " AND TIPMOV = 'DP'";
        db.WriteTransactionData(strSQL6, CommandType.Text);
        return false;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static void Contabilizza_Notifiche(
      DataLayer db,
      int CODPOS,
      int ANNO,
      int MESE,
      int PRODEN,
      string CODCAUMOV,
      string NUMERO_MOVIMENTO_SAP,
      string CODCAUSAN,
      string NUMERO_SANZIONE_SAP,
      int ANNBIL,
      string TIPANN,
      string DATASISTEMA,
      string strNomFileMav,
      string PARTITA_MOVIMENTO,
      Decimal PROGMOV_MOVIMENTO,
      string PARTITA_SANZIONE,
      Decimal PROGMOV_SANZIONE,
      TFI.OCM.Utente.Utente utente)
    {
      string numeroRicevuta = NotificheUfficioDAL.Module_Get_NumeroRicevuta(db, "NU");
      DataTable dataTable1 = new DataTable();
      clsWRITE_DB clsWriteDb = new clsWRITE_DB();
      try
      {
        string str1 = "UPDATE DENTES SET DATCONMOV = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATASISTEMA)) + ", " + " CODCAUMOV = " + DBMethods.DoublePeakForSql(CODCAUMOV.Trim().PadLeft(2, '0')) + ", " + " NUMMOV = " + DBMethods.DoublePeakForSql(NUMERO_MOVIMENTO_SAP) + ", ";
        if (CODCAUSAN != "")
          str1 = str1 + " CODCAUSAN = " + DBMethods.DoublePeakForSql(CODCAUSAN.Trim().PadLeft(2, '0')) + ", ";
        if (NUMERO_SANZIONE_SAP != "")
          str1 = str1 + " NUMSAN = " + DBMethods.DoublePeakForSql(NUMERO_SANZIONE_SAP) + ", " + " DATSAN = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATASISTEMA)) + ", " + " ANNBILSAN = " + ANNBIL.ToString() + ", " + " TIPANNSAN = " + DBMethods.DoublePeakForSql(TIPANN) + ", ";
        string strSQL1 = str1 + " TIPANNMOV = " + DBMethods.DoublePeakForSql(TIPANN) + ", " + " ANNBILMOV = " + ANNBIL.ToString() + ", " + " STADEN = 'A', " + " NUMRIC = " + DBMethods.DoublePeakForSql(numeroRicevuta) + ", " + " DATORARIC = CURRENT_TIMESTAMP, " + " NOMFILMAV = 'N'," + " CODMODPAG = 1, " + " CODLINE = '" + CODPOS.ToString().Trim().PadLeft(6, '0') + NUMERO_MOVIMENTO_SAP.Substring(5, 2) + NUMERO_MOVIMENTO_SAP.Substring(0, 2) + NUMERO_MOVIMENTO_SAP.Substring(8).PadLeft(3, '0') + "'" + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN=" + ANNO.ToString() + " AND MESDEN=" + MESE.ToString() + " AND PRODEN=" + PRODEN.ToString() + " AND TIPMOV = 'NU' ";
        db.WriteTransactionData(strSQL1, CommandType.Text);
        string str2 = "UPDATE DENDET SET DATCONMOV = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATASISTEMA)) + ", NUMMOV = " + DBMethods.DoublePeakForSql(NUMERO_MOVIMENTO_SAP);
        if (CODCAUSAN != "")
          str2 = str2 + ", CODCAUSAN = " + DBMethods.DoublePeakForSql(CODCAUSAN.Trim().PadLeft(2, '0'));
        if (NUMERO_SANZIONE_SAP != "")
          str2 = str2 + ", NUMSAN = " + DBMethods.DoublePeakForSql(NUMERO_SANZIONE_SAP) + ", PARTITASAN = " + DBMethods.DoublePeakForSql(PARTITA_SANZIONE) + ", PROGMOVSAN = " + PROGMOV_SANZIONE.ToString();
        string strSQL2 = str2 + ", PARTITA = " + DBMethods.DoublePeakForSql(PARTITA_MOVIMENTO) + ", PROGMOV = " + PROGMOV_MOVIMENTO.ToString() + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN=" + ANNO.ToString() + " AND MESDEN=" + MESE.ToString() + " AND PRODEN=" + PRODEN.ToString();
        db.WriteTransactionData(strSQL2, CommandType.Text);
        string strSQL3 = "SELECT * FROM DENDET" + " WHERE CODPOS = " + CODPOS.ToString() + " AND ANNDEN = " + ANNO.ToString() + " AND MESDEN = " + MESE.ToString() + " AND PRODEN = " + PRODEN.ToString();
        DataTable dataTable2 = db.GetDataTable(strSQL3);
        for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
        {
          if (!clsWriteDb.AGGIORNA_RETANN(db, utente, dataTable2.Rows[index][nameof (CODPOS)].ToString(), dataTable2.Rows[index]["MAT"].ToString(), Convert.ToInt32(dataTable2.Rows[index]["PRORAP"]), Convert.ToInt32(dataTable2.Rows[index]["ANNDEN"]), Convert.ToDecimal(dataTable2.Rows[index]["IMPRET"]), Convert.ToDecimal(dataTable2.Rows[index]["IMPOCC"]), Convert.ToDecimal(dataTable2.Rows[index]["IMPFIG"]), "+"))
            throw new Exception("Si sono verificati errori durante la contabilizzazione delle notifiche.");
        }
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static string Module_Get_NumeroRicevuta(DataLayer db, string TIPMOV)
    {
      DataTable dataTable1 = new DataTable();
      DateTime dateTime = Convert.ToDateTime(Utile.GetDataSistema());
      try
      {
        string strSQL = "SELECT VALUE(MAX( DECIMAL( VALUE( SUBSTR(NUMRIC, 6), '0'))), 0) AS NUMRIC " + " FROM DENTES ";
        string str = TIPMOV.Trim();
        if (!(str == "NU"))
        {
          if (str == "DP" || str == "AR")
            strSQL = strSQL + " WHERE YEAR(DATCHI) = " + dateTime.Year.ToString();
        }
        else
          strSQL = strSQL + " WHERE YEAR(DATCONMOV) = " + dateTime.Year.ToString();
        DataTable dataTable2 = db.GetDataTable(strSQL);
        int num = dataTable2.Rows.Count != 0 ? Convert.ToInt32(dataTable2.Rows[0]["NUMRIC"]) + 1 : 1;
        return dateTime.Year.ToString() + "/" + num.ToString();
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static void Cancella_Notifiche(
      DataLayer db,
      int CODPOS,
      int ANNO,
      int MESE,
      int PRODEN)
    {
      try
      {
        string strSQL1 = "DELETE FROM DENDETALI WHERE CODPOS=" + CODPOS.ToString() + " AND ANNDEN=" + ANNO.ToString() + " AND MESDEN=" + MESE.ToString() + " AND PRODEN=" + PRODEN.ToString();
        db.WriteTransactionData(strSQL1, CommandType.Text);
        string strSQL2 = "DELETE FROM DENDETSOS WHERE CODPOS=" + CODPOS.ToString() + " AND ANNDEN=" + ANNO.ToString() + " AND MESDEN=" + MESE.ToString() + " AND PRODEN=" + PRODEN.ToString();
        db.WriteTransactionData(strSQL2, CommandType.Text);
        string strSQL3 = "DELETE FROM DENDET WHERE CODPOS=" + CODPOS.ToString() + " AND ANNDEN=" + ANNO.ToString() + " AND MESDEN=" + MESE.ToString() + " AND PRODEN=" + PRODEN.ToString();
        db.WriteTransactionData(strSQL3, CommandType.Text);
        string strSQL4 = "DELETE FROM DENTES WHERE CODPOS=" + CODPOS.ToString() + " AND ANNDEN=" + ANNO.ToString() + " AND MESDEN=" + MESE.ToString() + " AND PRODEN=" + PRODEN.ToString() + " AND TIPMOV='NU' ";
        db.WriteTransactionData(strSQL4, CommandType.Text);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static List<NotificheUfficioOCM> CaricaDati(
      NotificheUfficioOCM notifiche,
      string MovRet)
    {
      DataTable dataTable1 = new DataTable();
      DataLayer dataLayer = new DataLayer();
      List<NotificheUfficioOCM> notificheUfficioOcmList = new List<NotificheUfficioOCM>();
      try
      {
        int codPos = notifiche.CodPos;
        string strSQL1 = "SELECT DENTIPMOV FROM TIPMOV WHERE TIPMOV IN (SELECT TIPMOV FROM DENTES " + " WHERE CODPOS = " + codPos.ToString() + " AND ANNDEN = " + notifiche.AnnDen + " AND MESDEN = " + notifiche.MesDen + " AND PRODEN = " + notifiche.ProDen + ")";
        DataTable dataTable2 = dataLayer.GetDataTable(strSQL1);
        notifiche.TipMov = !(MovRet == "") ? MovRet : dataTable2.Rows[0]["DENTIPMOV"].ToString();
        string str1 = "SELECT AZI.CODPOS, AZI.RAGSOC, AZI.DATAPE" + " FROM AZI";
        codPos = notifiche.CodPos;
        string str2 = codPos.ToString();
        string strSQL2 = str1 + " WHERE AZI.CODPOS = " + str2;
        dataTable2.Clear();
        DataTable dataTable3 = dataLayer.GetDataTable(strSQL2);
        if (dataTable3.Rows.Count > 0)
        {
          notifiche.CodPos = Convert.ToInt32(dataTable3.Rows[0]["CODPOS"]);
          notifiche.RagSoc = dataTable3.Rows[0]["RAGSOC"].ToString().Trim();
        }
        string strSQL3 = "SELECT TRIM(COG) || ' ' || TRIM(NOM) AS NOMINATIVO " + " FROM ISCT" + " WHERE MAT = " + notifiche.Matricola;
        dataTable3.Clear();
        DataTable dataTable4 = dataLayer.GetDataTable(strSQL3);
        notifiche.Nominativo = dataTable4.Rows[0]["NOMINATIVO"].ToString().Trim();
        string str3 = "SELECT CODPOS, ANNDEN, MESDEN, PRODEN, MAT, PRODENDET, CODFORASS, " + " (SELECT DENFORASS FROM FORASS WHERE CODFORASS = DENDETALI.CODFORASS) AS DENFORASS ," + " IMPCON, ALIQUOTA, ULTAGG, UTEAGG " + " FROM DENDETALI ";
        codPos = notifiche.CodPos;
        string str4 = codPos.ToString();
        string strSQL4 = str3 + " WHERE CODPOS = " + str4 + " AND ANNDEN = " + notifiche.AnnDen + " AND MESDEN = " + notifiche.MesDen + " AND PRODEN = " + notifiche.ProDen + " AND MAT = " + notifiche.Matricola + " AND PRODENDET = " + notifiche.ProDenDet + " ORDER BY DENFORASS";
        dataTable4.Clear();
        DataTable dataTable5 = dataLayer.GetDataTable(strSQL4);
        if (dataTable5.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable5.Rows)
          {
            NotificheUfficioOCM notificheUfficioOcm = new NotificheUfficioOCM()
            {
              CodPos = Convert.ToInt32(row["CODPOS"]),
              AnnDen = row["ANNDEN"].ToString(),
              MesDen = row["MESDEN"].ToString(),
              Matricola = row["MAT"].ToString(),
              DENFORASS = row["DENFORASS"].ToString(),
              Impcon = row["IMPCON"].ToString(),
              Aliq = row["ALIQUOTA"].ToString()
            };
            notificheUfficioOcmList.Add(notificheUfficioOcm);
          }
        }
        return notificheUfficioOcmList;
      }
      catch (Exception ex)
      {
        return (List<NotificheUfficioOCM>) null;
      }
    }

    public static List<NotificheUfficioOCM> DettaglioDenuncia(
      NotificheUfficioOCM notifiche,
      bool DOCUMENTO_ORIGINALE,
      ref string RagSoc,
      ref string tipomovimento)
    {
      DataLayer dataLayer = new DataLayer();
      DataTable dataTable1 = new DataTable();
      List<NotificheUfficioOCM> notificheUfficioOcmList = new List<NotificheUfficioOCM>();
      try
      {
        string strSQL1 = "SELECT RAGSOC FROM AZI WHERE CODPOS ='" + notifiche.CodPos.ToString() + "'";
        RagSoc = dataLayer.Get1ValueFromSQL(strSQL1, CommandType.Text);
        int codPos = notifiche.CodPos;
        string strSQL2 = "SELECT DENTIPMOV FROM TIPMOV WHERE TIPMOV IN (SELECT TIPMOV FROM DENTES " + " WHERE CODPOS = " + codPos.ToString() + " AND ANNDEN = '" + notifiche.AnnDen + "'" + " AND MESDEN = '" + notifiche.MesDen + "'" + " AND PRODEN = '" + notifiche.ProDen + "')";
        tipomovimento = dataLayer.Get1ValueFromSQL(strSQL2, CommandType.Text);
        string str1 = "SELECT DENDET.*, ISCT.COG, ISCT.NOM, " + " (SELECT DENGRUASS FROM GRUASS WHERE CODGRUASS = DENDET.CODGRUASS) AS DENGRUASS, " + " (SELECT DENQUA FROM QUACON WHERE CODQUACON = DENDET.CODQUACON) AS DENQUA, " + " (SELECT DENLIV FROM CONLIV WHERE CODCON = DENDET.CODCON AND PROCON = DENDET.PROCON AND CODLIV=DENDET.CODLIV) AS DENLIV, " + " (SELECT DENTIPRAP FROM TIPRAP WHERE TIPRAP = DENDET.TIPRAP) AS DENTIPRAP " + " FROM DENDET INNER JOIN ISCT ON DENDET.MAT = ISCT.MAT";
        codPos = notifiche.CodPos;
        string str2 = codPos.ToString();
        string str3 = str1 + " WHERE DENDET.CODPOS = '" + str2 + "'" + " AND DENDET.ANNDEN = '" + notifiche.AnnDen + "'" + " AND DENDET.MESDEN = '" + notifiche.MesDen + "'" + " AND DENDET.PRODEN = '" + notifiche.ProDen + "'";
        string strSQL3 = (!DOCUMENTO_ORIGINALE ? str3 + " AND VALUE(DENDET.ESIRET,'') <> 'S' AND DENDET.NUMMOVANN IS NULL" : str3 + " AND DENDET.TIPMOV <> 'RT'") + " ORDER BY COG, NOM, DENDET.MAT";
        dataTable1.Clear();
        DataTable dataTable2 = dataLayer.GetDataTable(strSQL3);
        if (dataTable2.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable2.Rows)
          {
            NotificheUfficioOCM notificheUfficioOcm = new NotificheUfficioOCM()
            {
              CodPos = Convert.ToInt32(row["CODPOS"]),
              Matricola = row["MAT"].ToString(),
              AnnDen = row["ANNDEN"].ToString(),
              MesDen = row["MESDEN"].ToString(),
              ProDen = row["PRODEN"].ToString(),
              ProDenDet = row["PRODENDET"].ToString(),
              Dal = row["DAL"].ToString(),
              Al = row["AL"].ToString(),
              TipMov = row["TIPMOV"].ToString(),
              Impcon = row["IMPCON"].ToString(),
              Imprett = row["IMPRET"].ToString(),
              Impfig = row["IMPFIG"].ToString(),
              ImpOcc = row["IMPOCC"].ToString(),
              Aliq = row["ALIQUOTA"].ToString(),
              Datces = row["DATCES"].ToString(),
              Datdec = row["DATDEC"].ToString(),
              Dengruass = row["DENGRUASS"].ToString(),
              Denliv = row["DENLIV"].ToString(),
              Denqua = row["DENQUA"].ToString(),
              Dentiprap = row["DENTIPRAP"].ToString(),
              Nominativo = row["COG"].ToString() + " " + row["NOM"].ToString()
            };
            notificheUfficioOcmList.Add(notificheUfficioOcm);
          }
        }
        return notificheUfficioOcmList;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static StampaDocumentoNotifiche GetStampaObj(
      string codPos,
      string annDen,
      string mesDen,
      string proDen)
    {
      try
      {
        DataTable dataTable = new DataLayer().GetDataTable("SELECT NUMMOV, ESIRET FROM DENTES WHERE CODPOS = " + codPos + " AND ANNDEN = " + annDen + " AND MESDEN= " + mesDen + " AND PRODEN= " + proDen);
        if (dataTable.Rows.Count <= 0)
          return (StampaDocumentoNotifiche) null;
        return new StampaDocumentoNotifiche()
        {
          NumMov = dataTable.Rows[0]["NUMMOV"].ToString(),
          EsiRet = dataTable.Rows[0]["ESIRET"].ToString()
        };
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static List<StampaDocumentoNotifiche> GetStampaList(
      string codPos,
      string annDen,
      string mesDen,
      string proDen)
    {
      try
      {
        DataTable dataTable = new DataLayer().GetDataTable("SELECT DISTINCT ANNRET, PRORET, PRORETTES " + " FROM DENDET WHERE IMPCONDEL <> 0 " + " AND CODPOS= " + codPos + " AND ANNDEN= " + annDen + " AND MESDEN= " + mesDen + " AND PRODEN= " + proDen + " AND ANNRET IS NOT NULL" + " AND NUMMOVANN IS NULL");
        if (dataTable.Rows.Count <= 0)
          return (List<StampaDocumentoNotifiche>) null;
        List<StampaDocumentoNotifiche> stampaList = new List<StampaDocumentoNotifiche>();
        foreach (DataRow row in (InternalDataCollectionBase) dataTable.Rows)
          stampaList.Add(new StampaDocumentoNotifiche()
          {
            AnnRet = !DBNull.Value.Equals(row["ANNRET"]) ? Convert.ToInt32(row["ANNRET"]) : 0,
            ProRet = !DBNull.Value.Equals(row["PRORET"]) ? Convert.ToInt32(row["PRORET"]) : 0,
            ProRetTes = !DBNull.Value.Equals(row["PRORETTES"]) ? Convert.ToInt32(row["PRORETTES"]) : 0
          });
        return stampaList;
      }
      catch (Exception ex)
      {
        throw;
      }
    }
  }
}
