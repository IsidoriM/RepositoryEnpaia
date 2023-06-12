// Decompiled with JetBrains decompiler
// Type: DAL.Iscritto.PrivacyDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using System;
using System.Data;
using System.Web;
using TFI.DAL.ConnectorDB;
using TFI.OCM.Iscritto;

namespace DAL.Iscritto
{
    public class PrivacyDAL
    {
        public void GestionePrivacy()
        {
            string username = ((TFI.OCM.Utente.Utente)HttpContext.Current.Session["utente"]).Username;
            DataLayer dataLayer = new DataLayer();
            iDB2Parameter parameter = dataLayer.CreateParameter("@codfis", iDB2DbType.iDB2VarChar, 30, ParameterDirection.ReturnValue, username);
            string strSQL1 = "SELECT NOM, COG, CODFIS FROM ISCT WHERE CODFIS=@codfis";
            DataTable dataTable1 = new DataTable();
            string Err = "";
            DataTable tableWithParameters = dataLayer.GetDataTableWithParameters(strSQL1, parameter);
            string strSQL2 = "SELECT MAT FROM ISCT WHERE CODFIS = @codfis";
            DataTable dataTable2 = new DataTable();
            string str1 = dataLayer.GetDataTableWithParameters(strSQL2, parameter).Rows[0]["MAT"].ToString();
            HttpContext.Current.Items[(object)"Matricola"] = (object)str1;
            if (tableWithParameters.Rows.Count > 0)
            {
                string str2 = tableWithParameters.Rows[0]["NOM"].ToString();
                string str3 = tableWithParameters.Rows[0]["COG"].ToString();
                string str4 = tableWithParameters.Rows[0]["CODFIS"].ToString();
                HttpContext.Current.Items[(object)"Nome"] = (object)str2;
                HttpContext.Current.Items[(object)"Cognome"] = (object)str3;
                HttpContext.Current.Items[(object)"CodiceFis"] = (object)str4;
            }
            string str5 = " SELECT TRIM(COG) AS COG, TRIM(NOM) AS NOM, CODFIS FROM ISCT" + " WHERE MAT = '" + str1 + "'";
            string strSQL3 = "SELECT PRIVACY, PRIVACY2 FROM TB_PRIVACY A WHERE MAT = '" + str1 + "' AND DATINS = (SELECT MAX(DATINS) FROM TB_PRIVACY WHERE MAT = A.MAT)";
            DataSet dataSet1 = new DataSet();
            DataSet dataSet2 = dataLayer.GetDataSet(strSQL3, ref Err);
            if (dataSet2.Tables[0].Rows.Count <= 0)
                return;
            string str6 = dataSet2.Tables[0].Rows[0]["PRIVACY"].ToString();
            string str7 = dataSet2.Tables[0].Rows[0]["PRIVACY2"].ToString();
            HttpContext.Current.Items[(object)"Checked1"] = (object)str6;
            HttpContext.Current.Items[(object)"Checked2"] = (object)str7;
        }

