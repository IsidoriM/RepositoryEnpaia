// Decompiled with JetBrains decompiler
// Type: TFI.BLL.AziendaConsulente.ConsultazioneArretratiBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using TFI.DAL.AziendaConsulente;
using TFI.OCM.AziendaConsulente;

namespace TFI.BLL.AziendaConsulente
{
  public class ConsultazioneArretratiBLL
  {
    public static ConsultazioneArretrati LoadData(
      string CodPos,
      string anno,
      string mese,
      string parm,
      string staDen,
      string proDen,
      string mat,
      string anncom)
    {
      string str = "";
      ConsultazioneArretrati totali = ConsultazioneArretratiDAL.GetTotali(CodPos, anno, mese, parm, staDen, proDen, mat, anncom, new ConsultazioneArretrati()
      {
        colMesVisible = false,
        colAnnoVisible = true,
        listaSelezione = ConsultazioneArretratiDAL.LoadData(CodPos, anno, mese, parm, staDen, proDen, mat, anncom)
      });
      if (anncom != "" && anncom != null)
        totali.colAnnComVisible = true;
      if (anncom == "" && mat == "")
      {
        totali.colMesVisible = false;
        totali.colAnnoVisible = false;
      }
      if (staDen == "A" || str == "S")
        totali.btnTotaliVisible = true;
      if (totali.listaSelezione.Count > 0)
      {
        foreach (DenunciaArretrati denunciaArretrati in totali.listaSelezione)
        {
          totali.codmodpag = denunciaArretrati.codmodpag;
          totali.intestazione = denunciaArretrati.lblIntestazione;
        }
      }
      return totali;
    }
  }
}
