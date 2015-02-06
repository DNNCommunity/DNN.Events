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
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Globalization

Namespace DotNetNuke.Modules.Events

    Partial Class EventList
        Inherits EventBase

#Region "Event Handlers"
        Private _selectedEvents As ArrayList


        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
            Try
                SetupViewControls(EventIcons, EventIcons2, SelectCategory, SelectLocation)

                gvEvents.PageSize = Settings.EventsListPageSize

                If Page.IsPostBack = False Then
                    If Settings.EventsListShowHeader <> "No" Then
                        gvEvents.ShowHeader = True
                        Localization.LocalizeGridView(gvEvents, LocalResourceFile)
                    End If
                    BindDataGrid()
                End If
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub
#End Region

#Region "Helper Functions"
        Private Sub BindDataGrid()
            'Default sort from settings
            Dim sortDirection As SortDirection
            If Settings.EventsListSortDirection = "ASC" Then
                sortDirection = sortDirection.Ascending
            Else
                sortDirection = sortDirection.Descending
            End If

            Dim sortExpression As EventListObject.SortFilter = GetListSortExpression(Settings.EventsListSortColumn)

            'Show header - or not
            If Settings.EventsListShowHeader = "Yes" Then
                gvEvents.ShowHeader = True
            Else
                gvEvents.ShowHeader = False
            End If

            'Get cached sort settings
            If Not ViewState("SortExpression") Is Nothing AndAlso TypeOf (ViewState("SortExpression")) Is EventListObject.SortFilter Then
                sortExpression = CType(ViewState("SortExpression"), EventListObject.SortFilter)
            End If
            If Not ViewState("SortDirection") Is Nothing AndAlso TypeOf (ViewState("SortDirection")) Is SortDirection Then
                sortDirection = CType(ViewState("SortDirection"), SortDirection)
            End If

            BindDataGrid(sortExpression, sortDirection)
        End Sub

        Private Sub BindDataGrid(ByVal sortExpression As EventListObject.SortFilter, ByVal sortDirection As SortDirection)
            Dim culture As CultureInfo = Threading.Thread.CurrentThread.CurrentCulture
            Dim objEvent As EventInfo
            Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
            Dim editColumnVisible As Boolean = False

            ' Get Events/Sub-Calendar Events
            _selectedEvents = Get_ListView_Events(SelectCategory.SelectedCategory, SelectLocation.SelectedLocation)

            Dim fmtEventTimeBegin As String = Settings.Templates.txtListEventTimeBegin
            If fmtEventTimeBegin = "" Then fmtEventTimeBegin = "g"

            Dim fmtEventTimeEnd As String = Settings.Templates.txtListEventTimeEnd
            If fmtEventTimeEnd = "" Then fmtEventTimeEnd = "g"

            Dim tmpListDescription As String = Settings.Templates.txtListEventDescription
            Dim tmpListLocation As String = Settings.Templates.txtListLocation

            If _selectedEvents.Count = 0 Then
                gvEvents.Visible = False
                divNoEvents.Visible = True
                Exit Sub
            Else
                gvEvents.Visible = True
                divNoEvents.Visible = False
            End If

            If Settings.Eventtooltiplist Then
                toolTipManager.TargetControls.Clear()
            End If

            ' if Events Selection Type only get the 1st N Events
            Dim colEvents As New ArrayList
            Dim lstEvent As EventListObject
            Dim indexID As Integer = 0
            For Each objEvent In _selectedEvents
                Dim tcc As New TokenReplaceControllerClass(ModuleId, LocalResourceFile)
                Dim objCtlEventRecurMaster As New EventRecurMasterController
                Dim fmtRowEnd, fmtRowBegin As String
                fmtRowEnd = tcc.TokenParameters(fmtEventTimeEnd, objEvent, Settings)
                fmtRowBegin = tcc.TokenParameters(fmtEventTimeBegin, objEvent, Settings)

                lstEvent = New EventListObject
                lstEvent.EventID = objEvent.EventID
                lstEvent.CreatedByID = objEvent.CreatedByID
                lstEvent.OwnerID = objEvent.OwnerID
                lstEvent.IndexId = indexID
                ' Get Dates (automatically converted to User's Timezone)
                lstEvent.EventDateBegin = objEvent.EventTimeBegin
                lstEvent.EventDateEnd = objEvent.EventTimeEnd
                If objEvent.DisplayEndDate Then
                    lstEvent.TxtEventDateEnd = String.Format("{0:" + fmtRowEnd + "}", lstEvent.EventDateEnd)
                Else
                    lstEvent.TxtEventDateEnd = ""
                End If
                lstEvent.EventTimeBegin = objEvent.EventTimeBegin
                lstEvent.TxtEventTimeBegin = String.Format("{0:" + fmtRowBegin + "}", lstEvent.EventTimeBegin)
                lstEvent.Duration = objEvent.Duration

                Dim isEvtEditor As Boolean = IsEventEditor(objEvent, False)

                Dim templatedescr As String = ""
                Dim iconString As String = ""

                If Not IsPrivateNotModerator Or UserId = objEvent.OwnerID Then
                    templatedescr = tcc.TokenReplaceEvent(objEvent, tmpListDescription, Nothing, False, isEvtEditor)
                    lstEvent.CategoryColor = GetColor(objEvent.Color)
                    lstEvent.CategoryFontColor = GetColor(objEvent.FontColor)

                    iconString = CreateIconString(objEvent, Settings.IconListPrio, Settings.IconListRec, Settings.IconListReminder, Settings.IconListEnroll)
                End If

                lstEvent.EventName = CreateEventName(objEvent, "[event:title]")
                lstEvent.EventDesc = objEvent.EventDesc
                ' RWJS - not sure why replace ' with \' - lstEvent.DecodedDesc = System.Web.HttpUtility.HtmlDecode(objEvent.EventDesc).Replace(Environment.NewLine, "").Trim.Replace("'", "\'")
                lstEvent.DecodedDesc = HttpUtility.HtmlDecode(templatedescr).Replace(Environment.NewLine, "")

                Dim objEventRRULE As EventRRULEInfo
                objEventRRULE = objCtlEventRecurMaster.DecomposeRRULE(objEvent.RRULE, objEvent.EventTimeBegin)
                lstEvent.RecurText = objCtlEventRecurMaster.RecurrenceText(objEventRRULE, LocalResourceFile, culture, objEvent.EventTimeBegin)
                If objEvent.RRULE <> "" Then
                    lstEvent.RecurUntil = objEvent.LastRecurrence.ToShortDateString
                Else
                    lstEvent.RecurUntil = ""
                End If
                lstEvent.EventID = objEvent.EventID
                lstEvent.ModuleID = objEvent.ModuleID

                lstEvent.ImageURL = ""
                If Settings.Eventimage And objEvent.ImageURL <> Nothing And objEvent.ImageDisplay = True Then
                    lstEvent.ImageURL = ImageInfo(objEvent.ImageURL, objEvent.ImageHeight, objEvent.ImageWidth)
                End If


                ' Get detail page url
                lstEvent.URL = objEventInfoHelper.DetailPageURL(objEvent)
                If objEvent.DetailPage And objEvent.DetailNewWin Then
                    lstEvent.Target = "_blank"
                End If

                lstEvent.Icons = iconString
                lstEvent.DisplayDuration = CType(Int(objEvent.Duration / 1440 + 1), Integer)
                lstEvent.CategoryName = objEvent.CategoryName
                lstEvent.LocationName = tcc.TokenReplaceEvent(objEvent, tmpListLocation)
                lstEvent.CustomField1 = objEvent.CustomField1
                lstEvent.CustomField2 = objEvent.CustomField2
                lstEvent.RecurMasterID = objEvent.RecurMasterID

                If Settings.Eventtooltiplist Then
                    lstEvent.Tooltip = ToolTipCreate(objEvent, Settings.Templates.txtTooltipTemplateTitle, Settings.Templates.txtTooltipTemplateBody, isEvtEditor)
                End If

                lstEvent.EditVisibility = False
                If isEvtEditor Then
                    lstEvent.EditVisibility = True
                    editColumnVisible = True
                End If

                colEvents.Add(lstEvent)
                indexID += 1
            Next

            'Determine which fields get displayed
            If (Not IsPrivateNotModerator) Then
                If Settings.EventsListFields.LastIndexOf("EB", StringComparison.Ordinal) < 0 Or editColumnVisible = False Then
                    gvEvents.Columns.Item(0).Visible = False
                Else
                    gvEvents.Columns.Item(0).Visible = True
                End If
                If Settings.EventsListFields.LastIndexOf("BD", StringComparison.Ordinal) < 0 Then
                    gvEvents.Columns.Item(1).Visible = False
                End If
                If Settings.EventsListFields.LastIndexOf("ED", StringComparison.Ordinal) < 0 Then
                    gvEvents.Columns.Item(2).Visible = False
                End If
                If Settings.EventsListFields.LastIndexOf("EN", StringComparison.Ordinal) < 0 Then
                    gvEvents.Columns.Item(3).Visible = False
                End If
                If Settings.EventsListFields.LastIndexOf("IM", StringComparison.Ordinal) < 0 Then
                    gvEvents.Columns.Item(4).Visible = False
                End If
                If Settings.EventsListFields.LastIndexOf("DU", StringComparison.Ordinal) < 0 Then
                    gvEvents.Columns.Item(5).Visible = False
                End If
                If Settings.EventsListFields.LastIndexOf("CA", StringComparison.Ordinal) < 0 Then
                    gvEvents.Columns.Item(6).Visible = False
                End If
                If Settings.EventsListFields.LastIndexOf("LO", StringComparison.Ordinal) < 0 Then
                    gvEvents.Columns.Item(7).Visible = False
                End If
                If Not Settings.EventsCustomField1 Or Settings.EventsListFields.LastIndexOf("C1", StringComparison.Ordinal) < 0 Then
                    gvEvents.Columns.Item(8).Visible = False
                End If
                If Not Settings.EventsCustomField2 Or Settings.EventsListFields.LastIndexOf("C2", StringComparison.Ordinal) < 0 Then
                    gvEvents.Columns.Item(9).Visible = False
                End If
                If Settings.EventsListFields.LastIndexOf("DE", StringComparison.Ordinal) < 0 Then
                    gvEvents.Columns.Item(10).Visible = False
                End If
                If Settings.EventsListFields.LastIndexOf("RT", StringComparison.Ordinal) < 0 Then
                    gvEvents.Columns.Item(11).Visible = False
                End If
                If Settings.EventsListFields.LastIndexOf("RU", StringComparison.Ordinal) < 0 Then
                    gvEvents.Columns.Item(12).Visible = False
                End If
            Else
                ' Set Defaults
                gvEvents.Columns.Item(0).Visible = False ' Edit Buttom
                gvEvents.Columns.Item(1).Visible = True  ' Begin Date
                gvEvents.Columns.Item(2).Visible = True  ' End Date
                gvEvents.Columns.Item(3).Visible = True  ' Title
                gvEvents.Columns.Item(4).Visible = False ' Image
                gvEvents.Columns.Item(5).Visible = False ' Duration
                gvEvents.Columns.Item(6).Visible = False ' Category
                gvEvents.Columns.Item(7).Visible = False ' Location
                gvEvents.Columns.Item(8).Visible = False ' Custom Field 1
                gvEvents.Columns.Item(9).Visible = False ' Custom Field 2
                gvEvents.Columns.Item(10).Visible = False ' Description
                gvEvents.Columns.Item(11).Visible = False ' Recurrence Pattern
                gvEvents.Columns.Item(12).Visible = False ' Recur Until
            End If

            EventListObject.SortExpression = sortExpression
            EventListObject.SortDirection = sortDirection
            colEvents.Sort()

            gvEvents.DataKeyNames = New String() {"IndexId", "EventID", "EventDateBegin"}
            gvEvents.DataSource = colEvents
            gvEvents.DataBind()
        End Sub

