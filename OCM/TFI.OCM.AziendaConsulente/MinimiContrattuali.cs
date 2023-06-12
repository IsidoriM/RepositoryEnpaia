namespace TFI.OCM.AziendaConsulente
{
    public class MinimiContrattuali
    {
        public decimal Codice { get; set; }

        public decimal Progressivo { get; set; }

        public decimal CodLiv { get; set; }

        public string DatAppIni { get; set; }

        public string DatAppFin { get; set; }

        public decimal ImpVocRet { get; set; }

        public MinimiContrattuali(decimal codice, decimal progressivo, decimal codLiv, string datAppIni, string datAppFin, decimal impVocRet)
        {
            Codice = codice;
            Progressivo = progressivo;
            CodLiv = codLiv;
            DatAppIni = datAppIni;
            DatAppFin = datAppFin;
            ImpVocRet = impVocRet;
        }
    }
}
