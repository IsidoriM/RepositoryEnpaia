$(document).ready(function () {

    $('.toast').toast('show');

    $("#ddl_sceltaAnno").change(function () {
        $("#form_sceltaAnno").submit();
    });

    $("#ddl_sceltaAnno option").each(function () {
        var annoSelezionato = $("#annoSelezionato").html();
        if ($(this).val() == annoSelezionato) {
            $(this).prop("selected", true);
        }
    });

    SetInsertButtons();
    SetRadioButtons();

    //$("#btn_eliminaDIPA").click(function () {
    //    var anno = $("#selectedYear").val();
    //    var mese = $("#selectedMonth").val();
    //    var prog = $("#selectedProg").val();
    //    var idDipa = $("#associatedIdDipa").val();
    //    //var staDen = $("#statoDenuncia").val();
    //    //var tipMov = $("#tipMov").val();

    //    //var statoAttualeDenuncia = $("#statoAttualeDenuncia").html();

    //    if (confirm("Confermi l'eliminazione della Denuncia?")) {
    //        $.ajax({
    //            url: '/AziendaConsulente/EliminaDIPA',
    //            dataType: 'json',
    //            method: 'POST',
    //            data: { 'anno': anno, 'mese': mese, 'proDen': prog, 'idDipa': idDipa },
    //            success: function (result) {
    //                var listaDenunceMensili = JSON.parse(result);
    //                SostituisciRecord(listaDenunceMensili);
    //            },
    //            error: function () {
    //                alert("errore ajax per eliminazione DIPA");
    //            }
    //        });
    //    }
    //});

    //$(".btn").click(function () {
        
    //    var bottone = $(this).prop("id");        

    //    var anno = $("#selectedYear").val();
    //    var mese = $("#selectedMonth").val();
    //    var prog = $("#selectedProg").val();
    //    var idDipa = $("#associatedIdDipa").val();
    //    var staDen = $("#statoDenuncia").val();
    //    var tipMov = $("#tipMov").val();

    //    var statoAttualeDenuncia = $("#statoAttualeDenuncia").html();

    //    if (staDen == statoAttualeDenuncia) {

    //        if (bottone == "btn_eliminaDIPA") {

    //            if (confirm("Confermi l'eliminazione della Denuncia?")) {
    //                $.ajax({
    //                    url: '/AziendaConsulente/EliminaDIPA',
    //                    dataType: 'json',
    //                    method: 'POST',
    //                    data: { 'anno': anno, 'mese': mese, 'proDen': prog, 'idDipa': idDipa },
    //                    success: function (result) {
    //                        var listaDenunceMensili = JSON.parse(result);
    //                        SostituisciRecord(listaDenunceMensili);
    //                    },
    //                    error: function () {
    //                        alert("errore ajax per eliminazione DIPA");
    //                    }
    //                });                
    //            }
    //        }
    //        else if (bottone == "btn_modificaDIPA") {
    //            if (tipMov == "DP") {
    //                $("#form_sceltaDipa").submit();
    //            }
    //        }
    //    }
    //    //else {
    //    //    alert("Lo stato della denuncia per il mese selezionato è cambiato!");
    //    //}
    //});
    
    var btnConfermaIsVisible = $("#btnConfermaIsVisible").html();

    if (btnConfermaIsVisible == "True") {
        $("#upload_btnConferma").css("display", "block");
    }
    else {
        $("#upload_btnConferma").css("display", "none");
    }

    $("#cBox_arretrato").click(function () {
        var isChecked = $(this).prop("checked");

        if (isChecked) {
            $("#opaco").fadeIn();
            $(".btn_arretrato").closest("div").css("display", "block");
            
            $("#btn_totMeseDIPA").css("display", "none");
            $("#btn_modificaDIPA").css("display", "none");
            $("#btn_eliminaDIPA").css("display", "none");
            $("#btn_readonlyDIPA").css("display", "none");
            $("#btn_RettificheDIPA").css("display", "none");

            $("#tb_DenunceAnnuali .radioB").each(function () {
                $(this).prop("checked", false);
            });
        }
        else {
            $("#opaco").fadeOut();
            $(".btn_arretrato").closest("div").css("display", "none");           
        }
    });
});


