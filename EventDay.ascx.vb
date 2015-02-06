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

    <DNNtc.ModuleControlProperties("Day", "Events Day", DNNtc.ControlType.View, "https://dnnevents.codeplex.com/documentation", True, True)> _
    Partial Class EventDay
        Inherits EventBase

#Region "Event Handlers"
        Private _selectedEvents As ArrayList

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
            Try
                SetupViewControls(EventIcons, EventIcons2, SelectCategory, SelectLocation)

                If Settings.Eventdaynewpage Then
                    SetTheme(pnlEventsModuleDay)
                    AddFacebookMetaTags()
                End If

                'Show header - or not
                If Settings.EventsListShowHeader = "Yes" Then
                    lstEvents.ShowHeader = True
                Else
                    lstEvents.ShowHeader = False
                End If

                If Page.IsPostBack = False Then
                    If Settings.EventsListShowHeader <> "No" Then
                        lstEvents.ShowHeader = True
                        Localization.LocalizeDataGrid(lstEvents, LocalResourceFile)
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
            Dim culture As CultureInfo = Threading.Thread.CurrentThread.CurrentCulture
            Dim startDate As DateTime   ' Start View Date Events Range
            Dim endDate As DateTime     ' End View Date Events Range
            Dim objEvent As EventInfo
            Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
            Dim editButtonVisible As Boolean = False

            ' Set Date Range
            Dim dDate As Date = SelectedDate.Date
            startDate = dDate.AddDays(-1)
            endDate = dDate.AddDays(1)

            ' Get Events/Sub-Calendar Events
            Dim getSubEvents As Boolean = Settings.MasterEvent
            _selectedEvents = objEventInfoHelper.GetEvents(startDate, endDate, getSubEvents, SelectCategory.SelectedCategory, SelectLocation.SelectedLocation, GetUrlGroupId, GetUrlUserId)

            _selectedEvents = objEventInfoHelper.ConvertEventListToDisplayTimeZone(_selectedEvents, GetDisplayTimeZoneId())

            If _selectedEvents.Count = 0 Then
                lstEvents.Visible = False
                divMessage.Visible = True
                Exit Sub
            Else
                lstEvents.Visible = True
                divMessage.Visible = False
            End If

            ' Get Date Events (used for Multiday event)
            Dim dayEvents As ArrayList
            dayEvents = objEventInfoHelper.GetDateEvents(_selectedEvents, dDate)

            Dim fmtEventTimeBegin As String = Settings.Templates.txtDayEventTimeBegin
            If fmtEventTimeBegin = "" Then fmtEventTimeBegin = "g"

            Dim fmtEventTimeEnd As String = Settings.Templates.txtDayEventTimeEnd
            If fmtEventTimeEnd = "" Then fmtEventTimeEnd = "g"

            Dim tmpDayDescription As String = Settings.Templates.txtDayEventDescription
            Dim tmpDayLocation As String = Settings.Templates.txtDayLocation

            If Settings.Eventtooltipday Then
                toolTipManager.TargetControls.Clear()
            End If

            Dim colEvents As New ArrayList
            Dim lstEvent As EventListObject
            For Each objEvent In dayEvents
                ' If full enrollments should be hidden, ignore
                If HideFullEvent(objEvent) Then
                    Continue For
                End If

                Dim blAddEvent As Boolean = True
                If Settings.Collapserecurring Then
                    For Each lstEvent In colEvents
                        If lstEvent.RecurMasterID = objEvent.RecurMasterID Then
                            blAddEvent = False
                        End If
                    Next
                End If
                If blAddEvent Then
                    Dim objCtlEventRecurMaster As New EventRecurMasterController
                    Dim tcc As New TokenReplaceControllerClass(ModuleId, LocalResourceFile)
                    Dim fmtRowEnd, fmtRowBegin As String
                    fmtRowEnd = tcc.TokenParameters(fmtEventTimeEnd, objEvent, Settings)
                    fmtRowBegin = tcc.TokenParameters(fmtEventTimeBegin, objEvent, Settings)

                    lstEvent = New EventListObject
                    lstEvent.EventID = objEvent.EventID
                    lstEvent.CreatedByID = objEvent.CreatedByID
                    lstEvent.OwnerID = objEvent.OwnerID
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
                        templatedescr = tcc.TokenReplaceEvent(objEvent, tmpDayDescription, Nothing, False, isEvtEditor)
                        lstEvent.CategoryColor = GetColor(objEvent.Color)
                        lstEvent.CategoryFontColor = GetColor(objEvent.FontColor)

                        iconString = CreateIconString(objEvent, Settings.IconListPrio, Settings.IconListRec, Settings.IconListReminder, Settings.IconListEnroll)
                    End If

                    lstEvent.EventName = CreateEventName(objEvent, "[event:title]")
                    lstEvent.EventDesc = objEvent.EventDesc
                    ' RWJS - not sure why replace ' with \' - lstEvent.DecodedDesc = System.Web.HttpUtility.HtmlDecode(objEvent.EventDesc).Replace(Environment.NewLine, "").Trim.Replace("'", "\'")
                    lstEvent.DecodedDesc = HttpUtility.HtmlDecode(templatedescr).Replace(Environment.NewLine, "")
                    lstEvent.EventID = objEvent.EventID
                    lstEvent.ModuleID = objEvent.ModuleID

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
                    lstEvent.LocationName = tcc.TokenReplaceEvent(objEvent, tmpDayLocation)
                    lstEvent.CustomField1 = objEvent.CustomField1
                    lstEvent.CustomField2 = objEvent.CustomField2
                    lstEvent.RecurMasterID = objEvent.RecurMasterID

                    If Settings.Eventtooltipday Then
                        lstEvent.Tooltip = ToolTipCreate(objEvent, Settings.Templates.txtTooltipTemplateTitle, Settings.Templates.txtTooltipTemplateBody, isEvtEditor)
                    End If

                    lstEvent.EditVisibility = False
                    If isEvtEditor Then
                        lstEvent.EditVisibility = True
                        editButtonVisible = True
                    End If

                    colEvents.Add(lstEvent)
                End If
            Next

            'Determine which fields get displayed
            If Not IsPrivateNotModerator Then
                If Settings.EventsListFields.LastIndexOf("EB", StringComparison.Ordinal) < 0 Or editButtonVisible = False Then
                    lstEvents.Columns.Item(0).Visible = False
                Else
                    lstEvents.Columns.Item(0).Visible = True
                End If
                If Settings.EventsListFields.LastIndexOf("BD", StringComparison.Ordinal) < 0 Then
                    lstEvents.Columns.Item(1).Visible = False
                End If
                If Settings.EventsListFields.LastIndexOf("ED", StringComparison.Ordinal) < 0 Then
                    lstEvents.Columns.Item(2).Visible = False
                End If
                If Settings.EventsListFields.LastIndexOf("EN", StringComparison.Ordinal) < 0 Then
                    lstEvents.Columns.Item(3).Visible = False
                End If
                If Settings.EventsListFields.LastIndexOf("IM", StringComparison.Ordinal) < 0 Then
                    lstEvents.Columns.Item(4).Visible = False
                End If
                If Settings.EventsListFields.LastIndexOf("DU", StringComparison.Ordinal) < 0 Then
                    lstEvents.Columns.Item(5).Visible = False
                End If
                If Settings.EventsListFields.LastIndexOf("CA", StringComparison.Ordinal) < 0 Then
                    lstEvents.Columns.Item(6).Visible = False
                End If
                If Settings.EventsListFields.LastIndexOf("LO", StringComparison.Ordinal) < 0 Then
                    lstEvents.Columns.Item(7).Visible = False
                End If
                If Not Settings.EventsCustomField1 Or Settings.EventsListFields.LastIndexOf("C1", StringComparison.Ordinal) < 0 Then
                    lstEvents.Columns.Item(8).Visible = False
                End If
                If Not Settings.EventsCustomField2 Or Settings.EventsListFields.LastIndexOf("C2", StringComparison.Ordinal) < 0 Then
                    lstEvents.Columns.Item(9).Visible = False
                End If
                If Settings.EventsListFields.LastIndexOf("DE", StringComparison.Ordinal) < 0 Then
                    lstEvents.Columns.Item(10).Visible = False
                End If
                If Settings.EventsListFields.LastIndexOf("RT", StringComparison.Ordinal) < 0 Then
                    lstEvents.Columns.Item(11).Visible = False
                End If
                If Settings.EventsListFields.LastIndexOf("RU", StringComparison.Ordinal) < 0 Then
                    lstEvents.Columns.Item(12).Visible = False
                End If
            Else
                ' Set Defaults
                lstEvents.Columns.Item(0).Visible = False ' Edit Buttom
                lstEvents.Columns.Item(1).Visible = True  ' Begin Date
                lstEvents.Columns.Item(2).Visible = True  ' End Date
                lstEvents.Columns.Item(3).Visible = True  ' Title
                lstEvents.Columns.Item(4).Visible = False ' Image
                lstEvents.Columns.Item(5).Visible = False ' Duration
                lstEvents.Columns.Item(6).Visible = False ' Category
                lstEvents.Columns.Item(7).Visible = False ' Location
                lstEvents.Columns.Item(8).Visible = False ' Custom Field 1
                lstEvents.Columns.Item(9).Visible = False ' Custom Field 2
                lstEvents.Columns.Item(10).Visible = False ' Description
                lstEvents.Columns.Item(11).Visible = False ' Recurrence Pattern
                lstEvents.Columns.Item(12).Visible = False ' Recur Until
            End If

            lstEvents.DataSource = colEvents
            lstEvents.DataBind()
        End Sub
