using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using Our.Umbraco.StackedContent.PropertyEditors;
using Semver;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Migrations;

namespace Our.Umbraco.StackedContent.Migrations
{
    internal static class MigrationHelper
    {
        public static void ApplyMigrations(ApplicationContext applicationContext)
        {
            var migrationName = StackedContentPropertyEditor.PropertyEditorAlias;
            var currentVersion = new SemVersion(0, 0, 0);
            var targetVersion = GetApplicationVersion();

            // get the latest migration executed
            var latestMigration = applicationContext.Services.MigrationEntryService
                .GetAll(migrationName)
                .OrderByDescending(x => x.Version)
                .FirstOrDefault();

            if (latestMigration != null)
                currentVersion = latestMigration.Version;

            if (targetVersion == currentVersion)
                return;

            var migrationsRunner = new MigrationRunner(
                applicationContext.Services.MigrationEntryService,
                applicationContext.ProfilingLogger.Logger,
                currentVersion,
                targetVersion,
                migrationName);

            try
            {
                migrationsRunner.Execute(applicationContext.DatabaseContext.Database);
            }
            catch (HttpException)
            {
                // because umbraco runs some other migrations after the migration runner
                // is executed we get HttpException
                // catch this error, but don't do anything
                // fixed in 7.4.2+ see : http://issues.umbraco.org/issue/U4-8077
            }
            catch (Exception ex)
            {
                LogHelper.Error<MigrationRunner>("Error running migration.", ex);
            }
        }

        private static SemVersion GetApplicationVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            if (assembly != null)
            {
                var info = FileVersionInfo.GetVersionInfo(assembly.Location);
                if (info != null
                    && string.IsNullOrWhiteSpace(info.ProductVersion) == false
                    && SemVersion.TryParse(info.ProductVersion, out SemVersion applicationVersion))
                {
                    return applicationVersion;
                }
            }

            return new SemVersion(0, 0, 0);
        }
    }
}