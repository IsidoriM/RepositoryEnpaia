import { datiAnagraficiSelectors, aziendaUtilizzatriceSelectors } from '../Azienda/Selectors/SchedaIscrittoSelectors.js';

var codPos = $('#codPosAzienda').val();
var checkboxes = document.getElementById("mesi").querySelectorAll('input[type="checkbox"]');
var listaAssConContratti = [];
var emolumentiCompleti = 0;

var data = new Date();
var gg, mm, aaaa;
gg = data.getDate() + "/";
mm = data.getMonth() + 1 + "/";
aaaa = data.getFullYear();
document.getElementById("datDecMod").value = gg + mm + aaaa;

$(document).ready(() => {
    const contrattoCod = $("#contratto").val();
    const contrattoDen = $("#contratto").text();
    const codLivello = $('#livello').val();
    const tipoRapporto = $("#tipRap").find('option:selected').val();
    const numeroMesi = $("#numMes").val();

    setClickEvents();
    setChangeEvents();
    setKeyUpEvents();
    setInputFilters();

    fillDurataMesi();
    getAndFillContrattiWith(contrattoCod);
    getAndFillFromLivello(contrattoCod, contrattoDen, codLivello);
    tipRapOptionSelection(tipoRapporto);
    setNumMes(numeroMesi);
    setNumMesEmolumenti(numeroMesi);
    setCampoCo();
    setAllInputTextFieldsToUpper();

    function fillDurataMesi() {
        let primaData = new Date($("#datIsc").val());
        let secondaData = new Date($("#alal").val());
        if (primaData.getDate() == 1) {
            primaData.setDate(2);
            secondaData.setDate(secondaData.getDate() + 1);
        }

        let difference = monthDiff(primaData, secondaData);
        $("#periodo").val(difference * 3);

        function monthDiff(d1, d2) {
            let months;
            months = (d2.getYear() - d1.getYear()) * 12;
            months -= d1.getMonth();
            months += d2.getMonth();
            return months <= 0 ? 0 : months;
        }
    }    

    function setCampoCo() {
        const co = $('#co').val();
        if (co != 'Null' || co != 'NULL')
            return;
        $("#co").val();
    }
});

function setClickEvents() {

    $('#next1').click(() => {
        let checkFormValidity = document.getElementById('cognome').reportValidity()
            && document.getElementById('nome').reportValidity()
            && document.getElementById('codFis').reportValidity()
            && document.getElementById('dataNas').reportValidity()
            && document.getElementById('sesso').reportValidity()
            && document.getElementById('comuneN').reportValidity()
            && document.getElementById('provinciaN').reportValidity()
            && document.getElementById('statoEsN').reportValidity()
            && document.getElementById('titoloStudio').reportValidity()
            && document.getElementById('indirizzo').reportValidity()
            && document.getElementById('residenza').reportValidity()
            && document.getElementById('civico').reportValidity()
            && document.getElementById('provincia').reportValidity()
            && document.getElementById('comune').reportValidity()
            && document.getElementById('localita').reportValidity()
            && document.getElementById('cap').reportValidity()
            && document.getElementById('statoEs').reportValidity()
            && document.getElementById('email').reportValidity()
            && document.getElementById('cell').reportValidity()
            && document.getElementById('tel').reportValidity()
            && document.getElementById('pec').reportValidity();

        if (checkFormValidity)
            $('#custom-tabs-one-profile-tab').tab('show');
    });

    $('#back2').click(() => $('#custom-tabs-one-home-tab').tab('show'));

    $('#next2').click(() => {
        let checkFormValidity = document.getElementById('datIsc').reportValidity()
            && document.getElementById('datDecMod').reportValidity()
            && document.getElementById('tipRap').reportValidity()
            && document.getElementById('periodo').reportValidity()
            && document.getElementById('datTerm').reportValidity()
            && document.getElementById('daal').reportValidity()
            && document.getElementById('daal2').reportValidity()
            && document.getElementById('daal3').reportValidity()
            && document.getElementById('alal').reportValidity()
            && document.getElementById('alal2').reportValidity()
            && document.getElementById('alal3').reportValidity()
            && document.getElementById('perce1').reportValidity()
            && document.getElementById('perce2').reportValidity()
            && document.getElementById('perce3').reportValidity()
            && document.getElementById('PercPT').reportValidity()
            && checkValiditaMesi()
            && document.getElementById('contratto').reportValidity()
            && document.getElementById('livello').reportValidity()
            && document.getElementById('qualifica').reportValidity()
            && document.getElementById('numMes').reportValidity()
            && document.getElementById('aliquota').reportValidity()
            && document.getElementById('aliqCont').reportValidity();
        if (checkFormValidity)
            $('#custom-tabs-one-messages-tab').tab('show');
    });

    $('#back3').click(() => $('#custom-tabs-one-profile-tab').tab('show'));

    $('#next3').click(() => $('#custom-tabs-one-settings-tab').tab('show'));

    $('#back4').click(() => $('#custom-tabs-one-messages-tab').tab('show'));

    $('#blocco').click(() => ajaxSubmit());
}

