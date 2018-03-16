using System.Linq;
using Umbraco.Core.PropertyEditors;
using Our.Umbraco.InnerContent.PropertyEditors;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Our.Umbraco.StackedContent.PropertyEditors
{
    [PropertyEditor(PropertyEditorAlias, "Stacked Content", "/App_Plugins/StackedContent/views/stackedcontent.html", Group = "rich content", Icon = "icon-umb-contour", ValueType = "JSON")]
    public class StackedContentPropertyEditor : SimpleInnerContentPropertyEditor
    {
        public const string PropertyEditorAlias = "Our.Umbraco.StackedContent";

        public StackedContentPropertyEditor()
            : base()
        {
            DefaultPreValues.Add("maxItems", 0);
            DefaultPreValues.Add("singleItemMode", "0");
            DefaultPreValues.Add("disablePreview", "0");
        }

        protected override PropertyValueEditor CreateValueEditor()
        {
            return new StackedContentValueEditor(base.CreateValueEditor());
        }

        internal class StackedContentValueEditor : SimpleInnerContentPropertyValueEditor
        {
            public StackedContentValueEditor(PropertyValueEditor wrapped) : base(wrapped)
            { }

            public override void ConfigureForDisplay(PreValueCollection preValues)
            {
                base.ConfigureForDisplay(preValues);

                var asDictionary = preValues.PreValuesAsDictionary.ToDictionary(x => x.Key, x => x.Value.Value);
                if (asDictionary.ContainsKey("hideLabel"))
                {
                    var boolAttempt = asDictionary["hideLabel"].TryConvertTo<bool>();
                    if (boolAttempt.Success)
                    {
                        HideLabel = boolAttempt.Result;
                    }
                }
            }
        }

        protected override PreValueEditor CreatePreValueEditor()
        {
            return new StackedContentPreValueEditor();
        }

        internal class StackedContentPreValueEditor : SimpleInnerContentPreValueEditor
        {
            [PreValueField("maxItems", "Max Items", "number", Description = "Set the maximum number of items allowed in this stack.")]
            public string MaxItems { get; set; }

            [PreValueField("singleItemMode", "Single Item Mode", "boolean", Description = "Set whether to work in single item mode (only the first defined Content Type will be used).")]
            public string SingleItemMode { get; set; }

            [PreValueField("hideLabel", "Hide Label", "boolean", Description = "Set whether to hide the editor label and have the list take up the full width of the editor window.")]
            public string HideLabel { get; set; }

            [PreValueField("disablePreview", "Disable Preview", "boolean", Description = "Set whether to disable the preview of the items in the stack.")]
            public string DisablePreview { get; set; }
        }
    }
}