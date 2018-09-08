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


namespace Components
{
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using System.Web;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Icons;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Entities.Users;
    using DotNetNuke.Framework;
    using DotNetNuke.Modules.Events;
    using DotNetNuke.Modules.Events.Components.Integration;
    using DotNetNuke.Security;
    using DotNetNuke.Security.Permissions;
    using DotNetNuke.Security.Roles;
    using DotNetNuke.Services.FileSystem;
    using DotNetNuke.Services.Localization;
    using Microsoft.VisualBasic;
    using Constants = Microsoft.VisualBasic.Constants;
    using Globals = DotNetNuke.Common.Globals;

    public class EventBase : PortalModuleBase
    {
        #region Properties

        private DateTime _selectedDate;
        private CultureInfo _currculture;

        public DateTime SelectedDate
        {
            get
                {
                    var objEventTimeZoneUtilities = new EventTimeZoneUtilities();
                    var currDateInfo =
                        objEventTimeZoneUtilities.ConvertFromUTCToDisplayTimeZone(
                            DateTime.UtcNow, GetDisplayTimeZoneId());
                    try
                    {
                        _currculture = Thread.CurrentThread.CurrentCulture;
                        if (_selectedDate.Year == 1)
                        {
                            if (!(Request.Params["selecteddate"] == null))
                            {
                                var strDate = Request.Params["selecteddate"];
                                if (Information.IsDate(strDate))
                                {
                                    _selectedDate = Convert.ToDateTime(strDate);
                                }
                                else
                                {
                                    var invCulture = CultureInfo.InvariantCulture;
                                    try
                                    {
                                        _selectedDate = DateTime.ParseExact(strDate, "yyyyMMdd", invCulture);
                                    }
                                    catch (Exception)
                                    {
                                        _selectedDate = currDateInfo.EventDate;
                                    }
                                }
                            }
                            else if (ReferenceEquals(Request.Cookies["DNNEvents"], null))
                            {
                                _selectedDate = currDateInfo.EventDate;
                            }
                            else if (ReferenceEquals(
                                Request.Cookies["DNNEvents"]
                                    ["EventSelectedDate" + Convert.ToString(ModuleId)], null))
                            {
                                _selectedDate = currDateInfo.EventDate;
                            }
                            else
                            {
                                var cookieDate =
                                    Convert.ToString(
                                        Request.Cookies["DNNEvents"][
                                            "EventSelectedDate" + Convert.ToString(ModuleId)]);
                                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                                if (Convert.ToDateTime(cookieDate).Year == 1)
                                {
                                    _selectedDate = currDateInfo.EventDate;
                                }
                                else
                                {
                                    _selectedDate = Convert.ToDateTime(cookieDate);
                                }
                                Thread.CurrentThread.CurrentCulture = _currculture;
                            }
                        }
                        return _selectedDate;
                    }
                    catch (Exception)
                    {
                        Thread.CurrentThread.CurrentCulture = _currculture;
                        _selectedDate = currDateInfo.EventDate;
                        return _selectedDate;
                    }
                }
            set
                {
                    if (Information.IsDate(value))
                    {
                        _selectedDate = Convert.ToDateTime(value.ToShortDateString());
                    }
                    else
                    {
                        var objEventTimeZoneUtilities = new EventTimeZoneUtilities();
                        _selectedDate = objEventTimeZoneUtilities
                            .ConvertFromUTCToDisplayTimeZone(DateTime.UtcNow, GetDisplayTimeZoneId()).EventDate;
                    }
                    if (_selectedDate.Year == 1)
                    {
                        var objEventTimeZoneUtilities = new EventTimeZoneUtilities();
                        _selectedDate = objEventTimeZoneUtilities
                            .ConvertFromUTCToDisplayTimeZone(DateTime.UtcNow, GetDisplayTimeZoneId()).EventDate;
                    }
                    _currculture = Thread.CurrentThread.CurrentCulture;
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                    Response.Cookies["DNNEvents"]["EventSelectedDate" + Convert.ToString(ModuleId)] =
                        _selectedDate.ToShortDateString();
                    Response.Cookies["DNNEvents"].Expires = DateTime.Now.AddMinutes(2);
                    Response.Cookies["DNNEvents"].Path = "/";
                    Thread.CurrentThread.CurrentCulture = _currculture;
                }
        }

        public CDefault BasePage => (CDefault) Page;

        #endregion

        #region Public Routines

