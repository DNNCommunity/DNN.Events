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

Imports DotNetNuke.Security

Namespace DotNetNuke.Modules.Events

    <DNNtc.ModuleControlProperties("Categories", "Edit Event Categories", DNNtc.ControlType.View, "https://dnnevents.codeplex.com/documentation", False, True)> _
    Partial Class EventEditCategories
        Inherits EventBase

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()

            'Add the external Validation.js to the Page
            Const csname As String = "ExtValidationScriptFile"
            Dim cstype As Type = System.Reflection.MethodBase.GetCurrentMethod().GetType()
            Dim cstext As String = "<script src=""" & ResolveUrl("~/DesktopModules/Events/Scripts/Validation.js") & """ type=""text/javascript""></script>"
            If Not Page.ClientScript.IsClientScriptBlockRegistered(csname) Then
                Page.ClientScript.RegisterClientScriptBlock(cstype, csname, cstext, False)
            End If

            Const csname2 As String = "LoadPreview"
            Dim cstype2 As Type = System.Reflection.MethodBase.GetCurrentMethod().GetType()
            Dim cstext2 As String = "<script type=""text/javascript"">byid('" & lblPreviewCat.ClientID & "').innerHTML = byid('" & _
                                   txtCategoryName.ClientID & "').value;byid('" & lblPreviewCat.ClientID & "').style.color = byid('" & _
                                  txtCategoryFontColor.ClientID & "').value;byid('" & previewpane.ClientID & "').style.backgroundColor = byid('" & _
                                 txtCategoryColor.ClientID & "').value;</script>"
            Page.ClientScript.RegisterStartupScript(cstype2, csname2, cstext2, False)

            Dim previewScript As String
            previewScript = "CategoryPreviewPane('" & cpBackColor.ClientID & "','" & cpForeColor.ClientID & "','" & previewpane.ClientID & "','" & lblPreviewCat.ClientID & "','" & txtCategoryFontColor.ClientID & "','" & txtCategoryColor.ClientID & "','" & txtCategoryName.ClientID & "','" + Localization.GetString("InvalidColor", LocalResourceFile) + "');"

            Const csname3 As String = "ColorPicker"
            Dim cstype3 As Type = System.Reflection.MethodBase.GetCurrentMethod().GetType()
            Dim cstext3 As String = "<script type=""text/javascript"">"
            cstext3 += " function HandleColorChange(sender,eventargs) { $get(""" & txtCategoryColor.ClientID & """).value = sender.get_selectedColor(); " & previewScript & "}"
            cstext3 += " function HandleColorFontChange(sender,eventargs) { $get(""" & txtCategoryFontColor.ClientID & """).value = sender.get_selectedColor(); " & previewScript & "}"
            cstext3 += "  </script>"
            If Not Page.ClientScript.IsClientScriptBlockRegistered(csname3) Then
                Page.ClientScript.RegisterClientScriptBlock(cstype3, csname3, cstext3, False)
            End If

            txtCategoryName.Attributes.Add("onchange", "byid('" & lblPreviewCat.ClientID & "').innerHTML = this.value;")

            txtCategoryFontColor.Attributes.Add("onchange", previewScript)
            txtCategoryColor.Attributes.Add("onchange", previewScript)


            'ColorPicker Icons

        End Sub

#End Region

#Region "Private Area"

        'Private itemId As Integer
        Private ReadOnly _objCtlCategory As New EventCategoryController
        Private _objCategory As New EventCategoryInfo
        Private _colCategories As ArrayList
        Private Const DefaultBackColor As String = "#ffffff"
        Private Const DefaultFontColor As String = "#000000"

#End Region

#Region "Event Handler"

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load

            LocalizeAll()

            If PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString) Or _
               IsCategoryEditor() Then
            Else
                Response.Redirect(GetSocialNavigateUrl(), True)
            End If

            ' Set the selected theme 
            SetTheme(pnlEventsModuleCategories)

            If Not Page.IsPostBack Then
                SetDefaultValues()
                BindData()
            End If
        End Sub

#End Region

#Region "Helper Routines"

        Private Sub LocalizeAll()
            GrdCategories.Columns(3).HeaderText = Localization.GetString("plCategoryName", LocalResourceFile)
        End Sub

        Private Sub BindData()
            Dim i As Integer

            _colCategories = _objCtlCategory.EventsCategoryList(PortalId)
            GrdCategories.DataSource = _colCategories
            GrdCategories.DataBind()
            If GrdCategories.Items.Count > 0 Then
                GrdCategories.Visible = True
                For i = 0 To GrdCategories.Items.Count - 1
                    CType(GrdCategories.Items(i).FindControl("DeleteButton"), ImageButton).Attributes.Add("onclick", "javascript:return confirm('" + Localization.GetString("AreYouSureYouWishToDelete.Text", LocalResourceFile) + "');")
                Next
            End If

        End Sub

#End Region

#Region "Control Events"

        Public Sub GrdCategories_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles GrdCategories.ItemCommand
            Select Case e.CommandName
                Case "Select"
                    Dim category As Integer = CType(GrdCategories.DataKeys(e.Item.ItemIndex), Int16)
                    _objCategory = _objCtlCategory.EventCategoryGet(category, PortalId)
                    txtCategoryName.Text = _objCategory.CategoryName
                    If _objCategory.Color <> "" Then
                        txtCategoryColor.Text = _objCategory.Color
                        cpBackColor.SelectedColor = ColorTranslator.FromHtml(txtCategoryColor.Text)
                    Else
                        txtCategoryColor.Text = ""
                        cpBackColor.SelectedColor = ColorTranslator.FromHtml(DefaultBackColor)
                    End If
                    If _objCategory.FontColor <> "" Then
                        txtCategoryFontColor.Text = _objCategory.FontColor
                        cpForeColor.SelectedColor = ColorTranslator.FromHtml(txtCategoryFontColor.Text)
                    Else
                        txtCategoryFontColor.Text = ""
                        cpForeColor.SelectedColor = ColorTranslator.FromHtml(DefaultFontColor)
                    End If

                    'Remember that we might use update
                    ViewState.Add("Category", _objCategory.Category.ToString())
                    cmdUpdate.Visible = True

                    BindData()
            End Select
        End Sub

        Public Sub GrdCategories_DeleteCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles GrdCategories.DeleteCommand
            Dim category As Integer
            category = CType(GrdCategories.DataKeys(e.Item.ItemIndex), Integer)
            divDeleteError.Visible = False

            Dim objDesktopModule As Entities.Modules.DesktopModuleInfo
            Dim objModules As New Entities.Modules.ModuleController
            objDesktopModule = Entities.Modules.DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", PortalId)
            Dim lstModules As ArrayList = objModules.GetModulesByDefinition(PortalId, objDesktopModule.FriendlyName)
            For Each objModule As Entities.Modules.ModuleInfo In lstModules
                Dim ems As New EventModuleSettings
                Dim categories As ArrayList = ems.GetEventModuleSettings(objModule.ModuleID, Nothing).ModuleCategoryIDs
                If categories.Contains(category) Then
                    lblDeleteError.Text = String.Format(Localization.GetString("lblDeleteError", LocalResourceFile), objModule.ModuleTitle)
                    divDeleteError.Visible = True
                    Return
                End If
            Next


            _objCtlCategory.EventsCategoryDelete(category, PortalId)
            BindData()

            'Be sure we cannot update any more
            txtCategoryName.Text = ""
            txtCategoryColor.Text = ""
            txtCategoryFontColor.Text = ""
            ViewState.Remove("Category")
            cmdUpdate.Visible = False
            cpBackColor.SelectedColor = ColorTranslator.FromHtml(DefaultBackColor)
            cpForeColor.SelectedColor = ColorTranslator.FromHtml(DefaultFontColor)

        End Sub

        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
            Try

                ' Only Update if the Entered Data is Valid
                If Page.IsValid And Not String.IsNullOrEmpty(txtCategoryName.Text) Then
                    Dim objCategory As New EventCategoryInfo
                    Dim objSecurity As New PortalSecurity
                    Dim categoryName As String

                    ' Filter text for non-admins
                    If (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString) = True) Then
                        categoryName = txtCategoryName.Text
                    Else
                        categoryName = objSecurity.InputFilter(txtCategoryName.Text, PortalSecurity.FilterFlag.NoScripting)
                    End If

                    'bind text values to object

                    objCategory.Category = CInt(ViewState("Category"))
                    objCategory.PortalID = PortalId
                    objCategory.CategoryName = categoryName
                    objCategory.Color = txtCategoryColor.Text
                    objCategory.FontColor = txtCategoryFontColor.Text
                    _objCtlCategory.EventsCategorySave(objCategory)

                    SetDefaultValues()
                End If

            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

            BindData()

        End Sub

        Private Sub cmdAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdAdd.Click
            Try
                ' Only Update if the Entered Data is Valid
                If Page.IsValid = True And Not String.IsNullOrEmpty(txtCategoryName.Text) Then
                    Dim objCategory As New EventCategoryInfo
                    Dim objSecurity As New PortalSecurity
                    Dim categoryName As String

                    ' Filter text for non-admins
                    If (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString) = True) Then
                        categoryName = txtCategoryName.Text
                    Else
                        categoryName = objSecurity.InputFilter(txtCategoryName.Text, PortalSecurity.FilterFlag.NoScripting)
                    End If

                    'bind text values to object

                    objCategory.Category = 0
                    objCategory.PortalID = PortalId
                    objCategory.CategoryName = categoryName
                    objCategory.Color = txtCategoryColor.Text
                    objCategory.FontColor = txtCategoryFontColor.Text
                    _objCtlCategory.EventsCategorySave(objCategory)

                    SetDefaultValues()
                End If

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

#Region "Private Functions"
        Private Sub SetDefaultValues()
            'Back to normal (add) mode
            txtCategoryName.Text = ""
            txtCategoryColor.Text = ""
            txtCategoryFontColor.Text = ""
            ViewState.Remove("Category")
            cmdUpdate.Visible = False
            cpBackColor.SelectedColor = ColorTranslator.FromHtml(DefaultBackColor)
            cpForeColor.SelectedColor = ColorTranslator.FromHtml(DefaultFontColor)
        End Sub

#End Region

    End Class

End Namespace
