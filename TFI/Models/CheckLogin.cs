// Decompiled with JetBrains decompiler
// Type: TFI.Models.CheckLogin
// Assembly: TFI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8C322276-BA13-4F76-8B41-0B6E94A1BA76
// Assembly location: C:\Users\unikey\Desktop\tfi_collaudo.enpaia.it\tfi_collaudo.enpaia.it\bin\TFI.dll

using System.Web;
using System.Web.Mvc;

namespace TFI.Models
{
  public class CheckLogin : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      if (HttpContext.Current.Session["utente"] != null)
        return;
      filterContext.Result = (ActionResult) new RedirectResult("~/Login");
    }
  }
}
