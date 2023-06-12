using OCM.TFI.OCM.Utilities;
using System.Collections.Generic;

namespace OCM.TFI.OCM.Infortuni
{
    public class InfortuniIndexModel : PagingModel
    {
        public IEnumerable<PraticaInfortunio> Infortuni { get; set; }
    }
}