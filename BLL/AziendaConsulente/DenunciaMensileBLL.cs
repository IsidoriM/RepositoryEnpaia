// Decompiled with JetBrains decompiler
// Type: TFI.BLL.AziendaConsulente.DenunciaMensileBLL
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using OCM.TFI.OCM;
using OCM.TFI.OCM.AziendaConsulente;
using OCM.TFI.OCM.Protocollo;
using TFI.BLL.DllProtocollo;
using TFI.BLL.Utilities;
using TFI.BLL.Utilities.PagoPa;
using TFI.BLL.Utilities.Pdf;
using TFI.DAL.AziendaConsulente;
using TFI.DAL.ConnectorDB;
using TFI.DAL.Utilities;
using TFI.OCM.AziendaConsulente;
using TFI.OCM.Utente;
using OCM.TFI.OCM.Utilities;
using TFI.OCM.Utilities;

namespace TFI.BLL.AziendaConsulente
{
    public class DenunciaMensileBLL
    {
        private static readonly ILog log = LogManager.GetLogger("RollingFile");
        private static readonly ILog TrackLog = LogManager.GetLogger("Track");
        public static string ErrorMessage = string.Empty;
        public static string WarningMessage = string.Empty;
        public static string SuccessMessage = string.Empty;
        public static int Outcome;
        private const int NUMERO_MESI = 12;
        private static List<ParametriGenerali> listaParametriGenerali;
        private static readonly ProtocolloDll _protocolloDll = new ();

        public static bool CiSonoRettificheDIPA(string codPos, int anno, int mese, out int? outcome)
        {
            try
            {
                outcome = new int?(0);
                return DenunciaMensileDAL.CiSonoRettificheDIPA(codPos, anno, mese);
            }
            catch (Exception ex)
            {
                outcome = new int?(1);
                DenunciaMensileBLL.log.Info((object)string.Format(
                    "[TFI.BLL] : DenunciaMensile_Lettura - La funzione CiSonoRettificheDIPA() ha generato un'eccezione in data: {0}",
                    (object)DateTime.Now));
                DenunciaMensileBLL.ErrorMessage = "L'applicazione ha generato un'eccezione " + ex.Message;
                return false;
            }
        }

        public static List<RetribuzioneRDL> PreparaDenunciaMensile(
            TFI.OCM.Utente.Utente utente,
            int proDen,
            string data_dal,
            string data_al,
            string codPos,
            string mat = "",
            bool isArretrato = false,
            int annFia = 0,
            int mesFia = 0,
            int idDipa = 0)
        {
            int num1 = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int IDPOLIZZA = 0;
            string empty1 = string.Empty;
            string empty2 = string.Empty;
            string empty3 = string.Empty;
            string tipSpe = string.Empty;
            string empty4 = string.Empty;
            string qualifica = string.Empty;
            string livello = string.Empty;
            Decimal impSca = 0.0M;
            Decimal impTraEco = 0.0M;
            Decimal perFap = 0.0M;
            Decimal proLoc = 0.0M;
            Decimal proCon = 0.0M;
            string str1 = "N";
            string empty5 = string.Empty;
            bool? isImportoPrev = new bool?();
            bool flag = false;
            DataLayer db = new DataLayer();
            List<RetribuzioneRDL> listaReport = (List<RetribuzioneRDL>)null;
            try
            {
                int? outcome;
                //string payments = DenunciaMensileDAL.GetPayments(codPos, out outcome);
                //int? nullable1 = outcome;
                //int num5 = 2;
                //if (!(nullable1.GetValueOrDefault() == num5 & nullable1.HasValue))
                {
                    outcome = new int?();
                    listaReport = new List<RetribuzioneRDL>();
                    DateTime dateTime1 = DateTime.Parse(data_dal);
                    int month1 = dateTime1.Month;
                    dateTime1 = DateTime.Parse(data_al);
                    int month2 = dateTime1.Month;
                    if (month1 == month2)
                    {
                        dateTime1 = DateTime.Parse(data_dal);
                        int year = dateTime1.Year;
                        dateTime1 = DateTime.Parse(data_dal);
                        int month3 = dateTime1.Month;
                        num1 = DateTime.DaysInMonth(year, month3);
                    }

                    List<AnagraficaLavorativa> personalAndWorkData =
                        DenunciaMensileDAL.GetPersonalAndWorkData(annFia.ToString(), data_dal, data_al, codPos,
                            out outcome);
                    int? nullable2 = outcome;
                    int num6 = 2;
                    if (nullable2.GetValueOrDefault() == num6 & nullable2.HasValue)
                    {
                        DenunciaMensileBLL.ErrorMessage =
                            "La funzione GetPersonalAndWorkData() non ha restituito risultati";
                        return (List<RetribuzioneRDL>)null;
                    }

                    db.StartTransaction();
                    flag = true;
                    outcome = new int?();
                    foreach (AnagraficaLavorativa anagraficaLavorativa in personalAndWorkData)
                    {
                        string codFiscale = anagraficaLavorativa.CodFiscale;
                        codPos = anagraficaLavorativa.CodPos;
                        int mat1 = anagraficaLavorativa.Mat;
                        int proRap = anagraficaLavorativa.ProRap;
                        string nome = anagraficaLavorativa.Nome;
                        DenunciaMensileBLL.listaParametriGenerali =
                            DenunciaMensileDAL.GetParametriGenerali(codPos, out outcome);
                        outcome = new int?();
                        if (!string.IsNullOrEmpty(anagraficaLavorativa.DataNas))
                        {
                            string dataNas = anagraficaLavorativa.DataNas;
                            dateTime1 = Convert.ToDateTime(dataNas);
                            dateTime1 = dateTime1.AddYears(65);
                            string dataNasPiu65 = dateTime1.ToString("d");
                            if (!string.IsNullOrEmpty(anagraficaLavorativa.DataDec))
                            {
                                string dataDec = anagraficaLavorativa.DataDec;
                                if (!string.IsNullOrEmpty(anagraficaLavorativa.DataCes))
                                {
                                    dateTime1 = Convert.ToDateTime(anagraficaLavorativa.DataCes);
                                    empty4 = dateTime1.ToString("d");
                                    dateTime1 = Convert.ToDateTime(empty4);
                                    int year = dateTime1.Year;
                                    dateTime1 = Convert.ToDateTime(data_dal);
                                    int num7 = dateTime1.Year - 1;
                                    if (year >= num7)
                                        str1 = "S";
                                }
                                else
                                {
                                    empty4 = string.Empty;
                                }

                                string str2;
                                if (DateTime.Compare(Convert.ToDateTime(data_dal),
                                        Convert.ToDateTime(anagraficaLavorativa.DataInizio)) >= 0)
                                {
                                    dateTime1 = Convert.ToDateTime(data_dal);
                                    str2 = dateTime1.ToString("d");
                                }
                                else
                                {
                                    dateTime1 = Convert.ToDateTime(anagraficaLavorativa.DataInizio);
                                    str2 = dateTime1.ToString("d");
                                }

                                string s2;
                                if (DateTime.Compare(DateTime.Parse(data_al),
                                        DateTime.Parse(anagraficaLavorativa.DataFine)) <= 0)
                                {
                                    dateTime1 = Convert.ToDateTime(data_al);
                                    s2 = dateTime1.ToString("d");
                                    if (!string.IsNullOrEmpty(empty4) &&
                                        DateTime.Compare(DateTime.Parse(empty4), DateTime.Parse(data_al)) <= 0)
                                        s2 = empty4;
                                }
                                else
                                {
                                    dateTime1 = Convert.ToDateTime(anagraficaLavorativa.DataFine);
                                    s2 = dateTime1.ToString("d");
                                }

                                int tipRap = Convert.ToInt32(anagraficaLavorativa.TipRap);
                                int codCon = Convert.ToInt32(anagraficaLavorativa.CodCon);
                                int codLoc = Convert.ToInt32(anagraficaLavorativa.CodLoc);
                                int codLiv = Convert.ToInt32(anagraficaLavorativa.CodLiv);
                                int codGruAss = Convert.ToInt32(anagraficaLavorativa.CodGruAss);
                                Decimal perPar = (Decimal)Convert.ToInt32(anagraficaLavorativa.PerPar);
                                Decimal perapp = (Decimal)Convert.ToInt32(anagraficaLavorativa.PerApp);
                                string fap = anagraficaLavorativa.Fap;
                                string abbPre = anagraficaLavorativa.AbbPre;
                                string assCon = anagraficaLavorativa.AssCon;
                                Decimal d = 0M;
                                List<ContrattoDiRiferimento> referralContracts =
                                    DenunciaMensileDAL.GetReferralContracts(codCon, codLiv, str2, out outcome);
                                int? nullable3 = outcome;
                                int num8 = 2;
                                Decimal num9;
                                if (!(nullable3.GetValueOrDefault() == num8 & nullable3.HasValue))
                                {
                                    outcome = new int?();
                                    qualifica = referralContracts[0].DenQua;
                                    num4 = referralContracts[0].CodQuaCon;
                                    proCon = referralContracts[0].Progressivo;
                                    livello = referralContracts[0].DenLiv;
                                    tipSpe = referralContracts[0].TipSpe;
                                    if (referralContracts[0].TipSpe == "S")
                                    {
                                        List<MinimiContrattuali> referenceContractualMin =
                                            DenunciaMensileDAL.GetReferenceContractualMin(codCon,
                                                referralContracts[0].Progressivo, codLiv, str2, out outcome);
                                        nullable3 = outcome;
                                        num8 = 2;
                                        if (!(nullable3.GetValueOrDefault() == num8 & nullable3.HasValue))
                                        {
                                            outcome = new int?();
                                            foreach (MinimiContrattuali minimiContrattuali in referenceContractualMin)
                                                d += minimiContrattuali.ImpVocRet;
                                            impSca = Convert.ToDecimal("0" + anagraficaLavorativa.ImpScaMat.ToString());
                                        }
                                    }
                                    else
                                    {
                                        num9 = anagraficaLavorativa.TraEco;
                                        impTraEco = Convert.ToDecimal("0" + num9.ToString());
                                    }
                                }
                                else
                                {
                                    DenunciaMensileBLL.log.Info((object)string.Format(
                                        "[TFI.BLL] : Preparazione denuncia mensile - Il contratto di riferimento dell'utente: {0} risulta mancante sul DB in data: {1}",
                                        (object)anagraficaLavorativa.Nome, (object)DateTime.Now));
                                    outcome = new int?();
                                }

                                if (codLoc != 0)
                                {
                                    List<Contratto> localContracts =
                                        DenunciaMensileDAL.GetLocalContracts(codLoc, str2, out outcome);
                                    nullable3 = outcome;
                                    num8 = 2;
                                    if (!(nullable3.GetValueOrDefault() == num8 & nullable3.HasValue))
                                    {
                                        outcome = new int?();
                                        proLoc = localContracts[0].Progressivo;
                                        if (localContracts[0].TipSpe == "S")
                                        {
                                            List<MinimiContrattuali> localContractualMin =
                                                DenunciaMensileDAL.GetLocalContractualMin(codLoc, proLoc, codLiv, str2,
                                                    out outcome);
                                            nullable3 = outcome;
                                            num8 = 2;
                                            if (!(nullable3.GetValueOrDefault() == num8 & nullable3.HasValue))
                                            {
                                                outcome = new int?();
                                                foreach (MinimiContrattuali minimiContrattuali in localContractualMin)
                                                    d += minimiContrattuali.ImpVocRet;
                                            }
                                        }
                                    }
                                }
                                else
                                    proLoc = 0M;

                                num9 = anagraficaLavorativa.PerApp;
                                if (Convert.ToDecimal("0" + num9.ToString()) > 0M)
                                    d = d / 100M * anagraficaLavorativa.PerApp;
                                num9 = anagraficaLavorativa.PerPar;
                                if (Convert.ToDecimal("0" + num9.ToString()) > 0M)
                                    d = d / 100M * anagraficaLavorativa.PerPar;
                                Decimal impMin = Decimal.Round(d, 2);
                                RetribuzioneRDL report = new RetribuzioneRDL();
                                report.CodFis = codFiscale;

                                DenunciaMensileBLL.FillBasicFields(report, tipRap, codCon, proCon, codLoc, proLoc,
                                    codLiv, livello, qualifica, num4, perPar, perapp, impMin, impTraEco, impSca, fap,
                                    assCon, abbPre, tipSpe, codGruAss, mat1, proRap, nome, dataNas, dataDec, empty4);
                                Decimal importoParametro1 =
                                    DenunciaMensileBLL.GetImportoParametro(1, str2, report.AbbPre);
                                Decimal importoParametro2 =
                                    DenunciaMensileBLL.GetImportoParametro(4, str2, report.AssCon);
                                if (IsOver65InYearMonth(dataNasPiu65, str2, s2))
                                {
                                    RetribuzioneRDL retribuzioneRdl1 = report;
                                    retribuzioneRdl1.Dal = str2;
                                    RetribuzioneRDL retribuzioneRdl2 = retribuzioneRdl1;
                                    dateTime1 = Convert.ToDateTime(dataNasPiu65);
                                    dateTime1 = dateTime1.AddDays(-1.0);
                                    string str3 = dateTime1.ToString("d");
                                    retribuzioneRdl2.Al = str3;
                                    retribuzioneRdl1.Eta65 = 'N';
                                    retribuzioneRdl1.ImpAbb = importoParametro1;
                                    retribuzioneRdl1.ImpAssCon = importoParametro2;
                                    Decimal aliquota1 = DenunciaMensileDAL.GetAliquota(num4, codGruAss, str2,
                                        retribuzioneRdl1.Eta65, anagraficaLavorativa.Fap, ref perFap, out outcome);
                                    nullable3 = outcome;
                                    num8 = 0;
                                    if (nullable3.GetValueOrDefault() == num8 & nullable3.HasValue)
                                    {
                                        outcome = new int?();
                                        retribuzioneRdl1.Aliquota = aliquota1;
                                    }
                                    else
                                    {
                                        if (outcome.HasValue && outcome.GetValueOrDefault() == 1)
                                            return (List<RetribuzioneRDL>)null;
                                        outcome = new int?();
                                    }

                                    retribuzioneRdl1.PerFap = perFap;
                                    retribuzioneRdl1.Prev = str1;
                                    report.ProDenDet = listaReport.Count(rep => rep.Mat == report.Mat) + 1;
                                    listaReport.Add(retribuzioneRdl1);
                                    var reportOver65 = CloneReport(report);
                                    RetribuzioneRDL retribuzioneRdl7 = reportOver65;
                                    dateTime1 = Convert.ToDateTime(dataNasPiu65);
                                    string str7 = dateTime1.ToString("d");
                                    retribuzioneRdl7.Dal = str7;
                                    reportOver65.Al = s2;
                                    reportOver65.Eta65 = 'S';
                                    reportOver65.ImpAbb = importoParametro1;
                                    reportOver65.ImpAssCon = importoParametro2;
                                    Decimal aliquota2 = DenunciaMensileDAL.GetAliquota(num4, codGruAss, str2,
                                        reportOver65.Eta65, anagraficaLavorativa.Fap, ref perFap, out outcome);
                                    nullable3 = outcome;
                                    num8 = 0;
                                    if (nullable3.GetValueOrDefault() == num8 & nullable3.HasValue)
                                    {
                                        reportOver65.Aliquota = aliquota2;
                                        outcome = new int?();
                                    }
                                    else
                                    {
                                        if (outcome.HasValue && outcome.GetValueOrDefault() == 1)
                                            return (List<RetribuzioneRDL>)null;
                                        outcome = new int?();
                                    }

                                    reportOver65.PerFap = perFap;
                                    reportOver65.ImpSanit = "0.00";
                                    reportOver65.IdPolizza = 0;
                                    reportOver65.Sanitario = "N";
                                    reportOver65.IdIsc = num3;
                                    reportOver65.IdAde = num2;
                                    reportOver65.Prev = str1;
                                    reportOver65.ProDenDet++;
                                    listaReport.Add(reportOver65);
                                }
                                else if (DateTime.Compare(DateTime.Parse(dataNasPiu65), DateTime.Parse(str2)) == 0)
                                {
                                    RetribuzioneRDL retribuzioneRdl8 = report;
                                    dateTime1 = Convert.ToDateTime(dataNasPiu65);
                                    string str8 = dateTime1.ToString("d");
                                    retribuzioneRdl8.Dal = str8;
                                    report.Al = s2;
                                    report.Eta65 = 'S';
                                    report.ImpAbb = importoParametro1;
                                    report.ImpAssCon = importoParametro2;
                                    Decimal aliquota = DenunciaMensileDAL.GetAliquota(num4, codGruAss, str2,
                                        report.Eta65, anagraficaLavorativa.Fap, ref perFap, out outcome);
                                    nullable3 = outcome;
                                    num8 = 0;
                                    if (nullable3.GetValueOrDefault() == num8 & nullable3.HasValue)
                                    {
                                        outcome = new int?();
                                        report.Aliquota = aliquota;
                                    }
                                    else
                                    {
                                        if (outcome.HasValue && outcome.GetValueOrDefault() == 1)
                                            return (List<RetribuzioneRDL>)null;
                                        outcome = new int?();
                                    }

                                    report.PerFap = perFap;
                                    report.IdIsc = num3;
                                    report.IdAde = num2;
                                    report.Prev = str1;
                                    report.ProDenDet = listaReport.Count(rep => rep.Mat == report.Mat) + 1;
                                    listaReport.Add(report);
                                }
                                else if (DateTime.Compare(DateTime.Parse(dataNasPiu65), DateTime.Parse(str2)) < 0)
                                {
                                    report.Dal = str2;
                                    report.Al = s2;
                                    report.Eta65 = 'S';
                                    report.ImpAbb = importoParametro1;
                                    report.ImpAssCon = importoParametro2;
                                    Decimal aliquota = DenunciaMensileDAL.GetAliquota(num4, codGruAss, str2,
                                        report.Eta65, anagraficaLavorativa.Fap, ref perFap, out outcome);
                                    nullable3 = outcome;
                                    num8 = 0;
                                    if (nullable3.GetValueOrDefault() == num8 & nullable3.HasValue)
                                    {
                                        outcome = new int?();
                                        report.Aliquota = aliquota;
                                    }
                                    else
                                    {
                                        if (outcome.HasValue && outcome.GetValueOrDefault() == 1)
                                            return (List<RetribuzioneRDL>)null;
                                        outcome = new int?();
                                    }

                                    report.PerFap = perFap;
                                    report.IdIsc = num3;
                                    report.IdAde = num2;
                                    report.Prev = str1;
                                    report.ProDenDet = listaReport.Count(rep => rep.Mat == report.Mat) + 1;
                                    listaReport.Add(report);
                                }
                                else if (DateTime.Compare(DateTime.Parse(dataNasPiu65), DateTime.Parse(s2)) == 0)
                                {
                                    RetribuzioneRDL retribuzioneRdl12 = report;
                                    retribuzioneRdl12.Dal = str2;
                                    RetribuzioneRDL retribuzioneRdl13 = retribuzioneRdl12;
                                    dateTime1 = Convert.ToDateTime(dataNasPiu65);
                                    dateTime1 = dateTime1.AddDays(-1.0);
                                    string str15 = dateTime1.ToString("d");
                                    retribuzioneRdl13.Al = str15;
                                    retribuzioneRdl12.Eta65 = 'N';
                                    retribuzioneRdl12.ImpAbb = importoParametro1;
                                    retribuzioneRdl12.ImpAssCon = importoParametro2;
                                    Decimal aliquota3 = DenunciaMensileDAL.GetAliquota(num4, codGruAss, str2,
                                        retribuzioneRdl12.Eta65, anagraficaLavorativa.Fap, ref perFap, out outcome);
                                    nullable3 = outcome;
                                    num8 = 0;
                                    if (nullable3.GetValueOrDefault() == num8 & nullable3.HasValue)
                                    {
                                        outcome = new int?();
                                        retribuzioneRdl12.Aliquota = aliquota3;
                                    }
                                    else
                                    {
                                        if (outcome.HasValue && outcome.GetValueOrDefault() == 1)
                                            return (List<RetribuzioneRDL>)null;
                                        outcome = new int?();
                                    }

                                    retribuzioneRdl12.PerFap = perFap;
                                    retribuzioneRdl12.ProDenDet = listaReport.Count(rep => rep.Mat == report.Mat) + 1;

                                    listaReport.Add(retribuzioneRdl12);
                                    RetribuzioneRDL retribuzioneRdl17 = CloneReport(report);
                                    RetribuzioneRDL retribuzioneRdl18 = retribuzioneRdl17;
                                    dateTime1 = Convert.ToDateTime(dataNasPiu65);
                                    string str19 = dateTime1.ToString("d");
                                    retribuzioneRdl18.Dal = str19;
                                    retribuzioneRdl17.Al = s2;
                                    retribuzioneRdl17.Eta65 = 'S';
                                    retribuzioneRdl17.ImpAbb = importoParametro1;
                                    retribuzioneRdl17.ImpAssCon = importoParametro2;
                                    Decimal aliquota4 = DenunciaMensileDAL.GetAliquota(num4, codGruAss, str2,
                                        retribuzioneRdl17.Eta65, anagraficaLavorativa.Fap, ref perFap, out outcome);
                                    nullable3 = outcome;
                                    num8 = 0;
                                    if (nullable3.GetValueOrDefault() == num8 & nullable3.HasValue)
                                    {
                                        outcome = new int?();
                                        retribuzioneRdl17.Aliquota = aliquota4;
                                    }
                                    else
                                    {
                                        if (outcome.HasValue && outcome.GetValueOrDefault() == 1)
                                            return (List<RetribuzioneRDL>)null;
                                        outcome = new int?();
                                    }

                                    retribuzioneRdl17.PerFap = perFap;
                                    retribuzioneRdl17.ImpSanit = "0.00";
                                    retribuzioneRdl17.IdPolizza = 0;
                                    retribuzioneRdl17.Sanitario = "N";
                                    retribuzioneRdl17.IdIsc = num3;
                                    retribuzioneRdl17.IdAde = num2;
                                    retribuzioneRdl17.Prev = str1;
                                    retribuzioneRdl17.ProDenDet++;

                                    listaReport.Add(retribuzioneRdl17);
                                }
                                else
                                {
                                    report.Dal = str2;
                                    report.Al = s2;
                                    report.Eta65 = 'N';
                                    report.ImpAbb = importoParametro1;
                                    report.ImpAssCon = importoParametro2;
                                    Decimal aliquota = DenunciaMensileDAL.GetAliquota(num4, codGruAss, str2,
                                        report.Eta65, anagraficaLavorativa.Fap, ref perFap, out outcome);
                                    nullable3 = outcome;
                                    num8 = 0;
                                    if (nullable3.GetValueOrDefault() == num8 & nullable3.HasValue)
                                    {
                                        outcome = new int?();
                                        report.Aliquota = aliquota;
                                    }
                                    else
                                    {
                                        if (outcome.HasValue && outcome.GetValueOrDefault() == 1)
                                            return (List<RetribuzioneRDL>)null;
                                        outcome = new int?();
                                    }

                                    report.PerFap = perFap;
                                    report.IdIsc = num3;
                                    report.IdAde = num2;
                                    report.Prev = str1;

                                    report.ProDenDet = listaReport.Count(rep => rep.Mat == report.Mat) + 1;
                                    listaReport.Add(report);
                                }
                            }
                            else
                            {
                                DenunciaMensileBLL.log.Info((object)string.Format(
                                    "[TFI.BLL] : Preparazione denuncia mensile - La data di iscrizione dell'utente: {0} risulta mancante sul DB in data: {1}",
                                    (object)anagraficaLavorativa.Nome, (object)DateTime.Now));
                                break;
                            }
                        }
                        else
                        {
                            DenunciaMensileBLL.log.Info((object)string.Format(
                                "[TFI.BLL] : Preparazione denuncia mensile - La data di nascita dell'utente: {0} risulta mancante sul DB in data: {1}",
                                (object)anagraficaLavorativa.Nome, (object)DateTime.Now));
                            break;
                        }
                    }

                    db.EndTransaction(true);



                    flag = false;
                    DenunciaMensileBLL.MergingDuplicateRows(listaReport);
                    DateTime dateTime2;
                    for (int index = listaReport.Count - 1; index >= 0; --index)
                    {
                        if (listaReport[index].Rimuovi)
                            listaReport.RemoveAt(index);
                        else if (listaReport[index].Prev == "S")
                        {
                            List<ImportiPrev> importiPrev1;
                            if ((importiPrev1 = DenunciaMensileDAL.GetImportiPrev(isArretrato, ref isImportoPrev,
                                    listaReport[index], codPos, out outcome, proDen)) != null)
                            {
                                outcome = new int?();
                                listaReport[index].ProMod = importiPrev1[0].ProMod;
                                if (importiPrev1[0].CodStaPre != 0)
                                {
                                    listaReport[index].Prev = "T";
                                    if (isArretrato)
                                    {
                                        RetribuzioneRDL record = listaReport[index];
                                        string strCodPos = codPos;
                                        dateTime2 = Convert.ToDateTime(data_dal);
                                        int year = dateTime2.Year;
                                        ref int? local = ref outcome;
                                        if (DenunciaMensileDAL.TheRowsMatches(record, strCodPos, year, out local) == 0)
                                        {
                                            int? nullable4 = outcome;
                                            int num10 = 0;
                                            if (nullable4.GetValueOrDefault() == num10 & nullable4.HasValue)
                                            {
                                                outcome = new int?();
                                                isImportoPrev = new bool?(false);
                                                //goto label_278;
                                            }
                                        }

                                        int? nullable5 = outcome;
                                        int num11 = 1;
                                        if (nullable5.GetValueOrDefault() == num11 & nullable5.HasValue)
                                        {
                                            DenunciaMensileBLL.log.Info((object)string.Format(
                                                "[TFI.BLL] : Preparazione denuncia mensile - La funzione TheRowsMatches() ha generato un'eccezione in data: {0}",
                                                (object)DateTime.Now));
                                            DenunciaMensileBLL.ErrorMessage =
                                                "La funzione TheRowsMatches() ha generato un'eccezione";
                                            return (List<RetribuzioneRDL>)null;
                                        }
                                    }
                                }

//label_278:
                                if (isImportoPrev.HasValue && isImportoPrev.Value)
                                {
                                    listaReport[index].ImpRet = importiPrev1[0].ImpRet;
                                    listaReport[index].ImpOcc = importiPrev1[0].ImpOcc;
                                    listaReport[index].ImpFig = importiPrev1[0].ImpFig;
                                    listaReport[index].ImpCon = importiPrev1[0].ImpCon;
                                }
                            }
                            else if (isArretrato)
                            {
                                outcome = new int?();
                                List<ImportiPrev> importiPrev2;
                                if ((importiPrev2 =
                                        DenunciaMensileDAL.GetImportiPrev(listaReport[index], codPos, out outcome)) !=
                                    null)
                                {
                                    listaReport[index].ProMod = importiPrev2[0].ProMod;
                                    if (importiPrev2[0].CodStaPre != 0)
                                        listaReport[index].Prev = "T";
                                }
                                else
                                {
                                    int? nullable6 = outcome;
                                    int num12 = 1;
                                    if (nullable6.GetValueOrDefault() == num12 & nullable6.HasValue)
                                    {
                                        DenunciaMensileBLL.log.Info((object)string.Format(
                                            "[TFI.BLL] : Preparazione denuncia mensile - La funzione GetImportiPrev() ha generato un'eccezione in data: {0}",
                                            (object)DateTime.Now));
                                        DenunciaMensileBLL.ErrorMessage =
                                            "La funzione GetImportiPrev() ha generato un'eccezione";
                                        return (List<RetribuzioneRDL>)null;
                                    }
                                }
                            }
                            else
                            {
                                int? nullable7 = outcome;
                                int num13 = 1;
                                if (nullable7.GetValueOrDefault() == num13 & nullable7.HasValue)
                                {
                                    DenunciaMensileBLL.log.Info((object)string.Format(
                                        "[TFI.BLL] : Preparazione denuncia mensile - La funzione GetImportiPrev() ha generato un'eccezione in data: {0}",
                                        (object)DateTime.Now));
                                    DenunciaMensileBLL.ErrorMessage =
                                        "La funzione GetImportiPrev() ha generato un'eccezione";
                                    return (List<RetribuzioneRDL>)null;
                                }
                            }
                        }
                    }

                    if (!isArretrato)
                    {
                        foreach (RetribuzioneRDL report in listaReport)
                        {
                            report.NumGGPer = DenunciaMensileBLL.GetNumGGPeriodo(report.Dal, report.Al);
                            report.NumGGDom = DenunciaMensileBLL.GetNumGGDomeniche(report.Dal, report.Al);
                            report.NumGGAzi = 0.0M;
                            report.NumGGSos = 0.0M;
                            report.NumGGFig = 0.0M;
                            outcome = new int?();
                            if (DenunciaMensileBLL.GetNumGGSospensioni(codPos, report, out outcome))
                            {
                                outcome = new int?();
                                if (num1 > 0)
                                    report.NumGGConAzi = !(report.NumGGSos > 0M)
                                        ? (!(Convert.ToDateTime(report.DatDec) <= Convert.ToDateTime(report.Dal))
                                            ? Decimal.Round((Decimal)num1 / Convert.ToDecimal(26), 0) *
                                              (Decimal)report.NumGGPer
                                            : (!(report.DatCes.Trim() == string.Empty)
                                                ? (!(Convert.ToDateTime(report.DatCes) > Convert.ToDateTime(report.Al))
                                                    ? Decimal.Round((Decimal)num1 / Convert.ToDecimal(26), 0) *
                                                      (Decimal)report.NumGGPer
                                                    : (Decimal)(report.NumGGPer - report.NumGGDom) - report.NumGGSos +
                                                      report.NumGGAzi)
                                                : Decimal.Round(
                                                    Convert.ToDecimal(report.NumGGPer) *
                                                    Convert.ToDecimal(26M / (Decimal)num1), 0)))
                                        : (Decimal)(report.NumGGPer - report.NumGGDom) - report.NumGGSos +
                                          report.NumGGAzi;
                                if (report.ImpFig == 0)
                                    report.ImpFig = !(report.TipSpe == "S")
                                        ? Convert.ToDecimal(report.ImpTraEco) * report.NumGGFig / 26M
                                        : (Convert.ToDecimal(report.ImpMin) + Convert.ToDecimal(report.ImpSca)) *
                                        report.NumGGFig / 26M;
                                report.ImpFig = Math.Round(report.ImpFig, 2);
                                report.ImpFap = 0.0M;
                                string strCodPos = codPos.ToString();
                                dateTime2 = Convert.ToDateTime(data_dal);
                                int year = dateTime2.Year;
                                dateTime2 = Convert.ToDateTime(data_dal);
                                int month4 = dateTime2.Month;
                                int mat2 = report.Mat;
                                int proRap = report.ProRap;
                                ref int? local = ref outcome;
                                List<FigurativaInfortuni> figurativaInfortuni;
                                if ((figurativaInfortuni =
                                        DenunciaMensileDAL.GetFigurativaInfortuni(strCodPos, year, month4, mat2, proRap,
                                            out local)) != null)
                                {
                                    outcome = new int?();
                                    report.ImpFig = figurativaInfortuni[0].ImpFig;
                                }
                                else
                                {
                                    int? nullable8 = outcome;
                                    int num14 = 1;
                                    if (nullable8.GetValueOrDefault() == num14 & nullable8.HasValue)
                                    {
                                        DenunciaMensileBLL.log.Info((object)string.Format(
                                            "[TFI.BLL] : Preparazione denuncia mensile - La funzione GetFigurativaInfortuni() ha generato un'eccezione in data: {0}",
                                            (object)DateTime.Now));
                                        DenunciaMensileBLL.ErrorMessage =
                                            "La funzione GetFigurativaInfortuni() ha generato un'eccezione";
                                        return (List<RetribuzioneRDL>)null;
                                    }
                                }
                            }
                            else
                            {
                                DenunciaMensileBLL.ErrorMessage =
                                    "La funzione GetSuspensions() ha generato un'eccezione";
                                return (List<RetribuzioneRDL>)null;
                            }
                        }
                    }
                    else
                    {
                        foreach (RetribuzioneRDL retribuzioneRdl in listaReport)
                        {
                            retribuzioneRdl.NumGGConAzi = 0M;
                            retribuzioneRdl.NumGGPer = 0;
                            retribuzioneRdl.NumGGDom = 0;
                            retribuzioneRdl.NumGGAzi = 0M;
                            retribuzioneRdl.NumGGSos = 0M;
                            retribuzioneRdl.NumGGFig = 0M;
                            retribuzioneRdl.ImpFig = 0.0M;
                            retribuzioneRdl.ImpFap = 0.0M;
                        }
                    }
                }

                return listaReport;
            }
            catch (Exception ex)
            {
                if (flag)
                    db.EndTransaction(false);
                DenunciaMensileBLL.log.Info((object)string.Format(
                    "[TFI.BLL] : Preparazione denuncia mensile - La funzione ha generato un'eccezione in data: {0}. Messaggio: {1}",
                    (object)DateTime.Now, (object)ex.Message));
                DenunciaMensileBLL.ErrorMessage =
                    "La funzione di preparazione della nuova denuncia ha generato un'eccezione: " + ex.Message;
                return (List<RetribuzioneRDL>)null;
            }
        }

