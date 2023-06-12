using System.Collections.Generic;
using System.Web;
using OCM.TFI.OCM.Iscritto;

namespace TFI.OCM.Iscritto
{
    public class IscrittoRicTFROCM : ModelloFormRichiestePagamento

    {
    public class RichiestaLiquidazione
    {
        public int ID { get; set; }

        public int CODPOS { get; set; }

        public string DTRIC { get; set; }

        public decimal IMPTFR { get; set; }

        public int TIPLIQ { get; set; }

        public string RAGSOC { get; set; }

        public bool Liquidato { get; set; }

        public string DATFINRDL { get; set; }

        public bool DocumentoCaricato { get; set; }
    }

    public List<RichiestaLiquidazione> listTFR { get; set; }

    public RichiestaLiquidazione ricTFR { get; set; }

    public bool AbilitaBtn { get; set; }

    public IscrittoRicTFROCM()
    {
        ricTFR = new RichiestaLiquidazione();
    }
    }
}
