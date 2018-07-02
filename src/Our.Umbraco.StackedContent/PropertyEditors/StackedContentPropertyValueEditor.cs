using Our.Umbraco.InnerContent.PropertyEditors;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.StackedContent.PropertyEditors
{
    internal class StackedContentPropertyValueEditor : SimpleInnerContentPropertyValueEditor
    {
        public StackedContentPropertyValueEditor(PropertyValueEditor wrapped)
            : base(wrapped)
        { }

        public override void ConfigureForDisplay(PreValueCollection preValues)
        {
            base.ConfigureForDisplay(preValues);

            var asDictionary = preValues.PreValuesAsDictionary;
            if (asDictionary.ContainsKey("hideLabel"))
            {
                var boolAttempt = asDictionary["hideLabel"].Value.TryConvertTo<bool>();
                if (boolAttempt.Success)
                {
                    HideLabel = boolAttempt.Result;
                }
            }
        }
    }
}