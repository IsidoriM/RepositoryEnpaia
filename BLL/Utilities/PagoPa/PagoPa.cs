using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using IBM.Data.DB2.iSeries;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using Utilities;

namespace TFI.BLL.Utilities.PagoPa
{
    public partial class PagoPa
    {

        public int Anno { get; }
        public int Mese { get; }
        public int Progressivo { get; }
        public string Iuv { get; set; }
        public string TransId { get; set; }
        public string Posizione { get; }
        public string Arretrato { get; }
        public string SessionUser { get; }
        private DataLayer dataLayer { get;set; } 

        public Dictionary<string, string> PagoPaSection { get; } = new();
        private static readonly ILog log = LogManager.GetLogger("RollingFile");
        private static readonly ILog TrackLog = LogManager.GetLogger("Track");

        public PagoPa(int anno, int mese, int progressivo, string iuv, string transId, string posizione, string arretrato, string sessionUser)
        {
            Anno = anno;
            Mese = mese;
            Progressivo = progressivo;
            Iuv = iuv;
            TransId = transId;
            Posizione = posizione;
            Arretrato = arretrato;
            SessionUser = sessionUser;

            PagoPaSection.Add("Attivo", "1");
            PagoPaSection.Add("LogFile", "/log/Log_PagoPa_xxx.txt");
            PagoPaSection.Add("CreaPagamento", "https://pagopa.enpaia.it/api/creaPagamento");
            PagoPaSection.Add("PathRedirectPagamento", "https://pagopa.enpaia.it/pagopa");
            PagoPaSection.Add("GetPagamento", "https://pagopa.enpaia.it/api/getPagamento");
            PagoPaSection.Add("GetBollettino", "https://pagopa.enpaia.it/api/getBollettino");
            PagoPaSection.Add("CancellaPagamento", "https://pagopa.enpaia.it/api/cancellaPagamento");
            dataLayer = new DataLayer();
        }

        public PagoPa()
        {
            PagoPaSection.Add("PathRedirectPagamento", "https://pagopa.enpaia.it/pagopa");
            PagoPaSection.Add("GetPagamento", "https://pagopa.enpaia.it/api/getPagamento");
        }

        public ObjResponseGet GetPagamento(string iuv, string transId)
        {

            HttpResponseMessage responseGet = GetPagamentoPagoPa(iuv, transId);

            if (responseGet.IsSuccessStatusCode)
            {
                string dataObjects = responseGet.Content.ReadAsStringAsync().Result;
                var resultObject = (ObjResponseGet)JsonConvert.DeserializeObject(dataObjects, typeof(ObjResponseGet));
                if (resultObject != null)
                    resultObject.Data.TransActionId = transId;
                return resultObject;
            }
            else
            {
                ObjResponseGet response = new ObjResponseGet(esito: responseGet.StatusCode.ToString(),
                    message: $"PagoPA: chiamata al servizio PagoPa.GetPagamento NON andato a buon fine! {responseGet.ReasonPhrase}",
                    null);
                return response;
            }
        }


        private HttpResponseMessage GetPagamentoPagoPa(string iuv, string transId)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client.GetAsync(PagoPaSection["GetPagamento"] + $"?IuvCode={iuv}&TransActionId={transId}").Result;
        }

