'
' DotNetNuke® - http://www.dnnsoftware.com
' Copyright (c) 2002-2014
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
Imports System.Web.UI.WebControls
Imports DotNetNuke.Security

Namespace DotNetNuke.Modules.Events

    Partial Class SelectLocation
        Inherits EventBase

#Region "Properties"

        Private _selectedLocation As New ArrayList
        Private _gotLocations As Boolean = False
        Private _modulelocationids As New ArrayList
        Public Property SelectedLocation() As ArrayList
            Get
                'have selected the location before?
                If Not _gotLocations Then
                    _gotLocations = True
                    _selectedLocation.Clear()

                    'is there a default module location when location select has been disabled
                    'if not has it been passed in as a parameter 
                    'if not is there a default module location when location select has not been disabled
                    'if not is there as setting in cookies available?
                    If Settings.Enablelocations = EventModuleSettings.DisplayLocations.DoNotDisplay Then
                        If Settings.ModuleLocationsSelected = EventModuleSettings.LocationsSelected.All Then
                            _selectedLocation.Clear()
                            _selectedLocation.Add("-1")
                        Else
                            _selectedLocation.Clear()
                            For Each location As Integer In Settings.ModuleLocationIDs
                                _selectedLocation.Add(location)
                            Next
                        End If
                    ElseIf Not Request.Params("Location") = Nothing Then
                        Dim objSecurity As New PortalSecurity
                        Dim tmpLocation As String = Request.Params("Location")
                        tmpLocation = objSecurity.InputFilter(tmpLocation, PortalSecurity.FilterFlag.NoScripting)
                        tmpLocation = objSecurity.InputFilter(tmpLocation, PortalSecurity.FilterFlag.NoSQL)
                        Dim oCntrlEventLocation As New EventLocationController
                        Dim oEventLocation As EventLocationInfo = oCntrlEventLocation.EventsLocationGetByName(tmpLocation, PortalSettings.PortalId)
                        If Not oEventLocation Is Nothing Then
                            _selectedLocation.Add(oEventLocation.Location)
                        End If
                    ElseIf Settings.ModuleLocationsSelected <> EventModuleSettings.LocationsSelected.All Then
                        _selectedLocation.Clear()
                        For Each location As Integer In Settings.ModuleLocationIDs
                            _selectedLocation.Add(location)
                        Next
                    ElseIf Request.Cookies("DNNEvents") Is Nothing Then
                        _selectedLocation.Clear()
                        _selectedLocation.Add("-1")
                    Else
                        'Do we have a special one for this module?
                        If Request.Cookies("DNNEvents")("EventLocation" & ModuleId) Is Nothing Then
                            _selectedLocation.Clear()
                            _selectedLocation.Add("-1")
                        Else
                            'Yes there is one!
                            Dim objSecurity As New PortalSecurity
                            Dim tmpLocation As String = Request.Cookies("DNNEvents")("EventLocation" & ModuleId)
                            tmpLocation = objSecurity.InputFilter(tmpLocation, PortalSecurity.FilterFlag.NoScripting)
                            tmpLocation = objSecurity.InputFilter(tmpLocation, PortalSecurity.FilterFlag.NoSQL)
                            Dim tmpArray() As String = Split(tmpLocation, ",")
                            For i As Integer = 0 To tmpArray.Length - 1
                                If tmpArray(i) <> "" Then
                                    _selectedLocation.Add(CInt(tmpArray(i)))
                                End If
                            Next
                        End If
                    End If
                End If
                Return _selectedLocation
            End Get
            Set(ByVal value As ArrayList)
                Try
                    _selectedLocation = value
                    _gotLocations = True
                    Response.Cookies("DNNEvents")("EventLocation" & ModuleId) = String.Join(",", CType(_selectedLocation.ToArray(GetType(String)), String()))
                    Response.Cookies("DNNEvents").Expires = DateTime.Now.AddMinutes(2)
                    Response.Cookies("DNNEvents").Path = "/"
                Catch ex As Exception
                End Try
            End Set
        End Property
        Public Property ModuleLocationIDs As ArrayList
            Get
                Return _modulelocationids
            End Get
            Set(value As ArrayList)
                _modulelocationids = value
            End Set
        End Property
#End Region

#Region "Public Methods"
        Public Sub StoreLocations()
            SelectedLocation.Clear()
            Dim lstLocations As New ArrayList
            If Settings.Enablelocations = EventModuleSettings.DisplayLocations.SingleSelect Then
                lstLocations.Add(ddlLocations.SelectedValue)
            Else
                If ddlLocations.CheckedItems.Count <> ddlLocations.Items.Count Then
                    For Each item As Telerik.Web.UI.RadComboBoxItem In ddlLocations.CheckedItems
                        lstLocations.Add(item.Value)
                    Next
                Else
                    lstLocations.Add("-1")
                End If
            End If
            SelectedLocation = lstLocations
        End Sub

#End Region

