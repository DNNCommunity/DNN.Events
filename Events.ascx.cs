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
    using System.Diagnostics;
    using System.IO;
    using System.Web;
    using Common;
    using Entities.Icons;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Modules.Actions;
    using Security;
    using Services.Exceptions;
    using Services.Localization;
    using global::Components;

    [DNNtc.ModulePermission("EVENTS_MODULE", "EVENTSSET", "Edit Settings")]
    [DNNtc.ModulePermission("EVENTS_MODULE", "EVENTSMOD", "Events Moderator")]
    [DNNtc.ModulePermission("EVENTS_MODULE", "EVENTSEDT", "Events Editor")]
    [DNNtc.ModulePermission("EVENTS_MODULE", "EVENTSCAT", "Global Category Editor")]
    [DNNtc.ModulePermission("EVENTS_MODULE", "EVENTSLOC", "Global Location Editor")]
    [DNNtc.ModuleDependencies(DNNtc.ModuleDependency.CoreVersion, "8.0.0")]
    [DNNtc.ModuleControlProperties("", "Events Container", DNNtc.ControlType.View, "https://github.com/DNNCommunity/DNN.Events/wiki", true, false)]
    public partial class Events : EventBase, IActionable
    {
        #region Optional Interfaces

        public ModuleActionCollection ModuleActions
        {
            get
                {
                    _socialGroupId = GetUrlGroupId();
                    _socialUserId = GetUrlUserId();
                    // ReSharper disable LocalVariableHidesMember
                    // ReSharper disable InconsistentNaming
                    var Actions =
                        new ModuleActionCollection();
                    // ReSharper restore InconsistentNaming
                    // ReSharper restore LocalVariableHidesMember
                    var objEventInfoHelper =
                        new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
                    var securityLevel = SecurityAccessLevel.View;

                    try
                    {
                        if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName))
                        {
                            securityLevel = SecurityAccessLevel.Admin;
                        }

                        // Add Event
                        if (IsModuleEditor())
                        {
                            if (_socialGroupId > 0)
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuAddEvents", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-add.gif",
                                            objEventInfoHelper.AddSkinContainerControls(
                                                EditUrl("groupid", _socialGroupId.ToString(), "Edit"), "?"),
                                            false,
                                            securityLevel, true, false);
                            }
                            else if (_socialUserId > 0)
                            {
                                if (_socialUserId == UserId || IsModerator())
                                {
                                    Actions.Add(GetNextActionID(),
                                                Localization.GetString("MenuAddEvents", LocalResourceFile),
                                                ModuleActionType.ContentOptions, "",
                                                "../DesktopModules/Events/Images/cal-add.gif",
                                                objEventInfoHelper.AddSkinContainerControls(
                                                    EditUrl("userid", _socialUserId.ToString(), "Edit"), "?"),
                                                false,
                                                securityLevel, true, false);
                                }
                            }
                            else
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuAddEvents", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-add.gif",
                                            objEventInfoHelper.AddSkinContainerControls(EditUrl("Edit"), "?"),
                                            false,
                                            securityLevel, true, false);
                            }
                        }

                        if (!(Request.QueryString["mctl"] == null) && ModuleId ==
                            Convert.ToInt32(Request.QueryString["ModuleID"]))
                        {
                            if (Request["mctl"].EndsWith(".ascx"))
                            {
                                _mcontrolToLoad = Request["mctl"];
                            }
                            else
                            {
                                _mcontrolToLoad = Request["mctl"] + ".ascx";
                            }
                        }

                        // Set Default, if none selected
                        if (_mcontrolToLoad.Length == 0)
                        {
                            if (!ReferenceEquals(
                                    Request.Cookies.Get("DNNEvents" + Convert.ToString(ModuleId)),
                                    null))
                            {
                                _mcontrolToLoad = Request
                                                           .Cookies.Get("DNNEvents" + Convert.ToString(ModuleId))
                                                           .Value;
                            }
                            else
                            {
                                // See if Default View Set
                                _mcontrolToLoad = Settings.DefaultView;
                            }
                        }

                        //Add Month and Week Views
                        if (Settings.MonthAllowed && _mcontrolToLoad != "EventMonth.ascx")
                        {
                            if (_socialGroupId > 0)
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuMonth", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-month.gif",
                                            Globals.NavigateURL(TabId, "", "ModuleID=" + ModuleId,
                                                                "mctl=EventMonth", "groupid=" + _socialGroupId),
                                            false, securityLevel, true, false);
                            }
                            else if (_socialUserId > 0)
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuMonth", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-month.gif",
                                            Globals.NavigateURL(TabId, "", "ModuleID=" + ModuleId,
                                                                "mctl=EventMonth", "userid=" + _socialUserId),
                                            false, securityLevel, true, false);
                            }
                            else
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuMonth", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-month.gif",
                                            Globals.NavigateURL(TabId, "", "ModuleID=" + ModuleId,
                                                                "mctl=EventMonth"), false, securityLevel, true, false);
                            }
                        }
                        if (Settings.WeekAllowed && _mcontrolToLoad != "EventWeek.ascx")
                        {
                            if (_socialGroupId > 0)
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuWeek", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-week.gif",
                                            Globals.NavigateURL(TabId, "", "ModuleID=" + ModuleId,
                                                                "mctl=EventWeek", "groupid=" + _socialGroupId),
                                            false, securityLevel, true, false);
                            }
                            else if (_socialUserId > 0)
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuWeek", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-week.gif",
                                            Globals.NavigateURL(TabId, "", "ModuleID=" + ModuleId,
                                                                "mctl=EventWeek", "userid=" + _socialUserId),
                                            false, securityLevel, true, false);
                            }
                            else
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuWeek", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-week.gif",
                                            Globals.NavigateURL(TabId, "", "ModuleID=" + ModuleId,
                                                                "mctl=EventWeek"), false, securityLevel, true, false);
                            }
                        }
                        if (Settings.ListAllowed && _mcontrolToLoad != "EventList.ascx")
                        {
                            if (_socialGroupId > 0)
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuList", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-list.gif",
                                            Globals.NavigateURL(TabId, "", "ModuleID=" + ModuleId,
                                                                "mctl=EventList", "groupid=" + _socialGroupId),
                                            false, securityLevel, true, false);
                            }
                            else if (_socialUserId > 0)
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuList", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-list.gif",
                                            Globals.NavigateURL(TabId, "", "ModuleID=" + ModuleId,
                                                                "mctl=EventList", "userid=" + _socialUserId),
                                            false, securityLevel, true, false);
                            }
                            else
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuList", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-list.gif",
                                            Globals.NavigateURL(TabId, "", "ModuleID=" + ModuleId,
                                                                "mctl=EventList"), false, securityLevel, true, false);
                            }
                        }


                        // See if Enrollments
                        if (Settings.Eventsignup)
                        {
                            if (_socialGroupId > 0)
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuMyEnrollments", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-enroll.gif",
                                            Globals.NavigateURL(TabId, "", "ModuleID=" + ModuleId,
                                                                "mctl=EventMyEnrollments",
                                                                "groupid=" + _socialGroupId), false,
                                            securityLevel, true, false);
                            }
                            else if (_socialUserId > 0)
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuMyEnrollments", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-enroll.gif",
                                            Globals.NavigateURL(TabId, "", "ModuleID=" + ModuleId,
                                                                "mctl=EventMyEnrollments",
                                                                "userid=" + _socialUserId), false, securityLevel,
                                            true, false);
                            }
                            else
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuMyEnrollments", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-enroll.gif",
                                            Globals.NavigateURL(TabId, "", "ModuleID=" + ModuleId,
                                                                "mctl=EventMyEnrollments"), false, securityLevel, true,
                                            false);
                            }
                        }

                        if (IsModerator() && Settings.Moderateall)
                        {
                            if (_socialGroupId > 0)
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuModerate", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/moderate.gif",
                                            objEventInfoHelper.AddSkinContainerControls(
                                                EditUrl("groupid", _socialGroupId.ToString(), "Moderate"),
                                                "?"), false,
                                            securityLevel, true, false);
                            }
                            else if (_socialUserId > 0)
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuModerate", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/moderate.gif",
                                            objEventInfoHelper.AddSkinContainerControls(
                                                EditUrl("userid", _socialUserId.ToString(), "Moderate"), "?"),
                                            false,
                                            securityLevel, true, false);
                            }
                            else
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuModerate", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/moderate.gif",
                                            objEventInfoHelper.AddSkinContainerControls(EditUrl("Moderate"), "?"),
                                            false, securityLevel, true, false);
                            }
                        }
                        if (IsSettingsEditor())
                        {
                            if (_socialGroupId > 0)
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuSettings", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            IconController.IconURL("EditTab"),
                                            objEventInfoHelper.AddSkinContainerControls(
                                                EditUrl("groupid", _socialGroupId.ToString(),
                                                             "EventSettings"), "?"),
                                            false, securityLevel, true, false);
                            }
                            else if (_socialUserId > 0)
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuSettings", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            IconController.IconURL("EditTab"),
                                            objEventInfoHelper.AddSkinContainerControls(
                                                EditUrl("userid", _socialUserId.ToString(), "EventSettings"),
                                                "?"),
                                            false, securityLevel, true, false);
                            }
                            else
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuSettings", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            IconController.IconURL("EditTab"),
                                            objEventInfoHelper.AddSkinContainerControls(
                                                EditUrl("EventSettings"), "?"),
                                            false, securityLevel, true, false);
                            }
                        }
                        if (IsCategoryEditor())
                        {
                            if (_socialGroupId > 0)
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuCategories", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/SmallCalendar.gif",
                                            objEventInfoHelper.AddSkinContainerControls(
                                                EditUrl("groupid", _socialGroupId.ToString(), "Categories"),
                                                "?"),
                                            false, securityLevel, true, false);
                            }
                            else if (_socialUserId > 0)
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuCategories", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/SmallCalendar.gif",
                                            objEventInfoHelper.AddSkinContainerControls(
                                                EditUrl("userid", _socialUserId.ToString(), "Categories"),
                                                "?"), false,
                                            securityLevel, true, false);
                            }
                            else
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuCategories", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/SmallCalendar.gif",
                                            objEventInfoHelper
                                                .AddSkinContainerControls(EditUrl("Categories"), "?"),
                                            false, securityLevel, true, false);
                            }
                        }
                        if (IsLocationEditor())
                        {
                            if (_socialGroupId > 0)
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuLocations", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/SmallCalendar.gif",
                                            objEventInfoHelper.AddSkinContainerControls(
                                                EditUrl("groupid", _socialGroupId.ToString(), "Locations"),
                                                "?"), false,
                                            securityLevel, true, false);
                            }
                            else if (_socialUserId > 0)
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuLocations", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/SmallCalendar.gif",
                                            objEventInfoHelper.AddSkinContainerControls(
                                                EditUrl("userid", _socialUserId.ToString(), "Locations"),
                                                "?"), false,
                                            securityLevel, true, false);
                            }
                            else
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuLocations", LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/SmallCalendar.gif",
                                            objEventInfoHelper.AddSkinContainerControls(EditUrl("Locations"), "?"),
                                            false, securityLevel, true, false);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //ProcessModuleLoadException(Me, exc)
                    }
                    return Actions;
                }
        }

        #endregion

        #region Private Methods

        private void LoadModuleControl()
        {
            try
            {
                _socialGroupId = GetUrlGroupId();
                _socialUserId = GetUrlUserId();
                var objModules = new ModuleController();
                var objModule = objModules.GetModule(ModuleId, TabId);
                var objDesktopModule =
                    DesktopModuleController.GetDesktopModule(objModule.DesktopModuleID, PortalId);
                var objEventInfoHelper = new EventInfoHelper(ModuleId, TabId, PortalId, Settings);

                // Force Module Default Settings on new Version or New Module Instance
                if (objDesktopModule.Version != Settings.Version)
                {
                    CreateThemeDirectory();
                    //                    objEventInfoHelper.SetDefaultModuleSettings(ModuleId, TabId, Page, LocalResourceFile)
                }

                if (!(Request.QueryString["mctl"] == null) &&
                    (ModuleId == Convert.ToInt32(Request.QueryString["ModuleID"]) ||
                     ModuleId == Convert.ToInt32(Request.QueryString["mid"])))
                {
                    if (Request["mctl"].EndsWith(".ascx"))
                    {
                        _mcontrolToLoad = Request["mctl"];
                    }
                    else
                    {
                        _mcontrolToLoad = Request["mctl"] + ".ascx";
                    }
                }

                // Set Default, if none selected
                if (_mcontrolToLoad.Length == 0)
                {
                    if (!ReferenceEquals(Request.Cookies.Get("DNNEvents" + Convert.ToString(ModuleId)), null))
                    {
                        _mcontrolToLoad = Request.Cookies.Get("DNNEvents" + Convert.ToString(ModuleId)).Value;
                    }
                    else
                    {
                        // See if Default View Set
                        _mcontrolToLoad = Settings.DefaultView;
                    }
                }

                // Check for Valid Module to Load
                _mcontrolToLoad = Path.GetFileNameWithoutExtension(_mcontrolToLoad) + ".ascx";
                switch (_mcontrolToLoad.ToLower())
                {
                    case "eventdetails.ascx":
                        // Search and RSS feed may direct detail page url to base module -
                        // should be put in new page
                        if (Settings.Eventdetailnewpage)
                        {
                            //Get the item id of the selected event
                            if (!ReferenceEquals(Request.Params["ItemId"], null))
                            {
                                _itemId = int.Parse(Request.Params["ItemId"]);
                                var objCtlEvents = new EventController();
                                var objEvent = objCtlEvents.EventsGet(_itemId, ModuleId);
                                if (!ReferenceEquals(objEvent, null))
                                {
                                    Response.Redirect(
                                        objEventInfoHelper.GetDetailPageRealURL(
                                            objEvent.EventID, _socialGroupId, _socialUserId));
                                }
                            }
                        }
                        break;
                    case "eventday.ascx":
                        break;
                    case "eventmonth.ascx":
                        break;
                    case "eventweek.ascx":
                        break;
                    case "eventrpt.ascx":
                        if (Settings.ListViewGrid)
                        {
                            _mcontrolToLoad = "EventList.ascx";
                        }
                        break;
                    case "eventlist.ascx":
                        if (!Settings.ListViewGrid)
                        {
                            _mcontrolToLoad = "EventRpt.ascx";
                        }
                        break;
                    case "eventmoderate.ascx":
                        Response.Redirect(
                            objEventInfoHelper.AddSkinContainerControls(
                                Globals.NavigateURL(TabId, "Moderate", "Mid=" + ModuleId),
                                "?"));
                        break;
                    case "eventmyenrollments.ascx":
                        break;
                    default:
                        lblModuleSettings.Text = Localization.GetString("lblBadControl", LocalResourceFile);
                        lblModuleSettings.Visible = true;
                        return;
                }

                var objPortalModuleBase = (PortalModuleBase) LoadControl(_mcontrolToLoad);
                objPortalModuleBase.ModuleConfiguration = ModuleConfiguration.Clone();
                objPortalModuleBase.ID = Path.GetFileNameWithoutExtension(_mcontrolToLoad);
                phMain.Controls.Add(objPortalModuleBase);

                //EVT-4499 Exlude the EventMyEnrollment.ascx to be set as cookie
                if (_mcontrolToLoad.ToLower() != "eventdetails.ascx"
                    && _mcontrolToLoad.ToLower() != "eventday.ascx"
                    && _mcontrolToLoad.ToLower() != "eventmyenrollments.ascx")
                {
                    var objCookie = new HttpCookie("DNNEvents" + Convert.ToString(ModuleId));
                    objCookie.Value = _mcontrolToLoad;
                    if (ReferenceEquals(Request.Cookies.Get("DNNEvents" + Convert.ToString(ModuleId)), null))
                    {
                        Response.Cookies.Add(objCookie);
                    }
                    else
                    {
                        Response.Cookies.Set(objCookie);
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        #region Private Members

        private int _itemId;
        private string _mcontrolToLoad = "";
        private int _socialGroupId;
        private int _socialUserId;

        #endregion

        #region Event Handlers

        //This call is required by the Web Form Designer.
        [DebuggerStepThrough]
        private static void InitializeComponent()
        { }

        private static void Page_Init(object sender, EventArgs e)
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }

        private void Page_Load(object sender, EventArgs e)
        {
            SetTheme(pnlEventsModule);
            AddFacebookMetaTags();
            LoadModuleControl(); // Load Module Control onto Page
        }

        #endregion
    }
}