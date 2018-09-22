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
using Components;
using DotNetNuke.Framework;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using Microsoft.VisualBasic;
using Globals = DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Events
{
    public partial class EventMyEnrollments : EventBase
    {
        #region Event Handlers

        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //EVT-4499 if not login, redirect user to login page
                if (!Request.IsAuthenticated)
                {
                    RedirectToLogin();
                }

                LocalizeAll();

                // Setup Icon Bar for use
                SetUpIconBar(EventIcons, EventIcons2);

                lnkSelectedDelete.Attributes.Add(
                    "onclick",
                    "javascript:return confirm('" +
                    Localization.GetString("ConfirmDeleteSelected", LocalResourceFile) + "');");

                if (Page.IsPostBack == false)
                {
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

        #endregion

        #region Helper Methods

        private void BindData()
        {
            try
            {
                var moduleStartDate = DateAndTime.DateAdd(DateInterval.Day,
                                                          Convert.ToDouble(-Settings.EnrolListDaysBefore),
                                                          ModuleNow());
                var moduleEndDate =
                    DateAndTime.DateAdd(DateInterval.Day, Settings.EnrolListDaysAfter, ModuleNow());
                var displayStartDate = DateAndTime.DateAdd(DateInterval.Day,
                                                           Convert.ToDouble(-Settings.EnrolListDaysBefore),
                                                           DisplayNow());
                var displayEndDate =
                    DateAndTime.DateAdd(DateInterval.Day, Settings.EnrolListDaysAfter, DisplayNow());

                //Default sort from settings
                var sortDirection = Settings.EnrolListSortDirection;
                var sortExpression = GetSignupsSortExpression("EventTimeBegin");

                var inCategoryIDs = new ArrayList();
                inCategoryIDs.Add("-1");
                var objEventInfoHelper = new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
                var categoryIDs = objEventInfoHelper.CreateCategoryFilter(inCategoryIDs);

                var eventSignups = default(ArrayList);
                var objCtlEventSignups = new EventSignupsController();

                eventSignups =
                    objCtlEventSignups.EventsSignupsMyEnrollments(ModuleId, UserId, GetUrlGroupId(),
                                                                  categoryIDs, moduleStartDate, moduleEndDate);

                var objEventTimeZoneUtilities = new EventTimeZoneUtilities();
                var displayEventSignups = new ArrayList();
                foreach (EventSignupsInfo eventSignup in eventSignups)
                {
                    var displayTimeZoneId = GetDisplayTimeZoneId();
                    eventSignup.EventTimeBegin = objEventTimeZoneUtilities
                        .ConvertToDisplayTimeZone(eventSignup.EventTimeBegin, eventSignup.EventTimeZoneId,
                                                  PortalId, displayTimeZoneId).EventDate;
                    eventSignup.EventTimeEnd = objEventTimeZoneUtilities
                        .ConvertToDisplayTimeZone(eventSignup.EventTimeEnd, eventSignup.EventTimeZoneId, PortalId,
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
                grdEnrollment.DataSource = displayEventSignups;
                grdEnrollment.DataBind();
                if (eventSignups.Count < 1)
                {
                    divMessage.Visible = true;
                    grdEnrollment.Visible = false;
                    //"No Events/Enrollments found..."
                    lblMessage.Text = Localization.GetString("MsgNoMyEventsOrEnrollment", LocalResourceFile);
                }
                else
                {
                    for (var i = 0; i <= eventSignups.Count - 1; i++)
                    {
                        var decTotal = Convert.ToDecimal(grdEnrollment.Items[i].Cells[7].Text) /
                                       Convert.ToDecimal(grdEnrollment.Items[i].Cells[8].Text);
                        var dtStartTime = Convert.ToDateTime(grdEnrollment.Items[i].Cells[1].Text);
                        // ReSharper disable LocalizableElement
                        ((Label) grdEnrollment.Items[i].FindControl("lblAmount")).Text =
                            string.Format("{0:F2}", decTotal) + " " + PortalSettings.Currency;
                        ((Label) grdEnrollment.Items[i].FindControl("lblTotal")).Text =
                            string.Format("{0:F2}", Convert.ToDecimal(grdEnrollment.Items[i].Cells[7].Text)) +
                            " " + PortalSettings.Currency;
                        // ReSharper restore LocalizableElement
                        if (decTotal > 0 || dtStartTime < ModuleNow().AddDays(Settings.Enrolcanceldays))
                        {
                            ((CheckBox) grdEnrollment.Items[i].FindControl("chkSelect")).Enabled = false;
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
            grdEnrollment.Columns[0].HeaderText = Localization.GetString("plSelect", LocalResourceFile);
            grdEnrollment.Columns[1].HeaderText = Localization.GetString("plDate", LocalResourceFile);
            grdEnrollment.Columns[2].HeaderText = Localization.GetString("plTime", LocalResourceFile);
            grdEnrollment.Columns[3].HeaderText = Localization.GetString("plEvent", LocalResourceFile);
            grdEnrollment.Columns[4].HeaderText = Localization.GetString("plApproved", LocalResourceFile);
            grdEnrollment.Columns[6].HeaderText = Localization.GetString("plAmount", LocalResourceFile);
            grdEnrollment.Columns[8].HeaderText = Localization.GetString("plNoEnrolees", LocalResourceFile);
            grdEnrollment.Columns[9].HeaderText = Localization.GetString("plTotal", LocalResourceFile);
            lnkSelectedDelete.ToolTip =
                string.Format(Localization.GetString("CancelEnrolments", LocalResourceFile),
                              Settings.Enrolcanceldays);
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
                            Convert.ToInt32(grdEnrollment.DataKeys[e.Item.ItemIndex]), ModuleId, false);
                        var iItemID = objEnroll.EventID;

                        var objEventInfoHelper =
                            new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
                        Response.Redirect(
                            objEventInfoHelper.GetDetailPageRealURL(
                                iItemID, GetUrlGroupId(), GetUrlUserId()));
                        break;
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            BindData();
        }

        protected void lnkSelectedDelete_Click(object sender, EventArgs e)
        {
            var item = default(DataGridItem);
            var objEnroll = default(EventSignupsInfo);
            var objCtlEventSignups = new EventSignupsController();
            var objEvent = new EventInfo();
            var objCtlEvent = new EventController();
            var eventID = 0;

            foreach (DataGridItem tempLoopVar_item in grdEnrollment.Items)
            {
                item = tempLoopVar_item;
                if (((CheckBox) item.FindControl("chkSelect")).Checked)
                {
                    objEnroll = objCtlEventSignups.EventsSignupsGet(
                        Convert.ToInt32(grdEnrollment.DataKeys[item.ItemIndex]), ModuleId, false);
                    if (eventID != objEnroll.EventID)
                    {
                        objEvent = objCtlEvent.EventsGet(objEnroll.EventID, ModuleId);
                    }
                    eventID = objEnroll.EventID;

                    // Delete Selected Enrollee
                    DeleteEnrollment(Convert.ToInt32(grdEnrollment.DataKeys[item.ItemIndex]),
                                          objEvent.ModuleID, objEvent.EventID);

                    // Mail users
                    if (Settings.SendEnrollMessageDeleted)
                    {
                        var objEventEmailInfo = new EventEmailInfo();
                        var objEventEmail = new EventEmails(PortalId, ModuleId, LocalResourceFile,
                                                            ((PageBase) Page).PageCulture.Name);
                        objEventEmailInfo.TxtEmailSubject = Settings.Templates.txtEnrollMessageSubject;
                        objEventEmailInfo.TxtEmailBody = Settings.Templates.txtEnrollMessageDeleted;
                        objEventEmailInfo.TxtEmailFrom = Settings.StandardEmail;
                        objEventEmailInfo.UserEmails.Add(PortalSettings.UserInfo.Email);
                        objEventEmailInfo.UserLocales.Add(PortalSettings.UserInfo.Profile.PreferredLocale);
                        objEventEmailInfo.UserTimeZoneIds.Add(PortalSettings.UserInfo.Profile.PreferredTimeZone
                                                                  .Id);
                        objEventEmailInfo.UserIDs.Add(objEvent.OwnerID);
                        objEventEmail.SendEmails(objEventEmailInfo, objEvent, objEnroll);
                    }
                }
            }

            BindData();
        }

        protected void returnButton_Click(object sender, EventArgs e)
        {
            var cntrl = Settings.DefaultView.Split('.');
            var socialGroupId = GetUrlGroupId();
            if (socialGroupId > 0)
            {
                Response.Redirect(Globals.NavigateURL(TabId, "", "mctl=" + cntrl[0],
                                                           "ModuleID=" + ModuleId, "groupid=" + socialGroupId));
            }
            else
            {
                Response.Redirect(Globals.NavigateURL(TabId, "", "mctl=" + cntrl[0],
                                                           "ModuleID=" + ModuleId));
            }
        }

        #endregion
    }
}