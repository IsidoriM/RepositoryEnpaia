// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Iscritto.ProsPagDAL
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
using TFI.OCM;

namespace TFI.DAL.Iscritto
{
  public class ProsPagDAL
  {
    public List<Prospetti> GeneraProspettiPagamento(ref string ErroreMSG)
    {
      string username = ((TFI.OCM.Utente.Utente) HttpContext.Current.Session["utente"]).Username;
      DataLayer dataLayer = new DataLayer();
      iDB2Parameter parameter1 = dataLayer.CreateParameter("@codfis", iDB2DbType.iDB2VarChar, 20, ParameterDirection.ReturnValue, username);
      string strSQL1 = "SELECT COGNOME, NOME, CODFIS,IND, NUMCIV, CAP, DENLOC, SIGPRO, CODDUG FROM ISCTWEB WHERE CODFIS='" + username + "'";
      DataTable dataTable1 = new DataTable();
      DataTable tableWithParameters1 = dataLayer.GetDataTableWithParameters(strSQL1, parameter1);
      string str1 = tableWithParameters1.Rows[0]["NOME"].ToString();
      string str2 = tableWithParameters1.Rows[0]["COGNOME"].ToString();
      string str3 = tableWithParameters1.Rows[0]["CODFIS"].ToString();
      string str4 = tableWithParameters1.Rows[0]["IND"].ToString();
      string str5 = tableWithParameters1.Rows[0]["NUMCIV"].ToString();
      string str6 = tableWithParameters1.Rows[0]["CAP"].ToString();
      string str7 = tableWithParameters1.Rows[0]["DENLOC"].ToString();
      string str8 = tableWithParameters1.Rows[0]["SIGPRO"].ToString();
      string parameterValue1 = tableWithParameters1.Rows[0]["CODDUG"].ToString();
      iDB2Parameter parameter2 = dataLayer.CreateParameter("@coddug", iDB2DbType.iDB2Decimal, 30, ParameterDirection.ReturnValue, parameterValue1);
      string strSQL2 = "SELECT DENDUG FROM DUG WHERE CODDUG=" + parameterValue1;
      DataTable dataTable2 = new DataTable();
      DataTable tableWithParameters2 = dataLayer.GetDataTableWithParameters(strSQL2, parameter2);
      string str9 = tableWithParameters2.Rows[0]["DENDUG"].ToString();
      if (tableWithParameters2.Rows.Count > 0)
      {
        HttpContext.Current.Items[(object) "Nominativo"] = (object) (str1 + " " + str2);
        HttpContext.Current.Items[(object) "CodiceFis"] = (object) str3;
        HttpContext.Current.Items[(object) "Indirizzo"] = (object) (str9 + " " + str4 + " " + str5);
        HttpContext.Current.Items[(object) "Cap"] = (object) str6;
        HttpContext.Current.Items[(object) "Comune"] = (object) str7;
        HttpContext.Current.Items[(object) "Provincia"] = (object) str8;
      }
      string strSQL3 = "SELECT MAT FROM ISCTWEB WHERE CODFIS='" + username + "'";
      DataTable dataTable3 = new DataTable();
      string parameterValue2 = dataLayer.GetDataTableWithParameters(strSQL3, parameter1).Rows[0]["MAT"].ToString();
      HttpContext.Current.Items[(object) "Matricola"] = (object) parameterValue2;
      dataLayer.CreateParameter("@mat", iDB2DbType.iDB2VarChar, 20, ParameterDirection.ReturnValue, parameterValue2);
      List<Prospetti> prospettiList = new List<Prospetti>();
      string strQuery = "SELECT ID, ANNOELE, NOMEFILE FROM LETLIQ WHERE MAT ='" + parameterValue2 + "'AND TIPDEST = 'I' AND WEB = 'S' ORDER BY ANNOELE DESC";
      iDB2DataReader dataReaderFromQuery = dataLayer.GetDataReaderFromQuery(strQuery, CommandType.Text);
      while (dataReaderFromQuery.Read())
        prospettiList.Add(new Prospetti()
        {
          ANNOELE = Convert.ToInt32(dataReaderFromQuery["ANNOELE"]),
          NOMEFILE = Cypher.CryptPassword(dataReaderFromQuery["NOMEFILE"].ToString())
        });
      return prospettiList;
    }
  }
}
