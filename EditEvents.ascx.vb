'
' DotNetNuke® - http://www.dnnsoftware.com
' Copyright (c) 2002-2013
' by DNNCorp
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'

Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Security.Roles
Imports System.Globalization
Imports System.Collections
Imports DotNetNuke.Security
Imports DotNetNuke.Web.UI.WebControls.Extensions

Namespace DotNetNuke.Modules.Events

    <DNNtc.ModuleControlProperties("Edit", "Edit Events", DNNtc.ControlType.View, "https://dnnevents.codeplex.com/documentation", False, True)> _
    Partial Class EditEvents
        Inherits EventBase

#Region "Private Area"
        Private _itemID As Integer = -1
        Private _editRecur As Boolean = True
        Private ReadOnly _objCtlEvent As New EventController
        Private ReadOnly _objCtlEventRecurMaster As New EventRecurMasterController
        Private _objEvent As New EventInfo
        Private _objEventSignups As New EventSignupsInfo
        Private ReadOnly _objCtlEventSignups As New EventSignupsController
        Private _lstEvents As New ArrayList
        Private ReadOnly _lstOwnerUsers As New ArrayList
        Private ReadOnly _culture As CultureInfo = Threading.Thread.CurrentThread.CurrentCulture
        Private Const RecurTableDisplayType As String = "inline-block"

#End Region

