// Type: System.Web.Mvc.UrlHelper
// Assembly: System.Web.Mvc, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// Assembly location: c:\Program Files (x86)\Microsoft ASP.NET\ASP.NET MVC 3\Assemblies\System.Web.Mvc.dll

using System.Web;
using System.Web.Routing;

namespace System.Web.Mvc
{
public class UrlHelper
{
    public UrlHelper(RequestContext requestContext);
    public UrlHelper(RequestContext requestContext, RouteCollection routeCollection);
    public RequestContext RequestContext
    {
        get;
    }
    public RouteCollection RouteCollection
    {
        get;
    }
    public string Action(string actionName);
    public string Action(string actionName, object routeValues);
    public string Action(string actionName, RouteValueDictionary routeValues);
    public string Action(string actionName, string controllerName);
    public string Action(string actionName, string controllerName, object routeValues);
    public string Action(string actionName, string controllerName, RouteValueDictionary routeValues);
    public string Action(string actionName, string controllerName, object routeValues, string protocol);
    public string Action(string actionName, string controllerName, RouteValueDictionary routeValues, string protocol, string hostName);
    public string Content(string contentPath);
    public static string GenerateContentUrl(string contentPath, HttpContextBase httpContext);
    public string Encode(string url);
    public static string GenerateUrl(string routeName, string actionName, string controllerName, string protocol, string hostName, string fragment, RouteValueDictionary routeValues, RouteCollection routeCollection, RequestContext requestContext, bool includeImplicitMvcValues);
    public static string GenerateUrl(string routeName, string actionName, string controllerName, RouteValueDictionary routeValues, RouteCollection routeCollection, RequestContext requestContext, bool includeImplicitMvcValues);
    public bool IsLocalUrl(string url);
    public string RouteUrl(object routeValues);
    public string RouteUrl(RouteValueDictionary routeValues);
    public string RouteUrl(string routeName);
    public string RouteUrl(string routeName, object routeValues);
    public string RouteUrl(string routeName, RouteValueDictionary routeValues);
    public string RouteUrl(string routeName, object routeValues, string protocol);
    public string RouteUrl(string routeName, RouteValueDictionary routeValues, string protocol, string hostName);
}
}
