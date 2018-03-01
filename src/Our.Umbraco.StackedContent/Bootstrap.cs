using Our.Umbraco.StackedContent.Migrations;
using Umbraco.Core;

namespace Our.Umbraco.StackedContent
{
    public class Bootstrap : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            MigrationHelper.ApplyMigrations(applicationContext);
        }
    }
}