using ASPCoreUtilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Services
{
    public class LinkBuilderService : ServiceBase
    {
        public LinkBuilderService(ServicesProvider services) : base(services)
        {

        }

        public string GenerateLink(string controller, string controllerAction, object values)
        {
            return Services.UrlHelper.Action(
                controllerAction, 
                controller.GetController(), 
                values, 
                Services.HttpContext.Request.Scheme);
        }

        //static class ObjectExtensions
        //{
        //    public static IDictionary<string, object> AddProperty(object obj, string name, object value)
        //    {
        //        var dictionary = ToDictionary(obj);
        //        dictionary.Add(name, value);
        //        return dictionary;
        //    }

        //    // helper
        //    public static IDictionary<string, object> ToDictionary(object obj)
        //    {
        //        IDictionary<string, object> result = new Dictionary<string, object>();
        //        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
        //        foreach (PropertyDescriptor property in properties)
        //        {
        //            result.Add(property.Name, property.GetValue(obj));
        //        }
        //        return result;
        //    }
        //}
    }
}
