// Decompiled with JetBrains decompiler
// Type: TFI.DAL.AziendaConsulente.CessazioniRdlDal
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.AziendaConsulente;
using TFI.OCM.Iscritto;
using Utilities;


namespace TFI.DAL.AziendaConsulente
{
    public class CessazioniRdlDal
    {
        public List<CessazioniRdl.Causas> Causales(string CodPos)
        {
            DataLayer dataLayer = new DataLayer();
            string strSQL = "SELECT CODCAUCES, DENCES FROM CAUCES WHERE WEB = 'S' ORDER BY DENCES";
            DataTable dataTable = new DataTable();
            dataTable = dataLayer.GetDataTable(strSQL);
            string Err = "";
            DataSet dataSet1 = new DataSet();
            DataSet dataSet2 = dataLayer.GetDataSet(strSQL, ref Err);
            CessazioniRdl cessazioniRdl = new CessazioniRdl();
            foreach (DataRow row in (InternalDataCollectionBase)dataSet2.Tables[0].Rows)
            {
                CessazioniRdl.Causas causas = new CessazioniRdl.Causas()
                {
                    Causale2 = row["DENCES"].ToString()
                };
                cessazioniRdl.causas.Add(causas);
            }
            return cessazioniRdl.causas;
        }

        public List<CessazioniRdl.Matricola> Matricolas(string CodPos)
        {
            DataLayer dataLayer = new DataLayer();
            string strSQL = "SELECT TRIM(CHAR(MAT)) AS MAT, TRIM(CHAR(MAT)) || ';' || TRIM(CHAR(PRORAP)) || ';' || TRIM(CHAR(DATDEC))  AS VALORE FROM RAPLAV WHERE CURRENT_DATE BETWEEN DATDEC AND VALUE(DATCES, '9999-12-31') AND (DATCES IS NULL OR DATCES = '9999-12-31') AND CodPos = '" + CodPos + "' ORDER BY MAT";
            DataTable dataTable = new DataTable();
            dataTable = dataLayer.GetDataTable(strSQL);
            string Err = "";
            DataSet dataSet1 = new DataSet();
            DataSet dataSet2 = dataLayer.GetDataSet(strSQL, ref Err);
            CessazioniRdl cessazioniRdl = new CessazioniRdl();
            foreach (DataRow row in (InternalDataCollectionBase)dataSet2.Tables[0].Rows)
            {
                CessazioniRdl.Matricola matricola = new CessazioniRdl.Matricola()
                {
                    Matricola1 = row["MAT"].ToString(),
                    ProRap = row["VALORE"].ToString().Split(';')[1]
                };
                cessazioniRdl.matricolas.Add(matricola);
            }
            return cessazioniRdl.matricolas;
        }

