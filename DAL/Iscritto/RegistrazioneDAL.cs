using IBM.Data.DB2.iSeries;
using OCM.TFI.OCM.Iscritto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Web;
using TFI.CRYPTO.Crypto;
using TFI.DAL.ConnectorDB;
using TFI.OCM.AziendaConsulente;
using TFI.OCM.Iscritto;

namespace TFI.DAL.Iscritto
{
    public class RegistrazioneDAL
    {
        private readonly DataLayer objDataAccess = new DataLayer();

        public bool CheckSignUp(string matricola,
          string cf,
          ref string ErrorMsg)
        {
            try
            {
                string isctQuery = $"SELECT * FROM ISCT WHERE MAT = {matricola} AND CODFIS = '{cf}'";
                DataTable isctData = this.objDataAccess.GetDataTable(isctQuery);
                if (isctData == null || isctData.Rows.Count == 0)
                {
                    ErrorMsg = "Non è possibile effettuare la registrazione. Bisogna prima essere censiti dalla propria azienda.";
                    return false;
                }

                string utentiQuery = $"SELECT * FROM UTENTI WHERE CODFIS = '{cf}'";
                DataTable utentiData = this.objDataAccess.GetDataTable(utentiQuery);
                if (utentiData != null && utentiData.Rows.Count > 0)
                {
                    ErrorMsg = "La registrazione è gia stata effettuata.";
                    return false;
                }
                return true;
            }
            catch
            {
                ErrorMsg = "Errore nella registrazione, controllare CF/Matricola. Si ricorda che la matricola deve essere numerica.";
                return false;
            }
        }

        public bool ConsolidaRegistrazione(AnagraficaConPwd anagrafica, ref string errorMSG, ref string successMSG)
        {
            var anagraficaParameters = CleanValuesAndPartiallyMapAnagraficaIntoParameters(anagrafica);

            objDataAccess.StartTransaction();
            try
            {
                WriteISCTWEB(anagrafica, objDataAccess, anagraficaParameters);

                if (!InsertTitStuISCT(anagrafica, objDataAccess))                
                    return EndTransactionAndSetOutcomeMsg(false, ref errorMSG, "Errore nel salvataggio TitStu");                

                if (!WritePrivacy(anagrafica, objDataAccess))
                    return EndTransactionAndSetOutcomeMsg(false, ref errorMSG, "Errore nel salvataggio Privacy");

                if (!InsertPwd(anagrafica, objDataAccess))
                    return EndTransactionAndSetOutcomeMsg(false, ref errorMSG, "Errore nel salvataggio Utepin");                

                if (!InsertIntoUtenti(anagrafica, objDataAccess))
                    return EndTransactionAndSetOutcomeMsg(false, ref errorMSG, "Errore nel salvataggio UTENTI");
            }
            catch
            {
                return EndTransactionAndSetOutcomeMsg(false, ref errorMSG, "Errore nel salvataggio");
            }

            return EndTransactionAndSetOutcomeMsg(true, ref successMSG, "Registrazione effettuata, riceverai una mail.");
        }

        public bool ConsolidaRegistrazioneModificaDati(AnagraficaConPwd anagrafica, ref string errorMSG, ref string successMSG)
        {
            var anagraficaParameters = CleanValuesAndPartiallyMapAnagraficaIntoParameters(anagrafica);

            objDataAccess.StartTransaction();
            try
            {
                WriteISCTWEB(anagrafica, objDataAccess, anagraficaParameters);

                if (!InsertTitStuISCT(anagrafica, objDataAccess))
                    return EndTransactionAndSetOutcomeMsg(false, ref errorMSG, "Errore nel salvataggio TitStu");

                if (!UpdateOrInsertISCD(anagrafica, objDataAccess, anagraficaParameters))
                    return EndTransactionAndSetOutcomeMsg(false, ref errorMSG, "Errore nel salvataggio ISCD");

                if (!WritePrivacy(anagrafica, objDataAccess))
                    return EndTransactionAndSetOutcomeMsg(false, ref errorMSG, "Errore nel salvataggio Privacy");

                if (!InsertPwd(anagrafica, objDataAccess))
                    return EndTransactionAndSetOutcomeMsg(false, ref errorMSG, "Errore nel salvataggio Utepin");

                if (!InsertIntoUtenti(anagrafica, objDataAccess))
                    return EndTransactionAndSetOutcomeMsg(false, ref errorMSG, "Errore nel salvataggio UTENTI");
            }
            catch
            {
                return EndTransactionAndSetOutcomeMsg(false, ref errorMSG, "Errore nel salvataggio");
            }

            return EndTransactionAndSetOutcomeMsg(true, ref successMSG, "Registrazione effettuata, riceverai una mail.");

        }

