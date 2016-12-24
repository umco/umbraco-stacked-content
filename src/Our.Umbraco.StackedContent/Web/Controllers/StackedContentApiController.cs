using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Our.Umbraco.InnerContent.Helpers;
using Our.Umbraco.StackedContent.Web.Helpers;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Our.Umbraco.StackedContent.Web.Controllers
{
    [PluginController("StackedContent")]
    public class StackedContentApiController : UmbracoAuthorizedApiController
    {
        [HttpPost]
        public HttpResponseMessage GetPreviewMarkup([FromBody] JObject item, int parentId)
        {
            // Get parent to container node
            //TODO: Convert IContent if no published content?
            var parent = UmbracoContext.ContentCache.GetById(parentId);

            // Convert item
            var content = InnerContentHelper.ConvertInnerContentToPublishedContent(item, parent);

            // Construct preview model
            var model = new PreviewModel { Page = parent, Item = content };

            // Render view
            var markup = ViewHelper.RenderPartial(content.DocumentTypeAlias, model, new[]
            {
                "~/Views/Partials/Stack/{0}.cshtml",
                "~/Views/Partials/Stack/Default.cshtml",
            });

            // Return response
            var response = new HttpResponseMessage
            {
                Content = new StringContent(markup ?? "")
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }
    }
}
