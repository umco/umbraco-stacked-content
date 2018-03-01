using Umbraco.Core.PropertyEditors;
using Our.Umbraco.InnerContent.PropertyEditors;

namespace Our.Umbraco.StackedContent.PropertyEditors
{
    [PropertyEditor(PropertyEditorAlias, "Stacked Content", "/App_Plugins/StackedContent/views/stackedcontent.html", Group = "rich content", Icon = "icon-umb-contour", ValueType = "JSON")]
    public class StackedContentPropertyEditor : SimpleInnerContentPropertyEditor
    {
        public const string PropertyEditorAlias = "Our.Umbraco.StackedContent";

        public StackedContentPropertyEditor()
            : base()
        { }
    }
}