#Region "Event Handlers"
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
            Try
                ' Verify that the current user has edit access to this module
                If PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString) Or _
                   IsModuleEditor() Then
                Else
                    ' to stop errors when not authorised to edit
                    valReminderTime.MinimumValue = "15"
                    valReminderTime.MaximumValue = "60"

                    Response.Redirect(GetSocialNavigateUrl(), True)
                End If

                grdAddUser.ModuleConfiguration = ModuleConfiguration.Clone

                ' Add the external Validation.js to the Page
                Const csname As String = "ExtValidationScriptFile"
                Dim cstype As Type = System.Reflection.MethodBase.GetCurrentMethod().GetType()
                Dim cstext As String = "<script src=""" & ResolveUrl("~/DesktopModules/Events/Scripts/Validation.js") & """ type=""text/javascript""></script>"
                If Not Page.ClientScript.IsClientScriptBlockRegistered(csname) Then
                    Page.ClientScript.RegisterClientScriptBlock(cstype, csname, cstext, False)
                End If

                ' Determine ItemId of Event to Update
                If Not (Request.Params("ItemId") Is Nothing) Then
                    _itemID = Int32.Parse(Request.Params("ItemId"))
                End If
                _editRecur = False
                If Not (Request.Params("EditRecur") Is Nothing) Then
                    If Request.Params("EditRecur").ToLower = "all" Then
                        _editRecur = True
                    End If
                End If

                ' Set the selected theme
                SetTheme(pnlEventsModuleEdit)

                'EPT: "Changed DotNetNuke.Security.PortalSecurity.HasEditPermissions(ModuleId)" into "IsEditable"
                'RWJS: Replaced with custom function IsModuleEditor which checks whether users has editor permissions
                If (IsModuleEditor()) Or _
                   (IsModerator()) Or _
                   (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString)) Then
                Else
                    Response.Redirect(GetSocialNavigateUrl(), True)
                End If

                trOwner.Visible = False
                If (IsModerator() And Settings.Ownerchangeallowed) Or _
                   PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString) Then
                    trOwner.Visible = True
                End If

                pnlEnroll.Visible = False
                divNoEnrolees.Visible = False

                If Settings.Eventsignup Then
                    pnlEnroll.Visible = True
                    chkSignups.Visible = True
                    If Settings.Maxnoenrolees > 1 And divAddUser.Visible Then
                        divNoEnrolees.Visible = True
                    End If
                End If

                trTypeOfEnrollment.Visible = Settings.Eventsignupallowpaid
                trPayPalAccount.Visible = Settings.Eventsignupallowpaid

                tblEventEmail.Attributes.Add("style", "display:none; width:100%")
                If Not Settings.Newpereventemail Then
                    pnlEventEmailRole.Visible = False
                ElseIf _itemID = -1 And Settings.Moderateall And Not IsModerator() Then
                    pnlEventEmailRole.Visible = False
                End If

                pnlReminder.Visible = Settings.Eventnotify
                pnlImage.Visible = Settings.Eventimage
                pnlDetailPage.Visible = Settings.DetailPageAllowed

                ' Setup Popup Event
                dpStartDate.ClientEvents.OnDateSelected = "function() {if (Page_ClientValidate('startdate')) CopyField('" + dpStartDate.ClientID + "','" + dpEndDate.ClientID + "');}"
                tpStartTime.ClientEvents.OnDateSelected = "function() {SetComboIndex('" + tpStartTime.ClientID + "','" + tpEndTime.ClientID + "','" + dpStartDate.ClientID + "','" + dpEndDate.ClientID + "','" + Settings.Timeinterval + "');}"
                ctlURL.FileFilter = glbImageFileTypes
                If Not Page.IsPostBack Then
                    txtSubject.MaxLength = txtSubject.MaxLength + 1
                    txtReminder.MaxLength = txtReminder.MaxLength + 1
                End If
                Dim limitSubject As String = "javascript:limitText(this," + (txtSubject.MaxLength - 1).ToString + ",'" + Localization.GetString("LimitChars", LocalResourceFile) + "');"
                Dim limitReminder As String = "javascript:limitText(this," + (txtReminder.MaxLength - 1).ToString + ",'" + Localization.GetString("LimitChars", LocalResourceFile) + "');"
                txtSubject.Attributes.Add("onkeydown", limitSubject)
                txtSubject.Attributes.Add("onkeyup", limitSubject)
                txtReminder.Attributes.Add("onkeydown", limitReminder)
                txtReminder.Attributes.Add("onkeyup", limitReminder)

                Page.ClientScript.RegisterExpandoAttribute(valValidStartTime2.ClientID, "TimeInterval", Settings.Timeinterval)
                Page.ClientScript.RegisterExpandoAttribute(valValidStartTime2.ClientID, "ErrorMessage", String.Format(Localization.GetString("valValidStartTime2", LocalResourceFile), Settings.Timeinterval))
                Page.ClientScript.RegisterExpandoAttribute(valValidStartTime2.ClientID, "ClientID", tpStartTime.ClientID)
                Page.ClientScript.RegisterExpandoAttribute(valValidEndTime2.ClientID, "TimeInterval", Settings.Timeinterval)
                Page.ClientScript.RegisterExpandoAttribute(valValidEndTime2.ClientID, "ErrorMessage", String.Format(Localization.GetString("valValidEndTime2", LocalResourceFile), Settings.Timeinterval))
                Page.ClientScript.RegisterExpandoAttribute(valValidEndTime2.ClientID, "ClientID", tpEndTime.ClientID)

                ' If the page is being requested the first time, determine if an
                ' contact itemId value is specified, and if so populate page
                ' contents with the contact details
                If Not Page.IsPostBack Then
                    LocalizeAll()
                    LoadEvent()
                Else
                    Dim url As String = ctlURL.Url
                    Dim urlType As String = ctlURL.UrlType
                    ctlURL.Url = url
                    ctlURL.UrlType = urlType
                End If

                If chkReminder.Checked Then
                    tblReminderDetail.Attributes.Add("style", "display:block;")
                Else
                    tblReminderDetail.Attributes.Add("style", "display:none;")
                End If

                If chkDetailPage.Checked Then
                    tblDetailPageDetail.Attributes.Add("style", "display:block;")
                Else
                    tblDetailPageDetail.Attributes.Add("style", "display:none;")
                End If

                If chkDisplayImage.Checked Then
                    tblImageURL.Attributes.Add("style", "display:block;")
                Else
                    tblImageURL.Attributes.Add("style", "display:none;")
                End If

                If chkSignups.Checked Then
                    tblEnrollmentDetails.Attributes.Add("style", "display:block;")
                Else
                    tblEnrollmentDetails.Attributes.Add("style", "display:none;")
                End If

                If chkReccuring.Checked Then
                    tblRecurringDetails.Attributes.Add("style", "display:block;")
                Else
                    tblRecurringDetails.Attributes.Add("style", "display:none;")
                End If

                If chkEventEmailChk.Checked Then
                    tblEventEmailRoleDetail.Attributes.Add("style", "display:block;")
                Else
                    tblEventEmailRoleDetail.Attributes.Add("style", "display:none;")
                End If

                If rblRepeatTypeP1.Checked Then
                    tblDetailP1.Attributes.Add("style", "display:" & RecurTableDisplayType & ";white-space:nowrap;")
                Else
                    tblDetailP1.Attributes.Add("style", "display:none;white-space:nowrap;")
                End If

                If rblRepeatTypeW1.Checked Then
                    tblDetailW1.Attributes.Add("style", "display:" & RecurTableDisplayType & ";")
                Else
                    tblDetailW1.Attributes.Add("style", "display:none;")
                End If

                If rblRepeatTypeM.Checked Then
                    tblDetailM1.Attributes.Add("style", "display:" & RecurTableDisplayType & ";")
                Else
                    tblDetailM1.Attributes.Add("style", "display:none;")
                End If

                If rblRepeatTypeY1.Checked Then
                    tblDetailY1.Attributes.Add("style", "display:" & RecurTableDisplayType & ";")
                Else
                    tblDetailY1.Attributes.Add("style", "display:none;")
                End If

                If dpY1Period.SelectedDate.ToString.Length = 0 Then
                    dpY1Period.SelectedDate = SelectedDate.Date
                End If
                If txtReminderFrom.Text.Length = 0 Then
                    txtReminderFrom.Text = Settings.Reminderfrom
                End If
                If txtEventEmailFrom.Text.Length = 0 Then
                    txtEventEmailFrom.Text = Settings.Reminderfrom
                End If

                If chkAllDayEvent.Checked Then
                    divStartTime.Attributes.Add("style", "display:none;")
                    divEndTime.Attributes.Add("style", "display:none;")
                End If

            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub valValidStartDate3_ServerValidate(source As Object, args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles valValidStartDate.ServerValidate
            If dpStartDate.SelectedDate Is Nothing Then
                args.IsValid = False
                valValidStartDate.Visible = True
            End If
        End Sub

        Private Sub valValidStartTime2_ServerValidate(source As Object, args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles valValidStartTime2.ServerValidate
            Dim inDate As DateTime = CType(tpStartTime.SelectedDate, DateTime)
            valValidStartTime2.ErrorMessage = String.Format(Localization.GetString("valValidStartTime2", LocalResourceFile), Settings.Timeinterval)
            args.IsValid = ValidateTime(inDate)
        End Sub

        Private Sub valValidEndTime2_ServerValidate(source As Object, args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles valValidEndTime2.ServerValidate
            Dim inDate As DateTime = CType(tpEndTime.SelectedDate, DateTime)
            valValidEndTime2.ErrorMessage = String.Format(Localization.GetString("valValidEndTime2", LocalResourceFile), Settings.Timeinterval)
            args.IsValid = ValidateTime(inDate)
        End Sub

        Private Sub valValidRecurEndDate_ServerValidate(source As Object, args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles valValidRecurEndDate.ServerValidate
            If dpStartDate.SelectedDate Is Nothing Then
                Return
            End If
            Dim recurDate As DateTime = CType(dpRecurEndDate.SelectedDate, DateTime)
            Dim startDate As DateTime = CType(dpStartDate.SelectedDate, DateTime)
            If recurDate < startDate And Not rblRepeatTypeN.Checked Then
                args.IsValid = False
                valValidRecurEndDate.Visible = True
            Else
                args.IsValid = True
                valValidRecurEndDate.Visible = False
            End If

        End Sub

#End Region

#Region "Helper Methods and Functions"

        Private Sub LocalizeAll()
            Dim culture As CultureInfo = Threading.Thread.CurrentThread.CurrentCulture

            txtSubject.Text = Settings.Templates.txtSubject
            txtReminder.Text = Settings.Templates.txtMessage

            grdEnrollment.Columns(0).HeaderText = Localization.GetString("Select", LocalResourceFile)
            grdEnrollment.Columns(1).HeaderText = Localization.GetString("EnrollUserName", LocalResourceFile)
            grdEnrollment.Columns(2).HeaderText = Localization.GetString("EnrollDisplayName", LocalResourceFile)
            grdEnrollment.Columns(3).HeaderText = Localization.GetString("EnrollEmail", LocalResourceFile)
            grdEnrollment.Columns(4).HeaderText = Localization.GetString("EnrollPhone", LocalResourceFile)
            grdEnrollment.Columns(5).HeaderText = Localization.GetString("EnrollApproved", LocalResourceFile)
            grdEnrollment.Columns(6).HeaderText = Localization.GetString("EnrollNo", LocalResourceFile)
            grdEnrollment.Columns(7).HeaderText = Localization.GetString("EventStart", LocalResourceFile)

            chkW1Sun.Text = culture.DateTimeFormat.AbbreviatedDayNames(DayOfWeek.Sunday)
            chkW1Sun2.Text = culture.DateTimeFormat.AbbreviatedDayNames(DayOfWeek.Sunday)
            chkW1Mon.Text = culture.DateTimeFormat.AbbreviatedDayNames(DayOfWeek.Monday)
            chkW1Tue.Text = culture.DateTimeFormat.AbbreviatedDayNames(DayOfWeek.Tuesday)
            chkW1Wed.Text = culture.DateTimeFormat.AbbreviatedDayNames(DayOfWeek.Wednesday)
            chkW1Thu.Text = culture.DateTimeFormat.AbbreviatedDayNames(DayOfWeek.Thursday)
            chkW1Fri.Text = culture.DateTimeFormat.AbbreviatedDayNames(DayOfWeek.Friday)
            chkW1Sat.Text = culture.DateTimeFormat.AbbreviatedDayNames(DayOfWeek.Saturday)

            cmbM1Period.Items.Clear()
            ' Corrected a problem w/Every nth Week on a specific day with the following
            cmbM1Period.Items.Add(New ListItem(culture.DateTimeFormat.GetDayName(DayOfWeek.Sunday), "0"))
            cmbM1Period.Items.Add(New ListItem(culture.DateTimeFormat.GetDayName(DayOfWeek.Monday), "1"))
            cmbM1Period.Items.Add(New ListItem(culture.DateTimeFormat.GetDayName(DayOfWeek.Tuesday), "2"))
            cmbM1Period.Items.Add(New ListItem(culture.DateTimeFormat.GetDayName(DayOfWeek.Wednesday), "3"))
            cmbM1Period.Items.Add(New ListItem(culture.DateTimeFormat.GetDayName(DayOfWeek.Thursday), "4"))
            cmbM1Period.Items.Add(New ListItem(culture.DateTimeFormat.GetDayName(DayOfWeek.Friday), "5"))
            cmbM1Period.Items.Add(New ListItem(culture.DateTimeFormat.GetDayName(DayOfWeek.Saturday), "6"))

            cmbM2Period.Items.Clear()
            For i As Integer = 1 To 31
                cmbM2Period.Items.Add(New ListItem(Localization.GetString(i.ToString(), LocalResourceFile), i.ToString()))
            Next

            lblMaxRecurrences.Text = String.Format(Localization.GetString("lblMaxRecurrences", LocalResourceFile), Settings.Maxrecurrences)

            If culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Sunday Then
                chkW1Sun.Attributes.Add("style", "display:inline;")
                chkW1Sun2.Attributes.Add("style", "display:none;")
            Else
                chkW1Sun2.Attributes.Add("style", "display:inline;")
                chkW1Sun.Attributes.Add("style", "display:none;")
            End If

            dpStartDate.DatePopupButton.ToolTip = Localization.GetString("DatePickerTooltip", LocalResourceFile)
            dpEndDate.DatePopupButton.ToolTip = Localization.GetString("DatePickerTooltip", LocalResourceFile)
            dpRecurEndDate.DatePopupButton.ToolTip = Localization.GetString("DatePickerTooltip", LocalResourceFile)
            dpY1Period.DatePopupButton.ToolTip = Localization.GetString("DatePickerTooltip", LocalResourceFile)

            tpEndTime.TimePopupButton.ToolTip = Localization.GetString("TimePickerTooltip", LocalResourceFile)

        End Sub

        Public Sub LoadEvent()
            StorePrevPageInViewState()

            pnlRecurring.Visible = True
            lblMaxRecurrences.Visible = False
            If Not Settings.Allowreoccurring Then
                dpRecurEndDate.Enabled = False
                cmbP1Period.Enabled = False
                txtP1Every.Enabled = False
                dpRecurEndDate.Visible = False
                cmbP1Period.Visible = False
                txtP1Every.Visible = False
                pnlRecurring.Visible = False
            Else
                If Settings.Maxrecurrences <> "" Then
                    lblMaxRecurrences.Visible = True
                End If
            End If

            'Populate the timezone combobox (look up timezone translations based on currently set culture)
            cboTimeZone.DataBind(Settings.TimeZoneId)
            If Not Settings.EnableEventTimeZones Then
                cboTimeZone.Enabled = False
            End If

            If _editRecur Then
                deleteButton.Attributes.Add("onClick", "javascript:return confirm('" + Localization.GetString("ConfirmEventSeriesDelete", LocalResourceFile) + "');")
                deleteButton.Text = Localization.GetString("deleteSeriesButton", LocalResourceFile)
                updateButton.Text = Localization.GetString("updateSeriesButton", LocalResourceFile)
                copyButton.Attributes.Add("onClick", "javascript:return confirm('" + Localization.GetString("ConfirmEventCopy", LocalResourceFile) + "');")
                copyButton.Text = Localization.GetString("copySeriesButton", LocalResourceFile)
            Else
                deleteButton.Attributes.Add("onClick", "javascript:return confirm('" + Localization.GetString("ConfirmEventDelete", LocalResourceFile) + "');")
                deleteButton.Text = Localization.GetString("deleteButton", LocalResourceFile)
                updateButton.Text = Localization.GetString("updateButton", LocalResourceFile)
                copyButton.Attributes.Add("onClick", "javascript:return confirm('" + Localization.GetString("ConfirmEventCopy", LocalResourceFile) + "');")
                copyButton.Text = Localization.GetString("copyButton", LocalResourceFile)
            End If
            lnkSelectedEmail.Attributes.Add("onClick", "javascript:return confirm('" + Localization.GetString("ConfirmSendToAllSelected", LocalResourceFile) + "');")
            lnkSelectedDelete.Attributes.Add("onClick", "javascript:return confirm('" + Localization.GetString("ConfirmDeleteSelected", LocalResourceFile) + "');")

            txtPayPalAccount.Text = Settings.Paypalaccount

            Dim iInterval As Integer = CInt(Settings.Timeinterval)
            Dim currentDate As DateTime = ModuleNow()
            Dim currentMinutes As Integer = currentDate.Minute
            Dim remainder As Integer = currentMinutes Mod iInterval
            If remainder > 0 Then
                currentDate = currentDate.AddMinutes(iInterval - remainder)
            End If

            tpStartTime.TimeView.Interval = New TimeSpan(0, iInterval, 0)
            tpStartTime.SelectedDate = currentDate
            tpEndTime.TimeView.Interval = New TimeSpan(0, iInterval, 0)
            tpEndTime.SelectedDate = currentDate.AddMinutes(iInterval)


            ' Can this event be moderated? 
            lblModerated.Visible = Settings.Moderateall

            ' Send Reminder Default
            chkReminder.Checked = Settings.Sendreminderdefault

            ' Populate description
            ftbDesktopText.Text = Settings.Templates.NewEventTemplate

            ' Set default validation value
            valNoEnrolees.MaximumValue = "9999"

            ' Hide enrolment info by default
            lblEnrolledUsers.Visible = False
            grdEnrollment.Visible = False
            lnkSelectedDelete.Visible = False
            lnkSelectedEmail.Visible = False

            trAllowAnonEnroll.Visible = Settings.AllowAnonEnroll

            ' Populate enrollment email text boxes
            txtEventEmailSubject.Text = Settings.Templates.txtEditViewEmailSubject
            txtEventEmailBody.Text = Settings.Templates.txtEditViewEmailBody

            If _itemID <> -1 Then
                ' Edit Item Mode 
                Dim objEvent As EventInfo
                objEvent = _objCtlEvent.EventsGet(_itemID, ModuleId)

                Dim blEventSignup As Boolean
                If Settings.Eventsignup Then
                    blEventSignup = objEvent.Signups
                Else
                    blEventSignup = False
                End If
                If blEventSignup Then
                    pnlEnroll.Visible = True
                    chkSignups.Visible = True
                    lnkSelectedDelete.Visible = True
                    lnkSelectedEmail.Visible = True
                    If _itemID <> 0 Then
                        tblEventEmail.Attributes.Add("style", "display:block; width:100%")
                    End If
                End If

                ' Check user has edit permissions to this event
                If IsEventEditor(objEvent, False) Then
                Else
                    Response.Redirect(GetSocialNavigateUrl(), True)
                End If

                ' Create an object to consolidate master/single data into - use master object for common data
                Dim objEventData As EventRecurMasterInfo
                objEventData = _objCtlEventRecurMaster.EventsRecurMasterGet(objEvent.RecurMasterID, objEvent.ModuleID)

                ' Hide recurrences section, disable timezone change if it is a recurring event 
                ' and we aren't editing the series
                If objEventData.RRULE <> "" And Not _editRecur Then
                    pnlRecurring.Visible = False
                    cboTimeZone.Enabled = False
                End If

                ' If we are editing single item, populate with single event data
                If Not _editRecur Then
                    With objEventData
                        .Dtstart = objEvent.EventTimeBegin
                        .Duration = CType(objEvent.Duration, String) + "M"
                        .Until = objEvent.EventTimeBegin
                        .RRULE = ""
                        .EventName = objEvent.EventName
                        .EventDesc = objEvent.EventDesc
                        .Importance = CType(objEvent.Importance, EventRecurMasterInfo.Priority)
                        .Notify = objEvent.Notify
                        .Approved = objEvent.Approved
                        .Signups = objEvent.Signups
                        .AllowAnonEnroll = objEvent.AllowAnonEnroll
                        .JournalItem = objEvent.JournalItem
                        .MaxEnrollment = objEvent.MaxEnrollment
                        .EnrollRoleID = objEvent.EnrollRoleID
                        .EnrollFee = objEvent.EnrollFee
                        .EnrollType = objEvent.EnrollType
                        .Enrolled = objEvent.Enrolled
                        .PayPalAccount = objEvent.PayPalAccount
                        .DetailPage = objEvent.DetailPage
                        .DetailNewWin = objEvent.DetailNewWin
                        .DetailURL = objEvent.DetailURL
                        .ImageURL = objEvent.ImageURL
                        .ImageType = objEvent.ImageType
                        .ImageWidth = objEvent.ImageWidth
                        .ImageHeight = objEvent.ImageHeight
                        .ImageDisplay = objEvent.ImageDisplay
                        .Location = objEvent.Location
                        .Category = objEvent.Category
                        .Reminder = objEvent.Reminder
                        .SendReminder = objEvent.SendReminder
                        .ReminderTime = objEvent.ReminderTime
                        .ReminderTimeMeasurement = objEvent.ReminderTimeMeasurement
                        .ReminderFrom = objEvent.ReminderFrom
                        .CustomField1 = objEvent.CustomField1
                        .CustomField2 = objEvent.CustomField2
                        .EnrollListView = objEvent.EnrollListView
                        .DisplayEndDate = objEvent.DisplayEndDate
                        .AllDayEvent = objEvent.AllDayEvent
                        .OwnerID = objEvent.OwnerID
                        .SocialGroupID = objEvent.SocialGroupId
                        .SocialUserID = objEvent.SocialUserId
                        .Summary = objEvent.Summary
                    End With
                End If
                Dim intDuration As Integer
                intDuration = CInt(Left(objEventData.Duration, Len(objEventData.Duration) - 1))
                txtTitle.Text = objEventData.EventName
                ftbDesktopText.Text = objEventData.EventDesc

                ' Set Dropdown to Original TimeZone w/ModuleID Settings TimeZone
                cboTimeZone.DataBind(objEvent.EventTimeZoneId)

                ' Set dates/times
                dpStartDate.SelectedDate = objEventData.Dtstart.Date()
                dpEndDate.SelectedDate = objEventData.Dtstart.AddMinutes(intDuration).Date
                dpRecurEndDate.SelectedDate = objEventData.Until

                ' Adjust Time not in DropDown Selection...
                Dim starttime As DateTime = objEventData.Dtstart
                If starttime.Minute Mod iInterval > 0 Then
                    starttime = objEventData.Dtstart.Date.AddMinutes((objEventData.Dtstart.Hour * 60) + (starttime.Minute) - (starttime.Minute Mod iInterval))
                End If
                tpStartTime.SelectedDate = starttime

                Dim endtime As DateTime = objEventData.Dtstart.AddMinutes(intDuration)
                If endtime.Minute Mod iInterval > 0 Then
                    endtime = objEventData.Dtstart.AddMinutes(intDuration).Date.AddMinutes((objEventData.Dtstart.AddMinutes(intDuration).Hour * 60) + (endtime.Minute) - (endtime.Minute Mod iInterval))
                End If
                tpEndTime.SelectedDate = endtime

                chkSignups.Checked = objEventData.Signups
                chkAllowAnonEnroll.Checked = objEventData.AllowAnonEnroll
                txtMaxEnrollment.Text = objEventData.MaxEnrollment.ToString
                txtEnrolled.Text = objEventData.Enrolled.ToString
                txtPayPalAccount.Text = objEventData.PayPalAccount.ToString
                If objEventData.EnrollType = "PAID" Then
                    rblFree.Checked = False
                    rblPaid.Checked = True
                ElseIf objEventData.EnrollType = "FREE" Then
                    rblFree.Checked = True
                    rblPaid.Checked = False
                End If

                txtEnrollFee.Text = String.Format("{0:F2}", objEventData.EnrollFee)
                lblTotalCurrency.Text = PortalSettings.Currency

                If blEventSignup Then
                    ' Load Enrolled User Grid
                    BuildEnrolleeGrid(objEvent)
                End If

                If IsNumeric(objEventData.EnrollRoleID) Then
                    LoadEnrollRoles(CInt(objEventData.EnrollRoleID))
                    LoadNewEventEmailRoles(CInt(objEventData.EnrollRoleID))
                Else
                    LoadEnrollRoles(-1)
                    LoadNewEventEmailRoles(-1)
                End If

                LoadCategory(objEventData.Category)
                LoadLocation(objEventData.Location)
                LoadOwnerUsers(objEventData.OwnerID)

                cmbImportance.SelectedIndex = CType(GetCmbStatus(objEventData.Importance, "cmbImportance"), Int16)
                CreatedBy.Text = objEvent.CreatedBy
                CreatedDate.Text = objEventData.CreatedDate.ToShortDateString
                lblCreatedBy.Visible = True
                CreatedBy.Visible = True
                lblOn.Visible = True
                CreatedDate.Visible = True
                pnlAudit.Visible = True

                Dim objEventRRULE As EventRRULEInfo
                objEventRRULE = _objCtlEventRecurMaster.DecomposeRRULE(objEventData.RRULE, objEventData.Dtstart)
                Dim strRepeatType As String = _objCtlEventRecurMaster.RepeatType(objEventRRULE)

                Select Case strRepeatType
                    Case "N"
                        rblRepeatTypeN.Checked = True
                        cmbP1Period.SelectedIndex = 0
                        'txtP1Every.Text = "0"
                    Case "P1"
                        rblRepeatTypeP1.Checked = True
                        chkReccuring.Checked = True
                        cmbP1Period.SelectedIndex = CType(GetCmbStatus(Left(objEventRRULE.Freq, 1), "cmbP1Period"), Int16)
                        txtP1Every.Text = objEventRRULE.Interval.ToString()
                    Case "W1"
                        rblRepeatTypeW1.Checked = True
                        chkReccuring.Checked = True
                        If _culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Sunday Then
                            chkW1Sun.Checked = objEventRRULE.Su
                        Else
                            chkW1Sun2.Checked = objEventRRULE.Su
                        End If
                        chkW1Mon.Checked = objEventRRULE.Mo
                        chkW1Tue.Checked = objEventRRULE.Tu
                        chkW1Wed.Checked = objEventRRULE.We
                        chkW1Thu.Checked = objEventRRULE.Th
                        chkW1Fri.Checked = objEventRRULE.Fr
                        chkW1Sat.Checked = objEventRRULE.Sa
                        txtW1Every.Text = objEventRRULE.Interval.ToString()
                    Case "M1"
                        rblRepeatTypeM1.Checked = True
                        rblRepeatTypeM.Checked = True
                        chkReccuring.Checked = True
                        txtMEvery.Text = objEventRRULE.Interval.ToString()
                        Dim intEvery, intPeriod As Integer
                        If objEventRRULE.Su Then
                            intPeriod = 0
                            intEvery = objEventRRULE.SuNo
                        End If
                        If objEventRRULE.Mo Then
                            intPeriod = 1
                            intEvery = objEventRRULE.MoNo()
                        End If
                        If objEventRRULE.Tu Then
                            intPeriod = 2
                            intEvery = objEventRRULE.TuNo
                        End If
                        If objEventRRULE.We Then
                            intPeriod = 3
                            intEvery = objEventRRULE.WeNo
                        End If
                        If objEventRRULE.Th Then
                            intPeriod = 4
                            intEvery = objEventRRULE.ThNo
                        End If
                        If objEventRRULE.Fr Then
                            intPeriod = 5
                            intEvery = objEventRRULE.FrNo
                        End If
                        If objEventRRULE.Sa Then
                            intPeriod = 6
                            intEvery = objEventRRULE.SaNo
                        End If
                        cmbM1Period.SelectedIndex = intPeriod
                        If intEvery = -1 Then
                            cmbM1Every.SelectedIndex = 4
                        Else
                            cmbM1Every.SelectedIndex = intEvery - 1
                        End If
                    Case "M2"
                        rblRepeatTypeM2.Checked = True
                        rblRepeatTypeM.Checked = True
                        chkReccuring.Checked = True
                        cmbM2Period.SelectedIndex = objEventRRULE.ByMonthDay - 1
                        txtMEvery.Text = objEventRRULE.Interval.ToString()
                    Case "Y1"
                        rblRepeatTypeY1.Checked = True
                        chkReccuring.Checked = True
                        Dim invCulture As CultureInfo = CultureInfo.InvariantCulture
                        Dim annualdate As Date = Date.ParseExact(Format(objEventData.Dtstart, "yyyy") & "/" & objEventRRULE.ByMonth.ToString & "/" & objEventRRULE.ByMonthDay.ToString, "yyyy/M/d", invCulture)
                        dpY1Period.SelectedDate = annualdate.Date

                End Select

                chkReminder.Checked = objEventData.SendReminder
                txtReminder.Text = objEventData.Reminder

                If txtReminder.Text.Length = 0 Then
                    txtReminder.Text = Settings.Templates.txtMessage
                End If

                txtSubject.Text = objEventData.Notify
                If txtSubject.Text.Length = 0 Then
                    txtSubject.Text = Settings.Templates.txtSubject
                End If

                txtReminderFrom.Text = objEventData.ReminderFrom
                txtEventEmailFrom.Text = objEventData.ReminderFrom
                If objEventData.ReminderTime < 0 Then
                    txtReminderTime.Text = 15.ToString()
                Else
                    txtReminderTime.Text = objEventData.ReminderTime.ToString
                End If

                If Not ddlReminderTimeMeasurement.Items.FindByValue(objEventData.ReminderTimeMeasurement.ToString) Is Nothing Then
                    ddlReminderTimeMeasurement.ClearSelection()
                    ddlReminderTimeMeasurement.Items.FindByValue(objEventData.ReminderTimeMeasurement.ToString).Selected = True
                End If

                ' Set DetailURL
                chkDetailPage.Checked = objEventData.DetailPage
                URLDetail.Url = objEventData.DetailURL


                ' Set Image Control
                chkDisplayImage.Checked = objEventData.ImageDisplay
                ctlURL.Url = objEventData.ImageURL
                If objEventData.ImageURL.StartsWith("FileID=") Then
                    Dim fileId As Integer = Integer.Parse(objEventData.ImageURL.Substring(7))
                    Dim objFileInfo As Services.FileSystem.IFileInfo = Services.FileSystem.FileManager.Instance.GetFile(fileId)
                    If Not objFileInfo Is Nothing Then
                        ctlURL.Url = objFileInfo.Folder & objFileInfo.FileName
                    Else
                        chkDisplayImage.Checked = False
                    End If
                End If

                If objEventData.ImageWidth <> 0 And objEventData.ImageWidth <> -1 Then
                    txtWidth.Text = objEventData.ImageWidth.ToString()
                End If

                If objEventData.ImageHeight <> 0 And objEventData.ImageHeight <> -1 Then
                    txtHeight.Text = objEventData.ImageHeight.ToString()
                End If
                txtCustomField1.Text = objEventData.CustomField1
                txtCustomField2.Text = objEventData.CustomField2
                chkEnrollListView.Checked = objEventData.EnrollListView
                chkDisplayEndDate.Checked = objEventData.DisplayEndDate
                chkAllDayEvent.Checked = objEventData.AllDayEvent
                ftbSummary.Text = objEventData.Summary

                If blEventSignup Then
                    LoadRegUsers()
                End If

            Else
                dpStartDate.SelectedDate = SelectedDate.Date
                dpEndDate.SelectedDate = SelectedDate.Date
                txtEnrollFee.Text = String.Format("{0:F2}", 0.0)
                lblTotalCurrency.Text = PortalSettings.Currency
                dpRecurEndDate.SelectedDate = SelectedDate.AddDays(1)
                dpY1Period.SelectedDate = SelectedDate.Date
                chkEnrollListView.Checked = Settings.Eventdefaultenrollview
                chkDisplayEndDate.Checked = True
                chkAllDayEvent.Checked = False

                ' Do not default recurrance end date
                ' Force user to key/select
                LoadEnrollRoles(-1)
                LoadNewEventEmailRoles(-1)
                LoadCategory()
                LoadLocation(-1)
                LoadOwnerUsers(UserId)
                pnlAudit.Visible = False
                deleteButton.Visible = False
            End If

            If _itemID = -1 Or _editRecur Then
                divAddUser.Visible = False
                divNoEnrolees.Visible = False
            End If

            Dim errorminutes As String = Localization.GetString("invalidReminderMinutes", LocalResourceFile)
            Dim errorhours As String = Localization.GetString("invalidReminderHours", LocalResourceFile)
            Dim errordays As String = Localization.GetString("invalidReminderDays", LocalResourceFile)
            ddlReminderTimeMeasurement.Attributes.Add("onchange", "valRemTime('" & valReminderTime.ClientID & "','" & valReminderTime2.ClientID & "','" & valReminderTime.ValidationGroup & "','" & ddlReminderTimeMeasurement.ClientID & "','" & errorminutes & "','" & errorhours & "','" & errordays & "');")
            Select Case ddlReminderTimeMeasurement.SelectedValue
                Case "m"
                    valReminderTime.ErrorMessage = errorminutes
                    valReminderTime.MinimumValue = "15"
                    valReminderTime.MaximumValue = "60"
                Case "h"
                    valReminderTime.ErrorMessage = errorhours
                    valReminderTime.MinimumValue = "1"
                    valReminderTime.MaximumValue = "24"
                Case "d"
                    valReminderTime.ErrorMessage = errordays
                    valReminderTime.MinimumValue = "1"
                    valReminderTime.MaximumValue = "30"
            End Select
            valReminderTime2.ErrorMessage = valReminderTime.ErrorMessage

            If txtPayPalAccount.Text.Length = 0 Then
                txtPayPalAccount.Text = Settings.Paypalaccount
            End If

            trCustomField1.Visible = Settings.EventsCustomField1
            trCustomField2.Visible = Settings.EventsCustomField2

            trTimeZone.Visible = Settings.Tzdisplay

            chkDetailPage.Attributes.Add("onclick", "javascript:showTbl('" + chkDetailPage.ClientID + "','" + tblDetailPageDetail.ClientID + "');")
            chkReminder.Attributes.Add("onclick", "javascript:showTbl('" + chkReminder.ClientID + "','" + tblReminderDetail.ClientID + "');")
            chkDisplayImage.Attributes.Add("onclick", "javascript:showTbl('" + chkDisplayImage.ClientID + "','" + tblImageURL.ClientID + "');")
            chkReccuring.Attributes.Add("onclick", "javascript:if (this.checked == true) dnn.dom.getById('" + rblRepeatTypeP1.ClientID + "').checked = true; else dnn.dom.getById('" + rblRepeatTypeN.ClientID + "').checked = true;showhideTbls('" + RecurTableDisplayType + "','" + chkReccuring.ClientID + "','" + tblRecurringDetails.ClientID + "','" + rblRepeatTypeP1.ClientID + "','" + tblDetailP1.ClientID + "','" + rblRepeatTypeW1.ClientID + "','" + tblDetailW1.ClientID + "','" + rblRepeatTypeM.ClientID + "','" + tblDetailM1.ClientID + "','" + rblRepeatTypeY1.ClientID + "','" + tblDetailY1.ClientID + "');")
            If Settings.Eventsignup Then
                chkEventEmailChk.Attributes.Add("onclick", "javascript:showhideChk2('" + chkEventEmailChk.ClientID + "','" + tblEventEmailRoleDetail.ClientID + "','" + chkSignups.ClientID + "','" + tblEventEmail.ClientID + "');")
            Else
                chkEventEmailChk.Attributes.Add("onclick", "javascript:showhideChk2('" + chkEventEmailChk.ClientID + "','" + tblEventEmailRoleDetail.ClientID + "','" + chkEventEmailChk.ClientID + "','" + tblEventEmail.ClientID + "');")
            End If
            rblRepeatTypeP1.Attributes.Add("onclick", "javascript:showhideTbls('" + RecurTableDisplayType + "','" + chkReccuring.ClientID + "','" + tblRecurringDetails.ClientID + "','" + rblRepeatTypeP1.ClientID + "','" + tblDetailP1.ClientID + "','" + rblRepeatTypeW1.ClientID + "','" + tblDetailW1.ClientID + "','" + rblRepeatTypeM.ClientID + "','" + tblDetailM1.ClientID + "','" + rblRepeatTypeY1.ClientID + "','" + tblDetailY1.ClientID + "');")
            rblRepeatTypeW1.Attributes.Add("onclick", "javascript:showhideTbls('" + RecurTableDisplayType + "','" + chkReccuring.ClientID + "','" + tblRecurringDetails.ClientID + "','" + rblRepeatTypeP1.ClientID + "','" + tblDetailP1.ClientID + "','" + rblRepeatTypeW1.ClientID + "','" + tblDetailW1.ClientID + "','" + rblRepeatTypeM.ClientID + "','" + tblDetailM1.ClientID + "','" + rblRepeatTypeY1.ClientID + "','" + tblDetailY1.ClientID + "');")
            rblRepeatTypeM.Attributes.Add("onclick", "javascript:showhideTbls('" + RecurTableDisplayType + "','" + chkReccuring.ClientID + "','" + tblRecurringDetails.ClientID + "','" + rblRepeatTypeP1.ClientID + "','" + tblDetailP1.ClientID + "','" + rblRepeatTypeW1.ClientID + "','" + tblDetailW1.ClientID + "','" + rblRepeatTypeM.ClientID + "','" + tblDetailM1.ClientID + "','" + rblRepeatTypeY1.ClientID + "','" + tblDetailY1.ClientID + "');if (this.checked == true) dnn.dom.getById('" + rblRepeatTypeM1.ClientID + "').checked = true;")
            rblRepeatTypeY1.Attributes.Add("onclick", "javascript:showhideTbls('" + RecurTableDisplayType + "','" + chkReccuring.ClientID + "','" + tblRecurringDetails.ClientID + "','" + rblRepeatTypeP1.ClientID + "','" + tblDetailP1.ClientID + "','" + rblRepeatTypeW1.ClientID + "','" + tblDetailW1.ClientID + "','" + rblRepeatTypeM.ClientID + "','" + tblDetailM1.ClientID + "','" + rblRepeatTypeY1.ClientID + "','" + tblDetailY1.ClientID + "');")
            btnCopyStartdate.Attributes.Add("onclick", String.Format("javascript:CopyStartDateToEnddate('{0}','{1}','{2}','{3}','{4}');", dpStartDate.ClientID, dpEndDate.ClientID, tpStartTime.ClientID, tpEndTime.ClientID, chkAllDayEvent.ClientID))
            chkAllDayEvent.Attributes.Add("onclick", "javascript:showTimes('" + chkAllDayEvent.ClientID + "','" + divStartTime.ClientID + "','" + divEndTime.ClientID + "');")

        End Sub

        Public Function GetCmbStatus(ByVal value As Object, ByVal cmbDropDown As String) As Long
            Dim iIndex As Integer
            Dim oDropDown As DropDownList

            oDropDown = CType(FindControl(cmbDropDown), DropDownList)
            For iIndex = 0 To oDropDown.Items.Count - 1
                If oDropDown.Items(iIndex).Value = CType(value, String) Then
                    Return iIndex
                End If
            Next
        End Function

        Public Sub LoadEnrollRoles(ByVal roleID As Integer)
            Dim objRoles As New RoleController
            ddEnrollRoles.DataSource = objRoles.GetRoles(PortalId)
            ddEnrollRoles.DataTextField = "RoleName"
            ddEnrollRoles.DataValueField = "RoleID"
            ddEnrollRoles.DataBind()
            '"<None Specified>"
            ddEnrollRoles.Items.Insert(0, New ListItem(Localization.GetString("None", LocalResourceFile), "-1"))
            If roleID = 0 Then
                ddEnrollRoles.Items.FindByValue("0").Selected() = True
            ElseIf (roleID > 0) Then
                If ddEnrollRoles.Items.FindByValue(CType(roleID, String)) Is Nothing Then
                    ddEnrollRoles.Items.Insert(0, New ListItem(Localization.GetString("EnrolleeRoleDeleted", LocalResourceFile), roleID.ToString))
                End If
                ddEnrollRoles.Items.FindByValue(CType(roleID, String)).Selected = True
            End If
        End Sub

        Public Sub LoadNewEventEmailRoles(ByVal roleID As Integer)
            Dim objRoles As New RoleController
            ddEventEmailRoles.DataSource = objRoles.GetRoles(PortalId)
            ddEventEmailRoles.DataTextField = "RoleName"
            ddEventEmailRoles.DataValueField = "RoleID"
            ddEventEmailRoles.DataBind()
            If roleID < 0 Or ddEventEmailRoles.Items.FindByValue(CType(roleID, String)) Is Nothing Then
                Try
                    ddEventEmailRoles.Items.FindByValue(PortalSettings.RegisteredRoleId.ToString).Selected() = True
                Catch
                End Try
            Else
                ddEventEmailRoles.Items.FindByValue(CType(roleID, String)).Selected = True
            End If
        End Sub

        Public Sub LoadCategory(Optional ByVal category As Integer = Nothing)
            Dim objCntCategories As New EventCategoryController
            Dim tmpCategories As ArrayList = objCntCategories.EventsCategoryList(PortalId)
            Dim objCategories As New ArrayList
            If (Settings.Enablecategories = EventModuleSettings.DisplayCategories.DoNotDisplay And Settings.ModuleCategoriesSelected = EventModuleSettings.CategoriesSelected.Some) _
               Or Settings.Restrictcategories Then
                For Each objCategory As EventCategoryInfo In tmpCategories
                    For Each moduleCategory As Integer In Settings.ModuleCategoryIDs
                        If moduleCategory = objCategory.Category Then
                            objCategories.Add(objCategory)
                        End If
                    Next
                Next
            Else
                objCategories = tmpCategories
            End If
            cmbCategory.DataSource = objCategories
            cmbCategory.DataTextField = "CategoryName"
            cmbCategory.DataValueField = "Category"
            cmbCategory.DataBind()

            ' Do we need to add None?
            If (Not Settings.Enablecategories = EventModuleSettings.DisplayCategories.DoNotDisplay Or Settings.ModuleCategoriesSelected = EventModuleSettings.CategoriesSelected.All) _
                And Settings.Restrictcategories = False Then
                cmbCategory.Items.Insert(0, New ListItem(Localization.GetString("None", LocalResourceFile), "-1"))
            End If

            ' Select the appropriate row
            If Settings.ModuleCategoriesSelected = EventModuleSettings.CategoriesSelected.All Then
                cmbCategory.ClearSelection()
                cmbCategory.Items(0).Selected = True
            End If

            If (category > 0) And Not IsNothing(category) Then
                cmbCategory.ClearSelection()
                cmbCategory.Items.FindByValue(CType(category, String)).Selected = True
            ElseIf Not Settings.Enablecategories = EventModuleSettings.DisplayCategories.DoNotDisplay And Settings.ModuleCategoriesSelected = EventModuleSettings.CategoriesSelected.Some Then
                cmbCategory.ClearSelection()
                cmbCategory.Items.FindByValue(CType(Settings.ModuleCategoryIDs.Item(0), String)).Selected = True
            End If

        End Sub

        Public Sub LoadLocation(ByVal location As Integer)
            Dim objCntLocation As New EventLocationController
            cmbLocation.DataSource = objCntLocation.EventsLocationList(PortalId)
            cmbLocation.DataTextField = "LocationName"
            cmbLocation.DataValueField = "Location"
            cmbLocation.DataBind()
            '"<None Specified>"
            cmbLocation.Items.Insert(0, New ListItem(Localization.GetString("None", LocalResourceFile), "-1"))
            If (location > 0) And Not IsNothing(location) Then
                cmbLocation.Items.FindByValue(CType(location, String)).Selected = True
            End If
        End Sub

        Private Sub LoadOwnerUsers(ByVal ownerID As Integer)
            Dim objCollModulePermission As ModulePermissionCollection
            objCollModulePermission = ModulePermissionController.GetModulePermissions(ModuleId, TabId)
            Dim objModulePermission As ModulePermissionInfo

            ' To cope with host users or someone who is no longer an editor!!
            Dim objEventModuleEditor As New EventUser
            objEventModuleEditor.UserID = ownerID
            LoadSingleUser(objEventModuleEditor, _lstOwnerUsers)

            If (IsModerator() And Settings.Ownerchangeallowed) Or _
               PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString) Then
                For Each objModulePermission In objCollModulePermission
                    If objModulePermission.PermissionKey = "EVENTSEDT" Then
                        If objModulePermission.UserID < 0 Then
                            Dim objCtlRole As New RoleController
                            Dim lstRoleUsers As ArrayList = CType(objCtlRole.GetUsersByRole(PortalId, objModulePermission.RoleName), ArrayList)
                            For Each objUser As UserInfo In lstRoleUsers
                                objEventModuleEditor = New EventUser
                                objEventModuleEditor.UserID = objUser.UserID
                                objEventModuleEditor.DisplayName = objUser.DisplayName
                                LoadSingleUser(objEventModuleEditor, _lstOwnerUsers)
                            Next
                        Else
                            objEventModuleEditor = New EventUser
                            objEventModuleEditor.UserID = objModulePermission.UserID
                            objEventModuleEditor.DisplayName = objModulePermission.DisplayName
                            LoadSingleUser(objEventModuleEditor, _lstOwnerUsers)
                        End If
                    End If
                Next
            End If
            _lstOwnerUsers.Sort(New UserListSort)

            cmbOwner.DataSource = _lstOwnerUsers
            cmbOwner.DataTextField = "DisplayName"
            cmbOwner.DataValueField = "UserID"
            cmbOwner.DataBind()
            cmbOwner.Items.FindByValue(CType(ownerID, String)).Selected = True
        End Sub

        Private Sub LoadRegUsers()
            grdAddUser.RefreshGrid()
        End Sub

        Private Sub LoadSingleUser(ByVal objEventUser As EventUser, ByVal lstUsers As ArrayList)
            Dim blAdd As Boolean = True
            Dim objEventUser2 As EventUser
            For Each objEventUser2 In lstUsers
                If objEventUser.UserID = objEventUser2.UserID Then
                    blAdd = False
                End If
            Next
            If blAdd Then
                If objEventUser.DisplayName = Nothing Then
                    Dim objCtlUser As New UserController
                    Dim objUser As UserInfo = objCtlUser.GetUser(PortalId, objEventUser.UserID)
                    If Not objUser Is Nothing Then
                        objEventUser.DisplayName = objUser.DisplayName
                    Else
                        objEventUser.DisplayName = Localization.GetString("OwnerDeleted", LocalResourceFile)
                    End If

                End If
                lstUsers.Add(objEventUser)
            End If
        End Sub

        Private Sub Email(ByVal selected As Boolean)
            Dim item As DataGridItem
            Dim objEnroll As EventSignupsInfo
            Dim objEvent As EventInfo
            ' Get Current Event, if <> 0
            If _itemID > 0 Then
                objEvent = _objCtlEvent.EventsGet(_itemID, ModuleId)
            Else
                Exit Sub
            End If

            Dim objEventEmailInfo As New EventEmailInfo
            Dim objEventEmail As New EventEmails(PortalId, ModuleId, LocalResourceFile, CType(Page, PageBase).PageCulture.Name)
            objEventEmailInfo.TxtEmailSubject = txtEventEmailSubject.Text
            objEventEmailInfo.TxtEmailBody = txtEventEmailBody.Text
            objEventEmailInfo.TxtEmailFrom() = txtEventEmailFrom.Text
            For Each item In grdEnrollment.Items
                If CType(item.FindControl("chkSelect"), CheckBox).Checked Or (selected = False) Then
                    objEnroll = _objCtlEventSignups.EventsSignupsGet(CType(grdEnrollment.DataKeys(item.ItemIndex), Integer), ModuleId, False)
                    objEventEmailInfo.UserIDs.Clear()
                    objEventEmailInfo.UserEmails.Clear()
                    objEventEmailInfo.UserLocales.Clear()
                    objEventEmailInfo.UserTimeZoneIds.Clear()
                    If objEnroll.UserID > -1 Then
                        objEventEmailInfo.UserIDs.Add(objEnroll.UserID)
                    Else
                        objEventEmailInfo.UserEmails.Add(objEnroll.AnonEmail)
                        objEventEmailInfo.UserLocales.Add(objEnroll.AnonCulture)
                        objEventEmailInfo.UserTimeZoneIds.Add(objEnroll.AnonTimeZoneId)
                    End If
                    objEventEmail.SendEmails(objEventEmailInfo, objEvent, objEnroll)
                End If
            Next
        End Sub

        Private Sub UpdateProcessing(ByVal processItem As Integer)
            If Not Page.IsValid Then
                Return
            End If

            Dim objSecurity As New PortalSecurity
            Dim tStartTime As DateTime
            Dim tEndTime As DateTime
            Dim tRecurEndDate As DateTime
            Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)

            ' Make EndDate = StartDate if no recurring dates
            If rblRepeatTypeN.Checked Then
                dpRecurEndDate.SelectedDate = ConvertDateStringstoDatetime(dpStartDate.SelectedDate.ToString, "00:00").Date
            End If

            valRequiredRecurEndDate.Validate()

            ' Make sure date formatted correctly
            If chkAllDayEvent.Checked Then
                tStartTime = ConvertDateStringstoDatetime(dpStartDate.SelectedDate.ToString, "00:00")
                tEndTime = ConvertDateStringstoDatetime(dpEndDate.SelectedDate.ToString, "00:00").AddMinutes(1439)
            Else
                tStartTime = ConvertDateStringstoDatetime(dpStartDate.SelectedDate.ToString, CDate(tpStartTime.SelectedDate).ToString("HH:mm", CultureInfo.InvariantCulture))
                tEndTime = ConvertDateStringstoDatetime(dpEndDate.SelectedDate.ToString, CDate(tpEndTime.SelectedDate).ToString("HH:mm", CultureInfo.InvariantCulture))
            End If

            If tEndTime < tStartTime And Not chkAllDayEvent.Checked Then
                valValidEndTime.ErrorMessage = Localization.GetString("valValidEndTime", LocalResourceFile)
                valValidEndTime.IsValid = False
                valValidEndTime.Visible = True
                Return
            End If

            tRecurEndDate = CType(dpRecurEndDate.SelectedDate, Date)

            If rblRepeatTypeP1.Checked Then
                valP1Every.Validate()
                valP1Every2.Validate()
                If valP1Every.IsValid = False Or valP1Every2.IsValid = False Then
                    Return
                End If
            End If

            If rblRepeatTypeW1.Checked Then
                valW1Day.Validate()
                valW1Day2.Validate()
                If (chkW1Sun.Checked = False And _
                    chkW1Sun2.Checked = False And _
                    chkW1Mon.Checked = False And _
                    chkW1Tue.Checked = False And _
                    chkW1Wed.Checked = False And _
                    chkW1Thu.Checked = False And _
                    chkW1Fri.Checked = False And _
                    chkW1Sat.Checked = False) Then
                    valW1Day3.ErrorMessage = Localization.GetString("valW1Day3", LocalResourceFile)
                    valW1Day3.Text = Localization.GetString("valW1Day3", LocalResourceFile)
                    valW1Day3.IsValid = False
                    valW1Day3.Visible = True
                    Return
                End If
                If valW1Day.IsValid = False Or valW1Day2.IsValid = False Then
                    Return
                End If
            End If

            If rblRepeatTypeM.Checked And rblRepeatTypeM2.Checked Then
                valM2Every.Validate()
                valM2Every2.Validate()
                If valM2Every.IsValid = False Or valM2Every2.IsValid = False Then
                    Return
                End If

            End If
            ' If Annual Recurrence, Check date
            If rblRepeatTypeY1.Checked Then
                valRequiredYearEventDate.Validate()
                valValidYearEventDate.Validate()
                If valRequiredYearEventDate.IsValid = False Or valValidYearEventDate.IsValid = False Then
                    Return
                End If
            End If

            If Settings.Expireevents <> "" _
               And Not _editRecur Then
                If tStartTime < Now.AddDays(-CType(Settings.Expireevents, Integer)) Then
                    valValidStartDate2.IsValid = False
                    valValidStartDate2.Visible = True
                    valValidStartDate2.Text = String.Format(Localization.GetString("valValidStartDate2", LocalResourceFile), CType(Settings.Expireevents, Integer))
                    valValidStartDate2.ErrorMessage = String.Format(Localization.GetString("valValidStartDate2", LocalResourceFile), CType(Settings.Expireevents, Integer))
                    Return
                End If
            End If

            Dim duration As Double
            duration = tEndTime.Subtract(tStartTime).TotalMinutes

            If rblPaid.Checked Then
                If IsNumeric(txtEnrollFee.Text) Then
                    ' ReSharper disable CompareOfFloatsByEqualityOperator
                    If CType(txtEnrollFee.Text, Double) = 0.0 Then
                        ' ReSharper restore CompareOfFloatsByEqualityOperator
                        valBadFee.IsValid = False
                        valBadFee.Visible = True
                        Return
                    End If
                Else
                    valBadFee.IsValid = False
                    valBadFee.Visible = True
                    Return
                End If
                If Trim(txtPayPalAccount.Text) = "" Then
                    valPayPalAccount.IsValid = False
                    valPayPalAccount.Visible = True
                    Return
                End If
            End If

            'Check valid Reminder Time
            If chkReminder.Checked Then
                Dim remtime As Integer = CType(txtReminderTime.Text, Integer)
                Select Case ddlReminderTimeMeasurement.SelectedValue
                    Case "m"
                        If remtime < 15 Or remtime > 60 Then
                            valReminderTime2.IsValid = False
                            valReminderTime2.Visible = True
                            Return
                        End If
                    Case "h"
                        If remtime < 1 Or remtime > 24 Then
                            valReminderTime2.IsValid = False
                            valReminderTime2.Visible = True
                            Return
                        End If
                    Case "d"
                        If remtime < 1 Or remtime > 30 Then
                            valReminderTime2.IsValid = False
                            valReminderTime2.Visible = True
                            Return
                        End If
                End Select
            End If

            If chkSignups.Checked Then
                valMaxEnrollment.Validate()
                If Not valMaxEnrollment.IsValid Then
                    Return
                End If
            End If

            valW1Day.Visible = False
            valW1Day2.Visible = False
            valW1Day3.Visible = False
            valConflict.Visible = False
            valLocationConflict.Visible = False
            valPayPalAccount.Visible = False
            valBadFee.Visible = False
            valValidRecurStartDate.Visible = False
            valNoEnrolees.Visible = False
            valMaxEnrollment.Visible = False

            ' Everythings Cool, Update Database
            Dim objEvent As EventInfo
            Dim objEventRecurMaster As New EventRecurMasterInfo
            Dim objEventRMSave As New EventRecurMasterInfo

            ' Get Current Event, if <> 0
            If processItem > 0 Then
                objEvent = _objCtlEvent.EventsGet(processItem, ModuleId)
                objEventRecurMaster = _objCtlEventRecurMaster.EventsRecurMasterGet(objEvent.RecurMasterID, objEvent.ModuleID)
                objEventRMSave = _objCtlEventRecurMaster.EventsRecurMasterGet(objEvent.RecurMasterID, objEvent.ModuleID)
                If _editRecur Then
                    _lstEvents = _objCtlEvent.EventsGetRecurrences(objEventRecurMaster.RecurMasterID, objEventRecurMaster.ModuleID)
                Else
                    _lstEvents.Add(objEvent)
                End If
            End If
            Dim intDuration As Integer
            With objEventRecurMaster
                .Dtstart = tStartTime
                .Duration = CType(duration, String) + "M"
                intDuration = CInt(duration)
                .Until = tRecurEndDate
                .UpdatedByID = UserId
                .OwnerID = CInt(cmbOwner.SelectedValue)
                If processItem < 0 Then
                    .RecurMasterID = -1
                    .CreatedByID = UserId
                    .ModuleID = ModuleId
                    .PortalID = PortalId
                    .CultureName = Threading.Thread.CurrentThread.CurrentCulture.Name
                    .JournalItem = False
                End If
                ' Filter text for non-admins and moderators
                If (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString) = True) Then
                    .EventName = txtTitle.Text
                    .EventDesc = ftbDesktopText.Text
                    .CustomField1 = txtCustomField1.Text
                    .CustomField2 = txtCustomField2.Text
                    .Notify = txtSubject.Text
                    .Reminder = txtReminder.Text
                    .Summary = ftbSummary.Text
                ElseIf IsModerator() Then
                    .EventName = objSecurity.InputFilter(txtTitle.Text, PortalSecurity.FilterFlag.NoScripting)
                    .EventDesc = ftbDesktopText.Text
                    .CustomField1 = objSecurity.InputFilter(txtCustomField1.Text, PortalSecurity.FilterFlag.NoScripting)
                    .CustomField2 = objSecurity.InputFilter(txtCustomField2.Text, PortalSecurity.FilterFlag.NoScripting)
                    .Notify = objSecurity.InputFilter(txtSubject.Text, PortalSecurity.FilterFlag.NoScripting)
                    .Reminder = objSecurity.InputFilter(txtReminder.Text, PortalSecurity.FilterFlag.NoScripting)
                    .Summary = ftbSummary.Text
                Else
                    .EventName = objSecurity.InputFilter(txtTitle.Text, PortalSecurity.FilterFlag.NoScripting)
                    .EventDesc = objSecurity.InputFilter(ftbDesktopText.Text, PortalSecurity.FilterFlag.NoScripting)
                    .CustomField1 = objSecurity.InputFilter(txtCustomField1.Text, PortalSecurity.FilterFlag.NoScripting)
                    .CustomField2 = objSecurity.InputFilter(txtCustomField2.Text, PortalSecurity.FilterFlag.NoScripting)
                    .Notify = objSecurity.InputFilter(txtSubject.Text, PortalSecurity.FilterFlag.NoScripting)
                    .Reminder = objSecurity.InputFilter(txtReminder.Text, PortalSecurity.FilterFlag.NoScripting)
                    .Summary = objSecurity.InputFilter(ftbSummary.Text, PortalSecurity.FilterFlag.NoScripting)
                End If

                ' If New Event
                If processItem < 0 Then
                    ' If Moderator turned on, set approve=false
                    If Settings.Moderateall Then
                        .Approved = False
                    Else
                        .Approved = True
                    End If
                End If

                ' Reset Approved, if Moderate All option is on
                If Settings.Moderateall And _
                    .Approved = True Then
                    .Approved = False
                End If

                ' If Admin or Moderator, automatically approve event
                If PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString) Or _
                    IsModerator() Then
                    .Approved = True
                End If

                .Importance = CType(cmbImportance.SelectedItem.Value, EventRecurMasterInfo.Priority)

                .Signups = chkSignups.Checked
                .AllowAnonEnroll = chkAllowAnonEnroll.Checked
                If rblFree.Checked Then
                    .EnrollType = "FREE"
                ElseIf rblPaid.Checked Then
                    .EnrollType = "PAID"
                End If

                .PayPalAccount = txtPayPalAccount.Text
                .EnrollFee = CDec(txtEnrollFee.Text)
                .MaxEnrollment = CType(txtMaxEnrollment.Text, Integer)

                If CInt(ddEnrollRoles.SelectedValue) <> -1 Then
                    .EnrollRoleID = CInt(ddEnrollRoles.SelectedItem.Value)
                Else
                    .EnrollRoleID = -1
                End If

                ' Update Detail Page setting in the database
                If chkDetailPage.Checked And URLDetail.Url <> "" Then
                    .DetailPage = True
                    .DetailURL = URLDetail.Url
                    .DetailNewWin = URLDetail.NewWindow
                Else
                    .DetailPage = False
                End If

                ' Update Image settings in the database
                If chkDisplayImage.Checked Then
                    .ImageDisplay = True
                    If ctlURL.UrlType = "F" Then
                        If ctlURL.Url.StartsWith("FileID=") Then
                            Dim fileId As Integer = Integer.Parse(ctlURL.Url.Substring(7))
                            Dim objFileInfo As Services.FileSystem.IFileInfo = Services.FileSystem.FileManager.Instance.GetFile(fileId)
                            If txtWidth.Text = "" Or txtWidth.Text = 0.ToString() Then
                                txtWidth.Text = objFileInfo.Width.ToString
                            End If
                            If txtHeight.Text = "" Or txtHeight.Text = 0.ToString() Then
                                txtHeight.Text = objFileInfo.Height.ToString
                            End If
                        End If
                    End If
                    .ImageURL = ctlURL.Url
                    .ImageType = ctlURL.UrlType
                Else
                    .ImageDisplay = False
                End If

                If txtWidth.Text = "" Then
                    .ImageWidth = 0
                Else
                    .ImageWidth = CInt(txtWidth.Text)
                End If
                If txtHeight.Text = "" Then
                    .ImageHeight = 0
                Else
                    .ImageHeight = CInt(txtHeight.Text)
                End If

                .Category = CInt(cmbCategory.SelectedValue)
                .Location = CInt(cmbLocation.SelectedValue)

                .SendReminder = chkReminder.Checked
                .ReminderTime = CInt(txtReminderTime.Text)
                .ReminderTimeMeasurement = ddlReminderTimeMeasurement.SelectedValue
                .ReminderFrom = txtReminderFrom.Text

                .EnrollListView = chkEnrollListView.Checked
                .DisplayEndDate = chkDisplayEndDate.Checked
                .AllDayEvent = chkAllDayEvent.Checked
                .EventTimeZoneId = cboTimeZone.SelectedValue
                .SocialGroupID = GetUrlGroupId()
                .SocialUserID = GetUrlUserId()


            End With

            ' If it is possible we are edititng a recurring event create RRULE
            If processItem = 0 Or _editRecur Or (Not _editRecur And objEventRMSave.RRULE = "") Then
                objEventRecurMaster = CreateEventRRULE(objEventRecurMaster)
                If rblRepeatTypeN.Checked Then
                    objEventRecurMaster.Until = objEventRecurMaster.Dtstart.Date
                End If
            End If

            ' If editing single occurence of recurring event & start date > last date, error
            If processItem > 0 And objEventRMSave.RRULE <> "" And Not _editRecur Then
                If tStartTime.Date > objEventRMSave.Until.Date Then
                    valValidRecurStartDate.IsValid = False
                    valValidRecurStartDate.Visible = True
                    Return
                End If
                If tStartTime.Date < objEventRMSave.Dtstart.Date Then
                    valValidRecurStartDate2.IsValid = False
                    valValidRecurStartDate2.Visible = True
                    Return
                End If
            End If

            ' If new Event or Recurring event then check for new instances
            If processItem < 0 Or _
               (objEventRMSave.RRULE = "" And objEventRecurMaster.RRULE <> "" And Not _editRecur) Or _
               _editRecur Then
                Dim lstEventsNew As ArrayList
                lstEventsNew = _objCtlEventRecurMaster.CreateEventRecurrences(objEventRecurMaster, intDuration, Settings.Maxrecurrences)
                _lstEvents = CompareOldNewEvents(_lstEvents, lstEventsNew)

                If lstEventsNew.Count = 0 Then
                    ' Last error!!
                    valValidRecurEndDate2.IsValid = False
                    valValidRecurEndDate2.Visible = True
                    Return
                End If
            End If

            For Each objEvent In _lstEvents
                With objEvent
                    If objEvent.EventID > 0 And objEvent.UpdateStatus <> "Delete" Then
                        objEvent.UpdateStatus = "Match"
                        Dim objEventSave As EventInfo = objEvent.Clone
                        If (_editRecur And objEvent.EventTimeBegin.ToShortTimeString = objEventRMSave.Dtstart.ToShortTimeString) Then
                            .EventTimeBegin = ConvertDateStringstoDatetime(.EventTimeBegin.ToShortDateString, Format(objEventRecurMaster.Dtstart, "HH:mm"))
                            If tRecurEndDate.Date < .EventTimeBegin.Date Then
                                tRecurEndDate = .EventTimeBegin.Date.AddDays(30)
                            End If
                        End If

                        If (_editRecur And CType(.Duration, String) & "M" = objEventRMSave.Duration) Or Not _editRecur Then
                            .Duration = intDuration
                        End If

                        If Not _editRecur Then
                            .EventTimeBegin = objEventRecurMaster.Dtstart
                            If tRecurEndDate.Date < .EventTimeBegin.Date Then
                                tRecurEndDate = .EventTimeBegin.Date.AddDays(30)
                            End If
                            .Duration = intDuration
                        End If

                        If (_editRecur And .EventName = objEventRMSave.EventName) Or Not _editRecur Then
                            .EventName = objEventRecurMaster.EventName
                        End If
                        If (_editRecur And .EventDesc = objEventRMSave.EventDesc) Or Not _editRecur Then
                            .EventDesc = objEventRecurMaster.EventDesc
                        End If

                        If (_editRecur And .Importance = objEventRMSave.Importance) Or Not _editRecur Then
                            .Importance = CType(objEventRecurMaster.Importance, EventInfo.Priority)
                        End If
                        If (_editRecur And .Signups = objEventRMSave.Signups) Or Not _editRecur Then
                            .Signups = objEventRecurMaster.Signups
                        End If
                        If (_editRecur And .JournalItem = objEventRMSave.JournalItem) Or Not _editRecur Then
                            .JournalItem = objEventRecurMaster.JournalItem
                        End If
                        If (_editRecur And .AllowAnonEnroll = objEventRMSave.AllowAnonEnroll) Or Not _editRecur Then
                            .AllowAnonEnroll = objEventRecurMaster.AllowAnonEnroll
                        End If
                        If (_editRecur And .EnrollType = objEventRMSave.EnrollType) Or Not _editRecur Then
                            .EnrollType = objEventRecurMaster.EnrollType
                        End If
                        If (_editRecur And .PayPalAccount = objEventRMSave.PayPalAccount) Or Not _editRecur Then
                            .PayPalAccount = objEventRecurMaster.PayPalAccount
                        End If
                        If (_editRecur And .EnrollFee = objEventRMSave.EnrollFee) Or Not _editRecur Then
                            .EnrollFee = objEventRecurMaster.EnrollFee
                        End If
                        If (_editRecur And .MaxEnrollment = objEventRMSave.MaxEnrollment) Or Not _editRecur Then
                            .MaxEnrollment = objEventRecurMaster.MaxEnrollment
                        End If
                        If (_editRecur And .EnrollRoleID = objEventRMSave.EnrollRoleID) Or Not _editRecur Then
                            .EnrollRoleID = objEventRecurMaster.EnrollRoleID
                        End If
                        If (_editRecur And .DetailPage = objEventRMSave.DetailPage) Or Not _editRecur Then
                            .DetailPage = objEventRecurMaster.DetailPage
                        End If
                        If (_editRecur And .DetailNewWin = objEventRMSave.DetailNewWin) Or Not _editRecur Then
                            .DetailNewWin = objEventRecurMaster.DetailNewWin
                        End If

                        If (_editRecur And .DetailURL = objEventRMSave.DetailURL) Or Not _editRecur Then
                            .DetailURL = objEventRecurMaster.DetailURL
                        End If

                        If (_editRecur And .ImageDisplay = objEventRMSave.ImageDisplay) Or Not _editRecur Then
                            .ImageDisplay = objEventRecurMaster.ImageDisplay
                        End If
                        If (_editRecur And .ImageType = objEventRMSave.ImageType) Or Not _editRecur Then
                            .ImageType = objEventRecurMaster.ImageType
                        End If

                        If (_editRecur And .ImageURL = objEventRMSave.ImageURL) Or Not _editRecur Then
                            .ImageURL = objEventRecurMaster.ImageURL
                        End If
                        If (_editRecur And .ImageWidth = objEventRMSave.ImageWidth) Or Not _editRecur Then
                            .ImageWidth = objEventRecurMaster.ImageWidth
                        End If
                        If (_editRecur And .ImageHeight = objEventRMSave.ImageHeight) Or Not _editRecur Then
                            .ImageHeight = objEventRecurMaster.ImageHeight
                        End If
                        If (_editRecur And .Category = objEventRMSave.Category) Or Not _editRecur Then
                            .Category = objEventRecurMaster.Category
                        End If
                        If (_editRecur And .Location = objEventRMSave.Location) Or Not _editRecur Then
                            .Location = objEventRecurMaster.Location
                        End If

                        ' Save Event Notification Info
                        If (_editRecur And .SendReminder = objEventRMSave.SendReminder) Or Not _editRecur Then
                            .SendReminder = objEventRecurMaster.SendReminder
                        End If
                        If (_editRecur And .Reminder = objEventRMSave.Reminder) Or Not _editRecur Then
                            .Reminder = objEventRecurMaster.Reminder
                        End If
                        If (_editRecur And .Notify = objEventRMSave.Notify) Or Not _editRecur Then
                            .Notify = objEventRecurMaster.Notify
                        End If
                        If (_editRecur And .ReminderTime = objEventRMSave.ReminderTime) Or Not _editRecur Then
                            .ReminderTime = objEventRecurMaster.ReminderTime
                        End If
                        If (_editRecur And .ReminderTimeMeasurement = objEventRMSave.ReminderTimeMeasurement) Or Not _editRecur Then
                            .ReminderTimeMeasurement = objEventRecurMaster.ReminderTimeMeasurement
                        End If
                        If (_editRecur And .ReminderFrom = objEventRMSave.ReminderFrom) Or Not _editRecur Then
                            .ReminderFrom = objEventRecurMaster.ReminderFrom
                        End If
                        If (_editRecur And .OwnerID = objEventRMSave.OwnerID) Or Not _editRecur Then
                            .OwnerID = objEventRecurMaster.OwnerID
                        End If

                        ' Set for re-submit to Search Engine
                        .SearchSubmitted = False

                        If (_editRecur And .CustomField1 = objEventRMSave.CustomField1) Or Not _editRecur Then
                            .CustomField1 = objEventRecurMaster.CustomField1
                        End If
                        If (_editRecur And .CustomField2 = objEventRMSave.CustomField2) Or Not _editRecur Then
                            .CustomField2 = objEventRecurMaster.CustomField2
                        End If
                        If (_editRecur And .EnrollListView = objEventRMSave.EnrollListView) Or Not _editRecur Then
                            .EnrollListView = objEventRecurMaster.EnrollListView
                        End If
                        If (_editRecur And .DisplayEndDate = objEventRMSave.DisplayEndDate) Or Not _editRecur Then
                            .DisplayEndDate = objEventRecurMaster.DisplayEndDate
                        End If
                        If (_editRecur And .AllDayEvent = objEventRMSave.AllDayEvent) Or Not _editRecur Then
                            .AllDayEvent = objEventRecurMaster.AllDayEvent
                        End If
                        If (_editRecur And .Summary = objEventRMSave.Summary) Or Not _editRecur Then
                            .Summary = objEventRecurMaster.Summary
                        End If

                        If .EventTimeBegin <> objEventSave.EventTimeBegin Or _
                           .Duration <> objEventSave.Duration Or _
                           .EventName <> objEventSave.EventName Or _
                           .EventDesc <> objEventSave.EventDesc Or _
                           .Importance <> objEventSave.Importance Or _
                           .Notify <> objEventSave.Notify Or _
                           .Signups <> objEventSave.Signups Or _
                           .AllowAnonEnroll <> objEventSave.AllowAnonEnroll Or _
                           .MaxEnrollment <> objEventSave.MaxEnrollment Or _
                           .EnrollRoleID <> objEventSave.EnrollRoleID Or _
                           .EnrollFee <> objEventSave.EnrollFee Or _
                           .EnrollType <> objEventSave.EnrollType Or _
                           .PayPalAccount <> objEventSave.PayPalAccount Or _
                           .DetailPage <> objEventSave.DetailPage Or _
                           .DetailNewWin <> objEventSave.DetailNewWin Or _
                           .DetailURL <> objEventSave.DetailURL Or _
                           .ImageURL <> objEventSave.ImageURL Or _
                           .ImageType <> objEventSave.ImageType Or _
                           .ImageWidth <> objEventSave.ImageWidth Or _
                           .ImageHeight <> objEventSave.ImageHeight Or _
                           .ImageDisplay <> objEventSave.ImageDisplay Or _
                           .Location <> objEventSave.Location Or _
                           .Category <> objEventSave.Category Or _
                           .Reminder <> objEventSave.Reminder Or _
                           .SendReminder <> objEventSave.SendReminder Or _
                           .ReminderTime <> objEventSave.ReminderTime Or _
                           .ReminderTimeMeasurement <> objEventSave.ReminderTimeMeasurement Or _
                           .ReminderFrom <> objEventSave.ReminderFrom Or _
                           .CustomField1 <> objEventSave.CustomField1 Or _
                           .CustomField2 <> objEventSave.CustomField2 Or _
                           .EnrollListView <> objEventSave.EnrollListView Or _
                           .DisplayEndDate <> objEventSave.DisplayEndDate Or _
                           .AllDayEvent <> objEventSave.AllDayEvent Or _
                           .Summary <> objEventSave.Summary Or _
                           .OwnerID <> objEventSave.OwnerID Then
                            .LastUpdatedID = UserId
                            .Approved = objEventRecurMaster.Approved
                            .UpdateStatus = "Update"
                        End If

                    End If

                    ' Do we need to check for schedule conflict?
                    If Settings.Preventconflicts And objEvent.UpdateStatus <> "Delete" Then
                        Dim getSubEvents As Boolean = Settings.MasterEvent
                        Dim categoryIDs As New ArrayList
                        categoryIDs.Add("-1")
                        Dim locationIDs As New ArrayList
                        locationIDs.Add("-1")
                        Dim selectedEvents As ArrayList = objEventInfoHelper.GetEvents(objEvent.EventTimeBegin.Date, objEvent.EventTimeBegin.AddMinutes(.Duration).Date, getSubEvents, categoryIDs, locationIDs, objEventRecurMaster.SocialGroupID, objEventRecurMaster.SocialUserID)
                        Dim conflictDateChk As DateTime = Now()
                        Dim conflictDate As DateTime = objEventInfoHelper.IsConflict(objEvent, selectedEvents, conflictDateChk)
                        If conflictDate <> conflictDateChk Then
                            'Conflict Error
                            If Settings.Locationconflict Then
                                valLocationConflict.IsValid = False
                                valLocationConflict.Visible = True
                                valLocationConflict.ErrorMessage = Localization.GetString("valLocationConflict", LocalResourceFile) + " - " + String.Format("{0:g}", conflictDate)
                                valLocationConflict.Text = Localization.GetString("valLocationConflict", LocalResourceFile) + " - " + String.Format("{0:g}", conflictDate)
                            Else
                                valConflict.IsValid = False
                                valConflict.Visible = True
                                valConflict.ErrorMessage = Localization.GetString("valConflict", LocalResourceFile) + " - " + String.Format("{0:g}", conflictDate)
                                valConflict.Text = Localization.GetString("valConflict", LocalResourceFile) + " - " + String.Format("{0:g}", conflictDate)
                            End If
                            Return
                        End If
                    End If

                End With
            Next

            If objEventRecurMaster.RecurMasterID = -1 Or _
               (objEventRMSave.RRULE = "" And Not _editRecur) Or _
               _editRecur Then
                objEventRecurMaster = _objCtlEventRecurMaster.EventsRecurMasterSave(objEventRecurMaster, TabId, True)
            End If

            If objEventRecurMaster.RecurMasterID = -1 Then
                SelectedDate = objEventRecurMaster.Dtstart.Date
            End If

            ' url tracking
            Dim objUrls As New UrlController
            objUrls.UpdateUrl(PortalId, ctlURL.Url, ctlURL.UrlType, ctlURL.Log, ctlURL.Track, ModuleId, ctlURL.NewWindow)
            objUrls.UpdateUrl(PortalId, URLDetail.Url, URLDetail.UrlType, URLDetail.Log, URLDetail.Track, ModuleId, URLDetail.NewWindow)

            Dim blEmailSend As Boolean = False
            Dim blModeratorEmailSent As Boolean = False
            Dim objEventEmail As New EventInfo
            For Each objEvent In _lstEvents
                objEvent.RecurMasterID = objEventRecurMaster.RecurMasterID
                Select Case objEvent.UpdateStatus
                    Case "Match"
                    Case "Delete"
                        _objCtlEvent.EventsDelete(objEvent.EventID, objEvent.ModuleID, objEvent.ContentItemID)
                    Case Else
                        If Not objEvent.Cancelled Then
                            Dim oEvent As EventInfo = objEvent
                            objEvent = _objCtlEvent.EventsSave(objEvent, False, TabId, True)
                            If Not oEvent.Approved And Not blModeratorEmailSent Then
                                oEvent.RRULE = objEventRecurMaster.RRULE
                                SendModeratorEmail(oEvent)
                                blModeratorEmailSent = True
                            End If
                            If oEvent.EventID <> -1 Then
                                UpdateExistingNotificationRecords(oEvent)
                            Else
                                If Not blEmailSend Then
                                    objEventEmail = objEvent
                                    blEmailSend = True
                                End If
                            End If
                        End If
                End Select
            Next
            If blEmailSend Then
                SendNewEventEmails(objEventEmail)
                CreateNewEventJournal(objEventEmail)
            End If
            If chkEventEmailChk.Checked Then
                SendEventEmail(CType(_lstEvents.Item(0), EventInfo))
            End If

        End Sub

        Private Function CreateEventRRULE(ByVal objEventRecurMaster As EventRecurMasterInfo) As EventRecurMasterInfo
            Dim strWkst As String
            Dim culture As New CultureInfo(objEventRecurMaster.CultureName, False)
            strWkst = "SU"
            If (culture.DateTimeFormat.FirstDayOfWeek <> DayOfWeek.Sunday) Then
                strWkst = "MO"
            End If

            If rblRepeatTypeN.Checked Then
                objEventRecurMaster.RRULE = ""
            ElseIf rblRepeatTypeP1.Checked Then
                Select Case Trim(cmbP1Period.SelectedItem.Value)
                    Case "D"
                        objEventRecurMaster.RRULE = "FREQ=DAILY"
                    Case "W"
                        objEventRecurMaster.RRULE = "FREQ=WEEKLY;WKST=" + strWkst
                    Case "M"
                        objEventRecurMaster.RRULE = "FREQ=MONTHLY"
                    Case "Y"
                        objEventRecurMaster.RRULE = "FREQ=YEARLY"
                End Select

                objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + ";INTERVAL=" + txtP1Every.Text
            ElseIf rblRepeatTypeW1.Checked Then
                objEventRecurMaster.RRULE = "FREQ=WEEKLY;WKST=" + strWkst + ";INTERVAL=" + txtW1Every.Text + ";BYDAY="
                If chkW1Sun.Checked Or chkW1Sun2.Checked Then
                    objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + "SU,"
                End If
                If chkW1Mon.Checked Then
                    objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + "MO,"
                End If
                If chkW1Tue.Checked Then
                    objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + "TU,"
                End If
                If chkW1Wed.Checked Then
                    objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + "WE,"
                End If
                If chkW1Thu.Checked Then
                    objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + "TH,"
                End If
                If chkW1Fri.Checked Then
                    objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + "FR,"
                End If
                If chkW1Sat.Checked Then
                    objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + "SA,"
                End If
                objEventRecurMaster.RRULE = Left(objEventRecurMaster.RRULE, Len(objEventRecurMaster.RRULE) - 1)
            ElseIf rblRepeatTypeM1.Checked And rblRepeatTypeM.Checked Then
                objEventRecurMaster.RRULE = "FREQ=MONTHLY;INTERVAL=" + txtMEvery.Text + ";BYDAY="
                Dim intWeek As Integer
                Dim strWeek As String
                If cmbM1Every.SelectedIndex < 4 Then
                    intWeek = cmbM1Every.SelectedIndex + 1
                    strWeek = "+" + Convert.ToString(intWeek)
                Else
                    intWeek = -1
                    strWeek = Convert.ToString(intWeek)
                End If
                objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + strWeek

                Dim strDay As String = ""
                Select Case cmbM1Period.SelectedValue
                    Case "0"
                        strDay = "SU"
                    Case "1"
                        strDay = "MO"
                    Case "2"
                        strDay = "TU"
                    Case "3"
                        strDay = "WE"
                    Case "4"
                        strDay = "TH"
                    Case "5"
                        strDay = "FR"
                    Case "6"
                        strDay = "SA"
                End Select
                objEventRecurMaster.RRULE = objEventRecurMaster.RRULE + strDay
            ElseIf rblRepeatTypeM2.Checked And rblRepeatTypeM.Checked Then
                objEventRecurMaster.RRULE = "FREQ=MONTHLY;INTERVAL=" + txtMEvery.Text + ";BYMONTHDAY=+" + cmbM2Period.SelectedValue
            ElseIf rblRepeatTypeY1.Checked Then
                Dim yearDate As DateTime = CType(dpY1Period.SelectedDate, Date)
                objEventRecurMaster.RRULE = "FREQ=YEARLY;INTERVAL=1;BYMONTH=" + Convert.ToString(yearDate.Month) + ";BYMONTHDAY=+" + Convert.ToString(yearDate.Day)
            End If
            Return objEventRecurMaster
        End Function

        Private Function CompareOldNewEvents(ByVal lstEventsOld As ArrayList, ByVal lstEventsNew As ArrayList) As ArrayList
            Dim objEventOld, objEventNew As EventInfo
            For Each objEventOld In lstEventsOld
                objEventOld.UpdateStatus = "Delete"
                For Each objEventNew In lstEventsNew
                    If objEventOld.OriginalDateBegin = objEventNew.OriginalDateBegin Then
                        objEventOld.UpdateStatus = "Match"
                    End If
                Next
            Next
            For Each objEventNew In lstEventsNew
                objEventNew.UpdateStatus = "Add"
                For Each objEventOld In lstEventsOld
                    If objEventOld.OriginalDateBegin = objEventNew.OriginalDateBegin Then
                        objEventNew.UpdateStatus = "Match"
                    End If
                Next
            Next
            For Each objEventNew In lstEventsNew
                If objEventNew.UpdateStatus = "Add" Then
                    lstEventsOld.Add(objEventNew)
                End If
            Next
            Return lstEventsOld
        End Function

        Private Sub UpdateExistingNotificationRecords(ByVal objEvent As EventInfo)
            Try
                ' Add Notification Records to Database, if required
                If chkReminder.Checked = True Then
                    Dim eventTimeBegin As DateTime
                    'Adjust Begin Time to UTC
                    eventTimeBegin = objEvent.EventTimeBegin
                    eventTimeBegin = eventTimeBegin.AddMinutes(0) 'only to pass to EventsNotificationTimeChange in correct format... 
                    ' Update Time for any existing Notifications for the Event
                    Dim objEventNotificationController As EventNotificationController = New EventNotificationController
                    objEventNotificationController.EventsNotificationTimeChange(objEvent.EventID, eventTimeBegin, ModuleId)
                End If
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub SendModeratorEmail(ByVal objEvent As EventInfo)
            Try
                ' Send Moderator email
                If Settings.Moderateall Then
                    Dim objEventEmailInfo As New EventEmailInfo
                    Dim objEventEmail As New EventEmails(PortalId, ModuleId, LocalResourceFile, CType(Page, PageBase).PageCulture.Name)
                    objEventEmailInfo.TxtEmailSubject = Settings.Templates.moderateemailsubject
                    objEventEmailInfo.TxtEmailBody = Settings.Templates.moderateemailmessage
                    objEventEmailInfo.TxtEmailFrom() = Settings.StandardEmail
                    Dim moderators As ArrayList = GetModerators()
                    For Each moderator As UserInfo In moderators
                        objEventEmailInfo.UserEmails.Add(moderator.Email)
                        objEventEmailInfo.UserLocales.Add(moderator.Profile.PreferredLocale)
                        objEventEmailInfo.UserTimeZoneIds.Add(moderator.Profile.PreferredTimeZone.Id)
                    Next
                    objEventEmail.SendEmails(objEventEmailInfo, objEvent)
                End If

            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub SendEventEmail(ByVal objEventEmailIn As EventInfo)
            Dim objEventEmailInfo As New EventEmailInfo
            Dim objEventEmail As New EventEmails(PortalId, ModuleId, LocalResourceFile, CType(Page, PageBase).PageCulture.Name)
            objEventEmailInfo.TxtEmailSubject = txtEventEmailSubject.Text
            objEventEmailInfo.TxtEmailBody = txtEventEmailBody.Text
            objEventEmailInfo.TxtEmailFrom() = txtEventEmailFrom.Text
            EventEmailAddRoleUsers(CInt(ddEventEmailRoles.SelectedValue), objEventEmailInfo)
            objEventEmail.SendEmails(objEventEmailInfo, objEventEmailIn)

        End Sub

        Private Function ConvertDateStringstoDatetime(ByVal strDate As String, ByVal strTime As String) As DateTime

            Dim invCulture As CultureInfo = CultureInfo.InvariantCulture

            Dim tDate As DateTime
            tDate = Convert.ToDateTime(strDate).Date

            ' Since dates may not be in a form directly combinable with time, convert back to string to enable combination
            strDate = tDate.ToString("yyyy/MM/dd", invCulture)
            tDate = Date.ParseExact(strDate + " " + strTime, "yyyy/MM/dd HH:mm", invCulture)
            Return tDate
        End Function

        Private Sub BuildEnrolleeGrid(ByVal objEvent As EventInfo)
            Dim objSignups As ArrayList
            ' Refresh Enrollment Grid
            If _editRecur Then
                objSignups = _objCtlEventSignups.EventsSignupsGetEventRecurMaster(objEvent.RecurMasterID, objEvent.ModuleID)
            Else
                objSignups = _objCtlEventSignups.EventsSignupsGetEvent(objEvent.EventID, objEvent.ModuleID)
            End If

            Dim eventEnrollment As New ArrayList
            Dim objSignup As EventSignupsInfo
            Dim objCtlUser As New UserController
            Dim noEnrolees As Integer = 0
            For Each objSignup In objSignups
                Dim objEnrollListItem As New EventEnrollList
                noEnrolees += objSignup.NoEnrolees
                If objSignup.UserID <> -1 Then
                    Dim objUser As UserInfo
                    objUser = objCtlUser.GetUser(PortalId, objSignup.UserID)
                    Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
                    objEnrollListItem.EnrollDisplayName = objEventInfoHelper.UserDisplayNameProfile(objSignup.UserID, objSignup.UserName, LocalResourceFile).DisplayNameURL
                    If Not objUser Is Nothing Then
                        objEnrollListItem.EnrollUserName = objUser.Username
                        objEnrollListItem.EnrollEmail = String.Format("<a href=""mailto:{0}?subject={1}"">{0}</a>", objSignup.Email, objEvent.EventName)
                        objEnrollListItem.EnrollPhone = objUser.Profile.Telephone
                    End If
                Else
                    objEnrollListItem.EnrollDisplayName = objSignup.AnonName
                    objEnrollListItem.EnrollUserName = Localization.GetString("AnonUser", LocalResourceFile)
                    objEnrollListItem.EnrollEmail = String.Format("<a href=""mailto:{0}?subject={1}"">{0}</a>", objSignup.AnonEmail, objEvent.EventName)
                    objEnrollListItem.EnrollPhone = objSignup.AnonTelephone
                End If
                objEnrollListItem.SignupID = objSignup.SignupID
                objEnrollListItem.EnrollApproved = objSignup.Approved
                objEnrollListItem.EnrollNo = objSignup.NoEnrolees
                objEnrollListItem.EnrollTimeBegin = objSignup.EventTimeBegin
                eventEnrollment.Add(objEnrollListItem)
            Next

            If eventEnrollment.Count > 0 Then
                grdEnrollment.DataSource = eventEnrollment
                grdEnrollment.DataBind()
                tblEventEmail.Attributes.Add("style", "display:block; width:100%")
                lblEnrolledUsers.Visible = True
                grdEnrollment.Visible = True
                lnkSelectedDelete.Visible = True
                lnkSelectedEmail.Visible = True
            Else
                lblEnrolledUsers.Visible = False
                grdEnrollment.Visible = False
                If Not Settings.Newpereventemail Then
                    tblEventEmail.Attributes.Add("style", "display:none; width:100%")
                End If
                lnkSelectedDelete.Visible = False
                lnkSelectedEmail.Visible = False
            End If

            objEvent.Enrolled = eventEnrollment.Count
            objEvent.Signups = True
            ShowHideEnrolleeColumns(objEvent)

            txtEnrolled.Text = noEnrolees.ToString
            valNoEnrolees.MaximumValue = CStr(objEvent.MaxEnrollment - noEnrolees)
            If CInt(valNoEnrolees.MaximumValue) > Settings.Maxnoenrolees Or objEvent.MaxEnrollment = 0 Then
                valNoEnrolees.MaximumValue = Settings.Maxnoenrolees.ToString
            ElseIf CInt(valNoEnrolees.MaximumValue) < 1 Then
                valNoEnrolees.MaximumValue = "1"
            End If
            lblMaxNoEnrolees.Text = String.Format(Localization.GetString("lblMaxNoEnrolees", LocalResourceFile), valNoEnrolees.MaximumValue)
        End Sub

        Private Sub ShowHideEnrolleeColumns(ByVal objEvent As EventInfo)
            Dim txtColumns As String = EnrolmentColumns(objEvent, True)
            Dim gvUsersToEnroll As GridView = CType(grdAddUser.FindControl("gvUsersToEnroll"), GridView)
            If txtColumns.LastIndexOf("UserName", StringComparison.Ordinal) < 0 Then
                grdEnrollment.Columns.Item(1).Visible = False
                gvUsersToEnroll.Columns.Item(1).Visible = False
            Else
                grdEnrollment.Columns.Item(1).Visible = True
                gvUsersToEnroll.Columns.Item(1).Visible = True
            End If
            If txtColumns.LastIndexOf("DisplayName", StringComparison.Ordinal) < 0 Then
                grdEnrollment.Columns.Item(2).Visible = False
                gvUsersToEnroll.Columns.Item(2).Visible = False
            Else
                grdEnrollment.Columns.Item(2).Visible = True
                gvUsersToEnroll.Columns.Item(2).Visible = True
            End If
            If txtColumns.LastIndexOf("Email", StringComparison.Ordinal) < 0 Then
                grdEnrollment.Columns.Item(3).Visible = False
                gvUsersToEnroll.Columns.Item(3).Visible = False
            Else
                grdEnrollment.Columns.Item(3).Visible = True
                gvUsersToEnroll.Columns.Item(3).Visible = True
            End If
            If txtColumns.LastIndexOf("Phone", StringComparison.Ordinal) < 0 Then
                grdEnrollment.Columns.Item(4).Visible = False
            Else
                grdEnrollment.Columns.Item(4).Visible = True
            End If
            If txtColumns.LastIndexOf("Approved", StringComparison.Ordinal) < 0 Then
                grdEnrollment.Columns.Item(5).Visible = False
            Else
                grdEnrollment.Columns.Item(5).Visible = True
            End If
            If txtColumns.LastIndexOf("Qty", StringComparison.Ordinal) < 0 Then
                grdEnrollment.Columns.Item(6).Visible = False
            Else
                grdEnrollment.Columns.Item(6).Visible = True
            End If
            If _editRecur Then
                grdEnrollment.Columns.Item(7).Visible = True
            Else
                grdEnrollment.Columns.Item(7).Visible = False
            End If

        End Sub

        Private Sub AddRegUser(ByVal inUserID As Integer, ByVal objEvent As EventInfo)
            ' Check if signup already exists since due to partial rendering it may be possible
            ' to click the enroll user link twice
            Dim intUserID As Integer = inUserID
            _objEventSignups = _objCtlEventSignups.EventsSignupsGetUser(objEvent.EventID, intUserID, objEvent.ModuleID)

            If _objEventSignups Is Nothing Then
                ' Get user info
                Dim objUserInfo As UserInfo = UserController.GetUserById(PortalId, intUserID)

                _objEventSignups = New EventSignupsInfo
                _objEventSignups.EventID = objEvent.EventID
                _objEventSignups.ModuleID = objEvent.ModuleID
                _objEventSignups.UserID = intUserID
                _objEventSignups.AnonEmail = Nothing
                _objEventSignups.AnonName = Nothing
                _objEventSignups.AnonTelephone = Nothing
                _objEventSignups.AnonCulture = Nothing
                _objEventSignups.AnonTimeZoneId = Nothing
                _objEventSignups.PayPalPaymentDate = DateTime.UtcNow
                _objEventSignups.Approved = True
                _objEventSignups.NoEnrolees = CInt(txtNoEnrolees.Text)
                _objEventSignups = CreateEnrollment(_objEventSignups, objEvent)

                ' Mail users
                If Settings.SendEnrollMessageAdded Then
                    Dim objEventEmailInfo As New EventEmailInfo
                    Dim objEventEmail As New EventEmails(PortalId, ModuleId, LocalResourceFile, CType(Page, PageBase).PageCulture.Name)
                    objEventEmailInfo.TxtEmailSubject = Settings.Templates.txtEnrollMessageSubject
                    objEventEmailInfo.TxtEmailBody = Settings.Templates.txtEnrollMessageAdded
                    objEventEmailInfo.TxtEmailFrom() = Settings.StandardEmail
                    objEventEmailInfo.UserIDs.Add(_objEventSignups.UserID)
                    objEventEmailInfo.UserIDs.Add(objEvent.OwnerID)
                    objEventEmail.SendEmails(objEventEmailInfo, objEvent, _objEventSignups)
                End If
            End If
        End Sub

        Private Function ValidateTime(ByVal indate As DateTime) As Boolean
            Dim inMinutes As Integer = indate.Minute
            Dim remainder As Integer = inMinutes Mod CInt(Settings.Timeinterval)
            If remainder > 0 Then
                Return False
            End If
            Return True
        End Function

#End Region

#Region "Links and Buttons"
        Private Sub cancelButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cancelButton.Click
            Try
                Response.Redirect(GetStoredPrevPage(), True)

            Catch exc As Exception 'Module failed to load
                'ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub updateButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles updateButton.Click
            Try
                UpdateProcessing(_itemID)
                If Page.IsValid Then
                    Response.Redirect(GetStoredPrevPage(), True)
                End If

            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub deleteButton_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles deleteButton.Click
            Try
                _objEvent = _objCtlEvent.EventsGet(_itemID, ModuleId)
                If _editRecur Then
                    _objCtlEventRecurMaster.EventsRecurMasterDelete(_objEvent.RecurMasterID, _objEvent.ModuleID)
                Else
                    If _objEvent.RRULE <> "" Then
                        _objEvent.Cancelled = True
                        _objEvent.LastUpdatedID = UserId
                        _objEvent = _objCtlEvent.EventsSave(_objEvent, False, TabId, True)
                    Else
                        _objCtlEventRecurMaster.EventsRecurMasterDelete(_objEvent.RecurMasterID, _objEvent.ModuleID)
                    End If
                End If
                Response.Redirect(GetSocialNavigateUrl(), True)
            Catch exc As Exception 'Module failed to load
                'ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub lnkSelectedEmail_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles lnkSelectedEmail.Click
            Email(True)
        End Sub

        Private Sub lnkSelectedDelete_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles lnkSelectedDelete.Click
            Dim item As DataGridItem
            Dim objEnroll As EventSignupsInfo
            Dim eventID As Integer = 0

            For Each item In grdEnrollment.Items
                If CType(item.FindControl("chkSelect"), CheckBox).Checked Then
                    Dim intSignupID As Integer = CInt(grdEnrollment.DataKeys(item.ItemIndex))
                    objEnroll = _objCtlEventSignups.EventsSignupsGet(intSignupID, ModuleId, False)
                    If Not objEnroll Is Nothing Then
                        If eventID <> objEnroll.EventID Then
                            _objEvent = _objCtlEvent.EventsGet(objEnroll.EventID, ModuleId)
                        End If
                        eventID = objEnroll.EventID()

                        ' Delete Selected Enrollee?
                        DeleteEnrollment(intSignupID, _objEvent.ModuleID, _objEvent.EventID)

                        ' Mail users
                        If Settings.SendEnrollMessageDeleted Then
                            Dim objEventEmailInfo As New EventEmailInfo
                            Dim objEventEmail As New EventEmails(PortalId, ModuleId, LocalResourceFile, CType(Page, PageBase).PageCulture.Name)
                            objEventEmailInfo.TxtEmailSubject = Settings.Templates.txtEnrollMessageSubject
                            objEventEmailInfo.TxtEmailBody = Settings.Templates.txtEnrollMessageDeleted
                            objEventEmailInfo.TxtEmailFrom() = Settings.StandardEmail
                            If objEnroll.UserID > -1 Then
                                objEventEmailInfo.UserIDs.Add(objEnroll.UserID)
                            Else
                                objEventEmailInfo.UserEmails.Add(objEnroll.AnonEmail)
                                objEventEmailInfo.UserLocales.Add(objEnroll.AnonCulture)
                                objEventEmailInfo.UserTimeZoneIds.Add(objEnroll.AnonTimeZoneId)
                            End If
                            objEventEmailInfo.UserIDs.Add(_objEvent.OwnerID)
                            objEventEmail.SendEmails(objEventEmailInfo, _objEvent, objEnroll)
                        End If
                    End If

                End If
            Next

            LoadRegUsers()
            BuildEnrolleeGrid(_objEvent)
        End Sub

        Private Sub copyButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles copyButton.Click
            Try
                UpdateProcessing(-1)
                If Page.IsValid Then
                    Response.Redirect(GetStoredPrevPage(), True)
                End If

            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub ddEnrollRoles_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddEnrollRoles.SelectedIndexChanged
            LoadRegUsers()
        End Sub

        Private Sub chkSignups_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chkSignups.CheckedChanged
            tblEventEmail.Attributes.Add("style", "display:none; width:100%")
            If chkSignups.Checked Then
                tblEnrollmentDetails.Attributes.Add("style", "display:block;")
                LoadRegUsers()
                If txtEnrolled.Text <> 0.ToString() Then
                    tblEventEmail.Attributes.Add("style", "display:block; width:100%")
                End If
            Else
                tblEnrollmentDetails.Attributes.Add("style", "display:none;")
                If Settings.Newpereventemail And chkEventEmailChk.Checked Then
                    tblEventEmail.Attributes.Add("style", "display:block; width:100%")
                End If
            End If
        End Sub

        Private Sub grdAddUser_AddSelectedUsers(ByVal sender As Object, ByVal e As EventArgs, ByVal arrUsers As ArrayList) Handles grdAddUser.AddSelectedUsers
            Try
                If CInt(txtNoEnrolees.Text) > CInt(valNoEnrolees.MaximumValue) Or _
                   CInt(txtNoEnrolees.Text) < CInt(valNoEnrolees.MinimumValue) Then
                    valNoEnrolees.IsValid = False
                    valNoEnrolees.Visible = True
                    valNoEnrolees.ErrorMessage = String.Format(Localization.GetString("valNoEnrolees", LocalResourceFile), valNoEnrolees.MaximumValue)
                    Return
                End If
            Catch
                valNoEnrolees.IsValid = False
                valNoEnrolees.Visible = True
                valNoEnrolees.ErrorMessage = String.Format(Localization.GetString("valNoEnrolees", LocalResourceFile), valNoEnrolees.MaximumValue)
                Return
            End Try

            Dim objEvent As EventInfo = _objCtlEvent.EventsGet(_itemID, ModuleId)

            For Each inUserid As Integer In arrUsers
                AddRegUser(inUserid, objEvent)
            Next
            LoadRegUsers()
            BuildEnrolleeGrid(objEvent)
            txtNoEnrolees.Text = 1.ToString()
        End Sub

#End Region

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region

    End Class

#Region "Comparer Class"
    Public Class UserListSort
        Implements IComparer

        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
            Dim xdisplayname As String
            Dim ydisplayname As String

            xdisplayname = CType(x, EventUser).DisplayName
            ydisplayname = CType(y, EventUser).DisplayName
            Dim c As New CaseInsensitiveComparer
            Return c.Compare(xdisplayname, ydisplayname)
        End Function
    End Class

#End Region

End Namespace
