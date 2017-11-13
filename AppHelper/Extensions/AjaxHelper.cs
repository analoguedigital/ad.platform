using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace AppHelper
{
    public static class AjaxHelperExtensions
    {
        public static string ImageActionLink(this AjaxHelper helper, string imageUrl, string altText, string titleText, string actionName, object routeValues, AjaxOptions ajaxOptions)
        {
            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", imageUrl);
            builder.MergeAttribute("alt", altText);
            builder.MergeAttribute("title", titleText);
            var link = helper.ActionLink("[replaceme]", actionName, routeValues, ajaxOptions);
            return link.ToString().Replace("[replaceme]", builder.ToString(TagRenderMode.SelfClosing));
        }
    }
}
