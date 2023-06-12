// Decompiled with JetBrains decompiler
// Type: ListtoDataTableConverter
// Assembly: DAL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3D8A72B5-139D-44E3-A72F-AE9C9551C15F
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\DAL.dll

using System.Collections.Generic;
using System.Data;
using System.Reflection;

public class ListtoDataTableConverter
{
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
