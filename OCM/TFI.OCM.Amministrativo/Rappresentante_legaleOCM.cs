using System.Collections.Generic;

namespace TFI.OCM.Amministrativo
{
    public class Rappresentante_legaleOCM
    {
        public class RicercaRapp
        {
            public string CodPos { get; set; }

            public string RagSoc { get; set; }

            public string PartIVAaz { get; set; }

            public string CodFisAz { get; set; }

            public string cog { get; set; }

            public string nom { get; set; }

            public string codfis { get; set; }
        }

        public class RappLegale
        {
            public string codpos { get; set; }

            public string prorec { get; set; }

            public string emailcert { get; set; }

            public string datini { get; set; }

            public string datfin { get; set; }

            public string codfunrap { get; set; }

            public string rappri { get; set; }

            public string ragsoc { get; set; }

            public string parivaAZ { get; set; }

            public string codfisAz { get; set; }

            public string dendug { get; set; }

            public string dencomnas { get; set; }

            public string dencom { get; set; }

            public string denmez { get; set; }

            public string denfunrap { get; set; }

            public string cog { get; set; }

            public string nom { get; set; }

            public string coddug { get; set; }

            public string ind { get; set; }

            public string numciv { get; set; }

            public string denstaest { get; set; }

            public string denloc { get; set; }

            public string cap { get; set; }

            public string sigpro { get; set; }

            public string tel1 { get; set; }

            public string tel2 { get; set; }

            public string fax { get; set; }

            public string cell { get; set; }

            public string email { get; set; }

            public string codfis { get; set; }

            public string datnas { get; set; }

            public string codcomnas { get; set; }

            public string sesso { get; set; }

            public string codmez { get; set; }

            public string datcom { get; set; }

            public string codcomres { get; set; }

            public string datconf { get; set; }
        }

        public class DettRap
        {
            public string codpos { get; set; }

            public string prorec { get; set; }

            public string emailcert { get; set; }

            public string datini { get; set; }

            public string datfin { get; set; }

            public string codfunrap { get; set; }

            public string rappri { get; set; }

            public string ragsoc { get; set; }

            public string parivaAZ { get; set; }

            public string codfisAz { get; set; }

            public string dendug { get; set; }

            public string dencomnas { get; set; }

            public string dencom { get; set; }

            public string denmez { get; set; }

            public string denfunrap { get; set; }

            public string cog { get; set; }

            public string nom { get; set; }

            public string coddug { get; set; }

            public string ind { get; set; }

            public string numciv { get; set; }

            public string denstaest { get; set; }

            public string denloc { get; set; }

            public string cap { get; set; }

            public string sigpro { get; set; }

            public string tel1 { get; set; }

            public string tel2 { get; set; }

            public string fax { get; set; }

            public string cell { get; set; }

            public string email { get; set; }

            public string codfis { get; set; }

            public string datnas { get; set; }

            public string codcomnas { get; set; }

            public string sigpronas { get; set; }

            public string sesso { get; set; }

            public string codmez { get; set; }

            public string datcom { get; set; }

            public string codcomres { get; set; }

            public string datconf { get; set; }
        }

        public class Dendug
        {
            public string dendug { get; set; }

            public string coddug { get; set; }
        }

        public class FunRap
        {
            public string denfunrap { get; set; }

            public string codfunrap { get; set; }
        }

        public RicercaRapp ricercaRapp { get; set; }

        public List<RappLegale> rapplegale { get; set; }

        public DettRap dettRap { get; set; }

        public List<Dendug> listNIVia { get; set; }

        public List<FunRap> listfunrap { get; set; }

        public Rappresentante_legaleOCM()
        {
            ricercaRapp = new RicercaRapp();
            rapplegale = new List<RappLegale>();
            dettRap = new DettRap();
            listNIVia = new List<Dendug>();
            listfunrap = new List<FunRap>();
        }
    }
}
