using System.Collections.Generic;

namespace TFI.OCM.Amministrativo
{
    public class TrasferimentiRdlOCM
    {
        public class DatiTrasferimento
        {
            public string CodPos { get; set; }

            public string CodPosNew { get; set; }

            public string CodPosTra { get; set; }

            public string Matricola { get; set; }

            public string MatTra { get; set; }

            public string ProSos { get; set; }

            public string ProRap { get; set; }

            public string ProRapTra { get; set; }

            public string ProTraRap { get; set; }

            public string DatDec { get; set; }

            public string Stato { get; set; }

            public string Nome { get; set; }

            public string Cognome { get; set; }

            public string DatNas { get; set; }

            public string CodFis { get; set; }

            public string DatTraDal { get; set; }

            public string DatTraAl { get; set; }

            public string DatTra { get; set; }

            public string DatAnn { get; set; }

            public string Gruppo { get; set; }

            public bool Seleziona { get; set; }

            public string DatDen { get; set; }

            public string Percen { get; set; }

            public string DatCess { get; set; }

            public string RagSoc { get; set; }

            public string DatIsc { get; set; }

            public string RagSocNew { get; set; }

            public string RagSocTra { get; set; }
        }

        public class ListaAziende
        {
            public string codpos { get; set; }

            public string datden { get; set; }

            public string Percen { get; set; }
        }

        public List<DatiTrasferimento> datiTrasferimentoList = new List<DatiTrasferimento>();

        public DatiTrasferimento datitrasferimento { get; set; }

        public List<DatiTrasferimento> ListaIscrittis { get; set; }

        public ListaAziende lista { get; set; }

        public List<ListaAziende> ListaAziendes { get; set; }

        public TrasferimentiRdlOCM()
        {
            lista = new ListaAziende();
            datitrasferimento = new DatiTrasferimento();
            datiTrasferimentoList = new List<DatiTrasferimento>();
        }
    }
}