function GetStatoAttualeDenuncia(anno, mese, tipMov, progressivo) {
    $.ajax({
        url: '/AziendaConsulente/GetStatoDenuncia',
        dataType: 'json',
        method: 'POST',
        data: { 'anno': anno, 'mese': mese, 'tipMov': tipMov, 'proDen': progressivo },
        success: function (result) {
            var statoAttualeDenuncia = result;
            $("#statoAttualeDenuncia").html(statoAttualeDenuncia);
        }
    });
}

function GetDatiRiga(riga) {
    
    var obj;
    var descrizione;
    var progressivo;
    var anno;
    var mese;
    var tipMov;
    var idDipa;
    var staDen;
    var rettifiche;
   
    progressivo = riga.children(".proDen").html();

    $("#ddl_sceltaAnno option").each(function () {
        if ($(this).prop("selected")) {
            anno = $(this).prop("value");
        }
    });

    mese = riga.children(".numMes").html();
    descrizione = riga.children(".descrizione").html();
    staDen = riga.children(".staDen").html();
    rettifiche = riga.children(".rettifiche").html();
    //debugger;
    if (rettifiche != "-") {
        $("#btn_RettificheDIPA").css("display", "block");
    }
    else {
        $("#btn_RettificheDIPA").css("display", "none");
    }

    if (descrizione.toUpperCase() == "DENUNCIA") {
        tipMov = "DP";
    }
    else {
        tipMov = "NU";
    }

    idDipa = riga.children(".idDipa").html();

    obj = {
        idDipa: idDipa,
        proDen: progressivo,
        anno: anno,
        mese: mese,
        tipMov: tipMov,
        staDen: staDen
    }

    return obj;
}

