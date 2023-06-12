// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Iscritto.CertUnicBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using DAL.Iscritto;
using OCM.Iscritto;
using System.Collections.Generic;

namespace TFI.BLL.Iscritto
{
  public class CertUnicBLL
  {
    private readonly CertUnicDAL loginDAL = new CertUnicDAL();

    public List<Cud> Dati() => this.loginDAL.Dati();
  }
}
