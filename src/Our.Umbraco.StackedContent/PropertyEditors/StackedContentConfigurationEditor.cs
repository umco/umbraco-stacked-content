using Our.Umbraco.InnerContent.PropertyEditors;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.StackedContent.PropertyEditors
{
    internal class StackedContentConfigurationEditor : SimpleInnerContentConfigurationEditor
    {
        public StackedContentConfigurationEditor()
            : base()
        {
            Fields.AddRange(new[]
            {
                new ConfigurationField
                {
                    Key = "maxItems",
                    Name = "Max Items",
                    View = "number",
                    Description = "Set the maximum number of items allowed in this stack."
                },
                new ConfigurationField
                {
                    Key = "singleItemMode",
                    Name = "Single Item Mode",
                    View = "boolean",
                    Description = "Set whether to work in single item mode (only the first defined Content Type will be used)."
                },
                new ConfigurationField
                {
                    Key = "hideLabel",
                    Name = "Hide Label",
                    View = "boolean",
                    Description = "Set whether to hide the editor label and have the list take up the full width of the editor window."
                },
                new ConfigurationField
                {
                    Key = "enablePreview",
                    Name = "Enable Preview",
                    View = "boolean",
                    Description = "Select to enable a preview of the items in the stack."
                },
                new ConfigurationField
                {
                    Key = "enableCopy",
                    Name = "Enable Copy",
                    View = "boolean",
                    Description = "Select to enable copying (and pasting) of items in the stack."
                }
            });
        }
    }
}