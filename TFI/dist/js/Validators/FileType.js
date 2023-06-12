$.validator.addMethod("filetype", function (documento, element, params) {
    let acceptedMimeTypes = JSON.parse(params.mimetypes);
    if(element.files[0] === undefined) return true;
    
    let fileType = element.files[0].type;
    return !!acceptedMimeTypes.includes(fileType);
})

$.validator.unobtrusive.adapters.add("filetype", ["mimetypes"], function (options) {
    options.rules["filetype"] = options.params;
    options.messages["filetype"] = options.message;
});