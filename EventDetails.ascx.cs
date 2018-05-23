using DotNetNuke.Services.Exceptions;
using System.Diagnostics;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using System.Collections;
using DotNetNuke.Common.Utilities;
using System.Web;
using DotNetNuke.Services.Localization;
using System;
using DotNetNuke.Security;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.IO;
using DotNetNuke.Entities.Profile;
using System.Linq;


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
    using global::Components;

    [DNNtc.ModuleControlProperties("Details", "Events Details", DNNtc.ControlType.View, "https://dnnevents.codeplex.com/documentation", true, false)]
    public partial class EventDetails : EventBase
    {

        #region  Web Form Designer Generated Code
        //This call is required by the Web Form Designer.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
        }

        private void Page_Init(System.Object sender, EventArgs e)
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Stores the ItemId in the viewstate
        /// </summary>
        private int ItemId
        {
            get
            {
                return System.Convert.ToInt32(ViewState["EventItemID" + ModuleId.ToString()]);
            }
            set
            {
                ViewState["EventItemID" + ModuleId.ToString()] = value.ToString();
            }
        }
        private EventInfo _eventInfo = new EventInfo();

        private enum MessageLevel : int
        {
            DNNSuccess = 1,
            DNNInformation,
            DNNWarning,
            DNNError
        }

        #endregion

        #region Event Handlers
        private void Page_Load(System.Object sender, EventArgs e)
        {
            // Log exception whem status is filled
            if (!(Request.Params["status"] == null))
            {
                PortalSecurity objSecurity = new PortalSecurity();
                string status = objSecurity.InputFilter(Request.Params["status"], (DotNetNuke.Security.PortalSecurity.FilterFlag)((int)(PortalSecurity.FilterFlag.NoScripting) | (int)PortalSecurity.FilterFlag.NoMarkup));
                Exceptions.LogException(new ModuleLoadException("EventDetails Call...status: " + status));
            }

            // Add the external Validation.js to the Page
            const string csname = "ExtValidationScriptFile";
            Type cstype = System.Reflection.MethodBase.GetCurrentMethod().GetType();
            string cstext = "<script src=\"" + ResolveUrl("~/DesktopModules/Events/Scripts/Validation.js") + "\" type=\"text/javascript\"></script>";
            if (!Page.ClientScript.IsClientScriptBlockRegistered(csname))
            {
                Page.ClientScript.RegisterClientScriptBlock(cstype, csname, cstext, false);
            }

            // Force full PostBack since these pass off to aspx page
            if (AJAX.IsInstalled())
            {
                AJAX.RegisterPostBackControl(cmdvEvent);
                AJAX.RegisterPostBackControl(cmdvEventSeries);
                AJAX.RegisterPostBackControl(cmdvEventSignups);
            }

            cmdvEvent.ToolTip = Localization.GetString("cmdvEventTooltip", LocalResourceFile);
            cmdvEvent.Text = Localization.GetString("cmdvEventExport", LocalResourceFile);
            cmdvEventSeries.ToolTip = Localization.GetString("cmdvEventSeriesTooltip", LocalResourceFile);
            cmdvEventSeries.Text = Localization.GetString("cmdvEventExportSeries", LocalResourceFile);
            cmdvEventSignups.ToolTip = Localization.GetString("cmdvEventSignupsTooltip", LocalResourceFile);
            cmdvEventSignups.Text = Localization.GetString("cmdvEventSignupsDownload", LocalResourceFile);

            cmdPrint.ToolTip = Localization.GetString("Print", LocalResourceFile);

            try
            {
                //Get the item id of the selected event
                if (!(ReferenceEquals(Request.Params["ItemId"], null)))
                {
                    ItemId = int.Parse(Request.Params["ItemId"]);
                }
                else
                {
                    Response.Redirect(GetSocialNavigateUrl(), true);
                }

                // Set the selected theme
                if (Settings.Eventdetailnewpage)
                {
                    SetTheme(pnlEventsModuleDetails);
                    AddFacebookMetaTags();
                }

                // If the page is being requested the first time, determine if an
                // contact itemId value is specified, and if so populate page
                // contents with the contact details
                if (Page.IsPostBack)
                {
                    return;
                }


                EventController objCtlEvent = new EventController();
                _eventInfo = objCtlEvent.EventsGet(ItemId, ModuleId);

                //If somebody has sent a bad ItemID and eventinfo not retrieved, return 301
                if (ReferenceEquals(_eventInfo, null))
                {
                    Response.StatusCode = 301;
                    Response.AppendHeader("Location", GetSocialNavigateUrl());
                    return;
                }

                // Do they have permissions to the event
                EventInfoHelper objCtlEventInfoHelper = new EventInfoHelper(ModuleId, Settings);
                if (Settings.Enforcesubcalperms && !objCtlEventInfoHelper.IsModuleViewer(_eventInfo.ModuleID))
                {
                    Response.Redirect(GetSocialNavigateUrl(), true);
                }
                else if (IsPrivateNotModerator && UserId != _eventInfo.OwnerID)
                {
                    Response.Redirect(GetSocialNavigateUrl(), true);
                }
                else if (Settings.SocialGroupModule == EventModuleSettings.SocialModule.UserProfile & !objCtlEventInfoHelper.IsSocialUserPublic(GetUrlUserId()))
                {
                    Response.Redirect(GetSocialNavigateUrl(), true);
                }
                else if (Settings.SocialGroupModule == EventModuleSettings.SocialModule.SocialGroup & !objCtlEventInfoHelper.IsSocialGroupPublic(GetUrlGroupId()))
                {
                    Response.Redirect(GetSocialNavigateUrl(), true);
                }

                // Has the event been cancelled
                if (_eventInfo.Cancelled)
                {
                    Response.StatusCode = 301;
                    Response.AppendHeader("Location", GetSocialNavigateUrl());
                    return;
                }

                // So we have a valid item, but is it from a module that has been deleted
                // but not removed from the recycle bin
                if (_eventInfo.ModuleID != ModuleId)
                {
                    Entities.Modules.ModuleController objCtlModule = new Entities.Modules.ModuleController();
                    ArrayList objModules = (ArrayList)(objCtlModule.GetModuleTabs(_eventInfo.ModuleID));
                    Entities.Modules.ModuleInfo objModule = default(Entities.Modules.ModuleInfo);
                    bool isDeleted = true;
                    foreach (Entities.Modules.ModuleInfo tempLoopVar_objModule in objModules)
                    {
                        objModule = tempLoopVar_objModule;
                        if (!objModule.IsDeleted)
                        {
                            isDeleted = false;
                        }
                    }
                    if (isDeleted)
                    {
                        Response.StatusCode = 301;
                        Response.AppendHeader("Location", GetSocialNavigateUrl());
                        return;
                    }
                }

                // Should all be OK to display now
                string displayTimeZoneId = _eventInfo.EventTimeZoneId;
                if (!Settings.EnableEventTimeZones)
                {
                    displayTimeZoneId = GetDisplayTimeZoneId();
                }
                EventInfoHelper objEventInfoHelper = new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
                _eventInfo = objEventInfoHelper.ConvertEventToDisplayTimeZone(_eventInfo, displayTimeZoneId);

                TokenReplaceControllerClass tcc = new TokenReplaceControllerClass(ModuleId, LocalResourceFile);

                //Set the page title
                if (Settings.EnableSEO)
                {
                    string txtPageText = string.Format(Settings.Templates.txtSEOPageTitle, BasePage.Title);
                    txtPageText = tcc.TokenReplaceEvent(_eventInfo, txtPageText, false);
                    txtPageText = HttpUtility.HtmlDecode(txtPageText);
                    txtPageText = HtmlUtils.StripTags(txtPageText, true);
                    txtPageText = txtPageText.Replace(Environment.NewLine, " ");
                    txtPageText = HtmlUtils.StripWhiteSpace(txtPageText, true);
                    BasePage.Title = txtPageText;
                    txtPageText = string.Format(Settings.Templates.txtSEOPageDescription, BasePage.Description);
                    txtPageText = tcc.TokenReplaceEvent(_eventInfo, txtPageText, false);
                    txtPageText = HttpUtility.HtmlDecode(txtPageText);
                    txtPageText = HtmlUtils.StripTags(txtPageText, true);
                    txtPageText = txtPageText.Replace(Environment.NewLine, " ");
                    txtPageText = HtmlUtils.StripWhiteSpace(txtPageText, true);
                    txtPageText = HtmlUtils.Shorten(txtPageText, Settings.SEODescriptionLength, "...");
                    BasePage.Description = txtPageText;
                    txtPageText = BasePage.KeyWords;
                    if (!ReferenceEquals(_eventInfo.LocationName, null))
                    {
                        if (!string.IsNullOrEmpty(txtPageText))
                        {
                            txtPageText = txtPageText + ",";
                        }
                        txtPageText = txtPageText + _eventInfo.LocationName;
                    }
                    if (!ReferenceEquals(_eventInfo.CategoryName, null))
                    {
                        if (!string.IsNullOrEmpty(txtPageText))
                        {
                            txtPageText = txtPageText + ",";
                        }
                        txtPageText = txtPageText + _eventInfo.CategoryName;
                    }
                    BasePage.KeyWords = txtPageText;
                }

                //Replace tokens
                string txtTemplate = "";
                string txtTemplate1 = "";
                string txtTemplate2 = "";
                string txtTemplate3 = "";
                string txtTemplate4 = "";
                txtTemplate = Settings.Templates.EventDetailsTemplate;
                txtTemplate1 = txtTemplate;
                txtTemplate2 = "";
                txtTemplate3 = "";
                txtTemplate4 = "";
                int nTemplate = 0;
                while (txtTemplate.IndexOf("[BREAK]") + 1 > 0)
                {
                    nTemplate++;
                    string txtBefore = "";
                    int nBreak = txtTemplate.IndexOf("[BREAK]") + 1;
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
                    divEventDetails1.InnerHtml = tcc.TokenReplaceEvent(_eventInfo, txtTemplate1);
                    divEventDetails1.Attributes.Add("style", "display:block;");
                }
                else
                {
                    divEventDetails1.Attributes.Add("style", "display:none;");
                }
                if (!string.IsNullOrEmpty(txtTemplate2) && txtTemplate2 != "\r\n")
                {
                    divEventDetails2.InnerHtml = tcc.TokenReplaceEvent(_eventInfo, txtTemplate2);
                    divEventDetails2.Attributes.Add("style", "display:block;");
                }
                else
                {
                    divEventDetails2.Attributes.Add("style", "display:none;");
                }
                if (!string.IsNullOrEmpty(txtTemplate3) && txtTemplate3 != "\r\n")
                {
                    divEventDetails3.InnerHtml = tcc.TokenReplaceEvent(_eventInfo, txtTemplate3);
                    divEventDetails3.Attributes.Add("style", "display:block;");
                }
                else
                {
                    divEventDetails3.Attributes.Add("style", "display:none;");
                }
                if (!string.IsNullOrEmpty(txtTemplate4) && txtTemplate4 != "\r\n")
                {
                    divEventDetails4.InnerHtml = tcc.TokenReplaceEvent(_eventInfo, txtTemplate4);
                    divEventDetails4.Attributes.Add("style", "display:block;");
                }
                else
                {
                    divEventDetails4.Attributes.Add("style", "display:none;");
                }

                editButton.Visible = false;
                deleteButton.Visible = false;
                cmdvEventSignups.Visible = false;
                if (IsEventEditor(_eventInfo, false))
                {
                    editButton.Visible = true;
                    editButton.NavigateUrl = objEventInfoHelper.GetEditURL(_eventInfo.EventID, _eventInfo.SocialGroupId, _eventInfo.SocialUserId);
                    editButton.ToolTip = Localization.GetString("editButton", LocalResourceFile);
                    deleteButton.Visible = true;
                    deleteButton.Attributes.Add("onclick", "javascript:return confirm('" + Localization.GetString("ConfirmEventDelete", LocalResourceFile) + "');");
                    cmdvEventSignups.Visible = true;
                }
                editSeriesButton.Visible = false;
                deleteSeriesButton.Visible = false;
                if (_eventInfo.RRULE != "")
                {
                    // Note that IsEventEditor with 'True' is used here because this is for the
                    // series(buttons)and excludes single event owner. Must be recurrence master owner.
                    if (IsEventEditor(_eventInfo, true))
                    {
                        editSeriesButton.Visible = true;
                        editSeriesButton.NavigateUrl = objEventInfoHelper.GetEditURL(_eventInfo.EventID, _eventInfo.SocialGroupId, _eventInfo.SocialUserId, "All");
                        editSeriesButton.ToolTip = Localization.GetString("editSeriesButton", LocalResourceFile);
                        deleteSeriesButton.Visible = true;
                        deleteSeriesButton.Attributes.Add("onclick", "javascript:return confirm('" + Localization.GetString("ConfirmEventSeriesDelete", LocalResourceFile) + "');");
                    }
                }
                if (_eventInfo.RRULE == "")
                {
                    cmdvEventSeries.Visible = false;
                }

                EventTimeZoneUtilities objEventTimeZoneUtilities = new EventTimeZoneUtilities();
                DateTime nowDisplay = objEventTimeZoneUtilities.ConvertFromUTCToDisplayTimeZone(DateTime.UtcNow, GetDisplayTimeZoneId()).EventDate;

                //  Compute Dates/Times (for recurring)
                DateTime startdate = _eventInfo.EventTimeBegin;
                SelectedDate = startdate.Date;

                // See if user already are signed up
                // And that Signup is Authorized
                // And also that the Date/Time has not passed
                divEnrollment.Attributes.Add("style", "display:none;");
                if (_eventInfo.Signups)
                {
                    if (startdate > nowDisplay)
                    {
                        if (_eventInfo.EnrollRoleID == null || _eventInfo.EnrollRoleID == -1)
                        {
                            UserEnrollment(_eventInfo);
                        }
                        else
                        {
                            EventSignupsController objEventSignupsController = new EventSignupsController();
                            if (objEventSignupsController.IsEnrollRole(_eventInfo.EnrollRoleID, PortalId))
                            {
                                UserEnrollment(_eventInfo);
                            }
                        }
                    }
                    else
                    {
                        divEnrollment.Attributes.Add("style", "display:block;");
                        lblEnrollTooLate.Text = Localization.GetString("EnrollTooLate", LocalResourceFile);
                        enroll4.Visible = true;
                    }
                }

                //Are You Sure You Want To Enroll?'
                if (Request.IsAuthenticated)
                {
                    if (Settings.Enableenrollpopup)
                    {
                        cmdSignup.Attributes.Add("onclick", "javascript:return confirm('" + Localization.GetString("SureYouWantToEnroll", LocalResourceFile) + "');");
                    }
                    valNoEnrolees.MaximumValue = Convert.ToString(_eventInfo.MaxEnrollment - _eventInfo.Enrolled);
                    if (int.Parse(valNoEnrolees.MaximumValue) > Settings.Maxnoenrolees | _eventInfo.MaxEnrollment == 0)
                    {
                        valNoEnrolees.MaximumValue = Settings.Maxnoenrolees.ToString();
                    }
                    lblMaxNoEnrolees.Text = string.Format(Localization.GetString("lblMaxNoEnrolees", LocalResourceFile), valNoEnrolees.MaximumValue);
                    valNoEnrolees.ErrorMessage = string.Format(Localization.GetString("valNoEnrolees", LocalResourceFile), int.Parse(valNoEnrolees.MaximumValue));
                    valNoEnrolees2.ErrorMessage = valNoEnrolees.ErrorMessage;
                }

                divMessage.Attributes.Add("style", "display:none;");

                if (Settings.IcalEmailEnable)
                {
                    divIcalendar.Attributes.Add("style", "display:block;");
                    txtUserEmailiCal.Text = UserInfo.Email;
                    if (Request.IsAuthenticated)
                    {
                        txtUserEmailiCal.Enabled = false;
                    }
                    else
                    {
                        txtUserEmailiCal.Enabled = true;
                    }
                }
                else
                {
                    divIcalendar.Attributes.Add("style", "display:none;");
                }

                //Is notification enabled
                divReminder.Attributes.Add("style", "display:none;");
                if (_eventInfo.SendReminder && startdate > nowDisplay)
                {
                    //Is registered user
                    if (Request.IsAuthenticated)
                    {
                        divReminder.Attributes.Add("style", "display:block;");
                        EventNotificationController objEventNotificationController = new EventNotificationController();
                        string notificationInfo = objEventNotificationController.NotifyInfo(_eventInfo.EventID, UserInfo.Email, ModuleId, LocalResourceFile, GetDisplayTimeZoneId());
                        if (!string.IsNullOrEmpty(notificationInfo))
                        {
                            lblConfirmation.Text = notificationInfo;
                            rem3.Visible = true;
                            imgConfirmation.AlternateText = Localization.GetString("Reminder", LocalResourceFile);
                        }
                        else
                        {
                            txtUserEmail.Text = UserInfo.Email;
                        }
                    }
                    // is anonymous notification allowed or registered user not yet notified
                    if ((Settings.Notifyanon && !Request.IsAuthenticated) || txtUserEmail.Text.Length > 0)
                    {
                        if (Request.IsAuthenticated)
                        {
                            txtUserEmail.Enabled = false;
                        }
                        else
                        {
                            txtUserEmail.Enabled = true;
                        }
                        divReminder.Attributes.Add("style", "display:block;");
                        string errorminutes = Localization.GetString("invalidReminderMinutes", LocalResourceFile);
                        string errorhours = Localization.GetString("invalidReminderHours", LocalResourceFile);
                        string errordays = Localization.GetString("invalidReminderDays", LocalResourceFile);

                        txtReminderTime.Text = _eventInfo.ReminderTime.ToString(); //load default Reminder Time of event
                        txtReminderTime.Visible = true;
                        ddlReminderTimeMeasurement.Attributes.Add("onchange", "valRemTime('" + valReminderTime.ClientID + "','" + valReminderTime2.ClientID + "','" + valReminderTime.ValidationGroup + "','" + ddlReminderTimeMeasurement.ClientID + "','" + errorminutes + "','" + errorhours + "','" + errordays + "');");
                        ddlReminderTimeMeasurement.Visible = true;
                        ddlReminderTimeMeasurement.SelectedValue = _eventInfo.ReminderTimeMeasurement;

                        switch (_eventInfo.ReminderTimeMeasurement)
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

                        rem1.Visible = true;
                        imgNotify.AlternateText = Localization.GetString("Reminder", LocalResourceFile);
                        rem2.Visible = true;
                        if (_eventInfo.RRULE != "")
                        {
                            chkReminderRec.Visible = true;
                        }
                    }
                }
                grdEnrollment.Columns[0].HeaderText = Localization.GetString("EnrollUserName", LocalResourceFile);
                grdEnrollment.Columns[1].HeaderText = Localization.GetString("EnrollDisplayName", LocalResourceFile);
                grdEnrollment.Columns[2].HeaderText = Localization.GetString("EnrollEmail", LocalResourceFile);
                grdEnrollment.Columns[3].HeaderText = Localization.GetString("EnrollPhone", LocalResourceFile);
                grdEnrollment.Columns[4].HeaderText = Localization.GetString("EnrollApproved", LocalResourceFile);
                grdEnrollment.Columns[5].HeaderText = Localization.GetString("EnrollNo", LocalResourceFile);

                BindEnrollList(_eventInfo);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Bind enrolled users to the enrollment grid
        /// </summary>
        private void BindEnrollList(EventInfo eventInfo)
        {
            divEnrollList.Attributes.Add("style", "display:none;");
            bool blEnrollList = false;
            string txtColumns = "";
            if (eventInfo.Signups && eventInfo.Enrolled > 0)
            {
                txtColumns = EnrolmentColumns(eventInfo, eventInfo.EnrollListView);
            }
            if (!string.IsNullOrEmpty(txtColumns))
            {
                blEnrollList = true;
            }

            if (blEnrollList)
            {
                if (txtColumns.LastIndexOf("UserName", StringComparison.Ordinal) < 0)
                {
                    grdEnrollment.Columns[0].Visible = false;
                }
                if (txtColumns.LastIndexOf("DisplayName", StringComparison.Ordinal) < 0)
                {
                    grdEnrollment.Columns[1].Visible = false;
                }
                if (txtColumns.LastIndexOf("Email", StringComparison.Ordinal) < 0)
                {
                    grdEnrollment.Columns[2].Visible = false;
                }
                if (txtColumns.LastIndexOf("Phone", StringComparison.Ordinal) < 0)
                {
                    grdEnrollment.Columns[3].Visible = false;
                }
                if (txtColumns.LastIndexOf("Approved", StringComparison.Ordinal) < 0)
                {
                    grdEnrollment.Columns[4].Visible = false;
                }
                if (txtColumns.LastIndexOf("Qty", StringComparison.Ordinal) < 0)
                {
                    grdEnrollment.Columns[5].Visible = false;
                }

                //Load enrol list
                ArrayList eventEnrollment = new ArrayList();
                ArrayList objSignups = default(ArrayList);
                EventSignupsInfo objSignup = default(EventSignupsInfo);
                UserController objCtlUser = new UserController();
                EventSignupsController objCtlEventSignups = new EventSignupsController();
                objSignups = objCtlEventSignups.EventsSignupsGetEvent(eventInfo.EventID, ModuleId);
                foreach (EventSignupsInfo tempLoopVar_objSignup in objSignups)
                {
                    objSignup = tempLoopVar_objSignup;
                    EventEnrollList objEnrollListItem = new EventEnrollList();
                    if (objSignup.UserID != -1)
                    {
                        UserInfo objUser = default(UserInfo);
                        objUser = objCtlUser.GetUser(PortalId, objSignup.UserID);
                        EventInfoHelper objEventInfoHelper = new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
                        objEnrollListItem.EnrollDisplayName = objEventInfoHelper.UserDisplayNameProfile(objSignup.UserID, objSignup.UserName, LocalResourceFile).DisplayNameURL;
                        if (!ReferenceEquals(objUser, null))
                        {
                            objEnrollListItem.EnrollUserName = objUser.Username;
                            objEnrollListItem.EnrollEmail = string.Format("<a href=\"mailto:{0}?subject={1}\">{0}</a>", objSignup.Email, eventInfo.EventName);
                            objEnrollListItem.EnrollPhone = objUser.Profile.Telephone;
                        }
                    }
                    else
                    {
                        objEnrollListItem.EnrollDisplayName = objSignup.AnonName;
                        objEnrollListItem.EnrollUserName = Localization.GetString("AnonUser", LocalResourceFile);
                        objEnrollListItem.EnrollEmail = string.Format("<a href=\"mailto:{0}?subject={1}\">{0}</a>", objSignup.AnonEmail, eventInfo.EventName);
                        objEnrollListItem.EnrollPhone = objSignup.AnonTelephone;
                    }
                    objEnrollListItem.SignupID = objSignup.SignupID;
                    objEnrollListItem.EnrollApproved = objSignup.Approved;
                    objEnrollListItem.EnrollNo = objSignup.NoEnrolees;
                    eventEnrollment.Add(objEnrollListItem);
                }
                if (eventEnrollment.Count > 0)
                {
                    divEnrollList.Attributes.Add("style", "display:block;");
                    grdEnrollment.DataSource = eventEnrollment;
                    grdEnrollment.DataBind();
                }
            }
        }

        /// <summary>
        /// Display User Enrollment Info on Page
        /// </summary>
        private MessageLevel UserEnrollment(EventInfo eventInfo)
        {
            MessageLevel returnValue = default(MessageLevel);
            returnValue = MessageLevel.DNNSuccess;
            if (!Settings.Eventsignup)
            {
                divEnrollment.Attributes.Add("style", "display:none;");
                return returnValue;
            }

            divEnrollment.Attributes.Add("style", "display:block;");
            enroll3.Visible = false;
            enroll5.Visible = false;
            if (!ReferenceEquals(Request.Params["Status"], null))
            {
                if (Request.Params["Status"].ToLower() == "enrolled")
                {
                    // User has been successfully enrolled for this event (paid enrollment)
                    lblSignup.Text = Localization.GetString("StatusPPSuccess", LocalResourceFile);
                    enroll2.Visible = true;
                    imgSignup.AlternateText = Localization.GetString("StatusPPSuccess", LocalResourceFile);
                }
                else if (Request.Params["Status"].ToLower() == "cancelled")
                {
                    // User has been cancelled paid enrollment
                    lblSignup.Text = Localization.GetString("StatusPPCancelled", LocalResourceFile);
                    returnValue = MessageLevel.DNNInformation;
                    enroll2.Visible = true;
                    imgSignup.AlternateText = Localization.GetString("StatusPPCancelled", LocalResourceFile);
                }
                return returnValue;
            }

            // If not authenticated and anonymous not allowed setup for logintoenroll
            if (!Request.IsAuthenticated && !eventInfo.AllowAnonEnroll)
            {
                enroll1.Visible = true;
                imgEnroll.AlternateText = Localization.GetString("LoginToEnroll", LocalResourceFile);
                cmdSignup.Text = Localization.GetString("LoginToEnroll", LocalResourceFile);
                return returnValue;
            }

            // If not authenticated make email/name boxes visible, or find out if authenticated user has already enrolled
            EventSignupsController objCtlEventSignups = new EventSignupsController();
            EventSignupsInfo objEventSignups = null;
            if (!Request.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(txtAnonEmail.Text))
                {
                    objEventSignups = objCtlEventSignups.EventsSignupsGetAnonUser(eventInfo.EventID, txtAnonEmail.Text, ModuleId);
                }
            }
            else
            {
                objEventSignups = objCtlEventSignups.EventsSignupsGetUser(eventInfo.EventID, UserId, ModuleId);
            }

            if (ReferenceEquals(objEventSignups, null))
            {
                if (!Request.IsAuthenticated && !Settings.EnrollmentPageAllowed)
                {
                    enroll5.Visible = true;
                }
                if ((eventInfo.Enrolled < eventInfo.MaxEnrollment) ||
                    (eventInfo.MaxEnrollment == 0))
                {
                    if (Settings.Maxnoenrolees > 1 && !Settings.EnrollmentPageAllowed)
                    {
                        enroll3.Visible = true;
                    }
                    // User is not enrolled for this event...press the link to enroll!
                    enroll1.Visible = true;
                    imgEnroll.AlternateText = Localization.GetString("EnrollForEvent", LocalResourceFile);
                    cmdSignup.Text = Localization.GetString("EnrollForEvent", LocalResourceFile);
                }
            }
            else
            {
                enroll2.Visible = true;
                if (objEventSignups.Approved)
                {
                    // User is enrolled and approved for this event!
                    imgSignup.AlternateText = Localization.GetString("YouAreEnrolledForThisEvent", LocalResourceFile);
                    lblSignup.Text = Localization.GetString("YouAreEnrolledForThisEvent", LocalResourceFile);
                    returnValue = MessageLevel.DNNSuccess;
                }
                else
                {
                    // User is enrolled for this event, but not yet approved!
                    imgSignup.AlternateText = Localization.GetString("EnrolledButNotApproved", LocalResourceFile);
                    lblSignup.Text = Localization.GetString("EnrolledButNotApproved", LocalResourceFile);
                    returnValue = MessageLevel.DNNWarning;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Redirect to EventVCal page which exports the event to a vCard
        /// </summary>
        private void ExportEvent(bool series)
        {
            try
            {
                Response.Redirect("~/DesktopModules/Events/EventVCal.aspx?ItemID=" + System.Convert.ToString(ItemId) + "&Mid=" + System.Convert.ToString(ModuleId) + "&tabid=" + System.Convert.ToString(TabId) + "&Series=" + System.Convert.ToString(series));
            }
            catch (System.Threading.ThreadAbortException)
            {
                //Ignore
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Download the enrollees for this event as a file.
        /// </summary>
        private void DownloadSignups()
        {
            //Get the event.
            EventInfo theEvent = new EventController().EventsGet(ItemId, ModuleId);
            if (theEvent != null)
            {
                //Dim xmlDoc As XmlDocument = DefineXmlFile(theEvent, True)
                //If xmlDoc IsNot Nothing Then
                //    GenerateXmlFile(theEvent, xmlDoc)
                //End If

                string csvDoc = DefineCsvFile(theEvent);
                if (csvDoc != null)
                {
                    GenerateCsvFile(theEvent, csvDoc);
                }
            }
        }

        /// <summary>
        /// Define the xml file for downloading the enrollees for this event.
        /// </summary>
        private XmlDocument DefineXmlFile(EventInfo theEvent, bool localizeTags)
        {
            XmlDocument returnValue = default(XmlDocument);
            returnValue = null;
            XmlDocument xmlDoc = new XmlDocument();

            //Get the enrollees.
            ArrayList eventSignups = new EventSignupsController().EventsSignupsGetEvent(ItemId, ModuleId);
            //Anything to do
            if (ReferenceEquals(eventSignups, null) || eventSignups.Count == 0)
            {
                return returnValue;
            }

            try
            {
                //Initialization.
                XNamespace xNamespace = XNamespace.Get("");
                XElement xRoot =
                    new XElement(xNamespace + "Document", new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"));

                //Tags should be localized for XML output. These will be converted to a header row in Excel.
                //Tags should not be localized for CSV output. A separate localized header row is added.
                XElement xElemHdr = default(XElement);
                if (localizeTags)
                {
                    xElemHdr = new XElement("Enrollee",
                        new XElement("EventName", Localization.GetString("Event Name.Header", LocalResourceFile)),
                        new XElement("EventStart", Localization.GetString("Event Start.Header", LocalResourceFile)),
                        new XElement("EventEnd", Localization.GetString("Event End.Header", LocalResourceFile)),
                        new XElement("Location", Localization.GetString("Location.Header", LocalResourceFile)),
                        new XElement("Category", Localization.GetString("Category.Header", LocalResourceFile)),
                        new XElement("ReferenceNumber", Localization.GetString("ReferenceNumber.Header", LocalResourceFile)),
                        new XElement("Company", Localization.GetString("Company.Header", LocalResourceFile)),
                        new XElement("JobTitle", Localization.GetString("JobTitle.Header", LocalResourceFile)),
                        new XElement("FullName", Localization.GetString("FullName.Header", LocalResourceFile)),
                        new XElement("FirstName", Localization.GetString("FirstName.Header", LocalResourceFile)),
                        new XElement("LastName", Localization.GetString("LastName.Header", LocalResourceFile)),
                        new XElement("Email", Localization.GetString("Email.Header", LocalResourceFile)),
                        new XElement("Phone", Localization.GetString("Phone.Header", LocalResourceFile)),
                        new XElement("Street", Localization.GetString("Street.Header", LocalResourceFile)),
                        new XElement("PostalCode", Localization.GetString("PostalCode.Header", LocalResourceFile)),
                        new XElement("City", Localization.GetString("City.Header", LocalResourceFile)),
                        new XElement("Region", Localization.GetString("Region.Header", LocalResourceFile)),
                        new XElement("Country", Localization.GetString("Country.Header", LocalResourceFile)));
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
                foreach (XElement xElem in xElemHdr.Elements())
                {
                    if (string.IsNullOrEmpty(xElem.Value))
                    {
                        xElem.Value = xElem.Name.ToString();
                    }
                    xElem.Value = System.Convert.ToString(xElem.Value.Trim().Replace(" ", "_"));
                }

                //Gather the information.
                List<XElement> xElemList = new List<XElement>();
                XElement xElemEvent = default(XElement);

                //Add a localized header row when tags are not localized.
                if (!localizeTags)
                {
                    xElemEvent = new XElement(xNamespace + "Enrollee",
                        new XElement(xNamespace + xElemHdr.Elements("EventName").First().Value,
                        Localization.GetString("Event Name.Header", LocalResourceFile)),
                        new XElement(xNamespace + xElemHdr.Elements("EventStart").First().Value,
                        Localization.GetString("Event Start.Header", LocalResourceFile)),
                        new XElement(xNamespace + xElemHdr.Elements("EventEnd").First().Value,
                        Localization.GetString("Event End.Header", LocalResourceFile)),
                        new XElement(xNamespace + xElemHdr.Elements("Location").First().Value,
                        Localization.GetString("Location.Header", LocalResourceFile)),
                        new XElement(xNamespace + xElemHdr.Elements("Category").First().Value,
                        Localization.GetString("Category.Header", LocalResourceFile)),
                        new XElement(xNamespace + xElemHdr.Elements("ReferenceNumber").First().Value,
                        Localization.GetString("ReferenceNumber.Header", LocalResourceFile)),
                        new XElement(xNamespace + xElemHdr.Elements("Company").First().Value,
                        Localization.GetString("Company.Header", LocalResourceFile)),
                        new XElement(xNamespace + xElemHdr.Elements("JobTitle").First().Value,
                        Localization.GetString("JobTitle.Header", LocalResourceFile)),
                        new XElement(xNamespace + xElemHdr.Elements("FullName").First().Value,
                        Localization.GetString("FullName.Header", LocalResourceFile)),
                        new XElement(xNamespace + xElemHdr.Elements("FirstName").First().Value,
                        Localization.GetString("FirstName.Header", LocalResourceFile)),
                        new XElement(xNamespace + xElemHdr.Elements("LastName").First().Value,
                        Localization.GetString("LastName.Header", LocalResourceFile)),
                        new XElement(xNamespace + xElemHdr.Elements("Email").First().Value,
                        Localization.GetString("Email.Header", LocalResourceFile)),
                        new XElement(xNamespace + xElemHdr.Elements("Phone").First().Value,
                        Localization.GetString("Phone.Header", LocalResourceFile)),
                        new XElement(xNamespace + xElemHdr.Elements("Street").First().Value,
                        Localization.GetString("Street.Header", LocalResourceFile)),
                        new XElement(xNamespace + xElemHdr.Elements("PostalCode").First().Value,
                        Localization.GetString("PostalCode.Header", LocalResourceFile)),
                        new XElement(xNamespace + xElemHdr.Elements("City").First().Value,
                        Localization.GetString("City.Header", LocalResourceFile)),
                        new XElement(xNamespace + xElemHdr.Elements("Region").First().Value,
                        Localization.GetString("Region.Header", LocalResourceFile)),
                        new XElement(xNamespace + xElemHdr.Elements("Country").First().Value,
                        Localization.GetString("Country.Header", LocalResourceFile))
                    );
                    xElemList.Add(xElemEvent);
                }

                UserController objCtlUser = new UserController();
                foreach (EventSignupsInfo eventSignup in eventSignups)
                {
                    if (eventSignup.UserID != -1)
                    {
                        //Known DNN/Evoq user. Get info from user profile.
                        UserInfo objUser = objCtlUser.GetUser(PortalId, eventSignup.UserID);
                        xElemEvent = new XElement(xNamespace + "Enrollee",
                            new XElement(xNamespace + xElemHdr.Elements("EventName").First().Value, theEvent.EventName),
                            new XElement(xNamespace + xElemHdr.Elements("EventStart").First().Value, theEvent.EventTimeBegin),
                            new XElement(xNamespace + xElemHdr.Elements("EventEnd").First().Value, theEvent.EventTimeEnd),
                            new XElement(xNamespace + xElemHdr.Elements("Location").First().Value, theEvent.LocationName),
                            new XElement(xNamespace + xElemHdr.Elements("Category").First().Value, theEvent.CategoryName),
                            new XElement(xNamespace + xElemHdr.Elements("ReferenceNumber").First().Value, eventSignup.ReferenceNumber),
                            new XElement(xNamespace + xElemHdr.Elements("Company").First().Value, GetPropertyForDownload(objUser, "Company")),
                            new XElement(xNamespace + xElemHdr.Elements("JobTitle").First().Value, GetPropertyForDownload(objUser, "JobTitle")),
                            new XElement(xNamespace + xElemHdr.Elements("FullName").First().Value, objUser.DisplayName),
                            new XElement(xNamespace + xElemHdr.Elements("FirstName").First().Value, objUser.FirstName),
                            new XElement(xNamespace + xElemHdr.Elements("LastName").First().Value, objUser.LastName),
                            new XElement(xNamespace + xElemHdr.Elements("Email").First().Value, objUser.Email),
                            new XElement(xNamespace + xElemHdr.Elements("Phone").First().Value, GetPropertyForDownload(objUser, "Telephone")),
                            new XElement(xNamespace + xElemHdr.Elements("Street").First().Value, GetPropertyForDownload(objUser, "Street")),
                            new XElement(xNamespace + xElemHdr.Elements("PostalCode").First().Value, GetPropertyForDownload(objUser, "PostalCode")),
                            new XElement(xNamespace + xElemHdr.Elements("City").First().Value, GetPropertyForDownload(objUser, "City")),
                            new XElement(xNamespace + xElemHdr.Elements("Region").First().Value, GetPropertyForDownload(objUser, "Region")),
                            new XElement(xNamespace + xElemHdr.Elements("Country").First().Value, GetPropertyForDownload(objUser, "Country")));
                    }
                    else
                    {
                        //Anonymous user (site visitor). Get info from event signup.
                        xElemEvent = new XElement(xNamespace + "Enrollee",
                            new XElement(xNamespace + xElemHdr.Elements("EventName").First().Value, theEvent.EventName),
                            new XElement(xNamespace + xElemHdr.Elements("EventStart").First().Value, theEvent.EventTimeBegin),
                            new XElement(xNamespace + xElemHdr.Elements("EventEnd").First().Value, theEvent.EventTimeEnd),
                            new XElement(xNamespace + xElemHdr.Elements("Location").First().Value, theEvent.LocationName),
                            new XElement(xNamespace + xElemHdr.Elements("Category").First().Value, theEvent.CategoryName),
                            new XElement(xNamespace + xElemHdr.Elements("ReferenceNumber").First().Value, eventSignup.ReferenceNumber),
                            new XElement(xNamespace + xElemHdr.Elements("Company").First().Value, eventSignup.Company),
                            new XElement(xNamespace + xElemHdr.Elements("JobTitle").First().Value, eventSignup.JobTitle),
                            new XElement(xNamespace + xElemHdr.Elements("FullName").First().Value, eventSignup.AnonName),
                            new XElement(xNamespace + xElemHdr.Elements("FirstName").First().Value, eventSignup.FirstName),
                            new XElement(xNamespace + xElemHdr.Elements("LastName").First().Value, eventSignup.LastName),
                            new XElement(xNamespace + xElemHdr.Elements("Email").First().Value, eventSignup.AnonEmail),
                            new XElement(xNamespace + xElemHdr.Elements("Phone").First().Value, eventSignup.AnonTelephone),
                            new XElement(xNamespace + xElemHdr.Elements("Street").First().Value, eventSignup.Street),
                            new XElement(xNamespace + xElemHdr.Elements("PostalCode").First().Value, eventSignup.PostalCode),
                            new XElement(xNamespace + xElemHdr.Elements("City").First().Value, eventSignup.City),
                            new XElement(xNamespace + xElemHdr.Elements("Region").First().Value, eventSignup.Region),
                            new XElement(xNamespace + xElemHdr.Elements("Country").First().Value, eventSignup.Country));
                    }
                    xElemList.Add(xElemEvent);
                }

                //Aggregate into a document.
                XElement xElemMain = xRoot;
                xElemMain.Add(xElemList); //Everything ...
                xmlDoc.Load(xElemMain.CreateReader());

                //Add a declaration.
                XmlDeclaration xmlDecla = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement xmlElemDoc = xmlDoc.DocumentElement;
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
        /// Define the csv file for downloading the enrollees for this event.
        /// </summary>
        private string DefineCsvFile(EventInfo theEvent)
        {
            string returnValue = "";
            returnValue = null;
            StringBuilder csvDoc = new StringBuilder();

            //From xml to csv by xslt.
            XmlDocument xmlDoc = DefineXmlFile(theEvent, false);
            if (xmlDoc != null)
            {
                XPathDocument xPathDoc = new XPathDocument(XmlReader.Create(new StringReader(xmlDoc.InnerXml.ToString())));
                XslCompiledTransform xslTrafo = new XslCompiledTransform();
                xslTrafo.Load(XmlReader.Create(new StringReader(File.ReadAllText(Path.Combine(Request.MapPath(ControlPath), "EventEnrollees.xslt")))));
                StringWriter xWriter = new StringWriter(csvDoc);
                xslTrafo.Transform(xPathDoc, null, xWriter);
                xWriter.Close();
            }

            return csvDoc.ToString();
        }

        /// <summary>
        /// Generate the xml file for downloading the enrollees for this event.
        /// </summary>
        private bool GenerateXmlFile(EventInfo theEvent, XmlDocument xmlDoc)
        {
            bool returnValue = false;
            returnValue = false;

            //Name the file with a timestamp.
            string fileName = theEvent.EventName;
            if (!string.IsNullOrEmpty(Localization.GetString("EnrollmentsFile.Text", LocalResourceFile)))
            {
                fileName += " - " + Localization.GetString("EnrollmentsFile.Text", LocalResourceFile);
            }
            fileName += DateTime.Now.ToString(" - yyyyMMdd_HHmmss");

            //The contents of the file.
            string fileContent = xmlDoc.InnerXml.ToString();

            try
            {
                //Stream the file.
                HttpContext myContext = HttpContext.Current;
                HttpResponse myResponse = default(HttpResponse);
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
        /// Generate the csv file for downloading the enrollees for this event.
        /// </summary>
        private bool GenerateCsvFile(EventInfo theEvent, string csvDoc)
        {
            bool returnValue = false;
            returnValue = false;

            //Name the file with a timestamp.
            string fileName = theEvent.EventName;
            if (!string.IsNullOrEmpty(Localization.GetString("EnrollmentsFile.Text", LocalResourceFile)))
            {
                fileName += " - " + Localization.GetString("EnrollmentsFile.Text", LocalResourceFile);
            }
            fileName += DateTime.Now.ToString(" - yyyyMMdd_HHmmss");

            //The contents of the file.
            string fileContent = csvDoc;

            try
            {
                //Stream the file.
                HttpContext myContext = HttpContext.Current;
                HttpResponse myResponse = default(HttpResponse);
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
            string returnValue = "";
            returnValue = null;

            if (!string.IsNullOrEmpty(propertyName))
            {
                ProfilePropertyDefinition profileProperty = objUser.Profile.GetProperty(propertyName);

                if (profileProperty != null)
                {
                    returnValue = profileProperty.PropertyValue;
                }
            }
            return returnValue;
        }

        private void ShowMessage(string msg, MessageLevel messageLevel)
        {
            lblMessage.Text = msg;

            //Hide the rest of the form fields.
            divMessage.Attributes.Add("style", "display:block;");

            switch (messageLevel)
            {
                case MessageLevel.DNNSuccess:
                    divMessage.Attributes.Add("class", "dnnFormMessage dnnFormSuccess");
                    break;
                case MessageLevel.DNNInformation:
                    divMessage.Attributes.Add("class", "dnnFormMessage dnnFormInfo");
                    break;
                case MessageLevel.DNNWarning:
                    divMessage.Attributes.Add("class", "dnnFormMessage dnnFormWarning");
                    break;
                case MessageLevel.DNNError:
                    divMessage.Attributes.Add("class", "dnnFormMessage dnnFormValidationSummary");
                    break;
            }
        }
        #endregion

        #region Links and Buttons
        /// <summary>
        /// When delete button is clicked the current event will be removed
        /// </summary>
        protected void deleteButton_Click(System.Object sender, EventArgs e)
        {
            try
            {
                int iItemID = ItemId;
                EventController objCtlEvent = new EventController();
                EventInfo objEvent = objCtlEvent.EventsGet(iItemID, ModuleId);
                if (objEvent.RRULE != "")
                {
                    objEvent.Cancelled = true;
                    objEvent.LastUpdatedID = UserId;
                    objCtlEvent.EventsSave(objEvent, true, TabId, true);
                }
                else
                {
                    EventRecurMasterController objCtlEventRecurMaster = new EventRecurMasterController();
                    objCtlEventRecurMaster.EventsRecurMasterDelete(objEvent.RecurMasterID, objEvent.ModuleID);
                }
                Response.Redirect(GetSocialNavigateUrl(), true);
            }
            catch (System.Threading.ThreadAbortException)
            {
                //Ignore
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// When delete series button is clicked the current event series will be removed
        /// </summary>
        protected void deleteSeriesButton_Click(object sender, EventArgs e)
        {
            try
            {
                int iItemID = ItemId;
                EventController objCtlEvent = new EventController();
                EventInfo objEvent = objCtlEvent.EventsGet(iItemID, ModuleId);
                EventRecurMasterController objCtlEventRecurMaster = new EventRecurMasterController();
                objCtlEventRecurMaster.EventsRecurMasterDelete(objEvent.RecurMasterID, objEvent.ModuleID);
                Response.Redirect(GetSocialNavigateUrl(), true);
            }
            catch (System.Threading.ThreadAbortException)
            {
                //Ignore
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// When Export simple button is clicked the current event will be exported to a vcard
        /// </summary>
        protected void cmdvEvent_Click(object sender, EventArgs e)
        {
            ExportEvent(false);
        }

        /// <summary>
        /// When Export event series button is clicked the current event will be exported to a vcard
        /// </summary>
        protected void cmdvEventSeries_Click(object sender, EventArgs e)
        {
            ExportEvent(true);
        }

        /// <summary>
        /// When Download event signups button is clicked the current event signups will be written to an XML file for download.
        /// </summary>
        protected void cmdvEventSignups_Click(object sender, EventArgs e)
        {
            DownloadSignups();
        }

        /// <summary>
        /// When return button is clicked the user is redirected to the previous page
        /// </summary>
        /// <remarks></remarks>
        protected void returnButton_Click(System.Object sender, EventArgs e)
        {
            try
            {
                Response.Redirect(GetSocialNavigateUrl(), true);
            }
            catch (System.Threading.ThreadAbortException)
            {
                //Ignore
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// When signup button is clicked the user will be signed up for the event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        protected void cmdSignup_Click(System.Object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated && !Settings.AllowAnonEnroll)
            {
                RedirectToLogin();
            }

            try
            {
                EventInfo objEvent = default(EventInfo);
                EventController objCtlEvent = new EventController();
                objEvent = objCtlEvent.EventsGet(ItemId, ModuleId);
                if (!Request.IsAuthenticated && !objEvent.AllowAnonEnroll)
                {
                    RedirectToLogin();
                }

                // In case of custom enrollment page.
                if (Settings.EnrollmentPageAllowed)
                {
                    if (!string.IsNullOrEmpty(Settings.EnrollmentPageDefaultUrl))
                    {
                        Response.Redirect(Settings.EnrollmentPageDefaultUrl + "?mod=" + System.Convert.ToString(ModuleId) + "&event=" + System.Convert.ToString(ItemId));
                    }
                    return;
                }

                // In case of standard paid enrollment.
                // Check to see if unauthenticated user has already enrolled
                EventSignupsController objCtlEventSignups = new EventSignupsController();
                if (!Request.IsAuthenticated)
                {
                    EventSignupsInfo objEventsSignups = default(EventSignupsInfo);
                    objEventsSignups = objCtlEventSignups.EventsSignupsGetAnonUser(objEvent.EventID, txtAnonEmail.Text, objEvent.ModuleID);
                    if (!ReferenceEquals(objEventsSignups, null))
                    {
                        ShowMessage(Localization.GetString("YouAreAlreadyEnrolledForThisEvent", LocalResourceFile), MessageLevel.DNNWarning);
                        enroll1.Visible = false;
                        enroll3.Visible = false;
                        enroll5.Visible = false;
                        return;
                    }
                }

                if (objEvent.EnrollType == "PAID")
                {
                    // Paid Even Process
                    try
                    {
                        EventInfoHelper objEventInfoHelper = new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
                        int socialGroupId = GetUrlGroupId();
                        if (Request.IsAuthenticated)
                        {
                            if (socialGroupId > 0)
                            {
                                Response.Redirect(objEventInfoHelper.AddSkinContainerControls(DotNetNuke.Common.Globals.NavigateURL(TabId, "PPEnroll", "Mid=" + System.Convert.ToString(ModuleId), "ItemID=" + System.Convert.ToString(ItemId), "NoEnrol=" + txtNoEnrolees.Text, "groupid=" + socialGroupId.ToString()), "?"));
                            }
                            else
                            {
                                Response.Redirect(objEventInfoHelper.AddSkinContainerControls(DotNetNuke.Common.Globals.NavigateURL(TabId, "PPEnroll", "Mid=" + System.Convert.ToString(ModuleId), "ItemID=" + System.Convert.ToString(ItemId), "NoEnrol=" + txtNoEnrolees.Text), "?"));
                            }
                        }
                        else
                        {
                            string urlAnonTelephone = txtAnonTelephone.Text.Trim();
                            if (string.IsNullOrEmpty(urlAnonTelephone))
                            {
                                urlAnonTelephone = "0";
                            }
                            if (socialGroupId > 0)
                            {
                                Response.Redirect(objEventInfoHelper.AddSkinContainerControls(DotNetNuke.Common.Globals.NavigateURL(TabId, "PPEnroll", "Mid=" + System.Convert.ToString(ModuleId), "ItemID=" + System.Convert.ToString(ItemId), "NoEnrol=" + txtNoEnrolees.Text, "groupid=" + socialGroupId.ToString(), "AnonEmail=" + HttpUtility.UrlEncode(txtAnonEmail.Text), "AnonName=" + HttpUtility.UrlEncode(txtAnonName.Text), "AnonPhone=" + HttpUtility.UrlEncode(urlAnonTelephone)), "&"));
                            }
                            else
                            {
                                Response.Redirect(objEventInfoHelper.AddSkinContainerControls(DotNetNuke.Common.Globals.NavigateURL(TabId, "PPEnroll", "Mid=" + System.Convert.ToString(ModuleId), "ItemID=" + System.Convert.ToString(ItemId), "NoEnrol=" + txtNoEnrolees.Text, "AnonEmail=" + HttpUtility.UrlEncode(txtAnonEmail.Text), "AnonName=" + HttpUtility.UrlEncode(txtAnonName.Text), "AnonPhone=" + HttpUtility.UrlEncode(urlAnonTelephone)), "&"));
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
                    EventSignupsInfo objEventSignups = new EventSignupsInfo();
                    objEventSignups.EventID = objEvent.EventID;

                    DateTime startdate = objEvent.EventTimeBegin;
                    SelectedDate = startdate.Date;

                    objEventSignups.ModuleID = objEvent.ModuleID;
                    if (Request.IsAuthenticated)
                    {
                        objEventSignups.UserID = UserId;
                        objEventSignups.AnonEmail = null;
                        objEventSignups.AnonName = null;
                        objEventSignups.AnonTelephone = null;
                        objEventSignups.AnonCulture = null;
                        objEventSignups.AnonTimeZoneId = null;
                    }
                    else
                    {
                        PortalSecurity objSecurity = new PortalSecurity();
                        objEventSignups.UserID = -1;
                        objEventSignups.AnonEmail = txtAnonEmail.Text;
                        objEventSignups.AnonName = objSecurity.InputFilter(txtAnonName.Text, PortalSecurity.FilterFlag.NoScripting);
                        objEventSignups.AnonTelephone = objSecurity.InputFilter(txtAnonTelephone.Text, PortalSecurity.FilterFlag.NoScripting);
                        objEventSignups.AnonCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                        objEventSignups.AnonTimeZoneId = GetDisplayTimeZoneId();
                    }
                    objEventSignups.PayPalPaymentDate = DateTime.UtcNow;
                    objEventSignups.NoEnrolees = int.Parse(txtNoEnrolees.Text);
                    if (IsModerator() ||
                        PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString()))
                    {
                        objEventSignups.Approved = true;
                        objEvent.Enrolled++;
                    }
                    else if (Settings.Moderateall)
                    {
                        objEventSignups.Approved = false;
                    }
                    else
                    {
                        objEventSignups.Approved = true;
                        objEvent.Enrolled++;
                    }
                    objEventSignups = CreateEnrollment(objEventSignups, objEvent);
                    enroll1.Visible = false;
                    MessageLevel msgLevel = UserEnrollment(objEvent);
                    ShowMessage(lblSignup.Text, msgLevel);
                    // Send Moderator email
                    EventEmailInfo objEventEmailInfo = new EventEmailInfo();
                    EventEmails objEventEmail = new EventEmails(PortalId, ModuleId, LocalResourceFile, ((PageBase)Page).PageCulture.Name);
                    if (Settings.Moderateall && objEventSignups.Approved == false)
                    {
                        objEventEmailInfo.TxtEmailSubject = Settings.Templates.moderateemailsubject;
                        objEventEmailInfo.TxtEmailBody = Settings.Templates.moderateemailmessage;
                        objEventEmailInfo.TxtEmailFrom = Settings.StandardEmail;
                        ArrayList moderators = GetModerators();
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
                    objEventEmailInfo.TxtEmailSubject = Settings.Templates.txtEnrollMessageSubject;
                    objEventEmailInfo.TxtEmailFrom = Settings.StandardEmail;
                    if (Request.IsAuthenticated)
                    {
                        objEventEmailInfo.UserEmails.Add(PortalSettings.UserInfo.Email);
                        objEventEmailInfo.UserLocales.Add(PortalSettings.UserInfo.Profile.PreferredLocale);
                        objEventEmailInfo.UserTimeZoneIds.Add(PortalSettings.UserInfo.Profile.PreferredTimeZone.Id);
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
                        if (Settings.SendEnrollMessageApproved)
                        {
                            objEventEmailInfo.TxtEmailBody = Settings.Templates.txtEmailMessage + Settings.Templates.txtEnrollMessageApproved;
                            objEventEmail.SendEmails(objEventEmailInfo, objEvent, objEventSignups);
                        }
                    }
                    else
                    {
                        if (Settings.SendEnrollMessageWaiting)
                        {
                            objEventEmailInfo.TxtEmailBody = Settings.Templates.txtEmailMessage + Settings.Templates.txtEnrollMessageWaiting;
                            objEventEmail.SendEmails(objEventEmailInfo, objEvent, objEventSignups);
                        }
                    }

                }
                BindEnrollList(objEvent);

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// When notify button is clicked the user will be added to the notification list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        protected void cmdNotify_Click(System.Object sender, EventArgs e)
        {
            valEmail.Validate();
            valEmail2.Validate();
            if (!valEmail.IsValid || !valEmail2.IsValid)
            {
                return;
            }

            ArrayList lstEvents = new ArrayList();
            EventNotificationInfo objEventNotification = default(EventNotificationInfo);
            EventNotificationController objEventNotificationController = new EventNotificationController();
            DateTime eventDate = default(DateTime);
            DateTime notifyTime = default(DateTime);
            int currentEv;
            EventTimeZoneUtilities objEventTimeZoneUtilities = new EventTimeZoneUtilities();
            try
            {
                EventInfo eventEvent = default(EventInfo);
                EventController objCtlEvent = new EventController();
                eventEvent = objCtlEvent.EventsGet(ItemId, ModuleId);
                currentEv = eventEvent.EventID;

                if (chkReminderRec.Checked)
                {
                    lstEvents = objCtlEvent.EventsGetRecurrences(eventEvent.RecurMasterID, ModuleId);
                }
                else
                {
                    lstEvents.Add(eventEvent);
                }

                foreach (EventInfo tempLoopVar_eventEvent in lstEvents)
                {
                    eventEvent = tempLoopVar_eventEvent;
                    if (eventEvent.SendReminder == true && eventEvent.EventTimeBegin > objEventTimeZoneUtilities.ConvertFromUTCToModuleTimeZone(DateTime.UtcNow, eventEvent.EventTimeZoneId))
                    {
                        eventDate = objEventTimeZoneUtilities.ConvertToUTCTimeZone(eventEvent.EventTimeBegin, eventEvent.EventTimeZoneId);
                        objEventNotification = objEventNotificationController.EventsNotificationGet(eventEvent.EventID, txtUserEmail.Text, ModuleId);

                        notifyTime = eventDate;
                        //*** Calculate notification time
                        switch (ddlReminderTimeMeasurement.SelectedValue)
                        {
                            case "m":
                                notifyTime = notifyTime.AddMinutes((int.Parse(txtReminderTime.Text)) * -1);
                                break;
                            case "h":
                                notifyTime = notifyTime.AddHours((int.Parse(txtReminderTime.Text)) * -1);
                                break;
                            case "d":
                                notifyTime = notifyTime.AddDays((int.Parse(txtReminderTime.Text)) * -1);
                                break;
                        }
                        // Registered users will overwrite existing notifications (in recurring events)
                        DateTime notifyDisplayTime = objEventTimeZoneUtilities.ConvertFromUTCToDisplayTimeZone(notifyTime, GetDisplayTimeZoneId()).EventDate;
                        if (!(ReferenceEquals(objEventNotification, null)) && Request.IsAuthenticated)
                        {
                            objEventNotification.NotifyByDateTime = notifyTime;
                            objEventNotificationController.EventsNotificationSave(objEventNotification);
                            if (currentEv == eventEvent.EventID)
                            {
                                lblConfirmation.Text = string.Format(Localization.GetString("lblReminderConfirmation", LocalResourceFile), notifyDisplayTime.ToString());
                                ShowMessage(lblConfirmation.Text, MessageLevel.DNNSuccess);
                            }
                            // Anonymous users can never overwrite an existing notification
                        }
                        else if (!(ReferenceEquals(objEventNotification, null)))
                        {
                            if (currentEv == eventEvent.EventID)
                            {
                                lblConfirmation.Text = string.Format(Localization.GetString("ReminderAlreadyReg", LocalResourceFile), txtUserEmail.Text.ToString(), objEventNotification.NotifyByDateTime.ToString());
                                ShowMessage(lblConfirmation.Text, MessageLevel.DNNWarning);
                            }
                        }
                        else
                        {
                            objEventNotification = new EventNotificationInfo();
                            objEventNotification.NotificationID = -1;
                            objEventNotification.EventID = eventEvent.EventID;
                            objEventNotification.PortalAliasID = PortalAlias.PortalAliasID;
                            objEventNotification.NotificationSent = false;
                            objEventNotification.EventTimeBegin = eventDate;
                            objEventNotification.NotifyLanguage = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                            objEventNotification.ModuleID = ModuleId;
                            objEventNotification.TabID = TabId;
                            objEventNotification.NotifyByDateTime = notifyTime;
                            objEventNotification.UserEmail = txtUserEmail.Text;
                            objEventNotificationController.EventsNotificationSave(objEventNotification);
                            if (currentEv == eventEvent.EventID)
                            {
                                lblConfirmation.Text = string.Format(Localization.GetString("lblReminderConfirmation", LocalResourceFile), notifyDisplayTime.ToString());
                                ShowMessage(lblConfirmation.Text, MessageLevel.DNNSuccess);
                            }
                        }
                    }
                }
                rem1.Visible = false;
                rem2.Visible = false;
                rem3.Visible = true;
                imgConfirmation.AlternateText = Localization.GetString("Reminder", LocalResourceFile);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdPrint_PreRender(object sender, EventArgs e)
        {

            cmdPrint.Target = " _blank";
            cmdPrint.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(TabId, PortalSettings, "", "mid=" + System.Convert.ToString(ModuleId), "itemid=" + System.Convert.ToString(ItemId), "ctl=Details", "ShowNav=False", "dnnprintmode=true", "SkinSrc=%5bG%5dSkins%2f_default%2fNo+Skin", "ContainerSrc=%5bG%5dContainers%2f_default%2fNo+Container");

        }
        protected void cmdEmail_Click(object sender, EventArgs e)
        {
            valEmailiCal.Validate();
            valEmailiCal2.Validate();
            if (!valEmailiCal.IsValid || !valEmailiCal2.IsValid)
            {
                return;
            }

            EventController objCtlEvent = new EventController();
            EventInfo objEvent = objCtlEvent.EventsGet(ItemId, ModuleId);
            if (ReferenceEquals(objEvent, null))
            {
                return;
            }

            VEvent iCalendar = new VEvent(false, HttpContext.Current);
            string iCal = "";
            iCal = iCalendar.CreateiCal(TabId, ModuleId, ItemId, objEvent.SocialGroupId);

            System.Net.Mail.Attachment attachment = System.Net.Mail.Attachment.CreateAttachmentFromString(iCal, new System.Net.Mime.ContentType("text/calendar"));
            attachment.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
            attachment.Name = objEvent.EventName + ".ics";
            System.Collections.Generic.List<System.Net.Mail.Attachment> attachments = new System.Collections.Generic.List<System.Net.Mail.Attachment>();
            attachments.Add(attachment);

            EventEmailInfo objEventEmailInfo = new EventEmailInfo();
            EventEmails objEventEmail = new EventEmails(PortalId, ModuleId, LocalResourceFile, ((PageBase)Page).PageCulture.Name);
            objEventEmailInfo.TxtEmailSubject = Settings.Templates.EventiCalSubject;
            objEventEmailInfo.TxtEmailBody = Settings.Templates.EventiCalBody;
            objEventEmailInfo.TxtEmailFrom = Settings.StandardEmail;
            objEventEmailInfo.UserEmails.Add(txtUserEmailiCal.Text);
            objEventEmailInfo.UserLocales.Add("");
            objEventEmailInfo.UserTimeZoneIds.Add(objEvent.EventTimeZoneId);

            objEventEmail.SendEmails(objEventEmailInfo, objEvent, attachments);
            divMessage.Attributes.Add("style", "display:block;");
            ShowMessage(string.Format(Localization.GetString("ConfirmationiCal", LocalResourceFile), txtUserEmailiCal.Text), MessageLevel.DNNSuccess);

        }

        #endregion

    }
}

