// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Utilities.Utile
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using TFI.DAL.ConnectorDB;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TFI.DAL.Utilities
{
    public class Utile
    {
        private DataLayer objDataAccess = new DataLayer();

        public DataTable CREA_DT_AUTORIZZAZIONI_ENPAIA(ref string ErroreMSG)
        {
            DataTable dataTable1 = new DataTable();
            DataTable dataTable2 = new DataTable();
            bool flag1 = false;
            bool flag2 = false;
            dataTable2.Columns.Add(new DataColumn()
            {
                ColumnName = "POSIZIONE"
            });
            dataTable2.Columns.Add(new DataColumn()
            {
                ColumnName = "POSIZIONE_AGF"
            });
            dataTable2.Columns.Add(new DataColumn()
            {
                ColumnName = "ABILITATO"
            });
            dataTable2.Columns.Add(new DataColumn()
            {
                ColumnName = "ABILITATO_AGF"
            });
            DataTable dataTable3 = this.CREA_DT_SOLO_AUTORIZZAZIONI_ENPAIA(ref ErroreMSG);
            for (int index = 0; index <= dataTable3.Rows.Count - 1; ++index)
            {
                flag1 = dataTable3.Rows[index]["ABILITATO"] == (object)"SI";
                flag2 = dataTable3.Rows[index]["ABILITATO_AGF"] == (object)"SI";
            }
            DataTable dataTable4 = this.objDataAccess.GetDataTable("SELECT CODPOS, CODPOS_AGF, CODFUNSIS, CODFUNSIS_AGF FROM PARGENPOS WHERE CURRENT_DATE BETWEEN DATINI AND DATFIN");
            for (int index = 0; index <= dataTable4.Rows.Count - 1; ++index)
            {
                dataTable2.Rows.Add();
                dataTable2.Rows[dataTable2.Rows.Count - 1]["POSIZIONE"] = dataTable4.Rows[index]["CODPOS"];
                dataTable2.Rows[dataTable2.Rows.Count - 1]["POSIZIONE_AGF"] = dataTable4.Rows[index]["CODPOS_AGF"];
                if (this.Module_Autorizzato(Convert.ToInt32(dataTable4.Rows[index]["CODFUNSIS"]), ref ErroreMSG, true))
                    dataTable2.Rows[dataTable2.Rows.Count - 1]["ABILITATO"] = (object)"SI";
                else if (flag1)
                    dataTable2.Rows[dataTable2.Rows.Count - 1]["ABILITATO"] = (object)"SI";
                else
                    dataTable2.Rows[dataTable2.Rows.Count - 1]["ABILITATO"] = (object)"NO";
                if (this.Module_Autorizzato(Convert.ToInt32(dataTable4.Rows[index]["CODFUNSIS_AGF"]), ref ErroreMSG, true))
                    dataTable2.Rows[dataTable2.Rows.Count - 1]["ABILITATO_AGF"] = (object)"SI";
                else if (flag1)
                    dataTable2.Rows[dataTable2.Rows.Count - 1]["ABILITATO_AGF"] = (object)"SI";
                else
                    dataTable2.Rows[dataTable2.Rows.Count - 1]["ABILITATO_AGF"] = (object)"NO";
            }
            return dataTable2;
        }

        public DataTable CREA_DT_SOLO_AUTORIZZAZIONI_ENPAIA(ref string ErroreMSG)
        {
            DataTable dataTable1 = new DataTable();
            DataTable dataTable2 = new DataTable();
            dataTable2.Columns.Add(new DataColumn()
            {
                ColumnName = "POSIZIONE"
            });
            dataTable2.Columns.Add(new DataColumn()
            {
                ColumnName = "POSIZIONE_AGF"
            });
            dataTable2.Columns.Add(new DataColumn()
            {
                ColumnName = "ABILITATO"
            });
            dataTable2.Columns.Add(new DataColumn()
            {
                ColumnName = "ABILITATO_AGF"
            });
            DataTable dataTable3 = this.objDataAccess.GetDataTable("SELECT CODPOS, CODPOS_AGF, CODFUNSIS, CODFUNSIS_AGF FROM PARGENPOS_PERS WHERE CURRENT_DATE BETWEEN DATINI AND DATFIN");
            for (int index = 0; index <= dataTable3.Rows.Count - 1; ++index)
            {
                dataTable2.Rows.Add();
                dataTable2.Rows[dataTable2.Rows.Count - 1]["POSIZIONE"] = dataTable3.Rows[index]["CODPOS"];
                dataTable2.Rows[dataTable2.Rows.Count - 1]["POSIZIONE_AGF"] = dataTable3.Rows[index]["CODPOS_AGF"];
                if (this.Module_Autorizzato(Convert.ToInt32(dataTable3.Rows[index]["CODFUNSIS"]), ref ErroreMSG, true))
                    dataTable2.Rows[dataTable2.Rows.Count - 1]["ABILITATO"] = (object)"SI";
                else
                    dataTable2.Rows[dataTable2.Rows.Count - 1]["ABILITATO"] = (object)"NO";
                if (this.Module_Autorizzato(Convert.ToInt32(dataTable3.Rows[index]["CODFUNSIS_AGF"]), ref ErroreMSG, true))
                    dataTable2.Rows[dataTable2.Rows.Count - 1]["ABILITATO_AGF"] = (object)"SI";
                else
                    dataTable2.Rows[dataTable2.Rows.Count - 1]["ABILITATO_AGF"] = (object)"NO";
            }
            return dataTable2;
        }

        private bool Module_Autorizzato(int intCodFunsis, ref string messaggio, bool SenzaMessaggio = false)
        {
            DataTable dataTable = this.Module_CaricaDataTableFunsisUtente();
            int index = 0;
            while (index <= dataTable.Rows.Count - 1 && Convert.ToInt32(dataTable.Rows[index]["CODFUNSIS"]) != intCodFunsis)
                ++index;
            if (index <= dataTable.Rows.Count - 1)
                return true;
            if (!SenzaMessaggio)
                messaggio = "L'utente non è abilitato a questa funzione";
            return false;
        }

        private DataTable Module_CaricaDataTableFunsisUtente()
        {
            TFI.OCM.Utente.Utente utente = HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente;
            DataTable dataTable1 = this.objDataAccess.GetDataTable("SELECT DISTINCT CODFUNSIS FROM (" + " SELECT CODFUNSIS FROM PROUTE " + " WHERE PROUTE.CODUTE = " + DBMethods.DoublePeakForSql(utente.Username) + " UNION " + " SELECT CODFUNSIS FROM FUNGRU, GRUSIS, GRUUTE " + " WHERE FUNGRU.CODGRU = GRUSIS.CODGRU " + " AND GRUSIS.CODGRU = GRUUTE.CODGRU " + " And GRUUTE.CODUTE = " + DBMethods.DoublePeakForSql(utente.Username) + " UNION " + " SELECT CODFUNSIS FROM PROENPF, ENPUTE " + " WHERE PROENPF.CODFUNENP = ENPUTE.CODFUNENP " + " And ENPUTE.CODUTE = " + DBMethods.DoublePeakForSql(utente.Username) + " UNION " + " SELECT CODFUNSIS FROM FUNGRU, ENPGRU, GRUUTE " + " WHERE FUNGRU.CODGRU = ENPGRU.CODGRU " + " AND ENPGRU.CODGRU = GRUUTE.CODGRU " + " And GRUUTE.CODUTE = " + DBMethods.DoublePeakForSql(utente.Username) + ") AS TABELLA ORDER BY CODFUNSIS");
            dataTable1.Columns.Add(new DataColumn()
            {
                ColumnName = "MENU"
            });
            DataTable dataTable2 = new DataTable();
            DataTable dataTable3 = this.objDataAccess.GetDataTable("SELECT DISTINCT FUNSIS FROM (" + "(SELECT DISTINCT FUNSIS FROM MENU WHERE FUNSIS IS NOT NULL AND FUNSIS<>0)" + " UNION " + "(SELECT DISTINCT FUNSIS FROM SUBMENU WHERE FUNSIS IS NOT NULL AND FUNSIS<>0))" + " AS TABELLA");
            for (int index1 = 0; index1 <= dataTable3.Rows.Count - 1; ++index1)
            {
                for (int index2 = 0; index2 <= dataTable1.Rows.Count - 1; ++index2)
                {
                    if (dataTable3.Rows[index1]["FUNSIS"] == dataTable1.Rows[index2]["CODFUNSIS"])
                    {
                        dataTable1.Rows[index2]["MENU"] = (object)"S";
                        break;
                    }
                }
            }
            if (utente.Username != "LMRDRN77R49H501V")
            {
                DataRow row = dataTable1.NewRow();
                row[0] = (object)2997;
                dataTable1.Rows.Add(row);
            }
            for (int index = 0; index <= dataTable1.Rows.Count - 1; ++index)
            {
                if (2997 == Convert.ToInt32(dataTable1.Rows[index]["CODFUNSIS"]))
                {
                    dataTable1.Rows[index]["MENU"] = (object)"S";
                    break;
                }
            }
            return dataTable1;
        }

        public DateTime Module_GetDataSistema() => Convert.ToDateTime(this.objDataAccess.Get1ValueFromSQL("SELECT CURRENT_DATE AS DATASISTEMA FROM DBUNICONET.TIPIND", CommandType.Text));

        public string Module_CheckData_9999_12_31(string strData) => strData == "9999-12-31" || strData == "2100-12-31" ? (strData = "") : strData;

        public bool Module_Check_65Anni(DateTime datDataConfronto, string strDataNascita) => !(strDataNascita.Trim() == "") && DateTime.Compare(DateTime.Parse(strDataNascita).AddYears(65), datDataConfronto) <= 0;

        public bool Invia_Email(string ALLEGATO, string MAIL_TESTO, string Oggetto, string Email)
        {
            try
            {
                MailMessage message = new MailMessage();
                message.Headers.Add("Disposition-notification-to", "contributi@enpaia.it");
                message.From = new MailAddress("comunicazioni_istituzionali@enpaia.it", "Fondazione ENPAIA");
                message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
                message.Priority = MailPriority.High;
                message.To.Add(Email);
                message.Attachments.Add(new Attachment(ALLEGATO));
                message.Subject = Oggetto;
                message.Body = MAIL_TESTO;
                message.IsBodyHtml = true;
                new SmtpClient(ConfigurationManager.AppSettings.Get("UserMail").ToString())
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = false,
                    Credentials = ((ICredentialsByHost)new NetworkCredential(ConfigurationManager.AppSettings.Get("UserMail").ToString(), ConfigurationManager.AppSettings.Get("PwdMail").ToString()).GetCredential(ConfigurationManager.AppSettings.Get("IndirizzoMail").ToString(), Convert.ToInt32(ConfigurationManager.AppSettings.Get("PortaMail").ToString()), ConfigurationManager.AppSettings.Get("TipoAutenticazione").ToString()))
                }.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Module_Autorizzazioni_Speciali_Solo_Enpaia(ref int codPos, string sistema)
        {
            string empty = string.Empty;
            DataTable dataTable = this.CREA_DT_SOLO_AUTORIZZAZIONI_ENPAIA(ref empty);
            if (!(sistema == "CONTRIBUTI"))
            {
                if (sistema == "AGRIFONDO")
                {
                    IEnumerator enumerator = dataTable.Rows.GetEnumerator();
                    try
                    {
                        if (enumerator.MoveNext())
                        {
                            DataRow current = (DataRow)enumerator.Current;
                            if ((!DBNull.Value.Equals(current["ABILITATO_AGF"]) ? current["ABILITATO_AGF"].ToString() : string.Empty) == "SI")
                            {
                                codPos = !DBNull.Value.Equals(current["POSIZIONE_AGF"]) ? Convert.ToInt32(current["POSIZIONE_AGF"]) : 0;
                                return true;
                            }
                            codPos = 0;
                            return false;
                        }
                    }
                    finally
                    {
                        if (enumerator is IDisposable disposable)
                            disposable.Dispose();
                    }
                }
            }
            else
            {
                IEnumerator enumerator = dataTable.Rows.GetEnumerator();
                try
                {
                    if (enumerator.MoveNext())
                    {
                        DataRow current = (DataRow)enumerator.Current;
                        if ((!DBNull.Value.Equals(current["ABILITATO"]) ? current["ABILITATO"].ToString() : string.Empty) == "SI")
                        {
                            codPos = !DBNull.Value.Equals(current["POSIZIONE"]) ? Convert.ToInt32(current["POSIZIONE"]) : 0;
                            return true;
                        }
                        codPos = 0;
                        return false;
                    }
                }
                finally
                {
                    if (enumerator is IDisposable disposable)
                        disposable.Dispose();
                }
            }
            return false;
        }

        public bool Module_Autorizzazioni_Speciali(string codPos, string sistema)
        {
            string empty = string.Empty;
            DataTable dataTable = this.CREA_DT_AUTORIZZAZIONI_ENPAIA(ref empty);
            if (!(sistema == "CONTRIBUTI"))
            {
                if (sistema == "AGRIFONDO")
                {
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
                    {
                        if ((!DBNull.Value.Equals(row["POSIZIONE_AGF"]) ? row["POSIZIONE_AGF"].ToString() : string.Empty) == codPos)
                            return (!DBNull.Value.Equals(row["ABILITATO_AGF"]) ? row["ABILITATO_AGF"].ToString() : string.Empty) == "SI";
                    }
                }
            }
            else
            {
                foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
                {
                    if ((!DBNull.Value.Equals(row["POSIZIONE"]) ? row["POSIZIONE"].ToString() : string.Empty) == codPos)
                        return (!DBNull.Value.Equals(row["ABILITATO"]) ? row["ABILITATO"].ToString() : string.Empty) == "SI";
                }
            }
            return true;
        }

        public static Dictionary<int, string> GetMesi() => new Dictionary<int, string>()
    {
      {
        0,
        "Seleziona"
      },
      {
        1,
        "Gennaio"
      },
      {
        2,
        "Febbraio"
      },
      {
        3,
        "Marzo"
      },
      {
        4,
        "Aprile"
      },
      {
        5,
        "Maggio"
      },
      {
        6,
        "Giugno"
      },
      {
        7,
        "Luglio"
      },
      {
        8,
        "Agosto"
      },
      {
        9,
        "Settembre"
      },
      {
        10,
        "Ottobre"
      },
      {
        11,
        "Novembre"
      },
      {
        12,
        "Dicembre"
      }
    };

        public static string GetDataSistema()
        {
            DataLayer dataLayer = new DataLayer();
            try
            {
                return dataLayer.Get1ValueFromSQL("SELECT CURRENT_DATE AS DATASISTEMA FROM DBUNICONET.TIPIND", CommandType.Text);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static string GetRagioneSociale(string codPos)
        {
            try
            {
                DataLayer dataLayer = new DataLayer();
                iDB2Parameter parameter = dataLayer.CreateParameter("codPos_param", iDB2DbType.iDB2VarChar, 50, ParameterDirection.Input, codPos);
                string strSQL = "SELECT RAGSOC FROM azi WHERE CODPOS = @codPos_param";
                DataTable tableWithParameters = dataLayer.GetDataTableWithParameters(strSQL, parameter);
                return tableWithParameters.Rows.Count > 0 ? (!DBNull.Value.Equals(tableWithParameters.Rows[0]["RAGSOC"]) ? tableWithParameters.Rows[0]["RAGSOC"].ToString() : string.Empty) : string.Empty;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static bool IsCabKnown(string cab)
        {
            DataLayer dataLayer = new DataLayer();
            iDB2Parameter parameter = dataLayer.CreateParameter("@cab_param", iDB2DbType.iDB2Char, 20, ParameterDirection.Input, cab);
            string query = "SELECT * FROM TABCAB WHERE CODCAB = @cab_param";
            DataTable table = dataLayer.GetDataTableWithParameters(query, parameter);
            return table.Rows.Count > 0;
        }

        public static bool IsAbiKnown(string abi)
        {
            DataLayer dataLayer = new DataLayer();
            iDB2Parameter parameter = dataLayer.CreateParameter("@abi_param", iDB2DbType.iDB2Char, 20, ParameterDirection.Input, abi);
            string query = "SELECT * FROM TABABI WHERE CODABI = @abi_param";
            DataTable table = dataLayer.GetDataTableWithParameters(query, parameter);
            return table.Rows.Count > 0;
        }

        public static bool CheckIban(string Iban)
        {
            if (string.IsNullOrWhiteSpace(Iban))
                return false;
            Iban = Iban.Replace(" ", string.Empty).ToUpper();
            if (!Regex.IsMatch(Iban, "^(AL|al)[0-9]{2}[0-9]{8}[a-zA-Z0-9]{16}$|^(AD|ad)[0-9]{2}[0-9]{8}[a-zA-Z0-9]{12}$|^(AT|at)[0-9]{2}[0-9]{16}$|^(AZ|az)[0-9]{2}[a-zA-Z]{4}[a-zA-Z0-9]{20}$|^(BH|bh)[0-9]{2}[a-zA-Z]{4}[a-zA-Z0-9]{14}$|^(BY|by)[0-9]{2}[a-zA-Z0-9]{4}[0-9]{4}[a-zA-Z0-9]{16}$|^(BE|be)[0-9]{2}[0-9]{12}$|^(BA|ba)[0-9]{2}[0-9]{16}$|^(BR|br)[0-9]{2}[0-9]{23}[a-zA-Z][a-zA-Z0-9]$|^(BG|bg)[0-9]{2}[a-zA-Z]{4}[0-9]{6}[a-zA-Z0-9]{8}$|^(CR|cr)[0-9]{2}[0-9]{18}$|^(HR|hr)[0-9]{2}[0-9]{17}$|^(CY|cy)[0-9]{2}[0-9]{8}[a-zA-Z0-9]{16}$|^(CZ|cz)[0-9]{2}[0-9]{20}$|^(DK|dk)[0-9]{2}[0-9]{14}$|^(DO|do)[0-9]{2}[a-zA-Z]{4}[0-9]{20}$|^(TL|tl)[0-9]{2}[0-9]{19}$|^(EG|eg)[0-9]{2}[0-9]{25}$|^(SV|sv)[0-9]{2}[a-zA-Z]{4}[0-9]{20}$|^(EE|ee)[0-9]{2}[0-9]{16}$|^(FO|fo)[0-9]{2}[0-9]{14}$|^(FI|fi)[0-9]{2}[0-9]{14}$|^(FR|fr)[0-9]{2}[0-9]{10}[a-zA-Z0-9]{11}[0-9]{2}$|^(GE|ge)[0-9]{2}[a-zA-Z0-9]{2}[0-9]{16}$|^(DE|de)[0-9]{2}[0-9]{18}$|^(GI|gi)[0-9]{2}[a-zA-Z]{4}[a-zA-Z0-9]{15}$|^(GR|gr)[0-9]{2}[0-9]{7}[a-zA-Z0-9]{16}$|^(GL|gl)[0-9]{2}[0-9]{14}$|^(GT|gt)[0-9]{2}[a-zA-Z0-9]{24}$|^(HU|hu)[0-9]{2}[0-9]{24}$|^(IS|is)[0-9]{2}[0-9]{22}$|^(IQ|iq)[0-9]{2}[a-zA-Z]{4}[0-9]{15}$|^(IE|ie)[0-9]{2}[a-zA-Z0-9]{4}[0-9]{14}$|^(IL|il)[0-9]{2}[0-9]{19}$|^(IT|it)[0-9]{2}[a-zA-Z][0-9]{10}[a-zA-Z0-9]{12}$|^(JO|jo)[0-9]{2}[a-zA-Z]{4}[0-9]{22}$|^(KZ|kz)[0-9]{2}[0-9]{3}[a-zA-Z0-9]{13}$|^(XK|xk)[0-9]{2}[0-9]{16}$|^(KW|kw)[0-9]{2}[a-zA-Z]{4}[a-zA-Z0-9]{22}$|^(LV|lv)[0-9]{2}[a-zA-Z]{4}[a-zA-Z0-9]{13}$|^(LB|lb)[0-9]{2}[0-9]{4}[a-zA-Z0-9]{20}$|^(LY|ly)[0-9]{2}[0-9]{21}$|^(LI|li)[0-9]{2}[0-9]{5}[a-zA-Z0-9]{12}$|^(LT|lt)[0-9]{2}[0-9]{16}$|^(LU|lu)[0-9]{2}[0-9]{3}[a-zA-Z0-9]{13}$|^(MK|mk)[0-9]{2}[0-9]{3}[a-zA-Z0-9]{10}[0-9]{2}$|^(MT|mt)[0-9]{2}[a-zA-Z]{4}[0-9]{5}[a-zA-Z0-9]{18}$|^(MR|mr)[0-9]{2}[0-9]{23}$|^(MU|mu)[0-9]{2}[a-zA-Z]{4}[0-9]{19}[a-zA-Z]{3}$|^(MC|mc)[0-9]{2}[0-9]{10}[a-zA-Z0-9]{11}[0-9]{2}$|^(MD|md)[0-9]{2}[a-zA-Z0-9]{20}$|^(ME|me)[0-9]{2}[0-9]{18}$|^(NL|nl)[0-9]{2}[a-zA-Z]{4}[0-9]{10}$|^(NO|no)[0-9]{2}[0-9]{11}$|^(PK|pk)[0-9]{2}[a-zA-Z0-9]{4}[0-9]{16}$|^(PS|ps)[0-9]{2}[a-zA-Z0-9]{4}[0-9]{21}$|^(PL|pl)[0-9]{2}[0-9]{24}$|^(PT|pt)[0-9]{2}[0-9]{21}$|^(QA|qa)[0-9]{2}[a-zA-Z]{4}[a-zA-Z0-9]{21}$|^(RO|ro)[0-9]{2}[a-zA-Z]{4}[a-zA-Z0-9]{16}$|^(LC|lc)[0-9]{2}[a-zA-Z]{4}[a-zA-Z0-9]{24}$|^(SM|sm)[0-9]{2}[a-zA-Z][0-9]{10}[a-zA-Z0-9]{12}$|^(ST|st)[0-9]{2}[0-9]{21}$|^(SA|sa)[0-9]{2}[0-9]{2}[a-zA-Z0-9]{18}$|^(RS|rs)[0-9]{2}[0-9]{18}$|^(SC|sc)[0-9]{2}[a-zA-Z]{4}[0-9]{20}[a-zA-Z]{3}$|^(SK|sk)[0-9]{2}[0-9]{20}$|^(SI|si)[0-9]{2}[0-9]{15}$|^(ES|es)[0-9]{2}[0-9]{20}$|^(SD|sd)[0-9]{2}[0-9]{14}$|^(SE|se)[0-9]{2}[0-9]{20}$|^(CH|ch)[0-9]{2}[0-9]{5}[a-zA-Z0-9]{12}$|^(TN|tn)[0-9]{2}[0-9]{20}$|^(TR|tr)[0-9]{2}[0-9]{5}[a-zA-Z0-9]{17}$|^(UA|ua)[0-9]{2}[0-9]{6}[a-zA-Z0-9]{19}$|^(AE|ae)[0-9]{2}[0-9]{19}$|^(GB|gb)[0-9]{2}[a-zA-Z]{4}[0-9]{14}$|^(VA|va)[0-9]{2}[0-9]{18}$|^(VG|vg)[0-9]{2}[a-zA-Z0-9]{4}[0-9]{16}$"))
                return false;
            Iban = Iban.Substring(4) + Iban.Substring(0, 4);
            int num = 0;
            foreach (char c in Iban)
                num = !char.IsLetter(c) ? (10 * num + ((int)c - 48)) % 97 : (100 * num + ((int)c - 55)) % 97;
            return num == 1;
        }

        public static bool CheckAbiAndCab(string iban)
        {
            iban = iban.Replace(" ", string.Empty).ToUpper();

            if (!iban.StartsWith("IT"))
                return true;

            string abi = iban.Substring(5, 5);
            string cab = iban.Substring(10, 5);

            return Utile.IsAbiKnown(abi) && Utile.IsCabKnown(cab);
        }

        public static string GetMatricolaIscritto(string username) => new DataLayer().Get1ValueFromSQL("SELECT MAT FROM ISCT WHERE CODFIS = '" + username + "'", CommandType.Text);

        public static DateTime GetDataDiNascita(string username) => DateTime.Parse(new DataLayer().Get1ValueFromSQL("SELECT DATNAS FROM ISCT WHERE CODFIS = '" + username + "'", CommandType.Text).ToString());
    
        public static string FromStringToDateTimeToFormatString(string date, string format = "yyyy-MM-dd")
        {
            if (DateTime.TryParse(date, out DateTime datetime))
            {
                return datetime.ToString(format);
            }
            return null;
        }
    }
}
