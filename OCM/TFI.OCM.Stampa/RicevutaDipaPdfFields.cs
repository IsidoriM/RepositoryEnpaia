using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace OCM.TFI.OCM.Stampa
{
    public class RicevutaDipaPdfFields
    {
        public string ProtocolloField { get; set; }
        public string NumeroProtocolloField { get; set; }
        public string DataProtocolloField { get; set; }
        public string PrRicevutaField { get; set; }
        public string PeriodoDipaField { get; set; }
        public string DataDipaRicevutaField { get; set; }
        public string OraDipaRicevutaField { get; set; }
        public string NominativoDipaRicevutaField { get; set; }
        public string NumeroProgressivoRicevutaField { get; set; }
        public string StatoRicevutaField { get; set; }
        public string TotaleDovutoRicevutaField { get; set; }
        public string TotaleDaPagareRicevutaField { get; set; }
    }
}
