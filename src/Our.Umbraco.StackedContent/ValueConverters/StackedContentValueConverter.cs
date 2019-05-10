using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Our.Umbraco.InnerContent.ValueConverters;
using Our.Umbraco.StackedContent.PropertyEditors;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.StackedContent.ValueConverters
{
    public class StackedContentValueConverter : InnerContentValueConverter
    {
        public override bool IsConverter(IPublishedPropertyType propertyType) => propertyType.EditorAlias.InvariantEquals(StackedContentDataEditor.DataEditorAlias);

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview)
        {
            var value = source?.ToString();
            if (value == null || string.IsNullOrWhiteSpace(value))
                return null;

            try
            {
                var items = JsonConvert.DeserializeObject<JArray>(value);
                return ConvertInnerContentDataToSource(items, preview);
            }
            catch (Exception ex)
            {
                Current.Logger.Error<StackedContentValueConverter>("Error converting value", ex);
            }

            return null;
        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType) => typeof(IEnumerable<IPublishedElement>);

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) => PropertyCacheLevel.Elements;
    }
}