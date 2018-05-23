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
    using DotNetNuke.Framework;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using global::Components;
    using Microsoft.VisualBasic;
    using Globals = DotNetNuke.Common.Globals;

    public partial class EventMyEnrollments : EventBase
    {
        #region Event Handlers

        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //EVT-4499 if not login, redirect user to login page
                if (!this.Request.IsAuthenticated)
                {
                    this.RedirectToLogin();
                }

                this.LocalizeAll();

                // Setup Icon Bar for use
                this.SetUpIconBar(this.EventIcons, this.EventIcons2);

                this.lnkSelectedDelete.Attributes.Add(
                    "onclick",
                    "javascript:return confirm('" +
                    Localization.GetString("ConfirmDeleteSelected", this.LocalResourceFile) + "');");

                if (this.Page.IsPostBack == false)
                {
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

        #endregion

        #region Helper Methods

        private void BindData()
        {
            try
            {
                var moduleStartDate = DateAndTime.DateAdd(DateInterval.Day,
                                                          Convert.ToDouble(-this.Settings.EnrolListDaysBefore),
                                                          this.ModuleNow());
                var moduleEndDate =
                    DateAndTime.DateAdd(DateInterval.Day, this.Settings.EnrolListDaysAfter, this.ModuleNow());
                var displayStartDate = DateAndTime.DateAdd(DateInterval.Day,
                                                           Convert.ToDouble(-this.Settings.EnrolListDaysBefore),
                                                           this.DisplayNow());
                var displayEndDate =
                    DateAndTime.DateAdd(DateInterval.Day, this.Settings.EnrolListDaysAfter, this.DisplayNow());

                //Default sort from settings
                var sortDirection = this.Settings.EnrolListSortDirection;
                var sortExpression = this.GetSignupsSortExpression("EventTimeBegin");

                var inCategoryIDs = new ArrayList();
                inCategoryIDs.Add("-1");
                var objEventInfoHelper = new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);
                var categoryIDs = objEventInfoHelper.CreateCategoryFilter(inCategoryIDs);

                var eventSignups = default(ArrayList);
                var objCtlEventSignups = new EventSignupsController();

                eventSignups =
                    objCtlEventSignups.EventsSignupsMyEnrollments(this.ModuleId, this.UserId, this.GetUrlGroupId(),
                                                                  categoryIDs, moduleStartDate, moduleEndDate);

                var objEventTimeZoneUtilities = new EventTimeZoneUtilities();
                var displayEventSignups = new ArrayList();
                foreach (EventSignupsInfo eventSignup in eventSignups)
                {
                    var displayTimeZoneId = this.GetDisplayTimeZoneId();
                    eventSignup.EventTimeBegin = objEventTimeZoneUtilities
                        .ConvertToDisplayTimeZone(eventSignup.EventTimeBegin, eventSignup.EventTimeZoneId,
                                                  this.PortalId, displayTimeZoneId).EventDate;
                    eventSignup.EventTimeEnd = objEventTimeZoneUtilities
                        .ConvertToDisplayTimeZone(eventSignup.EventTimeEnd, eventSignup.EventTimeZoneId, this.PortalId,
                                                  displayTimeZoneId).EventDate;
                    if (eventSignup.EventTimeBegin > displayEndDate || eventSignup.EventTimeEnd < displayStartDate)
                    {
                        continue;
                    }
                    displayEventSignups.Add(eventSignup);
                }

                EventSignupsInfo.SortExpression = sortExpression;
                EventSignupsInfo.SortDirection = sortDirection;
                displayEventSignups.Sort();

                //Get data for selected date and fill grid
                this.grdEnrollment.DataSource = displayEventSignups;
                this.grdEnrollment.DataBind();
                if (eventSignups.Count < 1)
                {
                    this.divMessage.Visible = true;
                    this.grdEnrollment.Visible = false;
                    //"No Events/Enrollments found..."
                    this.lblMessage.Text = Localization.GetString("MsgNoMyEventsOrEnrollment", this.LocalResourceFile);
                }
                else
                {
                    for (var i = 0; i <= eventSignups.Count - 1; i++)
                    {
                        var decTotal = Convert.ToDecimal(this.grdEnrollment.Items[i].Cells[7].Text) /
                                       Convert.ToDecimal(this.grdEnrollment.Items[i].Cells[8].Text);
                        var dtStartTime = Convert.ToDateTime(this.grdEnrollment.Items[i].Cells[1].Text);
                        // ReSharper disable LocalizableElement
                        ((Label) this.grdEnrollment.Items[i].FindControl("lblAmount")).Text =
                            string.Format("{0:F2}", decTotal) + " " + this.PortalSettings.Currency;
                        ((Label) this.grdEnrollment.Items[i].FindControl("lblTotal")).Text =
                            string.Format("{0:F2}", Convert.ToDecimal(this.grdEnrollment.Items[i].Cells[7].Text)) +
                            " " + this.PortalSettings.Currency;
                        // ReSharper restore LocalizableElement
                        if (decTotal > 0 || dtStartTime < this.ModuleNow().AddDays(this.Settings.Enrolcanceldays))
                        {
                            ((CheckBox) this.grdEnrollment.Items[i].FindControl("chkSelect")).Enabled = false;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void LocalizeAll()
        {
            this.grdEnrollment.Columns[0].HeaderText = Localization.GetString("plSelect", this.LocalResourceFile);
            this.grdEnrollment.Columns[1].HeaderText = Localization.GetString("plDate", this.LocalResourceFile);
            this.grdEnrollment.Columns[2].HeaderText = Localization.GetString("plTime", this.LocalResourceFile);
            this.grdEnrollment.Columns[3].HeaderText = Localization.GetString("plEvent", this.LocalResourceFile);
            this.grdEnrollment.Columns[4].HeaderText = Localization.GetString("plApproved", this.LocalResourceFile);
            this.grdEnrollment.Columns[6].HeaderText = Localization.GetString("plAmount", this.LocalResourceFile);
            this.grdEnrollment.Columns[8].HeaderText = Localization.GetString("plNoEnrolees", this.LocalResourceFile);
            this.grdEnrollment.Columns[9].HeaderText = Localization.GetString("plTotal", this.LocalResourceFile);
            this.lnkSelectedDelete.ToolTip =
                string.Format(Localization.GetString("CancelEnrolments", this.LocalResourceFile),
                              this.Settings.Enrolcanceldays);
        }

        #endregion

        #region Grid and Other Events

        public void grdEnrollment_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            try
            {
                switch (e.CommandName)
                {
                    case "Select":
                        var objEnroll = default(EventSignupsInfo);
                        var objCtlEventSignups = new EventSignupsController();
                        objEnroll = objCtlEventSignups.EventsSignupsGet(
                            Convert.ToInt32(this.grdEnrollment.DataKeys[e.Item.ItemIndex]), this.ModuleId, false);
                        var iItemID = objEnroll.EventID;

                        var objEventInfoHelper =
                            new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);
                        this.Response.Redirect(
                            objEventInfoHelper.GetDetailPageRealURL(
                                iItemID, this.GetUrlGroupId(), this.GetUrlUserId()));
                        break;
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            this.BindData();
        }

        protected void lnkSelectedDelete_Click(object sender, EventArgs e)
        {
            var item = default(DataGridItem);
            var objEnroll = default(EventSignupsInfo);
            var objCtlEventSignups = new EventSignupsController();
            var objEvent = new EventInfo();
            var objCtlEvent = new EventController();
            var eventID = 0;

            foreach (DataGridItem tempLoopVar_item in this.grdEnrollment.Items)
            {
                item = tempLoopVar_item;
                if (((CheckBox) item.FindControl("chkSelect")).Checked)
                {
                    objEnroll = objCtlEventSignups.EventsSignupsGet(
                        Convert.ToInt32(this.grdEnrollment.DataKeys[item.ItemIndex]), this.ModuleId, false);
                    if (eventID != objEnroll.EventID)
                    {
                        objEvent = objCtlEvent.EventsGet(objEnroll.EventID, this.ModuleId);
                    }
                    eventID = objEnroll.EventID;

                    // Delete Selected Enrollee
                    this.DeleteEnrollment(Convert.ToInt32(this.grdEnrollment.DataKeys[item.ItemIndex]),
                                          objEvent.ModuleID, objEvent.EventID);

                    // Mail users
                    if (this.Settings.SendEnrollMessageDeleted)
                    {
                        var objEventEmailInfo = new EventEmailInfo();
                        var objEventEmail = new EventEmails(this.PortalId, this.ModuleId, this.LocalResourceFile,
                                                            ((PageBase) this.Page).PageCulture.Name);
                        objEventEmailInfo.TxtEmailSubject = this.Settings.Templates.txtEnrollMessageSubject;
                        objEventEmailInfo.TxtEmailBody = this.Settings.Templates.txtEnrollMessageDeleted;
                        objEventEmailInfo.TxtEmailFrom = this.Settings.StandardEmail;
                        objEventEmailInfo.UserEmails.Add(this.PortalSettings.UserInfo.Email);
                        objEventEmailInfo.UserLocales.Add(this.PortalSettings.UserInfo.Profile.PreferredLocale);
                        objEventEmailInfo.UserTimeZoneIds.Add(this.PortalSettings.UserInfo.Profile.PreferredTimeZone
                                                                  .Id);
                        objEventEmailInfo.UserIDs.Add(objEvent.OwnerID);
                        objEventEmail.SendEmails(objEventEmailInfo, objEvent, objEnroll);
                    }
                }
            }

            this.BindData();
        }

        protected void returnButton_Click(object sender, EventArgs e)
        {
            var cntrl = this.Settings.DefaultView.Split('.');
            var socialGroupId = this.GetUrlGroupId();
            if (socialGroupId > 0)
            {
                this.Response.Redirect(Globals.NavigateURL(this.TabId, "", "mctl=" + cntrl[0],
                                                           "ModuleID=" + this.ModuleId, "groupid=" + socialGroupId));
            }
            else
            {
                this.Response.Redirect(Globals.NavigateURL(this.TabId, "", "mctl=" + cntrl[0],
                                                           "ModuleID=" + this.ModuleId));
            }
        }

        #endregion
    }
}