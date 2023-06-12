// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Utilities.Utils
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.Amministrativo;

namespace TFI.BLL.Utilities
{
    public class Utils
    {
        private static readonly ILog log = LogManager.GetLogger("RollingFile");
        private static readonly ILog TrackLog = LogManager.GetLogger("Track");

        private static string[] ValidFileExtensions = { ".pdf", ".jpeg", ".jpg", ".png" };

        public static bool IsNumeric(string text) => int.TryParse(text, out int _);

        public static bool IsDecimal(string text) => Decimal.TryParse(text, out Decimal _);

        public static Dictionary<int, string> GetMesi() => new Dictionary<int, string>()
    {
      {
        0,
        "Seleziona"
      },
      {
        1,
        "Gennaio"
      },
      {
        2,
        "Febbraio"
      },
      {
        3,
        "Marzo"
      },
      {
        4,
        "Aprile"
      },
      {
        5,
        "Maggio"
      },
      {
        6,
        "Giugno"
      },
      {
        7,
        "Luglio"
      },
      {
        8,
        "Agosto"
      },
      {
        9,
        "Settembre"
      },
      {
        10,
        "Ottobre"
      },
      {
        11,
        "Novembre"
      },
      {
        12,
        "Dicembre"
      }
    };

        public static int MonthToDays(int mese, string anno)
        {
            int days = 0;
            switch (mese)
            {
                case 1:
                    days = 31;
                    break;
                case 2:
                    days = DateTime.IsLeapYear(Convert.ToInt32(anno)) ? 29 : 28;
                    break;
                case 3:
                    days = 31;
                    break;
                case 4:
                    days = 30;
                    break;
                case 5:
                    days = 31;
                    break;
                case 6:
                    days = 30;
                    break;
                case 7:
                    days = 31;
                    break;
                case 8:
                    days = 31;
                    break;
                case 9:
                    days = 30;
                    break;
                case 10:
                    days = 31;
                    break;
                case 11:
                    days = 30;
                    break;
                case 12:
                    days = 31;
                    break;
            }
            return days;
        }

        public static List<int> GetIntegerListFromTo(int from, int to)
        {
            List<int> integerListFromTo = new List<int>();
            int num = from < to ? to : from;
            for (int index = num == to ? from : to; num >= index; --num)
                integerListFromTo.Add(num);
            return integerListFromTo;
        }

        public static string FixMese(int mese) => mese.ToString().PadLeft(2, '0');

