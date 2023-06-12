import {DatiInfortunio, DatiIscritto} from "./Selector.js";

$(document).ready(() => {
    $(DatiIscritto.nextButton).click(() => $(DatiInfortunio.tabId).tab('show'));
    $(DatiInfortunio.backButton).click(() => $(DatiIscritto.tabId).tab('show'));
})
$(document).on('iscrittoSelezionato', (e, data) => {
    $(DatiIscritto.matricola).val(data.Matricola);
    $(DatiIscritto.nome).val(data.Nome);
    $(DatiIscritto.cognome).val(data.Cognome);
    $(DatiIscritto.codiceFiscale).val(data.CodiceFiscale);
})