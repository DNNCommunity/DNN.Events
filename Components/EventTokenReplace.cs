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
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Icons;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Entities.Users;
    using DotNetNuke.Security;
    using DotNetNuke.Security.Roles;
    using DotNetNuke.Services.FileSystem;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.Services.Tokens;
    using global::Components;
    using Microsoft.VisualBasic;
    using Globals = DotNetNuke.Common.Globals;

    #region TokenReplaceController Class

    /// <summary>
    ///     Replaces the tokens that are defined for the Event e-mails and views
    /// </summary>
    public class TokenReplaceControllerClass
    {
        #region Constructor

        public TokenReplaceControllerClass(int moduleID, string localResourceFile)
        {
            this.ModuleID = moduleID;
            this.LocalResourceFile = localResourceFile;
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
            return this.TokenReplaceEvent(eventInfo, sourceText, null, false);
        }

        public string TokenReplaceEvent(EventInfo eventInfo, string sourceText, EventSignupsInfo eventSignupsInfo)
        {
            return this.TokenReplaceEvent(eventInfo, sourceText, eventSignupsInfo, false);
        }

        public string TokenReplaceEvent(EventInfo eventInfo, string sourceText, bool addsubmodulename)
        {
            return this.TokenReplaceEvent(eventInfo, sourceText, null, addsubmodulename);
        }

        public string TokenReplaceEvent(EventInfo eventInfo, string sourceText, EventSignupsInfo eventSignupsInfo,
                                        bool addsubmodulename)
        {
            return this.TokenReplaceEvent(eventInfo, sourceText, eventSignupsInfo, addsubmodulename, false);
        }

        public string TokenReplaceEvent(EventInfo eventInfo, string sourceText, EventSignupsInfo eventSignupsInfo,
                                        bool addsubmodulename, bool isEventEditor)
        {
            var dict = new Dictionary<string, object>();

            var cacheKey = "EventsFolderName" + this.ModuleID;
            var folderName = Convert.ToString(DataCache.GetCache(cacheKey));
            if (ReferenceEquals(folderName, null))
            {
                folderName = DesktopModuleController
                    .GetDesktopModuleByModuleName("DNN_Events", eventInfo.PortalID).FolderName;
                DataCache.SetCache(cacheKey, folderName);
            }

            //Module settings
            var settings = EventModuleSettings.GetEventModuleSettings(this.ModuleID, this.LocalResourceFile);


            var trn = new TokenReplace(Scope.DefaultSettings, this.ModuleID);

            //Parameter processing
            sourceText = this.TokenParameters(sourceText, eventInfo, settings);

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
            if (eventInfo.ModuleTitle != null && this.ModuleID != eventInfo.ModuleID)
            {
                dict.Add("subcalendarname", string.Format("({0})", eventInfo.ModuleTitle.Trim()));
                dict.Add("subcalendarnameclean", eventInfo.ModuleTitle.Trim());
            }

            //alldayeventtext
            dict.Add("alldayeventtext", Localization.GetString("TokenAllDayEventText", this.LocalResourceFile));

            //startdatelabel
            dict.Add("startdatelabel", Localization.GetString("TokenStartdateLabel", this.LocalResourceFile));

            //startdate
            sourceText = this.TokenReplaceDate(sourceText, "event", "startdate", eventInfo.EventTimeBegin);

            //enddatelabel
            dict.Add("enddatelabel", Localization.GetString("TokenEnddateLabel", this.LocalResourceFile));

            //enddate
            sourceText = this.TokenReplaceDate(sourceText, "event", "enddate", eventInfo.EventTimeEnd);

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
            dict.Add("durationdayslabel", Localization.GetString("TokenDurationDaysLabel", this.LocalResourceFile));
            dict.Add("durationdays", Convert.ToInt32(Conversion.Int((double) eventInfo.Duration / 1440 + 1)));

            //descriptionlabel
            dict.Add("descriptionlabel", Localization.GetString("TokenDescriptionLabel", this.LocalResourceFile));

            //description
            if (!dict.ContainsKey("description"))
            {
                dict.Add("description", HttpUtility.HtmlDecode(eventInfo.EventDesc));
            }
            sourceText = this.TokenLength(sourceText, "event", "description", dict);

            //categorylabel
            if (!ReferenceEquals(eventInfo.CategoryName, null))
            {
                dict.Add("categorylabel", Localization.GetString("TokenCategoryLabel", this.LocalResourceFile));
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
            if (!ReferenceEquals(eventInfo.LocationName, null))
            {
                dict.Add("locationlabel", Localization.GetString("TokenLocationLabel", this.LocalResourceFile));
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
            var eventLocation = new EventLocationInfo();
            eventLocation = new EventLocationController().EventsLocationGet(eventInfo.Location, eventInfo.PortalID);
            if (eventLocation != null)
            {
                dict.Add("locationaddresslabel",
                         Localization.GetString("TokenLocationAddressLabel", this.LocalResourceFile));
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
                dict.Add("customfield1label", Localization.GetString("TokenCustomField1Label", this.LocalResourceFile));
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
                dict.Add("customfield2label", Localization.GetString("TokenCustomField2Label", this.LocalResourceFile));
            }
            else
            {
                dict.Add("customfield2label", "");
            }

            //descriptionlabel
            dict.Add("summarylabel", Localization.GetString("TokenSummaryLabel", this.LocalResourceFile));

            //description
            if (!dict.ContainsKey("summary"))
            {
                dict.Add("summary", HttpUtility.HtmlDecode(eventInfo.Summary));
            }
            sourceText = this.TokenLength(sourceText, "event", "summary", dict);

            //eventid
            dict.Add("eventid", eventInfo.EventID);

            //eventmoduleid
            dict.Add("eventmoduleid", eventInfo.ModuleID);


            //Createddate
            //TokenCreatedOnLabel.Text   on
            dict.Add("createddatelabel", Localization.GetString("TokenCreatedOnLabel", this.LocalResourceFile));
            sourceText = this.TokenReplaceDate(sourceText, "event", "createddate", eventInfo.CreatedDate);

            //LastUpdateddate
            //TokenLastUpdatedOnLabel.Text   Last updated on
            dict.Add("lastupdateddatelabel", Localization.GetString("TokenLastUpdatedOnLabel", this.LocalResourceFile));
            sourceText = this.TokenReplaceDate(sourceText, "event", "lastupdateddate", eventInfo.LastUpdatedAt);

            if (settings.Eventsignup && eventInfo.Signups)
            {
                //maxenrollmentslabel
                //maxenrollments
                dict.Add("maxenrollmentslabel",
                         Localization.GetString("TokenMaxEnrollmentsLabel", this.LocalResourceFile));
                if (eventInfo.MaxEnrollment > 0)
                {
                    dict.Add("maxenrollments", eventInfo.MaxEnrollment.ToString());
                }
                else
                {
                    dict.Add("maxenrollments", Localization.GetString("Unlimited", this.LocalResourceFile));
                }

                //noenrollmentslabel
                //noenrollments
                dict.Add("noenrollmentslabel",
                         Localization.GetString("TokenNoEnrollmentsLabel", this.LocalResourceFile));
                dict.Add("noenrollments", eventInfo.Enrolled.ToString());

                //novacancieslabel
                //novacancies
                dict.Add("novacancieslabel", Localization.GetString("TokenNoVacanciesLabel", this.LocalResourceFile));
                if (eventInfo.MaxEnrollment > 0)
                {
                    dict.Add("novacancies", (eventInfo.MaxEnrollment - eventInfo.Enrolled).ToString());
                }
                else
                {
                    dict.Add("novacancies", Localization.GetString("Unlimited", this.LocalResourceFile));
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
                         Localization.GetString("TokenSocialGroupRoleNameLabel", this.LocalResourceFile));
                dict.Add("socialgrouprolename", rolename);
                dict.Add("socialgrouproleid", eventInfo.SocialGroupId.ToString());
            }

            if (eventInfo.SocialUserUserName != null)
            {
                //socialuserusernamelabel
                //socialuserusername
                dict.Add("socialuserusernamelabel",
                         Localization.GetString("TokenSocialUserUserNameLabel", this.LocalResourceFile));
                dict.Add("socialuserusername", eventInfo.SocialUserUserName);
            }

            if (eventInfo.SocialUserDisplayName != null)
            {
                //socialuserdisplaynamelabel
                //socialuserdisplayname
                dict.Add("socialuserdisplaynamelabel",
                         Localization.GetString("TokenSocialUserDisplayNameLabel", this.LocalResourceFile));
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
                dict.Add("signupdatelabel", Localization.GetString("TokenSignupdateLabel", this.LocalResourceFile));

                //signupdate
                sourceText = this.TokenReplaceDate(sourceText, "event", "signupdate",
                                                   eventSignupsInfo.PayPalPaymentDate);

                //noenroleeslabel
                dict.Add("noenroleeslabel", Localization.GetString("TokenNoenroleesLabel", this.LocalResourceFile));

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
                this.TokenReplacewithPortalSettings(ps, eventInfo, settings, dict, folderName, sourceText,
                                                    isEventEditor);
            }

            return trn.ReplaceEnvironmentTokens(sourceText, dict, "event");
        }

        private string TokenLength(string sourceText, string customCaption, string customToken,
                                   Dictionary<string, object> dict)
        {
            var trn = new TokenReplace(Scope.DefaultSettings, this.ModuleID);
            var tokenText =
                Convert.ToString(
                    trn.ReplaceEnvironmentTokens("[" + customCaption + ":" + customToken + "]", dict, customCaption));
            while (sourceText.IndexOf("[" + customCaption + ":" + customToken + "]") + 1 > 0 ||
                   sourceText.IndexOf("[" + customCaption + ":" + customToken + "|") + 1 > 0)
            {
                var with_1 = this.GetTokenFormat(sourceText, customToken, customCaption);
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
                var with_1 = this.GetTokenFormat(sourceText, customToken, customCaption);
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
                sourceText = this.TokenOneParameter(sourceText, "ALLDAYEVENT", true);
                sourceText = this.TokenOneParameter(sourceText, "NOTALLDAYEVENT", false);
            }
            else
            {
                sourceText = this.TokenOneParameter(sourceText, "ALLDAYEVENT", false);
                sourceText = this.TokenOneParameter(sourceText, "NOTALLDAYEVENT", true);
            }
            if (eventInfo.DisplayEndDate)
            {
                sourceText = this.TokenOneParameter(sourceText, "DISPLAYENDDATE", true);
            }
            else
            {
                sourceText = this.TokenOneParameter(sourceText, "DISPLAYENDDATE", false);
            }

            object eventimagesetting = settings.Eventimage;
            var eventimagebool = false;
            if (bool.TryParse(Convert.ToString(eventimagesetting), out eventimagebool) && eventimagebool &&
                eventInfo.ImageDisplay)
            {
                sourceText = this.TokenOneParameter(sourceText, "IFHASIMAGE", true);
                sourceText = this.TokenOneParameter(sourceText, "IFNOTHASIMAGE", false);
            }
            else
            {
                sourceText = this.TokenOneParameter(sourceText, "IFHASIMAGE", false);
                sourceText = this.TokenOneParameter(sourceText, "IFNOTHASIMAGE", true);
            }

            if (eventInfo.Category > 0)
            {
                sourceText = this.TokenOneParameter(sourceText, "IFHASCATEGORY", true);
            }
            else
            {
                sourceText = this.TokenOneParameter(sourceText, "IFHASCATEGORY", false);
            }
            if (eventInfo.Location > 0)
            {
                sourceText = this.TokenOneParameter(sourceText, "IFHASLOCATION", true);
            }
            else
            {
                sourceText = this.TokenOneParameter(sourceText, "IFHASLOCATION", false);
            }
            if (eventInfo.MapURL != "")
            {
                sourceText = this.TokenOneParameter(sourceText, "IFHASLOCATIONURL", true);
            }
            else
            {
                sourceText = this.TokenOneParameter(sourceText, "IFHASLOCATIONURL", false);
            }
            if (eventInfo.MapURL == "")
            {
                sourceText = this.TokenOneParameter(sourceText, "IFNOTHASLOCATIONURL", true);
            }
            else
            {
                sourceText = this.TokenOneParameter(sourceText, "IFNOTHASLOCATIONURL", false);
            }
            if (settings.Eventsignup && eventInfo.Signups)
            {
                sourceText = this.TokenOneParameter(sourceText, "IFALLOWSENROLLMENTS", true);
            }
            else
            {
                sourceText = this.TokenOneParameter(sourceText, "IFALLOWSENROLLMENTS", false);
            }
            if (settings.EventsCustomField1)
            {
                sourceText = this.TokenOneParameter(sourceText, "DISPLAYCUSTOMFIELD1", true);
            }
            else
            {
                sourceText = this.TokenOneParameter(sourceText, "DISPLAYCUSTOMFIELD1", false);
            }
            if (settings.EventsCustomField2)
            {
                sourceText = this.TokenOneParameter(sourceText, "DISPLAYCUSTOMFIELD2", true);
            }
            else
            {
                sourceText = this.TokenOneParameter(sourceText, "DISPLAYCUSTOMFIELD2", false);
            }
            if (settings.DetailPageAllowed && eventInfo.DetailPage)
            {
                sourceText = this.TokenOneParameter(sourceText, "CUSTOMDETAILPAGE", true);
            }
            else
            {
                sourceText = this.TokenOneParameter(sourceText, "CUSTOMDETAILPAGE", false);
            }
            if (eventInfo.EventTimeBegin.Date == eventInfo.EventTimeEnd.Date) //one day event...
            {
                sourceText = this.TokenOneParameter(sourceText, "ONEDAYEVENT", true);
                sourceText = this.TokenOneParameter(sourceText, "NOTONEDAYEVENT", false);
            }
            else
            {
                sourceText = this.TokenOneParameter(sourceText, "ONEDAYEVENT", false);
                sourceText = this.TokenOneParameter(sourceText, "NOTONEDAYEVENT", true);
            }
            if (eventInfo.RRULE != "") //recurring event
            {
                sourceText = this.TokenOneParameter(sourceText, "RECURRINGEVENT", true);
                sourceText = this.TokenOneParameter(sourceText, "NOTRECURRINGEVENT", false);
            }
            else
            {
                sourceText = this.TokenOneParameter(sourceText, "RECURRINGEVENT", false);
                sourceText = this.TokenOneParameter(sourceText, "NOTRECURRINGEVENT", true);
            }
            if (eventInfo.IsPrivate) //Is private event
            {
                sourceText = this.TokenOneParameter(sourceText, "PRIVATE", true);
                sourceText = this.TokenOneParameter(sourceText, "NOTPRIVATE", false);
            }
            else
            {
                sourceText = this.TokenOneParameter(sourceText, "PRIVATE", false);
                sourceText = this.TokenOneParameter(sourceText, "NOTPRIVATE", true);
            }
            if (settings.Tzdisplay)
            {
                sourceText = this.TokenOneParameter(sourceText, "IFTIMEZONEDISPLAY", true);
            }
            else
            {
                sourceText = this.TokenOneParameter(sourceText, "IFTIMEZONEDISPLAY", false);
            }
            if (eventInfo.Duration > 1440)
            {
                sourceText = this.TokenOneParameter(sourceText, "IFMULTIDAY", true);
                sourceText = this.TokenOneParameter(sourceText, "IFNOTMULTIDAY", false);
            }
            else
            {
                sourceText = this.TokenOneParameter(sourceText, "IFMULTIDAY", false);
                sourceText = this.TokenOneParameter(sourceText, "IFNOTMULTIDAY", true);
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
                    sourceText = this.TokenOneParameter(sourceText, "HASROLE_" + role.RoleName,
                                                        userRoles.Contains(role.RoleName));
                    sourceText = this.TokenOneParameter(sourceText, "HASNOTROLE_" + role.RoleName,
                                                        !userRoles.Contains(role.RoleName));
                }
            }

            if (eventInfo.Summary != "")
            {
                sourceText = this.TokenOneParameter(sourceText, "IFHASSUMMARY", true);
            }
            else
            {
                sourceText = this.TokenOneParameter(sourceText, "IFHASSUMMARY", false);
            }
            if (eventInfo.Summary == "")
            {
                sourceText = this.TokenOneParameter(sourceText, "IFNOTHASSUMMARY", true);
            }
            else
            {
                sourceText = this.TokenOneParameter(sourceText, "IFNOTHASSUMMARY", false);
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
                sourceText = this.TokenOneParameter(sourceText, "IFENROLED", blEnroled);
                sourceText = this.TokenOneParameter(sourceText, "IFNOTENROLED", blNotEnroled);
            }

            if (eventInfo.SocialUserId > 0)
            {
                sourceText = this.TokenOneParameter(sourceText, "IFISSOCIALUSER", true);
            }
            else
            {
                sourceText = this.TokenOneParameter(sourceText, "IFISSOCIALUSER", false);
            }

            if (eventInfo.SocialGroupId > 0)
            {
                sourceText = this.TokenOneParameter(sourceText, "IFISSOCIALGROUP", true);
            }
            else
            {
                sourceText = this.TokenOneParameter(sourceText, "IFISSOCIALGROUP", false);
            }

            if (eventInfo.MaxEnrollment > 0 && eventInfo.Enrolled >= eventInfo.MaxEnrollment)
            {
                sourceText = this.TokenOneParameter(sourceText, "IFISFULL", true);
            }
            else
            {
                sourceText = this.TokenOneParameter(sourceText, "IFISFULL", false);
            }
            if (eventInfo.MaxEnrollment > 0 && eventInfo.Enrolled < eventInfo.MaxEnrollment ||
                eventInfo.MaxEnrollment == 0)
            {
                sourceText = this.TokenOneParameter(sourceText, "IFNOTISFULL", true);
            }
            else
            {
                sourceText = this.TokenOneParameter(sourceText, "IFNOTISFULL", false);
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
            var eventInfoHelper = new EventInfoHelper(this.ModuleID, ps.ActiveTab.TabID, eventInfo.PortalID, settings);

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
            dict.Add("importancelabel", Localization.GetString("TokenImporatanceLabel", this.LocalResourceFile));

            //importance, importanceicon
            var result = "<img src='{0}{1}' class=\"{4}\" alt='{2}' /> {3}";
            switch (eventInfo.Importance)
            {
                case EventInfo.Priority.High:
                    dict.Add("importance",
                             string.Format(result, imagepath, "HighPrio.gif",
                                           Localization.GetString("HighPrio", this.LocalResourceFile),
                                           Localization.GetString("HighPrio", this.LocalResourceFile),
                                           "EventIconHigh"));
                    dict.Add("importanceicon",
                             string.Format(result, imagepath, "HighPrio.gif",
                                           Localization.GetString("HighPrio", this.LocalResourceFile), "",
                                           "EventIconHigh"));
                    break;
                case EventInfo.Priority.Low:
                    dict.Add("importance",
                             string.Format(result, imagepath, "LowPrio.gif",
                                           Localization.GetString("LowPrio", this.LocalResourceFile),
                                           Localization.GetString("LowPrio", this.LocalResourceFile), "EventIconLow"));
                    dict.Add("importanceicon",
                             string.Format(result, imagepath, "LowPrio.gif",
                                           Localization.GetString("HighPrio", this.LocalResourceFile), "",
                                           "EventIconLow"));
                    break;
                case EventInfo.Priority.Medium:
                    dict.Add("importance", Localization.GetString("NormPrio", this.LocalResourceFile));
                    dict.Add("importanceicon", "");
                    break;
            }

            //reminderlabel
            dict.Add("reminderlabel", Localization.GetString("TokenReminderLabel", this.LocalResourceFile));

            //reminder, remindericon
            var img = "";
            if (!ReferenceEquals(sourceText, null))
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
                                                                      this.LocalResourceFile,
                                                                      eventInfo.EventTimeZoneId);
                    }
                    if (eventInfo.SendReminder && HttpContext.Current.Request.IsAuthenticated &&
                        !string.IsNullOrEmpty(notificationInfo))
                    {
                        img = string.Format("<img src='{0}bell.gif' class=\"{2}\" alt='{1}' />", imagepath,
                                            Localization.GetString("ReminderEnabled", this.LocalResourceFile) + ": " +
                                            notificationInfo, "EventIconRem");
                    }
                    else if (eventInfo.SendReminder &&
                             (settings.Notifyanon || HttpContext.Current.Request.IsAuthenticated))
                    {
                        img = string.Format("<img src='{0}bell.gif' class=\"{2}\" alt='{1}' />", imagepath,
                                            Localization.GetString("ReminderEnabled", this.LocalResourceFile),
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
                                        Localization.GetString("EnrollEnabled", this.LocalResourceFile),
                                        "EventIconEnroll");
                }
                else
                {
                    img = string.Format("<img src='{0}EnrollFull.gif' class=\"{2}\" alt='{1}' />", imagepath,
                                        Localization.GetString("EnrollFull", this.LocalResourceFile),
                                        "EventIconEnrollFull");
                }
            }
            dict.Add("enrollicon", img);

            //recurringlabel
            dict.Add("recurringlabel", Localization.GetString("TokenRecurranceLabel", this.LocalResourceFile));

            //recurring, recurringicon
            var objEventRRULE = default(EventRRULEInfo);
            var objCtlEventRecurMaster = new EventRecurMasterController();
            objEventRRULE = objCtlEventRecurMaster.DecomposeRRULE(eventInfo.RRULE, eventInfo.EventTimeBegin);
            result = objCtlEventRecurMaster.RecurrenceText(objEventRRULE, this.LocalResourceFile,
                                                           Thread.CurrentThread.CurrentCulture,
                                                           eventInfo.EventTimeBegin);
            img = "";
            if (eventInfo.RRULE != "")
            {
                img = string.Format("<img src='{0}rec.gif' class=\"{2}\" alt='{1}' />", imagepath,
                                    Localization.GetString("RecurringEvent", this.LocalResourceFile), "EventIconRec");
                result = img + " " + result + " " +
                         objCtlEventRecurMaster.RecurrenceInfo(eventInfo, this.LocalResourceFile);
            }
            dict.Add("recurring", result);
            dict.Add("recurringicon", img);

            //titleurl
            var eventurl = eventInfoHelper.DetailPageURL(eventInfo);
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
                                        Localization.GetString("ViewEvent", this.LocalResourceFile));
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
                                                       this.LocalResourceFile);
            dict.Add("createdbylabel", Localization.GetString("TokenCreatedByLabel", this.LocalResourceFile));
            dict.Add("createdby", objEventUser.DisplayName);
            dict.Add("createdbyid", objEventUser.UserID);
            dict.Add("createdbyurl", objEventUser.ProfileURL);
            dict.Add("createdbyprofile", objEventUser.DisplayNameURL);

            //ownedby
            //TokenOwnedByLabel.Text   Owned by, OwnerID, Owned by Link
            objEventUser =
                eventInfoHelper.UserDisplayNameProfile(eventInfo.OwnerID, eventInfo.OwnerName, this.LocalResourceFile);
            dict.Add("ownedbylabel", Localization.GetString("TokenOwnedByLabel", this.LocalResourceFile));
            dict.Add("ownedby", objEventUser.DisplayName);
            dict.Add("ownedbyid", objEventUser.UserID);
            dict.Add("ownedbyurl", objEventUser.ProfileURL);
            dict.Add("ownedbyprofile", objEventUser.DisplayNameURL);

            //LastUpdatedby
            //TokenLastUpdatedByLabel.Text   Last updated by, Last updated ID, Last update by ID
            objEventUser =
                eventInfoHelper.UserDisplayNameProfile(eventInfo.LastUpdatedID, eventInfo.LastUpdatedBy,
                                                       this.LocalResourceFile);
            dict.Add("lastupdatedbylabel", Localization.GetString("TokenLastUpdatedByLabel", this.LocalResourceFile));
            dict.Add("lastupdatedby", objEventUser.DisplayName);
            dict.Add("lastupdatedbyid", objEventUser.UserID);
            dict.Add("lastupdatedbyurl", objEventUser.ProfileURL);
            dict.Add("lastupdatedbyprofile", objEventUser.DisplayNameURL);

            if (settings.Eventsignup && eventInfo.Signups)
            {
                //enrollfeelabel
                //enrollfee
                dict.Add("enrollfeelabel", Localization.GetString("TokenEnrollFeeLabel", this.LocalResourceFile));
                if (eventInfo.EnrollType == "PAID")
                {
                    var tokenEnrollFeePaid = Localization
                        .GetString("TokenEnrollFeePaid", this.LocalResourceFile).Replace("{0}", "{0:#0.00}");
                    dict.Add("enrollfee", string.Format(tokenEnrollFeePaid, eventInfo.EnrollFee, ps.Currency));
                }
                else
                {
                    dict.Add("enrollfee", Localization.GetString("TokenEnrollFeeFree", this.LocalResourceFile));
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
                                    Localization.GetString("EditEvent", this.LocalResourceFile), "EventIconEdit",
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