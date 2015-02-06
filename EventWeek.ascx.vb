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
Imports System.Globalization
Imports System.Web.UI.WebControls
Imports DotNetNuke.Modules.Events.ScheduleControl


Namespace DotNetNuke.Modules.Events

    Partial Class EventWeek
        Inherits EventBase


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

#Region "Private Variables"
        Private _dWeekStart As Date
        Private _selectedEvents As ArrayList
        Private ReadOnly _culture As CultureInfo = Threading.Thread.CurrentThread.CurrentCulture
#End Region

#Region "Page Events"
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
            Try
                LocalizeAll()

                SetupViewControls(EventIcons, EventIcons2, SelectCategory, SelectLocation, pnlDateControls)
                Dim initDate As Date = SelectedDate.Date()
                dpGoToDate.SelectedDate = initDate
                dpGoToDate.Calendar.FirstDayOfWeek = Settings.WeekStart

                If Not IsPostBack Then
                    schWeek.StartDay = _culture.DateTimeFormat.FirstDayOfWeek
                    If Settings.WeekStart <> System.Web.UI.WebControls.FirstDayOfWeek.Default Then
                        schWeek.StartDay = CType(Settings.WeekStart, DayOfWeek)
                    End If
                    If Settings.Fulltimescale Then
                        schWeek.FullTimeScale = True
                    End If
                    If Settings.Includeendvalue Then
                        schWeek.IncludeEndValue = True
                    Else
                        schWeek.IncludeEndValue = False
                    End If
                    If Settings.Showvaluemarks Then
                        schWeek.ShowValueMarks = True
                    Else
                        schWeek.ShowValueMarks = False
                    End If
                    BindPage(initDate)
                End If
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub
#End Region

