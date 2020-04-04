using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using System;

namespace Rinsen.InnovationBoost.Extensions
{
    public static class MenuExtensions
    {
        public static IHtmlContent MenuItem(
            this IHtmlHelper htmlHelper,
            string icon,
            string controller)
        {
            return htmlHelper.MenuItem(icon, controller, controller, "index");
        }

        public static IHtmlContent MenuItem(
            this IHtmlHelper htmlHelper,
            string icon,
            string text,
            string controller)
            {
            return htmlHelper.MenuItem(icon, text, controller, "index");
        }


        public static IHtmlContent MenuItem(
            this IHtmlHelper htmlHelper,
            string icon,
            string text,
            string controller,
            string action
        )
        {

            //< li class="nav-item">
            //                <a class="nav-link active" href="#">
            //                    <img src = "/icons/house.svg" alt="" width="32" height="32" title="Bootstrap">
            //                    Dashboard<span class="sr-only">(current)</span>
            //                </a>
            //            </li>
            var urlHelper = new UrlHelperFactory().GetUrlHelper(htmlHelper.ViewContext);

            var li = new TagBuilder("li");
            li.AddCssClass("nav-item");

            var a = new TagBuilder("a");
            a.MergeAttribute("href", urlHelper.Action(action, controller));
            a.AddCssClass("nav-link");

            var img = new TagBuilder("img") { TagRenderMode = TagRenderMode.SelfClosing };
            img.MergeAttribute("src", $"/icons/{icon}.svg");
            img.MergeAttribute("alt", icon);
            img.MergeAttribute("width", "32");
            img.MergeAttribute("height", "32");
            img.MergeAttribute("title", icon);

            a.InnerHtml.SetHtmlContent(img);
            a.InnerHtml.AppendLine();
            a.InnerHtml.AppendHtml(text);
            
            if (IsActiveControllerAction(htmlHelper, controller, action))
            {
                a.AddCssClass("active");
                var span = new TagBuilder("span");
                span.AddCssClass("sr-only");
                span.InnerHtml.SetContent("(current)"); 
                a.InnerHtml.AppendHtml(span);
            }
            
            li.InnerHtml.AppendHtml(a);

            return li;
        }

        private static bool IsActiveControllerAction(IHtmlHelper htmlHelper, string controller, string action)
        {
            var routeData = htmlHelper.ViewContext.RouteData;
            var currentAction = routeData.Values["action"].ToString();
            var currentController = routeData.Values["controller"].ToString();

            if (string.Equals(currentAction, action, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(currentController, controller, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}
