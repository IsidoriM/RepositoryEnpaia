using System;
using System.Collections.Generic;

namespace TFI.OCM.AziendaConsulente
{
    public class CessazioniRdl
    {
        public class Listas
        {
            public string Matricola2 { get; set; }

            public string Nome { get; set; }

            public string Cognome { get; set; }

            public DateTime DataIscrizione { get; set; }

            public DateTime DataCessazione { get; set; }

            public string Causale1 { get; set; }

            public string NomeFile { get; set; }
            public string Iban { get; set; }
        }

        public class Causas
        {
            public string Causale2 { get; set; }
        }

        public class Matricola
        {
            public string Matricola1 { get; set; }

            public string ProRap { get; set; }
        }

        public List<Listas> listas { get; set; }

        public List<Causas> causas { get; set; }

        public List<Matricola> matricolas { get; set; }

        public CessazioniRdl()
        {
            listas = new List<Listas>();
            causas = new List<Causas>();
            matricolas = new List<Matricola>();
        }
    }
}
