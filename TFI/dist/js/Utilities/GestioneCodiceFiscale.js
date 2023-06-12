export function compilaFromCodFis(
    { cognome, nome, codiceFiscale, dataNascita, genere, provinciaDiNascita, comuneNascita, statoEsteroDiNascita, 
     showDataNascita, showGeneri, showProvincieNascita, showComuniNascita, showStatiEsteriNascita, validatoreCodiceFiscale }) {

    const cognomeInserito = $(cognome).val().replace(/\s+/g, '').trim().toUpperCase();
    const nomeInserito = $(nome).val().replace(/\s+/g, '').trim().toUpperCase();
    const codFisInserito = $(codiceFiscale).val().toUpperCase().trim();

    
    let isCodiFiscValid = controlloRequisiticodFis(codFisInserito, cognomeInserito, nomeInserito, codFisInserito.slice(0,6));
    if(!isCodiFiscValid){
        $(validatoreCodiceFiscale).html('<span id="'+ validatoreCodiceFiscale + '-error" class="">Nome e Cognome non equivalgono al codice fiscale</span>')
        return;
    }
    
    if (isCfModified(codFisInserito))
        svuotaCampiCodFis();
    
    compilaCampiFrom(codFisInserito);

    function isCfModified(codFisInserito) {
        return (codFisInserito);
    }

    function svuotaCampiCodFis() {

        $(cognome).val('');
        $(nome).val('');
        $(showDataNascita).val('');
        $(showGeneri).val('');
        $(showProvincieNascita).val('');
        $(showComuniNascita).val('');
        $(showStatiEsteriNascita).val('');

        $(nome).val('');
        $(cognome).val('');
        $(dataNascita).val('');
        $(genere).val('');
        $(provinciaDiNascita).val('');
        $(comuneNascita).val('');
        $(statoEsteroDiNascita).val('');
        $(validatoreCodiceFiscale).html('')
    }

    function compilaCampiFrom(codFisInserito) {
        const annoNascita = getAnnoNascitaFrom(codFisInserito[6] + codFisInserito[7]);
        const meseNascita = getMeseNascitaFrom(codFisInserito[8]);
        const giornoNascita = getGiornoNascitaFrom(codFisInserito[9] + codFisInserito[10]);
        const genereFromCodFis = getGenereFrom(codFisInserito);

        if (isCittadinoEstero(codFisInserito[11])) {
            compilaCittadinoStraniero(codFisInserito);
            return;

            function compilaCittadinoStraniero(codFisInserito) {
                const codiceStatoEstero = codFisInserito[11] + codFisInserito[12] + codFisInserito[13] + codFisInserito[14];
                compilaStatoEstero(codiceStatoEstero);
                completaCompilazioneCampiFromCodFis(annoNascita, meseNascita, giornoNascita, genereFromCodFis);

                function compilaStatoEstero(codiceStatoEstero) {
                    $(provinciaDiNascita).val('');
                    $(comuneNascita).val('');
                    $(statoEsteroDiNascita).val(codiceStatoEstero);

                    $(showProvincieNascita).val('Seleziona provincia');
                    $(showComuniNascita).val('');
                    $(showStatiEsteriNascita).val(codiceStatoEstero);
                }
            }
        }

        getProvinciaAndCompilaCampiCittadinoItalianoFrom(codFisInserito);

        function getProvinciaAndCompilaCampiCittadinoItalianoFrom(codFisInserito) {
            const codiceComune = codFisInserito[11] + codFisInserito[12] + codFisInserito[13] + codFisInserito[14];
            getProvinciaFromAjaxAndCompilaFrom(codiceComune)

            function getProvinciaFromAjaxAndCompilaFrom(codiceComune) {
                const uriGetProvincia = '/Registrazione/GetDenominazioneComuneAndSiglaProvincia';
                let denominazioneComuneFromCodiceComune;
                let siglaProvinciaFromCodiceComune;

                $.ajax({
                    cache: false,
                    url: uriGetProvincia,
                    type: 'GET',
                    data: { codiceComune: codiceComune },
                    success: function (result) {
                        denominazioneComuneFromCodiceComune = result.Denominazione;
                        siglaProvinciaFromCodiceComune = result.SiglaProvincia;
                    },
                    error: function (error) { console.error(error); },
                    complete: function () {
                        compilaCampiComuneProvinciaWith(codiceComune, siglaProvinciaFromCodiceComune, denominazioneComuneFromCodiceComune);
                        completaCompilazioneCampiFromCodFis(annoNascita, meseNascita, giornoNascita);
                    }
                });

                function compilaCampiComuneProvinciaWith(codiceComune, siglaProvinciaFromCodiceComune, denominazioneComuneFromCodiceComune) {
                    $(provinciaDiNascita).val(siglaProvinciaFromCodiceComune);
                    $(comuneNascita).val(codiceComune);
                    $(statoEsteroDiNascita).val('');

                    $(showProvincieNascita).val(siglaProvinciaFromCodiceComune);
                    $(showComuniNascita).html('<option value="' + codiceComune + '">' + denominazioneComuneFromCodiceComune + '</option>');
                    $(showStatiEsteriNascita).val('Seleziona stato estero');
                }
            }
        }

        function getAnnoNascitaFrom(numero) {
            let annoNascita = '';
            if (numero <= 10)
                annoNascita += '20' + numero;
            else
                annoNascita += '19' + numero;
            return annoNascita;
        }

        function getMeseNascitaFrom(lettera) {
            switch (lettera) {
                case 'A': return '01';
                case 'B': return '02';
                case 'C': return '03';
                case 'D': return '04';
                case 'E': return '05';
                case 'H': return '06';
                case 'L': return '07';
                case 'M': return '08';
                case 'P': return '09';
                case 'R': return '10';
                case 'S': return '11';
                case 'T': return '12';
                default: return '';
            }
        }

        function getGiornoNascitaFrom(numero) {
            let giornoNascita = parseInt(numero);
            if (giornoNascita >= 40)
                giornoNascita -= 40;
            if (giornoNascita < 10)
                giornoNascita = '0' + giornoNascita;
            return giornoNascita;
        }

        function getGenereFrom(codFisInserito) {
            return parseInt(codFisInserito[9] + codFisInserito[10]) >= 40 ? 2 : 1;
        }

        function isCittadinoEstero(lettera) {
            return lettera == 'Z';
        }

        function completaCompilazioneCampiFromCodFis(annoNascita, meseNascita, giornoNascita) {
            $(cognome).val(cognomeInserito);
            $(nome).val(nomeInserito);
            $(codiceFiscale).val(codFisInserito);
            $(dataNascita).val(annoNascita + '-' + meseNascita + '-' + giornoNascita);
            $(genere).val(genereFromCodFis);
            $(cognome).val(cognomeInserito);
            $(nome).val(nomeInserito);
            $(showDataNascita).val(annoNascita + '-' + meseNascita + '-' + giornoNascita);
            $(showGeneri).val(genereFromCodFis);
        }
    }
}

