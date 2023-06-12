using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OCM.TFI.OCM.Registrazione;

namespace TFI.Utilities
{
    public static class DropdownExtensions
    {
        public static IList<SelectListItem> ToSelectListItem(this IEnumerable<DropdownModel> dropdownModels)
        {
            return dropdownModels.Select((model) => new SelectListItem
            {
                Value = model.Key,
                Text = model.Value
            }).ToList();
        }
    }
}