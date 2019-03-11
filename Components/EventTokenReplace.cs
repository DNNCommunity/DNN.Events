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
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web;
    using System.Web.UI.WebControls;
    using Common.Utilities;
    using Entities.Icons;
    using DotNetNuke.Entities.Modules;
    using Entities.Portals;
    using Entities.Users;
    using Security;
    using Security.Roles;
    using Services.FileSystem;
    using Services.Localization;
    using Services.Tokens;
    using global::Components;
    using Microsoft.VisualBasic;
    using Globals = Common.Globals;

    #region TokenReplaceController Class

    /// <summary>
    ///     Replaces the tokens that are defined for the Event e-mails and views
    /// </summary>
    public class TokenReplaceControllerClass
    {
        #region Constructor

        public TokenReplaceControllerClass(int moduleID, string localResourceFile)
        {
            ModuleID = moduleID;
            LocalResourceFile = localResourceFile;
        }

        #endregion

        #region Member Variables

        #endregion

        #region Properties

        private int ModuleID { get; }

        private string LocalResourceFile { get; }

        #endregion

        #region Methods

        /// <summary>
        ///     Replace tokens in sourcetext with eventspecific token
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

        public string TokenReplaceEvent(EventInfo eventInfo, string sourceText, EventSignupsInfo eventSignupsInfo,
                                        bool addsubmodulename)
        {
            return TokenReplaceEvent(eventInfo, sourceText, eventSignupsInfo, addsubmodulename, false);
        }

        public string TokenReplaceEvent(EventInfo eventInfo, string sourceText, EventSignupsInfo eventSignupsInfo,
                                        bool addsubmodulename, bool isEventEditor)
        {
            var dict = new Dictionary<string, object>();

            var cacheKey = "EventsFolderName" + ModuleID;
            var folderName = Convert.ToString(DataCache.GetCache(cacheKey));
            if (string.IsNullOrEmpty(folderName))
            {
                folderName = DesktopModuleController
                    .GetDesktopModuleByModuleName("DNN_Events", eventInfo.PortalID).FolderName;
                DataCache.SetCache(cacheKey, folderName);
            }

            //Module settings
            var settings = EventModuleSettings.GetEventModuleSettings(ModuleID, LocalResourceFile);


            var trn = new TokenReplace(Scope.DefaultSettings, ModuleID);

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
                var value = TimeZoneInfo.FindSystemTimeZoneById(eventInfo.EventTimeZoneId).DisplayName;
                dict.Add("timezone", value);
            }
            catch
            {
                dict.Add("timezone", "");
            }

            //Duration
            dict.Add("durationdayslabel", Localization.GetString("TokenDurationDaysLabel", LocalResourceFile));
            dict.Add("durationdays", Convert.ToInt32(Conversion.Int((double) eventInfo.Duration / 1440 + 1)));

            //descriptionlabel
            dict.Add("descriptionlabel", Localization.GetString("TokenDescriptionLabel", LocalResourceFile));

            //description
            if (!dict.ContainsKey("description"))
            {
                dict.Add("description", HttpUtility.HtmlDecode(eventInfo.EventDesc));
            }
            sourceText = TokenLength(sourceText, "event", "description", dict);

            //categorylabel
            if (!string.IsNullOrEmpty(eventInfo.CategoryName))
            {
                dict.Add("categorylabel", Localization.GetString("TokenCategoryLabel", LocalResourceFile));
            }
            else
            {
                dict.Add("categorylabel", "");
            }

            //category, categoryfontcolor, categorybackcolor
            if (!string.IsNullOrEmpty(eventInfo.CategoryName))
            {
                if (eventInfo.Color.Length > 0)
                {
                    dict.Add("category",
                             string.Format("<div style='background-color:{1};color:{2}'>{0}</div>",
                                           eventInfo.CategoryName, eventInfo.Color, eventInfo.FontColor));
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
            if (!string.IsNullOrEmpty(eventInfo.LocationName))
            {
                dict.Add("locationlabel", Localization.GetString("TokenLocationLabel", LocalResourceFile));
            }
            else
            {
                dict.Add("locationlabel", "");
            }

            //location, locationurl
            if (!string.IsNullOrEmpty(eventInfo.MapURL) && eventInfo.MapURL != "")
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
            var eventLocation = new EventLocationInfo();
            eventLocation = new EventLocationController().EventsLocationGet(eventInfo.Location, eventInfo.PortalID);
            if (eventLocation != null)
            {
                dict.Add("locationaddresslabel",
                         Localization.GetString("TokenLocationAddressLabel", LocalResourceFile));
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
                dict.Add("maxenrollmentslabel",
                         Localization.GetString("TokenMaxEnrollmentsLabel", LocalResourceFile));
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
                dict.Add("noenrollmentslabel",
                         Localization.GetString("TokenNoEnrollmentsLabel", LocalResourceFile));
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
                var roleController = new RoleController();
                var rolename = roleController.GetRole(eventInfo.SocialGroupId, eventInfo.PortalID).RoleName;
                dict.Add("socialgrouprolenamelabel",
                         Localization.GetString("TokenSocialGroupRoleNameLabel", LocalResourceFile));
                dict.Add("socialgrouprolename", rolename);
                dict.Add("socialgrouproleid", eventInfo.SocialGroupId.ToString());
            }

            if (eventInfo.SocialUserUserName != null)
            {
                //socialuserusernamelabel
                //socialuserusername
                dict.Add("socialuserusernamelabel",
                         Localization.GetString("TokenSocialUserUserNameLabel", LocalResourceFile));
                dict.Add("socialuserusername", eventInfo.SocialUserUserName);
            }

            if (eventInfo.SocialUserDisplayName != null)
            {
                //socialuserdisplaynamelabel
                //socialuserdisplayname
                dict.Add("socialuserdisplaynamelabel",
                         Localization.GetString("TokenSocialUserDisplayNameLabel", LocalResourceFile));
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
                sourceText = TokenReplaceDate(sourceText, "event", "signupdate",
                                                   eventSignupsInfo.PayPalPaymentDate);

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
                ps = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
            }
            catch (Exception)
            { }
            if (!ReferenceEquals(ps, null))
            {
                // add tokens for items that use PortalSettings
                TokenReplacewithPortalSettings(ps, eventInfo, settings, dict, folderName, sourceText,
                                                    isEventEditor);
            }

            return trn.ReplaceEnvironmentTokens(sourceText, dict, "event");
        }

        private string TokenLength(string sourceText, string customCaption, string customToken,
                                   Dictionary<string, object> dict)
        {
            var trn = new TokenReplace(Scope.DefaultSettings, ModuleID);
            var tokenText =
                Convert.ToString(
                    trn.ReplaceEnvironmentTokens("[" + customCaption + ":" + customToken + "]", dict, customCaption));
            while (sourceText.IndexOf("[" + customCaption + ":" + customToken + "]") + 1 > 0 ||
                   sourceText.IndexOf("[" + customCaption + ":" + customToken + "|") + 1 > 0)
            {
                var with_1 = GetTokenFormat(sourceText, customToken, customCaption);
                if (with_1.Tokenfound)
                {
                    if (!string.IsNullOrEmpty(with_1.Formatstring))
                    {
                        sourceText =
                            sourceText.Replace(
                                "[" + customCaption + ":" + customToken + "|" + with_1.Formatstring + "]",
                                HtmlUtils.Shorten(tokenText, int.Parse(with_1.Formatstring), "..."));
                    }
                    else
                    {
                        sourceText = sourceText.Replace("[" + customCaption + ":" + customToken + "]", tokenText);
                    }
                }
            }
            return sourceText;
        }

        private string TokenReplaceDate(string sourceText, string customCaption, string customToken,
                                        DateTime customDate)
        {
            var tokenDate = customDate.Date.AddMinutes(customDate.TimeOfDay.TotalMinutes);
            while (sourceText.IndexOf("[" + customCaption + ":" + customToken + "]") + 1 > 0 ||
                   sourceText.IndexOf("[" + customCaption + ":" + customToken + "|") + 1 > 0)
            {
                var with_1 = GetTokenFormat(sourceText, customToken, customCaption);
                if (with_1.Tokenfound)
                {
                    if (!string.IsNullOrEmpty(with_1.Formatstring))
                    {
                        sourceText =
                            sourceText.Replace(
                                "[" + customCaption + ":" + customToken + "|" + with_1.Formatstring + "]",
                                string.Format("{0:" + with_1.Formatstring + "}", tokenDate));
                    }
                    else
                    {
                        sourceText = sourceText.Replace("[" + customCaption + ":" + customToken + "]",
                                                        string.Format("{0:f}", tokenDate));
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
            var eventimagebool = false;
            if (bool.TryParse(Convert.ToString(eventimagesetting), out eventimagebool) && eventimagebool &&
                eventInfo.ImageDisplay)
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
            if (string.IsNullOrEmpty(eventInfo.MapURL))
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
                var role = default(RoleInfo);
                var roleController = new RoleController();
                var userRolesIList = default(IList<UserRoleInfo>);
                var userRoles = new ArrayList();
                if (!ReferenceEquals(UserController.GetCurrentUserInfo(), null))
                {
                    userRolesIList = roleController.GetUserRoles(UserController.GetCurrentUserInfo(), true);
                    // ReSharper disable NotAccessedVariable
                    var i = 0;
                    // ReSharper restore NotAccessedVariable
                    foreach (var userRole in userRolesIList)
                    {
                        userRoles.Add(userRole.RoleName);
                        i++;
                    }
                }
                foreach (RoleInfo tempLoopVar_role in roleController.GetPortalRoles(eventInfo.PortalID))
                {
                    role = tempLoopVar_role;
                    sourceText = TokenOneParameter(sourceText, "HASROLE_" + role.RoleName,
                                                        userRoles.Contains(role.RoleName));
                    sourceText = TokenOneParameter(sourceText, "HASNOTROLE_" + role.RoleName,
                                                        !userRoles.Contains(role.RoleName));
                }
            }

            if (string.IsNullOrEmpty(eventInfo.Summary))
            {
                sourceText = TokenOneParameter(sourceText, "IFHASSUMMARY", false);
            }
            else
            {
                sourceText = TokenOneParameter(sourceText, "IFHASSUMMARY", true);
            }

            if (string.IsNullOrEmpty(eventInfo.Summary))
            {
                sourceText = TokenOneParameter(sourceText, "IFNOTHASSUMMARY", true);
            }
            else
            {
                sourceText = TokenOneParameter(sourceText, "IFNOTHASSUMMARY", false);
            }

            if (sourceText.Contains("[IFENROLED]") || sourceText.Contains("[IFNOTENROLED]"))
            {
                var blEnroled = false;
                var blNotEnroled = false;
                if (eventInfo.Signups)
                {
                    blNotEnroled = true;
                    if (!ReferenceEquals(UserController.GetCurrentUserInfo(), null))
                    {
                        var signupsController = new EventSignupsController();
                        var signupInfo =
                            signupsController.EventsSignupsGetUser(eventInfo.EventID,
                                                                   UserController.GetCurrentUserInfo().UserID,
                                                                   eventInfo.ModuleID);
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
            if (eventInfo.MaxEnrollment > 0 && eventInfo.Enrolled < eventInfo.MaxEnrollment ||
                eventInfo.MaxEnrollment == 0)
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
            var sourceTextOut = new StringBuilder();
            if (parameterKeep)
            {
                sourceTextOut.Insert(0, sourceText);
                sourceTextOut.Replace("[" + parameterName + "]", "");
                sourceTextOut.Replace("[/" + parameterName + "]", "");
            }
            else
            {
                var rgx = new Regex("\\[" + parameterName + "][.\\s\\S]*?\\[/" + parameterName + "]");
                sourceTextOut.Insert(0, rgx.Replace(sourceText, ""));
            }
            return sourceTextOut.ToString();
        }

        private void TokenReplacewithPortalSettings(PortalSettings ps, EventInfo eventInfo,
                                                    EventModuleSettings settings, Dictionary<string, object> dict,
                                                    string folderName, string sourceText, bool isEventEditor)
        {
            //Build URL for event images
            var eventInfoHelper = new EventInfoHelper(ModuleID, ps.ActiveTab.TabID, eventInfo.PortalID, settings);

            // Dim portalurl As String = ps.PortalAlias.HTTPAlias
            // Dim domainurl As String = ps.PortalAlias.HTTPAlias
            var domainurl = eventInfoHelper.GetDomainURL();
            var portalurl = domainurl;
            if (ps.PortalAlias.HTTPAlias.IndexOf("/", StringComparison.Ordinal) > 0)
            {
                portalurl = portalurl + Globals.ApplicationPath;
            }
            var imagepath = Globals.AddHTTP(string.Format("{0}/DesktopModules/{1}/Images/", portalurl, folderName));

            //eventimage
            if (settings.Eventimage && eventInfo.ImageDisplay)
            {
                var imageSrc = eventInfo.ImageURL;

                if (eventInfo.ImageURL.StartsWith("FileID="))
                {
                    var fileId = int.Parse(eventInfo.ImageURL.Substring(7));
                    var objFileInfo = FileManager.Instance.GetFile(fileId);
                    if (!ReferenceEquals(objFileInfo, null))
                    {
                        imageSrc = Convert.ToString(objFileInfo.Folder + objFileInfo.FileName);
                        if (imageSrc.IndexOf("://") + 1 == 0)
                        {
                            var pi = new PortalController();
                            imageSrc = Globals.AddHTTP(
                                string.Format("{0}/{1}/{2}", portalurl, pi.GetPortal(eventInfo.PortalID).HomeDirectory,
                                              imageSrc));
                        }
                    }
                }

                if ((eventInfo.ImageWidth > 0) & (eventInfo.ImageHeight > 0))
                {
                    dict.Add("eventimage",
                             string.Format("<img src='{0}' alt='' width='{1}' height='{2}' />", imageSrc,
                                           Unit.Pixel(eventInfo.ImageWidth), Unit.Pixel(eventInfo.ImageHeight)));
                    var thumbWidth = eventInfo.ImageWidth;
                    var thumbHeight = eventInfo.ImageHeight;
                    if (eventInfo.ImageHeight > settings.MaxThumbHeight)
                    {
                        thumbHeight = settings.MaxThumbHeight;
                        thumbWidth = Convert.ToInt32((double) eventInfo.ImageWidth * settings.MaxThumbHeight /
                                                     eventInfo.ImageHeight);
                    }
                    if (thumbWidth > settings.MaxThumbWidth)
                    {
                        thumbWidth = settings.MaxThumbWidth;
                        thumbHeight = Convert.ToInt32((double) eventInfo.ImageHeight * settings.MaxThumbWidth /
                                                      eventInfo.ImageWidth);
                    }
                    dict.Add("eventthumb",
                             string.Format("<img src='{0}' alt='' width='{1}' height='{2}' />", imageSrc,
                                           Unit.Pixel(thumbWidth), Unit.Pixel(thumbHeight)));
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
            var result = "<img src='{0}{1}' class=\"{4}\" alt='{2}' /> {3}";
            switch (eventInfo.Importance)
            {
                case EventInfo.Priority.High:
                    dict.Add("importance",
                             string.Format(result, imagepath, "HighPrio.gif",
                                           Localization.GetString("HighPrio", LocalResourceFile),
                                           Localization.GetString("HighPrio", LocalResourceFile),
                                           "EventIconHigh"));
                    dict.Add("importanceicon",
                             string.Format(result, imagepath, "HighPrio.gif",
                                           Localization.GetString("HighPrio", LocalResourceFile), "",
                                           "EventIconHigh"));
                    break;
                case EventInfo.Priority.Low:
                    dict.Add("importance",
                             string.Format(result, imagepath, "LowPrio.gif",
                                           Localization.GetString("LowPrio", LocalResourceFile),
                                           Localization.GetString("LowPrio", LocalResourceFile), "EventIconLow"));
                    dict.Add("importanceicon",
                             string.Format(result, imagepath, "LowPrio.gif",
                                           Localization.GetString("HighPrio", LocalResourceFile), "",
                                           "EventIconLow"));
                    break;
                case EventInfo.Priority.Medium:
                    dict.Add("importance", Localization.GetString("NormPrio", LocalResourceFile));
                    dict.Add("importanceicon", "");
                    break;
            }

            //reminderlabel
            dict.Add("reminderlabel", Localization.GetString("TokenReminderLabel", LocalResourceFile));

            //reminder, remindericon
            var img = "";
            if (!string.IsNullOrEmpty(sourceText))
            {
                if (sourceText.Contains("[event:reminder]") || sourceText.Contains("[event:remindericon]"))
                {
                    var notificationInfo = "";
                    img = "";
                    var userEmail = UserController.GetCurrentUserInfo().Email;
                    if (eventInfo.SendReminder && HttpContext.Current.Request.IsAuthenticated)
                    {
                        var objEventNotificationController = new EventNotificationController();
                        notificationInfo =
                            objEventNotificationController.NotifyInfo(eventInfo.EventID, userEmail, eventInfo.ModuleID,
                                                                      LocalResourceFile,
                                                                      eventInfo.EventTimeZoneId);
                    }
                    if (eventInfo.SendReminder && HttpContext.Current.Request.IsAuthenticated &&
                        !string.IsNullOrEmpty(notificationInfo))
                    {
                        img = string.Format("<img src='{0}bell.gif' class=\"{2}\" alt='{1}' />", imagepath,
                                            Localization.GetString("ReminderEnabled", LocalResourceFile) + ": " +
                                            notificationInfo, "EventIconRem");
                    }
                    else if (eventInfo.SendReminder &&
                             (settings.Notifyanon || HttpContext.Current.Request.IsAuthenticated))
                    {
                        img = string.Format("<img src='{0}bell.gif' class=\"{2}\" alt='{1}' />", imagepath,
                                            Localization.GetString("ReminderEnabled", LocalResourceFile),
                                            "EventIconRem");
                    }
                    dict.Add("reminder", notificationInfo);
                    dict.Add("remindericon", img);
                }
            }
            //enrollicon
            var objEventSignupsController = new EventSignupsController();
            img = "";
            if (objEventSignupsController.DisplayEnrollIcon(eventInfo) && settings.Eventsignup)
            {
                if ((eventInfo.MaxEnrollment == 0) | (eventInfo.Enrolled < eventInfo.MaxEnrollment))
                {
                    img = string.Format("<img src='{0}enroll.gif' class=\"{2}\" alt='{1}' />", imagepath,
                                        Localization.GetString("EnrollEnabled", LocalResourceFile),
                                        "EventIconEnroll");
                }
                else
                {
                    img = string.Format("<img src='{0}EnrollFull.gif' class=\"{2}\" alt='{1}' />", imagepath,
                                        Localization.GetString("EnrollFull", LocalResourceFile),
                                        "EventIconEnrollFull");
                }
            }
            dict.Add("enrollicon", img);

            //recurringlabel
            dict.Add("recurringlabel", Localization.GetString("TokenRecurranceLabel", LocalResourceFile));

            //recurring, recurringicon
            var objEventRRULE = default(EventRRULEInfo);
            var objCtlEventRecurMaster = new EventRecurMasterController();
            objEventRRULE = objCtlEventRecurMaster.DecomposeRRULE(eventInfo.RRULE, eventInfo.EventTimeBegin);
            result = objCtlEventRecurMaster.RecurrenceText(objEventRRULE, LocalResourceFile,
                                                           Thread.CurrentThread.CurrentCulture,
                                                           eventInfo.EventTimeBegin);
            img = "";
            if (eventInfo.RRULE != "")
            {
                img = string.Format("<img src='{0}rec.gif' class=\"{2}\" alt='{1}' />", imagepath,
                                    Localization.GetString("RecurringEvent", LocalResourceFile), "EventIconRec");
                result = img + " " + result + " " +
                         objCtlEventRecurMaster.RecurrenceInfo(eventInfo, LocalResourceFile);
            }
            dict.Add("recurring", result);
            dict.Add("recurringicon", img);

            //titleurl
            var eventurl = eventInfoHelper.DetailPageURL(eventInfo);
            dict.Add("eventurl", eventurl);
            if (eventInfo.DetailPage && eventInfo.DetailNewWin)
            {
                dict.Add("titleurl", "<a href=\"" + eventurl + "\" target=\"_blank\" style=\"color: "+eventInfo.FontColor+ "\">" + eventInfo.EventName + "</a>");
            }
            else
            {
                dict.Add("titleurl", "<a href=\"" + eventurl + "\"style=\"color: " + eventInfo.FontColor + "\">" + eventInfo.EventName + "</a>");
            }

            //View page url
            if (settings.DetailPageAllowed && eventInfo.DetailPage)
            {
                var strUserID = UserController.GetCurrentUserInfo().UserID.ToString();
                var userID = -1;
                if (Information.IsNumeric(strUserID))
                {
                    userID = int.Parse(strUserID);
                }
                var blAuthenticated = false;
                if (userID > -1)
                {
                    blAuthenticated = true;
                }
                if ((eventInfo.CreatedByID == userID) | (eventInfo.OwnerID == userID) |
                    (eventInfo.RmOwnerID == userID) || eventInfoHelper.IsModerator(blAuthenticated) ||
                    PortalSecurity.IsInRole(ps.AdministratorRoleName))
                {
                    var imgurl = IconController.IconURL("View");
                    img = string.Format("<a href='{0}'><img src='{1}' border=\"0\" alt=\"{2}\" title=\"{2}\" /></a>",
                                        eventInfoHelper.GetDetailPageRealURL(
                                            eventInfo.EventID, eventInfo.SocialGroupId, eventInfo.SocialUserId), imgurl,
                                        Localization.GetString("ViewEvent", LocalResourceFile));
                    dict.Add("viewicon", img);
                }
            }
            else
            {
                dict.Add("viewicon", "");
            }

            //Createdby
            //TokenCreatedByLabel.Text   Created by, Created by ID, Created by Link
            var objEventUser =
                eventInfoHelper.UserDisplayNameProfile(eventInfo.CreatedByID, eventInfo.CreatedBy,
                                                       LocalResourceFile);
            dict.Add("createdbylabel", Localization.GetString("TokenCreatedByLabel", LocalResourceFile));
            dict.Add("createdby", objEventUser.DisplayName);
            dict.Add("createdbyid", objEventUser.UserID);
            dict.Add("createdbyurl", objEventUser.ProfileURL);
            dict.Add("createdbyprofile", objEventUser.DisplayNameURL);

            //ownedby
            //TokenOwnedByLabel.Text   Owned by, OwnerID, Owned by Link
            objEventUser =
                eventInfoHelper.UserDisplayNameProfile(eventInfo.OwnerID, eventInfo.OwnerName, LocalResourceFile);
            dict.Add("ownedbylabel", Localization.GetString("TokenOwnedByLabel", LocalResourceFile));
            dict.Add("ownedby", objEventUser.DisplayName);
            dict.Add("ownedbyid", objEventUser.UserID);
            dict.Add("ownedbyurl", objEventUser.ProfileURL);
            dict.Add("ownedbyprofile", objEventUser.DisplayNameURL);

            //LastUpdatedby
            //TokenLastUpdatedByLabel.Text   Last updated by, Last updated ID, Last update by ID
            objEventUser =
                eventInfoHelper.UserDisplayNameProfile(eventInfo.LastUpdatedID, eventInfo.LastUpdatedBy,
                                                       LocalResourceFile);
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
                    var tokenEnrollFeePaid = Localization
                        .GetString("TokenEnrollFeePaid", LocalResourceFile).Replace("{0}", "{0:#0.00}");
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
                var imgurl = IconController.IconURL("Edit");
                img = string.Format("<a href=\"{3}\"><img src='{0}' class=\"{2}\" alt='{1}' title='{1}' /></a>", imgurl,
                                    Localization.GetString("EditEvent", LocalResourceFile), "EventIconEdit",
                                    eventInfoHelper.GetEditURL(eventInfo.EventID, eventInfo.SocialGroupId,
                                                               eventInfo.SocialUserId));
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
            var search1 = string.Format("[{0}:{1}]", customcaption, token);
            var search2 = string.Format("[{0}:{1}|", customcaption, token);
            var starttoken1 = tokenstring.IndexOf(search1, StringComparison.Ordinal);
            var starttoken2 = tokenstring.IndexOf(search2, StringComparison.Ordinal);
            if ((starttoken1 == -1) & (starttoken2 == -1))
            {
                //Not found
                return new GetTokenFormatResult {Formatstring = null, Tokenfound = false};
            }

            var result = new GetTokenFormatResult();
            result.Tokenfound = true;
            if (starttoken1 == -1)
            {
                var endtoken =
                    Convert.ToInt32(tokenstring.Substring(starttoken2).IndexOf("]", StringComparison.Ordinal));
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