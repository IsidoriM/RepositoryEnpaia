// Decompiled with JetBrains decompiler
// Type: BLL.UtenteEnpaia.UtenteEnpaiaBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using TFI.DAL.UtenteEnpaia;
using System.Collections.Generic;
using TFI.DAL.UtenteEnpaia;
using TFI.OCM.AziendaConsulente;

namespace BLL.UtenteEnpaia
{
  public class UtenteEnpaiaBLL
  {
    private readonly UtenteEnpaiaDAL utenDAL = new UtenteEnpaiaDAL();

    public List<Azienda> GetAziendeEnpaia(
      string posizione,
      string ragioneSociale,
      string codiceFiscale,
      string partitaIVA)
    {
      return new UtenteEnpaiaDAL().GetAziendeEnpaia(posizione, ragioneSociale, codiceFiscale, partitaIVA);
    }
  }
}
