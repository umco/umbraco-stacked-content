using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Our.Umbraco.StackedContent.Web.Helpers
{
    internal static class ViewHelper
    {
        class DummyController : Controller { }

        public static string RenderPartial(string partialName, object model, string[] viewLocations)
        {
            var sw = new StringWriter();
            var httpContext = new HttpContextWrapper(HttpContext.Current);

            var routeData = new RouteData();
            routeData.Values.Add("controller", "DummyController");

            var controllerContext = new ControllerContext(new RequestContext(httpContext, routeData), new DummyController());

            var viewEngine = new RazorViewEngine
            {
                PartialViewLocationFormats = viewLocations
            };

            var viewResult = viewEngine.FindPartialView(controllerContext, partialName, false);
            if (viewResult.View == null)
            {
                //TODO: Log lack of view?
                return null;
            }

            viewResult.View.Render(new ViewContext(controllerContext, viewResult.View, new ViewDataDictionary { Model = model }, new TempDataDictionary(), sw), sw);

            return sw.ToString();
        }

    }
}
