// Decompiled with JetBrains decompiler
// Type: TFI.DAL.AziendaConsulente.RapportiLavoroDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using Itenso.TimePeriod;
using OCM.TFI.OCM.AziendaConsulente;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using log4net;
using Newtonsoft.Json;
using TFI.CRYPTO.Crypto;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.AziendaConsulente;
using Utilities;
using OCM.TFI.OCM.Utilities;

namespace TFI.DAL.AziendaConsulente
{
    public class RapportiLavoroDAL
    {
        private DataLayer objDataAccess = new DataLayer();
        private static readonly ILog _log = LogManager.GetLogger("RollingFile");

        public RDLPaginatiModel GetRapportiFiltrato(string codpos, string filtro, TipoRDL tipoRdl, int? page, int? pageSize, OrderByType orderBy, bool orderDesc)
        {
            iDB2Parameter codposParam = objDataAccess.CreateParameter("@codpos", iDB2DbType.iDB2Decimal, 8, ParameterDirection.ReturnValue, codpos);
            iDB2Parameter filtroParam = objDataAccess.CreateParameter("@filtro", iDB2DbType.iDB2VarChar, 1, ParameterDirection.ReturnValue, "");
            string querySelectRDL = "SELECT A.MAT, A.COG, A.NOM, A.CODFIS, TRANSLATE(CHAR(B.DATDEC,EUR),'/','.') AS DATDEC, B.DATDEC AS DATISCR, B.PRORAP, B.DATCES, B.DATPRE ";
            string queryCountRDL = "SELECT COUNT(*) AS TOTALRECORD ";
            string queryRDL = " FROM ISCT A INNER JOIN RAPLAV B ON A.MAT = B.MAT WHERE B.CODPOS = @codpos ";
            if (tipoRdl == TipoRDL.RDLAttivo)
            {
                queryRDL += "AND B.DATCES IS NULL ";
            }

            if (tipoRdl == TipoRDL.RDLCessato)
            {
                queryRDL += "AND B.DATCES IS NOT NULL ";
            }

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                filtroParam = objDataAccess.CreateParameter("@filtro", iDB2DbType.iDB2VarChar, filtro.Length + 2, ParameterDirection.ReturnValue, "%" + filtro.ToUpper() + "%");
                queryRDL += "AND (A.COG LIKE @filtro OR A.NOM LIKE @filtro OR A.CODFIS LIKE @filtro OR A.MAT LIKE @filtro) ";
            }
            DataTable totalCountTbl = objDataAccess.GetDataTableWithParameters(queryCountRDL + queryRDL, codposParam, filtroParam, filtroParam, filtroParam, filtroParam);

            queryRDL += "ORDER BY " + ChooseOrder(orderBy) + $" {(orderDesc ? "DESC" : "ASC")}";

            queryRDL += $" OFFSET {(page - 1)* (pageSize ?? 10)} ROWS FETCH NEXT {pageSize ?? 10} ROWS ONLY";

            var rdl = GetRapportiDiLavoro(querySelectRDL + queryRDL, filtroParam, codposParam, tipoRdl);
            rdl.PageNumber = page;
            rdl.TotalCount = int.Parse(totalCountTbl.Rows[0]["TOTALRECORD"].ToString());
            rdl.PageSize = pageSize;
            rdl.TotalPage = (rdl.TotalCount / (pageSize ?? 10)) + (rdl.TotalCount % (pageSize ?? 10) == 0 ? 0 : 1);
            rdl.OrderDesc = orderDesc;
            rdl.OrderBy = orderBy;
            return rdl;
        }

        public RapportiLavoro GetSchedaIscritto(
          string matricola,
          string prorap,
          string codpos)
        {
            var Err = "";

            var schedaIscritto = new RapportiLavoro();
            schedaIscritto.modificaIscritto = new List<NuovoIscritto>();

            var matParam = objDataAccess.CreateParameter("@mat", iDB2DbType.iDB2Decimal, 11, ParameterDirection.Input, matricola);
            var prorapParam = objDataAccess.CreateParameter("@prorap", iDB2DbType.iDB2Decimal, 3, ParameterDirection.Input, prorap);

            var getFromIsctSqlQuery = "SELECT MAT, COG, NOM, DATNAS, CURRENT_DATE AS TODAY, " +
                                      "CODCOM AS CODCOMNAS, TITSTU, (SELECT DENCOM FROM CODCOM WHERE CODCOM.CODCOM = VALUE(ISCT.CODCOM,'@@@@@')) AS DENCOMNAS, " +
                                      "(SELECT SIGPRO FROM CODCOM WHERE CODCOM.CODCOM = VALUE(ISCT.CODCOM,'@@@@@')) AS SIGPRONAS, " + " CODFIS, SES, DATCHIISC, COUCHIISC, STACIV, " +
                                      "(SELECT DENSTACIV FROM STACIV WHERE STACIV.CODSTACIV = VALUE(ISCT.STACIV ,0)) AS DENSTACIV, " +
                                      "TITSTU, (SELECT DENTITSTU FROM TITSTU WHERE TITSTU.CODTITSTU = VALUE(ISCT.TITSTU ,0)) AS DENTITSTU, ULTAGG, UTEAGG FROM ISCT WHERE MAT = @mat";
            var getFromIsct = this.objDataAccess.GetDataTableWithParameters(getFromIsctSqlQuery, matParam);


            if (getFromIsct.Rows.Count > 0)
                FillAnagraficaIsct(prorap, schedaIscritto, getFromIsct);

            var getNatgiuFromAziSqlQuery = "SELECT NATGIU FROM AZI WHERE CODPOS = " + codpos;
            HttpContext.Current.Session["GruCon"] = "N";
            var natgiu = Convert.ToInt16("0" + this.objDataAccess.Get1ValueFromSQL(getNatgiuFromAziSqlQuery, CommandType.Text));
            HttpContext.Current.Items["IsAziSomm"] = natgiu == 27;
            if (natgiu == 10)
                HttpContext.Current.Session["GruCon"] = "S";

            var lastDatini = objDataAccess.GetDataTableWithParameters("SELECT MAX(datini) AS DATINI FROM Iscd WHERE MAT = @mat", matParam).Rows[0].DateElementAt("DATINI");
            var isctForLastDatini = objDataAccess.GetDataTableWithParameters("SELECT * FROM Iscd WHERE DATINI ='" + lastDatini + "' AND MAT = @mat", matParam);

            if (isctForLastDatini.Rows.Count > 0)
            {
                var coddug = Convert.ToInt32(isctForLastDatini.Rows[0]["CODDUG"]);
                FillIndirizzoAndContacts(matParam, schedaIscritto);

                var blnStordlCorrente = true;
                var strDataMinCon = DateTime.Now.ToString("yyyy-MM-dd");

                if (!string.IsNullOrEmpty(lastDatini))
                {
                    blnStordlCorrente = false;
                    strDataMinCon = "";
                }

                //var datiCompletiStordl = GetDatiCompleti_STORDL(Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), lastDatini, strDataMinCon, blnStordlCorrente: blnStordlCorrente);
                var getFromStoRdl = this.objDataAccess.GetDataTableWithParameters(
                    "SELECT DISTINCT A.*, value(A.CODLOC,0) as codloc, " +
                          "TRIM(B.DENLIV) AS LIVELLO, TRIM(E.DENTIPRAP) AS TIPRAP, A.TIPRAP AS CODTIPRAP, " +
                          "(SELECT DATDEC FROM RAPLAV WHERE CODPOS = " + codpos + " AND MAT = @mat AND PRORAP = @prorap) AS DATISC " +
                          "FROM STORDL A INNER JOIN CONRIF C ON A.CODCON = C.CODCON AND C.DATINI <= A.DATINI " +
                          "LEFT OUTER JOIN CONLOC D ON A.CODLOC = D.CODLOC AND A.DATINI BETWEEN " +
                          "D.DATINI AND D.DATFIN INNER JOIN CONLIV B ON A.CODLIV = B.CODLIV AND A.CODCON = " +
                          "B.CODCON AND C.PROCON = B.PROCON INNER JOIN TIPRAP E ON A.TIPRAP = E.TIPRAP " +
                          "WHERE A.CODPOS = " + codpos + " AND A.MAT = @mat AND A.PRORAP = @prorap ORDER BY DATFIN DESC",
                    matParam, prorapParam, matParam, prorapParam);

                var codLiv = Convert.ToInt32(getFromStoRdl.Rows[0]["CODLIV"]);
                var codTipRap = Convert.ToInt32(getFromStoRdl.Rows[0]["CODTIPRAP"]);
                //var codLoc = Convert.ToInt32(getFromStoRdl.Rows[0]["CODLOC"]);
                var codGruAss = Convert.ToInt32(getFromStoRdl.Rows[0]["CODGRUASS"]);
                var codLoc = getFromStoRdl.Rows[0]["CODLOC"]?.ToString();

                FillRapInfo(codTipRap, schedaIscritto, getFromStoRdl, codGruAss, codLiv, codLoc, getFromStoRdl);

                var lastDatiniFromStoRdl = getFromStoRdl.Rows[0].DateElementAt("DATINI");
                schedaIscritto.modIsc.contrattocod = getFromStoRdl.Rows[0]["CODCON"].ToString();

                var denCon = "SELECT TRIM(DENCON) AS DENCON FROM CONLOC WHERE CODLOC = " + codLoc + " ORDER BY DATINI DESC";
                var denominazioneContratto = objDataAccess.Get1ValueFromSQL(denCon, CommandType.Text);
                if (string.IsNullOrWhiteSpace(denominazioneContratto))
                {
                    denCon = "SELECT TRIM(DENCON) AS DENCON FROM CONRIF WHERE CODCON = " + schedaIscritto.modIsc.contrattocod + " ORDER BY DATINI DESC";
                    denominazioneContratto = objDataAccess.Get1ValueFromSQL(denCon, CommandType.Text);
                }
                schedaIscritto.modIsc.contratto = denominazioneContratto;


                if (Convert.ToInt32(getFromStoRdl.Rows.Count) > 0)
                    FillEmolumenti(lastDatiniFromStoRdl, schedaIscritto, codLiv, getFromStoRdl);

                Decimal num;
                if (HasDaAl(codTipRap))
                {
                    var firstDatiniOfDaal = getFromStoRdl.Rows[2].DateElementAt("DATINI");
                    num = GetTotImporto(matParam, codpos, firstDatiniOfDaal, schedaIscritto, getFromStoRdl, prorapParam);
                }
                else
                    num = GetTotImporto(matParam, codpos, lastDatiniFromStoRdl, schedaIscritto, getFromStoRdl, prorapParam);

                var datscater = getFromStoRdl.Rows[0].DateElementAt("DATSCATER");
                schedaIscritto.modIsc.meseSel = new List<string>();

                FillMesi(matParam, prorapParam, codpos, schedaIscritto);

                FormatMoneyInput(schedaIscritto, num, datscater);

                FillAziendaUtilizzatrice(matParam, prorapParam, codpos, schedaIscritto);

                if (HasDaAl(codTipRap))
                    FillDaAl(schedaIscritto, getFromStoRdl);

                schedaIscritto.modIsc.datIsc = getFromStoRdl.Rows[0].DateElementAt("DATISC");
            }
            HttpContext.Current.Session["ModificaIscritto"] = (object)schedaIscritto;
            return schedaIscritto;
        }

        private bool HasDaAl(int codTipRap)
        {
            return codTipRap == 8 || codTipRap == 7 || codTipRap == 6 || codTipRap == 10;
        }

        private void FillDaAl(RapportiLavoro schedaIscritto, DataTable getFromStordl)
        {
            schedaIscritto.modIsc.daal3 = getFromStordl.Rows[0].DateElementAt("DATINI");
            schedaIscritto.modIsc.alal3 = getFromStordl.Rows[0].DateElementAt("DATFIN");
            schedaIscritto.modIsc.perce3 = getFromStordl.Rows[0].ElementAt("PERAPP").Replace(',', '.');

            schedaIscritto.modIsc.daal2 = getFromStordl.Rows[1].DateElementAt("DATINI");
            schedaIscritto.modIsc.alal2 = getFromStordl.Rows[1].DateElementAt("DATFIN");
            schedaIscritto.modIsc.perce2 = getFromStordl.Rows[1].ElementAt("PERAPP").Replace(',', '.');

            schedaIscritto.modIsc.daal = getFromStordl.Rows[2].DateElementAt("DATINI");
            schedaIscritto.modIsc.alal = getFromStordl.Rows[2].DateElementAt("DATFIN");
            schedaIscritto.modIsc.perce1 = getFromStordl.Rows[2].ElementAt("PERAPP").Replace(',', '.');

            schedaIscritto.modIsc.datIsc = getFromStordl.Rows[2].DateElementAt("DATISC");

        }
        private void FillAziendaUtilizzatrice(iDB2Parameter matricola, iDB2Parameter prorap, string codpos, RapportiLavoro schedaIscritto)
        {
            var getAzInfo = objDataAccess.GetDataTableWithParameters($"SELECT * FROM RAPLAV WHERE MAT = @mat AND CODPOS = {codpos} AND PRORAP = @prorap", matricola, prorap).Rows[0];
            schedaIscritto.modIsc.comuneAZCod = getAzInfo.ElementAt("CODCOMFOR");
            schedaIscritto.modIsc.comuneAZ = getAzInfo.ElementAt("DENCOMFOR");
            schedaIscritto.modIsc.residenzaAZCod = getAzInfo.ElementAt("CODDUGFOR");
            schedaIscritto.modIsc.provinciaAZ = getAzInfo.ElementAt("SIGPROFOR");
            schedaIscritto.modIsc.indirizzoAZ = getAzInfo.ElementAt("INDFOR");
            schedaIscritto.modIsc.numCivAZ = getAzInfo.ElementAt("NUMCIVFOR");
            schedaIscritto.modIsc.capAZ = getAzInfo.ElementAt("CAPFOR");
            schedaIscritto.modIsc.cfAz = getAzInfo.ElementAt("CODFISFOR");
            schedaIscritto.modIsc.piAZ = getAzInfo.ElementAt("PARIVAFOR");
            schedaIscritto.modIsc.ragsocAZ = getAzInfo.ElementAt("RAGSOCFOR");
            schedaIscritto.modIsc.pozioneAZ = getAzInfo.ElementAt("CODPOSFOR");
            schedaIscritto.modIsc.localitaAZ = getAzInfo.ElementAt("DENLOCFOR");
        }

        private decimal GetTotImporto(iDB2Parameter matricola, string codpos, string lastDatiniFromStoRdl, RapportiLavoro schedaIscritto, DataTable getFromStoRdl, iDB2Parameter prorap)
        {
            var getImpAgg = (string mes) =>
            {
                var result = objDataAccess.GetDataTableWithParameters(
                    "SELECT VALUE(IMPAGG, 0.00) AS IMPAGG13 FROM IMPAGG WHERE CODPOS = " + codpos + " AND MAT = @mat AND DATINI = '" +
                    lastDatiniFromStoRdl + "' AND MENAGG = " + mes +
                    $" AND DATINI = (SELECT MAX(DATINI) FROM IMPAGG WHERE MAT = @mat AND CODPOS = {codpos} AND PRORAP = @prorap)",
                    matricola, matricola, prorap).Rows;
                return result.Count > 0 ? result[0].ElementAt("IMPAGG13") : null;
            };

            var s12 = getImpAgg("12");
            var s13 = getImpAgg("13");
            var s14 = getImpAgg("14");
            var s15 = getImpAgg("15");
            var s16 = getImpAgg("16");

            schedaIscritto.modIsc.importoSc = getFromStoRdl.Rows[0].ElementAt("IMPSCAMAT");

            schedaIscritto.modIsc.S12 = string.IsNullOrEmpty(s12) ? "0" : s12;
            schedaIscritto.modIsc.S13 = string.IsNullOrEmpty(s13) ? "0" : s13;
            schedaIscritto.modIsc.S14 = string.IsNullOrEmpty(s14) ? "0" : s14;
            schedaIscritto.modIsc.S15 = string.IsNullOrEmpty(s15) ? "0" : s15;
            schedaIscritto.modIsc.S16 = string.IsNullOrEmpty(s16) ? "0" : s16;
            schedaIscritto.modIsc.emolumenti = string.IsNullOrEmpty(schedaIscritto.modIsc.emolumenti) ? "0" : schedaIscritto.modIsc.emolumenti;
            schedaIscritto.modIsc.importoSc = string.IsNullOrEmpty(schedaIscritto.modIsc.importoSc) ? "0" : schedaIscritto.modIsc.importoSc;
            schedaIscritto.modIsc.retribuzione = string.IsNullOrEmpty(schedaIscritto.modIsc.retribuzione) ? "0" : schedaIscritto.modIsc.retribuzione;

            var num = Convert.ToDecimal(schedaIscritto.modIsc.emolumenti) + Convert.ToDecimal(schedaIscritto.modIsc.importoSc) + Convert.ToDecimal(schedaIscritto.modIsc.S12) +
                      Convert.ToDecimal(schedaIscritto.modIsc.S13) +
                      Convert.ToDecimal(schedaIscritto.modIsc.S14) + Convert.ToDecimal(schedaIscritto.modIsc.S15) + Convert.ToDecimal(schedaIscritto.modIsc.S16) + Convert.ToDecimal(schedaIscritto.modIsc.retribuzione);
            return num;
        }

        private void FillMesi(iDB2Parameter matricola, iDB2Parameter prorap, string codpos, RapportiLavoro schedaIscritto)
        {
            var mesiRows = objDataAccess.GetDataTableWithParameters(
                $"SELECT PARMES FROM PARTIMM WHERE MAT = @mat AND CODPOS = {codpos} AND PRORAP = @prorap AND " +
                    $"DATINI = (SELECT MAX(DATINI) FROM PARTIMM WHERE MAT = @mat AND CODPOS = {codpos} AND PRORAP = @prorap)",
                matricola, prorap, matricola, prorap).Rows;
            for (int i = 0; i < mesiRows.Count; i++)
            {
                schedaIscritto.modIsc.meseSel.Add(mesiRows[i]["PARMES"].ToString());
            }
        }

        private static void FormatMoneyInput(RapportiLavoro schedaIscritto, decimal num, string datscater)
        {
            schedaIscritto.modIsc.importoSc = schedaIscritto.modIsc.importoSc.ToDomainMoneyFormat();
            schedaIscritto.modIsc.totaleS = num.ToString().ToDomainMoneyFormat();
            schedaIscritto.modIsc.S12 = schedaIscritto.modIsc.S12.ToDomainMoneyFormat();
            schedaIscritto.modIsc.S13 = schedaIscritto.modIsc.S13.ToDomainMoneyFormat();
            schedaIscritto.modIsc.S14 = schedaIscritto.modIsc.S14.ToDomainMoneyFormat();
            schedaIscritto.modIsc.S15 = schedaIscritto.modIsc.S15.ToDomainMoneyFormat();
            schedaIscritto.modIsc.S16 = schedaIscritto.modIsc.S16.ToDomainMoneyFormat();
            //schedaIscritto.modIsc.datTerm = string.IsNullOrWhiteSpace(datscater) ? null : datscater.ToDomainDateStringFormat();
            schedaIscritto.modIsc.datTerm = string.IsNullOrWhiteSpace(datscater) ? null : datscater;
            schedaIscritto.modIsc.retribuzione = schedaIscritto.modIsc.retribuzione.ToDomainMoneyFormat();
            schedaIscritto.modIsc.emolumenti = schedaIscritto.modIsc.emolumenti.ToDomainMoneyFormat();
        }

        private void FillEmolumenti(string lastDatiniFromStoRdl, RapportiLavoro schedaIscritto, int codLiv, DataTable datiCompletiStordl)
        {
            CaricaContratti(string.Empty);
            schedaIscritto.modIsc.emolumenti = AggiornaEmolumenti(codLiv.ToString(), schedaIscritto.modIsc.contratto, lastDatiniFromStoRdl).modIsc.emolumenti;
            schedaIscritto.modIsc.retribuzione = Convert.ToDecimal("0" + datiCompletiStordl.Rows[0]["TRAECO"]).ToString();
            if (!string.IsNullOrEmpty(datiCompletiStordl.Rows[0]["DATULTSCA"].ToString()))
                schedaIscritto.modIsc.dataUltSc = datiCompletiStordl.Rows[0].DateElementAt("DATULTSCA");
            if (!string.IsNullOrEmpty(datiCompletiStordl.Rows[0]["DATNEWSCA"].ToString()))
                schedaIscritto.modIsc.prossimoSc = datiCompletiStordl.Rows[0].DateElementAt("DATNEWSCA");
        }

        private void FillRapInfo(int codTipRap, RapportiLavoro schedaIscritto, DataTable getFromStoRdl, int codGruAss, int codLiv, string codLoc, DataTable datiCompletiStordl)
        {
            var tipRapSqlQuery = "SELECT DENTIPRAP FROM TIPRAP WHERE TIPRAP = " + codTipRap;
            schedaIscritto.modIsc.datIsc = getFromStoRdl.Rows[0].DateElementAt("DATINI");
            schedaIscritto.modIsc.tipRap = codTipRap.ToString();
            schedaIscritto.modIsc.dentipRap = objDataAccess.GetDataTable(tipRapSqlQuery).Rows[0].ElementAt("DENTIPRAP");
            schedaIscritto.modIsc.livello = getFromStoRdl.Rows[0].ElementAt("LIVELLO");
            schedaIscritto.modIsc.numMes = getFromStoRdl.Rows[0].ElementAt("NUMMEN");
            schedaIscritto.modIsc.m14 = getFromStoRdl.Rows[0].ElementAt("MESMEN14");
            schedaIscritto.modIsc.m15 = getFromStoRdl.Rows[0].ElementAt("MESMEN15");
            schedaIscritto.modIsc.m16 = getFromStoRdl.Rows[0].ElementAt("MESMEN16");
            schedaIscritto.modIsc.aliquota = getFromStoRdl.Rows[0].DecimalElementAt("ALIQUOTA")?.ToString();
            schedaIscritto.modIsc.PercPT = getFromStoRdl.Rows[0].ElementAt("PERPAR").Replace(',','.');
            schedaIscritto.modIsc.gruass = codGruAss.ToString();
            schedaIscritto.modIsc.codlivello = codLiv.ToString();
            schedaIscritto.modIsc.codloc = codLoc;
            schedaIscritto.modIsc.fap = getFromStoRdl.Rows[0].ElementAt("FAP") == "S";
            schedaIscritto.modIsc.scattiAnz = getFromStoRdl.Rows[0].ElementAt("NUMSCAMAT");

            var getDenQuaSqlQuery = "SELECT DENQUA FROM QUACON WHERE CODQUACON = " + Convert.ToInt32(getFromStoRdl.Rows[0]["CODQUACON"]);
            schedaIscritto.modIsc.qualifica = this.objDataAccess.Get1ValueFromSQL(getDenQuaSqlQuery, CommandType.Text);
            var denGruAss = "SELECT dengruass FROM GRUASS g WHERE CODGRUASS = " + codGruAss;
            schedaIscritto.modIsc.aliqCont = this.objDataAccess.Get1ValueFromSQL(denGruAss, CommandType.Text).Trim();
            schedaIscritto.modIsc.aliqContval = codGruAss.ToString();
            if (Convert.ToInt32(datiCompletiStordl.Rows.Count) > 0)
                schedaIscritto.modIsc.assCon = datiCompletiStordl.Rows[0]["ASSCON"].ToString().Trim() == "S";
        }

        private void FillIndirizzoAndContacts(iDB2Parameter mat, RapportiLavoro schedaIscritto)
        {
            var getIndirizzoInfo = objDataAccess.GetDataTableWithParameters("SELECT * FROM ISCD WHERE MAT = @mat ORDER BY DATINI DESC", mat).Rows[0];
            var dugData = objDataAccess.GetDataTable("SELECT CODDUG, DENDUG FROM DUG WHERE CODDUG = " + getIndirizzoInfo.ElementAt("CODDUG"));
            var indirizzoData = objDataAccess.GetDataTable("SELECT * FROM codcom WHERE codcom = '" + getIndirizzoInfo.ElementAt("CODCOM") + "'");

            fillModIscWithDataFromDb();

            void fillModIscWithDataFromDb()
            {
                if (indirizzoData.Rows.Count > 0)
                {
                    schedaIscritto.modIsc.comune = indirizzoData.Rows[0]["DENCOM"].ToString().Trim();
                    schedaIscritto.modIsc.comuneCod = indirizzoData.Rows[0]["CODCOM"].ToString().Trim();
                    schedaIscritto.modIsc.provincia = indirizzoData.Rows[0]["SIGPRO"].ToString().Trim();
                }

                if (dugData.Rows.Count > 0)
                {
                    schedaIscritto.modIsc.indirizzoCod = dugData.Rows[0].ElementAt("CODDUG");
                    schedaIscritto.modIsc.indirizzo = dugData.Rows[0].ElementAt("DENDUG");
                }

                schedaIscritto.modIsc.residenza = getIndirizzoInfo.ElementAt("IND");
                schedaIscritto.modIsc.civico = getIndirizzoInfo.ElementAt("NUMCIV");
                schedaIscritto.modIsc.cap = getIndirizzoInfo.ElementAt("CAP");
                schedaIscritto.modIsc.localita = getIndirizzoInfo.ElementAt("DENLOC");
                schedaIscritto.modIsc.statoEs = getIndirizzoInfo.ElementAt("DENSTAEST");
                schedaIscritto.modIsc.co = getIndirizzoInfo.ElementAt("CO");
                schedaIscritto.modIsc.tel = getIndirizzoInfo.ElementAt("TEL1");
                schedaIscritto.modIsc.cell = getIndirizzoInfo.ElementAt("CELL");
                schedaIscritto.modIsc.fax = getIndirizzoInfo.ElementAt("FAX");
                schedaIscritto.modIsc.email = getIndirizzoInfo.ElementAt("EMAIL");
                schedaIscritto.modIsc.pec = getIndirizzoInfo.ElementAt("EMAILCERT");
            }
        }

        private static void FillAnagraficaIsct(string prorap, RapportiLavoro schedaIscritto, DataTable getFromIsct)
        {
            schedaIscritto.modIsc.checkIscritto = 0;
            schedaIscritto.modIsc.prorap = prorap;
            schedaIscritto.modIsc.codFis = getFromIsct.Rows[0].ElementAt("CODFIS");
            schedaIscritto.modIsc.matricola = getFromIsct.Rows[0].ElementAt("MAT");
            schedaIscritto.modIsc.cognome = getFromIsct.Rows[0].ElementAt("COG");
            schedaIscritto.modIsc.nome = getFromIsct.Rows[0].ElementAt("NOM");
            schedaIscritto.modIsc.dataNas = getFromIsct.Rows[0].DateElementAt("DATNAS");
            schedaIscritto.modIsc.sesso = getFromIsct.Rows[0].ElementAt("SES");
            schedaIscritto.modIsc.comuneN = getFromIsct.Rows[0].ElementAt("DENCOMNAS");
            schedaIscritto.modIsc.comuneNCod = getFromIsct.Rows[0].ElementAt("CODCOMNAS");
            schedaIscritto.modIsc.provinciaN = getFromIsct.Rows[0].ElementAt("SIGPRONAS");

            if (string.IsNullOrEmpty(getFromIsct.Rows[0]["DENTITSTU"].ToString()))
                return;

            var objDataAccess = new DataLayer();
            var getTitStuInfo = objDataAccess.GetDataTable("SELECT * FROM TITSTU WHERE CODTITSTU = " + getFromIsct.Rows[0].ElementAt("TITSTU")).Rows[0];
            schedaIscritto.modIsc.titoloStudio = getTitStuInfo.ElementAt("DENTITSTU");
            schedaIscritto.modIsc.titoloStudioCod = getFromIsct.Rows[0].ElementAt("TITSTU");
        }

        public bool CheckUnioneSospensioniIntersecanti(string dataDa, string dataAl, string codsos, string matricola, string prorap, string codpos, object utente, string prosos, ref string messaggio, ref string successMSG)
        {
            TimePeriodCollection sospensioniUgualiIntersecanti = CheckIntersezioneSospensione(prosos, codsos, prorap, matricola, dataDa, dataAl, codpos, ref messaggio);
            return sospensioniUgualiIntersecanti.Count > 0;
        }

        public RapportiLavoro GetSospensioni(
          string matricola,
          string nome,
          string cognome,
          string prorap,
          string codpos,
          ref string messaggio,
          ref string SuccessMSG)
        {
            try
            {
                string strSQL1 = "SELECT MAT, COG, NOM, SES FROM ISCT WHERE MAT = " + matricola;
                string Err = "";
                DataSet dataSet1 = this.objDataAccess.GetDataSet(strSQL1, ref Err);
                RapportiLavoro sospensioni1 = new RapportiLavoro();
                sospensioni1.sospensioni = new List<Sospensioni>();
                sospensioni1.sosp.matricola = dataSet1.Tables[0].Rows[0]["MAT"].ToString().Trim();
                sospensioni1.sosp.nominativo = dataSet1.Tables[0].Rows[0]["COG"].ToString().Trim() + " " + dataSet1.Tables[0].Rows[0]["NOM"].ToString().Trim();
                sospensioni1.sosp.sesso = dataSet1.Tables[0].Rows[0]["SES"].ToString().Trim();
                sospensioni1.sosp.prorap = Convert.ToInt32(prorap);
                string sesso = sospensioni1.sosp.sesso;
                string strSQL2 = "SELECT TRANSLATE(CHAR(DATINISOS,EUR),'/','.') AS DAL,  TRANSLATE(CHAR(DATFINSOS,EUR),'/','.') AS AL, TRIM(DENSOS) AS SOSPENSIONE, PERAZI AS PERC_AZIENDA, PERFIG AS PERC_FIGURATIVA, PRORAP, SOSRAP.CODSOS, PROSOS FROM SOSRAP, CODSOS WHERE SOSRAP.CODSOS = CODSOS.CODSOS AND SOSRAP.MAT = " + matricola + " AND SOSRAP.PRORAP = " + prorap + " AND SOSRAP.CODPOS =  '" + codpos + "' AND STASOS = '0' ORDER BY DATINISOS DESC, DATFINSOS";
                DataSet dataSet2 = new DataSet();
                foreach (DataRow row in (InternalDataCollectionBase)this.objDataAccess.GetDataSet(strSQL2, ref Err).Tables[0].Rows)
                {
                    Sospensioni sospensioni2 = new Sospensioni()
                    {
                        dataDa = row["DAL"].ToString(),
                        dataAl = row["AL"].ToString(),
                        sospensione = row["SOSPENSIONE"].ToString().Trim(),
                        prosos = row["PROSOS"].ToString().Trim(),
                        prorap = Convert.ToInt32(row["PRORAP"]),
                        codsos = Convert.ToInt32(row["CODSOS"])
                    };
                    sospensioni1.sospensioni.Add(sospensioni2);
                }
                DataSet dataSet3 = this.objDataAccess.GetDataSet("SELECT CODSOS, TRIM(DENSOS) AS DENSOS FROM CODSOS WHERE CODSOS <> 0 AND UTESOS IN ('A', 'T') AND " + "TIPSES IN ('" + sesso + "', 'E') AND current_date BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31') " + "AND CODSOS <> 4 ORDER BY DENSOS", ref Err);
                sospensioni1.listSosps = new List<ListSosp>();
                foreach (DataRow row in (InternalDataCollectionBase)dataSet3.Tables[0].Rows)
                {
                    ListSosp listSosp = new ListSosp()
                    {
                        codsos = row["CODSOS"].ToString(),
                        densos = row["DENSOS"].ToString().Trim()
                    };
                    sospensioni1.listSosps.Add(listSosp);
                }
                return sospensioni1;
            }
            catch (Exception ex)
            {
                return (RapportiLavoro)null;
            }
        }

