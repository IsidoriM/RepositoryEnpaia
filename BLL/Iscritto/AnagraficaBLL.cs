// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Iscritto.AnagraficaBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using System;
using TFI.DAL.Iscritto;
using TFI.OCM.Iscritto;

namespace TFI.BLL.Iscritto
{
    public class AnagraficaBLL
    {
        private readonly AnagraficaDAL anagDAL = new AnagraficaDAL();

        public Anagrafica GetAnagrafica(string cf) => this.anagDAL.GetAnagrafica(cf);
        public Anagrafica ModificaAnagrafica(
          Anagrafica ModAnagrafica,
          ref string ErrorMsg,
          ref string SuccesMsg)
        {
            if (ModAnagrafica.EmailCert is not null)
            {
                if (ModAnagrafica.EmailCert.Contains("@pec")==false && (ModAnagrafica.EmailCert.Contains("@legalmail") == false))
                {
                    ErrorMsg = "Errore Pec non valida";
                    return (Anagrafica)null; 
                }
            }
            if (ModAnagrafica.StatoEsteroResidenza == "0")
            {
                ModAnagrafica.StatoEsteroResidenza = null;
            }
            string titoloStudio = ModAnagrafica.TitoloStudio;
            Decimal num = 1M;
            if (!(titoloStudio == "LICENZA ELEMENTARE"))
            {
                if (!(titoloStudio == "LICENZA MEDIA"))
                {
                    if (!(titoloStudio == "LAUREA"))
                    {
                        if (titoloStudio == "DIPLOMA")
                            num = 4M;
                    }
                    else
                        num = 3M;
                }
                else
                    num = 2M;
            }
            else
                num = 1M;
            ModAnagrafica.CodTitstu = num;
            return this.anagDAL.ModificaAnagrafica(ModAnagrafica, ref ErrorMsg, ref SuccesMsg);
        }
    }
}
