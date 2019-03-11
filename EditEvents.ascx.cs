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
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Web.UI.WebControls;
using Components;
using DNNtc;
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
using Microsoft.VisualBasic;
using EventInfo = Components.EventInfo;
using Globals = DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Events
{
    [DNNtc.ModuleControlProperties("Edit", "Edit Events", DNNtc.ControlType.View, "https://github.com/DNNCommunity/DNN.Events/wiki", false, true)]
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
                if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) || IsModuleEditor())
                { }
                else
                {
                    // to stop errors when not authorised to edit
                    valReminderTime.MinimumValue = "15";
                    valReminderTime.MaximumValue = "60";

                    Response.Redirect(GetSocialNavigateUrl(), true);
                }

                grdAddUser.ModuleConfiguration = ModuleConfiguration.Clone();

                // Add the external Validation.js to the Page
                const string csname = "ExtValidationScriptFile";
                var cstype = MethodBase.GetCurrentMethod().GetType();
                var cstext = "<script src=\"" + ResolveUrl("~/DesktopModules/Events/Scripts/Validation.js") +
                             "\" type=\"text/javascript\"></script>";
                if (!Page.ClientScript.IsClientScriptBlockRegistered(csname))
                {
                    Page.ClientScript.RegisterClientScriptBlock(cstype, csname, cstext, false);
                }

                // Determine ItemId of Event to Update
                if (!ReferenceEquals(Request.Params["ItemId"], null))
                {
                    _itemID = int.Parse(Request.Params["ItemId"]);
                }
                _editRecur = false;
                if (!ReferenceEquals(Request.Params["EditRecur"], null))
                {
                    if (Request.Params["EditRecur"].ToLower() == "all")
                    {
                        _editRecur = true;
                    }
                }

                // Set the selected theme
                SetTheme(pnlEventsModuleEdit);

                //EPT: "Changed DotNetNuke.Security.PortalSecurity.HasEditPermissions(ModuleId)" into "IsEditable"
                //RWJS: Replaced with custom function IsModuleEditor which checks whether users has editor permissions
                if (IsModuleEditor() ||
                    IsModerator() ||
                    PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName))
                { }
                else
                {
                    Response.Redirect(GetSocialNavigateUrl(), true);
                }

                trOwner.Visible = false;
                if (IsModerator() && Settings.Ownerchangeallowed ||
                    PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName))
                {
                    trOwner.Visible = true;
                }

                pnlEnroll.Visible = false;
                divNoEnrolees.Visible = false;

                if (Settings.Eventsignup)
                {
                    pnlEnroll.Visible = true;
                    chkSignups.Visible = true;
                    if (Settings.Maxnoenrolees > 1 && divAddUser.Visible)
                    {
                        divNoEnrolees.Visible = true;
                    }
                }

                trTypeOfEnrollment.Visible = Settings.Eventsignupallowpaid;
                trPayPalAccount.Visible = Settings.Eventsignupallowpaid;

                tblEventEmail.Attributes.Add("style", "display:none; width:100%");
                if (!Settings.Newpereventemail)
                {
                    pnlEventEmailRole.Visible = false;
                }
                else if (_itemID == -1 && Settings.Moderateall && !IsModerator())
                {
                    pnlEventEmailRole.Visible = false;
                }

                pnlReminder.Visible = Settings.Eventnotify;
                pnlImage.Visible = Settings.Eventimage;
                pnlDetailPage.Visible = Settings.DetailPageAllowed;

                // Setup Popup Event
                dpStartDate.ClientEvents.OnDateSelected =
                    "function() {if (Page_ClientValidate('startdate')) CopyField('" + dpStartDate.ClientID +
                    "','" + dpEndDate.ClientID + "');}";
                tpStartTime.ClientEvents.OnDateSelected =
                    "function() {SetComboIndex('" + tpStartTime.ClientID + "','" + tpEndTime.ClientID +
                    "','" + dpStartDate.ClientID + "','" + dpEndDate.ClientID + "','" +
                    Settings.Timeinterval + "');}";
                ctlURL.FileFilter = Globals.glbImageFileTypes;
                if (!Page.IsPostBack)
                {
                    txtSubject.MaxLength++;
                    txtReminder.MaxLength++;
                }
                var limitSubject = "javascript:limitText(this," + (txtSubject.MaxLength - 1) + ",'" +
                                   Localization.GetString("LimitChars", LocalResourceFile) + "');";
                var limitReminder = "javascript:limitText(this," + (txtReminder.MaxLength - 1) + ",'" +
                                    Localization.GetString("LimitChars", LocalResourceFile) + "');";
                txtSubject.Attributes.Add("onkeydown", limitSubject);
                txtSubject.Attributes.Add("onkeyup", limitSubject);
                txtReminder.Attributes.Add("onkeydown", limitReminder);
                txtReminder.Attributes.Add("onkeyup", limitReminder);

                Page.ClientScript.RegisterExpandoAttribute(valValidStartTime2.ClientID, "TimeInterval",
                                                                Settings.Timeinterval);
                Page.ClientScript.RegisterExpandoAttribute(valValidStartTime2.ClientID, "ErrorMessage",
                                                                string.Format(
                                                                    Localization.GetString(
                                                                        "valValidStartTime2", LocalResourceFile),
                                                                    Settings.Timeinterval));
                Page.ClientScript.RegisterExpandoAttribute(valValidStartTime2.ClientID, "ClientID",
                                                                tpStartTime.ClientID);
                Page.ClientScript.RegisterExpandoAttribute(valValidEndTime2.ClientID, "TimeInterval",
                                                                Settings.Timeinterval);
                Page.ClientScript.RegisterExpandoAttribute(valValidEndTime2.ClientID, "ErrorMessage",
                                                                string.Format(
                                                                    Localization.GetString(
                                                                        "valValidEndTime2", LocalResourceFile),
                                                                    Settings.Timeinterval));
                Page.ClientScript.RegisterExpandoAttribute(valValidEndTime2.ClientID, "ClientID",
                                                                tpEndTime.ClientID);

                // If the page is being requested the first time, determine if an
                // contact itemId value is specified, and if so populate page
                // contents with the contact details
                if (!Page.IsPostBack)
                {
                    LocalizeAll();
                    LoadEvent();
                }

                if (chkReminder.Checked)
                {
                    tblReminderDetail.Attributes.Add("style", "display:block;");
                }
                else
                {
                    tblReminderDetail.Attributes.Add("style", "display:none;");
                }

                if (chkDetailPage.Checked)
                {
                    tblDetailPageDetail.Attributes.Add("style", "display:block;");
                }
                else
                {
                    tblDetailPageDetail.Attributes.Add("style", "display:none;");
                }

                if (chkDisplayImage.Checked)
                {
                    tblImageURL.Attributes.Add("style", "display:block;");
                }
                else
                {
                    tblImageURL.Attributes.Add("style", "display:none;");
                }

                if (chkSignups.Checked)
                {
                    tblEnrollmentDetails.Attributes.Add("style", "display:block;");
                }
                else
                {
                    tblEnrollmentDetails.Attributes.Add("style", "display:none;");
                }

                if (chkReccuring.Checked)
                {
                    tblRecurringDetails.Attributes.Add("style", "display:block;");
                }
                else
                {
                    tblRecurringDetails.Attributes.Add("style", "display:none;");
                }

                if (chkEventEmailChk.Checked)
                {
                    tblEventEmailRoleDetail.Attributes.Add("style", "display:block;");
                }
                else
                {
                    tblEventEmailRoleDetail.Attributes.Add("style", "display:none;");
                }

                if (rblRepeatTypeP1.Checked)
                {
                    tblDetailP1.Attributes.Add(
                        "style", "display:" + RecurTableDisplayType + ";white-space:nowrap;");
                }
                else
                {
                    tblDetailP1.Attributes.Add("style", "display:none;white-space:nowrap;");
                }

                if (rblRepeatTypeW1.Checked)
                {
                    tblDetailW1.Attributes.Add("style", "display:" + RecurTableDisplayType + ";");
                }
                else
                {
                    tblDetailW1.Attributes.Add("style", "display:none;");
                }

                if (rblRepeatTypeM.Checked)
                {
                    tblDetailM1.Attributes.Add("style", "display:" + RecurTableDisplayType + ";");
                }
                else
                {
                    tblDetailM1.Attributes.Add("style", "display:none;");
                }

                if (rblRepeatTypeY1.Checked)
                {
                    tblDetailY1.Attributes.Add("style", "display:" + RecurTableDisplayType + ";");
                }
                else
                {
                    tblDetailY1.Attributes.Add("style", "display:none;");
                }

                if (dpY1Period.SelectedDate.ToString().Length == 0)
                {
                    dpY1Period.SelectedDate = SelectedDate.Date;
                }
                if (txtReminderFrom.Text.Length == 0)
                {
                    txtReminderFrom.Text = Settings.Reminderfrom;
                }
                if (txtEventEmailFrom.Text.Length == 0)
                {
                    txtEventEmailFrom.Text = Settings.Reminderfrom;
                }

                if (chkAllDayEvent.Checked)
                {
                    divStartTime.Attributes.Add("style", "display:none;");
                    divEndTime.Attributes.Add("style", "display:none;");
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void valValidStartDate3_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (ReferenceEquals(dpStartDate.SelectedDate, null))
            {
                args.IsValid = false;
                valValidStartDate.Visible = true;
            }
        }

        protected void valValidStartTime2_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var inDate = Convert.ToDateTime(tpStartTime.SelectedDate);
            valValidStartTime2.ErrorMessage =
                string.Format(Localization.GetString("valValidStartTime2", LocalResourceFile),
                              Settings.Timeinterval);
            args.IsValid = ValidateTime(inDate);
        }

        protected void valValidEndTime2_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var inDate = Convert.ToDateTime(tpEndTime.SelectedDate);
            valValidEndTime2.ErrorMessage =
                string.Format(Localization.GetString("valValidEndTime2", LocalResourceFile),
                              Settings.Timeinterval);
            args.IsValid = ValidateTime(inDate);
        }

        protected void valValidRecurEndDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (ReferenceEquals(dpStartDate.SelectedDate, null))
            {
                return;
            }
            var recurDate = Convert.ToDateTime(dpRecurEndDate.SelectedDate);
            var startDate = Convert.ToDateTime(dpStartDate.SelectedDate);
            if (recurDate < startDate && !rblRepeatTypeN.Checked)
            {
                args.IsValid = false;
                valValidRecurEndDate.Visible = true;
            }
            else
            {
                args.IsValid = true;
                valValidRecurEndDate.Visible = false;
            }
        }

        #endregion

        #region Helper Methods and Functions

        private void LocalizeAll()
        {
            var culture = Thread.CurrentThread.CurrentCulture;

            txtSubject.Text = Settings.Templates.txtSubject;
            txtReminder.Text = Settings.Templates.txtMessage;

            grdEnrollment.Columns[0].HeaderText = Localization.GetString("Select", LocalResourceFile);
            grdEnrollment.Columns[1].HeaderText = Localization.GetString("EnrollUserName", LocalResourceFile);
            grdEnrollment.Columns[2].HeaderText =
                Localization.GetString("EnrollDisplayName", LocalResourceFile);
            grdEnrollment.Columns[3].HeaderText = Localization.GetString("EnrollEmail", LocalResourceFile);
            grdEnrollment.Columns[4].HeaderText = Localization.GetString("EnrollPhone", LocalResourceFile);
            grdEnrollment.Columns[5].HeaderText = Localization.GetString("EnrollApproved", LocalResourceFile);
            grdEnrollment.Columns[6].HeaderText = Localization.GetString("EnrollNo", LocalResourceFile);
            grdEnrollment.Columns[7].HeaderText = Localization.GetString("EventStart", LocalResourceFile);

            chkW1Sun.Text = culture.DateTimeFormat.AbbreviatedDayNames[(int) DayOfWeek.Sunday];
            chkW1Sun2.Text = culture.DateTimeFormat.AbbreviatedDayNames[(int) DayOfWeek.Sunday];
            chkW1Mon.Text = culture.DateTimeFormat.AbbreviatedDayNames[(int) DayOfWeek.Monday];
            chkW1Tue.Text = culture.DateTimeFormat.AbbreviatedDayNames[(int) DayOfWeek.Tuesday];
            chkW1Wed.Text = culture.DateTimeFormat.AbbreviatedDayNames[(int) DayOfWeek.Wednesday];
            chkW1Thu.Text = culture.DateTimeFormat.AbbreviatedDayNames[(int) DayOfWeek.Thursday];
            chkW1Fri.Text = culture.DateTimeFormat.AbbreviatedDayNames[(int) DayOfWeek.Friday];
            chkW1Sat.Text = culture.DateTimeFormat.AbbreviatedDayNames[(int) DayOfWeek.Saturday];

            cmbM1Period.Items.Clear();
            // Corrected a problem w/Every nth Week on a specific day with the following
            cmbM1Period.Items.Add(new ListItem(culture.DateTimeFormat.GetDayName(DayOfWeek.Sunday), "0"));
            cmbM1Period.Items.Add(new ListItem(culture.DateTimeFormat.GetDayName(DayOfWeek.Monday), "1"));
            cmbM1Period.Items.Add(new ListItem(culture.DateTimeFormat.GetDayName(DayOfWeek.Tuesday), "2"));
            cmbM1Period.Items.Add(new ListItem(culture.DateTimeFormat.GetDayName(DayOfWeek.Wednesday), "3"));
            cmbM1Period.Items.Add(new ListItem(culture.DateTimeFormat.GetDayName(DayOfWeek.Thursday), "4"));
            cmbM1Period.Items.Add(new ListItem(culture.DateTimeFormat.GetDayName(DayOfWeek.Friday), "5"));
            cmbM1Period.Items.Add(new ListItem(culture.DateTimeFormat.GetDayName(DayOfWeek.Saturday), "6"));

            cmbM2Period.Items.Clear();
            for (var i = 1; i <= 31; i++)
            {
                cmbM2Period.Items.Add(new ListItem(Localization.GetString(i.ToString(), LocalResourceFile),
                                                        i.ToString()));
            }

            lblMaxRecurrences.Text =
                string.Format(Localization.GetString("lblMaxRecurrences", LocalResourceFile),
                              Settings.Maxrecurrences);

            if (culture.DateTimeFormat.FirstDayOfWeek == DayOfWeek.Sunday)
            {
                chkW1Sun.Attributes.Add("style", "display:inline;");
                chkW1Sun2.Attributes.Add("style", "display:none;");
            }
            else
            {
                chkW1Sun2.Attributes.Add("style", "display:inline;");
                chkW1Sun.Attributes.Add("style", "display:none;");
            }

            dpStartDate.DatePopupButton.ToolTip =
                Localization.GetString("DatePickerTooltip", LocalResourceFile);
            dpEndDate.DatePopupButton.ToolTip =
                Localization.GetString("DatePickerTooltip", LocalResourceFile);
            dpRecurEndDate.DatePopupButton.ToolTip =
                Localization.GetString("DatePickerTooltip", LocalResourceFile);
            dpY1Period.DatePopupButton.ToolTip =
                Localization.GetString("DatePickerTooltip", LocalResourceFile);

            tpEndTime.TimePopupButton.ToolTip =
                Localization.GetString("TimePickerTooltip", LocalResourceFile);
        }

        public void LoadEvent()
        {
            StorePrevPageInViewState();

            pnlRecurring.Visible = true;
            lblMaxRecurrences.Visible = false;
            if (!Settings.Allowreoccurring)
            {
                dpRecurEndDate.Enabled = false;
                cmbP1Period.Enabled = false;
                txtP1Every.Enabled = false;
                dpRecurEndDate.Visible = false;
                cmbP1Period.Visible = false;
                txtP1Every.Visible = false;
                pnlRecurring.Visible = false;
            }
            else
            {
                if (Settings.Maxrecurrences != "")
                {
                    lblMaxRecurrences.Visible = true;
                }
            }

            //Populate the timezone combobox (look up timezone translations based on currently set culture)
            cboTimeZone.DataBind(Settings.TimeZoneId);
            if (!Settings.EnableEventTimeZones)
            {
                cboTimeZone.Enabled = false;
            }

            if (_editRecur)
            {
                deleteButton.Attributes.Add(
                    "onClick",
                    "javascript:return confirm('" +
                    Localization.GetString("ConfirmEventSeriesDelete", LocalResourceFile) + "');");
                deleteButton.Text = Localization.GetString("deleteSeriesButton", LocalResourceFile);
                updateButton.Text = Localization.GetString("updateSeriesButton", LocalResourceFile);
                copyButton.Attributes.Add(
                    "onClick",
                    "javascript:return confirm('" +
                    Localization.GetString("ConfirmEventCopy", LocalResourceFile) + "');");
                copyButton.Text = Localization.GetString("copySeriesButton", LocalResourceFile);
            }
            else
            {
                deleteButton.Attributes.Add(
                    "onClick",
                    "javascript:return confirm('" +
                    Localization.GetString("ConfirmEventDelete", LocalResourceFile) + "');");
                deleteButton.Text = Localization.GetString("deleteButton", LocalResourceFile);
                updateButton.Text = Localization.GetString("updateButton", LocalResourceFile);
                copyButton.Attributes.Add(
                    "onClick",
                    "javascript:return confirm('" +
                    Localization.GetString("ConfirmEventCopy", LocalResourceFile) + "');");
                copyButton.Text = Localization.GetString("copyButton", LocalResourceFile);
            }
            lnkSelectedEmail.Attributes.Add(
                "onClick",
                "javascript:return confirm('" +
                Localization.GetString("ConfirmSendToAllSelected", LocalResourceFile) + "');");
            lnkSelectedDelete.Attributes.Add(
                "onClick",
                "javascript:return confirm('" +
                Localization.GetString("ConfirmDeleteSelected", LocalResourceFile) + "');");

            txtPayPalAccount.Text = Settings.Paypalaccount;

            var iInterval = int.Parse(Settings.Timeinterval);
            var currentDate = ModuleNow();
            var currentMinutes = currentDate.Minute;
            var remainder = currentMinutes % iInterval;
            if (remainder > 0)
            {
                currentDate = currentDate.AddMinutes(iInterval - remainder);
            }

            tpStartTime.SelectedDate = currentDate;
            tpEndTime.SelectedDate = currentDate.AddMinutes(iInterval);

            int pickerInterval = iInterval == 1440 ? 1439 : iInterval;
            tpStartTime.TimeView.Interval = new TimeSpan(0, pickerInterval, 0);
            tpEndTime.TimeView.Interval = new TimeSpan(0, pickerInterval, 0);


            // Can this event be moderated
            lblModerated.Visible = Settings.Moderateall;

            // Send Reminder Default
            chkReminder.Checked = Settings.Sendreminderdefault;

            // Populate description
            ftbDesktopText.Text = Settings.Templates.NewEventTemplate;

            // Set default validation value
            valNoEnrolees.MaximumValue = "9999";

            // Hide enrolment info by default
            lblEnrolledUsers.Visible = false;
            grdEnrollment.Visible = false;
            lnkSelectedDelete.Visible = false;
            lnkSelectedEmail.Visible = false;

            trAllowAnonEnroll.Visible = Settings.AllowAnonEnroll;

            // Populate enrollment email text boxes
            txtEventEmailSubject.Text = Settings.Templates.txtEditViewEmailSubject;
            txtEventEmailBody.Text = Settings.Templates.txtEditViewEmailBody;

            if (_itemID != -1)
            {
                // Edit Item Mode
                var objEvent = default(EventInfo);
                objEvent = _objCtlEvent.EventsGet(_itemID, ModuleId);

                var blEventSignup = false;
                if (Settings.Eventsignup)
                {
                    blEventSignup = objEvent.Signups;
                }
                else
                {
                    blEventSignup = false;
                }
                if (blEventSignup)
                {
                    pnlEnroll.Visible = true;
                    chkSignups.Visible = true;
                    lnkSelectedDelete.Visible = true;
                    lnkSelectedEmail.Visible = true;
                    if (_itemID != 0)
                    {
                        tblEventEmail.Attributes.Add("style", "display:block; width:100%");
                    }
                }

                // Check user has edit permissions to this event
                if (IsEventEditor(objEvent, false))
                { }
                else
                {
                    Response.Redirect(GetSocialNavigateUrl(), true);
                }

                // Create an object to consolidate master/single data into - use master object for common data
                var objEventData = default(EventRecurMasterInfo);
                objEventData =
                    _objCtlEventRecurMaster.EventsRecurMasterGet(objEvent.RecurMasterID, objEvent.ModuleID);

                // Hide recurrences section, disable timezone change if it is a recurring event
                // and we aren't editing the series
                if (objEventData.RRULE != "" && !_editRecur)
                {
                    pnlRecurring.Visible = false;
                    cboTimeZone.Enabled = false;
                }

                // If we are editing single item, populate with single event data
                if (!_editRecur)
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
                txtTitle.Text = objEventData.EventName;
                ftbDesktopText.Text = objEventData.EventDesc;

                // Set Dropdown to Original TimeZone w/ModuleID Settings TimeZone
                cboTimeZone.DataBind(objEvent.EventTimeZoneId);

                // Set dates/times
                dpStartDate.SelectedDate = objEventData.Dtstart.Date;
                dpEndDate.SelectedDate = objEventData.Dtstart.AddMinutes(intDuration).Date;
                dpRecurEndDate.SelectedDate = objEventData.Until;

                // Adjust Time not in DropDown Selection...
                var starttime = objEventData.Dtstart;
                if (starttime.Minute % iInterval > 0)
                {
                    starttime = objEventData.Dtstart.Date.AddMinutes(
                        Convert.ToDouble(objEventData.Dtstart.Hour * 60 + starttime.Minute -
                                         starttime.Minute % iInterval));
                }
                tpStartTime.SelectedDate = starttime;

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
                tpEndTime.SelectedDate = endtime;

                chkSignups.Checked = objEventData.Signups;
                chkAllowAnonEnroll.Checked = objEventData.AllowAnonEnroll;
                txtMaxEnrollment.Text = objEventData.MaxEnrollment.ToString();
                txtEnrolled.Text = objEventData.Enrolled.ToString();
                txtPayPalAccount.Text = objEventData.PayPalAccount;
                if (objEventData.EnrollType == "PAID")
                {
                    rblFree.Checked = false;
                    rblPaid.Checked = true;
                }
                else if (objEventData.EnrollType == "FREE")
                {
                    rblFree.Checked = true;
                    rblPaid.Checked = false;
                }

                txtEnrollFee.Text = string.Format("{0:F2}", objEventData.EnrollFee);
                lblTotalCurrency.Text = PortalSettings.Currency;

                if (blEventSignup)
                {
                    // Load Enrolled User Grid
                    BuildEnrolleeGrid(objEvent);
                }

                if (Information.IsNumeric(objEventData.EnrollRoleID))
                {
                    LoadEnrollRoles(objEventData.EnrollRoleID);
                    LoadNewEventEmailRoles(objEventData.EnrollRoleID);
                }
                else
                {
                    LoadEnrollRoles(-1);
                    LoadNewEventEmailRoles(-1);
                }

                LoadCategory(objEventData.Category);
                LoadLocation(objEventData.Location);
                LoadOwnerUsers(objEventData.OwnerID);

                cmbImportance.SelectedIndex =
                    Convert.ToInt16(GetCmbStatus(objEventData.Importance, "cmbImportance"));
                CreatedBy.Text = objEvent.CreatedBy;
                CreatedDate.Text = objEventData.CreatedDate.ToShortDateString();
                lblCreatedBy.Visible = true;
                CreatedBy.Visible = true;
                lblOn.Visible = true;
                CreatedDate.Visible = true;
                pnlAudit.Visible = true;

                var objEventRRULE = default(EventRRULEInfo);
                objEventRRULE = _objCtlEventRecurMaster.DecomposeRRULE(objEventData.RRULE, objEventData.Dtstart);
                var strRepeatType = _objCtlEventRecurMaster.RepeatType(objEventRRULE);

                switch (strRepeatType)
                {
                    case "N":
                        rblRepeatTypeN.Checked = true;
                        cmbP1Period.SelectedIndex = 0;
                        break;
                    //txtP1Every.Text = "0"
                    case "P1":
                        rblRepeatTypeP1.Checked = true;
                        chkReccuring.Checked = true;
                        cmbP1Period.SelectedIndex =
                            Convert.ToInt16(GetCmbStatus(objEventRRULE.Freq.Substring(0, 1), "cmbP1Period"));
                        txtP1Every.Text = objEventRRULE.Interval.ToString();
                        break;
                    case "W1":
                        rblRepeatTypeW1.Checked = true;
                        chkReccuring.Checked = true;
                        if (_culture.DateTimeFormat.FirstDayOfWeek == DayOfWeek.Sunday)
                        {
                            chkW1Sun.Checked = objEventRRULE.Su;
                        }
                        else
                        {
                            chkW1Sun2.Checked = objEventRRULE.Su;
                        }
                        chkW1Mon.Checked = objEventRRULE.Mo;
                        chkW1Tue.Checked = objEventRRULE.Tu;
                        chkW1Wed.Checked = objEventRRULE.We;
                        chkW1Thu.Checked = objEventRRULE.Th;
                        chkW1Fri.Checked = objEventRRULE.Fr;
                        chkW1Sat.Checked = objEventRRULE.Sa;
                        txtW1Every.Text = objEventRRULE.Interval.ToString();
                        break;
                    case "M1":
                        rblRepeatTypeM1.Checked = true;
                        rblRepeatTypeM.Checked = true;
                        chkReccuring.Checked = true;
                        txtMEvery.Text = objEventRRULE.Interval.ToString();
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
                        cmbM1Period.SelectedIndex = intPeriod;
                        if (intEvery == -1)
                        {
                            cmbM1Every.SelectedIndex = 4;
                        }
                        else
                        {
                            cmbM1Every.SelectedIndex = intEvery - 1;
                        }
                        break;
                    case "M2":
                        rblRepeatTypeM2.Checked = true;
                        rblRepeatTypeM.Checked = true;
                        chkReccuring.Checked = true;
                        cmbM2Period.SelectedIndex = objEventRRULE.ByMonthDay - 1;
                        txtMEvery.Text = objEventRRULE.Interval.ToString();
                        break;
                    case "Y1":
                        rblRepeatTypeY1.Checked = true;
                        chkReccuring.Checked = true;
                        var invCulture = CultureInfo.InvariantCulture;
                        var annualdate =
                            DateTime.ParseExact(
                                Strings.Format(objEventData.Dtstart, "yyyy") + "/" + objEventRRULE.ByMonth + "/" +
                                objEventRRULE.ByMonthDay, "yyyy/M/d", invCulture);
                        dpY1Period.SelectedDate = annualdate.Date;
                        break;
                }

                chkReminder.Checked = objEventData.SendReminder;
                txtReminder.Text = objEventData.Reminder;

                if (txtReminder.Text.Length == 0)
                {
                    txtReminder.Text = Settings.Templates.txtMessage;
                }

                txtSubject.Text = objEventData.Notify;
                if (txtSubject.Text.Length == 0)
                {
                    txtSubject.Text = Settings.Templates.txtSubject;
                }

                txtReminderFrom.Text = objEventData.ReminderFrom;
                txtEventEmailFrom.Text = objEventData.ReminderFrom;
                if (objEventData.ReminderTime < 0)
                {
                    txtReminderTime.Text = Convert.ToString(15.ToString());
                }
                else
                {
                    txtReminderTime.Text = objEventData.ReminderTime.ToString();
                }

                if (!ReferenceEquals(
                        ddlReminderTimeMeasurement.Items.FindByValue(objEventData.ReminderTimeMeasurement), null))
                {
                    ddlReminderTimeMeasurement.ClearSelection();
                    ddlReminderTimeMeasurement.Items.FindByValue(objEventData.ReminderTimeMeasurement).Selected =
                        true;
                }

                // Set DetailURL
                chkDetailPage.Checked = objEventData.DetailPage;
                URLDetail.Url = objEventData.DetailURL;


                // Set Image Control
                chkDisplayImage.Checked = objEventData.ImageDisplay;
                ctlURL.Url = objEventData.ImageURL;
                if (objEventData.ImageURL.StartsWith("FileID="))
                {
                    var fileId = int.Parse(objEventData.ImageURL.Substring(7));
                    var objFileInfo = FileManager.Instance.GetFile(fileId);
                    if (!ReferenceEquals(objFileInfo, null))
                    {
                        ctlURL.Url = objFileInfo.Folder + objFileInfo.FileName;
                    }
                    else
                    {
                        chkDisplayImage.Checked = false;
                    }
                }

                if ((objEventData.ImageWidth != 0) & (objEventData.ImageWidth != -1))
                {
                    txtWidth.Text = objEventData.ImageWidth.ToString();
                }

                if ((objEventData.ImageHeight != 0) & (objEventData.ImageHeight != -1))
                {
                    txtHeight.Text = objEventData.ImageHeight.ToString();
                }
                txtCustomField1.Text = objEventData.CustomField1;
                txtCustomField2.Text = objEventData.CustomField2;
                chkEnrollListView.Checked = objEventData.EnrollListView;
                chkDisplayEndDate.Checked = objEventData.DisplayEndDate;
                chkAllDayEvent.Checked = objEventData.AllDayEvent;
                ftbSummary.Text = objEventData.Summary;

                if (blEventSignup)
                {
                    LoadRegUsers();
                }
            }
            else
            {
                dpStartDate.SelectedDate = SelectedDate.Date;
                dpEndDate.SelectedDate = SelectedDate.Date;
                txtEnrollFee.Text = string.Format("{0:F2}", 0.0);
                lblTotalCurrency.Text = PortalSettings.Currency;
                dpRecurEndDate.SelectedDate = SelectedDate.AddDays(1);
                dpY1Period.SelectedDate = SelectedDate.Date;
                chkEnrollListView.Checked = Settings.Eventdefaultenrollview;
                chkDisplayEndDate.Checked = true;
                chkAllDayEvent.Checked = false;

                // Do not default recurrance end date
                // Force user to key/select
                LoadEnrollRoles(-1);
                LoadNewEventEmailRoles(-1);
                LoadCategory();
                LoadLocation(-1);
                LoadOwnerUsers(UserId);
                pnlAudit.Visible = false;
                deleteButton.Visible = false;
            }

            if (_itemID == -1 || _editRecur)
            {
                divAddUser.Visible = false;
                divNoEnrolees.Visible = false;
            }

            var errorminutes = Localization.GetString("invalidReminderMinutes", LocalResourceFile);
            var errorhours = Localization.GetString("invalidReminderHours", LocalResourceFile);
            var errordays = Localization.GetString("invalidReminderDays", LocalResourceFile);
            ddlReminderTimeMeasurement.Attributes.Add(
                "onchange",
                "valRemTime('" + valReminderTime.ClientID + "','" + valReminderTime2.ClientID + "','" +
                valReminderTime.ValidationGroup + "','" + ddlReminderTimeMeasurement.ClientID + "','" +
                errorminutes + "','" + errorhours + "','" + errordays + "');");
            switch (ddlReminderTimeMeasurement.SelectedValue)
            {
                case "m":
                    valReminderTime.ErrorMessage = errorminutes;
                    valReminderTime.MinimumValue = "15";
                    valReminderTime.MaximumValue = "60";
                    break;
                case "h":
                    valReminderTime.ErrorMessage = errorhours;
                    valReminderTime.MinimumValue = "1";
                    valReminderTime.MaximumValue = "24";
                    break;
                case "d":
                    valReminderTime.ErrorMessage = errordays;
                    valReminderTime.MinimumValue = "1";
                    valReminderTime.MaximumValue = "30";
                    break;
            }
            valReminderTime2.ErrorMessage = valReminderTime.ErrorMessage;

            if (txtPayPalAccount.Text.Length == 0)
            {
                txtPayPalAccount.Text = Settings.Paypalaccount;
            }

            trCustomField1.Visible = Settings.EventsCustomField1;
            trCustomField2.Visible = Settings.EventsCustomField2;

            trTimeZone.Visible = Settings.Tzdisplay;

            chkDetailPage.Attributes.Add(
                "onclick",
                "javascript:showTbl('" + chkDetailPage.ClientID + "','" + tblDetailPageDetail.ClientID +
                "');");
            chkReminder.Attributes.Add(
                "onclick",
                "javascript:showTbl('" + chkReminder.ClientID + "','" + tblReminderDetail.ClientID +
                "');");
            chkDisplayImage.Attributes.Add(
                "onclick",
                "javascript:showTbl('" + chkDisplayImage.ClientID + "','" + tblImageURL.ClientID +
                "');");
            chkReccuring.Attributes.Add(
                "onclick",
                "javascript:if (this.checked == true) dnn.dom.getById('" + rblRepeatTypeP1.ClientID +
                "').checked = true; else dnn.dom.getById('" + rblRepeatTypeN.ClientID +
                "').checked = true;showhideTbls('" + RecurTableDisplayType + "','" + chkReccuring.ClientID +
                "','" + tblRecurringDetails.ClientID + "','" + rblRepeatTypeP1.ClientID + "','" +
                tblDetailP1.ClientID + "','" + rblRepeatTypeW1.ClientID + "','" +
                tblDetailW1.ClientID + "','" + rblRepeatTypeM.ClientID + "','" +
                tblDetailM1.ClientID + "','" + rblRepeatTypeY1.ClientID + "','" +
                tblDetailY1.ClientID + "');");
            if (Settings.Eventsignup)
            {
                chkEventEmailChk.Attributes.Add(
                    "onclick",
                    "javascript:showhideChk2('" + chkEventEmailChk.ClientID + "','" +
                    tblEventEmailRoleDetail.ClientID + "','" + chkSignups.ClientID + "','" +
                    tblEventEmail.ClientID + "');");
            }
            else
            {
                chkEventEmailChk.Attributes.Add(
                    "onclick",
                    "javascript:showhideChk2('" + chkEventEmailChk.ClientID + "','" +
                    tblEventEmailRoleDetail.ClientID + "','" + chkEventEmailChk.ClientID + "','" +
                    tblEventEmail.ClientID + "');");
            }
            rblRepeatTypeP1.Attributes.Add(
                "onclick",
                "javascript:showhideTbls('" + RecurTableDisplayType + "','" + chkReccuring.ClientID + "','" +
                tblRecurringDetails.ClientID + "','" + rblRepeatTypeP1.ClientID + "','" +
                tblDetailP1.ClientID + "','" + rblRepeatTypeW1.ClientID + "','" +
                tblDetailW1.ClientID + "','" + rblRepeatTypeM.ClientID + "','" +
                tblDetailM1.ClientID + "','" + rblRepeatTypeY1.ClientID + "','" +
                tblDetailY1.ClientID + "');");
            rblRepeatTypeW1.Attributes.Add(
                "onclick",
                "javascript:showhideTbls('" + RecurTableDisplayType + "','" + chkReccuring.ClientID + "','" +
                tblRecurringDetails.ClientID + "','" + rblRepeatTypeP1.ClientID + "','" +
                tblDetailP1.ClientID + "','" + rblRepeatTypeW1.ClientID + "','" +
                tblDetailW1.ClientID + "','" + rblRepeatTypeM.ClientID + "','" +
                tblDetailM1.ClientID + "','" + rblRepeatTypeY1.ClientID + "','" +
                tblDetailY1.ClientID + "');");
            rblRepeatTypeM.Attributes.Add(
                "onclick",
                "javascript:showhideTbls('" + RecurTableDisplayType + "','" + chkReccuring.ClientID + "','" +
                tblRecurringDetails.ClientID + "','" + rblRepeatTypeP1.ClientID + "','" +
                tblDetailP1.ClientID + "','" + rblRepeatTypeW1.ClientID + "','" +
                tblDetailW1.ClientID + "','" + rblRepeatTypeM.ClientID + "','" +
                tblDetailM1.ClientID + "','" + rblRepeatTypeY1.ClientID + "','" +
                tblDetailY1.ClientID + "');if (this.checked == true) dnn.dom.getById('" +
                rblRepeatTypeM1.ClientID + "').checked = true;");
            rblRepeatTypeY1.Attributes.Add(
                "onclick",
                "javascript:showhideTbls('" + RecurTableDisplayType + "','" + chkReccuring.ClientID + "','" +
                tblRecurringDetails.ClientID + "','" + rblRepeatTypeP1.ClientID + "','" +
                tblDetailP1.ClientID + "','" + rblRepeatTypeW1.ClientID + "','" +
                tblDetailW1.ClientID + "','" + rblRepeatTypeM.ClientID + "','" +
                tblDetailM1.ClientID + "','" + rblRepeatTypeY1.ClientID + "','" +
                tblDetailY1.ClientID + "');");
            btnCopyStartdate.Attributes.Add(
                "onclick",
                string.Format("javascript:CopyStartDateToEnddate('{0}','{1}','{2}','{3}','{4}');",
                              dpStartDate.ClientID, dpEndDate.ClientID, tpStartTime.ClientID,
                              tpEndTime.ClientID, chkAllDayEvent.ClientID));
            chkAllDayEvent.Attributes.Add(
                "onclick",
                "javascript:showTimes('" + chkAllDayEvent.ClientID + "','" + divStartTime.ClientID +
                "','" + divEndTime.ClientID + "');");
        }

        public long GetCmbStatus(object value, string cmbDropDown)
        {
            var iIndex = 0;
            var oDropDown = default(DropDownList);

            oDropDown = (DropDownList) FindControl(cmbDropDown);
            for (iIndex = 0; iIndex <= oDropDown.Items.Count - 1; iIndex++)
            {
                if (oDropDown.Items[iIndex].Text == Convert.ToString(value))
                {
                    return iIndex;
                }
            }
            return iIndex;
        }

        public void LoadEnrollRoles(int roleID)
        {
            var objRoles = new RoleController();
            ddEnrollRoles.DataSource = objRoles.GetPortalRoles(PortalId);
            ddEnrollRoles.DataTextField = "RoleName";
            ddEnrollRoles.DataValueField = "RoleID";
            ddEnrollRoles.DataBind();
            //"<None Specified>"
            ddEnrollRoles.Items.Insert(
                0, new ListItem(Localization.GetString("None", LocalResourceFile), "-1"));
            if (roleID == 0)
            {
                ddEnrollRoles.Items.FindByValue("0").Selected = true;
            }
            else if (roleID > 0)
            {
                if (ReferenceEquals(ddEnrollRoles.Items.FindByValue(Convert.ToString(roleID)), null))
                {
                    ddEnrollRoles.Items.Insert(
                        0,
                        new ListItem(Localization.GetString("EnrolleeRoleDeleted", LocalResourceFile),
                                     roleID.ToString()));
                }
                ddEnrollRoles.Items.FindByValue(Convert.ToString(roleID)).Selected = true;
            }
        }

        public void LoadNewEventEmailRoles(int roleID)
        {
            var objRoles = new RoleController();
            ddEventEmailRoles.DataSource = objRoles.GetPortalRoles(PortalId);
            ddEventEmailRoles.DataTextField = "RoleName";
            ddEventEmailRoles.DataValueField = "RoleID";
            ddEventEmailRoles.DataBind();
            if (roleID < 0 || ReferenceEquals(ddEventEmailRoles.Items.FindByValue(Convert.ToString(roleID)), null))
            {
                try
                {
                    ddEventEmailRoles.Items.FindByValue(PortalSettings.RegisteredRoleId.ToString()).Selected =
                        true;
                }
                catch
                { }
            }
            else
            {
                ddEventEmailRoles.Items.FindByValue(Convert.ToString(roleID)).Selected = true;
            }
        }

        public void LoadCategory(int category = default(int))
        {
            var objCntCategories = new EventCategoryController();
            var tmpCategories = objCntCategories.EventsCategoryList(PortalId);
            var objCategories = new ArrayList();
            if ((Settings.Enablecategories == EventModuleSettings.DisplayCategories.DoNotDisplay) &
                (Settings.ModuleCategoriesSelected == EventModuleSettings.CategoriesSelected.Some)
                || Settings.Restrictcategories)
            {
                foreach (EventCategoryInfo objCategory in tmpCategories)
                {
                    foreach (int moduleCategory in Settings.ModuleCategoryIDs)
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
            cmbCategory.DataSource = objCategories;
            cmbCategory.DataTextField = "CategoryName";
            cmbCategory.DataValueField = "Category";
            cmbCategory.DataBind();

            // Do we need to add None
            if (!(Settings.Enablecategories == EventModuleSettings.DisplayCategories.DoNotDisplay) |
                (Settings.ModuleCategoriesSelected == EventModuleSettings.CategoriesSelected.All)
                && Settings.Restrictcategories == false)
            {
                cmbCategory.Items.Insert(
                    0, new ListItem(Localization.GetString("None", LocalResourceFile), "-1"));
            }

            // Select the appropriate row
            if (Settings.ModuleCategoriesSelected == EventModuleSettings.CategoriesSelected.All)
            {
                cmbCategory.ClearSelection();
                cmbCategory.Items[0].Selected = true;
            }

            if (category > 0)
            {
                cmbCategory.ClearSelection();
                cmbCategory.Items.FindByValue(Convert.ToString(category)).Selected = true;
            }
            else if (!(Settings.Enablecategories == EventModuleSettings.DisplayCategories.DoNotDisplay) &
                     (Settings.ModuleCategoriesSelected == EventModuleSettings.CategoriesSelected.Some))
            {
                cmbCategory.ClearSelection();
                cmbCategory.Items.FindByValue(Convert.ToString(Settings.ModuleCategoryIDs[0])).Selected =
                    true;
            }
        }

        public void LoadLocation(int location)
        {
            var objCntLocation = new EventLocationController();
            cmbLocation.DataSource = objCntLocation.EventsLocationList(PortalId);
            cmbLocation.DataTextField = "LocationName";
            cmbLocation.DataValueField = "Location";
            cmbLocation.DataBind();
            //"<None Specified>"
            cmbLocation.Items.Insert(
                0, new ListItem(Localization.GetString("None", LocalResourceFile), "-1"));
            if (location > 0)
            {
                cmbLocation.Items.FindByValue(Convert.ToString(location)).Selected = true;
            }
        }

        private void LoadOwnerUsers(int ownerID)
        {
            var objCollModulePermission = default(ModulePermissionCollection);
            objCollModulePermission = ModulePermissionController.GetModulePermissions(ModuleId, TabId);
            var objModulePermission = default(ModulePermissionInfo);

            // To cope with host users or someone who is no longer an editor!!
            var objEventModuleEditor = new EventUser();
            objEventModuleEditor.UserID = ownerID;
            LoadSingleUser(objEventModuleEditor, _lstOwnerUsers);

            if (IsModerator() && Settings.Ownerchangeallowed ||
                PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName))
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
                                objCtlRole.GetUsersByRoleName(PortalId, objModulePermission.RoleName);
                            foreach (UserInfo objUser in lstRoleUsers)
                            {
                                objEventModuleEditor = new EventUser();
                                objEventModuleEditor.UserID = objUser.UserID;
                                objEventModuleEditor.DisplayName = objUser.DisplayName;
                                LoadSingleUser(objEventModuleEditor, _lstOwnerUsers);
                            }
                        }
                        else
                        {
                            objEventModuleEditor = new EventUser();
                            objEventModuleEditor.UserID = objModulePermission.UserID;
                            objEventModuleEditor.DisplayName = objModulePermission.DisplayName;
                            LoadSingleUser(objEventModuleEditor, _lstOwnerUsers);
                        }
                    }
                }
            }
            _lstOwnerUsers.Sort(new UserListSort());

            cmbOwner.DataSource = _lstOwnerUsers;
            cmbOwner.DataTextField = "DisplayName";
            cmbOwner.DataValueField = "UserID";
            cmbOwner.DataBind();
            cmbOwner.Items.FindByValue(Convert.ToString(ownerID)).Selected = true;
        }

        private void LoadRegUsers()
        {
            grdAddUser.RefreshGrid();
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
                    var objUser = objCtlUser.GetUser(PortalId, objEventUser.UserID);
                    if (!ReferenceEquals(objUser, null))
                    {
                        objEventUser.DisplayName = objUser.DisplayName;
                    }
                    else
                    {
                        objEventUser.DisplayName = Localization.GetString("OwnerDeleted", LocalResourceFile);
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
            if (_itemID > 0)
            {
                objEvent = _objCtlEvent.EventsGet(_itemID, ModuleId);
            }
            else
            {
                return;
            }

            var objEventEmailInfo = new EventEmailInfo();
            var objEventEmail = new EventEmails(PortalId, ModuleId, LocalResourceFile,
                                                ((PageBase) Page).PageCulture.Name);
            objEventEmailInfo.TxtEmailSubject = txtEventEmailSubject.Text;
            objEventEmailInfo.TxtEmailBody = txtEventEmailBody.Text;
            objEventEmailInfo.TxtEmailFrom = txtEventEmailFrom.Text;
            foreach (DataGridItem tempLoopVar_item in grdEnrollment.Items)
            {
                item = tempLoopVar_item;
                if (((CheckBox) item.FindControl("chkSelect")).Checked || selected == false)
                {
                    objEnroll = _objCtlEventSignups.EventsSignupsGet(
                        Convert.ToInt32(grdEnrollment.DataKeys[item.ItemIndex]), ModuleId,
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
            if (!Page.IsValid)
            {
                return;
            }

            var objSecurity = new PortalSecurity();
            var tStartTime = default(DateTime);
            var tEndTime = default(DateTime);
            var tRecurEndDate = default(DateTime);
            var objEventInfoHelper = new EventInfoHelper(ModuleId, TabId, PortalId, Settings);

            // Make EndDate = StartDate if no recurring dates
            if (rblRepeatTypeN.Checked)
            {
                dpRecurEndDate.SelectedDate = ConvertDateStringstoDatetime(dpStartDate.SelectedDate.ToString(), "00:00").Date;
            }

            valRequiredRecurEndDate.Validate();

            // Make sure date formatted correctly
            if (chkAllDayEvent.Checked)
            {
                tStartTime = ConvertDateStringstoDatetime(dpStartDate.SelectedDate.ToString(), "00:00");
                tEndTime = Convert.ToDateTime(
                    ConvertDateStringstoDatetime(dpEndDate.SelectedDate.ToString(), "00:00")
                        .AddMinutes(1439));
            }
            else
            {
                tStartTime =
                    ConvertDateStringstoDatetime(dpStartDate.SelectedDate.ToString(),
                                                      Convert.ToString(
                                                          Convert.ToDateTime(tpStartTime.SelectedDate)
                                                                 .ToString("HH:mm", CultureInfo.InvariantCulture)));
                tEndTime = ConvertDateStringstoDatetime(dpEndDate.SelectedDate.ToString(),
                                                             Convert.ToString(
                                                                 Convert.ToDateTime(tpEndTime.SelectedDate)
                                                                        .ToString(
                                                                            "HH:mm", CultureInfo.InvariantCulture)));
            }

            if (tEndTime < tStartTime && !chkAllDayEvent.Checked)
            {
                valValidEndTime.ErrorMessage = Localization.GetString("valValidEndTime", LocalResourceFile);
                valValidEndTime.IsValid = false;
                valValidEndTime.Visible = true;
                return;
            }

            tRecurEndDate = Convert.ToDateTime(dpRecurEndDate.SelectedDate);

            if (rblRepeatTypeP1.Checked)
            {
                valP1Every.Validate();
                valP1Every2.Validate();
                if (valP1Every.IsValid == false || valP1Every2.IsValid == false)
                {
                    return;
                }
            }

            if (rblRepeatTypeW1.Checked)
            {
                valW1Day.Validate();
                valW1Day2.Validate();
                if (chkW1Sun.Checked == false && chkW1Sun2.Checked == false &&
                    chkW1Mon.Checked == false && chkW1Tue.Checked == false &&
                    chkW1Wed.Checked == false && chkW1Thu.Checked == false &&
                    chkW1Fri.Checked == false && chkW1Sat.Checked == false)
                {
                    valW1Day3.ErrorMessage = Localization.GetString("valW1Day3", LocalResourceFile);
                    valW1Day3.Text = Localization.GetString("valW1Day3", LocalResourceFile);
                    valW1Day3.IsValid = false;
                    valW1Day3.Visible = true;
                    return;
                }
                if (valW1Day.IsValid == false || valW1Day2.IsValid == false)
                {
                    return;
                }
            }

            if (rblRepeatTypeM.Checked && rblRepeatTypeM2.Checked)
            {
                valM2Every.Validate();
                valM2Every2.Validate();
                if (valM2Every.IsValid == false || valM2Every2.IsValid == false)
                {
                    return;
                }
            }
            // If Annual Recurrence, Check date
            if (rblRepeatTypeY1.Checked)
            {
                valRequiredYearEventDate.Validate();
                valValidYearEventDate.Validate();
                if (valRequiredYearEventDate.IsValid == false || valValidYearEventDate.IsValid == false)
                {
                    return;
                }
            }

            if (Settings.Expireevents != ""
                && !_editRecur)
            {
                if (tStartTime < DateTime.Now.AddDays(-Convert.ToInt32(Settings.Expireevents)))
                {
                    valValidStartDate2.IsValid = false;
                    valValidStartDate2.Visible = true;
                    valValidStartDate2.Text =
                        string.Format(Localization.GetString("valValidStartDate2", LocalResourceFile),
                                      Convert.ToInt32(Settings.Expireevents));
                    valValidStartDate2.ErrorMessage =
                        string.Format(Localization.GetString("valValidStartDate2", LocalResourceFile),
                                      Convert.ToInt32(Settings.Expireevents));
                    return;
                }
            }

            double duration = 0;
            duration = tEndTime.Subtract(tStartTime).TotalMinutes;

            if (rblPaid.Checked)
            {
                if (Information.IsNumeric(txtEnrollFee.Text))
                {
                    // ReSharper disable CompareOfFloatsByEqualityOperator
                    if (Convert.ToDouble(txtEnrollFee.Text) == 0.0)
                    {
                        // ReSharper restore CompareOfFloatsByEqualityOperator
                        valBadFee.IsValid = false;
                        valBadFee.Visible = true;
                        return;
                    }
                }
                else
                {
                    valBadFee.IsValid = false;
                    valBadFee.Visible = true;
                    return;
                }
                if (txtPayPalAccount.Text.Trim() == string.Empty)
                {
                    valPayPalAccount.IsValid = false;
                    valPayPalAccount.Visible = true;
                    return;
                }
            }

            //Check valid Reminder Time
            if (chkReminder.Checked)
            {
                var remtime = Convert.ToInt32(txtReminderTime.Text);
                switch (ddlReminderTimeMeasurement.SelectedValue)
                {
                    case "m":
                        if ((remtime < 15) | (remtime > 60))
                        {
                            valReminderTime2.IsValid = false;
                            valReminderTime2.Visible = true;
                            return;
                        }
                        break;
                    case "h":
                        if ((remtime < 1) | (remtime > 24))
                        {
                            valReminderTime2.IsValid = false;
                            valReminderTime2.Visible = true;
                            return;
                        }
                        break;
                    case "d":
                        if ((remtime < 1) | (remtime > 30))
                        {
                            valReminderTime2.IsValid = false;
                            valReminderTime2.Visible = true;
                            return;
                        }
                        break;
                }
            }

            if (chkSignups.Checked)
            {
                valMaxEnrollment.Validate();
                if (!valMaxEnrollment.IsValid)
                {
                    return;
                }
            }

            valW1Day.Visible = false;
            valW1Day2.Visible = false;
            valW1Day3.Visible = false;
            valConflict.Visible = false;
            valLocationConflict.Visible = false;
            valPayPalAccount.Visible = false;
            valBadFee.Visible = false;
            valValidRecurStartDate.Visible = false;
            valNoEnrolees.Visible = false;
            valMaxEnrollment.Visible = false;

            // Everythings Cool, Update Database
            var objEvent = default(EventInfo);
            var objEventRecurMaster = new EventRecurMasterInfo();
            var objEventRMSave = new EventRecurMasterInfo();

            // Get Current Event, if <> 0
            if (processItem > 0)
            {
                objEvent = _objCtlEvent.EventsGet(processItem, ModuleId);
                objEventRecurMaster =
                    _objCtlEventRecurMaster.EventsRecurMasterGet(objEvent.RecurMasterID, objEvent.ModuleID);
                objEventRMSave =
                    _objCtlEventRecurMaster.EventsRecurMasterGet(objEvent.RecurMasterID, objEvent.ModuleID);
                if (_editRecur)
                {
                    _lstEvents =
                        _objCtlEvent.EventsGetRecurrences(objEventRecurMaster.RecurMasterID,
                                                               objEventRecurMaster.ModuleID);
                }
                else
                {
                    _lstEvents.Add(objEvent);
                }
            }
            var intDuration = 0;
            objEventRecurMaster.Dtstart = tStartTime;
            objEventRecurMaster.Duration = Convert.ToString(Convert.ToString(duration) + "M");
            intDuration = Convert.ToInt32(duration);
            objEventRecurMaster.Until = tRecurEndDate;
            objEventRecurMaster.UpdatedByID = UserId;
            objEventRecurMaster.OwnerID = int.Parse(cmbOwner.SelectedValue);
            if (processItem < 0)
            {
                objEventRecurMaster.RecurMasterID = -1;
                objEventRecurMaster.CreatedByID = UserId;
                objEventRecurMaster.ModuleID = ModuleId;
                objEventRecurMaster.PortalID = PortalId;
                objEventRecurMaster.CultureName = Thread.CurrentThread.CurrentCulture.Name;
                objEventRecurMaster.JournalItem = false;
            }
            // Filter text for non-admins and moderators
            if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName))
            {
                objEventRecurMaster.EventName = txtTitle.Text;
                objEventRecurMaster.EventDesc = Convert.ToString(ftbDesktopText.Text);
                objEventRecurMaster.CustomField1 = txtCustomField1.Text;
                objEventRecurMaster.CustomField2 = txtCustomField2.Text;
                objEventRecurMaster.Notify = txtSubject.Text;
                objEventRecurMaster.Reminder = txtReminder.Text;
                objEventRecurMaster.Summary = Convert.ToString(ftbSummary.Text);
            }
            else if (IsModerator())
            {
                objEventRecurMaster.EventName =
                    objSecurity.InputFilter(txtTitle.Text, PortalSecurity.FilterFlag.NoScripting);
                objEventRecurMaster.EventDesc = Convert.ToString(ftbDesktopText.Text);
                objEventRecurMaster.CustomField1 =
                    objSecurity.InputFilter(txtCustomField1.Text, PortalSecurity.FilterFlag.NoScripting);
                objEventRecurMaster.CustomField2 =
                    objSecurity.InputFilter(txtCustomField2.Text, PortalSecurity.FilterFlag.NoScripting);
                objEventRecurMaster.Notify =
                    objSecurity.InputFilter(txtSubject.Text, PortalSecurity.FilterFlag.NoScripting);
                objEventRecurMaster.Reminder =
                    objSecurity.InputFilter(txtReminder.Text, PortalSecurity.FilterFlag.NoScripting);
                objEventRecurMaster.Summary = Convert.ToString(ftbSummary.Text);
            }
            else
            {
                objEventRecurMaster.EventName =
                    objSecurity.InputFilter(txtTitle.Text, PortalSecurity.FilterFlag.NoScripting);
                objEventRecurMaster.EventDesc =
                    objSecurity.InputFilter(Convert.ToString(ftbDesktopText.Text),
                                            PortalSecurity.FilterFlag.NoScripting);
                objEventRecurMaster.CustomField1 =
                    objSecurity.InputFilter(txtCustomField1.Text, PortalSecurity.FilterFlag.NoScripting);
                objEventRecurMaster.CustomField2 =
                    objSecurity.InputFilter(txtCustomField2.Text, PortalSecurity.FilterFlag.NoScripting);
                objEventRecurMaster.Notify =
                    objSecurity.InputFilter(txtSubject.Text, PortalSecurity.FilterFlag.NoScripting);
                objEventRecurMaster.Reminder =
                    objSecurity.InputFilter(txtReminder.Text, PortalSecurity.FilterFlag.NoScripting);
                objEventRecurMaster.Summary =
                    objSecurity.InputFilter(Convert.ToString(ftbSummary.Text),
                                            PortalSecurity.FilterFlag.NoScripting);
            }

            // If New Event
            if (processItem < 0)
            {
                // If Moderator turned on, set approve=false
                if (Settings.Moderateall)
                {
                    objEventRecurMaster.Approved = false;
                }
                else
                {
                    objEventRecurMaster.Approved = true;
                }
            }

            // Reset Approved, if Moderate All option is on
            if (Settings.Moderateall &&
                objEventRecurMaster.Approved)
            {
                objEventRecurMaster.Approved = false;
            }

            // If Admin or Moderator, automatically approve event
            if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) || IsModerator())
            {
                objEventRecurMaster.Approved = true;
            }

            objEventRecurMaster.Importance =
                (EventRecurMasterInfo.Priority) int.Parse(cmbImportance.SelectedItem.Value);

            objEventRecurMaster.Signups = chkSignups.Checked;
            objEventRecurMaster.AllowAnonEnroll = chkAllowAnonEnroll.Checked;
            if (rblFree.Checked)
            {
                objEventRecurMaster.EnrollType = "FREE";
            }
            else if (rblPaid.Checked)
            {
                objEventRecurMaster.EnrollType = "PAID";
            }

            objEventRecurMaster.PayPalAccount = txtPayPalAccount.Text;
            objEventRecurMaster.EnrollFee = decimal.Parse(txtEnrollFee.Text);
            objEventRecurMaster.MaxEnrollment = Convert.ToInt32(txtMaxEnrollment.Text);

            if (int.Parse(ddEnrollRoles.SelectedValue) != -1)
            {
                objEventRecurMaster.EnrollRoleID = int.Parse(ddEnrollRoles.SelectedItem.Value);
            }
            else
            {
                objEventRecurMaster.EnrollRoleID = -1;
            }

            // Update Detail Page setting in the database
            if (chkDetailPage.Checked && URLDetail.Url != "")
            {
                objEventRecurMaster.DetailPage = true;
                objEventRecurMaster.DetailURL = Convert.ToString(URLDetail.Url);
                objEventRecurMaster.DetailNewWin = Convert.ToBoolean(URLDetail.NewWindow);
            }
            else
            {
                objEventRecurMaster.DetailPage = false;
            }

            // Update Image settings in the database
            if (chkDisplayImage.Checked)
            {
                objEventRecurMaster.ImageDisplay = true;
                if (ctlURL.UrlType == "F")
                {
                    if (ctlURL.Url.StartsWith("FileID="))
                    {
                        var fileId = int.Parse(Convert.ToString(ctlURL.Url.Substring(7)));
                        var objFileInfo = FileManager.Instance.GetFile(fileId);
                        if (txtWidth.Text == string.Empty || txtWidth.Text == 0.ToString())
                        {
                            txtWidth.Text = Convert.ToString(objFileInfo.Width.ToString());
                        }
                        if (txtHeight.Text == "" || txtHeight.Text == 0.ToString())
                        {
                            txtHeight.Text = Convert.ToString(objFileInfo.Height.ToString());
                        }
                    }
                }
                objEventRecurMaster.ImageURL = Convert.ToString(ctlURL.Url);
                objEventRecurMaster.ImageType = Convert.ToString(ctlURL.UrlType);
            }
            else
            {
                objEventRecurMaster.ImageDisplay = false;
            }

            objEventRecurMaster.ImageWidth = txtWidth.Text == "" ? 0 : int.Parse(txtWidth.Text);

            objEventRecurMaster.ImageHeight = txtHeight.Text == "" ? 0 : int.Parse(txtHeight.Text);

            objEventRecurMaster.Category = int.Parse(cmbCategory.SelectedValue);
            objEventRecurMaster.Location = int.Parse(cmbLocation.SelectedValue);

            objEventRecurMaster.SendReminder = chkReminder.Checked;
            objEventRecurMaster.ReminderTime = int.Parse(txtReminderTime.Text);
            objEventRecurMaster.ReminderTimeMeasurement = ddlReminderTimeMeasurement.SelectedValue;
            objEventRecurMaster.ReminderFrom = txtReminderFrom.Text;

            objEventRecurMaster.EnrollListView = chkEnrollListView.Checked;
            objEventRecurMaster.DisplayEndDate = chkDisplayEndDate.Checked;
            objEventRecurMaster.AllDayEvent = chkAllDayEvent.Checked;
            objEventRecurMaster.EventTimeZoneId = cboTimeZone.SelectedValue;
            objEventRecurMaster.SocialGroupID = GetUrlGroupId();
            objEventRecurMaster.SocialUserID = GetUrlUserId();


            // If it is possible we are edititng a recurring event create RRULE
            if (processItem == 0 || _editRecur || !_editRecur && objEventRMSave.RRULE == "")
            {
                objEventRecurMaster = CreateEventRRULE(objEventRecurMaster);
                if (rblRepeatTypeN.Checked)
                {
                    objEventRecurMaster.Until = objEventRecurMaster.Dtstart.Date;
                }
            }

            // If editing single occurence of recurring event & start date > last date, error
            if (processItem > 0 && objEventRMSave.RRULE != "" && !_editRecur)
            {
                if (tStartTime.Date > objEventRMSave.Until.Date)
                {
                    valValidRecurStartDate.IsValid = false;
                    valValidRecurStartDate.Visible = true;
                    return;
                }
                if (tStartTime.Date < objEventRMSave.Dtstart.Date)
                {
                    valValidRecurStartDate2.IsValid = false;
                    valValidRecurStartDate2.Visible = true;
                    return;
                }
            }

            // If new Event or Recurring event then check for new instances
            if (processItem < 0 ||
                string.IsNullOrEmpty(objEventRMSave.RRULE) && objEventRecurMaster.RRULE != "" && !_editRecur || _editRecur)
            {
                var lstEventsNew = default(ArrayList);
                lstEventsNew =
                    _objCtlEventRecurMaster.CreateEventRecurrences(objEventRecurMaster, intDuration,
                                                                        Settings.Maxrecurrences);
                _lstEvents = CompareOldNewEvents(_lstEvents, lstEventsNew);

                if (lstEventsNew.Count == 0)
                {
                    // Last error!!
                    valValidRecurEndDate2.IsValid = false;
                    valValidRecurEndDate2.Visible = true;
                    return;
                }
            }

            foreach (EventInfo tempLoopVar_objEvent in _lstEvents)
            {
                objEvent = tempLoopVar_objEvent;
                if (objEvent.EventID > 0 && objEvent.UpdateStatus != "Delete")
                {
                    objEvent.UpdateStatus = "Match";
                    var objEventSave = objEvent.Clone();
                    if (_editRecur && objEvent.EventTimeBegin.ToShortTimeString() ==
                        objEventRMSave.Dtstart.ToShortTimeString())
                    {
                        objEvent.EventTimeBegin =
                            ConvertDateStringstoDatetime(objEvent.EventTimeBegin.ToShortDateString(),
                                                              Strings.Format(objEventRecurMaster.Dtstart, "HH:mm"));
                        if (tRecurEndDate.Date < objEvent.EventTimeBegin.Date)
                        {
                            tRecurEndDate = objEvent.EventTimeBegin.Date.AddDays(30);
                        }
                    }

                    if (_editRecur && Convert.ToString(objEvent.Duration) + "M" == objEventRMSave.Duration ||
                        !_editRecur)
                    {
                        objEvent.Duration = intDuration;
                    }

                    if (!_editRecur)
                    {
                        objEvent.EventTimeBegin = objEventRecurMaster.Dtstart;
                        if (tRecurEndDate.Date < objEvent.EventTimeBegin.Date)
                        {
                            tRecurEndDate = objEvent.EventTimeBegin.Date.AddDays(30);
                        }
                        objEvent.Duration = intDuration;
                    }

                    if (_editRecur && objEvent.EventName == objEventRMSave.EventName || !_editRecur)
                    {
                        objEvent.EventName = objEventRecurMaster.EventName;
                    }

                    if (_editRecur && objEvent.EventDesc == objEventRMSave.EventDesc || !_editRecur)
                    {
                        objEvent.EventDesc = objEventRecurMaster.EventDesc;
                    }

                    if (_editRecur && (int) objEvent.Importance == (int) objEventRMSave.Importance ||
                        !_editRecur)
                    {
                        objEvent.Importance = (EventInfo.Priority) objEventRecurMaster.Importance;
                    }

                    if (_editRecur && objEvent.Signups == objEventRMSave.Signups || !_editRecur)
                    {
                        objEvent.Signups = objEventRecurMaster.Signups;
                    }

                    if (_editRecur && objEvent.JournalItem == objEventRMSave.JournalItem || !_editRecur)
                    {
                        objEvent.JournalItem = objEventRecurMaster.JournalItem;
                    }

                    if (_editRecur && objEvent.AllowAnonEnroll == objEventRMSave.AllowAnonEnroll ||
                        !_editRecur)
                    {
                        objEvent.AllowAnonEnroll = objEventRecurMaster.AllowAnonEnroll;
                    }

                    if (_editRecur && objEvent.EnrollType == objEventRMSave.EnrollType || !_editRecur)
                    {
                        objEvent.EnrollType = objEventRecurMaster.EnrollType;
                    }

                    if (_editRecur && objEvent.PayPalAccount == objEventRMSave.PayPalAccount || !_editRecur)
                    {
                        objEvent.PayPalAccount = objEventRecurMaster.PayPalAccount;
                    }

                    if (_editRecur && objEvent.EnrollFee == objEventRMSave.EnrollFee || !_editRecur)
                    {
                        objEvent.EnrollFee = objEventRecurMaster.EnrollFee;
                    }

                    if (_editRecur && objEvent.MaxEnrollment == objEventRMSave.MaxEnrollment || !_editRecur)
                    {
                        objEvent.MaxEnrollment = objEventRecurMaster.MaxEnrollment;
                    }

                    if (_editRecur && objEvent.EnrollRoleID == objEventRMSave.EnrollRoleID || !_editRecur)
                    {
                        objEvent.EnrollRoleID = objEventRecurMaster.EnrollRoleID;
                    }
                    if (_editRecur && objEvent.DetailPage == objEventRMSave.DetailPage || !_editRecur)
                    {
                        objEvent.DetailPage = objEventRecurMaster.DetailPage;
                    }
                    if (_editRecur && objEvent.DetailNewWin == objEventRMSave.DetailNewWin || !_editRecur)
                    {
                        objEvent.DetailNewWin = objEventRecurMaster.DetailNewWin;
                    }

                    if (_editRecur && objEvent.DetailURL == objEventRMSave.DetailURL || !_editRecur)
                    {
                        objEvent.DetailURL = objEventRecurMaster.DetailURL;
                    }

                    if (_editRecur && objEvent.ImageDisplay == objEventRMSave.ImageDisplay || !_editRecur)
                    {
                        objEvent.ImageDisplay = objEventRecurMaster.ImageDisplay;
                    }
                    if (_editRecur && objEvent.ImageType == objEventRMSave.ImageType || !_editRecur)
                    {
                        objEvent.ImageType = objEventRecurMaster.ImageType;
                    }

                    if (_editRecur && objEvent.ImageURL == objEventRMSave.ImageURL || !_editRecur)
                    {
                        objEvent.ImageURL = objEventRecurMaster.ImageURL;
                    }

                    if (_editRecur && objEvent.ImageWidth == objEventRMSave.ImageWidth || !_editRecur)
                    {
                        objEvent.ImageWidth = objEventRecurMaster.ImageWidth;
                    }

                    if (_editRecur && objEvent.ImageHeight == objEventRMSave.ImageHeight || !_editRecur)
                    {
                        objEvent.ImageHeight = objEventRecurMaster.ImageHeight;
                    }

                    if (_editRecur && objEvent.Category == objEventRMSave.Category || !_editRecur)
                    {
                        objEvent.Category = objEventRecurMaster.Category;
                    }
                    if (_editRecur && objEvent.Location == objEventRMSave.Location || !_editRecur)
                    {
                        objEvent.Location = objEventRecurMaster.Location;
                    }

                    // Save Event Notification Info
                    if (_editRecur && objEvent.SendReminder == objEventRMSave.SendReminder || !_editRecur)
                    {
                        objEvent.SendReminder = objEventRecurMaster.SendReminder;
                    }

                    if (_editRecur && objEvent.Reminder == objEventRMSave.Reminder || !_editRecur)
                    {
                        objEvent.Reminder = objEventRecurMaster.Reminder;
                    }

                    if (_editRecur && objEvent.Notify == objEventRMSave.Notify || !_editRecur)
                    {
                        objEvent.Notify = objEventRecurMaster.Notify;
                    }

                    if (_editRecur && objEvent.ReminderTime == objEventRMSave.ReminderTime || !_editRecur)
                    {
                        objEvent.ReminderTime = objEventRecurMaster.ReminderTime;
                    }

                    if (_editRecur && objEvent.ReminderTimeMeasurement == objEventRMSave.ReminderTimeMeasurement ||
                        !_editRecur)
                    {
                        objEvent.ReminderTimeMeasurement = objEventRecurMaster.ReminderTimeMeasurement;
                    }

                    if (_editRecur && objEvent.ReminderFrom == objEventRMSave.ReminderFrom || !_editRecur)
                    {
                        objEvent.ReminderFrom = objEventRecurMaster.ReminderFrom;
                    }

                    if (_editRecur && objEvent.OwnerID == objEventRMSave.OwnerID || !_editRecur)
                    {
                        objEvent.OwnerID = objEventRecurMaster.OwnerID;
                    }

                    // Set for re-submit to Search Engine
                    objEvent.SearchSubmitted = false;

                    if (_editRecur && objEvent.CustomField1 == objEventRMSave.CustomField1 || !_editRecur)
                    {
                        objEvent.CustomField1 = objEventRecurMaster.CustomField1;
                    }

                    if (_editRecur && objEvent.CustomField2 == objEventRMSave.CustomField2 || !_editRecur)
                    {
                        objEvent.CustomField2 = objEventRecurMaster.CustomField2;
                    }

                    if (_editRecur && objEvent.EnrollListView == objEventRMSave.EnrollListView || !_editRecur)
                    {
                        objEvent.EnrollListView = objEventRecurMaster.EnrollListView;
                    }

                    if (_editRecur && objEvent.DisplayEndDate == objEventRMSave.DisplayEndDate || !_editRecur)
                    {
                        objEvent.DisplayEndDate = objEventRecurMaster.DisplayEndDate;
                    }

                    if (_editRecur && objEvent.AllDayEvent == objEventRMSave.AllDayEvent || !_editRecur)
                    {
                        objEvent.AllDayEvent = objEventRecurMaster.AllDayEvent;
                    }
                    if (_editRecur && objEvent.Summary == objEventRMSave.Summary || !_editRecur)
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
                        objEvent.LastUpdatedID = UserId;
                        objEvent.Approved = objEventRecurMaster.Approved;
                        objEvent.UpdateStatus = "Update";
                    }
                }

                // Do we need to check for schedule conflict
                if (Settings.Preventconflicts && objEvent.UpdateStatus != "Delete")
                {
                    var getSubEvents = Settings.MasterEvent;
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
                        if (Settings.Locationconflict)
                        {
                            valLocationConflict.IsValid = false;
                            valLocationConflict.Visible = true;
                            valLocationConflict.ErrorMessage =
                                Localization.GetString("valLocationConflict", LocalResourceFile) + " - " +
                                string.Format("{0:g}", conflictDate);
                            valLocationConflict.Text =
                                Localization.GetString("valLocationConflict", LocalResourceFile) + " - " +
                                string.Format("{0:g}", conflictDate);
                        }
                        else
                        {
                            valConflict.IsValid = false;
                            valConflict.Visible = true;
                            valConflict.ErrorMessage =
                                Localization.GetString("valConflict", LocalResourceFile) + " - " +
                                string.Format("{0:g}", conflictDate);
                            valConflict.Text = Localization.GetString("valConflict", LocalResourceFile) +
                                                    " - " + string.Format("{0:g}", conflictDate);
                        }
                        return;
                    }
                }
            }

            if (objEventRecurMaster.RecurMasterID == -1 ||
                objEventRMSave.RRULE == string.Empty && !_editRecur || _editRecur)
            {
                objEventRecurMaster =
                    _objCtlEventRecurMaster.EventsRecurMasterSave(objEventRecurMaster, TabId, true);
            }

            if (objEventRecurMaster.RecurMasterID == -1)
            {
                SelectedDate = objEventRecurMaster.Dtstart.Date;
            }

            // url tracking
            var objUrls = new UrlController();
            objUrls.UpdateUrl(PortalId, Convert.ToString(ctlURL.Url), Convert.ToString(ctlURL.UrlType),
                              Convert.ToBoolean(ctlURL.Log), Convert.ToBoolean(ctlURL.Track), ModuleId,
                              Convert.ToBoolean(ctlURL.NewWindow));
            objUrls.UpdateUrl(PortalId, Convert.ToString(URLDetail.Url),
                              Convert.ToString(URLDetail.UrlType), Convert.ToBoolean(URLDetail.Log),
                              Convert.ToBoolean(URLDetail.Track), ModuleId,
                              Convert.ToBoolean(URLDetail.NewWindow));

            var blEmailSend = false;
            var blModeratorEmailSent = false;
            var objEventEmail = new EventInfo();
            foreach (EventInfo tempLoopVar_objEvent in _lstEvents)
            {
                objEvent = tempLoopVar_objEvent;
                objEvent.RecurMasterID = objEventRecurMaster.RecurMasterID;
                switch (objEvent.UpdateStatus)
                {
                    case "Match":
                        break;
                    case "Delete":
                        _objCtlEvent.EventsDelete(objEvent.EventID, objEvent.ModuleID, objEvent.ContentItemID);
                        break;
                    default:
                        if (!objEvent.Cancelled)
                        {
                            var oEvent = objEvent;
                            objEvent = _objCtlEvent.EventsSave(objEvent, false, TabId, true);
                            if (!oEvent.Approved && !blModeratorEmailSent)
                            {
                                oEvent.RRULE = objEventRecurMaster.RRULE;
                                SendModeratorEmail(oEvent);
                                blModeratorEmailSent = true;
                            }
                            if (oEvent.EventID != -1)
                            {
                                UpdateExistingNotificationRecords(oEvent);
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
                SendNewEventEmails(objEventEmail);
                CreateNewEventJournal(objEventEmail);
            }
            if (chkEventEmailChk.Checked)
            {
                SendEventEmail((EventInfo) _lstEvents[0]);
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

            if (rblRepeatTypeN.Checked)
            {
                objEventRecurMaster.RRULE = "";
            }
            else if (rblRepeatTypeP1.Checked)
            {
                switch (cmbP1Period.SelectedItem.Value.Trim())
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

                objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + ";INTERVAL=" + txtP1Every.Text;
            }
            else if (rblRepeatTypeW1.Checked)
            {
                objEventRecurMaster.RRULE = "FREQ=WEEKLY;WKST=" + strWkst + ";INTERVAL=" + txtW1Every.Text +
                                            ";BYDAY=";
                if (chkW1Sun.Checked || chkW1Sun2.Checked)
                {
                    objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + "SU,";
                }
                if (chkW1Mon.Checked)
                {
                    objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + "MO,";
                }
                if (chkW1Tue.Checked)
                {
                    objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + "TU,";
                }
                if (chkW1Wed.Checked)
                {
                    objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + "WE,";
                }
                if (chkW1Thu.Checked)
                {
                    objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + "TH,";
                }
                if (chkW1Fri.Checked)
                {
                    objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + "FR,";
                }
                if (chkW1Sat.Checked)
                {
                    objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + "SA,";
                }
                objEventRecurMaster.RRULE =
                    objEventRecurMaster.RRULE.Substring(0, objEventRecurMaster.RRULE.Length - 1);
            }
            else if (rblRepeatTypeM1.Checked && rblRepeatTypeM.Checked)
            {
                objEventRecurMaster.RRULE = "FREQ=MONTHLY;INTERVAL=" + txtMEvery.Text + ";BYDAY=";
                var intWeek = 0;
                var strWeek = "";
                if (cmbM1Every.SelectedIndex < 4)
                {
                    intWeek = cmbM1Every.SelectedIndex + 1;
                    strWeek = "+" + Convert.ToString(intWeek);
                }
                else
                {
                    intWeek = -1;
                    strWeek = Convert.ToString(intWeek);
                }
                objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + strWeek;

                var strDay = "";
                switch (cmbM1Period.SelectedValue)
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
            else if (rblRepeatTypeM2.Checked && rblRepeatTypeM.Checked)
            {
                objEventRecurMaster.RRULE = "FREQ=MONTHLY;INTERVAL=" + txtMEvery.Text + ";BYMONTHDAY=+" +
                                            cmbM2Period.SelectedValue;
            }
            else if (rblRepeatTypeY1.Checked)
            {
                var yearDate = Convert.ToDateTime(dpY1Period.SelectedDate);
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
                if (chkReminder.Checked)
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
                        objEvent.EventID, eventTimeBegin, ModuleId);
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
                if (Settings.Moderateall)
                {
                    var objEventEmailInfo = new EventEmailInfo();
                    var objEventEmail = new EventEmails(PortalId, ModuleId, LocalResourceFile,
                                                        ((PageBase) Page).PageCulture.Name);
                    objEventEmailInfo.TxtEmailSubject = Settings.Templates.moderateemailsubject;
                    objEventEmailInfo.TxtEmailBody = Settings.Templates.moderateemailmessage;
                    objEventEmailInfo.TxtEmailFrom = Settings.StandardEmail;
                    var moderators = GetModerators();
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
            var objEventEmail = new EventEmails(PortalId, ModuleId, LocalResourceFile,
                                                ((PageBase) Page).PageCulture.Name);
            objEventEmailInfo.TxtEmailSubject = txtEventEmailSubject.Text;
            objEventEmailInfo.TxtEmailBody = txtEventEmailBody.Text;
            objEventEmailInfo.TxtEmailFrom = txtEventEmailFrom.Text;
            EventEmailAddRoleUsers(int.Parse(ddEventEmailRoles.SelectedValue), objEventEmailInfo);
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
            if (_editRecur)
            {
                objSignups =
                    _objCtlEventSignups
                        .EventsSignupsGetEventRecurMaster(objEvent.RecurMasterID, objEvent.ModuleID);
            }
            else
            {
                objSignups = _objCtlEventSignups.EventsSignupsGetEvent(objEvent.EventID, objEvent.ModuleID);
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
                    objUser = objCtlUser.GetUser(PortalId, objSignup.UserID);
                    var objEventInfoHelper =
                        new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
                    objEnrollListItem.EnrollDisplayName = objEventInfoHelper
                        .UserDisplayNameProfile(objSignup.UserID, objSignup.UserName, LocalResourceFile)
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
                    objEnrollListItem.EnrollUserName = Localization.GetString("AnonUser", LocalResourceFile);
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
                grdEnrollment.DataSource = eventEnrollment;
                grdEnrollment.DataBind();
                tblEventEmail.Attributes.Add("style", "display:block; width:100%");
                lblEnrolledUsers.Visible = true;
                grdEnrollment.Visible = true;
                lnkSelectedDelete.Visible = true;
                lnkSelectedEmail.Visible = true;
            }
            else
            {
                lblEnrolledUsers.Visible = false;
                grdEnrollment.Visible = false;
                if (!Settings.Newpereventemail)
                {
                    tblEventEmail.Attributes.Add("style", "display:none; width:100%");
                }
                lnkSelectedDelete.Visible = false;
                lnkSelectedEmail.Visible = false;
            }

            objEvent.Enrolled = eventEnrollment.Count;
            objEvent.Signups = true;
            ShowHideEnrolleeColumns(objEvent);

            txtEnrolled.Text = noEnrolees.ToString();
            valNoEnrolees.MaximumValue = Convert.ToString(objEvent.MaxEnrollment - noEnrolees);
            if ((int.Parse(valNoEnrolees.MaximumValue) > Settings.Maxnoenrolees) |
                (objEvent.MaxEnrollment == 0))
            {
                valNoEnrolees.MaximumValue = Settings.Maxnoenrolees.ToString();
            }
            else if (int.Parse(valNoEnrolees.MaximumValue) < 1)
            {
                valNoEnrolees.MaximumValue = "1";
            }
            lblMaxNoEnrolees.Text =
                string.Format(Localization.GetString("lblMaxNoEnrolees", LocalResourceFile),
                              valNoEnrolees.MaximumValue);
        }

        private void ShowHideEnrolleeColumns(EventInfo objEvent)
        {
            var txtColumns = EnrolmentColumns(objEvent, true);
            var gvUsersToEnroll = (GridView) grdAddUser.FindControl("gvUsersToEnroll");
            if (txtColumns.LastIndexOf("UserName", StringComparison.Ordinal) < 0)
            {
                grdEnrollment.Columns[1].Visible = false;
                gvUsersToEnroll.Columns[1].Visible = false;
            }
            else
            {
                grdEnrollment.Columns[1].Visible = true;
                gvUsersToEnroll.Columns[1].Visible = true;
            }
            if (txtColumns.LastIndexOf("DisplayName", StringComparison.Ordinal) < 0)
            {
                grdEnrollment.Columns[2].Visible = false;
                gvUsersToEnroll.Columns[2].Visible = false;
            }
            else
            {
                grdEnrollment.Columns[2].Visible = true;
                gvUsersToEnroll.Columns[2].Visible = true;
            }
            if (txtColumns.LastIndexOf("Email", StringComparison.Ordinal) < 0)
            {
                grdEnrollment.Columns[3].Visible = false;
                gvUsersToEnroll.Columns[3].Visible = false;
            }
            else
            {
                grdEnrollment.Columns[3].Visible = true;
                gvUsersToEnroll.Columns[3].Visible = true;
            }
            if (txtColumns.LastIndexOf("Phone", StringComparison.Ordinal) < 0)
            {
                grdEnrollment.Columns[4].Visible = false;
            }
            else
            {
                grdEnrollment.Columns[4].Visible = true;
            }
            if (txtColumns.LastIndexOf("Approved", StringComparison.Ordinal) < 0)
            {
                grdEnrollment.Columns[5].Visible = false;
            }
            else
            {
                grdEnrollment.Columns[5].Visible = true;
            }
            if (txtColumns.LastIndexOf("Qty", StringComparison.Ordinal) < 0)
            {
                grdEnrollment.Columns[6].Visible = false;
            }
            else
            {
                grdEnrollment.Columns[6].Visible = true;
            }
            if (_editRecur)
            {
                grdEnrollment.Columns[7].Visible = true;
            }
            else
            {
                grdEnrollment.Columns[7].Visible = false;
            }
        }

        private void AddRegUser(int inUserID, EventInfo objEvent)
        {
            // Check if signup already exists since due to partial rendering it may be possible
            // to click the enroll user link twice
            var intUserID = inUserID;
            _objEventSignups =
                _objCtlEventSignups.EventsSignupsGetUser(objEvent.EventID, intUserID, objEvent.ModuleID);

            if (ReferenceEquals(_objEventSignups, null))
            {
                // Get user info
                var objUserInfo = UserController.GetUserById(PortalId, intUserID);

                _objEventSignups = new EventSignupsInfo();
                _objEventSignups.EventID = objEvent.EventID;
                _objEventSignups.ModuleID = objEvent.ModuleID;
                _objEventSignups.UserID = intUserID;
                _objEventSignups.AnonEmail = null;
                _objEventSignups.AnonName = null;
                _objEventSignups.AnonTelephone = null;
                _objEventSignups.AnonCulture = null;
                _objEventSignups.AnonTimeZoneId = null;
                _objEventSignups.PayPalPaymentDate = DateTime.UtcNow;
                _objEventSignups.Approved = true;
                _objEventSignups.NoEnrolees = int.Parse(txtNoEnrolees.Text);
                _objEventSignups = CreateEnrollment(_objEventSignups, objEvent);

                // Mail users
                if (Settings.SendEnrollMessageAdded)
                {
                    var objEventEmailInfo = new EventEmailInfo();
                    var objEventEmail = new EventEmails(PortalId, ModuleId, LocalResourceFile,
                                                        ((PageBase) Page).PageCulture.Name);
                    objEventEmailInfo.TxtEmailSubject = Settings.Templates.txtEnrollMessageSubject;
                    objEventEmailInfo.TxtEmailBody = Settings.Templates.txtEnrollMessageAdded;
                    objEventEmailInfo.TxtEmailFrom = Settings.StandardEmail;
                    objEventEmailInfo.UserIDs.Add(_objEventSignups.UserID);
                    objEventEmailInfo.UserIDs.Add(objEvent.OwnerID);
                    objEventEmail.SendEmails(objEventEmailInfo, objEvent, _objEventSignups);
                }
            }
        }

        private bool ValidateTime(DateTime indate)
        {
            var inMinutes = indate.Minute;
            var remainder = inMinutes % int.Parse(Settings.Timeinterval);
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
                Response.Redirect(GetStoredPrevPage(), true);
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
                UpdateProcessing(_itemID);
                if (Page.IsValid)
                {
                    Response.Redirect(GetStoredPrevPage(), true);
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
                _objEvent = _objCtlEvent.EventsGet(_itemID, ModuleId);
                if (_editRecur)
                {
                    _objCtlEventRecurMaster.EventsRecurMasterDelete(
                        _objEvent.RecurMasterID, _objEvent.ModuleID);
                }
                else
                {
                    if (_objEvent.RRULE != "")
                    {
                        _objEvent.Cancelled = true;
                        _objEvent.LastUpdatedID = UserId;
                        _objEvent = _objCtlEvent.EventsSave(_objEvent, false, TabId, true);
                    }
                    else
                    {
                        _objCtlEventRecurMaster.EventsRecurMasterDelete(
                            _objEvent.RecurMasterID, _objEvent.ModuleID);
                    }
                }
                Response.Redirect(GetSocialNavigateUrl(), true);
            }
            catch (Exception) //Module failed to load
            {
                //ProcessModuleLoadException(Me, exc)
            }
        }

        protected void lnkSelectedEmail_Click(object sender, EventArgs e)
        {
            Email(true);
        }

        protected void lnkSelectedDelete_Click(object sender, EventArgs e)
        {
            var item = default(DataGridItem);
            var objEnroll = default(EventSignupsInfo);
            var eventID = 0;

            foreach (DataGridItem tempLoopVar_item in grdEnrollment.Items)
            {
                item = tempLoopVar_item;
                if (((CheckBox) item.FindControl("chkSelect")).Checked)
                {
                    var intSignupID = Convert.ToInt32(grdEnrollment.DataKeys[item.ItemIndex]);
                    objEnroll = _objCtlEventSignups.EventsSignupsGet(intSignupID, ModuleId, false);
                    if (!ReferenceEquals(objEnroll, null))
                    {
                        if (eventID != objEnroll.EventID)
                        {
                            _objEvent = _objCtlEvent.EventsGet(objEnroll.EventID, ModuleId);
                        }
                        eventID = objEnroll.EventID;

                        // Delete Selected Enrollee
                        DeleteEnrollment(intSignupID, _objEvent.ModuleID, _objEvent.EventID);

                        // Mail users
                        if (Settings.SendEnrollMessageDeleted)
                        {
                            var objEventEmailInfo = new EventEmailInfo();
                            var objEventEmail =
                                new EventEmails(PortalId, ModuleId, LocalResourceFile,
                                                ((PageBase) Page).PageCulture.Name);
                            objEventEmailInfo.TxtEmailSubject = Settings.Templates.txtEnrollMessageSubject;
                            objEventEmailInfo.TxtEmailBody = Settings.Templates.txtEnrollMessageDeleted;
                            objEventEmailInfo.TxtEmailFrom = Settings.StandardEmail;
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
                            objEventEmailInfo.UserIDs.Add(_objEvent.OwnerID);
                            objEventEmail.SendEmails(objEventEmailInfo, _objEvent, objEnroll);
                        }
                    }
                }
            }

            LoadRegUsers();
            BuildEnrolleeGrid(_objEvent);
        }

        protected void copyButton_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateProcessing(-1);
                if (Page.IsValid)
                {
                    Response.Redirect(GetStoredPrevPage(), true);
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void ddEnrollRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRegUsers();
        }

        protected void chkSignups_CheckedChanged(object sender, EventArgs e)
        {
            tblEventEmail.Attributes.Add("style", "display:none; width:100%");
            if (chkSignups.Checked)
            {
                tblEnrollmentDetails.Attributes.Add("style", "display:block;");
                LoadRegUsers();
                if (txtEnrolled.Text != 0.ToString())
                {
                    tblEventEmail.Attributes.Add("style", "display:block; width:100%");
                }
            }
            else
            {
                tblEnrollmentDetails.Attributes.Add("style", "display:none;");
                if (Settings.Newpereventemail && chkEventEmailChk.Checked)
                {
                    tblEventEmail.Attributes.Add("style", "display:block; width:100%");
                }
            }
        }

        protected void grdAddUser_AddSelectedUsers(object sender, EventArgs e, ArrayList arrUsers)
        {
            try
            {
                if (int.Parse(txtNoEnrolees.Text) > int.Parse(valNoEnrolees.MaximumValue) ||
                    int.Parse(txtNoEnrolees.Text) < int.Parse(valNoEnrolees.MinimumValue))
                {
                    valNoEnrolees.IsValid = false;
                    valNoEnrolees.Visible = true;
                    valNoEnrolees.ErrorMessage =
                        string.Format(Localization.GetString("valNoEnrolees", LocalResourceFile),
                                      valNoEnrolees.MaximumValue);
                    return;
                }
            }
            catch
            {
                valNoEnrolees.IsValid = false;
                valNoEnrolees.Visible = true;
                valNoEnrolees.ErrorMessage =
                    string.Format(Localization.GetString("valNoEnrolees", LocalResourceFile),
                                  valNoEnrolees.MaximumValue);
                return;
            }

            var objEvent = _objCtlEvent.EventsGet(_itemID, ModuleId);

            foreach (int inUserid in arrUsers)
            {
                AddRegUser(inUserid, objEvent);
            }
            LoadRegUsers();
            BuildEnrolleeGrid(objEvent);
            txtNoEnrolees.Text = Convert.ToString(1.ToString());
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