        public RapportiLavoro GetNuvoRapportiLavoro(string codpos)
        {
            string Err = "";
            RapportiLavoro nuvoRapportiLavoro = new RapportiLavoro();
            nuvoRapportiLavoro.listNuovoIscritto = new List<NuovoIscritto>();
            nuvoRapportiLavoro.modIsc.checkIscritto = 1;
            nuvoRapportiLavoro.ragioneSociale = this.objDataAccess.Get1ValueFromSQL("SELECT RAGSOC FROM AZI WHERE CODPOS =" + codpos, CommandType.Text);
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
            DataSet dataSet1 = this.objDataAccess.GetDataSet("SELECT CODDUG ,DENDUG FROM DUG ORDER BY DENDUG ASC", ref Err);
            nuvoRapportiLavoro.listNIVia = new List<NuovoIscritto>();
            if (dataSet1 != null)
            {
                foreach (DataRow row in (InternalDataCollectionBase)dataSet1.Tables[0].Rows)
                {
                    NuovoIscritto nuovoIscritto = new NuovoIscritto()
                    {
                        indirizzo = row["DENDUG"].ToString().Trim(),
                        indirizzoCod = row["CODDUG"].ToString().Trim()
                    };
                    nuvoRapportiLavoro.listNIVia.Add(nuovoIscritto);
                }
            }
            DataSet dataSet2 = this.objDataAccess.GetDataSet("SELECT DENTITSTU , CODTITSTU FROM TITSTU ORDER BY ORDINE ASC", ref Err);
            nuvoRapportiLavoro.listNIStudio = new List<NuovoIscritto>();
            if (dataSet1 != null)
            {
                foreach (DataRow row in (InternalDataCollectionBase)dataSet2.Tables[0].Rows)
                {
                    NuovoIscritto nuovoIscritto = new NuovoIscritto()
                    {
                        titoloStudio = row["DENTITSTU"].ToString().Trim(),
                        titoloStudioCod = row["CODTITSTU"].ToString().Trim()
                    };
                    nuvoRapportiLavoro.listNIStudio.Add(nuovoIscritto);
                }
            }
            string strSQL = "SELECT NATGIU FROM AZI WHERE CODPOS = " + codpos;
            HttpContext.Current.Session["GruCon"] = (object)"N";
            var natgiu = Convert.ToInt16("0" + this.objDataAccess.Get1ValueFromSQL(strSQL, CommandType.Text));
            HttpContext.Current.Items["IsAziSomm"] = natgiu == 27;
            if (natgiu == 10)
                HttpContext.Current.Session["GruCon"] = "S";
            DataSet dataSet3 = this.objDataAccess.GetDataSet(!(HttpContext.Current.Request.QueryString["ProRap"] == "") ? "SELECT TIPRAP, DENTIPRAP FROM TIPRAP ORDER BY TIPRAP" : "SELECT TIPRAP, DENTIPRAP FROM TIPRAP WHERE TIPRAP NOT IN (5) ORDER BY TIPRAP", ref Err);
            nuvoRapportiLavoro.listNuovoIscritto = new List<NuovoIscritto>();
            if (dataSet3 != null)
            {
                foreach (DataRow row in (InternalDataCollectionBase)dataSet3.Tables[0].Rows)
                {
                    NuovoIscritto nuovoIscritto = new NuovoIscritto()
                    {
                        tipRap = row["TIPRAP"].ToString().Trim(),
                        dentipRap = row["DENTIPRAP"].ToString().Trim()
                    };
                    nuvoRapportiLavoro.listNuovoIscritto.Add(nuovoIscritto);
                }
            }
            nuvoRapportiLavoro.listNuovoIscritto2 = new List<NuovoIscritto>();
            DataSet dataSet4 = this.objDataAccess.GetDataSet("SELECT CODQUACON, DENQUA FROM QUACON", ref Err);
            if (dataSet4 != null)
            {
                foreach (DataRow row in (InternalDataCollectionBase)dataSet4.Tables[0].Rows)
                {
                    NuovoIscritto nuovoIscritto = new NuovoIscritto()
                    {
                        qualifica = row["DENQUA"].ToString().Trim(),
                        codqualifica = row["CODQUACON"].ToString().Trim()
                    };
                    nuvoRapportiLavoro.listNuovoIscritto2.Add(nuovoIscritto);
                }
            }
            return nuvoRapportiLavoro;
        }

