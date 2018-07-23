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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using System.Xml.Xsl;    
    using DotNetNuke.Common;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Users;
    using DotNetNuke.Framework;
    using DotNetNuke.Security;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using global::Components;
    using EventInfo = global::Components.EventInfo;

    [DNNtc.ModuleControlProperties("Details", "Events Details", DNNtc.ControlType.View, "https://dnnevents.codeplex.com/documentation", true, false)]
    public partial class EventDetails : EventBase
    {
        #region Event Handlers

        private void Page_Load(object sender, EventArgs e)
        {
            // Log exception whem status is filled
            if (!(this.Request.Params["status"] == null))
            {
                var objSecurity = new PortalSecurity();
                var status = objSecurity.InputFilter(this.Request.Params["status"],
                                                     (PortalSecurity.FilterFlag) ((int) PortalSecurity
                                                                                      .FilterFlag.NoScripting |
                                                                                  (int) PortalSecurity
                                                                                      .FilterFlag.NoMarkup));
                Exceptions.LogException(new ModuleLoadException("EventDetails Call...status: " + status));
            }

            // Add the external Validation.js to the Page
            const string csname = "ExtValidationScriptFile";
            var cstype = MethodBase.GetCurrentMethod().GetType();
            var cstext = "<script src=\"" + this.ResolveUrl("~/DesktopModules/Events/Scripts/Validation.js") +
                         "\" type=\"text/javascript\"></script>";
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered(csname))
            {
                this.Page.ClientScript.RegisterClientScriptBlock(cstype, csname, cstext, false);
            }

            // Force full PostBack since these pass off to aspx page
            if (AJAX.IsInstalled())
            {
                AJAX.RegisterPostBackControl(this.cmdvEvent);
                AJAX.RegisterPostBackControl(this.cmdvEventSeries);
                AJAX.RegisterPostBackControl(this.cmdvEventSignups);
            }

            this.cmdvEvent.ToolTip = Localization.GetString("cmdvEventTooltip", this.LocalResourceFile);
            this.cmdvEvent.Text = Localization.GetString("cmdvEventExport", this.LocalResourceFile);
            this.cmdvEventSeries.ToolTip = Localization.GetString("cmdvEventSeriesTooltip", this.LocalResourceFile);
            this.cmdvEventSeries.Text = Localization.GetString("cmdvEventExportSeries", this.LocalResourceFile);
            this.cmdvEventSignups.ToolTip = Localization.GetString("cmdvEventSignupsTooltip", this.LocalResourceFile);
            this.cmdvEventSignups.Text = Localization.GetString("cmdvEventSignupsDownload", this.LocalResourceFile);

            this.cmdPrint.ToolTip = Localization.GetString("Print", this.LocalResourceFile);

            try
            {
                //Get the item id of the selected event
                if (!ReferenceEquals(this.Request.Params["ItemId"], null))
                {
                    this.ItemId = int.Parse(this.Request.Params["ItemId"]);
                }
                else
                {
                    this.Response.Redirect(this.GetSocialNavigateUrl(), true);
                }

                // Set the selected theme
                if (this.Settings.Eventdetailnewpage)
                {
                    this.SetTheme(this.pnlEventsModuleDetails);
                    this.AddFacebookMetaTags();
                }

                // If the page is being requested the first time, determine if an
                // contact itemId value is specified, and if so populate page
                // contents with the contact details
                if (this.Page.IsPostBack)
                {
                    return;
                }


                var objCtlEvent = new EventController();
                this._eventInfo = objCtlEvent.EventsGet(this.ItemId, this.ModuleId);

                //If somebody has sent a bad ItemID and eventinfo not retrieved, return 301
                if (ReferenceEquals(this._eventInfo, null))
                {
                    this.Response.StatusCode = 301;
                    this.Response.AppendHeader("Location", this.GetSocialNavigateUrl());
                    return;
                }

                // Do they have permissions to the event
                var objCtlEventInfoHelper = new EventInfoHelper(this.ModuleId, this.Settings);
                if (this.Settings.Enforcesubcalperms && !objCtlEventInfoHelper.IsModuleViewer(this._eventInfo.ModuleID))
                {
                    this.Response.Redirect(this.GetSocialNavigateUrl(), true);
                }
                else if (this.IsPrivateNotModerator && this.UserId != this._eventInfo.OwnerID)
                {
                    this.Response.Redirect(this.GetSocialNavigateUrl(), true);
                }
                else if ((this.Settings.SocialGroupModule == EventModuleSettings.SocialModule.UserProfile) &
                         !objCtlEventInfoHelper.IsSocialUserPublic(this.GetUrlUserId()))
                {
                    this.Response.Redirect(this.GetSocialNavigateUrl(), true);
                }
                else if ((this.Settings.SocialGroupModule == EventModuleSettings.SocialModule.SocialGroup) &
                         !objCtlEventInfoHelper.IsSocialGroupPublic(this.GetUrlGroupId()))
                {
                    this.Response.Redirect(this.GetSocialNavigateUrl(), true);
                }

                // Has the event been cancelled
                if (this._eventInfo.Cancelled)
                {
                    this.Response.StatusCode = 301;
                    this.Response.AppendHeader("Location", this.GetSocialNavigateUrl());
                    return;
                }

                // So we have a valid item, but is it from a module that has been deleted
                // but not removed from the recycle bin
                if (this._eventInfo.ModuleID != this.ModuleId)
                {
                    var objCtlModule = new ModuleController();
                    var objModules = objCtlModule.GetModuleTabs(this._eventInfo.ModuleID);
                    var objModule = default(ModuleInfo);
                    var isDeleted = true;
                    foreach (ModuleInfo tempLoopVar_objModule in objModules)
                    {
                        objModule = tempLoopVar_objModule;
                        if (!objModule.IsDeleted)
                        {
                            isDeleted = false;
                        }
                    }
                    if (isDeleted)
                    {
                        this.Response.StatusCode = 301;
                        this.Response.AppendHeader("Location", this.GetSocialNavigateUrl());
                        return;
                    }
                }

                // Should all be OK to display now
                var displayTimeZoneId = this._eventInfo.EventTimeZoneId;
                if (!this.Settings.EnableEventTimeZones)
                {
                    displayTimeZoneId = this.GetDisplayTimeZoneId();
                }
                var objEventInfoHelper = new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);
                this._eventInfo = objEventInfoHelper.ConvertEventToDisplayTimeZone(this._eventInfo, displayTimeZoneId);

                var tcc = new TokenReplaceControllerClass(this.ModuleId, this.LocalResourceFile);

                //Set the page title
                if (this.Settings.EnableSEO)
                {
                    var txtPageText = string.Format(this.Settings.Templates.txtSEOPageTitle, this.BasePage.Title);
                    txtPageText = tcc.TokenReplaceEvent(this._eventInfo, txtPageText, false);
                    txtPageText = HttpUtility.HtmlDecode(txtPageText);
                    txtPageText = HtmlUtils.StripTags(txtPageText, true);
                    txtPageText = txtPageText.Replace(Environment.NewLine, " ");
                    txtPageText = HtmlUtils.StripWhiteSpace(txtPageText, true);
                    this.BasePage.Title = txtPageText;
                    txtPageText = string.Format(this.Settings.Templates.txtSEOPageDescription,
                                                this.BasePage.Description);
                    txtPageText = tcc.TokenReplaceEvent(this._eventInfo, txtPageText, false);
                    txtPageText = HttpUtility.HtmlDecode(txtPageText);
                    txtPageText = HtmlUtils.StripTags(txtPageText, true);
                    txtPageText = txtPageText.Replace(Environment.NewLine, " ");
                    txtPageText = HtmlUtils.StripWhiteSpace(txtPageText, true);
                    txtPageText = HtmlUtils.Shorten(txtPageText, this.Settings.SEODescriptionLength, "...");
                    this.BasePage.Description = txtPageText;
                    txtPageText = this.BasePage.KeyWords;
                    if (!ReferenceEquals(this._eventInfo.LocationName, null))
                    {
                        if (!string.IsNullOrEmpty(txtPageText))
                        {
                            txtPageText = txtPageText + ",";
                        }
                        txtPageText = txtPageText + this._eventInfo.LocationName;
                    }
                    if (!ReferenceEquals(this._eventInfo.CategoryName, null))
                    {
                        if (!string.IsNullOrEmpty(txtPageText))
                        {
                            txtPageText = txtPageText + ",";
                        }
                        txtPageText = txtPageText + this._eventInfo.CategoryName;
                    }
                    this.BasePage.KeyWords = txtPageText;
                }

                //Replace tokens
                var txtTemplate = "";
                var txtTemplate1 = "";
                var txtTemplate2 = "";
                var txtTemplate3 = "";
                var txtTemplate4 = "";
                txtTemplate = this.Settings.Templates.EventDetailsTemplate;
                txtTemplate1 = txtTemplate;
                txtTemplate2 = "";
                txtTemplate3 = "";
                txtTemplate4 = "";
                var nTemplate = 0;
                while (txtTemplate.IndexOf("[BREAK]") + 1 > 0)
                {
                    nTemplate++;
                    var txtBefore = "";
                    var nBreak = txtTemplate.IndexOf("[BREAK]") + 1;
                    if (nBreak > 1)
                    {
                        txtBefore = txtTemplate.Substring(0, nBreak - 1);
                    }
                    else
                    {
                        txtBefore = "";
                    }
                    if (txtTemplate.Length > nBreak + 6)
                    {
                        txtTemplate = txtTemplate.Substring(nBreak + 7 - 1);
                    }
                    else
                    {
                        txtTemplate = "";
                    }
                    switch (nTemplate)
                    {
                        case 1:
                            txtTemplate1 = txtBefore;
                            txtTemplate2 = txtTemplate;
                            break;
                        case 2:
                            txtTemplate2 = txtBefore;
                            txtTemplate3 = txtTemplate;
                            break;
                        case 3:
                            txtTemplate3 = txtBefore;
                            txtTemplate4 = txtTemplate;
                            break;
                        case 4:
                            txtTemplate4 = txtBefore;
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(txtTemplate1) && txtTemplate1 != "\r\n")
                {
                    this.divEventDetails1.InnerHtml = tcc.TokenReplaceEvent(this._eventInfo, txtTemplate1);
                    this.divEventDetails1.Attributes.Add("style", "display:block;");
                }
                else
                {
                    this.divEventDetails1.Attributes.Add("style", "display:none;");
                }
                if (!string.IsNullOrEmpty(txtTemplate2) && txtTemplate2 != "\r\n")
                {
                    this.divEventDetails2.InnerHtml = tcc.TokenReplaceEvent(this._eventInfo, txtTemplate2);
                    this.divEventDetails2.Attributes.Add("style", "display:block;");
                }
                else
                {
                    this.divEventDetails2.Attributes.Add("style", "display:none;");
                }
                if (!string.IsNullOrEmpty(txtTemplate3) && txtTemplate3 != "\r\n")
                {
                    this.divEventDetails3.InnerHtml = tcc.TokenReplaceEvent(this._eventInfo, txtTemplate3);
                    this.divEventDetails3.Attributes.Add("style", "display:block;");
                }
                else
                {
                    this.divEventDetails3.Attributes.Add("style", "display:none;");
                }
                if (!string.IsNullOrEmpty(txtTemplate4) && txtTemplate4 != "\r\n")
                {
                    this.divEventDetails4.InnerHtml = tcc.TokenReplaceEvent(this._eventInfo, txtTemplate4);
                    this.divEventDetails4.Attributes.Add("style", "display:block;");
                }
                else
                {
                    this.divEventDetails4.Attributes.Add("style", "display:none;");
                }

                this.editButton.Visible = false;
                this.deleteButton.Visible = false;
                this.cmdvEventSignups.Visible = false;
                if (this.IsEventEditor(this._eventInfo, false))
                {
                    this.editButton.Visible = true;
                    this.editButton.NavigateUrl =
                        objEventInfoHelper.GetEditURL(this._eventInfo.EventID, this._eventInfo.SocialGroupId,
                                                      this._eventInfo.SocialUserId);
                    this.editButton.ToolTip = Localization.GetString("editButton", this.LocalResourceFile);
                    this.deleteButton.Visible = true;
                    this.deleteButton.Attributes.Add(
                        "onclick",
                        "javascript:return confirm('" +
                        Localization.GetString("ConfirmEventDelete", this.LocalResourceFile) + "');");
                    this.cmdvEventSignups.Visible = true;
                }
                this.editSeriesButton.Visible = false;
                this.deleteSeriesButton.Visible = false;
                if (this._eventInfo.RRULE != "")
                {
                    // Note that IsEventEditor with 'True' is used here because this is for the
                    // series(buttons)and excludes single event owner. Must be recurrence master owner.
                    if (this.IsEventEditor(this._eventInfo, true))
                    {
                        this.editSeriesButton.Visible = true;
                        this.editSeriesButton.NavigateUrl =
                            objEventInfoHelper.GetEditURL(this._eventInfo.EventID, this._eventInfo.SocialGroupId,
                                                          this._eventInfo.SocialUserId, "All");
                        this.editSeriesButton.ToolTip =
                            Localization.GetString("editSeriesButton", this.LocalResourceFile);
                        this.deleteSeriesButton.Visible = true;
                        this.deleteSeriesButton.Attributes.Add(
                            "onclick",
                            "javascript:return confirm('" +
                            Localization.GetString("ConfirmEventSeriesDelete", this.LocalResourceFile) + "');");
                    }
                }
                if (this._eventInfo.RRULE == "")
                {
                    this.cmdvEventSeries.Visible = false;
                }

                var objEventTimeZoneUtilities = new EventTimeZoneUtilities();
                var nowDisplay = objEventTimeZoneUtilities
                    .ConvertFromUTCToDisplayTimeZone(DateTime.UtcNow, this.GetDisplayTimeZoneId()).EventDate;

                //  Compute Dates/Times (for recurring)
                var startdate = this._eventInfo.EventTimeBegin;
                this.SelectedDate = startdate.Date;

                // See if user already are signed up
                // And that Signup is Authorized
                // And also that the Date/Time has not passed
                this.divEnrollment.Attributes.Add("style", "display:none;");
                if (this._eventInfo.Signups)
                {
                    if (startdate > nowDisplay)
                    {
                        if (this._eventInfo.EnrollRoleID == null || this._eventInfo.EnrollRoleID == -1)
                        {
                            this.UserEnrollment(this._eventInfo);
                        }
                        else
                        {
                            var objEventSignupsController = new EventSignupsController();
                            if (objEventSignupsController.IsEnrollRole(this._eventInfo.EnrollRoleID, this.PortalId))
                            {
                                this.UserEnrollment(this._eventInfo);
                            }
                        }
                    }
                    else
                    {
                        this.divEnrollment.Attributes.Add("style", "display:block;");
                        this.lblEnrollTooLate.Text = Localization.GetString("EnrollTooLate", this.LocalResourceFile);
                        this.enroll4.Visible = true;
                    }
                }

                //Are You Sure You Want To Enroll?'
                if (this.Request.IsAuthenticated)
                {
                    if (this.Settings.Enableenrollpopup)
                    {
                        this.cmdSignup.Attributes.Add(
                            "onclick",
                            "javascript:return confirm('" +
                            Localization.GetString("SureYouWantToEnroll", this.LocalResourceFile) + "');");
                    }
                    this.valNoEnrolees.MaximumValue =
                        Convert.ToString(this._eventInfo.MaxEnrollment - this._eventInfo.Enrolled);
                    if ((int.Parse(this.valNoEnrolees.MaximumValue) > this.Settings.Maxnoenrolees) |
                        (this._eventInfo.MaxEnrollment == 0))
                    {
                        this.valNoEnrolees.MaximumValue = this.Settings.Maxnoenrolees.ToString();
                    }
                    this.lblMaxNoEnrolees.Text =
                        string.Format(Localization.GetString("lblMaxNoEnrolees", this.LocalResourceFile),
                                      this.valNoEnrolees.MaximumValue);
                    this.valNoEnrolees.ErrorMessage =
                        string.Format(Localization.GetString("valNoEnrolees", this.LocalResourceFile),
                                      int.Parse(this.valNoEnrolees.MaximumValue));
                    this.valNoEnrolees2.ErrorMessage = this.valNoEnrolees.ErrorMessage;
                }

                this.divMessage.Attributes.Add("style", "display:none;");

                if (this.Settings.IcalEmailEnable)
                {
                    this.divIcalendar.Attributes.Add("style", "display:block;");
                    this.txtUserEmailiCal.Text = this.UserInfo.Email;
                    if (this.Request.IsAuthenticated)
                    {
                        this.txtUserEmailiCal.Enabled = false;
                    }
                    else
                    {
                        this.txtUserEmailiCal.Enabled = true;
                    }
                }
                else
                {
                    this.divIcalendar.Attributes.Add("style", "display:none;");
                }

                //Is notification enabled
                this.divReminder.Attributes.Add("style", "display:none;");
                if (this._eventInfo.SendReminder && startdate > nowDisplay)
                {
                    //Is registered user
                    if (this.Request.IsAuthenticated)
                    {
                        this.divReminder.Attributes.Add("style", "display:block;");
                        var objEventNotificationController = new EventNotificationController();
                        var notificationInfo =
                            objEventNotificationController.NotifyInfo(this._eventInfo.EventID, this.UserInfo.Email,
                                                                      this.ModuleId, this.LocalResourceFile,
                                                                      this.GetDisplayTimeZoneId());
                        if (!string.IsNullOrEmpty(notificationInfo))
                        {
                            this.lblConfirmation.Text = notificationInfo;
                            this.rem3.Visible = true;
                            this.imgConfirmation.AlternateText =
                                Localization.GetString("Reminder", this.LocalResourceFile);
                        }
                        else
                        {
                            this.txtUserEmail.Text = this.UserInfo.Email;
                        }
                    }
                    // is anonymous notification allowed or registered user not yet notified
                    if (this.Settings.Notifyanon && !this.Request.IsAuthenticated || this.txtUserEmail.Text.Length > 0)
                    {
                        if (this.Request.IsAuthenticated)
                        {
                            this.txtUserEmail.Enabled = false;
                        }
                        else
                        {
                            this.txtUserEmail.Enabled = true;
                        }
                        this.divReminder.Attributes.Add("style", "display:block;");
                        var errorminutes = Localization.GetString("invalidReminderMinutes", this.LocalResourceFile);
                        var errorhours = Localization.GetString("invalidReminderHours", this.LocalResourceFile);
                        var errordays = Localization.GetString("invalidReminderDays", this.LocalResourceFile);

                        this.txtReminderTime.Text =
                            this._eventInfo.ReminderTime.ToString(); //load default Reminder Time of event
                        this.txtReminderTime.Visible = true;
                        this.ddlReminderTimeMeasurement.Attributes.Add(
                            "onchange",
                            "valRemTime('" + this.valReminderTime.ClientID + "','" +
                            this.valReminderTime2.ClientID + "','" + this.valReminderTime.ValidationGroup + "','" +
                            this.ddlReminderTimeMeasurement.ClientID + "','" + errorminutes + "','" + errorhours +
                            "','" + errordays + "');");
                        this.ddlReminderTimeMeasurement.Visible = true;
                        this.ddlReminderTimeMeasurement.SelectedValue = this._eventInfo.ReminderTimeMeasurement;

                        switch (this._eventInfo.ReminderTimeMeasurement)
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

                        this.rem1.Visible = true;
                        this.imgNotify.AlternateText = Localization.GetString("Reminder", this.LocalResourceFile);
                        this.rem2.Visible = true;
                        if (this._eventInfo.RRULE != "")
                        {
                            this.chkReminderRec.Visible = true;
                        }
                    }
                }
                this.grdEnrollment.Columns[0].HeaderText =
                    Localization.GetString("EnrollUserName", this.LocalResourceFile);
                this.grdEnrollment.Columns[1].HeaderText =
                    Localization.GetString("EnrollDisplayName", this.LocalResourceFile);
                this.grdEnrollment.Columns[2].HeaderText =
                    Localization.GetString("EnrollEmail", this.LocalResourceFile);
                this.grdEnrollment.Columns[3].HeaderText =
                    Localization.GetString("EnrollPhone", this.LocalResourceFile);
                this.grdEnrollment.Columns[4].HeaderText =
                    Localization.GetString("EnrollApproved", this.LocalResourceFile);
                this.grdEnrollment.Columns[5].HeaderText = Localization.GetString("EnrollNo", this.LocalResourceFile);

                this.BindEnrollList(this._eventInfo);
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

        #region Properties

        /// <summary>
        ///     Stores the ItemId in the viewstate
        /// </summary>
        private int ItemId
        {
            get { return Convert.ToInt32(this.ViewState["EventItemID" + this.ModuleId]); }
            set { this.ViewState["EventItemID" + this.ModuleId] = value.ToString(); }
        }

        private EventInfo _eventInfo = new EventInfo();

        private enum MessageLevel
        {
            DNNSuccess = 1,
            DNNInformation,
            DNNWarning,
            DNNError
        }

        #endregion

        #region Helper Methods

        /// <summary>
        ///     Bind enrolled users to the enrollment grid
        /// </summary>
        private void BindEnrollList(EventInfo eventInfo)
        {
            this.divEnrollList.Attributes.Add("style", "display:none;");
            var blEnrollList = false;
            var txtColumns = "";
            if (eventInfo.Signups && eventInfo.Enrolled > 0)
            {
                txtColumns = this.EnrolmentColumns(eventInfo, eventInfo.EnrollListView);
            }
            if (!string.IsNullOrEmpty(txtColumns))
            {
                blEnrollList = true;
            }

            if (blEnrollList)
            {
                if (txtColumns.LastIndexOf("UserName", StringComparison.Ordinal) < 0)
                {
                    this.grdEnrollment.Columns[0].Visible = false;
                }
                if (txtColumns.LastIndexOf("DisplayName", StringComparison.Ordinal) < 0)
                {
                    this.grdEnrollment.Columns[1].Visible = false;
                }
                if (txtColumns.LastIndexOf("Email", StringComparison.Ordinal) < 0)
                {
                    this.grdEnrollment.Columns[2].Visible = false;
                }
                if (txtColumns.LastIndexOf("Phone", StringComparison.Ordinal) < 0)
                {
                    this.grdEnrollment.Columns[3].Visible = false;
                }
                if (txtColumns.LastIndexOf("Approved", StringComparison.Ordinal) < 0)
                {
                    this.grdEnrollment.Columns[4].Visible = false;
                }
                if (txtColumns.LastIndexOf("Qty", StringComparison.Ordinal) < 0)
                {
                    this.grdEnrollment.Columns[5].Visible = false;
                }

                //Load enrol list
                var eventEnrollment = new ArrayList();
                var objSignups = default(ArrayList);
                var objSignup = default(EventSignupsInfo);
                var objCtlUser = new UserController();
                var objCtlEventSignups = new EventSignupsController();
                objSignups = objCtlEventSignups.EventsSignupsGetEvent(eventInfo.EventID, this.ModuleId);
                foreach (EventSignupsInfo tempLoopVar_objSignup in objSignups)
                {
                    objSignup = tempLoopVar_objSignup;
                    var objEnrollListItem = new EventEnrollList();
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
                                              eventInfo.EventName);
                            objEnrollListItem.EnrollPhone = objUser.Profile.Telephone;
                        }
                    }
                    else
                    {
                        objEnrollListItem.EnrollDisplayName = objSignup.AnonName;
                        objEnrollListItem.EnrollUserName = Localization.GetString("AnonUser", this.LocalResourceFile);
                        objEnrollListItem.EnrollEmail =
                            string.Format("<a href=\"mailto:{0}?subject={1}\">{0}</a>", objSignup.AnonEmail,
                                          eventInfo.EventName);
                        objEnrollListItem.EnrollPhone = objSignup.AnonTelephone;
                    }
                    objEnrollListItem.SignupID = objSignup.SignupID;
                    objEnrollListItem.EnrollApproved = objSignup.Approved;
                    objEnrollListItem.EnrollNo = objSignup.NoEnrolees;
                    eventEnrollment.Add(objEnrollListItem);
                }
                if (eventEnrollment.Count > 0)
                {
                    this.divEnrollList.Attributes.Add("style", "display:block;");
                    this.grdEnrollment.DataSource = eventEnrollment;
                    this.grdEnrollment.DataBind();
                }
            }
        }

        /// <summary>
        ///     Display User Enrollment Info on Page
        /// </summary>
        private MessageLevel UserEnrollment(EventInfo eventInfo)
        {
            var returnValue = default(MessageLevel);
            returnValue = MessageLevel.DNNSuccess;
            if (!this.Settings.Eventsignup)
            {
                this.divEnrollment.Attributes.Add("style", "display:none;");
                return returnValue;
            }

            this.divEnrollment.Attributes.Add("style", "display:block;");
            this.enroll3.Visible = false;
            this.enroll5.Visible = false;
            if (!ReferenceEquals(this.Request.Params["Status"], null))
            {
                if (this.Request.Params["Status"].ToLower() == "enrolled")
                {
                    // User has been successfully enrolled for this event (paid enrollment)
                    this.lblSignup.Text = Localization.GetString("StatusPPSuccess", this.LocalResourceFile);
                    this.enroll2.Visible = true;
                    this.imgSignup.AlternateText = Localization.GetString("StatusPPSuccess", this.LocalResourceFile);
                }
                else if (this.Request.Params["Status"].ToLower() == "cancelled")
                {
                    // User has been cancelled paid enrollment
                    this.lblSignup.Text = Localization.GetString("StatusPPCancelled", this.LocalResourceFile);
                    returnValue = MessageLevel.DNNInformation;
                    this.enroll2.Visible = true;
                    this.imgSignup.AlternateText = Localization.GetString("StatusPPCancelled", this.LocalResourceFile);
                }
                return returnValue;
            }

            // If not authenticated and anonymous not allowed setup for logintoenroll
            if (!this.Request.IsAuthenticated && !eventInfo.AllowAnonEnroll)
            {
                this.enroll1.Visible = true;
                this.imgEnroll.AlternateText = Localization.GetString("LoginToEnroll", this.LocalResourceFile);
                this.cmdSignup.Text = Localization.GetString("LoginToEnroll", this.LocalResourceFile);
                return returnValue;
            }

            // If not authenticated make email/name boxes visible, or find out if authenticated user has already enrolled
            var objCtlEventSignups = new EventSignupsController();
            EventSignupsInfo objEventSignups = null;
            if (!this.Request.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(this.txtAnonEmail.Text))
                {
                    objEventSignups =
                        objCtlEventSignups.EventsSignupsGetAnonUser(eventInfo.EventID, this.txtAnonEmail.Text,
                                                                    this.ModuleId);
                }
            }
            else
            {
                objEventSignups =
                    objCtlEventSignups.EventsSignupsGetUser(eventInfo.EventID, this.UserId, this.ModuleId);
            }

            if (ReferenceEquals(objEventSignups, null))
            {
                if (!this.Request.IsAuthenticated && !this.Settings.EnrollmentPageAllowed)
                {
                    this.enroll5.Visible = true;
                }
                if (eventInfo.Enrolled < eventInfo.MaxEnrollment ||
                    eventInfo.MaxEnrollment == 0)
                {
                    if (this.Settings.Maxnoenrolees > 1 && !this.Settings.EnrollmentPageAllowed)
                    {
                        this.enroll3.Visible = true;
                    }
                    // User is not enrolled for this event...press the link to enroll!
                    this.enroll1.Visible = true;
                    this.imgEnroll.AlternateText = Localization.GetString("EnrollForEvent", this.LocalResourceFile);
                    this.cmdSignup.Text = Localization.GetString("EnrollForEvent", this.LocalResourceFile);
                }
            }
            else
            {
                this.enroll2.Visible = true;
                if (objEventSignups.Approved)
                {
                    // User is enrolled and approved for this event!
                    this.imgSignup.AlternateText =
                        Localization.GetString("YouAreEnrolledForThisEvent", this.LocalResourceFile);
                    this.lblSignup.Text = Localization.GetString("YouAreEnrolledForThisEvent", this.LocalResourceFile);
                    returnValue = MessageLevel.DNNSuccess;
                }
                else
                {
                    // User is enrolled for this event, but not yet approved!
                    this.imgSignup.AlternateText =
                        Localization.GetString("EnrolledButNotApproved", this.LocalResourceFile);
                    this.lblSignup.Text = Localization.GetString("EnrolledButNotApproved", this.LocalResourceFile);
                    returnValue = MessageLevel.DNNWarning;
                }
            }

            return returnValue;
        }

        /// <summary>
        ///     Redirect to EventVCal page which exports the event to a vCard
        /// </summary>
        private void ExportEvent(bool series)
        {
            try
            {
                this.Response.Redirect("~/DesktopModules/Events/EventVCal.aspx?ItemID=" +
                                       Convert.ToString(this.ItemId) + "&Mid=" + Convert.ToString(this.ModuleId) +
                                       "&tabid=" + Convert.ToString(this.TabId) + "&Series=" +
                                       Convert.ToString(series));
            }
            catch (ThreadAbortException)
            {
                //Ignore
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        ///     Download the enrollees for this event as a file.
        /// </summary>
        private void DownloadSignups()
        {
            //Get the event.
            var theEvent = new EventController().EventsGet(this.ItemId, this.ModuleId);
            if (theEvent != null)
            {
                //Dim xmlDoc As XmlDocument = DefineXmlFile(theEvent, True)
                //If xmlDoc IsNot Nothing Then
                //    GenerateXmlFile(theEvent, xmlDoc)
                //End If

                var csvDoc = this.DefineCsvFile(theEvent);
                if (csvDoc != null)
                {
                    this.GenerateCsvFile(theEvent, csvDoc);
                }
            }
        }

        /// <summary>
        ///     Define the xml file for downloading the enrollees for this event.
        /// </summary>
        private XmlDocument DefineXmlFile(EventInfo theEvent, bool localizeTags)
        {
            var returnValue = default(XmlDocument);
            returnValue = null;
            var xmlDoc = new XmlDocument();

            //Get the enrollees.
            var eventSignups = new EventSignupsController().EventsSignupsGetEvent(this.ItemId, this.ModuleId);
            //Anything to do
            if (ReferenceEquals(eventSignups, null) || eventSignups.Count == 0)
            {
                return returnValue;
            }

            try
            {
                //Initialization.
                var xNamespace = XNamespace.Get("");
                var xRoot =
                    new XElement(xNamespace + "Document",
                                 new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"));

                //Tags should be localized for XML output. These will be converted to a header row in Excel.
                //Tags should not be localized for CSV output. A separate localized header row is added.
                var xElemHdr = default(XElement);
                if (localizeTags)
                {
                    xElemHdr = new XElement("Enrollee",
                                            new XElement("EventName",
                                                         Localization.GetString(
                                                             "Event Name.Header", this.LocalResourceFile)),
                                            new XElement("EventStart",
                                                         Localization.GetString(
                                                             "Event Start.Header", this.LocalResourceFile)),
                                            new XElement(
                                                "EventEnd",
                                                Localization.GetString("Event End.Header", this.LocalResourceFile)),
                                            new XElement(
                                                "Location",
                                                Localization.GetString("Location.Header", this.LocalResourceFile)),
                                            new XElement(
                                                "Category",
                                                Localization.GetString("Category.Header", this.LocalResourceFile)),
                                            new XElement("ReferenceNumber",
                                                         Localization.GetString(
                                                             "ReferenceNumber.Header", this.LocalResourceFile)),
                                            new XElement(
                                                "Company",
                                                Localization.GetString("Company.Header", this.LocalResourceFile)),
                                            new XElement(
                                                "JobTitle",
                                                Localization.GetString("JobTitle.Header", this.LocalResourceFile)),
                                            new XElement(
                                                "FullName",
                                                Localization.GetString("FullName.Header", this.LocalResourceFile)),
                                            new XElement("FirstName",
                                                         Localization.GetString(
                                                             "FirstName.Header", this.LocalResourceFile)),
                                            new XElement(
                                                "LastName",
                                                Localization.GetString("LastName.Header", this.LocalResourceFile)),
                                            new XElement(
                                                "Email",
                                                Localization.GetString("Email.Header", this.LocalResourceFile)),
                                            new XElement(
                                                "Phone",
                                                Localization.GetString("Phone.Header", this.LocalResourceFile)),
                                            new XElement(
                                                "Street",
                                                Localization.GetString("Street.Header", this.LocalResourceFile)),
                                            new XElement("PostalCode",
                                                         Localization.GetString(
                                                             "PostalCode.Header", this.LocalResourceFile)),
                                            new XElement(
                                                "City", Localization.GetString("City.Header", this.LocalResourceFile)),
                                            new XElement(
                                                "Region",
                                                Localization.GetString("Region.Header", this.LocalResourceFile)),
                                            new XElement(
                                                "Country",
                                                Localization.GetString("Country.Header", this.LocalResourceFile)));
                }
                else
                {
                    xElemHdr = new XElement("Enrollee",
                                            new XElement("EventName", string.Empty),
                                            new XElement("EventStart", string.Empty),
                                            new XElement("EventEnd", string.Empty),
                                            new XElement("Location", string.Empty),
                                            new XElement("Category", string.Empty),
                                            new XElement("ReferenceNumber", string.Empty),
                                            new XElement("Company", string.Empty),
                                            new XElement("JobTitle", string.Empty),
                                            new XElement("FullName", string.Empty),
                                            new XElement("FirstName", string.Empty),
                                            new XElement("LastName", string.Empty),
                                            new XElement("Email", string.Empty),
                                            new XElement("Phone", string.Empty),
                                            new XElement("Street", string.Empty),
                                            new XElement("PostalCode", string.Empty),
                                            new XElement("City", string.Empty),
                                            new XElement("Region", string.Empty),
                                            new XElement("Country", string.Empty));
                }

                //Names cannot be empty nor contain spaces.
                foreach (var xElem in xElemHdr.Elements())
                {
                    if (string.IsNullOrEmpty(xElem.Value))
                    {
                        xElem.Value = xElem.Name.ToString();
                    }
                    xElem.Value = Convert.ToString(xElem.Value.Trim().Replace(" ", "_"));
                }

                //Gather the information.
                var xElemList = new List<XElement>();
                var xElemEvent = default(XElement);

                //Add a localized header row when tags are not localized.
                if (!localizeTags)
                {
                    xElemEvent = new XElement(xNamespace + "Enrollee",
                                              new XElement(xNamespace + xElemHdr.Elements("EventName").First().Value,
                                                           Localization.GetString(
                                                               "Event Name.Header", this.LocalResourceFile)),
                                              new XElement(xNamespace + xElemHdr.Elements("EventStart").First().Value,
                                                           Localization.GetString(
                                                               "Event Start.Header", this.LocalResourceFile)),
                                              new XElement(xNamespace + xElemHdr.Elements("EventEnd").First().Value,
                                                           Localization.GetString(
                                                               "Event End.Header", this.LocalResourceFile)),
                                              new XElement(xNamespace + xElemHdr.Elements("Location").First().Value,
                                                           Localization.GetString(
                                                               "Location.Header", this.LocalResourceFile)),
                                              new XElement(xNamespace + xElemHdr.Elements("Category").First().Value,
                                                           Localization.GetString(
                                                               "Category.Header", this.LocalResourceFile)),
                                              new XElement(
                                                  xNamespace + xElemHdr.Elements("ReferenceNumber").First().Value,
                                                  Localization.GetString(
                                                      "ReferenceNumber.Header", this.LocalResourceFile)),
                                              new XElement(xNamespace + xElemHdr.Elements("Company").First().Value,
                                                           Localization.GetString(
                                                               "Company.Header", this.LocalResourceFile)),
                                              new XElement(xNamespace + xElemHdr.Elements("JobTitle").First().Value,
                                                           Localization.GetString(
                                                               "JobTitle.Header", this.LocalResourceFile)),
                                              new XElement(xNamespace + xElemHdr.Elements("FullName").First().Value,
                                                           Localization.GetString(
                                                               "FullName.Header", this.LocalResourceFile)),
                                              new XElement(xNamespace + xElemHdr.Elements("FirstName").First().Value,
                                                           Localization.GetString(
                                                               "FirstName.Header", this.LocalResourceFile)),
                                              new XElement(xNamespace + xElemHdr.Elements("LastName").First().Value,
                                                           Localization.GetString(
                                                               "LastName.Header", this.LocalResourceFile)),
                                              new XElement(xNamespace + xElemHdr.Elements("Email").First().Value,
                                                           Localization.GetString(
                                                               "Email.Header", this.LocalResourceFile)),
                                              new XElement(xNamespace + xElemHdr.Elements("Phone").First().Value,
                                                           Localization.GetString(
                                                               "Phone.Header", this.LocalResourceFile)),
                                              new XElement(xNamespace + xElemHdr.Elements("Street").First().Value,
                                                           Localization.GetString(
                                                               "Street.Header", this.LocalResourceFile)),
                                              new XElement(xNamespace + xElemHdr.Elements("PostalCode").First().Value,
                                                           Localization.GetString(
                                                               "PostalCode.Header", this.LocalResourceFile)),
                                              new XElement(xNamespace + xElemHdr.Elements("City").First().Value,
                                                           Localization.GetString(
                                                               "City.Header", this.LocalResourceFile)),
                                              new XElement(xNamespace + xElemHdr.Elements("Region").First().Value,
                                                           Localization.GetString(
                                                               "Region.Header", this.LocalResourceFile)),
                                              new XElement(xNamespace + xElemHdr.Elements("Country").First().Value,
                                                           Localization.GetString(
                                                               "Country.Header", this.LocalResourceFile))
                    );
                    xElemList.Add(xElemEvent);
                }

                var objCtlUser = new UserController();
                foreach (EventSignupsInfo eventSignup in eventSignups)
                {
                    if (eventSignup.UserID != -1)
                    {
                        //Known DNN/Evoq user. Get info from user profile.
                        var objUser = objCtlUser.GetUser(this.PortalId, eventSignup.UserID);
                        xElemEvent = new XElement(xNamespace + "Enrollee",
                                                  new XElement(
                                                      xNamespace + xElemHdr.Elements("EventName").First().Value,
                                                      theEvent.EventName),
                                                  new XElement(
                                                      xNamespace + xElemHdr.Elements("EventStart").First().Value,
                                                      theEvent.EventTimeBegin),
                                                  new XElement(xNamespace + xElemHdr.Elements("EventEnd").First().Value,
                                                               theEvent.EventTimeEnd),
                                                  new XElement(xNamespace + xElemHdr.Elements("Location").First().Value,
                                                               theEvent.LocationName),
                                                  new XElement(xNamespace + xElemHdr.Elements("Category").First().Value,
                                                               theEvent.CategoryName),
                                                  new XElement(
                                                      xNamespace + xElemHdr.Elements("ReferenceNumber").First().Value,
                                                      eventSignup.ReferenceNumber),
                                                  new XElement(xNamespace + xElemHdr.Elements("Company").First().Value,
                                                               this.GetPropertyForDownload(objUser, "Company")),
                                                  new XElement(xNamespace + xElemHdr.Elements("JobTitle").First().Value,
                                                               this.GetPropertyForDownload(objUser, "JobTitle")),
                                                  new XElement(xNamespace + xElemHdr.Elements("FullName").First().Value,
                                                               objUser.DisplayName),
                                                  new XElement(
                                                      xNamespace + xElemHdr.Elements("FirstName").First().Value,
                                                      objUser.FirstName),
                                                  new XElement(xNamespace + xElemHdr.Elements("LastName").First().Value,
                                                               objUser.LastName),
                                                  new XElement(xNamespace + xElemHdr.Elements("Email").First().Value,
                                                               objUser.Email),
                                                  new XElement(xNamespace + xElemHdr.Elements("Phone").First().Value,
                                                               this.GetPropertyForDownload(objUser, "Telephone")),
                                                  new XElement(xNamespace + xElemHdr.Elements("Street").First().Value,
                                                               this.GetPropertyForDownload(objUser, "Street")),
                                                  new XElement(
                                                      xNamespace + xElemHdr.Elements("PostalCode").First().Value,
                                                      this.GetPropertyForDownload(objUser, "PostalCode")),
                                                  new XElement(xNamespace + xElemHdr.Elements("City").First().Value,
                                                               this.GetPropertyForDownload(objUser, "City")),
                                                  new XElement(xNamespace + xElemHdr.Elements("Region").First().Value,
                                                               this.GetPropertyForDownload(objUser, "Region")),
                                                  new XElement(xNamespace + xElemHdr.Elements("Country").First().Value,
                                                               this.GetPropertyForDownload(objUser, "Country")));
                    }
                    else
                    {
                        //Anonymous user (site visitor). Get info from event signup.
                        xElemEvent = new XElement(xNamespace + "Enrollee",
                                                  new XElement(
                                                      xNamespace + xElemHdr.Elements("EventName").First().Value,
                                                      theEvent.EventName),
                                                  new XElement(
                                                      xNamespace + xElemHdr.Elements("EventStart").First().Value,
                                                      theEvent.EventTimeBegin),
                                                  new XElement(xNamespace + xElemHdr.Elements("EventEnd").First().Value,
                                                               theEvent.EventTimeEnd),
                                                  new XElement(xNamespace + xElemHdr.Elements("Location").First().Value,
                                                               theEvent.LocationName),
                                                  new XElement(xNamespace + xElemHdr.Elements("Category").First().Value,
                                                               theEvent.CategoryName),
                                                  new XElement(
                                                      xNamespace + xElemHdr.Elements("ReferenceNumber").First().Value,
                                                      eventSignup.ReferenceNumber),
                                                  new XElement(xNamespace + xElemHdr.Elements("Company").First().Value,
                                                               eventSignup.Company),
                                                  new XElement(xNamespace + xElemHdr.Elements("JobTitle").First().Value,
                                                               eventSignup.JobTitle),
                                                  new XElement(xNamespace + xElemHdr.Elements("FullName").First().Value,
                                                               eventSignup.AnonName),
                                                  new XElement(
                                                      xNamespace + xElemHdr.Elements("FirstName").First().Value,
                                                      eventSignup.FirstName),
                                                  new XElement(xNamespace + xElemHdr.Elements("LastName").First().Value,
                                                               eventSignup.LastName),
                                                  new XElement(xNamespace + xElemHdr.Elements("Email").First().Value,
                                                               eventSignup.AnonEmail),
                                                  new XElement(xNamespace + xElemHdr.Elements("Phone").First().Value,
                                                               eventSignup.AnonTelephone),
                                                  new XElement(xNamespace + xElemHdr.Elements("Street").First().Value,
                                                               eventSignup.Street),
                                                  new XElement(
                                                      xNamespace + xElemHdr.Elements("PostalCode").First().Value,
                                                      eventSignup.PostalCode),
                                                  new XElement(xNamespace + xElemHdr.Elements("City").First().Value,
                                                               eventSignup.City),
                                                  new XElement(xNamespace + xElemHdr.Elements("Region").First().Value,
                                                               eventSignup.Region),
                                                  new XElement(xNamespace + xElemHdr.Elements("Country").First().Value,
                                                               eventSignup.Country));
                    }
                    xElemList.Add(xElemEvent);
                }

                //Aggregate into a document.
                var xElemMain = xRoot;
                xElemMain.Add(xElemList); //Everything ...
                xmlDoc.Load(xElemMain.CreateReader());

                //Add a declaration.
                var xmlDecla = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                var xmlElemDoc = xmlDoc.DocumentElement;
                xmlDoc.InsertBefore(xmlDecla, xmlElemDoc);

                //Return.
                returnValue = xmlDoc;
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            return returnValue;
        }

        /// <summary>
        ///     Define the csv file for downloading the enrollees for this event.
        /// </summary>
        private string DefineCsvFile(EventInfo theEvent)
        {
            var returnValue = "";
            returnValue = null;
            var csvDoc = new StringBuilder();

            //From xml to csv by xslt.
            var xmlDoc = this.DefineXmlFile(theEvent, false);
            if (xmlDoc != null)
            {
                var xPathDoc = new XPathDocument(XmlReader.Create(new StringReader(xmlDoc.InnerXml)));
                var xslTrafo = new XslCompiledTransform();
                xslTrafo.Load(XmlReader.Create(
                                  new StringReader(
                                      File.ReadAllText(
                                          Path.Combine(this.Request.MapPath(this.ControlPath),
                                                       "EventEnrollees.xslt")))));
                var xWriter = new StringWriter(csvDoc);
                xslTrafo.Transform(xPathDoc, null, xWriter);
                xWriter.Close();
            }

            return csvDoc.ToString();
        }

        /// <summary>
        ///     Generate the xml file for downloading the enrollees for this event.
        /// </summary>
        private bool GenerateXmlFile(EventInfo theEvent, XmlDocument xmlDoc)
        {
            var returnValue = false;
            returnValue = false;

            //Name the file with a timestamp.
            var fileName = theEvent.EventName;
            if (!string.IsNullOrEmpty(Localization.GetString("EnrollmentsFile.Text", this.LocalResourceFile)))
            {
                fileName += " - " + Localization.GetString("EnrollmentsFile.Text", this.LocalResourceFile);
            }
            fileName += DateTime.Now.ToString(" - yyyyMMdd_HHmmss");

            //The contents of the file.
            var fileContent = xmlDoc.InnerXml;

            try
            {
                //Stream the file.
                var myContext = HttpContext.Current;
                var myResponse = default(HttpResponse);
                myResponse = myContext.Response;
                myResponse.ContentEncoding = Encoding.UTF8;
                myResponse.ContentType = "application/force-download";
                myResponse.AppendHeader("Content-Disposition", "filename=\"" + fileName + ".xml\"");

                myResponse.Write(fileContent);
                myResponse.End();

                returnValue = true;
            }
            catch (Exception)
            {
                returnValue = false;
            }
            return returnValue;
        }

        /// <summary>
        ///     Generate the csv file for downloading the enrollees for this event.
        /// </summary>
        private bool GenerateCsvFile(EventInfo theEvent, string csvDoc)
        {
            var returnValue = false;
            returnValue = false;

            //Name the file with a timestamp.
            var fileName = theEvent.EventName;
            if (!string.IsNullOrEmpty(Localization.GetString("EnrollmentsFile.Text", this.LocalResourceFile)))
            {
                fileName += " - " + Localization.GetString("EnrollmentsFile.Text", this.LocalResourceFile);
            }
            fileName += DateTime.Now.ToString(" - yyyyMMdd_HHmmss");

            //The contents of the file.
            var fileContent = csvDoc;

            try
            {
                //Stream the file.
                var myContext = HttpContext.Current;
                var myResponse = default(HttpResponse);
                myResponse = myContext.Response;
                myResponse.ContentEncoding = Encoding.UTF8;
                myResponse.ContentType = "application/force-download";
                myResponse.AppendHeader("Content-Disposition", "filename=\"" + fileName + ".csv\"");

                myResponse.Write(fileContent);
                myResponse.End();

                returnValue = true;
            }
            catch (Exception)
            {
                returnValue = false;
            }
            return returnValue;
        }

        private string GetPropertyForDownload(UserInfo objUser, string propertyName)
        {
            var returnValue = "";
            returnValue = null;

            if (!string.IsNullOrEmpty(propertyName))
            {
                var profileProperty = objUser.Profile.GetProperty(propertyName);

                if (profileProperty != null)
                {
                    returnValue = profileProperty.PropertyValue;
                }
            }
            return returnValue;
        }

        private void ShowMessage(string msg, MessageLevel messageLevel)
        {
            this.lblMessage.Text = msg;

            //Hide the rest of the form fields.
            this.divMessage.Attributes.Add("style", "display:block;");

            switch (messageLevel)
            {
                case MessageLevel.DNNSuccess:
                    this.divMessage.Attributes.Add("class", "dnnFormMessage dnnFormSuccess");
                    break;
                case MessageLevel.DNNInformation:
                    this.divMessage.Attributes.Add("class", "dnnFormMessage dnnFormInfo");
                    break;
                case MessageLevel.DNNWarning:
                    this.divMessage.Attributes.Add("class", "dnnFormMessage dnnFormWarning");
                    break;
                case MessageLevel.DNNError:
                    this.divMessage.Attributes.Add("class", "dnnFormMessage dnnFormValidationSummary");
                    break;
            }
        }

        #endregion

        #region Links and Buttons

        /// <summary>
        ///     When delete button is clicked the current event will be removed
        /// </summary>
        protected void deleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                var iItemID = this.ItemId;
                var objCtlEvent = new EventController();
                var objEvent = objCtlEvent.EventsGet(iItemID, this.ModuleId);
                if (objEvent.RRULE != "")
                {
                    objEvent.Cancelled = true;
                    objEvent.LastUpdatedID = this.UserId;
                    objCtlEvent.EventsSave(objEvent, true, this.TabId, true);
                }
                else
                {
                    var objCtlEventRecurMaster = new EventRecurMasterController();
                    objCtlEventRecurMaster.EventsRecurMasterDelete(objEvent.RecurMasterID, objEvent.ModuleID);
                }
                this.Response.Redirect(this.GetSocialNavigateUrl(), true);
            }
            catch (ThreadAbortException)
            {
                //Ignore
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        ///     When delete series button is clicked the current event series will be removed
        /// </summary>
        protected void deleteSeriesButton_Click(object sender, EventArgs e)
        {
            try
            {
                var iItemID = this.ItemId;
                var objCtlEvent = new EventController();
                var objEvent = objCtlEvent.EventsGet(iItemID, this.ModuleId);
                var objCtlEventRecurMaster = new EventRecurMasterController();
                objCtlEventRecurMaster.EventsRecurMasterDelete(objEvent.RecurMasterID, objEvent.ModuleID);
                this.Response.Redirect(this.GetSocialNavigateUrl(), true);
            }
            catch (ThreadAbortException)
            {
                //Ignore
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        ///     When Export simple button is clicked the current event will be exported to a vcard
        /// </summary>
        protected void cmdvEvent_Click(object sender, EventArgs e)
        {
            this.ExportEvent(false);
        }

        /// <summary>
        ///     When Export event series button is clicked the current event will be exported to a vcard
        /// </summary>
        protected void cmdvEventSeries_Click(object sender, EventArgs e)
        {
            this.ExportEvent(true);
        }

        /// <summary>
        ///     When Download event signups button is clicked the current event signups will be written to an XML file for
        ///     download.
        /// </summary>
        protected void cmdvEventSignups_Click(object sender, EventArgs e)
        {
            this.DownloadSignups();
        }

        /// <summary>
        ///     When return button is clicked the user is redirected to the previous page
        /// </summary>
        /// <remarks></remarks>
        protected void returnButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.Response.Redirect(this.GetSocialNavigateUrl(), true);
            }
            catch (ThreadAbortException)
            {
                //Ignore
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        ///     When signup button is clicked the user will be signed up for the event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        protected void cmdSignup_Click(object sender, EventArgs e)
        {
            if (!this.Request.IsAuthenticated && !this.Settings.AllowAnonEnroll)
            {
                this.RedirectToLogin();
            }

            try
            {
                var objEvent = default(EventInfo);
                var objCtlEvent = new EventController();
                objEvent = objCtlEvent.EventsGet(this.ItemId, this.ModuleId);
                if (!this.Request.IsAuthenticated && !objEvent.AllowAnonEnroll)
                {
                    this.RedirectToLogin();
                }

                // In case of custom enrollment page.
                if (this.Settings.EnrollmentPageAllowed)
                {
                    if (!string.IsNullOrEmpty(this.Settings.EnrollmentPageDefaultUrl))
                    {
                        this.Response.Redirect(this.Settings.EnrollmentPageDefaultUrl + "?mod=" +
                                               Convert.ToString(this.ModuleId) + "&event=" +
                                               Convert.ToString(this.ItemId));
                    }
                    return;
                }

                // In case of standard paid enrollment.
                // Check to see if unauthenticated user has already enrolled
                var objCtlEventSignups = new EventSignupsController();
                if (!this.Request.IsAuthenticated)
                {
                    var objEventsSignups = default(EventSignupsInfo);
                    objEventsSignups =
                        objCtlEventSignups.EventsSignupsGetAnonUser(objEvent.EventID, this.txtAnonEmail.Text,
                                                                    objEvent.ModuleID);
                    if (!ReferenceEquals(objEventsSignups, null))
                    {
                        this.ShowMessage(
                            Localization.GetString("YouAreAlreadyEnrolledForThisEvent", this.LocalResourceFile),
                            MessageLevel.DNNWarning);
                        this.enroll1.Visible = false;
                        this.enroll3.Visible = false;
                        this.enroll5.Visible = false;
                        return;
                    }
                }

                if (objEvent.EnrollType == "PAID")
                {
                    // Paid Even Process
                    try
                    {
                        var objEventInfoHelper =
                            new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);
                        var socialGroupId = this.GetUrlGroupId();
                        if (this.Request.IsAuthenticated)
                        {
                            if (socialGroupId > 0)
                            {
                                this.Response.Redirect(
                                    objEventInfoHelper.AddSkinContainerControls(
                                        Globals.NavigateURL(this.TabId, "PPEnroll",
                                                            "Mid=" + Convert.ToString(this.ModuleId),
                                                            "ItemID=" + Convert.ToString(this.ItemId),
                                                            "NoEnrol=" + this.txtNoEnrolees.Text,
                                                            "groupid=" + socialGroupId), "?"));
                            }
                            else
                            {
                                this.Response.Redirect(
                                    objEventInfoHelper.AddSkinContainerControls(
                                        Globals.NavigateURL(this.TabId, "PPEnroll",
                                                            "Mid=" + Convert.ToString(this.ModuleId),
                                                            "ItemID=" + Convert.ToString(this.ItemId),
                                                            "NoEnrol=" + this.txtNoEnrolees.Text), "?"));
                            }
                        }
                        else
                        {
                            var urlAnonTelephone = this.txtAnonTelephone.Text.Trim();
                            if (string.IsNullOrEmpty(urlAnonTelephone))
                            {
                                urlAnonTelephone = "0";
                            }
                            if (socialGroupId > 0)
                            {
                                this.Response.Redirect(
                                    objEventInfoHelper.AddSkinContainerControls(
                                        Globals.NavigateURL(this.TabId, "PPEnroll",
                                                            "Mid=" + Convert.ToString(this.ModuleId),
                                                            "ItemID=" + Convert.ToString(this.ItemId),
                                                            "NoEnrol=" + this.txtNoEnrolees.Text,
                                                            "groupid=" + socialGroupId,
                                                            "AnonEmail=" +
                                                            HttpUtility.UrlEncode(this.txtAnonEmail.Text),
                                                            "AnonName=" +
                                                            HttpUtility.UrlEncode(this.txtAnonName.Text),
                                                            "AnonPhone=" +
                                                            HttpUtility.UrlEncode(urlAnonTelephone)), "&"));
                            }
                            else
                            {
                                this.Response.Redirect(
                                    objEventInfoHelper.AddSkinContainerControls(
                                        Globals.NavigateURL(this.TabId, "PPEnroll",
                                                            "Mid=" + Convert.ToString(this.ModuleId),
                                                            "ItemID=" + Convert.ToString(this.ItemId),
                                                            "NoEnrol=" + this.txtNoEnrolees.Text,
                                                            "AnonEmail=" +
                                                            HttpUtility.UrlEncode(this.txtAnonEmail.Text),
                                                            "AnonName=" +
                                                            HttpUtility.UrlEncode(this.txtAnonName.Text),
                                                            "AnonPhone=" +
                                                            HttpUtility.UrlEncode(urlAnonTelephone)), "&"));
                            }
                        }
                    }
                    catch (Exception exc) //Module failed to load
                    {
                        Exceptions.ProcessModuleLoadException(this, exc);
                    }
                }
                else
                {
                    // Non-Paid Event Process
                    // Got the Event, Now Add the User to the EventSignups
                    var objEventSignups = new EventSignupsInfo();
                    objEventSignups.EventID = objEvent.EventID;

                    var startdate = objEvent.EventTimeBegin;
                    this.SelectedDate = startdate.Date;

                    objEventSignups.ModuleID = objEvent.ModuleID;
                    if (this.Request.IsAuthenticated)
                    {
                        objEventSignups.UserID = this.UserId;
                        objEventSignups.AnonEmail = null;
                        objEventSignups.AnonName = null;
                        objEventSignups.AnonTelephone = null;
                        objEventSignups.AnonCulture = null;
                        objEventSignups.AnonTimeZoneId = null;
                    }
                    else
                    {
                        var objSecurity = new PortalSecurity();
                        objEventSignups.UserID = -1;
                        objEventSignups.AnonEmail = this.txtAnonEmail.Text;
                        objEventSignups.AnonName =
                            objSecurity.InputFilter(this.txtAnonName.Text, PortalSecurity.FilterFlag.NoScripting);
                        objEventSignups.AnonTelephone =
                            objSecurity.InputFilter(this.txtAnonTelephone.Text, PortalSecurity.FilterFlag.NoScripting);
                        objEventSignups.AnonCulture = Thread.CurrentThread.CurrentCulture.Name;
                        objEventSignups.AnonTimeZoneId = this.GetDisplayTimeZoneId();
                    }
                    objEventSignups.PayPalPaymentDate = DateTime.UtcNow;
                    objEventSignups.NoEnrolees = int.Parse(this.txtNoEnrolees.Text);
                    if (this.IsModerator() ||
                        PortalSecurity.IsInRole(this.PortalSettings.AdministratorRoleName))
                    {
                        objEventSignups.Approved = true;
                        objEvent.Enrolled++;
                    }
                    else if (this.Settings.Moderateall)
                    {
                        objEventSignups.Approved = false;
                    }
                    else
                    {
                        objEventSignups.Approved = true;
                        objEvent.Enrolled++;
                    }
                    objEventSignups = this.CreateEnrollment(objEventSignups, objEvent);
                    this.enroll1.Visible = false;
                    var msgLevel = this.UserEnrollment(objEvent);
                    this.ShowMessage(this.lblSignup.Text, msgLevel);
                    // Send Moderator email
                    var objEventEmailInfo = new EventEmailInfo();
                    var objEventEmail = new EventEmails(this.PortalId, this.ModuleId, this.LocalResourceFile,
                                                        ((PageBase) this.Page).PageCulture.Name);
                    if (this.Settings.Moderateall && objEventSignups.Approved == false)
                    {
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
                        objEventEmail.SendEmails(objEventEmailInfo, objEvent, objEventSignups);
                    }

                    // Mail users
                    objEventEmailInfo = new EventEmailInfo();
                    objEventEmailInfo.TxtEmailSubject = this.Settings.Templates.txtEnrollMessageSubject;
                    objEventEmailInfo.TxtEmailFrom = this.Settings.StandardEmail;
                    if (this.Request.IsAuthenticated)
                    {
                        objEventEmailInfo.UserEmails.Add(this.PortalSettings.UserInfo.Email);
                        objEventEmailInfo.UserLocales.Add(this.PortalSettings.UserInfo.Profile.PreferredLocale);
                        objEventEmailInfo.UserTimeZoneIds.Add(this.PortalSettings.UserInfo.Profile.PreferredTimeZone
                                                                  .Id);
                    }
                    else
                    {
                        objEventEmailInfo.UserEmails.Add(objEventSignups.AnonEmail);
                        objEventEmailInfo.UserLocales.Add(objEventSignups.AnonCulture);
                        objEventEmailInfo.UserTimeZoneIds.Add(objEventSignups.AnonTimeZoneId);
                    }
                    objEventEmailInfo.UserIDs.Add(objEvent.OwnerID);
                    if (objEventSignups.Approved)
                    {
                        if (this.Settings.SendEnrollMessageApproved)
                        {
                            objEventEmailInfo.TxtEmailBody =
                                this.Settings.Templates.txtEmailMessage +
                                this.Settings.Templates.txtEnrollMessageApproved;
                            objEventEmail.SendEmails(objEventEmailInfo, objEvent, objEventSignups);
                        }
                    }
                    else
                    {
                        if (this.Settings.SendEnrollMessageWaiting)
                        {
                            objEventEmailInfo.TxtEmailBody =
                                this.Settings.Templates.txtEmailMessage +
                                this.Settings.Templates.txtEnrollMessageWaiting;
                            objEventEmail.SendEmails(objEventEmailInfo, objEvent, objEventSignups);
                        }
                    }
                }
                this.BindEnrollList(objEvent);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        ///     When notify button is clicked the user will be added to the notification list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        protected void cmdNotify_Click(object sender, EventArgs e)
        {
            this.valEmail.Validate();
            this.valEmail2.Validate();
            if (!this.valEmail.IsValid || !this.valEmail2.IsValid)
            {
                return;
            }

            var lstEvents = new ArrayList();
            var objEventNotification = default(EventNotificationInfo);
            var objEventNotificationController = new EventNotificationController();
            var eventDate = default(DateTime);
            var notifyTime = default(DateTime);
            int currentEv;
            var objEventTimeZoneUtilities = new EventTimeZoneUtilities();
            try
            {
                var eventEvent = default(EventInfo);
                var objCtlEvent = new EventController();
                eventEvent = objCtlEvent.EventsGet(this.ItemId, this.ModuleId);
                currentEv = eventEvent.EventID;

                if (this.chkReminderRec.Checked)
                {
                    lstEvents = objCtlEvent.EventsGetRecurrences(eventEvent.RecurMasterID, this.ModuleId);
                }
                else
                {
                    lstEvents.Add(eventEvent);
                }

                foreach (EventInfo tempLoopVar_eventEvent in lstEvents)
                {
                    eventEvent = tempLoopVar_eventEvent;
                    if (eventEvent.SendReminder && eventEvent.EventTimeBegin >
                        objEventTimeZoneUtilities.ConvertFromUTCToModuleTimeZone(
                            DateTime.UtcNow, eventEvent.EventTimeZoneId))
                    {
                        eventDate = objEventTimeZoneUtilities.ConvertToUTCTimeZone(
                            eventEvent.EventTimeBegin, eventEvent.EventTimeZoneId);
                        objEventNotification =
                            objEventNotificationController.EventsNotificationGet(
                                eventEvent.EventID, this.txtUserEmail.Text, this.ModuleId);

                        notifyTime = eventDate;
                        //*** Calculate notification time
                        switch (this.ddlReminderTimeMeasurement.SelectedValue)
                        {
                            case "m":
                                notifyTime = notifyTime.AddMinutes(int.Parse(this.txtReminderTime.Text) * -1);
                                break;
                            case "h":
                                notifyTime = notifyTime.AddHours(int.Parse(this.txtReminderTime.Text) * -1);
                                break;
                            case "d":
                                notifyTime = notifyTime.AddDays(int.Parse(this.txtReminderTime.Text) * -1);
                                break;
                        }
                        // Registered users will overwrite existing notifications (in recurring events)
                        var notifyDisplayTime = objEventTimeZoneUtilities
                            .ConvertFromUTCToDisplayTimeZone(notifyTime, this.GetDisplayTimeZoneId()).EventDate;
                        if (!ReferenceEquals(objEventNotification, null) && this.Request.IsAuthenticated)
                        {
                            objEventNotification.NotifyByDateTime = notifyTime;
                            objEventNotificationController.EventsNotificationSave(objEventNotification);
                            if (currentEv == eventEvent.EventID)
                            {
                                this.lblConfirmation.Text =
                                    string.Format(
                                        Localization.GetString("lblReminderConfirmation", this.LocalResourceFile),
                                        notifyDisplayTime);
                                this.ShowMessage(this.lblConfirmation.Text, MessageLevel.DNNSuccess);
                            }
                            // Anonymous users can never overwrite an existing notification
                        }
                        else if (!ReferenceEquals(objEventNotification, null))
                        {
                            if (currentEv == eventEvent.EventID)
                            {
                                this.lblConfirmation.Text =
                                    string.Format(Localization.GetString("ReminderAlreadyReg", this.LocalResourceFile),
                                                  this.txtUserEmail.Text, objEventNotification.NotifyByDateTime);
                                this.ShowMessage(this.lblConfirmation.Text, MessageLevel.DNNWarning);
                            }
                        }
                        else
                        {
                            objEventNotification = new EventNotificationInfo();
                            objEventNotification.NotificationID = -1;
                            objEventNotification.EventID = eventEvent.EventID;
                            objEventNotification.PortalAliasID = this.PortalAlias.PortalAliasID;
                            objEventNotification.NotificationSent = false;
                            objEventNotification.EventTimeBegin = eventDate;
                            objEventNotification.NotifyLanguage = Thread.CurrentThread.CurrentCulture.Name;
                            objEventNotification.ModuleID = this.ModuleId;
                            objEventNotification.TabID = this.TabId;
                            objEventNotification.NotifyByDateTime = notifyTime;
                            objEventNotification.UserEmail = this.txtUserEmail.Text;
                            objEventNotificationController.EventsNotificationSave(objEventNotification);
                            if (currentEv == eventEvent.EventID)
                            {
                                this.lblConfirmation.Text =
                                    string.Format(
                                        Localization.GetString("lblReminderConfirmation", this.LocalResourceFile),
                                        notifyDisplayTime);
                                this.ShowMessage(this.lblConfirmation.Text, MessageLevel.DNNSuccess);
                            }
                        }
                    }
                }
                this.rem1.Visible = false;
                this.rem2.Visible = false;
                this.rem3.Visible = true;
                this.imgConfirmation.AlternateText = Localization.GetString("Reminder", this.LocalResourceFile);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdPrint_PreRender(object sender, EventArgs e)
        {
            this.cmdPrint.Target = " _blank";
            this.cmdPrint.NavigateUrl = Globals.NavigateURL(this.TabId, this.PortalSettings, "",
                                                            "mid=" + Convert.ToString(this.ModuleId),
                                                            "itemid=" + Convert.ToString(this.ItemId), "ctl=Details",
                                                            "ShowNav=False", "dnnprintmode=true",
                                                            "SkinSrc=%5bG%5dSkins%2f_default%2fNo+Skin",
                                                            "ContainerSrc=%5bG%5dContainers%2f_default%2fNo+Container");
        }

        protected void cmdEmail_Click(object sender, EventArgs e)
        {
            this.valEmailiCal.Validate();
            this.valEmailiCal2.Validate();
            if (!this.valEmailiCal.IsValid || !this.valEmailiCal2.IsValid)
            {
                return;
            }

            var objCtlEvent = new EventController();
            var objEvent = objCtlEvent.EventsGet(this.ItemId, this.ModuleId);
            if (ReferenceEquals(objEvent, null))
            {
                return;
            }

            var iCalendar = new VEvent(false, HttpContext.Current);
            var iCal = "";
            iCal = iCalendar.CreateiCal(this.TabId, this.ModuleId, this.ItemId, objEvent.SocialGroupId);

            var attachment = Attachment.CreateAttachmentFromString(iCal, new ContentType("text/calendar"));
            attachment.TransferEncoding = TransferEncoding.Base64;
            attachment.Name = objEvent.EventName + ".ics";
            var attachments = new List<Attachment>();
            attachments.Add(attachment);

            var objEventEmailInfo = new EventEmailInfo();
            var objEventEmail = new EventEmails(this.PortalId, this.ModuleId, this.LocalResourceFile,
                                                ((PageBase) this.Page).PageCulture.Name);
            objEventEmailInfo.TxtEmailSubject = this.Settings.Templates.EventiCalSubject;
            objEventEmailInfo.TxtEmailBody = this.Settings.Templates.EventiCalBody;
            objEventEmailInfo.TxtEmailFrom = this.Settings.StandardEmail;
            objEventEmailInfo.UserEmails.Add(this.txtUserEmailiCal.Text);
            objEventEmailInfo.UserLocales.Add("");
            objEventEmailInfo.UserTimeZoneIds.Add(objEvent.EventTimeZoneId);

            objEventEmail.SendEmails(objEventEmailInfo, objEvent, attachments);
            this.divMessage.Attributes.Add("style", "display:block;");
            this.ShowMessage(
                string.Format(Localization.GetString("ConfirmationiCal", this.LocalResourceFile),
                              this.txtUserEmailiCal.Text), MessageLevel.DNNSuccess);
        }

        #endregion
    }
}