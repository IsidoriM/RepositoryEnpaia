// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Amministrativo.RettifichePA_CA_DAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using System;
using System.Data;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.Amministrativo;

namespace TFI.DAL.Amministrativo
{
  public class RettifichePA_CA_DAL
  {
    private DataLayer objDataAccess = new DataLayer();

    public RettifichePA_CA_OCM SearchRett(
      RettifichePA_CA_OCM rettifichePA_CA_OCM1,
      string cerca,
      ref string MSGErorre,
      ref string MSGSuccess)
    {
      try
      {
        string strData1 = rettifichePA_CA_OCM1.search_rett.AnnoDA.ToString() + "-" + rettifichePA_CA_OCM1.search_rett.MeseDA.ToString().PadLeft(2, '0') + "-01";
        string strData2;
        if (rettifichePA_CA_OCM1.search_rett.AnnoA != null)
        {
          if (!string.IsNullOrEmpty(rettifichePA_CA_OCM1.search_rett.AnnoA.ToString()) && string.IsNullOrEmpty(rettifichePA_CA_OCM1.search_rett.MeseA.ToString()))
          {
            DateTime dateTime = Convert.ToDateTime(rettifichePA_CA_OCM1.search_rett.AnnoA.ToString() + "-01-01").AddMonths(1);
            dateTime = dateTime.AddDays(-1.0);
            strData2 = dateTime.ToString("yyyy-MM-dd");
          }
          else if (!string.IsNullOrEmpty(rettifichePA_CA_OCM1.search_rett.AnnoA.ToString()) && !string.IsNullOrEmpty(rettifichePA_CA_OCM1.search_rett.MeseA.ToString()))
          {
            DateTime dateTime = Convert.ToDateTime(rettifichePA_CA_OCM1.search_rett.AnnoA.ToString() + "-" + rettifichePA_CA_OCM1.search_rett.MeseA.ToString().PadLeft(2, '0') + "-01").AddMonths(1);
            dateTime = dateTime.AddDays(-1.0);
            strData2 = dateTime.ToString("yyyy-MM-dd");
          }
          else
            strData2 = "9999-12-31";
        }
        else
          strData2 = "9999-12-31";
        DataTable dataTable1 = this.objDataAccess.GetDataTable(rettifichePA_CA_OCM1.search_rett.AnnoA == null ? "SELECT ISCT.CODFIS, ISCT.MAT, ISCT.COG, ISCT.NOM, RAPLAV.DATDEC, RAPLAV.DATCES," + " RAPLAV.CODPOS, RAPLAV.PRORAP" + " FROM RAPLAV INNER JOIN ISCT ON RAPLAV.MAT = ISCT.MAT" + " WHERE CODPOS = '" + rettifichePA_CA_OCM1.search_rett.Codpos.ToString() + "' " + " AND '" + DBMethods.Db2Date(strData1) + "' <= VALUE(DATCES, '9999-12-31')" + " ORDER BY COG, NOM" : (!(rettifichePA_CA_OCM1.search_rett.AnnoA.ToString() != "") ? "SELECT ISCT.CODFIS, ISCT.MAT, ISCT.COG, ISCT.NOM, RAPLAV.DATDEC, RAPLAV.DATCES," + " RAPLAV.CODPOS, RAPLAV.PRORAP" + " FROM RAPLAV INNER JOIN ISCT ON RAPLAV.MAT = ISCT.MAT" + " WHERE CODPOS = '" + rettifichePA_CA_OCM1.search_rett.Codpos.ToString() + "' " + " AND '" + DBMethods.Db2Date(strData1) + "' <= VALUE(DATCES, '9999-12-31')" + " ORDER BY COG, NOM" : "SELECT ISCT.CODFIS, ISCT.MAT, ISCT.COG, ISCT.NOM, RAPLAV.DATDEC, RAPLAV.DATCES," + " RAPLAV.CODPOS, RAPLAV.PRORAP" + " FROM RAPLAV INNER JOIN ISCT ON RAPLAV.MAT = ISCT.MAT" + " WHERE CODPOS = '" + rettifichePA_CA_OCM1.search_rett.Codpos.ToString() + "' " + " AND '" + DBMethods.Db2Date(strData2) + "' >= DATDEC" + " AND '" + DBMethods.Db2Date(strData1) + "' <= VALUE(DATCES, '9999-12-31')" + " ORDER BY COG, NOM"));
        dataTable1.Columns.Add(new DataColumn("ABBPRE", typeof (string)));
        dataTable1.Columns.Add(new DataColumn("ASSCON", typeof (string)));
        if (dataTable1.Rows.Count > 0)
        {
          rettifichePA_CA_OCM1.search_rett.Occorrenze = dataTable1.Rows.Count.ToString();
          DataTable dataTable2 = new DataTable();
          for (int index = 0; index <= dataTable1.Rows.Count - 1; ++index)
          {
            dataTable2.Clear();
            dataTable2 = this.objDataAccess.GetDataTable(rettifichePA_CA_OCM1.search_rett.AnnoA == null ? "SELECT ASSCON, ABBPRE FROM STORDL" + " WHERE DATFIN >= '" + DBMethods.Db2Date(strData1) + "' " + " AND CODPOS = '" + dataTable1.Rows[index]["CODPOS"].ToString() + "'" + " AND MAT = '" + dataTable1.Rows[index]["MAT"].ToString() + "'" + " AND PRORAP = '" + dataTable1.Rows[index]["PRORAP"].ToString() + "'" + " ORDER BY DATINI DESC" : (!(rettifichePA_CA_OCM1.search_rett.AnnoA.ToString() != "") ? "SELECT ASSCON, ABBPRE FROM STORDL" + " WHERE DATFIN >= '" + DBMethods.Db2Date(strData1) + "' " + " AND CODPOS = '" + dataTable1.Rows[index]["CODPOS"].ToString() + "'" + " AND MAT = '" + dataTable1.Rows[index]["MAT"].ToString() + "'" + " AND PRORAP = '" + dataTable1.Rows[index]["PRORAP"].ToString() + "'" + " ORDER BY DATINI DESC" : "SELECT ASSCON, ABBPRE FROM STORDL" + " WHERE DATINI <= '" + DBMethods.Db2Date(strData2) + "' " + " AND DATFIN >= '" + DBMethods.Db2Date(strData1) + "' " + " AND CODPOS = '" + dataTable1.Rows[index]["CODPOS"].ToString() + "' " + " AND MAT = '" + dataTable1.Rows[index]["MAT"].ToString() + "' " + " AND PRORAP = '" + dataTable1.Rows[index]["PRORAP"].ToString() + "' " + " ORDER BY DATINI DESC"));
            if (dataTable2.Rows.Count > 0)
            {
              dataTable1.Rows[index]["ABBPRE"] = dataTable2.Rows[0]["ABBPRE"];
              dataTable1.Rows[index]["ASSCON"] = dataTable2.Rows[0]["ASSCON"];
            }
            else
            {
              dataTable1.Rows[index]["ABBPRE"] = (object) "N";
              dataTable1.Rows[index]["ASSCON"] = (object) "N";
            }
          }
        }
        if (dataTable1.Rows.Count > 0)
        {
          foreach (DataRow row in (InternalDataCollectionBase) dataTable1.Rows)
          {
            string str1 = "";
            string str2 = "";
            if (!string.IsNullOrEmpty(row["DATDEC"].ToString()))
              str1 = row["DATDEC"].ToString().Substring(0, 10);
            if (!string.IsNullOrEmpty(row["DATCES"].ToString()))
              str2 = row["DATCES"].ToString().Substring(0, 10);
            RettifichePA_CA_OCM.List_Rett listRett = new RettifichePA_CA_OCM.List_Rett()
            {
              Codpos = row["CODPOS"].ToString(),
              Mat = row["MAT"].ToString(),
              Codfis = row["CODFIS"].ToString(),
              Nom = row["NOM"].ToString(),
              Cog = row["COG"].ToString(),
              DatDec = str1,
              DatCes = str2,
              prorap = row["PRORAP"].ToString(),
              PA = row["ABBPRE"].ToString(),
              AC = row["ASSCON"].ToString()
            };
            rettifichePA_CA_OCM1.list_rett.Add(listRett);
          }
        }
      }
      catch (Exception ex)
      {
        MSGErorre = "Nessun risultato per la ricerca";
        return (RettifichePA_CA_OCM) null;
      }
      return rettifichePA_CA_OCM1;
    }

