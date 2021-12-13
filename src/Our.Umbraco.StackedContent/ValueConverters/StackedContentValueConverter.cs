﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Our.Umbraco.InnerContent.ValueConverters;
using Our.Umbraco.StackedContent.PropertyEditors;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.StackedContent.ValueConverters
{
    public class StackedContentValueConverter : InnerContentValueConverter, IPropertyValueConverterMeta
    {
        public override bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.InvariantEquals(StackedContentPropertyEditor.PropertyEditorAlias);
        }

        public override object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
        {
            var value = source?.ToString();
            if (value == null || string.IsNullOrWhiteSpace(value))
                return null;

            try
            {
                var items = JsonConvert.DeserializeObject<JArray>(value);
                return base.ConvertInnerContentDataToSource(items, null, 1, preview);
            }
            catch (Exception ex)
            {
                LogHelper.Error<StackedContentValueConverter>("Error converting value", ex);
            }

            return null;
        }

        public Type GetPropertyValueType(PublishedPropertyType propertyType)
        {
            return typeof(IEnumerable<IPublishedContent>);
        }

        public PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType, PropertyCacheValue cacheValue)
        {
            return PropertyCacheLevel.Content;
        }
    }
}