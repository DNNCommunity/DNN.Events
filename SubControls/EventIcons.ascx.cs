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
    using Common;
    using Entities.Icons;
    using Security;
    using Services.Exceptions;
    using Services.Localization;
    using global::Components;

    public partial class EventIcons : EventBase
    {
        private string _myFileName => GetType().BaseType.Name + ".ascx";

        protected new string LocalResourceFile => Localization.GetResourceFile(this, _myFileName);


        #region Event Handlers

        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                btnMonth.Visible = false;
                if (Settings.MonthAllowed && Parent.ID.ToLower() != "eventmonth")
                {
                    btnMonth.Visible = true;
                    btnMonth.AlternateText = Localization.GetString("MenuMonth", LocalResourceFile);
                    btnMonth.ToolTip = Localization.GetString("MenuMonth", LocalResourceFile);
                }
                btnWeek.Visible = false;
                if (Settings.WeekAllowed && Parent.ID.ToLower() != "eventweek")
                {
                    btnWeek.Visible = true;
                    btnWeek.AlternateText = Localization.GetString("MenuWeek", LocalResourceFile);
                    btnWeek.ToolTip = Localization.GetString("MenuWeek", LocalResourceFile);
                }
                btnList.Visible = false;
                if (Settings.ListAllowed && Parent.ID.ToLower() != "eventlist" &&
                    Parent.ID.ToLower() != "eventrpt")
                {
                    btnList.Visible = true;
                    btnList.AlternateText = Localization.GetString("MenuList", LocalResourceFile);
                    btnList.ToolTip = Localization.GetString("MenuList", LocalResourceFile);
                }
                btnEnroll.Visible = false;
                if (Settings.Eventsignup && Parent.ID.ToLower() != "eventmyenrollments")
                {
                    btnEnroll.Visible = true;
                    btnEnroll.AlternateText = Localization.GetString("MenuMyEnrollments", LocalResourceFile);
                    btnEnroll.ToolTip = Localization.GetString("MenuMyEnrollments", LocalResourceFile);
                }

                var socialGroupId = GetUrlGroupId();
                var groupStr = "";
                if (socialGroupId > 0)
                {
                    groupStr = "&GroupId=" + socialGroupId;
                }
                var socialUserId = GetUrlUserId();

                hypiCal.Visible = Settings.IcalOnIconBar;
                hypiCal.ToolTip = Localization.GetString("MenuiCal", LocalResourceFile);
                hypiCal.NavigateUrl = "~/DesktopModules/Events/EventVCal.aspx?ItemID=0&Mid=" +
                                           Convert.ToString(ModuleId) + "&tabid=" + Convert.ToString(TabId) +
                                           groupStr;

                btnRSS.Visible = Settings.RSSEnable;
                btnRSS.ToolTip = Localization.GetString("MenuRSS", LocalResourceFile);
                btnRSS.NavigateUrl = "~/DesktopModules/Events/EventRSS.aspx?mid=" +
                                          Convert.ToString(ModuleId) + "&tabid=" + Convert.ToString(TabId) +
                                          groupStr;
                btnRSS.Target = "_blank";

                btnAdd.Visible = false;
                btnModerate.Visible = false;
                btnSettings.Visible = false;
                btnCategories.Visible = false;
                btnLocations.Visible = false;
                btnSubscribe.Visible = false;
                lblSubscribe.Visible = false;
                imgBar.Visible = false;

                if (Request.IsAuthenticated)
                {
                    var objEventInfoHelper =
                        new EventInfoHelper(ModuleId, TabId, PortalId, Settings);

                    //Module Editor.
                    if (IsModuleEditor())
                    {
                        btnAdd.ToolTip = Localization.GetString("MenuAddEvents", LocalResourceFile);
                        if (socialGroupId > 0)
                        {
                            btnAdd.NavigateUrl =
                                objEventInfoHelper.AddSkinContainerControls(
                                    EditUrl("groupid", socialGroupId.ToString(), "Edit"), "?");
                            btnAdd.Visible = true;
                        }
                        else if (socialUserId > 0)
                        {
                            if (socialUserId == UserId || IsModerator())
                            {
                                btnAdd.NavigateUrl =
                                    objEventInfoHelper.AddSkinContainerControls(
                                        EditUrl("Userid", socialUserId.ToString(), "Edit"), "?");
                                btnAdd.Visible = true;
                            }
                        }
                        else
                        {
                            btnAdd.NavigateUrl =
                                objEventInfoHelper.AddSkinContainerControls(EditUrl("Edit"), "?");
                            btnAdd.Visible = true;
                        }
                    }
                    if (Settings.Moderateall &&
                        (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) || IsModerator()))
                    {
                        btnModerate.Visible = true;
                        btnModerate.AlternateText = Localization.GetString("MenuModerate", LocalResourceFile);
                        btnModerate.ToolTip = Localization.GetString("MenuModerate", LocalResourceFile);
                    }
                    // Settings Editor.
                    if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) || IsSettingsEditor())
                    {
                        btnSettings.Visible = true;
                        btnSettings.ToolTip = Localization.GetString("MenuSettings", LocalResourceFile);
                        if (socialGroupId > 0)
                        {
                            btnSettings.NavigateUrl =
                                objEventInfoHelper.AddSkinContainerControls(
                                    EditUrl("groupid", socialGroupId.ToString(), "EventSettings"), "?");
                        }
                        else if (socialUserId > 0)
                        {
                            btnSettings.NavigateUrl =
                                objEventInfoHelper.AddSkinContainerControls(
                                    EditUrl("userid", socialUserId.ToString(), "EventSettings"), "?");
                        }
                        else
                        {
                            btnSettings.NavigateUrl =
                                objEventInfoHelper.AddSkinContainerControls(EditUrl("EventSettings"), "?");
                        }
                    }
                    // Categories Editor.
                    if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) || IsCategoryEditor())
                    {
                        btnCategories.Visible = true;
                        btnCategories.ToolTip = Localization.GetString("MenuCategories", LocalResourceFile);
                        if (socialGroupId > 0)
                        {
                            btnCategories.NavigateUrl =
                                objEventInfoHelper.AddSkinContainerControls(
                                    EditUrl("groupid", socialGroupId.ToString(), "Categories"), "?");
                        }
                        else if (socialUserId > 0)
                        {
                            btnCategories.NavigateUrl =
                                objEventInfoHelper.AddSkinContainerControls(
                                    EditUrl("userid", socialUserId.ToString(), "Categories"), "?");
                        }
                        else
                        {
                            btnCategories.NavigateUrl =
                                objEventInfoHelper.AddSkinContainerControls(EditUrl("Categories"), "?");
                        }
                    }
                    // Locations Editor.
                    if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) || IsLocationEditor())
                    {
                        btnLocations.Visible = true;
                        btnLocations.ToolTip = Localization.GetString("MenuLocations", LocalResourceFile);
                        if (socialGroupId > 0)
                        {
                            btnLocations.NavigateUrl =
                                objEventInfoHelper.AddSkinContainerControls(
                                    EditUrl("groupid", socialGroupId.ToString(), "Locations"), "?");
                        }
                        else if (socialUserId > 0)
                        {
                            btnLocations.NavigateUrl =
                                objEventInfoHelper.AddSkinContainerControls(
                                    EditUrl("userid", socialUserId.ToString(), "Locations"), "?");
                        }
                        else
                        {
                            btnLocations.NavigateUrl =
                                objEventInfoHelper.AddSkinContainerControls(EditUrl("Locations"), "?");
                        }
                    }

                    if (Settings.Neweventemails == "Subscribe")
                    {
                        btnSubscribe.Visible = true;
                        lblSubscribe.Visible = true;
                        imgBar.Visible = true;
                        var objEventSubscriptionController = new EventSubscriptionController();
                        var objEventSubscription =
                            objEventSubscriptionController.EventsSubscriptionGetUser(UserId, ModuleId);
                        if (ReferenceEquals(objEventSubscription, null))
                        {
                            lblSubscribe.Text = Localization.GetString("lblSubscribe", LocalResourceFile);
                            btnSubscribe.AlternateText =
                                Localization.GetString("MenuSubscribe", LocalResourceFile);
                            btnSubscribe.ToolTip =
                                Localization.GetString("MenuTTSubscribe", LocalResourceFile);
                            btnSubscribe.ImageUrl = IconController.IconURL("Unchecked");
                        }
                        else
                        {
                            lblSubscribe.Text = Localization.GetString("lblUnsubscribe", LocalResourceFile);
                            btnSubscribe.AlternateText =
                                Localization.GetString("MenuUnsubscribe", LocalResourceFile);
                            btnSubscribe.ToolTip =
                                Localization.GetString("MenuTTUnsubscribe", LocalResourceFile);
                            btnSubscribe.ImageUrl = IconController.IconURL("Checked");
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
            var socialGroupId = GetUrlGroupId();
            var socialUserId = GetUrlUserId();
            if (socialGroupId > 0)
            {
                return Globals.NavigateURL(TabId, "", "ModuleID=" + ModuleId, "mctl=" + mctl,
                                           "groupid=" + socialGroupId);
            }
            if (socialUserId > 0)
            {
                return Globals.NavigateURL(TabId, "", "ModuleID=" + ModuleId, "mctl=" + mctl,
                                           "userid=" + socialUserId);
            }
            return Globals.NavigateURL(TabId, "", "ModuleID=" + ModuleId, "mctl=" + mctl);
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
            InitializeComponent();
        }

        #endregion

        #region Links and Buttons

        protected void btnMonth_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect(CreateNavigateURL("EventMonth"));
        }

        protected void btnEnroll_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect(CreateNavigateURL("EventMyEnrollments"));
        }

        protected void btnList_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect(CreateNavigateURL("EventList"));
        }

        protected void btnWeek_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect(CreateNavigateURL("EventWeek"));
        }

        protected void btnModerate_Click(object sender, ImageClickEventArgs e)
        {
            var objEventInfoHelper = new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
            var socialGroupId = GetUrlGroupId();
            var socialUserId = GetUrlUserId();
            if (socialGroupId > 0)
            {
                Response.Redirect(
                    objEventInfoHelper.AddSkinContainerControls(
                        EditUrl("groupid", socialGroupId.ToString(), "Moderate"), "?"));
            }
            else if (socialUserId > 0)
            {
                Response.Redirect(
                    objEventInfoHelper.AddSkinContainerControls(
                        EditUrl("userid", socialUserId.ToString(), "Moderate"), "?"));
            }
            else
            {
                Response.Redirect(objEventInfoHelper.AddSkinContainerControls(EditUrl("Moderate"), "?"));
            }
        }

        protected void btnSubscribe_Click(object sender, ImageClickEventArgs e)
        {
            var objEventSubscriptionController = new EventSubscriptionController();
            if (btnSubscribe.ImageUrl == IconController.IconURL("Unchecked"))
            {
                var objEventSubscription = new EventSubscriptionInfo();
                objEventSubscription.SubscriptionID = -1;
                objEventSubscription.ModuleID = ModuleId;
                objEventSubscription.PortalID = PortalId;
                objEventSubscription.UserID = UserId;
                objEventSubscriptionController.EventsSubscriptionSave(objEventSubscription);
                btnSubscribe.Visible = true;
                lblSubscribe.Text = Localization.GetString("lblUnsubscribe", LocalResourceFile);
                btnSubscribe.AlternateText = Localization.GetString("MenuUnsubscribe", LocalResourceFile);
                btnSubscribe.ToolTip = Localization.GetString("MenuTTUnsubscribe", LocalResourceFile);
                btnSubscribe.ImageUrl = IconController.IconURL("Checked");
            }
            else
            {
                objEventSubscriptionController.EventsSubscriptionDeleteUser(UserId, ModuleId);
                btnSubscribe.Visible = true;
                lblSubscribe.Text = Localization.GetString("lblSubscribe", LocalResourceFile);
                btnSubscribe.AlternateText = Localization.GetString("MenuSubscribe", LocalResourceFile);
                btnSubscribe.ToolTip = Localization.GetString("MenuTTSubscribe", LocalResourceFile);
                btnSubscribe.ImageUrl = IconController.IconURL("Unchecked");
            }
        }

        #endregion
    }
}