        public static int GetNumeroSospesi(string codPos) => DenunciaMensileDAL.GetNumSospesi(codPos);

        public static List<DenunciaMensileSalvata> GetDenunceMensili(
            int anno,
            int codPos,
            string tipoUtente)
        {
            int numeroSospesi = DenunciaMensileBLL.GetNumeroSospesi(codPos.ToString());
            int? outcome = new int?();
            bool flag1 = false;
            DenunciaMensileSalvata denunciaMensileSalvata1 = (DenunciaMensileSalvata)null;
            List<DenunciaMensileSalvata> denunceMensili = new List<DenunciaMensileSalvata>();
            for (int index1 = 1; index1 <= 12; ++index1)
            {
                int index2 = 0;
                bool flag2 = true;
                string upper = Utils.GetMesi()[index1].ToUpper();
                List<string> parGen;
                string empty;
                int? nullable;
                int num1;
                if ((parGen = DenunciaMensileDAL.GetParGen(anno, index1, out outcome)) != null)
                {
                    outcome = new int?();
                    empty = parGen[0];
                }
                else
                {
                    nullable = outcome;
                    num1 = 2;
                    if (nullable.GetValueOrDefault() == num1 & nullable.HasValue)
                    {
                        empty = string.Empty;
                    }
                    else
                    {
                        DenunciaMensileBLL.log.Info((object)string.Format(
                            "[TFI.BLL] : Get denuncie mensili - La funzione GetParGen() ha generato un'eccezione in data: {0}",
                            (object)DateTime.Now));
                        DenunciaMensileBLL.ErrorMessage = "La funzione GetParGen() ha generato un'eccezione";
                        return (List<DenunciaMensileSalvata>)null;
                    }
                }

                List<DenunciaMensileSalvata> savedReport =
                    DenunciaMensileDAL.GetSavedReport(anno, codPos, index1, out outcome, flag2);
                nullable = outcome;
                num1 = 1;
                //Nullable = outcome // outcome è settato in getsavedreport ad 1 se va in eccezione il metodo, a 2 se non trova, a 0 se trova report.
                if (nullable.GetValueOrDefault() == num1 &
                    nullable.HasValue) //in sostanza vede se è andato in eccezione il metodo getsavedreport
                {
                    DenunciaMensileBLL.log.Info((object)string.Format(
                        "[TFI.BLL] : Get denuncie mensili - La funzione GetSavedReport() ha generato un'eccezione in data: {0}",
                        (object)DateTime.Now));
                    DenunciaMensileBLL.ErrorMessage = "La funzione GetSavedReport() ha generato un'eccezione";
                    return (List<DenunciaMensileSalvata>)null;
                }

                outcome = new int?();
                bool flag3;
                for (; index2 < 1; ++index2)
                {
                    DateTime dateTime;
                    if (savedReport != null) //Se ci sono report nel mese index1
                    {
                        anno.ToString();
                        denunciaMensileSalvata1 = new DenunciaMensileSalvata();
                        denunciaMensileSalvata1.MesDen = upper;
                        if (savedReport[index2].TipMov.Trim() == "NU")
                            denunciaMensileSalvata1.TipMov = "Notifica di Ufficio";
                        else if (savedReport[index2].TipMov.Trim() == "DP")
                            denunciaMensileSalvata1.TipMov = "Denuncia";
                        if (string.IsNullOrEmpty(savedReport[index2].DatChi))
                        {
                            denunciaMensileSalvata1.DatApe = "-";
                        }
                        else
                        {
                            DenunciaMensileSalvata denunciaMensileSalvata2 = denunciaMensileSalvata1;
                            dateTime = Convert.ToDateTime(savedReport[index2].DatChi);
                            string str = dateTime.ToString("dd/MM/yyyy");
                            denunciaMensileSalvata2.DatApe = str;
                        }

                        if (string.IsNullOrEmpty(savedReport[index2].DatSca))
                        {
                            DenunciaMensileSalvata denunciaMensileSalvata3 = denunciaMensileSalvata1;
                            dateTime = Convert.ToDateTime(empty);
                            string str = dateTime.ToString("dd/MM/yyyy");
                            denunciaMensileSalvata3.DatSca = str;
                        }
                        else
                        {
                            DenunciaMensileSalvata denunciaMensileSalvata4 = denunciaMensileSalvata1;
                            dateTime = Convert.ToDateTime(savedReport[index2].DatSca);
                            string str = dateTime.ToString("dd/MM/yyyy");
                            denunciaMensileSalvata4.DatSca = str;
                        }

                        denunciaMensileSalvata1.DatScaPag = denunciaMensileSalvata1.DatSca;
                        Decimal num2;
                        if (savedReport[0].TipMov == "NU")
                            denunciaMensileSalvata1.StaDen = "Notifica di Ufficio";
                        else if (savedReport[index2].StaDen == "A")
                        {
                            num2 = savedReport[0].CodModPag;
                            string str = num2.ToString();
                            denunciaMensileSalvata1.StaDen = Convert.ToInt32("0" + str) <= 0
                                ? "Acquisita senza estremi di pagamento"
                                : "Acquisita con estremi di pagamento";
                        }
                        else
                        {
                            denunciaMensileSalvata1.StaDen = "Non Confermata";
                            flag1 = true;
                        }

                        denunciaMensileSalvata1.ImpCon = "-";
                        denunciaMensileSalvata1.ImpSan = "-";
                        denunciaMensileSalvata1.ImpDel = "-";
                        denunciaMensileSalvata1.ImpSanRet = "-";
                        denunciaMensileSalvata1.ImpTot = "-";
                        if (savedReport[index2].StaDen == "A")
                        {
                            denunciaMensileSalvata1.ImpTot = "0";
                            if (savedReport[index2].ImpCon != null)
                            {
                                Decimal d = Convert.ToDecimal(savedReport[index2].ImpCon) +
                                            Convert.ToDecimal(savedReport[0].ImpAddRec) +
                                            Convert.ToDecimal(savedReport[0].ImpAbb) +
                                            Convert.ToDecimal(savedReport[0].ImpAssCon);
                                DenunciaMensileSalvata denunciaMensileSalvata5 = denunciaMensileSalvata1;
                                num2 = Decimal.Round(d, 2);
                                string str1 = num2.ToString();
                                denunciaMensileSalvata5.ImpCon = str1;
                                if (savedReport[index2].EsiRet == "S")
                                {
                                    Decimal num3 = Decimal.Round(savedReport[index2].Rettif, 2);
                                    denunciaMensileSalvata1.ImpDel = num3.ToString();
                                    DenunciaMensileSalvata denunciaMensileSalvata6 = denunciaMensileSalvata1;
                                    num2 = Decimal.Round(Convert.ToDecimal(denunciaMensileSalvata1.ImpCon) + num3, 2);
                                    string str2 = num2.ToString();
                                    denunciaMensileSalvata6.ImpTot = str2;
                                }
                                else
                                    denunciaMensileSalvata1.ImpTot = denunciaMensileSalvata1.ImpCon;

                                num2 = savedReport[index2].ImpSanDet;
                                if (Convert.ToDecimal("0" + num2.ToString()) > 0M &&
                                    savedReport[index2].DatSanAnn == null && savedReport[index2].SanSotSog == "N")
                                {
                                    DenunciaMensileSalvata denunciaMensileSalvata7 = denunciaMensileSalvata1;
                                    num2 = Decimal.Round(Convert.ToDecimal(savedReport[index2].ImpSanDet), 2);
                                    string str3 = num2.ToString();
                                    denunciaMensileSalvata7.ImpSan = str3;
                                    DenunciaMensileSalvata denunciaMensileSalvata8 = denunciaMensileSalvata1;
                                    num2 = Decimal.Round(
                                        Convert.ToDecimal(denunciaMensileSalvata1.ImpTot) +
                                        Convert.ToDecimal(denunciaMensileSalvata1.ImpSan), 2);
                                    string str4 = num2.ToString();
                                    denunciaMensileSalvata8.ImpTot = str4;
                                }

                                if (Convert.ToDecimal(savedReport[index2].ImpSanRet) > 0M)
                                {
                                    DenunciaMensileSalvata denunciaMensileSalvata9 = denunciaMensileSalvata1;
                                    num2 = Decimal.Round(Convert.ToDecimal(savedReport[index2].ImpSanRet), 2);
                                    string str5 = num2.ToString();
                                    denunciaMensileSalvata9.ImpSanRet = str5;
                                }
                            }
                        }

                        denunciaMensileSalvata1.UteChi = !(tipoUtente.Trim() == "E") ? "Telematica" : "Cartacea";
                        denunciaMensileSalvata1.CodModPag = savedReport[index2].CodModPag;
                        denunciaMensileSalvata1.ProDen = savedReport[index2].ProDen;
                        denunciaMensileSalvata1.IdDipa = savedReport[index2].IdDipa;
                        if (index2 == savedReport.Count - 1)
                            flag3 = false;
                    }
                    else
                    {
                        outcome = new int?();
                        denunciaMensileSalvata1 = new DenunciaMensileSalvata();
                        denunciaMensileSalvata1.MesDen = upper;
                        denunciaMensileSalvata1.TipMov = "-";
                        denunciaMensileSalvata1.DatApe = "-";
                        if (empty != string.Empty)
                        {
                            DenunciaMensileSalvata denunciaMensileSalvata10 = denunciaMensileSalvata1;
                            dateTime = Convert.ToDateTime(empty);
                            string str = dateTime.ToString("dd/MM/yyyy");
                            denunciaMensileSalvata10.DatSca = str;
                        }

                        denunciaMensileSalvata1.DatScaPag = denunciaMensileSalvata1.DatSca;
                        // ISSUE: variable of a boxed type
                        var local = anno;
                        string str6 = Utils.FixMese(index1);
                        num1 = DateTime.DaysInMonth(anno, index1);
                        string str7 = num1.ToString().PadLeft(2, '0');
                        string data = string.Format("{0}-{1}-{2}", (object)local, (object)str6, (object)str7);
                        string dataInizio = string.Format("{0}-{1}-01", (object)anno, (object)Utils.FixMese(index1));
                        if (DenunciaMensileDAL.GetTOTRapLav(codPos, dataInizio, data, out outcome) != 0)
                        {
                            outcome = new int?();
                            denunciaMensileSalvata1.StaDen = "Denuncia non Presentata";
                        }
                        else
                        {
                            nullable = outcome;
                            num1 = 1;
                            if (nullable.GetValueOrDefault() == num1 & nullable.HasValue)
                            {
                                DenunciaMensileBLL.log.Info((object)string.Format(
                                    "[TFI.BLL] : Get denuncie mensili - La funzione GetTOTRapLav() ha generato un'eccezione in data: {0}",
                                    (object)DateTime.Now));
                                DenunciaMensileBLL.ErrorMessage = "La funzione GetTOTRapLav() ha generato un'eccezione";
                                return (List<DenunciaMensileSalvata>)null;
                            }

                            denunciaMensileSalvata1.StaDen = "Rapporti di Lavoro Assenti";
                        }

                        denunciaMensileSalvata1.ImpCon = "-";
                        denunciaMensileSalvata1.ImpDel = "-";
                        denunciaMensileSalvata1.ImpTot = "-";
                        denunciaMensileSalvata1.ImpSan = "-";
                        denunciaMensileSalvata1.ImpSanRet = "-";
                        denunciaMensileSalvata1.UteChi = !(tipoUtente.Trim() == "E") ? "Telematica" : "Cartacea";
                        denunciaMensileSalvata1.CodModPag = 0M;
                        denunciaMensileSalvata1.ProDen = 0M;
                        denunciaMensileSalvata1.IdDipa = 0;
                    }

                    denunceMensili.Add(denunciaMensileSalvata1);
                }

                denunciaMensileSalvata1.NumSospesi = numeroSospesi;
                denunciaMensileSalvata1.NumMes = index1;
                flag3 = false;
            }

            return denunceMensili;
        }