function setChangeEvents() {

    $(datiAnagraficiSelectors.provincia).change(async (e) => provinciaChangeFunctionWithStatoEsteroRdl(e, datiAnagraficiSelectors));

    $(datiAnagraficiSelectors.comune).change(async (e) => comuneChangeFunctionRdl(e, datiAnagraficiSelectors));

    $(datiAnagraficiSelectors.localita).change((e) => localitaChangeFunction(e, datiAnagraficiSelectors));

    $(datiAnagraficiSelectors.statoEstero).change((e) => statoEsteroChangeFunctionRdl(e, datiAnagraficiSelectors));

    $(aziendaUtilizzatriceSelectors.provincia).change(async (e) => provinciaChangeFunctionRdl(e, aziendaUtilizzatriceSelectors));

    $(aziendaUtilizzatriceSelectors.comune).change(async (e) => comuneChangeFunctionRdl(e, aziendaUtilizzatriceSelectors));

    $(aziendaUtilizzatriceSelectors.localita).change((e) => localitaChangeFunction(e, aziendaUtilizzatriceSelectors));

    $("#mesi input[type='checkbox']").change(() => checkRequiredToMesi());

    $("#datDecMod").change(() => {
        let dencon = $('#contratto').find('option:selected').text();
        let codliv = $('#livello').val();

        if (!dencon || !codliv)
            return;

        getEmolumentiFromLivelloReady(codliv, dencon);
    })

    $("#tipRap").change(({ target }) => {
        removeAllRequiredFromTipoContratto();
        cleanFormField();
        const selectedOption = $(target).find('option:selected');
        const tipoRapporto = selectedOption.val();
        tipRapOptionSelection(tipoRapporto);
        $("#emolumenti").val(emolumentiCompleti);
        calcolaImportoTotale();
    });

    $("#periodo").change(({ target }) => {
        if (!target) {
            $('#alal').val('');
            $('#daal2').val('');
            $('#alal2').val('');
            $('#daal3').val('');
            $('#alal3').val('');
            $('#perce1').val('');
            $('#perce2').val('');
            $('#perce3').val('');
            return;
        }
        if ($("#datIsc").val() == "") return;
        setDaalPerFields();
    });

    $('#contratto').change(({ target }) => {
        let $option = $(target).find('option:selected');
        let dencon = $option.text();
        let codcon = $option.val();

        if (!codcon || !dencon) {
            resetFields();
            return;
        }


        $.ajax({
            url: '/AziendaConsulente/DdlLivello',
            data: { 'codcon': codcon, 'dencon': dencon },
            success: function (datal) {
                let s = '<option value=""></option>';
                for (let i = 0; i < datal.length; i++)
                    s += '<option value="' + datal[i].codliv + '">' + datal[i].denliv + '</option>';

                $("#denconnew").val(dencon);
                $("#livello").html(s);
            },
            error: function () {
                alert('Errore');
            },
            complete: function () {
                Mensilita(dencon);
                Aliquote(dencon, codcon);
                compilaAssConFrom(codcon);
                checkFap(codPos, dencon);
            },
        });

        function resetFields() {
            $('#livello').html('');
            $('#qualifica').html('');
            $('#numMes').val('');
            $("#m14").val('');
            $('#m14').prop('readonly', true)
            $("#m15").val('');
            $('#m15').prop('readonly', true)
            $("#m16").val('');
            $('#m16').prop('readonly', true)
            $("#fap").val(false);
            $("#fap").attr('checked', false);
            $("#aliqCont").html('');
            $("#aliquota").val('');
            $("#assistenzaCon").val(false);
            $("#assistenzaCon").attr('checked', false);
        }
    });

    $('#livello').change(() => $("#denLiv").val($("#livello option:selected").text().trim()));

    $(document).change(() => calcolaImportoTotale());

    $("#scattiAnz").change(() => getLivelloSetImportoTotale());

    $("#aliqCont").change(({ target }) => {

        let $option = $(target).find('option:selected');
        let CODGRUASS = $option.val();
        let dencon = $('#contratto').find('option:selected').text();
        let dataNas = $('#dataNas').val();
        let datIsc = $('#datIsc').val();
        let datMod = $('#datDecMod').val();
        let fap = $('#fap').is('checked');

        $.ajax({
            url: '/AziendaConsulente/ValAliquota',
            data: { 'CODGRUASS': CODGRUASS, 'dencon': dencon, 'datIsc': datIsc, 'datMod': datMod, 'dataNas': dataNas, 'fap': fap },
            success: function (data) {
                $("#aliquota").val(data);
            },
            error: function () {
                alert('Errore');
            },
            complete: function () { },
        })
    });

    $("#tipRap").change(({ target }) => {
        let $option = $(target).find('option:selected');
        let tiprap = $option.val();
        $('#tiprapval').val(tiprap);
    });

    $("#livello").change(({ target }) => {
        let $option = $(target).find('option:selected');
        let codliv = $option.val();
        let dencon = $('#contratto').find('option:selected').html();
        let datMod = $('#datDecMod').val();

        if (!codliv || !dencon || !datMod)
            return;

        $.ajax({
            url: '/AziendaConsulente/Emolumenti',
            data: { 'codliv': codliv, 'dencon': dencon, 'datMod': datMod },
            success: function (data) {
                let percPt = parseFloat($('#PercPT').val());
                let emolumenti = parseFloat(data.split(" ")[0].replace(".", "").replace(",", "."));
                emolumentiCompleti = emolumenti.toFixed(2);
                if (percPt) {
                    emolumenti = emolumenti * percPt / 100;
                }
                $("#emolumenti").val(emolumenti.toFixed(2));
                $('#codlival').val(codliv);
            },
            error: function () {
                alert('Errore');
            },
            complete: function () {
                calcolaImportoTotale();
            },
        })
    });

    $("#PercPT").change(() => {
        let percPt = $("#PercPT").val();
        let emolumenti = parseFloat(emolumentiCompleti);
        if (percPt) {
            emolumenti = emolumenti * percPt / 100;
        }
        $("#emolumenti").val(emolumenti.toFixed(2));
    });
}

