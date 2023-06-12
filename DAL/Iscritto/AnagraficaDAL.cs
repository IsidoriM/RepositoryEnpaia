// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Iscritto.AnagraficaDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using TFI.DAL.ConnectorDB;
using TFI.OCM.Iscritto;

namespace TFI.DAL.Iscritto
{
    public class AnagraficaDAL
    {
        private readonly DataLayer objDataAccess = new DataLayer();

        public Anagrafica GetAnagrafica(string cf)
        {
           iDB2DataReader dataReaderFromQuery1 = this.objDataAccess.GetDataReaderFromQuery("SELECT DENCOM FROM CODCOM", CommandType.Text);
            List<string> stringList1 = new List<string>();
            while (dataReaderFromQuery1.Read())
                stringList1.Add(dataReaderFromQuery1["DENCOM"].ToString().Trim());
            HttpContext.Current.Items[(object)"ListaComuni"] = (object)stringList1.ToArray();
            iDB2DataReader dataReaderFromQuery2 = this.objDataAccess.GetDataReaderFromQuery("SELECT DENCOM FROM COM_ESTERO", CommandType.Text);
            List<string> stringList2 = new List<string>();
            while (dataReaderFromQuery2.Read())
                stringList2.Add(dataReaderFromQuery2["DENCOM"].ToString().Trim());
            HttpContext.Current.Items[(object)"ListaStati"] = (object)stringList2.ToArray();
            iDB2DataReader dataReaderFromQuery3 = this.objDataAccess.GetDataReaderFromQuery("SELECT DENDUG FROM DUG ORDER BY DENDUG", CommandType.Text);
            List<string> stringList3 = new List<string>();
            while (dataReaderFromQuery3.Read())
                stringList3.Add(dataReaderFromQuery3["DENDUG"].ToString().Trim());
            HttpContext.Current.Items[(object)"ListaTipoInd"] = (object)stringList3.ToArray();
            //string strSQL1 = "SELECT DISTINCT C.SIGPRO,C.DENCOM,T.DENTITSTU,I.MAT,  I.COGNOME,I.NOME,I.DATNAS,I.SES,I.STAESTNAS,I.IND,  I.NUMCIV,I.CAP,I.EMAIL,I.EMAILCERT,I.CELL,I.DENLOC,I.SIGPRO,I.DENSTAEST,D.DENDUG  FROM  CODCOM C  RIGHT JOIN ISCTWEB I ON I.CODCOM = C.CODCOM  LEFT JOIN TITSTU T ON I.CODTITSTU = T.CODTITSTU  LEFT JOIN DUG D ON I.CODDUG = D.CODDUG  WHERE  I.CODFIS = '" + cf + "'";

            string strSQL1 = $@"select  L.SIGPRO
                                        , L.DENCOM
                                        , T.DENTITSTU
                                        , I.MAT 
                                        , I.COG as COGNOME
                                        , I.NOM as NOME
                                        , I.DATNAS 
                                        , I.SES
                                        , '' as STAESTNAS
                                        , L.IND
                                        , L.NUMCIV
                                        , L.CAP
                                        , L.EMAIL
                                        , L.EMAILCERT
                                        , L.CELL
                                        , L.DENLOC
                                        , L.SIGPRO
                                        , L.DENSTAEST
                                        , L.IBAN
                                        , D.DENDUG  
                                        , L.TEL1
                                        , L.CO
                                FROM ISCT I 
                                LEFT JOIN ISCD L on I.MAT = L.MAT 
                                LEFT JOIN DUG D ON L.CODDUG = D.CODDUG 
                                LEFT JOIN TITSTU T ON I.TITSTU = T.CODTITSTU 
                                WHERE I.CODFIS = '{cf}'
                                ORDER BY L.ULTAGG DESC LIMIT 1";


            // string strSQL2 = "SELECT DISTINCT C.SIGPRO,C.DENCOM, C.CODCOM FROM  CODCOM C,  ISCTWEB I  WHERE  I.CODCOMNAS=C.CODCOM  AND I.CODFIS= '" + cf + "' ";
            string strSQL2 = $@"SELECT C.SIGPRO,C.DENCOM, C.CODCOM, C.CODSTA
                                FROM  ISCT I 
                                left join ISCD L on I.MAT = L.MAT   
                                left join CODCOM C ON I.CODCOM = C.CODCOM 
                                WHERE  I.CODFIS= '{cf}' 
                                order by L.ULTAGG limit 1";
            // DataTable dataTable1 = new DataTable();
            // DataTable dataTable2 = new DataTable();
            DataTable dataTable3 = this.objDataAccess.GetDataTable(strSQL1);
            DataTable dataTable4 = this.objDataAccess.GetDataTable(strSQL2);
            string stato = dataTable4.Rows[0]["CODSTA"].ToString().Trim();
            string empty1 = string.Empty;
            string empty2 = string.Empty;
            string denComEstero = "";
            string str2 = "";
            string str3 = "";
            string str9;
            string str10 = "";
            if (stato == "IT")
            {
                str2 = dataTable4.Rows[0]["DENCOM"].ToString().Trim();
                str3 = dataTable4.Rows[0]["SIGPRO"].ToString().Trim();
                str10 = dataTable4.Rows[0]["CODCOM"].ToString().Trim();
            }
            else
            {
                string strSQL3 = "SELECT C.SIGPRO, C.DENCOM, C.CODCOM  FROM  COM_ESTERO C, ISCT I  WHERE  I.CODCOM=C.CODCOM  AND I.CODFIS='" + cf + "' ";
                DataTable dataTable5 = new DataTable();
                DataTable dataTable6 = this.objDataAccess.GetDataTable(strSQL3);
                denComEstero = dataTable6.Rows[0]["DENCOM"].ToString().Trim();
                //str3 = dataTable6.Rows[0]["SIGPRO"].ToString().Trim();
                //str10 = dataTable6.Rows[0]["CODCOM"].ToString().Trim();
            }
            Decimal num = Convert.ToDecimal(dataTable3.Rows[0]["MAT"]);
            DataTable dt = new DataTable();
            string str4;
            string str5;
            string str6;
            try
            {
                dt = this.objDataAccess.GetDataTable("SELECT TEL1,TEL2,FAX FROM  ISCD  WHERE  MAT= '" + num.ToString() + "' ORDER BY ULTAGG DESC LIMIT 1");
                str4 = dt.Rows[0]["TEL1"].ToString().Trim();
                str5 = dt.Rows[0]["TEL2"].ToString().Trim();
                str6 = dt.Rows[0]["FAX"].ToString().Trim();
            }
            catch (Exception ex)
            {
                str4 = "";
                str5 = "";
                str6 = "";
            }
            string str7;
            string str8;
            string residenzaQuery = $@"SELECT C.SIGPRO,C.DENCOM, C.CODCOM 
                                FROM  ISCT I 
                                left join ISCD L on I.MAT = L.MAT   
                                left join CODCOM C ON L.CODCOM = C.CODCOM 
                                WHERE  I.CODFIS= '{cf}' 
                                order by L.ULTAGG desc limit 1";
            DataTable residenzaTable = this.objDataAccess.GetDataTable(residenzaQuery);
            if (dataTable3.Rows[0]["DENSTAEST"].ToString().Trim() == "")
            {
                str7 = residenzaTable.Rows[0]["DENCOM"].ToString().Trim();
                str8 = residenzaTable.Rows[0]["SIGPRO"].ToString().Trim();
                str9 = residenzaTable.Rows[0]["CODCOM"].ToString().Trim();
            }
            else
            {
                string strSQL4 = "SELECT C.DENCOM,C.SIGPRO, C.CODCOM FROM COM_ESTERO C,ISCTWEB I WHERE C.CODCOM=I.CODCOM";
                DataTable dataTable7 = new DataTable();
                DataTable dataTable8 = this.objDataAccess.GetDataTable(strSQL4);
                str7 = dataTable8.Rows[0]["DENCOM"].ToString().Trim();
                str8 = dataTable8.Rows[0]["SIGPRO"].ToString().Trim();
                str9 = dataTable8.Rows[0]["CODCOM"].ToString().Trim();
            }
            Anagrafica anagrafica = (Anagrafica)null;
            DataLayer dataLayer = new DataLayer();
            if (!dataLayer.queryOk(dataTable3) || !dataLayer.queryOk(dataTable4) || !dataLayer.queryOk(dt))
                return anagrafica;
            return new Anagrafica()
            {
                Mat = num,
                Nome = dataTable3.Rows[0]["NOME"].ToString().Trim(),
                Cognome = dataTable3.Rows[0]["COGNOME"].ToString().Trim(),
                CodiceFiscale = cf,
                DataNascita = Convert.ToDateTime(dataTable3.Rows[0]["DATNAS"]),
                Sesso = dataTable3.Rows[0]["SES"].ToString().Trim(),
                TitoloStudio = dataTable3.Rows[0]["DENTITSTU"].ToString().Trim(),
                Indirizzo = dataTable3.Rows[0]["IND"].ToString().Trim(),
                NumeroCivico = dataTable3.Rows[0]["NUMCIV"].ToString().Trim(),
                Cap = dataTable3.Rows[0]["CAP"].ToString().Trim(),
                Localita = dataTable3.Rows[0]["DENLOC"].ToString().Trim(),
                Iban = dataTable3.Rows[0]["IBAN"].ToString().Trim(),
                SigproResidenza = str8,
                ComuneResidenza = str7,
                CodComuneResidenza = str9,
                Email = dataTable3.Rows[0]["EMAIL"].ToString().Trim(),
                EmailCert = dataTable3.Rows[0]["EMAILCERT"].ToString().Trim(),
                Cellulare = dataTable3.Rows[0]["CELL"].ToString().Trim(),
                StatoEsteroResidenza = dataTable3.Rows[0]["DENSTAEST"].ToString().Trim(),
                ComuneNascita = str2,
                CodComuneNascita = str10,
                SigproNascita = str3,
                StatoEsteroNascita = denComEstero,
                Telefono1 = str4,
                Telefono2 = str5,
                Fax = str6,
                TipoResidenza = dataTable3.Rows[0]["DENDUG"].ToString().Trim(),
                Co = dataTable3.Rows[0]["CO"].ToString().Trim()
            };
        }

