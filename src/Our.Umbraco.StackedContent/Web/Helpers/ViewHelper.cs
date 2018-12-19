using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core.Logging;

namespace Our.Umbraco.StackedContent.Web.Helpers
{
    public static class ViewHelper
    {
        private class DummyController : Controller { }

        private static readonly RazorViewEngine ViewEngine = new RazorViewEngine
        {
            PartialViewLocationFormats = new[]
            {
                "~/Views/Partials/Stack/{0}.cshtml",
                "~/Views/Partials/Stack/Default.cshtml"
            }
        };

        // NOTE: So not to flood the logs with repeat warnings, the missing partial-views are kept track of.
        private static readonly HashSet<string> MissingPartialViews = new HashSet<string>();

        public static void AddViewLocationFormats(params string[] viewLocationFormats)
        {
            var newFormats = ViewEngine
                .PartialViewLocationFormats
                .Union(viewLocationFormats)
                .ToArray();

            ViewEngine.PartialViewLocationFormats = newFormats;
        }

        internal static string RenderPartial(string partialName, object model)
        {
            using (var sw = new StringWriter())
            {
                var httpContext = new HttpContextWrapper(HttpContext.Current);

                var routeData = new RouteData();
                routeData.Values.Add("controller", "DummyController");

                var controllerContext = new ControllerContext(new RequestContext(httpContext, routeData), new DummyController());

                var viewResult = ViewEngine.FindPartialView(controllerContext, partialName, false);
                if (viewResult.View == null && MissingPartialViews.Contains(partialName) == false)
                {
                    LogHelper.Warn(typeof(ViewHelper), $"No view found for partial '{partialName}'");
                    MissingPartialViews.Add(partialName);
                    return null;
                }

                viewResult.View.Render(new ViewContext(controllerContext, viewResult.View, new ViewDataDictionary { Model = model }, new TempDataDictionary(), sw), sw);

                return sw.ToString();
            }
        }
    }
}