
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


namespace DotNetNuke.Modules.Events.Providers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Security.Permissions;
    using DotNetNuke.Services.Sitemap;
    using global::Components;
    using Microsoft.VisualBasic;

    public class Sitemap : SitemapProvider
    {
        #region Public Methods

        public override List<SitemapUrl> GetUrls(int portalID, PortalSettings ps, string version)
        {
            var sitemapUrls = new List<SitemapUrl>();

            var objDesktopModule = default(DesktopModuleInfo);
            objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", portalID);

            var objModules = new ModuleController();
            var objModule = default(ModuleInfo);
            var lstModules = objModules.GetModulesByDefinition(portalID, objDesktopModule.FriendlyName);
            var moduleIds = new ArrayList();
            var visibleModuleIds = new ArrayList();
            var visibleTabModuleIds = new ArrayList();
            foreach (ModuleInfo tempLoopVar_objModule in lstModules)
            {
                objModule = tempLoopVar_objModule;
                var objTabPermissions = TabPermissionController.GetTabPermissions(objModule.TabID, portalID);
                var objTabPermission = default(TabPermissionInfo);
                foreach (TabPermissionInfo tempLoopVar_objTabPermission in objTabPermissions)
                {
                    objTabPermission = tempLoopVar_objTabPermission;
                    if (objTabPermission.PermissionKey == "VIEW" && objTabPermission.RoleName != "" &&
                        objTabPermission.AllowAccess && (objTabPermission.RoleID == -1) |
                        (objTabPermission.RoleID == -3))
                    {
                        if (objModule.InheritViewPermissions)
                        {
                            visibleTabModuleIds.Add("Tab" + objModule.TabID + "Mod" + objModule.ModuleID);
                            visibleModuleIds.Add(objModule.ModuleID);
                            break;
                        }
                        var objModulePermission = default(ModulePermissionInfo);
                        // ReSharper disable LoopCanBeConvertedToQuery
                        foreach (ModulePermissionInfo tempLoopVar_objModulePermission in objModule.ModulePermissions)
                        {
                            objModulePermission = tempLoopVar_objModulePermission;
                            // ReSharper restore LoopCanBeConvertedToQuery
                            if (objModulePermission.PermissionKey == "VIEW" && objModulePermission.RoleName != "" &&
                                objModulePermission.AllowAccess && (objModulePermission.RoleID == -1) |
                                (objModulePermission.RoleID == -3))
                            {
                                visibleTabModuleIds.Add("Tab" + objModule.TabID + "Mod" + objModule.ModuleID);
                                visibleModuleIds.Add(objModule.ModuleID);
                                break;
                            }
                        }
                    }
                }
            }
            foreach (ModuleInfo tempLoopVar_objModule in lstModules)
            {
                objModule = tempLoopVar_objModule;
                // This check for objModule = Nothing because of error in DNN 5.0.0 in GetModulesByDefinition
                if (ReferenceEquals(objModule, null))
                {
                    continue;
                }
                if (objModule.IsDeleted)
                {
                    continue;
                }
                if (moduleIds.Contains(objModule.ModuleID))
                {
                    continue;
                }
                if (!visibleTabModuleIds.Contains("Tab" + objModule.TabID + "Mod" + objModule.ModuleID))
                {
                    continue;
                }
                moduleIds.Add(objModule.ModuleID);

                var settings = EventModuleSettings.GetEventModuleSettings(objModule.ModuleID, null);
                if (!settings.EnableSitemap)
                {
                    continue;
                }
                if (settings.SocialGroupModule == EventModuleSettings.SocialModule.UserProfile)
                {
                    continue;
                }

                var iCategoryIDs = new ArrayList();
                if (settings.Enablecategories == EventModuleSettings.DisplayCategories.DoNotDisplay)
                {
                    iCategoryIDs = settings.ModuleCategoryIDs;
                }
                else
                {
                    iCategoryIDs.Add("-1");
                }
                var ilocationIDs = new ArrayList();
                if (settings.Enablelocations == EventModuleSettings.DisplayLocations.DoNotDisplay)
                {
                    ilocationIDs = settings.ModuleLocationIDs;
                }
                else
                {
                    ilocationIDs.Add("-1");
                }

                var objEventTimeZoneUtilities = new EventTimeZoneUtilities();
                var currDate =
                    objEventTimeZoneUtilities.ConvertFromUTCToModuleTimeZone(DateTime.UtcNow, settings.TimeZoneId);
                var dtStartDate = DateAndTime.DateAdd(DateInterval.Day, Convert.ToDouble(-settings.SiteMapDaysBefore),
                                                      currDate);
                var dtEndDate = DateAndTime.DateAdd(DateInterval.Day, settings.SiteMapDaysAfter, currDate);

                var objEventInfoHelper = new EventInfoHelper(objModule.ModuleID, objModule.TabID, portalID, settings);
                var lstevents = default(ArrayList);
                lstevents = objEventInfoHelper.GetEvents(dtStartDate, dtEndDate, settings.MasterEvent, iCategoryIDs,
                                                         ilocationIDs, -1, -1);

                var objEvent = default(EventInfo);
                foreach (EventInfo tempLoopVar_objEvent in lstevents)
                {
                    objEvent = tempLoopVar_objEvent;
                    if (settings.Enforcesubcalperms && !visibleModuleIds.Contains(objEvent.ModuleID))
                    {
                        continue;
                    }
                    var pageUrl = new SitemapUrl();
                    pageUrl.Url = objEventInfoHelper.DetailPageURL(objEvent, false);
                    pageUrl.Priority = settings.SiteMapPriority;
                    pageUrl.LastModified = objEvent.LastUpdatedAt;
                    pageUrl.ChangeFrequency = SitemapChangeFrequency.Daily;
                    sitemapUrls.Add(pageUrl);
                }
            }

            return sitemapUrls;
        }

        #endregion
    }
}