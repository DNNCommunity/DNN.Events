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
    using DotNetNuke.Common;
    using DotNetNuke.Entities.Icons;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Modules.Actions;
    using DotNetNuke.Security;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using global::Components;

    [DNNtc.ModulePermission("EVENTS_MODULE", "EVENTSSET", "Edit Settings")]
    [DNNtc.ModulePermission("EVENTS_MODULE", "EVENTSMOD", "Events Moderator")]
    [DNNtc.ModulePermission("EVENTS_MODULE", "EVENTSEDT", "Events Editor")]
    [DNNtc.ModulePermission("EVENTS_MODULE", "EVENTSCAT", "Global Category Editor")]
    [DNNtc.ModulePermission("EVENTS_MODULE", "EVENTSLOC", "Global Location Editor")]
    [DNNtc.ModuleDependencies(DNNtc.ModuleDependency.CoreVersion, "8.0.0")]
    [DNNtc.ModuleControlProperties("", "Events Container", DNNtc.ControlType.View, "https://dnnevents.codeplex.com/documentation", true, false)]
    public partial class Events : EventBase, IActionable
    {
        #region Optional Interfaces

        public ModuleActionCollection ModuleActions
        {
            get
                {
                    this._socialGroupId = this.GetUrlGroupId();
                    this._socialUserId = this.GetUrlUserId();
                    // ReSharper disable LocalVariableHidesMember
                    // ReSharper disable InconsistentNaming
                    var Actions =
                        new ModuleActionCollection();
                    // ReSharper restore InconsistentNaming
                    // ReSharper restore LocalVariableHidesMember
                    var objEventInfoHelper =
                        new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);
                    var securityLevel = SecurityAccessLevel.View;

                    try
                    {
                        if (PortalSecurity.IsInRole(this.PortalSettings.AdministratorRoleName))
                        {
                            securityLevel = SecurityAccessLevel.Admin;
                        }

                        // Add Event
                        if (this.IsModuleEditor())
                        {
                            if (this._socialGroupId > 0)
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuAddEvents", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-add.gif",
                                            objEventInfoHelper.AddSkinContainerControls(
                                                this.EditUrl("groupid", this._socialGroupId.ToString(), "Edit"), "?"),
                                            false,
                                            securityLevel, true, false);
                            }
                            else if (this._socialUserId > 0)
                            {
                                if (this._socialUserId == this.UserId || this.IsModerator())
                                {
                                    Actions.Add(this.GetNextActionID(),
                                                Localization.GetString("MenuAddEvents", this.LocalResourceFile),
                                                ModuleActionType.ContentOptions, "",
                                                "../DesktopModules/Events/Images/cal-add.gif",
                                                objEventInfoHelper.AddSkinContainerControls(
                                                    this.EditUrl("userid", this._socialUserId.ToString(), "Edit"), "?"),
                                                false,
                                                securityLevel, true, false);
                                }
                            }
                            else
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuAddEvents", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-add.gif",
                                            objEventInfoHelper.AddSkinContainerControls(this.EditUrl("Edit"), "?"),
                                            false,
                                            securityLevel, true, false);
                            }
                        }

                        if (!(this.Request.QueryString["mctl"] == null) && this.ModuleId ==
                            Convert.ToInt32(this.Request.QueryString["ModuleID"]))
                        {
                            if (this.Request["mctl"].EndsWith(".ascx"))
                            {
                                this._mcontrolToLoad = this.Request["mctl"];
                            }
                            else
                            {
                                this._mcontrolToLoad = this.Request["mctl"] + ".ascx";
                            }
                        }

                        // Set Default, if none selected
                        if (this._mcontrolToLoad.Length == 0)
                        {
                            if (!ReferenceEquals(
                                    this.Request.Cookies.Get("DNNEvents" + Convert.ToString(this.ModuleId)),
                                    null))
                            {
                                this._mcontrolToLoad = this.Request
                                                           .Cookies.Get("DNNEvents" + Convert.ToString(this.ModuleId))
                                                           .Value;
                            }
                            else
                            {
                                // See if Default View Set
                                this._mcontrolToLoad = this.Settings.DefaultView;
                            }
                        }

                        //Add Month and Week Views
                        if (this.Settings.MonthAllowed && this._mcontrolToLoad != "EventMonth.ascx")
                        {
                            if (this._socialGroupId > 0)
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuMonth", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-month.gif",
                                            Globals.NavigateURL(this.TabId, "", "ModuleID=" + this.ModuleId,
                                                                "mctl=EventMonth", "groupid=" + this._socialGroupId),
                                            false, securityLevel, true, false);
                            }
                            else if (this._socialUserId > 0)
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuMonth", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-month.gif",
                                            Globals.NavigateURL(this.TabId, "", "ModuleID=" + this.ModuleId,
                                                                "mctl=EventMonth", "userid=" + this._socialUserId),
                                            false, securityLevel, true, false);
                            }
                            else
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuMonth", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-month.gif",
                                            Globals.NavigateURL(this.TabId, "", "ModuleID=" + this.ModuleId,
                                                                "mctl=EventMonth"), false, securityLevel, true, false);
                            }
                        }
                        if (this.Settings.WeekAllowed && this._mcontrolToLoad != "EventWeek.ascx")
                        {
                            if (this._socialGroupId > 0)
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuWeek", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-week.gif",
                                            Globals.NavigateURL(this.TabId, "", "ModuleID=" + this.ModuleId,
                                                                "mctl=EventWeek", "groupid=" + this._socialGroupId),
                                            false, securityLevel, true, false);
                            }
                            else if (this._socialUserId > 0)
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuWeek", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-week.gif",
                                            Globals.NavigateURL(this.TabId, "", "ModuleID=" + this.ModuleId,
                                                                "mctl=EventWeek", "userid=" + this._socialUserId),
                                            false, securityLevel, true, false);
                            }
                            else
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuWeek", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-week.gif",
                                            Globals.NavigateURL(this.TabId, "", "ModuleID=" + this.ModuleId,
                                                                "mctl=EventWeek"), false, securityLevel, true, false);
                            }
                        }
                        if (this.Settings.ListAllowed && this._mcontrolToLoad != "EventList.ascx")
                        {
                            if (this._socialGroupId > 0)
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuList", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-list.gif",
                                            Globals.NavigateURL(this.TabId, "", "ModuleID=" + this.ModuleId,
                                                                "mctl=EventList", "groupid=" + this._socialGroupId),
                                            false, securityLevel, true, false);
                            }
                            else if (this._socialUserId > 0)
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuList", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-list.gif",
                                            Globals.NavigateURL(this.TabId, "", "ModuleID=" + this.ModuleId,
                                                                "mctl=EventList", "userid=" + this._socialUserId),
                                            false, securityLevel, true, false);
                            }
                            else
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuList", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-list.gif",
                                            Globals.NavigateURL(this.TabId, "", "ModuleID=" + this.ModuleId,
                                                                "mctl=EventList"), false, securityLevel, true, false);
                            }
                        }


                        // See if Enrollments
                        if (this.Settings.Eventsignup)
                        {
                            if (this._socialGroupId > 0)
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuMyEnrollments", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-enroll.gif",
                                            Globals.NavigateURL(this.TabId, "", "ModuleID=" + this.ModuleId,
                                                                "mctl=EventMyEnrollments",
                                                                "groupid=" + this._socialGroupId), false,
                                            securityLevel, true, false);
                            }
                            else if (this._socialUserId > 0)
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuMyEnrollments", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-enroll.gif",
                                            Globals.NavigateURL(this.TabId, "", "ModuleID=" + this.ModuleId,
                                                                "mctl=EventMyEnrollments",
                                                                "userid=" + this._socialUserId), false, securityLevel,
                                            true, false);
                            }
                            else
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuMyEnrollments", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/cal-enroll.gif",
                                            Globals.NavigateURL(this.TabId, "", "ModuleID=" + this.ModuleId,
                                                                "mctl=EventMyEnrollments"), false, securityLevel, true,
                                            false);
                            }
                        }

                        if (this.IsModerator() && this.Settings.Moderateall)
                        {
                            if (this._socialGroupId > 0)
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuModerate", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/moderate.gif",
                                            objEventInfoHelper.AddSkinContainerControls(
                                                this.EditUrl("groupid", this._socialGroupId.ToString(), "Moderate"),
                                                "?"), false,
                                            securityLevel, true, false);
                            }
                            else if (this._socialUserId > 0)
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuModerate", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/moderate.gif",
                                            objEventInfoHelper.AddSkinContainerControls(
                                                this.EditUrl("userid", this._socialUserId.ToString(), "Moderate"), "?"),
                                            false,
                                            securityLevel, true, false);
                            }
                            else
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuModerate", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/moderate.gif",
                                            objEventInfoHelper.AddSkinContainerControls(this.EditUrl("Moderate"), "?"),
                                            false, securityLevel, true, false);
                            }
                        }
                        if (this.IsSettingsEditor())
                        {
                            if (this._socialGroupId > 0)
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuSettings", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            IconController.IconURL("EditTab"),
                                            objEventInfoHelper.AddSkinContainerControls(
                                                this.EditUrl("groupid", this._socialGroupId.ToString(),
                                                             "EventSettings"), "?"),
                                            false, securityLevel, true, false);
                            }
                            else if (this._socialUserId > 0)
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuSettings", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            IconController.IconURL("EditTab"),
                                            objEventInfoHelper.AddSkinContainerControls(
                                                this.EditUrl("userid", this._socialUserId.ToString(), "EventSettings"),
                                                "?"),
                                            false, securityLevel, true, false);
                            }
                            else
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuSettings", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            IconController.IconURL("EditTab"),
                                            objEventInfoHelper.AddSkinContainerControls(
                                                this.EditUrl("EventSettings"), "?"),
                                            false, securityLevel, true, false);
                            }
                        }
                        if (this.IsCategoryEditor())
                        {
                            if (this._socialGroupId > 0)
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuCategories", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/SmallCalendar.gif",
                                            objEventInfoHelper.AddSkinContainerControls(
                                                this.EditUrl("groupid", this._socialGroupId.ToString(), "Categories"),
                                                "?"),
                                            false, securityLevel, true, false);
                            }
                            else if (this._socialUserId > 0)
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuCategories", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/SmallCalendar.gif",
                                            objEventInfoHelper.AddSkinContainerControls(
                                                this.EditUrl("userid", this._socialUserId.ToString(), "Categories"),
                                                "?"), false,
                                            securityLevel, true, false);
                            }
                            else
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuCategories", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/SmallCalendar.gif",
                                            objEventInfoHelper
                                                .AddSkinContainerControls(this.EditUrl("Categories"), "?"),
                                            false, securityLevel, true, false);
                            }
                        }
                        if (this.IsLocationEditor())
                        {
                            if (this._socialGroupId > 0)
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuLocations", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/SmallCalendar.gif",
                                            objEventInfoHelper.AddSkinContainerControls(
                                                this.EditUrl("groupid", this._socialGroupId.ToString(), "Locations"),
                                                "?"), false,
                                            securityLevel, true, false);
                            }
                            else if (this._socialUserId > 0)
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuLocations", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/SmallCalendar.gif",
                                            objEventInfoHelper.AddSkinContainerControls(
                                                this.EditUrl("userid", this._socialUserId.ToString(), "Locations"),
                                                "?"), false,
                                            securityLevel, true, false);
                            }
                            else
                            {
                                Actions.Add(this.GetNextActionID(),
                                            Localization.GetString("MenuLocations", this.LocalResourceFile),
                                            ModuleActionType.ContentOptions, "",
                                            "../DesktopModules/Events/Images/SmallCalendar.gif",
                                            objEventInfoHelper.AddSkinContainerControls(this.EditUrl("Locations"), "?"),
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
                this._socialGroupId = this.GetUrlGroupId();
                this._socialUserId = this.GetUrlUserId();
                var objModules = new ModuleController();
                var objModule = objModules.GetModule(this.ModuleId, this.TabId);
                var objDesktopModule =
                    DesktopModuleController.GetDesktopModule(objModule.DesktopModuleID, this.PortalId);
                var objEventInfoHelper = new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);

                // Force Module Default Settings on new Version or New Module Instance
                if (objDesktopModule.Version != this.Settings.Version)
                {
                    this.CreateThemeDirectory();
                    //                    objEventInfoHelper.SetDefaultModuleSettings(ModuleId, TabId, Page, LocalResourceFile)
                }

                if (!(this.Request.QueryString["mctl"] == null) &&
                    (this.ModuleId == Convert.ToInt32(this.Request.QueryString["ModuleID"]) ||
                     this.ModuleId == Convert.ToInt32(this.Request.QueryString["mid"])))
                {
                    if (this.Request["mctl"].EndsWith(".ascx"))
                    {
                        this._mcontrolToLoad = this.Request["mctl"];
                    }
                    else
                    {
                        this._mcontrolToLoad = this.Request["mctl"] + ".ascx";
                    }
                }

                // Set Default, if none selected
                if (this._mcontrolToLoad.Length == 0)
                {
                    if (!ReferenceEquals(this.Request.Cookies.Get("DNNEvents" + Convert.ToString(this.ModuleId)), null))
                    {
                        this._mcontrolToLoad = this
                            .Request.Cookies.Get("DNNEvents" + Convert.ToString(this.ModuleId)).Value;
                    }
                    else
                    {
                        // See if Default View Set
                        this._mcontrolToLoad = this.Settings.DefaultView;
                    }
                }

                // Check for Valid Module to Load
                this._mcontrolToLoad = Path.GetFileNameWithoutExtension(this._mcontrolToLoad) + ".ascx";
                switch (this._mcontrolToLoad.ToLower())
                {
                    case "eventdetails.ascx":
                        // Search and RSS feed may direct detail page url to base module -
                        // should be put in new page
                        if (this.Settings.Eventdetailnewpage)
                        {
                            //Get the item id of the selected event
                            if (!ReferenceEquals(this.Request.Params["ItemId"], null))
                            {
                                this._itemId = int.Parse(this.Request.Params["ItemId"]);
                                var objCtlEvents = new EventController();
                                var objEvent = objCtlEvents.EventsGet(this._itemId, this.ModuleId);
                                if (!ReferenceEquals(objEvent, null))
                                {
                                    this.Response.Redirect(
                                        objEventInfoHelper.GetDetailPageRealURL(
                                            objEvent.EventID, this._socialGroupId, this._socialUserId));
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
                        if (this.Settings.ListViewGrid)
                        {
                            this._mcontrolToLoad = "EventList.ascx";
                        }
                        break;
                    case "eventlist.ascx":
                        if (!this.Settings.ListViewGrid)
                        {
                            this._mcontrolToLoad = "EventRpt.ascx";
                        }
                        break;
                    case "eventmoderate.ascx":
                        this.Response.Redirect(
                            objEventInfoHelper.AddSkinContainerControls(
                                Globals.NavigateURL(this.TabId, "Moderate", "Mid=" + this.ModuleId),
                                "?"));
                        break;
                    case "eventmyenrollments.ascx":
                        break;
                    default:
                        this.lblModuleSettings.Text = Localization.GetString("lblBadControl", this.LocalResourceFile);
                        this.lblModuleSettings.Visible = true;
                        return;
                }

                var objPortalModuleBase =
                    (PortalModuleBase) this.LoadControl(this._mcontrolToLoad);
                objPortalModuleBase.ModuleConfiguration = this.ModuleConfiguration.Clone();
                objPortalModuleBase.ID = Path.GetFileNameWithoutExtension(this._mcontrolToLoad);
                this.phMain.Controls.Add(objPortalModuleBase);

                //EVT-4499 Exlude the EventMyEnrollment.ascx to be set as cookie
                if (this._mcontrolToLoad.ToLower() != "eventdetails.ascx"
                    && this._mcontrolToLoad.ToLower() != "eventday.ascx"
                    && this._mcontrolToLoad.ToLower() != "eventmyenrollments.ascx")
                {
                    var objCookie = new HttpCookie("DNNEvents" + Convert.ToString(this.ModuleId));
                    objCookie.Value = this._mcontrolToLoad;
                    if (ReferenceEquals(this.Request.Cookies.Get("DNNEvents" + Convert.ToString(this.ModuleId)), null))
                    {
                        this.Response.Cookies.Add(objCookie);
                    }
                    else
                    {
                        this.Response.Cookies.Set(objCookie);
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
            this.SetTheme(this.pnlEventsModule);
            this.AddFacebookMetaTags();
            this.LoadModuleControl(); // Load Module Control onto Page
        }

        #endregion
    }
}