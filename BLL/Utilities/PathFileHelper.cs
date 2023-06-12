using System;
using System.IO;

namespace TFI.BLL.Utilities;

public static class PathFileHelper
{
    private static readonly string _baseDirectoryTFI = AppDomain.CurrentDomain.BaseDirectory;
    public static readonly string PathLogoEnpaia = Path.Combine(_baseDirectoryTFI, "Images", "logo-orizzontale-Enpaia.jpg");
    public static readonly string FontPath = Path.Combine(_baseDirectoryTFI + "Utilities", "Fonts", "Lato-Regular.ttf");
    public static readonly string FontPathBlack = Path.Combine(_baseDirectoryTFI, "Utilities", "Fonts", "Lato-Black.ttf");
    public static readonly string PathTemplateRicevutaDipa = Path.Combine(_baseDirectoryTFI, "Templates", "TEMPLATE_RICEVUTA_DIPA.pdf");
    public static readonly string TemplateRegistrazioneAzienda = Path.Combine(_baseDirectoryTFI, "Templates", "TEMPLATE_RICEVUTA_REGAZI.pdf");
    public static readonly string TemplateRicevutaArretrato = Path.Combine(_baseDirectoryTFI, "Templates", "TEMPLATE_RICEVUTA_ARRETRATI.pdf");
}