        private bool ScriviTbPagoPA(string tipoDenuncia, InfoPagamento infoPagamento, AziendaPagoPa aziendaPagoPa, iDB2Transaction transObject)
        {
            if (aziendaPagoPa == null)
            {
                return false;
            }
            string STRSQL = string.Empty;
            try
            {
                STRSQL = "SELECT COUNT(*) FROM TBPAGOPA WHERE PAGPA_POS = " + Posizione + " AND PAGPA_ANN = " + Anno + " AND PAGPA_MES = " + Mese + " AND PAGPA_PROG = " + Progressivo;
                if (dataLayer.Get1ValueFromSQL(STRSQL, CommandType.Text) == "0")
                {
                    if (tipoDenuncia == "Diffida")
                    {
                        var query = $"SELECT CODPOS FROM TBPAGOPA_DIFF WHERE CODPOS = {Posizione}";
                        var codpos = dataLayer.Get1ValueFromSQL(query, CommandType.Text);
                        if (codpos != default) return default;

                        STRSQL = "INSERT INTO TBPAGOPA_DIFF (CODPOS, ANNDEN, MESDEN, RAGSOC, IND, DENLOC, CAP, SIGPRO, EMAIL, CAUSALE, DATSCA, IMPORTO, CODLINE, ULTAGG, UTEAGG, IUV, TRANSID) ";
                        STRSQL += "  VALUES (" + Posizione.PadLeft(6, '0') + ", " + Anno + ", " + Mese + ", '" + aziendaPagoPa.RagioneSociale.Replace("'", "''") + "', ";
                        STRSQL += "'" + aziendaPagoPa.Indirizzo.Replace("'", "''") + "', '";
                        STRSQL += "'" + aziendaPagoPa.Localita.Replace("'", "''").Trim() + "', '" + aziendaPagoPa.Cap.Trim() + "', '" + aziendaPagoPa.Prov.Trim() + "', '";
                        STRSQL += "'" + aziendaPagoPa.Mail.Replace("'", "''").Trim() + "', '" + infoPagamento.Causale.Replace("'", "''") + "', ";
                        STRSQL += "'" + infoPagamento.DataScadenza.ToString("yyyy-MM-dd").Trim() + "', '" + Convert.ToString(infoPagamento.Importo).Replace(",", ".") + "', ";
                        STRSQL += "'" + infoPagamento.CodeLine.Trim().Replace(" ", "") + "', ";
                        STRSQL += " current_timestamp, '" + SessionUser.Trim() + "', '" + Iuv.Trim() + "', '" + TransId.Trim() + "')";
                    }
                    else
                    {
                        var query = $"SELECT PAGPA_POS FROM TBPAGOPA WHERE PAGPA_POS = {Posizione}";
                        var codpos = dataLayer.Get1ValueFromSQL(query, CommandType.Text);
                        if (codpos == default) return default;
                        STRSQL = "INSERT INTO TBPAGOPA (PAGPA_POS, PAGPA_ANN, PAGPA_MES, PAGPA_PROG, PAGPA_COD, PAGPA_RAG_SOC, PAGPA_IND, PAGPA_LOC, PAGPA_CAP, PAGPA_PROV, ";
                        STRSQL += " PAGPA_EMAIL, PAGPA_CAUS, PAGPA_DAT_SCA, PAGPA_IMP, PAGPA_ULT_AGG, PAGPA_UTE_AGG, PAGPA_CAU, PAGPA_PRO_ACC, PAGPA_IUV, PAGPA_TRANSID) ";
                        STRSQL += " VALUES (" + Posizione + ", " + Anno + ", " + Mese + ", " + Progressivo + ", '" + Posizione.PadLeft(6, '0') + "', ";
                        STRSQL += "'" + aziendaPagoPa.RagioneSociale.Replace("'", "''") + "', '" + aziendaPagoPa.Indirizzo.Replace("'", "''") + ", " + aziendaPagoPa.Civ + "', ";
                        STRSQL += "'" + aziendaPagoPa.Localita.Replace("'", "''") + "', '" + aziendaPagoPa.Cap + "', '" + aziendaPagoPa.Prov + "', ";
                        STRSQL += "'" + aziendaPagoPa.Mail.Replace("'", "''") + "', '" + infoPagamento.Causale.Replace("'", "''") + "', '" + infoPagamento.DataScadenza.ToString("yyyy-MM-dd") + "', ";
                        STRSQL += "'" + Convert.ToString(infoPagamento.Importo).Replace(",", ".") + "', current_timestamp, '" + SessionUser.Trim() + "', '" + tipoDenuncia + "', ";
                        STRSQL += Progressivo + ", '" + Iuv.Trim() + "', '" + TransId.Trim() + "')";
                    }
                }

                var isSucceeded = dataLayer.WriteTransactionData(STRSQL, CommandType.Text);
                if (isSucceeded) return true;
                transObject.Rollback();
                return false;
            }
            catch (Exception ex)
            {
                transObject.Rollback();
                return false;
            }
        }


