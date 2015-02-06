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


Namespace DotNetNuke.Modules.Events

    Partial Class EventMyEnrollments
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
#End Region

#Region "Event Handlers"
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
            Try
                'EVT-4499 if not login, redirect user to login page
                If Not Request.IsAuthenticated Then
                    RedirectToLogin()
                End If

                LocalizeAll()

                ' Setup Icon Bar for use
                SetUpIconBar(EventIcons, EventIcons2)

                lnkSelectedDelete.Attributes.Add("onclick", "javascript:return confirm('" + Localization.GetString("ConfirmDeleteSelected", LocalResourceFile) + "');")

                If Page.IsPostBack = False Then
                    BindData()
                End If

            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub
#End Region

#Region "Helper Methods"
        Private Sub BindData()
            Try
                Dim moduleStartDate As DateTime = DateAdd(DateInterval.Day, -Settings.EnrolListDaysBefore, ModuleNow)
                Dim moduleEndDate As DateTime = DateAdd(DateInterval.Day, Settings.EnrolListDaysAfter, ModuleNow)
                Dim displayStartDate As DateTime = DateAdd(DateInterval.Day, -Settings.EnrolListDaysBefore, DisplayNow)
                Dim displayEndDate As DateTime = DateAdd(DateInterval.Day, Settings.EnrolListDaysAfter, DisplayNow)

                'Default sort from settings
                Dim sortDirection As SortDirection = Settings.EnrolListSortDirection
                Dim sortExpression As EventSignupsInfo.SortFilter = GetSignupsSortExpression("EventTimeBegin")

                Dim inCategoryIDs As New ArrayList
                inCategoryIDs.Add("-1")
                Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
                Dim categoryIDs As String = objEventInfoHelper.CreateCategoryFilter(inCategoryIDs)

                Dim eventSignups As ArrayList
                Dim objCtlEventSignups As New EventSignupsController

                eventSignups = objCtlEventSignups.EventsSignupsMyEnrollments(ModuleId, UserId, GetUrlGroupId, categoryIDs, moduleStartDate, moduleEndDate)

                Dim objEventTimeZoneUtilities As New EventTimeZoneUtilities
                Dim displayEventSignups As New ArrayList
                For Each eventSignup As EventSignupsInfo In eventSignups
                    Dim displayTimeZoneId As String = GetDisplayTimeZoneId()
                    eventSignup.EventTimeBegin = objEventTimeZoneUtilities.ConvertToDisplayTimeZone(eventSignup.EventTimeBegin, eventSignup.EventTimeZoneId, PortalId, displayTimeZoneId).EventDate
                    eventSignup.EventTimeEnd = objEventTimeZoneUtilities.ConvertToDisplayTimeZone(eventSignup.EventTimeEnd, eventSignup.EventTimeZoneId, PortalId, displayTimeZoneId).EventDate
                    If eventSignup.EventTimeBegin > displayEndDate Or eventSignup.EventTimeEnd < displayStartDate Then
                        Continue For
                    End If
                    displayEventSignups.Add(eventSignup)
                Next

                EventSignupsInfo.SortExpression = sortExpression
                EventSignupsInfo.SortDirection = sortDirection
                displayEventSignups.Sort()

                'Get data for selected date and fill grid
                grdEnrollment.DataSource = displayEventSignups
                grdEnrollment.DataBind()
                If eventSignups.Count < 1 Then
                    divMessage.Visible = True
                    grdEnrollment.Visible = False
                    '"No Events/Enrollments found..."
                    lblMessage.Text = Localization.GetString("MsgNoMyEventsOrEnrollment", LocalResourceFile)
                Else
                    For i As Integer = 0 To eventSignups.Count - 1
                        Dim decTotal As Decimal = CType(grdEnrollment.Items(i).Cells(7).Text, Decimal) / CType(grdEnrollment.Items(i).Cells(8).Text, Decimal)
                        Dim dtStartTime As DateTime = CType(grdEnrollment.Items(i).Cells(1).Text, DateTime)
                        ' ReSharper disable LocalizableElement
                        CType(grdEnrollment.Items(i).FindControl("lblAmount"), Label).Text = String.Format("{0:F2}", decTotal) & " " & PortalSettings.Currency
                        CType(grdEnrollment.Items(i).FindControl("lblTotal"), Label).Text = String.Format("{0:F2}", CType(grdEnrollment.Items(i).Cells(7).Text, Decimal)) & " " & PortalSettings.Currency
                        ' ReSharper restore LocalizableElement
                        If decTotal > 0 Or dtStartTime < ModuleNow.AddDays(Settings.Enrolcanceldays) Then
                            CType(grdEnrollment.Items(i).FindControl("chkSelect"), CheckBox).Enabled = False
                        End If
                    Next
                End If
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub LocalizeAll()
            grdEnrollment.Columns(0).HeaderText = Localization.GetString("plSelect", LocalResourceFile)
            grdEnrollment.Columns(1).HeaderText = Localization.GetString("plDate", LocalResourceFile)
            grdEnrollment.Columns(2).HeaderText = Localization.GetString("plTime", LocalResourceFile)
            grdEnrollment.Columns(3).HeaderText = Localization.GetString("plEvent", LocalResourceFile)
            grdEnrollment.Columns(4).HeaderText = Localization.GetString("plApproved", LocalResourceFile)
            grdEnrollment.Columns(6).HeaderText = Localization.GetString("plAmount", LocalResourceFile)
            grdEnrollment.Columns(8).HeaderText = Localization.GetString("plNoEnrolees", LocalResourceFile)
            grdEnrollment.Columns(9).HeaderText = Localization.GetString("plTotal", LocalResourceFile)
            lnkSelectedDelete.ToolTip = String.Format(Localization.GetString("CancelEnrolments", LocalResourceFile), Settings.Enrolcanceldays)
        End Sub
