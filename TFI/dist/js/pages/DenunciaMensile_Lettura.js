$(document).ready(function () {

    PageLoadSettings();

    $("#tb_DenunciaMensile").DataTable({ "paging": true, "ordering": false, "lengthChange": false, "responsive": false, "autoWidth": false, "info": false });


});

function GestioneInserimento(value) {
    var array;
    value = value.toString().replace('.', ',');
    value = value.replace('€', '');

    if (value.indexOf(',') < 0) {
        value = value + ",00";
    }
    else {
        array = value.split(',');
        if (array[1].length == 1) {
            value = value + "0";
        }
    }
    return "€ " + value;
}

function CleanValue(value) {

    if (value.includes('€')) {
        value = value.replace('€', '');
    }
    value = value.replace(',', '.');
    return Math.round(parseFloat(value) * 100) / 100;
}

function PageLoadSettings() {

    $(".txt_impRet").each(function () {
        var value = $(this).val();
        if (value != "" && value != "null") {
            value = $.trim(value);
            value = CleanValue(value);
            $(this).val(GestioneInserimento(value));
        }
        else {
            $(this).val("€ 0,00");
        }
    });

    $(".txt_impOcc").each(function () {
        var value = $(this).val();
        if (value != "" && value != "null") {
            value = $.trim(value);
            value = CleanValue(value);
            $(this).val(GestioneInserimento(value));
        }
        else {
            $(this).val("€ 0,00");
        }
    });


    $(".txt_impFig").each(function () {
        var value = $(this).val();
        if (value != "" && value != "null" && value != "undefined") {
            value = $.trim(value);
            value = CleanValue(value);
            $(this).val(GestioneInserimento(value));
        }
        else {
            $(this).val("€ 0,00");
        }
    });

    $(".prest").each(function () {
        var value = $(this).html();
        if (value != "" && value != "null") {
            value = $.trim(value);
            value = CleanValue(value);
            $(this).html(GestioneInserimento(value));
        }
        else {
            $(this).html("€ 0,00");
        }
    });

    $(".aliq").each(function () {
        var value = $(this).html();
        if (value != "" && value != "null") {
            value = $.trim(value);
            value = CleanValue(value);
            value = value.toString().replace('.', ',');

            if (value.indexOf(',') < 0) {
                value = value + ",00";
            }
            else {
                array = value.split(',');
                if (array[1].length == 1) {
                    value = value + "0";
                }
            }

            $(this).html(value);
        }
        else {
            $(this).html("0,00");
        }
    });

    $(".impCon").each(function () {
        var value = $(this).html();
        if (value != "" && value != "null") {
            value = $.trim(value);
            value = CleanValue(value);
            $(this).html(GestioneInserimento(value));
        }
        else {
            $(this).html("€ 0,00");
        }
    });

    $(".impSanit").each(function () {
        var value = $(this).html();
        if (value != "" && value != "null") {
            value = $.trim(value);
            value = CleanValue(value);
            $(this).html(GestioneInserimento(value));
        }
        else {
            $(this).html("€ 0,00");
        }
    });

    var totImponibili = $("#totImponibili").html();
    var totOccasionale = $("#totOccasionali").html();
    var totFigurativa = $("#totFigurativa").html();
    var totSanitario = $("#totSanitario").html();

    totImponibili = GestioneInserimento(totImponibili);
    totOccasionale = GestioneInserimento(totOccasionale);
    totFigurativa = GestioneInserimento(totFigurativa);
    totSanitario = GestioneInserimento(totSanitario);

    $("#totImponibili").html(totImponibili);
    $("#totOccasionali").html(totOccasionale);
    $("#totFigurativa").html(totFigurativa);
    $("#totSanitario").html(totSanitario);
}