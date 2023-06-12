// Decompiled with JetBrains decompiler
// Type: TFI.DAL.AziendaConsulente.DelegheDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using DocumentFormat.OpenXml.Wordprocessing;
using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Web;
using OCM.TFI.OCM.Utilities;
using TFI.CRYPTO.Crypto;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.AziendaConsulente;
using TFI.OCM.Utente;
using Utilities;

namespace TFI.DAL.AziendaConsulente
{
    public class DelegheDAL
    {
        private DelegheOCM Deleghe = new DelegheOCM();
        private DataLayer objDataAccess = new DataLayer();

        public void Liste()
        {
        }

        public DelegheOCM SearcheDeleghe(
          string PosAz,
          ref string ErroreMSG,
          ref string SuccessMSG)
        {
            DelegheOCM delegheOcm = new DelegheOCM();
            try
            {
                var codposParam = objDataAccess.CreateParameter("@codpos", iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, PosAz);
                var getDelegheSqlQuery = "SELECT D.DATINI, D.DATFIN, D.FLGATT, D.CODPOS, AZ.RAGSOC, AST.RAGSOC AS ASSTER, AN.RAGSOC AS ASSNAZ, D.CODTER, AST.CODNAZ " +
                                         "FROM DELEGHE D " +
                                         "INNER JOIN AZI AZ ON D.CODPOS = AZ.CODPOS " +
                                         "LEFT JOIN ASSTER AST ON D.CODTER = AST.CODTER " +
                                         "LEFT JOIN ASSNAZ AN ON AST.CODNAZ = AN.CODNAZ " +
                                         "WHERE D.CODPOS = @codpos";
                var deleghe = objDataAccess.GetDataTableWithParameters(getDelegheSqlQuery, codposParam);

                if (deleghe.Rows.Count > 0)
                {
                    foreach (DataRow row in (InternalDataCollectionBase)deleghe.Rows)
                    {
                        switch (row.ElementAt("FLGATT"))
                        {
                            case "0":
                                FillDelegheCancellate(row, delegheOcm);
                                break;
                            case "1":
                                FillDelegheAttive(row, delegheOcm);
                                break;
                            default:
                                FillDelegheNonAttive(row, delegheOcm);
                                break;
                        }

                    }
                }

                return delegheOcm;
            }
            catch
            {
                ErroreMSG = "Nessun risultato trovato";
                return null;
            }

            void FillDelegheAttive(DataRow row, DelegheOCM delegheOcm1)
            {
                DelegheOCM.DelegheAttive delegheAttive = new DelegheOCM.DelegheAttive()
                {
                    datini = row.DateElementAt("DATINI", StandardUse.Readable),
                    datfin = row.DateElementAt("DATFIN", StandardUse.Readable),
                    codter = row.ElementAt("CODTER"),
                    asster = row.ElementAt("ASSTER"),
                    assnaz = row.ElementAt("ASSNAZ"),
                    codpos = row.ElementAt("CODPOS"),
                    ragsoc = row.ElementAt("RAGSOC"),
                    stato = row.ElementAt("FLGATT"),
                    attivo = "Attiva"
                };
                delegheOcm1.delegheatt.Add(delegheAttive);
            }

            void FillDelegheCancellate(DataRow row, DelegheOCM delegheOcm2)
            {
                DelegheOCM.DelegheCancellate delegheCancellate = new DelegheOCM.DelegheCancellate()
                {
                    datini = row.DateElementAt("DATINI", StandardUse.Readable),
                    datfin = row.DateElementAt("DATFIN", StandardUse.Readable),
                    codter = row.ElementAt("CODTER"),
                    asster = row.ElementAt("ASSTER"),
                    assnaz = row.ElementAt("ASSNAZ"),
                    codpos = row.ElementAt("CODPOS"),
                    ragsoc = row.ElementAt("RAGSOC"),
                    stato = row.ElementAt("FLGATT"),
                    cancellato = "Cancellata"
                };
                delegheOcm2.deleghecanc.Add(delegheCancellate);
            }

            void FillDelegheNonAttive(DataRow row, DelegheOCM delegheOcm3)
            {
                DelegheOCM.DelegheNonAttive delegheNonAttive = new DelegheOCM.DelegheNonAttive()
                {
                    datini = row.DateElementAt("DATINI", StandardUse.Readable),
                    datfin = row.DateElementAt("DATFIN", StandardUse.Readable),
                    codter = row.ElementAt("CODTER"),
                    asster = row.ElementAt("ASSTER"),
                    assnaz = row.ElementAt("ASSNAZ"),
                    codpos = row.ElementAt("CODPOS"),
                    ragsoc = row.ElementAt("RAGSOC"),
                    stato = row.ElementAt("FLGATT"),
                    daAttivare = "Non Confermate"
                };
                delegheOcm3.deleghenonatt.Add(delegheNonAttive);
            }
        }

        public bool EliminaDeleghe(
          string codter,
          string codpos,
          string u)
        {
            var updateDelegheSqlCmd = $"UPDATE DELEGHE SET DATFIN ='{DateTime.Now.AddDays(-1):yyyy-MM-dd}', FLGATT = 0, UTEAGG = @username, " +
                                      $"ULTAGG = CURRENT TIMESTAMP WHERE CODPOS = @codpos AND CODTER = @codter AND DATINI = " +
                                      "(SELECT DATINI FROM DELEGHE WHERE CODPOS = @codpos AND CODTER = @codter ORDER BY DATINI DESC LIMIT 1)";
            return UpdateDeleghe(codter, codpos, u, updateDelegheSqlCmd);
        }

        public bool AttivaDeleghe(
          string codter,
          string codpos,
          string u)
        {
            var updateDelegheSqlCmd = "UPDATE DELEGHE SET FLGATT = 1, UTEAGG = @username, ULTAGG = " +
                                      "CURRENT TIMESTAMP WHERE CODPOS = @codpos AND CODTER = @codter AND DATINI = (SELECT DATINI FROM DELEGHE WHERE " +
                                      "CODPOS = @codpos AND CODTER = @codter ORDER BY DATINI DESC LIMIT 1) AND FLGATT IS NULL";
            return UpdateDeleghe(codter, codpos, u, updateDelegheSqlCmd);
        }

        public string GetDatiniDelega(
            string codter,
            string codpos)
        {
            var codposParam = objDataAccess.CreateParameter("@codpos", iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codpos);
            var codterParam = objDataAccess.CreateParameter("@codter", iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input, codter);

            var getDelegaSqlCmd = "SELECT * FROM DELEGHE WHERE CODPOS = @codpos AND CODTER = @codter ORDER BY DATINI DESC LIMIT 1";

            var delega = objDataAccess.GetDataTableWithParameters(getDelegaSqlCmd, codposParam, codterParam).Rows[0];
            return delega.DateElementAt("DATINI");
        }

        public DelegheOCM caricaDenominazione(string codNaz, ref string MSGErrore)
        {
            string strSQL = "SELECT CODTER, RAGSOC FROM ASSTER WHERE CODNAZ = " + codNaz + " ORDER BY RAGSOC";
            DataTable dataTable1 = new DataTable();
            DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL);
            DelegheOCM delegheOcm = new DelegheOCM();
            List<DelegheOCM.ListDen> listDenList = new List<DelegheOCM.ListDen>();
            foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
            {
                DelegheOCM.ListDen listDen = new DelegheOCM.ListDen()
                {
                    RAGSOCASS = row["RAGSOC"].ToString(),
                    CODTER = row["CODTER"].ToString()
                };
                listDenList.Add(listDen);
            }
            delegheOcm.ListDenominazione = listDenList;
            return delegheOcm;
        }