function setKeyUpEvents() {

    $("#numMes").on('keyup', ({ target }) => {
        setNumMes($(target).val());
        setNumMesEmolumenti($(target).val());
    });

    $("#piAZ").on('keyup', () => {
        let piva = $('#piAZ').val();
        $.ajax({
            url: '/Amministrativo/CercaAziendaConPiva',
            data: { 'piva': piva },
            success: function (data) {
                $("#ragsocAZ").val(data.ragsocAz);
                $("#cfAz").val(data.codfisAz);
                $("#pozioneAZ").val(data.codpos);
                $("#LocalitaS").val(data.localitaAz);
                $("#LocalitaS").html(`<option value="${data.localitaAz}" selected)>${data.localitaAz}</option>`);
                $("#DUG").val(data.dugCodAz);
                $("#indirizzoAZ").val(data.indirizzoAz);
                $("#numCivAZ").val(data.civicoAz);
                $("#ProvinciaS").val(data.provAz);
                $("#CAPS").val(data.capAz);
                $("#telefonoAZ").val(data.telefonoAz);
                $("#ComuneS").val(data.comuneCodAz);
                $("#ComuneS").html(`<option value="${data.comuneCodAz}" selected)>${data.comuneAz}</option>`);
            },
            error: function () {
                $("#ragsocAZ").val('');
                $("#cfAz").val('');
                $("#pozioneAZ").val('');
                $("#LocalitaS").val('');
                $("#LocalitaS").html('');
                $("#DUG").val('');
                $("#indirizzoAZ").val('');
                $("#numCivAZ").val('');
                $("#ProvinciaS").val('');
                $("#CAPS").val('');
                $("#telefonoAZ").val('');
                $("#ComuneS").val('');
                $("#ComuneS").html('');
            },
            complete: function () { }
        })
    });
}

function setInputFilters() {

    setInputFilter(document.getElementById("S12"), (value) => {
        return /^-?\d*[,]?\d{0,2}$/.test(value);
    });

    setInputFilter(document.getElementById("S13"), (value) => {
        return /^-?\d*[,]?\d{0,2}$/.test(value);
    });

    setInputFilter(document.getElementById("S14"), (value) => {
        return /^-?\d*[,]?\d{0,2}$/.test(value);
    });

}

async function getAndFillContrattiWith(contrattoCod) {
    const datIni = $('#datIsc').val();
    const codLoc = $("#codloc").val();
    const contratto = $("#contratto").text().trim();

    const selectContrattiOptions = await getOptionValuesForContratti();
    $("#contratto").html(selectContrattiOptions);

    let contrattoDaSelezionare = $('#contratto').find('option:contains(' + contratto + ')')[0];
    contrattoDaSelezionare.selected = true;

    async function getOptionValuesForContratti() {
        let contratti = await getContrattiFrom(datIni);

        let optionValuesForContratti = '<option value=""></option>';
        contratti.forEach(contratto => addContrattoToOptionValuesAndToLista(contratto));

        return optionValuesForContratti;

        function addContrattoToOptionValuesAndToLista(contratto) {
            optionValuesForContratti += '<option id="contratto" value="' + contratto.CODCON;
            optionValuesForContratti += isContrattoSelected() ? '" selected>' : '">';
            optionValuesForContratti += contratto.DENCON + '</option>'
            optionValuesForContratti += '<option id="procon" value="' + contratto.PROCON + '"hidden></option>';
            optionValuesForContratti += '<option id="CODQUACON" value="' + contratto.CODQUACON + '"hidden></option>';

            listaAssConContratti.push(
                { 'CODCON': contratto.CODCON, 'ASSCON': contratto.ASSCON });

            function isContrattoSelected() {
                return contrattoCod == contratto.CODCON
                    && (codLoc == contratto.CODLOC || codLoc == 0);
            }
        }
    }
}

