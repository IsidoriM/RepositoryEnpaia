// Decompiled with JetBrains decompiler
// Type: TFI.BLL.AziendaConsulente.DenunciaArretratiBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using TFI.DAL.AziendaConsulente;
using TFI.DAL.ConnectorDB;
using TFI.OCM.AziendaConsulente;

namespace TFI.BLL.AziendaConsulente
{
  public class DenunciaArretratiBLL
  {
    public List<DenunciaArretrati> CaricaArretrati(
      string txtAnno,
      string hdnSalva,
      string hdnAnnComp,
      string radio,
      string txtDataDenuncia,
      List<DenunciaArretrati> ListaDenunce,
      ref string ErrorMSG,
      ref string SuccessMSG)
    {
      return new GeneraDenunciaDAL().CaricaArretrati(Convert.ToInt32(txtAnno), hdnSalva, Convert.ToInt32(hdnAnnComp), radio, txtDataDenuncia, ListaDenunce, ref ErrorMSG, ref SuccessMSG);
    }

    public List<DenunciaArretrati> GeneraDenunciaArr(
      string strDataDal,
      string strDataAl,
      string strCodPos,
      string strMat,
      bool blnArretrato = false)
    {
      GeneraDenunciaDAL generaDenunciaDal = new GeneraDenunciaDAL();
      List<ParametriGenerali> listaParametriGen = null;
      return generaDenunciaDal.GeneraDenunciaArr(strDataDal, strDataAl, ref listaParametriGen, strCodPos, strMat);
    }

    public DenunciaArretrati SalvaArretrati(
      string radio,
      string txtDataDenuncia,
      List<DenunciaArretrati> ListaDenunce,
      ref string ErrorMSG,
      ref string SuccessMSG)
    {
      GeneraDenunciaDAL generaDenunciaDal = new GeneraDenunciaDAL();
      DenunciaArretrati denunciaArretrati = new DenunciaArretrati();
      return generaDenunciaDal.SalvaArretrati(radio, txtDataDenuncia, ListaDenunce, ref ErrorMSG, ref SuccessMSG);
    }

    public List<DenunciaArretrati> VediArretrati(
      string codPos,
      int hdnAnno,
      int hdnMese,
      int hdnProden)
    {
      return new GeneraDenunciaDAL().VediArretrati(codPos, hdnAnno, hdnMese, hdnProden);
    }