        public static bool CheckIban(string Iban)
        {
            if (string.IsNullOrWhiteSpace(Iban))
                return false;
            Iban = Iban.Replace(" ", string.Empty).ToUpper();
            if (!Regex.IsMatch(Iban, "^(AL|al)[0-9]{2}[0-9]{8}[a-zA-Z0-9]{16}$|^(AD|ad)[0-9]{2}[0-9]{8}[a-zA-Z0-9]{12}$|^(AT|at)[0-9]{2}[0-9]{16}$|^(AZ|az)[0-9]{2}[a-zA-Z]{4}[a-zA-Z0-9]{20}$|^(BH|bh)[0-9]{2}[a-zA-Z]{4}[a-zA-Z0-9]{14}$|^(BY|by)[0-9]{2}[a-zA-Z0-9]{4}[0-9]{4}[a-zA-Z0-9]{16}$|^(BE|be)[0-9]{2}[0-9]{12}$|^(BA|ba)[0-9]{2}[0-9]{16}$|^(BR|br)[0-9]{2}[0-9]{23}[a-zA-Z][a-zA-Z0-9]$|^(BG|bg)[0-9]{2}[a-zA-Z]{4}[0-9]{6}[a-zA-Z0-9]{8}$|^(CR|cr)[0-9]{2}[0-9]{18}$|^(HR|hr)[0-9]{2}[0-9]{17}$|^(CY|cy)[0-9]{2}[0-9]{8}[a-zA-Z0-9]{16}$|^(CZ|cz)[0-9]{2}[0-9]{20}$|^(DK|dk)[0-9]{2}[0-9]{14}$|^(DO|do)[0-9]{2}[a-zA-Z]{4}[0-9]{20}$|^(TL|tl)[0-9]{2}[0-9]{19}$|^(EG|eg)[0-9]{2}[0-9]{25}$|^(SV|sv)[0-9]{2}[a-zA-Z]{4}[0-9]{20}$|^(EE|ee)[0-9]{2}[0-9]{16}$|^(FO|fo)[0-9]{2}[0-9]{14}$|^(FI|fi)[0-9]{2}[0-9]{14}$|^(FR|fr)[0-9]{2}[0-9]{10}[a-zA-Z0-9]{11}[0-9]{2}$|^(GE|ge)[0-9]{2}[a-zA-Z0-9]{2}[0-9]{16}$|^(DE|de)[0-9]{2}[0-9]{18}$|^(GI|gi)[0-9]{2}[a-zA-Z]{4}[a-zA-Z0-9]{15}$|^(GR|gr)[0-9]{2}[0-9]{7}[a-zA-Z0-9]{16}$|^(GL|gl)[0-9]{2}[0-9]{14}$|^(GT|gt)[0-9]{2}[a-zA-Z0-9]{24}$|^(HU|hu)[0-9]{2}[0-9]{24}$|^(IS|is)[0-9]{2}[0-9]{22}$|^(IQ|iq)[0-9]{2}[a-zA-Z]{4}[0-9]{15}$|^(IE|ie)[0-9]{2}[a-zA-Z0-9]{4}[0-9]{14}$|^(IL|il)[0-9]{2}[0-9]{19}$|^(IT|it)[0-9]{2}[a-zA-Z][0-9]{10}[a-zA-Z0-9]{12}$|^(JO|jo)[0-9]{2}[a-zA-Z]{4}[0-9]{22}$|^(KZ|kz)[0-9]{2}[0-9]{3}[a-zA-Z0-9]{13}$|^(XK|xk)[0-9]{2}[0-9]{16}$|^(KW|kw)[0-9]{2}[a-zA-Z]{4}[a-zA-Z0-9]{22}$|^(LV|lv)[0-9]{2}[a-zA-Z]{4}[a-zA-Z0-9]{13}$|^(LB|lb)[0-9]{2}[0-9]{4}[a-zA-Z0-9]{20}$|^(LY|ly)[0-9]{2}[0-9]{21}$|^(LI|li)[0-9]{2}[0-9]{5}[a-zA-Z0-9]{12}$|^(LT|lt)[0-9]{2}[0-9]{16}$|^(LU|lu)[0-9]{2}[0-9]{3}[a-zA-Z0-9]{13}$|^(MK|mk)[0-9]{2}[0-9]{3}[a-zA-Z0-9]{10}[0-9]{2}$|^(MT|mt)[0-9]{2}[a-zA-Z]{4}[0-9]{5}[a-zA-Z0-9]{18}$|^(MR|mr)[0-9]{2}[0-9]{23}$|^(MU|mu)[0-9]{2}[a-zA-Z]{4}[0-9]{19}[a-zA-Z]{3}$|^(MC|mc)[0-9]{2}[0-9]{10}[a-zA-Z0-9]{11}[0-9]{2}$|^(MD|md)[0-9]{2}[a-zA-Z0-9]{20}$|^(ME|me)[0-9]{2}[0-9]{18}$|^(NL|nl)[0-9]{2}[a-zA-Z]{4}[0-9]{10}$|^(NO|no)[0-9]{2}[0-9]{11}$|^(PK|pk)[0-9]{2}[a-zA-Z0-9]{4}[0-9]{16}$|^(PS|ps)[0-9]{2}[a-zA-Z0-9]{4}[0-9]{21}$|^(PL|pl)[0-9]{2}[0-9]{24}$|^(PT|pt)[0-9]{2}[0-9]{21}$|^(QA|qa)[0-9]{2}[a-zA-Z]{4}[a-zA-Z0-9]{21}$|^(RO|ro)[0-9]{2}[a-zA-Z]{4}[a-zA-Z0-9]{16}$|^(LC|lc)[0-9]{2}[a-zA-Z]{4}[a-zA-Z0-9]{24}$|^(SM|sm)[0-9]{2}[a-zA-Z][0-9]{10}[a-zA-Z0-9]{12}$|^(ST|st)[0-9]{2}[0-9]{21}$|^(SA|sa)[0-9]{2}[0-9]{2}[a-zA-Z0-9]{18}$|^(RS|rs)[0-9]{2}[0-9]{18}$|^(SC|sc)[0-9]{2}[a-zA-Z]{4}[0-9]{20}[a-zA-Z]{3}$|^(SK|sk)[0-9]{2}[0-9]{20}$|^(SI|si)[0-9]{2}[0-9]{15}$|^(ES|es)[0-9]{2}[0-9]{20}$|^(SD|sd)[0-9]{2}[0-9]{14}$|^(SE|se)[0-9]{2}[0-9]{20}$|^(CH|ch)[0-9]{2}[0-9]{5}[a-zA-Z0-9]{12}$|^(TN|tn)[0-9]{2}[0-9]{20}$|^(TR|tr)[0-9]{2}[0-9]{5}[a-zA-Z0-9]{17}$|^(UA|ua)[0-9]{2}[0-9]{6}[a-zA-Z0-9]{19}$|^(AE|ae)[0-9]{2}[0-9]{19}$|^(GB|gb)[0-9]{2}[a-zA-Z]{4}[0-9]{14}$|^(VA|va)[0-9]{2}[0-9]{18}$|^(VG|vg)[0-9]{2}[a-zA-Z0-9]{4}[0-9]{16}$"))
                return false;
            Iban = Iban.Substring(4) + Iban.Substring(0, 4);
            int num = 0;
            foreach (char c in Iban)
                num = !char.IsLetter(c) ? (10 * num + ((int)c - 48)) % 97 : (100 * num + ((int)c - 55)) % 97;
            return num == 1;
        }

