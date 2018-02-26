using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Our.Umbraco.StackedContent.PropertyEditors;
using Umbraco.Core;
using Umbraco.Core.Logging;
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

            var db = context.Database;

            // get the patterns/replacements (e.g. the doctypes alias and guid replacement)
            var patterns = GetContentTypeRegexPatterns(db);

            // get all the property-data that has "icContentTypeAlias"
            var rows = GetPropertyDataRows(db);

            // loop over the property data
            foreach (var row in rows)
            {
                // loop over each pattern/replacement
                foreach (var pattern in patterns)
                {
                    // perform a regex replace against the property-data for each doctype alias
                    row.dataNtext = Regex.Replace(row.dataNtext, pattern.Key, pattern.Value);
                }

                // save back to database (transactional)
                using (var transaction = db.GetTransaction())
                {
                    db.Execute("UPDATE [cmsPropertyData] SET [dataNtext] = @0 WHERE [id] = @1", row.dataNtext, row.id);
                    transaction.Complete();
                }
            }
        }

        private Dictionary<string, string> GetContentTypeRegexPatterns(UmbracoDatabase db)
        {
            var sql = @"
SELECT DISTINCT
    ct.alias,
    LOWER(n.uniqueID) AS [uniqueID]
FROM
    cmsContentType AS ct
    INNER JOIN umbracoNode AS n ON n.id = ct.nodeId
    INNER JOIN cmsDataTypePreValues AS dtpv ON dtpv.[value] LIKE '%""icContentTypeAlias"": ""' + ct.alias + '""%'
    INNER JOIN cmsDataType AS dt ON dt.nodeId = dtpv.datatypeNodeId
WHERE
    dt.propertyEditorAlias = '@0' AND dtpv.alias = 'contentTypes'
;";
            return db
                .Query<ContentTypeDto>(sql, StackedContentPropertyEditor.PropertyEditorAlias)
                .ToDictionary(
                    x => $"\"icContentTypeAlias([\\\\\"]*):([\\\\\"]*){x.alias}([\\\\\"]*)",
                    x => $"\"icContentTypeGuid${{1}}:${{2}}{x.uniqueID}$3");
        }

        private IEnumerable<PropertyDataDto> GetPropertyDataRows(UmbracoDatabase db)
        {
            var sql = @"
SELECT
    pd.id,
    pd.dataNtext
FROM
    cmsPropertyData AS pd
    INNER JOIN cmsDocument AS d ON d.versionId = pd.versionId
WHERE
    d.newest = 1 AND pd.dataNtext LIKE '%icContentTypeAlias%:%'
;";
            return db.Query<PropertyDataDto>(sql);
        }

        private class ContentTypeDto
        {
            public string alias { get; set; }
            public string uniqueID { get; set; }
        }

        private class PropertyDataDto
        {
            public int id { get; set; }
            public string dataNtext { get; set; }
        }
    }
}