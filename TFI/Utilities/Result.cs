// Decompiled with JetBrains decompiler
// Type: TFI.Utilities.Result`1
// Assembly: TFI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8C322276-BA13-4F76-8B41-0B6E94A1BA76
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\TFI.dll

using System;
using System.Net;
using System.Runtime.Serialization;

namespace TFI.Utilities
{
  public class Result<T>
  {
    [DataMember(Name = "DT")]
    public T Data { get; set; }

    internal void SetError(WebException wEx) => throw new NotImplementedException();
  }
}
