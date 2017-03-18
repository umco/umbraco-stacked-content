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
            try
            {
                if (source != null && !source.ToString().IsNullOrWhiteSpace())
                {
                    var rawValue = JsonConvert.DeserializeObject<JArray>(source.ToString());

                    return ConvertInnerContentDataToSource(rawValue, null, 1, preview);
                }
            }
            catch (Exception e)
            {
                LogHelper.Error<StackedContentValueConverter>("Error converting value", e);
            }

            return null;
        }
    }
}