function getAndFillFromLivello(contrattoCod, contratto, livello) {
    const contrattoPulito = contratto.trim();
    $.ajax({
        url: '/AziendaConsulente/DdlLivello',
        data: { 'codcon': contrattoCod, 'dencon': contrattoPulito },
        success: function (datal) {
            let selectOptions = '<option value=""></option>';
            for (let i = 0; i < datal.length; i++)
                selectOptions += '<option value="' + datal[i].codliv + '">' + datal[i].denliv + '</option>';

            $("#denconnew").val(contrattoCod);
            $("#livello").html(selectOptions);
            $('#livello').val(livello);
        },
        error: function () {
            alert('Errore');
        },
        complete: function () {
            getEmolumentiFromLivelloReady(livello, contratto);
            aliquoteReady(contratto, contrattoCod);
            compilaAssConFrom(contrattoCod);
            checkFap(codPos, contratto);
        },
    })

    function aliquoteReady(contratto, contrattoCod) {
        let dataNas = $('#dataNas').val();
        let aliquota = $("#aliqCont").val();
        $.ajax({
            url: '/AziendaConsulente/DdlAliquota',
            data: { 'dataNas': dataNas, 'codcon': contrattoCod, 'dencon': contratto },
            success: function (datas) {
                let s = '';
                for (let i = 0; i < datas.length; i++)
                    s += '<option value="' + datas[i].CODGRUASS + '">' + datas[i].DENGRUASS + '</option>';
                $("#aliqCont").html(s);
                $("#aliqCont").val(aliquota);
            },
            error: function () {
                alert('Per determinare correttamente le aliquote inserire la data di nascita!');
            },
            complete: function () {
            },
        });
    }
}

function ajaxSubmit() {
    const proRapInt = parseInt($('#prorap').val());
    const codPosInt = parseInt(codPos);
    const updatedIscritto = mapFromFormToNuovoIscritto();
    let isSubmitSuccessful;

    $.ajax({
        url: '/AziendaConsulente/UpdateRapportoLavoro',
        type: 'POST',
        data: JSON.stringify({ 'codPos': codPosInt, 'proRap': proRapInt, 'updatedIscritto': updatedIscritto }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            isSubmitSuccessful = result.isRapportoUpdated;
            alert(result.resultMsg);
        },
        error: function () {
            alert('Spiamo spiacenti abbiamo problemi con il sistema al momento, riprovare più tardi');
        },
        complete: function () {
            if (isSubmitSuccessful)
                window.location.href = '/AziendaConsulente/RapportiLavoro';
        }
    });

    function mapFromFormToNuovoIscritto() {
        const serializedForm = $('#formSchedaIscritto').serializeArray();
        let iscrittoMappato = {};
        for (let i = 0; i < serializedForm.length; i++)
            iscrittoMappato[serializedForm[i].name] = serializedForm[i].value;

        const mesiSelezionatiNellaForm = $('.spazioCheckMesi input[type="checkbox"]:checked').serializeArray();
        if (!mesiSelezionatiNellaForm.length)
            return iscrittoMappato;

        const listaMesiSelezionati = mesiSelezionatiNellaForm.map(mese => mese.value);
        iscrittoMappato = { ...iscrittoMappato, meseSel: listaMesiSelezionati };

        return iscrittoMappato;
    }
}

function getEmolumentiFromLivelloReady(livello, contratto) {
    let datMod = $('#datDecMod').val();
    $.ajax({
        url: '/AziendaConsulente/Emolumenti',
        data: { 'codliv': livello, 'dencon': contratto, 'datMod': datMod },
        success: function (data) {
            let percPt = parseFloat($('#PercPT').val());
            let emolumenti = parseFloat(data.split(" ")[0].replace(".", "").replace(",", "."));
            if (percPt) {
                emolumenti = emolumenti * percPt / 100;
            }
            $("#emolumenti").val(emolumenti.toFixed(2));
            emolumentiCompleti = $("#emolumenti").val();
            $('#codlival').val(livello);
        },
        error: function () {
            alert('Errore');
        },
        complete: function () {
            calcolaImportoTotale();
        },
    })
}

function compilaAssConFrom(codcon) {
    for (let contratto of listaAssConContratti) {
        let codConFromListaContratti = contratto['CODCON'];
        if (codConFromListaContratti != codcon)
            continue;
        contratto['ASSCON'] == 'S' ? checkOrUncheck(true) : checkOrUncheck(false);
        break;
    }

    function checkOrUncheck(isAsscon) {
        if (isAsscon) {
            $("#assistenzaCon").val(isAsscon);
            $("#assistenzaCon").attr('checked', isAsscon);
            $("#assistenzaCon").attr('style', 'pointer-events: none;background: #E9ECEF;');
        }
        else {
            $("#assistenzaCon").val(isAsscon);
            $("#assistenzaCon").attr('checked', isAsscon);
            $("#assistenzaCon").attr('style', 'pointer-events: none;background: #E9ECEF;');
        }
    }
}

