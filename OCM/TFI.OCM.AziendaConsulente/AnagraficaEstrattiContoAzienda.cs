using System.Collections.Generic;

namespace TFI.OCM.AziendaConsulente
{
    public class AnagraficaEstrattiContoAzienda
    {
        public List<EstrattiConto> estrattiContos { get; set; }

        public string Posizione { get; set; }

        public string RagioneSociale { get; set; }

        public string PartitaIva { get; set; }

        public string CodiceFis { get; set; }

        public string Indirizzo { get; set; }

        public string NumeroCivico { get; set; }

        public string CAP { get; set; }

        public string Provincia { get; set; }

        public string StaestRes { get; set; }

        public string Comune { get; set; }
    }
}
