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
    using System.Collections.Generic;
    using System.Linq;
    using DotNetNuke.Services.Journal;
    using global::Components;

    public class Journal
    {
        #region Internal Methods

        /// <summary>
        ///     Informs the core journal that an event has been created.
        /// </summary>
        /// <param name="objEvent"></param>
        /// <param name="tabId"></param>
        /// <param name="url"></param>
        /// <param name="imageurl"></param>
        public void NewEvent(EventInfo objEvent, int tabId, string url, string imageurl)
        {
            var objectKey = Constants.ContentEventTypeName + "_" + Constants.JournalEventCreateTypeName + "_" +
                            string.Format("{0}:{1}", objEvent.ModuleID, objEvent.EventID);
            var ji = JournalController.Instance.GetJournalItemByKey(objEvent.PortalID, objectKey);

            if (ji != null)
            {
                JournalController.Instance.DeleteJournalItemByKey(objEvent.PortalID, objectKey);
            }

            var securitySet = "E,";
            var socialGroupId = 0;
            if (objEvent.SocialGroupId > 0)
            {
                securitySet = ",";
                socialGroupId = objEvent.SocialGroupId;
            }
            else if (objEvent.SocialUserId > 0)
            {
                securitySet = ",";
            }

            ji = new JournalItem
                     {
                         PortalId = objEvent.PortalID,
                         ProfileId = objEvent.OwnerID,
                         UserId = objEvent.CreatedByID,
                         ContentItemId = objEvent.ContentItemID,
                         Title = objEvent.EventName,
                         ItemData = new ItemData {Url = url, ImageUrl = imageurl},
                         Summary = objEvent.Summary,
                         Body = objEvent.EventDesc,
                         JournalTypeId = GetEventCreateJournalTypeId(objEvent.PortalID),
                         ObjectKey = objectKey,
                         SecuritySet = securitySet,
                         SocialGroupId = socialGroupId
                     };

            JournalController.Instance.SaveJournalItem(ji, tabId);
        }

        /// <summary>
        ///     Informs the core journal that an event has been deleted.
        /// </summary>
        /// <param name="objEvent"></param>
        public void DeleteEvent(EventInfo objEvent)
        {
            var objectKey = Constants.ContentEventTypeName + "_" + Constants.JournalEventCreateTypeName + "_" +
                            string.Format("{0}:{1}", objEvent.ModuleID, objEvent.EventID);
            JournalController.Instance.DeleteJournalItemByKey(objEvent.PortalID, objectKey);
        }

        /// <summary>
        ///     Informs the core journal that a user has enrolled to attend an event.
        /// </summary>
        /// <param name="objEventSignup"></param>
        /// <param name="objEvent"></param>
        /// <param name="tabId"></param>
        /// <param name="url"></param>
        /// <param name="userid"></param>
        public void NewEnrollment(EventSignupsInfo objEventSignup, EventInfo objEvent, int tabId, string url,
                                  int userid)
        {
            var objectKey = Constants.ContentEventTypeName + "_" + Constants.JournalEventAttendTypeName + "_" +
                            string.Format("{0}:{1};{2}", objEventSignup.ModuleID, objEventSignup.EventID,
                                          objEventSignup.SignupID);
            var ji = JournalController.Instance.GetJournalItemByKey(objEvent.PortalID, objectKey);

            if (ji != null)
            {
                JournalController.Instance.DeleteJournalItemByKey(objEvent.PortalID, objectKey);
            }

            var securitySet = "E,";
            var socialGroupId = 0;
            if (objEvent.SocialGroupId > 0)
            {
                securitySet = ",";
                socialGroupId = objEvent.SocialGroupId;
            }

            ji = new JournalItem
                     {
                         PortalId = objEvent.PortalID,
                         ProfileId = objEventSignup.UserID,
                         UserId = userid,
                         ContentItemId = 0,
                         Title = objEvent.EventName,
                         ItemData = new ItemData {Url = url},
                         Summary = null,
                         Body = null,
                         JournalTypeId = GetEventAttendJournalTypeId(objEvent.PortalID),
                         ObjectKey = objectKey,
                         SecuritySet = securitySet,
                         SocialGroupId = socialGroupId
                     };

            JournalController.Instance.SaveJournalItem(ji, tabId);
        }


        /// <summary>
        ///     Informs the core journal that a user has unenrolled from attend an event.
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="eventId"></param>
        /// <param name="signupId"></param>
        /// <param name="portalId"></param>
        public void DeleteEnrollment(int moduleId, int eventId, int signupId, int portalId)
        {
            var objectKey = Constants.ContentEventTypeName + "_" + Constants.JournalEventAttendTypeName + "_" +
                            string.Format("{0}:{1};{2}", moduleId, eventId, signupId);
            JournalController.Instance.DeleteJournalItemByKey(portalId, objectKey);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Returns a journal type associated with new event creation (using one of the core built in journal types).
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        private static int GetEventCreateJournalTypeId(int portalId)
        {
            var colJournalTypes = from t in JournalController.Instance.GetJournalTypes(portalId)
                                  where t.JournalType == Constants.JournalEventCreateTypeName
                                  select t;
            var journalTypeId = 0;

            IEnumerable<JournalTypeInfo> journalTypeInfos =
                colJournalTypes as JournalTypeInfo[] != null
                    ? colJournalTypes as JournalTypeInfo[]
                    : colJournalTypes.ToArray();
            if (journalTypeInfos.Any())
            {
                var journalType = journalTypeInfos.Single();
                journalTypeId = journalType.JournalTypeId;
            }
            else
            {
                journalTypeId = 21;
            }

            return journalTypeId;
        }

        /// <summary>
        ///     Returns a journal type associated with user event enrollment (using one of the core built in journal types).
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        private static int GetEventAttendJournalTypeId(int portalId)
        {
            var colJournalTypes = from t in JournalController.Instance.GetJournalTypes(portalId)
                                  where t.JournalType == Constants.JournalEventAttendTypeName
                                  select t;
            var journalTypeId = 0;

            IEnumerable<JournalTypeInfo> journalTypeInfos =
                colJournalTypes as JournalTypeInfo[] != null
                    ? colJournalTypes as JournalTypeInfo[]
                    : colJournalTypes.ToArray();
            if (journalTypeInfos.Any())
            {
                var journalType = journalTypeInfos.Single();
                journalTypeId = journalType.JournalTypeId;
            }
            else
            {
                journalTypeId = 22;
            }

            return journalTypeId;
        }

        #endregion
    }
}