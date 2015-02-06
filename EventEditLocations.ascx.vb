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
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Common.Lists
Imports System.Collections.Generic
Imports DotNetNuke.UI.UserControls

Namespace DotNetNuke.Modules.Events

    <DNNtc.ModuleControlProperties("Locations", "Edit Event Locations", DNNtc.ControlType.View, "https://dnnevents.codeplex.com/documentation", True, True)> _
    Partial Class EventEditLocations
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

        End Sub

#End Region

#Region "Private Area"

        Private ReadOnly _objCtlLocation As New EventLocationController
        Private _objLocation As New EventLocationInfo
        Private _colLocations As ArrayList
#End Region

#Region "Event Handlers"
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
            LocalizeAll()

            If PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString) Or _
               IsLocationEditor() Then
            Else
                Response.Redirect(GetSocialNavigateUrl(), True)
            End If

            ' Set the selected theme 
            SetTheme(pnlEventsModuleLocations)

            If Not Page.IsPostBack Then
                BindData()
                BindCountry()
                BindRegion()
            End If
        End Sub
#End Region

#Region "Helper Routines"
        Private Sub LocalizeAll()
            GrdLocations.Columns(3).HeaderText = Localization.GetString("plLocationName", LocalResourceFile)
            GrdLocations.Columns(4).HeaderText = Localization.GetString("plMapURL", LocalResourceFile)
        End Sub

        Private Sub BindData()
            'Grid
            Dim i As Integer
            _colLocations = _objCtlLocation.EventsLocationList(PortalId)
            GrdLocations.DataSource = _colLocations
            GrdLocations.DataBind()
            If GrdLocations.Items.Count > 0 Then
                GrdLocations.Visible = True
                For i = 0 To GrdLocations.Items.Count - 1
                    CType(GrdLocations.Items(i).FindControl("DeleteButton"), ImageButton).Attributes.Add("onclick", "javascript:return confirm('" + Localization.GetString("AreYouSureYouWishToDelete.Text", LocalResourceFile) + "');")
                Next
            End If
        End Sub

        Private Sub BindCountry()
            Dim ctlEntry As New ListController()
            Dim entryCollection As IEnumerable(Of ListEntryInfo) = ctlEntry.GetListEntryInfoItems("Country")

            cboCountry.DataSource = entryCollection
            cboCountry.DataBind()
            cboCountry.Items.Insert(0, New ListItem("<" + Localization.GetString("Not_Specified", Localization.SharedResourceFile) + ">", ""))
            If Not Page.IsPostBack Then cboCountry.SelectedIndex = 0
        End Sub

        Private Sub BindRegion()
            Dim ctlEntry As New ListController()
            Dim countryCode As String = cboCountry.SelectedItem.Value
            Dim listKey As String = "Country." + countryCode ' listKey in format "Country.US:Region"
            Dim entryCollection As IEnumerable(Of ListEntryInfo) = ctlEntry.GetListEntryInfoItems("Region", listKey)

            If entryCollection.Any() Then
                txtRegion.Visible = False
                cboRegion.Visible = True

                cboRegion.Items.Clear()
                cboRegion.DataSource = entryCollection
                cboRegion.DataBind()
                cboRegion.Items.Insert(0, New ListItem("<" + Localization.GetString("Not_Specified", Localization.SharedResourceFile) + ">", ""))

                If countryCode.ToLower() = "us" Then
                    lblRegionCap.Text = Localization.GetString("plState.Text", LocalResourceFile)
                    lblRegionCap.HelpText = Localization.GetString("plState.Help", LocalResourceFile)
                    lblPostalCodeCap.Text = Localization.GetString("plZipCode.Text", LocalResourceFile)
                    lblPostalCodeCap.HelpText = Localization.GetString("plZipCode.Help", LocalResourceFile)
                Else
                    lblRegionCap.Text = Localization.GetString("plRegion.Text", LocalResourceFile)
                    lblRegionCap.HelpText = Localization.GetString("plRegion.Help", LocalResourceFile)
                    lblPostalCodeCap.Text = Localization.GetString("plPostalCode.Text", LocalResourceFile)
                    lblPostalCodeCap.HelpText = Localization.GetString("plPostalCode.Help", LocalResourceFile)
                End If
            Else
                txtRegion.Visible = True
                cboRegion.Visible = False

                lblRegionCap.Text = Localization.GetString("plRegion.Text", LocalResourceFile)
                lblRegionCap.HelpText = Localization.GetString("plRegion.Help", LocalResourceFile)
                lblPostalCodeCap.Text = Localization.GetString("plPostalCode.Text", LocalResourceFile)
                lblPostalCodeCap.HelpText = Localization.GetString("plPostalCode.Help", LocalResourceFile)
            End If
        End Sub

        Private Function PutHTTPInFront(ByVal myURL As String) As String
            If myURL.ToLower.StartsWith("http://") Or myURL.ToLower.StartsWith("https://") Then
                Return myURL
            ElseIf Trim(myURL) = "" Then
                Return ""
            Else
                Return AddHTTP(myURL)
            End If
        End Function

        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
            Try
                SaveLocation(CInt(ViewState("Location")))
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
            BindData()
        End Sub

        Private Sub cmdAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdAdd.Click
            Try
                SaveLocation(0)
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
            BindData()
        End Sub

        Private Sub SaveLocation(ByVal locationID As Integer)
            ' Only Update if the Entered Data is Valid
            If Page.IsValid And Not String.IsNullOrEmpty(txtLocationName.Text) Then

                Dim objLocation As New EventLocationInfo
                Dim objSecurity As New PortalSecurity
                Dim locationName, mapURL, street, postalCode, city As String
                Dim region As String = Nothing
                Dim country As String = Nothing

                ' Filter text for non-admins
                If (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString) = True) Then
                    locationName = txtLocationName.Text
                    mapURL = PutHTTPInFront(txtMapURL.Text)
                    street = txtStreet.Text
                    postalCode = txtPostalCode.Text
                    city = txtCity.Text
                    If cboRegion.SelectedIndex > 0 Then region = cboRegion.SelectedItem.Text Else region = txtRegion.Text
                    If cboCountry.SelectedIndex > 0 Then country = cboCountry.SelectedItem.Text
                Else
                    locationName = objSecurity.InputFilter(txtLocationName.Text, PortalSecurity.FilterFlag.NoScripting)
                    mapURL = objSecurity.InputFilter(PutHTTPInFront(txtMapURL.Text), PortalSecurity.FilterFlag.NoScripting)
                    street = objSecurity.InputFilter(txtStreet.Text, PortalSecurity.FilterFlag.NoScripting)
                    postalCode = objSecurity.InputFilter(txtPostalCode.Text, PortalSecurity.FilterFlag.NoScripting)
                    city = objSecurity.InputFilter(txtCity.Text, PortalSecurity.FilterFlag.NoScripting)
                    If cboRegion.SelectedIndex > 0 Then
                        region = objSecurity.InputFilter(cboRegion.SelectedItem.Text, PortalSecurity.FilterFlag.NoScripting)
                    Else
                        region = objSecurity.InputFilter(txtRegion.Text, PortalSecurity.FilterFlag.NoScripting)
                    End If
                    If cboCountry.SelectedIndex > 0 Then
                        country = objSecurity.InputFilter(cboCountry.SelectedItem.Text, PortalSecurity.FilterFlag.NoScripting)
                    End If
                End If

                'bind text values to object
                objLocation.Location = locationID
                objLocation.PortalID = PortalId
                objLocation.LocationName = locationName
                objLocation.MapURL = mapURL
                objLocation.Street = street
                objLocation.PostalCode = postalCode
                objLocation.City = city
                objLocation.Region = region
                objLocation.Country = country
                _objCtlLocation.EventsLocationSave(objLocation)

                'Back to normal (add) mode
                txtLocationName.Text = ""
                txtMapURL.Text = ""
                txtStreet.Text = ""
                txtPostalCode.Text = ""
                txtCity.Text = ""
                txtRegion.Text = ""
                cboRegion.ClearSelection()
                cboCountry.ClearSelection()

                ViewState.Remove("Location")
                cmdUpdate.Visible = False
            End If
        End Sub

        Protected Sub OnCountryIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cboCountry.SelectedIndexChanged
            Try
                BindRegion()
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub
#End Region