function setAllInputTextFieldsToUpper() {
    let allInputsTypeText = document.querySelectorAll("input[type=text]");
    allInputsTypeText.forEach(
        (element) => element.oninput = function () {
            this.value = this.value.toUpperCase();
        });
}

function checkFap(codPos, contrattoLavoro) {
    selectOrDeselectFap(false);
    if (!isCodPosValidForFap(codPos) || !isContrattoLavoroValidForFap(contrattoLavoro.toUpperCase()))
        return;
    selectOrDeselectFap(true);

    function selectOrDeselectFap(isFapChecked) {
        $("#fap").val(isFapChecked);
        $("#fap").attr('checked', isFapChecked);
    }

    function isCodPosValidForFap(codPos) {
        return codPos == '40833' || codPos == '43656';
    }

    function isContrattoLavoroValidForFap(contrattoLavoro) {
        return contrattoLavoro.includes('DIRIGENT');
    }
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
        $("#datTerm").attr("required", 'required');
    }

    //Part time
    if (valueSelected == 3) {
        $("#part_time").show();
        $("#data_termine").hide(); //????
        $("#da_al").hide();
        $("#da_al1").hide();
        $("#da_al2").hide();
        $("#mesi").hide();
        $("#periododll").hide();
        $("#PercPT").attr("required", 'required');
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
        $("#PercPT").attr("required", 'required');
        $("#datTerm").attr("required", 'required');
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

        $("#daal").attr("required", 'required');
        $("#daal2").attr("required", 'required');
        $("#daal3").attr("required", 'required');
        $("#alal").attr("required", 'required');
        $("#alal2").attr("required", 'required');
        $("#alal3").attr("required", 'required');
        $("#perce1").attr("required", 'required');
        $("#perce2").attr("required", 'required');
        $("#perce3").attr("required", 'required');
        $("#periodo").attr("required", 'required');
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

        $("#daal").attr("required", 'required');
        $("#daal2").attr("required", 'required');
        $("#daal3").attr("required", 'required');
        $("#alal").attr("required", 'required');
        $("#alal2").attr("required", 'required');
        $("#alal3").attr("required", 'required');
        $("#perce1").attr("required", 'required');
        $("#perce2").attr("required", 'required');
        $("#perce3").attr("required", 'required');
        $("#periodo").attr("required", 'required');
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

        $("#daal").attr("required", 'required');
        $("#daal2").attr("required", 'required');
        $("#daal3").attr("required", 'required');
        $("#alal").attr("required", 'required');
        $("#alal2").attr("required", 'required');
        $("#alal3").attr("required", 'required');
        $("#perce1").attr("required", 'required');
        $("#perce2").attr("required", 'required');
        $("#perce3").attr("required", 'required');
        $("#PercPT").attr("required", 'required');
        $("#periodo").attr("required", 'required');
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
        $("#datTerm").attr("required", 'required');
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

        $("#PercPT").attr("required", 'required');
        $("#daal").attr("required", 'required');
        $("#daal2").attr("required", 'required');
        $("#daal3").attr("required", 'required');
        $("#alal").attr("required", 'required');
        $("#alal2").attr("required", 'required');
        $("#alal3").attr("required", 'required');
        $("#perce1").attr("required", 'required');
        $("#perce2").attr("required", 'required');
        $("#perce3").attr("required", 'required');
        $("#periodo").attr("required", 'required');
    }

    if (valueSelected == 11) {
        $("#part_time").show();
        $("#mesi").show();
        $("#da_al").hide();
        $("#da_al1").hide();
        $("#da_al2").hide();
        $("#data_termine").hide();
        $("#periododll").hide();
        $("#PercPT").attr("required", 'required');
        setRequiredToMesi();
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
        $("#datTerm").attr("required", 'required');
    }

    if (valueSelected == 13) {
        $("#data_termine").show();
        $("#part_time").show();
        $("#da_al").hide();
        $("#da_al1").hide();
        $("#da_al2").hide();
        $("#mesi").hide();
        $("#periododll").hide();
        $("#datTerm").attr("required", 'required');
        $("#PercPT").attr("required", 'required');
    }

    if (valueSelected == 16) {
        $("#mesi").show();
        $("#data_termine").show();
        $("#da_al").hide();
        $("#da_al1").hide();
        $("#da_al2").hide();
        $("#part_time").hide();
        $("#periododll").hide();
        $("#datTerm").attr("required", 'required');
        setRequiredToMesi();
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
        setRequiredToMesi();
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
        $("#PercPT").attr("required", 'required');
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
        $("#PercPT").attr("required", 'required');
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

function Aliquote(dencon, codcon) {
    let dataNas = $('#dataNas').val();
    $.ajax({
        url: '/AziendaConsulente/DdlAliquota',
        data: { 'dataNas': dataNas, 'codcon': codcon, 'dencon': dencon },
        success: function (datas) {
            let s = '';
            for (let i = 0; i < datas.length; i++)
                s += '<option value="' + datas[i].CODGRUASS + '">' + datas[i].DENGRUASS + '</option>';
            $("#aliqCont").html(s);
        },
        error: function () {
            alert('Per determinare correttamente le aliquote inserire la data di nascita!');
        },
        complete: function () { },
    });
}

function Mensilita(dencon) {
    $.ajax({
        url: '/AziendaConsulente/Mensilita',
        data: { 'dencon': dencon },
        success: function (datam) {
            let s = '<option selected value="' + datam.qualifica + '">' + datam.qualifica + '</option>';
            $("#qualifica").html(s);
            $("#numMes").val(datam.mensilita);
            $("#m14").val(datam.m14);
            $("#m15").val(datam.m15);
            $("#m16").val(datam.m16);
            $("#tipspe").val(datam.tipspe);
            $("#codloc").val(datam.codloc);
            setNumMes(datam.mensilita);
            setNumMesEmolumenti(datam.mensilita);
        },
        error: function () { },
        complete: function () { },
    });
}

function setNumMes(valore) {
    if (valore == '14') {
        $("#m14").prop("readonly", false);
        $("#m15").prop("readonly", true);
        $("#m16").prop("readonly", true);
    } else if (valore == '15') {
        $("#m14").prop("readonly", false);
        $("#m15").prop("readonly", false);
        $("#m16").prop("readonly", true);
    } else if (valore == '16') {
        $("#m14").prop("readonly", false);
        $("#m15").prop("readonly", false);
        $("#m16").prop("readonly", false);
    }
    else {
        $("#m14").prop("readonly", true);
        $("#m15").prop("readonly", true);
        $("#m16").prop("readonly", true);
    }
}

function setNumMesEmolumenti(valore) {
    if (valore == '14') {
        $("#S14").prop("readonly", false);
        $("#S15").prop("readonly", true);
        $("#S16").prop("readonly", true);
    } else if (valore == '15') {
        $("#S14").prop("readonly", false);
        $("#S15").prop("readonly", false);
        $("#S16").prop("readonly", true);
    } else if (valore == '16') {
        $("#S14").prop("readonly", false);
        $("#S15").prop("readonly", false);
        $("#S16").prop("readonly", false);
    }
    else {
        $("#S14").prop("readonly", true);
        $("#S15").prop("readonly", true);
        $("#S16").prop("readonly", true);
    }
}

function getLivelloSetImportoTotale() {
    let codliv = $("#livello").val();
    let datIsc = $('#datIsc').val();
    let datMod = $('#datDecMod').val();
    let numScatti = $('#scattiAnz').val();
    let dencon = $('#contratto').find('option:selected').text();
    let prorap = null;
    let tiprap = $('#tiprapval').val();

    $.ajax({

        url: '/AziendaConsulente/SelLivello',
        data: { 'numScatti': numScatti, 'dencon': dencon, 'codliv': codliv, 'datMod': datMod, 'datIsc': datIsc, 'prorap': prorap, 'tiprap': tiprap },

        success: function (data) {
            $("#importoSc").val(data);
            calcolaImportoTotale();
        },
        error: function () {
            alert('Errore selezionare livello');
        },
        complete: function () {
        },
    })
}

//CONTROLLO INPUT SOLO NUMERI SEGUITI DA VIRGOLA CON 2 DECIMALI
function setInputFilter(textbox, inputFilter) {
    ["input", "keydown", "keyup", "mousedown", "mouseup", "select", "contextmenu", "drop"].forEach((event) => {
        textbox.addEventListener(event, function () {
            if (inputFilter(this.value)) {
                this.oldValue = this.value;
                this.oldSelectionStart = this.selectionStart;
                this.oldSelectionEnd = this.selectionEnd;
            } else if (this.hasOwnProperty("oldValue")) {
                this.value = this.oldValue;
                this.setSelectionRange(this.oldSelectionStart, this.oldSelectionEnd);
            } else {
                this.value = "";
            }
        });
    });
}

function calcolaImportoTotale() {
    let a = (parseFloat($('#emolumenti').val() || 0.00).toFixed(2));
    let b = (parseFloat($('#retDic').val().replace(",", ".") || 0.00).toFixed(2));
    let c = (parseFloat($('#importoSc').val().replace(",", ".") || 0.00).toFixed(2));
    let d = (parseFloat($('#S12').val().replace(",", ".") || 0.00).toFixed(2));
    let e = (parseFloat($('#S13').val().replace(",", ".") || 0.00).toFixed(2));
    let f = (parseFloat($('#S14').val().replace(",", ".") || 0.00).toFixed(2));
    let g = (parseFloat($('#S15').val().replace(",", ".") || 0.00).toFixed(2));
    let h = (parseFloat($('#S16').val().replace(",", ".") || 0.00).toFixed(2));

    let sum = 0;
    sum = parseFloat(a) + parseFloat(b) + parseFloat(c) + parseFloat(d) + parseFloat(e) + parseFloat(f) + parseFloat(g) + parseFloat(h);
    let totale = sum.toFixed(2).toString().replace(".", ",")
    $('#emolumenti').val($('#emolumenti').val().replace(".", ","))
    $("#totaleS").val(totale);
}

function cleanFormField() {
    let $option = $('#tipRap').find('option:selected');
    let a = $option.val();
    let datIni = $('#datIsc').val();
    $("#periodo").val('');
    $("#datTerm").val('');
    $("#daal").val('');
    $("#daal2").val('');
    $("#daal3").val('');
    $("#alal").val('');
    $("#alal2").val('');
    $("#alal3").val('');
    $("#perce1").val('');
    $("#perce2").val('');
    $("#perce3").val('');
    $("#mesi").val('');
    $("#PercPT").val('');

    if (a == 6 || a == 7 || a == 8 || a == 10)
        $("#daal").val(datIni);

    for (let i = 0; i < checkboxes.length; i++) {
        checkboxes[i].checked = false;
    }
}

function setDaalPerFields() {
    $('#daal').val($("#datIsc").val());
    let mesi = $('#periodo').val() / 3;
    let data1 = new Date($("#datIsc").val());
    var data2 = new Date(data1);
    var data2 = new Date(data2.setMonth(data2.getMonth() + mesi));
    let data3 = new Date(data2);
    data2.setDate(data2.getDate() - 1);
    let data4 = new Date(data3);
    data4 = new Date(data4.setMonth(data4.getMonth() + mesi));
    let data5 = new Date(data4);
    data4.setDate(data4.getDate() - 1);
    let data6 = new Date(data5);
    data6 = new Date(data6.setMonth(data6.getMonth() + mesi));
    data6.setDate(data6.getDate() - 1);

    let d3 = data3.getFullYear() + '-' + (+data3.getMonth() + 1).toString().padStart(2, '0') + '-' + data3.getDate().toString().padStart(2, '0');
    let d2 = data2.getFullYear() + '-' + (+data2.getMonth() + 1).toString().padStart(2, '0') + '-' + data2.getDate().toString().padStart(2, '0');
    let d4 = data4.getFullYear() + '-' + (+data4.getMonth() + 1).toString().padStart(2, '0') + '-' + data4.getDate().toString().padStart(2, '0');
    let d5 = data5.getFullYear() + '-' + (+data5.getMonth() + 1).toString().padStart(2, '0') + '-' + data5.getDate().toString().padStart(2, '0');
    let d6 = data6.getFullYear() + '-' + (+data6.getMonth() + 1).toString().padStart(2, '0') + '-' + data6.getDate().toString().padStart(2, '0');

    $("#alal").val(d2);
    $("#daal2").val(d3);
    $("#alal2").val(d4);
    $("#daal3").val(d5);
    $("#alal3").val(d6);

    $('#perce1').val('70');
    $('#perce2').val('80');
    $('#perce3').val('90');
}

function removeAllRequiredFromTipoContratto() {
    $("#periodo").attr("required", false);
    $("#datTerm").attr("required", false);
    $("#daal").attr("required", false);
    $("#daal2").attr("required", false);
    $("#daal3").attr("required", false);
    $("#alal").attr("required", false);
    $("#alal2").attr("required", false);
    $("#alal3").attr("required", false);
    $("#perce1").attr("required", false);
    $("#perce2").attr("required", false);
    $("#perce3").attr("required", false);
    $("#PercPT").attr("required", false);
    removeRequiredFromMesi();
}

function checkRequiredToMesi() {
    for (let i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked) {
            removeRequiredFromMesi();
            return;
        }
    }
    setRequiredToMesi();
}

