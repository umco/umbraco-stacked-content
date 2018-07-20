using System.Collections.Generic;
using Our.Umbraco.InnerContent.PropertyEditors;
using Umbraco.Core.Models;
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
                    Key = "enablePreview",
                    Name = "Enable Preview",
                    View = "boolean",
                    Description = "Select to enable a preview of the items in the stack."
                },
                new PreValueField
                {
                    Key = "enableCopy",
                    Name = "Enable Copy",
                    View = "boolean",
                    Description = "Select to enable copying (and pasting) of items in the stack."
                }
            });
        }

        public override IDictionary<string, object> ConvertDbToEditor(IDictionary<string, object> defaultPreVals, PreValueCollection persistedPreVals)
        {
            // NOTE: For v1.0, we switched around the default option for the preview feature.
            // For backwards-compatibility, we check if the legacy "disablePreview" value is available and handle accordingly.
            if (persistedPreVals.IsDictionaryBased && persistedPreVals.PreValuesAsDictionary.ContainsKey("disablePreview"))
            {
                var enablePreview = persistedPreVals.PreValuesAsDictionary["disablePreview"].Value == "1" ? "0" : "1";
                if (persistedPreVals.PreValuesAsDictionary.ContainsKey("enablePreview"))
                {
                    persistedPreVals.PreValuesAsDictionary["enablePreview"].Value = enablePreview;
                }
                else
                {
                    persistedPreVals.PreValuesAsDictionary.Add("enablePreview", new PreValue(enablePreview));
                }
            }

            return base.ConvertDbToEditor(defaultPreVals, persistedPreVals);
        }
    }
}