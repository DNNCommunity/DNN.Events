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

Imports System.Collections

Namespace DotNetNuke.Modules.Events

    <DNNtc.ModuleControlProperties("Moderate", "Moderate Events and Enrollment", DNNtc.ControlType.View, "https://dnnevents.codeplex.com/documentation", True, True)> _
    Partial Class EventModerate
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

#Region "Private Area"
        Private ReadOnly _objCtlEvent As New EventController
        Private ReadOnly _objCtlEventRecurMaster As New EventRecurMasterController
        Private ReadOnly _objCtlEventSignups As New EventSignupsController
        Private _eventModeration, _eventRecurModeration As New ArrayList
#End Region

#Region "Event Handlers"
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
            Try
                ' Verify that the current user has moderator access to this module
                If Security.PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString) Or _
                   IsModerator() Then
                Else
                    Response.Redirect(GetSocialNavigateUrl(), True)
                End If

                ' Set the selected theme 
                SetTheme(pnlEventsModuleModerate)

                If Page.IsPostBack = False Then
                    txtEmailFrom.Text = UserInfo.Email.ToString
                    LocalizeAll()
                    'Are You Sure You Wish To Update/Delete Item(s) (and send Email) ?'
                    cmdUpdateSelected.Attributes.Add("onclick", "javascript:return confirm('" + Localization.GetString("ConfirmUpdateDeleteModerate", LocalResourceFile) + "');")
                    BindData()
                End If
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub
#End Region