        public void GestionePrivacy(Anagrafica a, ref string ErroreMSG, ref string SuccessMSG)
        {
            bool blnCommit = true;
            DataLayer dataLayer = new DataLayer();
            dataLayer.StartTransaction();
            string str1 = "";
            string username = ((TFI.OCM.Utente.Utente)HttpContext.Current.Session["utente"]).Username;
            iDB2Parameter parameter1 = dataLayer.CreateParameter("@codfis", iDB2DbType.iDB2VarChar, 30, ParameterDirection.Input, username);
            //  string strSQL1 = "SELECT NOME, COGNOME, CODFIS, MAT FROM ISCTWEB WHERE CODFIS=@codfis";
            string strSQL1 = "SELECT NOM, COG, CODFIS, MAT FROM ISCT WHERE CODFIS=@codfis";
            DataTable dataTable = new DataTable();
            string Err = "";
            DataTable tableWithParameters = dataLayer.GetDataTableWithParameters(strSQL1, parameter1);
            if (tableWithParameters.Rows.Count > 0)
            {
                string str2 = tableWithParameters.Rows[0]["NOM"].ToString();
                string str3 = tableWithParameters.Rows[0]["COG"].ToString();
                string str4 = tableWithParameters.Rows[0]["CODFIS"].ToString();
                str1 = tableWithParameters.Rows[0]["MAT"].ToString();
                dataLayer.CreateParameter(str1, iDB2DbType.iDB2Decimal, 20, ParameterDirection.ReturnValue, str1);
                HttpContext.Current.Items[(object)"Nome"] = (object)str2;
                HttpContext.Current.Items[(object)"Cognome"] = (object)str3;
                HttpContext.Current.Items[(object)"CodiceFis"] = (object)str4;
                HttpContext.Current.Items[(object)"Mat"] = (object)str1;
            }
            string str5 = HttpContext.Current.Request.Form["flexRadioDefault1"].ToString();
            string strSQL2 = "SELECT A.*, CURRENT_DATE AS OGGI FROM TB_PRIVACY A WHERE MAT = '" + str1 + "' AND DATINI = (SELECT MAX(DATINI) FROM TB_PRIVACY WHERE MAT = A.MAT)";
            /*DataSet dataSet1 = new DataSet();
            DataSet dataSet2 = new DataSet();
            DataSet dataSet3 = new DataSet();
            DataSet dataSet4 = new DataSet();
            DataSet dataSet5 = new DataSet();*/
            DataSet dataSet6 = dataLayer.GetDataSet(strSQL2, ref Err);
            if (dataSet6.Tables[0].Rows.Count > 0)
            {
                DateTime dateTime1 = Convert.ToDateTime(dataSet6.Tables[0].Rows[0]["DATINI"].ToString());
                DateTime dateTime2 = Convert.ToDateTime(dataSet6.Tables[0].Rows[0]["OGGI"].ToString());
                string str6 = dateTime1.ToString("yyyy-MM-dd");
                string str7 = dateTime2.ToString("yyyy-MM-dd");
                string str8 = dateTime2.ToString("yyyy-MM-dd");
                iDB2Parameter parameter2 = dataLayer.CreateParameter(str6, iDB2DbType.iDB2Decimal, 20, ParameterDirection.ReturnValue, str6);
                iDB2Parameter parameter3 = dataLayer.CreateParameter(str7, iDB2DbType.iDB2Decimal, 20, ParameterDirection.ReturnValue, str7);
                iDB2Parameter parameter4 = dataLayer.CreateParameter(str8, iDB2DbType.iDB2Decimal, 20, ParameterDirection.ReturnValue, str8);
                /*if (str6 == parameter3.iDB2Value.ToString())
                {
                    string strSQL3 = "DELETE FROM TB_PRIVACY WHERE MAT = '" + str1 + "' AND DATINI = '" + parameter2.iDB2Value?.ToString() + "'";
                    if (!dataLayer.WriteData(strSQL3, CommandType.Text))
                    {
                        blnCommit = false;
                        ErroreMSG = "Errore";
                    }
                }
                else
                {
                    string strSQL4 = "UPDATE TB_PRIVACY SET DATFIN = '" + parameter4.iDB2Value?.ToString() + "' WHERE MAT = '" + str1 + "' AND DATINI = '" + parameter2.iDB2Value?.ToString() + "'";
                    if (!dataLayer.WriteData(strSQL4, CommandType.Text))
                    {
                        blnCommit = false;
                        ErroreMSG = "Errore";
                    }
                }*/
                string strSQL4 = "UPDATE TB_PRIVACY SET DATFIN = '" + parameter4.iDB2Value?.ToString() + "' WHERE MAT = '" + str1 + "' AND DATINI = '" + parameter2.iDB2Value?.ToString() + "'";
                if (!dataLayer.WriteData(strSQL4, CommandType.Text))
                {
                    blnCommit = false;
                    ErroreMSG = "Errore";
                }
            }

            string strSQL5 = "INSERT INTO TB_PRIVACY (MAT, PRIVACY, PRIVACY2, TIPO, DATINI, DATFIN, DATINS, UTEINS) VALUES ('" + str1 + "','S','" + str5 + "','I', CURRENT_DATE, '9999-12-31', CURRENT_TIMESTAMP, @codfis)";
            if (dataLayer.WriteDataWithParameters(strSQL5, CommandType.Text, parameter1))
            {
                if (!dataLayer.WriteData("UPDATE ISCT SET FLGAPP = 'M' WHERE MAT = '" + str1 + "'", CommandType.Text))
                {
                    blnCommit = false;
                    ErroreMSG = "Errore";
                }
                else
                {
                    SuccessMSG = "Procedura terminata correttamente.";
                    HttpContext.Current.Items[(object)"Checked2"] = (object)str5;
                }
            }
            dataLayer.EndTransaction(blnCommit);
        }
    }
}
