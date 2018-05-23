#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2018
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//

#endregion


namespace DotNetNuke.Modules.Events.Components.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Content;
    using DotNetNuke.Entities.Content.Common;
    using global::Components;

    public class Content
    {
        /// <summary>
        ///     This should only run after the Event exists in the data store.
        /// </summary>
        /// <returns>The newly created ContentItemID from the data store.</returns>
        internal ContentItem CreateContentEventRecurMaster(EventRecurMasterInfo objEventRecurMaster, int tabId)
        {
            var objContent = new ContentItem
                                 {
                                     Content = objEventRecurMaster.EventDesc,
                                     ContentTypeId = GetContentTypeId(Constants.ContentEventRecurMasterTypeName),
                                     Indexed = false,
                                     ContentKey = "RecurItemID=" + objEventRecurMaster.RecurMasterID +
                                                  "&mctl=EventDetails",
                                     ModuleID = objEventRecurMaster.ModuleID,
                                     TabID = tabId
                                 };

            objContent.ContentItemId = Convert.ToInt32(Util.GetContentController().AddContentItem(objContent));

            return objContent;
        }

        /// <summary>
        ///     This is used to update the content in the ContentItems table. Should be called when an Event is updated.
        /// </summary>
        internal void UpdateContentEventRecurMaster(EventRecurMasterInfo objEventRecurMaster)
        {
            var objContent = Util.GetContentController().GetContentItem(objEventRecurMaster.ContentItemID);

            if (ReferenceEquals(objContent, null))
            {
                return;
            }
            objContent.Content = objEventRecurMaster.EventDesc;
            objContent.ContentKey = "ItemID=" + objEventRecurMaster.RecurMasterID + "&mctl=EventDetails";

            Util.GetContentController().UpdateContentItem(objContent);
        }

        /// <summary>
        ///     This should only run after the Event exists in the data store.
        /// </summary>
        /// <returns>The newly created ContentItemID from the data store.</returns>
        internal ContentItem CreateContentEvent(EventInfo objEvent, int tabId)
        {
            var objContent = new ContentItem
                                 {
                                     Content = objEvent.EventDesc,
                                     ContentTypeId = GetContentTypeId(Constants.ContentEventTypeName),
                                     Indexed = false,
                                     ContentKey = "ItemID=" + objEvent.EventID + "&mctl=EventDetails",
                                     ModuleID = objEvent.ModuleID,
                                     TabID = tabId
                                 };

            objContent.ContentItemId = Convert.ToInt32(Util.GetContentController().AddContentItem(objContent));

            return objContent;
        }

        /// <summary>
        ///     This is used to update the content in the ContentItems table. Should be called when an Event is updated.
        /// </summary>
        internal void UpdateContentEvent(EventInfo objEvent)
        {
            var objContent = Util.GetContentController().GetContentItem(objEvent.ContentItemID);

            if (ReferenceEquals(objContent, null))
            {
                return;
            }
            objContent.Content = objEvent.EventDesc;
            objContent.ContentKey = "ItemID=" + objEvent.EventID + "&mctl=EventDetails";

            Util.GetContentController().UpdateContentItem(objContent);
        }

        /// <summary>
        ///     This removes a content item associated with an event from the data store.
        /// </summary>
        /// <param name="contentItemId"></param>
        internal void DeleteContentItem(int contentItemId)
        {
            if (contentItemId <= Null.NullInteger)
            {
                return;
            }
            var objContent = Util.GetContentController().GetContentItem(contentItemId);
            if (ReferenceEquals(objContent, null))
            {
                return;
            }

            Util.GetContentController().DeleteContentItem(objContent);
        }

        /// <summary>
        ///     This is used to determine the ContentTypeID (part of the Core API) based on this module's content type. If the
        ///     content type doesn't exist yet for the module, it is created.
        /// </summary>
        /// <returns>The primary key value (ContentTypeID) from the core API's Content Types table.</returns>
        internal static int GetContentTypeId(string contentTypeConstant)
        {
            var typeController = new ContentTypeController();
            IEnumerable<ContentType> colContentTypes =
                from t in typeController.GetContentTypes() where t.ContentType == contentTypeConstant select t;
            var contentTypeId = 0;

            if (colContentTypes.Any())
            {
                var contentType = colContentTypes.Single();
                contentTypeId = Convert.ToInt32(ReferenceEquals(contentType, null)
                                                    ? CreateContentType(contentTypeConstant)
                                                    : contentType.ContentTypeId);
            }
            else
            {
                contentTypeId = CreateContentType(contentTypeConstant);
            }

            return contentTypeId;
        }

        #region Private Methods

        /// <summary>
        ///     Creates a Content Type (for taxonomy) in the data store.
        /// </summary>
        /// <returns>The primary key value of the new ContentType.</returns>
        private static int CreateContentType(string contentTypeConstant)
        {
            var typeController = new ContentTypeController();
            var objContentType = new ContentType {ContentType = contentTypeConstant};

            return typeController.AddContentType(objContentType);
        }

        #endregion
    }
}