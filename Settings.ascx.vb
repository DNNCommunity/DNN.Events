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
Imports DotNetNuke.Security.Permissions
Namespace DotNetNuke.Modules.Events

    <DNNtc.ModuleControlProperties("Settings", "Event Settings", DNNtc.ControlType.Admin, "https://dnnevents.codeplex.com/documentation", True, True)> _
    Partial Class Settings
        Inherits Entities.Modules.ModuleSettingsBase

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

#Region "Private Data"
#End Region

#Region "Help Methods"
        ''' <summary>
        ''' Load current settings into the controls from the modulesettings
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub LoadSettings()
            ' Force full PostBack since these pass off to aspx page
            If AJAX.IsInstalled Then
                AJAX.RegisterPostBackControl(cmdUpgrade)
            End If

            Dim ems As New EventModuleSettings
            Dim emSettings As EventModuleSettings = ems.GetEventModuleSettings(ModuleId, LocalResourceFile)

            Dim dummyRmid As String = emSettings.RecurDummy
            divUpgrade.Visible = False
            divRetry.Visible = False
            If Not dummyRmid Is Nothing And _
                   dummyRmid <> "99999" Then
                divUpgrade.Visible = True
            End If


        End Sub

        ''' <summary>
        ''' Take all settings and write them back to the database
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub UpdateSettings()
            Try

                MakeModerator_Editor()
                UpdateSubscriptions()
                Dim emCacheKey As String = "EventsModuleTitle" & ModuleId.ToString
                Common.Utilities.DataCache.ClearCache(emCacheKey)

            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub MakeModerator_Editor()
            Try
                Dim blEditor As Boolean
                Dim arrRoles As New ArrayList
                Dim arrUsers As New ArrayList

                Dim objPermission As Security.Permissions.ModulePermissionInfo
                Dim objPermissionController As New Security.Permissions.PermissionController

                Dim objModules As New Entities.Modules.ModuleController
                ' Get existing module permissions
                Dim objModule As Entities.Modules.ModuleInfo = objModules.GetModule(ModuleId, TabId)

                Dim objModulePermissions2 As New Security.Permissions.ModulePermissionCollection
                For Each perm As ModulePermissionInfo In objModule.ModulePermissions
                    If (perm.PermissionKey = "EVENTSMOD" And perm.AllowAccess) Or (perm.PermissionKey = "EDIT" And perm.AllowAccess) Then
                        blEditor = False
                        For Each perm2 As ModulePermissionInfo In objModule.ModulePermissions
                            If perm2.PermissionKey = "EVENTSEDT" And ((perm.RoleID = perm2.RoleID And perm.RoleID >= 0) Or (perm.UserID = perm2.UserID And perm.UserID >= 0)) Then
                                If perm2.AllowAccess Then
                                    blEditor = True
                                Else
                                    objModulePermissions2.Add(perm2)
                                End If
                            End If
                        Next
                        If blEditor = False Then
                            If perm.UserID >= 0 Then
                                arrUsers.Add(perm.UserID)
                            Else
                                arrRoles.Add(perm.RoleID)
                            End If
                        End If
                    End If
                Next

                ' Remove negative edit permissions where user is moderator
                For Each perm As ModulePermissionInfo In objModulePermissions2
                    objModule.ModulePermissions.Remove(perm)
                Next

                Dim objEditPermissions As ArrayList = objPermissionController.GetPermissionByCodeAndKey("EVENTS_MODULE", "EVENTSEDT")
                Dim objEditPermission As PermissionInfo = CType(objEditPermissions.Item(0), PermissionInfo)

                For Each iRoleID As Integer In arrRoles
                    ' Add Edit Permission for Moderator Role
                    objPermission = New Security.Permissions.ModulePermissionInfo
                    objPermission.RoleID = iRoleID
                    objPermission.ModuleID = ModuleId
                    objPermission.PermissionKey = objEditPermission.PermissionKey
                    objPermission.PermissionName = objEditPermission.PermissionName
                    objPermission.PermissionCode = objEditPermission.PermissionCode
                    objPermission.PermissionID = objEditPermission.PermissionID
                    objPermission.AllowAccess = True
                    objModule.ModulePermissions.Add(objPermission)
                Next
                For Each iUserID As Integer In arrUsers
                    objPermission = New Security.Permissions.ModulePermissionInfo
                    objPermission.UserID = iUserID
                    objPermission.ModuleID = ModuleId
                    objPermission.PermissionKey = objEditPermission.PermissionKey
                    objPermission.PermissionName = objEditPermission.PermissionName
                    objPermission.PermissionCode = objEditPermission.PermissionCode
                    objPermission.PermissionID = objEditPermission.PermissionID
                    objPermission.AllowAccess = True
                    objModule.ModulePermissions.Add(objPermission)
                Next
                ModulePermissionController.SaveModulePermissions(objModule)

            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub UpdateSubscriptions()

            Dim objCtlEventSubscriptions As New EventSubscriptionController
            Dim lstEventSubscriptions As ArrayList
            lstEventSubscriptions = objCtlEventSubscriptions.EventsSubscriptionGetModule(ModuleId)
            If lstEventSubscriptions.Count = 0 Then
                Return
            End If

            Dim objEventInfo As New EventInfoHelper(ModuleId, TabId, PortalId, Nothing)
            Dim lstusers As ArrayList = objEventInfo.GetEventModuleViewers

            Dim objEventSubscription As EventSubscriptionInfo
            For Each objEventSubscription In lstEventSubscriptions
                If Not lstusers.Contains(objEventSubscription.UserID) Then
                    Dim objCtlUser As New UserController
                    Dim objUser As UserInfo = objCtlUser.GetUser(PortalId, objEventSubscription.UserID)

                    If objUser Is Nothing Or Not objUser.IsSuperUser Then
                        objCtlEventSubscriptions.EventsSubscriptionDeleteUser(objEventSubscription.UserID, ModuleId)
                    End If
                End If
            Next
        End Sub

#End Region

#Region "Links, Buttons and Events"
        Private Sub cmdUpgrade_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpgrade.Click
            Dim ems As New EventModuleSettings
            Dim emSettings As EventModuleSettings = ems.GetEventModuleSettings(ModuleId, LocalResourceFile)

            Dim dummyRmid As String = emSettings.RecurDummy
            divUpgrade.Visible = False
            divRetry.Visible = False
            If Not dummyRmid Is Nothing And _
                   dummyRmid <> "99999" Then
                Dim objEventController As New EventController
                Dim upgradeOk As Boolean = objEventController.UpgradeRecurringEventModule(ModuleId, CInt(dummyRmid), emSettings.Maxrecurrences, LocalResourceFile)
                Dim objEventCtl As New EventController
                objEventCtl.EventsUpgrade("04.01.00")
                If Not upgradeOk Then
                    divUpgrade.Visible = True
                    divRetry.Visible = True
                End If
            End If
        End Sub

#End Region

    End Class

End Namespace
