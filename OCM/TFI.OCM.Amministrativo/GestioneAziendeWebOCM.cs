using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ExpressiveAnnotations.Attributes;
using OCM.TFI.OCM.Utilities;
using OCM.TFI.OCM.Utilities.CustomValidators;

namespace TFI.OCM.Amministrativo
{
    public class GestioneAziendeWebOCM
    {
        public class DatiAzienda
        {
            public string CodiceAziendaWeb { get; set; }
            [DisplayName("Ragione Sociale:")]
            [Required(ErrorMessage = "Ragione sociale obbligatoria")]
            public string RagioneSociale { get; set; }
            public string RagioneSocialeBreve { get; set; }
            [DisplayName("Partita IVA:")]            
            [RequiredIf($"{nameof(CodiceFiscale)} == null",ErrorMessage = "Partita iva obbligatoria")]
            [MaxLength(11, ErrorMessage = "Partita iva troppo lunga")]
            public string PartitaIva { get; set; }
            [DisplayName("Codice Fiscale:")]
            [RequiredIf($"{nameof(PartitaIva)} == null", ErrorMessage = "Codice fiscale obbligatorio")]
            [CodiceFiscale(ErrorMessage = "Codice fiscale non valido")]
            public string CodiceFiscale { get; set; }
            [DisplayName("Data Apertura:")]
            public string DataApertura { get; set; }
            [DisplayName("Data Denuncia Apertura:")]
            public string DataDenunciaApertura { get; set; }
            public string URL { get; set; }
            public string MezziComunicazione { get; set; }
            public string NaturaGiuridica { get; set; }
            [DisplayName("Natura Giuridica:")]
            [Required(ErrorMessage = "Natura giuridica obbligatoria")]
            public string SelectedNaturaGiuridica { get; set; }
            [DisplayName("Note:")]
            public string Note { get; set; }

            public string DataRegistrazione { get; set; }

            public string CodiceNaturaGiuridica { get; set; }

            public string DataConferma { get; set; }

            public string CodiceMezzoComunicazione { get; set; }

            public string CodicePosizioneAzienda { get; set; }

            public string DataComunicazione { get; set; }
            
            [DisplayName("Email:")]
            [Required(ErrorMessage = "Email obbligatoria"), EmailAddress(ErrorMessage = "Email non valida")]
            [MaxLength(100, ErrorMessage = "Email troppo lunga")]
            public string Email { get; set; }
            
            [DisplayName("PEC:")]
            [Required(ErrorMessage = "Pec obbligatoria"), EmailAddress(ErrorMessage = "Pec non valida")]
            [RegularExpression(RegexRules.PecRegex, ErrorMessage = "Formato pec non valido")]
            [MaxLength(100, ErrorMessage = "Pec troppo lunga")]
            public string Pec { get; set; }
        }

        public class IndirizzoModel
        {
            protected IndirizzoModel()
            {
                Comuni = new List<SelectListItem>();
                LocalitaSelezionabili = new List<SelectListItem>();
            }
            public IEnumerable<SelectListItem> Comuni { get; set; }
            public IEnumerable<SelectListItem> LocalitaSelezionabili { get; set; }
            [DisplayName("DUG:")]
            [Required(ErrorMessage = "Dug obbligatorio")]
            public int SelectedTipoIndirizzo { get; set; }
            [DisplayName("Indirizzo:")]
            [Required(ErrorMessage = "Indirizzo obbligatorio")]
            [MaxLength(80, ErrorMessage = "Indirizzo troppo lungo")]
            public string Indirizzo { get; set; }
            [DisplayName("Civico:")]
            [RegularExpression(RegexRules.CivicoRegex, ErrorMessage = "Civico non valido")]
            [MaxLength(20, ErrorMessage = "Civico troppo lungo")]
            [Required(ErrorMessage = "Civico obbligatorio")]
            public string Civico { get; set; }
            public string Comune { get; set; }
            [DisplayName("Comune:")]
            [RequiredIf($"{nameof(SelectedStatoEstero)} == null && {nameof(SelectedProvincia)} != null", ErrorMessage = "Comune obbligatorio")]
            public string SelectedComune { get; set; }