        private AziendaPagoPa? GetAziendaByPosizione()
        {
            DataLayer dataLayer = new DataLayer();
            string strSql = "SELECT RAGSOC,  CODFIS, PARIVA FROM AZI WHERE CODPOS = " + Posizione;
            DataTable azienda = dataLayer.GetDataTable(strSql);

            strSql = "SELECT A.TIPIND, A.CODDUG, DUG.DENDUG, B.DENIND, CHAR(B.ULTAGG) AS ULTAGG, A.DENSTAEST, A.IND, ";
            strSql += "A.NUMCIV, A.DENLOC, A.CAP, A.SIGPRO, A.TEL1, A.TEL2, A.FAX, '' as EMAIL FROM INDSED A INNER JOIN ";  // , A.EMAIL la colonna nn esiste
            strSql += "TIPIND B ON A.TIPIND = B.TIPIND LEFT JOIN DUG ON A.CODDUG = DUG.CODDUG INNER JOIN (SELECT ";
            strSql += "TIPIND, MAX(DATCOM) AS DATCOM FROM INDSED WHERE CODPOS = " + Posizione + " AND ";
            strSql += "current_date BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31') AND TIPIND = 1 GROUP BY TIPIND) ";
            strSql += "AS C ON A.TIPIND = C.TIPIND AND A.DATCOM = C.DATCOM WHERE A.CODPOS = " + Posizione;

            DataTable aziendaAltriDati = dataLayer.GetDataTable(strSql);
            string emailAzienda = dataLayer.Get1ValueFromSQL("select email from azemail where codpos='" + Posizione + "' order by DATINI desc limit 1", CommandType.Text);
            aziendaAltriDati.Rows[0]["Email"] = emailAzienda;
            var aziendaPagoPa = AziendaPagoPa.Create(azienda, aziendaAltriDati);

            return aziendaPagoPa;

        }


        private InfoPagamento? GetInfoPagamento(string tipoDenuncia)
        {
            DataLayer dataLayer1 = new DataLayer();
            string strSQL;
            DataTable objDt;

            if ((tipoDenuncia == "DP" | tipoDenuncia == "AR"))
            {
                // Recuperiamo l'importo per le retribuzioni
                // -------------------------------------------------------
                strSQL = "SELECT DATSCA, IMPDEC, IMPCON, IMPADDREC, IMPABB, IMPASSCON, VALUE(IMPSANDET, 0) AS IMPSANDET, ";
                strSQL += "VALUE(IMPDEC, 0) AS IMPDEC, VALUE(ESIRET, 'N') AS ESIRET, IMPCONDEL, IMPADDRECDEL, IMPABBDEL, ";
                strSQL += "IMPASSCONDEL, VALUE(IMPSANRET, 0) AS IMPSANRET, TIPMOV, NUMSAN, NUMSANANN, NUMMOV FROM DENTES WHERE CODPOS = ";
                strSQL += Posizione + " AND ANNDEN = " + Anno + " AND MESDEN = " + Mese + " AND PRODEN = " + Progressivo;

                objDt = dataLayer1.GetDataTable(strSQL);
                var infoPagamento = InfoPagamento.Create(objDt, tipoDenuncia, Posizione, Anno, Mese, Progressivo);
                return infoPagamento;
            }
            else if ((tipoDenuncia == "Diffida"))
            {
                strSQL = "SELECT annodiff, mesediff, impdiff, numprot FROM CONAZEC91f WHERE CODPOSIZ = " + Posizione + " AND annodiff = " + Anno + " AND mesediff = " + Mese;
                objDt = dataLayer1.GetDataTable(strSQL);
                var infoPagamento = InfoPagamento.CreateDiffida(objDt, tipoDenuncia, Posizione, Anno, Mese, Progressivo);
                return infoPagamento;
            }
            else
                return null;
        }

