using DotNetNuke.Services.Exceptions;
using Microsoft.VisualBasic;
using System.Collections;
using System.Web;
using DotNetNuke.Services.Localization;
using System;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Modules;


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
		public class EventNotification : Services.Scheduling.SchedulerClient
		{
			
#region Constructors
			public EventNotification(Services.Scheduling.ScheduleHistoryItem objScheduleHistoryItem)
			{
				ScheduleHistoryItem = objScheduleHistoryItem;
			}
#endregion
			
#region Methods
			public override void DoWork()
			{
				try
				{
					//notification that the event is progressing
					Progressing(); //OPTIONAL
					
					string returnStrCleanup = "";
					returnStrCleanup = CleanupExpired();
					if (!string.IsNullOrEmpty(returnStrCleanup))
					{
						ScheduleHistoryItem.AddLogNote("<br />" + returnStrCleanup + "<br />");
					}
					
					string returnStr = "";
					returnStr = SendEventNotifications();
					ScheduleHistoryItem.AddLogNote(returnStr);
					
					ScheduleHistoryItem.Succeeded = true; //REQUIRED
					
				}
				catch (Exception exc) //REQUIRED
				{
					ScheduleHistoryItem.Succeeded = false; //REQUIRED
					ScheduleHistoryItem.AddLogNote("Event Notification failed." + exc.ToString() + (int) DotNetNuke.Common.Globals.Status); //OPTIONAL
					//notification that we have errored
					Errored(ref exc); //REQUIRED
					//log the exception
					Exceptions.LogException(exc); //OPTIONAL
				}
			}
			
			private string SendEventNotifications()
			{
				EventController objEvents = new EventController();
				EventInfo objEvent = default(EventInfo);
				EventNotificationController objEventNotifications = new EventNotificationController();
				ArrayList notifyEvents = default(ArrayList);
				string returnStr = "Event Notification completed.";
				
				this.Status = "Sending Event Notifications";
				
				//***  All Event Notifications are stored in UTC internally.
				notifyEvents = objEventNotifications.EventsNotificationsToSend(DateTime.UtcNow);
				
				foreach (EventNotificationInfo objNotification in notifyEvents)
				{
					//Get the Associated Event
					objEvent = objEvents.EventsGet(objNotification.EventID, objNotification.ModuleID);
					
					if (!(ReferenceEquals(objEvent, null)))
					{
						// Setup PortalSettings
						PortalSettings portalSettings = CreatePortalSettings(objNotification.PortalAliasID, objNotification.TabID);
						string folderName = DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", objEvent.PortalID).FolderName;
						
						if (!ReferenceEquals(portalSettings, null))
						{
							// Make up the LocalResourceFile value
							string templateSourceDirectory = Common.Globals.ApplicationPath;
							string localResourceFile = templateSourceDirectory + "/DesktopModules/" + folderName + "/" + Localization.LocalResourceDirectory + "/EventNotification.ascx.resx";
							
							// Send the email
							EventEmailInfo objEventEmailInfo = new EventEmailInfo();
							EventEmails objEventEmail = new EventEmails(portalSettings.PortalId, objNotification.ModuleID, localResourceFile, objNotification.NotifyLanguage);
							objEventEmailInfo.TxtEmailSubject = objEvent.Notify;
							objEventEmailInfo.TxtEmailBody = objEvent.Reminder;
							objEventEmailInfo.TxtEmailFrom = objEvent.ReminderFrom;
							objEventEmailInfo.UserEmails.Add(objNotification.UserEmail);
							objEventEmailInfo.UserLocales.Add(objNotification.NotifyLanguage);
							objEventEmailInfo.UserTimeZoneIds.Add(objEvent.EventTimeZoneId);
							string domainurl = portalSettings.PortalAlias.HTTPAlias;
							if (domainurl.IndexOf("/", StringComparison.Ordinal) > 0)
							{
								domainurl = domainurl.Substring(0, domainurl.IndexOf("/", StringComparison.Ordinal));
							}
							
							objEventEmail.SendEmails(objEventEmailInfo, objEvent, domainurl);
						}
						
						//*** Update Notification (so we don't send again)
						objNotification.NotificationSent = true;
						objEventNotifications.EventsNotificationSave(objNotification);
						returnStr = "Event Notification completed. " + notifyEvents.Count.ToString() + " Notification(s) sent!";
					}
				}
				
				//**** Delete Expired EventNotifications (older than 30 days)
				DateTime endDate = DateTime.UtcNow.AddDays(-30);
				objEventNotifications.EventsNotificationDelete(endDate);
				
				this.Status = "Event Notifications Sent Successfully";
				ScheduleHistoryItem.Succeeded = true;
				return returnStr;
				
			}
			
			private string CleanupExpired()
			{
				string returnStr = "Event Cleanup completed.";
				int noDeletedEvents = 0;
				
				this.Status = "Performing Event Cleanup";
				
				DesktopModuleInfo objDesktopModule = default(DesktopModuleInfo);
				objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", 0);
				
				if (ReferenceEquals(objDesktopModule, null))
				{
					return "Module Definition 'DNN_Events' not found. Cleanup cannot be performed.";
				}

			    this.Status = "Performing Event Cleanup: Dummy";
			    this.Status = "Performing Event Cleanup:" + objDesktopModule.FriendlyName;
				
				Entities.Portals.PortalController objPortals = new Entities.Portals.PortalController();
				Entities.Portals.PortalInfo objPortal = default(Entities.Portals.PortalInfo);
				ModuleController objModules = new ModuleController();
				ModuleInfo objModule = default(ModuleInfo);
				
				ArrayList lstportals = objPortals.GetPortals();
				foreach (Entities.Portals.PortalInfo tempLoopVar_objPortal in lstportals)
				{
					objPortal = tempLoopVar_objPortal;
				    this.Status = "Performing Event Cleanup:" + objDesktopModule.FriendlyName + " PortalID: Dummy";
				    this.Status = "Performing Event Cleanup:" + objDesktopModule.FriendlyName + " PortalID:" + objPortal.PortalID.ToString();
					
					ArrayList lstModules = objModules.GetModulesByDefinition(objPortal.PortalID, objDesktopModule.FriendlyName);
					foreach (ModuleInfo tempLoopVar_objModule in lstModules)
					{
						objModule = tempLoopVar_objModule;
						// This check for objModule = Nothing because of error in DNN 5.0.0 in GetModulesByDefinition
						if (ReferenceEquals(objModule, null))
						{
							continue;
						}
					    this.Status = "Performing Event Cleanup:" + objDesktopModule.FriendlyName + " PortalID:" + objPortal.PortalID.ToString() + " ModuleID: Dummy";
					    this.Status = "Performing Event Cleanup:" + objDesktopModule.FriendlyName + " PortalID:" + objPortal.PortalID.ToString() + " ModuleID:" + objModule.ModuleID.ToString();
						
						EventModuleSettings settings = EventModuleSettings.GetEventModuleSettings(objModule.ModuleID, null);
						if (settings.Expireevents != "")
						{
							this.Status = "Performing Event Cleanup:" + objDesktopModule.FriendlyName + " PortalID:" + objPortal.PortalID.ToString() + " ModuleID:" + objModule.ModuleID.ToString() + " IN PROGRESS";
							
							EventController objEventCtl = new EventController();
							DateTime expireDate = DateAndTime.DateAdd(DateInterval.Day, - System.Convert.ToInt32(settings.Expireevents), DateTime.UtcNow);
							DateTime startdate = expireDate.AddYears(-10);
							DateTime endDate = expireDate.AddDays(1);
							EventInfoHelper objEventInfoHelper = new EventInfoHelper(objModule.ModuleID, settings);
							ArrayList categoryIDs = new ArrayList();
							categoryIDs.Add("-1");
							ArrayList locationIDs = new ArrayList();
							locationIDs.Add("-1");
							ArrayList lstEvents = objEventInfoHelper.GetEvents(startdate, endDate, false, categoryIDs, locationIDs, -1, -1);
							
							EventTimeZoneUtilities objEventTimeZoneUtilities = new EventTimeZoneUtilities();
							foreach (EventInfo objEvent in lstEvents)
							{
								DateTime eventTime = objEventTimeZoneUtilities.ConvertToUTCTimeZone(objEvent.EventTimeEnd, objEvent.EventTimeZoneId);
								if (eventTime < expireDate)
								{
									objEvent.Cancelled = true;
									objEventCtl.EventsSave(objEvent, true, 0, true);
									noDeletedEvents++;
									returnStr = "Event Cleanup completed. " + noDeletedEvents.ToString() + " Events deleted.";
								}
							}
							objEventCtl.EventsCleanupExpired(objModule.PortalID, objModule.ModuleID);
						}
					}
				}
				this.Status = "Cleanup complete";
				return returnStr;
			}
			
			public PortalSettings CreatePortalSettings(int portalAliasID, int tabID)
			{
				string cacheKey = "EventsPortalSettings" + portalAliasID.ToString();
				PortalSettings ps = new PortalSettings();
				PortalSettings pscache = (PortalSettings) (Common.Utilities.DataCache.GetCache(cacheKey));
				
				if (ReferenceEquals(pscache, null))
				{
					// Setup PortalSettings
					PortalAliasController objPortalAliases = new PortalAliasController();
					PortalAliasInfo objPortalAlias = default(PortalAliasInfo);
					
					objPortalAlias = objPortalAliases.GetPortalAliasByPortalAliasID(portalAliasID);
					if (ReferenceEquals(objPortalAlias, null))
					{
						return null;
					}
					PortalController portalController = new PortalController();
					Entities.Portals.PortalInfo portal = portalController.GetPortal(objPortalAlias.PortalID);
					
					
					ps.PortalAlias = objPortalAlias;
					ps.PortalId = portal.PortalID;
					ps.PortalName = portal.PortalName;
					ps.LogoFile = portal.LogoFile;
					ps.FooterText = portal.FooterText;
					ps.ExpiryDate = portal.ExpiryDate;
					ps.UserRegistration = portal.UserRegistration;
					ps.BannerAdvertising = portal.BannerAdvertising;
					ps.Currency = portal.Currency;
					ps.AdministratorId = portal.AdministratorId;
					ps.Email = portal.Email;
					ps.HostFee = portal.HostFee;
					ps.HostSpace = portal.HostSpace;
					ps.PageQuota = portal.PageQuota;
					ps.UserQuota = portal.UserQuota;
					ps.AdministratorRoleId = portal.AdministratorRoleId;
					ps.AdministratorRoleName = portal.AdministratorRoleName;
					ps.RegisteredRoleId = portal.RegisteredRoleId;
					ps.RegisteredRoleName = portal.RegisteredRoleName;
					ps.Description = portal.Description;
					ps.KeyWords = portal.KeyWords;
					ps.BackgroundFile = portal.BackgroundFile;
					ps.GUID = portal.GUID;
					ps.SiteLogHistory = portal.SiteLogHistory;
					ps.AdminTabId = portal.AdminTabId;
					ps.SuperTabId = portal.SuperTabId;
					ps.SplashTabId = portal.SuperTabId;
					ps.HomeTabId = portal.HomeTabId;
					ps.LoginTabId = portal.LoginTabId;
					ps.UserTabId = portal.UserTabId;
					ps.DefaultLanguage = portal.DefaultLanguage;
					ps.Pages = portal.Pages;
					ps.Users = portal.Users;
					if (ps.HostSpace == null)
					{
						ps.HostSpace = 0;
					}
					if (ReferenceEquals(ps.DefaultLanguage, null))
					{
						ps.DefaultLanguage = Localization.SystemLocale;
					}
					if (ReferenceEquals(ps.TimeZone, null))
					{
						ps.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(Localization.SystemTimeZone);
					}
					ps.HomeDirectory = Common.Globals.ApplicationPath + "/" + portal.HomeDirectory + "/";
					Common.Utilities.DataCache.SetCache(cacheKey, ps);
				}
				else
				{
					ps = pscache;
				}
				Entities.Tabs.TabController tabController = new Entities.Tabs.TabController();
				Entities.Tabs.TabInfo tab = tabController.GetTab(tabID, ps.PortalId, false);
				if (ReferenceEquals(tab, null))
				{
					return null;
				}
				ps.ActiveTab = tab;
				
				try
				{
					// Now to put it into the HTTPContext
					if (ReferenceEquals(HttpContext.Current, null))
					{
						string page = ps.PortalAlias.HTTPAlias;
						string query = string.Empty;
						System.IO.TextWriter output = null;
						System.Web.Hosting.SimpleWorkerRequest workerrequest = new System.Web.Hosting.SimpleWorkerRequest(page, query, output);
						HttpContext.Current = new HttpContext(workerrequest);
						HttpContext.Current.Items.Add("PortalSettings", ps);
					}
					else
					{
						HttpContext.Current.Items.Remove("PortalSettings");
						HttpContext.Current.Items.Add("PortalSettings", ps);
					}
				}
				catch (Exception)
				{
				}
				
				return ps;
			}
#endregion
			
		}
		
	}
	

