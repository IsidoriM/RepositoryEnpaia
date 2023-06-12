import {
    comuneChangeFunction, localitaChangeFunction,
    provinciaChangeFunction
} from "../../Utilities/GestioneSelectProvinciaComuneLocalitaCapStatoEstero.js";
import {selectors} from "./Selectors.js";
import { convertToLowerCase } from '../../Utilities/InputConvertCase.js';

$(document).ready(() => {
    $(selectors.provincia).change(async e => await provinciaChangeFunction(e, selectors));
    $(selectors.comune).change(e => comuneChangeFunction(e, selectors));
    $(selectors.localita).change(e => localitaChangeFunction(e, selectors));
    $(selectors.email).on('input', () => convertToLowerCase(selectors.email));
    $(selectors.pec).on('input', () => convertToLowerCase(selectors.pec));
})