        public Anagrafica ModificaAnagrafica(
          Anagrafica ModAnagrafica,
          ref string ErrorMsg,
          ref string SuccesMsg)
        {
            var anagraficaParameters = CleanValuesAndPartiallyMapAnagraficaIntoParameters(ModAnagrafica);

            this.objDataAccess.StartTransaction();
            try
            {
                Decimal num;
                if (string.IsNullOrEmpty(ModAnagrafica.StatoEsteroResidenza))
                {
                    this.objDataAccess.WriteTransactionData("UPDATE ISCTWEB SET " +
                                                            "CODCOM=(SELECT CODCOM FROM CODCOM WHERE DENCOM='" + ModAnagrafica.ComuneResidenza + "')" +
                                                            ",NUMCIV='" + ModAnagrafica.NumeroCivico + "', DENSTAEST='" + ModAnagrafica.StatoEsteroResidenza + "'" +
                                                            ", IND='" + ModAnagrafica.Indirizzo + "', DENLOC='" + ModAnagrafica.Localita + "', " +
                                                            "CAP='" + ModAnagrafica.Cap + "', " +
                                                            "SIGPRO=(SELECT SIGPRO FROM CODCOM WHERE DENCOM='" + ModAnagrafica.ComuneResidenza.Replace("'", "''") + "'), " +
                                                            "CELL='" + ModAnagrafica.Cellulare + "', " +
                                                            "" +
                                                            "" +
                                                            "EMAIL='" + ModAnagrafica.Email + "', " +
                                                            "EMAILCERT='" + ModAnagrafica.EmailCert + "'," +
                                                            " CODTITSTU='" + ModAnagrafica.CodTitstu.ToString() +
                                                            "' WHERE CODFIS='" + ModAnagrafica.CodiceFiscale + "'", CommandType.Text);
                }
                else
                {
                    string[] strArray = new string[23]
                    {
            "UPDATE ISCTWEB SET CODCOM=(SELECT CODCOM FROM COM_ESTERO WHERE DENCOM='",
            ModAnagrafica.StatoEsteroResidenza,
            "'),NUMCIV='",
            ModAnagrafica.NumeroCivico,
            "', DENSTAEST='",
            ModAnagrafica.StatoEsteroResidenza,
            "', IND='",
            ModAnagrafica.Indirizzo,
            "', DENLOC='",
            ModAnagrafica.Localita,
            "', CAP='",
            ModAnagrafica.Cap,
            "', SIGPRO='EE', CELL='",
            ModAnagrafica.Cellulare,
            "', EMAIL='",
            ModAnagrafica.Email,
            "', EMAILCERT='",
            ModAnagrafica.EmailCert,
            "', CODTITSTU='",
            null,
            null,
            null,
            null
                    };
                    num = ModAnagrafica.CodTitstu;
                    strArray[19] = num.ToString();
                    strArray[20] = "' WHERE CODFIS='";
                    strArray[21] = ModAnagrafica.CodiceFiscale;
                    strArray[22] = "'";
                    this.objDataAccess.WriteTransactionData(string.Concat(strArray), CommandType.Text);
                }
                string ctrlUpdate = $"SELECT * FROM ISCD WHERE MAT = {ModAnagrafica.Mat} AND DATINI = '{DateTime.Now.ToString("yyyy-MM-dd")}'";
                DataTable dtCtrlUpdate = this.objDataAccess.GetDataTable(ctrlUpdate);
                if (dtCtrlUpdate != null && dtCtrlUpdate.Rows.Count > 0)
                {
                    string updateIscd = $@"UPDATE ISCD SET 
                                           CODDUG = '{ModAnagrafica.CodTipoResidenza}'
                                            , IND = @Indirizzo
                                            , NUMCIV = @Civico
                                            , DENSTAEST = '{ModAnagrafica.StatoEsteroResidenza}'
                                            , DENLOC = '{ModAnagrafica.Localita}'
                                            , CAP = '{ModAnagrafica.Cap}'
                                            , SIGPRO = '{ModAnagrafica.SigproResidenza}'
                                            , AGGMAN = 'S'
                                            , TEL1 = @Tel
                                            , EMAIL = @Email
                                            , ULTAGG = CURRENT TIMESTAMP
                                            , UTEAGG = '{ModAnagrafica.CodiceFiscale}'
                                            , CODCOM = '{ModAnagrafica.CodComuneResidenza}'
                                            , DENCOM = '{ModAnagrafica.ComuneResidenza}'
                                            , CO = @Co
                                            , EMAILCERT = @Pec
                                            , CELL = @Cell
                                            , IBAN = '{ModAnagrafica.Iban}'
                                    WHERE MAT = {ModAnagrafica.Mat} AND DATINI = '{DateTime.Now.ToString("yyyy-MM-dd")}'";

                    var result = objDataAccess.WriteTransactionDataWithParametersAndDontCall(updateIscd, CommandType.Text,
                        anagraficaParameters["@Indirizzo"], anagraficaParameters["@Civico"], anagraficaParameters["@Tel"], anagraficaParameters["@Email"],
                        anagraficaParameters["@Co"], anagraficaParameters["@Pec"], anagraficaParameters["@Cell"]);
                    if (result)
                    {
                        SuccesMsg = "Modifiche avvenute con successo";
                    }
                    else
                    {
                        this.objDataAccess.EndTransaction(false);
                        ErrorMsg = "Errore nel salvataggio";
                        return (Anagrafica)null;
                    }
                }
                else
                {
                    string insertIscd = $@"INSERT INTO ISCD (MAT, DATINI, CODDUG, IND, NUMCIV, DENSTAEST, DENLOC, CAP, SIGPRO, AGGMAN, TEL1
	                                   , EMAIL, ULTAGG, UTEAGG, CODCOM, DENCOM, CO, EMAILCERT, CELL, IBAN)
                                    VALUES ({ModAnagrafica.Mat} 
                                            , current date
                                            , '{ModAnagrafica.CodTipoResidenza}' 
                                            , @Indirizzo  
                                            , @Civico
                                            , '{ModAnagrafica.StatoEsteroResidenza}' 
                                            , '{ModAnagrafica.Localita}'
                                            , '{ModAnagrafica.Cap}'
                                            , '{ModAnagrafica.SigproResidenza}'
                                            , 'S'
                                            , @Tel
                                            , @Email
                                            , current_timestamp
                                            , '{ModAnagrafica.CodiceFiscale}'
                                            , '{ModAnagrafica.CodComuneResidenza}' 
                                            , '{ModAnagrafica.ComuneResidenza}'
                                            , @Co
                                            , @Pec
                                            , @Cell
                                            , '{ModAnagrafica.Iban}')";

                    var result = objDataAccess.WriteTransactionDataWithParametersAndDontCall(insertIscd, CommandType.Text,
                        anagraficaParameters["@Indirizzo"], anagraficaParameters["@Civico"], anagraficaParameters["@Tel"], anagraficaParameters["@Email"],
                        anagraficaParameters["@Co"], anagraficaParameters["@Pec"], anagraficaParameters["@Cell"]);
                    if (result)
                    {
                        SuccesMsg = "Modifiche avvenute con successo";
                    }
                    else
                    {
                        this.objDataAccess.EndTransaction(false);
                        ErrorMsg = "Errore nel salvataggio";
                        return (Anagrafica)null;
                    }
                }

                /*string[] strArray1 = new string[11]
                {
          "UPDATE ISCD SET DENCOM='",
          ModAnagrafica.ComuneResidenza.Replace("'", "''"),
          "',TEL1='",
          ModAnagrafica.Telefono1,
          "', TEL2='",
          ModAnagrafica.Telefono2,
          "', FAX='",
          ModAnagrafica.Fax,
          "' WHERE MAT='",
          null,
          null
                };*/
                //num = ModAnagrafica.Mat;
                //strArray1[9] = num.ToString();
                //strArray1[10] = "'";


            }
            catch (Exception ex)
            {
                this.objDataAccess.EndTransaction(false);
                ErrorMsg = "Errore nel salvataggio";
                return (Anagrafica)null;
            }


