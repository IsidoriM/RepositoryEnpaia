// Decompiled with JetBrains decompiler
// Type: DAL.UtenteEnpaia.UtenteEnpaiaDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Data;
using TFI.DAL.ConnectorDB;
using TFI.OCM.AziendaConsulente;

namespace TFI.DAL.UtenteEnpaia
{
  public class UtenteEnpaiaDAL
  {
    public List<Azienda> GetAziendeEnpaia(
      string posizione,
      string ragioneSociale,
      string codiceFiscale,
      string partitaIVA)
    {
      string str = (string) null;
      List<iDB2Parameter> iDb2ParameterList = new List<iDB2Parameter>();
      DataLayer dataLayer = new DataLayer();
      List<Azienda> aziendeEnpaia = new List<Azienda>();
      if (!string.IsNullOrEmpty(posizione))
      {
        iDB2Parameter parameter = dataLayer.CreateParameter("@posizione", iDB2DbType.iDB2VarChar, 50, ParameterDirection.Input, posizione.ToUpper());
        str = " AND A.CODPOS = @posizione";
        iDb2ParameterList.Add(parameter);
      }
      if (!string.IsNullOrEmpty(ragioneSociale))
      {
        iDB2Parameter parameter = dataLayer.CreateParameter("@ragioneSociale", iDB2DbType.iDB2VarChar, 50, ParameterDirection.Input, "%" + ragioneSociale.ToUpper().Replace("'", "''") + "%");
        str += " AND A.RAGSOC LIKE @ragioneSociale";
        iDb2ParameterList.Add(parameter);
      }
      if (!string.IsNullOrEmpty(codiceFiscale))
      {
        iDB2Parameter parameter = dataLayer.CreateParameter("@codiceFiscale", iDB2DbType.iDB2VarChar, 50, ParameterDirection.Input, "%" + codiceFiscale.ToUpper() + "%");
        str += " AND A.CODFIS LIKE @codiceFiscale";
        iDb2ParameterList.Add(parameter);
      }
      if (!string.IsNullOrEmpty(partitaIVA))
      {
        iDB2Parameter parameter = dataLayer.CreateParameter("@partitaIVA", iDB2DbType.iDB2VarChar, 50, ParameterDirection.Input, "%" + partitaIVA.ToUpper() + "%");
        str += " AND A.PARIVA LIKE @partitaIVA";
        iDb2ParameterList.Add(parameter);
      }
      if (string.IsNullOrEmpty(posizione) && string.IsNullOrEmpty(ragioneSociale) && string.IsNullOrEmpty(codiceFiscale) && string.IsNullOrEmpty(partitaIVA))
        return (List<Azienda>) null;
      string strSQL = "SELECT A.CODPOS, A.RAGSOC, A.CODFIS, A.PARIVA FROM AZI A where VALUE(A.DATCHI, '9999-12-31') = '9999-12-31' " + str;
      foreach (DataRow row in (InternalDataCollectionBase) dataLayer.GetDataTableWithParameters(strSQL, iDb2ParameterList.ToArray()).Rows)
      {
        Azienda azienda = new Azienda()
        {
          codicePosizione = Convert.ToInt32(row["CODPOS"]),
          utenzaConsulente = row["RAGSOC"].ToString(),
          codiceFiscale = row["CODFIS"].ToString(),
          partitaIVA = row["PARIVA"].ToString()
        };
        aziendeEnpaia.Add(azienda);
      }
      return aziendeEnpaia;
    }
  }
}
