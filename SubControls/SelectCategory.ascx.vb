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

    Partial  Class SelectCategory
        Inherits EventBase

#Region "Properties"

        Private _selectedCategory As New ArrayList
        Private _gotCategories As Boolean = False
        Private _modulecategoryids As New ArrayList
        Public Property SelectedCategory() As ArrayList
            Get
                'have selected the category before?
                If Not _GotCategories Then
                    _GotCategories = True
                    _SelectedCategory.Clear()
                    'is there a default module category when category select has been disabled
                    'if not has it been passed in as a parameter 
                    'if not is there a default module category when category select has not been disabled
                    'if not is there as setting in cookies available?
                    If Settings.enablecategories = EventModuleSettings.DisplayCategories.DoNotDisplay Then
                        If Settings.ModuleCategoriesSelected = EventModuleSettings.CategoriesSelected.All Then
                            _SelectedCategory.Clear()
                            _SelectedCategory.Add("-1")
                        Else
                            _SelectedCategory.Clear()
                            For Each category As Integer In Settings.ModuleCategoryIDs
                                _selectedCategory.Add(category)
                            Next
                        End If
                    ElseIf Not Request.Params("Category") = Nothing Then
                        Dim objSecurity As New PortalSecurity
                        Dim tmpCategory As String = Request.Params("Category")
                        tmpCategory = objSecurity.InputFilter(tmpCategory, PortalSecurity.FilterFlag.NoScripting)
                        tmpCategory = objSecurity.InputFilter(tmpCategory, PortalSecurity.FilterFlag.NoSQL)
                        Dim oCntrlEventCategory As New EventCategoryController
                        Dim oEventCategory As EventCategoryInfo = oCntrlEventCategory.EventCategoryGetByName(tmpCategory, PortalSettings.PortalId)
                        If Not oEventCategory Is Nothing Then
                            _SelectedCategory.Add(oEventCategory.Category)
                        End If
                    ElseIf Settings.ModuleCategoriesSelected <> EventModuleSettings.CategoriesSelected.All Then
                        _SelectedCategory.Clear()
                        For Each category As Integer In Settings.ModuleCategoryIDs
                            _selectedCategory.Add(category)
                        Next
                    ElseIf Request.Cookies("DNNEvents") Is Nothing Then
                        _SelectedCategory.Clear()
                        _SelectedCategory.Add("-1")
                    Else
                        'Do we have a special one for this module?
                        If Request.Cookies("DNNEvents")("EventCategory" & ModuleId) Is Nothing Then
                            _SelectedCategory.Clear()
                            _SelectedCategory.Add("-1")
                        Else
                            'Yes there is one!
                            Dim objSecurity As New PortalSecurity
                            Dim tmpCategory As String = Request.Cookies("DNNEvents")("EventCategory" & ModuleId)
                            tmpCategory = objSecurity.InputFilter(tmpCategory, PortalSecurity.FilterFlag.NoScripting)
                            tmpCategory = objSecurity.InputFilter(tmpCategory, PortalSecurity.FilterFlag.NoSQL)
                            Dim tmpArray() As String = Split(tmpCategory, ",")
                            For i As Integer = 0 To tmpArray.Length - 1
                                If tmpArray(i) <> "" Then
                                    _SelectedCategory.Add(CInt(tmpArray(i)))
                                End If
                            Next
                        End If
                    End If
                End If
                Return _SelectedCategory
            End Get
            Set(ByVal value As ArrayList)
                Try
                    _selectedCategory = Value
                    _gotCategories = True
                    Response.Cookies("DNNEvents")("EventCategory" & ModuleId) = String.Join(",", CType(_selectedCategory.ToArray(GetType(String)), String()))
                    Response.Cookies("DNNEvents").Expires = DateTime.Now.AddMinutes(2)
                    Response.Cookies("DNNEvents").Path = "/"
                Catch ex As Exception
                End Try
            End Set
        End Property
        Public Property ModuleCategoryIDs As ArrayList
            Get
                Return _modulecategoryids
            End Get
            Set(value As ArrayList)
                _modulecategoryids = value
            End Set
        End Property
