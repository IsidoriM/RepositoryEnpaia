using OCM.TFI.OCM.Protocollo;

namespace TFI.OCM.AziendaConsulente
{
    public class TotaliDenuncia
    {
        public decimal ImpDec { get; set; }

        public decimal ImpCon { get; set; }

        public decimal ImpAddRec { get; set; }

        public decimal ImpAbb { get; set; }

        public decimal ImpAssCon { get; set; }

        public decimal ImpSanDet { get; set; }

        public int CodModPag { get; set; }

        public decimal ImpVer { get; set; }

        public string DatVer { get; set; }

        public string UffPos { get; set; }

        public string CitDic { get; set; }

        public string ProDic { get; set; }

        public string Iban { get; set; }

        public string NumMov { get; set; }

        public string TipMov { get; set; }

        public string StaDen { get; set; }

        public string NumSanAnn { get; set; }

        public string SanSotSog { get; set; }

        public string Esiret { get; set; }

        public decimal ImpConDel { get; set; }

        public decimal ImpAddRecDel { get; set; }

        public decimal ImpAbbDel { get; set; }

        public decimal ImpAssConDel { get; set; }

        public decimal ImpSanRet { get; set; }

        public string NumSan { get; set; }

        public string CodLine { get; set; }

        public string ProtRic { get; set; }
        public Protocollo Protocollo { get; set; }
    }
}
