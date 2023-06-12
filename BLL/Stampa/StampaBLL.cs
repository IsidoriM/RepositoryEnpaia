// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Stampa.StampaBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using System;
using System.IO;
using TFI.DAL.ConnectorDB;
using TFI.OCM.Stampa;

namespace TFI.BLL.Stampa
{
  public class StampaBLL
  {
    public static string ErrorMessage;

    public static bool StampaRicevuta_Denunce(
      TFI.OCM.Utente.Utente utente,
      string connProtocollo,
      ref string fileName,
      string ragioneSociale,
      string ricevutaDIPA,
      string prot,
      string numPro,
      string datPro,
      int anno,
      int mese,
      int proDen,
      string basePath,
      string tipMov)
    {
      DataLayer dataLayer = new DataLayer();
      bool flag = false;
      try
      {
        return true;
      }
      catch (Exception ex)
      {
        if (flag)
          dataLayer.EndTransaction(false);
        StampaBLL.ErrorMessage = "Attenzione si è verificato il seguente errore:" + ex.Message;
        return false;
      }
    }

    public static MemoryStream GetPdfMethods(string tipMov, DatiDiStampa dati)
    {
      try
      {
        return (MemoryStream) null;
      }
      catch (Exception ex)
      {
        StampaBLL.ErrorMessage = "Attenzione si è verificato il seguente errore: " + ex.Message;
        return (MemoryStream) null;
      }
    }
  }
}
