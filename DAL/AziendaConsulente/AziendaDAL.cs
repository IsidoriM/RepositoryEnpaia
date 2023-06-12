// Decompiled with JetBrains decompiler
// Type: TFI.DAL.AziendaConsulente.AziendaDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using OCM.TFI.OCM.AziendaConsulente;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using TFI.DAL.ConnectorDB;
using TFI.OCM.AziendaConsulente;
using TFI.OCM.Utente;
using Utilities;
using static TFI.OCM.Amministrativo.EstrattoContoOCM;
using Azienda = TFI.OCM.AziendaConsulente.Azienda;

namespace TFI.DAL.AziendaConsulente
{
    public class AziendaDAL
    {
        private DataLayer dataLayer;

        public Azienda.RapLeg GetRapLeg(string strCodPos)
        {
            string Err = "";
            try
            {
                DataSet dataSet1 = new DataSet();
                string strSQL = " SELECT DISTINCT A.COG, A.NOM, A.SES, TRANSLATE(CHAR(A.DATNAS, EUR), '/', '.') AS DATNAS, A.CODCOMNAS, " + "COM.DENCOM AS COM_NASCITA, PNASC.SIGPRO AS PROV_NASCITA, A.CODFIS AS CODFISRAP, A.DENSTAEST, " +
                    "(SELECT DENCOM FROM CODCOM WHERE CODCOM = A.CODCOMRES) AS DENCOM," + "A.CODCOMRES, B.CODLOC AS CODLOC_RES, A.DENLOC AS LOC_RES, A.CAP AS CAP_RES, A.SIGPRO AS PROV_RES, A.CODDUG, " +
                    "DENDUG, A.IND AS INDRAP, A.NUMCIV, A.CODFUNRAP, C.DENFUNRAP, A.DATINI FROM AZIRAP A LEFT JOIN CODCOM AS " + "COM ON A.CODCOMNAS = COM.CODCOM LEFT JOIN CODLOC AS PNASC ON COM.DENCOM = PNASC.DENLOC LEFT JOIN CODLOC B " +
                    "ON A.SIGPRO = B.SIGPRO AND A.CAP = B.CAP AND A.DENLOC = B.DENLOC LEFT JOIN DUG ON A.CODDUG = DUG.CODDUG " + "INNER JOIN FUNRAP C ON A.CODFUNRAP = C.CODFUNRAP WHERE A.CODPOS = " + strCodPos + " AND A.DATCOM = " +
                    "(SELECT MAX(DATCOM) FROM AZIRAP WHERE CODPOS = " + strCodPos + ") AND DATINI = (SELECT MAX(DATINI) FROM AZIRAP WHERE CODPOS = " + strCodPos + ") AND RAPPRI = 'S'";
                this.dataLayer = new DataLayer();
                DataSet dataSet2 = this.dataLayer.GetDataSet(strSQL, ref Err);
                if (this.dataLayer.queryOk(dataSet2))
                    return new Azienda.RapLeg()
                    {
                        CognomeRP = dataSet2.Tables[0].Rows[0]["COG"].ToString(),
                        NomeRP = dataSet2.Tables[0].Rows[0]["NOM"].ToString(),
                        SessoRP = dataSet2.Tables[0].Rows[0]["SES"].ToString(),
                        DataNascitaRP = dataSet2.Tables[0].Rows[0]["DATNAS"].ToString(),
                        CodiceComuneNascitaRP = dataSet2.Tables[0].Rows[0]["CODCOMNAS"].ToString(),
                        ComuneNascitaRP = dataSet2.Tables[0].Rows[0]["COM_NASCITA"].ToString().Trim(),
                        ProvinciaNascitaRP = dataSet2.Tables[0].Rows[0]["PROV_NASCITA"].ToString(),
                        CodFiscaleRP = dataSet2.Tables[0].Rows[0]["CODFISRAP"].ToString(),
                        DecorrenzaRP = dataSet2.Tables[0].Rows[0]["DATINI"].ToString().Substring(0, 10),
                        FunzioneRP = dataSet2.Tables[0].Rows[0]["DENFUNRAP"].ToString(),
                        IndirizzoRP = dataSet2.Tables[0].Rows[0]["INDRAP"].ToString(),
                        TipoIndirizzoRP = dataSet2.Tables[0].Rows[0]["DENDUG"].ToString(),
                        CivicoRP = dataSet2.Tables[0].Rows[0]["NUMCIV"].ToString(),
                        LocalitaRP = dataSet2.Tables[0].Rows[0]["LOC_RES"].ToString(),
                        CapRP = dataSet2.Tables[0].Rows[0]["CAP_RES"].ToString(),
                        ProvinciaRP = dataSet2.Tables[0].Rows[0]["PROV_RES"].ToString(),
                        StatoEsteroRP = dataSet2.Tables[0].Rows[0]["DENSTAEST"].ToString(),
                        ComuneRP = dataSet2.Tables[0].Rows[0]["DENCOM"].ToString().Trim(),
                        CodiceComuneResidenzaRP = dataSet2.Tables[0].Rows[0]["CODCOMRES"].ToString(),
                        CodiceLocalitaResidenzaRP = dataSet2.Tables[0].Rows[0]["CODLOC_RES"].ToString(),
                        CODDUGRP = Convert.ToInt32(dataSet2.Tables[0].Rows[0]["CODDUG"].ToString()),
                        CODFUNRAPRP = dataSet2.Tables[0].Rows[0]["CODFUNRAP"].ToString()
                    };
            }
            catch (Exception ex)
            {
            }
            return new Azienda.RapLeg();
        }

        public List<Azienda.Sedi> ListaTipiSedi()
        {
            DataLayer dataLayer = new DataLayer();
            string Err = "";
            DataSet dataSet1 = new DataSet();
            string strSQL = "SELECT TIPIND, DENIND FROM TIPIND";
            DataSet dataSet2 = dataLayer.GetDataSet(strSQL, ref Err);
            List<Azienda.Sedi> sediList = new List<Azienda.Sedi>();
            List<Azienda.Sedi> source = new List<Azienda.Sedi>();
            for (int index = 0; index < dataSet2.Tables[0].Rows.Count; ++index)
                source.Add(new Azienda.Sedi()
                {
                    CODDUGS = Convert.ToInt32(dataSet2.Tables[0].Rows[index]["TIPIND"]),
                    TipoIndirizzoS = dataSet2.Tables[0].Rows[index]["DENIND"].ToString()
                });
            return source.ToList<Azienda.Sedi>();
        }

