namespace TFI.OCM.AziendaConsulente
{
    public class PagamentoAzienda
    {
        public int IdTipPag { get; set; }

        public string DisPag { get; set; }

        public PagamentoAzienda(int idTipPag, string disPag)
        {
            IdTipPag = idTipPag;
            DisPag = disPag;
        }
    }
}
