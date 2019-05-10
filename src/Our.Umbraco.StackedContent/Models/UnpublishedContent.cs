using System;
using System.Collections.Generic;
using System.Linq;
using Our.Umbraco.InnerContent.Models;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace Our.Umbraco.StackedContent.Models
{
    internal class UnpublishedContent : PublishedContentBase
    {
        private readonly IContent content;

        private readonly Lazy<IEnumerable<IPublishedContent>> children;
        private readonly Lazy<IPublishedContentType> contentType;
        private readonly Lazy<string> creatorName;
        private readonly Lazy<IPublishedContent> parent;
        private readonly Lazy<Dictionary<string, IPublishedProperty>> properties;
        private readonly Lazy<string> urlName;
        private readonly Lazy<string> writerName;

        public UnpublishedContent(int id, ServiceContext serviceContext, IUmbracoContextAccessor umbracoContextAccessor)
            : this(serviceContext.ContentService.GetById(id), serviceContext, umbracoContextAccessor)
        { }

        public UnpublishedContent(IContent content, ServiceContext serviceContext, IUmbracoContextAccessor umbracoContextAccessor)
            : base()
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (serviceContext == null) throw new ArgumentNullException(nameof(serviceContext));

            var userService = new Lazy<IUserService>(() => serviceContext.UserService);

            this.content = content;

            this.children = new Lazy<IEnumerable<IPublishedContent>>(() => ((global::Umbraco.Core.Services.Implement.ContentService)serviceContext.ContentService).GetPublishedChildren(this.content.Id).Select(x => new UnpublishedContent(x, serviceContext, umbracoContextAccessor)).ToList());
            this.contentType = new Lazy<IPublishedContentType>(() => umbracoContextAccessor.UmbracoContext.PublishedSnapshot.Content.GetContentType(this.content.ContentType?.Alias));
            this.creatorName = new Lazy<string>(() => this.content.GetCreatorProfile(userService.Value).Name);
            this.parent = new Lazy<IPublishedContent>(() => new UnpublishedContent(serviceContext.ContentService.GetById(this.content.ParentId), serviceContext, umbracoContextAccessor));
            this.properties = new Lazy<Dictionary<string, IPublishedProperty>>(() => MapProperties(Current.PropertyEditors, serviceContext));
            this.urlName = new Lazy<string>(() => this.content.Name.ToUrlSegment());
            this.writerName = new Lazy<string>(() => this.content.GetWriterProfile(userService.Value).Name);
        }

        public override Guid Key => this.content.Key;

        public override PublishedItemType ItemType => PublishedItemType.Content;

        public override IReadOnlyDictionary<string, PublishedCultureInfo> Cultures => throw new NotImplementedException();

        public override int Id => this.content.Id;

        public override int? TemplateId => this.content.TemplateId;

        public override int SortOrder => this.content.SortOrder;

        public override string Name => this.content.Name;

        public override string UrlSegment => this.urlName.Value;

        public override string WriterName => this.writerName.Value;

        public override string CreatorName => this.creatorName.Value;

        public override int WriterId => this.content.WriterId;

        public override int CreatorId => this.content.CreatorId;

        public override string Path => this.content.Path;

        public override DateTime CreateDate => this.content.CreateDate;

        public override DateTime UpdateDate => this.content.UpdateDate;

        public override int Level => this.content.Level;

        public override IPublishedContent Parent => this.parent.Value;

        public override IEnumerable<IPublishedContent> Children => this.children.Value;

        public override IPublishedContentType ContentType => this.contentType.Value;

        public override IEnumerable<IPublishedProperty> Properties => this.properties.Value.Values;

        public override IEnumerable<IPublishedContent> ChildrenForAllCultures
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override IPublishedProperty GetProperty(string alias)
        {
            return this.properties.Value.TryGetValue(alias, out IPublishedProperty property) ? property : null;
        }

        public override bool IsDraft(string culture = null)
        {
            return true;
        }

        public override bool IsPublished(string culture = null)
        {
            return false;
        }

        private Dictionary<string, IPublishedProperty> MapProperties(PropertyEditorCollection editors, ServiceContext services)
        {
            // TODO: Figure out what the "owner" object is for. [LK:2019-04-02]
            var owner = default(IPublishedElement);

            var contentType = this.contentType.Value;
            var properties = this.content.Properties;

            var items = new Dictionary<string, IPublishedProperty>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var propertyType in contentType.PropertyTypes)
            {
                var property = properties.FirstOrDefault(x => x.Alias.InvariantEquals(propertyType.Alias));
                var value = property.GetValue();
                if (value != null && editors.TryGet(propertyType.Alias, out IDataEditor propertyEditor))
                {
                    // TODO: Clarify this is correct. [LK:2019-04-02]
                    var valueEditor = propertyEditor.GetValueEditor();
                    value = valueEditor.ConvertDbToString(property.PropertyType, value, services.DataTypeService);
                }

                items.Add(propertyType.Alias, new DetachedPublishedProperty(propertyType, owner,value, false));
            }

            return items;
        }
    }
}