        public List<Azienda.StatoEstero> GetStatiEsteri()
        {
            try
            {
                DataTable dataTable1 = new DataTable();
                string strSQL = "SELECT * FROM COM_ESTERO";
                this.dataLayer = new DataLayer();
                DataTable dataTable2 = this.dataLayer.GetDataTable(strSQL);
                if (this.dataLayer.queryOk(dataTable2))
                {
                    List<Azienda.StatoEstero> statiEsteri = new List<Azienda.StatoEstero>();
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                        statiEsteri.Add(new Azienda.StatoEstero()
                        {
                            CODCOMSE = row["CODCOM"].ToString(),
                            DENCOMSE = row["DENCOM"].ToString()
                        });
                    return statiEsteri;
                }
            }
            catch (Exception ex)
            {
            }
            return (List<Azienda.StatoEstero>)null;
        }

        public List<Azienda.Localita> GetLocalita()
        {
            try
            {
                DataTable dataTable1 = new DataTable();
                string strSQL = "SELECT * FROM CODLOC";
                this.dataLayer = new DataLayer();
                DataTable dataTable2 = this.dataLayer.GetDataTable(strSQL);
                if (this.dataLayer.queryOk(dataTable2))
                {
                    List<Azienda.Localita> localita = new List<Azienda.Localita>();
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                        localita.Add(new Azienda.Localita()
                        {
                            CODLOCL = Convert.ToInt32(row["CODLOC"].ToString()),
                            DENLOCL = row["DENLOC"].ToString(),
                            SIGPROL = row["SIGPRO"].ToString(),
                            CAPL = row["CAP"].ToString()
                        });
                    return localita;
                }
            }
            catch (Exception ex)
            {
            }
            return (List<Azienda.Localita>)null;
        }

        public List<Azienda.Comune> GetComuni()
        {
            try
            {
                DataTable dataTable1 = new DataTable();
                string strSQL = "SELECT * FROM CODCOM ORDER BY DENCOM ASC";
                this.dataLayer = new DataLayer();
                DataTable dataTable2 = this.dataLayer.GetDataTable(strSQL);
                if (this.dataLayer.queryOk(dataTable2))
                {
                    List<Azienda.Comune> comuni = new List<Azienda.Comune>();
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                        comuni.Add(new Azienda.Comune()
                        {
                            CODCOMCo = row["CODCOM"].ToString(),
                            DENCOMCo = row["DENCOM"].ToString(),
                            SIGPROCo = row["SIGPRO"].ToString()
                        });
                    return comuni;
                }
            }
            catch (Exception ex)
            {
            }
            return (List<Azienda.Comune>)null;
        }

        public void AutoCompleate()
        {
            DataLayer dataLayer = new DataLayer();
            string strQuery1 = "SELECT DENCOM FROM CODCOM";
            iDB2DataReader dataReaderFromQuery1 = dataLayer.GetDataReaderFromQuery(strQuery1, CommandType.Text);
            List<string> stringList1 = new List<string>();
            while (dataReaderFromQuery1.Read())
                stringList1.Add(dataReaderFromQuery1["DENCOM"].ToString().Trim());
            HttpContext.Current.Items[(object)"ListaComuni"] = (object)stringList1.ToArray();
            string strQuery2 = "SELECT DENCOM FROM COM_ESTERO";
            iDB2DataReader dataReaderFromQuery2 = dataLayer.GetDataReaderFromQuery(strQuery2, CommandType.Text);
            List<string> stringList2 = new List<string>();
            while (dataReaderFromQuery2.Read())
                stringList2.Add(dataReaderFromQuery2["DENCOM"].ToString().Trim());
            HttpContext.Current.Items[(object)"ListaStati"] = (object)stringList2.ToArray();
        }

        public List<string> GetComunis()
        {
            try
            {
                DataTable dataTable1 = new DataTable();
                string strSQL = "SELECT DENCOM FROM CODCOM ORDER BY DENCOM ASC";
                this.dataLayer = new DataLayer();
                DataTable dataTable2 = this.dataLayer.GetDataTable(strSQL);
                if (this.dataLayer.queryOk(dataTable2))
                {
                    List<string> comunis = new List<string>();
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                        comunis.Add(row["DENCOM"].ToString());
                    return comunis;
                }
            }
            catch (Exception ex)
            {
            }
            return (List<string>)null;
        }

        public List<string> GetStatiEsteris()
        {
            try
            {
                DataTable dataTable1 = new DataTable();
                string strSQL = "SELECT DENCOM FROM COM_ESTERO ORDER BY DENCOM ASC";
                this.dataLayer = new DataLayer();
                DataTable dataTable2 = this.dataLayer.GetDataTable(strSQL);
                if (this.dataLayer.queryOk(dataTable2))
                {
                    List<string> statiEsteris = new List<string>();
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                        statiEsteris.Add(row["DENCOM"].ToString());
                    return statiEsteris;
                }
            }
            catch (Exception ex)
            {
            }
            return (List<string>)null;
        }

        public List<Azienda.Via> GetVia()
        {
            try
            {
                DataTable dataTable1 = new DataTable();
                string strSQL = "SELECT DISTINCT * FROM DUG";
                this.dataLayer = new DataLayer();
                DataTable dataTable2 = this.dataLayer.GetDataTable(strSQL);
                if (this.dataLayer.queryOk(dataTable2))
                {
                    List<Azienda.Via> via = new List<Azienda.Via>();
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                        via.Add(new Azienda.Via()
                        {
                            CODDUGV = Convert.ToInt32(row["CODDUG"].ToString()),
                            DENDUGV = row["DENDUG"].ToString()
                        });
                    return via;
                }
            }
            catch (Exception ex)
            {
            }
            return (List<Azienda.Via>)null;
        }

        public IEnumerable<Genere> GetGeneri()
        {
            List<Genere> generi = new List<Genere>();
            try
            {
                string getGeneriQuery = "SELECT ID, DESCRIZIONE FROM TBGENERE";
                dataLayer = new DataLayer();
                DataTable resultTable = dataLayer.GetDataTable(getGeneriQuery);

                if (dataLayer.queryOk(resultTable))
                {
                    foreach (DataRow row in resultTable.Rows)
                        generi.Add(new Genere()
                        {
                            Id = Convert.ToInt32(row["ID"].ToString()),
                            Descrizione = row["DESCRIZIONE"].ToString(),
                        });
                }
                return generi;
            }
            catch
            {
                return generi;
            }
        }

