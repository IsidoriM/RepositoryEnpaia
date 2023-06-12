// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Stampa.StampaDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using System;
using System.Data;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.AziendaConsulente;

namespace TFI.DAL.Stampa
{
  public class StampaDAL
  {
    public static bool AggiornamentoDiStampa(
      DataLayer db,
      string strProtocollo,
      string numPro,
      string datPro,
      string codPos,
      int anno,
      int mese,
      int proDen)
    {
      try
      {
        string strSQL = "UPDATE DENTES SET PROTRIC = " + DBMethods.DoublePeakForSql(strProtocollo) + ", " + " DATPROTRIC = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(datPro)) + ", " + " NUMPROTRIC = " + DBMethods.DoublePeakForSql(numPro) + ", FLGRIC = 'S'" + " WHERE CODPOS = " + codPos + " AND ANNDEN = " + anno.ToString() + " AND MESDEN = " + mese.ToString() + " AND PRODEN = " + proDen.ToString();
        return db.WriteTransactionData(strSQL, CommandType.Text);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public static DatiProtocollo GetDatiProtocollo(int idProtocollo)
    {
      DataLayer dataLayer = new DataLayer();
      try
      {
        string strSQL = "SELECT A.SOTTOCARTELLA AS CARTELLA , A.NOMEFILE FROM TBALLEGATI A, TBPROTOCOLLIALLEGATI" + " B WHERE A.IDALLEGATO=B.IDALLEGATO AND B.IDPROTOCOLLO =" + idProtocollo.ToString();
        DataTable dataTable = dataLayer.GetDataTable(strSQL);
        if (dataTable.Rows.Count <= 0)
          return (DatiProtocollo) null;
        return new DatiProtocollo()
        {
          Cartella = dataTable.Rows[0]["CARTELLA"].ToString(),
          NomeFile = dataTable.Rows[0]["NOMEFILE"].ToString()
        };
      }
      catch (Exception ex)
      {
        throw;
      }
    }
  }
}
