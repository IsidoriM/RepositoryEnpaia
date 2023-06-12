using System.Collections.Generic;

namespace TFI.OCM.Amministrativo
{
    public class Rettifiche_OCM
    {
        public class SearchRett
        {
            public string Codpos { get; set; }

            public string RagSoc { get; set; }

            public string RagSoc2 { get; set; }

            public string Mat { get; set; }

            public string Nominativo { get; set; }

            public string Nominativo2 { get; set; }

            public string AnnoDA { get; set; }

            public string MeseDA { get; set; }

            public string AnnoA { get; set; }

            public string MeseA { get; set; }

            public string Occorrenze { get; set; }
        }

        public class ListRett
        {
            public string AnnoDen { get; set; }

            public string MeseDen { get; set; }

            public string AnnoComp { get; set; }

            public string Mat { get; set; }

            public string Retribuzione { get; set; }

            public string Occ { get; set; }

            public string Fig { get; set; }

            public string DatIniSanz { get; set; }

            public string DatFinSanz { get; set; }

            public string NumGGRit { get; set; }

            public string TassoSanz { get; set; }

            public string Dal { get; set; }

            public string Al { get; set; }

            public string TipMov { get; set; }

            public string CodContr { get; set; }

            public string CodLiv { get; set; }

            public string CodQua { get; set; }

            public string FAP { get; set; }

            public string Contributo { get; set; }

            public string Aliq { get; set; }

            public string OccPre { get; set; }

            public string FigPre { get; set; }

            public string RetPre { get; set; }

            public string Sanz { get; set; }

            public string OccDiff { get; set; }

            public string FigDiff { get; set; }

            public string RetDiff { get; set; }

            public string ContributoPre { get; set; }

            public string ContributoDiff { get; set; }

            public string SanzPre { get; set; }

            public string AbbPre { get; set; }

            public string AssConPre { get; set; }

            public string Abb { get; set; }

            public string AssCon { get; set; }

            public string Codpos { get; set; }

            public string Proden { get; set; }

            public string Prodendet { get; set; }

            public string Prorap { get; set; }

            public string Procon { get; set; }

            public string Codgruass { get; set; }

            public string CodquaCon { get; set; }

            public string Perfap { get; set; }

            public string Impsca { get; set; }

            public string Sos { get; set; }

            public string Min { get; set; }

            public string TipDen { get; set; }

            public string ImpRetDel { get; set; }

            public string ImpOccDel { get; set; }

            public string ImpFigDel { get; set; }

            public string ImpConDel { get; set; }

            public string ImpAbbDel { get; set; }

            public string ImpAssConDel { get; set; }

            public string ImpSanDet { get; set; }

            public string NumMov { get; set; }

            public string Prev { get; set; }

            public string CodLoc { get; set; }

            public string ProLoc { get; set; }

            public string Eta65 { get; set; }

            public string DatNas { get; set; }

            public string DatDec { get; set; }

            public string DatCes { get; set; }

            public string NumGgAzi { get; set; }

            public string NumGgFig { get; set; }

            public string NumGgPer { get; set; }

            public string NumGgDom { get; set; }

            public string NumGgSos { get; set; }

            public string NumGgConAzi { get; set; }

            public string ImpTraEco { get; set; }

            public string TipRap { get; set; }

            public string PerPar { get; set; }

            public string PerApp { get; set; }

            public string TipSpe { get; set; }

            public string TipMovDenTes { get; set; }

            public string ImpCon { get; set; }
        }

        public class ModRett
        {
            public string Retr { get; set; }

            public string Occ { get; set; }

            public string Fig { get; set; }

            public string DatIniSan { get; set; }

            public string DatFinSan { get; set; }
        }

        public SearchRett searchRett { get; set; }

        public ListRett Rett { get; set; }

        public List<ListRett> listRett { get; set; }

        public ModRett modRett { get; set; }

        public Rettifiche_OCM()
        {
            searchRett = new SearchRett();
            Rett = new ListRett();
            listRett = new List<ListRett>();
            modRett = new ModRett();
        }
    }
}
