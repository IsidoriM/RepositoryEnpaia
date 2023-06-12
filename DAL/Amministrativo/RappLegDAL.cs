// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Amministrativo.RappLegDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.Amministrativo;

namespace TFI.DAL.Amministrativo
{
  public class RappLegDAL
  {
    private DataLayer objDataAccess = new DataLayer();

    public Rappresentante_legaleOCM RicRappLeg(
      string posizione,
      string ragioneSociale,
      string partitaiva,
      string codiceFiscale,
      string cognome,
      string nome,
      string codfis,
      ref string ErroreMSG,
      ref string SuccessMSG)
    {
      string str = "";
      string Err = "";
      Rappresentante_legaleOCM rappresentanteLegaleOcm = new Rappresentante_legaleOCM();
      if (!string.IsNullOrEmpty(posizione))
      {
        rappresentanteLegaleOcm.ricercaRapp.CodPos = posizione;
        str = str + " AND A.CODPOS = '" + posizione + "' ";
      }
      if (!string.IsNullOrEmpty(ragioneSociale))
      {
        rappresentanteLegaleOcm.ricercaRapp.RagSoc = ragioneSociale;
        str = str + " AND B.RAGSOC = '" + ragioneSociale.ToUpper() + "' ";
      }
      if (!string.IsNullOrEmpty(partitaiva))
      {
        rappresentanteLegaleOcm.ricercaRapp.PartIVAaz = partitaiva;
        str = str + " AND B.PARIVA = '" + partitaiva + "' ";
      }
      if (!string.IsNullOrEmpty(codiceFiscale))
      {
        rappresentanteLegaleOcm.ricercaRapp.CodFisAz = codiceFiscale;
        str = str + " AND B.CODFIS = '" + codiceFiscale.ToUpper() + "' ";
      }
      if (!string.IsNullOrEmpty(cognome))
      {
        rappresentanteLegaleOcm.ricercaRapp.cog = cognome;
        str = str + " AND A.COG = '" + cognome.ToUpper() + "' ";
      }
      if (!string.IsNullOrEmpty(nome))
      {
        rappresentanteLegaleOcm.ricercaRapp.nom = nome;
        str = str + " AND A.NOM = '" + nome.ToUpper() + "' ";
      }
      if (!string.IsNullOrEmpty(codfis))
      {
        rappresentanteLegaleOcm.ricercaRapp.codfis = codfis;
        str = str + " AND A.CODFIS = '" + codfis.ToUpper() + "' ";
      }
      string strSQL = "SELECT A.CODPOS,PROREC,A.EMAILCERT,A.DATINI,A.DATFIN,A.CODFUNRAP,A.RAPPRI,B.RAGSOC,B.PARIVA AS PARIVAAZI,B.CODFIS AS CODFISAZI, " + " (SELECT DENDUG FROM DUG WHERE CODDUG = A.CODDUG) AS DENDUG,(SELECT DENCOM FROM CODCOM WHERE CODCOM = A.CODCOMNAS) AS DENCOMNAS, " + " (SELECT DENCOM FROM CODCOM WHERE CODCOM = A.CODCOMRES) AS DENCOM,(SELECT DENMEZ FROM CODMEZ WHERE CODMEZ = A.CODMEZ) AS DENMEZ, " + " (SELECT DENFUNRAP FROM FUNRAP WHERE CODFUNRAP = A.CODFUNRAP) AS DENFUNRAP,COG,NOM,CODDUG,IND,NUMCIV,DENSTAEST,DENLOC,CAP,SIGPRO, " + " TEL1,TEL2,FAX,CELL,EMAIL,A.CODFIS,DATNAS,CODCOMNAS,SES,A.CODMEZ,DATCOM,A.ULTAGG,A.UTEAGG, " + " (SELECT CODOPE FROM TKRISORSE WHERE DATANN IS NULL AND CODFIS IN (SELECT CODFIS FROM UTENTI WHERE CODUTE = A.UTECONF)) AS UTECONF, " + " CODCOMRES,DATCONF " + " FROM RAPLEGWEB A, AZI B " + " WHERE A.CODPOS=B.CODPOS" + str;
      try
      {
        DataSet dataSet1 = new DataSet();
        DataSet dataSet2 = this.objDataAccess.GetDataSet(strSQL, ref Err);
        List<Rappresentante_legaleOCM.RappLegale> rappLegaleList = new List<Rappresentante_legaleOCM.RappLegale>();
        if (dataSet2.Tables[0].Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataSet2.Tables[0].Rows)
          {
            Rappresentante_legaleOCM.RappLegale rappLegale = new Rappresentante_legaleOCM.RappLegale()
            {
              codpos = row["CODPOS"].ToString(),
              prorec = row["PROREC"].ToString(),
              emailcert = row["EMAILCERT"].ToString(),
              datini = row["DATINI"].ToString(),
              datfin = row["DATFIN"].ToString(),
              codfunrap = row["CODFUNRAP"].ToString(),
              rappri = row["RAPPRI"].ToString(),
              ragsoc = row["RAGSOC"].ToString(),
              parivaAZ = row["PARIVAAZI"].ToString(),
              codfisAz = row["CODFISAZI"].ToString(),
              dendug = row["DENDUG"].ToString(),
              dencomnas = row["DENCOMNAS"].ToString(),
              dencom = row["DENCOM"].ToString(),
              denmez = row["DENMEZ"].ToString(),
              denfunrap = row["DENFUNRAP"].ToString(),
              cog = row["COG"].ToString(),
              nom = row["NOM"].ToString(),
              coddug = row["CODDUG"].ToString(),
              ind = row["IND"].ToString(),
              numciv = row["NUMCIV"].ToString(),
              denstaest = row["DENSTAEST"].ToString(),
              denloc = row["DENLOC"].ToString(),
              cap = row["CAP"].ToString(),
              sigpro = row["SIGPRO"].ToString(),
              tel1 = row["TEL1"].ToString(),
              tel2 = row["TEL2"].ToString(),
              fax = row["FAX"].ToString(),
              cell = row["CELL"].ToString(),
              email = row["EMAIL"].ToString(),
              codfis = row["CODFIS"].ToString(),
              datnas = row["DATNAS"].ToString(),
              codcomnas = row["CODCOMNAS"].ToString(),
              sesso = row["SES"].ToString(),
              codmez = row["CODMEZ"].ToString(),
              datcom = row["DATCOM"].ToString(),
              codcomres = row["CODCOMRES"].ToString(),
              datconf = row["DATCONF"].ToString()
            };
            rappLegaleList.Add(rappLegale);
          }
          rappresentanteLegaleOcm.rapplegale = rappLegaleList;
        }
        else
          ErroreMSG = "Nessun rappresentante trovato";
      }
      catch (Exception ex)
      {
        ErroreMSG = ex.Message;
        return (Rappresentante_legaleOCM) null;
      }
      return rappresentanteLegaleOcm;
    }

