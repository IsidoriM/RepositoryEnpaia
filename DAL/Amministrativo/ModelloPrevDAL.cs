// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Amministrativo.ModelloPrevDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using System;
using System.Data;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.Amministrativo;

namespace TFI.DAL.Amministrativo
{
  public class ModelloPrevDAL
  {
    public ModelloPrevOCM CaricaDatiModelloPrev(
      ModelloPrevOCM.DatiModello datiModello,
      ref string ErrorMSG,
      ref string SuccessMSG)
    {
      string str1 = "";
      DataLayer dataLayer = new DataLayer();
      DataTable dataTable1 = new DataTable();
      ModelloPrevOCM modelloPrevOcm = new ModelloPrevOCM();
      try
      {
        dataLayer.StartTransaction();
        if (!string.IsNullOrEmpty(datiModello.CodPos) && !this.Module_Autorizzazioni_Speciali(datiModello.CodPos.Trim(), "CONTRIBUTI", ref ErrorMSG))
        {
          ErrorMSG = "Utente non abilitato";
          return (ModelloPrevOCM) null;
        }
        string str2 = "SELECT MODPRE.CODPOS,MODPRE.FLGIRR, MODPRE.MAT, MODPRE.PRORAP, MODPRE.PROMOD, CODSTAPRE, DATAPE," + " (SELECT DENTIPUTE FROM TIPUTE WHERE CODTIPUTE = MODPRE.UTEAPE) AS UTEAPE, DATCHI," + " (SELECT DENTIPUTE FROM TIPUTE WHERE CODTIPUTE = MODPRE.UTECHI) AS UTECHI, PREVUFF, PREAVVISO," + " DATINIPRE, DATSCAPRE, GIOINDSOS, IMPINDSOS, INDDENDIP," + " RIFNOME, RIFTEL, NOTE, DATANN, UTEASS, COGRAP, NOMRAP, CODFISRAP," + " CODCOMNASRAP, SIGPRONASRAP, DATNASRAP, DENLOCRAP, DENSTAESTRAP," + " SIGPRORAP, CODDUGRAP, INDRAP, NUMCIVRAP," + " CAPRAP, TELRAP, MODPRE.ULTAGG, MODPRE.UTEAGG, TIPPRE, COMPLETO," + " UTEANN, CODFUNRAP, PRESTITO, CARTAENP, DENBAN, DENAGE," + " COONUM, COOCIN, COOABI, COOCAB, COOIBAN, CODCOMRAPRES, DENCOMRAPRES," + " COOAGENZIA, COOBANCA, PROT, NUMPROT, DATPROT, ALLEGATO," + " ISCT.COG, ISCT.NOM, ISCT.CODFIS, " + " (SELECT DATPAG FROM RAPLAV WHERE CODPOS = MODPRE.CODPOS AND MAT = MODPRE.MAT AND PRORAP = MODPRE.PRORAP) AS DATPAG, " + " (SELECT DENSTAPRE FROM CODSTAPRE WHERE CODSTAPRE.CODSTAPRE = MODPRE.CODSTAPRE) AS STATO, " + " (SELECT RAGSOC FROM AZI WHERE AZI.CODPOS = MODPRE.CODPOS) AS RAGSOC,  " + " (SELECT DATDEC FROM RAPLAV WHERE CODPOS = MODPRE.CODPOS AND PRORAP = MODPRE.PRORAP AND MAT = MODPRE.MAT) AS DATDEC,  " + " (SELECT DATCES FROM RAPLAV WHERE CODPOS = MODPRE.CODPOS AND PRORAP = MODPRE.PRORAP AND MAT = MODPRE.MAT) AS DATCES  " + " FROM MODPRE LEFT JOIN ISCT ON MODPRE.MAT = ISCT.MAT " + " WHERE DATANN IS NULL ";
        if (!string.IsNullOrEmpty(datiModello.CodPos))
          str1 = str1 + " AND MODPRE.CODPOS = " + datiModello.CodPos.Trim();
        if (!string.IsNullOrEmpty(datiModello.Matricola))
          str1 = str1 + " AND MODPRE.MAT = " + datiModello.Matricola.Trim();
        if (!string.IsNullOrEmpty(datiModello.RagSoc))
          str1 = str1 + " AND MODPRE.CODPOS IN ( SELECT CODPOS FROM AZI WHERE UPPER(TRIM(RAGSOC)) LIKE " + DBMethods.DoublePeakForSql(datiModello.RagSoc.Trim().ToUpper()) + ")";
        if (!string.IsNullOrEmpty(datiModello.Cognome))
          str1 = str1 + " AND UPPER(TRIM(ISCT.COG)) LIKE " + DBMethods.DoublePeakForSql(datiModello.Cognome.Trim().ToUpper());
        if (!string.IsNullOrEmpty(datiModello.Stato))
          str1 = str1 + " AND MODPRE.CODSTAPRE = '" + datiModello.Stato.Trim() + "'";
        if (!string.IsNullOrEmpty(datiModello.Utenti))
          str1 = str1 + " AND MODPRE.UTEASS = '" + DBMethods.DoublePeakForSql(datiModello.Utenti.Trim()) + "'";
        if (!string.IsNullOrEmpty(datiModello.UteAss))
        {
          string uteAss = datiModello.UteAss;
          if (!(uteAss == "UTENTE ASSEGNATO"))
          {
            if (uteAss == "UTENTE NON ASSEGNATO")
              str1 += " AND MODPRE.UTEASS IS NULL";
          }
          else
            str1 += " AND MODPRE.UTEASS IS NOT NULL";
        }
        if (!string.IsNullOrEmpty(datiModello.UteApePrev))
        {
          string uteApePrev = datiModello.UteApePrev;
          if (!(uteApePrev == "AZIENDA"))
          {
            if (!(uteApePrev == "UTENTE ENPAIA"))
            {
              if (uteApePrev == "CONSULENTE")
                str1 += " AND MODPRE.UTEAPE = 'C'";
            }
            else
              str1 += " AND MODPRE.UTEAPE = 'E'";
          }
          else
            str1 += " AND MODPRE.UTEAPE = 'A'";
        }
        if (!string.IsNullOrEmpty(datiModello.CesDal))
          str1 = str1 + " AND MODPRE.CODPOS || MODPRE.MAT || MODPRE.PRORAP IN (SELECT CODPOS || MAT || PRORAP FROM RAPLAV WHERE DATCES >= '" + DBMethods.Db2Date(datiModello.CesDal.Substring(0, 10)) + "')";
        if (!string.IsNullOrEmpty(datiModello.CesAl))
          str1 = str1 + " AND MODPRE.CODPOS || MODPRE.MAT || MODPRE.PRORAP IN (SELECT CODPOS || MAT || PRORAP FROM RAPLAV WHERE DATCES <= '" + DBMethods.Db2Date(datiModello.CesAl.Substring(0, 10)) + "')";
        if (!string.IsNullOrEmpty(datiModello.DataApePrev))
          str1 = str1 + " AND DATE(DATAPE) = '" + DBMethods.Db2Date(datiModello.DataApePrev.Substring(0, 10)) + "'";
        if (!string.IsNullOrEmpty(datiModello.DatCesRDL))
          str1 = str1 + " AND MODPRE.CODPOS || MODPRE.MAT || MODPRE.PRORAP IN (SELECT CODPOS || MAT || PRORAP FROM RAPLAV WHERE DATCES = '" + DBMethods.Db2Date(datiModello.DatCesRDL.Substring(0, 10)) + "')";
        if (!string.IsNullOrEmpty(datiModello.AnnCesRDL))
          str1 = str1 + " AND MODPRE.CODPOS || MODPRE.MAT || MODPRE.PRORAP IN (SELECT CODPOS || MAT || PRORAP FROM RAPLAV WHERE YEAR(DATCES) = '" + datiModello.AnnCesRDL.Substring(0, 4) + "')";
        string atti = datiModello.Atti;
        if (!(atti == "S"))
        {
          if (atti == "N")
            str1 += " AND MODPRE.FLGIRR IS NULL";
        }
        else
          str1 = str1 + " AND MODPRE.FLGIRR = '" + datiModello.Atti + "'";
        if (!string.IsNullOrEmpty(datiModello.UteChiPrev))
        {
          string uteChiPrev = datiModello.UteChiPrev;
          if (!(uteChiPrev == "AZIENDA"))
          {
            if (!(uteChiPrev == "UTENTE ENPAIA"))
            {
              if (uteChiPrev == "CONSULENTE")
                str1 += " AND MODPRE.UTECHI = 'C'";
            }
            else
              str1 += " AND MODPRE.UTECHI = 'E'";
          }
          else
            str1 += " AND MODPRE.UTECHI = 'A'";
        }
        if (!string.IsNullOrEmpty(datiModello.DataChiPrev))
          str1 = str1 + " AND DATE(DATCHI) = '" + DBMethods.Db2Date(datiModello.DataChiPrev.Substring(0, 10)) + "'";
        if (!string.IsNullOrEmpty(datiModello.ValidiAnn))
        {
          string validiAnn = datiModello.ValidiAnn;
          if (!(validiAnn == "ANNULLATI"))
          {
            if (validiAnn == "VALIDI")
              str1 += " AND MODPRE.DATANN IS NULL AND UTEANN IS NULL";
          }
          else
            str1 += " AND MODPRE.DATANN IS NOT NULL AND UTEANN IS NOT NULL";
        }
        if (!string.IsNullOrEmpty(datiModello.Tipo))
        {
          string tipo = datiModello.Tipo;
          if (!(tipo == "CARTACEO"))
          {
            if (tipo == "TELEMATICO")
              str1 += " AND MODPRE.TIPPRE='T'";
          }
          else
            str1 += " AND MODPRE.TIPPRE='C'";
        }
        if (!string.IsNullOrEmpty(datiModello.Compilazione))
        {
          string compilazione = datiModello.Compilazione;
          if (!(compilazione == "COMPLETO"))
          {
            if (compilazione == "INCOMPLETO")
              str1 += " AND MODPRE.COMPLETO='N'";
          }
          else
            str1 += " AND MODPRE.COMPLETO='S'";
        }
        string strSQL = str2 + str1 + " ORDER BY RAGSOC";
        dataTable1.Clear();
        DataTable dataTable2 = dataLayer.GetDataTable(strSQL);
        if (dataTable2.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable2.Rows)
          {
            ModelloPrevOCM.DatiModello datiModello1 = new ModelloPrevOCM.DatiModello()
            {
              CodPos = row["CODPOS"].ToString(),
              Matricola = row["MAT"].ToString(),
              RagSoc = row["RAGSOC"].ToString(),
              Nome = row["NOM"].ToString(),
              Cognome = row["COG"].ToString(),
              Stato = row["STATO"].ToString(),
              ValidiAnn = row["DATANN"].ToString(),
              Tipo = row["TIPPRE"].ToString(),
              Compilazione = row["COMPLETO"].ToString(),
              Utenti = row["UTEASS"].ToString(),
              UteAss = row["UTEASS"].ToString(),
              DataApePrev = DBMethods.Db2Date(row["DATAPE"].ToString()),
              DataChiPrev = DBMethods.Db2Date(row["DATCHI"].ToString()),
              UteApePrev = row["UTEAPE"].ToString(),
              UteChiPrev = row["UTECHI"].ToString(),
              DatCesRDL = row["DATCES"].ToString().Substring(0, 10),
              AnnCesRDL = row["DATCES"].ToString().Substring(6, 4),
              Atti = row["FLGIRR"].ToString(),
              CesDal = row["DATCES"].ToString().Substring(0, 10),
              CesAl = row["DATCES"].ToString().Substring(0, 10)
            };
            modelloPrevOcm.datiList.Add(datiModello1);
          }
          SuccessMSG = string.Format("La ricerca ha prodotto {0} risultati", (object) modelloPrevOcm.datiList.Count);
          dataLayer.EndTransaction(true);
        }
        else
        {
          ErrorMSG = "La ricerca non ha prodotto risultati";
          dataLayer.EndTransaction(true);
        }
        return modelloPrevOcm;
      }
      catch (Exception ex)
      {
        dataLayer.EndTransaction(false);
        ErrorMSG = "Ricerca fallita";
        return (ModelloPrevOCM) null;
      }
    }

    public bool Module_Autorizzazioni_Speciali(string CODPOS, string SISTEMA, ref string ErrorMSG)
    {
      DataTable dataTable = new Utile().CREA_DT_AUTORIZZAZIONI_ENPAIA(ref ErrorMSG);
      if (!(SISTEMA == "CONTRIBUTI"))
      {
        if (SISTEMA == "AGRIFONDO")
        {
          for (int index = 0; index <= dataTable.Rows.Count - 1; ++index)
          {
            if (dataTable.Rows[index]["POSIZIONE_AGF"].ToString() == CODPOS)
              return dataTable.Rows[index]["ABILITATO_AGF"].ToString() == "SI";
          }
        }
      }
      else
      {
        for (int index = 0; index <= dataTable.Rows.Count - 1; ++index)
        {
          if (dataTable.Rows[index]["POSIZIONE"].ToString() == CODPOS)
            return dataTable.Rows[index]["ABILITATO"].ToString() == "SI";
        }
      }
      return true;
    }
  }
}