#End Region

#Region "Grid and Other Events"

        Public Sub grdEnrollment_ItemCommand(ByVal source As System.Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles grdEnrollment.ItemCommand
            Try
                Select Case e.CommandName
                    Case "Select"
                        Dim objEnroll As EventSignupsInfo
                        Dim objCtlEventSignups As New EventSignupsController
                        objEnroll = objCtlEventSignups.EventsSignupsGet(CType(grdEnrollment.DataKeys(e.Item.ItemIndex), Integer), ModuleId, False)
                        Dim iItemID As Integer = objEnroll.EventID

                        Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
                        Response.Redirect(objEventInfoHelper.GetDetailPageRealURL(iItemID, GetUrlGroupId, GetUrlUserId))
                End Select
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
            BindData()
        End Sub

        Private Sub lnkSelectedDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkSelectedDelete.Click
            Dim item As DataGridItem
            Dim objEnroll As EventSignupsInfo
            Dim objCtlEventSignups As New EventSignupsController
            Dim objEvent As New EventInfo
            Dim objCtlEvent As New EventController
            Dim eventID As Integer = 0

            For Each item In grdEnrollment.Items
                If CType(item.FindControl("chkSelect"), CheckBox).Checked Then
                    objEnroll = objCtlEventSignups.EventsSignupsGet(CType(grdEnrollment.DataKeys(item.ItemIndex), Integer), ModuleId, False)
                    If eventID <> objEnroll.EventID Then
                        objEvent = objCtlEvent.EventsGet(objEnroll.EventID, ModuleId)
                    End If
                    eventID = objEnroll.EventID()

                    ' Delete Selected Enrollee?
                    DeleteEnrollment(CType(grdEnrollment.DataKeys(item.ItemIndex), Integer), objEvent.ModuleID, objEvent.EventID)

                    ' Mail users
                    If Settings.SendEnrollMessageDeleted Then
                        Dim objEventEmailInfo As New EventEmailInfo
                        Dim objEventEmail As New EventEmails(PortalId, ModuleId, LocalResourceFile, CType(Page, PageBase).PageCulture.Name)
                        objEventEmailInfo.TxtEmailSubject = Settings.Templates.txtEnrollMessageSubject
                        objEventEmailInfo.TxtEmailBody = Settings.Templates.txtEnrollMessageDeleted
                        objEventEmailInfo.TxtEmailFrom() = Settings.StandardEmail
                        objEventEmailInfo.UserEmails.Add(PortalSettings.UserInfo.Email)
                        objEventEmailInfo.UserLocales.Add(PortalSettings.UserInfo.Profile.PreferredLocale)
                        objEventEmailInfo.UserTimeZoneIds.Add(PortalSettings.UserInfo.Profile.PreferredTimeZone.Id)
                        objEventEmailInfo.UserIDs.Add(objEvent.OwnerID)
                        objEventEmail.SendEmails(objEventEmailInfo, objEvent, objEnroll)
                    End If

                End If
            Next

            BindData()

        End Sub

        Private Sub returnButton_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles returnButton.Click
            Dim cntrl As String() = Settings.DefaultView.Split("."c)
            Dim socialGroupId As Integer = GetUrlGroupId()
            If socialGroupId > 0 Then
                Response.Redirect(NavigateURL(TabId, "", "mctl=" & cntrl(0), "ModuleID=" & ModuleId.ToString, "groupid=" & socialGroupId.ToString))
            Else
                Response.Redirect(NavigateURL(TabId, "", "mctl=" & cntrl(0), "ModuleID=" & ModuleId.ToString))
            End If
        End Sub

#End Region

    End Class
End Namespace