        public ObjResponseCrea CreaPagamento(string urlCallBack, string tipoDenuncia)
        {
            ObjResponseCrea objResponseCreaResponse = new ObjResponseCrea();
            string URL = PagoPaSection["CreaPagamento"];
          

            List<string> log = new List<string>();
            try
            {
                InfoPagamento infoPagamento = GetInfoPagamento(tipoDenuncia);

                if ((infoPagamento.Importo > 0))
                {
                    AziendaPagoPa struct_DatiAzienda = GetAziendaByPosizione();

                    if (!verificaEmail(struct_DatiAzienda.Mail))
                    {
                        return new ObjResponseCrea("-1", $@"Attenzione, l\'indirizzo email {struct_DatiAzienda.Mail} non è valido! Inserire un indirizzo email valido nella sezione anagrafica e riprovare.", null);
                    }

                    dataLayer.StartTransaction();
                    var transObject = dataLayer.objTransaction;
                    try
                    {
                        // ''''''''''''''''''INSERISCO SU TBPAGOPA''''''''''''''''''''''''''''''''''''
                        // '''''''''''''''''APERTURA DI UNA TRANSAZIONE'''''''''''''''''''''''''''''''
                        if (ScriviTbPagoPA(tipoDenuncia, infoPagamento, struct_DatiAzienda, transObject) == false)
                            throw new InvalidOperationException("PagamentoPagoPa Errore durante il salvataggio della tabella PagoPA");
                        // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                        HttpClient client = new HttpClient();
                        HttpResponseMessage response = new HttpResponseMessage();
                        DataTable objDt = new DataTable();
                        string STRSQL;
                        STRSQL = "SELECT * FROM PAGOPANET.TB_PAGAMENTI WHERE CODICEDEBITO = " + DBMethods.DoublePeakForSql(infoPagamento.CodiceDebito.Trim().ToUpper()) + " AND DATACANCELLAZIONE IS NULL";
                        objDt = dataLayer.GetDataTable(STRSQL);

                        if (objDt.Rows.Count > 0)
                        {
                            Iuv = objDt.Rows[0]["IUV"].ToString().Trim().ToUpper();
                            TransId = objDt.Rows[0]["TRANSACTIONID"].ToString().Trim().ToUpper();
                        }
                        else
                        {
                            response = CreaPagamentoPagoPa(infoPagamento, struct_DatiAzienda, urlCallBack, out client);

                            if (!response.IsSuccessStatusCode)
                            {
                                string message = string.Empty;
                                message = $"PagoPA: chiamata al servizio PagoPa.enpaia NON andato a buon fine! {response.ReasonPhrase}.";
                                if ((struct_DatiAzienda.Mail == ""))
                                    message += $@" Indirizzo email mancante per l\'azienda {struct_DatiAzienda.RagioneSociale.Replace("'", @"\'")}.";
                                transObject.Rollback();
                                return new ObjResponseCrea(response.StatusCode.ToString(), message, null);
                            }
                            else
                            {
                                objResponseCreaResponse = JsonConvert.DeserializeObject<ObjResponseCrea>(response.Content.ReadAsStringAsync().Result);

                                switch (objResponseCreaResponse.Esito)
                                {
                                    case "OK":
                                        {
                                            Iuv = objResponseCreaResponse.Data.IuvCode.Trim();
                                            TransId = objResponseCreaResponse.Data.TransActionId.Trim();

                                            PagoPa.log.Info(DateTime.Now.ToString() + "| PagamentoPagoPa Creazione posizione debitoria su PagoPa avvenuta con successo. {IUV: " + Iuv + "- TRANSID: " + TransId + " - CODPOS: " + Posizione + " - ANNDEN: " + Anno + " - MESDEN: " + Mese + " - TIPOLOGIA: " + tipoDenuncia);
                                            break;
                                        }

                                    case "KO":
                                        {
                                            transObject.Rollback();
                                            PagoPa.log.Info(DateTime.Now.ToString() + "| PagamentoPagoPa Creazione posizione debitoria su PagoPa non andata a buon fine. {CODPOS: " + Posizione + " - ANNDEN: " + Anno + " - MESDEN: " + Mese + " - TIPOLOGIA: " + tipoDenuncia +"}");
                                            objResponseCreaResponse = new ObjResponseCrea(response.StatusCode.ToString(), $"PagoPA: chiamata al servizio PagoPa.enpaia NON andato a buon fine! {response.ReasonPhrase}", null);
                                            return objResponseCreaResponse;
                                        }
                                }
                            }
                        }

                        client.Dispose();
                        log.Clear();
                    }
                    catch (Exception ex)
                    {
                        PagoPa.log.Info(DateTime.Now.ToString() + "| PagamentoPagoPa Errore durante l\'apertura della posizione debitoria su PagoPa. {CODPOS: " + Posizione + " - ANNDEN: " + Anno + " - MESDEN: " + Mese + " - TIPOLOGIA: " + tipoDenuncia + " - ERRORE:" + ex.Message + "}");
                        throw new InvalidOperationException(@"PagamentoPagoPa Errore durante l\'apertura della posizione debitoria su PagoPa: " + ex.Message);
                    }

                    if (tipoDenuncia != "Diffida")
                    {
                        try
                        {
                            // '''''''''''APRO TRANSAZIONE
                            if (AggiornaEstremiPagoPa(int.Parse(Posizione), Anno, Mese, Progressivo, 7, infoPagamento.Importo, DateTime.Today.GetDateTimeFormats(new CultureInfo("it-IT", false).DateTimeFormat)[0], infoPagamento.CodeLine, Posizione, Iuv, TransId, transObject))
                            {
                                // '''''''''''COMMIT TRANSAZIONE
                                PagoPa.log.Info(DateTime.Now.ToString() + "| PagamentoPagoPa Salvataggio del PagoPa generato avvenuto con successo. {IUV: " + Iuv + " - TRANSID: " + TransId + " - CODPOS: " + Posizione + " - ANNDEN: " + Anno + " - MESDEN: " + Mese + " - TIPOLOGIA: " + tipoDenuncia +"}");
                            }
                            else
                            {
                                // '''''''''''ROLLBACK TRANSAZIONE
                                PagoPa.log.Info(DateTime.Now.ToString() + "| PagamentoPagoPa Errore durante il salvataggio del PagoPa generato. {IUV: " + Iuv + " - TRANSID: " + TransId + " - CODPOS: " + Posizione + " - ANNDEN: " + Anno + " - MESDEN: " + Mese + " - TIPOLOGIA: " + tipoDenuncia + "}");
                                throw new InvalidOperationException("PagamentoPagoPa Errore durante il salvataggio del PagoPa generato. {IUV: " + Iuv + " - TRANSID: " + TransId + " - CODPOS: " + Posizione + " - ANNDEN: " + Anno + " - MESDEN: " + Mese + " - TIPOLOGIA: " + tipoDenuncia + "}");
                            }
                        }
                        catch (Exception ex)
                        {
                            // '''''''''''ROLLBACK TRANSAZIONE
                            PagoPa.log.Info(DateTime.Now.ToString() + "| PagamentoPagoPa Errore durante il salvataggio del PagoPa generato. {IUV: " + Iuv + " - TRANSID: " + TransId + " - CODPOS: " + Posizione + " - ANNDEN: " + Anno + " - MESDEN: " + Mese + " - TIPOLOGIA: " + tipoDenuncia + "}");
                            throw new InvalidOperationException("PagamentoPagoPa Errore durante il salvataggio del PagoPa generato: " + Iuv + "-" + ex.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            // '''''''''''APRO TRANSAZIONE
                            if (AggiornaEstremiPagoPaDiffida(int.Parse(Posizione), Anno, Mese, Progressivo, 7, infoPagamento.Importo, DateTime.Today.GetDateTimeFormats(new CultureInfo("it-IT", false).DateTimeFormat)[0], infoPagamento.CodeLine, Posizione, Iuv, TransId, transObject))
                            {
                                // '''''''''''COMMIT TRANSAZIONE
                                PagoPa.log.Info(DateTime.Now.ToString() + "| PagamentoPagoPa Salvataggio del PagoPa generato avvenuto con successo. {IUV: " + Iuv + " - TRANSID: " + TransId + " - CODPOS: " + Posizione + " - ANNDEN: " + Anno + " - MESDEN: " + Mese + " - TIPOLOGIA: " + tipoDenuncia +"}");
                            }
                            else
                            {
                                // '''''''''''ROLLBACK TRANSAZIONE
                                PagoPa.log.Info(DateTime.Now.ToString() + "| PagamentoPagoPa Errore durante il salvataggio del PagoPa generato. {IUV: " + Iuv + " - TRANSID: " + TransId + " - CODPOS: " + Posizione + " - ANNDEN: " + Anno + " - MESDEN: " + Mese + " - TIPOLOGIA: " + tipoDenuncia + "}");
                                throw new InvalidOperationException("PagamentoPagoPa Errore durante il salvataggio del PagoPa generato. {IUV: " + Iuv + " - TRANSID: " + TransId + " - CODPOS: " + Posizione + " - ANNDEN: " + Anno + " - MESDEN: " + Mese + " - TIPOLOGIA: " + tipoDenuncia + "}");
                            }
                        }
                        catch (Exception ex)
                        {
                            // '''''''''''ROLLBACK TRANSAZIONE
                            PagoPa.log.Info(DateTime.Now.ToString() + "| PagamentoPagoPa Errore durante il salvataggio del PagoPa generato. {IUV: " + Iuv + " - TRANSID: " + TransId + " - CODPOS: " + Posizione + " - ANNDEN: " + Anno + " - MESDEN: " + Mese + " - TIPOLOGIA: " + tipoDenuncia + "}");
                            throw new InvalidOperationException("PagamentoPagoPa Errore durante il salvataggio del PagoPa generato: " + Iuv + "-" + ex.Message);
                        }
                    }
                    transObject.Commit();
                    return objResponseCreaResponse;
                }
                else
                {
                    PagoPa.log.Info(DateTime.Now.ToString() + "|  PagamentoPagoPa: importo zero del pagamento, operazione annullata. {CODPOS: " + Posizione + " - ANNDEN: " + Anno + " - MESDEN: " + Mese + " - TIPOLOGIA: " + tipoDenuncia + "}");
                    throw new InvalidOperationException("PagamentoPagoPa: importo zero del pagamento, operazione annullata");
                }
            }
            catch (Exception ex)
            {
                PagoPa.log.Info(DateTime.Now.ToString() + "| PagamentoPagoPa Errore durante la creazione del PagoPa generato.  {CODPOS: " + Posizione + " - ANNDEN: " + Anno + " - MESDEN: " + Mese + " - TIPOLOGIA: " + tipoDenuncia + "}");
                throw new InvalidOperationException("PagamentoPagoPa Errore durante la creazione del PagoPa generato: " + ex.Message);
            }
        }
        private bool verificaEmail(string email)
        {
            return Regex.IsMatch(email, @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))\z");
        }