        public DelegheOCM AssNaz()
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
            string strSQL1 = "SELECT * FROM ASSNAZ";
            DataTable dataTable1 = new DataTable();
            DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL1);
            DelegheOCM delegheOcm = new DelegheOCM();
            List<DelegheOCM.AssNaz> assNazList = new List<DelegheOCM.AssNaz>();
            foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
            {
                DelegheOCM.AssNaz assNaz = new DelegheOCM.AssNaz()
                {
                    CODNAZ = row["CODNAZ"].ToString(),
                    RAGSOCASS = row["RAGSOC"].ToString(),
                    RAGSOCBRASS = row["RAGSOCBRE"].ToString()
                };
                assNazList.Add(assNaz);
            }
            delegheOcm.AssociazioneNaz = assNazList;
            string strSQL2 = "SELECT CODDUG, DENDUG FROM DUG ORDER BY DENDUG";
            DataTable dataTable3 = new DataTable();
            DataTable dataTable4 = this.objDataAccess.GetDataTable(strSQL2);
            List<DelegheOCM.ListInd> listIndList = new List<DelegheOCM.ListInd>();
            foreach (DataRow row in (InternalDataCollectionBase)dataTable4.Rows)
            {
                DelegheOCM.ListInd listInd = new DelegheOCM.ListInd()
                {
                    DENDUG = row["DENDUG"].ToString(),
                    CODDUG = row["CODDUG"].ToString()
                };
                listIndList.Add(listInd);
            }
            delegheOcm.ListaIndirizzi = listIndList;
            return delegheOcm;
        }

        public DelegheOCM carica_Dati_Azienda(
          ref string MSGErrore,
          string posizione,
          string strModo)
        {
            DelegheOCM delegheOcm = new DelegheOCM();
            DataTable dataTable1 = new DataTable();
            try
            {
                if ((strModo ?? "") == "NUOVO" && Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL("SELECT COUNT(*) FROM AZI WHERE CODPOS =" + posizione, CommandType.Text)) == 0)
                {
                    MSGErrore = "Posizione aziendale inesistente";
                    return (DelegheOCM)null;
                }
                string str1 = "SELECT AZI.RAGSOC, AZI.NATGIU, NG.DENNATGIU, AZI.PARIVA, AZI.CODFIS, DUG.DENDUG, INDSED.IND, INDSED.NUMCIV, " +
                            " INDSED.CAP, INDSED.DENLOC, INDSED.SIGPRO, INDSED.DENSTAEST,  INDSED.TEL1,  INDSED.TEL2,  " + " INDSED.FAX,  AZM.EMAIL " +
                            " FROM INDSED INNER JOIN AZI ON" + " INDSED.CODPOS =  AZI.CODPOS LEFT JOIN  DUG ON  INDSED.CODDUG =  DUG.CODDUG JOIN AZEMAIL AS AZM " +
                            "ON AZM.CODPOS = AZI.CODPOS JOIN NATGIU AS NG ON NG.NATGIU = AZI.NATGIU";
                string str2 = strModo ?? "";
                if (!(str2 == "VISUALIZZA") && !(str2 == "MODIFICA"))
                {
                    if (str2 == "NUOVO")
                        str1 = str1 + " WHERE INDSED.CODPOS = " + posizione;
                }
                else
                    str1 = str1 + " WHERE INDSED.CODPOS = " + posizione;
                string strSQL1 = str1 + " AND INDSED.TIPIND=1" + " AND INDSED.DATINI <= CURRENT_DATE" + " ORDER BY INDSED.DATCOM, AZM.DATFIN DESC FETCH FIRST 1 ROWS ONLY";
                dataTable1.Clear();
                DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL1);
                if (dataTable2 != null && dataTable2.Rows.Count > 0)
                {
                    this.Deleghe.datidelega.RagSocAz = dataTable2.Rows[0]["RAGSOC"].ToString().Trim();
                    this.Deleghe.datidelega.ParIvaAz = dataTable2.Rows[0]["PARIVA"].ToString().Trim();
                    this.Deleghe.datidelega.CodFisAz = dataTable2.Rows[0]["CODFIS"].ToString().Trim();
                    if (!string.IsNullOrEmpty(dataTable2.Rows[0]["NUMCIV"].ToString()))
                        this.Deleghe.datidelega.IndAz = dataTable2.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable2.Rows[0]["IND"].ToString().Trim() + ", " + dataTable2.Rows[0]["NUMCIV"].ToString().Trim();
                    else
                        this.Deleghe.datidelega.IndAz = dataTable2.Rows[0]["DENDUG"].ToString().Trim() + " " + dataTable2.Rows[0]["IND"].ToString().Trim();
                    this.Deleghe.datidelega.CapAz = dataTable2.Rows[0]["CAP"].ToString().Trim();
                    this.Deleghe.datidelega.EmailAz = dataTable2.Rows[0]["EMAIL"].ToString().ToLower().Trim();
                    this.Deleghe.datidelega.FaxAz = dataTable2.Rows[0]["FAX"].ToString().Trim();
                    this.Deleghe.datidelega.StatoEstAz = dataTable2.Rows[0]["DENSTAEST"].ToString().Trim();
                    this.Deleghe.datidelega.CittaAz = dataTable2.Rows[0]["DENLOC"].ToString().Trim();
                    this.Deleghe.datidelega.PrAz = dataTable2.Rows[0]["SIGPRO"].ToString().Trim();
                    this.Deleghe.datidelega.TelAz = dataTable2.Rows[0]["TEL1"].ToString().Trim();
                    this.Deleghe.datidelega.NATGIU = dataTable2.Rows[0]["DENNATGIU"].ToString().Trim();
                }
                string str3 = "SELECT COG, NOM, CODFIS FROM AZIRAP";
                string str4 = strModo ?? "";
                if (!(str4 == "VISUALIZZA") && !(str4 == "MODIFICA"))
                {
                    if (str4 == "NUOVO")
                        str3 = str3 + " WHERE CODPOS = " + posizione;
                }
                else
                    str3 = str3 + " WHERE CODPOS = " + posizione;
                string strSQL2 = str3 + " AND DATINI <= CURRENT_DATE" + " ORDER BY DATCOM DESC FETCH FIRST 1 ROWS ONLY";
                dataTable2.Clear();
                DataTable dataTable3 = this.objDataAccess.GetDataTable(strSQL2);
                if (dataTable3 != null && dataTable3.Rows.Count > 0)
                {
                    this.Deleghe.datidelega.Cognome = dataTable3.Rows[0]["COG"].ToString().Trim();
                    this.Deleghe.datidelega.Nome = dataTable3.Rows[0]["NOM"].ToString().Trim();
                    this.Deleghe.datidelega.COdFIs = dataTable3.Rows[0]["CODFIS"].ToString().Trim();
                }
                delegheOcm.datidelega = this.Deleghe.datidelega;
            }
            catch (Exception ex)
            {
                return (DelegheOCM)null;
            }
            return delegheOcm;
        }

        public DelegheOCM carica_Dati_Associazione(
          DelegheOCM docm,
          string codTer,
          string codNaz,
          string codpos,
          string assnaz,
          string asster)
        {
            DataTable dataTable1 = new DataTable();
            try
            {
                string strSQL1 = " SELECT VALUE(CHAR(ASSTER.CODUTE),'') AS CODUTE, UTEPIN.PIN FROM ASSTER INNER JOIN UTEPIN ON" +
                            " ASSTER.CODUTE = UTEPIN.CODUTE INNER JOIN DELEGHE ON" + " DELEGHE.CODTER = ASSTER.CODTER" + " WHERE ASSTER.CODTER = " + codTer +
                            " AND DELEGHE.CODPOS = " + codpos + " AND UTEPIN.STAPIN = 'A'" +
                            " AND UTEPIN.DATINI = (SELECT MAX(DATINI) FROM UTEPIN WHERE CODUTE = ASSTER.CODUTE)" + " AND CURRENT_DATE BETWEEN DELEGHE.DATINI AND DELEGHE.DATFIN";
                dataTable1.Clear();
                DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL1);
                if (dataTable2.Rows.Count > 0)
                {
                    this.Deleghe.datidelega.CodUteDe = dataTable2.Rows[0]["CODUTE"].ToString().Trim();
                    this.Deleghe.datidelega.Pin = Cypher.DeCryptPassword(dataTable2.Rows[0]["PIN"].ToString().Trim());
                }
                else
                {
                    this.Deleghe.datidelega.CodUteDe = "";
                    this.Deleghe.datidelega.Pin = "";
                }
                string strSQL2 = "SELECT ASSTER.RAGSOC, ASSTER.CODCOM, ASSTER.RAGSOCBRE, ASSTER.EMAILCERT, ASSTER.CELL,  ASSTER.PARIVA, DUG.DENDUG, DUG.CODDUG, ASSTER.IND, ASSTER.NUMCIV, " +
                            " ASSTER.CAP, ASSTER.DENLOC, ASSTER.SIGPRO,  ASSTER.TEL1,  ASSTER.TEL2, ASSTER.CODFIS, " +
                            " ASSTER.FAX,  ASSTER.EMAIL " + " FROM  ASSTER LEFT JOIN DUG ON  ASSTER.CODDUG =  DUG.CODDUG " + " WHERE ASSTER.CODTER = " + codTer + " AND ASSTER.CODNAZ = " + codNaz;
                dataTable2.Clear();
                DataTable dataTable3 = this.objDataAccess.GetDataTable(strSQL2);
                if (dataTable3 != null && dataTable3.Rows.Count > 0)
                {
                    this.Deleghe.datidelega.AssStudio = assnaz;
                    this.Deleghe.datidelega.Denominazione = asster;
                    this.Deleghe.datidelega.ParIvaDe = dataTable3.Rows[0]["PARIVA"].ToString();
                    this.Deleghe.datidelega.RagSocDe = dataTable3.Rows[0]["RAGSOC"].ToString().Trim();
                    this.Deleghe.datidelega.CodFisDe = dataTable3.Rows[0]["CODFIS"].ToString().Trim();
                    this.Deleghe.datidelega.ViaDe = dataTable3.Rows[0]["DENDUG"].ToString().Trim();
                    this.Deleghe.datidelega.IndDe = dataTable3.Rows[0]["IND"].ToString().Trim();
                    this.Deleghe.datidelega.CapDe = dataTable3.Rows[0]["CAP"].ToString().Trim();
                    this.Deleghe.datidelega.NumCivDe = dataTable3.Rows[0]["NUMCIV"].ToString().Trim();
                    this.Deleghe.datidelega.CODDUG = dataTable3.Rows[0]["CODDUG"].ToString().Trim();
                    this.Deleghe.datidelega.EmailDe = dataTable3.Rows[0]["EMAIL"].ToString().Trim();
                    this.Deleghe.datidelega.LocalitaDe = dataTable3.Rows[0]["DENLOC"].ToString().Trim();
                    this.Deleghe.datidelega.PrDe = dataTable3.Rows[0]["SIGPRO"].ToString().Trim();
                    this.Deleghe.datidelega.TelDe = dataTable3.Rows[0]["TEL1"].ToString();
                    this.Deleghe.datidelega.EmailCertDe = dataTable3.Rows[0]["EMAILCERT"].ToString().Trim();
                    this.Deleghe.datidelega.CelDe = dataTable3.Rows[0]["CELL"].ToString().Trim();
                    this.Deleghe.datidelega.ComuneDe = string.IsNullOrEmpty(dataTable3.Rows[0]["CODCOM"].ToString().Trim()) ? "" : this.objDataAccess.Get1ValueFromSQL("SELECT VALUE(DENCOM,'') AS DENCOM FROM CODCOM WHERE CODCOM = " + DBMethods.DoublePeakForSql(dataTable3.Rows[0]["CODCOM"].ToString().Trim()), CommandType.Text).ToString().Trim();
                }
                docm.datidelega = this.Deleghe.datidelega;
            }
            catch (Exception ex)
            {
                return (DelegheOCM)null;
            }
            return docm;
        }

        public bool CheckInput(
          ref string MSGErrore,
          string strModo,
          string codTer,
          string codpos,
          DelegheOCM.DatiDelega Deleghe)
        {
            string str1 = "CURRENT_TIMESTAMP";
            if (strModo == "MODIFICA")
            {
                if (Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL("SELECT COUNT(*) FROM DELEGHE " + " WHERE CODPOS = " + codpos + " AND CURRENT_DATE BETWEEN DATINI AND DATFIN ", CommandType.Text)) > 1)
                {
                    MSGErrore = "Attenzione... è già presente una delega per questa Azienda";
                    return false;
                }
                string str2 = this.objDataAccess.Get1ValueFromSQL(Convert.ToString(" SELECT VALUE(TRIM(CHAR(CODUTE),'') FROM ASSTER JOIN DELEGHE WHERE CURRENT_DATE BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31') " + (" AND CODTER = ", codTer).ToString()), CommandType.Text).ToString();
                if (str2 != null && !string.IsNullOrEmpty(str2.Trim()) && Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL("SELECT COUNT(*) FROM AZIUTE A, DELEGHE B " + " WHERE A.CODPOS = B.CODPOS AND A.CODPOS = " + codpos + " AND A.CODUTE = " + DBMethods.DoublePeakForSql(str2.Trim()) + " AND CURRENT_DATE BETWEEN B.DATINI AND B.DATFIN", CommandType.Text)) > 1)
                {
                    MSGErrore = "Attenzione... è già presente una delega per questa Azienda";
                    return false;
                }
            }
            if (strModo == "NUOVO")
            {
                if (!string.IsNullOrEmpty(strModo) && strModo != "NUOVO" && Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL("SELECT COUNT(*) FROM ASSTER WHERE RAGSOC = " + DBMethods.DoublePeakForSql(Deleghe.RagSocDe.Trim().ToUpper()), CommandType.Text)) == 0)
                {
                    MSGErrore = "Nessuna articolazione locale o studio di consulenza esistente con questa denominazione";
                    return false;
                }
                if (Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL("SELECT COUNT(*) FROM AZI WHERE CURRENT_DATE BETWEEN DATAPE AND VALUE(DATCHI, '9999-12-31') AND CODPOS='" + codpos + "' ", CommandType.Text)) == 0)
                {
                    MSGErrore = "L'azienda risulta chiusa. Impossibile inserire la delega";
                    return false;
                }
                if (Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL("SELECT COUNT(*) FROM DELEGHE " + " WHERE CODPOS = " + codpos + " AND CURRENT_DATE BETWEEN DATINI AND DATFIN ", CommandType.Text)) > 0)
                {
                    MSGErrore = "Attenzione... è già presente una delega per questa Azienda";
                    return false;
                }
                if (strModo != "NUOVO")
                {
                    object obj = (object)this.objDataAccess.Get1ValueFromSQL(Convert.ToString("SELECT MAX(DATFIN) FROM DELEGHE WHERE CODTER = " + codTer + " AND CODPOS = " + codpos) + " AND CURRENT_DATE NOT BETWEEN DATINI AND DATFIN ", CommandType.Text);
                    if (!string.IsNullOrEmpty(obj.ToString()) && Convert.ToDateTime(obj) > Convert.ToDateTime(str1))
                    {
                        MSGErrore = "Attenzione... in archivio esiste una delega valida fino al " + obj?.ToString() + ". Inserire una data maggiore";
                        return false;
                    }
                }
            }
            return true;
        }

        private string Module_GeneraPIN()
        {
            string str1 = "";
            string str2 = "0123456789";
            string str3 = "!$%&/()=?+-*_.:,;^|@[]";
            string str4 = "qwertyuioplkjhgfdsazxcvbnm";
            Random random = new Random();
            string str5 = str4 + str4.ToUpper();
            int length1 = str5.Length;
            int length2 = str2.Length;
            for (int index = 0; index <= 3; ++index)
                str1 += str5.Substring((int)Math.Round((double)random.Next(length1) + 1.0), 1);
            string str6 = str2 + str3;
            for (int index = 0; index <= 3; ++index)
                str1 += str6.Substring((int)Math.Round((double)random.Next(length2) + 1.0), 1);
            return str1;
        }

        public bool btnSalva_Click(
          TFI.OCM.Utente.Utente u,
          ref string MSGErrore,
          ref string MSGSuccess,
          DelegheOCM.DatiDelega Deleghe)
        {
            string str1 = "CURRENT_TIMESTAMP";
            DataTable dataTable1 = new DataTable();
            int num = 0;
            DataTable dataTable2 = new DataTable();
            string str2 = (string)null;
            bool flag = false;
            if (string.IsNullOrEmpty(Deleghe.ParIvaDe) && string.IsNullOrEmpty(Deleghe.CodFisDe))
            {
                MSGErrore = "Impossibile salvare delega, mancanza di codice fiscale e partita iva";
                return false;
            }
            string str3 = string.IsNullOrEmpty(Deleghe.ParIvaDe) ? Deleghe.CodFisDe : Deleghe.ParIvaDe;
            Deleghe.CodUteDe = str3;
            try
            {
                string strSQL = "SELECT CODCOM FROM CODCOM WHERE DENCOM = '" + Deleghe.ComuneDe + "'";
                Deleghe.CODCOM = this.objDataAccess.Get1ValueFromSQL(strSQL, CommandType.Text);
                this.objDataAccess.StartTransaction();
                if (Deleghe.strModo == "NUOVO")
                {
                    if (Deleghe.strModo == "NUOVO")
                    {
                        num = Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL(" Select VALUE(Max(CODTER), 0) + 1 FROM ASSTER WHERE CODTER < 99991", CommandType.Text));
                        string str4 = "INSERT INTO ASSTER (CODNAZ, CODTER, RAGSOC, " + " PARIVA, CODFIS, CODDUG, IND, NUMCIV, CAP, DENLOC, " + " SIGPRO, TEL1, TEL2, EMAIL, EMAILCERT, " + " UTEAGG, ULTAGG, CODCOM, CELL)" + " VALUES ( '" + Deleghe.codNaz + "', '" + Deleghe.codTer + "', '" + Deleghe.RagSocDe + "', '" + Deleghe.ParIvaDe + "', '" + Deleghe.CodFisDe + "', ";
                        string str5 = (!(Deleghe.ViaDe != "") ? str4 + "Null, '" : str4 + " " + DBMethods.DoublePeakForSql(Deleghe.ViaDe.ToString()) + ", '") + Deleghe.IndDe + "', '" + Deleghe.NumCivDe + "', '" + Deleghe.CapDe + "', '" + Deleghe.LocalitaDe + "', '" + Deleghe.PrDe + "', '" + Deleghe.TelDe + "', '" + Deleghe.TelDe + "', '" + Deleghe.EmailDe + "', '" + Deleghe.EmailCertDe + "', '" + u.Username + "', " + str1 + ", ";
                        flag = this.objDataAccess.WriteTransactionData((string.IsNullOrEmpty(Deleghe.CODCOM) ? str5 + " NULL, '" : str5 + " '" + Deleghe.CODCOM.ToString() + "', '") + Deleghe.CelDe + "') ", CommandType.Text);
                    }
                    else
                    {
                        string str6 = "UPDATE ASSTER SET ";
                        string str7 = " CODNAZ = '" + Deleghe.codNaz + "', " + " RAGSOC = '" + Deleghe.RagSocDe + "', " + " PARIVA = '" + Deleghe.ParIvaDe + "', " + " CODFIS = '" + Deleghe.CodFisDe + "', ";
                        string str8 = (!(Deleghe.CODDUG == "") ? str7 + " CODDUG = Null, " : str7 + " CODDUG = '" + Deleghe.CODDUG + "', ") + " IND = '" + Deleghe.IndDe + "', " + " NUMCIV = '" + Deleghe.NumCivDe + "', " + " CAP = '" + Deleghe.CapDe + "', " + " DENLOC = '" + Deleghe.LocalitaDe + "', " + " SIGPRO = '" + Deleghe.PrDe + "', " + " TEL1 = '" + Deleghe.TelDe + "', " + " TEL2 = '" + Deleghe.TelDe + "', " + " CELL = '" + Deleghe.CelDe + "', ";
                        str6 = (!(Deleghe.ComuneDe != "") ? str8 + " CODCOM = Null, " : str8 + " CODCOM = '" + Deleghe.ComuneDe.ToString() + "', ") + " EMAIL = '" + Deleghe.EmailDe + "', " + " EMAILCERT = '" + Deleghe.EmailCertDe + "', " + " DATSAP = NULL," + " UTEAGG = '" + u.Username + "', " + " ULTAGG = CURRENT_TIMESTAMP ";
                        flag = this.objDataAccess.WriteTransactionData(" WHERE CODTER = '" + Deleghe.codTer + "', ", CommandType.Text);
                    }
                    flag = this.objDataAccess.WriteTransactionData("DELETE FROM GRUDEL " + " WHERE CODTER = '" + Deleghe.codTer + "' " + " AND CODPOS = '" + u.CodPosizione + "' " + " AND DATINI = " + str1 + " ", CommandType.Text);
                    flag = this.objDataAccess.WriteTransactionData("DELETE FROM DELEGHE " + " WHERE CODTER = '" + Deleghe.codTer + "' " + " AND CODPOS = '" + u.CodPosizione + "' " + " AND DATINI =  " + str1 + "  ", CommandType.Text);
                    flag = this.objDataAccess.WriteTransactionData("INSERT INTO DELEGHE (CODTER, CODPOS, DATINI, DATFIN, ULTAGG , UTEAGG, FLGAPP, FLGATT)" + " VALUES ( '" + Deleghe.codTer + "', '" + u.CodPosizione + "', " + str1 + ", " + "'9999-12-31', " + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ", 'I', '0')", CommandType.Text);
                    flag = this.objDataAccess.WriteTransactionData("UPDATE AZI SET FLGAPP = 'M' WHERE CODPOS = " + u.CodPosizione, CommandType.Text);
                    string str9 = "Insert Into GRUDEL (CODTER, CODPOS, DATINI, DATFIN, CODGRU, ULTAGG , UTEAGG)" + " Values ( '" + Deleghe.codTer + "','" + u.CodPosizione + "', " + str1 + ", " + "'9999-12-31', ";
                    flag = this.objDataAccess.WriteTransactionData((Convert.ToDouble(Deleghe.NATGIU) != 10.0 ? str9 + " 8, " : str9 + " 46, ") + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ")", CommandType.Text);
                    string str10 = this.objDataAccess.Get1ValueFromSQL(" SELECT VALUE(TRIM(CHAR(CODUTE),'') FROM ASSTER WHERE CODTER= '" + num.ToString() + "' " + " AND CODTER IN  (SELECT CODTER FROM DELEGHE WHERE CODTER = '" + Deleghe.codTer + "' " + " AND  CURRENT_DATE BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31') " + " AND CODTER = '" + num.ToString() + "' )", CommandType.Text).ToString();
                    if (!string.IsNullOrEmpty(str10.Trim()) && Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL("SELECT COUNT(*) FROM AZIUTE A, DELEGHE B " + " WHERE A.CODPOS = B.CODPOS AND A.CODPOS = " + u.CodPosizione + " AND A.CODUTE = " + DBMethods.DoublePeakForSql(str10.Trim()) + " AND CURRENT_DATE BETWEEN B.DATINI AND B.DATFIN", CommandType.Text)) > 0)
                    {
                        MSGErrore = "Attenzione... è già presente una delega per questa Azienda";
                        return false;
                    }
                    DataTable dataTable3 = this.objDataAccess.GetDataTable(" SELECT CODUTE FROM ASSTER WHERE ASSTER.CODTER = '" + Deleghe.codTer + "' " + " AND ASSTER.CODTER IN  (SELECT DELEGHE.CODTER FROM DELEGHE WHERE DELEGHE.CODTER ='" + Deleghe.codTer + "' " + " AND CURRENT_DATE BETWEEN DELEGHE.DATINI AND DELEGHE.DATFIN AND DELEGHE.CODPOS <> '" + u.CodPosizione + "')");
                    if (dataTable3.Rows.Count > 0)
                    {
                        if ((Deleghe.CodFisDe ?? "") != (str3 ?? ""))
                        {
                            if (Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL(" SELECT COUNT(*) FROM UTENTI WHERE CODUTE = " + DBMethods.DoublePeakForSql(str3), CommandType.Text).ToString()) == 0)
                            {
                                flag = this.objDataAccess.WriteTransactionData("Insert Into UTENTI (CODUTE , DENUTE,  CODTIPUTE, CODFIS, ULTAGG , UTEAGG)" + " Values ( " + DBMethods.DoublePeakForSql(str3) + "', " + Deleghe.RagSocDe + "', " + "'C', " + DBMethods.DoublePeakForSql(str3) + "', " + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + "')", CommandType.Text);
                                flag = this.objDataAccess.WriteTransactionData("UPDATE UTEPIN SET CODUTE = '" + DBMethods.DoublePeakForSql(str3) + "', " + " ULTAGG = CURRENT_TIMESTAMP, UTEAGG = '" + DBMethods.DoublePeakForSql(u.Username) + " WHERE CODUTE = '" + DBMethods.DoublePeakForSql(str3), CommandType.Text);
                            }
                            else if (Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL(" SELECT COUNT(*) FROM UTENTI WHERE CODTIPUTE = 'I' AND CODUTE = " + DBMethods.DoublePeakForSql(str3), CommandType.Text).ToString()) > 0)
                                flag = this.objDataAccess.WriteTransactionData("UPDATE UTENTI SET CODTIPUTE2 = 'C', ULTAGG = CURRENT_TIMESTAMP, UTEAGG = '" + DBMethods.DoublePeakForSql(u.Username) + " WHERE CODUTE = '" + DBMethods.DoublePeakForSql(str3), CommandType.Text);
                            flag = this.objDataAccess.WriteTransactionData("UPDATE AZIUTE SET CODUTE = " + DBMethods.DoublePeakForSql(str3) + ", " + " ULTAGG = CURRENT_TIMESTAMP, UTEAGG = " + DBMethods.DoublePeakForSql(u.Username) + " WHERE CODUTE = " + DBMethods.DoublePeakForSql(dataTable3.Rows[0]["CODUTE"].ToString().Trim()), CommandType.Text);
                            flag = this.objDataAccess.WriteTransactionData("INSERT INTO AZIUTE (CODPOS, CODUTE, ULTAGG, UTEAGG) " + " VALUES ( " + u.CodPosizione + "', " + DBMethods.DoublePeakForSql(str3) + "', " + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + "')", CommandType.Text);
                            if (!string.IsNullOrEmpty(dataTable3.Rows[0]["CODUTE"].ToString().Trim()))
                                flag = this.objDataAccess.WriteTransactionData("DELETE FROM UTENTI WHERE CODUTE = '" + DBMethods.DoublePeakForSql(dataTable3.Rows[0]["CODUTE"].ToString().Trim()), CommandType.Text);
                        }
                        else
                            flag = this.objDataAccess.WriteTransactionData("INSERT INTO AZIUTE (CODPOS, CODUTE, ULTAGG, UTEAGG) " + " VALUES ( " + u.CodPosizione + ", " + DBMethods.DoublePeakForSql(str3) + ", " + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ")", CommandType.Text);
                    }
                }
                else
                {
                    if (Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL(" SELECT COUNT(*) FROM ASSTER,DELEGHE WHERE ASSTER.CODTER = '" + Deleghe.codTer + "' AND ASSTER.CODTER=DELEGHE.CODTER " + " AND ASSTER.CODTER IN  (SELECT CODTER FROM DELEGHE WHERE DELEGHE.CODTER = '" + Deleghe.codTer + "' " + " AND CURRENT_DATE BETWEEN DELEGHE.DATINI AND DELEGHE.DATFIN AND CODPOS <> '" + u.CodPosizione + "')" + " AND CURRENT_DATE BETWEEN DELEGHE.DATINI AND DELEGHE.DATFIN", CommandType.Text)) > 0)
                    {
                        MSGErrore = "Attenzione... e' già presente un'associazione territoriale che opera sulle Aziende. Impossibile continuare l'operazione";
                        return false;
                    }
                    if (Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL("SELECT COUNT(*) AS TOT FROM DELEGHE WHERE CODTER = " + Deleghe.codTer + " AND CURRENT_DATE BETWEEN Deleghe.DATINI AND Deleghe.DATFIN ", CommandType.Text)) == 0)
                    {
                        MSGErrore = "Attenzione... l'associazione territoriale  non ha Deleghe. Impossibile continuare l'operazione";
                        return false;
                    }
                    DataTable dataTable4 = this.objDataAccess.GetDataTable("SELECT * FROM AZIUTE, UTENTI " + " WHERE AZIUTE.CODUTE = UTENTI.CODUTE AND UTENTI.CODTIPUTE = 'C' AND CODPOS IN (SELECT CODPOS FROM DELEGHE WHERE CODTER = " + Deleghe.codTer + " AND CURRENT_DATE BETWEEN Deleghe.DATINI AND Deleghe.DATFIN) ");
                    if (dataTable4.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL(" SELECT COUNT(*) FROM DELEGHE" + " WHERE CODPOS = '" + dataTable4.Rows[0]["CODPOS"]?.ToString() + "' AND CODTER <> '" + Deleghe.codTer + "' " + " AND CURRENT_DATE BETWEEN Deleghe.DATINI AND Deleghe.DATFIN", CommandType.Text)) > 0)
                        {
                            MSGErrore = "Attenzione... e' già presente un' associazione territoriale che opera sulle Aziende. Impossibile continuare l'operazione";
                            return false;
                        }
                    }
                    if (Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL("SELECT COUNT(*) FROM UTENTI WHERE CODUTE = " + DBMethods.DoublePeakForSql(str3), CommandType.Text)) == 0)
                    {
                        if (this.objDataAccess.Get1ValueFromSQL(" SELECT COUNT(*) FROM UTENTI WHERE CODTIPUTE = 'I' AND CODUTE = " + DBMethods.DoublePeakForSql(str3), CommandType.Text).ToString() != "")
                        {
                            flag = this.objDataAccess.WriteTransactionData("UPDATE UTENTI SET CODTIPUTE2 = 'C', ULTAGG = CURRENT_TIMESTAMP, UTEAGG = " + DBMethods.DoublePeakForSql(u.Username) + " WHERE CODUTE = " + DBMethods.DoublePeakForSql(str3), CommandType.Text);
                            str2 = Cypher.DeCryptPassword(this.objDataAccess.Get1ValueFromSQL(" SELECT PIN FROM UTEPIN WHERE CODUTE = " + DBMethods.DoublePeakForSql(str3) + " AND CURRENT_DATE BETWEEN DATINI AND DATFIN", CommandType.Text).ToString());
                        }
                        else
                        {
                            flag = this.objDataAccess.WriteTransactionData("Insert Into UTENTI (CODUTE , DENUTE,  CODTIPUTE, CODFIS, ULTAGG , UTEAGG)" + " Values ( " + DBMethods.DoublePeakForSql(str3) + ", " + Deleghe.RagSocDe + "', " + "'C', " + Deleghe.CodFisDe + "', " + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ")", CommandType.Text);
                            flag = this.objDataAccess.WriteTransactionData("INSERT INTO UTEPIN (CODUTE, PIN, DATINI, DATFIN, STAPIN, ULTAGG, UTEAGG) " + " VALUES (" + DBMethods.DoublePeakForSql(str3) + ", " + DBMethods.DoublePeakForSql(Cypher.DeCryptPassword(this.Module_GeneraPIN())) + ", " + " CURRENT_DATE, " + "'1899-12-31', " + "'A', " + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ")", CommandType.Text);
                        }
                    }
                    else if (this.objDataAccess.Get1ValueFromSQL(" SELECT COUNT(*) FROM UTENTI WHERE CODTIPUTE = 'I' AND CODUTE = " + DBMethods.DoublePeakForSql(str3), CommandType.Text).ToString() != "")
                        flag = this.objDataAccess.WriteTransactionData("UPDATE UTENTI SET CODTIPUTE2 = 'C', ULTAGG = CURRENT_TIMESTAMP, UTEAGG = " + DBMethods.DoublePeakForSql(u.Username) + " WHERE CODUTE = " + DBMethods.DoublePeakForSql(str3), CommandType.Text);
                    flag = this.objDataAccess.WriteTransactionData("INSERT INTO AZIUTE (CODPOS, CODUTE, ULTAGG, UTEAGG) " + " VALUES ( " + u.CodPosizione + ", " + DBMethods.DoublePeakForSql(str3) + ", " + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ")", CommandType.Text);
                }
                string str11 = "UPDATE ASSTER SET " + " CODNAZ = '" + Deleghe.codNaz + "', " + " RAGSOC = '" + Deleghe.RagSocDe + "', " + " RAGSOCBRE = '" + Deleghe.RagSocBrDe + "', " + " PARIVA = '" + Deleghe.ParIvaDe + "', " + " CODFIS = '" + Deleghe.CodFisDe + "', ";
                string str12 = (!(Deleghe.CODDUG == "") ? str11 + " CODDUG = Null, " : str11 + " CODDUG = " + DBMethods.DoublePeakForSql(Convert.ToString(Deleghe.CODDUG)) + ", ") + " IND = '" + Deleghe.IndDe + "', " + " NUMCIV = '" + Deleghe.NumCivDe + "', " + " CAP = '" + Deleghe.CapDe + "', " + " DENLOC = '" + Deleghe.LocalitaDe + "', " + " SIGPRO = '" + Deleghe.PrDe + "', " + " TEL1 = '" + Deleghe.TelDe + "', " + " TEL2 = '" + Deleghe.TelDe + "', " + " FAX = '" + Deleghe.FaxDe + "', " + " CELL = '" + Deleghe.CelDe + "', ";
                flag = this.objDataAccess.WriteTransactionData((!(Deleghe.CODCOM != "") ? str12 + " CODCOM = Null, " : str12 + " CODCOM = '" + Deleghe.CODCOM.ToString() + "', ") + " EMAIL = '" + Deleghe.EmailDe + "', " + " EMAILCERT = '" + Deleghe.EmailCertDe + "', " + " DATSAP = NULL," + " UTEAGG = " + DBMethods.DoublePeakForSql(u.Username) + ", " + " ULTAGG = CURRENT_TIMESTAMP, " + " CODUTE = " + DBMethods.DoublePeakForSql(str3) + " " + " WHERE CODTER = " + DBMethods.DoublePeakForSql(Deleghe.codTer) + " ", CommandType.Text);
                if (string.IsNullOrEmpty(str3) != string.IsNullOrEmpty(Deleghe.CodFisDe))
                {
                    flag = this.objDataAccess.WriteTransactionData("DELETE FROM UTEPIN WHERE CODUTE = " + DBMethods.DoublePeakForSql(str3), CommandType.Text);
                    if (this.objDataAccess.Get1ValueFromSQL(" SELECT COUNT(*) FROM UTENTI WHERE CODTIPUTE = 'I' AND CODUTE = " + DBMethods.DoublePeakForSql(str3), CommandType.Text).ToString() != "")
                    {
                        flag = this.objDataAccess.WriteTransactionData("DELETE FROM UTENTI WHERE CODUTE = " + DBMethods.DoublePeakForSql(str3), CommandType.Text);
                        flag = this.objDataAccess.WriteTransactionData("Insert Into UTENTI (CODUTE , DENUTE,  CODTIPUTE, CODTIPUTE2, CODFIS, ULTAGG , UTEAGG)" + " Values ( " + DBMethods.DoublePeakForSql(str3) + ", '" + Deleghe.RagSocDe + "', " + "'C', 'I', " + DBMethods.DoublePeakForSql(str3) + ", " + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ")", CommandType.Text);
                    }
                    else
                    {
                        flag = this.objDataAccess.WriteTransactionData("DELETE FROM UTENTI WHERE CODUTE = " + DBMethods.DoublePeakForSql(str3), CommandType.Text);
                        flag = this.objDataAccess.WriteTransactionData("Insert Into UTENTI (CODUTE , DENUTE,  CODTIPUTE, CODFIS, ULTAGG , UTEAGG)" + " Values ( " + DBMethods.DoublePeakForSql(str3) + ", " + Deleghe.RagSocDe + "', " + "'C', " + Deleghe.CodFisDe + "', " + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + "')", CommandType.Text);
                    }
                    flag = this.objDataAccess.WriteTransactionData("INSERT INTO UTEPIN (CODUTE, PIN, DATINI, DATFIN, STAPIN, ULTAGG, UTEAGG) " + " VALUES (" + DBMethods.DoublePeakForSql(str3) + ", " + DBMethods.DoublePeakForSql(Cypher.CryptPassword(this.Module_GeneraPIN())) + ", " + " CURRENT_DATE, " + "'1899-12-31', " + "'A', " + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ")", CommandType.Text);
                    flag = this.objDataAccess.WriteTransactionData("UPDATE AZIUTE SET CODUTE = " + DBMethods.DoublePeakForSql(str3) + ", " + " ULTAGG = CURRENT_TIMESTAMP, UTEAGG = " + DBMethods.DoublePeakForSql(u.Username) + " WHERE CODUTE = " + DBMethods.DoublePeakForSql(str3), CommandType.Text);
                }
                MSGSuccess = "Operazione completata";
            }
            catch (Exception ex)
            {
                this.objDataAccess.EndTransaction(false);
                MSGErrore = "Errore nel salvataggio causa mancanza dati ";
                return false;
            }
            this.objDataAccess.EndTransaction(true);
            return true;
        }

        public bool Invia_Email(string ALLEGATO, DelegheOCM.DatiDelega Deleghe)
        {
            try
            {
                string str = "Come da richiesta on line, si invia il codice PIN MULTIAZIENDALE per l’accesso ai servizi on line della Gestione Ordinaria.<BR>" + "Cordiali Saluti<BR><BR>" + "Ilaria Daddi<BR>" + "Responsabile Attività Contributi e Riscossione<BR>" + "Tel. 06/5458235 - Fax 06/5458385<BR><BR><BR>" + "Le informazioni contenute nella presente comunicazione e i relativi allegati possono essere riservate e<BR>" + "sono, comunque, destinate esclusivamente alle persone o alla Società sopraindicati. La diffusione,<BR>" + "distribuzione e/o copiatura del documento trasmesso da parte di qualsiasi soggetto diverso dal destinatario<BR>" + "è proibita, sia ai sensi dell'art. 616 c.p. , che ai sensi del D.Lgs. n. 196/2003. Se avete ricevuto questo<BR>" + "messaggio per errore, vi preghiamo di distruggerlo e di informarci immediatamente inviando un messaggio<BR>" + "al presente indirizzo e-mail.<BR>Grazie";
                MailMessage message = new MailMessage();
                message.Headers.Add("Disposition-notification-to", "contributi@enpaia.it");
                message.From = new MailAddress("comunicazioni_istituzionali@enpaia.it", "Fondazione ENPAIA");
                message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
                message.Priority = MailPriority.High;
                message.To.Add(Deleghe.EmailCertDe.ToLower());
                message.Attachments.Add(new Attachment(ALLEGATO));
                message.Subject = "INVIO PIN CONSULENTE '" + Deleghe.codTer + "' - '" + Deleghe.RagSocDe + "' ";
                message.Body = str;
                message.IsBodyHtml = true;
                new SmtpClient("mailweb.enpaia.it")
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = false,
                    Credentials = ((ICredentialsByHost)new NetworkCredential(ConfigurationManager.AppSettings.Get("UserMail").ToString(), ConfigurationManager.AppSettings.Get("PwdMail").ToString()).GetCredential(ConfigurationManager.AppSettings.Get("IndirizzoMail").ToString(), Convert.ToInt32(ConfigurationManager.AppSettings.Get("PortaMail").ToString()), ConfigurationManager.AppSettings.Get("TipoAutenticazione").ToString()))
                }.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Credenziali per invio mail non valide");
            }
        }

        public DelegheOCM CARICA_IVA(ref string MSGErrore, string codfis, string pariva)
        {
            DelegheOCM delegheOcm = new DelegheOCM();
            DataTable dataTable1 = new DataTable();
            try
            {
                string strSQL = " SELECT * FROM ASSTER";
                if (string.IsNullOrEmpty(pariva.Trim()) & !string.IsNullOrEmpty(codfis))
                    strSQL = strSQL + " WHERE CODFIS = " + DBMethods.DoublePeakForSql(codfis);
                if (!string.IsNullOrEmpty(pariva.Trim()) & string.IsNullOrEmpty(codfis))
                    strSQL = strSQL + " WHERE PARIVA = " + DBMethods.DoublePeakForSql(pariva);
                if (!string.IsNullOrEmpty(pariva.Trim()) & !string.IsNullOrEmpty(codfis))
                    strSQL = strSQL + " WHERE PARIVA = " + DBMethods.DoublePeakForSql(pariva);
                dataTable1.Clear();
                DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL);
                if (dataTable2 != null)
                {
                    if (dataTable2.Rows.Count > 0)
                    {
                        if (dataTable2.Rows.Count == 1)
                        {
                            this.Deleghe.datidelega.EmailCertDe = dataTable2.Rows[0]["EMAILCERT"].ToString();
                            this.Deleghe.datidelega.CelDe = dataTable2.Rows[0]["CELL"].ToString();
                            this.Deleghe.datidelega.RagSocBrDe = dataTable2.Rows[0]["RAGSOCBRE"].ToString();
                            this.Deleghe.datidelega.codNaz = dataTable2.Rows[0]["CODNAZ"].ToString();
                            this.Deleghe.datidelega.codTer = dataTable2.Rows[0]["CODTER"].ToString();
                            this.Deleghe.datidelega.AssStudio = dataTable2.Rows[0]["RAGSOC"].ToString();
                            this.Deleghe.datidelega.CodFisDe = dataTable2.Rows[0]["CODFIS"].ToString();
                            this.Deleghe.datidelega.IndDe = dataTable2.Rows[0]["IND"].ToString();
                            this.Deleghe.datidelega.CapDe = dataTable2.Rows[0]["CAP"].ToString();
                            this.Deleghe.datidelega.NumCivDe = dataTable2.Rows[0]["NUMCIV"].ToString();
                            this.Deleghe.datidelega.CODDUG = dataTable2.Rows[0]["CODDUG"].ToString();
                            this.Deleghe.datidelega.EmailDe = dataTable2.Rows[0]["EMAIL"].ToString();
                            this.Deleghe.datidelega.FaxDe = dataTable2.Rows[0]["FAX"].ToString();
                            this.Deleghe.datidelega.LocalitaDe = dataTable2.Rows[0]["DENLOC"].ToString();
                            this.Deleghe.datidelega.PrDe = dataTable2.Rows[0]["SIGPRO"].ToString();
                            this.Deleghe.datidelega.TelDe = dataTable2.Rows[0]["TEL1"].ToString();
                            this.Deleghe.datidelega.TelDe = dataTable2.Rows[0]["TEL2"].ToString();
                            this.Deleghe.datidelega.ParIvaDe = dataTable2.Rows[0]["PARIVA"].ToString();
                            if (!string.IsNullOrEmpty(dataTable2.Rows[0]["CODCOM"].ToString().Trim()))
                            {
                                this.Deleghe.datidelega.ComuneDe = this.objDataAccess.Get1ValueFromSQL("SELECT VALUE(DENCOM,'') AS DENCOM FROM CODCOM WHERE CODCOM = " + DBMethods.DoublePeakForSql(dataTable2.Rows[0]["CODCOM"].ToString().Trim()), CommandType.Text).ToString().Trim();
                                this.Deleghe.datidelega.ComuneDe = dataTable2.Rows[0]["CODCOM"].ToString().Trim();
                            }
                            else
                                this.Deleghe.datidelega.ComuneDe = "";
                        }
                        else if (string.IsNullOrEmpty(pariva) && string.IsNullOrEmpty(codfis.Trim()))
                        {
                            this.Deleghe.datidelega.EmailCertDe = dataTable2.Rows[0]["EMAILCERT"].ToString();
                            this.Deleghe.datidelega.CelDe = dataTable2.Rows[0]["CELL"].ToString();
                            this.Deleghe.datidelega.RagSocBrDe = dataTable2.Rows[0]["RAGSOCBRE"].ToString();
                            this.Deleghe.datidelega.codNaz = dataTable2.Rows[0]["CODNAZ"].ToString();
                            this.Deleghe.datidelega.codTer = dataTable2.Rows[0]["CODTER"].ToString();
                            this.Deleghe.datidelega.RagSocDe = dataTable2.Rows[0]["RAGSOC"].ToString();
                            this.Deleghe.datidelega.CodFisDe = dataTable2.Rows[0]["CODFIS"].ToString();
                            this.Deleghe.datidelega.IndDe = dataTable2.Rows[0]["IND"].ToString();
                            this.Deleghe.datidelega.CapDe = dataTable2.Rows[0]["CAP"].ToString();
                            this.Deleghe.datidelega.NumCivDe = dataTable2.Rows[0]["NUMCIV"].ToString();
                            this.Deleghe.datidelega.CODDUG = dataTable2.Rows[0]["CODDUG"].ToString();
                            this.Deleghe.datidelega.EmailDe = dataTable2.Rows[0]["EMAIL"].ToString();
                            this.Deleghe.datidelega.FaxDe = dataTable2.Rows[0]["FAX"].ToString();
                            this.Deleghe.datidelega.LocalitaDe = dataTable2.Rows[0]["DENLOC"].ToString();
                            this.Deleghe.datidelega.PrDe = dataTable2.Rows[0]["SIGPRO"].ToString();
                            this.Deleghe.datidelega.TelDe = dataTable2.Rows[0]["TEL1"].ToString();
                            this.Deleghe.datidelega.TelDe = dataTable2.Rows[0]["TEL2"].ToString();
                            this.Deleghe.datidelega.ParIvaDe = dataTable2.Rows[0]["PARIVA"].ToString();
                            if (!string.IsNullOrEmpty(dataTable2.Rows[0]["CODCOM"].ToString()))
                            {
                                this.Deleghe.datidelega.ComuneDe = dataTable2.Rows[0]["DENCOM"].ToString();
                                this.Deleghe.datidelega.ComuneDe = dataTable2.Rows[0]["CODCOM"].ToString();
                            }
                            else
                                this.Deleghe.datidelega.ComuneDe = "";
                        }
                    }
                    if (string.IsNullOrEmpty(this.Deleghe.datidelega.codTer))
                        MSGErrore = "Nella base dati non sono presenti associazioni/studi con questa partita I.V.A.";
                }
                else if (string.IsNullOrEmpty(this.Deleghe.datidelega.codNaz))
                    MSGErrore = "Nella base dati non sono presenti associazioni/studi con questa partita I.V.A.";
            }
            catch (Exception ex)
            {
                MSGErrore = "Errore nel caricamento dei dati. Riprovare" + ex.Message;
                return (DelegheOCM)null;
            }
            delegheOcm.datidelega = this.Deleghe.datidelega;
            return delegheOcm;
        }

        public string GeneraOTPDeleghe()
        {
            string str1 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ" + "abcdefghijklmnopqrstuvwxyz" + "1234567890";
            int num = 8;
            string str2 = "";
            for (int index1 = 0; index1 < num; ++index1)
            {
                string str3;
                do
                {
                    int index2 = new Random().Next(0, str1.Length);
                    str3 = str1.ToCharArray()[index2].ToString();
                }
                while (str2.IndexOf(str3) != -1);
                str2 += str3;
            }
            return str2;
        }

        public bool SalvaOTPDeleghe(string codUte, string otp)
        {
            try
            {
                return new DataLayer().WriteData("UPDATE UTENTI SET OTPDLG = '" + otp + "' WHERE CODUTE = '" + codUte + "'", CommandType.Text);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public DelegheOCM GetCodNazAssTerAssNaz(string codTer)
        {
            DataTable resultTable = new DataTable();
            DelegheOCM delegaPerSopra = new DelegheOCM();

            string assTerAssNazSqlQry =
                $"SELECT ASSTER.RAGSOC, ASSNAZ.RAGSOC AS ASNAZ, ASSNAZ.CODNAZ FROM ASSTER JOIN ASSNAZ ON ASSNAZ.CODNAZ = ASSTER.CODNAZ " +
                $"WHERE ASSTER.CODTER = {codTer}";
            resultTable = objDataAccess.GetDataTable(assTerAssNazSqlQry);
            if (resultTable != null && resultTable.Rows.Count > 0)
            {
                delegaPerSopra.datidelega.AssStudio = resultTable.Rows[0]["ASNAZ"].ToString();
                delegaPerSopra.datidelega.RagSocDe = resultTable.Rows[0]["RAGSOC"].ToString();
                delegaPerSopra.datidelega.codNaz = resultTable.Rows[0]["CODNAZ"].ToString();
            }

            return delegaPerSopra;
        }

        private bool UpdateDeleghe(string codter, string codpos, string u, string updateDelegheSqlCmd)
        {
            objDataAccess.StartTransaction();
            try
            {
                var usernameParam = objDataAccess.CreateParameter("@username", iDB2DbType.iDB2VarChar, 20, ParameterDirection.Input, u);
                var codposParam = objDataAccess.CreateParameter("@codpos", iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codpos);
                var codterParam = objDataAccess.CreateParameter("@codter", iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input, codter);


                var result = objDataAccess.WriteTransactionDataWithParametersAndDontCall(updateDelegheSqlCmd, CommandType.Text, usernameParam,
                    codposParam, codterParam, codposParam, codterParam);

                objDataAccess.EndTransaction(result);
                return result;
            }
            catch
            {
                objDataAccess.EndTransaction(false);
                return false;
            }
        }

        public void GetFlagAttDelega(string codTer, string codPos, string datini, DelegheOCM delegaPerSopra)
        {
            string infoDelegaSqlQry = $"SELECT FLGATT FROM DELEGHE WHERE CODTER = {codTer} AND CODPOS = {codPos} AND DATINI = '{datini}'";
            string flagAtt = objDataAccess.Get1ValueFromSQL(infoDelegaSqlQry, CommandType.Text);

            if (flagAtt == "0")
                delegaPerSopra.datidelega.IsCancellata = true;
            else if (flagAtt == "1")
                delegaPerSopra.datidelega.IsAttiva = true;
            else
                delegaPerSopra.datidelega.IsInAttesa = true;
        }

        public DelegheOCM.DatiDelega GetInfoForDelega(string codter, string codpos)
        {
            var codposParam = objDataAccess.CreateParameter("@codpos", iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codpos);
            var codterParam = objDataAccess.CreateParameter("@codter", iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input, codter);

            var getAssInfoSqlQuery = "SELECT RAGSOC, EMAIL, EMAILCERT " +
                                     "FROM ASSTER " +
                                     "WHERE CODTER = @codter";

            var getAziInfoSqlQuery = "SELECT AZ.RAGSOC, AM.EMAIL, AM.EMAILCERT " +
                                     "FROM AZI AZ " +
                                     "INNER JOIN AZEMAIL AM ON AZ.CODPOS = AM.CODPOS " +
                                     "WHERE AZ.CODPOS = @codpos AND AM.DATINI = " +
                                        "(SELECT MAX(DATINI) FROM AZEMAIL WHERE CODPOS = @codpos)";

            var assInfo = objDataAccess.GetDataTableWithParameters(getAssInfoSqlQuery, codterParam).Rows[0];
            var aziInfo = objDataAccess.GetDataTableWithParameters(getAziInfoSqlQuery, codposParam, codposParam).Rows[0];

            return new DelegheOCM.DatiDelega
            {
                EmailAz = aziInfo.ElementAt("EMAIL"),
                EmailCertAz = aziInfo.ElementAt("EMAILCERT"),
                RagSocAz = aziInfo.ElementAt("RAGSOC"),
                EmailDe = assInfo.ElementAt("EMAIL"),
                EmailCertDe = assInfo.ElementAt("EMAILCERT"),
                RagSocDe = assInfo.ElementAt("RAGSOC")
            };
        }

        public bool HaDelgaAttiva(string codpos)
        {
            string delegheAttiveSqlQry = $"SELECT * FROM DELEGHE WHERE CODPOS = {codpos} AND FLGATT = 1";
            DataTable delegheAttive = objDataAccess.GetDataTable(delegheAttiveSqlQry);
            return delegheAttive.Rows.Count > 0;
        }
    }
}
