using System.Collections.Generic;
using Dropdown;
using OCM.TFI.OCM.Registrazione;

namespace TFI.BLL.Dropdown;

public class DropdownBLL
{
    private readonly DropdownDAL _dropdownDal;
    public DropdownBLL()
    {
        _dropdownDal = new DropdownDAL();
    }

    public IEnumerable<DropdownModel> GetAssociazioni() => _dropdownDal.GetAssociazioni();

    public IEnumerable<DropdownModel> GetTipiVia() => _dropdownDal.GetTipiVia();

    public IEnumerable<DropdownModel> GetProvince() => _dropdownDal.GetProvince();

    public IEnumerable<DropdownModel> GetStatiEsteri() => _dropdownDal.GetStatiEsteri();

    public IEnumerable<DropdownModel> GetNatureGiuridiche() => _dropdownDal.GetNatureGiuridiche();

    public IEnumerable<DropdownModel> GetGeneri() => _dropdownDal.GetGeneri();

    public IEnumerable<DropdownModel> GetIncarichiRappresentante() => _dropdownDal.GetIncarichiRappresentante();

    public IEnumerable<DropdownModel> GetCategorieAttivita() => _dropdownDal.GetCategorieAttivita();

    public IEnumerable<DropdownModel> GetCodiciStatistici() => _dropdownDal.GetCodiciStatistici();

    public IEnumerable<DropdownModel> GetComuniFrom(string provincia) => _dropdownDal.GetComuniFrom(provincia);

    public IEnumerable<DropdownModel> GetLocalitaFrom(string codiceComune) => _dropdownDal.GetLocalitaFrom(codiceComune);

    public IEnumerable<DropdownModel> GetTipiAttivitaFrom(string categoria) => _dropdownDal.GetTipiAttivitaFrom(categoria);

    public IEnumerable<DropdownModel> GetAnniCompetenzaNonPrescrittiFrom(string codicePosizione) => _dropdownDal.GetAnniCompetenzaNonPrescrittiFrom(codicePosizione);

    public IEnumerable<DropdownModel> GetTipiInfortunio() => _dropdownDal.GetTipiInfortunio();
}