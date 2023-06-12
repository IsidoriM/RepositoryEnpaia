// Decompiled with JetBrains decompiler
// Type: BLL.Iscritto.FondoContoAssegniBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using System.Web;
using TFI.BLL.Utilities;
using TFI.DAL.Iscritto;
using TFI.OCM.Iscritto;

namespace BLL.Iscritto
{
  public class FondoContoAssegniBLL
  {
    private Anagrafica FondoAnag;
    private FondoContoAssegniDAL FondoDAL = new FondoContoAssegniDAL();

    public Anagrafica GetFondoContoAssegni(
      string cf,
      ref string ErroreMSG,
      ref string SuccessMSG,
      ref string InfoMSG)
    {
      this.FondoAnag = new Anagrafica();
      this.FondoAnag = this.FondoDAL.GetFondoContoAssegni(cf, ref ErroreMSG, ref SuccessMSG, ref InfoMSG);
      return this.FondoAnag;
    }

    public Anagrafica RicFondo(
      Anagrafica FondoAnag,
      ref string ErroreMSG,
      ref string SuccessMSG)
    {
      FondoAnag = new Anagrafica();
      string str = HttpContext.Current.Request.Form["divPagamento"].ToString();
      if (!Utils.CheckIban(FondoAnag.Iban) && str == "1")
        ErroreMSG = "Iban Errato";
      else
        FondoAnag = this.FondoDAL.RicFondo(FondoAnag, ref ErroreMSG, ref SuccessMSG);
      return FondoAnag;
    }
  }
}