function removeRequiredFromMesi() {
    checkboxes.forEach(ele => ele.removeAttribute("required"));
}

function setRequiredToMesi() {
    checkboxes.forEach(ele => ele.setAttribute("required", 'required'));
}

function checkValiditaMesi() {
    for (let i = 0; i < checkboxes.length; i++) {
        if (checkboxes[i].checked) {
            removeRequiredFromMesi();
            return true;
        }
    }

    let randomBoxToTrigger = Math.floor(Math.random() * 12);
    return checkboxes[randomBoxToTrigger].reportValidity();
}

function provinciaChangeFunctionWithStatoEsteroRdl({ target }, { provincia, comune, localita, cap, statoEstero }) {
    provinciaChangeFunctionRdl({ target }, { provincia, comune, localita, cap });
    if (!target.value)
        return;
    cleanStatoEsteroAndSetRequired();

    function cleanStatoEsteroAndSetRequired() {
        $(statoEstero).val('');
        $(provincia).attr('required', true);
        $(comune).attr('required', true);
        $(localita).attr('required', true);
        $(cap).attr('required', true);
        $(statoEstero).attr('required', false);
    }
}

async function provinciaChangeFunctionRdl({ target }, { provincia, comune, localita, cap }) {
    const valoreProvincia = target.value;
    if (!valoreProvincia) {
        cleanAndFormat();
        return;
    }
    const optionValues = await getOptionValuesForComuniFrom(valoreProvincia);
    fillComuniAndActivateIt();

    function cleanAndFormat() {
        $(provincia).val('');
        $(comune).html('');
        $(localita).html('');
        $(cap).val('');
        $(comune).attr('style', 'pointer-events: none;background: #E9ECEF;');
        $(localita).attr('style', 'pointer-events: none;background: #E9ECEF;');
    }

    async function getOptionValuesForComuniFrom(provincia) {
        let listaComuniFromProvincia = await getComuniFrom(provincia);

        let optionValues = '<option value="">Selezionare Comune</option>';
        listaComuniFromProvincia
            .forEach(comune => optionValues += '<option value="' + comune.Value + '">' + comune.Text + '</option>');

        return optionValues;
    }

    function fillComuniAndActivateIt() {
        $(comune).html(optionValues);
        $(comune).attr('style', '');
        $(localita).html('');
        $(localita).attr('style', 'pointer-events: none;background: #E9ECEF;');
        $(cap).val('');
    }
}

