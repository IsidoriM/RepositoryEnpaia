// Decompiled with JetBrains decompiler
// Type: DAL.Iscritto.CertUnicDAL
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

namespace DAL.Iscritto
{
    public class CertUnicDAL
    {
        public List<Cud> Dati()
        {
            string username = ((TFI.OCM.Utente.Utente)HttpContext.Current.Session["utente"]).Username;
            DataLayer dataLayer = new DataLayer();
            iDB2Parameter parameter = dataLayer.CreateParameter("@codfis", iDB2DbType.iDB2VarChar, 20, ParameterDirection.ReturnValue, username);
            string strSQL1 = "SELECT NOME, COGNOME, CODFIS, SIGPRO, DENLOC, IND, NUMCIV, CAP FROM ISCTWEB WHERE CODFIS=@codfis";
            DataTable dataTable1 = new DataTable();
            DataTable tableWithParameters = dataLayer.GetDataTableWithParameters(strSQL1, parameter);
            if (tableWithParameters.Rows.Count > 0)
            {
                string str1 = tableWithParameters.Rows[0]["NOME"].ToString();
                string str2 = tableWithParameters.Rows[0]["COGNOME"].ToString();
                string str3 = tableWithParameters.Rows[0]["CODFIS"].ToString();
                string str4 = tableWithParameters.Rows[0]["DENLOC"].ToString();
                string str5 = tableWithParameters.Rows[0]["SIGPRO"].ToString();
                string str6 = tableWithParameters.Rows[0]["IND"].ToString();
                string str7 = tableWithParameters.Rows[0]["NUMCIV"].ToString();
                string str8 = tableWithParameters.Rows[0]["CAP"].ToString();
                HttpContext.Current.Items[(object)"Nominativo"] = (object)(str1 + " " + str2);
                HttpContext.Current.Items[(object)"CodiceFis"] = (object)str3;
                HttpContext.Current.Items[(object)"NumeroCivico"] = (object)str7;
                HttpContext.Current.Items[(object)"Cap"] = (object)str8;
                HttpContext.Current.Items[(object)"Comune"] = (object)str4;
                HttpContext.Current.Items[(object)"Provincia"] = (object)str5;
                HttpContext.Current.Items[(object)"Indirizzo"] = (object)("VIA " + str6 + ", " + str7);
            }
            // string strSQL2 = "SELECT MAT FROM ISCTWEB WHERE CODFIS = '" + username + "'";
            string strSQL2 = "SELECT MAT FROM ISCT WHERE CODFIS = '" + username + "'";
            DataTable dataTable2 = new DataTable();
            string str9 = dataLayer.GetDataTableWithParameters(strSQL2, parameter).Rows[0]["MAT"].ToString();
            HttpContext.Current.Items[(object)"Matricola"] = (object)str9;
            dataLayer.CreateParameter(str9, iDB2DbType.iDB2VarChar, 20, ParameterDirection.ReturnValue, str9);
            string str10 = "SELECT ID, PROGCUD, ANNO, MAT, (SELECT TRIM(COG) || ' ' || TRIM(NOM) FROM ISCT WHERE MAT = CUD.MAT) AS NOMINATIVO, TIPOCUD, IMPORTO, NOMEFILE, ULTAGG FROM CUD WHERE MAT = " + str9 + " ORDER BY ANNO DESC";
            List<Cud> cudList = new List<Cud>();
            iDB2Command iDb2Command = new iDB2Command(str10);
            foreach (DataRow row in (InternalDataCollectionBase)dataLayer.GetDataTable(str10).Rows)
                cudList.Add(new Cud()
                {
                    Anno = (Decimal)Convert.ToInt32(row["ANNO"]),
                    NomeFile = Cypher.CryptPassword(row["NOMEFILE"].ToString()),
                    Importo = (Decimal)Convert.ToInt32(row["IMPORTO"]),
                    Mese = Convert.ToDateTime(row["ULTAGG"])
                });
            return cudList;
        }
    }
}