            [DisplayName("Provincia:")]
            [RequiredIf($"{nameof(SelectedStatoEstero)} == null", ErrorMessage = "Provincia obbligatoria")]
            public string SelectedProvincia { get; set; }
            public string Provincia { get; set; }
            [DisplayName("CAP:")]
            public string CAP { get; set; }
            public string Localita { get; set; }
            [DisplayName("Localita:")]
            [RequiredIf($"{nameof(SelectedStatoEstero)} == null && {nameof(SelectedComune)} != null", ErrorMessage = "Località obbligatoria")]
            public string SelectedLocalita { get; set; }

            [DisplayName("Stato Estero:")] 
            [RequiredIf($"{nameof(SelectedProvincia)} == null", ErrorMessage = "Stato estero obbligatorio")]
            public string SelectedStatoEstero { get; set; }
        }

        public class RapLeg: IndirizzoModel
        {
            public RapLeg()
            {
                ComuniDiNascita = new List<SelectListItem>();
            }
            public string PROREC { get; set; }

            [DisplayName("Cellulare:")]
            [RegularExpression(RegexRules.CellulareRegex, ErrorMessage = "Cellulare non valido")]
            [MaxLength(20, ErrorMessage = "Cellulare troppo lungo")]
            [RequiredIf($"{nameof(Telefono1)} == null", ErrorMessage = "Fornire almeno un telefono o un cellulare")]
            public string Cell { get; set; }

            [DisplayName("Data decorrenza incarico:"), Required(ErrorMessage = "Data decorrenza incarico obbligatoria")]
            public DateTime DataDecIncDateTime { get; set; }
            
            [DisplayName("Incarico:")]
            [Required(ErrorMessage = "Incarico obbligatorio")]
            public int SelectedIncarico { get; set; }
            public string DataDecInc { get; set; }

            public string RapLegPrinc { get; set; }

            public string TipoVia { get; set; }
            
            [DisplayName("Provincia di nascita:")]
            [RequiredIf($"{nameof(SelectedStatoEsteroDiNascita)} == null", ErrorMessage = "Provincia di nascita obbligatoria")]
            public string SelectedProvinciaDiNascita { get; set; }
            public IEnumerable<SelectListItem> ComuniDiNascita { get; set; }

            [DisplayName("Comune di nascita:")]
            [RequiredIf($"{nameof(SelectedStatoEsteroDiNascita)} == null && {nameof(SelectedProvinciaDiNascita)} != null", ErrorMessage = "Comune di nascita obbligatorio")]
            public string SelectedComuneDiNascita { get; set; }
            public string ComuneNascita { get; set; }

            public string Provincia { get; set; }

            public string TipoRapp { get; set; }

            [DisplayName("Nome:"), Required(ErrorMessage = "Nome obbligatorio")]
            [MaxLength(40, ErrorMessage = "Nome troppo lungo")]
            public string Nome { get; set; }

            [DisplayName("Cognome:"), Required(ErrorMessage = "Cognome obbligatorio")]
            [MaxLength(80, ErrorMessage = "Cognome troppo lungo")]
            public string Cognome { get; set; }

            public string CODDUG { get; set; }
            public string StatoEstero { get; set; }
            
            [DisplayName("Stato Estero di nascita:")]
            [RequiredIf($"{nameof(SelectedProvinciaDiNascita)} == null", ErrorMessage = "Stato estero di nascita obbligatorio")]

            public string SelectedStatoEsteroDiNascita { get; set; }

            
            public string ProvinciaNas { get; set; }
            
            [DisplayName("Telefono:")]
            [RegularExpression(RegexRules.TelefonoRegex, ErrorMessage = "Numero di telefono non valido")]
            [MaxLength(13, ErrorMessage = "Telefono troppo lungo")]
            [RequiredIf($"{nameof(Cell)} == null", ErrorMessage = "Fornire almeno un telefono o un cellulare")]

            public string Telefono1 { get; set; }

            public string Telefono2 { get; set; }

            public string Fax { get; set; }

            [DisplayName("Codice fiscale:"), Required(ErrorMessage = "Codice fiscale obbligatorio")]
            [CodiceFiscale("Nome", "Cognome", ErrorMessage = "Codice fiscale non valido")]
            public string CodiceFiscale { get; set; }

            [DisplayName("Email:"), Required(ErrorMessage = "Email obbligatoria"), EmailAddress(ErrorMessage = "Email non valida")]
            [MaxLength(100, ErrorMessage = "Email troppo lunga")]
            public string Email { get; set; }
            