async function comuneChangeFunctionRdl({ target },  { localita, cap }) {
    const comune = target.value;
    if (!comune) {
        cleanAndFormatLocalita();
        return;
    }
    const optionValues = await getOptionValuesForLocalitaFrom(comune);
    fillLocalitaAndActivateIt();

    function cleanAndFormatLocalita() {
        $(localita).html('');
        $(localita).attr('style', 'pointer-events: none;background: #E9ECEF;');
        $(cap).val('');
    }

    async function getOptionValuesForLocalitaFrom(comune) {
        let listaLocalitaFromComune = await getLocalitaFrom(comune);

        let optionValues = '<option value="">Selezionare Localita</option>';
        listaLocalitaFromComune
            .forEach(localita => optionValues += '<option value="' + localita.Text.split('-')[0].trim() + '">' + localita.Text + '</option>');

        return optionValues;
    }

    function fillLocalitaAndActivateIt() {
        $(localita).html(optionValues);
        $(localita).attr('style', '');
        $(cap).val('');
    }
}

function localitaChangeFunction({ target }, { localita, cap }) {
    const localitaSelezionataValue = target.value;

    if (!localitaSelezionataValue) {
        $(cap).val('');
        return;
    }

    const localitaSelezionataHtml = $(localita + ' option:selected').html();
    const capFromLocalitaSelezionata = localitaSelezionataHtml.split('-')[1].trim();

    $(cap).val(capFromLocalitaSelezionata);
}

