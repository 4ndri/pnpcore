﻿using Microsoft.SharePoint.Client;
using PnP.Core.Transformation.SharePoint.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PnP.Core.Transformation.SharePoint.Extensions
{
    internal static class WebExtensions
    {
        internal static string GetLocalizedListName(this Web web, ListType listType, string defaultName)
        {
            var context = web.Context;

            int lcid = (int)web.EnsureProperty(w => w.Language);

            // TODO: Consider using caching

            string resourceName = null;
            string assemblyName = "core";
            switch (listType)
            {
                case ListType.Blogs:
                    resourceName = "$Resources:blogpost_Folder";
                    break;
                case ListType.SitePages:
                    resourceName = "$Resources:pages_Folder";
                    break;
                case ListType.PublishingPages:
                    resourceName = "$Resources:List_Pages_UrlName";
                    assemblyName = "osrvcore";
                    break;
                default:
                    break;
            }

            ClientResult<string> result = Microsoft.SharePoint.Client.Utilities.Utility.GetLocalizedString(context, resourceName, assemblyName, lcid);
            context.ExecuteQueryRetry();
            var listName = new Regex(@"['´`]").Replace(result.Value, "");

            return (string.IsNullOrEmpty(listName) ? defaultName : listName).ToLowerInvariant();
        }
    }

    /// <summary>
    /// Defines the supported types of lists and libraries
    /// </summary>
    internal enum ListType
    {
        /// <summary>
        /// List of blogs
        /// </summary>
        Blogs,
        /// <summary>
        /// List of site pages
        /// </summary>
        SitePages,
        /// <summary>
        /// List of pages
        /// </summary>
        PublishingPages,
    }
}
