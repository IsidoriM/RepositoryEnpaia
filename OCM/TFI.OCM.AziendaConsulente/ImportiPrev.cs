namespace TFI.OCM.AziendaConsulente
{
    public class ImportiPrev
    {
        public int ProMod { get; set; }

        public int CodStaPre { get; set; }

        public decimal ImpRet { get; set; }

        public decimal ImpOcc { get; set; }

        public decimal ImpFig { get; set; }

        public decimal ImpCon { get; set; }

        public decimal ImpRetPrv { get; set; }

        public decimal ImpOccPrv { get; set; }

        public decimal ImpFigPrv { get; set; }

        public decimal ImpConPrv { get; set; }

        public string TipMov { get; set; }

        public ImportiPrev(int proMod, int codStaPre, decimal impRet, decimal impOcc, decimal impFig, decimal impCon, decimal impRetPrv, decimal impOccPrv, decimal impFigPrv, decimal impConPrv, string tipMov)
        {
            ProMod = proMod;
            CodStaPre = codStaPre;
            ImpRet = impRet;
            ImpOcc = impOcc;
            ImpFig = impFig;
            ImpCon = impCon;
            ImpRetPrv = impRetPrv;
            ImpOccPrv = impOccPrv;
            ImpFigPrv = impFigPrv;
            ImpConPrv = impConPrv;
            TipMov = tipMov;
        }
    }
}
