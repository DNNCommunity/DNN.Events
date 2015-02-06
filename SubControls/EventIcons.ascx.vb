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


Namespace DotNetNuke.Modules.Events

    Partial Class EventIcons
        Inherits EventBase
        Private ReadOnly _myFileName As String = Me.GetType().BaseType.Name + ".ascx"

        Protected Overloads ReadOnly Property LocalResourceFile() As String
            Get
                Return Localization.GetResourceFile(Me, _myFileName)
            End Get
        End Property



#Region "Event Handlers"
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
            Try

                btnMonth.Visible = False
                If Settings.MonthAllowed And Parent.ID.ToLower <> "eventmonth" Then
                    btnMonth.Visible = True
                    btnMonth.AlternateText = Localization.GetString("MenuMonth", LocalResourceFile)
                    btnMonth.ToolTip = Localization.GetString("MenuMonth", LocalResourceFile)
                End If
                btnWeek.Visible = False
                If Settings.WeekAllowed And Parent.ID.ToLower <> "eventweek" Then
                    btnWeek.Visible = True
                    btnWeek.AlternateText = Localization.GetString("MenuWeek", LocalResourceFile)
                    btnWeek.ToolTip = Localization.GetString("MenuWeek", LocalResourceFile)
                End If
                btnList.Visible = False
                If Settings.ListAllowed And Parent.ID.ToLower <> "eventlist" And Parent.ID.ToLower <> "eventrpt" Then
                    btnList.Visible = True
                    btnList.AlternateText = Localization.GetString("MenuList", LocalResourceFile)
                    btnList.ToolTip = Localization.GetString("MenuList", LocalResourceFile)
                End If
                btnEnroll.Visible = False
                If Settings.eventsignup And Parent.ID.ToLower <> "eventmyenrollments" Then
                    btnEnroll.Visible = True
                    btnEnroll.AlternateText = Localization.GetString("MenuMyEnrollments", LocalResourceFile)
                    btnEnroll.ToolTip = Localization.GetString("MenuMyEnrollments", LocalResourceFile)
                End If

                Dim socialGroupId As Integer = GetUrlGroupId()
                Dim groupStr As String = ""
                If socialGroupId > 0 Then
                    groupStr = "&GroupId=" & socialGroupId.ToString
                End If
                Dim socialUserId As Integer = GetUrlUserId()

                hypiCal.Visible = Settings.IcalOnIconBar
                hypiCal.ToolTip = Localization.GetString("MenuiCal", LocalResourceFile)
                hypiCal.NavigateUrl = "~/DesktopModules/Events/EventVCal.aspx?ItemID=0&Mid=" & ModuleId & "&tabid=" & TabId & groupStr

                btnRSS.Visible = Settings.RSSEnable
                btnRSS.ToolTip = Localization.GetString("MenuRSS", LocalResourceFile)
                btnRSS.NavigateUrl = "~/DesktopModules/Events/EventRSS.aspx?mid=" & ModuleId & "&tabid=" & TabId & groupStr
                btnRSS.Target = "_blank"

                btnAdd.Visible = False
                btnModerate.Visible = False
                btnSettings.Visible = False
                btnSubscribe.Visible = False
                lblSubscribe.Visible = False
                imgBar.Visible = False
                If Request.IsAuthenticated() Then
                    If IsModuleEditor() Then
                        btnAdd.ToolTip = Localization.GetString("MenuAddEvents", LocalResourceFile)
                        Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
                        If socialGroupId > 0 Then
                            btnAdd.NavigateUrl = objEventInfoHelper.AddSkinContainerControls(EditUrl("groupid", socialGroupId.ToString, "Edit"), "?")
                            btnAdd.Visible = True
                        ElseIf socialUserId > 0 Then
                            If socialUserId = UserId Or IsModerator() Then
                                btnAdd.NavigateUrl = objEventInfoHelper.AddSkinContainerControls(EditUrl("Userid", socialUserId.ToString, "Edit"), "?")
                                btnAdd.Visible = True
                            End If
                        Else
                            btnAdd.NavigateUrl = objEventInfoHelper.AddSkinContainerControls(EditUrl("Edit"), "?")
                            btnAdd.Visible = True
                        End If
                    End If
                    If Settings.moderateall And (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString) Or IsModerator()) Then
                        btnModerate.Visible = True
                        btnModerate.AlternateText = Localization.GetString("MenuModerate", LocalResourceFile)
                        btnModerate.ToolTip = Localization.GetString("MenuModerate", LocalResourceFile)
                    End If
                    If PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString) Or IsSettingsEditor() Then
                        btnSettings.Visible = True
                        btnSettings.ToolTip = Localization.GetString("MenuSettings", LocalResourceFile)
                        Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
                        If socialGroupId > 0 Then
                            btnSettings.NavigateUrl = objEventInfoHelper.AddSkinContainerControls(EditUrl("groupid", socialGroupId.ToString, "EventSettings"), "?")
                        ElseIf socialUserId > 0 Then
                            btnSettings.NavigateUrl = objEventInfoHelper.AddSkinContainerControls(EditUrl("userid", socialUserId.ToString, "EventSettings"), "?")
                        Else
                            btnSettings.NavigateUrl = objEventInfoHelper.AddSkinContainerControls(EditUrl("EventSettings"), "?")
                        End If
                    End If
                    If Settings.neweventemails = "Subscribe" Then
                        btnSubscribe.Visible = True
                        lblSubscribe.Visible = True
                        imgBar.Visible = True
                        Dim objEventSubscriptionController As New EventSubscriptionController
                        Dim objEventSubscription As EventSubscriptionInfo = objEventSubscriptionController.EventsSubscriptionGetUser(UserId, ModuleId)
                        If IsNothing(objEventSubscription) Then
                            lblSubscribe.Text = Localization.GetString("lblSubscribe", LocalResourceFile)
                            btnSubscribe.AlternateText = Localization.GetString("MenuSubscribe", LocalResourceFile)
                            btnSubscribe.ToolTip = Localization.GetString("MenuTTSubscribe", LocalResourceFile)
                            btnSubscribe.ImageUrl = Entities.Icons.IconController.IconURL("Unchecked")
                        Else
                            lblSubscribe.Text = Localization.GetString("lblUnsubscribe", LocalResourceFile)
                            btnSubscribe.AlternateText = Localization.GetString("MenuUnsubscribe", LocalResourceFile)
                            btnSubscribe.ToolTip = Localization.GetString("MenuTTUnsubscribe", LocalResourceFile)
                            btnSubscribe.ImageUrl = Entities.Icons.IconController.IconURL("Checked")
                        End If
                    End If
                End If
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
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