function statoEsteroChangeFunctionRdl({ target }, { provincia, comune, localita, cap, statoEstero }) {
    if (!target.value) {
        setRequiredToFields();
        return;
    }
    cleanFieldsAndFormat();

    function setRequiredToFields(){
        $(provincia).attr('required', true);
        $(comune).attr('required', true);
        $(localita).attr('required', true);
        $(cap).attr('required', true);
        $(statoEstero).attr('required', false);
    }

    function cleanFieldsAndFormat() {
        $(provincia).val('');
        $(comune).html('');
        $(localita).html('');
        $(cap).val('');
        $(provincia).attr('required', false);
        $(comune).attr('required', false);
        $(localita).attr('required', false);
        $(cap).attr('required', false);
        $(statoEstero).attr('required', true);
        $(comune).attr('style', 'pointer-events: none;background: #E9ECEF;');
        $(localita).attr('style', 'pointer-events: none;background: #E9ECEF;');

    }
}

function getContrattiFrom(datIni) {
    return $.ajax({
        url: '/AziendaConsulente/DdlContratto',
        type: 'GET',
        data: { 'datIni': datIni }
    });
}

function getLivelliFrom(contrattoCod, contratto) {
    return $.ajax({
        url: '/AziendaConsulente/DdlLivello',
        type: 'GET',
        data: { 'codcon': contrattoCod, 'dencon': contratto },
    });
}

function getComuniFrom(provincia) {
    return $.ajax({
        cache: false,
        url: '/Registrazione/GetComuniFromProvincia',
        type: 'GET',
        data: { provincia }
    });
}

function getLocalitaFrom(codiceComune) {
    return $.ajax({
        cache: false,
        url: '/Registrazione/GetLocalitaFromComune',
        type: 'GET',
        data: { codiceComune }
    });
}