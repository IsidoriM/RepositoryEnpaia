// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Amministrativo.GestioneAziendaWebDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using Newtonsoft.Json;
using OCM.TFI.OCM.AziendaConsulente;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using TFI.CRYPTO.Crypto;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.Amministrativo;
using Utilities;

namespace TFI.DAL.Amministrativo
{
    public class GestioneAziendaWebDAL
    {
        public List<GestioneAziendeWebOCM.DatiAzienda> Ricerca(
          string Codpos,
          string Ragsoc,
          string Partitaiva,
          string codfis,
          ref string ErroreMSG,
          ref string SuccessMSG,
          string Cessato)
        {
            DataLayer dataLayer = new DataLayer();
            DataTable dataTable1 = new DataTable();
            string str1 = "";
            string str2 = "";
            if (!string.IsNullOrEmpty(Codpos) && string.IsNullOrEmpty(Ragsoc) && string.IsNullOrEmpty(Partitaiva) && string.IsNullOrEmpty(codfis))
                str1 = str1 + "A.CODICE = '" + Codpos + "'";
            if (!string.IsNullOrEmpty(Ragsoc) && string.IsNullOrEmpty(Partitaiva) && string.IsNullOrEmpty(codfis) && string.IsNullOrEmpty(Codpos))
                str1 = str1 + "A.RAGSOC LIKE '%" + Ragsoc.ToUpper() + "%'";
            if (!string.IsNullOrEmpty(Partitaiva) && string.IsNullOrEmpty(Ragsoc) && string.IsNullOrEmpty(codfis) && string.IsNullOrEmpty(Codpos))
                str1 = str1 + "A.PARIVA = '" + Partitaiva + "'";
            if (!string.IsNullOrEmpty(codfis) && string.IsNullOrEmpty(Ragsoc) && string.IsNullOrEmpty(Partitaiva) && string.IsNullOrEmpty(Codpos))
                str1 = str1 + "A.CODFIS = '" + codfis.ToUpper() + "'";
            if (!string.IsNullOrEmpty(Ragsoc) && !string.IsNullOrEmpty(Partitaiva) && string.IsNullOrEmpty(codfis) && string.IsNullOrEmpty(Codpos))
                str1 = str1 + "A.RAGSOC LIKE '%" + Ragsoc.ToUpper() + "%' AND A.PARIVA = '" + Partitaiva + "'";
            if (!string.IsNullOrEmpty(Ragsoc) && string.IsNullOrEmpty(Partitaiva) && !string.IsNullOrEmpty(codfis) && string.IsNullOrEmpty(Codpos))
                str1 = str1 + "A.RAGSOC LIKE '%" + Ragsoc.ToUpper() + "%' AND A.CODFIS = '" + codfis.ToUpper() + "'";
            if (string.IsNullOrEmpty(Ragsoc) && !string.IsNullOrEmpty(Partitaiva) && !string.IsNullOrEmpty(codfis) && string.IsNullOrEmpty(Codpos))
                str1 = str1 + "A.PARIVA = '" + Partitaiva + "' AND A.CODFIS = '" + codfis + "'";
            if (!string.IsNullOrEmpty(Ragsoc) && !string.IsNullOrEmpty(Partitaiva) && !string.IsNullOrEmpty(codfis) && string.IsNullOrEmpty(Codpos))
                str1 = str1 + "A.RAGSOC LIKE '%" + Ragsoc.ToUpper() + "%' AND A.PARIVA = '" + Partitaiva + "' AND A.CODFIS = '" + codfis.ToUpper() + "'";
            if (!string.IsNullOrEmpty(Ragsoc) && !string.IsNullOrEmpty(Partitaiva) && !string.IsNullOrEmpty(codfis) && !string.IsNullOrEmpty(Codpos))
                str1 = str1 + "A.RAGSOC LIKE '%" + Ragsoc.ToUpper() + "%' AND A.PARIVA = '" + Partitaiva + "' AND A.CODFIS = '" + codfis.ToUpper() + "' AND A.CODICE='" + Codpos + "'";
            if (!string.IsNullOrEmpty(Ragsoc) && string.IsNullOrEmpty(Partitaiva) && string.IsNullOrEmpty(codfis) && !string.IsNullOrEmpty(Codpos))
                str1 = str1 + "A.RAGSOC LIKE '%" + Ragsoc.ToUpper() + "%' AND A.CODICE ='" + Codpos + "'";
            if (string.IsNullOrEmpty(Ragsoc) && !string.IsNullOrEmpty(Partitaiva) && string.IsNullOrEmpty(codfis) && !string.IsNullOrEmpty(Codpos))
                str1 = str1 + "A.PARIVA = '" + Partitaiva + "' AND A.CODICE ='" + Codpos + "'";
            if (string.IsNullOrEmpty(Ragsoc) && string.IsNullOrEmpty(Partitaiva) && !string.IsNullOrEmpty(codfis) && !string.IsNullOrEmpty(Codpos))
                str1 = str1 + "A.CODFIS = '" + codfis.ToUpper() + "' AND A.CODICE ='" + Codpos + "'";
            if (!string.IsNullOrEmpty(Ragsoc) && string.IsNullOrEmpty(Partitaiva) && !string.IsNullOrEmpty(codfis) && !string.IsNullOrEmpty(Codpos))
                str1 = str1 + "A.RAGSOC LIKE '%" + Ragsoc.ToUpper() + "%' AND A.CODFIS = '" + codfis.ToUpper() + "' AND A.CgenerapinODPOSCONF ='" + Codpos + "'";
            if (string.IsNullOrEmpty(Ragsoc) && !string.IsNullOrEmpty(Partitaiva) && !string.IsNullOrEmpty(codfis) && !string.IsNullOrEmpty(Codpos))
                str1 = str1 + "A.PARIVA = '" + Partitaiva + "' AND A.CODFIS = '" + codfis.ToUpper() + "' AND A.CODICE ='" + Codpos + "'";
            if (!string.IsNullOrEmpty(Ragsoc) && !string.IsNullOrEmpty(Partitaiva) && string.IsNullOrEmpty(codfis) && !string.IsNullOrEmpty(Codpos))
                str1 = str1 + "A.RAGSOC LIKE '%" + Ragsoc.ToUpper() + "%' AND A.PARIVA = '" + Partitaiva + "' AND A.CODICE ='" + Codpos + "'";
            if (str1 == "")
            {
                if (Cessato == "Attivo")
                    str2 = "A.DATCONF IS NOT NULL";
                else if (Cessato == nameof(Cessato))
                    str2 = "A.DATCONF IS NULL";
            }
            else if (Cessato == "Attivo")
                str2 = "AND A.DATCONF IS NOT NULL";
            else if (Cessato == nameof(Cessato))
                str2 = "AND A.DATCONF IS NULL";
            string strSQL = "SELECT A.CODICE, A.RAGSOC, A.RAGSOCBRE, A.PARIVA, A.CODFIS, A.NATGIU, A.URL, A.DATAPE, A.ULTAGG, A.UTEAGG, A.NOTE,  A.CODMEZ, A.DATCOM, A.DATDENAPE, A.CODPOSCONF, A.DATCONF," + "(SELECT DENMEZ FROM CODMEZ WHERE CODMEZ = A.CODMEZ) AS DENMEZ, (SELECT DENNATGIU FROM NATGIU WHERE NATGIU = A.NATGIU) AS DENNATGIU," + "(SELECT CODOPE FROM TKRISORSE WHERE DATANN IS NULL  AND CODFIS IN(SELECT CODFIS FROM UTENTI WHERE CODUTE = A.UTECONF)) AS UTECONF FROM AZIWEB A " + " WHERE " + str1 + " " + str2 + " " + " ORDER BY DATCOM";
            DataTable dataTable2 = dataLayer.GetDataTable(strSQL);
            List<GestioneAziendeWebOCM.DatiAzienda> datiAziendaList = new List<GestioneAziendeWebOCM.DatiAzienda>();
            if (dataTable2.Rows.Count > 0)
            {
                foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                {
                    GestioneAziendeWebOCM.DatiAzienda datiAzienda1 = new GestioneAziendeWebOCM.DatiAzienda();
                    datiAzienda1.CodiceAziendaWeb = row["CODICE"].ToString();
                    datiAzienda1.RagioneSociale = row["RAGSOC"].ToString().Trim();
                    datiAzienda1.RagioneSocialeBreve = row["RAGSOCBRE"].ToString();
                    datiAzienda1.PartitaIva = row["PARIVA"].ToString();
                    datiAzienda1.CodiceFiscale = row["CODFIS"].ToString();
                    datiAzienda1.CodiceNaturaGiuridica = row["NATGIU"].ToString();
                    datiAzienda1.Note = row["NOTE"].ToString();
                    datiAzienda1.NaturaGiuridica = row["DENNATGIU"].ToString();
                    datiAzienda1.URL = row["URL"].ToString();
                    DateTime dateTime;
                    string str3;
                    if (!Convert.IsDBNull(row["DATCOM"]))
                    {
                        dateTime = Convert.ToDateTime(row["DATCOM"]);
                        str3 = dateTime.ToString("dd/MM/yyyy");
                    }
                    else
                        str3 = (string)null;
                    datiAzienda1.DataComunicazione = str3;
                    datiAzienda1.MezziComunicazione = row["DENMEZ"].ToString();
                    string str4;
                    if (!Convert.IsDBNull(row["DATAPE"]))
                    {
                        dateTime = Convert.ToDateTime(row["DATAPE"]);
                        str4 = dateTime.ToString("dd/MM/yyyy");
                    }
                    else
                        str4 = (string)null;
                    datiAzienda1.DataApertura = str4;
                    string str5;
                    if (!Convert.IsDBNull(row["DATDENAPE"]))
                    {
                        dateTime = Convert.ToDateTime(row["DATDENAPE"]);
                        str5 = dateTime.ToString("dd/MM/yyyy");
                    }
                    else
                        str5 = (string)null;
                    datiAzienda1.DataDenunciaApertura = str5;
                    datiAzienda1.CodicePosizioneAzienda = row["CODPOSCONF"].ToString();
                    datiAzienda1.CodiceMezzoComunicazione = row["CODMEZ"].ToString();
                    string str6;
                    if (!Convert.IsDBNull(row["DATCONF"]))
                    {
                        dateTime = Convert.ToDateTime(row["DATCONF"]);
                        str6 = dateTime.ToString("dd/MM/yyyy");
                    }
                    else
                        str6 = (string)null;
                    datiAzienda1.DataConferma = str6;
                    GestioneAziendeWebOCM.DatiAzienda datiAzienda2 = datiAzienda1;
                    datiAziendaList.Add(datiAzienda2);
                }
            }
            else
                ErroreMSG = "Nessun risultato trovato";
            return datiAziendaList;
        }

        public Dictionary<string, string> GetTitStu()
        {
            var titStu = new DataLayer().GetDataTable("SELECT DENTITSTU, CODTITSTU FROM TITSTU ORDER BY ORDINE ASC");
            return titStu.Rows.Cast<DataRow>().ToDictionary(row => row.ElementAt("CODTITSTU"), row => row.ElementAt("DENTITSTU"));
        }

