using Our.Umbraco.InnerContent.PropertyEditors;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.StackedContent.PropertyEditors
{
    [PropertyEditor(PropertyEditorAlias, PropertyEditorName, "JSON", PropertyEditorViewPath, Group = "rich content", Icon = "icon-umb-contour")]
    public class StackedContentPropertyEditor : SimpleInnerContentPropertyEditor
    {
        public const string PropertyEditorAlias = "Our.Umbraco.StackedContent";
        public const string PropertyEditorName = "Stacked Content";
        public const string PropertyEditorViewPath = "~/App_Plugins/StackedContent/views/stackedcontent.html";

        public StackedContentPropertyEditor()
            : base()
        {
            DefaultPreValues.Add("maxItems", 0);
            DefaultPreValues.Add("singleItemMode", "0");
            DefaultPreValues.Add("disablePreview", "0");
        }

        protected override PreValueEditor CreatePreValueEditor()
        {
            return new StackedContentPreValueEditor();
        }

        protected override PropertyValueEditor CreateValueEditor()
        {
            return new StackedContentPropertyValueEditor(base.CreateValueEditor());
        }
    }
}