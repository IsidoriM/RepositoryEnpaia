// Decompiled with JetBrains decompiler
// Type: TFI.DAL.Iscritto.AnagraficaDAL
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using IBM.Data.DB2.iSeries;
using OCM.TFI.OCM.AziendaConsulente;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using TFI.DAL.ConnectorDB;
using TFI.OCM.AziendaConsulente;
using static TFI.OCM.AziendaConsulente.CessazioniRdl;
using static TFI.OCM.AziendaConsulente.Azienda;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Drawing;
using TFI.OCM.Utente;
using TFI.OCM.Iscritto;
using static TFI.OCM.Amministrativo.GestioneAziendeWebOCM;
using  Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using Itenso.TimePeriod;
using System.Globalization;
using System.Data.Sql;
using DataTable = System.Data.DataTable;
using TFI.DAL.Utilities;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using System.Configuration;
using TFI.CRYPTO.Crypto;
using Microsoft.Win32;
using DocumentFormat.OpenXml.Office2013.Word;
using TFI.OCM.Utilities;
using OCM.TFI.OCM.Iscritto;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;


namespace TFI.DAL.Infortuni
{
    public class InfortuniDAL
    {
        private readonly DataLayer objDataAccess = new DataLayer();

        public NuovoInfortunio GetArea(String matr)
        {
            
            try
            {
                

                string strSQL = "";

                
               
                strSQL = " SELECT D.DENLOC AS DENLOC,D.IND AS INDI,C.CODREG as CODREG,C.DENOM AS DENOM,D.CODDUG,(SELECT DISTINCT(K.DENDUG) FROM DBUNICOSV.DUG K WHERE K.CODDUG = D.CODDUG) AS VIA   FROM DBUNICOSV.REGIONI C " +
                         " INNER JOIN DBUNICOSV.ISCD D  ON D.SIGPRO = C.CODPROV  WHERE D.MAT = " + matr + " ORDER BY D.DATINI DESC LIMIT 1 ";


                    NuovoInfortunio InfortuniArea = new NuovoInfortunio();
                    DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL);
                    if (this.objDataAccess.queryOk(dataTable2))
                    {

                        foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                            return new NuovoInfortunio()

                            {

                                AreaPer = dataTable2.Rows[0]["DENOM"].ToString(),
                                Via = dataTable2.Rows[0]["VIA"].ToString(),
                                Localita = dataTable2.Rows[0]["DENLOC"].ToString(),
                                Indirizzo = dataTable2.Rows[0]["INDI"].ToString(),

                            };
                        return InfortuniArea;

                    };
                



            
            }
            catch (Exception ex)
            {
                return (NuovoInfortunio)null;
            }
            return (NuovoInfortunio)null; 
        }



