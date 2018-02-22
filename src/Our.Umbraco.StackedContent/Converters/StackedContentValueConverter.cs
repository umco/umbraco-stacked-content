using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Our.Umbraco.InnerContent.Converters;
using Our.Umbraco.StackedContent.PropertyEditors;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.StackedContent.Converters
{
    [PropertyValueType(typeof(IEnumerable<IPublishedContent>))]
    public class StackedContentValueConverter : InnerContentValueConverter
    {
        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.InvariantEquals(StackedContentPropertyEditor.PropertyEditorAlias);
        }

        public override object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
        {
            if (source == null)
                return null;

            var str = source.ToString();
            if (string.IsNullOrWhiteSpace(str))
                return null;

            try
            {
                var rawValue = JsonConvert.DeserializeObject<JArray>(str);

                return ConvertInnerContentDataToSource(rawValue, null, 1, preview);
            }
            catch (Exception ex)
            {
                LogHelper.Error<StackedContentValueConverter>("Error converting value", ex);
            }

            return null;
        }
    }
}