        public string controllo(string g)
        {
            DataTable dataTable1 = new DataTable();
            string strSQL = "SELECT CODCOM FROM CODCOM WHERE DENCOM ='" + g + "'";
            this.dataLayer = new DataLayer();
            DataTable dataTable2 = this.dataLayer.GetDataTable(strSQL);
            string str = (string)null;
            if (this.dataLayer.queryOk(dataTable2))
                str = dataTable2.Rows[0]["CODCOM"].ToString();
            return str;
        }

        public Azienda.Sedi GetListaSedi(string codPos)
        {
            DataLayer dataLayer = new DataLayer();

            var codPosParameter = dataLayer.CreateParameter("@Codpos", iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codPos);
            string strSQL = "SELECT I.CODDUG, D.DENDUG, I.IND, I.NUMCIV, I.CODCOM, C.DENCOM, I.CAP, I.SIGPRO, I.DENLOC, " +
                    "I.DENSTAEST, I.TEL1 " + ", AZ.EMAIL, AZ.EMAILCERT " +
                    "FROM INDSED I INNER JOIN AZEMAIL AZ ON AZ.CODPOS = I.CODPOS LEFT JOIN CODCOM C ON C.CODCOM = I.CODCOM " +
                    "INNER JOIN DUG D ON D.CODDUG = I.CODDUG " +
                    "WHERE I.CODPOS = @Codpos AND I.TIPIND = 2 AND I.DATINI = (SELECT MAX(DATINI) FROM INDSED WHERE CODPOS = @Codpos AND TIPIND = 2)";

            DataTable dataTable2 = this.dataLayer.GetDataTableWithParameters(strSQL, codPosParameter, codPosParameter);
            if (this.dataLayer.queryOk(dataTable2))
                return new Azienda.Sedi()
                {
                    //IdS = Convert.ToInt32(dataTable2.Rows[0]["TIPIND"].ToString()),
                    CODDUGS = Convert.ToInt32(dataTable2.Rows[0]["CODDUG"].ToString()),
                    //TipoIndirizzoS = dataTable2.Rows[0]["DENIND"].ToString(),
                    IndirizzoS = dataTable2.Rows[0]["DENDUG"].ToString(),
                    ViaS = dataTable2.Rows[0]["IND"].ToString(),
                    CivicoS = dataTable2.Rows[0]["NUMCIV"].ToString(),
                    CODCOMS = dataTable2.Rows[0]["CODCOM"].ToString(),
                    ComuneS = dataTable2.Rows[0]["DENCOM"].ToString(),
                    CAPS = dataTable2.Rows[0]["CAP"].ToString(),
                    ProvinciaS = dataTable2.Rows[0]["SIGPRO"].ToString(),
                    LocalitaS = dataTable2.Rows[0]["DENLOC"].ToString(),
                    StatoEsteroS = dataTable2.Rows[0]["DENSTAEST"].ToString(),
                    Telefono1S = dataTable2.Rows[0]["TEL1"].ToString(),
                    //Telefono2S = dataTable2.Rows[0]["TEL2"].ToString(),
                    //FaxS = dataTable2.Rows[0]["FAX"].ToString(),
                    EmailS = dataTable2.Rows[0]["EMAIL"].ToString(),
                    EmailCertificataS = dataTable2.Rows[0]["EMAILCERT"].ToString(),
                    //DATINIS = dataTable2.Rows[0]["DATINI"].ToString(),
                    //AGGMANS = dataTable2.Rows[0]["AGGMAN"].ToString()
                };


            //if (str != "")
            //{
            //    string strSQL = "DELETE FROM INDSED WHERE CODPOS =" + codPos + "AND TIPIND =" + id;
            //    dataLayer.WriteData(strSQL, CommandType.Text);
            //}
            return null;
        }

        public Azienda TrovaSedeIndDAL(string id, string codPos)
        {
            Azienda azienda = new Azienda();
            try
            {
                DataTable dataTable = new DataLayer().GetDataTable("SELECT A.TIPIND, A.CODDUG, DUG.DENDUG, B.DENIND, CHAR(B.ULTAGG) AS ULTAGG, A.IND, A.NUMCIV, " +
                                                                   "A.CODCOM, C.DENCOM, A.CAP, A.SIGPRO, A.DENLOC, A.DENSTAEST, A.TEL1, A.TEL2, A.FAX, AZ.EMAIL, AZ.EMAILCERT, " +
                                                                   "TRANSLATE(CHAR(A.DATINI, EUR), '/', '.') AS DATINI, AGGMAN FROM INDSED A " +
                                                                   "INNER JOIN AZEMAIL AZ ON AZ.CODPOS = A.CODPOS INNER JOIN TIPIND B " + "ON  B.TIPIND = " +
                                                                   id + "  LEFT JOIN DUG ON A.CODDUG = DUG.CODDUG LEFT JOIN CODCOM C ON A.CODCOM = " +
                                                                   "C.CODCOM INNER JOIN (SELECT TIPIND, MAX(DATCOM) AS DATCOM FROM INDSED WHERE CODPOS = " +
                                                                   codPos + " AND current_date BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31') GROUP BY TIPIND) " +
                                                                   "AS D ON D.TIPIND =" + id + " AND A.DATCOM = D.DATCOM WHERE A.CODPOS = " + codPos + " AND A.TIPIND=" + id);
                if (dataTable.Rows.Count > 0)
                {
                    Azienda.Sedi sedi = new Azienda.Sedi()
                    {
                        IdS = Convert.ToInt32(dataTable.Rows[0]["TIPIND"].ToString()),
                        CODDUGS = Convert.ToInt32(dataTable.Rows[0]["CODDUG"].ToString()),
                        TipoIndirizzoS = dataTable.Rows[0]["DENIND"].ToString(),
                        IndirizzoS = dataTable.Rows[0]["DENDUG"].ToString(),
                        ViaS = dataTable.Rows[0]["IND"].ToString(),
                        CivicoS = dataTable.Rows[0]["NUMCIV"].ToString(),
                        CODCOMS = dataTable.Rows[0]["CODCOM"].ToString(),
                        ComuneS = dataTable.Rows[0]["DENCOM"].ToString(),
                        CAPS = dataTable.Rows[0]["CAP"].ToString(),
                        ProvinciaS = dataTable.Rows[0]["SIGPRO"].ToString(),
                        LocalitaS = dataTable.Rows[0]["DENLOC"].ToString(),
                        StatoEsteroS = dataTable.Rows[0]["DENSTAEST"].ToString(),
                        Telefono1S = dataTable.Rows[0]["TEL1"].ToString(),
                        Telefono2S = dataTable.Rows[0]["TEL2"].ToString(),
                        FaxS = dataTable.Rows[0]["FAX"].ToString(),
                        EmailS = dataTable.Rows[0]["EMAIL"].ToString(),
                        EmailCertificataS = dataTable.Rows[0]["EMAILCERT"].ToString(),
                        DATINIS = dataTable.Rows[0]["DATINI"].ToString(),
                        AGGMANS = dataTable.Rows[0]["AGGMAN"].ToString()
                    };
                    azienda.sedi = sedi;
                }
            }
            catch (Exception ex)
            {
                return (Azienda)null;
            }
            return azienda;
        }