#Region "Helper Methods"
        Private Sub BindData()
            Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
            Dim objEventTimeZoneUtilities As New EventTimeZoneUtilities
            _eventModeration = New ArrayList
            Select Case rbModerate.SelectedValue
                Case "Events"
                    _eventModeration = objEventInfoHelper.ConvertEventListToDisplayTimeZone(_objCtlEvent.EventsModerateEvents(ModuleId, GetUrlGroupId), GetDisplayTimeZoneId())

                    _eventRecurModeration = New ArrayList
                    _eventRecurModeration = _objCtlEventRecurMaster.EventsRecurMasterModerate(ModuleId, GetUrlGroupId)
                    For Each objRecurMaster As EventRecurMasterInfo In _eventRecurModeration
                        objRecurMaster.Dtstart = objEventTimeZoneUtilities.ConvertToDisplayTimeZone(objRecurMaster.Dtstart, objRecurMaster.EventTimeZoneId, PortalId, GetDisplayTimeZoneId()).EventDate
                    Next

                    'Get data for selected date and fill grid
                    grdEvents.DataSource = _eventModeration
                    grdEvents.DataBind()

                    If _eventRecurModeration.Count > 0 Then
                        grdRecurEvents.DataSource = _eventRecurModeration
                        grdRecurEvents.DataBind()
                        grdRecurEvents.Visible = True
                    End If

                    grdEvents.Visible = True
                    grdEnrollment.Visible = False
                Case "Enrollment"
                    _eventModeration = _objCtlEventSignups.EventsModerateSignups(ModuleId, GetUrlGroupId)

                    Dim objSignup As EventSignupsInfo
                    For Each objSignup In _eventModeration
                        If objSignup.UserID <> -1 Then
                            objSignup.UserName = objEventInfoHelper.UserDisplayNameProfile(objSignup.UserID, objSignup.UserName, LocalResourceFile).DisplayNameURL
                        Else
                            objSignup.UserName = objSignup.AnonName
                            objSignup.Email = objSignup.AnonEmail
                        End If
                        If objSignup.Email = "" Then
                            objSignup.EmailVisible = False
                        Else
                            objSignup.EmailVisible = True
                        End If
                        objSignup.EventTimeBegin = objEventTimeZoneUtilities.ConvertToDisplayTimeZone(objSignup.EventTimeBegin, objSignup.EventTimeZoneId, PortalId, GetDisplayTimeZoneId()).EventDate
                    Next

                    'Get data for selected date and fill grid
                    grdEnrollment.DataSource = _eventModeration
                    grdEnrollment.DataBind()
                    'Add Remove Popup to grid
                    Dim i As Integer
                    If grdEnrollment.Items.Count > 0 Then
                        For i = 0 To grdEnrollment.Items.Count - 1
                            'Are You Sure You Wish To Email the User?'
                            CType(grdEnrollment.Items(i).FindControl("btnUserEmail"), ImageButton).Attributes.Add("onclick", "javascript:return confirm('" + Localization.GetString("ConfirmModerateSendMailToUser", LocalResourceFile) + "');")
                            CType(grdEnrollment.Items(i).FindControl("btnUserEmail"), ImageButton).AlternateText = Localization.GetString("EmailUser", LocalResourceFile)
                            CType(grdEnrollment.Items(i).FindControl("btnUserEmail"), ImageButton).ToolTip = Localization.GetString("EmailUser", LocalResourceFile)
                        Next
                        grdEvents.Visible = False
                        grdRecurEvents.Visible = False
                        grdEnrollment.Visible = True
                    End If
            End Select
            If _eventModeration.Count < 1 Then
                '"No New Events/Enrollments to Moderate..."
                lblMessage.Text = Localization.GetString("MsgModerateNothingToModerate", LocalResourceFile)
                ShowButtonsGrid(False)
            Else
                'Deny option will delete Event/Enrollment Entries from the Database!"
                lblMessage.Text = Localization.GetString("MsgModerateNoteDenyOption", LocalResourceFile)
                ShowButtonsGrid(True)
            End If
        End Sub

        Private Sub LocalizeAll()
            txtEmailSubject.Text = Settings.Templates.txtEmailSubject
            txtEmailMessage.Text = Settings.Templates.txtEmailMessage

            grdEvents.Columns(0).HeaderText = Localization.GetString("SingleAction", LocalResourceFile)
            grdEvents.Columns(1).HeaderText = Localization.GetString("Date", LocalResourceFile)
            grdEvents.Columns(2).HeaderText = Localization.GetString("Time", LocalResourceFile)
            grdEvents.Columns(3).HeaderText = Localization.GetString("Event", LocalResourceFile)

            grdRecurEvents.Columns(0).HeaderText = Localization.GetString("RecurAction", LocalResourceFile)
            grdRecurEvents.Columns(1).HeaderText = Localization.GetString("Date", LocalResourceFile)
            grdRecurEvents.Columns(2).HeaderText = Localization.GetString("Time", LocalResourceFile)
            grdRecurEvents.Columns(3).HeaderText = Localization.GetString("Event", LocalResourceFile)

            grdEnrollment.Columns(0).HeaderText = Localization.GetString("Action", LocalResourceFile)
            grdEnrollment.Columns(1).HeaderText = Localization.GetString("Date", LocalResourceFile)
            grdEnrollment.Columns(2).HeaderText = Localization.GetString("Time", LocalResourceFile)
            grdEnrollment.Columns(3).HeaderText = Localization.GetString("Event", LocalResourceFile)
            grdEnrollment.Columns(4).HeaderText = ""
            grdEnrollment.Columns(5).HeaderText = Localization.GetString("User", LocalResourceFile)
            grdEnrollment.Columns(6).HeaderText = Localization.GetString("NoEnrolees", LocalResourceFile)

        End Sub

        Private Sub ShowButtonsGrid(ByVal blShow As Boolean)
            pnlEmail.Visible = blShow
            pnlGrid.Visible = blShow
            cmdUpdateSelected.Visible = blShow
            cmdSelectApproveAll.Visible = blShow
            cmdSelectDenyAll.Visible = blShow
            cmdUnmarkAll.Visible = blShow
        End Sub
#End Region

