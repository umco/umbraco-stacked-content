using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Our.Umbraco.InnerContent.Helpers;
using Our.Umbraco.StackedContent.Models;
using Our.Umbraco.StackedContent.Web.Helpers;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.Routing;
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
                // TODO: Review. Previewing multiple blocks on the same page will make subsequent calls to the ContentService. Is it cacheable? [LK:2018-12-12]
                // Get page container node, otherwise it's unpublished then fake PublishedContent (by IContent object)
                page = UmbracoContext.Content.GetById(pageId) ?? new UnpublishedContent(pageId, Services, UmbracoContextAccessor);

                // TODO: Implement later [LK:2019-04-03]
                //                // Ensure PublishedContentRequest exists, just in case there are any RTE Macros to render
                //                if (UmbracoContext.PublishedRequest == null)
                //                {
                //                    var pageUrl = page.UrlAbsolute();

                //                    // If the page is unpublished, then the URL will be empty or a hash '#'.
                //                    // Use the current request as a fallback, as we need to give the PublishedContentRequest a URI.
                //                    if (string.IsNullOrEmpty(pageUrl) || pageUrl.Equals("#"))
                //                    {
                //                        pageUrl = string.Concat(Request.RequestUri.GetLeftPart(UriPartial.Authority), "/#", pageId);
                //                    }

                //#pragma warning disable CS0618 // Type or member is obsolete
                //                    var pcr = new PublishedRequest(new Uri(pageUrl), UmbracoContext.RoutingContext);
                //#pragma warning restore CS0618 // Type or member is obsolete

                //                    UmbracoContext.PublishedRequest = pcr;
                //                    UmbracoContext.PublishedRequest.PublishedContent = page;
                //                    UmbracoContext.PublishedRequest.Prepare();
                //                }
            }

            // TODO: Review. The values in `item` are the "editor values", whereas for prevalue-based editors, the converter is expecting the "database value". [LK:2018-12-12]

            // Convert item
            var content = InnerContentHelper.ConvertInnerContentToPublishedElement(item);

            // Construct preview model
            var model = new PreviewModel { Page = page, Item = content };

            // Render view
            var markup = ViewHelper.RenderPartial(content.ContentType.Alias, model);

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