#End Region
#Region "Public Methods"
        Public Sub StoreCategories()
            SelectedCategory.Clear()
            Dim lstCategories As New ArrayList
            If Settings.enablecategories = EventModuleSettings.DisplayCategories.SingleSelect Then
                lstCategories.Add(ddlCategories.SelectedValue)
            Else
                If ddlCategories.CheckedItems.Count <> ddlCategories.Items.Count Then
                    For Each item As Telerik.Web.UI.RadComboBoxItem In ddlCategories.CheckedItems
                        lstCategories.Add(item.Value)
                    Next
                Else
                    lstCategories.Add("-1")
                End If
            End If
            SelectedCategory = lstCategories
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

                ddlCategories.EmptyMessage = Localization.GetString("NoCategories", LocalResourceFile)
                ddlCategories.Localization.AllItemsCheckedString = Localization.GetString("AllCategories", LocalResourceFile)
                ddlCategories.Localization.CheckAllString = Localization.GetString("SelectAllCategories", LocalResourceFile)
                If Settings.enablecategories = EventModuleSettings.DisplayCategories.SingleSelect Then
                    ddlCategories.CheckBoxes = False
                End If

                If Not Page.IsPostBack Then
                    'Bind DDL
                    Dim ctrlEventCategories As New EventCategoryController
                    Dim lstCategories As ArrayList = ctrlEventCategories.EventsCategoryList(PortalId)

                    Dim arrCategories As New ArrayList
                    If Settings.restrictcategories Then
                        For Each dbCategory As EventCategoryInfo In lstCategories
                            For Each category As Integer In Settings.ModuleCategoryIDs
                                If dbCategory.Category = category Then
                                    arrCategories.Add(dbCategory)
                                End If
                            Next
                        Next
                    Else
                        arrCategories.AddRange(lstCategories)
                    End If

                    If lstCategories.Count = 0 Then
                        Visible = False
                        SelectedCategory.Clear()
                        Exit Sub
                    End If

                    'Restrict categories by events in time frame.
                    If Settings.RestrictCategoriesToTimeFrame Then
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

                            Dim eventCategoryIds As ArrayList = New ArrayList()
                            For Each lstEvent As EventInfo In lstEvents
                                eventCategoryIds.Add(lstEvent.Category)
                            Next
                            For Each lstCategory As EventCategoryInfo In lstCategories
                                If Not eventCategoryIds.Contains(lstCategory.Category) Then
                                    arrCategories.Remove(lstCategory)
                                End If
                            Next
                        End If
                    End If

                    'Bind categories.
                    ddlCategories.DataSource = arrCategories
                    ddlCategories.DataBind()

                    If Settings.Enablecategories = EventModuleSettings.DisplayCategories.SingleSelect Then
                        ddlCategories.Items.Insert(0, New Telerik.Web.UI.RadComboBoxItem(Localization.GetString("AllCategories", LocalResourceFile), "-1"))
                        ddlCategories.SelectedIndex = 0
                    End If
                    ddlCategories.OnClientDropDownClosed = "function() { btnUpdateClick('" + btnUpdate.UniqueID + "','" + ddlCategories.ClientID + "');}"
                    ddlCategories.OnClientLoad = "function() { storeText('" + ddlCategories.ClientID + "');}"
                    If Settings.Enablecategories = EventModuleSettings.DisplayCategories.SingleSelect Then
                        For Each category As Integer In SelectedCategory
                            ddlCategories.SelectedIndex = ddlCategories.FindItemByValue(category.ToString).Index
                            Exit For
                        Next
                    Else
                        For Each category As Integer In SelectedCategory
                            For Each item As Telerik.Web.UI.RadComboBoxItem In ddlCategories.Items
                                If item.Value = category.ToString Then
                                    item.Checked = True
                                End If
                            Next
                        Next

                        If CInt(SelectedCategory.Item(0)) = -1 Then
                            For Each item As Telerik.Web.UI.RadComboBoxItem In ddlCategories.Items
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
            StoreCategories()

            ' Fire the CategorySelected event...
            Dim args As CommandEventArgs = New CommandEventArgs(SelectedCategory.ToString, Nothing)
            RaiseEvent CategorySelectedChanged(Me, args)
        End Sub

        Public Event CategorySelectedChanged(ByVal sender As Object, ByVal e As CommandEventArgs)

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