#Region "Links and Buttons"
        Private Sub cmdUpdateSelected_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles cmdUpdateSelected.Click
            Dim item As DataGridItem
            Dim objEventEmail As New EventEmails(PortalId, ModuleId, LocalResourceFile, CType(Page, PageBase).PageCulture.Name)

            Try
                Select Case rbModerate.SelectedValue
                    Case "Events"
                        Dim objEventEmailInfo As New EventEmailInfo
                        objEventEmailInfo.TxtEmailSubject = txtEmailSubject.Text
                        objEventEmailInfo.TxtEmailBody = txtEmailMessage.Text
                        objEventEmailInfo.TxtEmailFrom() = txtEmailFrom.Text
                        Dim objCal As New EventInfo
                        Dim objEventRecurMaster As EventRecurMasterInfo
                        For Each item In grdEvents.Items
                            Select Case CType(item.FindControl("rbEventAction"), RadioButtonList).SelectedValue
                                Case "Approve"
                                    objCal = _objCtlEvent.EventsGet(CType(grdEvents.DataKeys(item.ItemIndex), Integer), ModuleId)
                                    objCal.Approved = True
                                    Dim newEventEmailSent As Boolean = objCal.NewEventEmailSent
                                    objCal.NewEventEmailSent = True
                                    _objCtlEvent.EventsSave(objCal, True, TabId, False)
                                    ' Only send event emails when event approved for first time
                                    If Not newEventEmailSent Then
                                        objCal.RRULE = ""
                                        SendNewEventEmails(objCal)
                                        CreateNewEventJournal(objCal)
                                    End If
                                    ' Email Requesting/Moderated User
                                    If chkEmail.Checked Then
                                        objCal.RRULE = ""
                                        objEventEmailInfo.UserIDs.Clear()
                                        objEventEmailInfo.UserIDs.Add(objCal.OwnerID)
                                        objEventEmail.SendEmails(objEventEmailInfo, objCal)
                                    End If
                                Case "Deny"
                                    objCal = _objCtlEvent.EventsGet(CType(grdEvents.DataKeys(item.ItemIndex), Integer), ModuleId)
                                    'Don't Allow Delete on Enrolled Event - Only Cancel
                                    objEventRecurMaster = _objCtlEventRecurMaster.EventsRecurMasterGet(objCal.RecurMasterID, objCal.ModuleID)
                                    If objEventRecurMaster.RRULE <> "" Then
                                        objCal.Cancelled = True
                                        objCal.LastUpdatedID = UserId
                                        objCal = _objCtlEvent.EventsSave(objCal, False, TabId, True)
                                    Else
                                        _objCtlEventRecurMaster.EventsRecurMasterDelete(objCal.RecurMasterID, objCal.ModuleID)
                                    End If
                                    ' Email Requesting/Moderated User
                                    If chkEmail.Checked Then
                                        objCal.RRULE = ""
                                        objEventEmailInfo.UserIDs.Clear()
                                        objEventEmailInfo.UserIDs.Add(objCal.OwnerID)
                                        objEventEmail.SendEmails(objEventEmailInfo, objCal)
                                    End If
                            End Select
                        Next
                        For Each item In grdRecurEvents.Items
                            Select Case CType(item.FindControl("rbEventRecurAction"), RadioButtonList).SelectedValue
                                Case "Approve"
                                    objEventRecurMaster = _objCtlEventRecurMaster.EventsRecurMasterGet(CType(grdRecurEvents.DataKeys(item.ItemIndex), Integer), ModuleId)
                                    objEventRecurMaster.Approved = True
                                    _objCtlEventRecurMaster.EventsRecurMasterSave(objEventRecurMaster, TabId, False)
                                    Dim lstEvents As ArrayList
                                    lstEvents = _objCtlEvent.EventsGetRecurrences(objEventRecurMaster.RecurMasterID, objEventRecurMaster.ModuleID)
                                    Dim blEmailSent As Boolean = False
                                    For Each objCal In lstEvents
                                        If Not objCal.Cancelled Then
                                            objCal.Approved = True
                                            Dim newEventEmailSent As Boolean = objCal.NewEventEmailSent
                                            objCal.NewEventEmailSent = True
                                            _objCtlEvent.EventsSave(objCal, True, TabId, False)
                                            ' Only send event emails when event approved for first time
                                            If Not newEventEmailSent And Not blEmailSent Then
                                                objCal.RRULE = objEventRecurMaster.RRULE
                                                SendNewEventEmails(objCal)
                                                CreateNewEventJournal(objCal)
                                                blEmailSent = True
                                            End If
                                        End If
                                    Next
                                    ' Email Requesting/Moderated User
                                    If chkEmail.Checked Then
                                        objCal.RRULE = objEventRecurMaster.RRULE
                                        objEventEmailInfo.UserIDs.Clear()
                                        objEventEmailInfo.UserIDs.Add(objEventRecurMaster.CreatedByID)
                                        objEventEmail.SendEmails(objEventEmailInfo, objCal)
                                    End If
                                Case "Deny"
                                    objEventRecurMaster = _objCtlEventRecurMaster.EventsRecurMasterGet(CType(grdRecurEvents.DataKeys(item.ItemIndex), Integer), ModuleId)
                                    'Don't Allow Delete on Enrolled Event - Only Cancel
                                    _objCtlEventRecurMaster.EventsRecurMasterDelete(CType(grdRecurEvents.DataKeys(item.ItemIndex), Integer), ModuleId)
                                    ' Email Requesting/Moderated User
                                    If chkEmail.Checked Then
                                        objCal.RRULE = objEventRecurMaster.RRULE
                                        objEventEmailInfo.UserIDs.Clear()
                                        objEventEmailInfo.UserIDs.Add(objEventRecurMaster.CreatedByID)
                                        objEventEmail.SendEmails(objEventEmailInfo, objCal)
                                    End If
                            End Select
                        Next
                    Case "Enrollment"
                        ' Not moderated
                        Dim objEnroll As EventSignupsInfo
                        For Each item In grdEnrollment.Items
                            If CType(item.FindControl("rbEnrollAction"), RadioButtonList).SelectedValue <> "" Then
                                objEnroll = _objCtlEventSignups.EventsSignupsGet(CType(grdEnrollment.DataKeys(item.ItemIndex), Integer), ModuleId, False)
                                Dim objCtlEvent As New EventController
                                Dim objEvent As EventInfo = objCtlEvent.EventsGet(objEnroll.EventID, objEnroll.ModuleID)
                                Dim objEventEmailInfo As New EventEmailInfo
                                objEventEmailInfo.TxtEmailSubject = txtEmailSubject.Text
                                objEventEmailInfo.TxtEmailFrom() = txtEmailFrom.Text
                                If chkEmail.Checked Then
                                    If objEnroll.UserID > -1 Then
                                        objEventEmailInfo.UserIDs.Add(objEnroll.UserID)
                                    Else
                                        objEventEmailInfo.UserEmails.Add(objEnroll.AnonEmail)
                                        objEventEmailInfo.UserLocales.Add(objEnroll.AnonCulture)
                                        objEventEmailInfo.UserTimeZoneIds.Add(objEnroll.AnonTimeZoneId)
                                    End If
                                End If
                                Select Case CType(item.FindControl("rbEnrollAction"), RadioButtonList).SelectedValue
                                    Case "Approve"
                                        objEnroll.Approved = True
                                        CreateEnrollment(objEnroll, objEvent)

                                        ' Email Requesting/Moderated User
                                        If Settings.SendEnrollMessageApproved Then
                                            objEventEmailInfo.UserIDs.Add(objEvent.OwnerID)
                                        End If
                                        objEventEmailInfo.TxtEmailBody = txtEmailMessage.Text & Settings.Templates.txtEnrollMessageApproved
                                        objEventEmail.SendEmails(objEventEmailInfo, objEvent, objEnroll)
                                    Case "Deny"
                                        DeleteEnrollment(CType(grdEnrollment.DataKeys(item.ItemIndex), Integer), objEvent.ModuleID, objEvent.EventID)

                                        ' Email Requesting/Moderated User
                                        If Settings.SendEnrollMessageDenied Then
                                            objEventEmailInfo.UserIDs.Add(objEvent.OwnerID)
                                        End If
                                        objEventEmailInfo.TxtEmailBody = txtEmailMessage.Text & Settings.Templates.txtEnrollMessageDenied
                                        objEventEmail.SendEmails(objEventEmailInfo, objEvent, objEnroll)
                                End Select
                            End If
                        Next
                End Select

            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
            BindData()
        End Sub

        Private Sub cmdSelectApproveAll_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles cmdSelectApproveAll.Click
            BindData()
            Dim item As DataGridItem
            If rbModerate.SelectedValue = "Events" Then
                For Each item In grdEvents.Items
                    CType(item.FindControl("rbEventAction"), RadioButtonList).SelectedValue = "Approve"
                Next
                For Each item In grdRecurEvents.Items
                    CType(item.FindControl("rbEventRecurAction"), RadioButtonList).SelectedValue = "Approve"
                Next
            Else
                For Each item In grdEnrollment.Items
                    CType(item.FindControl("rbEnrollAction"), RadioButtonList).SelectedValue = "Approve"
                Next
            End If
            cmdUpdateSelected_Click(sender, e)
        End Sub

        Private Sub cmdSelectDenyAll_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles cmdSelectDenyAll.Click
            BindData()
            Dim item As DataGridItem
            If rbModerate.SelectedValue = "Events" Then
                For Each item In grdEvents.Items
                    CType(item.FindControl("rbEventAction"), RadioButtonList).SelectedValue = "Deny"
                Next
                For Each item In grdRecurEvents.Items
                    CType(item.FindControl("rbEventRecurAction"), RadioButtonList).SelectedValue = "Deny"
                Next
            Else
                For Each item In grdEnrollment.Items
                    CType(item.FindControl("rbEnrollAction"), RadioButtonList).SelectedValue = "Deny"
                Next
            End If
            cmdUpdateSelected_Click(sender, e)
        End Sub

        Private Sub cmdUnmarkAll_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles cmdUnmarkAll.Click
            BindData()
            Dim item As DataGridItem
            If rbModerate.SelectedValue = "Events" Then
                For Each item In grdEvents.Items
                    CType(item.FindControl("rbEventAction"), RadioButtonList).SelectedValue = Nothing
                Next
                For Each item In grdRecurEvents.Items
                    CType(item.FindControl("rbEventRecurAction"), RadioButtonList).SelectedValue = Nothing
                Next
            Else
                For Each item In grdEnrollment.Items
                    CType(item.FindControl("rbEnrollAction"), RadioButtonList).SelectedValue = Nothing
                Next
            End If
        End Sub

