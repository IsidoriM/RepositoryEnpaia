// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Iscritto.ListaConvenzioniBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using System.Collections.Generic;
using TFI.DAL.Iscritto;
using TFI.OCM.Iscritto;

namespace TFI.BLL.Iscritto
{
  public class ListaConvenzioniBLL
  {
    private readonly ListaConvenzioniDAL iscrittoDAL = new ListaConvenzioniDAL();

    public List<ListaConvenzioniIscritto> Convenzioni() => this.iscrittoDAL.Convenzioni();
  }
}
