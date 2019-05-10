using System;
using System.Collections.Generic;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.StackedContent.Web
{
    public class PreviewModel : IPublishedElement
    {
        public IPublishedContent Page { get; set; }

        public IPublishedElement Item { get; set; }

        #region IPublishedContent Implementation

        public IPublishedContentType ContentType => Item.ContentType;

        public Guid Key => Item.Key;

        public IEnumerable<IPublishedProperty> Properties => Item.Properties;

        public IPublishedProperty GetProperty(string alias)
        {
            return Item.GetProperty(alias);
        }

        #endregion
    }
}