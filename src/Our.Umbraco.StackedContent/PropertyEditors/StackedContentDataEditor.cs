using Our.Umbraco.InnerContent.PropertyEditors;
using Umbraco.Core.Composing;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.StackedContent.PropertyEditors
{
    [DataEditor(
        DataEditorAlias,
        EditorType.PropertyValue,
        DataEditorName,
        DataEditorViewPath,
        Group = "rich content",
        Icon = "icon-umb-contour",
        ValueType = ValueTypes.Json)]
    public class StackedContentDataEditor : SimpleInnerContentDataEditor
    {
        public const string DataEditorAlias = "Our.Umbraco.StackedContent";
        public const string DataEditorName = "[UMCO] Stacked Content 8";
        public const string DataEditorViewPath = "~/App_Plugins/StackedContent/views/stackedcontent.html";

        public StackedContentDataEditor()
            : base(Current.Logger, EditorType.PropertyValue)
        {
            DefaultConfiguration.Add("maxItems", 0);
            DefaultConfiguration.Add("singleItemMode", "0");
            DefaultConfiguration.Add("hideLabel", "0");
            DefaultConfiguration.Add("enablePreview", "0");
            DefaultConfiguration.Add("enableCopy", "0");
        }

        protected override IConfigurationEditor CreateConfigurationEditor()
        {
            return new StackedContentConfigurationEditor();
        }

        protected override IDataValueEditor CreateValueEditor()
        {
            return new SimpleInnerContentValueEditor(Attribute);
        }
    }
}