        public static List<int> CaricaDDLAnni(
            int codPos,
            ref string hdnPrimoAnno,
            ref bool ciSonoNonConfermate)
        {
            int? outcome = new int?();
            List<int> intList = new List<int>();
            DateTime now = DateTime.Now;
            int year = DateTime.Now.Year;
            int denunceNonConfermate = DenunciaMensileDAL.GetDenunceNonConfermate(codPos, out outcome);
            int? nullable1 = outcome;
            int num1 = 0;
            if (nullable1.GetValueOrDefault() == num1 & nullable1.HasValue)
            {
                if (denunceNonConfermate > 0)
                {
                    outcome = new int?();
                    ciSonoNonConfermate = true;
                }
            }
            else
            {
                int? nullable2 = outcome;
                int num2 = 1;
                if (nullable2.GetValueOrDefault() == num2 & nullable2.HasValue)
                {
                    DenunciaMensileBLL.log.Info((object)string.Format(
                        "[TFI.BLL] : Carica DDL Anni - La funzione GetDenunceNonConfermate() ha generato un'eccezione in data: {0}",
                        (object)DateTime.Now));
                    DenunciaMensileBLL.ErrorMessage = "La funzione GetDenunceNonConfermate() ha generato un'eccezione";
                    return (List<int>)null;
                }
            }

            int annoRapportiAttivi;
            if ((annoRapportiAttivi = DenunciaMensileDAL.GetPrimoAnnoRapportiAttivi(codPos, out outcome)) != 0)
            {
                outcome = new int?();
                int num3;
                if ((num3 = DenunciaMensileDAL.GetPrimoAnnoDIPA(codPos, out outcome)) != 0)
                {
                    outcome = new int?();
                    if (num3 > annoRapportiAttivi)
                        num3 = annoRapportiAttivi;
                    Dentes_Data dentesData;
                    if ((dentesData = DenunciaMensileDAL.GetDentesData(codPos, out outcome)) != null)
                    {
                        outcome = new int?();
                        if (dentesData.StaDen == "N")
                            hdnPrimoAnno = dentesData.AnnDen.ToString();
                        else if (dentesData.MesDen == 12M)
                            hdnPrimoAnno = (Convert.ToInt32(dentesData.AnnDen) + 1).ToString();
                        else if (DenunciaMensileDAL.CountRecordFromRaplav(codPos, Convert.ToInt32(dentesData.AnnDen),
                                     Convert.ToInt32(dentesData.MesDen), out outcome) == 0)
                        {
                            int? nullable3 = outcome;
                            int num4 = 1;
                            if (nullable3.GetValueOrDefault() == num4 & nullable3.HasValue)
                            {
                                DenunciaMensileBLL.log.Info((object)string.Format(
                                    "[TFI.BLL] : Carica DDL Anni - La funzione CountRecordFromRaplav() ha generato un'eccezione in data: {0}",
                                    (object)DateTime.Now));
                                DenunciaMensileBLL.ErrorMessage =
                                    "La funzione CountRecordFromRaplav() ha generato un'eccezione";
                                return (List<int>)null;
                            }

                            outcome = new int?();
                            string firstYear;
                            if ((firstYear = DenunciaMensileDAL.GetFirstYear(codPos, out outcome)) != null)
                            {
                                hdnPrimoAnno = firstYear;
                            }
                            else
                            {
                                DenunciaMensileBLL.log.Info((object)string.Format(
                                    "[TFI.BLL] : Carica DDL Anni - La funzione GetFirstYear() ha generato un'eccezione in data: {0}",
                                    (object)DateTime.Now));
                                DenunciaMensileBLL.ErrorMessage = "La funzione GetFirstYear() ha generato un'eccezione";
                                return (List<int>)null;
                            }
                        }

                        if (hdnPrimoAnno != string.Empty && annoRapportiAttivi > Convert.ToInt32(hdnPrimoAnno))
                            hdnPrimoAnno = annoRapportiAttivi.ToString();
                    }
                    else
                    {
                        int? nullable4 = outcome;
                        int num5 = 1;
                        if (nullable4.GetValueOrDefault() == num5 & nullable4.HasValue)
                        {
                            DenunciaMensileBLL.log.Info((object)string.Format(
                                "[TFI.BLL] : Carica DDL Anni - La funzione GetDentesData() ha generato un'eccezione in data: {0}",
                                (object)DateTime.Now));
                            DenunciaMensileBLL.ErrorMessage = "La funzione GetDentesData() ha generato un'eccezione";
                            return (List<int>)null;
                        }

                        outcome = new int?();
                        hdnPrimoAnno = annoRapportiAttivi.ToString();
                    }

                    if (hdnPrimoAnno == string.Empty)
                        hdnPrimoAnno = "2003";
                    if (year != 0)
                    {
                        for (int index = year; index >= num3; --index)
                        {
                            if(DenunciaMensileDAL.GetTOTRapLav(codPos, $"{index}-01-01", $"{index}-12-31", out outcome) != 0)
                                intList.Add(index);
                        }
                    }

                    return intList;
                }

                int? nullable5 = outcome;
                int num6 = 1;
                if (nullable5.GetValueOrDefault() == num6 & nullable5.HasValue)
                {
                    DenunciaMensileBLL.log.Info((object)string.Format(
                        "[TFI.BLL] : Carica DDL Anni - La funzione GetPrimoAnnoDIPA() ha generato un'eccezione in data: {0}",
                        (object)DateTime.Now));
                    DenunciaMensileBLL.ErrorMessage = "La funzione GetPrimoAnnoDIPA() ha generato un'eccezione";
                    return (List<int>)null;
                }

                DenunciaMensileBLL.ErrorMessage = "GetPrimoAnnoDIPA() non ha trovato risultati";
                return (List<int>)null;
            }

            int? nullable6 = outcome;
            int num7 = 1;
            if (nullable6.GetValueOrDefault() == num7 & nullable6.HasValue)
            {
                DenunciaMensileBLL.log.Info((object)string.Format(
                    "[TFI.BLL] : Carica DDL Anni - La funzione GetPrimoAnnoRapportiAttivi() ha generato un'eccezione in data: {0}",
                    (object)DateTime.Now));
                DenunciaMensileBLL.ErrorMessage = "La funzione GetPrimoAnnoRapportiAttivi() ha generato un'eccezione";
                return (List<int>)null;
            }

            DenunciaMensileBLL.ErrorMessage = "GetPrimoAnnoRapportiAttivi() non ha trovato risultati";
            return (List<int>)null;
        }

        public static DatiNuovaDenuncia CaricaNuovaDenunciaMensile(
            TFI.OCM.Utente.Utente utente,
            string anno,
            string mese,
            int proDen,
            int idDIPA,
            string sanit,
            out string currentTimeStamp_session)
        {
            string codPosizione = utente.CodPosizione;
            int int32_1 = Convert.ToInt32(mese);
            int int32_2 = Convert.ToInt32(anno);
            int mesFia = int32_1;
            int num1 = 0;
            int num2 = 0;
            Decimal num3 = 0M;
            Decimal num4 = 0M;
            Decimal num5 = 0M;
            Decimal num6 = 0M;
            int? outcome1 = new int?();
            string upper = Utils.GetMesi()[Convert.ToInt32(mese)].ToUpper();
            string str1 = int32_1.ToString().PadLeft(2, '0');
            string str2 = string.Empty;
            if (sanit == "S")
                str2 =
                    "Si informa che, in ottemperanza di quanto disposto dall’art. 39 del C.C.N.L. per i quadri e gli impiegati agricoli, " +
                    "rinnovato in data 23/02/2017, a decorrere dal 2017, il contributo del datore di lavoro è pari a € 470,00 complessivi annui.";
            currentTimeStamp_session = DenunciaMensileDAL.GetCurrentTimeStampDIPA(codPosizione, Convert.ToInt32(anno),
                Convert.ToInt32(mese), proDen, out outcome1);
            int? nullable = outcome1;
            int num7 = 1;
            if (nullable.GetValueOrDefault() == num7 & nullable.HasValue)
            {
                DenunciaMensileBLL.log.Info((object)string.Format(
                    "[TFI.BLL] : DenunciaMensile_Nuova - La funzione GetCurrentTimeStampDIPA() ha generato un'eccezione in data: {0}",
                    (object)DateTime.Now));
                DenunciaMensileBLL.ErrorMessage = "La funzione GetCurrentTimeStampDIPA() ha generato un'eccezione";
                return (DatiNuovaDenuncia)null;
            }

            int? outcome2 = new int?();
            string data_dal = "01/" + str1 + "/" + anno;
            string data_al = DateTime.DaysInMonth(Convert.ToInt32(anno), int32_1).ToString().PadLeft(2, '0') + "/" +
                             str1 + "/" + anno;
            List<RetribuzioneRDL> listaReport = DenunciaMensileBLL.PreparaDenunciaMensile(utente, proDen, data_dal,
                data_al, codPosizione, string.Empty, annFia: int32_2, mesFia: mesFia, idDipa: idDIPA);
            if (listaReport == null)
                return (DatiNuovaDenuncia)null;
            DatiNuovaDenuncia datiNuovaDenuncia = new DatiNuovaDenuncia()
            {
                Anno = anno,
                IntMese = int32_1,
                StrMese = upper,
                AnnFia = int32_2,
                MesFia = mesFia,
                ProDen = proDen,
                IdDIPA = idDIPA,
                Fia = str2,
                IsNotEnpaiaUser = utente.Tipo != "E",
                DataDenuncia = DateTime.Now.ToString("d"),
                DatiAnagrafici = DenunciaMensileBLL.GetDatiAnagraficiAzienda(codPosizione),
                DatiSedeLegale = DenunciaMensileBLL.GetDatiSedeLegale(codPosizione),
                IndirizzoSedeLegale = DenunciaMensileBLL.GetIndirizzoSedeLegale(codPosizione),
                BtnTotali_Enabled = true
            };
            datiNuovaDenuncia.ABBPREColumnIsHidden = datiNuovaDenuncia.DatiAnagrafici.NatGiu == 10M;
            int idAzi = DenunciaMensileDAL.GetIDAzi(codPosizione, out outcome2);
            if (idAzi != 0)
            {
                datiNuovaDenuncia.IdAzi = idAzi;
            }
            else
            {
                nullable = outcome2;
                int num8 = 1;
                if (nullable.GetValueOrDefault() == num8 & nullable.HasValue)
                {
                    DenunciaMensileBLL.log.Info((object)string.Format(
                        "[TFI.BLL] : DenunciaMensile_Nuova - La funzione GetIDAzi() ha generato un'eccezione in data: {0}",
                        (object)DateTime.Now));
                    DenunciaMensileBLL.ErrorMessage = "La funzione GetIDAzi() ha generato un'eccezione";
                    return (DatiNuovaDenuncia)null;
                }
            }

            if (proDen != 0)
            {
                DenunciaMensileBLL.VerificaDipa(ref listaReport, codPosizione, anno, int32_1, proDen);
            }
            else
            {
                bool flag = DenunciaMensileBLL.SospensioniImportoZero(listaReport);
                if (flag)
                    datiNuovaDenuncia.HdnImportoZero = flag;
                datiNuovaDenuncia.BtnTotali_Enabled = false;
            }

            datiNuovaDenuncia.Figurative = "0,00";
            datiNuovaDenuncia.Occasionali = "0,00";
            datiNuovaDenuncia.Imponibile = "0,00";
            if (listaReport != null)
            {
                foreach (RetribuzioneRDL report in listaReport)
                {
                    num3 += Convert.ToDecimal(report.ImpRet);
                    num4 += Convert.ToDecimal(report.ImpOcc);
                    num5 += Convert.ToDecimal(report.ImpFig);
                    if (num1 != report.Mat)
                    {
                        ++num2;
                        num1 = report.Mat;
                    }

                    report.AreSospesioniInMonth = report.NumGGSos > 0;
                    report.ImpCon = Math.Round(report.ImpRet * report.Aliquota / 100, 2);
                }

                datiNuovaDenuncia.Contributi = listaReport.Sum(report => report.ImpCon).ToString();
            }

            datiNuovaDenuncia.ListaReport = listaReport;
            datiNuovaDenuncia.Imponibile = num3.ToString();
            datiNuovaDenuncia.Occasionali = num4.ToString();
            datiNuovaDenuncia.Figurative = num5.ToString();
            datiNuovaDenuncia.Sanitario = num6.ToString();
            datiNuovaDenuncia.TotDip = num2.ToString();
            datiNuovaDenuncia.ListaParametriGenerali = DenunciaMensileBLL.listaParametriGenerali;
            return datiNuovaDenuncia;
        }

        public static bool SalvaParzialeDIPA(
            TFI.OCM.Utente.Utente utente,
            List<RetribuzioneRDL> listaReport,
            DatiNuovaDenuncia datiDenuncia,
            ref string currentTimeStamp_session,
            out int proDen,
            out int idDipa)
        {
            int? outcome1 = new int?();
            int int32 = Convert.ToInt32(datiDenuncia.Anno);
            int intMese = datiDenuncia.IntMese;
            string codPosizione = utente.CodPosizione;
            string tipo = utente.Tipo;
            string dataDenuncia = datiDenuncia.DataDenuncia;
            string str = datiDenuncia.Sanitario.Replace(',', '.');
            int idAzi = datiDenuncia.IdAzi;
            idDipa = datiDenuncia.IdDIPA;
            proDen = datiDenuncia.ProDen;
            int mesFia = datiDenuncia.MesFia;
            int annFia = datiDenuncia.AnnFia;
            int num1 = 0;
            int intProDenDet = 0;
            int count = 0;
            string ANNBILMOV = (string)null;
            string DATAPE = string.Empty;
            string meseFixato = Utils.FixMese(intMese);
            Decimal impAbb = 0M;
            Decimal impAssCon = 0M;
            bool blnCommit = false;
            DataLayer db = new DataLayer();
            List<ParametriGenerali> parametriGenerali = datiDenuncia.ListaParametriGenerali;
            string timeStampDentes1 = DenunciaMensileDAL.GetTimeStampDENTES(codPosizione, int32, intMese, out outcome1);
            int? nullable1 = outcome1;
            int num2 = 0;
            if (nullable1.GetValueOrDefault() == num2 & nullable1.HasValue)
            {
                int? outcome2 = new int?();
                //if (timeStampDentes1 != string.Empty && timeStampDentes1 != currentTimeStamp_session)
                //{
                //  DenunciaMensileBLL.ErrorMessage = "I dati del DIPA sono stati modificati da un altro utente. Impossibile continuare.";
                //  return false;
                //}
                Dictionary<string, string> tipoDenuncia =
                    DenunciaMensileDAL.GetTipoDenuncia(codPosizione, int32, meseFixato, out outcome2);
                int? nullable2 = outcome2;
                int num3 = 0;
                if (nullable2.GetValueOrDefault() == num3 & nullable2.HasValue)
                {
                    int? outcome3 = new int?();
                    string TIPDEN = tipoDenuncia["TIPISC"];
                    if (tipoDenuncia["ABB"] == "S")
                    {
                        impAbb = DenunciaMensileDAL.GetImportoAbbonamento(int32, meseFixato, out outcome3);
                        nullable2 = outcome3;
                        int num4 = 1;
                        if (nullable2.GetValueOrDefault() == num4 & nullable2.HasValue)
                        {
                            DenunciaMensileBLL.log.Info((object)string.Format(
                                "[TFI.BLL] : DenunciaMensile_Nuova - La funzione GetImportoAbbonamento() ha generato un'eccezione in data: {0}",
                                (object)DateTime.Now));
                            DenunciaMensileBLL.ErrorMessage =
                                "La funzione GetImportoAbbonamento() ha generato un'eccezione";
                            return false;
                        }

                        outcome3 = new int?();
                    }

                    try
                    {
                        db.StartTransaction();
                        if (proDen == 0)
                        {
                            if (timeStampDentes1 == string.Empty)
                                DATAPE = DBMethods.Db2Date(dataDenuncia);
                            string dataScadenza = DenunciaMensileDAL.GetDataScadenza(int32, meseFixato, out outcome3);
                            nullable2 = outcome3;
                            int num5 = 1;
                            if (nullable2.GetValueOrDefault() == num5 & nullable2.HasValue)
                            {
                                DenunciaMensileBLL.log.Info((object)string.Format(
                                    "[TFI.BLL] : DenunciaMensile_Nuova - La funzione GetDataScadenza() ha generato un'eccezione in data: {0}",
                                    (object)DateTime.Now));
                                DenunciaMensileBLL.ErrorMessage =
                                    "La funzione GetDataScadenza() ha generato un'eccezione";
                                throw new Exception();
                            }

                            proDen = WriteDIPA.WRITE_INSERT_DENTES(db, utente.Username, codPosizione, int32, intMese,
                                DATAPE, tipo, "", "", "DP", TIPDEN, "N", 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M,
                                0.0M, "", "", "", 0, dataScadenza, ANNBILMOV, 0, 0.0M, "", "", "", "", 0M, 0M, "", 0, 0,
                                "N", "", "");
                            //DateTime dateTime = Convert.ToDateTime(string.Format("{2}-{1}-{0}", (object) DateTime.DaysInMonth(int32, mesFia), (object) mesFia, (object) annFia));
                            //dateTime = dateTime.AddDays(30.0);
                            //string DATSCA = dateTime.ToString("d");
                            //if (str != "0.00")
                            //{
                            //  idDipa = WriteDIPA.WRITE_INSERT_DIPATESTMP(db, utente.Username, codPosizione, int32, intMese, proDen, annFia, mesFia, "A", idAzi, 0M, DATSCA);
                            //  if (idDipa == 0)
                            //    proDen = 0;
                            //}
                        }

                        //else if (idDipa == 0 && str != "0.00")
                        //{
                        //  string DATSCA = Convert.ToDateTime(string.Format("{0}/{1}/{2}", (object) DateTime.DaysInMonth(int32, mesFia), (object) mesFia, (object) annFia)).AddDays(30.0).ToString("d");
                        //  idDipa = WriteDIPA.WRITE_INSERT_DIPATESTMP(db, utente.Username, codPosizione, int32, intMese, proDen, annFia, mesFia, "A", idAzi, 0M, DATSCA);
                        //}
                        if (proDen > 0)
                        {
                            blnCommit = WriteDIPA.WRITE_DELETE_DENDET(db, codPosizione, int32, intMese, proDen, 0,
                                "DP");
                            //if (str != "0.00")
                            //  blnCommit = WriteDIPA.WRITE_DELETE_DIPADETTMP(db, idDipa, codPosizione, int32, intMese, proDen);
                            if (blnCommit)
                            {
                                foreach (RetribuzioneRDL retribuzioneRdl in listaReport)
                                {
                                    if (retribuzioneRdl.Mat != num1)
                                    {
                                        impAbb += retribuzioneRdl.ImpAbb;
                                        impAssCon += retribuzioneRdl.ImpAssCon;
                                    }

                                    blnCommit = WriteDIPA.WRITE_INSERT_DENDET(db, listaReport, parametriGenerali,
                                        utente, codPosizione, int32, intMese, proDen, retribuzioneRdl.Mat, "DP",
                                        retribuzioneRdl.Dal, retribuzioneRdl.Al, "", retribuzioneRdl.ImpRet,
                                        retribuzioneRdl.ImpOcc, retribuzioneRdl.ImpFig, retribuzioneRdl.ImpAbb,
                                        retribuzioneRdl.ImpAssCon, retribuzioneRdl.ImpCon, retribuzioneRdl.ImpMin,
                                        retribuzioneRdl.Prev.ToString(), retribuzioneRdl.ProMod, retribuzioneRdl.DatDec,
                                        retribuzioneRdl.DatCes, retribuzioneRdl.NumGGAzi, retribuzioneRdl.NumGGFig,
                                        (Decimal)retribuzioneRdl.NumGGPer, (Decimal)retribuzioneRdl.NumGGDom,
                                        retribuzioneRdl.NumGGSos, retribuzioneRdl.NumGGConAzi, retribuzioneRdl.ImpSca,
                                        retribuzioneRdl.ImpTraEco, retribuzioneRdl.Eta65.ToString(),
                                        retribuzioneRdl.TipRap, retribuzioneRdl.Fap, retribuzioneRdl.PerFap,
                                        retribuzioneRdl.ImpFap, retribuzioneRdl.PerPar, retribuzioneRdl.PerApp,
                                        retribuzioneRdl.ProRap, retribuzioneRdl.CodCon, (int)retribuzioneRdl.ProCon,
                                        retribuzioneRdl.TipSpe, retribuzioneRdl.CodLoc, (int)retribuzioneRdl.ProLoc,
                                        retribuzioneRdl.CodLiv, retribuzioneRdl.CodGruAss, retribuzioneRdl.CodQuaCon,
                                        retribuzioneRdl.Aliquota, retribuzioneRdl.DatNas, 0, TIPDEN, ref intProDenDet);
                                    //if (str != "0.00" && retribuzioneRdl.Sanitario == "S" && WriteDIPA.WRITE_INSERT_DIPADETTMP(db, utente, codPosizione, int32, intMese, proDen, intProDenDet, retribuzioneRdl.Mat, idDipa, annFia, mesFia, idAzi, retribuzioneRdl.IdIsc, retribuzioneRdl.IdAde, retribuzioneRdl.IdPolizza, Convert.ToDecimal(retribuzioneRdl.ImpSanit)) == 0)
                                    //{
                                    //  blnCommit = false;
                                    //  break;
                                    //}
                                    ++count;
                                    num1 = retribuzioneRdl.Mat;
                                }

                                if (blnCommit)
                                {
                                    TotaliDettaglio totaliDettaglio =
                                        DenunciaMensileDAL.GetTotaliDettaglio(db, codPosizione, int32, intMese, proDen);
                                    blnCommit = DenunciaMensileDAL.AggiornaTestata(db, totaliDettaglio, impAbb,
                                        impAssCon, count, codPosizione, int32, intMese, proDen);
                                    if (blnCommit)
                                    {
                                        Decimal importoParametro = WriteDIPA.GetImportoParametro(parametriGenerali, 5,
                                            DateTime.Now.ToString("d"));
                                        blnCommit = DenunciaMensileDAL.AggiornaTestata(db, codPosizione, int32, intMese,
                                            proDen, importoParametro);
                                        if (blnCommit)
                                            blnCommit = DenunciaMensileDAL.AggiornaTestata(db, codPosizione, int32,
                                                intMese, proDen);
                                    }
                                    //if (blnCommit && str != "0.00")
                                    //{
                                    //  string totSanitario = DenunciaMensileDAL.GetTotSanitario(db, codPosizione, int32, intMese, proDen, idDipa);
                                    //  blnCommit = DenunciaMensileDAL.AggiornaTestata(db, totSanitario.Replace(',', '.'), codPosizione, int32, intMese, proDen, idDipa);
                                    //}
                                }
                            }
                        }

                        db.EndTransaction(blnCommit);
                        if (blnCommit)
                        {
                            int? outcome4 = new int?();
                            string timeStampDentes2 =
                                DenunciaMensileDAL.GetTimeStampDENTES(codPosizione, int32, intMese, out outcome4);
                            nullable2 = outcome4;
                            int num6 = 1;
                            if (nullable2.GetValueOrDefault() == num6 & nullable2.HasValue)
                                throw new Exception();
                            currentTimeStamp_session = timeStampDentes2;
                        }

                        return blnCommit;
                    }
                    catch (Exception ex)
                    {
                        db.EndTransaction(false);
                        DenunciaMensileBLL.log.Info((object)string.Format(
                            "[TFI.BLL] : DenunciaMensile_Nuova - In fase di salvataggio parziale è stata generata un'eccezione in data: {0}",
                            (object)DateTime.Now));
                        DenunciaMensileBLL.ErrorMessage = DenunciaMensileBLL.ErrorMessage +
                                                          " Si è generata un' eccezione in fase di salvataggio: \n " +
                                                          DenunciaMensileBLL.ErrorMessage + " " + ex.Message;
                        return false;
                    }
                }
                else
                {
                    nullable2 = outcome2;
                    int num7 = 1;
                    if (nullable2.GetValueOrDefault() == num7 & nullable2.HasValue)
                    {
                        DenunciaMensileBLL.log.Info((object)string.Format(
                            "[TFI.BLL] : DenunciaMensile_Nuova - La funzione GetTipoDenuncia() ha generato un'eccezione in data: {0}",
                            (object)DateTime.Now));
                        DenunciaMensileBLL.ErrorMessage = "La funzione GetTipoDenuncia() ha generato un'eccezione";
                        return false;
                    }

                    DenunciaMensileBLL.WarningMessage = "La funzione GetTipoDenuncia() non ha restituito risultati";
                    return false;
                }
            }
            else
            {
                DenunciaMensileBLL.log.Info((object)string.Format(
                    "[TFI.BLL] : DenunciaMensile_Nuova - La funzione GetTimeStampDENTES() ha generato un'eccezione in data: {0}",
                    (object)DateTime.Now));
                DenunciaMensileBLL.ErrorMessage = "La funzione GetTimeStampDENTES() ha generato un'eccezione";
                return false;
            }
        }

