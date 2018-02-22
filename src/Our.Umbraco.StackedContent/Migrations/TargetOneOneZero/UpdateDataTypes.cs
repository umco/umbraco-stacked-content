using Newtonsoft.Json;
using Our.Umbraco.StackedContent.PropertyEditors;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Our.Umbraco.StackedContent.Migrations.TargetOneOneZero
{
    [Migration("1.1.0", 1, StackedContentPropertyEditor.PropertyEditorAlias)]
    public class UpdateDataTypes : MigrationBase
    {
        public UpdateDataTypes(ISqlSyntaxProvider sqlSyntax, ILogger logger)
            : base(sqlSyntax, logger)
        { }

        public override void Down()
        { }

        public override void Up()
        {
            var contentTypeService = ApplicationContext.Current.Services.ContentTypeService;
            var dataTypeService = ApplicationContext.Current.Services.DataTypeService;

            // Find all the StackedContent data-types
            var dataTypes = dataTypeService.GetDataTypeDefinitionByPropertyEditorAlias(StackedContentPropertyEditor.PropertyEditorAlias);
            if (dataTypes == null)
                return;

            foreach (var dataType in dataTypes)
            {
                var requiresSave = false;

                var preValues = dataTypeService.GetPreValuesCollectionByDataTypeId(dataType.Id);
                if (preValues == null)
                    continue;

                var config = preValues.PreValuesAsDictionary;
                if (config.ContainsKey("contentTypes"))
                {
                    // Get all the saved doc-type aliases
                    var contentTypes = config["contentTypes"];
                    var json = contentTypes.Value;
                    if (string.IsNullOrWhiteSpace(json))
                        continue;

                    // Replace the doc-type alias with the doc-type GUID
                    var options = JsonConvert.DeserializeObject<ContentTypeOption[]>(json);
                    if (options == null)
                        continue;

                    foreach (var option in options)
                    {
                        // If the prevalue already has a GUID, then skip it
                        if (string.IsNullOrWhiteSpace(option.icContentTypeGuid) == false)
                            continue;

                        if (string.IsNullOrWhiteSpace(option.icContentTypeAlias))
                            continue;

                        var docType = contentTypeService.GetContentType(option.icContentTypeAlias);
                        if (docType == null)
                            continue;

                        option.icContentTypeGuid = docType.Key.ToString();
                        requiresSave = true;
                    }

                    // Save the prevalue and data-type
                    if (requiresSave)
                    {
                        contentTypes.Value = JsonConvert.SerializeObject(options);
                        dataTypeService.SaveDataTypeAndPreValues(dataType, config);
                    }
                }
            }
        }

#pragma warning disable IDE1006 // Naming Styles
        public class ContentTypeOption
        {
            public string icContentTypeAlias { get; set; }
            public string icContentTypeGuid { get; set; }
            public string nameTemplate { get; set; }
        }
#pragma warning restore IDE1006 // Naming Styles
    }
}