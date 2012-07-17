using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.WebPages;

namespace eAd.Website.CustomControls
{
    public class DialogBox
    {
        //rendering the dialog box
        public static void Render(string title, string message)
        {
      var  insideView=    ((System.Web.Mvc.WebViewPage) WebPageContext.Current.Page);
            insideView.Html.RenderPartial(
                "DialogBox",
                new DialogBox()
                {
                    Title = title,
                    Message = message
                });
        }

        //Properties
        public string Title { get; set; }
        public string Message { get; set; }

     
    }
}