        public List<Tipologie> GetTipiInfortuni()
        {
            string Err = "";
            try
            {
                
                string strSQL = "Select CODINF, DESCRTIPO FROM INFORTUSVI.TBTIPOINF ORDER BY DESCRTIPO ASC";
                DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL);
                if (this.objDataAccess.queryOk(dataTable2))
                {
                    List<Tipologie> TipoInfortuni = new List<Tipologie>();
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                        TipoInfortuni.Add(new Tipologie()

                        {

                            Id = row["CODINF"].ToString(),
                            Descrizione = row["DESCRTIPO"].ToString(),


                        });
                    return TipoInfortuni;

                };
            }
            catch (Exception ex)
            {
                return (List<Tipologie>)null;
            }
            return (List<Tipologie>)null;  
        }
        public List<Tipologie> GetForma()
        {
            string Err = "";
            try
            {
                DataSet dataSet1 = new DataSet();
                string strSQL = "SELECT CODFORM, DESCRFORM FROM INFORTUSVI.TBFORMINF ORDER BY DESCRFORM ASC ";
                dataSet1 = this.objDataAccess.GetDataSet(strSQL, ref Err);
                DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL);
                if (this.objDataAccess.queryOk(dataTable2))
                {
                    List<Tipologie> TipoForma = new List<Tipologie>();
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                        TipoForma.Add(new Tipologie()

                        {

                            Id = row["CODFORM"].ToString(),
                            Descrizione = row["DESCRFORM"].ToString(),


                        });
                    return TipoForma;

                };
            }
            catch (Exception ex)
            {
                return (List<Tipologie>)null;
            }
            return (List<Tipologie>)null; 
        }
        public List<Tipologie> GetAgente()
        {
            string Err = "";
            try
            {
                DataSet dataSet1 = new DataSet();
                string strSQL = "select CODAGE,DESCRAGE from INFORTUSVI.TBAGEINF ORDER BY DESCRAGE ASC ";
                dataSet1 = this.objDataAccess.GetDataSet(strSQL, ref Err);
                DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL);
                if (this.objDataAccess.queryOk(dataTable2))
                {
                    List<Tipologie> TipoAgente = new List<Tipologie>();
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                        TipoAgente.Add(new Tipologie()

                        {

                            Id = row["CODAGE"].ToString(),
                            Descrizione = row["DESCRAGE"].ToString(),


                        });
                    return TipoAgente;

                };
            }
            catch (Exception ex)
            {
                return (List<Tipologie>)null;
            }
            return (List<Tipologie>)null;
        }

        public List<Tipologie> GetCause()
        {
            string Err = "";
            try
            {
                DataSet dataSet1 = new DataSet();
                string strSQL = "SELECT CODINF, DESCRINF FROM INFORTUSVI.TBCAUINF order by DESCRINF ASC ";
                dataSet1 = this.objDataAccess.GetDataSet(strSQL, ref Err);
                DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL);
                if (this.objDataAccess.queryOk(dataTable2))
                {
                    List<Tipologie> TipoCause = new List<Tipologie>();
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                        TipoCause.Add(new Tipologie()

                        {

                            Id = row["CODINF"].ToString(),
                            Descrizione = row["DESCRINF"].ToString(),


                        });
                    return TipoCause;

                };
            }
            catch (Exception ex)
            {
                return (List<Tipologie>)null;
            }
            return (List<Tipologie>)null; 
        }


        public NuovoInfortunio GetRap(string codpos,string matr)
        {
            string Err = "";
            try
            {
                
                string strSQL = "SELECT PRORAP as CODRAP FROM RAPLAV WHERE CODPOS = " + codpos +  " AND MAT = " + matr + " AND '2022-12-31' BETWEEN DATDEC AND VALUE(DATCES, '9999-12-31')  ";
                
                NuovoInfortunio tiprap = new NuovoInfortunio();
                DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL);
                if (this.objDataAccess.queryOk(dataTable2))
                {
                   
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                        return new NuovoInfortunio()

                        {

                          Codstapra   = dataTable2.Rows[0]["CODRAP"].ToString(),
                            
                        };
                    return tiprap;

                };
            }
            catch (Exception ex)
            {
                return (NuovoInfortunio)null;
            }
            return (NuovoInfortunio)null;
        }

        public NuovoInfortunio GetRapinf(string codpos)
        {
            string Err = "";
            try
            {
                
                string strSQL = "SELECT MAX(PROINF) AS PROINF  from INFORTUSVI.TBPRATICHE  where CODPOS =" +  codpos + "";
                NuovoInfortunio tiprap = new NuovoInfortunio();
                DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL);
                if (this.objDataAccess.queryOk(dataTable2))
                {

                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                        return new NuovoInfortunio()

                        {

                            Proinf = dataTable2.Rows[0]["PROINF"].ToString(),

                        };
                    return tiprap;

                };
            }
            catch (Exception ex)
            {
                return (NuovoInfortunio)null;
            }
            return (NuovoInfortunio)null;
        }

        public Pratiche GetRapdoc()
        {
            string Err = "";
            try
            {
                
                string strSQL = " SELECT MAX(IDDOC) as IDDOC from INFORTUSVI.TBPRADOC ";
                Pratiche tiprap = new Pratiche();
                DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL);
                if (this.objDataAccess.queryOk(dataTable2))
                {

                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                        return new Pratiche()

                        {

                            Pradoc = dataTable2.Rows[0]["IDDOC"].ToString(),

                        };
                    return tiprap;

                };
            }
            catch (Exception ex)
            {
                return (Pratiche)null;
            }
            return (Pratiche)null;
        }

        public List<Pratiche> GetTipoPratiche()
        {
            string Err = "";
            try
            {
                DataSet dataSet1 = new DataSet();
                string strSQL = "SELECT IDTIPODOC,DESCRIZIONE FROM INFORTUSVI.TBTIPODOC";
                dataSet1 = this.objDataAccess.GetDataSet(strSQL, ref Err);
                DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL);
                if (this.objDataAccess.queryOk(dataTable2))
                {
                    List<Pratiche> tipprat = new List<Pratiche>();
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                        tipprat.Add(new Pratiche()

                        {
                            ID = row["IDTIPODOC"].ToString(),
                            Descrizione = row["DESCRIZIONE"].ToString(),

                        });
                    return tipprat;

                };
            }
            catch (Exception ex)
            {
                return (List<Pratiche>)null;
            }
            return (List<Pratiche>)null;
        }
        public List<Tipologie> GetSede()
        {
            string Err = "";
            try
            {
                DataSet dataSet1 = new DataSet();
                string strSQL = "SELECT CODSEDE, DESCRSEDE FROM INFORTUSVI.TBSEDEINF order by DESCRSEDE asc  ";
                dataSet1 = this.objDataAccess.GetDataSet(strSQL, ref Err);
                DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL);
                if (this.objDataAccess.queryOk(dataTable2))
                {
                    List<Tipologie> TipoSede = new List<Tipologie>();
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                        TipoSede.Add(new Tipologie()

                        {

                            Id = row["CODSEDE"].ToString(),
                            Descrizione = row["DESCRSEDE"].ToString(),


                        });
                    return TipoSede;

                };
            }
            catch (Exception ex)
            {
                return (List<Tipologie>)null;
            }
            return (List<Tipologie>)null;
        }
        public NuovoInfortunio GetMatricola(String codpos, string matr,string cognome, string nome)
        {
            string Err = "";
            try
            {
                DataSet dataSet1 = new DataSet();
                string strSQL = "";

                
                if (matr.ToString() == "")
                {
                    matr = null;
                }
                if (cognome.ToString() == "")
                {
                    cognome = null;
                }
                if (nome.ToString() == "")
                {
                    nome = null;
                }

                if(matr == null && cognome == null  ) { return null; };

                strSQL = "SELECT A.CODPOS, A.MAT, A.PRORAP, A.ULTAGG, A.UTEAGG ,B.COG,B.NOM,B.CODFIS, C.DENQUA AS QUACON " +
                                " FROM DBUNICOSV.RAPLAV A INNER JOIN DBUNICOSV.ISCT B ON A.MAT = B.MAT " +
                                "LEFT  JOIN DBUNICOSV.QUACON C ON C.CODQUACON = A.CODQUACON " +
                                " WHERE a.CODPOS = " + codpos + " and A.MAT =" + matr +
                                " AND A.DATDENCES is null";

                if (cognome != null && nome != null && matr == null  )
                {
                    strSQL = "SELECT A.CODPOS, A.MAT, A.PRORAP, A.ULTAGG, A.UTEAGG ,B.COG,B.NOM,B.CODFIS, C.DENQUA AS QUACON " +
                                  " FROM DBUNICOSV.RAPLAV A INNER JOIN DBUNICOSV.ISCT B ON A.MAT = B.MAT " +
                                  "LEFT  JOIN DBUNICOSV.QUACON C ON C.CODQUACON = A.CODQUACON " +
                                  " WHERE a.CODPOS = " + codpos + " and B.COG='" + cognome + "'" +" and B.NOM='" + nome + "'" +   
                                  " AND A.DATDENCES is null";
                }
                
                
               
                
                DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL);
                if (this.objDataAccess.queryOk(dataTable2))
                {
                    NuovoInfortunio Infortuni = new NuovoInfortunio();
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                        return new NuovoInfortunio()

                        {

                            CodPos = dataTable2.Rows[0]["CODPOS"].ToString(),
                            Matricola = dataTable2.Rows[0]["MAT"].ToString(),
                            Ultagg = dataTable2.Rows[0]["ULTAGG"].ToString(),
                            Uteagg = dataTable2.Rows[0]["UTEAGG"].ToString(),
                            Cognome = dataTable2.Rows[0]["COG"].ToString(),
                            Nome = dataTable2.Rows[0]["NOM"].ToString(),
                            Codfiscale = dataTable2.Rows[0]["CODFIS"].ToString(),
                            Tipodip = dataTable2.Rows[0]["QUACON"].ToString(),

                        };
                    return Infortuni;





                };
            }
            catch (Exception ex)
            {
                return (NuovoInfortunio)null;
            }
            return null;
        }

        public List<NuovoInfortunio> GetPratiche(String codpos)
        {
            string Err = "";
            try
            {
                DataSet dataSet1 = new DataSet();
                string strSQL = "";



                if (codpos != null)
                {
                    strSQL = "SELECT A.CODPOS, A.MAT, B.COG,B.NOM,D.DATINF,D.DATDEN,D.TIPINF,(SELECT DISTINCT DESCRTIPO FROM INFORTUSVI.TBTIPOINF  WHERE CODINF = D.TIPINF) AS DESC ,B.CODFIS, C.DENQUA AS QUACON ,D.PROINF  " +
                                    " FROM DBUNICOSV.RAPLAV A INNER JOIN DBUNICOSV.ISCT B ON A.MAT = B.MAT " +
                                    " LEFT  JOIN DBUNICOSV.QUACON C ON C.CODQUACON = A.CODQUACON" +
                                    " INNER JOIN INFORTUSVI.TBPRATICHE D on A.MAT = D.MAT AND B.MAT = D.MAT" +
                                    " WHERE a.CODPOS = " + codpos + "  AND A.DATDENCES is null  " +
                                    " GROUP BY A.CODPOS, A.MAT, B.COG,B.NOM,B.CODFIS, C.DENQUA,D.PROINF ,D.TIPINF,D.DATINF,D.DATDEN   " +
                                    " order by D.DATDEN desc, D.DATINF asc";

                }

                
                dataSet1 = this.objDataAccess.GetDataSet(strSQL, ref Err);
                DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL);
                if (this.objDataAccess.queryOk(dataTable2))
                {
                    List<NuovoInfortunio> Infortuni = new List<NuovoInfortunio>();
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                        Infortuni.Add(new NuovoInfortunio()

                        {

                            
                            Matricola = row["MAT"].ToString(),
                            Cognome = row["COG"].ToString(),
                            Nome = row["NOM"].ToString(),
                            Prognosi = row["DESC"].ToString(),
                            Datainf = row["DATINF"].ToString(),
                            Dataden = row["DATDEN"].ToString(),
                            Proinf = row["PROINF"].ToString(),
                        });
                    return Infortuni;





                };
            }
            catch (Exception ex)
            {
                return (List<NuovoInfortunio>)null;
            }
            return (List<NuovoInfortunio>)null; 
        }

        public NuovoInfortunio GetPraticheDettaglio(String codpos,string matr,String ID)
        {
            string Err = "";
            try
            {
                DataSet dataSet1 = new DataSet();
                string strSQL = "";




                strSQL = "SELECT A.CODPOS, A.MAT, B.COG,B.NOM,D.DATINF,D.DATDEN,D.TIPINF,(SELECT DISTINCT DESCRTIPO FROM INFORTUSVI.TBTIPOINF  WHERE CODINF = D.TIPINF) AS DESC ,B.CODFIS, C.DENQUA AS QUACON ,D.PROINF  " +
                                " FROM DBUNICOSV.RAPLAV A INNER JOIN DBUNICOSV.ISCT B ON A.MAT = B.MAT " +
                                " LEFT  JOIN DBUNICOSV.QUACON C ON C.CODQUACON = A.CODQUACON" +
                                " INNER JOIN INFORTUSVI.TBPRATICHE D on A.MAT = D.MAT AND B.MAT = D.MAT" +
                                " WHERE a.CODPOS = " + codpos + " AND a.MAT = " + matr  + "  AND A.DATDENCES is null AND D.PROINF = " + ID  +
                                " GROUP BY A.CODPOS, A.MAT, B.COG,B.NOM,B.CODFIS, C.DENQUA,D.PROINF ,D.TIPINF,D.DATINF,D.DATDEN   ";



                
                DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL);
                if (this.objDataAccess.queryOk(dataTable2))
                {
                    NuovoInfortunio Infortuni = new NuovoInfortunio();
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                        return new NuovoInfortunio()

                        {


                            Matricola = dataTable2.Rows[0]["MAT"].ToString(),
                            Cognome = dataTable2.Rows[0]["COG"].ToString(),
                            Nome = dataTable2.Rows[0]["NOM"].ToString(),
                            Prognosi = dataTable2.Rows[0]["DESC"].ToString(),
                            Datainf = dataTable2.Rows[0]["DATINF"].ToString(),
                            Dataden = dataTable2.Rows[0]["DATDEN"].ToString(),
                            Codfiscale = dataTable2.Rows[0]["CODFIS"].ToString(),
                            Descrizione = dataTable2.Rows[0]["DESC"].ToString(),
                        };
                    return Infortuni;





                };
            }
            catch (Exception ex)
            {
                return (NuovoInfortunio)null;
            }
            return (NuovoInfortunio)null; 
        }

        public List<NuovoInfortunio> GetMatricole(String codpos)
        {
            string Err = "";
            try
            {
                DataSet dataSet1 = new DataSet();
                string strSQL = "SELECT distinct( A.MAT )" +
                                 " FROM DBUNICOSV.RAPLAV A INNER JOIN DBUNICOSV.ISCT B ON A.MAT = B.MAT " +
                                 " WHERE CODPOS = " + codpos + " and DATDENCES is null order by A.MAT ASC ";


                dataSet1 = this.objDataAccess.GetDataSet(strSQL, ref Err);
                DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL);
                if (this.objDataAccess.queryOk(dataTable2))
                {
                    List<NuovoInfortunio> Anagrafica = new List<NuovoInfortunio>();
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                        Anagrafica.Add(new NuovoInfortunio()

                        {
                           Matricola = row["MAT"].ToString(),

                        });
                    return Anagrafica;

                };
            }
            catch (Exception ex)
            {
                return (List<NuovoInfortunio>)null;
            }
            return (List<NuovoInfortunio>)null; 
        }


        public List<Tipologie> GetNatura()
        {
            string Err = "";
            try
            {
                DataSet dataSet1 = new DataSet();
                string strSQL = "SELECT CODNAT, DESCRNAT FROM INFORTUSVI.TBNATINF order by DESCRNAT  ";
                dataSet1 = this.objDataAccess.GetDataSet(strSQL, ref Err);
                DataTable dataTable2 = this.objDataAccess.GetDataTable(strSQL);
                if (this.objDataAccess.queryOk(dataTable2))
                {
                    List<Tipologie> TipNAT = new List<Tipologie>();
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                        TipNAT.Add(new Tipologie()

                        {

                            Id = row["CODNAT"].ToString(),
                            Descrizione = row["DESCRNAT"].ToString(),


                        });
                    return TipNAT;

                };
            }
            catch (Exception ex)
            {
                return (List<Tipologie>)null;
            }
            return (List<Tipologie>)null;  
        }

        public bool  SalvaAllegati(string rapdoc,string proinf,string tipdoc,string idproto,string idalleg,string uuid,string ute)
        {
            DataLayer dataLayer = new DataLayer();
            dataLayer.StartTransaction();

            
            

            string Proinf = "0";

            if (proinf != "")
            {
                Proinf = proinf;
            }

            string Tipdoc = "0";

            if (tipdoc != "")
            {
                Tipdoc = tipdoc;
            }

            string Idproto = "0";

            if (idproto != "")
            {
                Idproto = idproto;
            }

            string Idallegato = "0";

            if (idalleg != "")
            {
                Idallegato = idalleg;
            }

            var iddoc = dataLayer.CreateParameter("@Iddoc", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, rapdoc);
            var proinfp = dataLayer.CreateParameter("@Proinf", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, Proinf);
            var tipopratica = dataLayer.CreateParameter("@Tipo", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, Tipdoc);
            var idprotoc = dataLayer.CreateParameter("@Idprot", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, Idproto);
            var idallega = dataLayer.CreateParameter("@Idallega", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, Idallegato);
            var uuidp =    dataLayer.CreateParameter("@UUID", iDB2DbType.iDB2VarChar, 100, ParameterDirection.Input, uuid);
            var utente = dataLayer.CreateParameter("@Utente", iDB2DbType.iDB2VarChar, 100, ParameterDirection.Input, ute);

            var insertPratica = "INSERT INTO INFORTUSVI.TBPRADOC (IDDOC, IDPRA, IDTIPODOC, IDPROTOCOLLO, IDALLEGATO, UUID, ULTAGG, UTEAGG)" +
                    " values(@Iddoc,@Proinf,@Tipo,@Idprot,@Idallega,@UUID,CURRENT_TIMESTAMP,@Utente)";


            Boolean flag = dataLayer.WriteTransactionDataWithParametersAndDontCall(insertPratica, CommandType.Text, iddoc, proinfp,
                tipopratica, idprotoc, idallega, uuidp, utente);
            if (flag)
            {
                
                dataLayer.EndTransaction(true);
                return true;
            }
            else
            {
                dataLayer.EndTransaction(false);
                return false;
            }
            
        }

        public bool  SalvaPratica(List<NuovoInfortunio> json,string utente,String Prorap,String Proinf, string protoc) {

            DataLayer dataLayer = new DataLayer();
            dataLayer.StartTransaction();

            string msgErrore = "";

            
            
            try
            {
                foreach (var riga in json)
                {
                    string Codpos = riga.CodPos;
                    int codpos = Convert.ToInt32(Codpos);

                    string Matr = riga.Matricola;

                    string Prognosi = "";
                    if (riga.Prognosi.ToString() == string.Empty) { Prognosi = "0"; }
                    else {
                        Prognosi = riga.Prognosi.ToString();

                    };

                    string Causa = "";
                    if (riga.Codcauinf.ToString() == string.Empty) { Causa = "0"; }
                    else
                    {
                        Causa = riga.Codcauinf.ToString();

                    };

                    string Sede = "";
                    if (riga.Codsedinf.ToString() == string.Empty) { Sede = "0"; }
                    else
                    {
                        Sede = riga.Codcauinf.ToString();

                    };

                    string Natura = "";
                    if (riga.Codnatinf.ToString() == string.Empty) { Natura = "0"; }
                    else
                    {
                        Natura = riga.Codnatinf.ToString();

                    };

                    string Agente = "";
                    if (riga.Codageinf.ToString() == string.Empty) { Agente = "0"; }
                    else
                    {
                        Agente = riga.Codageinf.ToString();

                    };
                    string TipoAgente = "";
                    if (riga.Tipage.ToString() == string.Empty) { TipoAgente = "0"; }
                    else
                    {
                        TipoAgente = riga.Tipage.ToString();

                    };

                    string Forma = "";
                    if (riga.Codform.ToString() == string.Empty) { Forma = "0"; }
                    else
                    {
                        Forma = riga.Codform.ToString();

                    };
                   
                    string Protoc = "";
                    if (protoc.ToString() == string.Empty) { Protoc = "0"; }
                    else
                    {
                        Protoc = protoc.ToString();

                    };

                   

                   

                    String Dt1 = riga.Dataden.Replace('/','-');
                    String Dt2 = riga.Datainf.Replace('/', '-');
                    
                   


                    var codposParam = dataLayer.CreateParameter("@Codpos", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, Codpos);
                    var matr = dataLayer.CreateParameter("@Matr", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, Matr);
                    var prorap = dataLayer.CreateParameter("@Prorap", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, Prorap);
                    var proinf = dataLayer.CreateParameter("@Proinf", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input,Proinf);
                    var datinf = dataLayer.CreateParameter("@Datinf", iDB2DbType.iDB2Date, 10, ParameterDirection.Input, Dt2);
                    var tipinf = dataLayer.CreateParameter("@Tipinf", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, riga.Tipoinf);
                    
                     var indrd = dataLayer.CreateParameter("@Indrd", iDB2DbType.iDB2Char, 1, ParameterDirection.Input, riga.Indrd);
                     var diagnosi = dataLayer.CreateParameter("@Diagnosi", iDB2DbType.iDB2Char, 1000, ParameterDirection.Input, riga.Diagnosi);
                     
                    var prognosi = dataLayer.CreateParameter("@Prognosi", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, Prognosi);
                    var causa = dataLayer.CreateParameter("@Causa", iDB2DbType.iDB2Integer, 1, ParameterDirection.Input, Causa);
                    var natura = dataLayer.CreateParameter("@Natura", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, Natura);


                    var sede = dataLayer.CreateParameter("@Sede", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, Sede);
                    var agente = dataLayer.CreateParameter("@Agente", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, Agente);
                    var tipage = dataLayer.CreateParameter("@Tipoagente", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, TipoAgente);
                    var codform = dataLayer.CreateParameter("@Codform", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, Forma);

                    var protocollo = dataLayer.CreateParameter("@Prot", iDB2DbType.iDB2Char, 100, ParameterDirection.Input, Protoc);
                    

                    /*
                     var codsta = dataLayer.CreateParameter("@Codsta", iDB2DbType.iDB2Decimal, 10, ParameterDirection.Input, riga.Codstapra);
                     var datann = dataLayer.CreateParameter("@Datann", iDB2DbType.iDB2Date,10, ParameterDirection.Input, riga.Datann);
                     */
                    var uteagg = dataLayer.CreateParameter("@Uteagg", iDB2DbType.iDB2VarChar, 20, ParameterDirection.Input, utente);
                   


                    var insertPratica  = "INSERT INTO INFORTUSVI.TBPRATICHE (CODPOS, MAT, PRORAP, PROINF, DATINF, DATDEN, TIPINF,INDRD,DIAGNOSI,PROGNOSI,CODCAUINF,CODSEDINF,CODNATINF,CODAGEINF,TIPAGE,CODFORM,UTEAGG,ULTAGG,PROT)" +
                      " values(@Codpos,@Matr,@Prorap, @Proinf,@Datinf,Current_date,@Tipinf,@Indrd,@Diagnosi,@Prognosi,@Causa,@Sede,@Natura,@Agente,@Tipoagente,@Codform,@Uteagg,CURRENT_TIMESTAMP,@Prot)";


                    Boolean flag = dataLayer.WriteTransactionDataWithParametersAndDontCall(insertPratica, CommandType.Text, codposParam, matr,
                        prorap, proinf,datinf,tipinf, indrd, diagnosi,prognosi,causa,sede, natura,agente,tipage,codform,uteagg,protocollo);
                    if (flag)
                    {
                        dataLayer.EndTransaction(true);
                        return true;
                    }
                    else
                    {
                        
                        dataLayer.EndTransaction(false);
                        return false;
                    }
                    
                }
                return false;
            }
            catch (Exception ex)
            {
                dataLayer.EndTransaction(false);
                msgErrore = ex.Message;
                msgErrore += ex.ToString();
                return false;
            }



            


        }

   
    }
            
            

        }




