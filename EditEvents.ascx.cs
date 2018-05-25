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
    using System.Globalization;
    using System.Reflection;
    using System.Threading;
    using System.Web.UI.WebControls;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Users;
    using DotNetNuke.Framework;
    using DotNetNuke.Security;
    using DotNetNuke.Security.Permissions;
    using DotNetNuke.Security.Roles;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.FileSystem;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.Web.UI.WebControls.Extensions;
    using global::Components;
    using Microsoft.VisualBasic;
    using EventInfo = global::Components.EventInfo;
    using Globals = DotNetNuke.Common.Globals;

    [DNNtc.ModuleControlProperties("Edit", "Edit Events", DNNtc.ControlType.View, "https://dnnevents.codeplex.com/documentation", false, true)]
    public partial class EditEvents : EventBase
    {
        #region Private Area

        private int _itemID = -1;
        private bool _editRecur = true;
        private readonly EventController _objCtlEvent = new EventController();
        private readonly EventRecurMasterController _objCtlEventRecurMaster = new EventRecurMasterController();
        private EventInfo _objEvent = new EventInfo();
        private EventSignupsInfo _objEventSignups = new EventSignupsInfo();
        private readonly EventSignupsController _objCtlEventSignups = new EventSignupsController();
        private ArrayList _lstEvents = new ArrayList();
        private readonly ArrayList _lstOwnerUsers = new ArrayList();
        private readonly CultureInfo _culture = Thread.CurrentThread.CurrentCulture;
        private const string RecurTableDisplayType = "inline-block";

        #endregion

        #region Event Handlers

        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Verify that the current user has edit access to this module
                if (PortalSecurity.IsInRole(this.PortalSettings.AdministratorRoleName) || this.IsModuleEditor())
                { }
                else
                {
                    // to stop errors when not authorised to edit
                    this.valReminderTime.MinimumValue = "15";
                    this.valReminderTime.MaximumValue = "60";

                    this.Response.Redirect(this.GetSocialNavigateUrl(), true);
                }

                this.grdAddUser.ModuleConfiguration = this.ModuleConfiguration.Clone();

                // Add the external Validation.js to the Page
                const string csname = "ExtValidationScriptFile";
                var cstype = MethodBase.GetCurrentMethod().GetType();
                var cstext = "<script src=\"" + this.ResolveUrl("~/DesktopModules/Events/Scripts/Validation.js") +
                             "\" type=\"text/javascript\"></script>";
                if (!this.Page.ClientScript.IsClientScriptBlockRegistered(csname))
                {
                    this.Page.ClientScript.RegisterClientScriptBlock(cstype, csname, cstext, false);
                }

                // Determine ItemId of Event to Update
                if (!ReferenceEquals(this.Request.Params["ItemId"], null))
                {
                    this._itemID = int.Parse(this.Request.Params["ItemId"]);
                }
                this._editRecur = false;
                if (!ReferenceEquals(this.Request.Params["EditRecur"], null))
                {
                    if (this.Request.Params["EditRecur"].ToLower() == "all")
                    {
                        this._editRecur = true;
                    }
                }

                // Set the selected theme
                this.SetTheme(this.pnlEventsModuleEdit);

                //EPT: "Changed DotNetNuke.Security.PortalSecurity.HasEditPermissions(ModuleId)" into "IsEditable"
                //RWJS: Replaced with custom function IsModuleEditor which checks whether users has editor permissions
                if (this.IsModuleEditor() ||
                    this.IsModerator() ||
                    PortalSecurity.IsInRole(this.PortalSettings.AdministratorRoleName))
                { }
                else
                {
                    this.Response.Redirect(this.GetSocialNavigateUrl(), true);
                }

                this.trOwner.Visible = false;
                if (this.IsModerator() && this.Settings.Ownerchangeallowed ||
                    PortalSecurity.IsInRole(this.PortalSettings.AdministratorRoleName))
                {
                    this.trOwner.Visible = true;
                }

                this.pnlEnroll.Visible = false;
                this.divNoEnrolees.Visible = false;

                if (this.Settings.Eventsignup)
                {
                    this.pnlEnroll.Visible = true;
                    this.chkSignups.Visible = true;
                    if (this.Settings.Maxnoenrolees > 1 && this.divAddUser.Visible)
                    {
                        this.divNoEnrolees.Visible = true;
                    }
                }

                this.trTypeOfEnrollment.Visible = this.Settings.Eventsignupallowpaid;
                this.trPayPalAccount.Visible = this.Settings.Eventsignupallowpaid;

                this.tblEventEmail.Attributes.Add("style", "display:none; width:100%");
                if (!this.Settings.Newpereventemail)
                {
                    this.pnlEventEmailRole.Visible = false;
                }
                else if (this._itemID == -1 && this.Settings.Moderateall && !this.IsModerator())
                {
                    this.pnlEventEmailRole.Visible = false;
                }

                this.pnlReminder.Visible = this.Settings.Eventnotify;
                this.pnlImage.Visible = this.Settings.Eventimage;
                this.pnlDetailPage.Visible = this.Settings.DetailPageAllowed;

                // Setup Popup Event
                this.dpStartDate.ClientEvents.OnDateSelected =
                    "function() {if (Page_ClientValidate('startdate')) CopyField('" + this.dpStartDate.ClientID +
                    "','" + this.dpEndDate.ClientID + "');}";
                this.tpStartTime.ClientEvents.OnDateSelected =
                    "function() {SetComboIndex('" + this.tpStartTime.ClientID + "','" + this.tpEndTime.ClientID +
                    "','" + this.dpStartDate.ClientID + "','" + this.dpEndDate.ClientID + "','" +
                    this.Settings.Timeinterval + "');}";
                this.ctlURL.FileFilter = Globals.glbImageFileTypes;
                if (!this.Page.IsPostBack)
                {
                    this.txtSubject.MaxLength++;
                    this.txtReminder.MaxLength++;
                }
                var limitSubject = "javascript:limitText(this," + (this.txtSubject.MaxLength - 1) + ",'" +
                                   Localization.GetString("LimitChars", this.LocalResourceFile) + "');";
                var limitReminder = "javascript:limitText(this," + (this.txtReminder.MaxLength - 1) + ",'" +
                                    Localization.GetString("LimitChars", this.LocalResourceFile) + "');";
                this.txtSubject.Attributes.Add("onkeydown", limitSubject);
                this.txtSubject.Attributes.Add("onkeyup", limitSubject);
                this.txtReminder.Attributes.Add("onkeydown", limitReminder);
                this.txtReminder.Attributes.Add("onkeyup", limitReminder);

                this.Page.ClientScript.RegisterExpandoAttribute(this.valValidStartTime2.ClientID, "TimeInterval",
                                                                this.Settings.Timeinterval);
                this.Page.ClientScript.RegisterExpandoAttribute(this.valValidStartTime2.ClientID, "ErrorMessage",
                                                                string.Format(
                                                                    Localization.GetString(
                                                                        "valValidStartTime2", this.LocalResourceFile),
                                                                    this.Settings.Timeinterval));
                this.Page.ClientScript.RegisterExpandoAttribute(this.valValidStartTime2.ClientID, "ClientID",
                                                                this.tpStartTime.ClientID);
                this.Page.ClientScript.RegisterExpandoAttribute(this.valValidEndTime2.ClientID, "TimeInterval",
                                                                this.Settings.Timeinterval);
                this.Page.ClientScript.RegisterExpandoAttribute(this.valValidEndTime2.ClientID, "ErrorMessage",
                                                                string.Format(
                                                                    Localization.GetString(
                                                                        "valValidEndTime2", this.LocalResourceFile),
                                                                    this.Settings.Timeinterval));
                this.Page.ClientScript.RegisterExpandoAttribute(this.valValidEndTime2.ClientID, "ClientID",
                                                                this.tpEndTime.ClientID);

                // If the page is being requested the first time, determine if an
                // contact itemId value is specified, and if so populate page
                // contents with the contact details
                if (!this.Page.IsPostBack)
                {
                    this.LocalizeAll();
                    this.LoadEvent();
                }
                else
                {
                    var url = Convert.ToString(this.ctlURL.Url);
                    var urlType = Convert.ToString(this.ctlURL.UrlType);
                    this.ctlURL.Url = url;
                    this.ctlURL.UrlType = urlType;
                }

                if (this.chkReminder.Checked)
                {
                    this.tblReminderDetail.Attributes.Add("style", "display:block;");
                }
                else
                {
                    this.tblReminderDetail.Attributes.Add("style", "display:none;");
                }

                if (this.chkDetailPage.Checked)
                {
                    this.tblDetailPageDetail.Attributes.Add("style", "display:block;");
                }
                else
                {
                    this.tblDetailPageDetail.Attributes.Add("style", "display:none;");
                }

                if (this.chkDisplayImage.Checked)
                {
                    this.tblImageURL.Attributes.Add("style", "display:block;");
                }
                else
                {
                    this.tblImageURL.Attributes.Add("style", "display:none;");
                }

                if (this.chkSignups.Checked)
                {
                    this.tblEnrollmentDetails.Attributes.Add("style", "display:block;");
                }
                else
                {
                    this.tblEnrollmentDetails.Attributes.Add("style", "display:none;");
                }

                if (this.chkReccuring.Checked)
                {
                    this.tblRecurringDetails.Attributes.Add("style", "display:block;");
                }
                else
                {
                    this.tblRecurringDetails.Attributes.Add("style", "display:none;");
                }

                if (this.chkEventEmailChk.Checked)
                {
                    this.tblEventEmailRoleDetail.Attributes.Add("style", "display:block;");
                }
                else
                {
                    this.tblEventEmailRoleDetail.Attributes.Add("style", "display:none;");
                }

                if (this.rblRepeatTypeP1.Checked)
                {
                    this.tblDetailP1.Attributes.Add(
                        "style", "display:" + RecurTableDisplayType + ";white-space:nowrap;");
                }
                else
                {
                    this.tblDetailP1.Attributes.Add("style", "display:none;white-space:nowrap;");
                }

                if (this.rblRepeatTypeW1.Checked)
                {
                    this.tblDetailW1.Attributes.Add("style", "display:" + RecurTableDisplayType + ";");
                }
                else
                {
                    this.tblDetailW1.Attributes.Add("style", "display:none;");
                }

                if (this.rblRepeatTypeM.Checked)
                {
                    this.tblDetailM1.Attributes.Add("style", "display:" + RecurTableDisplayType + ";");
                }
                else
                {
                    this.tblDetailM1.Attributes.Add("style", "display:none;");
                }

                if (this.rblRepeatTypeY1.Checked)
                {
                    this.tblDetailY1.Attributes.Add("style", "display:" + RecurTableDisplayType + ";");
                }
                else
                {
                    this.tblDetailY1.Attributes.Add("style", "display:none;");
                }

                if (this.dpY1Period.SelectedDate.ToString().Length == 0)
                {
                    this.dpY1Period.SelectedDate = this.SelectedDate.Date;
                }
                if (this.txtReminderFrom.Text.Length == 0)
                {
                    this.txtReminderFrom.Text = this.Settings.Reminderfrom;
                }
                if (this.txtEventEmailFrom.Text.Length == 0)
                {
                    this.txtEventEmailFrom.Text = this.Settings.Reminderfrom;
                }

                if (this.chkAllDayEvent.Checked)
                {
                    this.divStartTime.Attributes.Add("style", "display:none;");
                    this.divEndTime.Attributes.Add("style", "display:none;");
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void valValidStartDate3_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (ReferenceEquals(this.dpStartDate.SelectedDate, null))
            {
                args.IsValid = false;
                this.valValidStartDate.Visible = true;
            }
        }

        protected void valValidStartTime2_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var inDate = Convert.ToDateTime(this.tpStartTime.SelectedDate);
            this.valValidStartTime2.ErrorMessage =
                string.Format(Localization.GetString("valValidStartTime2", this.LocalResourceFile),
                              this.Settings.Timeinterval);
            args.IsValid = this.ValidateTime(inDate);
        }

        protected void valValidEndTime2_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var inDate = Convert.ToDateTime(this.tpEndTime.SelectedDate);
            this.valValidEndTime2.ErrorMessage =
                string.Format(Localization.GetString("valValidEndTime2", this.LocalResourceFile),
                              this.Settings.Timeinterval);
            args.IsValid = this.ValidateTime(inDate);
        }

        private void valValidRecurEndDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (ReferenceEquals(this.dpStartDate.SelectedDate, null))
            {
                return;
            }
            var recurDate = Convert.ToDateTime(this.dpRecurEndDate.SelectedDate);
            var startDate = Convert.ToDateTime(this.dpStartDate.SelectedDate);
            if (recurDate < startDate && !this.rblRepeatTypeN.Checked)
            {
                args.IsValid = false;
                this.valValidRecurEndDate.Visible = true;
            }
            else
            {
                args.IsValid = true;
                this.valValidRecurEndDate.Visible = false;
            }
        }

        #endregion

        #region Helper Methods and Functions

        private void LocalizeAll()
        {
            var culture = Thread.CurrentThread.CurrentCulture;

            this.txtSubject.Text = this.Settings.Templates.txtSubject;
            this.txtReminder.Text = this.Settings.Templates.txtMessage;

            this.grdEnrollment.Columns[0].HeaderText = Localization.GetString("Select", this.LocalResourceFile);
            this.grdEnrollment.Columns[1].HeaderText = Localization.GetString("EnrollUserName", this.LocalResourceFile);
            this.grdEnrollment.Columns[2].HeaderText =
                Localization.GetString("EnrollDisplayName", this.LocalResourceFile);
            this.grdEnrollment.Columns[3].HeaderText = Localization.GetString("EnrollEmail", this.LocalResourceFile);
            this.grdEnrollment.Columns[4].HeaderText = Localization.GetString("EnrollPhone", this.LocalResourceFile);
            this.grdEnrollment.Columns[5].HeaderText = Localization.GetString("EnrollApproved", this.LocalResourceFile);
            this.grdEnrollment.Columns[6].HeaderText = Localization.GetString("EnrollNo", this.LocalResourceFile);
            this.grdEnrollment.Columns[7].HeaderText = Localization.GetString("EventStart", this.LocalResourceFile);

            this.chkW1Sun.Text = culture.DateTimeFormat.AbbreviatedDayNames[(int) DayOfWeek.Sunday];
            this.chkW1Sun2.Text = culture.DateTimeFormat.AbbreviatedDayNames[(int) DayOfWeek.Sunday];
            this.chkW1Mon.Text = culture.DateTimeFormat.AbbreviatedDayNames[(int) DayOfWeek.Monday];
            this.chkW1Tue.Text = culture.DateTimeFormat.AbbreviatedDayNames[(int) DayOfWeek.Tuesday];
            this.chkW1Wed.Text = culture.DateTimeFormat.AbbreviatedDayNames[(int) DayOfWeek.Wednesday];
            this.chkW1Thu.Text = culture.DateTimeFormat.AbbreviatedDayNames[(int) DayOfWeek.Thursday];
            this.chkW1Fri.Text = culture.DateTimeFormat.AbbreviatedDayNames[(int) DayOfWeek.Friday];
            this.chkW1Sat.Text = culture.DateTimeFormat.AbbreviatedDayNames[(int) DayOfWeek.Saturday];

            this.cmbM1Period.Items.Clear();
            // Corrected a problem w/Every nth Week on a specific day with the following
            this.cmbM1Period.Items.Add(new ListItem(culture.DateTimeFormat.GetDayName(DayOfWeek.Sunday), "0"));
            this.cmbM1Period.Items.Add(new ListItem(culture.DateTimeFormat.GetDayName(DayOfWeek.Monday), "1"));
            this.cmbM1Period.Items.Add(new ListItem(culture.DateTimeFormat.GetDayName(DayOfWeek.Tuesday), "2"));
            this.cmbM1Period.Items.Add(new ListItem(culture.DateTimeFormat.GetDayName(DayOfWeek.Wednesday), "3"));
            this.cmbM1Period.Items.Add(new ListItem(culture.DateTimeFormat.GetDayName(DayOfWeek.Thursday), "4"));
            this.cmbM1Period.Items.Add(new ListItem(culture.DateTimeFormat.GetDayName(DayOfWeek.Friday), "5"));
            this.cmbM1Period.Items.Add(new ListItem(culture.DateTimeFormat.GetDayName(DayOfWeek.Saturday), "6"));

            this.cmbM2Period.Items.Clear();
            for (var i = 1; i <= 31; i++)
            {
                this.cmbM2Period.Items.Add(new ListItem(Localization.GetString(i.ToString(), this.LocalResourceFile),
                                                        i.ToString()));
            }

            this.lblMaxRecurrences.Text =
                string.Format(Localization.GetString("lblMaxRecurrences", this.LocalResourceFile),
                              this.Settings.Maxrecurrences);

            if (culture.DateTimeFormat.FirstDayOfWeek == DayOfWeek.Sunday)
            {
                this.chkW1Sun.Attributes.Add("style", "display:inline;");
                this.chkW1Sun2.Attributes.Add("style", "display:none;");
            }
            else
            {
                this.chkW1Sun2.Attributes.Add("style", "display:inline;");
                this.chkW1Sun.Attributes.Add("style", "display:none;");
            }

            this.dpStartDate.DatePopupButton.ToolTip =
                Localization.GetString("DatePickerTooltip", this.LocalResourceFile);
            this.dpEndDate.DatePopupButton.ToolTip =
                Localization.GetString("DatePickerTooltip", this.LocalResourceFile);
            this.dpRecurEndDate.DatePopupButton.ToolTip =
                Localization.GetString("DatePickerTooltip", this.LocalResourceFile);
            this.dpY1Period.DatePopupButton.ToolTip =
                Localization.GetString("DatePickerTooltip", this.LocalResourceFile);

            this.tpEndTime.TimePopupButton.ToolTip =
                Localization.GetString("TimePickerTooltip", this.LocalResourceFile);
        }

        public void LoadEvent()
        {
            this.StorePrevPageInViewState();

            this.pnlRecurring.Visible = true;
            this.lblMaxRecurrences.Visible = false;
            if (!this.Settings.Allowreoccurring)
            {
                this.dpRecurEndDate.Enabled = false;
                this.cmbP1Period.Enabled = false;
                this.txtP1Every.Enabled = false;
                this.dpRecurEndDate.Visible = false;
                this.cmbP1Period.Visible = false;
                this.txtP1Every.Visible = false;
                this.pnlRecurring.Visible = false;
            }
            else
            {
                if (this.Settings.Maxrecurrences != "")
                {
                    this.lblMaxRecurrences.Visible = true;
                }
            }

            //Populate the timezone combobox (look up timezone translations based on currently set culture)
            this.cboTimeZone.DataBind(this.Settings.TimeZoneId);
            if (!this.Settings.EnableEventTimeZones)
            {
                this.cboTimeZone.Enabled = false;
            }

            if (this._editRecur)
            {
                this.deleteButton.Attributes.Add(
                    "onClick",
                    "javascript:return confirm('" +
                    Localization.GetString("ConfirmEventSeriesDelete", this.LocalResourceFile) + "');");
                this.deleteButton.Text = Localization.GetString("deleteSeriesButton", this.LocalResourceFile);
                this.updateButton.Text = Localization.GetString("updateSeriesButton", this.LocalResourceFile);
                this.copyButton.Attributes.Add(
                    "onClick",
                    "javascript:return confirm('" +
                    Localization.GetString("ConfirmEventCopy", this.LocalResourceFile) + "');");
                this.copyButton.Text = Localization.GetString("copySeriesButton", this.LocalResourceFile);
            }
            else
            {
                this.deleteButton.Attributes.Add(
                    "onClick",
                    "javascript:return confirm('" +
                    Localization.GetString("ConfirmEventDelete", this.LocalResourceFile) + "');");
                this.deleteButton.Text = Localization.GetString("deleteButton", this.LocalResourceFile);
                this.updateButton.Text = Localization.GetString("updateButton", this.LocalResourceFile);
                this.copyButton.Attributes.Add(
                    "onClick",
                    "javascript:return confirm('" +
                    Localization.GetString("ConfirmEventCopy", this.LocalResourceFile) + "');");
                this.copyButton.Text = Localization.GetString("copyButton", this.LocalResourceFile);
            }
            this.lnkSelectedEmail.Attributes.Add(
                "onClick",
                "javascript:return confirm('" +
                Localization.GetString("ConfirmSendToAllSelected", this.LocalResourceFile) + "');");
            this.lnkSelectedDelete.Attributes.Add(
                "onClick",
                "javascript:return confirm('" +
                Localization.GetString("ConfirmDeleteSelected", this.LocalResourceFile) + "');");

            this.txtPayPalAccount.Text = this.Settings.Paypalaccount;

            var iInterval = int.Parse(this.Settings.Timeinterval);
            var currentDate = this.ModuleNow();
            var currentMinutes = currentDate.Minute;
            var remainder = currentMinutes % iInterval;
            if (remainder > 0)
            {
                currentDate = currentDate.AddMinutes(iInterval - remainder);
            }

            this.tpStartTime.TimeView.Interval = new TimeSpan(0, iInterval, 0);
            this.tpStartTime.SelectedDate = currentDate;
            this.tpEndTime.TimeView.Interval = new TimeSpan(0, iInterval, 0);
            this.tpEndTime.SelectedDate = currentDate.AddMinutes(iInterval);


            // Can this event be moderated
            this.lblModerated.Visible = this.Settings.Moderateall;

            // Send Reminder Default
            this.chkReminder.Checked = this.Settings.Sendreminderdefault;

            // Populate description
            this.ftbDesktopText.Text = this.Settings.Templates.NewEventTemplate;

            // Set default validation value
            this.valNoEnrolees.MaximumValue = "9999";

            // Hide enrolment info by default
            this.lblEnrolledUsers.Visible = false;
            this.grdEnrollment.Visible = false;
            this.lnkSelectedDelete.Visible = false;
            this.lnkSelectedEmail.Visible = false;

            this.trAllowAnonEnroll.Visible = this.Settings.AllowAnonEnroll;

            // Populate enrollment email text boxes
            this.txtEventEmailSubject.Text = this.Settings.Templates.txtEditViewEmailSubject;
            this.txtEventEmailBody.Text = this.Settings.Templates.txtEditViewEmailBody;

            if (this._itemID != -1)
            {
                // Edit Item Mode
                var objEvent = default(EventInfo);
                objEvent = this._objCtlEvent.EventsGet(this._itemID, this.ModuleId);

                var blEventSignup = false;
                if (this.Settings.Eventsignup)
                {
                    blEventSignup = objEvent.Signups;
                }
                else
                {
                    blEventSignup = false;
                }
                if (blEventSignup)
                {
                    this.pnlEnroll.Visible = true;
                    this.chkSignups.Visible = true;
                    this.lnkSelectedDelete.Visible = true;
                    this.lnkSelectedEmail.Visible = true;
                    if (this._itemID != 0)
                    {
                        this.tblEventEmail.Attributes.Add("style", "display:block; width:100%");
                    }
                }

                // Check user has edit permissions to this event
                if (this.IsEventEditor(objEvent, false))
                { }
                else
                {
                    this.Response.Redirect(this.GetSocialNavigateUrl(), true);
                }

                // Create an object to consolidate master/single data into - use master object for common data
                var objEventData = default(EventRecurMasterInfo);
                objEventData =
                    this._objCtlEventRecurMaster.EventsRecurMasterGet(objEvent.RecurMasterID, objEvent.ModuleID);

                // Hide recurrences section, disable timezone change if it is a recurring event
                // and we aren't editing the series
                if (objEventData.RRULE != "" && !this._editRecur)
                {
                    this.pnlRecurring.Visible = false;
                    this.cboTimeZone.Enabled = false;
                }

                // If we are editing single item, populate with single event data
                if (!this._editRecur)
                {
                    objEventData.Dtstart = objEvent.EventTimeBegin;
                    objEventData.Duration = Convert.ToString(Convert.ToString(objEvent.Duration) + "M");
                    objEventData.Until = objEvent.EventTimeBegin;
                    objEventData.RRULE = "";
                    objEventData.EventName = objEvent.EventName;
                    objEventData.EventDesc = objEvent.EventDesc;
                    objEventData.Importance = (EventRecurMasterInfo.Priority) objEvent.Importance;
                    objEventData.Notify = objEvent.Notify;
                    objEventData.Approved = objEvent.Approved;
                    objEventData.Signups = objEvent.Signups;
                    objEventData.AllowAnonEnroll = objEvent.AllowAnonEnroll;
                    objEventData.JournalItem = objEvent.JournalItem;
                    objEventData.MaxEnrollment = objEvent.MaxEnrollment;
                    objEventData.EnrollRoleID = objEvent.EnrollRoleID;
                    objEventData.EnrollFee = objEvent.EnrollFee;
                    objEventData.EnrollType = objEvent.EnrollType;
                    objEventData.Enrolled = objEvent.Enrolled;
                    objEventData.PayPalAccount = objEvent.PayPalAccount;
                    objEventData.DetailPage = objEvent.DetailPage;
                    objEventData.DetailNewWin = objEvent.DetailNewWin;
                    objEventData.DetailURL = objEvent.DetailURL;
                    objEventData.ImageURL = objEvent.ImageURL;
                    objEventData.ImageType = objEvent.ImageType;
                    objEventData.ImageWidth = objEvent.ImageWidth;
                    objEventData.ImageHeight = objEvent.ImageHeight;
                    objEventData.ImageDisplay = objEvent.ImageDisplay;
                    objEventData.Location = objEvent.Location;
                    objEventData.Category = objEvent.Category;
                    objEventData.Reminder = objEvent.Reminder;
                    objEventData.SendReminder = objEvent.SendReminder;
                    objEventData.ReminderTime = objEvent.ReminderTime;
                    objEventData.ReminderTimeMeasurement = objEvent.ReminderTimeMeasurement;
                    objEventData.ReminderFrom = objEvent.ReminderFrom;
                    objEventData.CustomField1 = objEvent.CustomField1;
                    objEventData.CustomField2 = objEvent.CustomField2;
                    objEventData.EnrollListView = objEvent.EnrollListView;
                    objEventData.DisplayEndDate = objEvent.DisplayEndDate;
                    objEventData.AllDayEvent = objEvent.AllDayEvent;
                    objEventData.OwnerID = objEvent.OwnerID;
                    objEventData.SocialGroupID = objEvent.SocialGroupId;
                    objEventData.SocialUserID = objEvent.SocialUserId;
                    objEventData.Summary = objEvent.Summary;
                }
                var intDuration = 0;
                intDuration = int.Parse(objEventData.Duration.Substring(0, objEventData.Duration.Length - 1));
                this.txtTitle.Text = objEventData.EventName;
                this.ftbDesktopText.Text = objEventData.EventDesc;

                // Set Dropdown to Original TimeZone w/ModuleID Settings TimeZone
                this.cboTimeZone.DataBind(objEvent.EventTimeZoneId);

                // Set dates/times
                this.dpStartDate.SelectedDate = objEventData.Dtstart.Date;
                this.dpEndDate.SelectedDate = objEventData.Dtstart.AddMinutes(intDuration).Date;
                this.dpRecurEndDate.SelectedDate = objEventData.Until;

                // Adjust Time not in DropDown Selection...
                var starttime = objEventData.Dtstart;
                if (starttime.Minute % iInterval > 0)
                {
                    starttime = objEventData.Dtstart.Date.AddMinutes(
                        Convert.ToDouble(objEventData.Dtstart.Hour * 60 + starttime.Minute -
                                         starttime.Minute % iInterval));
                }
                this.tpStartTime.SelectedDate = starttime;

                var endtime = objEventData.Dtstart.AddMinutes(intDuration);
                if (endtime.Minute % iInterval > 0)
                {
                    endtime = Convert.ToDateTime(objEventData
                                                     .Dtstart.AddMinutes(intDuration).Date
                                                     .AddMinutes(
                                                         Convert.ToDouble(
                                                             Convert.ToInt32(
                                                                 objEventData.Dtstart.AddMinutes(intDuration).Hour *
                                                                 60) + endtime.Minute - endtime.Minute % iInterval)));
                }
                this.tpEndTime.SelectedDate = endtime;

                this.chkSignups.Checked = objEventData.Signups;
                this.chkAllowAnonEnroll.Checked = objEventData.AllowAnonEnroll;
                this.txtMaxEnrollment.Text = objEventData.MaxEnrollment.ToString();
                this.txtEnrolled.Text = objEventData.Enrolled.ToString();
                this.txtPayPalAccount.Text = objEventData.PayPalAccount;
                if (objEventData.EnrollType == "PAID")
                {
                    this.rblFree.Checked = false;
                    this.rblPaid.Checked = true;
                }
                else if (objEventData.EnrollType == "FREE")
                {
                    this.rblFree.Checked = true;
                    this.rblPaid.Checked = false;
                }

                this.txtEnrollFee.Text = string.Format("{0:F2}", objEventData.EnrollFee);
                this.lblTotalCurrency.Text = this.PortalSettings.Currency;

                if (blEventSignup)
                {
                    // Load Enrolled User Grid
                    this.BuildEnrolleeGrid(objEvent);
                }

                if (Information.IsNumeric(objEventData.EnrollRoleID))
                {
                    this.LoadEnrollRoles(objEventData.EnrollRoleID);
                    this.LoadNewEventEmailRoles(objEventData.EnrollRoleID);
                }
                else
                {
                    this.LoadEnrollRoles(-1);
                    this.LoadNewEventEmailRoles(-1);
                }

                this.LoadCategory(objEventData.Category);
                this.LoadLocation(objEventData.Location);
                this.LoadOwnerUsers(objEventData.OwnerID);

                this.cmbImportance.SelectedIndex =
                    Convert.ToInt16(this.GetCmbStatus(objEventData.Importance, "cmbImportance"));
                this.CreatedBy.Text = objEvent.CreatedBy;
                this.CreatedDate.Text = objEventData.CreatedDate.ToShortDateString();
                this.lblCreatedBy.Visible = true;
                this.CreatedBy.Visible = true;
                this.lblOn.Visible = true;
                this.CreatedDate.Visible = true;
                this.pnlAudit.Visible = true;

                var objEventRRULE = default(EventRRULEInfo);
                objEventRRULE = this._objCtlEventRecurMaster.DecomposeRRULE(objEventData.RRULE, objEventData.Dtstart);
                var strRepeatType = this._objCtlEventRecurMaster.RepeatType(objEventRRULE);

                switch (strRepeatType)
                {
                    case "N":
                        this.rblRepeatTypeN.Checked = true;
                        this.cmbP1Period.SelectedIndex = 0;
                        break;
                    //txtP1Every.Text = "0"
                    case "P1":
                        this.rblRepeatTypeP1.Checked = true;
                        this.chkReccuring.Checked = true;
                        this.cmbP1Period.SelectedIndex =
                            Convert.ToInt16(this.GetCmbStatus(objEventRRULE.Freq.Substring(0, 1), "cmbP1Period"));
                        this.txtP1Every.Text = objEventRRULE.Interval.ToString();
                        break;
                    case "W1":
                        this.rblRepeatTypeW1.Checked = true;
                        this.chkReccuring.Checked = true;
                        if (this._culture.DateTimeFormat.FirstDayOfWeek == DayOfWeek.Sunday)
                        {
                            this.chkW1Sun.Checked = objEventRRULE.Su;
                        }
                        else
                        {
                            this.chkW1Sun2.Checked = objEventRRULE.Su;
                        }
                        this.chkW1Mon.Checked = objEventRRULE.Mo;
                        this.chkW1Tue.Checked = objEventRRULE.Tu;
                        this.chkW1Wed.Checked = objEventRRULE.We;
                        this.chkW1Thu.Checked = objEventRRULE.Th;
                        this.chkW1Fri.Checked = objEventRRULE.Fr;
                        this.chkW1Sat.Checked = objEventRRULE.Sa;
                        this.txtW1Every.Text = objEventRRULE.Interval.ToString();
                        break;
                    case "M1":
                        this.rblRepeatTypeM1.Checked = true;
                        this.rblRepeatTypeM.Checked = true;
                        this.chkReccuring.Checked = true;
                        this.txtMEvery.Text = objEventRRULE.Interval.ToString();
                        var intEvery = 0;
                        var intPeriod = 0;
                        if (objEventRRULE.Su)
                        {
                            intPeriod = 0;
                            intEvery = objEventRRULE.SuNo;
                        }
                        if (objEventRRULE.Mo)
                        {
                            intPeriod = 1;
                            intEvery = objEventRRULE.MoNo;
                        }
                        if (objEventRRULE.Tu)
                        {
                            intPeriod = 2;
                            intEvery = objEventRRULE.TuNo;
                        }
                        if (objEventRRULE.We)
                        {
                            intPeriod = 3;
                            intEvery = objEventRRULE.WeNo;
                        }
                        if (objEventRRULE.Th)
                        {
                            intPeriod = 4;
                            intEvery = objEventRRULE.ThNo;
                        }
                        if (objEventRRULE.Fr)
                        {
                            intPeriod = 5;
                            intEvery = objEventRRULE.FrNo;
                        }
                        if (objEventRRULE.Sa)
                        {
                            intPeriod = 6;
                            intEvery = objEventRRULE.SaNo;
                        }
                        this.cmbM1Period.SelectedIndex = intPeriod;
                        if (intEvery == -1)
                        {
                            this.cmbM1Every.SelectedIndex = 4;
                        }
                        else
                        {
                            this.cmbM1Every.SelectedIndex = intEvery - 1;
                        }
                        break;
                    case "M2":
                        this.rblRepeatTypeM2.Checked = true;
                        this.rblRepeatTypeM.Checked = true;
                        this.chkReccuring.Checked = true;
                        this.cmbM2Period.SelectedIndex = objEventRRULE.ByMonthDay - 1;
                        this.txtMEvery.Text = objEventRRULE.Interval.ToString();
                        break;
                    case "Y1":
                        this.rblRepeatTypeY1.Checked = true;
                        this.chkReccuring.Checked = true;
                        var invCulture = CultureInfo.InvariantCulture;
                        var annualdate =
                            DateTime.ParseExact(
                                Strings.Format(objEventData.Dtstart, "yyyy") + "/" + objEventRRULE.ByMonth + "/" +
                                objEventRRULE.ByMonthDay, "yyyy/M/d", invCulture);
                        this.dpY1Period.SelectedDate = annualdate.Date;
                        break;
                }

                this.chkReminder.Checked = objEventData.SendReminder;
                this.txtReminder.Text = objEventData.Reminder;

                if (this.txtReminder.Text.Length == 0)
                {
                    this.txtReminder.Text = this.Settings.Templates.txtMessage;
                }

                this.txtSubject.Text = objEventData.Notify;
                if (this.txtSubject.Text.Length == 0)
                {
                    this.txtSubject.Text = this.Settings.Templates.txtSubject;
                }

                this.txtReminderFrom.Text = objEventData.ReminderFrom;
                this.txtEventEmailFrom.Text = objEventData.ReminderFrom;
                if (objEventData.ReminderTime < 0)
                {
                    this.txtReminderTime.Text = Convert.ToString(15.ToString());
                }
                else
                {
                    this.txtReminderTime.Text = objEventData.ReminderTime.ToString();
                }

                if (!ReferenceEquals(
                        this.ddlReminderTimeMeasurement.Items.FindByValue(objEventData.ReminderTimeMeasurement), null))
                {
                    this.ddlReminderTimeMeasurement.ClearSelection();
                    this.ddlReminderTimeMeasurement.Items.FindByValue(objEventData.ReminderTimeMeasurement).Selected =
                        true;
                }

                // Set DetailURL
                this.chkDetailPage.Checked = objEventData.DetailPage;
                this.URLDetail.Url = objEventData.DetailURL;


                // Set Image Control
                this.chkDisplayImage.Checked = objEventData.ImageDisplay;
                this.ctlURL.Url = objEventData.ImageURL;
                if (objEventData.ImageURL.StartsWith("FileID="))
                {
                    var fileId = int.Parse(objEventData.ImageURL.Substring(7));
                    var objFileInfo = FileManager.Instance.GetFile(fileId);
                    if (!ReferenceEquals(objFileInfo, null))
                    {
                        this.ctlURL.Url = objFileInfo.Folder + objFileInfo.FileName;
                    }
                    else
                    {
                        this.chkDisplayImage.Checked = false;
                    }
                }

                if ((objEventData.ImageWidth != 0) & (objEventData.ImageWidth != -1))
                {
                    this.txtWidth.Text = objEventData.ImageWidth.ToString();
                }

                if ((objEventData.ImageHeight != 0) & (objEventData.ImageHeight != -1))
                {
                    this.txtHeight.Text = objEventData.ImageHeight.ToString();
                }
                this.txtCustomField1.Text = objEventData.CustomField1;
                this.txtCustomField2.Text = objEventData.CustomField2;
                this.chkEnrollListView.Checked = objEventData.EnrollListView;
                this.chkDisplayEndDate.Checked = objEventData.DisplayEndDate;
                this.chkAllDayEvent.Checked = objEventData.AllDayEvent;
                this.ftbSummary.Text = objEventData.Summary;

                if (blEventSignup)
                {
                    this.LoadRegUsers();
                }
            }
            else
            {
                this.dpStartDate.SelectedDate = this.SelectedDate.Date;
                this.dpEndDate.SelectedDate = this.SelectedDate.Date;
                this.txtEnrollFee.Text = string.Format("{0:F2}", 0.0);
                this.lblTotalCurrency.Text = this.PortalSettings.Currency;
                this.dpRecurEndDate.SelectedDate = this.SelectedDate.AddDays(1);
                this.dpY1Period.SelectedDate = this.SelectedDate.Date;
                this.chkEnrollListView.Checked = this.Settings.Eventdefaultenrollview;
                this.chkDisplayEndDate.Checked = true;
                this.chkAllDayEvent.Checked = false;

                // Do not default recurrance end date
                // Force user to key/select
                this.LoadEnrollRoles(-1);
                this.LoadNewEventEmailRoles(-1);
                this.LoadCategory();
                this.LoadLocation(-1);
                this.LoadOwnerUsers(this.UserId);
                this.pnlAudit.Visible = false;
                this.deleteButton.Visible = false;
            }

            if (this._itemID == -1 || this._editRecur)
            {
                this.divAddUser.Visible = false;
                this.divNoEnrolees.Visible = false;
            }

            var errorminutes = Localization.GetString("invalidReminderMinutes", this.LocalResourceFile);
            var errorhours = Localization.GetString("invalidReminderHours", this.LocalResourceFile);
            var errordays = Localization.GetString("invalidReminderDays", this.LocalResourceFile);
            this.ddlReminderTimeMeasurement.Attributes.Add(
                "onchange",
                "valRemTime('" + this.valReminderTime.ClientID + "','" + this.valReminderTime2.ClientID + "','" +
                this.valReminderTime.ValidationGroup + "','" + this.ddlReminderTimeMeasurement.ClientID + "','" +
                errorminutes + "','" + errorhours + "','" + errordays + "');");
            switch (this.ddlReminderTimeMeasurement.SelectedValue)
            {
                case "m":
                    this.valReminderTime.ErrorMessage = errorminutes;
                    this.valReminderTime.MinimumValue = "15";
                    this.valReminderTime.MaximumValue = "60";
                    break;
                case "h":
                    this.valReminderTime.ErrorMessage = errorhours;
                    this.valReminderTime.MinimumValue = "1";
                    this.valReminderTime.MaximumValue = "24";
                    break;
                case "d":
                    this.valReminderTime.ErrorMessage = errordays;
                    this.valReminderTime.MinimumValue = "1";
                    this.valReminderTime.MaximumValue = "30";
                    break;
            }
            this.valReminderTime2.ErrorMessage = this.valReminderTime.ErrorMessage;

            if (this.txtPayPalAccount.Text.Length == 0)
            {
                this.txtPayPalAccount.Text = this.Settings.Paypalaccount;
            }

            this.trCustomField1.Visible = this.Settings.EventsCustomField1;
            this.trCustomField2.Visible = this.Settings.EventsCustomField2;

            this.trTimeZone.Visible = this.Settings.Tzdisplay;

            this.chkDetailPage.Attributes.Add(
                "onclick",
                "javascript:showTbl('" + this.chkDetailPage.ClientID + "','" + this.tblDetailPageDetail.ClientID +
                "');");
            this.chkReminder.Attributes.Add(
                "onclick",
                "javascript:showTbl('" + this.chkReminder.ClientID + "','" + this.tblReminderDetail.ClientID +
                "');");
            this.chkDisplayImage.Attributes.Add(
                "onclick",
                "javascript:showTbl('" + this.chkDisplayImage.ClientID + "','" + this.tblImageURL.ClientID +
                "');");
            this.chkReccuring.Attributes.Add(
                "onclick",
                "javascript:if (this.checked == true) dnn.dom.getById('" + this.rblRepeatTypeP1.ClientID +
                "').checked = true; else dnn.dom.getById('" + this.rblRepeatTypeN.ClientID +
                "').checked = true;showhideTbls('" + RecurTableDisplayType + "','" + this.chkReccuring.ClientID +
                "','" + this.tblRecurringDetails.ClientID + "','" + this.rblRepeatTypeP1.ClientID + "','" +
                this.tblDetailP1.ClientID + "','" + this.rblRepeatTypeW1.ClientID + "','" +
                this.tblDetailW1.ClientID + "','" + this.rblRepeatTypeM.ClientID + "','" +
                this.tblDetailM1.ClientID + "','" + this.rblRepeatTypeY1.ClientID + "','" +
                this.tblDetailY1.ClientID + "');");
            if (this.Settings.Eventsignup)
            {
                this.chkEventEmailChk.Attributes.Add(
                    "onclick",
                    "javascript:showhideChk2('" + this.chkEventEmailChk.ClientID + "','" +
                    this.tblEventEmailRoleDetail.ClientID + "','" + this.chkSignups.ClientID + "','" +
                    this.tblEventEmail.ClientID + "');");
            }
            else
            {
                this.chkEventEmailChk.Attributes.Add(
                    "onclick",
                    "javascript:showhideChk2('" + this.chkEventEmailChk.ClientID + "','" +
                    this.tblEventEmailRoleDetail.ClientID + "','" + this.chkEventEmailChk.ClientID + "','" +
                    this.tblEventEmail.ClientID + "');");
            }
            this.rblRepeatTypeP1.Attributes.Add(
                "onclick",
                "javascript:showhideTbls('" + RecurTableDisplayType + "','" + this.chkReccuring.ClientID + "','" +
                this.tblRecurringDetails.ClientID + "','" + this.rblRepeatTypeP1.ClientID + "','" +
                this.tblDetailP1.ClientID + "','" + this.rblRepeatTypeW1.ClientID + "','" +
                this.tblDetailW1.ClientID + "','" + this.rblRepeatTypeM.ClientID + "','" +
                this.tblDetailM1.ClientID + "','" + this.rblRepeatTypeY1.ClientID + "','" +
                this.tblDetailY1.ClientID + "');");
            this.rblRepeatTypeW1.Attributes.Add(
                "onclick",
                "javascript:showhideTbls('" + RecurTableDisplayType + "','" + this.chkReccuring.ClientID + "','" +
                this.tblRecurringDetails.ClientID + "','" + this.rblRepeatTypeP1.ClientID + "','" +
                this.tblDetailP1.ClientID + "','" + this.rblRepeatTypeW1.ClientID + "','" +
                this.tblDetailW1.ClientID + "','" + this.rblRepeatTypeM.ClientID + "','" +
                this.tblDetailM1.ClientID + "','" + this.rblRepeatTypeY1.ClientID + "','" +
                this.tblDetailY1.ClientID + "');");
            this.rblRepeatTypeM.Attributes.Add(
                "onclick",
                "javascript:showhideTbls('" + RecurTableDisplayType + "','" + this.chkReccuring.ClientID + "','" +
                this.tblRecurringDetails.ClientID + "','" + this.rblRepeatTypeP1.ClientID + "','" +
                this.tblDetailP1.ClientID + "','" + this.rblRepeatTypeW1.ClientID + "','" +
                this.tblDetailW1.ClientID + "','" + this.rblRepeatTypeM.ClientID + "','" +
                this.tblDetailM1.ClientID + "','" + this.rblRepeatTypeY1.ClientID + "','" +
                this.tblDetailY1.ClientID + "');if (this.checked == true) dnn.dom.getById('" +
                this.rblRepeatTypeM1.ClientID + "').checked = true;");
            this.rblRepeatTypeY1.Attributes.Add(
                "onclick",
                "javascript:showhideTbls('" + RecurTableDisplayType + "','" + this.chkReccuring.ClientID + "','" +
                this.tblRecurringDetails.ClientID + "','" + this.rblRepeatTypeP1.ClientID + "','" +
                this.tblDetailP1.ClientID + "','" + this.rblRepeatTypeW1.ClientID + "','" +
                this.tblDetailW1.ClientID + "','" + this.rblRepeatTypeM.ClientID + "','" +
                this.tblDetailM1.ClientID + "','" + this.rblRepeatTypeY1.ClientID + "','" +
                this.tblDetailY1.ClientID + "');");
            this.btnCopyStartdate.Attributes.Add(
                "onclick",
                string.Format("javascript:CopyStartDateToEnddate('{0}','{1}','{2}','{3}','{4}');",
                              this.dpStartDate.ClientID, this.dpEndDate.ClientID, this.tpStartTime.ClientID,
                              this.tpEndTime.ClientID, this.chkAllDayEvent.ClientID));
            this.chkAllDayEvent.Attributes.Add(
                "onclick",
                "javascript:showTimes('" + this.chkAllDayEvent.ClientID + "','" + this.divStartTime.ClientID +
                "','" + this.divEndTime.ClientID + "');");
        }

        public long GetCmbStatus(object value, string cmbDropDown)
        {
            var iIndex = 0;
            var oDropDown = default(DropDownList);

            oDropDown = (DropDownList) this.FindControl(cmbDropDown);
            for (iIndex = 0; iIndex <= oDropDown.Items.Count - 1; iIndex++)
            {
                if (oDropDown.Items[iIndex].Value == Convert.ToString(value))
                {
                    return iIndex;
                }
            }
            return iIndex;
        }

        public void LoadEnrollRoles(int roleID)
        {
            var objRoles = new RoleController();
            this.ddEnrollRoles.DataSource = objRoles.GetPortalRoles(this.PortalId);
            this.ddEnrollRoles.DataTextField = "RoleName";
            this.ddEnrollRoles.DataValueField = "RoleID";
            this.ddEnrollRoles.DataBind();
            //"<None Specified>"
            this.ddEnrollRoles.Items.Insert(
                0, new ListItem(Localization.GetString("None", this.LocalResourceFile), "-1"));
            if (roleID == 0)
            {
                this.ddEnrollRoles.Items.FindByValue("0").Selected = true;
            }
            else if (roleID > 0)
            {
                if (ReferenceEquals(this.ddEnrollRoles.Items.FindByValue(Convert.ToString(roleID)), null))
                {
                    this.ddEnrollRoles.Items.Insert(
                        0,
                        new ListItem(Localization.GetString("EnrolleeRoleDeleted", this.LocalResourceFile),
                                     roleID.ToString()));
                }
                this.ddEnrollRoles.Items.FindByValue(Convert.ToString(roleID)).Selected = true;
            }
        }

        public void LoadNewEventEmailRoles(int roleID)
        {
            var objRoles = new RoleController();
            this.ddEventEmailRoles.DataSource = objRoles.GetPortalRoles(this.PortalId);
            this.ddEventEmailRoles.DataTextField = "RoleName";
            this.ddEventEmailRoles.DataValueField = "RoleID";
            this.ddEventEmailRoles.DataBind();
            if (roleID < 0 || ReferenceEquals(this.ddEventEmailRoles.Items.FindByValue(Convert.ToString(roleID)), null))
            {
                try
                {
                    this.ddEventEmailRoles.Items.FindByValue(this.PortalSettings.RegisteredRoleId.ToString()).Selected =
                        true;
                }
                catch
                { }
            }
            else
            {
                this.ddEventEmailRoles.Items.FindByValue(Convert.ToString(roleID)).Selected = true;
            }
        }

        public void LoadCategory(int category = default(int))
        {
            var objCntCategories = new EventCategoryController();
            var tmpCategories = objCntCategories.EventsCategoryList(this.PortalId);
            var objCategories = new ArrayList();
            if ((this.Settings.Enablecategories == EventModuleSettings.DisplayCategories.DoNotDisplay) &
                (this.Settings.ModuleCategoriesSelected == EventModuleSettings.CategoriesSelected.Some)
                || this.Settings.Restrictcategories)
            {
                foreach (EventCategoryInfo objCategory in tmpCategories)
                {
                    foreach (int moduleCategory in this.Settings.ModuleCategoryIDs)
                    {
                        if (moduleCategory == objCategory.Category)
                        {
                            objCategories.Add(objCategory);
                        }
                    }
                }
            }
            else
            {
                objCategories = tmpCategories;
            }
            this.cmbCategory.DataSource = objCategories;
            this.cmbCategory.DataTextField = "CategoryName";
            this.cmbCategory.DataValueField = "Category";
            this.cmbCategory.DataBind();

            // Do we need to add None
            if (!(this.Settings.Enablecategories == EventModuleSettings.DisplayCategories.DoNotDisplay) |
                (this.Settings.ModuleCategoriesSelected == EventModuleSettings.CategoriesSelected.All)
                && this.Settings.Restrictcategories == false)
            {
                this.cmbCategory.Items.Insert(
                    0, new ListItem(Localization.GetString("None", this.LocalResourceFile), "-1"));
            }

            // Select the appropriate row
            if (this.Settings.ModuleCategoriesSelected == EventModuleSettings.CategoriesSelected.All)
            {
                this.cmbCategory.ClearSelection();
                this.cmbCategory.Items[0].Selected = true;
            }

            if (category > 0 && !ReferenceEquals(category, null))
            {
                this.cmbCategory.ClearSelection();
                this.cmbCategory.Items.FindByValue(Convert.ToString(category)).Selected = true;
            }
            else if (!(this.Settings.Enablecategories == EventModuleSettings.DisplayCategories.DoNotDisplay) &
                     (this.Settings.ModuleCategoriesSelected == EventModuleSettings.CategoriesSelected.Some))
            {
                this.cmbCategory.ClearSelection();
                this.cmbCategory.Items.FindByValue(Convert.ToString(this.Settings.ModuleCategoryIDs[0])).Selected =
                    true;
            }
        }

        public void LoadLocation(int location)
        {
            var objCntLocation = new EventLocationController();
            this.cmbLocation.DataSource = objCntLocation.EventsLocationList(this.PortalId);
            this.cmbLocation.DataTextField = "LocationName";
            this.cmbLocation.DataValueField = "Location";
            this.cmbLocation.DataBind();
            //"<None Specified>"
            this.cmbLocation.Items.Insert(
                0, new ListItem(Localization.GetString("None", this.LocalResourceFile), "-1"));
            if (location > 0 && !ReferenceEquals(location, null))
            {
                this.cmbLocation.Items.FindByValue(Convert.ToString(location)).Selected = true;
            }
        }

        private void LoadOwnerUsers(int ownerID)
        {
            var objCollModulePermission = default(ModulePermissionCollection);
            objCollModulePermission = ModulePermissionController.GetModulePermissions(this.ModuleId, this.TabId);
            var objModulePermission = default(ModulePermissionInfo);

            // To cope with host users or someone who is no longer an editor!!
            var objEventModuleEditor = new EventUser();
            objEventModuleEditor.UserID = ownerID;
            this.LoadSingleUser(objEventModuleEditor, this._lstOwnerUsers);

            if (this.IsModerator() && this.Settings.Ownerchangeallowed ||
                PortalSecurity.IsInRole(this.PortalSettings.AdministratorRoleName))
            {
                foreach (ModulePermissionInfo tempLoopVar_objModulePermission in objCollModulePermission)
                {
                    objModulePermission = tempLoopVar_objModulePermission;
                    if (objModulePermission.PermissionKey == "EVENTSEDT")
                    {
                        if (objModulePermission.UserID < 0)
                        {
                            var objCtlRole = new RoleController();
                            var lstRoleUsers =
                                objCtlRole.GetUsersByRoleName(this.PortalId, objModulePermission.RoleName);
                            foreach (UserInfo objUser in lstRoleUsers)
                            {
                                objEventModuleEditor = new EventUser();
                                objEventModuleEditor.UserID = objUser.UserID;
                                objEventModuleEditor.DisplayName = objUser.DisplayName;
                                this.LoadSingleUser(objEventModuleEditor, this._lstOwnerUsers);
                            }
                        }
                        else
                        {
                            objEventModuleEditor = new EventUser();
                            objEventModuleEditor.UserID = objModulePermission.UserID;
                            objEventModuleEditor.DisplayName = objModulePermission.DisplayName;
                            this.LoadSingleUser(objEventModuleEditor, this._lstOwnerUsers);
                        }
                    }
                }
            }
            this._lstOwnerUsers.Sort(new UserListSort());

            this.cmbOwner.DataSource = this._lstOwnerUsers;
            this.cmbOwner.DataTextField = "DisplayName";
            this.cmbOwner.DataValueField = "UserID";
            this.cmbOwner.DataBind();
            this.cmbOwner.Items.FindByValue(Convert.ToString(ownerID)).Selected = true;
        }

        private void LoadRegUsers()
        {
            this.grdAddUser.RefreshGrid();
        }

        private void LoadSingleUser(EventUser objEventUser, ArrayList lstUsers)
        {
            var blAdd = true;
            var objEventUser2 = default(EventUser);
            foreach (EventUser tempLoopVar_objEventUser2 in lstUsers)
            {
                objEventUser2 = tempLoopVar_objEventUser2;
                if (objEventUser.UserID == objEventUser2.UserID)
                {
                    blAdd = false;
                }
            }
            if (blAdd)
            {
                if (objEventUser.DisplayName == null)
                {
                    var objCtlUser = new UserController();
                    var objUser = objCtlUser.GetUser(this.PortalId, objEventUser.UserID);
                    if (!ReferenceEquals(objUser, null))
                    {
                        objEventUser.DisplayName = objUser.DisplayName;
                    }
                    else
                    {
                        objEventUser.DisplayName = Localization.GetString("OwnerDeleted", this.LocalResourceFile);
                    }
                }
                lstUsers.Add(objEventUser);
            }
        }

        private void Email(bool selected)
        {
            var item = default(DataGridItem);
            var objEnroll = default(EventSignupsInfo);
            var objEvent = default(EventInfo);
            // Get Current Event, if <> 0
            if (this._itemID > 0)
            {
                objEvent = this._objCtlEvent.EventsGet(this._itemID, this.ModuleId);
            }
            else
            {
                return;
            }

            var objEventEmailInfo = new EventEmailInfo();
            var objEventEmail = new EventEmails(this.PortalId, this.ModuleId, this.LocalResourceFile,
                                                ((PageBase) this.Page).PageCulture.Name);
            objEventEmailInfo.TxtEmailSubject = this.txtEventEmailSubject.Text;
            objEventEmailInfo.TxtEmailBody = this.txtEventEmailBody.Text;
            objEventEmailInfo.TxtEmailFrom = this.txtEventEmailFrom.Text;
            foreach (DataGridItem tempLoopVar_item in this.grdEnrollment.Items)
            {
                item = tempLoopVar_item;
                if (((CheckBox) item.FindControl("chkSelect")).Checked || selected == false)
                {
                    objEnroll = this._objCtlEventSignups.EventsSignupsGet(
                        Convert.ToInt32(this.grdEnrollment.DataKeys[item.ItemIndex]), this.ModuleId,
                        false);
                    objEventEmailInfo.UserIDs.Clear();
                    objEventEmailInfo.UserEmails.Clear();
                    objEventEmailInfo.UserLocales.Clear();
                    objEventEmailInfo.UserTimeZoneIds.Clear();
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
                }
            }
        }

        private void UpdateProcessing(int processItem)
        {
            if (!this.Page.IsValid)
            {
                return;
            }

            var objSecurity = new PortalSecurity();
            var tStartTime = default(DateTime);
            var tEndTime = default(DateTime);
            var tRecurEndDate = default(DateTime);
            var objEventInfoHelper = new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);

            // Make EndDate = StartDate if no recurring dates
            if (this.rblRepeatTypeN.Checked)
            {
                this.dpRecurEndDate.SelectedDate = this
                    .ConvertDateStringstoDatetime(this.dpStartDate.SelectedDate.ToString(), "00:00").Date;
            }

            this.valRequiredRecurEndDate.Validate();

            // Make sure date formatted correctly
            if (this.chkAllDayEvent.Checked)
            {
                tStartTime = this.ConvertDateStringstoDatetime(this.dpStartDate.SelectedDate.ToString(), "00:00");
                tEndTime = Convert.ToDateTime(
                    this.ConvertDateStringstoDatetime(this.dpEndDate.SelectedDate.ToString(), "00:00")
                        .AddMinutes(1439));
            }
            else
            {
                tStartTime =
                    this.ConvertDateStringstoDatetime(this.dpStartDate.SelectedDate.ToString(),
                                                      Convert.ToString(
                                                          Convert.ToDateTime(this.tpStartTime.SelectedDate)
                                                                 .ToString("HH:mm", CultureInfo.InvariantCulture)));
                tEndTime = this.ConvertDateStringstoDatetime(this.dpEndDate.SelectedDate.ToString(),
                                                             Convert.ToString(
                                                                 Convert.ToDateTime(this.tpEndTime.SelectedDate)
                                                                        .ToString(
                                                                            "HH:mm", CultureInfo.InvariantCulture)));
            }

            if (tEndTime < tStartTime && !this.chkAllDayEvent.Checked)
            {
                this.valValidEndTime.ErrorMessage = Localization.GetString("valValidEndTime", this.LocalResourceFile);
                this.valValidEndTime.IsValid = false;
                this.valValidEndTime.Visible = true;
                return;
            }

            tRecurEndDate = Convert.ToDateTime(this.dpRecurEndDate.SelectedDate);

            if (this.rblRepeatTypeP1.Checked)
            {
                this.valP1Every.Validate();
                this.valP1Every2.Validate();
                if (this.valP1Every.IsValid == false || this.valP1Every2.IsValid == false)
                {
                    return;
                }
            }

            if (this.rblRepeatTypeW1.Checked)
            {
                this.valW1Day.Validate();
                this.valW1Day2.Validate();
                if (this.chkW1Sun.Checked == false && this.chkW1Sun2.Checked == false &&
                    this.chkW1Mon.Checked == false && this.chkW1Tue.Checked == false &&
                    this.chkW1Wed.Checked == false && this.chkW1Thu.Checked == false &&
                    this.chkW1Fri.Checked == false && this.chkW1Sat.Checked == false)
                {
                    this.valW1Day3.ErrorMessage = Localization.GetString("valW1Day3", this.LocalResourceFile);
                    this.valW1Day3.Text = Localization.GetString("valW1Day3", this.LocalResourceFile);
                    this.valW1Day3.IsValid = false;
                    this.valW1Day3.Visible = true;
                    return;
                }
                if (this.valW1Day.IsValid == false || this.valW1Day2.IsValid == false)
                {
                    return;
                }
            }

            if (this.rblRepeatTypeM.Checked && this.rblRepeatTypeM2.Checked)
            {
                this.valM2Every.Validate();
                this.valM2Every2.Validate();
                if (this.valM2Every.IsValid == false || this.valM2Every2.IsValid == false)
                {
                    return;
                }
            }
            // If Annual Recurrence, Check date
            if (this.rblRepeatTypeY1.Checked)
            {
                this.valRequiredYearEventDate.Validate();
                this.valValidYearEventDate.Validate();
                if (this.valRequiredYearEventDate.IsValid == false || this.valValidYearEventDate.IsValid == false)
                {
                    return;
                }
            }

            if (this.Settings.Expireevents != ""
                && !this._editRecur)
            {
                if (tStartTime < DateTime.Now.AddDays(-Convert.ToInt32(this.Settings.Expireevents)))
                {
                    this.valValidStartDate2.IsValid = false;
                    this.valValidStartDate2.Visible = true;
                    this.valValidStartDate2.Text =
                        string.Format(Localization.GetString("valValidStartDate2", this.LocalResourceFile),
                                      Convert.ToInt32(this.Settings.Expireevents));
                    this.valValidStartDate2.ErrorMessage =
                        string.Format(Localization.GetString("valValidStartDate2", this.LocalResourceFile),
                                      Convert.ToInt32(this.Settings.Expireevents));
                    return;
                }
            }

            double duration = 0;
            duration = tEndTime.Subtract(tStartTime).TotalMinutes;

            if (this.rblPaid.Checked)
            {
                if (Information.IsNumeric(this.txtEnrollFee.Text))
                {
                    // ReSharper disable CompareOfFloatsByEqualityOperator
                    if (Convert.ToDouble(this.txtEnrollFee.Text) == 0.0)
                    {
                        // ReSharper restore CompareOfFloatsByEqualityOperator
                        this.valBadFee.IsValid = false;
                        this.valBadFee.Visible = true;
                        return;
                    }
                }
                else
                {
                    this.valBadFee.IsValid = false;
                    this.valBadFee.Visible = true;
                    return;
                }
                if (this.txtPayPalAccount.Text.Trim() == "")
                {
                    this.valPayPalAccount.IsValid = false;
                    this.valPayPalAccount.Visible = true;
                    return;
                }
            }

            //Check valid Reminder Time
            if (this.chkReminder.Checked)
            {
                var remtime = Convert.ToInt32(this.txtReminderTime.Text);
                switch (this.ddlReminderTimeMeasurement.SelectedValue)
                {
                    case "m":
                        if ((remtime < 15) | (remtime > 60))
                        {
                            this.valReminderTime2.IsValid = false;
                            this.valReminderTime2.Visible = true;
                            return;
                        }
                        break;
                    case "h":
                        if ((remtime < 1) | (remtime > 24))
                        {
                            this.valReminderTime2.IsValid = false;
                            this.valReminderTime2.Visible = true;
                            return;
                        }
                        break;
                    case "d":
                        if ((remtime < 1) | (remtime > 30))
                        {
                            this.valReminderTime2.IsValid = false;
                            this.valReminderTime2.Visible = true;
                            return;
                        }
                        break;
                }
            }

            if (this.chkSignups.Checked)
            {
                this.valMaxEnrollment.Validate();
                if (!this.valMaxEnrollment.IsValid)
                {
                    return;
                }
            }

            this.valW1Day.Visible = false;
            this.valW1Day2.Visible = false;
            this.valW1Day3.Visible = false;
            this.valConflict.Visible = false;
            this.valLocationConflict.Visible = false;
            this.valPayPalAccount.Visible = false;
            this.valBadFee.Visible = false;
            this.valValidRecurStartDate.Visible = false;
            this.valNoEnrolees.Visible = false;
            this.valMaxEnrollment.Visible = false;

            // Everythings Cool, Update Database
            var objEvent = default(EventInfo);
            var objEventRecurMaster = new EventRecurMasterInfo();
            var objEventRMSave = new EventRecurMasterInfo();

            // Get Current Event, if <> 0
            if (processItem > 0)
            {
                objEvent = this._objCtlEvent.EventsGet(processItem, this.ModuleId);
                objEventRecurMaster =
                    this._objCtlEventRecurMaster.EventsRecurMasterGet(objEvent.RecurMasterID, objEvent.ModuleID);
                objEventRMSave =
                    this._objCtlEventRecurMaster.EventsRecurMasterGet(objEvent.RecurMasterID, objEvent.ModuleID);
                if (this._editRecur)
                {
                    this._lstEvents =
                        this._objCtlEvent.EventsGetRecurrences(objEventRecurMaster.RecurMasterID,
                                                               objEventRecurMaster.ModuleID);
                }
                else
                {
                    this._lstEvents.Add(objEvent);
                }
            }
            var intDuration = 0;
            objEventRecurMaster.Dtstart = tStartTime;
            objEventRecurMaster.Duration = Convert.ToString(Convert.ToString(duration) + "M");
            intDuration = Convert.ToInt32(duration);
            objEventRecurMaster.Until = tRecurEndDate;
            objEventRecurMaster.UpdatedByID = this.UserId;
            objEventRecurMaster.OwnerID = int.Parse(this.cmbOwner.SelectedValue);
            if (processItem < 0)
            {
                objEventRecurMaster.RecurMasterID = -1;
                objEventRecurMaster.CreatedByID = this.UserId;
                objEventRecurMaster.ModuleID = this.ModuleId;
                objEventRecurMaster.PortalID = this.PortalId;
                objEventRecurMaster.CultureName = Thread.CurrentThread.CurrentCulture.Name;
                objEventRecurMaster.JournalItem = false;
            }
            // Filter text for non-admins and moderators
            if (PortalSecurity.IsInRole(this.PortalSettings.AdministratorRoleName))
            {
                objEventRecurMaster.EventName = this.txtTitle.Text;
                objEventRecurMaster.EventDesc = Convert.ToString(this.ftbDesktopText.Text);
                objEventRecurMaster.CustomField1 = this.txtCustomField1.Text;
                objEventRecurMaster.CustomField2 = this.txtCustomField2.Text;
                objEventRecurMaster.Notify = this.txtSubject.Text;
                objEventRecurMaster.Reminder = this.txtReminder.Text;
                objEventRecurMaster.Summary = Convert.ToString(this.ftbSummary.Text);
            }
            else if (this.IsModerator())
            {
                objEventRecurMaster.EventName =
                    objSecurity.InputFilter(this.txtTitle.Text, PortalSecurity.FilterFlag.NoScripting);
                objEventRecurMaster.EventDesc = Convert.ToString(this.ftbDesktopText.Text);
                objEventRecurMaster.CustomField1 =
                    objSecurity.InputFilter(this.txtCustomField1.Text, PortalSecurity.FilterFlag.NoScripting);
                objEventRecurMaster.CustomField2 =
                    objSecurity.InputFilter(this.txtCustomField2.Text, PortalSecurity.FilterFlag.NoScripting);
                objEventRecurMaster.Notify =
                    objSecurity.InputFilter(this.txtSubject.Text, PortalSecurity.FilterFlag.NoScripting);
                objEventRecurMaster.Reminder =
                    objSecurity.InputFilter(this.txtReminder.Text, PortalSecurity.FilterFlag.NoScripting);
                objEventRecurMaster.Summary = Convert.ToString(this.ftbSummary.Text);
            }
            else
            {
                objEventRecurMaster.EventName =
                    objSecurity.InputFilter(this.txtTitle.Text, PortalSecurity.FilterFlag.NoScripting);
                objEventRecurMaster.EventDesc =
                    objSecurity.InputFilter(Convert.ToString(this.ftbDesktopText.Text),
                                            PortalSecurity.FilterFlag.NoScripting);
                objEventRecurMaster.CustomField1 =
                    objSecurity.InputFilter(this.txtCustomField1.Text, PortalSecurity.FilterFlag.NoScripting);
                objEventRecurMaster.CustomField2 =
                    objSecurity.InputFilter(this.txtCustomField2.Text, PortalSecurity.FilterFlag.NoScripting);
                objEventRecurMaster.Notify =
                    objSecurity.InputFilter(this.txtSubject.Text, PortalSecurity.FilterFlag.NoScripting);
                objEventRecurMaster.Reminder =
                    objSecurity.InputFilter(this.txtReminder.Text, PortalSecurity.FilterFlag.NoScripting);
                objEventRecurMaster.Summary =
                    objSecurity.InputFilter(Convert.ToString(this.ftbSummary.Text),
                                            PortalSecurity.FilterFlag.NoScripting);
            }

            // If New Event
            if (processItem < 0)
            {
                // If Moderator turned on, set approve=false
                if (this.Settings.Moderateall)
                {
                    objEventRecurMaster.Approved = false;
                }
                else
                {
                    objEventRecurMaster.Approved = true;
                }
            }

            // Reset Approved, if Moderate All option is on
            if (this.Settings.Moderateall &&
                objEventRecurMaster.Approved)
            {
                objEventRecurMaster.Approved = false;
            }

            // If Admin or Moderator, automatically approve event
            if (PortalSecurity.IsInRole(this.PortalSettings.AdministratorRoleName) || this.IsModerator())
            {
                objEventRecurMaster.Approved = true;
            }

            objEventRecurMaster.Importance =
                (EventRecurMasterInfo.Priority) int.Parse(this.cmbImportance.SelectedItem.Value);

            objEventRecurMaster.Signups = this.chkSignups.Checked;
            objEventRecurMaster.AllowAnonEnroll = this.chkAllowAnonEnroll.Checked;
            if (this.rblFree.Checked)
            {
                objEventRecurMaster.EnrollType = "FREE";
            }
            else if (this.rblPaid.Checked)
            {
                objEventRecurMaster.EnrollType = "PAID";
            }

            objEventRecurMaster.PayPalAccount = this.txtPayPalAccount.Text;
            objEventRecurMaster.EnrollFee = decimal.Parse(this.txtEnrollFee.Text);
            objEventRecurMaster.MaxEnrollment = Convert.ToInt32(this.txtMaxEnrollment.Text);

            if (int.Parse(this.ddEnrollRoles.SelectedValue) != -1)
            {
                objEventRecurMaster.EnrollRoleID = int.Parse(this.ddEnrollRoles.SelectedItem.Value);
            }
            else
            {
                objEventRecurMaster.EnrollRoleID = -1;
            }

            // Update Detail Page setting in the database
            if (this.chkDetailPage.Checked && this.URLDetail.Url != "")
            {
                objEventRecurMaster.DetailPage = true;
                objEventRecurMaster.DetailURL = Convert.ToString(this.URLDetail.Url);
                objEventRecurMaster.DetailNewWin = Convert.ToBoolean(this.URLDetail.NewWindow);
            }
            else
            {
                objEventRecurMaster.DetailPage = false;
            }

            // Update Image settings in the database
            if (this.chkDisplayImage.Checked)
            {
                objEventRecurMaster.ImageDisplay = true;
                if (this.ctlURL.UrlType == "F")
                {
                    if (this.ctlURL.Url.StartsWith("FileID="))
                    {
                        var fileId = int.Parse(Convert.ToString(this.ctlURL.Url.Substring(7)));
                        var objFileInfo = FileManager.Instance.GetFile(fileId);
                        if (this.txtWidth.Text == "" || this.txtWidth.Text == 0.ToString())
                        {
                            this.txtWidth.Text = Convert.ToString(objFileInfo.Width.ToString());
                        }
                        if (this.txtHeight.Text == "" || this.txtHeight.Text == 0.ToString())
                        {
                            this.txtHeight.Text = Convert.ToString(objFileInfo.Height.ToString());
                        }
                    }
                }
                objEventRecurMaster.ImageURL = Convert.ToString(this.ctlURL.Url);
                objEventRecurMaster.ImageType = Convert.ToString(this.ctlURL.UrlType);
            }
            else
            {
                objEventRecurMaster.ImageDisplay = false;
            }

            if (this.txtWidth.Text == "")
            {
                objEventRecurMaster.ImageWidth = 0;
            }
            else
            {
                objEventRecurMaster.ImageWidth = int.Parse(this.txtWidth.Text);
            }
            if (this.txtHeight.Text == "")
            {
                objEventRecurMaster.ImageHeight = 0;
            }
            else
            {
                objEventRecurMaster.ImageHeight = int.Parse(this.txtHeight.Text);
            }

            objEventRecurMaster.Category = int.Parse(this.cmbCategory.SelectedValue);
            objEventRecurMaster.Location = int.Parse(this.cmbLocation.SelectedValue);

            objEventRecurMaster.SendReminder = this.chkReminder.Checked;
            objEventRecurMaster.ReminderTime = int.Parse(this.txtReminderTime.Text);
            objEventRecurMaster.ReminderTimeMeasurement = this.ddlReminderTimeMeasurement.SelectedValue;
            objEventRecurMaster.ReminderFrom = this.txtReminderFrom.Text;

            objEventRecurMaster.EnrollListView = this.chkEnrollListView.Checked;
            objEventRecurMaster.DisplayEndDate = this.chkDisplayEndDate.Checked;
            objEventRecurMaster.AllDayEvent = this.chkAllDayEvent.Checked;
            objEventRecurMaster.EventTimeZoneId = this.cboTimeZone.SelectedValue;
            objEventRecurMaster.SocialGroupID = this.GetUrlGroupId();
            objEventRecurMaster.SocialUserID = this.GetUrlUserId();


            // If it is possible we are edititng a recurring event create RRULE
            if (processItem == 0 || this._editRecur || !this._editRecur && objEventRMSave.RRULE == "")
            {
                objEventRecurMaster = this.CreateEventRRULE(objEventRecurMaster);
                if (this.rblRepeatTypeN.Checked)
                {
                    objEventRecurMaster.Until = objEventRecurMaster.Dtstart.Date;
                }
            }

            // If editing single occurence of recurring event & start date > last date, error
            if (processItem > 0 && objEventRMSave.RRULE != "" && !this._editRecur)
            {
                if (tStartTime.Date > objEventRMSave.Until.Date)
                {
                    this.valValidRecurStartDate.IsValid = false;
                    this.valValidRecurStartDate.Visible = true;
                    return;
                }
                if (tStartTime.Date < objEventRMSave.Dtstart.Date)
                {
                    this.valValidRecurStartDate2.IsValid = false;
                    this.valValidRecurStartDate2.Visible = true;
                    return;
                }
            }

            // If new Event or Recurring event then check for new instances
            if (processItem < 0 ||
                objEventRMSave.RRULE == "" && objEventRecurMaster.RRULE != "" && !this._editRecur || this._editRecur)
            {
                var lstEventsNew = default(ArrayList);
                lstEventsNew =
                    this._objCtlEventRecurMaster.CreateEventRecurrences(objEventRecurMaster, intDuration,
                                                                        this.Settings.Maxrecurrences);
                this._lstEvents = this.CompareOldNewEvents(this._lstEvents, lstEventsNew);

                if (lstEventsNew.Count == 0)
                {
                    // Last error!!
                    this.valValidRecurEndDate2.IsValid = false;
                    this.valValidRecurEndDate2.Visible = true;
                    return;
                }
            }

            foreach (EventInfo tempLoopVar_objEvent in this._lstEvents)
            {
                objEvent = tempLoopVar_objEvent;
                if (objEvent.EventID > 0 && objEvent.UpdateStatus != "Delete")
                {
                    objEvent.UpdateStatus = "Match";
                    var objEventSave = objEvent.Clone();
                    if (this._editRecur && objEvent.EventTimeBegin.ToShortTimeString() ==
                        objEventRMSave.Dtstart.ToShortTimeString())
                    {
                        objEvent.EventTimeBegin =
                            this.ConvertDateStringstoDatetime(objEvent.EventTimeBegin.ToShortDateString(),
                                                              Strings.Format(objEventRecurMaster.Dtstart, "HH:mm"));
                        if (tRecurEndDate.Date < objEvent.EventTimeBegin.Date)
                        {
                            tRecurEndDate = objEvent.EventTimeBegin.Date.AddDays(30);
                        }
                    }

                    if (this._editRecur && Convert.ToString(objEvent.Duration) + "M" == objEventRMSave.Duration ||
                        !this._editRecur)
                    {
                        objEvent.Duration = intDuration;
                    }

                    if (!this._editRecur)
                    {
                        objEvent.EventTimeBegin = objEventRecurMaster.Dtstart;
                        if (tRecurEndDate.Date < objEvent.EventTimeBegin.Date)
                        {
                            tRecurEndDate = objEvent.EventTimeBegin.Date.AddDays(30);
                        }
                        objEvent.Duration = intDuration;
                    }

                    if (this._editRecur && objEvent.EventName == objEventRMSave.EventName || !this._editRecur)
                    {
                        objEvent.EventName = objEventRecurMaster.EventName;
                    }
                    if (this._editRecur && objEvent.EventDesc == objEventRMSave.EventDesc || !this._editRecur)
                    {
                        objEvent.EventDesc = objEventRecurMaster.EventDesc;
                    }

                    if (this._editRecur && (int) objEvent.Importance == (int) objEventRMSave.Importance ||
                        !this._editRecur)
                    {
                        objEvent.Importance = (EventInfo.Priority) objEventRecurMaster.Importance;
                    }
                    if (this._editRecur && objEvent.Signups == objEventRMSave.Signups || !this._editRecur)
                    {
                        objEvent.Signups = objEventRecurMaster.Signups;
                    }
                    if (this._editRecur && objEvent.JournalItem == objEventRMSave.JournalItem || !this._editRecur)
                    {
                        objEvent.JournalItem = objEventRecurMaster.JournalItem;
                    }
                    if (this._editRecur && objEvent.AllowAnonEnroll == objEventRMSave.AllowAnonEnroll ||
                        !this._editRecur)
                    {
                        objEvent.AllowAnonEnroll = objEventRecurMaster.AllowAnonEnroll;
                    }
                    if (this._editRecur && objEvent.EnrollType == objEventRMSave.EnrollType || !this._editRecur)
                    {
                        objEvent.EnrollType = objEventRecurMaster.EnrollType;
                    }
                    if (this._editRecur && objEvent.PayPalAccount == objEventRMSave.PayPalAccount || !this._editRecur)
                    {
                        objEvent.PayPalAccount = objEventRecurMaster.PayPalAccount;
                    }
                    if (this._editRecur && objEvent.EnrollFee == objEventRMSave.EnrollFee || !this._editRecur)
                    {
                        objEvent.EnrollFee = objEventRecurMaster.EnrollFee;
                    }
                    if (this._editRecur && objEvent.MaxEnrollment == objEventRMSave.MaxEnrollment || !this._editRecur)
                    {
                        objEvent.MaxEnrollment = objEventRecurMaster.MaxEnrollment;
                    }
                    if (this._editRecur && objEvent.EnrollRoleID == objEventRMSave.EnrollRoleID || !this._editRecur)
                    {
                        objEvent.EnrollRoleID = objEventRecurMaster.EnrollRoleID;
                    }
                    if (this._editRecur && objEvent.DetailPage == objEventRMSave.DetailPage || !this._editRecur)
                    {
                        objEvent.DetailPage = objEventRecurMaster.DetailPage;
                    }
                    if (this._editRecur && objEvent.DetailNewWin == objEventRMSave.DetailNewWin || !this._editRecur)
                    {
                        objEvent.DetailNewWin = objEventRecurMaster.DetailNewWin;
                    }

                    if (this._editRecur && objEvent.DetailURL == objEventRMSave.DetailURL || !this._editRecur)
                    {
                        objEvent.DetailURL = objEventRecurMaster.DetailURL;
                    }

                    if (this._editRecur && objEvent.ImageDisplay == objEventRMSave.ImageDisplay || !this._editRecur)
                    {
                        objEvent.ImageDisplay = objEventRecurMaster.ImageDisplay;
                    }
                    if (this._editRecur && objEvent.ImageType == objEventRMSave.ImageType || !this._editRecur)
                    {
                        objEvent.ImageType = objEventRecurMaster.ImageType;
                    }

                    if (this._editRecur && objEvent.ImageURL == objEventRMSave.ImageURL || !this._editRecur)
                    {
                        objEvent.ImageURL = objEventRecurMaster.ImageURL;
                    }
                    if (this._editRecur && objEvent.ImageWidth == objEventRMSave.ImageWidth || !this._editRecur)
                    {
                        objEvent.ImageWidth = objEventRecurMaster.ImageWidth;
                    }
                    if (this._editRecur && objEvent.ImageHeight == objEventRMSave.ImageHeight || !this._editRecur)
                    {
                        objEvent.ImageHeight = objEventRecurMaster.ImageHeight;
                    }
                    if (this._editRecur && objEvent.Category == objEventRMSave.Category || !this._editRecur)
                    {
                        objEvent.Category = objEventRecurMaster.Category;
                    }
                    if (this._editRecur && objEvent.Location == objEventRMSave.Location || !this._editRecur)
                    {
                        objEvent.Location = objEventRecurMaster.Location;
                    }

                    // Save Event Notification Info
                    if (this._editRecur && objEvent.SendReminder == objEventRMSave.SendReminder || !this._editRecur)
                    {
                        objEvent.SendReminder = objEventRecurMaster.SendReminder;
                    }
                    if (this._editRecur && objEvent.Reminder == objEventRMSave.Reminder || !this._editRecur)
                    {
                        objEvent.Reminder = objEventRecurMaster.Reminder;
                    }
                    if (this._editRecur && objEvent.Notify == objEventRMSave.Notify || !this._editRecur)
                    {
                        objEvent.Notify = objEventRecurMaster.Notify;
                    }
                    if (this._editRecur && objEvent.ReminderTime == objEventRMSave.ReminderTime || !this._editRecur)
                    {
                        objEvent.ReminderTime = objEventRecurMaster.ReminderTime;
                    }
                    if (this._editRecur && objEvent.ReminderTimeMeasurement == objEventRMSave.ReminderTimeMeasurement ||
                        !this._editRecur)
                    {
                        objEvent.ReminderTimeMeasurement = objEventRecurMaster.ReminderTimeMeasurement;
                    }
                    if (this._editRecur && objEvent.ReminderFrom == objEventRMSave.ReminderFrom || !this._editRecur)
                    {
                        objEvent.ReminderFrom = objEventRecurMaster.ReminderFrom;
                    }
                    if (this._editRecur && objEvent.OwnerID == objEventRMSave.OwnerID || !this._editRecur)
                    {
                        objEvent.OwnerID = objEventRecurMaster.OwnerID;
                    }

                    // Set for re-submit to Search Engine
                    objEvent.SearchSubmitted = false;

                    if (this._editRecur && objEvent.CustomField1 == objEventRMSave.CustomField1 || !this._editRecur)
                    {
                        objEvent.CustomField1 = objEventRecurMaster.CustomField1;
                    }
                    if (this._editRecur && objEvent.CustomField2 == objEventRMSave.CustomField2 || !this._editRecur)
                    {
                        objEvent.CustomField2 = objEventRecurMaster.CustomField2;
                    }
                    if (this._editRecur && objEvent.EnrollListView == objEventRMSave.EnrollListView || !this._editRecur)
                    {
                        objEvent.EnrollListView = objEventRecurMaster.EnrollListView;
                    }
                    if (this._editRecur && objEvent.DisplayEndDate == objEventRMSave.DisplayEndDate || !this._editRecur)
                    {
                        objEvent.DisplayEndDate = objEventRecurMaster.DisplayEndDate;
                    }
                    if (this._editRecur && objEvent.AllDayEvent == objEventRMSave.AllDayEvent || !this._editRecur)
                    {
                        objEvent.AllDayEvent = objEventRecurMaster.AllDayEvent;
                    }
                    if (this._editRecur && objEvent.Summary == objEventRMSave.Summary || !this._editRecur)
                    {
                        objEvent.Summary = objEventRecurMaster.Summary;
                    }

                    if (objEvent.EventTimeBegin != objEventSave.EventTimeBegin ||
                        objEvent.Duration != objEventSave.Duration ||
                        objEvent.EventName != objEventSave.EventName ||
                        objEvent.EventDesc != objEventSave.EventDesc ||
                        objEvent.Importance != objEventSave.Importance ||
                        objEvent.Notify != objEventSave.Notify ||
                        objEvent.Signups != objEventSave.Signups ||
                        objEvent.AllowAnonEnroll != objEventSave.AllowAnonEnroll ||
                        (objEvent.MaxEnrollment != objEventSave.MaxEnrollment) |
                        (objEvent.EnrollRoleID != objEventSave.EnrollRoleID) |
                        (objEvent.EnrollFee != objEventSave.EnrollFee) ||
                        objEvent.EnrollType != objEventSave.EnrollType ||
                        objEvent.PayPalAccount != objEventSave.PayPalAccount ||
                        objEvent.DetailPage != objEventSave.DetailPage ||
                        objEvent.DetailNewWin != objEventSave.DetailNewWin ||
                        objEvent.DetailURL != objEventSave.DetailURL ||
                        objEvent.ImageURL != objEventSave.ImageURL ||
                        objEvent.ImageType != objEventSave.ImageType ||
                        (objEvent.ImageWidth != objEventSave.ImageWidth) |
                        (objEvent.ImageHeight != objEventSave.ImageHeight) ||
                        objEvent.ImageDisplay != objEventSave.ImageDisplay ||
                        (objEvent.Location != objEventSave.Location) |
                        (objEvent.Category != objEventSave.Category) ||
                        objEvent.Reminder != objEventSave.Reminder ||
                        objEvent.SendReminder != objEventSave.SendReminder ||
                        objEvent.ReminderTime != objEventSave.ReminderTime ||
                        objEvent.ReminderTimeMeasurement != objEventSave.ReminderTimeMeasurement ||
                        objEvent.ReminderFrom != objEventSave.ReminderFrom ||
                        objEvent.CustomField1 != objEventSave.CustomField1 ||
                        objEvent.CustomField2 != objEventSave.CustomField2 ||
                        objEvent.EnrollListView != objEventSave.EnrollListView ||
                        objEvent.DisplayEndDate != objEventSave.DisplayEndDate ||
                        objEvent.AllDayEvent != objEventSave.AllDayEvent ||
                        objEvent.Summary != objEventSave.Summary ||
                        objEvent.OwnerID != objEventSave.OwnerID)
                    {
                        objEvent.LastUpdatedID = this.UserId;
                        objEvent.Approved = objEventRecurMaster.Approved;
                        objEvent.UpdateStatus = "Update";
                    }
                }

                // Do we need to check for schedule conflict
                if (this.Settings.Preventconflicts && objEvent.UpdateStatus != "Delete")
                {
                    var getSubEvents = this.Settings.MasterEvent;
                    var categoryIDs = new ArrayList();
                    categoryIDs.Add("-1");
                    var locationIDs = new ArrayList();
                    locationIDs.Add("-1");
                    var selectedEvents =
                        objEventInfoHelper.GetEvents(objEvent.EventTimeBegin.Date,
                                                     objEvent.EventTimeBegin.AddMinutes(objEvent.Duration).Date,
                                                     getSubEvents, categoryIDs, locationIDs,
                                                     objEventRecurMaster.SocialGroupID,
                                                     objEventRecurMaster.SocialUserID);
                    var conflictDateChk = DateTime.Now;
                    var conflictDate = objEventInfoHelper.IsConflict(objEvent, selectedEvents, conflictDateChk);
                    if (conflictDate != conflictDateChk)
                    {
                        //Conflict Error
                        if (this.Settings.Locationconflict)
                        {
                            this.valLocationConflict.IsValid = false;
                            this.valLocationConflict.Visible = true;
                            this.valLocationConflict.ErrorMessage =
                                Localization.GetString("valLocationConflict", this.LocalResourceFile) + " - " +
                                string.Format("{0:g}", conflictDate);
                            this.valLocationConflict.Text =
                                Localization.GetString("valLocationConflict", this.LocalResourceFile) + " - " +
                                string.Format("{0:g}", conflictDate);
                        }
                        else
                        {
                            this.valConflict.IsValid = false;
                            this.valConflict.Visible = true;
                            this.valConflict.ErrorMessage =
                                Localization.GetString("valConflict", this.LocalResourceFile) + " - " +
                                string.Format("{0:g}", conflictDate);
                            this.valConflict.Text = Localization.GetString("valConflict", this.LocalResourceFile) +
                                                    " - " + string.Format("{0:g}", conflictDate);
                        }
                        return;
                    }
                }
            }

            if (objEventRecurMaster.RecurMasterID == -1 ||
                objEventRMSave.RRULE == "" && !this._editRecur || this._editRecur)
            {
                objEventRecurMaster =
                    this._objCtlEventRecurMaster.EventsRecurMasterSave(objEventRecurMaster, this.TabId, true);
            }

            if (objEventRecurMaster.RecurMasterID == -1)
            {
                this.SelectedDate = objEventRecurMaster.Dtstart.Date;
            }

            // url tracking
            var objUrls = new UrlController();
            objUrls.UpdateUrl(this.PortalId, Convert.ToString(this.ctlURL.Url), Convert.ToString(this.ctlURL.UrlType),
                              Convert.ToBoolean(this.ctlURL.Log), Convert.ToBoolean(this.ctlURL.Track), this.ModuleId,
                              Convert.ToBoolean(this.ctlURL.NewWindow));
            objUrls.UpdateUrl(this.PortalId, Convert.ToString(this.URLDetail.Url),
                              Convert.ToString(this.URLDetail.UrlType), Convert.ToBoolean(this.URLDetail.Log),
                              Convert.ToBoolean(this.URLDetail.Track), this.ModuleId,
                              Convert.ToBoolean(this.URLDetail.NewWindow));

            var blEmailSend = false;
            var blModeratorEmailSent = false;
            var objEventEmail = new EventInfo();
            foreach (EventInfo tempLoopVar_objEvent in this._lstEvents)
            {
                objEvent = tempLoopVar_objEvent;
                objEvent.RecurMasterID = objEventRecurMaster.RecurMasterID;
                switch (objEvent.UpdateStatus)
                {
                    case "Match":
                        break;
                    case "Delete":
                        this._objCtlEvent.EventsDelete(objEvent.EventID, objEvent.ModuleID, objEvent.ContentItemID);
                        break;
                    default:
                        if (!objEvent.Cancelled)
                        {
                            var oEvent = objEvent;
                            objEvent = this._objCtlEvent.EventsSave(objEvent, false, this.TabId, true);
                            if (!oEvent.Approved && !blModeratorEmailSent)
                            {
                                oEvent.RRULE = objEventRecurMaster.RRULE;
                                this.SendModeratorEmail(oEvent);
                                blModeratorEmailSent = true;
                            }
                            if (oEvent.EventID != -1)
                            {
                                this.UpdateExistingNotificationRecords(oEvent);
                            }
                            else
                            {
                                if (!blEmailSend)
                                {
                                    objEventEmail = objEvent;
                                    blEmailSend = true;
                                }
                            }
                        }
                        break;
                }
            }
            if (blEmailSend)
            {
                this.SendNewEventEmails(objEventEmail);
                this.CreateNewEventJournal(objEventEmail);
            }
            if (this.chkEventEmailChk.Checked)
            {
                this.SendEventEmail((EventInfo) this._lstEvents[0]);
            }
        }

        private EventRecurMasterInfo CreateEventRRULE(EventRecurMasterInfo objEventRecurMaster)
        {
            var strWkst = "";
            var culture = new CultureInfo(objEventRecurMaster.CultureName, false);
            strWkst = "SU";
            if (culture.DateTimeFormat.FirstDayOfWeek != DayOfWeek.Sunday)
            {
                strWkst = "MO";
            }

            if (this.rblRepeatTypeN.Checked)
            {
                objEventRecurMaster.RRULE = "";
            }
            else if (this.rblRepeatTypeP1.Checked)
            {
                switch (this.cmbP1Period.SelectedItem.Value.Trim())
                {
                    case "D":
                        objEventRecurMaster.RRULE = "FREQ=DAILY";
                        break;
                    case "W":
                        objEventRecurMaster.RRULE = "FREQ=WEEKLY;WKST=" + strWkst;
                        break;
                    case "M":
                        objEventRecurMaster.RRULE = "FREQ=MONTHLY";
                        break;
                    case "Y":
                        objEventRecurMaster.RRULE = "FREQ=YEARLY";
                        break;
                }

                objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + ";INTERVAL=" + this.txtP1Every.Text;
            }
            else if (this.rblRepeatTypeW1.Checked)
            {
                objEventRecurMaster.RRULE = "FREQ=WEEKLY;WKST=" + strWkst + ";INTERVAL=" + this.txtW1Every.Text +
                                            ";BYDAY=";
                if (this.chkW1Sun.Checked || this.chkW1Sun2.Checked)
                {
                    objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + "SU,";
                }
                if (this.chkW1Mon.Checked)
                {
                    objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + "MO,";
                }
                if (this.chkW1Tue.Checked)
                {
                    objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + "TU,";
                }
                if (this.chkW1Wed.Checked)
                {
                    objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + "WE,";
                }
                if (this.chkW1Thu.Checked)
                {
                    objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + "TH,";
                }
                if (this.chkW1Fri.Checked)
                {
                    objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + "FR,";
                }
                if (this.chkW1Sat.Checked)
                {
                    objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + "SA,";
                }
                objEventRecurMaster.RRULE =
                    objEventRecurMaster.RRULE.Substring(0, objEventRecurMaster.RRULE.Length - 1);
            }
            else if (this.rblRepeatTypeM1.Checked && this.rblRepeatTypeM.Checked)
            {
                objEventRecurMaster.RRULE = "FREQ=MONTHLY;INTERVAL=" + this.txtMEvery.Text + ";BYDAY=";
                var intWeek = 0;
                var strWeek = "";
                if (this.cmbM1Every.SelectedIndex < 4)
                {
                    intWeek = this.cmbM1Every.SelectedIndex + 1;
                    strWeek = "+" + Convert.ToString(intWeek);
                }
                else
                {
                    intWeek = -1;
                    strWeek = Convert.ToString(intWeek);
                }
                objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + strWeek;

                var strDay = "";
                switch (this.cmbM1Period.SelectedValue)
                {
                    case "0":
                        strDay = "SU";
                        break;
                    case "1":
                        strDay = "MO";
                        break;
                    case "2":
                        strDay = "TU";
                        break;
                    case "3":
                        strDay = "WE";
                        break;
                    case "4":
                        strDay = "TH";
                        break;
                    case "5":
                        strDay = "FR";
                        break;
                    case "6":
                        strDay = "SA";
                        break;
                }
                objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + strDay;
            }
            else if (this.rblRepeatTypeM2.Checked && this.rblRepeatTypeM.Checked)
            {
                objEventRecurMaster.RRULE = "FREQ=MONTHLY;INTERVAL=" + this.txtMEvery.Text + ";BYMONTHDAY=+" +
                                            this.cmbM2Period.SelectedValue;
            }
            else if (this.rblRepeatTypeY1.Checked)
            {
                var yearDate = Convert.ToDateTime(this.dpY1Period.SelectedDate);
                objEventRecurMaster.RRULE = "FREQ=YEARLY;INTERVAL=1;BYMONTH=" + Convert.ToString(yearDate.Month) +
                                            ";BYMONTHDAY=+" + Convert.ToString(yearDate.Day);
            }
            return objEventRecurMaster;
        }

        private ArrayList CompareOldNewEvents(ArrayList lstEventsOld, ArrayList lstEventsNew)
        {
            var objEventOld = default(EventInfo);
            var objEventNew = default(EventInfo);
            foreach (EventInfo tempLoopVar_objEventOld in lstEventsOld)
            {
                objEventOld = tempLoopVar_objEventOld;
                objEventOld.UpdateStatus = "Delete";
                foreach (EventInfo tempLoopVar_objEventNew in lstEventsNew)
                {
                    objEventNew = tempLoopVar_objEventNew;
                    if (objEventOld.OriginalDateBegin == objEventNew.OriginalDateBegin)
                    {
                        objEventOld.UpdateStatus = "Match";
                    }
                }
            }
            foreach (EventInfo tempLoopVar_objEventNew in lstEventsNew)
            {
                objEventNew = tempLoopVar_objEventNew;
                objEventNew.UpdateStatus = "Add";
                foreach (EventInfo tempLoopVar_objEventOld in lstEventsOld)
                {
                    objEventOld = tempLoopVar_objEventOld;
                    if (objEventOld.OriginalDateBegin == objEventNew.OriginalDateBegin)
                    {
                        objEventNew.UpdateStatus = "Match";
                    }
                }
            }
            foreach (EventInfo tempLoopVar_objEventNew in lstEventsNew)
            {
                objEventNew = tempLoopVar_objEventNew;
                if (objEventNew.UpdateStatus == "Add")
                {
                    lstEventsOld.Add(objEventNew);
                }
            }
            return lstEventsOld;
        }

        private void UpdateExistingNotificationRecords(EventInfo objEvent)
        {
            try
            {
                // Add Notification Records to Database, if required
                if (this.chkReminder.Checked)
                {
                    var eventTimeBegin = default(DateTime);
                    //Adjust Begin Time to UTC
                    eventTimeBegin = objEvent.EventTimeBegin;
                    eventTimeBegin =
                        eventTimeBegin
                            .AddMinutes(0); //only to pass to EventsNotificationTimeChange in correct format...
                    // Update Time for any existing Notifications for the Event
                    var objEventNotificationController = new EventNotificationController();
                    objEventNotificationController.EventsNotificationTimeChange(
                        objEvent.EventID, eventTimeBegin, this.ModuleId);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void SendModeratorEmail(EventInfo objEvent)
        {
            try
            {
                // Send Moderator email
                if (this.Settings.Moderateall)
                {
                    var objEventEmailInfo = new EventEmailInfo();
                    var objEventEmail = new EventEmails(this.PortalId, this.ModuleId, this.LocalResourceFile,
                                                        ((PageBase) this.Page).PageCulture.Name);
                    objEventEmailInfo.TxtEmailSubject = this.Settings.Templates.moderateemailsubject;
                    objEventEmailInfo.TxtEmailBody = this.Settings.Templates.moderateemailmessage;
                    objEventEmailInfo.TxtEmailFrom = this.Settings.StandardEmail;
                    var moderators = this.GetModerators();
                    foreach (UserInfo moderator in moderators)
                    {
                        objEventEmailInfo.UserEmails.Add(moderator.Email);
                        objEventEmailInfo.UserLocales.Add(moderator.Profile.PreferredLocale);
                        objEventEmailInfo.UserTimeZoneIds.Add(moderator.Profile.PreferredTimeZone.Id);
                    }
                    objEventEmail.SendEmails(objEventEmailInfo, objEvent);
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void SendEventEmail(EventInfo objEventEmailIn)
        {
            var objEventEmailInfo = new EventEmailInfo();
            var objEventEmail = new EventEmails(this.PortalId, this.ModuleId, this.LocalResourceFile,
                                                ((PageBase) this.Page).PageCulture.Name);
            objEventEmailInfo.TxtEmailSubject = this.txtEventEmailSubject.Text;
            objEventEmailInfo.TxtEmailBody = this.txtEventEmailBody.Text;
            objEventEmailInfo.TxtEmailFrom = this.txtEventEmailFrom.Text;
            this.EventEmailAddRoleUsers(int.Parse(this.ddEventEmailRoles.SelectedValue), objEventEmailInfo);
            objEventEmail.SendEmails(objEventEmailInfo, objEventEmailIn);
        }

        private DateTime ConvertDateStringstoDatetime(string strDate, string strTime)
        {
            var invCulture = CultureInfo.InvariantCulture;

            var tDate = default(DateTime);
            tDate = Convert.ToDateTime(strDate).Date;

            // Since dates may not be in a form directly combinable with time, convert back to string to enable combination
            strDate = tDate.ToString("yyyy/MM/dd", invCulture);
            tDate = DateTime.ParseExact(strDate + " " + strTime, "yyyy/MM/dd HH:mm", invCulture);
            return tDate;
        }

        private void BuildEnrolleeGrid(EventInfo objEvent)
        {
            var objSignups = default(ArrayList);
            // Refresh Enrollment Grid
            if (this._editRecur)
            {
                objSignups =
                    this._objCtlEventSignups
                        .EventsSignupsGetEventRecurMaster(objEvent.RecurMasterID, objEvent.ModuleID);
            }
            else
            {
                objSignups = this._objCtlEventSignups.EventsSignupsGetEvent(objEvent.EventID, objEvent.ModuleID);
            }

            var eventEnrollment = new ArrayList();
            var objSignup = default(EventSignupsInfo);
            var objCtlUser = new UserController();
            var noEnrolees = 0;
            foreach (EventSignupsInfo tempLoopVar_objSignup in objSignups)
            {
                objSignup = tempLoopVar_objSignup;
                var objEnrollListItem = new EventEnrollList();
                noEnrolees += objSignup.NoEnrolees;
                if (objSignup.UserID != -1)
                {
                    var objUser = default(UserInfo);
                    objUser = objCtlUser.GetUser(this.PortalId, objSignup.UserID);
                    var objEventInfoHelper =
                        new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);
                    objEnrollListItem.EnrollDisplayName = objEventInfoHelper
                        .UserDisplayNameProfile(objSignup.UserID, objSignup.UserName, this.LocalResourceFile)
                        .DisplayNameURL;
                    if (!ReferenceEquals(objUser, null))
                    {
                        objEnrollListItem.EnrollUserName = objUser.Username;
                        objEnrollListItem.EnrollEmail =
                            string.Format("<a href=\"mailto:{0}?subject={1}\">{0}</a>", objSignup.Email,
                                          objEvent.EventName);
                        objEnrollListItem.EnrollPhone = objUser.Profile.Telephone;
                    }
                }
                else
                {
                    objEnrollListItem.EnrollDisplayName = objSignup.AnonName;
                    objEnrollListItem.EnrollUserName = Localization.GetString("AnonUser", this.LocalResourceFile);
                    objEnrollListItem.EnrollEmail =
                        string.Format("<a href=\"mailto:{0}?subject={1}\">{0}</a>", objSignup.AnonEmail,
                                      objEvent.EventName);
                    objEnrollListItem.EnrollPhone = objSignup.AnonTelephone;
                }
                objEnrollListItem.SignupID = objSignup.SignupID;
                objEnrollListItem.EnrollApproved = objSignup.Approved;
                objEnrollListItem.EnrollNo = objSignup.NoEnrolees;
                objEnrollListItem.EnrollTimeBegin = objSignup.EventTimeBegin;
                eventEnrollment.Add(objEnrollListItem);
            }

            if (eventEnrollment.Count > 0)
            {
                this.grdEnrollment.DataSource = eventEnrollment;
                this.grdEnrollment.DataBind();
                this.tblEventEmail.Attributes.Add("style", "display:block; width:100%");
                this.lblEnrolledUsers.Visible = true;
                this.grdEnrollment.Visible = true;
                this.lnkSelectedDelete.Visible = true;
                this.lnkSelectedEmail.Visible = true;
            }
            else
            {
                this.lblEnrolledUsers.Visible = false;
                this.grdEnrollment.Visible = false;
                if (!this.Settings.Newpereventemail)
                {
                    this.tblEventEmail.Attributes.Add("style", "display:none; width:100%");
                }
                this.lnkSelectedDelete.Visible = false;
                this.lnkSelectedEmail.Visible = false;
            }

            objEvent.Enrolled = eventEnrollment.Count;
            objEvent.Signups = true;
            this.ShowHideEnrolleeColumns(objEvent);

            this.txtEnrolled.Text = noEnrolees.ToString();
            this.valNoEnrolees.MaximumValue = Convert.ToString(objEvent.MaxEnrollment - noEnrolees);
            if ((int.Parse(this.valNoEnrolees.MaximumValue) > this.Settings.Maxnoenrolees) |
                (objEvent.MaxEnrollment == 0))
            {
                this.valNoEnrolees.MaximumValue = this.Settings.Maxnoenrolees.ToString();
            }
            else if (int.Parse(this.valNoEnrolees.MaximumValue) < 1)
            {
                this.valNoEnrolees.MaximumValue = "1";
            }
            this.lblMaxNoEnrolees.Text =
                string.Format(Localization.GetString("lblMaxNoEnrolees", this.LocalResourceFile),
                              this.valNoEnrolees.MaximumValue);
        }

        private void ShowHideEnrolleeColumns(EventInfo objEvent)
        {
            var txtColumns = this.EnrolmentColumns(objEvent, true);
            var gvUsersToEnroll = (GridView) this.grdAddUser.FindControl("gvUsersToEnroll");
            if (txtColumns.LastIndexOf("UserName", StringComparison.Ordinal) < 0)
            {
                this.grdEnrollment.Columns[1].Visible = false;
                gvUsersToEnroll.Columns[1].Visible = false;
            }
            else
            {
                this.grdEnrollment.Columns[1].Visible = true;
                gvUsersToEnroll.Columns[1].Visible = true;
            }
            if (txtColumns.LastIndexOf("DisplayName", StringComparison.Ordinal) < 0)
            {
                this.grdEnrollment.Columns[2].Visible = false;
                gvUsersToEnroll.Columns[2].Visible = false;
            }
            else
            {
                this.grdEnrollment.Columns[2].Visible = true;
                gvUsersToEnroll.Columns[2].Visible = true;
            }
            if (txtColumns.LastIndexOf("Email", StringComparison.Ordinal) < 0)
            {
                this.grdEnrollment.Columns[3].Visible = false;
                gvUsersToEnroll.Columns[3].Visible = false;
            }
            else
            {
                this.grdEnrollment.Columns[3].Visible = true;
                gvUsersToEnroll.Columns[3].Visible = true;
            }
            if (txtColumns.LastIndexOf("Phone", StringComparison.Ordinal) < 0)
            {
                this.grdEnrollment.Columns[4].Visible = false;
            }
            else
            {
                this.grdEnrollment.Columns[4].Visible = true;
            }
            if (txtColumns.LastIndexOf("Approved", StringComparison.Ordinal) < 0)
            {
                this.grdEnrollment.Columns[5].Visible = false;
            }
            else
            {
                this.grdEnrollment.Columns[5].Visible = true;
            }
            if (txtColumns.LastIndexOf("Qty", StringComparison.Ordinal) < 0)
            {
                this.grdEnrollment.Columns[6].Visible = false;
            }
            else
            {
                this.grdEnrollment.Columns[6].Visible = true;
            }
            if (this._editRecur)
            {
                this.grdEnrollment.Columns[7].Visible = true;
            }
            else
            {
                this.grdEnrollment.Columns[7].Visible = false;
            }
        }

        private void AddRegUser(int inUserID, EventInfo objEvent)
        {
            // Check if signup already exists since due to partial rendering it may be possible
            // to click the enroll user link twice
            var intUserID = inUserID;
            this._objEventSignups =
                this._objCtlEventSignups.EventsSignupsGetUser(objEvent.EventID, intUserID, objEvent.ModuleID);

            if (ReferenceEquals(this._objEventSignups, null))
            {
                // Get user info
                var objUserInfo = UserController.GetUserById(this.PortalId, intUserID);

                this._objEventSignups = new EventSignupsInfo();
                this._objEventSignups.EventID = objEvent.EventID;
                this._objEventSignups.ModuleID = objEvent.ModuleID;
                this._objEventSignups.UserID = intUserID;
                this._objEventSignups.AnonEmail = null;
                this._objEventSignups.AnonName = null;
                this._objEventSignups.AnonTelephone = null;
                this._objEventSignups.AnonCulture = null;
                this._objEventSignups.AnonTimeZoneId = null;
                this._objEventSignups.PayPalPaymentDate = DateTime.UtcNow;
                this._objEventSignups.Approved = true;
                this._objEventSignups.NoEnrolees = int.Parse(this.txtNoEnrolees.Text);
                this._objEventSignups = this.CreateEnrollment(this._objEventSignups, objEvent);

                // Mail users
                if (this.Settings.SendEnrollMessageAdded)
                {
                    var objEventEmailInfo = new EventEmailInfo();
                    var objEventEmail = new EventEmails(this.PortalId, this.ModuleId, this.LocalResourceFile,
                                                        ((PageBase) this.Page).PageCulture.Name);
                    objEventEmailInfo.TxtEmailSubject = this.Settings.Templates.txtEnrollMessageSubject;
                    objEventEmailInfo.TxtEmailBody = this.Settings.Templates.txtEnrollMessageAdded;
                    objEventEmailInfo.TxtEmailFrom = this.Settings.StandardEmail;
                    objEventEmailInfo.UserIDs.Add(this._objEventSignups.UserID);
                    objEventEmailInfo.UserIDs.Add(objEvent.OwnerID);
                    objEventEmail.SendEmails(objEventEmailInfo, objEvent, this._objEventSignups);
                }
            }
        }

        private bool ValidateTime(DateTime indate)
        {
            var inMinutes = indate.Minute;
            var remainder = inMinutes % int.Parse(this.Settings.Timeinterval);
            if (remainder > 0)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region Links and Buttons

        protected void cancelButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.Response.Redirect(this.GetStoredPrevPage(), true);
            }
            catch (Exception) //Module failed to load
            {
                //ProcessModuleLoadException(Me, exc)
            }
        }

        protected void updateButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.UpdateProcessing(this._itemID);
                if (this.Page.IsValid)
                {
                    this.Response.Redirect(this.GetStoredPrevPage(), true);
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void deleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                this._objEvent = this._objCtlEvent.EventsGet(this._itemID, this.ModuleId);
                if (this._editRecur)
                {
                    this._objCtlEventRecurMaster.EventsRecurMasterDelete(
                        this._objEvent.RecurMasterID, this._objEvent.ModuleID);
                }
                else
                {
                    if (this._objEvent.RRULE != "")
                    {
                        this._objEvent.Cancelled = true;
                        this._objEvent.LastUpdatedID = this.UserId;
                        this._objEvent = this._objCtlEvent.EventsSave(this._objEvent, false, this.TabId, true);
                    }
                    else
                    {
                        this._objCtlEventRecurMaster.EventsRecurMasterDelete(
                            this._objEvent.RecurMasterID, this._objEvent.ModuleID);
                    }
                }
                this.Response.Redirect(this.GetSocialNavigateUrl(), true);
            }
            catch (Exception) //Module failed to load
            {
                //ProcessModuleLoadException(Me, exc)
            }
        }

        protected void lnkSelectedEmail_Click(object sender, EventArgs e)
        {
            this.Email(true);
        }

        protected void lnkSelectedDelete_Click(object sender, EventArgs e)
        {
            var item = default(DataGridItem);
            var objEnroll = default(EventSignupsInfo);
            var eventID = 0;

            foreach (DataGridItem tempLoopVar_item in this.grdEnrollment.Items)
            {
                item = tempLoopVar_item;
                if (((CheckBox) item.FindControl("chkSelect")).Checked)
                {
                    var intSignupID = Convert.ToInt32(this.grdEnrollment.DataKeys[item.ItemIndex]);
                    objEnroll = this._objCtlEventSignups.EventsSignupsGet(intSignupID, this.ModuleId, false);
                    if (!ReferenceEquals(objEnroll, null))
                    {
                        if (eventID != objEnroll.EventID)
                        {
                            this._objEvent = this._objCtlEvent.EventsGet(objEnroll.EventID, this.ModuleId);
                        }
                        eventID = objEnroll.EventID;

                        // Delete Selected Enrollee
                        this.DeleteEnrollment(intSignupID, this._objEvent.ModuleID, this._objEvent.EventID);

                        // Mail users
                        if (this.Settings.SendEnrollMessageDeleted)
                        {
                            var objEventEmailInfo = new EventEmailInfo();
                            var objEventEmail =
                                new EventEmails(this.PortalId, this.ModuleId, this.LocalResourceFile,
                                                ((PageBase) this.Page).PageCulture.Name);
                            objEventEmailInfo.TxtEmailSubject = this.Settings.Templates.txtEnrollMessageSubject;
                            objEventEmailInfo.TxtEmailBody = this.Settings.Templates.txtEnrollMessageDeleted;
                            objEventEmailInfo.TxtEmailFrom = this.Settings.StandardEmail;
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
                            objEventEmailInfo.UserIDs.Add(this._objEvent.OwnerID);
                            objEventEmail.SendEmails(objEventEmailInfo, this._objEvent, objEnroll);
                        }
                    }
                }
            }

            this.LoadRegUsers();
            this.BuildEnrolleeGrid(this._objEvent);
        }

        protected void copyButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.UpdateProcessing(-1);
                if (this.Page.IsValid)
                {
                    this.Response.Redirect(this.GetStoredPrevPage(), true);
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void ddEnrollRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.LoadRegUsers();
        }

        protected void chkSignups_CheckedChanged(object sender, EventArgs e)
        {
            this.tblEventEmail.Attributes.Add("style", "display:none; width:100%");
            if (this.chkSignups.Checked)
            {
                this.tblEnrollmentDetails.Attributes.Add("style", "display:block;");
                this.LoadRegUsers();
                if (this.txtEnrolled.Text != 0.ToString())
                {
                    this.tblEventEmail.Attributes.Add("style", "display:block; width:100%");
                }
            }
            else
            {
                this.tblEnrollmentDetails.Attributes.Add("style", "display:none;");
                if (this.Settings.Newpereventemail && this.chkEventEmailChk.Checked)
                {
                    this.tblEventEmail.Attributes.Add("style", "display:block; width:100%");
                }
            }
        }

        protected void grdAddUser_AddSelectedUsers(object sender, EventArgs e, ArrayList arrUsers)
        {
            try
            {
                if (int.Parse(this.txtNoEnrolees.Text) > int.Parse(this.valNoEnrolees.MaximumValue) ||
                    int.Parse(this.txtNoEnrolees.Text) < int.Parse(this.valNoEnrolees.MinimumValue))
                {
                    this.valNoEnrolees.IsValid = false;
                    this.valNoEnrolees.Visible = true;
                    this.valNoEnrolees.ErrorMessage =
                        string.Format(Localization.GetString("valNoEnrolees", this.LocalResourceFile),
                                      this.valNoEnrolees.MaximumValue);
                    return;
                }
            }
            catch
            {
                this.valNoEnrolees.IsValid = false;
                this.valNoEnrolees.Visible = true;
                this.valNoEnrolees.ErrorMessage =
                    string.Format(Localization.GetString("valNoEnrolees", this.LocalResourceFile),
                                  this.valNoEnrolees.MaximumValue);
                return;
            }

            var objEvent = this._objCtlEvent.EventsGet(this._itemID, this.ModuleId);

            foreach (int inUserid in arrUsers)
            {
                this.AddRegUser(inUserid, objEvent);
            }
            this.LoadRegUsers();
            this.BuildEnrolleeGrid(objEvent);
            this.txtNoEnrolees.Text = Convert.ToString(1.ToString());
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
    }

    #region Comparer Class

    public class UserListSort : IComparer
    {
        public int Compare(object x, object y)
        {
            var xdisplayname = "";
            var ydisplayname = "";

            xdisplayname = ((EventUser) x).DisplayName;
            ydisplayname = ((EventUser) y).DisplayName;
            var c = new CaseInsensitiveComparer();
            return c.Compare(xdisplayname, ydisplayname);
        }
    }

    #endregion
}