using System.Web;
using OCM.TFI.OCM.Utilities.CustomValidators;
using TFI.OCM.Utilities;
using static OCM.TFI.OCM.Utilities.MimeTypes;

namespace OCM.TFI.OCM.Utilities;

public class Documento
{
    public Documento(TipoDocumento tipoDocumento)
    {
        TipoDocumento = tipoDocumento;
    }
    [FileSize(FileUnits.MB * 4, ErrorMessage = "File non deve superare i 4MB")]
    [FileType(Pdf, Doc, Docx, ErrorMessage = "Formato non valido (.pdf, .doc, .docx)")]
    public HttpPostedFileBase File { get; set; }
    public TipoDocumento TipoDocumento { get; set; }
    public string Uuid { get; set; }
    public int? IdAllegato { get; set; }
}