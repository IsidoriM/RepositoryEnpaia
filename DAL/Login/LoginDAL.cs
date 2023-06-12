// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Login.LoginDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using log4net;
using System;
using System.Data;
using System.Web;
using TFI.DAL.ConnectorDB;
using Utilities;
using static Utilities.DbParameters;


namespace TFI.DAL.Login
{
    public static class LoginDAL
    {
        private static readonly ILog log = LogManager.GetLogger("RollingFile");
        private static readonly ILog TrackLog = LogManager.GetLogger("Track");

        public static int GetMatricola(TFI.OCM.Utente.Utente utente)
        {
            DataLayer dataLayer = new DataLayer();
            try
            {
                string strSQL = "SELECT DISTINCT MAT FROM RAPLAV WHERE VALUE(CODCAUCES, 0) <> 50 AND MAT IN (SELECT MAT FROM ISCT " + "WHERE CODFIS = '" + (!(utente.Tipo == "E") ? utente.CodFiscale : utente.Username) + "' AND CURRENT_DATE BETWEEN DATISC AND VALUE (DATCHIISC, '9999-12-31'))";
                string str = dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
                return !string.IsNullOrEmpty(str) ? Convert.ToInt32(str) : 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static string GetRagioneSociale(string codUte)
        {
            DataLayer dataLayer = new DataLayer();
            try
            {
                string strSQL = "SELECT az.ragsoc FROM AZI az INNER JOIN AZIUTE au ON az.CODPOS = au.CODPOS WHERE au.CODUTE = '" + codUte + "'";
                return dataLayer.Get1ValueFromSQL(strSQL, CommandType.Text);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static OCM.Utente.Utente GetUserNoRetired(string username, string tipoUtente)
        {
            try
            {
                DataLayer dataLayer = new DataLayer();
                DataTable tableWithParameters;
                iDB2Parameter parameter1;
                iDB2Parameter parameter2;
                string query;
                if (tipoUtente == "A")
                {
                    parameter1 = dataLayer.CreateParameter(DbParameters.PartitaIva, iDB2DbType.iDB2Char, 20, ParameterDirection.Input, username);
                    parameter2 = dataLayer.CreateParameter(DbParameters.CodiceFiscale, iDB2DbType.iDB2Char, 20, ParameterDirection.Input, username);
                    query = "SELECT u.CODUTE, u.DENUTE, u.CODTIPUTE, up.PIN, up.DATFIN, a.CODPOS, u.CODFIS " +
                        "FROM UTENTI u " +
                        "INNER JOIN UTEPIN up ON u.CODUTE = up.CODUTE AND u.CODTIPUTE = 'A' " +
                        "LEFT JOIN AZIUTE a ON u.CODUTE = a.CODUTE " +
                        "INNER JOIN AZI az ON a.CODPOS = az.CODPOS " +
                        $"WHERE az.PARIVA = {DbParameters.PartitaIva} OR az.CODFIS = {DbParameters.CodiceFiscale} " +
                        "ORDER BY DATFIN DESC FETCH FIRST 1 ROW ONLY";
                }
                else if (tipoUtente == "C")
                {
                    parameter1 = dataLayer.CreateParameter(DbParameters.Username, iDB2DbType.iDB2Char, 20, ParameterDirection.Input, username);
                    parameter2 = dataLayer.CreateParameter(DbParameters.TipoUtente, iDB2DbType.iDB2Char, 20, ParameterDirection.Input, tipoUtente);
                    query =
                        "SELECT u.CODUTE, u.DENUTE, u.CODTIPUTE, up.PIN, up.DATFIN, u.CODFIS, a.CODTER, a.RAGSOC " +
                        "FROM UTENTI u INNER JOIN UTEPIN up ON u.CODUTE = up.CODUTE LEFT JOIN ASSTER a ON (a.CODFIS = u.CODUTE OR u.CODUTE = a.PARIVA) " +
                        $"WHERE (u.CODUTE = {DbParameters.Username} OR u.CODFIS = {DbParameters.Username}) AND u.CODTIPUTE = {DbParameters.TipoUtente} ORDER BY DATFIN DESC FETCH FIRST 1 ROW ONLY";
                }
                else
                {
                    parameter1 = dataLayer.CreateParameter(DbParameters.Username, iDB2DbType.iDB2Char, 20, ParameterDirection.Input, username);
                    parameter2 = dataLayer.CreateParameter(DbParameters.TipoUtente, iDB2DbType.iDB2Char, 20, ParameterDirection.Input, tipoUtente);
                    query = "SELECT u.CODUTE, u.DENUTE, u.CODTIPUTE, up.PIN, up.DATFIN, a.CODPOS, u.CODFIS " +
                        "FROM UTENTI u INNER JOIN UTEPIN up ON u.CODUTE = up.CODUTE LEFT JOIN AZIUTE a ON u.CODUTE = a.CODUTE " +
                        $"WHERE u.CODUTE = {DbParameters.Username} AND u.CODTIPUTE = {DbParameters.TipoUtente} ORDER BY DATFIN DESC FETCH FIRST 1 ROW ONLY";
                }

                tableWithParameters = dataLayer.GetDataTableWithParameters(query, parameter1, parameter1, parameter2);
                if (tableWithParameters.Rows.Count <= 0)
                    return null;

                DataRow recordFromQuery = tableWithParameters.Rows[0];

                if (tipoUtente == "C")
                    return new OCM.Utente.Utente(
                        recordFromQuery.ElementAt("CODUTE"),
                        recordFromQuery.ElementAt("DENUTE"),
                        recordFromQuery.ElementAt("CODTIPUTE"),
                        recordFromQuery.ElementAt("PIN"),
                        recordFromQuery.ElementAt("DATFIN"),
                        recordFromQuery.ElementAt("CODFIS"),
                        $"{recordFromQuery.ElementAt("CODTER")} - {recordFromQuery.ElementAt("RAGSOC")}",
                        recordFromQuery.ElementAt("CODTER"));
                
                return new OCM.Utente.Utente(
                    recordFromQuery.ElementAt("CODUTE"),
                    recordFromQuery.ElementAt("DENUTE"),
                    recordFromQuery.ElementAt("CODTIPUTE"),
                    recordFromQuery.ElementAt("PIN"),
                    recordFromQuery.ElementAt("DATFIN"),
                    recordFromQuery.ElementAt("CODPOS"),
                    recordFromQuery.ElementAt("CODFIS"),
                    true);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool VerifyPassword(string password)
        {
            try
            {
                DataLayer dataLayer = new DataLayer();
                string empty = string.Empty;
                iDB2Parameter parameter = dataLayer.CreateParameter("@password", iDB2DbType.iDB2Char, 500, ParameterDirection.Input, password);
                string strSQL = "SELECT * FROM UTEPIN WHERE PIN = @password AND DATFIN >= CURRENT_DATE AND STAPIN = 'A'";
                return dataLayer.GetDataTableWithParameters(strSQL, parameter).Rows.Count > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string DoublePeakForSql(string strValue)
        {
            strValue = strValue.Replace("'", "''");
            strValue = "'" + strValue.Trim() + "'";
            return strValue;
        }

        public static void ModificaPWD(string VecchiaPassword, string NuovaPassword, string tipoUtente)
        {
            DataLayer dataLayer = new DataLayer();
            dataLayer.StartTransaction();
            TFI.OCM.Utente.Utente utente = (TFI.OCM.Utente.Utente)HttpContext.Current.Session["utente"];
            try
            {
                var oldPasswordParameter = dataLayer.CreateParameter("@VecchiaPassword", iDB2DbType.iDB2Char, 500, ParameterDirection.Input, VecchiaPassword);
                var coduteParameter = dataLayer.CreateParameter("@Codute", iDB2DbType.iDB2Char, 20, ParameterDirection.Input, utente.Username);

                if (VecchiaPassword == NuovaPassword)
                {
                    HttpContext.Current.Items["errorMessage"] = "Non è possibile inserire una password uguale alla password precedente";
                    return;
                }

                string strSQL1 = "SELECT A.PIN, A.DATINI, A.DATFIN, B.CODUTE, B.DENUTE, B.CODTIPUTE, CURRENT_DATE AS TODAY, " +
                    "CURRENT_TIME AS NOW, (SELECT COUNT(Codpos) FROM AZIUTE WHERE Codute = @Codute) AS NUM_AZIENDE, " +
                    "E.CODPOS, E.RAGSOC FROM UTEPIN A LEFT JOIN UTENTI B ON A.CODUTE = B.CODUTE " + "LEFT JOIN AZIUTE D ON B.CODUTE = D.CODUTE" +
                    " LEFT JOIN AZI E ON D.CODPOS = E.CODPOS WHERE B.CODUTE = @Codute AND A.PIN = @VecchiaPassword AND A.STAPIN <> 'D' AND " +
                    "current_date BETWEEN A.DATINI AND A.DATFIN";

                DataTable dataTable2 = dataLayer.GetDataTableWithParameters(strSQL1, coduteParameter, coduteParameter, oldPasswordParameter);
                if (dataTable2.Rows.Count > 0)
                {
                    string str = Convert.ToDateTime(dataTable2.Rows[0]["DATINI"]).ToString("yyyy-MM-dd").Substring(0, 10);
                    string endDatePrevVersion = DateTime.Now.Date.AddDays(-1).ToString("yyyy-MM-dd");
                    string startDateCurrVersion = DateTime.Now.Date.ToString("yyyy-MM-dd");
                    string endDateCurrVersion = DateTime.Now.Date.AddDays(90).ToString("yyyy-MM-dd");

                    var datFinParameter = dataLayer.CreateParameter("@datFin", iDB2DbType.iDB2Date, 2000, ParameterDirection.Input, endDatePrevVersion);
                    var datIniParameter = dataLayer.CreateParameter("@datIni", iDB2DbType.iDB2Date, 2000, ParameterDirection.Input, str);
                    var endDateCurrVersionParameter = dataLayer.CreateParameter("@newDatFin", iDB2DbType.iDB2Date, 2000, ParameterDirection.Input, endDateCurrVersion);
                    var startDateCurrVersionParameter = dataLayer.CreateParameter("@newDatIni", iDB2DbType.iDB2Date, 2000, ParameterDirection.Input, startDateCurrVersion);
                    var newPasswordParameter = dataLayer.CreateParameter("@NuovaPassword", iDB2DbType.iDB2Char, 500, ParameterDirection.Input, NuovaPassword);

                    string updatePrevPwdCommand = "UPDATE UTEPIN SET DATFIN = @datFin WHERE CODUTE = @Codute AND DATINI = @datIni";
                    bool flag2 = dataLayer.WriteTransactionDataWithParametersAndDontCall(updatePrevPwdCommand, CommandType.Text, datFinParameter, coduteParameter, datIniParameter);

                    string insertNewPwdCommand = "INSERT INTO UTEPIN (CODUTE, PIN, DATINI, DATFIN, STAPIN, ULTAGG, UTEAGG) VALUES " +
                        "(@Codute, @NuovaPassword, @newDatIni, @newDatFin, 'A', CURRENT_TIMESTAMP, @Codute)";
                    bool flag1 = dataLayer.WriteTransactionDataWithParametersAndDontCall(insertNewPwdCommand, CommandType.Text, coduteParameter, newPasswordParameter, startDateCurrVersionParameter,
                        endDateCurrVersionParameter, coduteParameter);

                    bool flag3 = true;
                    if (tipoUtente == "I")
                        flag3 = dataLayer.WriteTransactionDataWithParametersAndDontCall("UPDATE ISCT SET FLGAPP = 'M' WHERE CODFIS = @Codute", CommandType.Text, coduteParameter);
                    if (flag1 && flag2 && flag3)
                    {
                        HttpContext.Current.Items["successMessage"] = "Password modificata correttamente!";
                        utente.Username = null;
                        dataLayer.EndTransaction(true);
                    }
                    else
                    {
                        HttpContext.Current.Items["errorMessage"] = "Password non modificata!";
                        dataLayer.EndTransaction(false);
                    }
                }
                else
                {
                    HttpContext.Current.Items["errorMessage"] = "Vecchia Password non valida!";
                    dataLayer.EndTransaction(false);
                }
            }
            catch
            {
                HttpContext.Current.Items["errorMessage"] = "Vecchia Password non valida!";
                dataLayer.EndTransaction(false);
            }
        }

        public static bool CiSonoDeleghePerConsulente(string codTer)
        {
            try
            {
                DataLayer dataLayer = new DataLayer();
                iDB2Parameter codTerParameter = dataLayer.CreateParameter(DbParameters.CodiceTerritoriale, iDB2DbType.iDB2VarChar, 50, ParameterDirection.Input, codTer);
                string strSQL = $"SELECT d.* FROM DELEGHE d INNER JOIN ASSTER a ON d.CODTER = a.CODTER WHERE d.CODTER = {DbParameters.CodiceTerritoriale}";
                DataTable tableWithParameters = dataLayer.GetDataTableWithParameters(strSQL, codTerParameter);
                if (tableWithParameters.Rows.Count > 0)
                {
                    foreach (DataRow row in (InternalDataCollectionBase)tableWithParameters.Rows)
                    {
                        string s1 = !DBNull.Value.Equals(row["DATINI"]) ? row["DATINI"].ToString() : string.Empty;
                        string s2 = !DBNull.Value.Equals(row["DATFIN"]) ? row["DATFIN"].ToString() : string.Empty;
                        int int32 = Convert.ToInt32(row["FLGATT"]);
                        if (s2 != string.Empty && s1 != string.Empty)
                        {
                            DateTime dateTime = DateTime.Parse(s2);
                            if (DateTime.Parse(s1) <= DateTime.Now && dateTime >= DateTime.Now && int32 == 1)
                                return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static bool ControllaOTPDeleghe(string otp, string username)
        {
            try
            {
                string str1 = string.Empty;
                DataLayer dataLayer = new DataLayer();
                iDB2Parameter parameter = dataLayer.CreateParameter("@usernameP", iDB2DbType.iDB2VarChar, 50, ParameterDirection.Input, username);
                string strSQL1 = "SELECT d.CODTER, d.CODPOS, d.DATINI, d.FLGATT, u.OTPDLG FROM DELEGHE d INNER JOIN ASSTER a ON d.CODTER = a.CODTER " + "INNER JOIN AZIUTE a2 ON (a2.CODPOS = d.CODPOS AND a2.CODUTE = LPAD(d.CODPOS, 8, '0')) " + "INNER JOIN UTENTI u ON u.CODUTE = a2.CODUTE WHERE a.CODUTE = @usernameP";
                DataTable tableWithParameters1 = dataLayer.GetDataTableWithParameters(strSQL1, parameter);
                string strSQL2 = "SELECT OTPDLG FROM UTENTI WHERE CODUTE = @usernameP";
                DataTable tableWithParameters2 = dataLayer.GetDataTableWithParameters(strSQL2, parameter);
                if (tableWithParameters2.Rows.Count > 0)
                    str1 = !DBNull.Value.Equals(tableWithParameters2.Rows[0]["OTPDLG"]) ? tableWithParameters2.Rows[0]["OTPDLG"].ToString() : string.Empty;
                if (tableWithParameters1.Rows.Count <= 0)
                    return false;
                string str2 = !DBNull.Value.Equals(tableWithParameters1.Rows[0]["CODTER"]) ? tableWithParameters1.Rows[0]["CODTER"].ToString() : string.Empty;
                string str3 = !DBNull.Value.Equals(tableWithParameters1.Rows[0]["CODPOS"]) ? tableWithParameters1.Rows[0]["CODPOS"].ToString() : string.Empty;
                string str4 = !DBNull.Value.Equals(tableWithParameters1.Rows[0]["DATINI"]) ? Convert.ToDateTime(tableWithParameters1.Rows[0]["DATINI"]).ToString("yyyy-MM-dd") : string.Empty;
                if (!(str1 != string.Empty) || !(otp == str1))
                    return false;
                string strSQL3 = "UPDATE DELEGHE SET FLGATT = 1 WHERE CODTER = '" + str2 + "' AND CODPOS = '" + str3 + "' AND DATINI = '" + str4 + "'";
                dataLayer.WriteData(strSQL3, CommandType.Text);
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static bool CheckPasswordValidity(string encryptedPassword, iDB2Parameter coduteParameter)
        {
            DataLayer dataLayer = new DataLayer();

            var codUte = coduteParameter.ParameterName;
            var lastPasswordSqlQuery = $"SELECT PIN FROM UTEPIN WHERE CODUTE = {codUte} ORDER BY DATINI DESC";
            var lastPassword = dataLayer.GetDataTableWithParameters(lastPasswordSqlQuery, coduteParameter).Rows[0]["PIN"].ToString().Trim();

            return encryptedPassword == lastPassword;
        }

        public static void ResetPasswordIscritto(string username, string encryptedPassword)
        {
            DataLayer dataLayer = new DataLayer();
            dataLayer.StartTransaction();
            try
            {
                var coduteParameter = dataLayer.CreateParameter("@Codute", iDB2DbType.iDB2Char, 20, ParameterDirection.Input, username);
                var isPinAlreadyModifiedTodaySqlQuery = "SELECT * FROM UTEPIN WHERE CODUTE = @Codute AND DATE(ULTAGG) = CURRENT_DATE";
                var isPinAlreadyModifiedToday = dataLayer.GetDataTableWithParameters(isPinAlreadyModifiedTodaySqlQuery, coduteParameter).Rows.Count > 0;

                var result = ResetPassword(username, encryptedPassword, dataLayer, isPinAlreadyModifiedToday);

                if (result && isPinAlreadyModifiedToday)
                {
                    var updateIsctSqlCmd = "UPDATE ISCT SET FLGAPP = 'M' WHERE CODFIS = @Codute";
                    result = dataLayer.WriteTransactionDataWithParametersAndDontCall(updateIsctSqlCmd, CommandType.Text, coduteParameter);
                }

                if (result)
                    HttpContext.Current.Items["successMessage"] = "Password modificata correttamente!";
                else
                    HttpContext.Current.Items["errorMessage"] = "Operazione non riuscita" + HttpContext.Current.Items["errorMessage"];

                dataLayer.EndTransaction(result);
            }
            catch
            {
                HttpContext.Current.Items["errorMessage"] = "Operazione non riuscita";
                dataLayer.EndTransaction(false);
            }
        }

        public static string GetEmailIscritto(string username)
        {
            var dataLayer = new DataLayer();

            var codFisParameter = dataLayer.CreateParameter("@Codfis", iDB2DbType.iDB2VarChar, 16, ParameterDirection.Input, username);
            var getMatricolaQuery = "SELECT MAT FROM ISCT WHERE CODFIS = @Codfis";
            var dataTable1 = dataLayer.GetDataTableWithParameters(getMatricolaQuery, codFisParameter);
            string matricola = dataTable1.Rows[0]["MAT"].ToString();

            var matParameter = dataLayer.CreateParameter("@Mat", iDB2DbType.iDB2Decimal, 16, ParameterDirection.Input, matricola);
            var getEmailsqlQuery = "SELECT U.EMAIL FROM ISCD U WHERE U.MAT = @Mat AND DATINI = (SELECT MAX(DATINI) FROM ISCD WHERE MAT = @Mat)";
            var dataTable2 = dataLayer.GetDataTableWithParameters(getEmailsqlQuery, matParameter, matParameter);

            if (dataTable2.Rows.Count > 0)
                return dataTable2.Rows[0]["EMAIL"].ToString();

            return default;
        }

        public static (string email, string codPos) GetDettaglioAzienda(string identifier)
        {
            var dataLayer = new DataLayer();

            var identifierParameter = dataLayer.CreateParameter("@Identifier", iDB2DbType.iDB2VarChar, 16, ParameterDirection.Input, identifier);
            var getCodPosSqlQuery = "SELECT u.CODUTE, u.DENUTE, u.CODTIPUTE, up.PIN, up.DATFIN, a.CODPOS, u.CODFIS FROM UTENTI u INNER JOIN UTEPIN up ON u.CODUTE = up.CODUTE AND u.CODTIPUTE = 'A' " +
                                    "LEFT JOIN AZIUTE a ON u.CODUTE = a.CODUTE INNER JOIN AZI az ON a.CODPOS = az.CODPOS WHERE az.PARIVA = @pIva OR az.CODFIS = @codFis " +
                                    "ORDER BY DATFIN DESC FETCH FIRST 1 ROW ONLY";
            var codPos = dataLayer.GetDataTableWithParameters(getCodPosSqlQuery, identifierParameter, identifierParameter).Rows[0]["CODPOS"].ToString();
            var codUte = dataLayer.GetDataTableWithParameters(getCodPosSqlQuery, identifierParameter, identifierParameter).Rows[0]["CODUTE"].ToString();

            var codPosParameter = dataLayer.CreateParameter("@Codpos", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, codPos);
            var getEmailSqlQuery = "SELECT EMAIL FROM AZEMAIL WHERE CODPOS = @Codpos AND DATINI = (SELECT MAX(DATINI) FROM AZEMAIL WHERE CODPOS = @Codpos)";
            var getEmail = dataLayer.GetDataTableWithParameters(getEmailSqlQuery, codPosParameter, codPosParameter);

            return getEmail.Rows.Count > 0 ? (getEmail.Rows[0]["EMAIL"].ToString(), codUte) : default;
        }

        public static string GetEmailConsulente(string codiceTerritoriale)
        {
            var dataLayer = new DataLayer();
            var codiceTerritorialeParameter = dataLayer.CreateParameter(CodiceTerritoriale, iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input, codiceTerritoriale);
            var getEmailQuery = $"SELECT EMAIL FROM ASSTER WHERE CODTER = {CodiceTerritoriale}";
            var dataTable1 = dataLayer.GetDataTableWithParameters(getEmailQuery, codiceTerritorialeParameter);
            
            return dataTable1.Rows.Count > 0 ? dataTable1.Rows[0].ElementAt("EMAIL") : default;
        }
        
        public static void ResetPasswordAziendaConsulente(string identifier, string encryptedPassword)
        {
            DataLayer dataLayer = new DataLayer();
            dataLayer.StartTransaction();
            try
            {
                var coduteParameter = dataLayer.CreateParameter("@Codute", iDB2DbType.iDB2Char, 20, ParameterDirection.Input, identifier);
                var isPinAlreadyModifiedTodaySqlQuery = "SELECT * FROM UTEPIN WHERE CODUTE = @Codute AND DATE(ULTAGG) = CURRENT_DATE";
                var isPinAlreadyModifiedToday = dataLayer.GetDataTableWithParameters(isPinAlreadyModifiedTodaySqlQuery, coduteParameter).Rows.Count > 0;

                var result = ResetPassword(identifier, encryptedPassword, dataLayer, isPinAlreadyModifiedToday);

                if (result)
                    HttpContext.Current.Items["successMessage"] = "Password modificata correttamente!";

                dataLayer.EndTransaction(result);
            }
            catch
            {
                HttpContext.Current.Items["errorMessage"] = "Operazione non riuscita";
                dataLayer.EndTransaction(false);
            }
        }

        private static bool ResetPassword(string identifier, string encryptedPassword, DataLayer dataLayer, bool hasBeenUpdatedToday)
        {
            var passwordParameter = dataLayer.CreateParameter("@Password", iDB2DbType.iDB2Char, 500, ParameterDirection.Input, encryptedPassword);
            var coduteParameter = dataLayer.CreateParameter("@Codute", iDB2DbType.iDB2Char, 20, ParameterDirection.Input, identifier);

            var updatePrevPwdSqlCmd = $"UPDATE UTEPIN SET DATFIN = '{DateTime.Now.Date.AddDays(-1):yyyy-MM-dd}', STAPIN = 'D' " +
                                      "WHERE CODUTE = @Codute AND DATINI = (SELECT MAX(DATINI) FROM UTEPIN WHERE CODUTE =  @Codute)";
            var insertNewPwdSqlCmd = "INSERT INTO UTEPIN (CODUTE, PIN, DATINI, DATFIN, STAPIN, ULTAGG, UTEAGG) VALUES " +
                                     $"(@Codute , @Password, CURRENT_DATE, '{DateTime.Now.Date.AddDays(90):yyyy-MM-dd}', 'A', CURRENT_TIMESTAMP, @Codute)";
            var updateCurrPwdSqlCmd = "UPDATE UTEPIN SET PIN = @Password WHERE CODUTE = @Codute AND DATINI = CURRENT_DATE";

            if (CheckPasswordValidity(encryptedPassword, coduteParameter))
            {
                HttpContext.Current.Items["errorMessage"] = ": Non è possibile inserire una password uguale alla password precedente";
                return false;
            }

            if (hasBeenUpdatedToday)
            {
                var flag2 = dataLayer.WriteTransactionDataWithParametersAndDontCall(updateCurrPwdSqlCmd, CommandType.Text,
                    passwordParameter, coduteParameter);

                if (flag2)
                    return true;

                HttpContext.Current.Items["errorMessage"] = "Operazione non riuscita";
                return false;
            }

            var flag3 = dataLayer.WriteTransactionDataWithParametersAndDontCall(updatePrevPwdSqlCmd, CommandType.Text,
                coduteParameter, coduteParameter);

            var flag1 = dataLayer.WriteTransactionDataWithParametersAndDontCall(insertNewPwdSqlCmd, CommandType.Text,
                coduteParameter, passwordParameter, coduteParameter);

            if (flag3 && flag1)
                return true;

            HttpContext.Current.Items["errorMessage"] = "Operazione non riuscita";
            return false;

        }

        public static string GetCodiceFiscaleConsulente(string codiceTerritoriale)
        {
            var dataLayer = new DataLayer();
            var codiceTerritorialeParameter = dataLayer.CreateParameter(CodiceTerritoriale, iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input, codiceTerritoriale);
            const string getEmailQuery = $"SELECT CODFIS FROM ASSTER WHERE CODTER = {CodiceTerritoriale}";
            var dataTable1 = dataLayer.GetDataTableWithParameters(getEmailQuery, codiceTerritorialeParameter);
            
            return dataTable1.Rows.Count > 0 ? dataTable1.Rows[0].ElementAt("CODFIS") : default;
        }
    }
}
