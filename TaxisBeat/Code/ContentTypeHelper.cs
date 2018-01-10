using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web.PublishedContentModels;

namespace TaxisBeat.Code
{
    public sealed class ContentTypeHelper
    {
        private static readonly Lazy<ContentTypeHelper> lazy = new Lazy<ContentTypeHelper>(() => new ContentTypeHelper());
        public static ContentTypeHelper Default { get { return lazy.Value; } }

        private IContentType EmailRepoType { get; set; }
        public IEnumerable<IContent> EmailTemplateRepos { get; private set; }
        public IContent ContactUsRepo { get; private set; }

        public IDictionary<string, PreValue> ArrayServiceList { get; private set; }
        public IDictionary<string, PreValue> DoysArray { get; private set; }
        public IDictionary<string, PreValue> FaqCategories { get; private set; }

        public ContentTypeHelper()
        {
            var services = ApplicationContext.Current.Services;
            var allContentTypes = services.ContentTypeService.GetAllContentTypes();
            EmailRepoType = allContentTypes.SingleOrDefault(z => z.Alias == EmailTemplateRepository.ModelTypeAlias);
            EmailTemplateRepos = services.ContentService.GetContentOfContentType(EmailRepoType.Id).Where(z => z.Published);

            //var contactUsServicesdd = services.DataTypeService.GetAllDataTypeDefinitions().FirstOrDefault(z => z.Name == Constants.ContactUsServiceDropdownName);
            //ArrayServiceList = services.DataTypeService.GetPreValuesCollectionByDataTypeId(contactUsServicesdd.Id).PreValuesAsDictionary;

            var memberType = services.MemberTypeService.Get(Constants.MemberAlias);
            var doyPropertyType = memberType.PropertyTypes.FirstOrDefault(propertyType => string.Equals(propertyType.Alias, Constants.DoyDropDownAlias));
            DoysArray = services.DataTypeService.GetPreValuesCollectionByDataTypeId(doyPropertyType.DataTypeDefinitionId).PreValuesAsDictionary;
        }
    }
}