// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Iscritto.RicPagTFR_DAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.Iscritto;
using Utilities;

namespace TFI.DAL.Iscritto
{
    public class RicPagTFR_DAL
    {
        public IscrittoRicTFROCM CaricaDatiTFR(
          ref string ErrorMsg,
          ref string SuccesMsg,
          ref string InfoMSG)
        {
            bool flag = true;
            string Err = "";
            DataLayer dataLayer = new DataLayer();
            string username = ((TFI.OCM.Utente.Utente)HttpContext.Current.Session["utente"]).Username;
            iDB2Parameter parameter = dataLayer.CreateParameter("@codfis", iDB2DbType.iDB2VarChar, 30, ParameterDirection.ReturnValue, username);
            string strSQL1 = "SELECT i.COGNOME, i.NOME, i.DATNAS, i.STAESTNAS, i.DENSTAEST, i.CODDUG, i.CODFIS,i.IND, i.NUMCIV, i.CELL, i.CAP, i.DENLOC, i.SIGPRO, i.SES, i.EMAIL, i.EMAILCERT, r.TIPO_DOC, r.NUMERO_DOC, r.SCADENZA_DOC FROM ISCTWEB i LEFT JOIN TBRICLIQ r ON i.CODFIS = r.UTERIC WHERE CODFIS='" + username + "'";
            string strSQL2 = "SELECT C.DENCOM, C.SIGPRO FROM CODCOM C, ISCTWEB I WHERE I.CODCOMNAS=C.CODCOM AND I.CODFIS='" + username + "'";
            string strSQL3 = "SELECT C.DENCOM FROM CODCOM C, ISCTWEB I WHERE I.CODCOM=C.CODCOM AND I.CODFIS='" + username + "'";
            DataTable dataTable1 = new DataTable();
            DataTable dataTable2 = new DataTable();
            DataTable dataTable3 = new DataTable();
            DataTable tableWithParameters1 = dataLayer.GetDataTableWithParameters(strSQL1, parameter);
            DataTable tableWithParameters2 = dataLayer.GetDataTableWithParameters(strSQL2, parameter);
            DataTable tableWithParameters3 = dataLayer.GetDataTableWithParameters(strSQL3, parameter);
            string str1 = tableWithParameters2.Rows[0]["DENCOM"].ToString();
            string str2 = tableWithParameters2.Rows[0]["SIGPRO"].ToString();
            string str3 = tableWithParameters3.Rows.Count <= 0 ? "" : (string.IsNullOrEmpty(tableWithParameters3.Rows[0]["DENCOM"].ToString()) ? "" : tableWithParameters3.Rows[0]["DENCOM"].ToString());
            string str4 = tableWithParameters1.Rows[0]["NOME"].ToString();
            string str5 = tableWithParameters1.Rows[0]["CELL"].ToString();
            string str6 = tableWithParameters1.Rows[0]["DATNAS"].ToString();
            string str7 = tableWithParameters1.Rows[0]["STAESTNAS"].ToString();
            string str8 = tableWithParameters1.Rows[0]["DENSTAEST"].ToString();
            string str9 = tableWithParameters1.Rows[0]["EMAIL"].ToString();
            string str10 = tableWithParameters1.Rows[0]["EMAILCERT"].ToString();
            string str11 = tableWithParameters1.Rows[0]["SES"].ToString();
            string str12 = tableWithParameters1.Rows[0]["COGNOME"].ToString();
            string str13 = tableWithParameters1.Rows[0]["CODFIS"].ToString();
            string str14 = tableWithParameters1.Rows[0]["IND"].ToString();
            string str15 = tableWithParameters1.Rows[0]["NUMCIV"].ToString();
            string str16 = tableWithParameters1.Rows[0]["CAP"].ToString();
            string str17 = tableWithParameters1.Rows[0]["DENLOC"].ToString();
            string str18 = tableWithParameters1.Rows[0]["SIGPRO"].ToString();
            string str19 = tableWithParameters1.Rows[0]["CODDUG"].ToString();
            string str20 = tableWithParameters1.Rows[0]["TIPO_DOC"].ToString();
            string str21 = tableWithParameters1.Rows[0]["NUMERO_DOC"].ToString();
            string str22 = tableWithParameters1.Rows[0]["SCADENZA_DOC"].ToString();
            string strSQL4 = "SELECT DENDUG FROM DUG WHERE CODDUG=" + str19;
            DataTable dataTable4 = new DataTable();
            DataTable dataTable5 = dataLayer.GetDataTable(strSQL4);
            string str23 = dataTable5.Rows[0]["DENDUG"].ToString();
            string matricolaSQL = "SELECT MAT FROM ISCT WHERE CODFIS='" + username + "'";
            string matricola = dataLayer.GetDataTableWithParameters(matricolaSQL, parameter).Rows[0]["MAT"].ToString();
            HttpContext.Current.Items[(object)"Matricola"] = (object)matricola;
            if (tableWithParameters2.Rows.Count > 0 && tableWithParameters1.Rows.Count > 0 && dataTable5.Rows.Count > 0)
            {
                HttpContext.Current.Items[(object)"Nome"] = (object)str4;
                HttpContext.Current.Items[(object)"ComuneNascita"] = (object)str1;
                HttpContext.Current.Items[(object)"ProvinciaNascita"] = (object)str2;
                HttpContext.Current.Items[(object)"ComuneResidenza"] = (object)str3;
                HttpContext.Current.Items[(object)"Cognome"] = (object)str12;
                HttpContext.Current.Items[(object)"DataNascita"] = (object)str6.Substring(0, 10);
                HttpContext.Current.Items[(object)"Sesso"] = (object)str11;
                if (str7 != "Null")
                    HttpContext.Current.Items[(object)"StatoEsteroNascita"] = (object)str7;
                else
                    HttpContext.Current.Items[(object)"StatoEsteroNascita"] = (object)" ";
                if (str8 != "Null")
                    HttpContext.Current.Items[(object)"StatoEstero"] = (object)str8;
                else
                    HttpContext.Current.Items[(object)"StatoEstero"] = (object)" ";
                HttpContext.Current.Items[(object)"Email"] = (object)str9;
                HttpContext.Current.Items[(object)"EmailCert"] = (object)str10;
                HttpContext.Current.Items[(object)"Cellulare"] = (object)str5;
                HttpContext.Current.Items[(object)"CodiceFis"] = (object)str13;
                HttpContext.Current.Items[(object)"Indirizzo"] = (object)(str23 + " " + str14 + " " + str15);
                HttpContext.Current.Items[(object)"Cap"] = (object)str16;
                HttpContext.Current.Items[(object)"Località"] = (object)str17;
                HttpContext.Current.Items[(object)"Provincia"] = (object)str18;
                HttpContext.Current.Items[(object)"TipoDocumento"] = (object)str20;
                HttpContext.Current.Items[(object)"NumeroDocumento"] = (object)str21;
                if (!string.IsNullOrEmpty(str22))
                    HttpContext.Current.Items[(object)"ScadenzaDocumento"] = (object)Convert.ToDateTime(str22).ToString("yyyy-MM-dd");
                else
                    HttpContext.Current.Items[(object)"ScadenzaDocumento"] = (object)" ";

                string strSQL6 = "SELECT TEL1, TEL2, FAX FROM ISCD WHERE MAT='" + matricola + "'";
                DataSet dataSet1 = new DataSet();
                DataSet dataSet2 = dataLayer.GetDataSet(strSQL6, ref Err);
                try
                {
                    string str25 = dataSet2.Tables[0].Rows[0]["TEL1"].ToString();
                    string str26 = dataSet2.Tables[0].Rows[0]["TEL2"].ToString();
                    string str27 = dataSet2.Tables[0].Rows[0]["FAX"].ToString();
                    HttpContext.Current.Items[(object)"Telefono1"] = (object)str25;
                    HttpContext.Current.Items[(object)"Telefono2"] = (object)str26;
                    HttpContext.Current.Items[(object)"Fax"] = (object)str27;
                }
                catch (NullReferenceException ex)
                {
                    HttpContext.Current.Items[(object)"Telefono1"] = (object)"";
                    HttpContext.Current.Items[(object)"Telefono2"] = (object)"";
                    HttpContext.Current.Items[(object)"Fax"] = (object)"";
                    ex.StackTrace.ToString();
                }
            }
            string strQuery1 = $@"SELECT CODPOS, TRIM(CODPOS || ' - ' || (SELECT TRIM(RAGSOC) FROM AZI WHERE CODPOS = A.CODPOS)) AS POSIZIONE, DATDEC, DATCES 
                                FROM RAPLAV A WHERE MAT = {matricola} AND CODPOS || MAT || PRORAP NOT IN(SELECT CODPOS || MAT || PRORAPTFR FROM TBRICLIQ WHERE TIPLIQ = 1 AND MAT = A.MAT) AND DATCES IS NOT NULL AND DTTFR IS NOT NULL";
            iDB2DataReader dataReaderFromQuery1 = dataLayer.GetDataReaderFromQuery(strQuery1, CommandType.Text);
            List<string> stringList = new List<string>();
            try
            {
                while (dataReaderFromQuery1.Read())
                    stringList.Add(dataReaderFromQuery1["DATDEC"].ToString().Substring(0, 10) + " - " + dataReaderFromQuery1["DATCES"].ToString().Substring(0, 10) + " | " + dataReaderFromQuery1["POSIZIONE"].ToString());
                if (stringList.Count > 0)
                {
                    HttpContext.Current.Items[(object)"ListaRDL"] = (object)stringList;
                }
                else
                {
                    HttpContext.Current.Items[(object)"ListaRDL"] = (object)"NESSUN RAPPORTO DI LAVORO DISPONIBILE";
                    InfoMSG = "Non e' possibile richiedere il pagamento del TFR. Non risulta nessun rapporto di lavoro cessato!";
                    flag = false;
                }
            }
            catch (NullReferenceException ex)
            {
                ex.StackTrace.ToString();
            }
            IscrittoRicTFROCM iscrittoRicTfrocm = new IscrittoRicTFROCM();
            iscrittoRicTfrocm.AbilitaBtn = flag;
            List<IscrittoRicTFROCM.RichiestaLiquidazione> richiestaLiquidazioneList = new List<IscrittoRicTFROCM.RichiestaLiquidazione>();
            string strQuery2 = "SELECT l.DATLIQ, t.ID, t.CODPOS, t.DTRIC, t.IMPTFR, t.TIPLIQ, a.RAGSOC, t.NUMERO_DOC FROM TBRICLIQ t INNER JOIN AZI a ON t.CODPOS = a.CODPOS LEFT JOIN LIQUITFR l ON l.CODPOS = t.CODPOS AND l.MAT = '" + HttpContext.Current.Items[(object)"Matricola"]?.ToString() + "'  WHERE t.MAT ='" + HttpContext.Current.Items[(object)"Matricola"]?.ToString() + "' AND t.TIPLIQ = '1'";
            iDB2DataReader dataReaderFromQuery2 = dataLayer.GetDataReaderFromQuery(strQuery2, CommandType.Text);
            while (dataReaderFromQuery2.Read())
                richiestaLiquidazioneList.Add(new IscrittoRicTFROCM.RichiestaLiquidazione()
                {
                    ID = !DBNull.Value.Equals(dataReaderFromQuery2["ID"]) ? Convert.ToInt32(dataReaderFromQuery2["ID"]) : 0,
                    CODPOS = !DBNull.Value.Equals(dataReaderFromQuery2["CODPOS"]) ? Convert.ToInt32(dataReaderFromQuery2["CODPOS"]) : 0,
                    DTRIC = !DBNull.Value.Equals(dataReaderFromQuery2["DTRIC"]) ? dataReaderFromQuery2["DTRIC"].ToString() : "Dato non disponibile",
                    IMPTFR = !DBNull.Value.Equals(dataReaderFromQuery2["IMPTFR"]) ? Convert.ToDecimal(dataReaderFromQuery2["IMPTFR"]) : 0M,
                    TIPLIQ = !DBNull.Value.Equals(dataReaderFromQuery2["TIPLIQ"]) ? Convert.ToInt32(dataReaderFromQuery2["TIPLIQ"]) : 0,
                    RAGSOC = !DBNull.Value.Equals(dataReaderFromQuery2["RAGSOC"]) ? dataReaderFromQuery2["RAGSOC"].ToString() : "Dato non disponibile",
                    Liquidato = !DBNull.Value.Equals(dataReaderFromQuery2["DATLIQ"]),
                    DocumentoCaricato = !DBNull.Value.Equals(dataReaderFromQuery2["NUMERO_DOC"])
                });
            int count = richiestaLiquidazioneList.Count;
            iscrittoRicTfrocm.listTFR = richiestaLiquidazioneList;
            return iscrittoRicTfrocm;
        }