        private bool AggiornaEstremiPagoPa(Int32 CODPOS, Int32 ANNDEN, Int32 MESDEN, Int32 PRODEN, Int32 CODMODPAG, decimal IMPVER, string DATVER, string CODLINE, string UTEAGG, string IUV, string TRANSID, iDB2Transaction transObject)
        {
            try
            {
                string queryDentes;
            
                queryDentes = "UPDATE DENTES SET";
                queryDentes += " CODMODPAG = " + CODMODPAG + ", ";
                queryDentes += " IMPVER = " + IMPVER.ToString().Replace(",", ".") + ", ";
                queryDentes += " DATVER = " + DBMethods.DoublePeakForSql(Module_DB2Date(DATVER)) + ", ";
                queryDentes += " CODLINE = " + DBMethods.DoublePeakForSql(CODLINE) + ", ";
                queryDentes += " ULTAGG = CURRENT_TIMESTAMP, ";
                queryDentes += " UTEAGG = " + DBMethods.DoublePeakForSql(UTEAGG) + ", ";
                queryDentes += " IUV = " + DBMethods.DoublePeakForSql(IUV) + ", ";
                queryDentes += " TRANSID = " + DBMethods.DoublePeakForSql(TRANSID);
                queryDentes += " WHERE CODPOS = " + CODPOS;
                queryDentes += " AND ANNDEN = " + ANNDEN;
                queryDentes += " AND MESDEN = " + MESDEN;
                queryDentes += " AND PRODEN = " + PRODEN;
                // '''''''AGGIORNAMENTO IN TABELLA
                var isSucceededDentes = dataLayer.WriteTransactionData(queryDentes, CommandType.Text);
                if (!isSucceededDentes)
                {
                    transObject.Rollback();
                    return false;
                } 

                string queryTbPagopa;

                queryTbPagopa = "UPDATE TBPAGOPA SET  PAGPA_IUV = " + DBMethods.DoublePeakForSql(IUV) + ", ";
                queryTbPagopa += " PAGPA_TRANSID = " + DBMethods.DoublePeakForSql(TRANSID);
                queryTbPagopa += " WHERE PAGPA_POS = " + CODPOS;
                queryTbPagopa += " AND PAGPA_ANN = " + ANNDEN;
                queryTbPagopa += " AND PAGPA_MES = " + MESDEN;
                queryTbPagopa += " AND PAGPA_PROG = " + PRODEN;
                // '''''''AGGIORNAMENTO IN TABELLA
                var isSucceededTbPagoPa = dataLayer.WriteTransactionData(queryTbPagopa, CommandType.Text);
                if(isSucceededTbPagoPa) return true;
                transObject.Rollback();
                return false;
            }
            catch (Exception ex)
            {
                transObject.Rollback();
                return false;
            }
        }
        private bool AggiornaEstremiPagoPaDiffida(Int32 CODPOS, Int32 ANNDEN, Int32 MESDEN, Int32 PRODEN, Int32 CODMODPAG, decimal IMPVER, string DATVER, string CODLINE, string UTEAGG, string IUV, string TRANSID, iDB2Transaction transObject)
        {
            string strSQL;

            try
            {
                strSQL = "UPDATE TBPAGOPA SET  PAGPA_IUV = " + DBMethods.DoublePeakForSql(IUV) + ", ";
                strSQL += " PAGPA_TRANSID = " + DBMethods.DoublePeakForSql(TRANSID);
                strSQL += " WHERE PAGPA_POS = " + CODPOS;
                strSQL += " AND PAGPA_ANN = " + ANNDEN;
                strSQL += " AND PAGPA_MES = " + MESDEN;
                strSQL += " AND PAGPA_PROG = " + PRODEN;
                // '''''''AGGIORNAMENTO IN TABELLA
                var isSucceeded = dataLayer.WriteTransactionData(strSQL, CommandType.Text);
                if (isSucceeded) return true;
                
                transObject.Rollback();
                return false;
            }
            catch (Exception ex)
            {
                transObject.Rollback();
                return false;
            }
        }

