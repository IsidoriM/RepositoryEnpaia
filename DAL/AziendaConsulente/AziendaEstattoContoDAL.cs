// Decompiled with JetBrains decompiler
// Type: TFI.DAL.AziendaConsulente.AziendaEstattoContoDAL
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
using TFI.OCM.AziendaConsulente;

namespace TFI.DAL.AziendaConsulente
{
  public class AziendaEstattoContoDAL
  {
    public AnagraficaEstrattiContoAzienda GetEstrattoContoAzienda(
      string CodPos)
    {
      DataLayer dataLayer = new DataLayer();
      string strSQL1 = "SELECT RAGSOC, CODFIS, PARIVA FROM AZI WHERE CODPOS = '" + CodPos + "'";
      DataTable dataTable1 = new DataTable();
      DataTable dataTable2 = dataLayer.GetDataTable(strSQL1);
      string strSQL2 = "SELECT IND, NUMCIV, DENLOC, DENSTAEST, SIGPRO, CAP FROM INDSED WHERE CODPOS = '" + CodPos + "' AND TIPIND = 1 AND CURRENT_DATE BETWEEN DATINI AND DATFIN ";
      DataTable dataTable3 = new DataTable();
      DataTable dataTable4 = dataLayer.GetDataTable(strSQL2);
      string str = "SELECT ID, ANNO, MAT, CODPOS, NOMEFILE FROM EST_CONTO WHERE WEB = 'S' AND CODPOS = '" + CodPos + "'  ORDER BY ANNO";
      DataTable dataTable5 = new DataTable();
      DataTable dataTable6 = dataLayer.GetDataTable(str);
      if (dataTable6.Rows.Count > 0)
      {
        AnagraficaEstrattiContoAzienda estrattoContoAzienda = new AnagraficaEstrattiContoAzienda()
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
        if (string.IsNullOrEmpty(estrattoContoAzienda.StaestRes))
          estrattoContoAzienda.Comune = dataTable4.Rows[0]["DENLOC"].ToString();
        estrattoContoAzienda.estrattiContos = new List<EstrattiConto>();
        iDB2DataReader dataReaderFromQuery = dataLayer.GetDataReaderFromQuery(str, CommandType.Text);
        while (dataReaderFromQuery.Read())
          estrattoContoAzienda.estrattiContos.Add(new EstrattiConto()
          {
            Anno = (Decimal) Convert.ToInt32(dataReaderFromQuery["Anno"]),
            NomeFile = Cypher.CryptPassword(dataReaderFromQuery["NomeFile"].ToString())
          });
        return estrattoContoAzienda;
      }
      TFI.OCM.Utente.Utente utente = (TFI.OCM.Utente.Utente) HttpContext.Current.Session["utente"];
      AnagraficaEstrattiContoAzienda estrattoContoAzienda1 = new AnagraficaEstrattiContoAzienda()
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
      if (string.IsNullOrEmpty(estrattoContoAzienda1.StaestRes))
        estrattoContoAzienda1.Comune = dataTable4.Rows[0]["DENLOC"].ToString();
      estrattoContoAzienda1.estrattiContos = new List<EstrattiConto>();
      iDB2DataReader dataReaderFromQuery1 = dataLayer.GetDataReaderFromQuery(str, CommandType.Text);
      while (dataReaderFromQuery1.Read())
        estrattoContoAzienda1.estrattiContos.Add(new EstrattiConto()
        {
          Anno = (Decimal) Convert.ToInt32(dataReaderFromQuery1["Anno"]),
          NomeFile = Cypher.CryptPassword(dataReaderFromQuery1["NomeFile"].ToString())
        });
      return estrattoContoAzienda1;
    }
  }
}
