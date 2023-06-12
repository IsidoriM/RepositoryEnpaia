// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Iscritto.FondoContoIndDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.Iscritto;
using Utilities;

namespace TFI.DAL.Iscritto
{
    public class FondoContoIndDAL
    {
        private readonly DataLayer objDataAccess = new DataLayer();

        public List<IscrittoRicTFROCM.RichiestaLiquidazione> GetListaRichiestaLiquidazione(
          int mat)
        {
            List<IscrittoRicTFROCM.RichiestaLiquidazione> richiestaLiquidazione = new List<IscrittoRicTFROCM.RichiestaLiquidazione>();
            DataTable dataTable = this.objDataAccess.GetDataTable(string.Format("SELECT t.DATFINRDL, t.DTRIC, t.IMPTFR FROM TBRICLIQ t WHERE MAT = '{0}' AND TIPLIQ = '2' ORDER BY t.DATFINRDL LIMIT 1", (object)mat));
            if (dataTable.Rows.Count <= 0)
                return (List<IscrittoRicTFROCM.RichiestaLiquidazione>)null;
            foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
                richiestaLiquidazione.Add(new IscrittoRicTFROCM.RichiestaLiquidazione()
                {
                    DATFINRDL = !DBNull.Value.Equals(row["DATFINRDL"]) ? row["DATFINRDL"].ToString() : "Dato non disponibile",
                    DTRIC = !DBNull.Value.Equals(row["DTRIC"]) ? row["DTRIC"].ToString() : "Dato non disponibile",
                    IMPTFR = !DBNull.Value.Equals(row["IMPTFR"]) ? Convert.ToDecimal(row["IMPTFR"]) : 0M
                });
            return richiestaLiquidazione;
        }