#Region "Helper Methods"
        Private Sub LocalizeAll()
            lnkToday.Text = Localization.GetString("lnkToday", LocalResourceFile)
            dpGoToDate.DatePopupButton.ToolTip = Localization.GetString("DatePickerTooltip", LocalResourceFile)
        End Sub

        Private Sub BindPage(ByVal dDate As DateTime)
            Dim dBegin, dEnd, sBegin, sEnd As Date
            Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)

            Try
                ' Set Date Range
                If Settings.WeekStart <> System.Web.UI.WebControls.FirstDayOfWeek.Default Then
                    _dWeekStart = dDate.AddDays(-dDate.DayOfWeek + Settings.WeekStart)
                Else
                    _dWeekStart = dDate.AddDays(-dDate.DayOfWeek)
                End If
                If dDate.DayOfWeek < Settings.WeekStart And Settings.WeekStart <> FirstDayOfWeek.Default Then
                    _dWeekStart = _dWeekStart.AddDays(-7)
                End If
                lblWeekOf.Text = String.Format(Localization.GetString("capWeekEvent", LocalResourceFile), DatePart(DateInterval.WeekOfYear, _dWeekStart, , FirstWeekOfYear.FirstFourDays), _dWeekStart.ToLongDateString)
                ViewState(ModuleId & "WeekOf") = _dWeekStart.ToShortDateString

                ' Allow 7 days for events that might start before beginning of week
                sBegin = _dWeekStart
                dBegin = DateAdd(DateInterval.Day, -7, _dWeekStart)
                sEnd = DateAdd(DateInterval.Day, +7, _dWeekStart)
                dEnd = sEnd

                ' Get Events/Sub-Calendar Events

                Dim getSubEvents As Boolean = Settings.MasterEvent
                _selectedEvents = objEventInfoHelper.GetEvents(dBegin, dEnd, getSubEvents, SelectCategory.SelectedCategory, SelectLocation.SelectedLocation, GetUrlGroupId, GetUrlUserId)

                _selectedEvents = objEventInfoHelper.ConvertEventListToDisplayTimeZone(_selectedEvents, GetDisplayTimeZoneId())

                ' Setup ScheduleGeneral
                ' Create DataView
                Dim eventTable As DataTable = New DataTable("Events")
                With eventTable.Columns
                    .Add("ID", Type.GetType("System.Int32"))
                    .Add("CreatedByID", Type.GetType("System.Int32"))
                    .Add("OwnerID", Type.GetType("System.Int32"))
                    .Add("StartTime", Type.GetType("System.DateTime"))
                    .Add("EndTime", Type.GetType("System.DateTime"))
                    .Add("Icons", Type.GetType("System.String"))
                    .Add("Task", Type.GetType("System.String"))
                    .Add("Description", Type.GetType("System.String"))
                    .Add("StartDateTime", Type.GetType("System.DateTime"))
                    .Add("Duration", Type.GetType("System.Int32"))
                    .Add("URL", Type.GetType("System.String"))
                    .Add("Target", Type.GetType("System.String"))
                    .Add("Tooltip", Type.GetType("System.String"))
                    .Add("BackColor", Type.GetType("System.String"))
                End With

                If Settings.Eventtooltipweek Then
                    toolTipManager.TargetControls.Clear()
                End If

                Dim dgRow As DataRow
                Dim objEvent As EventInfo
                For Each objEvent In _selectedEvents
                    ' If full enrollments should be hidden, ignore
                    If HideFullEvent(objEvent) Then
                        Continue For
                    End If

                    If objEvent.EventTimeEnd > sBegin And objEvent.EventTimeBegin < sEnd Then
                        dgRow = eventTable.NewRow()
                        dgRow("ID") = objEvent.EventID
                        dgRow("CreatedByID") = objEvent.CreatedByID
                        dgRow("OwnerID") = objEvent.OwnerID
                        dgRow("StartTime") = objEvent.EventTimeBegin
                        If Not objEvent.AllDayEvent Then
                            dgRow("EndTime") = objEvent.EventTimeEnd
                        Else
                            ' all day events are recorded as 23:59
                            dgRow("EndTime") = objEvent.EventTimeEnd.AddMinutes(1)
                        End If
                        '**** Add ModuleName if SubCalendar
                        Dim imagestring As String = ""
                        If Settings.Eventimage And Settings.EventImageWeek _
                          And objEvent.ImageURL <> Nothing And objEvent.ImageDisplay = True Then
                            imagestring = ImageInfo(objEvent.ImageURL, objEvent.ImageHeight, objEvent.ImageWidth)
                        End If

                        dgRow("BackColor") = ""
                        Dim iconString As String = ""

                        Dim eventtext As String = CreateEventName(objEvent, Settings.Templates.txtWeekEventText)

                        If Not IsPrivateNotModerator Or UserId = objEvent.OwnerID Then
                            Dim forecolorstr As String = ""
                            Dim backcolorstr As String = ""
                            Dim blankstr As String = ""
                            If objEvent.Color <> "" Then
                                backcolorstr = "background-color: " + objEvent.Color + ";"
                                blankstr = "&nbsp;"
                                dgRow("BackColor") = objEvent.Color
                            End If
                            If objEvent.FontColor <> "" Then
                                forecolorstr = "color: " + objEvent.FontColor + ";"
                            End If
                            dgRow("Task") = "<span style=""" + backcolorstr + forecolorstr + """>" + imagestring + blankstr + eventtext + blankstr + "</span>"

                            iconString = CreateIconString(objEvent, Settings.IconWeekPrio, Settings.IconWeekRec, Settings.IconWeekReminder, Settings.IconWeekEnroll)

                            ' Get detail page url
                            dgRow("URL") = objEventInfoHelper.DetailPageURL(objEvent)
                            If objEvent.DetailPage And objEvent.DetailNewWin Then
                                dgRow("Target") = "_blank"
                            End If

                        Else
                            dgRow("Task") = imagestring + eventtext
                        End If

                        dgRow("Icons") = iconString
                        dgRow("Description") = objEvent.EventDesc
                        dgRow("StartDateTime") = objEvent.EventTimeBegin
                        dgRow("Duration") = objEvent.Duration
                        If Settings.Eventtooltipweek Then
                            Dim isEvtEditor As Boolean = IsEventEditor(objEvent, False)
                            dgRow("Tooltip") = ToolTipCreate(objEvent, Settings.Templates.txtTooltipTemplateTitle, Settings.Templates.txtTooltipTemplateBody, isEvtEditor)
                        End If


                        eventTable.Rows.Add(dgRow)
                    End If
                Next
                Dim dvEvent As New DataView(eventTable)

                schWeek.StartDate = _dWeekStart
                schWeek.DateFormatString = Settings.Templates.txtWeekTitleDate
                schWeek.Weeks = 1
                schWeek.DataSource = dvEvent
                schWeek.DataBind()
            Catch
            End Try
        End Sub

#End Region

#Region "Links and Buttons"
        Private Sub lnkNext_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles lnkNext.Click
            Dim dDate As DateTime = CType(ViewState(ModuleId & "WeekOf"), DateTime).AddDays(7)
            SelectedDate = dDate.Date
            dpGoToDate.SelectedDate = dDate.Date
            SelectCategory.StoreCategories()
            SelectLocation.StoreLocations()
            BindPage(dDate)
        End Sub

        Private Sub lnkPrev_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles lnkPrev.Click
            Dim dDate As DateTime = CType(ViewState(ModuleId & "WeekOf"), DateTime).AddDays(-7)
            SelectedDate = dDate.Date
            dpGoToDate.SelectedDate = dDate.Date
            SelectCategory.StoreCategories()
            SelectLocation.StoreLocations()
            BindPage(dDate)
        End Sub

        Private Sub schWeek_ItemDataBound(ByVal sender As Object, ByVal e As ScheduleItemEventArgs) Handles schWeek.ItemDataBound
            If e.Item.ItemType = ScheduleItemType.Item Or e.Item.ItemType = ScheduleItemType.AlternatingItem Then
                Dim row As DataRowView = CType(e.Item.DataItem, DataRowView)
                Dim itemCell As System.Web.UI.WebControls.TableCell = CType(e.Item.Parent, System.Web.UI.WebControls.TableCell)
                If Settings.Eventtooltipweek Then
                    Dim tooltip As String = CType(row("Tooltip"), String)
                    itemCell.Attributes.Add("title", tooltip)
                    toolTipManager.TargetControls.Add(itemCell.ClientID, True)
                End If
                If IsPrivateNotModerator And Not UserId = CType(row("OwnerID"), Integer) Then
                    itemCell.Style.Add("cursor", "text")
                    itemCell.Style.Add("text-decoration", "none")
                    itemCell.Attributes.Add("onclick", "javascript:return false;")
                End If
                Dim backColor As String = CType(row("BackColor"), String)
                If backColor <> "" Then
                    itemCell.BackColor = GetColor(backColor)
                End If
            End If
        End Sub

        Private Sub SelectCategory_CategorySelected(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs) Handles SelectCategory.CategorySelectedChanged
            'Store the other selection(s) too.
            SelectLocation.StoreLocations()
            Dim dDate As DateTime = CType(ViewState(ModuleId & "WeekOf"), DateTime)
            BindPage(dDate)
        End Sub
        Private Sub SelectLocation_LocationSelected(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CommandEventArgs) Handles SelectLocation.LocationSelectedChanged
            'Store the other selection(s) too.
            SelectCategory.StoreCategories()
            Dim dDate As DateTime = CType(ViewState(ModuleId & "WeekOf"), DateTime)
            BindPage(dDate)
        End Sub

        Private Sub lnkToday_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkToday.Click
            Dim dDate As Date = DateTime.Now.Date()
            SelectedDate = dDate
            dpGoToDate.SelectedDate = dDate
            SelectCategory.StoreCategories()
            SelectLocation.StoreLocations()
            BindPage(dDate)
        End Sub

        Private Sub dpGoToDate_SelectedDateChanged(sender As Object, e As Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs) Handles dpGoToDate.SelectedDateChanged
            Dim dDate As Date = CType(dpGoToDate.SelectedDate, Date)
            SelectedDate = dDate
            BindPage(dDate)
        End Sub

#End Region

    End Class

End Namespace
