namespace TFI.OCM.Amministrativo
{
    public class CercaNotifiche : NotificheBase
    {
        public string RagSoc { get; set; }

        public string DenMese { get; set; }

        public decimal ImpDis { get; set; }

        public decimal ImpSanDet { get; set; }

        public decimal ImpCon { get; set; }

        public decimal ImpAddRec { get; set; }

        public decimal ImpAbb { get; set; }

        public string DatSca { get; set; }

        public string SanSotSog { get; set; }

        public decimal ImpAssCon { get; set; }

        public string CodCauSan { get; set; }

        public string CancellateDalDIPA { get; set; }

        public string Checked { get; set; }

        public int TotOccorrenze { get; set; }

        public string TipMov { get; set; }
    }
}
