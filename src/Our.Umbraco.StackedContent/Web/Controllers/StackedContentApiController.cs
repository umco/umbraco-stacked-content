using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Our.Umbraco.InnerContent.Helpers;
using Our.Umbraco.StackedContent.Models;
using Our.Umbraco.StackedContent.Web.Helpers;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Our.Umbraco.StackedContent.Web.Controllers
{
    [PluginController("StackedContent")]
    public class StackedContentApiController : UmbracoAuthorizedApiController
    {
        [HttpPost]
        public HttpResponseMessage GetPreviewMarkup([FromBody] JObject item, int pageId)
        {
            var page = default(IPublishedContent);

            // If the page is new, then the ID will be zero
            if (pageId > 0)
            {
                // Get page container node
                page = UmbracoContext.ContentCache.GetById(pageId);
                if (page == null)
                {
                    // If unpublished, then fake PublishedContent (with IContent object)
                    page = new UnpublishedContent(pageId, Services);
                }
            }

            // Convert item
            var content = InnerContentHelper.ConvertInnerContentToPublishedContent(item, page);

            // Construct preview model
            var model = new PreviewModel { Page = page, Item = content };

            // Render view
            var markup = ViewHelper.RenderPartial(content.DocumentTypeAlias, model);

            // Return response
            var response = new HttpResponseMessage
            {
                Content = new StringContent(markup ?? string.Empty)
            };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Text.Html);

            return response;
        }
    }
}