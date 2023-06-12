import {
    generalSelectors,
    datiAnagraficiSelectors
} from "./Selectors.js";


export function isValidSedeLegale({ dug, indirizzo, civico, provincia, comune, localita, cap, statoEstero, telefono, cellulare }) {
    return isValidIndirizzo(dug, indirizzo, civico, provincia, comune, localita, cap, statoEstero, telefono, cellulare);
}

export async function isValidDatiAnagrafici({ ragioneSociale, codiceFiscale, partitaIva, naturaGiuridica, email, pec }) {
    let isValid = true;
    const doesCodiceFiscaleExistParameters = {
        url: `/Registrazione/DoesCodiceFiscaleExist?codiceFiscale=${$(datiAnagraficiSelectors.codiceFiscale).val() }`,
        validationFieldName: datiAnagraficiSelectors.codiceFiscaleName,
        validationMessage: "Codice Fiscale gia' presente"
    }
    const doesPartitaIvaExistParameters = {
        url: `/Registrazione/DoesPartitaIvaExist?partitaIva=${$(datiAnagraficiSelectors.partitaIva).val() }`,
        validationFieldName: datiAnagraficiSelectors.partitaIvaName,
        validationMessage: "Partita Iva gia' presente"
    }

    if(!$(ragioneSociale).valid()) isValid = false;
    if(!$(codiceFiscale).valid()) isValid = false;
    if (await checkValueExistsAndValidate(doesCodiceFiscaleExistParameters)) isValid = false;
    if(!$(partitaIva).valid()) isValid = false;
    if (await checkValueExistsAndValidate(doesPartitaIvaExistParameters)) isValid = false;
    if(!$(naturaGiuridica).valid()) isValid = false;
    if(!$(email).valid()) isValid = false;
    if (!$(pec).valid()) isValid = false;

    return isValid;

    async function checkValueExistsAndValidate({ url, validationFieldName, validationMessage }) {
        try {
            let valueExists = await $.ajax({
                url: url,
                type: 'GET',
            });

            if (!valueExists) return false;

            let errorArray = {};
            errorArray[validationFieldName] = validationMessage;
            $(generalSelectors.formId).validate().showErrors(errorArray);

            return true;
        } catch (error) {
            console.error(error);
        }
    }
}

export function isValidCorrispondenza({ dug, indirizzo, civico, provincia, comune, localita, cap, statoEstero, telefono, cellulare }) {
    return isValidIndirizzo(dug, indirizzo, civico, provincia, comune, localita, cap, statoEstero, telefono, cellulare);
}

export function isValidRappresentanteLegale({cognome, nome, codiceFiscale, dataNascita, genere, provinciaDiNascita, comuneNascita, statoEsteroDiNascita, incarico, dataDecorrenzaIncarico, dug, indirizzo, civico, provincia, comune, localita, cap, statoEstero, telefono, cellulare, email, pec}){
    let indirizzoValidity = isValidIndirizzo(dug, indirizzo, civico, provincia, comune, localita, cap, statoEstero, telefono, cellulare);
    let isValidRemaningData = true;

    if(!$(cognome).valid()) isValidRemaningData = false;
    if(!$(nome).valid()) isValidRemaningData = false;
    if(!$(codiceFiscale).valid()) isValidRemaningData = false;
    if(!$(dataNascita).valid()) isValidRemaningData = false;
    if(!$(genere).valid()) isValidRemaningData = false;
    if(!$(provinciaDiNascita).valid()) isValidRemaningData = false;
    if(!$(comuneNascita).valid()) isValidRemaningData = false;
    if(!$(statoEsteroDiNascita).valid()) isValidRemaningData = false;
    if(!$(incarico).valid()) isValidRemaningData = false;
    if(!$(dataDecorrenzaIncarico).valid()) isValidRemaningData = false;
    if(!$(email).valid()) isValidRemaningData = false;
    if(!$(pec).valid()) isValidRemaningData = false;

    return indirizzoValidity && isValidRemaningData === true;
}

export function isValidTipoAttivita({ categoria, tipo, codiceStatistico }){
    let isValid = true;
    
    if(!$(categoria).valid()) isValid = false;
    if(!$(tipo).valid()) isValid = false;
    if(!$(codiceStatistico).valid()) isValid = false;
    
    return isValid;
}

function isValidIndirizzo(dug, indirizzo, civico, provincia, comune, localita, cap, statoEstero, telefono, cellulare) {
    let isValid = true;

    if(!$(dug).valid()) isValid = false;
    if(!$(indirizzo).valid()) isValid = false;
    if(!$(civico).valid()) isValid = false;
    if(!$(provincia).valid()) isValid = false;
    if(!$(comune).valid()) isValid = false;
    if(!$(localita).valid()) isValid = false;
    if(!$(cap).valid()) isValid = false;
    if(!$(statoEstero).valid()) isValid = false;
    if(!$(telefono).valid()) isValid = false;
    if(!$(cellulare).valid()) isValid = false;

    return isValid;
}

export function isValidDocumenti({CertificatoIscrizioneCCIAA, StatutoAttoCostitutivo, DocumentoLegaleRappresentante, PartitaIva, DM80, DelegaConsulente, InformativaPrivacy}){
    let isValid = true;
    
    if(!$(CertificatoIscrizioneCCIAA).valid()) isValid = false;
    if(!$(StatutoAttoCostitutivo).valid()) isValid = false;
    if(!$(DocumentoLegaleRappresentante).valid()) isValid = false;
    if(!$(PartitaIva).valid()) isValid = false;
    if(!$(DM80).valid()) isValid = false;
    if(!$(DelegaConsulente).valid()) isValid = false;
    if(!$(InformativaPrivacy).valid()) isValid = false;
    
    return isValid;
}