        public bool IsModerator()
        {
            if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName))
            {
                return true;
            }
            var objEventInfoHelper = new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
            return objEventInfoHelper.IsModerator(Request.IsAuthenticated);
        }

        public bool IsModuleEditor()
        {
            var blHasBasePermissions = false;
            try
            {
                var mc = new ModuleController();
                var objMod = default(ModuleInfo);
                var mp = default(ModulePermissionCollection);

                objMod = mc.GetModule(ModuleId, TabId, false);

                if (!ReferenceEquals(objMod, null))
                {
                    mp = objMod.ModulePermissions;
                    if (ModulePermissionController.HasModulePermission(mp, "EVENTSEDT"))
                    {
                        blHasBasePermissions = true;
                    }
                    else if (ModulePermissionController.HasModulePermission(mp, "EDIT"))
                    {
                        blHasBasePermissions = true;
                    }
                }
            }
            catch
            { }
            if (blHasBasePermissions &&
                (Settings.SocialGroupModule == EventModuleSettings.SocialModule.SocialGroup) &
                (Settings.SocialGroupSecurity != EventModuleSettings.SocialGroupPrivacy.OpenToAll))
            {
                var socialGroupID = GetUrlGroupId();
                if (socialGroupID > -1)
                {
                    var objRoleCtl = new RoleController();
                    var objRoleInfo = objRoleCtl.GetRole(socialGroupID, PortalSettings.PortalId);
                    if (!ReferenceEquals(objRoleInfo, null))
                    {
                        if (!PortalSettings.UserInfo.IsInRole(objRoleInfo.RoleName))
                        {
                            return false;
                        }
                    }
                }
            }
            return blHasBasePermissions;
        }

        public bool IsEventEditor(EventInfo objEvent, bool blMasterOwner)
        {
            try
            {
                if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) ||
                    IsModuleEditor() && (objEvent.CreatedByID == UserId ||
                                              objEvent.OwnerID == UserId && !blMasterOwner ||
                                              objEvent.RmOwnerID == UserId) ||
                    IsModerator())
                {
                    return true;
                }
                return false;
            }
            catch
            { }
            return false;
        }

        public bool IsPrivateNotModerator
        {
            get
                {
                    var returnValue = false;
                    if (Settings.PrivateMessage != "" && !IsModerator())
                    {
                        returnValue = true;
                    }
                    else
                    {
                        returnValue = false;
                    }
                    return returnValue;
                }
        }

        public bool IsCategoryEditor()
        {
            if (Request.IsAuthenticated)
            {
                if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName))
                {
                    return true;
                }
                try
                {
                    var mc = new ModuleController();
                    var objMod = default(ModuleInfo);
                    var mp = default(ModulePermissionCollection);

                    objMod = mc.GetModule(ModuleId, TabId, false);

                    if (!ReferenceEquals(objMod, null))
                    {
                        mp = objMod.ModulePermissions;
                        return ModulePermissionController.HasModulePermission(mp, "EVENTSCAT");
                    }
                    return false;
                }
                catch
                { }
            }
            return false;
        }

        public bool IsLocationEditor()
        {
            if (Request.IsAuthenticated)
            {
                if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName))
                {
                    return true;
                }
                try
                {
                    var mc = new ModuleController();
                    var objMod = default(ModuleInfo);
                    var mp = default(ModulePermissionCollection);

                    objMod = mc.GetModule(ModuleId, TabId, false);

                    if (!ReferenceEquals(objMod, null))
                    {
                        mp = objMod.ModulePermissions;
                        return ModulePermissionController.HasModulePermission(mp, "EVENTSLOC");
                    }
                    return false;
                }
                catch
                { }
            }
            return false;
        }

        public bool IsSettingsEditor()
        {
            if (Request.IsAuthenticated)
            {
                if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName))
                {
                    return true;
                }
                try
                {
                    var mc = new ModuleController();
                    var objMod = default(ModuleInfo);
                    var mp = default(ModulePermissionCollection);

                    objMod = mc.GetModule(ModuleId, TabId, false);

                    if (!ReferenceEquals(objMod, null))
                    {
                        mp = objMod.ModulePermissions;
                        return ModulePermissionController.HasModulePermission(mp, "EVENTSSET");
                    }
                    return false;
                }
                catch
                { }
            }
            return false;
        }

        public ArrayList GetModerators()
        {
            var moderators = new ArrayList();
            var objCollModulePermission = default(ModulePermissionCollection);
            objCollModulePermission = ModulePermissionController.GetModulePermissions(ModuleId, TabId);
            var objModulePermission = default(ModulePermissionInfo);

            // To cope with host users or someone who is no longer an editor!!

            foreach (ModulePermissionInfo tempLoopVar_objModulePermission in objCollModulePermission)
            {
                objModulePermission = tempLoopVar_objModulePermission;
                if (objModulePermission.PermissionKey == "EVENTSMOD")
                {
                    if (objModulePermission.UserID < 0)
                    {
                        var objCtlRole = new RoleController();
                        if (objModulePermission.RoleID != PortalSettings.AdministratorRoleId)
                        {
                            var lstUsers = objCtlRole.GetUsersByRoleName(PortalId, objModulePermission.RoleName);
                            var objUser = default(UserInfo);
                            foreach (UserInfo tempLoopVar_objUser in lstUsers)
                            {
                                objUser = tempLoopVar_objUser;
                                if (!moderators.Contains(objUser))
                                {
                                    moderators.Add(objUser);
                                }
                            }
                        }
                    }
                    else
                    {
                        var objUserCtl = new UserController();
                        var objUser = objUserCtl.GetUser(PortalId, objModulePermission.UserID);
                        if (!ReferenceEquals(objUser, null))
                        {
                            if (!moderators.Contains(objUser.Email))
                            {
                                moderators.Add(objUser.Email);
                            }
                        }
                    }
                }
            }
            return moderators;
        }

        public Color ImportanceColor(int iImportance)
        {
            switch (iImportance)
            {
                case 1:
                    return Color.Red;
                case 2:
                    return Color.Blue;
                case 3:
                    return Color.Black;
                default:
                    return Color.Red;
            }
        }

        public Color GetColor(string categoryColor)
        {
            var cc = new ColorConverter();
            return (Color) cc.ConvertFromString(categoryColor);
        }

        /// <summary>
        ///     Attach a theme css to the supplied panel
        /// </summary>
        /// <param name="ctlPnlTheme"></param>
        /// <remarks></remarks>
        public void SetTheme(Panel ctlPnlTheme)
        {
            var themeSettings = GetThemeSettings();

            var cssLink = new HtmlLink();
            cssLink.Href = themeSettings.ThemeFile;
            cssLink.Attributes.Add("rel", "stylesheet");
            cssLink.Attributes.Add("type", "text/css");
            var added = false;
            foreach (var pagecontrol in Page.Header.Controls)
            {
                if (pagecontrol is PlaceHolder)
                {
                    var placeholder = (PlaceHolder) pagecontrol;
                    if (placeholder.ID == "CSS")
                    {
                        var insertat = 1;
                        foreach (var placeholdercontrol in placeholder.Controls)
                        {
                            if (placeholdercontrol is HtmlLink)
                            {
                                var htmllink = (HtmlLink) placeholdercontrol;
                                if (htmllink.Href.ToLower().Contains("desktopmodules/events/module.css"))
                                {
                                    placeholder.Controls.AddAt(insertat, cssLink);
                                    added = true;
                                    break;
                                }
                            }
                            insertat++;
                        }
                        if (added)
                        {
                            break;
                        }
                    }
                }
            }
            if (!added)
            {
                Page.Header.Controls.Add(cssLink);
            }

            ctlPnlTheme.CssClass = themeSettings.CssClass;
        }

        public ThemeSetting GetThemeSettings()
        {
            var themeSettings = new ThemeSetting();
            if (themeSettings.ValidateSetting(Settings.EventTheme) == false)
            {
                themeSettings.ReadSetting(Settings.EventThemeDefault, PortalId);
            }
            else if (Settings.EventTheme != "")
            {
                themeSettings.ReadSetting(Settings.EventTheme, PortalId);
            }
            return themeSettings;
        }

        public void AddFacebookMetaTags()
        {
            if (Settings.FBAdmins != "")
            {
                var fbMeta = new HtmlMeta();
                fbMeta.Name = "fb:admins";
                fbMeta.Content = Settings.FBAdmins;
                Page.Header.Controls.Add(fbMeta);
            }

            if (Settings.FBAppID != "")
            {
                var fbMeta = new HtmlMeta();
                fbMeta.Name = "fb:app_id";
                fbMeta.Content = Settings.FBAppID;
                Page.Header.Controls.Add(fbMeta);
            }
        }

        public string ImageInfo(string imageUrl, int imageHeight, int imageWidth)
        {
            var imagestring = "";
            var imageSrc = "";
            if (imageUrl.StartsWith("FileID="))
            {
                var objFile = default(IFileInfo);
                var fileId = int.Parse(imageUrl.Substring(7));
                objFile = FileManager.Instance.GetFile(fileId);
                if (!ReferenceEquals(objFile, null))
                {
                    imageSrc = objFile.Folder + objFile.FileName.Replace(" ", "%20");
                    if (imageSrc.IndexOf("://") + 1 == 0)
                    {
                        imageSrc = PortalSettings.HomeDirectory + imageSrc;
                    }
                    imagestring = ConvertToThumb(imageSrc, imageWidth, imageHeight);
                }
            }
            else if (imageUrl.StartsWith("http"))
            {
                imageSrc = imageUrl;
                imagestring = ConvertToThumb(imageSrc, imageWidth, imageHeight);
            }
            return imagestring;
        }

        private string ConvertToThumb(string imageSrc, int imageWidth, int imageHeight)
        {
            var imagestring = "";
            if ((imageWidth > 0) & (imageHeight > 0))
            {
                var thumbWidth = imageWidth;
                var thumbHeight = imageHeight;
                if (imageHeight > Settings.MaxThumbHeight)
                {
                    thumbHeight = Settings.MaxThumbHeight;
                    thumbWidth = Convert.ToInt32((double) imageWidth * Settings.MaxThumbHeight / imageHeight);
                }
                if (thumbWidth > Settings.MaxThumbWidth)
                {
                    thumbWidth = Settings.MaxThumbWidth;
                    thumbHeight = Convert.ToInt32((double) imageHeight * Settings.MaxThumbWidth / imageWidth);
                }
                imagestring = "<img src=\"" + imageSrc + "\" border=\"0\" width=\"" + thumbWidth + "\" height=\"" +
                              thumbHeight + "\" align=\"middle\" alt=\"\" /><br />";
            }
            else
            {
                imagestring = "<img src=\"" + imageSrc + "\" border=\"0\" align=\"middle\" alt=\"\" /><br />";
            }
            return imagestring;
        }

        public string DetailPageEdit(EventInfo objEvent)
        {
            var editString = "";
            if (IsEventEditor(objEvent, false))
            {
                var objEventInfoHelper = new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
                var imgurl = IconController.IconURL("View");
                editString = "<a href='" +
                             objEventInfoHelper.GetDetailPageRealURL(objEvent.EventID, objEvent.SocialGroupId,
                                                                     objEvent.SocialUserId) + "'><img src=\"" + imgurl +
                             "\" border=\"0\" alt=\"" + Localization.GetString("ViewEvent", LocalResourceFile) +
                             "\" title=\"" + Localization.GetString("ViewEvent", LocalResourceFile) + "\" /></a>";
            }
            return editString;
        }

        //EVT-4499 Redirect to login page with url parameter returnurl
        public void RedirectToLogin()
        {
            var returnUrl = HttpContext.Current.Request.RawUrl;
            if (returnUrl.IndexOf("?returnurl=", StringComparison.Ordinal) != -1)
            {
                returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("?returnurl=", StringComparison.Ordinal));
            }
            returnUrl = HttpUtility.UrlEncode(returnUrl);

            Response.Redirect(
                Globals.LoginURL(returnUrl, Convert.ToBoolean(Request.QueryString["override"] != null)),
                true);
        }

        public void SetUpIconBar(EventIcons eventIcons, EventIcons eventIcons2)
        {
            eventIcons.Visible = false;
            eventIcons2.Visible = false;
            eventIcons.ModuleConfiguration = ModuleConfiguration.Clone();
            eventIcons2.ModuleConfiguration = ModuleConfiguration.Clone();
            switch (Settings.IconBar)
            {
                case "TOP":
                    eventIcons.Visible = true;
                    break;
                case "BOTTOM":
                    eventIcons2.Visible = true;
                    break;
            }
        }

        public void SendNewEventEmails(EventInfo objEvent)
        {
            if (!objEvent.Approved)
            {
                return;
            }
            var objEventEmailInfo = new EventEmailInfo();
            var objEventEmail = new EventEmails(PortalId, ModuleId, LocalResourceFile,
                                                ((PageBase) Page).PageCulture.Name);
            objEventEmailInfo.TxtEmailSubject = Settings.Templates.txtNewEventEmailSubject;
            objEventEmailInfo.TxtEmailBody = Settings.Templates.txtNewEventEmailMessage;
            objEventEmailInfo.TxtEmailFrom = Settings.StandardEmail;
            switch (Settings.Neweventemails)
            {
                case "Subscribe":
                    // Email Subscribed Users

                    var objEventSubscriptionController = new EventSubscriptionController();
                    var lstSubscriptions = default(ArrayList);
                    var objEventSubscription = default(EventSubscriptionInfo);
                    lstSubscriptions = objEventSubscriptionController.EventsSubscriptionGetSubModule(ModuleId);

                    if (lstSubscriptions.Count == 0)
                    {
                        return;
                    }

                    var objEventInfo = new EventInfoHelper(ModuleId, TabId, PortalId, null);
                    var lstusers = objEventInfo.GetEventModuleViewers();

                    foreach (EventSubscriptionInfo tempLoopVar_objEventSubscription in lstSubscriptions)
                    {
                        objEventSubscription = tempLoopVar_objEventSubscription;
                        if (!lstusers.Contains(objEventSubscription.UserID))
                        {
                            var objCtlUser = new UserController();
                            var objUser = objCtlUser.GetUser(PortalId, objEventSubscription.UserID);
                            if (!ReferenceEquals(objUser, null) && objUser.IsSuperUser)
                            {
                                objEventEmailInfo.UserEmails.Add(objUser.Email);
                                objEventEmailInfo.UserLocales.Add(objUser.Profile.PreferredLocale);
                                objEventEmailInfo.UserTimeZoneIds.Add(objUser.Profile.PreferredTimeZone.Id);
                            }
                        }
                        else
                        {
                            objEventEmailInfo.UserIDs.Add(objEventSubscription.UserID);
                        }
                    }
                    break;
                case "Role":
                    // Email users in role
                    EventEmailAddRoleUsers(Settings.Neweventemailrole, objEventEmailInfo);
                    break;
                default:
                    return;
            }
            objEventEmail.SendEmails(objEventEmailInfo, objEvent);
        }

        public void EventEmailAddRoleUsers(int roleId, EventEmailInfo objEventEmailInfo)
        {
            var objRoleController = new RoleController();
            var objRole = objRoleController.GetRole(roleId, PortalId);
            if (!ReferenceEquals(objRole, null))
            {
                var lstUsers = objRoleController.GetUsersByRoleName(PortalId, objRole.RoleName);
                foreach (UserInfo objUser in lstUsers)
                {
                    objEventEmailInfo.UserEmails.Add(objUser.Email);
                    objEventEmailInfo.UserLocales.Add(objUser.Profile.PreferredLocale);
                    objEventEmailInfo.UserTimeZoneIds.Add(objUser.Profile.PreferredTimeZone.Id);
                }
            }
        }

        public void CreateNewEventJournal(EventInfo objEvent)
        {
            if (!Settings.JournalIntegration)
            {
                return;
            }

            if (objEvent.Approved)
            {
                var cntJournal = new Journal();
                var objEventInfoHelper = new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
                var url = objEventInfoHelper.DetailPageURL(objEvent);
                string imageSrc = null;
                if (Settings.Eventimage && objEvent.ImageDisplay)
                {
                    var portalurl = objEventInfoHelper.GetDomainURL();
                    if (PortalSettings.PortalAlias.HTTPAlias.IndexOf("/", StringComparison.Ordinal) > 0)
                    {
                        portalurl = portalurl + Globals.ApplicationPath;
                    }
                    imageSrc = objEvent.ImageURL;
                    if (objEvent.ImageURL.StartsWith("FileID="))
                    {
                        var fileId = int.Parse(objEvent.ImageURL.Substring(7));
                        var objFileInfo = FileManager.Instance.GetFile(fileId);
                        if (!ReferenceEquals(objFileInfo, null))
                        {
                            imageSrc = objFileInfo.Folder + objFileInfo.FileName;
                            if (imageSrc.IndexOf("://") + 1 == 0)
                            {
                                var pi = new PortalController();
                                imageSrc = Globals.AddHTTP(
                                    string.Format("{0}/{1}/{2}", portalurl,
                                                  pi.GetPortal(objEvent.PortalID).HomeDirectory, imageSrc));
                            }
                        }
                    }
                }

                cntJournal.NewEvent(objEvent, TabId, url, imageSrc);

                // Update event to show it has an associated JournalItem
                var cntEvent = new EventController();
                objEvent.JournalItem = true;
                cntEvent.EventsSave(objEvent, true, TabId, false);
            }
        }

        public void CreateEnrollmentJournal(EventSignupsInfo objEventSignup, EventInfo objEvent,
                                            EventModuleSettings enrollSettings)
        {
            if (!enrollSettings.JournalIntegration)
            {
                return;
            }

            if (objEventSignup.Approved && objEventSignup.UserID > -1)
            {
                var modTab = TabId;
                if (modTab == -1)
                {
                    var cntModule = new ModuleController();
                    modTab = cntModule.GetModule(objEvent.ModuleID).TabID;
                }

                var objEventInfoHelper =
                    new EventInfoHelper(objEvent.ModuleID, modTab, objEvent.PortalID, enrollSettings);
                var url = objEventInfoHelper.DetailPageURL(objEvent);

                var creatorUserid = UserId;
                if (creatorUserid == -1)
                {
                    creatorUserid = objEventSignup.UserID;
                }

                var cntJournal = new Journal();
                cntJournal.NewEnrollment(objEventSignup, objEvent, modTab, url, creatorUserid);
            }
        }

        public EventSignupsInfo CreateEnrollment(EventSignupsInfo objEventSignup, EventInfo objEvent)
        {
            return CreateEnrollment(objEventSignup, objEvent, Settings);
        }

        public EventSignupsInfo CreateEnrollment(EventSignupsInfo objEventSignup, EventInfo objEvent,
                                                 EventModuleSettings enrollSettings)
        {
            var objCtlEventSignups = new EventSignupsController();
            if (objEventSignup.SignupID == 0)
            {
                objEventSignup = objCtlEventSignups.EventsSignupsSave(objEventSignup);
            }
            else
            {
                objCtlEventSignups.EventsSignupsSave(objEventSignup);
            }
            CreateEnrollmentJournal(objEventSignup, objEvent, enrollSettings);
            return objEventSignup;
        }

        public void DeleteEnrollment(int signupId, int inModuleId, int eventId)
        {
            var objCtlEventSignups = new EventSignupsController();
            objCtlEventSignups.EventsSignupsDelete(signupId, inModuleId);
            var cntJournal = new Journal();
            cntJournal.DeleteEnrollment(inModuleId, eventId, signupId, PortalId);
        }

        public string ToolTipCreate(EventInfo objEvent, string templateTitle, string templateBody, bool isEvtEditor)
        {
            var themeCss = GetThemeSettings().CssClass;

            var tr = new TokenReplaceControllerClass(ModuleId, LocalResourceFile);

            // Add sub module name if a sub-calendar
            var blAddSubModuleName = false;
            if (objEvent.ModuleID != ModuleId && objEvent.ModuleTitle != null && Settings.Addsubmodulename)
            {
                blAddSubModuleName = true;
            }

            var tooltipTitle =
                tr.TokenReplaceEvent(
                    objEvent, Convert.ToString(templateTitle.Replace(Constants.vbLf, "").Replace(Constants.vbCr, "")),
                    blAddSubModuleName);
            var tooltipBody =
                tr.TokenReplaceEvent(
                    objEvent, Convert.ToString(templateBody.Replace(Constants.vbLf, "").Replace(Constants.vbCr, "")),
                    null, false, isEvtEditor);

            // Shorten to maximum length
            var intTooltipLength = Settings.Eventtooltiplength;
            tooltipBody =
                Convert.ToString(
                    HtmlUtils
                        .Shorten(
                            Convert.ToString(HttpUtility.HtmlDecode(tooltipBody).Replace(Environment.NewLine, " ")),
                            intTooltipLength, "...").Replace("[", "&#91;").Replace("]", "&#93;"));
            var tooltip = "<table class=\"" + themeCss + " Eventtooltiptable\" cellspacing=\"0\"><tr><td class=\"" +
                          themeCss + " Eventtooltipheader\">" + tooltipTitle + "</td></tr><tr><td class=\"" + themeCss +
                          " Eventtooltipbody\">" + tooltipBody + "</td></tr></table>";
            return tooltip;
        }

        public string EnrolmentColumns(EventInfo eventInfo, bool enrollListView)
        {
            var txtColumns = "";
            if (Settings.Eventsignup && enrollListView)
            {
                if (IsEventEditor(eventInfo, false))
                {
                    if (Settings.EnrollEditFields != "" || Settings.EnrollViewFields != "" ||
                        Settings.EnrollAnonFields != "")
                    {
                        txtColumns = Settings.EnrollEditFields + ";" + Settings.EnrollViewFields + ";" +
                                     Settings.EnrollAnonFields;
                    }
                }
                else if (Request.IsAuthenticated)
                {
                    if (Settings.EnrollViewFields != "" || Settings.EnrollAnonFields != "")
                    {
                        txtColumns = Settings.EnrollViewFields + ";" + Settings.EnrollAnonFields;
                    }
                }
                else
                {
                    if (Settings.EnrollAnonFields != "")
                    {
                        txtColumns = Settings.EnrollAnonFields;
                    }
                }
            }
            txtColumns = txtColumns.Replace("01", "UserName");
            txtColumns = txtColumns.Replace("02", "DisplayName");
            txtColumns = txtColumns.Replace("03", "Email");
            txtColumns = txtColumns.Replace("04", "Phone");
            txtColumns = txtColumns.Replace("05", "Approved");
            txtColumns = txtColumns.Replace("06", "Qty");

            return txtColumns;
        }

        public void CreateThemeDirectory()
        {
            //Create theme folder if needed
            var pc = new PortalController();
            var with_1 = pc.GetPortal(PortalId);
            var eventSkinPath = string.Format("{0}\\DNNEvents\\Themes", with_1.HomeDirectoryMapPath);
            if (!Directory.Exists(eventSkinPath))
            {
                Directory.CreateDirectory(eventSkinPath);
            }
        }

        public bool HideFullEvent(EventInfo objevent)
        {
            var objEventInfoHelper = new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
            return objEventInfoHelper.HideFullEvent(objevent, Settings.Eventhidefullenroll, UserId,
                                                    Request.IsAuthenticated);
        }

        public string CreateIconString(EventInfo objEvent, bool iconPrio, bool iconRec, bool iconReminder,
                                       bool iconEnroll)
        {
            var objCtlEventRecurMaster = new EventRecurMasterController();
            var iconString = "";
            if (iconPrio)
            {
                switch (objEvent.Importance.ToString())
                {
                    case "High":
                        iconString = "<img src=\"" + ResolveUrl("Images/HighPrio.gif") +
                                     "\" class=\"EventIconHigh\" alt=\"" +
                                     Localization.GetString("HighPrio", LocalResourceFile) + "\" title=\"" +
                                     Localization.GetString("HighPrio", LocalResourceFile) + "\" /> ";
                        break;
                    case "Low":
                        iconString = "<img src=\"" + ResolveUrl("Images/LowPrio.gif") +
                                     "\" class=\"EventIconLow\" alt=\"" +
                                     Localization.GetString("LowPrio", LocalResourceFile) + "\" title=\"" +
                                     Localization.GetString("LowPrio", LocalResourceFile) + "\" /> ";
                        break;
                }
            }
            if (objEvent.RRULE != "" && iconRec)
            {
                iconString = iconString + "<img src=\"" + ResolveUrl("Images/rec.gif") +
                             "\" class=\"EventIconRec\" alt=\"" +
                             Localization.GetString("RecurringEvent", LocalResourceFile) + ": " +
                             objCtlEventRecurMaster.RecurrenceInfo(objEvent, LocalResourceFile) + "\" title=\"" +
                             Localization.GetString("RecurringEvent", LocalResourceFile) + ": " +
                             objCtlEventRecurMaster.RecurrenceInfo(objEvent, LocalResourceFile) + "\" /> ";
            }

            var notificationInfo = "";
            if (objEvent.SendReminder && iconReminder && Request.IsAuthenticated)
            {
                var objEventNotificationController = new EventNotificationController();
                notificationInfo =
                    objEventNotificationController.NotifyInfo(objEvent.EventID, UserInfo.Email, objEvent.ModuleID,
                                                              LocalResourceFile, GetDisplayTimeZoneId());
            }
            if (objEvent.SendReminder && iconReminder && Request.IsAuthenticated &&
                !string.IsNullOrEmpty(notificationInfo))
            {
                iconString = iconString + "<img src=\"" + ResolveUrl("Images/bell.gif") +
                             "\" class=\"EventIconRem\" alt=\"" +
                             Localization.GetString("ReminderEnabled", LocalResourceFile) + ": " +
                             notificationInfo + "\" title=\"" +
                             Localization.GetString("ReminderEnabled", LocalResourceFile) + ": " +
                             notificationInfo + "\" /> ";
            }
            else if (objEvent.SendReminder && iconReminder &&
                     (Settings.Notifyanon || Request.IsAuthenticated))
            {
                iconString = iconString + "<img src=\"" + ResolveUrl("Images/bell.gif") +
                             "\" class=\"EventIconRem\" alt=\"" +
                             Localization.GetString("ReminderEnabled", LocalResourceFile) + "\" title=\"" +
                             Localization.GetString("ReminderEnabled", LocalResourceFile) + "\" /> ";
            }

            var objEventSignupsController = new EventSignupsController();
            if (iconEnroll && objEventSignupsController.DisplayEnrollIcon(objEvent) && Settings.Eventsignup)
            {
                if ((objEvent.MaxEnrollment == 0) | (objEvent.Enrolled < objEvent.MaxEnrollment))
                {
                    iconString = iconString + "<img src=\"" + ResolveUrl("Images/enroll.gif") +
                                 "\" class=\"EventIconEnroll\" alt=\"" +
                                 Localization.GetString("EnrollEnabled", LocalResourceFile) + "\" title=\"" +
                                 Localization.GetString("EnrollEnabled", LocalResourceFile) + "\" /> ";
                }
                else
                {
                    iconString = iconString + "<img src=\"" + ResolveUrl("Images/EnrollFull.gif") +
                                 "\" class=\"EventIconEnrollFull\" alt=\"" +
                                 Localization.GetString("EnrollFull", LocalResourceFile) + "\" title=\"" +
                                 Localization.GetString("EnrollFull", LocalResourceFile) + "\" /> ";
                }
            }
            if (objEvent.DetailPage)
            {
                iconString = iconString + DetailPageEdit(objEvent);
            }
            return iconString;
        }

        public void SetupViewControls(EventIcons eventIcons, EventIcons eventIcons2, SelectCategory selectCategory,
                                      SelectLocation selectLocation, Panel pnlDateControls = null)
        {
            // Disable Top Navigation
            if (!ReferenceEquals(pnlDateControls, null) && Settings.DisableEventnav)
            {
                pnlDateControls.Visible = false;
            }

            // Setup Icon Bar for use
            SetUpIconBar(eventIcons, eventIcons2);

            // Category Configuration and Settings.
            selectCategory.ModuleConfiguration = ModuleConfiguration.Clone();

            // Disable Category Select
            if ((Settings.Enablecategories == EventModuleSettings.DisplayCategories.DoNotDisplay) |
                (IsPrivateNotModerator &&
                 !IsCategoryEditor()))
            {
                selectCategory.Visible = false;
            }

            // Location Configuration and Settings.
            selectLocation.ModuleConfiguration = ModuleConfiguration.Clone();

            // Disable Location Select
            if ((Settings.Enablelocations == EventModuleSettings.DisplayLocations.DoNotDisplay) |
                (IsPrivateNotModerator &&
                 !IsLocationEditor()))
            {
                selectLocation.Visible = false;
            }
        }

        public string CreateEventName(EventInfo objEvent, string template = null)
        {
            var isEvtEditor = IsEventEditor(objEvent, false);

            var blAddSubModuleName = false;
            if (objEvent.ModuleID != ModuleId && objEvent.ModuleTitle != null && Settings.Addsubmodulename)
            {
                blAddSubModuleName = true;
            }
            var tcc = new TokenReplaceControllerClass(ModuleId, LocalResourceFile);
            return tcc.TokenReplaceEvent(objEvent, template, null, blAddSubModuleName, isEvtEditor);
        }

        public ArrayList Get_ListView_Events(ArrayList categoryIDs, ArrayList locationIDs)
        {
            var moduleStartDate = default(DateTime); // Start View Date Events Range
            var moduleEndDate = default(DateTime); // End View Date Events Range
            var displayStartDate = default(DateTime); // Start View Date Events Range
            var displayEndDate = default(DateTime); // End View Date Events Range
            var noEvents = 0;

            // Set Date Range
            var moduleDate = default(DateTime);
            var displayDate = default(DateTime);
            if (Settings.ListViewUseTime)
            {
                moduleDate = ModuleNow();
                displayDate = DisplayNow();
            }
            else
            {
                moduleDate = ModuleNow().Date;
                displayDate = DisplayNow().Date;
            }
            var numDays = Settings.EventsListEventDays;
            if (Settings.EventsListSelectType == "DAYS")
            {
                //****DO NOT CHANGE THE NEXT SECTION FOR ML CODING ****
                // Used Only to select view dates on Event Month View..
                moduleStartDate = moduleDate.AddDays((Settings.EventsListBeforeDays + 1) * -1);
                moduleEndDate = moduleDate.AddDays((Settings.EventsListAfterDays + 1) * 1);
                displayStartDate = displayDate.AddDays(Settings.EventsListBeforeDays * -1);
                displayEndDate = displayDate.AddDays(Settings.EventsListAfterDays * 1);
            }
            else
            {
                noEvents = Settings.EventsListNumEvents;
                moduleStartDate = moduleDate.AddDays(-1);
                moduleEndDate = moduleDate.AddDays(numDays + 1);
                displayStartDate = displayDate;
                displayEndDate = displayDate.AddDays(numDays);
            }

            var getSubEvents = Settings.MasterEvent;

            var objEvent = default(EventInfo);
            var lstEvent = default(EventInfo);
            var objEventInfoHelper = new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
            var lstEvents = default(ArrayList);
            var selectedEvents = new ArrayList();
            lstEvents = objEventInfoHelper.GetEvents(moduleStartDate, moduleEndDate, getSubEvents, categoryIDs,
                                                     locationIDs, GetUrlGroupId(), GetUrlUserId());

            lstEvents = objEventInfoHelper.ConvertEventListToDisplayTimeZone(lstEvents, GetDisplayTimeZoneId());

            foreach (EventInfo tempLoopVar_objEvent in lstEvents)
            {
                objEvent = tempLoopVar_objEvent;
                // If full enrollments should be hidden, ignore
                if (HideFullEvent(objEvent))
                {
                    continue;
                }
                if (objEvent.EventTimeEnd < displayStartDate ||
                    objEvent.EventTimeBegin > displayEndDate)
                {
                    continue;
                }

                var blAddEvent = true;
                if (Settings.Collapserecurring)
                {
                    foreach (EventInfo tempLoopVar_lstEvent in selectedEvents)
                    {
                        lstEvent = tempLoopVar_lstEvent;
                        if (lstEvent.RecurMasterID == objEvent.RecurMasterID)
                        {
                            blAddEvent = false;
                        }
                    }
                }
                if (blAddEvent)
                {
                    selectedEvents.Add(objEvent);
                }

                if (Settings.EventsListSelectType == "EVENTS" &&
                    selectedEvents.Count >= noEvents)
                {
                    break;
                }
            }
            return selectedEvents;
        }

        public DateTime ModuleNow()
        {
            var objEventTimeZoneUtilities = new EventTimeZoneUtilities();
            return objEventTimeZoneUtilities.ConvertFromUTCToModuleTimeZone(DateTime.UtcNow, Settings.TimeZoneId);
        }

        public DateTime DisplayNow()
        {
            var objEventTimeZoneUtilities = new EventTimeZoneUtilities();
            return objEventTimeZoneUtilities
                .ConvertFromUTCToDisplayTimeZone(DateTime.UtcNow, GetDisplayTimeZoneId()).EventDate;
        }

        public string GetDisplayTimeZoneId()
        {
            return GetDisplayTimeZoneId(Settings, PortalId);
        }

        public string GetDisplayTimeZoneId(EventModuleSettings modSettings, int modPortalid)
        {
            return GetDisplayTimeZoneId(modSettings, modPortalid, null);
        }

        public string GetDisplayTimeZoneId(EventModuleSettings modSettings, int modPortalid, string userTimeZoneId)
        {
            var displayTimeToneId = "";

            //Try Primary
            if (modSettings.PrimaryTimeZone == EventModuleSettings.TimeZones.UserTZ)
            {
                if (!string.IsNullOrEmpty(userTimeZoneId))
                {
                    displayTimeToneId = userTimeZoneId;
                }
                else
                {
                    displayTimeToneId = GetUserTimeZoneId();
                }
            }
            else if (modSettings.PrimaryTimeZone == EventModuleSettings.TimeZones.ModuleTZ)
            {
                displayTimeToneId = GetModuleTimeZoneId(modSettings);
            }
            else if (modSettings.PrimaryTimeZone == EventModuleSettings.TimeZones.PortalTZ)
            {
                displayTimeToneId = GetPortalTimeZoneId();
            }

            // Try Secondary
            if (string.IsNullOrEmpty(displayTimeToneId))
            {
                if (modSettings.SecondaryTimeZone == EventModuleSettings.TimeZones.UserTZ)
                {
                    if (!string.IsNullOrEmpty(userTimeZoneId))
                    {
                        displayTimeToneId = userTimeZoneId;
                    }
                    else
                    {
                        displayTimeToneId = GetUserTimeZoneId();
                    }
                }
                else if (modSettings.SecondaryTimeZone == EventModuleSettings.TimeZones.ModuleTZ)
                {
                    displayTimeToneId = GetModuleTimeZoneId(modSettings);
                }
                else if (modSettings.SecondaryTimeZone == EventModuleSettings.TimeZones.PortalTZ)
                {
                    displayTimeToneId = GetPortalTimeZoneId();
                }
            }

            // If all else fails use Portal
            if (string.IsNullOrEmpty(displayTimeToneId))
            {
                displayTimeToneId = GetPortalTimeZoneId();
            }

            return displayTimeToneId;
        }

        public EventListObject.SortFilter GetListSortExpression(string columnName)
        {
            var sortExpression = EventListObject.SortFilter.EventDateBegin;
            switch (columnName)
            {
                case "CategoryName":
                    sortExpression = EventListObject.SortFilter.CategoryName;
                    break;
                case "CustomField1":
                    sortExpression = EventListObject.SortFilter.CustomField1;
                    break;
                case "CustomField2":
                    sortExpression = EventListObject.SortFilter.CustomField2;
                    break;
                case "Description":
                    sortExpression = EventListObject.SortFilter.Description;
                    break;
                case "Duration":
                    sortExpression = EventListObject.SortFilter.Duration;
                    break;
                case "EventDateBegin":
                    sortExpression = EventListObject.SortFilter.EventDateBegin;
                    break;
                case "EventDateEnd":
                    sortExpression = EventListObject.SortFilter.EventDateEnd;
                    break;
                case "EventName":
                    sortExpression = EventListObject.SortFilter.EventName;
                    break;
                case "LocationName":
                    sortExpression = EventListObject.SortFilter.LocationName;
                    break;
                case "EventID":
                    sortExpression = EventListObject.SortFilter.EventID;
                    break;
            }
            return sortExpression;
        }

        public EventSignupsInfo.SortFilter GetSignupsSortExpression(string columnName)
        {
            var sortExpression = EventSignupsInfo.SortFilter.EventTimeBegin;
            switch (columnName)
            {
                case "EventID":
                    sortExpression = EventSignupsInfo.SortFilter.EventID;
                    break;
                case "Duration":
                    sortExpression = EventSignupsInfo.SortFilter.Duration;
                    break;
                case "EventTimeBegin":
                    sortExpression = EventSignupsInfo.SortFilter.EventTimeBegin;
                    break;
                case "EventTimeEnd":
                    sortExpression = EventSignupsInfo.SortFilter.EventTimeEnd;
                    break;
                case "EventName":
                    sortExpression = EventSignupsInfo.SortFilter.EventName;
                    break;
                case "Approved":
                    sortExpression = EventSignupsInfo.SortFilter.Approved;
                    break;
            }
            return sortExpression;
        }

        public int GetUrlGroupId()
        {
            var socialGroupId = -1;
            if (!(HttpContext.Current.Request.QueryString["groupid"] == "") && Settings.SocialGroupModule ==
                EventModuleSettings.SocialModule.SocialGroup)
            {
                socialGroupId = Convert.ToInt32(HttpContext.Current.Request.QueryString["groupid"]);
            }
            return socialGroupId;
        }

        public int GetUrlUserId()
        {
            var socialUserId = -1;
            if (!(HttpContext.Current.Request.QueryString["userid"] == "") && Settings.SocialGroupModule ==
                EventModuleSettings.SocialModule.UserProfile)
            {
                socialUserId = Convert.ToInt32(HttpContext.Current.Request.QueryString["Userid"]);
            }
            return socialUserId;
        }

        public void StorePrevPageInViewState()
        {
            if (!ReferenceEquals(Request.UrlReferrer, null))
            {
                ViewState["prevPage"] = Request.UrlReferrer.ToString();
            }
            else
            {
                ViewState["prevPage"] = GetSocialNavigateUrl();
            }
        }

        public string GetStoredPrevPage()
        {
            return ViewState["prevPage"].ToString();
        }

        public string GetSocialNavigateUrl()
        {
            var socialGroupId = GetUrlGroupId();
            var socialUserId = GetUrlUserId();
            if (socialGroupId > 0)
            {
                return Globals.NavigateURL(TabId, "", "groupid=" + socialGroupId);
            }
            if (socialUserId > 0)
            {
                return Globals.NavigateURL(TabId, "", "userid=" + socialUserId);
            }
            return Globals.NavigateURL();
        }

        #endregion

        #region Private Routines

        private string GetUserTimeZoneId()
        {
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                var objUser = UserController.GetCurrentUserInfo();
                var authUserTimeZone = objUser.Profile.PreferredTimeZone;
                return authUserTimeZone.Id;
            }
            return "";
        }

        private string GetModuleTimeZoneId(EventModuleSettings modSettings)
        {
            if (!ReferenceEquals(modSettings.TimeZoneId, null))
            {
                return modSettings.TimeZoneId;
            }
            return "";
        }

        private string GetPortalTimeZoneId()
        {
            var portalTimeZoneId = "";
            if (ReferenceEquals(HttpContext.Current, null))
            {
                portalTimeZoneId = PortalController.GetPortalSetting("TimeZone", PortalId, string.Empty);
            }
            else
            {
                portalTimeZoneId = PortalController.GetCurrentPortalSettings().TimeZone.Id;
            }
            return portalTimeZoneId;
        }

        #endregion

        #region  Shadow PMB Settings

        #region  Variables

        private EventModuleSettings _settings;

        #endregion

        #region  Properties

        public new EventModuleSettings Settings
        {
            get
                {
                    if (ReferenceEquals(_settings, null))
                    {
                        _settings =
                            EventModuleSettings.GetEventModuleSettings(ModuleId, LocalResourceFile);
                    }
                    return _settings;
                }
        }

        #endregion

        #endregion
    }
}