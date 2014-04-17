﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Kooboo.CMS.Common.Runtime.Dependency;

namespace Kooboo.Commerce.HAL
{
    [Dependency(typeof(IUriResolver), ComponentLifeStyle.Singleton)]
    public class DefaultUriResolver : IUriResolver
    {
        private IResourceDescriptorProvider _resourceProvider;
        public DefaultUriResolver(IResourceDescriptorProvider resourceProvider)
        {
            _resourceProvider = resourceProvider;
        }

        /// <summary>
        /// find resource by uri, for example:
        /// when you find resource by /product/123, the resource /product/{id} may be found.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public ResourceDescriptor FindResource(string uri)
        {
            Uri inputUri = new Uri(uri, UriKind.Relative);
            var resources = _resourceProvider.GetAllDescriptors();
            foreach(var res in resources)
            {
                Uri resUri = new Uri(res.ResourceUri, UriKind.Relative);
                if (IsUriMatched(inputUri, resUri))
                {
                    return res;
                }
            }
            return null;
        }

        /// <summary>
        /// resolve url pattern by parameters.
        /// resolve uri /product/{id} with parameters dic{ id=123 } will output /product/123
        /// </summary>
        /// <param name="uriPattern"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public string Resovle(string uriPattern, Dictionary<string, object> paras)
        {
            Uri resUri = new Uri(uriPattern, UriKind.Relative);
            var segements = resUri.Segments.Select(o =>
                {
                    string val = o.TrimStart('/').TrimEnd('/');
                    return GetValueFromParameters(val, paras, val);
                });
            string queryString = string.Empty;
            if (!string.IsNullOrEmpty(resUri.Query))
            {
                var qs = HttpUtility.ParseQueryString(resUri.Query);
                foreach(var k in qs.AllKeys)
                {
                    string val = qs[k];
                    qs[k] = GetValueFromParameters(k, paras, val);
                }
                queryString = qs.ToString();
            }
            return string.Join("?", "/" + string.Join("/", segements.ToArray()), queryString);
        }

        /// <summary>
        /// verify if two uris are the same by comparing segements
        /// </summary>
        /// <param name="uri1">first uri</param>
        /// <param name="uri2">second uri</param>
        /// <returns>true if matched, else false</returns>
        private bool IsUriMatched(Uri uri1, Uri uri2)
        {
            if (uri1.Segments.Length != uri2.Segments.Length)
                return false;
            for(var i= 0; i<uri1.Segments.Length; i++)
            {
                string seg1 = uri1.Segments[i].TrimStart('/').TrimEnd('/');
                string seg2 = uri2.Segments[i].TrimStart('/').TrimEnd('/');
                // if the segement enclosed by {}, that means this segement allow any string. so continue.
                if ((seg1.StartsWith("{") && seg1.EndsWith("}")) || (seg2.StartsWith("{") && seg2.EndsWith("}")))
                    continue;
                if (string.Compare(uri1.Segments[i], uri2.Segments[i], true) != 0)
                    return false;
            }
            return true;
        }

        private string GetValueFromParameters(string key, Dictionary<string, object> paras, object defaultValue)
        {
            if (key.StartsWith("{") && key.EndsWith("}"))
            {
                string paraName = key.Substring(1, key.Length - 2);
                object paraVal = paras.ContainsKey(paraName) ? paras[paraName] : defaultValue;
                return paraVal == null ? defaultValue.ToString() : paraVal.ToString();
            }
            return defaultValue.ToString();
        }
    }
}
