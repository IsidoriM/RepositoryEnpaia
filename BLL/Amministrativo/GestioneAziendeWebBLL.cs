// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Amministrativo.GestioneAziendeWebBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using Newtonsoft.Json;
using OCM.TFI.OCM.AziendaConsulente;
using System;
using System.Collections.Generic;
using System.Web;
using TFI.DAL.Amministrativo;
using TFI.DAL.Utilities;
using TFI.OCM.Amministrativo;

namespace TFI.BLL.Amministrativo
{
    public class GestioneAziendeWebBLL
    {
        private readonly GestioneAziendaWebDAL aziendaDAL = new GestioneAziendaWebDAL();
        private TFI.OCM.Utente.Utente u = HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente;

        public List<GestioneAziendeWebOCM.DatiAzienda> Lista(
          string Codpos,
          string Ragsoc,
          string Partitaiva,
          string codfis,
          ref string ErroreMSG,
          ref string SuccessMSG,
          string Cessato)
        {
            List<GestioneAziendeWebOCM.DatiAzienda> datiAziendaList = new List<GestioneAziendeWebOCM.DatiAzienda>();
            try
            {
                if (!string.IsNullOrEmpty(Ragsoc) || !string.IsNullOrEmpty(Partitaiva) || !string.IsNullOrEmpty(codfis) || !string.IsNullOrEmpty(Codpos) || !string.IsNullOrEmpty(Cessato))
                    datiAziendaList = this.aziendaDAL.Ricerca(Codpos, Ragsoc, Partitaiva, codfis, ref ErroreMSG, ref SuccessMSG, Cessato);
                else
                    ErroreMSG = "inserire almeno un campo di ricerca";
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)datiAziendaList));
                ErroreMSG = "Errore durante l'operazione di ricerca";
            }
            return datiAziendaList;
        }

        public Dictionary<string, string> GetTitStu()
        {
            var dictTitStu = new Dictionary<string, string>();
            try
            {
                dictTitStu = aziendaDAL.GetTitStu();
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject(u), JsonConvert.SerializeObject(dictTitStu));
            }

            return dictTitStu;
        }
        public GestioneAziendeWebOCM.DatiAzienda a(int id)
        {
            GestioneAziendeWebOCM.DatiAzienda datiAzienda = new GestioneAziendeWebOCM.DatiAzienda();
            try
            {
                return this.aziendaDAL.CaricaDatiDettagliAzienda(id);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)datiAzienda));
                return (GestioneAziendeWebOCM.DatiAzienda)null;
            }
        }

        public GestioneAziendeWebOCM.RapLeg b(int id)
        {
            GestioneAziendeWebOCM.RapLeg rapLeg = new GestioneAziendeWebOCM.RapLeg();
            try
            {
                return this.aziendaDAL.CaricaDatiDettagliRapLegale(id);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)rapLeg));
                return (GestioneAziendeWebOCM.RapLeg)null;
            }
        }

        public GestioneAziendeWebOCM.AltriDati c(int id)
        {
            GestioneAziendeWebOCM.AltriDati altriDati = new GestioneAziendeWebOCM.AltriDati();
            try
            {
                return this.aziendaDAL.AltriDati(id);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)altriDati));
                return (GestioneAziendeWebOCM.AltriDati)null;
            }
        }

        public GestioneAziendeWebOCM.Documenti d(int id)
        {
            GestioneAziendeWebOCM.Documenti documenti = new GestioneAziendeWebOCM.Documenti();
            try
            {
                return this.aziendaDAL.Documenti(id);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)documenti));
                return (GestioneAziendeWebOCM.Documenti)null;
            }
        }

        public GestioneAziendeWebOCM.SedeLegale e(int id)
        {
            GestioneAziendeWebOCM.SedeLegale sedeLegale = new GestioneAziendeWebOCM.SedeLegale();
            try
            {
                return this.aziendaDAL.IndirizzoSedeLegale(id);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)sedeLegale));
                return (GestioneAziendeWebOCM.SedeLegale)null;
            }
        }

        public GestioneAziendeWebOCM.SedeAmministrativa f(int id)
        {
            GestioneAziendeWebOCM.SedeAmministrativa sedeAmministrativa = new GestioneAziendeWebOCM.SedeAmministrativa();
            try
            {
                return this.aziendaDAL.IndirizzoSedeAmm(id);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)sedeAmministrativa));
                return (GestioneAziendeWebOCM.SedeAmministrativa)null;
            }
        }

        public GestioneAziendeWebOCM.IndirizzoCorrispondenza g(int id)
        {
            GestioneAziendeWebOCM.IndirizzoCorrispondenza indirizzoCorrispondenza = new GestioneAziendeWebOCM.IndirizzoCorrispondenza();
            try
            {
                return this.aziendaDAL.IndirizzoCorrisp(id);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)indirizzoCorrispondenza));
                return (GestioneAziendeWebOCM.IndirizzoCorrispondenza)null;
            }
        }

        public List<GestioneAziendeWebOCM.NatGiu> natg()
        {
            List<GestioneAziendeWebOCM.NatGiu> natGiuList = new List<GestioneAziendeWebOCM.NatGiu>();
            try
            {
                return this.aziendaDAL.nats();
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)natGiuList));
                return (List<GestioneAziendeWebOCM.NatGiu>)null;
            }
        }

        public List<GestioneAziendeWebOCM.MezzComm> mezzComms()
        {
            List<GestioneAziendeWebOCM.MezzComm> mezzCommList = new List<GestioneAziendeWebOCM.MezzComm>();
            try
            {
                return this.aziendaDAL.mezzs();
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)mezzCommList));
                return (List<GestioneAziendeWebOCM.MezzComm>)null;
            }
        }

        public List<GestioneAziendeWebOCM.TipoRapLeg> tipoRapLegs()
        {
            List<GestioneAziendeWebOCM.TipoRapLeg> tipoRapLegList = new List<GestioneAziendeWebOCM.TipoRapLeg>();
            try
            {
                return this.aziendaDAL.rapLegs();
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)tipoRapLegList));
                return (List<GestioneAziendeWebOCM.TipoRapLeg>)null;
            }
        }

        public List<GestioneAziendeWebOCM.Via> GetVias()
        {
            List<GestioneAziendeWebOCM.Via> viaList = new List<GestioneAziendeWebOCM.Via>();
            try
            {
                return this.aziendaDAL.via();
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)viaList));
                return (List<GestioneAziendeWebOCM.Via>)null;
            }
        }

        public Dictionary<string, string> GetTipRap()
        {
            var tipRapDict = new Dictionary<string, string>();
            try
            {
                return this.aziendaDAL.GetTipRap();
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject(u), JsonConvert.SerializeObject(tipRapDict));
            }

            return tipRapDict;
        }

        public List<GestioneAziendeWebOCM.Comune> GetComunes()
        {
            List<GestioneAziendeWebOCM.Comune> comuneList = new List<GestioneAziendeWebOCM.Comune>();
            try
            {
                return this.aziendaDAL.comunes();
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)comuneList));
                return (List<GestioneAziendeWebOCM.Comune>)null;
            }
        }

        public List<GestioneAziendeWebOCM.Provincia> GetProvinces()
        {
            List<GestioneAziendeWebOCM.Provincia> provinvciaList = new List<GestioneAziendeWebOCM.Provincia>();
            try
            {
                return this.aziendaDAL.provincia();
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)provinvciaList));
                return (List<GestioneAziendeWebOCM.Provincia>)null;
            }
        }

        public List<GestioneAziendeWebOCM.Localita> GetLocalita()
        {
            List<GestioneAziendeWebOCM.Localita> localitaList = new List<GestioneAziendeWebOCM.Localita>();
            try
            {
                return this.aziendaDAL.GetLocalita();
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)localitaList));
                return (List<GestioneAziendeWebOCM.Localita>)null;
            }
        }

        public List<GestioneAziendeWebOCM.CategoriaAttivita> GetCategorias()
        {
            List<GestioneAziendeWebOCM.CategoriaAttivita> categoriaAttivitaList = new List<GestioneAziendeWebOCM.CategoriaAttivita>();
            try
            {
                return this.aziendaDAL.categoriaAttivitas();
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)categoriaAttivitaList));
                return (List<GestioneAziendeWebOCM.CategoriaAttivita>)null;
            }
        }

        public List<GestioneAziendeWebOCM.TipoAttivita> GetTipoAttivitas()
        {
            List<GestioneAziendeWebOCM.TipoAttivita> tipoAttivitaList = new List<GestioneAziendeWebOCM.TipoAttivita>();
            try
            {
                return this.aziendaDAL.tipoAttivitas();
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)tipoAttivitaList));
                return (List<GestioneAziendeWebOCM.TipoAttivita>)null;
            }
        }

        public List<GestioneAziendeWebOCM.CodiceStatistico> GetStatisticos()
        {
            List<GestioneAziendeWebOCM.CodiceStatistico> codiceStatisticoList = new List<GestioneAziendeWebOCM.CodiceStatistico>();
            try
            {
                return this.aziendaDAL.codiceStatisticos();
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)codiceStatisticoList));
                return (List<GestioneAziendeWebOCM.CodiceStatistico>)null;
            }
        }

        public GestioneAziendeWebOCM Update(
          GestioneAziendeWebOCM gestione,
          TFI.OCM.Utente.Utente u,
          ref string ErroreMSG,
          ref string SuccessMSG)
        {
            try
            {
                gestione = this.aziendaDAL.Update(gestione, u, ref ErroreMSG, ref SuccessMSG);
                return gestione;
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)u), JsonConvert.SerializeObject((object)gestione));
                return gestione;
            }
        }

        public IEnumerable<Genere> GetGeneri() => aziendaDAL.GetGeneri();
    }
}