    public int InsertArretrato(DataLayer db, TFI.OCM.Utente.Utente user, int ANNDEN, int MESDEN)
    {
      try
      {
        return WriteDIPA.WRITE_INSERT_DENTES(db, user.Username, user.CodPosizione, ANNDEN, MESDEN, DateTime.Now.ToString(), user.Tipo.Trim(), string.Empty, string.Empty, "AR", "O", "N", 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, string.Empty, string.Empty, string.Empty, 0, string.Empty, "0", 0, 0.0M, string.Empty, string.Empty, string.Empty, string.Empty, 0M, 0M, string.Empty, 0, 0, "N", string.Empty, string.Empty);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public bool InsertDettaglioArretrato(
      DataLayer db,
      TFI.OCM.Utente.Utente user,
      DenunciaArretrati den,
      int ANNDEN,
      int MESDEN,
      int PRODEN,
      int MAT,
      string DAL,
      string AL,
      Decimal IMPRET,
      Decimal IMPOCC,
      Decimal ALIQUOTA,
      int ANNOCOMP)
    {
      int intProDenDet = 0;
      try
      {
        return WriteDIPA.WRITE_INSERT_DENDET(db, (List<RetribuzioneRDL>) null, (List<ParametriGenerali>) null, user, user.CodPosizione, ANNDEN, MESDEN, PRODEN, MAT, "AR", DAL, AL, string.Empty, Decimal.Round(IMPRET, 0), Decimal.Round(IMPOCC, 0), 0M, 0M, 0M, Convert.ToDecimal(den.impcon), 0.0M, "N", 0, string.Empty, string.Empty, 0.0M, 0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, den.eta65, 0, den.fap, Convert.ToDecimal(den.perfap), den.impfap, (Decimal) den.perpar, den.perapp, den.prorap, Convert.ToInt32(den.codcon), den.procon, den.tipse, den.codloc, Convert.ToInt32(den.proloc), den.codliv, den.codgruass, den.codquacon, ALIQUOTA, den.datnas, ANNOCOMP, "", ref intProDenDet);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public bool UploadArretrato(TFI.OCM.Utente.Utente user, List<string> fileContent, int anno, List<DenunciaArretrati> listaDenunciaArretrati)
    {
      DataLayer db = new DataLayer();
      DateTimeFormatInfo dateTimeFormat = new CultureInfo("it-IT", false).DateTimeFormat;
      int? outcome = new int?();
      int num1 = 0;
      int year = DateTime.Now.Year;
      int month = DateTime.Now.Month;
      string strDataDal = $"01/01/{anno}";
      string strDataAl = $"31/12/{anno}";
      string strProDen = string.Empty;
      string empty2 = string.Empty;
      string empty3 = string.Empty;
      string MAT = string.Empty;
      bool flag1 = true;
      bool flag2 = false;
      bool flag3 = true;
      bool blnCommit = false;
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      try
      {
        RicercaArretrato ricercaArretrato = DenunciaMensileBLL.Ricerca(user.CodPosizione, out outcome, year, month);
        
        db.StartTransaction();
        
        if (ricercaArretrato != null)
          DenunciaMensileDAL.EliminaArretrato(db, user.CodPosizione, year, month, ricercaArretrato.proDen);
        
        // List<DenunciaArretrati> source1 = GeneraDenunciaArr(strDataDal, strDataAl, user.CodPosizione, string.Empty, true);
        List<DenunciaArretrati> source1 = listaDenunciaArretrati;

        // string[] source2 = File.ReadAllLines(fullpath, Encoding.UTF8);
        if (fileContent.Any())
        {
          // db.StartTransaction();
          int proDen = InsertArretrato(db, user, year, month);
          if (proDen > 0)
            strProDen = proDen.ToString();
          foreach (string str in fileContent)
          {
            if (!string.IsNullOrWhiteSpace(strProDen))
            {
              dictionary.Add("codiceFiscale", str.Substring(0, 16));
              dictionary.Add("progressivoDipendente", str.Substring(16, 2));
              dictionary.Add("tipoRetribuzione", str.Substring(18, 1));
              dictionary.Add("periodoDal", str.Substring(19, 4));
              dictionary.Add("periodoAl", str.Substring(23, 4));
              dictionary.Add("aliquota", str.Substring(27, 5).Insert(2, ","));
              dictionary.Add("retribuzioneImponibile", str.Substring(32, 9).Insert(7, ","));
              dictionary.Add("retribuzioneOccasionale", str.Substring(41, 9).Insert(7, ","));
              dictionary.Add("contributiFigurativa", str.Substring(50, 9).Insert(7, ","));
              dictionary.Add("annoCompetenza", anno.ToString());
              decimal aliquota = Convert.ToDecimal(dictionary["aliquota"]);
              decimal IMPRET = decimal.Round(Convert.ToDecimal(dictionary["retribuzioneImponibile"]), 2);
              decimal IMPOCC = decimal.Round(Convert.ToDecimal(dictionary["retribuzioneOccasionale"]), 2);
              decimal.Round(Convert.ToDecimal(dictionary["contributiFigurativa"]), 2);
              decimal num2 = (IMPRET + IMPOCC) / 100M * aliquota;
              string DAL = $"{anno}-{dictionary["periodoDal"].Substring(0, 2)}-{dictionary["periodoDal"].Substring(2)}";
              string AL = $"{anno}-{dictionary["periodoAl"].Substring(0, 2)}-{dictionary["periodoAl"].Substring(2)}";
              MAT = DenunciaMensileBLL.GetMatricolaByCodFiscale(dictionary["codiceFiscale"]);
              if (string.IsNullOrWhiteSpace(MAT))
              {
                flag1 = false;
                flag2 = true;
              }
              else
              {
                List<DenunciaArretrati> list = source1.Where(d => d.mat.ToString() == MAT).ToList();
                if (list.Count == 0)
                {
                  flag1 = false;
                  flag2 = true;
                }
                else if (aliquota == 0M)
                {
                  flag1 = false;
                  flag2 = true;
                }
                else if (list.First().datadal == DAL && list.First().dataal != AL && list.Last().dataal != AL)
                {
                  flag1 = false;
                  flag2 = true;
                }
                if (flag1)
                {
                  flag3 = InsertDettaglioArretrato(db, user, list.First(), year, month, proDen, Convert.ToInt32(MAT), DAL, AL, IMPRET, IMPOCC, aliquota, anno);
                  if (flag3)
                    ++num1;
                  else
                    flag3 = true;
                }
                else
                  flag2 = true;
              }
            }
            dictionary.Clear();
          }
          if (flag3 && !flag2)
          {
            string strSql1 = $"SELECT SUM(IMPRET) AS IMPRET, SUM(IMPOCC) AS IMPOCC, SUM(IMPCON) AS IMPCON, SUM(IMPABB) AS IMPABB, SUM(IMPASSCON) AS IMPASSCON FROM DENDET WHERE CODPOS = '{user.CodPosizione}' AND ANNDEN = '{year}' AND MESDEN = '{month}' AND PRODEN = '{proDen}' AND TIPMOV = 'AR'";
            DataTable dataTable = db.GetDataTable(strSql1);
            if (dataTable.Rows.Count > 0)
            {
              string strSql2 = "UPDATE DENTES SET " +
                               $"IMPRET = {dataTable.Rows[0]["IMPRET"].ToString().Replace(",", ".")}, " +
                               $"IMPOCC = {dataTable.Rows[0]["IMPOCC"].ToString().Replace(",", ".")}, " +
                               $"IMPCON = {dataTable.Rows[0]["IMPCON"].ToString().Replace(",", ".")}, " +
                               $"IMPABB = {dataTable.Rows[0]["IMPABB"].ToString().Replace(",", ".")}, " +
                               $"IMPASSCON = {dataTable.Rows[0]["IMPASSCON"].ToString().Replace(",", ".")}, " +
                               $"NUMRIGDET = {num1} " + 
                               $"WHERE CODPOS = {user.CodPosizione} " +
                               $"AND ANNDEN = '{year}' " +
                               $"AND MESDEN = '{month}' " +
                               $"AND PRODEN = {strProDen} " +
                               "AND TIPMOV = 'AR'";
              blnCommit = db.WriteTransactionData(strSql2, CommandType.Text);
              if (blnCommit)
              {
                string strSql3 = "UPDATE DENTES SET " +
                                 $"IMPADDREC = ROUND((IMPCON / 100) * {DenunciaMensileBLL.GetImportoParametro(5, DateTime.Now.GetDateTimeFormats(dateTimeFormat)[0]).ToString().Replace(",", ".")}, 2) " +
                                 $"WHERE CODPOS = {user.CodPosizione} " +
                                 $"AND ANNDEN = '{year}' " +
                                 $"AND MESDEN = '{month}' " +
                                 $"AND PRODEN = {strProDen} " +
                                 "AND TIPMOV = 'DP'";
                blnCommit = db.WriteTransactionData(strSql3, CommandType.Text);
                if (blnCommit)
                {
                  string strSql4 = "UPDATE DENTES SET " +
                                   "IMPDIS = IMPCON + IMPASSCON + IMPADDREC + IMPABB " +
                                   $"WHERE CODPOS = {user.CodPosizione} " +
                                   $"AND ANNDEN = '{year}' " +
                                   $"AND MESDEN = '{month}' " +
                                   $"AND PRODEN = {strProDen} " +
                                   "AND TIPMOV = 'AR'";
                  blnCommit = db.WriteTransactionData(strSql4, CommandType.Text);
                }
              }
            }
          }
          db.EndTransaction(blnCommit);
        }
        return blnCommit && !flag2;
      }
      catch (Exception ex)
      {
        if (db != null && db.isInTransaction)
          db.EndTransaction(false);
        return false;
      }
      // finally
      // {
      //   File.Delete(fullpath);
      // }
    }
  }
}