        private bool EndTransactionAndSetOutcomeMsg(bool outcome, ref string outcomeMsg, string outcomeMsgContent)
        {
            objDataAccess.EndTransaction(outcome);
            outcomeMsg = outcomeMsgContent;
            return outcome;
        }

        private Dictionary<string, iDB2Parameter> CleanValuesAndPartiallyMapAnagraficaIntoParameters(AnagraficaConPwd anagrafica)
        {
            CleanAnagrafica();
            return PartiallyMapAnagraficaIntoParameters();

            void CleanAnagrafica()
            {
                anagrafica.CodiceFiscale = anagrafica.CodiceFiscale?.Trim();
                anagrafica.NumeroCivico= anagrafica.NumeroCivico?.Replace("'", "");
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
                var numeroDocumentoParameter = objDataAccess.CreateParameter("@NumDoc", iDB2DbType.iDB2VarChar, 20, ParameterDirection.Input, anagrafica.Numero_Documento);

                anagraficaConPswParameters.Add(civicoParameter.ParameterName, civicoParameter);
                anagraficaConPswParameters.Add(indirizzoParameter.ParameterName, indirizzoParameter);
                anagraficaConPswParameters.Add(coParameter.ParameterName, coParameter);
                anagraficaConPswParameters.Add(telefonoParameter.ParameterName, telefonoParameter);
                anagraficaConPswParameters.Add(cellulareParameter.ParameterName, cellulareParameter);
                anagraficaConPswParameters.Add(emailParameter.ParameterName, emailParameter);
                anagraficaConPswParameters.Add(pecParameter.ParameterName, pecParameter);
                anagraficaConPswParameters.Add(numeroDocumentoParameter.ParameterName, numeroDocumentoParameter);

                return anagraficaConPswParameters;
            }
        }

        public static string DoublePeakForSql(string strValue)
        {
            strValue = strValue.Replace("'", "''");
            strValue = "'" + strValue.Trim() + "'";
            return strValue;
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

        private void WriteISCTWEB(AnagraficaConPwd anagrafica, DataLayer objDataAccess, Dictionary<string, iDB2Parameter> anagraficaParameters)
        {
            if (string.IsNullOrEmpty(anagrafica.StatoEsteroResidenza))
            {
                var insertIsctwebQuery = "INSERT INTO ISCTWEB (MAT, COGNOME, NOME, CODFIS, DATNAS, SES, CODCOMNAS, STAESTNAS, CODDUG, CODCOM, NUMCIV, DENSTAEST, IND, DENLOC, CAP, SIGPRO, CELL, EMAIL, EMAILCERT, CODTITSTU, ULTAGG, UTEAGG) " +
                                            @$"VALUES (
                                                      {anagrafica.Mat}
                                                    , '{anagrafica.Cognome}'
                                                    , '{anagrafica.Nome}'
                                                    , '{anagrafica.CodiceFiscale}'
                                                    , '{anagrafica.DataNascita.ToString("yyyy-MM-dd")}'
                                                    , '{anagrafica.Sesso}'
                                                    , '{anagrafica.CodComuneNascita}'
                                                    , '{anagrafica.StatoEsteroNascita}'
                                                    , '{anagrafica.CodTipoResidenza}'
                                                    , (SELECT CODCOM FROM CODCOM WHERE DENCOM='{anagrafica.ComuneResidenza}')
                                                    , @Civico
                                                    , '{anagrafica.StatoEsteroResidenza}'
                                                    , @Indirizzo
                                                    , '{anagrafica.Localita}'
                                                    , '{anagrafica.Cap}'
                                                    , (SELECT SIGPRO FROM CODCOM WHERE DENCOM='{anagrafica.ComuneResidenza.Replace("'", "''")}')
                                                    , @Cell
                                                    , @Email
                                                    , @Pec
                                                    , '{anagrafica.CodTitstu}'
                                                    , current_timestamp
                                                    , '{anagrafica.CodiceFiscale}')";

                objDataAccess.WriteTransactionDataWithParametersAndDontCall(insertIsctwebQuery, CommandType.Text, 
                    anagraficaParameters["@Civico"], anagraficaParameters["@Indirizzo"], anagraficaParameters["@Cell"], anagraficaParameters["@Email"], anagraficaParameters["@Pec"]);
                return;
            }
            var insertIsctwebQueryEstero = "INSERT INTO ISCTWEB (MAT, COGNOME, NOME, CODFIS, DATNAS, SES, CODCOMNAS, STAESTNAS, CODDUG, CODCOM, NUMCIV, DENSTAEST, IND, DENLOC, CAP, SIGPRO, CELL, EMAIL, EMAILCERT, CODTITSTU, ULTAGG, UTEAGG) " +
                                        @$"VALUES (
                                                      {anagrafica.Mat}
                                                    , '{anagrafica.Cognome}'
                                                    , '{anagrafica.Nome}'
                                                    , '{anagrafica.CodiceFiscale}'
                                                    , '{anagrafica.DataNascita.ToString("yyyy-MM-dd")}'
                                                    , '{anagrafica.Sesso}'
                                                    , '{anagrafica.CodComuneNascita}'
                                                    , '{anagrafica.StatoEsteroNascita}'
                                                    , '{anagrafica.CodTipoResidenza}'
                                                    , (SELECT CODCOM FROM COM_ESTERO WHERE DENCOM='{anagrafica.StatoEsteroResidenza}')
                                                    , @Civico
                                                    , '{anagrafica.StatoEsteroResidenza}'
                                                    , @Indirizzo
                                                    , '{anagrafica.Localita}'
                                                    , '{anagrafica.Cap}'
                                                    , 'EE'
                                                    , @Cell
                                                    , @Email
                                                    , @Pec
                                                    , '{anagrafica.CodTitstu}'
                                                    , current_timestamp
                                                    , '{anagrafica.CodiceFiscale}')";

            objDataAccess.WriteTransactionDataWithParametersAndDontCall(insertIsctwebQueryEstero, CommandType.Text, 
                anagraficaParameters["@Civico"], anagraficaParameters["@Indirizzo"], anagraficaParameters["@Cell"], anagraficaParameters["@Email"], anagraficaParameters["@Pec"]);
        }