export function controlloRequisiticodFis(codFisInserito, cognomeInserito, nomeInserito, nomeFromCodFis) {
    return threeThingsAllTruthy(codFisInserito, cognomeInserito, nomeInserito)
        && checkCongruenzaNomeCognomeConCodFis(nomeFromCodFis, nomeInserito, cognomeInserito) && isUltimaLetteraCfValid(codFisInserito);
    function threeThingsAllTruthy(one, two, three) {
        return one && two && three;
    }
    function checkCongruenzaNomeCognomeConCodFis(nomeFromCodFis, nome, cognome) {
        const cognomeDecostruito = getConsonantiAndVocaliFrom(cognome);
        const nomeDecostruito = getConsonantiAndVocaliFrom(nome);
        return sonoCongruentiNomeCognomeConCodFis(
            nomeFromCodFis, nomeDecostruito.consonanti, cognomeDecostruito.consonanti, nomeDecostruito.vocali, cognomeDecostruito.vocali);
        function getConsonantiAndVocaliFrom(parola) {
            let isConsonante = true;
            const vocali = ['A', 'E', 'I', 'O', 'U', 'Y'];
            let consonantiOfParola = [];
            let vocaliOfParola = [];
            for (let lettera of parola) {
                for (let vocale of vocali) {
                    if (lettera == vocale) {
                        isConsonante = false;
                        vocaliOfParola.push(lettera);
                        break;
                    }
                }
                if (isConsonante)
                    consonantiOfParola.push(lettera);
                isConsonante = true;
            }
            return {
                consonanti: consonantiOfParola,
                vocali: vocaliOfParola
            };
        }
        function sonoCongruentiNomeCognomeConCodFis(nomeFromCodFis, consonantiOfNome, consonantiOfCognome, vocaliOfNome, vocaliOfCognome) {
            let isTuttoCongruente = false;
            let isCognomeCongruo = false;
            if (consonantiOfCognome.length >= 3) {
                if (nomeFromCodFis[0] == consonantiOfCognome[0] && nomeFromCodFis[1] == consonantiOfCognome[1] && nomeFromCodFis[2] == consonantiOfCognome[2])
                    isCognomeCongruo = true;
            }
            else if (consonantiOfCognome.length == 2) {
                if (nomeFromCodFis[0] == consonantiOfCognome[0] && nomeFromCodFis[1] == consonantiOfCognome[1] && nomeFromCodFis[2] == vocaliOfCognome[0])
                    isCognomeCongruo = true;
            }
            else if (consonantiOfCognome.length == 1) {
                if(vocaliOfCognome.length < 2)
                    if (nomeFromCodFis[0] == consonantiOfCognome[0] && nomeFromCodFis[1] == vocaliOfCognome[0] && nomeFromCodFis[2] == 'X')
                        isCognomeCongruo = true;
                else 
                    if (nomeFromCodFis[0] == consonantiOfCognome[0] && nomeFromCodFis[1] == vocaliOfCognome[0] && nomeFromCodFis[2] == vocaliOfCognome[1])
                        isCognomeCongruo = true;
            }
            let isNomeCongruo = false;
            if (consonantiOfNome.length >= 4) {
                if (nomeFromCodFis[3] == consonantiOfNome[0] && nomeFromCodFis[4] == consonantiOfNome[2] && nomeFromCodFis[5] == consonantiOfNome[3]) {
                    isNomeCongruo = true;
                }
            }
            else if (consonantiOfNome.length == 3) {
                if (nomeFromCodFis[3] == consonantiOfNome[0] && nomeFromCodFis[4] == consonantiOfNome[1] && nomeFromCodFis[5] == consonantiOfNome[2]) {
                    isNomeCongruo = true;
                }
            }
            else if (consonantiOfNome.length == 2) {
                if (nomeFromCodFis[3] == consonantiOfNome[0] && nomeFromCodFis[4] == consonantiOfNome[1] && nomeFromCodFis[5] == vocaliOfNome[0]) {
                    isNomeCongruo = true;
                }
            }
            else if (consonantiOfNome.length == 1) {
                if(vocaliOfNome.length < 2)
                    if (nomeFromCodFis[3] == consonantiOfNome[0] && nomeFromCodFis[4] == vocaliOfNome[0] && nomeFromCodFis[5] == 'X')
                        isNomeCongruo = true;   
                else 
                    if (nomeFromCodFis[3] == consonantiOfNome[0] && nomeFromCodFis[4] == vocaliOfNome[0] && nomeFromCodFis[5] == vocaliOfNome[1])
                        isNomeCongruo = true;
            }
            if (isCognomeCongruo && isNomeCongruo)
                isTuttoCongruente = true;
            return isTuttoCongruente;
        }
    }
    function isUltimaLetteraCfValid(codFis) {
        const codFisNoUltimaLettera = codFis.slice(0, 15);
        const ultimaLetteraCfInserito = codFis[15];
        const ultimaLetteraCalcolata = calcoloUltimaLetteraCf(codFisNoUltimaLettera);
        return ultimaLetteraCalcolata == ultimaLetteraCfInserito;
        function calcoloUltimaLetteraCf(codFisNoUltimaLettera) {
            let sommaLettere = 0;
            let isEven = false;
            for (let lettera of codFisNoUltimaLettera) {
                isEven ? sommaLettere += convertLettera(lettera, isEven) : sommaLettere += convertLettera(lettera, isEven);
                isEven = !isEven;
            }
            return convertResto(sommaLettere % 26);
            function convertLettera(lettera, isEven) {
                let totaleNumeriLettere = caricaNumeriLettere();
                let evenValues = caricaEvenValues();
                let unevenValues = caricaUnevenValues();
                return convertLetteraToValore(lettera, isEven, totaleNumeriLettere, evenValues, unevenValues)
                function caricaNumeriLettere() {
                    return ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'];
                }
                function caricaEvenValues() {
                    let evenValues = [];
                    let altroContatore = 0;
                    for (let i = 0; i < 36; i++) {
                        if (i >= 10) {
                            evenValues.push(altroContatore)
                            altroContatore++;
                        }
                        else
                            evenValues.push(i);
                    }
                    return evenValues;
                }
                function caricaUnevenValues() {
                    return [1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 2, 4, 18, 20, 11, 3, 6, 8, 12, 14, 16, 10, 22, 25, 24, 23];
                }
                function convertLetteraToValore(lettera, isEven, totaleNumeriLettere, evenValues, unevenValues) {
                    let valore = 0;
                    for (let i = 0; i < totaleNumeriLettere.length; i++)
                        if (totaleNumeriLettere[i] == lettera) {
                            isEven ? valore = evenValues[i] : valore = unevenValues[i];
                            break;
                        }
                    return valore;
                }
            }
            function convertResto(resto) {
                let letteraDaNumero;
                let alfabeto = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'];
                for (let i = 0; i < 26; i++) {
                    if (resto == i) {
                        letteraDaNumero = alfabeto[i];
                        break;
                    }
                }
                return letteraDaNumero;
            }
        }
    }
}