using System;
using System.Web.Mvc;

namespace irio.mvc.htmlhelpers.RoundedCorner
{
public class RoundedCorner : IDisposable
{
    private readonly ViewContext _viewContext;
    private bool _disposed;

    public RoundedCorner(ViewContext viewContext)
    {
        _viewContext = viewContext;
    }

    #region IDisposable Members

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            _disposed = true;
            _viewContext.Writer.Write(
                @"<div class=""Clear"">
                  </div>
                  </div>
                  </div>
                  <div class=""bottom"">
                  <div class=""right"">
                  </div>
                  </div>
                  </div>
                  </div>"
            );
        }
    }
}

public static partial class HtmlExtensions
{
    public static RoundedCorner RoundedCorner(this HtmlHelper htmlHelper)
    {
        htmlHelper.ViewContext.Writer.Write(
            @"<div class=""rounded"">
            <div class=""top"">
            <div class=""right"">
            </div>
            </div>
            <div class=""middle"">
            <div class=""right"">
            <div class=""content"">"
        );
        return new RoundedCorner(htmlHelper.ViewContext);
    }
}
}