using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Our.Umbraco.StackedContent.PropertyEditors;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Our.Umbraco.StackedContent.Migrations.TargetOneOneZero
{
    [Migration("1.1.0", 2, StackedContentPropertyEditor.PropertyEditorAlias)]
    public class UpdatePropertyData : MigrationBase
    {
        public UpdatePropertyData(ISqlSyntaxProvider sqlSyntax, ILogger logger)
            : base(sqlSyntax, logger)
        { }

        public override void Down()
        { }

        public override void Up()
        {
            var context = ApplicationContext.Current.DatabaseContext;
            if (context.IsDatabaseConfigured == false)
                return;

            var contentService = ApplicationContext.Current.Services.ContentService;

            // get the patterns/replacements (e.g. the doctypes alias and guid replacement)
            var patterns = GetContentTypeRegexPatterns(context);

            // get all the content node IDs (and property-type aliases) that have property-data with "icContentTypeAlias"
            var aliasIds = GetPropertyAliasWithContentNodeId(context);
            if (aliasIds.Any() == false)
                return;

            var contentToBeSaved = new List<IContent>();

            // loop over the content items
            foreach (var aliasId in aliasIds)
            {
                var content = contentService.GetById(aliasId.contentNodeId);
                var propertyDataValue = content.GetValue<string>(aliasId.propertyAlias);

                // loop over each pattern/replacement
                foreach (var pattern in patterns)
                {
                    if (Regex.IsMatch(propertyDataValue, pattern.Key) == false)
                        continue;

                    // perform a regex replace against the property-data for each doctype alias
                    propertyDataValue = Regex.Replace(propertyDataValue, pattern.Key, pattern.Value);
                }

                content.SetValue(aliasId.propertyAlias, propertyDataValue);

                if (content.IsPropertyDirty(aliasId.propertyAlias))
                    contentToBeSaved.Add(content);
            }

            if (contentToBeSaved.Count > 0)
            {
                contentService.Save(contentToBeSaved);
            }
        }

        private Dictionary<string, string> GetContentTypeRegexPatterns(DatabaseContext context)
        {
            var sql = @"
SELECT DISTINCT
    ct.alias AS [alias],
    LOWER(n.uniqueID) AS [uniqueID]
FROM
    cmsContentType AS ct
    INNER JOIN umbracoNode AS n ON n.id = ct.nodeId
    INNER JOIN cmsDataTypePreValues AS dtpv ON dtpv.[value] LIKE '%""icContentTypeGuid"":""' + LOWER(n.uniqueID) + '""%'
    INNER JOIN cmsDataType AS dt ON dt.nodeId = dtpv.datatypeNodeId
WHERE
    dt.propertyEditorAlias = @0 AND dtpv.alias = 'contentTypes'
;";
            using (var db = context.Database)
            {
                return db
                    .Fetch<ContentTypeDto>(sql, StackedContentPropertyEditor.PropertyEditorAlias)
                    .ToDictionary(
                        x => $"\"icContentTypeAlias([\\\\\"]*):([\\\\\"]*){x.alias}([\\\\\"]*)",
                        x => $"\"icContentTypeGuid${{1}}:${{2}}{x.uniqueID}$3");
            }
        }

        private IEnumerable<PropertyAliasContentNodeIdDto> GetPropertyAliasWithContentNodeId(DatabaseContext context)
        {
            var sql = @"
SELECT
    pd.contentNodeId,
    pt.Alias AS propertyAlias
FROM
    cmsPropertyData AS pd
    INNER JOIN cmsPropertyType AS pt ON pt.id = pd.propertytypeid 
    INNER JOIN cmsDocument AS d ON d.versionId = pd.versionId
WHERE
    d.newest = 1 AND pd.dataNtext LIKE '%icContentTypeAlias%:%'
;";
            using (var db = context.Database)
            {
                return db.Fetch<PropertyAliasContentNodeIdDto>(sql);
            }
        }

        private class ContentTypeDto
        {
            public string alias { get; set; }
            public string uniqueID { get; set; }
        }

        private class PropertyAliasContentNodeIdDto
        {
            public int contentNodeId { get; set; }
            public string propertyAlias { get; set; }
        }
    }
}