#Region "Control Events"
        Public Sub GrdLocations_DeleteCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles GrdLocations.DeleteCommand
            Dim location As Integer
            location = CType(GrdLocations.DataKeys(e.Item.ItemIndex), Integer)
            _objCtlLocation.EventsLocationDelete(location, PortalId)
            BindData()

            'Back to normal (add) mode
            txtLocationName.Text = ""
            txtMapURL.Text = ""
            txtStreet.Text = ""
            txtPostalCode.Text = ""
            txtCity.Text = ""
            txtRegion.Text = ""
            cboRegion.ClearSelection()
            cboCountry.ClearSelection()

            ViewState.Remove("Location")
            cmdUpdate.Visible = False
        End Sub

        Public Sub GrdLocations_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles GrdLocations.ItemCommand
            Select Case e.CommandName
                Case "Select"
                    Dim location As Integer = CType(GrdLocations.DataKeys(e.Item.ItemIndex), Int16)
                    _objLocation = _objCtlLocation.EventsLocationGet(location, PortalId)

                    ' Clear selections.
                    cboCountry.ClearSelection()
                    cboRegion.ClearSelection()
                    txtRegion.Text = String.Empty

                    ' Fill fields.
                    txtLocationName.Text = _objLocation.LocationName
                    txtMapURL.Text = _objLocation.MapURL
                    txtStreet.Text = _objLocation.Street
                    txtPostalCode.Text = _objLocation.PostalCode
                    txtCity.Text = _objLocation.City

                    If cboCountry.Items.FindByText(_objLocation.Country) IsNot Nothing Then
                        cboCountry.Items.FindByText(_objLocation.Country).Selected = True
                    End If
                    BindRegion()
                    If cboRegion.Items.FindByText(_objLocation.Region) IsNot Nothing Then
                        cboRegion.Items.FindByText(_objLocation.Region).Selected = True
                    Else
                        txtRegion.Text = _objLocation.Region
                    End If

                    ViewState.Add("Location", _objLocation.Location.ToString())
                    BindData()

                    'We can update
                    cmdUpdate.Visible = True
            End Select
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