        public List<GestioneAziendeWebOCM.NatGiu> nats()
        {
            DataTable dataTable = new DataLayer().GetDataTable("SELECT NATGIU, DENNATGIU FROM NATGIU");
            List<GestioneAziendeWebOCM.NatGiu> natGiuList = new List<GestioneAziendeWebOCM.NatGiu>();
            foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
                natGiuList.Add(new GestioneAziendeWebOCM.NatGiu()
                {
                    NATGIU = row["NATGIU"].ToString(),
                    DENNATGIU = row["DENNATGIU"].ToString()
                });
            return natGiuList;
        }

        public List<GestioneAziendeWebOCM.MezzComm> mezzs()
        {
            DataTable dataTable = new DataLayer().GetDataTable("SELECT CODMEZ, DENMEZ FROM CODMEZ");
            List<GestioneAziendeWebOCM.MezzComm> mezzCommList = new List<GestioneAziendeWebOCM.MezzComm>();
            foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
                mezzCommList.Add(new GestioneAziendeWebOCM.MezzComm()
                {
                    CODMEZ = row["CODMEZ"].ToString(),
                    DENMEZ = row["DENMEZ"].ToString()
                });
            return mezzCommList;
        }

        public List<GestioneAziendeWebOCM.TipoRapLeg> rapLegs()
        {
            DataTable dataTable = new DataLayer().GetDataTable("SELECT CODFUNRAP, DENFUNRAP FROM FUNRAP");
            List<GestioneAziendeWebOCM.TipoRapLeg> tipoRapLegList = new List<GestioneAziendeWebOCM.TipoRapLeg>();
            foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
                tipoRapLegList.Add(new GestioneAziendeWebOCM.TipoRapLeg()
                {
                    CODFUNRAP = row["CODFUNRAP"].ToString(),
                    DENFUNRAP = row["DENFUNRAP"].ToString()
                });
            return tipoRapLegList;
        }

        public List<GestioneAziendeWebOCM.Via> via()
        {
            DataTable dataTable = new DataLayer().GetDataTable("SELECT CODDUG, DENDUG FROM DUG ORDER BY DENDUG ASC");
            List<GestioneAziendeWebOCM.Via> viaList = new List<GestioneAziendeWebOCM.Via>();
            foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
                viaList.Add(new GestioneAziendeWebOCM.Via()
                {
                    CODDUG = row["CODDUG"].ToString(),
                    DENDUG = row["DENDUG"].ToString()
                });
            return viaList;
        }

        public List<GestioneAziendeWebOCM.Provincia> provincia()
        {
            DataTable dataTable = new DataLayer().GetDataTable("SELECT SIGPRO, CODREG, DENPRO FROM SIGPRO");
            List<GestioneAziendeWebOCM.Provincia> provinciaList = new List<GestioneAziendeWebOCM.Provincia>();
            foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
                provinciaList.Add(new GestioneAziendeWebOCM.Provincia()
                {
                    SIGPRO = row["SIGPRO"].ToString(),
                    DENPRO = row["DENPRO"].ToString(),
                    CODREG = row["CODREG"].ToString()
                });
            
            return provinciaList;
        }


        public List<GestioneAziendeWebOCM.Comune> comunes()
        {
            DataTable dataTable = new DataLayer().GetDataTable("SELECT CODCOM, DENCOM, SIGPRO FROM CODCOM ORDER BY DENCOM ASC");
            List<GestioneAziendeWebOCM.Comune> comuneList = new List<GestioneAziendeWebOCM.Comune>();
            foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
                comuneList.Add(new GestioneAziendeWebOCM.Comune()
                {
                    CODCOM = row["CODCOM"].ToString(),
                    DENCOM = row["DENCOM"].ToString(),
                    SIGPRO = row["SIGPRO"].ToString()
                });
            return comuneList;
        }

        public List<GestioneAziendeWebOCM.CategoriaAttivita> categoriaAttivitas()
        {
            DataTable dataTable = new DataLayer().GetDataTable("SELECT CATATTCAM, DENCATATT FROM CATATT");
            List<GestioneAziendeWebOCM.CategoriaAttivita> categoriaAttivitaList = new List<GestioneAziendeWebOCM.CategoriaAttivita>();
            foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
                categoriaAttivitaList.Add(new GestioneAziendeWebOCM.CategoriaAttivita()
                {
                    CATATTCAM = row["CATATTCAM"].ToString(),
                    DENCATATT = row["DENCATATT"].ToString()
                });
            return categoriaAttivitaList;
        }

        public List<GestioneAziendeWebOCM.TipoAttivita> tipoAttivitas()
        {
            DataTable dataTable = new DataLayer().GetDataTable("SELECT CODATTCAM, DENATTCAM, CATATTCAM FROM TIPATT");
            List<GestioneAziendeWebOCM.TipoAttivita> tipoAttivitaList = new List<GestioneAziendeWebOCM.TipoAttivita>();
            foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
                tipoAttivitaList.Add(new GestioneAziendeWebOCM.TipoAttivita()
                {
                    CODATTCAM = row["CODATTCAM"].ToString(),
                    DENATTCAM = row["DENATTCAM"].ToString(),
                    CATATTCAM = row["CATATTCAM"].ToString()
                });
            return tipoAttivitaList;
        }

        public List<GestioneAziendeWebOCM.CodiceStatistico> codiceStatisticos()
        {
            DataTable dataTable = new DataLayer().GetDataTable("SELECT CODSTACON, DENCODSTA  FROM CODSTA");
            List<GestioneAziendeWebOCM.CodiceStatistico> codiceStatisticoList = new List<GestioneAziendeWebOCM.CodiceStatistico>();
            foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
                codiceStatisticoList.Add(new GestioneAziendeWebOCM.CodiceStatistico()
                {
                    CODSTACON = row["CODSTACON"].ToString(),
                    DENCODSTA = row["DENCODSTA"].ToString()
                });
            return codiceStatisticoList;
        }

        public GestioneAziendeWebOCM.DatiAzienda CaricaDatiDettagliAzienda(
          int CodiceAziendaWeb)
        {
            DataTable dataTable = new DataLayer().GetDataTable("SELECT A.*, (SELECT DENMEZ FROM CODMEZ WHERE CODMEZ = A.CODMEZ) AS DENMEZ," + " (SELECT DENNATGIU FROM NATGIU WHERE NATGIU = A.NATGIU) AS DENNATGIU FROM AZIWEB  A " + " WHERE A.CODICE = " + CodiceAziendaWeb.ToString());
            GestioneAziendeWebOCM.DatiAzienda datiAzienda = new GestioneAziendeWebOCM.DatiAzienda();
            if (dataTable.Rows.Count > 0)
            {
                datiAzienda.CodiceAziendaWeb = dataTable.Rows[0]["CODICE"].ToString().Trim();
                datiAzienda.RagioneSociale = dataTable.Rows[0]["RAGSOC"].ToString().Trim();
                datiAzienda.RagioneSocialeBreve = dataTable.Rows[0]["RAGSOCBRE"].ToString().Trim();
                datiAzienda.DataApertura = dataTable.Rows[0]["DATAPE"].ToString().Substring(0, 10);
                datiAzienda.DataDenunciaApertura = dataTable.Rows[0]["DATDENAPE"].ToString().Substring(0, 10);
                datiAzienda.PartitaIva = dataTable.Rows[0]["PARIVA"].ToString().Trim();
                datiAzienda.CodiceFiscale = dataTable.Rows[0]["CODFIS"].ToString().Trim();
                datiAzienda.URL = dataTable.Rows[0]["URL"].ToString().Trim();
                datiAzienda.MezziComunicazione = dataTable.Rows[0]["DENMEZ"].ToString().Trim();
                datiAzienda.NaturaGiuridica = dataTable.Rows[0]["DENNATGIU"].ToString().Trim();
                datiAzienda.Note = dataTable.Rows[0]["NOTE"].ToString().Trim();
                datiAzienda.DataRegistrazione = DBMethods.Db2Date(dataTable.Rows[0]["DATCOM"].ToString());
                datiAzienda.CodicePosizioneAzienda = dataTable.Rows[0]["CODPOSCONF"].ToString().Trim();
                datiAzienda.CodiceMezzoComunicazione = dataTable.Rows[0]["CODMEZ"].ToString().Trim();
                datiAzienda.CodiceNaturaGiuridica = dataTable.Rows[0]["NATGIU"].ToString().Trim();
            }
            return datiAzienda;
        }

        public GestioneAziendeWebOCM.RapLeg CaricaDatiDettagliRapLegale(
          int CodiceAziendaWeb)
        {
            DataTable dataTable = new DataLayer().GetDataTable("SELECT PROREC, EMAILCERT, CELL, DATINI, DATFIN, CODFUNRAP, RAPPRI," + " (SELECT DENDUG FROM DUG WHERE CODDUG = A.CODDUG) AS DENDUG," + " (SELECT DENCOM FROM CODCOM WHERE CODCOM = A.CODCOMNAS) AS DENCOMNAS," + " (SELECT DENCOM FROM CODCOM WHERE CODCOM = A.CODCOMRES) AS DENCOM," + " (SELECT SIGPRO FROM CODCOM WHERE CODCOM = A.CODCOMNAS) AS SIGPRONAS," + " (SELECT DENFUNRAP FROM FUNRAP WHERE CODFUNRAP = A.CODFUNRAP) AS DENFUNRAP, " + "(SELECT CODLOC FROM CODLOC WHERE CODCOM = A.CODCOMRES AND DENLOC = A.DENLOC ORDER BY ULTAGG DESC LIMIT 1) AS CODLOC, " + " COG, NOM, CODDUG, IND," + " NUMCIV, DENSTAEST, DENLOC, CAP, SIGPRO," + " TEL1, TEL2, FAX, EMAIL, CODFIS," + " DATNAS, CODCOMNAS, SES, ULTAGG, UTEAGG," + " CODCOMRES FROM AZIRAPWEB A" + " WHERE CODICE = " + CodiceAziendaWeb.ToString());
            GestioneAziendeWebOCM.RapLeg rapLeg = new GestioneAziendeWebOCM.RapLeg();
            if (dataTable.Rows.Count > 0)
            {
                rapLeg.TipoRapp = dataTable.Rows[0]["DENFUNRAP"].ToString().Trim();
                rapLeg.DataDecInc = dataTable.Rows[0]["DATINI"].ToString().Trim();
                rapLeg.Nome = dataTable.Rows[0]["NOM"].ToString().Trim();
                rapLeg.Cognome = dataTable.Rows[0]["COG"].ToString().Trim();
                rapLeg.DataNascita = DBMethods.Db2Date(dataTable.Rows[0]["DATNAS"].ToString());
                rapLeg.ComuneNascita = dataTable.Rows[0]["DENCOMNAS"].ToString().Trim();
                rapLeg.CodiceFiscale = dataTable.Rows[0]["CODFIS"].ToString().Trim();
                rapLeg.Sesso = dataTable.Rows[0]["SES"].ToString().Trim();
                rapLeg.ProvinciaNas = dataTable.Rows[0]["SIGPRONAS"].ToString().Trim();
                rapLeg.Provincia = dataTable.Rows[0]["SIGPRO"].ToString().Trim();
                rapLeg.StatoEstero = dataTable.Rows[0]["DENSTAEST"].ToString().Trim() == null ? dataTable.Rows[0]["DENSTAEST"].ToString().Trim() : "-";
                rapLeg.TipoVia = dataTable.Rows[0]["DENDUG"].ToString().Trim();
                rapLeg.Indirizzo = dataTable.Rows[0]["IND"].ToString().Trim();
                rapLeg.Civico = dataTable.Rows[0]["NUMCIV"].ToString().Trim();
                rapLeg.Comune = dataTable.Rows[0]["DENCOM"].ToString().Trim();
                rapLeg.Localita = dataTable.Rows[0]["DENLOC"].ToString().Trim();
                rapLeg.Civico = dataTable.Rows[0]["NUMCIV"].ToString().Trim();
                rapLeg.Telefono1 = dataTable.Rows[0]["TEL1"].ToString().Trim();
                rapLeg.Telefono2 = dataTable.Rows[0]["TEL2"].ToString().Trim();
                rapLeg.Cell = dataTable.Rows[0]["CELL"].ToString().Trim();
                rapLeg.Fax = dataTable.Rows[0]["FAX"].ToString().Trim();
                rapLeg.Email = dataTable.Rows[0]["EMAIL"].ToString().Trim();
                rapLeg.EmailCert = dataTable.Rows[0]["EMAILCERT"].ToString().Trim();
                rapLeg.RapLegPrinc = dataTable.Rows[0]["RAPPRI"].ToString().Trim();
                rapLeg.CodiceComuneResidenza = dataTable.Rows[0]["CODCOMRES"].ToString().Trim();
                rapLeg.CodiceComuneNascita = dataTable.Rows[0]["CODCOMNAS"].ToString().Trim();
                rapLeg.CodiceLocalita = dataTable.Rows[0]["CODLOC"].ToString().Trim();
                rapLeg.CAP = dataTable.Rows[0]["CAP"].ToString().Trim();
            }
            return rapLeg;
        }