        public Azienda.Consulente GetConsulente(string strCodPos)
        {
            string Err = "";
            try
            {
                DataSet dataSet1 = new DataSet();
                string strSQL = "SELECT F.RAGSOC, F.IND, E.DENDUG, F.NUMCIV, F.DENLOC, G.DENCOM, F.CAP, F.TEL2, F.CELL, F.SIGPRO, G.CODCOM, " +
                                "F.TEL1, F.FAX, F.EMAIL, F.EMAILCERT, A.CODTER, F.CODDUG FROM DELEGHE A INNER JOIN ASSTER F ON A.CODTER = F.CODTER " +
                                " LEFT JOIN DUG E ON F.CODDUG = E.CODDUG" + " LEFT  JOIN CODCOM G ON F.CODCOM = G.CODCOM" + " WHERE A.CODPOS = " +
                                strCodPos + " AND CURRENT_DATE BETWEEN A.DATINI AND VALUE(A.DATFIN, '9999-12-31')";
                this.dataLayer = new DataLayer();
                DataSet dataSet2 = this.dataLayer.GetDataSet(strSQL, ref Err);
                if (dataSet2.Tables[0].Rows.Count > 0)
                    return new Azienda.Consulente()
                    {
                        RagioneSocialeC = dataSet2.Tables[0].Rows[0]["RAGSOC"].ToString(),
                        IndirizzoC = dataSet2.Tables[0].Rows[0]["IND"].ToString(),
                        DENDUGC = dataSet2.Tables[0].Rows[0]["DENDUG"].ToString(),
                        NumeroCivicoC = dataSet2.Tables[0].Rows[0]["NUMCIV"].ToString(),
                        LocalitaC = dataSet2.Tables[0].Rows[0]["DENLOC"].ToString(),
                        ComuneC = dataSet2.Tables[0].Rows[0]["DENCOM"].ToString(),
                        CAPC = dataSet2.Tables[0].Rows[0]["CAP"].ToString(),
                        Telefono1C = dataSet2.Tables[0].Rows[0]["TEL1"].ToString(),
                        Telefono2C = dataSet2.Tables[0].Rows[0]["TEL2"].ToString(),
                        CellulareC = dataSet2.Tables[0].Rows[0]["CELL"].ToString(),
                        ProvinciaC = dataSet2.Tables[0].Rows[0]["SIGPRO"].ToString(),
                        CODCOMC = dataSet2.Tables[0].Rows[0]["CODCOM"].ToString(),
                        FaxC = dataSet2.Tables[0].Rows[0]["FAX"].ToString(),
                        EmailC = dataSet2.Tables[0].Rows[0]["EMAIL"].ToString(),
                        EmailCertificataC = dataSet2.Tables[0].Rows[0]["EMAILCERT"].ToString(),
                        CODTERC = dataSet2.Tables[0].Rows[0]["CODTER"].ToString(),
                        CODDUGC = string.IsNullOrEmpty(dataSet2.Tables[0].Rows[0]["CODDUG"].ToString()) ? 0 : Convert.ToInt32(dataSet2.Tables[0].Rows[0]["CODDUG"].ToString())
                    };
                return new Azienda.Consulente()
                {
                    RagioneSocialeC = "",
                    IndirizzoC = "",
                    DENDUGC = "",
                    NumeroCivicoC = "",
                    LocalitaC = "",
                    ComuneC = "",
                    CAPC = "",
                    Telefono1C = "",
                    Telefono2C = "",
                    CellulareC = "",
                    ProvinciaC = "",
                    CODCOMC = "",
                    FaxC = "",
                    EmailC = "",
                    EmailCertificataC = "",
                    CODTERC = "",
                    CODDUGC = 0
                };
            }
            catch (Exception ex)
            {
            }
            return (Azienda.Consulente)null;
        }

