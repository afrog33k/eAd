// Type: System.Web.IHttpModule
// Assembly: System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_32\System.Web\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Web.dll

namespace System.Web
{
    public interface IHttpModule
    {
        void Init(HttpApplication context);
        void Dispose();
    }
}