        public GestioneAziendeWebOCM.AltriDati AltriDati(int CodiceAziendaWeb)
        {
            string strSQL1 = "SELECT A.DATINI, A.CATATTCAM, " + "  A.CODATTCAM, t.DENATTCAM, c.DENCATATT " + " FROM AZIATTWEB A, TIPATT t, CATATT c" + " WHERE CODICE =" + CodiceAziendaWeb.ToString() + " AND t.CODATTCAM = A.CODATTCAM AND c.CATATTCAM = A.CATATTCAM ";
            DataLayer dataLayer = new DataLayer();
            DataTable dataTable1 = dataLayer.GetDataTable(strSQL1);
            GestioneAziendeWebOCM.AltriDati altriDati = new GestioneAziendeWebOCM.AltriDati();
            if (dataTable1.Rows.Count > 0)
            {
                altriDati.DataDecorrenza = DBMethods.Db2Date(dataTable1.Rows[0]["DATINI"].ToString());
                altriDati.CategoriaAttivita = dataTable1.Rows[0]["DENCATATT"].ToString().Trim();
                altriDati.TipoAttivita = dataTable1.Rows[0]["DENATTCAM"].ToString().Trim();
                altriDati.CodiceCategoriaAttivita = dataTable1.Rows[0]["CATATTCAM"].ToString().Trim();
                altriDati.CodiceTipoAttivita = dataTable1.Rows[0]["CODATTCAM"].ToString().Trim();
            }
            string strSQL2 = " SELECT A.DATINI, A.CODSTACON, A.TIPISC, " + " A.ABB, A.ABBDEFMAT, c.DENCODSTA " + " FROM AZISTOWEB A, CODSTA c" + " WHERE CODICE =" + CodiceAziendaWeb.ToString() + " AND A.CODSTACON = c.CODSTACON";
            DataTable dataTable2 = dataLayer.GetDataTable(strSQL2);
            if (dataTable2.Rows.Count > 0)
            {
                altriDati.TipoIscrizione = dataTable2.Rows[0]["TIPISC"].ToString().Trim();
                altriDati.CodiceStatistico = dataTable2.Rows[0]["CODSTACON"].ToString().Trim() + " " + dataTable2.Rows[0]["DENCODSTA"].ToString().Trim();
                altriDati.AbbonamentoPA = "N";
                altriDati.AbbonamentoPAiscrizione = "N";
            }
            return altriDati;
        }

        public GestioneAziendeWebOCM.Documenti Documenti(int CodiceAziendaWeb)
        {
            DataTable dataTable = new DataLayer().GetDataTable("SELECT FLGCCIAA, FLGSTAT, FLGLEG, FLGIVA, FLGDM80, FLGTUT, FLGDEL FROM AZIWEB" + " WHERE CODICE = " + CodiceAziendaWeb.ToString());
            GestioneAziendeWebOCM.Documenti documenti = new GestioneAziendeWebOCM.Documenti();
            if (dataTable.Rows.Count > 0)
            {
                documenti.CCIAA = dataTable.Rows[0]["FLGCCIAA"].ToString().Trim() == "S";
                documenti.Statuto = dataTable.Rows[0]["FLGSTAT"].ToString().Trim() == "S";
                documenti.Legalerapp = dataTable.Rows[0]["FLGLEG"].ToString().Trim() == "S";
                documenti.partitaIVA = dataTable.Rows[0]["FLGIVA"].ToString().Trim() == "S";
                documenti.DM80 = dataTable.Rows[0]["FLGDM80"].ToString().Trim() == "S";
                documenti.privacy = dataTable.Rows[0]["FLGTUT"].ToString().Trim() == "S";
                documenti.delega = dataTable.Rows[0]["FLGDEL"].ToString().Trim() == "S";
            }
            return documenti;
        }

        public List<GestioneAziendeWebOCM.Localita> GetLocalita()
        {
            DataTable dataTable = new DataLayer().GetDataTable("SELECT CODCOM, DENLOC, CODLOC, CAP FROM CODLOC ORDER BY DENLOC ");
            List<GestioneAziendeWebOCM.Localita> localita = new List<GestioneAziendeWebOCM.Localita>();
            foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
                localita.Add(new GestioneAziendeWebOCM.Localita()
                {
                    CODCOM = row["CODCOM"].ToString(),
                    DENLOC = row["DENLOC"].ToString(),
                    CODLOC = row["CODLOC"].ToString(),
                    CAP = row["CAP"].ToString()
                });
            return localita;
        }

        public GestioneAziendeWebOCM.SedeLegale IndirizzoSedeLegale(
          int CodiceAziendaWeb)
        {
            DataTable dataTable = new DataLayer().GetDataTable(" SELECT i.DATINI, d.DENDUG, i.IND, i.NUMCIV,i.CAP,i.SIGPRO, i.CODDUG, " + "(SELECT DENCOM FROM CODCOM WHERE CODCOM = i.CODCOM) AS DENCOM, i.CODCOM, i.DENLOC, i.DENSTAEST, i.TEL1, i.TEL2, i.CELL, i.FAX, i.EMAIL, i.EMAILCERT, " + "(SELECT CODLOC FROM CODLOC WHERE CODCOM = i.CODCOM AND DENLOC = i.DENLOC ORDER BY ULTAGG DESC LIMIT 1) AS CODLOC FROM INDSEDWEB i, DUG d " + " WHERE CODICE = " + CodiceAziendaWeb.ToString() + " AND d.CODDUG = i.CODDUG " + " AND TIPIND = 1");
            GestioneAziendeWebOCM.SedeLegale sedeLegale = new GestioneAziendeWebOCM.SedeLegale();
            if (dataTable.Rows.Count > 0)
            {
                sedeLegale.DataDecorrenza = DBMethods.Db2Date(dataTable.Rows[0]["DATINI"].ToString());
                sedeLegale.TipoIndirizzo = dataTable.Rows[0]["DENDUG"].ToString().Trim();
                sedeLegale.Indirizzo = dataTable.Rows[0]["IND"].ToString().Trim();
                sedeLegale.Civico = dataTable.Rows[0]["NUMCIV"].ToString().Trim();
                sedeLegale.Comune = dataTable.Rows[0]["DENCOM"].ToString().Trim();
                sedeLegale.Localita = dataTable.Rows[0]["DENLOC"].ToString().Trim();
                sedeLegale.StatoEstero = dataTable.Rows[0]["DENSTAEST"].ToString().Trim();
                sedeLegale.Telefono1 = dataTable.Rows[0]["TEL1"].ToString().Trim();
                sedeLegale.Telefono2 = dataTable.Rows[0]["TEL2"].ToString().Trim();
                sedeLegale.Cellulare = dataTable.Rows[0]["CELL"].ToString().Trim();
                sedeLegale.Fax = dataTable.Rows[0]["FAX"].ToString().Trim();
                sedeLegale.Email = dataTable.Rows[0]["EMAIL"].ToString().Trim();
                sedeLegale.EmailCert = dataTable.Rows[0]["EMAILCERT"].ToString().Trim();
                sedeLegale.CAP = dataTable.Rows[0]["CAP"].ToString().Trim();
                sedeLegale.Provincia = dataTable.Rows[0]["SIGPRO"].ToString().Trim();
                sedeLegale.CodiceTipoIndirizzo = dataTable.Rows[0]["CODDUG"].ToString().Trim();
                sedeLegale.CodiceComune = dataTable.Rows[0]["CODCOM"].ToString().Trim();
                sedeLegale.CodiceLocalita = dataTable.Rows[0]["CODLOC"].ToString().Trim();
            }
            return sedeLegale;
        }

        public GestioneAziendeWebOCM.SedeAmministrativa IndirizzoSedeAmm(
          int CodiceAziendaWeb)
        {
            DataTable dataTable = new DataLayer().GetDataTable(" SELECT i.DATINI, d.DENDUG, i.IND, i.NUMCIV,i.CAP,i.SIGPRO, i.CODDUG, i.CODCOM, " + "(SELECT DENCOM FROM CODCOM WHERE CODCOM = i.CODCOM) AS DENCOM, i.DENLOC, i.DENSTAEST, i.TEL1, i.TEL2, i.CELL, i.FAX, i.EMAIL, i.EMAILCERT, " + "(SELECT CODLOC FROM CODLOC WHERE CODCOM = i.CODCOM AND DENLOC = i.DENLOC ORDER BY ULTAGG DESC LIMIT 1) AS CODLOC  FROM INDSEDWEB i, DUG d" + " WHERE CODICE = " + CodiceAziendaWeb.ToString() + " AND d.CODDUG = i.CODDUG " + " AND TIPIND = 3");
            GestioneAziendeWebOCM.SedeAmministrativa sedeAmministrativa = new GestioneAziendeWebOCM.SedeAmministrativa();
            if (dataTable.Rows.Count > 0)
            {
                sedeAmministrativa.DataDecorrenza = DBMethods.Db2Date(dataTable.Rows[0]["DATINI"].ToString());
                sedeAmministrativa.TipoIndirizzo = dataTable.Rows[0]["DENDUG"].ToString().Trim();
                sedeAmministrativa.Indirizzo = dataTable.Rows[0]["IND"].ToString().Trim();
                sedeAmministrativa.Civico = dataTable.Rows[0]["NUMCIV"].ToString().Trim();
                sedeAmministrativa.Comune = dataTable.Rows[0]["DENCOM"].ToString().Trim();
                sedeAmministrativa.Localita = dataTable.Rows[0]["DENLOC"].ToString().Trim();
                sedeAmministrativa.StatoEstero = dataTable.Rows[0]["DENSTAEST"].ToString().Trim();
                sedeAmministrativa.Telefono1 = dataTable.Rows[0]["TEL1"].ToString().Trim();
                sedeAmministrativa.Telefono2 = dataTable.Rows[0]["TEL2"].ToString().Trim();
                sedeAmministrativa.Cellulare = dataTable.Rows[0]["CELL"].ToString().Trim();
                sedeAmministrativa.Fax = dataTable.Rows[0]["FAX"].ToString().Trim();
                sedeAmministrativa.Email = dataTable.Rows[0]["EMAIL"].ToString().Trim();
                sedeAmministrativa.EmailCert = dataTable.Rows[0]["EMAILCERT"].ToString().Trim();
                sedeAmministrativa.CAP = dataTable.Rows[0]["CAP"].ToString().Trim();
                sedeAmministrativa.Provincia = dataTable.Rows[0]["SIGPRO"].ToString().Trim();
                sedeAmministrativa.CodiceTipoIndirizzo = dataTable.Rows[0]["CODDUG"].ToString().Trim();
                sedeAmministrativa.CodiceComune = dataTable.Rows[0]["CODCOM"].ToString().Trim();
                sedeAmministrativa.CodiceLocalita = dataTable.Rows[0]["CODLOC"].ToString().Trim();
            }
            return sedeAmministrativa;
        }