        public Azienda.PosAss GetPosAss(string strCodPos)
        {
            string Err = "";
            try
            {
                this.AutoCompleate();
                DataSet dataSet1 = new DataSet();
                string strSQL = "SELECT DISTINCT A.INSAZI, A.AZINOTC, A.CODPOS, RAGSOC, RAGSOCBRE, CODFIS AS CODFISAZI, PARIVA, A.NATGIU, ABB, " +
                                "TRANSLATE(CHAR(AZISTO.DATINI, EUR), '/', '.') AS DATINI, CHAR(A.ULTAGG) AS ULTAGG ,NATGIU.DENNATGIU, TRANSLATE" +
                                "(CHAR(A.DATAPE, EUR), '/', '.') AS DATAPE, TRANSLATE(CHAR(A.DATCHI, EUR), '/', '.') AS DATCHI, AZISTO.TIPISC, " +
                                "B.CATATTCAM, C.DENATTCAM, C.CODATTCAM, TRANSLATE(CHAR(B.DATINI, EUR), '/', '.') AS DATINIATT, D.CODSTACON, " +
                                "D.DENCODSTA FROM AZI A LEFT JOIN AZISTO ON A.CODPOS = AZISTO.CODPOS AND AZISTO.DATCOM = (SELECT MAX(DATCOM) " +
                                "FROM AZISTO WHERE CODPOS = " + strCodPos + " AND current_date BETWEEN DATINI AND VALUE(DATFIN, '9999-12-31')) " +
                                "INNER JOIN NATGIU ON A.NATGIU = NATGIU.NATGIU LEFT JOIN AZIATT B ON A.CODPOS = B.CODPOS AND B.DATCOM = " +
                                "(SELECT MAX(DATCOM) FROM AZIATT WHERE CODPOS = " + strCodPos + " AND current_date BETWEEN DATINI AND VALUE(DATFIN, " +
                                "'9999-12-31')) LEFT JOIN TIPATT AS C ON B.CATATTCAM = C.CATATTCAM AND B.CODATTCAM = C.CODATTCAM LEFT JOIN CODSTA " +
                                "AS D ON AZISTO.CODSTACON = D.CODSTACON WHERE A.CODPOS = " + strCodPos;
                this.dataLayer = new DataLayer();
                DataSet dataSet2 = this.dataLayer.GetDataSet(strSQL, ref Err);
                DataSet dataSet3 = new DataSet();
                DataSet dataSet4 = this.dataLayer.GetDataSet("SELECT A.TIPIND, A.CODDUG, DUG.DENDUG, B.DENIND, CHAR(B.ULTAGG) AS ULTAGG, A.IND, A.NUMCIV, " +
                                                             "A.CODCOM, C.DENCOM, A.CAP, A.SIGPRO, A.DENLOC, A.DENSTAEST, A.TEL1, A.TEL2, A.FAX, AZ.EMAIL, AZ.EMAILCERT, " +
                                                             "TRANSLATE(CHAR(A.DATINI, EUR), '/', '.') AS DATINI, AGGMAN FROM INDSED A " +
                                                             "INNER JOIN AZEMAIL AZ ON AZ.CODPOS = A.CODPOS INNER JOIN TIPIND B " +
                                                             "ON A.TIPIND = B.TIPIND LEFT JOIN DUG ON A.CODDUG = DUG.CODDUG LEFT JOIN CODCOM C ON A.CODCOM = " +
                                                             "C.CODCOM INNER JOIN (SELECT TIPIND, MAX(DATCOM) AS DATCOM FROM INDSED WHERE CODPOS = " + strCodPos +
                                                             " AND DATINI = (SELECT MAX(DATINI) FROM INDSED WHERE CODPOS = " + strCodPos + " AND TIPIND = 1) GROUP BY TIPIND) " +
                                                             "AS D ON A.TIPIND = D.TIPIND AND A.DATCOM = D.DATCOM WHERE A.CODPOS = " + strCodPos + 
                                                             " AND AZ.DATINI = (SELECT MAX(DATINI) FROM AZEMAIL WHERE CODPOS = " + strCodPos + ") AND A.TIPIND = 1", ref Err);
                if (this.dataLayer.queryOk(dataSet2))
                {
                    Azienda.PosAss posAss = new Azienda.PosAss();
                    posAss.INSAZIPA = dataSet2.Tables[0].Rows[0]["INSAZI"].ToString();
                    posAss.AZINOTCPA = dataSet2.Tables[0].Rows[0]["AZINOTC"].ToString();
                    posAss.CODPOSPA = dataSet2.Tables[0].Rows[0]["CODPOS"].ToString();
                    posAss.NATGIUPA = dataSet2.Tables[0].Rows[0]["NATGIU"].ToString();
                    posAss.DATINIPA = dataSet2.Tables[0].Rows[0]["DATINI"].ToString();
                    posAss.ULTAGGPA = dataSet2.Tables[0].Rows[0]["ULTAGG"].ToString();
                    posAss.RagioneSocialePA = dataSet2.Tables[0].Rows[0]["RAGSOC"].ToString();
                    posAss.CodiceFiscalePA = dataSet2.Tables[0].Rows[0]["CODFISAZI"].ToString();
                    posAss.PartitaIVAPA = dataSet2.Tables[0].Rows[0]["PARIVA"].ToString();
                    posAss.RSBrevePA = dataSet2.Tables[0].Rows[0]["RAGSOCBRE"].ToString();
                    posAss.DataAperturaPA = dataSet2.Tables[0].Rows[0]["DATAPE"].ToString();
                    posAss.DataChiusuraPA = dataSet2.Tables[0].Rows[0]["DATCHI"].ToString();
                    posAss.TIPISCPA = dataSet2.Tables[0].Rows[0]["TIPISC"].ToString();
                    posAss.CATATTCAMPA = dataSet2.Tables[0].Rows[0]["CATATTCAM"].ToString();
                    posAss.NaturaGiuridicaPA = dataSet2.Tables[0].Rows[0]["DENNATGIU"].ToString();
                    posAss.CODATTCAMPA = dataSet2.Tables[0].Rows[0]["CODATTCAM"].ToString();
                    posAss.CODSTACONPA = dataSet2.Tables[0].Rows[0]["CODSTACON"].ToString();
                    posAss.DataInizioAttivitaPA = dataSet2.Tables[0].Rows[0]["DATINIATT"].ToString();
                    if (!string.IsNullOrEmpty(dataSet2.Tables[0].Rows[0]["CODATTCAM"].ToString()))
                        posAss.CodiceAttivitaAziPA = dataSet2.Tables[0].Rows[0]["DENATTCAM"].ToString();
                    if (!string.IsNullOrEmpty(dataSet2.Tables[0].Rows[0]["DENCODSTA"].ToString()))
                        posAss.CodiceStatisticoAziPA = dataSet2.Tables[0].Rows[0]["DENCODSTA"].ToString();
                    posAss.TipoIndirizzoPA = dataSet4.Tables[0].Rows[0]["TIPIND"].ToString();
                    posAss.CODDUGPA = Convert.ToInt32(dataSet4.Tables[0].Rows[0]["CODDUG"].ToString());
                    posAss.DENDUGPA = dataSet4.Tables[0].Rows[0]["DENDUG"].ToString();
                    posAss.DENINDPA = dataSet4.Tables[0].Rows[0]["DENIND"].ToString();
                    posAss.IndirizzoPA = dataSet4.Tables[0].Rows[0]["IND"].ToString();
                    posAss.NumeroCivicoPA = dataSet4.Tables[0].Rows[0]["NUMCIV"].ToString();
                    posAss.CODCOMPA = dataSet4.Tables[0].Rows[0]["CODCOM"].ToString();
                    posAss.ComunePA = dataSet4.Tables[0].Rows[0]["DENCOM"].ToString().Trim();
                    posAss.CAPPA = dataSet4.Tables[0].Rows[0]["CAP"].ToString();
                    posAss.ProvinciaPA = dataSet4.Tables[0].Rows[0]["SIGPRO"].ToString();
                    posAss.LocalitaPA = dataSet4.Tables[0].Rows[0]["DENLOC"].ToString();
                    posAss.StatoEsteroPA = dataSet4.Tables[0].Rows[0]["DENSTAEST"].ToString();
                    posAss.Telefono1PA = dataSet4.Tables[0].Rows[0]["TEL1"].ToString();
                    posAss.Telefono2PA = dataSet4.Tables[0].Rows[0]["TEL2"].ToString();
                    posAss.FaxPA = dataSet4.Tables[0].Rows[0]["FAX"].ToString();
                    posAss.EmailPA = dataSet4.Tables[0].Rows[0]["EMAIL"].ToString();
                    posAss.EmailCertificataPA = dataSet4.Tables[0].Rows[0]["EMAILCERT"].ToString();
                    posAss.AGGMANPA = dataSet4.Tables[0].Rows[0]["AGGMAN"].ToString();
                    return posAss;
                }
            }
            catch (Exception ex)
            {
            }
            return (Azienda.PosAss)null;
        }

