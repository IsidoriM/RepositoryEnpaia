
$(document).ready(function () {

    let valore;

    $('.toast').toast('show');

    valore = $("#txtDataVersam_attr_today").html();
    if (valore != null) {
        $("#txtDataVersamento").attr("today", valore);
    }

    valore = $("#txtDataVersam_attr_value").html();
    if (valore != null) {
        valore = valore.substring(0, 11);
        $("#txtDataVersamento").attr("value", valore);
    }

    valore = $("#txtImpVersato_attr_value").html();
    if (valore != null) {
        $("#txtImportoVersato").attr("value", valore);
    }

    valore = $("#txtDataVersamento_readonly").html();
    if (valore) {
        $("#txtDataVersamento").prop("readonly", true);
    }
    else if (!valore) {
        $("#txtDataVersamento").prop("readonly", false);
    }

    valore = $("#txtImportoVersato_readonly").html();
    if (valore) {
        $("#txtImportoVersato").prop("readonly", true);
    }
    else if (!valore) {
        $("#txtImportoVersato").prop("readonly", false);
    }

    valore = $("#txtDataVersamentoSanit_readonly").html();
    if (valore) {
        $("#txtDataVersamentoSanit").prop("readonly", true);
    }
    else if (!valore) {
        $("#txtDataVersamentoSanit").prop("readonly", false);
    }

    valore = $("#txtImportoVersatoSanit_readonly").html();
    if (valore) {
        $("#txtImportoVersatoSanit").prop("readonly", true);
    }
    else if (!valore) {
        $("#txtImportoVersatoSanit").prop("readonly", false);
    }

    $("#txtCrediti").focus(function () {
        $(this).select();

    });

    valore = $("txtCrediti_readonly").html();
    if (valore) {
        $("#txtCrediti").prop("readonly", true);
    }
    else if (!valore) {
        $("#txtCrediti").prop("readonly", false);
    }

    VisualizzaBottone();

    $("#btn_decurtaCredito").click(function () {
        var anno = $("#anno").val();
        var mese = $("#mese").val();
        var proDen = $("#proDen").val();
        var txtCrediti = $("#txtCrediti").val();

        DecurtaCredito(anno, mese, proDen, txtCrediti);
    });

    $("#stampaDenuncia").click(function () {
        
        //var blnSanzione = false;
        //var anno = $("#anno").val();
        //var mese = $("#mese").val();
        //var proDen = $("#proDen").val();
        //var tipMov = $("#tipMov").val();
        //var sanzioni = parseFloat($("#lblSanzioni").val().replace(".", "").replace(",", "."));

        //if (sanzioni > 0) {
        //    if (confirm("Si desidera stampare anche i dati della sanzione?") == true) { blnSanzione = true; }
        //}

        //if (tipMov == "DP") {
        //    if (blnSanzione) {
        //        tipMov = "SD";
        //    }
        //}

        //var dati = {
        //    Anno: anno,
        //    Mese: mese,
        //    ProDen: proDen
        //};
        
        //$.ajax({
        //    type: "GET",
        //    url: "/Stampa/Stampa",
        //    data: { 'tipMov': tipMov, 'datiDiStampa': JSON.stringify(dati) }            
        //});
    });

    $("#stampaDenSanit").click(function () {

        //var idDipaDEF = $("#idDipaDEF").val();
        //var tipMov = "DENSANIT";

        //var dati = {
        //    IdDipa: idDipaDEF
        //};

        //$.ajax({
        //    type: "GET",
        //    url: "/Stampa/Stampa",
        //    data: { 'tipMov': tipMov, 'datiDiStampa': JSON.stringify(dati) }
        //});
    });

    $("#stampaRicSanit").click(function () {

        //var idDipaDEF = $("#idDipaDEF").val();
        //var tipMov = "RICSANIT";

        //var dati = {
        //    IdDipa: idDipaDEF
        //};

        //$.ajax({
        //    type: "GET",
        //    url: "/Stampa/Stampa",
        //    data: { 'tipMov': tipMov, 'datiDiStampa': JSON.stringify(dati) }
        //});
    });


    $("#stampaModuli").click(function () {

        //var idDipaDEF = $("#idDipaDEF").val();
        //var tipMov = "ADE";

        //var dati = {
        //    IdDipa: idDipaDEF
        //};

        //$.ajax({
        //    type: "GET",
        //    url: "/Stampa/Stampa",
        //    data: { 'tipMov': tipMov, 'datiDiStampa': JSON.stringify(dati) }
        //});
    });

});


function GetBtnType(value) {
    $("#btnType").prop("value", value);
}

function DecurtaCredito(anno, mese, proDen, txtCrediti) {

    let importoCrediti = parseFloat($("#txtCrediti").val().replace(',', '.')).toFixed(2);
    let totPagare = parseFloat($("#totPagare").val().replace(',', '.')).toFixed(2);
    let totDovuto = parseFloat($("#totDovuto").val().replace(',', '.')).toFixed(2);
    let decurtato = parseFloat($("#decurtato").val()).toFixed(2);
    let staDen = $("#staDen").val();

    decurtato = parseFloat(decurtato) + parseFloat(importoCrediti);
    decurtato = decurtato.toString().replace('.', ',');

    if (parseFloat(importoCrediti) <= parseFloat(totDovuto)) {
        totDovuto = parseFloat(totDovuto) - parseFloat(importoCrediti);
        totPagare = totDovuto;
        $("#decurtato").val(decurtato);
        importoCrediti = importoCrediti.toString().replace('.', ',');
        $("#impDec").val(importoCrediti);
    }
    else {
        totPagare = parseFloat("0");
        importoCrediti = totDovuto.toString().replace('.', ',');
        $("#impDec").val(importoCrediti);
        $("#txtCrediti").val(importoCrediti);
    }

    totPagare = totPagare.toFixed(2);
    totPagare = totPagare.toString().replace('.', ',');
    $("#totPagare").val(totPagare);
}

function VisualizzaBottone() {

    let staDen = $.trim($("#staDen").val());
    let decurtato = parseFloat($("#decurtato").val()).toFixed(2);

    if (staDen == "A") {
        if (decurtato > 0) {
            $("#btn_ripristina").css("display", "block");
        }
    }
}

function Conferma() {
    if (confirm("Confermare dati ad Enpaia?")) {
        GetBtnType('SALVA_TOTALE');
        return true;
    }
    else
        return false;
}