        public GestioneAziendeWebOCM.IndirizzoCorrispondenza IndirizzoCorrisp(
          int CodiceAziendaWeb)
        {
            DataTable dataTable = new DataLayer().GetDataTable(" SELECT i.DATINI, d.DENDUG, i.IND, i.NUMCIV,i.CAP,i.SIGPRO, i.CODDUG, i.CODCOM, " + "(SELECT DENCOM FROM CODCOM WHERE CODCOM = i.CODCOM) AS DENCOM, i.DENLOC, i.DENSTAEST, i.TEL1, i.TEL2, i.CELL, i.FAX, i.EMAIL, i.EMAILCERT, " + "(SELECT CODLOC FROM CODLOC WHERE CODCOM = i.CODCOM AND DENLOC = i.DENLOC ORDER BY ULTAGG DESC LIMIT 1) AS CODLOC FROM INDSEDWEB i, DUG d" + " WHERE CODICE = " + CodiceAziendaWeb.ToString() + " AND d.CODDUG = i.CODDUG " + " AND TIPIND = 3");
            GestioneAziendeWebOCM.IndirizzoCorrispondenza indirizzoCorrispondenza = new GestioneAziendeWebOCM.IndirizzoCorrispondenza();
            if (dataTable.Rows.Count > 0)
            {
                indirizzoCorrispondenza.Destinatario = this.CaricaDatiDettagliAzienda(CodiceAziendaWeb).RagioneSociale;
                indirizzoCorrispondenza.DataDecorrenza = DBMethods.Db2Date(dataTable.Rows[0]["DATINI"].ToString());
                indirizzoCorrispondenza.TipoIndirizzo = dataTable.Rows[0]["DENDUG"].ToString().Trim();
                indirizzoCorrispondenza.Indirizzo = dataTable.Rows[0]["IND"].ToString().Trim();
                indirizzoCorrispondenza.Civico = dataTable.Rows[0]["NUMCIV"].ToString().Trim();
                indirizzoCorrispondenza.Comune = dataTable.Rows[0]["DENCOM"].ToString().Trim();
                indirizzoCorrispondenza.Localita = dataTable.Rows[0]["DENLOC"].ToString().Trim();
                indirizzoCorrispondenza.StatoEstero = dataTable.Rows[0]["DENSTAEST"].ToString().Trim();
                indirizzoCorrispondenza.Telefono1 = dataTable.Rows[0]["TEL1"].ToString().Trim();
                indirizzoCorrispondenza.Telefono2 = dataTable.Rows[0]["TEL2"].ToString().Trim();
                indirizzoCorrispondenza.Cellulare = dataTable.Rows[0]["CELL"].ToString().Trim();
                indirizzoCorrispondenza.Fax = dataTable.Rows[0]["FAX"].ToString().Trim();
                indirizzoCorrispondenza.Email = dataTable.Rows[0]["EMAIL"].ToString().Trim();
                indirizzoCorrispondenza.EmailCert = dataTable.Rows[0]["EMAILCERT"].ToString().Trim();
                indirizzoCorrispondenza.CAP = dataTable.Rows[0]["CAP"].ToString().Trim();
                indirizzoCorrispondenza.Provincia = dataTable.Rows[0]["SIGPRO"].ToString().Trim();
                indirizzoCorrispondenza.CodiceTipoIndirizzo = dataTable.Rows[0]["CODDUG"].ToString().Trim();
                indirizzoCorrispondenza.CodiceComune = dataTable.Rows[0]["CODCOM"].ToString().Trim();
                indirizzoCorrispondenza.CodiceLocalita = dataTable.Rows[0]["CODLOC"].ToString().Trim();
            }
            return indirizzoCorrispondenza;
        }

