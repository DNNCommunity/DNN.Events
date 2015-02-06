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
Imports DotNetNuke.Services.Localization
Imports System.Globalization
Imports DotNetNuke.Modules.Events.ScheduleControl.MonthControl
Imports DotNetNuke.Framework.JavaScriptLibraries

Namespace DotNetNuke.Modules.Events
    Partial Class EventMonth
        Inherits EventBase

#Region "Private Variables"

        Private _pageBound As Boolean = False
        Private _selectedEvents As ArrayList
        Private ReadOnly _culture As CultureInfo = Threading.Thread.CurrentThread.CurrentCulture
        Private _objEventInfoHelper As EventInfoHelper

#End Region

#Region "Event Handlers"

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.PreRender
            ' This handles the case where the same cell date is selected twice
            If Not _pageBound Then
                BindDataGrid()
            End If
        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
            Try

                ' Be sure to load the required scripts always
                JavaScript.RequestRegistration(CommonJs.DnnPlugins)

                LocalizeAll()

                SetupViewControls(EventIcons, EventIcons2, SelectCategory, SelectLocation, pnlDateControls)

                dpGoToDate.SelectedDate = SelectedDate.Date()
                dpGoToDate.Calendar.FirstDayOfWeek = Settings.WeekStart

                ' Set Weekend Display
                If Settings.Fridayweekend Then
                    EventCalendar.WeekEndDays = MyDayOfWeek.Friday Or MyDayOfWeek.Saturday
                End If

                ' Set 1st Day of Week
                EventCalendar.FirstDayOfWeek = CType(_culture.DateTimeFormat.FirstDayOfWeek, System.Web.UI.WebControls.FirstDayOfWeek)

                If Settings.WeekStart <> System.Web.UI.WebControls.FirstDayOfWeek.Default Then
                    EventCalendar.FirstDayOfWeek = Settings.WeekStart
                End If

                ' if 1st time on page...
                If Not Page.IsPostBack Then
                    EventCalendar.VisibleDate = CType(dpGoToDate.SelectedDate, Date)
                    If Not Settings.Monthcellnoevents Then
                        EventCalendar.SelectedDate = EventCalendar.VisibleDate
                    End If
                    BindDataGrid()
                End If
            Catch exc As Exception 'Module failed to load
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

#Region "Helper Methods & Functions"

        Private Sub LocalizeAll()

            lnkToday.Text = Localization.GetString("lnkToday", LocalResourceFile)
            dpGoToDate.DatePopupButton.ToolTip = Localization.GetString("DatePickerTooltip", LocalResourceFile)

        End Sub

        Private Sub BindDataGrid()

            Dim startDate As DateTime   ' Start View Date Events Range
            Dim endDate As DateTime     ' End View Date Events Range
            Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)

            _pageBound = True
            '****DO NOT CHANGE THE NEXT SECTION FOR ML CODING ****
            ' Used Only to select view dates on Event Month View...
            Dim useDate As Date = CType(dpGoToDate.SelectedDate, Date)
            Dim initDate As Date = New Date(useDate.Year, useDate.Month, 1)
            startDate = initDate.AddDays(-10) ' Allow for Prev Month days in View
            ' Load 2 months of events.  This used to load only the events for the current month,
            ' but was changed so that events for multiple events can be displayed in the case when
            ' the Event displays some days for the next month.
            endDate = initDate.AddMonths(1).AddDays(10)

            Dim getSubEvents As Boolean = Settings.MasterEvent
            _selectedEvents = objEventInfoHelper.GetEvents(startDate, endDate, getSubEvents, SelectCategory.SelectedCategory, SelectLocation.SelectedLocation, GetUrlGroupId, GetUrlUserId)

            _selectedEvents = objEventInfoHelper.ConvertEventListToDisplayTimeZone(_selectedEvents, GetDisplayTimeZoneId())

            'Write current date to UI
            SelectedDate = EventCalendar.VisibleDate

            ' Setup the Tooltip TargetControls because it doesn't work in DayRender!
            If Settings.Eventtooltipmonth Then
                toolTipManager.TargetControls.Clear()
                If Settings.Monthcellnoevents Then
                    Dim calcDate As DateTime = startDate
                    Do While calcDate <= endDate
                        toolTipManager.TargetControls.Add("ctlEvents_Mod_" & ModuleId.ToString & "_EventDate_" & calcDate.Date.ToString("yyyyMMMdd"), True)
                        calcDate = calcDate.AddDays(1)
                    Loop
                Else
                    For Each objEvent As EventInfo In _selectedEvents
                        Dim calcDate As DateTime = objEvent.EventTimeBegin.Date
                        Do While calcDate <= objEvent.EventTimeEnd.Date
                            toolTipManager.TargetControls.Add("ctlEvents_Mod_" & ModuleId.ToString & "_EventID_" & objEvent.EventID.ToString & "_EventDate_" & calcDate.Date.ToString("yyyyMMMdd"), True)
                            calcDate = calcDate.AddDays(1)
                        Loop
                    Next
                End If

            End If
        End Sub

