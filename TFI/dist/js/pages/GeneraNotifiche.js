$(document).ready(function () {

    var codPosDaReadonly = $("#hidden_codPos_da_Readonly").val();

    if (codPosDaReadonly == "Y") {
        $("#codPos_da").prop("readonly", true);
        $("#codPos_a").prop("readonly", true);
    }

    setTimeout(function () { $("#codPos_da").focus(); $("#codPos_a").focus(); $("#anno").focus(); }, 300);

    $("#codPos_da").blur(function () {
        var codPos = $(this).val();
        GetRagioneSociale(codPos, 1);
    });

    $("#codPos_a").blur(function () {
        var codPos = $(this).val();
        GetRagioneSociale(codPos, 2);
    });

    setInputFilter(document.getElementById("codPos_da"), function (value) {
        return /^\d*$/.test(value);
    });

    setInputFilter(document.getElementById("codPos_a"), function (value) {
        return /^\d*$/.test(value);
    });

    setInputFilter(document.getElementById("anno"), function (value) {
        return /^\d*$/.test(value);
    });

    $("#btn_cerca").click(function () {
        return RicercaValida();
    });

    $("#btn_stampa").click(function () {
        window.print();
    });

    $(".rb_seleziona").change(function () {
        Seleziona_Deseleziona($(this));
    });

    $("#btn_calcolo").click(function () {

        if (MessaggioConferma()) {

            var data = GetDataFromTableRows();

            $.ajax({
                url: "/Amministrativo/CalcolaNotifiche",
                dataType: "text",
                method: "POST",
                data: { "jsonString": JSON.stringify(data) },
                beforeSend: function () {
                    // Show image container
                    $('.pannelloOpaco').fadeIn()
                    //
                },
                success: function (result) {
                    if (result == "OK") {
                        window.location.href = "/Amministrativo/ContabilizzazioneNotifiche";                        
                    }
                    else {
                        window.location.reload();
                    }
                },
                complete: function () {

                    // Hide image container
                    $('.pannelloOpaco').fadeOut();
                    //
                }
            });
        }

    });

    var hd_meseDa = $("#hd_meseDa").html();
    var hd_meseA = $("#hd_meseA").html();

    if (hd_meseDa != "") {
        $("#mese_da option").each(function () {
            var optionValue = $(this).prop("value");
            if (optionValue == hd_meseDa) {
                $(this).prop("selected", true);
            }
        });
    }

    if (hd_meseA != "") {
        $("#mese_a option").each(function () {
            var optionValue = $(this).prop("value");
            if (optionValue == hd_meseA) {
                $(this).prop("selected", true);
            }
        });
    }

    $("#downloadExcel a").click(function () {
        $(this).fadeOut();
    });


    $("#btnExport").click(function (e) {
        window.open('data:application/vnd.ms-excel,' + encodeURI($('#cont_tabella').html()));
        e.preventDefault();
    });

});



function GetRagioneSociale(codPos, valore) {
    $.ajax({
        url: "/Amministrativo/VisualizzaRagioneSociale",
        dataType: "text",
        method: "POST",
        data: { "codPos": codPos },
        success: function (result) {
            if (valore == 1) {
                $("#ragSoc_da").val(result);
            }
            else {
                $("#ragSoc_a").val(result);
            }
        }
    });
}

function setInputFilter(textbox, inputFilter) {
    ["input", "keydown", "keyup", "mousedown", "mouseup", "select", "contextmenu", "drop"].forEach(function (event) {
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

function RicercaValida() {

    var codPosDa = $("#codPos_da").val();
    var anno = $("#anno").val();
    var meseDa = parseInt($("#mese_da").val());
    var meseA = parseInt($("#mese_a").val());

    if (codPosDa == "" && anno == "") {
        alert("Inserire una posizione o selezionare un periodo");
        return false;
    }

    if (anno != "") {

        anno = parseInt(anno);

        if (anno < 2003) {
            alert("Impostare un anno maggiore del 2002");
            return false;
        }

        if (meseDa == 0) {
            alert("Selezionare il mese di partenza");
            return false;
        }

        if (meseA == 0) {
            alert("Selezionare il mese di arrivo");
            return false;
        }

        if (meseDa > meseA) {
            alert("Il mese di arrivo non può essere minore del mese di partenza");
            return false;
        }
    }

    return true;
}

function Seleziona_Deseleziona(radioButton) {

    if (radioButton.val() == "TUTTE") {
        $(".ckb_seleziona").prop("checked", true);
    }
    else {
        $(".ckb_seleziona").prop("checked", false);
    }
}

function GetDataFromTableRows() {

    var obj;
    var codPos;
    var anno;
    var numMese;
    var tipIsc;
    var causale;
    var dataRiferimento;
    var objList = [];

    $(".rb_causale").each(function () {
        if ($(this).prop("checked")) {
            causale = $(this).val();
        }
    });

    dataRiferimento = $("#dataRiferimento").html();


    $("#tabellaNotifiche tbody tr").each(function () {
        var chkBox = $(this).find(".ckb_seleziona");
        if (chkBox.prop("checked")) {

            codPos = $(this).find(".hd_codPos").html();
            anno = $(this).find(".hd_anno").html();
            numMese = $(this).find(".hd_numMese").html();
            tipIsc = $(this).find(".hd_tipIsc").html();

            obj = {
                CodPos: codPos,
                Anno: anno,
                NumMese: numMese,
                TipIsc: tipIsc,
                Causale: causale,
                DataRiferimento: dataRiferimento
            }

            objList.push(obj);
        }
    });

    return objList;
}

function MessaggioConferma() {

    var i = 0;
    var numOccorrenze = $("#numOccorrenze").html();
    var dataSistema = $("#dataSistema").html();

    $("#tabellaNotifiche tbody tr").each(function () {

        var chkBox = $(this).find(".ckb_seleziona");
        if (chkBox.prop("checked")) {
            i++;
        }
    });

    if (parseInt(i) == 0) {
        alert("Nessuna riga selezionata. Impossibile continuare");
        return false;
    }
    else {
        var result = prompt("Inserisci la data di emissione della notifica", dataSistema);

        if (result != null) {
            if (result != "") {
                $("#dataRiferimento").html(result);
                return confirm("Righe in lista: " + numOccorrenze + " - Righe selezionate: " + i + ". Procedere con la generazione delle notifiche?");
            }
        }


    }
}



    
