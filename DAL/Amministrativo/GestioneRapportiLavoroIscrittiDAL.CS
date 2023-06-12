// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Amministrativo.GestioneRapportiLavoroIscrittiDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.Amministrativo;

namespace TFI.DAL.Amministrativo
{
    public class GestioneRapportiLavoroIscrittiDAL
    {
        private DataLayer objDataAccess = new DataLayer();
        private Utile Utils = new Utile();
        private ModGetDati ModGetDati = new ModGetDati();

        public GestioneRapportiLavoroIscrittiOCM RicercaIscrittiDAL(
          GestioneRapportiLavoroIscrittiOCM.Iscritti isc,
          ref string ErroreMSG,
          ref string SuccessMSG)
        {
            string str1 = "";
            string str2 = "";
            DataTable dataTable1 = new DataTable();
            string str3 = "";
            int CODPOS = 0;
            DataTable dataTable2 = new DataTable();
            List<GestioneRapportiLavoroIscrittiOCM.Iscritti> iscrittiList = new List<GestioneRapportiLavoroIscrittiOCM.Iscritti>();
            GestioneRapportiLavoroIscrittiOCM lavoroIscrittiOcm = new GestioneRapportiLavoroIscrittiOCM();
            try
            {
                this.Module_Autorizzazioni_Speciali_Solo_Enpaia(ref CODPOS, "CONTRIBUTI", ref ErroreMSG);
                if (isc.statocheck == 0)
                    str1 = str1 + " SELECT AZ.CODPOS,AZ.RAGSOC, A.MAT, A.COG, A.NOM, A.CODFIS,A.DATNAS, TRANSLATE(CHAR(B.DATDEC, EUR), '/', '.') AS DATDEC," + "B.DATDEC AS DATISCR, B.PRORAP, TRANSLATE(CHAR(B.DATCES, EUR), '/', '.') AS DATCES, B.DATPRE ," + "(SELECT C.DENCES FROM CAUCES C WHERE C.CODCAUCES = B.CODCAUCES) AS DENCES " + "FROM ISCT A INNER JOIN RAPLAV B ON A.MAT = B.MAT INNER JOIN AZI AZ ON B.CODPOS = AZ.CODPOS WHERE A.MAT = B.MAT ";
                if (isc.statocheck == 1)
                    str1 = str1 + " SELECT AZ.CODPOS,AZ.RAGSOC, A.MAT, A.COG, A.NOM, A.CODFIS,A.DATNAS, TRANSLATE(CHAR(B.DATDEC, EUR), '/', '.') AS DATDEC," + "B.DATDEC AS DATISCR, B.PRORAP, TRANSLATE(CHAR(B.DATCES, EUR), '/', '.') AS DATCES, B.DATPRE ," + "(SELECT C.DENCES FROM CAUCES C WHERE C.CODCAUCES = B.CODCAUCES) AS DENCES " + "FROM ISCT A INNER JOIN RAPLAV B ON A.MAT = B.MAT INNER JOIN AZI AZ ON B.CODPOS = AZ.CODPOS WHERE " + " B.DATCES IS  NULL ";
                if (isc.statocheck == 2)
                    str1 = str1 + " SELECT AZ.CODPOS,AZ.RAGSOC  , A.MAT, A.COG, A.NOM, A.CODFIS,A.DATNAS, TRANSLATE(CHAR(B.DATDEC, EUR), '/', '.') AS DATDEC," + "B.DATDEC AS DATISCR, B.PRORAP, TRANSLATE(CHAR(B.DATCES, EUR), '/', '.') AS DATCES, B.DATPRE ," + "(SELECT C.DENCES FROM CAUCES C WHERE C.CODCAUCES = B.CODCAUCES) AS DENCES " + "FROM ISCT A INNER JOIN RAPLAV B ON A.MAT = B.MAT INNER JOIN AZI AZ ON B.CODPOS = AZ.CODPOS WHERE " + " B.DATCES IS NOT NULL ";
                if (CODPOS != 0)
                {
                    str2 = str2 + " AND A.MAT IN (SELECT MAT FROM RAPLAV WHERE CODPOS = " + CODPOS.ToString() + ") ";
                }
                else
                {
                    DataTable dataTable3 = this.Utils.CREA_DT_AUTORIZZAZIONI_ENPAIA(ref ErroreMSG);
                    for (int index = 0; index <= dataTable3.Rows.Count - 1; ++index)
                    {
                        if (dataTable3.Rows[index]["ABILITATO"].ToString() == "NO")
                            str3 = str3 + "," + dataTable3.Rows[index]["POSIZIONE"]?.ToString();
                    }
                    if (str3.ToString().Trim() != "")
                    {
                        string str4 = str3.Substring(1);
                        str2 = str2 + " AND A.MAT NOT IN (SELECT MAT FROM RAPLAV WHERE CODPOS = " + str4 + ") ";
                    }
                }
                if (!string.IsNullOrEmpty(isc.mat))
                    str2 = str2 + " And A.MAT = " + isc.mat;
                if (!string.IsNullOrEmpty(isc.cognome))
                    str2 = str2 + " AND A.COG LIKE '" + isc.cognome.Trim().ToUpper() + "%'";
                if (!string.IsNullOrEmpty(isc.nome))
                    str2 = str2 + " AND A.NOM LIKE '" + isc.nome.Trim().ToUpper() + "%'";
                if (!string.IsNullOrEmpty(isc.codfis))
                    str2 = str2 + " AND A.CODFIS LIKE '%" + isc.codfis.Trim().ToUpper() + "%'";
                if (!string.IsNullOrEmpty(isc.datnas))
                    str2 = str2 + " AND A.DATNAS = '" + DBMethods.Db2Date(isc.datnas) + "'";
                if (!string.IsNullOrEmpty(isc.codposAz))
                    str2 = str2 + " AND AZ.CODPOS = " + DBMethods.DoublePeakForSql(isc.codposAz);
                if (!string.IsNullOrEmpty(isc.ragsocAz))
                    str2 = str2 + " AND AZ.RAGSOC  LIKE '% " + isc.ragsocAz + "%'";
                if (!string.IsNullOrEmpty(isc.parivaAz))
                    str2 = str2 + " AND (AZ.PARIVA LIKE '%" + isc.parivaAz + "%') OR (AZ.CODFIS LIKE '%" + isc.parivaAz + "%')";
                DataTable dataTable4 = this.objDataAccess.GetDataTable(str1 + str2 + " ORDER BY A.MAT");
                if (dataTable4.Rows.Count == 0)
                    ErroreMSG = "Nessun risultato ottenuto";
                if (dataTable4.Rows.Count > 0)
                    dataTable2 = this.objDataAccess.GetDataTable("SELECT STAPIN FROM UTEPIN WHERE CODUTE = " + DBMethods.DoublePeakForSql(dataTable4.Rows[0]["CODFIS"].ToString()) + " ORDER BY DATFIN DESC");
                HttpContext.Current.Session["DTIscritti"] = (object)dataTable4;
                if (dataTable4.Rows.Count >= 1)
                {
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable4.Rows)
                    {
                        GestioneRapportiLavoroIscrittiOCM.Iscritti iscritti = new GestioneRapportiLavoroIscrittiOCM.Iscritti()
                        {
                            mat = row["MAT"].ToString(),
                            cognome = row["COG"].ToString(),
                            nome = row["NOM"].ToString(),
                            datnas = row["DATNAS"].ToString().Substring(0, 10),
                            codfis = row["CODFIS"].ToString(),
                            datini = row["DATISCR"].ToString().Substring(0, 10),
                            datfin = row["DATCES"].ToString(),
                            motivo = row["DENCES"].ToString(),
                            ragsocAz = row["RAGSOC"].ToString(),
                            codposAz = row["CODPOS"].ToString()
                        };
                        iscrittiList.Add(iscritti);
                    }
                }
                lavoroIscrittiOcm.listiscritti = iscrittiList;
            }
            catch (Exception ex)
            {
                ErroreMSG = "Nessun risultato ottenuto";
                return (GestioneRapportiLavoroIscrittiOCM)null;
            }
            return lavoroIscrittiOcm;
        }

        public bool Module_Autorizzazioni_Speciali_Solo_Enpaia(
          ref int CODPOS,
          string SISTEMA,
          ref string messaggio)
        {
            DataTable dataTable = this.Utils.CREA_DT_SOLO_AUTORIZZAZIONI_ENPAIA(ref messaggio);
            if (!(SISTEMA == "CONTRIBUTI"))
            {
                if (SISTEMA == "AGRIFONDO")
                {
                    int index = 0;
                    if (index <= dataTable.Rows.Count - 1)
                    {
                        if (dataTable.Rows[index]["ABILITATO_AGF"].ToString() == "SI")
                        {
                            CODPOS = Convert.ToInt32(dataTable.Rows[index]["POSIZIONE_AGF"]);
                            return true;
                        }
                        CODPOS = 0;
                        return false;
                    }
                }
            }
            else
            {
                int index = 0;
                if (index <= dataTable.Rows.Count - 1)
                {
                    if (dataTable.Rows[index]["ABILITATO"].ToString() == "SI")
                    {
                        CODPOS = Convert.ToInt32(dataTable.Rows[index]["POSIZIONE"]);
                        return true;
                    }
                    CODPOS = 0;
                    return false;
                }
            }
            return false;
        }

        public GestioneRapportiLavoroIscrittiOCM CaricaInserimentoDAL(
          GestioneRapportiLavoroIscrittiOCM rdl,
          string codpos,
          ref string ErroreMSG,
          TFI.OCM.Utente.Utente u)
        {
            string str1 = codpos;
            DataTable dataTable1 = new DataTable();
            string str2 = " ";
            string str3 = "";
            DataTable dataTable2 = new DataTable();
            ModGetDati modGetDati = new ModGetDati();
            try
            {
                str2 = "CURRENT_TIMESTAMP";
                DataTable dataTable3 = this.objDataAccess.GetDataTable("SELECT RAGSOC, NATGIU FROM AZI WHERE CODPOS = " + str1);
                GestioneRapportiLavoroIscrittiOCM.AziendaUtilizzatrice aziendaUtilizzatrice = new GestioneRapportiLavoroIscrittiOCM.AziendaUtilizzatrice();
                GestioneRapportiLavoroIscrittiOCM.DatiContrattuali datiContrattuali = new GestioneRapportiLavoroIscrittiOCM.DatiContrattuali();
                aziendaUtilizzatrice.ragsocAz = dataTable3.Rows[0]["RAGSOC"].ToString().Trim().ToUpper();
                if (Convert.ToInt32(dataTable3.Rows[0]["NATGIU"]) == 10)
                {
                    str3 = "S";
                    datiContrattuali.CbAssCon = "N";
                }
                else
                {
                    str3 = "N";
                    datiContrattuali.CbAssCon = "S";
                }
                rdl.aziendaUtilizzatrice = aziendaUtilizzatrice;
                rdl.datiContrattuali = datiContrattuali;
                dataTable3.Clear();
                modGetDati.Module_Carica_DUG(this.objDataAccess, rdl, ConPrimaVoceVuota: true);
                modGetDati.Module_Carica_TITSTU(this.objDataAccess, rdl);
                modGetDati.Module_Carica_TIPRAP(this.objDataAccess, rdl, "WHERE TIPRAP<>5");
                string strSQL = "SELECT ABB, ABBDEFMAT, TIPISC, (SELECT DENTIPISC FROM TIPISC WHERE CODTIPISC = TIPISC) AS DENTIPISC" + " FROM AZISTO WHERE CODPOS = " + codpos + " AND DATINI <= CURRENT_DATE ORDER BY DATCOM DESC";
                dataTable3.Clear();
                dataTable1 = this.objDataAccess.GetDataTable(strSQL);
                DataTable dataTable4 = this.objDataAccess.GetDataTable("SELECT RAGSOC FROM AZI WHERE CODPOS='" + str1 + "' ");
                rdl.aziendaUtilizzatrice.ragsocAz = dataTable4.Rows[0]["RAGSOC"].ToString();
                rdl.aziendaUtilizzatrice.codposAz = str1;
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nel caricamento dati");
            }
            return rdl;
        }

        private void CaricaDatiRdl(
          string MsgErrore,
          GestioneRapportiLavoroIscrittiOCM rdl,
          TFI.OCM.Utente.Utente u,
          string codpos,
          string mat,
          string strDataNascita)
        {
            DataTable dataTable1 = new DataTable();
            DataTable dataTable2 = this.objDataAccess.GetDataTable("SELECT PRORAP FROM RAPLAV WHERE MAT=" + mat);
            string str1 = dataTable2.Rows[0]["PRORAP"].ToString();
            rdl.iscritti.PRORAP = str1;
            dataTable2.Clear();
            rdl.iscritti.PRORAP = str1;
            rdl.iscritti.mat = mat;
            int num = 0;
            ModGetDati modGetDati = new ModGetDati();
            try
            {
                string strSQL = "SELECT TIPISC, (SELECT DENTIPISC FROM TIPISC WHERE CODTIPISC = TIPISC) AS DENTIPISC" + " FROM AZISTO WHERE CODPOS = " + codpos + " AND DATINI <= CURRENT_DATE ORDER BY DATCOM DESC";
                dataTable2.Clear();
                DataTable dataTable3 = this.objDataAccess.GetDataTable(strSQL);
                if (dataTable3.Rows.Count > 0 && dataTable3.Rows[0]["TIPISC"].ToString().Trim() == "F")
                    MsgErrore = "Aliquota Contributiva - Tipo Iscrizione Azienda: " + dataTable3.Rows[0]["DENTIPISC"].ToString().Trim();
                DataTable datiCompletiIsct = modGetDati.Module_GetDatiCompleti_ISCT(this.objDataAccess, Convert.ToInt32(mat));
                strDataNascita = datiCompletiIsct.Rows[0]["DATNAS"].ToString();
                rdl.iscritti.nominativo = datiCompletiIsct.Rows[0]["COG"].ToString().Trim() + " " + datiCompletiIsct.Rows[0]["NOM"].ToString().Trim();
                datiCompletiIsct.Clear();
                DataTable datiCompletiRaplav = modGetDati.Module_GetDatiCompleti_RAPLAV(this.objDataAccess, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(str1));
                if (!string.IsNullOrEmpty(datiCompletiRaplav.Rows[0]["DATDEC"].ToString()))
                    rdl.datiContrattuali.datisc = DBMethods.Db2Date(datiCompletiRaplav.Rows[0]["DATDEC"].ToString());
                datiCompletiRaplav.Clear();
                DataTable dataTable4 = !(rdl.datiContrattuali.DataVariazione != "") ? modGetDati.Module_GetDatiCompleti_STORDL(this.objDataAccess, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(str1), MsgErrore, " ") : modGetDati.Module_GetDatiCompleti_STORDL(this.objDataAccess, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(str1), "", rdl.datiContrattuali.DataVariazione);
                num = Convert.ToInt32(dataTable4.Rows[0]["ALIQUOTA"]);
                rdl.datiContrattuali.DataUltimaVar = DBMethods.Db2Date(dataTable4.Rows[0]["DATINI"].ToString());
                rdl.datiContrattuali.tiprapCod = dataTable4.Rows[0]["TIPRAP"].ToString();
                rdl.datiContrattuali.tiprap = dataTable4.Rows[0]["DENTIPRAP"].ToString();
                rdl.datiContrattuali.CODLIV = dataTable4.Rows[0]["CODLIV"].ToString();
                rdl.datiContrattuali.CODQUA = dataTable4.Rows[0]["CODQUACON"].ToString();
                rdl.datiContrattuali.qualifica = dataTable4.Rows[0]["DENQUA"].ToString();
                rdl.datiRetributivi.scattiAnz = dataTable4.Rows[0]["NUMSCA"].ToString();
                rdl.datiRetributivi.datUltSc = dataTable4.Rows[0]["DATSCA"].ToString();
                rdl.datiContrattuali.contratto = dataTable4.Rows[0]["DENCON"].ToString();
                rdl.datiContrattuali.CODCON = dataTable4.Rows[0]["CODCON"].ToString();
                rdl.datiContrattuali.CODLOC = dataTable4.Rows[0]["CODLOC"].ToString();
                rdl.datiContrattuali.PROCON = dataTable4.Rows[0]["PROCON"].ToString();
                rdl.datiContrattuali.PROLOC = dataTable4.Rows[0]["PROLOC"].ToString();
                rdl.datiContrattuali.livello = dataTable4.Rows[0]["DENLIV"].ToString();
                rdl.datiContrattuali.DataVariazione = dataTable4.Rows[0]["DATINI"].ToString();
                string str2 = !(rdl.datiContrattuali.DataVariazione != "") ? DBMethods.Db2Date(dataTable4.Rows[0]["DATINI"].ToString()) : rdl.datiContrattuali.DataVariazione;
                if (!string.IsNullOrEmpty(str2))
                    this.Module_Carica_Contratti(rdl, str2, "MODIFICA");
                rdl.datiContrattuali.qualifica = dataTable4.Rows[0]["DENQUA"].ToString();
                rdl.datiContrattuali.mensilita = dataTable4.Rows[0]["NUMMEN"].ToString();
                rdl.datiContrattuali.mens14 = dataTable4.Rows[0]["MESMEN14"].ToString();
                if (rdl.datiContrattuali.mens14 == "0")
                    rdl.datiContrattuali.mens14 = "0";
                rdl.datiContrattuali.mens15 = dataTable4.Rows[0]["MESMEN15"].ToString();
                if (rdl.datiContrattuali.mens15 == "0" || rdl.datiContrattuali.mens15 == null)
                    rdl.datiContrattuali.mens15 = "0";
                rdl.datiContrattuali.mens16 = dataTable4.Rows[0]["MESMEN16"].ToString();
                if (rdl.datiContrattuali.mens16 == "0" || rdl.datiContrattuali.mens15 == null)
                    rdl.datiContrattuali.mens16 = "0";
                rdl.datiContrattuali.AbbPre = "N";
                rdl.datiContrattuali.asscontr = dataTable4.Rows[0]["ASSCON"].ToString();
                string str3 = dataTable4.Rows[0]["FAP"].ToString();
                if (!(str3 == "N") && (str3 == null || str3.Length != 0))
                {
                    if (str3 == "S")
                        rdl.datiContrattuali.FAP = "S";
                }
                else
                    rdl.datiContrattuali.FAP = "N";
                rdl.datiRetributivi.emolumenti = Math.Round(Convert.ToDecimal(dataTable4.Rows[0]["MINCON"]), 2).ToString();
                rdl.datiRetributivi.RetDichiarata = dataTable4.Rows[0]["TRAECO"].ToString();
                rdl.datiContrattuali.TIPSPE = dataTable4.Rows[0]["TIPSPE"].ToString();
                if (dataTable4.Rows[0]["TIPSPE"].ToString() == "S")
                {
                    rdl.datiRetributivi.importoSc = dataTable4.Rows[0]["IMPSCA"].ToString();
                    rdl.datiRetributivi.NumScatt = dataTable4.Rows[0]["NUMSCA"].ToString();
                    if (!string.IsNullOrEmpty(dataTable4.Rows[0]["DATSCA"].ToString()))
                        rdl.datiRetributivi.datUltSc = DBMethods.Db2Date(dataTable4.Rows[0]["DATSCA"].ToString());
                    if (!string.IsNullOrEmpty(dataTable4.Rows[0]["DATPROSCA"].ToString()))
                        rdl.datiRetributivi.datProsSc = DBMethods.Db2Date(dataTable4.Rows[0]["DATPROSCA"].ToString());
                    if (dataTable4.Rows[0]["PERPAR"].ToString().Trim() != "" && dataTable4.Rows[0]["PERPAR"].ToString().Trim() != "0,00")
                        this.ImportoScatti(rdl);
                }
                switch (Convert.ToInt16(dataTable4.Rows[0]["NUMMEN"]))
                {
                    case 12:
                        rdl.datiRetributivi.S12 = dataTable4.Rows[0]["IMPAGG12"].ToString();
                        break;
                    case 13:
                        rdl.datiRetributivi.S12 = dataTable4.Rows[0]["IMPAGG12"].ToString();
                        rdl.datiRetributivi.S13 = dataTable4.Rows[0]["IMPAGG13"].ToString();
                        break;
                    case 14:
                        rdl.datiRetributivi.S12 = dataTable4.Rows[0]["IMPAGG12"].ToString();
                        rdl.datiRetributivi.S13 = dataTable4.Rows[0]["IMPAGG13"].ToString();
                        rdl.datiRetributivi.S14 = dataTable4.Rows[0]["IMPAGG14"].ToString();
                        break;
                    case 15:
                        rdl.datiRetributivi.S12 = dataTable4.Rows[0]["IMPAGG12"].ToString();
                        rdl.datiRetributivi.S13 = dataTable4.Rows[0]["IMPAGG13"].ToString();
                        rdl.datiRetributivi.S14 = dataTable4.Rows[0]["IMPAGG14"].ToString();
                        rdl.datiRetributivi.S15 = dataTable4.Rows[0]["IMPAGG15"].ToString();
                        break;
                    case 16:
                        rdl.datiRetributivi.S12 = dataTable4.Rows[0]["IMPAGG12"].ToString();
                        rdl.datiRetributivi.S13 = dataTable4.Rows[0]["IMPAGG13"].ToString();
                        rdl.datiRetributivi.S14 = dataTable4.Rows[0]["IMPAGG14"].ToString();
                        rdl.datiRetributivi.S15 = dataTable4.Rows[0]["IMPAGG15"].ToString();
                        rdl.datiRetributivi.S16 = dataTable4.Rows[0]["IMPAGG16"].ToString();
                        break;
                }
                if (rdl.datiRetributivi.S12 == "0" || string.IsNullOrEmpty(rdl.datiRetributivi.S12))
                    rdl.datiRetributivi.S12 = "0,00";
                if (rdl.datiRetributivi.S13 == "0" || string.IsNullOrEmpty(rdl.datiRetributivi.S13))
                    rdl.datiRetributivi.S13 = "0,00";
                if (rdl.datiRetributivi.S14 == "0" || string.IsNullOrEmpty(rdl.datiRetributivi.S14))
                    rdl.datiRetributivi.S14 = "0,00";
                if (rdl.datiRetributivi.S15 == "0" || string.IsNullOrEmpty(rdl.datiRetributivi.S15))
                    rdl.datiRetributivi.S15 = "0,00";
                if (rdl.datiRetributivi.S16 == "0" || string.IsNullOrEmpty(rdl.datiRetributivi.S16))
                    rdl.datiRetributivi.S16 = "0,00";
                rdl.datiRetributivi.totaleS = Math.Round(Convert.ToDecimal(dataTable4.Rows[0]["RETTOT"]), 2).ToString();
                rdl.datiContrattuali.CODGRUASS = dataTable4.Rows[0]["CODGRUASS"].ToString();
                rdl.datiContrattuali.aliqCont = dataTable4.Rows[0]["DENGRUASS"].ToString();
                modGetDati.Module_GetListaAliquoteContributive(this.objDataAccess, rdl, Convert.ToInt32(dataTable4.Rows[0]["CODQUACON"]), Convert.ToInt32(dataTable4.Rows[0]["CODGRUASS"]), str2, "", false, rdl.datiContrattuali.FAP);
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nel caricamento dei dati");
            }
        }

        public GestioneRapportiLavoroIscrittiOCM LoadVariazione(
          string MsgErrore,
          GestioneRapportiLavoroIscrittiOCM rdl,
          TFI.OCM.Utente.Utente u,
          string mat,
          string datNas)
        {
            DataTable dataTable1 = new DataTable();
            string str1 = " ";
            string str2 = "";
            DataTable dataTable2 = new DataTable();
            rdl.iscritti.datnas = datNas;
            ModGetDati modGetDati = new ModGetDati();
            try
            {
                string codpos = this.objDataAccess.GetDataTable("SELECT CODPOS FROM RAPLAV WHERE MAT=" + mat).Rows[0]["CODPOS"].ToString();
                rdl.iscritti.CODPOS = codpos;
                str1 = "CURRENT_TIMESTAMP";
                DataTable dataTable3 = this.objDataAccess.GetDataTable("SELECT RAGSOC, NATGIU FROM AZI WHERE CODPOS = " + codpos);
                GestioneRapportiLavoroIscrittiOCM.AziendaUtilizzatrice aziendaUtilizzatrice = new GestioneRapportiLavoroIscrittiOCM.AziendaUtilizzatrice();
                aziendaUtilizzatrice.ragsocAz = dataTable3.Rows[0]["RAGSOC"].ToString().Trim();
                str2 = Convert.ToInt32(dataTable3.Rows[0]["NATGIU"]) != 10 ? "N" : "S";
                rdl.aziendaUtilizzatrice = aziendaUtilizzatrice;
                dataTable3.Clear();
                modGetDati.Module_Carica_DUG(this.objDataAccess, rdl, ConPrimaVoceVuota: true);
                modGetDati.Module_Carica_TIPRAP(this.objDataAccess, rdl, "WHERE TIPRAP<>5");
                this.CaricaDatiRdl(MsgErrore, rdl, u, codpos, mat, datNas);
                this.Module_Carica_Contratti(rdl, rdl.datiContrattuali.datisc, "MODIFICA");
                return rdl;
            }
            catch (Exception ex)
            {
                return (GestioneRapportiLavoroIscrittiOCM)null;
            }
        }

        public void SalvaVariazioni(
          GestioneRapportiLavoroIscrittiOCM rdl,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMsg,
          ref string SuccessMSG)
        {
            if (DateTime.Compare(DateTime.Parse(rdl.datiContrattuali.DataUltimaVar), Convert.ToDateTime(rdl.datiContrattuali.DataNuovaVariazione)) == 0)
                this.ModificaRDL(rdl, u, ref ErroreMsg, ref SuccessMSG);
            else
                this.VariazioneRDL(rdl, u, ref ErroreMsg, ref SuccessMSG);
        }

