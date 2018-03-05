using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web.Models;

namespace Our.Umbraco.StackedContent.Models
{
    internal class UnpublishedContent : PublishedContentWithKeyBase
    {
        private readonly IContent content;
        private readonly ServiceContext serviceContext;

        private readonly Lazy<IEnumerable<IPublishedContent>> children;
        private readonly Lazy<PublishedContentType> contentType;
        private readonly Lazy<string> creatorName;
        private readonly Lazy<IPublishedContent> parent;
        private readonly IPublishedProperty[] properties;
        private readonly Lazy<string> urlName;
        private readonly Lazy<string> writerName;

        public UnpublishedContent(int id, ServiceContext serviceContext)
            : this(serviceContext.ContentService.GetById(id), serviceContext)
        { }

        public UnpublishedContent(IContent content, ServiceContext serviceContext)
            : base()
        {
            Mandate.ParameterNotNull(content, nameof(content));
            Mandate.ParameterNotNull(serviceContext, nameof(serviceContext));

            var userService = new Lazy<IUserService>(() => serviceContext.UserService);

            this.content = content;
            this.serviceContext = serviceContext;

            this.children = new Lazy<IEnumerable<IPublishedContent>>(() => this.content.Children().Select(x => new UnpublishedContent(x, serviceContext)));
            this.contentType = new Lazy<PublishedContentType>(() => PublishedContentType.Get(this.ItemType, this.DocumentTypeAlias));
            this.creatorName = new Lazy<string>(() => this.content.GetCreatorProfile(userService.Value).Name);
            this.parent = new Lazy<IPublishedContent>(() => new UnpublishedContent(this.content.Parent(), serviceContext));
            this.urlName = new Lazy<string>(() => this.content.Name.ToUrlSegment());
            this.writerName = new Lazy<string>(() => this.content.GetWriterProfile(userService.Value).Name);

            // TODO: Refactor `MapProperties` [LK:2018-03-05]
            this.properties = MapProperties(
                this.contentType.Value.PropertyTypes,
                this.content.Properties,
                (t, v) => new UnpublishedProperty(t, v, true)).ToArray();
        }

        public override Guid Key => this.content.Key;

        public override PublishedItemType ItemType => PublishedItemType.Content;

        public override int Id => this.content.Id;

        public override int TemplateId => this.content.Template?.Id ?? default(int);

        public override int SortOrder => this.content.SortOrder;

        public override string Name => this.content.Name;

        public override string UrlName => this.urlName.Value;

        public override string DocumentTypeAlias => this.content.ContentType?.Alias;

        public override int DocumentTypeId => this.content.ContentType?.Id ?? default(int);

        public override string WriterName => this.writerName.Value;

        public override string CreatorName => this.creatorName.Value;

        public override int WriterId => this.content.WriterId;

        public override int CreatorId => this.content.CreatorId;

        public override string Path => this.content.Path;

        public override DateTime CreateDate => this.content.CreateDate;

        public override DateTime UpdateDate => this.content.UpdateDate;

        public override Guid Version => this.content.Version;

        public override int Level => this.content.Level;

        public override bool IsDraft => true;

        public override IPublishedContent Parent => this.parent.Value;

        public override IEnumerable<IPublishedContent> Children => this.children.Value;

        public override PublishedContentType ContentType => this.contentType.Value;

        public override ICollection<IPublishedProperty> Properties => this.properties;

        public override IPublishedProperty GetProperty(string alias)
        {
            return this.properties.FirstOrDefault(x => x.PropertyTypeAlias.InvariantEquals(alias));
        }

        private IEnumerable<IPublishedProperty> MapProperties(
            IEnumerable<PublishedPropertyType> propertyTypes,
            IEnumerable<Property> properties,
            Func<PublishedPropertyType, object, IPublishedProperty> map)
        {
            var propertyEditorResolver = PropertyEditorResolver.Current;
            var dataTypeService = this.serviceContext.DataTypeService;

            return propertyTypes.Select(x =>
            {
                var p = properties.SingleOrDefault(xx => xx.Alias == x.PropertyTypeAlias);
                var v = p == null || p.Value == null ? null : p.Value;
                if (v != null)
                {
                    var e = propertyEditorResolver.GetByAlias(x.PropertyEditorAlias);

                    // We are converting to string, even for database values which are integer or
                    // DateTime, which is not optimum. Doing differently would require that we have a way to tell
                    // whether the conversion to XML string changes something or not... which we don't, and we
                    // don't want to implement it as PropertyValueEditor.ConvertDbToXml/String should die anyway.

                    // Don't think about improving the situation here: this is a corner case and the real
                    // thing to do is to get rig of PropertyValueEditor.ConvertDbToXml/String.

                    // Use ConvertDbToString to keep it simple, although everywhere we use ConvertDbToXml and
                    // nothing ensures that the two methods are consistent.
                    if (e != null)
                    {
                        v = e.ValueEditor.ConvertDbToString(p, p.PropertyType, dataTypeService);
                    }
                }

                return map(x, v);
            });
        }
    }
}