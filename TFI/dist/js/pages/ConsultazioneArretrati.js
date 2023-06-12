
var anno_comp = 0;
function checkFields() {
    var blnSubmit = false;
    if (document.getElementById("txtAnno") != null) {
        if (document.getElementById("txtAnno").value.replace(' ', '') == '') {
            alert("Inserire l'anno di competenza!");
            document.getElementById("txtAnno").select();
            event.preventDefault(); //.returnValue = null;
            return;
        }
        else { anno_comp = document.getElementById("txtAnno").value; }
    }
    else {
        anno_comp = event.srcElement.value;
        blnSubmit = true;
    }
    pageClose(1);
    if (blnSubmit == true) { document.Form1.submit(); }
}

function checkValue(ctl, ctl_id) {
    var ctl_compare = document.getElementById(ctl_id);
    if ((parseFloat(ctl.value)) > (parseFloat(ctl_compare.value))) {
        alert("La Retribuzione Occasionale non può essere maggiore dell'imponibile!")
        ctl.value = '0,00';
        ctl.select();
    }
    sum(ctl, 2);
}

function checkDate() {
    if (document.getElementById("txtDataDenuncia").value.replace(' ', '') == '') {
        alert("Inserire la data denuncia!");
        document.getElementById("txtDataDenuncia").select();
        event.preventDefault(); //.returnValue = null;
        return false;
    }
    else { return true; }
}

function sum(ctl, index) {
    var tot = 0.00;
    var lblTot;

    switch (index) {
        case 1:
            lblTot = document.getElementById("lblTotRetribuzioni");
            break;

        case 2:
            lblTot = document.getElementById("lblTotOccasionali");
            break;
    }
    tot = parseFloat("0" + lblTot.innerText.replace(",", "."));
    tot -= parseFloat(ctl.previous_value.replace(",", "."))
    tot += parseFloat(ctl.value.replace(",", "."));
    lblTot.innerText = tot.toFixed(2).toString().replace(".", ",");
    if (lblTot.innerText.indexOf(",") == -1) { lblTot.innerText += ",00"; }
    ctl.previous_value = ctl.value;
    document.getElementById("hdnTotRetribuzioni").value = document.getElementById("lblTotRetribuzioni").innerText;
    document.getElementById("hdnTotOccasionali").value = document.getElementById("lblTotOccasionali").innerText;
    document.getElementById("hdnTotContributi").value = document.getElementById("lblTotContributi").innerText;
}

function pageClose(index) {
    var msg;
    switch (index) {
        case 1:
            msg = "Variando l'anno di competenza i dati inseriti e non salvati andranno persi. Salvare i dati inseriti?";
            if (anno_comp == document.getElementById("hdnAnno").value) { return; }
            break;
        case 2:
            msg = "Chiudendo la pagina i dati inseriti e non salvati andranno persi. Salvare i dati inseriti?";
            break;
        case 3:
            msg = "Spostandosi sulla pagina dei totali i dati inseriti e non salvati andranno persi. Salvare i dati inseriti?";
            break;
    }
    if (document.getElementById("pnlTotali") != null) {
        if (document.getElementById("lblTotOccasionali").innerText != "0,00" || document.getElementById("lblTotRetribuzioni").innerText != "0,00") {
            if (confirm(msg) == true) {
                if (checkDate() == true) { document.getElementById("hdnSalva").value = "S"; }
            }
        }
    }
}
