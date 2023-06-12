using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using TFI.OCM.Iscritto;

namespace TFI.OCM.Iscritto
{
    public class AnagraficaConPwd : Anagrafica
    {
        public string Password { get; set; }

        public string ConfermaPassword { get; set; }

        public bool Privacy { get; set; }
        [FileExtensions(Extensions = ".pdf,.jpeg,.jpg,.png", ErrorMessage = "Formato file non accettato")]
        public HttpPostedFileBase FronteFile { get; set; }

        [FileExtensions(Extensions = ".pdf,.jpeg,.jpg,.png", ErrorMessage = "Formato file non accettato")]
        public HttpPostedFileBase RetroFile { get; set; }
    }
}