#End Region

#Region "Control Events"
        Private Sub lstEvents_ItemDataBound(ByVal sender As Object, ByVal e As DataGridItemEventArgs) Handles lstEvents.ItemDataBound
            If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim lnkevent As HyperLink = CType(e.Item.FindControl("lnkEvent"), HyperLink)
                If Settings.Eventtooltipday Then
                    Dim tooltip As String = CType(DataBinder.Eval(e.Item.DataItem, "Tooltip"), String)
                    e.Item.Attributes.Add("title", tooltip)
                    toolTipManager.TargetControls.Add(e.Item.ClientID, True)
                End If
                Dim backColor As Color = CType(DataBinder.Eval(e.Item.DataItem, "CategoryColor"), Color)
                If backColor.Name <> "0" Then
                    For i As Integer = 0 To e.Item.Cells.Count - 1
                        If e.Item.Cells(i).Visible And Not lstEvents.Columns(i).SortExpression = "Description" Then
                            e.Item.Cells(i).BackColor = backColor
                        End If
                    Next
                End If
                If IsPrivateNotModerator And Not UserId = CType(DataBinder.Eval(e.Item.DataItem, "OwnerID"), Integer) Then
                    lnkevent.Style.Add("cursor", "text")
                    lnkevent.Style.Add("text-decoration", "none")
                    lnkevent.Attributes.Add("onclick", "javascript:return false;")
                End If
            End If
        End Sub

        Private Sub lstEvents_ItemCommand(ByVal source As Object, ByVal e As DataGridCommandEventArgs) Handles lstEvents.ItemCommand
            Try
                Select Case e.CommandName
                    Case "Edit"
                        'set selected row editable
                        Dim iItemID As Integer = CType(e.CommandArgument, Integer)
                        Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
                        Response.Redirect(objEventInfoHelper.GetEditURL(iItemID, GetUrlGroupId, GetUrlUserId))
                End Select
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
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

        Private Sub returnButton_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles returnButton.Click
            Response.Redirect(GetSocialNavigateUrl(), True)
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
