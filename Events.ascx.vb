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

Imports DotNetNuke.Common
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Security
Imports DotNetNuke.Services.Localization

Namespace DotNetNuke.Modules.Events

    <DNNtc.ModulePermission("EVENTS_MODULE", "EVENTSSET", "Edit Settings")> _
    <DNNtc.ModulePermission("EVENTS_MODULE", "EVENTSMOD", "Events Moderator")> _
    <DNNtc.ModulePermission("EVENTS_MODULE", "EVENTSEDT", "Events Editor")> _
    <DNNtc.ModulePermission("EVENTS_MODULE", "EVENTSCAT", "Global Category Editor")> _
    <DNNtc.ModulePermission("EVENTS_MODULE", "EVENTSLOC", "Global Location Editor")> _
    <DNNtc.ModuleDependencies(DNNtc.ModuleDependency.CoreVersion, "07.02.00")> _
    <DNNtc.ModuleControlProperties("", "Events Container", DNNtc.ControlType.View, "https://dnnevents.codeplex.com/documentation", True, False)> _
    Partial Class Events
        Inherits EventBase
        Implements Entities.Modules.IActionable

#Region "Private Members"
        Private _itemId As Integer
        Private _mcontrolToLoad As String = ""
        Private _socialGroupId As Integer = 0
        Private _socialUserId As Integer = 0
#End Region

#Region "Event Handlers"

        'This call is required by the Web Form Designer.
        <DebuggerStepThrough()> Private Shared Sub InitializeComponent()

        End Sub

        Private Shared Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load

            SetTheme(pnlEventsModule)
            AddFacebookMetaTags()
            LoadModuleControl() ' Load Module Control onto Page
        End Sub
#End Region

#Region "Private Methods "
        Private Sub LoadModuleControl()
            Try
                _socialGroupId = GetUrlGroupId()
                _socialUserId = GetUrlUserId()
                Dim objModules As New Entities.Modules.ModuleController
                Dim objModule As Entities.Modules.ModuleInfo = objModules.GetModule(ModuleId, TabId)
                Dim objDesktopModule As Entities.Modules.DesktopModuleInfo = Entities.Modules.DesktopModuleController.GetDesktopModule(objModule.DesktopModuleID, PortalId)
                Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)

                ' Force Module Default Settings on new Version or New Module Instance
                If objDesktopModule.Version <> Settings.Version Then
                    CreateThemeDirectory()
                    '                    objEventInfoHelper.SetDefaultModuleSettings(ModuleId, TabId, Page, LocalResourceFile)
                End If

                If Not Request.QueryString("mctl") = Nothing And _
                    (ModuleId = CType(Request.QueryString("ModuleID"), Integer) Or _
                    ModuleId = CType(Request.QueryString("mid"), Integer)) Then
                    If Request("mctl").EndsWith(".ascx") Then
                        _mcontrolToLoad = Request("mctl")
                    Else
                        _mcontrolToLoad = Request("mctl") & ".ascx"
                    End If
                End If

                ' Set Default, if none selected
                If _mcontrolToLoad.Length = 0 Then
                    If Not Request.Cookies.Get("DNNEvents" & ModuleId) Is Nothing Then
                        _mcontrolToLoad = Request.Cookies.Get("DNNEvents" & ModuleId).Value
                    Else
                        ' See if Default View Set
                        _mcontrolToLoad = Settings.DefaultView
                    End If
                End If

                ' Check for Valid Module to Load
                _mcontrolToLoad = IO.Path.GetFileNameWithoutExtension(_mcontrolToLoad) + ".ascx"
                Select Case _mcontrolToLoad.ToLower
                    Case "eventdetails.ascx"
                        ' Search and RSS feed may direct detail page url to base module - 
                        ' should be put in new page
                        If Settings.Eventdetailnewpage Then
                            'Get the item id of the selected event
                            If Not (Request.Params("ItemId") Is Nothing) Then
                                _itemId = Int32.Parse(Request.Params("ItemId"))
                                Dim objCtlEvents As New EventController
                                Dim objEvent As EventInfo = objCtlEvents.EventsGet(_itemId, ModuleId)
                                If Not objEvent Is Nothing Then
                                    Response.Redirect(objEventInfoHelper.GetDetailPageRealURL(objEvent.EventID, _socialGroupId, _socialUserId))
                                End If
                            End If
                        End If
                    Case "eventday.ascx"
                    Case "eventmonth.ascx"
                    Case "eventweek.ascx"
                    Case "eventrpt.ascx"
                        If Settings.ListViewGrid Then
                            _mcontrolToLoad = "EventList.ascx"
                        End If
                    Case "eventlist.ascx"
                        If Not Settings.ListViewGrid Then
                            _mcontrolToLoad = "EventRpt.ascx"
                        End If
                    Case "eventmoderate.ascx"
                        Response.Redirect(objEventInfoHelper.AddSkinContainerControls(NavigateURL(TabId, "Moderate", "Mid=" & ModuleId.ToString), "?"))
                    Case "eventmyenrollments.ascx"
                    Case Else
                        lblModuleSettings.Text = Localization.GetString("lblBadControl", LocalResourceFile)
                        lblModuleSettings.Visible = True
                        Exit Sub
                End Select

                Dim objPortalModuleBase As Entities.Modules.PortalModuleBase = CType(LoadControl(_mcontrolToLoad), Entities.Modules.PortalModuleBase)
                objPortalModuleBase.ModuleConfiguration = ModuleConfiguration.Clone
                objPortalModuleBase.ID = IO.Path.GetFileNameWithoutExtension(_mcontrolToLoad)
                phMain.Controls.Add(objPortalModuleBase)

                'EVT-4499 Exlude the EventMyEnrollment.ascx to be set as cookie
                If _mcontrolToLoad.ToLower <> "eventdetails.ascx" _
                   And _mcontrolToLoad.ToLower <> "eventday.ascx" _
                   And _mcontrolToLoad.ToLower <> "eventmyenrollments.ascx" Then
                    Dim objCookie As HttpCookie = New HttpCookie("DNNEvents" & ModuleId)
                    objCookie.Value = _mcontrolToLoad
                    If Request.Cookies.Get("DNNEvents" & ModuleId) Is Nothing Then
                        Response.Cookies.Add(objCookie)
                    Else
                        Response.Cookies.Set(objCookie)
                    End If
                End If

            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub
