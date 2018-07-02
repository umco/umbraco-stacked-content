using Our.Umbraco.InnerContent.PropertyEditors;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.StackedContent.PropertyEditors
{
    internal class StackedContentPreValueEditor : SimpleInnerContentPreValueEditor
    {
        public StackedContentPreValueEditor()
            : base()
        {
            Fields.AddRange(new[]
            {
                new PreValueField
                {
                    Key = "maxItems",
                    Name = "Max Items",
                    View = "number",
                    Description = "Set the maximum number of items allowed in this stack."
                },
                new PreValueField
                {
                    Key = "singleItemMode",
                    Name = "Single Item Mode",
                    View = "boolean",
                    Description = "Set whether to work in single item mode (only the first defined Content Type will be used)."
                },
                new PreValueField
                {
                    Key = "hideLabel",
                    Name = "Hide Label",
                    View = "boolean",
                    Description = "Set whether to hide the editor label and have the list take up the full width of the editor window."
                },
                new PreValueField
                {
                    Key = "disablePreview",
                    Name = "Disable Preview",
                    View = "boolean",
                    Description = "Set whether to disable the preview of the items in the stack."
                }
            });
        }
    }
}