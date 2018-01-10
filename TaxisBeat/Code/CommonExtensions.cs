using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Member = Umbraco.Web.PublishedContentModels.Member;
using System.Data;
using Umbraco.Web.PublishedCache;
using Umbraco.Web;
using TaxisBeat.Models;

namespace TaxisBeat.Code
{
    public static class CommonExtensions
    {
        // Umbraco Unwrap
        public static Guid GetRealKey(this IPublishedContent content)
        {
            return content.GetKey();
        }

        public static string GetRealKeyStr(this IPublishedContent content)
        {
            return content.GetKey().ToString("N");
        }

        private static Guid GetKey(this IPublishedContent content)
        {
            var contentWithKey = content as IPublishedContentWithKey;
            if (contentWithKey != null)
                return contentWithKey.Key;
            if (content is PublishedContentWrapped)
            {
                contentWithKey = ((PublishedContentWrapped)content).Unwrap() as IPublishedContentWithKey;
                if (contentWithKey != null)
                    return contentWithKey.Key;
            }
            return Guid.Empty;
        }


        public static MemberName ExtractNameSurname(this IMember logedInMember)
        {
            return logedInMember.Name._ExtractNameSurname();
        }

        public static MemberName ExtractNameSurname(this Member logedInMember)
        {
            return logedInMember.Name._ExtractNameSurname();
        }

        public static string Email(this IPublishedContent member)
        {
            if(member != null && member is Member)
            {
                return (member as Member).GetPropertyValue<string>("Email");
            }
                
            if(member != null && member.HasProperty("Email"))
            {
                return member.GetPropertyValue<string>("Email");
            }
            return string.Empty;
        }

        public static string UserName(this IPublishedContent member)
        {
            if (member != null)
            {
                return member.GetPropertyValue<string>("UserName");
            }
            return string.Empty;
        }
        
        private static MemberName _ExtractNameSurname(this string name)
        {    var memName = name.Split(' ');
            if (memName != null && memName.Count() > 1)
            {
                return new MemberName { Name = memName[1], Surname = memName[0] };
            }
            return new MemberName { Name = name, Surname = name };
        }



        
        // DataTables Ordinal
        public static void SetColumnsOrderRemoveRest(this DataTable table, params String[] columnNames)
        {
            int correctIndex = 0;
            foreach (var columnName in columnNames)
            {
                if(table.Columns[columnName] != null)
                {
                    table.Columns[columnName].SetOrdinal(correctIndex);
                }
                correctIndex++;
            }
            for (int index = table.Columns.Count - 1; index >= 0; index--)
            {
                if (table.Columns[index].Ordinal > correctIndex)
                    table.Columns.RemoveAt(index);
            }
        }
    }
}