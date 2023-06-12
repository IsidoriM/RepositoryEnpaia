
// -------------------------------------------------------------------------------- 
//                                                                            READY
// --------------------------------------------------------------------------------

$(document).ready(function () {

    $('.toast').toast('show');
    // ---------------------------------------------------------------------------- CREAZIONE BTN PAGINAZ.
    var tagPrev = "<td id='PREC'>Precedente</td>";
    var tagSucc = "<td id='SUCC'>Successivo</td>";
    var numPagine;
    var totRecords = $("#totDipendenti").html();
    totRecords = parseInt(totRecords);
    var righePerPagina = 10;

    numPagine = totRecords / righePerPagina;

    $("#btn_paginazione tr").html("");
    $("#btn_paginazione tr").append(tagPrev);

    for (var b = 1; b <= numPagine; b++) {
        $("#btn_paginazione tr").append("<td class='number'>" + b + "</td>");
    }

    $("#btn_paginazione tr").append(tagSucc);
    $("#PREC").css('display', 'none');
    // ---------------------------------------------------------------------------- PAGINAZIONE
    $("#btn_paginazione tr td").click(function () {

        var paginaDiDestinazione;
        var paginaDiProvenienza = $("#currentPage").html();
        var bottonePremuto = $(this);
        var classeCSS = bottonePremuto.prop("class");
        var id = bottonePremuto.prop("id");

        if (classeCSS == "number") {
            paginaDiDestinazione = bottonePremuto.html();
        }
        else {
            if (id == "PREC") {
                paginaDiDestinazione = parseInt(paginaDiProvenienza) - 1;
            }
            else {
                paginaDiDestinazione = parseInt(paginaDiProvenienza) + 1;
            }
        }

        $("#paginaDiDestinazione").html(paginaDiDestinazione);
        var dati = GetDatiTabella();

        CambiaPagina(numPagine, paginaDiProvenienza, paginaDiDestinazione, dati);

    });

    // ---------------------------------------------------------------------------- VISUALIZZAZIONE IMPORTI
    PageLoadSettings();

    //var totFig = $("#totFigurativa").html();
    //totFig = GestioneInserimento(totFig);
    //$("#totFigurativa").html(totFig);

    //var totSan = $("#totSanitario").html();
    //totSan = GestioneInserimento(totSan);
    //$("#totSanitario").html(totSan);

});

// -------------------------------------------------------------------------------- 
//                                                                        FUNCTIONS
// --------------------------------------------------------------------------------

function GetDatiTabella() {

    var data = [];

    $(".txt_impRet").each(function () {

        var rowData;
        var riga = $(this).closest("tr");

        var mat = riga.children(".mat").html();
        var proRap = riga.children(".proRap").html();
        var impRet = $(this).val();
        var impOcc = riga.find(".txt_impOcc").val();

        mat = CleanValue(mat);
        proRap = CleanValue(proRap);

        if (impRet != null || impRet != "") {
            impRet = $.trim(impRet);
            impRet = CleanValue(impRet);
        }
        if (impOcc != null || impOcc != "") {
            impOcc = $.trim(impOcc);
            impOcc = CleanValue(impOcc);
        }

        rowData = {
            Matricola: mat,
            ProRap: proRap,
            ImpRet: impRet,
            ImpOcc: impOcc
        };

        data.push(rowData);
    });

    return data;
}

function CambiaPagina(numPagine, paginaProvenienza, paginaDestinazione, dati) {

    $.ajax({
        url: '/AziendaConsulente/PaginazioneArretrati',
        dataType: "json",
        method: "POST",
        data: { 'paginaProvenienza': paginaProvenienza, 'paginaSelezionata': paginaDestinazione, 'datiModificati_json': JSON.stringify(dati) },
        success: function (result) {

            var recordDaVisualizzare = JSON.parse(result);
            SostituisciRecord(recordDaVisualizzare);

            $("#currentPage").html(paginaDestinazione);

            //--------------------------------------------------- DISABILITA/ABILITA BTN

            if ((parseInt(paginaDestinazione) + 1) > parseInt(numPagine)) {
                $("#SUCC").css('display', 'none');
            }
            else {
                $("#SUCC").css('display', 'block');
            }

            if ((parseInt(paginaDestinazione) - 1) == 0) {
                $("#PREC").css('display', 'none');
            }
            else {
                $("#PREC").css('display', 'block');
            }
        },
        /*error: alert("Errore chiamata ajax"),*/
        complete: function () {
            PageLoadSettings();
        }
    });
}

