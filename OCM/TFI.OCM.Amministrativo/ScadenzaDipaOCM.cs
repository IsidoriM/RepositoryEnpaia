using System.Collections.Generic;

namespace TFI.OCM.Amministrativo
{
    public class ScadenzaDipaOCM
    {
        public class DatiScadenzeDipa
        {
            public string codparam { get; set; }

            public string datinival { get; set; }

            public string datfinval { get; set; }

            public string mese { get; set; }

            public string datsca { get; set; }

            public string datinival_1 { get; set; }

            public string datfinval_1 { get; set; }

            public string mese_1 { get; set; }

            public string datsca_1 { get; set; }

            public string datinival_2 { get; set; }

            public string datfinval_2 { get; set; }

            public string mese_2 { get; set; }

            public string datsca_2 { get; set; }

            public string datinival_3 { get; set; }

            public string datfinval_3 { get; set; }

            public string mese_3 { get; set; }

            public string datsca_3 { get; set; }

            public string datinival_4 { get; set; }

            public string datfinval_4 { get; set; }

            public string mese_4 { get; set; }

            public string datsca_4 { get; set; }

            public string datinival_5 { get; set; }

            public string datfinval_5 { get; set; }

            public string mese_5 { get; set; }

            public string datsca_5 { get; set; }

            public string datinival_6 { get; set; }

            public string datfinval_6 { get; set; }

            public string mese_6 { get; set; }

            public string datsca_6 { get; set; }

            public string datinival_7 { get; set; }

            public string datfinval_7 { get; set; }

            public string mese_7 { get; set; }

            public string datsca_7 { get; set; }

            public string datinival_8 { get; set; }

            public string datfinval_8 { get; set; }

            public string mese_8 { get; set; }

            public string datsca_8 { get; set; }

            public string datinival_9 { get; set; }

            public string datfinval_9 { get; set; }

            public string mese_9 { get; set; }

            public string datsca_9 { get; set; }

            public string datinival_10 { get; set; }

            public string datfinval_10 { get; set; }

            public string mese_10 { get; set; }

            public string datsca_10 { get; set; }

            public string datinival_11 { get; set; }

            public string datfinval_11 { get; set; }

            public string mese_11 { get; set; }

            public string datsca_11 { get; set; }

            public string datinival_12 { get; set; }

            public string datfinval_12 { get; set; }

            public string mese_12 { get; set; }

            public string datsca_12 { get; set; }
        }

        public class Anno
        {
            public string anno { get; set; }
        }

        public DatiScadenzeDipa datiScadenzeDipa { get; set; }

        public List<DatiScadenzeDipa> listScadenzaDipa { get; set; }

        public List<Anno> listanni { get; set; }

        public ScadenzaDipaOCM()
        {
            datiScadenzeDipa = new DatiScadenzeDipa();
            listanni = new List<Anno>();
            listScadenzaDipa = new List<DatiScadenzeDipa>();
        }
    }
}
