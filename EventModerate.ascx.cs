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


using System;
using System.Collections;
using System.Diagnostics;
using System.Web.UI.WebControls;
using DNNtc;
using Components;
using DotNetNuke.Common;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.Events
{
    [DNNtc.ModuleControlProperties("Moderate", "Moderate Events and Enrollment", ControlType.View, "https://github.com/DNNCommunity/DNN.Events/wiki", true, true)]
    public partial class EventModerate : EventBase
    {
        #region Event Handlers

        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Verify that the current user has moderator access to this module
                if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) || IsModerator())
                { }
                else
                {
                    Response.Redirect(GetSocialNavigateUrl(), true);
                }

                // Set the selected theme
                SetTheme(pnlEventsModuleModerate);

                if (Page.IsPostBack == false)
                {
                    txtEmailFrom.Text = UserInfo.Email;
                    LocalizeAll();
                    //Are You Sure You Wish To Update/Delete Item(s) (and send Email) ?'
                    cmdUpdateSelected.Attributes.Add(
                        "onclick",
                        "javascript:return confirm('" +
                        Localization.GetString("ConfirmUpdateDeleteModerate", LocalResourceFile) + "');");
                    BindData();
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
            InitializeComponent();
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
            var objEventInfoHelper = new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
            var objEventTimeZoneUtilities = new EventTimeZoneUtilities();
            _eventModeration = new ArrayList();
            switch (rbModerate.SelectedValue)
            {
                case "Events":
                    _eventModeration =
                        objEventInfoHelper.ConvertEventListToDisplayTimeZone(
                            _objCtlEvent.EventsModerateEvents(ModuleId, GetUrlGroupId()),
                            GetDisplayTimeZoneId());

                    _eventRecurModeration = new ArrayList();
                    _eventRecurModeration =
                        _objCtlEventRecurMaster.EventsRecurMasterModerate(ModuleId, GetUrlGroupId());
                    foreach (EventRecurMasterInfo objRecurMaster in _eventRecurModeration)
                    {
                        objRecurMaster.Dtstart = objEventTimeZoneUtilities
                            .ConvertToDisplayTimeZone(objRecurMaster.Dtstart, objRecurMaster.EventTimeZoneId,
                                                      PortalId, GetDisplayTimeZoneId()).EventDate;
                    }

                    //Get data for selected date and fill grid
                    grdEvents.DataSource = _eventModeration;
                    grdEvents.DataBind();

                    if (_eventRecurModeration.Count > 0)
                    {
                        grdRecurEvents.DataSource = _eventRecurModeration;
                        grdRecurEvents.DataBind();
                        grdRecurEvents.Visible = true;
                    }

                    grdEvents.Visible = true;
                    grdEnrollment.Visible = false;
                    break;
                case "Enrollment":
                    _eventModeration =
                        _objCtlEventSignups.EventsModerateSignups(ModuleId, GetUrlGroupId());

                    var objSignup = default(EventSignupsInfo);
                    foreach (EventSignupsInfo tempLoopVar_objSignup in _eventModeration)
                    {
                        objSignup = tempLoopVar_objSignup;
                        if (objSignup.UserID != -1)
                        {
                            objSignup.UserName = objEventInfoHelper
                                .UserDisplayNameProfile(objSignup.UserID, objSignup.UserName, LocalResourceFile)
                                .DisplayNameURL;
                        }
                        else
                        {
                            objSignup.UserName = objSignup.AnonName;
                            objSignup.Email = objSignup.AnonEmail;
                        }                        
                        objSignup.EmailVisible = objSignup.Email != string.Empty;

                        objSignup.EventTimeBegin = objEventTimeZoneUtilities
                            .ConvertToDisplayTimeZone(objSignup.EventTimeBegin, objSignup.EventTimeZoneId,
                                                      PortalId, GetDisplayTimeZoneId()).EventDate;
                    }

                    //Get data for selected date and fill grid
                    grdEnrollment.DataSource = _eventModeration;
                    grdEnrollment.DataBind();
                    //Add Remove Popup to grid
                    var i = 0;
                    if (grdEnrollment.Items.Count > 0)
                    {
                        for (i = 0; i <= grdEnrollment.Items.Count - 1; i++)
                        {
                            //Are You Sure You Wish To Email the User?'
                            ((ImageButton) grdEnrollment.Items[i].FindControl("btnUserEmail")).Attributes.Add(
                                "onclick",
                                "javascript:return confirm('" +
                                Localization
                                    .GetString(
                                        "ConfirmModerateSendMailToUser",
                                        LocalResourceFile) +
                                "');");
                            ((ImageButton) grdEnrollment.Items[i].FindControl("btnUserEmail")).AlternateText =
                                Localization.GetString("EmailUser", LocalResourceFile);
                            ((ImageButton) grdEnrollment.Items[i].FindControl("btnUserEmail")).ToolTip =
                                Localization.GetString("EmailUser", LocalResourceFile);
                        }
                        grdEvents.Visible = false;
                        grdRecurEvents.Visible = false;
                        grdEnrollment.Visible = true;
                    }
                    break;
            }
            if (_eventModeration.Count < 1)
            {
                //"No New Events/Enrollments to Moderate..."
                lblMessage.Text = Localization.GetString("MsgModerateNothingToModerate", LocalResourceFile);
                ShowButtonsGrid(false);
            }
            else
            {
                //Deny option will delete Event/Enrollment Entries from the Database!"
                lblMessage.Text = Localization.GetString("MsgModerateNoteDenyOption", LocalResourceFile);
                ShowButtonsGrid(true);
            }
        }

        private void LocalizeAll()
        {
            txtEmailSubject.Text = Settings.Templates.txtEmailSubject;
            txtEmailMessage.Text = Settings.Templates.txtEmailMessage;

            grdEvents.Columns[0].HeaderText = Localization.GetString("SingleAction", LocalResourceFile);
            grdEvents.Columns[1].HeaderText = Localization.GetString("Date", LocalResourceFile);
            grdEvents.Columns[2].HeaderText = Localization.GetString("Time", LocalResourceFile);
            grdEvents.Columns[3].HeaderText = Localization.GetString("Event", LocalResourceFile);

            grdRecurEvents.Columns[0].HeaderText = Localization.GetString("RecurAction", LocalResourceFile);
            grdRecurEvents.Columns[1].HeaderText = Localization.GetString("Date", LocalResourceFile);
            grdRecurEvents.Columns[2].HeaderText = Localization.GetString("Time", LocalResourceFile);
            grdRecurEvents.Columns[3].HeaderText = Localization.GetString("Event", LocalResourceFile);

            grdEnrollment.Columns[0].HeaderText = Localization.GetString("Action", LocalResourceFile);
            grdEnrollment.Columns[1].HeaderText = Localization.GetString("Date", LocalResourceFile);
            grdEnrollment.Columns[2].HeaderText = Localization.GetString("Time", LocalResourceFile);
            grdEnrollment.Columns[3].HeaderText = Localization.GetString("Event", LocalResourceFile);
            grdEnrollment.Columns[4].HeaderText = "";
            grdEnrollment.Columns[5].HeaderText = Localization.GetString("User", LocalResourceFile);
            grdEnrollment.Columns[6].HeaderText = Localization.GetString("NoEnrolees", LocalResourceFile);
        }

        private void ShowButtonsGrid(bool blShow)
        {
            pnlEmail.Visible = blShow;
            pnlGrid.Visible = blShow;
            cmdUpdateSelected.Visible = blShow;
            cmdSelectApproveAll.Visible = blShow;
            cmdSelectDenyAll.Visible = blShow;
            cmdUnmarkAll.Visible = blShow;
        }

        #endregion

        #region Links and Buttons

        protected void cmdUpdateSelected_Click(object sender, EventArgs e)
        {
            var item = default(DataGridItem);
            var objEventEmail = new EventEmails(PortalId, ModuleId, LocalResourceFile,
                                                ((PageBase) Page).PageCulture.Name);

            try
            {
                switch (rbModerate.SelectedValue)
                {
                    case "Events":
                        var objEventEmailInfo_1 = new EventEmailInfo();
                        objEventEmailInfo_1.TxtEmailSubject = txtEmailSubject.Text;
                        objEventEmailInfo_1.TxtEmailBody = txtEmailMessage.Text;
                        objEventEmailInfo_1.TxtEmailFrom = txtEmailFrom.Text;
                        var objCal = new EventInfo();
                        var objEventRecurMaster = default(EventRecurMasterInfo);
                        foreach (DataGridItem tempLoopVar_item in grdEvents.Items)
                        {
                            item = tempLoopVar_item;
                            switch (((RadioButtonList) item.FindControl("rbEventAction")).SelectedValue)
                            {
                                case "Approve":
                                    objCal = _objCtlEvent.EventsGet(
                                        Convert.ToInt32(grdEvents.DataKeys[item.ItemIndex]),
                                        ModuleId);
                                    objCal.Approved = true;
                                    var newEventEmailSent = objCal.NewEventEmailSent;
                                    objCal.NewEventEmailSent = true;
                                    _objCtlEvent.EventsSave(objCal, true, TabId, false);
                                    // Only send event emails when event approved for first time
                                    if (!newEventEmailSent)
                                    {
                                        objCal.RRULE = "";
                                        SendNewEventEmails(objCal);
                                        CreateNewEventJournal(objCal);
                                    }
                                    // Email Requesting/Moderated User
                                    if (chkEmail.Checked)
                                    {
                                        objCal.RRULE = "";
                                        objEventEmailInfo_1.UserIDs.Clear();
                                        objEventEmailInfo_1.UserIDs.Add(objCal.OwnerID);
                                        objEventEmail.SendEmails(objEventEmailInfo_1, objCal);
                                    }
                                    break;
                                case "Deny":
                                    objCal = _objCtlEvent.EventsGet(
                                        Convert.ToInt32(grdEvents.DataKeys[item.ItemIndex]),
                                        ModuleId);
                                    //Don't Allow Delete on Enrolled Event - Only Cancel
                                    objEventRecurMaster =
                                        _objCtlEventRecurMaster.EventsRecurMasterGet(
                                            objCal.RecurMasterID, objCal.ModuleID);
                                    if (objEventRecurMaster.RRULE != "")
                                    {
                                        objCal.Cancelled = true;
                                        objCal.LastUpdatedID = UserId;
                                        objCal = _objCtlEvent.EventsSave(objCal, false, TabId, true);
                                    }
                                    else
                                    {
                                        _objCtlEventRecurMaster.EventsRecurMasterDelete(
                                            objCal.RecurMasterID, objCal.ModuleID);
                                    }
                                    // Email Requesting/Moderated User
                                    if (chkEmail.Checked)
                                    {
                                        objCal.RRULE = "";
                                        objEventEmailInfo_1.UserIDs.Clear();
                                        objEventEmailInfo_1.UserIDs.Add(objCal.OwnerID);
                                        objEventEmail.SendEmails(objEventEmailInfo_1, objCal);
                                    }
                                    break;
                            }
                        }
                        foreach (DataGridItem tempLoopVar_item in grdRecurEvents.Items)
                        {
                            item = tempLoopVar_item;
                            switch (((RadioButtonList) item.FindControl("rbEventRecurAction")).SelectedValue)
                            {
                                case "Approve":
                                    objEventRecurMaster =
                                        _objCtlEventRecurMaster.EventsRecurMasterGet(
                                            Convert.ToInt32(grdRecurEvents.DataKeys[item.ItemIndex]),
                                            ModuleId);
                                    objEventRecurMaster.Approved = true;
                                    _objCtlEventRecurMaster.EventsRecurMasterSave(
                                        objEventRecurMaster, TabId, false);
                                    var lstEvents = default(ArrayList);
                                    lstEvents = _objCtlEvent.EventsGetRecurrences(
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
                                            _objCtlEvent.EventsSave(objCal, true, TabId, false);
                                            // Only send event emails when event approved for first time
                                            if (!newEventEmailSent && !blEmailSent)
                                            {
                                                objCal.RRULE = objEventRecurMaster.RRULE;
                                                SendNewEventEmails(objCal);
                                                CreateNewEventJournal(objCal);
                                                blEmailSent = true;
                                            }
                                        }
                                    }
                                    // Email Requesting/Moderated User
                                    if (chkEmail.Checked)
                                    {
                                        objCal.RRULE = objEventRecurMaster.RRULE;
                                        objEventEmailInfo_1.UserIDs.Clear();
                                        objEventEmailInfo_1.UserIDs.Add(objEventRecurMaster.CreatedByID);
                                        objEventEmail.SendEmails(objEventEmailInfo_1, objCal);
                                    }
                                    break;
                                case "Deny":
                                    objEventRecurMaster =
                                        _objCtlEventRecurMaster.EventsRecurMasterGet(
                                            Convert.ToInt32(grdRecurEvents.DataKeys[item.ItemIndex]),
                                            ModuleId);
                                    //Don't Allow Delete on Enrolled Event - Only Cancel
                                    _objCtlEventRecurMaster.EventsRecurMasterDelete(
                                        Convert.ToInt32(grdRecurEvents.DataKeys[item.ItemIndex]),
                                        ModuleId);
                                    // Email Requesting/Moderated User
                                    if (chkEmail.Checked)
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
                        foreach (DataGridItem tempLoopVar_item in grdEnrollment.Items)
                        {
                            item = tempLoopVar_item;
                            if (((RadioButtonList) item.FindControl("rbEnrollAction")).SelectedValue != "")
                            {
                                objEnroll = _objCtlEventSignups.EventsSignupsGet(
                                    Convert.ToInt32(grdEnrollment.DataKeys[item.ItemIndex]),
                                    ModuleId, false);
                                var objCtlEvent = new EventController();
                                var objEvent = objCtlEvent.EventsGet(objEnroll.EventID, objEnroll.ModuleID);
                                var objEventEmailInfo = new EventEmailInfo();
                                objEventEmailInfo.TxtEmailSubject = txtEmailSubject.Text;
                                objEventEmailInfo.TxtEmailFrom = txtEmailFrom.Text;
                                if (chkEmail.Checked)
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
                                        CreateEnrollment(objEnroll, objEvent);

                                        // Email Requesting/Moderated User
                                        if (Settings.SendEnrollMessageApproved)
                                        {
                                            objEventEmailInfo.UserIDs.Add(objEvent.OwnerID);
                                        }
                                        objEventEmailInfo.TxtEmailBody =
                                            txtEmailMessage.Text +
                                            Settings.Templates.txtEnrollMessageApproved;
                                        objEventEmail.SendEmails(objEventEmailInfo, objEvent, objEnroll);
                                        break;
                                    case "Deny":
                                        DeleteEnrollment(
                                            Convert.ToInt32(grdEnrollment.DataKeys[item.ItemIndex]),
                                            objEvent.ModuleID, objEvent.EventID);

                                        // Email Requesting/Moderated User
                                        if (Settings.SendEnrollMessageDenied)
                                        {
                                            objEventEmailInfo.UserIDs.Add(objEvent.OwnerID);
                                        }
                                        objEventEmailInfo.TxtEmailBody =
                                            txtEmailMessage.Text + Settings.Templates.txtEnrollMessageDenied;
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
            BindData();
        }

        protected void cmdSelectApproveAll_Click(object sender, EventArgs e)
        {
            BindData();
            var item = default(DataGridItem);
            if (rbModerate.SelectedValue == "Events")
            {
                foreach (DataGridItem tempLoopVar_item in grdEvents.Items)
                {
                    item = tempLoopVar_item;
                    ((RadioButtonList) item.FindControl("rbEventAction")).SelectedValue = "Approve";
                }
                foreach (DataGridItem tempLoopVar_item in grdRecurEvents.Items)
                {
                    item = tempLoopVar_item;
                    ((RadioButtonList) item.FindControl("rbEventRecurAction")).SelectedValue = "Approve";
                }
            }
            else
            {
                foreach (DataGridItem tempLoopVar_item in grdEnrollment.Items)
                {
                    item = tempLoopVar_item;
                    ((RadioButtonList) item.FindControl("rbEnrollAction")).SelectedValue = "Approve";
                }
            }
            cmdUpdateSelected_Click(sender, e);
        }

        protected void cmdSelectDenyAll_Click(object sender, EventArgs e)
        {
            BindData();
            var item = default(DataGridItem);
            if (rbModerate.SelectedValue == "Events")
            {
                foreach (DataGridItem tempLoopVar_item in grdEvents.Items)
                {
                    item = tempLoopVar_item;
                    ((RadioButtonList) item.FindControl("rbEventAction")).SelectedValue = "Deny";
                }
                foreach (DataGridItem tempLoopVar_item in grdRecurEvents.Items)
                {
                    item = tempLoopVar_item;
                    ((RadioButtonList) item.FindControl("rbEventRecurAction")).SelectedValue = "Deny";
                }
            }
            else
            {
                foreach (DataGridItem tempLoopVar_item in grdEnrollment.Items)
                {
                    item = tempLoopVar_item;
                    ((RadioButtonList) item.FindControl("rbEnrollAction")).SelectedValue = "Deny";
                }
            }
            cmdUpdateSelected_Click(sender, e);
        }

        protected void cmdUnmarkAll_Click(object sender, EventArgs e)
        {
            BindData();
            var item = default(DataGridItem);
            if (rbModerate.SelectedValue == "Events")
            {
                foreach (DataGridItem tempLoopVar_item in grdEvents.Items)
                {
                    item = tempLoopVar_item;
                    ((RadioButtonList) item.FindControl("rbEventAction")).SelectedValue = null;
                }
                foreach (DataGridItem tempLoopVar_item in grdRecurEvents.Items)
                {
                    item = tempLoopVar_item;
                    ((RadioButtonList) item.FindControl("rbEventRecurAction")).SelectedValue = null;
                }
            }
            else
            {
                foreach (DataGridItem tempLoopVar_item in grdEnrollment.Items)
                {
                    item = tempLoopVar_item;
                    ((RadioButtonList) item.FindControl("rbEnrollAction")).SelectedValue = null;
                }
            }
        }

        #endregion

        #region Grid and Other Events

        protected void rbModerate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rbModerate.SelectedValue == "Events")
            {
                grdEnrollment.Visible = false;
                grdEvents.Visible = true;
                grdRecurEvents.Visible = true;
            }
            else
            {
                grdEnrollment.Visible = true;
                grdEvents.Visible = false;
            }
            BindData();
        }

        public void grdEvents_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Select":
                    var itemID = Convert.ToInt32(grdEvents.DataKeys[e.Item.ItemIndex]);
                    var objEventInfoHelper =
                        new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
                    Response.Redirect(
                        objEventInfoHelper.AddSkinContainerControls(
                            Globals.NavigateURL(TabId, "Edit", "Mid=" + ModuleId, "ItemID=" + itemID,
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
                        new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
                    Response.Redirect(
                        objEventInfoHelper.AddSkinContainerControls(
                            Globals.NavigateURL(TabId, "Edit", "Mid=" + ModuleId, "ItemID=" + itemID,
                                                "EditRecur=All"), "?"));
                    break;
            }
        }


        public void grdEnrollment_ItemCommand(object sender, DataGridCommandEventArgs e)
        {
            var objEnroll = default(EventSignupsInfo);
            objEnroll = _objCtlEventSignups.EventsSignupsGet(
                Convert.ToInt32(grdEnrollment.DataKeys[e.Item.ItemIndex]), ModuleId, false);

            try
            {
                switch (e.CommandName)
                {
                    case "Select":
                        try
                        {
                            var itemID = objEnroll.EventID;
                            var objEventInfoHelper =
                                new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
                            Response.Redirect(
                                objEventInfoHelper.AddSkinContainerControls(
                                    EditUrl("ItemID", itemID.ToString(), "Edit"), "?"));
                        }
                        catch (Exception)
                        { }
                        break;
                    case "User":
                        var objCtlEvent = new EventController();
                        var objEvent = objCtlEvent.EventsGet(objEnroll.EventID, objEnroll.ModuleID);

                        var objEventEmailInfo = new EventEmailInfo();
                        var objEventEmail = new EventEmails(PortalId, ModuleId, LocalResourceFile,
                                                            ((PageBase) Page).PageCulture.Name);
                        objEventEmailInfo.TxtEmailSubject = txtEmailSubject.Text;
                        objEventEmailInfo.TxtEmailBody = txtEmailMessage.Text;
                        objEventEmailInfo.TxtEmailFrom = txtEmailFrom.Text;
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
            BindData();
        }

        protected void returnButton_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect(GetSocialNavigateUrl(), true);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion
    }
}