        public static List<SospensioneRapporto> GetSospensioni(
            string mat,
            string proRap,
            string codPos,
            string dataIni,
            string dataFin)
        {
            List<SospensioneRapporto> listaSospensioni;
            if ((listaSospensioni = DenunciaMensileDAL.GetListaSospensioni(mat, proRap, codPos, dataIni, dataFin, out var outcome)) != null)
                return listaSospensioni;
            
            int? nullable = outcome;
            int num = 1;
            if (!(nullable.GetValueOrDefault() == num & nullable.HasValue))
                return null;
            
            log.Info($"[TFI.BLL] : DenunciaMensile_Nuova - La funzione GetListaSospensioni() ha generato un'eccezione in data: {DateTime.Now}");
            ErrorMessage = "La funzione GetListaSospensioni() ha generato un'eccezione";
            return null;
        }

        public static DatiDenuncia CaricaDati(
            string codPos,
            int anno,
            int mese,
            int proDen,
            int idDipa,
            bool isFirstLoading)
        {
            int? outcome1 = new int?();
            string str1 = string.Empty;
            DatiDenuncia datiDenuncia = new DatiDenuncia();
            int? nullable1;
            int? nullable2;
            if (isFirstLoading)
            {
                TotaliTestata totaliTestata = DenunciaMensileDAL.GetTotaliTestata(codPos, anno, mese, proDen, out var outcome2);
                int? nullable4 = outcome2;
                int num2 = 1;
                if (!(nullable4.GetValueOrDefault() == num2 & nullable4.HasValue))
                {
                    int? outcome3 = new int?();
                    datiDenuncia.IdDipa = idDipa;
                    datiDenuncia.ProDen = proDen;
                    datiDenuncia.Anno = anno;
                    datiDenuncia.Mese = mese;
                    datiDenuncia.TotTestata.TotImpRet = totaliTestata.TotImpRet;
                    datiDenuncia.TotTestata.TotImpOcc = totaliTestata.TotImpOcc;
                    datiDenuncia.TotTestata.TotImpFig = totaliTestata.TotImpFig;
                    datiDenuncia.TotTestata.TotImpCon = datiDenuncia.Records.Sum(record => record.ImpCon);
                    string str2 = totaliTestata.TipMov.Trim();

                    if (str2 != "DP" && str2 == "NU") str1 = $"Notifica di Ufficio relativa al Periodo {Utils.GetMesi()[mese].ToUpper()} {anno}";
                    else str1 = $"Denuncia Mensile relativa al Periodo {Utils.GetMesi()[mese].ToUpper()} {anno}";
                    datiDenuncia.Periodo = str1;

                    datiDenuncia.TotTestata.DataDenuncia = Convert.ToDateTime(totaliTestata.DataDenuncia).ToString("d");
                    
                    int totDipendenti = DenunciaMensileDAL.GetTotDipendenti(codPos, anno, mese, proDen, out outcome3);
                    nullable1 = outcome3;
                    int num3 = 1;
                    if (!(nullable1.GetValueOrDefault() == num3 & nullable1.HasValue))
                    {
                        //int? outcome4 = new int?();
                        datiDenuncia.TotDipendenti = totDipendenti;

                        DatiAnagraficiAzienda anagraficiAzienda2 = DenunciaMensileDAL.GetDatiAnagraficiAzienda_2(codPos, out var outcome4);
                        nullable1 = outcome4;
                        int num4 = 1;
                        if (!(nullable1.GetValueOrDefault() == num4 & nullable1.HasValue))
                        {
                            //nullable2 = new int?();
                            datiDenuncia.DatiAnagrafici = anagraficiAzienda2;

                            string indirizzoSedeLegale = GetIndirizzoSedeLegale(codPos);
                            //outcome1 = new int?();
                            datiDenuncia.SedeLegale = indirizzoSedeLegale;
                        }
                        else
                        {
                            log.Info($"[TFI.BLL] : DenunciaMensile_Lettura - La funzione GetDatiAnagraficiAzienda_2() ha generato un'eccezione in data: {DateTime.Now}");
                            ErrorMessage = "La funzione GetDatiAnagraficiAzienda_2() ha generato un'eccezione";
                            return null;
                        }
                    }
                    else
                    {
                        log.Info($"[TFI.BLL] : DenunciaMensile_Lettura - La funzione GetTotDipendenti() ha generato un'eccezione in data: {DateTime.Now}");
                        ErrorMessage = "La funzione GetTotDipendenti() ha generato un'eccezione";
                        return null;
                    }
                }
                else
                {
                    log.Info($"[TFI.BLL] : DenunciaMensile_Lettura - La funzione GetTotaliTestata() ha generato un'eccezione in data: {DateTime.Now}");
                    ErrorMessage = "La funzione GetTotaliTestata() ha generato un'eccezione";
                    return null;
                }
            }

            DenunciaMensileDAL.CheckRettifiche(codPos, anno, mese, proDen, out outcome1);
            nullable1 = outcome1;
            int num5 = 0;
            if (!(nullable1.GetValueOrDefault() == num5 & nullable1.HasValue))
            {
                nullable1 = outcome1;
                int num6 = 2;
                if (!(nullable1.GetValueOrDefault() == num6 & nullable1.HasValue))
                {
                    log.Info($"[TFI.BLL] : DenunciaMensile_Lettura - La funzione CheckRettifiche() ha generato un'eccezione in data: {DateTime.Now}");
                    ErrorMessage = "La funzione CheckRettifiche() ha generato un'eccezione";
                    return null;
                }
            }

            nullable1 = outcome1;
            int num7 = 0;
            datiDenuncia.BtnRettificheIsVisible = nullable1.GetValueOrDefault() == num7 & nullable1.HasValue;

            int? outcome5 = new int?();
            List<RetribuzioneRDL> data = DenunciaMensileDAL.GetData(codPos, idDipa, anno, mese, proDen, out outcome5);
            if (data != null)
            {
                foreach (RetribuzioneRDL retribuzioneRdl in data)
                {
                    GetNumGGSospensioni(codPos, retribuzioneRdl, out var outcome);
                    retribuzioneRdl.AreSospesioniInMonth = retribuzioneRdl.NumGGSos > 0M;

                    if (retribuzioneRdl.Prev == "S")
                    {
                        int proMod = DenunciaMensileDAL.GetProMod(codPos, retribuzioneRdl.Mat, retribuzioneRdl.ProRap, out outcome5);
                        if (proMod != 0)
                        {
                            retribuzioneRdl.ProMod = proMod;
                        }
                        else
                        {
                            nullable1 = outcome5;
                            int num8 = 1;
                            if (nullable1.GetValueOrDefault() == num8 & nullable1.HasValue)
                            {
                                log.Info($"[TFI.BLL] : DenunciaMensile_Lettura - La funzione GetProMod() ha generato un'eccezione in data: {DateTime.Now}");
                                ErrorMessage = "La funzione GetProMod() ha generato un'eccezione";
                                return null;
                            }
                        }
                    }
                }

                datiDenuncia.Records = data;
                datiDenuncia.ImpFigColumnIsVisible = datiDenuncia.DatiAnagrafici.NatGiu != 10M;
                datiDenuncia.TotTestata.TotImpCon = datiDenuncia.Records.Sum(record => record.ImpCon);

                return datiDenuncia;
            }

            nullable1 = outcome5;
            int num11 = 1;
            if (nullable1.GetValueOrDefault() == num11 & nullable1.HasValue)
            {
                log.Info($"[TFI.BLL] : DenunciaMensile_Lettura - La funzione GetData() ha generato un'eccezione in data: {DateTime.Now}");
                ErrorMessage = "La funzione GetData() ha generato un'eccezione";
                return null;
            }

            ErrorMessage = "Errore in fase di caricamento: dati non trovati.";
            return null;
        }

        public static void Aggiornamento(
            ref DatiNuovaDenuncia datiDenuncia,
            List<DatiRicerca> datiModificati)
        {
            Decimal num = Convert.ToDecimal(datiDenuncia.Imponibile);
            if (datiDenuncia.ListaReport != null)
            {
                foreach (RetribuzioneRDL retribuzioneRdl in datiDenuncia.ListaReport)
                {
                    foreach (DatiRicerca datiRicerca in datiModificati)
                    {
                        Decimal mat = (Decimal)retribuzioneRdl.Mat;
                        Decimal? matricola = datiRicerca.Matricola;
                        Decimal valueOrDefault1 = matricola.GetValueOrDefault();
                        if (mat == valueOrDefault1 & matricola.HasValue)
                        {
                            Decimal proRap1 = (Decimal)retribuzioneRdl.ProRap;
                            Decimal? proRap2 = datiRicerca.ProRap;
                            Decimal valueOrDefault2 = proRap2.GetValueOrDefault();
                            if (proRap1 == valueOrDefault2 & proRap2.HasValue)
                            {
                                retribuzioneRdl.ImpRet = Convert.ToDecimal((object)datiRicerca.ImpRet);
                                retribuzioneRdl.ImpOcc = Convert.ToDecimal((object)datiRicerca.ImpOcc);
                                retribuzioneRdl.ImpCon = Convert.ToDecimal((object)datiRicerca.ImpCon);
                                num += retribuzioneRdl.ImpRet;
                            }
                        }
                    }
                }
            }

            datiDenuncia.Imponibile = num.ToString();
        }

        public static string GetStaDen(int codPos, int anno, int mese, string tipMov, int proDen = 0)
        {
            int? outcome = new int?();
            StatoAttualeDenuncia statoAttuale = (StatoAttualeDenuncia)null;
            if (proDen != 0)
                DenunciaMensileDAL.GetStatoAttualeDenuncia(codPos, anno, mese, tipMov, out outcome, ref statoAttuale,
                    proDen);
            else
                DenunciaMensileDAL.GetStatoAttualeDenuncia(codPos, anno, mese, tipMov, out outcome, ref statoAttuale);
            string staDen;
            if (statoAttuale == null)
            {
                int? nullable = outcome;
                int num = 1;
                if (nullable.GetValueOrDefault() == num & nullable.HasValue)
                {
                    DenunciaMensileBLL.log.Info((object)string.Format(
                        "[TFI.BLL] : GetStaDen() - La funzione GetStatoAttualeDenuncia() ha generato un'eccezione in data: {0}",
                        (object)DateTime.Now));
                    DenunciaMensileBLL.ErrorMessage = "La funzione GetStatoAttualeDenuncia() ha generato un'eccezione";
                    return (string)null;
                }

                staDen = "Denuncia non Presentata";
            }
            else
                staDen = !(statoAttuale.StaDen == "A")
                    ? "Non Confermata"
                    : (!(statoAttuale.CodModPag > 0M)
                        ? "Acquisita senza estremi di pagamento"
                        : "Acquisita con estremi di pagamento");

            return staDen;
        }

        public static bool EliminaDipa(Utente utente, int anno, int mese, int proDen, int idDipa)
        {
            DataLayer db = new DataLayer();
            int? nullable1 = DenunciaMensileDAL.ControllaPrevDegliIscritti(utente.CodPosizione, anno, mese, proDen, out var outcome);
            int num1 = -1;
            if (nullable1.GetValueOrDefault() != num1)
            {
                int num2 = 0;
                if (nullable1.GetValueOrDefault() == num2)
                {
                    try
                    {
                        db.StartTransaction();
                        if (DenunciaMensileDAL.EliminaDenuncia(db, utente.CodPosizione, anno, mese, proDen, idDipa))
                        {
                            db.EndTransaction(true);
                            return true;
                        }

                        db.EndTransaction(false);
                        DenunciaMensileBLL.ErrorMessage = "Si sono verificati dei problemi nella cancellazione della denuncia.";
                        return false;
                    }
                    catch (Exception ex)
                    {
                        db.EndTransaction(false);
                        DenunciaMensileBLL.log.Info($"[TFI.BLL] : EliminaDipa - La funzione EliminaDipa() ha generato un'eccezione in data: {DateTime.Now}");
                        DenunciaMensileBLL.ErrorMessage = "E' stata generata un'eccezione in fase di cancellazione della denuncia.";
                        return false;
                    }
                }

                DenunciaMensileBLL.ErrorMessage = "Impossibile eliminare la denuncia! Per uno o piu' iscritti sono presenti dei Prev gia' trasmessi.";
                return false;
            }

            int? nullable4 = outcome;
            int num3 = 1;
            if (nullable4.GetValueOrDefault() == num3 & nullable4.HasValue)
            {
                DenunciaMensileBLL.log.Info($"[TFI.BLL] : EliminaDipa - La funzione ControllaPrevDegliIscritti() ha generato un'eccezione in data: {DateTime.Now}");
                DenunciaMensileBLL.ErrorMessage = "La funzione ControllaPrevDegliIscritti() ha generato un'eccezione";
                return false;
            }

            DenunciaMensileBLL.ErrorMessage = "La funzione ControllaPrevDegliIscritti() non ha restituito risultati";
            return false;
        }

