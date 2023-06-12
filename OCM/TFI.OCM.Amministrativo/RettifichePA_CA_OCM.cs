using System.Collections.Generic;

namespace TFI.OCM.Amministrativo
{
    public class RettifichePA_CA_OCM
    {
        public class Search_Rett
        {
            public string Codpos { get; set; }

            public string RagSoc { get; set; }

            public string AnnoDA { get; set; }

            public string MeseDA { get; set; }

            public string AnnoA { get; set; }

            public string MeseA { get; set; }

            public string Occorrenze { get; set; }
        }

        public class List_Rett
        {
            public string PA { get; set; }

            public string AC { get; set; }

            public string Codfis { get; set; }

            public string Mat { get; set; }

            public string Cog { get; set; }

            public string Nom { get; set; }

            public string DatDec { get; set; }

            public string DatCes { get; set; }

            public string Codpos { get; set; }

            public string prorap { get; set; }

            public bool PAbool { get; set; }

            public bool ACbool { get; set; }
        }

        public Search_Rett search_rett { get; set; }

        public List<List_Rett> list_rett { get; set; }

        public RettifichePA_CA_OCM()
        {
            list_rett = new List<List_Rett>();
            search_rett = new Search_Rett();
        }
    }
}