#End Region

#Region "Optional Interfaces"
        Public ReadOnly Property ModuleActions() As Entities.Modules.Actions.ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
            Get
                _socialGroupId = GetUrlGroupId()
                _socialUserId = GetUrlUserId()
                ' ReSharper disable LocalVariableHidesMember
                ' ReSharper disable InconsistentNaming
                Dim Actions As New Entities.Modules.Actions.ModuleActionCollection
                ' ReSharper restore InconsistentNaming
                ' ReSharper restore LocalVariableHidesMember
                Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
                Dim securityLevel As SecurityAccessLevel = SecurityAccessLevel.View

                Try
                    If PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString) Then
                        securityLevel = SecurityAccessLevel.Admin
                    End If

                    ' Add Event 
                    If IsModuleEditor() Then
                        If _socialGroupId > 0 Then
                            Actions.Add(GetNextActionID(), Localization.GetString("MenuAddEvents", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/cal-add.gif", objEventInfoHelper.AddSkinContainerControls(EditUrl("groupid", _socialGroupId.ToString, "Edit"), "?"), False, securityLevel, True, False)
                        ElseIf _socialUserId > 0 Then
                            If _socialUserId = UserId Or IsModerator() Then
                                Actions.Add(GetNextActionID(), Localization.GetString("MenuAddEvents", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/cal-add.gif", objEventInfoHelper.AddSkinContainerControls(EditUrl("userid", _socialUserId.ToString, "Edit"), "?"), False, securityLevel, True, False)
                            End If
                        Else
                            Actions.Add(GetNextActionID(), Localization.GetString("MenuAddEvents", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/cal-add.gif", objEventInfoHelper.AddSkinContainerControls(EditUrl("Edit"), "?"), False, securityLevel, True, False)
                        End If
                    End If

                    If Not Request.QueryString("mctl") = Nothing And ModuleId = CType(Request.QueryString("ModuleID"), Integer) Then
                        If Request("mctl").EndsWith(".ascx") Then
                            _mcontrolToLoad = Request("mctl")
                        Else
                            _mcontrolToLoad = Request("mctl") & ".ascx"
                        End If
                    End If

                    ' Set Default, if none selected
                    If _mcontrolToLoad.Length = 0 Then
                        If Not Request.Cookies.Get("DNNEvents" & ModuleId) Is Nothing Then
                            _mcontrolToLoad = Request.Cookies.Get("DNNEvents" & ModuleId).Value
                        Else
                            ' See if Default View Set
                            _mcontrolToLoad = Settings.DefaultView
                        End If
                    End If

                    'Add Month and Week Views
                    If Settings.MonthAllowed And _mcontrolToLoad <> "EventMonth.ascx" Then
                        If _socialGroupId > 0 Then
                            Actions.Add(GetNextActionID, Localization.GetString("MenuMonth", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/cal-month.gif", NavigateURL(TabId, "", "ModuleID=" & ModuleId.ToString, "mctl=EventMonth", "groupid=" & _socialGroupId.ToString), False, securityLevel, True, False)
                        ElseIf _socialUserId > 0 Then
                            Actions.Add(GetNextActionID, Localization.GetString("MenuMonth", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/cal-month.gif", NavigateURL(TabId, "", "ModuleID=" & ModuleId.ToString, "mctl=EventMonth", "userid=" & _socialUserId.ToString), False, securityLevel, True, False)
                        Else
                            Actions.Add(GetNextActionID, Localization.GetString("MenuMonth", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/cal-month.gif", NavigateURL(TabId, "", "ModuleID=" & ModuleId.ToString, "mctl=EventMonth"), False, securityLevel, True, False)
                        End If
                    End If
                    If Settings.WeekAllowed And _mcontrolToLoad <> "EventWeek.ascx" Then
                        If _socialGroupId > 0 Then
                            Actions.Add(GetNextActionID, Localization.GetString("MenuWeek", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/cal-week.gif", NavigateURL(TabId, "", "ModuleID=" & ModuleId.ToString, "mctl=EventWeek", "groupid=" & _socialGroupId.ToString), False, securityLevel, True, False)
                        ElseIf _socialUserId > 0 Then
                            Actions.Add(GetNextActionID, Localization.GetString("MenuWeek", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/cal-week.gif", NavigateURL(TabId, "", "ModuleID=" & ModuleId.ToString, "mctl=EventWeek", "userid=" & _socialUserId.ToString), False, securityLevel, True, False)
                        Else
                            Actions.Add(GetNextActionID, Localization.GetString("MenuWeek", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/cal-week.gif", NavigateURL(TabId, "", "ModuleID=" & ModuleId.ToString, "mctl=EventWeek"), False, securityLevel, True, False)
                        End If
                    End If
                    If Settings.ListAllowed And _mcontrolToLoad <> "EventList.ascx" Then
                        If _socialGroupId > 0 Then
                            Actions.Add(GetNextActionID, Localization.GetString("MenuList", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/cal-list.gif", NavigateURL(TabId, "", "ModuleID=" & ModuleId.ToString, "mctl=EventList", "groupid=" & _socialGroupId.ToString), False, securityLevel, True, False)
                        ElseIf _socialUserId > 0 Then
                            Actions.Add(GetNextActionID, Localization.GetString("MenuList", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/cal-list.gif", NavigateURL(TabId, "", "ModuleID=" & ModuleId.ToString, "mctl=EventList", "userid=" & _socialUserId.ToString), False, securityLevel, True, False)
                        Else
                            Actions.Add(GetNextActionID, Localization.GetString("MenuList", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/cal-list.gif", NavigateURL(TabId, "", "ModuleID=" & ModuleId.ToString, "mctl=EventList"), False, securityLevel, True, False)
                        End If
                    End If


                    ' See if Enrollments 
                    If Settings.Eventsignup Then
                        If _socialGroupId > 0 Then
                            Actions.Add(GetNextActionID, Localization.GetString("MenuMyEnrollments", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/cal-enroll.gif", NavigateURL(TabId, "", "ModuleID=" & ModuleId.ToString, "mctl=EventMyEnrollments", "groupid=" & _socialGroupId.ToString), False, securityLevel, True, False)
                        ElseIf _socialUserId > 0 Then
                            Actions.Add(GetNextActionID, Localization.GetString("MenuMyEnrollments", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/cal-enroll.gif", NavigateURL(TabId, "", "ModuleID=" & ModuleId.ToString, "mctl=EventMyEnrollments", "userid=" & _socialUserId.ToString), False, securityLevel, True, False)
                        Else
                            Actions.Add(GetNextActionID, Localization.GetString("MenuMyEnrollments", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/cal-enroll.gif", NavigateURL(TabId, "", "ModuleID=" & ModuleId.ToString, "mctl=EventMyEnrollments"), False, securityLevel, True, False)
                        End If
                    End If

                    If IsModerator() And Settings.Moderateall Then
                        If _socialGroupId > 0 Then
                            Actions.Add(GetNextActionID(), Localization.GetString("MenuModerate", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/moderate.gif", objEventInfoHelper.AddSkinContainerControls(EditUrl("groupid", _socialGroupId.ToString, "Moderate"), "?"), False, securityLevel, True, False)
                        ElseIf _socialUserId > 0 Then
                            Actions.Add(GetNextActionID(), Localization.GetString("MenuModerate", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/moderate.gif", objEventInfoHelper.AddSkinContainerControls(EditUrl("userid", _socialUserId.ToString, "Moderate"), "?"), False, securityLevel, True, False)
                        Else
                            Actions.Add(GetNextActionID(), Localization.GetString("MenuModerate", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/moderate.gif", objEventInfoHelper.AddSkinContainerControls(EditUrl("Moderate"), "?"), False, securityLevel, True, False)
                        End If
                    End If
                    If IsSettingsEditor() Then
                        If _socialGroupId > 0 Then
                            Actions.Add(GetNextActionID(), Localization.GetString("MenuSettings", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", Entities.Icons.IconController.IconURL("EditTab"), objEventInfoHelper.AddSkinContainerControls(EditUrl("groupid", _socialGroupId.ToString, "EventSettings"), "?"), False, securityLevel, True, False)
                        ElseIf _socialUserId > 0 Then
                            Actions.Add(GetNextActionID(), Localization.GetString("MenuSettings", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", Entities.Icons.IconController.IconURL("EditTab"), objEventInfoHelper.AddSkinContainerControls(EditUrl("userid", _socialUserId.ToString, "EventSettings"), "?"), False, securityLevel, True, False)
                        Else
                            Actions.Add(GetNextActionID(), Localization.GetString("MenuSettings", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", Entities.Icons.IconController.IconURL("EditTab"), objEventInfoHelper.AddSkinContainerControls(EditUrl("EventSettings"), "?"), False, securityLevel, True, False)
                        End If
                    End If
                    If IsCategoryEditor() Then
                        If _socialGroupId > 0 Then
                            Actions.Add(GetNextActionID(), Localization.GetString("MenuCategories", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/Categories.gif", objEventInfoHelper.AddSkinContainerControls(EditUrl("groupid", _socialGroupId.ToString, "Categories"), "?"), False, securityLevel, True, False)
                        ElseIf _socialUserId > 0 Then
                            Actions.Add(GetNextActionID(), Localization.GetString("MenuCategories", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/Categories.gif", objEventInfoHelper.AddSkinContainerControls(EditUrl("userid", _socialUserId.ToString, "Categories"), "?"), False, securityLevel, True, False)
                        Else
                            Actions.Add(GetNextActionID(), Localization.GetString("MenuCategories", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/Categories.gif", objEventInfoHelper.AddSkinContainerControls(EditUrl("Categories"), "?"), False, securityLevel, True, False)
                        End If
                    End If
                    If IsLocationEditor() Then
                        If _socialGroupId > 0 Then
                            Actions.Add(GetNextActionID(), Localization.GetString("MenuLocations", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/Locations.gif", objEventInfoHelper.AddSkinContainerControls(EditUrl("groupid", _socialGroupId.ToString, "Locations"), "?"), False, securityLevel, True, False)
                        ElseIf _socialUserId > 0 Then
                            Actions.Add(GetNextActionID(), Localization.GetString("MenuLocations", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/Locations.gif", objEventInfoHelper.AddSkinContainerControls(EditUrl("userid", _socialUserId.ToString, "Locations"), "?"), False, securityLevel, True, False)
                        Else
                            Actions.Add(GetNextActionID(), Localization.GetString("MenuLocations", LocalResourceFile), Entities.Modules.Actions.ModuleActionType.ContentOptions, "", "../DesktopModules/Events/Images/Locations.gif", objEventInfoHelper.AddSkinContainerControls(EditUrl("Locations"), "?"), False, securityLevel, True, False)
                        End If
                    End If

                Catch exc As Exception
                    'ProcessModuleLoadException(Me, exc)
                End Try
                Return Actions
            End Get
        End Property
#End Region

    End Class

End Namespace
