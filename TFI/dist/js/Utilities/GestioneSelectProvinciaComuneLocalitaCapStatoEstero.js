/*
Queste funzioni gestiscono la compilazione automatica e lo svuotamento di 4 select (provincia, comune, localit� e statoEstero) ed un input text readonly (il cap). 

Perch� il flusso possa funzionare bisogna che siano presenti tutti questi 5 attori e la classe customReadOnlyForSelect che � la seguente 

.customReadonlyForSelect {
    pointer-events: none;
    background: #E9ECEF;
}

I selector da passargli sono gli id pi� cancelletto all'inzio per le JQuery (esempio: '#comune') mentre le rotte delle get sono quelle nel controller Registrazione.
Per poter importare queste funzioni � necessario che lo script che fa l'import sia di tipo module (vedere come esempio la view RegistrazioneAzienda).
*/
export async function provinciaChangeFunction({target}, {provincia, comune, localita, cap, statoEstero}) {
    const provinciaSelezionata = target.value

    if (!provinciaSelezionata) {
        cleanAndSetNotRequiredComuneLocalitaCapFields();
        return;
    }

    await getAndFillComuniSelectFrom(provinciaSelezionata);

    function cleanAndSetNotRequiredComuneLocalitaCapFields() {
        $(comune).html('');
        $(comune).attr('disabled', true);
        $(localita).html('');
        $(localita).attr('disabled', true);
        $(cap).val('');
    }

    async function getAndFillComuniSelectFrom(provinciaSelezionata) {
        
        let comuni = await getComuniFrom(provinciaSelezionata);
        
        $(comune).html(getComuniDropdownHtml(comuni));
        $(comune).attr('disabled', false);
        $(localita).val('');
        $(localita).html('');
        $(localita).attr('disabled', true);
        $(cap).val('');
        $(statoEstero).val('');
    }
}
async function getComuniFrom(provincia){
    return $.ajax({
        cache: false,
        url: "/Registrazione/GetComuniFromProvincia",
        type: 'GET',
        data: { provincia }
    });
}
export async function provinciaNascitaChanged({target},{comuneDiNascita, statoEsteroDiNascita}) {
    let comuni = await getComuniFrom((target.value));
    $(comuneDiNascita).html(getComuniDropdownHtml(comuni)).attr('disabled', false);
    $(statoEsteroDiNascita).val('');
}

export function statoEsteroNascitaChanged({provinciaDiNascita, comuneDiNascita}){
    $(provinciaDiNascita).val('');
    $(comuneDiNascita).val('').attr('disabled',true).html('')
}

function getComuniDropdownHtml(comuni){
    let elencoSelect = '<option value="">Selezionare Comune</option>';
    for (let i = 0; i < comuni.length; i++) {
        elencoSelect += '<option value="' + comuni[i].Value + '">' + comuni[i].Text + '</option>';
    }
    return elencoSelect;
}

export function comuneChangeFunction({target}, {localita, cap}) {
    const codiceComuneSelezionato = target.value;

    if (!codiceComuneSelezionato) {
        cleanAndSetNotRequiredLocalitaCapFields();
        return;
    }

    getAndFillLocalitaSelectFrom(codiceComuneSelezionato);

    function cleanAndSetNotRequiredLocalitaCapFields() {
        $(localita).html('');
        $(localita).attr('disabled', true);
        $(cap).val('');
    }

    function getAndFillLocalitaSelectFrom(codiceComuneSelezionato) {
        const uriGetLocalita = '/Registrazione/GetLocalitaFromComune';

        $.ajax({
            cache: false,
            url: uriGetLocalita,
            type: 'GET',
            data: { codiceComune: codiceComuneSelezionato },
            success: function (data) {
                let elencoSelect = '<option value="">Selezionare la Localita</option>';
                for (let i = 0; i < data.length; i++) {
                    elencoSelect += '<option value="' + data[i].Value + '">' + data[i].Text + '</option>';
                }
                $(localita).html(elencoSelect);
                $(localita).attr('disabled', false);
                $(cap).val('');
            },
            error: function () {}
        });
    }
}

export function localitaChangeFunction({target}, {localita, cap}) {
    const localitaSelezionataValue = target.value;

    if (!localitaSelezionataValue) {
        $(cap).val('');
        return;
    }

    const localitaSelezionataHtml = $(localita + ' option:selected').html();
    const capFromLocalitaSelezionata = localitaSelezionataHtml.split('-')[1].trim();

    $(cap).val(capFromLocalitaSelezionata);
}

export function statoEsteroChangeFunction({target}, {provincia, comune, localita, cap, statoEstero}) {
    const codiceStatoEsteroSelezionato = target.value;

    setRequiredAndAttributesToFieldsFrom(codiceStatoEsteroSelezionato);

    function setRequiredAndAttributesToFieldsFrom(codiceStatoEsteroSelezionato) {

        if (codiceStatoEsteroSelezionato) {
            $(provincia).val('');
            $(comune).val('');
            $(comune).html('');
            $(comune).attr('disabled', true);
            $(localita).val('');
            $(localita).html('');
            $(localita).attr('disabled', true);
            $(cap).val('');
            $(cap).html('');
        }
    }
}

export function categoriaAttivitaChanged({target}, {tipo}) {
    if(target.value === ""){
        $(tipo).attr('disabled', true);
        $(tipo).val('');
        return;
    }
    $.ajax({
        url: '/Registrazione/GetTipiAttivita',
        type: 'GET',
        data: { categoria: target.value },
        success: function (data) {
            let elencoSelect = '<option value="">Seleziona tipo attivita</option>';
            for (let i = 0; i < data.length; i++) {
                elencoSelect += '<option value="' + data[i].Value + '">' + data[i].Text + '</option>';
            }
            $(tipo).html(elencoSelect);
            $(tipo).attr('disabled', false);
        },
        error: (error) => console.error(error)
    });
}