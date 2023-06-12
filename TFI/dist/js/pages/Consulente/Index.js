import {GetInitializerOptionsWithUniformedPagingAndSearchingBox} from "../../Utilities/DataTableHelper.js"

$(document).ready(function () {
    $("#tabella_Aziende").DataTable(GetInitializerOptionsWithUniformedPagingAndSearchingBox('tabella_Aziende'));
    $('#ricercaAziendaButton').click(() => cercaAzienda());
    $('#linkRichiediDelega').click((e) => richiediDelega(e));
    formatSearchBar();

    function formatSearchBar() {
        let rigaBarraRicerca = document.getElementById('tabella_Aziende_wrapper');
        rigaBarraRicerca.removeChild(rigaBarraRicerca.firstChild);

        rigaBarraRicerca.firstChild.classList.remove('col-md-6');
        rigaBarraRicerca.firstChild.classList.remove('col-sm-12');
        rigaBarraRicerca.firstChild.classList.add('col-10');

        let labelRicerca = document.getElementById('tabella_Aziende_filter').firstChild;

        labelRicerca.classList.add('d-flex');
        labelRicerca.classList.add('justify-content-around');
        labelRicerca.firstElementChild.classList.add('w-50');
    }
});

async function cercaAzienda() {
    const identificativo = $('#CercaAzienda').val();
    let ricercaAziendaResult = (await cercaAziendaAjax(identificativo));

    if (!ricercaAziendaResult.azienda) {
        window.alert(ricercaAziendaResult.errorMsg)
        return;
    }

    mostraAziendaRicercata();

    function mostraAziendaRicercata() {
        $('#tabella_Esito_Ricerca').attr('hidden', false);
        $('#codPosAzienda').text(ricercaAziendaResult.azienda.CodiceIdentificativo);
        $('#pivaAzienda').text(ricercaAziendaResult.azienda.PartitaIva);
        $('#utenzaAzienda').text(ricercaAziendaResult.azienda.RagioneSociale);
    }
}

async function cercaAziendaAjax(identificativo) {
    return $.ajax({
        cache: false,
        url: '/Consulente/CercaAziendaSenzaDelegaAttivaAjax',
        type: 'GET',
        data: { identificativo: identificativo },
        error: (error) => console.error(error)
    });
}

function richiediDelega(ev) {
    ev.preventDefault();
    let confirmResult = window.confirm('Confermi la richiesta di delega?');

    if (!confirmResult)
        return;

    const codPos = $('#codPosAzienda').text().trim();
    $('#codPos').val(codPos);
    $('#richiestaDelegaForm').submit();
}