function SostituisciRecord(recordDaVisualizzare) {
    var btnConfermaIsVisible = $("#btnConfermaIsVisible").html();
    var annoSelezionato = parseInt($("#annoSelezionato").html());
    var meseDenunciabile = parseInt($("#meseDenunciabile").html());
    var annoDenunciabile = parseInt($("#annoDenunciabile").html());
    var numeroDenunceSospese = parseInt($("#numSospesi").html());
    var numeroDenunceSospese = numeroDenunceSospese == 0 ? 0 : numeroDenunceSospese - 1;
    var numDenunceAnnoPrecedente = parseInt($("#numDenunceAnnoPrecedente").html());
    var primoAnnoNonPrescritto = parseInt($("#primoAnnoNonPrescritto").html());
    var semaforo = true;
    var newcode = "";

    $("#tb_DenunceAnnuali tbody").html("");

    for (i in recordDaVisualizzare) {
        newcode = "<tr>";
        newcode += "<td>" + recordDaVisualizzare[i].MesDen + "</td>";
        newcode += "<td class='descrizione'>" + recordDaVisualizzare[i].TipMov + "</td>";
        newcode += "<td>" + recordDaVisualizzare[i].DatSca + "</td>";
        newcode += "<td>" + recordDaVisualizzare[i].DatApe + "</td>";
        newcode += "<td class='staDen'>" + recordDaVisualizzare[i].StaDen + "</td>";
        newcode += "<td>" + recordDaVisualizzare[i].ImpCon + "</td>";
        newcode += "<td>" + recordDaVisualizzare[i].ImpSan + "</td>";
        newcode += "<td class='rettifiche'>" + recordDaVisualizzare[i].ImpDel + "</td>";
        newcode += "<td>" + recordDaVisualizzare[i].ImpSanRet + "</td>";
        newcode += "<td>" + recordDaVisualizzare[i].ImpTot + "</td>";
        if (recordDaVisualizzare[i].StaDen == "Rapporti di Lavoro Assenti") { 
            newcode += "<td>-</td>";
            newcode += "<td>-</td>";
        }
        else if (recordDaVisualizzare[i].StaDen != "Denuncia non Presentata") {
            
            if (recordDaVisualizzare[i].StaDen == "Non Confermata") {
                numeroDenunceSospese++;
                newcode += "<td><button type='button' class='btn_inserisci'><i class='fas fa-pencil-alt'></i></button></td>";
                newcode += "<td><button type='button' id='btn_eliminaDIPA'><i class='fas fa-trash-alt'></i></button></td>";
            } else {
                newcode += "<td></td>";
                newcode += "<td><input type='radio' name='selezRecord' value='" + recordDaVisualizzare[i].NumMes + "' checked='false' class='radioB'></td>";
            }
        }
        else {
            if ((numDenunceAnnoPrecedente >= 12 && annoSelezionato > primoAnnoNonPrescritto) || annoSelezionato === primoAnnoNonPrescritto) {
                if (annoSelezionato < annoDenunciabile) {
                    if (numeroDenunceSospese == 0 && semaforo) {
                        semaforo = false;
                        newcode += "<td><button type='button' class='btn_inserisci'><i class='fas fa-pencil-alt'></i></button></td>";
                        newcode += "<td><button type='button' class='btn_upload'><i class='fas fa-file-upload nav-icon'></i></button></td>";
                    }
                    else {
                        newcode += "<td></td>";
                        newcode += "<td></td>";
                    }
                }
                else {
                    if (recordDaVisualizzare[i].NumMes <= meseDenunciabile && recordDaVisualizzare[i].NumSospesi == 0 && semaforo) {
                        semaforo = false;
                        newcode += "<td><button type='button' class='btn_inserisci'><i class='fas fa-pencil-alt'></i></button></td>";
                        newcode += "<td><button type='button' class='btn_upload'><i class='fas fa-file-upload nav-icon'></i></button></td>";
                    }
                    else {
                        newcode += "<td></td>";
                        newcode += "<td></td>";
                    }
                }
            }
            else {
                newcode += "<td></td>";
                newcode += "<td></td>";
            }            
        }
        newcode += "<td style='display: none'>" + recordDaVisualizzare[i].UteChi + "</td>";
        newcode += "<td style='display: none' class='proDen'>" + recordDaVisualizzare[i].ProDen + "</td>";
        newcode += "<td style='display: none' class='idDipa'>" + recordDaVisualizzare[i].IdDipa + "</td>";
        newcode += "<td style='display: none' class='numMes'>" + recordDaVisualizzare[i].NumMes + "</td>";
        newcode += "<td style='display: none'><div class='tipOpe'></div></td>";
        newcode += "</tr>";

        $("#tb_DenunceAnnuali tbody").append(newcode);
    }

    $("#tb_DenunceAnnuali .radioB").each(function () {
        $(this).prop("checked", false);
    });

    SetRadioButtons();
    SetInsertButtons();

    //$("#btn_eliminaDIPA").css("display", "none");
    //$("#btn_modificaDIPA").css("display", "none");
    $("#btn_readonlyDIPA").css("display", "none");
    $("#btn_totMeseDIPA").css("display", "none");
    
    
    $("#upload_btnConferma").css("display", "block");
    
}

function FillHiddenForms(anno, mese, proDen, idDipa, staDen, tipMov) {
    $(".selectedYear").val(anno);
    $(".selectedMonth").val(mese);
    $(".selectedProg").val(proDen);
    $(".associatedIdDipa").val(idDipa);
    $("#statoDenuncia").val(staDen);
    $("#tipMov").val(tipMov);
    $(".isFirstLoading").val(true);
}

function SetRadioButtons() {

    $("#tb_DenunceAnnuali .radioB").change(function () {
        
        var rigaSelezionata = $(this).closest("tr");
        var staDen = rigaSelezionata.children(".staDen").html();

        if (staDen == "Non Confermata") {
            $("#btn_eliminaDIPA").css("display", "block");
            $("#btn_modificaDIPA").css("display", "block");
            $("#btn_readonlyDIPA").css("display", "none");
            $("#btn_totMeseDIPA").css("display", "none");
        }
        else {
            $("#btn_eliminaDIPA").css("display", "none");
            $("#btn_modificaDIPA").css("display", "none");
            $("#btn_readonlyDIPA").css("display", "block");
            $("#btn_totMeseDIPA").css("display", "block");
        }

        var datiRiga = GetDatiRiga(rigaSelezionata);
        GetStatoAttualeDenuncia(datiRiga.anno, datiRiga.mese, datiRiga.tipMov, datiRiga.proDen);
        FillHiddenForms(datiRiga.anno, datiRiga.mese, datiRiga.proDen, datiRiga.idDipa, datiRiga.staDen, datiRiga.tipMov);
    });

}

