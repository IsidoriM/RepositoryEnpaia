using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TFI.OCM.AziendaConsulente
{
    public class Azienda
    {
        public class RapLeg
        {
            public string NomeRP { get; set; }

            public string CognomeRP { get; set; }

            public string SessoRP { get; set; }

            public string DataNascitaRP { get; set; }

            public string CodiceComuneNascitaRP { get; set; }

            public string ComuneNascitaRP { get; set; }

            public string ProvinciaNascitaRP { get; set; }

            public string ProvinciaRP { get; set; }

            public string CodFiscaleRP { get; set; }

            public string FunzioneRP { get; set; }

            public string DecorrenzaRP { get; set; }

            public string IndirizzoRP { get; set; }

            public string CivicoRP { get; set; }

            public string ComuneRP { get; set; }

            public string CapRP { get; set; }

            public string LocalitaRP { get; set; }

            public string StatoEsteroRP { get; set; }

            public string TipoIndirizzoRP { get; set; }

            public string CodiceComuneResidenzaRP { get; set; }

            public string CodiceLocalitaResidenzaRP { get; set; }

            public int CODDUGRP { get; set; }

            public string CODFUNRAPRP { get; set; }
        }

        public class PosAss
        {
            public string INSAZIPA { get; set; }

            public string CODPOSPA { get; set; }

            public string RagioneSocialePA { get; set; }

            public string CodiceFiscalePA { get; set; }

            public string PartitaIVAPA { get; set; }

            public string RSBrevePA { get; set; }

            public string DataAperturaPA { get; set; }

            public string NaturaGiuridicaPA { get; set; }

            public string CodiceAttivitaAziPA { get; set; }

            public string CodiceStatisticoAziPA { get; set; }

            public string TipoIndirizzoPA { get; set; }

            public int CODDUGPA { get; set; }

            public string DENDUGPA { get; set; }

            public string NumeroCivicoPA { get; set; }

            public string CODCOMPA { get; set; }

            public string ComunePA { get; set; }

            public string CAPPA { get; set; }

            public string ProvinciaPA { get; set; }

            public string LocalitaPA { get; set; }

            public string StatoEsteroPA { get; set; }

            public string Telefono1PA { get; set; }

            public string Telefono2PA { get; set; }

            public string FaxPA { get; set; }

            [Required(ErrorMessage = "Il campo non può essere vuoto")]
            [EmailAddress(ErrorMessage = "E-Mail non valida")]
            public string EmailPA { get; set; }

            public string EmailCertificataPA { get; set; }

            public string IndirizzoPA { get; set; }

            public string DENINDPA { get; set; }

            public string AZINOTCPA { get; set; }

            public string NATGIUPA { get; set; }

            public string DATINIPA { get; set; }

            public string ULTAGGPA { get; set; }

            public string DataChiusuraPA { get; set; }

            public string TIPISCPA { get; set; }

            public string CATATTCAMPA { get; set; }

            public string CODATTCAMPA { get; set; }

            public string DataInizioAttivitaPA { get; set; }

            public string CODSTACONPA { get; set; }

            public string AGGMANPA { get; set; }
        }

        public class Consulente
        {
            public string RagioneSocialeC { get; set; }

            public string IndirizzoC { get; set; }

            public string DENDUGC { get; set; }

            public string NumeroCivicoC { get; set; }

            public string LocalitaC { get; set; }

            public string ComuneC { get; set; }

            public string CAPC { get; set; }

            public string Telefono1C { get; set; }

            public string CellulareC { get; set; }

            public string ProvinciaC { get; set; }

            public string CODCOMC { get; set; }

            public string FaxC { get; set; }

            public string EmailC { get; set; }

            public string EmailCertificataC { get; set; }

            public string CODTERC { get; set; }

            public int CODDUGC { get; set; }

            public string Telefono2C { get; set; }
        }

        public class Sedi
        {
            public int IdS { get; set; }

            public string IdS2 { get; set; }

            public string TipoIndirizzoS { get; set; }

            public string IndirizzoS { get; set; }

            public string ViaS { get; set; }

            public string CivicoS { get; set; }

            public string ComuneS { get; set; }

            public string CAPS { get; set; }

            public string ProvinciaS { get; set; }

            public string LocalitaS { get; set; }

            public string StatoEsteroS { get; set; }

            public string Telefono1S { get; set; }

            public string Telefono2S { get; set; }

            public string FaxS { get; set; }

            public string EmailS { get; set; }

            public string EmailCertificataS { get; set; }

            public int CODDUGS { get; set; }

            public string CODCOMS { get; set; }

            public string DATINIS { get; set; }

            public string AGGMANS { get; set; }

            public bool UpdateSediS { get; set; }

            public int appoggioS { get; set; }
        }

        public class Via
        {
            public int CODDUGV { get; set; }

            public string DENDUGV { get; set; }
        }

        public class Comune
        {
            public string CODCOMCo { get; set; }

            public string DENCOMCo { get; set; }

            public string SIGPROCo { get; set; }
        }

        public class Localita
        {
            public int CODLOCL { get; set; }

            public string CAPL { get; set; }

            public string DENLOCL { get; set; }

            public string SIGPROL { get; set; }
        }

        public class StatoEstero
        {
            public string CODCOMSE { get; set; }

            public string DENCOMSE { get; set; }
        }

        public List<Sedi> sedis = new List<Sedi>();

        public List<Via> vias = new List<Via>();

        public List<Comune> comunes = new List<Comune>();

        public List<Localita> localitas = new List<Localita>();

        public List<StatoEstero> statoEsteros = new List<StatoEstero>();

        public List<Sedi> TypeSedi = new List<Sedi>();

        public string utenzaConsulente { get; set; }

        public int codicePosizione { get; set; }

        public string codiceFiscale { get; set; }

        public string partitaIVA { get; set; }

        public RapLeg rapLeg { get; set; }

        public PosAss posAss { get; set; }

        public Consulente consulente { get; set; }

        public Sedi sedi { get; set; }

        public Via via { get; set; }

        public Comune comune { get; set; }

        public Localita localita { get; set; }

        public StatoEstero statoEstero { get; set; }
    }
}
