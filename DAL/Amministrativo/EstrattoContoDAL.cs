// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Amministrativo.EstrattoContoDAL
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
using TFI.OCM.Amministrativo;

namespace TFI.DAL.Amministrativo
{
  public class EstrattoContoDAL
  {
    public EstrattoContoOCM GetEstrattoContoAzienda(string CodPos, string ragsoc)
    {
      DataLayer dataLayer = new DataLayer();
      if (string.IsNullOrEmpty(CodPos) || !string.IsNullOrEmpty(ragsoc))
        CodPos = dataLayer.Get1ValueFromSQL("SELECT CODPOS FROM AZI WHERE RAGSOC='%" + ragsoc.Trim().ToUpper() + "%'", CommandType.Text);
      string strSQL1 = "SELECT RAGSOC, CODFIS, PARIVA FROM AZI WHERE CODPOS = '" + CodPos + "'";
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = dataLayer.GetDataTable(strSQL1);
      string strSQL2 = "SELECT IND, NUMCIV, DENLOC, DENSTAEST, SIGPRO, CAP FROM INDSED WHERE CODPOS = '" + CodPos + "' AND TIPIND = 1 AND CURRENT_DATE BETWEEN DATINI AND DATFIN ";
      DataTable dataTable3 = new DataTable();
      DataTable dataTable4 = dataLayer.GetDataTable(strSQL2);
      string str = "SELECT ID, ANNO, MAT, CODPOS, NOMEFILE FROM EST_CONTO WHERE WEB = 'S' AND CODPOS = '" + CodPos + "'  ORDER BY ANNO";
      DataTable dataTable5 = new DataTable();
      DataTable dataTable6 = dataLayer.GetDataTable(str);
      EstrattoContoOCM estrattoContoAzienda = new EstrattoContoOCM();
      if (dataTable6.Rows.Count > 0)
      {
        EstrattoContoOCM.Azienda azienda = new EstrattoContoOCM.Azienda()
        {
          RagioneSociale = dataTable2.Rows[0]["RAGSOC"].ToString(),
          PartitaIva = dataTable2.Rows[0]["PARIVA"].ToString(),
          CodiceFis = dataTable2.Rows[0]["CODFIS"].ToString(),
          Indirizzo = dataTable4.Rows[0]["IND"].ToString(),
          NumeroCivico = dataTable4.Rows[0]["NUMCIV"].ToString(),
          CAP = dataTable4.Rows[0]["CAP"].ToString(),
          Provincia = dataTable4.Rows[0]["SIGPRO"].ToString(),
          StaestRes = dataTable4.Rows[0]["DENSTAEST"].ToString(),
          Comune = dataTable4.Rows[0]["DENLOC"].ToString(),
          Posizione = dataTable6.Rows[0]["CODPOS"].ToString()
        };
        if (string.IsNullOrEmpty(azienda.StaestRes))
          azienda.Comune = dataTable4.Rows[0]["DENLOC"].ToString();
        List<EstrattoContoOCM.EstrattiConto> estrattiContoList = new List<EstrattoContoOCM.EstrattiConto>();
        iDB2DataReader dataReaderFromQuery = dataLayer.GetDataReaderFromQuery(str, CommandType.Text);
        while (dataReaderFromQuery.Read())
          estrattiContoList.Add(new EstrattoContoOCM.EstrattiConto()
          {
            Anno = (Decimal) Convert.ToInt32(dataReaderFromQuery["Anno"]),
            NomeFile = Cypher.CryptPassword(dataReaderFromQuery["NomeFile"].ToString())
          });
        estrattoContoAzienda.estrattiContos = estrattiContoList;
        estrattoContoAzienda.az = azienda;
        return estrattoContoAzienda;
      }
      TFI.OCM.Utente.Utente utente = (TFI.OCM.Utente.Utente) HttpContext.Current.Session["utente"];
      EstrattoContoOCM.Azienda azienda1 = new EstrattoContoOCM.Azienda()
      {
        RagioneSociale = dataTable2.Rows[0]["RAGSOC"].ToString(),
        PartitaIva = dataTable2.Rows[0]["PARIVA"].ToString(),
        CodiceFis = dataTable2.Rows[0]["CODFIS"].ToString(),
        Indirizzo = dataTable4.Rows[0]["IND"].ToString(),
        NumeroCivico = dataTable4.Rows[0]["NUMCIV"].ToString(),
        CAP = dataTable4.Rows[0]["CAP"].ToString(),
        Provincia = dataTable4.Rows[0]["SIGPRO"].ToString(),
        StaestRes = dataTable4.Rows[0]["DENSTAEST"].ToString(),
        Comune = dataTable4.Rows[0]["DENLOC"].ToString(),
        Posizione = utente.CodPosizione
      };
      if (string.IsNullOrEmpty(azienda1.StaestRes))
        azienda1.Comune = dataTable4.Rows[0]["DENLOC"].ToString();
      List<EstrattoContoOCM.EstrattiConto> estrattiContoList1 = new List<EstrattoContoOCM.EstrattiConto>();
      iDB2DataReader dataReaderFromQuery1 = dataLayer.GetDataReaderFromQuery(str, CommandType.Text);
      while (dataReaderFromQuery1.Read())
        estrattiContoList1.Add(new EstrattoContoOCM.EstrattiConto()
        {
          Anno = (Decimal) Convert.ToInt32(dataReaderFromQuery1["Anno"]),
          NomeFile = Cypher.CryptPassword(dataReaderFromQuery1["NomeFile"].ToString())
        });
      estrattoContoAzienda.estrattiContos = estrattiContoList1;
      estrattoContoAzienda.az = azienda1;
      return estrattoContoAzienda;
    }