function SetInsertButtons() {

    $("#tb_DenunceAnnuali .btn_inserisci").click(function () {
        var rigaSelezionata = $(this).closest("tr");
        var datiRiga = GetDatiRiga(rigaSelezionata);
        FillHiddenForms(datiRiga.anno, datiRiga.mese, datiRiga.proDen, datiRiga.idDipa, datiRiga.staDen, datiRiga.tipMov);
        $("#form_sceltaDipa").submit();
    });

    $("#tb_DenunceAnnuali .btn_upload").click(function () {
        var rigaSelezionata = $(this).closest("tr");
        var datiRiga = GetDatiRiga(rigaSelezionata);
        FillUploadForm(datiRiga.anno, datiRiga.mese, datiRiga.proDen);
        $('#denunciaMensile_Upload').modal({ backdrop: 'static' });       
    });

    $("#tb_DenunceAnnuali #btn_eliminaDIPA").click(function () {
        var rigaSelezionata = $(this).closest("tr");
        var datiRiga = GetDatiRiga(rigaSelezionata);
        FillHiddenForms(datiRiga.anno, datiRiga.mese, datiRiga.proDen, datiRiga.idDipa, datiRiga.staDen, datiRiga.tipMov);

        var anno = $("#selectedYear").val();
        var mese = $("#selectedMonth").val();
        var prog = $("#selectedProg").val();
        var idDipa = $("#associatedIdDipa").val();

        if (confirm("Confermi l'eliminazione della Denuncia?")) {
            $.ajax({
                url: '/AziendaConsulente/EliminaDipa',
                dataType: 'json',
                method: 'POST',
                data: { 'anno': anno, 'mese': mese, 'proDen': prog, 'idDipa': idDipa },
                success: function (result) {
                    var listaDenunceMensili = result.listaDenunceMensili;
                    SostituisciRecord(listaDenunceMensili);
                    if (result.isDeleted) {
                        alert("Denuncia eliminata con successo")
                    }
                    else {
                        alert("Si sono verificati dei problemi nella cancellazione della denuncia.")
                    }
                },
                error: function () {
                    alert("errore ajax per eliminazione DIPA");
                }
            });
        }
    });

    $("#fileUpload").change(function () {
        if ($("#fileUpload").val() == '')
            return;

        if ($("#fileUpload").val().match(/.+(.txt)/)) {
            $("#fileUpload_errorMsg").attr('hidden', true);
        }
        else {
            $("#fileUpload").val('');
            $("#fileUpload_errorMsg").attr('hidden', false);
        }
    });

    $("#fileUpload").blur(function (e) {
        if (e.target.files.length == 0) {
            $("#fileUpload_errorMsg").attr('hidden', true);
            $("#fileUpload").val('');
            return;
        }
    });
}

function GetDenunceSospese() {

    $.ajax({
        url: '/AziendaConsulente/GetDenunceSospese',
        dataType: 'text',
        method: 'POST',
        success: function (result) {
            $("#numSospesi").html(result);
        }
    });
}

function FillUploadForm(anno, mese, proDen) {
    
    $("#denunciaMensile_Upload #upload_anno").val(anno);
    $("#denunciaMensile_Upload #upload_mese option").each(function () {
        if ($("#upload_selectedMonth").html() == 0) {
            if ($(this).prop("value") == mese) {
                $(this).prop("selected", true);
                $("#upload_selectedMonth").val(mese);
            }
        }
    });
    $("#denunciaMensile_Upload #upload_proDen").val(proDen);
}