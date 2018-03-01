using System;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Our.Umbraco.StackedContent.Models
{
    internal class UnpublishedProperty : IPublishedProperty
    {
        private readonly PublishedPropertyType propertyType;
        private readonly object rawValue;
        private readonly Lazy<object> sourceValue;
        private readonly Lazy<object> objectValue;
        private readonly Lazy<object> xpathValue;
        private readonly bool isPreview;

        public UnpublishedProperty(PublishedPropertyType propertyType, object value)
            : this(propertyType, value, false)
        { }

        public UnpublishedProperty(PublishedPropertyType propertyType, object value, bool isPreview)
        {
            this.propertyType = propertyType;
            this.isPreview = isPreview;

            this.rawValue = value;

            this.sourceValue = new Lazy<object>(() => this.propertyType.ConvertDataToSource(this.rawValue, this.isPreview));
            this.objectValue = new Lazy<object>(() => this.propertyType.ConvertSourceToObject(this.sourceValue.Value, this.isPreview));
            this.xpathValue = new Lazy<object>(() => this.propertyType.ConvertSourceToXPath(this.sourceValue.Value, this.isPreview));
        }

        public string PropertyTypeAlias => this.propertyType.PropertyTypeAlias;

        public bool HasValue => this.DataValue != null && this.DataValue.ToString().Trim().Length > 0;

        public object DataValue => this.rawValue;

        public object Value => this.objectValue.Value;

        public object XPathValue => this.xpathValue.Value;
    }
}