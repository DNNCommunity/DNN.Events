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

Imports System
Imports DotNetNuke.Security
Imports System.Xml
Imports System.Xml.Linq
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml.XPath
Imports System.Xml.Xsl
Imports System.IO
Imports System.Windows.Forms
Imports DotNetNuke.Services.Tokens
Imports DotNetNuke.Entities.Profile

Namespace DotNetNuke.Modules.Events

    <DNNtc.ModuleControlProperties("Details", "Events Details", DNNtc.ControlType.View, "https://dnnevents.codeplex.com/documentation", True, False)> _
    Partial Class EventDetails
        Inherits EventBase

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

#Region "Properties"
        ''' <summary>
        ''' Stores the ItemId in the viewstate
        ''' </summary>
        Private Property ItemId() As Int32
            Get
                Return CInt(ViewState("EventItemID" + ModuleId.ToString))
            End Get
            Set(ByVal value As Int32)
                ViewState("EventItemID" + ModuleId.ToString) = value.ToString
            End Set
        End Property
        Private _eventInfo As New EventInfo

        Private Enum MessageLevel As Integer
            DNNSuccess = 1
            DNNInformation
            DNNWarning
            DNNError
        End Enum

#End Region

#Region "Event Handlers"
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
            ' Log exception whem status is filled
            If Not Request.Params("status") = Nothing Then
                Dim objSecurity As New PortalSecurity
                Dim status As String = objSecurity.InputFilter(Request.Params("status"), PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup)
                LogException(New ModuleLoadException("EventDetails Call...status: " & status))
            End If

            ' Add the external Validation.js to the Page
            Const csname As String = "ExtValidationScriptFile"
            Dim cstype As Type = Reflection.MethodBase.GetCurrentMethod().GetType()
            Dim cstext As String = "<script src=""" & ResolveUrl("~/DesktopModules/Events/Scripts/Validation.js") & """ type=""text/javascript""></script>"
            If Not Page.ClientScript.IsClientScriptBlockRegistered(csname) Then
                Page.ClientScript.RegisterClientScriptBlock(cstype, csname, cstext, False)
            End If

            ' Force full PostBack since these pass off to aspx page
            If AJAX.IsInstalled Then
                AJAX.RegisterPostBackControl(cmdvEvent)
                AJAX.RegisterPostBackControl(cmdvEventSeries)
                AJAX.RegisterPostBackControl(cmdvEventSignups)
            End If

            cmdvEvent.ToolTip = Localization.GetString("cmdvEventTooltip", LocalResourceFile)
            cmdvEvent.Text = Localization.GetString("cmdvEventExport", LocalResourceFile)
            cmdvEventSeries.ToolTip = Localization.GetString("cmdvEventSeriesTooltip", LocalResourceFile)
            cmdvEventSeries.Text = Localization.GetString("cmdvEventExportSeries", LocalResourceFile)
            cmdvEventSignups.ToolTip = Localization.GetString("cmdvEventSignupsTooltip", LocalResourceFile)
            cmdvEventSignups.Text = Localization.GetString("cmdvEventSignupsDownload", LocalResourceFile)

            cmdPrint.ToolTip = Localization.GetString("Print", LocalResourceFile)

            Try
                'Get the item id of the selected event
                If Not (Request.Params("ItemId") Is Nothing) Then
                    ItemId = Int32.Parse(Request.Params("ItemId"))
                Else
                    Response.Redirect(GetSocialNavigateUrl(), True)
                End If

                ' Set the selected theme 
                If Settings.Eventdetailnewpage Then
                    SetTheme(pnlEventsModuleDetails)
                    AddFacebookMetaTags()
                End If

                ' If the page is being requested the first time, determine if an
                ' contact itemId value is specified, and if so populate page
                ' contents with the contact details
                If Page.IsPostBack Then
                    Return
                End If


                Dim objCtlEvent As New EventController
                _eventInfo = objCtlEvent.EventsGet(ItemId, ModuleId)

                'If somebody has sent a bad ItemID and eventinfo not retrieved, return 301
                If IsNothing(_eventInfo) Then
                    Response.StatusCode = 301
                    Response.AppendHeader("Location", GetSocialNavigateUrl())
                    Return
                End If

                ' Do they have permissions to the event?
                Dim objCtlEventInfoHelper As New EventInfoHelper(ModuleId, Settings)
                If Settings.Enforcesubcalperms And Not objCtlEventInfoHelper.IsModuleViewer(_eventInfo.ModuleID) Then
                    Response.Redirect(GetSocialNavigateUrl(), True)
                ElseIf IsPrivateNotModerator And UserId <> _eventInfo.OwnerID Then
                    Response.Redirect(GetSocialNavigateUrl(), True)
                ElseIf Settings.SocialGroupModule = EventModuleSettings.SocialModule.UserProfile And Not objCtlEventInfoHelper.IsSocialUserPublic(GetUrlUserId()) Then
                    Response.Redirect(GetSocialNavigateUrl(), True)
                ElseIf Settings.SocialGroupModule = EventModuleSettings.SocialModule.SocialGroup And Not objCtlEventInfoHelper.IsSocialGroupPublic(GetUrlGroupId()) Then
                    Response.Redirect(GetSocialNavigateUrl(), True)
                End If

                ' Has the event been cancelled?
                If _eventInfo.Cancelled Then
                    Response.StatusCode = 301
                    Response.AppendHeader("Location", GetSocialNavigateUrl())
                    Return
                End If

                ' So we have a valid item, but is it from a module that has been deleted
                ' but not removed from the recycle bin
                If _eventInfo.ModuleID <> ModuleId Then
                    Dim objCtlModule As New Entities.Modules.ModuleController
                    Dim objModules As ArrayList = CType(objCtlModule.GetTabModulesByModule(_eventInfo.ModuleID), ArrayList)
                    Dim objModule As Entities.Modules.ModuleInfo
                    Dim isDeleted As Boolean = True
                    For Each objModule In objModules
                        If Not objModule.IsDeleted Then
                            isDeleted = False
                        End If
                    Next
                    If isDeleted Then
                        Response.StatusCode = 301
                        Response.AppendHeader("Location", GetSocialNavigateUrl())
                        Return
                    End If
                End If

                ' Should all be OK to display now
                Dim displayTimeZoneId As String = _eventInfo.EventTimeZoneId
                If Not Settings.EnableEventTimeZones Then
                    displayTimeZoneId = GetDisplayTimeZoneId()
                End If
                Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
                _eventInfo = objEventInfoHelper.ConvertEventToDisplayTimeZone(_eventInfo, displayTimeZoneId)

                Dim tcc As New TokenReplaceControllerClass(ModuleId, LocalResourceFile)

                'Set the page title
                If Settings.EnableSEO Then
                    Dim txtPageText As String = String.Format(Settings.Templates.txtSEOPageTitle, BasePage.Title)
                    txtPageText = tcc.TokenReplaceEvent(_eventInfo, txtPageText, False)
                    txtPageText = HttpUtility.HtmlDecode(txtPageText)
                    txtPageText = HtmlUtils.StripTags(txtPageText, True)
                    txtPageText = txtPageText.Replace(Environment.NewLine, " ")
                    txtPageText = HtmlUtils.StripWhiteSpace(txtPageText, True)
                    BasePage.Title = txtPageText
                    txtPageText = String.Format(Settings.Templates.txtSEOPageDescription, BasePage.Description)
                    txtPageText = tcc.TokenReplaceEvent(_eventInfo, txtPageText, False)
                    txtPageText = HttpUtility.HtmlDecode(txtPageText)
                    txtPageText = HtmlUtils.StripTags(txtPageText, True)
                    txtPageText = txtPageText.Replace(Environment.NewLine, " ")
                    txtPageText = HtmlUtils.StripWhiteSpace(txtPageText, True)
                    txtPageText = HtmlUtils.Shorten(txtPageText, Settings.SEODescriptionLength, "...")
                    BasePage.Description = txtPageText
                    txtPageText = BasePage.KeyWords
                    If Not _eventInfo.LocationName Is Nothing Then
                        If txtPageText <> "" Then
                            txtPageText = txtPageText + ","
                        End If
                        txtPageText = txtPageText + _eventInfo.LocationName
                    End If
                    If Not _eventInfo.CategoryName Is Nothing Then
                        If txtPageText <> "" Then
                            txtPageText = txtPageText + ","
                        End If
                        txtPageText = txtPageText + _eventInfo.CategoryName
                    End If
                    BasePage.KeyWords = txtPageText
                End If

                'Replace tokens
                Dim txtTemplate, txtTemplate1, txtTemplate2, txtTemplate3, txtTemplate4 As String
                txtTemplate = Settings.Templates.EventDetailsTemplate
                txtTemplate1 = txtTemplate
                txtTemplate2 = ""
                txtTemplate3 = ""
                txtTemplate4 = ""
                Dim nTemplate As Integer = 0
                Do While InStr(txtTemplate, "[BREAK]") > 0
                    nTemplate = nTemplate + 1
                    Dim txtBefore As String
                    Dim nBreak As Integer = InStr(txtTemplate, "[BREAK]")
                    If nBreak > 1 Then
                        txtBefore = Left(txtTemplate, nBreak - 1)
                    Else
                        txtBefore = ""
                    End If
                    If Len(txtTemplate) > nBreak + 6 Then
                        txtTemplate = Mid(txtTemplate, nBreak + 7)
                    Else
                        txtTemplate = ""
                    End If
                    Select Case nTemplate
                        Case 1
                            txtTemplate1 = txtBefore
                            txtTemplate2 = txtTemplate
                        Case 2
                            txtTemplate2 = txtBefore
                            txtTemplate3 = txtTemplate
                        Case 3
                            txtTemplate3 = txtBefore
                            txtTemplate4 = txtTemplate
                        Case 4
                            txtTemplate4 = txtBefore
                    End Select
                Loop
                If txtTemplate1 <> "" And txtTemplate1 <> vbCrLf Then
                    divEventDetails1.InnerHtml = tcc.TokenReplaceEvent(_eventInfo, txtTemplate1)
                    divEventDetails1.Attributes.Add("style", "display:block;")
                Else
                    divEventDetails1.Attributes.Add("style", "display:none;")
                End If
                If txtTemplate2 <> "" And txtTemplate2 <> vbCrLf Then
                    divEventDetails2.InnerHtml = tcc.TokenReplaceEvent(_eventInfo, txtTemplate2)
                    divEventDetails2.Attributes.Add("style", "display:block;")
                Else
                    divEventDetails2.Attributes.Add("style", "display:none;")
                End If
                If txtTemplate3 <> "" And txtTemplate3 <> vbCrLf Then
                    divEventDetails3.InnerHtml = tcc.TokenReplaceEvent(_eventInfo, txtTemplate3)
                    divEventDetails3.Attributes.Add("style", "display:block;")
                Else
                    divEventDetails3.Attributes.Add("style", "display:none;")
                End If
                If txtTemplate4 <> "" And txtTemplate4 <> vbCrLf Then
                    divEventDetails4.InnerHtml = tcc.TokenReplaceEvent(_eventInfo, txtTemplate4)
                    divEventDetails4.Attributes.Add("style", "display:block;")
                Else
                    divEventDetails4.Attributes.Add("style", "display:none;")
                End If

                editButton.Visible = False
                deleteButton.Visible = False
                cmdvEventSignups.Visible = False
                If IsEventEditor(_eventInfo, False) Then
                    editButton.Visible = True
                    editButton.NavigateUrl = objEventInfoHelper.GetEditURL(_eventInfo.EventID, _eventInfo.SocialGroupId, _eventInfo.SocialUserId)
                    editButton.ToolTip = Localization.GetString("editButton", LocalResourceFile)
                    deleteButton.Visible = True
                    deleteButton.Attributes.Add("onclick", "javascript:return confirm('" + Localization.GetString("ConfirmEventDelete", LocalResourceFile) + "');")
                    cmdvEventSignups.Visible = True
                End If
                editSeriesButton.Visible = False
                deleteSeriesButton.Visible = False
                If _eventInfo.RRULE <> "" Then
                    ' Note that IsEventEditor with 'True' is used here because this is for the 
                    ' series(buttons)and excludes single event owner. Must be recurrence master owner.
                    If IsEventEditor(_eventInfo, True) Then
                        editSeriesButton.Visible = True
                        editSeriesButton.NavigateUrl = objEventInfoHelper.GetEditURL(_eventInfo.EventID, _eventInfo.SocialGroupId, _eventInfo.SocialUserId, "All")
                        editSeriesButton.ToolTip = Localization.GetString("editSeriesButton", LocalResourceFile)
                        deleteSeriesButton.Visible = True
                        deleteSeriesButton.Attributes.Add("onclick", "javascript:return confirm('" + Localization.GetString("ConfirmEventSeriesDelete", LocalResourceFile) + "');")
                    End If
                End If
                If _eventInfo.RRULE = "" Then
                    cmdvEventSeries.Visible = False
                End If

                Dim objEventTimeZoneUtilities As New EventTimeZoneUtilities
                Dim nowDisplay As DateTime = objEventTimeZoneUtilities.ConvertFromUTCToDisplayTimeZone(Date.UtcNow, GetDisplayTimeZoneId()).EventDate

                '  Compute Dates/Times (for recurring)
                Dim startdate As DateTime = _eventInfo.EventTimeBegin
                SelectedDate = startdate.Date

                ' See if user already are signed up 
                ' And that Signup is Authorized
                ' And also that the Date/Time has not passed
                divEnrollment.Attributes.Add("style", "display:none;")
                If _eventInfo.Signups Then
                    If startdate > nowDisplay Then
                        If _eventInfo.EnrollRoleID = Nothing Or _eventInfo.EnrollRoleID = -1 Then
                            UserEnrollment(_eventInfo)
                        Else
                            Dim objEventSignupsController As New EventSignupsController
                            If objEventSignupsController.IsEnrollRole(_eventInfo.EnrollRoleID, PortalId) Then
                                UserEnrollment(_eventInfo)
                            End If
                        End If
                    Else
                        divEnrollment.Attributes.Add("style", "display:block;")
                        lblEnrollTooLate.Text = Localization.GetString("EnrollTooLate", LocalResourceFile)
                        enroll4.Visible = True
                    End If
                End If

                'Are You Sure You Want To Enroll?'
                If Request.IsAuthenticated Then
                    If Settings.Enableenrollpopup Then
                        cmdSignup.Attributes.Add("onclick", "javascript:return confirm('" + Localization.GetString("SureYouWantToEnroll", LocalResourceFile) + "');")
                    End If
                    valNoEnrolees.MaximumValue = CStr(_eventInfo.MaxEnrollment - _eventInfo.Enrolled)
                    If CInt(valNoEnrolees.MaximumValue) > Settings.Maxnoenrolees Or _eventInfo.MaxEnrollment = 0 Then
                        valNoEnrolees.MaximumValue = Settings.Maxnoenrolees.ToString
                    End If
                    lblMaxNoEnrolees.Text = String.Format(Localization.GetString("lblMaxNoEnrolees", LocalResourceFile), valNoEnrolees.MaximumValue)
                    valNoEnrolees.ErrorMessage = String.Format(Localization.GetString("valNoEnrolees", LocalResourceFile), CInt(valNoEnrolees.MaximumValue))
                    valNoEnrolees2.ErrorMessage = valNoEnrolees.ErrorMessage
                End If

                divMessage.Attributes.Add("style", "display:none;")

                If Settings.IcalEmailEnable Then
                    divIcalendar.Attributes.Add("style", "display:block;")
                    txtUserEmailiCal.Text = UserInfo.Email
                    If Request.IsAuthenticated Then
                        txtUserEmailiCal.Enabled = False
                    Else
                        txtUserEmailiCal.Enabled = True
                    End If
                Else
                    divIcalendar.Attributes.Add("style", "display:none;")
                End If

                'Is notification enabled
                divReminder.Attributes.Add("style", "display:none;")
                If _eventInfo.SendReminder And startdate > nowDisplay Then
                    'Is registered user
                    If Request.IsAuthenticated() Then
                        divReminder.Attributes.Add("style", "display:block;")
                        Dim objEventNotificationController As EventNotificationController = New EventNotificationController
                        Dim notificationInfo As String = objEventNotificationController.NotifyInfo(_eventInfo.EventID, UserInfo.Email, ModuleId, LocalResourceFile, GetDisplayTimeZoneId())
                        If notificationInfo <> "" Then
                            lblConfirmation.Text = notificationInfo
                            rem3.Visible = True
                            imgConfirmation.AlternateText = Localization.GetString("Reminder", LocalResourceFile)
                        Else
                            txtUserEmail.Text = UserInfo.Email
                        End If
                    End If
                    ' is anonymous notification allowed or registered user not yet notified
                    If (Settings.Notifyanon And Not Request.IsAuthenticated()) Or txtUserEmail.Text.Length > 0 Then
                        If Request.IsAuthenticated Then
                            txtUserEmail.Enabled = False
                        Else
                            txtUserEmail.Enabled = True
                        End If
                        divReminder.Attributes.Add("style", "display:block;")
                        Dim errorminutes As String = Localization.GetString("invalidReminderMinutes", LocalResourceFile)
                        Dim errorhours As String = Localization.GetString("invalidReminderHours", LocalResourceFile)
                        Dim errordays As String = Localization.GetString("invalidReminderDays", LocalResourceFile)

                        txtReminderTime.Text = _eventInfo.ReminderTime.ToString  'load default Reminder Time of event
                        txtReminderTime.Visible = True
                        ddlReminderTimeMeasurement.Attributes.Add("onchange", "valRemTime('" & valReminderTime.ClientID & "','" & valReminderTime2.ClientID & "','" & valReminderTime.ValidationGroup & "','" & ddlReminderTimeMeasurement.ClientID & "','" & errorminutes & "','" & errorhours & "','" & errordays & "');")
                        ddlReminderTimeMeasurement.Visible = True
                        ddlReminderTimeMeasurement.SelectedValue = _eventInfo.ReminderTimeMeasurement

                        Select Case _eventInfo.ReminderTimeMeasurement
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

                        rem1.Visible = True
                        imgNotify.AlternateText = Localization.GetString("Reminder", LocalResourceFile)
                        rem2.Visible = True
                        If _eventInfo.RRULE <> "" Then
                            chkReminderRec.Visible = True
                        End If
                    End If
                End If
                With grdEnrollment
                    .Columns(0).HeaderText = Localization.GetString("EnrollUserName", LocalResourceFile)
                    .Columns(1).HeaderText = Localization.GetString("EnrollDisplayName", LocalResourceFile)
                    .Columns(2).HeaderText = Localization.GetString("EnrollEmail", LocalResourceFile)
                    .Columns(3).HeaderText = Localization.GetString("EnrollPhone", LocalResourceFile)
                    .Columns(4).HeaderText = Localization.GetString("EnrollApproved", LocalResourceFile)
                    .Columns(5).HeaderText = Localization.GetString("EnrollNo", LocalResourceFile)
                End With

                BindEnrollList(_eventInfo)
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub
#End Region

#Region "Helper Methods"
        ''' <summary>
        ''' Bind enrolled users to the enrollment grid
        ''' </summary>
        Private Sub BindEnrollList(ByVal eventInfo As EventInfo)
            divEnrollList.Attributes.Add("style", "display:none;")
            Dim blEnrollList As Boolean = False
            Dim txtColumns As String = ""
            If eventInfo.Signups And eventInfo.Enrolled > 0 Then
                txtColumns = EnrolmentColumns(eventInfo, eventInfo.EnrollListView)
            End If
            If txtColumns <> "" Then
                blEnrollList = True
            End If

            If blEnrollList Then
                If txtColumns.LastIndexOf("UserName", StringComparison.Ordinal) < 0 Then
                    grdEnrollment.Columns.Item(0).Visible = False
                End If
                If txtColumns.LastIndexOf("DisplayName", StringComparison.Ordinal) < 0 Then
                    grdEnrollment.Columns.Item(1).Visible = False
                End If
                If txtColumns.LastIndexOf("Email", StringComparison.Ordinal) < 0 Then
                    grdEnrollment.Columns.Item(2).Visible = False
                End If
                If txtColumns.LastIndexOf("Phone", StringComparison.Ordinal) < 0 Then
                    grdEnrollment.Columns.Item(3).Visible = False
                End If
                If txtColumns.LastIndexOf("Approved", StringComparison.Ordinal) < 0 Then
                    grdEnrollment.Columns.Item(4).Visible = False
                End If
                If txtColumns.LastIndexOf("Qty", StringComparison.Ordinal) < 0 Then
                    grdEnrollment.Columns.Item(5).Visible = False
                End If

                'Load enrol list
                Dim eventEnrollment As New ArrayList
                Dim objSignups As ArrayList
                Dim objSignup As EventSignupsInfo
                Dim objCtlUser As New UserController
                Dim objCtlEventSignups As New EventSignupsController
                objSignups = objCtlEventSignups.EventsSignupsGetEvent(eventInfo.EventID, ModuleId)
                For Each objSignup In objSignups
                    Dim objEnrollListItem As New EventEnrollList
                    If objSignup.UserID <> -1 Then
                        Dim objUser As UserInfo
                        objUser = objCtlUser.GetUser(PortalId, objSignup.UserID)
                        Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
                        objEnrollListItem.EnrollDisplayName = objEventInfoHelper.UserDisplayNameProfile(objSignup.UserID, objSignup.UserName, LocalResourceFile).DisplayNameURL
                        If Not objUser Is Nothing Then
                            objEnrollListItem.EnrollUserName = objUser.Username
                            objEnrollListItem.EnrollEmail = String.Format("<a href=""mailto:{0}?subject={1}"">{0}</a>", objSignup.Email, eventInfo.EventName)
                            objEnrollListItem.EnrollPhone = objUser.Profile.Telephone
                        End If
                    Else
                        objEnrollListItem.EnrollDisplayName = objSignup.AnonName
                        objEnrollListItem.EnrollUserName = Localization.GetString("AnonUser", LocalResourceFile)
                        objEnrollListItem.EnrollEmail = String.Format("<a href=""mailto:{0}?subject={1}"">{0}</a>", objSignup.AnonEmail, eventInfo.EventName)
                        objEnrollListItem.EnrollPhone = objSignup.AnonTelephone
                    End If
                    objEnrollListItem.SignupID = objSignup.SignupID
                    objEnrollListItem.EnrollApproved = objSignup.Approved
                    objEnrollListItem.EnrollNo = objSignup.NoEnrolees
                    eventEnrollment.Add(objEnrollListItem)
                Next
                If eventEnrollment.Count > 0 Then
                    divEnrollList.Attributes.Add("style", "display:block;")
                    grdEnrollment.DataSource = eventEnrollment
                    grdEnrollment.DataBind()
                End If
            End If
        End Sub

        ''' <summary>
        ''' Display User Enrollment Info on Page
        ''' </summary>
        Private Function UserEnrollment(ByVal eventInfo As EventInfo) As MessageLevel
            UserEnrollment = MessageLevel.DNNSuccess
            If Not Settings.Eventsignup Then
                divEnrollment.Attributes.Add("style", "display:none;")
                Exit Function
            End If

            divEnrollment.Attributes.Add("style", "display:block;")
            enroll3.Visible = False
            enroll5.Visible = False
            If Not Request.Params("Status") Is Nothing Then
                If Request.Params("Status").ToLower = "enrolled" Then
                    ' User has been successfully enrolled for this event (paid enrollment)
                    lblSignup.Text = Localization.GetString("StatusPPSuccess", LocalResourceFile)
                    enroll2.Visible = True
                    imgSignup.AlternateText = Localization.GetString("StatusPPSuccess", LocalResourceFile)
                ElseIf Request.Params("Status").ToLower = "cancelled" Then
                    ' User has been cancelled paid enrollment
                    lblSignup.Text = Localization.GetString("StatusPPCancelled", LocalResourceFile)
                    UserEnrollment = MessageLevel.DNNInformation
                    enroll2.Visible = True
                    imgSignup.AlternateText = Localization.GetString("StatusPPCancelled", LocalResourceFile)
                End If
                Exit Function
            End If

            ' If not authenticated and anonymous not allowed setup for logintoenroll
            If Not Request.IsAuthenticated And Not eventInfo.AllowAnonEnroll Then
                enroll1.Visible = True
                imgEnroll.AlternateText = Localization.GetString("LoginToEnroll", LocalResourceFile)
                cmdSignup.Text = Localization.GetString("LoginToEnroll", LocalResourceFile)
                Exit Function
            End If

            ' If not authenticated make email/name boxes visible, or find out if authenticated user has already enrolled
            Dim objCtlEventSignups As New EventSignupsController
            Dim objEventSignups As EventSignupsInfo = Nothing
            If Not Request.IsAuthenticated Then
                If Not String.IsNullOrEmpty(txtAnonEmail.Text) Then
                    objEventSignups = objCtlEventSignups.EventsSignupsGetAnonUser(eventInfo.EventID, txtAnonEmail.Text, ModuleId)
                End If
            Else
                objEventSignups = objCtlEventSignups.EventsSignupsGetUser(eventInfo.EventID, UserId, ModuleId)
            End If

            If objEventSignups Is Nothing Then
                If Not Request.IsAuthenticated And Not Settings.EnrollmentPageAllowed Then
                    enroll5.Visible = True
                End If
                If (eventInfo.Enrolled < eventInfo.MaxEnrollment) Or _
                (eventInfo.MaxEnrollment = 0) Then
                    If Settings.Maxnoenrolees > 1 And Not Settings.EnrollmentPageAllowed Then
                        enroll3.Visible = True
                    End If
                    ' User is not enrolled for this event...press the link to enroll!
                    enroll1.Visible = True
                    imgEnroll.AlternateText = Localization.GetString("EnrollForEvent", LocalResourceFile)
                    cmdSignup.Text = Localization.GetString("EnrollForEvent", LocalResourceFile)
                End If
            Else
                enroll2.Visible = True
                If objEventSignups.Approved Then
                    ' User is enrolled and approved for this event!
                    imgSignup.AlternateText = Localization.GetString("YouAreEnrolledForThisEvent", LocalResourceFile)
                    lblSignup.Text = Localization.GetString("YouAreEnrolledForThisEvent", LocalResourceFile)
                    UserEnrollment = MessageLevel.DNNSuccess
                Else
                    ' User is enrolled for this event, but not yet approved!
                    imgSignup.AlternateText = Localization.GetString("EnrolledButNotApproved", LocalResourceFile)
                    lblSignup.Text = Localization.GetString("EnrolledButNotApproved", LocalResourceFile)
                    UserEnrollment = MessageLevel.DNNWarning
                End If
            End If

        End Function

        ''' <summary>
        ''' Redirect to EventVCal page which exports the event to a vCard
        ''' </summary>
        Private Sub ExportEvent(ByVal series As Boolean)
            Try
                Response.Redirect("~/DesktopModules/Events/EventVCal.aspx?ItemID=" & ItemId & "&Mid=" & ModuleId & "&tabid=" & TabId & "&Series=" & CType(series, String))
            Catch ex As Threading.ThreadAbortException
                'Ignore
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' <summary>
        ''' Download the enrollees for this event as a file.
        ''' </summary>
        Private Sub DownloadSignups()
            'Get the event.
            Dim theEvent As EventInfo = New EventController().EventsGet(ItemId, ModuleId)
            If theEvent IsNot Nothing Then
                'Dim xmlDoc As XmlDocument = DefineXmlFile(theEvent, True)
                'If xmlDoc IsNot Nothing Then
                '    GenerateXmlFile(theEvent, xmlDoc)
                'End If

                Dim csvDoc As String = DefineCsvFile(theEvent)
                If csvDoc IsNot Nothing Then
                    GenerateCsvFile(theEvent, csvDoc)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Define the xml file for downloading the enrollees for this event.
        ''' </summary>
        Private Function DefineXmlFile(ByVal theEvent As EventInfo, ByVal localizeTags As Boolean) As XmlDocument
            DefineXmlFile = Nothing
            Dim xmlDoc As XmlDocument = New XmlDocument()

            'Get the enrollees.
            Dim eventSignups As ArrayList = New EventSignupsController().EventsSignupsGetEvent(ItemId, ModuleId)
            'Anything to do?
            If eventSignups Is Nothing Or eventSignups.Count = 0 Then
                Exit Function
            End If

            Try
                'Initialization.
                Dim xNamespace As XNamespace = xNamespace.Get("")
                Dim xRoot As XElement = _
                    New XElement(xNamespace + "Document", New XAttribute(xNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"))

                'Tags should be localized for XML output. These will be converted to a header row in Excel.
                'Tags should not be localized for CSV output. A separate localized header row is added.
                Dim xElemHdr As XElement
                If localizeTags Then
                    xElemHdr = New XElement("Enrollee", _
                                New XElement("EventName", Localization.GetString("Event Name.Header", LocalResourceFile)), _
                                New XElement("EventStart", Localization.GetString("Event Start.Header", LocalResourceFile)), _
                                New XElement("EventEnd", Localization.GetString("Event End.Header", LocalResourceFile)), _
                                New XElement("Location", Localization.GetString("Location.Header", LocalResourceFile)), _
                                New XElement("Category", Localization.GetString("Category.Header", LocalResourceFile)), _
                                New XElement("ReferenceNumber", Localization.GetString("ReferenceNumber.Header", LocalResourceFile)), _
                                New XElement("Company", Localization.GetString("Company.Header", LocalResourceFile)), _
                                New XElement("JobTitle", Localization.GetString("JobTitle.Header", LocalResourceFile)), _
                                New XElement("FullName", Localization.GetString("FullName.Header", LocalResourceFile)), _
                                New XElement("FirstName", Localization.GetString("FirstName.Header", LocalResourceFile)), _
                                New XElement("LastName", Localization.GetString("LastName.Header", LocalResourceFile)), _
                                New XElement("Email", Localization.GetString("Email.Header", LocalResourceFile)), _
                                New XElement("Phone", Localization.GetString("Phone.Header", LocalResourceFile)), _
                                New XElement("Street", Localization.GetString("Street.Header", LocalResourceFile)), _
                                New XElement("PostalCode", Localization.GetString("PostalCode.Header", LocalResourceFile)), _
                                New XElement("City", Localization.GetString("City.Header", LocalResourceFile)), _
                                New XElement("Region", Localization.GetString("Region.Header", LocalResourceFile)), _
                                New XElement("Country", Localization.GetString("Country.Header", LocalResourceFile)) _
                                )
                Else
                    xElemHdr = New XElement("Enrollee", _
                                New XElement("EventName", String.Empty), _
                                New XElement("EventStart", String.Empty), _
                                New XElement("EventEnd", String.Empty), _
                                New XElement("Location", String.Empty), _
                                New XElement("Category", String.Empty), _
                                New XElement("ReferenceNumber", String.Empty), _
                                New XElement("Company", String.Empty), _
                                New XElement("JobTitle", String.Empty), _
                                New XElement("FullName", String.Empty), _
                                New XElement("FirstName", String.Empty), _
                                New XElement("LastName", String.Empty), _
                                New XElement("Email", String.Empty), _
                                New XElement("Phone", String.Empty), _
                                New XElement("Street", String.Empty), _
                                New XElement("PostalCode", String.Empty), _
                                New XElement("City", String.Empty), _
                                New XElement("Region", String.Empty), _
                                New XElement("Country", String.Empty) _
                                )
                End If

                'Names cannot be empty nor contain spaces.
                For Each xElem As XElement In xElemHdr.Elements()
                    If String.IsNullOrEmpty(xElem.Value) Then
                        xElem.Value = xElem.Name.ToString()
                    End If
                    xElem.Value = xElem.Value.Trim().Replace(" ", "_")
                Next

                'Gather the information.
                Dim xElemList As List(Of XElement) = New List(Of XElement)
                Dim xElemEvent As XElement

                'Add a localized header row when tags are not localized.
                If Not localizeTags Then
                    xElemEvent = New XElement(xNamespace + "Enrollee", _
                                    New XElement(xNamespace + xElemHdr.Elements("EventName").First().Value, _
                                                 Localization.GetString("Event Name.Header", LocalResourceFile)), _
                                    New XElement(xNamespace + xElemHdr.Elements("EventStart").First().Value, _
                                                 Localization.GetString("Event Start.Header", LocalResourceFile)), _
                                    New XElement(xNamespace + xElemHdr.Elements("EventEnd").First().Value, _
                                                 Localization.GetString("Event End.Header", LocalResourceFile)), _
                                    New XElement(xNamespace + xElemHdr.Elements("Location").First().Value, _
                                                 Localization.GetString("Location.Header", LocalResourceFile)), _
                                    New XElement(xNamespace + xElemHdr.Elements("Category").First().Value, _
                                                 Localization.GetString("Category.Header", LocalResourceFile)), _
                                    New XElement(xNamespace + xElemHdr.Elements("ReferenceNumber").First().Value, _
                                                 Localization.GetString("ReferenceNumber.Header", LocalResourceFile)), _
                                    New XElement(xNamespace + xElemHdr.Elements("Company").First().Value, _
                                                 Localization.GetString("Company.Header", LocalResourceFile)), _
                                    New XElement(xNamespace + xElemHdr.Elements("JobTitle").First().Value, _
                                                 Localization.GetString("JobTitle.Header", LocalResourceFile)), _
                                    New XElement(xNamespace + xElemHdr.Elements("FullName").First().Value, _
                                                 Localization.GetString("FullName.Header", LocalResourceFile)), _
                                    New XElement(xNamespace + xElemHdr.Elements("FirstName").First().Value, _
                                                 Localization.GetString("FirstName.Header", LocalResourceFile)), _
                                    New XElement(xNamespace + xElemHdr.Elements("LastName").First().Value, _
                                                 Localization.GetString("LastName.Header", LocalResourceFile)), _
                                    New XElement(xNamespace + xElemHdr.Elements("Email").First().Value, _
                                                 Localization.GetString("Email.Header", LocalResourceFile)), _
                                    New XElement(xNamespace + xElemHdr.Elements("Phone").First().Value, _
                                                 Localization.GetString("Phone.Header", LocalResourceFile)), _
                                    New XElement(xNamespace + xElemHdr.Elements("Street").First().Value, _
                                                 Localization.GetString("Street.Header", LocalResourceFile)), _
                                    New XElement(xNamespace + xElemHdr.Elements("PostalCode").First().Value, _
                                                 Localization.GetString("PostalCode.Header", LocalResourceFile)), _
                                    New XElement(xNamespace + xElemHdr.Elements("City").First().Value, _
                                                 Localization.GetString("City.Header", LocalResourceFile)), _
                                    New XElement(xNamespace + xElemHdr.Elements("Region").First().Value, _
                                                 Localization.GetString("Region.Header", LocalResourceFile)), _
                                    New XElement(xNamespace + xElemHdr.Elements("Country").First().Value, _
                                                 Localization.GetString("Country.Header", LocalResourceFile)) _
                                    )
                    xElemList.Add(xElemEvent)
                End If

                Dim objCtlUser As New UserController
                For Each eventSignup As EventSignupsInfo In eventSignups
                    If eventSignup.UserID <> -1 Then
                        'Known DNN/Evoq user. Get info from user profile.
                        Dim objUser As UserInfo = objCtlUser.GetUser(PortalId, eventSignup.UserID)
                        xElemEvent = New XElement(xNamespace + "Enrollee", _
                                        New XElement(xNamespace + xElemHdr.Elements("EventName").First().Value, theEvent.EventName), _
                                        New XElement(xNamespace + xElemHdr.Elements("EventStart").First().Value, theEvent.EventTimeBegin), _
                                        New XElement(xNamespace + xElemHdr.Elements("EventEnd").First().Value, theEvent.EventTimeEnd), _
                                        New XElement(xNamespace + xElemHdr.Elements("Location").First().Value, theEvent.LocationName), _
                                        New XElement(xNamespace + xElemHdr.Elements("Category").First().Value, theEvent.CategoryName), _
                                        New XElement(xNamespace + xElemHdr.Elements("ReferenceNumber").First().Value, eventSignup.ReferenceNumber), _
                                        New XElement(xNamespace + xElemHdr.Elements("Company").First().Value, GetPropertyForDownload(objUser, "Company")), _
                                        New XElement(xNamespace + xElemHdr.Elements("JobTitle").First().Value, GetPropertyForDownload(objUser, "JobTitle")), _
                                        New XElement(xNamespace + xElemHdr.Elements("FullName").First().Value, objUser.DisplayName), _
                                        New XElement(xNamespace + xElemHdr.Elements("FirstName").First().Value, objUser.FirstName), _
                                        New XElement(xNamespace + xElemHdr.Elements("LastName").First().Value, objUser.LastName), _
                                        New XElement(xNamespace + xElemHdr.Elements("Email").First().Value, objUser.Email), _
                                        New XElement(xNamespace + xElemHdr.Elements("Phone").First().Value, GetPropertyForDownload(objUser, "Telephone")), _
                                        New XElement(xNamespace + xElemHdr.Elements("Street").First().Value, GetPropertyForDownload(objUser, "Street")), _
                                        New XElement(xNamespace + xElemHdr.Elements("PostalCode").First().Value, GetPropertyForDownload(objUser, "PostalCode")), _
                                        New XElement(xNamespace + xElemHdr.Elements("City").First().Value, GetPropertyForDownload(objUser, "City")), _
                                        New XElement(xNamespace + xElemHdr.Elements("Region").First().Value, GetPropertyForDownload(objUser, "Region")), _
                                        New XElement(xNamespace + xElemHdr.Elements("Country").First().Value, GetPropertyForDownload(objUser, "Country")) _
                                        )
                    Else
                        'Anonymous user (site visitor). Get info from event signup.
                        xElemEvent = New XElement(xNamespace + "Enrollee", _
                                        New XElement(xNamespace + xElemHdr.Elements("EventName").First().Value, theEvent.EventName), _
                                        New XElement(xNamespace + xElemHdr.Elements("EventStart").First().Value, theEvent.EventTimeBegin), _
                                        New XElement(xNamespace + xElemHdr.Elements("EventEnd").First().Value, theEvent.EventTimeEnd), _
                                        New XElement(xNamespace + xElemHdr.Elements("Location").First().Value, theEvent.LocationName), _
                                        New XElement(xNamespace + xElemHdr.Elements("Category").First().Value, theEvent.CategoryName), _
                                        New XElement(xNamespace + xElemHdr.Elements("ReferenceNumber").First().Value, eventSignup.ReferenceNumber), _
                                        New XElement(xNamespace + xElemHdr.Elements("Company").First().Value, eventSignup.Company), _
                                        New XElement(xNamespace + xElemHdr.Elements("JobTitle").First().Value, eventSignup.JobTitle), _
                                        New XElement(xNamespace + xElemHdr.Elements("FullName").First().Value, eventSignup.AnonName), _
                                        New XElement(xNamespace + xElemHdr.Elements("FirstName").First().Value, eventSignup.FirstName), _
                                        New XElement(xNamespace + xElemHdr.Elements("LastName").First().Value, eventSignup.LastName), _
                                        New XElement(xNamespace + xElemHdr.Elements("Email").First().Value, eventSignup.AnonEmail), _
                                        New XElement(xNamespace + xElemHdr.Elements("Phone").First().Value, eventSignup.AnonTelephone), _
                                        New XElement(xNamespace + xElemHdr.Elements("Street").First().Value, eventSignup.Street), _
                                        New XElement(xNamespace + xElemHdr.Elements("PostalCode").First().Value, eventSignup.PostalCode), _
                                        New XElement(xNamespace + xElemHdr.Elements("City").First().Value, eventSignup.City), _
                                        New XElement(xNamespace + xElemHdr.Elements("Region").First().Value, eventSignup.Region), _
                                        New XElement(xNamespace + xElemHdr.Elements("Country").First().Value, eventSignup.Country) _
                                        )
                    End If
                    xElemList.Add(xElemEvent)
                Next

                'Aggregate into a document.
                Dim xElemMain As XElement = xRoot
                xElemMain.Add(xElemList) 'Everything ...
                xmlDoc.Load(xElemMain.CreateReader())

                'Add a declaration.
                Dim xmlDecla As XmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", Nothing)
                Dim xmlElemDoc As XmlElement = xmlDoc.DocumentElement
                xmlDoc.InsertBefore(xmlDecla, xmlElemDoc)

                'Return.
                DefineXmlFile = xmlDoc
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Function

        ''' <summary>
        ''' Define the csv file for downloading the enrollees for this event.
        ''' </summary>
        Private Function DefineCsvFile(ByVal theEvent As EventInfo) As String
            DefineCsvFile = Nothing
            Dim csvDoc As StringBuilder = New StringBuilder()

            'From xml to csv by xslt.
            Dim xmlDoc As XmlDocument = DefineXmlFile(theEvent, False)
            If xmlDoc IsNot Nothing Then
                Dim xPathDoc As XPathDocument = New XPathDocument(XmlReader.Create(New StringReader(xmlDoc.InnerXml.ToString())))
                Dim xslTrafo As XslCompiledTransform = New XslCompiledTransform()
                xslTrafo.Load(XmlReader.Create(New StringReader(File.ReadAllText(Path.Combine(Request.MapPath(ControlPath), "EventEnrollees.xslt")))))
                Dim xWriter As StringWriter = New StringWriter(csvDoc)
                xslTrafo.Transform(xPathDoc, Nothing, xWriter)
                xWriter.Close()
            End If

            Return csvDoc.ToString()
        End Function

        ''' <summary>
        ''' Generate the xml file for downloading the enrollees for this event.
        ''' </summary>
        Private Function GenerateXmlFile(ByVal theEvent As EventInfo, ByVal xmlDoc As XmlDocument) As Boolean
            GenerateXmlFile = False

            'Name the file with a timestamp.
            Dim fileName As String = theEvent.EventName
            If Not String.IsNullOrEmpty(Localization.GetString("EnrollmentsFile.Text", LocalResourceFile)) Then
                fileName &= " - " & Localization.GetString("EnrollmentsFile.Text", LocalResourceFile)
            End If
            fileName &= DateTime.Now.ToString(" - yyyyMMdd_HHmmss")

            'The contents of the file.
            Dim fileContent As String = xmlDoc.InnerXml.ToString()

            Try
                'Stream the file.
                Dim myContext As HttpContext = HttpContext.Current
                Dim myResponse As HttpResponse
                myResponse = myContext.Response
                myResponse.ContentEncoding = Encoding.UTF8
                myResponse.ContentType = "application/force-download"
                myResponse.AppendHeader("Content-Disposition", "filename=""" & fileName & ".xml""")

                myResponse.Write(fileContent)
                myResponse.End()

                GenerateXmlFile = True
            Catch exc As Exception
                GenerateXmlFile = False
            End Try
        End Function

        ''' <summary>
        ''' Generate the csv file for downloading the enrollees for this event.
        ''' </summary>
        Private Function GenerateCsvFile(ByVal theEvent As EventInfo, ByVal csvDoc As String) As Boolean
            GenerateCsvFile = False

            'Name the file with a timestamp.
            Dim fileName As String = theEvent.EventName
            If Not String.IsNullOrEmpty(Localization.GetString("EnrollmentsFile.Text", LocalResourceFile)) Then
                fileName &= " - " & Localization.GetString("EnrollmentsFile.Text", LocalResourceFile)
            End If
            fileName &= DateTime.Now.ToString(" - yyyyMMdd_HHmmss")

            'The contents of the file.
            Dim fileContent As String = csvDoc

            Try
                'Stream the file.
                Dim myContext As HttpContext = HttpContext.Current
                Dim myResponse As HttpResponse
                myResponse = myContext.Response
                myResponse.ContentEncoding = Encoding.UTF8
                myResponse.ContentType = "application/force-download"
                myResponse.AppendHeader("Content-Disposition", "filename=""" & fileName & ".csv""")

                myResponse.Write(fileContent)
                myResponse.End()

                GenerateCsvFile = True
            Catch exc As Exception
                GenerateCsvFile = False
            End Try
        End Function

        Private Function GetPropertyForDownload(ByRef objUser As UserInfo, ByVal propertyName As String) As String
            GetPropertyForDownload = Nothing

            If Not String.IsNullOrEmpty(propertyName) Then
                Dim profileProperty As ProfilePropertyDefinition = objUser.Profile.GetProperty(propertyName)

                If profileProperty IsNot Nothing Then
                    GetPropertyForDownload = profileProperty.PropertyValue
                End If
            End If
        End Function

        Private Sub ShowMessage(ByVal msg As String, ByVal messageLevel As MessageLevel)
            lblMessage.Text = msg

            'Hide the rest of the form fields.
            divMessage.Attributes.Add("style", "display:block;")

            Select Case messageLevel
                Case messageLevel.DNNSuccess
                    divMessage.Attributes.Add("class", "dnnFormMessage dnnFormSuccess")
                Case messageLevel.DNNInformation
                    divMessage.Attributes.Add("class", "dnnFormMessage dnnFormInfo")
                Case messageLevel.DNNWarning
                    divMessage.Attributes.Add("class", "dnnFormMessage dnnFormWarning")
                Case messageLevel.DNNError
                    divMessage.Attributes.Add("class", "dnnFormMessage dnnFormValidationSummary")
            End Select
        End Sub
