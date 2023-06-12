// Decompiled with JetBrains decompiler
// Type: DAL.Iscritto.AnticipazioniTFRDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using TFI.CRYPTO.Crypto;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.Amministrativo;
using TFI.OCM.Iscritto;
using Utilities;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace DAL.Iscritto
{
    public class AnticipazioniTFRDAL
    {
        private readonly DataLayer objDataAccess = new DataLayer();

        public List<IscrittoRicTFROCM.RichiestaLiquidazione> GetListaRichiestaLiquidazione(
          string username)
        {
            List<IscrittoRicTFROCM.RichiestaLiquidazione> richiestaLiquidazione = new List<IscrittoRicTFROCM.RichiestaLiquidazione>();
            DataTable dataTable = this.objDataAccess.GetDataTable("SELECT t.ID, t.CODPOS, t.DTRIC, t.IMPTFR, t.TIPLIQ, a.RAGSOC FROM TBRICLIQ t LEFT OUTER JOIN AZI a ON t.CODPOS = a.CODPOS WHERE MAT = '" + this.GetMatricolaIscritto(username) + "' AND TIPLIQ = '3'");
            if (dataTable.Rows.Count <= 0)
                return (List<IscrittoRicTFROCM.RichiestaLiquidazione>)null;
            foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
                richiestaLiquidazione.Add(new IscrittoRicTFROCM.RichiestaLiquidazione()
                {
                    ID = !DBNull.Value.Equals(row["ID"]) ? Convert.ToInt32(row["ID"]) : 0,
                    RAGSOC = !DBNull.Value.Equals(row["RAGSOC"]) ? row["RAGSOC"].ToString() : "Dato non disponibile",
                    CODPOS = !DBNull.Value.Equals(row["CODPOS"]) ? Convert.ToInt32(row["CODPOS"]) : 0,
                    DTRIC = !DBNull.Value.Equals(row["DTRIC"]) ? row["DTRIC"].ToString() : "Dato non disponibile",
                    IMPTFR = !DBNull.Value.Equals(row["IMPTFR"]) ? Convert.ToDecimal(row["IMPTFR"]) : 0M,
                    TIPLIQ = !DBNull.Value.Equals(row["TIPLIQ"]) ? Convert.ToInt32(row["TIPLIQ"]) : 0
                });
            return richiestaLiquidazione;
        }

        public void ATFR(ref string ErroreMSG, ref string SuccessMSG, ref string InfoMSG)
        {
            string username = ((TFI.OCM.Utente.Utente)HttpContext.Current.Session["utente"]).Username;
            DataLayer dataLayer = new DataLayer();
            dataLayer.CreateParameter("@codfis", iDB2DbType.iDB2VarChar, 30, ParameterDirection.ReturnValue, username);
            string strSQL1 = "SELECT MAT, COGNOME, NOME, CODFIS, DATNAS, SES, STAESTNAS, IND, NUMCIV, CAP, EMAIL, EMAILCERT, CELL, DENLOC, SIGPRO, TIPO_DOCUMENTO, NUMERO_DOCUMENTO, DATA_SCADENZA_DOCUMENTO FROM ISCTWEB WHERE CODFIS='" + username + "'";
            DataTable dataTable1 = new DataTable();
            string Err = "";
            DataTable dataTable2 = dataLayer.GetDataTable(strSQL1);
            string str1 = dataTable2.Rows[0]["MAT"].ToString();
            string str2 = dataTable2.Rows[0]["NOME"].ToString();
            string str3 = dataTable2.Rows[0]["COGNOME"].ToString();
            string str4 = dataTable2.Rows[0]["CODFIS"].ToString();
            string str5 = dataTable2.Rows[0]["SES"].ToString();
            string str6 = dataTable2.Rows[0]["DATNAS"].ToString().Substring(0, 10);
            string str7 = dataTable2.Rows[0]["STAESTNAS"].ToString();
            string str8 = dataTable2.Rows[0]["IND"].ToString();
            string str9 = dataTable2.Rows[0]["NUMCIV"].ToString();
            string str10 = dataTable2.Rows[0]["CAP"].ToString();
            string str11 = dataTable2.Rows[0]["DENLOC"].ToString();
            string str12 = dataTable2.Rows[0]["SIGPRO"].ToString();
            string str13 = dataTable2.Rows[0]["EMAIL"].ToString();
            string str14 = dataTable2.Rows[0]["EMAILCERT"].ToString();
            string str15 = dataTable2.Rows[0]["CELL"].ToString();
            string strSQL2 = "SELECT TEL1, TEL2, FAX FROM ISCD WHERE MAT='" + str1 + "'";
            DataTable dataTable3 = new DataTable();
            DataTable dataTable4 = dataLayer.GetDataTable(strSQL2);
            string str16 = dataTable4.Rows[0]["TEL1"].ToString();
            string str17 = dataTable4.Rows[0]["TEL2"].ToString();
            string str18 = dataTable4.Rows[0]["FAX"].ToString();
            string str19 = dataTable2.Rows[0]["NUMERO_DOCUMENTO"].ToString();
            string str20 = dataTable2.Rows[0]["TIPO_DOCUMENTO"].ToString();
            string str21 = dataTable2.Rows[0]["DATA_SCADENZA_DOCUMENTO"].ToString();
            if (dataTable2.Rows.Count > 0)
            {
                HttpContext.Current.Items[(object)"Matricola"] = (object)str1;
                HttpContext.Current.Items[(object)"Nome"] = (object)str2;
                HttpContext.Current.Items[(object)"Cognome"] = (object)str3;
                HttpContext.Current.Items[(object)"CodiceFis"] = (object)str4;
                HttpContext.Current.Items[(object)"Indirizzo"] = (object)("Via " + str8 + " " + str9);
                HttpContext.Current.Items[(object)"Cap"] = (object)str10;
                HttpContext.Current.Items[(object)"Sesso"] = (object)str5;
                HttpContext.Current.Items[(object)"Cell"] = (object)str15;
                HttpContext.Current.Items[(object)"Email"] = (object)str13;
                HttpContext.Current.Items[(object)"EmailCert"] = (object)str14;
                HttpContext.Current.Items[(object)"Comune2"] = (object)str11;
                HttpContext.Current.Items[(object)"Provincia2"] = (object)str12;
                HttpContext.Current.Items[(object)"Datadinascita"] = (object)str6;
                HttpContext.Current.Items[(object)"TEL1"] = (object)str16;
                HttpContext.Current.Items[(object)"TEL2"] = (object)str17;
                HttpContext.Current.Items[(object)"FAX"] = (object)str18;
                HttpContext.Current.Items[(object)"SEN"] = (object)str7;
                HttpContext.Current.Items[(object)"TipoDocumento"] = (object)str20;
                HttpContext.Current.Items[(object)"NumeroDocumento"] = (object)str19;
                if (!string.IsNullOrEmpty(str21))
                    HttpContext.Current.Items[(object)"ScadenzaDocumento"] = (object)Convert.ToDateTime(str21).ToString("yyyy-MM-dd");
                else
                    HttpContext.Current.Items[(object)"ScadenzaDocumento"] = (object)" ";
            }
            iDB2Connection iDb2Connection = new iDB2Connection(Cypher.DeCryptPassword(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString));
            string str22 = "SELECT CODMOTANT, DENMOTANT FROM MOTANTLIQ";
            DataSet dataSet1 = new DataSet();
            if (dataLayer.GetDataSet(str22, ref Err).Tables[0].Rows.Count > 0)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                iDB2Command iDb2Command = new iDB2Command(str22);
                iDb2Command.Connection = iDb2Connection;
                iDb2Connection.Open();
                iDB2DataReader iDb2DataReader = iDb2Command.ExecuteReader();
                while (iDb2DataReader.Read())
                    dictionary.Add(iDb2DataReader["CODMOTANT"].ToString(), iDb2DataReader["DENMOTANT"].ToString());
                iDb2Connection.Close();
                HttpContext.Current.Items[(object)"Motivazione"] = (object)dictionary;
            }
            string str23 = "SELECT CODPOS, TRIM(CODPOS || ' - ' || (SELECT TRIM(RAGSOC) FROM AZI WHERE CODPOS = A.CODPOS)) AS POSIZIONE, DATDEC FROM RAPLAV A WHERE MAT = " + str1 + " AND YEAR(CURRENT_DATE - DATLIQTFR) >= 8 AND CURRENT_DATE BETWEEN DATDEC AND VALUE(DATCES, '9999-12-31') AND CODPOS || MAT || PRORAP NOT IN (SELECT CODPOS || MAT || PRORAPTFR FROM TBRICLIQ WHERE TIPLIQ = 3 AND MAT = A.MAT)";
            DataSet dataSet2 = new DataSet();
            DataSet dataSet3 = dataLayer.GetDataSet(str23, ref Err);
            try
            {
                if (dataSet3.Tables[0].Rows.Count > 0)
                {
                    List<string> stringList = new List<string>();
                    iDB2Command iDb2Command = new iDB2Command(str23);
                    iDb2Command.Connection = iDb2Connection;
                    iDb2Connection.Open();
                    iDB2DataReader iDb2DataReader = iDb2Command.ExecuteReader();
                    while (iDb2DataReader.Read())
                        stringList.Add(iDb2DataReader["POSIZIONE"].ToString() + "<" + iDb2DataReader["DATDEC"].ToString());
                    iDb2Connection.Close();
                    if (stringList.Count > 0)
                        HttpContext.Current.Items[(object)"Posizione"] = (object)stringList;
                    else
                        HttpContext.Current.Items[(object)"Posizione"] = (object)"Nessun rapporto di lavoro disponibile";
                }
                else
                {
                    InfoMSG = "Non e' possibile richiedere l'anticipazione del TFR, il rapporto assicurativo e' inferiore agli otto anni (legge 297/82).";
                    HttpContext.Current.Items[(object)"Posizione"] = (object)"Nessun rapporto di lavoro disponibile";
                    HttpContext.Current.Items[(object)"Visibility"] = (object)false;
                }
            }
            catch (NullReferenceException ex)
            {
                HttpContext.Current.Items[(object)"Posizione"] = (object)"";
            }
        }
        public bool CheckAnticipoTFR(int matricola)
        {
            DataLayer dataLayer = new DataLayer();
            string Err = string.Empty;
            string str23 = "SELECT CODPOS, TRIM(CODPOS || ' - ' || (SELECT TRIM(RAGSOC) FROM AZI WHERE CODPOS = A.CODPOS)) AS POSIZIONE, DATDEC FROM RAPLAV A WHERE MAT = " + matricola.ToString() + " AND YEAR(CURRENT_DATE - DATLIQTFR) >= 8 AND CURRENT_DATE BETWEEN DATDEC AND VALUE(DATCES, '9999-12-31') AND CODPOS || MAT || PRORAP NOT IN (SELECT CODPOS || MAT || PRORAPTFR FROM TBRICLIQ WHERE TIPLIQ = 3 AND MAT = A.MAT)";
            DataSet dataSet2 = dataLayer.GetDataSet(str23, ref Err);
            if (dataSet2.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void ATFR(Anagrafica a, ref string ErroreMSG, ref string SuccessMSG)
        {
            string Err = "";
            string cf = HttpContext.Current.Request.Form["codfis"].ToString();
            DataLayer dataLayer = new DataLayer();
            dataLayer.StartTransaction();
            iDB2Parameter parameter = dataLayer.CreateParameter("@codfis", iDB2DbType.iDB2VarChar, 30, ParameterDirection.ReturnValue, cf);
            string detimp = HttpContext.Current.Request.Form["CheckFruizione"].ToString();
            string matQuery = "SELECT MAT FROM ISCT WHERE CODFIS='" + cf + "'";
            DataSet dataSet = new DataSet();
            string matricola = dataLayer.GetDataSet(matQuery, ref Err).Tables[0].Rows[0]["MAT"].ToString();
            string str2 = HttpContext.Current.Request.Form["RAL"].Substring(0, 5);
            int num = HttpContext.Current.Request.Form["RAL"].IndexOf("<");
            string str3 = Convert.ToDateTime(HttpContext.Current.Request.Form["RAL"].ToString().Substring(num + 1, 10)).ToString("yyyy-MM-dd");
            string str4 = HttpContext.Current.Request.Form["MotRic"];
            string iban = HttpContext.Current.Request.Form["IBAN"];
            string str5 = HttpContext.Current.Request.Form["BIC"];
            if (str5 == "")
                str5 = "NULL";
            string str6 = HttpContext.Current.Request.Form["Importo"]?.ToString().Replace(".", "").Replace(",", ".");
            string str7 = HttpContext.Current.Request.Form["flexRadioTipo"].ToString();
            bool blnCommit = true;
            try
            {
                string strSQL2 = "SELECT * FROM TBRICLIQ WHERE TIPLIQ = 3 AND MAT =" + matricola + " AND CODPOS =" + str2 + " AND DALTFR ='" + str3 + "'";
                DataTable dataTable = new DataTable();
                if (dataLayer.GetDataTable(strSQL2).Rows.Count > 0)
                {
                    ErroreMSG = "Attenzione! Per questo RDL e' gia' stata inviata l'anticipazione del TFR";
                    blnCommit = false;
                }
                else
                {
                    string strSQL3 = "SELECT PRORAP FROM RAPLAV WHERE CODPOS =" + str2;
                    string str8 = dataLayer.Get1ValueFromSQL(strSQL3, CommandType.Text);
                    string strSQL4 = "SELECT VALUE(COUNT(*),0) FROM TBRICLIQ WHERE CODPOS = " + str2 + " AND MAT = " + matricola + " AND PRORAPTFR = " + str8 + " AND DALTFR = '" + str3 + "' AND TIPLIQ = 3";
                    if (dataLayer.Get1ValueFromSQL(strSQL4, CommandType.Text) == "0")
                    {
                        string strSQL5 = "SELECT VALUE(MAX(ID),0) + 1 FROM TBRICLIQ";

                        if (!IbanUtils.CheckAndInsertIban(iban, matricola, dataLayer, ref ErroreMSG, cf))
                            return;

                        var TBRICLIQQuery = "INSERT INTO TBRICLIQ (ID, MAT, CODPOS, PRORAPTFR, DALTFR, ALTFR, MODPAG, IBAN, SWIFT, TIPLIQ, CODSTAPRA, DATCONF, UTECONF, DTRIC, UTERIC, DETIMP, CODMOTANT, IMPTFR, PERCTFR) VALUES (" + dataLayer.Get1ValueFromSQL(strSQL5, CommandType.Text) + ", " + matricola + ", " + str2 + ", " + str8 + ", '" + str3 + "', " + " NULL,  " + " 1, '" + iban + "', '" + str5 + "', " + " 3, 0," + " CURRENT_DATE, '" + cf + "', " + " CURRENT_DATE, '" + cf + "', '" + detimp + "',  " + str4 + ", ";
                        string strSQL6 = (!(str7 == "N") ? TBRICLIQQuery + str6 + ", " + " NULL" : TBRICLIQQuery + " NULL, " + " 'S'") + ")";
                        if (dataLayer.WriteDataWithParameters(strSQL6, CommandType.Text, parameter))
                        {
                            SuccessMSG = "Operazione effettuata con successo.";
                            blnCommit = true;
                        }
                        else
                        {
                            ErroreMSG = "Non e' possibile eseguire la richiesta del TFR per il rapporto di lavoro selezionato.";
                            blnCommit = false;
                            dataLayer.EndTransaction(blnCommit);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErroreMSG = "Non e' possibile eseguire la richiesta del TFR per il rapporto di lavoro selezionato.";
                blnCommit = false;
            }
            dataLayer.EndTransaction(blnCommit);
        }

        public string GetMatricolaIscritto(string username) => new DataLayer().Get1ValueFromSQL("SELECT i.MAT FROM UTENTI u INNER JOIN ISCTWEB i ON u.CODFIS = i.CODFIS WHERE u.CODTIPUTE = 'I' AND u.CODUTE = '" + username + "'", CommandType.Text);

        public bool IsDocumentUploaded(string mat)
        {
            try
            {
                return !string.IsNullOrEmpty(new DataLayer().Get1ValueFromSQL("SELECT NUMERO_DOCUMENTO FROM ISCTWEB WHERE MAT = " + mat, CommandType.Text));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<string> GetTipoDocumentoList()
        {
            List<string> listTipoDoc = new List<string>();
            DataTable dataTable = this.objDataAccess.GetDataTable("SELECT DESCDOC FROM TIPODOC");
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
                    listTipoDoc.Add(row["DESCDOC"].ToString());
            }
            return listTipoDoc;
        }
        public bool SalvaDatiDocumento(string tipDoc, string numDoc, string scadDoc, string mat)
        {
            try
            {
                string str1 = DBMethods.DoublePeakForSql(tipDoc);
                string str2 = DBMethods.DoublePeakForSql(numDoc);
                string str3 = DBMethods.DoublePeakForSql(scadDoc);
                DataLayer dataLayer = new DataLayer();
                iDB2Parameter parameter1 = dataLayer.CreateParameter("@TIPO_DOCUMENTO", iDB2DbType.iDB2VarChar, 100, ParameterDirection.ReturnValue, tipDoc);
                iDB2Parameter parameter2 = dataLayer.CreateParameter("@NUMERO_DOCUMENTO", iDB2DbType.iDB2VarChar, 100, ParameterDirection.ReturnValue, numDoc);
                iDB2Parameter parameter3 = dataLayer.CreateParameter("@DATA_SCADENZA_DOCUMENTO", iDB2DbType.iDB2Date, 10, ParameterDirection.ReturnValue, scadDoc);
                string strSQL = "UPDATE ISCTWEB SET TIPO_DOCUMENTO = " + str1 + ", NUMERO_DOCUMENTO = " + str2 + ", DATA_SCADENZA_DOCUMENTO = " + str3 + " WHERE MAT =" + mat;
                return dataLayer.WriteDataWithParameters(strSQL, CommandType.Text, parameter1, parameter2, parameter3);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
