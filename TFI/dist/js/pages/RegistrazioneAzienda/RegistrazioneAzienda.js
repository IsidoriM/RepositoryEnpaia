import {
    provinciaChangeFunction,
    comuneChangeFunction,
    localitaChangeFunction,
    statoEsteroChangeFunction,
    provinciaNascitaChanged,
    statoEsteroNascitaChanged,
    categoriaAttivitaChanged
} from '../../Utilities/GestioneSelectProvinciaComuneLocalitaCapStatoEstero.js';
import { convertToLowerCase } from '../../Utilities/InputConvertCase.js';

import {
    attivitaSelectors,
    corrispondenzaSelectors, datiAnagraficiSelectors, documentiSelectors, generalSelectors,
    rappresentanteLegaleSelectors,
    sedeLegaleSelectors
} from "./Selectors.js";

import {
    compilaFromCodFis
} from "../../Utilities/GestioneCodiceFiscale.js";
import {
    isValidCorrispondenza,
    isValidDatiAnagrafici,
    isValidRappresentanteLegale,
    isValidTipoAttivita,
    isValidSedeLegale,
    isValidDocumenti,
} from "./validationFunctions.js";

$.validator.setDefaults({ ignore: null });

$(document).ready(function () {

    setupFormNavigation();
    sedeLegaleSelectChangeSetup();
    sedeOperativaSelectChangeSetup();
    rappresentanteLegaleSelectChangeSetup();
    informazioniAttivitaChangeSetup();
    $(rappresentanteLegaleSelectors.estraiDaCodiceFiscaleButton).click(() => compilaFromCodFis(rappresentanteLegaleSelectors));
    
    $(corrispondenzaSelectors.copiaDaSedeLegaleButton).click(() => {
        $(corrispondenzaSelectors.dug).val($(sedeLegaleSelectors.dug).val());
        $(corrispondenzaSelectors.indirizzo).val($(sedeLegaleSelectors.indirizzo).val());
        $(corrispondenzaSelectors.civico).val($(sedeLegaleSelectors.civico).val());
        setIndirizzoValues();
        $(corrispondenzaSelectors.statoEstero).val($(sedeLegaleSelectors.statoEstero).val());
        $(corrispondenzaSelectors.statoEstero).valid();
        $(corrispondenzaSelectors.telefono).val($(sedeLegaleSelectors.telefono).val());
        $(corrispondenzaSelectors.cellulare).val($(sedeLegaleSelectors.cellulare).val());
        function setIndirizzoValues(){
            let observeOptions = {
                childList:  true,
            };
            $(corrispondenzaSelectors.provincia).val($(sedeLegaleSelectors.provincia).val());
            $(corrispondenzaSelectors.provincia).change();
            
            let comuneObserver = createMutationObserver(corrispondenzaSelectors.comune, sedeLegaleSelectors.comune);
            let localitaObserver = createMutationObserver(corrispondenzaSelectors.localita, sedeLegaleSelectors.localita);

            comuneObserver.observe($(corrispondenzaSelectors.comune).get(0), observeOptions);
            localitaObserver.observe($(corrispondenzaSelectors.localita).get(0), observeOptions);
            
            $(corrispondenzaSelectors.cap).val($(sedeLegaleSelectors.cap).val());
            function createMutationObserver(destinationSelector, originSelector){
                return new MutationObserver((mutations) =>{
                    $(destinationSelector).val($(originSelector).val());
                    $(destinationSelector).valid();
                    $(destinationSelector).change();
                });   
            }
        }
    });

    convertEmailAndPecToLowerCase();
    
    function sedeLegaleSelectChangeSetup() {
        $(sedeLegaleSelectors.provincia).change(async e => await provinciaChangeFunction(e, sedeLegaleSelectors));
        $(sedeLegaleSelectors.comune).change(e => comuneChangeFunction(e, sedeLegaleSelectors));
        $(sedeLegaleSelectors.localita).change(e => localitaChangeFunction(e, sedeLegaleSelectors));
        $(sedeLegaleSelectors.statoEstero).change(e => statoEsteroChangeFunction(e, sedeLegaleSelectors));
    }

    function sedeOperativaSelectChangeSetup() {
        $(corrispondenzaSelectors.provincia).change(async e => await provinciaChangeFunction(e, corrispondenzaSelectors));
        $(corrispondenzaSelectors.comune).change(e => comuneChangeFunction(e, corrispondenzaSelectors));
        $(corrispondenzaSelectors.localita).change(e => localitaChangeFunction(e, corrispondenzaSelectors));
        $(corrispondenzaSelectors.statoEstero).change(e => statoEsteroChangeFunction(e, corrispondenzaSelectors));
    }

    function rappresentanteLegaleSelectChangeSetup() {
        $(rappresentanteLegaleSelectors.provincia).change(async (e) => await provinciaChangeFunction(e, rappresentanteLegaleSelectors));
        $(rappresentanteLegaleSelectors.comune).change(e => comuneChangeFunction(e, rappresentanteLegaleSelectors));
        $(rappresentanteLegaleSelectors.localita).change(e => localitaChangeFunction(e, rappresentanteLegaleSelectors));
        $(rappresentanteLegaleSelectors.statoEstero).change(e => statoEsteroChangeFunction(e, rappresentanteLegaleSelectors));
        $(rappresentanteLegaleSelectors.provinciaDiNascita).change(e => provinciaNascitaChanged(e, rappresentanteLegaleSelectors));
        $(rappresentanteLegaleSelectors.statoEsteroDiNascita).change(() => statoEsteroNascitaChanged(rappresentanteLegaleSelectors));
    }

    function informazioniAttivitaChangeSetup() {
        $(attivitaSelectors.categoria).change((e) => categoriaAttivitaChanged(e, attivitaSelectors));
    }

    function setupFormNavigation() {
        $(datiAnagraficiSelectors.nextButton).click(async (e) => {
            e.preventDefault();
            if (await isValidDatiAnagrafici(datiAnagraficiSelectors)) goToTabOf(sedeLegaleSelectors);
        });        
        
        setupFullNavigationWithValidation(sedeLegaleSelectors, datiAnagraficiSelectors, corrispondenzaSelectors, isValidSedeLegale);
        setupFullNavigationWithValidation(corrispondenzaSelectors, sedeLegaleSelectors, rappresentanteLegaleSelectors, isValidCorrispondenza);
        setupFullNavigationWithValidation(rappresentanteLegaleSelectors, corrispondenzaSelectors, attivitaSelectors, isValidRappresentanteLegale);
        setupFullNavigationWithValidation(attivitaSelectors, rappresentanteLegaleSelectors, documentiSelectors, isValidTipoAttivita);
        
        $(documentiSelectors.backButton).click(() => goToTabOf(attivitaSelectors));
        $(documentiSelectors.nextButton).click((e) => {
            e.preventDefault();
            if(isValidDocumenti(documentiSelectors)) $(generalSelectors.formId).submit();
        });
        
        function setupFullNavigationWithValidation(currentTab, previousTab, nextTabSelector, validationFunction) {
            $(currentTab.nextButton).click((e) => {
                e.preventDefault();
                if(validationFunction(currentTab)) goToTabOf(nextTabSelector)
            });
            $(currentTab.backButton).click(() => goToTabOf(previousTab));
        }

        function goToTabOf({tabId}) {
            $(tabId).tab('show');
        }
    }
    
    function convertEmailAndPecToLowerCase(){
        $(datiAnagraficiSelectors.email).on('input', () => convertToLowerCase(datiAnagraficiSelectors.email));
        $(datiAnagraficiSelectors.pec).on('input', () => convertToLowerCase(datiAnagraficiSelectors.pec));
        $(rappresentanteLegaleSelectors.email).on('input', () => convertToLowerCase(rappresentanteLegaleSelectors.email));
        $(rappresentanteLegaleSelectors.pec).on('input', () => convertToLowerCase(rappresentanteLegaleSelectors.pec))
    }
});

