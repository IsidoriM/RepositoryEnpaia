using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCM.TFI.OCM.Iscritto
{
    public class Pratica
    {
        public string Id { get; set; }
        public string Posizione { get; set; }
        public string Ragione_Sociale { get; set; }
        public string Nominativo { get; set; }
        public string Data_Inizio_RDL { get; set; }
        public string Data_Fine_RDL { get; set; }
        public string Tipo_Pratica { get; set; }
        public string Stato_Pratica { get; set; }
    }
}
