
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


namespace DotNetNuke.Modules.Events
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;
    using System.Threading;
    using System.Web;
    using DotNetNuke.Common.Lists;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Controllers;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Modules.Definitions;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Entities.Tabs;
    using DotNetNuke.Entities.Users;
    using DotNetNuke.Modules.Events.Components.Integration;
    using DotNetNuke.Security;
    using DotNetNuke.Security.Permissions;
    using DotNetNuke.Security.Roles;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.Services.Mail;
    using DotNetNuke.Services.Scheduling;
    using DotNetNuke.Services.Search;
    using global::Components;
    using Microsoft.VisualBasic;
    using Globals = DotNetNuke.Common.Globals;
    using MailPriority = DotNetNuke.Services.Mail.MailPriority;

    #region EventInfoHelper Class

    public class EventInfoHelper : PortalModuleBase
    {
        #region Private Functions and Methods

        // Adds a new Recurring Event (not the real event) to the EventInfo ArrayList
        // Sets Instance Date/Time and Converts to User Time Zone
        private void AddEvent(EventInfo objEvent)
        {
            var objEvent2 = objEvent.Clone();

            this.LstEvents.Add(objEvent2);
        }

        #endregion

        #region Public Functions and Methods

        // public properties
        public ArrayList LstEvents { get; set; } = new ArrayList();

        // Module ID of Base Calendar (used for converting Sub-Calendar Event Time Zones)

        public int BaseModuleID { get; set; }

        public int BaseTabID { get; set; }

        public int BasePortalID { get; set; }

        public EventModuleSettings BaseSettings { get; set; }

        public EventInfoHelper()
        { }

        public EventInfoHelper(int moduleID, EventModuleSettings settings)
        {
            this.BaseModuleID = moduleID;
            this.BaseSettings = settings;
        }

        public EventInfoHelper(int moduleID, int tabID, int portalID, EventModuleSettings settings)
        {
            this.BaseModuleID = moduleID;
            this.BaseTabID = tabID;
            this.BasePortalID = portalID;
            this.BaseSettings = settings;
        }

        public string GetDetailPageRealURL(int eventID, int socialGroupId, int socialUserId)
        {
            return this.GetDetailPageRealURL(eventID, true, socialGroupId, socialUserId);
        }

        public string GetDetailPageRealURL(int eventID, bool blSkinContainer, int socialGroupId, int socialUserId)
        {
            var url = "";
            if (this.BaseSettings.Eventdetailnewpage)
            {
                if (socialGroupId > 0)
                {
                    url = Globals.NavigateURL(this.BaseTabID, "Details", "Mid=" + this.BaseModuleID,
                                              "ItemID=" + eventID, "groupid=" + socialGroupId);
                }
                else if (socialUserId > 0)
                {
                    url = Globals.NavigateURL(this.BaseTabID, "Details", "Mid=" + this.BaseModuleID,
                                              "ItemID=" + eventID, "userid=" + socialUserId);
                }
                else
                {
                    url = Globals.NavigateURL(this.BaseTabID, "Details", "Mid=" + this.BaseModuleID,
                                              "ItemID=" + eventID);
                }
                if (blSkinContainer)
                {
                    url = this.AddSkinContainerControls(url, "?");
                }
            }
            else
            {
                if (socialGroupId > 0)
                {
                    url = Globals.NavigateURL(this.BaseTabID, "", "ModuleID=" + this.BaseModuleID, "ItemID=" + eventID,
                                              "mctl=EventDetails", "groupid=" + socialGroupId);
                }
                else if (socialUserId > 0)
                {
                    url = Globals.NavigateURL(this.BaseTabID, "", "ModuleID=" + this.BaseModuleID, "ItemID=" + eventID,
                                              "mctl=EventDetails", "userid=" + socialUserId);
                }
                else
                {
                    url = Globals.NavigateURL(this.BaseTabID, "", "ModuleID=" + this.BaseModuleID, "ItemID=" + eventID,
                                              "mctl=EventDetails");
                }
            }
            if (url.IndexOf("://") + 1 == 0)
            {
                var domainurl = this.GetDomainURL();
                url = Globals.AddHTTP(domainurl) + url;
            }
            return url;
        }

        public string GetDomainURL()
        {
            // Dim domainurl As String = ps.PortalAlias.HTTPAlias
            var domainurl = HttpContext.Current.Request.ServerVariables["HTTP_HOST"];
            if (ReferenceEquals(domainurl, null))
            {
                var ps = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
                domainurl = ps.PortalAlias.HTTPAlias;
            }
            if (domainurl.IndexOf("/", StringComparison.Ordinal) > 0)
            {
                domainurl = domainurl.Substring(0, domainurl.IndexOf("/", StringComparison.Ordinal));
            }
            return domainurl;
        }

        public string AddSkinContainerControls(string url, string addchar)
        {
            if (url.Substring(0, 10).ToLower() == "javascript")
            {
                return url;
            }

            if (this.BaseSettings.Enablecontainerskin)
            {
                var strFriendlyUrls = HostController.Instance.GetString("UseFriendlyUrls");
                if (strFriendlyUrls == "N")
                {
                    addchar = "&";
                }
                var objCtlTab = new TabController();
                var objTabInfo = objCtlTab.GetTab(this.BaseTabID, this.BasePortalID, false);
                string skinSrc = null;
                if (!ReferenceEquals(objTabInfo, null))
                {
                    if (!(objTabInfo.SkinSrc == ""))
                    {
                        skinSrc = objTabInfo.SkinSrc;
                        if (skinSrc.Substring(skinSrc.Length - 5, 5) == ".ascx")
                        {
                            skinSrc = skinSrc.Substring(0, skinSrc.Length - 5);
                        }
                    }
                }
                var objCtlModule = new ModuleController();
                var objModuleInfo = objCtlModule.GetModule(this.BaseModuleID, this.BaseTabID, false);
                string containerSrc = null;
                if (!ReferenceEquals(objModuleInfo, null))
                {
                    if (objModuleInfo.DisplayTitle)
                    {
                        if (!(objModuleInfo.ContainerSrc == ""))
                        {
                            containerSrc = objModuleInfo.ContainerSrc;
                        }
                        else if (!(objTabInfo.ContainerSrc == ""))
                        {
                            containerSrc = objTabInfo.ContainerSrc;
                        }
                        if (!ReferenceEquals(containerSrc, null))
                        {
                            if (containerSrc.Substring(containerSrc.Length - 5, 5) == ".ascx")
                            {
                                containerSrc = containerSrc.Substring(0, containerSrc.Length - 5);
                            }
                        }
                    }
                    else
                    {
                        containerSrc = "[G]Containers/_default/No+Container";
                    }
                }
                if (!ReferenceEquals(containerSrc, null))
                {
                    url += addchar + "ContainerSrc=" + HttpUtility.HtmlEncode(containerSrc);
                    addchar = "&";
                }
                if (!ReferenceEquals(skinSrc, null))
                {
                    url += addchar + "SkinSrc=" + HttpUtility.HtmlEncode(skinSrc);
                }
            }
            return url;
        }

        public bool HideFullEvent(EventInfo objevent, bool blEventHideFullEnroll, int intuserid, bool blAuthenticated)
        {
            if (objevent.Signups && blEventHideFullEnroll && (objevent.MaxEnrollment > 0) &
                (objevent.Enrolled >= objevent.MaxEnrollment) & (this.UserId != objevent.OwnerID) &
                (intuserid != objevent.CreatedByID) & (intuserid != objevent.RmOwnerID) &&
                !this.IsModerator(blAuthenticated))
            {
                return true;
            }
            return false;
        }

        public string DetailPageURL(EventInfo objEvent)
        {
            return this.DetailPageURL(objEvent, true);
        }

        public string DetailPageURL(EventInfo objEvent, bool blSkinContainer)
        {
            var returnURL = "";
            if (objEvent.DetailPage)
            {
                if (objEvent.DetailURL.StartsWith("http"))
                {
                    returnURL = objEvent.DetailURL;
                }
                else
                {
                    returnURL = Globals.LinkClick(objEvent.DetailURL, this.BaseTabID, this.BaseModuleID, false);
                }
            }
            else
            {
                returnURL = this.GetDetailPageRealURL(objEvent.EventID, blSkinContainer, objEvent.SocialGroupId,
                                                      objEvent.SocialUserId);
            }
            return returnURL;
        }

        public string GetModerateUrl()
        {
            return Globals.NavigateURL(this.BaseTabID, "", "Mid=" + this.BaseModuleID, "mctl=EventModerate");
        }

        public string GetEditURL(int itemid, int socialGroupId, int socialUserId)
        {
            return this.GetEditURL(itemid, socialGroupId, socialUserId, "Single");
        }

        public string GetEditURL(int itemid, int socialGroupId, int socialUserId, string editRecur)
        {
            if (socialGroupId > 0)
            {
                return this.AddSkinContainerControls(
                    this.EditUrl("ItemID", itemid.ToString(), "Edit", "Mid=" + this.BaseModuleID,
                                 "EditRecur=" + editRecur, "groupid=" + socialGroupId), "?");
            }
            if (socialUserId > 0)
            {
                return this.AddSkinContainerControls(
                    this.EditUrl("ItemID", itemid.ToString(), "Edit", "Mid=" + this.BaseModuleID,
                                 "EditRecur=" + editRecur, "userid=" + socialUserId), "?");
            }
            return this.AddSkinContainerControls(
                this.EditUrl("ItemID", itemid.ToString(), "Edit", "Mid=" + this.BaseModuleID, "EditRecur=" + editRecur),
                "?");
        }

        // Detemines if a EventInfo ArrayList already contains the EventInfo Object
        public DateTime IsConflict(EventInfo objEvent, ArrayList objEvents, DateTime conflictDateChk)
        {
            var objEvent1 = default(EventInfo);
            var objEvent2 = default(EventInfo);
            var eventTimeBegin1 = default(DateTime);
            var eventTimeBegin2 = default(DateTime);
            var eventTimeEnd1 = default(DateTime);
            var eventTimeEnd2 = default(DateTime);
            var locationConflict = this.BaseSettings.Locationconflict;

            // Handle Recurring Event Conflict Detection
            this.LstEvents = new ArrayList();
            this.AddEvent(objEvent);

            //Convert both lists to common timezone
            objEvents = this.ConvertEventListToDisplayTimeZone(objEvents, this.BaseSettings.TimeZoneId);
            this.LstEvents = this.ConvertEventListToDisplayTimeZone(this.LstEvents, this.BaseSettings.TimeZoneId);

            foreach (EventInfo tempLoopVar_objEvent1 in this.LstEvents)
            {
                objEvent1 = tempLoopVar_objEvent1;
                // Take into account timezone offsets and length of event when deciding on conflicts
                eventTimeBegin1 = objEvent1.EventTimeBegin;
                eventTimeEnd1 = objEvent1.EventTimeEnd;
                foreach (EventInfo tempLoopVar_objEvent2 in objEvents)
                {
                    objEvent2 = tempLoopVar_objEvent2;
                    eventTimeBegin2 = objEvent2.EventTimeBegin;
                    eventTimeEnd2 = objEvent2.EventTimeEnd;
                    if ((eventTimeBegin1 >= eventTimeBegin2 && eventTimeBegin1 < eventTimeEnd2 ||
                         eventTimeBegin1 <= eventTimeBegin2 && eventTimeEnd1 > eventTimeBegin2)
                        && objEvent1.EventID != objEvent2.EventID)
                    {
                        if (locationConflict)
                        {
                            if ((objEvent1.Location > 0) & (objEvent1.Location == objEvent2.Location))
                            {
                                return objEvent.EventTimeBegin;
                            }
                        }
                        else
                        {
                            return objEvent.EventTimeBegin;
                        }
                    }
                }
            }
            return conflictDateChk;
        }

        // Get Events (including SubModule Events, including categories, including locations)
        public ArrayList GetEvents(DateTime startDate, DateTime endDate, bool getSubEvents, ArrayList categoryIDs,
                                   ArrayList locationIDs, int socialGroupId, int socialUserId)
        {
            return this.GetEvents(startDate, endDate, getSubEvents, categoryIDs, locationIDs, false, socialGroupId,
                                  socialUserId);
        }

        public ArrayList GetEvents(DateTime startDate, DateTime endDate, bool getSubEvents, ArrayList inCategoryIDs,
                                   ArrayList inLocationIDs, bool isSearch, int socialGroupId, int socialUserId)
        {
            //Dim objEventInfoHelper As New EventInfoHelper(ModID)
            var objCtlMasterEvent = new EventMasterController();
            var moduleIDs = this.BaseModuleID.ToString();
            try
            {
                //*** See what Sub-Events/Calendars are included
                if (getSubEvents)
                {
                    // Get Assigned Sub Events and Bind to Grid
                    var subEvents = default(ArrayList);
                    subEvents = objCtlMasterEvent.EventsMasterAssignedModules(this.BaseModuleID);
                    if (!ReferenceEquals(subEvents, null))
                    {
                        var myEnumerator = subEvents.GetEnumerator();
                        var objSubEvent = default(EventMasterInfo);
                        while (myEnumerator.MoveNext())
                        {
                            objSubEvent = (EventMasterInfo) myEnumerator.Current;
                            if (this.IsModuleViewer(objSubEvent.SubEventID) || !this.BaseSettings.Enforcesubcalperms)
                            {
                                moduleIDs = moduleIDs + "," + objSubEvent.SubEventID;
                            }
                        }
                    }
                }

                // Adds Recurring Dates to EventInfo ArrayList for given Date Range
                var objEvents = default(EventInfo);
                var ctrlEvent = new EventController();
                this.LstEvents = new ArrayList();
                this.LstEvents.Clear();

                var categoryIDs = this.CreateCategoryFilter(inCategoryIDs);
                var locationIDs = this.CreateLocationFilter(inLocationIDs);

                var newlstEvents = new ArrayList();
                if ((this.BaseSettings.SocialGroupModule == EventModuleSettings.SocialModule.No) |
                    this.IsSocialUserPublic(socialUserId) || this.IsSocialGroupPublic(socialGroupId))
                {
                    newlstEvents =
                        ctrlEvent.EventsGetByRange(moduleIDs, startDate, endDate, categoryIDs, locationIDs,
                                                   socialGroupId, socialUserId);
                }

                foreach (EventInfo tempLoopVar_objEvents in newlstEvents)
                {
                    objEvents = tempLoopVar_objEvents;
                    // If the module is set for private events, then obfuscate the appropriate information
                    objEvents.IsPrivate = false;
                    if (this.BaseSettings.PrivateMessage != "")
                    {
                        if (isSearch)
                        {
                            objEvents.EventName = this.BaseSettings.PrivateMessage;
                            objEvents.EventDesc = "";
                            objEvents.Summary = "";
                            objEvents.IsPrivate = true;
                        }
                        else if (!(this.UserId == objEvents.OwnerID) && !this.IsModerator(true))
                        {
                            objEvents.EventName = this.BaseSettings.PrivateMessage;
                            objEvents.EventDesc = "";
                            objEvents.Summary = "";
                            objEvents.IsPrivate = true;
                        }
                    }
                    objEvents.ModuleTitle = objCtlMasterEvent.GetModuleTitle(objEvents.ModuleID);
                    this.AddEvent(objEvents);
                }
                this.LstEvents.Sort(new EventDateSort());

                return this.LstEvents;
            }
            catch (Exception)
            {
                return new ArrayList();
            }
        }

        // Get Events for a Specifc Date and returns a EventInfo ArrayList
        public ArrayList GetDateEvents(ArrayList selectedEvents, DateTime selectDate)
        {
            var newEventEvents = new ArrayList();
            var itemid = 0;
            // Modified to account for multi-day events
            if (!ReferenceEquals(selectedEvents, null))
            {
                foreach (EventInfo objEvent in selectedEvents)
                {
                    if (objEvent.EventTimeBegin == selectDate ||
                        objEvent.EventTimeBegin.Date <= selectDate.Date &&
                        objEvent.EventTimeEnd.Date >= selectDate.Date)
                    {
                        if (itemid != objEvent.EventID)
                        {
                            newEventEvents.Add(objEvent);
                        }
                        itemid = objEvent.EventID;
                    }
                }
            }
            return newEventEvents;
        }

        public string CreateCategoryFilter(ArrayList inCategoryIDs)
        {
            var restrictedCategories = new ArrayList();
            if (this.BaseSettings.Restrictcategories)
            {
                if (inCategoryIDs[0].ToString() == "-1")
                {
                    restrictedCategories = this.BaseSettings.ModuleCategoryIDs;
                }
                else
                {
                    foreach (int inCategory in inCategoryIDs)
                    {
                        foreach (int category in this.BaseSettings.ModuleCategoryIDs)
                        {
                            if (category == inCategory)
                            {
                                restrictedCategories.Add(category);
                            }
                        }
                    }
                }
            }
            else
            {
                restrictedCategories = inCategoryIDs;
            }

            var categoryIDs = "";
            // ReSharper disable LoopCanBeConvertedToQuery
            foreach (string category in restrictedCategories)
            {
                // ReSharper restore LoopCanBeConvertedToQuery
                categoryIDs = categoryIDs + "," + category;
            }
            categoryIDs = categoryIDs.Substring(1);

            return categoryIDs;
        }

        public string CreateLocationFilter(ArrayList inLocationIDs)
        {
            // Because of method overloads without locations.
            if (ReferenceEquals(inLocationIDs, null))
            {
                inLocationIDs = new ArrayList(Convert.ToInt32(new[] {"-1"}));
            }

            var restrictedLocations = new ArrayList();
            if (this.BaseSettings.Restrictlocations)
            {
                if (inLocationIDs[0].ToString() == "-1")
                {
                    restrictedLocations = this.BaseSettings.ModuleLocationIDs;
                }
                else
                {
                    foreach (int inLocation in inLocationIDs)
                    {
                        foreach (int location in this.BaseSettings.ModuleLocationIDs)
                        {
                            if (location == inLocation)
                            {
                                restrictedLocations.Add(location);
                            }
                        }
                    }
                }
            }
            else
            {
                restrictedLocations = inLocationIDs;
            }

            var locationIDs = "";
            // ReSharper disable LoopCanBeConvertedToQuery
            foreach (string location in restrictedLocations)
            {
                // ReSharper restore LoopCanBeConvertedToQuery
                locationIDs = locationIDs + "," + location;
            }
            locationIDs = locationIDs.Substring(1);

            return locationIDs;
        }

        public bool IsModuleViewer(int subModuleID)
        {
            try
            {
                if (!PortalSecurity.IsInRole(this.PortalSettings.AdministratorRoleName))
                {
                    var objCtlModule = new ModuleController();

                    var objModules = objCtlModule.GetModuleTabs(subModuleID);
                    var objModule = default(ModuleInfo);
                    foreach (ModuleInfo tempLoopVar_objModule in objModules)
                    {
                        objModule = tempLoopVar_objModule;
                        if (!objModule.InheritViewPermissions)
                        {
                            var objCollModulePermission = default(ModulePermissionCollection);
                            objCollModulePermission =
                                ModulePermissionController.GetModulePermissions(subModuleID, objModule.TabID);
                            return ModulePermissionController.HasModulePermission(objCollModulePermission, "VIEW");
                        }
                        var objCollTabPermission = default(TabPermissionCollection);
                        objCollTabPermission =
                            TabPermissionController.GetTabPermissions(objModule.TabID, this.PortalId);
                        return TabPermissionController.HasTabPermission(objCollTabPermission, "VIEW");
                    }
                    return false;
                }
                return true;
            }
            catch
            { }
            return false;
        }

        public bool IsSocialUserPublic(int socialUserId)
        {
            if ((this.BaseSettings.SocialGroupModule == EventModuleSettings.SocialModule.UserProfile) &
                !this.BaseSettings.SocialUserPrivate)
            {
                return true;
            }
            if (socialUserId == -1)
            {
                return false;
            }
            if ((this.BaseSettings.SocialGroupModule == EventModuleSettings.SocialModule.UserProfile) &
                this.BaseSettings.SocialUserPrivate && socialUserId == this.PortalSettings.UserInfo.UserID)
            {
                return true;
            }
            return false;
        }

        public bool IsSocialGroupPublic(int socialGroupId)
        {
            if ((this.BaseSettings.SocialGroupModule == EventModuleSettings.SocialModule.SocialGroup) &
                !(this.BaseSettings.SocialGroupSecurity == EventModuleSettings.SocialGroupPrivacy.PrivateToGroup))
            {
                return true;
            }
            if (socialGroupId == -1)
            {
                return false;
            }
            var objRoleCtl = new RoleController();
            var objRoleInfo = objRoleCtl.GetRole(socialGroupId, this.PortalSettings.PortalId);
            if (ReferenceEquals(objRoleInfo, null))
            {
                return false;
            }
            if ((this.BaseSettings.SocialGroupModule == EventModuleSettings.SocialModule.SocialGroup) &
                (this.BaseSettings.SocialGroupSecurity == EventModuleSettings.SocialGroupPrivacy.PrivateToGroup) &
                this.PortalSettings.UserInfo.IsInRole(objRoleInfo.RoleName))
            {
                return true;
            }
            return false;
        }

        public bool IsModerator(bool blAuthenticated)
        {
            if (blAuthenticated)
            {
                try
                {
                    var mc = new ModuleController();
                    var objMod = default(ModuleInfo);
                    var mp = default(ModulePermissionCollection);

                    objMod = mc.GetModule(this.BaseModuleID, this.BaseTabID, false);

                    if (!ReferenceEquals(objMod, null))
                    {
                        mp = objMod.ModulePermissions;
                        return ModulePermissionController.HasModulePermission(mp, "EVENTSMOD");
                    }
                    return false;
                }
                catch
                { }
            }
            return false;
        }

        public ArrayList GetEventModuleViewers()
        {
            var objCtlModule = new ModuleController();
            var objModule = objCtlModule.GetModule(this.BaseModuleID, this.BaseTabID, false);

            var lstUsers = new ArrayList();
            var lstDeniedUsers = new ArrayList();

            if (!objModule.InheritViewPermissions)
            {
                var objModulePermission = default(ModulePermissionInfo);
                foreach (ModulePermissionInfo tempLoopVar_objModulePermission in objModule.ModulePermissions)
                {
                    objModulePermission = tempLoopVar_objModulePermission;
                    if (objModulePermission.PermissionKey == "VIEW")
                    {
                        if (objModulePermission.UserID < 0)
                        {
                            var roleName = "";
                            var objCtlRole = new RoleController();
                            if (objModulePermission.RoleID < 0)
                            {
                                roleName = this.PortalSettings.RegisteredRoleName;
                            }
                            else
                            {
                                roleName = objModulePermission.RoleName;
                            }
                            var lstRoleUsers = objCtlRole.GetUsersByRoleName(this.BasePortalID, roleName);
                            foreach (UserInfo objUser in lstRoleUsers)
                            {
                                this.AddViewUserid(objUser.UserID, objModulePermission.AllowAccess, lstUsers,
                                                   lstDeniedUsers);
                            }
                        }
                        else
                        {
                            this.AddViewUserid(objModulePermission.UserID, objModulePermission.AllowAccess, lstUsers,
                                               lstDeniedUsers);
                        }
                    }
                }
            }
            else
            {
                var objCollTabPermission = default(TabPermissionCollection);
                objCollTabPermission = TabPermissionController.GetTabPermissions(this.BaseTabID, this.BasePortalID);
                var objTabPermission = default(TabPermissionInfo);
                foreach (TabPermissionInfo tempLoopVar_objTabPermission in objCollTabPermission)
                {
                    objTabPermission = tempLoopVar_objTabPermission;
                    if (objTabPermission.PermissionKey == "VIEW")
                    {
                        if (objTabPermission.UserID < 0)
                        {
                            var roleName = "";
                            var objCtlRole = new RoleController();
                            if (objTabPermission.RoleID < 0)
                            {
                                roleName = this.PortalSettings.RegisteredRoleName;
                            }
                            else
                            {
                                roleName = objTabPermission.RoleName;
                            }
                            var lstRoleUsers = objCtlRole.GetUsersByRoleName(this.BasePortalID, roleName);
                            foreach (UserInfo objUser in lstRoleUsers)
                            {
                                this.AddViewUserid(objUser.UserID, objTabPermission.AllowAccess, lstUsers,
                                                   lstDeniedUsers);
                            }
                        }
                        else
                        {
                            this.AddViewUserid(objTabPermission.UserID, objTabPermission.AllowAccess, lstUsers,
                                               lstDeniedUsers);
                        }
                    }
                }
            }
            return lstUsers;
        }

        private void AddViewUserid(int viewUserid, bool viewAllowAccess, ArrayList lstUsers, ArrayList lstDeniedUsers)
        {
            if (lstUsers.Contains(viewUserid) && !viewAllowAccess)
            {
                lstUsers.Remove(viewUserid);
            }
            if (lstDeniedUsers.Contains(viewUserid))
            {
                return;
            }
            if (!viewAllowAccess)
            {
                lstDeniedUsers.Add(viewUserid);
                return;
            }
            if (lstUsers.Contains(viewUserid))
            {
                return;
            }
            lstUsers.Add(viewUserid);
        }

        public DateTime ConvertDateTimeToTZ(DateTime fromDateTime, int fromTZ, int toTZ)
        {
            return fromDateTime.AddMinutes(Convert.ToDouble(toTZ) - Convert.ToDouble(fromTZ));
        }

        public EventUser UserDisplayNameProfile(int prfUserID, string prfDisplayName, string inLocalResourceFile)
        {
            var returnValue = default(EventUser);
            var domainurl = this.GetDomainURL();
            returnValue = new EventUser();
            var with_1 = returnValue;
            with_1.UserID = prfUserID;
            with_1.DisplayName = prfDisplayName;
            if (with_1.DisplayName != "")
            {
                with_1.ProfileURL = HttpUtility.HtmlEncode(Globals.UserProfileURL(Convert.ToInt32(with_1.UserID)));
                if (!with_1.ProfileURL.ToLower().StartsWith("http://") &&
                    !with_1.ProfileURL.ToLower().StartsWith("https://"))
                {
                    with_1.ProfileURL = Globals.AddHTTP(domainurl) + with_1.ProfileURL;
                }
                with_1.DisplayNameURL = string.Format("<a href=\"{0}\"{2}>{1}</a>", with_1.ProfileURL,
                                                      with_1.DisplayName, " target=\"_blank\"");
            }
            else
            {
                with_1.ProfileURL = Globals.NavigateURL();
                with_1.DisplayName = Localization.GetString("UserDeleted", inLocalResourceFile);
                with_1.DisplayNameURL = with_1.DisplayName;
            }
            return returnValue;
        }

        public EventInfo ConvertEventToDisplayTimeZone(EventInfo moduleEvent, string displayTimeZoneId)
        {
            var with_1 = moduleEvent;
            var objEventTimeZoneUtilities = new EventTimeZoneUtilities();
            var eventDateInfo = default(EventTimeZoneUtilities.DateInfo);
            // Convert to display timezone based on timezone event was stored in
            eventDateInfo =
                objEventTimeZoneUtilities.ConvertToDisplayTimeZone(with_1.EventTimeBegin, with_1.EventTimeZoneId,
                                                                   with_1.PortalID, displayTimeZoneId);
            // If it is an all day event then no need to adjust
            if (!moduleEvent.AllDayEvent)
            {
                with_1.EventTimeBegin = eventDateInfo.EventDate;
            }
            eventDateInfo =
                objEventTimeZoneUtilities.ConvertToDisplayTimeZone(with_1.LastRecurrence, with_1.EventTimeZoneId,
                                                                   with_1.PortalID, displayTimeZoneId);
            // If it is an all day event then no need to adjust
            if (!moduleEvent.AllDayEvent)
            {
                with_1.LastRecurrence = eventDateInfo.EventDate;
            }
            // Store the new timezone so it can be used again if needed
            with_1.EventTimeZoneId = eventDateInfo.EventTimeZoneId;
            // Convert to display timezone. If times have not been converted before, then OtherTimeZoneId will conatin "UTC" since these date are stored to DB in UTC
            eventDateInfo =
                objEventTimeZoneUtilities.ConvertToDisplayTimeZone(with_1.CreatedDate, with_1.OtherTimeZoneId,
                                                                   with_1.PortalID, displayTimeZoneId);
            with_1.CreatedDate = eventDateInfo.EventDate;
            eventDateInfo =
                objEventTimeZoneUtilities.ConvertToDisplayTimeZone(with_1.LastUpdatedAt, with_1.OtherTimeZoneId,
                                                                   with_1.PortalID, displayTimeZoneId);
            with_1.LastUpdatedAt = eventDateInfo.EventDate;
            // Store the new timezone so it can be used again if needed
            with_1.OtherTimeZoneId = eventDateInfo.EventTimeZoneId;
            return moduleEvent;
        }

        public ArrayList ConvertEventListToDisplayTimeZone(ArrayList moduleEvents, string displayTimeZoneId)
        {
            var outEvents = new ArrayList();
            foreach (EventInfo objEvent in moduleEvents)
            {
                outEvents.Add(this.ConvertEventToDisplayTimeZone(objEvent, displayTimeZoneId));
            }
            return outEvents;
        }

        // DateTime Sort Class
        public class EventDateSort : IComparer
        {
            public int Compare(object x, object y)
            {
                var xEventInfo = (EventInfo) x;
                var yEventInfo = (EventInfo) y;

                var objEventTimeZoneUtilities = new EventTimeZoneUtilities();

                var result = 0;
                result = DateTime.Compare(
                    objEventTimeZoneUtilities.ConvertToUTCTimeZone(xEventInfo.EventTimeBegin,
                                                                   xEventInfo.EventTimeZoneId),
                    objEventTimeZoneUtilities.ConvertToUTCTimeZone(yEventInfo.EventTimeBegin,
                                                                   yEventInfo.EventTimeZoneId));
                if (result == 0)
                {
                    result = xEventInfo.EventID.CompareTo(yEventInfo.EventID);
                }

                return result;
            }
        }

        #endregion
    }

    #endregion

    #region EventController Class

    [DNNtc.UpgradeEventMessage("01.01.01,04.00.02,04.01.00,05.02.00")]
    [DNNtc.BusinessControllerClass]
    public class EventController : ISearchable, IUpgradeable
    {
        public void EventsDelete(int eventID, int moduleID, int contentItemID)
        {
            // Dim cntTaxonomy As New Content
            // cntTaxonomy.DeleteContentItem(ContentItemID)
            DataProvider.Instance().EventsDelete(eventID, moduleID);
        }

        public EventInfo EventsGet(int eventID, int moduleID)
        {
            var eventInfo =
                (EventInfo) CBO.FillObject(DataProvider.Instance().EventsGet(eventID, moduleID), typeof(EventInfo));
            if (!ReferenceEquals(eventInfo, null))
            {
                var objCtlMasterEvent = new EventMasterController();
                eventInfo.ModuleTitle = objCtlMasterEvent.GetModuleTitle(eventInfo.ModuleID);
            }
            return eventInfo;
        }

        public ArrayList EventsGetByRange(string moduleIDs, DateTime beginDate, DateTime endDate, string categoryIDs,
                                          string locationIDs, int socialGroupId, int socialUserId)
        {
            return CBO.FillCollection(
                DataProvider.Instance().EventsGetByRange(moduleIDs, beginDate, endDate, categoryIDs, locationIDs,
                                                         socialGroupId, socialUserId), typeof(EventInfo));
        }


        public EventInfo EventsSave(EventInfo objEvent, bool saveOnly, int tabID, bool updateContent)
        {
            // Dim cntTaxonomy As New Content
            // If UpdateContent Then
            // If Not objEvent.ContentItemID = Nothing And objEvent.ContentItemID <> 0 Then
            // If Not objEvent.Cancelled Then
            // cntTaxonomy.UpdateContentEvent(objEvent)
            // Else
            // cntTaxonomy.DeleteContentItem(objEvent.ContentItemID)
            // objEvent.ContentItemID = Nothing
            // End If
            // End If
            // End If

            if (objEvent.Cancelled && objEvent.JournalItem)
            {
                var cntJournal = new Journal();
                cntJournal.DeleteEvent(objEvent);
                objEvent.JournalItem = false;
            }

            var objEventOut = this.EventsSave(objEvent, saveOnly);

            // If UpdateContent And objEvent.EventID = -1 Then
            // If objEventOut.ContentItemID = 0 And Not SaveOnly And Not TabID = Nothing Then
            // objEventOut.ContentItemID = cntTaxonomy.CreateContentEvent(objEventOut, TabID).ContentItemId
            // EventsSave(objEventOut, SaveOnly)
            // End If
            // End If

            return objEventOut;
        }

        public EventInfo EventsSave(EventInfo objEvent, bool saveOnly)
        {
            return (EventInfo) CBO.FillObject(
                DataProvider.Instance().EventsSave(objEvent.PortalID, objEvent.EventID, objEvent.RecurMasterID,
                                                   objEvent.ModuleID, objEvent.EventTimeBegin, objEvent.Duration,
                                                   objEvent.EventName, objEvent.EventDesc, (int) objEvent.Importance,
                                                   objEvent.CreatedByID.ToString(), objEvent.Notify, objEvent.Approved,
                                                   objEvent.Signups, objEvent.MaxEnrollment, objEvent.EnrollRoleID,
                                                   objEvent.EnrollFee, objEvent.EnrollType, objEvent.PayPalAccount,
                                                   objEvent.Cancelled, objEvent.DetailPage, objEvent.DetailNewWin,
                                                   objEvent.DetailURL, objEvent.ImageURL, objEvent.ImageType,
                                                   objEvent.ImageWidth, objEvent.ImageHeight, objEvent.ImageDisplay,
                                                   objEvent.Location, objEvent.Category, objEvent.Reminder,
                                                   objEvent.SendReminder, objEvent.ReminderTime,
                                                   objEvent.ReminderTimeMeasurement, objEvent.ReminderFrom,
                                                   objEvent.SearchSubmitted, objEvent.CustomField1,
                                                   objEvent.CustomField2, objEvent.EnrollListView,
                                                   objEvent.DisplayEndDate, objEvent.AllDayEvent, objEvent.OwnerID,
                                                   objEvent.LastUpdatedID, objEvent.OriginalDateBegin,
                                                   objEvent.NewEventEmailSent, objEvent.AllowAnonEnroll,
                                                   objEvent.ContentItemID, objEvent.JournalItem, objEvent.Summary,
                                                   saveOnly), typeof(EventInfo));
        }

        public ArrayList EventsModerateEvents(int moduleID, int socialGroupId)
        {
            return CBO.FillCollection(DataProvider.Instance().EventsModerateEvents(moduleID, socialGroupId),
                                      typeof(EventInfo));
        }

        public int EventsTimeZoneCount(int moduleID)
        {
            // ReSharper disable RedundantCast
            return Convert.ToInt32(DataProvider.Instance().EventsTimeZoneCount(moduleID));
            // ReSharper restore RedundantCast
        }

        public void EventsUpgrade(string moduleVersion)
        {
            DataProvider.Instance().EventsUpgrade(moduleVersion);
        }

        public void EventsCleanupExpired(int portalId, int moduleId)
        {
            DataProvider.Instance().EventsCleanupExpired(portalId, moduleId);
        }

        public ArrayList EventsGetRecurrences(int recurMasterID, int moduleID)
        {
            return CBO.FillCollection(DataProvider.Instance().EventsGetRecurrences(recurMasterID, moduleID),
                                      typeof(EventInfo));
        }


        #region Optional Interfaces

        //*** Implement ISearchable
        public SearchItemInfoCollection GetSearchItems(ModuleInfo modInfo)
        {
            var settings = EventModuleSettings.GetEventModuleSettings(modInfo.ModuleID, null);

            var objPortals = new PortalController();
            var objPortal = default(PortalInfo);
            objPortal = objPortals.GetPortal(modInfo.PortalID);
            // Set Thread Culture for Portal Default Culture (for Dates/Times Formatting)
            var lang = objPortal.DefaultLanguage;
            Thread.CurrentThread.CurrentCulture = new CultureInfo(lang, false);
            var searchItemCollection = new SearchItemInfoCollection();

            try
            {
                if (settings.Eventsearch)
                {
                    // Get Date Recurrences from 6 months prior to Current Date and 1 year out
                    var objEventInfoHelper = new EventInfoHelper(modInfo.ModuleID, settings);
                    var categoryIDs = new ArrayList();
                    categoryIDs.Add("-1");
                    var locationIDs = new ArrayList();
                    locationIDs.Add("-1");
                    var lstEvents =
                        objEventInfoHelper.GetEvents(DateTime.UtcNow.AddMonths(-6), DateTime.UtcNow.AddYears(1), false,
                                                     categoryIDs, locationIDs, true, -1, -1);

                    var portalTimeZoneId =
                        PortalController.GetPortalSetting("TimeZone", modInfo.PortalID, string.Empty);
                    lstEvents = objEventInfoHelper.ConvertEventListToDisplayTimeZone(lstEvents, portalTimeZoneId);

                    foreach (EventInfo objEvent in lstEvents)
                    {
                        var searchItem = default(SearchItemInfo);
                        // Item Title
                        var strTitle =
                            HttpUtility.HtmlDecode(objEvent.ModuleTitle + ": " + objEvent.EventName + ", " +
                                                   objEvent.EventTimeBegin);
                        // Displayed Description
                        var strDescription =
                            HtmlUtils.Shorten(HtmlUtils.StripTags(HttpUtility.HtmlDecode(objEvent.EventDesc), false),
                                              255, "...");
                        // Search Items
                        var strContent =
                            HttpUtility.HtmlDecode(objEvent.ModuleTitle + " " + objEvent.EventName + " " +
                                                   objEvent.EventTimeBegin + " " + objEvent.EventDesc);
                        // Added to Link
                        var strGUID =
                            HttpUtility.HtmlDecode("ModuleID=" + objEvent.ModuleID + "&ItemID=" + objEvent.EventID +
                                                   "&mctl=EventDetails");
                        // Unique Item Key
                        var strUnique = "Event: " + objEvent.EventID + ", Date:" + objEvent.EventTimeBegin;

                        searchItem = new SearchItemInfo();
                        searchItem.Title = strTitle;
                        searchItem.PubDate = objEvent.LastUpdatedAt;
                        searchItem.Description = strDescription;
                        searchItem.Author = objEvent.LastUpdatedID;
                        searchItem.ModuleId = modInfo.ModuleID;
                        searchItem.SearchKey = strUnique;
                        searchItem.Content = strContent;
                        searchItem.GUID = strGUID;

                        searchItemCollection.Add(searchItem);
                    }
                }
                return searchItemCollection;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Public Function ExportModule(ByVal ModuleID As Integer) As String Implements Entities.Modules.IPortable.ExportModule
        //    Dim strXML As String = ""
        //    Dim objEventCtl As New EventController

        //    Dim arrEvents As ArrayList = objEventCtl.EventsGetByRange(ModuleID.ToString(), DateTime.Now, DateTime.Now.AddYears(1), "")
        //    If arrEvents.Count <> 0 Then
        //        strXML += "<events>"
        //        Dim objEvent As EventInfo
        //        For Each objEvent In arrEvents
        //            strXML += "<event>"
        //            strXML += "<description>" & XMLEncode(objEvent.EventDesc) & "</description>"
        //            strXML += "<datetime>" & XMLEncode(objEvent.EventTimeBegin.ToString) & "</datetime>"
        //            strXML += "<title>" & XMLEncode(objEvent.EventName) & "</title>"
        //            strXML += "<icon>" & XMLEncode(objEvent.IconFile) & "</icon>"
        //            strXML += "<occursevery>" & XMLEncode(objEvent.Every.ToString) & "</occursevery>"
        //            strXML += "<alttext>" & XMLEncode(objEvent.AltText) & "</alttext>"
        //            strXML += "<expires>" & XMLEncode(objEvent.ExpireDate.ToString) & "</expires>"
        //            strXML += "<maxWidth>" & XMLEncode(objEvent.MaxWidth.ToString) & "</maxWidth>"
        //            strXML += "<period>" & XMLEncode(objEvent.Period) & "</period>"
        //            strXML += "</event>"
        //        Next
        //        strXML += "</events>"
        //    End If

        //    Return strXML
        //End Function

        //Public Sub ImportModule(ByVal ModuleID As Integer, ByVal Content As String, ByVal Version As String, ByVal UserId As Integer) Implements Entities.Modules.IPortable.ImportModule
        //    Dim xmlEvent As XmlNode
        //    Dim xmlEvents As XmlNode = GetContent(Content, "events")
        //    For Each xmlEvent In xmlEvents.SelectNodes("event")
        //        Dim objEvent As New EventInfo
        //        objEvent.ModuleID = ModuleID
        //        objEvent.Description = xmlEvent.Item("description").InnerText
        //        objEvent.DateTime = Date.Parse(xmlEvent.Item("datetime").InnerText)
        //        objEvent.Title = xmlEvent.Item("title").InnerText
        //        objEvent.IconFile = ImportUrl(ModuleID, xmlEvent.Item("icon").InnerText)
        //        objEvent.Every = Integer.Parse(xmlEvent.Item("occursevery").InnerText)
        //        objEvent.AltText = xmlEvent.Item("alttext").InnerText
        //        objEvent.ExpireDate = Date.Parse(xmlEvent.Item("expires").InnerText)
        //        objEvent.MaxWidth = Integer.Parse(xmlEvent.Item("maxWidth").InnerText)
        //        objEvent.Period = xmlEvent.Item("period").InnerText
        //        objEvent.CreatedByUser = UserId.ToString
        //        AddEvent(objEvent)
        //    Next
        //End Sub

        public string UpgradeModule(string version)
        {
            var rtnMessage = "Events Module Updated: " + version;
            try
            {
                // Create Lists and Schedule - they should always exist
                this.CreateListsAndSchedule();

                //Lookup DesktopModuleID
                var objDesktopModule = default(DesktopModuleInfo);
                objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", 0);

                if (!ReferenceEquals(objDesktopModule, null))
                {
                    var objModuleDefinition = default(ModuleDefinitionInfo);
                    //Lookup ModuleDefID
                    objModuleDefinition =
                        ModuleDefinitionController.GetModuleDefinitionByFriendlyName(
                            "Events", objDesktopModule.DesktopModuleID);

                    if (!ReferenceEquals(objModuleDefinition, null))
                    {
                        var objModuleControlInfo = default(ModuleControlInfo);
                        //Lookup ModuleControlID
                        objModuleControlInfo =
                            ModuleControlController.GetModuleControlByControlKey(
                                "Import", objModuleDefinition.ModuleDefID);
                        if (!ReferenceEquals(objModuleControlInfo, null))
                        {
                            //Remove Import control key
                            ModuleControlController.DeleteModuleControl(objModuleControlInfo.ModuleControlID);
                        }
                        // ReSharper disable RedundantAssignment
                        objModuleControlInfo = null;
                        // ReSharper restore RedundantAssignment
                        objModuleControlInfo =
                            ModuleControlController.GetModuleControlByControlKey(
                                "TZUpdate", objModuleDefinition.ModuleDefID);
                        if (!ReferenceEquals(objModuleControlInfo, null))
                        {
                            //Remove TZUpdate control key
                            ModuleControlController.DeleteModuleControl(objModuleControlInfo.ModuleControlID);
                        }
                    }
                }

                if (version == "04.00.02")
                {
                    // Copy moderators from ModuleSettings to ModulePermissions
                    var objEventCtl = new EventController();
                    objEventCtl.EventsUpgrade(version);
                }

                if (version == "04.01.00")
                {
                    // Upgrade recurring events
                    var blAllOk = false;
                    blAllOk = this.UpgradeRecurringEvents();
                    this.EventsUpgrade(version);
                    if (blAllOk)
                    {
                        rtnMessage = "Events Module Updated: " + version + " --> All Events Upgraded";
                    }
                    else
                    {
                        rtnMessage = "Events Module Updated: " + version +
                                     " --> Not All Events Upgraded - Check database for errors";
                    }
                }

                if (version == "05.02.00")
                {
                    // ReSharper disable UnusedVariable
                    var result = this.ConvertEditPermissions();
                    // ReSharper restore UnusedVariable
                }
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);

                return "Events Module Updated - Exception: " + version + " - Message: " + ex.Message;
            }
            return rtnMessage;
        }

        public void CreateListsAndSchedule()
        {
            // Create schedule
            var objEventNotificationController = new EventNotificationController();
            objEventNotificationController.InstallEventSchedule();

            // Add TimeInterval List entries
            var ctlLists = new ListController();
            var colThreadStatus = ctlLists.GetListEntryInfoItems("TimeInterval");
            if (!colThreadStatus.Any())
            {
                this.AddLists();
            }
        }

        private bool UpgradeRecurringEvents()
        {
            var returnStr = true;

            var objPortals = new PortalController();
            var objPortal = default(PortalInfo);
            var objModules = new ModuleController();
            var objModule = default(ModuleInfo);

            var lstportals = objPortals.GetPortals();
            foreach (PortalInfo tempLoopVar_objPortal in lstportals)
            {
                objPortal = tempLoopVar_objPortal;
                var objDesktopModule = default(DesktopModuleInfo);
                objDesktopModule =
                    DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", objPortal.PortalID);
                var folderName = objDesktopModule.FolderName;
                var templateSourceDirectory = Globals.ApplicationPath;
                var localResourceFile = templateSourceDirectory + "/DesktopModules/" + folderName + "/" +
                                        Localization.LocalResourceDirectory + "/EventSettings.ascx.resx";

                var lstModules = objModules.GetModulesByDefinition(objPortal.PortalID, objDesktopModule.FriendlyName);
                foreach (ModuleInfo tempLoopVar_objModule in lstModules)
                {
                    objModule = tempLoopVar_objModule;
                    // This check for objModule = Nothing because of error in DNN 5.0.0 in GetModulesByDefinition
                    if (ReferenceEquals(objModule, null))
                    {
                        continue;
                    }
                    var settings = EventModuleSettings.GetEventModuleSettings(objModule.ModuleID, null);

                    var maxRecurrences = settings.Maxrecurrences;

                    if (!this.UpgradeRecurringEventModule(objModule.ModuleID, Convert.ToInt32(settings.RecurDummy),
                                                          maxRecurrences, localResourceFile))
                    {
                        returnStr = false;
                    }
                }
            }

            return returnStr;
        }

        public bool UpgradeRecurringEventModule(int moduleID, int recurMasterID, string maxRecurrences,
                                                string localResourceFile)
        {
            if (recurMasterID == 99999)
            {
                return true;
            }
            var objCtlEventRecurMaster = new EventRecurMasterController();
            var objCtlEvent = new EventController();
            var objEvent = default(EventInfo);
            var objEventNew = default(EventInfo);

            var lstEvents = default(ArrayList);
            lstEvents = objCtlEvent.EventsGetRecurrences(recurMasterID, moduleID);

            foreach (EventInfo tempLoopVar_objEvent in lstEvents)
            {
                objEvent = tempLoopVar_objEvent;
                var objEventRecurMaster = new EventRecurMasterInfo();
                objEventRecurMaster.RecurMasterID = -1;
                objEventRecurMaster.ModuleID = objEvent.ModuleID;
                objEventRecurMaster.PortalID = objEvent.PortalID;
                objEventRecurMaster.Dtstart = objEvent.EventTimeBegin;
                objEventRecurMaster.Duration = Convert.ToString(Convert.ToString(objEvent.Duration) + "M");
                // ReSharper disable VBWarnings::BC40008
                objEventRecurMaster.Until = objEvent.EventDateEnd;
                // ReSharper restore VBWarnings::BC40008
                objEventRecurMaster.EventName = objEvent.EventName;
                objEventRecurMaster.EventDesc = objEvent.EventDesc;
                objEventRecurMaster.Importance = (EventRecurMasterInfo.Priority) objEvent.Importance;
                objEventRecurMaster.Notify = objEvent.Notify;
                objEventRecurMaster.Approved = objEvent.Approved;
                objEventRecurMaster.Signups = objEvent.Signups;
                objEventRecurMaster.MaxEnrollment = objEvent.MaxEnrollment;
                objEventRecurMaster.EnrollRoleID = objEvent.EnrollRoleID;
                objEventRecurMaster.EnrollFee = objEvent.EnrollFee;
                objEventRecurMaster.EnrollType = objEvent.EnrollType;
                objEventRecurMaster.Enrolled = objEvent.Enrolled;
                objEventRecurMaster.PayPalAccount = objEvent.PayPalAccount;
                objEventRecurMaster.DetailPage = objEvent.DetailPage;
                objEventRecurMaster.DetailNewWin = objEvent.DetailNewWin;
                objEventRecurMaster.DetailURL = objEvent.DetailURL;
                objEventRecurMaster.ImageURL = objEvent.ImageURL;
                objEventRecurMaster.ImageType = objEvent.ImageType;
                objEventRecurMaster.ImageWidth = objEvent.ImageWidth;
                objEventRecurMaster.ImageHeight = objEvent.ImageHeight;
                objEventRecurMaster.ImageDisplay = objEvent.ImageDisplay;
                objEventRecurMaster.Location = objEvent.Location;
                objEventRecurMaster.Category = objEvent.Category;
                objEventRecurMaster.Reminder = objEvent.Reminder;
                objEventRecurMaster.SendReminder = objEvent.SendReminder;
                objEventRecurMaster.ReminderTime = objEvent.ReminderTime;
                objEventRecurMaster.ReminderTimeMeasurement = objEvent.ReminderTimeMeasurement;
                objEventRecurMaster.ReminderFrom = objEvent.ReminderFrom;
                objEventRecurMaster.CustomField1 = objEvent.CustomField1;
                objEventRecurMaster.CustomField2 = objEvent.CustomField2;
                objEventRecurMaster.EnrollListView = objEvent.EnrollListView;
                objEventRecurMaster.DisplayEndDate = objEvent.DisplayEndDate;
                objEventRecurMaster.AllDayEvent = objEvent.AllDayEvent;
                objEventRecurMaster.OwnerID = objEvent.OwnerID;
                objEventRecurMaster.CreatedByID = objEvent.CreatedByID;
                objEventRecurMaster.UpdatedByID = objEvent.CreatedByID;
                objEventRecurMaster.AllowAnonEnroll = false;
                objEventRecurMaster.ContentItemID = 0;
                objEventRecurMaster.SocialGroupID = 0;
                objEventRecurMaster.SocialUserID = 0;
                objEventRecurMaster.Summary = null;
                var objCtlUsers = new UserController();
                var objUserInfo = objCtlUsers.GetUser(objEvent.PortalID, objEvent.CreatedByID);
                if (!ReferenceEquals(objUserInfo, null))
                {
                    objEventRecurMaster.CultureName = objUserInfo.Profile.PreferredLocale;
                }
                if (ReferenceEquals(objUserInfo, null) || objEventRecurMaster.CultureName == "")
                {
                    var objCtlPortal = new PortalController();
                    var objPortalinfo = objCtlPortal.GetPortal(objEvent.PortalID);
                    objEventRecurMaster.CultureName = objPortalinfo.DefaultLanguage;
                }

                objEventRecurMaster.RRULE = this.CreateRRULE(objEvent, objEventRecurMaster.CultureName);
                var lstEventsNew = default(ArrayList);

                lstEventsNew =
                    objCtlEventRecurMaster.CreateEventRecurrences(objEventRecurMaster, objEvent.Duration,
                                                                  maxRecurrences);
                objEventRecurMaster = objCtlEventRecurMaster.EventsRecurMasterSave(objEventRecurMaster);

                // If no events generated, mark original as cancelled and link to new recurmaster - Non Destructive
                if (lstEventsNew.Count == 0)
                {
                    objEvent.Cancelled = true;
                    objEvent.RecurMasterID = objEventRecurMaster.RecurMasterID;
                    objCtlEvent.EventsSave(objEvent, true);
                }

                var i = 0;
                foreach (EventInfo tempLoopVar_objEventNew in lstEventsNew)
                {
                    objEventNew = tempLoopVar_objEventNew;
                    i++;
                    if (i == 1)
                    {
                        objEventNew.EventID = objEvent.EventID;
                    }
                    objEventNew.RecurMasterID = objEventRecurMaster.RecurMasterID;
                    objEventNew.Cancelled = objEvent.Cancelled;
                    objEventNew.SearchSubmitted = objEvent.SearchSubmitted;
                    // ReSharper disable RedundantAssignment
                    objEventNew = objCtlEvent.EventsSave(objEventNew, true);
                    // ReSharper restore RedundantAssignment
                }
            }
            lstEvents.Clear();
            lstEvents = objCtlEvent.EventsGetRecurrences(recurMasterID, moduleID);
            if (lstEvents.Count == 0)
            {
                objCtlEventRecurMaster.EventsRecurMasterDelete(recurMasterID, moduleID);

                var moduleController = new ModuleController();
                var moduleInfo = moduleController.GetModule(moduleID);

                var repository = new EventModuleSettingsRepository();
                var settings = EventModuleSettings.GetEventModuleSettings(moduleID, localResourceFile);
                settings.RecurDummy = "99999";
                repository.SaveSettings(moduleInfo, settings);
                return true;
            }
            return false;
        }

        private string CreateRRULE(EventInfo objEvent, string cultureName)
        {
            var rrule = "";
            var strWkst = "";
            var culture = new CultureInfo(cultureName, false);
            strWkst = "SU";
            if (culture.DateTimeFormat.FirstDayOfWeek != DayOfWeek.Sunday)
            {
                strWkst = "MO";
            }
            // ReSharper disable VBWarnings::BC40008
            switch (objEvent.RepeatType.Trim())
            {
                case "N":
                    rrule = "";
                    break;
                case "P1":
                    switch (objEvent.Period.Trim())
                    {
                        case "D":
                            rrule = "FREQ=DAILY";
                            break;
                        case "W":
                            rrule = "FREQ=WEEKLY;WKST=" + strWkst;
                            break;
                        case "M":
                            rrule = "FREQ=MONTHLY";
                            break;
                        case "Y":
                            rrule = "FREQ=YEARLY";
                            break;
                    }
                    rrule = rrule + ";INTERVAL=" + objEvent.Every;
                    break;
                case "W1":
                    rrule = "FREQ=WEEKLY;WKST=" + strWkst + ";INTERVAL=" + objEvent.Every + ";BYDAY=";
                    if (Convert.ToBoolean(objEvent.Period.Substring(0, 1)))
                    {
                        rrule = rrule + "SU,";
                    }
                    if (Convert.ToBoolean(objEvent.Period.Substring(1, 1)))
                    {
                        rrule = rrule + "MO,";
                    }
                    if (Convert.ToBoolean(objEvent.Period.Substring(2, 1)))
                    {
                        rrule = rrule + "TU,";
                    }
                    if (Convert.ToBoolean(objEvent.Period.Substring(3, 1)))
                    {
                        rrule = rrule + "WE,";
                    }
                    if (Convert.ToBoolean(objEvent.Period.Substring(4, 1)))
                    {
                        rrule = rrule + "TH,";
                    }
                    if (Convert.ToBoolean(objEvent.Period.Substring(5, 1)))
                    {
                        rrule = rrule + "FR,";
                    }
                    if (Convert.ToBoolean(objEvent.Period.Substring(6, 1)))
                    {
                        rrule = rrule + "SA,";
                    }
                    rrule = rrule.Substring(0, rrule.Length - 1);
                    break;
                case "M1":
                    rrule = "FREQ=MONTHLY;INTERVAL=1;BYDAY=";
                    var strWeek = "";
                    if (objEvent.Every < 5)
                    {
                        strWeek = "+" + Convert.ToString(objEvent.Every);
                    }
                    else
                    {
                        strWeek = "-1";
                    }
                    rrule = rrule + strWeek;

                    var strDay = "";
                    switch (objEvent.Period.Trim())
                    {
                        case "0":
                            strDay = "SU";
                            break;
                        case "1":
                            strDay = "MO";
                            break;
                        case "2":
                            strDay = "TU";
                            break;
                        case "3":
                            strDay = "WE";
                            break;
                        case "4":
                            strDay = "TH";
                            break;
                        case "5":
                            strDay = "FR";
                            break;
                        case "6":
                            strDay = "SA";
                            break;
                    }
                    rrule = rrule + strDay;
                    break;
                case "M2":
                    rrule = "FREQ=MONTHLY;INTERVAL=" + objEvent.Every + ";BYMONTHDAY=+" + objEvent.Period.Trim();
                    break;
                case "Y1":
                    var uiculture = Thread.CurrentThread.CurrentCulture;
                    var usculture = new CultureInfo("en-US", false);
                    Thread.CurrentThread.CurrentCulture = usculture;
                    var yearDate = Convert.ToDateTime(objEvent.Period);
                    // ReSharper restore VBWarnings::BC40008
                    Thread.CurrentThread.CurrentCulture = uiculture;
                    rrule = "FREQ=YEARLY;INTERVAL=1;BYMONTH=" + Convert.ToString(yearDate.Month) + ";BYMONTHDAY=+" +
                            Convert.ToString(yearDate.Day);
                    break;
            }
            return rrule;
        }

        private bool ConvertEditPermissions()
        {
            var returnStr = true;

            var objPortals = new PortalController();
            var objPortal = default(PortalInfo);
            var objModules = new ModuleController();
            var objModule = default(ModuleInfo);

            var lstportals = objPortals.GetPortals();
            foreach (PortalInfo tempLoopVar_objPortal in lstportals)
            {
                objPortal = tempLoopVar_objPortal;
                var objDesktopModule = default(DesktopModuleInfo);
                objDesktopModule =
                    DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", objPortal.PortalID);

                var lstModules = objModules.GetModulesByDefinition(objPortal.PortalID, objDesktopModule.FriendlyName);
                foreach (ModuleInfo tempLoopVar_objModule in lstModules)
                {
                    objModule = tempLoopVar_objModule;
                    // This check for objModule = Nothing because of error in DNN 5.0.0 in GetModulesByDefinition
                    if (ReferenceEquals(objModule, null))
                    {
                        continue;
                    }

                    if (!ConvertEditPermissionsModule(objModule.ModuleID, objModule.TabID))
                    {
                        returnStr = false;
                    }
                }
            }
            return returnStr;
        }

        private static bool ConvertEditPermissionsModule(int moduleID, int tabID)
        {
            var returnStr = false;
            var arrRoles = new ArrayList();
            var arrUsers = new ArrayList();

            var objPermission = default(ModulePermissionInfo);
            var objPermissionController = new PermissionController();

            var objModules = new ModuleController();
            // Get existing module permissions
            var objModule = objModules.GetModule(moduleID, tabID);
            var objModulePermissions2 = new ModulePermissionCollection();

            foreach (ModulePermissionInfo perm in objModule.ModulePermissions)
            {
                if (perm.PermissionKey == "EDIT" && perm.AllowAccess)
                {
                    objModulePermissions2.Add(perm);
                    if (perm.UserID >= 0)
                    {
                        arrUsers.Add(perm.UserID);
                    }
                    else
                    {
                        arrRoles.Add(perm.RoleID);
                    }
                }
            }
            foreach (ModulePermissionInfo perm in objModulePermissions2)
            {
                if (perm.RoleName != "Administrators")
                {
                    objModule.ModulePermissions.Remove(perm);
                }
            }

            var objEditPermissions = objPermissionController.GetPermissionByCodeAndKey("EVENTS_MODULE", "EVENTSEDT");
            var objEditPermission = (PermissionInfo) objEditPermissions[0];

            foreach (int iRoleID in arrRoles)
            {
                objPermission = new ModulePermissionInfo();
                objPermission.RoleID = iRoleID;
                objPermission.ModuleID = moduleID;
                objPermission.PermissionKey = objEditPermission.PermissionKey;
                objPermission.PermissionName = objEditPermission.PermissionName;
                objPermission.PermissionCode = objEditPermission.PermissionCode;
                objPermission.PermissionID = objEditPermission.PermissionID;
                objPermission.AllowAccess = true;
                objModule.ModulePermissions.Add(objPermission);
            }
            foreach (int iUserID in arrUsers)
            {
                objPermission = new ModulePermissionInfo();
                objPermission.UserID = iUserID;
                objPermission.ModuleID = moduleID;
                objPermission.PermissionKey = objEditPermission.PermissionKey;
                objPermission.PermissionName = objEditPermission.PermissionName;
                objPermission.PermissionCode = objEditPermission.PermissionCode;
                objPermission.PermissionID = objEditPermission.PermissionID;
                objPermission.AllowAccess = true;
                objModule.ModulePermissions.Add(objPermission);
            }

            ModulePermissionController.SaveModulePermissions(objModule);
            returnStr = true;
            return returnStr;
        }

        private void AddLists()
        {
            var objDesktopModule = default(DesktopModuleInfo);
            objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", 0);
            if (ReferenceEquals(objDesktopModule, null))
            {
                return;
            }
            var objModuleDefinition = default(ModuleDefinitionInfo);
            objModuleDefinition =
                ModuleDefinitionController
                    .GetModuleDefinitionByFriendlyName("Events", objDesktopModule.DesktopModuleID);
            if (ReferenceEquals(objModuleDefinition, null))
            {
                return;
            }

            var moduleDefId = objModuleDefinition.ModuleDefID;

            var ctlLists = new ListController();
            //description is missing, not needed

            //ThreadStatus
            var objList = new ListEntryInfo();
            objList.ListName = "TimeInterval";
            objList.Value = "5";
            objList.Text = "5";
            objList.SortOrder = 1;
            objList.ParentID = 0;
            objList.Level = 0;
            objList.DefinitionID = moduleDefId;
            ctlLists.AddListEntry(objList);

            objList.ListName = "TimeInterval";
            objList.Value = "10";
            objList.Text = "10";
            objList.SortOrder = 2;
            objList.ParentID = 0;
            objList.Level = 0;
            objList.DefinitionID = moduleDefId;
            ctlLists.AddListEntry(objList);

            objList.ListName = "TimeInterval";
            objList.Value = "15";
            objList.Text = "15";
            objList.SortOrder = 3;
            objList.ParentID = 0;
            objList.Level = 0;
            objList.DefinitionID = moduleDefId;
            ctlLists.AddListEntry(objList);

            objList.ListName = "TimeInterval";
            objList.Value = "20";
            objList.Text = "20";
            objList.SortOrder = 4;
            objList.ParentID = 0;
            objList.Level = 0;
            objList.DefinitionID = moduleDefId;
            ctlLists.AddListEntry(objList);

            objList.ListName = "TimeInterval";
            objList.Value = "30";
            objList.Text = "30";
            objList.SortOrder = 5;
            objList.ParentID = 0;
            objList.Level = 0;
            objList.DefinitionID = moduleDefId;
            ctlLists.AddListEntry(objList);

            objList.ListName = "TimeInterval";
            objList.Value = "60";
            objList.Text = "60";
            objList.SortOrder = 6;
            objList.ParentID = 0;
            objList.Level = 0;
            objList.DefinitionID = moduleDefId;
            ctlLists.AddListEntry(objList);


            objList.ListName = "TimeInterval";
            objList.Value = "120";
            objList.Text = "120";
            objList.SortOrder = 7;
            objList.ParentID = 0;
            objList.Level = 0;
            objList.DefinitionID = moduleDefId;
            ctlLists.AddListEntry(objList);


            objList.ListName = "TimeInterval";
            objList.Value = "240";
            objList.Text = "240";
            objList.SortOrder = 8;
            objList.ParentID = 0;
            objList.Level = 0;
            objList.DefinitionID = moduleDefId;
            ctlLists.AddListEntry(objList);


            objList.ListName = "TimeInterval";
            objList.Value = "360";
            objList.Text = "360";
            objList.SortOrder = 9;
            objList.ParentID = 0;
            objList.Level = 0;
            objList.DefinitionID = moduleDefId;
            ctlLists.AddListEntry(objList);

            objList.ListName = "TimeInterval";
            objList.Value = "720";
            objList.Text = "720";
            objList.SortOrder = 10;
            objList.ParentID = 0;
            objList.Level = 0;
            objList.DefinitionID = moduleDefId;
            ctlLists.AddListEntry(objList);

            objList.ListName = "TimeInterval";
            objList.Value = "1440";
            objList.Text = "1440";
            objList.SortOrder = 11;
            objList.ParentID = 0;
            objList.Level = 0;
            objList.DefinitionID = moduleDefId;
            ctlLists.AddListEntry(objList);
        }

        #endregion
    }

    #endregion

    #region EventMasterController Class

    public class EventMasterController
    {
        public void EventsMasterDelete(int masterID, int moduleID)
        {
            DataProvider.Instance().EventsMasterDelete(masterID, moduleID);
        }

        public EventMasterInfo EventsMasterGet(int moduleID, int subEventID)
        {
            var eventMasterInfo =
                (EventMasterInfo) CBO.FillObject(DataProvider.Instance().EventsMasterGet(moduleID, subEventID),
                                                 typeof(EventMasterInfo));
            if (!ReferenceEquals(eventMasterInfo, null))
            {
                eventMasterInfo.SubEventTitle = this.GetModuleTitle(eventMasterInfo.SubEventID);
            }
            return eventMasterInfo;
        }

        public ArrayList EventsMasterAssignedModules(int moduleID)
        {
            var assignedModules = CBO.FillCollection(DataProvider.Instance().EventsMasterAssignedModules(moduleID),
                                                     typeof(EventMasterInfo));
            var objEventMasterInfo = default(EventMasterInfo);
            foreach (EventMasterInfo tempLoopVar_objEventMasterInfo in assignedModules)
            {
                objEventMasterInfo = tempLoopVar_objEventMasterInfo;
                objEventMasterInfo.SubEventTitle = this.GetModuleTitle(objEventMasterInfo.SubEventID);
            }
            assignedModules.Sort(new ModuleListSort());
            return assignedModules;
        }

        public ArrayList EventsMasterAvailableModules(int portalID, int moduleID)
        {
            var availableModules =
                CBO.FillCollection(DataProvider.Instance().EventsMasterAvailableModules(portalID, moduleID),
                                   typeof(EventMasterInfo));
            var objEventMasterInfo = default(EventMasterInfo);
            foreach (EventMasterInfo tempLoopVar_objEventMasterInfo in availableModules)
            {
                objEventMasterInfo = tempLoopVar_objEventMasterInfo;
                objEventMasterInfo.SubEventTitle = this.GetModuleTitle(objEventMasterInfo.SubEventID);
            }
            availableModules.Sort(new ModuleListSort());
            return availableModules;
        }

        public EventMasterInfo EventsMasterSave(EventMasterInfo objEventMaster)
        {
            return (EventMasterInfo) CBO.FillObject(
                DataProvider.Instance().EventsMasterSave(objEventMaster.MasterID, objEventMaster.ModuleID,
                                                         objEventMaster.SubEventID), typeof(EventMasterInfo));
        }

        public string GetModuleTitle(int intModuleID)
        {
            var cacheKey = "EventsModuleTitle" + intModuleID;
            var moduleTitle = Convert.ToString(DataCache.GetCache(cacheKey));
            if (ReferenceEquals(moduleTitle, null))
            {
                var objModuleController = new ModuleController();
                var objModuleTabs = objModuleController.GetModuleTabs(intModuleID);
                var objModuleInfo = default(ModuleInfo);
                var intTabID = 0;
                foreach (ModuleInfo tempLoopVar_objModuleInfo in objModuleTabs)
                {
                    objModuleInfo = tempLoopVar_objModuleInfo;
                    if ((objModuleInfo.TabID < intTabID) | (intTabID == 0))
                    {
                        moduleTitle = HtmlUtils.StripTags(objModuleInfo.ModuleTitle, false);
                        intTabID = objModuleInfo.TabID;
                    }
                }
                DataCache.SetCache(cacheKey, moduleTitle);
            }
            return moduleTitle;
        }
    }

    #endregion

    #region Comparer Class

    public class ModuleListSort : IComparer
    {
        public int Compare(object x, object y)
        {
            var xdisplayname = "";
            var ydisplayname = "";

            xdisplayname = ((EventMasterInfo) x).SubEventTitle;
            ydisplayname = ((EventMasterInfo) y).SubEventTitle;
            var c = new CaseInsensitiveComparer();
            return c.Compare(xdisplayname, ydisplayname);
        }
    }

    #endregion


    #region EventSignupsController Class

    // EventSignupsController class made public in order to accommodate an external enrollment page.
    public class EventSignupsController
    {
        public void EventsSignupsDelete(int signupID, int moduleID)
        {
            DataProvider.Instance().EventsSignupsDelete(signupID, moduleID);
        }

        public EventSignupsInfo EventsSignupsGet(int signupID, int moduleID, bool ppipn)
        {
            return (EventSignupsInfo) CBO.FillObject(
                DataProvider.Instance().EventsSignupsGet(signupID, moduleID, ppipn), typeof(EventSignupsInfo));
        }

        public ArrayList EventsSignupsGetEvent(int eventID, int moduleID)
        {
            return CBO.FillCollection(DataProvider.Instance().EventsSignupsGetEvent(eventID, moduleID),
                                      typeof(EventSignupsInfo));
        }

        public ArrayList EventsSignupsGetEventRecurMaster(int recurMasterID, int moduleID)
        {
            return CBO.FillCollection(DataProvider.Instance().EventsSignupsGetEventRecurMaster(recurMasterID, moduleID),
                                      typeof(EventSignupsInfo));
        }

        public ArrayList EventsSignupsMyEnrollments(int moduleID, int userID, int socialGroupId, string categoryIDs,
                                                    DateTime beginDate, DateTime endDate)
        {
            return CBO.FillCollection(
                DataProvider
                    .Instance().EventsSignupsMyEnrollments(moduleID, userID, socialGroupId, categoryIDs, beginDate,
                                                           endDate), typeof(EventSignupsInfo));
        }

        public EventSignupsInfo EventsSignupsGetUser(int eventID, int userID, int moduleID)
        {
            return (EventSignupsInfo) CBO.FillObject(
                DataProvider.Instance().EventsSignupsGetUser(eventID, userID, moduleID), typeof(EventSignupsInfo));
        }

        public EventSignupsInfo EventsSignupsGetAnonUser(int eventID, string anonEmail, int moduleID)
        {
            return (EventSignupsInfo) CBO.FillObject(
                DataProvider.Instance().EventsSignupsGetAnonUser(eventID, anonEmail, moduleID),
                typeof(EventSignupsInfo));
        }

        public EventSignupsInfo EventsSignupsSave(EventSignupsInfo objEventSignup)
        {
            return (EventSignupsInfo) CBO.FillObject(
                DataProvider.Instance().EventsSignupsSave(objEventSignup.EventID, objEventSignup.SignupID,
                                                          objEventSignup.ModuleID, objEventSignup.UserID,
                                                          objEventSignup.Approved, objEventSignup.PayPalStatus,
                                                          objEventSignup.PayPalReason, objEventSignup.PayPalTransID,
                                                          objEventSignup.PayPalPayerID,
                                                          objEventSignup.PayPalPayerStatus,
                                                          objEventSignup.PayPalRecieverEmail,
                                                          objEventSignup.PayPalUserEmail,
                                                          objEventSignup.PayPalPayerEmail,
                                                          objEventSignup.PayPalFirstName, objEventSignup.PayPalLastName,
                                                          objEventSignup.PayPalAddress, objEventSignup.PayPalCity,
                                                          objEventSignup.PayPalState, objEventSignup.PayPalZip,
                                                          objEventSignup.PayPalCountry, objEventSignup.PayPalCurrency,
                                                          objEventSignup.PayPalPaymentDate, objEventSignup.PayPalAmount,
                                                          objEventSignup.PayPalFee, objEventSignup.NoEnrolees,
                                                          objEventSignup.AnonEmail, objEventSignup.AnonName,
                                                          objEventSignup.AnonTelephone, objEventSignup.AnonCulture,
                                                          objEventSignup.AnonTimeZoneId, objEventSignup.FirstName,
                                                          objEventSignup.LastName, objEventSignup.Company,
                                                          objEventSignup.JobTitle, objEventSignup.ReferenceNumber,
                                                          objEventSignup.Street, objEventSignup.PostalCode,
                                                          objEventSignup.City, objEventSignup.Region,
                                                          objEventSignup.Country), typeof(EventSignupsInfo));
        }

        public ArrayList EventsModerateSignups(int moduleID, int socialGroupId)
        {
            return CBO.FillCollection(DataProvider.Instance().EventsModerateSignups(moduleID, socialGroupId),
                                      typeof(EventSignupsInfo));
        }

        public bool DisplayEnrollIcon(EventInfo eventInfo)
        {
            var objEventTimeZoneUtilities = new EventTimeZoneUtilities();
            if (eventInfo.EventTimeBegin <
                objEventTimeZoneUtilities.ConvertFromUTCToModuleTimeZone(DateTime.UtcNow, eventInfo.EventTimeZoneId))
            {
                return false;
            }

            if (!eventInfo.Signups)
            {
                return false;
            }

            if (eventInfo.EnrollRoleID == null || eventInfo.EnrollRoleID == -1)
            {
                return true;
            }
            if (this.IsEnrollRole(eventInfo.EnrollRoleID, eventInfo.PortalID))
            {
                return true;
            }

            return false;
        }

        public bool IsEnrollRole(int roleID, int portalId)
        {
            try
            {
                // ReSharper disable RedundantAssignment
                var roleName = "";
                // ReSharper restore RedundantAssignment
                if (roleID != -1)
                {
                    var enrollRoleInfo = default(RoleInfo);
                    var enrollRoleCntrl = new RoleController();
                    enrollRoleInfo = enrollRoleCntrl.GetRole(roleID, portalId);
                    roleName = enrollRoleInfo.RoleName;
                    return PortalSecurity.IsInRole(roleName);
                }
            }
            catch
            { }
            return false;
        }
    }

    #endregion

    #region EventPPErrorLogController Class

    public class EventPpErrorLogController
    {
        public EventPpErrorLogInfo EventsPpErrorLogAdd(EventPpErrorLogInfo objEventPpErrorLog)
        {
            return (EventPpErrorLogInfo) CBO.FillObject(
                DataProvider.Instance().EventsPpErrorLogAdd(objEventPpErrorLog.SignupID,
                                                            objEventPpErrorLog.PayPalStatus,
                                                            objEventPpErrorLog.PayPalReason,
                                                            objEventPpErrorLog.PayPalTransID,
                                                            objEventPpErrorLog.PayPalPayerID,
                                                            objEventPpErrorLog.PayPalPayerStatus,
                                                            objEventPpErrorLog.PayPalRecieverEmail,
                                                            objEventPpErrorLog.PayPalUserEmail,
                                                            objEventPpErrorLog.PayPalPayerEmail,
                                                            objEventPpErrorLog.PayPalFirstName,
                                                            objEventPpErrorLog.PayPalLastName,
                                                            objEventPpErrorLog.PayPalAddress,
                                                            objEventPpErrorLog.PayPalCity,
                                                            objEventPpErrorLog.PayPalState,
                                                            objEventPpErrorLog.PayPalZip,
                                                            objEventPpErrorLog.PayPalCountry,
                                                            objEventPpErrorLog.PayPalCurrency,
                                                            objEventPpErrorLog.PayPalPaymentDate,
                                                            objEventPpErrorLog.PayPalAmount,
                                                            objEventPpErrorLog.PayPalFee), typeof(EventPpErrorLogInfo));
        }
    }

    #endregion

    #region EventCategoryController Class

    public class EventCategoryController
    {
        public void EventsCategoryDelete(int category, int portalID)
        {
            DataProvider.Instance().EventsCategoryDelete(category, portalID);
        }

        public EventCategoryInfo EventCategoryGet(int category, int portalID)
        {
            return (EventCategoryInfo) CBO.FillObject(DataProvider.Instance().EventsCategoryGet(category, portalID),
                                                      typeof(EventCategoryInfo));
        }

        public EventCategoryInfo EventCategoryGetByName(string categoryName, int portalID)
        {
            return (EventCategoryInfo) CBO.FillObject(
                DataProvider.Instance().EventsCategoryGetByName(categoryName, portalID), typeof(EventCategoryInfo));
        }

        public ArrayList EventsCategoryList(int portalID)
        {
            return CBO.FillCollection(DataProvider.Instance().EventsCategoryList(portalID), typeof(EventCategoryInfo));
        }

        public EventCategoryInfo EventsCategorySave(EventCategoryInfo objEventCategory)
        {
            return (EventCategoryInfo) CBO.FillObject(
                DataProvider.Instance().EventsCategorySave(objEventCategory.PortalID, objEventCategory.Category,
                                                           objEventCategory.CategoryName, objEventCategory.Color,
                                                           objEventCategory.FontColor), typeof(EventCategoryInfo));
        }
    }

    #endregion

    #region EventLocationController Class

    public class EventLocationController
    {
        public void EventsLocationDelete(int location, int portalID)
        {
            DataProvider.Instance().EventsLocationDelete(location, portalID);
        }

        public EventLocationInfo EventsLocationGet(int location, int portalID)
        {
            return (EventLocationInfo) CBO.FillObject(DataProvider.Instance().EventsLocationGet(location, portalID),
                                                      typeof(EventLocationInfo));
        }

        public EventLocationInfo EventsLocationGetByName(string locationName, int portalID)
        {
            return (EventLocationInfo) CBO.FillObject(
                DataProvider.Instance().EventsLocationGetByName(locationName, portalID), typeof(EventLocationInfo));
        }

        public ArrayList EventsLocationList(int portalID)
        {
            return CBO.FillCollection(DataProvider.Instance().EventsLocationList(portalID), typeof(EventLocationInfo));
        }

        public EventLocationInfo EventsLocationSave(EventLocationInfo objEventLocation)
        {
            return (EventLocationInfo) CBO.FillObject(
                DataProvider.Instance().EventsLocationSave(objEventLocation.PortalID, objEventLocation.Location,
                                                           objEventLocation.LocationName, objEventLocation.MapURL,
                                                           objEventLocation.Street, objEventLocation.PostalCode,
                                                           objEventLocation.City, objEventLocation.Region,
                                                           objEventLocation.Country), typeof(EventLocationInfo));
        }
    }

    #endregion

    #region EventNotificationController Class

    public class EventNotificationController
    {
        public void EventsNotificationTimeChange(int eventID, DateTime eventTimeBegin, int moduleID)
        {
            DataProvider.Instance().EventsNotificationTimeChange(eventID, eventTimeBegin, moduleID);
        }

        public void EventsNotificationDelete(DateTime deleteDate)
        {
            DataProvider.Instance().EventsNotificationDelete(deleteDate);
        }

        public EventNotificationInfo EventsNotificationGet(int eventID, string userEmail, int moduleID)
        {
            return (EventNotificationInfo) CBO.FillObject(
                DataProvider.Instance().EventsNotificationGet(eventID, userEmail, moduleID),
                typeof(EventNotificationInfo));
        }

        public ArrayList EventsNotificationsToSend(DateTime notifyTime)
        {
            return CBO.FillCollection(DataProvider.Instance().EventsNotificationsToSend(notifyTime),
                                      typeof(EventNotificationInfo));
        }

        public EventNotificationInfo EventsNotificationSave(EventNotificationInfo objEventNotification)
        {
            return (EventNotificationInfo) CBO.FillObject(
                DataProvider.Instance().EventsNotificationSave(objEventNotification.NotificationID,
                                                               objEventNotification.EventID,
                                                               objEventNotification.PortalAliasID,
                                                               objEventNotification.UserEmail,
                                                               objEventNotification.NotificationSent,
                                                               objEventNotification.NotifyByDateTime,
                                                               objEventNotification.EventTimeBegin,
                                                               objEventNotification.NotifyLanguage,
                                                               objEventNotification.ModuleID,
                                                               objEventNotification.TabID),
                typeof(EventNotificationInfo));
        }

        public string NotifyInfo(int itemID, string email, int moduleID, string localResourceFile,
                                 string displayTimeZoneId)
        {
            var notiinfo = "";
            var objEventNotification = default(EventNotificationInfo);
            objEventNotification = this.EventsNotificationGet(itemID, email, moduleID);
            if (!ReferenceEquals(objEventNotification, null))
            {
                var objEventTimeZoneUtilities = new EventTimeZoneUtilities();
                var notifyDisplay = objEventTimeZoneUtilities
                    .ConvertFromUTCToDisplayTimeZone(objEventNotification.NotifyByDateTime, displayTimeZoneId)
                    .EventDate;
                notiinfo = string.Format(Localization.GetString("lblReminderConfirmation", localResourceFile),
                                         notifyDisplay);
            }
            return notiinfo;
        }

        // Creates Event Schedule (if Not already installed)
        public void InstallEventSchedule()
        {
            var objScheduleItem = default(ScheduleItem);
            objScheduleItem = SchedulingProvider
                .Instance().GetSchedule("DotNetNuke.Modules.Events.EventNotification, DotNetNuke.Modules.Events", null);
            if (ReferenceEquals(objScheduleItem, null))
            {
                objScheduleItem = new ScheduleItem();
                objScheduleItem.TypeFullName = "DotNetNuke.Modules.Events.EventNotification, DotNetNuke.Modules.Events";
                objScheduleItem.TimeLapse = 1;
                objScheduleItem.TimeLapseMeasurement = "h";
                objScheduleItem.RetryTimeLapse = 30;
                objScheduleItem.RetryTimeLapseMeasurement = "m";
                objScheduleItem.RetainHistoryNum = 10;
                objScheduleItem.Enabled = true;
                objScheduleItem.ObjectDependencies = "";
                objScheduleItem.FriendlyName = "DNN Events";
                SchedulingProvider.Instance().AddSchedule(objScheduleItem);
            }
            else
            {
                objScheduleItem.FriendlyName = "DNN Events";
                SchedulingProvider.Instance().UpdateSchedule(objScheduleItem);
            }
        }
    }

    #endregion


    #region EventRecurMasterController Class

    public class EventRecurMasterController
    {
        public void EventsRecurMasterDelete(int recurMasterID, int moduleID)
        {
            // Delete ContentItems for the Events
            var objCtlEvent = new EventController();
            var lstEvents = objCtlEvent.EventsGetRecurrences(recurMasterID, moduleID);
            // Dim cntTaxonomy As New Content
            var cntJournal = new Journal();
            foreach (EventInfo objEvent in lstEvents)
            {
                // cntTaxonomy.DeleteContentItem(objEvent.ContentItemID)
                if (objEvent.JournalItem)
                {
                    cntJournal.DeleteEvent(objEvent);
                }
            }

            // Delete ContentItem for recurmaster
            // Dim objCtlEventRecurMaster As New EventRecurMasterController
            // Dim objEventRecurMaster As EventRecurMasterInfo = objCtlEventRecurMaster.EventsRecurMasterGet(RecurMasterID, ModuleID)
            // If Not objEventRecurMaster Is Nothing Then
            // cntTaxonomy.DeleteContentItem(objEventRecurMaster.ContentItemID)
            // End If

            // Delete recurmaster
            DataProvider.Instance().EventsRecurMasterDelete(recurMasterID, moduleID);
        }

        public EventRecurMasterInfo EventsRecurMasterGet(int recurMasterID, int moduleID)
        {
            return (EventRecurMasterInfo) CBO.FillObject(
                DataProvider
                    .Instance().EventsRecurMasterGet(recurMasterID, moduleID),
                typeof(EventRecurMasterInfo));
        }

        public EventRecurMasterInfo EventsRecurMasterSave(EventRecurMasterInfo objEventRecurMaster, int tabId,
                                                          bool updateContent)
        {
            // Dim cntTaxonomy As New Content
            // If UpdateContent Then
            // If Not objEventRecurMaster.ContentItemID = Nothing And objEventRecurMaster.ContentItemID <> 0 Then
            // cntTaxonomy.UpdateContentEventRecurMaster(objEventRecurMaster)
            // End If
            // End If

            var objEventRecurMasterOut = this.EventsRecurMasterSave(objEventRecurMaster);

            // If UpdateContent And objEventRecurMaster.RecurMasterID = -1 Then
            // If objEventRecurMasterOut.ContentItemID = 0 And Not TabId = Nothing Then
            // objEventRecurMasterOut.ContentItemID = cntTaxonomy.CreateContentEventRecurMaster(objEventRecurMasterOut, TabId).ContentItemId
            // EventsRecurMasterSave(objEventRecurMasterOut)
            // End If
            // End If

            return objEventRecurMasterOut;
        }


        public EventRecurMasterInfo EventsRecurMasterSave(EventRecurMasterInfo objEventRecurMaster)
        {
            return (EventRecurMasterInfo) CBO.FillObject(
                DataProvider
                    .Instance().EventsRecurMasterSave(
                        objEventRecurMaster.RecurMasterID,
                        objEventRecurMaster.ModuleID, objEventRecurMaster.PortalID,
                        objEventRecurMaster.RRULE, objEventRecurMaster.Dtstart,
                        objEventRecurMaster.Duration, objEventRecurMaster.Until,
                        objEventRecurMaster.EventName,
                        objEventRecurMaster.EventDesc,
                        (int) objEventRecurMaster.Importance,
                        objEventRecurMaster.Notify, objEventRecurMaster.Approved,
                        objEventRecurMaster.Signups,
                        objEventRecurMaster.MaxEnrollment,
                        objEventRecurMaster.EnrollRoleID,
                        objEventRecurMaster.EnrollFee,
                        objEventRecurMaster.EnrollType,
                        objEventRecurMaster.PayPalAccount,
                        objEventRecurMaster.DetailPage,
                        objEventRecurMaster.DetailNewWin,
                        objEventRecurMaster.DetailURL,
                        objEventRecurMaster.ImageURL,
                        objEventRecurMaster.ImageType,
                        objEventRecurMaster.ImageWidth,
                        objEventRecurMaster.ImageHeight,
                        objEventRecurMaster.ImageDisplay,
                        objEventRecurMaster.Location, objEventRecurMaster.Category,
                        objEventRecurMaster.Reminder,
                        objEventRecurMaster.SendReminder,
                        objEventRecurMaster.ReminderTime,
                        objEventRecurMaster.ReminderTimeMeasurement,
                        objEventRecurMaster.ReminderFrom,
                        objEventRecurMaster.CustomField1,
                        objEventRecurMaster.CustomField2,
                        objEventRecurMaster.EnrollListView,
                        objEventRecurMaster.DisplayEndDate,
                        objEventRecurMaster.AllDayEvent,
                        objEventRecurMaster.CultureName,
                        objEventRecurMaster.OwnerID,
                        objEventRecurMaster.CreatedByID,
                        objEventRecurMaster.UpdatedByID,
                        objEventRecurMaster.EventTimeZoneId,
                        objEventRecurMaster.AllowAnonEnroll,
                        objEventRecurMaster.ContentItemID,
                        objEventRecurMaster.SocialGroupID,
                        objEventRecurMaster.SocialUserID,
                        objEventRecurMaster.Summary),
                typeof(EventRecurMasterInfo));
        }

        public ArrayList EventsRecurMasterModerate(int moduleID, int socialGroupId)
        {
            return CBO.FillCollection(DataProvider.Instance().EventsRecurMasterModerate(moduleID, socialGroupId),
                                      typeof(EventRecurMasterInfo));
        }

        public EventRRULEInfo DecomposeRRULE(string strRRULE, DateTime dtStart)
        {
            var objEventRRULE = new EventRRULEInfo();
            var intEqual = 0;
            var intKeyEnd = 0;
            var strKey = "";
            var strValue = "";
            strRRULE = strRRULE + ";";
            objEventRRULE.FreqBasic = false;
            while (strRRULE.Length > 1)
            {
                intEqual = strRRULE.IndexOf("=") + 1;
                intKeyEnd = strRRULE.IndexOf(";") + 1;
                strKey = strRRULE.Substring(0, intEqual - 1);
                strValue = strRRULE.Substring(intEqual + 1 - 1, intKeyEnd - intEqual - 1);
                switch (strKey)
                {
                    case "FREQ":
                        objEventRRULE.Freq = strValue;
                        break;
                    case "WKST":
                        objEventRRULE.Wkst = strValue;
                        break;
                    case "INTERVAL":
                        objEventRRULE.Interval = int.Parse(strValue);
                        break;
                    case "BYDAY":
                        objEventRRULE.ByDay = strValue;
                        if (Information.IsNumeric(strValue.Substring(0, 2)))
                        {
                            if (strValue.IndexOf("SU") + 1 > 0)
                            {
                                objEventRRULE.Su = true;
                                objEventRRULE.SuNo = int.Parse(strValue.Substring(0, 2));
                            }
                            if (strValue.IndexOf("MO") + 1 > 0)
                            {
                                objEventRRULE.Mo = true;
                                objEventRRULE.MoNo = int.Parse(strValue.Substring(0, 2));
                            }
                            if (strValue.IndexOf("TU") + 1 > 0)
                            {
                                objEventRRULE.Tu = true;
                                objEventRRULE.TuNo = int.Parse(strValue.Substring(0, 2));
                            }
                            if (strValue.IndexOf("WE") + 1 > 0)
                            {
                                objEventRRULE.We = true;
                                objEventRRULE.WeNo = int.Parse(strValue.Substring(0, 2));
                            }
                            if (strValue.IndexOf("TH") + 1 > 0)
                            {
                                objEventRRULE.Th = true;
                                objEventRRULE.ThNo = int.Parse(strValue.Substring(0, 2));
                            }
                            if (strValue.IndexOf("FR") + 1 > 0)
                            {
                                objEventRRULE.Fr = true;
                                objEventRRULE.FrNo = int.Parse(strValue.Substring(0, 2));
                            }
                            if (strValue.IndexOf("SA") + 1 > 0)
                            {
                                objEventRRULE.Sa = true;
                                objEventRRULE.SaNo = int.Parse(strValue.Substring(0, 2));
                            }
                        }
                        else
                        {
                            if (strValue.IndexOf("SU") + 1 > 0)
                            {
                                objEventRRULE.Su = true;
                                objEventRRULE.SuNo = 0;
                            }
                            if (strValue.IndexOf("MO") + 1 > 0)
                            {
                                objEventRRULE.Mo = true;
                                objEventRRULE.MoNo = 0;
                            }
                            if (strValue.IndexOf("TU") + 1 > 0)
                            {
                                objEventRRULE.Tu = true;
                                objEventRRULE.TuNo = 0;
                            }
                            if (strValue.IndexOf("WE") + 1 > 0)
                            {
                                objEventRRULE.We = true;
                                objEventRRULE.WeNo = 0;
                            }
                            if (strValue.IndexOf("TH") + 1 > 0)
                            {
                                objEventRRULE.Th = true;
                                objEventRRULE.ThNo = 0;
                            }
                            if (strValue.IndexOf("FR") + 1 > 0)
                            {
                                objEventRRULE.Fr = true;
                                objEventRRULE.FrNo = 0;
                            }
                            if (strValue.IndexOf("SA") + 1 > 0)
                            {
                                objEventRRULE.Sa = true;
                                objEventRRULE.SaNo = 0;
                            }
                        }
                        break;
                    case "BYMONTH":
                        objEventRRULE.ByMonth = int.Parse(strValue);
                        break;
                    case "BYMONTHDAY":
                        objEventRRULE.ByMonthDay = int.Parse(strValue);
                        break;
                }
                strRRULE = strRRULE.Substring(intKeyEnd + 1 - 1);
            }
            switch (objEventRRULE.Freq)
            {
                case "YEARLY":
                    if ((objEventRRULE.ByMonth == 0) & (objEventRRULE.ByMonthDay == 0))
                    {
                        objEventRRULE.FreqBasic = true;
                    }
                    if (objEventRRULE.ByMonth == 0)
                    {
                        objEventRRULE.ByMonth = dtStart.Month;
                    }
                    if (objEventRRULE.ByMonthDay == 0)
                    {
                        objEventRRULE.ByMonthDay = dtStart.Day;
                    }
                    break;
                case "MONTHLY":
                    if (objEventRRULE.ByMonthDay == 0 && ReferenceEquals(objEventRRULE.ByDay, null))
                    {
                        objEventRRULE.ByMonthDay = dtStart.Day;
                        objEventRRULE.FreqBasic = true;
                    }
                    break;
                case "WEEKLY":
                    if (ReferenceEquals(objEventRRULE.ByDay, null))
                    {
                        objEventRRULE.FreqBasic = true;
                        var dtdow = dtStart.DayOfWeek;
                        switch (dtdow)
                        {
                            case DayOfWeek.Sunday:
                                objEventRRULE.Su = true;
                                objEventRRULE.SuNo = 0;
                                objEventRRULE.ByDay = "SU";
                                break;
                            case DayOfWeek.Monday:
                                objEventRRULE.Mo = true;
                                objEventRRULE.MoNo = 0;
                                objEventRRULE.ByDay = "MO";
                                break;
                            case DayOfWeek.Tuesday:
                                objEventRRULE.Tu = true;
                                objEventRRULE.TuNo = 0;
                                objEventRRULE.ByDay = "TU";
                                break;
                            case DayOfWeek.Wednesday:
                                objEventRRULE.We = true;
                                objEventRRULE.WeNo = 0;
                                objEventRRULE.ByDay = "WE";
                                break;
                            case DayOfWeek.Thursday:
                                objEventRRULE.Th = true;
                                objEventRRULE.ThNo = 0;
                                objEventRRULE.ByDay = "TH";
                                break;
                            case DayOfWeek.Friday:
                                objEventRRULE.Fr = true;
                                objEventRRULE.FrNo = 0;
                                objEventRRULE.ByDay = "FR";
                                break;
                            case DayOfWeek.Saturday:
                                objEventRRULE.Sa = true;
                                objEventRRULE.SaNo = 0;
                                objEventRRULE.ByDay = "SA";
                                break;
                        }
                    }
                    break;
            }
            return objEventRRULE;
        }

        public ArrayList CreateEventRecurrences(EventRecurMasterInfo objEventRecurMaster, int intDuration,
                                                string maxRecurrences)
        {
            var dtStart = objEventRecurMaster.Dtstart;
            var objCtlEventRecurMaster = new EventRecurMasterController();
            var objEventRRULE =
                objCtlEventRecurMaster.DecomposeRRULE(objEventRecurMaster.RRULE, objEventRecurMaster.Dtstart);
            var lstEvents = new ArrayList();
            var nextDate = dtStart.Date;
            var blAddDate = true;
            while (nextDate <= objEventRecurMaster.Until)
            {
                if ((objEventRRULE.ByMonth != 0) & (nextDate.Month != objEventRRULE.ByMonth))
                {
                    blAddDate = false;
                }
                if ((objEventRRULE.ByMonthDay != 0) & (nextDate.Day != objEventRRULE.ByMonthDay))
                {
                    blAddDate = false;
                }
                if (!ReferenceEquals(objEventRRULE.ByDay, null))
                {
                    var dtdow = nextDate.DayOfWeek;
                    switch (dtdow)
                    {
                        case DayOfWeek.Sunday:
                            blAddDate = this.CheckWeekday(nextDate, objEventRRULE.Su, objEventRRULE.SuNo, blAddDate);
                            break;
                        case DayOfWeek.Monday:
                            blAddDate = this.CheckWeekday(nextDate, objEventRRULE.Mo, objEventRRULE.MoNo, blAddDate);
                            break;
                        case DayOfWeek.Tuesday:
                            blAddDate = this.CheckWeekday(nextDate, objEventRRULE.Tu, objEventRRULE.TuNo, blAddDate);
                            break;
                        case DayOfWeek.Wednesday:
                            blAddDate = this.CheckWeekday(nextDate, objEventRRULE.We, objEventRRULE.WeNo, blAddDate);
                            break;
                        case DayOfWeek.Thursday:
                            blAddDate = this.CheckWeekday(nextDate, objEventRRULE.Th, objEventRRULE.ThNo, blAddDate);
                            break;
                        case DayOfWeek.Friday:
                            blAddDate = this.CheckWeekday(nextDate, objEventRRULE.Fr, objEventRRULE.FrNo, blAddDate);
                            break;
                        case DayOfWeek.Saturday:
                            blAddDate = this.CheckWeekday(nextDate, objEventRRULE.Sa, objEventRRULE.SaNo, blAddDate);
                            break;
                    }
                }

                if (blAddDate)
                {
                    switch (objEventRRULE.Freq)
                    {
                        case "YEARLY":
                            var intYear = (int) DateAndTime.DateDiff(DateInterval.Year, dtStart.Date, nextDate);
                            // ReSharper disable CompareOfFloatsByEqualityOperator
                            if ((double) intYear / objEventRRULE.Interval !=
                                Convert.ToInt32((double) intYear / objEventRRULE.Interval))
                            {
                                // ReSharper restore CompareOfFloatsByEqualityOperator
                                blAddDate = false;
                            }
                            break;
                        case "MONTHLY":
                            var intMonth = (int) DateAndTime.DateDiff(DateInterval.Month, dtStart.Date, nextDate);
                            // ReSharper disable CompareOfFloatsByEqualityOperator
                            if ((double) intMonth / objEventRRULE.Interval !=
                                Convert.ToInt32((double) intMonth / objEventRRULE.Interval))
                            {
                                // ReSharper restore CompareOfFloatsByEqualityOperator
                                blAddDate = false;
                            }
                            break;
                        case "WEEKLY":
                            var fdow = default(FirstDayOfWeek);
                            if (objEventRRULE.Wkst == "SU")
                            {
                                fdow = FirstDayOfWeek.Sunday;
                            }
                            else
                            {
                                fdow = FirstDayOfWeek.Monday;
                            }
                            var intWeek =
                                (int) DateAndTime.DateDiff(DateInterval.WeekOfYear, dtStart.Date, nextDate, fdow);
                            // ReSharper disable CompareOfFloatsByEqualityOperator
                            if ((double) intWeek / objEventRRULE.Interval !=
                                Convert.ToInt32((double) intWeek / objEventRRULE.Interval))
                            {
                                // ReSharper restore CompareOfFloatsByEqualityOperator
                                blAddDate = false;
                            }
                            break;
                        case "DAILY":
                            var intDay = (int) DateAndTime.DateDiff(DateInterval.Day, dtStart.Date, nextDate);
                            // ReSharper disable CompareOfFloatsByEqualityOperator
                            if ((double) intDay / objEventRRULE.Interval !=
                                Convert.ToInt32((double) intDay / objEventRRULE.Interval))
                            {
                                // ReSharper restore CompareOfFloatsByEqualityOperator
                                blAddDate = false;
                            }
                            break;
                    }
                }
                if (blAddDate)
                {
                    var objEvent = new EventInfo();
                    objEvent.EventTimeBegin = nextDate.Date + dtStart.TimeOfDay;
                    objEvent.Duration = intDuration;
                    objEvent.OriginalDateBegin = nextDate.Date;
                    var with_1 = objEventRecurMaster;
                    objEvent.EventID = -1;
                    objEvent.RecurMasterID = with_1.RecurMasterID;
                    objEvent.ModuleID = with_1.ModuleID;
                    objEvent.PortalID = with_1.PortalID;
                    objEvent.EventName = with_1.EventName;
                    objEvent.EventDesc = with_1.EventDesc;
                    objEvent.Importance = (EventInfo.Priority) with_1.Importance;
                    objEvent.Notify = with_1.Notify;
                    objEvent.Approved = with_1.Approved;
                    objEvent.Signups = with_1.Signups;
                    objEvent.AllowAnonEnroll = with_1.AllowAnonEnroll;
                    objEvent.JournalItem = with_1.JournalItem;
                    objEvent.MaxEnrollment = with_1.MaxEnrollment;
                    objEvent.EnrollRoleID = with_1.EnrollRoleID;
                    objEvent.EnrollFee = with_1.EnrollFee;
                    objEvent.EnrollType = with_1.EnrollType;
                    objEvent.PayPalAccount = with_1.PayPalAccount;
                    objEvent.DetailPage = with_1.DetailPage;
                    objEvent.DetailNewWin = with_1.DetailNewWin;
                    objEvent.DetailURL = with_1.DetailURL;
                    objEvent.ImageURL = with_1.ImageURL;
                    objEvent.ImageType = with_1.ImageType;
                    objEvent.ImageWidth = with_1.ImageWidth;
                    objEvent.ImageHeight = with_1.ImageHeight;
                    objEvent.ImageDisplay = with_1.ImageDisplay;
                    objEvent.Location = with_1.Location;
                    objEvent.Category = with_1.Category;
                    objEvent.Reminder = with_1.Reminder;
                    objEvent.SendReminder = with_1.SendReminder;
                    objEvent.ReminderTime = with_1.ReminderTime;
                    objEvent.ReminderTimeMeasurement = with_1.ReminderTimeMeasurement;
                    objEvent.ReminderFrom = with_1.ReminderFrom;
                    objEvent.CustomField1 = with_1.CustomField1;
                    objEvent.CustomField2 = with_1.CustomField2;
                    objEvent.EnrollListView = with_1.EnrollListView;
                    objEvent.DisplayEndDate = with_1.DisplayEndDate;
                    objEvent.AllDayEvent = with_1.AllDayEvent;
                    objEvent.OwnerID = with_1.OwnerID;
                    objEvent.CreatedByID = with_1.CreatedByID;
                    objEvent.LastUpdatedID = with_1.UpdatedByID;
                    objEvent.Cancelled = false;
                    objEvent.SearchSubmitted = false;
                    objEvent.NewEventEmailSent = objEvent.Approved;
                    objEvent.SocialGroupId = with_1.SocialGroupID;
                    objEvent.SocialUserId = with_1.SocialUserID;
                    objEvent.Summary = with_1.Summary;
                    objEvent.UpdateStatus = "Add";
                    lstEvents.Add(objEvent);
                    if (maxRecurrences != "")
                    {
                        if (lstEvents.Count == int.Parse(maxRecurrences))
                        {
                            objEventRecurMaster.Until = objEvent.EventTimeBegin.Date;
                            break;
                        }
                    }
                }
                nextDate = DateAndTime.DateAdd(DateInterval.Day, 1, nextDate);
                blAddDate = true;
            }
            return lstEvents;
        }

        private bool CheckWeekday(DateTime dtDate, bool blDay, int intDayNo, bool blAddDate)
        {
            if (blAddDate == false)
            {
                return false;
            }

            if (!blDay)
            {
                return false;
            }

            if (ReferenceEquals(intDayNo, null) || intDayNo == 0)
            {
                return true;
            }

            if (intDayNo > 0)
            {
                // ReSharper disable CompareOfFloatsByEqualityOperator
                if (Conversion.Int((double) (dtDate.Day - 1) / 7) == intDayNo - 1)
                {
                    // ReSharper restore CompareOfFloatsByEqualityOperator
                    return true;
                }
            }
            else
            {
                var intDaysInMonth = 0;
                intDaysInMonth = DateTime.DaysInMonth(dtDate.Year, dtDate.Month);
                // ReSharper disable CompareOfFloatsByEqualityOperator
                if (Conversion.Int((double) (intDaysInMonth - dtDate.Day) / 7) == intDayNo * -1 - 1)
                {
                    // ReSharper restore CompareOfFloatsByEqualityOperator
                    return true;
                }
            }

            return false;
        }

        public string RepeatType(EventRRULEInfo objEventRRULE)
        {
            var strRepeatType = "N";
            switch (objEventRRULE.Freq)
            {
                case "DAILY":
                    strRepeatType = "P1";
                    break;
                case "WEEKLY":
                    if (objEventRRULE.FreqBasic)
                    {
                        strRepeatType = "P1";
                    }
                    else
                    {
                        strRepeatType = "W1";
                    }
                    break;
                case "MONTHLY":
                    if (objEventRRULE.FreqBasic)
                    {
                        strRepeatType = "P1";
                    }
                    else if (!ReferenceEquals(objEventRRULE.ByDay, null))
                    {
                        strRepeatType = "M1";
                    }
                    else
                    {
                        strRepeatType = "M2";
                    }
                    break;
                case "YEARLY":
                    if (objEventRRULE.FreqBasic)
                    {
                        strRepeatType = "P1";
                    }
                    else
                    {
                        strRepeatType = "Y1";
                    }
                    break;
            }
            return strRepeatType;
        }

        public string RecurrenceText(EventRRULEInfo objEventRRULE, string localResourceFile, CultureInfo culture,
                                     DateTime eventDateBegin)
        {
            var lblEventText = "";
            var strRepeatType = this.RepeatType(objEventRRULE);
            switch (strRepeatType)
            {
                case "N":
                    lblEventText = Localization.GetString("OneTimeEvent", localResourceFile);
                    break;
                case "P1":
                    var txtEvent = "";
                    switch (objEventRRULE.Freq.Substring(0, 1).Trim())
                    {
                        case "D":
                            txtEvent = Localization.GetString("EveryXDays", localResourceFile);
                            break;
                        case "W":
                            txtEvent = Localization.GetString("EveryXWeeks", localResourceFile);
                            break;
                        case "M":
                            txtEvent = Localization.GetString("EveryXMonths", localResourceFile);
                            break;
                        case "Y":
                            txtEvent = Localization.GetString("EveryXYears", localResourceFile);
                            break;
                    }
                    lblEventText = string.Format(txtEvent, objEventRRULE.Interval);
                    break;
                case "W1":
                    var txtdays = "";
                    if (objEventRRULE.Su)
                    {
                        txtdays += culture.DateTimeFormat.DayNames[(int) DayOfWeek.Sunday];
                    }
                    if (objEventRRULE.Mo)
                    {
                        if (txtdays.Trim().Length > 0)
                        {
                            txtdays += ", ";
                        }
                        txtdays += culture.DateTimeFormat.DayNames[(int) DayOfWeek.Monday];
                    }
                    if (objEventRRULE.Tu)
                    {
                        if (txtdays.Trim().Length > 0)
                        {
                            txtdays += ", ";
                        }
                        txtdays += culture.DateTimeFormat.DayNames[(int) DayOfWeek.Tuesday];
                    }
                    if (objEventRRULE.We)
                    {
                        if (txtdays.Trim().Length > 0)
                        {
                            txtdays += ", ";
                        }
                        txtdays += culture.DateTimeFormat.DayNames[(int) DayOfWeek.Wednesday];
                    }
                    if (objEventRRULE.Th)
                    {
                        if (txtdays.Trim().Length > 0)
                        {
                            txtdays += ", ";
                        }
                        txtdays += culture.DateTimeFormat.DayNames[(int) DayOfWeek.Thursday];
                    }
                    if (objEventRRULE.Fr)
                    {
                        if (txtdays.Trim().Length > 0)
                        {
                            txtdays += ", ";
                        }
                        txtdays += culture.DateTimeFormat.DayNames[(int) DayOfWeek.Friday];
                    }
                    if (objEventRRULE.Sa)
                    {
                        if (txtdays.Trim().Length > 0)
                        {
                            txtdays += ", ";
                        }
                        txtdays += culture.DateTimeFormat.DayNames[(int) DayOfWeek.Saturday];
                    }
                    lblEventText = string.Format(Localization.GetString("RecurringWeeksOn", localResourceFile),
                                                 objEventRRULE.Interval, txtdays);
                    break;
                case "M1":
                    var txtDay = "";
                    var txtWeek = "";
                    var intEvery = 0;
                    if (objEventRRULE.Su)
                    {
                        txtDay = culture.DateTimeFormat.DayNames[(int) DayOfWeek.Sunday];
                        intEvery = objEventRRULE.SuNo;
                    }
                    if (objEventRRULE.Mo)
                    {
                        txtDay = culture.DateTimeFormat.DayNames[(int) DayOfWeek.Monday];
                        intEvery = objEventRRULE.MoNo;
                    }
                    if (objEventRRULE.Tu)
                    {
                        txtDay = culture.DateTimeFormat.DayNames[(int) DayOfWeek.Tuesday];
                        intEvery = objEventRRULE.TuNo;
                    }
                    if (objEventRRULE.We)
                    {
                        txtDay = culture.DateTimeFormat.DayNames[(int) DayOfWeek.Wednesday];
                        intEvery = objEventRRULE.WeNo;
                    }
                    if (objEventRRULE.Th)
                    {
                        txtDay = culture.DateTimeFormat.DayNames[(int) DayOfWeek.Thursday];
                        intEvery = objEventRRULE.ThNo;
                    }
                    if (objEventRRULE.Fr)
                    {
                        txtDay = culture.DateTimeFormat.DayNames[(int) DayOfWeek.Friday];
                        intEvery = objEventRRULE.FrNo;
                    }
                    if (objEventRRULE.Sa)
                    {
                        txtDay = culture.DateTimeFormat.DayNames[(int) DayOfWeek.Saturday];
                        intEvery = objEventRRULE.SaNo;
                    }
                    switch (Strings.Trim(Convert.ToString(intEvery)))
                    {
                        case "1":
                            txtWeek = Localization.GetString("First", localResourceFile);
                            break;
                        case "2":
                            txtWeek = Localization.GetString("Second", localResourceFile);
                            break;
                        case "3":
                            txtWeek = Localization.GetString("Third", localResourceFile);
                            break;
                        case "4":
                            txtWeek = Localization.GetString("Fourth", localResourceFile);
                            break;
                        case "-1":
                            txtWeek = Localization.GetString("Last", localResourceFile);
                            break;
                    }
                    if (objEventRRULE.Interval == 1)
                    {
                        lblEventText = string.Format(Localization.GetString("RecurringInMonth", localResourceFile),
                                                     txtWeek, txtDay);
                    }
                    else
                    {
                        lblEventText = string.Format(Localization.GetString("RecurringEveryMonth2", localResourceFile),
                                                     objEventRRULE.Interval.ToString().Trim(), txtWeek, txtDay);
                    }
                    break;
                case "M2":
                    lblEventText = string.Format(Localization.GetString("RecurringEveryMonth", localResourceFile),
                                                 objEventRRULE.Interval.ToString().Trim(),
                                                 Localization.GetString(
                                                     Convert.ToString(objEventRRULE.ByMonthDay),
                                                     localResourceFile));
                    break;
                case "Y1":
                    lblEventText = string.Format(Localization.GetString("RecurringYearsOn", localResourceFile),
                                                 eventDateBegin);
                    break;
            }
            return lblEventText;
        }

        public string RecurrenceInfo(EventInfo objEvent, string localResourceFile)
        {
            var recinfo = "";
            recinfo = string.Format(Localization.GetString("RecurringUntil", localResourceFile),
                                    objEvent.LastRecurrence.ToString("d"), objEvent.NoOfRecurrences);
            return recinfo;
        }
    }

    #endregion

    #region EventSubscriptionController Class

    public class EventSubscriptionController
    {
        public void EventsSubscriptionDeleteUser(int userID, int moduleID)
        {
            DataProvider.Instance().EventsSubscriptionDeleteUser(userID, moduleID);
        }

        public EventSubscriptionInfo EventsSubscriptionGetUser(int userID, int moduleID)
        {
            return (EventSubscriptionInfo) CBO.FillObject(
                DataProvider.Instance().EventsSubscriptionGetUser(userID, moduleID), typeof(EventSubscriptionInfo));
        }

        public ArrayList EventsSubscriptionGetModule(int moduleID)
        {
            return CBO.FillCollection(DataProvider.Instance().EventsSubscriptionGetModule(moduleID),
                                      typeof(EventSubscriptionInfo));
        }

        public ArrayList EventsSubscriptionGetSubModule(int moduleID)
        {
            return CBO.FillCollection(DataProvider.Instance().EventsSubscriptionGetSubModule(moduleID),
                                      typeof(EventSubscriptionInfo));
        }

        public EventSubscriptionInfo EventsSubscriptionSave(EventSubscriptionInfo objEventSubscription)
        {
            return (EventSubscriptionInfo) CBO.FillObject(
                DataProvider
                    .Instance().EventsSubscriptionSave(objEventSubscription.SubscriptionID,
                                                       objEventSubscription.ModuleID, objEventSubscription.PortalID,
                                                       objEventSubscription.UserID), typeof(EventSubscriptionInfo));
        }
    }

    #endregion

    #region EventEmails Class

    public class EventEmails
    {
        #region Member Variables

        private CultureInfo _currculture;

        #endregion

        #region Constructor

        public EventEmails(int portalID, int moduleID, string localResourceFile)
        {
            this.PortalID = portalID;
            this.ModuleID = moduleID;
            this.LocalResourceFile = localResourceFile;
        }

        public EventEmails(int portalID, int moduleID, string localResourceFile, string cultureName)
        {
            this.PortalID = portalID;
            this.ModuleID = moduleID;
            this.LocalResourceFile = localResourceFile;
            this.CultureName = cultureName;
        }

        #endregion

        #region Properties

        private int PortalID { get; }

        private int ModuleID { get; }

        private string LocalResourceFile { get; }

        private string CultureName { get; }

        #endregion


        #region Methods

        public void SendEmails(EventEmailInfo objEventEmailInfo, EventInfo objEvent)
        {
            this.SendEmails(objEventEmailInfo, objEvent, null, null, null, true);
        }

        public void SendEmails(EventEmailInfo objEventEmailInfo, EventInfo objEvent, EventSignupsInfo objEventSignups)
        {
            this.SendEmails(objEventEmailInfo, objEvent, objEventSignups, null, null, true);
        }

        public void SendEmails(EventEmailInfo objEventEmailInfo, EventInfo objEvent, EventSignupsInfo objEventSignups,
                               bool blTokenReplace)
        {
            this.SendEmails(objEventEmailInfo, objEvent, objEventSignups, null, null, blTokenReplace);
        }

        public void SendEmails(EventEmailInfo objEventEmailInfo, EventInfo objEvent, List<Attachment> attachments)
        {
            this.SendEmails(objEventEmailInfo, objEvent, null, attachments, null, true);
        }

        public void SendEmails(EventEmailInfo objEventEmailInfo, EventInfo objEvent, string domainurl)
        {
            this.SendEmails(objEventEmailInfo, objEvent, null, null, domainurl, true);
        }

        public void SendEmails(EventEmailInfo objEventEmailInfo, EventInfo objEvent, EventSignupsInfo objEventSignups,
                               List<Attachment> attachments, string domainurl, bool blTokenReplace)
        {
            this._currculture = Thread.CurrentThread.CurrentCulture;

            var with_1 = objEventEmailInfo;
            var userEmail = "";
            var itemNo = 0;

            var settings = EventModuleSettings.GetEventModuleSettings(this.ModuleID, this.LocalResourceFile);
            var objEventBase = new EventBase();
            var displayTimeZoneId = objEventBase.GetDisplayTimeZoneId(settings, objEvent.PortalID, "User");

            foreach (string tempLoopVar_userEmail in with_1.UserEmails)
            {
                userEmail = tempLoopVar_userEmail;
                var usedTimeZoneId = displayTimeZoneId;
                if (displayTimeZoneId == "User")
                {
                    usedTimeZoneId = with_1.UserTimeZoneIds[itemNo].ToString();
                }
                this.SendSingleEmail(userEmail, with_1.UserLocales[itemNo], objEvent, with_1.TxtEmailSubject,
                                     with_1.TxtEmailBody, with_1.TxtEmailFrom, objEventSignups, attachments, domainurl,
                                     usedTimeZoneId, blTokenReplace);
                itemNo++;
            }

            var userID = 0;
            foreach (int tempLoopVar_userID in with_1.UserIDs)
            {
                userID = tempLoopVar_userID;
                var objCtlUser = new UserController();
                var objUser = objCtlUser.GetUser(this.PortalID, userID);

                if (!ReferenceEquals(objUser, null))
                {
                    var usedTimeZoneId = displayTimeZoneId;
                    if (displayTimeZoneId == "User")
                    {
                        usedTimeZoneId = objUser.Profile.PreferredTimeZone.Id;
                    }
                    this.SendSingleEmail(objUser.Email, objUser.Profile.PreferredLocale, objEvent,
                                         with_1.TxtEmailSubject, with_1.TxtEmailBody, with_1.TxtEmailFrom,
                                         objEventSignups, attachments, domainurl, usedTimeZoneId, blTokenReplace);
                }
            }

            Thread.CurrentThread.CurrentCulture = this._currculture;
        }

        private void SendSingleEmail(string userEmail, object userLocale, EventInfo objEvent, string txtEmailSubject,
                                     string txtEmailBody, string txtEmailFrom, EventSignupsInfo objEventSignups,
                                     List<Attachment> attachments, string domainurl, string displayTimeZoneId,
                                     bool blTokenReplace)
        {
            var tcc = default(TokenReplaceControllerClass);
            var subject = "";
            var body = "";
            // ReSharper disable RedundantAssignment
            var bodyformat = "html";
            // ReSharper restore RedundantAssignment
            this.ChangeLocale(userLocale);

            var objEventInfoHelper = new EventInfoHelper(this.ModuleID, null);
            objEvent = objEventInfoHelper.ConvertEventToDisplayTimeZone(objEvent.Clone(), displayTimeZoneId);
            var objUsedSignup = new EventSignupsInfo();
            if (!ReferenceEquals(objEventSignups, null))
            {
                objUsedSignup = objEventSignups.Clone();
                var objEventTimeZoneUtilities = new EventTimeZoneUtilities();
                objUsedSignup.PayPalPaymentDate = objEventTimeZoneUtilities
                    .ConvertFromUTCToDisplayTimeZone(objUsedSignup.PayPalPaymentDate, displayTimeZoneId).EventDate;
            }

            if (blTokenReplace)
            {
                tcc = new TokenReplaceControllerClass(this.ModuleID, this.LocalResourceFile);
                subject = tcc.TokenReplaceEvent(objEvent, txtEmailSubject, objUsedSignup);
                body = tcc.TokenReplaceEvent(objEvent, txtEmailBody, objUsedSignup);
            }
            else
            {
                subject = txtEmailSubject;
                body = txtEmailBody;
            }
            body = this.AddHost(body, domainurl);
            bodyformat = this.GetBodyFormat(body);
            if (bodyformat == "text")
            {
                body = HtmlUtils.StripTags(body, true);
            }
            if (!ReferenceEquals(attachments, null))
            {
                var smtpEnableSsl = false;
                if (HostController.Instance.GetString("SMTPEnableSSL") == "Y")
                {
                    smtpEnableSsl = true;
                }
                Mail.SendMail(txtEmailFrom, userEmail, "", "", "", MailPriority.Normal, subject, MailFormat.Html,
                              Encoding.UTF8, body, attachments, "", "", "", "", smtpEnableSsl);
            }
            else
            {
                Mail.SendMail(txtEmailFrom, userEmail, "", subject, body, "", bodyformat, "", "", "", "");
            }
        }

        private string GetBodyFormat(string body)
        {
            var bodyformat = "";
            var settings = EventModuleSettings.GetEventModuleSettings(this.ModuleID, this.LocalResourceFile);

            bodyformat = settings.HTMLEmail;
            if (bodyformat == "auto")
            {
                if (HtmlUtils.IsHtml(body))
                {
                    bodyformat = "html";
                }
                else
                {
                    bodyformat = "text";
                }
            }
            return bodyformat;
        }

        private void ChangeLocale(object userLocale)
        {
            var strLocale = "";
            Thread.CurrentThread.CurrentCulture = this._currculture;
            if (!ReferenceEquals(userLocale, null))
            {
                strLocale = userLocale.ToString();
            }
            if (!ReferenceEquals(userLocale, null) && !string.IsNullOrEmpty(strLocale))
            {
                var userculture = new CultureInfo(strLocale, false);
                Thread.CurrentThread.CurrentCulture = userculture;
            }
            else if (!(this.CultureName == null))
            {
                var userculture = new CultureInfo(this.CultureName, false);
                Thread.CurrentThread.CurrentCulture = userculture;
            }
        }

        private string AddHost(string content, string domainurl)
        {
            if (ReferenceEquals(domainurl, null))
            {
                var objEventInfoHelper = new EventInfoHelper();
                domainurl = objEventInfoHelper.GetDomainURL();
            }
            domainurl = Globals.AddHTTP(domainurl);

            var txtContent = content;
            if (domainurl != "")
            {
                txtContent = Strings.Replace(txtContent, "src=\"/", "src=\"" + domainurl + "/",
                                             Compare: CompareMethod.Text);
                txtContent = Strings.Replace(txtContent, "href=\"/", "href=\"" + domainurl + "/",
                                             Compare: CompareMethod.Text);
            }
            return txtContent;
        }

        #endregion
    }

    #endregion

    #region TimeZone Utilities Class

    public class EventTimeZoneUtilities
    {
        public DateInfo ConvertFromUTCToDisplayTimeZone(DateTime utcDate, string displayTimeZoneId)
        {
            var displayInfo = new DateInfo();
            displayInfo.EventDate = utcDate;
            displayInfo.EventTimeZoneId = "UTC";
            try
            {
                var displayTimeZone = TimeZoneInfo.FindSystemTimeZoneById(displayTimeZoneId);
                displayInfo.EventTimeZoneId = displayTimeZone.Id;
                displayInfo.EventDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, displayTimeZone);
            }
            catch (Exception)
            {
                return displayInfo;
            }
            return displayInfo;
        }

        public DateInfo ConvertToDisplayTimeZone(DateTime inDate, string inTimeZoneId, int portalId,
                                                 string displayTimeZoneId)
        {
            var displayDateInfo = new DateInfo();
            if (inTimeZoneId == displayTimeZoneId)
            {
                displayDateInfo.EventDate = inDate;
                displayDateInfo.EventTimeZoneId = inTimeZoneId;
                return displayDateInfo;
            }
            var utcDate = inDate;
            if (inTimeZoneId != "UTC")
            {
                utcDate = this.ConvertToUTCTimeZone(inDate, inTimeZoneId);
            }

            displayDateInfo.EventDate = utcDate;
            displayDateInfo.EventTimeZoneId = displayTimeZoneId;
            if (displayTimeZoneId != "UTC")
            {
                displayDateInfo = this.ConvertFromUTCToDisplayTimeZone(utcDate, displayTimeZoneId);
            }
            return displayDateInfo;
        }

        public DateTime ConvertToUTCTimeZone(DateTime inDate, string inTimeZoneId)
        {
            var utcDate = default(DateTime);
            try
            {
                var inTimeZone = TimeZoneInfo.FindSystemTimeZoneById(inTimeZoneId);
                utcDate = TimeZoneInfo.ConvertTimeToUtc(inDate, inTimeZone);
            }
            catch (Exception)
            {
                return inDate;
            }
            return utcDate;
        }

        public DateTime ConvertFromUTCToModuleTimeZone(DateTime utcDate, string moduleTimeZoneId)
        {
            var moduleDate = default(DateTime);
            try
            {
                var moduleTimeZone = TimeZoneInfo.FindSystemTimeZoneById(moduleTimeZoneId);
                moduleDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, moduleTimeZone);
            }
            catch (Exception)
            {
                return utcDate;
            }
            return moduleDate;
        }

        public class DateInfo
        {
            public DateTime EventDate { get; set; }

            public string EventTimeZoneId { get; set; }
        }
    }

    #endregion
}