        public bool AggiornamentoSedi(Azienda.Sedi azs, Utente u, string codPos, DataLayer dataLayer)
        {
            var flag = false;

            var codPosParameter = dataLayer.CreateParameter("@Codpos", iDB2DbType.iDB2Decimal, 8, ParameterDirection.Input, codPos);
            var coddugsParameter = dataLayer.CreateParameter("@Coddugs", iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input, azs.CODDUGS.ToString());
            var viasParameter = dataLayer.CreateParameter("@Vias", iDB2DbType.iDB2VarChar, 80, ParameterDirection.Input, azs.ViaS);
            var civicosParameter = dataLayer.CreateParameter("@Civicos", iDB2DbType.iDB2VarChar, 20, ParameterDirection.Input, azs.CivicoS);
            var comunesParameter = dataLayer.CreateParameter("@Comunes", iDB2DbType.iDB2VarChar, 100, ParameterDirection.Input, azs.ComuneS);
            var capsParameter = dataLayer.CreateParameter("@Caps", iDB2DbType.iDB2VarChar, 9, ParameterDirection.Input, azs.CAPS);
            var provinciasParameter = dataLayer.CreateParameter("@Provincias", iDB2DbType.iDB2Char, 2, ParameterDirection.Input, azs.ProvinciaS);
            var localitasParameter = dataLayer.CreateParameter("@Localitas", iDB2DbType.iDB2VarChar, 100, ParameterDirection.Input, azs.LocalitaS);
            var statoEsteroSParameter = dataLayer.CreateParameter("@StatoEsteroS", iDB2DbType.iDB2VarChar, 50, ParameterDirection.Input, azs.StatoEsteroS);
            var telefono1SParameter = dataLayer.CreateParameter("@Telefono1S", iDB2DbType.iDB2VarChar, 13, ParameterDirection.Input, azs.Telefono1S);
            var usernameParameter = dataLayer.CreateParameter("@Username", iDB2DbType.iDB2VarChar, 20, ParameterDirection.Input, u.Username);
            var ids2Parameter = dataLayer.CreateParameter("@Ids2", iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input, "2");
            var dencomParameter = dataLayer.CreateParameter("@Codcom", iDB2DbType.iDB2Char, 4, ParameterDirection.Input, null);

            var todayIndSedRecordSqlQuery = "SELECT * FROM INDSED WHERE CODPOS = @Codpos AND DATINI = CURRENT_DATE AND TIPIND = @Id2s";
            var nextProrecSqlQuery = "SELECT VALUE(MAX(PROREC), 0) + 1 AS PROREC FROM INDSED WHERE CODPOS = @Codpos";
            var dencomSqlQuery = "SELECT DENCOM FROM CODCOM WHERE CODCOM = @Comunes";
            var insertIndSedSqlCommand =
                "INSERT INTO INDSED (CODPOS, PROREC, TIPIND, DATCOM, DATINI, DATFIN, CODDUG, IND, NUMCIV, CODCOM, " +
                "CAP, SIGPRO, DENLOC, DENSTAEST, AGGMAN, TEL1, CODMEZ, ULTAGG, UTEAGG, DENCOM) " +
                "VALUES (@Codpos, @Prorec, @Ids2, CURRENT_DATE, CURRENT_DATE, '9999-12-31', @Coddugs, @Vias, @Civicos, @Comunes, " +
                "@Caps, @Provincias, @Localitas, @StatoEsteroS, 'S', @Telefono1S, 0, CURRENT_TIMESTAMP, @Username, @Dencom)";
            var updateTodayIndSedRecordSqlCommand =
                "UPDATE INDSED SET " +
                $"DATFIN = '{DateTime.Now.AddDays(-1):yyyy-MM-dd}', " +
                "ULTAGG = CURRENT_TIMESTAMP, " +
                "UTEAGG = @Username " +
                "WHERE CODPOS = @Codpos AND TIPIND = @Ids2 AND DATINI = " +
                    "(SELECT MAX(DATINI) FROM INDSED WHERE CODPOS = @Codpos AND TIPIND = @Id2s)";
            var upsertIndSedSqlCommand =
                "UPDATE INDSED SET CODDUG = @Coddugs, IND = @Vias, NUMCIV = @Civicos, CODCOM = @Comunes, CAP = @Caps, SIGPRO = @Provincias, DENLOC = @Localitas, " +
                "ULTAGG = CURRENT_TIMESTAMP, DENSTAEST = @StatoEsteroS, TEL1 = @Telefono1S, DENCOM = @Dencom, UTEAGG = @Username " +
                "WHERE CODPOS = @Codpos AND TIPIND = @Ids2 AND DATINI = (SELECT MAX(DATINI) FROM INDSED WHERE CODPOS = @Codpos AND TIPIND = @Id2s)";

            if (string.IsNullOrWhiteSpace(azs.StatoEsteroS))
            {
                var dencom = dataLayer.GetDataTableWithParameters(dencomSqlQuery, comunesParameter).Rows[0][0].ToString();
                dencomParameter = dataLayer.CreateParameter("@Dencom", iDB2DbType.iDB2Char, 4, ParameterDirection.Input, dencom);
            }

            var todayIndSedRecord = dataLayer.GetDataTableWithParameters(todayIndSedRecordSqlQuery, codPosParameter, ids2Parameter);
            if (todayIndSedRecord.Rows.Count > 0)
            {
                flag = dataLayer.WriteTransactionDataWithParametersAndDontCall(upsertIndSedSqlCommand, CommandType.Text, coddugsParameter, viasParameter, civicosParameter,
                    dencomParameter, capsParameter, provinciasParameter, localitasParameter, statoEsteroSParameter, telefono1SParameter, comunesParameter, usernameParameter, codPosParameter,
                    ids2Parameter, codPosParameter, ids2Parameter);
            }
            else
            {
                flag = dataLayer.WriteTransactionDataWithParametersAndDontCall(updateTodayIndSedRecordSqlCommand, CommandType.Text,
                    usernameParameter, codPosParameter, ids2Parameter, codPosParameter, ids2Parameter);

                if (!flag)
                    throw new Exception("Update non riuscito");

                var nextProrec = dataLayer.GetDataTableWithParameters(nextProrecSqlQuery, codPosParameter).Rows[0][0].ToString();
                var prorecParameter = dataLayer.CreateParameter("@Prorec", iDB2DbType.iDB2Decimal, 3, ParameterDirection.Input, nextProrec);

                flag = dataLayer.WriteTransactionDataWithParametersAndDontCall(insertIndSedSqlCommand, CommandType.Text, codPosParameter, prorecParameter,
                    ids2Parameter, coddugsParameter, viasParameter, civicosParameter, dencomParameter, capsParameter, provinciasParameter, localitasParameter,
                    statoEsteroSParameter, telefono1SParameter, usernameParameter, comunesParameter);
            }

            return flag;
        }