#End Region

#Region "Event Event Grid Methods and Functions"

        ''' <summary>
        ''' Render each day in the event (i.e. Cells)
        ''' </summary>
        Private Sub EventCalendar_DayRender(ByVal sender As System.Object, ByVal e As System.Web.UI.WebControls.DayRenderEventArgs) Handles EventCalendar.DayRender
            Dim objEvent As EventInfo
            Dim cellcontrol As New LiteralControl
            _objEventInfoHelper = New EventInfoHelper(ModuleId, TabId, PortalId, Settings)

            ' Get Events/Sub-Calendar Events
            Dim dayEvents As New ArrayList
            Dim allDayEvents As ArrayList
            allDayEvents = _objEventInfoHelper.GetDateEvents(_selectedEvents, e.Day.Date)
            allDayEvents.Sort(New EventInfoHelper.EventDateSort)

            For Each objEvent In allDayEvents
                'if day not in current (selected) Event month OR full enrollments should be hidden, ignore
                If (Settings.ShowEventsAlways Or e.Day.Date.Month = SelectedDate.Month) _
                   And Not HideFullEvent(objEvent) Then
                    dayEvents.Add(objEvent)
                End If
            Next

            ' If No Cell Event Display...
            If Settings.Monthcellnoevents Then
                If (Settings.ShowEventsAlways = False And e.Day.IsOtherMonth) Then
                    e.Cell.Text = ""
                    Exit Sub
                End If

                If dayEvents.Count > 0 Then
                    e.Day.IsSelectable = True

                    If e.Day.Date = SelectedDate Then
                        e.Cell.CssClass = "EventSelectedDay"
                    Else
                        If e.Day.IsWeekend Then
                            e.Cell.CssClass = "EventWeekendDayEvents"
                        Else
                            e.Cell.CssClass = "EventDayEvents"
                        End If
                    End If

                    If Settings.Eventtooltipmonth Then
                        Dim themeCss As String = GetThemeSettings().CssClass

                        Dim tmpToolTipTitle As String = Settings.Templates.txtTooltipTemplateTitleNT
                        If InStr(tmpToolTipTitle, "{0}") > 0 Then
                            tmpToolTipTitle = Replace(tmpToolTipTitle, "{0}", "{0:d}")
                        End If
                        Dim tooltipTitle As String = HttpUtility.HtmlDecode(String.Format(tmpToolTipTitle, e.Day.Date)).Replace(Environment.NewLine, "")
                        Dim cellToolTip As String = "" 'Holds control generated tooltip

                        For Each objEvent In dayEvents
                            'Add horizontal row to seperate the eventdescriptions
                            If cellToolTip <> "" Then cellToolTip = cellToolTip + "<hr/>"

                            cellToolTip &= CreateEventName(objEvent, Settings.Templates.txtTooltipTemplateBodyNT.Replace(vbLf, "").Replace(vbCr, ""))
                        Next
                        e.Cell.Attributes.Add("title", "<table class=""" & themeCss & " Eventtooltiptable""><tr><td class=""" & themeCss & " Eventtooltipheader"">" + tooltipTitle + "</td></tr><tr><td class=""" & themeCss & " Eventtooltipbody"">" + cellToolTip + "</td></tr></table>")
                        e.Cell.ID = "ctlEvents_Mod_" & ModuleId.ToString & "_EventDate_" & e.Day.Date.ToString("yyyyMMMdd")
                    End If

                    Dim dailyLink As New HyperLink
                    dailyLink.Text = String.Format(Settings.Templates.txtMonthDayEventCount, dayEvents.Count.ToString)
                    Dim socialGroupId As Integer = GetUrlGroupId()
                    Dim socialUserId As Integer = GetUrlUserId()
                    If dayEvents.Count > 1 Then
                        If Settings.Eventdaynewpage Then
                            If socialGroupId > 0 Then
                                dailyLink.NavigateUrl = _objEventInfoHelper.AddSkinContainerControls(NavigateURL(TabId, "Day", "Mid=" & ModuleId.ToString, "selecteddate=" & Format(e.Day.Date, "yyyyMMdd"), "groupid=" & socialGroupId.ToString), "?")
                            ElseIf socialUserId > 0 Then
                                dailyLink.NavigateUrl = _objEventInfoHelper.AddSkinContainerControls(NavigateURL(TabId, "Day", "Mid=" & ModuleId.ToString, "selecteddate=" & Format(e.Day.Date, "yyyyMMdd"), "userid=" & socialUserId.ToString), "?")
                            Else
                                dailyLink.NavigateUrl = _objEventInfoHelper.AddSkinContainerControls(NavigateURL(TabId, "Day", "Mid=" & ModuleId.ToString, "selecteddate=" & Format(e.Day.Date, "yyyyMMdd")), "?")
                            End If
                        Else
                            If socialGroupId > 0 Then
                                dailyLink.NavigateUrl = NavigateURL(TabId, "", "ModuleID=" & ModuleId.ToString, "mctl=EventDay", "selecteddate=" & Format(e.Day.Date, "yyyyMMdd"), "groupid=" & socialGroupId.ToString)
                            ElseIf socialUserId > 0 Then
                                dailyLink.NavigateUrl = NavigateURL(TabId, "", "ModuleID=" & ModuleId.ToString, "mctl=EventDay", "selecteddate=" & Format(e.Day.Date, "yyyyMMdd"), "userid=" & socialUserId.ToString)
                            Else
                                dailyLink.NavigateUrl = NavigateURL(TabId, "", "ModuleID=" & ModuleId.ToString, "mctl=EventDay", "selecteddate=" & Format(e.Day.Date, "yyyyMMdd"))
                            End If
                        End If
                    Else
                        ' Get detail page url
                        dailyLink = GetDetailPageUrl(CType(dayEvents.Item(0), EventInfo), dailyLink)
                    End If
                    Using stringWrite As New IO.StringWriter
                        Using eventoutput As New HtmlTextWriter(stringWrite)
                            dailyLink.RenderControl(eventoutput)
                            cellcontrol.Text = "<div class='EventDayScroll'>" + stringWrite.ToString + "</div>"
                            e.Cell.Controls.Add(cellcontrol)
                        End Using
                    End Using
                Else
                    e.Day.IsSelectable = False
                End If
                Exit Sub
            End If

            'Make day unselectable
            If Not Settings.Monthdayselect Then
                e.Day.IsSelectable = False
            End If

            'loop through records and render if startDate = current day and is not null
            Dim celldata As String = ""  ' Holds Control Generated HTML

            For Each objEvent In dayEvents
                Dim dailyLink As New HyperLink
                Dim iconString As String = ""

                ' See if an Image is to be displayed for the Event
                If Settings.Eventimage And Settings.EventImageMonth _
                  And objEvent.ImageURL <> Nothing And objEvent.ImageDisplay = True Then
                    dailyLink.Text = ImageInfo(objEvent.ImageURL, objEvent.ImageHeight, objEvent.ImageWidth)
                End If

                If Settings.Timeintitle Then
                    dailyLink.Text = dailyLink.Text + objEvent.EventTimeBegin.ToString("t") + " - "
                End If

                Dim eventtext As String = CreateEventName(objEvent, Settings.Templates.txtMonthEventText)
                dailyLink.Text = dailyLink.Text + eventtext.Trim

                If Not IsPrivateNotModerator Or UserId = objEvent.OwnerID Then
                    dailyLink.ForeColor = GetColor(objEvent.FontColor)
                    iconString = CreateIconString(objEvent, Settings.IconMonthPrio, Settings.IconMonthRec, Settings.IconMonthReminder, Settings.IconMonthEnroll)

                    ' Get detail page url
                    dailyLink = GetDetailPageUrl(objEvent, dailyLink)
                Else
                    dailyLink.Style.Add("cursor", "text")
                    dailyLink.Style.Add("text-decoration", "none")
                    dailyLink.Attributes.Add("onclick", "javascript:return false;")
                End If

                ' See If Description Tooltip to be added
                If Settings.Eventtooltipmonth Then
                    Dim isEvtEditor As Boolean = IsEventEditor(objEvent, False)
                    dailyLink.Attributes.Add("title", ToolTipCreate(objEvent, Settings.Templates.txtTooltipTemplateTitle, Settings.Templates.txtTooltipTemplateBody, isEvtEditor))
                End If

                ' Capture Control Info & save
                Using stringWrite As New IO.StringWriter
                    Using eventoutput As New HtmlTextWriter(stringWrite)
                        dailyLink.ID = "ctlEvents_Mod_" & ModuleId.ToString & "_EventID_" & objEvent.EventID.ToString & "_EventDate_" & e.Day.Date.ToString("yyyyMMMdd")
                        dailyLink.RenderControl(eventoutput)
                        If objEvent.Color <> Nothing And (Not IsPrivateNotModerator Or UserId = objEvent.OwnerID) Then
                            celldata = celldata + "<div style=""background-color: " + objEvent.Color + ";"">" + iconString + stringWrite.ToString + "</div>"
                        Else
                            celldata = celldata + "<div>" + iconString + stringWrite.ToString + "</div>"
                        End If
                    End Using
                End Using
            Next

            ' Add Literal Control Data to Cell w/DIV tag (in order to support scrolling in cell)
            cellcontrol.Text = "<div class='EventDayScroll'>" + celldata + "</div>"
            e.Cell.Controls.Add(cellcontrol)
        End Sub


        Private Sub EventCalendar_SelectionChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles EventCalendar.SelectionChanged
            EventCalendar.VisibleDate = EventCalendar.SelectedDate
            SelectedDate = EventCalendar.SelectedDate.Date
            Dim urlDate As String = EventCalendar.SelectedDate.Date.ToShortDateString
            dpGoToDate.SelectedDate = SelectedDate().Date
            If Settings.Monthcellnoevents Then
                Try
                    EventCalendar.SelectedDate = New DateTime
                    If Settings.Eventdaynewpage Then
                        Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
                        Response.Redirect(objEventInfoHelper.AddSkinContainerControls(NavigateURL(TabId, "Day", "Mid=" & ModuleId.ToString, "selecteddate=" & urlDate), "&"))
                    Else
                        Response.Redirect(NavigateURL(TabId, "", "ModuleID=" & ModuleId.ToString, "mctl=EventDay", "selecteddate=" & urlDate))
                    End If
                Catch ex As Exception
                End Try
            Else
                'fill grid with current selection's data
                BindDataGrid()
            End If
        End Sub

        Private Sub EventCalendar_VisibleMonthChanged(ByVal sender As Object, ByVal e As MonthChangedEventArgs) Handles EventCalendar.VisibleMonthChanged
            'set selected date to first of month
            SelectedDate = e.NewDate
            dpGoToDate.SelectedDate = e.NewDate.Date()
            If Not Settings.Monthcellnoevents Then
                EventCalendar.SelectedDate = e.NewDate
            End If
            SelectCategory.StoreCategories()
            SelectLocation.StoreLocations()
            'bind datagrid
            BindDataGrid()
        End Sub

        Private Function GetDetailPageUrl(ByVal objevent As EventInfo, ByVal dailyLink As HyperLink) As HyperLink
            ' Get detail page url
            dailyLink.NavigateUrl = _objEventInfoHelper.DetailPageURL(objevent)
            If objevent.DetailPage And objevent.DetailNewWin Then
                dailyLink.Attributes.Add("target", "_blank")
            End If
            Return dailyLink
        End Function
#End Region

#Region "Links and Buttons"
        Private Sub lnkToday_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles lnkToday.Click
            'set grid uneditable
            SelectedDate = DateTime.Now.Date
            EventCalendar.VisibleDate = SelectedDate
            dpGoToDate.SelectedDate = SelectedDate.Date()
            If Not Settings.Monthcellnoevents Then
                EventCalendar.SelectedDate = SelectedDate
            End If
            SelectCategory.StoreCategories()
            SelectLocation.StoreLocations()
            'fill grid with current selection's data
            BindDataGrid()
        End Sub

        Private Sub SelectCategoryChanged(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs) Handles SelectCategory.CategorySelectedChanged
            'Store the other selection(s) too.
            SelectLocation.StoreLocations()
            BindDataGrid()
        End Sub
        Private Sub SelectLocationChanged(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs) Handles SelectLocation.LocationSelectedChanged
            'Store the other selection(s) too.
            SelectCategory.StoreCategories()
            BindDataGrid()
        End Sub
        Private Sub dpGoToDate_SelectedDateChanged(sender As Object, e As Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs) Handles dpGoToDate.SelectedDateChanged
            Dim dDate As Date = CType(dpGoToDate.SelectedDate, Date)
            SelectedDate = dDate
            EventCalendar.VisibleDate = dDate
            If Not Settings.Monthcellnoevents Then
                EventCalendar.SelectedDate = dDate
            End If
            'fill grid with current selection's data
            BindDataGrid()
        End Sub

#End Region

    End Class

End Namespace