        private string Module_DB2Date(string strDate)
        {
            DateTime dtDate;
            dtDate = DateTime.Parse(strDate);
            return dtDate.Year.ToString() + "-" + dtDate.Month.ToString().PadLeft(2, '0') + "-" + dtDate.Day.ToString().PadLeft(2, '0');
        }

        private HttpResponseMessage CreaPagamentoPagoPa(InfoPagamento infoPagamento, AziendaPagoPa infoAzienda, string returnUrl,
            out HttpClient client)
        {
            HttpResponseMessage response;
            string azienda, indirizzo;

            if (infoAzienda.RagioneSociale != "")
            {
                if (infoAzienda.RagioneSociale.Length > 33)
                    azienda = infoAzienda.RagioneSociale.Substring(0, 32);
                else
                    azienda = infoAzienda.RagioneSociale;
            }
            else
                azienda = "--";

            if (infoAzienda.Indirizzo != "")
            {
                if (infoAzienda.Indirizzo.Length > 69)
                    indirizzo = infoAzienda.Indirizzo.Substring(0, 68);
                else
                    indirizzo = infoAzienda.Indirizzo;
            }
            else
                indirizzo = "--";

            CreaPagamentoParam parametri = CreaPagamentoParam.Create(
                idServizio: infoPagamento.IdServizio,
                dataRimozione: infoPagamento.DataRimozione.ToString("dd/MM/yyyy"),
                dataScadenza: infoPagamento.DataScadenza.ToString("dd/MM/yyyy"),
                codiceDebito: string.IsNullOrEmpty(infoPagamento.CodiceDebito) == true ? "--" : infoPagamento.CodiceDebito,
                tipoPagatore: "G",
                cfPivaPagatore: string.IsNullOrEmpty(infoAzienda.PIVA) == true ? "--" : infoAzienda.PIVA,
                nomePagatore: azienda,
                indirizzoPagatore: indirizzo,
                civPagatore: string.IsNullOrEmpty(infoAzienda.Civ) == true ? "--" : infoAzienda.Civ,
                capPagatore: string.IsNullOrEmpty(infoAzienda.Cap) == true ? "--" : infoAzienda.Cap,
                localitaPagatore: string.IsNullOrEmpty(infoAzienda.Localita) == true ? "--" : infoAzienda.Localita,
                provinciaPagatore: string.IsNullOrEmpty(infoAzienda.Prov) == true ? "--" : infoAzienda.Prov,
                nazionePagatore: string.IsNullOrEmpty(infoAzienda.Nazione) == true ? "--" : infoAzienda.Nazione,
                emailPagatore: string.IsNullOrEmpty(infoAzienda.Mail) == true ? "fabio.vecchietti@enpaia.it" : infoAzienda.Mail,
                importo: string.Format("{0:0.00}", infoPagamento.Importo).Replace(",", "."),
                causaleVersamento: string.IsNullOrEmpty(infoPagamento.Causale) == true ? "--" : infoPagamento.Causale,
                descrizioneCausaleVersamento: string.IsNullOrEmpty(infoPagamento.Causale) == true ? "--" : infoPagamento.Causale,
                utenteInserimento: SessionUser,
                returnUrl: returnUrl
            );
            string json = JsonConvert.SerializeObject(parametri);
            client = new HttpClient();
            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            client.BaseAddress = new Uri(PagoPaSection["CreaPagamento"]);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            response = client.PostAsync(client.BaseAddress, httpContent).Result;

            return response;
        }