#End Region

#Region "Links and Buttons"
        ''' <summary>
        ''' When delete button is clicked the current event will be removed
        ''' </summary>
        Private Sub deleteButton_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles deleteButton.Click
            Try
                Dim iItemID As Integer = ItemId
                Dim objCtlEvent As New EventController
                Dim objEvent As EventInfo = objCtlEvent.EventsGet(iItemID, ModuleId)
                If objEvent.RRULE <> "" Then
                    objEvent.Cancelled = True
                    objEvent.LastUpdatedID = UserId
                    objCtlEvent.EventsSave(objEvent, True, TabId, True)
                Else
                    Dim objCtlEventRecurMaster As New EventRecurMasterController
                    objCtlEventRecurMaster.EventsRecurMasterDelete(objEvent.RecurMasterID, objEvent.ModuleID)
                End If
                Response.Redirect(GetSocialNavigateUrl(), True)
            Catch ex As Threading.ThreadAbortException
                'Ignore
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' <summary>
        ''' When delete series button is clicked the current event series will be removed
        ''' </summary>
        Private Sub deleteSeriesButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles deleteSeriesButton.Click
            Try
                Dim iItemID As Integer = ItemId
                Dim objCtlEvent As New EventController
                Dim objEvent As EventInfo = objCtlEvent.EventsGet(iItemID, ModuleId)
                Dim objCtlEventRecurMaster As New EventRecurMasterController
                objCtlEventRecurMaster.EventsRecurMasterDelete(objEvent.RecurMasterID, objEvent.ModuleID)
                Response.Redirect(GetSocialNavigateUrl(), True)
            Catch ex As Threading.ThreadAbortException
                'Ignore
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' <summary>
        ''' When Export simple button is clicked the current event will be exported to a vcard
        ''' </summary>
        Private Sub cmdvEvent_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdvEvent.Click
            ExportEvent(False)
        End Sub

        ''' <summary>
        ''' When Export event series button is clicked the current event will be exported to a vcard
        ''' </summary>
        Private Sub cmdvEventSeries_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdvEventSeries.Click
            ExportEvent(True)
        End Sub

        ''' <summary>
        ''' When Download event signups button is clicked the current event signups will be written to an XML file for download.
        ''' </summary>
        Private Sub cmdvEventSignups_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdvEventSignups.Click
            DownloadSignups()
        End Sub

        ''' <summary>
        ''' When return button is clicked the user is redirected to the previous page
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub returnButton_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles returnButton.Click
            Try
                Response.Redirect(GetSocialNavigateUrl(), True)
            Catch ex As Threading.ThreadAbortException
                'Ignore
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' <summary>
        ''' When signup button is clicked the user will be signed up for the event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub cmdSignup_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles cmdSignup.Click
            If Not Request.IsAuthenticated And Not Settings.AllowAnonEnroll Then
                RedirectToLogin()
            End If

            Try
                Dim objEvent As EventInfo
                Dim objCtlEvent As New EventController
                objEvent = objCtlEvent.EventsGet(ItemId, ModuleId)
                If Not Request.IsAuthenticated And Not objEvent.AllowAnonEnroll Then
                    RedirectToLogin()
                End If

                ' In case of custom enrollment page.
                If Settings.EnrollmentPageAllowed Then
                    If Not String.IsNullOrEmpty(Settings.EnrollmentPageDefaultUrl) Then
                        Response.Redirect(Settings.EnrollmentPageDefaultUrl & "?mod=" & ModuleId & "&event=" & ItemId)
                    End If
                    Exit Sub
                End If

                ' In case of standard paid enrollment.
                ' Check to see if unauthenticated user has already enrolled
                Dim objCtlEventSignups As New EventSignupsController
                If Not Request.IsAuthenticated Then
                    Dim objEventsSignups As EventSignupsInfo
                    objEventsSignups = objCtlEventSignups.EventsSignupsGetAnonUser(objEvent.EventID, txtAnonEmail.Text, objEvent.ModuleID)
                    If Not objEventsSignups Is Nothing Then
                        ShowMessage(Localization.GetString("YouAreAlreadyEnrolledForThisEvent", LocalResourceFile), MessageLevel.DNNWarning)
                        enroll1.Visible = False
                        enroll3.Visible = False
                        enroll5.Visible = False
                        Exit Sub
                    End If
                End If

                If objEvent.EnrollType = "PAID" Then
                    ' Paid Even Process
                    Try
                        Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
                        Dim socialGroupId As Integer = GetUrlGroupId()
                        If Request.IsAuthenticated Then
                            If socialGroupId > 0 Then
                                Response.Redirect(objEventInfoHelper.AddSkinContainerControls(NavigateURL(TabId, "PPEnroll", "Mid=" & ModuleId, "ItemID=" & ItemId, "NoEnrol=" & txtNoEnrolees.Text, "groupid=" & socialGroupId.ToString), "?"))
                            Else
                                Response.Redirect(objEventInfoHelper.AddSkinContainerControls(NavigateURL(TabId, "PPEnroll", "Mid=" & ModuleId, "ItemID=" & ItemId, "NoEnrol=" & txtNoEnrolees.Text), "?"))
                            End If
                        Else
                            Dim urlAnonTelephone As String = Trim(txtAnonTelephone.Text)
                            If urlAnonTelephone = "" Then
                                urlAnonTelephone = "0"
                            End If
                            If socialGroupId > 0 Then
                                Response.Redirect(objEventInfoHelper.AddSkinContainerControls(NavigateURL(TabId, "PPEnroll", "Mid=" & ModuleId, "ItemID=" & ItemId, "NoEnrol=" & txtNoEnrolees.Text, "groupid=" & socialGroupId.ToString, "AnonEmail=" & HttpUtility.UrlEncode(txtAnonEmail.Text), "AnonName=" & HttpUtility.UrlEncode(txtAnonName.Text), "AnonPhone=" & HttpUtility.UrlEncode(urlAnonTelephone)), "&"))
                            Else
                                Response.Redirect(objEventInfoHelper.AddSkinContainerControls(NavigateURL(TabId, "PPEnroll", "Mid=" & ModuleId, "ItemID=" & ItemId, "NoEnrol=" & txtNoEnrolees.Text, "AnonEmail=" & HttpUtility.UrlEncode(txtAnonEmail.Text), "AnonName=" & HttpUtility.UrlEncode(txtAnonName.Text), "AnonPhone=" & HttpUtility.UrlEncode(urlAnonTelephone)), "&"))
                            End If
                        End If
                    Catch exc As Exception 'Module failed to load
                        ProcessModuleLoadException(Me, exc)
                    End Try
                Else
                    ' Non-Paid Event Process
                    ' Got the Event, Now Add the User to the EventSignups
                    Dim objEventSignups As New EventSignupsInfo
                    objEventSignups.EventID = objEvent.EventID

                    Dim startdate As DateTime = objEvent.EventTimeBegin
                    SelectedDate = startdate.Date

                    objEventSignups.ModuleID = objEvent.ModuleID
                    If Request.IsAuthenticated Then
                        objEventSignups.UserID = UserId
                        objEventSignups.AnonEmail = Nothing
                        objEventSignups.AnonName = Nothing
                        objEventSignups.AnonTelephone = Nothing
                        objEventSignups.AnonCulture = Nothing
                        objEventSignups.AnonTimeZoneId = Nothing
                    Else
                        Dim objSecurity As New PortalSecurity
                        objEventSignups.UserID = -1
                        objEventSignups.AnonEmail = txtAnonEmail.Text
                        objEventSignups.AnonName = objSecurity.InputFilter(txtAnonName.Text, PortalSecurity.FilterFlag.NoScripting)
                        objEventSignups.AnonTelephone = objSecurity.InputFilter(txtAnonTelephone.Text, PortalSecurity.FilterFlag.NoScripting)
                        objEventSignups.AnonCulture = Threading.Thread.CurrentThread.CurrentCulture.Name
                        objEventSignups.AnonTimeZoneId = GetDisplayTimeZoneId()
                    End If
                    objEventSignups.PayPalPaymentDate = DateTime.UtcNow
                    objEventSignups.NoEnrolees = CInt(txtNoEnrolees.Text)
                    If IsModerator() Or _
                        PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString) Then
                        objEventSignups.Approved = True
                        objEvent.Enrolled += 1
                    ElseIf Settings.Moderateall Then
                        objEventSignups.Approved = False
                    Else
                        objEventSignups.Approved = True
                        objEvent.Enrolled += 1
                    End If
                    objEventSignups = CreateEnrollment(objEventSignups, objEvent)
                    enroll1.Visible = False
                    Dim msgLevel As MessageLevel = UserEnrollment(objEvent)
                    ShowMessage(lblSignup.Text, msgLevel)
                    ' Send Moderator email
                    Dim objEventEmailInfo As New EventEmailInfo
                    Dim objEventEmail As New EventEmails(PortalId, ModuleId, LocalResourceFile, CType(Page, PageBase).PageCulture.Name)
                    If Settings.Moderateall And objEventSignups.Approved = False Then
                        objEventEmailInfo.TxtEmailSubject = Settings.Templates.moderateemailsubject
                        objEventEmailInfo.TxtEmailBody = Settings.Templates.moderateemailmessage
                        objEventEmailInfo.TxtEmailFrom() = Settings.StandardEmail
                        Dim moderators As ArrayList = GetModerators()
                        For Each moderator As UserInfo In moderators
                            objEventEmailInfo.UserEmails.Add(moderator.Email)
                            objEventEmailInfo.UserLocales.Add(moderator.Profile.PreferredLocale)
                            objEventEmailInfo.UserTimeZoneIds.Add(moderator.Profile.PreferredTimeZone.Id)
                        Next
                        objEventEmail.SendEmails(objEventEmailInfo, objEvent, objEventSignups)
                    End If

                    ' Mail users
                    objEventEmailInfo = New EventEmailInfo
                    objEventEmailInfo.TxtEmailSubject = Settings.Templates.txtEnrollMessageSubject
                    objEventEmailInfo.TxtEmailFrom() = Settings.StandardEmail
                    If Request.IsAuthenticated Then
                        objEventEmailInfo.UserEmails.Add(PortalSettings.UserInfo.Email)
                        objEventEmailInfo.UserLocales.Add(PortalSettings.UserInfo.Profile.PreferredLocale)
                        objEventEmailInfo.UserTimeZoneIds.Add(PortalSettings.UserInfo.Profile.PreferredTimeZone.Id)
                    Else
                        objEventEmailInfo.UserEmails.Add(objEventSignups.AnonEmail)
                        objEventEmailInfo.UserLocales.Add(objEventSignups.AnonCulture)
                        objEventEmailInfo.UserTimeZoneIds.Add(objEventSignups.AnonTimeZoneId)
                    End If
                    objEventEmailInfo.UserIDs.Add(objEvent.OwnerID)
                    If objEventSignups.Approved Then
                        If Settings.SendEnrollMessageApproved Then
                            objEventEmailInfo.TxtEmailBody = Settings.Templates.txtEmailMessage & Settings.Templates.txtEnrollMessageApproved
                            objEventEmail.SendEmails(objEventEmailInfo, objEvent, objEventSignups)
                        End If
                    Else
                        If Settings.SendEnrollMessageWaiting Then
                            objEventEmailInfo.TxtEmailBody = Settings.Templates.txtEmailMessage & Settings.Templates.txtEnrollMessageWaiting
                            objEventEmail.SendEmails(objEventEmailInfo, objEvent, objEventSignups)
                        End If
                    End If

                End If
                BindEnrollList(objEvent)

            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' <summary>
        ''' When notify button is clicked the user will be added to the notification list
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub cmdNotify_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles cmdNotify.Click
            valEmail.Validate()
            valEmail2.Validate()
            If Not valEmail.IsValid Or Not valEmail2.IsValid Then
                Return
            End If

            Dim lstEvents As New ArrayList
            Dim objEventNotification As EventNotificationInfo
            Dim objEventNotificationController As EventNotificationController = New EventNotificationController
            Dim eventDate As DateTime
            Dim notifyTime As DateTime
            Dim currentEv As Integer
            Dim objEventTimeZoneUtilities As New EventTimeZoneUtilities
            Try
                Dim eventEvent As EventInfo
                Dim objCtlEvent As New EventController
                eventEvent = objCtlEvent.EventsGet(ItemId, ModuleId)
                currentEv = eventEvent.EventID

                If chkReminderRec.Checked Then
                    lstEvents = objCtlEvent.EventsGetRecurrences(eventEvent.RecurMasterID, ModuleId)
                Else
                    lstEvents.Add(eventEvent)
                End If

                For Each eventEvent In lstEvents
                    If eventEvent.SendReminder = True And eventEvent.EventTimeBegin > objEventTimeZoneUtilities.ConvertFromUTCToModuleTimeZone(Date.UtcNow, eventEvent.EventTimeZoneId) Then
                        eventDate = objEventTimeZoneUtilities.ConvertToUTCTimeZone(eventEvent.EventTimeBegin, eventEvent.EventTimeZoneId)
                        objEventNotification = objEventNotificationController.EventsNotificationGet(eventEvent.EventID, txtUserEmail.Text, ModuleId)

                        notifyTime = eventDate
                        '*** Calculate notification time
                        Select Case ddlReminderTimeMeasurement.SelectedValue
                            Case "m"
                                notifyTime = notifyTime.AddMinutes(CInt(txtReminderTime.Text) * -1)
                            Case "h"
                                notifyTime = notifyTime.AddHours(CInt(txtReminderTime.Text) * -1)
                            Case "d"
                                notifyTime = notifyTime.AddDays(CInt(txtReminderTime.Text) * -1)
                        End Select
                        ' Registered users will overwrite existing notifications (in recurring events)
                        Dim notifyDisplayTime As DateTime = objEventTimeZoneUtilities.ConvertFromUTCToDisplayTimeZone(notifyTime, GetDisplayTimeZoneId()).EventDate
                        If Not (objEventNotification Is Nothing) And Request.IsAuthenticated() Then
                            objEventNotification.NotifyByDateTime = notifyTime
                            objEventNotificationController.EventsNotificationSave(objEventNotification)
                            If currentEv = eventEvent.EventID Then
                                lblConfirmation.Text = String.Format(Localization.GetString("lblReminderConfirmation", LocalResourceFile), notifyDisplayTime.ToString)
                                ShowMessage(lblConfirmation.Text, MessageLevel.DNNSuccess)
                            End If
                            ' Anonymous users can never overwrite an existing notification
                        ElseIf Not (objEventNotification Is Nothing) Then
                            If currentEv = eventEvent.EventID Then
                                lblConfirmation.Text = String.Format(Localization.GetString("ReminderAlreadyReg", LocalResourceFile), txtUserEmail.Text.ToString, objEventNotification.NotifyByDateTime.ToString)
                                ShowMessage(lblConfirmation.Text, MessageLevel.DNNWarning)
                            End If
                        Else
                            objEventNotification = New EventNotificationInfo
                            objEventNotification.NotificationID = -1
                            objEventNotification.EventID = eventEvent.EventID
                            objEventNotification.PortalAliasID = PortalAlias.PortalAliasID
                            objEventNotification.NotificationSent = False
                            objEventNotification.EventTimeBegin = eventDate
                            objEventNotification.NotifyLanguage = Threading.Thread.CurrentThread.CurrentCulture.Name
                            objEventNotification.ModuleID = ModuleId
                            objEventNotification.TabID = TabId
                            objEventNotification.NotifyByDateTime = notifyTime
                            objEventNotification.UserEmail = txtUserEmail.Text
                            objEventNotificationController.EventsNotificationSave(objEventNotification)
                            If currentEv = eventEvent.EventID Then
                                lblConfirmation.Text = String.Format(Localization.GetString("lblReminderConfirmation", LocalResourceFile), notifyDisplayTime.ToString)
                                ShowMessage(lblConfirmation.Text, MessageLevel.DNNSuccess)
                            End If
                        End If
                    End If
                Next
                rem1.Visible = False
                rem2.Visible = False
                rem3.Visible = True
                imgConfirmation.AlternateText = Localization.GetString("Reminder", LocalResourceFile)
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdPrint_PreRender(ByVal sender As Object, ByVal e As EventArgs) Handles cmdPrint.PreRender

            cmdPrint.Target = " _blank"
            cmdPrint.NavigateUrl = NavigateURL(TabId, PortalSettings, "", "mid=" & ModuleId, "itemid=" & ItemId, "ctl=Details", "ShowNav=False", "dnnprintmode=true", "SkinSrc=%5bG%5dSkins%2f_default%2fNo+Skin", "ContainerSrc=%5bG%5dContainers%2f_default%2fNo+Container")

        End Sub
        Private Sub cmdEmail_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdEmail.Click
            valEmailiCal.Validate()
            valEmailiCal2.Validate()
            If Not valEmailiCal.IsValid Or Not valEmailiCal2.IsValid Then
                Return
            End If

            Dim objCtlEvent As New EventController
            Dim objEvent As EventInfo = objCtlEvent.EventsGet(ItemId, ModuleId)
            If objEvent Is Nothing Then
                Exit Sub
            End If

            Dim iCalendar As New VEvent(False, HttpContext.Current)
            Dim iCal As String
            iCal = iCalendar.CreateiCal(TabId, ModuleId, ItemId, objEvent.SocialGroupId)

            Dim attachment As Net.Mail.Attachment = Net.Mail.Attachment.CreateAttachmentFromString(iCal, New Net.Mime.ContentType("text/calendar"))
            attachment.TransferEncoding = Net.Mime.TransferEncoding.Base64
            attachment.Name = objEvent.EventName + ".ics"
            Dim attachments As New Generic.List(Of Net.Mail.Attachment)
            attachments.Add(attachment)

            Dim objEventEmailInfo As New EventEmailInfo
            Dim objEventEmail As New EventEmails(PortalId, ModuleId, LocalResourceFile, CType(Page, PageBase).PageCulture.Name)
            objEventEmailInfo.TxtEmailSubject = Settings.Templates.EventiCalSubject
            objEventEmailInfo.TxtEmailBody = Settings.Templates.EventiCalBody
            objEventEmailInfo.TxtEmailFrom() = Settings.StandardEmail
            objEventEmailInfo.UserEmails.Add(txtUserEmailiCal.Text)
            objEventEmailInfo.UserLocales.Add("")
            objEventEmailInfo.UserTimeZoneIds.Add(objEvent.EventTimeZoneId)

            objEventEmail.SendEmails(objEventEmailInfo, objEvent, attachments)
            divMessage.Attributes.Add("style", "display:block;")
            ShowMessage(String.Format(Localization.GetString("ConfirmationiCal", LocalResourceFile), txtUserEmailiCal.Text), MessageLevel.DNNSuccess)

        End Sub

#End Region

    End Class
End Namespace