#End Region

#Region "Control Events"
        Sub gvEvents_RowCreated(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles gvEvents.RowCreated

            If e.Row.RowType = DataControlRowType.DataRow Then
                If Settings.Eventtooltiplist Then
                    Dim tooltip As String = CType(DataBinder.Eval(e.Row.DataItem, "Tooltip"), String)
                    e.Row.Attributes.Add("title", tooltip)
                End If
                Dim backColor As Color = CType(DataBinder.Eval(e.Row.DataItem, "CategoryColor"), Color)
                If backColor.Name <> "0" Then
                    For i As Integer = 0 To e.Row.Cells.Count - 1
                        If e.Row.Cells(i).Visible And Not gvEvents.Columns(i).SortExpression = "Description" Then
                            e.Row.Cells(i).BackColor = backColor
                        End If
                    Next
                End If
                If IsPrivateNotModerator And Not UserId = CType(DataBinder.Eval(e.Row.DataItem, "OwnerID"), Integer) Then
                    Dim lnkevent As HyperLink = CType(e.Row.FindControl("lnkEvent"), HyperLink)
                    lnkevent.Style.Add("cursor", "text")
                    lnkevent.Style.Add("text-decoration", "none")
                    lnkevent.Attributes.Add("onclick", "javascript:return false;")
                End If
            End If
        End Sub

        Private Sub gvEvents_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles gvEvents.RowDataBound
            If e.Row.RowType = DataControlRowType.DataRow And Settings.Eventtooltiplist Then
                toolTipManager.TargetControls.Add(e.Row.ClientID, True)
            End If
        End Sub

        Private Sub SelectCategory_CategorySelected(ByVal sender As Object, ByVal e As CommandEventArgs) Handles SelectCategory.CategorySelectedChanged
            'Store the other selection(s) too.
            SelectLocation.StoreLocations()
            BindDataGrid()
        End Sub
        Private Sub SelectLocation_LocationSelected(ByVal sender As Object, ByVal e As CommandEventArgs) Handles SelectLocation.LocationSelectedChanged
            'Store the other selection(s) too.
            SelectCategory.StoreCategories()
            BindDataGrid()
        End Sub

        Private Sub gvEvents_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles gvEvents.PageIndexChanging
            'Set page index
            gvEvents.PageIndex = e.NewPageIndex

            'Binddata
            BindDataGrid()
        End Sub

        Private Sub gvEvents_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvEvents.RowCommand
            Select Case e.CommandName
                Case "Edit"
                    Dim iItemID As Integer = CType(e.CommandArgument, Integer)
                    'set selected row editable
                    Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
                    Response.Redirect(objEventInfoHelper.GetEditURL(iItemID, GetUrlGroupId, GetUrlUserId))
            End Select
        End Sub

        Private Sub gvEvents_Sorting(ByVal sender As Object, ByVal e As GridViewSortEventArgs) Handles gvEvents.Sorting
            'Get the sort expression
            Dim sortExpression As EventListObject.SortFilter = GetListSortExpression(e.SortExpression)

            'HACK Change sortdirection
            Dim sortDirection As SortDirection = e.SortDirection
            If Not ViewState("SortExpression") Is Nothing AndAlso sortExpression = CType(ViewState("SortExpression"), EventListObject.SortFilter) Then
                If CType(ViewState("SortDirection"), SortDirection) = sortDirection.Ascending Then
                    sortDirection = sortDirection.Descending
                Else
                    sortDirection = sortDirection.Ascending
                End If
            End If

            'Cache direction en expression
            ViewState("SortExpression") = sortExpression
            ViewState("SortDirection") = sortDirection

            'Binddata
            BindDataGrid(sortExpression, sortDirection)
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

    End Class
End Namespace