        public bool AggiornamentoRapLeg(Azienda.RapLeg rapLeg, Utente u, string codPos, DataLayer dataLayer)
        {
            var codposParam = dataLayer.CreateParameter("@Codpos", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, codPos);
            var usernameParam = dataLayer.CreateParameter("@Username", iDB2DbType.iDB2VarChar, 20, ParameterDirection.Input, u.Username);
            var coddugRpParam = dataLayer.CreateParameter("@CoddugRp", iDB2DbType.iDB2Decimal, 5, ParameterDirection.Input, rapLeg.TipoIndirizzoRP);
            var indirizzoRpParam = dataLayer.CreateParameter("@IndirizzoRp", iDB2DbType.iDB2VarChar, 80, ParameterDirection.Input, rapLeg.IndirizzoRP.ToUpper());
            var civicoRpParam = dataLayer.CreateParameter("@CivicoRp", iDB2DbType.iDB2VarChar, 20, ParameterDirection.Input, rapLeg.CivicoRP);
            var provinciaRpParam = dataLayer.CreateParameter("@ProvinciaRp", iDB2DbType.iDB2Char, 2, ParameterDirection.Input, rapLeg.ProvinciaRP);
            var comuneRpParam = dataLayer.CreateParameter("@ComuneRp", iDB2DbType.iDB2Char, 4, ParameterDirection.Input, rapLeg.CodiceComuneResidenzaRP);
            var localitaRpParam = dataLayer.CreateParameter("@LocalitaRp", iDB2DbType.iDB2VarChar, 100, ParameterDirection.Input, rapLeg.LocalitaRP);
            var capParam = dataLayer.CreateParameter("@CapRp", iDB2DbType.iDB2VarChar, 9, ParameterDirection.Input, rapLeg.CapRP);
            var statoEsteroRpParam = dataLayer.CreateParameter("@StatoEsteroRp", iDB2DbType.iDB2VarChar, 50, ParameterDirection.Input, rapLeg.StatoEsteroRP);

            var prorecSqlQuery = "SELECT VALUE(MAX(PROREC), 0) + 1 AS PROREC FROM AZIRAP WHERE CODPOS = @Codpos";
            var fullCurrentAziRapSqlQuery = "SELECT * FROM AZIRAP WHERE CODPOS = @Codpos AND DATINI = (SELECT MAX(DATINI) FROM AZIRAP WHERE CODPOS = @Codpos)";
            var closePrevAzirapSqlCmd = $"UPDATE AZIRAP SET DATFIN = '{DateTime.Now.AddDays(-1):yyyy-MM-dd}', UTEAGG = @Username, ULTAGG = CURRENT_TIMESTAMP WHERE CODPOS = @Codpos " +
                "AND DATINI = (SELECT MAX(DATINI) FROM AZIRAP WHERE CODPOS = @Codpos)";
            var updateAzirapSqlCmd = "UPDATE AZIRAP SET CODDUG = @CoddugRp, IND = @IndirizzoRp, NUMCIV = @CivicoRp, CODCOMRES = @ComuneRp, DENLOC = @LocalitaRp, SIGPRO = @ProvinciaRp, " +
                "CAP = @CapRp, DENSTAEST = @StatoEsteroRp, ULTAGG = CURRENT_TIMESTAMP WHERE CODPOS = @Codpos AND DATINI = (SELECT MAX(DATINI) FROM AZIRAP WHERE CODPOS = @Codpos)";
            var selectAzirapSqlQuery = "SELECT * FROM AZIRAP WHERE CODPOS = @Codpos AND DATINI = CURRENT_DATE";

            var fullCurrentAzirap = dataLayer.GetDataTableWithParameters(fullCurrentAziRapSqlQuery, codposParam, codposParam).Rows[0];
            var prorec = dataLayer.GetDataTableWithParameters(prorecSqlQuery, codposParam).Rows[0][0];

            var codfunrap = fullCurrentAzirap["CODFUNRAP"] != null ? "'" + fullCurrentAzirap["CODFUNRAP"] + "'" : null;
            var rappri = fullCurrentAzirap["RAPPRI"] != null ? "'" + fullCurrentAzirap["RAPPRI"] + "'" : null;
            var cog = fullCurrentAzirap["COG"] != null ? "'" + fullCurrentAzirap["COG"] + "'" : null;
            var nom = fullCurrentAzirap["NOM"] != null ? "'" + fullCurrentAzirap["NOM"] + "'" : null;
            var tel1 = fullCurrentAzirap["TEL1"] != null ? "'" + fullCurrentAzirap["TEL1"] + "'" : null;
            var tel2 = fullCurrentAzirap["TEL2"] != null ? "'" + fullCurrentAzirap["TEL2"] + "'" : null;
            var fax = fullCurrentAzirap["FAX"] != null ? "'" + fullCurrentAzirap["FAX"] + "'" : null;
            var email = fullCurrentAzirap["EMAIL"] != null ? "'" + fullCurrentAzirap["EMAIL"] + "'" : null;
            var codfis = fullCurrentAzirap["CODFIS"] != null ? "'" + fullCurrentAzirap["CODFIS"] + "'" : null;
            var datnas = fullCurrentAzirap["DATNAS"] != null ? $"{fullCurrentAzirap["DATNAS"]}" : null;
            var codcommnas = fullCurrentAzirap["CODCOMNAS"] != null ? "'" + fullCurrentAzirap["CODCOMNAS"] + "'" : null;
            var ses = fullCurrentAzirap["SES"] != null ? "'" + fullCurrentAzirap["SES"] + "'" : null;
            var comez = fullCurrentAzirap["CODMEZ"] != null ? "'" + fullCurrentAzirap["CODMEZ"] + "'" : null;
            var datcom = fullCurrentAzirap["DATCOM"] != null ? $"{fullCurrentAzirap["DATCOM"]}" : null;
            var datsap = fullCurrentAzirap["DATSAP"] != null ? "'" + fullCurrentAzirap["DATSAP"] + "'" : null;
            var dencomres = fullCurrentAzirap["DENCOMRES"] != null ? "'" + fullCurrentAzirap["DENCOMRES"] + "'" : null;
            var emailcert = fullCurrentAzirap["EMAILCERT"] != null ? "'" + fullCurrentAzirap["EMAILCERT"] + "'" : null;
            var cell = fullCurrentAzirap["CELL"] != null ? "'" + fullCurrentAzirap["CELL"] + "'" : null;

            var insertAzirapSqlCmd = "INSERT INTO AZIRAP " +
                                     "(CODPOS, PROREC, DATINI, DATFIN, CODFUNRAP, RAPPRI, COG, " +
                                     "NOM, CODDUG, IND, NUMCIV, DENSTAEST, DENLOC, CAP, SIGPRO, TEL1, " +
                                     "TEL2, FAX, EMAIL, CODFIS, DATNAS, " +
                                     "CODCOMNAS, SES, CODMEZ, DATCOM, DATSAP, " +
                                     "ULTAGG, UTEAGG, CODCOMRES, DENCOMRES, EMAILCERT, CELL)" +
                                     $" VALUES (@Codpos, {prorec}, CURRENT_DATE, '9999-12-31', {codfunrap}, {rappri}, {cog}, " +
                                     $"{nom}, @CoddugRp, @IndirizzoRp, @CivicoRp, @StatoEsteroRp, @LocalitaRp, @CapRp, @ProvinciaRp, {tel1}, " +
                                     $"{tel2}, {fax}, {email}, {codfis}, '{datnas.ToDomainDateStringFormat()}', " +
                                     $"{codcommnas}, {ses}, {comez}, '{datcom.ToDomainDateStringFormat()}', NULL, " +
                                     $"CURRENT_TIMESTAMP, @Username, @ComuneRp, {dencomres}, {emailcert}, {cell})";

            var isAzirapRecordUpdatedToday = dataLayer.GetDataTableWithParameters(selectAzirapSqlQuery, codposParam).Rows.Count > 0;
            var flag1 = true;
            var flag2 = true;
            if (isAzirapRecordUpdatedToday)
            {
                flag1 = dataLayer.WriteTransactionDataWithParametersAndDontCall(updateAzirapSqlCmd, CommandType.Text, coddugRpParam, indirizzoRpParam, civicoRpParam,
                    comuneRpParam, localitaRpParam, provinciaRpParam, capParam, statoEsteroRpParam, codposParam, codposParam);
            }
            else
            {
                flag1 = dataLayer.WriteTransactionDataWithParametersAndDontCall(closePrevAzirapSqlCmd, CommandType.Text, usernameParam, codposParam, codposParam);
                flag2 = dataLayer.WriteTransactionDataWithParametersAndDontCall(insertAzirapSqlCmd, CommandType.Text, codposParam, coddugRpParam, indirizzoRpParam, civicoRpParam, statoEsteroRpParam,
                    localitaRpParam, capParam, provinciaRpParam, usernameParam, comuneRpParam);
            }

            return flag1 && flag2;
        }