            [DisplayName("PEC:"), Required(ErrorMessage = "Pec obbligatoria"), EmailAddress(ErrorMessage = "Pec non valida")]
            [RegularExpression(RegexRules.PecRegex, ErrorMessage = "Formato pec non valido")]
            [MaxLength(100, ErrorMessage = "Pec troppo lunga")]
            public string EmailCert { get; set; }

            [DisplayName("Data di nascita:"), Required(ErrorMessage = "Data di nascita obbligatoria")]
            public DateTime? DataNascitaDateTime { get; set; }
            public string DataNascita { get; set; }

            public string CodiceComuneNascita { get; set; }

            public string Sesso { get; set; }

            [DisplayName("Genere:"), Required(ErrorMessage = "Genere obbligatorio")]
            public int? SelectedGenere { get; set; }

            public string StatoEsteroNascita { get; set; }

            public string SigPro { get; set; }

            public string CodiceComuneResidenza { get; set; }

            public string CodiceLocalita { get; set; }
        }

        public class AltriDati
        {
            public AltriDati()
            {
                TipiAttivia = new List<SelectListItem>();
            }
            public string DataDecorrenza { get; set; }

            public string CategoriaAttivita { get; set; }

            [DisplayName("Categoria attivita:"), Required(ErrorMessage = "Categoria attivita obbligatoria")]
            public string SelectedCategorieAttivita { get; set; }

            public IEnumerable<SelectListItem> TipiAttivia { get; set; }
            [DisplayName("Tipo attivita:"), Required(ErrorMessage = "Tipo attivita obbligatoria")]
            public string SelectedTipoAttivita { get; set; }

            [DisplayName("Codice statistico:"), Required(ErrorMessage = "Codice statistico obbligatorio")]

            public string SelectedCodiceStatistico { get; set; }
            public string TipoAttivita { get; set; }

            public string TipoIscrizione { get; set; }

            public string CodiceStatistico { get; set; }

            public string AbbonamentoPA { get; set; }

            public string AbbonamentoPAiscrizione { get; set; }

            public string CodiceCategoriaAttivita { get; set; }

            public string CodiceTipoAttivita { get; set; }
        }

        public class Documenti
        {
            [DisplayName("INFO SUI DOCUMENTI:")]
            public string Info { get; set; }
            public bool CCIAA { get; set; }

            public bool Statuto { get; set; }

            public bool Legalerapp { get; set; }

            public bool partitaIVA { get; set; }

            public bool DM80 { get; set; }

            public bool privacy { get; set; }

            public bool delega { get; set; }
        }

        public class SedeLegale : IndirizzoModel
        {

            public string DataDecorrenza { get; set; }

            public string TipoIndirizzo { get; set; }
            public string StatoEstero { get; set; }
            [DisplayName("Telefono:")]
            [RegularExpression(RegexRules.TelefonoRegex, ErrorMessage = "Numero di telefono non valido")]
            [MaxLength(13, ErrorMessage = "Telefono troppo lungo")]
            [RequiredIf($"{nameof(Cellulare)} == null", ErrorMessage = "Fornire almeno un telefono o un cellulare")]
            public string Telefono1 { get; set; }

            public string Telefono2 { get; set; }

            [DisplayName("Cellulare:")]
            [RegularExpression(RegexRules.CellulareRegex, ErrorMessage = "Cellulare non valido")]
            [MaxLength(20, ErrorMessage = "Cellulare troppo lungo")]
            [RequiredIf($"{nameof(Telefono1)} == null", ErrorMessage = "Fornire almeno un telefono o un cellulare")]
            public string Cellulare { get; set; }

            public string Fax { get; set; }

            public string Email { get; set; }

            public string EmailCert { get; set; }

            public string CodiceTipoIndirizzo { get; set; }

            public string CodiceComune { get; set; }

            public string CodiceLocalita { get; set; }
        }

        public class SedeAmministrativa
        {
            public string DataDecorrenza { get; set; }

            public string TipoIndirizzo { get; set; }

            public string Indirizzo { get; set; }

            public string Civico { get; set; }

            public string Comune { get; set; }

            public string Provincia { get; set; }

            public string CAP { get; set; }

            public string Localita { get; set; }

