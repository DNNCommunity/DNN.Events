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
    using System.Web.UI;
    using DotNetNuke.Common;
    using DotNetNuke.Entities.Icons;
    using DotNetNuke.Security;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using global::Components;

    public partial class EventIcons : EventBase
    {
        private static readonly string _myFileName = typeof(EventIcons).BaseType.Name + ".ascx";

        protected new string LocalResourceFile => Localization.GetResourceFile(this, _myFileName);


        #region Event Handlers

        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.btnMonth.Visible = false;
                if (this.Settings.MonthAllowed && this.Parent.ID.ToLower() != "eventmonth")
                {
                    this.btnMonth.Visible = true;
                    this.btnMonth.AlternateText = Localization.GetString("MenuMonth", this.LocalResourceFile);
                    this.btnMonth.ToolTip = Localization.GetString("MenuMonth", this.LocalResourceFile);
                }
                this.btnWeek.Visible = false;
                if (this.Settings.WeekAllowed && this.Parent.ID.ToLower() != "eventweek")
                {
                    this.btnWeek.Visible = true;
                    this.btnWeek.AlternateText = Localization.GetString("MenuWeek", this.LocalResourceFile);
                    this.btnWeek.ToolTip = Localization.GetString("MenuWeek", this.LocalResourceFile);
                }
                this.btnList.Visible = false;
                if (this.Settings.ListAllowed && this.Parent.ID.ToLower() != "eventlist" &&
                    this.Parent.ID.ToLower() != "eventrpt")
                {
                    this.btnList.Visible = true;
                    this.btnList.AlternateText = Localization.GetString("MenuList", this.LocalResourceFile);
                    this.btnList.ToolTip = Localization.GetString("MenuList", this.LocalResourceFile);
                }
                this.btnEnroll.Visible = false;
                if (this.Settings.Eventsignup && this.Parent.ID.ToLower() != "eventmyenrollments")
                {
                    this.btnEnroll.Visible = true;
                    this.btnEnroll.AlternateText = Localization.GetString("MenuMyEnrollments", this.LocalResourceFile);
                    this.btnEnroll.ToolTip = Localization.GetString("MenuMyEnrollments", this.LocalResourceFile);
                }

                var socialGroupId = this.GetUrlGroupId();
                var groupStr = "";
                if (socialGroupId > 0)
                {
                    groupStr = "&GroupId=" + socialGroupId;
                }
                var socialUserId = this.GetUrlUserId();

                this.hypiCal.Visible = this.Settings.IcalOnIconBar;
                this.hypiCal.ToolTip = Localization.GetString("MenuiCal", this.LocalResourceFile);
                this.hypiCal.NavigateUrl = "~/DesktopModules/Events/EventVCal.aspx?ItemID=0&Mid=" +
                                           Convert.ToString(this.ModuleId) + "&tabid=" + Convert.ToString(this.TabId) +
                                           groupStr;

                this.btnRSS.Visible = this.Settings.RSSEnable;
                this.btnRSS.ToolTip = Localization.GetString("MenuRSS", this.LocalResourceFile);
                this.btnRSS.NavigateUrl = "~/DesktopModules/Events/EventRSS.aspx?mid=" +
                                          Convert.ToString(this.ModuleId) + "&tabid=" + Convert.ToString(this.TabId) +
                                          groupStr;
                this.btnRSS.Target = "_blank";

                this.btnAdd.Visible = false;
                this.btnModerate.Visible = false;
                this.btnSettings.Visible = false;
                this.btnCategories.Visible = false;
                this.btnLocations.Visible = false;
                this.btnSubscribe.Visible = false;
                this.lblSubscribe.Visible = false;
                this.imgBar.Visible = false;

                if (this.Request.IsAuthenticated)
                {
                    var objEventInfoHelper =
                        new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);

                    //Module Editor.
                    if (this.IsModuleEditor())
                    {
                        this.btnAdd.ToolTip = Localization.GetString("MenuAddEvents", this.LocalResourceFile);
                        if (socialGroupId > 0)
                        {
                            this.btnAdd.NavigateUrl =
                                objEventInfoHelper.AddSkinContainerControls(
                                    this.EditUrl("groupid", socialGroupId.ToString(), "Edit"), "?");
                            this.btnAdd.Visible = true;
                        }
                        else if (socialUserId > 0)
                        {
                            if (socialUserId == this.UserId || this.IsModerator())
                            {
                                this.btnAdd.NavigateUrl =
                                    objEventInfoHelper.AddSkinContainerControls(
                                        this.EditUrl("Userid", socialUserId.ToString(), "Edit"), "?");
                                this.btnAdd.Visible = true;
                            }
                        }
                        else
                        {
                            this.btnAdd.NavigateUrl =
                                objEventInfoHelper.AddSkinContainerControls(this.EditUrl("Edit"), "?");
                            this.btnAdd.Visible = true;
                        }
                    }
                    if (this.Settings.Moderateall &&
                        (PortalSecurity.IsInRole(this.PortalSettings.AdministratorRoleName) || this.IsModerator()))
                    {
                        this.btnModerate.Visible = true;
                        this.btnModerate.AlternateText = Localization.GetString("MenuModerate", this.LocalResourceFile);
                        this.btnModerate.ToolTip = Localization.GetString("MenuModerate", this.LocalResourceFile);
                    }
                    // Settings Editor.
                    if (PortalSecurity.IsInRole(this.PortalSettings.AdministratorRoleName) || this.IsSettingsEditor())
                    {
                        this.btnSettings.Visible = true;
                        this.btnSettings.ToolTip = Localization.GetString("MenuSettings", this.LocalResourceFile);
                        if (socialGroupId > 0)
                        {
                            this.btnSettings.NavigateUrl =
                                objEventInfoHelper.AddSkinContainerControls(
                                    this.EditUrl("groupid", socialGroupId.ToString(), "EventSettings"), "?");
                        }
                        else if (socialUserId > 0)
                        {
                            this.btnSettings.NavigateUrl =
                                objEventInfoHelper.AddSkinContainerControls(
                                    this.EditUrl("userid", socialUserId.ToString(), "EventSettings"), "?");
                        }
                        else
                        {
                            this.btnSettings.NavigateUrl =
                                objEventInfoHelper.AddSkinContainerControls(this.EditUrl("EventSettings"), "?");
                        }
                    }
                    // Categories Editor.
                    if (PortalSecurity.IsInRole(this.PortalSettings.AdministratorRoleName) || this.IsCategoryEditor())
                    {
                        this.btnCategories.Visible = true;
                        this.btnCategories.ToolTip = Localization.GetString("MenuCategories", this.LocalResourceFile);
                        if (socialGroupId > 0)
                        {
                            this.btnCategories.NavigateUrl =
                                objEventInfoHelper.AddSkinContainerControls(
                                    this.EditUrl("groupid", socialGroupId.ToString(), "Categories"), "?");
                        }
                        else if (socialUserId > 0)
                        {
                            this.btnCategories.NavigateUrl =
                                objEventInfoHelper.AddSkinContainerControls(
                                    this.EditUrl("userid", socialUserId.ToString(), "Categories"), "?");
                        }
                        else
                        {
                            this.btnCategories.NavigateUrl =
                                objEventInfoHelper.AddSkinContainerControls(this.EditUrl("Categories"), "?");
                        }
                    }
                    // Locations Editor.
                    if (PortalSecurity.IsInRole(this.PortalSettings.AdministratorRoleName) || this.IsLocationEditor())
                    {
                        this.btnLocations.Visible = true;
                        this.btnLocations.ToolTip = Localization.GetString("MenuLocations", this.LocalResourceFile);
                        if (socialGroupId > 0)
                        {
                            this.btnLocations.NavigateUrl =
                                objEventInfoHelper.AddSkinContainerControls(
                                    this.EditUrl("groupid", socialGroupId.ToString(), "Locations"), "?");
                        }
                        else if (socialUserId > 0)
                        {
                            this.btnLocations.NavigateUrl =
                                objEventInfoHelper.AddSkinContainerControls(
                                    this.EditUrl("userid", socialUserId.ToString(), "Locations"), "?");
                        }
                        else
                        {
                            this.btnLocations.NavigateUrl =
                                objEventInfoHelper.AddSkinContainerControls(this.EditUrl("Locations"), "?");
                        }
                    }

                    if (this.Settings.Neweventemails == "Subscribe")
                    {
                        this.btnSubscribe.Visible = true;
                        this.lblSubscribe.Visible = true;
                        this.imgBar.Visible = true;
                        var objEventSubscriptionController = new EventSubscriptionController();
                        var objEventSubscription =
                            objEventSubscriptionController.EventsSubscriptionGetUser(this.UserId, this.ModuleId);
                        if (ReferenceEquals(objEventSubscription, null))
                        {
                            this.lblSubscribe.Text = Localization.GetString("lblSubscribe", this.LocalResourceFile);
                            this.btnSubscribe.AlternateText =
                                Localization.GetString("MenuSubscribe", this.LocalResourceFile);
                            this.btnSubscribe.ToolTip =
                                Localization.GetString("MenuTTSubscribe", this.LocalResourceFile);
                            this.btnSubscribe.ImageUrl = IconController.IconURL("Unchecked");
                        }
                        else
                        {
                            this.lblSubscribe.Text = Localization.GetString("lblUnsubscribe", this.LocalResourceFile);
                            this.btnSubscribe.AlternateText =
                                Localization.GetString("MenuUnsubscribe", this.LocalResourceFile);
                            this.btnSubscribe.ToolTip =
                                Localization.GetString("MenuTTUnsubscribe", this.LocalResourceFile);
                            this.btnSubscribe.ImageUrl = IconController.IconURL("Checked");
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        #region Helper Routines

        private string CreateNavigateURL(string mctl)
        {
            var socialGroupId = this.GetUrlGroupId();
            var socialUserId = this.GetUrlUserId();
            if (socialGroupId > 0)
            {
                return Globals.NavigateURL(this.TabId, "", "ModuleID=" + this.ModuleId, "mctl=" + mctl,
                                           "groupid=" + socialGroupId);
            }
            if (socialUserId > 0)
            {
                return Globals.NavigateURL(this.TabId, "", "ModuleID=" + this.ModuleId, "mctl=" + mctl,
                                           "userid=" + socialUserId);
            }
            return Globals.NavigateURL(this.TabId, "", "ModuleID=" + this.ModuleId, "mctl=" + mctl);
        }

        #endregion

        #region  Web Form Designer Generated Code

        //This call is required by the Web Form Designer.
        [DebuggerStepThrough]
        private void InitializeComponent()
        { }

        private void Page_Init(object sender, EventArgs e)
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            this.InitializeComponent();
        }

        #endregion

        #region Links and Buttons

        protected void btnMonth_Click(object sender, ImageClickEventArgs e)
        {
            this.Response.Redirect(this.CreateNavigateURL("EventMonth"));
        }

        protected void btnEnroll_Click(object sender, ImageClickEventArgs e)
        {
            this.Response.Redirect(this.CreateNavigateURL("EventMyEnrollments"));
        }

        protected void btnList_Click(object sender, ImageClickEventArgs e)
        {
            this.Response.Redirect(this.CreateNavigateURL("EventList"));
        }

        protected void btnWeek_Click(object sender, ImageClickEventArgs e)
        {
            this.Response.Redirect(this.CreateNavigateURL("EventWeek"));
        }

        protected void btnModerate_Click(object sender, ImageClickEventArgs e)
        {
            var objEventInfoHelper = new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);
            var socialGroupId = this.GetUrlGroupId();
            var socialUserId = this.GetUrlUserId();
            if (socialGroupId > 0)
            {
                this.Response.Redirect(
                    objEventInfoHelper.AddSkinContainerControls(
                        this.EditUrl("groupid", socialGroupId.ToString(), "Moderate"), "?"));
            }
            else if (socialUserId > 0)
            {
                this.Response.Redirect(
                    objEventInfoHelper.AddSkinContainerControls(
                        this.EditUrl("userid", socialUserId.ToString(), "Moderate"), "?"));
            }
            else
            {
                this.Response.Redirect(objEventInfoHelper.AddSkinContainerControls(this.EditUrl("Moderate"), "?"));
            }
        }

        protected void btnSubscribe_Click(object sender, ImageClickEventArgs e)
        {
            var objEventSubscriptionController = new EventSubscriptionController();
            if (this.btnSubscribe.ImageUrl == IconController.IconURL("Unchecked"))
            {
                var objEventSubscription = new EventSubscriptionInfo();
                objEventSubscription.SubscriptionID = -1;
                objEventSubscription.ModuleID = this.ModuleId;
                objEventSubscription.PortalID = this.PortalId;
                objEventSubscription.UserID = this.UserId;
                objEventSubscriptionController.EventsSubscriptionSave(objEventSubscription);
                this.btnSubscribe.Visible = true;
                this.lblSubscribe.Text = Localization.GetString("lblUnsubscribe", this.LocalResourceFile);
                this.btnSubscribe.AlternateText = Localization.GetString("MenuUnsubscribe", this.LocalResourceFile);
                this.btnSubscribe.ToolTip = Localization.GetString("MenuTTUnsubscribe", this.LocalResourceFile);
                this.btnSubscribe.ImageUrl = IconController.IconURL("Checked");
            }
            else
            {
                objEventSubscriptionController.EventsSubscriptionDeleteUser(this.UserId, this.ModuleId);
                this.btnSubscribe.Visible = true;
                this.lblSubscribe.Text = Localization.GetString("lblSubscribe", this.LocalResourceFile);
                this.btnSubscribe.AlternateText = Localization.GetString("MenuSubscribe", this.LocalResourceFile);
                this.btnSubscribe.ToolTip = Localization.GetString("MenuTTSubscribe", this.LocalResourceFile);
                this.btnSubscribe.ImageUrl = IconController.IconURL("Unchecked");
            }
        }

        #endregion
    }
}