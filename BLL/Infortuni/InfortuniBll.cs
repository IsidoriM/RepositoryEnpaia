using System.Collections.Generic;
using OCM.TFI.OCM;
using OCM.TFI.OCM.Infortuni;
using DAL.Infortuni;

namespace TFI.BLL.Infortuni;

public class InfortuniBll
{
    private readonly InfortuniDAL _infortuniDal;

    public InfortuniBll()
    {
        _infortuniDal = new InfortuniDAL();
    }

    public ResultDtoWithContent<List<IscrittoTrovato>> RicercaIscritto(RicercaIscritto ricercaIscritto)
        => _infortuniDal.RicercaIscritto(ricercaIscritto);
}