using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace OCM.TFI.OCM.Utilities.CustomValidators;

public class FileSize : ValidationAttribute, IClientValidatable
{
    private readonly int _fileSizeInBytes;
    public FileSize(int fileSizeInBytes)
    {
        _fileSizeInBytes = fileSizeInBytes;
    }
    public override bool IsValid(object value)
    {
        var file = value as HttpPostedFileWrapper;
        if (file == null) return true;
        return file.ContentLength <= _fileSizeInBytes;
    }

    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
        var rule = new ModelClientValidationRule
        {
            ValidationType = "filesize",
            ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
        };
        
        rule.ValidationParameters.Add("maxsize", _fileSizeInBytes);

        yield return rule;
    }
}