// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Amministrativo.DelegheDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using TFI.CRYPTO.Crypto;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.Amministrativo;

namespace TFI.DAL.Amministrativo
{
  public class DelegheDAL
  {
    private DelegheOCM Deleghe = new DelegheOCM();
    private DataLayer objDataAccess = new DataLayer();

    public DelegheOCM SearcheDeleghe(
      string PosAz,
      string Partitaiva,
      string tipass,
      ref string ErroreMSG,
      ref string SuccessMSG)
    {
      DelegheOCM delegheOcm = new DelegheOCM();
      try
      {
        string str = "";
        if (!string.IsNullOrEmpty(PosAz) && string.IsNullOrEmpty(Partitaiva) && string.IsNullOrEmpty(tipass))
          str = str + " AZI.CODPOS = " + DBMethods.DoublePeakForSql(PosAz) + " ";
        else if (string.IsNullOrEmpty(PosAz) && !string.IsNullOrEmpty(Partitaiva) && string.IsNullOrEmpty(tipass))
          str = str + " AZI.PARIVA = " + DBMethods.DoublePeakForSql(Partitaiva) + " ";
        else if (string.IsNullOrEmpty(PosAz) && string.IsNullOrEmpty(Partitaiva) && !string.IsNullOrEmpty(tipass))
          str = str + " ASSTER.CODTER = " + DBMethods.DoublePeakForSql(tipass) + " ";
        else if (!string.IsNullOrEmpty(PosAz) && !string.IsNullOrEmpty(Partitaiva) && string.IsNullOrEmpty(tipass))
          str = str + " AZI.CODPOS = " + DBMethods.DoublePeakForSql(PosAz) + " AND  AZI.PARIVA = " + DBMethods.DoublePeakForSql(Partitaiva) + " ";
        else if (!string.IsNullOrEmpty(PosAz) && string.IsNullOrEmpty(Partitaiva) && !string.IsNullOrEmpty(tipass))
          str = str + " AZI.CODPOS = " + DBMethods.DoublePeakForSql(PosAz) + " AND ASSTER.CODTER = " + DBMethods.DoublePeakForSql(tipass) + " ";
        else if (string.IsNullOrEmpty(PosAz) && !string.IsNullOrEmpty(Partitaiva) && !string.IsNullOrEmpty(tipass))
          str = str + " AZI.PARIVA = " + DBMethods.DoublePeakForSql(Partitaiva) + " AND ASSTER.CODTER = " + DBMethods.DoublePeakForSql(tipass) + " ";
        else if (!string.IsNullOrEmpty(PosAz) && !string.IsNullOrEmpty(Partitaiva) && !string.IsNullOrEmpty(tipass))
          str = str + " AZI.CODPOS = " + DBMethods.DoublePeakForSql(PosAz) + " AND AZI.PARIVA = " + DBMethods.DoublePeakForSql(Partitaiva) + " AND ASSTER.CODTER = " + DBMethods.DoublePeakForSql(tipass) + " ";
        string strSQL1 = "SELECT * FROM (SELECT DELEGHE.DATINI, DELEGHE.DATFIN, DELEGHE.CODTER, " + " ASSTER.RAGSOC AS ASSTER, ASSNAZ.CODNAZ, ASSNAZ.RAGSOC AS ASSNAZ, " + " AZI.CODPOS, AZI.RAGSOC, " + " (SELECT CASE CHAR(COUNT(*)) WHEN '0' THEN 'SENZA CONSULENTE REGISTRATO' ELSE 'CON CONSULENTE REGISTRATO' END CASE FROM ASSTER WHERE ASSTER.CODTER = DELEGHE.CODTER AND CURRENT_DATE BETWEEN DELEGHE.DATINI AND DELEGHE.DATFIN) AS STATO " + " FROM ASSNAZ RIGHT JOIN ASSTER ON ASSNAZ.CODNAZ = ASSTER.CODNAZ " + " RIGHT JOIN DELEGHE ON ASSTER.CODTER = DELEGHE.CODTER " + " LEFT JOIN AZI ON DELEGHE.CODPOS = AZI.CODPOS " + " WHERE CURRENT_DATE BETWEEN DELEGHE.DATINI AND DELEGHE.DATFIN AND" + str + " ORDER BY ASSNAZ) AS TABELLA ";
        string strSQL2 = "SELECT * FROM (SELECT DELEGHE.DATINI, DELEGHE.DATFIN, DELEGHE.CODTER, " + " ASSTER.RAGSOC AS ASSTER, ASSNAZ.CODNAZ, ASSNAZ.RAGSOC AS ASSNAZ, " + " AZI.CODPOS, AZI.RAGSOC, " + " (SELECT CASE CHAR(COUNT(*)) WHEN '0' THEN 'SENZA CONSULENTE REGISTRATO' ELSE 'CON CONSULENTE REGISTRATO' END CASE FROM ASSTER WHERE ASSTER.CODTER = DELEGHE.CODTER AND CURRENT_DATE BETWEEN DELEGHE.DATINI AND DELEGHE.DATFIN) AS STATO " + " FROM ASSNAZ RIGHT JOIN ASSTER ON ASSNAZ.CODNAZ = ASSTER.CODNAZ " + " RIGHT JOIN DELEGHE ON ASSTER.CODTER = DELEGHE.CODTER " + " LEFT JOIN AZI ON DELEGHE.CODPOS = AZI.CODPOS " + " WHERE CURRENT_DATE NOT BETWEEN DELEGHE.DATINI AND DELEGHE.DATFIN AND" + str + " ORDER BY ASSNAZ) AS TABELLA ";
        string strSQL3 = "SELECT * FROM (SELECT DELEGHE.DATINI, DELEGHE.DATFIN, DELEGHE.CODTER, " + " ASSTER.RAGSOC AS ASSTER, ASSNAZ.CODNAZ, ASSNAZ.RAGSOC AS ASSNAZ, " + " AZI.CODPOS, AZI.RAGSOC, " + " (SELECT CASE CHAR(COUNT(*)) WHEN '0' THEN 'SENZA CONSULENTE REGISTRATO' ELSE 'CON CONSULENTE REGISTRATO' END CASE FROM ASSTER WHERE ASSTER.CODTER = DELEGHE.CODTER AND CURRENT_DATE BETWEEN DELEGHE.DATINI AND DELEGHE.DATFIN) AS STATO " + " FROM ASSNAZ RIGHT JOIN ASSTER ON ASSNAZ.CODNAZ = ASSTER.CODNAZ " + " RIGHT JOIN DELEGHE ON ASSTER.CODTER = DELEGHE.CODTER " + " LEFT JOIN AZI ON DELEGHE.CODPOS = AZI.CODPOS " + " WHERE DELEGHE.FLGATT = 0 AND" + str + " ORDER BY ASSNAZ) AS TABELLA ";
        DataTable dataTable1 = this.objDataAccess.GetDataTable(strSQL1);
        DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL2);
        DataTable dataTable3 = this.objDataAccess.GetDataTable(strSQL3);
        if (dataTable1.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable1.Rows)
          {
            DelegheOCM.DelegheAttive delegheAttive = new DelegheOCM.DelegheAttive()
            {
              datini = row["DATINI"].ToString(),
              datfin = row["DATFIN"].ToString().Substring(0, 10),
              codter = row["CODTER"].ToString(),
              asster = row["ASSTER"].ToString(),
              codnaz = row["CODNAZ"].ToString(),
              assnaz = row["ASSNAZ"].ToString(),
              codpos = row["CODPOS"].ToString(),
              ragsoc = row["RAGSOC"].ToString(),
              stato = row["STATO"].ToString(),
              attivo = "Attiva"
            };
            delegheOcm.delegheatt.Add(delegheAttive);
          }
        }
        if (dataTable2.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable2.Rows)
          {
            DelegheOCM.DelegheCancellate delegheCancellate = new DelegheOCM.DelegheCancellate()
            {
              datini = row["DATINI"].ToString(),
              datfin = row["DATFIN"].ToString().Substring(0, 10),
              codter = row["CODTER"].ToString(),
              asster = row["ASSTER"].ToString(),
              codnaz = row["CODNAZ"].ToString(),
              assnaz = row["ASSNAZ"].ToString(),
              codpos = row["CODPOS"].ToString(),
              ragsoc = row["RAGSOC"].ToString(),
              stato = row["STATO"].ToString(),
              cancellato = "Cancellata"
            };
            delegheOcm.deleghecanc.Add(delegheCancellate);
          }
        }
        if (dataTable3.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable3.Rows)
          {
            DelegheOCM.DelegheNonAttive delegheNonAttive = new DelegheOCM.DelegheNonAttive()
            {
              datini = row["DATINI"].ToString(),
              datfin = row["DATFIN"].ToString().Substring(0, 10),
              codter = row["CODTER"].ToString(),
              asster = row["ASSTER"].ToString(),
              codnaz = row["CODNAZ"].ToString(),
              assnaz = row["ASSNAZ"].ToString(),
              codpos = row["CODPOS"].ToString(),
              ragsoc = row["RAGSOC"].ToString(),
              stato = row["STATO"].ToString(),
              daAttivare = "Non Confermate"
            };
            delegheOcm.deleghenonatt.Add(delegheNonAttive);
          }
        }
        if (dataTable3.Rows.Count < 1)
        {
          if (dataTable2.Rows.Count < 1)
          {
            if (dataTable1.Rows.Count < 1)
              ErroreMSG = "Nessun risultato trovato ";
          }
        }
      }
      catch (Exception ex)
      {
        ErroreMSG = "Nessun risultato trovato ";
        return (DelegheOCM) null;
      }
      return delegheOcm;
    }

    public DelegheOCM caricaDenominazione(string codNaz, ref string MSGErrore)
    {
      string strSQL = "SELECT CODTER, RAGSOC FROM ASSTER WHERE CODNAZ = " + codNaz + " ORDER BY RAGSOC";
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL);
      DelegheOCM delegheOcm = new DelegheOCM();
      List<DelegheOCM.ListDen> listDenList = new List<DelegheOCM.ListDen>();
      foreach (DataRow row in (InternalDataCollectionBase) dataTable2.Rows)
      {
        DelegheOCM.ListDen listDen = new DelegheOCM.ListDen()
        {
          RAGSOCASS = row["RAGSOC"].ToString(),
          CODTER = row["CODTER"].ToString()
        };
        listDenList.Add(listDen);
      }
      delegheOcm.ListDenominazione = listDenList;
      return delegheOcm;
    }

    public DelegheOCM AssNaz()
    {
      iDB2DataReader dataReaderFromQuery1 = this.objDataAccess.GetDataReaderFromQuery("SELECT DENCOM FROM CODCOM", CommandType.Text);
      List<string> stringList1 = new List<string>();
      while (dataReaderFromQuery1.Read())
        stringList1.Add(dataReaderFromQuery1["DENCOM"].ToString().Trim());
      HttpContext.Current.Items[(object) "ListaComuni"] = (object) stringList1.ToArray();
      iDB2DataReader dataReaderFromQuery2 = this.objDataAccess.GetDataReaderFromQuery("SELECT DENCOM FROM COM_ESTERO", CommandType.Text);
      List<string> stringList2 = new List<string>();
      while (dataReaderFromQuery2.Read())
        stringList2.Add(dataReaderFromQuery2["DENCOM"].ToString().Trim());
      HttpContext.Current.Items[(object) "ListaStati"] = (object) stringList2.ToArray();
      string strSQL1 = "SELECT * FROM ASSNAZ";
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL1);
      DelegheOCM delegheOcm = new DelegheOCM();
      List<DelegheOCM.AssNaz> assNazList = new List<DelegheOCM.AssNaz>();
      foreach (DataRow row in (InternalDataCollectionBase) dataTable2.Rows)
      {
        DelegheOCM.AssNaz assNaz = new DelegheOCM.AssNaz()
        {
          CODNAZ = row["CODNAZ"].ToString(),
          RAGSOCASS = row["RAGSOC"].ToString(),
          RAGSOCBRASS = row["RAGSOCBRE"].ToString()
        };
        assNazList.Add(assNaz);
      }
      delegheOcm.AssociazioneNaz = assNazList;
      string strSQL2 = "SELECT CODDUG, DENDUG FROM DUG ORDER BY DENDUG";
      DataTable dataTable3 = new DataTable();
      DataTable dataTable4 = this.objDataAccess.GetDataTable(strSQL2);
      List<DelegheOCM.ListInd> listIndList = new List<DelegheOCM.ListInd>();
      foreach (DataRow row in (InternalDataCollectionBase) dataTable4.Rows)
      {
        DelegheOCM.ListInd listInd = new DelegheOCM.ListInd()
        {
          DENDUG = row["DENDUG"].ToString(),
          CODDUG = row["CODDUG"].ToString()
        };
        listIndList.Add(listInd);
      }
      delegheOcm.ListaIndirizzi = listIndList;
      return delegheOcm;
    }

    public DelegheOCM carica_Dati_Associazione(
      DelegheOCM docm,
      string codTer,
      string codNaz,
      string codpos,
      string assnaz,
      string asster,
      string ragsoc)
    {
      DataTable dataTable1 = new DataTable();
      try
      {
        string strSQL1 = " SELECT VALUE(CHAR(ASSTER.CODUTE),'') AS CODUTE, UTEPIN.PIN FROM ASSTER INNER JOIN UTEPIN ON" + " ASSTER.CODUTE = UTEPIN.CODUTE INNER JOIN DELEGHE ON" + " DELEGHE.CODTER = ASSTER.CODTER" + " WHERE ASSTER.CODTER = " + codTer + " AND DELEGHE.CODPOS = " + codpos + " AND UTEPIN.STAPIN = 'A'" + " AND UTEPIN.DATINI = (SELECT MAX(DATINI) FROM UTEPIN WHERE CODUTE = ASSTER.CODUTE)" + " AND CURRENT_DATE BETWEEN DELEGHE.DATINI AND DELEGHE.DATFIN";
        dataTable1.Clear();
        DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL1);
        if (dataTable2.Rows.Count > 0)
        {
          this.Deleghe.datidelega.CodUteDe = dataTable2.Rows[0]["CODUTE"].ToString().Trim();
          this.Deleghe.datidelega.Pin = Cypher.DeCryptPassword(dataTable2.Rows[0]["PIN"].ToString().Trim());
        }
        else
        {
          this.Deleghe.datidelega.CodUteDe = "";
          this.Deleghe.datidelega.Pin = "";
        }
        string strSQL2 = "SELECT ASSTER.RAGSOC, ASSTER.CODCOM, ASSTER.RAGSOCBRE, ASSTER.EMAILCERT, ASSTER.CELL,  ASSTER.PARIVA, DUG.DENDUG, DUG.CODDUG, ASSTER.IND, ASSTER.NUMCIV, " + " ASSTER.CAP, ASSTER.DENLOC, ASSTER.SIGPRO,  ASSTER.TEL1,  ASSTER.TEL2, ASSTER.CODFIS, " + " ASSTER.FAX,  ASSTER.EMAIL " + " FROM  ASSTER LEFT JOIN DUG ON  ASSTER.CODDUG =  DUG.CODDUG " + " WHERE ASSTER.CODTER = " + codTer + " AND ASSTER.CODNAZ = " + codNaz;
        dataTable2.Clear();
        DataTable dataTable3 = this.objDataAccess.GetDataTable(strSQL2);
        if (dataTable3 != null && dataTable3.Rows.Count > 0)
        {
          this.Deleghe.datidelega.AssStudio = assnaz;
          this.Deleghe.datidelega.Denominazione = asster;
          this.Deleghe.datidelega.ParIvaDe = dataTable3.Rows[0]["PARIVA"].ToString();
          this.Deleghe.datidelega.RagSocDe = dataTable3.Rows[0]["RAGSOC"].ToString().Trim();
          this.Deleghe.datidelega.CodFisDe = dataTable3.Rows[0]["CODFIS"].ToString().Trim();
          this.Deleghe.datidelega.ViaDe = dataTable3.Rows[0]["DENDUG"].ToString().Trim();
          this.Deleghe.datidelega.IndDe = dataTable3.Rows[0]["IND"].ToString().Trim();
          this.Deleghe.datidelega.CapDe = dataTable3.Rows[0]["CAP"].ToString().Trim();
          this.Deleghe.datidelega.NumCivDe = dataTable3.Rows[0]["NUMCIV"].ToString().Trim();
          this.Deleghe.datidelega.CODDUG = dataTable3.Rows[0]["CODDUG"].ToString().Trim();
          this.Deleghe.datidelega.EmailDe = dataTable3.Rows[0]["EMAIL"].ToString().Trim();
          this.Deleghe.datidelega.LocalitaDe = dataTable3.Rows[0]["DENLOC"].ToString().Trim();
          this.Deleghe.datidelega.PrDe = dataTable3.Rows[0]["SIGPRO"].ToString().Trim();
          this.Deleghe.datidelega.TelDe = dataTable3.Rows[0]["TEL1"].ToString();
          this.Deleghe.datidelega.EmailCertDe = dataTable3.Rows[0]["EMAILCERT"].ToString().Trim();
          this.Deleghe.datidelega.CelDe = dataTable3.Rows[0]["CELL"].ToString().Trim();
          this.Deleghe.datidelega.ComuneDe = string.IsNullOrEmpty(dataTable3.Rows[0]["CODCOM"].ToString().Trim()) ? "" : this.objDataAccess.Get1ValueFromSQL("SELECT VALUE(DENCOM,'') AS DENCOM FROM CODCOM WHERE CODCOM = " + DBMethods.DoublePeakForSql(dataTable3.Rows[0]["CODCOM"].ToString().Trim()), CommandType.Text).ToString().Trim();
        }
        this.Deleghe.datidelega.RagSocDe = ragsoc;
        this.Deleghe.datidelega.codpos = codpos;
        docm.datidelega = this.Deleghe.datidelega;
      }
      catch (Exception ex)
      {
        return (DelegheOCM) null;
      }
      return docm;
    }

    public DelegheOCM CARICA_IVA(ref string MSGErrore, string codfis, string pariva)
    {
      DelegheOCM delegheOcm = new DelegheOCM();
      DataTable dataTable1 = new DataTable();
      try
      {
        string strSQL = " SELECT * FROM ASSTER";
        if (string.IsNullOrEmpty(pariva.Trim()) & !string.IsNullOrEmpty(codfis))
          strSQL = strSQL + " WHERE CODFIS = " + DBMethods.DoublePeakForSql(codfis);
        if (!string.IsNullOrEmpty(pariva.Trim()) & string.IsNullOrEmpty(codfis))
          strSQL = strSQL + " WHERE PARIVA = " + DBMethods.DoublePeakForSql(pariva);
        if (!string.IsNullOrEmpty(pariva.Trim()) & !string.IsNullOrEmpty(codfis))
          strSQL = strSQL + " WHERE PARIVA = " + DBMethods.DoublePeakForSql(pariva);
        dataTable1.Clear();
        DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL);
        if (dataTable2 != null)
        {
          if (dataTable2.Rows.Count > 0)
          {
            if (dataTable2.Rows.Count == 1)
            {
              this.Deleghe.datidelega.EmailCertDe = dataTable2.Rows[0]["EMAILCERT"].ToString();
              this.Deleghe.datidelega.CelDe = dataTable2.Rows[0]["CELL"].ToString();
              this.Deleghe.datidelega.RagSocBrDe = dataTable2.Rows[0]["RAGSOCBRE"].ToString();
              this.Deleghe.datidelega.codNaz = dataTable2.Rows[0]["CODNAZ"].ToString();
              this.Deleghe.datidelega.codTer = dataTable2.Rows[0]["CODTER"].ToString();
              this.Deleghe.datidelega.AssStudio = dataTable2.Rows[0]["RAGSOC"].ToString();
              this.Deleghe.datidelega.CodFisDe = dataTable2.Rows[0]["CODFIS"].ToString();
              this.Deleghe.datidelega.IndDe = dataTable2.Rows[0]["IND"].ToString();
              this.Deleghe.datidelega.CapDe = dataTable2.Rows[0]["CAP"].ToString();
              this.Deleghe.datidelega.NumCivDe = dataTable2.Rows[0]["NUMCIV"].ToString();
              this.Deleghe.datidelega.CODDUG = dataTable2.Rows[0]["CODDUG"].ToString();
              this.Deleghe.datidelega.EmailDe = dataTable2.Rows[0]["EMAIL"].ToString();
              this.Deleghe.datidelega.FaxDe = dataTable2.Rows[0]["FAX"].ToString();
              this.Deleghe.datidelega.LocalitaDe = dataTable2.Rows[0]["DENLOC"].ToString();
              this.Deleghe.datidelega.PrDe = dataTable2.Rows[0]["SIGPRO"].ToString();
              this.Deleghe.datidelega.TelDe = dataTable2.Rows[0]["TEL1"].ToString();
              this.Deleghe.datidelega.TelDe = dataTable2.Rows[0]["TEL2"].ToString();
              this.Deleghe.datidelega.ParIvaDe = dataTable2.Rows[0]["PARIVA"].ToString();
              if (!string.IsNullOrEmpty(dataTable2.Rows[0]["CODCOM"].ToString().Trim()))
              {
                this.Deleghe.datidelega.ComuneDe = this.objDataAccess.Get1ValueFromSQL("SELECT VALUE(DENCOM,'') AS DENCOM FROM CODCOM WHERE CODCOM = " + DBMethods.DoublePeakForSql(dataTable2.Rows[0]["CODCOM"].ToString().Trim()), CommandType.Text).ToString().Trim();
                this.Deleghe.datidelega.ComuneDe = dataTable2.Rows[0]["CODCOM"].ToString().Trim();
              }
              else
                this.Deleghe.datidelega.ComuneDe = "";
            }
            else if (string.IsNullOrEmpty(pariva) && string.IsNullOrEmpty(codfis.Trim()))
            {
              this.Deleghe.datidelega.EmailCertDe = dataTable2.Rows[0]["EMAILCERT"].ToString();
              this.Deleghe.datidelega.CelDe = dataTable2.Rows[0]["CELL"].ToString();
              this.Deleghe.datidelega.RagSocBrDe = dataTable2.Rows[0]["RAGSOCBRE"].ToString();
              this.Deleghe.datidelega.codNaz = dataTable2.Rows[0]["CODNAZ"].ToString();
              this.Deleghe.datidelega.codTer = dataTable2.Rows[0]["CODTER"].ToString();
              this.Deleghe.datidelega.RagSocDe = dataTable2.Rows[0]["RAGSOC"].ToString();
              this.Deleghe.datidelega.CodFisDe = dataTable2.Rows[0]["CODFIS"].ToString();
              this.Deleghe.datidelega.IndDe = dataTable2.Rows[0]["IND"].ToString();
              this.Deleghe.datidelega.CapDe = dataTable2.Rows[0]["CAP"].ToString();
              this.Deleghe.datidelega.NumCivDe = dataTable2.Rows[0]["NUMCIV"].ToString();
              this.Deleghe.datidelega.CODDUG = dataTable2.Rows[0]["CODDUG"].ToString();
              this.Deleghe.datidelega.EmailDe = dataTable2.Rows[0]["EMAIL"].ToString();
              this.Deleghe.datidelega.FaxDe = dataTable2.Rows[0]["FAX"].ToString();
              this.Deleghe.datidelega.LocalitaDe = dataTable2.Rows[0]["DENLOC"].ToString();
              this.Deleghe.datidelega.PrDe = dataTable2.Rows[0]["SIGPRO"].ToString();
              this.Deleghe.datidelega.TelDe = dataTable2.Rows[0]["TEL1"].ToString();
              this.Deleghe.datidelega.TelDe = dataTable2.Rows[0]["TEL2"].ToString();
              this.Deleghe.datidelega.ParIvaDe = dataTable2.Rows[0]["PARIVA"].ToString();
              if (!string.IsNullOrEmpty(dataTable2.Rows[0]["CODCOM"].ToString()))
              {
                this.Deleghe.datidelega.ComuneDe = dataTable2.Rows[0]["DENCOM"].ToString();
                this.Deleghe.datidelega.ComuneDe = dataTable2.Rows[0]["CODCOM"].ToString();
              }
              else
                this.Deleghe.datidelega.ComuneDe = "";
            }
          }
          if (string.IsNullOrEmpty(this.Deleghe.datidelega.codTer))
            MSGErrore = "Nella base dati non sono presenti associazioni/studi con questa partita I.V.A.";
        }
        else if (string.IsNullOrEmpty(this.Deleghe.datidelega.codNaz))
          MSGErrore = "Nella base dati non sono presenti associazioni/studi con questa partita I.V.A.";
      }
      catch (Exception ex)
      {
        MSGErrore = "Errore nel caricamento dei dati. Riprovare" + ex.Message;
        return (DelegheOCM) null;
      }
      delegheOcm.datidelega = this.Deleghe.datidelega;
      return delegheOcm;
    }
  }
}