        public static bool CheckAbiAndCab(string iban)
        {
            if (string.IsNullOrWhiteSpace(iban))
                return false;
            iban = iban.Replace(" ", string.Empty).ToUpper();

            if (!iban.StartsWith("IT"))
                return true;

            string abi = iban.Substring(5, 5);
            string cab = iban.Substring(10, 5);

            return Utile.IsAbiKnown(abi) && Utile.IsCabKnown(cab);
        }

        public static List<TFI.OCM.Amministrativo.GestioneRapportiLavoroIscrittiOCM.TitoliStudio> GetTitoliStudio()
        {
            DataLayer objDataAccess = new DataLayer();
            GestioneRapportiLavoroIscrittiOCM rdl = new GestioneRapportiLavoroIscrittiOCM();
            var dal = new ModGetDati();
            dal.Module_Carica_TITSTU(objDataAccess, rdl);
            return rdl.listTitoli;
        }

        public static string GetIban(string codFiscale)
        {
            string ibanInTblQuery = $"SELECT IBAN, DATINI FROM TBIBAN WHERE MAT = {GetMatricolaIscritto(codFiscale)} ORDER BY DATFIN DESC LIMIT 1";
            return new DataLayer().Get1ValueFromSQL(ibanInTblQuery, CommandType.Text);
        }

        public static string GetMatricolaIscritto(string username) => new DataLayer().Get1ValueFromSQL("SELECT MAT FROM ISCT WHERE CODFIS = '" + username + "'", CommandType.Text);

        public static bool FileExtensionsIsValid(string filename) => ValidFileExtensions.Contains(Path.GetExtension(filename).ToLower());

    }
}
