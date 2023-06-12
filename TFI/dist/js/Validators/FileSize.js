$.validator.addMethod("filesize", function (documento, element, params) {   
    if(element.files.length <= 0) return true;
    
    return element.files[0].size <= +params.maxsize;
})

$.validator.unobtrusive.adapters.add("filesize", ["maxsize"], function (options) {
    options.rules["filesize"] = options.params;
    options.messages["filesize"] = options.message;
});