        public HttpResponseMessage GetBollettinoPagoPa(string iuv, string transId)
        {
            HttpClient client = new HttpClient();
            var url = PagoPaSection["GetBollettino"] + $"?IuvCode={iuv}&TransactionId={transId}";
            var result = client.GetAsync(url).Result;
            return result;
        }

        public ObjResponseGetBollettino GetBollettino()
        {
            HttpResponseMessage responseGet = GetBollettinoPagoPa(Iuv, TransId);
            ObjResponseGetBollettino dataObjects = new ObjResponseGetBollettino();

            if (responseGet.IsSuccessStatusCode)
            {
                dataObjects = JsonConvert.DeserializeObject<ObjResponseGetBollettino>(responseGet.Content.ReadAsStringAsync().Result);

                switch (dataObjects.Esito)
                {
                    case "OK":
                        {
                            return dataObjects;
                        }

                    case "KO":
                        {
                            dataObjects.Esito = responseGet.StatusCode.ToString();
                            dataObjects.Message = $"PagoPA: chiamata al servizio PagoPa.enpaia NON andato a buon fine! {responseGet.ReasonPhrase}";
                            return dataObjects;
                        }

                    case "ERR":
                    case "ALT":
                        {
                            dataObjects.Esito = responseGet.StatusCode.ToString();
                            dataObjects.Message = $"PagoPA: chiamata al servizio PagoPa.enpaia NON andato a buon fine! {responseGet.ReasonPhrase}";
                            return dataObjects;
                        }

                    case null/* TODO Change to default(_) if this is not a reference type */
                   :
                        {
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
            }
            else
            {
                dataObjects.Esito = responseGet.StatusCode.ToString();
                dataObjects.Message = $"PagoPA: chiamata al servizio PagoPa.GetPagamento NON andato a buon fine! {responseGet.ReasonPhrase}";
            }

            return dataObjects;
        }
        
        public (string iuvCod, string transaActionId) GetDettagliPagoPa(int anno, int mese, int proDen, string codPos)
        {
            DataLayer dl = new DataLayer();
            var strSql = $"SELECT * FROM TBPAGOPA WHERE PAGPA_ANN = {anno} AND PAGPA_MES = {mese} AND PAGPA_PROG = {proDen} AND PAGPA_POS = {codPos}";
            var dt = dl.GetDataTable(strSql);
            if (dt.Rows.Count == 0) return (string.Empty, string.Empty);
            return (dt.Rows[0].ElementAt("PAGPA_IUV"), dt.Rows[0].ElementAt("PAGPA_TRANSID"));
        }

    }
}
