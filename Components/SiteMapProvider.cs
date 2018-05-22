using Microsoft.VisualBasic;
using System.Collections;
using System;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Sitemap;


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
		
		public class Sitemap : SitemapProvider
		{
#region Public Methods
			public override System.Collections.Generic.List<SitemapUrl> GetUrls(int portalID, PortalSettings ps, string version)
			{
				System.Collections.Generic.List<SitemapUrl> sitemapUrls = new System.Collections.Generic.List<SitemapUrl>();
				
				Entities.Modules.DesktopModuleInfo objDesktopModule = default(Entities.Modules.DesktopModuleInfo);
				objDesktopModule = Entities.Modules.DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", portalID);
				
				Entities.Modules.ModuleController objModules = new Entities.Modules.ModuleController();
				Entities.Modules.ModuleInfo objModule = default(Entities.Modules.ModuleInfo);
				ArrayList lstModules = objModules.GetModulesByDefinition(portalID, objDesktopModule.FriendlyName);
				ArrayList moduleIds = new ArrayList();
				ArrayList visibleModuleIds = new ArrayList();
				ArrayList visibleTabModuleIds = new ArrayList();
				foreach (Entities.Modules.ModuleInfo tempLoopVar_objModule in lstModules)
				{
					objModule = tempLoopVar_objModule;
					DotNetNuke.Security.Permissions.TabPermissionCollection objTabPermissions = DotNetNuke.Security.Permissions.TabPermissionController.GetTabPermissions(objModule.TabID, portalID);
					DotNetNuke.Security.Permissions.TabPermissionInfo objTabPermission = default(DotNetNuke.Security.Permissions.TabPermissionInfo);
					foreach (DotNetNuke.Security.Permissions.TabPermissionInfo tempLoopVar_objTabPermission in objTabPermissions)
					{
						objTabPermission = tempLoopVar_objTabPermission;
						if (objTabPermission.PermissionKey == "VIEW" && objTabPermission.RoleName != "" && objTabPermission.AllowAccess && (objTabPermission.RoleID == -1 | objTabPermission.RoleID == -3))
						{
							if (objModule.InheritViewPermissions)
							{
								visibleTabModuleIds.Add("Tab" + objModule.TabID.ToString() + "Mod" + objModule.ModuleID.ToString());
								visibleModuleIds.Add(objModule.ModuleID);
								break;
							}
							else
							{
								DotNetNuke.Security.Permissions.ModulePermissionInfo objModulePermission = default(DotNetNuke.Security.Permissions.ModulePermissionInfo);
								// ReSharper disable LoopCanBeConvertedToQuery
								foreach (DotNetNuke.Security.Permissions.ModulePermissionInfo tempLoopVar_objModulePermission in objModule.ModulePermissions)
								{
									objModulePermission = tempLoopVar_objModulePermission;
									// ReSharper restore LoopCanBeConvertedToQuery
									if (objModulePermission.PermissionKey == "VIEW" && objModulePermission.RoleName != "" && objModulePermission.AllowAccess && (objModulePermission.RoleID == -1 | objModulePermission.RoleID == -3))
									{
										visibleTabModuleIds.Add("Tab" + objModule.TabID.ToString() + "Mod" + objModule.ModuleID.ToString());
										visibleModuleIds.Add(objModule.ModuleID);
										break;
									}
								}
							}
						}
					}
				}
				foreach (Entities.Modules.ModuleInfo tempLoopVar_objModule in lstModules)
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
					if (!visibleTabModuleIds.Contains("Tab" + objModule.TabID.ToString() + "Mod" + objModule.ModuleID.ToString()))
					{
						continue;
					}
					moduleIds.Add(objModule.ModuleID);
					
					EventModuleSettings settings = EventModuleSettings.GetEventModuleSettings(objModule.ModuleID, null);
					if (!settings.EnableSitemap)
					{
						continue;
					}
					if (settings.SocialGroupModule == EventModuleSettings.SocialModule.UserProfile)
					{
						continue;
					}
					
					ArrayList iCategoryIDs = new ArrayList();
					if (settings.Enablecategories == EventModuleSettings.DisplayCategories.DoNotDisplay)
					{
						iCategoryIDs = settings.ModuleCategoryIDs;
					}
					else
					{
						iCategoryIDs.Add("-1");
					}
					ArrayList ilocationIDs = new ArrayList();
					if (settings.Enablelocations == EventModuleSettings.DisplayLocations.DoNotDisplay)
					{
						ilocationIDs = settings.ModuleLocationIDs;
					}
					else
					{
						ilocationIDs.Add("-1");
					}
					
					EventTimeZoneUtilities objEventTimeZoneUtilities = new EventTimeZoneUtilities();
					DateTime currDate = objEventTimeZoneUtilities.ConvertFromUTCToModuleTimeZone(DateTime.UtcNow, settings.TimeZoneId);
					DateTime dtStartDate = DateAndTime.DateAdd(DateInterval.Day, System.Convert.ToDouble(- settings.SiteMapDaysBefore), currDate);
					DateTime dtEndDate = DateAndTime.DateAdd(DateInterval.Day, settings.SiteMapDaysAfter, currDate);
					
					EventInfoHelper objEventInfoHelper = new EventInfoHelper(objModule.ModuleID, objModule.TabID, portalID, settings);
					ArrayList lstevents = default(ArrayList);
					lstevents = objEventInfoHelper.GetEvents(dtStartDate, dtEndDate, settings.MasterEvent, iCategoryIDs, ilocationIDs, -1, -1);
					
					EventInfo objEvent = default(EventInfo);
					foreach (EventInfo tempLoopVar_objEvent in lstevents)
					{
						objEvent = tempLoopVar_objEvent;
						if (settings.Enforcesubcalperms && !visibleModuleIds.Contains(objEvent.ModuleID))
						{
							continue;
						}
						SitemapUrl pageUrl = new SitemapUrl();
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
	