#End Region

#Region "Grid and Other Events"
        Private Sub rbModerate_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles rbModerate.SelectedIndexChanged
            If rbModerate.SelectedValue = "Events" Then
                grdEnrollment.Visible = False
                grdEvents.Visible = True
                grdRecurEvents.Visible = True
            Else
                grdEnrollment.Visible = True
                grdEvents.Visible = False
            End If
            BindData()
        End Sub

        Public Sub grdEvents_ItemCommand(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs)
            Select Case e.CommandName
                Case "Select"
                    Dim itemID As Integer = CType(grdEvents.DataKeys(e.Item.ItemIndex), Integer)
                    Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
                    Response.Redirect(objEventInfoHelper.AddSkinContainerControls(NavigateURL(TabId, "Edit", "Mid=" & ModuleId.ToString, "ItemID=" & itemID.ToString(), "EditRecur=Single"), "?"))
            End Select
        End Sub

        Public Sub grdRecurEvents_ItemCommand(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs)
            Select Case e.CommandName
                Case "Select"
                    Dim itemID As Integer = CType(e.Item.Cells(4).Text, Integer)
                    Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
                    Response.Redirect(objEventInfoHelper.AddSkinContainerControls(NavigateURL(TabId, "Edit", "Mid=" & ModuleId.ToString, "ItemID=" & itemID.ToString(), "EditRecur=All"), "?"))
            End Select
        End Sub


        Public Sub grdEnrollment_ItemCommand(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs)
            Dim objEnroll As EventSignupsInfo
            objEnroll = _objCtlEventSignups.EventsSignupsGet(CType(grdEnrollment.DataKeys(e.Item.ItemIndex), Integer), ModuleId, False)

            Try
                Select Case e.CommandName
                    Case "Select"
                        Try
                            Dim itemID As Integer = objEnroll.EventID
                            Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
                            Response.Redirect(objEventInfoHelper.AddSkinContainerControls(EditUrl("ItemID", itemID.ToString(), "Edit"), "?"))
                        Catch ex As Exception
                        End Try
                    Case "User"
                        Dim objCtlEvent As New EventController
                        Dim objEvent As EventInfo = objCtlEvent.EventsGet(objEnroll.EventID, objEnroll.ModuleID)

                        Dim objEventEmailInfo As New EventEmailInfo
                        Dim objEventEmail As New EventEmails(PortalId, ModuleId, LocalResourceFile, CType(Page, PageBase).PageCulture.Name)
                        objEventEmailInfo.TxtEmailSubject = txtEmailSubject.Text
                        objEventEmailInfo.TxtEmailBody = txtEmailMessage.Text
                        objEventEmailInfo.TxtEmailFrom() = txtEmailFrom.Text
                        If objEnroll.UserID > -1 Then
                            objEventEmailInfo.UserIDs.Add(objEnroll.UserID)
                        Else
                            objEventEmailInfo.UserEmails.Add(objEnroll.AnonEmail)
                            objEventEmailInfo.UserLocales.Add(objEnroll.AnonCulture)
                            objEventEmailInfo.UserTimeZoneIds.Add(objEnroll.AnonTimeZoneId)
                        End If
                        objEventEmail.SendEmails(objEventEmailInfo, objEvent, objEnroll)

                End Select
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
            BindData()
        End Sub

        Private Sub returnButton_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles returnButton.Click
            Try
                Response.Redirect(GetSocialNavigateUrl(), True)
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

    End Class

End Namespace
