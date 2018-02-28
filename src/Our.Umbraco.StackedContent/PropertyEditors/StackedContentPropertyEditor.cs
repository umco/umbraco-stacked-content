using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Our.Umbraco.InnerContent.PropertyEditors;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Our.Umbraco.StackedContent.PropertyEditors
{
    [PropertyEditor(PropertyEditorAlias, "Stacked Content", "/App_Plugins/StackedContent/views/stackedcontent.html", Group = "rich content", Icon = "icon-umb-contour", ValueType = "JSON")]
    public class StackedContentPropertyEditor : PropertyEditor
    {
        public const string PropertyEditorAlias = "Our.Umbraco.StackedContent";

        private IDictionary<string, object> _defaultPreValues;
        public override IDictionary<string, object> DefaultPreValues
        {
            get { return _defaultPreValues; }
            set { _defaultPreValues = value; }
        }

        public StackedContentPropertyEditor()
        {
            // Setup default values
            _defaultPreValues = new Dictionary<string, object>
            {
                { "contentTypes", "" },
                { "maxItems", 0 },
                { "singleItemMode", "0" },
                { "disablePreview", "0" }
            };
        }

        internal static bool TryEnsureContentTypeGuids(JArray items, IContentTypeService contentTypeService)
        {
            if (items == null)
                return false;

            var persist = false;

            foreach (var item in items)
            {
                var contentTypeGuid = item["icContentTypeGuid"];
                if (contentTypeGuid != null)
                    continue;

                var contentTypeAlias = item["icContentTypeAlias"];
                if (contentTypeAlias == null)
                    continue;

                var docType = contentTypeService.GetContentType(contentTypeAlias.Value<string>());
                if (docType == null)
                    continue;

                item["icContentTypeGuid"] = docType.Key.ToString();
                persist = true;
            }

            return persist;
        }

        #region Pre Value Editor

        protected override PreValueEditor CreatePreValueEditor()
        {
            return new StackPreValueEditor();
        }

        internal class StackPreValueEditor : PreValueEditor
        {
            [PreValueField("contentTypes", "Doc Types", "~/App_Plugins/InnerContent/views/innercontent.doctypepicker.html", Description = "Select the doc types to use as the data blueprint.")]
            public string[] ContentTypes { get; set; }

            [PreValueField("maxItems", "Max Items", "number", Description = "Set the maximum number of items allowed in this stack.")]
            public string MaxItems { get; set; }

            [PreValueField("singleItemMode", "Single Item Mode", "boolean", Description = "Set whether to work in single item mode (only the first defined Doc Type will be used).")]
            public string SingleItemMode { get; set; }

            [PreValueField("hideLabel", "Hide Label", "boolean", Description = "Set whether to hide the editor label and have the list take up the full width of the editor window.")]
            public string HideLabel { get; set; }

            [PreValueField("disablePreview", "Disable Preview", "boolean", Description = "Set whether to disable the preview of the items in the stack.")]
            public string DisablePreview { get; set; }

            public override IDictionary<string, object> ConvertDbToEditor(IDictionary<string, object> defaultPreVals, PreValueCollection persistedPreVals)
            {
                if (persistedPreVals.IsDictionaryBased)
                {
                    var dict = persistedPreVals.PreValuesAsDictionary;
                    if (dict.TryGetValue("contentTypes", out PreValue contentTypes) && string.IsNullOrWhiteSpace(contentTypes.Value) == false)
                    {
                        var items = JArray.Parse(contentTypes.Value);
                        if (TryEnsureContentTypeGuids(items, ApplicationContext.Current.Services.ContentTypeService))
                        {
                            contentTypes.Value = items.ToString();
                        }
                    }
                }

                return base.ConvertDbToEditor(defaultPreVals, persistedPreVals);
            }
        }

        #endregion

        #region Value Editor

        protected override PropertyValueEditor CreateValueEditor()
        {
            return new StackedContentPropertyValueEditor(base.CreateValueEditor());
        }

        internal class StackedContentPropertyValueEditor : SimpleInnerContentPropertyValueEditor
        {
            public StackedContentPropertyValueEditor(PropertyValueEditor wrapped)
                : base(wrapped)
            { }

            public override object ConvertDbToEditor(Property property, PropertyType propertyType, IDataTypeService dataTypeService)
            {
                if (property.Value == null)
                    return string.Empty;

                var propertyValue = property.Value.ToString();
                if (string.IsNullOrWhiteSpace(propertyValue))
                    return string.Empty;

                var value = JsonConvert.DeserializeObject<JToken>(propertyValue);
                if (value is JArray items)
                {
                    if (TryEnsureContentTypeGuids(items, ApplicationContext.Current.Services.ContentTypeService))
                    {
                        property.Value = items.ToString();
                    }
                }

                return base.ConvertDbToEditor(property, propertyType, dataTypeService);
            }
        }

        #endregion
    }
}