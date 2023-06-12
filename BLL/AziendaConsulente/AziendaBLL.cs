// Decompiled with JetBrains decompiler
// Type: TFI.BLL.AziendaConsulente.AziendaBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Web;
using Newtonsoft.Json;
using OCM.TFI.OCM.AziendaConsulente;
using TFI.DAL.AziendaConsulente;
using TFI.DAL.Utilities;
using TFI.OCM.AziendaConsulente;

namespace TFI.BLL.AziendaConsulente
{
    public class AziendaBLL
    {
        private readonly AziendaDAL aziendaDAL = new AziendaDAL();
        private TFI.OCM.Utente.Utente u = HttpContext.Current.Session["utente"] as TFI.OCM.Utente.Utente;

        public Azienda.RapLeg GetRapLeg(string codPos)
        {
            Azienda.RapLeg rapLeg = new Azienda.RapLeg();
            try
            {
                return this.aziendaDAL.GetRapLeg(codPos);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)rapLeg));
                throw;
            }
        }

        public Azienda.PosAss GetPosAss(string codPos)
        {
            Azienda.PosAss posAss = new Azienda.PosAss();
            try
            {
                return this.aziendaDAL.GetPosAss(codPos);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)posAss));
                throw;
            }
        }

        public Azienda.Consulente GetConsulente(string codPos)
        {
            Azienda.Consulente consulente = new Azienda.Consulente();
            try
            {
                return this.aziendaDAL.GetConsulente(codPos);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)consulente));
                throw;
            }
        }

        public List<Azienda.StatoEstero> GetStatiEsteri()
        {
            List<Azienda.StatoEstero> statoEsteroList = new List<Azienda.StatoEstero>();
            return this.aziendaDAL.GetStatiEsteri();
        }

        public List<string> GetStatiEsteris()
        {
            List<string> stringList = new List<string>();
            return this.aziendaDAL.GetStatiEsteris();
        }

        public List<Azienda.Localita> GetLocalita()
        {
            List<Azienda.Localita> localitaList = new List<Azienda.Localita>();
            return this.aziendaDAL.GetLocalita();
        }

        public List<Azienda.Comune> GetComuni()
        {
            List<Azienda.Comune> comuneList = new List<Azienda.Comune>();
            return this.aziendaDAL.GetComuni();
        }

        public List<string> GetComunis()
        {
            List<string> stringList = new List<string>();
            return this.aziendaDAL.GetComunis();
        }

        public void UpdateAzienda(
          Azienda azienda,
          string codpos,
          ref string ErroreMSG,
          ref string SuccessMSG)
        {
            try
            {
                aziendaDAL.UpdateAzienda(azienda, u, codpos, ref ErroreMSG, ref SuccessMSG);
            }
            catch (Exception ex)
            {
                ErrorHandler.AggErrori(ex, JsonConvert.SerializeObject((object)this.u), JsonConvert.SerializeObject((object)azienda));
            }
        }


        public List<Azienda.Via> GetVias()
        {
            List<Azienda.Via> viaList = new List<Azienda.Via>();
            return this.aziendaDAL.GetVia();
        }

        //public List<Azienda.Sedi> GetListaSedi(string codPos)
        //{
        //  try
        //  {
        //    List<Azienda.Sedi> sediList = new List<Azienda.Sedi>();
        //    return this.aziendaDAL.GetListaSedi(codPos);
        //  }
        //  catch (Exception ex)
        //  {
        //    throw;
        //  }
        //}

        public Azienda.Sedi GetListaSedi(string codPos)
        {
            try
            {
                Azienda.Sedi sedi = new Azienda.Sedi();
                return this.aziendaDAL.GetListaSedi(codPos);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Azienda GetSedeInd(string id, string codpos) => this.aziendaDAL.TrovaSedeIndDAL(id, codpos);

        public List<Azienda.Sedi> ListaTipiSedi() => this.aziendaDAL.ListaTipiSedi();

        public IEnumerable<Genere> GetGeneri() => aziendaDAL.GetGeneri();
    }
}
