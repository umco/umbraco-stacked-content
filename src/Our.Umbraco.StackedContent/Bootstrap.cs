using System.Diagnostics;
using System.Reflection;
using Our.Umbraco.InnerContent.Helpers;
using Our.Umbraco.StackedContent.PropertyEditors;
using Semver;
using Umbraco.Core;

namespace Our.Umbraco.StackedContent
{
    public class Bootstrap : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ApplyMigrations(applicationContext);
        }

        private void ApplyMigrations(ApplicationContext applicationContext)
        {
            var productName = StackedContentPropertyEditor.PropertyEditorAlias;
            var currentVersion = new SemVersion(0, 0, 0);
            var targetVersion = GetApplicationVersion();

            MigrationHelper.ApplyMigrations(applicationContext, productName, currentVersion, targetVersion);
        }

        private SemVersion GetApplicationVersion()
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