$.validator.addMethod("customcodicefiscale", function (codiceFiscale, element, params) {
    if(codiceFiscale == null || codiceFiscale == undefined || codiceFiscale === '') return true;
    if(codiceFiscale.trim().length == 0) return false;
    
    let hasNomeAndCognomeElement = element.attributes.getNamedItem("valida-nome-cognome");
    
    if(!basicRegexValidation(codiceFiscale)) return false;
    if(!isLastCharValid(codiceFiscale)) return false;
    
    if(hasNomeAndCognomeElement && hasNomeAndCognomeElement.value.toLowerCase() === 'true') {
        const vocali = "AEIOUY";
        let nomeId = getPropIdValueFrom('nome-id');
        if(nomeId === '') return false;
        let nome = $(`#${nomeId}`).val();
        if(!isNomeValid(codiceFiscale, nome)) return false;
        
        let cognomeId = getPropIdValueFrom('cognome-id');
        if (cognomeId === '') return false;
        let cognome = $(`#${cognomeId}`).val();
        if(!isCognomeValid(codiceFiscale, cognome)) return false;
        function getPropIdValueFrom(attributeName){
            let nameId = element.attributes.getNamedItem(attributeName);
            
            if(!nameId) {
                console.error(`No ${attributeName} attribute provided`);
                return '';
            }
            
            return nameId.value;
        }
        
        function isCognomeValid(codiceFiscale, cognome) {
            if (!cognome) return false;

            const consonantiCognome = getConsonanti(cognome);

            if (consonantiCognome.length >= 3)
                return isValidParolaBase(0, codiceFiscale, consonantiCognome[0], consonantiCognome[1], consonantiCognome[2]);

            const vocaliCognome = getVocali(cognome);
            if (!vocaliCognome.length) return false;

            if (consonantiCognome.length === 2)
                return isValidParolaBase(0, codiceFiscale, consonantiCognome[0], consonantiCognome[1], vocaliCognome[0]);
            if (consonantiCognome.length === 1 && vocaliCognome.length >= 2)
                return isValidParolaBase(0, codiceFiscale, consonantiCognome[0], vocaliCognome[0], vocaliCognome[1]);

            return isValidParolaBase(0, codiceFiscale, consonantiCognome[0], vocaliCognome[0], 'X');
        }

        function isNomeValid(codiceFiscale, nome) {
            if (!nome) return false;
            const consonantiNome = getConsonanti(nome);

            if(consonantiNome.length >=4)
                return isValidParolaBase(3, codiceFiscale, consonantiNome[0], consonantiNome[2], consonantiNome[3]);

            switch (consonantiNome.length) {
                case 3:
                    return isValidParolaBase(3, codiceFiscale, consonantiNome[0], consonantiNome[1], consonantiNome[2]);
                default:
                    const vocaliNome = getVocali(nome);
                    if (!vocaliNome.length) return false;

                    if (consonantiNome.length === 2)
                        return isValidParolaBase(3, codiceFiscale, consonantiNome[0], consonantiNome[1], vocaliNome[0]);
                    if (consonantiNome.length === 1 && vocaliNome.length >= 2)
                        return isValidParolaBase(3, codiceFiscale, consonantiNome[0], vocaliNome[0], vocaliNome[1]);

                    return isValidParolaBase(3, codiceFiscale, consonantiNome[0], vocaliNome[0], 'X');
            }
        }

        function getVocali(parola) {
            return Array.from(parola).filter(carattere => vocali.includes(carattere.toUpperCase()));
        }

        function getConsonanti(parola) {
            return Array.from(parola).filter(carattere => !vocali.includes(carattere.toUpperCase()));
        }

        function isValidParolaBase(startingPos, codiceFiscale, ...carattere) {
            const caratteriCodiceFiscale = codiceFiscale.substring(startingPos).split('');

            for (let i = 0; i < carattere.length; i++) {
                if (caratteriCodiceFiscale[i].toUpperCase() !== carattere[i].toUpperCase()) return false;
            }

            return true;
        }
    }
    
    return true;
    
    function basicRegexValidation(codiceFiscale) {
        const codiceFiscaleRegex = /^[A-Z]{6}\d{2}[A-Z]\d{2}[A-Z]\d{3}[A-Z]$/;
        return codiceFiscaleRegex.test(codiceFiscale);
    }
    
    function isLastCharValid(codiceFiscale) {
        if (codiceFiscale.length !== 16)
            return false;

        return calculateLastLetter() === codiceFiscale[15];

        function calculateLastLetter() {
            const numberAndAlphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            const evenValues = [0,1,2,3,4,5,6,7,8,9,0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25];
            const oddValues = [1,0,5,7,9,13,15,17,19,21,1,0,5,7,9,13,15,17,19,21,2,4,18,20,11,3,6,8,12,14,16,10,22,25,24,23];

            let sum = 0;
            for (let i = 0; i < codiceFiscale.length - 1; i++) {
                if (i % 2 === 0) {
                    sum += oddValues[numberAndAlphabet.indexOf(codiceFiscale[i])];
                    continue;
                }

                sum += evenValues[numberAndAlphabet.indexOf(codiceFiscale[i])];
            }

            return alphabet[sum % 26];
        }
    }
});

$.validator.unobtrusive.adapters.add("customcodicefiscale", [], function (options) {
    options.rules["customcodicefiscale"] = true;
    options.messages["customcodicefiscale"] = options.message;
});