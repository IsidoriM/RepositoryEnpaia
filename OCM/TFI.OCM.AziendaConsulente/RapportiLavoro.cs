using System.Collections.Generic;

namespace TFI.OCM.AziendaConsulente
{
    public class RapportiLavoro
    {
        public class DatiRapporto
        {
            public string qualifica { get; set; }

            public string mensilita { get; set; }

            public string m14 { get; set; }

            public string m15 { get; set; }

            public string m16 { get; set; }

            public string tipspe { get; set; }

            public string codloc { get; set; }
        }

        public Sospensioni sosp { get; set; }

        public Carriera carr { get; set; }

        public DettagliCarriera detCar { get; set; }

        public NuovoIscritto modIsc { get; set; }

        public List<RapportiCessati> rapportiCes { get; set; }

        public List<RapportiAttivi> rapportiAtt { get; set; }

        public List<Sospensioni> sospensioni { get; set; }

        public List<Carriera> carriera { get; set; }

        public List<DettagliCarriera> dettagliCarriera { get; set; }

        public List<NuovoIscritto> modificaIscritto { get; set; }

        public List<NuovoIscritto> modificaIscritto2 { get; set; }

        public List<NuovoIscritto> modificaIscrittoliv { get; set; }

        public List<ListSosp> listSosps { get; set; }

        public List<ListContratti> listContratti { get; set; }

        public List<NuovoIscritto> listNuovoIscritto { get; set; }

        public List<NuovoIscritto> listNuovoIscritto2 { get; set; }

        public List<NuovoIscritto> listNIVia { get; set; }

        public List<NuovoIscritto> listNIStudio { get; set; }

        public List<Contratti> listCon { get; set; }

        public List<Livello> listLiv { get; set; }

        public List<Aliquota> aliq { get; set; }

        public DatiRapporto datRap { get; set; }

        public string posizione { get; set; }

        public string ragioneSociale { get; set; }

        public string codiceFis { get; set; }

        public string partitaIva { get; set; }

        public string indirizzo { get; set; }

        public string comune { get; set; }

        public string cap { get; set; }

        public string provincia { get; set; }

        public string aliquota { get; set; }

        public RapportiLavoro()
        {
            sosp = new Sospensioni();
            carr = new Carriera();
            detCar = new DettagliCarriera();
            modIsc = new NuovoIscritto();
            datRap = new DatiRapporto();
        }
    }
}
