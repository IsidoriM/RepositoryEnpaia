// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Amministrativo.ScadenzeDipaBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using System;
using TFI.DAL.Amministrativo;
using TFI.OCM.Amministrativo;

namespace TFI.BLL.Amministrativo
{
  public class ScadenzeDipaBLL
  {
    private ScadenzeDipaDAL scadipaDAL = new ScadenzeDipaDAL();

    public ScadenzaDipaOCM AnnoScaDipa()
    {
      ScadenzaDipaOCM scadenzaDipaOcm = new ScadenzaDipaOCM();
      return this.scadipaDAL.AnnoScaDipa();
    }

    public ScadenzaDipaOCM CercaDataBLL(
      string anno,
      ScadenzaDipaOCM scadipaOCM,
      ref string ErroreMSG)
    {
      if (!string.IsNullOrEmpty(anno))
        this.scadipaDAL.CercaDataDAL(anno, scadipaOCM);
      else
        ErroreMSG = "Selezionare anno";
      return scadipaOCM;
    }

    public ScadenzaDipaOCM GetNewScadipa(ScadenzaDipaOCM scadipaOCM)
    {
      string str1 = "";
      string str2 = "";
      string str3 = "";
      string str4 = "";
      int num = 1;
      for (int index = 0; index < 12; ++index)
      {
        DateTime dateTime;
        switch (num)
        {
          case 1:
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str3 = "01/01/" + dateTime.Year.ToString();
            str1 = "Gennaio";
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str2 = "31/01/" + dateTime.Year.ToString();
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str4 = "25/02/" + dateTime.Year.ToString();
            if (Convert.ToDateTime(str4).DayOfWeek == DayOfWeek.Sunday)
            {
              dateTime = DateTime.Now;
              dateTime = dateTime.AddYears(1);
              str4 = "26/02/" + dateTime.Year.ToString();
              break;
            }
            break;
          case 2:
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str3 = "01/02/" + dateTime.Year.ToString();
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str2 = "28/02/" + dateTime.Year.ToString();
            str1 = "Febbraio";
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str4 = "25/03/" + dateTime.Year.ToString();
            if (Convert.ToDateTime(str4).DayOfWeek == DayOfWeek.Sunday)
            {
              dateTime = DateTime.Now;
              dateTime = dateTime.AddYears(1);
              str4 = "26/03/" + dateTime.Year.ToString();
              break;
            }
            break;
          case 3:
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str3 = "01/03/" + dateTime.Year.ToString();
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str2 = "31/03/" + dateTime.Year.ToString();
            str1 = "Marzo";
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str4 = "25/04/" + dateTime.Year.ToString();
            if (Convert.ToDateTime(str4).DayOfWeek == DayOfWeek.Sunday)
            {
              dateTime = DateTime.Now;
              dateTime = dateTime.AddYears(1);
              str4 = "26/04/" + dateTime.Year.ToString();
              break;
            }
            break;
          case 4:
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str3 = "01/04/" + dateTime.Year.ToString();
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str2 = "30/04/" + dateTime.Year.ToString();
            str1 = "Aprile";
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str4 = "25/05/" + dateTime.Year.ToString();
            if (Convert.ToDateTime(str4).DayOfWeek == DayOfWeek.Sunday)
            {
              dateTime = DateTime.Now;
              dateTime = dateTime.AddYears(1);
              str4 = "26/05/" + dateTime.Year.ToString();
              break;
            }
            break;
          case 5:
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str3 = "01/05/" + dateTime.Year.ToString();
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str2 = "31/05/" + dateTime.Year.ToString();
            str1 = "Maggio";
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str4 = "25/06/" + dateTime.Year.ToString();
            if (Convert.ToDateTime(str4).DayOfWeek == DayOfWeek.Sunday)
            {
              dateTime = DateTime.Now;
              dateTime = dateTime.AddYears(1);
              str4 = "26/06/" + dateTime.Year.ToString();
              break;
            }
            break;
          case 6:
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str3 = "01/06/" + dateTime.Year.ToString();
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str2 = "30/06/" + dateTime.Year.ToString();
            str1 = "Giugno";
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str4 = "25/07/" + dateTime.Year.ToString();
            if (Convert.ToDateTime(str4).DayOfWeek == DayOfWeek.Sunday)
            {
              dateTime = DateTime.Now;
              dateTime = dateTime.AddYears(1);
              str4 = "26/07/" + dateTime.Year.ToString();
              break;
            }
            break;
          case 7:
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str3 = "01/07/" + dateTime.Year.ToString();
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str2 = "31/07/" + dateTime.Year.ToString();
            str1 = "Luglio";
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str4 = "31/08/" + dateTime.Year.ToString();
            if (Convert.ToDateTime(str4).DayOfWeek == DayOfWeek.Sunday)
            {
              dateTime = DateTime.Now;
              dateTime = dateTime.AddYears(1);
              str4 = "01/09/" + dateTime.Year.ToString();
              break;
            }
            break;
          case 8:
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str3 = "01/08/" + dateTime.Year.ToString();
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str2 = "31/08/" + dateTime.Year.ToString();
            str1 = "Agosto";
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str4 = "25/09/" + dateTime.Year.ToString();
            if (Convert.ToDateTime(str4).DayOfWeek == DayOfWeek.Sunday)
            {
              dateTime = DateTime.Now;
              dateTime = dateTime.AddYears(1);
              str4 = "26/09/" + dateTime.Year.ToString();
              break;
            }
            break;
          case 9:
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str3 = "01/09/" + dateTime.Year.ToString();
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str2 = "30/09/" + dateTime.Year.ToString();
            str1 = "Settembre";
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str4 = "25/10/" + dateTime.Year.ToString();
            if (Convert.ToDateTime(str4).DayOfWeek == DayOfWeek.Sunday)
            {
              dateTime = DateTime.Now;
              dateTime = dateTime.AddYears(1);
              str4 = "26/10/" + dateTime.Year.ToString();
              break;
            }
            break;
          case 10:
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str3 = "01/10/" + dateTime.Year.ToString();
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str2 = "31/10/" + dateTime.Year.ToString();
            str1 = "Ottobre";
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str4 = "25/11/" + dateTime.Year.ToString();
            if (Convert.ToDateTime(str4).DayOfWeek == DayOfWeek.Sunday)
            {
              dateTime = DateTime.Now;
              dateTime = dateTime.AddYears(1);
              str4 = "26/11/" + dateTime.Year.ToString();
              break;
            }
            break;
          case 11:
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str3 = "01/11/" + dateTime.Year.ToString();
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str2 = "30/11/" + dateTime.Year.ToString();
            str1 = "Novembre";
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str4 = "31/12/" + dateTime.Year.ToString();
            if (Convert.ToDateTime(str4).DayOfWeek == DayOfWeek.Sunday)
            {
              dateTime = DateTime.Now;
              dateTime = dateTime.AddYears(1);
              str4 = "26/12/" + dateTime.Year.ToString();
              break;
            }
            break;
          case 12:
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str3 = "01/12/" + dateTime.Year.ToString();
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(1);
            str2 = "31/12/" + dateTime.Year.ToString();
            str1 = "Dicembre";
            dateTime = DateTime.Now;
            dateTime = dateTime.AddYears(2);
            str4 = "25/01/" + dateTime.Year.ToString();
            if (Convert.ToDateTime(str4).DayOfWeek == DayOfWeek.Sunday)
            {
              dateTime = DateTime.Now;
              dateTime = dateTime.AddYears(1);
              str4 = "26/01/" + dateTime.Year.ToString();
              break;
            }
            break;
        }
        ++num;
        ScadenzaDipaOCM.DatiScadenzeDipa datiScadenzeDipa = new ScadenzaDipaOCM.DatiScadenzeDipa()
        {
          datinival = str3,
          datfinval = str2,
          mese = str1,
          datsca = str4
        };
        scadipaOCM.listScadenzaDipa.Add(datiScadenzeDipa);
      }
      return scadipaOCM;
    }

    public ScadenzaDipaOCM ModDipaBLL(
      ScadenzaDipaOCM scadipaOCM,
      TFI.OCM.Utente.Utente u,
      ref string ErroreMSG,
      ref string SuccessMSG)
    {
      scadipaOCM = this.scadipaDAL.ModDipaDAL(scadipaOCM, u, ref ErroreMSG, ref SuccessMSG);
      return scadipaOCM;
    }
  }
}
