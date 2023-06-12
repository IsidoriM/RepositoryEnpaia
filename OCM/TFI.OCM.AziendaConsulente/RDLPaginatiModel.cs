using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFI.OCM.AziendaConsulente;

namespace OCM.TFI.OCM.AziendaConsulente
{
    public class RDLPaginatiModel : FilterRDLModel
    {
        public List<RapportoDiLavoroLight> Rapporti { get; set; } = new List<RapportoDiLavoroLight>();
        public int TotalCount { get; set; }
        public int TotalPage { get; set; }
    }
}