        public static DatiTotaliDenuncia CaricaTotaliDenuncia(
            DatiTotaliDenuncia datiTotDenuncia,
            string codPos,
            int anno,
            int mese,
            int proDen,
            int idDipa,
            string ret,
            ref string timeStampDipa)
        {
            int? outcome = new int?();
            int month = DateTime.Now.Month;
            string meseFixato = Utils.FixMese(mese);
            Decimal num1 = 0.00M;
            DataLayer db = new DataLayer();
            datiTotDenuncia.Anno = anno;
            datiTotDenuncia.Mese = mese;
            datiTotDenuncia.NomeMese = Utils.GetMesi()[mese].ToUpper();
            datiTotDenuncia.ProDen = proDen;
            datiTotDenuncia.IdDipa = idDipa;
            datiTotDenuncia.LblCreditiIsVisible = true;
            datiTotDenuncia.TxtCreditiIsVisible = true;
            datiTotDenuncia.BtnCreditiIsVisible = true;
            datiTotDenuncia.BtnRipristinaImportoIsVisible = true;
            datiTotDenuncia.RigaSanzioniIsVisible = true;
            datiTotDenuncia.LblTotaleIsVisible = true;
            datiTotDenuncia.LblTotPagareIsVisible = true;
            datiTotDenuncia.BtnConfermaTotaliIsVisible = true;
            datiTotDenuncia.BtnDettagliDipaIsVisible = true;
            datiTotDenuncia.TbIntestazionePagamentoIsVisible = false;
            datiTotDenuncia.TbPagamentoIsVisible = true;
            datiTotDenuncia.BtnMAVIsVisible = true;
            datiTotDenuncia.BtnStampaMAVIsVisible = true;
            datiTotDenuncia.BtnStampaRicevutaIsVisible = true;
            datiTotDenuncia.TbIntestazionePagamentoFondoSanitarioIsVisible = true;
            datiTotDenuncia.TbPagamentoSanitarioIsVisible = true;
            datiTotDenuncia.BtnMAVSanitIsVisible = true;
            datiTotDenuncia.BtnStampaMAVSanitIsVisible = true;
            TotaliDenuncia totaliDenuncia;
            if ((totaliDenuncia = DenunciaMensileDAL.CaricaTotaliDenuncia(codPos, anno, mese, proDen, out outcome)) !=
                null)
            {
                outcome = new int?();
                datiTotDenuncia.ImpDec = totaliDenuncia.ImpDec;
                datiTotDenuncia.BtnStampaMAVIsVisible = false;
                datiTotDenuncia.Prot =
                    !(totaliDenuncia.ProtRic == string.Empty) ? totaliDenuncia.ProtRic : string.Empty;
                datiTotDenuncia.Protocollo = totaliDenuncia.Protocollo;
                datiTotDenuncia.StaDen = totaliDenuncia.StaDen;
                datiTotDenuncia.TipMov = totaliDenuncia.TipMov;
                if (datiTotDenuncia.TipMov == "NU")
                {
                    datiTotDenuncia.BtnStampaRicevutaIsVisible = false;
                    datiTotDenuncia.IntestazioneTotali = "Totali Notifica d'Ufficio";
                }
                else
                    datiTotDenuncia.IntestazioneTotali = "Totali Denuncia Mensile";

                if (totaliDenuncia.NumSanAnn == string.Empty && totaliDenuncia.NumSan != string.Empty)
                    num1 = totaliDenuncia.ImpSanDet;
                if (ret == "S" && totaliDenuncia.Esiret == "S")
                {
                    datiTotDenuncia.TbIntestazionePagamentoIsVisible = false;
                    datiTotDenuncia.BtnCreditiIsVisible = false;
                    datiTotDenuncia.TbPagamentoIsVisible = false;
                    datiTotDenuncia.BtnDettagliDipaIsVisible = false;
                    datiTotDenuncia.LblCreditiIsVisible = false;
                    datiTotDenuncia.TxtCreditiIsVisible = false;
                    datiTotDenuncia.LblTotaleIsVisible = false;
                    datiTotDenuncia.LblTotPagareIsVisible = false;
                    datiTotDenuncia.LblContributi = totaliDenuncia.ImpCon + totaliDenuncia.ImpConDel;
                    datiTotDenuncia.LblAddizionale = totaliDenuncia.ImpAddRec + totaliDenuncia.ImpAddRecDel;
                    datiTotDenuncia.LblAbbonamento = totaliDenuncia.ImpAbb + totaliDenuncia.ImpAbbDel;
                    datiTotDenuncia.LblAssistenza = totaliDenuncia.ImpAssCon + totaliDenuncia.ImpAssConDel;
                    datiTotDenuncia.LblTotContributi = totaliDenuncia.ImpAddRec + totaliDenuncia.ImpAddRecDel;
                    Decimal num2 = num1 + totaliDenuncia.ImpSanRet;
                    datiTotDenuncia.LblSanzioni = num2;
                }
                else
                {
                    datiTotDenuncia.TxtCrediti = 0.00M;
                    datiTotDenuncia.LblDecurtato = totaliDenuncia.ImpDec;
                    datiTotDenuncia.LblContributi = totaliDenuncia.ImpCon;
                    datiTotDenuncia.LblAddizionale = totaliDenuncia.ImpAddRec;
                    datiTotDenuncia.LblAbbonamento = totaliDenuncia.ImpAbb;
                    datiTotDenuncia.LblAssistenza = totaliDenuncia.ImpAssCon;
                    datiTotDenuncia.LblTotContributi = totaliDenuncia.ImpCon + totaliDenuncia.ImpAddRec;
                    datiTotDenuncia.LblSanzioni = num1;
                }

                datiTotDenuncia.LblTotDovuto =
                    datiTotDenuncia.LblTotContributi + totaliDenuncia.ImpAssCon + totaliDenuncia.ImpAbb;
                datiTotDenuncia.LblTotPagare = datiTotDenuncia.LblTotDovuto + datiTotDenuncia.LblSanzioni;
                Decimal num3 = datiTotDenuncia.LblTotDovuto + datiTotDenuncia.LblSanzioni;
                if (totaliDenuncia.ImpVer != 0M)
                    datiTotDenuncia.TxtImportoVersato = totaliDenuncia.ImpVer;
                if (totaliDenuncia.DatVer != string.Empty)
                    datiTotDenuncia.TxtDataVersamento = totaliDenuncia.DatVer;
                datiTotDenuncia.LblTotPagare = !(datiTotDenuncia.LblDecurtato <= num3)
                    ? 0.00M
                    : num3 - datiTotDenuncia.LblDecurtato;
                if (totaliDenuncia.ImpSanDet == 0M && mese != month)
                {
                    string dataScadenza;
                    if ((dataScadenza = DenunciaMensileDAL.GetDataScadenza(anno, meseFixato, out outcome)) != null)
                    {
                        outcome = new int?();
                        if (DateTime.Compare(Convert.ToDateTime(dataScadenza), DateTime.Now) < 0)
                        {
                            int days = DateTime.Now.Subtract(Convert.ToDateTime(dataScadenza)).Days;
                            Decimal tasso = DenunciaMensileDAL.GetTasso(out outcome);
                            int? nullable = outcome;
                            int num4 = 1;
                            if (!(nullable.GetValueOrDefault() == num4 & nullable.HasValue))
                            {
                                Decimal num5 = Decimal.Round(totaliDenuncia.ImpCon * (Decimal)days * tasso / 36500M, 2);
                                datiTotDenuncia.LblTotPagare =
                                    datiTotDenuncia.LblTotDovuto + num5 - datiTotDenuncia.LblDecurtato;
                                datiTotDenuncia.LblSanzioni = num5;
                            }
                            else
                            {
                                DenunciaMensileBLL.log.Info((object)string.Format(
                                    "[TFI.BLL] : CaricaTotaliDenuncia() - La funzione GetTasso() ha generato un'eccezione in data: {0}",
                                    (object)DateTime.Now));
                                DenunciaMensileBLL.ErrorMessage = "La funzione GetTasso() ha generato un'eccezione";
                                return (DatiTotaliDenuncia)null;
                            }
                        }
                    }
                    else
                    {
                        int? nullable = outcome;
                        int num6 = 1;
                        if (nullable.GetValueOrDefault() == num6 & nullable.HasValue)
                        {
                            DenunciaMensileBLL.log.Info((object)string.Format(
                                "[TFI.BLL] : CaricaTotaliDenuncia() - La funzione GetDataScadenza() ha generato un'eccezione in data: {0}",
                                (object)DateTime.Now));
                            DenunciaMensileBLL.ErrorMessage = "La funzione GetDataScadenza() ha generato un'eccezione";
                            return (DatiTotaliDenuncia)null;
                        }
                    }
                }

                datiTotDenuncia.TxtDataVersamento_attr_today = DateTime.Now.ToString("d");
                if (totaliDenuncia.CodModPag != 0 && ret != "S" && totaliDenuncia.Esiret != "S")
                {
                    datiTotDenuncia.StaDen = "AP";
                    datiTotDenuncia.TxtDataVersamento_attr_value = totaliDenuncia.DatVer;
                    datiTotDenuncia.TxtImportoVersato_attr_value = totaliDenuncia.ImpVer.ToString();
                    if (totaliDenuncia.CodModPag == 1)
                    {
                        datiTotDenuncia.HdnCheck = "chkMav";
                        if (totaliDenuncia.CodLine != string.Empty)
                        {
                            datiTotDenuncia.BtnStampaMAVIsVisible = true;
                            datiTotDenuncia.BtnMAVIsVisible = false;
                            datiTotDenuncia.HdnMAV = "S";
                        }
                    }
                }

                switch (DenunciaMensileDAL.GetIBAN(codPos, out outcome))
                {
                    case null:
                        int? nullable1 = outcome;
                        int num7 = 1;
                        if (nullable1.GetValueOrDefault() == num7 & nullable1.HasValue)
                        {
                            DenunciaMensileBLL.log.Info((object)string.Format(
                                "[TFI.BLL] : CaricaTotaliDenuncia() - La funzione GetIBAN() ha generato un'eccezione in data: {0}",
                                (object)DateTime.Now));
                            DenunciaMensileBLL.ErrorMessage = "La funzione GetIBAN() ha generato un'eccezione";
                            return (DatiTotaliDenuncia)null;
                        }

                        break;
                    default:
                        if (ret == "S" && totaliDenuncia.Esiret == "S")
                        {
                            datiTotDenuncia.TbIntestazionePagamentoIsVisible = false;
                            datiTotDenuncia.TbPagamentoIsVisible = false;
                            datiTotDenuncia.BtnDettagliDipaIsVisible = false;
                            datiTotDenuncia.LblCreditiIsVisible = false;
                            datiTotDenuncia.TxtCreditiIsVisible = false;
                            datiTotDenuncia.LblContributi = totaliDenuncia.ImpCon + totaliDenuncia.ImpConDel;
                            datiTotDenuncia.LblAddizionale = totaliDenuncia.ImpAddRec + totaliDenuncia.ImpAddRecDel;
                            datiTotDenuncia.LblSanzioni = totaliDenuncia.ImpSanDet + totaliDenuncia.ImpSanRet;
                            datiTotDenuncia.LblTotaleIsVisible = false;
                            datiTotDenuncia.LblTotPagareIsVisible = false;
                            break;
                        }

                        break;
                }
            }
            else
            {
                int? nullable = outcome;
                int num8 = 1;
                if (nullable.GetValueOrDefault() == num8 & nullable.HasValue)
                {
                    DenunciaMensileBLL.log.Info((object)string.Format(
                        "[TFI.BLL] : CaricaTotaliDenuncia() - La funzione CaricaTotaliDenuncia_1() ha generato un'eccezione in data: {0}",
                        (object)DateTime.Now));
                    DenunciaMensileBLL.ErrorMessage = "La funzione CaricaTotaliDenuncia_1() ha generato un'eccezione";
                    return (DatiTotaliDenuncia)null;
                }
            }

            Decimal addizionale;
            if ((addizionale = DenunciaMensileDAL.GetAddizionale(out outcome)) > 0M)
            {
                outcome = new int?();
                datiTotDenuncia.LblDescrizAddizionale = string.Format("Addizionale del {0}%", (object)addizionale);
            }
            else
            {
                int? nullable = outcome;
                int num9 = 1;
                if (nullable.GetValueOrDefault() == num9 & nullable.HasValue)
                {
                    DenunciaMensileBLL.log.Info((object)string.Format(
                        "[TFI.BLL] : CaricaTotaliDenuncia() - La funzione GetAddizionale() ha generato un'eccezione in data: {0}",
                        (object)DateTime.Now));
                    DenunciaMensileBLL.ErrorMessage = "La funzione GetAddizionale() ha generato un'eccezione";
                    return (DatiTotaliDenuncia)null;
                }

                outcome = new int?();
            }

            timeStampDipa = DenunciaMensileDAL.GetTimeStampDipa(codPos, anno, mese, proDen, out outcome);
            int? nullable2 = outcome;
            int num10 = 1;
            if (nullable2.GetValueOrDefault() == num10 & nullable2.HasValue)
            {
                DenunciaMensileBLL.log.Info((object)string.Format(
                    "[TFI.BLL] : CaricaTotaliDenuncia() - La funzione GetTimeStampDipa() ha generato un'eccezione in data: {0}",
                    (object)DateTime.Now));
                DenunciaMensileBLL.ErrorMessage = "La funzione GetTimeStampDipa() ha generato un'eccezione";
                return (DatiTotaliDenuncia)null;
            }

            if (string.IsNullOrEmpty(timeStampDipa))
                timeStampDipa = "0";
            outcome = new int?();
            if (ConfigurationManager.AppSettings["PeriodiSenzaImporto"] == "True" && datiTotDenuncia.StaDen == "N")
            {
                datiTotDenuncia.PeriodiSenzaImporto =
                    DenunciaMensileDAL.GetPeriodiSenzaImporto(codPos, anno, mese, proDen, out outcome);
                nullable2 = outcome;
                int num11 = 1;
                if (nullable2.GetValueOrDefault() == num11 & nullable2.HasValue)
                {
                    DenunciaMensileBLL.log.Info((object)string.Format(
                        "[TFI.BLL] : CaricaTotaliDenuncia() - La funzione GetPeriodiSenzaImporto() ha generato un'eccezione in data: {0}",
                        (object)DateTime.Now));
                    DenunciaMensileBLL.ErrorMessage = "La funzione GetPeriodiSenzaImporto() ha generato un'eccezione";
                    return (DatiTotaliDenuncia)null;
                }

                outcome = new int?();
            }

            if (!string.IsNullOrEmpty(totaliDenuncia.NumMov))
            {
                if (datiTotDenuncia.LblTotPagare == 0.00M)
                    datiTotDenuncia.BtnMAVIsVisible = false;
                idDipa = DenunciaMensileDAL.GetIdDipa(codPos, anno, mese, proDen, out outcome);
                nullable2 = outcome;
                int num12 = 1;
                if (nullable2.GetValueOrDefault() == num12 & nullable2.HasValue)
                {
                    DenunciaMensileBLL.log.Info((object)string.Format(
                        "[TFI.BLL] : CaricaTotaliDenuncia() - La funzione GetIdDipa() ha generato un'eccezione in data: {0}",
                        (object)DateTime.Now));
                    DenunciaMensileBLL.ErrorMessage = "La funzione GetIdDipa() ha generato un'eccezione";
                    return (DatiTotaliDenuncia)null;
                }

                outcome = new int?();
                Decimal totFondoSanitario =
                    DenunciaMensileDAL.GetTotFondoSanitario(codPos, anno, mese, proDen, out outcome);
                nullable2 = outcome;
                int num13 = 1;
                if (nullable2.GetValueOrDefault() == num13 & nullable2.HasValue)
                {
                    DenunciaMensileBLL.log.Info((object)string.Format(
                        "[TFI.BLL] : CaricaTotaliDenuncia() - La funzione GetIdDipa() ha generato un'eccezione in data: {0}",
                        (object)DateTime.Now));
                    DenunciaMensileBLL.ErrorMessage = "La funzione GetIdDipa() ha generato un'eccezione";
                    return (DatiTotaliDenuncia)null;
                }

                outcome = new int?();
                datiTotDenuncia.LblTotFondo = !(totFondoSanitario > 0M) ? 0.00M : totFondoSanitario;
                datiTotDenuncia.MesFIA = mese;
                datiTotDenuncia.AnnFIA = anno;
                DatiSanitario dataSanit;
                if ((dataSanit = DenunciaMensileDAL.GetDataSanit(idDipa, out outcome)) != null)
                {
                    datiTotDenuncia.FondoSanitarioIsVisible = true;
                    datiTotDenuncia.BtnMAVSanitIsVisible = true;
                    datiTotDenuncia.BtnStampaMAVSanitIsVisible = false;
                    datiTotDenuncia.TbPagamentoSanitarioIsVisible = true;
                    datiTotDenuncia.TbIntestazionePagamentoFondoSanitarioIsVisible = true;
                    if (dataSanit.CodModPag == 1)
                    {
                        datiTotDenuncia.TxtDataVersamentoSanit = dataSanit.DatVer;
                        datiTotDenuncia.TxtImportoVersatoSanit = dataSanit.ImpVer;
                        datiTotDenuncia.TxtDataVersamentoSanit_readonly = true;
                        datiTotDenuncia.TxtImportoVersatoSanit_readonly = true;
                        datiTotDenuncia.BtnMAVSanitIsVisible = false;
                        datiTotDenuncia.BtnStampaMAVSanitIsVisible = true;
                    }
                    else if (dataSanit.CodModPag == 0)
                        datiTotDenuncia.TxtImportoVersatoSanit = 0.00M;
                }
                else
                {
                    nullable2 = outcome;
                    int num14 = 1;
                    if (nullable2.GetValueOrDefault() == num14 & nullable2.HasValue)
                    {
                        DenunciaMensileBLL.log.Info((object)string.Format(
                            "[TFI.BLL] : CaricaTotaliDenuncia() - La funzione GetDataSanit() ha generato un'eccezione in data: {0}",
                            (object)DateTime.Now));
                        DenunciaMensileBLL.ErrorMessage = "La funzione GetDataSanit() ha generato un'eccezione";
                        return (DatiTotaliDenuncia)null;
                    }

                    datiTotDenuncia.TbPagamentoSanitarioIsVisible = false;
                    datiTotDenuncia.BtnStampaMAVSanitIsVisible = false;
                    datiTotDenuncia.TbIntestazionePagamentoFondoSanitarioIsVisible = false;
                    datiTotDenuncia.FondoSanitarioIsVisible = false;
                    datiTotDenuncia.BtnMAVSanitIsVisible = false;
                }
            }
            else
            {
                Decimal num15;
                try
                {
                    num15 = Convert.ToDecimal(
                        DenunciaMensileDAL.GetTotSanitario(db, codPos, anno, mese, proDen, idDipa));
                }
                catch (Exception ex)
                {
                    DenunciaMensileBLL.log.Info((object)string.Format(
                        "[TFI.BLL] : CaricaTotaliDenuncia() - La funzione GetTotSanitario() ha generato un'eccezione in data: {0}",
                        (object)DateTime.Now));
                    DenunciaMensileBLL.ErrorMessage = "La funzione GetTotSanitario() ha generato un'eccezione";
                    return (DatiTotaliDenuncia)null;
                }

                datiTotDenuncia.LblTotFondo = !(num15 > 0M) ? 0.00M : num15;
                datiTotDenuncia.LblTotPagare = datiTotDenuncia.LblTotDovuto;
                datiTotDenuncia.MesFIA = mese;
                datiTotDenuncia.AnnFIA = anno;
                DatiSanitario dataSanit2;
                if ((dataSanit2 = DenunciaMensileDAL.GetDataSanit2(idDipa, out outcome)) != null)
                {
                    outcome = new int?();
                    datiTotDenuncia.IdDipaDef = dataSanit2.IdDipaDef;
                    if (dataSanit2.IdDipaDef != 0)
                    {
                        DatiSanitario dataSanit;
                        if ((dataSanit = DenunciaMensileDAL.GetDataSanit(dataSanit2.IdDipaDef, out outcome)) != null)
                        {
                            datiTotDenuncia.FondoSanitarioIsVisible = true;
                            datiTotDenuncia.BtnMAVSanitIsVisible = true;
                            datiTotDenuncia.BtnStampaMAVSanitIsVisible = false;
                            datiTotDenuncia.TbPagamentoSanitarioIsVisible = true;
                            datiTotDenuncia.TbIntestazionePagamentoFondoSanitarioIsVisible = true;
                            if (dataSanit.CodModPag == 1)
                            {
                                datiTotDenuncia.TxtDataVersamentoSanit = dataSanit.DatVer;
                                datiTotDenuncia.TxtImportoVersatoSanit = dataSanit.ImpVer;
                                datiTotDenuncia.TxtDataVersamentoSanit_readonly = true;
                                datiTotDenuncia.TxtImportoVersatoSanit_readonly = true;
                                datiTotDenuncia.BtnMAVSanitIsVisible = false;
                                datiTotDenuncia.BtnStampaMAVSanitIsVisible = true;
                            }
                            else if (dataSanit.CodModPag == 0)
                                datiTotDenuncia.TxtImportoVersatoSanit = 0.00M;
                        }
                    }
                    else
                    {
                        datiTotDenuncia.TbPagamentoSanitarioIsVisible = false;
                        datiTotDenuncia.BtnStampaMAVSanitIsVisible = false;
                        datiTotDenuncia.TbIntestazionePagamentoFondoSanitarioIsVisible = false;
                        datiTotDenuncia.FondoSanitarioIsVisible = false;
                        datiTotDenuncia.BtnMAVSanitIsVisible = false;
                    }
                }
                else
                {
                    nullable2 = outcome;
                    int num16 = 1;
                    if (nullable2.GetValueOrDefault() == num16 & nullable2.HasValue)
                    {
                        DenunciaMensileBLL.log.Info((object)string.Format(
                            "[TFI.BLL] : CaricaTotaliDenuncia() - La funzione GetDataSanit() ha generato un'eccezione in data: {0}",
                            (object)DateTime.Now));
                        DenunciaMensileBLL.ErrorMessage = "La funzione GetDataSanit() ha generato un'eccezione";
                        return (DatiTotaliDenuncia)null;
                    }

                    datiTotDenuncia.BtnMAVSanitIsVisible = false;
                    datiTotDenuncia.BtnStampaMAVSanitIsVisible = false;
                    datiTotDenuncia.TbPagamentoSanitarioIsVisible = false;
                    datiTotDenuncia.TbIntestazionePagamentoFondoSanitarioIsVisible = false;
                    datiTotDenuncia.FondoSanitarioIsVisible = false;
                }
            }

            return datiTotDenuncia;
        }

        public static void InitPage(DatiTotaliDenuncia totDenuncia)
        {
            totDenuncia.TxtDataVersamento = Convert.ToDateTime(totDenuncia.TxtDataVersamento).ToString("d");
            string staDen = totDenuncia.StaDen;
            if (!(staDen == "N"))
            {
                if (!(staDen == "A"))
                {
                    if (!(staDen == "AP"))
                        return;
                    totDenuncia.BtnCreditiIsVisible = false;
                    totDenuncia.TxtCrediti_readonly = true;
                    totDenuncia.TbIntestazionePagamentoIsVisible = true;
                    totDenuncia.TbPagamentoIsVisible = true;
                    totDenuncia.BtnConfermaTotaliIsVisible = false;
                    totDenuncia.TxtDataVersamento_readonly = true;
                    totDenuncia.TxtImportoVersato_readonly = true;
                }
                else
                {
                    totDenuncia.BtnCreditiIsVisible = false;
                    totDenuncia.TbIntestazionePagamentoIsVisible = true;
                    totDenuncia.TbPagamentoIsVisible = true;
                    totDenuncia.RigaSanzioniIsVisible = true;
                    if (totDenuncia.LblDecurtato > 0M)
                        totDenuncia.BtnRipristinaImportoIsVisible = true;
                    totDenuncia.BtnConfermaTotaliIsVisible = false;
                    totDenuncia.TxtImportoVersato = 0.00M;
                }
            }
            else
                totDenuncia.RigaSanzioniIsVisible = false;
        }

