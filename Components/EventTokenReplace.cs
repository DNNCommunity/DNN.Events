using DotNetNuke.Entities.Users;
using Microsoft.VisualBasic;
using System.Web.UI.WebControls;
using System.Collections;
using DotNetNuke.Common.Utilities;
using System.Web;
using DotNetNuke.Services.Localization;
using System;
using System.Text;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Roles;


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
	    using global::Components;

	    #region TokenReplaceController Class
		/// <summary>
		/// Replaces the tokens that are defined for the Event e-mails and views
		/// </summary>
		public class TokenReplaceControllerClass
		{
			
#region Member Variables
			private string _localResourceFile;
			private int _moduleId;
#endregion
			
#region Constructor
			public TokenReplaceControllerClass(int moduleID, string localResourceFile)
			{
				this.ModuleID = moduleID;
				this.LocalResourceFile = localResourceFile;
			}
#endregion
			
#region Properties
			private int ModuleID
			{
				get
				{
					return _moduleId;
				}
				set
				{
					_moduleId = value;
				}
			}
			
			private string LocalResourceFile
			{
				get
				{
					return _localResourceFile;
				}
				set
				{
					_localResourceFile = value;
				}
			}
#endregion
			
#region Methods
			/// <summary>
			/// Replace tokens in sourcetext with eventspecific token
			/// </summary>
			public string TokenReplaceEvent(EventInfo eventInfo, string sourceText)
			{
				return TokenReplaceEvent(eventInfo, sourceText, null, false);
			}
			public string TokenReplaceEvent(EventInfo eventInfo, string sourceText, EventSignupsInfo eventSignupsInfo)
			{
				return TokenReplaceEvent(eventInfo, sourceText, eventSignupsInfo, false);
			}
			public string TokenReplaceEvent(EventInfo eventInfo, string sourceText, bool addsubmodulename)
			{
				return TokenReplaceEvent(eventInfo, sourceText, null, addsubmodulename);
			}
			public string TokenReplaceEvent(EventInfo eventInfo, string sourceText, EventSignupsInfo eventSignupsInfo, bool addsubmodulename)
			{
				return TokenReplaceEvent(eventInfo, sourceText, eventSignupsInfo, addsubmodulename, false);
			}
			public string TokenReplaceEvent(EventInfo eventInfo, string sourceText, EventSignupsInfo eventSignupsInfo, bool addsubmodulename, bool isEventEditor)
			{
				
				System.Collections.Generic.Dictionary<string, object> dict = new System.Collections.Generic.Dictionary<string, object>();
				
				string cacheKey = "EventsFolderName" + ModuleID.ToString();
				string folderName = System.Convert.ToString(Common.Utilities.DataCache.GetCache(cacheKey));
				if (ReferenceEquals(folderName, null))
				{
					folderName = DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", eventInfo.PortalID).FolderName;
					Common.Utilities.DataCache.SetCache(cacheKey, folderName);
				}
				
				//Module settings
				EventModuleSettings settings = EventModuleSettings.GetEventModuleSettings(ModuleID, LocalResourceFile);
				
				
				Services.Tokens.TokenReplace trn = new Services.Tokens.TokenReplace(Services.Tokens.Scope.DefaultSettings, ModuleID);
				
				//Parameter processing
				sourceText = TokenParameters(sourceText, eventInfo, settings);
				
				//title
				if (!dict.ContainsKey("title"))
				{
					if (addsubmodulename && eventInfo.ModuleTitle != null)
					{
						//Eventname and moduletitle
						dict.Add("title", string.Format("{0} ({1})", eventInfo.EventName, eventInfo.ModuleTitle.Trim()));
					}
					else
					{
						//Just eventname
						dict.Add("title", eventInfo.EventName);
					}
				}
				
				//submodule name
				if (eventInfo.ModuleTitle != null && ModuleID != eventInfo.ModuleID)
				{
					dict.Add("subcalendarname", string.Format("({0})", eventInfo.ModuleTitle.Trim()));
					dict.Add("subcalendarnameclean", eventInfo.ModuleTitle.Trim());
				}
				
				//alldayeventtext
				dict.Add("alldayeventtext", Localization.GetString("TokenAllDayEventText", LocalResourceFile));
				
				//startdatelabel
				dict.Add("startdatelabel", Localization.GetString("TokenStartdateLabel", LocalResourceFile));
				
				//startdate
				sourceText = TokenReplaceDate(sourceText, "event", "startdate", eventInfo.EventTimeBegin);
				
				//enddatelabel
				dict.Add("enddatelabel", Localization.GetString("TokenEnddateLabel", LocalResourceFile));
				
				//enddate
				sourceText = TokenReplaceDate(sourceText, "event", "enddate", eventInfo.EventTimeEnd);
				
				//timezone
				// Added a try/catch since dnn core can failing getting timezone info
				try
				{
					string value = TimeZoneInfo.FindSystemTimeZoneById(eventInfo.EventTimeZoneId).DisplayName;
					dict.Add("timezone", value);
				}
				catch
				{
					dict.Add("timezone", "");
				}
				
				//Duration
				dict.Add("durationdayslabel", Localization.GetString("TokenDurationDaysLabel", LocalResourceFile));
				dict.Add("durationdays", System.Convert.ToInt32(Conversion.Int((double) eventInfo.Duration / 1440 + 1)));
				
				//descriptionlabel
				dict.Add("descriptionlabel", Localization.GetString("TokenDescriptionLabel", LocalResourceFile));
				
				//description
				if (!dict.ContainsKey("description"))
				{
					dict.Add("description", HttpUtility.HtmlDecode(eventInfo.EventDesc));
				}
				sourceText = TokenLength(sourceText, "event", "description", dict);
				
				//categorylabel
				if (!ReferenceEquals(eventInfo.CategoryName, null))
				{
					dict.Add("categorylabel", Localization.GetString("TokenCategoryLabel", LocalResourceFile));
				}
				else
				{
					dict.Add("categorylabel", "");
				}
				
				//category, categoryfontcolor, categorybackcolor
				if (!ReferenceEquals(eventInfo.CategoryName, null))
				{
					if (eventInfo.Color.Length > 0)
					{
						dict.Add("category", string.Format("<div style='background-color:{1};color:{2}'>{0}</div>", eventInfo.CategoryName, eventInfo.Color, eventInfo.FontColor));
						dict.Add("categoryfontcolor", eventInfo.FontColor);
						dict.Add("categorybackcolor", eventInfo.Color);
					}
					else
					{
						dict.Add("category", eventInfo.CategoryName);
					}
					dict.Add("categoryname", eventInfo.CategoryName);
				}
				else
				{
					dict.Add("category", "");
					dict.Add("categoryname", "");
				}
				
				//locationlabel
				if (!ReferenceEquals(eventInfo.LocationName, null))
				{
					dict.Add("locationlabel", Localization.GetString("TokenLocationLabel", LocalResourceFile));
				}
				else
				{
					dict.Add("locationlabel", "");
				}
				
				//location, locationurl
				if (!ReferenceEquals(eventInfo.MapURL, null) && eventInfo.MapURL != "")
				{
					dict.Add("location", string.Format("<a href='{1}'>{0}</a>", eventInfo.LocationName, eventInfo.MapURL));
					dict.Add("locationurl", eventInfo.MapURL);
				}
				else
				{
					dict.Add("location", eventInfo.LocationName);
					dict.Add("locationurl", "");
				}
				
				//locationname
				dict.Add("locationname", eventInfo.LocationName);
				
				//Other location properties.
				EventLocationInfo eventLocation = new EventLocationInfo();
				eventLocation = new EventLocationController().EventsLocationGet(eventInfo.Location, eventInfo.PortalID);
				if (eventLocation != null)
				{
					dict.Add("locationaddresslabel", Localization.GetString("TokenLocationAddressLabel", LocalResourceFile));
					dict.Add("locationstreet", eventLocation.Street);
					dict.Add("locationpostalcode", eventLocation.PostalCode);
					dict.Add("locationcity", eventLocation.City);
					dict.Add("locationregion", eventLocation.Region);
					dict.Add("locationcountry", eventLocation.Country);
				}
				else
				{
					dict.Add("locationaddresslabel", "");
					dict.Add("locationstreet", "");
					dict.Add("locationpostalcode", "");
					dict.Add("locationcity", "");
					dict.Add("locationregion", "");
					dict.Add("locationcountry", "");
				}
				
				//customfield1
				if (settings.EventsCustomField1)
				{
					dict.Add("customfield1", eventInfo.CustomField1);
				}
				else
				{
					dict.Add("customfield1", "");
				}
				
				//customfield1label
				if (settings.EventsCustomField1)
				{
					dict.Add("customfield1label", Localization.GetString("TokenCustomField1Label", LocalResourceFile));
				}
				else
				{
					dict.Add("customfield1label", "");
				}
				
				//customfield2
				if (settings.EventsCustomField2)
				{
					dict.Add("customfield2", eventInfo.CustomField2);
				}
				else
				{
					dict.Add("customfield2", "");
				}
				
				//customfield2label
				if (settings.EventsCustomField2)
				{
					dict.Add("customfield2label", Localization.GetString("TokenCustomField2Label", LocalResourceFile));
				}
				else
				{
					dict.Add("customfield2label", "");
				}
				
				//descriptionlabel
				dict.Add("summarylabel", Localization.GetString("TokenSummaryLabel", LocalResourceFile));
				
				//description
				if (!dict.ContainsKey("summary"))
				{
					dict.Add("summary", HttpUtility.HtmlDecode(eventInfo.Summary));
				}
				sourceText = TokenLength(sourceText, "event", "summary", dict);
				
				//eventid
				dict.Add("eventid", eventInfo.EventID);
				
				//eventmoduleid
				dict.Add("eventmoduleid", eventInfo.ModuleID);
				
				
				//Createddate
				//TokenCreatedOnLabel.Text   on
				dict.Add("createddatelabel", Localization.GetString("TokenCreatedOnLabel", LocalResourceFile));
				sourceText = TokenReplaceDate(sourceText, "event", "createddate", eventInfo.CreatedDate);
				
				//LastUpdateddate
				//TokenLastUpdatedOnLabel.Text   Last updated on
				dict.Add("lastupdateddatelabel", Localization.GetString("TokenLastUpdatedOnLabel", LocalResourceFile));
				sourceText = TokenReplaceDate(sourceText, "event", "lastupdateddate", eventInfo.LastUpdatedAt);
				
				if (settings.Eventsignup && eventInfo.Signups)
				{
					//maxenrollmentslabel
					//maxenrollments
					dict.Add("maxenrollmentslabel", Localization.GetString("TokenMaxEnrollmentsLabel", LocalResourceFile));
					if (eventInfo.MaxEnrollment > 0)
					{
						dict.Add("maxenrollments", eventInfo.MaxEnrollment.ToString());
					}
					else
					{
						dict.Add("maxenrollments", Localization.GetString("Unlimited", LocalResourceFile));
					}
					
					//noenrollmentslabel
					//noenrollments
					dict.Add("noenrollmentslabel", Localization.GetString("TokenNoEnrollmentsLabel", LocalResourceFile));
					dict.Add("noenrollments", eventInfo.Enrolled.ToString());
					
					//novacancieslabel
					//novacancies
					dict.Add("novacancieslabel", Localization.GetString("TokenNoVacanciesLabel", LocalResourceFile));
					if (eventInfo.MaxEnrollment > 0)
					{
						dict.Add("novacancies", (eventInfo.MaxEnrollment - eventInfo.Enrolled).ToString());
					}
					else
					{
						dict.Add("novacancies", Localization.GetString("Unlimited", LocalResourceFile));
					}
				}
				else
				{
					dict.Add("maxenrollmentslabel", "");
					dict.Add("maxenrollments", "");
					dict.Add("noenrollmentslabel", "");
					dict.Add("noenrollments", "");
				}
				
				if (eventInfo.SocialGroupId > 0)
				{
					//groupnamelabel
					//groupname
					RoleController roleController = new RoleController();
					string rolename = roleController.GetRole(eventInfo.SocialGroupId, eventInfo.PortalID).RoleName;
					dict.Add("socialgrouprolenamelabel", Localization.GetString("TokenSocialGroupRoleNameLabel", LocalResourceFile));
					dict.Add("socialgrouprolename", rolename);
					dict.Add("socialgrouproleid", eventInfo.SocialGroupId.ToString());
				}
				
				if (eventInfo.SocialUserUserName != null)
				{
					//socialuserusernamelabel
					//socialuserusername
					dict.Add("socialuserusernamelabel", Localization.GetString("TokenSocialUserUserNameLabel", LocalResourceFile));
					dict.Add("socialuserusername", eventInfo.SocialUserUserName);
				}
				
				if (eventInfo.SocialUserDisplayName != null)
				{
					//socialuserdisplaynamelabel
					//socialuserdisplayname
					dict.Add("socialuserdisplaynamelabel", Localization.GetString("TokenSocialUserDisplayNameLabel", LocalResourceFile));
					dict.Add("socialuserdisplayname", eventInfo.SocialUserDisplayName);
				}
				
				// Process Event Signups Info is passed
				if (eventSignupsInfo != null)
				{
					//signupuserid
					dict.Add("signupuserid", eventSignupsInfo.UserID);
					
					//signupusername, ...
					if (eventSignupsInfo.UserID != -1)
					{
						dict.Add("signupusername", eventSignupsInfo.UserName);
						dict.Add("signupuserfirstname", "");
						dict.Add("signupuserlastname", "");
						dict.Add("signupuseremail", "");
						dict.Add("signupuserstreet", "");
						dict.Add("signupuserpostalcode", "");
						dict.Add("signupusercity", "");
						dict.Add("signupuserregion", "");
						dict.Add("signupusercountry", "");
						dict.Add("signupusercompany", "");
						dict.Add("signupuserjobtitle", "");
						dict.Add("signupuserrefnumber", "");
					}
					else
					{
						dict.Add("signupusername", eventSignupsInfo.AnonName);
						dict.Add("signupuserfirstname", eventSignupsInfo.FirstName);
						dict.Add("signupuserlastname", eventSignupsInfo.LastName);
						dict.Add("signupuseremail", eventSignupsInfo.AnonEmail);
						dict.Add("signupuserstreet", eventSignupsInfo.Street);
						dict.Add("signupuserpostalcode", eventSignupsInfo.PostalCode);
						dict.Add("signupusercity", eventSignupsInfo.City);
						dict.Add("signupuserregion", eventSignupsInfo.Region);
						dict.Add("signupusercountry", eventSignupsInfo.Country);
						dict.Add("signupusercompany", eventSignupsInfo.Company);
						dict.Add("signupuserjobtitle", eventSignupsInfo.JobTitle);
						dict.Add("signupuserrefnumber", eventSignupsInfo.ReferenceNumber);
					}
					
					//signupdatelabel
					dict.Add("signupdatelabel", Localization.GetString("TokenSignupdateLabel", LocalResourceFile));
					
					//signupdate
					sourceText = TokenReplaceDate(sourceText, "event", "signupdate", eventSignupsInfo.PayPalPaymentDate);
					
					//noenroleeslabel
					dict.Add("noenroleeslabel", Localization.GetString("TokenNoenroleesLabel", LocalResourceFile));
					
					//noenrolees
					dict.Add("noenrolees", eventSignupsInfo.NoEnrolees);
				}
				
				//Custom/external enrollment page
				if (settings.EnrollmentPageAllowed)
				{
					dict.Add("enrollmentdefaulturl", settings.EnrollmentPageDefaultUrl);
				}
				else
				{
					dict.Add("enrollmentdefaulturl", "");
				}
				
				//try and get the portalsettings. When in scheduled mode (EventNotifications)
				//and no permissions, these will not be available and will error
				PortalSettings ps = null;
				try
				{
					ps = (Entities.Portals.PortalSettings) (HttpContext.Current.Items["PortalSettings"]);
				}
				catch (Exception)
				{
				}
				if (!ReferenceEquals(ps, null))
				{
					// add tokens for items that use PortalSettings
					TokenReplacewithPortalSettings(ps, eventInfo, settings, dict, folderName, sourceText, isEventEditor);
				}
				
				return trn.ReplaceEnvironmentTokens(sourceText, dict, "event");
			}
			
			private string TokenLength(string sourceText, string customCaption, string customToken, System.Collections.Generic.Dictionary<string, object> dict)
			{
				Services.Tokens.TokenReplace trn = new Services.Tokens.TokenReplace(Services.Tokens.Scope.DefaultSettings, ModuleID);
				string tokenText = System.Convert.ToString(trn.ReplaceEnvironmentTokens("[" + customCaption + ":" + customToken + "]", dict, customCaption));
				while (sourceText.IndexOf("[" + customCaption + ":" + customToken + "]") + 1 > 0 || sourceText.IndexOf("[" + customCaption + ":" + customToken + "|") + 1 > 0)
				{
					var with_1 = GetTokenFormat(sourceText, customToken, customCaption);
					if (with_1.Tokenfound)
					{
						if (!string.IsNullOrEmpty(with_1.Formatstring))
						{
							sourceText = sourceText.Replace("[" + customCaption + ":" + customToken + "|" + with_1.Formatstring + "]", HtmlUtils.Shorten(tokenText, int.Parse(with_1.Formatstring), "..."));
						}
						else
						{
							sourceText = sourceText.Replace("[" + customCaption + ":" + customToken + "]", tokenText);
						}
					}
				}
				return sourceText;
			}
			
			private string TokenReplaceDate(string sourceText, string customCaption, string customToken, DateTime customDate)
			{
				
				DateTime tokenDate = customDate.Date.AddMinutes(customDate.TimeOfDay.TotalMinutes);
				while (sourceText.IndexOf("[" + customCaption + ":" + customToken + "]") + 1 > 0 || sourceText.IndexOf("[" + customCaption + ":" + customToken + "|") + 1 > 0)
				{
					var with_1 = GetTokenFormat(sourceText, customToken, customCaption);
					if (with_1.Tokenfound)
					{
						if (!string.IsNullOrEmpty(with_1.Formatstring))
						{
							sourceText = sourceText.Replace("[" + customCaption + ":" + customToken + "|" + with_1.Formatstring + "]", string.Format("{0:" + with_1.Formatstring + "}", tokenDate));
						}
						else
						{
							sourceText = sourceText.Replace("[" + customCaption + ":" + customToken + "]", string.Format("{0:f}", tokenDate));
						}
					}
				}
				
				return sourceText;
				
			}
			
			public string TokenParameters(string sourceText, EventInfo eventInfo, EventModuleSettings settings)
			{
				
				if (eventInfo.AllDayEvent)
				{
					sourceText = TokenOneParameter(sourceText, "ALLDAYEVENT", true);
					sourceText = TokenOneParameter(sourceText, "NOTALLDAYEVENT", false);
				}
				else
				{
					sourceText = TokenOneParameter(sourceText, "ALLDAYEVENT", false);
					sourceText = TokenOneParameter(sourceText, "NOTALLDAYEVENT", true);
				}
				if (eventInfo.DisplayEndDate)
				{
					sourceText = TokenOneParameter(sourceText, "DISPLAYENDDATE", true);
				}
				else
				{
					sourceText = TokenOneParameter(sourceText, "DISPLAYENDDATE", false);
				}
				
				object eventimagesetting = settings.Eventimage;
				bool eventimagebool = false;
				if (bool.TryParse(Convert.ToString(eventimagesetting), out eventimagebool) && (eventimagebool && eventInfo.ImageDisplay))
				{
					sourceText = TokenOneParameter(sourceText, "IFHASIMAGE", true);
					sourceText = TokenOneParameter(sourceText, "IFNOTHASIMAGE", false);
				}
				else
				{
					sourceText = TokenOneParameter(sourceText, "IFHASIMAGE", false);
					sourceText = TokenOneParameter(sourceText, "IFNOTHASIMAGE", true);
				}
				
				if (eventInfo.Category > 0)
				{
					sourceText = TokenOneParameter(sourceText, "IFHASCATEGORY", true);
				}
				else
				{
					sourceText = TokenOneParameter(sourceText, "IFHASCATEGORY", false);
				}
				if (eventInfo.Location > 0)
				{
					sourceText = TokenOneParameter(sourceText, "IFHASLOCATION", true);
				}
				else
				{
					sourceText = TokenOneParameter(sourceText, "IFHASLOCATION", false);
				}
				if (eventInfo.MapURL != "")
				{
					sourceText = TokenOneParameter(sourceText, "IFHASLOCATIONURL", true);
				}
				else
				{
					sourceText = TokenOneParameter(sourceText, "IFHASLOCATIONURL", false);
				}
				if (eventInfo.MapURL == "")
				{
					sourceText = TokenOneParameter(sourceText, "IFNOTHASLOCATIONURL", true);
				}
				else
				{
					sourceText = TokenOneParameter(sourceText, "IFNOTHASLOCATIONURL", false);
				}
				if (settings.Eventsignup && eventInfo.Signups)
				{
					sourceText = TokenOneParameter(sourceText, "IFALLOWSENROLLMENTS", true);
				}
				else
				{
					sourceText = TokenOneParameter(sourceText, "IFALLOWSENROLLMENTS", false);
				}
				if (settings.EventsCustomField1)
				{
					sourceText = TokenOneParameter(sourceText, "DISPLAYCUSTOMFIELD1", true);
				}
				else
				{
					sourceText = TokenOneParameter(sourceText, "DISPLAYCUSTOMFIELD1", false);
				}
				if (settings.EventsCustomField2)
				{
					sourceText = TokenOneParameter(sourceText, "DISPLAYCUSTOMFIELD2", true);
				}
				else
				{
					sourceText = TokenOneParameter(sourceText, "DISPLAYCUSTOMFIELD2", false);
				}
				if (settings.DetailPageAllowed && eventInfo.DetailPage)
				{
					sourceText = TokenOneParameter(sourceText, "CUSTOMDETAILPAGE", true);
				}
				else
				{
					sourceText = TokenOneParameter(sourceText, "CUSTOMDETAILPAGE", false);
				}
				if (eventInfo.EventTimeBegin.Date == eventInfo.EventTimeEnd.Date) //one day event...
				{
					sourceText = TokenOneParameter(sourceText, "ONEDAYEVENT", true);
					sourceText = TokenOneParameter(sourceText, "NOTONEDAYEVENT", false);
				}
				else
				{
					sourceText = TokenOneParameter(sourceText, "ONEDAYEVENT", false);
					sourceText = TokenOneParameter(sourceText, "NOTONEDAYEVENT", true);
				}
				if (eventInfo.RRULE != "") //recurring event
				{
					sourceText = TokenOneParameter(sourceText, "RECURRINGEVENT", true);
					sourceText = TokenOneParameter(sourceText, "NOTRECURRINGEVENT", false);
				}
				else
				{
					sourceText = TokenOneParameter(sourceText, "RECURRINGEVENT", false);
					sourceText = TokenOneParameter(sourceText, "NOTRECURRINGEVENT", true);
				}
				if (eventInfo.IsPrivate) //Is private event
				{
					sourceText = TokenOneParameter(sourceText, "PRIVATE", true);
					sourceText = TokenOneParameter(sourceText, "NOTPRIVATE", false);
				}
				else
				{
					sourceText = TokenOneParameter(sourceText, "PRIVATE", false);
					sourceText = TokenOneParameter(sourceText, "NOTPRIVATE", true);
				}
				if (settings.Tzdisplay)
				{
					sourceText = TokenOneParameter(sourceText, "IFTIMEZONEDISPLAY", true);
				}
				else
				{
					sourceText = TokenOneParameter(sourceText, "IFTIMEZONEDISPLAY", false);
				}
				if (eventInfo.Duration > 1440)
				{
					sourceText = TokenOneParameter(sourceText, "IFMULTIDAY", true);
					sourceText = TokenOneParameter(sourceText, "IFNOTMULTIDAY", false);
				}
				else
				{
					sourceText = TokenOneParameter(sourceText, "IFMULTIDAY", false);
					sourceText = TokenOneParameter(sourceText, "IFNOTMULTIDAY", true);
				}
				
				if (sourceText.Contains("[HASROLE_") || sourceText.Contains("[HASNOTROLE_"))
				{
					RoleInfo role = default(RoleInfo);
					RoleController roleController = new RoleController();
					System.Collections.Generic.IList<UserRoleInfo> userRolesIList = default(System.Collections.Generic.IList<UserRoleInfo>);
					ArrayList userRoles = new ArrayList();
					if (!ReferenceEquals(UserController.GetCurrentUserInfo(), null))
					{
						userRolesIList = roleController.GetUserRoles(UserController.GetCurrentUserInfo(), true);
						// ReSharper disable NotAccessedVariable
						int i = 0;
						// ReSharper restore NotAccessedVariable
						foreach (UserRoleInfo userRole in userRolesIList)
						{
							userRoles.Add(userRole.RoleName);
							i++;
						}
					}
					foreach (RoleInfo tempLoopVar_role in roleController.GetPortalRoles(eventInfo.PortalID))
					{
						role = tempLoopVar_role;
						sourceText = TokenOneParameter(sourceText, "HASROLE_" + role.RoleName, userRoles.Contains(role.RoleName));
						sourceText = TokenOneParameter(sourceText, "HASNOTROLE_" + role.RoleName, !userRoles.Contains(role.RoleName));
					}
				}
				
				if (eventInfo.Summary != "")
				{
					sourceText = TokenOneParameter(sourceText, "IFHASSUMMARY", true);
				}
				else
				{
					sourceText = TokenOneParameter(sourceText, "IFHASSUMMARY", false);
				}
				if (eventInfo.Summary == "")
				{
					sourceText = TokenOneParameter(sourceText, "IFNOTHASSUMMARY", true);
				}
				else
				{
					sourceText = TokenOneParameter(sourceText, "IFNOTHASSUMMARY", false);
				}
				
				if (sourceText.Contains("[IFENROLED]") || sourceText.Contains("[IFNOTENROLED]"))
				{
					bool blEnroled = false;
					bool blNotEnroled = false;
					if (eventInfo.Signups)
					{
						blNotEnroled = true;
						if (!ReferenceEquals(UserController.GetCurrentUserInfo(), null))
						{
							EventSignupsController signupsController = new EventSignupsController();
							EventSignupsInfo signupInfo = signupsController.EventsSignupsGetUser(eventInfo.EventID, UserController.GetCurrentUserInfo().UserID, eventInfo.ModuleID);
							if (!ReferenceEquals(signupInfo, null))
							{
								blEnroled = signupInfo.Approved;
								blNotEnroled = !signupInfo.Approved;
							}
						}
					}
					sourceText = TokenOneParameter(sourceText, "IFENROLED", blEnroled);
					sourceText = TokenOneParameter(sourceText, "IFNOTENROLED", blNotEnroled);
				}
				
				if (eventInfo.SocialUserId > 0)
				{
					sourceText = TokenOneParameter(sourceText, "IFISSOCIALUSER", true);
				}
				else
				{
					sourceText = TokenOneParameter(sourceText, "IFISSOCIALUSER", false);
				}
				
				if (eventInfo.SocialGroupId > 0)
				{
					sourceText = TokenOneParameter(sourceText, "IFISSOCIALGROUP", true);
				}
				else
				{
					sourceText = TokenOneParameter(sourceText, "IFISSOCIALGROUP", false);
				}
				
				if (eventInfo.MaxEnrollment > 0 && eventInfo.Enrolled >= eventInfo.MaxEnrollment)
				{
					sourceText = TokenOneParameter(sourceText, "IFISFULL", true);
				}
				else
				{
					sourceText = TokenOneParameter(sourceText, "IFISFULL", false);
				}
				if ((eventInfo.MaxEnrollment > 0 && eventInfo.Enrolled < eventInfo.MaxEnrollment) || eventInfo.MaxEnrollment == 0)
				{
					sourceText = TokenOneParameter(sourceText, "IFNOTISFULL", true);
				}
				else
				{
					sourceText = TokenOneParameter(sourceText, "IFNOTISFULL", false);
				}
				
				return sourceText;
			}
			
			public string TokenOneParameter(string sourceText, string parameterName, bool parameterKeep)
			{
				StringBuilder sourceTextOut = new StringBuilder();
				if (parameterKeep)
				{
					sourceTextOut.Insert(0, sourceText);
					sourceTextOut.Replace("[" + parameterName + "]", "");
					sourceTextOut.Replace("[/" + parameterName + "]", "");
				}
				else
				{
					System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex("\\[" + parameterName + "][.\\s\\S]*?\\[/" + parameterName + "]");
					sourceTextOut.Insert(0, rgx.Replace(sourceText, ""));
				}
				return sourceTextOut.ToString();
			}
			
			private void TokenReplacewithPortalSettings(PortalSettings ps, EventInfo eventInfo, EventModuleSettings settings, System.Collections.Generic.Dictionary<string, object> dict, string folderName, string sourceText, bool isEventEditor)
			{
				//Build URL for event images
				EventInfoHelper eventInfoHelper = new EventInfoHelper(ModuleID, ps.ActiveTab.TabID, eventInfo.PortalID, settings);
				
				// Dim portalurl As String = ps.PortalAlias.HTTPAlias
				// Dim domainurl As String = ps.PortalAlias.HTTPAlias
				string domainurl = eventInfoHelper.GetDomainURL();
				string portalurl = domainurl;
				if (ps.PortalAlias.HTTPAlias.IndexOf("/", StringComparison.Ordinal) > 0)
				{
					portalurl = portalurl + Common.Globals.ApplicationPath;
				}
				string imagepath = DotNetNuke.Common.Globals.AddHTTP(string.Format("{0}/DesktopModules/{1}/Images/", portalurl, folderName));
				
				//eventimage
				if (settings.Eventimage && eventInfo.ImageDisplay)
				{
					string imageSrc = eventInfo.ImageURL;
					
					if (eventInfo.ImageURL.StartsWith("FileID="))
					{
						int fileId = int.Parse(eventInfo.ImageURL.Substring(7));
						Services.FileSystem.IFileInfo objFileInfo = Services.FileSystem.FileManager.Instance.GetFile(fileId);
						if (!ReferenceEquals(objFileInfo, null))
						{
							imageSrc = System.Convert.ToString(objFileInfo.Folder + objFileInfo.FileName);
							if (imageSrc.IndexOf("://") + 1 == 0)
							{
								Entities.Portals.PortalController pi = new Entities.Portals.PortalController();
								imageSrc = DotNetNuke.Common.Globals.AddHTTP(string.Format("{0}/{1}/{2}", portalurl, pi.GetPortal(eventInfo.PortalID).HomeDirectory, imageSrc));
							}
						}
					}
					
					if (eventInfo.ImageWidth > 0 & eventInfo.ImageHeight > 0)
					{
						dict.Add("eventimage", string.Format("<img src='{0}' alt='' width='{1}' height='{2}' />", imageSrc, Unit.Pixel(eventInfo.ImageWidth), Unit.Pixel(eventInfo.ImageHeight)));
						int thumbWidth = eventInfo.ImageWidth;
						int thumbHeight = eventInfo.ImageHeight;
						if (eventInfo.ImageHeight > settings.MaxThumbHeight)
						{
							thumbHeight = settings.MaxThumbHeight;
							thumbWidth = System.Convert.ToInt32((double) eventInfo.ImageWidth * settings.MaxThumbHeight / eventInfo.ImageHeight);
						}
						if (thumbWidth > settings.MaxThumbWidth)
						{
							thumbWidth = settings.MaxThumbWidth;
							thumbHeight = System.Convert.ToInt32((double) eventInfo.ImageHeight * settings.MaxThumbWidth / eventInfo.ImageWidth);
						}
						dict.Add("eventthumb", string.Format("<img src='{0}' alt='' width='{1}' height='{2}' />", imageSrc, Unit.Pixel(thumbWidth), Unit.Pixel(thumbHeight)));
					}
					else
					{
						dict.Add("eventimage", string.Format("<img src='{0}' alt='' />", imageSrc));
						dict.Add("eventthumb", string.Format("<img src='{0}' alt='' />", imageSrc));
					}
					dict.Add("imageurl", imageSrc);
				}
				else
				{
					//?
					dict.Add("eventimage", "");
					dict.Add("eventthumb", "");
					dict.Add("imageurl", "");
				}
				
				//importancelabel
				dict.Add("importancelabel", Localization.GetString("TokenImporatanceLabel", LocalResourceFile));
				
				//importance, importanceicon
				string result = "<img src='{0}{1}' class=\"{4}\" alt='{2}' /> {3}";
				switch (eventInfo.Importance)
				{
				    case EventInfo.Priority.High:
				        dict.Add("importance", string.Format(result, imagepath, "HighPrio.gif", Localization.GetString("HighPrio", this.LocalResourceFile), Localization.GetString("HighPrio", this.LocalResourceFile), "EventIconHigh"));
				        dict.Add("importanceicon", string.Format(result, imagepath, "HighPrio.gif", Localization.GetString("HighPrio", this.LocalResourceFile), "", "EventIconHigh"));
				        break;
				    case EventInfo.Priority.Low:
				        dict.Add("importance", string.Format(result, imagepath, "LowPrio.gif", Localization.GetString("LowPrio", this.LocalResourceFile), Localization.GetString("LowPrio", this.LocalResourceFile), "EventIconLow"));
				        dict.Add("importanceicon", string.Format(result, imagepath, "LowPrio.gif", Localization.GetString("HighPrio", this.LocalResourceFile), "", "EventIconLow"));
				        break;
				    case EventInfo.Priority.Medium:
				        dict.Add("importance", Localization.GetString("NormPrio", this.LocalResourceFile));
				        dict.Add("importanceicon", "");
				        break;
				}
				
				//reminderlabel
				dict.Add("reminderlabel", Localization.GetString("TokenReminderLabel", LocalResourceFile));
				
				//reminder, remindericon
				string img = "";
				if (!ReferenceEquals(sourceText, null))
				{
					if (sourceText.Contains("[event:reminder]") || sourceText.Contains("[event:remindericon]"))
					{
						string notificationInfo = "";
						img = "";
						string userEmail = Entities.Users.UserController.GetCurrentUserInfo().Email;
						if (eventInfo.SendReminder && HttpContext.Current.Request.IsAuthenticated)
						{
							EventNotificationController objEventNotificationController = new EventNotificationController();
							notificationInfo = objEventNotificationController.NotifyInfo(eventInfo.EventID, userEmail, eventInfo.ModuleID, LocalResourceFile, eventInfo.EventTimeZoneId);
						}
						if (eventInfo.SendReminder && HttpContext.Current.Request.IsAuthenticated && !string.IsNullOrEmpty(notificationInfo))
						{
							img = string.Format("<img src='{0}bell.gif' class=\"{2}\" alt='{1}' />", imagepath, Localization.GetString("ReminderEnabled", LocalResourceFile) + ": " + notificationInfo, "EventIconRem");
						}
						else if (eventInfo.SendReminder && (settings.Notifyanon || HttpContext.Current.Request.IsAuthenticated))
						{
							img = string.Format("<img src='{0}bell.gif' class=\"{2}\" alt='{1}' />", imagepath, Localization.GetString("ReminderEnabled", LocalResourceFile), "EventIconRem");
						}
						dict.Add("reminder", notificationInfo);
						dict.Add("remindericon", img);
					}
				}
				//enrollicon
				EventSignupsController objEventSignupsController = new EventSignupsController();
				img = "";
				if (objEventSignupsController.DisplayEnrollIcon(eventInfo) && settings.Eventsignup)
				{
					if (eventInfo.MaxEnrollment == 0 | eventInfo.Enrolled < eventInfo.MaxEnrollment)
					{
						img = string.Format("<img src='{0}enroll.gif' class=\"{2}\" alt='{1}' />", imagepath, Localization.GetString("EnrollEnabled", LocalResourceFile), "EventIconEnroll");
					}
					else
					{
						img = string.Format("<img src='{0}EnrollFull.gif' class=\"{2}\" alt='{1}' />", imagepath, Localization.GetString("EnrollFull", LocalResourceFile), "EventIconEnrollFull");
					}
				}
				dict.Add("enrollicon", img);
				
				//recurringlabel
				dict.Add("recurringlabel", Localization.GetString("TokenRecurranceLabel", LocalResourceFile));
				
				//recurring, recurringicon
				EventRRULEInfo objEventRRULE = default(EventRRULEInfo);
				EventRecurMasterController objCtlEventRecurMaster = new EventRecurMasterController();
				objEventRRULE = objCtlEventRecurMaster.DecomposeRRULE(eventInfo.RRULE, eventInfo.EventTimeBegin);
				result = objCtlEventRecurMaster.RecurrenceText(objEventRRULE, LocalResourceFile, System.Threading.Thread.CurrentThread.CurrentCulture, eventInfo.EventTimeBegin);
				img = "";
				if (eventInfo.RRULE != "")
				{
					img = string.Format("<img src='{0}rec.gif' class=\"{2}\" alt='{1}' />", imagepath, Localization.GetString("RecurringEvent", LocalResourceFile), "EventIconRec");
					result = img + " " + result + " " + objCtlEventRecurMaster.RecurrenceInfo(eventInfo, LocalResourceFile);
				}
				dict.Add("recurring", result);
				dict.Add("recurringicon", img);
				
				//titleurl
				string eventurl = eventInfoHelper.DetailPageURL(eventInfo);
				dict.Add("eventurl", eventurl);
				if (eventInfo.DetailPage && eventInfo.DetailNewWin)
				{
					dict.Add("titleurl", "<a href=\"" + eventurl + "\" target=\"_blank\">" + eventInfo.EventName + "</a>");
				}
				else
				{
					dict.Add("titleurl", "<a href=\"" + eventurl + "\">" + eventInfo.EventName + "</a>");
				}
				
				//View page url
				if (settings.DetailPageAllowed && eventInfo.DetailPage)
				{
					string strUserID = Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString();
					int userID = -1;
					if (Information.IsNumeric(strUserID))
					{
						userID = int.Parse(strUserID);
					}
					bool blAuthenticated = false;
					if (userID > -1)
					{
						blAuthenticated = true;
					}
					if (eventInfo.CreatedByID == userID | eventInfo.OwnerID == userID | eventInfo.RmOwnerID == userID || eventInfoHelper.IsModerator(blAuthenticated) == true || Security.PortalSecurity.IsInRole(ps.AdministratorRoleName.ToString()))
					{
						string imgurl = Entities.Icons.IconController.IconURL("View");
						img = string.Format("<a href='{0}'><img src='{1}' border=\"0\" alt=\"{2}\" title=\"{2}\" /></a>", eventInfoHelper.GetDetailPageRealURL(eventInfo.EventID, eventInfo.SocialGroupId, eventInfo.SocialUserId), imgurl, Localization.GetString("ViewEvent", LocalResourceFile));
						dict.Add("viewicon", img);
					}
				}
				else
				{
					dict.Add("viewicon", "");
				}
				
				//Createdby
				//TokenCreatedByLabel.Text   Created by, Created by ID, Created by Link
				EventUser objEventUser = eventInfoHelper.UserDisplayNameProfile(eventInfo.CreatedByID, eventInfo.CreatedBy, LocalResourceFile);
				dict.Add("createdbylabel", Localization.GetString("TokenCreatedByLabel", LocalResourceFile));
				dict.Add("createdby", objEventUser.DisplayName);
				dict.Add("createdbyid", objEventUser.UserID);
				dict.Add("createdbyurl", objEventUser.ProfileURL);
				dict.Add("createdbyprofile", objEventUser.DisplayNameURL);
				
				//ownedby
				//TokenOwnedByLabel.Text   Owned by, OwnerID, Owned by Link
				objEventUser = eventInfoHelper.UserDisplayNameProfile(eventInfo.OwnerID, eventInfo.OwnerName, LocalResourceFile);
				dict.Add("ownedbylabel", Localization.GetString("TokenOwnedByLabel", LocalResourceFile));
				dict.Add("ownedby", objEventUser.DisplayName);
				dict.Add("ownedbyid", objEventUser.UserID);
				dict.Add("ownedbyurl", objEventUser.ProfileURL);
				dict.Add("ownedbyprofile", objEventUser.DisplayNameURL);
				
				//LastUpdatedby
				//TokenLastUpdatedByLabel.Text   Last updated by, Last updated ID, Last update by ID
				objEventUser = eventInfoHelper.UserDisplayNameProfile(eventInfo.LastUpdatedID, eventInfo.LastUpdatedBy, LocalResourceFile);
				dict.Add("lastupdatedbylabel", Localization.GetString("TokenLastUpdatedByLabel", LocalResourceFile));
				dict.Add("lastupdatedby", objEventUser.DisplayName);
				dict.Add("lastupdatedbyid", objEventUser.UserID);
				dict.Add("lastupdatedbyurl", objEventUser.ProfileURL);
				dict.Add("lastupdatedbyprofile", objEventUser.DisplayNameURL);
				
				if (settings.Eventsignup && eventInfo.Signups)
				{
					//enrollfeelabel
					//enrollfee
					dict.Add("enrollfeelabel", Localization.GetString("TokenEnrollFeeLabel", LocalResourceFile));
					if (eventInfo.EnrollType == "PAID")
					{
						string tokenEnrollFeePaid = Localization.GetString("TokenEnrollFeePaid", LocalResourceFile).Replace("{0}", "{0:#0.00}");
						dict.Add("enrollfee", string.Format(tokenEnrollFeePaid, eventInfo.EnrollFee, ps.Currency));
					}
					else
					{
						dict.Add("enrollfee", Localization.GetString("TokenEnrollFeeFree", LocalResourceFile));
					}
				}
				else
				{
					dict.Add("enrollfeelabel", "");
					dict.Add("enrollfee", "");
				}
				
				if (settings.Moderateall)
				{
					dict.Add("moderateurl", eventInfoHelper.GetModerateUrl());
				}
				else
				{
					dict.Add("moderateurl", "");
				}
				
				//edit button
				if (isEventEditor)
				{
					string imgurl = Entities.Icons.IconController.IconURL("Edit");
					img = string.Format("<a href=\"{3}\"><img src='{0}' class=\"{2}\" alt='{1}' title='{1}' /></a>", imgurl, Localization.GetString("EditEvent", LocalResourceFile), "EventIconEdit", eventInfoHelper.GetEditURL(eventInfo.EventID, eventInfo.SocialGroupId, eventInfo.SocialUserId));
					dict.Add("editbutton", img);
				}
				else
				{
					dict.Add("editbutton", "");
				}
			}
#endregion
			
#region Helperfunctions
			private struct GetTokenFormatResult
			{
				public string Formatstring;
				public bool Tokenfound;
			}
			
			private GetTokenFormatResult GetTokenFormat(string tokenstring, string token, string customcaption)
			{
				string search1 = string.Format("[{0}:{1}]", customcaption, token);
				string search2 = string.Format("[{0}:{1}|", customcaption, token);
				int starttoken1 = tokenstring.IndexOf(search1, StringComparison.Ordinal);
				int starttoken2 = tokenstring.IndexOf(search2, StringComparison.Ordinal);
				if (starttoken1 == -1 & starttoken2 == -1)
				{
					//Not found
					return new GetTokenFormatResult() {Formatstring = null, Tokenfound = false};
				}
				
				GetTokenFormatResult result = new GetTokenFormatResult();
				result.Tokenfound = true;
				if (starttoken1 == -1)
				{
					int endtoken = System.Convert.ToInt32(tokenstring.Substring(starttoken2).IndexOf("]", StringComparison.Ordinal));
					result.Formatstring = tokenstring.Substring(starttoken2 + search2.Length, endtoken - search2.Length);
				}
				else
				{
					result.Formatstring = "";
				}
				
				return result;
			}
			
#endregion
		}
#endregion
		
	}
