using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace OCM.TFI.OCM.Utilities.CustomValidators;

public class FileType : ValidationAttribute, IClientValidatable
{
    private readonly List<ContentType> _acceptedMimeTypes;
    public FileType(params string[] acceptedMimeTypes)
    {
        _acceptedMimeTypes = acceptedMimeTypes.Select(acceptedMimeType => new ContentType(acceptedMimeType)).ToList();
    }
    public override bool IsValid(object value)
    {
        var val = value as HttpPostedFileWrapper;
        if (val == default) return true;
        
        return _acceptedMimeTypes.Any(acceptedType => acceptedType.MediaType.Equals(val.ContentType));
    }

    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
        var rule = new ModelClientValidationRule
        {
            ValidationType = "filetype",
            ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
        };
        
        rule.ValidationParameters.Add("mimetypes", JsonConvert.SerializeObject(_acceptedMimeTypes.Select(x => x.MediaType)));

        yield return rule;
    }
}