        public void VariazioneRDL(
          GestioneRapportiLavoroIscrittiOCM rdl,
          TFI.OCM.Utente.Utente u,
          ref string MsgErrore,
          ref string MSGSuccess)
        {
            string codpos = rdl.iscritti.CODPOS;
            string mat = rdl.iscritti.mat;
            string prorap = rdl.iscritti.PRORAP;
            string str1 = "";
            DataView dataView = new DataView();
            Decimal num1 = 0M;
            string[] strArray1 = rdl.datiContrattuali.CODCON.Split(' ');
            rdl.datiContrattuali.CODCON = strArray1[0];
            rdl.datiContrattuali.CODLOC = strArray1[2];
            DataTable dataTable1 = new DataTable();
            Decimal decTraEco = 0M;
            string str2 = "";
            string[] strArray2 = new string[13];
            DataTable dataTable2 = new DataTable();
            DataTable dtAziende = new DataTable();
            string str3 = "";
            string str4 = "";
            string str5 = "";
            string str6 = "";
            DataTable dtNot = new DataTable();
            DataTable dtLOG = new DataTable();
            string TIPMOVSAN = "";
            DataTable dtDet = new DataTable();
            DataTable dataTable3 = new DataTable();
            ModGeneraNotifiche modGeneraNotifiche = new ModGeneraNotifiche();
            ModGeneraRettifiche generaRettifiche = new ModGeneraRettifiche();
            ModGetDati modGetDati = new ModGetDati();
            IDOC_RDL idocRdl = new IDOC_RDL();
            clsWRITE_DB clsWriteDb = new clsWRITE_DB();
            if (!modGetDati.Module_RDL_IsCessato(this.objDataAccess, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap)))
            {
                if (!this.Module_Autorizzato(u, 232, ref MsgErrore))
                    return;
            }
            else if (!this.Module_Autorizzato(u, 372, ref MsgErrore))
                return;
            try
            {
                this.objDataAccess.StartTransaction();
                string str7 = this.objDataAccess.Get1ValueFromSQL("SELECT ULTAGG FROM RAPLAV WHERE CODPOS = " + codpos + " AND PRORAP=" + prorap + " AND MAT = " + mat, CommandType.Text);
                if (!string.IsNullOrEmpty(str1) & (str7 ?? "") != (str1 ?? ""))
                {
                    this.objDataAccess.EndTransaction(false);
                    MsgErrore = "Attenzione... il record è stato modificato da un'altro utente";
                }
                else
                {
                    string str8 = DBMethods.Db2Date(Convert.ToDateTime(rdl.datiContrattuali.DataNuovaVariazione).ToString());
                    DataTable dataTable4 = this.objDataAccess.GetDataTable("SELECT DATINI FROM STORDL " + " WHERE CODPOS = '" + codpos + "' AND PRORAP= '" + prorap + "' AND MAT = '" + mat + "' " + " AND DATINI > '" + str8 + "' ORDER BY DATINI");
                    string strDatFin;
                    DateTime dateTime1;
                    if (dataTable4.Rows.Count > 0)
                    {
                        DateTime dateTime2 = Convert.ToDateTime(dataTable4.Rows[0]["DATINI"]);
                        dateTime2 = dateTime2.AddDays(-1.0);
                        strDatFin = DBMethods.Db2Date(dateTime2.ToString());
                    }
                    else
                    {
                        dateTime1 = Convert.ToDateTime("31-12-9999");
                        strDatFin = DBMethods.Db2Date(dateTime1.ToString());
                    }
                    string strDataScadenzaTermine = "";
                    string strPerApp = "";
                    str2 = "";
                    switch (Convert.ToInt32(rdl.datiContrattuali.tiprap))
                    {
                        case 2:
                        case 5:
                        case 9:
                        case 12:
                            dateTime1 = Convert.ToDateTime(rdl.datiContrattuali.datScadenzaTermine);
                            strDataScadenzaTermine = DBMethods.Db2Date(dateTime1.ToString());
                            break;
                        case 3:
                            str2 = rdl.datiContrattuali.PerPar.Replace(",", ".");
                            break;
                        case 4:
                        case 13:
                            dateTime1 = Convert.ToDateTime(rdl.datiContrattuali.datScadenzaTermine);
                            strDataScadenzaTermine = DBMethods.Db2Date(dateTime1.ToString());
                            str2 = rdl.datiContrattuali.PerPar.Replace(",", ".");
                            break;
                        case 6:
                        case 7:
                            dateTime1 = Convert.ToDateTime(rdl.datiContrattuali.datScadenzaTermine);
                            strDataScadenzaTermine = DBMethods.Db2Date(dateTime1.ToString());
                            strPerApp = rdl.datiContrattuali.PerApp.Replace(",", ".");
                            break;
                        case 8:
                        case 10:
                            dateTime1 = Convert.ToDateTime(rdl.datiContrattuali.datScadenzaTermine);
                            strDataScadenzaTermine = DBMethods.Db2Date(dateTime1.ToString());
                            strPerApp = rdl.datiContrattuali.PerApp.Replace(",", ".");
                            str2 = rdl.datiContrattuali.PerPar.Replace(",", ".");
                            break;
                        case 11:
                            str2 = rdl.datiContrattuali.PerPar.Replace(",", ".");
                            if (rdl.mesi.gennaio)
                                strArray2[0] = "1";
                            if (rdl.mesi.febbraio)
                                strArray2[1] = "2";
                            if (rdl.mesi.marzo)
                                strArray2[2] = "3";
                            if (rdl.mesi.aprile)
                                strArray2[3] = "4";
                            if (rdl.mesi.maggio)
                                strArray2[4] = "5";
                            if (rdl.mesi.giugno)
                                strArray2[5] = "6";
                            if (rdl.mesi.luglio)
                                strArray2[6] = "7";
                            if (rdl.mesi.agosto)
                                strArray2[7] = "8";
                            if (rdl.mesi.settembre)
                                strArray2[8] = "9";
                            if (rdl.mesi.ottobre)
                                strArray2[9] = "10";
                            if (rdl.mesi.novembre)
                                strArray2[10] = "11";
                            if (rdl.mesi.dicembre)
                            {
                                strArray2[11] = "12";
                                break;
                            }
                            break;
                        case 15:
                            if (!string.IsNullOrEmpty(rdl.datiContrattuali.datScadenzaTermine))
                            {
                                dateTime1 = Convert.ToDateTime(rdl.datiContrattuali.datScadenzaTermine);
                                strDataScadenzaTermine = DBMethods.Db2Date(dateTime1.ToString());
                                break;
                            }
                            break;
                        case 16:
                            dateTime1 = Convert.ToDateTime(rdl.datiContrattuali.DatScaTerPart);
                            strDataScadenzaTermine = DBMethods.Db2Date(dateTime1.ToString().Trim());
                            if (rdl.mesi.gennaio)
                                strArray2[0] = "1";
                            if (rdl.mesi.febbraio)
                                strArray2[1] = "2";
                            if (rdl.mesi.marzo)
                                strArray2[2] = "3";
                            if (rdl.mesi.aprile)
                                strArray2[3] = "4";
                            if (rdl.mesi.maggio)
                                strArray2[4] = "5";
                            if (rdl.mesi.giugno)
                                strArray2[5] = "6";
                            if (rdl.mesi.luglio)
                                strArray2[6] = "7";
                            if (rdl.mesi.agosto)
                                strArray2[7] = "8";
                            if (rdl.mesi.settembre)
                                strArray2[8] = "9";
                            if (rdl.mesi.ottobre)
                                strArray2[9] = "10";
                            if (rdl.mesi.novembre)
                                strArray2[10] = "11";
                            if (rdl.mesi.dicembre)
                            {
                                strArray2[11] = "12";
                                break;
                            }
                            break;
                        case 17:
                            if (rdl.mesi.gennaio)
                                strArray2[0] = "1";
                            if (rdl.mesi.febbraio)
                                strArray2[1] = "2";
                            if (rdl.mesi.marzo)
                                strArray2[2] = "3";
                            if (rdl.mesi.aprile)
                                strArray2[3] = "4";
                            if (rdl.mesi.maggio)
                                strArray2[4] = "5";
                            if (rdl.mesi.giugno)
                                strArray2[5] = "6";
                            if (rdl.mesi.luglio)
                                strArray2[6] = "7";
                            if (rdl.mesi.agosto)
                                strArray2[7] = "8";
                            if (rdl.mesi.settembre)
                                strArray2[8] = "9";
                            if (rdl.mesi.ottobre)
                                strArray2[9] = "10";
                            if (rdl.mesi.novembre)
                                strArray2[10] = "11";
                            if (rdl.mesi.dicembre)
                            {
                                strArray2[11] = "12";
                                break;
                            }
                            break;
                    }
                    string strDataUltScatto;
                    if (!string.IsNullOrEmpty(rdl.datiRetributivi.datUltSc))
                    {
                        dateTime1 = Convert.ToDateTime(rdl.datiRetributivi.datUltSc);
                        strDataUltScatto = DBMethods.Db2Date(dateTime1.ToString());
                    }
                    else
                        strDataUltScatto = "";
                    string strDataNuovoScatto;
                    if (!string.IsNullOrEmpty(rdl.datiRetributivi.datProsSc))
                    {
                        dateTime1 = Convert.ToDateTime(rdl.datiRetributivi.datProsSc);
                        strDataNuovoScatto = DBMethods.Db2Date(dateTime1.ToString());
                    }
                    else
                        strDataNuovoScatto = "";
                    if (rdl.datiContrattuali.PerPar == "0" || string.IsNullOrEmpty(rdl.datiContrattuali.PerPar))
                        rdl.datiContrattuali.PerPar = "0";
                    string asscontr = rdl.datiContrattuali.asscontr;
                    if (!(asscontr == "N") && (asscontr == null || asscontr.Length != 0) && asscontr != null)
                    {
                        if (asscontr == "S")
                            rdl.datiContrattuali.asscontr = "S";
                    }
                    else
                        rdl.datiContrattuali.asscontr = "N";
                    string fap = rdl.datiContrattuali.FAP;
                    if (!(fap == "N") && (fap == null || fap.Length != 0) && fap != null)
                    {
                        if (fap == "S" || fap == "on")
                            rdl.datiContrattuali.FAP = "S";
                    }
                    else
                        rdl.datiContrattuali.FAP = "N";
                    string tipspe = rdl.datiContrattuali.TIPSPE;
                    if (!(tipspe == "S"))
                    {
                        if (tipspe == "M" || tipspe == "A")
                            decTraEco = Convert.ToDecimal(rdl.datiRetributivi.totaleS) - Convert.ToDecimal(rdl.datiRetributivi.S12) - Convert.ToDecimal(rdl.datiRetributivi.S13) - Convert.ToDecimal(rdl.datiRetributivi.S14) - Convert.ToDecimal(rdl.datiRetributivi.S15) - Convert.ToDecimal(rdl.datiRetributivi.S16) - Convert.ToDecimal(rdl.datiRetributivi.importoSc);
                    }
                    else
                        decTraEco = 0M;
                    DataTable idocDatiE1Pitype1 = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0016", Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), 0, "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "MODIFICA RDL", "", "");
                    idocRdl.WRITE_IDOC_TESTATA(this.objDataAccess, idocDatiE1Pitype1.Rows[0]);
                    idocDatiE1Pitype1.Clear();
                    DataTable idocDatiE1Pitype2 = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0016", Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), 0, "", "", "9999-12-31", "", "", rdl.datiContrattuali.datisc, 0, 0, 0, "D", "MODIFICA RDL", "", "");
                    idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "0016", idocDatiE1Pitype2, true);
                    idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "9001", idocDatiE1Pitype2, true);
                    clsWriteDb.WRITE_INSERT_STORDL(this.objDataAccess, u, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), str8, strDatFin, decTraEco, strPerApp, Convert.ToInt32(rdl.datiContrattuali.tiprap), Convert.ToInt32(rdl.datiContrattuali.CODCON), Convert.ToInt32(rdl.datiContrattuali.CODLOC), Convert.ToInt32(rdl.datiContrattuali.CODLIV), Convert.ToInt32(rdl.datiContrattuali.mensilita), rdl.datiContrattuali.mens14, rdl.datiContrattuali.mens15, rdl.datiContrattuali.mens16, Convert.ToInt32(rdl.datiContrattuali.CODGRUASS), rdl.datiContrattuali.asscontr, rdl.datiContrattuali.AbbPre, rdl.datiContrattuali.PerPar, strDataScadenzaTermine, Convert.ToInt32(rdl.datiRetributivi.scattiAnz), Convert.ToDecimal(rdl.datiRetributivi.importoSc), strDataUltScatto, strDataNuovoScatto, rdl.datiContrattuali.FAP);
                    clsWriteDb.WRITE_INSERT_IMPAGG(this.objDataAccess, u, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), str8, "12", Convert.ToDecimal(rdl.datiRetributivi.S12));
                    clsWriteDb.WRITE_INSERT_IMPAGG(this.objDataAccess, u, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), str8, "13", Convert.ToDecimal(rdl.datiRetributivi.S13));
                    clsWriteDb.WRITE_INSERT_IMPAGG(this.objDataAccess, u, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), str8, "14", Convert.ToDecimal(rdl.datiRetributivi.S14));
                    clsWriteDb.WRITE_INSERT_IMPAGG(this.objDataAccess, u, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), str8, "15", Convert.ToDecimal(rdl.datiRetributivi.S15));
                    clsWriteDb.WRITE_INSERT_IMPAGG(this.objDataAccess, u, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), str8, "16", Convert.ToDecimal(rdl.datiRetributivi.S16));
                    int num2 = strArray2.Length - 1;
                    for (int index = 0; index <= num2; ++index)
                    {
                        if (!string.IsNullOrEmpty(strArray2[index]))
                            clsWriteDb.WRITE_INSERT_PARTIMM(this.objDataAccess, u, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), str8, Convert.ToInt32(strArray2[index]));
                    }
                    dataTable4.Clear();
                    string str9 = "SELECT DATINI FROM STORDL WHERE CODPOS='" + codpos + "' " + " AND MAT= '" + mat + "' " + " AND PRORAP = " + prorap;
                    dateTime1 = Convert.ToDateTime(rdl.datiContrattuali.DataNuovaVariazione);
                    string str10 = DBMethods.Db2Date(dateTime1.ToString());
                    DataTable dataTable5 = this.objDataAccess.GetDataTable(str9 + " AND DATINI < '" + str10 + "' " + " ORDER BY DATINI DESC FETCH FIRST 1 ROWS ONLY");
                    if (dataTable5.Rows.Count > 0)
                    {
                        dateTime1 = Convert.ToDateTime(rdl.datiContrattuali.DataNuovaVariazione);
                        dateTime1 = dateTime1.AddDays(-1.0);
                        string str11 = "UPDATE STORDL SET DATFIN = '" + DBMethods.Db2Date(dateTime1.ToString()) + "' " + " WHERE CODPOS = '" + codpos + "' " + " AND MAT= '" + mat + "' " + " AND PRORAP = '" + prorap + "' ";
                        dateTime1 = Convert.ToDateTime(dataTable5.Rows[0]["DATINI"]);
                        string str12 = DBMethods.Db2Date(dateTime1.ToString());
                        this.objDataAccess.WriteTransactionData(str11 + " AND DATINI = '" + str12 + "' ", CommandType.Text);
                    }
                    idocRdl.AGGIORNA_RAPLAV_INPS(this.objDataAccess, u, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap));
                    int num3;
                    (string, object) valueTuple;
                    if (Convert.ToDouble(num1) != Convert.ToDouble(rdl.datiContrattuali.aliqContval))
                    {
                        string[] strArray3 = new string[5]
                        {
              "SELECT CODPOS, ANNDEN, MESDEN, PRODEN FROM DENTES " + " WHERE CODPOS = " + codpos + " AND TIPMOV IN ('DP','NU') " + " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL " + " AND TRIM(CHAR(ANNDEN)) || ",
              " RIGHT('00' || TRIM(CHAR(MESDEN)), 2)  >= '",
              null,
              null,
              null
                        };
                        dateTime1 = Convert.ToDateTime(rdl.datiContrattuali.DataNuovaVariazione.Replace("'", ""));
                        num3 = dateTime1.Year;
                        strArray3[2] = num3.ToString().Trim();
                        dateTime1 = Convert.ToDateTime(rdl.datiContrattuali.DataNuovaVariazione.Replace("'", ""));
                        num3 = dateTime1.Month;
                        strArray3[3] = num3.ToString().Trim().PadLeft(2, '0');
                        strArray3[4] = "'";
                        string strSQL1 = string.Concat(strArray3) + " ORDER BY ANNDEN, MESDEN ";
                        dataTable2.Clear();
                        dataTable2 = this.objDataAccess.GetDataTable(strSQL1);
                        if (dataTable2.Rows.Count > 0)
                        {
                            string TIPPRI = this.objDataAccess.Get1ValueFromSQL("SELECT PRIORITA FROM TIPPRIRET WHERE CODTIPRET = 10", CommandType.Text);
                            dtAziende.Columns.Clear();
                            dtAziende.Columns.Add(new DataColumn("CODPOS", Type.GetType("System.Int32")));
                            dtAziende.Columns.Add(new DataColumn("ANNO", Type.GetType("System.Int32")));
                            dtAziende.Columns.Add(new DataColumn("MESE", Type.GetType("System.Int32")));
                            dtAziende.Columns.Add(new DataColumn("DATSCA", Type.GetType("System.String")));
                            dtAziende.Columns.Add(new DataColumn("TIPISC", Type.GetType("System.String")));
                            dtAziende.Columns.Add(new DataColumn("RIMUOVI", Type.GetType("System.String")));
                            int num4 = dataTable2.Rows.Count - 1;
                            for (int index1 = 0; index1 <= num4; ++index1)
                            {
                                if (Convert.ToBoolean((object)(dataTable2.Rows[index1]["ANNDEN"], 2003, false)))
                                {
                                    string TIPISC = Convert.ToString(this.objDataAccess.Get1ValueFromSQL("SELECT TIPISC FROM AZISTO WHERE CODPOS = " + dataTable2.Rows[index1]["CODPOS"]?.ToString() + " AND '" + dataTable2.Rows[index1]["ANNDEN"]?.ToString() + "-" + dataTable2.Rows[index1]["MESDEN"].ToString().Trim().PadLeft(2, '0') + "-01' BETWEEN DATINI AND DATFIN", CommandType.Text)).Trim();
                                    string DATA_ORA_SISTEMA = "CURRENT_TIMESTAMP";
                                    valueTuple = ("01/" + dataTable2.Rows[index1]["MESDEN"].ToString().Trim().PadLeft(2, '0') + "/", dataTable2.Rows[index1]["ANNDEN"]);
                                    string strDataDecorrenza = valueTuple.ToString();
                                    dateTime1 = Convert.ToDateTime(clsWriteDb.Module_GetValorePargen(this.objDataAccess, 3, strDataDecorrenza, Convert.ToInt32(codpos)));
                                    dateTime1 = dateTime1.AddDays(1.0);
                                    string DATINISAN = dateTime1.ToString();
                                    dateTime1 = Convert.ToDateTime(DATA_ORA_SISTEMA);
                                    string DATFINSAN = dateTime1.ToString();
                                    if (Convert.ToDateTime(DATINISAN) > Convert.ToDateTime(DATFINSAN))
                                        DATFINSAN = DATINISAN;
                                    dtAziende.Clear();
                                    DataRow row = dtAziende.NewRow();
                                    row["CODPOS"] = dataTable2.Rows[index1]["CODPOS"];
                                    row["ANNO"] = dataTable2.Rows[index1]["ANNDEN"];
                                    row["MESE"] = dataTable2.Rows[index1]["MESDEN"];
                                    row["TIPISC"] = (object)"";
                                    row["DATSCA"] = (object)"";
                                    row["RIMUOVI"] = (object)"";
                                    dtAziende.Rows.Add(row);
                                    dtNot.Clear();
                                    dtNot = modGeneraNotifiche.Module_Genera_Notifiche(this.objDataAccess, u, ref dtAziende, ref dtLOG, TIPMOVSAN, "", OPTIONAL_MATRICOLA: Convert.ToInt32(rdl.iscritti.mat), OPTIONAL_PRORAP: Convert.ToInt32(prorap));
                                    string strSQL2 = "SELECT DENTES.TIPMOV, DENDET.* FROM DENTES, DENDET " + " WHERE DENTES.CODPOS = DENDET.CODPOS " + " AND DENTES.ANNDEN = DENDET.ANNDEN " + " AND DENTES.MESDEN = DENDET.MESDEN " + " AND DENTES.PRODEN = DENDET.PRODEN " + " AND DENTES.CODPOS = " + dataTable2.Rows[index1]["CODPOS"].ToString() + " AND DENTES.ANNDEN = " + dataTable2.Rows[index1]["ANNDEN"].ToString() + " AND DENTES.MESDEN = " + dataTable2.Rows[index1]["MESDEN"].ToString() + " AND DENTES.PRODEN = " + dataTable2.Rows[index1]["PRODEN"].ToString() + " AND MAT = " + rdl.iscritti.mat + " AND PRORAP = " + prorap + " AND VALUE(DENDET.ESIRET, '') <> 'S' " + " AND DENDET.NUMMOV IS NOT NULL" + " AND DENDET.NUMMOVANN IS NULL" + " ORDER BY DAL";
                                    dtDet.Clear();
                                    dtDet = this.objDataAccess.GetDataTable(strSQL2);
                                    if (dtNot.Rows.Count > 0)
                                    {
                                        if (dtDet.Rows.Count == dtNot.Rows.Count)
                                        {
                                            if (dtDet.Rows.Count == 1)
                                            {
                                                if (Convert.ToDecimal(dtDet.Rows[0]["ALIQUOTA"]) != Convert.ToDecimal(dtNot.Rows[0]["ALIQUOTA"]))
                                                    generaRettifiche.Module_Rettifiche_05(this.objDataAccess, u, ref dtDet, ref dtNot, Convert.ToInt32(dataTable2.Rows[index1]["CODPOS"]), Convert.ToInt32(dataTable2.Rows[index1]["ANNDEN"]), Convert.ToInt32(dataTable2.Rows[index1]["MESDEN"]), Convert.ToInt32(dataTable2.Rows[index1]["PRODEN"]), Convert.ToInt32(mat), TIPMOVSAN, DATA_ORA_SISTEMA, TIPPRI, TIPISC, DATINISAN, DATFINSAN, "X");
                                            }
                                            else
                                            {
                                                int num5 = dtDet.Rows.Count - 1;
                                                for (int index2 = 0; index2 <= num5; ++index2)
                                                {
                                                    if (Convert.ToDecimal(dtDet.Rows[0]["ALIQUOTA"]) != Convert.ToDecimal(dtNot.Rows[0]["ALIQUOTA"]))
                                                        generaRettifiche.Module_Rettifiche_07(this.objDataAccess, u, ref dtDet, ref dtNot, Convert.ToInt32(dataTable2.Rows[index1]["CODPOS"]), Convert.ToInt32(dataTable2.Rows[index1]["ANNDEN"]), Convert.ToInt32(dataTable2.Rows[index1]["MESDEN"]), Convert.ToInt32(dataTable2.Rows[index1]["PRODEN"]), Convert.ToInt32(mat), TIPMOVSAN, DATA_ORA_SISTEMA, TIPPRI, TIPISC, DATINISAN, DATFINSAN, "X");
                                                }
                                            }
                                        }
                                        else if (dtNot.Rows.Count == 2 & dtDet.Rows.Count == 1)
                                            generaRettifiche.Module_Rettifiche_03(this.objDataAccess, u, ref dtDet, ref dtNot, Convert.ToInt32(dataTable2.Rows[index1]["CODPOS"]), Convert.ToInt32(dataTable2.Rows[index1]["ANNDEN"]), Convert.ToInt32(dataTable2.Rows[index1]["MESDEN"]), Convert.ToInt32(dataTable2.Rows[index1]["PRODEN"]), Convert.ToInt32(mat), TIPMOVSAN, DATA_ORA_SISTEMA, TIPPRI, TIPISC, DATINISAN, DATFINSAN, "X");
                                    }
                                    str6 = "";
                                    str4 = "";
                                    str5 = "";
                                    str3 = "";
                                }
                            }
                        }
                    }
                    if ((double)num1 != Convert.ToDouble(rdl.datiContrattuali.aliqContval))
                    {
                        string str13 = " SELECT DISTINCT A.CODPOS, A.ANNDEN, A.MESDEN, A.PRODEN, A.TIPMOV, B.ANNCOM FROM DENTES A, DENDET B" + " WHERE A.CODPOS = B.CODPOS AND A.ANNDEN = B.ANNDEN " + " AND A.MESDEN = B.MESDEN AND A.PRODEN = B.PRODEN" + " AND A.CODPOS = " + codpos + " AND A.TIPMOV IN ('AR')  AND A.NUMMOV IS NOT NULL AND A.NUMMOVANN IS NULL";
                        dateTime1 = Convert.ToDateTime(rdl.datiContrattuali.DataNuovaVariazione.Replace("'", ""));
                        num3 = dateTime1.Year;
                        string str14 = num3.ToString().Trim();
                        string strSQL3 = str13 + " AND B.ANNCOM >= " + str14 + " ORDER BY A.ANNDEN, A.MESDEN";
                        dataTable2.Clear();
                        dataTable2 = this.objDataAccess.GetDataTable(strSQL3);
                        if (dataTable2.Rows.Count > 0)
                        {
                            string TIPPRI = this.objDataAccess.Get1ValueFromSQL("SELECT PRIORITA FROM TIPPRIRET WHERE CODTIPRET = 10", CommandType.Text).ToString();
                            dtAziende.Columns.Clear();
                            dtAziende.Columns.Add(new DataColumn("CODPOS", Type.GetType("System.Int32")));
                            dtAziende.Columns.Add(new DataColumn("ANNO", Type.GetType("System.Int32")));
                            dtAziende.Columns.Add(new DataColumn("MESE", Type.GetType("System.Int32")));
                            dtAziende.Columns.Add(new DataColumn("DATSCA", Type.GetType("System.String")));
                            dtAziende.Columns.Add(new DataColumn("TIPISC", Type.GetType("System.String")));
                            dtAziende.Columns.Add(new DataColumn("RIMUOVI", Type.GetType("System.String")));
                            int num6 = dataTable2.Rows.Count - 1;
                            for (int index3 = 0; index3 <= num6; ++index3)
                            {
                                if (Convert.ToBoolean((object)(dataTable2.Rows[index3]["ANNDEN"], 2003, false)))
                                {
                                    string TIPISC = Convert.ToString(this.objDataAccess.Get1ValueFromSQL("SELECT TIPISC FROM AZISTO WHERE CODPOS = " + dataTable2.Rows[index3]["CODPOS"]?.ToString() + " AND '" + dataTable2.Rows[index3]["ANNDEN"]?.ToString() + "-" + dataTable2.Rows[index3]["MESDEN"].ToString().Trim().PadLeft(2, '0') + "-01' BETWEEN DATINI AND DATFIN", CommandType.Text)).Trim();
                                    string DATA_ORA_SISTEMA = "CURRENT_TIMESTAMP";
                                    valueTuple = ("01/" + dataTable2.Rows[index3]["MESDEN"].ToString().Trim().PadLeft(2, '0') + "/", dataTable2.Rows[index3]["ANNDEN"]);
                                    string strDataDecorrenza = valueTuple.ToString();
                                    dateTime1 = Convert.ToDateTime(clsWriteDb.Module_GetValorePargen(this.objDataAccess, 3, strDataDecorrenza, Convert.ToInt32(codpos)));
                                    dateTime1 = dateTime1.AddDays(1.0);
                                    string DATINISAN = dateTime1.ToString();
                                    dateTime1 = Convert.ToDateTime(DATA_ORA_SISTEMA);
                                    string DATFINSAN = dateTime1.ToString();
                                    if (Convert.ToDateTime(DATINISAN) > Convert.ToDateTime(DATFINSAN))
                                        DATFINSAN = DATINISAN;
                                    dtAziende.Clear();
                                    DataRow row = dtAziende.NewRow();
                                    row["CODPOS"] = dataTable2.Rows[index3]["CODPOS"];
                                    row["ANNO"] = dataTable2.Rows[index3]["ANNDEN"];
                                    row["MESE"] = dataTable2.Rows[index3]["MESDEN"];
                                    row["TIPISC"] = (object)"";
                                    row["DATSCA"] = (object)"";
                                    row["RIMUOVI"] = (object)"";
                                    dtAziende.Rows.Add(row);
                                    dtNot.Clear();
                                    dtNot = modGeneraNotifiche.Module_Genera_Notifiche(this.objDataAccess, u, ref dtAziende, ref dtLOG, TIPMOVSAN, "", OPTIONAL_MATRICOLA: Convert.ToInt32(rdl.iscritti.mat), OPTIONAL_PRORAP: Convert.ToInt32(prorap));
                                    string strSQL4 = "SELECT DENTES.TIPMOV, DENDET.* FROM DENTES, DENDET " + " WHERE DENTES.CODPOS = DENDET.CODPOS " + " AND DENTES.ANNDEN = DENDET.ANNDEN " + " AND DENTES.MESDEN = DENDET.MESDEN " + " AND DENTES.PRODEN = DENDET.PRODEN " + " AND DENTES.CODPOS = " + dataTable2.Rows[index3]["CODPOS"]?.ToString() + " AND DENTES.ANNDEN = " + dataTable2.Rows[index3]["ANNDEN"]?.ToString() + " AND DENTES.MESDEN = " + dataTable2.Rows[index3]["MESDEN"]?.ToString() + " AND DENTES.PRODEN = " + dataTable2.Rows[index3]["PRODEN"]?.ToString() + " AND ANNCOM = " + dataTable2.Rows[index3]["ANNCOM"]?.ToString() + " AND MAT = " + rdl.iscritti.mat + " AND PRORAP = " + prorap + " AND VALUE(DENDET.ESIRET, '') <> 'S' " + " AND DENDET.NUMMOV IS NOT NULL" + " AND DENDET.NUMMOVANN IS NULL" + " ORDER BY DAL";
                                    dtDet.Clear();
                                    dtDet = this.objDataAccess.GetDataTable(strSQL4);
                                    if (dtNot.Rows.Count > 0)
                                    {
                                        if (dtDet.Rows.Count == dtNot.Rows.Count)
                                        {
                                            if (dtDet.Rows.Count == 1)
                                            {
                                                if (Convert.ToDecimal(dtDet.Rows[0]["ALIQUOTA"]) != Convert.ToDecimal(dtNot.Rows[0]["ALIQUOTA"]))
                                                    generaRettifiche.Module_Rettifiche_05(this.objDataAccess, u, ref dtDet, ref dtNot, Convert.ToInt32(dataTable2.Rows[index3]["CODPOS"]), Convert.ToInt32(dataTable2.Rows[index3]["ANNDEN"]), Convert.ToInt32(dataTable2.Rows[index3]["MESDEN"]), Convert.ToInt32(dataTable2.Rows[index3]["PRODEN"]), Convert.ToInt32(mat), TIPMOVSAN, DATA_ORA_SISTEMA, TIPPRI, TIPISC, DATINISAN, DATFINSAN, "X");
                                            }
                                            else
                                            {
                                                int num7 = dtDet.Rows.Count - 1;
                                                for (int index4 = 0; index4 <= num7; ++index4)
                                                {
                                                    if (Convert.ToDecimal(dtDet.Rows[0]["ALIQUOTA"]) != Convert.ToDecimal(dtNot.Rows[0]["ALIQUOTA"]))
                                                        generaRettifiche.Module_Rettifiche_07(this.objDataAccess, u, ref dtDet, ref dtNot, Convert.ToInt32(dataTable2.Rows[index3]["CODPOS"]), Convert.ToInt32(dataTable2.Rows[index3]["ANNDEN"]), Convert.ToInt32(dataTable2.Rows[index3]["MESDEN"]), Convert.ToInt32(dataTable2.Rows[index3]["PRODEN"]), Convert.ToInt32(mat), TIPMOVSAN, DATA_ORA_SISTEMA, TIPPRI, TIPISC, DATINISAN, DATFINSAN, "X");
                                                }
                                            }
                                        }
                                        else if (dtNot.Rows.Count == 2 & dtDet.Rows.Count == 1)
                                            generaRettifiche.Module_Rettifiche_03(this.objDataAccess, u, ref dtDet, ref dtNot, Convert.ToInt32(dataTable2.Rows[index3]["CODPOS"]), Convert.ToInt32(dataTable2.Rows[index3]["ANNDEN"]), Convert.ToInt32(dataTable2.Rows[index3]["MESDEN"]), Convert.ToInt32(dataTable2.Rows[index3]["PRODEN"]), Convert.ToInt32(mat), TIPMOVSAN, DATA_ORA_SISTEMA, TIPPRI, TIPISC, DATINISAN, DATFINSAN, "X");
                                    }
                                    str6 = "";
                                    str4 = "";
                                    str5 = "";
                                    str3 = "";
                                }
                            }
                        }
                    }
                    string str15 = this.objDataAccess.Get1ValueFromSQL("SELECT VALUE(CHAR(DATCES), '') AS DATCES FROM RAPLAV " + " WHERE CODPOS = " + codpos + " AND MAT =" + mat + " AND PRORAP = " + prorap, CommandType.Text).ToString();
                    if (!string.IsNullOrEmpty(str15))
                    {
                        dateTime1 = Convert.ToDateTime(str15);
                        this.objDataAccess.WriteTransactionData("UPDATE STORDL SET " + " DATFIN = '" + DBMethods.Db2Date(dateTime1.ToString()) + "' " + " WHERE CODPOS = '" + codpos + "' " + " AND MAT = '" + mat + "' " + " AND PRORAP = '" + prorap + "' " + " AND DATINI = (SELECT MAX(DATINI) FROM STORDL " + " WHERE CODPOS = '" + codpos + "' " + " AND MAT = '" + mat + "' " + " AND PRORAP = '" + prorap + "')", CommandType.Text);
                    }
                    idocDatiE1Pitype2.Clear();
                    DataTable idocDatiE1Pitype3 = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0016", Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), 0, "", "", "9999-12-31", "", "", rdl.datiContrattuali.datisc, 0, 0, 0, "", "MODIFICA RDL", "", "");
                    idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "0016", idocDatiE1Pitype3, false);
                    idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "9001", idocDatiE1Pitype3, false);
                    idocRdl.Aggiorna_IDOC(this.objDataAccess);
                    idocRdl.objDtCONTIDOC = (DataTable)null;
                    string strSQL;
                    if (!string.IsNullOrEmpty(rdl.datiContrattuali.CODLOC))
                        strSQL = "SELECT DATANN AS DATANN FROM CONLOC WHERE CODCON = '" + rdl.datiContrattuali.CODCON + "' AND PROCON = '" + rdl.datiContrattuali.PROCON + "' " + " AND CODLOC = '" + rdl.datiContrattuali.CODLOC + "' AND PROLOC = '" + rdl.datiContrattuali.PROLOC + "' ";
                    else
                        strSQL = "SELECT DATANN AS DATANN FROM CONRIF " + " WHERE CODCON = '" + rdl.datiContrattuali.CODCON + "' AND PROCON = '" + rdl.datiContrattuali.PROCON + "' ";
                    dataTable2.Clear();
                    DataTable dataTable6 = this.objDataAccess.GetDataTable(strSQL);
                    if (dataTable6.Rows.Count > 0 && !string.IsNullOrEmpty(dataTable6.Rows[0]["DATANN"].ToString().Trim()) && Convert.ToDateTime(dataTable6.Rows[0]["DATANN"]) < Convert.ToDateTime(rdl.datiContrattuali.DataNuovaVariazione))
                    {
                        MsgErrore = "Il contratto selezionato è stato chiuso. Per i rapporti dal " + dataTable6.Rows[0]["DATANN"]?.ToString() + ", selezionare un altro contratto.";
                        this.objDataAccess.EndTransaction(true);
                    }
                    else
                    {
                        modGetDati.Module_AggiornaRaplav(this.objDataAccess, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap));
                        this.Module_AggiornaStordl(this.objDataAccess, u, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), str8);
                        this.objDataAccess.EndTransaction(true);
                        MSGSuccess = "Operazione completata con successo";
                    }
                }
            }
            catch (Exception ex)
            {
                this.objDataAccess.EndTransaction(false);
                MsgErrore = "Salvataggio non riuscito dati errati. Riprovare";
            }
        }

        public void ModificaRDL(
          GestioneRapportiLavoroIscrittiOCM rdl,
          TFI.OCM.Utente.Utente u,
          ref string MsgErrore,
          ref string MSGSuccess)
        {
            string codpos = rdl.iscritti.CODPOS;
            string mat = rdl.iscritti.mat;
            string prorap = rdl.iscritti.PRORAP;
            string[] strArray1 = rdl.datiContrattuali.CODCON.Split(' ');
            rdl.datiContrattuali.CODCON = strArray1[0];
            rdl.datiContrattuali.CODLOC = strArray1[2];
            Decimal decTraEco = 0M;
            string str1 = "";
            string[] strArray2 = new string[13];
            DataTable dataTable1 = new DataTable();
            DataTable dataTable2 = new DataTable();
            DataTable dtAziende = new DataTable();
            string str2 = "";
            string str3 = "";
            string str4 = "";
            string str5 = "";
            string str6 = "";
            Decimal num1 = 0M;
            DataTable dtNot = new DataTable();
            DataTable dtLOG = new DataTable();
            string TIPMOVSAN = "";
            DataTable dtDet = new DataTable();
            ModGetDati modGetDati = new ModGetDati();
            IDOC_RDL idocRdl = new IDOC_RDL();
            clsWRITE_DB clsWriteDb = new clsWRITE_DB();
            ModGeneraNotifiche modGeneraNotifiche = new ModGeneraNotifiche();
            ModGeneraRettifiche generaRettifiche = new ModGeneraRettifiche();
            if (!modGetDati.Module_RDL_IsCessato(this.objDataAccess, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap)))
            {
                if (!this.Module_Autorizzato(u, 232, ref MsgErrore))
                    return;
            }
            else if (!this.Module_Autorizzato(u, 372, ref MsgErrore))
                return;
            try
            {
                this.objDataAccess.StartTransaction();
                string str7 = this.objDataAccess.Get1ValueFromSQL("SELECT ULTAGG FROM RAPLAV WHERE CODPOS = '" + codpos + "' AND PRORAP='" + prorap + "' AND MAT = '" + mat + "' ", CommandType.Text);
                if (str5 != "" & str7 != str5)
                {
                    MsgErrore = "Attenzione... il record è stato modificato da un'altro utente";
                }
                else
                {
                    string str8;
                    string strDatFin;
                    if (Convert.ToInt32(rdl.datiContrattuali.tiprap) != 1)
                    {
                        str8 = DBMethods.Db2Date(rdl.datiContrattuali.datScadenzaTermineDa);
                        strDatFin = !(rdl.datiContrattuali.datScadenzaTermineAl == "") ? DBMethods.Db2Date(rdl.datiContrattuali.datScadenzaTermineAl) : DBMethods.Db2Date("31-12-9999");
                    }
                    else
                    {
                        str8 = DBMethods.Db2Date(Convert.ToDateTime(rdl.datiContrattuali.DataNuovaVariazione).ToString());
                        strDatFin = DBMethods.Db2Date("31-12-9999");
                    }
                    string strDataScadenzaTermine = "";
                    string strPercentualeApp = "";
                    str1 = "";
                    switch (Convert.ToInt16(rdl.datiContrattuali.tiprap))
                    {
                        case 2:
                        case 5:
                        case 9:
                        case 12:
                            strDataScadenzaTermine = DBMethods.Db2Date(rdl.datiContrattuali.datScadenzaTermine);
                            break;
                        case 3:
                            str1 = rdl.datiContrattuali.PerPar.Replace(",", ".");
                            break;
                        case 4:
                        case 13:
                            strDataScadenzaTermine = DBMethods.Db2Date(rdl.datiContrattuali.datScadenzaTermine);
                            str1 = rdl.datiContrattuali.PerPar.Replace(",", ".");
                            break;
                        case 6:
                        case 7:
                            strDataScadenzaTermine = DBMethods.Db2Date(rdl.datiContrattuali.datScadenzaTermine);
                            strPercentualeApp = rdl.datiContrattuali.PerApp.Replace(",", ".");
                            break;
                        case 8:
                        case 10:
                            strDataScadenzaTermine = DBMethods.Db2Date(rdl.datiContrattuali.datScadenzaTermine);
                            strPercentualeApp = rdl.datiContrattuali.PerApp.Replace(",", ".");
                            str1 = rdl.datiContrattuali.PerPar.Replace(",", ".");
                            break;
                        case 11:
                            str1 = rdl.datiContrattuali.PerPar.Replace(",", ".");
                            if (rdl.mesi.gennaio)
                                strArray2[0] = "1";
                            if (rdl.mesi.febbraio)
                                strArray2[1] = "2";
                            if (rdl.mesi.marzo)
                                strArray2[2] = "3";
                            if (rdl.mesi.aprile)
                                strArray2[3] = "4";
                            if (rdl.mesi.maggio)
                                strArray2[4] = "5";
                            if (rdl.mesi.giugno)
                                strArray2[5] = "6";
                            if (rdl.mesi.luglio)
                                strArray2[6] = "7";
                            if (rdl.mesi.agosto)
                                strArray2[7] = "8";
                            if (rdl.mesi.settembre)
                                strArray2[8] = "9";
                            if (rdl.mesi.ottobre)
                                strArray2[9] = "10";
                            if (rdl.mesi.novembre)
                                strArray2[10] = "11";
                            if (rdl.mesi.dicembre)
                            {
                                strArray2[11] = "12";
                                break;
                            }
                            break;
                        case 15:
                            if (!string.IsNullOrEmpty(rdl.datiContrattuali.datScadenzaTermine))
                            {
                                strDataScadenzaTermine = DBMethods.Db2Date(Convert.ToDateTime(rdl.datiContrattuali.datScadenzaTermine).ToString());
                                break;
                            }
                            break;
                        case 16:
                            strDataScadenzaTermine = DBMethods.Db2Date(Convert.ToDateTime(rdl.datiContrattuali.DatScaTerPart).ToString().Trim());
                            if (rdl.mesi.gennaio)
                                strArray2[0] = "1";
                            if (rdl.mesi.febbraio)
                                strArray2[1] = "2";
                            if (rdl.mesi.marzo)
                                strArray2[2] = "3";
                            if (rdl.mesi.aprile)
                                strArray2[3] = "4";
                            if (rdl.mesi.maggio)
                                strArray2[4] = "5";
                            if (rdl.mesi.giugno)
                                strArray2[5] = "6";
                            if (rdl.mesi.luglio)
                                strArray2[6] = "7";
                            if (rdl.mesi.agosto)
                                strArray2[7] = "8";
                            if (rdl.mesi.settembre)
                                strArray2[8] = "9";
                            if (rdl.mesi.ottobre)
                                strArray2[9] = "10";
                            if (rdl.mesi.novembre)
                                strArray2[10] = "11";
                            if (rdl.mesi.dicembre)
                            {
                                strArray2[11] = "12";
                                break;
                            }
                            break;
                        case 17:
                            if (rdl.mesi.gennaio)
                                strArray2[0] = "1";
                            if (rdl.mesi.febbraio)
                                strArray2[1] = "2";
                            if (rdl.mesi.marzo)
                                strArray2[2] = "3";
                            if (rdl.mesi.aprile)
                                strArray2[3] = "4";
                            if (rdl.mesi.maggio)
                                strArray2[4] = "5";
                            if (rdl.mesi.giugno)
                                strArray2[5] = "6";
                            if (rdl.mesi.luglio)
                                strArray2[6] = "7";
                            if (rdl.mesi.agosto)
                                strArray2[7] = "8";
                            if (rdl.mesi.settembre)
                                strArray2[8] = "9";
                            if (rdl.mesi.ottobre)
                                strArray2[9] = "10";
                            if (rdl.mesi.novembre)
                                strArray2[10] = "11";
                            if (rdl.mesi.dicembre)
                            {
                                strArray2[11] = "12";
                                break;
                            }
                            break;
                    }
                    string strDataUltScatto = string.IsNullOrEmpty(rdl.datiRetributivi.datUltSc) ? "" : DBMethods.Db2Date(Convert.ToDateTime(rdl.datiRetributivi.datUltSc).ToString());
                    string strDataNuovoScatto = string.IsNullOrEmpty(rdl.datiRetributivi.datProsSc) ? "" : DBMethods.Db2Date(Convert.ToDateTime(rdl.datiRetributivi.datProsSc).ToString());
                    if (strPercentualeApp == "0" || string.IsNullOrEmpty(strPercentualeApp))
                        strPercentualeApp = "0";
                    if (rdl.datiContrattuali.PerPar == "0" || string.IsNullOrEmpty(rdl.datiContrattuali.PerPar))
                        rdl.datiContrattuali.PerPar = "0";
                    string asscontr = rdl.datiContrattuali.asscontr;
                    if (!(asscontr == "N") && (asscontr == null || asscontr.Length != 0) && asscontr != null)
                    {
                        if (asscontr == "S")
                            rdl.datiContrattuali.asscontr = "S";
                    }
                    else
                        rdl.datiContrattuali.asscontr = "N";
                    string fap = rdl.datiContrattuali.FAP;
                    if (!(fap == "N") && (fap == null || fap.Length != 0) && fap != null)
                    {
                        if (fap == "S" || fap == "on")
                            rdl.datiContrattuali.FAP = "S";
                    }
                    else
                        rdl.datiContrattuali.FAP = "N";
                    string tipspe = rdl.datiContrattuali.TIPSPE;
                    if (!(tipspe == "S"))
                    {
                        if (tipspe == "M" || tipspe == "A")
                            decTraEco = Convert.ToDecimal(rdl.datiRetributivi.totaleS) - Convert.ToDecimal(rdl.datiRetributivi.S12) - Convert.ToDecimal(rdl.datiRetributivi.S13) - Convert.ToDecimal(rdl.datiRetributivi.S14) - Convert.ToDecimal(rdl.datiRetributivi.S15) - Convert.ToDecimal(rdl.datiRetributivi.S16) - Convert.ToDecimal(rdl.datiRetributivi.importoSc);
                    }
                    else
                        decTraEco = 0M;
                    DataTable idocDatiE1Pitype1 = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0016", Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), 0, "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "MODIFICA RDL", "", "");
                    idocRdl.WRITE_IDOC_TESTATA(this.objDataAccess, idocDatiE1Pitype1.Rows[0]);
                    idocDatiE1Pitype1.Clear();
                    DataTable idocDatiE1Pitype2 = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0016", Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), 0, "", "", "9999-12-31", "", "", rdl.datiContrattuali.datisc, 0, 0, 0, "D", "MODIFICA RDL", "", "");
                    idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "0016", idocDatiE1Pitype2, true);
                    idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "9001", idocDatiE1Pitype2, true);
                    clsWriteDb.WRITE_UPDATE_STORDL(this.objDataAccess, u, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), str8, strDatFin, decTraEco, strPercentualeApp, Convert.ToInt32(rdl.datiContrattuali.tiprap), Convert.ToInt32(rdl.datiContrattuali.CODCON), Convert.ToInt32(rdl.datiContrattuali.CODLOC), Convert.ToInt32(rdl.datiContrattuali.CODLIV), Convert.ToInt32(rdl.datiContrattuali.mensilita), rdl.datiContrattuali.mens14, rdl.datiContrattuali.mens15, rdl.datiContrattuali.mens16, Convert.ToInt32(rdl.datiContrattuali.CODGRUASS), rdl.datiContrattuali.asscontr, rdl.datiContrattuali.AbbPre, rdl.datiContrattuali.PerPar, strDataScadenzaTermine, Convert.ToInt32(rdl.datiRetributivi.scattiAnz), Convert.ToDecimal(rdl.datiRetributivi.importoSc), strDataUltScatto, strDataNuovoScatto, rdl.datiContrattuali.FAP);
                    clsWriteDb.WRITE_DELETE_IMPAGG(this.objDataAccess, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), rdl.datiContrattuali.DataUltimaVar);
                    clsWriteDb.WRITE_INSERT_IMPAGG(this.objDataAccess, u, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), str8, "12", Convert.ToDecimal(rdl.datiRetributivi.S12));
                    clsWriteDb.WRITE_INSERT_IMPAGG(this.objDataAccess, u, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), str8, "13", Convert.ToDecimal(rdl.datiRetributivi.S13));
                    clsWriteDb.WRITE_INSERT_IMPAGG(this.objDataAccess, u, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), str8, "14", Convert.ToDecimal(rdl.datiRetributivi.S14));
                    clsWriteDb.WRITE_INSERT_IMPAGG(this.objDataAccess, u, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), str8, "15", Convert.ToDecimal(rdl.datiRetributivi.S15));
                    clsWriteDb.WRITE_INSERT_IMPAGG(this.objDataAccess, u, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), str8, "16", Convert.ToDecimal(rdl.datiRetributivi.S16));
                    clsWriteDb.WRITE_DELETE_PARTIMM(this.objDataAccess, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), str8.Replace("'", " "));
                    for (int index = 0; index <= strArray2.Length - 1; ++index)
                    {
                        if (strArray2[index] != "")
                            clsWriteDb.WRITE_INSERT_PARTIMM(this.objDataAccess, u, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), str8, Convert.ToInt32(strArray2[index]));
                    }
                    idocRdl.AGGIORNA_RAPLAV_INPS(this.objDataAccess, u, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap));
                    int num2;
                    DateTime dateTime;
                    if (Convert.ToDouble(num1) != Convert.ToDouble(rdl.datiContrattuali.aliqContval))
                    {
                        string[] strArray3 = new string[5]
                        {
              "SELECT CODPOS, ANNDEN, MESDEN, PRODEN FROM DENTES " + " WHERE CODPOS = '" + codpos + "' " + " AND TIPMOV IN ('DP','NU') " + " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL " + " AND TRIM(CHAR(ANNDEN)) || ",
              " RIGHT('00' || TRIM(CHAR(MESDEN)), 2)  >= '",
              null,
              null,
              null
                        };
                        num2 = Convert.ToDateTime(rdl.datiContrattuali.DataNuovaVariazione.Replace("'", "")).Year;
                        strArray3[2] = num2.ToString().Trim();
                        dateTime = Convert.ToDateTime(rdl.datiContrattuali.DataNuovaVariazione.Replace("'", ""));
                        num2 = dateTime.Month;
                        strArray3[3] = num2.ToString().Trim().PadLeft(2, '0');
                        strArray3[4] = "'";
                        string strSQL1 = string.Concat(strArray3) + " ORDER BY ANNDEN, MESDEN ";
                        dataTable2.Clear();
                        dataTable2 = this.objDataAccess.GetDataTable(strSQL1);
                        if (dataTable2.Rows.Count > 0)
                        {
                            string TIPPRI = this.objDataAccess.Get1ValueFromSQL("SELECT PRIORITA FROM TIPPRIRET WHERE CODTIPRET = 10", CommandType.Text);
                            dtAziende.Columns.Clear();
                            dtAziende.Columns.Add(new DataColumn("CODPOS", Type.GetType("System.Int32")));
                            dtAziende.Columns.Add(new DataColumn("ANNO", Type.GetType("System.Int32")));
                            dtAziende.Columns.Add(new DataColumn("MESE", Type.GetType("System.Int32")));
                            dtAziende.Columns.Add(new DataColumn("DATSCA", Type.GetType("System.String")));
                            dtAziende.Columns.Add(new DataColumn("TIPISC", Type.GetType("System.String")));
                            dtAziende.Columns.Add(new DataColumn("RIMUOVI", Type.GetType("System.String")));
                            for (int index1 = 0; index1 <= dataTable2.Rows.Count - 1; ++index1)
                            {
                                if (Convert.ToInt32(dataTable2.Rows[index1]["ANNDEN"]) >= 2003)
                                {
                                    string TIPISC = Convert.ToString(this.objDataAccess.Get1ValueFromSQL("SELECT TIPISC FROM AZISTO WHERE CODPOS = " + dataTable2.Rows[index1]["CODPOS"]?.ToString() + " AND '" + dataTable2.Rows[index1]["ANNDEN"]?.ToString() + "-" + dataTable2.Rows[index1]["MESDEN"].ToString().Trim().PadLeft(2, '0') + "-01' BETWEEN DATINI AND DATFIN", CommandType.Text)).Trim();
                                    string DATA_ORA_SISTEMA = "CURRENT_TIMESTAMP";
                                    string strDataDecorrenza = "01/" + dataTable2.Rows[index1]["MESDEN"].ToString().Trim().PadLeft(2, '0') + "/" + dataTable2.Rows[index1]["ANNDEN"]?.ToString();
                                    dateTime = Convert.ToDateTime(clsWriteDb.Module_GetValorePargen(this.objDataAccess, 3, strDataDecorrenza, Convert.ToInt32(codpos)));
                                    dateTime = dateTime.AddDays(1.0);
                                    string DATINISAN = dateTime.ToString();
                                    dateTime = Convert.ToDateTime(DATA_ORA_SISTEMA);
                                    string DATFINSAN = dateTime.ToString();
                                    if (Convert.ToDateTime(DATINISAN) > Convert.ToDateTime(DATFINSAN))
                                        DATFINSAN = DATINISAN;
                                    dtAziende.Clear();
                                    DataRow row = dtAziende.NewRow();
                                    row["CODPOS"] = dataTable2.Rows[index1]["CODPOS"];
                                    row["ANNO"] = dataTable2.Rows[index1]["ANNDEN"];
                                    row["MESE"] = dataTable2.Rows[index1]["MESDEN"];
                                    row["TIPISC"] = (object)"";
                                    row["DATSCA"] = (object)"";
                                    row["RIMUOVI"] = (object)"";
                                    dtAziende.Rows.Add(row);
                                    dtNot.Clear();
                                    dtNot = modGeneraNotifiche.Module_Genera_Notifiche(this.objDataAccess, u, ref dtAziende, ref dtLOG, TIPMOVSAN, "", OPTIONAL_MATRICOLA: Convert.ToInt32(mat), OPTIONAL_PRORAP: Convert.ToInt32(prorap));
                                    string strSQL2 = "SELECT DENTES.TIPMOV, DENDET.* FROM DENTES, DENDET " + " WHERE DENTES.CODPOS = DENDET.CODPOS " + " AND DENTES.ANNDEN = DENDET.ANNDEN " + " AND DENTES.MESDEN = DENDET.MESDEN " + " AND DENTES.PRODEN = DENDET.PRODEN " + " AND DENTES.CODPOS = '" + dataTable2.Rows[index1]["CODPOS"]?.ToString() + "' " + " AND DENTES.ANNDEN = '" + dataTable2.Rows[index1]["ANNDEN"]?.ToString() + "' " + " AND DENTES.MESDEN = '" + dataTable2.Rows[index1]["MESDEN"]?.ToString() + "' " + " AND DENTES.PRODEN = '" + dataTable2.Rows[index1]["PRODEN"]?.ToString() + "' " + " AND MAT = '" + mat + "' " + " AND PRORAP = '" + prorap + "' " + " AND VALUE(DENDET.ESIRET, '') <> 'S' " + " AND DENDET.NUMMOV IS NOT NULL" + " AND DENDET.NUMMOVANN IS NULL" + " ORDER BY DAL";
                                    dtDet.Clear();
                                    dtDet = this.objDataAccess.GetDataTable(strSQL2);
                                    if (dtNot.Rows.Count > 0)
                                    {
                                        if (dtDet.Rows.Count == dtNot.Rows.Count)
                                        {
                                            if (dtDet.Rows.Count == 1)
                                            {
                                                if (Convert.ToDecimal(dtDet.Rows[0]["ALIQUOTA"]) != Convert.ToDecimal(dtNot.Rows[0]["ALIQUOTA"]))
                                                    generaRettifiche.Module_Rettifiche_05(this.objDataAccess, u, ref dtDet, ref dtNot, Convert.ToInt32(dataTable2.Rows[index1]["CODPOS"]), Convert.ToInt32(dataTable2.Rows[index1]["ANNDEN"]), Convert.ToInt32(dataTable2.Rows[index1]["MESDEN"]), Convert.ToInt32(dataTable2.Rows[index1]["PRODEN"]), Convert.ToInt32(mat), TIPMOVSAN, DATA_ORA_SISTEMA, TIPPRI, TIPISC, DATINISAN, DATFINSAN, "X");
                                            }
                                            else
                                            {
                                                for (int index2 = 0; index2 <= dtDet.Rows.Count - 1; ++index2)
                                                {
                                                    if (Convert.ToDecimal(dtDet.Rows[0]["ALIQUOTA"]) != Convert.ToDecimal(dtNot.Rows[0]["ALIQUOTA"]))
                                                        generaRettifiche.Module_Rettifiche_07(this.objDataAccess, u, ref dtDet, ref dtNot, Convert.ToInt32(dataTable2.Rows[index1]["CODPOS"]), Convert.ToInt32(dataTable2.Rows[index1]["ANNDEN"]), Convert.ToInt32(dataTable2.Rows[index1]["MESDEN"]), Convert.ToInt32(dataTable2.Rows[index1]["PRODEN"]), Convert.ToInt32(mat), TIPMOVSAN, DATA_ORA_SISTEMA, TIPPRI, TIPISC, DATINISAN, DATFINSAN, "X");
                                                }
                                            }
                                        }
                                        else if (dtNot.Rows.Count == 2 & dtDet.Rows.Count == 1)
                                            generaRettifiche.Module_Rettifiche_03(this.objDataAccess, u, ref dtDet, ref dtNot, Convert.ToInt32(dataTable2.Rows[index1]["CODPOS"]), Convert.ToInt32(dataTable2.Rows[index1]["ANNDEN"]), Convert.ToInt32(dataTable2.Rows[index1]["MESDEN"]), Convert.ToInt32(dataTable2.Rows[index1]["PRODEN"]), Convert.ToInt32(mat), TIPMOVSAN, DATA_ORA_SISTEMA, TIPPRI, TIPISC, DATINISAN, DATFINSAN, "X");
                                    }
                                    str6 = "";
                                    str3 = "";
                                    str4 = "";
                                    str2 = "";
                                }
                            }
                        }
                    }
                    if (Convert.ToDouble(num1) != Convert.ToDouble(rdl.datiContrattuali.aliqContval))
                    {
                        string str9 = " SELECT DISTINCT A.CODPOS, A.ANNDEN, A.MESDEN, A.PRODEN, A.TIPMOV, B.ANNCOM FROM DENTES A, DENDET B" + " WHERE A.CODPOS = B.CODPOS AND A.ANNDEN = B.ANNDEN " + " AND A.MESDEN = B.MESDEN AND A.PRODEN = B.PRODEN" + " AND A.CODPOS = '" + codpos + "' " + " AND A.TIPMOV IN ('AR')  AND A.NUMMOV IS NOT NULL AND A.NUMMOVANN IS NULL";
                        dateTime = Convert.ToDateTime(rdl.datiContrattuali.DataNuovaVariazione.Replace("'", ""));
                        num2 = dateTime.Year;
                        string str10 = num2.ToString().Trim();
                        string strSQL3 = str9 + " AND B.ANNCOM >= '" + str10 + "' " + " ORDER BY A.ANNDEN, A.MESDEN";
                        dataTable2.Clear();
                        dataTable2 = this.objDataAccess.GetDataTable(strSQL3);
                        if (dataTable2.Rows.Count > 0)
                        {
                            string TIPPRI = this.objDataAccess.Get1ValueFromSQL("SELECT PRIORITA FROM TIPPRIRET WHERE CODTIPRET = 10", CommandType.Text);
                            dtAziende.Columns.Clear();
                            dtAziende.Columns.Add(new DataColumn("CODPOS", Type.GetType("System.Int32")));
                            dtAziende.Columns.Add(new DataColumn("ANNO", Type.GetType("System.Int32")));
                            dtAziende.Columns.Add(new DataColumn("MESE", Type.GetType("System.Int32")));
                            dtAziende.Columns.Add(new DataColumn("DATSCA", Type.GetType("System.String")));
                            dtAziende.Columns.Add(new DataColumn("TIPISC", Type.GetType("System.String")));
                            dtAziende.Columns.Add(new DataColumn("RIMUOVI", Type.GetType("System.String")));
                            for (int index3 = 0; index3 <= dataTable2.Rows.Count - 1; ++index3)
                            {
                                if (Convert.ToInt32(dataTable2.Rows[index3]["ANNDEN"]) >= 2003)
                                {
                                    string TIPISC = Convert.ToString(this.objDataAccess.Get1ValueFromSQL("SELECT TIPISC FROM AZISTO WHERE CODPOS = " + dataTable2.Rows[index3]["CODPOS"]?.ToString() + " AND '" + dataTable2.Rows[index3]["ANNDEN"]?.ToString() + "-" + dataTable2.Rows[index3]["MESDEN"].ToString().Trim().PadLeft(2, '0') + "-01' BETWEEN DATINI AND DATFIN", CommandType.Text)).Trim();
                                    string DATA_ORA_SISTEMA = " CURRENT_TIMESTAMP";
                                    string strDataDecorrenza = "01/" + dataTable2.Rows[index3]["MESDEN"].ToString().Trim().PadLeft(2, '0') + "/" + dataTable2.Rows[index3]["ANNDEN"]?.ToString();
                                    dateTime = Convert.ToDateTime(clsWriteDb.Module_GetValorePargen(this.objDataAccess, 3, strDataDecorrenza, Convert.ToInt32(codpos)));
                                    dateTime = dateTime.AddDays(1.0);
                                    string DATINISAN = dateTime.ToString();
                                    dateTime = Convert.ToDateTime(DATA_ORA_SISTEMA);
                                    string DATFINSAN = dateTime.ToString();
                                    if (Convert.ToDateTime(DATINISAN) > Convert.ToDateTime(DATFINSAN))
                                        DATFINSAN = DATINISAN;
                                    dtAziende.Clear();
                                    DataRow row = dtAziende.NewRow();
                                    row["CODPOS"] = dataTable2.Rows[index3]["CODPOS"];
                                    row["ANNO"] = dataTable2.Rows[index3]["ANNDEN"];
                                    row["MESE"] = dataTable2.Rows[index3]["MESDEN"];
                                    row["TIPISC"] = (object)"";
                                    row["DATSCA"] = (object)"";
                                    row["RIMUOVI"] = (object)"";
                                    dtAziende.Rows.Add(row);
                                    dtNot.Clear();
                                    dtNot = modGeneraNotifiche.Module_Genera_Notifiche(this.objDataAccess, u, ref dtAziende, ref dtLOG, TIPMOVSAN, "", OPTIONAL_MATRICOLA: Convert.ToInt32(mat), OPTIONAL_PRORAP: Convert.ToInt32(prorap));
                                    string strSQL4 = "SELECT DENTES.TIPMOV, DENDET.* FROM DENTES, DENDET " + " WHERE DENTES.CODPOS = DENDET.CODPOS " + " AND DENTES.ANNDEN = DENDET.ANNDEN " + " AND DENTES.MESDEN = DENDET.MESDEN " + " AND DENTES.PRODEN = DENDET.PRODEN " + " AND DENTES.CODPOS = '" + dataTable2.Rows[index3]["CODPOS"]?.ToString() + "' " + " AND DENTES.ANNDEN = '" + dataTable2.Rows[index3]["ANNDEN"]?.ToString() + "' " + " AND DENTES.MESDEN = '" + dataTable2.Rows[index3]["MESDEN"]?.ToString() + "' " + " AND DENTES.PRODEN = '" + dataTable2.Rows[index3]["PRODEN"]?.ToString() + "' " + " AND ANNCOM = '" + dataTable2.Rows[index3]["ANNCOM"]?.ToString() + "' " + " AND MAT = '" + mat + "' " + " AND PRORAP = '" + prorap + "' " + " AND VALUE(DENDET.ESIRET, '') <> 'S' " + " AND DENDET.NUMMOV IS NOT NULL" + " AND DENDET.NUMMOVANN IS NULL" + " ORDER BY DAL";
                                    dtDet.Clear();
                                    dtDet = this.objDataAccess.GetDataTable(strSQL4);
                                    if (dtNot.Rows.Count > 0)
                                    {
                                        if (dtDet.Rows.Count == dtNot.Rows.Count)
                                        {
                                            if (dtDet.Rows.Count == 1)
                                            {
                                                if (Convert.ToDecimal(dtDet.Rows[0]["ALIQUOTA"]) != Convert.ToDecimal(dtNot.Rows[0]["ALIQUOTA"]))
                                                    generaRettifiche.Module_Rettifiche_05(this.objDataAccess, u, ref dtDet, ref dtNot, Convert.ToInt32(dataTable2.Rows[index3]["CODPOS"]), Convert.ToInt32(dataTable2.Rows[index3]["ANNDEN"]), Convert.ToInt32(dataTable2.Rows[index3]["MESDEN"]), Convert.ToInt32(dataTable2.Rows[index3]["PRODEN"]), Convert.ToInt32(mat), TIPMOVSAN, DATA_ORA_SISTEMA, TIPPRI, TIPISC, DATINISAN, DATFINSAN, "X");
                                            }
                                            else
                                            {
                                                for (int index4 = 0; index4 <= dtDet.Rows.Count - 1; ++index4)
                                                {
                                                    if (Convert.ToDecimal(dtDet.Rows[0]["ALIQUOTA"]) != Convert.ToDecimal(dtNot.Rows[0]["ALIQUOTA"]))
                                                        generaRettifiche.Module_Rettifiche_07(this.objDataAccess, u, ref dtDet, ref dtNot, Convert.ToInt32(dataTable2.Rows[index3]["CODPOS"]), Convert.ToInt32(dataTable2.Rows[index3]["ANNDEN"]), Convert.ToInt32(dataTable2.Rows[index3]["MESDEN"]), Convert.ToInt32(dataTable2.Rows[index3]["PRODEN"]), Convert.ToInt32(mat), TIPMOVSAN, DATA_ORA_SISTEMA, TIPPRI, TIPISC, DATINISAN, DATFINSAN, "X");
                                                }
                                            }
                                        }
                                        else if (dtNot.Rows.Count == 2 & dtDet.Rows.Count == 1)
                                            generaRettifiche.Module_Rettifiche_03(this.objDataAccess, u, ref dtDet, ref dtNot, Convert.ToInt32(dataTable2.Rows[index3]["CODPOS"]), Convert.ToInt32(dataTable2.Rows[index3]["ANNDEN"]), Convert.ToInt32(dataTable2.Rows[index3]["MESDEN"]), Convert.ToInt32(dataTable2.Rows[index3]["PRODEN"]), Convert.ToInt32(mat), TIPMOVSAN, DATA_ORA_SISTEMA, TIPPRI, TIPISC, DATINISAN, DATFINSAN, "X");
                                    }
                                    str6 = "";
                                    str3 = "";
                                    str4 = "";
                                    str2 = "";
                                }
                            }
                        }
                    }
                    string strData = this.objDataAccess.Get1ValueFromSQL("SELECT VALUE(CHAR(DATCES), '') AS DATCES FROM RAPLAV " + " WHERE CODPOS = '" + codpos + "' " + " AND MAT = '" + mat + "' " + " AND PRORAP = '" + prorap + "' ", CommandType.Text);
                    if (strData != "")
                        this.objDataAccess.WriteTransactionData("UPDATE STORDL SET " + " DATFIN = '" + DBMethods.Db2Date(strData) + "' " + " WHERE CODPOS = '" + codpos + "' " + " AND MAT = '" + mat + "' " + " AND PRORAP = '" + prorap + "' " + " AND DATINI = (SELECT MAX(DATINI) FROM STORDL " + " WHERE CODPOS = '" + codpos + "' " + " AND MAT = '" + mat + "' " + " AND PRORAP = '" + prorap + "')", CommandType.Text);
                    idocDatiE1Pitype2.Clear();
                    DataTable idocDatiE1Pitype3 = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0016", Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), 0, "", "", "9999-12-31", "", "", rdl.datiContrattuali.datisc, 0, 0, 0, "", "MODIFICA RDL", "", "");
                    idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "0016", idocDatiE1Pitype3, false);
                    idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "9001", idocDatiE1Pitype3, false);
                    idocRdl.Aggiorna_IDOC(this.objDataAccess);
                    idocRdl.objDtCONTIDOC = (DataTable)null;
                    string strSQL;
                    if (!string.IsNullOrEmpty(rdl.datiContrattuali.CODLOC))
                        strSQL = "SELECT DATANN AS DATANN FROM CONLOC WHERE CODCON = '" + rdl.datiContrattuali.CODCON + "' AND PROCON = '" + rdl.datiContrattuali.PROCON + "' " + " AND CODLOC = '" + rdl.datiContrattuali.CODLOC + "' AND PROLOC = '" + rdl.datiContrattuali.PROLOC + "' ";
                    else
                        strSQL = "SELECT DATANN AS DATANN FROM CONRIF " + " WHERE CODCON = '" + rdl.datiContrattuali.CODCON + "' AND PROCON = '" + rdl.datiContrattuali.PROCON + "' ";
                    dataTable2.Clear();
                    DataTable dataTable3 = this.objDataAccess.GetDataTable(strSQL);
                    if (dataTable3.Rows.Count > 0 && !string.IsNullOrEmpty(dataTable3.Rows[0]["DATANN"].ToString().Trim()) && Convert.ToDateTime(dataTable3.Rows[0]["DATANN"]) < Convert.ToDateTime(rdl.datiContrattuali.DataNuovaVariazione))
                    {
                        MsgErrore = "Il contratto selezionato è stato chiuso. Per i rapporti dal " + dataTable3.Rows[0]["DATANN"]?.ToString() + ", selezionare un altro contratto.";
                        this.objDataAccess.EndTransaction(true);
                    }
                    else
                    {
                        modGetDati.Module_AggiornaRaplav(this.objDataAccess, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap));
                        this.Module_AggiornaStordl(this.objDataAccess, u, Convert.ToInt32(codpos), Convert.ToInt32(mat), Convert.ToInt32(prorap), str8);
                        MSGSuccess = "Operazione completata con successo " + MsgErrore;
                        this.objDataAccess.EndTransaction(true);
                    }
                }
            }
            catch (Exception ex)
            {
                MsgErrore = "Errore nel caricamento dei dati. Riprovare" + ex?.ToString();
                this.objDataAccess.EndTransaction(false);
            }
        }

        public GestioneRapportiLavoroIscrittiOCM DettaglioModificaIscrittiDAL(
          string mat)
        {
            GestioneRapportiLavoroIscrittiOCM rdl = new GestioneRapportiLavoroIscrittiOCM();
            GestioneRapportiLavoroIscrittiOCM.Iscritti iscritti = new GestioneRapportiLavoroIscrittiOCM.Iscritti();
            try
            {
                this.caricaDati(rdl, mat);
                this.CaricaDatiBancariIscritto(rdl, mat);
                this.CaricaDebitiIscritto(rdl, mat);
                this.CaricaEredi(rdl, mat);
                this.CaricaBlocchiIscritto(rdl, mat);
                this.CaricaNoteIscritto(rdl, mat);
                rdl.iscritti.datiNONModificati = this.getDatiForm(rdl.iscritti);
                this.DebitoLoad(rdl, mat);
                this.ErediLoad(rdl);
                this.BloccoLoad(rdl);
            }
            catch (Exception ex)
            {
                return (GestioneRapportiLavoroIscrittiOCM)null;
            }
            return rdl;
        }

        public GestioneRapportiLavoroIscrittiOCM GetMatricolaDAL(
          string mat,
          string nom,
          string cog,
          string codfis)
        {
            GestioneRapportiLavoroIscrittiOCM rdl = new GestioneRapportiLavoroIscrittiOCM();
            try
            {
                if (!string.IsNullOrEmpty(mat) || !string.IsNullOrEmpty(nom) || !string.IsNullOrEmpty(cog) || !string.IsNullOrEmpty(codfis))
                {
                    string str = "";
                    if (!string.IsNullOrEmpty(mat) && string.IsNullOrEmpty(nom) && string.IsNullOrEmpty(cog) && string.IsNullOrEmpty(codfis))
                        str = str + "MAT = '" + mat + "'";
                    if (!string.IsNullOrEmpty(nom) && string.IsNullOrEmpty(cog) && string.IsNullOrEmpty(codfis) && string.IsNullOrEmpty(mat))
                        str = str + "NOM = '" + nom.ToUpper() + "'";
                    if (!string.IsNullOrEmpty(cog) && string.IsNullOrEmpty(nom) && string.IsNullOrEmpty(codfis) && string.IsNullOrEmpty(mat))
                        str = str + "COG = '" + cog.ToUpper() + "'";
                    if (!string.IsNullOrEmpty(codfis) && string.IsNullOrEmpty(nom) && string.IsNullOrEmpty(cog) && string.IsNullOrEmpty(mat))
                        str = str + "CODFIS LIKE '%" + codfis.ToUpper() + "%'";
                    if (!string.IsNullOrEmpty(nom) && !string.IsNullOrEmpty(cog) && string.IsNullOrEmpty(codfis) && string.IsNullOrEmpty(mat))
                        str = str + "NOM = '" + nom.ToUpper() + "' AND COG ='" + cog.ToUpper() + "'";
                    if (!string.IsNullOrEmpty(nom) && string.IsNullOrEmpty(cog) && !string.IsNullOrEmpty(codfis) && string.IsNullOrEmpty(mat))
                        str = str + "NOM = '" + nom.ToUpper() + "' AND CODFIS LIKE '%" + codfis.ToUpper() + "%'";
                    if (string.IsNullOrEmpty(nom) && !string.IsNullOrEmpty(cog) && !string.IsNullOrEmpty(codfis) && string.IsNullOrEmpty(mat))
                        str = str + "COG = '" + cog.ToUpper() + "' AND CODFIS LIKE '%" + codfis.ToUpper() + "%'";
                    if (!string.IsNullOrEmpty(nom) && !string.IsNullOrEmpty(cog) && !string.IsNullOrEmpty(codfis) && string.IsNullOrEmpty(mat))
                        str = str + "NOM = '" + nom.ToUpper() + "' AND COG = '" + cog.ToUpper() + "' AND CODFIS LIKE '%" + codfis.ToUpper() + "%'";
                    if (!string.IsNullOrEmpty(nom) && !string.IsNullOrEmpty(cog) && !string.IsNullOrEmpty(codfis) && !string.IsNullOrEmpty(mat))
                        str = str + "NOM = '" + nom.ToUpper() + "' AND COG = '" + cog.ToUpper() + "' AND CODFIS LIKE '%" + codfis.ToUpper() + "%' AND MAT = '" + mat + "'";
                    if (!string.IsNullOrEmpty(nom) && string.IsNullOrEmpty(cog) && string.IsNullOrEmpty(codfis) && !string.IsNullOrEmpty(mat))
                        str = str + "NOM = '" + nom.ToUpper() + "' AND MAT = '" + mat + "'";
                    if (string.IsNullOrEmpty(nom) && !string.IsNullOrEmpty(cog) && string.IsNullOrEmpty(codfis) && !string.IsNullOrEmpty(mat))
                        str = str + "COG = '" + cog.ToUpper() + "' AND MAT = '" + mat + "'";
                    if (string.IsNullOrEmpty(nom) && string.IsNullOrEmpty(cog) && !string.IsNullOrEmpty(codfis) && !string.IsNullOrEmpty(mat))
                        str = str + "CODFIS = '" + codfis.ToUpper() + "' AND MAT = '" + mat + "'";
                    if (!string.IsNullOrEmpty(nom) && string.IsNullOrEmpty(cog) && !string.IsNullOrEmpty(codfis) && !string.IsNullOrEmpty(mat))
                        str = str + "NOM = '" + nom.ToUpper() + "' AND CODFIS LIKE '%" + codfis.ToUpper() + "%' AND MAT = '" + mat + "'";
                    if (string.IsNullOrEmpty(nom) && !string.IsNullOrEmpty(cog) && !string.IsNullOrEmpty(codfis) && !string.IsNullOrEmpty(mat))
                        str = str + "COG = '" + cog + "' AND CODFIS LIKE '%" + codfis.ToUpper() + "%' AND MAT = '" + mat + "'";
                    if (!string.IsNullOrEmpty(nom) && !string.IsNullOrEmpty(cog) && string.IsNullOrEmpty(codfis) && !string.IsNullOrEmpty(mat))
                        str = str + "NOME = '" + nom.ToUpper() + "' AND COG = '" + cog + "' AND MAT = '" + mat + "'";
                    DataTable dataTable = this.objDataAccess.GetDataTable("SELECT * FROM ISCT WHERE " + str);
                    if (dataTable.Rows.Count == 1)
                    {
                        if (string.IsNullOrEmpty(mat))
                            mat = dataTable.Rows[0]["MAT"].ToString();
                        this.caricaDati(rdl, mat);
                        if (string.IsNullOrEmpty(rdl.iscritti.mat))
                            return (GestioneRapportiLavoroIscrittiOCM)null;
                        rdl.iscritti.datiNONModificati = this.getDatiForm(rdl.iscritti);
                        rdl.iscritti.datiAnagraficiOld = this.ModGetDati.getDatiAnagrafici(rdl);
                        rdl.iscritti.datiRecapitiOld = this.ModGetDati.getDatiRecapiti(rdl);
                        return rdl;
                    }
                }
                return (GestioneRapportiLavoroIscrittiOCM)null;
            }
            catch (Exception ex)
            {
                return (GestioneRapportiLavoroIscrittiOCM)null;
            }
        }

        public void caricaDati(GestioneRapportiLavoroIscrittiOCM rdl, string mat)
        {
            List<GestioneRapportiLavoroIscrittiOCM.TitoliStudio> titoliStudioList = new List<GestioneRapportiLavoroIscrittiOCM.TitoliStudio>();
            List<GestioneRapportiLavoroIscrittiOCM.Indirizzi> indirizziList = new List<GestioneRapportiLavoroIscrittiOCM.Indirizzi>();
            GestioneRapportiLavoroIscrittiOCM.Iscritti iscritti = new GestioneRapportiLavoroIscrittiOCM.Iscritti();
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
            foreach (DataRow row in (InternalDataCollectionBase)this.objDataAccess.GetDataTable("SELECT CODTITSTU, DENTITSTU FROM TITSTU ORDER BY DENTITSTU").Rows)
            {
                GestioneRapportiLavoroIscrittiOCM.TitoliStudio titoliStudio = new GestioneRapportiLavoroIscrittiOCM.TitoliStudio()
                {
                    dentistu = row["DENTITSTU"].ToString(),
                    codtitstu = row["CODTITSTU"].ToString()
                };
                titoliStudioList.Add(titoliStudio);
            }
            rdl.listTitoli = titoliStudioList;
            foreach (DataRow row in (InternalDataCollectionBase)this.objDataAccess.GetDataTable("SELECT CODDUG, DENDUG FROM DUG ORDER BY DENDUG").Rows)
            {
                GestioneRapportiLavoroIscrittiOCM.Indirizzi indirizzi = new GestioneRapportiLavoroIscrittiOCM.Indirizzi()
                {
                    dendug = row["DENDUG"].ToString(),
                    coddug = row["CODDUG"].ToString()
                };
                indirizziList.Add(indirizzi);
            }
            rdl.listaIndirizzi = indirizziList;
            DataTable dataTable1 = new DataTable();
            DataTable dataTable2 = this.objDataAccess.GetDataTable("SELECT ISCT.COG,ISCT.NOM,ISCT.DATNAS,ISCT.CODCOM, " + "( " + "SELECT DENCOM FROM CODCOM WHERE CODCOM.CODCOM=ISCT.CODCOM " + ") AS DENCOMNAS, " + "( " + "SELECT DENTITSTU FROM TITSTU WHERE TITSTU.CODTITSTU=ISCT.TITSTU " + ") AS DENTITSTU, " + "( " + "SELECT SIGPRO FROM CODCOM WHERE CODCOM.CODCOM=ISCT.CODCOM " + ") AS SIGPRONAS, " + "ISCT.SES, " + "ISCT.CODFIS,ISCT.ULTAGG,ISCT.UTEAGG, ISCD.IND,ISCD.CODDUG, " + "( " + "SELECT DENDUG FROM DUG WHERE DUG.CODDUG=ISCD.CODDUG " + ") AS DENDUG, " + " ISCD.CELL, ISCD.EMAILCERT, ISCD.CODCOM AS CODCOMRES, " + "ISCD.NUMCIV,ISCD.DENSTAEST,ISCD.CO, ISCD.DENLOC,ISCD.CAP,ISCD.SIGPRO, " + " ISCD.TEL1, ISCD.TEL2, ISCD.FAX, ISCD.EMAIL, ISCD.URL " + "FROM ISCT INNER JOIN ISCD " + "ON ISCT.MAT=ISCD.MAT " + " WHERE ISCT.MAT = " + mat + " AND ISCD.DATINI=(SELECT MAX(DATINI) FROM ISCD WHERE MAT=" + mat + ")");
            if (dataTable2.Rows.Count > 0)
            {
                iscritti.mat = mat;
                iscritti.ultagg = dataTable2.Rows[0]["ULTAGG"].ToString();
                iscritti.strdatnasold = dataTable2.Rows[0]["DATNAS"].ToString();
                iscritti.nominativo = dataTable2.Rows[0]["COG"].ToString().Trim() + " " + dataTable2.Rows[0]["NOM"].ToString().Trim();
                iscritti.cognome = dataTable2.Rows[0]["COG"].ToString().Trim();
                iscritti.nome = dataTable2.Rows[0]["NOM"].ToString().Trim();
                iscritti.datnas = Convert.ToDateTime(dataTable2.Rows[0]["DATNAS"].ToString().Substring(0, 10)).ToString("yyyy-MM-dd");
                iscritti.comuneCod = dataTable2.Rows[0]["CODCOM"].ToString();
                iscritti.sesso = dataTable2.Rows[0]["SES"].ToString().Trim();
                iscritti.codfis = dataTable2.Rows[0]["CODFIS"].ToString().Trim();
                if (!string.IsNullOrEmpty(dataTable2.Rows[0]["CODCOM"].ToString()))
                {
                    if (dataTable2.Rows[0]["CODCOM"].ToString().Substring(0, 1) != "Z")
                    {
                        iscritti.comuneN = dataTable2.Rows[0]["DENCOMNAS"].ToString().Trim();
                        iscritti.comuneCodN = dataTable2.Rows[0]["CODCOM"].ToString().Trim();
                        iscritti.provN = dataTable2.Rows[0]["SIGPRONAS"].ToString().Trim();
                    }
                    else
                    {
                        iscritti.statoEsN = dataTable2.Rows[0]["DENCOMNAS"].ToString().Trim();
                        iscritti.statoEsCodN = dataTable2.Rows[0]["CODCOM"].ToString().Trim();
                    }
                }
                iscritti.dug = dataTable2.Rows[0]["DENDUG"].ToString().Trim();
                iscritti.coddug = dataTable2.Rows[0]["CODDUG"].ToString().Trim();
                iscritti.titoloStudio = dataTable2.Rows[0]["DENTITSTU"].ToString().Trim();
                iscritti.titoloStudioCod = this.objDataAccess.Get1ValueFromSQL("SELECT CODTITSTU FROM TITSTU  WHERE DENTITSTU ='" + iscritti.titoloStudio + "'", CommandType.Text);
                iscritti.indirizzo = dataTable2.Rows[0]["IND"].ToString().Trim();
                iscritti.numciv = dataTable2.Rows[0]["NUMCIV"].ToString().Trim();
                iscritti.localita = dataTable2.Rows[0]["DENLOC"].ToString().Trim();
                iscritti.prov = dataTable2.Rows[0]["SIGPRO"].ToString().Trim();
                iscritti.cap = dataTable2.Rows[0]["CAP"].ToString().Trim();
                iscritti.statoEs = dataTable2.Rows[0]["DENSTAEST"].ToString().Trim();
                iscritti.tel = dataTable2.Rows[0]["TEL1"].ToString().Trim();
                iscritti.tel2 = dataTable2.Rows[0]["TEL2"].ToString().Trim();
                iscritti.fax = dataTable2.Rows[0]["FAX"].ToString().Trim();
                iscritti.email = dataTable2.Rows[0]["EMAIL"].ToString().Trim();
                iscritti.co = dataTable2.Rows[0]["CO"].ToString().Trim();
                iscritti.url = dataTable2.Rows[0]["URL"].ToString().Trim();
                iscritti.cellulare = dataTable2.Rows[0]["CELL"].ToString().Trim();
                iscritti.pec = dataTable2.Rows[0]["EMAILCERT"].ToString().Trim();
                if (dataTable2.Rows[0]["CODCOMRES"].ToString().Trim() != "")
                {
                    string str = this.objDataAccess.Get1ValueFromSQL("SELECT VALUE(DENCOM,'') AS DENCOM FROM CODCOM WHERE CODCOM = " + DBMethods.DoublePeakForSql(dataTable2.Rows[0]["CODCOMRES"].ToString().Trim()), CommandType.Text);
                    iscritti.comune = str.Trim();
                }
                iscritti.comuneCod = dataTable2.Rows[0]["CODCOMRES"].ToString().Trim();
            }
            DataTable dataTable3 = this.objDataAccess.GetDataTable("SELECT PRIVACY, PRIVACY2 FROM TB_PRIVACY A WHERE MAT = " + mat + " AND DATINI = (SELECT MAX(DATINI) FROM TB_PRIVACY WHERE MAT = A.MAT)");
            if (dataTable3.Rows.Count > 0)
            {
                if (dataTable3.Rows[0]["PRIVACY"].ToString().Trim() != "")
                {
                    if (dataTable3.Rows[0]["PRIVACY"].ToString().Trim() == "S")
                    {
                        iscritti.privacy1SI = true;
                        iscritti.privacy1NO = false;
                    }
                    else
                    {
                        iscritti.privacy1NO = true;
                        iscritti.privacy1SI = false;
                    }
                }
                if (dataTable3.Rows[0]["PRIVACY2"].ToString().Trim() != "")
                {
                    if (dataTable3.Rows[0]["PRIVACY2"].ToString().Trim() == "S")
                    {
                        iscritti.privacy2SI = true;
                        iscritti.privacy2NO = false;
                    }
                    else
                    {
                        iscritti.privacy2NO = true;
                        iscritti.privacy2SI = false;
                    }
                }
            }
            rdl.iscritti = iscritti;
        }

        public void CaricaDatiBancariIscritto(GestioneRapportiLavoroIscrittiOCM rdl, string mat)
        {
            DataTable dataTable1 = new DataTable();
            GestioneRapportiLavoroIscrittiOCM.DatiBancariIscritto datiBancariIscritto1 = new GestioneRapportiLavoroIscrittiOCM.DatiBancariIscritto();
            List<GestioneRapportiLavoroIscrittiOCM.DatiBancariIscritto> datiBancariIscrittoList = new List<GestioneRapportiLavoroIscrittiOCM.DatiBancariIscritto>();
            try
            {
                DataTable dataTable2 = this.objDataAccess.GetDataTable("SELECT MAT, DATINI, DATFIN, MODPAG, IBAN, PROISCBAN FROM ISCBANC WHERE MAT = " + mat);
                datiBancariIscritto1.occorrenze = Convert.ToInt32(dataTable2.Rows.Count);
                if (dataTable2.Rows.Count > 0)
                {
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                    {
                        GestioneRapportiLavoroIscrittiOCM.DatiBancariIscritto datiBancariIscritto2 = new GestioneRapportiLavoroIscrittiOCM.DatiBancariIscritto()
                        {
                            mat = row["MAT"].ToString(),
                            datini = row["DATINI"].ToString().Substring(0, 10),
                            datfin = row["DATFIN"].ToString().Substring(0, 10),
                            modpag = row["MODPAG"].ToString(),
                            iban = row["IBAN"].ToString(),
                            proiscban = row["PROISCBAN"].ToString()
                        };
                        datiBancariIscrittoList.Add(datiBancariIscritto2);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            rdl.listDatiBancariIscritto = datiBancariIscrittoList;
            rdl.datiBancariIscritto = datiBancariIscritto1;
        }

        public void CaricaDebitiIscritto(GestioneRapportiLavoroIscrittiOCM rdl, string mat)
        {
            DataTable dataTable1 = new DataTable();
            GestioneRapportiLavoroIscrittiOCM.DebitiIscritto debitiIscritto1 = new GestioneRapportiLavoroIscrittiOCM.DebitiIscritto();
            List<GestioneRapportiLavoroIscrittiOCM.DebitiIscritto> debitiIscrittoList = new List<GestioneRapportiLavoroIscrittiOCM.DebitiIscritto>();
            try
            {
                DataTable dataTable2 = this.objDataAccess.GetDataTable("SELECT * FROM ISCTRATT WHERE MAT = " + mat);
                debitiIscritto1.occorrenze = Convert.ToInt32(dataTable2.Rows.Count);
                if (dataTable2.Rows.Count > 0)
                {
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                    {
                        GestioneRapportiLavoroIscrittiOCM.DebitiIscritto debitiIscritto2 = new GestioneRapportiLavoroIscrittiOCM.DebitiIscritto()
                        {
                            codpos = row["CODPOS"].ToString(),
                            mat = row["MAT"].ToString(),
                            prorap = row["PRORAP"].ToString(),
                            protratt = row["PROTRATT"].ToString(),
                            tiptratt = row["TIPTRATT"].ToString(),
                            desctratt = row["DESCTRATT"].ToString(),
                            impdebito = row["IMPDEBITO"].ToString(),
                            impestinto = row["IMPESTINTO"].ToString(),
                            impresiduo = row["IMPRESIDUO"].ToString(),
                            flgsap = row["FLGSAP"].ToString(),
                            ultagg = row["ULTAGG"].ToString().Trim().Substring(0, 10),
                            uteagg = row["UTEAGG"].ToString()
                        };
                        debitiIscrittoList.Add(debitiIscritto2);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            rdl.listDebitiIscritto = debitiIscrittoList;
            rdl.debitiIscritto = debitiIscritto1;
        }

        public void CaricaEredi(GestioneRapportiLavoroIscrittiOCM rdl, string mat)
        {
            DataTable dataTable1 = new DataTable();
            GestioneRapportiLavoroIscrittiOCM.Eredi eredi1 = new GestioneRapportiLavoroIscrittiOCM.Eredi();
            List<GestioneRapportiLavoroIscrittiOCM.Eredi> erediList = new List<GestioneRapportiLavoroIscrittiOCM.Eredi>();
            try
            {
                DataTable dataTable2 = this.objDataAccess.GetDataTable("  SELECT A.MAT, A.CODDUG, (SELECT DENDUG FROM DUG WHERE CODDUG = A.CODDUG) AS DENDUG, A.TIPPAR, A.PROERE, A.DATINI, A.DATFIN, A.COGNOME, A.NOME, " + " A.SESSO, A.CODFISC, A.DATNAS, A.CODCOMNAS, A.IND, A.CAP, A.DENLOC, A.SIGPRO, A.PERCTFR, A.PERCFP, " + " A.MAGGIOR, A.PERCMAGG, A.SIGPRORES, A.STAESTRES, A.NUMCIV, B.DENPAR, B.CODPAR," + " (SELECT DATINI FROM EREDIBANC WHERE MAT = A.MAT AND PROERE = A.PROERE AND TIPPAR = A.TIPPAR) AS DATINI," + " (SELECT DATFIN FROM EREDIBANC WHERE MAT = A.MAT AND PROERE = A.PROERE AND TIPPAR = A.TIPPAR) AS DATFIN," + " (SELECT IBAN FROM EREDIBANC WHERE MAT = A.MAT AND PROERE = A.PROERE AND TIPPAR = A.TIPPAR) AS IBAN," + " (SELECT PROBANC FROM EREDIBANC WHERE MAT = A.MAT AND PROERE = A.PROERE AND TIPPAR = A.TIPPAR) AS PROBANC" + " FROM EREDI A, TIPPAR B WHERE A.TIPPAR = B.CODPAR AND A.MAT = " + mat);
                eredi1.occorrenzeE = Convert.ToInt32(dataTable2.Rows.Count);
                if (dataTable2.Rows.Count > 0)
                {
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                    {
                        GestioneRapportiLavoroIscrittiOCM.Eredi eredi2 = new GestioneRapportiLavoroIscrittiOCM.Eredi()
                        {
                            cognomeE = row["COGNOME"].ToString(),
                            nomeE = row["NOME"].ToString(),
                            sessoE = row["SESSO"].ToString(),
                            codfisE = row["CODFISC"].ToString(),
                            datnasE = row["DATNAS"].ToString().Substring(0, 10),
                            denparE = row["DENPAR"].ToString(),
                            proereE = row["PROERE"].ToString()
                        };
                        erediList.Add(eredi2);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            rdl.listEredi = erediList;
            rdl.eredi = eredi1;
        }

        public void CaricaBlocchiIscritto(GestioneRapportiLavoroIscrittiOCM rdl, string mat)
        {
            DataTable dataTable1 = new DataTable();
            GestioneRapportiLavoroIscrittiOCM.BlocchiIscritto blocchiIscritto1 = new GestioneRapportiLavoroIscrittiOCM.BlocchiIscritto();
            List<GestioneRapportiLavoroIscrittiOCM.BlocchiIscritto> blocchiIscrittoList = new List<GestioneRapportiLavoroIscrittiOCM.BlocchiIscritto>();
            try
            {
                DataTable dataTable2 = this.objDataAccess.GetDataTable("SELECT a.id,a.mat,(SELECT DENBLOCCO FROM TIPBLOCCO WHERE TIPBLOCCO=a.TIPOBLOCCO),a.TIPOBLOCCO ,(SELECT DENERRORE FROM TIPERRORE WHERE TIPERRORE=a.TIPOERRORE),a.TIPOERRORE FROM ISCTBLOC a WHERE MAT = " + mat);
                blocchiIscritto1.occorrenze = Convert.ToInt32(dataTable2.Rows.Count);
                if (dataTable2.Rows.Count > 0)
                {
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                    {
                        GestioneRapportiLavoroIscrittiOCM.BlocchiIscritto blocchiIscritto2 = new GestioneRapportiLavoroIscrittiOCM.BlocchiIscritto()
                        {
                            id = row["ID"].ToString(),
                            tipoBlocco = row["TIPOBLOCCO"].ToString(),
                            tipoErrore = row["TIPOERRORE"].ToString(),
                            denblocco = row["DENBLOCCO"].ToString(),
                            denerrore = row["DENERRORE"].ToString()
                        };
                        blocchiIscrittoList.Add(blocchiIscritto2);
                    }
                }
                this.Module_Carica_TIPBLOCCO();
                this.Module_Carica_TIPERRORE();
            }
            catch (Exception ex)
            {
            }
            rdl.listBlocchiIscritto = blocchiIscrittoList;
            rdl.blocchiIscritto = blocchiIscritto1;
        }

        public void CaricaNoteIscritto(GestioneRapportiLavoroIscrittiOCM rdl, string mat)
        {
            DataTable dataTable1 = new DataTable();
            GestioneRapportiLavoroIscrittiOCM.NoteIscritto noteIscritto1 = new GestioneRapportiLavoroIscrittiOCM.NoteIscritto();
            List<GestioneRapportiLavoroIscrittiOCM.NoteIscritto> noteIscrittoList = new List<GestioneRapportiLavoroIscrittiOCM.NoteIscritto>();
            try
            {
                DataTable dataTable2 = this.objDataAccess.GetDataTable("SELECT * FROM ISCTNOTE WHERE MAT = " + mat);
                noteIscritto1.occorrenze = Convert.ToInt32(dataTable2.Rows.Count);
                if (dataTable2.Rows.Count > 0)
                {
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                    {
                        GestioneRapportiLavoroIscrittiOCM.NoteIscritto noteIscritto2 = new GestioneRapportiLavoroIscrittiOCM.NoteIscritto()
                        {
                            idnote = row["ID"].ToString(),
                            note = row["NOTE"].ToString()
                        };
                        noteIscrittoList.Add(noteIscritto2);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            rdl.listNoteIscritto = noteIscrittoList;
            rdl.noteIscritto = noteIscritto1;
        }

        public bool BtnSalvaMod_Click(
          GestioneRapportiLavoroIscrittiOCM.Iscritti rdl,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMSG,
          ref string SuccessMSG)
        {
            if (!this.Module_Autorizzato(u, 228, ref ErroreMSG))
                return false;
            if (this.getDatiForm(rdl) == rdl.datiNONModificati)
            {
                ErroreMSG = "Non sono state effettuate modifiche, impossibile continuare";
                return false;
            }
            DataTable dataTable1 = new DataTable();
            int num1 = 0;
            string str1 = "";
            DataTable dataTable2 = new DataTable();
            DataTable dataTable3 = new DataTable();
            DateTime dataSistema = this.Utils.Module_GetDataSistema();
            try
            {
                this.objDataAccess.StartTransaction();
                string str2 = this.objDataAccess.Get1ValueFromSQL("SELECT ULTAGG FROM ISCT WHERE MAT = " + rdl.mat, CommandType.Text);
                if (rdl.ultagg != "" & str2 != rdl.ultagg)
                {
                    ErroreMSG = "Attenzione... il record è stato modificato da un'altro utente";
                    this.objDataAccess.EndTransaction(false);
                    return false;
                }
                IDOC_RDL idocRdl = new IDOC_RDL();
                clsWRITE_DB clsWriteDb = new clsWRITE_DB();
                string strCODCOM = !(rdl.comuneCodN.Trim() != "") ? rdl.comuneCodN.Trim() : rdl.comuneCodN.Trim();
                DataTable idocDatiE1Pitype1 = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0001", 0, Convert.ToInt32(rdl.mat), 0, 0, "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                idocRdl.WRITE_IDOC_TESTATA(this.objDataAccess, idocDatiE1Pitype1.Rows[0]);
                for (int index = 0; index <= idocDatiE1Pitype1.Rows.Count - 1; ++index)
                {
                    idocDatiE1Pitype1.Rows[index]["DATINI"] = idocDatiE1Pitype1.Rows[index]["DATNAS"];
                    idocDatiE1Pitype1.Rows[index]["DATBEGDA"] = idocDatiE1Pitype1.Rows[index]["DATNAS"];
                }
                idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "0002", idocDatiE1Pitype1, true);
                idocDatiE1Pitype1.Clear();
                DataTable idocDatiE1Pitype2 = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0006", 0, Convert.ToInt32(rdl.mat), 0, 0, "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "CANCELLAZIONE", "", "");
                idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "0006", idocDatiE1Pitype2, true);
                clsWriteDb.WRITE_UPDATE_ISCT(this.objDataAccess, u, Convert.ToInt32(rdl.mat), rdl.cognome, rdl.nome, rdl.codfis, rdl.datnas, strCODCOM, rdl.sesso, rdl.titoloStudio);
                clsWriteDb.WRITE_INSERT_ISCD(this.objDataAccess, u, Convert.ToInt32(rdl.mat), dataSistema.ToString(), Convert.ToInt32(rdl.dug), rdl.indirizzo, rdl.numciv, rdl.statoEs, rdl.localita, rdl.cap, rdl.prov, rdl.tel, rdl.tel2, rdl.fax, rdl.email, rdl.url, rdl.comuneCod, rdl.pec, rdl.cellulare, rdl.co);
                this.objDataAccess.WriteTransactionData("DELETE FROM TB_PRIVACY WHERE MAT = " + Convert.ToInt32(rdl.mat).ToString(), CommandType.Text);
                int num2 = Convert.ToInt32(rdl.mat);
                string str3 = "INSERT INTO TB_PRIVACY (MAT, PRIVACY, PRIVACY2, TIPO, DATINI, DATFIN, DATINS, UTEINS) VALUES (" + num2.ToString() + ", ";
                string str4 = !(!rdl.privacy1SI & !rdl.privacy1NO) ? (!rdl.privacy1SI ? str3 + " 'N', " : str3 + " 'S', ") : str3 + " NULL, ";
                this.objDataAccess.WriteTransactionData((!(!rdl.privacy2SI & !rdl.privacy2NO) ? (!rdl.privacy2SI ? str4 + " 'N', " : str4 + " 'S', ") : str4 + " NULL, ") + " 'I', CURRENT_DATE, '9999-12-31', CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ")", CommandType.Text);
                idocDatiE1Pitype2.Clear();
                DataTable idocDatiE1Pitype3 = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0001", 0, Convert.ToInt32(rdl.mat), 0, 0, "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                for (int index = 0; index <= idocDatiE1Pitype3.Rows.Count - 1; ++index)
                    idocDatiE1Pitype3.Rows[index]["DATINI"] = idocDatiE1Pitype3.Rows[index]["DATNAS"];
                idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "0002", idocDatiE1Pitype3, false);
                idocDatiE1Pitype3.Clear();
                DataTable idocDatiE1Pitype4 = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0006", 0, Convert.ToInt32(rdl.mat), 0, 0, "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                for (int index = 0; index <= idocDatiE1Pitype4.Rows.Count - 1; ++index)
                    idocDatiE1Pitype4.Rows[index]["DATENDDA"] = idocDatiE1Pitype4.Rows[index]["DATFIN"];
                idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "0006", idocDatiE1Pitype4, false);
                if (Convert.ToDateTime(rdl.strdatnasold) != Convert.ToDateTime(rdl.datnas) && this.Utils.Module_Check_65Anni(dataSistema, rdl.datnas) | this.Utils.Module_Check_65Anni(dataSistema, rdl.strdatnasold))
                {
                    num2 = Convert.ToInt32(rdl.mat);
                    DataTable dataTable4 = this.objDataAccess.GetDataTable("SELECT CODPOS, PRORAP FROM RAPLAV WHERE MAT = " + num2.ToString() + " AND CURRENT_DATE BETWEEN DATDEC AND VALUE(DATCES, '9999-12-31')");
                    if (dataTable4.Rows.Count > 0)
                    {
                        num2 = Convert.ToDateTime(rdl.strdatnasold).Year;
                        string str5 = num2.ToString().Trim();
                        num2 = Convert.ToDateTime(rdl.strdatnasold).Month;
                        string str6 = num2.ToString().Trim().PadLeft(2, '0');
                        int int32_1 = Convert.ToInt32(str5 + str6);
                        DateTime dateTime = Convert.ToDateTime(rdl.datnas);
                        num2 = dateTime.Year;
                        string str7 = num2.ToString().Trim();
                        dateTime = Convert.ToDateTime(rdl.datnas);
                        num2 = dateTime.Month;
                        string str8 = num2.ToString().Trim().PadLeft(2, '0');
                        int int32_2 = Convert.ToInt32(str7 + str8);
                        if (int32_1 >= int32_2)
                        {
                            for (int index = 0; index <= dataTable4.Rows.Count - 1; ++index)
                            {
                                string[] strArray = new string[7]
                                {
                  "SELECT ANNDEN, MESDEN FROM DENTES ",
                  " WHERE CODPOS = ",
                  dataTable4.Rows[index]["CODPOS"]?.ToString(),
                  " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL AND TRIM(CHAR(ANNDEN)) || RIGHT('00' || TRIM(CHAR(MESDEN)), 2)  >= '",
                  null,
                  null,
                  null
                                };
                                dateTime = Convert.ToDateTime(rdl.datnas);
                                num2 = dateTime.Year;
                                strArray[4] = num2.ToString().Trim();
                                dateTime = Convert.ToDateTime(rdl.datnas);
                                num2 = dateTime.Month;
                                strArray[5] = num2.ToString().Trim().PadLeft(2, '0');
                                strArray[6] = "'";
                                DataTable dataTable5 = this.objDataAccess.GetDataTable(string.Concat(strArray) + " ORDER BY ANNDEN, MESDEN");
                                if (dataTable5.Rows.Count > 0)
                                    clsWriteDb.WRITE_INSERT_MODRDL(this.objDataAccess, u, Convert.ToInt32(dataTable4.Rows[index]["CODPOS"]), Convert.ToInt32(rdl.mat), Convert.ToInt32(dataTable4.Rows[index]["PRORAP"]), Convert.ToInt32(dataTable5.Rows[0]["MESDEN"]), Convert.ToInt32(dataTable5.Rows[0]["ANNDEN"]), 5, rdl.datnas);
                            }
                        }
                        else
                        {
                            for (int index = 0; index <= dataTable4.Rows.Count - 1; ++index)
                            {
                                string[] strArray = new string[7]
                                {
                  "SELECT ANNDEN, MESDEN FROM DENTES ",
                  " WHERE CODPOS = ",
                  dataTable4.Rows[index]["CODPOS"]?.ToString(),
                  " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL AND TRIM(CHAR(ANNDEN)) || RIGHT('00' || TRIM(CHAR(MESDEN)), 2)  >= '",
                  null,
                  null,
                  null
                                };
                                dateTime = Convert.ToDateTime(rdl.strdatnasold.Replace("'", ""));
                                num2 = dateTime.Year;
                                strArray[4] = num2.ToString().Trim();
                                dateTime = Convert.ToDateTime(rdl.strdatnasold.Replace("'", ""));
                                num2 = dateTime.Month;
                                strArray[5] = num2.ToString().Trim().PadLeft(2, '0');
                                strArray[6] = "'";
                                DataTable dataTable6 = this.objDataAccess.GetDataTable(string.Concat(strArray) + " ORDER BY ANNDEN, MESDEN");
                                if (dataTable6.Rows.Count > 0)
                                    clsWriteDb.WRITE_INSERT_MODRDL(this.objDataAccess, u, Convert.ToInt32(dataTable4.Rows[index]["CODPOS"]), Convert.ToInt32(rdl.mat), Convert.ToInt32(dataTable4.Rows[index]["PRORAP"]), Convert.ToInt32(dataTable6.Rows[0]["MESDEN"]), Convert.ToInt32(dataTable6.Rows[0]["ANNDEN"]), 5, rdl.strdatnasold);
                            }
                        }
                    }
                    num2 = Convert.ToInt32(rdl.mat);
                    string strSQL = "SELECT CODPOS FROM RAPLAV WHERE MAT = " + num2.ToString() + " AND CURRENT_DATE NOT BETWEEN DATDEC AND VALUE(DATCES, '9999-12-31')";
                    dataTable4.Clear();
                    DataTable dataTable7 = this.objDataAccess.GetDataTable(strSQL);
                    for (int index = 0; index <= dataTable7.Rows.Count - 1; ++index)
                    {
                        ++num1;
                        str1 = str1 + ", " + dataTable7.Rows[index]["CODPOS"]?.ToString();
                    }
                    if (str1 != "")
                        str1 = str1.Substring(2);
                }
                idocRdl.AGGIORNA_RAPLAV_INPS(this.objDataAccess, u, 0, Convert.ToInt32(rdl.mat), 0);
                idocRdl.Aggiorna_IDOC(this.objDataAccess);
                idocRdl.objDtCONTIDOC = (DataTable)null;
                SuccessMSG = "Operazione completata";
                if (num1 != 0)
                {
                    ref string local = ref ErroreMSG;
                    string[] strArray = new string[6];
                    strArray[0] = "Attenzione per la matricola ";
                    num2 = Convert.ToInt32(rdl.mat);
                    strArray[1] = num2.ToString();
                    strArray[2] = " sono presenti: ";
                    strArray[3] = num1.ToString();
                    strArray[4] = " rapporto/i di lavoro cessati per la/e Posizione/i: ";
                    strArray[5] = str1;
                    string str9 = string.Concat(strArray);
                    local = str9;
                }
                this.objDataAccess.EndTransaction(true);
                return true;
            }
            catch (Exception ex)
            {
                this.objDataAccess.EndTransaction(false);
                return false;
            }
        }

        public bool Module_Autorizzato(TFI.OCM.Utente.Utente u, int intCodFunsis, ref string ErroreMSG)
        {
            DataTable dataTable = this.Module_CaricaDataTableFunsisUtente(u);
            int index = 0;
            while (index <= dataTable.Rows.Count - 1 && Convert.ToInt32(dataTable.Rows[index]["CODFUNSIS"]) != intCodFunsis)
                ++index;
            if (index <= dataTable.Rows.Count - 1)
                return true;
            ErroreMSG = "L'utente non è abilitato a questa funzione";
            return false;
        }

        private string getDatiForm(GestioneRapportiLavoroIscrittiOCM.Iscritti rdl)
        {
            string str1 = "" + rdl.nome.ToUpper().Trim() + "@" + rdl.cognome.ToUpper().Trim() + "@" + rdl.codfis.ToUpper().Trim() + "@" + rdl.datnas + "@" + rdl.sesso + "@" + rdl.comuneN + "@" + rdl.provN + "@" + rdl.statoEsN + "@" + rdl.dug + "@" + rdl.indirizzo + "@" + rdl.numciv + "@" + rdl.comune + "@" + rdl.localita + "@" + rdl.prov + "@" + rdl.cap + "@" + rdl.statoEs + "@" + rdl.tel + "@" + rdl.tel2 + "@" + rdl.fax + "@" + rdl.email + "@" + rdl.url + "@" + rdl.co + "@" + rdl.cellulare + "@" + rdl.pec + "@" + rdl.titoloStudio + "@";
            string str2 = !rdl.privacy1SI ? str1 + "NO@" : str1 + "SI@";
            string str3 = !rdl.privacy2SI ? str2 + "NO@" : str2 + "SI@";
            string str4 = !rdl.privacy1NO ? str3 + "NO@" : str3 + "SI@";
            return !rdl.privacy2NO ? str4 + "NO@" : str4 + "SI@";
        }

        public DataTable Module_CaricaDataTableFunsisUtente(TFI.OCM.Utente.Utente u)
        {
            DataTable dataTable1 = new DataTable();
            DataTable dataTable2 = this.objDataAccess.GetDataTable("SELECT DISTINCT CODFUNSIS FROM (" + " SELECT CODFUNSIS FROM PROUTE " + " WHERE PROUTE.CODUTE = " + DBMethods.DoublePeakForSql(u.Username) + " UNION " + " SELECT CODFUNSIS FROM FUNGRU, GRUSIS, GRUUTE " + " WHERE FUNGRU.CODGRU = GRUSIS.CODGRU " + " AND GRUSIS.CODGRU = GRUUTE.CODGRU " + " And GRUUTE.CODUTE = " + DBMethods.DoublePeakForSql(u.Username) + " UNION " + " SELECT CODFUNSIS FROM PROENPF, ENPUTE " + " WHERE PROENPF.CODFUNENP = ENPUTE.CODFUNENP " + " And ENPUTE.CODUTE = " + DBMethods.DoublePeakForSql(u.Username) + " UNION " + " SELECT CODFUNSIS FROM FUNGRU, ENPGRU, GRUUTE " + " WHERE FUNGRU.CODGRU = ENPGRU.CODGRU " + " AND ENPGRU.CODGRU = GRUUTE.CODGRU " + " And GRUUTE.CODUTE = " + DBMethods.DoublePeakForSql(u.Username) + ") AS TABELLA ORDER BY CODFUNSIS");
            dataTable2.Columns.Add(new DataColumn()
            {
                ColumnName = "MENU"
            });
            DataTable dataTable3 = new DataTable();
            DataTable dataTable4 = this.objDataAccess.GetDataTable("SELECT DISTINCT FUNSIS FROM (" + "(SELECT DISTINCT FUNSIS FROM MENU WHERE FUNSIS IS NOT NULL AND FUNSIS<>0)" + " UNION " + "(SELECT DISTINCT FUNSIS FROM SUBMENU WHERE FUNSIS IS NOT NULL AND FUNSIS<>0))" + " AS TABELLA");
            for (int index1 = 0; index1 <= dataTable4.Rows.Count - 1; ++index1)
            {
                for (int index2 = 0; index2 <= dataTable2.Rows.Count - 1; ++index2)
                {
                    if (dataTable4.Rows[index1]["FUNSIS"] == dataTable2.Rows[index2]["CODFUNSIS"])
                    {
                        dataTable2.Rows[index2]["MENU"] = (object)"S";
                        break;
                    }
                }
            }
            if (u.Username != "LMRDRN77R49H501V")
            {
                DataRow row = dataTable2.NewRow();
                row[0] = (object)2997;
                dataTable2.Rows.Add(row);
            }
            for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
            {
                if (2997 == Convert.ToInt32(dataTable2.Rows[index]["CODFUNSIS"]))
                {
                    dataTable2.Rows[index]["MENU"] = (object)"S";
                    break;
                }
            }
            return dataTable2;
        }

        public void Module_Carica_TIPBLOCCO()
        {
            DataTable dataTable = new DataTable();
            dataTable = this.objDataAccess.GetDataTable("SELECT TIPBLOCCO, DENBLOCCO FROM TIPBLOCCO ORDER BY DENBLOCCO");
        }

        public void Module_Carica_TIPERRORE()
        {
            DataTable dataTable = new DataTable();
            dataTable = this.objDataAccess.GetDataTable("SELECT TIPERRORE, DENERRORE FROM TIPERRORE ORDER BY DENERRORE");
        }

        public void mnuEliminaDatBanc_Click(
          string proiscban,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMsg,
          ref string SuccessMSG,
          string mat)
        {
            if (!this.Module_Autorizzato(u, 500, ref ErroreMsg))
                return;
            try
            {
                this.objDataAccess.StartTransaction();
                this.objDataAccess.WriteTransactionData(" DELETE FROM ISCBANC WHERE MAT='" + mat + "'" + " AND PROISCBAN='" + proiscban + "' ", CommandType.Text);
                this.objDataAccess.EndTransaction(true);
                SuccessMSG = "Operazione effettuata con successo";
            }
            catch (Exception ex)
            {
                this.objDataAccess.EndTransaction(false);
                ErroreMsg = "Errore durante l'operazione";
            }
        }

        public void mnuEliminaBlocchi_Click(
          string bloccoID,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMsg,
          ref string SuccessMSG,
          string mat)
        {
            if (!this.Module_Autorizzato(u, 517, ref ErroreMsg))
                return;
            try
            {
                this.objDataAccess.StartTransaction();
                this.objDataAccess.WriteTransactionData(" DELETE FROM ISCTBLOC WHERE MAT='" + mat + "'" + " AND ID = '" + bloccoID + "'", CommandType.Text);
                this.objDataAccess.EndTransaction(true);
                SuccessMSG = "Operazione effettuata con successo";
            }
            catch (Exception ex)
            {
                this.objDataAccess.EndTransaction(false);
            }
        }

        public void mnuEliminaDebito_Click(
          string codpos,
          string prorap,
          string protratt,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMsg,
          ref string SuccessMSG,
          string mat)
        {
            if (!this.Module_Autorizzato(u, 498, ref ErroreMsg))
                return;
            try
            {
                this.objDataAccess.StartTransaction();
                this.objDataAccess.WriteTransactionData(" DELETE FROM ISCTRATT WHERE CODPOS ='" + codpos + "'" + " AND MAT ='" + mat + "'" + " AND PRORAP ='" + prorap + "'" + " AND PROTRATT ='" + protratt + "'", CommandType.Text);
                this.objDataAccess.EndTransaction(true);
                SuccessMSG = "Operazione effettuata con successo";
            }
            catch (Exception ex)
            {
                this.objDataAccess.EndTransaction(false);
            }
        }

        public void mnuEliminaErede_Click(
          string proereE,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMsg,
          ref string SuccessMSG,
          string mat)
        {
            if (!this.Module_Autorizzato(u, 497, ref ErroreMsg))
                return;
            try
            {
                this.objDataAccess.StartTransaction();
                this.objDataAccess.WriteTransactionData(" DELETE FROM EREDI WHERE MAT='" + mat + "'" + " AND PROERE='" + proereE + "' ", CommandType.Text);
                this.objDataAccess.WriteTransactionData(" DELETE FROM EREDIBANC WHERE MAT='" + mat + "' " + " AND PROERE='" + proereE + "' ", CommandType.Text);
                this.objDataAccess.EndTransaction(true);
                SuccessMSG = "Operazione effettuata con successo";
            }
            catch (Exception ex)
            {
                this.objDataAccess.EndTransaction(false);
            }
        }

        public void mnuEliminaNote_Click(
          string NoteID,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMsg,
          ref string SuccessMSG,
          string mat)
        {
            if (!this.Module_Autorizzato(u, 522, ref ErroreMsg))
                return;
            try
            {
                this.objDataAccess.StartTransaction();
                this.objDataAccess.WriteTransactionData(" DELETE FROM ISCTNOTE WHERE MAT='" + mat + "'" + " AND ID = '" + NoteID + "'", CommandType.Text);
                SuccessMSG = "Operazione effettuata con successo";
                this.objDataAccess.EndTransaction(true);
            }
            catch (Exception ex)
            {
                this.objDataAccess.EndTransaction(false);
            }
        }

        public void SalvaNote(
          GestioneRapportiLavoroIscrittiOCM rdl,
          string mat,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMsg,
          ref string SuccessMSG)
        {
            try
            {
                this.objDataAccess.StartTransaction();
                if (Convert.ToInt32(rdl.noteIscritto.idnote) == 0)
                    this.objDataAccess.WriteTransactionData("INSERT INTO ISCTNOTE(ID, MAT, NOTE, ULTAGG, UTEAGG)" + "VALUES( " + Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL("SELECT VALUE(MAX(ID), 0) + 1 FROM ISCTNOTE WHERE MAT = " + mat, CommandType.Text)).ToString() + ", " + mat + ", " + DBMethods.DoublePeakForSql(rdl.noteIscritto.note) + ", " + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ") ", CommandType.Text);
                else
                    this.objDataAccess.WriteTransactionData("UPDATE ISCTNOTE SET NOTE = " + DBMethods.DoublePeakForSql(rdl.noteIscritto.note) + " WHERE MAT = " + mat + "  AND ID = " + rdl.noteIscritto.idnote, CommandType.Text);
                this.objDataAccess.EndTransaction(true);
                SuccessMSG = "Operazione completata";
            }
            catch (Exception ex)
            {
                ErroreMsg = "Errore, Impossibile Salvare";
                this.objDataAccess.EndTransaction(false);
            }
        }

        public void frmRDL_LoadInserimeto(string codpos)
        {
            DataTable dataTable1 = new DataTable();
            GestioneRapportiLavoroIscrittiOCM rdl = new GestioneRapportiLavoroIscrittiOCM();
            ModGetDati modGetDati = new ModGetDati();
            try
            {
                DataTable dataTable2 = this.objDataAccess.GetDataTable("SELECT RAGSOC, NATGIU FROM AZI WHERE CODPOS = " + codpos);
                dataTable2.Rows[0]["RAGSOC"].ToString().Trim().ToUpper();
                if (Convert.ToInt32(dataTable2.Rows[0]["NATGIU"]) == 10)
                {
                    rdl.datiContrattuali.strGruCon = "S";
                    rdl.datiContrattuali.CbAssCon = "N";
                }
                else
                {
                    rdl.datiContrattuali.strGruCon = "N";
                    rdl.datiContrattuali.CbAssCon = "S";
                }
                modGetDati.Module_Carica_DUG(this.objDataAccess, rdl);
                modGetDati.Module_Carica_TIPRAP(this.objDataAccess, rdl);
                modGetDati.Module_Carica_TITSTU(this.objDataAccess, rdl);
                this.Utils.Module_GetDataSistema().ToString();
                string strSQL = "SELECT ABB, ABBDEFMAT, TIPISC, (SELECT DENTIPISC FROM TIPISC WHERE CODTIPISC = TIPISC) AS DENTIPISC" + " FROM AZISTO WHERE CODPOS = " + codpos + " AND DATINI <= CURRENT_DATE ORDER BY DATCOM DESC";
                dataTable2.Clear();
                DataTable dataTable3 = this.objDataAccess.GetDataTable(strSQL);
                if (dataTable3.Rows.Count > 0)
                {
                    if (dataTable3.Rows[0]["TIPISC"].ToString().Trim() == "F")
                        rdl.datiContrattuali.gbAliquota = "Aliquota Contributiva - Tipo Iscrizione Azienda: " + dataTable3.Rows[0]["DENTIPISC"].ToString().Trim();
                    rdl.datiContrattuali.AbbPre = "N";
                }
                else
                    rdl.datiContrattuali.AbbPre = "N";
                rdl.datiContrattuali.FAP = "N";
            }
            catch (Exception ex)
            {
            }
        }

        public void btnSalva_Click(
          TFI.OCM.Utente.Utente u,
          GestioneRapportiLavoroIscrittiOCM rdl,
          string codpos,
          ref string ErroreMsg,
          ref string SuccessMSG)
        {
            if (!this.Module_Autorizzato(u, 229, ref ErroreMsg))
                return;
            Decimal decTraEco = 0M;
            DataTable dataTable1 = new DataTable();
            string[] strArray1 = new string[1];
            string[] strArray2 = new string[1];
            string[] strArray3 = new string[1];
            string[] strArray4 = new string[1];
            string str1 = "";
            string[] strArray5 = new string[13];
            bool flag = false;
            DataTable DT = new DataTable();
            IDOC_RDL idocRdl = new IDOC_RDL();
            clsWRITE_DB clsWriteDb = new clsWRITE_DB();
            ModGeneraNotifiche modGeneraNotifiche = new ModGeneraNotifiche();
            ModGeneraRettifiche generaRettifiche = new ModGeneraRettifiche();
            ModGetDati modGetDati = new ModGetDati();
            try
            {
                this.objDataAccess.StartTransaction();
                DateTime dateTime;
                if (!string.IsNullOrEmpty(rdl.iscritti.mat))
                {
                    string[] strArray6 = new string[5]
                    {
            "SELECT * FROM DENDET WHERE CODPOS = " + codpos + " AND MAT = " + rdl.iscritti.mat + " AND TIPMOV IN ('DP','NU') " + " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL " + " AND TRIM(CHAR(ANNDEN)) || ",
            " RIGHT('00' || TRIM(CHAR(MESDEN)), 2) >= '",
            Convert.ToDateTime(rdl.datiContrattuali.datisc.Replace("'", "")).Year.ToString().Trim(),
            null,
            null
                    };
                    dateTime = Convert.ToDateTime(rdl.datiContrattuali.datisc.Replace("'", ""));
                    strArray6[3] = dateTime.Month.ToString().Trim().PadLeft(2, '0');
                    strArray6[4] = "'";
                    dataTable1 = this.objDataAccess.GetDataTable(string.Concat(strArray6) + " ORDER BY ANNDEN, MESDEN ");
                    if (dataTable1.Rows.Count > 0)
                    {
                        ErroreMsg = "Esistono denunce sul RDL precedente che saranno automaticamente attribuite al nuovo RDL. Continuare?";
                        return;
                    }
                }
                if (string.IsNullOrEmpty(rdl.datiRetributivi.S12))
                    rdl.datiRetributivi.S12 = "0.00";
                if (string.IsNullOrEmpty(rdl.datiRetributivi.S13))
                    rdl.datiRetributivi.S13 = "0.00";
                if (string.IsNullOrEmpty(rdl.datiRetributivi.S14))
                    rdl.datiRetributivi.S14 = "0.00";
                if (string.IsNullOrEmpty(rdl.datiRetributivi.S15))
                    rdl.datiRetributivi.S15 = "0.00";
                if (string.IsNullOrEmpty(rdl.datiRetributivi.S16))
                    rdl.datiRetributivi.S16 = "0.00";
                string[] strArray7;
                string[] strArray8;
                string[] strArray9;
                switch (Convert.ToInt16(rdl.datiContrattuali.tiprap))
                {
                    case 1:
                        strArray7 = new string[2];
                        strArray8 = new string[2];
                        strArray9 = new string[2];
                        string[] strArray10 = new string[2];
                        strArray7[1] = DBMethods.Db2Date(rdl.datiContrattuali.datisc);
                        strArray8[1] = "'9999-12-31'";
                        strArray9[1] = "";
                        strArray10[1] = "";
                        break;
                    case 2:
                    case 9:
                    case 12:
                        strArray7 = new string[2];
                        strArray8 = new string[2];
                        strArray9 = new string[2];
                        string[] strArray11 = new string[2];
                        strArray7[1] = DBMethods.Db2Date(rdl.datiContrattuali.datisc);
                        strArray8[1] = "'9999-12-31'";
                        strArray9[1] = DBMethods.Db2Date(rdl.datiContrattuali.datScadenzaTermine);
                        strArray11[1] = "";
                        break;
                    case 3:
                        strArray7 = new string[2];
                        strArray8 = new string[2];
                        strArray9 = new string[2];
                        string[] strArray12 = new string[2];
                        strArray7[1] = DBMethods.Db2Date(rdl.datiContrattuali.datisc);
                        strArray8[1] = "'9999-12-31'";
                        strArray9[1] = "";
                        strArray12[1] = "";
                        str1 = rdl.datiContrattuali.perParTime.Replace(",", ".");
                        break;
                    case 4:
                    case 13:
                        strArray7 = new string[2];
                        strArray8 = new string[2];
                        strArray9 = new string[2];
                        string[] strArray13 = new string[2];
                        strArray7[1] = DBMethods.Db2Date(rdl.datiContrattuali.datisc);
                        strArray8[1] = "'9999-12-31'";
                        strArray9[1] = DBMethods.Db2Date(rdl.datiContrattuali.datTerParTime);
                        strArray13[1] = "";
                        str1 = rdl.datiContrattuali.perParTime.Replace(",", ".");
                        break;
                    case 6:
                    case 7:
                        strArray7 = new string[4];
                        strArray8 = new string[4];
                        strArray9 = new string[4];
                        string[] strArray14 = new string[4];
                        strArray7[1] = DBMethods.Db2Date(rdl.datiContrattuali.datisc);
                        strArray8[1] = DBMethods.Db2Date(rdl.datiContrattuali.PrimoImpiegoAl1);
                        strArray9[1] = DBMethods.Db2Date(rdl.datiContrattuali.PrimoImpiegoAl3);
                        strArray7[2] = DBMethods.Db2Date(rdl.datiContrattuali.PrimoImpiegoDal2);
                        strArray8[2] = DBMethods.Db2Date(rdl.datiContrattuali.PrimoImpiegoAl2);
                        strArray9[2] = DBMethods.Db2Date(rdl.datiContrattuali.PrimoImpiegoAl3);
                        strArray7[3] = DBMethods.Db2Date(rdl.datiContrattuali.PrimoImpiegoDal3);
                        strArray8[3] = "'9999-12-31'";
                        strArray9[3] = DBMethods.Db2Date(rdl.datiContrattuali.PrimoImpiegoAl3);
                        strArray14[1] = rdl.datiContrattuali.FrazPrimoImpiego1.Replace(",", ".");
                        strArray14[2] = rdl.datiContrattuali.FrazPrimoImpiego2.Replace(",", ".");
                        strArray14[3] = rdl.datiContrattuali.FrazPrimoImpiego3.Replace(",", ".");
                        break;
                    case 8:
                    case 10:
                        strArray7 = new string[4];
                        strArray8 = new string[4];
                        strArray9 = new string[4];
                        string[] strArray15 = new string[4];
                        strArray7[1] = DBMethods.Db2Date(rdl.datiContrattuali.datisc);
                        strArray8[1] = DBMethods.Db2Date(rdl.datiContrattuali.PrimoImpiegoPartTimeAl1);
                        strArray9[1] = DBMethods.Db2Date(rdl.datiContrattuali.PrimoImpiegoPartTimeAl3);
                        strArray7[2] = DBMethods.Db2Date(rdl.datiContrattuali.PrimoImpiegoPartTimeDal2);
                        strArray8[2] = DBMethods.Db2Date(rdl.datiContrattuali.PrimoImpiegoPartTimeAl2);
                        strArray9[2] = DBMethods.Db2Date(rdl.datiContrattuali.PrimoImpiegoPartTimeAl3);
                        strArray7[3] = DBMethods.Db2Date(rdl.datiContrattuali.PrimoImpiegoPartTimeDal3);
                        strArray8[3] = "'9999-12-31'";
                        strArray9[3] = DBMethods.Db2Date(rdl.datiContrattuali.PrimoImpiegoPartTimeAl3);
                        strArray15[1] = rdl.datiContrattuali.FrazPrimoImpiegoPartTime1.Replace(",", ".");
                        strArray15[2] = rdl.datiContrattuali.FrazPrimoImpiegoPartTime2.Replace(",", ".");
                        strArray15[3] = rdl.datiContrattuali.FrazPrimoImpiegoPartTime3.Replace(",", ".");
                        str1 = rdl.datiContrattuali.PartTimePrimoImpiego.Replace(",", ".");
                        break;
                    case 11:
                        strArray7 = new string[2];
                        strArray8 = new string[2];
                        strArray9 = new string[2];
                        string[] strArray16 = new string[2];
                        strArray7[1] = DBMethods.Db2Date(rdl.datiContrattuali.datisc);
                        strArray8[1] = "'9999-12-31'";
                        strArray9[1] = "";
                        strArray16[1] = "";
                        str1 = rdl.datiContrattuali.PartTimeVerticale.Trim().Replace(",", ".");
                        if (rdl.mesi.gennaio)
                            strArray5[0] = "1";
                        if (rdl.mesi.febbraio)
                            strArray5[1] = "2";
                        if (rdl.mesi.marzo)
                            strArray5[2] = "3";
                        if (rdl.mesi.aprile)
                            strArray5[3] = "4";
                        if (rdl.mesi.maggio)
                            strArray5[4] = "5";
                        if (rdl.mesi.giugno)
                            strArray5[5] = "6";
                        if (rdl.mesi.luglio)
                            strArray5[6] = "7";
                        if (rdl.mesi.agosto)
                            strArray5[7] = "8";
                        if (rdl.mesi.settembre)
                            strArray5[8] = "9";
                        if (rdl.mesi.ottobre)
                            strArray5[9] = "10";
                        if (rdl.mesi.novembre)
                            strArray5[10] = "11";
                        if (rdl.mesi.dicembre)
                        {
                            strArray5[11] = "12";
                            break;
                        }
                        break;
                    case 14:
                        strArray7 = new string[2];
                        strArray8 = new string[2];
                        strArray9 = new string[2];
                        string[] strArray17 = new string[2];
                        strArray7[1] = DBMethods.Db2Date(rdl.datiContrattuali.datisc);
                        strArray8[1] = "'9999-12-31'";
                        strArray9[1] = "";
                        strArray17[1] = "";
                        break;
                    case 16:
                        strArray7 = new string[2];
                        strArray8 = new string[2];
                        strArray9 = new string[2];
                        string[] strArray18 = new string[2];
                        strArray7[1] = DBMethods.Db2Date(rdl.datiContrattuali.datisc);
                        strArray8[1] = "'9999-12-31'";
                        strArray9[1] = DBMethods.Db2Date(rdl.datiContrattuali.DatScaTerPart);
                        strArray18[1] = "";
                        if (rdl.mesi.gennaio)
                            strArray5[0] = "1";
                        if (rdl.mesi.febbraio)
                            strArray5[1] = "2";
                        if (rdl.mesi.marzo)
                            strArray5[2] = "3";
                        if (rdl.mesi.aprile)
                            strArray5[3] = "4";
                        if (rdl.mesi.maggio)
                            strArray5[4] = "5";
                        if (rdl.mesi.giugno)
                            strArray5[5] = "6";
                        if (rdl.mesi.luglio)
                            strArray5[6] = "7";
                        if (rdl.mesi.agosto)
                            strArray5[7] = "8";
                        if (rdl.mesi.settembre)
                            strArray5[8] = "9";
                        if (rdl.mesi.ottobre)
                            strArray5[9] = "10";
                        if (rdl.mesi.novembre)
                            strArray5[10] = "11";
                        if (rdl.mesi.dicembre)
                        {
                            strArray5[11] = "12";
                            break;
                        }
                        break;
                    case 17:
                        strArray7 = new string[2];
                        strArray8 = new string[2];
                        strArray9 = new string[2];
                        string[] strArray19 = new string[2];
                        strArray7[1] = DBMethods.Db2Date(rdl.datiContrattuali.datisc);
                        strArray8[1] = "'9999-12-31'";
                        strArray9[1] = "";
                        strArray19[1] = "";
                        if (rdl.mesi.gennaio)
                            strArray5[0] = "1";
                        if (rdl.mesi.febbraio)
                            strArray5[1] = "2";
                        if (rdl.mesi.marzo)
                            strArray5[2] = "3";
                        if (rdl.mesi.aprile)
                            strArray5[3] = "4";
                        if (rdl.mesi.maggio)
                            strArray5[4] = "5";
                        if (rdl.mesi.giugno)
                            strArray5[5] = "6";
                        if (rdl.mesi.luglio)
                            strArray5[6] = "7";
                        if (rdl.mesi.agosto)
                            strArray5[7] = "8";
                        if (rdl.mesi.settembre)
                            strArray5[8] = "9";
                        if (rdl.mesi.ottobre)
                            strArray5[9] = "10";
                        if (rdl.mesi.novembre)
                            strArray5[10] = "11";
                        if (rdl.mesi.dicembre)
                        {
                            strArray5[11] = "12";
                            break;
                        }
                        break;
                    default:
                        strArray7 = new string[2];
                        strArray8 = new string[2];
                        strArray9 = new string[2];
                        string[] strArray20 = new string[2];
                        strArray7[1] = DBMethods.Db2Date(rdl.datiContrattuali.datisc);
                        strArray8[1] = "'9999-12-31'";
                        strArray9[1] = "";
                        strArray20[1] = "";
                        break;
                }
                string strDataUltScatto = !(rdl.datiRetributivi.datUltSc != "") ? "" : rdl.datiRetributivi.datUltSc;
                string strDataNuovoScatto = !(rdl.datiRetributivi.datProsSc != "") ? "" : rdl.datiRetributivi.datProsSc;
                int num1 = !(rdl.iscritti.mat != "") ? 0 : Convert.ToInt32(rdl.iscritti.mat);
                if (rdl.datiContrattuali.PerApp == "0" || string.IsNullOrEmpty(rdl.datiContrattuali.PerApp))
                    rdl.datiContrattuali.PerApp = "0";
                if (rdl.datiContrattuali.PerPar == "0" || string.IsNullOrEmpty(rdl.datiContrattuali.PerPar))
                    rdl.datiContrattuali.PerPar = "0";
                rdl.datiContrattuali.AbbPre = "N";
                if (string.IsNullOrEmpty(rdl.datiContrattuali.mens14))
                    rdl.datiContrattuali.mens14 = "0";
                if (string.IsNullOrEmpty(rdl.datiContrattuali.mens15))
                    rdl.datiContrattuali.mens15 = "0";
                if (string.IsNullOrEmpty(rdl.datiContrattuali.mens16))
                    rdl.datiContrattuali.mens16 = "0";
                string asscontr = rdl.datiContrattuali.asscontr;
                if (!(asscontr == "N") && (asscontr == null || asscontr.Length != 0) && asscontr != null)
                {
                    if (asscontr == "S")
                        rdl.datiContrattuali.asscontr = "S";
                }
                else
                    rdl.datiContrattuali.asscontr = "N";
                string fap = rdl.datiContrattuali.FAP;
                if (!(fap == "N") && (fap == null || fap.Length != 0) && fap != null)
                {
                    if (fap == "S" || fap == "on")
                        rdl.datiContrattuali.FAP = "S";
                }
                else
                    rdl.datiContrattuali.FAP = "N";
                string tipspe1 = rdl.datiContrattuali.TIPSPE;
                if (!(tipspe1 == "S"))
                {
                    if (tipspe1 == "M" || tipspe1 == "A")
                        decTraEco = Convert.ToDecimal(rdl.datiRetributivi.totaleS) - Convert.ToDecimal(rdl.datiRetributivi.S12) - Convert.ToDecimal(rdl.datiRetributivi.S13) - Convert.ToDecimal(rdl.datiRetributivi.S14) - Convert.ToDecimal(rdl.datiRetributivi.S15) - Convert.ToDecimal(rdl.datiRetributivi.S16) - Convert.ToDecimal(rdl.datiRetributivi.importoSc);
                }
                else
                    decTraEco = 0M;
                if (num1 == 0)
                {
                    flag = true;
                    num1 = clsWriteDb.WRITE_INSERT_ISCT(this.objDataAccess, u, rdl.iscritti.cognome, rdl.iscritti.nome, rdl.iscritti.datnas, rdl.iscritti.comuneN, rdl.iscritti.comuneCodN, rdl.iscritti.statoEsCodN, rdl.iscritti.codfis, rdl.iscritti.sesso, Convert.ToInt32(rdl.iscritti.titoloStudioCod));
                    clsWriteDb.WRITE_INSERT_ISCD(this.objDataAccess, u, Convert.ToInt32(rdl.iscritti.mat), rdl.datiContrattuali.datisc, Convert.ToInt32(rdl.iscritti.coddug), rdl.iscritti.indirizzo, rdl.iscritti.numciv, rdl.iscritti.statoEs, rdl.iscritti.localita, rdl.iscritti.cap, rdl.iscritti.prov, rdl.iscritti.tel, rdl.iscritti.tel2, rdl.iscritti.fax, rdl.iscritti.email, rdl.iscritti.url, rdl.iscritti.comune, rdl.iscritti.pec, rdl.iscritti.cellulare, rdl.iscritti.co);
                }
                else
                {
                    DT = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0000", 0, num1, 0, 0, "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                    idocRdl.WRITE_IDOC_TESTATA(this.objDataAccess, DT.Rows[0]);
                    if (this.ModGetDati.getDatiRecapiti(rdl) != rdl.iscritti.datiRecapitiOld)
                    {
                        DT.Clear();
                        DataTable idocDatiE1Pitype = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0006", 0, num1, 0, 0, "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "CANCELLAZIONE", "", "");
                        idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "0006", idocDatiE1Pitype, true);
                        clsWriteDb.WRITE_INSERT_ISCD(this.objDataAccess, u, Convert.ToInt32(rdl.iscritti.mat), rdl.datiContrattuali.datisc, Convert.ToInt32(rdl.iscritti.coddug), rdl.iscritti.indirizzo, rdl.iscritti.numciv, rdl.iscritti.statoEs, rdl.iscritti.localita, rdl.iscritti.cap, rdl.iscritti.prov, rdl.iscritti.tel, rdl.iscritti.tel2, rdl.iscritti.fax, rdl.iscritti.email, rdl.iscritti.url, rdl.iscritti.comune, rdl.iscritti.pec, rdl.iscritti.cellulare, rdl.iscritti.co);
                        idocDatiE1Pitype.Clear();
                        DT = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0006", 0, num1, 0, 0, "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                        for (int index = 0; index <= DT.Rows.Count - 1; ++index)
                        {
                            DT.Rows[index]["DATBEGDA"] = DT.Rows[index]["DATINI"];
                            DT.Rows[index]["DATENDDA"] = DT.Rows[index]["DATFIN"];
                        }
                        idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "0006", DT, false);
                    }
                    if (this.ModGetDati.getDatiAnagrafici(rdl) != rdl.iscritti.datiAnagraficiOld)
                    {
                        DT.Clear();
                        DataTable idocDatiE1Pitype = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0002", 0, num1, 0, 0, "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                        for (int index = 0; index <= idocDatiE1Pitype.Rows.Count - 1; ++index)
                        {
                            idocDatiE1Pitype.Rows[index]["DATINI"] = idocDatiE1Pitype.Rows[index]["DATNAS"];
                            idocDatiE1Pitype.Rows[index]["DATBEGDA"] = idocDatiE1Pitype.Rows[index]["DATNAS"];
                        }
                        idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "0002", idocDatiE1Pitype, true);
                        string str2 = !(rdl.iscritti.statoEsCodN == "") ? rdl.iscritti.statoEsCodN : rdl.iscritti.comuneCodN;
                        clsWriteDb.WRITE_UPDATE_ISCT(this.objDataAccess, u, num1, rdl.iscritti.cognome, rdl.iscritti.nome, rdl.iscritti.codfis, rdl.iscritti.datnas, rdl.iscritti.comuneCodN, rdl.iscritti.sesso, rdl.iscritti.titoloStudioCod);
                        idocDatiE1Pitype.Clear();
                        DT = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0001", 0, num1, 0, 0, "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                        for (int index = 0; index <= DT.Rows.Count - 1; ++index)
                        {
                            DT.Rows[index]["DATINI"] = DT.Rows[index]["DATNAS"];
                            DT.Rows[index]["DATBEGDA"] = DT.Rows[index]["DATNAS"];
                        }
                        idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "0002", DT, false);
                    }
                }
                int num2 = clsWriteDb.WRITE_INSERT_RAPLAV(this.objDataAccess, u, Convert.ToInt32(codpos), num1, rdl.datiContrattuali.datisc, rdl.altriDati.datAssunzione, rdl.altriDati.datDenuncia, rdl.altriDati.numProtocollo, rdl.altriDati.datProtocollo, rdl.aziendaUtilizzatrice.codposAz, rdl.aziendaUtilizzatrice.ragsocAz, rdl.aziendaUtilizzatrice.parivaAz, rdl.aziendaUtilizzatrice.codfisAz, rdl.aziendaUtilizzatrice.indirizzoAz, (object)rdl.aziendaUtilizzatrice.dugCodAz, rdl.aziendaUtilizzatrice.civicoAz, rdl.aziendaUtilizzatrice.localitaAz, rdl.aziendaUtilizzatrice.capAz, rdl.aziendaUtilizzatrice.provAz, rdl.aziendaUtilizzatrice.comuneAz, rdl.aziendaUtilizzatrice.telefonoAz);
                for (int index1 = 1; index1 <= strArray7.Length - 1; ++index1)
                {
                    string tipspe2 = rdl.datiContrattuali.TIPSPE;
                    if (!(tipspe2 == "S"))
                    {
                        if (tipspe2 == "M" || tipspe2 == "A")
                            decTraEco = Convert.ToDecimal(rdl.datiRetributivi.totaleS) - Convert.ToDecimal(rdl.datiRetributivi.S12) - Convert.ToDecimal(rdl.datiRetributivi.S13) - Convert.ToDecimal(rdl.datiRetributivi.S14) - Convert.ToDecimal(rdl.datiRetributivi.S15) - Convert.ToDecimal(rdl.datiRetributivi.S16) - Convert.ToDecimal(rdl.datiRetributivi.importoSc);
                    }
                    else
                        decTraEco = 0M;
                    clsWriteDb.WRITE_INSERT_STORDL(this.objDataAccess, u, Convert.ToInt32(codpos), num1, num2, strArray7[index1], strArray8[index1], decTraEco, rdl.datiContrattuali.PerApp, Convert.ToInt32(rdl.datiContrattuali.tiprap), Convert.ToInt32(rdl.datiContrattuali.contratto), Convert.ToInt32(rdl.datiContrattuali.CODLOC), Convert.ToInt32(rdl.datiContrattuali.livello), Convert.ToInt32(rdl.datiContrattuali.mensilita), rdl.datiContrattuali.mens14, rdl.datiContrattuali.mens15, rdl.datiContrattuali.mens16, Convert.ToInt32(rdl.datiContrattuali.aliqCont), rdl.datiContrattuali.asscontr, rdl.datiContrattuali.AbbPre, rdl.datiContrattuali.PerPar, strArray9[index1].Trim(), Convert.ToInt32(rdl.datiRetributivi.scattiAnz), Convert.ToDecimal(rdl.datiRetributivi.importoSc), strDataUltScatto, strDataNuovoScatto, rdl.datiContrattuali.FAP);
                    clsWriteDb.WRITE_INSERT_IMPAGG(this.objDataAccess, u, Convert.ToInt32(codpos), num1, num2, strArray7[index1], "12", Convert.ToDecimal(rdl.datiRetributivi.S12));
                    clsWriteDb.WRITE_INSERT_IMPAGG(this.objDataAccess, u, Convert.ToInt32(codpos), num1, num2, strArray7[index1], "13", Convert.ToDecimal(rdl.datiRetributivi.S13));
                    clsWriteDb.WRITE_INSERT_IMPAGG(this.objDataAccess, u, Convert.ToInt32(codpos), num1, num2, strArray7[index1], "14", Convert.ToDecimal(rdl.datiRetributivi.S14));
                    clsWriteDb.WRITE_INSERT_IMPAGG(this.objDataAccess, u, Convert.ToInt32(codpos), num1, num2, strArray7[index1], "15", Convert.ToDecimal(rdl.datiRetributivi.S15));
                    clsWriteDb.WRITE_INSERT_IMPAGG(this.objDataAccess, u, Convert.ToInt32(codpos), num1, num2, strArray7[index1], "16", Convert.ToDecimal(rdl.datiRetributivi.S16));
                    for (int index2 = 0; index2 <= strArray5.Length - 1; ++index2)
                    {
                        if (strArray5[index2] != "")
                            clsWriteDb.WRITE_INSERT_PARTIMM(this.objDataAccess, u, Convert.ToInt32(codpos), num1, num2, strArray7[index1], Convert.ToInt32(strArray5[index2]));
                    }
                    this.Module_AggiornaStordl(this.objDataAccess, u, Convert.ToInt32(codpos), num1, num2, strArray7[index1]);
                }
                this.objDataAccess.WriteTransactionData("UPDATE ISCT SET DATISC = (SELECT MIN(DATDEC) FROM RAPLAV WHERE MAT=" + num1.ToString() + ") WHERE MAT=" + num1.ToString(), CommandType.Text);
                idocRdl.AGGIORNA_RAPLAV_INPS(this.objDataAccess, u, Convert.ToInt32(codpos), num1, num2);
                if (flag)
                {
                    DT.Clear();
                    DataTable idocDatiE1Pitype = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0000", 0, num1, 0, 0, "A1", "01", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                    idocRdl.WRITE_IDOC_TESTATA(this.objDataAccess, idocDatiE1Pitype.Rows[0]);
                    idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "0000", idocDatiE1Pitype, false);
                    for (int index = 0; index <= idocDatiE1Pitype.Rows.Count - 1; ++index)
                    {
                        idocDatiE1Pitype.Rows[index]["DATFIN"] = idocDatiE1Pitype.Rows[index]["DATINI"];
                        idocDatiE1Pitype.Rows[index]["DATBEGDA"] = idocDatiE1Pitype.Rows[index]["DATINI"];
                        idocDatiE1Pitype.Rows[index]["DATENDDA"] = idocDatiE1Pitype.Rows[index]["DATINI"];
                    }
                    idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "0302", idocDatiE1Pitype, false);
                    for (int index = 0; index <= idocDatiE1Pitype.Rows.Count - 1; ++index)
                    {
                        idocDatiE1Pitype.Rows[index]["DATFIN"] = (object)"9999-12-31";
                        idocDatiE1Pitype.Rows[index]["DATENDDA"] = (object)"9999-12-31";
                    }
                    idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "0001", idocDatiE1Pitype, false);
                    for (int index = 0; index <= idocDatiE1Pitype.Rows.Count - 1; ++index)
                    {
                        idocDatiE1Pitype.Rows[index]["DATINI"] = idocDatiE1Pitype.Rows[index]["DATNAS"];
                        idocDatiE1Pitype.Rows[index]["DATBEGDA"] = idocDatiE1Pitype.Rows[index]["DATNAS"];
                    }
                    idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "0002", idocDatiE1Pitype, false);
                    for (int index = 0; index <= idocDatiE1Pitype.Rows.Count - 1; ++index)
                    {
                        idocDatiE1Pitype.Rows[index]["DATINISTO"] = (object)rdl.datiContrattuali.datisc;
                        idocDatiE1Pitype.Rows[index]["DATBEGDA"] = (object)"1800-01-01";
                    }
                    idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "0003", idocDatiE1Pitype, false);
                    idocDatiE1Pitype.Clear();
                    DT = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0006", 0, num1, 0, 0, "", "", "9999-12-31", "", "", "", 0, 0, 0, "", "", "", "");
                    idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "0006", DT, false);
                }
                DT.Clear();
                DataTable idocDatiE1Pitype1 = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "0016", Convert.ToInt32(codpos), num1, num2, 0, "", "", "9999-12-31", "", "", rdl.datiContrattuali.datisc, 0, 0, 0, "", "INSERIMENTO RDL", "", "");
                idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "9001", idocDatiE1Pitype1, false);
                idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "0016", idocDatiE1Pitype1, false);
                DataTable dtAziende = new DataTable();
                DataTable dtNot = new DataTable();
                DataTable dtLOG = new DataTable();
                string TIPMOVSAN = "";
                string str3 = "";
                string str4 = "";
                string str5 = "";
                string str6 = "";
                dtLOG.Columns.Add(new DataColumn()
                {
                    ColumnName = "CODPOS"
                });
                dtLOG.Columns.Add(new DataColumn()
                {
                    ColumnName = "ANNDEN"
                });
                dtLOG.Columns.Add(new DataColumn()
                {
                    ColumnName = "MESDEN"
                });
                dtLOG.Columns.Add(new DataColumn()
                {
                    ColumnName = "MAT"
                });
                dtLOG.Columns.Add(new DataColumn()
                {
                    ColumnName = "DESERR"
                });
                for (int index = 0; index <= dataTable1.Rows.Count - 1; ++index)
                {
                    idocDatiE1Pitype1.Clear();
                    DataTable idocDatiE1Pitype2 = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9004", Convert.ToInt32(codpos), num1, 0, 0, "", "", "9999-12-31", "", "", "", (int)dataTable1.Rows[index]["ANNDEN"], (int)dataTable1.Rows[index]["MESDEN"], (int)dataTable1.Rows[index]["PRODEN"], "", "ANNULLAMENTO", dataTable1.Rows[index]["TIPMOV"].ToString().Trim(), "");
                    idocRdl.WRITE_IDOC_TESTATA(this.objDataAccess, idocDatiE1Pitype2.Rows[0]);
                    idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "9004", idocDatiE1Pitype2, false);
                    string str7 = "UPDATE DENDET SET PRORAP = " + num2.ToString();
                    string str8 = dataTable1.Rows[index]["MESDEN"].ToString().Trim().PadLeft(2, '0');
                    dateTime = Convert.ToDateTime(rdl.datiContrattuali.datisc);
                    string str9 = dateTime.Month.ToString().Trim().PadLeft(2, '0');
                    string str10;
                    if (str8 == str9)
                        str10 = str7 + ", DAL = " + DBMethods.Db2Date(rdl.datiContrattuali.datisc) + ", AL = '" + dataTable1.Rows[index]["ANNDEN"]?.ToString() + "-" + dataTable1.Rows[index]["MESDEN"]?.ToString() + "-" + DateTime.DaysInMonth((int)dataTable1.Rows[index]["ANNDEN"], (int)dataTable1.Rows[index]["MESDEN"]).ToString() + "'";
                    else
                        str10 = str7 + ", DAL = '" + dataTable1.Rows[index]["ANNDEN"]?.ToString() + "-" + dataTable1.Rows[index]["MESDEN"]?.ToString() + "-01'";
                    this.objDataAccess.WriteTransactionData(str10 + " WHERE CODPOS=" + codpos + " AND ANNDEN=" + dataTable1.Rows[index]["ANNDEN"]?.ToString() + " AND MESDEN=" + dataTable1.Rows[index]["MESDEN"]?.ToString() + " AND PRODEN=" + dataTable1.Rows[index]["PRODEN"]?.ToString() + " AND PRODENDET=" + dataTable1.Rows[index]["PRODENDET"]?.ToString() + " AND MAT=" + num1.ToString() + " AND PRORAP = " + dataTable1.Rows[index]["PRORAP"]?.ToString(), CommandType.Text);
                    this.objDataAccess.WriteTransactionData(" DELETE FROM MODPREDET" + " WHERE CODPOS=" + codpos + " AND ANNDEN=" + dataTable1.Rows[index]["ANNDEN"]?.ToString() + " AND MESDEN=" + dataTable1.Rows[index]["MESDEN"]?.ToString() + " AND PRODEN=" + dataTable1.Rows[index]["PRODEN"]?.ToString() + " AND PRODENDET=" + dataTable1.Rows[index]["PRODENDET"]?.ToString() + " AND MAT=" + num1.ToString() + " AND PRORAP = " + dataTable1.Rows[index]["PRORAP"]?.ToString(), CommandType.Text);
                    idocDatiE1Pitype2.Clear();
                    idocDatiE1Pitype1 = idocRdl.GET_IDOC_DATI_E1PITYPE(this.objDataAccess, "9004", Convert.ToInt32(codpos), num1, 0, 0, "", "", "9999-12-31", "", "", "", (int)dataTable1.Rows[index]["ANNDEN"], (int)dataTable1.Rows[index]["MESDEN"], (int)dataTable1.Rows[index]["PRODEN"], "", "", dataTable1.Rows[index]["TIPMOV"].ToString().Trim(), "");
                    idocRdl.WRITE_IDOC_TESTATA(this.objDataAccess, idocDatiE1Pitype1.Rows[0]);
                    idocRdl.WRITE_IDOC_E1PITYP(this.objDataAccess, "9004", idocDatiE1Pitype1, false);
                }
                if (dataTable1.Rows.Count == 0)
                {
                    string[] strArray21 = new string[5]
                    {
            "SELECT CODPOS, ANNDEN, MESDEN, PRODEN FROM DENTES " + " WHERE CODPOS = " + codpos + " AND TIPMOV IN ('DP','NU') " + " AND NUMMOV IS NOT NULL AND NUMMOVANN IS NULL " + " AND TRIM(CHAR(ANNDEN)) || ",
            " RIGHT('00' || TRIM(CHAR(MESDEN)), 2)  >= '",
            null,
            null,
            null
                    };
                    dateTime = Convert.ToDateTime(rdl.datiContrattuali.datisc.Replace("'", ""));
                    strArray21[2] = dateTime.Year.ToString().Trim();
                    dateTime = Convert.ToDateTime(rdl.datiContrattuali.datisc.Replace("'", ""));
                    strArray21[3] = dateTime.Month.ToString().Trim().PadLeft(2, '0');
                    strArray21[4] = "'";
                    string strSQL = string.Concat(strArray21) + " ORDER BY ANNDEN, MESDEN ";
                    dataTable1.Clear();
                    DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL);
                    if (dataTable2.Rows.Count > 0)
                    {
                        string TIPPRI = this.objDataAccess.Get1ValueFromSQL("SELECT PRIORITA FROM TIPPRIRET WHERE CODTIPRET = 2", CommandType.Text);
                        dtAziende.Columns.Clear();
                        dtAziende.Columns.Add(new DataColumn("CODPOS", Type.GetType("System.Int32")));
                        dtAziende.Columns.Add(new DataColumn("ANNO", Type.GetType("System.Int32")));
                        dtAziende.Columns.Add(new DataColumn("MESE", Type.GetType("System.Int32")));
                        dtAziende.Columns.Add(new DataColumn("DATSCA", Type.GetType("System.String")));
                        dtAziende.Columns.Add(new DataColumn("TIPISC", Type.GetType("System.String")));
                        dtAziende.Columns.Add(new DataColumn("RIMUOVI", Type.GetType("System.String")));
                        for (int index = 0; index <= dataTable2.Rows.Count - 1; ++index)
                        {
                            if (Convert.ToInt32(dataTable2.Rows[index]["ANNDEN"]) >= 2003)
                            {
                                string TIPISC = Convert.ToString(this.objDataAccess.Get1ValueFromSQL("SELECT TIPISC FROM AZISTO WHERE CODPOS = " + dataTable2.Rows[index]["CODPOS"]?.ToString() + " AND '" + dataTable2.Rows[index]["ANNDEN"]?.ToString() + "-" + dataTable2.Rows[index]["MESDEN"].ToString().Trim().PadLeft(2, '0') + "-01' BETWEEN DATINI AND DATFIN", CommandType.Text)).Trim();
                                dateTime = this.Utils.Module_GetDataSistema();
                                string DATA_ORA_SISTEMA = dateTime.ToString();
                                string strDataDecorrenza = "01/" + dataTable2.Rows[index]["MESDEN"].ToString().Trim().PadLeft(2, '0') + "/" + dataTable2.Rows[index]["ANNDEN"]?.ToString();
                                dateTime = Convert.ToDateTime(clsWriteDb.Module_GetValorePargen(this.objDataAccess, 3, strDataDecorrenza));
                                dateTime = dateTime.AddDays(1.0);
                                string DATINISAN = dateTime.ToString();
                                string DATFINSAN = DATA_ORA_SISTEMA;
                                if (Convert.ToDateTime(DATINISAN) > Convert.ToDateTime(DATFINSAN))
                                    DATFINSAN = DATINISAN;
                                dtAziende.Clear();
                                DataRow row = dtAziende.NewRow();
                                row["CODPOS"] = dataTable2.Rows[index]["CODPOS"];
                                row["ANNO"] = dataTable2.Rows[index]["ANNDEN"];
                                row["MESE"] = dataTable2.Rows[index]["MESDEN"];
                                row["TIPISC"] = (object)"";
                                row["DATSCA"] = (object)"";
                                row["RIMUOVI"] = (object)"";
                                dtAziende.Rows.Add(row);
                                dtNot.Clear();
                                dtNot = modGeneraNotifiche.Module_Genera_Notifiche(this.objDataAccess, u, ref dtAziende, ref dtLOG, TIPMOVSAN, "", OPTIONAL_MATRICOLA: num1, OPTIONAL_PRORAP: num2);
                                if (dtNot.Rows.Count > 0)
                                    generaRettifiche.Module_Rettifiche_01(this.objDataAccess, u, ref dtNot, (int)dataTable2.Rows[index]["CODPOS"], (int)dataTable2.Rows[index]["ANNDEN"], (int)dataTable2.Rows[index]["MESDEN"], (int)dataTable2.Rows[index]["PRODEN"], num1, TIPMOVSAN, DATA_ORA_SISTEMA, TIPPRI, TIPISC, DATINISAN, DATFINSAN, "X");
                                str3 = "";
                                str4 = "";
                                str5 = "";
                                str6 = "";
                            }
                        }
                    }
                }
                idocRdl.Aggiorna_IDOC(this.objDataAccess);
                modGetDati.Module_AggiornaRaplav(this.objDataAccess, Convert.ToInt32(codpos), num1, num2);
                this.objDataAccess.EndTransaction(true);
                SuccessMSG = "Operazione completata con successo";
            }
            catch (Exception ex)
            {
                ErroreMsg = "Errore durante l'applicazione";
                this.objDataAccess.EndTransaction(false);
            }
        }

        public void Module_AggiornaStordl(
          DataLayer objDataAccess,
          TFI.OCM.Utente.Utente u,
          int CODPOS,
          int MAT,
          int PRORAP,
          string DATINI = "")
        {
            DataTable dataTable1 = new DataTable();
            DataTable dataTable2 = new DataTable();
            string strData1 = "";
            string strData2 = "";
            string strData3 = "";
            string str1 = "";
            string str2 = "";
            string str3 = "";
            Decimal num1 = 0.0M;
            Decimal num2 = 0.0M;
            Decimal num3 = 0.0M;
            string str4 = "";
            int num4 = 0;
            string str5 = "";
            string str6 = "";
            DataTable dataTable3 = new DataTable();
            int num5 = 0;
            string strDataNascita = objDataAccess.Get1ValueFromSQL("SELECT DATNAS FROM ISCT WHERE MAT = " + MAT.ToString(), CommandType.Text);
            string str7 = "SELECT * FROM STORDL WHERE" + " CODPOS = '" + CODPOS.ToString() + "' " + " AND MAT = '" + MAT.ToString() + "' AND PRORAP = '" + PRORAP.ToString() + "' ";
            string strSQL1 = !(DATINI != "") ? str7 + " ORDER BY DATINI ASC " : str7 + " AND DATINI = '" + DBMethods.Db2Date(DATINI.Replace("'", "")) + "' ";
            DataTable dataTable4 = objDataAccess.GetDataTable(strSQL1);
            for (int index1 = 0; index1 <= dataTable4.Rows.Count - 1; ++index1)
            {
                if (DATINI != "")
                {
                    string strSQL2 = "SELECT DTTFR, DTFP, DTINF FROM STORDL WHERE" + " CODPOS = '" + CODPOS.ToString() + "' " + " AND MAT = '" + MAT.ToString() + "' AND PRORAP = '" + PRORAP.ToString() + "' " + " AND DATINI < '" + DBMethods.Db2Date(DATINI.Replace("'", "")) + "' " + " ORDER BY DATINI DESC";
                    dataTable2.Clear();
                    dataTable2 = objDataAccess.GetDataTable(strSQL2);
                    if (dataTable2.Rows.Count > 0)
                    {
                        str1 = string.IsNullOrEmpty(dataTable2.Rows[0]["DTTFR"].ToString()) ? "" : dataTable2.Rows[0]["DTTFR"].ToString();
                        str2 = string.IsNullOrEmpty(dataTable2.Rows[0]["DTINF"].ToString()) ? "" : dataTable2.Rows[0]["DTINF"].ToString();
                        str3 = string.IsNullOrEmpty(dataTable2.Rows[0]["DTFP"].ToString()) ? "" : dataTable2.Rows[0]["DTFP"].ToString();
                    }
                }
                int num6 = Convert.ToInt32(dataTable4.Rows[index1]["CODCON"]);
                int num7 = !string.IsNullOrEmpty(dataTable4.Rows[index1]["CODLOC"].ToString()) ? Convert.ToInt32(dataTable4.Rows[index1]["CODLOC"]) : 0;
                Decimal num8 = Convert.ToDecimal(dataTable4.Rows[index1]["CODGRUASS"]);
                string str8 = dataTable4.Rows[index1]["FAP"].ToString().Trim();
                DataTable dataTable5;
                int num9;
                int num10;
                if (num7 == 0)
                {
                    string strSQL3 = "SELECT CODQUACON, PROCON FROM CONRIF " + " WHERE CODCON = " + num6.ToString() + " AND DATDEC <= '" + DBMethods.Db2Date(dataTable4.Rows[index1][nameof(DATINI)].ToString()) + "' " + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
                    dataTable2.Clear();
                    dataTable5 = objDataAccess.GetDataTable(strSQL3);
                    if (dataTable5.Rows.Count > 0)
                    {
                        num9 = Convert.ToInt32(dataTable5.Rows[0]["CODQUACON"]);
                        num10 = Convert.ToInt32(dataTable5.Rows[0]["PROCON"]);
                    }
                    else
                    {
                        num9 = 0;
                        num10 = 0;
                    }
                }
                else
                {
                    string strSQL4 = "SELECT PROCON FROM CONRIF WHERE CODCON = " + num6.ToString() + " AND DATDEC <= '" + DBMethods.Db2Date(dataTable4.Rows[index1][nameof(DATINI)].ToString()) + "' " + " ORDER BY DATINI DESC FETCH FIRST ROWS ONLY";
                    dataTable2.Clear();
                    dataTable5 = objDataAccess.GetDataTable(strSQL4);
                    num10 = dataTable5.Rows.Count <= 0 ? 0 : Convert.ToInt32(dataTable5.Rows[0]["PROCON"]);
                    string strSQL5 = "SELECT CODQUACON FROM QUACON WHERE CODQUACON IN (SELECT CODQUACON FROM CONRIF WHERE CODCON = " + num6.ToString() + ")";
                    num9 = Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL5, CommandType.Text));
                }
                string str9 = "SELECT VALUE(SUM(ALIQUOTA), 0.00) AS ALIQUOTA " + " FROM ALIFORASS " + " WHERE ALIFORASS.CODGRUASS = " + dataTable4.Rows[index1]["CODGRUASS"]?.ToString() + " AND ALIFORASS.CODQUACON=" + num9.ToString() + " AND '" + DBMethods.Db2Date(dataTable4.Rows[index1][nameof(DATINI)].ToString()) + "' BETWEEN ALIFORASS.DATINI AND VALUE(ALIFORASS.DATFIN,'9999-12-31') ";
                if (this.Utils.Module_Check_65Anni(Convert.ToDateTime(dataTable4.Rows[index1][nameof(DATINI)]), strDataNascita))
                    str9 += " AND ALIFORASS.CODFORASS IN (SELECT CODFORASS FROM FORASS WHERE CATFORASS <> 'PREV') ";
                string strSQL6 = str9 + " AND ALIFORASS.CODFORASS NOT IN (SELECT CODFORASS FROM FORASS WHERE CATFORASS = 'FAP') ";
                dataTable3.Clear();
                dataTable3 = objDataAccess.GetDataTable(strSQL6);
                dataTable4.Rows[index1]["ALIQUOTA"] = dataTable3.Rows[0]["ALIQUOTA"];
                Decimal num11 = (Decimal)dataTable4.Rows[index1]["ALIQUOTA"];
                Decimal num12;
                if (str8 == "S")
                {
                    string strSQL7 = "SELECT VALFAP FROM CODFAP WHERE " + DBMethods.Db2Date(dataTable4.Rows[index1][nameof(DATINI)].ToString()) + " BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31')";
                    num12 = Convert.ToDecimal(objDataAccess.Get1ValueFromSQL(strSQL7, CommandType.Text));
                }
                else
                    num12 = 0M;
                string strSQL8 = "SELECT DENLIV FROM CONLIV WHERE CODCON = " + num6.ToString() + " AND PROCON = " + num10.ToString() + " AND CODLIV = " + dataTable4.Rows[index1]["CODLIV"]?.ToString();
                string str10 = objDataAccess.Get1ValueFromSQL(strSQL8, CommandType.Text);
                string strSQL9 = " SELECT * FROM (SELECT (SELECT CATFORASS FROM FORASS WHERE CODFORASS = A.CODFORASS) AS CAT, A.ALIQUOTA " + " FROM ALIFORASS A WHERE A.CODGRUASS =  " + num8.ToString() + " AND A.CODQUACON = " + num9.ToString() + " ) AS TAB WHERE TAB.CAT <> 'FAP'";
                dataTable5.Clear();
                dataTable2 = objDataAccess.GetDataTable(strSQL9);
                for (int index2 = 0; index2 <= dataTable2.Rows.Count - 1; ++index2)
                {
                    string str11 = dataTable2.Rows[index2]["CAT"].ToString().Trim();
                    if (!(str11 == "TFR"))
                    {
                        if (!(str11 == "PREV"))
                        {
                            if (!(str11 == "INF"))
                                throw new Exception("Caso non gestito " + dataTable2.Rows[index2]["CAT"]?.ToString());
                            num2 = (Decimal)dataTable2.Rows[index2]["ALIQUOTA"];
                        }
                        else
                            num3 = (Decimal)dataTable2.Rows[index2]["ALIQUOTA"];
                    }
                    else
                        num1 = (Decimal)dataTable2.Rows[index2]["ALIQUOTA"];
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
                    string strSQL10 = " SELECT COUNT(*) FROM ALIFORASS A, FORASS B WHERE A.CODGRUASS =  " + num8.ToString() + " AND A.CODQUACON = " + num9.ToString() + " AND A.CODFORASS = B.CODFORASS AND B.CATFORASS = " + DBMethods.DoublePeakForSql(str6);
                    if (Convert.ToInt32(objDataAccess.Get1ValueFromSQL(strSQL10, CommandType.Text)) > 0)
                    {
                        switch (index3)
                        {
                            case 0:
                                strData1 = !((Decimal)num5 == num8) ? (!(str1 == "") ? str1 : dataTable4.Rows[index1][nameof(DATINI)].ToString()) : (!(str2 == "") ? str1 : dataTable4.Rows[index1][nameof(DATINI)].ToString());
                                continue;
                            case 1:
                                strData3 = !((Decimal)num5 == num8) ? (!(str3 == "") ? str3 : dataTable4.Rows[index1][nameof(DATINI)].ToString()) : (!(str3 == "") ? str2 : dataTable4.Rows[index1][nameof(DATINI)].ToString());
                                continue;
                            case 2:
                                strData2 = !((Decimal)num5 == num8) ? (!(str2 == "") ? str2 : dataTable4.Rows[index1][nameof(DATINI)].ToString()) : (!(str2 == "") ? str2 : dataTable4.Rows[index1][nameof(DATINI)].ToString());
                                continue;
                            default:
                                continue;
                        }
                    }
                }
                string str12 = " UPDATE STORDL SET ";
                string str13 = !(strData1.Trim() != "") ? str12 + " DTTFR = NULL, " + " ALIQTFR = NULL, " : str12 + " DTTFR  = '" + DBMethods.Db2Date(strData1) + "', " + " ALIQTFR  = " + num1.ToString().Replace(",", ".") + ", ";
                string str14 = !(strData2.Trim() != "") ? str13 + " DTINF  = NULL, " + " ALIQINF  = NULL, " : str13 + " DTINF  = '" + DBMethods.Db2Date(strData2) + "', " + " ALIQINF  = " + num2.ToString().Replace(",", ".") + ", ";
                string str15 = !(strData3.Trim() != "") ? str14 + " DTFP  = NULL, " + " ALIQFP  = NULL, " : str14 + " DTFP  = '" + DBMethods.Db2Date(strData3) + "', " + " ALIQFP  = " + num3.ToString().Replace(",", ".") + ", ";
                string str16 = (!(str8 == "S") ? str15 + " ALIFAP = NULL, " : str15 + " ALIFAP  = " + num12.ToString().Replace(",", ".") + ", ") + " ALIQUOTA  = " + num11.ToString().Replace(",", ".") + ", " + " CODQUACON  = " + num9.ToString() + ", ";
                string strSQL11 = (!string.IsNullOrEmpty(str10) ? str16 + " DENLIV  = " + DBMethods.DoublePeakForSql(str10.Trim()) : str16 + " DENLIV  = NULL ") + " WHERE CODPOS = " + CODPOS.ToString() + " AND MAT = " + MAT.ToString() + " AND PRORAP = " + PRORAP.ToString() + " AND DATINI = '" + DBMethods.Db2Date(dataTable4.Rows[index1][nameof(DATINI)].ToString()) + "' ";
                objDataAccess.WriteTransactionData(strSQL11, CommandType.Text);
                num5 = Convert.ToInt32(num8);
                str1 = strData1;
                str2 = strData2;
                str3 = strData3;
                strData1 = "";
                strData2 = "";
                strData3 = "";
                num1 = 0.0M;
                num2 = 0.0M;
                num3 = 0.0M;
                str4 = "";
                num12 = 0.0M;
                num8 = 0.0M;
                num11 = 0.0M;
                num9 = 0;
                num6 = 0;
                num4 = 0;
                str5 = "";
                str6 = "";
                num10 = 0;
            }
        }

        public Decimal Module_GetImportoScatti(
          string strData,
          int CODCON,
          Decimal PERAPP,
          Decimal PERPAR,
          int CODLIV)
        {
            Decimal importoScatti = 0M;
            try
            {
                importoScatti = Convert.ToDecimal(this.objDataAccess.Get1ValueFromSQL("SELECT IMPVOCRET FROM CONRET WHERE CODVOCRET=4" + " AND " + DBMethods.Db2Date(strData) + " BETWEEN DATAPPINI AND DATAPPFIN AND CODCON = " + CODCON.ToString() + " AND CODLIV =" + CODLIV.ToString(), CommandType.Text));
                if (PERAPP > 0M)
                    importoScatti = importoScatti * PERAPP / 100M;
                if (PERPAR > 0M)
                    importoScatti = importoScatti * PERPAR / 100M;
                return importoScatti;
            }
            catch (Exception ex)
            {
                return importoScatti;
            }
        }

        public void Module_Carica_Contratti(
          GestioneRapportiLavoroIscrittiOCM rdl,
          string strData,
          string strAzione,
          bool PrimaRigaVuota = false)
        {
            DataTable dataTable1 = new DataTable();
            int num1 = 0;
            DataTable dataTable2 = new DataTable();
            DataTable dataTable3 = new DataTable();
            try
            {
                string str1 = " SELECT " + " CODCON, PROCON, 0 AS CODLOC, 0 AS PROLOC," + " CODQUACON," + " (SELECT DENQUA FROM QUACON WHERE QUACON.CODQUACON=CONRIF.CODQUACON) AS DENQUA," + " DATINI,DATFIN, DATDEC, ASSCON,DENCON," + " MAXSCA,PERSCA,NUMMEN,M14,M15,M16,RIVAUT," + " TIPSPE," + " '' AS RIMUOVI" + " FROM CONRIF ";
                DataTable dataTable4 = this.objDataAccess.GetDataTable((!(strAzione == "MODIFICA") ? str1 + " WHERE DATDEC <= '" + DBMethods.Db2Date(strData) + "' AND DATANN IS NULL" : str1 + " WHERE DATDEC <= '" + DBMethods.Db2Date(strData) + "' " + " AND (DATANN > '" + DBMethods.Db2Date(strData) + "' OR DATANN IS NULL)") + " ORDER BY CODCON, DATDEC");
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
                string str2 = "SELECT " + " CODCON, CODLOC, PROLOC, " + " DATINI,DATFIN,DATDEC, ASSCON,DENCON, " + " MAXSCA,PERSCA,NUMMEN,M14,M15,M16,RIVAUT, " + " TIPSPE, " + " '' AS RIMUOVI " + " FROM CONLOC ";
                string strSQL = (!(strAzione == "MODIFICA") ? str2 + " WHERE DATDEC <= '" + DBMethods.Db2Date(strData) + "' AND DATANN IS NULL" : str2 + " WHERE DATDEC <= '" + DBMethods.Db2Date(strData) + "' " + " AND (DATANN > '" + DBMethods.Db2Date(strData) + "' OR DATANN IS NULL)") + " ORDER BY CODLOC, DATDEC ";
                int num2 = 0;
                DataTable dataTable5 = this.objDataAccess.GetDataTable(strSQL);
                for (int index = dataTable5.Rows.Count - 1; index >= 0; index += -1)
                {
                    if (num2 != Convert.ToInt32(dataTable5.Rows[index]["CODLOC"]))
                    {
                        num2 = Convert.ToInt32(dataTable5.Rows[index]["CODLOC"]);
                        dataTable5.Rows[index]["RIMUOVI"] = (object)"NO";
                    }
                    else
                        dataTable5.Rows[index]["RIMUOVI"] = (object)"SI";
                }
                for (int index = dataTable5.Rows.Count - 1; index >= 0; index += -1)
                {
                    if (dataTable5.Rows[index]["RIMUOVI"].ToString() == "SI")
                        dataTable5.Rows.Remove(dataTable5.Rows[index]);
                }
                for (int index1 = 0; index1 <= dataTable5.Rows.Count - 1; ++index1)
                {
                    DataRow row = dataTable4.NewRow();
                    row["CODCON"] = dataTable5.Rows[index1]["CODCON"];
                    int index2;
                    for (index2 = 0; index2 <= dataTable4.Rows.Count - 1; ++index2)
                    {
                        if (dataTable4.Rows[index2]["CODCON"] == dataTable5.Rows[index1]["CODCON"])
                        {
                            row["PROCON"] = dataTable4.Rows[index2]["PROCON"];
                            row["CODQUACON"] = dataTable4.Rows[index2]["CODQUACON"];
                            row["DENQUA"] = dataTable4.Rows[index2]["DENQUA"];
                            break;
                        }
                    }
                    if (index2 == dataTable4.Rows.Count)
                    {
                        DataTable dataTable6 = this.objDataAccess.GetDataTable("SELECT PROCON, CODQUACON, (SELECT DENQUA FROM QUACON WHERE QUACON.CODQUACON=CONRIF.CODQUACON) AS DENQUA FROM CONRIF WHERE CODCON = '" + dataTable5.Rows[index1]["CODCON"]?.ToString() + "' " + " AND PROCON = (SELECT MAX(PROCON) FROM CONRIF WHERE CODCON = '" + dataTable5.Rows[index1]["CODCON"]?.ToString() + "')");
                        if (dataTable6.Rows.Count > 0)
                        {
                            row["PROCON"] = dataTable6.Rows[0]["PROCON"];
                            row["CODQUACON"] = dataTable6.Rows[0]["CODQUACON"];
                            row["DENQUA"] = dataTable6.Rows[0]["DENQUA"];
                        }
                    }
                    row["CODLOC"] = dataTable5.Rows[index1]["CODLOC"];
                    row["PROLOC"] = dataTable5.Rows[index1]["PROLOC"];
                    row["DATDEC"] = dataTable5.Rows[index1]["DATDEC"];
                    row["DATINI"] = dataTable5.Rows[index1]["DATINI"];
                    row["DATFIN"] = dataTable5.Rows[index1]["DATFIN"];
                    row["ASSCON"] = dataTable5.Rows[index1]["ASSCON"];
                    row["DENCON"] = dataTable5.Rows[index1]["DENCON"];
                    row["MAXSCA"] = dataTable5.Rows[index1]["MAXSCA"];
                    row["PERSCA"] = dataTable5.Rows[index1]["PERSCA"];
                    row["NUMMEN"] = dataTable5.Rows[index1]["NUMMEN"];
                    row["M14"] = dataTable5.Rows[index1]["M14"];
                    row["M15"] = dataTable5.Rows[index1]["M15"];
                    row["M16"] = dataTable5.Rows[index1]["M16"];
                    row["RIVAUT"] = dataTable5.Rows[index1]["RIVAUT"];
                    row["TIPSPE"] = dataTable5.Rows[index1]["TIPSPE"];
                    row["RIMUOVI"] = dataTable5.Rows[index1]["RIMUOVI"];
                    dataTable4.Rows.Add(row);
                    DataView defaultView = dataTable4.DefaultView;
                    HttpContext.Current.Session["ContrattiView"] = (object)defaultView;
                    List<GestioneRapportiLavoroIscrittiOCM.Contratti> contrattiList = new List<GestioneRapportiLavoroIscrittiOCM.Contratti>();
                    foreach (DataRowView dataRowView in defaultView)
                    {
                        GestioneRapportiLavoroIscrittiOCM.Contratti contratti = new GestioneRapportiLavoroIscrittiOCM.Contratti()
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
                            RIMUOVI = dataRowView["RIMUOVI"].ToString()
                        };
                        contrattiList.Add(contratti);
                    }
                    rdl.listContratti = contrattiList;
                }
            }
            catch (Exception ex)
            {
                rdl.listContratti = (List<GestioneRapportiLavoroIscrittiOCM.Contratti>)null;
            }
        }

        public void ImportoScatti(GestioneRapportiLavoroIscrittiOCM rdl)
        {
            Decimal num1 = 0M;
            Decimal PERAPP = 0M;
            Decimal PERPAR = 0M;
            if (Convert.ToDecimal(rdl.datiRetributivi.NumScatt) != 0M & !string.IsNullOrEmpty(rdl.datiContrattuali.tiprapCod))
            {
                switch (Convert.ToInt16(rdl.datiContrattuali.tiprapCod))
                {
                    case 3:
                    case 4:
                    case 13:
                    case 16:
                        if (rdl.datiContrattuali.PerPar != "")
                        {
                            PERPAR = Convert.ToDecimal(rdl.datiContrattuali.PerPar);
                            break;
                        }
                        break;
                    case 6:
                    case 7:
                        if (rdl.datiContrattuali.PerApp != "")
                        {
                            PERAPP = Convert.ToDecimal(rdl.datiContrattuali.PerApp);
                            break;
                        }
                        break;
                    case 8:
                    case 10:
                        if (rdl.datiContrattuali.PerPar != "")
                            PERPAR = Convert.ToDecimal(rdl.datiContrattuali.PerPar);
                        if (rdl.datiContrattuali.PerApp != "")
                        {
                            PERAPP = Convert.ToDecimal(rdl.datiContrattuali.PerApp);
                            break;
                        }
                        break;
                    case 11:
                        if (rdl.datiContrattuali.PerPar != "")
                        {
                            PERPAR = Convert.ToDecimal(rdl.datiContrattuali.PerPar);
                            break;
                        }
                        break;
                }
                num1 = this.Module_GetImportoScatti("", Convert.ToInt32(rdl.datiContrattuali.CODCON), PERAPP, PERPAR, Convert.ToInt32(rdl.datiContrattuali.livello));
            }
            Decimal num2 = Convert.ToDecimal(rdl.datiRetributivi.importoSc);
            num2 = Convert.ToDecimal(num1) * Convert.ToDecimal(rdl.datiRetributivi.NumScatt);
            this.Module_Totale_Retribuzione(rdl);
        }

        public void Module_Totale_Retribuzione(GestioneRapportiLavoroIscrittiOCM rdl)
        {
            if (Convert.ToInt32(rdl.datiContrattuali.ContRif) > -1)
            {
                string tipspe = rdl.datiContrattuali.TIPSPE;
                if (!(tipspe == "S"))
                {
                    if (!(tipspe == "M"))
                    {
                        if (!(tipspe == "A"))
                            return;
                        Decimal num = Convert.ToDecimal(rdl.datiRetributivi.emolumenti) + Convert.ToDecimal(rdl.datiRetributivi.importoSc) + Convert.ToDecimal(rdl.datiRetributivi.S12) + Convert.ToDecimal(rdl.datiRetributivi.S13) + Convert.ToDecimal(rdl.datiRetributivi.S14) + Convert.ToDecimal(rdl.datiRetributivi.S15) + Convert.ToDecimal(rdl.datiRetributivi.S16);
                        if (!(Convert.ToDecimal(rdl.datiRetributivi.RetDichiarata) == 0M))
                            return;
                        num = 0.00M;
                    }
                    else
                    {
                        Decimal num = Convert.ToDecimal(rdl.datiRetributivi.emolumenti) + Convert.ToDecimal(rdl.datiRetributivi.importoSc) + Convert.ToDecimal(rdl.datiRetributivi.S12) + Convert.ToDecimal(rdl.datiRetributivi.S13) + Convert.ToDecimal(rdl.datiRetributivi.S14) + Convert.ToDecimal(rdl.datiRetributivi.S15) + Convert.ToDecimal(rdl.datiRetributivi.S16);
                        if (!(Convert.ToDecimal(rdl.datiRetributivi.RetDichiarata) == 0M))
                            return;
                        num = 0.00M;
                    }
                }
                else
                {
                    Decimal num = Convert.ToDecimal(rdl.datiRetributivi.emolumenti) + Convert.ToDecimal(rdl.datiRetributivi.importoSc) + Convert.ToDecimal(rdl.datiRetributivi.S12) + Convert.ToDecimal(rdl.datiRetributivi.S13) + Convert.ToDecimal(rdl.datiRetributivi.S14) + Convert.ToDecimal(rdl.datiRetributivi.S15) + Convert.ToDecimal(rdl.datiRetributivi.S16);
                    if (!(Convert.ToDecimal(rdl.datiRetributivi.emolumenti) == 0M))
                        return;
                    num = 0.00M;
                }
            }
            else
            {
                Decimal num = Convert.ToDecimal(rdl.datiRetributivi.emolumenti) + Convert.ToDecimal(rdl.datiRetributivi.importoSc) + Convert.ToDecimal(rdl.datiRetributivi.S12) + Convert.ToDecimal(rdl.datiRetributivi.S13) + Convert.ToDecimal(rdl.datiRetributivi.S14) + Convert.ToDecimal(rdl.datiRetributivi.S15) + Convert.ToDecimal(rdl.datiRetributivi.S16);
                if (!(Convert.ToDecimal(rdl.datiRetributivi.emolumenti) == 0M))
                    return;
                num = 0.00M;
            }
        }

        public GestioneRapportiLavoroIscrittiOCM Carica_AziendaSomministrazione(
          GestioneRapportiLavoroIscrittiOCM rdl,
          string codposAz,
          ref string ErroreMSG)
        {
            DataTable dataTable1 = new DataTable();
            try
            {
                DataTable dataTable2 = this.objDataAccess.GetDataTable(" SELECT AZI.RAGSOC,INDSED.TEL1,AZI.CODFIS,AZI.PARIVA, " +
                                                                       " DUG.CODDUG, DUG.DENDUG, INDSED.IND, INDSED.NUMCIV, " +
                                                                       " INDSED.CAP, INDSED.DENLOC, INDSED.SIGPRO, INDSED.CODCOM, INDSED.DENCOM,INDSED.EMAIL,INDSED.EMAILCERT " +
                                                                       " FROM AZI LEFT JOIN INDSED ON AZI.CODPOS = INDSED.CODPOS " +
                                                                       " LEFT JOIN DUG ON INDSED.CODDUG =  DUG.CODDUG " +
                                                                       " WHERE AZI.CODPOS = " + codposAz + " AND INDSED.TIPIND = 1 " +
                                                                       " AND DATINI <= CURRENT_DATE" + " ORDER BY INDSED.DATCOM DESC FETCH FIRST 1 ROWS ONLY");
                if (dataTable2.Rows.Count == 0)
                    return (GestioneRapportiLavoroIscrittiOCM)null;
                rdl.aziendaUtilizzatrice = new GestioneRapportiLavoroIscrittiOCM.AziendaUtilizzatrice()
                {
                    ragsocAz = dataTable2.Rows[0]["RAGSOC"].ToString(),
                    codfisAz = dataTable2.Rows[0]["CODFIS"].ToString(),
                    parivaAz = dataTable2.Rows[0]["PARIVA"].ToString(),
                    localitaAz = dataTable2.Rows[0]["DENLOC"].ToString(),
                    dugAz = dataTable2.Rows[0]["DENDUG"].ToString(),
                    indirizzoAz = dataTable2.Rows[0]["IND"].ToString(),
                    civicoAz = dataTable2.Rows[0]["NUMCIV"].ToString(),
                    provAz = dataTable2.Rows[0]["SIGPRO"].ToString(),
                    capAz = dataTable2.Rows[0]["CAP"].ToString(),
                    telefonoAz = dataTable2.Rows[0]["TEL1"].ToString(),
                    comuneAz = dataTable2.Rows[0]["DENCOM"].ToString(),
                    comuneCodAz = dataTable2.Rows[0]["CODCOM"].ToString(),
                    emailAz = dataTable2.Rows[0]["EMAIL"].ToString(),
                    pecAz = dataTable2.Rows[0]["EMAILCERT"].ToString()
                };
                if (string.IsNullOrEmpty(rdl.aziendaUtilizzatrice.comuneAz))
                {
                    string strSQL = "SELECT DENCOM FROM CODCOM WHERE CODCOM='" + rdl.aziendaUtilizzatrice.comuneCodAz + "'";
                    rdl.aziendaUtilizzatrice.comuneAz = this.objDataAccess.Get1ValueFromSQL(strSQL, CommandType.Text).ToString().Trim();
                }
                return rdl;
            }
            catch (Exception ex)
            {
                return rdl;
            }
        }

        public GestioneRapportiLavoroIscrittiOCM Carica_AziendaSomministrazioneConPiva(
          GestioneRapportiLavoroIscrittiOCM rdl,
          string piva,
          ref string ErroreMSG)
        {
            try
            {
                var pivaParam = objDataAccess.CreateParameter("@piva", iDB2DbType.iDB2VarChar, 11, ParameterDirection.Input, piva);
                DataTable aziInfo = this.objDataAccess.GetDataTableWithParameters(" SELECT AZI.RAGSOC, AZI.CODPOS, INDSED.TEL1, AZI.CODFIS, " +
                                                                       "DUG.CODDUG, DUG.DENDUG, INDSED.IND, INDSED.NUMCIV, " +
                                                                       "INDSED.CAP, INDSED.DENLOC, INDSED.SIGPRO, INDSED.CODCOM, INDSED.DENCOM, AZEMAIL.EMAIL, AZEMAIL.EMAILCERT " +
                                                                       "FROM AZI INNER JOIN AZEMAIL ON AZEMAIL.CODPOS = AZI.CODPOS AND " +
                                                                       "AZEMAIL.DATINI = (SELECT MAX(DATINI) FROM AZEMAIL WHERE CODPOS = AZI.CODPOS) " +
                                                                       "LEFT JOIN INDSED ON AZI.CODPOS = INDSED.CODPOS " +
                                                                       "LEFT JOIN DUG ON INDSED.CODDUG =  DUG.CODDUG " +
                                                                       "WHERE AZI.PARIVA = @piva AND INDSED.TIPIND = 1 " +
                                                                       "AND INDSED.DATINI <= CURRENT_DATE ORDER BY INDSED.DATCOM DESC FETCH FIRST 1 ROWS ONLY", pivaParam);
                if (aziInfo.Rows.Count == 0)
                    return null;

                rdl.aziendaUtilizzatrice = new GestioneRapportiLavoroIscrittiOCM.AziendaUtilizzatrice()
                {
                    ragsocAz = aziInfo.Rows[0]["RAGSOC"].ToString(),
                    codfisAz = aziInfo.Rows[0]["CODFIS"].ToString(),
                    codposAz = aziInfo.Rows[0]["CODPOS"].ToString(),
                    localitaAz = aziInfo.Rows[0]["DENLOC"].ToString(),
                    dugAz = aziInfo.Rows[0]["DENDUG"].ToString(),
                    dugCodAz = aziInfo.Rows[0]["CODDUG"].ToString(),
                    indirizzoAz = aziInfo.Rows[0]["IND"].ToString(),
                    civicoAz = aziInfo.Rows[0]["NUMCIV"].ToString(),
                    provAz = aziInfo.Rows[0]["SIGPRO"].ToString(),
                    capAz = aziInfo.Rows[0]["CAP"].ToString(),
                    telefonoAz = aziInfo.Rows[0]["TEL1"].ToString(),
                    comuneAz = aziInfo.Rows[0]["DENCOM"].ToString(),
                    comuneCodAz = aziInfo.Rows[0]["CODCOM"].ToString(),
                    emailAz = aziInfo.Rows[0]["EMAIL"].ToString(),
                    pecAz = aziInfo.Rows[0]["EMAILCERT"].ToString()
                };

                if (string.IsNullOrEmpty(rdl.aziendaUtilizzatrice.comuneAz))
                {
                    string strSQL = "SELECT DENCOM FROM CODCOM WHERE CODCOM='" + rdl.aziendaUtilizzatrice.comuneCodAz + "'";
                    rdl.aziendaUtilizzatrice.comuneAz = objDataAccess.Get1ValueFromSQL(strSQL, CommandType.Text).Trim();
                }
                return rdl;
            }
            catch
            {
                return rdl;
            }
        }

        public void Carica_Livelli(GestioneRapportiLavoroIscrittiOCM rdl, string codcon, string dencon)
        {
            DataTable dataTable = new DataTable();
            try
            {
                string str = Convert.ToString(((DataView)HttpContext.Current.Session["ContrattiView"]).ToTable().Select("DENCON like '%" + dencon.Trim().Replace("'", "'''+'") + "%'")[0]["PROCON"]);
                DataView defaultView = this.objDataAccess.GetDataTable("SELECT CODLIV, DENLIV FROM CONLIV WHERE CODCON =" + codcon + " AND PROCON = " + str).DefaultView;
                List<GestioneRapportiLavoroIscrittiOCM.Livello> livelloList = new List<GestioneRapportiLavoroIscrittiOCM.Livello>();
                foreach (DataRowView dataRowView in defaultView)
                {
                    GestioneRapportiLavoroIscrittiOCM.Livello livello = new GestioneRapportiLavoroIscrittiOCM.Livello()
                    {
                        CODLIV = dataRowView["CODLIV"].ToString(),
                        DENLIV = dataRowView["DENLIV"].ToString()
                    };
                    livelloList.Add(livello);
                }
                rdl.listLivello = livelloList;
            }
            catch (Exception ex)
            {
            }
        }

        public void Carica_Mensilità(GestioneRapportiLavoroIscrittiOCM rdl, string dencon)
        {
            DataTable dataTable = new DataTable();
            try
            {
                DataRow[] dataRowArray = ((DataView)HttpContext.Current.Session["ContrattiView"]).ToTable().Select("DENCON like '%" + dencon.Trim().Replace("'", "'''+'") + "%'");
                Convert.ToString(dataRowArray[0]["PROCON"]);
                rdl.datiContrattuali.qualifica = Convert.ToString(dataRowArray[0]["DENQUA"]);
                rdl.datiContrattuali.mensilita = Convert.ToString(dataRowArray[0]["NUMMEN"]);
                rdl.datiContrattuali.mens14 = !string.IsNullOrEmpty(Convert.ToString(dataRowArray[0]["M14"])) ? Convert.ToString(dataRowArray[0]["M14"]) : "0";
                rdl.datiContrattuali.mens15 = !string.IsNullOrEmpty(Convert.ToString(dataRowArray[0]["M15"])) ? Convert.ToString(dataRowArray[0]["M15"]) : "0";
                if (string.IsNullOrEmpty(Convert.ToString(dataRowArray[0]["M16"])))
                    rdl.datiContrattuali.mens16 = "0";
                else
                    rdl.datiContrattuali.mens16 = Convert.ToString(dataRowArray[0]["M16"]);
            }
            catch (Exception ex)
            {
            }
        }

        public void ModInsEredi(
          GestioneRapportiLavoroIscrittiOCM.Eredi erediOCM,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMSG,
          ref string SuccessMSG,
          string mat)
        {
            try
            {
                string strSQL1 = "SELECT CODCOM FROM CODCOM WHERE DENCOM = '" + erediOCM.comuneE.ToUpper() + "'";
                erediOCM.comuneCodE = this.objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text);
                string strSQL2 = "SELECT CODCOM FROM CODCOM WHERE DENCOM = '" + erediOCM.comuneNE.ToUpper() + "'";
                erediOCM.comuneCodNE = this.objDataAccess.Get1ValueFromSQL(strSQL2, CommandType.Text);
                this.objDataAccess.StartTransaction();
                if (erediOCM.proereE == "0")
                {
                    string strSQL3 = "SELECT VALUE(MAX(PROERE), 0) + 1 FROM EREDI WHERE MAT = '" + mat + "' ";
                    erediOCM.proereE = Convert.ToString(this.objDataAccess.Get1ValueFromSQL(strSQL3, CommandType.Text));
                    string str1 = "INSERT INTO EREDI(MAT, TIPPAR, PROERE, DATINI, DATFIN, COGNOME, NOME, SESSO, CODFISC, DATNAS, CODCOMNAS, SIGPRO, PERCTFR, PERCFP,  " + " MAGGIOR, PERCMAGG, CODDUG, IND, NUMCIV, DENLOC, SIGPRORES, STAESTRES, CODCOM, CAP, TEL, EMAIL, CO, ULTAGG, UTEAGG) " + " VALUES( " + "'" + mat + "', " + "'" + erediOCM.parentelaE + "', " + "'" + erediOCM.proereE + "', " + " '" + DBMethods.Db2Date(erediOCM.datiniE) + "', " + "'9999-12-31', " + "'" + erediOCM.cognomeE.ToUpper() + "', " + "'" + erediOCM.nomeE.ToUpper() + "', " + "'" + erediOCM.sessoE + "', " + "'" + erediOCM.codfisE + "', " + "'" + DBMethods.Db2Date(erediOCM.datnasE) + "', ";
                    string str2 = (string.IsNullOrEmpty(erediOCM.statoEsNE) ? str1 + "'" + erediOCM.comuneCodNE + "', " : str1 + "'" + erediOCM.statoEsNE + "', ") + "'" + erediOCM.provNE.ToUpper() + "', ";
                    string str3 = string.IsNullOrEmpty(erediOCM.perTFRE) ? str2 + " NULL," : str2 + "'" + erediOCM.perTFRE.Replace(",", ".") + "', ";
                    string str4 = string.IsNullOrEmpty(erediOCM.perFPE) ? str3 + " NULL," : str3 + "'" + erediOCM.perFPE.Replace(",", ".") + "', ";
                    string str5 = string.IsNullOrEmpty(erediOCM.maggE) ? str4 + " NULL," : str4 + "'" + erediOCM.maggE.Trim() + "', ";
                    string str6 = (string.IsNullOrEmpty(erediOCM.maggPerE) ? str5 + " NULL, " : str5 + "'" + erediOCM.maggPerE.Replace(",", ".") + "', ") + "'" + erediOCM.coddugE + "', " + "'" + erediOCM.indirizzoE + "', " + "'" + erediOCM.numcivE + "', " + "'" + erediOCM.localitaE + "', " + "'" + erediOCM.provE.ToUpper() + "', ";
                    string str7 = (string.IsNullOrEmpty(erediOCM.statoEs_E) ? str6 + " NULL, " + "'" + erediOCM.comuneCodE + "', " : str6 + "'" + erediOCM.statoEs_E + "', " + "'" + erediOCM.comuneCodE + "', ") + "'" + erediOCM.capE + "', " + "'" + erediOCM.telE + "', " + "'" + erediOCM.emailE + "', ";
                    this.objDataAccess.WriteTransactionData((string.IsNullOrEmpty(erediOCM.coE) ? str7 + "'NULL', " : str7 + "'" + erediOCM.coE + "', ") + "CURRENT_TIMESTAMP, " + "'" + u.Username + "') ", CommandType.Text);
                    if (!string.IsNullOrEmpty(erediOCM.IBANE))
                    {
                        string strSQL4 = "SELECT VALUE(MAX(PROBANC), 0) + 1 FROM EREDIBANC WHERE MAT = '" + mat + "' ";
                        erediOCM.probancE = this.objDataAccess.Get1ValueFromSQL(strSQL4, CommandType.Text);
                        string str8 = "INSERT INTO EREDIBANC(MAT, TIPPAR, PROERE, PROBANC, DATINI, DATFIN, MODPAG, PAESE, DIVISA, INTEST, ABI," + "CAB, CCERE, IBAN, ULTAGG, UTEAGG, KEYBANC, CODSWIFT, CODCIN) " + "VALUES( " + "'" + mat + "', " + "'" + erediOCM.parentelaE + "', " + "'" + erediOCM.proereE + "', " + "'" + erediOCM.probancE + "', " + "'" + DBMethods.Db2Date(erediOCM.datiniE) + "', " + "'9999-12-31', " + "'B', " + "'" + erediOCM.IBANE.Substring(0, 2) + "', " + "'EUR', " + "'" + erediOCM.cognomeE + " " + erediOCM.nomeE + "', " + "'" + erediOCM.IBANE.Substring(5, 5) + "', " + "'" + erediOCM.IBANE.Substring(10, 5) + "', " + "'" + erediOCM.IBANE.Substring(15) + "', " + "'" + erediOCM.IBANE + "', " + "CURRENT_TIMESTAMP, " + "'" + u.Username + "', " + "'" + erediOCM.IBANE.Substring(5, 10) + "', ";
                        this.objDataAccess.WriteTransactionData((string.IsNullOrEmpty(erediOCM.SWIFTE) ? str8 + " 'NULL', " : str8 + "'" + erediOCM.SWIFTE + "', ") + "'" + erediOCM.IBANE.Substring(4, 1) + "') ", CommandType.Text);
                    }
                }
                else
                {
                    string str9 = " UPDATE EREDI SET  " + " TIPPAR = '" + erediOCM.parentelaE + "'" + ", DATINI = '" + DBMethods.Db2Date(erediOCM.datiniE) + "'" + ", COGNOME = '" + erediOCM.cognomeE.ToUpper() + "'" + ", NOME = '" + erediOCM.nomeE.ToUpper() + "'" + ", SESSO = '" + erediOCM.sessoE + "'" + ", CODFISC = '" + erediOCM.codfisE + "'" + ", DATNAS = '" + DBMethods.Db2Date(erediOCM.datnasE) + "'";
                    string str10 = (!(erediOCM.statoEsNE != "") ? str9 + ", CODCOMNAS = '" + erediOCM.comuneCodNE + "'" : str9 + ", CODCOMNAS = '" + erediOCM.statoEsCodNE + "'") + ", SIGPRO = '" + erediOCM.provNE.ToUpper() + "'" + ", CODDUG = '" + erediOCM.coddugE + "'" + ", IND = '" + erediOCM.indirizzoE + "'" + ", NUMCIV = '" + erediOCM.numcivE + "'";
                    string str11 = (!(erediOCM.statoEs_E != "") ? str10 + " NULL, " + ", CODCOM = '" + erediOCM.comuneCodE + "'" : str10 + ", CODCOM = '" + erediOCM.statoEs_E + "'" + ", STAESTRES = '" + erediOCM.comuneCodE + "'") + ", CAP = '" + erediOCM.capE + "'" + ", DENLOC = '" + erediOCM.localitaE + "'" + ", SIGPRORES = '" + erediOCM.provE.ToUpper() + "'";
                    string str12 = !(erediOCM.perTFRE != "") ? str11 + ", PERCTFR = NULL " : str11 + ", PERCTFR = '" + erediOCM.perTFRE.Replace(",", ".") + "'";
                    string str13 = !(erediOCM.perFPE != "") ? str12 + ", PERCFP = NULL " : str12 + ", PERCFP = '" + erediOCM.perFPE.Replace(",", ".") + "'";
                    string str14 = !(erediOCM.maggPerE != "") ? str13 + ", PERCMAGG = NULL " : str13 + ", PERCMAGG = '" + erediOCM.maggPerE.Replace(",", ".") + "'";
                    this.objDataAccess.WriteTransactionData((!(erediOCM.maggE != "") ? str14 + ", MAGGIOR = NULL " : str14 + ", MAGGIOR = '" + erediOCM.maggE + "'") + ", EMAIL = '" + erediOCM.emailE + "'" + ", TEL = '" + erediOCM.telE + "'" + ", CO = '" + erediOCM.coE + "'" + ", ULTAGG = CURRENT_TIMESTAMP" + ", UTEAGG = '" + u.Username + "'" + " WHERE MAT = '" + mat + "'" + " AND PROERE = '" + erediOCM.proereE + "'", CommandType.Text);
                    if (erediOCM.IBANE != "")
                    {
                        if (erediOCM.probancE == "0")
                        {
                            string strSQL5 = "SELECT VALUE(MAX(PROBANC), 0) + 1 FROM EREDIBANC WHERE MAT = " + mat + "'";
                            erediOCM.probancE = this.objDataAccess.Get1ValueFromSQL(strSQL5, CommandType.Text);
                            this.objDataAccess.WriteTransactionData("INSERT INTO EREDIBANC(MAT, TIPPAR, PROERE, PROBANC, DATINI, DATFIN, MODPAG, PAESE, DIVISA, INTEST, ABI," + "CAB, CCERE, IBAN, ULTAGG, UTEAGG, KEYBANC, CODSWIFT, CODCIN) " + "VALUES( " + "'" + mat + "', " + "'" + erediOCM.parentelaE + "', " + "'" + erediOCM.proereE + "', " + "'" + erediOCM.probancE + "', " + "'" + DBMethods.Db2Date(erediOCM.datiniE) + "', " + "'9999-12-31', " + "'B', " + "'" + erediOCM.IBANE.Substring(0, 2) + "', " + "'EUR', " + "'" + erediOCM.cognomeE.ToUpper() + " " + erediOCM.nomeE.ToUpper() + "', " + "'" + erediOCM.IBANE.Substring(5, 5) + "', " + "'" + erediOCM.IBANE.Substring(10, 5) + "', " + "'" + erediOCM.IBANE.Substring(15) + "', " + "'" + erediOCM.IBANE + "', " + "CURRENT_TIMESTAMP, " + "'" + u.Username + "', " + "'" + erediOCM.IBANE.Substring(5, 10) + "', " + "'" + erediOCM.SWIFTE + "', " + "'" + erediOCM.IBANE.Substring(4, 1) + "') ", CommandType.Text);
                        }
                        else
                            this.objDataAccess.WriteTransactionData(" UPDATE EREDIBANC SET  " + " IBAN = '" + erediOCM.IBANE + "'" + ", INTEST = '" + erediOCM.cognomeE.ToUpper() + " " + erediOCM.nomeE.ToUpper() + "'" + ", TIPPAR = '" + erediOCM.parentelaE + "'" + ", DATINI = '" + DBMethods.Db2Date(erediOCM.datiniE) + "'" + ", PAESE = '" + erediOCM.IBANE.Substring(0, 2) + "'" + ", ABI = '" + erediOCM.IBANE.Substring(5, 5) + "'" + ", CAB = '" + erediOCM.IBANE.Substring(10, 5) + "'" + ", CCERE = '" + erediOCM.IBANE.Substring(15) + "'" + ", CODCIN = '" + erediOCM.IBANE.Substring(4, 1) + "'" + ", CODSWIFT = '" + erediOCM.SWIFTE + "'" + ", KEYBANC = '" + erediOCM.IBANE.Substring(5, 10) + "'" + ", ULTAGG = CURRENT_TIMESTAMP" + ", UTEAGG = '" + u.Username + "'" + " WHERE MAT = '" + mat + "'" + " AND PROERE = '" + erediOCM.proereE + "'" + " AND PROBANC = '" + erediOCM.probancE + "'", CommandType.Text);
                    }
                }
                this.objDataAccess.EndTransaction(true);
                SuccessMSG = "Inserimento erede effettuato con successo";
            }
            catch (Exception ex)
            {
                this.objDataAccess.EndTransaction(false);
                ErroreMSG = "Errore durante l'inserimento dell'erede";
            }
        }

        public void ModInsBlocchi(
          GestioneRapportiLavoroIscrittiOCM.BlocchiIscritto blocchiOCM,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMSG,
          ref string SuccessMSG,
          string mat)
        {
            try
            {
                this.objDataAccess.StartTransaction();
                if (blocchiOCM.id == "0" || string.IsNullOrEmpty(blocchiOCM.id))
                {
                    string strSQL = "SELECT VALUE(MAX(ID), 0) + 1 FROM ISCTBLOC";
                    blocchiOCM.id = this.objDataAccess.Get1ValueFromSQL(strSQL, CommandType.Text);
                    this.objDataAccess.WriteTransactionData("INSERT INTO ISCTBLOC(ID, MAT, TIPOBLOCCO, TIPOERRORE, ULTAGG, UTEAGG)" + "VALUES( " + "'" + blocchiOCM.id + "', " + "'" + mat + "', " + "'" + blocchiOCM.tipoBlocco + "', " + "'" + blocchiOCM.tipoErrore + "', " + "CURRENT_TIMESTAMP, " + "'" + u.Username + "') ", CommandType.Text);
                }
                else
                    this.objDataAccess.WriteTransactionData("UPDATE ISCTBLOC SET TIPOBLOCCO = '" + blocchiOCM.tipoBlocco + "', " + " TIPOERRORE = '" + blocchiOCM.tipoErrore + "' " + " WHERE MAT = '" + mat + "'  AND ID = '" + blocchiOCM.id + "'", CommandType.Text);
                this.objDataAccess.EndTransaction(true);
                SuccessMSG = "Inserimento effettuato con successo";
            }
            catch (Exception ex)
            {
                this.objDataAccess.EndTransaction(false);
                ErroreMSG = "Errore durante l'inserimneto";
            }
        }

        public void ModInsBnacari(
          GestioneRapportiLavoroIscrittiOCM.DatiBancariIscritto bancariOCM,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMSG,
          ref string SuccessMSG,
          string mat)
        {
            try
            {
                this.objDataAccess.StartTransaction();
                if (bancariOCM.proiscban == "0" || string.IsNullOrEmpty(bancariOCM.proiscban))
                {
                    string strSQL = "SELECT VALUE(MAX(PROISCBAN), 0) + 1 FROM ISCBANC WHERE MAT = '" + mat + "'";
                    bancariOCM.proiscban = this.objDataAccess.Get1ValueFromSQL(strSQL, CommandType.Text);
                    string str1 = this.objDataAccess.Get1ValueFromSQL("SELECT TRIM(COG) || ' ' || TRIM(NOM) FROM ISCT WHERE MAT =  '" + mat + "'", CommandType.Text);
                    string str2 = "INSERT INTO ISCBANC(MAT, INTEST, DATINI, DATFIN, MODPAG, PAESE, DIVISA,  ABI," + "CAB, CCERE, CODCIN, CODSWIFT, IBAN, KEYBANC, PROISCBAN, FLGINS, ULTAGG, UTEAGG) " + "VALUES( " + "'" + mat + "', " + "'" + str1 + "', " + " CURRENT_DATE, " + "'9999-12-31', " + "'B', " + "'" + bancariOCM.iban.Substring(0, 2) + "', " + "'EUR', " + "'" + bancariOCM.iban.Substring(5, 5) + "', " + "'" + bancariOCM.iban.Substring(10, 5) + "', " + "'" + bancariOCM.iban.Substring(15) + "', " + "'" + bancariOCM.iban.Substring(4, 1) + "', ";
                    this.objDataAccess.WriteTransactionData((string.IsNullOrEmpty(bancariOCM.swift) ? str2 + "NULL, " : str2 + "'" + bancariOCM.swift + "', ") + "'" + bancariOCM.iban + "', " + "'" + bancariOCM.iban.Substring(5, 10) + "', " + "'" + bancariOCM.proiscban + "', " + " 'I', CURRENT_TIMESTAMP, " + "'" + u.Username + "')", CommandType.Text);
                }
                else
                {
                    string str = this.objDataAccess.Get1ValueFromSQL("SELECT TRIM(COG) || ' ' || TRIM(NOM) FROM ISCT WHERE MAT = '" + mat + "'", CommandType.Text);
                    this.objDataAccess.WriteTransactionData(" UPDATE ISCBANC SET  " + " IBAN = '" + bancariOCM.iban + "'" + ", INTEST = '" + str + "'" + ", PAESE = '" + bancariOCM.iban.Substring(0, 2) + "'" + ", ABI = '" + bancariOCM.iban.Substring(5, 5) + "'" + ", CAB = '" + bancariOCM.iban.Substring(10, 5) + "'" + ", CCERE = '" + bancariOCM.iban.Substring(15) + "'" + ", CODCIN = '" + bancariOCM.iban.Substring(4, 1) + "'" + ", CODSWIFT = '" + bancariOCM.swift + "'" + ", KEYBANC = '" + bancariOCM.iban.Substring(5, 10) + "'" + ", ULTAGG = CURRENT_TIMESTAMP" + ", UTEAGG = '" + u.Username + "'" + ", FLGINS = 'I'" + " WHERE MAT = '" + mat + "'" + " AND PROISCBAN = '" + bancariOCM.proiscban + "'", CommandType.Text);
                }
                this.objDataAccess.EndTransaction(true);
                SuccessMSG = "Inserimento dati bancari andato a buon fine";
            }
            catch (Exception ex)
            {
                this.objDataAccess.EndTransaction(false);
                ErroreMSG = "Errore durante l'inserimento dei dati bancari";
            }
        }

        public void ModInsNote(
          GestioneRapportiLavoroIscrittiOCM.NoteIscritto noteOCM,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMSG,
          ref string SuccessMSG,
          string mat)
        {
            try
            {
                this.objDataAccess.StartTransaction();
                if (noteOCM.idnote == "0" || string.IsNullOrEmpty(noteOCM.idnote))
                {
                    string strSQL = "SELECT VALUE(MAX(ID), 0) + 1 FROM ISCTNOTE WHERE MAT = '" + mat + "'";
                    noteOCM.idnote = this.objDataAccess.Get1ValueFromSQL(strSQL, CommandType.Text);
                    this.objDataAccess.WriteTransactionData("INSERT INTO ISCTNOTE(ID, MAT, NOTE, ULTAGG, UTEAGG)" + "VALUES( " + "'" + noteOCM.idnote + "', " + "'" + mat + "', " + "'" + noteOCM.note + "', " + "CURRENT_TIMESTAMP, " + "'" + u.Username + "') ", CommandType.Text);
                }
                else
                    this.objDataAccess.WriteTransactionData("UPDATE ISCTNOTE SET NOTE = '" + noteOCM.note + "' " + " WHERE MAT = '" + mat + "'  AND ID = '" + noteOCM.idnote + "' ", CommandType.Text);
                this.objDataAccess.EndTransaction(true);
                SuccessMSG = "Inserimento note andato a buon fine";
            }
            catch (Exception ex)
            {
                this.objDataAccess.EndTransaction(false);
                ErroreMSG = "Errore durante l'inserimento note";
            }
        }

        public void ModInsDebito(
          GestioneRapportiLavoroIscrittiOCM.DebitiIscritto debitoOCM,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMSG,
          ref string SuccessMSG,
          string mat)
        {
            try
            {
                this.objDataAccess.StartTransaction();
                if (Convert.ToInt32(debitoOCM.protratt) == 0 || string.IsNullOrEmpty(debitoOCM.protratt))
                {
                    string strSQL1 = "SELECT PRORAP FROM RAPLAV WHERE MAT='" + mat + "' AND CODPOS = '" + debitoOCM.azi + "'";
                    debitoOCM.prorap = this.objDataAccess.Get1ValueFromSQL(strSQL1, CommandType.Text);
                    string strSQL2 = "SELECT DENTRATT FROM TIPTRATT WHERE TIPTRATT='" + debitoOCM.tiptratt + "'";
                    debitoOCM.desctratt = this.objDataAccess.Get1ValueFromSQL(strSQL2, CommandType.Text);
                    int int32 = Convert.ToInt32(this.objDataAccess.Get1ValueFromSQL("SELECT VALUE(MAX(PROTRATT), 0) + 1 FROM ISCTRATT WHERE CODPOS = '" + debitoOCM.azi + "' AND MAT = '" + mat + "'  AND PRORAP = '" + debitoOCM.prorap + "' ", CommandType.Text));
                    this.objDataAccess.WriteTransactionData("INSERT INTO ISCTRATT(CODPOS, MAT, PRORAP, PROTRATT, TIPTRATT, DESCTRATT, IMPDEBITO," + " IMPESTINTO, IMPRESIDUO, ULTAGG, UTEAGG)" + "VALUES( " + "'" + debitoOCM.azi + "', " + "'" + mat + "', " + "'" + debitoOCM.prorap + "', " + "'" + int32.ToString() + "', " + "'" + debitoOCM.tiptratt + "', " + "'" + debitoOCM.desctratt + "', " + "'" + debitoOCM.impdebito.Replace(".", "").Replace(",", ".") + "', " + "'" + debitoOCM.impestinto.Replace(".", "").Replace(",", ".") + "', " + "'" + debitoOCM.impresiduo.Replace(".", "").Replace(",", ".") + "', " + "CURRENT_TIMESTAMP, " + "'" + u.Username + "') ", CommandType.Text);
                }
                else
                    this.objDataAccess.WriteTransactionData("UPDATE ISCTRATT SET IMPDEBITO = '" + debitoOCM.impdebito.Replace(".", "").Replace(",", ".") + "', " + " IMPESTINTO = '" + debitoOCM.impestinto.Replace(".", "").Replace(",", ".") + "', " + " IMPRESIDUO = '" + debitoOCM.impresiduo.Replace(".", "").Replace(",", ".") + "' " + " WHERE CODPOS = '" + debitoOCM.codpos + "' AND MAT = '" + mat + "'  AND PRORAP = '" + debitoOCM.prorap + "'  AND PROTRATT = '" + debitoOCM.protratt + "' ", CommandType.Text);
                SuccessMSG = "Operazione completata";
                this.objDataAccess.EndTransaction(true);
            }
            catch (Exception ex)
            {
                this.objDataAccess.EndTransaction(false);
            }
        }

        public void Carica_Aliquote(
          GestioneRapportiLavoroIscrittiOCM rdl,
          string datanas,
          string codcon,
          string dencon,
          string codpos)
        {
            bool flag = false;
            string strData = datanas;
            string str1 = Convert.ToString(((DataView)HttpContext.Current.Session["ContrattiView"]).ToTable().Select("DENCON like '%" + dencon.Trim().Replace("'", "'''+'") + "%'")[0]["CODQUACON"]);
            try
            {
                if (string.IsNullOrEmpty(str1))
                    return;
                string str2 = "SELECT DISTINCT B.CODGRUASS, B.DENGRUASS, B.GRUDEF, B.GRUDEFCON, B.GRUDEFENP FROM ALIFORASS A INNER " + "JOIN GRUASS B ON A.CODGRUASS = B.CODGRUASS WHERE (B.CODQUACON = " + str1 + " OR B.CODQUACON IS " + "NULL) AND B.GRUCON IN ";
                string str3;
                string str4;
                if (Convert.ToInt32("0" + this.objDataAccess.Get1ValueFromSQL("SELECT COUNT(CODPOS) FROM PARGENPOS WHERE CODPOS = " + codpos, CommandType.Text)) > 0)
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
                List<GestioneRapportiLavoroIscrittiOCM.ListAliquota> listAliquotaList = new List<GestioneRapportiLavoroIscrittiOCM.ListAliquota>();
                foreach (DataRow row in (InternalDataCollectionBase)this.objDataAccess.GetDataTable(strSQL).Rows)
                {
                    GestioneRapportiLavoroIscrittiOCM.ListAliquota listAliquota = new GestioneRapportiLavoroIscrittiOCM.ListAliquota()
                    {
                        DENFORASS = row["DENGRUASS"].ToString(),
                        CODFORASS = row["CODGRUASS"].ToString()
                    };
                    listAliquotaList.Add(listAliquota);
                }
                rdl.ListAliq = listAliquotaList;
            }
            catch (Exception ex)
            {
            }
        }

        public void GetPercentuali(
          GestioneRapportiLavoroIscrittiOCM rdl,
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
            string str1 = Convert.ToString(((DataView)HttpContext.Current.Session["ContrattiView"]).ToTable().Select("DENCON like '%" + dencon.Trim().Replace("'", "'''+'") + "%'")[0]["CODQUACON"]);
            if (Convert.ToInt32(CODGRUASS) <= 0 || !flag)
                return;
            string strData = HttpContext.Current.Session["ProRap"] == null ? datIsc : (string.IsNullOrEmpty(datMod) ? datIsc : datMod);
            DataTable dataTable = this.objDataAccess.GetDataTable("SELECT A.CODGRUASS, C.DENFORASS, B.CODFORASS, B.ALIQUOTA, A.GRUDEF FROM GRUASS A " + "INNER JOIN ALIFORASS B ON A.CODGRUASS = B.CODGRUASS INNER JOIN FORASS C ON " + "B.CODFORASS = C.CODFORASS WHERE A.CODGRUASS = " + CODGRUASS + " AND B.CODQUACON = " + str1 + " AND '" + DBMethods.Db2Date(strData) + "' BETWEEN B.DATINI AND " + "VALUE(B.DATFIN,'9999-12-31') AND C.CATFORASS <> 'FAP' ");
            if (dataTable.Rows.Count > 0)
            {
                int num2 = dataTable.Rows.Count - 1;
                for (short index = 0; (int)index <= num2; ++index)
                    num1 += Convert.ToDecimal("0" + dataTable.Rows[(int)index]["ALIQUOTA"]?.ToString());
            }
            string strSQL = "SELECT VALFAP FROM CODFAP WHERE '" + DBMethods.Db2Date(strData) + "' BETWEEN " + "DATINI AND VALUE(DATFIN,'9999-12-31')";
            this.objDataAccess.Get1ValueFromSQL(strSQL, CommandType.Text);
            Decimal num3 = Convert.ToDecimal(this.objDataAccess.Get1ValueFromSQL(strSQL, CommandType.Text) ?? "");
            if (fap)
                num1 += Convert.ToDecimal("0" + num3.ToString());
            string str2 = num1.ToString("#,##0.#0");
            dataTable.Dispose();
            rdl.datiRetributivi.TOTALIQ = str2;
            rdl.datiContrattuali.CODQUA = str1;
        }

        public void AggiornaScatti(
          GestioneRapportiLavoroIscrittiOCM rdl,
          string numscatti,
          string dencon,
          string codliv,
          string datmod,
          string datini,
          string prorap,
          string tipRap,
          string perPar)
        {
            DateTimeFormatInfo dateTimeFormat = new CultureInfo("it-IT", false).DateTimeFormat;
            DataRow[] dataRowArray = ((DataView)HttpContext.Current.Session["ContrattiView"]).ToTable().Select("DENCON like '%" + dencon.Trim().Replace("'", "'''+'") + "%'");
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
                    rdl.datiRetributivi.importoSc = num1.ToString();
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
                            if (!string.IsNullOrEmpty(perPar))
                            {
                                Decimal num2 = Convert.ToDecimal("0" + perPar);
                                num1 = num1 / 100M * num2;
                            }
                            if (prorap == null)
                            {
                                if (!string.IsNullOrEmpty(perPar))
                                {
                                    Decimal num3 = Convert.ToDecimal("0" + perPar);
                                    num1 = num1 / 100M * num3;
                                    break;
                                }
                                break;
                            }
                            if (!string.IsNullOrEmpty(perPar))
                            {
                                Decimal num4 = Convert.ToDecimal("0" + perPar);
                                num1 = num1 / 100M * num4;
                                break;
                            }
                            break;
                    }
                    rdl.datiRetributivi.importoSc = ((Decimal)Convert.ToInt32(numscatti) * num1).ToString("#,##0.#0");
                }
                else
                {
                    if (Convert.ToInt32("0" + numscatti) != 0)
                        return;
                    rdl.datiRetributivi.importoSc = "0,00";
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void AggiornaEmolumenti(
          GestioneRapportiLavoroIscrittiOCM rdl,
          string codliv,
          string dencon,
          string datMod)
        {
            try
            {
                DataTable table = ((DataView)HttpContext.Current.Session["ContrattiView"]).ToTable();
                DataLayer objDataAccess = new DataLayer();
                string str = dencon.Trim().Replace("'", "'''+'");
                DataRow[] dataRowArray = table.Select("DENCON like '%" + str + "%'");
                int int32_1 = Convert.ToInt32(dataRowArray[0]["PROCON"]);
                int int32_2 = (int)(short)Convert.ToInt32(dataRowArray[0]["CODLOC"]);
                int int32_3 = (int)(short)Convert.ToInt32(dataRowArray[0]["CODCON"]);
                int int32_4 = Convert.ToInt32(dataRowArray[0]["PROLOC"]);
                Decimal minimoContrattuale = this.ModGetDati.Module_GetMinimoContrattuale(objDataAccess, int32_3, int32_1, int32_2, int32_4, Convert.ToInt32(codliv), datMod, 0M, 0M);
                rdl.datiRetributivi.emolumenti = minimoContrattuale.ToString("#,##0.#0");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void DebitoLoad(GestioneRapportiLavoroIscrittiOCM rdl, string mat)
        {
            try
            {
                DataTable dataTable = this.objDataAccess.GetDataTable("SELECT DISTINCT CODPOS, TRIM(CODPOS || ' - ' || (SELECT TRIM(RAGSOC) FROM AZI WHERE CODPOS = A.CODPOS)) AS POSIZIONE FROM RAPLAV A WHERE MAT = " + mat + " ORDER BY CODPOS");
                List<GestioneRapportiLavoroIscrittiOCM.AziendaUtilizzatrice> aziendaUtilizzatriceList = new List<GestioneRapportiLavoroIscrittiOCM.AziendaUtilizzatrice>();
                List<GestioneRapportiLavoroIscrittiOCM.listdebiti2> listdebiti2List = new List<GestioneRapportiLavoroIscrittiOCM.listdebiti2>();
                foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
                {
                    GestioneRapportiLavoroIscrittiOCM.AziendaUtilizzatrice aziendaUtilizzatrice = new GestioneRapportiLavoroIscrittiOCM.AziendaUtilizzatrice()
                    {
                        codposAz = row["CODPOS"].ToString(),
                        ragsocAz = row["POSIZIONE"].ToString()
                    };
                    aziendaUtilizzatriceList.Add(aziendaUtilizzatrice);
                }
                rdl.listAzienda = aziendaUtilizzatriceList;
                foreach (DataRow row in (InternalDataCollectionBase)this.objDataAccess.GetDataTable("SELECT TIPTRATT, DENTRATT FROM TIPTRATT ORDER BY DENTRATT").Rows)
                {
                    GestioneRapportiLavoroIscrittiOCM.listdebiti2 listdebiti2 = new GestioneRapportiLavoroIscrittiOCM.listdebiti2()
                    {
                        tiptratt = row["TIPTRATT"].ToString(),
                        TipDeb = row["DENTRATT"].ToString()
                    };
                    listdebiti2List.Add(listdebiti2);
                }
                rdl.listdebiti = listdebiti2List;
            }
            catch (Exception ex)
            {
            }
        }

        private void ErediLoad(GestioneRapportiLavoroIscrittiOCM rdl)
        {
            string strSQL = "SELECT CODPAR,DENPAR FROM TIPPAR";
            try
            {
                DataTable dataTable = this.objDataAccess.GetDataTable(strSQL);
                List<GestioneRapportiLavoroIscrittiOCM.eredi2> eredi2List = new List<GestioneRapportiLavoroIscrittiOCM.eredi2>();
                foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
                {
                    GestioneRapportiLavoroIscrittiOCM.eredi2 eredi2 = new GestioneRapportiLavoroIscrittiOCM.eredi2()
                    {
                        codpar = row["CODPAR"].ToString(),
                        denpar = row["DENPAR"].ToString()
                    };
                    eredi2List.Add(eredi2);
                }
                rdl.listeredi = eredi2List;
            }
            catch (Exception ex)
            {
            }
        }

        private void BloccoLoad(GestioneRapportiLavoroIscrittiOCM rdl)
        {
            try
            {
                DataTable dataTable1 = this.objDataAccess.GetDataTable("SELECT DENERRORE,TIPERRORE FROM TIPERRORE");
                List<GestioneRapportiLavoroIscrittiOCM.tipERR> tipErrList = new List<GestioneRapportiLavoroIscrittiOCM.tipERR>();
                foreach (DataRow row in (InternalDataCollectionBase)dataTable1.Rows)
                {
                    GestioneRapportiLavoroIscrittiOCM.tipERR tipErr = new GestioneRapportiLavoroIscrittiOCM.tipERR()
                    {
                        denerrore = row["DENERRORE"].ToString(),
                        tiperrore = row["TIPERRORE"].ToString()
                    };
                    tipErrList.Add(tipErr);
                }
                rdl.listErr = tipErrList;
                DataTable dataTable2 = this.objDataAccess.GetDataTable("SELECT TIPBLOCCO,DENBLOCCO FROM TIPBLOCCO");
                List<GestioneRapportiLavoroIscrittiOCM.tipBlocco> tipBloccoList = new List<GestioneRapportiLavoroIscrittiOCM.tipBlocco>();
                foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                {
                    GestioneRapportiLavoroIscrittiOCM.tipBlocco tipBlocco = new GestioneRapportiLavoroIscrittiOCM.tipBlocco()
                    {
                        denblocco = row["DENBLOCCO"].ToString(),
                        tipblocco = row["TIPBLOCCO"].ToString()
                    };
                    tipBloccoList.Add(tipBlocco);
                }
                rdl.listblocco = tipBloccoList;
            }
            catch (Exception ex)
            {
            }
        }

        public void CercaBanca(GestioneRapportiLavoroIscrittiOCM rdl, string iban)
        {
            DataTable dataTable1 = new DataTable();
            try
            {
                DataTable dataTable2 = this.objDataAccess.GetDataTable("SELECT PAESE, ISTITUTO, FILIALE, SIGPRO, IND FROM BANCHE WHERE KEYBANC = '" + iban.Substring(5, 10) + "' AND PAESE = '" + iban.Substring(0, 2) + "'");
                if (dataTable2.Rows.Count <= 0)
                    return;
                rdl.datiBancariIscritto.istCre = dataTable2.Rows[0]["ISTITUTO"].ToString().Trim();
                rdl.datiBancariIscritto.age = dataTable2.Rows[0]["FILIALE"].ToString().Trim();
                rdl.datiBancariIscritto.indAge = dataTable2.Rows[0]["IND"].ToString().Trim();
                rdl.datiBancariIscritto.proiscban = dataTable2.Rows[0]["SIGPRO"].ToString().Trim();
            }
            catch (Exception ex)
            {
            }
        }

        public void CarciaErediMod(GestioneRapportiLavoroIscrittiOCM rdl, string mat, string proere)
        {
            try
            {
                string strSQL1 = " SELECT * FROM EREDI WHERE MAT = " + mat + " AND PROERE = " + proere + " ";
                string strSQL2 = "SELECT * FROM EREDIBANC  WHERE MAT = " + mat + " AND PROERE = " + proere + " ";
                DataTable dataTable1 = this.objDataAccess.GetDataTable(strSQL1);
                DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL2);
                rdl.eredi.nomeE = dataTable1.Rows[0]["NOME"].ToString();
                rdl.eredi.cognomeE = dataTable1.Rows[0]["COGNOME"].ToString();
                rdl.eredi.sessoE = dataTable1.Rows[0]["SESSO"].ToString();
                rdl.eredi.codfisE = dataTable1.Rows[0]["CODFISC"].ToString();
                rdl.eredi.datnasE = Convert.ToDateTime(dataTable1.Rows[0]["DATNAS"]).ToString("yyyy-MM-dd");
                rdl.eredi.parentelaE = dataTable1.Rows[0]["TIPPAR"].ToString();
                rdl.eredi.datiniE = Convert.ToDateTime(dataTable1.Rows[0]["DATINI"]).ToString("yyyy-MM-dd");
                rdl.eredi.coddugE = dataTable1.Rows[0]["CODDUG"].ToString();
                rdl.eredi.comuneCodNE = dataTable1.Rows[0]["CODCOMNAS"].ToString();
                rdl.eredi.provNE = dataTable1.Rows[0]["SIGPRO"].ToString();
                rdl.eredi.indirizzoE = dataTable1.Rows[0]["IND"].ToString().Trim();
                rdl.eredi.statoEs_E = dataTable1.Rows[0]["STAESTRES"].ToString();
                rdl.eredi.comuneCodE = dataTable1.Rows[0]["CODCOM"].ToString();
                rdl.eredi.capE = dataTable1.Rows[0]["CAP"].ToString();
                rdl.eredi.localitaE = dataTable1.Rows[0]["DENLOC"].ToString();
                rdl.eredi.provE = dataTable1.Rows[0]["SIGPRORES"].ToString();
                rdl.eredi.perTFRE = dataTable1.Rows[0]["PERCTFR"].ToString();
                rdl.eredi.perFPE = dataTable1.Rows[0]["PERCFP"].ToString();
                rdl.eredi.maggE = dataTable1.Rows[0]["MAGGIOR"].ToString();
                rdl.eredi.maggPerE = dataTable1.Rows[0]["PERCMAGG"].ToString();
                rdl.eredi.emailE = dataTable1.Rows[0]["EMAIL"].ToString();
                rdl.eredi.telE = dataTable1.Rows[0]["TEL"].ToString();
                rdl.eredi.coE = dataTable1.Rows[0]["CO"].ToString();
                if (dataTable2.Rows.Count > 0)
                {
                    rdl.eredi.IBANE = dataTable2.Rows[0]["IBAN"].ToString();
                    rdl.eredi.probancE = dataTable2.Rows[0]["PROBANC"].ToString();
                    rdl.eredi.SWIFTE = dataTable2.Rows[0]["CODSWIFT"].ToString();
                    this.CercaBanca(rdl, rdl.eredi.IBANE);
                }
                string strSQL3 = " SELECT DENPAR FROM TIPPAR WHERE CODPAR = '" + rdl.eredi.parentelaE + "' ";
                rdl.eredi.denparE = this.objDataAccess.Get1ValueFromSQL(strSQL3, CommandType.Text).ToString();
                string strSQL4 = " SELECT DENCOM FROM CODCOM WHERE CODCOM = '" + rdl.eredi.comuneCodNE + "' ";
                rdl.eredi.comuneNE = this.objDataAccess.Get1ValueFromSQL(strSQL4, CommandType.Text).ToString().Trim();
                string strSQL5 = " SELECT DENCOM FROM CODCOM WHERE CODCOM = '" + rdl.eredi.comuneCodE + "' ";
                rdl.eredi.comuneE = this.objDataAccess.Get1ValueFromSQL(strSQL5, CommandType.Text).ToString().Trim();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Module_GetListaAliquoteContributive(
          GestioneRapportiLavoroIscrittiOCM rdl,
          int CODQUACON,
          int CODGRUASS,
          string STRDATA,
          string FAP)
        {
            DataLayer dataLayer = new DataLayer();
            Decimal num1 = 0.00M;
            DataTable dataTable1 = new DataTable();
            bool flag = this.Utils.Module_Check_65Anni(this.Utils.Module_GetDataSistema(), STRDATA);
            string str1 = (string)null;
            if (FAP == "false")
                FAP = "N";
            else if (FAP == "true")
                FAP = "S";
            if (string.IsNullOrEmpty(STRDATA))
                return;
            try
            {
                if (FAP == "S")
                {
                    string strSQL = "SELECT VALFAP FROM CODFAP WHERE '" + DBMethods.Db2Date(STRDATA) + "' BETWEEN DATINI AND  VALUE(DATFIN, '9999-12-31')";
                    dataTable1 = dataLayer.GetDataTable(strSQL);
                    str1 = dataTable1.Rows.Count <= 0 ? "0,00" : dataTable1.Rows[0]["VALFAP"].ToString();
                }
                string str2 = "SELECT DISTINCT ALIFORASS.CODFORASS,ALIFORASS.ALIQUOTA," + " (" + " SELECT DENFORASS FROM FORASS WHERE FORASS.CODFORASS=ALIFORASS.CODFORASS" + " ) AS DENFORASS" + " FROM ALIFORASS WHERE" + " CODQUACON =  '" + CODQUACON.ToString() + "' " + " AND CODGRUASS= '" + CODGRUASS.ToString() + "' " + " AND '" + DBMethods.Db2Date(STRDATA) + "' BETWEEN DATINI AND DATFIN";
                if (flag)
                    str2 += " AND ALIFORASS.CODFORASS IN (SELECT CODFORASS FROM FORASS WHERE CATFORASS<>'PREV') ";
                string strSQL1 = str2 + " AND ALIFORASS.CODFORASS NOT IN (SELECT CODFORASS FROM FORASS WHERE CATFORASS='FAP') ";
                dataTable1.Rows.Clear();
                DataTable dataTable2 = dataLayer.GetDataTable(strSQL1);
                int num2 = dataTable2.Rows.Count - 1;
                List<GestioneRapportiLavoroIscrittiOCM.ListAliquota> listAliquotaList = new List<GestioneRapportiLavoroIscrittiOCM.ListAliquota>();
                foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                {
                    GestioneRapportiLavoroIscrittiOCM.ListAliquota listAliquota = new GestioneRapportiLavoroIscrittiOCM.ListAliquota()
                    {
                        DENFORASS = row["DENFORASS"].ToString(),
                        CODFORASS = row["CODFORASS"].ToString(),
                        ALIQUOTA = row["ALIQUOTA"].ToString()
                    };
                    listAliquotaList.Add(listAliquota);
                }
                rdl.ListAliq = listAliquotaList;
                int index;
                for (index = 0; index <= num2; ++index)
                    num1 += Convert.ToDecimal(dataTable2.Rows[index]["ALIQUOTA"]);
                if (FAP == "S")
                {
                    string str3 = Convert.ToString(dataTable2.Rows[index][nameof(FAP)].ToString()).Replace(".", ",");
                    num1 += Convert.ToDecimal(str3);
                }
                rdl.datiRetributivi.TOTALIQ = num1.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nel caricamento lista aliquote");
            }
        }
    }
}
