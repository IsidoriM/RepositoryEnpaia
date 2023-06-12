// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Iscritto.IscrittoEstrattoContoDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using OCM.Iscritto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using TFI.CRYPTO.Crypto;
using TFI.DAL.ConnectorDB;

namespace TFI.DAL.Iscritto
{
  public class IscrittoEstrattoContoDAL
  {
    public List<ListaEstrattiConto> EstrattoConto()
    {
      string username = ((TFI.OCM.Utente.Utente) HttpContext.Current.Session["utente"]).Username;
      DataLayer dataLayer = new DataLayer();
      iDB2Parameter parameter = dataLayer.CreateParameter("@codfis", iDB2DbType.iDB2VarChar, 30, ParameterDirection.ReturnValue, username);
      string strSQL1 = "SELECT COGNOME, NOME, CODFIS, IND, NUMCIV, CAP, DENLOC, SIGPRO FROM ISCTWEB  WHERE CODFIS='" + username + "'";
      DataTable dataTable1 = new DataTable();
      DataTable tableWithParameters = dataLayer.GetDataTableWithParameters(strSQL1, parameter);
      string str1 = tableWithParameters.Rows[0]["NOME"].ToString();
      string str2 = tableWithParameters.Rows[0]["COGNOME"].ToString();
      string str3 = tableWithParameters.Rows[0]["CODFIS"].ToString();
      string str4 = tableWithParameters.Rows[0]["IND"].ToString();
      string str5 = tableWithParameters.Rows[0]["NUMCIV"].ToString();
      string str6 = tableWithParameters.Rows[0]["CAP"].ToString();
      string str7 = tableWithParameters.Rows[0]["DENLOC"].ToString();
      string str8 = tableWithParameters.Rows[0]["SIGPRO"].ToString();
      if (tableWithParameters.Rows.Count > 0)
      {
        HttpContext.Current.Items[(object) "NomeCognome"] = (object) (str1 + " " + str2);
        HttpContext.Current.Items[(object) "CodiceFis"] = (object) str3;
        HttpContext.Current.Items[(object) "Indirizzo"] = (object) ("Via " + str4 + " " + str5);
        HttpContext.Current.Items[(object) "Cap"] = (object) str6;
        HttpContext.Current.Items[(object) "Comune2"] = (object) str7;
        HttpContext.Current.Items[(object) "Provincia2"] = (object) str8;
      }
      string strSQL2 = "SELECT MAT FROM ISCTWEB WHERE CODFIS='" + username + "'";
      DataTable dataTable2 = new DataTable();
      string str9 = dataLayer.GetDataTableWithParameters(strSQL2, parameter).Rows[0]["MAT"].ToString();
      HttpContext.Current.Items[(object) "Matricola"] = (object) str9;
      List<ListaEstrattiConto> listaEstrattiContoList = new List<ListaEstrattiConto>();
      string strQuery = "SELECT ID, ANNO, MAT, (SELECT TRIM(COG) || ' ' || TRIM(NOM) FROM ISCT WHERE MAT = EST_CONTO.MAT) AS NOMINATIVO, CODPOS, NOMEFILE FROM EST_CONTO  WHERE WEB = 'S' AND MAT = '" + str9 + "' ORDER BY ANNO DESC";
      iDB2DataReader dataReaderFromQuery = dataLayer.GetDataReaderFromQuery(strQuery, CommandType.Text);
      while (dataReaderFromQuery.Read())
        listaEstrattiContoList.Add(new ListaEstrattiConto()
        {
          Anno = (Decimal) Convert.ToInt32(dataReaderFromQuery["Anno"]),
          NomeFile = Cypher.CryptPassword(dataReaderFromQuery["NomeFile"].ToString())
        });
      return listaEstrattiContoList;
    }
  }
}