#Region "Event Handlers"
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
            Try
                ' Add the external Validation.js to the Page
                Const csname As String = "ExtValidationScriptFile"
                Dim cstype As Type = Reflection.MethodBase.GetCurrentMethod().GetType()
                Dim cstext As String = "<script src=""" & ResolveUrl("~/DesktopModules/Events/Scripts/Validation.js") & """ type=""text/javascript""></script>"
                If Not Page.ClientScript.IsClientScriptBlockRegistered(csname) Then
                    Page.ClientScript.RegisterClientScriptBlock(cstype, csname, cstext, False)
                End If

                ddlLocations.EmptyMessage = Localization.GetString("NoLocations", LocalResourceFile)
                ddlLocations.Localization.AllItemsCheckedString = Localization.GetString("AllLocations", LocalResourceFile)
                ddlLocations.Localization.CheckAllString = Localization.GetString("SelectAllLocations", LocalResourceFile)
                If Settings.Enablelocations = EventModuleSettings.DisplayLocations.SingleSelect Then
                    ddlLocations.CheckBoxes = False
                End If

                If Not Page.IsPostBack Then
                    'Bind DDL
                    Dim ctrlEventLocations As New EventLocationController
                    Dim lstLocations As ArrayList = ctrlEventLocations.EventsLocationList(PortalId)

                    Dim arrLocations As New ArrayList
                    If Settings.Restrictlocations Then
                        For Each dbLocation As EventLocationInfo In lstLocations
                            For Each location As Integer In Settings.ModuleLocationIDs
                                If dbLocation.Location = location Then
                                    arrLocations.Add(dbLocation)
                                End If
                            Next
                        Next
                    Else
                        arrLocations.AddRange(lstLocations)
                    End If

                    If lstLocations.Count = 0 Then
                        Visible = False
                        SelectedLocation.Clear()
                        Exit Sub
                    End If

                    'Restrict locations by events in time frame.
                    If Settings.RestrictLocationsToTimeFrame Then
                        'Only for list view.
                        Dim whichView As String = String.Empty
                        If Not Request.QueryString("mctl") = Nothing And ModuleId = CType(Request.QueryString("ModuleID"), Integer) Then
                            If Request("mctl").EndsWith(".ascx") Then
                                whichView = Request("mctl")
                            Else
                                whichView = Request("mctl") & ".ascx"
                            End If
                        End If
                        If whichView.Length = 0 Then
                            If Not Request.Cookies.Get("DNNEvents" & ModuleId) Is Nothing Then
                                whichView = Request.Cookies.Get("DNNEvents" & ModuleId).Value
                            Else
                                whichView = Settings.DefaultView
                            End If
                        End If

                        If (whichView = "EventList.ascx" OrElse whichView = "EventRpt.ascx") Then
                            Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
                            Dim lstEvents As ArrayList

                            Dim getSubEvents As Boolean = Settings.MasterEvent
                            Dim numDays As Integer = Settings.EventsListEventDays
                            Dim displayDate, startDate, endDate As DateTime
                            If Settings.ListViewUseTime Then
                                displayDate = DisplayNow()
                            Else
                                displayDate = DisplayNow.Date
                            End If
                            If Settings.EventsListSelectType = "DAYS" Then
                                startDate = displayDate.AddDays(Settings.EventsListBeforeDays * -1)
                                endDate = displayDate.AddDays(Settings.EventsListAfterDays * 1)
                            Else
                                startDate = displayDate
                                endDate = displayDate.AddDays(numDays)
                            End If

                            lstEvents = objEventInfoHelper.GetEvents(startDate, endDate, getSubEvents, _
                                                                     New ArrayList({"-1"}), New ArrayList({"-1"}), -1, -1)

                            Dim eventLocationIds As ArrayList = New ArrayList()
                            For Each lstEvent As EventInfo In lstEvents
                                eventLocationIds.Add(lstEvent.Location)
                            Next
                            For Each lstLocation As EventLocationInfo In lstLocations
                                If Not eventLocationIds.Contains(lstLocation.Location) Then
                                    arrLocations.Remove(lstLocation)
                                End If
                            Next
                        End If
                    End If

                    'Bind locations.
                    ddlLocations.DataSource = arrLocations
                    ddlLocations.DataBind()

                    If Settings.Enablelocations = EventModuleSettings.DisplayLocations.SingleSelect Then
                        ddlLocations.Items.Insert(0, New Telerik.Web.UI.RadComboBoxItem(Localization.GetString("AllLocations", LocalResourceFile), "-1"))
                        ddlLocations.SelectedIndex = 0
                    End If
                    ddlLocations.OnClientDropDownClosed = "function() { btnUpdateClick('" + btnUpdate.UniqueID + "','" + ddlLocations.ClientID + "');}"
                    ddlLocations.OnClientLoad = "function() { storeText('" + ddlLocations.ClientID + "');}"
                    If Settings.Enablelocations = EventModuleSettings.DisplayLocations.SingleSelect Then
                        For Each location As Integer In SelectedLocation
                            ddlLocations.SelectedIndex = ddlLocations.FindItemByValue(location.ToString).Index
                            Exit For
                        Next
                    Else
                        For Each location As Integer In SelectedLocation
                            For Each item As Telerik.Web.UI.RadComboBoxItem In ddlLocations.Items
                                If item.Value = location.ToString Then
                                    item.Checked = True
                                End If
                            Next
                        Next

                        If CInt(SelectedLocation.Item(0)) = -1 Then
                            For Each item As Telerik.Web.UI.RadComboBoxItem In ddlLocations.Items
                                item.Checked = True
                            Next
                        End If
                    End If

                End If
            Catch exc As Exception
                'ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
            StoreLocations()

            ' Fire the LocationSelected event...
            Dim args As CommandEventArgs = New CommandEventArgs(SelectedLocation.ToString, Nothing)
            RaiseEvent LocationSelectedChanged(Me, args)
        End Sub

        Public Event LocationSelectedChanged(ByVal sender As Object, ByVal e As CommandEventArgs)

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

End Namespace