        private bool WritePrivacy(AnagraficaConPwd anagrafica, DataLayer objDataAccess)
        {
            string privacyQuery = $"INSERT INTO TB_PRIVACY (MAT, PRIVACY, TIPO, DATINI, DATFIN, DATINS, UTEINS) VALUES ({anagrafica.Mat},'S', 'I', CURRENT_DATE, '9999-12-31', CURRENT_TIMESTAMP, '{anagrafica.CodiceFiscale}')";
            var result3 = objDataAccess.WriteTransactionData(privacyQuery, CommandType.Text);
            if (result3)
            {
                if (!objDataAccess.WriteTransactionData($"UPDATE ISCT SET FLGAPP = 'M' WHERE MAT = {anagrafica.Mat}", CommandType.Text))
                {
                    return false;
                }
            }
            return true;
        }
        private bool UpdateOrInsertISCD(AnagraficaConPwd anagrafica, DataLayer objDataAccess, Dictionary<string, iDB2Parameter> anagraficaParameters)
        {
            string ctrlUpdate = $"SELECT * FROM ISCD WHERE MAT = {anagrafica.Mat} AND DATINI = '{DateTime.Now.ToString("yyyy-MM-dd")}'";
            DataTable dtCtrlUpdate = this.objDataAccess.GetDataTable(ctrlUpdate);
            if (dtCtrlUpdate != null && dtCtrlUpdate.Rows.Count > 0)
            {
                string updateIscd = $@"UPDATE ISCD SET 
                                           CODDUG = '{anagrafica.CodTipoResidenza}'
                                            , IND = @Indirizzo
                                            , NUMCIV = @Civico
                                            , DENSTAEST = '{anagrafica.StatoEsteroResidenza}'
                                            , DENLOC = '{anagrafica.Localita}'
                                            , CAP = '{anagrafica.Cap}'
                                            , SIGPRO = '{anagrafica.SigproResidenza}'
                                            , AGGMAN = 'S'
                                            , TEL1 = @Tel
                                            , EMAIL = @Email
                                            , ULTAGG = CURRENT TIMESTAMP
                                            , UTEAGG = '{anagrafica.CodiceFiscale}'
                                            , CODCOM = '{anagrafica.CodComuneResidenza}'
                                            , DENCOM = '{anagrafica.ComuneResidenza}'
                                            , CO = @Co
                                            , EMAILCERT = @Pec
                                            , CELL = @Cell
                                            , IBAN = '{anagrafica.Iban}'
                                    WHERE MAT = {anagrafica.Mat} AND DATINI = '{DateTime.Now.ToString("yyyy-MM-dd")}'";

                var result = objDataAccess.WriteTransactionDataWithParametersAndDontCall(updateIscd, CommandType.Text, 
                    anagraficaParameters["@Indirizzo"], anagraficaParameters["@Civico"], anagraficaParameters["@Tel"], anagraficaParameters["@Email"], 
                    anagraficaParameters["@Co"], anagraficaParameters["@Pec"], anagraficaParameters["@Cell"]);
                return result;
            }
            else
            {
                string insertIscd = $@"INSERT INTO ISCD (MAT, DATINI, CODDUG, IND, NUMCIV, DENSTAEST, DENLOC, CAP, SIGPRO, AGGMAN, TEL1
	                                   , EMAIL, ULTAGG, UTEAGG, CODCOM, DENCOM, CO, EMAILCERT, CELL, IBAN)
                                    VALUES ({anagrafica.Mat} 
                                            , current date
                                            , '{anagrafica.CodTipoResidenza}' 
                                            , @Indirizzo  
                                            , @Civico
                                            , '{anagrafica.StatoEsteroResidenza}' 
                                            , '{anagrafica.Localita}'
                                            , '{anagrafica.Cap}'
                                            , '{anagrafica.SigproResidenza}'
                                            , 'S'
                                            , @Tel
                                            , @Email
                                            , current_timestamp
                                            , '{anagrafica.CodiceFiscale}'
                                            , '{anagrafica.CodComuneResidenza}' 
                                            , '{anagrafica.ComuneResidenza}'
                                            , @Co
                                            , @Pec
                                            , @Cell
                                            , '{anagrafica.Iban}')";

                var result = objDataAccess.WriteTransactionDataWithParametersAndDontCall(insertIscd, CommandType.Text, 
                    anagraficaParameters["@Indirizzo"], anagraficaParameters["@Civico"], anagraficaParameters["@Tel"], anagraficaParameters["@Email"], 
                    anagraficaParameters["@Co"], anagraficaParameters["@Pec"], anagraficaParameters["@Cell"]);
                return result;
            }
        }
        private bool InsertTitStuISCT(AnagraficaConPwd anagrafica, DataLayer objDataAccess)
        {
            string updateIsctTitStud = $@"UPDATE ISCT SET TITSTU = '{anagrafica.CodTitstu}', ULTAGG = CURRENT TIMESTAMP WHERE MAT = {anagrafica.Mat}";
            var updateResult = objDataAccess.WriteTransactionData(updateIsctTitStud, CommandType.Text);
            return updateResult;
        }
        private bool InsertPwd(AnagraficaConPwd anagrafica, DataLayer objDataAccess)
        {
            string startDateCurrVersion = DateTime.Now.Date.ToString("yyyy-MM-dd");
            string endDateCurrVersion = DateTime.Now.Date.AddDays(90).ToString("yyyy-MM-dd");
            string codUte = DoublePeakForSql(anagrafica.CodiceFiscale);

            var passwordCryptata = Cypher.CryptPassword(anagrafica.Password, "", "");
            var passwordCryptataParameter = objDataAccess.CreateParameter("@Password", iDB2DbType.iDB2VarChar, 500, ParameterDirection.Input, passwordCryptata);

            string insertNewPwdCommand = "INSERT INTO UTEPIN (CODUTE, PIN, DATINI, DATFIN, STAPIN, ULTAGG, UTEAGG) VALUES (" +
                codUte + ", @Password, '" + startDateCurrVersion + "', '" + endDateCurrVersion + "', 'A', CURRENT_TIMESTAMP, " + codUte + ")";
            return objDataAccess.WriteTransactionDataWithParametersAndDontCall(insertNewPwdCommand, CommandType.Text, passwordCryptataParameter);
        }

        private bool InsertIntoUtenti(AnagraficaConPwd anagrafica, DataLayer objDataAccess)
        {
            return objDataAccess.WriteTransactionData("Insert Into UTENTI (CODUTE , DENUTE,  CODTIPUTE, CODFIS, ULTAGG , UTEAGG)"
                        + " Values ( '" + anagrafica.CodiceFiscale + "', '" + anagrafica.Cognome + " " + anagrafica.Nome + "', "
                        + "'I', '" + anagrafica.CodiceFiscale + "', " + " CURRENT_TIMESTAMP, '" + anagrafica.CodiceFiscale + "')"
                        , CommandType.Text);
        }
    }
}
