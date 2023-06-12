using System.Collections.Generic;

namespace TFI.OCM.Amministrativo
{
    public class DelegheOCM
    {
        public class ListInd
        {
            public string DENDUG { get; set; }

            public string CODDUG { get; set; }
        }

        public class AssNaz
        {
            public string CODNAZ { get; set; }

            public string RAGSOCASS { get; set; }

            public string RAGSOCBRASS { get; set; }
        }

        public class ListDen
        {
            public string CODNAZ { get; set; }

            public string RAGSOCASS { get; set; }

            public string RAGSOCBRASS { get; set; }

            public string CODTER { get; set; }
        }

        public class DatiDelega
        {
            public string Pin { get; set; }

            public string codpos { get; set; }

            public string AssStudio { get; set; }

            public string Denominazione { get; set; }

            public string codNaz { get; set; }

            public string codTer { get; set; }

            public string ViaDe { get; set; }

            public string CodUteDe { get; set; }

            public string CODDUG { get; set; }

            public string CODCOM { get; set; }

            public string PersonaRiferimento { get; set; }

            public string PersonaRiferimentoOld { get; set; }

            public string strModo { get; set; }

            public string RagSocDe { get; set; }

            public string RagSocBrDe { get; set; }

            public string CodFisDe { get; set; }

            public string ParIvaDe { get; set; }

            public string IndDe { get; set; }

            public string NumCivDe { get; set; }

            public string ComuneDe { get; set; }

            public string PrDe { get; set; }

            public string CapDe { get; set; }

            public string LocalitaDe { get; set; }

            public string TelDe { get; set; }

            public string CelDe { get; set; }

            public string FaxDe { get; set; }

            public string EmailDe { get; set; }

            public string EmailCertDe { get; set; }
        }

        public class DelegheNonAttive
        {
            public string datini { get; set; }

            public string datfin { get; set; }

            public string codter { get; set; }

            public string asster { get; set; }

            public string codnaz { get; set; }

            public string assnaz { get; set; }

            public string codpos { get; set; }

            public string ragsoc { get; set; }

            public string stato { get; set; }

            public string daAttivare { get; set; }
        }

        public class DelegheAttive
        {
            public string datini { get; set; }

            public string datfin { get; set; }

            public string codter { get; set; }

            public string asster { get; set; }

            public string codnaz { get; set; }

            public string assnaz { get; set; }

            public string codpos { get; set; }

            public string ragsoc { get; set; }

            public string stato { get; set; }

            public string attivo { get; set; }
        }

        public class DelegheCancellate
        {
            public string datini { get; set; }

            public string datfin { get; set; }

            public string codter { get; set; }

            public string asster { get; set; }

            public string codnaz { get; set; }

            public string assnaz { get; set; }

            public string codpos { get; set; }

            public string ragsoc { get; set; }

            public string stato { get; set; }

            public string cancellato { get; set; }
        }

        public List<DelegheAttive> delegheatt { get; set; }

        public List<AssNaz> AssociazioneNaz { get; set; }

        public List<ListDen> ListDenominazione { get; set; }

        public List<DelegheCancellate> deleghecanc { get; set; }

        public List<DelegheNonAttive> deleghenonatt { get; set; }

        public List<ListInd> ListaIndirizzi { get; set; }

        public DatiDelega datidelega { get; set; }

        public DelegheOCM()
        {
            ListaIndirizzi = new List<ListInd>();
            ListDenominazione = new List<ListDen>();
            AssociazioneNaz = new List<AssNaz>();
            datidelega = new DatiDelega();
            delegheatt = new List<DelegheAttive>();
            deleghecanc = new List<DelegheCancellate>();
            deleghenonatt = new List<DelegheNonAttive>();
        }
    }
}
