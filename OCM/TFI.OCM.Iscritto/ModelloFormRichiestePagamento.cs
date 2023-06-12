using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OCM.TFI.OCM.Iscritto
{
    public class ModelloFormRichiestePagamento
    {
        public string TipoPagamento { get; set; }
        public string Iban { get; set; }
        public string BICSWIFT { get; set; }
        public string CheckFruizione { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string ScadenzaDocumento { get; set; }
        public HttpPostedFileBase FronteDocumento { get; set; }
        public HttpPostedFileBase RetroDocumento { get; set; }
    }
}
