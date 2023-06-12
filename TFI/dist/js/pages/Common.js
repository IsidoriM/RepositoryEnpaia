

$('.numberonly').keypress(function (e) {
    
    var charCode = (e.which) ? e.which : event.keyCode;
    var value = $(this).val();   

    if (IsDecimal(value)) {
        if (String.fromCharCode(charCode).match(/[^0-9]/g) || String.fromCharCode(charCode).match(/[<>'"/;`%]/g)) {
            return false;
        }
    }
    else {
        if (String.fromCharCode(charCode).match(/[^0-9,.]/g) || String.fromCharCode(charCode).match(/[<>'"/;`%]/g)) {
            return false;
        }
    }
    
    
});

function IsDecimal(value) {
    if (value.includes(',') || value.includes('.')) {
        return true;
    }
    else {
        return false;
    }
}