namespace TFI.OCM.AziendaConsulente
{
    public class FigurativaInfortuni
    {
        public int CodPos { get; set; }

        public int Mat { get; set; }

        public int ProRap { get; set; }

        public decimal ImpFig { get; set; }

        public FigurativaInfortuni(int codPos, int mat, int proRap, decimal impFig)
        {
            CodPos = codPos;
            Mat = mat;
            ProRap = proRap;
            ImpFig = impFig;
        }
    }
}