            public string StatoEstero { get; set; }

            public string Telefono1 { get; set; }

            public string Telefono2 { get; set; }

            public string Cellulare { get; set; }

            public string Fax { get; set; }

            public string Email { get; set; }

            public string EmailCert { get; set; }

            public string CodiceTipoIndirizzo { get; set; }

            public string CodiceComune { get; set; }

            public string CodiceLocalita { get; set; }
        }

        public class IndirizzoCorrispondenza : IndirizzoModel
        {
            public string Destinatario { get; set; }

            public string DataDecorrenza { get; set; }

            public string TipoIndirizzo { get; set; }
            public string StatoEstero { get; set; }
            [DisplayName("Telefono:")]
            [RegularExpression(RegexRules.TelefonoRegex, ErrorMessage = "Numero di telefono non valido")]
            [MaxLength(13, ErrorMessage = "Telefono troppo lungo")]
            [RequiredIf($"{nameof(Cellulare)} == null", ErrorMessage = "Fornire almeno un telefono o un cellulare")]
            public string Telefono1 { get; set; }

            public string Telefono2 { get; set; }

            [DisplayName("Cellulare:")]
            [RegularExpression(RegexRules.CellulareRegex, ErrorMessage = "Cellulare non valido")]
            [MaxLength(20, ErrorMessage = "Cellulare troppo lungo")]
            [RequiredIf($"{nameof(Telefono1)} == null", ErrorMessage = "Fornire almeno un telefono o un cellulare")]
            public string Cellulare { get; set; }

            public string Fax { get; set; }
            public string Email { get; set; }
            public string EmailCert { get; set; }

            public string CodiceTipoIndirizzo { get; set; }

            public string CodiceComune { get; set; }

            public string CodiceLocalita { get; set; }
        }

        public class NatGiu
        {
            public string NATGIU { get; set; }

            public string DENNATGIU { get; set; }
        }

        public class MezzComm
        {
            public string CODMEZ { get; set; }

            public string DENMEZ { get; set; }
        }

        public class TipoRapLeg
        {
            public string CODFUNRAP { get; set; }

            public string DENFUNRAP { get; set; }
        }

        public class Via
        {
            public string CODDUG { get; set; }

            public string DENDUG { get; set; }
        }

        public class Comune
        {
            public string CODCOM { get; set; }

            public string DENCOM { get; set; }
            public string SIGPRO { get; set; }
        }

        public class Provincia
        {
            public string SIGPRO { get; set; }
            public string CODREG { get; set; }
            public string DENPRO { get; set; }
        }

        public class CategoriaAttivita
        {
            public string CATATTCAM { get; set; }

            public string DENCATATT { get; set; }
        }

        public class TipoAttivita
        {
            public string CODATTCAM { get; set; }

            public string DENATTCAM { get; set; }

            public string CATATTCAM { get; set; }
        }

        public class TipoIscrizione
        {
            public string CODATTCAM { get; set; }

            public string DENATTCAM { get; set; }
        }

        public class CodiceStatistico
        {
            public string CODSTACON { get; set; }

            public string DENCODSTA { get; set; }
        }

        public class Localita
        {
            public string CODLOC { get; set; }

            public string CODCOM { get; set; }

            public string DENLOC { get; set; }
            public string CAP { get; set; }
        }

        public List<DatiAzienda> Ricerca { get; set; }

        public List<NatGiu> natgiu { get; set; }

        public List<MezzComm> GetMezzs { get; set; }

        public List<TipoRapLeg> tipoRapLegs { get; set; }

        public List<Via> vias { get; set; }

        public List<Comune> Comunes { get; set; }

        public List<CategoriaAttivita> GetCategoriaAttivitas { get; set; }

        public List<TipoAttivita> tipoAttivitas { get; set; }

        public List<Localita> ListaLocalita { get; set; }

        public List<CodiceStatistico> CodiceStatisticos { get; set; }

        public DatiAzienda datiAzienda { get; set; }

        public RapLeg rapleg { get; set; }

        public AltriDati altridati { get; set; }

        public Documenti documenti { get; set; }

        public SedeLegale sedeLegale { get; set; }

        public SedeAmministrativa sedeAmministrativa { get; set; }

        public IndirizzoCorrispondenza indirizzoCorrispondenza { get; set; }
    }
}
