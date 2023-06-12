

$(document).ready(function () {

    $("#codPos").blur(function () {

        var value = $(this).val();

        $.ajax({
            url: "/Amministrativo/VisualizzaRagioneSociale",
            dataType: "text",
            method: "POST",
            data: { "codPos": value },
            success: function (result) {
                var ragioneSociale = result;
                $("#ragSoc").val(ragioneSociale);
            }
        });
    });

    setInputFilter(document.getElementById("codPos"), function (value) {
        return /^\d*$/.test(value);
    });

    setInputFilter(document.getElementById("anno_da"), function (value) {
        return /^\d*$/.test(value);
    });

    setInputFilter(document.getElementById("anno_a"), function (value) {
        return /^\d*$/.test(value);
    });

    var codPosReadonly = $("#hidden_codPosReadonly").val();

    if (codPosReadonly == "Y") {
        $("#codPos").prop("readonly", true);
        setTimeout(function () { $("#codPos").focus(); $("#anno_da").focus(); }, 300);
    }

    var oldRagSoc = $("#oldRagSoc").val();
    var oldTipMov = $("#oldTipMov").val();
    var oldMese_da = $("#oldMese_da").val();
    var oldMese_a = $("#oldMese_a").val();

    $("#ragSoc").val(oldRagSoc);
    $("#tipMov option").each(function () {
        var optionValue = $(this).prop("value");
        if (optionValue == oldTipMov) {
            $(this).prop("selected", true);
        }
    });
    $("#mese_da option").each(function () {
        var optionValue = $(this).prop("value");
        if (optionValue == oldMese_da) {
            $(this).prop("selected", true);
        }
    });
    $("#mese_a option").each(function () {
        var optionValue = $(this).prop("value");
        if (optionValue == oldMese_a) {
            $(this).prop("selected", true);
        }
    });


    $(".rb_seleziona").change(function () {
        Seleziona_Deseleziona($(this));
    });


    $("#btn_annullaMovimenti").click(function () {

        if (CiSonoRecordCeccati()) {
            var data = GetDataFromTableRows();

            $.ajax({
                url: "/Amministrativo/AnnullaDenuncia_Step1",
                dataType: "json",
                method: "POST",
                data: { "objList": JSON.stringify(data) },
                success: function (result) {

                    var listaConferme = JSON.parse(result);
                    var flag = listaConferme[0];

                    if (flag == "CONFERMA") {

                        listaConferme.splice(0, 1);
                        $("#confirm_modal_1").modal("show");
                        for (var i in listaConferme) {
                            $("#confirm_modal_1 .cont_messages").append("<p>" + listaConferme[i] + "</p> <br/>");
                        }

                    }
                    else if (flag == "PROSEGUI") {
                        $.ajax({
                            url: "/Amministrativo/AnnullaDenuncia_Step2_Ajax",
                            dataType: "text",
                            method: "POST",
                            data: { "objList": JSON.stringify(data) },
                            success: function (result) {

                                var stringArray;
                                $("#confirm_modal_2").modal("show");

                                if (result.includes("|")) {

                                    stringArray = result.split("|");
                                    $("#confirm_modal_2 table").css("display", "block");
                                    for (var i in stringArray) {
                                        $("#confirm_modal_2 table tbody").append("<tr><td>" + stringArray[i] + "</td></tr>");
                                    };

                                }
                                else {
                                    $("#confirm_modal_2 .cont_messages").html("<p>" + result + "</p>");
                                }
                            }

                        });
                    }
                    else {
                        location.reload();
                    }

                }
            });
        }


    });


    $("#confirm_modal_2 .btn_official").click(function () {
        $("#confirm_modal_1 .cont_messages").html("<p></p>");
        $("#confirm_modal_2 table tbody").html("<tr></tr>");
        $("#confirm_modal_2").modal("hide");
    });

    $("#confirm_modal_1 #confirm_no").click(function () {
        $("#confirm_modal_1 .cont_messages").html("<p></p>");
    });

    $("#confirm_modal_1 #confirm_si").click(function () {

        $("#confirm_modal_1").modal("hide");
        var data = GetDataFromTableRows();
        $.ajax({
            url: "/Amministrativo/AnnullaDenuncia_Step2_Ajax",
            dataType: "text",
            method: "POST",
            data: { "objList": JSON.stringify(data) },
            success: function (result) {

                var stringArray;
                $("#confirm_modal_2").modal("show");

                if (result.includes("|")) {

                    stringArray = result.split("|");
                    $("#confirm_modal_2 table").css("display", "block");
                    for (var i in stringArray) {
                        $("#confirm_modal_2 table tbody").append("<tr><td>" + stringArray[i] + "</td></tr>");
                    };

                }
                else {
                    $("#confirm_modal_2 .cont_messages").html("<p>" + result + "</p>");
                }
            }

        });
    });


    $("#btn_stampa").click(function () {
        print();
    });


});


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