        public Anagrafica GetDatiAnagrafici(string matricola)
        {
            try
            {
                DataLayer dataLayer = new DataLayer();
                iDB2Parameter parameter = dataLayer.CreateParameter("@mat", iDB2DbType.iDB2Integer, 20, ParameterDirection.Input, matricola);
                string strSQL = "SELECT I.COG, I.NOM, I.DATISC, (SELECT IBAN FROM TBIBAN WHERE MAT = @mat AND DATINI = (SELECT MAX(DATINI) FROM TBIBAN WHERE MAT = @mat)) " +
                                "AS IBAN FROM ISCT I INNER JOIN ISCD D ON I.MAT = D.MAT WHERE I.MAT = @mat";
                DataTable tableWithParameters = dataLayer.GetDataTableWithParameters(strSQL, parameter, parameter, parameter);
                if (tableWithParameters == null || tableWithParameters.Rows.Count <= 0)
                    return (Anagrafica)null;
                return new Anagrafica()
                {
                    Nome = !DBNull.Value.Equals(tableWithParameters.Rows[0]["NOM"]) ? tableWithParameters.Rows[0]["NOM"].ToString().Trim() : string.Empty,
                    Cognome = !DBNull.Value.Equals(tableWithParameters.Rows[0]["COG"]) ? tableWithParameters.Rows[0]["COG"].ToString().Trim() : string.Empty,
                    DataIscrizione = !DBNull.Value.Equals(tableWithParameters.Rows[0]["DATISC"]) ? Convert.ToDateTime(tableWithParameters.Rows[0]["DATISC"]).ToString("yyyy-MM-dd") : string.Empty,
                    Iban = tableWithParameters.Rows[0]["IBAN"]?.ToString().Trim()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<CessazioniRdl.Listas> CessazioniRdlGet(string CodPos)
        {
            DataLayer dataLayer = new DataLayer();
            string strSQL = "SELECT ISCT.MAT, ISCT.COG, ISCT.NOM, raplav.datprotCES AS datprot, raplav.protCES as strprot, TRANSLATE(CHAR(raplav.datdec, EUR),'/','.') AS DATDEC," +
                " TRANSLATE(CHAR(raplav.datces, EUR),'/','.') AS DATCES, TRANSLATE(CHAR(raplav.datass, EUR),'/','.') AS DATASS, CAUCES.DENCES, RAPLAV.CodPos, RAPLAV.PRORAP " +
                "FROM ISCT INNER JOIN RAPLAV ON ISCT.MAT = RAPLAV.MAT INNER JOIN CAUCES ON RAPLAV.CODCAUCES = CAUCES.CODCAUCES WHERE RAPLAV.CodPos = '" + CodPos + "' AND DATCES IS NOT NULL" +
                " ORDER BY DATCES DESC";
            DataTable dataTable = new DataTable();
            dataTable = dataLayer.GetDataTable(strSQL);
            string Err = "";
            DataSet dataSet1 = new DataSet();
            DataSet dataSet2 = dataLayer.GetDataSet(strSQL, ref Err);
            CessazioniRdl cessazioniRdl = new CessazioniRdl();
            foreach (DataRow row in (InternalDataCollectionBase)dataSet2.Tables[0].Rows)
            {
                CessazioniRdl.Listas listas = new CessazioniRdl.Listas()
                {
                    Matricola2 = row["MAT"].ToString(),
                    Nome = row["NOM"].ToString(),
                    Cognome = row["COG"].ToString(),
                    DataCessazione = Convert.ToDateTime(row["DATCES"], CultureInfo.GetCultureInfo("it-IT")),
                    DataIscrizione = Convert.ToDateTime(row["DATDEC"], CultureInfo.GetCultureInfo("it-IT")),
                    Causale1 = row["DENCES"].ToString(),
                    NomeFile = this.FileExists(string.Format("{0}{1}CESS_{2}_{3}_{4}.pdf", (object)AppDomain.CurrentDomain.BaseDirectory, (object)ConfigurationManager.AppSettings["path_ricevuteCessazRDL"], (object)CodPos, row["MAT"], row["PRORAP"]))
                };
                cessazioniRdl.listas.Add(listas);
            }

            cessazioniRdl.listas = cessazioniRdl.listas.OrderByDescending(rdl => rdl.DataCessazione).ToList();
            return cessazioniRdl.listas;
        }

        private string FileExists(string path) => !File.Exists(path) ? "#" : path.Replace(AppDomain.CurrentDomain.BaseDirectory, "../../");

        public bool Controllo(
          string Matricola,
          string Nome,
          string Cognome,
          string DataIscrizione,
          string DataCessazione,
          string Causale,
          string ProRap,
          string CodPos,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMSG,
          ref string SuccessMSG)
        {
            DataLayer dataLayer = new();
            DateTime dataIscrizioneDate = DateTime.Parse(DataIscrizione);
            DateTime dataCessazioneDate = DateTime.Parse(DataCessazione);
            int giornoCessazione = dataCessazioneDate.Day;
            int meseCessazione = dataCessazioneDate.Month;
            int annoCessazione = dataCessazioneDate.Year;

            if (string.IsNullOrEmpty(Matricola) || string.IsNullOrEmpty(DataCessazione) || string.IsNullOrEmpty(DataIscrizione) || string.IsNullOrEmpty(Causale))
            {
                ErroreMSG = "Richiesta di cessazione non completa";
                return false;
            }
            if (dataCessazioneDate > DateTime.Now)
            {
                ErroreMSG = "La data di cessazione non puo' essere maggiore della data odierna";
                return false;
            }
            if (dataCessazioneDate < dataIscrizioneDate)
            {
                ErroreMSG = "La data di cessazione non puo' essere minore della data di decorrenza";
                return false;
            }

            string trattamentiEconomiciSuccessiviCheckSql =
                $"SELECT VALUE(COUNT(*), 0) FROM STORDL WHERE CODPOS = {CodPos} AND MAT = {Matricola} AND PRORAP = {ProRap} AND DATINI > '{DataCessazione}'";
            string trattamentiEconomiciCheckResult = dataLayer.Get1ValueFromSQL(trattamentiEconomiciSuccessiviCheckSql, CommandType.Text);
            if (int.Parse(trattamentiEconomiciCheckResult) != 0)
            {
                ErroreMSG = "Esistono trattamenti economici con data successiva alla data di cessazione. Impossibile continuare";
                return false;
            }

            string dipaNotificheUfficioCheckSql =
                $"SELECT VALUE(COUNT(*), 0) FROM DENDET WHERE CODPOS = {CodPos} AND MAT = {Matricola} AND PRORAP = {ProRap} AND ANNDEN = {annoCessazione} " +
                $"AND MESDEN > {meseCessazione} AND TIPMOV IN ('DP', 'NU') AND NUMMOVANN IS NULL AND NUMMOV IS NOT NULL";
            string dipaNotificheUfficioCheckResult = dataLayer.Get1ValueFromSQL(dipaNotificheUfficioCheckSql, CommandType.Text);
            if (int.Parse(dipaNotificheUfficioCheckResult) != 0)
            {
                ErroreMSG = "Esistono DIPA/NOTIFICHE DI UFFICIO con data successiva alla data di cessazione. Impossibile continuare";
                return false;
            }

            if (meseCessazione == 1)
            {
                string firstRaplavCheckSqlCmd;
                if (giornoCessazione < 25)
                {
                    firstRaplavCheckSqlCmd =
                        $"SELECT VALUE(COUNT(*), 0) FROM RAPLAV WHERE DATDEC <= '{annoCessazione}-01-01' AND VALUE(DATCES, '9999-12-31') " +
                        $">= '{annoCessazione}-01-{DateTime.DaysInMonth(annoCessazione, meseCessazione)}' AND CODPOS = {CodPos}";
                }
                else
                {
                    firstRaplavCheckSqlCmd =
                        $"SELECT VALUE(COUNT(*), 0) FROM RAPLAV WHERE DATDEC <= '{annoCessazione - 1}-12-31' AND CODPOS = {CodPos}";
                }

                string firstRaplavCheckResult = dataLayer.Get1ValueFromSQL(firstRaplavCheckSqlCmd, CommandType.Text);

                if (int.Parse(firstRaplavCheckResult) > 0)
                {
                    string dentesCheckSqlCmd;
                    if (giornoCessazione < 25)
                    {
                        dentesCheckSqlCmd =
                            $"SELECT VALUE(COUNT(*), 0) FROM DENTES WHERE CODPOS = {CodPos} AND TIPMOV IN ('DP', 'NU') AND NUMMOV IS NOT NULL " +
                            $"AND NUMMOVANN IS NULL AND ANNDEN = {annoCessazione - 1} AND MESDEN = 11";
                    }
                    else
                    {
                        dentesCheckSqlCmd =
                            $"SELECT VALUE(COUNT(*), 0) FROM DENTES WHERE CODPOS = {CodPos} AND TIPMOV IN ('DP', 'NU') AND NUMMOV IS NOT NULL " +
                            $"AND NUMMOVANN IS NULL AND ANNDEN = {annoCessazione - 1} AND MESDEN = 12";
                    }
                    string dentesCheckResult = dataLayer.Get1ValueFromSQL(dentesCheckSqlCmd, CommandType.Text);
                    int dentesCheckCount = int.Parse(dentesCheckResult);

                    string secondRaplavCheckSqlCmd;
                    if (giornoCessazione < 25)
                    {
                        secondRaplavCheckSqlCmd =
                            $"SELECT VALUE (COUNT(*), 0) FROM RAPLAV WHERE CODPOS = {CodPos} AND '{annoCessazione - 1}-11-01' " +
                            $"BETWEEN DATDEC AND VALUE(DATCES, '9999-12-31')";
                    }
                    else
                    {
                        secondRaplavCheckSqlCmd =
                            $"SELECT VALUE (COUNT(*), 0) FROM RAPLAV WHERE CODPOS = {CodPos} AND '{annoCessazione - 1}-12-01' " +
                            $"BETWEEN DATDEC AND VALUE(DATCES, '9999-12-31')";
                    }
                    string secondoRaplavCheckResult = dataLayer.Get1ValueFromSQL(secondRaplavCheckSqlCmd, CommandType.Text);
                    int secondoRaplavCheckCount = int.Parse(secondoRaplavCheckResult);

                    if (dentesCheckCount == 0 && secondoRaplavCheckCount > 1)
                    {
                        if (giornoCessazione < 25)
                            ErroreMSG = $"Non è presente la denuncia mensile del mese di Dicembre di {annoCessazione - 1}";
                        else
                            ErroreMSG = $"Non è presente la denuncia mensile del mese di Novembre di {annoCessazione - 1}";
                        return false;
                    }
                }
            }
            else
            {
                string notJanuaryRaplavSqlCmd;
                if (giornoCessazione > 25)
                {
                    notJanuaryRaplavSqlCmd =
                        $"SELECT VALUE(COUNT(*),0) FROM RAPLAV WHERE DATDEC <= '{annoCessazione}-{meseCessazione - 1}-01' AND VALUE(DATCES, '9999-12-31')" +
                        $" >= '{annoCessazione}-{meseCessazione - 1}-{DateTime.DaysInMonth(annoCessazione, meseCessazione - 1)}' AND CODPOS = {CodPos}";
                }
                else
                {
                    if (meseCessazione == 2)
                    {
                        notJanuaryRaplavSqlCmd =
                            $"SELECT VALUE(COUNT(*), 0) FROM RAPLAV WHERE DATDEC <= '{annoCessazione - 1}-12-01' AND VALUE(DATCES, '9999-12-31') >= " +
                            $"'{annoCessazione - 1}-12-31' AND CODPOS = {CodPos}";
                    }
                    else
                    {
                        notJanuaryRaplavSqlCmd =
                            $"SELECT VALUE(COUNT(*), 0) FROM RAPLAV WHERE DATDEC <= '{annoCessazione}-{meseCessazione - 1}-01' AND VALUE(DATCES, '9999-12-31')" +
                            $" >= '{annoCessazione}-{meseCessazione - 2}-{DateTime.DaysInMonth(annoCessazione, meseCessazione - 2)}' AND CODPOS = {CodPos}";
                    }
                }

                string notJanuaryRaplavCheckResult = dataLayer.Get1ValueFromSQL(notJanuaryRaplavSqlCmd, CommandType.Text);

                if (int.Parse(notJanuaryRaplavCheckResult) > 0)
                {
                    string notJanuaryDentesSqlCmd;
                    if (giornoCessazione > 25)
                    {
                        notJanuaryDentesSqlCmd =
                            $"SELECT VALUE(COUNT(*),0) FROM DENTES WHERE CODPOS = {CodPos} AND TIPMOV IN ('DP', 'NU') AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL " +
                            $"AND ANNDEN = {annoCessazione} AND MESDEN = {meseCessazione - 1}";
                    }
                    else
                    {
                        if (meseCessazione == 2)
                        {
                            notJanuaryDentesSqlCmd =
                                $"SELECT VALUE(COUNT(*),0) FROM DENTES WHERE CODPOS = {CodPos} AND TIPMOV IN ('DP', 'NU') AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL " +
                                $"AND ANNDEN = {annoCessazione - 1} AND MESDEN = 12";
                        }
                        else
                        {
                            notJanuaryDentesSqlCmd =
                                $"SELECT VALUE(COUNT(*),0) FROM DENTES WHERE CODPOS = {CodPos} AND TIPMOV IN ('DP', 'NU') AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL " +
                                $"AND ANNDEN = {annoCessazione} AND MESDEN = {meseCessazione - 2}";
                        }
                    }
                    string notJanuaryDentesCheckResult = dataLayer.Get1ValueFromSQL(notJanuaryDentesSqlCmd, CommandType.Text);
                    int notJanuaryDentesCheckCount = int.Parse(notJanuaryDentesCheckResult);

                    string notJanuaryRaplavSecondCheck;
                    if (giornoCessazione > 25)
                    {
                        notJanuaryRaplavSecondCheck =
                            $"SELECT VALUE(COUNT(*),0) FROM RAPLAV WHERE CODPOS = {CodPos} AND '{annoCessazione}-{meseCessazione - 1}-01' " +
                            $"BETWEEN DATDEC AND VALUE(DATCES, '9999-12-31')";
                    }
                    else
                    {
                        if (meseCessazione == 2)
                        {
                            notJanuaryRaplavSecondCheck =
                                $"SELECT VALUE(COUNT(*),0) FROM RAPLAV WHERE CODPOS = {CodPos} AND '{annoCessazione - 1}-12-01' " +
                                $"BETWEEN DATDEC AND VALUE(DATCES, '9999-12-31')";
                        }
                        else
                        {
                            notJanuaryRaplavSecondCheck =
                                $"SELECT VALUE(COUNT(*),0) FROM RAPLAV WHERE CODPOS = {CodPos} AND '{annoCessazione}-{meseCessazione - 2}-01' " +
                                $"BETWEEN DATDEC AND VALUE(DATCES, '9999-12-31')";
                        }
                    }
                    string notJanuaryRaplavSecondCheckresult = dataLayer.Get1ValueFromSQL(notJanuaryRaplavSecondCheck, CommandType.Text);
                    int notJanuaryRaplavSecondCheckCount = int.Parse(notJanuaryRaplavSecondCheckresult);

                    if (notJanuaryDentesCheckCount == 0 && notJanuaryRaplavSecondCheckCount > 1)
                    {
                        if (giornoCessazione > 25)
                        {
                            ErroreMSG = $"Non è presente la denuncia mensile del mese di {DBMethods.GetMesi()[meseCessazione - 1]} dell'anno {annoCessazione}";                            
                        }
                        else
                        {
                            if (meseCessazione == 2)
                                ErroreMSG = $"Non è presente la denuncia mensile del mese di dicembre dell'anno {annoCessazione - 1}";
                            else
                                ErroreMSG = $"Non è presente la denuncia mensile del mese di {DBMethods.GetMesi()[meseCessazione - 2]} dell'anno {annoCessazione}";
                        }
                        return false;
                    }
                }
            }
            return true;

        }

        #region Salvataggio vecchio
        //public bool Btn_Salva(
        //  string Matricola,
        //  string Nome,
        //  string Cognome,
        //  string DataIscrizione,
        //  string DataCessazione,
        //  string Causale,
        //  string ProRap,
        //  string CodPos,
        //  TFI.OCM.Utente.Utente u,
        //  int giorniPreav,
        //  double importoPreav,
        //  ref string ErroreMSG,
        //  ref string SuccessMSG)
        //{
        //    iDB2DataAdapter iDb2DataAdapter = new iDB2DataAdapter();
        //    clsIDOC clsIdoc1 = new clsIDOC();
        //    clsPrev clsPrev = new clsPrev();
        //    DataLayer cmd = new DataLayer();
        //    DataTable DT = new DataTable();
        //    DataTable dataTable1 = new DataTable();
        //    ClsProtocolloDll clsProtocolloDll = new ClsProtocolloDll();
        //    bool blnCommit = false;
        //    int num1 = 0;
        //    try
        //    {
        //        cmd.StartTransaction();
        //        string Err = " ";
        //        string strSQL1 = "SELECT PROMOD FROM MODPRE WHERE CODPOS='" + CodPos + "'AND MAT= '" + Matricola + "' AND PRORAP='" + ProRap + "'";
        //        DataSet dataSet1 = new DataSet();
        //        DataSet dataSet2 = cmd.GetDataSet(strSQL1, ref Err);
        //        string PROMOD = dataSet2.Tables[0].Rows.Count != 0 ? dataSet2.Tables[0].Rows[0]["PROMOD"].ToString() : "1";
        //        string strSQL2 = "SELECT VALUE(COUNT(*), 0) FROM MODPRE" + " WHERE CODPOS = '" + CodPos + "' " + " AND MAT ='" + Matricola + "' " + " AND PRORAP ='" + ProRap + "' " + " AND DATANN IS NULL";
        //        DataLayer dataLayer1;
        //        if (Convert.ToInt32("0" + cmd.Get1ValueFromSQL(strSQL2, CommandType.Text)) > 0)
        //        {
        //            dataLayer1 = (DataLayer)null;
        //        }
        //        else
        //        {
        //            DateTime dateTime1 = Convert.ToDateTime(DataCessazione);
        //            int year1 = dateTime1.Year;
        //            dateTime1 = Convert.ToDateTime(DataCessazione);
        //            int num2 = dateTime1.Year - 1;
        //            string strSQL3 = "SELECT DATDEC FROM RAPLAV WHERE CODPOS='" + CodPos + "' " + " AND MAT ='" + Matricola + "' " + " AND PRORAP= " + ProRap;
        //            string str1 = cmd.Get1ValueFromSQL(strSQL3, CommandType.Text);
        //            string[] strArray = new string[5]
        //            {
        //    "RAPLAV",
        //    "STORDL",
        //    "IMPAGG",
        //    "PARTIMM",
        //    "SOSRAP"
        //            };
        //            dateTime1 = Convert.ToDateTime(str1);
        //            if (dateTime1.Year == year1)
        //                num2 = year1;
        //            for (int index1 = 0; index1 <= strArray.Length - 1; ++index1)
        //            {
        //                string strSQL4 = "SELECT * FROM " + strArray[index1] + " " + " WHERE CODPOS ='" + CodPos + "' " + " AND MAT ='" + Matricola + "' " + " AND PRORAP ='" + ProRap + "' ";
        //                DataTable dataTable2 = cmd.GetDataTable(strSQL4);

        //                string str2 = "INSERT INTO BK_" + strArray[index1] + "_CESS" + " ( ";
        //                for (int index2 = 0; index2 <= dataTable2.Columns.Count - 1; ++index2)
        //                {
        //                    if (dataTable2.Columns[index2].ColumnName.ToUpper().Contains("EMAIL")) continue;
        //                    str2 = index2 != 0 ? str2 + ", " + dataTable2.Columns[index2].ColumnName : str2 + dataTable2.Columns[index2].ColumnName;
        //                }
        //                string str3 = str2 + " ) " + " SELECT ";
        //                for (int index3 = 0; index3 <= dataTable2.Columns.Count - 1; ++index3)
        //                {
        //                    if (dataTable2.Columns[index3].ColumnName.ToUpper().Contains("EMAIL")) continue;
        //                    str3 = index3 != 0 ? str3 + ", " + dataTable2.Columns[index3].ColumnName : str3 + dataTable2.Columns[index3].ColumnName;
        //                }
        //                string strSQL5 = str3 + " FROM " + strArray[index1] + " " + " WHERE CODPOS ='" + CodPos + "' " + " AND MAT ='" + Matricola + "' " + " AND PRORAP ='" + ProRap + "' ";
        //                blnCommit = cmd.WriteTransactionData(strSQL5, CommandType.Text);
        //                if (!blnCommit)
        //                    break;
        //            }
        //            if (blnCommit && !(Causale.ToString().Trim() == "10"))
        //            {
        //                DataTable idocDatiE1Pitype = clsIdoc1.GET_IDOC_DATI_E1PITYPE(cmd, "0016", Convert.ToInt32(CodPos), Convert.ToInt32(Matricola), Convert.ToInt32(ProRap), 0, "", "", Convert.ToString(9956), "", "", "", 0, 0, 0, "", "MODIFICA RDL", "", "");
        //                clsIdoc1.WRITE_IDOC_TESTATA(cmd, idocDatiE1Pitype.Rows[0]);
        //                idocDatiE1Pitype.Clear();
        //                DT = clsIdoc1.GET_IDOC_DATI_E1PITYPE(cmd, "0016", Convert.ToInt32(CodPos), Convert.ToInt32(Matricola), Convert.ToInt32(ProRap), 0, "", "", "9999 - 12 - 31", "", "", "", 0, 0, 0, "D", "MODIFICA RDL", "", "");
        //                clsIdoc1.WRITE_IDOC_E1PITYP(cmd, "9001", DT, true);
        //                clsIdoc1.WRITE_IDOC_E1PITYP(cmd, "0016", DT, true);
        //            }
        //            if (Causale == "DECESSO")
        //                num1 = 3;
        //            else if (Causale == "DIMISSIONI")
        //                num1 = 2;
        //            else if (Causale == "LICENZIAMENTO")
        //                num1 = 1;
        //            else if (Causale == "MANCATO SUPERAMENTO PERIODO DI PROVA")
        //                num1 = 100;
        //            else if (Causale == "PENSIONAMENTO")
        //                num1 = 101;
        //            else if (Causale == "RISOLUZIONE CONSENSUALE")
        //                num1 = 104;
        //            else if (Causale == "SCADENZA DEL TERMINE")
        //                num1 = 27;
        //            if (blnCommit)
        //            {
        //                string strSQL6 = "UPDATE RAPLAV SET " + " FLGAPP = 'M', CODCAUCES = " + num1.ToString() + " ," + " DATCES = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DataCessazione)) + ", " + " UTEAGG = " + DBMethods.DoublePeakForSql(u.Username) + ", " + " ULTAGG = CURRENT TIMESTAMP " + " WHERE CODPOS ='" + CodPos + "' " + " AND MAT ='" + Matricola + "' " + " AND PRORAP ='" + ProRap + "' ";
        //                blnCommit = cmd.WriteTransactionData(strSQL6, CommandType.Text);
        //            }
        //            if (blnCommit)
        //            {
        //                string rdlDaCessareQuery = "SELECT CODPOS, MAT, PRORAP, DATDEC, DATCES, ULTAGG, UTEAGG, CODPOSDA, CODPOSA, DTTFR, ALIQTFR, DTINF, " +
        //                "ALIQINF, DTFP, ALIQFP, FAP, ALIFAP, ALIQUOTA, CODQUACON, CODCON, CODLOC, DENLIV, TIPRAP, ABBPRE, ASSCON, CODGRUASS, CODLIV FROM RAPLAV WHERE CODPOS = " + CodPos + " AND MAT = " + Matricola + " AND PRORAP = " + ProRap;
        //                DataRow rdlDaCessareRecord = cmd.GetDataTable(rdlDaCessareQuery).Rows[0];

        //                string ultimoStordlQuery = "SELECT DATINI, DATFIN, TRAECO, PERFAP, NUMMEN, PERAPP, MESMEN14, MESMEN15, MESMEN16, INDANN, GRUREC" +
        //                                                    ", PERPAR, DATSCATER, NUMSCAMAT, IMPSCAMAT, DATULTSCA, DATNEWSCA, " +
        //                                                    "DATULTRIC, TOTIMPSCAT, IMPSCATRIV, IMPSCATMAT FROM STORDL WHERE CODPOS = " + CodPos + " AND MAT = " + Matricola + " AND PRORAP = " + ProRap + " ORDER BY DATINI DESC LIMIT 1";

        //                DataTable stordlTable = cmd.GetDataTable(ultimoStordlQuery);
        //                if (stordlTable.Rows.Count > 0)
        //                {
        //                    string queryInsertIntoStordl = "INSERT INTO STORDL (";
        //                    string queryCampiRaplav = "CODPOS, MAT, PRORAP, DATINI, DATFIN, ULTAGG, UTEAGG, CODPOSDA, CODPOSA, DTTFR, ALIQTFR, DTINF, ALIQINF, DTFP, ALIQFP, FAP, ALIFAP, ALIQUOTA, CODQUACON, CODCON, CODLOC, DENLIV, TIPRAP, ABBPRE, ASSCON, CODGRUASS, CODLIV, ";
        //                    string queryCampiStordl = "TRAECO, PERFAP, NUMMEN, PERAPP, MESMEN14, MESMEN15, MESMEN16, INDANN, GRUREC, PERPAR, DATSCATER, NUMSCAMAT, IMPSCAMAT, DATULTSCA, DATNEWSCA, DATULTRIC, TOTIMPSCAT, IMPSCATRIV, IMPSCATMAT";
        //                    queryInsertIntoStordl += queryCampiRaplav + queryCampiStordl + ") VALUES (";

        //                    string queryValueRaplav = $"{rdlDaCessareRecord["CODPOS"].ToString()}," +
        //                                                $" {rdlDaCessareRecord["MAT"].ToString()}, " +
        //                                                $"{rdlDaCessareRecord["PRORAP"].ToString()}, " +
        //                                                $"'{Utile.FromStringToDateTimeToFormatString(DataCessazione)}'," +
        //                                                $" '{Utile.FromStringToDateTimeToFormatString(DataCessazione)}', " +
        //                                                $"CURRENT DATE, " +
        //                                                $"'{rdlDaCessareRecord["CODPOS"].ToString()}'," +
        //                                                $" {(string.IsNullOrWhiteSpace(rdlDaCessareRecord["CODPOSDA"].ToString()) ? "0" : rdlDaCessareRecord["CODPOSDA"].ToString())}," +
        //                                                $" {(string.IsNullOrWhiteSpace(rdlDaCessareRecord["CODPOSA"].ToString()) ? "0" : rdlDaCessareRecord["CODPOSA"].ToString())}," +
        //                                                $"{(string.IsNullOrWhiteSpace(rdlDaCessareRecord["DTTFR"].ToString()) ? "null" : "'" + Utile.FromStringToDateTimeToFormatString(rdlDaCessareRecord["DTTFR"].ToString()) + "'")}, " +
        //                                                $"{(string.IsNullOrWhiteSpace(rdlDaCessareRecord["ALIQTFR"].ToString()) ? "0" : rdlDaCessareRecord["ALIQTFR"].ToString())}, " +
        //                                                $"{(string.IsNullOrWhiteSpace(rdlDaCessareRecord["DTINF"].ToString()) ? "null" : "'" + Utile.FromStringToDateTimeToFormatString(rdlDaCessareRecord["DTINF"].ToString()) + "'")}, " +
        //                                                $"{(string.IsNullOrWhiteSpace(rdlDaCessareRecord["ALIQINF"].ToString()) ? "0" : rdlDaCessareRecord["ALIQINF"].ToString())}, " +
        //                                                $"{(string.IsNullOrWhiteSpace(rdlDaCessareRecord["DTFP"].ToString()) ? "null" : "'" + Utile.FromStringToDateTimeToFormatString(rdlDaCessareRecord["DTFP"].ToString()) + "'")}, " +
        //                                                $"{(string.IsNullOrWhiteSpace(rdlDaCessareRecord["ALIQFP"].ToString()) ? "0" : rdlDaCessareRecord["ALIQFP"].ToString())}, " +
        //                                                $"'{(string.IsNullOrWhiteSpace(rdlDaCessareRecord["FAP"].ToString()) ? "0" : rdlDaCessareRecord["FAP"].ToString())}', " +
        //                                                $"{(string.IsNullOrWhiteSpace(rdlDaCessareRecord["ALIFAP"].ToString()) ? "0" : rdlDaCessareRecord["ALIFAP"].ToString())}, " +
        //                                                $"{(string.IsNullOrWhiteSpace(rdlDaCessareRecord["ALIQUOTA"].ToString()) ? "0" : rdlDaCessareRecord["ALIQUOTA"].ToString())}, " +
        //                                                $"{(string.IsNullOrWhiteSpace(rdlDaCessareRecord["CODQUACON"].ToString()) ? "0" : rdlDaCessareRecord["CODQUACON"].ToString())}, " +
        //                                                $"{(string.IsNullOrWhiteSpace(rdlDaCessareRecord["CODCON"].ToString()) ? "0" : rdlDaCessareRecord["CODCON"].ToString())}, " +
        //                                                $"{(string.IsNullOrWhiteSpace(rdlDaCessareRecord["CODLOC"].ToString()) ? "0" : rdlDaCessareRecord["CODLOC"].ToString())}, " +
        //                                                $"'{(string.IsNullOrWhiteSpace(rdlDaCessareRecord["DENLIV"].ToString()) ? "0" : rdlDaCessareRecord["DENLIV"].ToString())}', " +
        //                                                $"{(string.IsNullOrWhiteSpace(rdlDaCessareRecord["TIPRAP"].ToString()) ? "0" : rdlDaCessareRecord["TIPRAP"].ToString())}, " +
        //                                                $"'{(string.IsNullOrWhiteSpace(rdlDaCessareRecord["ABBPRE"].ToString()) ? "0" : rdlDaCessareRecord["ABBPRE"].ToString())}', " +
        //                                                $"'{(string.IsNullOrWhiteSpace(rdlDaCessareRecord["ASSCON"].ToString()) ? "0" : rdlDaCessareRecord["ASSCON"].ToString())}', " +
        //                                                $"{(string.IsNullOrWhiteSpace(rdlDaCessareRecord["CODGRUASS"].ToString()) ? "0" : rdlDaCessareRecord["CODGRUASS"].ToString())}, " +
        //                                                $"{(string.IsNullOrWhiteSpace(rdlDaCessareRecord["CODLIV"].ToString()) ? "0" : rdlDaCessareRecord["CODLIV"].ToString())}";


        //                    string queryValueStordl = $" {(string.IsNullOrWhiteSpace(stordlTable.Rows[0]["TRAECO"].ToString()) ? "0" : stordlTable.Rows[0]["TRAECO"].ToString())}," +
        //                                                $" {(string.IsNullOrWhiteSpace(stordlTable.Rows[0]["PERFAP"].ToString()) ? "0" : stordlTable.Rows[0]["PERFAP"].ToString())}," +
        //                                                $" {(string.IsNullOrWhiteSpace(stordlTable.Rows[0]["NUMMEN"].ToString()) ? "0" : stordlTable.Rows[0]["NUMMEN"].ToString())}," +
        //                                                $" {(string.IsNullOrWhiteSpace(stordlTable.Rows[0]["PERAPP"].ToString()) ? "0" : stordlTable.Rows[0]["PERAPP"].ToString())}," +
        //                                                $" {(string.IsNullOrWhiteSpace(stordlTable.Rows[0]["MESMEN14"].ToString()) ? "0" : stordlTable.Rows[0]["MESMEN14"].ToString())}," +
        //                                                $" {(string.IsNullOrWhiteSpace(stordlTable.Rows[0]["MESMEN15"].ToString()) ? "0" : stordlTable.Rows[0]["MESMEN15"].ToString())}," +
        //                                                $" {(string.IsNullOrWhiteSpace(stordlTable.Rows[0]["MESMEN16"].ToString()) ? "0" : stordlTable.Rows[0]["MESMEN16"].ToString())}," +
        //                                                $"'{(string.IsNullOrWhiteSpace(stordlTable.Rows[0]["INDANN"].ToString()) ? "0" : stordlTable.Rows[0]["INDANN"].ToString())}', " +
        //                                                $" {(string.IsNullOrWhiteSpace(stordlTable.Rows[0]["GRUREC"].ToString()) ? "0" : stordlTable.Rows[0]["GRUREC"].ToString())}," +
        //                                                $" {(string.IsNullOrWhiteSpace(stordlTable.Rows[0]["PERPAR"].ToString()) ? "0" : stordlTable.Rows[0]["PERPAR"].ToString())}," +
        //                                                $"{(string.IsNullOrWhiteSpace(stordlTable.Rows[0]["DATSCATER"].ToString()) ? "null" : "'" + Utile.FromStringToDateTimeToFormatString(stordlTable.Rows[0]["DATSCATER"].ToString()) + "'")}, " +
        //                                                $" {(string.IsNullOrWhiteSpace(stordlTable.Rows[0]["NUMSCAMAT"].ToString()) ? "0" : stordlTable.Rows[0]["NUMSCAMAT"].ToString())}," +
        //                                                $" {(string.IsNullOrWhiteSpace(stordlTable.Rows[0]["IMPSCAMAT"].ToString()) ? "0" : stordlTable.Rows[0]["IMPSCAMAT"].ToString())}," +
        //                                                $"{(string.IsNullOrWhiteSpace(stordlTable.Rows[0]["DATULTSCA"].ToString()) ? "null" : "'" + Utile.FromStringToDateTimeToFormatString(stordlTable.Rows[0]["DATULTSCA"].ToString()) + "'")}, " +
        //                                                $"{(string.IsNullOrWhiteSpace(stordlTable.Rows[0]["DATNEWSCA"].ToString()) ? "null" : "'" + Utile.FromStringToDateTimeToFormatString(stordlTable.Rows[0]["DATNEWSCA"].ToString()) + "'")}, " +
        //                                                $"{(string.IsNullOrWhiteSpace(stordlTable.Rows[0]["DATULTRIC"].ToString()) ? "null" : "'" + Utile.FromStringToDateTimeToFormatString(stordlTable.Rows[0]["DATULTRIC"].ToString()) + "'")}, " +
        //                                                $" {(string.IsNullOrWhiteSpace(stordlTable.Rows[0]["TOTIMPSCAT"].ToString()) ? "0" : stordlTable.Rows[0]["TOTIMPSCAT"].ToString())}," +
        //                                                $" {(string.IsNullOrWhiteSpace(stordlTable.Rows[0]["IMPSCATRIV"].ToString()) ? "0" : stordlTable.Rows[0]["IMPSCATRIV"].ToString())}," +
        //                                                $" {(string.IsNullOrWhiteSpace(stordlTable.Rows[0]["IMPSCATMAT"].ToString()) ? "0" : stordlTable.Rows[0]["IMPSCATMAT"].ToString())}";

        //                    queryInsertIntoStordl += queryValueRaplav + ", " + queryValueStordl + ")";
        //                    blnCommit = cmd.WriteTransactionData(queryInsertIntoStordl, CommandType.Text);
        //                }
        //                string strSQL7 = "UPDATE STORDL SET " + " DATFIN = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DataCessazione)) + ", " + " UTEAGG = " + DBMethods.DoublePeakForSql(u.Username) + ", " + " ULTAGG = CURRENT_TIMESTAMP " + " WHERE CODPOS ='" + CodPos + "' " + " AND MAT ='" + Matricola + "' " + " AND PRORAP ='" + ProRap + "' " + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DataCessazione)) + "  BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31')";
        //                blnCommit = cmd.WriteTransactionData(strSQL7, CommandType.Text);
        //            }
        //            if (blnCommit)
        //            {
        //                string strSQL8 = " DELETE FROM IMPAGG" + " WHERE CODPOS = '" + CodPos + "' " + " AND MAT ='" + Matricola + "' " + " AND PRORAP ='" + ProRap + "' " + " AND DATINI > " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DataCessazione));
        //                blnCommit = cmd.WriteTransactionData(strSQL8, CommandType.Text);
        //            }
        //            if (blnCommit)
        //            {
        //                string strSQL9 = "DELETE FROM PARTIMM" + " WHERE CODPOS = '" + CodPos + "' " + " AND MAT ='" + Matricola + "' " + " AND PRORAP ='" + ProRap + "' " + " AND DATINI > " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DataCessazione));
        //                blnCommit = cmd.WriteTransactionData(strSQL9, CommandType.Text);
        //            }
        //            if (blnCommit)
        //            {
        //                string strSQL10 = "DELETE FROM STORDL" + " WHERE CODPOS = " + CodPos + " AND MAT ='" + Matricola + "' " + " AND PRORAP ='" + ProRap + "' " + " AND DATINI > " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DataCessazione));
        //                blnCommit = cmd.WriteTransactionData(strSQL10, CommandType.Text);
        //            }
        //            if (blnCommit)
        //            {
        //                string str4 = " SELECT ANNDEN, MESDEN, CODPOS, MAT, PRODEN, PRODENDET, AL, TIPMOV FROM DENDET WHERE " + " CODPOS ='" + CodPos + "' ";
        //                dateTime1 = Convert.ToDateTime(DataCessazione);
        //                string str5 = dateTime1.Year.ToString();
        //                string str6 = str4 + " AND ANNDEN =" + str5;
        //                dateTime1 = Convert.ToDateTime(DataCessazione);
        //                string str7 = dateTime1.Month.ToString();
        //                string strSQL11 = str6 + " AND MESDEN =" + str7 + " AND MAT ='" + Matricola + "' " + " AND NUMMOVANN IS NULL ORDER BY TIPMOV ASC";
        //                DataTable dataTable3 = cmd.GetDataTable(strSQL11);
        //                for (int index = 0; index <= dataTable3.Rows.Count - 1; ++index)
        //                {
        //                    if (Convert.ToDateTime(DataCessazione) != Convert.ToDateTime(dataTable3.Rows[index]["AL"]))
        //                    {
        //                        string str8 = dataTable3.Rows[index]["TIPMOV"].ToString().Trim();
        //                        if (!(str8 == "AR"))
        //                        {
        //                            if (str8 == "DP" || str8 == "NU")
        //                            {
        //                                DT = clsIdoc1.GET_IDOC_DATI_E1PITYPE(cmd, "9004", Convert.ToInt32(CodPos), Convert.ToInt32(Matricola), 0, 0, "", "", "9999-12-31", "", "", "", Convert.ToInt32(dataTable3.Rows[index]["ANNDEN"]), Convert.ToInt32(dataTable3.Rows[index]["MESDEN"]), Convert.ToInt32(dataTable3.Rows[index]["PRODEN"]), "D", "", dataTable3.Rows[index]["TIPMOV"].ToString().Trim(), "");
        //                                if (Causale.ToString().Trim() == "10")
        //                                    clsIdoc1.WRITE_IDOC_TESTATA(cmd, DT.Rows[0]);
        //                                clsIdoc1.WRITE_IDOC_E1PITYP(cmd, "9004", DT, false);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            DT = clsIdoc1.GET_IDOC_DATI_E1PITYPE(cmd, "9005", Convert.ToInt32(CodPos), Convert.ToInt32(Matricola), 0, 0, "", "", "9999-12-31", "", "", "", Convert.ToInt32(dataTable3.Rows[index]["ANNDEN"]), Convert.ToInt32(dataTable3.Rows[index]["MESDEN"]), Convert.ToInt32(dataTable3.Rows[index]["PRODEN"]), "D", "", dataTable3.Rows[index]["TIPMOV"].ToString().Trim(), "");
        //                            if (Causale.ToString().Trim() == "10")
        //                                clsIdoc1.WRITE_IDOC_TESTATA(cmd, DT.Rows[0]);
        //                            clsIdoc1.WRITE_IDOC_E1PITYP(cmd, "9005", DT, false);
        //                        }
        //                        string strSQL12 = "UPDATE DENDET SET " + " AL = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DataCessazione)) + ", " + " UTEAGG = " + DBMethods.DoublePeakForSql(u.Username) + ", " + " ULTAGG = CURRENT_TIMESTAMP " + " WHERE CODPOS ='" + CodPos + "' " + " AND ANNDEN = '" + dataTable3.Rows[index]["ANNDEN"].ToString() + "' " + " AND MESDEN = '" + dataTable3.Rows[index]["MESDEN"].ToString() + "' " + " AND PRODEN ='" + dataTable3.Rows[index]["PRODEN"].ToString() + "' " + " AND PRODENDET ='" + dataTable3.Rows[index]["PRODENDET"].ToString() + "' " + " AND MAT ='" + Matricola + "' ";
        //                        blnCommit = cmd.WriteTransactionData(strSQL12, CommandType.Text);
        //                        if (blnCommit)
        //                        {
        //                            string str9 = dataTable3.Rows[index]["TIPMOV"].ToString().Trim();
        //                            if (!(str9 == "AR"))
        //                            {
        //                                if (str9 == "DP" || str9 == "NU")
        //                                {
        //                                    DT.Clear();
        //                                    DT = clsIdoc1.GET_IDOC_DATI_E1PITYPE(cmd, "9004", Convert.ToInt32(CodPos), Convert.ToInt32(Matricola), 0, 0, "", "", "9999-12-31", "", "", "", Convert.ToInt32(dataTable3.Rows[index]["ANNDEN"]), Convert.ToInt32(dataTable3.Rows[index]["MESDEN"]), Convert.ToInt32(dataTable3.Rows[index]["PRODEN"]), "", "", dataTable3.Rows[index]["TIPMOV"].ToString().Trim(), "");
        //                                    clsIdoc1.WRITE_IDOC_E1PITYP(cmd, "9004", DT, false);
        //                                }
        //                            }
        //                            else
        //                            {
        //                                DT.Clear();
        //                                DT = clsIdoc1.GET_IDOC_DATI_E1PITYPE(cmd, "9005", Convert.ToInt32(CodPos), Convert.ToInt32(Matricola), 0, 0, "", "", "9999-12-31", "", "", "", Convert.ToInt32(dataTable3.Rows[index]["ANNDEN"]), Convert.ToInt32(dataTable3.Rows[index]["MESDEN"]), Convert.ToInt32(dataTable3.Rows[index]["PRODEN"]), "", "", dataTable3.Rows[index]["TIPMOV"].ToString().Trim(), "");
        //                                clsIdoc1.WRITE_IDOC_E1PITYP(cmd, "9005", DT, false);
        //                            }
        //                        }
        //                        else
        //                            break;
        //                    }
        //                }
        //            }
        //            if (blnCommit)
        //            {
        //                //Va in eccezione, nella tabella PREV è un char e causale invece è la denominazione del motivo di cessazione
        //                string str10 = cmd.Get1ValueFromSQL("SELECT VALUE(PREV, 'N') AS PREV FROM CAUCES WHERE CODCAUCES='" + Causale + "'", CommandType.Text).ToString().Trim();
        //                if (str10 == "S")
        //                {
        //                    string strSQL13 = "SELECT value(COUNT(*), 0) AS TOT " + " FROM STORDL INNER JOIN RAPLAV ON" + " STORDL.CODPOS = RAPLAV.CODPOS" + " AND STORDL.MAT=RAPLAV.MAT" + " AND STORDL.PRORAP=RAPLAV.PRORAP" + " WHERE STORDL.CODPOS ='" + CodPos + "' " + " AND STORDL.MAT ='" + Matricola + "' " + " AND STORDL.PRORAP ='" + ProRap + "' " + " AND STORDL.CODGRUASS IN (" + " SELECT CODGRUASS FROM ALIFORASS, FORASS " + " WHERE ALIFORASS.CODFORASS = FORASS.CODFORASS " + " AND FORASS.CATFORASS='TFR')";
        //                    if (Convert.ToInt32("0" + cmd.Get1ValueFromSQL(strSQL13, CommandType.Text)) == 0)
        //                        str10 = "N";
        //                }
        //                if (str10 == "S")
        //                {
        //                    blnCommit = clsPrev.WRITE_INSERT_MODPRE(CodPos, Matricola, ProRap, "0", "", "", "", "", "N", "N", "", "", " ", 0M, "", "", "", "", "", "");
        //                    if (blnCommit)
        //                    {
        //                        string strSQL14 = "SELECT DATDEC FROM RAPLAV WHERE" + " CODPOS ='" + CodPos + "' " + " AND MAT ='" + Matricola + "' " + " AND PRORAP ='" + ProRap + "' ";
        //                        string str11 = cmd.Get1ValueFromSQL(strSQL14, CommandType.Text);
        //                        dateTime1 = Convert.ToDateTime(str11);
        //                        int num3;
        //                        if (dateTime1.Year == num2)
        //                        {
        //                            dateTime1 = Convert.ToDateTime(str11);
        //                            num3 = dateTime1.Month;
        //                        }
        //                        else
        //                            num3 = 1;
        //                        dateTime1 = Convert.ToDateTime(DataCessazione);
        //                        int month = dateTime1.Month;
        //                        dateTime1 = Convert.ToDateTime(DataCessazione);
        //                        int year2 = dateTime1.Year;
        //                        DataTable dataTable4 = new DataTable();
        //                        int num4 = 0;
        //                        for (int index4 = 0; index4 <= 1; ++index4)
        //                        {
        //                            switch (index4)
        //                            {
        //                                case 0:
        //                                    if (year1 != num2)
        //                                    {
        //                                        for (int index5 = num3; index5 <= 12; ++index5)
        //                                        {
        //                                            dataTable4.Rows.InsertAt(dataTable4.NewRow(), num4);
        //                                            dataTable4.Rows[num4]["ANNO"] = (object)num2;
        //                                            dataTable4.Rows[num4]["MESE"] = (object)index5;
        //                                            ++num4;
        //                                        }
        //                                        break;
        //                                    }
        //                                    break;
        //                                case 1:
        //                                    for (int index6 = 1; index6 <= month; ++index6)
        //                                    {
        //                                        dataTable4.Rows.InsertAt(dataTable4.NewRow(), num4);
        //                                        dataTable4.Rows[num4]["ANNO"] = (object)year1;
        //                                        dataTable4.Rows[num4]["MESE"] = (object)index6;
        //                                        ++num4;
        //                                    }
        //                                    break;
        //                            }
        //                        }
        //                        if (year1 != num2)
        //                        {
        //                            blnCommit = clsPrev.WRITE_INSERT_PARPREV(CodPos, Matricola, ProRap, PROMOD);
        //                            if (blnCommit)
        //                                blnCommit = clsPrev.WRITE_INSERT_SOSPREV(CodPos, Matricola, ProRap, PROMOD, DataCessazione);
        //                        }
        //                        else
        //                        {
        //                            blnCommit = clsPrev.WRITE_INSERT_PARPREV(CodPos, Matricola, ProRap, PROMOD);
        //                            if (blnCommit)
        //                                blnCommit = clsPrev.WRITE_INSERT_SOSPREV(CodPos, Matricola, ProRap, PROMOD, DataCessazione);
        //                        }
        //                        string str12 = " ";
        //                        string TIPMOVSAN = " ";
        //                        if (blnCommit)
        //                        {
        //                            for (int index7 = 0; index7 <= dataTable4.Rows.Count - 1; ++index7)
        //                            {
        //                                string strSQL15 = "SELECT DENDET.CODPOS, DENDET.PRODEN, DENDET.PRODENDET, DENTES.TIPMOV  " + " FROM DENDET INNER JOIN DENTES ON" + " DENTES.CODPOS = DENDET.CODPOS " + " AND DENTES.ANNDEN = DENDET.ANNDEN " + " AND DENTES.MESDEN = DENDET.MESDEN " + " AND DENTES.PRODEN = DENDET.PRODEN " + " WHERE DENDET.CODPOS ='" + CodPos + "' " + " AND DENDET.MAT ='" + Matricola + "' " + " AND DENDET.PRORAP ='" + ProRap + "' " + " AND DENDET.ANNDEN = " + dataTable4.Rows[index7]["ANNO"]?.ToString() + " AND DENDET.MESDEN = " + dataTable4.Rows[index7]["MESE"]?.ToString() + " AND DENTES.TIPMOV IN ('DP', 'NU')" + " AND DENDET.NUMMOVANN IS NULL" + " AND VALUE(DENDET.ESIRET,'') <> 'S' ORDER BY DENTES.TIPMOV DESC";
        //                                DataTable dataTable5 = cmd.GetDataTable(strSQL15);
        //                                if (dataTable5.Rows.Count == 0)
        //                                {
        //                                    DataTable dtAziende = new DataTable();
        //                                    DataTable dtLOG = new DataTable();
        //                                    DataRow row = dtAziende.NewRow();
        //                                    row["CODPOS"] = (object)CodPos;
        //                                    row["ANNO"] = dataTable4.Rows[index7]["ANNO"];
        //                                    row["MESE"] = dataTable4.Rows[index7]["MESE"];
        //                                    row["TIPISC"] = (object)"";
        //                                    row["DATSCA"] = (object)"";
        //                                    row["RIMUOVI"] = (object)"";
        //                                    dtAziende.Rows.Add(row);
        //                                    DataTable dtNot = clsProtocolloDll.Genera_Notifiche(ref dtAziende, ref dtLOG, TIPMOVSAN, "", OPTIONAL_MATRICOLA: Convert.ToInt32(Matricola), OPTIONAL_PRORAP: Convert.ToInt32(ProRap), PREV: true);
        //                                    if (dtNot.Rows.Count > 0)
        //                                    {
        //                                        string strSQL16 = "SELECT CODPOS, ANNDEN, MESDEN, PRODEN FROM DENTES WHERE CODPOS =  " + CodPos + " AND ANNDEN = " + dataTable4.Rows[index7]["ANNO"]?.ToString() + " AND MESDEN = " + dataTable4.Rows[index7]["MESE"]?.ToString() + " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL" + " AND TIPMOV IN ('DP', 'NU')";
        //                                        DataTable dataTable6 = cmd.GetDataTable(strSQL16);
        //                                        if (dataTable6.Rows.Count > 0)
        //                                        {
        //                                            DateTime now = DateTime.Now;
        //                                            string strSQL17 = "SELECT PRIORITA FROM TIPPRIRET WHERE CODTIPRET = 2";
        //                                            string TIPPRI = cmd.Get1ValueFromSQL(strSQL17, CommandType.Text);
        //                                            string TIPISC = Convert.ToString(cmd.Get1ValueFromSQL("SELECT TIPISC FROM AZISTO WHERE CODPOS = " + CodPos + " AND '" + dataTable6.Rows[0]["ANNDEN"]?.ToString() + "-" + dataTable6.Rows[0]["MESDEN"].ToString().Trim().PadLeft(2, '0') + "-01' BETWEEN DATINI AND DATFIN", CommandType.Text)).Trim();
        //                                            DateTime dateTime2 = now;
        //                                            string strSQL18 = "SELECT VALORE FROM PARGENDET WHERE CODPAR = 3" + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date("01/" + dataTable6.Rows[0]["MESDEN"].ToString().Trim().PadLeft(2, '0') + "/" + dataTable6.Rows[0]["ANNDEN"]?.ToString())) + " BETWEEN DATINI AND DATFIN";
        //                                            string DATINISAN = cmd.Get1ValueFromSQL(strSQL18, CommandType.Text);
        //                                            DateTime dateTime3 = Convert.ToDateTime(dateTime2);
        //                                            if (Convert.ToDateTime(DATINISAN) > Convert.ToDateTime(dateTime3))
        //                                                dateTime3 = Convert.ToDateTime(DATINISAN);
        //                                            blnCommit = clsProtocolloDll.Module_Rettifiche_01(u, ref dtNot, Convert.ToInt32(CodPos), Convert.ToInt32(dataTable6.Rows[0]["ANNDEN"]), Convert.ToInt32(dataTable6.Rows[0]["MESDEN"]), Convert.ToInt32(dataTable6.Rows[0]["PRODEN"]), Convert.ToInt32(Matricola), TIPMOVSAN, dateTime2.ToString().Trim(), TIPPRI, TIPISC, DATINISAN, dateTime3.ToString().Trim(), "S");
        //                                            if (blnCommit)
        //                                            {
        //                                                string strSQL19 = "SELECT * FROM DENDET WHERE CODPOS = '" + CodPos + "' " + " AND ANNDEN = '" + dataTable6.Rows[0]["ANNDEN"].ToString() + "' " + " AND MESDEN = '" + dataTable6.Rows[0]["MESDEN"].ToString() + "' " + " AND PRORAP = '" + ProRap + "' " + " AND PRODEN = '" + dataTable6.Rows[0]["PRODEN"].ToString() + "' " + " AND MAT = '" + Matricola + "' " + " AND TIPMOV = 'RT' ";
        //                                                DataTable dataTable7 = cmd.GetDataTable(strSQL19);
        //                                                for (int index8 = 0; index8 <= dataTable7.Rows.Count - 1; ++index8)
        //                                                {
        //                                                    int int32_1 = Convert.ToInt32(Convert.ToDateTime(dataTable4.Rows[index7]["ANNO"]));
        //                                                    dateTime1 = Convert.ToDateTime(dataTable7.Rows[index8]["AL"]);
        //                                                    int int32_2 = Convert.ToInt32(dateTime1.Month);
        //                                                    int int32_3 = Convert.ToInt32(Convert.ToDateTime(month));
        //                                                    string AL = year1 != int32_1 || int32_2 != int32_3 ? dataTable7.Rows[index8]["AL"].ToString().Trim() : DataCessazione;
        //                                                    blnCommit = clsPrev.WRITE_INSERT_MODPREDET(Convert.ToString((object)u), Convert.ToInt32(CodPos), Convert.ToInt32(Matricola), Convert.ToInt32(ProRap), Convert.ToInt32(PROMOD), Convert.ToInt32(dataTable7.Rows[index8]["ANNDEN"].ToString().Trim()), Convert.ToInt32(dataTable7.Rows[index8]["MESDEN"].ToString().Trim()), "DP", dataTable7.Rows[index8]["DAL"].ToString().Trim(), AL, "", "0.0", "0.0", dataTable7.Rows[index8]["IMPFIG"].ToString().Trim(), Convert.ToDecimal(dataTable7.Rows[index8]["IMPABB"].ToString().Trim()), Convert.ToDecimal(dataTable7.Rows[index8]["IMPASSCON"].ToString().Trim()), Convert.ToDecimal(0.0), "", "", "", "", "", "", Convert.ToDecimal(dataTable7.Rows[index8]["IMPMIN"].ToString().Trim()), dataTable7.Rows[index8]["DATDEC"].ToString().Trim(), dataTable7.Rows[index8]["DATCES"].ToString().Trim(), Convert.ToDecimal(dataTable7.Rows[index8]["NUMGGAZI"].ToString().Trim()), Convert.ToDecimal(dataTable7.Rows[index8]["NUMGGFIG"].ToString().Trim()), Convert.ToDecimal(dataTable7.Rows[index8]["NUMGGPER"].ToString().Trim()), Convert.ToDecimal(dataTable7.Rows[index8]["NUMGGDOM"].ToString().Trim()), Convert.ToDecimal(dataTable7.Rows[index8]["NUMGGSOS"].ToString().Trim()), Convert.ToDecimal(dataTable7.Rows[index8]["NUMGGCONAZI"].ToString().Trim()), Convert.ToDecimal(dataTable7.Rows[index8]["IMPSCA"].ToString().Trim()), Convert.ToDecimal(dataTable7.Rows[index8]["IMPTRAECO"].ToString().Trim()), dataTable7.Rows[index8]["ETAENNE"].ToString().Trim(), Convert.ToInt32(dataTable7.Rows[index8]["TIPRAP"].ToString().Trim()), dataTable7.Rows[index8]["FAP"].ToString().Trim(), Convert.ToDecimal(dataTable7.Rows[index8]["PERFAP"].ToString().Trim()), Convert.ToDecimal(0.0), Convert.ToDecimal(dataTable7.Rows[index8]["PERPAR"].ToString().Trim()), Convert.ToDecimal(dataTable7.Rows[index8]["PERAPP"].ToString().Trim()), Convert.ToInt32(dataTable7.Rows[index8]["CODCON"].ToString().Trim()), Convert.ToInt32(dataTable7.Rows[index8]["PROCON"].ToString().Trim()), dataTable7.Rows[index8]["TIPSPE"].ToString().Trim(), Convert.ToInt32(dataTable7.Rows[index8]["CODLOC"].ToString().Trim()), Convert.ToInt32(dataTable7.Rows[index8]["PROLOC"].ToString().Trim()), Convert.ToInt32(dataTable7.Rows[index8]["CODLIV"].ToString().Trim()), Convert.ToInt32(dataTable7.Rows[index8]["CODGRUASS"].ToString().Trim()), Convert.ToInt32(dataTable7.Rows[index8]["CODQUACON"].ToString().Trim()), Convert.ToDecimal(dataTable7.Rows[index8]["ALIQUOTA"].ToString().Trim()), dataTable7.Rows[index8]["DATNAS"].ToString().Trim(), 0);
        //                                                    if (!blnCommit)
        //                                                        break;
        //                                                }
        //                                            }
        //                                            else
        //                                                break;
        //                                        }
        //                                        else
        //                                        {
        //                                            for (int index9 = 0; index9 <= dtNot.Rows.Count - 1; ++index9)
        //                                            {
        //                                                int num5 = Convert.ToString(year1) == Convert.ToString(dataTable4.Rows[index7]["ANNO"]) ? 1 : 0;
        //                                                dateTime1 = Convert.ToDateTime(dtNot.Rows[index9]["AL"]);
        //                                                int num6 = dateTime1.Month == month ? 1 : 0;
        //                                                string AL = (num5 & num6) == 0 ? Convert.ToString(dtNot.Rows[index9]["AL"]) : DataCessazione;
        //                                                blnCommit = clsPrev.WRITE_INSERT_MODPREDET(Convert.ToString((object)u), Convert.ToInt32(CodPos), Convert.ToInt32(Matricola), Convert.ToInt32(ProRap), Convert.ToInt32(PROMOD), Convert.ToInt32(dtNot.Rows[index9]["ANNDEN"].ToString().Trim()), Convert.ToInt32(dtNot.Rows[index9]["MESDEN"].ToString().Trim()), "DP", dtNot.Rows[index9]["DAL"].ToString().Trim(), AL, "", "0.0", "0.0", dtNot.Rows[index9]["IMPFIG"].ToString().Trim(), Convert.ToDecimal(dtNot.Rows[index9]["IMPABB"].ToString().Trim()), Convert.ToDecimal(dtNot.Rows[index9]["IMPASSCON"].ToString().Trim()), Convert.ToDecimal(0.0), "", "", "", "", "", "", Convert.ToDecimal(dtNot.Rows[index9]["IMPMIN"].ToString().Trim()), dtNot.Rows[index9]["DATDEC"].ToString().Trim(), dtNot.Rows[index9]["DATCES"].ToString().Trim(), Convert.ToDecimal(dtNot.Rows[index9]["NUMGGAZI"].ToString().Trim()), Convert.ToDecimal(dtNot.Rows[index9]["NUMGGFIG"].ToString().Trim()), Convert.ToDecimal(dtNot.Rows[index9]["NUMGGPER"].ToString().Trim()), Convert.ToDecimal(dtNot.Rows[index9]["NUMGGDOM"].ToString().Trim()), Convert.ToDecimal(dtNot.Rows[index9]["NUMGGSOS"].ToString().Trim()), Convert.ToDecimal(dtNot.Rows[index9]["NUMGGCONAZI"].ToString().Trim()), Convert.ToDecimal(dtNot.Rows[index9]["IMPSCA"].ToString().Trim()), Convert.ToDecimal(dtNot.Rows[index9]["IMPTRAECO"].ToString().Trim()), dtNot.Rows[index9]["ETAENNE"].ToString().Trim(), Convert.ToInt32(dtNot.Rows[index9]["TIPRAP"].ToString().Trim()), dtNot.Rows[index9]["FAP"].ToString().Trim(), Convert.ToDecimal(dtNot.Rows[index9]["PERFAP"].ToString().Trim()), Convert.ToDecimal(0.0), Convert.ToDecimal(dtNot.Rows[index9]["PERPAR"].ToString().Trim()), Convert.ToDecimal(dtNot.Rows[index9]["PERAPP"].ToString().Trim()), Convert.ToInt32(dtNot.Rows[index9]["CODCON"].ToString().Trim()), Convert.ToInt32(dtNot.Rows[index9]["PROCON"].ToString().Trim()), dtNot.Rows[index9]["TIPSPE"].ToString().Trim(), Convert.ToInt32(dtNot.Rows[index9]["CODLOC"].ToString().Trim()), Convert.ToInt32(dtNot.Rows[index9]["PROLOC"].ToString().Trim()), Convert.ToInt32(dtNot.Rows[index9]["CODLIV"].ToString().Trim()), Convert.ToInt32(dtNot.Rows[index9]["CODGRUASS"].ToString().Trim()), Convert.ToInt32(dtNot.Rows[index9]["CODQUACON"].ToString().Trim()), Convert.ToDecimal(dtNot.Rows[index9]["ALIQUOTA"].ToString().Trim()), dtNot.Rows[index9]["DATNAS"].ToString().Trim(), 0);
        //                                            }
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        str12 = !(Convert.ToString(year1) == Convert.ToString(dataTable4.Rows[index7]["ANNO"]) & Convert.ToString(dataTable4.Rows[index7]["MESE"]) == Convert.ToString(month)) ? "" : DataCessazione;
        //                                        for (int index10 = 0; index10 <= dataTable5.Rows.Count - 1; ++index10)
        //                                        {
        //                                            if (Convert.ToString(year1) == Convert.ToString(dataTable4.Rows[index7]["ANNO"]))
        //                                            {
        //                                                string strSQL20 = "SELECT VALUE(COUNT(*),0) FROM DENDET WHERE CODPOS=" + CodPos + " AND MAT = " + Matricola + " AND ANNDEN = " + dataTable4.Rows[index7]["ANNO"]?.ToString() + " AND MESDEN < " + month.ToString() + " AND PRORAP = " + ProRap + " AND PRODEN = " + dataTable5.Rows[index10]["PRODEN"]?.ToString() + " AND TIPMOV = 'AR' AND NUMMOV IS NULL AND NUMMOVANN IS NULL ";
        //                                                if (Convert.ToInt32("0" + cmd.Get1ValueFromSQL(strSQL20, CommandType.Text)) != 0)
        //                                                {
        //                                                    ErroreMSG = "Attenzione esistono arretrati non contabilizzati. Per continuare contabilizzare prima gli arretrati.";
        //                                                    return false;
        //                                                }
        //                                            }
        //                                            else
        //                                            {
        //                                                string strSQL21 = "SELECT VALUE(COUNT(*),0) FROM DENDET WHERE CODPOS= '" + CodPos + "' " + " AND MAT = '" + Matricola + "' " + " AND ANNDEN = " + dataTable4.Rows[index7]["ANNO"]?.ToString() + " AND MESDEN > " + num3.ToString() + " AND PRODEN = " + dataTable5.Rows[index10]["PRODEN"]?.ToString() + " AND PRORAP = '" + ProRap + "' " + " AND TIPMOV = 'AR' AND NUMMOV IS NULL AND NUMMOVANN IS NULL ";
        //                                                if (Convert.ToInt32("0" + cmd.Get1ValueFromSQL(strSQL21, CommandType.Text)) != 0)
        //                                                {
        //                                                    ErroreMSG = "Attenzione esistono arretrati non contabilizzati. Per continuare contabilizzare prima gli arretrati.";
        //                                                    return false;
        //                                                }
        //                                            }
        //                                            blnCommit = clsPrev.WRITE_INSERT_DIPAPREV(CodPos, Matricola, ProRap, PROMOD, Convert.ToString(dataTable4.Rows[index7]["MESE"]), Convert.ToString(dataTable5.Rows[index10]["PRODEN"]), Convert.ToString(dataTable5.Rows[index10]["PRODENDET"]), Convert.ToString(dataTable4.Rows[index7]["ANNO"]), Convert.ToString(dataTable5.Rows[index10]["TIPMOV"]));
        //                                            if (!blnCommit)
        //                                                break;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                            if (true & blnCommit)
        //                            {
        //                                string strSQL22 = "SELECT VALUE(COUNT(*), 0) FROM MODPREDET WHERE " + " CODPOS ='" + CodPos + "' " + " AND MAT ='" + Matricola + "' " + " AND PRORAP ='" + ProRap + "' " + " AND PROMOD ='" + PROMOD + "' " + " AND TIPMOV = 'AR'";
        //                                if (Convert.ToInt32("0" + cmd.Get1ValueFromSQL(strSQL22, CommandType.Text)) == 0)
        //                                {
        //                                    string str13 = "SELECT DENDET.CODPOS, DENDET.TIPMOV AS TIPMOV_QRY, DENDET.PRODEN, DENDET.ANNDEN, DENDET.MESDEN, DENDET.PRODENDET, DENTES.TIPMOV  " + " FROM DENDET INNER JOIN DENTES ON" + " DENTES.CODPOS = DENDET.CODPOS " + " AND DENTES.ANNDEN = DENDET.ANNDEN " + " AND DENTES.MESDEN = DENDET.MESDEN " + " AND DENTES.PRODEN = DENDET.PRODEN " + " WHERE DENDET.CODPOS ='" + CodPos + "' " + " AND DENDET.MAT ='" + Matricola + "' " + " AND DENDET.PRORAP ='" + ProRap + "' ";
        //                                    string str14;
        //                                    if (year1 != num2)
        //                                        str14 = str13 + " AND DENDET.ANNCOM IN (" + year2.ToString() + ", " + (year2 - 1).ToString() + ")";
        //                                    else
        //                                        str14 = str13 + " AND DENDET.ANNCOM IN (" + year2.ToString() + ")";
        //                                    string strSQL23 = str14 + " AND DENDET.TIPMOV IN ('AR', 'RT') " + " AND DENDET.NUMMOVANN IS NULL" + " AND VALUE(DENDET.ESIRET,'') <> 'S' ORDER BY DENTES.TIPMOV DESC";
        //                                    DataTable dataTable8 = cmd.GetDataTable(strSQL23);
        //                                    if (dataTable8.Rows.Count > 0)
        //                                    {
        //                                        for (int index = 0; index <= dataTable8.Rows.Count - 1; ++index)
        //                                        {
        //                                            clsPrev.WRITE_INSERT_DIPAPREV(CodPos, Matricola, ProRap, PROMOD, dataTable8.Rows[index]["MESDEN"].ToString(), dataTable8.Rows[index]["PRODEN"].ToString(), dataTable8.Rows[index]["PRODENDET"].ToString(), year2.ToString(), dataTable8.Rows[index]["ANNDEN"].ToString());
        //                                            if (!blnCommit)
        //                                                break;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                            if (blnCommit)
        //                            {
        //                                string strSQL24 = "UPDATE MODPREDET SET AL = '" + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DataCessazione)) + "'" + " WHERE CODPOS ='" + CodPos + "' " + " AND MAT ='" + Matricola + "' " + " AND PRORAP ='" + ProRap + "' " + " AND PROMOD ='" + PROMOD + "' " + " AND ANNDEN = " + year2.ToString() + " AND MESDEN = " + month.ToString();
        //                                blnCommit = cmd.WriteTransactionData(strSQL24, CommandType.Text);
        //                            }
        //                            if (blnCommit)
        //                            {
        //                                string strSQL25 = "SELECT AZIRAP.*, CODCOM.SIGPRO AS SIGPRONAS FROM AZIRAP " + " LEFT JOIN CODCOM ON AZIRAP.CODCOMNAS = CODCOM.CODCOM " + " WHERE CODPOS ='" + CodPos + "' " + " AND RAPPRI = 'S'" + " AND DATINI = (SELECT MAX(DATINI) FROM AZIRAP WHERE RAPPRI = 'S' AND CODPOS = " + CodPos + ")";
        //                                DataTable dataTable9 = cmd.GetDataTable(strSQL25);
        //                                if (dataTable9.Rows.Count > 0)
        //                                {
        //                                    string str15;
        //                                    if (dataTable9.Rows[0]["COG"].ToString().Trim() != "")
        //                                        str15 = "COGRAP = " + DBMethods.DoublePeakForSql(dataTable9.Rows[0]["COG"].ToString().Trim()) + ", ";
        //                                    if (dataTable9.Rows[0]["NOM"].ToString().Trim() != "")
        //                                        str15 = "NOMRAP = " + DBMethods.DoublePeakForSql(dataTable9.Rows[0]["NOM"].ToString().Trim()) + ", ";
        //                                    if (dataTable9.Rows[0]["CODFIS"].ToString().Trim() != "")
        //                                        str15 = "CODFISRAP = " + DBMethods.DoublePeakForSql(dataTable9.Rows[0]["CODFIS"].ToString().Trim()) + ", ";
        //                                    if (dataTable9.Rows[0]["CODFUNRAP"].ToString().Trim() != "")
        //                                        str15 = "CODFUNRAP = " + DBMethods.DoublePeakForSql(dataTable9.Rows[0]["CODFUNRAP"].ToString().Trim()) + ", ";
        //                                    if (dataTable9.Rows[0]["DATNAS"].ToString().Trim() != "")
        //                                        str15 = "DATNASRAP = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataTable9.Rows[0]["DATNAS"].ToString().Trim())) + ", ";
        //                                    if (dataTable9.Rows[0]["CODCOMNAS"].ToString().Trim() != "")
        //                                        str15 = "CODCOMNASRAP = " + DBMethods.DoublePeakForSql(dataTable9.Rows[0]["CODCOMNAS"].ToString().Trim()) + ", ";
        //                                    if (dataTable9.Rows[0]["SIGPRONAS"].ToString().Trim() != "")
        //                                        str15 = "SIGPRONASRAP = " + DBMethods.DoublePeakForSql(dataTable9.Rows[0]["SIGPRONAS"].ToString().Trim()) + ", ";
        //                                    if (dataTable9.Rows[0]["CODDUG"].ToString().Trim() != "")
        //                                        str15 = "CODDUGRAP = " + dataTable9.Rows[0]["CODDUG"].ToString().Trim() + ", ";
        //                                    if (dataTable9.Rows[0]["IND"].ToString().Trim() != "")
        //                                        str15 = "INDRAP = " + DBMethods.DoublePeakForSql(dataTable9.Rows[0]["IND"].ToString().Trim()) + ", ";
        //                                    if (dataTable9.Rows[0]["NUMCIV"].ToString().Trim() != "")
        //                                        str15 = "NUMCIVRAP = " + DBMethods.DoublePeakForSql(dataTable9.Rows[0]["NUMCIV"].ToString().Trim()) + ", ";
        //                                    if (dataTable9.Rows[0]["DENLOC"].ToString().Trim() != "")
        //                                        str15 = "DENLOCRAP = " + DBMethods.DoublePeakForSql(dataTable9.Rows[0]["DENLOC"].ToString().Trim()) + ", ";
        //                                    if (dataTable9.Rows[0]["DENSTAEST"].ToString().Trim() != "")
        //                                        str15 = "DENSTAESTRAP = " + DBMethods.DoublePeakForSql(dataTable9.Rows[0]["DENSTAEST"].ToString().Trim()) + ", ";
        //                                    if (dataTable9.Rows[0]["CAP"].ToString().Trim() != "")
        //                                        str15 = "CAPRAP = " + DBMethods.DoublePeakForSql(dataTable9.Rows[0]["CAP"].ToString().Trim()) + ", ";
        //                                    if (dataTable9.Rows[0]["SIGPRO"].ToString().Trim() != "")
        //                                        str15 = "SIGPRORAP = " + DBMethods.DoublePeakForSql(dataTable9.Rows[0]["SIGPRO"].ToString().Trim()) + ", ";
        //                                    if (dataTable9.Rows[0]["TEL1"].ToString().Trim() != "")
        //                                        str15 = "TELRAP = " + DBMethods.DoublePeakForSql(dataTable9.Rows[0]["TEL1"].ToString().Trim()) + ", ";
        //                                    if (dataTable9.Rows[0]["EMAIL"].ToString().Trim() != "")
        //                                        str15 = "EMAILRAP = " + DBMethods.DoublePeakForSql(dataTable9.Rows[0]["EMAIL"].ToString().Trim()) + ", ";
        //                                    if (dataTable9.Rows[0]["EMAILCERT"].ToString().Trim() != "")
        //                                        str15 = "PECRAP = " + DBMethods.DoublePeakForSql(dataTable9.Rows[0]["EMAILCERT"].ToString().Trim()) + ", ";
        //                                    if (dataTable9.Rows[0]["DENCOMRES"].ToString().Trim() != "")
        //                                        str15 = "DENCOMRAPRES = " + DBMethods.DoublePeakForSql(dataTable9.Rows[0]["DENCOMRES"].ToString().Trim()) + ", ";
        //                                    if (dataTable9.Rows[0]["CODCOMRES"].ToString().Trim() != "")
        //                                        str15 = "CODCOMRAPRES = " + DBMethods.DoublePeakForSql(dataTable9.Rows[0]["CODCOMRES"].ToString().Trim()) + ", ";
        //                                }
        //                                string strSQL26 = "UPDATE MODPRE SET " + " ULTAGG = CURRENT_TIMESTAMP, " + " UTEAGG = '" + DBMethods.DoublePeakForSql(u.Username) + "' " + " WHERE CODPOS ='" + CodPos + "' " + " AND MAT ='" + Matricola + "' " + " AND PRORAP ='" + ProRap + "' " + " AND PROMOD ='" + PROMOD + "' ";
        //                                blnCommit = cmd.WriteTransactionData(strSQL26, CommandType.Text);
        //                            }
        //                            if (blnCommit)
        //                            {
        //                                string strSQL27 = " SELECT DATFIN FROM UTEPIN WHERE CODUTE IN" + " (SELECT CODUTE FROM CONSUL WHERE CODTER IN " + " (SELECT CODTER FROM DELEGHE WHERE CODPOS=" + CodPos + " AND CURRENT_DATE" + " BETWEEN DATINI AND VALUE(DATFIN,'9999-12-31')))";
        //                                string str16;
        //                                switch (cmd.Get1ValueFromSQL(strSQL27, CommandType.Text))
        //                                {
        //                                    case "":
        //                                    case "1899-12-31":
        //                                        str16 = "C";
        //                                        break;
        //                                    default:
        //                                        str16 = "T";
        //                                        break;
        //                                }
        //                                string strSQL28 = "UPDATE MODPRE SET TIPPRE = " + DBMethods.DoublePeakForSql(str16) + " WHERE CODPOS ='" + CodPos + "' " + " AND MAT ='" + Matricola + "' " + " AND PRORAP ='" + ProRap + "' " + " AND PROMOD ='" + PROMOD + "' ";
        //                                blnCommit = cmd.WriteTransactionData(strSQL28, CommandType.Text);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            if (blnCommit)
        //            {
        //                if (Causale.ToString().Trim() == "10")
        //                {
        //                    clsIDOC clsIdoc2 = clsIdoc1;
        //                    DataLayer dataLayer2 = cmd;
        //                    int int32_4 = Convert.ToInt32(Matricola);
        //                    dateTime1 = Convert.ToDateTime(DataCessazione);
        //                    dateTime1 = dateTime1.AddDays(1.0);
        //                    string DATBEGDA1 = dateTime1.ToString();
        //                    DataTable idocDatiE1Pitype1 = clsIdoc2.GET_IDOC_DATI_E1PITYPE(dataLayer2, "0000", 0, int32_4, 0, 0, "VI", "01", "9999-12-31", DATBEGDA1, "", "", 0, 0, 0, "", "", "", "");
        //                    clsIdoc1.WRITE_IDOC_TESTATA(cmd, idocDatiE1Pitype1.Rows[0]);
        //                    idocDatiE1Pitype1.Clear();
        //                    clsIDOC clsIdoc3 = clsIdoc1;
        //                    DataLayer dataLayer3 = cmd;
        //                    int int32_5 = Convert.ToInt32(Matricola);
        //                    dateTime1 = Convert.ToDateTime(DataCessazione);
        //                    dateTime1 = dateTime1.AddDays(1.0);
        //                    string DATBEGDA2 = dateTime1.ToString();
        //                    DataTable idocDatiE1Pitype2 = clsIdoc3.GET_IDOC_DATI_E1PITYPE(dataLayer3, "0000", 0, int32_5, 0, 0, "VI", "01", "9999-12-31", DATBEGDA2, "", "", 0, 0, 0, "", "VARIAZIONE INQUADRAMENTO", "", "", "VI");
        //                    clsIdoc1.WRITE_IDOC_E1PITYP(cmd, "0000", idocDatiE1Pitype2, false);
        //                    idocDatiE1Pitype2.Clear();
        //                    DataTable idocDatiE1Pitype3 = clsIdoc1.GET_IDOC_DATI_E1PITYPE(cmd, "0016", 0, Convert.ToInt32(Matricola), 0, 0, "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "VARIAZIONE INQUADRAMENTO", "", "", "VI");
        //                    clsIdoc1.WRITE_IDOC_E1PITYP(cmd, "9001", idocDatiE1Pitype3, false);
        //                    clsIdoc1.WRITE_IDOC_E1PITYP(cmd, "0016", idocDatiE1Pitype3, false);
        //                }
        //                else
        //                {
        //                    DT.Clear();
        //                    DataTable idocDatiE1Pitype = clsIdoc1.GET_IDOC_DATI_E1PITYPE(cmd, "0016", Convert.ToInt32(CodPos), Convert.ToInt32(Matricola), Convert.ToInt32(ProRap), 0, "", "", Convert.ToString(9956), "", "", DataIscrizione, 0, 0, 0, "", "MODIFICA RDL", "", "");
        //                    clsIdoc1.WRITE_IDOC_E1PITYP(cmd, "9001", idocDatiE1Pitype, false);
        //                    clsIdoc1.WRITE_IDOC_E1PITYP(cmd, "0016", idocDatiE1Pitype, false);
        //                }
        //                blnCommit = clsIdoc1.AGGIORNA_RAPLAV_INPS(CodPos, Matricola, ProRap);
        //            }
        //            string str17 = u.Denominazione.ToString();
        //            dateTime1 = DateTime.Now;
        //            string DATPRO = dateTime1.ToString("d");
        //            string str18 = " ";
        //            string str19 = " ";
        //            string str20 = clsProtocolloDll.GeneraProtocolloLettera(CodPos, str17, Matricola.Trim(), Cognome.Trim(), Nome.Trim(), "", "AZ", "9.3", 3233010, 4792886, "RICEVUTA CESSAZIONE RDL - MAT : " + Matricola + " (" + Cognome.Trim() + " " + Nome.Trim() + ") - POS: " + CodPos, Convert.ToString((object)u), ref str18, ref DATPRO, "P");
        //            if (blnCommit)
        //            {
        //                string strSQL29 = "UPDATE RAPLAV SET PROTCES = " + DBMethods.DoublePeakForSql(str20) + ", " + " DATPROTCES = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATPRO)) + ", " + " NUMPROTCES = " + DBMethods.DoublePeakForSql(str18) + " WHERE CODPOS = '" + CodPos + "' " + " AND MAT =  '" + Matricola + "' " + " AND PRORAP = '" + ProRap + "' ";
        //                blnCommit = cmd.WriteTransactionData(strSQL29, CommandType.Text);
        //            }
        //            //Va in false il CreaStampaRicevutaProtocolloCessazioneRDL perche utilizza al suo interno il protocollo preso sopra che però è vuoto
        //            //if (blnCommit)
        //            //{
        //            //    str19 = clsProtocolloDll.PROT_PRES_GET_TEMP_PATH_APPLICATION() + "\\CESS_" + CodPos + "_" + Matricola + "_" + ProRap + ".pdf";
        //            //    blnCommit = clsProtocolloDll.CreaStampaRicevutaProtocolloCessazioneRDL(str17, Convert.ToInt32(Matricola), Cognome, Nome, DataCessazione, DATPRO, Causale, str20, str19);
        //            //}
        //            //if (blnCommit)
        //            //    blnCommit = clsProtocolloDll.Module_AllegaLetteraProtocollo(str19, str20, ref str18);
        //            bool flag;
        //            if (blnCommit)
        //            {
        //                string strSQL30 = "Select VALUE(COUNT(*), 0) FROM MODPRE" + " WHERE CODPOS =  '" + CodPos + "' " + " And MAT = '" + Matricola + "' " + " And PRORAP ='" + ProRap + "' " + " And DATANN Is NULL";
        //                if (Convert.ToInt32("0" + cmd.Get1ValueFromSQL(strSQL30, CommandType.Text)) > 1)
        //                {
        //                    cmd.EndTransaction(false);
        //                    flag = false;
        //                    dataLayer1 = (DataLayer)null;
        //                    return false;
        //                }
        //                else
        //                {
        //                    clsIdoc1.Aggiorna_IDOC(ref cmd);
        //                    //SALVARE IN TBDATPREAV --> IMPORTOPREAV, GIORNIPREAV, CODPOS, MAT, PRORAP, ULTAGG, UTEAGG
        //                    iDB2Parameter matricolaParam = cmd.CreateParameter("@matricola", iDB2DbType.iDB2Decimal, 11, ParameterDirection.Input, Matricola);
        //                    iDB2Parameter codposParam = cmd.CreateParameter("@codpos", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, CodPos);
        //                    iDB2Parameter prorapParam = cmd.CreateParameter("@proraptfr", iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input, ProRap);
        //                    iDB2Parameter userParam = cmd.CreateParameter("@user", iDB2DbType.iDB2VarChar, 20, ParameterDirection.Input, CodPos);
        //                    iDB2Parameter giorniPreavParam = cmd.CreateParameter("@proraptfr", iDB2DbType.iDB2Integer, 11, ParameterDirection.Input, giorniPreav.ToString());
        //                    iDB2Parameter importoPreavParam = cmd.CreateParameter("@proraptfr", iDB2DbType.iDB2Decimal, 14, ParameterDirection.Input, importoPreav.ToString());
        //                    string preavvisoSqlQuery = "INSERT INTO TBDATPREAV (MAT, CODPOS, PRORAP, GGPREAV, IMPPREAV, ULTAGG, UTEAGG) " +
        //                        "VALUES(@mat, @codpos, @prorap, @giornipreav, @importopreav, current date, @user)";
        //                    if (!cmd.WriteTransactionDataWithParametersAndDontCall(preavvisoSqlQuery, CommandType.Text, matricolaParam, codposParam, prorapParam, giorniPreavParam, importoPreavParam, userParam))
        //                    {
        //                        cmd.EndTransaction(false);
        //                        ErroreMSG = "Errore salvataggio dati di preavviso.";
        //                        return false;
        //                    }
        //                    cmd.EndTransaction(blnCommit);
        //                    flag = false;
        //                    SuccessMSG = "Operazione effettuata con successo!";
        //                    return true;
        //                }
        //            }
        //            else
        //            {
        //                cmd.EndTransaction(blnCommit);
        //                flag = false;
        //                ErroreMSG = "Si sono verificati dei problemi durante la conferma dei dati!";
        //                return false;
        //            }
        //        }
        //        cmd.EndTransaction(blnCommit);
        //        ErroreMSG = "Si sono verificati dei problemi durante la conferma dei dati!";
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        cmd.EndTransaction(blnCommit);
        //        ErroreMSG = "Si sono verificati dei problemi durante la conferma dei dati!";
        //        return false;
        //    }
        //}
        #endregion


        public bool CessazioneRDL(
          string iban,
          string matricola,
          string dataCessazione,
          string causale,
          string proRap,
          string codPos,
          int? giorniPreav,
          double? importoPreav,
          TFI.OCM.Utente.Utente utente,
          ref string ErroreMSG,
          ref string SuccessMSG)
        {
            DataLayer dataLayer = new DataLayer();
            dataLayer.StartTransaction();
            bool resultInsert = false;
            string codCauCess = GetCodCauCess(dataLayer, causale.Trim());
            string[] bkTbl = new string[5]
                    {
                        "RAPLAV",
                        "STORDL",
                        "IMPAGG",
                        "PARTIMM",
                        "SOSRAP"
                    };



            iDB2Parameter matricolaParam = dataLayer.CreateParameter("@matricola", iDB2DbType.iDB2Decimal, 11, ParameterDirection.Input, matricola);
            iDB2Parameter codposParam = dataLayer.CreateParameter("@codpos", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, codPos);
            iDB2Parameter codCauCesParam = dataLayer.CreateParameter("@codCauCes", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, codCauCess);
            iDB2Parameter prorapParam = dataLayer.CreateParameter("@prorap", iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input, proRap);
            iDB2Parameter userParam = dataLayer.CreateParameter("@user", iDB2DbType.iDB2VarChar, 20, ParameterDirection.Input, utente.Username);
            iDB2Parameter datCessParam = dataLayer.CreateParameter("@datCess", iDB2DbType.iDB2Date, 20, ParameterDirection.Input, dataCessazione);

            var getCodFisSqlQuery = "SELECT CODFIS FROM ISCT WHERE MAT = @matricola";
            var codfis = dataLayer.GetDataTableWithParameters(getCodFisSqlQuery, matricolaParam).Rows[0]["CODFIS"].ToString();
            var insertIbanResult = IbanUtils.CheckAndInsertIban(iban, matricola, dataLayer, ref ErroreMSG, codfis);

            if (!insertIbanResult)
            {
                dataLayer.EndTransaction(false);
                return false;
            }
            
            //SERVE???
            DateTime dataCess = DateTime.Parse(dataCessazione);
            int annoCessazione = dataCess.Year;
            int annoPrecedenteCessazione = annoCessazione - 1;
            string dataDecorrenzaRDLQuery = "SELECT DATDEC FROM RAPLAV WHERE CODPOS = @codpos AND MAT = @matricola AND PRORAP = @prorap";
            DataTable rdlTbl = dataLayer.GetDataTableWithParameters(dataDecorrenzaRDLQuery, codposParam, matricolaParam, prorapParam);
            if (rdlTbl.Rows.Count == 0)
            {
                ErroreMSG = "Non ci sono rapporti di lavoro attivi";
                dataLayer.EndTransaction(false);
                return false;
            }
            string dataDecorrenza = rdlTbl.Rows[0]["DATDEC"].ToString();
            if (DateTime.Parse(dataDecorrenza).Year == annoCessazione)
                annoPrecedenteCessazione = annoCessazione;
            ////////

            for (int index = 0; index < bkTbl.Length - 1; index++)
            {
                string query = $"SELECT * FROM {bkTbl[index]} WHERE CODPOS = @codpos AND MAT = @matricola AND PRORAP = @prorap";
                DataTable tblResult = dataLayer.GetDataTableWithParameters(query, codposParam, matricolaParam, prorapParam);

                string querybkp = $"INSERT INTO BK_{bkTbl[index]}_CESS (";
                for (int col = 0; col < tblResult.Columns.Count - 1; col++)
                {
                    if (col == 0)
                        querybkp += tblResult.Columns[col].ColumnName;
                    else
                        querybkp += ", " + tblResult.Columns[col].ColumnName;
                }
                querybkp += " ) SELECT ";
                for (int col = 0; col < tblResult.Columns.Count - 1; col++)
                {
                    if (col == 0)
                        querybkp += tblResult.Columns[col].ColumnName;
                    else
                        querybkp += ", " + tblResult.Columns[col].ColumnName;
                }
                querybkp += $" FROM {bkTbl[index]} WHERE CODPOS = @codpos AND MAT = @matricola AND PRORAP = @prorap";
                resultInsert = dataLayer.WriteTransactionDataWithParametersAndDontCall(querybkp, CommandType.Text, codposParam, matricolaParam, prorapParam);
                if (!resultInsert)
                {
                    break;
                }
            }

            if (resultInsert)
            {
                string updateRaplavQuery = "UPDATE RAPLAV SET FLGAPP = 'M', CODCAUCES = @codCauCes, DATCES = @datCess, UTEAGG = @user, ULTAGG = CURRENT_TIMESTAMP " +
                    "WHERE CODPOS = @codpos AND MAT = @matricola AND PRORAP = @prorap";
                resultInsert = dataLayer.WriteTransactionDataWithParametersAndDontCall(updateRaplavQuery, CommandType.Text, codCauCesParam, datCessParam, userParam, codposParam, matricolaParam, prorapParam);

            }
            if (resultInsert)
            {
                string updateStordlQuery = "UPDATE STORDL SET DATFIN = @datCess, UTEAGG = @user, ULTAGG = CURRENT_TIMESTAMP " +
                    "WHERE CODPOS = @codpos AND MAT = @matricola AND PRORAP = @prorap AND @datCess BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31')";
                resultInsert = dataLayer.WriteTransactionDataWithParametersAndDontCall(updateStordlQuery, CommandType.Text, datCessParam, userParam, codposParam, matricolaParam, prorapParam, datCessParam);
            }
            if (resultInsert)
            {
                string deleteImpaggQuery = "DELETE FROM IMPAGG WHERE CODPOS = @codpos AND MAT = @matricola AND PRORAP = @prorap AND DATINI > @datCess";
                resultInsert = dataLayer.WriteTransactionDataWithParametersAndDontCall(deleteImpaggQuery, CommandType.Text, codposParam, matricolaParam, prorapParam, datCessParam);
            }
            if (resultInsert)
            {
                string deletePartimmQuery = "DELETE FROM PARTIMM WHERE CODPOS = @codpos AND MAT = @matricola AND PRORAP = @prorap AND DATINI > @datCess";
                resultInsert = dataLayer.WriteTransactionDataWithParametersAndDontCall(deletePartimmQuery, CommandType.Text, codposParam, matricolaParam, prorapParam, datCessParam);
            }
            if (resultInsert)
            {
                string deleteStordlQuery = "DELETE FROM STORDL WHERE CODPOS = @codpos AND MAT = @matricola AND PRORAP = @prorap AND DATINI > @datCess";
                resultInsert = dataLayer.WriteTransactionDataWithParametersAndDontCall(deleteStordlQuery, CommandType.Text, codposParam, matricolaParam, prorapParam, datCessParam);
            }

            if (resultInsert)
            {
                string recordFromDendet = "SELECT ANNDEN, MESDEN, CODPOS, MAT, PRODEN, PRODENDET, AL, TIPMOV FROM DENDET " +
                    $"WHERE CODPOS = @codpos AND ANNDEN =  {dataCess.Year} AND MESDEN = {dataCess.Month} AND MAT = @matricola AND NUMMOVANN IS NULL ORDER BY TIPMOV ASC";
                DataTable denunce = dataLayer.GetDataTableWithParameters(recordFromDendet, codposParam, matricolaParam);

                for (int i = 0; i < denunce.Rows.Count - 1; i++)
                {
                    string updateDendet = "UPDATE DENDET SET AL = @datCess, UTEAGG = @user, ULTAGG = CURRENT_TIMESTAMP " +
                        $"WHERE CODPOS = @codpos AND ANNDEN = {denunce.Rows[i]["ANNDEN"]} AND MESDEN = {denunce.Rows[i]["MESDEN"]} AND PRODEN = {denunce.Rows[i]["PRODEN"]} AND PRODENDET = {denunce.Rows[i]["PRODENDET"]} AND MAT = @matricola";
                    resultInsert = dataLayer.WriteTransactionDataWithParametersAndDontCall(updateDendet, CommandType.Text, datCessParam, userParam, codposParam, matricolaParam);
                }
            }

            if (resultInsert)
            {
                if (giorniPreav.HasValue || importoPreav.HasValue)
                {
                    iDB2Parameter giorniPreavParam = dataLayer.CreateParameter("@proraptfr", iDB2DbType.iDB2Integer, 11, ParameterDirection.Input,  giorniPreav.HasValue ? giorniPreav.ToString() : "0");
                    iDB2Parameter importoPreavParam = dataLayer.CreateParameter("@proraptfr", iDB2DbType.iDB2Decimal, 14, ParameterDirection.Input, importoPreav.HasValue ? importoPreav.ToString() : "0");
                    string preavvisoSqlQuery = "INSERT INTO TBDATPREAV (MAT, CODPOS, PRORAP, GGPREAV, IMPPREAV, ULTAGG, UTEAGG) " +
                        "VALUES(@mat, @codpos, @prorap, @giornipreav, @importopreav, CURRENT_TIMESTAMP, @user)";
                    resultInsert = dataLayer.WriteTransactionDataWithParametersAndDontCall(preavvisoSqlQuery, CommandType.Text, matricolaParam, codposParam, prorapParam, giorniPreavParam, importoPreavParam, userParam);
                }
            }

            if (!resultInsert)
                ErroreMSG = "Errore nel salvataggio!";
            else
                SuccessMSG = "Cessazione Rapporto avvenuta con successo!";

            dataLayer.EndTransaction(resultInsert);
            return resultInsert;
        }

        public string GetEmailAzienda(string CodPos)
        {
            DataLayer dt = new DataLayer();
            string emailAzienda = dt.Get1ValueFromSQL("select email from azemail where codpos='" + CodPos + "' order by DATINI desc limit 1", CommandType.Text);
            return emailAzienda;
        }

        public DataRow GetEmailCodFisIscritto(string matricola)
        {
            DataLayer dt = new DataLayer();
            string sql = "select D.EMAIL, T.CODFIS from ISCD D JOIN ISCT T ON D.MAT = T.MAT " +
                "where D.MAT = " + matricola + " order by DATINI desc limit 1";
            DataTable isct = dt.GetDataTable(sql);
            return isct.Rows[0];

        }

        public string GetEmailConsulente(string CodPos)
        {
            DataLayer dt = new DataLayer();
            string sql = "select asster.email from deleghe, asster " +
                "where deleghe.codter = asster.codter " +
                "and deleghe.codpos = " + CodPos + " and deleghe.FLGATT=1";
            string emailConsulente = dt.Get1ValueFromSQL(sql, CommandType.Text);
            return emailConsulente;
        }

        private string GetCodCauCess(DataLayer dataLayer, string causale)
        {
            iDB2Parameter denCessParam = dataLayer.CreateParameter("@denCess", iDB2DbType.iDB2VarChar, 100, ParameterDirection.Input, causale);
            string queryCauCess = "SELECT CODCAUCES FROM CAUCES WHERE DENCES = @denCess";
            DataTable cauCessTbl = dataLayer.GetDataTableWithParameters(queryCauCess, denCessParam);
            return cauCessTbl.Rows[0]["CODCAUCES"].ToString();
        }
    }
}