        public Anagrafica GetFondoContoInd(string cf)
        {
            string strSQL1 = "SELECT DISTINCT C.SIGPRO,C.DENCOM,T.DENTITSTU,I.MAT,I.COGNOME,I.NOME,I.DATNAS,I.SES,I.STAESTNAS,I.IND,I.NUMCIV,I.CAP,I.EMAIL,I.EMAILCERT,I.CELL,I.DENLOC,I.SIGPRO,I.DENSTAEST,D.DENDUG, I.TIPO_DOCUMENTO, I.NUMERO_DOCUMENTO,I.DATA_SCADENZA_DOCUMENTO FROM  CODCOM C,  ISCTWEB I,  TITSTU T,  DUG D  WHERE  I.CODFIS= '" + cf + "' AND I.CODDUG=D.CODDUG  AND I.CODCOMNAS=C.CODCOM  AND I.CODTITSTU=T.CODTITSTU ";
            string strSQL2 = "SELECT DISTINCT C.SIGPRO,C.DENCOM FROM  CODCOM C,  ISCTWEB I  WHERE  I.CODCOMNAS=C.CODCOM  AND I.CODFIS= '" + cf + "' ";
            DataTable dataTable1 = new DataTable();
            DataTable dataTable2 = new DataTable();
            DataTable dataTable3 = this.objDataAccess.GetDataTable(strSQL1);
            DataTable dataTable4 = this.objDataAccess.GetDataTable(strSQL2);
            string str1 = dataTable3.Rows[0]["STAESTNAS"].ToString();
            string empty1 = string.Empty;
            string empty2 = string.Empty;
            string str2;
            string str3;
            if (string.IsNullOrEmpty(str1))
            {
                str2 = dataTable4.Rows[0]["DENCOM"].ToString();
                str3 = dataTable4.Rows[0]["SIGPRO"].ToString();
            }
            else
            {
                string strSQL3 = "SELECT C.SIGPRO, C.DENCOM  FROM  COM_ESTERO C, ISCTWEB I  WHERE  I.CODCOMNAS=C.CODCOM  AND I.CODFIS='" + cf + "' ";
                DataTable dataTable5 = new DataTable();
                DataTable dataTable6 = this.objDataAccess.GetDataTable(strSQL3);
                str2 = dataTable6.Rows[0]["DENCOM"].ToString();
                str3 = dataTable6.Rows[0]["SIGPRO"].ToString();
            }
            Decimal num = Convert.ToDecimal(dataTable3.Rows[0]["MAT"]);
            DataTable dt = new DataTable();
            string str4;
            string str5;
            string str6;
            try
            {
                dt = this.objDataAccess.GetDataTable("SELECT DISTINCT TEL1,TEL2,FAX FROM  ISCD  WHERE  MAT='" + num.ToString() + "' ");
                str4 = dt.Rows[0]["TEL1"].ToString();
                str5 = dt.Rows[0]["TEL2"].ToString();
                str6 = dt.Rows[0]["FAX"].ToString();
            }
            catch (Exception ex)
            {
                str4 = "";
                str5 = "";
                str6 = "";
            }
            string str7;
            string str8;
            if (dataTable3.Rows[0]["DENSTAEST"].ToString() == "")
            {
                str7 = dataTable4.Rows[0]["DENCOM"].ToString();
                str8 = dataTable4.Rows[0]["SIGPRO"].ToString();
            }
            else
            {
                string strSQL4 = "SELECT C.DENCOM,C.SIGPRO FROM COM_ESTERO C,ISCTWEB I WHERE C.CODCOM=I.CODCOM";
                DataTable dataTable7 = new DataTable();
                DataTable dataTable8 = this.objDataAccess.GetDataTable(strSQL4);
                str7 = dataTable8.Rows[0]["DENCOM"].ToString();
                str8 = dataTable8.Rows[0]["SIGPRO"].ToString();
            }
            Anagrafica fondoContoInd = (Anagrafica)null;
            DataLayer dataLayer = new DataLayer();
            if (dataLayer.queryOk(dataTable3) && dataLayer.queryOk(dataTable4) && dataLayer.queryOk(dt))
            {
                fondoContoInd = new Anagrafica()
                {
                    Mat = num,
                    Nome = dataTable3.Rows[0]["NOME"].ToString(),
                    Cognome = dataTable3.Rows[0]["COGNOME"].ToString(),
                    CodiceFiscale = cf,
                    DataNascita = Convert.ToDateTime(dataTable3.Rows[0]["DATNAS"]),
                    Sesso = dataTable3.Rows[0]["SES"].ToString(),
                    TitoloStudio = dataTable3.Rows[0]["DENTITSTU"].ToString(),
                    Indirizzo = dataTable3.Rows[0]["IND"].ToString(),
                    NumeroCivico = dataTable3.Rows[0]["NUMCIV"].ToString(),
                    Cap = dataTable3.Rows[0]["CAP"].ToString(),
                    Localita = dataTable3.Rows[0]["DENLOC"].ToString(),
                    SigproResidenza = str8,
                    ComuneResidenza = str7,
                    Email = dataTable3.Rows[0]["EMAIL"].ToString(),
                    EmailCert = dataTable3.Rows[0]["EMAILCERT"].ToString(),
                    Cellulare = dataTable3.Rows[0]["CELL"].ToString(),
                    StatoEsteroResidenza = dataTable3.Rows[0]["DENSTAEST"].ToString(),
                    ComuneNascita = str2,
                    SigproNascita = str3,
                    StatoEsteroNascita = str1,
                    Telefono1 = str4,
                    Telefono2 = str5,
                    Fax = str6,
                    TipoResidenza = dataTable3.Rows[0]["DENDUG"].ToString(),
                    Numero_Documento = dataTable3.Rows[0]["NUMERO_DOCUMENTO"].ToString(),
                    Tipo_Documento = dataTable3.Rows[0]["TIPO_DOCUMENTO"].ToString(),
                    Scadenza_Documento = dataTable3.Rows[0]["DATA_SCADENZA_DOCUMENTO"].ToString()
                };
                HttpContext.Current.Items[(object)"NumeroDocumento"] = (object)dataTable3.Rows[0]["NUMERO_DOCUMENTO"].ToString();
                HttpContext.Current.Items[(object)"TipoDocumento"] = (object)dataTable3.Rows[0]["TIPO_DOCUMENTO"].ToString();
                HttpContext.Current.Items[(object)"ScadenzaDocumento"] = (object)dataTable3.Rows[0]["DATA_SCADENZA_DOCUMENTO"].ToString();
                if (!string.IsNullOrEmpty(dataTable3.Rows[0]["DATA_SCADENZA_DOCUMENTO"].ToString()))
                    HttpContext.Current.Items[(object)"ScadenzaDocumento"] = (object)Convert.ToDateTime(dataTable3.Rows[0]["DATA_SCADENZA_DOCUMENTO"].ToString()).ToString("yyyy-MM-dd");
                else
                    HttpContext.Current.Items[(object)"ScadenzaDocumento"] = (object)" ";
            }
            string str9 = "";
            string str10 = "";
            try
            {
                string strSQL5 = string.Format(" SELECT DTRIC,UTERIC FROM TBRICLIQ t WHERE TIPLIQ = 2 AND MAT = {0}", (object)fondoContoInd.Mat);
                DataTable dataTable9 = new DataTable();
                DataTable dataTable10 = this.objDataAccess.GetDataTable(strSQL5);
                if (dataTable10.Rows.Count > 0)
                {
                    str9 = dataTable10.Rows[0]["UTERIC"].ToString();
                    str10 = dataTable10.Rows[0]["DTRIC"].ToString().Substring(0, 10);
                }
            }
            catch (Exception ex)
            {
            }
            string str11 = fondoContoInd.DataNascita.AddYears(65).ToString();
            if (Convert.ToDateTime(str11) <= DateTime.Now)
            {
                Decimal mat = fondoContoInd.Mat;
                string strSQL6 = "SELECT MAX(DATCES) AS DATCES FROM RAPLAV A WHERE MAT ='" + mat.ToString() + "' AND DATCES IS NOT NULL AND DTFP IS NOT NULL AND MAT NOT IN (SELECT MAT FROM TBRICLIQ WHERE TIPLIQ = 2 AND MAT = A.MAT AND DATFINRDL = A.DATCES)";
                DataTable dataTable11 = new DataTable();
                DataTable dataTable12 = this.objDataAccess.GetDataTable(strSQL6);
                try
                {
                    DateTime dateTime1 = Convert.ToDateTime(dataTable12.Rows[0]["DATCES"].ToString());
                    string str12 = dateTime1.ToString("yyyy-MM-dd");
                    if (dataTable12.Rows[0].ToString().Substring(0, 10) != "" && dataTable12.Rows.Count > 0)
                    {
                        mat = fondoContoInd.Mat;
                        string strSQL7 = "SELECT count(*) AS EXP1 FROM RAPLAV A WHERE MAT = '" + mat.ToString() + "' AND CURRENT_DATE BETWEEN DATDEC AND VALUE(DATCES, '9999-12-31')";
                        DataTable dataTable13 = new DataTable();
                        if (this.objDataAccess.GetDataTable(strSQL7).Rows[0]["EXP1"].ToString() == "0")
                        {
                            string[] strArray = new string[5]
                            {
                "SELECT * FROM TBRICLIQ WHERE TIPLIQ = 2 AND MAT = '",
                null,
                null,
                null,
                null
                            };
                            mat = fondoContoInd.Mat;
                            strArray[1] = mat.ToString();
                            strArray[2] = "' AND DATFINRDL >= '";
                            strArray[3] = str12;
                            strArray[4] = "'";
                            string strSQL8 = string.Concat(strArray);
                            DataTable dataTable14 = new DataTable();
                            if (this.objDataAccess.GetDataTable(strSQL8).Rows.Count > 0)
                            {
                                HttpContext.Current.Items[(object)"WarningMessage"] = (object)("E' stata effettuata la richiesta di pagamento del fondo in data " + str10 + " dall'utente " + str9);
                            }
                            else
                            {
                                fondoContoInd.DataFineRapp = dateTime1.ToString().Substring(0, 10);
                                HttpContext.Current.Session["DataRapp"] = HttpContext.Current.Items[(object)"DataRapp"];
                            }
                        }
                        else
                        {
                            string[] strArray = new string[5]
                            {
                "SELECT * FROM TBRICLIQ WHERE TIPLIQ = 2 AND MAT = '",
                null,
                null,
                null,
                null
                            };
                            mat = fondoContoInd.Mat;
                            strArray[1] = mat.ToString();
                            strArray[2] = "' AND DATFINRDL > '";
                            DateTime dateTime2 = Convert.ToDateTime(str11);
                            dateTime2 = dateTime2.AddDays(-1.0);
                            strArray[3] = dateTime2.ToString();
                            strArray[4] = "'";
                            string strSQL9 = string.Concat(strArray);
                            DataTable dataTable15 = new DataTable();
                            if (this.objDataAccess.GetDataTable(strSQL9).Rows.Count > 0)
                            {
                                HttpContext.Current.Items[(object)"WarningMessage"] = (object)("E' stata effettuata la richiesta di pagamento del fondo in data " + str10 + " dall'utente " + str9);
                            }
                            else
                            {
                                dateTime2 = Convert.ToDateTime(str11);
                                dateTime2 = dateTime2.AddDays(-1.0);
                                string str13 = dateTime2.ToString().Substring(0, 10);
                                fondoContoInd.DataFineRapp = str13;
                                HttpContext.Current.Session["DataRapp"] = (object)fondoContoInd.DataFineRapp;
                            }
                        }
                    }
                    else
                    {
                        string[] strArray = new string[5]
                        {
              "SELECT * FROM TBRICLIQ WHERE TIPLIQ = 2 AND MAT = '",
              null,
              null,
              null,
              null
                        };
                        mat = fondoContoInd.Mat;
                        strArray[1] = mat.ToString();
                        strArray[2] = "' AND DATFINRDL > '";
                        DateTime dateTime3 = Convert.ToDateTime(str11);
                        dateTime3 = dateTime3.AddDays(-1.0);
                        strArray[3] = dateTime3.ToString();
                        strArray[4] = "'";
                        string strSQL10 = string.Concat(strArray);
                        DataTable dataTable16 = new DataTable();
                        if (this.objDataAccess.GetDataTable(strSQL10).Rows.Count > 0)
                        {
                            HttpContext.Current.Items[(object)"WarningMessage"] = (object)("E' stata effettuata la richiesta di pagamento del fondo in data " + str10 + " dall'utente " + str9);
                        }
                        else
                        {
                            dateTime3 = Convert.ToDateTime(str11);
                            dateTime3 = dateTime3.AddDays(-1.0);
                            string str14 = dateTime3.ToString().Substring(0, 10);
                            fondoContoInd.DataFineRapp = str14;
                            HttpContext.Current.Session["DataRapp"] = (object)fondoContoInd.DataFineRapp;
                        }
                    }
                }
                catch (Exception ex)
                {
                    fondoContoInd.DataFineRapp = "Nessuna Data";
                    HttpContext.Current.Items[(object)"WarningMessage"] = (object)"Nessuna data fine rapporto trovata";
                }
            }
            else
            {
                Decimal mat = fondoContoInd.Mat;
                string strSQL11 = "SELECT MAX(DATCES) AS DATCES FROM RAPLAV A WHERE MAT ='" + mat.ToString() + "' AND DATCES IS NOT NULL AND DTFP IS NOT NULL AND MAT NOT IN (SELECT MAT FROM TBRICLIQ WHERE TIPLIQ = 2 AND MAT = A.MAT AND DATFINRDL = A.DATCES)";
                DataTable dataTable17 = new DataTable();
                DataTable dataTable18 = this.objDataAccess.GetDataTable(strSQL11);
                try
                {
                    string str15 = "";
                    string str16 = "";
                    if (!string.IsNullOrEmpty(dataTable18.Rows[0]["DATCES"].ToString()))
                    {
                        str15 = Convert.ToString(Convert.ToDateTime(dataTable18.Rows[0]["DATCES"].ToString().Substring(0, 10)));
                        str16 = Convert.ToDateTime(str15).ToString("yyyy-MM-dd");
                    }
                    DataRow row = dataTable18.Rows[0];
                    row.ToString().Substring(0, 10);
                    if (!string.IsNullOrEmpty(row.ToString().Substring(0, 10)) && dataTable18.Rows.Count > 0)
                    {
                        mat = fondoContoInd.Mat;
                        string strSQL12 = "SELECT * FROM RAPLAV A WHERE MAT = '" + mat.ToString() + "' AND DATCES IS NULL";
                        DataTable dataTable19 = new DataTable();
                        if (this.objDataAccess.GetDataTable(strSQL12).Rows.Count > 0)
                        {
                            HttpContext.Current.Items[(object)"WarningMessage"] = (object)("E' stata effettuata la richiesta di pagamento del fondo in data " + str10 + " dall'utente " + str9);
                        }
                        else
                        {
                            string[] strArray = new string[5]
                            {
                "SELECT * FROM TBRICLIQ WHERE TIPLIQ = 2 AND MAT = '",
                null,
                null,
                null,
                null
                            };
                            mat = fondoContoInd.Mat;
                            strArray[1] = mat.ToString();
                            strArray[2] = "' AND DATFINRDL > '";
                            strArray[3] = str16;
                            strArray[4] = "'";
                            string strSQL13 = string.Concat(strArray);
                            DataTable dataTable20 = new DataTable();
                            if (this.objDataAccess.GetDataTable(strSQL13).Rows.Count > 0)
                            {
                                HttpContext.Current.Items[(object)"WarningMessage"] = (object)("E' stata effettuata la richiesta di pagamento del fondo in data " + str10 + " dall'utente " + str9);
                            }
                            else
                            {
                                fondoContoInd.DataFineRapp = str15.ToString().Substring(0, 10);
                                HttpContext.Current.Session["DataRapp"] = (object)fondoContoInd.DataFineRapp;
                            }
                        }
                    }
                    else
                        HttpContext.Current.Items[(object)"WarningMessage"] = (object)("E' stata effettuata la richiesta di pagamento del fondo in data " + str10 + " dall'utente " + str9);
                }
                catch (Exception ex)
                {
                    fondoContoInd.DataFineRapp = "Nessuna Data";
                    HttpContext.Current.Items[(object)"WarningMessage"] = (object)"Errore: Nessuna data fine rapporto trovata";
                }
            }
            return fondoContoInd;
        }

