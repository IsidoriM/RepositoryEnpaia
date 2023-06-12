using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCM.TFI.OCM.AziendaConsulente
{
    public class FilterRDLModel
    {
        public string Filter { get; set; }
        public TipoRDL TipoRDL { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public OrderByType OrderBy { get; set; }
        public bool OrderDesc { get; set; }
    }
}
