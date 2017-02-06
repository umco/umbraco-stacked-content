using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Our.Umbraco.InnerContent.PropertyEditors;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Editors;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;

namespace Our.Umbraco.StackedContent.PropertyEditors
{
    [PropertyEditor(PropertyEditorAlias, "Stacked Content", "/App_Plugins/StackedContent/views/stackedcontent.html", ValueType = "JSON")]
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
                {"contentTypes", ""},
                {"maxItems", 0},
                {"singleItemMode", "0"},
                {"disablePreview", "0"}
            };
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
        }

        #endregion

        #region Value Editor

        protected override PropertyValueEditor CreateValueEditor()
        {
            return new StackPropertyValueEditor(base.CreateValueEditor());
        }

        internal class StackPropertyValueEditor : InnerContentPropertyValueEditorWrapper
        {
            public StackPropertyValueEditor(PropertyValueEditor wrapped)
                : base(wrapped)
            { }

            public override string ConvertDbToString(Property property, PropertyType propertyType, IDataTypeService dataTypeService)
            {
                // Convert / validate value
                if (property.Value == null || string.IsNullOrWhiteSpace(property.Value.ToString()))
                    return string.Empty;

                var value = JsonConvert.DeserializeObject<JArray>(property.Value.ToString());
                if (value == null)
                    return string.Empty;

                // Process value
                ConvertInnerContentDbToString(value);

                // Update the value on the property
                property.Value = JsonConvert.SerializeObject(value);

                // Pass the call down
                return base.ConvertDbToString(property, propertyType, dataTypeService);
            }

            public override object ConvertDbToEditor(Property property, PropertyType propertyType, IDataTypeService dataTypeService)
            {
                // Convert / validate value
                if (property.Value == null || string.IsNullOrWhiteSpace(property.Value.ToString()))
                    return string.Empty;

                var value = JsonConvert.DeserializeObject<JArray>(property.Value.ToString());
                if (value == null)
                    return string.Empty;

                // Process value
                ConvertInnerContentDbToEditor(value);

                // Update the value on the property
                property.Value = JsonConvert.SerializeObject(value);

                // Pass the call down
                return base.ConvertDbToEditor(property, propertyType, dataTypeService);
            }

            public override object ConvertEditorToDb(ContentPropertyData editorValue, object currentValue)
            {
                // Convert / validate value
                if (editorValue.Value == null || string.IsNullOrWhiteSpace(editorValue.Value.ToString()))
                    return null;

                var value = JsonConvert.DeserializeObject<JArray>(editorValue.Value.ToString());
                if (value == null || value.Count == 0)
                    return null;

                // Process value
                ConvertInnerContentEditorToDb(value);

                // Return value
                return JsonConvert.SerializeObject(value);
            }
        }

        #endregion
    }
}