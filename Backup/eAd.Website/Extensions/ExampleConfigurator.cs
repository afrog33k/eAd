using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.UI;

namespace eAd.Website.Extensions
{
    public class ExampleConfigurator : IDisposable
    {
        public const string CssClass = "configurator";

        private readonly HtmlHelper htmlHelper;
        private readonly HtmlTextWriter writer;
        private MvcForm form;
        private string title;

        public ExampleConfigurator(HtmlHelper htmlHelper)
        {
            this.htmlHelper = htmlHelper;
            writer = new HtmlTextWriter(htmlHelper.ViewContext.Writer);
        }

        #region IDisposable Members

        public void Dispose()
        {
            End();
        }

        #endregion

        public ExampleConfigurator Title(string title)
        {
            this.title = title;

            return this;
        }

        public ExampleConfigurator Begin()
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "legend");
            writer.RenderBeginTag(HtmlTextWriterTag.H3);
            writer.Write(title);
            writer.RenderEndTag();

            return this;
        }

        public ExampleConfigurator End()
        {
            writer.RenderEndTag(); // fieldset

            if (form != null)
            {
                form.EndForm();
            }

            return this;
        }

        public ExampleConfigurator PostTo(string action, string controller)
        {
            string theme = htmlHelper.ViewContext.HttpContext.Request.Params["theme"] ?? "sunset";

            form = htmlHelper.BeginForm(action, controller, new { theme });

            return this;
        }
    }
}