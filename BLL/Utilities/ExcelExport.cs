// Decompiled with JetBrains decompiler
// Type: TFI.BLL.Utilities.ExcelExport
// Assembly: BLL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 355CD4EE-66F8-4E70-A596-5A3A4EB0EBAB
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\BLL.dll

using ClosedXML.Excel;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;

namespace TFI.BLL.Utilities
{
  public class ExcelExport
  {
    public static ExcelExport excelExporter;

    public XLWorkbook ExportExcelDataTable<T>(List<T> items, string filePath)
    {
      using (XLWorkbook xlWorkbook = new XLWorkbook())
      {
        DataTable dataTable1 = new DataTable();
        ExcelExport.excelExporter = new ExcelExport();
        DataTable dataTable2 = ExcelExport.excelExporter.ToDataTable<T>(items);
        xlWorkbook.Worksheets.Add(dataTable2);
        using (new MemoryStream())
        {
          xlWorkbook.SaveAs(filePath);
          return xlWorkbook;
        }
      }
    }

    public DataTable ToDataTable<T>(List<T> items)
    {
      DataTable dataTable = new DataTable(typeof (T).Name);
      PropertyInfo[] properties = typeof (T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
      foreach (PropertyInfo propertyInfo in properties)
        dataTable.Columns.Add(propertyInfo.Name.ToUpper());
      foreach (T obj in items)
      {
        object[] objArray = new object[properties.Length];
        for (int index = 0; index < properties.Length; ++index)
          objArray[index] = properties[index].GetValue((object) obj, (object[]) null);
        dataTable.Rows.Add(objArray);
      }
      return dataTable;
    }
  }
}