        public GestioneAziendeWebOCM Update(
          GestioneAziendeWebOCM gestione,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMSG,
          ref string SuccessMSG)
        {
            DataLayer dataLayer1 = new DataLayer();
            string str1 = HttpContext.Current.Request.Form["NaturaGiuridica"];
            string str2 = HttpContext.Current.Request.Form["MezziComunicazione"];
            string str3 = HttpContext.Current.Request.Form["TipoRapLeg"];
            string str4 = HttpContext.Current.Request.Form["RapLegprincipale"];
            string str5 = HttpContext.Current.Request.Form["TipoIndirizzo"];
            string str6 = HttpContext.Current.Request.Form["RapLegSesso"];
            string str7 = HttpContext.Current.Request.Form["codComune"];
            string str8 = HttpContext.Current.Request.Form["codCategoriaAttivita"];
            string str9 = HttpContext.Current.Request.Form["codTipoAttivita"];
            string str10 = HttpContext.Current.Request.Form["codTipoIscrizione"];
            string str11 = HttpContext.Current.Request.Form["codCodiceStatistico"];
            DataLayer dataLayer2 = new DataLayer();
            iDB2Connection Conn = new iDB2Connection();
            iDB2Command cmd = new iDB2Command();
            iDB2DataAdapter da = new iDB2DataAdapter();
            string str12 = "1";
            DataTable dataTable1 = new DataTable();
            DataTable DT_SEDI_OLD = new DataTable();
            DataTable dataTable2 = new DataTable();
            string str13 = "S";
            string str14 = "";
            string str15 = "U";
            try
            {
                dataLayer1.StartTransaction();
                string dataSistema = this.Module_GetDataSistema();
                int CODPOS;
                if (gestione.datiAzienda.CodicePosizioneAzienda == null)
                {
                    string strSQL1 = "SELECT DISTINCT MAX(CODPOS) FROM AZI WHERE CODPOS < 900000";
                    CODPOS = Convert.ToInt32((object)dataLayer1.Get1ValueFromSQL(strSQL1, CommandType.Text)) + 1;
                    if (gestione.datiAzienda.RagioneSocialeBreve == null)
                        gestione.datiAzienda.RagioneSocialeBreve = gestione.datiAzienda.RagioneSociale;
                    if (gestione.datiAzienda.URL == null)
                        gestione.datiAzienda.URL = "";
                    if (gestione.datiAzienda.DataApertura == null)
                        gestione.datiAzienda.DataApertura = gestione.datiAzienda.DataDenunciaApertura;
                    if (gestione.datiAzienda.RagioneSociale == null)
                        gestione.datiAzienda.RagioneSociale = "";
                    string str16 = "INSERT INTO AZI(CODPOS, RAGSOC, RAGSOCBRE, PARIVA, CODFIS, NATGIU, URL," + " DATAPE,AZINOTC,OLDTIPAZI,INSAZI, CODMEZ, " + " FLGAPP, ULTAGG, UTEAGG, DATDENAPE)" + "VALUES( " + CODPOS.ToString() + ", " + DBMethods.DoublePeakForSql(gestione.datiAzienda.RagioneSociale.Trim()) + ", " + (!string.IsNullOrEmpty(gestione.datiAzienda.RagioneSocialeBreve) ? DBMethods.DoublePeakForSql(gestione.datiAzienda.RagioneSocialeBreve.Trim()) + ", " : DBMethods.DoublePeakForSql(string.Empty) + ", ") + gestione.datiAzienda.PartitaIva.Trim() + ", " + gestione.datiAzienda.CodiceFiscale.Trim() + ", " + gestione.datiAzienda.NaturaGiuridica.Trim() + ", " + (!string.IsNullOrEmpty(gestione.datiAzienda.URL) ? gestione.datiAzienda.URL.Trim() + ", " : DBMethods.DoublePeakForSql(string.Empty) + ", ") + DBMethods.DoublePeakForSql(DBMethods.Db2Date(gestione.datiAzienda.DataApertura.Trim())) + ", " + (!string.IsNullOrEmpty(gestione.datiAzienda.Note) ? gestione.datiAzienda.Note.Trim() + ", " : DBMethods.DoublePeakForSql(string.Empty) + ", ") + str12 + ", " + DBMethods.DoublePeakForSql(str15) + ", ";
                    string strSQL2 = (string.IsNullOrEmpty(str2.ToString().Trim()) ? str16 + "Null, " : str16 + str2.ToString().Trim() + ", ") + " 'I', CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ", " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(gestione.datiAzienda.DataDenunciaApertura)) + ") ";
                    dataLayer1.WriteTransactionData(strSQL2, CommandType.Text);
                    for (int index = 1; index <= 3; ++index)
                    {
                        switch (index - 1)
                        {
                            case 0:
                                str14 = "SEDLEG";
                                break;
                            case 1:
                                str14 = "RECPOS";
                                break;
                            case 2:
                                str14 = "SEDAMM";
                                break;
                        }
                        if (index == 1)
                        {
                            if (gestione.datiAzienda.DataComunicazione == null)
                                gestione.datiAzienda.DataComunicazione = DBMethods.DoublePeakForSql(DBMethods.Db2Date(gestione.datiAzienda.DataApertura));
                            if (gestione.sedeLegale.StatoEstero == null)
                                gestione.sedeLegale.StatoEstero = "ITALIA";
                            if (gestione.sedeLegale.Cellulare == null)
                                gestione.sedeLegale.Cellulare = "";
                            if (gestione.sedeLegale.Telefono1 == null)
                                gestione.sedeLegale.Telefono1 = "";
                            if (gestione.sedeLegale.Telefono2 == null)
                                gestione.sedeLegale.Telefono2 = "";
                            if (gestione.sedeLegale.Fax == null)
                                gestione.sedeLegale.Fax = "";
                        }
                        if (index == 3)
                        {
                            if (gestione.datiAzienda.DataComunicazione == null)
                                gestione.datiAzienda.DataComunicazione = DBMethods.DoublePeakForSql(DBMethods.Db2Date(gestione.datiAzienda.DataApertura));
                            if (gestione.sedeAmministrativa.StatoEstero == null)
                                gestione.sedeAmministrativa.StatoEstero = "ITALIA";
                            if (gestione.sedeAmministrativa.Cellulare == null)
                                gestione.sedeAmministrativa.Cellulare = "";
                            if (gestione.sedeAmministrativa.Telefono1 == null)
                                gestione.sedeAmministrativa.Telefono1 = "";
                            if (gestione.sedeAmministrativa.Telefono2 == null)
                                gestione.sedeAmministrativa.Telefono2 = "";
                            if (gestione.sedeAmministrativa.Fax == null)
                                gestione.sedeAmministrativa.Fax = "";
                        }
                        if (index == 2)
                        {
                            if (gestione.datiAzienda.DataComunicazione == null)
                                gestione.datiAzienda.DataComunicazione = DBMethods.DoublePeakForSql(DBMethods.Db2Date(gestione.datiAzienda.DataApertura));
                            if (gestione.indirizzoCorrispondenza.StatoEstero == null)
                                gestione.indirizzoCorrispondenza.StatoEstero = "ITALIA";
                            if (gestione.indirizzoCorrispondenza.Cellulare == null)
                                gestione.indirizzoCorrispondenza.Cellulare = "";
                            if (gestione.indirizzoCorrispondenza.Telefono1 == null)
                                gestione.indirizzoCorrispondenza.Telefono1 = "";
                            if (gestione.indirizzoCorrispondenza.Telefono2 == null)
                                gestione.indirizzoCorrispondenza.Telefono2 = "";
                            if (gestione.indirizzoCorrispondenza.Fax == null)
                                gestione.indirizzoCorrispondenza.Fax = "";
                        }
                        string str17 = "INSERT INTO INDSED(CODPOS,PROREC,DATCOM,CODMEZ,TIPIND,DATINI,DATFIN,CODDUG, " + " IND,NUMCIV,DENSTAEST,AGGMAN, CELL, DENLOC, CAP, SIGPRO, " + " TEL1,TEL2,FAX, EMAIL, EMAILCERT, ";
                        if (str14 == "RECPOS")
                            str17 += " DESTINAT, ";
                        string str18 = str17 + " ULTAGG, UTEAGG, CODCOM) " + " VALUES( " + CODPOS.ToString() + ", " + " (SELECT VALUE(MAX(PROREC), 0) + 1 FROM INDSED WHERE CODPOS=" + CODPOS.ToString() + "), " + gestione.datiAzienda.DataComunicazione + ", ";
                        string str19 = (string.IsNullOrEmpty(str2.ToString().Trim()) ? str18 + str2.ToString().Trim() + ", " : str18 + " null,") + index.ToString() + ", " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(gestione.datiAzienda.DataApertura)) + ", " + "'9999-12-31', ";
                        switch (index)
                        {
                            case 1:
                                str19 = str19 + gestione.sedeLegale.TipoIndirizzo + ", " + DBMethods.DoublePeakForSql(gestione.sedeLegale.Indirizzo) + ", " + DBMethods.DoublePeakForSql(gestione.sedeLegale.Civico) + ", " + DBMethods.DoublePeakForSql(gestione.sedeLegale.StatoEstero) + ", " + DBMethods.DoublePeakForSql(str13) + ", " + DBMethods.DoublePeakForSql(gestione.sedeLegale.Cellulare) + ", " + DBMethods.DoublePeakForSql(gestione.sedeLegale.Localita) + ", " + DBMethods.DoublePeakForSql(gestione.sedeLegale.CAP) + ", " + DBMethods.DoublePeakForSql(gestione.sedeLegale.Provincia) + ", " + DBMethods.DoublePeakForSql(gestione.sedeLegale.Telefono1) + ", " + DBMethods.DoublePeakForSql(gestione.sedeLegale.Telefono2) + ", " + DBMethods.DoublePeakForSql(gestione.sedeLegale.Fax) + ", " + DBMethods.DoublePeakForSql(gestione.sedeLegale.Email ?? "") + ", " + DBMethods.DoublePeakForSql(gestione.sedeLegale.EmailCert ?? "") + ", ";
                                break;
                            case 2:
                                str19 = str19 + gestione.indirizzoCorrispondenza.TipoIndirizzo + ", " + DBMethods.DoublePeakForSql(gestione.indirizzoCorrispondenza.Indirizzo) + ", " + DBMethods.DoublePeakForSql(gestione.indirizzoCorrispondenza.Civico) + ", " + DBMethods.DoublePeakForSql(gestione.indirizzoCorrispondenza.StatoEstero) + ", " + DBMethods.DoublePeakForSql(str13) + ", " + DBMethods.DoublePeakForSql(gestione.indirizzoCorrispondenza.Cellulare) + ", " + DBMethods.DoublePeakForSql(gestione.indirizzoCorrispondenza.Localita) + ", " + DBMethods.DoublePeakForSql(gestione.indirizzoCorrispondenza.CAP) + ", " + DBMethods.DoublePeakForSql(gestione.indirizzoCorrispondenza.Provincia) + ", " + DBMethods.DoublePeakForSql(gestione.indirizzoCorrispondenza.Telefono1) + ", " + DBMethods.DoublePeakForSql(gestione.indirizzoCorrispondenza.Telefono2) + ", " + DBMethods.DoublePeakForSql(gestione.indirizzoCorrispondenza.Fax) + ", " + DBMethods.DoublePeakForSql(gestione.indirizzoCorrispondenza.Email ?? "") + ", " + DBMethods.DoublePeakForSql(gestione.indirizzoCorrispondenza.EmailCert ?? "") + ", ";
                                break;
                            case 3:
                                str19 = str19 + gestione.sedeAmministrativa.TipoIndirizzo + ", " + DBMethods.DoublePeakForSql(gestione.sedeAmministrativa.Indirizzo) + ", " + DBMethods.DoublePeakForSql(gestione.sedeAmministrativa.Civico) + ", " + DBMethods.DoublePeakForSql(gestione.sedeAmministrativa.StatoEstero) + ", " + DBMethods.DoublePeakForSql(str13) + ", " + DBMethods.DoublePeakForSql(gestione.sedeAmministrativa.Cellulare) + ", " + DBMethods.DoublePeakForSql(gestione.sedeAmministrativa.Localita) + ", " + DBMethods.DoublePeakForSql(gestione.sedeAmministrativa.CAP) + ", " + DBMethods.DoublePeakForSql(gestione.sedeAmministrativa.Provincia) + ", " + DBMethods.DoublePeakForSql(gestione.sedeAmministrativa.Telefono1) + ", " + DBMethods.DoublePeakForSql(gestione.sedeAmministrativa.Telefono2) + ", " + DBMethods.DoublePeakForSql(gestione.sedeAmministrativa.Fax) + ", " + DBMethods.DoublePeakForSql(gestione.sedeAmministrativa.Email ?? "") + ", " + DBMethods.DoublePeakForSql(gestione.sedeAmministrativa.EmailCert ?? "") + ", ";
                                break;
                        }
                        if (str14 == "RECPOS")
                            str19 = str19 + DBMethods.DoublePeakForSql(gestione.indirizzoCorrispondenza.Destinatario) + ", ";
                        string strSQL3 = str19 + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ", ";
                        switch (index)
                        {
                            case 1:
                                strSQL3 = !(gestione.sedeLegale.Comune == "") ? strSQL3 + " NULL )" : strSQL3 + DBMethods.DoublePeakForSql(gestione.sedeLegale.Comune) + ") ";
                                break;
                            case 2:
                                strSQL3 = !(gestione.indirizzoCorrispondenza.Comune == "") ? strSQL3 + " NULL )" : strSQL3 + DBMethods.DoublePeakForSql(gestione.indirizzoCorrispondenza.Comune) + ") ";
                                break;
                            case 3:
                                strSQL3 = !(gestione.sedeAmministrativa.Comune == "") ? strSQL3 + " NULL )" : strSQL3 + DBMethods.DoublePeakForSql(gestione.sedeAmministrativa.Comune) + ") ";
                                break;
                        }
                        dataLayer1.WriteTransactionData(strSQL3, CommandType.Text);
                    }
                    if (string.IsNullOrEmpty(gestione.datiAzienda.CodiceMezzoComunicazione))
                        gestione.datiAzienda.CodiceMezzoComunicazione = gestione.datiAzienda.MezziComunicazione;
                    if (!string.IsNullOrEmpty(gestione.rapleg.TipoVia))
                        gestione.rapleg.CODDUG = gestione.rapleg.TipoVia;
                    if (gestione.rapleg.RapLegPrinc == null || gestione.rapleg.RapLegPrinc == "")
                    {
                        string strSQL4 = "SELECT RAPPRI FROM AZIRAPWEB A  WHERE COG = '" + gestione.rapleg.Cognome + "' AND NOM = '" + gestione.rapleg.Nome + "' ";
                        gestione.rapleg.RapLegPrinc = dataLayer1.Get1ValueFromSQL(strSQL4, CommandType.Text);
                    }
                    if (gestione.rapleg.Telefono2 == null)
                        gestione.rapleg.Telefono2 = "";
                    if (gestione.rapleg.Fax == null)
                        gestione.rapleg.Fax = "";
                    if (gestione.rapleg.Cell == null)
                        gestione.rapleg.Cell = "";
                    string str20 = "INSERT INTO AZIRAP (CODPOS,PROREC,DATCOM,CODMEZ,DATINI,DATFIN,CODFUNRAP," + "RAPPRI,COG,NOM, CODDUG, IND, NUMCIV, DENSTAEST, DENLOC, " + "CAP,SIGPRO,TEL1,TEL2,FAX, EMAIL, CODFIS,DATNAS," + " CODCOMNAS, SES, ULTAGG, UTEAGG, CELL, EMAILCERT, CODCOMRES)" + " VALUES( " + CODPOS.ToString() + ", " + " (SELECT VALUE(MAX(PROREC), 0) + 1 FROM AZIRAP WHERE CODPOS=" + CODPOS.ToString() + "), " + gestione.datiAzienda.DataComunicazione + ", " + gestione.datiAzienda.CodiceMezzoComunicazione + ", " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(gestione.datiAzienda.DataApertura)) + ", " + "'9999-12-31', " + gestione.rapleg.TipoRapp + ", " + DBMethods.DoublePeakForSql(gestione.rapleg.RapLegPrinc) + ", " + DBMethods.DoublePeakForSql(gestione.rapleg.Cognome) + ", " + DBMethods.DoublePeakForSql(gestione.rapleg.Nome) + ", " + DBMethods.DoublePeakForSql(gestione.rapleg.CODDUG) + ", " + DBMethods.DoublePeakForSql(gestione.rapleg.Indirizzo) + ", " + DBMethods.DoublePeakForSql(gestione.rapleg.Civico) + ", ";
                    string str21 = (string.IsNullOrEmpty(gestione.rapleg.StatoEstero) ? str20 + " NULL, " : str20 + DBMethods.DoublePeakForSql(gestione.rapleg.StatoEstero) + ", ") + DBMethods.DoublePeakForSql(gestione.rapleg.Localita) + ", " + DBMethods.DoublePeakForSql(gestione.rapleg.CAP) + ", " + DBMethods.DoublePeakForSql(gestione.rapleg.Provincia) + ", " + DBMethods.DoublePeakForSql(gestione.rapleg.Telefono1 ?? "") + ", " + DBMethods.DoublePeakForSql(gestione.rapleg.Telefono2) + ", " + DBMethods.DoublePeakForSql(gestione.rapleg.Fax) + ", " + DBMethods.DoublePeakForSql(gestione.rapleg.Email ?? "") + ", " + DBMethods.DoublePeakForSql(gestione.rapleg.CodiceFiscale) + ", " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(gestione.rapleg.DataNascita)) + ", ";
                    string str22 = (string.IsNullOrEmpty(gestione.rapleg.StatoEsteroNascita) ? str21 + DBMethods.DoublePeakForSql(gestione.rapleg.ComuneNascita) + ", " : str21 + DBMethods.DoublePeakForSql(gestione.rapleg.StatoEsteroNascita) + ", ") + DBMethods.DoublePeakForSql(gestione.rapleg.Sesso) + ", " + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ", " + DBMethods.DoublePeakForSql(gestione.rapleg.Cell) + ", " + DBMethods.DoublePeakForSql(gestione.rapleg.EmailCert ?? "") + ", ";
                    string strSQL5 = string.IsNullOrEmpty(gestione.rapleg.CodiceComuneResidenza) ? str22 + " NULL )" : str22 + DBMethods.DoublePeakForSql(gestione.rapleg.CodiceComuneResidenza) + ") ";
                    dataLayer1.WriteTransactionData(strSQL5, CommandType.Text);
                    string str23 = "INSERT INTO AZIATT(CODPOS,PROREC,DATCOM,CODMEZ, CATATTCAM, CODATTCAM, DATINI,DATFIN," + "ULTAGG,UTEAGG)" + "VALUES( " + CODPOS.ToString() + ", " + " (SELECT VALUE(MAX(PROREC), 0) + 1 FROM AZIATT WHERE CODPOS=" + CODPOS.ToString() + "), " + gestione.datiAzienda.DataComunicazione + ", ";
                    string strSQL6 = (!string.IsNullOrEmpty(gestione.datiAzienda.CodiceMezzoComunicazione) ? str23 + gestione.datiAzienda.CodiceMezzoComunicazione + ", " : str23 + " null,") + gestione.altridati.CategoriaAttivita + ", " + DBMethods.DoublePeakForSql(gestione.altridati.TipoAttivita) + ", " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(gestione.altridati.DataDecorrenza.Substring(0, 10))) + ", " + "'9999-12-31', " + "CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ")";
                    dataLayer1.WriteTransactionData(strSQL6, CommandType.Text);
                    if (gestione.altridati.TipoIscrizione == null)
                        gestione.altridati.TipoIscrizione = "NULL";
                    string str24 = "INSERT INTO AZISTO(CODPOS,PROREC,DATCOM,CODMEZ,DATINI,DATFIN,ABB,ABBDEFMAT,TIPISC," + "CODSTACON,ULTAGG,UTEAGG)" + "VALUES( " + CODPOS.ToString() + ", " + " (SELECT VALUE(MAX(PROREC), 0) + 1 FROM AZISTO WHERE CODPOS=" + CODPOS.ToString() + "), " + gestione.datiAzienda.DataComunicazione + ", ";
                    string strSQL7 = (!(gestione.datiAzienda.CodiceMezzoComunicazione == "") ? str24 + gestione.datiAzienda.CodiceMezzoComunicazione + ", " : str24 + " null,") + DBMethods.DoublePeakForSql(DBMethods.Db2Date(gestione.datiAzienda.DataApertura)) + ", " + " '9999-12-31', " + " 'N', 'N'," + DBMethods.DoublePeakForSql(gestione.altridati.TipoIscrizione) + ", " + gestione.altridati.CodiceStatistico + ", " + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ")";
                    dataLayer1.WriteTransactionData(strSQL7, CommandType.Text);
                    string str25 = CODPOS.ToString().PadLeft(8, '0');
                    string str26 = "INSERT INTO UTENTI (CODUTE, DENUTE,  CODTIPUTE, CODFIS, ULTAGG , UTEAGG)" + " VALUES ( " + DBMethods.DoublePeakForSql(str25) + ", " + DBMethods.DoublePeakForSql(gestione.datiAzienda.RagioneSociale) + ", " + "'A', ";
                    string strSQL8 = (string.IsNullOrEmpty(gestione.datiAzienda.PartitaIva) ? str26 + DBMethods.DoublePeakForSql(gestione.datiAzienda.CodiceFiscale) + ", " : str26 + DBMethods.DoublePeakForSql(gestione.datiAzienda.PartitaIva) + ", ") + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ")";
                    dataLayer1.WriteTransactionData(strSQL8, CommandType.Text);
                    string strPassword = this.GeneraPWD();
                    string strSQL9 = "INSERT INTO UTEPIN (CODUTE, PIN, DATINI, DATFIN, STAPIN, ULTAGG, UTEAGG) " + " VALUES (" + DBMethods.DoublePeakForSql(str25) + ", " + DBMethods.DoublePeakForSql(Cypher.CryptPassword(strPassword)) + ", " + " CURRENT_DATE, " + "'2000-12-31', " + "'A', " + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ")";
                    dataLayer1.WriteTransactionData(strSQL9, CommandType.Text);
                    new Utile().Invia_Email((string)null, "<TABLE>" + "<DIV ALIGN=CENTER><P ALIGN=CENTER>" + " <img src = 'http://www.enpaia.it/logoEnpaia.png'><br><br><br>" + "</P></DIV><DIV ALIGN=JUSTIFY><P ALIGN=JUSTIFY><FONT FACE='ARIAL' SIZE=2>" + "A seguito della sua richiesta di registrazione sul portale Enpaia, si comunica la prima parte del PIN per l’accesso ai servizi on line:<br><BR></FONT>" + "<FONT FACE='ARIAL' SIZE=3><B>" + strPassword + "</B></FONT><BR>BR>" + "<FONT FACE='ARIAL' SIZE=2>La seconda parte verrà spedita all’ indirizzo di residenza.<BR><BR>Cordiali saluti." + "<BR><BR><br>" + "</FONT></P></DIV>" + "<DIV ALIGN=right><P ALIGN=right><FONT FACE='ARIAL' SIZE=2>" + "Il Dirigente della Divisione Attività Istituzionali<br>" + "(Avv. Fabio Petrucci)&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br><br>" + "</FONT></P></DIV>" + "</TABLE>" + "<TABLE>" + "<DIV ALIGN=LEFT><FONT FACE='ARIAL' SIZE=1>" + "<BR><P ALIGN=LEFT><i>Le informazioni contenute nella presente comunicazione e i relativi allegati possono essere riservate e sono, comunque, " + "destinate<BR>esclusivamente alle persone o alla Società sopraindicati.La diffusione, distribuzione e/o copiatura del documento trasmesso " + "da parte<BR>di qualsiasi soggetto diverso dal destinatario è proibita, sia ai sensi dell'art. 616 c.p. , che ai sensi del D.Lgs. n. 196/2003.</i>" + "</P></DIV></FONT>" + "</TABLE>", "INVIO PIN AZIENDA  ", "fabio.vecchietti@enpaia.it");
                    string strSQL10 = "INSERT INTO AZIUTE (CODUTE, CODPOS, ULTAGG, UTEAGG) " + " VALUES( " + DBMethods.DoublePeakForSql(str25) + ", " + CODPOS.ToString() + ", " + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ")";
                    dataLayer1.WriteTransactionData(strSQL10, CommandType.Text);
                    string strSQL11 = "INSERT INTO GRUAZI (CODPOS, CODGRU, ULTAGG , UTEAGG)" + " VALUES ( " + CODPOS.ToString() + ", " + " 2, " + " CURRENT_TIMESTAMP, " + DBMethods.DoublePeakForSql(u.Username) + ")";
                    dataLayer1.WriteTransactionData(strSQL11, CommandType.Text);
                }
                else
                {
                    CODPOS = int.Parse(gestione.datiAzienda.CodicePosizioneAzienda);
                    string str27 = "UPDATE AZIWEB SET DATCONF = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataSistema)) + ", " + " UTECONF = " + DBMethods.DoublePeakForSql(u.Username) + ", " + " CODPOSCONF = " + CODPOS.ToString() + ", ";
                    if (!string.IsNullOrEmpty(gestione.datiAzienda.RagioneSociale ?? ""))
                        str27 = str27 + "RAGSOC = " + DBMethods.DoublePeakForSql(gestione.datiAzienda.RagioneSociale ?? "") + ", ";
                    if (!string.IsNullOrEmpty(gestione.datiAzienda.RagioneSocialeBreve ?? ""))
                        str27 = str27 + "RAGSOCBRE = " + DBMethods.DoublePeakForSql(gestione.datiAzienda.RagioneSocialeBreve ?? "") + ", ";
                    if (!string.IsNullOrEmpty(gestione.datiAzienda.PartitaIva ?? ""))
                        str27 = str27 + "PARIVA = '" + gestione.datiAzienda.PartitaIva + "', ";
                    if (!string.IsNullOrEmpty(gestione.datiAzienda.CodiceFiscale ?? ""))
                        str27 = str27 + "CODFIS = '" + gestione.datiAzienda.CodiceFiscale + "', ";
                    if (!string.IsNullOrEmpty(gestione.datiAzienda.NaturaGiuridica ?? ""))
                        str27 = str27 + "NATGIU = " + gestione.datiAzienda.NaturaGiuridica + ", ";
                    if (!string.IsNullOrEmpty(gestione.datiAzienda.URL ?? ""))
                        str27 = str27 + "URL = '" + gestione.datiAzienda.URL + "', ";
                    if (!string.IsNullOrEmpty(gestione.datiAzienda.Note ?? ""))
                        str27 = str27 + "NOTE = '" + gestione.datiAzienda.Note + "', ";
                    if (!string.IsNullOrEmpty(gestione.datiAzienda.CodiceMezzoComunicazione ?? ""))
                        str27 = str27 + "CODMEZ = '" + gestione.datiAzienda.CodiceMezzoComunicazione + "', ";
                    if (!string.IsNullOrEmpty(gestione.datiAzienda.DataApertura ?? ""))
                        str27 = str27 + "DATAPE = '" + DBMethods.Db2Date(gestione.datiAzienda.DataApertura.Trim()) + "', ";
                    if (!string.IsNullOrEmpty(gestione.datiAzienda.DataDenunciaApertura ?? ""))
                        str27 = str27 + "DATDENAPE = '" + DBMethods.Db2Date(gestione.datiAzienda.DataDenunciaApertura.Trim()) + "', ";
                    string str28 = !gestione.documenti.CCIAA ? str27 + " FLGCCIAA = 'N', " : str27 + " FLGCCIAA = 'S', ";
                    string str29 = !gestione.documenti.Statuto ? str28 + " FLGSTAT = 'N', " : str28 + " FLGSTAT = 'S', ";
                    string str30 = !gestione.documenti.Legalerapp ? str29 + " FLGLEG = 'N', " : str29 + " FLGLEG = 'S', ";
                    string str31 = !gestione.documenti.partitaIVA ? str30 + " FLGIVA = 'N', " : str30 + " FLGIVA = 'S', ";
                    string str32 = !gestione.documenti.DM80 ? str31 + " FLGDM80 = 'N', " : str31 + " FLGDM80 = 'S', ";
                    string str33 = !gestione.documenti.privacy ? str32 + " FLGTUT = 'N', " : str32 + " FLGTUT = 'S', ";
                    string strSQL12 = (!gestione.documenti.delega ? str33 + " FLGDEL = 'N', " : str33 + " FLGDEL = 'S', ") + "ULTAGG = CURRENT_TIMESTAMP, UTEAGG = '{u.Username}' WHERE CODPOSCONF = " + gestione.datiAzienda.CodicePosizioneAzienda;
                    dataLayer1.WriteTransactionData(strSQL12, CommandType.Text);
                    string strSQL13 = "UPDATE INDSEDWEB SET DATCONF = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataSistema)) + ", " + " UTECONF = " + DBMethods.DoublePeakForSql(u.Username) + ", " + " CODPOSCONF = '" + CODPOS.ToString() + "' " + " WHERE CODICE = " + gestione.datiAzienda.CodicePosizioneAzienda;
                    dataLayer1.WriteTransactionData(strSQL13, CommandType.Text);
                    string str34 = "UPDATE AZIRAPWEB SET DATCONF = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataSistema)) + ", " + " UTECONF = " + DBMethods.DoublePeakForSql(u.Username) + ", " + " CODPOSCONF = '" + CODPOS.ToString() + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.TipoRapp ?? ""))
                        str34 = str34 + "CODFUNRAP = '" + gestione.rapleg.TipoRapp + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.RapLegPrinc ?? ""))
                        str34 = str34 + "RAPPRI = '" + gestione.rapleg.RapLegPrinc + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Cognome ?? ""))
                        str34 = str34 + "COG = '" + gestione.rapleg.Cognome + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Nome ?? ""))
                        str34 = str34 + "NOM = '" + gestione.rapleg.Nome + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.CODDUG ?? ""))
                        str34 = str34 + "CODDUG = '" + gestione.rapleg.CODDUG + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Indirizzo ?? ""))
                        str34 = str34 + "IND = '" + gestione.rapleg.Indirizzo + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Civico ?? ""))
                        str34 = str34 + "NUMCIV = '" + gestione.rapleg.Civico + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.StatoEstero ?? ""))
                        str34 = str34 + "DENSTAEST = '" + gestione.rapleg.StatoEstero + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Localita ?? ""))
                        str34 = str34 + "DENLOC = '" + gestione.rapleg.Localita + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.CAP ?? ""))
                        str34 = str34 + "CAP = '" + gestione.rapleg.CAP + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Provincia ?? ""))
                        str34 = str34 + "SIGPRO = '" + gestione.rapleg.Provincia + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Telefono1 ?? ""))
                        str34 = str34 + "TEL1 = '" + gestione.rapleg.Telefono1 + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Telefono2 ?? ""))
                        str34 = str34 + "TEL2 = '" + gestione.rapleg.Telefono2 + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Fax ?? ""))
                        str34 = str34 + "FAX = '" + gestione.rapleg.Fax + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Email ?? ""))
                        str34 = str34 + "EMAIL = '" + gestione.rapleg.Email + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.EmailCert ?? ""))
                        str34 = str34 + "EMAILCERT = '" + gestione.rapleg.EmailCert + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.CodiceFiscale ?? ""))
                        str34 = str34 + "CODFIS = '" + gestione.rapleg.CodiceFiscale + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.DataNascita ?? ""))
                        str34 = str34 + "DATNAS = '" + DBMethods.Db2Date(gestione.rapleg.DataNascita) + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.CodiceComuneNascita ?? ""))
                        str34 = str34 + "CODCOMNAS = '" + gestione.rapleg.CodiceComuneNascita + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Sesso ?? ""))
                        str34 = str34 + "SES = '" + gestione.rapleg.Sesso + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Cell ?? ""))
                        str34 = str34 + "CELL = '" + gestione.rapleg.Cell + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.CodiceComuneResidenza ?? ""))
                        str34 = str34 + "CODCOMRES = '" + gestione.rapleg.CodiceComuneResidenza + "', ";
                    string strSQL14 = str34 + "ULTAGG = CURRENT_TIMESTAMP, UTEAGG = '" + u.Username + "' WHERE CODPOSCONF = " + gestione.datiAzienda.CodicePosizioneAzienda;
                    dataLayer1.WriteTransactionData(strSQL14, CommandType.Text);
                    string str35 = "UPDATE AZIATTWEB SET DATCONF = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataSistema)) + ", " + " UTECONF = " + DBMethods.DoublePeakForSql(u.Username) + ", " + " CODPOSCONF = '" + CODPOS.ToString() + "', ";
                    if (!string.IsNullOrEmpty(gestione.altridati.CodiceCategoriaAttivita ?? ""))
                        str35 = str35 + "CATATTCAM = '" + gestione.altridati.CodiceCategoriaAttivita + "', ";
                    if (!string.IsNullOrEmpty(gestione.altridati.CodiceTipoAttivita ?? ""))
                        str35 = str35 + "CODATTCAM = '" + gestione.altridati.CodiceTipoAttivita + "', ";
                    string strSQL15 = str35 + "ULTAGG = CURRENT_TIMESTAMP, UTEAGG = '" + u.Username + "' WHERE CODICE ='" + gestione.datiAzienda.CodicePosizioneAzienda + "' ";
                    dataLayer1.WriteTransactionData(strSQL15, CommandType.Text);
                    string strSQL16 = "UPDATE AZISTOWEB SET DATCONF = " + DBMethods.DoublePeakForSql(DBMethods.Db2Date(dataSistema)) + ", " + " UTECONF = " + DBMethods.DoublePeakForSql(u.Username) + ", " + " CODPOSCONF = '" + CODPOS.ToString() + "' " + " WHERE CODICE = '" + gestione.datiAzienda.CodicePosizioneAzienda + "' ";
                    dataLayer1.WriteTransactionData(strSQL16, CommandType.Text);
                    string str36 = "UPDATE AZI SET ";
                    if (!string.IsNullOrEmpty(gestione.datiAzienda.RagioneSociale ?? ""))
                        str36 = str36 + "RAGSOC = " + DBMethods.DoublePeakForSql(gestione.datiAzienda.RagioneSociale ?? "") + ", ";
                    if (!string.IsNullOrEmpty(gestione.datiAzienda.RagioneSocialeBreve ?? ""))
                        str36 = str36 + "RAGSOCBRE = " + DBMethods.DoublePeakForSql(gestione.datiAzienda.RagioneSocialeBreve ?? "") + ", ";
                    if (!string.IsNullOrEmpty(gestione.datiAzienda.PartitaIva ?? ""))
                        str36 = str36 + "PARIVA = '" + gestione.datiAzienda.PartitaIva + "', ";
                    if (!string.IsNullOrEmpty(gestione.datiAzienda.CodiceFiscale ?? ""))
                        str36 = str36 + "CODFIS = '" + gestione.datiAzienda.CodiceFiscale + "', ";
                    if (!string.IsNullOrEmpty(gestione.datiAzienda.NaturaGiuridica ?? ""))
                        str36 = str36 + "NATGIU = " + gestione.datiAzienda.NaturaGiuridica + ", ";
                    if (!string.IsNullOrEmpty(gestione.datiAzienda.URL ?? ""))
                        str36 = str36 + "URL = '" + gestione.datiAzienda.URL + "', ";
                    if (!string.IsNullOrEmpty(gestione.datiAzienda.Note ?? ""))
                        str36 = str36 + "AZINOTC = '" + gestione.datiAzienda.Note + "', ";
                    if (!string.IsNullOrEmpty(gestione.datiAzienda.CodiceMezzoComunicazione ?? ""))
                        str36 = str36 + "CODMEZ = '" + gestione.datiAzienda.CodiceMezzoComunicazione + "', ";
                    if (!string.IsNullOrEmpty(gestione.datiAzienda.DataApertura ?? ""))
                        str36 = str36 + "DATAPE = '" + DBMethods.Db2Date(gestione.datiAzienda.DataApertura.Trim()) + "', ";
                    if (!string.IsNullOrEmpty(gestione.datiAzienda.DataDenunciaApertura ?? ""))
                        str36 = str36 + "DATDENAPE = '" + DBMethods.Db2Date(gestione.datiAzienda.DataDenunciaApertura.Trim()) + "', ";
                    string strSQL17 = str36 + string.Format("ULTAGG = CURRENT_TIMESTAMP, UTEAGG = '{0}' WHERE CODPOS = {1}", (object)u.Username, (object)CODPOS);
                    dataLayer1.WriteTransactionData(strSQL17, CommandType.Text);
                    for (int index = 1; index < 4; ++index)
                    {
                        string str37 = "UPDATE INDSED SET ";
                        switch (index - 1)
                        {
                            case 0:
                                if (!string.IsNullOrEmpty(gestione.sedeLegale.CodiceTipoIndirizzo ?? ""))
                                    str37 = str37 + "CODDUG = '" + gestione.sedeLegale.CodiceTipoIndirizzo + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeLegale.Indirizzo ?? ""))
                                    str37 = str37 + "IND = '" + gestione.sedeLegale.Indirizzo + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeLegale.Civico ?? ""))
                                    str37 = str37 + "NUMCIV = '" + gestione.sedeLegale.Civico + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeLegale.StatoEstero ?? ""))
                                    str37 = str37 + "DENSTAEST = '" + gestione.sedeLegale.StatoEstero + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeLegale.Cellulare ?? ""))
                                    str37 = str37 + "CELL = '" + gestione.sedeLegale.Cellulare + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeLegale.Localita ?? ""))
                                    str37 = str37 + "DENLOC = '" + gestione.sedeLegale.Localita + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeLegale.CAP ?? ""))
                                    str37 = str37 + "CAP = '" + gestione.sedeLegale.CAP + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeLegale.Telefono1 ?? ""))
                                    str37 = str37 + "TEL1 = '" + gestione.sedeLegale.Telefono1 + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeLegale.Telefono2 ?? ""))
                                    str37 = str37 + "TEL2 = '" + gestione.sedeLegale.Telefono2 + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeLegale.Fax ?? ""))
                                    str37 = str37 + "FAX = '" + gestione.sedeLegale.Fax + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeLegale.Email ?? ""))
                                    str37 = str37 + "EMAIL = '" + gestione.sedeLegale.Email + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeLegale.EmailCert ?? ""))
                                    str37 = str37 + "EMAILCERT = '" + gestione.sedeLegale.EmailCert + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeLegale.CodiceComune ?? ""))
                                {
                                    str37 = str37 + "CODCOM = '" + gestione.sedeLegale.CodiceComune + "', ";
                                    break;
                                }
                                break;
                            case 1:
                                if (!string.IsNullOrEmpty(gestione.indirizzoCorrispondenza.CodiceTipoIndirizzo ?? ""))
                                    str37 = str37 + "CODDUG = '" + gestione.indirizzoCorrispondenza.CodiceTipoIndirizzo + "', ";
                                if (!string.IsNullOrEmpty(gestione.indirizzoCorrispondenza.Indirizzo ?? ""))
                                    str37 = str37 + "IND = '" + gestione.indirizzoCorrispondenza.Indirizzo + "', ";
                                if (!string.IsNullOrEmpty(gestione.indirizzoCorrispondenza.Civico ?? ""))
                                    str37 = str37 + "NUMCIV = '" + gestione.indirizzoCorrispondenza.Civico + "', ";
                                if (!string.IsNullOrEmpty(gestione.indirizzoCorrispondenza.StatoEstero ?? ""))
                                    str37 = str37 + "DENSTAEST = '" + gestione.indirizzoCorrispondenza.StatoEstero + "', ";
                                if (!string.IsNullOrEmpty(gestione.indirizzoCorrispondenza.Cellulare ?? ""))
                                    str37 = str37 + "CELL = '" + gestione.indirizzoCorrispondenza.Cellulare + "', ";
                                if (!string.IsNullOrEmpty(gestione.indirizzoCorrispondenza.Localita ?? ""))
                                    str37 = str37 + "DENLOC = '" + gestione.indirizzoCorrispondenza.Localita + "', ";
                                if (!string.IsNullOrEmpty(gestione.indirizzoCorrispondenza.CAP ?? ""))
                                    str37 = str37 + "CAP = '" + gestione.indirizzoCorrispondenza.CAP + "', ";
                                if (!string.IsNullOrEmpty(gestione.indirizzoCorrispondenza.Telefono1 ?? ""))
                                    str37 = str37 + "TEL1 = '" + gestione.indirizzoCorrispondenza.Telefono1 + "', ";
                                if (!string.IsNullOrEmpty(gestione.indirizzoCorrispondenza.Telefono2 ?? ""))
                                    str37 = str37 + "TEL2 = '" + gestione.indirizzoCorrispondenza.Telefono2 + "', ";
                                if (!string.IsNullOrEmpty(gestione.indirizzoCorrispondenza.Fax ?? ""))
                                    str37 = str37 + "FAX = '" + gestione.indirizzoCorrispondenza.Fax + "', ";
                                if (!string.IsNullOrEmpty(gestione.indirizzoCorrispondenza.Email ?? ""))
                                    str37 = str37 + "EMAIL = '" + gestione.indirizzoCorrispondenza.Email + "', ";
                                if (!string.IsNullOrEmpty(gestione.indirizzoCorrispondenza.EmailCert ?? ""))
                                    str37 = str37 + "EMAILCERT = '" + gestione.indirizzoCorrispondenza.EmailCert + "', ";
                                if (!string.IsNullOrEmpty(gestione.indirizzoCorrispondenza.CodiceComune ?? ""))
                                    str37 = str37 + "CODCOM = '" + gestione.indirizzoCorrispondenza.CodiceComune + "', ";
                                if (!string.IsNullOrEmpty(gestione.indirizzoCorrispondenza.Destinatario ?? ""))
                                {
                                    str37 = str37 + "DENSTINAT = '" + gestione.indirizzoCorrispondenza.Destinatario + "', ";
                                    break;
                                }
                                break;
                            case 2:
                                if (!string.IsNullOrEmpty(gestione.sedeAmministrativa.CodiceTipoIndirizzo ?? ""))
                                    str37 = str37 + "CODDUG = '" + gestione.sedeAmministrativa.CodiceTipoIndirizzo + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeAmministrativa.Indirizzo ?? ""))
                                    str37 = str37 + "IND = '" + gestione.sedeAmministrativa.Indirizzo + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeAmministrativa.Civico ?? ""))
                                    str37 = str37 + "NUMCIV = '" + gestione.sedeAmministrativa.Civico + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeAmministrativa.StatoEstero ?? ""))
                                    str37 = str37 + "DENSTAEST = '" + gestione.sedeAmministrativa.StatoEstero + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeAmministrativa.Cellulare ?? ""))
                                    str37 = str37 + "CELL = '" + gestione.sedeAmministrativa.Cellulare + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeAmministrativa.Localita ?? ""))
                                    str37 = str37 + "DENLOC = '" + gestione.sedeAmministrativa.Localita + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeAmministrativa.CAP ?? ""))
                                    str37 = str37 + "CAP = '" + gestione.sedeAmministrativa.CAP + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeAmministrativa.Telefono1 ?? ""))
                                    str37 = str37 + "TEL1 = '" + gestione.sedeAmministrativa.Telefono1 + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeAmministrativa.Telefono2 ?? ""))
                                    str37 = str37 + "TEL2 = '" + gestione.sedeAmministrativa.Telefono2 + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeAmministrativa.Fax ?? ""))
                                    str37 = str37 + "FAX = '" + gestione.sedeAmministrativa.Fax + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeAmministrativa.Email ?? ""))
                                    str37 = str37 + "EMAIL = '" + gestione.sedeAmministrativa.Email + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeAmministrativa.EmailCert ?? ""))
                                    str37 = str37 + "EMAILCERT = '" + gestione.sedeAmministrativa.EmailCert + "', ";
                                if (!string.IsNullOrEmpty(gestione.sedeAmministrativa.CodiceComune ?? ""))
                                {
                                    str37 = str37 + "CODCOM = '" + gestione.sedeAmministrativa.CodiceComune + "', ";
                                    break;
                                }
                                break;
                        }
                        string strSQL18 = str37 + string.Format("ULTAGG = CURRENT_TIMESTAMP, UTEAGG = '{0}' WHERE CODPOS = {1} AND TIPIND = {2}", (object)u.Username, (object)CODPOS, (object)index);
                        dataLayer1.WriteTransactionData(strSQL18, CommandType.Text);
                        string strSQL19 = strSQL18.Replace("INDSED", "INDSEDWEB").Replace("WHERE CODPOS", "WHERE CODPOSCONF");
                        dataLayer1.WriteTransactionData(strSQL19, CommandType.Text);
                    }
                    string str38 = "UPDATE AZIRAP SET ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.TipoRapp ?? ""))
                        str38 = str38 + "CODFUNRAP = '" + gestione.rapleg.TipoRapp + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.RapLegPrinc ?? ""))
                        str38 = str38 + "RAPPRI = '" + gestione.rapleg.RapLegPrinc + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Cognome ?? ""))
                        str38 = str38 + "COG = '" + gestione.rapleg.Cognome + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Nome ?? ""))
                        str38 = str38 + "NOM = '" + gestione.rapleg.Nome + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.CODDUG ?? ""))
                        str38 = str38 + "CODDUG = '" + gestione.rapleg.CODDUG + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Indirizzo ?? ""))
                        str38 = str38 + "IND = '" + gestione.rapleg.Indirizzo + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Civico ?? ""))
                        str38 = str38 + "NUMCIV = '" + gestione.rapleg.Civico + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.StatoEstero ?? ""))
                        str38 = str38 + "DENSTAEST = '" + gestione.rapleg.StatoEstero + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Localita ?? ""))
                        str38 = str38 + "DENLOC = '" + gestione.rapleg.Localita + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.CAP ?? ""))
                        str38 = str38 + "CAP = '" + gestione.rapleg.CAP + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Provincia ?? ""))
                        str38 = str38 + "SIGPRO = '" + gestione.rapleg.Provincia + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Telefono1 ?? ""))
                        str38 = str38 + "TEL1 = '" + gestione.rapleg.Telefono1 + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Telefono2 ?? ""))
                        str38 = str38 + "TEL2 = '" + gestione.rapleg.Telefono2 + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Fax ?? ""))
                        str38 = str38 + "FAX = '" + gestione.rapleg.Fax + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Email ?? ""))
                        str38 = str38 + "EMAIL = '" + gestione.rapleg.Email + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.EmailCert ?? ""))
                        str38 = str38 + "EMAILCERT = '" + gestione.rapleg.EmailCert + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.CodiceFiscale ?? ""))
                        str38 = str38 + "CODFIS = '" + gestione.rapleg.CodiceFiscale + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.DataNascita ?? ""))
                        str38 = str38 + "DATNAS = '" + DBMethods.Db2Date(gestione.rapleg.DataNascita) + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.CodiceComuneNascita ?? ""))
                        str38 = str38 + "CODCOMNAS = '" + gestione.rapleg.CodiceComuneNascita + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Sesso ?? ""))
                        str38 = str38 + "SES = '" + gestione.rapleg.Sesso + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.Cell ?? ""))
                        str38 = str38 + "CELL = '" + gestione.rapleg.Cell + "', ";
                    if (!string.IsNullOrEmpty(gestione.rapleg.CodiceComuneResidenza ?? ""))
                        str38 = str38 + "CODCOMRES = '" + gestione.rapleg.CodiceComuneResidenza + "', ";
                    string strSQL20 = str38 + string.Format("ULTAGG = CURRENT_TIMESTAMP, UTEAGG = '{0}' WHERE CODPOS = {1} AND PROREC = {2}", (object)u.Username, (object)CODPOS, (object)gestione.rapleg.PROREC);
                    dataLayer1.WriteTransactionData(strSQL20, CommandType.Text);
                    string str39 = "UPDATE AZIATT SET ";
                    if (!string.IsNullOrEmpty(gestione.altridati.CodiceCategoriaAttivita ?? ""))
                        str39 = str39 + "CATATTCAM = '" + gestione.altridati.CodiceCategoriaAttivita + "', ";
                    if (!string.IsNullOrEmpty(gestione.altridati.CodiceTipoAttivita ?? ""))
                        str39 = str39 + "CODATTCAM = '" + gestione.altridati.CodiceTipoAttivita + "', ";
                    string strSQL21 = str39 + string.Format("ULTAGG = CURRENT_TIMESTAMP, UTEAGG = '{0}' WHERE CODPOS = {1}", (object)u.Username, (object)CODPOS);
                    dataLayer1.WriteTransactionData(strSQL21, CommandType.Text);
                    string strSQL22 = "SELECT COUNT(*) FROM TB_VERPEC WHERE CODPOS = '" + CODPOS.ToString() + "' ";
                    if (dataLayer1.Get1ValueFromSQL(strSQL22, CommandType.Text) == "0")
                    {
                        string strSQL23 = "INSERT INTO TB_VERPEC(CODPOS) VALUES (" + CODPOS.ToString() + ")";
                        dataLayer1.WriteTransactionData(strSQL23, CommandType.Text);
                    }
                }
                modIDOC_Aziende modIdocAziende = new modIDOC_Aziende();
                modIdocAziende.WRITE_IDOC_ANAG_IND_AZI(ref Conn, ref cmd, ref da, CODPOS, ref DT_SEDI_OLD);
                modIdocAziende.Aggiorna_IDOC_AZI(ref cmd);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)u), JsonConvert.SerializeObject((object)gestione));
                ErroreMSG = "Errore durante la procedura di inserimento";
                dataLayer1.EndTransaction(false);
                return gestione;
            }
            SuccessMSG = "Inserimento andato a buon fine";
            dataLayer1.EndTransaction(true);
            return gestione;
        }

        public string GeneraPWD()
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

        public string Module_GetDataSistema()
        {
            DataLayer dataLayer = new DataLayer();
            string Err = "";
            string strSQL = "SELECT DISTINCT CURRENT_DATE AS DATASISTEMA FROM DBUNICONET.TIPIND";
            return dataLayer.GetDataSet(strSQL, ref Err).Tables[0].Rows[0]["DATASISTEMA"].ToString();
        }

        public Dictionary<string, string> GetTipRap()
        {
            var dataLayer = new DataLayer();
            var tipRap = dataLayer.GetDataTable("SELECT TIPRAP, DENTIPRAP FROM TIPRAP");
            return tipRap.Rows.Cast<DataRow>().ToDictionary(row => row.ElementAt("TIPRAP"), row => row.ElementAt("DENTIPRAP"));
        }

        public IEnumerable<Genere> GetGeneri()
        {
            var generi = new List<Genere>();
            try
            {
                string getGeneriQuery = "SELECT ID, DESCRIZIONE FROM TBGENERE";
                var dataLayer = new DataLayer();
                DataTable resultTable = dataLayer.GetDataTable(getGeneriQuery);

                if (dataLayer.queryOk(resultTable))
                {
                    foreach (DataRow row in resultTable.Rows)
                        generi.Add(new Genere()
                        {
                            Id = (int)row.IntElementAt("ID"),
                            Descrizione = row.ElementAt("DESCRIZIONE")
                        });
                }
                return generi;
            }
            catch
            {
                return generi;
            }
        }
    }
}
