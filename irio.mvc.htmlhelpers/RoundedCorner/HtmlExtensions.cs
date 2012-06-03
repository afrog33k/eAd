using System.Web.Mvc;

namespace irio.mvc.htmlhelpers.RoundedCorner
{
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