// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Iscritto.ProsPagBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using System.Collections.Generic;
using TFI.DAL.Iscritto;
using TFI.OCM;

namespace TFI.BLL.Iscritto
{
  public class ProsPagBLL
  {
    private readonly ProsPagDAL tfr = new ProsPagDAL();

    public List<Prospetti> GeneraProspettiPagamento(ref string ErroreMSG)
    {
      return this.tfr.GeneraProspettiPagamento(ref ErroreMSG);
    }
  }
}
