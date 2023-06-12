// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Amministrativo.ModelloPrevBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using TFI.DAL.Amministrativo;
using TFI.OCM.Amministrativo;

namespace TFI.BLL.Amministrativo
{
  public class ModelloPrevBLL
  {
    private readonly ModelloPrevDAL modelloPrevDAL = new ModelloPrevDAL();

    public ModelloPrevOCM CaricaDatiModelloPrev(
      ModelloPrevOCM.DatiModello datiModello,
      ref string ErrorMSG,
      ref string SuccessMSG)
    {
      ModelloPrevOCM modelloPrevOcm = new ModelloPrevOCM();
      if (datiModello != null)
        return this.modelloPrevDAL.CaricaDatiModelloPrev(datiModello, ref ErrorMSG, ref SuccessMSG);
      ErrorMSG = "Inserire almeno un campo di ricerca";
      return (ModelloPrevOCM) null;
    }
  }
}
