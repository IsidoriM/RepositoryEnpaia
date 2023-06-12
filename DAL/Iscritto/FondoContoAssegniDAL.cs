// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Iscritto.FondoContoAssegniDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using System;
using System.Data;
using System.Web;
using TFI.DAL.ConnectorDB;
using TFI.OCM.Iscritto;

namespace TFI.DAL.Iscritto
{
    public class FondoContoAssegniDAL
    {
        private readonly DataLayer objDataAccess = new DataLayer();

        public Anagrafica GetFondoContoAssegni(
          string cf,
          ref string ErroreMSG,
          ref string SuccessMSG,
          ref string InfoMSG)
        {
            string strSQL1 = "SELECT DISTINCT C.SIGPRO,C.DENCOM,T.DENTITSTU,I.MAT,I.COGNOME,I.NOME,I.DATNAS,I.SES,I.STAESTNAS,I.IND,I.NUMCIV,I.CAP,I.EMAIL,I.EMAILCERT,I.CELL,I.DENLOC,I.SIGPRO,I.DENSTAEST,D.DENDUG, C.CODSTA FROM  CODCOM C,  ISCTWEB I,  TITSTU T,  DUG D  WHERE  I.CODFIS= '" + cf + "' AND I.CODDUG=D.CODDUG  AND I.CODCOMNAS=C.CODCOM  AND I.CODTITSTU=T.CODTITSTU ";
            string strSQL2 = "SELECT DISTINCT C.SIGPRO,C.DENCOM FROM  CODCOM C,  ISCTWEB I  WHERE  I.CODCOMNAS=C.CODCOM  AND I.CODFIS= '" + cf + "' ";
            DataTable dataTable1 = new DataTable();
            DataTable dataTable2 = new DataTable();
            DataTable dataTable3 = this.objDataAccess.GetDataTable(strSQL1);
            DataTable dataTable4 = this.objDataAccess.GetDataTable(strSQL2);
            string stato = dataTable3.Rows[0]["CODSTA"].ToString().Trim();
            string empty1 = string.Empty;
            string empty2 = string.Empty;
            string denComEstero = "";
            string str2 = "";
            string str3 = "";
            if (stato == "IT")
            {
                str2 = dataTable4.Rows[0]["DENCOM"].ToString();
                str3 = dataTable4.Rows[0]["SIGPRO"].ToString();
            }
            else
            {
                string strSQL3 = "SELECT C.SIGPRO, C.DENCOM  FROM  COM_ESTERO C, ISCTWEB I  WHERE  I.CODCOMNAS=C.CODCOM  AND I.CODFIS='" + cf + "' ";
                DataTable dataTable5 = new DataTable();
                DataTable dataTable6 = this.objDataAccess.GetDataTable(strSQL3);
                denComEstero = dataTable6.Rows[0]["DENCOM"].ToString().Trim();
                //str2 = dataTable6.Rows[0]["DENCOM"].ToString();
                //str3 = dataTable6.Rows[0]["SIGPRO"].ToString();
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
            Anagrafica fondoContoAssegni = (Anagrafica)null;
            DataLayer dataLayer = new DataLayer();
            if (dataLayer.queryOk(dataTable3) && dataLayer.queryOk(dataTable4) && dataLayer.queryOk(dt))
                fondoContoAssegni = new Anagrafica()
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
                    StatoEsteroNascita = denComEstero,
                    Telefono1 = str4,
                    Telefono2 = str5,
                    Fax = str6,
                    TipoResidenza = dataTable3.Rows[0]["DENDUG"].ToString(),
                    ShowBtn = true
                };
            string str9 = "";
            string str10 = (string)null;
            try
            {
                string strSQL5 = string.Format(" SELECT DTRIC,UTERIC FROM TBRICLIQ t WHERE TIPLIQ = 2 AND MAT = {0}", (object)fondoContoAssegni.Mat);
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
            string str11 = fondoContoAssegni.DataNascita.AddYears(65).ToString();
            if (Convert.ToDateTime(str11) <= DateTime.Now)
            {
                Decimal mat = fondoContoAssegni.Mat;
                string strSQL6 = "SELECT MAX(DATCES) AS DATCES FROM RAPLAV A WHERE MAT ='" + mat.ToString() + "' AND DATCES IS NOT NULL AND DTFP IS NOT NULL AND MAT NOT IN (SELECT MAT FROM TBRICLIQ WHERE TIPLIQ = 2 AND MAT = A.MAT AND DATFINRDL = A.DATCES)";
                DataTable dataTable11 = new DataTable();
                DataTable dataTable12 = this.objDataAccess.GetDataTable(strSQL6);
                try
                {
                    DateTime dateTime1 = Convert.ToDateTime(dataTable12.Rows[0]["DATCES"].ToString());
                    string str12 = dateTime1.ToString("yyyy-MM-dd");
                    if (dataTable12.Rows[0].ToString().Substring(0, 10) != "" && dataTable12.Rows.Count > 0)
                    {
                        mat = fondoContoAssegni.Mat;
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
                            mat = fondoContoAssegni.Mat;
                            strArray[1] = mat.ToString();
                            strArray[2] = "' AND DATFINRDL >= '";
                            strArray[3] = str12;
                            strArray[4] = "'";
                            string strSQL8 = string.Concat(strArray);
                            DataTable dataTable14 = new DataTable();
                            if (this.objDataAccess.GetDataTable(strSQL8).Rows.Count > 0 && str10 != null)
                            {
                                InfoMSG = "E' stata effettuata la richiesta di pagamento del fondo in data " + str10 + " dall'utente " + str9;
                            }
                            else
                            {
                                fondoContoAssegni.DataFineRapp = dateTime1.ToString().Substring(0, 10);
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
                            mat = fondoContoAssegni.Mat;
                            strArray[1] = mat.ToString();
                            strArray[2] = "' AND DATFINRDL > '";
                            DateTime dateTime2 = Convert.ToDateTime(str11);
                            dateTime2 = dateTime2.AddDays(-1.0);
                            strArray[3] = dateTime2.ToString();
                            strArray[4] = "'";
                            string strSQL9 = string.Concat(strArray);
                            DataTable dataTable15 = new DataTable();
                            if (this.objDataAccess.GetDataTable(strSQL9).Rows.Count > 0 && str10 != null)
                            {
                                InfoMSG = "E' stata effettuata la richiesta di pagamento del fondo in data " + str10 + " dall'utente " + str9;
                            }
                            else
                            {
                                dateTime2 = Convert.ToDateTime(str11);
                                dateTime2 = dateTime2.AddDays(-1.0);
                                string str13 = dateTime2.ToString().Substring(0, 10);
                                fondoContoAssegni.DataFineRapp = str13;
                                HttpContext.Current.Session["DataRapp"] = (object)fondoContoAssegni.DataFineRapp;
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
                        mat = fondoContoAssegni.Mat;
                        strArray[1] = mat.ToString();
                        strArray[2] = "' AND DATFINRDL > '";
                        DateTime dateTime3 = Convert.ToDateTime(str11);
                        dateTime3 = dateTime3.AddDays(-1.0);
                        strArray[3] = dateTime3.ToString();
                        strArray[4] = "'";
                        string strSQL10 = string.Concat(strArray);
                        DataTable dataTable16 = new DataTable();
                        if (this.objDataAccess.GetDataTable(strSQL10).Rows.Count > 0 && str10 != null)
                        {
                            InfoMSG = "E' stata effettuata la richiesta di pagamento del fondo in data " + str10 + " dall'utente " + str9;
                        }
                        else
                        {
                            dateTime3 = Convert.ToDateTime(str11);
                            dateTime3 = dateTime3.AddDays(-1.0);
                            string str14 = dateTime3.ToString().Substring(0, 10);
                            fondoContoAssegni.DataFineRapp = str14;
                            HttpContext.Current.Session["DataRapp"] = (object)fondoContoAssegni.DataFineRapp;
                        }
                    }
                }
                catch (Exception ex)
                {
                    fondoContoAssegni.DataFineRapp = "Nessuna Data";
                    ErroreMSG = "Nessuna data fine rapporto trovata";
                }
            }
            else
            {
                Decimal mat = fondoContoAssegni.Mat;
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
                    if (dataTable18.Rows[0].ToString().Substring(0, 10) != "" && dataTable18.Rows.Count > 0)
                    {
                        mat = fondoContoAssegni.Mat;
                        string strSQL12 = "SELECT * FROM RAPLAV A WHERE MAT = '" + mat.ToString() + "' AND DATCES IS NULL";
                        DataTable dataTable19 = new DataTable();
                        if (this.objDataAccess.GetDataTable(strSQL12).Rows.Count > 0 && str10 != null)
                            InfoMSG = "E' stata effettuata la richiesta di pagamento del fondo in data " + str10 + " dall'utente " + str9;
                        else if (str16 != "")
                        {
                            string[] strArray = new string[5]
                            {
                "SELECT * FROM TBRICLIQ WHERE TIPLIQ = 2 AND MAT = '",
                null,
                null,
                null,
                null
                            };
                            mat = fondoContoAssegni.Mat;
                            strArray[1] = mat.ToString();
                            strArray[2] = "' AND DATFINRDL > '";
                            strArray[3] = str16;
                            strArray[4] = "'";
                            string strSQL13 = string.Concat(strArray);
                            DataTable dataTable20 = new DataTable();
                            if (this.objDataAccess.GetDataTable(strSQL13).Rows.Count > 0 && str10 != null)
                            {
                                InfoMSG = "E' stata effettuata la richiesta di pagamento del fondo in data " + str10 + " dall'utente " + str9;
                            }
                            else
                            {
                                fondoContoAssegni.DataFineRapp = str15.ToString().Substring(0, 10);
                                HttpContext.Current.Session["DataRapp"] = (object)fondoContoAssegni.DataFineRapp;
                            }
                        }
                    }
                    else
                        InfoMSG = "E' stata effettuata la richiesta di pagamento del fondo in data " + str10 + " dall'utente " + str9;
                    if (HttpContext.Current.Session["DataRapp"] == null)
                    {
                        InfoMSG = "Non è possibile richiedere l'assegno poiché il rapporto assicurativo con ENPAIA è ancora aperto.";
                        fondoContoAssegni.ShowBtn = false;
                    }
                }
                catch (Exception ex)
                {
                    fondoContoAssegni.DataFineRapp = "Nessuna Data";
                    ErroreMSG = "Errore: Nessuna data fine rapporto trovata";
                }
            }
            return fondoContoAssegni;
        }

        public Anagrafica RicFondo(
          Anagrafica FondoAnag,
          ref string ErroreMSG,
          ref string SuccessMSG)
        {
            this.objDataAccess.StartTransaction();
            string str1 = Convert.ToDateTime(FondoAnag.DataFineRapp).ToString("yyyy-MM-dd");
            if (HttpContext.Current.Request.Form["divPagamento"].ToString() == "1")
            {
                iDB2Command objCommand = this.objDataAccess.objCommand;
                string[] strArray = new string[5]
                {
          "SELECT * FROM ISCBANC WHERE MAT='",
          FondoAnag.Mat.ToString(),
          "' AND IBAN='",
          FondoAnag.Iban,
          "'"
                };
                string str2;
                string str3 = str2 = string.Concat(strArray);
                objCommand.CommandText = str2;
                string strSQL = str3;
                DataTable dataTable = new DataTable();
                if (this.objDataAccess.GetDataTable(strSQL).Rows.Count < 1)
                {
                    ErroreMSG = "Iban errato";
                    return FondoAnag;
                }
            }
            else
            {
                iDB2Command objCommand1 = this.objDataAccess.objCommand;
                string[] strArray1 = new string[5]
                {
          "SELECT * FROM TBRICLIQ WHERE TIPLIQ = 2 AND MAT = '",
          FondoAnag.Mat.ToString(),
          "'  AND DATFINRDL = '",
          str1,
          "'"
                };
                string str4;
                string str5 = str4 = string.Concat(strArray1);
                objCommand1.CommandText = str4;
                string strSQL1 = str5;
                DataTable dataTable1 = new DataTable();
                if (this.objDataAccess.GetDataTable(strSQL1).Rows.Count > 0)
                {
                    ErroreMSG = "Impossibile richiedere pagamento Conto, richiesta già effettuata";
                }
                else
                {
                    iDB2Command objCommand2 = this.objDataAccess.objCommand;
                    string str6 = FondoAnag.Mat.ToString();
                    string str7;
                    string str8 = str7 = "SELECT VALUE(COUNT(*),0) AS CONS FROM RAPLAV WHERE MAT = '" + str6 + "' AND CODPOS IN (SELECT CODPOS FROM AZI WHERE NATGIU = 10)";
                    objCommand2.CommandText = str7;
                    string strSQL2 = str8;
                    DataTable dataTable2 = new DataTable();
                    string str9 = this.objDataAccess.GetDataTable(strSQL2).Rows[0]["CONS"].ToString();
                    iDB2Command objCommand3 = this.objDataAccess.objCommand;
                    string[] strArray2 = new string[5]
                    {
            "SELECT VALUE(COUNT(*), 0) AS CONT  FROM TBRICLIQ WHERE MAT =  '",
            FondoAnag.Mat.ToString(),
            "' AND DATFINRDL = '",
            str1,
            "' AND TIPLIQ = 2"
                    };
                    string str10;
                    string str11 = str10 = string.Concat(strArray2);
                    objCommand3.CommandText = str10;
                    string strSQL3 = str11;
                    DataTable dataTable3 = new DataTable();
                    if (this.objDataAccess.GetDataTable(strSQL3).Rows[0]["CONT"].ToString() == "0")
                    {
                        string strSQL4 = this.objDataAccess.objCommand.CommandText = "SELECT VALUE(MAX(ID),0) + 1 AS ID FROM TBRICLIQ";
                        DataTable dataTable4 = new DataTable();
                        string str12 = this.objDataAccess.GetDataTable(strSQL4).Rows[0]["ID"].ToString();
                        string str13 = HttpContext.Current.Request.Form["divPagamento"].ToString();
                        string str14 = (this.objDataAccess.objCommand.CommandText = "INSERT INTO TBRICLIQ(ID, MAT, DATFINRDL, MODPAG, IBAN, SWIFT, TIPLIQ, DATCONF, UTECONF, DTRIC, UTERIC, FLGCON, FLGINS) VALUES(") + str12 + ", " + FondoAnag.Mat.ToString() + ", " + str1.ToString().Substring(0, 10) + ", " + str13 + ", ";
                        string str15 = (!(str13 == "1") ? str14 + "NULL, " + "NULL, " : str14 + FondoAnag.Iban + ", " + FondoAnag.Bic_swift + ", ") + "2, " + DateTime.Now.ToString() + ", " + FondoAnag.CodiceFiscale + ", " + DateTime.Now.ToString() + ", " + FondoAnag.CodiceFiscale + ", ";
                        if (this.objDataAccess.WriteData(Convert.ToInt32(str9) <= 0 ? str15 + " NULL, 'W')" : str15 + " 'S', 'W')", CommandType.Text))
                            SuccessMSG = " Inserimento Andato a buon fine";
                        else
                            ErroreMSG = "Si sono verificati errori durente il salvataggio della richiesta ";
                    }
                }
            }
            return FondoAnag;
        }
    }
}
