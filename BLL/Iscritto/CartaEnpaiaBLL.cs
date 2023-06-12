// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Iscritto.CartaEnpaiaBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using TFI.DAL.Iscritto;
using TFI.OCM.Iscritto;

namespace TFI.BLL.Iscritto
{
  public class CartaEnpaiaBLL
  {
    private Anagrafica AnagCarta = new Anagrafica();
    private CartaDAL cartaDAL = new CartaDAL();

    public Anagrafica getCartaEnapia(string cf)
    {
      this.AnagCarta = this.cartaDAL.getCartaEnapia(cf);
      return this.AnagCarta;
    }

    public Anagrafica RicCartaEnpaia(
      Anagrafica AnagCarta,
      ref string ErroreMSG,
      ref string SuccessMSG)
    {
      AnagCarta = this.cartaDAL.RicCartaEnpaia(AnagCarta, ref ErroreMSG, ref SuccessMSG);
      return AnagCarta;
    }
  }
}