            string updateIsctTitStud = $@"UPDATE ISCT SET TITSTU = '{ModAnagrafica.CodTitstu}', ULTAGG = CURRENT TIMESTAMP WHERE MAT = {ModAnagrafica.Mat}";
            var updateResult = this.objDataAccess.WriteTransactionData(updateIsctTitStud, CommandType.Text);
            if (updateResult)
            {
                SuccesMsg = "Modifiche avvenute con successo";
            }
            else
            {
                this.objDataAccess.EndTransaction(false);
                ErrorMsg = "Errore nel salvataggio";
                return (Anagrafica)null;
            }


            this.objDataAccess.EndTransaction(true);
            return ModAnagrafica;

            Dictionary<string, iDB2Parameter> CleanValuesAndPartiallyMapAnagraficaIntoParameters(Anagrafica anagrafica)
            {
                CleanAnagrafica();
                return PartiallyMapAnagraficaIntoParameters();

                void CleanAnagrafica()
                {
                    anagrafica.CodiceFiscale = anagrafica.CodiceFiscale?.Trim();
                    anagrafica.NumeroCivico = anagrafica.NumeroCivico?.Replace("'", "");
                    anagrafica.Email = anagrafica.Email?.Replace("'", "");
                    anagrafica.EmailCert = anagrafica.EmailCert?.Replace("'", "");
                }

                Dictionary<string, iDB2Parameter> PartiallyMapAnagraficaIntoParameters()
                {
                    var anagraficaConPswParameters = new Dictionary<string, iDB2Parameter>();

                    var civicoParameter = objDataAccess.CreateParameter("@Civico", iDB2DbType.iDB2VarChar, 20, ParameterDirection.Input, anagrafica.NumeroCivico);
                    var indirizzoParameter = objDataAccess.CreateParameter("@Indirizzo", iDB2DbType.iDB2VarChar, 80, ParameterDirection.Input, anagrafica.Indirizzo);
                    var coParameter = objDataAccess.CreateParameter("@Co", iDB2DbType.iDB2VarChar, 40, ParameterDirection.Input, anagrafica.Co);
                    var telefonoParameter = objDataAccess.CreateParameter("@Tel", iDB2DbType.iDB2VarChar, 13, ParameterDirection.Input, anagrafica.Telefono1);
                    var cellulareParameter = objDataAccess.CreateParameter("@Cell", iDB2DbType.iDB2VarChar, 20, ParameterDirection.Input, anagrafica.Cellulare);
                    var emailParameter = objDataAccess.CreateParameter("@Email", iDB2DbType.iDB2VarChar, 100, ParameterDirection.Input, anagrafica.Email);
                    var pecParameter = objDataAccess.CreateParameter("@Pec", iDB2DbType.iDB2VarChar, 100, ParameterDirection.Input, anagrafica.EmailCert);

                    anagraficaConPswParameters.Add(civicoParameter.ParameterName, civicoParameter);
                    anagraficaConPswParameters.Add(indirizzoParameter.ParameterName, indirizzoParameter);
                    anagraficaConPswParameters.Add(coParameter.ParameterName, coParameter);
                    anagraficaConPswParameters.Add(telefonoParameter.ParameterName, telefonoParameter);
                    anagraficaConPswParameters.Add(cellulareParameter.ParameterName, cellulareParameter);
                    anagraficaConPswParameters.Add(emailParameter.ParameterName, emailParameter);
                    anagraficaConPswParameters.Add(pecParameter.ParameterName, pecParameter);

                    return anagraficaConPswParameters;
                }
            }
        }
    }
}
