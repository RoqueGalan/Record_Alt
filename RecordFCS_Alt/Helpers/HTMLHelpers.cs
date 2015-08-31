using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace RecordFCS_Alt.Helpers
{
    public static class HTMLHelpers
    {
        // Render BootStrap menu atributo with active class
        public static MvcHtmlString MenuItem(this HtmlHelper htmlHelper,
                                             string text, string action,
                                             string controller,
                                             object routeValues = null,
                                             object htmlAttributes = null)
        {
            var li = new TagBuilder("li");
            var routeData = htmlHelper.ViewContext.RouteData;
            var currentAction = routeData.GetRequiredString("action");
            var currentController = routeData.GetRequiredString("controller");
            if (string.Equals(currentAction,
                              action,
                              StringComparison.OrdinalIgnoreCase) &&
                string.Equals(currentController,
                              controller,
                              StringComparison.OrdinalIgnoreCase))
            {
                li.AddCssClass("active");
            }
            if (routeValues != null)
            {
                li.InnerHtml = (htmlAttributes != null)
                    ? htmlHelper.ActionLink(text,
                                            action,
                                            controller,
                                            routeValues,
                                            htmlAttributes).ToHtmlString()
                    : htmlHelper.ActionLink(text,
                                            action,
                                            controller,
                                            routeValues).ToHtmlString();
            }
            else
            {
                li.InnerHtml = (htmlAttributes != null)
                    ? htmlHelper.ActionLink(text,
                                            action,
                                            controller,
                                            null,
                                            htmlAttributes).ToHtmlString()
                    : htmlHelper.ActionLink(text,
                                            action,
                                            controller).ToHtmlString();
            }
            return MvcHtmlString.Create(li.ToString());
        }


        public static MvcHtmlString MenuItemIcon(this HtmlHelper htmlHelper,
                                             string text, string title, string action,
                                             string controller,
                                             object routeValues = null,
                                             object htmlAttributes = null)
        {
            var li = new TagBuilder("li");
            var routeData = htmlHelper.ViewContext.RouteData;
            var currentAction = routeData.GetRequiredString("action");
            var currentController = routeData.GetRequiredString("controller");

            if (string.Equals(currentAction,
                              action,
                              StringComparison.OrdinalIgnoreCase) &&
                string.Equals(currentController,
                              controller,
                              StringComparison.OrdinalIgnoreCase))
            {
                li.AddCssClass("active");
            }

            li.InnerHtml = htmlHelper.NoEncodeActionLink(text, title, action, controller, routeValues, htmlAttributes).ToHtmlString();

            return MvcHtmlString.Create(li.ToString());
        }


        // As the text the: "<span class='glyphicon glyphicon-plus'></span>" can be entered
        public static MvcHtmlString NoEncodeActionLink(this HtmlHelper htmlHelper,
                                             string text, string title, string action,
                                             string controller,
                                             object routeValues = null,
                                             object htmlAttributes = null)
        {
            UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            TagBuilder builder = new TagBuilder("a");
            builder.InnerHtml = text;
            builder.Attributes["title"] = title;
            builder.Attributes["href"] = urlHelper.Action(action, controller, routeValues);
            builder.MergeAttributes(new RouteValueDictionary(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)));

            return MvcHtmlString.Create(builder.ToString());
        }






        public static IHtmlString ActionLinkIcon(this HtmlHelper htmlHelper, string linkText,
                         object routeValues, object htmlAttributes)
        {
            return ActionLinkIcon(htmlHelper, linkText, null, null, routeValues, htmlAttributes);
        }

        public static IHtmlString ActionLinkIcon(this HtmlHelper htmlHelper, string linkText,
                        string action, object routeValues, object htmlAttributes)
        {
            return ActionLinkIcon(htmlHelper, linkText, action, null, routeValues, htmlAttributes);
        }

        /// <summary>
        /// HTML Extension method to support Bootstrap icon with text in ActionLink. For example
        /// @Html.ActionLinkIcon("<i class=\"icon-edit\"></i>", "ActionName", "Controller",
        ///                             new { parameter = parameterValue },
        ///                             new { id = "btnName", @class = "btn btn-success" })
        /// Important Note: Please provide link of icon as: <i class=\"icon-edit\"></i> NOT <i class=\"icon-edit\" />. NOTE: no <i />.
        /// Concept Credits: http://bloggingabout.net/blogs/rick/archive/2012/11/07/how-to-create-an-actionlink-with-a-twitter-bootstrap-icon-in-mvc4.aspx
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="linkText"></param>
        /// <param name="action"></param>
        /// <param name="controller"></param>
        /// <param name="routeValues"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static IHtmlString ActionLinkIcon(this HtmlHelper htmlHelper, string linkText,
                        string action, string controller, object routeValues, object htmlAttributes)
        {
            TagBuilder tagBuilder;
            UrlHelper urlHelper;

            urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            tagBuilder = new TagBuilder("a");

            tagBuilder.InnerHtml = linkText;

            tagBuilder.Attributes["href"] = urlHelper.Action(action, controller, routeValues);

            tagBuilder.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            return MvcHtmlString.Create(tagBuilder.ToString());
        }
    }
}