function SostituisciRecord(recordDaVisualizzare) {

    var newcode = "";
    for (i in recordDaVisualizzare) {
        newcode += "<tr>";
        newcode += "<td class='mat'>" + recordDaVisualizzare[i].Mat + "</td>";
        newcode += "<td>" + recordDaVisualizzare[i].Nome + "</td>";
        newcode += "<td>" + recordDaVisualizzare[i].Dal + "</td>";
        newcode += "<td>" + recordDaVisualizzare[i].Al + "</td>";
        newcode += "<td>" + recordDaVisualizzare[i].Qualifica + "</td>";
        newcode += "<td>" + recordDaVisualizzare[i].Livello + "</td>";
        newcode += "<td class='prest'>" + recordDaVisualizzare[i].PerApp + "</td>";

        newcode += "<td> <input type='text' value='" + recordDaVisualizzare[i].ImpRet + "' id='impRet_" + recordDaVisualizzare[i].Mat + "' style='max-width: 100px' class='txt_impRet numberonly' oldValue='0' /> </td>";
        newcode += "<td> <input type='text' value='" + recordDaVisualizzare[i].ImpOcc + "' id='impOcc_" + recordDaVisualizzare[i].Mat + "' style='max-width: 80px' class='txt_impOcc numberonly' oldValue='0' /> </td>";
        newcode += "<td> <input type='text' value='" + recordDaVisualizzare[i].impFig + "' id='impFig_" + recordDaVisualizzare[i].Mat + "' style='max-width: 80px' class='txt_impFig' oldValue='0' readonly='readonly' /> </td>";

        newcode += "<td class='aliq'>" + recordDaVisualizzare[i].Aliquota + "</td>";
        newcode += "<td class='impCon'>" + recordDaVisualizzare[i].ImpCon + "</td>";
        /*newcode += "<td>" + record.AbbPre + "</td>";*/
        newcode += "<td class='assCon'>" + recordDaVisualizzare[i].AssCon + "</td>";
        newcode += "<td><a class='link_sosp'><i class='far fa-calendar-alt'></i></a></td>";
        newcode += "<td class='impSanit'>" + recordDaVisualizzare[i].ImpSanit + "</td>";
        newcode += "<td style='display:none' class='proRap'>" + recordDaVisualizzare[i].ProRap + "</td>";
        newcode += "<td style='display:none' class='impMin'>" + recordDaVisualizzare[i].ImpMin + "</td>";
        newcode += "<td style='display:none' class='impSca'>" + recordDaVisualizzare[i].ImpSca + "</td>";
        newcode += "<td style='display:none' class='traEco'>" + recordDaVisualizzare[i].ImpTraEco + "</td>";
        newcode += "</tr>";
    }

    $("#tb_DenunciaMensile tbody").html(newcode);
}