    public bool SalvaRett(
      RettifichePA_CA_OCM rettifichePA_CA_OCM1,
      ref string MSGErorre,
      ref string MSGSuccess,
      TFI.OCM.Utente.Utente u)
    {
      try
      {
        this.objDataAccess.StartTransaction();
        DataTable dataTable = new DataTable();
        ModGetDati modGetDati = new ModGetDati();
        string str1 = rettifichePA_CA_OCM1.search_rett.AnnoA.ToString() + "-" + rettifichePA_CA_OCM1.search_rett.MeseA.ToString().PadLeft(2, '0') + "-01";
        string str2;
        if (rettifichePA_CA_OCM1.search_rett.AnnoA != "")
        {
          DateTime dateTime = Convert.ToDateTime(rettifichePA_CA_OCM1.search_rett.AnnoDA.ToString() + "-" + rettifichePA_CA_OCM1.search_rett.MeseA.ToString().PadLeft(2, '0') + "-01");
          dateTime.AddMonths(1);
          dateTime.AddDays(-1.0);
          str2 = dateTime.ToString("yyyy-MM-dd");
        }
        else
          str2 = "9999-12-31";
        for (int index1 = 1; index1 <= rettifichePA_CA_OCM1.list_rett.Count - 1; ++index1)
        {
          if (rettifichePA_CA_OCM1.search_rett.AnnoA != "")
          {
            string strSQL = "SELECT CODPOS, MAT, PRORAP, DATINI, DATFIN FROM STORDL " + " WHERE DATINI <= '" + str1 + "'" + " AND DATFIN >= '" + str2 + "'" + " AND CODPOS = '" + rettifichePA_CA_OCM1.list_rett[index1].Codpos.ToString() + "' " + " AND MAT = '" + rettifichePA_CA_OCM1.list_rett[index1].Mat.ToString() + "' " + " AND PRORAP = '" + rettifichePA_CA_OCM1.list_rett[index1].prorap.ToString() + "' " + " ORDER BY DATINI DESC";
            dataTable.Clear();
            dataTable = this.objDataAccess.GetDataTable(strSQL);
            for (int index2 = 0; index2 <= dataTable.Rows.Count - 1; ++index2)
            {
              string str3 = "UPDATE STORDL SET";
              string str4 = !rettifichePA_CA_OCM1.list_rett[index1].PAbool ? str3 + " ABBPRE = 'N' " : str3 + " ABBPRE = 'S' ";
              this.objDataAccess.WriteTransactionData((!rettifichePA_CA_OCM1.list_rett[index1].ACbool ? str4 + ", ASSCON = 'N' " : str4 + ", ASSCON = 'S' ") + ", UTEAGG = '" + u.Username + "' " + ", ULTAGG = CURRENT_TIMESTAMP " + " WHERE CODPOS = '" + dataTable.Rows[index2]["CODPOS"].ToString() + "' " + " AND MAT = '" + dataTable.Rows[index2]["MAT"].ToString() + "' " + " AND PRORAP = '" + dataTable.Rows[index2]["PRORAP"].ToString() + "' " + " AND DATINI = '" + DBMethods.Db2Date(dataTable.Rows[index2]["DATINI"].ToString()) + "' " + " AND DATFIN = '" + DBMethods.Db2Date(dataTable.Rows[index2]["DATFIN"].ToString()) + "' ", CommandType.Text);
            }
          }
          else
          {
            string strSQL = "SELECT CODPOS, MAT, PRORAP, DATINI, DATFIN FROM STORDL" + " WHERE DATFIN >= '" + rettifichePA_CA_OCM1.search_rett.AnnoA + "'" + " AND CODPOS = '" + rettifichePA_CA_OCM1.list_rett[index1].Codpos.ToString() + "' " + " AND MAT = '" + rettifichePA_CA_OCM1.list_rett[index1].Mat.ToString() + "' " + " AND PRORAP = '" + rettifichePA_CA_OCM1.list_rett[index1].prorap.ToString() + "' " + " ORDER BY DATINI DESC";
            dataTable.Clear();
            dataTable = this.objDataAccess.GetDataTable(strSQL);
            for (int index3 = 0; index3 <= dataTable.Rows.Count - 1; ++index3)
            {
              string str5 = "UPDATE STORDL SET";
              string str6 = !(rettifichePA_CA_OCM1.list_rett[index3].PA.ToString() == "S") ? str5 + " ABBPRE = 'N' " : str5 + " ABBPRE = 'S' ";
              this.objDataAccess.WriteTransactionData((!(rettifichePA_CA_OCM1.list_rett[index3].AC.ToString() == "S") ? str6 + ", ASSCON = 'N' " : str6 + ", ASSCON = 'S' ") + ", UTEAGG = '" + u.Username + "' " + ", ULTAGG = CURRENT_TIMESTAMP " + " WHERE CODPOS = '" + dataTable.Rows[index3]["CODPOS"].ToString() + "' " + " AND MAT = '" + dataTable.Rows[index3]["MAT"].ToString() + "' " + " AND PRORAP = '" + dataTable.Rows[index3]["PRORAP"].ToString() + "' " + " AND DATINI = '" + DBMethods.Db2Date(dataTable.Rows[index3]["DATINI"].ToString()) + "' " + " AND DATFIN = '" + DBMethods.Db2Date(dataTable.Rows[index3]["DATFIN"].ToString()) + "' ", CommandType.Text);
            }
          }
          modGetDati.Module_AggiornaRaplav(this.objDataAccess, Convert.ToInt32(rettifichePA_CA_OCM1.list_rett[index1].Codpos), Convert.ToInt32(rettifichePA_CA_OCM1.list_rett[index1].Mat), Convert.ToInt32(rettifichePA_CA_OCM1.list_rett[index1].prorap));
        }
        this.objDataAccess.EndTransaction(true);
        MSGSuccess = "Operazione completata";
        return true;
      }
      catch (Exception ex)
      {
        this.objDataAccess.EndTransaction(false);
        MSGErorre = "Errore durante operazione di salvataggio";
        return false;
      }
    }

    public string GetRagioneSociale(string codPos)
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
  }
}