function Seleziona_Deseleziona(radioButton) {

    if (radioButton.val() == 1) {

        $(".chk_annulla").prop("checked", true);
    }
    else {
        $(".chk_annulla").prop("checked", false);
    }
}


function CiSonoRecordCeccati() {
    var n = 0;

    $("#tb_AnnullamentoDenunce tbody tr").each(function () {

        if ($(this).find(".chk_annulla").prop("checked")) {
            n++;
        }
    });

    if (n == 0) {
        alert("Nessuna denuncia da annullare.");
        return false;
    }
    else {
        return true;
    }
}


function GetDataFromTableRows() {

    var obj;
    var codPos;
    var annDen;
    var mesDen;
    var numMov;
    var tipMov;
    var proDen;
    var oldTipMov;
    var impAbb;
    var impCon;
    var numSan;
    var numSanAnn;
    var impSanDet;
    var impAddRec;
    var impAssCon;
    var impConDel;
    var impAddRecDel;
    var impSanRet;
    var impDis;
    var numRecord;
    var codCauSan;
    var codCauMov;
    var ultAgg;
    var objList = [];

    $("#tb_AnnullamentoDenunce tbody tr").each(function () {
        var chkBox = $(this).find(".chk_annulla");
        if (chkBox.prop("checked")) {

            codPos = $(this).find(".hd_codPos").html();
            annDen = $(this).find(".hd_annDen").html();
            mese = $(this).find(".hd_mese").html();
            numMov = $(this).find(".hd_numMov").html();
            tipMov = $(this).find(".hd_tipMov").html();
            proDen = $(this).find(".hd_proDen").html();
            oldTipMov = $("#oldTipMov").val();
            impAbb = parseFloat($(this).find(".hd_impAbb").html().replace(",", "."));
            impCon = parseFloat($(this).find(".hd_impCon").html().replace(",", "."));
            numSan = $(this).find(".hd_numSan").html();
            numSanAnn = $(this).find(".hd_numSanAnn").html();
            impSanDet = parseFloat($(this).find(".hd_impSanDet").html().replace(",", "."));
            impAddRec = parseFloat($(this).find(".hd_impAddRec").html().replace(",", "."));
            impAssCon = parseFloat($(this).find(".hd_impAssCon").html().replace(",", "."));
            impConDel = parseFloat($(this).find(".hd_impConDel").html().replace(",", "."));
            impAddRecDel = parseFloat($(this).find(".hd_impAddRecDel").html().replace(",", "."));
            impSanRet = parseFloat($(this).find(".hd_impSanRet").html().replace(",", "."));
            impDis = parseFloat($(this).find(".hd_impDis").html().replace(",", "."));
            codCauSan = $(this).find(".hd_codCauSan").html();
            codCauMov = $(this).find(".hd_codCauMov").html();
            numRecord = $("#numRecord").val();
            ultAgg = $(this).find(".hd_ultAgg").html()

            obj = {
                CodPos: codPos,
                AnnDen: annDen,
                Mese: mese,
                NumMov: numMov,
                TipMov: tipMov,
                ProDen: proDen,
                OldTipMov: oldTipMov,
                ImpAbb: impAbb,
                ImpCon: impCon,
                NumSan: numSan,
                NumSanAnn: numSanAnn,
                ImpSanDet: impSanDet,
                ImpAddRec: impAddRec,
                ImpAssCon: impAssCon,
                ImpConDel: impConDel,
                ImpAddRecDel: impAddRecDel,
                ImpSanRet: impSanRet,
                ImpDis: impDis,
                CodCauSan: codCauSan,
                CodCauMov: codCauMov,
                NumRecord: numRecord,
                UltAgg: ultAgg
            }

            objList.push(obj);
        }
    });

    return objList;
}

