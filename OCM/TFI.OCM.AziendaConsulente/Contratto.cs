namespace TFI.OCM.AziendaConsulente
{
    public class Contratto
    {
        public decimal Codice { get; set; }

        public decimal Progressivo { get; set; }

        public string DataInizio { get; set; }

        public string DataFine { get; set; }

        public string DatDec { get; set; }

        public string TipSpe { get; set; }

        public Contratto(decimal codice, decimal progressivo, string dataInizio, string dataFine, string datDec, string tipSpe)
        {
            Codice = codice;
            Progressivo = progressivo;
            DataInizio = dataInizio;
            DataFine = dataFine;
            DatDec = datDec;
            TipSpe = tipSpe;
        }
    }
}
