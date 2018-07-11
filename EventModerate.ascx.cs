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
    using System.Diagnostics;
    using System.Web.UI.WebControls;
    using DotNetNuke.Common;
    using DotNetNuke.Framework;
    using DotNetNuke.Security;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using global::Components;

    [DNNtc.ModuleControlProperties("Moderate", "Moderate Events and Enrollment", DNNtc.ControlType.View, "https://dnnevents.codeplex.com/documentation", true, true)]
    public partial class EventModerate : EventBase
    {
        #region Event Handlers

        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Verify that the current user has moderator access to this module
                if (PortalSecurity.IsInRole(this.PortalSettings.AdministratorRoleName) || this.IsModerator())
                { }
                else
                {
                    this.Response.Redirect(this.GetSocialNavigateUrl(), true);
                }

                // Set the selected theme
                this.SetTheme(this.pnlEventsModuleModerate);

                if (this.Page.IsPostBack == false)
                {
                    this.txtEmailFrom.Text = this.UserInfo.Email;
                    this.LocalizeAll();
                    //Are You Sure You Wish To Update/Delete Item(s) (and send Email) ?'
                    this.cmdUpdateSelected.Attributes.Add(
                        "onclick",
                        "javascript:return confirm('" +
                        Localization.GetString("ConfirmUpdateDeleteModerate", this.LocalResourceFile) + "');");
                    this.BindData();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
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

        #region Private Area

        private readonly EventController _objCtlEvent = new EventController();
        private readonly EventRecurMasterController _objCtlEventRecurMaster = new EventRecurMasterController();
        private readonly EventSignupsController _objCtlEventSignups = new EventSignupsController();
        private ArrayList _eventModeration = new ArrayList();
        private ArrayList _eventRecurModeration = new ArrayList();

        #endregion

        #region Helper Methods

        private void BindData()
        {
            var objEventInfoHelper = new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);
            var objEventTimeZoneUtilities = new EventTimeZoneUtilities();
            this._eventModeration = new ArrayList();
            switch (this.rbModerate.SelectedValue)
            {
                case "Events":
                    this._eventModeration =
                        objEventInfoHelper.ConvertEventListToDisplayTimeZone(
                            this._objCtlEvent.EventsModerateEvents(this.ModuleId, this.GetUrlGroupId()),
                            this.GetDisplayTimeZoneId());

                    this._eventRecurModeration = new ArrayList();
                    this._eventRecurModeration =
                        this._objCtlEventRecurMaster.EventsRecurMasterModerate(this.ModuleId, this.GetUrlGroupId());
                    foreach (EventRecurMasterInfo objRecurMaster in this._eventRecurModeration)
                    {
                        objRecurMaster.Dtstart = objEventTimeZoneUtilities
                            .ConvertToDisplayTimeZone(objRecurMaster.Dtstart, objRecurMaster.EventTimeZoneId,
                                                      this.PortalId, this.GetDisplayTimeZoneId()).EventDate;
                    }

                    //Get data for selected date and fill grid
                    this.grdEvents.DataSource = this._eventModeration;
                    this.grdEvents.DataBind();

                    if (this._eventRecurModeration.Count > 0)
                    {
                        this.grdRecurEvents.DataSource = this._eventRecurModeration;
                        this.grdRecurEvents.DataBind();
                        this.grdRecurEvents.Visible = true;
                    }

                    this.grdEvents.Visible = true;
                    this.grdEnrollment.Visible = false;
                    break;
                case "Enrollment":
                    this._eventModeration =
                        this._objCtlEventSignups.EventsModerateSignups(this.ModuleId, this.GetUrlGroupId());

                    var objSignup = default(EventSignupsInfo);
                    foreach (EventSignupsInfo tempLoopVar_objSignup in this._eventModeration)
                    {
                        objSignup = tempLoopVar_objSignup;
                        if (objSignup.UserID != -1)
                        {
                            objSignup.UserName = objEventInfoHelper
                                .UserDisplayNameProfile(objSignup.UserID, objSignup.UserName, this.LocalResourceFile)
                                .DisplayNameURL;
                        }
                        else
                        {
                            objSignup.UserName = objSignup.AnonName;
                            objSignup.Email = objSignup.AnonEmail;
                        }
                        if (objSignup.Email == "")
                        {
                            objSignup.EmailVisible = false;
                        }
                        else
                        {
                            objSignup.EmailVisible = true;
                        }
                        objSignup.EventTimeBegin = objEventTimeZoneUtilities
                            .ConvertToDisplayTimeZone(objSignup.EventTimeBegin, objSignup.EventTimeZoneId,
                                                      this.PortalId, this.GetDisplayTimeZoneId()).EventDate;
                    }

                    //Get data for selected date and fill grid
                    this.grdEnrollment.DataSource = this._eventModeration;
                    this.grdEnrollment.DataBind();
                    //Add Remove Popup to grid
                    var i = 0;
                    if (this.grdEnrollment.Items.Count > 0)
                    {
                        for (i = 0; i <= this.grdEnrollment.Items.Count - 1; i++)
                        {
                            //Are You Sure You Wish To Email the User?'
                            ((ImageButton) this.grdEnrollment.Items[i].FindControl("btnUserEmail")).Attributes.Add(
                                "onclick",
                                "javascript:return confirm('" +
                                Localization
                                    .GetString(
                                        "ConfirmModerateSendMailToUser",
                                        this
                                            .LocalResourceFile) +
                                "');");
                            ((ImageButton) this.grdEnrollment.Items[i].FindControl("btnUserEmail")).AlternateText =
                                Localization.GetString("EmailUser", this.LocalResourceFile);
                            ((ImageButton) this.grdEnrollment.Items[i].FindControl("btnUserEmail")).ToolTip =
                                Localization.GetString("EmailUser", this.LocalResourceFile);
                        }
                        this.grdEvents.Visible = false;
                        this.grdRecurEvents.Visible = false;
                        this.grdEnrollment.Visible = true;
                    }
                    break;
            }
            if (this._eventModeration.Count < 1)
            {
                //"No New Events/Enrollments to Moderate..."
                this.lblMessage.Text = Localization.GetString("MsgModerateNothingToModerate", this.LocalResourceFile);
                this.ShowButtonsGrid(false);
            }
            else
            {
                //Deny option will delete Event/Enrollment Entries from the Database!"
                this.lblMessage.Text = Localization.GetString("MsgModerateNoteDenyOption", this.LocalResourceFile);
                this.ShowButtonsGrid(true);
            }
        }

        private void LocalizeAll()
        {
            this.txtEmailSubject.Text = this.Settings.Templates.txtEmailSubject;
            this.txtEmailMessage.Text = this.Settings.Templates.txtEmailMessage;

            this.grdEvents.Columns[0].HeaderText = Localization.GetString("SingleAction", this.LocalResourceFile);
            this.grdEvents.Columns[1].HeaderText = Localization.GetString("Date", this.LocalResourceFile);
            this.grdEvents.Columns[2].HeaderText = Localization.GetString("Time", this.LocalResourceFile);
            this.grdEvents.Columns[3].HeaderText = Localization.GetString("Event", this.LocalResourceFile);

            this.grdRecurEvents.Columns[0].HeaderText = Localization.GetString("RecurAction", this.LocalResourceFile);
            this.grdRecurEvents.Columns[1].HeaderText = Localization.GetString("Date", this.LocalResourceFile);
            this.grdRecurEvents.Columns[2].HeaderText = Localization.GetString("Time", this.LocalResourceFile);
            this.grdRecurEvents.Columns[3].HeaderText = Localization.GetString("Event", this.LocalResourceFile);

            this.grdEnrollment.Columns[0].HeaderText = Localization.GetString("Action", this.LocalResourceFile);
            this.grdEnrollment.Columns[1].HeaderText = Localization.GetString("Date", this.LocalResourceFile);
            this.grdEnrollment.Columns[2].HeaderText = Localization.GetString("Time", this.LocalResourceFile);
            this.grdEnrollment.Columns[3].HeaderText = Localization.GetString("Event", this.LocalResourceFile);
            this.grdEnrollment.Columns[4].HeaderText = "";
            this.grdEnrollment.Columns[5].HeaderText = Localization.GetString("User", this.LocalResourceFile);
            this.grdEnrollment.Columns[6].HeaderText = Localization.GetString("NoEnrolees", this.LocalResourceFile);
        }

        private void ShowButtonsGrid(bool blShow)
        {
            this.pnlEmail.Visible = blShow;
            this.pnlGrid.Visible = blShow;
            this.cmdUpdateSelected.Visible = blShow;
            this.cmdSelectApproveAll.Visible = blShow;
            this.cmdSelectDenyAll.Visible = blShow;
            this.cmdUnmarkAll.Visible = blShow;
        }

        #endregion

        #region Links and Buttons

        protected void cmdUpdateSelected_Click(object sender, EventArgs e)
        {
            var item = default(DataGridItem);
            var objEventEmail = new EventEmails(this.PortalId, this.ModuleId, this.LocalResourceFile,
                                                ((PageBase) this.Page).PageCulture.Name);

            try
            {
                switch (this.rbModerate.SelectedValue)
                {
                    case "Events":
                        var objEventEmailInfo_1 = new EventEmailInfo();
                        objEventEmailInfo_1.TxtEmailSubject = this.txtEmailSubject.Text;
                        objEventEmailInfo_1.TxtEmailBody = this.txtEmailMessage.Text;
                        objEventEmailInfo_1.TxtEmailFrom = this.txtEmailFrom.Text;
                        var objCal = new EventInfo();
                        var objEventRecurMaster = default(EventRecurMasterInfo);
                        foreach (DataGridItem tempLoopVar_item in this.grdEvents.Items)
                        {
                            item = tempLoopVar_item;
                            switch (((RadioButtonList) item.FindControl("rbEventAction")).SelectedValue)
                            {
                                case "Approve":
                                    objCal = this._objCtlEvent.EventsGet(
                                        Convert.ToInt32(this.grdEvents.DataKeys[item.ItemIndex]),
                                        this.ModuleId);
                                    objCal.Approved = true;
                                    var newEventEmailSent = objCal.NewEventEmailSent;
                                    objCal.NewEventEmailSent = true;
                                    this._objCtlEvent.EventsSave(objCal, true, this.TabId, false);
                                    // Only send event emails when event approved for first time
                                    if (!newEventEmailSent)
                                    {
                                        objCal.RRULE = "";
                                        this.SendNewEventEmails(objCal);
                                        this.CreateNewEventJournal(objCal);
                                    }
                                    // Email Requesting/Moderated User
                                    if (this.chkEmail.Checked)
                                    {
                                        objCal.RRULE = "";
                                        objEventEmailInfo_1.UserIDs.Clear();
                                        objEventEmailInfo_1.UserIDs.Add(objCal.OwnerID);
                                        objEventEmail.SendEmails(objEventEmailInfo_1, objCal);
                                    }
                                    break;
                                case "Deny":
                                    objCal = this._objCtlEvent.EventsGet(
                                        Convert.ToInt32(this.grdEvents.DataKeys[item.ItemIndex]),
                                        this.ModuleId);
                                    //Don't Allow Delete on Enrolled Event - Only Cancel
                                    objEventRecurMaster =
                                        this._objCtlEventRecurMaster.EventsRecurMasterGet(
                                            objCal.RecurMasterID, objCal.ModuleID);
                                    if (objEventRecurMaster.RRULE != "")
                                    {
                                        objCal.Cancelled = true;
                                        objCal.LastUpdatedID = this.UserId;
                                        objCal = this._objCtlEvent.EventsSave(objCal, false, this.TabId, true);
                                    }
                                    else
                                    {
                                        this._objCtlEventRecurMaster.EventsRecurMasterDelete(
                                            objCal.RecurMasterID, objCal.ModuleID);
                                    }
                                    // Email Requesting/Moderated User
                                    if (this.chkEmail.Checked)
                                    {
                                        objCal.RRULE = "";
                                        objEventEmailInfo_1.UserIDs.Clear();
                                        objEventEmailInfo_1.UserIDs.Add(objCal.OwnerID);
                                        objEventEmail.SendEmails(objEventEmailInfo_1, objCal);
                                    }
                                    break;
                            }
                        }
                        foreach (DataGridItem tempLoopVar_item in this.grdRecurEvents.Items)
                        {
                            item = tempLoopVar_item;
                            switch (((RadioButtonList) item.FindControl("rbEventRecurAction")).SelectedValue)
                            {
                                case "Approve":
                                    objEventRecurMaster =
                                        this._objCtlEventRecurMaster.EventsRecurMasterGet(
                                            Convert.ToInt32(this.grdRecurEvents.DataKeys[item.ItemIndex]),
                                            this.ModuleId);
                                    objEventRecurMaster.Approved = true;
                                    this._objCtlEventRecurMaster.EventsRecurMasterSave(
                                        objEventRecurMaster, this.TabId, false);
                                    var lstEvents = default(ArrayList);
                                    lstEvents = this._objCtlEvent.EventsGetRecurrences(
                                        objEventRecurMaster.RecurMasterID,
                                        objEventRecurMaster.ModuleID);
                                    var blEmailSent = false;
                                    foreach (EventInfo tempLoopVar_objCal in lstEvents)
                                    {
                                        objCal = tempLoopVar_objCal;
                                        if (!objCal.Cancelled)
                                        {
                                            objCal.Approved = true;
                                            var newEventEmailSent = objCal.NewEventEmailSent;
                                            objCal.NewEventEmailSent = true;
                                            this._objCtlEvent.EventsSave(objCal, true, this.TabId, false);
                                            // Only send event emails when event approved for first time
                                            if (!newEventEmailSent && !blEmailSent)
                                            {
                                                objCal.RRULE = objEventRecurMaster.RRULE;
                                                this.SendNewEventEmails(objCal);
                                                this.CreateNewEventJournal(objCal);
                                                blEmailSent = true;
                                            }
                                        }
                                    }
                                    // Email Requesting/Moderated User
                                    if (this.chkEmail.Checked)
                                    {
                                        objCal.RRULE = objEventRecurMaster.RRULE;
                                        objEventEmailInfo_1.UserIDs.Clear();
                                        objEventEmailInfo_1.UserIDs.Add(objEventRecurMaster.CreatedByID);
                                        objEventEmail.SendEmails(objEventEmailInfo_1, objCal);
                                    }
                                    break;
                                case "Deny":
                                    objEventRecurMaster =
                                        this._objCtlEventRecurMaster.EventsRecurMasterGet(
                                            Convert.ToInt32(this.grdRecurEvents.DataKeys[item.ItemIndex]),
                                            this.ModuleId);
                                    //Don't Allow Delete on Enrolled Event - Only Cancel
                                    this._objCtlEventRecurMaster.EventsRecurMasterDelete(
                                        Convert.ToInt32(this.grdRecurEvents.DataKeys[item.ItemIndex]),
                                        this.ModuleId);
                                    // Email Requesting/Moderated User
                                    if (this.chkEmail.Checked)
                                    {
                                        objCal.RRULE = objEventRecurMaster.RRULE;
                                        objEventEmailInfo_1.UserIDs.Clear();
                                        objEventEmailInfo_1.UserIDs.Add(objEventRecurMaster.CreatedByID);
                                        objEventEmail.SendEmails(objEventEmailInfo_1, objCal);
                                    }
                                    break;
                            }
                        }
                        break;
                    case "Enrollment":
                        // Not moderated
                        var objEnroll = default(EventSignupsInfo);
                        foreach (DataGridItem tempLoopVar_item in this.grdEnrollment.Items)
                        {
                            item = tempLoopVar_item;
                            if (((RadioButtonList) item.FindControl("rbEnrollAction")).SelectedValue != "")
                            {
                                objEnroll = this._objCtlEventSignups.EventsSignupsGet(
                                    Convert.ToInt32(this.grdEnrollment.DataKeys[item.ItemIndex]),
                                    this.ModuleId, false);
                                var objCtlEvent = new EventController();
                                var objEvent = objCtlEvent.EventsGet(objEnroll.EventID, objEnroll.ModuleID);
                                var objEventEmailInfo = new EventEmailInfo();
                                objEventEmailInfo.TxtEmailSubject = this.txtEmailSubject.Text;
                                objEventEmailInfo.TxtEmailFrom = this.txtEmailFrom.Text;
                                if (this.chkEmail.Checked)
                                {
                                    if (objEnroll.UserID > -1)
                                    {
                                        objEventEmailInfo.UserIDs.Add(objEnroll.UserID);
                                    }
                                    else
                                    {
                                        objEventEmailInfo.UserEmails.Add(objEnroll.AnonEmail);
                                        objEventEmailInfo.UserLocales.Add(objEnroll.AnonCulture);
                                        objEventEmailInfo.UserTimeZoneIds.Add(objEnroll.AnonTimeZoneId);
                                    }
                                }
                                switch (((RadioButtonList) item.FindControl("rbEnrollAction")).SelectedValue)
                                {
                                    case "Approve":
                                        objEnroll.Approved = true;
                                        this.CreateEnrollment(objEnroll, objEvent);

                                        // Email Requesting/Moderated User
                                        if (this.Settings.SendEnrollMessageApproved)
                                        {
                                            objEventEmailInfo.UserIDs.Add(objEvent.OwnerID);
                                        }
                                        objEventEmailInfo.TxtEmailBody =
                                            this.txtEmailMessage.Text +
                                            this.Settings.Templates.txtEnrollMessageApproved;
                                        objEventEmail.SendEmails(objEventEmailInfo, objEvent, objEnroll);
                                        break;
                                    case "Deny":
                                        this.DeleteEnrollment(
                                            Convert.ToInt32(this.grdEnrollment.DataKeys[item.ItemIndex]),
                                            objEvent.ModuleID, objEvent.EventID);

                                        // Email Requesting/Moderated User
                                        if (this.Settings.SendEnrollMessageDenied)
                                        {
                                            objEventEmailInfo.UserIDs.Add(objEvent.OwnerID);
                                        }
                                        objEventEmailInfo.TxtEmailBody =
                                            this.txtEmailMessage.Text + this.Settings.Templates.txtEnrollMessageDenied;
                                        objEventEmail.SendEmails(objEventEmailInfo, objEvent, objEnroll);
                                        break;
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            this.BindData();
        }

        protected void cmdSelectApproveAll_Click(object sender, EventArgs e)
        {
            this.BindData();
            var item = default(DataGridItem);
            if (this.rbModerate.SelectedValue == "Events")
            {
                foreach (DataGridItem tempLoopVar_item in this.grdEvents.Items)
                {
                    item = tempLoopVar_item;
                    ((RadioButtonList) item.FindControl("rbEventAction")).SelectedValue = "Approve";
                }
                foreach (DataGridItem tempLoopVar_item in this.grdRecurEvents.Items)
                {
                    item = tempLoopVar_item;
                    ((RadioButtonList) item.FindControl("rbEventRecurAction")).SelectedValue = "Approve";
                }
            }
            else
            {
                foreach (DataGridItem tempLoopVar_item in this.grdEnrollment.Items)
                {
                    item = tempLoopVar_item;
                    ((RadioButtonList) item.FindControl("rbEnrollAction")).SelectedValue = "Approve";
                }
            }
            this.cmdUpdateSelected_Click(sender, e);
        }

        protected void cmdSelectDenyAll_Click(object sender, EventArgs e)
        {
            this.BindData();
            var item = default(DataGridItem);
            if (this.rbModerate.SelectedValue == "Events")
            {
                foreach (DataGridItem tempLoopVar_item in this.grdEvents.Items)
                {
                    item = tempLoopVar_item;
                    ((RadioButtonList) item.FindControl("rbEventAction")).SelectedValue = "Deny";
                }
                foreach (DataGridItem tempLoopVar_item in this.grdRecurEvents.Items)
                {
                    item = tempLoopVar_item;
                    ((RadioButtonList) item.FindControl("rbEventRecurAction")).SelectedValue = "Deny";
                }
            }
            else
            {
                foreach (DataGridItem tempLoopVar_item in this.grdEnrollment.Items)
                {
                    item = tempLoopVar_item;
                    ((RadioButtonList) item.FindControl("rbEnrollAction")).SelectedValue = "Deny";
                }
            }
            this.cmdUpdateSelected_Click(sender, e);
        }

        protected void cmdUnmarkAll_Click(object sender, EventArgs e)
        {
            this.BindData();
            var item = default(DataGridItem);
            if (this.rbModerate.SelectedValue == "Events")
            {
                foreach (DataGridItem tempLoopVar_item in this.grdEvents.Items)
                {
                    item = tempLoopVar_item;
                    ((RadioButtonList) item.FindControl("rbEventAction")).SelectedValue = null;
                }
                foreach (DataGridItem tempLoopVar_item in this.grdRecurEvents.Items)
                {
                    item = tempLoopVar_item;
                    ((RadioButtonList) item.FindControl("rbEventRecurAction")).SelectedValue = null;
                }
            }
            else
            {
                foreach (DataGridItem tempLoopVar_item in this.grdEnrollment.Items)
                {
                    item = tempLoopVar_item;
                    ((RadioButtonList) item.FindControl("rbEnrollAction")).SelectedValue = null;
                }
            }
        }

        #endregion

        #region Grid and Other Events

        private void rbModerate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.rbModerate.SelectedValue == "Events")
            {
                this.grdEnrollment.Visible = false;
                this.grdEvents.Visible = true;
                this.grdRecurEvents.Visible = true;
            }
            else
            {
                this.grdEnrollment.Visible = true;
                this.grdEvents.Visible = false;
            }
            this.BindData();
        }

        public void grdEvents_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Select":
                    var itemID = Convert.ToInt32(this.grdEvents.DataKeys[e.Item.ItemIndex]);
                    var objEventInfoHelper =
                        new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);
                    this.Response.Redirect(
                        objEventInfoHelper.AddSkinContainerControls(
                            Globals.NavigateURL(this.TabId, "Edit", "Mid=" + this.ModuleId, "ItemID=" + itemID,
                                                "EditRecur=Single"), "?"));
                    break;
            }
        }

        public void grdRecurEvents_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Select":
                    var itemID = Convert.ToInt32(e.Item.Cells[4].Text);
                    var objEventInfoHelper =
                        new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);
                    this.Response.Redirect(
                        objEventInfoHelper.AddSkinContainerControls(
                            Globals.NavigateURL(this.TabId, "Edit", "Mid=" + this.ModuleId, "ItemID=" + itemID,
                                                "EditRecur=All"), "?"));
                    break;
            }
        }


        public void grdEnrollment_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            var objEnroll = default(EventSignupsInfo);
            objEnroll = this._objCtlEventSignups.EventsSignupsGet(
                Convert.ToInt32(this.grdEnrollment.DataKeys[e.Item.ItemIndex]), this.ModuleId, false);

            try
            {
                switch (e.CommandName)
                {
                    case "Select":
                        try
                        {
                            var itemID = objEnroll.EventID;
                            var objEventInfoHelper =
                                new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);
                            this.Response.Redirect(
                                objEventInfoHelper.AddSkinContainerControls(
                                    this.EditUrl("ItemID", itemID.ToString(), "Edit"), "?"));
                        }
                        catch (Exception)
                        { }
                        break;
                    case "User":
                        var objCtlEvent = new EventController();
                        var objEvent = objCtlEvent.EventsGet(objEnroll.EventID, objEnroll.ModuleID);

                        var objEventEmailInfo = new EventEmailInfo();
                        var objEventEmail = new EventEmails(this.PortalId, this.ModuleId, this.LocalResourceFile,
                                                            ((PageBase) this.Page).PageCulture.Name);
                        objEventEmailInfo.TxtEmailSubject = this.txtEmailSubject.Text;
                        objEventEmailInfo.TxtEmailBody = this.txtEmailMessage.Text;
                        objEventEmailInfo.TxtEmailFrom = this.txtEmailFrom.Text;
                        if (objEnroll.UserID > -1)
                        {
                            objEventEmailInfo.UserIDs.Add(objEnroll.UserID);
                        }
                        else
                        {
                            objEventEmailInfo.UserEmails.Add(objEnroll.AnonEmail);
                            objEventEmailInfo.UserLocales.Add(objEnroll.AnonCulture);
                            objEventEmailInfo.UserTimeZoneIds.Add(objEnroll.AnonTimeZoneId);
                        }
                        objEventEmail.SendEmails(objEventEmailInfo, objEvent, objEnroll);
                        break;
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            this.BindData();
        }

        protected void returnButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.Response.Redirect(this.GetSocialNavigateUrl(), true);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion
    }
}