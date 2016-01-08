Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Security.Roles

Namespace DotNetNuke.Modules.Events
    <System.ComponentModel.DefaultEvent("Refreshed")> Partial Public Class EventUserGrid
        '  Inherits Framework.UserControlBase
        Inherits EventBase

        Private _users As ArrayList = New ArrayList
        Private ReadOnly _myFileName As String = Me.GetType().BaseType.Name + ".ascx"
        ' ReSharper disable EventNeverInvoked
        Public Event Refreshed(ByVal sender As Object, ByVal e As EventArgs)
        ' ReSharper restore EventNeverInvoked
        Public Event AddSelectedUsers(ByVal sender As Object, ByVal e As EventArgs, ByVal arrUsers As ArrayList)

        Protected Property Users() As ArrayList
            Get
                Return _users
            End Get
            Set(ByVal value As ArrayList)
                _users = Value
            End Set
        End Property

        Protected Overloads ReadOnly Property LocalResourceFile() As String
            Get
                Return Localization.GetResourceFile(Me, _myFileName)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Page Size for the Grid
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/02/2006  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property PageSize() As Integer
            Get
                Dim setting As Object = UserModuleBase.GetSetting(PortalId, "Records_PerPage")
                Return CType(setting, Integer)
            End Get
        End Property


        Protected ReadOnly Property ItemID() As Integer
            Get
                Return CType(Request.QueryString("ItemID"), Integer)
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            If Not Page.IsPostBack Then

            End If
        End Sub

        Private Sub Localize_Text()
            Dim localText As String
            localText = Localization.GetString("lblStartswith.Text", LocalResourceFile())
            If (Not String.IsNullOrEmpty(localText)) Then
                lblStartswith.Text = localText
            End If
            localText = Localization.GetString("cmdSelectedAddUser.Text", LocalResourceFile())
            If (Not String.IsNullOrEmpty(localText)) Then
                cmdSelectedAddUser.Text = localText
            End If
            localText = Localization.GetString("cmdRefreshList.Text", LocalResourceFile())
            If (Not String.IsNullOrEmpty(localText)) Then
                cmdRefreshList.Text = localText
            End If
            localText = Localization.GetString("Select.Header", LocalResourceFile())
            If (Not String.IsNullOrEmpty(localText)) Then
                gvUsersToEnroll.Columns.Item(0).HeaderText = localText
            End If
            localText = Localization.GetString("Username.Header", LocalResourceFile())
            If (Not String.IsNullOrEmpty(localText)) Then
                gvUsersToEnroll.Columns.Item(1).HeaderText = localText
            End If
            localText = Localization.GetString("Displayname.Header", LocalResourceFile())
            If (Not String.IsNullOrEmpty(localText)) Then
                gvUsersToEnroll.Columns.Item(2).HeaderText = localText
            End If
            localText = Localization.GetString("Emailaddress.Header", LocalResourceFile())
            If (Not String.IsNullOrEmpty(localText)) Then
                gvUsersToEnroll.Columns.Item(3).HeaderText = localText
            End If

        End Sub

        Public Sub RefreshGrid()
            Const csname As String = "ChangeScrip"
            Dim cstype As Type = System.Reflection.MethodBase.GetCurrentMethod().GetType()
            Dim cstext As String = "function ChangedropdownFilterItem(event) {" + vbCrLf + _
                                        "var DropDownFilterItem = document.getElementById('" + dropdownFilterItem.ClientID + "');" + _
                                        "var lblStartswith = document.getElementById('" + lblStartswith.ClientID + "');" + _
                                        "if (DropDownFilterItem.value =='1') lblStartswith.style.display = 'none';" + vbCrLf + _
                                        "else lblStartswith.style.display = '';" + vbCrLf + "}"

            If Not Page.ClientScript.IsClientScriptBlockRegistered(csname) Then
                Page.ClientScript.RegisterClientScriptBlock(cstype, csname, cstext, True)
            End If

            Localize_Text()
            BindData(txtFilterUsers.Text, dropdownFilterItem.Value)
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindData gets the users from the Database and binds them to the DataGrid
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="searchText">Text to Search</param>
        ''' <param name="searchField">Field to Search</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindData(ByVal searchText As String, ByVal searchField As String)

            gvUsersToEnroll.PageSize = PageSize

            Dim objCtlRole As New RoleController
            Dim objRole As RoleInfo = objCtlRole.GetRoleByName(PortalId, PortalSettings.RegisteredRoleName)
            Dim roleName As String = ""
            Dim regRoleName As String = ""
            If Not objRole Is Nothing Then
                roleName = objRole.RoleName
                regRoleName = roleName
            End If
            Dim ddEnrollRoles As DropDownList = CType(Parent.FindControl("ddEnrollRoles"), DropDownList)
            If ddEnrollRoles.SelectedValue <> "-1" Then
                roleName = ddEnrollRoles.SelectedItem.Text
            End If

            dropdownFilterItem.Items.Clear()
            dropdownFilterItem.Items.Add(New ListItem(Localization.GetString("dropdownFilterItem00.Text", LocalResourceFile()), "0"))
            dropdownFilterItem.Items.Add(New ListItem(Localization.GetString("dropdownFilterItem02.Text", LocalResourceFile()), "2"))
            If roleName = regRoleName Then
                dropdownFilterItem.Items.Add(New ListItem(Localization.GetString("dropdownFilterItem01.Text", LocalResourceFile()), "1"))
            End If

            Dim tmpUsers As ArrayList
            If roleName <> regRoleName Or searchField <> "1" Then
                tmpUsers = CType(objCtlRole.GetUsersByRole(PortalId, roleName), ArrayList)
            Else
                tmpUsers = CType(objCtlRole.GetUsersByRole(PortalId, searchText), ArrayList)
            End If

            Dim objCtlEventSignups As New EventSignupsController
            Dim lstSignups As ArrayList = objCtlEventSignups.EventsSignupsGetEvent(ItemID(), ModuleId)

            Users = New ArrayList
            If searchText <> "None" Then
                For Each objUser As UserInfo In tmpUsers
                    Select Case SearchField
                        Case "0" 'username
                            If Left(objUser.Username, Len(searchText)).ToLower = searchText.ToLower Then
                                UserAdd(objUser, lstSignups)
                            End If
                        Case "1" 'Groupname
                            UserAdd(objUser, lstSignups)
                        Case "2" 'Lastname
                            If Left(objUser.LastName, Len(searchText)).ToLower = searchText.ToLower Then
                                UserAdd(objUser, lstSignups)
                            End If
                        Case Else
                            UserAdd(objUser, lstSignups)
                    End Select
                Next
            End If
            If Users.Count > 0 Then
                gvUsersToEnroll.Visible = True
                cmdSelectedAddUser.Visible = True
            Else
                gvUsersToEnroll.Visible = False
                cmdSelectedAddUser.Visible = False
            End If
            gvUsersToEnroll.DataSource = Users
            gvUsersToEnroll.DataBind()
        End Sub

        Private Sub UserAdd(ByVal inUser As UserInfo, ByVal lstSignups As ArrayList)
            Dim blAdd As Boolean = True
            For Each objEventSignup As EventSignupsInfo In lstSignups
                If inUser.UserID = objEventSignup.UserID Then
                    blAdd = False
                End If
            Next
            If blAdd Then
                Users.Add(inUser)
            End If

        End Sub

        Protected Sub gvUsersToEnroll_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gvUsersToEnroll.PageIndexChanging
            gvUsersToEnroll.PageIndex = e.NewPageIndex
            BindData(txtFilterUsers.Text, dropdownFilterItem.Value)
        End Sub

        Protected Sub cmdSelectedAddUser_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSelectedAddUser.Click
            Dim row As GridViewRow
            Dim arrUsers As New ArrayList
            Try
                For Each row In gvUsersToEnroll.Rows
                    If CType(row.FindControl("chkSelectUser"), CheckBox).Checked Then
                        arrUsers.Add(CInt(gvUsersToEnroll.DataKeys(row.RowIndex).Value))
                    End If
                Next
                RaiseEvent AddSelectedUsers(Me, New EventArgs, arrUsers)
            Catch ex As Exception

            End Try

        End Sub

        Protected Sub cmdRefreshList_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdRefreshList.Click
            gvUsersToEnroll.PageIndex = 0
            BindData(txtFilterUsers.Text, dropdownFilterItem.Value)
            If dropdownFilterItem.Value = "1" Then
                lblStartswith.Attributes.Add("style", "display: none")
            Else
                lblStartswith.Attributes.Remove("style")
            End If

        End Sub
    End Class
End Namespace