        public Anagrafica RicFondo(Anagrafica FondoAnag)
        {
            var matricola = Utile.GetMatricolaIscritto(FondoAnag.CodiceFiscale);
            this.objDataAccess.StartTransaction();
            string datfinrdl = Convert.ToDateTime(FondoAnag.DataFineRapp.Substring(0, 10)).ToString("yyyy-MM-dd");
            //  //  if (HttpContext.Current.Request.Form["divPagamento"].ToString() == "1")
            //  //  {
            //  //      iDB2Command objCommand = this.objDataAccess.objCommand;
            //  //      string[] strArray = new string[5]
            //  //      {
            //  //"SELECT * FROM ISCBANC WHERE MAT='",
            //  //FondoAnag.Mat.ToString(),
            //  //"' AND IBAN='",
            //  //FondoAnag.Iban,
            //  //"'"
            //  //      };
            //  //      string str2;
            //  //      string str3 = str2 = string.Concat(strArray);
            //  //      objCommand.CommandText = str2;
            //  //      string strSQL = str3;
            //  //      DataTable dataTable = new DataTable();
            //  //      if (this.objDataAccess.GetDataTable(strSQL).Rows.Count < 1)
            //  //      {
            //  //          HttpContext.Current.Items[(object)"ErrorMessage"] = (object)"Iban errato";
            //  //          return FondoAnag;
            //  //      }
            //  //  }
            //  //  else

            //  iDB2Command objCommand1 = this.objDataAccess.objCommand;
            //  string[] strArray1 = new string[5]
            //  {
            //"SELECT * FROM TBRICLIQ WHERE TIPLIQ = 2 AND MAT = '",
            //FondoAnag.Mat.ToString(),
            //"'  AND DATFINRDL = '",
            //datfinrdl,
            //"'"
            //  };
            //  string str4;
            //  string str5 = str4 = string.Concat(strArray1);
            //  objCommand1.CommandText = str4;
            //  string strSQL1 = str5;
            //  DataTable dataTable1 = new DataTable();
            //  if (this.objDataAccess.GetDataTable(strSQL1).Rows.Count > 0)
            //  {
            //      HttpContext.Current.Items[(object)"ErrorMessage"] = (object)"Impossibile richiedere pagamento Conto, richiesta già effettuata";
            //  }
            //  else
            //  {
            //      //iDB2Command objCommand2 = this.objDataAccess.objCommand;
            //      //string str6 = FondoAnag.Mat.ToString();
            //      //string str7;
            //      //string str8 = str7 = "SELECT VALUE(COUNT(*),0) AS CONS FROM RAPLAV WHERE MAT = '" + str6 + "' AND CODPOS IN (SELECT CODPOS FROM AZI WHERE NATGIU = 10)";
            //      //objCommand2.CommandText = str7;
            //      //string strSQL2 = str8;
            //      //DataTable dataTable2 = new DataTable();
            //      //string str9 = this.objDataAccess.GetDataTable(strSQL2).Rows[0]["CONS"].ToString();
            //      iDB2Command objCommand3 = this.objDataAccess.objCommand;
            //      string[] strArray2 = new string[5]
            //      {
            //  "SELECT VALUE(COUNT(*), 0) AS CONT  FROM TBRICLIQ WHERE MAT =  '",
            //  FondoAnag.Mat.ToString(),
            //  "' AND DATFINRDL = '",
            //  datfinrdl,
            //  "' AND TIPLIQ = 2"
            //      };
            //      string str10;
            //      string str11 = str10 = string.Concat(strArray2);
            //      objCommand3.CommandText = str10;
            //      string strSQL3 = str11;
            //      DataTable dataTable3 = new DataTable();
            if (HasFondoContoIndividuale(FondoAnag.CodiceFiscale))
            {
                string idQuery = this.objDataAccess.objCommand.CommandText = "SELECT VALUE(MAX(ID),0) + 1 AS ID FROM TBRICLIQ";
                string id = this.objDataAccess.GetDataTable(idQuery).Rows[0]["ID"].ToString();
                string tipoPagamento = HttpContext.Current.Request.Form["divPagamento"].ToString();


                string tbricliqQuery = $"INSERT INTO TBRICLIQ (ID, MAT, DATFINRDL, MODPAG, IBAN, SWIFT, TIPLIQ, CODSTAPRA, DATCONF, UTECONF, DTRIC, UTERIC, FLGCON) " +
                                $"VALUES(" +
                                $"@id, @mat, @datfinrdl, @modpag, @iban, @swift, 2, 0, CURRENT_DATE, @user, CURRENT_DATE, @user, @flgcon)";

                string consorzioQuery = $"SELECT VALUE(COUNT(*),0) FROM RAPLAV WHERE MAT = {FondoAnag.Mat} AND CODPOS IN (SELECT CODPOS FROM AZI WHERE NATGIU = 10)";
                var countConsorzio = objDataAccess.Get1ValueFromSQL(consorzioQuery, CommandType.Text);

                iDB2Parameter idParam = objDataAccess.CreateParameter("@id", iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input, id);
                iDB2Parameter matricolaParam = objDataAccess.CreateParameter("@matricola", iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, matricola);
                iDB2Parameter ibanParam = objDataAccess.CreateParameter("@iban", iDB2DbType.iDB2VarChar, 50, ParameterDirection.Input, FondoAnag.Iban);
                iDB2Parameter swiftParam = objDataAccess.CreateParameter("@swift", iDB2DbType.iDB2VarChar, 50, ParameterDirection.Input, FondoAnag.Bic_swift);
                iDB2Parameter userParam = objDataAccess.CreateParameter("@user", iDB2DbType.iDB2VarChar, 20, ParameterDirection.Input, FondoAnag.CodiceFiscale);
                iDB2Parameter datfinrdlParam = objDataAccess.CreateParameter("@datfinrdl", iDB2DbType.iDB2Date, 10, ParameterDirection.Input, datfinrdl);
                iDB2Parameter modpagParam = objDataAccess.CreateParameter("@modpag", iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input, tipoPagamento);
                iDB2Parameter flgonParam = objDataAccess.CreateParameter("@flgon", iDB2DbType.iDB2Char, 1, ParameterDirection.Input, int.Parse(countConsorzio) > 0 ? "S" : "NULL");

                if (tipoPagamento == "2")
                {
                    ibanParam = objDataAccess.CreateParameter("@iban", iDB2DbType.iDB2VarChar, 50, ParameterDirection.Input, "NULL");
                    swiftParam = objDataAccess.CreateParameter("@swift", iDB2DbType.iDB2VarChar, 50, ParameterDirection.Input, "NULL");

                }
                var resultQuery = objDataAccess.WriteTransactionDataWithParametersAndDontCall(tbricliqQuery, CommandType.Text, idParam, matricolaParam, datfinrdlParam, modpagParam, ibanParam, swiftParam, userParam, userParam, flgonParam);

                if (resultQuery)
                {
                    string errorMsg = "";
                    if (tipoPagamento == "1" && !IbanUtils.CheckAndInsertIban(FondoAnag.Iban, matricola, objDataAccess, ref errorMsg, FondoAnag.CodiceFiscale))
                        return FondoAnag;
                    HttpContext.Current.Items[(object)"SuccessMessage"] = (object)" Inserimento andato a buon fine";
                    objDataAccess.EndTransaction(true);

                }
                else
                {
                    objDataAccess.EndTransaction(false);
                    HttpContext.Current.Items[(object)"ErrorMessage"] = (object)"Si sono verificati errori durante il salvataggio della richiesta ";
                }
                //string str14 = (this.objDataAccess.objCommand.CommandText = "INSERT INTO TBRICLIQ(ID, MAT, DATFINRDL, MODPAG, IBAN, SWIFT, TIPLIQ, DATCONF, UTECONF, DTRIC, UTERIC, FLGCON, FLGINS, TIPO_DOC, NUMERO_DOC, SCADENZA_DOC) VALUES(") + str12 + ", " + FondoAnag.Mat.ToString() + ", " + "'" + datfinrdl.ToString().Substring(0, 10) + "', " + tipoPagamento + ", ";
                //string str15 = (!(tipoPagamento == "1") ? str14 + "NULL, " + "NULL, " : str14 + FondoAnag.Iban + ", " + FondoAnag.Bic_swift + ", ") + "2, " + "'" + DateTime.Now.ToString("yyyy-MM-dd") + "', " + "'" + FondoAnag.CodiceFiscale + "', " + "'" + DateTime.Now.ToString("yyyy-MM-dd") + "', " + "'" + FondoAnag.CodiceFiscale + "', ";
                //if (this.objDataAccess.WriteData((Convert.ToInt32(str9) <= 0 ? str15 + " NULL, 'W'," : str15 + " 'S', 'W',") + "'" + FondoAnag.Tipo_Documento + "','" + FondoAnag.Numero_Documento + "','" + FondoAnag.Scadenza_Documento.Substring(0, 10) + "')", CommandType.Text))
                //    HttpContext.Current.Items[(object)"SuccessMessage"] = (object)" Inserimento Andato a buon fine";
                //else
                //    HttpContext.Current.Items[(object)"ErrorMessage"] = (object)"Si sono verificati errori durente il salvataggio della richiesta ";
            }
            return FondoAnag;
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

        public bool HasFondoContoIndividuale(string username)
        {
            string matricola = Utile.GetMatricolaIscritto(username);
            DateTime dob = Utile.GetDataDiNascita(username);
            if (IsOver65(dob))
            {
                string dataCessQuery = $@"SELECT MAX(DATCES) AS DATCES FROM RAPLAV A WHERE MAT = {matricola} AND DATCES IS NOT NULL AND DTFP IS NOT NULL AND MAT NOT IN(SELECT MAT FROM TBRICLIQ WHERE TIPLIQ = 2 AND MAT = A.MAT AND DATFINRDL = A.DATCES)";
                DataTable fondoContoResult = objDataAccess.GetDataTable(dataCessQuery);
                if (fondoContoResult.Rows.Count > 0 && fondoContoResult.Rows[0]["DATCES"].ToString().Trim() != "")
                {
                    string strSQL = @$"SELECT count(*) FROM RAPLAV A WHERE MAT = {matricola} AND CURRENT_DATE BETWEEN DATDEC AND VALUE(DATCES, '9999-12-31')";
                    if (int.Parse(objDataAccess.Get1ValueFromSQL(strSQL, CommandType.Text)) == 0)
                    {
                        string datFineRDLQuery = $@"SELECT * FROM TBRICLIQ WHERE TIPLIQ = 2 AND MAT = {matricola} AND DATFINRDL >= '{Utile.FromStringToDateTimeToFormatString(fondoContoResult.Rows[0]["DATCES"].ToString().Trim())}'";
                        DataTable datFineRDLResult = objDataAccess.GetDataTable(datFineRDLQuery);

                        if (datFineRDLResult.Rows.Count > 0)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        string strSQL2 = $@"SELECT * FROM TBRICLIQ WHERE TIPLIQ = 2 AND MAT = {matricola} AND DATFINRDL >= '{dob.AddYears(65).AddDays(-1).ToString("yyyy-MM-dd")}'";
                        DataTable objDt2 = objDataAccess.GetDataTable(strSQL2);

                        if (objDt2.Rows.Count > 0)
                            return false;
                        else
                        {
                            HttpContext.Current.Items["DataFineRapp"] = dob.AddYears(+65).AddDays(-1).ToString("dd/MM/yyyy");
                            return true;
                        }

                    }
                }
                else
                {
                    string strSQL3 = $@"SELECT * FROM TBRICLIQ WHERE TIPLIQ = 2 AND MAT = {matricola} AND DATFINRDL >= '{dob.AddYears(65).AddDays(-1).ToString("yyyy-MM-dd")}'";
                    DataTable objDt2 = objDataAccess.GetDataTable(strSQL3);

                    if (objDt2.Rows.Count > 0)
                        return false;
                    else
                    {
                        HttpContext.Current.Items["DataFineRapp"] = dob.AddYears(+65).AddDays(-1).ToString("dd/MM/yyyy");
                        return true;
                    }
                }
            }
            else
            {
                string strSQL4 = $@"SELECT MAX(DATCES) AS DATCES FROM RAPLAV A WHERE MAT = {matricola} AND DATCES IS NOT NULL AND DTFP IS NOT NULL AND MAT NOT IN(SELECT MAT FROM TBRICLIQ WHERE TIPLIQ = 2 AND MAT = A.MAT AND DATFINRDL = A.DATCES)";
                DataTable fondoContoResult = objDataAccess.GetDataTable(strSQL4);

                if (fondoContoResult.Rows.Count > 0 && fondoContoResult.Rows[0]["DATCES"].ToString().Trim() != "")
                {
                    string strSQL5 = $@"SELECT * FROM RAPLAV A WHERE MAT = {matricola} AND DATCES IS NULL";
                    DataTable objDt3 = objDataAccess.GetDataTable(strSQL5);
                    if (objDt3.Rows.Count > 0)
                    {
                        return false;
                    }
                    else
                    {
                        string strSql10 = $@"SELECT * FROM TBRICLIQ WHERE TIPLIQ = 2 AND MAT = {matricola} AND DATFINRDL > '{Utile.FromStringToDateTimeToFormatString(fondoContoResult.Rows[0]["DATCES"].ToString().Trim())}'";
                        DataTable objDt10 = objDataAccess.GetDataTable(strSql10);
                        if (objDt10.Rows.Count > 0)
                        {
                            return false;
                        }
                        else
                        {
                            HttpContext.Current.Items["DataFineRapp"] = fondoContoResult.Rows[0]["DATCES"].ToString().Trim().Substring(0, 10);
                            return true;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        private bool IsOver65(DateTime dob)
        {
            DateTime today = DateTime.Now.Date;
            return DateTime.Compare(dob.AddYears(65), today) <= 0;
        }

    }
}