    public EstrattoContoOCM GetEstrattoContoIscritto(
      string mat,
      string nom,
      string cog,
      string codfis)
    {
      EstrattoContoOCM estrattoContoIscritto = new EstrattoContoOCM();
      DataLayer dataLayer = new DataLayer();
      string strSQL = "SELECT MAT,COGNOME, NOME, CODFIS, IND, NUMCIV, CAP, DENLOC, SIGPRO FROM ISCTWEB  WHERE MAT='" + mat + "' ";
      if (!string.IsNullOrEmpty(nom))
        strSQL = strSQL + " AND NOME='" + nom + "' ";
      if (!string.IsNullOrEmpty(cog))
        strSQL = strSQL + " AND COGNOME='" + cog + "' ";
      if (!string.IsNullOrEmpty(codfis))
        strSQL = strSQL + " AND CODFIS='" + codfis + "' ";
      DataTable dataTable = dataLayer.GetDataTable(strSQL);
      EstrattoContoOCM.Iscritto iscritto = new EstrattoContoOCM.Iscritto()
      {
        matrciola = dataTable.Rows[0]["MAT"].ToString(),
        cognome = dataTable.Rows[0]["COGNOME"].ToString(),
        nome = dataTable.Rows[0]["NOME"].ToString(),
        codfis = dataTable.Rows[0]["CODFIS"].ToString(),
        Indirizzo = dataTable.Rows[0]["IND"].ToString(),
        NumeroCivico = dataTable.Rows[0]["NUMCIV"].ToString(),
        CAP = dataTable.Rows[0]["CAP"].ToString(),
        comune = dataTable.Rows[0]["DENLOC"].ToString(),
        prov = dataTable.Rows[0]["SIGPRO"].ToString()
      };
      List<EstrattoContoOCM.EstrattiContoIsc> estrattiContoIscList = new List<EstrattoContoOCM.EstrattiContoIsc>();
      string strQuery = "SELECT ID, ANNO, MAT, (SELECT TRIM(COG) || ' ' || TRIM(NOM) FROM ISCT WHERE MAT = EST_CONTO.MAT) AS NOMINATIVO, CODPOS, NOMEFILE FROM EST_CONTO  WHERE WEB = 'S' AND MAT = '" + mat + "' ORDER BY ANNO ";
      iDB2DataReader dataReaderFromQuery = dataLayer.GetDataReaderFromQuery(strQuery, CommandType.Text);
      while (dataReaderFromQuery.Read())
        estrattiContoIscList.Add(new EstrattoContoOCM.EstrattiContoIsc()
        {
          Anno = (Decimal) Convert.ToInt32(dataReaderFromQuery["Anno"]),
          NomeFile = Cypher.CryptPassword(dataReaderFromQuery["NomeFile"].ToString())
        });
      estrattoContoIscritto.estrattiContoIsc = estrattiContoIscList;
      estrattoContoIscritto.isc = iscritto;
      return estrattoContoIscritto;
    }
  }
}