function PageLoadSettings() {


    $("#tb_DenunciaMensile .link_sosp").click(function () {
        var obj = $(this);
        VisualizzaSospensioni(obj);
    });

    $(".txt_impRet").each(function () {
        var value = $(this).val();
        if (value != "" && value != "null") {
            value = $.trim(value);
            value = CleanValue(value);
            $(this).attr("oldvalue", value);
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
            $(this).attr("oldvalue", value);
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

    $(".txt_impRet").blur(function () {

        var impRet = $(this).val();
        if (IsValued(impRet)) {
            if (impRet.startsWith(',') || impRet.startsWith('.')) {
                impRet = '0';
            }
            if ($.trim(impRet) == "€") {
                impRet = '0';
            }
            impRet = parseFloat(CleanValue($.trim(impRet)).toFixed(2));
            var impOcc = $(this).closest("tr").children().children(".txt_impOcc").val();
            var minCon = $(this).closest("tr").children(".impMin").html();
            var impSca = $(this).closest("tr").children(".impSca").html();
            var traEco = $(this).closest("tr").children(".traEco").html();
            var aliq = $(this).closest("tr").children(".aliq").html();
            var impCon = 0;
            $(this).val(GestioneInserimento(impRet));

            if (IsValued(impOcc) && IsValued(minCon) && IsValued(aliq) && IsValued(impSca) && IsValued(traEco)) {

                impOcc = parseFloat(CleanValue($.trim(impOcc)).toFixed(2));
                minCon = parseFloat(CleanValue($.trim(minCon)).toFixed(2));
                aliq = parseFloat(CleanValue($.trim(aliq)).toFixed(2));
                impSca = parseFloat(CleanValue($.trim(impSca)).toFixed(2));
                traEco = parseFloat(CleanValue($.trim(traEco)).toFixed(2));

                if (impOcc > impRet && impOcc > 0) {
                    $(this).closest("tr").children().children(".txt_impOcc").focus();
                    $(this).focus();
                }
                else {

                    if (traEco > 0) {
                        if (impRet < traEco && impRet > 0) {
                            alert("Attenzione! E\' stato inserito un imponibile inferiore al trattamento economico.");
                        }
                    }
                    else {
                        if (impRet < (minCon + impSca) && impRet > 0) {
                            alert("Attenzione! E\' stato inserito un imponibile inferiore al minimo contrattuale.");
                        }
                    }
                }

                //Calcoliamo l'importo dei contributi
                //-----------------------------------
                impCon = ((impRet * aliq) / 100).toFixed(2);
                impCon = GestioneInserimento(impCon);
                $(this).closest("tr").children(".impCon").html(impCon);
            }
        }
        else {
            $(this).val(GestioneInserimento(0));
        }
        SetTotale($(this));
        $(this).attr('oldvalue', impRet);
    });

    $(".txt_impOcc").blur(function () {

        var impOcc = $(this).val();
        var impRet = $(this).closest("tr").children().children(".txt_impRet").val();

        if (IsValued(impOcc)) {
            if (impOcc.startsWith(',') || impOcc.startsWith('.')) {
                impOcc = '0';
            }
            if ($.trim(impOcc) == "€") {
                impOcc = '0';
            }
            impOcc = CleanValue($.trim(impOcc)).toFixed(2);
            impRet = CleanValue($.trim(impRet)).toFixed(2);
            $(this).val(GestioneInserimento(impOcc));

            impOcc = parseFloat(impOcc);
            impRet = parseFloat(impRet);

            if (impOcc > impRet && impOcc > 0) {
                alert("L\'importo occasionale non puo\' essere maggiore dell'imponibile!");
                $(this).focus();
                $(this).val('0,00');
                $(this).select();
                $(this).attr('oldvalue', 0);
            }
        }
        else {
            $(this).val(GestioneInserimento(0));
        }
        SetTotale($(this));
        $(this).attr('oldvalue', impOcc);
    });


    $(".txt_impRet").click(function () {
        $(this).select();
    });
    $(".txt_impOcc").click(function () {
        $(this).select();
    });

    var totaleImponibili = $("#totImponibili").html();
    var totaleOccasionali = $("#totOccasionali").html();

    totaleImponibili = GestioneInserimento(totaleImponibili);
    totaleOccasionali = GestioneInserimento(totaleOccasionali);

    $("#totImponibili").html(totaleImponibili);
    $("#totOccasionali").html(totaleOccasionali);
}

function VisualizzaSospensioni(obj) {

    var matricola = obj.parent().parent().children().eq(0).html();
    var proRap = obj.parent().parent().children(".proRap").html();
    var dal = obj.parent().parent().children().eq(2).html();
    var al = obj.parent().parent().children().eq(3).html();
    var row = "";

    $.ajax({
        url: '/AziendaConsulente/CaricaListaSospensioni',
        dataType: "json",
        method: "POST",
        data: { 'p_matricola': matricola, 'p_proRap': proRap, 'p_dataIni': dal, 'p_dataFin': al },
        success: function (result) {

            var listaSospensioni = JSON.parse(result);
            $("#numMatricola").html(matricola);
            SvuotaTabellaSospensioni();

            if (listaSospensioni != null) {

                for (var i in listaSospensioni) {
                    row = "<tr>";
                    row += "<td>" + listaSospensioni[i].DataInizio + "</td>";
                    row += "<td>" + listaSospensioni[i].DataFine + "</td>";
                    row += "<td>" + listaSospensioni[i].DenSos + "</td>";
                    row += "</tr>";
                    $("#tb_sospensioni tbody").append(row);
                }
            }
        },
        /*error: alert("Errore chiamata ajax"),*/
        complete: function () { $("#sospensioni_modal").modal("show"); }
    });
}

function SvuotaTabellaSospensioni() {
    var row = "<tr><td></td></tr>";
    $("#tb_sospensioni tbody").html(row);
}

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

function IsValued(value) {
    if (value != null && value != "" && value != "undefined") {
        return true;
    }
    else {
        return false;
    }
}

function SetTotale(textBox) {
    //debugger;
    var classi = textBox.prop("class");
    var totaleImponibili = $("#totImponibili").html();
    var totaleOccasionali = $("#totOccasionali").html();
    var importoCorrente = textBox.val();
    var importoPrecedente = textBox.attr("oldvalue");

    // Aggiornamento dei totali
    // ------------------------
    if (IsValued(importoCorrente) && IsValued(importoPrecedente)) {

        importoCorrente = parseFloat(CleanValue(importoCorrente).toFixed(2));
        importoPrecedente = parseFloat(CleanValue(importoPrecedente).toFixed(2));

        if (classi.includes("txt_impRet")) {
            if (IsValued(totaleImponibili)) {
                totaleImponibili = parseFloat(CleanValue(totaleImponibili).toFixed(2));
                totaleImponibili = totaleImponibili - importoPrecedente;
                totaleImponibili = totaleImponibili + importoCorrente;
                totaleImponibili = GestioneInserimento(totaleImponibili);
                $("#totImponibili").html(totaleImponibili);
            }
        }
        else if (classi.includes("txt_impOcc")) {
            if (IsValued(totaleOccasionali)) {
                totaleOccasionali = parseFloat(CleanValue(totaleOccasionali).toFixed(2));
                totaleOccasionali = totaleOccasionali - importoPrecedente;
                totaleOccasionali = totaleOccasionali + importoCorrente;
                totaleOccasionali = GestioneInserimento(totaleOccasionali);
                $("#totOccasionali").html(totaleOccasionali);
            }
        }

        // impostazione dell'importo precedente
        // ------------------------------------
    }
    textBox.prop("oldValue", importoCorrente);
}

function Conferma() {
    if (confirm('Confermi operazione?')) {
        $("#btn_paginazione tr #SUCC").click();
        $("#btn_paginazione tr #PREC").click();
        return true;
    }
    else {
        return false;
    }
}