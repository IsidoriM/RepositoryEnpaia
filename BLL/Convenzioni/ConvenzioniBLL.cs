using Convenzioni;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFI.OCM.Iscritto;

namespace TFI.BLL.Convenzioni
{
    public class ConvenzioniBLL
    {
        private readonly ConvenzioniDAL _convenzioniDAL = new();
        public IEnumerable<ListaConvenzioniIscritto> GetListaConvenzioni => _convenzioniDAL.GetConvenzioni();
    }
}
