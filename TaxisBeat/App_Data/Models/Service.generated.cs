//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder v3.0.7.99
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.ModelsBuilder;
using Umbraco.ModelsBuilder.Umbraco;

namespace Umbraco.Web.PublishedContentModels
{
	/// <summary>Service</summary>
	[PublishedContentModel("service")]
	public partial class Service : PublishedContentModel
	{
#pragma warning disable 0109 // new is redundant
		public new const string ModelTypeAlias = "service";
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
#pragma warning restore 0109

		public Service(IPublishedContent content)
			: base(content)
		{ }

#pragma warning disable 0109 // new is redundant
		public new static PublishedContentType GetModelContentType()
		{
			return PublishedContentType.Get(ModelItemType, ModelTypeAlias);
		}
#pragma warning restore 0109

		public static PublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<Service, TValue>> selector)
		{
			return PublishedContentModelUtility.GetModelPropertyType(GetModelContentType(), selector);
		}

		///<summary>
		/// Content
		///</summary>
		[ImplementPropertyType("bodyText")]
		public Newtonsoft.Json.Linq.JToken BodyText
		{
			get { return this.GetPropertyValue<Newtonsoft.Json.Linq.JToken>("bodyText"); }
		}

		///<summary>
		/// Category
		///</summary>
		[ImplementPropertyType("category")]
		public IEnumerable<string> Category
		{
			get { return this.GetPropertyValue<IEnumerable<string>>("category"); }
		}

		///<summary>
		/// Description
		///</summary>
		[ImplementPropertyType("description")]
		public string Description
		{
			get { return this.GetPropertyValue<string>("description"); }
		}

		///<summary>
		/// Price
		///</summary>
		[ImplementPropertyType("price")]
		public decimal Price
		{
			get { return this.GetPropertyValue<decimal>("price"); }
		}

		///<summary>
		/// Service Icon
		///</summary>
		[ImplementPropertyType("serviceIcon")]
		public string ServiceIcon
		{
			get { return this.GetPropertyValue<string>("serviceIcon"); }
		}

		///<summary>
		/// Service Name
		///</summary>
		[ImplementPropertyType("serviceName")]
		public string ServiceName
		{
			get { return this.GetPropertyValue<string>("serviceName"); }
		}

		///<summary>
		/// Service Photo
		///</summary>
		[ImplementPropertyType("servicePhoto")]
		public IPublishedContent ServicePhoto
		{
			get { return this.GetPropertyValue<IPublishedContent>("servicePhoto"); }
		}

		///<summary>
		/// SKU
		///</summary>
		[ImplementPropertyType("sku")]
		public string Sku
		{
			get { return this.GetPropertyValue<string>("sku"); }
		}
	}
}
