﻿// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Iscritto.PrivacyBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using DAL.Iscritto;
using TFI.OCM.Iscritto;

namespace TFI.BLL.Iscritto
{
  public class PrivacyBLL
  {
    private readonly PrivacyDAL PriDAL = new PrivacyDAL();

    public void GestionePrivacy() => this.PriDAL.GestionePrivacy();

    public void GestionePrivacy(Anagrafica a, ref string ErroreMSG, ref string SuccessMSG) => this.PriDAL.GestionePrivacy(a, ref ErroreMSG, ref SuccessMSG);
  }
}