        public bool GetSalvaSospensioni(
          string dataDa,
          string dataAl,
          string codsos,
          string matricola,
          string prorap,
          string codpos,
          TFI.OCM.Utente.Utente u,
          string prosos,
          ref string messaggio,
          ref string SuccessMSG)
        {
            string strData1 = DateTime.Now.ToString();
            string nextProSos = "";
            bool blnCommit;
            try
            {
                DateTime dal = DateTime.Parse(dataDa);
                DateTime al = DateTime.Parse(dataAl);
                if (dal > al || dal > DateTime.Now.Date)
                {
                    messaggio = "Data inizio non può essere maggiore della data di fine o maggiore della data di oggi";
                    return false;
                }
                //if(al > GetDataCessazioneRDL(matricola, codpos, prorap)){
                //    messaggio = "Non è possibile inserire la sospensione. Il rapporto lavorativo termina prima della";
                //    return false;
                //}

                if (!CheckTipoEDurataSospensione(codsos, dal, al, ref messaggio))
                    return false;

                TimePeriodCollection sospensioniUgualiIntersecanti = CheckIntersezioneSospensione(prosos, codsos, prorap, matricola, dataDa, dataAl, codpos, ref messaggio);
                if (sospensioniUgualiIntersecanti == null)
                    return false;

                if (sospensioniUgualiIntersecanti.Count > 0)
                {
                    sospensioniUgualiIntersecanti.Add(new TimeRange()
                    {
                        Start = DateTime.Parse(dataDa),
                        End = DateTime.Parse(dataAl)
                    });

                    var result = CheckTipoEDurataSospensione(codsos, sospensioniUgualiIntersecanti.Start, sospensioniUgualiIntersecanti.End, ref messaggio);
                    if (!result)
                    {
                        messaggio = "E' stato inserito un perido continuativo troppo lungo per lo stesso tipo di sospensione.";
                        return false;
                    }
                    if (dal > sospensioniUgualiIntersecanti.Start && al < sospensioniUgualiIntersecanti.End)
                    {
                        if (sospensioniUgualiIntersecanti.Count <= 2)
                        {
                            messaggio = "Esiste già una sospensione di questo tipo nel periodo indicato.";
                            return false;
                        }
                    }

                    dataDa = sospensioniUgualiIntersecanti.Start.ToString("yyyy-MM-dd");
                    dataAl = sospensioniUgualiIntersecanti.End.ToString("yyyy-MM-dd");
                }

                this.objDataAccess.StartTransaction();
                Decimal num1 = 0M;
                Decimal num2 = 0M;
                iDB2DataReader dataReaderFromQuery1 = this.objDataAccess.GetDataReaderFromQuery("SELECT PERAZI, PERFIG FROM PERSOS WHERE CODSOS = " + codsos + " AND DATINI <= '" + dataAl + "' ORDER BY DATINI DESC", CommandType.Text);
                if (dataReaderFromQuery1.Read())
                {
                    num1 = Convert.ToDecimal(dataReaderFromQuery1["perazi"]);
                    num2 = Convert.ToDecimal(dataReaderFromQuery1["perfig"]);
                }
                iDB2DataReader dataReaderFromQuery2 = this.objDataAccess.GetDataReaderFromQuery("SELECT * FROM SOSRAP WHERE " + " CODPOS = " + codpos + " AND MAT = " + matricola + " AND PRORAP = " + prorap + " AND CODSOS = " + codsos + " AND DATFINSOS = '" + DBMethods.Db2Date(Convert.ToDateTime(dataDa).AddDays(-1.0).ToString()) + "'" + " AND STASOS = '0'", CommandType.Text);
                bool flag1 = false;
                DataTable dataTable = new DataTable();
                clsIDOC clsIdoc = new clsIDOC();
                iDB2DataAdapter iDb2DataAdapter = new iDB2DataAdapter();
                bool flag2;
                bool flag3;
                if (dataReaderFromQuery2.Read())
                {
                    string strData2 = dataReaderFromQuery2["DATINISOS"].ToString();
                    string strData3 = dataAl;
                    if (!flag1)
                    {
                        dataTable = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(dataReaderFromQuery2["PROSOS"]), "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                        clsIdoc.WRITE_IDOC_TESTATA(this.objDataAccess, dataTable.Rows[0]);
                        flag1 = true;
                    }
                    dataTable.Clear();
                    DataTable idocDatiE1Pitype1 = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(dataReaderFromQuery2["PROSOS"]), "", "", "9999-12-31", "", "", "", 0, 0, 0, "D", "", "", "");
                    clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "9003", idocDatiE1Pitype1, true);
                    if (!this.objDataAccess.WriteTransactionData(" UPDATE SOSRAP SET STASOS = '2'," + " UTEAGG = '" + u.CodPosizione.Replace("'", "''") + "'," + " ULTAGG = CURRENT_TIMESTAMP WHERE CODPOS = " + codpos + " AND MAT = " + matricola + " AND PRORAP = " + prorap + " AND PROSOS = " + dataReaderFromQuery2["PROSOS"]?.ToString(), CommandType.Text))
                        throw new Exception();
                    nextProSos = this.objDataAccess.Get1ValueFromSQL("SELECT VALUE(MAX(PROSOS), 0) + 1 AS TOT FROM SOSRAP " + " WHERE CODPOS = " + codpos + " AND MAT = " + matricola + " " + " AND PRORAP = " + prorap, CommandType.Text);
                    flag2 = this.objDataAccess.WriteTransactionData("INSERT INTO SOSRAP (CODSOS, CODPOS, MAT, PRORAP, PROSOS," + " DATINISOS, DATFINSOS, PERAZI, PERFIG, STASOS, UTEAGG, ULTAGG )" + " Values ( " + codsos + ", " + codpos + ", " + matricola + ", " + prorap + ", " + nextProSos + ", " + "'" + DBMethods.Db2Date(strData2) + "', " + "'" + DBMethods.Db2Date(strData3) + "', " + DBMethods.DoublePeakForSql(num1.ToString()).Replace(",", ".") + ", " + DBMethods.DoublePeakForSql(num2.ToString()).Replace(",", ".") + ", " + " '0', " + "'" + u.CodPosizione.Replace("'", "''") + "'," + "CURRENT_TIMESTAMP)", CommandType.Text);
                    if (!flag2)
                        throw new Exception();
                    if (!flag1)
                    {
                        idocDatiE1Pitype1 = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(nextProSos), "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                        clsIdoc.WRITE_IDOC_TESTATA(this.objDataAccess, idocDatiE1Pitype1.Rows[0]);
                        flag1 = true;
                    }
                    idocDatiE1Pitype1.Clear();
                    DataTable idocDatiE1Pitype2 = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(nextProSos), "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                    clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "9003", idocDatiE1Pitype2, true);
                    iDB2DataReader dataReaderFromQuery3 = this.objDataAccess.GetDataReaderFromQuery("SELECT * FROM SOSRAP WHERE " + " CODPOS = " + codpos + " AND MAT = " + matricola + " AND PRORAP = " + prorap + " AND CODSOS = " + codsos + " AND DATFINSOS = '" + DBMethods.Db2Date(dataAl) + "'" + " AND DATINISOS <> '" + DBMethods.Db2Date(strData2) + "'" + " AND STASOS = '0'", CommandType.Text);
                    if (dataReaderFromQuery3.Read())
                    {
                        strData2 = dataReaderFromQuery3["DATINISOS"].ToString();
                        dataReaderFromQuery3["DATFINSOS"].ToString();
                        if (!flag1)
                        {
                            idocDatiE1Pitype2 = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(dataReaderFromQuery3["PROSOS"]), "", "", "9999-12-31", "", "", "", 0, 0, 0, "D", "", "", "");
                            clsIdoc.WRITE_IDOC_TESTATA(this.objDataAccess, idocDatiE1Pitype2.Rows[0]);
                            flag1 = true;
                        }
                        idocDatiE1Pitype2.Clear();
                        idocDatiE1Pitype2 = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(dataReaderFromQuery3["PROSOS"]), "", "", "9999-12-31", "", "", "", 0, 0, 0, "D", "", "", "");
                        clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "9003", idocDatiE1Pitype2, true);
                        flag2 = this.objDataAccess.WriteTransactionData(" UPDATE SOSRAP SET STASOS = '2'," + " UTEAGG = '" + u.CodPosizione.Replace("'", "''") + "'," + " ULTAGG = CURRENT_TIMESTAMP WHERE CODPOS = " + codpos + " AND MAT = " + matricola + " AND PRORAP = " + prorap + " AND PROSOS = " + dataReaderFromQuery3["PROSOS"]?.ToString(), CommandType.Text);
                        if (!flag2)
                            throw new Exception();
                    }
                    iDB2DataReader dataReaderFromQuery4 = this.objDataAccess.GetDataReaderFromQuery("SELECT * FROM SOSRAP WHERE " + " CODPOS = " + codpos + " AND MAT = " + matricola + " AND PRORAP = " + prorap + " AND CODSOS = " + codsos + " AND DATINISOS = '" + DBMethods.Db2Date(Convert.ToDateTime(dataAl).AddDays(1.0).ToString()) + "'" + " AND STASOS = '0'", CommandType.Text);
                    if (dataReaderFromQuery4.Read())
                    {
                        string strData4 = strData2;
                        string strData5 = dataReaderFromQuery4["DATFINSOS"].ToString();
                        if (!flag1)
                        {
                            idocDatiE1Pitype2 = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(dataReaderFromQuery4["PROSOS"]), "", "", "9999-12-31", "", "", "", 0, 0, 0, "D", "", "", "");
                            clsIdoc.WRITE_IDOC_TESTATA(this.objDataAccess, idocDatiE1Pitype2.Rows[0]);
                            flag1 = true;
                        }
                        idocDatiE1Pitype2.Clear();
                        DataTable idocDatiE1Pitype3 = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(dataReaderFromQuery4["PROSOS"]), "", "", "9999-12-31", "", "", "", 0, 0, 0, "D", "", "", "");
                        clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "9003", idocDatiE1Pitype3, true);
                        if (!flag1)
                        {
                            idocDatiE1Pitype3 = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(nextProSos), "", "", "9999-12-31", "", "", "", 0, 0, 0, "D", "", "", "");
                            clsIdoc.WRITE_IDOC_TESTATA(this.objDataAccess, idocDatiE1Pitype3.Rows[0]);
                            flag1 = true;
                        }
                        idocDatiE1Pitype3.Clear();
                        DataTable idocDatiE1Pitype4 = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(nextProSos), "", "", "9999-12-31", "", "", "", 0, 0, 0, "D", "", "", "");
                        clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "9003", idocDatiE1Pitype4, true);
                        if (!this.objDataAccess.WriteTransactionData(" UPDATE SOSRAP SET STASOS = '2', " + " UTEAGG = '" + u.CodPosizione.Replace("'", "''") + "'," + " ULTAGG = CURRENT_TIMESTAMP WHERE CODPOS = " + codpos + " AND MAT = " + matricola + " AND PRORAP = " + prorap + " AND PROSOS = " + dataReaderFromQuery4["PROSOS"]?.ToString(), CommandType.Text))
                            throw new Exception();
                        if (!this.objDataAccess.WriteTransactionData(" UPDATE SOSRAP SET STASOS = '2', " + " UTEAGG = '" + u.CodPosizione.Replace("'", "''") + "'," + " ULTAGG = CURRENT_TIMESTAMP WHERE CODPOS = " + codpos + " AND MAT = " + matricola + " AND PRORAP = " + prorap + " AND PROSOS = " + nextProSos, CommandType.Text))
                            throw new Exception();
                        nextProSos = this.objDataAccess.Get1ValueFromSQL("SELECT VALUE(MAX(PROSOS), 0) + 1 AS TOT FROM SOSRAP " + " WHERE CODPOS = " + codpos + " AND MAT = " + matricola + " " + " AND PRORAP = " + prorap, CommandType.Text);
                        flag2 = this.objDataAccess.WriteTransactionData("INSERT INTO SOSRAP (CODSOS, CODPOS, MAT, PRORAP, PROSOS," +
                                                                        " DATINISOS, DATFINSOS, PERAZI, PERFIG, STASOS, UTEAGG, ULTAGG )" +
                                                                        " Values ( " + codsos + ", " + codpos + ", " + matricola + ", " + prorap + ", " + nextProSos + ", " +
                                                                        "'" + DBMethods.Db2Date(strData4) + "', " + "'" + DBMethods.Db2Date(strData5) + "', " +
                                                                        DBMethods.DoublePeakForSql(num1.ToString()).Replace(",", ".") + ", " +
                                                                        DBMethods.DoublePeakForSql(num2.ToString()).Replace(",", ".") + ", " + " '0', " + "'" +
                                                                        u.CodPosizione.Replace("'", "''") + "', " + "CURRENT_TIMESTAMP)", CommandType.Text);
                        if (!flag2)
                            throw new Exception();
                        if (!flag1)
                        {
                            idocDatiE1Pitype4 = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(nextProSos), "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                            clsIdoc.WRITE_IDOC_TESTATA(this.objDataAccess, idocDatiE1Pitype4.Rows[0]);
                            flag3 = true;
                        }
                        idocDatiE1Pitype4.Clear();
                        DataTable idocDatiE1Pitype5 = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(nextProSos), "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                        clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "9003", idocDatiE1Pitype5, true);
                    }
                }
                else
                {
                    iDB2DataReader dataReaderFromQuery5 = this.objDataAccess.GetDataReaderFromQuery("SELECT * FROM SOSRAP WHERE " + " CODPOS = " + codpos + " AND MAT = " + matricola + " AND PRORAP = " + prorap + " AND CODSOS = " + codsos + " AND DATINISOS = '" + DBMethods.Db2Date(Convert.ToDateTime(dataAl).AddDays(1.0).ToString()) + "'" + " AND STASOS = '0'", CommandType.Text);
                    if (dataReaderFromQuery5.Read())
                    {
                        string strData6 = dataDa;
                        string strData7 = dataReaderFromQuery5["DATFINSOS"].ToString();
                        if (!flag1)
                        {
                            DataTable idocDatiE1Pitype = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(dataReaderFromQuery5["PROSOS"]), "", "", "9999-12-31", "", "", "", 0, 0, 0, "D", "", "", "");
                            clsIdoc.WRITE_IDOC_TESTATA(this.objDataAccess, idocDatiE1Pitype.Rows[0]);
                            flag1 = true;
                        }
                        DataTable idocDatiE1Pitype6 = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(dataReaderFromQuery5["PROSOS"]), "", "", "9999-12-31", "", "", "", 0, 0, 0, "D", "", "", "");
                        clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "9003", idocDatiE1Pitype6, true);
                        if (!string.IsNullOrEmpty(prosos))
                        {
                            if (!flag1)
                            {
                                idocDatiE1Pitype6 = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(prosos), "", "", "9999-12-31", "", "", "", 0, 0, 0, "D", "", "", "");
                                clsIdoc.WRITE_IDOC_TESTATA(this.objDataAccess, idocDatiE1Pitype6.Rows[0]);
                                flag1 = true;
                            }
                            idocDatiE1Pitype6.Clear();
                            idocDatiE1Pitype6 = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(prosos), "", "", "9999-12-31", "", "", "", 0, 0, 0, "D", "", "", "");
                            clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "9003", idocDatiE1Pitype6, true);
                        }
                        if (!this.objDataAccess.WriteTransactionData(" UPDATE SOSRAP SET STASOS = '2', " + " UTEAGG = '" + u.Username.Replace("'", "''") + "'," + " ULTAGG = CURRENT_TIMESTAMP WHERE CODPOS = " + codpos + " AND MAT = " + matricola + " AND PRORAP = " + prorap + " AND PROSOS = " + dataReaderFromQuery5["PROSOS"]?.ToString(), CommandType.Text))
                            throw new Exception();
                        if (!string.IsNullOrEmpty(prosos) && !this.objDataAccess.WriteTransactionData(" UPDATE SOSRAP SET STASOS = '2', " + " UTEAGG = '" + u.Username.Replace("'", "''") + "'," + " ULTAGG = CURRENT_TIMESTAMP WHERE CODPOS = " + codpos + " AND MAT = " + matricola + " AND PRORAP = " + prorap + " AND PROSOS = " + prosos, CommandType.Text))
                            throw new Exception();
                        nextProSos = this.objDataAccess.Get1ValueFromSQL("SELECT VALUE(MAX(PROSOS), 0) + 1 AS TOT FROM SOSRAP " + " WHERE CODPOS = " + codpos + " AND MAT = " + matricola + " " + " AND PRORAP = " + prorap, CommandType.Text);
                        flag2 = this.objDataAccess.WriteTransactionData("INSERT INTO SOSRAP (CODSOS, CODPOS, MAT, PRORAP, PROSOS," + " DATINISOS, DATFINSOS, PERAZI, PERFIG, STASOS, UTEAGG, ULTAGG )" + " Values ( " + codsos + ", " + codpos + ", " + matricola + ", " + prorap + ", " + nextProSos + ", " + "'" + DBMethods.Db2Date(strData6) + "', " + "'" + DBMethods.Db2Date(strData7) + "', " + DBMethods.DoublePeakForSql(num1.ToString()).Replace(",", ".") + ", " + DBMethods.DoublePeakForSql(num2.ToString()).Replace(",", ".") + ", " + " '0', " + "'" + u.Username.Replace("'", "''") + "', " + "CURRENT_TIMESTAMP)", CommandType.Text);
                        if (!flag2)
                            throw new Exception();
                        if (!flag1)
                        {
                            idocDatiE1Pitype6 = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(nextProSos), "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                            clsIdoc.WRITE_IDOC_TESTATA(this.objDataAccess, idocDatiE1Pitype6.Rows[0]);
                            flag3 = true;
                        }
                        idocDatiE1Pitype6.Clear();
                        DataTable idocDatiE1Pitype7 = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(nextProSos), "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                        clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "9003", idocDatiE1Pitype7, true);
                    }
                    else
                    {
                        string strData8 = dataDa;
                        string strData9 = dataAl;
                        if (string.IsNullOrEmpty(prosos))
                        {
                            nextProSos = this.objDataAccess.Get1ValueFromSQL("SELECT VALUE(MAX(PROSOS), 0) + 1 AS TOT FROM SOSRAP " + " WHERE CODPOS = " + codpos + " AND MAT = " + matricola + " " + " AND PRORAP = " + prorap, CommandType.Text);
                            flag2 = this.objDataAccess.WriteTransactionData("INSERT INTO SOSRAP (CODSOS, CODPOS, MAT, PRORAP, PROSOS," + " DATINISOS, DATFINSOS, PERAZI, PERFIG, STASOS, UTEAGG, ULTAGG )" + " Values ( " + codsos + ", " + codpos + ", " + matricola + ", " + prorap + ", " + nextProSos + ", " + "'" + DBMethods.Db2Date(strData8) + "', " + "'" + DBMethods.Db2Date(strData9) + "', " + DBMethods.DoublePeakForSql(num1.ToString()).Replace(",", ".") + ", " + DBMethods.DoublePeakForSql(num2.ToString()).Replace(",", ".") + ", " + " '0', " + "'" + u.Username.Replace("'", "''") + "', " + "CURRENT_TIMESTAMP)", CommandType.Text);
                            if (!flag2)
                                throw new Exception();
                            if (!flag1)
                            {
                                dataTable = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(nextProSos), "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                                clsIdoc.WRITE_IDOC_TESTATA(this.objDataAccess, dataTable.Rows[0]);
                                flag3 = true;
                            }
                            dataTable.Clear();
                            DataTable idocDatiE1Pitype = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(nextProSos), "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                            clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "9003", idocDatiE1Pitype, true);
                        }
                        else
                        {
                            if (!flag1)
                            {
                                dataTable = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(prosos), "", "", "9999-12-31", "", "", "", 0, 0, 0, "D", "", "", "");
                                clsIdoc.WRITE_IDOC_TESTATA(this.objDataAccess, dataTable.Rows[0]);
                                flag1 = true;
                            }
                            dataTable.Clear();
                            DataTable idocDatiE1Pitype8 = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(prosos), "", "", "9999-12-31", "", "", "", 0, 0, 0, "D", "", "", "");
                            clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "9003", idocDatiE1Pitype8, true);
                            flag2 = this.objDataAccess.WriteTransactionData("UPDATE SOSRAP SET " + " PERAZI = " + DBMethods.DoublePeakForSql(num1.ToString()).Replace(",", ".") + ", " + " PERFIG = " + DBMethods.DoublePeakForSql(num2.ToString()).Replace(",", ".") + ", " + " CODSOS = " + codsos + ", " + " DATINISOS= '" + DBMethods.Db2Date(strData8) + "', " + " DATFINSOS = '" + DBMethods.Db2Date(strData9) + "', " + " UTEAGG = '" + u.Username.Replace("'", "''") + "'," + " ULTAGG = CURRENT_TIMESTAMP WHERE MAT = " + matricola + " AND CODPOS = " + codpos + " AND STASOS = '0'" + " AND PROSOS = " + prosos + " AND PRORAP = " + prorap, CommandType.Text);
                            if (!flag2)
                                throw new Exception();
                            if (!flag1)
                            {
                                idocDatiE1Pitype8 = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(prosos), "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                                clsIdoc.WRITE_IDOC_TESTATA(this.objDataAccess, idocDatiE1Pitype8.Rows[0]);
                                flag3 = true;
                            }
                            idocDatiE1Pitype8.Clear();
                            DataTable idocDatiE1Pitype9 = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(prosos), "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                            clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "9003", idocDatiE1Pitype9, true);
                        }
                    }
                }

                iDB2Parameter nextProSosParam = objDataAccess.CreateParameter("@nextProSos", iDB2DbType.iDB2Decimal, 7, ParameterDirection.Input, string.IsNullOrWhiteSpace(nextProSos) ? prosos : nextProSos);
                iDB2Parameter prorapParam = objDataAccess.CreateParameter("@prorap", iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input, prorap);
                iDB2Parameter matricolaParam = objDataAccess.CreateParameter("@matricola", iDB2DbType.iDB2Decimal, 11, ParameterDirection.Input, matricola);
                iDB2Parameter dataDaParam = objDataAccess.CreateParameter("@dataDa", iDB2DbType.iDB2Date, 10, ParameterDirection.Input, dal.ToString("yyyy-MM-dd"));
                iDB2Parameter dataAlParam = objDataAccess.CreateParameter("@dataAl", iDB2DbType.iDB2Date, 10, ParameterDirection.Input, al.ToString("yyyy-MM-dd"));
                iDB2Parameter codposParam = objDataAccess.CreateParameter("@codpos", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, codpos);
                string updateSospensioneInRange = $"UPDATE SOSRAP SET STASOS = '1' WHERE MAT = @matricola AND CODPOS = @codpos AND PRORAP = @prorap AND PROSOS <> @nextProSos " +
                                $"AND STASOS  = '0' AND ((DATINISOS >= @dataDa AND DATFINSOS <= @dataAl) OR (DATFINSOS >= @dataDa AND DATFINSOS <= @dataAl) OR (DATINISOS >= @dataDa AND DATINISOS <= @dataAl))";
                flag2 = this.objDataAccess.WriteTransactionDataWithParametersAndDontCall(updateSospensioneInRange, CommandType.Text, matricolaParam, codposParam, prorapParam, nextProSosParam, dataDaParam, dataAlParam, dataDaParam, dataAlParam, dataDaParam, dataAlParam);
                if (flag2)
                {
                    clsIdoc.Aggiorna_IDOC(ref this.objDataAccess);
                    SuccessMSG = "Salvataggio andato a buon fine";
                }
                else
                {
                    messaggio = "Errore";
                    this.objDataAccess.EndTransaction(false);
                    return false;
                }
                blnCommit = true;

            }
            catch (Exception ex)
            {
                messaggio = "Errore";
                this.objDataAccess.EndTransaction(false);
                return false;
            }
            this.objDataAccess.EndTransaction(blnCommit);
            return true;
        }

        public bool GetEliminaSospensioni(
          string dataDa,
          string dataAl,
          string matricola,
          string prorap,
          string prosos,
          string codpos,
          TFI.OCM.Utente.Utente u,
          ref string messaggio,
          ref string SuccessMSG)
        {
            try
            {
                string str1 = DateTime.Parse(dataDa).Year.ToString();
                string str2 = DateTime.Parse(dataDa).Month.ToString();
                string strSQL = "SELECT COUNT(*) FROM DENTES WHERE CODPOS = " + codpos + " AND ANNDEN = " + str1 + " AND MESDEN = " + str2 + " AND DATCHI IS NOT NULL AND NUMMOVANN IS NULL";
                this.objDataAccess = new DataLayer();
                DataTable dataTable = new DataTable();
                if (Convert.ToInt16("0" + this.objDataAccess.Get1ValueFromSQL(strSQL, CommandType.Text)) > (short)0)
                {
                    messaggio = "Impossibile eliminare! Esiste una denuncia contabilizzata per il periodo di sospensione.";
                }
                else
                {
                    string str3 = DateTime.Parse(dataAl).Year.ToString();
                    string str4 = DateTime.Parse(dataAl).Month.ToString();
                    if (Convert.ToInt16("0" + this.objDataAccess.Get1ValueFromSQL("SELECT COUNT(*) FROM DENTES WHERE CODPOS = " + codpos + " AND ANNDEN = " + str3 + " AND MESDEN = " + str4 + " AND DATANN IS NOT NULL AND DATCHI IS NOT NULL", CommandType.Text)) > (short)0)
                    {
                        messaggio = "Impossibile eliminare! Esiste una denuncia contabilizzata per il periodo di sospensione.";
                    }
                    else
                    {
                        this.objDataAccess.StartTransaction();
                        iDB2DataAdapter iDb2DataAdapter = new iDB2DataAdapter();
                        bool blnCommit = this.objDataAccess.WriteTransactionData("UPDATE SOSRAP SET STASOS = '1' WHERE CODPOS = " + codpos + " AND MAT = " + matricola + " AND PRORAP = " + prorap + " AND PROSOS = " + prosos, CommandType.Text);
                        clsIDOC clsIdoc = new clsIDOC();
                        if (blnCommit)
                        {
                            clsIdoc.strUserCode = u.Username;
                            DataTable idocDatiE1Pitype1 = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(prosos), "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                            clsIdoc.WRITE_IDOC_TESTATA(this.objDataAccess, idocDatiE1Pitype1.Rows[0]);
                            idocDatiE1Pitype1.Clear();
                            DataTable idocDatiE1Pitype2 = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9003", Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), Convert.ToInt32(prosos), "", "", "9999-12-31", "", "", "", 0, 0, 0, "D", "", "", "");
                            clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "9003", idocDatiE1Pitype2, true);
                        }
                        if (blnCommit)
                            clsIdoc.Aggiorna_IDOC(ref this.objDataAccess);
                        this.objDataAccess.EndTransaction(blnCommit);
                        if (blnCommit)
                        {
                            SuccessMSG = "Operazione effettuata!";
                        }
                        else
                        {
                            messaggio = "Si sono verificati dei problemi nella eliminazione della sospensione!";
                            this.objDataAccess.EndTransaction(true);
                            if (blnCommit)
                                SuccessMSG = "Operazione effettuata!";
                            else
                                messaggio = "Si sono verificati dei problemi nella eliminazione della sospensione!";
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                bool blnCommit = false;
                this.objDataAccess.EndTransaction(blnCommit);
                messaggio = "Si sono verificati dei problemi nella eliminazione della sospensione!";
                return blnCommit;
            }
        }

        public RapportiLavoro GetCarrieraRapportiLavoro(
          string matricola,
          string prorap,
          string codpos)
        {
            string strSQL1 = "SELECT MAT, COG, NOM, UTEAGG FROM ISCT WHERE MAT =  " + matricola;
            string Err = "";
            DataSet dataSet1 = this.objDataAccess.GetDataSet(strSQL1, ref Err);
            RapportiLavoro carrieraRapportiLavoro = new RapportiLavoro();
            carrieraRapportiLavoro.carriera = new List<Carriera>();
            if (dataSet1.Tables[0].Rows.Count > 0)
            {
                carrieraRapportiLavoro.carr.matricola = dataSet1.Tables[0].Rows[0]["MAT"].ToString().Trim();
                carrieraRapportiLavoro.carr.nominativo = dataSet1.Tables[0].Rows[0]["COG"].ToString().Trim() + " " + dataSet1.Tables[0].Rows[0]["NOM"].ToString().Trim();
                carrieraRapportiLavoro.carr.uteagg = dataSet1.Tables[0].Rows[0]["UTEAGG"].ToString().Trim();
            }
            DataSet dataSet2 = this.objDataAccess.GetDataSet("SELECT TRANSLATE(CHAR(DATDEC, EUR),'/','.') AS DATDEC, TRANSLATE(CHAR(DATASS, EUR),'/','.') " + "AS DATASS, TRANSLATE(CHAR(DATDEN, EUR),'/','.') AS DATDEN FROM RAPLAV WHERE MAT = " + matricola + " AND CODPOS = " + codpos + " AND PRORAP = " + prorap, ref Err);
            if (dataSet1.Tables[0].Rows.Count > 0)
            {
                carrieraRapportiLavoro.carr.datIsc = dataSet2.Tables[0].Rows[0]["DATDEC"].ToString().Trim();
                carrieraRapportiLavoro.carr.datDen = dataSet2.Tables[0].Rows[0]["DATDEN"].ToString().Trim();
            }
            carrieraRapportiLavoro.carr.prorap = prorap;
            DataTable dataTable = this.objDataAccess.GetDataTable("SELECT DISTINCT A.DATINI, A.CODCON, value(A.CODLOC,0) as codloc , '' AS CONTRATTO, A.CODLIV, TRIM(B.DENLIV) " + "AS LIVELLO, TRIM(E.DENTIPRAP) AS TIPRAP, A.PRORAP, A.TIPRAP AS CODTIPRAP, A.DATFIN FROM " + "STORDL A INNER JOIN CONRIF C ON A.CODCON = C.CODCON AND C.DATINI <= A.DATINI " + "LEFT OUTER JOIN CONLOC D ON A.CODLOC = D.CODLOC AND A.DATINI BETWEEN " + "D.DATINI AND D.DATFIN INNER JOIN CONLIV B ON A.CODLIV = B.CODLIV AND A.CODCON = " + "B.CODCON AND C.PROCON = B.PROCON INNER JOIN TIPRAP E ON A.TIPRAP = E.TIPRAP " + " WHERE A.CODPOS = " + codpos + " AND A.MAT = " + matricola + " AND A.PRORAP = " + prorap + " ORDER BY DATINI DESC");
            int num = dataTable.Rows.Count - 1;
            for (int index = 0; index <= num; ++index)
            {
                string str = DateTime.Parse(dataTable.Rows[index]["DATINI"].ToString()).ToString("yyyy-MM-dd");
                string strSQL2 = Convert.ToInt32(dataTable.Rows[index]["CODLOC"]) != 0 ? "SELECT TRIM(DENCON) AS DENCON FROM CONLOC WHERE CODLOC = " + dataTable.Rows[index]["CODLOC"]?.ToString() + " ORDER BY DATINI DESC" : "SELECT TRIM(DENCON) AS DENCON FROM CONRIF WHERE CODCON = " + dataTable.Rows[index]["CODCON"]?.ToString() + " AND DATINI <='" + str + "' ORDER BY DATINI DESC";
                dataTable.Rows[index]["CONTRATTO"] = (object)this.objDataAccess.Get1ValueFromSQL(strSQL2, CommandType.Text);
            }
            carrieraRapportiLavoro.listContratti = new List<ListContratti>();
            foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
            {
                ListContratti listContratti = new ListContratti()
                {
                    datVar = row["DATINI"].ToString().Trim().Substring(0, 10),
                    tipRap = row["TIPRAP"].ToString().Trim(),
                    codTipRap = row["CODTIPRAP"].ToString().Trim(),
                    livello = row["LIVELLO"].ToString().Trim(),
                    contratto = row["CONTRATTO"].ToString().Trim(),
                    prorap = row["PRORAP"].ToString().Trim(),
                    datfin = DateTime.Parse(row["DATFIN"].ToString().Trim())
                };
                carrieraRapportiLavoro.listContratti.Add(listContratti);
            }

            var groupProrap = carrieraRapportiLavoro.listContratti.GroupBy(x => x.prorap);
            carrieraRapportiLavoro.listContratti = new List<ListContratti>();
            foreach (var recordProrap in groupProrap)
            {
                var orderedGroup = recordProrap.OrderByDescending(x => x.datfin);
                if (HasDaAl(int.Parse(orderedGroup.First().codTipRap)))
                {
                    carrieraRapportiLavoro.listContratti.AddRange(orderedGroup.Take(3));
                }
                else
                {
                    carrieraRapportiLavoro.listContratti.AddRange(orderedGroup.Take(1));
                }
            }

            return carrieraRapportiLavoro;
        }

        public RapportiLavoro GetDettagliCarriera(
          string matricola,
          string uteagg,
          string datini,
          string prorap,
          string codpos)
        {
            try
            {
                string strSQL1 = "SELECT MAT, COG, NOM, DATNAS, CURRENT_DATE AS TODAY," + " CODCOM AS CODCOMNAS, TITSTU, (SELECT DENCOM FROM CODCOM WHERE CODCOM.CODCOM = VALUE(ISCT.CODCOM,'@@@@@')) AS DENCOMNAS, " + " (SELECT SIGPRO FROM CODCOM WHERE CODCOM.CODCOM = VALUE(ISCT.CODCOM,'@@@@@')) AS SIGPRONAS, " + " CODFIS, SES, DATCHIISC, COUCHIISC, STACIV, (SELECT DENSTACIV FROM STACIV WHERE STACIV.CODSTACIV = VALUE(ISCT.STACIV ,0)) AS DENSTACIV, " + " TITSTU, (SELECT DENTITSTU FROM TITSTU WHERE TITSTU.CODTITSTU = VALUE(ISCT.TITSTU ,0)) AS DENTITSTU, ULTAGG, UTEAGG" + " FROM ISCT " + " WHERE MAT = " + matricola;
                string Err = "";
                DataSet dataSet1 = this.objDataAccess.GetDataSet(strSQL1, ref Err);
                RapportiLavoro dettagliCarriera = new RapportiLavoro();
                dettagliCarriera.dettagliCarriera = new List<DettagliCarriera>();
                dettagliCarriera.detCar.datIni = datini;
                if (dataSet1.Tables[0].Rows.Count > 0)
                {
                    dettagliCarriera.detCar.matricola = dataSet1.Tables[0].Rows[0]["MAT"].ToString().Trim();
                    dettagliCarriera.detCar.nome = dataSet1.Tables[0].Rows[0]["NOM"].ToString().Trim();
                    dettagliCarriera.detCar.cognome = dataSet1.Tables[0].Rows[0]["COG"].ToString().Trim();
                    dettagliCarriera.detCar.codFis = dataSet1.Tables[0].Rows[0]["CODFIS"].ToString().Trim();
                    dettagliCarriera.detCar.dataNas = dataSet1.Tables[0].Rows[0]["DATNAS"].ToString().ToDomainDateStringFormat();
                    dettagliCarriera.detCar.sesso = dataSet1.Tables[0].Rows[0]["SES"].ToString().Trim();
                    int num = !string.IsNullOrEmpty(dataSet1.Tables[0].Rows[0]["TITSTU"].ToString()) ? Convert.ToInt32(dataSet1.Tables[0].Rows[0]["TITSTU"]) : 0;
                    DataSet dataSet2 = this.objDataAccess.GetDataSet("SELECT dencom,sigpro FROM codcom WHERE codcom='" + dataSet1.Tables[0].Rows[0]["CODCOMNAS"].ToString() + "'", ref Err);
                    if (dataSet2.Tables[0].Rows.Count > 0)
                    {
                        dettagliCarriera.detCar.comuneN = dataSet2.Tables[0].Rows[0]["DENCOM"].ToString().Trim();
                        dettagliCarriera.detCar.provinciaN = dataSet2.Tables[0].Rows[0]["SIGPRO"].ToString().Trim();
                    }
                    DataSet dataSet3 = this.objDataAccess.GetDataSet("SELECT DENTITSTU FROM TITSTU WHERE codtitstu =" + num.ToString(), ref Err);
                    if (dataSet3.Tables[0].Rows.Count > 0)
                        dettagliCarriera.detCar.titoloStudio = dataSet3.Tables[0].Rows[0]["DENTITSTU"].ToString().Trim();
                }
                string str1 = "SELECT DATINI, IND, EMAILCERT, CELL, URL, NUMCIV, CODDUG, (SELECT DENDUG FROM DUG WHERE DUG.CODDUG = ISCD.CODDUG) " + "AS DENDUG, CODCOM, (SELECT DENCOM FROM CODCOM WHERE CODCOM.CODCOM = VALUE(ISCD.CODCOM,'@@@@@')) AS DENCOM, CAP, SIGPRO, DENLOC, DENSTAEST, TEL1, TEL2, FAX, EMAIL , CO FROM ISCD " + "WHERE MAT = " + matricola;
                DataSet dataSet4 = this.objDataAccess.GetDataSet(!(datini != "") ? str1 + " ORDER BY DATINI DESC" : str1 + " ORDER BY DATINI DESC  FETCH FIRST 1 ROWS ONLY", ref Err);
                if (dataSet4.Tables[0].Rows.Count > 0)
                {
                    dettagliCarriera.detCar.indirizzo = dataSet4.Tables[0].Rows[0]["DENDUG"].ToString().Trim();
                    dettagliCarriera.detCar.via = dataSet4.Tables[0].Rows[0]["IND"].ToString().Trim();
                    dettagliCarriera.detCar.numCiv = dataSet4.Tables[0].Rows[0]["NUMCIV"].ToString().Trim();
                    dettagliCarriera.detCar.comune = dataSet4.Tables[0].Rows[0]["DENCOM"].ToString().Trim();
                    dettagliCarriera.detCar.provincia = dataSet4.Tables[0].Rows[0]["SIGPRO"].ToString().Trim();
                    dettagliCarriera.detCar.cap = dataSet4.Tables[0].Rows[0]["CAP"].ToString().Trim();
                    dettagliCarriera.detCar.localita = dataSet4.Tables[0].Rows[0]["DENLOC"].ToString().Trim();
                    dettagliCarriera.detCar.statoEs = dataSet4.Tables[0].Rows[0]["DENSTAEST"].ToString().Trim();
                    dettagliCarriera.detCar.co = dataSet4.Tables[0].Rows[0]["CO"].ToString().Trim();
                    dettagliCarriera.detCar.tel = dataSet4.Tables[0].Rows[0]["TEL1"].ToString().Trim();
                    dettagliCarriera.detCar.tel2 = dataSet4.Tables[0].Rows[0]["TEL2"].ToString().Trim();
                    dettagliCarriera.detCar.cell = dataSet4.Tables[0].Rows[0]["CELL"].ToString().Trim();
                    dettagliCarriera.detCar.fax = dataSet4.Tables[0].Rows[0]["FAX"].ToString().Trim();
                    dettagliCarriera.detCar.email = dataSet4.Tables[0].Rows[0]["EMAIL"].ToString().Trim();
                    dettagliCarriera.detCar.pec = dataSet4.Tables[0].Rows[0]["EMAILCERT"].ToString().Trim();
                }
                string strSQL2 = "SELECT DATDEC FROM raplav  WHERE CODPOS =" + codpos + " AND mat=" + matricola;
                dettagliCarriera.detCar.datIsc = this.objDataAccess.Get1ValueFromSQL(strSQL2, CommandType.Text).ToDomainDateStringFormat();
                string strSQL3 = "SELECT PROTISC FROM raplav  WHERE CODPOS =" + codpos + " AND mat=" + matricola;
                dettagliCarriera.detCar.prot = this.objDataAccess.Get1ValueFromSQL(strSQL3, CommandType.Text);
                string str2 = DateTime.Parse(datini).ToString("yyyy-MM-dd");
                DataSet dataSet5 = this.objDataAccess.GetDataSet("SELECT *FROM Stordl WHERE CODPOS =" + codpos + " AND mat=" + matricola + " AND datini = '" + str2 + "'", ref Err);
                string strSQL4 = "SELECT DENTIPRAP FROM CONTCONSIP.TIPRAP WHERE TIPRAP = " + Convert.ToInt32(dataSet5.Tables[0].Rows[0]["TIPRAP"]);
                dettagliCarriera.detCar.tipRap = this.objDataAccess.Get1ValueFromSQL(strSQL4, CommandType.Text).Trim();
                dettagliCarriera.detCar.livello = dataSet5.Tables[0].Rows[0]["DENLIV"].ToString().Trim();
                int codLiv = Convert.ToInt32(dataSet5.Tables[0].Rows[0]["CODLIV"]);
                dettagliCarriera.detCar.numMes = dataSet5.Tables[0].Rows[0]["NUMMEN"].ToString().Trim();
                dettagliCarriera.detCar.m14 = dataSet5.Tables[0].Rows[0]["MESMEN14"].ToString().Trim();
                dettagliCarriera.detCar.m15 = dataSet5.Tables[0].Rows[0]["MESMEN15"].ToString().Trim();
                dettagliCarriera.detCar.m16 = dataSet5.Tables[0].Rows[0]["MESMEN16"].ToString().Trim();
                dettagliCarriera.detCar.aliquota = dataSet5.Tables[0].Rows[0]["ALIQUOTA"].ToString().Trim();
                var codQuaCon = dataSet5.Tables[0].Rows[0]["CODQUACON"].ToString();
                if (!string.IsNullOrWhiteSpace(codQuaCon))
                {
                    string strSQL5 = " SELECT DENQUA FROM QUACON WHERE CODQUACON =" + Convert.ToInt32(codQuaCon).ToString();
                    dettagliCarriera.detCar.qualifica = this.objDataAccess.Get1ValueFromSQL(strSQL5, CommandType.Text);
                }
                string strSQL6 = "SELECT dengruass FROM GRUASS g WHERE CODGRUASS = " + Convert.ToInt32(dataSet5.Tables[0].Rows[0]["CODGRUASS"]).ToString();
                dettagliCarriera.detCar.aliqCont = this.objDataAccess.Get1ValueFromSQL(strSQL6, CommandType.Text).Trim();
                dettagliCarriera.detCar.assCon = dataSet5.Tables[0].Rows[0]["ASSCON"].ToString().Trim() == "S";
                DateTime now = DateTime.Parse(dettagliCarriera.detCar.datIsc);
                bool blnStordlCorrente = true;
                string strDataMinCon;
                if (!string.IsNullOrEmpty(datini))
                {
                    blnStordlCorrente = false;
                    strDataMinCon = "";
                }
                else
                {
                    now = DateTime.Now;
                    strDataMinCon = now.ToString("d");
                }
                DataTable datiCompletiStordl = this.GetDatiCompleti_STORDL(Convert.ToInt32(codpos), Convert.ToInt32(matricola), Convert.ToInt32(prorap), datini, strDataMinCon, blnStordlCorrente: blnStordlCorrente);
                string str3 = "";
                str3 = string.IsNullOrEmpty(Convert.ToString(datiCompletiStordl.Rows[0]["CODLOC"]).Trim()) ? Convert.ToString(datiCompletiStordl.Rows[0]["CODCON"]) + "-0" : Convert.ToString(datiCompletiStordl.Rows[0]["CODCON"]) + "-" + Convert.ToString(datiCompletiStordl.Rows[0]["CODLOC"]);
                string strSQL7 = Convert.ToInt32(datiCompletiStordl.Rows[0]["CODLOC"]) != 0 ? "SELECT TRIM(DENCON) AS DENCON FROM CONLOC WHERE CODLOC = " + datiCompletiStordl.Rows[0]["CODLOC"].ToString() + " ORDER BY DATINI DESC" : "SELECT TRIM(DENCON) AS DENCON FROM CONRIF WHERE CODCON = " + datiCompletiStordl.Rows[0]["CODCON"].ToString() + " AND DATINI <='" + Utile.FromStringToDateTimeToFormatString(datini) + "' ORDER BY DATINI DESC";
                dettagliCarriera.detCar.contratto = this.objDataAccess.Get1ValueFromSQL(strSQL7, CommandType.Text);
                dettagliCarriera.detCar.fap = (datiCompletiStordl.Rows[0]["FAP"]?.ToString() ?? "") == "S";
                if (string.IsNullOrEmpty(datiCompletiStordl.Rows[0]["CODGRUASS"].ToString()))
                {
                    datiCompletiStordl.Rows[0]["CODGRUASS"].ToString();
                    if (datiCompletiStordl.Rows[0]["CODGRUASS"] != null)
                    {
                        dettagliCarriera.detCar.aliqCont = Convert.ToDecimal(datiCompletiStordl.Rows[0]["CODGRUASS"]).ToString();
                        dettagliCarriera.detCar.aliquota = string.Format(datiCompletiStordl.Rows[0]["ALIQUOTA"].ToString() + datiCompletiStordl.Rows[0]["VALFAP"]?.ToString());
                    }
                }
                string str4 = datiCompletiStordl.Rows[0]["CODCON"].ToString();
                dettagliCarriera.detCar.codloc = datiCompletiStordl.Rows[0]["CODLOC"].ToString();
                dettagliCarriera.detCar.tipspe = datiCompletiStordl.Rows[0]["TIPSPE"].ToString();
                string str5 = str4;
                this.objDataAccess.GetDataView("SELECT CODLIV, DENLIV, ORDLIV FROM CONLIV WHERE CODCON = " + str5 + " AND " + "PROCON = (SELECT MAX(PROCON) FROM CONRIF WHERE CODCON = " + str5 + ")");
                datiCompletiStordl.Rows[0]["TRAECO"].ToString();
                datiCompletiStordl.Rows[0]["MINCON"].ToString();
                int int32_2 = Convert.ToInt32(this.objDataAccess.GetDataSet("SELECT CONRIF.*, (SELECT DENQUA FROM QUACON WHERE CODQUACON = CONRIF.CODQUACON) AS DENQUA, 0 AS CODLOC, 0 AS PROLOC FROM CONRIF " + " WHERE CODCON = " + Convert.ToInt32(datiCompletiStordl.Rows[0]["CODCON"]).ToString() + " " + " AND DATDEC <= '" + str2 + "' " + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY", ref Err).Tables[0].Rows[0]["PROCON"]);
                //Decimal minimoContrattuale = this.GetMinimoContrattuale(Convert.ToInt32(datiCompletiStordl.Rows[0]["CODCON"]), int32_2, Convert.ToInt32(datiCompletiStordl.Rows[0]["CODLOC"]), Convert.ToInt32(datiCompletiStordl.Rows[0]["PROLOC"]), int32_1, datini, Convert.ToDecimal(datiCompletiStordl.Rows[0]["PERPAR"]), 0M);
                dettagliCarriera.detCar.retribuzione = Convert.ToDecimal("0" + datiCompletiStordl.Rows[0]["TRAECO"]?.ToString()).ToString("#,##0.#0");
                CaricaContratti(datini);
                var emolumenti = AggiornaEmolumenti(codLiv.ToString(), dettagliCarriera.detCar.contratto, datini).modIsc.emolumenti;
                dettagliCarriera.detCar.emolumenti = emolumenti;
                dettagliCarriera.detCar.scattiAnz = dataSet5.Tables[0].Rows[0]["NUMSCAMAT"].ToString().Trim();
                if (!string.IsNullOrEmpty(dataSet5.Tables[0].Rows[0]["DATULTSCA"].ToString()))
                    dettagliCarriera.detCar.dataUltSc = dataSet5.Tables[0].Rows[0]["DATULTSCA"].ToString().ToDomainDateStringFormat();
                if (!string.IsNullOrEmpty(dataSet5.Tables[0].Rows[0]["DATNEWSCA"].ToString()))
                    dettagliCarriera.detCar.prossimoSc = dataSet5.Tables[0].Rows[0]["DATNEWSCA"].ToString().ToDomainDateStringFormat();
                dettagliCarriera.detCar.importoSc = dataSet5.Tables[0].Rows[0]["IMPSCAMAT"].ToString().Trim();

                if (HasDaAl(Convert.ToInt32(dataSet5.Tables[0].Rows[0]["TIPRAP"])))
                {
                    string datiniFirstDaAlSqlQuery = "SELECT DATINI FROM STORDL WHERE CODPOS = " + codpos + " AND MAT = " + matricola + "  AND PRORAP = '" + prorap + "' ORDER BY DATFIN DESC";
                    var datiniFirstDaAl = objDataAccess.GetDataTable(datiniFirstDaAlSqlQuery);
                    str2 = datiniFirstDaAl.Rows[2].DateElementAt("DATINI");
                }

                var dataSet6 = this.objDataAccess.GetDataTable("SELECT VALUE(IMPAGG, 0.00) AS IMPAGG12 FROM IMPAGG WHERE CODPOS = " + codpos + " AND MAT = " + matricola + "  AND DATINI = '" + str2 + "' AND MENAGG = 12");
                var dataSet7 = this.objDataAccess.GetDataTable("SELECT VALUE(IMPAGG, 0.00) AS IMPAGG13 FROM IMPAGG WHERE CODPOS = " + codpos + " AND MAT = " + matricola + "  AND DATINI = '" + str2 + "' AND MENAGG = 13");
                var dataSet8 = this.objDataAccess.GetDataTable("SELECT VALUE(IMPAGG, 0.00) AS IMPAGG14 FROM IMPAGG WHERE CODPOS = " + codpos + " AND MAT = " + matricola + "  AND DATINI = '" + str2 + "' AND MENAGG = 14");
                var dataSet9 = this.objDataAccess.GetDataTable("SELECT VALUE(IMPAGG, 0.00) AS IMPAGG15 FROM IMPAGG WHERE CODPOS = " + codpos + " AND MAT = " + matricola + "  AND DATINI = '" + str2 + "' AND MENAGG = 15");
                var dataSet10 = this.objDataAccess.GetDataTable("SELECT VALUE(IMPAGG, 0.00) AS IMPAGG16 FROM IMPAGG WHERE CODPOS = " + codpos + " AND MAT = " + matricola + "  AND DATINI = '" + str2 + "' AND MENAGG = 16");
                dettagliCarriera.detCar.S12 = dataSet6.Rows.Count <= 0 ? "0,00" : dataSet6.Rows[0]["IMPAGG12"].ToString().Trim();
                dettagliCarriera.detCar.S13 = dataSet7.Rows.Count <= 0 ? "0,00" : dataSet7.Rows[0]["IMPAGG13"].ToString().Trim();
                dettagliCarriera.detCar.S14 = dataSet8.Rows.Count <= 0 ? "0,00" : dataSet8.Rows[0]["IMPAGG14"].ToString().Trim();
                dettagliCarriera.detCar.S15 = dataSet9.Rows.Count <= 0 ? "0,00" : dataSet9.Rows[0]["IMPAGG15"].ToString().Trim();
                dettagliCarriera.detCar.S16 = dataSet10.Rows.Count <= 0 ? "0,00" : dataSet10.Rows[0]["IMPAGG16"].ToString().Trim();
                dettagliCarriera.detCar.totaleS = (Convert.ToDecimal(dettagliCarriera.detCar.emolumenti) + Convert.ToDecimal(dettagliCarriera.detCar.importoSc) + Convert.ToDecimal(dettagliCarriera.detCar.S12) + Convert.ToDecimal(dettagliCarriera.detCar.S13) + Convert.ToDecimal(dettagliCarriera.detCar.S14) + Convert.ToDecimal(dettagliCarriera.detCar.S15) + Convert.ToDecimal(dettagliCarriera.detCar.S16) + Convert.ToDecimal(dettagliCarriera.detCar.retribuzione)).ToString("#,##0.#0");
                DataTable datiCompletiRaplav = this.GetDatiCompleti_RAPLAV(matricola, prorap, codpos);
                if (datiCompletiRaplav.Rows.Count > 0)
                {
                    dettagliCarriera.detCar.pozioneAZ = datiCompletiRaplav.Rows[0]["CODPOSFOR"]?.ToString() ?? "";
                    dettagliCarriera.detCar.ragsocAZ = datiCompletiRaplav.Rows[0]["RAGSOCFOR"]?.ToString() ?? "";
                    dettagliCarriera.detCar.cfAz = datiCompletiRaplav.Rows[0]["CODFISFOR"]?.ToString() ?? "";
                    dettagliCarriera.detCar.piAZ = datiCompletiRaplav.Rows[0]["PARIVAFOR"]?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(datiCompletiRaplav.Rows[0]["CODDUGFOR"].ToString()) && datiCompletiRaplav.Rows[0]["CODDUGFOR"] != null)
                    {
                        string denDugForQuery =
                            $"SELECT DENDUG FROM DUG WHERE CODDUG = {datiCompletiRaplav.Rows[0]["CODDUGFOR"]}";
                        string denDug = objDataAccess.Get1ValueFromSQL(denDugForQuery, CommandType.Text);
                        dettagliCarriera.detCar.residenzaAZ = denDug;
                    }

                    dettagliCarriera.detCar.indirizzoAZ = datiCompletiRaplav.Rows[0]["INDFOR"]?.ToString() ?? "";
                    dettagliCarriera.detCar.numCivAZ = datiCompletiRaplav.Rows[0]["NUMCIVFOR"]?.ToString() ?? "";
                    dettagliCarriera.detCar.comuneAZ = datiCompletiRaplav.Rows[0]["DENCOMFOR"]?.ToString() ?? "";
                    dettagliCarriera.detCar.comuneAZCod = datiCompletiRaplav.Rows[0]["CODCOMFOR"]?.ToString() ?? "";
                    dettagliCarriera.detCar.capAZ = datiCompletiRaplav.Rows[0]["CAPFOR"]?.ToString() ?? "";
                    dettagliCarriera.detCar.provinciaAZ = datiCompletiRaplav.Rows[0]["SIGPROFOR"]?.ToString() ?? "";
                    dettagliCarriera.detCar.localitaAZ = datiCompletiRaplav.Rows[0]["DENLOCFOR"]?.ToString() ?? "";
                    dettagliCarriera.detCar.hdnProt = datiCompletiRaplav.Rows[0]["PROTISC"]?.ToString() ?? "";
                }
                dettagliCarriera.detCar.emolumenti = emolumenti.ToDomainMoneyFormat();
                dettagliCarriera.detCar.totaleS = dettagliCarriera.detCar.totaleS.ToDomainMoneyFormat();
                dettagliCarriera.detCar.importoSc = dettagliCarriera.detCar.importoSc.ToDomainMoneyFormat();
                dettagliCarriera.detCar.S12 = dettagliCarriera.detCar.S12.ToDomainMoneyFormat();
                dettagliCarriera.detCar.S13 = dettagliCarriera.detCar.S13.ToDomainMoneyFormat();
                dettagliCarriera.detCar.S14 = dettagliCarriera.detCar.S14.ToDomainMoneyFormat();
                dettagliCarriera.detCar.S15 = dettagliCarriera.detCar.S15.ToDomainMoneyFormat();
                dettagliCarriera.detCar.S16 = dettagliCarriera.detCar.S16.ToDomainMoneyFormat();
                dettagliCarriera.detCar.codTipRap = dataSet5.Tables[0].Rows[0]["TIPRAP"].ToString();
                var datTerm = datiCompletiStordl.Rows[0]["DATSCATER"].ToString();

                dettagliCarriera.detCar.datTerm = string.IsNullOrWhiteSpace(datTerm)
                    ? datiCompletiStordl.Rows[0].DateElementAt("DATFIN")
                    : datiCompletiStordl.Rows[0].DateElementAt("DATSCATER");
                dettagliCarriera.detCar.datIni = Utile.FromStringToDateTimeToFormatString(datini);
                dettagliCarriera.detCar.PercPT = datiCompletiStordl.Rows[0]["PERPAR"].ToString().Replace(',', '.');
                dettagliCarriera.detCar.perce = datiCompletiStordl.Rows[0]["PERAPP"].ToString().Replace(',', '.');

                dettagliCarriera.detCar.meseSel = new List<string>();
                var mesiRows = objDataAccess.GetDataTable($"SELECT PARMES FROM PARTIMM WHERE MAT = {matricola} AND CODPOS = {codpos} AND PRORAP = {prorap} AND DATINI = (SELECT MAX(DATINI) FROM PARTIMM WHERE MAT = {matricola} AND CODPOS = {codpos} AND PRORAP = {prorap})").Rows;
                for (int i = 0; i < mesiRows.Count; i++)
                {
                    dettagliCarriera.detCar.meseSel.Add(mesiRows[i]["PARMES"].ToString());
                }

                string strSQL = "SELECT NATGIU FROM AZI WHERE CODPOS = " + codpos;
                var natgiu = Convert.ToInt16("0" + this.objDataAccess.Get1ValueFromSQL(strSQL, CommandType.Text));
                HttpContext.Current.Items["IsAziSomm"] = natgiu == 27;

                return dettagliCarriera;
            }
            catch (Exception ex)
            {
                return (RapportiLavoro)null;
            }
        }

        public void btnStampa(string prot, ref string messaggio)
        {
            try
            {
                SqlConnection objConnection = new SqlConnection(Cypher.DeCryptPassword("ConnProtocollo"));
                SqlCommand objCommand = new SqlCommand();
                SqlDataAdapter objDataAdapter = new SqlDataAdapter();
                DataTable dtTable = new DataTable();
                ClsProtocolloDll clsProtocolloDll = new ClsProtocolloDll();
                if (!string.IsNullOrEmpty(prot))
                {
                    string strSql = "SELECT A.SOTTOCARTELLA AS CARTELLA , A.NOMEFILE AS [NOME FILE] FROM TBALLEGATI A, TBPROTOCOLLIALLEGATI B WHERE A.IDALLEGATO=B.IDALLEGATO AND B.IDPROTOCOLLO =" + prot.Split(';')[0];
                    DataTable dataTable = clsProtocolloDll.SqlClient_GetDataTable(strSql, CommandType.Text, (string)null, ref objConnection, ref dtTable, ref objDataAdapter, ref objCommand);
                    clsProtocolloDll.FTP_Download("ftp://192.168.1.6/SCANSIONI/" + dataTable.Rows[0]["CARTELLA"]?.ToString() + dataTable.Rows[0]["NOME FILE"]?.ToString());
                }
                else
                    messaggio = "Connessione al protocollo non autorizzata";
            }
            catch (Exception ex)
            {
                messaggio = "Connessione al protocollo non autorizzata";
            }
        }

        private bool Check_Date(
          string dataDa,
          string dataAl,
          string matricola,
          string prorap,
          string codpos,
          TFI.OCM.Utente.Utente u,
          string codsos,
          string prosos,
          ref string messaggio)
        {
            string str1 = "";
            string str2 = "";
            bool flag = true;
            DataTable dataTable = this.objDataAccess.GetDataTable("SELECT * FROM RAPLAV  WHERE MAT = " + matricola + " AND '" + DBMethods.Db2Date(dataAl) + "' BETWEEN DATDEC AND VALUE(DATCES,'2100-01-01')  AND '" + DBMethods.Db2Date(dataDa) + "' " + "BETWEEN DATDEC AND VALUE(DATCES,'2100-01-01') AND PRORAP = " + prorap + " AND CODPOS = " + codpos);
            if (dataTable.Rows.Count == 0)
            {
                messaggio = "Il periodo immesso deve essere compreso tra la data di iscrizione e quella di cessazione del rapporto!";
                flag = false;
            }
            if (flag)
            {
                if (Convert.ToInt16("0" + this.objDataAccess.Get1ValueFromSQL("SELECT COUNT(*) FROM SOSRAP " + " WHERE MAT = " + matricola + " AND SOSRAP.CODPOS <> " + codpos + " AND STASOS  = '0'", CommandType.Text)) > (short)0 && Convert.ToInt16("0" + this.objDataAccess.Get1ValueFromSQL("SELECT COUNT(*) FROM SOSRAP " + " WHERE MAT = " + matricola + " AND SOSRAP.CODSOS = " + codsos + " AND '" + DBMethods.Db2Date(dataDa) + "' = DATINISOS " + " AND '" + DBMethods.Db2Date(dataAl) + "' = DATFINSOS " + " AND STASOS  = '0'", CommandType.Text)) == (short)0 && Convert.ToInt16("0" + this.objDataAccess.Get1ValueFromSQL("SELECT COUNT(*) FROM SOSRAP," + "CODSOS WHERE CODSOS.CODSOS = SOSRAP.CODSOS AND MAT = " + matricola + " AND '" + DBMethods.Db2Date(dataDa) + "' = DATINISOS AND '" + DBMethods.Db2Date(dataAl) + "' = DATFINSOS AND STASOS  = '0'" + " AND SOSRAP.CODPOS <> " + codpos, CommandType.Text)) != (short)1 && Convert.ToInt16("0" + this.objDataAccess.Get1ValueFromSQL("SELECT COUNT(*) FROM SOSRAP," + "CODSOS WHERE CODSOS.CODSOS = SOSRAP.CODSOS AND MAT = " + matricola + " AND '" + DBMethods.Db2Date(dataAl) + "' >= DATINISOS AND '" + DBMethods.Db2Date(dataDa) + "' <= DATFINSOS AND STASOS  = '0'" + " AND SOSRAP.CODPOS <> " + codpos, CommandType.Text)) > (short)0)
                {
                    messaggio = "Attenzione! Il periodo immesso non e' coerente con sospensioni gia' presenti in archivio per altri rapporti. \n";
                    flag = false;
                }
                string strSQL = "SELECT SOSRAP.DATINISOS AS DAL, SOSRAP.DATFINSOS AS AL, CODSOS.DENSOS FROM SOSRAP," + "CODSOS WHERE CODSOS.CODSOS = SOSRAP.CODSOS AND MAT = " + matricola + " AND '" + DBMethods.Db2Date(dataAl) + "' >= DATINISOS AND '" + DBMethods.Db2Date(dataDa) + "' <= DATFINSOS AND STASOS  = '0'" + " AND SOSRAP.CODPOS = " + codpos;
                if (!string.IsNullOrEmpty(prosos))
                    strSQL = strSQL + " AND (CODPOS <> " + codpos + " AND MAT <> " + matricola + " AND PRORAP <> " + prorap + " AND PROSOS <> " + prosos + ")";
                dataTable.Rows.Clear();
                dataTable = this.objDataAccess.GetDataTable(strSQL);
                int num = dataTable.Rows.Count - 1;
                int index;
                for (index = 0; index <= num; ++index)
                {
                    str1 = dataTable.Rows[index]["DAL"].ToString();
                    str2 = dataTable.Rows[index]["AL"].ToString();
                }
                switch (index)
                {
                    case 0:
                        break;
                    case 1:
                        if (DateTime.Compare(Convert.ToDateTime(dataDa), Convert.ToDateTime(str1)) != 0 & DateTime.Compare(Convert.ToDateTime(dataAl), Convert.ToDateTime(str2)) != 0)
                        {
                            messaggio = "Il periodo immesso e' gia' presente in archivio: \n";
                            flag = false;
                            break;
                        }
                        messaggio = "Il periodo immesso impatta periodi esistenti: \n";
                        flag = false;
                        break;
                    default:
                        messaggio = "Il periodo immesso e' gia' presente in archivio: \n";
                        flag = false;
                        break;
                }
            }
            dataTable.Dispose();
            return flag;
        }

        private bool Check_Periodo_Sospensione(
          string dataDa,
          string dataAl,
          string matricola,
          string prorap,
          string codpos,
          TFI.OCM.Utente.Utente u,
          string codsos,
          ref string messaggio)
        {
            bool flag = true;
            int num1 = 0;
            int num2 = 0;
            DataTable dataTable = this.objDataAccess.GetDataTable("SELECT MAXMES, MAXGG FROM CODSOS WHERE CODSOS = " + codsos);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dataTable.Rows[0]["MAXMES"].ToString()))
                    num1 = Convert.ToInt32(dataTable.Rows[0]["MAXMES"]);
                if (!string.IsNullOrEmpty(dataTable.Rows[0]["MAXGG"].ToString()))
                    num2 = Convert.ToInt32(dataTable.Rows[0]["MAXGG"]);
            }
            DateTime dateTime;
            if (num1 > 0)
            {
                dateTime = DateTime.Parse(dataAl);
                if (DateTime.Compare(dateTime.AddMonths(num1 * -1), DateTime.Parse(dataDa)) > 0)
                {
                    flag = false;
                    messaggio = "La durata della sospensione non pu  superare";
                    messaggio = num1 <= 1 ? messaggio + " " + num1.ToString() + " mese!" : messaggio + " i " + num1.ToString() + " mesi!";
                }
            }
            if (num2 > 0)
            {
                dateTime = DateTime.Parse(dataAl);
                TimeSpan timeSpan = dateTime.Subtract(DateTime.Parse(dataDa));
                if (num2 < timeSpan.Days)
                {
                    flag = false;
                    messaggio = "La durata della sospensione non può superare";
                    messaggio = num2 <= 1 ? messaggio + " " + num2.ToString() + " giorno!" : messaggio + " i " + num2.ToString() + " giorni!";
                }
            }
            if (codsos == "2" && DateTime.Parse(dataDa) >= DateTime.Parse("01/07/2005"))
            {
                messaggio = "Il servizio di leva non viene più effettuato dal 01/07/2005!";
                flag = false;
            }
            if (flag)
            {
                string str1 = "SELECT COUNT(*) FROM DENTES WHERE CODPOS = " + codpos + " AND ((ANNDEN = ";
                dateTime = DateTime.Parse(dataAl);
                string str2 = dateTime.Year.ToString();
                dateTime = DateTime.Parse(dataDa);
                int num3 = dateTime.Month;
                string str3 = num3.ToString();
                string str4 = str1 + str2 + " AND MESDEN = " + str3;
                dateTime = DateTime.Parse(dataAl);
                num3 = dateTime.Year;
                string str5 = num3.ToString();
                string str6 = str4 + ") OR (ANNDEN = " + str5 + " AND MESDEN = ";
                dateTime = DateTime.Parse(dataAl);
                num3 = dateTime.Month;
                string str7 = num3.ToString();
                if (Convert.ToInt16("0" + this.objDataAccess.Get1ValueFromSQL(str6 + str7 + ")) AND TIPMOV IN ('NU', 'DP') AND NUMMOV IS NOT NULL " + "AND NUMMOVANN IS NULL", CommandType.Text)) > (short)0)
                {
                    messaggio = "Per il periodo indicato è già presente una denuncia!";
                    flag = false;
                }
            }
            int num4 = flag ? 1 : 0;
            return flag;
        }

        public DataTable GetDatiCompleti_RAPLAV(
          string matricola,
          string prorap,
          string codpos)
        {
            DataTable dataTable = new DataTable();
            return this.objDataAccess.GetDataTable("SELECT CODPOS, (SELECT RAGSOC FROM AZI WHERE AZI.CODPOS = RAPLAV.CODPOS) AS RAGSOC, MAT, PRORAP, " + " DATDEC, VALUE(CODCAUCES, 0) AS CODCAUCE, DATCES, " + " (SELECT DENCES FROM CAUCES WHERE CAUCES.CODCAUCES = VALUE(RAPLAV.CODCAUCES, 0)) AS DENCES, " + " DATDENCES, DATASS, DATDEN, DATPRE, DATPRO, NUMPRO, MATFON, PROFON, ULTAGG, UTEAGG, " + " CODPOSFOR, RAGSOCFOR, PARIVAFOR, CODFISFOR, INDFOR, TELFOR, " + " CODDUGFOR, (SELECT DENDUG FROM DUG WHERE DUG.CODDUG = VALUE(RAPLAV.CODDUGFOR, 0)) AS DENDUGFOR, " + " NUMCIVFOR, CODCOMFOR, DENCOMFOR, CAPFOR, SIGPROFOR, DENLOCFOR, DATLIQ, DATPAG, DATSAP, DATINPS, PROTISC, PROTCES " + " FROM RAPLAV " + " WHERE CODPOS = " + codpos + " AND MAT = " + matricola + " AND PRORAP = " + prorap);
        }

        public Decimal GetMinimoContrattuale(
          int CODCON,
          int PROCON,
          int CODLOC,
          int PROLOC,
          int CODLIV,
          string strData,
          Decimal PERPAR,
          Decimal PERAPP)
        {
            DataLayer dataLayer = new DataLayer();
            string strSQL1 = "SELECT VALUE(SUM(IMPVOCRET), 0) AS EMOLUMENTI FROM CONRET " + " WHERE CODCON = " + CODCON.ToString() + " AND PROCON = " + PROCON.ToString() + " AND CODLIV = " + CODLIV.ToString() + " AND CODVOCRET <> 4 AND " + " DATAPPINI <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + " AND " + " VALUE(DATAPPFIN, '9999-12-31') >= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData));
            Decimal minimoContrattuale = Convert.ToDecimal("0" + dataLayer.Get1ValueFromSQL(strSQL1, CommandType.Text));
            if (CODLOC > 0)
            {
                string strSQL2 = "SELECT VALUE(SUM(IMPVOCRET), 0) AS EMOLUMENTI FROM LOCRET " + " WHERE CODLOC = " + CODLOC.ToString() + " AND PROLOC = " + PROLOC.ToString() + " AND CODLIV = " + CODLIV.ToString() + " AND CODVOCRET <> 4 AND " + " DATAPPINI <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + " AND " + " VALUE(DATAPPFIN, '9999-12-31') >= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData));
                minimoContrattuale += Convert.ToDecimal("0" + dataLayer.Get1ValueFromSQL(strSQL2, CommandType.Text));
            }
            if (PERAPP > 0M)
                minimoContrattuale = minimoContrattuale * PERAPP / 100M;
            if (PERPAR > 0M)
                minimoContrattuale = minimoContrattuale * PERPAR / 100M;
            return minimoContrattuale;
        }

        public DataTable GetDatiCompleti_STORDL(
          int CODPOS,
          int MAT,
          int PRORAP,
          string strData = "",
          string strDataMinCon = "",
          string strOrdinamento = "DESC",
          bool blnStordlCorrente = false)
        {
            DataTable dataTable1 = new DataTable();
            string s1 = this.objDataAccess.Get1ValueFromSQL("SELECT DATNAS FROM ISCT WHERE MAT = " + MAT.ToString(), CommandType.Text);
            this.objDataAccess.Get1ValueFromSQL("SELECT DATDEC FROM RAPLAV WHERE CODPOS = " + CODPOS.ToString() + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString(), CommandType.Text);
            string str1 = "SELECT STORDL.CODPOS, STORDL.MAT, STORDL.PRORAP, " + " STORDL.DATINI, STORDL.DATFIN, " + " STORDL.TIPRAP, (SELECT DENTIPRAP FROM TIPRAP WHERE TIPRAP = STORDL.TIPRAP) AS DENTIPRAP, " + " STORDL.CODLIV, '' as DENLIV, " + " '' AS DENCON, " + " VALUE(STORDL.CODCON, 0) AS CODCON, 0 AS PROCON, " + " VALUE(STORDL.CODLOC, 0) AS CODLOC, 0 AS PROLOC, " + " 0.00 AS MINCON, " + " 0 AS CODQUACON, '' AS DENQUA, " + " '' AS TIPSPE, " + " '' AS DENTIPSPE, " + " VALUE(STORDL.TRAECO,0.00) AS TRAECO, " + " STORDL.NUMMEN, " + " 0.00 AS IMPAGG12, " + " 0.00 AS IMPAGG13, " + " 0.00 AS IMPAGG14, " + " STORDL.MESMEN14, " + " 0.00 AS IMPAGG15, " + " STORDL.MESMEN15, " + " 0.00 AS IMPAGG16, " + " STORDL.MESMEN16, " + " STORDL.PERAPP, " + " STORDL.PERPAR, " + " STORDL.DATSCATER, " + " STORDL.FAP, " + " 0.00 AS VALFAP, " + " CODGRUASS, " + " '' AS DENGRUASS, " + " 0.00 AS ALIQUOTA, " + " STORDL.NUMSCAMAT, " + " STORDL.DATULTSCA, " + " STORDL.DATNEWSCA, " + " STORDL.IMPSCAMAT, " + " 0.00 AS RETTOT, " + " ABBPRE, " + " ASSCON, " + " STORDL.ULTAGG, STORDL.UTEAGG" + " FROM STORDL " + " WHERE CODPOS = " + CODPOS.ToString() + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString();
            if (blnStordlCorrente)
                str1 += " AND CURRENT_DATE BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31') ";
            else if (!string.IsNullOrEmpty(strData))
                str1 = str1 + " AND '" + DBMethods.Db2Date(strData) + "' BETWEEN DATINI AND " + "VALUE(DATFIN, '9999-12-31')";
            DataTable dataTable2 = this.objDataAccess.GetDataTable(str1 + " ORDER BY STORDL.DATINI " + strOrdinamento);
            DataTable dataTable3 = new DataTable();
            int num = dataTable2.Rows.Count - 1;
            for (int index1 = 0; index1 <= num; ++index1)
            {
                if (string.IsNullOrEmpty(dataTable2.Rows[index1]["TIPRAP"].ToString()))
                    dataTable2.Rows[index1]["TIPRAP"] = (object)0;
                for (int index2 = 12; index2 <= 16; ++index2)
                {
                    string strSQL = "SELECT VALUE(IMPAGG, 0.00) AS IMPAGG FROM IMPAGG WHERE CODPOS = " + CODPOS.ToString() + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString() + " AND DATINI = " + " '" + DBMethods.Db2Date(dataTable2.Rows[index1]["DATINI"].ToString()) + "' AND MENAGG = ' " + index2.ToString() + " ' ";
                    dataTable3.Clear();
                    dataTable3 = this.objDataAccess.GetDataTable(strSQL);
                    switch (dataTable3.Rows.Count)
                    {
                        case 0:
                            dataTable2.Rows[index1]["IMPAGG" + index2.ToString()] = (object)0.0;
                            break;
                        case 1:
                            dataTable2.Rows[index1]["IMPAGG" + index2.ToString()] = dataTable3.Rows[0]["IMPAGG"];
                            break;
                        default:
                            return dataTable2;
                    }
                }
                string str2 = dataTable2.Rows[index1]["TIPRAP"].ToString().Trim() ?? "";
                if (str2 == "0" || str2 == "")
                    return dataTable2;
                var x = dataTable2.Rows[index1]["CODLOC"].ToString();
                dataTable3 = Convert.ToInt32(dataTable2.Rows[index1]["CODLOC"]) != 0 ?
                                        (!string.IsNullOrEmpty(strDataMinCon) ?
                                            this.GetDatiContrattoLocale(Convert.ToInt32(dataTable2.Rows[index1]["CODLOC"]), strDataMinCon)
                                            : this.GetDatiContrattoLocale(Convert.ToInt32(dataTable2.Rows[index1]["CODLOC"]), Convert.ToString(dataTable2.Rows[index1]["DATINI"])))
                                                    : (!string.IsNullOrEmpty(strDataMinCon) ?
                                                            this.GetDatiContrattoRiferimento(Convert.ToInt32(dataTable2.Rows[index1]["CODCON"]), strDataMinCon)
                                                                : this.GetDatiContrattoRiferimento(Convert.ToInt32(dataTable2.Rows[index1]["CODCON"]), Convert.ToString(dataTable2.Rows[index1]["DATINI"])));
                if (dataTable3.Rows.Count > 0)
                {
                    dataTable2.Rows[index1]["DENQUA"] = (object)dataTable3.Rows[0]["DENQUA"].ToString().Trim();
                    dataTable2.Rows[index1]["DENCON"] = (object)dataTable3.Rows[0]["DENCON"].ToString().Trim();
                    dataTable2.Rows[index1]["PROCON"] = (object)dataTable3.Rows[0]["PROCON"].ToString().Trim();
                    dataTable2.Rows[index1]["CODLOC"] = (object)dataTable3.Rows[0]["CODLOC"].ToString().Trim();
                    dataTable2.Rows[index1]["PROLOC"] = (object)dataTable3.Rows[0]["PROLOC"].ToString().Trim();
                    dataTable2.Rows[index1]["CODQUACON"] = (object)dataTable3.Rows[0]["CODQUACON"]?.ToString().Trim();
                    dataTable2.Rows[index1]["TIPSPE"] = (object)dataTable3.Rows[0]["TIPSPE"].ToString().Trim();
                    string str3 = dataTable3.Rows[0]["TIPSPE"].ToString().Trim() ?? "";
                    if (!(str3 == "S"))
                    {
                        if (!(str3 == "M"))
                        {
                            if (str3 == "A")
                                dataTable2.Rows[index1]["DENTIPSPE"] = (object)"CONTRATTO AZIENDALE";
                        }
                        else
                            dataTable2.Rows[index1]["DENTIPSPE"] = (object)"AREA MINIMALE";
                    }
                    else
                        dataTable2.Rows[index1]["DENTIPSPE"] = (object)"CONTRATTO MECCANIZZATO";
                    string strSQL1 = "SELECT DENLIV FROM CONLIV WHERE CODCON = " + Convert.ToInt32(dataTable2.Rows[index1]["CODCON"]).ToString() + " AND PROCON = " + Convert.ToInt32(dataTable2.Rows[index1]["PROCON"]).ToString() + " AND CODLIV = " + Convert.ToInt32(dataTable2.Rows[index1]["CODLIV"]).ToString();
                    dataTable3.Clear();
                    DataTable dataTable4 = this.objDataAccess.GetDataTable(strSQL1);
                    if (dataTable4.Rows.Count > 0)
                        dataTable2.Rows[index1]["DENLIV"] = dataTable4.Rows[0]["DENLIV"];
                    Decimal PERPAR = string.IsNullOrEmpty(dataTable2.Rows[index1]["PERPAR"].ToString()) ? 0M : Convert.ToDecimal(dataTable2.Rows[index1]["PERPAR"]);
                    Decimal PERAPP = string.IsNullOrEmpty(dataTable2.Rows[index1]["PERAPP"].ToString()) ? 0M : Convert.ToDecimal(dataTable2.Rows[index1]["PERAPP"]);
                    dataTable2.Rows[index1]["MINCON"] = !string.IsNullOrEmpty(strDataMinCon) ? (object)this.GetMinimoContrattuale(Convert.ToInt32(dataTable2.Rows[index1]["CODCON"]), Convert.ToInt32(dataTable2.Rows[index1]["PROCON"]), Convert.ToInt32(dataTable2.Rows[index1]["CODLOC"]), Convert.ToInt32(dataTable2.Rows[index1]["PROLOC"]), Convert.ToInt32(dataTable2.Rows[index1]["CODLIV"]), strDataMinCon, PERPAR, PERAPP) : (object)this.GetMinimoContrattuale(Convert.ToInt32(dataTable2.Rows[index1]["CODCON"]), Convert.ToInt32(dataTable2.Rows[index1]["PROCON"]), Convert.ToInt32(dataTable2.Rows[index1]["CODLOC"]), Convert.ToInt32(dataTable2.Rows[index1]["PROLOC"]), Convert.ToInt32(dataTable2.Rows[index1]["CODLIV"]), Convert.ToString(dataTable2.Rows[index1]["DATINI"]), PERPAR, PERAPP);
                    string str4 = dataTable2.Rows[index1]["TIPSPE"].ToString().Trim() ?? "";
                    if (!(str4 == "S"))
                    {
                        if (!(str4 == "M"))
                        {
                            if (str4 == "A")
                            {
                                dataTable2.Rows[index1]["MINCON"] = (object)0.0;
                                dataTable2.Rows[index1]["RETTOT"] = (object)(Convert.ToInt32(dataTable2.Rows[index1]["TRAECO"]) + Convert.ToInt32(dataTable2.Rows[index1]["IMPAGG12"]) + Convert.ToInt32(dataTable2.Rows[index1]["IMPAGG13"]) + Convert.ToInt32(dataTable2.Rows[index1]["IMPAGG14"]) + Convert.ToInt32(dataTable2.Rows[index1]["IMPAGG15"]) + Convert.ToInt32(dataTable2.Rows[index1]["IMPAGG16"]));
                            }
                        }
                        else
                            dataTable2.Rows[index1]["RETTOT"] = (object)(Convert.ToInt32(dataTable2.Rows[index1]["TRAECO"]) + Convert.ToInt32(dataTable2.Rows[index1]["IMPAGG12"]) + Convert.ToInt32(dataTable2.Rows[index1]["IMPAGG13"]) + Convert.ToInt32(dataTable2.Rows[index1]["IMPAGG14"]) + Convert.ToInt32(dataTable2.Rows[index1]["IMPAGG15"]) + Convert.ToInt32(dataTable2.Rows[index1]["IMPAGG16"]));
                    }
                    else
                    {
                        dataTable2.Rows[index1]["TRAECO"] = (object)0.0;
                        dataTable2.Rows[index1]["RETTOT"] = (object)(Convert.ToInt32(dataTable2.Rows[index1]["MINCON"]) + Convert.ToInt32(dataTable2.Rows[index1]["IMPSCAMAT"]) + Convert.ToInt32(dataTable2.Rows[index1]["IMPAGG12"]) + Convert.ToInt32(dataTable2.Rows[index1]["IMPAGG13"]) + Convert.ToInt32(dataTable2.Rows[index1]["IMPAGG14"]) + Convert.ToInt32(dataTable2.Rows[index1]["IMPAGG15"]) + Convert.ToInt32(dataTable2.Rows[index1]["IMPAGG16"]));
                    }
                    if (string.IsNullOrEmpty(dataTable2.Rows[index1]["CODGRUASS"].ToString()))
                        dataTable2.Rows[index1]["CODGRUASS"] = (object)0;
                    string str5 = "Select VALUE(SUM(ALIQUOTA), 0) AS ALIQUOTA " + " FROM ALIFORASS " + " WHERE ALIFORASS.CODGRUASS = " + dataTable2.Rows[index1]["CODGRUASS"]?.ToString() + " AND ALIFORASS.CODQUACON=" + dataTable2.Rows[index1]["CODQUACON"]?.ToString() + " AND '" + DBMethods.Db2Date(dataTable2.Rows[index1]["DATINI"].ToString()) + "' BETWEEN ALIFORASS.DATINI AND VALUE(ALIFORASS.DATFIN,'9999-12-31') ";
                    DateTime now;
                    string s2;
                    if (blnStordlCorrente)
                    {
                        now = DateTime.Now;
                        s2 = now.ToString("d");
                    }
                    else
                        s2 = dataTable2.Rows[index1]["DATINI"].ToString();
                    now = DateTime.Parse(s1);
                    if (DateTime.Compare(now.AddYears(65), DateTime.Parse(s2)) < 0)
                        str5 += " AND ALIFORASS.CODFORASS IN (SELECT CODFORASS FROM FORASS WHERE CATFORASS <> 'PREV') ";
                    string strSQL2 = str5 + " AND ALIFORASS.CODFORASS NOT IN (SELECT CODFORASS FROM FORASS WHERE CATFORASS = 'FAP') ";
                    dataTable4.Clear();
                    DataTable dataTable5 = this.objDataAccess.GetDataTable(strSQL2);
                    dataTable2.Rows[index1]["ALIQUOTA"] = dataTable5.Rows[0]["ALIQUOTA"];
                    if (Convert.ToString(dataTable2.Rows[index1]["FAP"]) != "S")
                    {
                        string strSQL3 = "SELECT VALFAP FROM CODFAP WHERE '" + DBMethods.Db2Date(dataTable2.Rows[index1]["DATINI"].ToString()) + "' BETWEEN " + "DATINI AND VALUE(DATFIN,'9999-12-31')";
                        dataTable5.Clear();
                        dataTable5 = this.objDataAccess.GetDataTable(strSQL3);
                        if (dataTable5.Rows.Count > 0)
                            dataTable2.Rows[index1]["VALFAP"] = dataTable5.Rows[0]["VALFAP"];
                    }
                    string strSQL4 = " SELECT GRUASS.CODGRUASS, GRUASS.DENGRUASS FROM GRUASS " + " WHERE GRUASS.CODGRUASS = " + dataTable2.Rows[index1]["CODGRUASS"]?.ToString();
                    dataTable5.Clear();
                    dataTable3 = this.objDataAccess.GetDataTable(strSQL4);
                    if (dataTable3.Rows.Count > 0)
                    {
                        dataTable2.Rows[index1]["CODGRUASS"] = dataTable3.Rows[0]["CODGRUASS"];
                        dataTable2.Rows[index1]["DENGRUASS"] = dataTable3.Rows[0]["DENGRUASS"];
                    }
                }
            }
            return dataTable2;
        }

        public DataTable GetDatiContrattoRiferimento(int CODCON, string strData)
        {
            DataTable dataTable = new DataTable();
            return new DataLayer().GetDataTable("SELECT CONRIF.*, (SELECT DENQUA FROM QUACON WHERE CODQUACON = " + "CONRIF.CODQUACON) AS DENQUA, 0 AS CODLOC, 0 AS PROLOC FROM CONRIF " + " WHERE CODCON = " + CODCON.ToString() + " AND DATDEC <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + " ORDER BY DATDEC DESC FETCH FIRST ROWS ONLY");
        }

        public DataTable GetDatiContrattoLocale(int CODLOC, string strData)
        {
            DataTable dataTable1 = new DataTable();
            DataTable dataTable2 = new DataTable();
            DataLayer dataLayer = new DataLayer();
            string strSQL1 = "SELECT CONLOC.*, 0 AS CODQUACON, '' as DENQUA FROM CONLOC " + " WHERE CODLOC = " + CODLOC.ToString() + " AND DATDEC <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + " ORDER BY DATDEC DESC FETCH FIRST ROWS ONLY";
            DataTable dataTable3 = dataLayer.GetDataTable(strSQL1);
            if (dataTable3.Rows.Count > 0)
            {
                string strSQL2 = "SELECT PROCON FROM CONRIF WHERE CODCON = '" + dataTable3.Rows[0]["CODCON"]?.ToString() + "' " + " AND DATDEC <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + " " + " ORDER BY DATDEC DESC FETCH FIRST ROWS ONLY";
                dataTable3.Rows[0]["PROCON"] = (object)dataLayer.Get1ValueFromSQL(strSQL2, CommandType.Text);
                string strSQL3 = "SELECT CODQUACON, DENQUA FROM QUACON WHERE CODQUACON IN " + "(SELECT CODQUACON FROM CONRIF WHERE CODCON = '" + dataTable3.Rows[0]["CODCON"]?.ToString() + "')";
                dataTable2 = dataLayer.GetDataTable(strSQL3);
                dataTable3.Rows[0]["CODQUACON"] = dataTable2.Rows[0]["CODQUACON"];
                dataTable3.Rows[0]["DENQUA"] = dataTable2.Rows[0]["DENQUA"];
            }
            dataTable2.Dispose();
            return dataTable3;
        }

        public RapportiLavoro CaricaContratti(string datIni)
        {
            RapportiLavoro rapportiLavoro = new RapportiLavoro();
            string empty = string.Empty;
            DataTable dataTable1 = new DataTable();
            int num1 = 0;
            DataLayer dataLayer = new DataLayer();
            DataTable dataTable2 = new DataTable();
            DataTable dataTable3 = new DataTable();
            rapportiLavoro.listContratti = new List<ListContratti>();
            List<Contratti> contrattiList = new List<Contratti>();
            try
            {
                string strData = string.IsNullOrEmpty(datIni) ? DateTime.Now.ToString("d") : datIni;
                string strSQL1 = " SELECT " + " CODCON, PROCON, 0 AS CODLOC, 0 AS PROLOC," + " CODQUACON," + " (SELECT DENQUA FROM QUACON WHERE QUACON.CODQUACON=CONRIF.CODQUACON) AS DENQUA," + " DATINI,DATFIN, DATDEC, ASSCON,DENCON," + " MAXSCA,PERSCA,NUMMEN,M14,M15,M16,RIVAUT," + " TIPSPE," + " '' AS RIMUOVI" + " FROM CONRIF " + " WHERE DATDEC <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + " between datdec and value(datann,'9999-12-31')" + " ORDER BY CODCON, DATDEC";
                DataTable dataTable4 = dataLayer.GetDataTable(strSQL1);
                for (int index = dataTable4.Rows.Count - 1; index >= 0; index += -1)
                {
                    if (num1 != Convert.ToInt32(dataTable4.Rows[index]["CODCON"]))
                    {
                        num1 = Convert.ToInt32(dataTable4.Rows[index]["CODCON"]);
                        dataTable4.Rows[index]["RIMUOVI"] = (object)"NO";
                    }
                    else
                        dataTable4.Rows[index]["RIMUOVI"] = (object)"SI";
                }
                for (int index = dataTable4.Rows.Count - 1; index >= 0; index += -1)
                {
                    if (dataTable4.Rows[index]["RIMUOVI"].ToString() == "SI")
                        dataTable4.Rows.Remove(dataTable4.Rows[index]);
                }
                string strSQL2 = "SELECT " + " CODCON, CODLOC, PROLOC, " + " DATINI,DATFIN,DATDEC, ASSCON,DENCON, " + " MAXSCA,PERSCA,NUMMEN,M14,M15,M16,RIVAUT, " + " TIPSPE, " + " '' AS RIMUOVI " + " FROM CONLOC " + " WHERE DATDEC <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + "  AND DATANN IS  NULL " + " ORDER BY CODLOC, DATDEC ";
                int num2 = 0;
                dataTable1 = dataLayer.GetDataTable(strSQL2);
                for (int index = dataTable1.Rows.Count - 1; index >= 0; index += -1)
                {
                    if (num2 != Convert.ToInt32(dataTable1.Rows[index]["CODLOC"]))
                    {
                        num2 = Convert.ToInt32(dataTable1.Rows[index]["CODLOC"]);
                        dataTable1.Rows[index]["RIMUOVI"] = (object)"NO";
                    }
                    else
                        dataTable1.Rows[index]["RIMUOVI"] = (object)"SI";
                }
                for (int index = dataTable1.Rows.Count - 1; index >= 0; index += -1)
                {
                    if (dataTable1.Rows[index]["RIMUOVI"].ToString() == "SI")
                        dataTable1.Rows.Remove(dataTable1.Rows[index]);
                }
                int num3 = dataTable1.Rows.Count - 1;
                for (int index1 = 0; index1 <= num3; ++index1)
                {
                    DataRow row = dataTable4.NewRow();
                    row["CODCON"] = dataTable1.Rows[index1]["CODCON"];
                    int num4 = dataTable4.Rows.Count - 1;
                    int index2;
                    for (index2 = 0; index2 <= num4; ++index2)
                    {
                        if (dataTable4.Rows[index2]["CODCON"] == dataTable1.Rows[index1]["CODCON"])
                        {
                            row["PROCON"] = dataTable4.Rows[index2]["PROCON"];
                            row["CODQUACON"] = dataTable4.Rows[index2]["CODQUACON"];
                            row["DENQUA"] = dataTable4.Rows[index2]["DENQUA"];
                            break;
                        }
                    }
                    if (index2 == dataTable4.Rows.Count)
                    {
                        string strSQL3 = "SELECT PROCON, CODQUACON, (SELECT DENQUA FROM QUACON WHERE QUACON.CODQUACON=CONRIF.CODQUACON) AS DENQUA FROM CONRIF WHERE CODCON = " + dataTable1.Rows[index1]["CODCON"]?.ToString() + " AND PROCON = (SELECT MAX(PROCON) FROM CONRIF WHERE CODCON = '" + dataTable1.Rows[index1]["CODCON"].ToString() + "') ";
                        DataTable dataTable5 = dataLayer.GetDataTable(strSQL3);
                        if (dataTable5.Rows.Count > 0)
                        {
                            row["PROCON"] = dataTable5.Rows[0]["PROCON"];
                            row["CODQUACON"] = dataTable5.Rows[0]["CODQUACON"];
                            row["DENQUA"] = dataTable5.Rows[0]["DENQUA"];
                        }
                    }
                    row["CODLOC"] = dataTable1.Rows[index1]["CODLOC"];
                    row["PROLOC"] = dataTable1.Rows[index1]["PROLOC"];
                    row["DATDEC"] = dataTable1.Rows[index1]["DATDEC"];
                    row["DATINI"] = dataTable1.Rows[index1]["DATINI"];
                    row["DATFIN"] = dataTable1.Rows[index1]["DATFIN"];
                    row["ASSCON"] = dataTable1.Rows[index1]["ASSCON"];
                    row["DENCON"] = dataTable1.Rows[index1]["DENCON"];
                    row["MAXSCA"] = dataTable1.Rows[index1]["MAXSCA"];
                    row["PERSCA"] = dataTable1.Rows[index1]["PERSCA"];
                    row["NUMMEN"] = dataTable1.Rows[index1]["NUMMEN"];
                    row["M14"] = dataTable1.Rows[index1]["M14"];
                    row["M15"] = dataTable1.Rows[index1]["M15"];
                    row["M16"] = dataTable1.Rows[index1]["M16"];
                    row["RIVAUT"] = dataTable1.Rows[index1]["RIVAUT"];
                    row["TIPSPE"] = dataTable1.Rows[index1]["TIPSPE"];
                    row["RIMUOVI"] = dataTable1.Rows[index1]["RIMUOVI"];
                    dataTable4.Rows.Add(row);
                }
                dataTable4.Columns.Add(new DataColumn("CODCONLOC", Type.GetType("System.String")));
                int num5 = dataTable4.Rows.Count - 1;
                for (int index = 0; index <= num5; ++index)
                    dataTable4.Rows[index]["CODCONLOC"] = (object)(dataTable4.Rows[index]["CODCON"]?.ToString() + "-" + dataTable4.Rows[index]["CODLOC"]?.ToString());
                DataView defaultView = dataTable4.DefaultView;
                defaultView.Sort = "DENCON";
                HttpContext.Current.Session["ContrattiView"] = (object)defaultView;
                foreach (DataRowView dataRowView in defaultView)
                {
                    Contratti contratti = new Contratti()
                    {
                        CODCON = dataRowView["CODCON"].ToString(),
                        PROCON = dataRowView["PROCON"].ToString(),
                        CODQUACON = dataRowView["CODQUACON"].ToString(),
                        DENQUA = dataRowView["DENQUA"].ToString(),
                        CODLOC = dataRowView["CODLOC"].ToString(),
                        PROLOC = dataRowView["PROLOC"].ToString(),
                        DATDEC = dataRowView["DATDEC"].ToString(),
                        DATINI = dataRowView["DATINI"].ToString(),
                        DATFIN = dataRowView["DATFIN"].ToString(),
                        ASSCON = dataRowView["ASSCON"].ToString(),
                        DENCON = dataRowView["DENCON"].ToString(),
                        MAXSCA = dataRowView["MAXSCA"].ToString(),
                        PERSCA = dataRowView["PERSCA"].ToString(),
                        NUMMEN = dataRowView["NUMMEN"].ToString(),
                        M14 = dataRowView["M14"].ToString(),
                        M15 = dataRowView["M15"].ToString(),
                        M16 = dataRowView["M16"].ToString(),
                        RIVAUT = dataRowView["RIVAUT"].ToString(),
                        TIPSPE = dataRowView["TIPSPE"].ToString(),
                        RIMUOVI = dataRowView["RIMUOVI"].ToString(),
                        CODCONLOC = dataRowView["CODCONLOC"].ToString()
                    };
                    contrattiList.Add(contratti);
                }
                rapportiLavoro.listCon = contrattiList;
                return rapportiLavoro;
            }
            catch (Exception ex)
            {
                return (RapportiLavoro)null;
            }
            finally
            {
                dataTable1?.Dispose();
            }
        }

        public RapportiLavoro CaricaLivello(string codcon, string dencon)
        {
            try
            {
                DataTable table = ((DataView)HttpContext.Current.Session["ContrattiView"]).ToTable();
                RapportiLavoro rapportiLavoro = new RapportiLavoro();
                DataLayer dataLayer = new DataLayer();
                List<Livello> livelloList = new List<Livello>();
                RapportiLavoro.DatiRapporto datiRapporto = new RapportiLavoro.DatiRapporto();
                string str1 = dencon.Trim().Replace("'", "'''+'");
                DataRow[] dataRowArray = table.Select("DENCON like '%" + str1 + "%'");
                string str2 = Convert.ToString(dataRowArray[0]["PROCON"]);
                Convert.ToString(dataRowArray[0]["ASSCON"]);
                int int32_1 = (int)(short)Convert.ToInt32(dataRowArray[0]["CODLOC"]);
                int int32_2 = (int)(short)Convert.ToInt32(dataRowArray[0]["CODCON"]);
                Convert.ToString(dataRowArray[0]["TIPSPE"]);
                datiRapporto.qualifica = Convert.ToString(dataRowArray[0]["DENQUA"]);
                datiRapporto.mensilita = Convert.ToString(dataRowArray[0]["NUMMEN"]);
                datiRapporto.m14 = Convert.ToString(dataRowArray[0]["M14"]);
                if (string.IsNullOrEmpty(datiRapporto.m14))
                    datiRapporto.m14 = "0,00";
                datiRapporto.m15 = Convert.ToString(dataRowArray[0]["M15"]);
                if (string.IsNullOrEmpty(datiRapporto.m15))
                    datiRapporto.m15 = "0,00";
                datiRapporto.m16 = Convert.ToString(dataRowArray[0]["M16"]);
                if (string.IsNullOrEmpty(datiRapporto.m16))
                    datiRapporto.m16 = "0,00";
                string strSQL = "SELECT CODLIV, DENLIV FROM CONLIV WHERE CODCON = " + codcon + " AND PROCON = " + str2 + " ORDER BY ORDLIV";
                foreach (DataRow row in (InternalDataCollectionBase)dataLayer.GetDataTable(strSQL).Rows)
                {
                    Livello livello = new Livello()
                    {
                        codliv = row["CODLIV"].ToString(),
                        denliv = row["DENLIV"].ToString()
                    };
                    livelloList.Add(livello);
                }
                rapportiLavoro.listLiv = livelloList;
                rapportiLavoro.datRap.qualifica = datiRapporto.qualifica;
                rapportiLavoro.datRap.mensilita = datiRapporto.mensilita;
                rapportiLavoro.datRap.m14 = datiRapporto.m14;
                rapportiLavoro.datRap.m15 = datiRapporto.m15;
                rapportiLavoro.datRap.m16 = datiRapporto.m16;
                return rapportiLavoro;
            }
            catch (Exception ex)
            {
                return (RapportiLavoro)null;
            }
        }

        public (int Matricola, bool Result) WriteData(
          TFI.OCM.Utente.Utente u,
          int codpos,
          int intProRap,
          NuovoIscritto nuovoIscritto,
          ref string messaggio,
          ref string SuccessMSG)
        {
            bool isUpdate = intProRap != 0;
            if (CheckDataDecorrenzaModifica(isUpdate, nuovoIscritto.datDecMod, nuovoIscritto.datIsc, DateTime.Now.Date))
            {
                messaggio = "La data di decorrenza modifica non può essere antecedente alla data di iscrizione e successiva alla data odierna.";
                return (0, false);
            }
            if (!IsValidRangeDate(nuovoIscritto.datIsc, nuovoIscritto.datTerm))
            {
                messaggio = "La data di termine non può essere antecedente alla data di iscrizione.";
                return (0, false);
            }
            if (!IsCambioRapportoLavoroConsentito(nuovoIscritto.RapportoDiLavoroCorrente, nuovoIscritto.tipRap))
            {
                messaggio = "Non è consentito un cambio peggiorativo del rapporto di lavoro.";
                return (0, false);
            }

            string datini = isUpdate ? nuovoIscritto.datDecMod : nuovoIscritto.datIsc;//DateTime.Now.ToString("d");
            string strDatFin = "31/12/9999";
            bool flag1 = false;
            bool flag2 = false;
            string str2 = "";
            bool flag3 = true;
            bool flag4 = true;
            bool flag5 = true;
            bool flag6 = true;
            ClsProtocolloDll clsProtocolloDll = new ClsProtocolloDll();
            string path = "";
            string numProt = "";
            string strData = "";
            string prorap = (string)null;
            if (Convert.ToInt32(nuovoIscritto.checkIscritto) == 0)
            {
                prorap = intProRap.ToString();
                if (((RapportiLavoro)HttpContext.Current.Session["ModificaIscritto"]).modIsc.Equals((object)nuovoIscritto))
                {
                    flag3 = false;
                    flag4 = false;
                    flag5 = false;
                    flag6 = false;
                }
            }
            Decimal decPerPeriodo = 0M;
            int num1 = 1;
            int tiprap = 0;
            int intMatricola = 0;
            if (!string.IsNullOrEmpty(nuovoIscritto.codtiprap))
                tiprap = Convert.ToInt32(nuovoIscritto.codtiprap);
            if (!string.IsNullOrEmpty(nuovoIscritto.tipRap))
                tiprap = Convert.ToInt32(nuovoIscritto.tipRap);
            if (!string.IsNullOrEmpty(nuovoIscritto.matricola))
                intMatricola = Convert.ToInt32(nuovoIscritto.matricola);
            try
            {
                if (this.CheckFields(codpos.ToString(), datini, prorap, nuovoIscritto, ref messaggio))
                {
                    string queryDenCom = "SELECT DENCOM FROM CODCOM WHERE CODCOM = '" + nuovoIscritto.comuneCod + "'";
                    nuovoIscritto.comune = this.objDataAccess.Get1ValueFromSQL(queryDenCom, CommandType.Text).Trim();
                    //string strSQL2 = "SELECT DENDUG FROM DUG WHERE CODDUG = '" + nuovoIscritto.indirizzo + "'";
                    //nuovoIscritto.indirizzo = this.objDataAccess.Get1ValueFromSQL(strSQL2, CommandType.Text).Trim();

                    string strSQL3 = "SELECT DENCOM FROM CODCOM WHERE CODCOM = '" + nuovoIscritto.comuneAZCod + "'";
                    nuovoIscritto.comuneAZ = this.objDataAccess.Get1ValueFromSQL(strSQL3, CommandType.Text).Trim();
                    string strSQL4 = "SELECT DENCOM FROM CODCOM WHERE CODCOM = '" + nuovoIscritto.comuneNCod + "'";
                    nuovoIscritto.comuneN = this.objDataAccess.Get1ValueFromSQL(strSQL4, CommandType.Text).Trim();
                    string denStaEstQuery = "SELECT DENCOM FROM COM_ESTERO WHERE CODCOM = '" + nuovoIscritto.statoEs + "'";
                    nuovoIscritto.statoEs = this.objDataAccess.Get1ValueFromSQL(denStaEstQuery, CommandType.Text).Trim();
                    if (flag3 | flag4 | flag5 | flag6)
                    {
                        this.objDataAccess.StartTransaction();
                        flag2 = true;
                        new clsIDOC().strUserCode = u.Username;
                        Tuple<int, bool> tuple1 = this.WriteDatiAnagrafici(nuovoIscritto, codpos.ToString(), u, datini, ref messaggio, intMatricola, prorap);
                        intMatricola = tuple1.Item1;
                        flag1 = tuple1.Item2;
                        string str3;
                        if (flag1)
                        {
                            Tuple<int, bool> tuple2 = this.WriteDatiRapporto(intProRap, codpos, intMatricola, nuovoIscritto, prorap, u, ref messaggio, ref SuccessMSG);
                            flag1 = tuple2.Item2;
                            prorap = tuple2.Item1.ToString();
                            //IsUpdate --> intProRap != 0

                            if (flag1)
                            {
                                string str4 = "31/12/9999";
                                short num2;
                                nuovoIscritto.PercPT = nuovoIscritto.PercPT?.Replace(".", ",");
                                switch (Convert.ToInt32(nuovoIscritto.tipRap))
                                {
                                    //datfin: sempre 31/12/9999 - tranne daal daal daal
                                    //datTer: solo a quelli che hanno data termine, altrimenti null
                                    case 1: //tempo indeterminato
                                    case 15: //assunzione congiunta
                                    case 18: //SW
                                    case 20: //Apprendistato di alta formazione e ricerca
                                        flag1 = this.WriteDatiStorico(isUpdate, ref messaggio, intMatricola.ToString(), tiprap, nuovoIscritto, prorap, codpos.ToString(), u, datini, strDatFin, decPerPeriodo);
                                        break;
                                    case 2: //A termine
                                    case 5: // lavoro e formazione
                                    case 9: //contratto di inserimento
                                    case 12: // apprendistato professionalizzante
                                        flag1 = this.WriteDatiStorico(isUpdate, ref messaggio, intMatricola.ToString(), tiprap, nuovoIscritto, prorap, codpos.ToString(), u, datini, strDatFin, decPerPeriodo, strDataScadenza: nuovoIscritto.datTerm);
                                        break;
                                    case 3: //Part time
                                    case 19: //SW Part time
                                    case 21: //Apprendistato di alta formazione e ricerca %
                                        flag1 = this.WriteDatiStorico(isUpdate, ref messaggio, intMatricola.ToString(), tiprap, nuovoIscritto, prorap, codpos.ToString(), u, datini, strDatFin, decPerPeriodo, Convert.ToDecimal(nuovoIscritto.PercPT));
                                        break;
                                    case 4: //Part time a termine
                                        string strDataScadenza1 = nuovoIscritto.datTerm.ToString();
                                        flag1 = this.WriteDatiStorico(isUpdate, ref messaggio, intMatricola.ToString(), tiprap, nuovoIscritto, prorap, codpos.ToString(), u, datini, strDatFin, decPerPeriodo, Convert.ToDecimal(nuovoIscritto.PercPT), strDataScadenza1);
                                        break;
                                    case 6: //primo impiego
                                    case 7: //apprendistato
                                            //3 righe in stordl, uno per ogni periodo - datini : inizio periodo - datfin : fine periodo - perapp : percentuale del periodo (perc1 - perc2 - perc3)
                                            //perPar = percentuale del partime
                                            //short num3 = 0;
                                            //if (intProRap.ToString() == null) //inProRap == 0 - nuovo inserimento
                                            //{
                                            //    short num4 = 8;
                                            //    short num5 = 3;
                                            //    string strDataScadenza2 = nuovoIscritto.datTerm?.ToString();
                                            //    for (short index = num3; (int)index <= (int)num4; index += num5)
                                            //    {
                                            //        str4 = (int)index >= (int)num4 - 2 ? "31/12/9999" : nuovoIscritto.datTerm?.ToString();
                                            //        decPerPeriodo = Convert.ToDecimal(nuovoIscritto.percPeri.ToString());
                                            //        flag1 = this.WriteDatiStorico(isUpdate, ref messaggio, intMatricola.ToString(), tiprap, nuovoIscritto, prorap, codpos.ToString(), u, str1, strDatFin, decPerPeriodo, Convert.ToDecimal(str2[(int)index].ToString()), strDataScadenza2);
                                            //        if (!flag1)
                                            //            break;
                                            //    }
                                            //    break;
                                            //}//else: update - 
                                            //str4 = "31/12/9999";
                                            //string strDataScadenza3 = nuovoIscritto.datTerm?.ToString();
                                            //decPerPeriodo = Convert.ToDecimal(nuovoIscritto.percPeri.ToString());
                                            //flag1 = this.WriteDatiStorico(isUpdate, ref messaggio, intMatricola.ToString(), tiprap, nuovoIscritto, prorap, codpos.ToString(), u, str1, strDatFin, decPerPeriodo, strDataScadenza: strDataScadenza3);

                                        flag1 = this.WriteDatiStorico(isUpdate, ref messaggio, intMatricola.ToString(), tiprap, nuovoIscritto, prorap, codpos.ToString(), u, nuovoIscritto.daal, nuovoIscritto.alal, Convert.ToDecimal(nuovoIscritto.perce1))
                                                && this.WriteDatiStorico(isUpdate, ref messaggio, intMatricola.ToString(), tiprap, nuovoIscritto, prorap, codpos.ToString(), u, nuovoIscritto.daal2, nuovoIscritto.alal2, Convert.ToDecimal(nuovoIscritto.perce2), isFirstIteration: false)
                                                && this.WriteDatiStorico(isUpdate, ref messaggio, intMatricola.ToString(), tiprap, nuovoIscritto, prorap, codpos.ToString(), u, nuovoIscritto.daal3, nuovoIscritto.alal3, Convert.ToDecimal(nuovoIscritto.perce3), isFirstIteration: false);
                                        break;
                                    case 8: // primo impiego perc
                                    case 10: // apprendistato perc
                                        //3 righe in stordl, uno per ogni periodo
                                        num2 = (short)1;
                                        Decimal decPerParTime = Convert.ToDecimal(nuovoIscritto.PercPT.ToString());
                                        //decPerPeriodo = Convert.ToDecimal(nuovoIscritto.percPeri.ToString());
                                        flag1 = this.WriteDatiStorico(isUpdate, ref messaggio, intMatricola.ToString(), tiprap, nuovoIscritto, prorap, codpos.ToString(), u, nuovoIscritto.daal, nuovoIscritto.alal, Convert.ToDecimal(nuovoIscritto.perce1), decPerParTime)
                                                && this.WriteDatiStorico(isUpdate, ref messaggio, intMatricola.ToString(), tiprap, nuovoIscritto, prorap, codpos.ToString(), u, nuovoIscritto.daal2, nuovoIscritto.alal2, Convert.ToDecimal(nuovoIscritto.perce2), decPerParTime, isFirstIteration: false)
                                                && this.WriteDatiStorico(isUpdate, ref messaggio, intMatricola.ToString(), tiprap, nuovoIscritto, prorap, codpos.ToString(), u, nuovoIscritto.daal3, nuovoIscritto.alal3, Convert.ToDecimal(nuovoIscritto.perce3), decPerParTime, isFirstIteration: false);
                                        break;
                                    case 11: //Part time verticale tempo ridotto
                                        var percentualePartTime = decimal.TryParse(nuovoIscritto.PercPT, out var perPar) ? perPar : 0;
                                        flag1 = WriteDatiStorico(isUpdate, ref messaggio, intMatricola.ToString(), tiprap, nuovoIscritto, prorap, codpos.ToString(), u, datini, strDatFin, 
                                            decPerPeriodo, percentualePartTime, strDataScadenza: (str3 = ""));
                                        if (flag1)
                                        {
                                            flag1 = this.WritePartTime(ref messaggio, prorap, codpos, intMatricola, tiprap, datini, u, nuovoIscritto);
                                            break;
                                        }
                                        break;
                                    case 13: // apprendistato professionalizzante perc
                                        decPerParTime = Convert.ToDecimal(nuovoIscritto.PercPT.ToString());
                                        flag1 = this.WriteDatiStorico(isUpdate, ref messaggio, intMatricola.ToString(), tiprap, nuovoIscritto, prorap, codpos.ToString(), u, datini, strDatFin, decPerPeriodo, decPerParTime, nuovoIscritto.datTerm);
                                        break;
                                    case 14: //contratto intermittente
                                        flag1 = this.WriteDatiStorico(isUpdate, ref messaggio, intMatricola.ToString(), tiprap, nuovoIscritto, prorap, codpos.ToString(), u, datini, strDatFin, decPerPeriodo);
                                        break;
                                    case 16: // part time verticale a termine
                                        flag1 = this.WriteDatiStorico(isUpdate, ref messaggio, intMatricola.ToString(), tiprap, nuovoIscritto, prorap, codpos.ToString(), u, datini, strDatFin, decPerPeriodo, strDataScadenza: nuovoIscritto.datTerm);
                                        if (flag1)
                                        {
                                            flag1 = this.WritePartTime(ref messaggio, prorap, codpos, intMatricola, tiprap, datini, u, nuovoIscritto);
                                            break;
                                        }
                                        break;
                                    case 17: //part time verticale
                                        flag1 = this.WriteDatiStorico(isUpdate, ref messaggio, intMatricola.ToString(), tiprap, nuovoIscritto, prorap, codpos.ToString(), u, datini, strDatFin, decPerPeriodo);
                                        if (flag1)
                                        {
                                            flag1 = this.WritePartTime(ref messaggio, prorap, codpos, intMatricola, tiprap, datini, u, nuovoIscritto);
                                            break;
                                        }
                                        break;
                                    default:
                                        flag1 = false;
                                        messaggio = "Tipo di rapporto non presente nella base dati. Contattare la Fondazione Enpaia";
                                        break;
                                }
                            }
                            else
                            {
                                messaggio = "Errore nel salvataggio dei dati del rapporto lavorativo";
                            }

                        }
                        else
                        {
                            messaggio = "Errore nel salvataggio dei dati anagrafici";
                        }

                        if (flag1)
                            flag1 = this.Module_AggiornaRaplav(Convert.ToInt32(prorap), codpos, intMatricola, nuovoIscritto, ref messaggio);
                        if (flag1)
                        {

                            if (HasDaAl(int.Parse(nuovoIscritto.tipRap)))
                            {
                                flag1 = this.Module_AggiornaStordl(ref messaggio, Convert.ToInt32(prorap), codpos, intMatricola, nuovoIscritto, nuovoIscritto.daal, nuovoIscritto.alal, decPerPeriodo, strDataScadenza: (str3 = ""));
                                flag1 = this.Module_AggiornaStordl(ref messaggio, Convert.ToInt32(prorap), codpos, intMatricola, nuovoIscritto, nuovoIscritto.daal2, nuovoIscritto.alal2, decPerPeriodo, strDataScadenza: (str3 = ""));
                                flag1 = this.Module_AggiornaStordl(ref messaggio, Convert.ToInt32(prorap), codpos, intMatricola, nuovoIscritto, nuovoIscritto.daal3, nuovoIscritto.alal3, decPerPeriodo, strDataScadenza: (str3 = ""));
                            }
                            else
                            {
                                flag1 = this.Module_AggiornaStordl(ref messaggio, Convert.ToInt32(prorap), codpos, intMatricola, nuovoIscritto, datini, strDatFin, decPerPeriodo, strDataScadenza: (str3 = ""));
                            }

                        }
                        if (flag1 && num1 == 0)
                        {
                            string Protocollo_Data = "testn12";
                            flag1 = this.objDataAccess.WriteTransactionData("UPDATE RAPLAV SET PROTISC = " + DBMethods.DoublePeakForSql(Protocollo_Data) + ", " + " DATPROTISC = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + ", " + " NUMPROTISC = " + DBMethods.DoublePeakForSql(numProt) + ", " + " DATPRO = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData)) + ", " + " NUMPRO = " + DBMethods.DoublePeakForSql(numProt) + " WHERE CODPOS = " + codpos.ToString() + " AND MAT = " + intMatricola.ToString() + " AND PRORAP = " + intProRap.ToString(), CommandType.Text);
                            if (flag1)
                                path = clsProtocolloDll.PROT_PRES_GET_TEMP_PATH_APPLICATION() + "\\ISCR01_" + codpos.ToString() + "_" + intMatricola.ToString() + "_" + intProRap.ToString() + ".pdf";
                            if (flag1)
                                flag1 = clsProtocolloDll.Module_AllegaLetteraProtocollo(path, Protocollo_Data, ref numProt);
                        }
                        //if (string.IsNullOrEmpty(this.objDataAccess.Get1ValueFromSQL("SELECT CODUTE  FROM UTEPIN WHERE CODUTE ='" + nuovoIscritto.codFis + "'", CommandType.Text)))
                        //{
                        //    this.objDataAccess.WriteTransactionData("Insert Into UTENTI (CODUTE , DENUTE,  CODTIPUTE, CODFIS, ULTAGG , UTEAGG)" + " Values ( " + DBMethods.DoublePeakForSql(nuovoIscritto.codFis) + ", " + DBMethods.DoublePeakForSql(nuovoIscritto.cognome + " " + nuovoIscritto.nome) + ", " + "'I', " + DBMethods.DoublePeakForSql(nuovoIscritto.codFis) + ", " + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ")", CommandType.Text);
                        //    string strPassword = this.Module_GeneraPIN();
                        //    bool flag7 = this.objDataAccess.WriteTransactionData("INSERT INTO UTEPIN (CODUTE, PIN, DATINI, DATFIN, STAPIN, ULTAGG, UTEAGG) " + " VALUES (" + DBMethods.DoublePeakForSql(nuovoIscritto.codFis) + ", " + DBMethods.DoublePeakForSql(Cypher.DeCryptPassword(strPassword)) + ", " + " CURRENT_DATE, " + "'1899-12-31', " + "'A', " + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ")", CommandType.Text);
                        //    string str6 = "Insert Into GRUISCT (MAT, CODGRU, ULTAGG , UTEAGG)" + " Values ( " + DBMethods.DoublePeakForSql(intMatricola.ToString()) + ", " + " 52, " + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ")";
                        //    if (flag7)
                        //        new Utile().Invia_Email((string)null, "<TABLE>" + "<DIV ALIGN=CENTER><P ALIGN=CENTER>" + " <img src = 'http://www.enpaia.it/logoEnpaia.png'><br><br><br>" + "</P></DIV><DIV ALIGN=JUSTIFY><P ALIGN=JUSTIFY><FONT FACE='ARIAL' SIZE=2>" + "A seguito della sua richiesta di registrazione sul portale Enpaia, si comunica la prima parte del PIN per l’accesso ai servizi on line:<br><BR></FONT>" + "<FONT FACE='ARIAL' SIZE=3><B>" + strPassword + "</B></FONT><BR>BR>" + "<FONT FACE='ARIAL' SIZE=2>La seconda parte verrà spedita all’ indirizzo di residenza.<BR><BR>Cordiali saluti." + "<BR><BR><br>" + "</FONT></P></DIV>" + "<DIV ALIGN=right><P ALIGN=right><FONT FACE='ARIAL' SIZE=2>" + "Il Dirigente della Divisione Attività Istituzionali<br>" + "(Avv. Fabio Petrucci)&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br><br>" + "</FONT></P></DIV>" + "</TABLE>" + "<TABLE>" + "<DIV ALIGN=LEFT><FONT FACE='ARIAL' SIZE=1>" + "<BR><P ALIGN=LEFT><i>Le informazioni contenute nella presente comunicazione e i relativi allegati possono essere riservate e sono, comunque, " + "destinate<BR>esclusivamente alle persone o alla Società sopraindicati.La diffusione, distribuzione e/o copiatura del documento trasmesso " + "da parte<BR>di qualsiasi soggetto diverso dal destinatario è proibita, sia ai sensi dell'art. 616 c.p. , che ai sensi del D.Lgs. n. 196/2003.</i>" + "</P></DIV></FONT>" + "</TABLE>", "INVIO PRIMA PARTE PIN ISCRITTO - " + nuovoIscritto.cognome + " " + nuovoIscritto.nome, nuovoIscritto.email);
                        //}
                    }
                    else
                    {
                        messaggio = "Impossibile salvare! Non sono state apportate modifiche.";
                        this.objDataAccess.EndTransaction(false);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Info($"[DAL - RapportiLavoroDAL- WriteData] - Eccezione generata: {ex.Message} - stackTrace: {ex.StackTrace}");
                this.objDataAccess.EndTransaction(false);
                messaggio = "Impossibile salvare!";
                flag1 = false;

                return (0, false);
            }
            finally
            {
                if (flag1)
                {
                    nuovoIscritto.matricola = intMatricola.ToString();
                    SuccessMSG = "Operazione effettuata con successo!";
                    this.objDataAccess.EndTransaction(true);
                }
                else
                {
                    this.objDataAccess.EndTransaction(false);
                }

            }
            return (intMatricola, flag1);
        }

        private bool IsValidRangeDate(string datIsc, string datTerm)
        {
            return string.IsNullOrWhiteSpace(datTerm) || DateTime.Parse(datTerm) > DateTime.Parse(datIsc);
        }

        private bool CheckDataDecorrenzaModifica(bool isUpdate, string datDecMod, string datIsc, DateTime date) =>
            isUpdate && (DateTime.Parse(datDecMod) < DateTime.Parse(datIsc) || DateTime.Parse(datDecMod) > DateTime.Now.Date);

        private Tuple<int, bool> WriteDatiAnagrafici(
          NuovoIscritto nuovoIscritto,
          string codpos,
          TFI.OCM.Utente.Utente u,
          string strDataInizio,
          ref string messaggio,
          int intMatricola,
          string prorap)
        {
            bool flag1 = true;
            bool flag2 = true;
            string str1 = "";
            string str2 = "N";
            DataTable DT = new DataTable();
            string str3 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            clsIDOC clsIdoc = new clsIDOC();
            iDB2DataAdapter iDb2DataAdapter = new iDB2DataAdapter();
            bool flag3 = true;
            try
            {
                if (HttpContext.Current.Session["ProRap"] == null)
                {
                    if (string.IsNullOrEmpty(nuovoIscritto.matricola))
                    {
                        string str4 = this.objDataAccess.Get1ValueFromSQL(
                            "SELECT CODCOM FROM CODCOM WHERE DENCOM='" + nuovoIscritto.comuneN.ToUpper() + "'",
                            CommandType.Text);
                        DataTable dataTable = this.objDataAccess.GetDataTable("SELECT * FROM ISCT WHERE COG = " +
                                                                              DBMethods.DoublePeakForSql(nuovoIscritto
                                                                                  .cognome.Trim().ToUpper()) +
                                                                              " AND NOM = " +
                                                                              DBMethods.DoublePeakForSql(nuovoIscritto
                                                                                  .nome.Trim().ToUpper()) +
                                                                              " AND DATNAS = " +
                                                                              DBMethods.DoublePeakForSql(
                                                                                  DBMethods.Db2Date(nuovoIscritto
                                                                                      .dataNas)) + " AND CODCOM = " +
                                                                              DBMethods.DoublePeakForSql(str4) +
                                                                              " AND SES = " +
                                                                              DBMethods.DoublePeakForSql(
                                                                                  nuovoIscritto.sesso.ToUpper()));
                        if (dataTable.Rows.Count > 0)
                        {
                            str1 = dataTable.Rows[0]["MAT"].ToString();
                            if (nuovoIscritto.codFis.Trim() != dataTable.Rows[0]["CODFIS"].ToString().Trim())
                            {
                                messaggio =
                                    "Attenzione! Per i dati anagrafici inseriti esiste gia\\' un iscritto con altro Codice Fiscale. Si invita a contattare Enpaia prima di continuare.";
                                return Tuple.Create<int, bool>(intMatricola, false);
                            }

                            nuovoIscritto.matricola = str1;
                        }

                        if (Convert.ToInt32("0" + this.objDataAccess.Get1ValueFromSQL(
                                "SELECT VALUE(COUNT(*), 0) FROM ISCT WHERE CODFIS = " +
                                DBMethods.DoublePeakForSql(nuovoIscritto.codFis.Trim()) + " AND (" + "NOM <> " +
                                DBMethods.DoublePeakForSql(nuovoIscritto.nome.Trim().ToUpper()) + " OR COG <> " +
                                DBMethods.DoublePeakForSql(nuovoIscritto.cognome.Trim().ToUpper()) + " OR " +
                                "DATNAS <> " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(nuovoIscritto.dataNas)) +
                                ")", CommandType.Text)) > 0)
                        {
                            messaggio =
                                "Attenzione! Esiste gia\\' un iscritto con lo stesso Codice Fiscale ma dati anagrafici differenti. Si invita a contattare Enpaia prima di continuare.";
                            return Tuple.Create<int, bool>(intMatricola, false);
                        }

                        if (str1 == "")
                        {
                            string codiceLuogoNascita = string.IsNullOrEmpty(nuovoIscritto.comuneNCod) ? nuovoIscritto.statoEsN.Trim().ToUpper() : nuovoIscritto.comuneNCod.Trim().ToUpper();

                            string str5 = string.IsNullOrEmpty(nuovoIscritto.titoloStudio)
                                ? "Null"
                                : nuovoIscritto.titoloStudio;
                            str1 = this.objDataAccess.Get1ValueFromSQL("SELECT VALUE(MAX(MAT), 1) + 1 AS MAT FROM ISCT",
                                CommandType.Text);
                            flag1 = this.objDataAccess.WriteTransactionData(
                                "INSERT INTO ISCT (MAT, COG, NOM, DATNAS, CODCOM, CODFIS, SES, DATCHIISC, COUCHIISC, STACIV, TITSTU, DATINSMAT, FLGAPP, ULTAGG, UTEAGG) VALUES (" +
                                str1 + ", " + DBMethods.DoublePeakForSql(nuovoIscritto.cognome) + ", " +
                                DBMethods.DoublePeakForSql(nuovoIscritto.nome) + ", '" +
                                DBMethods.Db2Date(nuovoIscritto.dataNas) + "', " +
                                DBMethods.DoublePeakForSql(codiceLuogoNascita) + ", " +
                                DBMethods.DoublePeakForSql(nuovoIscritto.codFis) + ", " +
                                DBMethods.DoublePeakForSql(nuovoIscritto.sesso) + ", '9999-12-31', " + "Null, Null, " +
                                str5 + ", CURRENT_DATE, 'I', CURRENT_TIMESTAMP, " +
                                DBMethods.DoublePeakForSql(u.Username) + ")", CommandType.Text);
                        }
                    }
                    else
                    {
                        string str6 = string.IsNullOrEmpty(nuovoIscritto.titoloStudio.Trim())
                            ? "Null"
                            : nuovoIscritto.titoloStudio;
                        str1 = nuovoIscritto.matricola;
                        flag1 = this.objDataAccess.WriteTransactionData(
                            "UPDATE ISCT SET FLGAPP = 'M', TITSTU = '" + str6 + "', COG = " +
                            DBMethods.DoublePeakForSql(nuovoIscritto.cognome.ToUpper()) + ", NOM = " +
                            DBMethods.DoublePeakForSql(nuovoIscritto.nome.ToUpper()) + ", DATNAS = '" +
                            DBMethods.Db2Date(nuovoIscritto.dataNas) + "', CODCOM = " +
                            DBMethods.DoublePeakForSql(nuovoIscritto.comuneNCod) + ", CODFIS = " +
                            DBMethods.DoublePeakForSql(nuovoIscritto.codFis) + ", SES = " +
                            DBMethods.DoublePeakForSql(nuovoIscritto.sesso) +
                            ", DATINPS = Null, TIPOPE = 'V', ULTAGG = CURRENT_TIMESTAMP, UTEAGG = " +
                            DBMethods.DoublePeakForSql(u.Username) + " WHERE MAT = '" + str1 + "'", CommandType.Text);
                    }

                    if (flag1)
                    {
                        //if (!string.IsNullOrEmpty(nuovoIscritto.matricola) && Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL("SELECT COUNT(*) FROM ISCD WHERE MAT = " + nuovoIscritto.matricola, CommandType.Text)) > 0)
                        //{
                        //    DT.Clear();
                        //    DT = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0006", 0, Convert.ToInt32(str1), 0, 0, "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "CANCELLAZIONE", "", "");
                        //    clsIdoc.WRITE_IDOC_TESTATA(this.objDataAccess, DT.Rows[0]);
                        //    flag3 = true;
                        //    clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "0006", DT, true);
                        //}
                        //flag1 = this.objDataAccess.WriteTransactionData("DELETE FROM ISCD WHERE MAT = " + str1 + " AND DATINI = '" + DBMethods.Db2Date(strDataInizio) + "'", CommandType.Text);
                        //if (flag1)
                        //{
                        //    string str7 = string.IsNullOrEmpty(nuovoIscritto.statoEsN) ? "Null" : "'" + nuovoIscritto.statoEsN.Replace("'", "''").Trim() + "'";
                        //    string str8 = string.IsNullOrEmpty(nuovoIscritto.tel) ? "Null" : "'" + nuovoIscritto.tel.Replace("'", "''").Trim() + "'";
                        //    string str9 = string.IsNullOrEmpty(nuovoIscritto.tel2) ? "Null" : "'" + nuovoIscritto.tel2.Replace("'", "''").Trim() + "'";
                        //    string str10 = string.IsNullOrEmpty(nuovoIscritto.fax) ? "Null" : "'" + nuovoIscritto.fax.Replace("'", "''").Trim() + "'";
                        //    string str11 = string.IsNullOrEmpty(nuovoIscritto.cell) ? "Null" : "'" + nuovoIscritto.cell.Replace("'", "''").Trim() + "'";
                        //    string str12;
                        //    if (!string.IsNullOrEmpty(nuovoIscritto.email))
                        //    {
                        //        str12 = nuovoIscritto.email.Replace("'", "''").Trim();
                        //    }
                        //    else
                        //    {
                        //        DataTable dataTable1 = new DataTable();
                        //        DataTable dataTable2 = this.objDataAccess.GetDataTable("SELECT * FROM ISCD A WHERE MAT = " + str1 + " AND DATINI = (SELECT MAX(DATINI) FROM ISCD WHERE MAT = A.MAT)");
                        //        str12 = dataTable2.Rows.Count <= 0 ? "" : (!(dataTable2.Rows[0]["EMAIL"].ToString().Trim() != "") ? "" : dataTable2.Rows[0]["EMAIL"].ToString().Trim());
                        //    }
                        //    string str13 = string.IsNullOrEmpty(nuovoIscritto.pec) ? "Null" : "'" + nuovoIscritto.pec.Replace("'", "''").Trim() + "'";
                        //    string str14 = string.IsNullOrEmpty(nuovoIscritto.co) ? "Null" : "'" + nuovoIscritto.co.Replace("'", "''").Trim() + "'";
                        //    string str15 = "INSERT INTO ISCD (MAT, DATINI, CODDUG, IND, NUMCIV, CODCOM, CAP, SIGPRO, DENLOC, DENSTAEST, AGGMAN, TEL1, " + "TEL2, FAX, EMAIL, URL, CO, EMAILCERT, CELL, ULTAGG, UTEAGG) VALUES (" + str1 + ", '" + DBMethods.Db2Date(strDataInizio) + "', " + nuovoIscritto.indirizzoCod + ", " + DBMethods.DoublePeakForSql(nuovoIscritto.residenza) + ", " + DBMethods.DoublePeakForSql(nuovoIscritto.civico) + ", " + DBMethods.DoublePeakForSql(nuovoIscritto.comuneCod) + ", " + DBMethods.DoublePeakForSql(nuovoIscritto.cap) + ", " + DBMethods.DoublePeakForSql(nuovoIscritto.provincia).ToUpper() + ", " + DBMethods.DoublePeakForSql(nuovoIscritto.localita).ToUpper() + ", " + str7 + ", " + DBMethods.DoublePeakForSql(str2) + ", " + str8 + ", " + str9 + ", " + str10;
                        //    string strSQL;
                        //    if (str12 != "")
                        //        strSQL = str15 + ", " + DBMethods.DoublePeakForSql(str12) + ", Null, " + DBMethods.DoublePeakForSql(str14) + ", " + str13 + ", " + str11.Replace(" ", "") + ", '" + str3 + "', " + u.Username + ")";
                        //    else
                        //        strSQL = str15 + ", Null, Null, " + DBMethods.DoublePeakForSql(str14) + ", " + str13 + ", " + str11.Replace(" ", "") + ", '" + str3 + "', " + u.Username + ")";
                        //    flag1 = this.objDataAccess.WriteTransactionData(strSQL, CommandType.Text);

                        //UPSERT ISCD
                        if (flag1)
                        {

                            intMatricola = Convert.ToInt32(str1);
                            nuovoIscritto.matricola = str1;
                            flag1 = UpsertISCD(nuovoIscritto);
                            if (flag1)
                                flag1 = this.objDataAccess.WriteTransactionData(
                                    "UPDATE ISCT SET DATINPS = Null, TIPOPE = 'V' WHERE MAT = " + str1,
                                    CommandType.Text);
                            else
                            {
                                messaggio = "Errore nel salvataggio dei contatti";
                            } //if (str1 != "")
                            //{
                            //    DT.Clear();
                            //    DataTable idocDatiE1Pitype = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0006", 0, Convert.ToInt32(str1), 0, 0, "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                            //    if (!flag3)
                            //        clsIdoc.WRITE_IDOC_TESTATA(this.objDataAccess, idocDatiE1Pitype.Rows[0]);
                            //    for (int index = 0; index <= idocDatiE1Pitype.Rows.Count - 1; ++index)
                            //    {
                            //        idocDatiE1Pitype.Rows[index]["DATBEGDA"] = idocDatiE1Pitype.Rows[index]["DATINI"];
                            //        idocDatiE1Pitype.Rows[index]["DATENDDA"] = idocDatiE1Pitype.Rows[index]["DATFIN"];
                            //    }
                            //    clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "0006", idocDatiE1Pitype, false);
                            //}
                        }

                    }

                }
                if (!flag1)
                    messaggio = "Errore nel salvataggio dei dati anagrafici";
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)u), JsonConvert.SerializeObject((object)(0, false)));
                messaggio = "Si sono verificati errori durante il salvataggio sulla tabella degli Iscritti.";
                flag1 = false;
            }
            return Tuple.Create<int, bool>(intMatricola, flag1);
        }

        private bool CheckFields(
          string codpos,
          string DataModifica,
          string prorap,
          NuovoIscritto nuovoIscritto,
          ref string messaggio)
        {
            return this.CheckDipa(codpos, DataModifica, ref messaggio) && (prorap != null || this.CheckRDL(prorap, nuovoIscritto, codpos, ref messaggio) && this.CheckDateInterval(nuovoIscritto, prorap, ref messaggio)) && (!(nuovoIscritto.codCon + "-" + nuovoIscritto.codloc == nuovoIscritto.contrattocod) || this.CheckLivello(nuovoIscritto, prorap, ref messaggio));
        }

        private bool CheckLivello(NuovoIscritto nuovoIscritto, string prorap, ref string messaggio)
        {
            NuovoIscritto nuovoIscritto1 = (NuovoIscritto)HttpContext.Current.Session["ModificaIscritto"];
            if (string.IsNullOrEmpty(prorap))
                return true;
            string str1 = this.objDataAccess.Get1ValueFromSQL("SELECT ORDLIV FROM CONLIV WHERE CODCON = " + nuovoIscritto.codCon + " AND CODLIV = " + nuovoIscritto.codlivello + " AND PROCON = (SELECT MAX(PROCON) FROM CONRIF WHERE " + "CODCON = " + nuovoIscritto1.codCon + ")", CommandType.Text);
            string str2 = this.objDataAccess.Get1ValueFromSQL("SELECT ORDLIV FROM CONLIV WHERE CODCON =" + nuovoIscritto1.codCon + " AND CODLIV = " + nuovoIscritto1.codlivello, CommandType.Text);
            if (Convert.ToInt32("0" + str1) <= (int)Convert.ToInt16("0" + str2))
                return true;
            messaggio = "Non è possibile selezionare un livello inferiore a quello attuale!";
            return false;
        }

        private bool CheckDateInterval(
          NuovoIscritto nuovoIscritto,
          string prorap,
          ref string messaggio)
        {
            //Data di modifica non può essere minore della data di inizio dell'ultimo rapporto di lavoro. in modifica non posso avere una data inferiore.


            DateTime dateTime1 = new DateTime();
            DateTime dateTime2 = new DateTime();
            DateTime dateTime3 = new DateTime();
            DateTime dateTime4 = new DateTime();
            short months = 0;
            bool flag = true;
            DateTime t1 = new DateTime();
            DateTime dateTime5 = new DateTime();
            if (string.IsNullOrEmpty(nuovoIscritto.tipRap))
                return true;
            string str = nuovoIscritto.tipRap ?? "";
            if (!(str == "6") && !(str == "7") && !(str == "8") && !(str == "10"))
                return true;
            if (!string.IsNullOrEmpty(nuovoIscritto.periodo))
                months = (short)Math.Round(Convert.ToDouble(nuovoIscritto.periodo) / 3.0);
            DateTime dateTime6 = Convert.ToDateTime(DateTime.Parse(nuovoIscritto.daal));
            DateTime dateTime7 = Convert.ToDateTime(DateTime.Parse(nuovoIscritto.alal));
            short num1;
            if (string.IsNullOrEmpty(prorap))
            {
                num1 = (short)3;
                dateTime1 = Convert.ToDateTime(DateTime.Parse(nuovoIscritto.daal2));
                dateTime2 = Convert.ToDateTime(DateTime.Parse(nuovoIscritto.alal2));
                dateTime3 = Convert.ToDateTime(DateTime.Parse(nuovoIscritto.daal3));
                dateTime4 = Convert.ToDateTime(DateTime.Parse(nuovoIscritto.alal3));
            }
            else
                num1 = (short)1;
            DateTime dateTime8 = dateTime6;
            short num2 = 1;
            for (short index = num1; (int)num2 <= (int)index; ++num2)
            {
                switch (num2)
                {
                    case 1:
                        dateTime8 = dateTime6;
                        t1 = dateTime7;
                        dateTime5 = Convert.ToDateTime("01/01/1900");
                        break;
                    case 2:
                        dateTime8 = dateTime1;
                        t1 = dateTime2;
                        dateTime5 = dateTime7;
                        break;
                    case 3:
                        dateTime8 = dateTime3;
                        t1 = dateTime4;
                        dateTime5 = dateTime2;
                        break;
                }
                DateTime dateTime9 = Convert.ToDateTime(dateTime8);
                DateTime t2_1 = dateTime9.AddDays(1.0);
                dateTime9 = Convert.ToDateTime(dateTime8);
                DateTime t2_2 = dateTime9.AddMonths((int)months);
                if (DateTime.Compare(t1, t2_1) <= 0 | DateTime.Compare(t1, t2_2) > 0)
                    flag = false;
                else if (dateTime5 != DateTime.Parse("01/01/1900") & !(dateTime5.AddDays(1.0) == dateTime8))
                    flag = false;
            }
            if (!flag)
                messaggio = "Intervallo di date rapporto non valido!";
            return flag;
        }

        private bool CheckDipa(string codpos, string DataModifica, ref string messaggio)
        {
            try
            {
                bool flag = true;
                string str1 = "select count(*) from dentes where nummov is not null and nummovann is null and tipmov = 'DP'" + " and codpos = " + codpos;
                string strSQL;
                if (!string.IsNullOrEmpty(DataModifica))
                {
                    string str2 = str1;
                    int num = DateTime.Parse(DataModifica).Year;
                    string str3 = num.ToString();
                    string str4 = str2 + " and annden >= " + str3;
                    num = DateTime.Parse(DataModifica).Month;
                    string str5 = num.ToString();
                    strSQL = str4 + " and mesden >= " + str5;
                }
                else
                {
                    string str6 = str1;
                    int num = DateTime.Parse(DataModifica).Year;
                    string str7 = num.ToString();
                    string str8 = str6 + " and annden >= " + str7;
                    num = DateTime.Parse(DataModifica).Month;
                    string str9 = num.ToString();
                    strSQL = str8 + " and mesden >= " + str9;
                }
                if (Convert.ToInt32("0" + this.objDataAccess.Get1ValueFromSQL(strSQL, CommandType.Text)) > 0)
                {
                    messaggio = "Impossibile inserire l\\'iscritto per questa data.! Esiste una denuncia confermata.";
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                messaggio = "Errore nel controllo DIPA";
                return false;
            }
        }

        private bool WriteDatiStorico(
            bool isUpdate,
            ref string messaggio,
            string intMatricola,
            int tiprap,
            NuovoIscritto nuovoIscritto,
            string prorap,
            string codpos,
            TFI.OCM.Utente.Utente u,
            string strDatIni,
            string strDatFin,
            Decimal decPerPeriodo,
            Decimal decPerParTime = 0M,
            string strDataScadenza = "",
            bool isFirstIteration = true)
        {
            bool flag1 = true;
            try
            {
                bool flag2 = true;
                Decimal num1 = 0M;
                DataTable dataTable = new DataTable();
                clsIDOC clsIdoc = new clsIDOC();
                iDB2DataAdapter iDb2DataAdapter = new iDB2DataAdapter();
                bool flag3 = false;
                string str1 = "CURRENT TIMESTAMP";
                ArrayList arrayList = new ArrayList() //EMOLUMENTI AGGIUNTIVI
                {
                    nuovoIscritto.S12.FromDomainCurrencyFormat(),
                    nuovoIscritto.S13.FromDomainCurrencyFormat(),
                    nuovoIscritto.S14.FromDomainCurrencyFormat(),
                    nuovoIscritto.S15.FromDomainCurrencyFormat(),
                    nuovoIscritto.S16.FromDomainCurrencyFormat()
                };
                if (flag2)
                {
                    short int16_1 = Convert.ToInt16("0" + nuovoIscritto.m14);
                    short int16_2 = Convert.ToInt16("0" + nuovoIscritto.m15);
                    short int16_3 = Convert.ToInt16("0" + nuovoIscritto.m16);
                    Decimal num2 = 0M;
                    if (!string.IsNullOrEmpty(nuovoIscritto.tipspe)) //TIPSPE???
                    {
                        string upper = nuovoIscritto.tipspe.Trim().ToUpper();
                        if (!(upper == "S"))
                        {
                            if (upper == "M" || upper == "A")
                            {
                                for (short index = 0; index <= (short)4; ++index)
                                {
                                    if (arrayList[(int)index] != null)
                                        num1 += Convert.ToDecimal("0" + arrayList[(int)index]?.ToString());
                                }

                                num2 = Convert.ToDecimal(nuovoIscritto.totaleS) - num1;
                            }
                        }
                        else
                            num2 = Convert.ToDecimal(0);
                    }

                    if (isUpdate && isFirstIteration)
                    {
                        int int32 = Convert.ToInt32(prorap);
                        bool flag4 = this.objDataAccess.WriteTransactionData(
                            "DELETE FROM IMPAGG WHERE CODPOS = " + codpos + " AND MAT = " + nuovoIscritto.matricola +
                            " AND PRORAP = " + int32.ToString() + " AND DATINI = '" + DBMethods.Db2Date(strDatIni) +
                            "'", CommandType.Text);
                        if (tiprap.ToString() == "11" | tiprap.ToString() == "16" | tiprap.ToString() == "17")
                            flag4 = this.objDataAccess.WriteTransactionData(
                                "DELETE FROM PARTIMM WHERE MAT = " + intMatricola + " AND CODPOS = " + codpos +
                                " AND PRORAP = " + int32.ToString() + " AND DATINI = '" + DBMethods.Db2Date(strDatIni) +
                                "'", CommandType.Text);



                        this.objDataAccess.Get1ValueFromSQL(
                            "SELECT DISTINCT TIPRAP FROM STORDL WHERE MAT = " + intMatricola + " AND CODPOS = " +
                            codpos + " AND " + "PRORAP = " + int32.ToString() + " AND '" +
                            DBMethods.Db2Date(strDatIni) + "' BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31')",
                            CommandType.Text);
                        if (flag1)
                        {
                            //Se la nuova datini è < della vecchia allora devo prendere maxdatfin e impostarla a ieri.
                            //se la nuova datini è > della vecchia (non può essere uguale perche l'avrebbe deletata sopra) andrebbe bene max datini dove datini < nuovadatini 
                            string strData = this.objDataAccess.Get1ValueFromSQL(
                                "SELECT DATINI FROM STORDL WHERE MAT = " + intMatricola + " AND CODPOS = " + codpos +
                                " AND PRORAP = " + int32.ToString() +
                                " AND DATFIN = (SELECT MAX(DATFIN) FROM STORDL WHERE MAT = " + intMatricola +
                                " AND CODPOS = " + codpos + " AND PRORAP = " + int32.ToString() + ")",
                                CommandType.Text);
                            string oldTipRap = this.objDataAccess.Get1ValueFromSQL(
                                "SELECT TIPRAP FROM STORDL WHERE MAT = " + intMatricola + " AND CODPOS = " + codpos +
                                " AND PRORAP = " + int32.ToString() +
                                " AND DATFIN = (SELECT MAX(DATFIN) FROM STORDL WHERE MAT = " + intMatricola +
                                " AND CODPOS = " + codpos + " AND PRORAP = " + int32.ToString() + ")",
                                CommandType.Text);

                            DataTable recordDaAl = new DataTable();
                            if (HasDaAl(int.Parse(oldTipRap)))
                            {
                                string getDaAlRecord = "SELECT * FROM STORDL WHERE MAT = " + intMatricola +
                                                       " AND CODPOS = " + codpos + " AND PRORAP = " + int32.ToString() +
                                                       " ORDER BY DATFIN DESC LIMIT 3";
                                recordDaAl = objDataAccess.GetDataTable(getDaAlRecord);
                                strData = recordDaAl.Rows[2].DateElementAt("DATINI");
                            }

                            flag1 = this.objDataAccess.WriteTransactionData(
                                "DELETE FROM STORDL WHERE MAT = " + intMatricola + " AND CODPOS = " + codpos +
                                " AND PRORAP = " + int32.ToString() + " AND DATINI = '" + DBMethods.Db2Date(strDatIni) +
                                "'", CommandType.Text);


                            string updateStordlQuery =
                                "UPDATE STORDL SET DATFIN = DATE('" + DBMethods.Db2Date(strDatIni) +
                                "') - 1 days, UTEAGG = '" + u.Username +
                                "', ULTAGG = current_timestamp  WHERE MAT = " + intMatricola +
                                " AND CODPOS = " + codpos + " AND PRORAP = " + int32.ToString() +
                                " AND DATINI = '" + DBMethods.Db2Date(strData) + "'";
                            if (!string.IsNullOrEmpty(strData))
                                flag1 = this.objDataAccess.WriteTransactionData(updateStordlQuery, CommandType.Text);
                            if (flag1)
                            {

                                switch (Convert.ToInt16("0" + oldTipRap.ToString()))
                                {
                                    case 6:
                                    case 7:
                                    case 8:
                                    case 10:
                                        //Getto tutti i record in stordl limit 2 order by DATINI o DATFIN. Prendo DATINI 2 e 3 e li rimuovo
                                        //Non li rimuovo se hanno 
                                        string datini2 = recordDaAl.Rows[0].DateElementAt("DATINI");
                                        string datini3 = recordDaAl.Rows[1].DateElementAt("DATINI");
                                        flag1 = this.objDataAccess.WriteTransactionData(
                                            "DELETE FROM STORDL WHERE MAT = " + intMatricola + " AND CODPOS = " +
                                            codpos + " AND PRORAP = " + int32.ToString() + " AND DATINI = '" +
                                            DBMethods.Db2Date(datini2) + "' OR DATINI = '" +
                                            DBMethods.Db2Date(datini3) + "'", CommandType.Text);

                                        if (Convert.ToInt32("0" + this.objDataAccess.Get1ValueFromSQL(
                                                "SELECT COUNT(*) FROM STORDL WHERE MAT = " + intMatricola +
                                                " AND CODPOS = " + codpos + " AND PRORAP = " + int32.ToString() +
                                                " AND DATINI > '" + DBMethods.Db2Date(strDatIni) + "'",
                                                CommandType.Text)) > 0)
                                        {
                                            flag1 = this.objDataAccess.WriteTransactionData(
                                                "DELETE FROM IMPAGG WHERE MAT = " + intMatricola + " AND CODPOS = " +
                                                codpos + " AND PRORAP = " + int32.ToString() + " AND DATINI > '" +
                                                DBMethods.Db2Date(strDatIni) + "'", CommandType.Text);
                                            //if (flag1)
                                            //{
                                            //    flag1 = this.objDataAccess.WriteTransactionData("DELETE FROM STORDL WHERE MAT = " + intMatricola + " AND CODPOS = " + codpos + " AND PRORAP = " + int32.ToString() + " AND DATINI > '" + DBMethods.Db2Date(strDatIni) + "'", CommandType.Text);
                                            //    break;
                                            //}
                                            break;
                                        }

                                        break;
                                }
                            }
                            else
                            {
                                messaggio = "Errore aggiornamento Storico Rapporto di Lavoro";
                                return false;
                            }
                        }
                    }
                    else if (Convert.ToInt32("0" + this.objDataAccess.Get1ValueFromSQL(
                                 "SELECT COUNT(*) FROM RAPLAV WHERE CODPOS = " + codpos + " AND MAT = " + intMatricola +
                                 " AND '" + DBMethods.Db2Date(strDatIni) +
                                 "' BETWEEN DATDEC AND DATCES AND DATCES IS NOT NULL", CommandType.Text)) > 0)
                    {
                        flag1 = false;
                        messaggio =
                            "Impossibile salvare! La data inizio indicata risulta all\\'interno del precedente rapporto di lavoro!";
                        return false;
                    }

                    if (flag1)
                    {
                        //INSERT STORDL
                        string contratto = nuovoIscritto.contratto;
                        string fap = "N";
                        if (nuovoIscritto.fap)
                            fap = "S";
                        string retDic;
                        if (string.IsNullOrEmpty(nuovoIscritto.retDic))
                            retDic = "0";
                        else
                        {
                            retDic = nuovoIscritto.retDic.ToDomainDecimalFormat();
                            num2 = Convert.ToDecimal(nuovoIscritto.retDic);
                        }

                        string str4 = "N";
                        string assistenzaCon = !nuovoIscritto.assistenzaCon ? "N" : "S";
                        strDataScadenza = string.IsNullOrEmpty(strDataScadenza)
                            ? "Null"
                            : "'" + DBMethods.Db2Date(strDataScadenza) + "'";
                        string dataUltScat = string.IsNullOrEmpty(nuovoIscritto.dataUltSc)
                            ? "Null"
                            : "'" + DBMethods.Db2Date(nuovoIscritto.dataUltSc) + "'";
                        string dataProssScat = string.IsNullOrEmpty(nuovoIscritto.prossimoSc)
                            ? "Null"
                            : "'" + DBMethods.Db2Date(nuovoIscritto.prossimoSc) + "'";

                        var importoWithoutCurrency = nuovoIscritto.importoSc?.Trim().Split(' ')[0];

                        string importoFormattato = "0";
                        if (!string.IsNullOrWhiteSpace(importoWithoutCurrency))
                        {
                            if (importoWithoutCurrency.Length < 4)
                                importoFormattato = importoWithoutCurrency.Replace(",", ".");
                            else
                            {
                                var last3Chars = importoWithoutCurrency.Substring(importoWithoutCurrency.Length - 3);
                                var firstXChars = importoWithoutCurrency.Substring(0, importoWithoutCurrency.Length - 3);
                                var last3CharsWithoutComma = last3Chars.Replace(",", ".");
                                var firstCharsWithoutPunctuaction = firstXChars.Replace(",", "").Replace(".", "");
                                importoFormattato = firstCharsWithoutPunctuaction + last3CharsWithoutComma;
                            }
                        }

                        string insertStordlQuery =
                            "INSERT INTO STORDL(CODPOS, MAT, PRORAP, DATINI, DATFIN, TIPRAP, CODCON, CODLOC, CODLIV, TRAECO, FAP, NUMMEN, PERAPP, PERPAR, MESMEN14, " +
                            "MESMEN15, MESMEN16, INDANN, ASSCON, ABBPRE, CODGRUASS, DATSCATER, NUMSCAMAT, IMPSCAMAT, DATULTSCA, DATNEWSCA, ULTAGG, UTEAGG, DENLIV) VALUES (" +
                            codpos + ", " + intMatricola + ", " + prorap + ", '" + DBMethods.Db2Date(strDatIni) +
                            "', '" +
                            DBMethods.Db2Date(strDatFin) + "', " + nuovoIscritto.tipRap + ", " + contratto + ", " +
                            nuovoIscritto.codloc + ", " + nuovoIscritto.codlivello + ", " + retDic + ", '" + fap +
                            "', " +
                            nuovoIscritto.numMes + ", " + decPerPeriodo.ToString().Replace(",", ".") + ", " +
                            decPerParTime.ToString().Replace(",", ".") + ", " + int16_1.ToString() + ", " +
                            int16_2.ToString() + ", " + int16_3.ToString() + ", 'N', '" + assistenzaCon + "', '" +
                            str4 + "', " +
                            nuovoIscritto.gruass + ", " + strDataScadenza + ", " + nuovoIscritto.scattiAnz + ", '" +
                            importoFormattato + "', " + dataUltScat + ", " + dataProssScat + ", CURRENT_TIMESTAMP, '" +
                            u.Username + "', '" + nuovoIscritto.denlivello + "')";
                        flag1 = this.objDataAccess.WriteTransactionData(insertStordlQuery, CommandType.Text);

                        //INSERT EMOLUMENTI
                        if (flag1)
                        {
                            for (short index = 0; index <= 4; index++)
                            {
                                if (arrayList[index] != null && Convert.ToDecimal("0" + arrayList[index]) > 0M)
                                {
                                    Decimal num3 = Convert.ToDecimal("0" + arrayList[index]);
                                    string str9 = this.objDataAccess.Get1ValueFromSQL(
                                        "SELECT VALUE(MAX(PROIMP), 0) + 1 FROM IMPAGG WHERE CODPOS = " + codpos +
                                        " AND MAT = " + intMatricola + " AND PRORAP = " + prorap + " AND DATINI = '" +
                                        DBMethods.Db2Date(strDatIni) + "'", CommandType.Text);
                                    int menagg = index + 12;
                                    flag1 = this.objDataAccess.WriteTransactionData(
                                        "INSERT INTO IMPAGG (CODPOS, MAT, PRORAP, PROIMP, DATINI, MENAGG, IMPAGG, ULTAGG, UTEAGG) VALUES (" +
                                        codpos + ", " + intMatricola + ", " + prorap + ", " + str9 + ", '" +
                                        DBMethods.Db2Date(strDatIni) + "', " + menagg + ", '" +
                                        num3.ToString().Replace(",", ".") + "', CURRENT_TIMESTAMP, '" + u.Username + "')",
                                        CommandType.Text);
                                    if (!flag1)
                                    {
                                        messaggio =
                                            "Si sono verificati errori durante il salvataggio sulla tabella IMPAGG.";
                                        break;
                                    }
                                }
                            }

                            nuovoIscritto.S12 = null;
                            nuovoIscritto.S13 = null;
                            nuovoIscritto.S14 = null;
                            nuovoIscritto.S15 = null;
                            nuovoIscritto.S16 = null;
                        }
                        else
                        {
                            messaggio = "Errore nel salvataggio storico rapporto di lavoro";
                            return false;
                        }
                    }
                    //if (flag1)
                    //{
                    //    DataTable DT = prorap == null ? clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0016", Convert.ToInt32(codpos), Convert.ToInt32(intMatricola), Convert.ToInt32(prorap), 0, "", "", "9999-12-31", "", "", nuovoIscritto.datIsc, 0, 0, 0, "", "INSERIMENTO RDL", "", "", flgWEB: "S") : clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0016", Convert.ToInt32(codpos), Convert.ToInt32(intMatricola), Convert.ToInt32(prorap), 0, "", "", "9999-12-31", "", "", nuovoIscritto.datIsc, 0, 0, 0, "", "MODIFICA RDL", "", "", flgWEB: "S");
                    //    if (!flag3)
                    //        clsIdoc.WRITE_IDOC_TESTATA(this.objDataAccess, DT.Rows[0]);
                    //    clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "9001", DT, false);
                    //    clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "0016", DT, false);
                    //}
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)u), JsonConvert.SerializeObject((object)""));
                messaggio = "Errore nel salvataggio dei dati storico";
                return false;
            }

            return flag1;
        }

        private bool Module_AggiornaRaplav(
          int intProRap,
          int codpos,
          int intMatricola,
          NuovoIscritto nuovoIscritto,
          ref string messaggio)
        {
            DataTable dataTable1 = new DataTable();
            DataTable dataTable2 = new DataTable();
            string strData1 = "";
            string strData2 = "";
            string strData3 = "";
            Decimal num1 = 0M;
            Decimal num2 = 0M;
            Decimal num3 = 0M;
            string str1 = "";
            Decimal num4 = 0M;
            Decimal num5 = 0M;
            Decimal num6 = 0M;
            int num7 = 0;
            int num8 = 0;
            int num9 = 0;
            int num10 = 0;
            string str2 = "";
            int num11 = 0;
            string str3 = "";
            string str4 = "";
            int num12 = 0;
            string str5 = "";
            bool flag;
            try
            {
                int PRORAP = HttpContext.Current.Session["ProRap"] != null ? Convert.ToInt32(HttpContext.Current.Session["ProRap"]) : intProRap;
                int MAT = intMatricola <= 0 ? Convert.ToInt32(nuovoIscritto.matricola) : intMatricola;
                DataTable datiCompletiStordl = this.GetDatiCompleti_STORDL(codpos, MAT, PRORAP, (string)null, (string)null);
                if (datiCompletiStordl.Rows.Count > 0)
                {
                    str3 = datiCompletiStordl.Rows[0]["ABBPRE"].ToString();
                    str4 = datiCompletiStordl.Rows[0]["ASSCON"].ToString();
                    num8 = Convert.ToInt32(datiCompletiStordl.Rows[0]["CODCON"]);
                    num9 = Convert.ToInt32(datiCompletiStordl.Rows[0]["CODLOC"]) == 0 ? Convert.ToInt32(nuovoIscritto.codloc) : Convert.ToInt32(datiCompletiStordl.Rows[0]["CODLOC"]);
                    num5 = Convert.ToDecimal(datiCompletiStordl.Rows[0]["CODGRUASS"]);
                    str1 = datiCompletiStordl.Rows[0]["FAP"].ToString().Trim();
                    if (str1 == "S")
                        num4 = Convert.ToDecimal(this.objDataAccess.Get1ValueFromSQL("SELECT VALFAP FROM CODFAP WHERE " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(datiCompletiStordl.Rows[0]["DATINI"].ToString())) + " BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31')", CommandType.Text));
                    num6 = Convert.ToDecimal(datiCompletiStordl.Rows[0]["ALIQUOTA"]);
                    num11 = Convert.ToInt32(datiCompletiStordl.Rows[0]["TIPRAP"]);
                    num10 = Convert.ToInt32(datiCompletiStordl.Rows[0]["CODLIV"]);
                    str2 = datiCompletiStordl.Rows[0]["DENLIV"].ToString().Trim();
                    num7 = Convert.ToInt32(datiCompletiStordl.Rows[0]["CODQUACON"]);
                    DataTable dataTable3 = this.objDataAccess.GetDataTable(" SELECT * FROM (SELECT (SELECT CATFORASS FROM FORASS WHERE CODFORASS = A.CODFORASS) AS CAT, A.ALIQUOTA " + " FROM ALIFORASS A WHERE A.CODGRUASS =  " + num5.ToString() + " AND A.CODQUACON = " + num7.ToString() + " ) AS TAB WHERE TAB.CAT <> 'FAP'");
                    for (int index = 0; index <= dataTable3.Rows.Count - 1; ++index)
                    {
                        string str6 = dataTable3.Rows[index]["CAT"].ToString().Trim();
                        if (!(str6 == "TFR"))
                        {
                            if (!(str6 == "PREV"))
                            {
                                if (!(str6 == "INF"))
                                    throw new Exception("Caso non gestito " + dataTable3.Rows[index]["CAT"]?.ToString());
                                num2 = Convert.ToDecimal(dataTable3.Rows[index]["ALIQUOTA"]);
                            }
                            else
                                num3 = Convert.ToDecimal(dataTable3.Rows[index]["ALIQUOTA"]);
                        }
                        else
                            num1 = Convert.ToDecimal(dataTable3.Rows[index]["ALIQUOTA"]);
                    }
                }
                for (int index1 = 0; index1 <= 2; ++index1)
                {
                    switch (index1)
                    {
                        case 0:
                            str5 = "TFR";
                            break;
                        case 1:
                            str5 = "PREV";
                            break;
                        case 2:
                            str5 = "INF";
                            break;
                    }
                    for (int index2 = 0; index2 <= datiCompletiStordl.Rows.Count - 1; ++index2)
                    {
                        num12 = 0;
                        int int32 = Convert.ToInt32("0" + this.objDataAccess.Get1ValueFromSQL("SELECT CODQUACON FROM CONRIF " + " WHERE CODCON = " + datiCompletiStordl.Rows[index2]["CODCON"]?.ToString() + " AND DATDEC <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(datiCompletiStordl.Rows[index2]["DATINI"].ToString())) + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY", CommandType.Text));
                        if (Convert.ToInt32("0" + this.objDataAccess.Get1ValueFromSQL(" SELECT COUNT(*) FROM ALIFORASS A, FORASS B WHERE A.CODGRUASS =  " + datiCompletiStordl.Rows[index2]["CODGRUASS"]?.ToString() + " AND A.CODQUACON = " + int32.ToString() + " AND A.CODFORASS = B.CODFORASS AND B.CATFORASS = " + DBMethods.DoublePeakForSql(str5), CommandType.Text)) > 0)
                        {
                            switch (index1)
                            {
                                case 0:
                                    strData1 = datiCompletiStordl.Rows[index2]["DATINI"].ToString();
                                    break;
                                case 1:
                                    strData3 = datiCompletiStordl.Rows[index2]["DATINI"].ToString();
                                    break;
                                case 2:
                                    strData2 = datiCompletiStordl.Rows[index2]["DATINI"].ToString();
                                    break;
                            }
                        }
                        else
                            break;
                    }
                }
                string str7 = " UPDATE RAPLAV SET ";
                if (strData1.Trim() != "")
                    str7 = str7 + " DTTFR  = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData1)) + ", " + " DATLIQTFR  = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData1)) + ", ";
                if (strData2.Trim() != "")
                    str7 = str7 + " DTINF  = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData2)) + ", ";
                if (strData3.Trim() != "")
                    str7 = str7 + " DTFP  = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData3)) + ", ";
                flag = this.objDataAccess.WriteTransactionData(str7 + " ALIQTFR  = " + num1.ToString().Replace(",", ".") + ", " + " ALIQINF  = " + num2.ToString().Replace(",", ".") + ", " + " ALIQFP  = " + num3.ToString().Replace(",", ".") + ", " + " FAP  = " + DBMethods.DoublePeakForSql(str1) + ", " + " ALIFAP  = " + num4.ToString().Replace(",", ".") + ", " + " ALIQUOTA  = " + num6.ToString().Replace(",", ".") + ", " + " CODGRUASS  = " + num5.ToString() + ", " + " CODQUACON  = " + num7.ToString() + ", " + " CODCON  = " + num8.ToString() + ", " + " CODLOC  = " + num9.ToString() + ", " + " CODLIV  = " + num10.ToString() + ", " + " DENLIV  = " + DBMethods.DoublePeakForSql(str2) + ", " + " TIPRAP  = " + num11.ToString() + ", " + " ABBPRE  = " + DBMethods.DoublePeakForSql(str3) + ", " + " ASSCON  = " + DBMethods.DoublePeakForSql(str4) + " WHERE CODPOS = " + codpos.ToString() + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString(), CommandType.Text);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)"Lorenzo"), JsonConvert.SerializeObject((object)""));

                flag = false;
                messaggio = "Si sono verificati errori durante il salvataggio sulla tabella RapLav. Modulo Agg-RapLav ";
            }
            return flag;
        }

        private bool Module_AggiornaStordl(
          ref string messaggio,
          int intProRap,
          int codpos,
          int intMatricola,
          NuovoIscritto nuovoIscritto,
          string strDataInizio,
          string strDatFin,
          Decimal decPerPeriodo,
          Decimal decPerParTime = 0M,
          string strDataScadenza = "")
        {
            bool flag = false;
            DataTable dataTable1 = new DataTable();
            DataTable dataTable2 = new DataTable();
            string strData1 = "";
            string strData2 = "";
            string strData3 = "";
            string str1 = "";
            string str2 = "";
            string str3 = "";
            Decimal num1 = 0M;
            Decimal num2 = 0M;
            Decimal num3 = 0M;
            string str4 = "";
            Decimal num4 = 0M;
            Decimal num5 = 0M;
            int num6 = 0;
            string str5 = "";
            string str6 = "";
            DataTable dataTable3 = new DataTable();
            int num7 = 0;
            clsIDOC clsIdoc = new clsIDOC();
            try
            {
                int num8 = HttpContext.Current.Session["ProRap"] != null ? Convert.ToInt32(HttpContext.Current.Session["ProRap"]) : intProRap;
                int num9 = intMatricola <= 0 ? Convert.ToInt32(nuovoIscritto.matricola) : intMatricola;
                string strDataNascita = this.objDataAccess.Get1ValueFromSQL("SELECT DATNAS FROM ISCT WHERE MAT = " + num9.ToString(), CommandType.Text);
                DataTable dataTable4 = this.objDataAccess.GetDataTable("SELECT * FROM STORDL WHERE" + " CODPOS = " + codpos.ToString() + " AND MAT = " + num9.ToString() + " AND PRORAP = " + num8.ToString() + " AND DATINI = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strDataInizio)));
                if (dataTable4.Rows.Count == 0)
                    flag = true;
                for (int index1 = 0; index1 <= dataTable4.Rows.Count - 1; ++index1)
                {
                    if (strDataInizio != "")
                    {
                        string strSQL = "SELECT DTTFR, DTFP, DTINF FROM STORDL WHERE" + " CODPOS = " + codpos.ToString() + " AND MAT = " + num9.ToString() + " AND PRORAP = " + num8.ToString() + " AND DATINI < " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strDataInizio)) + " ORDER BY DATINI DESC";
                        dataTable2.Clear();
                        dataTable2 = this.objDataAccess.GetDataTable(strSQL);
                        if (dataTable2.Rows.Count > 0)
                        {
                            str1 = string.IsNullOrEmpty(dataTable2.Rows[0]["DTTFR"].ToString()) ? "" : dataTable2.Rows[0]["DTTFR"].ToString();
                            str2 = string.IsNullOrEmpty(dataTable2.Rows[0]["DTINF"].ToString()) ? "" : dataTable2.Rows[0]["DTINF"].ToString();
                            str3 = string.IsNullOrEmpty(dataTable2.Rows[0]["DTFP"].ToString()) ? "" : dataTable2.Rows[0]["DTFP"].ToString();
                        }
                    }
                    int num10 = Convert.ToInt32(dataTable4.Rows[index1]["CODCON"]);
                    int num11 = string.IsNullOrEmpty(dataTable4.Rows[index1]["CODLOC"].ToString()) ? Convert.ToInt32(dataTable4.Rows[index1]["CODLOC"]) : 0;
                    Decimal num12 = Convert.ToDecimal(dataTable4.Rows[index1]["CODGRUASS"]);
                    string str7 = dataTable4.Rows[index1]["FAP"].ToString().Trim();
                    DataTable dataTable5;
                    int num13;
                    int num14;
                    if (num11 == 0)
                    {
                        string strSQL = "SELECT CODQUACON, PROCON FROM CONRIF " + " WHERE CODCON = " + num10.ToString() + " AND DATDEC <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataTable4.Rows[index1]["DATINI"].ToString())) + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
                        dataTable2.Clear();
                        dataTable5 = this.objDataAccess.GetDataTable(strSQL);
                        if (dataTable5.Rows.Count > 0)
                        {
                            num13 = Convert.ToInt32(dataTable5.Rows[0]["CODQUACON"]);
                            num14 = Convert.ToInt32(dataTable5.Rows[0]["PROCON"]);
                        }
                        else
                        {
                            num13 = 0;
                            num14 = 0;
                        }
                    }
                    else
                    {
                        string strSQL = "SELECT PROCON FROM CONRIF WHERE CODCON = " + num10.ToString() + " AND DATDEC <= " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataTable4.Rows[index1]["DATINI"].ToString())) + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
                        dataTable2.Clear();
                        dataTable5 = this.objDataAccess.GetDataTable(strSQL);
                        num14 = dataTable5.Rows.Count <= 0 ? 0 : Convert.ToInt32(dataTable5.Rows[0]["PROCON"]);
                        num13 = Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL("SELECT CODQUACON FROM QUACON WHERE CODQUACON IN (SELECT CODQUACON FROM CONRIF WHERE CODCON = " + num10.ToString() + ")", CommandType.Text));
                    }
                    string str8 = "SELECT VALUE(SUM(ALIQUOTA), 0.00) AS ALIQUOTA " + " FROM ALIFORASS " + " WHERE ALIFORASS.CODGRUASS = " + dataTable4.Rows[index1]["CODGRUASS"]?.ToString() + " AND ALIFORASS.CODQUACON=" + num13.ToString() + " AND " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataTable4.Rows[index1]["DATINI"].ToString())) + "  BETWEEN ALIFORASS.DATINI AND VALUE(ALIFORASS.DATFIN,'9999-12-31') ";
                    if (clsIdoc.Check_65Anni(Convert.ToDateTime(dataTable4.Rows[index1]["DATINI"]), strDataNascita))
                        str8 += " AND ALIFORASS.CODFORASS IN (SELECT CODFORASS FROM FORASS WHERE CATFORASS <> 'PREV') ";
                    string strSQL1 = str8 + " AND ALIFORASS.CODFORASS NOT IN (SELECT CODFORASS FROM FORASS WHERE CATFORASS = 'FAP') ";
                    dataTable3.Clear();
                    dataTable3 = this.objDataAccess.GetDataTable(strSQL1);
                    Decimal num15 = Convert.ToDecimal(dataTable3.Rows[0]["ALIQUOTA"]);
                    Decimal num16 = !(str7 == "S") ? 0M : Convert.ToDecimal(this.objDataAccess.Get1ValueFromSQL("SELECT VALFAP FROM CODFAP WHERE " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataTable4.Rows[index1]["DATINI"].ToString())) + " BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31')", CommandType.Text));
                    string str9 = this.objDataAccess.Get1ValueFromSQL("SELECT DENLIV FROM CONLIV WHERE CODCON = " + num10.ToString() + " AND PROCON = " + num14.ToString() + " AND CODLIV = " + dataTable4.Rows[index1]["CODLIV"]?.ToString(), CommandType.Text);
                    string strSQL2 = " SELECT * FROM (SELECT (SELECT CATFORASS FROM FORASS WHERE CODFORASS = A.CODFORASS) AS CAT, A.ALIQUOTA " + " FROM ALIFORASS A WHERE A.CODGRUASS =  " + num12.ToString() + " AND A.CODQUACON = " + num13.ToString() + " ) AS TAB WHERE TAB.CAT <> 'FAP'";
                    dataTable5.Clear();
                    dataTable2 = this.objDataAccess.GetDataTable(strSQL2);
                    for (int index2 = 0; index2 <= dataTable2.Rows.Count - 1; ++index2)
                    {
                        string str10 = dataTable2.Rows[index2]["CAT"].ToString().Trim();
                        if (!(str10 == "TFR"))
                        {
                            if (!(str10 == "PREV"))
                            {
                                if (!(str10 == "INF"))
                                    throw new Exception("Caso non gestito " + dataTable2.Rows[index2]["CAT"]?.ToString());
                                num2 = Convert.ToDecimal(dataTable2.Rows[index2]["ALIQUOTA"]);
                            }
                            else
                                num3 = Convert.ToDecimal(dataTable2.Rows[index2]["ALIQUOTA"]);
                        }
                        else
                            num1 = Convert.ToDecimal(dataTable2.Rows[index2]["ALIQUOTA"]);
                    }
                    for (int index3 = 0; index3 <= 2; ++index3)
                    {
                        switch (index3)
                        {
                            case 0:
                                str6 = "TFR";
                                break;
                            case 1:
                                str6 = "PREV";
                                break;
                            case 2:
                                str6 = "INF";
                                break;
                        }
                        if (Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL(" SELECT COUNT(*) FROM ALIFORASS A, FORASS B WHERE A.CODGRUASS =  " + num12.ToString() + " AND A.CODQUACON = " + num13.ToString() + " AND A.CODFORASS = B.CODFORASS AND B.CATFORASS = " + DBMethods.DoublePeakForSql(str6), CommandType.Text)) > 0)
                        {
                            switch (index3)
                            {
                                case 0:
                                    strData1 = !((Decimal)num7 == num12) ? (!(str1 == "") ? str1 : dataTable4.Rows[index1]["DATINI"].ToString()) : (!(str2 == "") ? str1 : dataTable4.Rows[index1]["DATINI"].ToString());
                                    continue;
                                case 1:
                                    strData3 = !((Decimal)num7 == num12) ? (!(str3 == "") ? str3 : dataTable4.Rows[index1]["DATINI"].ToString()) : (!(str3 == "") ? str2 : dataTable4.Rows[index1]["DATINI"].ToString());
                                    continue;
                                case 2:
                                    strData2 = !((Decimal)num7 == num12) ? (!(str2 == "") ? str2 : dataTable4.Rows[index1]["DATINI"].ToString()) : (!(str2 == "") ? str2 : dataTable4.Rows[index1]["DATINI"].ToString());
                                    continue;
                                default:
                                    continue;
                            }
                        }
                    }
                    string str11 = " UPDATE STORDL SET ";
                    string str12 = !(strData1.Trim() != "") ? str11 + " DTTFR = NULL, " + " ALIQTFR = NULL, " : str11 + " DTTFR  = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData1)) + ", " + " ALIQTFR  = " + num1.ToString().Replace(",", ".") + ", ";
                    string str13 = !(strData2.Trim() != "") ? str12 + " DTINF  = NULL, " + " ALIQINF  = NULL, " : str12 + " DTINF  = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData2)) + ", " + " ALIQINF  = " + num2.ToString().Replace(",", ".") + ", ";
                    string str14 = !(strData3.Trim() != "") ? str13 + " DTFP  = NULL, " + " ALIQFP  = NULL, " : str13 + " DTFP  = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(strData3)) + ", " + " ALIQFP  = " + num3.ToString().Replace(",", ".") + ", ";
                    string str15 = (!(str7 == "S") ? str14 + " ALIFAP = NULL, " : str14 + " ALIFAP  = " + num16.ToString().Replace(",", ".") + ", ") + " ALIQUOTA  = " + num15.ToString().Replace(",", ".") + ", " + " CODQUACON  = " + num13.ToString() + ", ";
                    flag = this.objDataAccess.WriteTransactionData((!string.IsNullOrEmpty(str9) ? str15 + " DENLIV  = " + DBMethods.DoublePeakForSql(str9.Trim()) : str15 + " DENLIV  = NULL ") + " WHERE CODPOS = " + codpos.ToString() + " AND MAT = " + num9.ToString() + " AND PRORAP = " + num8.ToString() + " AND DATINI = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataTable4.Rows[index1]["DATINI"].ToString())), CommandType.Text);
                    num7 = Convert.ToInt32(num12);
                    str1 = strData1;
                    str2 = strData2;
                    str3 = strData3;
                    strData1 = "";
                    strData2 = "";
                    strData3 = "";
                    num1 = 0M;
                    num2 = 0M;
                    num3 = 0M;
                    str4 = "";
                    num4 = 0M;
                    num12 = 0M;
                    num5 = 0M;
                    num13 = 0;
                    num10 = 0;
                    num6 = 0;
                    str5 = "";
                    str6 = "";
                    num14 = 0;
                }
            }
            catch (Exception ex)
            {
                flag = false;
                messaggio = "Si sono verificati errori durante il salvataggio sulla tabella STORDL. Modulo AggiornaStordl ";
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)""), JsonConvert.SerializeObject((object)(0, false)));
            }
            return flag;
        }

        private Tuple<int, bool> WriteDatiRapporto(
          int intProRap,
          int codpos,
          int intMatricola,
          NuovoIscritto nuovoIscritto,
          string prorap,
          TFI.OCM.Utente.Utente u,
          ref string messaggio,
          ref string messaggioSuccesso)
        {
            bool flag1 = true;
            bool flag2 = true;
            string[] strArray = new string[12];
            string str1 = "";
            DataTable DT = new DataTable();
            clsIDOC clsIdoc = new clsIDOC();
            iDB2DataAdapter iDb2DataAdapter = new iDB2DataAdapter();
            bool flag3 = true;
            string str2 = "CURRENT TIMESTAMP";
            bool flag4 = false;
            try
            {
                for (short index = 0; (int)index <= strArray.GetUpperBound(0); ++index)
                    strArray[(int)index] = "Null";
                if (!string.IsNullOrEmpty(nuovoIscritto.ragsocAZ))
                {
                    string denComQuery = "SELECT DENDUG FROM DUG WHERE CODDUG = '" + nuovoIscritto.residenzaAZCod.ToUpper() + "'";
                    nuovoIscritto.residenzaAZ = this.objDataAccess.Get1ValueFromSQL(denComQuery, CommandType.Text).Trim();

                    if (!string.IsNullOrEmpty(nuovoIscritto.pozioneAZ.Trim()))
                        strArray[0] = nuovoIscritto.pozioneAZ.Trim();
                    strArray[1] = DBMethods.DoublePeakForSql(nuovoIscritto.ragsocAZ);
                    strArray[2] = DBMethods.DoublePeakForSql(nuovoIscritto.cfAz);
                    strArray[3] = DBMethods.DoublePeakForSql(nuovoIscritto.piAZ);
                    strArray[4] = nuovoIscritto.residenzaAZCod;
                    if (nuovoIscritto.indirizzoAZ.Trim() != "")
                        strArray[5] = DBMethods.DoublePeakForSql(nuovoIscritto.indirizzoAZ);
                    if (nuovoIscritto.numCivAZ.Trim() != "")
                        strArray[6] = DBMethods.DoublePeakForSql(nuovoIscritto.numCivAZ);
                    if (nuovoIscritto.comuneAZ.Trim() != "")
                    {
                        strArray[7] = DBMethods.DoublePeakForSql(nuovoIscritto.comuneAZ);
                        strArray[8] = DBMethods.DoublePeakForSql(nuovoIscritto.comuneAZCod);
                    }
                    if (nuovoIscritto.capAZ.Trim() != "")
                        strArray[9] = DBMethods.DoublePeakForSql(nuovoIscritto.capAZ);
                    if (nuovoIscritto.provinciaAZ.Trim() != "")
                        strArray[10] = DBMethods.DoublePeakForSql(nuovoIscritto.provinciaAZ);
                    if (nuovoIscritto.localitaAZ.Trim() != "")
                        strArray[11] = DBMethods.DoublePeakForSql(nuovoIscritto.localitaAZ);
                }
                if (prorap == null)
                {
                    //Nuovo rapporto di lavoro
                    for (short index = 0; (int)index <= strArray.GetUpperBound(0); ++index)
                        str1 = str1 + strArray[(int)index] + ", ";
                    string strSQL = "SELECT VALUE(MAX(PRORAP), 0) + 1 FROM RAPLAV WHERE MAT = " + intMatricola.ToString();
                    prorap = this.objDataAccess.Get1ValueFromSQL(strSQL, CommandType.Text);
                    if (prorap != "")
                        strSQL = "INSERT INTO RAPLAV (CODPOS, MAT, PRORAP, DATDEC, DATCES, CODCAUCES, DATDENCES, DATASS, DATDEN, DATPRE, DATPRO, ";
                    flag1 = this.objDataAccess.WriteTransactionData(strSQL + "NUMPRO, MATFON, PROFON, CODPOSFOR, RAGSOCFOR, CODFISFOR, PARIVAFOR, CODDUGFOR, INDFOR, NUMCIVFOR, DENCOMFOR, " + "CODCOMFOR, CAPFOR, SIGPROFOR, DENLOCFOR, ULTAGG, FLGAPP, UTEAGG) VALUES (" + codpos.ToString() + ", " + intMatricola.ToString() + ", " + prorap + ", '" + DBMethods.Db2Date(nuovoIscritto.datIsc) + "', Null, Null, Null, '" + DBMethods.Db2Date(nuovoIscritto.datIsc) + "', " + "'" + DBMethods.Db2Date(nuovoIscritto.datDen) + "', Null, Null, 0, Null, Null, " + str1 + " CURRENT_TIMESTAMP, 'I', '" + u.Username + "')", CommandType.Text);
                    if (flag1)
                    {
                        flag1 = this.objDataAccess.WriteTransactionData("UPDATE ISCT SET DATISC = '" + DBMethods.Db2Date(nuovoIscritto.datIsc) + "' WHERE MAT = " + intMatricola.ToString(), CommandType.Text);
                        if (flag1)
                            flag1 = this.AGGIORNA_RAPLAV_INPS(codpos, intMatricola, Convert.ToInt32(prorap), "I", u);
                    }
                }
                else
                {
                    //RapportiLavoro rapportiLavoro = (RapportiLavoro)HttpContext.Current.Session["ModificaIscritto"];
                    //if (rapportiLavoro.modIsc.titoloStudio != nuovoIscritto.titoloStudio || rapportiLavoro.modIsc.indirizzo != nuovoIscritto.indirizzo || rapportiLavoro.modIsc.civico != nuovoIscritto.civico || rapportiLavoro.modIsc.comune != nuovoIscritto.comune || rapportiLavoro.modIsc.cap != nuovoIscritto.cap || rapportiLavoro.modIsc.provincia != nuovoIscritto.provincia || rapportiLavoro.modIsc.localita != nuovoIscritto.localita || rapportiLavoro.modIsc.statoEs != nuovoIscritto.statoEs || rapportiLavoro.modIsc.co != nuovoIscritto.co || rapportiLavoro.modIsc.tel != nuovoIscritto.tel || rapportiLavoro.modIsc.tel2 != nuovoIscritto.tel2 || rapportiLavoro.modIsc.cell != nuovoIscritto.cell || rapportiLavoro.modIsc.email != nuovoIscritto.email || rapportiLavoro.modIsc.pec != nuovoIscritto.pec || rapportiLavoro.modIsc.fax != nuovoIscritto.fax)
                    //{
                    //    DT = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0016", codpos, intMatricola, Convert.ToInt32(str4), 0, "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "MODIFICA RDL", "", "");
                    //    clsIdoc.WRITE_IDOC_TESTATA(this.objDataAccess, DT.Rows[0]);
                    //    flag3 = true;
                    //}
                    //DT.Clear();
                    //DT = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0016", codpos, intMatricola, Convert.ToInt32(str4), 0, "", "", "9999-12-31", "", "", nuovoIscritto.datIsc, 0, 0, 0, "D", "MODIFICA RDL", "", "", flgWEB: "S");
                    //clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "9001", DT, true);
                    //clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "0016", DT, true);
                    //var datDenChecked = DBMethods.Db2Date(nuovoIscritto.datDen) == "null" ? "null" : $"'{DBMethods.Db2Date(nuovoIscritto.datDen)}'";
                    string updateRaplav = "UPDATE RAPLAV SET FLGAPP = 'M', DATASS = '" + DBMethods.Db2Date(nuovoIscritto.datDecMod)
                                          + "', CODPOSFOR = " + strArray[0] + ", RAGSOCFOR = " + strArray[1] + ", " +
                                          "CODFISFOR = " + strArray[2] + ", PARIVAFOR = " + strArray[3] + ", " + "CODDUGFOR = " + strArray[4] + ", INDFOR = " + strArray[5] + ", " +
                                          "NUMCIVFOR = " + strArray[6] + ", DENCOMFOR = " + strArray[7] + ", CODCOMFOR = " + strArray[8] + ", CAPFOR = " + strArray[9] + ", " +
                                          "SIGPROFOR = " + strArray[10] + ", DENLOCFOR = " + strArray[11] + ", DATINPS = Null, TIPOPE = 'I'" + ", DATSAP = Null, " +
                                          "ULTAGG = CURRENT_TIMESTAMP, UTEAGG = '" + u.Username + "', TIPRAP = " + nuovoIscritto.tipRap + " WHERE CODPOS = " + codpos.ToString() + " AND MAT = " + intMatricola.ToString() +
                                          " AND PRORAP = " + prorap;
                    flag1 = this.objDataAccess.WriteTransactionData(updateRaplav, CommandType.Text);
                    if (flag1)
                    {
                        flag1 = this.AGGIORNA_RAPLAV_INPS(codpos, intMatricola, Convert.ToInt32(prorap), "V", u);
                        //if (flag1)
                        //{
                        //    string str5 = DateTime.Parse(nuovoIscritto.datDen).Year.ToString();
                        //    string str6 = DateTime.Parse(nuovoIscritto.datDen).Month.ToString();
                        //    flag1 = this.objDataAccess.WriteTransactionData("DELETE FROM MODRDL WHERE CODPOS = " + codpos.ToString() + " AND MAT = " + intMatricola.ToString() + " AND PRORAP = " + prorap + " AND DATELA IS NULL AND ANNDA >= " + str5 + " AND MESDA >= " + str6, CommandType.Text);
                        //    if (flag1)
                        //    {
                        //        if (Convert.ToInt16("0" + this.objDataAccess.Get1ValueFromSQL("SELECT COUNT(*) AS NUMREC FROM MODRDL WHERE CODPOS = " + Convert.ToInt32(codpos).ToString() + " AND MAT = " + intMatricola.ToString() + " AND PRORAP = " + prorap + " AND DATELA IS NULL AND ANNDA < " + str5 + " AND MESDA < " + str6, CommandType.Text)) == (short)0)
                        //        {
                        //            string str7 = this.objDataAccess.Get1ValueFromSQL(" SELECT VALUE(MAX(PROREC),0) + 1  FROM MODRDL WHERE CODPOS = " + codpos.ToString() + " AND MAT = " + intMatricola.ToString() + " AND PRORAP = " + prorap, CommandType.Text);
                        //            flag1 = this.objDataAccess.WriteTransactionData(" INSERT INTO MODRDL (CODPOS, MAT, PRORAP, PROREC, ANNDA, MESDA, DATELA, ULTAGG, UTEAGG) VALUES (" + codpos.ToString() + ", " + intMatricola.ToString() + ", " + prorap + ", " + str7 + "," + str5 + ", " + str6 + ", NULL, CURRENT_TIMESTAMP, '" + u.Username + "')", CommandType.Text);
                        //        }
                        //    }
                        //}
                    }
                }
                if (flag1)
                {
                    intProRap = Convert.ToInt32("0" + prorap);
                    //messaggioSuccesso = "Dati Salvati Correttamente";
                    //if (!flag4 && string.IsNullOrEmpty(nuovoIscritto.matricola))
                    //{
                    //    DT.Clear();
                    //    DataTable idocDatiE1Pitype1 = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0000", 0, intMatricola, 0, 0, "A1", "01", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                    //    clsIdoc.WRITE_IDOC_TESTATA(this.objDataAccess, idocDatiE1Pitype1.Rows[0]);
                    //    flag3 = true;
                    //    clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "0000", idocDatiE1Pitype1, false);
                    //    for (short index = 0; (int)index <= idocDatiE1Pitype1.Rows.Count - 1; ++index)
                    //    {
                    //        idocDatiE1Pitype1.Rows[(int)index]["DATFIN"] = idocDatiE1Pitype1.Rows[(int)index]["DATINI"];
                    //        idocDatiE1Pitype1.Rows[(int)index]["DATBEGDA"] = idocDatiE1Pitype1.Rows[(int)index]["DATINI"];
                    //        idocDatiE1Pitype1.Rows[(int)index]["DATENDDA"] = idocDatiE1Pitype1.Rows[(int)index]["DATINI"];
                    //    }
                    //    clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "0302", idocDatiE1Pitype1, false);
                    //    for (short index = 0; (int)index <= idocDatiE1Pitype1.Rows.Count - 1; ++index)
                    //    {
                    //        idocDatiE1Pitype1.Rows[(int)index]["DATFIN"] = (object)"9999-12-31";
                    //        idocDatiE1Pitype1.Rows[(int)index]["DATENDDA"] = (object)"9999-12-31";
                    //    }
                    //    clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "0001", idocDatiE1Pitype1, false);
                    //    for (short index = 0; (int)index <= idocDatiE1Pitype1.Rows.Count - 1; ++index)
                    //    {
                    //        idocDatiE1Pitype1.Rows[(int)index]["DATINI"] = idocDatiE1Pitype1.Rows[(int)index]["DATNAS"];
                    //        idocDatiE1Pitype1.Rows[(int)index]["DATBEGDA"] = idocDatiE1Pitype1.Rows[(int)index]["DATNAS"];
                    //    }
                    //    clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "0002", idocDatiE1Pitype1, false);
                    //    for (short index = 0; (int)index <= idocDatiE1Pitype1.Rows.Count - 1; ++index)
                    //    {
                    //        idocDatiE1Pitype1.Rows[(int)index]["DATINISTO"] = (object)nuovoIscritto.datIsc;
                    //        idocDatiE1Pitype1.Rows[(int)index]["DATBEGDA"] = (object)"1800-01-01";
                    //    }
                    //    clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "0003", idocDatiE1Pitype1, false);
                    //    idocDatiE1Pitype1.Clear();
                    //    DataTable idocDatiE1Pitype2 = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0006", 0, intMatricola, 0, 0, "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                    //    clsIdoc.WRITE_IDOC_E1PITYP(this.objDataAccess, "0006", idocDatiE1Pitype2, false);
                    //}
                    //else if (flag4 && string.IsNullOrEmpty(nuovoIscritto.matricola))
                    //{
                    //    DT.Clear();
                    //    DataTable idocDatiE1Pitype = clsIdoc.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0000", 0, intMatricola, 0, 0, "A1", "01", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                    //    clsIdoc.WRITE_IDOC_TESTATA(this.objDataAccess, idocDatiE1Pitype.Rows[0]);
                    //    flag3 = true;
                    //}
                }
                else
                {
                    messaggio = "Errore nel salvataggio dei dati rapporto di lavoro";
                }
            }
            catch (Exception ex)
            {
                _log.Info($"[DAL - RapportiLavoroDAL- WriteDatiRapporto] - Eccezione generata: {ex.Message} - stackTrace: {ex.StackTrace}");
                messaggio = "Si sono verificati errori durante il salvataggio sulla tabella dei Rapporti di Lavoro.";
                flag1 = false;
            }
            return Tuple.Create<int, bool>(intProRap, flag1);
        }

        private bool AGGIORNA_RAPLAV_INPS(
          int CODPOS,
          int MAT,
          int PRORAP,
          string FLGOPERAZIONE,
          TFI.OCM.Utente.Utente u)
        {
            return this.objDataAccess.WriteTransactionData("UPDATE RAPLAV SET DATINPS = NULL, TIPOPE = 'M', " + " ULTAGG = CURRENT_TIMESTAMP, UTEAGG = " + DBMethods.DoublePeakForSql(u.Username) + " WHERE CODPOS = " + CODPOS.ToString() + " AND MAT=" + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString(), CommandType.Text);
        }

        private bool WritePartTime(
          ref string messaggio,
          string prorap,
          int codpos,
          int intMatricola,
          int tiprap,
          string strDataInizio,
          TFI.OCM.Utente.Utente u,
          NuovoIscritto nuovoIscritto)
        {
            ArrayList arrayList = new ArrayList();
            string str = "CURRENT TIMESTAMP";
            bool flag = false;
            try
            {
                flag = true;
                switch (tiprap)
                {
                    case 11:
                    case 16:
                    case 17:
                        foreach (string mese in nuovoIscritto.meseSel)
                        {
                            flag = this.objDataAccess.WriteTransactionData("INSERT INTO PARTIMM (CODPOS, MAT, PRORAP, DATINI, PARMES, ULTAGG, UTEAGG) VALUES (" + codpos.ToString() + ", " + intMatricola.ToString() + ", " + prorap + ", '" + DBMethods.Db2Date(strDataInizio) + "', " + mese + ", CURRENT_TIMESTAMP, '" + u.Username + "')", CommandType.Text);
                            if (!flag)
                                break;
                        };
                        break;
                        //IEnumerator enumerator = new ArrayList((ICollection)nuovoIscritto.meseSel).GetEnumerator();
                        //try
                        //{
                        //    while (enumerator.MoveNext())
                        //    {
                        //        //bool current = (bool)enumerator.Current;
                        //        //if (current)
                        //            flag = this.objDataAccess.WriteTransactionData("INSERT INTO PARTIMM (CODPOS, MAT, PRORAP, DATINI, PARMES, ULTAGG, UTEAGG) VALUES (" + codpos.ToString() + ", " + intMatricola.ToString() + ", " + prorap + ", '" + DBMethods.Db2Date(strDataInizio) + "', " + enumerator.Current.ToString() + ", " + str + ", '" + u.Username + "')", CommandType.Text);
                        //        if (!flag)
                        //            break;
                        //    }
                        //    break;
                        //}
                        //finally
                        //{
                        //    if (enumerator is IDisposable disposable)
                        //        disposable.Dispose();
                        //}
                }
            }
            catch (Exception ex)
            {
                _log.Info($"[DAL - RapportiLavoroDAL- WritePartTime] - Eccezione generata: {ex.Message} - stackTrace: {ex.StackTrace}");
                flag = false;
            }
            finally
            {
                if (!flag)
                    messaggio = "Si sono verificati errori durante il salvataggio sulla tabella PARTIMM.";
            }
            return flag;
        }

        private bool CheckRDL(string prorap, NuovoIscritto n, string codpos, ref string messaggio)
        {
            bool flag = true;
            if (prorap == null)
            {
                DataTable dataTable = this.objDataAccess.GetDataTable("SELECT CODPOS FROM RAPLAV A INNER JOIN ISCT B ON A.MAT = B.MAT WHERE B.COG = '" + n.cognome.Trim().Replace("'", "''") + "' AND B.NOM = '" + n.nome.Trim().Replace("'", "''") + "' AND B.CODFIS = '" + n.codFis.Trim().Replace("'", "''") + "' AND DATCES IS " + "NULL AND CODCAUCES IS NULL AND CODPOS = " + codpos);
                if (dataTable != null)
                {
                    if (dataTable.Rows.Count > 0)
                    {
                        flag = false;
                        messaggio = "Il soggetto ha gia\\' un rapporto di lavoro attivo con questa posizione!";
                    }
                    else
                        flag = true;
                }
                else
                    flag = true;
            }
            return flag;
        }

        public RapportiLavoro GetAliquota(
          string strData,
          string codcon,
          string dencon,
          string codpos)
        {
            bool flag = false;
            RapportiLavoro aliquota1 = new RapportiLavoro();
            List<Aliquota> aliquotaList = new List<Aliquota>();
            string str1 = Convert.ToString(((DataView)HttpContext.Current.Session["ContrattiView"]).ToTable().Select("DENCON like '%" + dencon.Trim().Replace("'", "'''+'") + "%'")[0]["CODQUACON"]);
            try
            {
                if (!string.IsNullOrEmpty(str1))
                {
                    string str2 = "SELECT DISTINCT B.CODGRUASS, B.DENGRUASS, B.GRUDEF, B.GRUDEFCON, B.GRUDEFENP FROM ALIFORASS A INNER " + "JOIN GRUASS B ON A.CODGRUASS = B.CODGRUASS WHERE (B.CODQUACON = " + str1 + " OR B.CODQUACON IS " + "NULL) AND B.GRUCON IN ";
                    string str3;
                    string str4;
                    if (HttpContext.Current.Session["GruCon"].ToString() == "S")
                    {
                        str3 = str2 + "('E', 'C')" + " and GRUDEFCON = 'S'";
                        str4 = "GRUDEFCON";
                    }
                    else if (Convert.ToInt32("0" + this.objDataAccess.Get1ValueFromSQL("SELECT COUNT(CODPOS) FROM PARGENPOS WHERE CODPOS = " + codpos, CommandType.Text)) > 0)
                    {
                        str3 = str2 + "('E', 'A')";
                        str4 = "GRUDEFENP";
                    }
                    else
                    {
                        str3 = str2 + "('E', 'A')" + " AND GRUDEFCON <> 'S'";
                        str4 = "GRUDEF";
                    }
                    string strSQL = str3 + " AND B.GRUWEB = 'S' AND '" + DBMethods.Db2Date(strData) + "' BETWEEN A.DATINI AND " + "VALUE(A.DATFIN,'9999-12-31') ORDER BY DENGRUASS";
                    if (this.objDataAccess == null)
                        this.objDataAccess = new DataLayer();
                    else
                        flag = true;
                    foreach (DataRow row in (InternalDataCollectionBase)this.objDataAccess.GetDataTable(strSQL).Rows)
                    {
                        Aliquota aliquota2 = new Aliquota()
                        {
                            DENGRUASS = row["DENGRUASS"].ToString(),
                            CODGRUASS = row["CODGRUASS"].ToString()
                        };
                        aliquotaList.Add(aliquota2);
                    }
                    aliquota1.aliq = aliquotaList;
                }
                return aliquota1;
            }
            catch (Exception ex)
            {
                return (RapportiLavoro)null;
            }
        }

        public RapportiLavoro GetCorrectAliquota(
          string strData,
          string codcon,
          string dencon,
          string codpos)
        {
            bool flag = false;
            RapportiLavoro aliquota1 = new RapportiLavoro();
            List<Aliquota> aliquotaList = new List<Aliquota>();
            string str1 = Convert.ToString(((DataView)HttpContext.Current.Session["ContrattiView"]).ToTable().Select("DENCON like '%" + dencon.Trim().Replace("'", "'''+'") + "%'")[0]["CODQUACON"]);
            try
            {
                if (!string.IsNullOrEmpty(str1))
                {
                    string str2 = "SELECT DISTINCT B.CODGRUASS, B.DENGRUASS, B.GRUDEF, B.GRUDEFCON, B.GRUDEFENP FROM ALIFORASS A INNER " + "JOIN GRUASS B ON A.CODGRUASS = B.CODGRUASS WHERE (B.CODQUACON = " + str1 + " OR B.CODQUACON IS " + "NULL) AND B.GRUCON IN ";
                    string str3;
                    string str4;
                    if (HttpContext.Current.Session["GruCon"].ToString() == "S")
                    {
                        str3 = str2 + "('E', 'C')" + " and GRUDEFCON = 'S'";
                        str4 = "GRUDEFCON";
                    }
                    else if (Convert.ToInt32("0" + this.objDataAccess.Get1ValueFromSQL("SELECT COUNT(CODPOS) FROM PARGENPOS WHERE CODPOS = " + codpos, CommandType.Text)) > 0)
                    {
                        str3 = str2 + "('E', 'A')";
                        str4 = "GRUDEFENP";
                    }
                    else
                    {
                        str3 = str2 + "('E', 'A')" + " AND GRUDEFCON <> 'S'";
                        str4 = "GRUDEF";
                    }
                    string strSQL = str3 + " AND B.GRUWEB = 'S' AND '" + DBMethods.Db2Date(strData) + "' BETWEEN A.DATINI AND " + "VALUE(A.DATFIN,'9999-12-31') ORDER BY DENGRUASS";
                    if (this.objDataAccess == null)
                        this.objDataAccess = new DataLayer();
                    else
                        flag = true;

                    var aliqTable = this.objDataAccess.GetDataTable(strSQL);
                    foreach (DataRow row in aliqTable.Rows)
                    {
                        if (row[str4].ToString() == "S")
                        {
                            Aliquota aliquota2 = new Aliquota()
                            {
                                DENGRUASS = row["DENGRUASS"].ToString(),
                                CODGRUASS = row["CODGRUASS"].ToString()
                            };
                            aliquotaList.Add(aliquota2);
                            break;
                        }
                    }
                    aliquota1.aliq = aliquotaList;
                }
                return aliquota1;
            }
            catch (Exception ex)
            {
                return (RapportiLavoro)null;
            }
        }

        public RapportiLavoro GetPercentuali(
          string CODGRUASS,
          string dencon,
          string datIsc,
          string datMod,
          ref string messaggio,
          string dataNas,
          bool fap)
        {
            Decimal num1 = 0M;
            bool flag = true;
            RapportiLavoro percentuali = new RapportiLavoro();
            string str1 = Convert.ToString(((DataView)HttpContext.Current.Session["ContrattiView"]).ToTable().Select("DENCON like '%" + dencon.Trim().Replace("'", "'''+'") + "%'")[0]["CODQUACON"]);
            if (Convert.ToInt32(CODGRUASS) > 0 && flag)
            {
                string strData = HttpContext.Current.Session["ProRap"] == null ? datIsc : (string.IsNullOrEmpty(datMod) ? datIsc : datMod);
                string strSQL1 = "SELECT A.CODGRUASS, C.DENFORASS, B.CODFORASS, B.ALIQUOTA, A.GRUDEF FROM GRUASS A " + "INNER JOIN ALIFORASS B ON A.CODGRUASS = B.CODGRUASS INNER JOIN FORASS C ON " + "B.CODFORASS = C.CODFORASS WHERE A.CODGRUASS = " + CODGRUASS + " AND B.CODQUACON = " + str1 + " AND '" + DBMethods.Db2Date(strData) + "' BETWEEN B.DATINI AND " + "VALUE(B.DATFIN,'9999-12-31') AND C.CATFORASS <> 'FAP' ";
                if (this.RemoveFondoPrevidenza(dataNas, ref messaggio))
                    strSQL1 += " AND C.CATFORASS <> 'PRE'";
                if (this.objDataAccess == null)
                    this.objDataAccess = new DataLayer();
                DataTable dataTable = this.objDataAccess.GetDataTable(strSQL1);
                if (dataTable.Rows.Count > 0)
                {
                    int num2 = dataTable.Rows.Count - 1;
                    for (short index = 0; (int)index <= num2; ++index)
                        num1 += Convert.ToDecimal("0" + dataTable.Rows[(int)index]["ALIQUOTA"]?.ToString());
                }
                string strSQL2 = "SELECT VALFAP FROM CODFAP WHERE '" + DBMethods.Db2Date(strData) + "' BETWEEN " + "DATINI AND VALUE(DATFIN,'9999-12-31')";
                this.objDataAccess.Get1ValueFromSQL(strSQL2, CommandType.Text);
                Decimal num3 = Convert.ToDecimal(this.objDataAccess.Get1ValueFromSQL(strSQL2, CommandType.Text) ?? "");
                if (fap)
                    num1 += Convert.ToDecimal("0" + num3.ToString());
                string str2 = num1.ToString("#,##0.#0");
                dataTable.Dispose();
                percentuali.aliquota = str2;
            }
            return percentuali;
        }

        private bool RemoveFondoPrevidenza(string dataNas, ref string messaggio)
        {
            if (string.IsNullOrEmpty(dataNas))
                messaggio = "Per determinare correttamente le aliquote inserire la data di nascita!";
            else if (DateTime.Compare(DateTime.Today, DateTime.Parse(dataNas).AddYears(65)) >= 0)
                return true;
            return false;
        }

        public RapportiLavoro GetMensilitames(string dencon)
        {
            try
            {
                DataTable table = ((DataView)HttpContext.Current.Session["ContrattiView"]).ToTable();
                RapportiLavoro mensilitames = new RapportiLavoro();
                DataLayer dataLayer = new DataLayer();
                List<Livello> livelloList = new List<Livello>();
                RapportiLavoro.DatiRapporto datiRapporto = new RapportiLavoro.DatiRapporto();
                string str1 = dencon.Trim().Replace("'", "'''+'");
                DataRow[] dataRowArray = table.Select("DENCON like '%" + str1 + "%'");
                Convert.ToString(dataRowArray[0]["PROCON"]);
                Convert.ToString(dataRowArray[0]["ASSCON"]);
                int int32_1 = (int)(short)Convert.ToInt32(dataRowArray[0]["CODLOC"]);
                int int32_2 = (int)(short)Convert.ToInt32(dataRowArray[0]["CODCON"]);
                string str2 = Convert.ToString(dataRowArray[0]["TIPSPE"]);
                datiRapporto.qualifica = Convert.ToString(dataRowArray[0]["DENQUA"]);
                datiRapporto.mensilita = Convert.ToString(dataRowArray[0]["NUMMEN"]);
                datiRapporto.m14 = Convert.ToString(dataRowArray[0]["M14"]);
                if (string.IsNullOrEmpty(datiRapporto.m14))
                    datiRapporto.m14 = "0";
                datiRapporto.m15 = Convert.ToString(dataRowArray[0]["M15"]);
                if (string.IsNullOrEmpty(datiRapporto.m15))
                    datiRapporto.m15 = "0";
                datiRapporto.m16 = Convert.ToString(dataRowArray[0]["M16"]);
                if (string.IsNullOrEmpty(datiRapporto.m16))
                    datiRapporto.m16 = "0";
                mensilitames.datRap.qualifica = datiRapporto.qualifica;
                mensilitames.datRap.mensilita = datiRapporto.mensilita;
                mensilitames.datRap.m14 = datiRapporto.m14;
                mensilitames.datRap.m15 = datiRapporto.m15;
                mensilitames.datRap.m16 = datiRapporto.m16;
                mensilitames.datRap.tipspe = str2;
                mensilitames.datRap.codloc = int32_1.ToString();
                return mensilitames;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public RapportiLavoro AggiornaScatti(
          string numscatti,
          string dencon,
          string codliv,
          string datmod,
          string datini,
          string prorap,
          string tipRap)
        {
            DateTimeFormatInfo dateTimeFormat = new CultureInfo("it-IT", false).DateTimeFormat;
            RapportiLavoro rapportiLavoro = new RapportiLavoro();
            DataView dataView = (DataView)HttpContext.Current.Session["ContrattiView"];
            if (dataView == null)
                return (RapportiLavoro)null;
            DataRow[] dataRowArray = dataView.ToTable().Select("DENCON like '%" + dencon.Trim().Replace("'", "'''+'") + "%'");
            string str1 = Convert.ToString(dataRowArray[0]["CODCON"]);
            string str2 = Convert.ToString(dataRowArray[0]["PROCON"]);
            try
            {
                if (Convert.ToInt32(str1) > 0 && Convert.ToInt32("0" + numscatti) > 0)
                {
                    string strData = string.IsNullOrEmpty(datmod) ? (string.IsNullOrEmpty(datini) ? DateTime.Now.GetDateTimeFormats((IFormatProvider)dateTimeFormat)[0] : datini) : datmod;
                    if (this.objDataAccess == null)
                        this.objDataAccess = new DataLayer();
                    Decimal num1 = Convert.ToDecimal("0" + this.objDataAccess.Get1ValueFromSQL("SELECT IMPVOCRET FROM CONRET WHERE CODCON = " + str1 + " AND PROCON = " + str2 + " AND CODLIV = " + codliv + " AND '" + DBMethods.Db2Date(strData) + "' BETWEEN DATAPPINI AND VALUE(DATAPPFIN, " + "'9999-12-31') AND CODVOCRET = 4", CommandType.Text));
                    rapportiLavoro.modIsc.importoSc = num1.ToString();
                    switch (Convert.ToInt32("0" + tipRap))
                    {
                        case 3:
                        case 4:
                        case 6:
                        case 7:
                        case 8:
                        case 10:
                        case 11:
                        case 13:
                            if (!string.IsNullOrEmpty(rapportiLavoro.modIsc.PercPT))
                            {
                                Decimal num2 = Convert.ToDecimal("0" + rapportiLavoro.modIsc.PercPT);
                                num1 = num1 / 100M * num2;
                            }
                            if (prorap == null)
                            {
                                if (!string.IsNullOrEmpty(rapportiLavoro.modIsc.PercPT))
                                {
                                    Decimal num3 = Convert.ToDecimal("0" + rapportiLavoro.modIsc.PercPT);
                                    num1 = num1 / 100M * num3;
                                    break;
                                }
                                break;
                            }
                            if (!string.IsNullOrEmpty(rapportiLavoro.modIsc.PercPT))
                            {
                                Decimal num4 = Convert.ToDecimal("0" + rapportiLavoro.modIsc.PercPT);
                                num1 = num1 / 100M * num4;
                                break;
                            }
                            break;
                    }
                    rapportiLavoro.modIsc.importoSc = ((Decimal)Convert.ToInt32(numscatti) * num1).ToString("#,##0.#0");
                }
                else if (Convert.ToInt32("0" + numscatti) == 0)
                    rapportiLavoro.modIsc.importoSc = "0,00";
                return rapportiLavoro;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public RapportiLavoro AggiornaEmolumenti(
          string codliv,
          string dencon,
          string datMod)
        {
            try
            {
                DataTable table = ((DataView)HttpContext.Current.Session["ContrattiView"]).ToTable();
                RapportiLavoro rapportiLavoro = new RapportiLavoro();
                DataLayer dataLayer = new DataLayer();
                List<Livello> livelloList = new List<Livello>();
                RapportiLavoro.DatiRapporto datiRapporto = new RapportiLavoro.DatiRapporto();
                string str = dencon.Trim().Replace("'", "'''+'");
                DataRow[] dataRowArray = table.Select("DENCON like '%" + str + "%'");
                int int32_1 = Convert.ToInt32(dataRowArray[0]["PROCON"]);
                int int32_2 = Convert.ToInt32(dataRowArray[0]["CODLOC"]);
                int int32_3 = Convert.ToInt32(dataRowArray[0]["CODCON"]);
                int int32_4 = Convert.ToInt32(dataRowArray[0]["PROLOC"]);
                Decimal minimoContrattuale = this.GetMinimoContrattuale(int32_3, int32_1, int32_2, int32_4, Convert.ToInt32(codliv), datMod, 0M, 0M);
                rapportiLavoro.modIsc.emolumenti = minimoContrattuale.ToString();
                return rapportiLavoro;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public RapportiLavoro CaricaDatiMatricola(string codfis)
        {
            RapportiLavoro rapportiLavoro = new RapportiLavoro();
            DataLayer dataLayer = new DataLayer();
            try
            {
                string strSQL1 = "SELECT MAT, COG, NOM, DATNAS, CURRENT_DATE AS TODAY," + " CODCOM AS CODCOMNAS, TITSTU, (SELECT DENCOM FROM CODCOM WHERE CODCOM.CODCOM = VALUE(ISCT.CODCOM,'@@@@@')) AS DENCOMNAS, " + " (SELECT SIGPRO FROM CODCOM WHERE CODCOM.CODCOM = VALUE(ISCT.CODCOM,'@@@@@')) AS SIGPRONAS, " + " CODFIS, SES, DATCHIISC, COUCHIISC, STACIV, (SELECT DENSTACIV FROM STACIV WHERE STACIV.CODSTACIV = VALUE(ISCT.STACIV ,0)) AS DENSTACIV, " + " TITSTU, (SELECT DENTITSTU FROM TITSTU WHERE TITSTU.CODTITSTU = VALUE(ISCT.TITSTU ,0)) AS DENTITSTU, ULTAGG, UTEAGG" + " FROM ISCT " + " WHERE CODFIS = '" + codfis + "'";
                string Err = "";
                DataSet dataSet1 = dataLayer.GetDataSet(strSQL1, ref Err);
                rapportiLavoro.modificaIscritto = new List<NuovoIscritto>();
                if (dataSet1.Tables[0].Rows.Count > 0)
                {
                    rapportiLavoro.modIsc.codFis = Convert.ToString(dataSet1.Tables[0].Rows[0]["CODFIS"]).Trim();
                    rapportiLavoro.modIsc.matricola = Convert.ToString(dataSet1.Tables[0].Rows[0]["MAT"]).Trim();
                    rapportiLavoro.modIsc.cognome = Convert.ToString(dataSet1.Tables[0].Rows[0]["COG"]).Trim();
                    rapportiLavoro.modIsc.nome = Convert.ToString(dataSet1.Tables[0].Rows[0]["NOM"]).Trim();
                    rapportiLavoro.modIsc.dataNas = dataSet1.Tables[0].Rows[0]["DATNAS"].ToString().Substring(0, 10);
                    rapportiLavoro.modIsc.sesso = dataSet1.Tables[0].Rows[0]["SES"].ToString();
                    Convert.ToString(dataSet1.Tables[0].Rows[0]["DENCOMNAS"]);
                    rapportiLavoro.modIsc.comuneN = Convert.ToString(dataSet1.Tables[0].Rows[0]["DENCOMNAS"]).Trim() ?? "";
                    rapportiLavoro.modIsc.comuneNCod = dataSet1.Tables[0].Rows[0]["CODCOMNAS"].ToString();
                    rapportiLavoro.modIsc.provinciaN = dataSet1.Tables[0].Rows[0]["SIGPRONAS"]?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(dataSet1.Tables[0].Rows[0]["DENTITSTU"].ToString()))
                    {
                        rapportiLavoro.modIsc.titoloStudio = dataSet1.Tables[0].Rows[0]["DENTITSTU"].ToString().Trim();
                        rapportiLavoro.modIsc.titoloStudioCod = dataSet1.Tables[0].Rows[0]["TITSTU"].ToString().Trim();
                    }
                }
                string strSQL2 = "SELECT  MAX(datini)  FROM Iscd WHERE  MAT = " + rapportiLavoro.modIsc.matricola;
                string strSQL3 = "SELECT*FROM Iscd WHERE DATINI ='" + DateTime.Parse(dataLayer.Get1ValueFromSQL(strSQL2, CommandType.Text).Substring(0, 10).ToString()).ToString("yyyy-MM-dd") + "' AND MAT = " + rapportiLavoro.modIsc.matricola;
                DataSet dataSet2 = dataLayer.GetDataSet(strSQL3, ref Err);
                if (dataSet2.Tables[0].Rows.Count > 0)
                {
                    int int32 = Convert.ToInt32(dataSet2.Tables[0].Rows[0]["CODDUG"]);
                    string strQuery = "SELECT CODDUG,DENDUG FROM DUG";
                    iDB2DataReader dataReaderFromQuery = dataLayer.GetDataReaderFromQuery(strQuery, CommandType.Text);
                    while (dataReaderFromQuery.Read())
                        rapportiLavoro.modificaIscritto.Add(new NuovoIscritto()
                        {
                            indirizzoCod = dataReaderFromQuery["CODDUG"].ToString(),
                            indirizzo = dataReaderFromQuery["DENDUG"].ToString()
                        });
                    string strSQL4 = "SELECT CODDUG,DENDUG FROM DUG WHERE CODDUG = " + int32.ToString();
                    DataSet dataSet3 = dataLayer.GetDataSet(strSQL4, ref Err);
                    if (dataSet3.Tables[0].Rows.Count > 0)
                    {
                        rapportiLavoro.modIsc.indirizzoCod = dataSet3.Tables[0].Rows[0]["CODDUG"].ToString().Trim();
                        rapportiLavoro.modIsc.indirizzo = dataSet3.Tables[0].Rows[0]["DENDUG"].ToString().Trim();
                    }
                    rapportiLavoro.modIsc.residenza = dataSet2.Tables[0].Rows[0]["IND"].ToString().Trim();
                    rapportiLavoro.modIsc.civico = dataSet2.Tables[0].Rows[0]["NUMCIV"].ToString().Trim();
                    string strSQL5 = "SELECT dencom,sigpro FROM codcom WHERE codcom='" + dataSet2.Tables[0].Rows[0]["CODCOM"].ToString() + "'";
                    DataSet dataSet4 = dataLayer.GetDataSet(strSQL5, ref Err);
                    if (dataSet4.Tables[0].Rows.Count > 0)
                    {
                        rapportiLavoro.modIsc.comune = dataSet4.Tables[0].Rows[0]["DENCOM"].ToString().Trim();
                        rapportiLavoro.modIsc.provincia = dataSet4.Tables[0].Rows[0]["SIGPRO"].ToString().Trim();
                    }
                    rapportiLavoro.modIsc.cap = dataSet2.Tables[0].Rows[0]["CAP"].ToString().Trim();
                    rapportiLavoro.modIsc.localita = dataSet2.Tables[0].Rows[0]["DENLOC"].ToString().Trim();
                    rapportiLavoro.modIsc.statoEs = dataSet2.Tables[0].Rows[0]["DENSTAEST"].ToString().Trim();
                    rapportiLavoro.modIsc.co = dataSet2.Tables[0].Rows[0]["CO"].ToString().Trim();
                    rapportiLavoro.modIsc.tel = dataSet2.Tables[0].Rows[0]["TEL1"].ToString().Trim();
                    rapportiLavoro.modIsc.tel2 = dataSet2.Tables[0].Rows[0]["TEL2"].ToString().Trim();
                    rapportiLavoro.modIsc.cell = dataSet2.Tables[0].Rows[0]["CELL"].ToString().Trim();
                    rapportiLavoro.modIsc.fax = dataSet2.Tables[0].Rows[0]["FAX"].ToString().Trim();
                    rapportiLavoro.modIsc.email = dataSet2.Tables[0].Rows[0]["EMAIL"].ToString().Trim();
                    rapportiLavoro.modIsc.pec = dataSet2.Tables[0].Rows[0]["EMAILCERT"].ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return rapportiLavoro;
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
        private DataRowCollection GetSospensioniIntersecanti(string prosos, string prorap, string matricola, string dataDa, string dataAl, string codpos)
        {
            iDB2Parameter prorapParam = objDataAccess.CreateParameter("@prorap", iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input, prorap);
            iDB2Parameter matricolaParam = objDataAccess.CreateParameter("@matricola", iDB2DbType.iDB2Decimal, 11, ParameterDirection.Input, matricola);
            iDB2Parameter dataDaParam = objDataAccess.CreateParameter("@dataDa", iDB2DbType.iDB2Date, 10, ParameterDirection.Input, dataDa);
            iDB2Parameter dataAlParam = objDataAccess.CreateParameter("@dataAl", iDB2DbType.iDB2Date, 10, ParameterDirection.Input, dataAl);
            iDB2Parameter codposParam = objDataAccess.CreateParameter("@codpos", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, codpos);
            iDB2Parameter prososParam = objDataAccess.CreateParameter("@prosos", iDB2DbType.iDB2Decimal, 7, ParameterDirection.Input, prosos);
            DataTable sospensioniInRange;

            string querySospensioneInRange = "SELECT CODSOS, DATINISOS, DATFINSOS FROM SOSRAP WHERE MAT = @matricola AND CODPOS = @codpos AND PRORAP = @prorap ";
            if (!string.IsNullOrWhiteSpace(prosos))
            {
                querySospensioneInRange += "AND PROSOS <> @prosos AND STASOS  = '0' AND ((DATINISOS >= @dataDa AND DATFINSOS <= @dataAl) OR (DATFINSOS >= @dataDa AND DATFINSOS <= @dataAl) OR (DATINISOS >= @dataDa AND DATINISOS <= @dataAl) OR (DATINISOS <= @dataDa AND DATFINSOS >= @dataAl))";
                sospensioniInRange = objDataAccess.GetDataTableWithParameters(querySospensioneInRange, matricolaParam, codposParam, prorapParam, prososParam, dataDaParam, dataAlParam, dataDaParam, dataAlParam, dataDaParam, dataAlParam, dataDaParam, dataAlParam);
            }
            else
            {
                querySospensioneInRange += " AND STASOS  = '0' AND ((DATINISOS >= @dataDa AND DATFINSOS <= @dataAl) OR (DATFINSOS >= @dataDa AND DATFINSOS <= @dataAl) OR (DATINISOS >= @dataDa AND DATINISOS <= @dataAl) OR (DATINISOS <= @dataDa AND DATFINSOS >= @dataAl))";
                sospensioniInRange = objDataAccess.GetDataTableWithParameters(querySospensioneInRange, matricolaParam, codposParam, prorapParam, dataDaParam, dataAlParam, dataDaParam, dataAlParam, dataDaParam, dataAlParam, dataDaParam, dataAlParam);
            }
            return sospensioniInRange.Rows;
        }

        private bool CheckTipoEDurataSospensione(string codsos, DateTime dataDa, DateTime dataAl, ref string errorMsg)
        {
            if (dataDa > dataAl)
            {
                errorMsg = "E' stata inserita una data di inizio maggiore della data di fine.";
                return false;
            }

            var differenceDate = new DateDiff(dataDa, dataAl);

            iDB2Parameter codsosParam = objDataAccess.CreateParameter("@codsos", iDB2DbType.iDB2Decimal, 7, ParameterDirection.Input, codsos);
            string queryGiorniMesiMax = "SELECT MAXGG, MAXMES FROM CODSOS WHERE CODSOS = @codsos";
            DataTable mesiGgMax = objDataAccess.GetDataTableWithParameters(queryGiorniMesiMax, codsosParam);

            if (string.IsNullOrEmpty(mesiGgMax.Rows[0]["MAXGG"].ToString()) && string.IsNullOrEmpty(mesiGgMax.Rows[0]["MAXMES"].ToString()))
            {
                return true;
            }

            if (!string.IsNullOrEmpty(mesiGgMax.Rows[0]["MAXGG"].ToString()))
            {
                bool result = differenceDate.Days < int.Parse(mesiGgMax.Rows[0]["MAXGG"].ToString());
                if (!result)
                    errorMsg = "E' stato inserito un intervallo di date troppo ampio rispetto a questo tipo di sospensione.";
                return result;
            }
            if (!string.IsNullOrEmpty(mesiGgMax.Rows[0]["MAXMES"].ToString()))
            {
                bool result = differenceDate.Months <= int.Parse(mesiGgMax.Rows[0]["MAXMES"].ToString());
                if (differenceDate.Months == int.Parse(mesiGgMax.Rows[0]["MAXMES"].ToString()))
                {
                    result = dataDa.Day > dataAl.Day;
                }
                if (!result)
                    errorMsg = "E' stato inserito un intervallo di date troppo ampio rispetto a questo tipo di sospensione.";
                return result;
            }
            return false;
        }

        private TimePeriodCollection CheckIntersezioneSospensione(string prosos, string codsos, string prorap, string matricola, string dataDa, string dataAl, string codpos, ref string messaggio)
        {
            DataRowCollection sospensioniInRange = GetSospensioniIntersecanti(prosos, prorap, matricola, dataDa, dataAl, codpos);

            TimePeriodCollection timePeriods = new TimePeriodCollection();

            if (sospensioniInRange.Count == 0)
                return timePeriods;

            foreach (DataRow row in sospensioniInRange)
            {
                if (row["CODSOS"].ToString() != codsos)
                {
                    messaggio = "Sono presenti sospensioni di tipo diverso nel periodo indicato.";
                    return null;
                }
                timePeriods.Add(new TimeRange()
                {
                    Start = DateTime.Parse(row["DATINISOS"].ToString()),
                    End = DateTime.Parse(row["DATFINSOS"].ToString())
                });
            }
            return timePeriods;
        }

        public bool CheckStessaSospensione(string prosos, string prorap, string matricola, string dataDa, string dataAl, string codpos, string codsos)
        {
            var sospensioniInRange = GetSospensioniIntersecanti(prosos, prorap, matricola, dataDa, dataAl, codpos);
            foreach (DataRow row in sospensioniInRange)
            {
                if (row["CODSOS"].ToString() == codsos)
                {
                    return true;
                }
            }
            return false;
        }

        private string ChooseOrder(OrderByType orderBy)
        {
            switch (orderBy)
            {
                case OrderByType.Matricola: return "A.MAT";
                case OrderByType.Cognome: return "A.COG";
                case OrderByType.Nome: return "A.NOM";
                case OrderByType.CodiceFiscale: return "A.CODFIS";
                case OrderByType.DataIscrizione: return "DATISCR";
                case OrderByType.DataCessazione: return "DATCES";
                default: return "A.MAT, DATISCR";
            }
        }

        private RDLPaginatiModel GetRapportiDiLavoro(string query, iDB2Parameter filtroParam, iDB2Parameter codposParam, TipoRDL tipoRdl)
        {
            RDLPaginatiModel rapporti = new RDLPaginatiModel();
            DataTable resultRdl = objDataAccess.GetDataTableWithParameters(query, codposParam, filtroParam, filtroParam, filtroParam, filtroParam);

            foreach (DataRow row in resultRdl.Rows)
            {
                var rapporto = new RapportoDiLavoroLight()
                {
                    Matricola = row["MAT"].ToString(),
                    Cognome = row["COG"].ToString().Trim(),
                    Nome = row["NOM"].ToString().Trim(),
                    CodFis = row["CODFIS"].ToString(),
                    Iscrizione = row.DateElementAt("DATISCR", StandardUse.Readable),
                    Prorap = row["PRORAP"].ToString(),
                    DataCessazione = row.RawDateElementAt("DATCES", StandardUse.Readable),
                    TipoRDL = tipoRdl
                };
                rapporti.Rapporti.Add(rapporto);
            }
            return rapporti;
        }

        private DateTime? GetDataCessazioneRDL(string matricola, string codpos, string prorap)
        {
            string getRDLQuery = "SELECT DATCES FROM RAPLAV WHERE CODPOS = " + codpos + " AND MAT= " + matricola + " AND PRORAP= " + prorap;
            string dataCessRdl = objDataAccess.Get1ValueFromSQL(getRDLQuery, CommandType.Text);
            return string.IsNullOrWhiteSpace(dataCessRdl) ? null : DateTime.Parse(dataCessRdl);
        }

        private bool UpsertISCD(NuovoIscritto iscritto)
        {
            bool flag;
            string ctrlUpdate = $"SELECT * FROM ISCD WHERE MAT = {iscritto.matricola} AND DATINI = '{DateTime.Now.ToString("yyyy-MM-dd")}'";
            DataTable dtCtrlUpdate = this.objDataAccess.GetDataTable(ctrlUpdate);

            var nuovoIscrittoParameters = CleanValuesAndPartiallyMapNuovoIscrittoIntoParameters(iscritto);

            if (dtCtrlUpdate != null && dtCtrlUpdate.Rows.Count > 0)
            {
                string updateIscd = $"UPDATE ISCD SET CODDUG = '{iscritto.indirizzo}'," +
                $" IND = @Indirizzo," +
                $" NUMCIV = @Civico," +
                $" DENSTAEST = '{iscritto.statoEs}'," +
                $" DENLOC = '{iscritto.localita}'," +
                $" CAP = '{iscritto.cap}'," +
                $" SIGPRO = '{(string.IsNullOrWhiteSpace(iscritto.statoEs) ? iscritto.provincia : "EE")}'," +
                $" AGGMAN = 'S'," +
                $" TEL1 = @Tel," +
                $" EMAIL = @Email," +
                $" ULTAGG = CURRENT TIMESTAMP," +
                $" UTEAGG = '{iscritto.codFis}'," +
                $" CODCOM = '{iscritto.comuneCod}'," +
                $" DENCOM = '{iscritto.comune}'," +
                $" CO = @Co," +
                $" EMAILCERT = @Pec," +
                $" CELL = @Cell WHERE MAT = {iscritto.matricola} AND DATINI = '{DateTime.Now.ToString("yyyy-MM-dd")}'";
                flag = objDataAccess.WriteTransactionDataWithParametersAndDontCall(updateIscd, CommandType.Text,
                nuovoIscrittoParameters["@Indirizzo"], nuovoIscrittoParameters["@Civico"], nuovoIscrittoParameters["@Tel"],
                nuovoIscrittoParameters["@Email"], nuovoIscrittoParameters["@Co"], nuovoIscrittoParameters["@Pec"], nuovoIscrittoParameters["@Cell"]);
            }
            else
            {
                string insertIscd = $"INSERT INTO ISCD (MAT, DATINI, CODDUG, IND, NUMCIV, DENSTAEST, DENLOC, CAP, SIGPRO, AGGMAN, TEL1, EMAIL, ULTAGG, UTEAGG, CODCOM, DENCOM, CO, EMAILCERT, CELL) " +
                $"VALUES ({iscritto.matricola}," +
                $" current date," +
                $" '{iscritto.indirizzo}'," +
                $" @Indirizzo," +
                $" @Civico," +
                $" '{iscritto.statoEs}'," +
                $" '{iscritto.localita}'," +
                $" '{iscritto.cap}'," +
                $" '{(string.IsNullOrWhiteSpace(iscritto.statoEs) ? iscritto.provincia : "EE")}'," +
                $" 'S'," +
                $" @Tel," +
                $" @Email," +
                $" current timestamp," +
                $" '{iscritto.codFis}'," +
                $" '{iscritto.comuneCod}'," +
                $" '{iscritto.comune}'," +
                $" @Co," +
                $" @Pec," +
                $" @Cell)";
                flag = objDataAccess.WriteTransactionDataWithParametersAndDontCall(insertIscd, CommandType.Text,
                nuovoIscrittoParameters["@Indirizzo"], nuovoIscrittoParameters["@Civico"], nuovoIscrittoParameters["@Tel"],
                nuovoIscrittoParameters["@Email"], nuovoIscrittoParameters["@Co"], nuovoIscrittoParameters["@Pec"], nuovoIscrittoParameters["@Cell"]);
            }

            if (flag)
            {
                string updateIsctTitStud = $@"UPDATE ISCT SET TITSTU = '{iscritto.titoloStudio}', ULTAGG = CURRENT TIMESTAMP WHERE MAT = {iscritto.matricola}";
                flag = this.objDataAccess.WriteTransactionData(updateIsctTitStud, CommandType.Text);
            }

            return flag;

            Dictionary<string, iDB2Parameter> CleanValuesAndPartiallyMapNuovoIscrittoIntoParameters(NuovoIscritto nuovoIscritto)
            {
                CleanNuovoIscritto();
                return PartiallyMapNuovoIscrittoIntoParameters();

                void CleanNuovoIscritto()
                {
                    nuovoIscritto.codFis = nuovoIscritto.codFis?.Trim();
                    nuovoIscritto.civico = nuovoIscritto.civico?.Replace("'", "");
                    nuovoIscritto.email = nuovoIscritto.email?.Replace("'", "");
                    nuovoIscritto.pec = nuovoIscritto.pec?.Replace("'", "");
                }

                Dictionary<string, iDB2Parameter> PartiallyMapNuovoIscrittoIntoParameters()
                {
                    var nuovoIscrittoParameters = new Dictionary<string, iDB2Parameter>();

                    var civicoParameter = objDataAccess.CreateParameter("@Civico", iDB2DbType.iDB2VarChar, 20, ParameterDirection.Input, nuovoIscritto.civico);
                    var indirizzoParameter = objDataAccess.CreateParameter("@Indirizzo", iDB2DbType.iDB2VarChar, 80, ParameterDirection.Input, nuovoIscritto.residenza);
                    var coParameter = objDataAccess.CreateParameter("@Co", iDB2DbType.iDB2VarChar, 40, ParameterDirection.Input, nuovoIscritto.co);
                    var telefonoParameter = objDataAccess.CreateParameter("@Tel", iDB2DbType.iDB2VarChar, 13, ParameterDirection.Input, nuovoIscritto.tel);
                    var cellulareParameter = objDataAccess.CreateParameter("@Cell", iDB2DbType.iDB2VarChar, 20, ParameterDirection.Input, nuovoIscritto.cell);
                    var emailParameter = objDataAccess.CreateParameter("@Email", iDB2DbType.iDB2VarChar, 100, ParameterDirection.Input, nuovoIscritto.email);
                    var pecParameter = objDataAccess.CreateParameter("@Pec", iDB2DbType.iDB2VarChar, 100, ParameterDirection.Input, nuovoIscritto.pec);

                    nuovoIscrittoParameters.Add(civicoParameter.ParameterName, civicoParameter);
                    nuovoIscrittoParameters.Add(indirizzoParameter.ParameterName, indirizzoParameter);
                    nuovoIscrittoParameters.Add(coParameter.ParameterName, coParameter);
                    nuovoIscrittoParameters.Add(telefonoParameter.ParameterName, telefonoParameter);
                    nuovoIscrittoParameters.Add(cellulareParameter.ParameterName, cellulareParameter);
                    nuovoIscrittoParameters.Add(emailParameter.ParameterName, emailParameter);
                    nuovoIscrittoParameters.Add(pecParameter.ParameterName, pecParameter);

                    return nuovoIscrittoParameters;
                }
            }

        }

        public string GetEmailAzienda(string CodPos)
        {
            DataLayer dt = new DataLayer();
            string emailAzienda = dt.Get1ValueFromSQL("select email from azemail where codpos='" + CodPos + "' order by DATINI desc limit 1", CommandType.Text);
            return emailAzienda;
        }

        private bool IsCambioRapportoLavoroConsentito(string tipoRapportoCorrente,string tipoRapportoSelezionato)
        {
            if (tipoRapportoCorrente == tipoRapportoSelezionato)
                return true;

            var tipRapParameter = objDataAccess.CreateParameter(DbParameters.TipoRapporto, iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input, tipoRapportoCorrente);
            var tipRapVarParameter = objDataAccess.CreateParameter(DbParameters.TipoRapportoVariazione, iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input, tipoRapportoSelezionato);

            var isCambioConsentitoQuery = $"SELECT * FROM TIPRAPVAR WHERE TIPRAP = {DbParameters.TipoRapporto} AND TIPRAPVAR = {DbParameters.TipoRapportoVariazione}";
            var isCambioConsentitoResultRows = objDataAccess.GetDataTableWithParameters(isCambioConsentitoQuery, tipRapParameter, tipRapVarParameter).Rows.OfType<DataRow>();

            if (!isCambioConsentitoResultRows.Any())
                return true;

            var consentitoColumn = isCambioConsentitoResultRows.First().ElementAt("CONSENTITO");

            return consentitoColumn == "S";
        }
    }
}
