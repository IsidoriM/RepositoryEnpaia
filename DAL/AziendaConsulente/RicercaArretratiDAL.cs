using IBM.Data.DB2.iSeries;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.AziendaConsulente;
using Utilities;

namespace TFI.DAL.AziendaConsulente
{
    public class RicercaArretratiDAL
    {
        private static readonly ILog log = LogManager.GetLogger("RollingFile");
        private static readonly ILog TrackLog = LogManager.GetLogger("Track");
        private static readonly DataLayer _dataLayer = new();
        public static List<ParametriGenerali> listaParametriGenerali;
        public static string ErrorMessage = "";

        public static List<CaricamentoAnniDenuncia> LoadAnniDenuncia(
          string codPos)
        {
            DataTable dataTable1 = new DataTable();
            DataLayer dataLayer = new DataLayer();
            List<CaricamentoAnniDenuncia> caricamentoAnniDenunciaList = new List<CaricamentoAnniDenuncia>();
            try
            {
                string strSQL = "SELECT DISTINCT TRIM(CHAR(ANNDEN)) AS TESTO, TRIM(CHAR(ANNDEN)) AS VALORE FROM " + "DENTES WHERE CODPOS = " + codPos + " AND TIPMOV = 'AR' AND DATCHI " + "IS NOT NULL AND NUMMOVANN IS NULL ORDER BY VALORE DESC";
                DataTable dataTable2 = dataLayer.GetDataTable(strSQL);
                if (dataTable2.Rows.Count <= 0)
                    return (List<CaricamentoAnniDenuncia>) null;
                caricamentoAnniDenunciaList.Add(new CaricamentoAnniDenuncia()
                {
                    testo = " ",
                    valore = -1
                });
                if (dataTable2.Rows.Count > 1)
                    caricamentoAnniDenunciaList.Add(new CaricamentoAnniDenuncia()
                    {
                        testo = "Tutti",
                        valore = 0
                    });
                foreach (DataRow row in (InternalDataCollectionBase) dataTable2.Rows)
                    caricamentoAnniDenunciaList.Add(new CaricamentoAnniDenuncia()
                    {
                        testo = !DBNull.Value.Equals(row["TESTO"]) ? row["TESTO"].ToString() : string.Empty,
                        valore = !DBNull.Value.Equals(row["VALORE"]) ? Convert.ToInt32(row["VALORE"]) : 0
                    });
                return caricamentoAnniDenunciaList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<CaricamentoAnniDenuncia> LoadAnniCompetenza(
          string codPos)
        {
            DataTable dataTable1 = new DataTable();
            DataLayer dataLayer = new DataLayer();
            List<CaricamentoAnniDenuncia> caricamentoAnniDenunciaList = new List<CaricamentoAnniDenuncia>();
            try
            {
                string strSQL = "SELECT DISTINCT TRIM(CHAR(ANNCOM)) AS TESTO, TRIM(CHAR(ANNCOM)) AS VALORE FROM DENDET " + "A INNER JOIN DENTES B ON A.CODPOS = B.CODPOS AND A.ANNDEN = B.ANNDEN AND A.PRODEN = " + "B.PRODEN WHERE A.CODPOS = " + codPos + " AND B.TIPMOV = 'AR' AND B.DATCHI " + "IS NOT NULL AND B.NUMMOVANN IS NULL ORDER BY VALORE DESC";
                DataTable dataTable2 = dataLayer.GetDataTable(strSQL);
                if (dataTable2.Rows.Count <= 0)
                    return (List<CaricamentoAnniDenuncia>) null;
                caricamentoAnniDenunciaList.Add(new CaricamentoAnniDenuncia()
                {
                    testo = " ",
                    valore = -1
                });
                if (dataTable2.Rows.Count >= 1)
                    caricamentoAnniDenunciaList.Add(new CaricamentoAnniDenuncia()
                    {
                        testo = "Tutti",
                        valore = 0
                    });
                foreach (DataRow row in (InternalDataCollectionBase) dataTable2.Rows)
                    caricamentoAnniDenunciaList.Add(new CaricamentoAnniDenuncia()
                    {
                        testo = !DBNull.Value.Equals(row["TESTO"]) ? row["TESTO"].ToString() : string.Empty,
                        valore = !DBNull.Value.Equals(row["VALORE"]) ? Convert.ToInt32(row["VALORE"]) : 0
                    });
                return caricamentoAnniDenunciaList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }        

        public static List<RicercaArretrati_Data> GetArretrati(string codPos, string matricola, string annoCompetenza, string annoDenuncia, ref string lblIntestazione)
        {
            try
            {
                var isRicercaByAnnoDenuncia = !string.IsNullOrEmpty(annoDenuncia);
                var isRicercaByAnnoCompetenza = !string.IsNullOrEmpty(annoCompetenza);
                var isRicercaByMatricola = !string.IsNullOrEmpty(matricola);
                var isMatricolaFound = false;

                var codPosParameter = _dataLayer.CreateParameter(DbParameters.CodicePosizione, iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codPos);
                var annoDenunciaParameter = new iDB2Parameter();
                var annoCompetenzaParameter = new iDB2Parameter();
                var matricolaParameter = new iDB2Parameter();

                var selectArretrati =
                    "SELECT A.CODPOS, A.ANNDEN, A.MESDEN, 0 AS CODMODPAG, A.PRODEN, ANNCOM, 0 AS MAT, '' AS AL, '' AS DAL, SUM(A.IMPRET) AS IMPRET,  SUM(A.IMPOCC) AS IMPOCC, " +
                    "SUM(A.IMPFIG) AS IMPFIG, SUM(A.IMPCON) AS IMPCON, SUM(A.IMPCONDEL + A.IMPASSCONDEL + A.IMPABBDEL + 0) AS IMPRTF, SUM(A.IMPRETDEL) AS IMPRETDEL, STADEN, DATEROARR, 0 AS INV " +
                    "FROM DENDET A INNER JOIN DENTES B ON A.CODPOS = B.CODPOS AND A.ANNDEN = B.ANNDEN AND A.MESDEN = B.MESDEN AND A.PRODEN = B.PRODEN " +
                    $"WHERE B.CODPOS = {DbParameters.CodicePosizione} AND B.TIPMOV = 'AR' ";

                lblIntestazione = CompleteQueryStringAndReturnIntestazioneCompleta();
                var arretratiFromDbRows = 
                    _dataLayer.GetDataTableWithParameters(selectArretrati, codPosParameter, annoDenunciaParameter, annoCompetenzaParameter, matricolaParameter).Rows.OfType<DataRow>();

                if (arretratiFromDbRows.Count() <= 0 || (isRicercaByMatricola && !isMatricolaFound))
                    return null;

                return MapArretratiFromDb(arretratiFromDbRows, lblIntestazione);

                string CompleteQueryStringAndReturnIntestazioneCompleta()
                {
                    var intestazione = "Riepilogo Denunce Arretrati";

                    IfAnnoDenunciaIsPresentFillItsParameterAndAdaptQuery();
                    IfAnnoCompetenzaIsPresentFillItsParameterAndAdaptQuery();
                    IfMatricolaIsPresentFillItsParameterAndAdaptQuery();
                    
                    selectArretrati += "GROUP BY A.CODPOS, A.ANNDEN, A.MESDEN, A.PRODEN, A.ANNCOM, STADEN, DATEROARR ORDER BY A.ANNDEN, A.MESDEN, A.ANNCOM";

                    return intestazione;

                    void IfAnnoDenunciaIsPresentFillItsParameterAndAdaptQuery()
                    {
                        if (!isRicercaByAnnoDenuncia) return;

                        if (annoDenuncia != "0")
                        {
                            annoDenunciaParameter = _dataLayer.CreateParameter(DbParameters.AnnoDenuncia, iDB2DbType.iDB2Decimal, 9, ParameterDirection.Input, annoDenuncia.Trim());
                            selectArretrati += $"AND A.ANNDEN = {DbParameters.AnnoDenuncia} ";
                            intestazione += $" effettuate nell'Anno {annoDenuncia.Trim()}";
                        }
                        else
                        {
                            selectArretrati += " AND A.ANNDEN IS NOT NULL ";
                        }
                    }

                    void IfAnnoCompetenzaIsPresentFillItsParameterAndAdaptQuery()
                    {
                        if (!isRicercaByAnnoCompetenza) return;

                        if (annoCompetenza != "0")
                        {
                            annoCompetenzaParameter = _dataLayer.CreateParameter(DbParameters.AnnoCompetenza, iDB2DbType.iDB2Decimal, 9, ParameterDirection.Input, annoCompetenza.Trim());
                            selectArretrati += $"AND A.ANNCOM = {DbParameters.AnnoCompetenza} ";
                            intestazione += $" per l'Anno di Competenza {annoCompetenza.Trim()}";
                        }
                        else
                        {
                            selectArretrati += " AND A.ANNCOM IS NOT NULL ";
                        }
                    }

                    void IfMatricolaIsPresentFillItsParameterAndAdaptQuery()
                    {
                        if(!isRicercaByMatricola) return;

                        matricolaParameter = _dataLayer.CreateParameter(DbParameters.Matricola, iDB2DbType.iDB2Decimal, 9, ParameterDirection.Input, matricola.Trim());
                        var getNominativoFromMatricolaQuery = $"SELECT TRIM(COG) || ' ' || TRIM(NOM) AS NOMINATIVO FROM ISCT WHERE MAT = {DbParameters.Matricola}";
                        var getNominativoFromMatricolaRows = _dataLayer.GetDataTableWithParameters(getNominativoFromMatricolaQuery, matricolaParameter).Rows;

                        if (getNominativoFromMatricolaRows.Count > 0)
                        {
                            isMatricolaFound = true;
                            var nominativoFromMatricola = getNominativoFromMatricolaRows[0].ElementAt("NOMINATIVO");
                            selectArretrati += $"AND A.MAT = {DbParameters.Matricola} ";
                            intestazione += $" relativo alla Matricola {matricola} - {nominativoFromMatricola}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Info(ex.Message, ex);
                return null;
            }
        }

        private static List<RicercaArretrati_Data> MapArretratiFromDb(IEnumerable<DataRow> arretratiFromQuery, string intestazioneArretrato)
        {
            int manualId = 0;
            int intDiAppoggio = 0;
            decimal decimalDiAppoggio = 0;
            return arretratiFromQuery.Select(MapArretrato).ToList();

            RicercaArretrati_Data MapArretrato(DataRow arretratoRowFromDb)
            {
                var arretratoMappato = new RicercaArretrati_Data()
                {
                    Id = manualId,
                    lblIntestazione = intestazioneArretrato,
                    AnnDen = int.TryParse(arretratoRowFromDb.ElementAt("ANNDEN"), out intDiAppoggio) ? intDiAppoggio : 0,
                    MesDen = int.TryParse(arretratoRowFromDb.ElementAt("MESDEN"), out intDiAppoggio) ? intDiAppoggio : 0,
                    ProDen = int.TryParse(arretratoRowFromDb.ElementAt("PRODEN"), out intDiAppoggio) ? intDiAppoggio : 0,
                    AnnCom = int.TryParse(arretratoRowFromDb.ElementAt("ANNCOM"), out intDiAppoggio) ? intDiAppoggio : 0,
                    CodModPag = int.TryParse(arretratoRowFromDb.ElementAt("CODMODPAG"), out intDiAppoggio) ? intDiAppoggio : 0,
                    Mat = int.TryParse(arretratoRowFromDb.ElementAt("MAT"), out intDiAppoggio) ? intDiAppoggio : 0,
                    Al = arretratoRowFromDb.ElementAt("AL"),
                    Dal = arretratoRowFromDb.ElementAt("DAL"),
                    ImpRet = decimal.TryParse(arretratoRowFromDb.ElementAt("IMPRET"), out decimalDiAppoggio) ? decimalDiAppoggio : 0,
                    ImpOcc = decimal.TryParse(arretratoRowFromDb.ElementAt("IMPOCC"), out decimalDiAppoggio) ? decimalDiAppoggio : 0,
                    ImpFig = decimal.TryParse(arretratoRowFromDb.ElementAt("IMPFIG"), out decimalDiAppoggio) ? decimalDiAppoggio : 0,
                    ImpCon = decimal.TryParse(arretratoRowFromDb.ElementAt("IMPCON"), out decimalDiAppoggio) ? decimalDiAppoggio : 0,
                    StaDen = arretratoRowFromDb.ElementAt("STADEN"),
                    DatEroArr = arretratoRowFromDb.ElementAt("DATEROARR"),
                    Inv = decimal.TryParse(arretratoRowFromDb.ElementAt("INV"), out decimalDiAppoggio) ? decimalDiAppoggio : 0,
                    ImpRtf = decimal.TryParse(arretratoRowFromDb.ElementAt("IMPRTF"), out decimalDiAppoggio) ? decimalDiAppoggio : 0,
                    ImpRetDel = decimal.TryParse(arretratoRowFromDb.ElementAt("IMPRETDEL"), out decimalDiAppoggio) ? decimalDiAppoggio : 0
                };
                arretratoMappato.meseSettato = DBMethods.GetMesi()[arretratoMappato.MesDen];
                ++manualId;
                return arretratoMappato;
            }
        }

        public static List<TabRettifiche> LoadRettifiche(
          string codPos,
          string anno,
          string mese,
          string proDen,
          string anncom,
          string mat,
          RettificheArretrati arretrato)
        {
            DateTimeFormatInfo dateTimeFormat = new CultureInfo("it-IT", false).DateTimeFormat;
            string str1 = "";
            DataLayer dataLayer = new DataLayer();
            DataTable dataTable1 = new DataTable();
            List<TabRettifiche> tabRettificheList = new List<TabRettifiche>();
            arretrato.lblTotContributi = 0M;
            arretrato.lblAddizionale = 0M;
            arretrato.lblTotale = 0M;
            arretrato.lblSanzione = 0M;
            try
            {
                string strSQL1 = "SELECT DATAPE, VALUE(IMPCONDEL, 0) AS IMPCONDEL, VALUE(IMPADDRECDEL, 0) AS IMPADDRECDEL, VALUE(IMPABBDEL,  0) " + "AS IMPABBDEL, VALUE(IMPASSCONDEL, 0) AS IMPASSCONDEL, VALUE(IMPSANRET, 0) AS IMPSANRET, TIPMOV, SANSOTSOG " + "FROM DENTES WHERE CODPOS = " + codPos + " AND ANNDEN = " + anno + " AND MESDEN = " + mese + " AND PRODEN = " + proDen;
                DataTable dataTable2 = dataLayer.GetDataTable(strSQL1);
                if (dataTable2.Rows.Count > 0)
                {
                    str1 = dataLayer.Get1ValueFromSQL(strSQL1, CommandType.Text);
                    arretrato.lblTotContributi = Convert.ToDecimal(dataTable2.Rows[0]["IMPCONDEL"]);
                    arretrato.lblAddizionale = Convert.ToDecimal(dataTable2.Rows[0]["IMPADDRECDEL"]);
                    arretrato.lblTotale = Convert.ToDecimal(dataTable2.Rows[0]["IMPCONDEL"]) + Convert.ToDecimal(dataTable2.Rows[0]["IMPADDRECDEL"]);
                    arretrato.lblSanzione = Convert.ToDecimal(dataTable2.Rows[0]["IMPSANRET"]);
                    arretrato.lblPeriodo = "Rettifiche Denuncia Arretrati relativa al Periodo ";
                    RettificheArretrati rettificheArretrati = arretrato;
                    rettificheArretrati.lblPeriodo = rettificheArretrati.lblPeriodo + DBMethods.GetMesi()[Convert.ToInt32(mese)].ToUpper() + " " + anno;
                }
                if (str1.Length > 10)
                    arretrato.denuncia = DateTime.Parse(str1.Substring(0, 10)).GetDateTimeFormats((IFormatProvider) dateTimeFormat)[0];
                string strSQL2 = "SELECT DISTINCT A.INSAZI, A.AZINOTC, A.CODPOS, A.RAGSOC, A.RAGSOCBRE, A.CODFIS AS CODFISAZI, A.PARIVA, A.NATGIU, " + "CHAR(A.ULTAGG) AS ULTAGG, NATGIU.DENNATGIU, TRANSLATE(CHAR(A.DATAPE, EUR), '/', '.') AS DATAPE, TRANSLATE(CHAR" + "(A.DATCHI, EUR), '/', '.') AS DATCHI, C.CATATTCAM, D.DENATTCAM, C.CODATTCAM, TRANSLATE(CHAR(C.DATINI, EUR), " + "'/', '.') AS DATINIATT, F.CODSTACON, F.DENCODSTA FROM AZI A INNER JOIN NATGIU ON A.NATGIU = NATGIU.NATGIU LEFT " + "JOIN AZIATT AS C ON A.CODPOS = C.CODPOS AND C.DATFIN = '9999-12-31' LEFT JOIN TIPATT AS D ON C.CATATTCAM = " + "D.CATATTCAM AND C.CODATTCAM = D.CODATTCAM LEFT JOIN AZISTO AS E ON A.CODPOS = E.CODPOS AND current_date BETWEEN " + "E.DATINI AND VALUE(E.DATFIN, '9999-12-31') LEFT JOIN CODSTA AS F ON E.CODSTACON = F.CODSTACON WHERE A.CODPOS = " + codPos;
                dataTable2.Clear();
                DataTable dataTable3 = dataLayer.GetDataTable(strSQL2);
                if (dataTable3.Rows.Count > 0)
                {
                    arretrato.CodFis = Convert.ToString(dataTable3.Rows[0]["CODFISAZI"]).Trim();
                    arretrato.pIVA = Convert.ToString(dataTable3.Rows[0]["PARIVA"]).Trim();
                    arretrato.hdNatGiu = Convert.ToString(dataTable3.Rows[0]["NATGIU"]).Trim();
                }
                string strSQL3 = "SELECT B.DENIND, CHAR(B.ULTAGG) AS ULTAGG, B.TIPIND, A.IND, A.DENLOC AS LOC_SEDE, A.CAP " + "AS CAP_SEDE, A.SIGPRO AS PROV_SEDE, A.TEL1, A.FAX, A.EMAIL,TRANSLATE(CHAR(A.DATINI, EUR), " + "'/', '.') AS DATINI FROM INDSED A INNER JOIN TIPIND B ON A.TIPIND = B.TIPIND LEFT JOIN " + "CODLOC AS LOCSEDE ON A.DENLOC = LOCSEDE.DENLOC AND A.CAP = LOCSEDE.CAP AND A.SIGPRO = " + "LOCSEDE.SIGPRO WHERE A.CODPOS = " + codPos + " AND current_date BETWEEN " + "A.DATINI AND VALUE(A.DATFIN, '9999-12-31')";
                dataTable3.Clear();
                DataTable dataTable4 = dataLayer.GetDataTable(strSQL3);
                if (dataTable4.Rows.Count > 0 && Convert.ToInt32(dataTable4.Rows[0]["TIPIND"]) == 1)
                {
                    arretrato.sedeLegale = Convert.ToString(dataTable4.Rows[0]["IND"]).Trim() + " ";
                    RettificheArretrati rettificheArretrati1 = arretrato;
                    rettificheArretrati1.sedeLegale = rettificheArretrati1.sedeLegale + Convert.ToString(dataTable4.Rows[0]["LOC_SEDE"]).Trim() + " ";
                    RettificheArretrati rettificheArretrati2 = arretrato;
                    rettificheArretrati2.sedeLegale = rettificheArretrati2.sedeLegale + "(" + Convert.ToString(dataTable4.Rows[0]["PROV_SEDE"]).Trim();
                    RettificheArretrati rettificheArretrati3 = arretrato;
                    rettificheArretrati3.sedeLegale = rettificheArretrati3.sedeLegale + ") Tel." + Convert.ToString(dataTable4.Rows[0]["TEL1"]).Trim();
                }
                string str2 = "SELECT B.MAT, (TRIM(A.COG) || ' ' || TRIM(A.NOM)) AS NOME, TRANSLATE(CHAR(DAL,EUR),'/','.') " + "AS DAL, ANNCOM, TRANSLATE(CHAR(AL,EUR),'/','.') AS AL, LEFT((SELECT DISTINCT TRIM(DENQUA) " + "FROM QUACON WHERE QUACON.CODQUACON = B.CODQUACON), 1) AS DENQUA, CODLIV, PERAPP, IMPRET, " + "IMPRETDEL, VALUE(IMPRETPRE, 0) AS IMPRETPRE, IMPOCCDEL, ALIQUOTA, IMPCONDEL, IMPABB, " + "IMPASSCON, CASE WHEN NUMSAN IS NULL THEN 0 ELSE IMPSANDET END AS IMPSANDET, PRODENDET " + "FROM DENDET B INNER JOIN ISCT A ON B.MAT = A.MAT WHERE CODPOS = " + codPos + " AND ANNDEN = " + anno + " AND MESDEN = " + mese + " AND PRODEN = " + proDen + " AND B.TIPMOV = 'RT' AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL " + "AND VALUE(ESIRET, '') <> 'S'";
                if (!string.IsNullOrEmpty(anncom))
                {
                    str2 = str2 + "AND B.ANNCOM = " + anncom;
                    RettificheArretrati rettificheArretrati = arretrato;
                    rettificheArretrati.lblPeriodo = rettificheArretrati.lblPeriodo + " per l'anno di competenza: " + anncom;
                    arretrato.annoCompetenzaVisible = false;
                }
                if (!string.IsNullOrEmpty(mat) && mat != "0")
                {
                    str2 = str2 + "AND B.MAT = " + mat;
                    RettificheArretrati rettificheArretrati = arretrato;
                    rettificheArretrati.lblPeriodo = rettificheArretrati.lblPeriodo + " per la Matricola: " + mat;
                }
                string strSQL4 = str2 + " ORDER BY ANNCOM, MAT, PRODENDET ";
                dataTable1 = dataLayer.GetDataTable(strSQL4);
                return tabRettificheList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static bool UploadArretrati(
          TFI.OCM.Utente.Utente utente,
          string codPos,
          int anno,
          HttpPostedFileBase dipa,
          string strPath,
          ref string PRODEN,
          ref bool btnStampaIsVisible,
          ref bool btnConfermaIsVisible)
        {
            DataLayer db1 = new DataLayer();
            int intProDenDet = 0;
            string newLine = Environment.NewLine;
            string[] strArray = new string[10];
            StreamReader streamReader = (StreamReader) null;
            DataTable dataTable1 = new DataTable();
            bool blnCommit = false;
            bool flag1 = false;
            int PRODEN1 = 0;
            int num1 = 0;
            string TIPDEN = (string) null;
            Decimal num2 = 0M;
            Decimal num3 = 0M;
            string MAT = "";
            bool flag2 = false;
            bool flag3 = false;
            DataTable dataTable2 = new DataTable();
            List<DenunciaArretrati> denunciaArretratiList1 = new List<DenunciaArretrati>();
            List<DenunciaArretrati> denunciaArretratiList2 = new List<DenunciaArretrati>();
            try
            {
                if (File.Exists(strPath))
                    File.Delete(strPath);
                Stream inputStream = dipa.InputStream;
                FileStream fileStream = File.OpenWrite(strPath);
                byte[] instance = (byte[]) Array.CreateInstance(Type.GetType("System.Byte"), inputStream.Length);
                inputStream.Read(instance, 0, (int) inputStream.Length);
                fileStream.Write(instance, 0, (int) inputStream.Length);
                fileStream.Close();
                inputStream.Close();
                inputStream.Dispose();
                DateTime dateTime = DateTime.Today;
                string s1 = dateTime.Year.ToString();
                dateTime = DateTime.Today;
                string s2 = dateTime.Month.ToString();
                if (db1 == null)
                    db1 = new DataLayer();
                db1.StartTransaction();
                flag1 = true;
                string strSQL1 = "SELECT PRODEN FROM DENTES WHERE" + " CODPOS = " + codPos + " AND ANNDEN = " + s1 + " AND" + " MESDEN = " + s2 + " AND TIPMOV = 'AR' AND STADEN = 'N' AND" + " DATMOVANN IS NULL";
                string str1 = db1.Get1ValueFromSQL(strSQL1, CommandType.Text) ?? "";
                if (str1 != "")
                {
                    string strSQL2 = "DELETE FROM DENDETALI WHERE CODPOS = " + codPos + " AND ANNDEN = " + s1 + " AND MESDEN = " + s2 + " AND PRODEN = " + str1;
                    if (db1.WriteTransactionData(strSQL2, CommandType.Text))
                    {
                        string strSQL3 = "DELETE FROM DENDETSOS WHERE CODPOS = " + codPos + " AND ANNDEN = " + s1 + " AND MESDEN = " + s2 + " AND PRODEN = " + str1;
                        if (db1.WriteTransactionData(strSQL3, CommandType.Text))
                        {
                            string strSQL4 = "DELETE FROM DENDET WHERE CODPOS = " + codPos + " AND ANNDEN = " + s1 + " AND MESDEN = " + s2 + " AND PRODEN = " + str1;
                            if (db1.WriteTransactionData(strSQL4, CommandType.Text))
                            {
                                string strSQL5 = "DELETE FROM DENTES WHERE CODPOS = " + codPos + " AND ANNDEN = " + s1 + " AND MESDEN = " + s2 + " AND PRODEN = " + str1;
                                flag2 = db1.WriteTransactionData(strSQL5, CommandType.Text);
                            }
                        }
                    }
                    if (!flag2)
                        flag3 = true;
                }
                string DAL = "01/01/" + anno.ToString();
                string AL = "31/12/" + anno.ToString();
                List<ParametriGenerali> listaParametriGen = (List<ParametriGenerali>) null;
                GeneraDenunciaDAL generaDenunciaDal1 = new GeneraDenunciaDAL();
                List<DenunciaArretrati> denunciaArretratiList3 = generaDenunciaDal1.GeneraDenunciaArr(DAL, AL, ref listaParametriGen, codPos, blnArretrato: true);
                streamReader = File.OpenText(strPath);
                while (streamReader.Peek() >= 0)
                {
                    if (PRODEN1 == 0)
                    {
                        DataLayer db2 = db1;
                        string username = utente.Username;
                        string CODPOS = codPos;
                        int ANNDEN = int.Parse(s1);
                        int MESDEN = int.Parse(s2);
                        dateTime = DateTime.Now;
                        string DATAPE = dateTime.ToString();
                        string UTEAPE = utente.Tipo.Trim();
                        PRODEN1 = WriteDIPA.WRITE_INSERT_DENTES(db2, username, CODPOS, ANNDEN, MESDEN, DATAPE, UTEAPE, "", "", "AR", "O", "N", 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, "", "", "", 0, "", "0", 0, 0.0M, "", "", "", "", 0M, 0M, "", 0, 0, "N", "", "");
                        if (PRODEN1 > 0)
                            PRODEN = PRODEN1.ToString();
                    }
                    if (PRODEN != "")
                    {
                        bool flag4 = true;
                        string str2 = streamReader.ReadLine();
                        strArray[0] = str2.Substring(0, 16);
                        strArray[1] = str2.Substring(16, 2);
                        strArray[2] = str2.Substring(18, 1);
                        strArray[3] = str2.Substring(19, 4);
                        strArray[4] = str2.Substring(23, 4);
                        strArray[5] = str2.Substring(27, 5);
                        strArray[5] = strArray[5].Substring(0, 2) + "," + strArray[5].Substring(2, 3);
                        Decimal num4 = Convert.ToDecimal(strArray[5]);
                        strArray[6] = str2.Substring(32, 9);
                        strArray[6] = strArray[6].Substring(0, 7) + "," + strArray[6].Substring(7, 2);
                        Decimal d1 = Decimal.Round(Convert.ToDecimal(strArray[6]), 2);
                        strArray[7] = str2.Substring(41, 9);
                        strArray[7] = strArray[7].Substring(0, 7) + "," + strArray[7].Substring(7, 2);
                        Decimal d2 = Decimal.Round(Convert.ToDecimal(strArray[7]), 2);
                        strArray[8] = str2.Substring(50, 9);
                        strArray[8] = strArray[8].Substring(0, 7) + "," + strArray[8].Substring(7, 2);
                        num2 = Decimal.Round(Convert.ToDecimal(strArray[8]), 2);
                        strArray[9] = anno.ToString();
                        num3 = (Convert.ToDecimal(strArray[6]) + Convert.ToDecimal(strArray[7]) / 100M) * num4;
                        DAL = anno.ToString() + "-" + strArray[3].Substring(0, 2) + "-" + strArray[3].Substring(2);
                        AL = anno.ToString() + "-" + strArray[4].Substring(0, 2) + "-" + strArray[4].Substring(2);
                        string strSQL6 = "SELECT MAT FROM ISCT WHERE CODFIS = '" + strArray[0] + "'";
                        MAT = db1.Get1ValueFromSQL(strSQL6, CommandType.Text) ?? "";
                        if (string.IsNullOrEmpty(MAT))
                        {
                            flag3 = true;
                            break;
                        }
                        List<DenunciaArretrati> list = denunciaArretratiList3.Where<DenunciaArretrati>((Func<DenunciaArretrati, bool>) (r => r.mat == Convert.ToInt32(MAT) && r.datadal.Substring(0, 10) == DateTime.Parse(DAL).ToString("d") && r.dataal.Substring(0, 10) == DateTime.Parse(AL).ToString("d"))).ToList<DenunciaArretrati>();
                        if (Convert.ToDecimal("0" + list[0].aliquota) == 0M)
                        {
                            flag4 = false;
                            flag3 = true;
                        }
                        else if (list[0].datadal == DAL && list[0].dataal != AL)
                        {
                            DenunciaArretrati denunciaArretrati = new DenunciaArretrati();
                            if (list.Last<DenunciaArretrati>().dataal != AL)
                            {
                                flag4 = false;
                                flag3 = true;
                            }
                        }
                        if (flag4)
                        {
                            flag2 = RicercaArretratiDAL.WRITE_INSERT_DENDET2(db1, denunciaArretratiList3, RicercaArretratiDAL.listaParametriGenerali, utente, codPos, int.Parse(s1), int.Parse(s2), PRODEN1, int.Parse(MAT), "AR", DAL, AL, "", Decimal.Round(d1, 0), Decimal.Round(d2, 0), 0M, 0M, 0M, Convert.ToDecimal(list[0].impcon), 0.0M, "N", 0, "", "", 0.0M, 0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, list[0].eta65.ToString(), 0, list[0].fap, Convert.ToDecimal(list[0].perfap), list[0].impfap, (Decimal) list[0].perpar, list[0].perapp, list[0].prorap, Convert.ToInt32(list[0].codcon), Convert.ToInt32(list[0].procon), list[0].tipse, list[0].codloc, Convert.ToInt32(list[0].proloc), list[0].codliv, list[0].codgruass, list[0].codquacon, Convert.ToDecimal(strArray[5]), list[0].datnas, (int) Convert.ToInt16(strArray[9]), TIPDEN, ref intProDenDet);
                            if (flag2)
                                ++num1;
                            else
                                flag3 = true;
                        }
                    }
                    else
                    {
                        flag3 = true;
                        break;
                    }
                }
                streamReader.Close();
                if (flag2 & !flag3)
                {
                    string strSQL7 = "SELECT SUM(IMPRET) AS IMPRET, SUM(IMPOCC) AS IMPOCC, SUM(IMPCON) AS IMPCON, " + "SUM(IMPABB) AS IMPABB, SUM(IMPASSCON) AS IMPASSCON FROM DENDET WHERE CODPOS = " + codPos + " AND ANNDEN = " + s1.ToString() + " AND MESDEN = " + s2.ToString() + " AND PRODEN = " + PRODEN1.ToString() + " AND TIPMOV = 'AR'";
                    dataTable1?.Rows.Clear();
                    DataTable dataTable3 = db1.GetDataTable(strSQL7);
                    if (dataTable3.Rows.Count > 0)
                    {
                        string strSQL8 = "UPDATE DENTES SET IMPRET = " + dataTable3.Rows[0]["IMPRET"].ToString().Replace(",", ".") + ", IMPOCC = " + dataTable3.Rows[0]["IMPOCC"].ToString().Replace(",", ".") + ", IMPCON = " + dataTable3.Rows[0]["IMPCON"].ToString().Replace(",", ".") + ", IMPABB = " + dataTable3.Rows[0]["IMPABB"].ToString().Replace(",", ".") + ", IMPASSCON = " + dataTable3.Rows[0]["IMPASSCON"].ToString().Replace(",", ".") + ", NUMRIGDET = " + num1.ToString() + " WHERE CODPOS = " + utente.CodPosizione + " AND ANNDEN = " + s1.ToString() + " AND MESDEN = " + s2.ToString() + " AND PRODEN = " + PRODEN1.ToString() + " AND TIPMOV = 'AR'";
                        blnCommit = db1.WriteTransactionData(strSQL8, CommandType.Text);
                        if (blnCommit)
                        {
                            GeneraDenunciaDAL generaDenunciaDal2 = generaDenunciaDal1;
                            dateTime = DateTime.Now;
                            string strDataDal = dateTime.ToString();
                            string strSQL9 = "UPDATE DENTES SET IMPADDREC = ROUND(((IMPCON / 100) * " + generaDenunciaDal2.GetImportoParametro((short) 5, strDataDal).ToString().Replace(",", ".") + "), 2) " + "WHERE CODPOS = " + codPos + " AND ANNDEN = " + s1.ToString() + " AND MESDEN = " + s2.ToString() + " AND PRODEN = " + PRODEN1.ToString() + " AND TIPMOV = 'DP'";
                            blnCommit = db1.WriteTransactionData(strSQL9, CommandType.Text);
                            if (blnCommit)
                            {
                                string strSQL10 = "UPDATE DENTES SET IMPDIS = IMPCON + IMPASSCON + IMPADDREC + IMPABB" + " WHERE CODPOS = " + codPos + " AND ANNDEN = " + s1.ToString() + " AND MESDEN = " + s2.ToString() + " AND PRODEN = " + PRODEN1.ToString() + " AND TIPMOV = 'AR'";
                                blnCommit = db1.WriteTransactionData(strSQL10, CommandType.Text);
                            }
                        }
                    }
                    db1.EndTransaction(blnCommit);
                    if (!blnCommit)
                        flag3 = true;
                }
                flag1 = false;
                btnConfermaIsVisible = false;
                return blnCommit;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (flag1)
                    db1.EndTransaction(false);
                throw;
            }
            finally
            {
                if (flag3)
                    btnStampaIsVisible = true;
                if (streamReader != null)
                {
                    streamReader.Close();
                    File.Delete(strPath);
                }
            }
        }

        public static bool WRITE_INSERT_DENDET2(
          DataLayer db,
          List<DenunciaArretrati> listaReport,
          List<ParametriGenerali> listaParametriGenerali,
          TFI.OCM.Utente.Utente utente,
          string CODPOS,
          int ANNDEN,
          int MESDEN,
          int PRODEN,
          int MAT,
          string TIPMOV,
          string DAL,
          string AL,
          string DATERO,
          Decimal IMPRET,
          Decimal IMPOCC,
          Decimal IMPFIG,
          Decimal IMPABB,
          Decimal IMPASSCON,
          Decimal IMPCON,
          Decimal IMPMIN,
          string PREV,
          int PROMOD,
          string DATDEC,
          string DATCES,
          Decimal NUMGGAZI,
          Decimal NUMGGFIG,
          Decimal NUMGGPER,
          Decimal NUMGGDOM,
          Decimal NUMGGSOS,
          Decimal NUMGGCONAZI,
          Decimal IMPSCA,
          Decimal IMPTRAECO,
          string ETA65,
          int TIPRAP,
          string FAP,
          Decimal PERFAP,
          Decimal IMPFAP,
          Decimal PERPAR,
          Decimal PERAPP,
          int PRORAP,
          int CODCON,
          int PROCON,
          string TIPSPE,
          int CODLOC,
          int PROLOC,
          int CODLIV,
          int CODGRUASS,
          int CODQUACON,
          Decimal ALIQUOTA,
          string DATNAS,
          int ANNCOM,
          string TIPDEN,
          ref int intProDenDet)
        {
            int? outcome = new int?();
            int[,] numArray = new int[1, 2];
            Decimal num1 = 0.0M;
            try
            {
                if (listaParametriGenerali == null)
                    listaParametriGenerali = DenunciaMensileDAL.GetParametriGenerali(CODPOS.ToString(), out outcome);
                int? nullable = outcome;
                int num2 = 1;
                if (nullable.GetValueOrDefault() == num2 & nullable.HasValue)
                    throw new Exception();
                num1 = WriteDIPA.GetImportoParametro(listaParametriGenerali, 5, DAL);
                string strSQL1 = " SELECT VALUE(MAX(PRODENDET), 0) + 1  FROM DENDET " + " WHERE CODPOS = " + CODPOS + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() + " AND PRODEN = " + PRODEN.ToString() + " AND MAT = " + MAT.ToString();
                int int32 = Convert.ToInt32(db.Get1ValueFromSQL(strSQL1, CommandType.Text));
                intProDenDet = int32;
                if (PREV == "S")
                {
                    string strSQL2 = "UPDATE MODPREDET SET PRODEN = " + PRODEN.ToString() + ", PRODENDET = " + int32.ToString() + ", IMPRET = " + IMPRET.ToString().Replace(",", ".") + ", IMPOCC = " + IMPOCC.ToString().Replace(",", ".") + ", " + "IMPFIG = " + IMPFIG.ToString().Replace(",", ".") + " WHERE CODPOS = " + CODPOS + " AND " + "ANNDEN = " + ANNDEN.ToString() + " And MESDEN = " + MESDEN.ToString() + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString() + " AND TIPMOV = 'DP' AND PROMOD = (SELECT MAX(PROMOD) FROM MODPREDET WHERE CODPOS = " + CODPOS + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString() + ") AND PRODEN IS NULL AND " + "PRODENDET IS NULL";
                    if (!db.WriteTransactionData(strSQL2, CommandType.Text))
                        throw new Exception();
                }
                string str1 = "INSERT INTO DENDET (" + "CODPOS, " + "ANNDEN, " + "MESDEN, " + "PRODEN, " + "MAT, " + "PRODENDET, " + "TIPMOV, " + "DAL, " + "AL, " + "DATERO, " + "IMPRET, " + "IMPOCC, " + "IMPFIG, " + "IMPABB, " + "IMPASSCON, " + "IMPCON, " + "IMPMIN, " + "PREV, " + "DATDEC, " + "DATCES, " + "NUMGGAZI, " + "NUMGGFIG, " + "NUMGGPER, " + "NUMGGDOM, " + "NUMGGSOS, " + "NUMGGCONAZI, " + "IMPSCA, " + "IMPTRAECO, " + "ETA65 ," + "TIPRAP, " + "FAP ," + "PERFAP ," + "IMPFAP ," + "PERPAR, " + "PERAPP, " + "PRORAP, " + "CODCON, " + "PROCON, " + "TIPSPE, " + "CODLOC, " + "PROLOC, " + "CODLIV, " + "CODGRUASS, " + "CODQUACON, " + "ALIQUOTA, " + "DATNAS, " + "ANNCOM, " + "TIPDEN, " + "ULTAGG, " + "UTEAGG) " + " VALUES (" + CODPOS + ", " + ANNDEN.ToString() + ", " + MESDEN.ToString() + ", " + PRODEN.ToString() + ", " + MAT.ToString() + ", " + int32.ToString() + ", " + DBMethods.DoublePeakForSql(TIPMOV) + ", " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DAL)) + ", " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(AL)) + ", ";
                string str2 = (string.IsNullOrEmpty(DATERO) ? str1 + "Null, " : str1 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATERO)) + ", ") + IMPRET.ToString().Replace(",", ".") + ", " + IMPOCC.ToString().Replace(",", ".") + ", " + IMPFIG.ToString().Replace(",", ".") + ", " + IMPABB.ToString().Replace(",", ".") + ", " + IMPASSCON.ToString().Replace(",", ".") + ", " + "ROUND(" + IMPRET.ToString().Replace(",", ".") + " * " + ALIQUOTA.ToString().Replace(",", ".") + " / 100, 2), " + IMPMIN.ToString().Replace(",", ".") + ", ";
                string str3 = !(PREV != "N") ? str2 + "'N', " : str2 + "'S', ";
                string str4 = string.IsNullOrEmpty(DATDEC) ? str3 + "NULL, " : str3 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATDEC)) + ", ";
                string str5 = (string.IsNullOrEmpty(DATCES) ? str4 + "NULL, " : str4 + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATCES)) + ", ") + NUMGGAZI.ToString().Replace(",", ".") + ", " + NUMGGFIG.ToString().Replace(",", ".") + ", " + NUMGGPER.ToString().Replace(",", ".") + ", " + NUMGGDOM.ToString().Replace(",", ".") + ", " + NUMGGSOS.ToString().Replace(",", ".") + ", " + NUMGGCONAZI.ToString().Replace(",", ".") + ", " + IMPSCA.ToString().Replace(",", ".") + ", " + IMPTRAECO.ToString().Replace(",", ".") + ", " + DBMethods.DoublePeakForSql(ETA65) + ", " + TIPRAP.ToString() + ", " + DBMethods.DoublePeakForSql(FAP) + ", ";
                if (FAP == "S")
                    IMPFAP = IMPRET / 100M * PERFAP;
                string str6 = str5 + PERFAP.ToString().Replace(",", ".") + ", " + IMPFAP.ToString().Replace(",", ".") + ", " + PERPAR.ToString().Replace(",", ".") + ", " + PERAPP.ToString().Replace(",", ".") + ", " + PRORAP.ToString() + ", " + CODCON.ToString() + ", " + PROCON.ToString() + ", " + DBMethods.DoublePeakForSql(TIPSPE) + ", " + CODLOC.ToString() + ", " + PROLOC.ToString() + ", " + CODLIV.ToString() + ", " + CODGRUASS.ToString() + ", " + CODQUACON.ToString() + ", " + ALIQUOTA.ToString().Replace(",", ".") + ", " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DATNAS)) + ", ";
                string str7 = ANNCOM != 0 ? str6 + ANNCOM.ToString() + ", " : str6 + "NULL, ";
                string strSQL3 = (!string.IsNullOrEmpty(TIPDEN) ? str7 + DBMethods.DoublePeakForSql(TIPDEN) + ", " : str7 + "NULL, ") + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(utente.Username) + ")";
                if (!db.WriteTransactionData(strSQL3, CommandType.Text))
                    throw new Exception();
                if (PREV == "S" & PROMOD > 0 & TIPMOV == "DP")
                {
                    string strSQL4 = "SELECT CODSTAPRE FROM MODPRE WHERE CODPOS = " + CODPOS + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString() + " AND PROMOD = " + PROMOD.ToString();
                    if (Convert.ToInt16(db.Get1ValueFromSQL(strSQL4, CommandType.Text)) == (short) 0)
                    {
                        string str8 = "UPDATE MODPREDET SET IMPRET = " + IMPRET.ToString().Replace(",", ".") + ", " + "IMPOCC = " + IMPOCC.ToString().Replace(",", ".") + ", " + "IMPFIG = " + IMPFIG.ToString().Replace(",", ".") + ", " + "IMPABB = " + IMPABB.ToString().Replace(",", ".") + ", " + "IMPASSCON = " + IMPASSCON.ToString().Replace(",", ".") + ", " + "IMPCON = ROUND(" + IMPRET.ToString().Replace(",", ".") + " * " + ALIQUOTA.ToString().Replace(",", ".") + " / 100, 2), " + "IMPMIN = " + IMPMIN.ToString().Replace(",", ".") + ", " + "IMPRETPRV = NULL, IMPOCCPRV = NULL, IMPFIGPRV = NULL, " + "PRODEN = " + PRODEN.ToString() + ", " + "PRODENDET = " + int32.ToString() + " WHERE CODPOS = " + CODPOS + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString() + " AND PROMOD = " + PROMOD.ToString() + " AND DAL = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(DAL)) + " AND AL = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(AL)) + " AND TIPMOV = " + DBMethods.DoublePeakForSql(TIPMOV);
                        string strSQL5 = !(TIPMOV == "AR") ? str8 + " AND ANNDEN = " + ANNDEN.ToString() + " AND MESDEN = " + MESDEN.ToString() : str8 + " AND ANNCOM = " + ANNCOM.ToString();
                        if (!db.WriteTransactionData(strSQL5, CommandType.Text))
                            throw new Exception();
                    }
                }
                return Convert.ToBoolean(int32);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static RicercaArretrati_Data GetArretratoNonConfermatoFromDentesWith(string codPos)
        {
            int intDiAppoggio;
            decimal decimalDiAppoggio;
            DataLayer dataLayer = new DataLayer();
            try
            {
                var codPosParameter = dataLayer.CreateParameter("@codPos", iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codPos);
                var arretratoNonConfermatoQuery = "SELECT * FROM DENTES WHERE CODPOS = @codPos AND STADEN = 'N' AND TIPMOV = 'AR'";
                var arretratoNonConfermatoRow = dataLayer.GetDataTableWithParameters(arretratoNonConfermatoQuery, codPosParameter).Rows[0];

                return new RicercaArretrati_Data
                {
                    DatApe = arretratoNonConfermatoRow.ElementAt("DATAPE"),
                    AnnDen = int.TryParse(arretratoNonConfermatoRow.ElementAt("ANNDEN"), out intDiAppoggio) ? intDiAppoggio : 0,
                    MesDen = int.TryParse(arretratoNonConfermatoRow.ElementAt("MESDEN"), out intDiAppoggio) ? intDiAppoggio : 0,
                    ProDen = int.TryParse(arretratoNonConfermatoRow.ElementAt("PRODEN"), out intDiAppoggio) ? intDiAppoggio : 0,
                    ImpRet = decimal.TryParse(arretratoNonConfermatoRow.ElementAt("IMPRET"), out decimalDiAppoggio) ? decimalDiAppoggio : 0,
                    ImpOcc = decimal.TryParse(arretratoNonConfermatoRow.ElementAt("IMPOCC"), out decimalDiAppoggio) ? decimalDiAppoggio : 0,
                    ImpFig = decimal.TryParse(arretratoNonConfermatoRow.ElementAt("IMPFIG"), out decimalDiAppoggio) ? decimalDiAppoggio : 0,
                    ImpCon = decimal.TryParse(arretratoNonConfermatoRow.ElementAt("IMPCON"), out decimalDiAppoggio) ? decimalDiAppoggio : 0,
                    StaDen = arretratoNonConfermatoRow.ElementAt("STADEN"),
                    DatEroArr = arretratoNonConfermatoRow.ElementAt("DATEROARR"),
                    ImpRetDel = decimal.TryParse(arretratoNonConfermatoRow.ElementAt("IMPRETDEL"), out decimalDiAppoggio) ? decimalDiAppoggio : 0
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