#Region "Links and Buttons"

        Private Sub btnMonth_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs) Handles btnMonth.Click
            Response.Redirect(CreateNavigateURL("EventMonth"))
        End Sub

        Private Sub btnEnroll_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs) Handles btnEnroll.Click
            Response.Redirect(CreateNavigateURL("EventMyEnrollments"))
        End Sub

        Private Sub btnList_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs) Handles btnList.Click
            Response.Redirect(CreateNavigateURL("EventList"))
        End Sub

        Private Sub btnWeek_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs) Handles btnWeek.Click
            Response.Redirect(CreateNavigateURL("EventWeek"))
        End Sub

        Private Sub btnModerate_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs) Handles btnModerate.Click
            Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
            Dim socialGroupId As Integer = GetUrlGroupId()
            Dim socialUserId As Integer = GetUrlUserId()
            If socialGroupId > 0 Then
                Response.Redirect(objEventInfoHelper.AddSkinContainerControls(EditUrl("groupid", socialGroupId.ToString, "Moderate"), "?"))
            ElseIf socialUserId > 0 Then
                Response.Redirect(objEventInfoHelper.AddSkinContainerControls(EditUrl("userid", socialUserId.ToString, "Moderate"), "?"))
            Else
                Response.Redirect(objEventInfoHelper.AddSkinContainerControls(EditUrl("Moderate"), "?"))
            End If
        End Sub

        Private Sub btnSubscribe_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs) Handles btnSubscribe.Click
            Dim objEventSubscriptionController As New EventSubscriptionController
            If btnSubscribe.ImageUrl = Entities.Icons.IconController.IconURL("Unchecked") Then
                Dim objEventSubscription As New EventSubscriptionInfo
                objEventSubscription.SubscriptionID = -1
                objEventSubscription.ModuleID = ModuleId
                objEventSubscription.PortalID = PortalId
                objEventSubscription.UserID = UserId
                objEventSubscriptionController.EventsSubscriptionSave(objEventSubscription)
                btnSubscribe.Visible = True
                lblSubscribe.Text = Localization.GetString("lblUnsubscribe", LocalResourceFile)
                btnSubscribe.AlternateText = Localization.GetString("MenuUnsubscribe", LocalResourceFile)
                btnSubscribe.ToolTip = Localization.GetString("MenuTTUnsubscribe", LocalResourceFile)
                btnSubscribe.ImageUrl = Entities.Icons.IconController.IconURL("Checked")
            Else
                objEventSubscriptionController.EventsSubscriptionDeleteUser(UserId, ModuleId)
                btnSubscribe.Visible = True
                lblSubscribe.Text = Localization.GetString("lblSubscribe", LocalResourceFile)
                btnSubscribe.AlternateText = Localization.GetString("MenuSubscribe", LocalResourceFile)
                btnSubscribe.ToolTip = Localization.GetString("MenuTTSubscribe", LocalResourceFile)
                btnSubscribe.ImageUrl = Entities.Icons.IconController.IconURL("Unchecked")
            End If

        End Sub

#End Region

#Region "Helper Routines"
        Private Function CreateNavigateURL(ByVal mctl As String) As String
            Dim socialGroupId As Integer = GetUrlGroupId()
            Dim socialUserId As Integer = GetUrlUserId()
            If socialGroupId > 0 Then
                Return NavigateURL(TabId, "", "ModuleID=" & ModuleId.ToString, "mctl=" & mctl, "groupid=" & socialGroupId.ToString)
            ElseIf socialUserId > 0 Then
                Return NavigateURL(TabId, "", "ModuleID=" & ModuleId.ToString, "mctl=" & mctl, "userid=" & socialUserId.ToString)
            Else
                Return NavigateURL(TabId, "", "ModuleID=" & ModuleId.ToString, "mctl=" & mctl)
            End If
        End Function
#End Region
    End Class

End Namespace
