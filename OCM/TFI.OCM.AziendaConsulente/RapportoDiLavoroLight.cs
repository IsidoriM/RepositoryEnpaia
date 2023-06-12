using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCM.TFI.OCM.AziendaConsulente
{
    public class RapportoDiLavoroLight
    {
        public TipoRDL TipoRDL { get; set; }
        public DateTime? DataCessazione { get; set; }
        public string Matricola { get; set; }

        public string Cognome { get; set; }

        public string Nome { get; set; }

        public string CodFis { get; set; }

        public string Iscrizione { get; set; }

        public string Prorap { get; set; }
    }
}
