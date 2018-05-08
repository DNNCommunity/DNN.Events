using DotNetNuke.Services.Exceptions;
using System.Diagnostics;
using System.Web;
using DotNetNuke.Services.Localization;
using System;
using DotNetNuke.Security;

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
    [DNNtc.ModulePermission("EVENTS_MODULE", "EVENTSSET", "Edit Settings")]
    [DNNtc.ModulePermission("EVENTS_MODULE", "EVENTSMOD", "Events Moderator")]
    [DNNtc.ModulePermission("EVENTS_MODULE", "EVENTSEDT", "Events Editor")]
    [DNNtc.ModulePermission("EVENTS_MODULE", "EVENTSCAT", "Global Category Editor")]
    [DNNtc.ModulePermission("EVENTS_MODULE", "EVENTSLOC", "Global Location Editor")]
    [DNNtc.ModuleDependencies(DNNtc.ModuleDependency.CoreVersion, "8.0.0")]
    [DNNtc.ModuleControlProperties("", "Events Container", DNNtc.ControlType.View, "https://dnnevents.codeplex.com/documentation", true, false)]
    public partial class Events : EventBase, Entities.Modules.IActionable
    {

        #region Private Members

        private int _itemId;
        private string _mcontrolToLoad = "";
        private int _socialGroupId = 0;
        private int _socialUserId = 0;

        #endregion

        #region Event Handlers

        //This call is required by the Web Form Designer.
        [DebuggerStepThrough()]
        private static void InitializeComponent()
        { }

        private static void Page_Init(object sender, EventArgs e)
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }

        private void Page_Load(System.Object sender, EventArgs e)
        {

            SetTheme(pnlEventsModule);
            AddFacebookMetaTags();
            LoadModuleControl(); // Load Module Control onto Page
        }

        #endregion

        #region Private Methods

        private void LoadModuleControl()
        {
            try
            {
                _socialGroupId = GetUrlGroupId();
                _socialUserId = GetUrlUserId();
                Entities.Modules.ModuleController objModules = new Entities.Modules.ModuleController();
                Entities.Modules.ModuleInfo objModule = objModules.GetModule(ModuleId, TabId);
                Entities.Modules.DesktopModuleInfo objDesktopModule =
                    Entities.Modules.DesktopModuleController.GetDesktopModule(objModule.DesktopModuleID, PortalId);
                EventInfoHelper objEventInfoHelper = new EventInfoHelper(ModuleId, TabId, PortalId, Settings);

                // Force Module Default Settings on new Version or New Module Instance
                if (objDesktopModule.Version != Settings.Version)
                {
                    CreateThemeDirectory();
                    //                    objEventInfoHelper.SetDefaultModuleSettings(ModuleId, TabId, Page, LocalResourceFile)
                }

                if (!(Request.QueryString["mctl"] == null) &&
                    (ModuleId == System.Convert.ToInt32(Request.QueryString["ModuleID"]) ||
                     ModuleId == System.Convert.ToInt32(Request.QueryString["mid"])))
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
                    if (!ReferenceEquals(Request.Cookies.Get("DNNEvents" + System.Convert.ToString(ModuleId)), null))
                    {
                        _mcontrolToLoad = Request.Cookies.Get("DNNEvents" + System.Convert.ToString(ModuleId)).Value;
                    }
                    else
                    {
                        // See if Default View Set
                        _mcontrolToLoad = Settings.DefaultView;
                    }
                }

                // Check for Valid Module to Load
                _mcontrolToLoad = System.IO.Path.GetFileNameWithoutExtension(_mcontrolToLoad) + ".ascx";
                switch (_mcontrolToLoad.ToLower())
                {
                    case "eventdetails.ascx":
                        // Search and RSS feed may direct detail page url to base module -
                        // should be put in new page
                        if (Settings.Eventdetailnewpage)
                        {
                            //Get the item id of the selected event
                            if (!(ReferenceEquals(Request.Params["ItemId"], null)))
                            {
                                _itemId = int.Parse(Request.Params["ItemId"]);
                                EventController objCtlEvents = new EventController();
                                EventInfo objEvent = objCtlEvents.EventsGet(_itemId, ModuleId);
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
                                DotNetNuke.Common.Globals.NavigateURL(TabId, "Moderate", "Mid=" + ModuleId.ToString()),
                                "?"));
                        break;
                    case "eventmyenrollments.ascx":
                        break;
                    default:
                        lblModuleSettings.Text = Localization.GetString("lblBadControl", LocalResourceFile);
                        lblModuleSettings.Visible = true;
                        return;
                }

                Entities.Modules.PortalModuleBase objPortalModuleBase =
                    (Entities.Modules.PortalModuleBase)(LoadControl(_mcontrolToLoad));
                objPortalModuleBase.ModuleConfiguration = ModuleConfiguration.Clone();
                objPortalModuleBase.ID = System.IO.Path.GetFileNameWithoutExtension(_mcontrolToLoad);
                phMain.Controls.Add(objPortalModuleBase);

                //EVT-4499 Exlude the EventMyEnrollment.ascx to be set as cookie
                if (_mcontrolToLoad.ToLower() != "eventdetails.ascx"
                    && _mcontrolToLoad.ToLower() != "eventday.ascx"
                    && _mcontrolToLoad.ToLower() != "eventmyenrollments.ascx")
                {
                    HttpCookie objCookie = new HttpCookie("DNNEvents" + System.Convert.ToString(ModuleId));
                    objCookie.Value = _mcontrolToLoad;
                    if (ReferenceEquals(Request.Cookies.Get("DNNEvents" + System.Convert.ToString(ModuleId)), null))
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

        #region Optional Interfaces

        public Entities.Modules.Actions.ModuleActionCollection ModuleActions
        {
            get
            {
                _socialGroupId = GetUrlGroupId();
                _socialUserId = GetUrlUserId();
                // ReSharper disable LocalVariableHidesMember
                // ReSharper disable InconsistentNaming
                Entities.Modules.Actions.ModuleActionCollection Actions =
                    new Entities.Modules.Actions.ModuleActionCollection();
                // ReSharper restore InconsistentNaming
                // ReSharper restore LocalVariableHidesMember
                EventInfoHelper objEventInfoHelper = new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
                SecurityAccessLevel securityLevel = SecurityAccessLevel.View;

                try
                {
                    if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString()))
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
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        "../DesktopModules/Events/Images/cal-add.gif",
                                        objEventInfoHelper.AddSkinContainerControls(
                                            EditUrl("groupid", _socialGroupId.ToString(), "Edit"), "?"), false,
                                        securityLevel, true, false);
                        }
                        else if (_socialUserId > 0)
                        {
                            if (_socialUserId == UserId || IsModerator())
                            {
                                Actions.Add(GetNextActionID(),
                                            Localization.GetString("MenuAddEvents", LocalResourceFile),
                                            Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-add.gif",
                                            objEventInfoHelper.AddSkinContainerControls(
                                                EditUrl("userid", _socialUserId.ToString(), "Edit"), "?"), false,
                                            securityLevel, true, false);
                            }
                        }
                        else
                        {
                            Actions.Add(GetNextActionID(),
                                        Localization.GetString("MenuAddEvents", LocalResourceFile),
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        "../DesktopModules/Events/Images/cal-add.gif",
                                        objEventInfoHelper.AddSkinContainerControls(EditUrl("Edit"), "?"), false,
                                        securityLevel, true, false);
                        }
                    }

                    if (!(Request.QueryString["mctl"] == null) &&
                        ModuleId == System.Convert.ToInt32(Request.QueryString["ModuleID"]))
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
                        if (!ReferenceEquals(Request.Cookies.Get("DNNEvents" + System.Convert.ToString(ModuleId)),
                                             null))
                        {
                            _mcontrolToLoad = Request
                                .Cookies.Get("DNNEvents" + System.Convert.ToString(ModuleId)).Value;
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
                            Actions.Add(GetNextActionID(), Localization.GetString("MenuMonth", LocalResourceFile),
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        "../DesktopModules/Events/Images/cal-month.gif",
                                        DotNetNuke.Common.Globals.NavigateURL(
                                            TabId, "", "ModuleID=" + ModuleId.ToString(),
                                            "mctl=EventMonth", "groupid=" + _socialGroupId.ToString()),
                                        false, securityLevel, true, false);
                        }
                        else if (_socialUserId > 0)
                        {
                            Actions.Add(GetNextActionID(), Localization.GetString("MenuMonth", LocalResourceFile),
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        "../DesktopModules/Events/Images/cal-month.gif",
                                        DotNetNuke.Common.Globals.NavigateURL(
                                            TabId, "", "ModuleID=" + ModuleId.ToString(),
                                            "mctl=EventMonth", "userid=" + _socialUserId.ToString()),
                                        false, securityLevel, true, false);
                        }
                        else
                        {
                            Actions.Add(GetNextActionID(), Localization.GetString("MenuMonth", LocalResourceFile),
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        "../DesktopModules/Events/Images/cal-month.gif",
                                        DotNetNuke.Common.Globals.NavigateURL(
                                            TabId, "", "ModuleID=" + ModuleId.ToString(),
                                            "mctl=EventMonth"), false, securityLevel, true, false);
                        }
                    }
                    if (Settings.WeekAllowed && _mcontrolToLoad != "EventWeek.ascx")
                    {
                        if (_socialGroupId > 0)
                        {
                            Actions.Add(GetNextActionID(), Localization.GetString("MenuWeek", LocalResourceFile),
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        "../DesktopModules/Events/Images/cal-week.gif",
                                        DotNetNuke.Common.Globals.NavigateURL(
                                            TabId, "", "ModuleID=" + ModuleId.ToString(),
                                            "mctl=EventWeek", "groupid=" + _socialGroupId.ToString()),
                                        false, securityLevel, true, false);
                        }
                        else if (_socialUserId > 0)
                        {
                            Actions.Add(GetNextActionID(), Localization.GetString("MenuWeek", LocalResourceFile),
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        "../DesktopModules/Events/Images/cal-week.gif",
                                        DotNetNuke.Common.Globals.NavigateURL(
                                            TabId, "", "ModuleID=" + ModuleId.ToString(),
                                            "mctl=EventWeek", "userid=" + _socialUserId.ToString()),
                                        false, securityLevel, true, false);
                        }
                        else
                        {
                            Actions.Add(GetNextActionID(), Localization.GetString("MenuWeek", LocalResourceFile),
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        "../DesktopModules/Events/Images/cal-week.gif",
                                        DotNetNuke.Common.Globals.NavigateURL(
                                            TabId, "", "ModuleID=" + ModuleId.ToString(),
                                            "mctl=EventWeek"), false, securityLevel, true, false);
                        }
                    }
                    if (Settings.ListAllowed && _mcontrolToLoad != "EventList.ascx")
                    {
                        if (_socialGroupId > 0)
                        {
                            Actions.Add(GetNextActionID(), Localization.GetString("MenuList", LocalResourceFile),
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        "../DesktopModules/Events/Images/cal-list.gif",
                                        DotNetNuke.Common.Globals.NavigateURL(
                                            TabId, "", "ModuleID=" + ModuleId.ToString(),
                                            "mctl=EventList", "groupid=" + _socialGroupId.ToString()),
                                        false, securityLevel, true, false);
                        }
                        else if (_socialUserId > 0)
                        {
                            Actions.Add(GetNextActionID(), Localization.GetString("MenuList", LocalResourceFile),
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        "../DesktopModules/Events/Images/cal-list.gif",
                                        DotNetNuke.Common.Globals.NavigateURL(
                                            TabId, "", "ModuleID=" + ModuleId.ToString(),
                                            "mctl=EventList", "userid=" + _socialUserId.ToString()),
                                        false, securityLevel, true, false);
                        }
                        else
                        {
                            Actions.Add(GetNextActionID(), Localization.GetString("MenuList", LocalResourceFile),
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        "../DesktopModules/Events/Images/cal-list.gif",
                                        DotNetNuke.Common.Globals.NavigateURL(
                                            TabId, "", "ModuleID=" + ModuleId.ToString(),
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
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        "../DesktopModules/Events/Images/cal-enroll.gif",
                                        DotNetNuke.Common.Globals.NavigateURL(
                                            TabId, "", "ModuleID=" + ModuleId.ToString(),
                                            "mctl=EventMyEnrollments",
                                            "groupid=" + _socialGroupId.ToString()), false,
                                        securityLevel, true, false);
                        }
                        else if (_socialUserId > 0)
                        {
                            Actions.Add(GetNextActionID(),
                                        Localization.GetString("MenuMyEnrollments", LocalResourceFile),
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        "../DesktopModules/Events/Images/cal-enroll.gif",
                                        DotNetNuke.Common.Globals.NavigateURL(
                                            TabId, "", "ModuleID=" + ModuleId.ToString(),
                                            "mctl=EventMyEnrollments",
                                            "userid=" + _socialUserId.ToString()), false, securityLevel,
                                        true, false);
                        }
                        else
                        {
                            Actions.Add(GetNextActionID(),
                                        Localization.GetString("MenuMyEnrollments", LocalResourceFile),
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        "../DesktopModules/Events/Images/cal-enroll.gif",
                                        DotNetNuke.Common.Globals.NavigateURL(
                                            TabId, "", "ModuleID=" + ModuleId.ToString(),
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
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        "../DesktopModules/Events/Images/moderate.gif",
                                        objEventInfoHelper.AddSkinContainerControls(
                                            EditUrl("groupid", _socialGroupId.ToString(), "Moderate"), "?"), false,
                                        securityLevel, true, false);
                        }
                        else if (_socialUserId > 0)
                        {
                            Actions.Add(GetNextActionID(),
                                        Localization.GetString("MenuModerate", LocalResourceFile),
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        "../DesktopModules/Events/Images/moderate.gif",
                                        objEventInfoHelper.AddSkinContainerControls(
                                            EditUrl("userid", _socialUserId.ToString(), "Moderate"), "?"), false,
                                        securityLevel, true, false);
                        }
                        else
                        {
                            Actions.Add(GetNextActionID(),
                                        Localization.GetString("MenuModerate", LocalResourceFile),
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
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
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        Entities.Icons.IconController.IconURL("EditTab"),
                                        objEventInfoHelper.AddSkinContainerControls(
                                            EditUrl("groupid", _socialGroupId.ToString(), "EventSettings"), "?"),
                                        false, securityLevel, true, false);
                        }
                        else if (_socialUserId > 0)
                        {
                            Actions.Add(GetNextActionID(),
                                        Localization.GetString("MenuSettings", LocalResourceFile),
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        Entities.Icons.IconController.IconURL("EditTab"),
                                        objEventInfoHelper.AddSkinContainerControls(
                                            EditUrl("userid", _socialUserId.ToString(), "EventSettings"), "?"),
                                        false, securityLevel, true, false);
                        }
                        else
                        {
                            Actions.Add(GetNextActionID(),
                                        Localization.GetString("MenuSettings", LocalResourceFile),
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        Entities.Icons.IconController.IconURL("EditTab"),
                                        objEventInfoHelper.AddSkinContainerControls(EditUrl("EventSettings"), "?"),
                                        false, securityLevel, true, false);
                        }
                    }
                    if (IsCategoryEditor())
                    {
                        if (_socialGroupId > 0)
                        {
                            Actions.Add(GetNextActionID(),
                                        Localization.GetString("MenuCategories", LocalResourceFile),
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        "../DesktopModules/Events/Images/SmallCalendar.gif",
                                        objEventInfoHelper.AddSkinContainerControls(
                                            EditUrl("groupid", _socialGroupId.ToString(), "Categories"), "?"),
                                        false, securityLevel, true, false);
                        }
                        else if (_socialUserId > 0)
                        {
                            Actions.Add(GetNextActionID(),
                                        Localization.GetString("MenuCategories", LocalResourceFile),
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        "../DesktopModules/Events/Images/SmallCalendar.gif",
                                        objEventInfoHelper.AddSkinContainerControls(
                                            EditUrl("userid", _socialUserId.ToString(), "Categories"), "?"), false,
                                        securityLevel, true, false);
                        }
                        else
                        {
                            Actions.Add(GetNextActionID(),
                                        Localization.GetString("MenuCategories", LocalResourceFile),
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        "../DesktopModules/Events/Images/SmallCalendar.gif",
                                        objEventInfoHelper.AddSkinContainerControls(EditUrl("Categories"), "?"),
                                        false, securityLevel, true, false);
                        }
                    }
                    if (IsLocationEditor())
                    {
                        if (_socialGroupId > 0)
                        {
                            Actions.Add(GetNextActionID(),
                                        Localization.GetString("MenuLocations", LocalResourceFile),
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        "../DesktopModules/Events/Images/SmallCalendar.gif",
                                        objEventInfoHelper.AddSkinContainerControls(
                                            EditUrl("groupid", _socialGroupId.ToString(), "Locations"), "?"), false,
                                        securityLevel, true, false);
                        }
                        else if (_socialUserId > 0)
                        {
                            Actions.Add(GetNextActionID(),
                                        Localization.GetString("MenuLocations", LocalResourceFile),
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
                                        "../DesktopModules/Events/Images/SmallCalendar.gif",
                                        objEventInfoHelper.AddSkinContainerControls(
                                            EditUrl("userid", _socialUserId.ToString(), "Locations"), "?"), false,
                                        securityLevel, true, false);
                        }
                        else
                        {
                            Actions.Add(GetNextActionID(),
                                        Localization.GetString("MenuLocations", LocalResourceFile),
                                        Entities.Modules.Actions.ModuleActionType.ContentOptions, "",
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

    }

}


