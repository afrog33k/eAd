using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.UI;
using irio.utilities;

namespace eAd.Website.Extensions
{
#if MVC1 || MVC2
    using IHtmlString = System.String;
#else

#endif

    public static class HtmlHelpersExtensions
    {
     public    static string appPath = HttpContext.Current.Request.ApplicationPath;
        internal static string UPLOAD_ID_TAG = "::DJ_UPLOAD_ID::";
        /// <summary>
        /// Renders JavaScript to turn the specified file input control into an 
        /// Uploadify upload control.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <param name="legend"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public static MvcHtmlString AjaxUpload(this HtmlHelper helper, string name, string action, string legend, string label)
        {
            string scriptPath = ("~/Content/jqueryPlugins/uploadify/");


            StringBuilder sb = new StringBuilder();
            //Include the JS file.

            sb.AppendLine(helper.ScriptInclude("~/Scripts/jquery.form.js").ToString());
            sb.AppendLine(helper.ScriptInclude("~/Scripts/jquery.blockUI.js").ToString());

            sb.AppendLine(String.Format(
                        "<script type=\"text/javascript\">" + Environment.NewLine +
                    "$(function() {{" + Environment.NewLine +
                     "   $(\"#{0}\").ajaxForm({{" + Environment.NewLine +
                           " iframe: true," + Environment.NewLine +
                           " dataType: \"json\"," + Environment.NewLine +
                            "beforeSubmit: function() {{" + Environment.NewLine +
                                "$(\"#{0}\").block({{ message: '<h1><img src=\"" + appPath + "/Content/busy.gif\" /> Uploading file...</h1>' }});" + Environment.NewLine +
                            "}}," +
                            "success: function(result) {{" + Environment.NewLine +
                                "$(\"#{0}\").unblock();" + Environment.NewLine +
                                "$(\"#{0}\").resetForm();" + Environment.NewLine +
                                "$.growlUI('Upload Status',result.message);" + Environment.NewLine +
                                "$(\"#{0}preview\").attr(\"src\",result.thumbnail);" + Environment.NewLine +
                                 "HandleType(\"{0}\",result.type,result.thumbnail,result.text,result.path,result.duration);" + Environment.NewLine +
                            "}}," +
                           "error: function(xhr, textStatus, errorThrown) {{" + Environment.NewLine +
                                "$(\"#{0}\").unblock();" + Environment.NewLine +
                                "$(\"#{0}\").resetForm();" + Environment.NewLine +
                                "$.growlUI(null, 'Error uploading file');" + Environment.NewLine +
                            "}}" + Environment.NewLine +
                        "}});" + Environment.NewLine +
                    "}});" + Environment.NewLine +
                "</script>", name))
                        ;

            //Dump the script to initialze Uploadify
            TagBuilder uploadFormTagBuilder = new TagBuilder("form");

            uploadFormTagBuilder.GenerateId(name);

            uploadFormTagBuilder.Attributes["action"] = action;

            uploadFormTagBuilder.Attributes["method"] = "post";

            uploadFormTagBuilder.Attributes["enctype"] = "multipart/form-data";


            TagBuilder fieldsetBuilder = new TagBuilder("fieldset");

            TagBuilder legendBuilder = new TagBuilder("legend");
            legendBuilder.InnerHtml = legend;

            TagBuilder labelBuilder = new TagBuilder("label");


            TagBuilder hiddenTagBuilder = new TagBuilder("input");
            hiddenTagBuilder.Attributes["type"] = "hidden";
            hiddenTagBuilder.Attributes["value"] = UPLOAD_ID_TAG + Guid.NewGuid().ToString();
            hiddenTagBuilder.Attributes["name"] = name + "UploadID";


            TagBuilder inputBuilder = new TagBuilder("input");
            inputBuilder.Attributes["type"] = "file";
            inputBuilder.Attributes["name"] = name + "file";

            labelBuilder.InnerHtml = label + inputBuilder.ToString() + "(" + PrettyPrinter.FormatByteCount((ulong)((HttpRuntimeSection)ConfigurationManager.GetSection("system.web/httpRuntime")).MaxRequestLength * 100) + " Max Upload Size)" + "<br/>";

            //<img id="@(ViewBag.Name)preview" alt="Preview" src="../../Content/check48.png"/>

            TagBuilder imagePreview = new TagBuilder("img");
            imagePreview.GenerateId(name + "preview");
            imagePreview.Attributes["src"] = "../../Content/check48.png";

            imagePreview.Attributes["alt"] = "Preview";

            TagBuilder ajaxUploadButtonBuilder = new TagBuilder("input");
            ajaxUploadButtonBuilder.GenerateId(name + "ajaxUploadButton");
            ajaxUploadButtonBuilder.Attributes["type"] = "submit";

            ajaxUploadButtonBuilder.Attributes["value"] = "Submit";

            fieldsetBuilder.InnerHtml = legendBuilder.ToString() + Environment.NewLine +
                                        labelBuilder.ToString() + Environment.NewLine +
                                          imagePreview.ToString() + Environment.NewLine +
                                        hiddenTagBuilder + Environment.NewLine +

                                        ajaxUploadButtonBuilder.ToString();

            uploadFormTagBuilder.InnerHtml = fieldsetBuilder.ToString();

            sb.Append(uploadFormTagBuilder.ToString());
            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString ScriptInclude(this HtmlHelper helper, string scriptPath)
        {//<script type="text/javascript" src="/Scripts/2011.1.224/jquery.validate.js"></script>
            TagBuilder tagBuilder = new TagBuilder("script");
            tagBuilder.Attributes["src"] = new UrlHelper(helper.ViewContext.RequestContext).Content(scriptPath);
            tagBuilder.Attributes["type"] = "text/javascript";
            return MvcHtmlString.Create(tagBuilder.ToString());
        }



        private static readonly IDictionary<string, int> ControllerToProductIdMap =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                {
                    {"grid", 718},
                    {"menu", 719},
                    {"panelbar", 720},
                    {"tabstrip", 721}
                };

        public static ExampleConfigurator Configurator(this HtmlHelper html, string title)
        {
            return new ExampleConfigurator(html)
                .Title(title);
        }

       
        public static IHtmlString ProductMetaTag(this HtmlHelper html)
        {
            var controller = (string)html.ViewContext.RouteData.Values["controller"];

            if (!ControllerToProductIdMap.ContainsKey(controller))
            {
                return string.Empty.Raw();
            }

            return
                String.Format("<meta name=\"ProductId\" content=\"{0}\" />", ControllerToProductIdMap[controller]).Raw();
        }


       

        public static string GetCurrentTheme(this HtmlHelper html)
        {
            return html.ViewContext.HttpContext.Request.QueryString["theme"] ?? "sunset";
        }

     

        public static string SwitchToRazorLink(this HtmlHelper html)
        {
#if MVC3
            var link = html.ActionLink("Switch to Razor view engine",
                (string)html.ViewContext.RouteData.Values["action"],
                new { area = "razor", controller = (string)html.ViewContext.RouteData.Values["controller"] },
                new { @class = "t-button" });

            return link.ToString();
#else
            return "";
#endif
        }
    }

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