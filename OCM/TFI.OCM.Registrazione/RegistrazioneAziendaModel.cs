using System.ComponentModel.DataAnnotations;
using TFI.OCM.Amministrativo;

namespace OCM.TFI.OCM.Registrazione;

public class RegistrazioneAziendaModel
{
    public RegistrazioneAziendaModel()
    {
        SedeLegale = new GestioneAziendeWebOCM.SedeLegale();
        Corrispondenza = new GestioneAziendeWebOCM.IndirizzoCorrispondenza();
        RappresentanteLegale = new GestioneAziendeWebOCM.RapLeg();
        AltriDati = new GestioneAziendeWebOCM.AltriDati();
    }
    [Required]
    public GestioneAziendeWebOCM.DatiAzienda DatiAzienda { get; set; }
    [Required]
    public GestioneAziendeWebOCM.SedeLegale SedeLegale { get; set; }
    [Required]
    public GestioneAziendeWebOCM.IndirizzoCorrispondenza Corrispondenza { get; set; }
    [Required]
    public GestioneAziendeWebOCM.RapLeg RappresentanteLegale { get; set; }
    [Required]
    public GestioneAziendeWebOCM.AltriDati AltriDati { get; set; }
    public FileDocumentiRegistrazioneAzienda DocumentiRegistrazioneAzienda { get; set; }
}