        public static bool SalvaTotaleDipa(
          Utente utente,
          List<RetribuzioneRDL> listaReport,
          int anno,
          int mese,
          int proDen,
          string sessionTimeStampDipa,
          decimal txtCrediti,
          decimal lblTotFondo,
          int annFia,
          ref string staDen)
        {
            int PROGMOV = 0;
            int num1 = 0;
            int IDPOL = 0;
            int annoBilancio = 0;
            int annCom = 0;
            int num4 = 0;
            string codPosizione = utente.CodPosizione;
            string empty1 = string.Empty;
            string strContabilizza = string.Empty;
            string str2 = string.Empty;
            string empty2 = string.Empty;
            string TIPO_OPERAZIONE = string.Empty;
            string strDatSca = string.Empty;
            string empty3 = string.Empty;
            string empty4 = string.Empty;
            string partitaMov = string.Empty;
            string codCaus = string.Empty;
            string empty5 = string.Empty;
            string empty6 = string.Empty;
            string empty7 = string.Empty;
            bool flag1 = false;
            bool flag2 = false;
            decimal impAddRec = 0M;
            decimal impAssCon = 0M;
            decimal impAbb = 0M;
            List<DatiContabilita> datiContabilitaList = null;
            List<DatiContabilita> datiContabilita1 = new List<DatiContabilita>();

            //clsIDOC iDoc = (clsIDOC)null;
            //bool sanitario = lblTotFondo != 0.00M;

            if (DenunciaMensileDAL.CheckNotificheUfficio(codPosizione, anno, mese, out var outcome1) != 0)
            {
                ErrorMessage = "Impossibile salvare! Per il periodo è stata inserita una notifica di ufficio";
                return false;
            }
            int? nullable1 = outcome1;
            int num8 = 1;
            if (nullable1.GetValueOrDefault() == num8 & nullable1.HasValue)
            {
                log.Info($"[TFI.BLL] : SalvaTotaleDIPA() - La funzione CheckNotificheUfficio() ha generato un'eccezione in data: {DateTime.Now}");
                ErrorMessage = "La funzione CheckNotificheUfficio() ha generato un'eccezione";
                return false;
            }

            Dentes_Data dataFromDentes = DenunciaMensileDAL.GetDataFromDENTES(codPosizione, anno, mese, proDen, out var outcome2);
            
            if (dataFromDentes != null)
            {
                if (dataFromDentes.TimeStamp == sessionTimeStampDipa)
                {
                    if (VerificaDipa_2(listaReport, codPosizione, anno, mese, proDen))
                    {
                        int? outcome3;
                        DataLayer db1 = new DataLayer();
                        bool blnRet;
                        try
                        {
                            db1.StartTransaction();

                            string timeStampDipa2 = DenunciaMensileDAL.GetTimeStampDipa2();

                            string datChi = utente.Tipo != "A" 
                                ? dataFromDentes.TmstApe 
                                : timeStampDipa2;

                            blnRet = WriteDIPA_SalvataggioTotale.WRITE_CONFERMA_DENUNCIA(db1, utente, Convert.ToInt32(codPosizione), anno, mese, proDen, "DP", datChi, utente.Tipo, "A", txtCrediti, timeStampDipa2, "RITARDO", string.Empty);
                            
                            if (blnRet)
                            {
                                blnRet = WriteDIPA_SalvataggioTotale.WRITE_INSERT_ALISOS(db1, utente, Convert.ToInt32(codPosizione), anno, mese, proDen);
                            
                                if (blnRet)
                                {
                                    strContabilizza = DenunciaMensileDAL.GetContabilizAutomatica().Trim();

                                    if (strContabilizza == "S")
                                    {
                                        str2 = "prima di contabilizzare";
                                        string tipMov = dataFromDentes.TipMov.Trim();
                                        decimal impDis = dataFromDentes.ImpDis;
                                        impAbb = dataFromDentes.ImpAbb;
                                        impAddRec = dataFromDentes.ImpAddRec;
                                        impAssCon = dataFromDentes.ImpAssCon;
                                        if (dataFromDentes.TipMov.Trim() == "AR")
                                        {
                                            List<ParametriGenerali> parametriGenerali =
                                                DenunciaMensileDAL.GetParametriGenerali(codPosizione, out outcome3);
                                            int? nullable3 = outcome3;
                                            int num9 = 1;
                                            if (nullable3.GetValueOrDefault() == num9 & nullable3.HasValue)
                                                throw new Exception();

                                            outcome3 = new int?();
                                            decimal importoParametro = GetImportoParametro(parametriGenerali, 5, DateTime.Now.ToString("d"));
                                            annoBilancio = DenunciaMensileDAL.GetAnnoBilancio(DateTime.Now.Year, out outcome3);
                                            datiContabilitaList = DenunciaMensileDAL.GetDatiContabilita(importoParametro, annoBilancio, codPosizione, anno, mese, proDen, out outcome3);
                                        }
                                        else if (dataFromDentes.TipMov.Trim() == "DP")
                                        {
                                            annoBilancio = DenunciaMensileDAL.GetAnnoBilancio(anno, out outcome3);
                                            datiContabilitaList = DenunciaMensileDAL.GetDatiContabilita2(annoBilancio, codPosizione, anno, mese, proDen, out outcome3);
                                        }

                                        if (dataFromDentes.TipMov.Trim() == "AR")
                                        {
                                            foreach (DatiContabilita datiContabilita2 in datiContabilitaList)
                                            {
                                                if (annCom != datiContabilita2.AnnCom)
                                                {
                                                    if (annCom != 0)
                                                        datiContabilita1.Add(new DatiContabilita()
                                                        {
                                                            TipMov = dataFromDentes.TipMov.Trim(),
                                                            AnnCom = annCom,
                                                            ImpAddRec = impAddRec,
                                                            ImpAssCon = impAssCon,
                                                            ImpAbb = impAbb,
                                                            AnnBil = annoBilancio
                                                        });
                                                    impAddRec = 0M;
                                                    impAssCon = 0M;
                                                    impAbb = 0M;
                                                    annCom = datiContabilita2.AnnCom;
                                                }
                                                impAddRec += datiContabilita2.ImpAddRec;
                                                impAssCon += datiContabilita2.ImpAssCon;
                                                impAbb += datiContabilita2.ImpAbb;
                                            }

                                            datiContabilita1.Add(new DatiContabilita()
                                            {
                                                TipMov = dataFromDentes.TipMov.Trim(),
                                                AnnCom = annCom,
                                                ImpAddRec = impAddRec,
                                                ImpAssCon = impAssCon,
                                                ImpAbb = impAbb,
                                                AnnBil = annoBilancio
                                            });
                                        }
                                        else
                                            datiContabilita1 = datiContabilitaList;

                                        string codCau = DenunciaMensileDAL.GetCodCau(db1, dataFromDentes.TipMov.Trim());
                                        if (dataFromDentes.TipMov.Trim() == "DP")
                                        {
                                            strDatSca = DenunciaMensileDAL.GetDataScadenzaDenuncia(db1, mese, anno);
                                            TIPO_OPERAZIONE = "DIPA";
                                        }
                                        else if (dataFromDentes.TipMov.Trim() == "AR")
                                        {
                                            strDatSca = DenunciaMensileDAL.GetDataScadenzaDenuncia2(db1, mese, anno, codPosizione, proDen);
                                            TIPO_OPERAZIONE = "ARRETRATO";
                                        }
                                        string numMov = WriteDIPA_SalvataggioTotale.WRITE_INSERT_MOVIMSAP(db1, utente, Convert.ToInt32(codPosizione), anno, mese, proDen, codCau, DateTime.Now.ToString("d"), annoBilancio, strDatSca, impDis, impAbb, impAddRec, impAssCon, "N", "N", TIPO_OPERAZIONE, ref empty5, ref PROGMOV, ref datiContabilita1);
                                        partitaMov = empty5;
                                        num4 = PROGMOV;
                                        codCaus = codCau;
                                        if (!string.IsNullOrEmpty(numMov) && 
                                            DenunciaMensileDAL.AggiornaContabilitaDENTES(db1, codPosizione, anno, mese, proDen, codCaus, numMov, strDatSca, annoBilancio, tipMov) && 
                                            DenunciaMensileDAL.AggiornaContabilitaDENDET(db1, codPosizione, anno, mese, proDen, numMov, tipMov, PROGMOV, empty5))
                                        {
                                            Tuple<decimal, string, string> tuple = 
                                                DenunciaMensileDAL.ControllaSanzioni(db1, codPosizione, anno, mese, proDen, tipMov);
                                            
                                            decimal importoSanzione = tuple.Item1;
                                            string sanSotSog = tuple.Item2;
                                            
                                            if (importoSanzione > 0M && sanSotSog == "N")
                                            {
                                                string ricSanUte = tuple.Item3.Trim().ToUpper();
                                                if (ricSanUte != "ESCLUDI")
                                                {
                                                    string CODCAU = 
                                                        ricSanUte == "RITARDO" 
                                                            ? DenunciaMensileDAL.GetCausaleSanzione(db1, "SAN_RD") 
                                                            : (ricSanUte == "OMISSIONE" 
                                                                ? DenunciaMensileDAL.GetCausaleSanzione(db1, "SAN_MD") 
                                                                : DenunciaMensileDAL.GetCausaleSanzione(db1, "SAN_RD"));

                                                    empty5 = string.Empty;
                                                    PROGMOV = 0;
                                                    string numSan = WriteDIPA_SalvataggioTotale.WRITE_INSERT_MOVIMSAP(db1, utente, Convert.ToInt32(codPosizione), anno, mese, proDen, CODCAU, DateTime.Now.ToString("d"), annoBilancio, strDatSca, importoSanzione, 0M, 0M, 0M, "S", "N", TIPO_OPERAZIONE, ref empty5, ref PROGMOV, ref datiContabilita1);
                                                    blnRet = !string.IsNullOrEmpty(numSan) && 
                                                             DenunciaMensileDAL.AggiornaTestataEDettaglio(db1, codPosizione, anno, mese, proDen, numSan, tipMov, PROGMOV, empty5, annoBilancio);
                                                }
                                            }
                                        }

                                        if (blnRet && !string.IsNullOrEmpty(numMov))
                                            blnRet = DenunciaMensileDAL.AggiornaMODPREDET(db1, codPosizione, anno, mese,
                                                proDen, numMov);
                                    }
                                    //if (blnRet)
                                    //{
                                    //    DataTable idocDataTable = DenunciaMensileDAL.GetIDocDataTable(db1, utente, anno, mese, proDen, out iDoc);
                                    //    if (idocDataTable != null)
                                    //    {
                                    //        foreach (DataRow row in (InternalDataCollectionBase)idocDataTable.Rows)
                                    //        {
                                    //            DenunciaMensileDAL.WriteIDocTestata(db1, row, iDoc);
                                    //            DataTable dt_mat = idocDataTable.Clone();
                                    //            dt_mat.ImportRow(row);
                                    //            DenunciaMensileDAL.WRITE_IDOC_E1PITYP(db1, dt_mat, iDoc);
                                    //        }
                                    //        DenunciaMensileDAL.Aggiorna_IDOC(db1, iDoc);
                                    //    }
                                    //}
                                }
                            }
                            if (blnRet && strContabilizza == "S")
                            {
                                if (DenunciaMensileDAL.VerificaContabilizzazione(db1, partitaMov, num4, codCaus) == 0)
                                {
                                    string empty8 = string.Empty;
                                    PROGMOV = 0;
                                    string empty9 = string.Empty;
                                    string tipMov = dataFromDentes.TipMov.Trim();
                                    decimal impDis = dataFromDentes.ImpDis;
                                    impAbb = dataFromDentes.ImpAbb;
                                    impAddRec = dataFromDentes.ImpAddRec;
                                    impAssCon = dataFromDentes.ImpAssCon;
                                    if (dataFromDentes.TipMov.Trim() == "AR")
                                    {
                                        List<ParametriGenerali> parametriGenerali =
                                            DenunciaMensileDAL.GetParametriGenerali(codPosizione, out outcome3);
                                        int? nullable4 = outcome3;
                                        int num11 = 1;
                                        if (nullable4.GetValueOrDefault() == num11 & nullable4.HasValue)
                                            throw new Exception();

                                        outcome3 = default;
                                        decimal importoParametro = GetImportoParametro(parametriGenerali, 5, DateTime.Now.ToString("d"));

                                        annoBilancio = DenunciaMensileDAL.GetAnnoBilancio(DateTime.Now.Year, out outcome3);
                                        datiContabilitaList = DenunciaMensileDAL.GetDatiContabilita(importoParametro, annoBilancio, codPosizione, anno, mese, proDen, out outcome3);
                                    }
                                    else if (dataFromDentes.TipMov.Trim() == "DP")
                                    {
                                        annoBilancio = DenunciaMensileDAL.GetAnnoBilancio(anno, out outcome3);
                                        datiContabilitaList = DenunciaMensileDAL.GetDatiContabilita2(annoBilancio, codPosizione, anno, mese, proDen, out outcome3);
                                    }

                                    if (dataFromDentes.TipMov.Trim() == "AR")
                                    {
                                        foreach (DatiContabilita datiContabilita3 in datiContabilitaList)
                                        {
                                            if (annCom != datiContabilita3.AnnCom)
                                            {
                                                if (annCom != 0)
                                                    datiContabilita1.Add(new DatiContabilita()
                                                    {
                                                        TipMov = dataFromDentes.TipMov.Trim(),
                                                        AnnCom = annCom,
                                                        ImpAddRec = impAddRec,
                                                        ImpAssCon = impAssCon,
                                                        ImpAbb = impAbb,
                                                        AnnBil = annoBilancio
                                                    });
                                                impAddRec = 0M;
                                                impAssCon = 0M;
                                                impAbb = 0M;
                                                annCom = datiContabilita3.AnnCom;
                                            }
                                            impAddRec += datiContabilita3.ImpAddRec;
                                            impAssCon += datiContabilita3.ImpAssCon;
                                            impAbb += datiContabilita3.ImpAbb;
                                        }

                                        datiContabilita1.Add(new DatiContabilita()
                                        {
                                            TipMov = dataFromDentes.TipMov.Trim(),
                                            AnnCom = annCom,
                                            ImpAddRec = impAddRec,
                                            ImpAssCon = impAssCon,
                                            ImpAbb = impAbb,
                                            AnnBil = annoBilancio
                                        });
                                    }
                                    else
                                        datiContabilita1 = datiContabilitaList;

                                    string codCau = DenunciaMensileDAL.GetCodCau(db1, dataFromDentes.TipMov.Trim());
                                    
                                    if (dataFromDentes.TipMov.Trim() == "DP")
                                    {
                                        strDatSca = DenunciaMensileDAL.GetDataScadenzaDenuncia(db1, mese, anno);
                                        TIPO_OPERAZIONE = "DIPA";
                                    }
                                    else if (dataFromDentes.TipMov.Trim() == "AR")
                                    {
                                        strDatSca = DenunciaMensileDAL.GetDataScadenzaDenuncia2(db1, mese, anno, codPosizione, proDen);
                                        TIPO_OPERAZIONE = "ARRETRATO";
                                    }
                                    
                                    string numMov = WriteDIPA_SalvataggioTotale.WRITE_INSERT_MOVIMSAP(db1, utente, Convert.ToInt32(codPosizione), anno, mese, proDen, codCau, DateTime.Now.ToString("d"), annoBilancio, strDatSca, impDis, impAbb, impAddRec, impAssCon, "N", "N", TIPO_OPERAZIONE, ref empty5, ref PROGMOV, ref datiContabilita1);
                                    
                                    partitaMov = empty5;
                                    num4 = PROGMOV;
                                    codCaus = codCau;

                                    if (!string.IsNullOrEmpty(numMov) && 
                                        DenunciaMensileDAL.AggiornaContabilitaDENTES(db1, codPosizione, anno, mese, proDen, codCaus, numMov, strDatSca, annoBilancio, tipMov) && 
                                        DenunciaMensileDAL.AggiornaContabilitaDENDET(db1, codPosizione, anno, mese, proDen, numMov, tipMov, PROGMOV, empty5))
                                    {
                                        Tuple<decimal, string, string> tuple = DenunciaMensileDAL.ControllaSanzioni(db1, codPosizione, anno, mese, proDen, tipMov);
                                        decimal IMPORTO = tuple.Item1;
                                        string str5 = tuple.Item2;
                                        if (IMPORTO > 0M && str5 == "N")
                                        {
                                            string upper = tuple.Item3.Trim().ToUpper();
                                            if (upper != "ESCLUDI")
                                            {
                                                string CODCAU = upper == "RITARDO" 
                                                    ? DenunciaMensileDAL.GetCausaleSanzione(db1, "SAN_RD") 
                                                    : (upper == "OMISSIONE" 
                                                        ? DenunciaMensileDAL.GetCausaleSanzione(db1, "SAN_MD") 
                                                        : DenunciaMensileDAL.GetCausaleSanzione(db1, "SAN_RD"));
                                                int annBillSan = annoBilancio;
                                                empty5 = string.Empty;
                                                PROGMOV = 0;
                                                string numSan = WriteDIPA_SalvataggioTotale.WRITE_INSERT_MOVIMSAP(db1, utente, Convert.ToInt32(codPosizione), anno, mese, proDen, CODCAU, DateTime.Now.ToString("d"), annBillSan, strDatSca, IMPORTO, 0M, 0M, 0M, "S", "N", TIPO_OPERAZIONE, ref empty5, ref PROGMOV, ref datiContabilita1);
                                                blnRet = !string.IsNullOrEmpty(numSan) && 
                                                         DenunciaMensileDAL.AggiornaTestataEDettaglio(db1, codPosizione, anno, mese, proDen, numSan, tipMov, PROGMOV, empty5, annBillSan);
                                            }
                                        }
                                    }

                                    if (blnRet && !string.IsNullOrEmpty(numMov))
                                        blnRet = DenunciaMensileDAL.AggiornaMODPREDET(db1, codPosizione, anno, mese, proDen, numMov);
                                }
                                else
                                    blnRet = true;

                                DenunciaMensileDAL.VerificaContabilizzazione2(db1, codPosizione, anno, mese, proDen, partitaMov, PROGMOV, codCaus, ref blnRet);
                            }

                            if (blnRet)
                            {
                                foreach (RetribuzioneRDL retribuzioneRdl in DenunciaMensileDAL.GetDataFromDENDET_2(db1, codPosizione, anno, mese, proDen))
                                {
                                    if (!WriteDIPA_SalvataggioTotale.AGGIORNA_RETANN(
                                            db1, 
                                            utente,
                                            retribuzioneRdl.CodPos, 
                                            retribuzioneRdl.Mat.ToString(),
                                            retribuzioneRdl.ProRap,
                                            Convert.ToInt32(retribuzioneRdl.AnnDen),
                                            retribuzioneRdl.ImpRet,
                                            retribuzioneRdl.ImpOcc,
                                            retribuzioneRdl.ImpFig,
                                            "+"))
                                    {
                                        blnRet = false;
                                        break;
                                    }
                                }
                            }
                            //if (DenunciaMensileDAL.CountRecordsFromDIPATESTMP(db1, codPosizione, anno, mese, proDen) == 1 && DenunciaMensileDAL.CountRecordsFromDIPATES(db1, codPosizione, anno, mese, proDen) == 0)
                            //{
                            //    flag2 = true;
                            //    blnRet = false;
                            //}
                        }
                        catch (Exception ex)
                        {
                            string.IsNullOrEmpty(WriteDIPA_SalvataggioTotale.Trace);

                            db1.EndTransaction(false);
                            
                            log.Info($"[TFI.BLL] : SalvaTotaleDIPA() - La funzione di salvataggio totale ha generato un'eccezione in data: {DateTime.Now}");
                            ErrorMessage = $"La funzione di salvataggio totale ha generato un'eccezione: {ex.Message}";
                            return false;
                        }

                        if (blnRet)
                        {
                            if (!DenunciaMensileDAL.Aggiorna_Contabilita_Dettaglio(db1, codPosizione, anno, mese, proDen))
                            {
                                log.Info($"[TFI.BLL] : SalvaTotaleDIPA() - La funzione Aggiorna_Contabilita_Dettaglio() ha generato un'eccezione in data: {DateTime.Now}");
                                ErrorMessage = "La funzione Aggiorna_Contabilita_Dettaglio() ha generato un'eccezione";
                                db1.EndTransaction(false);
                                return false;
                            }

                            staDen = "A";
                            
                            SuccessMessage = "Conferma dati ad Enpaia effettuata.";

                            var denTes = DenunciaMensileDAL.GetDataFromDENTES(codPosizione, anno, mese, proDen, out var outcome);
                            
                            
                            var sendEmailResult = InviaEmailConfermaDipa(utente, denTes);
                            if (!sendEmailResult.Success)
                            {
                                log.Info($"[TFI.BLL] : InvioRicevutaDipa() - errore invio mail in data: {DateTime.Now} - messaggio: {sendEmailResult.Message}");
                                WarningMessage = "Invio Mail non riuscito";
                            }
                            db1.EndTransaction(true);
                            return true;
                        }
                        ErrorMessage = "Conferma ad Enpaia non effettuata.";
                        db1.EndTransaction(false);
                        return false;
                    }
                    ErrorMessage = "Impossibile salvare! Sono state apportate variazioni ai trattamenti economici del periodo.";
                    return false;
                }
                ErrorMessage = "Impossibile salvare! I dati del DIPA sono stati modificati da un altro utente.";
                return false;
            }

            int? nullable5 = outcome2;
            int num15 = 1;
            if (nullable5.GetValueOrDefault() == num15 & nullable5.HasValue)
            {
                log.Info($"[TFI.BLL] : SalvaTotaleDIPA() - La funzione GetDataFromDENTES() ha generato un'eccezione in data: {DateTime.Now}");
                ErrorMessage = "La funzione GetDataFromDENTES() ha generato un'eccezione";
                return false;
            }

            ErrorMessage = "Impossibile salvare! La denuncia mensile è stata eliminata o sostituita da una notifica di ufficio.";
            return false;
        }

        public static bool AggiornaCreditoDecurtato(
            string codPos,
            int anno,
            int mese,
            int proDen,
            Decimal txtCrediti)
        {
            if (DenunciaMensileDAL.AggiornaCreditoDecurtato(codPos, anno, mese, proDen,
                    txtCrediti.ToString().Replace(',', '.')))
                return true;
            DenunciaMensileBLL.log.Info((object)string.Format(
                "[TFI.BLL] : SalvaTotaleDIPA() - La funzione AggiornaCreditoDecurtato() ha generato un'eccezione in data: {0}",
                (object)DateTime.Now));
            DenunciaMensileBLL.ErrorMessage = "La funzione AggiornaCreditoDecurtato() ha generato un'eccezione";
            return false;
        }

        public static bool RipristinaImporto(string codPos, int anno, int mese, int proDen)
        {
            if (DenunciaMensileDAL.RipristinaImporto(codPos, anno, mese, proDen))
                return true;
            DenunciaMensileBLL.log.Info((object)string.Format(
                "[TFI.BLL] : SalvaTotaleDIPA() - La funzione RipristinaImporto() ha generato un'eccezione in data: {0}",
                (object)DateTime.Now));
            DenunciaMensileBLL.ErrorMessage = "La funzione RipristinaImporto() ha generato un'eccezione";
            return false;
        }

        public static bool VerificaImponibile(List<RetribuzioneRDL> listaReport)
        {
            bool flag = false;
            Decimal num1 = 0M;
            Decimal num2 = 0M;
            if (DenunciaMensileBLL.SospensioniImportoZero(listaReport))
                return false;
            if (listaReport.Count != 0)
            {
                foreach (RetribuzioneRDL retribuzioneRdl in listaReport)
                {
                    if (retribuzioneRdl.ImpRet == 0M)
                        num1 = !(retribuzioneRdl.TipSpe == "S") ? retribuzioneRdl.ImpTraEco : retribuzioneRdl.ImpMin;
                    num2 += retribuzioneRdl.ImpRet;
                    num2 += retribuzioneRdl.ImpFig;
                }

                if (num2 == 0M)
                    flag = true;
            }

            return flag;
        }

        public static int GetNumeroDenunceDellAnno(string codPos, int anno) =>
            DenunciaMensileDAL.GetNumeroDenunceDellAnno(codPos, anno);

        public static Dictionary<int, string> GetListaMesi() => Utils.GetMesi();

