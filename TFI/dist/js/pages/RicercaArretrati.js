$(document).ready(function () {

    //if ($('#txtMatricola').val() != "") {
    //    $('#rbtnMatricola').prop('checked', true);
    //}
    //if ($('#txtMatricola').val() != "") {
    //    $('#rbtnCompetenza').prop('checked', true);
    //}

    if ($("#txtMatricola").val() == 0) {
        $("#txtMatricola").val("");
    }

    SetRadioButtons();

    $(document).on("input", "#txtMatricola", function (evt) {
        var input = $(evt.target),
            val = input.val();

        if (val.length > 9) {
            alert("Superato il limite massimo di caratteri!");
            input.val(val.substring(0, 9));
            return false;
        }
    });

    $(".paginate_button").click(function () {
        SetRadioButtons();
    });
});


function checkFields() {
    //debugger;
    if (document.getElementById("ddlAnno").selectedIndex <= 0 && document.getElementById("ddlAnno").disabled == false) {
        alert("Selezionare un anno denuncia!");
        document.getElementById("ddlAnno").focus();
        event.preventDefault(); //.returnValue = null;
        return false;
    }
    if (document.getElementById("ddlAnnCom") != null) {
        if (document.getElementById("ddlAnnCom").selectedIndex <= 0 && document.getElementById("ddlAnnCom").disabled == false) {
            alert("Selezionare un anno di competenza!");
            document.getElementById("ddlAnnCom").focus();
            event.preventDefault(); //.returnValue = null;
            return false;
        }
    }
    if (document.getElementById("txtMatricola").value.replace(' ', '') == '' && document.getElementById("txtMatricola").disabled == false) {
        alert("Inserire la matricola!");
        document.getElementById("txtMatricola").select();
        event.preventDefault(); //.returnValue = null;
        return false;
    }
    else if (document.getElementById("txtMatricola").disabled == false) {
        var reg = /^\d+$/;
        var val = document.getElementById("txtMatricola").value.trim();
        if (!reg.test(val)) {
            alert("Valore non corretto per il campo matricola!\n(sono ammessi solo valori numerici)");
            event.preventDefault(); //.returnValue = null;
            return false;
		}
	}
}

function enableElement() {
    document.getElementById("ddlAnno").disabled = (document.getElementById("rbtnDenuncia").checked == false);
    if (document.getElementById("ddlAnno").disabled) {
        document.getElementById("ddlAnno").selectedIndex = 0;
    }
    else { document.getElementById("ddlAnno").focus(); }
    if (document.getElementById("ddlAnnCom") != null) {
        document.getElementById("ddlAnnCom").disabled = (document.getElementById("rbtnCompetenza").checked == false);
        if (document.getElementById("ddlAnnCom").disabled) {
            document.getElementById("ddlAnnCom").selectedIndex = 0;
        }
        else { document.getElementById("ddlAnnCom").focus(); }
    }
    document.getElementById("txtMatricola").disabled = (document.getElementById("rbtnMatricola").checked == false);
    if (document.getElementById("txtMatricola").disabled) {
        document.getElementById("txtMatricola").value = '';
    }
    else { document.getElementById("txtMatricola").focus(); }
}

function unCheck() {
    if (document.getElementById("hdnCheck").value != "") {
        document.getElementById(document.getElementById("hdnCheck").value).checked = false;
    }
    if (document.getElementById("hdnCheck").value != event.srcElement.id) {
        document.getElementById("hdnCheck").value = event.srcElement.id;
    }
    else {
        document.getElementById("hdnCheck").value = "";
        document.getElementById("hdnStato").value = "";
    }
}

function isSelected() {
    if (document.getElementById("hdnSelectedIndex").value == "") {
        alert("Selezionare un arretrato!");
        event.preventDefault(); //.returnValue = null;
        return false;
    }
    else { return true; }
}


function SetRadioButtons() {
    $("#tbArretrati .radioB").change(function () {
        var rigaSelezionata = $(this).closest("tr");
        var rettifiche = rigaSelezionata.children("#rettifiche").html();
        document.getElementById('btnDiv').style.display = 'block';
        document.getElementById('btnSeleziona').style.display = 'block';
        document.getElementById('btnChiudi').style.display = 'block';      
        if (rettifiche.replace(" ", "") != "") {
            if (parseFloat(rettifiche.replace(/\./g, "").replace(",", ".")) != 0) {
                document.getElementById('btnRettifiche').style.display = 'block';
                document.getElementById('btnSeleziona').innerHTML = "Arretrato Originale";
            }
            else {
                document.getElementById('btnRettifiche').style.display = 'none';
                document.getElementById('btnSeleziona').innerHTML = "Seleziona";
            }
        }
        
        var datiRiga = GetDatiRiga(rigaSelezionata);
        /*GetStatoAttualeDenuncia(datiRiga.anno, datiRiga.mese, datiRiga.tipMov, datiRiga.proDen);*/
        FillHiddenForms(datiRiga.anno, datiRiga.mese, datiRiga.proDen, datiRiga.matricola, datiRiga.parm, datiRiga.descrizione, datiRiga.anncom);
    });
    /*document.getElementById('#btnDiv').style.visibility = 'visible';*/
};

