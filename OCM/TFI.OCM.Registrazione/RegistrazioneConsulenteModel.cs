using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExpressiveAnnotations.Attributes;
using OCM.TFI.OCM.Utilities.CustomValidators;

namespace OCM.TFI.OCM.Registrazione;

public class RegistrazioneConsulenteModel : BaseRegistrationModel
{
    [DisplayName("Codice fiscale:")]
    [CodiceFiscale(ErrorMessage = "Codice fiscale invalido")]
    [RequiredIf($"{nameof(PartitaIva)} == null", ErrorMessage = "Codice fiscale o Partita iva obbligatori")]
    public string CodiceFiscale { get; set; }
        
    [DisplayName("Partita IVA:")]            
    [RequiredIf($"{nameof(CodiceFiscale)} == null",ErrorMessage = "Partita iva o Codice fiscale obbligatori")]
    [MaxLength(11, ErrorMessage = "Partita iva troppo lunga")]
    public string PartitaIva { get; set; }
        
    [DisplayName("Denominazione:")]            
    [Required(ErrorMessage = "Denominazione obbligatoria")]
    public string Denominazione { get; set; }
        
    [DisplayName("Associazione/Studio:")]
    [Required(ErrorMessage = "Associazione/studio obblicatorio")]
    public string SelectedAssociazione { get; set; }
}