        public static bool SalvaDIPA_upload(
            TFI.OCM.Utente.Utente utente,
            int anno,
            int mese,
            HttpPostedFileBase dipa,
            string path,
            ref string proDen,
            ref bool btnStampaIsVisible,
            ref bool btnConfermaIsVisible)
        {
            try
            {
                if (DenunciaMensileDAL.SalvaDIPA_upload(utente, utente.CodPosizione, anno, mese, dipa, path, ref proDen,
                        ref btnStampaIsVisible, ref btnConfermaIsVisible))
                {
                    DenunciaMensileBLL.SuccessMessage = "Operazione effettuata con successo!";
                    return true;
                }

                DenunciaMensileBLL.ErrorMessage = "Caricamento della denuncia mensile non riuscito! " +
                                                  Environment.NewLine + DenunciaMensileDAL.ErrorMessage;
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static RicercaArretrato Ricerca(
            string codPos,
            out int? outcome,
            int anno = 0,
            int mese = 0,
            int progressivo = 0)
        {
            try
            {
                return DenunciaMensileDAL.RicercaArretrato(codPos, out outcome, anno, mese, progressivo);
            }
            catch (Exception ex)
            {
                log.Info($"[TFI.BLL] : Ricerca() - La funzione Ricerca() ha generato un'eccezione in data: {DateTime.Now}");
                ErrorMessage = "La funzione Ricerca() ha generato un'eccezione";
                outcome = 1;
                return null;
            }
        }

        public static RettificheDIPA CaricaDatiRettifiche(
            string codPos,
            int anno,
            int mese,
            int proDen)
        {
            try
            {
                int? outcome = new int?();
                RettificheDIPA rettificheDipa1 = DenunciaMensileDAL.GetRettificheDIPA_1(codPos, anno, mese, proDen);
                if (rettificheDipa1 != null)
                {
                    if (!string.IsNullOrEmpty(rettificheDipa1.TipMov))
                    {
                        rettificheDipa1.TipMov = rettificheDipa1.TipMov.Trim();
                        if (rettificheDipa1.TipMov == "DP")
                        {
                            rettificheDipa1.Periodo = "Rettifiche Denuncia Mensile relativa al Periodo ";
                            rettificheDipa1.BtnOriginale = "Denuncia Originale";
                        }
                        else if (rettificheDipa1.TipMov == "NU")
                        {
                            rettificheDipa1.Periodo = "Rettifiche Notifica di Ufficio relativa al Periodo ";
                            rettificheDipa1.BtnOriginale = "Notifica di Ufficio Originale";
                        }

                        RettificheDIPA rettificheDipa = rettificheDipa1;
                        rettificheDipa.Periodo = rettificheDipa.Periodo + Utils.GetMesi()[mese].ToUpper() + " " +
                                                 anno.ToString();
                    }

                    if (!string.IsNullOrEmpty(rettificheDipa1.DatApe))
                        rettificheDipa1.DatApe = DateTime.Parse(rettificheDipa1.DatApe).ToString("d");
                    DenunciaMensileDAL.GetRettificheDIPA_2(rettificheDipa1, codPos);
                    SediAziendali sedeLegale = DenunciaMensileDAL.GetSedeLegale(codPos, out outcome);
                    int? nullable1 = outcome;
                    int num1 = 1;
                    if (nullable1.GetValueOrDefault() == num1 & nullable1.HasValue)
                        throw new Exception();
                    int? nullable2 = outcome;
                    int num2 = 0;
                    if (nullable2.GetValueOrDefault() == num2 & nullable2.HasValue)
                        rettificheDipa1.SedeLegale = sedeLegale.Indirizzo + " " + sedeLegale.Localita + " " +
                                                     sedeLegale.Provincia + " tel. " + sedeLegale.Telefono;
                    return rettificheDipa1;
                }

                DenunciaMensileBLL.ErrorMessage = "Dati non trovati!";
                return (RettificheDIPA)null;
            }
            catch (Exception ex)
            {
                DenunciaMensileBLL.log.Info((object)string.Format(
                    "[TFI.BLL] : CaricaDatiRettifiche() - La funzione CaricaDatiRettifiche() ha generato un'eccezione in data: {0}",
                    (object)DateTime.Now));
                DenunciaMensileBLL.ErrorMessage = "Caricamento dei dati delle rettifiche non riuscito!: " + ex.Message;
                return (RettificheDIPA)null;
            }
        }

        public static List<DatiRettificheDIPA> CaricaRettifiche(
            string codPos,
            int anno,
            int mese,
            int proDen)
        {
            try
            {
                return DenunciaMensileDAL.GetRettificheDIPA_3(codPos, anno, mese, proDen);
            }
            catch (Exception ex)
            {
                DenunciaMensileBLL.log.Info((object)string.Format(
                    "[TFI.BLL] : CaricaRettifiche() - La funzione CaricaRettifiche() ha generato un'eccezione in data: {0}",
                    (object)DateTime.Now));
                DenunciaMensileBLL.ErrorMessage = "Caricamento delle rettifiche non riuscito! : " + ex.Message;
                return (List<DatiRettificheDIPA>)null;
            }
        }

        public static bool EliminaArretrato(Utente utente, int anno, int mese, int proDen)
        {
            DataLayer db = new DataLayer();
            int? nullable = new int?();
            try
            {
                db.StartTransaction();
                if (DenunciaMensileDAL.EliminaArretrato(db, utente.CodPosizione, anno, mese, proDen))
                {
                    db.EndTransaction(true);
                    return true;
                }

                db.EndTransaction(false);
                ErrorMessage = "Si sono verificati dei problemi nella cancellazione della denuncia.";
                return false;
            }
            catch (Exception ex)
            {
                db.EndTransaction(false);
                log.Info($"[TFI.BLL] : EliminaDIPA - La funzione EliminaDIPA() ha generato un'eccezione in data: {DateTime.Now}");
                ErrorMessage = "E' stata generata un'eccezione in fase di cancellazione della denuncia.";
                return false;
            }
        }

        public static string GetMatricolaByCodFiscale(string cFiscale)
        {
            int? outcome = new int?();
            try
            {
                return DenunciaMensileDAL.GetMatricolaByCodFiscale(cFiscale, out outcome);
            }
            catch (Exception ex)
            {
                DenunciaMensileBLL.log.Info((object)string.Format(
                    "[TFI.BLL] : GetMatricolaByCodFiscale - La funzione GetMatricolaByCodFiscale() ha generato un'eccezione in data: {0}",
                    (object)DateTime.Now));
                DenunciaMensileBLL.ErrorMessage = "E' stata generata un'eccezione in fase di recupero della Matricola.";
                return string.Empty;
            }
        }

        private static bool VerificaDipa_2(
            List<RetribuzioneRDL> listaReport,
            string codPos,
            int anno,
            int mese,
            int proDen)
        {
            List<RetribuzioneRDL> dataFromDendet = DenunciaMensileDAL.GetDataFromDENDET(codPos, anno, mese, proDen, out var outcome);
            int? nullable = outcome;
            int num = 1;
            if (nullable.GetValueOrDefault() == num & nullable.HasValue)
            {
                log.Info($"[TFI.BLL] : SalvaTotaleDIPA() - La funzione GetDataFromDENDET() ha generato un'eccezione in data: {DateTime.Now}");
                ErrorMessage = "La funzione GetDataFromDENDET() ha generato un'eccezione";
                return false;
            }

            return VerificaDenuncia(dataFromDendet, ref listaReport, codPos, anno, mese, proDen);
        }

        public static decimal GetImportoParametro(int codPar, string dal, string flag = "S")
        {
            DateTime data = Convert.ToDateTime(dal);
            Decimal importoParametro;
            if (flag == "S")
            {
                if (DenunciaMensileBLL.listaParametriGenerali != null)
                {
                    List<ParametriGenerali> list = DenunciaMensileBLL.listaParametriGenerali
                        .Where<ParametriGenerali>((Func<ParametriGenerali, bool>)(p =>
                            p.CodPar == (Decimal)codPar && DateTime.Parse(p.DataInizio) <= data))
                        .ToList<ParametriGenerali>();
                    importoParametro = list[0].Valore != null ? Convert.ToDecimal(list[0].Valore.Trim()) : 0M;
                }
                else
                    importoParametro = 0M;
            }
            else
                importoParametro = !string.IsNullOrEmpty(DenunciaMensileBLL.listaParametriGenerali[0].Valore)
                    ? Convert.ToDecimal(DenunciaMensileBLL.listaParametriGenerali[0].Valore.Trim())
                    : 0M;

            return importoParametro;
        }

        public static decimal GetImportoParametro(
          List<ParametriGenerali> parameters,
          int codPar,
          string dal,
          string flag = "S")
        {
            DateTime dateDal = Convert.ToDateTime(dal);
            decimal importoParametro;
            if (flag == "S")
            {
                //parameters = DenunciaMensileBLL.listaParametriGenerali.Where<ParametriGenerali>((Func<ParametriGenerali, bool>)(p => p.CodPar == (Decimal)codPar && DateTime.Parse(p.DataInizio) <= dateDal)).ToList<ParametriGenerali>();
                parameters = parameters.Where(p => p.CodPar == (decimal)codPar && DateTime.Parse(p.DataInizio) <= dateDal).ToList();
                importoParametro = parameters[0].Valore != null 
                    ? Convert.ToDecimal(parameters[0].Valore.Trim()) 
                    : 0M;
            }
            else
                //importoParametro = !string.IsNullOrEmpty(DenunciaMensileBLL.listaParametriGenerali[0].Valore) ? Convert.ToDecimal(DenunciaMensileBLL.listaParametriGenerali[0].Valore.Trim()) : 0M;
                importoParametro = !string.IsNullOrEmpty(parameters[0].Valore) 
                    ? Convert.ToDecimal(parameters[0].Valore.Trim()) 
                    : 0M;

            return importoParametro;
        }

        private static bool SospensioniImportoZero(List<RetribuzioneRDL> listaReport)
        {
            bool flag = true;
            if (listaReport != null)
            {
                foreach (RetribuzioneRDL retribuzioneRdl in listaReport)
                {
                    if (retribuzioneRdl.NumGGSos > 0M)
                    {
                        if ((Decimal)(retribuzioneRdl.NumGGPer - retribuzioneRdl.NumGGDom) == retribuzioneRdl.NumGGSos)
                        {
                            string str1 = retribuzioneRdl.CodSos.ToString();
                            char[] chArray = new char[1] { '-' };
                            foreach (string str2 in str1.Split(chArray))
                            {
                                if (str2 != "5" && str2 != "11")
                                {
                                    flag = false;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        flag = false;
                        break;
                    }
                }
            }

            return flag;
        }

        private static bool VerificaDipa(
            ref List<RetribuzioneRDL> listaReport,
            string codPos,
            string anno,
            int mese,
            int proDen)
        {
            int? outcome1 = new int?();
            DataLayer db = new DataLayer();
            string str = mese.ToString().PadLeft(2, '0');
            string empty = string.Empty;
            bool flag = false;
            try
            {
                if (listaReport.Count == 0)
                {
                    db.StartTransaction();
                    bool blnCommit = DenunciaMensileDAL.DeleteRowsFromDENDET(db, codPos, anno, mese, proDen, ref empty);
                    if (blnCommit)
                        blnCommit = DenunciaMensileDAL.DeleteRowsFromDENTES(db, codPos, anno, mese, proDen, ref empty);
                    db.EndTransaction(blnCommit);
                    DenunciaMensileBLL.WarningMessage +=
                        "Impossibile compilare la denuncia! Non risultano dipendenti attivi.";
                }

                List<Raplav_Dendet_Data> raplavDendetData;
                if ((raplavDendetData =
                        DenunciaMensileDAL.GetRaplavDendetData(db, codPos, anno, mese, proDen, ref empty)) != null)
                {
                    bool blnCommit = true;
                    db.StartTransaction();
                    foreach (Raplav_Dendet_Data RD_Data in raplavDendetData)
                    {
                        blnCommit = DenunciaMensileDAL.UpdateDendetData_1(db, codPos, anno, mese, proDen, RD_Data,
                            ref empty);
                        blnCommit = DenunciaMensileDAL.UpdateDendetData_2(db, codPos, anno, mese, proDen, RD_Data,
                            ref empty);
                    }

                    db.EndTransaction(blnCommit);
                }

                List<RetribuzioneRDL> denunceSalvate;
                if ((denunceSalvate =
                        DenunciaMensileDAL.GetDenunceSalvate(codPos, anno, mese, proDen, ref empty, out outcome1)) !=
                    null)
                {
                    int? outcome2 = new int?();
                    List<Raplav_Dendet_Data> raplavDendetData2;
                    if ((raplavDendetData2 =
                            DenunciaMensileDAL.GetRaplavDendetData_2(codPos, anno, mese, ref empty, out outcome2)) !=
                        null)
                    {
                        string dataFine = string.Format("{0}/{1}/{2}",
                            (object)DateTime.DaysInMonth(Convert.ToInt32(anno), mese), (object)str, (object)anno);
                        bool blnCommit = true;
                        db.StartTransaction();
                        foreach (Raplav_Dendet_Data RD_Data in raplavDendetData2)
                        {
                            blnCommit = DenunciaMensileDAL.UpdateDendetData_3(db, codPos, anno, mese, proDen, RD_Data,
                                ref empty);
                            blnCommit = DenunciaMensileDAL.UpdateDendetData_4(db, codPos, anno, mese, proDen, dataFine,
                                RD_Data, ref empty);
                            if (denunceSalvate != null)
                            {
                                foreach (RetribuzioneRDL retribuzioneRdl1 in denunceSalvate)
                                {
                                    if (retribuzioneRdl1.Mat == RD_Data.Mat && retribuzioneRdl1.Dal == RD_Data.Dal)
                                    {
                                        foreach (RetribuzioneRDL retribuzioneRdl2 in listaReport)
                                        {
                                            if (retribuzioneRdl2.Mat == retribuzioneRdl1.Mat &&
                                                retribuzioneRdl2.Dal == retribuzioneRdl1.Dal)
                                            {
                                                retribuzioneRdl1.Al = retribuzioneRdl2.Al;
                                                retribuzioneRdl1.NumGGFig = retribuzioneRdl2.NumGGFig;
                                                retribuzioneRdl1.NumGGSos = retribuzioneRdl2.NumGGSos;
                                                retribuzioneRdl1.NumGGPer = retribuzioneRdl2.NumGGPer;
                                                retribuzioneRdl1.NumGGDom = retribuzioneRdl2.NumGGDom;
                                                retribuzioneRdl1.NumGGConAzi = retribuzioneRdl2.NumGGConAzi;
                                                flag = true;
                                                break;
                                            }
                                        }
                                    }

                                    if (flag)
                                        break;
                                }

                                flag = false;
                            }
                        }

                        db.EndTransaction(blnCommit);
                        return blnCommit;
                    }

                    int? nullable = outcome2;
                    int num = 1;
                    if (!(nullable.GetValueOrDefault() == num & nullable.HasValue))
                        return DenunciaMensileBLL.VerificaDenuncia(denunceSalvate, ref listaReport, codPos,
                            Convert.ToInt32(anno), mese, proDen);
                    DenunciaMensileBLL.log.Info((object)string.Format(
                        "[TFI.BLL] : Verifica Dipa - La funzione {0}() ha generato un'eccezione in data: {1}",
                        (object)empty, (object)DateTime.Now));
                    DenunciaMensileBLL.ErrorMessage = "La funzione " + empty + "() ha generato un'eccezione";
                    return false;
                }

                DenunciaMensileBLL.log.Info((object)string.Format(
                    "[TFI.BLL] : Verifica Dipa - La funzione {0}() ha generato un'eccezione in data: {1}",
                    (object)empty, (object)DateTime.Now));
                DenunciaMensileBLL.ErrorMessage = "La funzione " + empty + "() ha generato un'eccezione";
                return false;
            }
            catch (Exception ex)
            {
                DenunciaMensileBLL.log.Info((object)string.Format(
                    "[TFI.BLL] : Verifica Dipa - La funzione {0}() ha generato un'eccezione in data: {1}",
                    (object)empty, (object)DateTime.Now));
                DenunciaMensileBLL.ErrorMessage = "La funzione " + empty + "() ha generato un'eccezione: " + ex.Message;
                db.EndTransaction(false);
                return false;
            }
        }

        private static bool VerificaDenuncia(
            List<RetribuzioneRDL> listaDenunceSalvate,
            ref List<RetribuzioneRDL> listaReport,
            string codPos,
            int anno,
            int mese,
            int proDen)
        {
            DataLayer db = new DataLayer();
            bool blnCommit = true;
            int modCount = 0;
            try
            {
                db.StartTransaction();
                if (listaDenunceSalvate.Count > 0)
                {
                    foreach (RetribuzioneRDL report in listaReport)
                    {
                        report.Mod = "S";

                        foreach (RetribuzioneRDL denuncia in listaDenunceSalvate)
                        {
                            if (denuncia.Mat == report.Mat && 
                                denuncia.ProRap == report.ProRap && 
                                denuncia.Dal == report.Dal && 
                                denuncia.Al == report.Al && 
                                denuncia.CodCon == report.CodCon && 
                                denuncia.ProCon == report.ProCon && 
                                denuncia.CodLoc == report.CodLoc && 
                                denuncia.ProLoc == report.ProLoc && 
                                denuncia.CodLiv == report.CodLiv && 
                                denuncia.CodGruAss == report.CodGruAss && 
                                denuncia.CodQuaCon == report.CodQuaCon && 
                                denuncia.Aliquota == report.Aliquota && 
                                denuncia.Eta65 == report.Eta65 &&
                                denuncia.Fap == report.Fap && 
                                denuncia.PerFap == report.PerFap && 
                                denuncia.PerPar == report.PerPar && 
                                denuncia.PerApp == report.PerApp)
                            {
                                report.ImpRet = denuncia.ImpRet;
                                report.ImpOcc = denuncia.ImpOcc;
                                report.ImpCon = denuncia.ImpCon;

                                if (denuncia.NumGGSos != report.NumGGSos || 
                                    denuncia.NumGGFig != report.NumGGFig || 
                                    denuncia.NumGGAzi != report.NumGGAzi || 
                                    denuncia.NumGGConAzi != report.NumGGConAzi)
                                {
                                    blnCommit = DenunciaMensileDAL.AggiornaVariazSospensioni(db, denuncia, report, codPos, anno, mese, proDen);
                                    
                                    report.NumGGSos = denuncia.NumGGSos;
                                    report.NumGGFig = denuncia.NumGGFig;
                                    report.NumGGAzi = denuncia.NumGGAzi;
                                    report.NumGGPer = denuncia.NumGGPer;
                                    report.NumGGDom = denuncia.NumGGDom;
                                    report.NumGGConAzi = denuncia.NumGGConAzi;
                                }
                                else if (report.ImpFig != denuncia.ImpFig)
                                    report.ImpFig = denuncia.ImpFig;

                                if (!string.IsNullOrEmpty(denuncia.DatEro))
                                    report.DatEro = Convert.ToDateTime(denuncia.DatEro).ToString("d");

                                report.Mod = string.Empty;
                            }
                        }
                    }

                    db.EndTransaction(blnCommit);
                    
                    foreach (RetribuzioneRDL retribuzioneRdl1 in listaReport)
                    {
                        RetribuzioneRDL report = retribuzioneRdl1;
                        if (report.Mod == "S")
                        {
                            var retribuzioneRdlList = listaDenunceSalvate.Where(r => r.Mat == report.Mat).ToList();
                            if (retribuzioneRdlList.Count > 0)
                            {
                                foreach (RetribuzioneRDL retribuzioneRdl2 in listaReport)
                                {
                                    if (retribuzioneRdl2.Mat == report.Mat && retribuzioneRdl2.ProRap == report.ProRap)
                                    {
                                        retribuzioneRdl2.ImpRet = 0M;
                                        retribuzioneRdl2.ImpOcc = 0M;
                                        retribuzioneRdl2.ImpCon = 0M;
                                        retribuzioneRdl2.Mod = "S";
                                        ++modCount;
                                    }
                                }
                            }
                            else
                                report.Mod = string.Empty;
                        }
                    }
                }
                return modCount == 0;
            }
            catch (Exception ex)
            {
                log.Info($"[TFI.BLL] : VerificaDipa() - La funzione VerificaDenuncia() ha generato un'eccezione in data: {DateTime.Now}");
                ErrorMessage = "La funzione VerificaDenuncia() ha generato un'eccezione";
                return false;
            }
        }

        private static SediAziendali GetDatiSedeLegale(string codPos)
        {
            int? outcome = new int?();
            List<SediAziendali> sediAziendali;
            if ((sediAziendali = DenunciaMensileDAL.GetSediAziendali(codPos, out outcome)) != null)
            {
                int? nullable = new int?();
                return sediAziendali[0];
            }

            int? nullable1 = outcome;
            int num = 1;
            if (nullable1.GetValueOrDefault() == num & nullable1.HasValue)
            {
                DenunciaMensileBLL.log.Info((object)string.Format(
                    "[TFI.BLL] : GetDatiSedeLegale() - La funzione GetSediAziendali() ha generato un'eccezione in data: {0}",
                    (object)DateTime.Now));
                DenunciaMensileBLL.ErrorMessage = "La funzione GetSediAziendali() ha generato un'eccezione";
                return (SediAziendali)null;
            }

            DenunciaMensileBLL.ErrorMessage =
                "La funzione per ottenere i dati sulle sedi dell'azienda non ha restituito risultati";
            return (SediAziendali)null;
        }

        private static string GetIndirizzoSedeLegale(string codPos)
        {
            int? outcome = new int?();
            string str1 = string.Empty;
            List<SediAziendali> sediAziendali1;
            if ((sediAziendali1 = DenunciaMensileDAL.GetSediAziendali(codPos, out outcome)) != null)
            {
                int? nullable = new int?();
                SediAziendali sediAziendali2 = sediAziendali1[0];
                if (sediAziendali2.Tipo == 1)
                {
                    if (sediAziendali2.DenDug.Trim() != string.Empty)
                        str1 = str1 + sediAziendali2.DenDug.Trim() + " ";
                    string str2 = str1 + sediAziendali2.Indirizzo.Trim();
                    if (sediAziendali2.NumeroCivico.Trim() != string.Empty)
                        str2 = str2 + ", " + sediAziendali2.NumeroCivico.Trim();
                    string indirizzoSedeLegale = str2 + " " + sediAziendali2.Localita.Trim() + " (" +
                                                 sediAziendali2.Provincia.Trim() + ") Tel. " +
                                                 sediAziendali2.Telefono.Trim();
                    nullable = new int?(0);
                    return indirizzoSedeLegale;
                }

                DenunciaMensileBLL.ErrorMessage =
                    "La funzione per ottenere l'indirizzo della sede legale dell'azienda ha generato un errore";
                return (string)null;
            }

            int? nullable1 = outcome;
            int num = 1;
            if (nullable1.GetValueOrDefault() == num & nullable1.HasValue)
            {
                DenunciaMensileBLL.log.Info((object)string.Format(
                    "[TFI.BLL] : GetIndirizzoSedeLegale() - La funzione GetSediAziendali() ha generato un'eccezione in data: {0}",
                    (object)DateTime.Now));
                DenunciaMensileBLL.ErrorMessage = "La funzione GetSediAziendali() ha generato un'eccezione";
                return (string)null;
            }

            DenunciaMensileBLL.ErrorMessage =
                "La funzione per ottenere l'indirizzo della sede legale dell'azienda non ha restituito risultati";
            return (string)null;
        }

        private static DatiAnagraficiAzienda GetDatiAnagraficiAzienda(string codPos)
        {
            int? outcome = new int?();
            DatiAnagraficiAzienda anagraficiAzienda;
            if ((anagraficiAzienda = DenunciaMensileDAL.GetDatiAnagraficiAzienda(codPos, out outcome)) != null)
            {
                int? nullable = new int?();
                return anagraficiAzienda;
            }

            int? nullable1 = outcome;
            int num = 1;
            if (nullable1.GetValueOrDefault() == num & nullable1.HasValue)
            {
                DenunciaMensileBLL.log.Info((object)string.Format(
                    "[TFI.BLL] : GetDatiAnagraficiAzienda() - La funzione GetDatiAnagraficiAzienda() ha generato un'eccezione in data: {0}",
                    (object)DateTime.Now));
                DenunciaMensileBLL.ErrorMessage = "La funzione GetDatiAnagraficiAzienda() ha generato un'eccezione";
                return (DatiAnagraficiAzienda)null;
            }

            DenunciaMensileBLL.ErrorMessage =
                "La funzione per ottenere i dati anagrafici dell'azienda non ha restituito risultati";
            return (DatiAnagraficiAzienda)null;
        }

        private static void FillBasicFields(
            RetribuzioneRDL report,
            int tipRap,
            int codCon,
            Decimal proCon,
            int codLoc,
            Decimal proLoc,
            int codLiv,
            string livello,
            string qualifica,
            int codQuaCon,
            Decimal perPar,
            Decimal perApp,
            Decimal impMin,
            Decimal impTraEco,
            Decimal impSca,
            string fap,
            string assCon,
            string abbPre,
            string tipSpe,
            int codGruAss,
            int mat,
            int proRap,
            string nome,
            string datNas,
            string datDec,
            string datCes)
        {
            report.TipRap = tipRap;
            report.CodCon = codCon;
            report.ProCon = proCon;
            report.CodLoc = codLoc;
            report.ProLoc = proLoc;
            report.CodLiv = codLiv;
            report.Livello = livello;
            report.Qualifica = qualifica;
            report.CodQuaCon = codQuaCon;
            report.PerPar = perPar;
            report.PerApp = perApp;
            report.ImpMin = impMin;
            report.ImpTraEco = impTraEco;
            report.ImpSca = impSca;
            report.Fap = fap;
            report.AssCon = assCon;
            report.AbbPre = abbPre;
            report.TipSpe = tipSpe;
            report.CodGruAss = codGruAss;
            report.Mat = mat;
            report.ProRap = proRap;
            report.Nome = nome;
            report.DatNas = datNas;
            report.DatDec = datDec;
            report.DatCes = datCes;
        }

        private static void MergingDuplicateRows(List<RetribuzioneRDL> listaReport)
        {
            int num1 = 0;
            int num2 = 0;
            Decimal num3 = 0M;
            string str = string.Empty;
            for (int index = 0; index <= listaReport.Count - 1; ++index)
            {
                if (listaReport[index].Mat == num1 && listaReport[index].Aliquota == num3 &&
                    listaReport[index].DatCes == str)
                {
                    listaReport[index - num2].Al = listaReport[index].Al;
                    listaReport[index - num2].ImpSca = listaReport[index].ImpSca;
                    listaReport[index - num2].ImpMin = listaReport[index].ImpMin;
                    listaReport[index - num2].DatDec = listaReport[index].DatDec;
                    listaReport[index - num2].ImpTraEco = listaReport[index].ImpTraEco;
                    listaReport[index - num2].AbbPre = listaReport[index].AbbPre;
                    listaReport[index - num2].ImpAbb = listaReport[index].ImpAbb;
                    listaReport[index - num2].AssCon = listaReport[index].AssCon;
                    listaReport[index - num2].ImpAssCon = listaReport[index].ImpAssCon;
                    listaReport[index - num2].CodFis = listaReport[index].CodFis;
                    listaReport[index].Rimuovi = true;
                    ++num2;
                }
                else
                    num2 = 1;

                listaReport[index].ImpRet = 0.00M;
                listaReport[index].ImpOcc = 0.00M;
                listaReport[index].ImpCon = 0.00M;
                listaReport[index].ImpFig = 0.00M;
                num1 = listaReport[index].Mat;
                num3 = listaReport[index].Aliquota;
                str = listaReport[index].DatCes;
            }
        }

        private static int GetNumGGPeriodo(string dataDal, string dataAl) =>
            Convert.ToDateTime(dataAl).Subtract(Convert.ToDateTime(dataDal)).Days + 1;

        private static int GetNumGGDomeniche(string dataDal, string dataAl)
        {
            int numGgDomeniche = 0;
            DateTime dateTime1 = Convert.ToDateTime(dataDal);
            for (DateTime dateTime2 = Convert.ToDateTime(dataAl);
                 dateTime1 <= dateTime2;
                 dateTime1 = dateTime1.AddDays(1.0))
            {
                if (dateTime1.DayOfWeek == DayOfWeek.Sunday)
                    ++numGgDomeniche;
            }

            return numGgDomeniche;
        }

        private static bool GetNumGGSospensioni(
            string codPos,
            RetribuzioneRDL report,
            out int? outcome)
        {
            string dal = report.Dal;
            string al = report.Al;
            int mat = report.Mat;
            int proRap = report.ProRap;
            int num1 = 0;
            Decimal num2 = 0.0M;
            Decimal num3 = 0.0M;
            Decimal codSos = (Decimal)report.CodSos;
            DateTime dateTime1 = Convert.ToDateTime(dal);
            DateTime dateTime2 = Convert.ToDateTime(al);
            List<SospensioneRapporto> sospensioneRapportoList = new List<SospensioneRapporto>();
            List<SospensioneRapporto> suspensions;
            if ((suspensions = DenunciaMensileDAL.GetSuspensions(codPos, dal, al, mat, proRap, out outcome)) != null)
            {
                outcome = new int?();
                foreach (SospensioneRapporto sospensioneRapporto in suspensions)
                {
                    if (Convert.ToDateTime(sospensioneRapporto.DataFine) >= dateTime1)
                    {
                        if (codSos == 0M)
                            codSos += sospensioneRapporto.CodSos;
                        else
                            string.Format("{0}-{1}", (object)codSos, (object)sospensioneRapporto.CodSos);
                        TimeSpan timeSpan;
                        DateTime dateTime3;
                        if (Convert.ToDateTime(sospensioneRapporto.DataInizio) <= dateTime2)
                        {
                            if (dateTime1 >= Convert.ToDateTime(sospensioneRapporto.DataInizio))
                            {
                                if (dateTime2 <= Convert.ToDateTime(sospensioneRapporto.DataFine))
                                {
                                    int numGgDomeniche = DenunciaMensileBLL.GetNumGGDomeniche(dal, al);
                                    timeSpan = dateTime2.Subtract(dateTime1);
                                    num1 = timeSpan.Days + 1 - numGgDomeniche;
                                    if (sospensioneRapporto.PerAzi > 0M)
                                        num2 = (Decimal)num1 * sospensioneRapporto.PerAzi / 100M;
                                    if (sospensioneRapporto.PerFig > 0M)
                                    {
                                        num3 = (Decimal)num1 * sospensioneRapporto.PerFig / 100M;
                                        break;
                                    }

                                    break;
                                }

                                int numGgDomeniche1 =
                                    DenunciaMensileBLL.GetNumGGDomeniche(dal, sospensioneRapporto.DataFine);
                                dateTime3 = Convert.ToDateTime(sospensioneRapporto.DataFine);
                                timeSpan = dateTime3.Subtract(dateTime1);
                                num1 = timeSpan.Days + 1 - numGgDomeniche1;
                                if (sospensioneRapporto.PerAzi > 0M)
                                    num2 = (Decimal)num1 * sospensioneRapporto.PerAzi / 100M;
                                if (sospensioneRapporto.PerFig > 0M)
                                    num3 = (Decimal)num1 * sospensioneRapporto.PerFig / 100M;
                            }
                            else
                            {
                                if (dateTime2 <= Convert.ToDateTime(sospensioneRapporto.DataFine))
                                {
                                    int numGgDomeniche =
                                        DenunciaMensileBLL.GetNumGGDomeniche(sospensioneRapporto.DataInizio, al);
                                    int num4 = num1;
                                    timeSpan = dateTime2.Subtract(Convert.ToDateTime(sospensioneRapporto.DataInizio));
                                    int num5 = timeSpan.Days + 1 - numGgDomeniche;
                                    num1 = num4 + num5;
                                    if (sospensioneRapporto.PerAzi > 0M)
                                    {
                                        Decimal num6 = num2;
                                        timeSpan = dateTime2.Subtract(
                                            Convert.ToDateTime(sospensioneRapporto.DataInizio));
                                        Decimal num7 = (Decimal)(timeSpan.Days + 1) -
                                                       (Decimal)numGgDomeniche * sospensioneRapporto.PerAzi / 100M;
                                        num2 = num6 + num7;
                                    }

                                    if (sospensioneRapporto.PerFig > 0M)
                                    {
                                        Decimal num8 = num3;
                                        timeSpan = dateTime2.Subtract(
                                            Convert.ToDateTime(sospensioneRapporto.DataInizio));
                                        Decimal num9 = (Decimal)(timeSpan.Days + 1) -
                                                       (Decimal)numGgDomeniche * sospensioneRapporto.PerFig / 100M;
                                        num3 = num8 + num9;
                                        break;
                                    }

                                    break;
                                }

                                int numGgDomeniche2 =
                                    DenunciaMensileBLL.GetNumGGDomeniche(sospensioneRapporto.DataInizio,
                                        sospensioneRapporto.DataFine);
                                int num10 = num1;
                                dateTime3 = Convert.ToDateTime(sospensioneRapporto.DataFine);
                                timeSpan = dateTime3.Subtract(Convert.ToDateTime(sospensioneRapporto.DataInizio));
                                int num11 = timeSpan.Days + 1 - numGgDomeniche2;
                                num1 = num10 + num11;
                                if (sospensioneRapporto.PerAzi > 0M)
                                {
                                    Decimal num12 = num2;
                                    dateTime3 = Convert.ToDateTime(sospensioneRapporto.DataFine);
                                    timeSpan = dateTime3.Subtract(Convert.ToDateTime(sospensioneRapporto.DataInizio));
                                    Decimal num13 = (Decimal)(timeSpan.Days + 1) -
                                                    (Decimal)numGgDomeniche2 * sospensioneRapporto.PerAzi / 100M;
                                    num2 = num12 + num13;
                                }

                                if (sospensioneRapporto.PerFig > 0M)
                                {
                                    Decimal num14 = num3;
                                    dateTime3 = Convert.ToDateTime(sospensioneRapporto.DataFine);
                                    timeSpan = dateTime3.Subtract(Convert.ToDateTime(sospensioneRapporto.DataInizio));
                                    Decimal num15 = (Decimal)(timeSpan.Days + 1) -
                                                    (Decimal)numGgDomeniche2 * sospensioneRapporto.PerFig / 100M;
                                    num3 = num14 + num15;
                                }
                            }
                        }
                    }
                }

                outcome = new int?(0);
            }
            else
            {
                int? nullable = outcome;
                int num16 = 1;
                if (nullable.GetValueOrDefault() == num16 & nullable.HasValue)
                    return false;
            }

            report.NumGGSos = (Decimal)num1;
            report.NumGGFig = num3;
            report.NumGGAzi = num2;
            return true;
        }

        private static bool IsOver65InYearMonth(string dataNasPiu65, string str2, string s2)
        {
            return DateTime.Compare(DateTime.Parse(dataNasPiu65), DateTime.Parse(str2)) > 0 &
                   DateTime.Compare(DateTime.Parse(dataNasPiu65), DateTime.Parse(s2)) < 0;
        }

        private static RetribuzioneRDL CloneReport(RetribuzioneRDL report)
        {
            string under65Record = JsonConvert.SerializeObject(report);
            return JsonConvert.DeserializeObject<RetribuzioneRDL>(under65Record);
        }

        public static void ParseAndFillDenunciaFromFile(List<string> fileContent, DatiNuovaDenuncia datiDenuncia, int anno, int mese)
        {
            foreach (var line in fileContent)
            {
                ReplaceInFileFromUpload(line, datiDenuncia, anno, mese);
            }
        }

        private static void ReplaceInFileFromUpload(string line, DatiNuovaDenuncia datiDenuncia, int anno, int mese)
        {
            var codfis = line.Substring(0, 16);
            var progrDip = line.Substring(16, 2);
            var tipoRetr = line.Substring(18, 1);
            var mgDal = line.Substring(19, 4);
            var mgAl = line.Substring(23, 4);
            var aliquota = line.Substring(27, 5);
            var retribu = line.Substring(32, 9);
            var occasio = line.Substring(41, 9);
            var figurat = line.Substring(50, 9);

            var datiDenunciaReport = datiDenuncia.ListaReport.Single(e => 
                string.Equals(e.CodFis, codfis, StringComparison.InvariantCultureIgnoreCase)
                && e.ProDenDet == int.Parse(progrDip));

            datiDenunciaReport.Aliquota = decimal.Parse(aliquota.Insert(aliquota.Length - 3, ","));
            datiDenunciaReport.ImpRet = decimal.Parse(retribu.Insert(retribu.Length - 2, ","));
            datiDenunciaReport.ImpOcc = decimal.Parse(occasio.Insert(occasio.Length - 2, ","));
            datiDenunciaReport.ImpFig = decimal.Parse(figurat.Insert(figurat.Length - 2, ","));
            datiDenunciaReport.Dal = new DateTime(
                anno, 
                mese, 
                int.Parse(mgDal.Substring(2, 2))).ToString("dd/MM/yyyy");
            datiDenunciaReport.Al = new DateTime(
                anno,
                mese,
                int.Parse(mgAl.Substring(2, 2))).ToString("dd/MM/yyyy");
        }

        public static bool CheckDenunciaEsistente(int anno, int mese, string codPos) => DenunciaMensileDAL.CheckDenunciaEsistente(anno, mese, codPos);

        private static (bool Success, string Message) InviaEmailConfermaDipa(Utente utente, Dentes_Data denTes)
        {
            var emailAzienda = DenunciaMensileDAL.GetEmailAzienda(utente.CodPosizione);
            var emailSubject = "Conferma dati denuncia ad Enpaia";
            var emailBody = $"I dati della denuncia relativa al periodo {denTes.MesDen}/{denTes.AnnDen} per la posizione {denTes.CodPos} sono stati confermati ad Enpaia.";
            return SmtpEmailService.InviaEmailConfermaDipa(emailSubject, emailBody, emailAzienda, utente.CodPosizione);
        }

        public static (ObjResponseGet EsitoPagoPa, ObjResDataCrea EsitoIuvTransId) GetDettagliPagoPa(int anno, int mese, int proDen, string utenteCodPosizione)
        {
            var pagoPa = new PagoPa();
            var dettagliPagoPaTable = pagoPa.GetDettagliPagoPa(anno, mese, proDen, utenteCodPosizione);
            var dettaglioObject = new ObjResDataCrea { TransActionId = dettagliPagoPaTable.transaActionId, IuvCode = dettagliPagoPaTable.iuvCod};
            return (pagoPa.GetPagamento(dettagliPagoPaTable.iuvCod, dettagliPagoPaTable.transaActionId), dettaglioObject);
        }

        public static ResultDtoWithContent<byte[]> GetRicevutaDipa(int proDen, int mese, int anno, string codPos)
        {
            var protocolloDenunciaMensile = DenunciaMensileDAL.GetProtocolloDenunciaMensile(anno, mese, proDen, codPos);
            if (protocolloDenunciaMensile.Success)
            {
                var resultFile = _protocolloDll.GetFile(protocolloDenunciaMensile.IdProtocollo);
                return new ResultDtoWithContent<byte[]>(resultFile.Length > 0, resultFile);
            }
            
            var protocollo = _protocolloDll.ProtocollaPratica(ref ErrorMessage);
            if (!protocollo.Success)
            {
                log.Info($"[TFI.BLL] : GetRicevutaDIPA - errore protocollo dll in data: {DateTime.Now} - messaggio: {protocollo.Message}");
                return new ResultDtoWithContent<byte[]>(false, protocollo.Message);
            }

            var dettaglioDipa = new DatiPdfDatiTotaliDenuncia()
            {
                Protocollo = protocollo,
                DatiTotaliDenuncia = new DatiTotaliDenuncia()
                {
                    ProDen = proDen,
                    Anno = anno,
                    Mese = mese,
                    CodPos = codPos
                }
            };
            
            var ricevutaDipa = PdfService.CreaPdfRicevutaDipa(dettaglioDipa, ref ErrorMessage);
            string fileName = $"RicevutaDipa_{mese}/{anno}_{codPos}.pdf";
            
            var result = _protocolloDll.AllegaFilePratica(new AllegaProtocolloModel
            {
                UserIdentifier = codPos,
                IdProtocollo = protocollo.IdProtocollo,
                FileName = fileName,
                FileByteArray = ricevutaDipa,
                TipoDocumento = TipoDocumento.RicevutaDipa 
            });
            if (!result.IsSuccessfull)
            {
                log.Info($"[TFI.BLL] : GetRicevutaDIPA - errore allega file dll in data: {DateTime.Now} - messaggio: {result.Message}");
                return new ResultDtoWithContent<byte[]>(false, result.Message);
            }
            
            var resultAggiornaDenunciaTestataConProtocollo = DenunciaMensileDAL.AggiornaDenunciaTestataConProtocollo(protocollo, anno, mese, proDen, codPos);
            if (!resultAggiornaDenunciaTestataConProtocollo)
            {
                log.Info($"[TFI.BLL] : GetRicevutaDIPA - errore aggiornamento testata con protocollo in data: {DateTime.Now}");
                return new ResultDtoWithContent<byte[]>(false, "errore aggiornamento testata con protocollo");
            }
                            
            return new ResultDtoWithContent<byte[]>(true, _protocolloDll.GetFile(protocollo.IdProtocollo));
        }

        public static bool CheckDIPAMesePrecedente(int anno, int mese, int codPos)
        {
            var dataDenunciaMesePrecedentePrimoGiorno = new DateTime(anno, mese, 1).AddMonths(-1);
            var isDIPAMesePrecedenteConfermata = DenunciaMensileDAL.IsDenunciaMesePrecedenteConfermata(dataDenunciaMesePrecedentePrimoGiorno.Year, dataDenunciaMesePrecedentePrimoGiorno.Month, codPos);

            if(isDIPAMesePrecedenteConfermata)
                return isDIPAMesePrecedenteConfermata;

            var dataDenunciaMesePrecedenteUltimoGiorno = new DateTime(anno, mese, DateTime.DaysInMonth(anno, mese)).AddMonths(-1);
            var numeroRapportiLavoroNelMesePrecedente = DenunciaMensileDAL.GetTOTRapLav(
                codPos, dataDenunciaMesePrecedentePrimoGiorno.StandardizeDateString(StandardUse.Internal), dataDenunciaMesePrecedenteUltimoGiorno.StandardizeDateString(StandardUse.Internal), out _);
            var isDIPAMesePrecedenteNonNecessaria = numeroRapportiLavoroNelMesePrecedente == 0;

            return isDIPAMesePrecedenteNonNecessaria;
        }
        public static bool IsDenunciaPresentabile(int annoDenuncia, int meseDenuncia, int codPosAzienda)
        {
            var dataDenunciaMesePrecedentePrimoGiorno = new DateTime(annoDenuncia, meseDenuncia, 1);
            var dataDenunciaMesePrecedenteUltimoGiorno = new DateTime(annoDenuncia, meseDenuncia, DateTime.DaysInMonth(annoDenuncia, meseDenuncia));

            var numeroRapportiLavoroNelMese = DenunciaMensileDAL.GetTOTRapLav(
                codPosAzienda, dataDenunciaMesePrecedentePrimoGiorno.StandardizeDateString(StandardUse.Internal), dataDenunciaMesePrecedenteUltimoGiorno.StandardizeDateString(StandardUse.Internal), out _);
            var isDIPAPresentabile = numeroRapportiLavoroNelMese != 0;

            return isDIPAPresentabile;
        }
        
        public static bool SalvaParzialeDIPAConsulente(int annoDenuncia, int meseDenuncia, Utente utente, List<RetribuzioneRDL> datiDenunciaListaReport, DatiNuovaDenuncia datiDenuncia, ref string currentTimeStampSession, out int o, out int o1)
        {
            datiDenuncia.ProDen = GetProDen(annoDenuncia, meseDenuncia, utente.CodPosizione);
            return SalvaParzialeDIPA(utente, datiDenunciaListaReport, datiDenuncia, ref currentTimeStampSession, out o, out o1);
        }
        
        private static int GetProDen(int annoDenuncia, int meseDenuncia, string codPosAzienda)
            => DenunciaMensileDAL.GetProDen(annoDenuncia, meseDenuncia, codPosAzienda);
        
    }
}
