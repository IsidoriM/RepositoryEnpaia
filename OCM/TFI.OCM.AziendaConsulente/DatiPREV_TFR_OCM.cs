using System.Collections.Generic;

namespace TFI.OCM.AziendaConsulente
{
    public class DatiPREV_TFR_OCM
    {
        public class PrevdaConf
        {
            public string Matricola { get; set; }

            public string Cognome { get; set; }

            public string Nome { get; set; }

            public string DataIsc { get; set; }

            public string DataCess { get; set; }

            public string Causale { get; set; }

            public string Prorap { get; set; }

            public string Promod { get; set; }

            public string Strprot { get; set; }

            public string Datprot { get; set; }
            public string NomeFile { get; set; }
        }

        public class ProspLiqTfr
        {
            public string Posizione { get; set; }

            public string RagSoc { get; set; }

            public string CodFis { get; set; }

            public string ParIVA { get; set; }

            public string IndSede { get; set; }

            public string Comune { get; set; }

            public string Cap { get; set; }

            public string Prov { get; set; }
        }

        public class ListProspLiqTfr
        {
            public string anno { get; set; }

            public string numElenco { get; set; }

            public string matricola { get; set; }
            public string Cognome { get; set; }
            public string Nome { get; set; }

            public string path { get; set; }
        }

        public List<PrevdaConf> prevdaConfs { get; set; }

        public ProspLiqTfr prospLiqTfr { get; set; }

        public List<ListProspLiqTfr> listProspLiqTfrs { get; set; }

        public DatiPREV_TFR_OCM()
        {
            prevdaConfs = new List<PrevdaConf>();
            listProspLiqTfrs = new List<ListProspLiqTfr>();
            prospLiqTfr = new ProspLiqTfr();
        }
    }
}
