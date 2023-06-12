// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Amministrativo.DiffideBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll



using TFI.DAL.Amministrativo;
using TFI.OCM.Amministrativo;

namespace TFI.BLL.Amministrativo
{
  public class DiffideBLL
  {
    private readonly DiffideDAL diffDal = new DiffideDAL();

    public void CercaDiffide(DiffideOCM oCM, string CodPos, string Anno, ref string MsgErrore) => this.diffDal.btnCerca_Click(oCM, CodPos, Anno, ref MsgErrore);
  }
}
