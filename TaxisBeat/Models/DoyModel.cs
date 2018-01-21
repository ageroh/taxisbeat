using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using TaxisBeat.Code;
using Umbraco.Core;
using Constants = TaxisBeat.Code.Constants;

namespace TaxisBeat.Models
{
    public class DoyModel
    {
        [DisplayName("Δ.Ο.Υ.")]
        public int? Doy { get; set; }

        public IEnumerable<SelectListItem> AllDoy()
        {
            return (IEnumerable<SelectListItem>)ApplicationContext.Current.ApplicationCache.RuntimeCache.GetCacheItem(
            $"{Constants.CacheKey_GetDOYs}",
            () =>
            {
                return
                 ContentTypeHelper.Default.DoysArray
                    .Where(z => !string.IsNullOrEmpty(z.Value?.Value?.ToString()))
                        .Select(t =>
                                new SelectListItem()
                                {
                                    Text = t.Value?.Value?.ToString(),
                                    Value = t.Value?.Id.ToInvariantString(),
                                    Selected = Doy != null && Doy == t.Value?.Id
                                }
                            ).OrderBy(x => x.Text);
                
            }, TimeSpan.FromMinutes(30));
        }
        public DoyModel()
        {

        }

        public DoyModel(string memberDoy)
        {
            var key = ContentTypeHelper.Default.DoysArray.FirstOrDefault(z => z.Value?.Value == memberDoy);
            Doy = key.Value != null ? key.Value.Id : default(int?);
        }
    }
}