        public bool AggiornamentoEmail(Azienda.PosAss posAss, Utente u, string codPos, DataLayer dataLayer)
        {
            var emailParam = dataLayer.CreateParameter("@Email", iDB2DbType.iDB2VarChar, 100, ParameterDirection.Input,
                posAss.EmailPA ?? string.Empty);
            var emailCertParam = dataLayer.CreateParameter("@EmailCert", iDB2DbType.iDB2VarChar, 100, ParameterDirection.Input,
                posAss.EmailCertificataPA ?? string.Empty);
            var codposParam = dataLayer.CreateParameter("@Codpos", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, codPos);
            var usernameParam = dataLayer.CreateParameter("@Username", iDB2DbType.iDB2VarChar, 20, ParameterDirection.Input, u.Username);

            var updateCurrAzemailSqlCmd = "UPDATE AZEMAIL SET EMAIL = @Email, EMAILCERT = @EmailCert WHERE CODPOS = @Codpos AND DATINI = " +
                                          "(SELECT MAX(DATINI) FROM AZEMAIL WHERE CODPOS = @Codpos)";
            var closePrevAzemailSqlCmd = $"UPDATE AZEMAIL SET DATFIN = '{DateTime.Now.AddDays(-1):yyyy-MM-dd}', ULTAGG = CURRENT_TIMESTAMP, UTEAGG = @Username";
            var insertCurrAzemailSqlCmd = "INSERT INTO AZEMAIL (CODPOS, DATINI, DATFIN, EMAIL, EMAILCERT, ULTAGG, UTEAGG) VALUES" +
                                          "(@Codpos, CURRENT_DATE, '9999-12-31', @Email, @EmailCert, CURRENT_TIMESTAMP, @Username)";
            var getCurrAzemailSqlCmd = "SELECT CODPOS FROM AZEMAIL WHERE CODPOS = @Codpos AND DATINI = CURRENT_DATE";

            bool flag1 = true;
            bool flag2 = true;
            var isAzemailRecordUpdatedToday = dataLayer.GetDataTableWithParameters(getCurrAzemailSqlCmd, codposParam).Rows.Count > 0;

            if (isAzemailRecordUpdatedToday)
            {
                flag1 = dataLayer.WriteTransactionDataWithParametersAndDontCall(updateCurrAzemailSqlCmd, CommandType.Text, emailParam, emailCertParam,
                    codposParam, codposParam);
            }
            else
            {
                flag1 = dataLayer.WriteTransactionDataWithParametersAndDontCall(closePrevAzemailSqlCmd, CommandType.Text, usernameParam);
                flag2 = dataLayer.WriteTransactionDataWithParametersAndDontCall(insertCurrAzemailSqlCmd, CommandType.Text, codposParam, emailParam,
                    emailCertParam, usernameParam);
            }

            return flag1 && flag2;
        }

        public void UpdateAzienda(Azienda azienda, Utente u, string codpos, ref string ErroreMsg, ref string SuccessMsg)
        {
            DataLayer dataLayer = new DataLayer();
            dataLayer.StartTransaction();
            try
            {
                var flag1 = AggiornamentoRapLeg(azienda.rapLeg, u, codpos, dataLayer);
                var flag2 = AggiornamentoEmail(azienda.posAss, u, codpos, dataLayer);
                var flag3 = AggiornamentoSedi(azienda.sedi, u, codpos, dataLayer);

                if (flag1 && flag2 && flag3)
                {
                    SuccessMsg = "Modifica effettuata con successo!";
                    dataLayer.EndTransaction(true);
                    return;
                }

                ErroreMsg = "Si sono verificati problemi nel salvataggio!";
                dataLayer.EndTransaction(false);
            }
            catch
            {
                ErroreMsg = "Si sono verificati problemi nel salvataggio!";
                dataLayer.EndTransaction(false);
                throw;
            }
        }
    }
}