    public Rappresentante_legaleOCM DetRap(
      string codpos,
      string datini,
      string rappri,
      string denfunrap,
      string datcom,
      string cog,
      string nom,
      ref string ErroreMSG,
      ref string SuccessMSG)
    {
      Rappresentante_legaleOCM rappresentanteLegaleOcm = new Rappresentante_legaleOCM();
      string Err = "";
      string strSQL = "SELECT A.CODPOS,PROREC,A.EMAILCERT,A.DATINI,A.DATFIN,A.CODFUNRAP,A.RAPPRI,B.RAGSOC,B.PARIVA AS PARIVAAZI,B.CODFIS AS CODFISAZI, " + " (SELECT DENDUG FROM DUG WHERE CODDUG = A.CODDUG) AS DENDUG,(SELECT DENCOM FROM CODCOM WHERE CODCOM = A.CODCOMNAS) AS DENCOMNAS, " + " (SELECT DENCOM FROM CODCOM WHERE CODCOM = A.CODCOMRES) AS DENCOM,(SELECT DENMEZ FROM CODMEZ WHERE CODMEZ = A.CODMEZ) AS DENMEZ, " + " (SELECT DENFUNRAP FROM FUNRAP WHERE CODFUNRAP = A.CODFUNRAP) AS DENFUNRAP,COG,NOM,CODDUG,IND,NUMCIV,DENSTAEST,DENLOC,CAP,SIGPRO, " + " TEL1,TEL2,FAX,CELL,EMAIL,A.CODFIS,DATNAS,CODCOMNAS,SES,A.CODMEZ,DATCOM,A.ULTAGG,A.UTEAGG, " + " (SELECT CODOPE FROM TKRISORSE WHERE DATANN IS NULL AND CODFIS IN (SELECT CODFIS FROM UTENTI WHERE CODUTE = A.UTECONF)) AS UTECONF, " + " CODCOMRES,DATCONF " + " FROM RAPLEGWEB A, AZI B " + " WHERE A.CODPOS=B.CODPOS AND A.CODPOS='" + codpos + "' AND A.RAPPRI='" + rappri + "' " + " AND COG='" + cog + "' AND NOM='" + nom + "' ";
      try
      {
        DataSet dataSet1 = new DataSet();
        DataSet dataSet2 = this.objDataAccess.GetDataSet(strSQL, ref Err);
        rappresentanteLegaleOcm.dettRap.codpos = dataSet2.Tables[0].Rows[0]["CODPOS"].ToString();
        rappresentanteLegaleOcm.dettRap.prorec = dataSet2.Tables[0].Rows[0]["PROREC"].ToString();
        rappresentanteLegaleOcm.dettRap.emailcert = dataSet2.Tables[0].Rows[0]["EMAILCERT"].ToString();
        rappresentanteLegaleOcm.dettRap.datini = dataSet2.Tables[0].Rows[0]["DATINI"].ToString();
        rappresentanteLegaleOcm.dettRap.datfin = dataSet2.Tables[0].Rows[0]["DATFIN"].ToString();
        rappresentanteLegaleOcm.dettRap.codfunrap = dataSet2.Tables[0].Rows[0]["CODFUNRAP"].ToString();
        rappresentanteLegaleOcm.dettRap.rappri = dataSet2.Tables[0].Rows[0]["RAPPRI"].ToString();
        rappresentanteLegaleOcm.dettRap.ragsoc = dataSet2.Tables[0].Rows[0]["RAGSOC"].ToString();
        rappresentanteLegaleOcm.dettRap.parivaAZ = dataSet2.Tables[0].Rows[0]["PARIVAAZI"].ToString();
        rappresentanteLegaleOcm.dettRap.codfisAz = dataSet2.Tables[0].Rows[0]["CODFISAZI"].ToString();
        rappresentanteLegaleOcm.dettRap.dendug = dataSet2.Tables[0].Rows[0]["DENDUG"].ToString();
        rappresentanteLegaleOcm.dettRap.dencomnas = dataSet2.Tables[0].Rows[0]["DENCOMNAS"].ToString();
        rappresentanteLegaleOcm.dettRap.dencom = dataSet2.Tables[0].Rows[0]["DENCOM"].ToString();
        rappresentanteLegaleOcm.dettRap.denmez = dataSet2.Tables[0].Rows[0]["DENMEZ"].ToString();
        rappresentanteLegaleOcm.dettRap.denfunrap = dataSet2.Tables[0].Rows[0]["DENFUNRAP"].ToString();
        rappresentanteLegaleOcm.dettRap.cog = dataSet2.Tables[0].Rows[0]["COG"].ToString();
        rappresentanteLegaleOcm.dettRap.nom = dataSet2.Tables[0].Rows[0]["NOM"].ToString();
        rappresentanteLegaleOcm.dettRap.coddug = dataSet2.Tables[0].Rows[0]["CODDUG"].ToString();
        rappresentanteLegaleOcm.dettRap.ind = dataSet2.Tables[0].Rows[0]["IND"].ToString();
        rappresentanteLegaleOcm.dettRap.numciv = dataSet2.Tables[0].Rows[0]["NUMCIV"].ToString();
        rappresentanteLegaleOcm.dettRap.denstaest = dataSet2.Tables[0].Rows[0]["DENSTAEST"].ToString();
        rappresentanteLegaleOcm.dettRap.denloc = dataSet2.Tables[0].Rows[0]["DENLOC"].ToString();
        rappresentanteLegaleOcm.dettRap.cap = dataSet2.Tables[0].Rows[0]["CAP"].ToString();
        rappresentanteLegaleOcm.dettRap.sigpro = dataSet2.Tables[0].Rows[0]["SIGPRO"].ToString();
        rappresentanteLegaleOcm.dettRap.tel1 = dataSet2.Tables[0].Rows[0]["TEL1"].ToString();
        rappresentanteLegaleOcm.dettRap.tel2 = dataSet2.Tables[0].Rows[0]["TEL2"].ToString();
        rappresentanteLegaleOcm.dettRap.fax = dataSet2.Tables[0].Rows[0]["FAX"].ToString();
        rappresentanteLegaleOcm.dettRap.cell = dataSet2.Tables[0].Rows[0]["CELL"].ToString();
        rappresentanteLegaleOcm.dettRap.email = dataSet2.Tables[0].Rows[0]["EMAIL"].ToString();
        rappresentanteLegaleOcm.dettRap.codfis = dataSet2.Tables[0].Rows[0]["CODFIS"].ToString();
        rappresentanteLegaleOcm.dettRap.datnas = dataSet2.Tables[0].Rows[0]["DATNAS"].ToString();
        rappresentanteLegaleOcm.dettRap.codcomnas = dataSet2.Tables[0].Rows[0]["CODCOMNAS"].ToString();
        rappresentanteLegaleOcm.dettRap.sigpronas = this.objDataAccess.Get1ValueFromSQL("SELECT SIGPRO FROM CODCOM WHERE CODCOM ='" + dataSet2.Tables[0].Rows[0]["CODCOMNAS"].ToString() + "' ", CommandType.Text).ToString();
        rappresentanteLegaleOcm.dettRap.sesso = dataSet2.Tables[0].Rows[0]["SES"].ToString();
        rappresentanteLegaleOcm.dettRap.codmez = dataSet2.Tables[0].Rows[0]["CODMEZ"].ToString();
        rappresentanteLegaleOcm.dettRap.datcom = dataSet2.Tables[0].Rows[0]["DATCOM"].ToString();
        rappresentanteLegaleOcm.dettRap.codcomres = dataSet2.Tables[0].Rows[0]["CODCOMRES"].ToString();
        rappresentanteLegaleOcm.dettRap.datconf = dataSet2.Tables[0].Rows[0]["DATCONF"].ToString();
        DataSet dataSet3 = this.objDataAccess.GetDataSet("SELECT CODDUG ,DENDUG FROM DUG", ref Err);
        rappresentanteLegaleOcm.listNIVia = new List<Rappresentante_legaleOCM.Dendug>();
        if (dataSet3 != null)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataSet3.Tables[0].Rows)
          {
            Rappresentante_legaleOCM.Dendug dendug = new Rappresentante_legaleOCM.Dendug()
            {
              dendug = row["DENDUG"].ToString().Trim(),
              coddug = row["CODDUG"].ToString().Trim()
            };
            rappresentanteLegaleOcm.listNIVia.Add(dendug);
          }
        }
        DataSet dataSet4 = this.objDataAccess.GetDataSet("SELECT DENFUNRAP,CODFUNRAP FROM FUNRAP", ref Err);
        rappresentanteLegaleOcm.listfunrap = new List<Rappresentante_legaleOCM.FunRap>();
        if (dataSet4 != null)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataSet4.Tables[0].Rows)
          {
            Rappresentante_legaleOCM.FunRap funRap = new Rappresentante_legaleOCM.FunRap()
            {
              denfunrap = row["DENFUNRAP"].ToString().Trim(),
              codfunrap = row["CODFUNRAP"].ToString().Trim()
            };
            rappresentanteLegaleOcm.listfunrap.Add(funRap);
          }
        }
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
      }
      catch (Exception ex)
      {
        ErroreMSG = "Errore durante la ricerca";
      }
      return rappresentanteLegaleOcm;
    }

    public Rappresentante_legaleOCM InsRap(
      Rappresentante_legaleOCM rap,
      ref string ErroreMSG,
      ref string SuccessMSG,
      TFI.OCM.Utente.Utente u)
    {
      this.objDataAccess.StartTransaction();
      bool blnCommit1 = false;
      bool blnCommit2 = false;
      string str = this.objDataAccess.Get1ValueFromSQL("SELECT VALUE(MAX(PROREC), 0) + 1 AS PROREC FROM AZIRAP WHERE CODPOS = '" + rap.dettRap.codpos + "'", CommandType.Text);
      string strSQL1 = "INSERT INTO AZIRAP(CODPOS,PROREC,DATCOM,CODMEZ,DATINI,DATFIN,CODFUNRAP,RAPPRI,COG,NOM,CODDUG,IND,NUMCIV,DENSTAEST, " + "DENLOC,CAP,SIGPRO,TEL1,TEL2,FAX,EMAIL,CODFIS,DATNAS,CODCOMNAS,SES,CELL,ULTAGG,UTEAGG,EMAILCERT,CODCOMRES) " + " VALUES('" + rap.dettRap.codpos + "','" + str + "','" + DBMethods.Db2Date(rap.dettRap.datcom) + "','" + rap.dettRap.codmez + "','" + DBMethods.Db2Date(rap.dettRap.datini) + "', " + "'" + DBMethods.Db2Date(rap.dettRap.datfin) + "','" + rap.dettRap.codfunrap + "','" + rap.dettRap.rappri + "','" + rap.dettRap.cog + "','" + rap.dettRap.nom + "','" + rap.dettRap.coddug + "', " + "'" + rap.dettRap.ind.Replace("'", "") + "','" + rap.dettRap.numciv + "','" + rap.dettRap.denstaest + "','" + rap.dettRap.denloc + "','" + rap.dettRap.cap + "','" + rap.dettRap.sigpro + "', " + "'" + rap.dettRap.tel1 + "','" + rap.dettRap.tel2 + "','" + rap.dettRap.fax + "','" + rap.dettRap.email + "','" + rap.dettRap.codfis + "','" + DBMethods.Db2Date(rap.dettRap.datnas) + "', " + "'" + rap.dettRap.codcomnas + "','" + rap.dettRap.sesso + "','" + rap.dettRap.cell + "',CURRENT_TIMESTAMP,'" + u.Username + "','" + rap.dettRap.emailcert + "','" + rap.dettRap.codcomres + "') ";
      try
      {
        blnCommit1 = this.objDataAccess.WriteTransactionData(strSQL1, CommandType.Text);
        if (blnCommit1)
        {
          this.objDataAccess.EndTransaction(blnCommit1);
        }
        else
        {
          ErroreMSG = "Errore durante la conferma del rappresentante legale";
          this.objDataAccess.EndTransaction(blnCommit1);
        }
      }
      catch (Exception ex)
      {
        this.objDataAccess.EndTransaction(blnCommit1);
        ErroreMSG = "Errore durante la conferma del rappresentante legale";
        return rap;
      }
      if (blnCommit1)
        this.objDataAccess.StartTransaction();
      string strSQL2 = "UPDATE RAPLEGWEB SET DATCONF = '" + DBMethods.Db2Date(DateTime.Now.ToString()) + "',  UTECONF = '" + u.Username + "' WHERE CODPOS = '" + rap.dettRap.codpos + "' AND PROREC = '" + rap.dettRap.prorec + "' ";
      try
      {
        blnCommit2 = this.objDataAccess.WriteTransactionData(strSQL2, CommandType.Text);
        if (blnCommit2)
        {
          this.objDataAccess.EndTransaction(blnCommit2);
        }
        else
        {
          ErroreMSG = "Errore durante la conferma del rappresentante legale";
          this.objDataAccess.EndTransaction(blnCommit2);
        }
      }
      catch (Exception ex)
      {
        this.objDataAccess.EndTransaction(blnCommit2);
        ErroreMSG = "Errore durante la conferma del rappresentante legale";
        return rap;
      }
      if (blnCommit1 && blnCommit2)
        SuccessMSG = "Conferma del rappresentante andata a buon fine";
      return rap;
    }

    public Rappresentante_legaleOCM Delete(
      Rappresentante_legaleOCM rap,
      string codpos,
      string prorec,
      string datconf,
      ref string ErroreMSG,
      ref string SuccessMSG)
    {
      this.objDataAccess.StartTransaction();
      if (!string.IsNullOrEmpty(datconf) || datconf == null)
      {
        string strSQL = "DELETE FROM RAPLEGWEB WHERE CODPOS = '" + codpos + "' AND PROREC = '" + prorec + "' ";
        try
        {
          if (this.objDataAccess.WriteData(strSQL, CommandType.Text))
            SuccessMSG = "Eliminazione andata a buon fine";
        }
        catch (Exception ex)
        {
          ErroreMSG = "Impossibile effettuare Eliminazione,errore durante la procedura";
          return rap;
        }
      }
      else
        ErroreMSG = "Impossibile effettuare Eliminazione, rappresentante già confermato";
      return rap;
    }
  }
}
