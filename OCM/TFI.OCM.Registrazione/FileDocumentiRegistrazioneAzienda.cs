using System.ComponentModel;
using OCM.TFI.OCM.Utilities;
using TFI.OCM.Utilities;

namespace OCM.TFI.OCM.Registrazione;

public class FileDocumentiRegistrazioneAzienda
{
    public FileDocumentiRegistrazioneAzienda()
    {
        CertificatoIscrizioneCCIAA = new Documento(TipoDocumento.CertificatoIscrizioneCCIAA) ;
        StatutoAttoCostitutivo = new Documento(TipoDocumento.StatutoAttoCostitutivo);
        DocumentoLegaleRappresentante = new Documento(TipoDocumento.DocumentoLegaleRappresentante);
        PartitaIva = new Documento(TipoDocumento.PartitaIva);
        DM80 = new Documento (TipoDocumento.DM80);
        InformativaPrivacy = new Documento(TipoDocumento.InformativaPrivacy);
        DelegaConsulente = new Documento(TipoDocumento.DelegaConsulente);
    }
    [DisplayName("Certificato Iscrizione CCIAA:")]
    public Documento CertificatoIscrizioneCCIAA { get; set; }
    [DisplayName("Statuto Atto Costitutivo:")]
    public Documento StatutoAttoCostitutivo { get; set; }
    [DisplayName("Documento Legale Rappresentante:")]
    public Documento DocumentoLegaleRappresentante { get; set; }
    [DisplayName("Partita Iva:")]
    public Documento PartitaIva { get; set; }
    [DisplayName("DM80:")]
    public Documento DM80 { get; set; }
    [DisplayName("Informativa Privacy:")]
    public Documento InformativaPrivacy { get; set; }
    [DisplayName("Delega Consulente:")]
    public Documento DelegaConsulente { get; set; }
}