        public bool CheckTfr(int matricola)
        {
            DataLayer dataLayer = new DataLayer();
            string strQuery1 = $@"SELECT CODPOS, TRIM(CODPOS || ' - ' || (SELECT TRIM(RAGSOC) FROM AZI WHERE CODPOS = A.CODPOS)) AS POSIZIONE, DATDEC, DATCES 
                                FROM RAPLAV A WHERE MAT = {matricola} AND CODPOS || MAT || PRORAP NOT IN(SELECT CODPOS || MAT || PRORAPTFR FROM TBRICLIQ WHERE TIPLIQ = 1 AND MAT = A.MAT) AND DATCES IS NOT NULL AND DTTFR IS NOT NULL";
            iDB2DataReader dataReaderFromQuery1 = dataLayer.GetDataReaderFromQuery(strQuery1, CommandType.Text);
            List<string> stringList = new List<string>();
            try
            {
                while (dataReaderFromQuery1.Read())
                    stringList.Add(dataReaderFromQuery1["DATDEC"].ToString().Substring(0, 10) + " - " + dataReaderFromQuery1["DATCES"].ToString().Substring(0, 10) + " | " + dataReaderFromQuery1["POSIZIONE"].ToString());
                if (stringList.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (NullReferenceException ex)
            {
                ex.StackTrace.ToString();
                return false;
            }
        }
        public void InvioRichiestaTFR(IscrittoRicTFROCM modelloForm, ref string ErrorMsg, ref string SuccesMsg)
        {
            TFI.OCM.Utente.Utente utente = (TFI.OCM.Utente.Utente)HttpContext.Current.Session["utente"];
            HttpContext.Current.Items[(object)"VisibilityShaker"] = (object)" style = 'visibility:hidden;' ";
            DataLayer dataLayer = new DataLayer();
            dataLayer.StartTransaction();

            string Err = "";
            string parameterValue1 = modelloForm.CheckFruizione;
            string parameterValue2 = modelloForm.TipoPagamento;
            iDB2Parameter parameter1 = dataLayer.CreateParameter("@check", iDB2DbType.iDB2VarChar, 30, ParameterDirection.ReturnValue, parameterValue1);
            iDB2Parameter parameter2 = dataLayer.CreateParameter("@tipoPag", iDB2DbType.iDB2VarChar, 30, ParameterDirection.ReturnValue, parameterValue2);
            string username = utente.Username;
            iDB2Parameter parameter3 = dataLayer.CreateParameter("@codfis", iDB2DbType.iDB2VarChar, 30, ParameterDirection.ReturnValue, username);
            string strSQL1 = "SELECT MAT FROM ISCTWEB WHERE CODFIS='" + username + "'";
            DataTable dataTable1 = new DataTable();
            string str1 = dataLayer.GetDataTableWithParameters(strSQL1, parameter3).Rows[0]["MAT"].ToString();
            string str2 = HttpContext.Current.Request.Form["ddlPosizione"].ToString().Substring(0, 23);
            string str3 = Convert.ToDateTime(str2.Substring(0, 10)).ToString("yyyy-MM-dd");
            string str4 = Convert.ToDateTime(str2.Substring(13)).ToString("yyyy-MM-dd");
            iDB2Parameter parameter4 = dataLayer.CreateParameter("@dataInizio", iDB2DbType.iDB2Date, 30, ParameterDirection.ReturnValue, str3.ToString());
            iDB2Parameter parameter5 = dataLayer.CreateParameter("@dataFine", iDB2DbType.iDB2Date, 30, ParameterDirection.ReturnValue, str4.ToString());
            string parameterValue3 = HttpContext.Current.Request.Form["ddlPosizione"].ToString().Substring(26, 6);
            iDB2Parameter parameter6 = dataLayer.CreateParameter("@codicePosizione", iDB2DbType.iDB2Decimal, 30, ParameterDirection.ReturnValue, parameterValue3);
            iDB2Command objCommand1 = dataLayer.objCommand;
            string[] strArray1 = new string[9]
            {
        "SELECT * FROM TBRICLIQ WHERE TIPLIQ = 1 AND MAT = '",
        str1,
        "' AND CODPOS =",
        parameterValue3,
        " AND DALTFR='",
        str3.ToString(),
        "' AND ALTFR='",
        str4.ToString(),
        "'"
            };
            string str5;
            string str6 = str5 = string.Concat(strArray1);
            objCommand1.CommandText = str5;
            string strSQL2 = str6;
            DataTable dataTable2 = new DataTable();
            DataTable tableWithParameters1 = dataLayer.GetDataTableWithParameters(strSQL2, parameter6, parameter4, parameter5);
            string strSQL3 = "SELECT PRORAP FROM RAPLAV WHERE MAT='" + str1 + "' AND CODPOS=" + parameterValue3 + " AND DATDEC='" + str3.ToString() + "' AND DATCES='" + str4.ToString() + "'";
            DataTable dataTable3 = new DataTable();
            string parameterValue4 = dataLayer.GetDataTableWithParameters(strSQL3, parameter3, parameter6, parameter5, parameter4).Rows[0]["PRORAP"].ToString();
            dataLayer.CreateParameter("@prorap", iDB2DbType.iDB2Decimal, 30, ParameterDirection.ReturnValue, parameterValue4);
            bool blnCommit;
            if (tableWithParameters1.Rows.Count > 0)
            {
                ErrorMsg = "Per questo RDL risulta attualmente una richiesta di pagamento del TFR effettuata";
                blnCommit = false;
            }
            else
            {
                try
                {
                    string iban;
                    string parameterValue5;
                    if (parameterValue2 == "1")
                    {
                        iban = HttpContext.Current.Request.Form["IBAN"].ToString();
                        parameterValue5 = HttpContext.Current.Request.Form["BICSWIFT"].ToString() == ""
                            ? "NULL" : HttpContext.Current.Request.Form["BICSWIFT"].ToString();
                    }
                    else
                    {
                        iban = "";
                        parameterValue5 = "";
                    }
                    iDB2Parameter parameter7 = dataLayer.CreateParameter("@iban", iDB2DbType.iDB2VarChar, 50, ParameterDirection.Input, iban);
                    iDB2Parameter parameter8 = dataLayer.CreateParameter("@bicswift", iDB2DbType.iDB2VarChar, 50, ParameterDirection.Input, parameterValue5);
                    iDB2Command objCommand2 = dataLayer.objCommand;
                    string[] strArray2 = new string[9]
                    {
            "SELECT VALUE(COUNT(*), 0) AS VALORE FROM TBRICLIQ WHERE CODPOS = ",
            parameterValue3,
            " AND MAT ='",
            str1,
            "' AND TIPLIQ=1 AND DALTFR='",
            str3.ToString(),
            "' AND ALTFR='",
            str4.ToString(),
            "' AND PRORAPTFR=1"
                    };
                    string str8;
                    string str9 = str8 = string.Concat(strArray2);
                    objCommand2.CommandText = str8;
                    string strSQL4 = str9;
                    DataTable dataTable4 = new DataTable();
                    if (dataLayer.GetDataTableWithParameters(strSQL4, parameter6, parameter4, parameter5).Rows[0]["VALORE"].ToString() == "0")
                    {
                        string strSQL5 = "SELECT VALUE(MAX(ID),0) +1 AS ID FROM TBRICLIQ";
                        DataSet dataSet1 = new DataSet();
                        string parameterValue6 = dataLayer.GetDataSet(strSQL5, ref Err).Tables[0].Rows[0]["ID"].ToString();
                        iDB2Parameter parameter9 = dataLayer.CreateParameter("@id", iDB2DbType.iDB2Decimal, 30, ParameterDirection.ReturnValue, parameterValue6);
                        dataLayer.CreateParameter("@TIPO_DOCUMENTO", iDB2DbType.iDB2VarChar, 50, ParameterDirection.ReturnValue, HttpContext.Current.Request.Form["tipoDocumento"].ToString());
                        dataLayer.CreateParameter("@NUMERO_DOCUMENTO", iDB2DbType.iDB2VarChar, 50, ParameterDirection.ReturnValue, HttpContext.Current.Request.Form["numeroDocumento"].ToString());
                        dataLayer.CreateParameter("@DATA_SCADENZA_DOCUMENTO", iDB2DbType.iDB2Date, 10, ParameterDirection.ReturnValue, HttpContext.Current.Request.Form["scadenzaDocumento"].ToString());


                        iDB2Parameter idParam = dataLayer.CreateParameter("@id", iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input, parameterValue6);
                        iDB2Parameter matricolaParam = dataLayer.CreateParameter("@matricola", iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, str1);
                        iDB2Parameter codposParam = dataLayer.CreateParameter("@codpos", iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, parameterValue3);
                        iDB2Parameter proraptfrParam = dataLayer.CreateParameter("@proraptfr", iDB2DbType.iDB2Decimal, 3, ParameterDirection.Input, parameterValue4);
                        iDB2Parameter daltfrParam = dataLayer.CreateParameter("@daltfr", iDB2DbType.iDB2Date, 30, ParameterDirection.Input, str3);
                        iDB2Parameter altfrParam = dataLayer.CreateParameter("@altfr", iDB2DbType.iDB2Date, 30, ParameterDirection.Input, str4);
                        iDB2Parameter ibanParam = dataLayer.CreateParameter("@iban", iDB2DbType.iDB2VarChar, 50, ParameterDirection.Input, iban);
                        iDB2Parameter swiftParam = dataLayer.CreateParameter("@swift", iDB2DbType.iDB2VarChar, 50, ParameterDirection.Input, parameterValue5);
                        iDB2Parameter userParam = dataLayer.CreateParameter("@user", iDB2DbType.iDB2VarChar, 20, ParameterDirection.Input, username);
                        iDB2Parameter modpaParam = dataLayer.CreateParameter("@modpa", iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input, "");
                        iDB2Parameter detimpParam = dataLayer.CreateParameter("@detimp", iDB2DbType.iDB2Char, 1, ParameterDirection.Input, "");


                        var TBRICLIQQuery = "INSERT INTO TBRICLIQ " +
                                "(ID, MAT, MODPAG, TIPLIQ, UTERIC, CODPOS, PRORAPTFR, DALTFR, ALTFR, IBAN, SWIFT, CODSTAPRA, DATCONF," +
                                " UTECONF, DTRIC, DETIMP)" +
                                " VALUES " +
                                "(@id, @mat, @modpa, 1, @user, @codpos, @proraptfr, @daltfr, @altfr, @iban, @swift, 0, CURRENT_DATE," +
                                " @user, CURRENT_DATE, @detimp)";

                        if (parameterValue2 == "1")
                        {
                            modpaParam = dataLayer.CreateParameter("@modpa", iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input, "1");
                            ibanParam = dataLayer.CreateParameter("@iban", iDB2DbType.iDB2VarChar, 50, ParameterDirection.Input, iban);
                            swiftParam = dataLayer.CreateParameter("@swift", iDB2DbType.iDB2VarChar, 50, ParameterDirection.Input, parameterValue5);
                        }
                        else if (parameterValue2 == "2")
                        {
                            modpaParam = dataLayer.CreateParameter("@modpa", iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input, "2");
                            ibanParam = dataLayer.CreateParameter("@iban", iDB2DbType.iDB2VarChar, 50, ParameterDirection.Input, "NULL");
                            swiftParam = dataLayer.CreateParameter("@swift", iDB2DbType.iDB2VarChar, 50, ParameterDirection.Input, "NULL");
                        }
                        if (parameterValue1 == "S")
                        {
                            detimpParam = dataLayer.CreateParameter("@detimp", iDB2DbType.iDB2Char, 1, ParameterDirection.Input, "S");
                        }
                        else if (parameterValue1 == "N")
                        {
                            detimpParam = dataLayer.CreateParameter("@detimp", iDB2DbType.iDB2Char, 1, ParameterDirection.Input, "N");
                        }



                        //string strSQL7 = "SELECT * FROM ISCBANC WHERE MAT='" + str1 + "' AND IBAN='" + iban + "'";
                        //DataTable dataTable5 = new DataTable();
                        //DataTable tableWithParameters2 = dataLayer.GetDataTableWithParameters(strSQL7, parameter7);

                        if (parameterValue2 == "1")
                        {

                            //string strSQL8 = "SELECT VALUE(MAX(PROISCBAN), 0) + 1 FROM ISCBANC WHERE MAT=" + str1;
                            //string parameterValue7 = dataLayer.Get1ValueFromSQL(strSQL8, CommandType.Text);
                            //iDB2Parameter parameter10 = dataLayer.CreateParameter("@PROISCBAN", iDB2DbType.iDB2Decimal, 30, ParameterDirection.ReturnValue, parameterValue7);
                            //string strSQL9 = dataLayer.objCommand.CommandText = "SELECT NOM,COG FROM ISCT WHERE MAT=" + str1;
                            //DataSet dataSet2 = new DataSet();
                            //DataSet dataSet3 = dataLayer.GetDataSet(strSQL9, ref Err);
                            //string parameterValue8 = dataSet3.Tables[0].Rows[0]["NOME"].ToString();
                            //string parameterValue9 = dataSet3.Tables[0].Rows[0]["COGNOME"].ToString();
                            //iDB2Parameter parameter11 = dataLayer.CreateParameter("@nome", iDB2DbType.iDB2VarChar, 30, ParameterDirection.ReturnValue, parameterValue8);
                            //iDB2Parameter parameter12 = dataLayer.CreateParameter("@cognome", iDB2DbType.iDB2VarChar, 30, ParameterDirection.ReturnValue, parameterValue9);
                            //string strSQL10 = "INSERT INTO ISCBANC(MAT, INTEST, DATINI, DATFIN, MODPAG, PAESE, DIVISA,  ABI," + "CAB, CCERE, CODCIN, CODSWIFT, IBAN, KEYBANC, PROISCBAN, FLGINS, ULTAGG, UTEAGG) " + "VALUES('" + str1 + "', '" + parameterValue9 + " " + parameterValue8 + "', '" + DateTime.Now.ToString("yyyy-MM-dd").Substring(0, 10) + "' , " + "'9999-12-31', " + "'B', '" + iban.ToString().Substring(0, 2) + "', " + "'EUR', '" + iban.ToString().Substring(5, 5) + "', '" + iban.ToString().Substring(10, 5) + "', '" + iban.ToString().Substring(15) + "', '" + iban.ToString().Substring(4, 1) + "', '" + iban.ToString() + "','" + iban + "', '" + iban.ToString().Substring(5, 10) + "', " + parameterValue7 + ", " + " 'W', '" + DateTime.Now.ToString("yyyy-MM-dd").Substring(0, 10) + "', '" + username + "') ";

                            if (dataLayer.WriteTransactionDataWithParametersAndDontCall(TBRICLIQQuery, CommandType.Text, idParam, matricolaParam, modpaParam, userParam, codposParam, proraptfrParam, daltfrParam, altfrParam, ibanParam, swiftParam, userParam, detimpParam))
                            {
                                if (!IbanUtils.CheckAndInsertIban(iban, str1, dataLayer, ref ErrorMsg, username))
                                    return;
                                dataLayer.EndTransaction(true);
                                return;
                            }
                            ErrorMsg = "Si sono verificati problemi durante il salvataggio della richiesta.";
                            blnCommit = false;
                        }
                        else if (parameterValue2 == "2")
                        {
                            if (dataLayer.WriteTransactionDataWithParametersAndDontCall(TBRICLIQQuery, CommandType.Text, idParam, matricolaParam, modpaParam, userParam, codposParam, proraptfrParam, daltfrParam, altfrParam, ibanParam, swiftParam, userParam, detimpParam))
                            {
                                SuccesMsg = "Operazione effettuata con successo.";
                                dataLayer.EndTransaction(true);
                                return;
                            }
                            else
                            {
                                ErrorMsg = "Si sono verificati problemi durante il salvataggio della richiesta.";
                                dataLayer.EndTransaction(false);
                                return;
                            }
                        }
                        else
                        {
                            dataLayer.EndTransaction(false);
                            return;
                        }
                    }
                    else
                    {
                        ErrorMsg = "Non e' possibile eseguire la richiesta del TFR per il rapporto di lavoro selezionato.";
                        blnCommit = false;
                        dataLayer.EndTransaction(blnCommit);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.Current.Session["LastException"] = (object)ex;
                    ErrorMsg = "Non e' possibile eseguire la richiesta del TFR per il rapporto di lavoro selezionato.";
                    blnCommit = false;
                    dataLayer.EndTransaction(blnCommit);
                    return;
                }
            }
        }

        public static bool CheckIban(string Iban)
        {
            if (string.IsNullOrEmpty(Iban))
                return false;
            Iban = Iban.Replace(" ", string.Empty).ToUpper();
            if (!Regex.IsMatch(Iban, "^[a-zA-Z]{2}[0-9]{2}[a-zA-Z0-9]{4}[0-9]{7}([a-zA-Z0-9]?){0,16}$"))
                return false;
            Iban = Iban.Substring(4) + Iban.Substring(0, 4);
            int num = 0;
            foreach (char c in Iban)
                num = !char.IsLetter(c) ? (10 * num + ((int)c - 48)) % 97 : (100 * num + ((int)c - 55)) % 97;
            return num == 1;
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

        public bool SalvaDatiDocumento(string tipDoc, string numDoc, string scadDoc, string mat)
        {
            try
            {
                string str1 = DBMethods.DoublePeakForSql(tipDoc);
                string str2 = DBMethods.DoublePeakForSql(numDoc);
                string str3 = DBMethods.DoublePeakForSql(scadDoc);
                DataLayer dataLayer = new DataLayer();
                iDB2Parameter parameter1 = dataLayer.CreateParameter("@TIPO_DOCUMENTO", iDB2DbType.iDB2VarChar, 50, ParameterDirection.ReturnValue, tipDoc);
                iDB2Parameter parameter2 = dataLayer.CreateParameter("@NUMERO_DOCUMENTO", iDB2DbType.iDB2VarChar, 50, ParameterDirection.ReturnValue, numDoc);
                iDB2Parameter parameter3 = dataLayer.CreateParameter("@DATA_SCADENZA_DOCUMENTO", iDB2DbType.iDB2Date, 10, ParameterDirection.ReturnValue, scadDoc);
                string strSQL = "UPDATE TBRICLIQ SET TIPO_DOC = " + str1 + ", NUMERO_DOC = " + str2 + ", SCADENZA_DOC = " + str3 + " WHERE MAT =" + mat;
                return dataLayer.WriteDataWithParameters(strSQL, CommandType.Text, parameter1, parameter2, parameter3);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
