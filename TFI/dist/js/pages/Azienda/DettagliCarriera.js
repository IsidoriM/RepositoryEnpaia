$(document).ready(function () {
    setTabNavigationViaButtons();
    tipRapOptionSelection($("#codTipRap").val());
    fillDurataMesi();
});

function setTabNavigationViaButtons() {
    $('#datiAnagraficiNext').click(() => goToTabOf('#datiContrattualiTab'));
    $('#datiContrattualiBack').click(() => goToTabOf('#datiAnagraficiTab'));
    $('#datiContrattualiNext').click(() => goToTabOf('#datiRetributiviTab'));
    $('#datiRetributiviBack').click(() => goToTabOf('#datiContrattualiTab'));
    $('#datiRetributiviNext').click(() => goToTabOf('#aziendaUtilizzatriceTab'));
    $('#aziendaUtilizzatriceBack').click(() => goToTabOf('#datiRetributiviTab'));
}

function goToTabOf(tabId) {
    $(tabId).tab('show');
}

function tipRapOptionSelection(valueSelected) {

    if (valueSelected == 1) {
        $("#da_al").hide();
        $("#da_al1").hide();
        $("#da_al2").hide();
        $("#part_time").hide();
        $("#mesi").hide();
        $("#periododll").hide();
    }

    //A TERMINE
    if (valueSelected == 2) {
        $("#data_termine").show();
        $("#part_time").hide();
        $("#da_al").hide();
        $("#da_al1").hide();
        $("#da_al2").hide();
        $("#mesi").hide();
        $("#periododll").hide();
    }

    //Part time
    if (valueSelected == 3) {
        $("#part_time").show();
        $("#data_termine").hide();
        $("#da_al").hide();
        $("#da_al1").hide();
        $("#da_al2").hide();
        $("#mesi").hide();
        $("#periododll").hide();
    }
    //Part time a termine
    if (valueSelected == 4) {
        $("#part_time").show();
        $("#data_termine").show();
        $("#da_al").hide();
        $("#da_al1").hide();
        $("#da_al2").hide();
        $("#mesi").hide();
        $("#periododll").hide();
    }

    if (valueSelected == 6) {
        $("#part_time").hide();
        $("#data_termine").hide();
        $("#da_al").css('display', 'flex');
        $("#da_al1").css('display', 'flex');
        $("#da_al2").css('display', 'flex');
        $("#mesi").hide();
        $("#periododll").show();
        if ($('#datIsc').val() != '')
            $('#daal').val($('#datIsc').val());
    }

    //APPRENDISTATO
    if (valueSelected == 7) {
        $("#part_time").hide();
        $("#data_termine").hide();
        $("#da_al").css('display', 'flex');
        $("#da_al1").css('display', 'flex');
        $("#da_al2").css('display', 'flex');
        $("#mesi").hide();
        $("#periododll").show();
        if ($('#datIsc').val() != '')
            $('#daal').val($('#datIsc').val());
    }

    if (valueSelected == 8) {
        $("#part_time").show();
        $("#data_termine").hide();
        $("#da_al").css('display', 'flex');
        $("#da_al1").css('display', 'flex');
        $("#da_al2").css('display', 'flex');
        $("#mesi").hide();
        $("#periododll").show();
        if ($('#datIsc').val() != '')
            $('#daal').val($('#datIsc').val());
    }

    //Contratto di inserimento
    if (valueSelected == 9) {
        $("#data_termine").show();
        $("#da_al").hide();
        $("#da_al1").hide();
        $("#da_al2").hide();
        $("#mesi").hide();
        $("#part_time").hide();
        $("#periododll").hide();
    }

    //Apprendistato %
    if (valueSelected == 10) {
        $("#part_time").show();
        $("#data_termine").hide();
        $("#da_al").css('display', 'flex');
        $("#da_al1").css('display', 'flex');
        $("#da_al2").css('display', 'flex');
        $("#mesi").hide();
        $("#periododll").show();
        if ($('#datIsc').val() != '')
            $('#daal').val($('#datIsc').val());
    }

    if (valueSelected == 11) {
        $("#part_time").show();
        $("#mesi").show();
        $("#da_al").hide();
        $("#da_al1").hide();
        $("#da_al2").hide();
        $("#data_termine").hide();
        $("#periododll").hide();
    }

    //Apprendistato professionalizzante
    if (valueSelected == 12) {
        $("#data_termine").show();
        $("#da_al").hide();
        $("#da_al1").hide();
        $("#da_al2").hide();
        $("#part_time").hide();
        $("#mesi").hide();
        $("#periododll").hide();
    }

    if (valueSelected == 13) {
        $("#data_termine").show();
        $("#part_time").show();
        $("#da_al").hide();
        $("#da_al1").hide();
        $("#da_al2").hide();
        $("#mesi").hide();
        $("#periododll").hide();
    }

    if (valueSelected == 16) {
        console.log(valueSelected)
        $("#mesi").show();
        $("#data_termine").show();
        $("#da_al").hide();
        $("#da_al1").hide();
        $("#da_al2").hide();
        $("#part_time").hide();
        $("#periododll").hide();
    }

    //Part time verticale
    if (valueSelected == 17) {
        $("#mesi").show();
        $("#data_termine").hide();
        $("#da_al").hide();
        $("#da_al1").hide();
        $("#da_al2").hide();
        $("#part_time").hide();
        $("#periododll").hide();
    }

    //19 smart warking part time
    if (valueSelected == 19) {
        $("#mesi").hide();
        $("#data_termine").hide();
        $("#da_al").hide();
        $("#da_al1").hide();
        $("#da_al2").hide();
        $("#part_time").show();
        $("#periododll").hide();
    }

    //Apprendistato di alta formazione e ricerca %
    if (valueSelected == 21) {
        $("#mesi").hide();
        $("#data_termine").hide();
        $("#da_al").hide();
        $("#da_al1").hide();
        $("#da_al2").hide();
        $("#part_time").show();
        $("#periododll").hide();
    }

    if (valueSelected == 1 || valueSelected == 14 || valueSelected == 15 || valueSelected == 18 || valueSelected == 20 || valueSelected == 5 || valueSelected == '') {
        $("#mesi").hide();
        $("#data_termine").hide();
        $("#da_al").hide();
        $("#da_al1").hide();
        $("#da_al2").hide();
        $("#part_time").hide();
        $("#periododll").hide();
    }
}

function fillDurataMesi() {
    let primaData = new Date($("#datIni").val());
    let secondaData = new Date($("#datTerm").val());

    if (primaData.getDate() == 1) {
        primaData.setDate(2);
        secondaData.setDate(secondaData.getDate() + 1);
    }

    let difference = monthDiff(primaData, secondaData);
    $("#periodo").val(difference * 3);

    function monthDiff(d1, d2) {
        var months;
        months = (d2.getYear() - d1.getYear()) * 12;
        months -= d1.getMonth();
        months += d2.getMonth();
        return months <= 0 ? 0 : months;
    }
}