function GetDatiRiga(riga) {
   
    var obj;
    var descrizione;
    var progressivo;
    var anno;
    var mese;
    var matricola;
    var parm;
    var anncom;

    anno = riga.children("#anno").html();
    progressivo = riga.children("#progressivo").html();
    matricola = riga.children("#matricola").html();
    parm = riga.children("#parm").html();
    anncom = riga.children("#annoC").html();
    //$("#ddlAnno option").each(function () {
    //    if ($(this).prop("selected")) {
    //        anno = $(this).prop("value");
    //    }
    //});

    //$("#ddlAnnCom option").each(function () {
    //    if ($(this).prop("selected")) {
    //        anno = $(this).prop("value");
    //    }
    //});

    mese = riga.children("#mese").html();
/*    descrizione = riga.children("#descrizione").html();*/
    descrizione = riga.children("#staDen").html();

    if (descrizione.toUpperCase() == "NON CONFERMATA") {
        descrizione = "N";
    }
    else {
        descrizione = "A";
    }

    obj = {
        anno: anno,
        proDen: progressivo,
        mese: mese,
        descrizione: descrizione,
        matricola: matricola,
        parm: parm,
        anncom: anncom
         
    }
    
    return obj;
}

function FillHiddenForms(anno, mese, progressivo, matricola, parm, descrizione, anncom) {
    $("#selectedYear").val(anno);
    $("#selectedMonth").val(mese);
    $("#selectedParametro").val(parm);
    $("#selectedMatricola").val(matricola);
    $("#selectedDescrizione").val(descrizione);
    $("#selectedProgressivo").val(progressivo);
    $("#selectedAnnoCompetenza").val(anncom);
    
}

function selItem() {
    
    var datagrid;
    var rettifiche = "";

    var index = event.srcElement.parentElement.parentElement.rowIndex;

    if (document.getElementById("rbtnDenuncia").checked == true) {
        datagrid = "dgTestata__ctl";
        //if (event.srcElement.parentElement.parentElement.cells(6).innerText != "-")
        //	{rettifiche = event.srcElement.parentElement.parentElement.cells(6).innerText.replace("-", "");}
        //}
        if ($("#tbArretrati tr:eq(" + index + ")").find('td:eq(6)').text() != "-") { rettifiche = $("#tbArretrati tr:eq(" + index + ")").find('td:eq(6)').text(); }

    }
    else {
        datagrid = "dgDettaglio__ctl";;
        rettifiche = $("#tbArretrati tr:eq(" + index + ")").find('td:eq(8)').text();

        //rettifiche = event.srcElement.parentElement.parentElement.cells(8).innerText;
    }

    document.getElementById('btnSeleziona').style.display = 'none';
    document.getElementById('btnRettifiche').style.display = 'none';
    document.getElementById('btnSeleziona').value = "[Seleziona]";


    if (event.srcElement.checked == true) {
        //event.srcElement.checked = false;
        //event.srcElement.src = "../img/Radio_Unchecked.gif";
        document.getElementById('hdnSelectedIndex').value = '';

    }
    else {
        event.srcElement.checked = true;
        /* event.srcElement.src = "../img/Radio_Checked.gif";*/
        document.getElementById('hdnSelectedIndex').value = event.srcElement.parentElement.parentElement.rowIndex - 1;


        var indice = 3;
        var strindex = "03";
        //var campo = document.getElementById(datagrid + indice + '_imgRadio');

        //while (campo != null) {
        //    if (document.getElementById(datagrid + indice + '_imgRadio').id != IDClient.id) {
        //        document.getElementById(datagrid + indice + '_imgRadio').checked = false;
        //        document.getElementById(datagrid + indice + '_imgRadio').src = "../img/Radio_Unchecked.gif";

        //    }
        //    else {
        //        document.getElementById('btnSeleziona').style.display = 'block';

        //        if (rettifiche.replace(" ", "") != "") {
        //            if (parseFloat(rettifiche.replace(/\./g, "").replace(",", ".")) != 0) {
        //                document.getElementById('btnRettifiche').style.display = 'block';
        //                document.getElementById('btnSeleziona').value = "[Arretrato Originale]";

        //            }
        //        }
        //    }
        //    indice = indice + 1;
        //    if (indice < 10) { strindex = "0" + indice }
        //    else { strindex = indice.toString(); }
        //    campo = document.getElementById(datagrid + indice + '_imgRadio');

        //}

    }
}