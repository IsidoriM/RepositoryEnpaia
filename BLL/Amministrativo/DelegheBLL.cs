// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Amministrativo.DelegheBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll


using TFI.DAL.Amministrativo;
using TFI.OCM.Amministrativo;

namespace TFI.BLL.Amministrativo
{
  public class DelegheBLL
  {
    private readonly DelegheDAL delegheDal = new DelegheDAL();

    public DelegheOCM AssNaz() => this.delegheDal.AssNaz();

    public DelegheOCM SearchDeleghe(
      string PosAz,
      string Partitaiva,
      string tipass,
      ref string ErroreMSG,
      ref string SuccessMSG)
    {
      DelegheOCM delegheOcm = new DelegheOCM();
      if (!string.IsNullOrEmpty(PosAz) || !string.IsNullOrEmpty(Partitaiva) || !string.IsNullOrEmpty(tipass))
        return this.delegheDal.SearcheDeleghe(PosAz, Partitaiva, tipass, ref ErroreMSG, ref SuccessMSG);
      ErroreMSG = "Inserire almeno un campo di ricerca";
      return (DelegheOCM) null;
    }

    public DelegheOCM GetCaricaIva(ref string MSGErrore, string codFis, string ParIva) => this.delegheDal.CARICA_IVA(ref MSGErrore, codFis, ParIva);

    public DelegheOCM GetDenominazione(string codNaz, ref string MSGErrore) => this.delegheDal.caricaDenominazione(codNaz, ref MSGErrore);

    public DelegheOCM GetAssociazione(
      DelegheOCM delegheOCM,
      string codTer,
      string codNaz,
      string codpos,
      string assnaz,
      string asster,
      string ragsoc)
    {
      return this.delegheDal.carica_Dati_Associazione(delegheOCM, codTer, codNaz, codpos, assnaz, asster, ragsoc);
    }
  }
}
