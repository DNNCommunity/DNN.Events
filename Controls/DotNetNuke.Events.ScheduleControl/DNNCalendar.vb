'
'DotNetNuke® - http://www.dnnsoftware.com
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
Imports System.Collections
Imports System.ComponentModel
Imports System.Drawing
Imports System.Globalization
Imports System.Text
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls

Namespace MonthControl

    ''' <summary>
    ''' Enumerator for Days of Week
    ''' </summary>
    <Flags()> _
    Public Enum MyDayOfWeek
        ''' <summary>
        ''' Monday
        ''' </summary>
        Monday = 1
        ''' <summary>
        ''' Tuesday
        ''' </summary>
        Tuesday = 2
        ''' <summary>
        ''' Wednesday
        ''' </summary>
        Wednesday = 4
        ''' <summary>
        ''' Thursday
        ''' </summary>
        Thursday = 8
        ''' <summary>
        ''' Friday
        ''' </summary>
        Friday = 16
        ''' <summary>
        ''' Saturday
        ''' </summary>
        Saturday = 32
        ''' <summary>
        ''' Sunday
        ''' </summary>
        Sunday = 64
    End Enum

    ''' <summary>
    ''' Fixes Web Calendar Control Style Issues
    ''' </summary>
    <CLSCompliant(True), DefaultEvent("SelectionChanged"), ToolboxBitmap(GetType(DNNCalendar), "DNNCalendar.bmp"), ToolboxData("<{0}:DNNCalendar runat=""server""></{0}:DNNCalendar>")> _
    Public Class DNNCalendar
        Inherits System.Web.UI.WebControls.Calendar

        ' Mapping of built-in DayOfWeek and our custom MyDayOfWeek
        Private Function DayOfWeekMapping(ByVal orig As DayOfWeek) As MyDayOfWeek
            Dim ret As MyDayOfWeek
            Select Case orig
                Case DayOfWeek.Monday
                    ret = MyDayOfWeek.Monday
                    Exit Select
                Case DayOfWeek.Tuesday
                    ret = MyDayOfWeek.Tuesday
                    Exit Select
                Case DayOfWeek.Wednesday
                    ret = MyDayOfWeek.Wednesday
                    Exit Select
                Case DayOfWeek.Thursday
                    ret = MyDayOfWeek.Thursday
                    Exit Select
                Case DayOfWeek.Friday
                    ret = MyDayOfWeek.Friday
                    Exit Select
                Case DayOfWeek.Saturday
                    ret = MyDayOfWeek.Saturday
                    Exit Select
                Case DayOfWeek.Sunday
                    ret = MyDayOfWeek.Sunday
                    Exit Select
                Case Else
                    ret = MyDayOfWeek.Saturday
                    Exit Select
            End Select

            Return ret
        End Function

        ''' <summary>
        ''' Set WeekEnd days
        ''' </summary>
        <Bindable(True), Browsable(True), Category("Appearance"), Description("What days are considered Weekend Days"), DefaultValue(MyDayOfWeek.Saturday Or MyDayOfWeek.Sunday)> _
        Public Property WeekEndDays() As MyDayOfWeek
            Get
                Dim wed As MyDayOfWeek = MyDayOfWeek.Saturday Or MyDayOfWeek.Sunday
                Dim obj As Object = Me.ViewState("wed")
                If obj IsNot Nothing Then
                    wed = DirectCast(obj, MyDayOfWeek)
                End If
                Return wed
            End Get
            Set(ByVal value As MyDayOfWeek)
                Me.ViewState("wed") = value
            End Set
        End Property

        ''' <summary>
        ''' Initializes a new instance of the <see cref="DNNCalendar"/> class.
        ''' </summary>
        ''' <remarks>
        ''' Use this constructor to create and initialize a new instance of
        ''' the <see cref="DNNCalendar"/> class.
        ''' </remarks>
        Public Sub New()
            MyBase.New()
        End Sub

#Region "Private properties"
        ' Gets the date that specifies the month to be displayed. This will
        ' be VisibleDate unless that property is defaulted to
        ' DateTime.MinValue, in which case TodaysDate is returned instead.
        Private ReadOnly Property TargetDate() As DateTime
            Get
                If Me.VisibleDate = DateTime.MinValue Then
                    Return Me.TodaysDate
                Else
                    Return Me.VisibleDate
                End If
            End Get
        End Property

        ' This is the date used for creating day count values, i.e., the
        ' number of days between some date and this one. These values are
        ' used to create post back event arguments identical to those used
        ' by the base Calendar class.
        Private Shared ReadOnly DayCountBaseDate As New DateTime(2000, 1, 1)
#End Region

#Region "Control rendering"

        ''' <summary> 
        ''' This member overrides <see cref="System.Web.UI.Control.Render"/>.
        ''' </summary>
        Protected Overloads Overrides Sub Render(ByVal output As HtmlTextWriter)
            ' Create the main table.
            Dim table As New Table()
            table.CellPadding = Me.CellPadding
            table.CellSpacing = Me.CellSpacing

            If Me.ShowGridLines Then
                table.GridLines = GridLines.Both
            Else
                table.GridLines = GridLines.None
            End If

            ' If ShowTitle is true, add a row with the calendar title.
            If Me.ShowTitle Then
                ' Create a one-cell table row.
                Dim row As New TableRow()
                Dim cell As New TableCell()
                If Me.HasWeekSelectors(Me.SelectionMode) Then
                    cell.ColumnSpan = 8
                Else
                    cell.ColumnSpan = 7
                End If

                ' Apply styling.
                cell.MergeStyle(Me.TitleStyle)

                ' Add the title table to the cell.
                cell.Controls.Add(Me.TitleTable())
                row.Cells.Add(cell)

                ' Add it to the table.
                table.Rows.Add(row)
            End If

            ' If ShowDayHeader is true, add a row with the days header.
            If Me.ShowDayHeader Then
                table.Rows.Add(DaysHeaderTableRow())
            End If

            ' Find the first date that will be visible on the calendar.
            Dim [date] As DateTime = Me.GetFirstCalendarDate()

            ' Create a list for storing nonselectable dates.
            Dim nonselectableDates As New ArrayList()

            ' Add rows for the dates (six rows are always displayed).
            For i As Integer = 0 To 5
                Dim row As New TableRow()

                ' Create a week selector, if needed.
                If Me.HasWeekSelectors(Me.SelectionMode) Then
                    Dim cell As New TableCell()
                    cell.HorizontalAlign = HorizontalAlign.Center
                    cell.MergeStyle(Me.SelectorStyle)

                    If Me.Enabled Then
                        ' Create the post back link.
                        Dim anchor As New HtmlAnchor()
                        Dim arg As String = [String].Format("R{0}07", Me.DayCountFromDate([date]))
                        anchor.HRef = Me.Page.ClientScript.GetPostBackClientHyperlink(Me, arg)
                        anchor.Controls.Add(New LiteralControl(Me.SelectWeekText))

                        ' Add a color style to the anchor if it is explicitly
                        ' set.
                        If Not Me.SelectorStyle.ForeColor.IsEmpty Then
                            anchor.Attributes.Add("style", [String].Format("color:{0}", Me.SelectorStyle.ForeColor.Name))
                        End If

                        cell.Controls.Add(anchor)
                    Else
                        cell.Controls.Add(New LiteralControl(Me.SelectWeekText))
                    End If

                    row.Cells.Add(cell)
                End If

                ' Add the days (there are always seven days per row).
                For j As Integer = 0 To 6
                    ' Create a CalendarDay and a TableCell for the date.
                    Dim day As CalendarDay = Me.Day([date])
                    Dim cell As TableCell = Me.Cell(day)

                    ' Raise the OnDayRender event.
                    Me.OnDayRender(cell, day)

                    ' If the day was marked nonselectable, add it to the list.
                    If Not day.IsSelectable Then
                        nonselectableDates.Add(day.[Date].ToShortDateString())
                    End If

                    ' If the day is selectable, and the selection mode allows
                    ' it, convert the text to a link with post back.
                    If Me.Enabled AndAlso day.IsSelectable AndAlso Me.SelectionMode <> CalendarSelectionMode.None Then
                        Try
                            ' Create the post back link.
                            Dim anchor As New HtmlAnchor()
                            Dim arg As String = Me.DayCountFromDate([date]).ToString()
                            anchor.HRef = Me.Page.ClientScript.GetPostBackClientHyperlink(Me, arg)

                            ' Copy the existing text.
                            anchor.Controls.Add(New LiteralControl(DirectCast(cell.Controls(0), LiteralControl).Text))

                            ' Add a color style to the anchor if it is
                            ' explicitly set. Note that the style precedence
                            ' follows that of the base Calendar control.
                            Dim s As String = ""
                            If Not Me.DayStyle.ForeColor.IsEmpty Then
                                s = Me.DayStyle.ForeColor.Name
                            End If
                            Dim currdayofweek As MyDayOfWeek = DayOfWeekMapping(day.[Date].DayOfWeek)
                            If ((Me.WeekEndDays And currdayofweek) = currdayofweek) And (Not Me.WeekendDayStyle.ForeColor.IsEmpty) Then
                                s = Me.WeekendDayStyle.ForeColor.Name
                            End If
                            If day.IsOtherMonth AndAlso Not Me.OtherMonthDayStyle.ForeColor.IsEmpty Then
                                s = Me.OtherMonthDayStyle.ForeColor.Name
                            End If
                            If day.IsToday AndAlso Not Me.TodayDayStyle.ForeColor.IsEmpty Then
                                s = Me.TodayDayStyle.ForeColor.Name
                            End If
                            If Me.SelectedDates.Contains(day.[Date]) AndAlso Not Me.SelectedDayStyle.ForeColor.IsEmpty Then
                                s = Me.SelectedDayStyle.ForeColor.Name
                            End If
                            If s.Length > 0 Then
                                anchor.Attributes.Add("style", [String].Format("color:{0}", s))
                            End If

                            ' Replace the literal control in the cell with
                            ' the anchor.
                            cell.Controls.RemoveAt(0)
                            cell.Controls.AddAt(0, anchor)
                        Catch generatedExceptionName As Exception
                        End Try
                    End If

                    ' Add the cell to the current table row.
                    row.Cells.Add(cell)

                    ' Bump the date.
                    [date] = [date].AddDays(1)
                Next

                ' Add the row.
                table.Rows.Add(row)
            Next

            ' Save the list of nonselectable dates.
            If nonselectableDates.Count > 0 Then
                Me.SaveNonselectableDates(nonselectableDates)
            End If

            ' Apply styling.
            Me.AddAttributesToRender(output)

            ' Render the table.
            table.RenderControl(output)
        End Sub

        ' ====================================================================
        ' Helper functions for rendering the control.
        ' ====================================================================

        '
        ' Generates a Table control for the calendar title.
        '
        Private Function TitleTable() As Table
            ' Create a table row.
            Dim row As New TableRow()
            Dim cell As TableCell
            Dim anchor As HtmlAnchor
            Dim [date] As DateTime
            Dim text As String

            ' Add a table cell with the previous month.
            If Me.ShowNextPrevMonth Then
                cell = New TableCell()
                cell.MergeStyle(Me.NextPrevStyle)
                cell.Style.Add("width", "15%")

                ' Find the first of the previous month, needed for post back
                ' processing.
                Try
                    [date] = New DateTime(Me.TargetDate.Year, Me.TargetDate.Month, 1).AddMonths(-1)
                Catch generatedExceptionName As Exception
                    [date] = Me.TargetDate
                End Try

                ' Get the previous month text.
                If Me.NextPrevFormat = NextPrevFormat.CustomText Then
                    text = Me.PrevMonthText
                Else
                    If Me.NextPrevFormat = NextPrevFormat.ShortMonth Then
                        text = [date].ToString("MMM")
                    Else
                        text = [date].ToString("MMMM")
                    End If
                End If

                If Me.Enabled Then
                    ' Create the post back link.
                    anchor = New HtmlAnchor()
                    Dim arg As String = [String].Format("V{0}", Me.DayCountFromDate([date]))
                    anchor.HRef = Me.Page.ClientScript.GetPostBackClientHyperlink(Me, arg)
                    anchor.Controls.Add(New LiteralControl(text))

                    ' Add a color style to the anchor if it is explicitly
                    ' set.
                    If Not Me.NextPrevStyle.ForeColor.IsEmpty Then
                        anchor.Attributes.Add("style", [String].Format("color:{0}", Me.NextPrevStyle.ForeColor.Name))
                    End If

                    ' Add the link to the cell.
                    ' }
                    cell.Controls.Add(anchor)
                Else
                    cell.Controls.Add(New LiteralControl(text))
                End If

                row.Cells.Add(cell)
            End If

            ' Add a table cell for the title text.
            cell = New TableCell()
            cell.HorizontalAlign = HorizontalAlign.Center
            cell.VerticalAlign = VerticalAlign.Middle
            If Me.ShowNextPrevMonth Then
                cell.Style.Add("width", "70%")
            End If
            If Me.TitleFormat = TitleFormat.Month Then
                cell.Text = Me.TargetDate.ToString("MMMM")
            Else
                cell.Text = Me.TargetDate.ToString("y").Replace(", ", " ")
            End If
            row.Cells.Add(cell)

            ' Add a table cell for the next month.
            If Me.ShowNextPrevMonth Then
                cell = New TableCell()
                cell.HorizontalAlign = HorizontalAlign.Right
                cell.MergeStyle(Me.NextPrevStyle)
                cell.Style.Add("width", "15%")

                ' Find the first of the next month, needed for post back
                ' processing.
                Try
                    [date] = New DateTime(Me.TargetDate.Year, Me.TargetDate.Month, 1).AddMonths(1)
                Catch generatedExceptionName As Exception
                    [date] = Me.TargetDate
                End Try

                ' Get the next month text.
                If Me.NextPrevFormat = NextPrevFormat.CustomText Then
                    text = Me.NextMonthText
                Else
                    If Me.NextPrevFormat = NextPrevFormat.ShortMonth Then
                        text = [date].ToString("MMM")
                    Else
                        text = [date].ToString("MMMM")
                    End If
                End If

                If Me.Enabled Then
                    ' Create the post back link.
                    anchor = New HtmlAnchor()
                    Dim arg As String = [String].Format("V{0}", Me.DayCountFromDate([date]))
                    anchor.HRef = Me.Page.ClientScript.GetPostBackClientHyperlink(Me, arg)
                    anchor.Controls.Add(New LiteralControl(text))

                    ' Add a color style to the anchor if it is explicitly
                    ' set.
                    If Not Me.NextPrevStyle.ForeColor.IsEmpty Then
                        anchor.Attributes.Add("style", [String].Format("color:{0}", Me.NextPrevStyle.ForeColor.Name))
                    End If

                    ' Add the link to the cell.
                    ' }
                    cell.Controls.Add(anchor)
                Else
                    cell.Controls.Add(New LiteralControl(text))
                End If

                row.Cells.Add(cell)
            End If

            ' Create the table and add the title row to it.
            Dim table As New Table()
            table.CellPadding = 0
            table.CellSpacing = 0
            table.Attributes.Add("style", "width:100%;height:100%")
            table.Rows.Add(row)

            Return table
        End Function

        '
        ' Generates a TableRow control for the calendar days header.
        '
        Private Function DaysHeaderTableRow() As TableRow
            ' Create the table row.
            Dim row As New TableRow()

            ' Create an array of days.
            Dim days As DayOfWeek() = {DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, _
            DayOfWeek.Saturday}

            ' Adjust the array to get the specified starting day at the first index.
            Dim first As DayOfWeek = Me.GetFirstDayOfWeek()
            While days(0) <> first
                Dim temp As DayOfWeek = days(0)
                For i As Integer = 0 To days.Length - 2
                    days(i) = days(i + 1)
                Next
                days(days.Length - 1) = temp
            End While

            ' Add a month selector column, if needed.
            If Me.HasWeekSelectors(Me.SelectionMode) Then
                Dim cell As New TableCell()
                cell.HorizontalAlign = HorizontalAlign.Center

                ' If months are selectable, create the selector.
                If Me.SelectionMode = CalendarSelectionMode.DayWeekMonth Then
                    ' Find the first of the month.
                    Dim [date] As New DateTime(Me.TargetDate.Year, Me.TargetDate.Month, 1)

                    ' Use the selector style.
                    cell.MergeStyle(Me.SelectorStyle)

                    ' Create the post back link.
                    If Me.Enabled Then
                        Dim anchor As New HtmlAnchor()
                        Dim arg As String = [String].Format("R{0}{1}", Me.DayCountFromDate([date]), DateTime.DaysInMonth([date].Year, [date].Month))
                        anchor.HRef = Me.Page.ClientScript.GetPostBackClientHyperlink(Me, arg)
                        anchor.Controls.Add(New LiteralControl(Me.SelectMonthText))

                        ' Add a color style to the anchor if it is explicitly
                        ' set.
                        If Not Me.SelectorStyle.ForeColor.IsEmpty Then
                            anchor.Attributes.Add("style", [String].Format("color:{0}", Me.SelectorStyle.ForeColor.Name))
                        End If

                        cell.Controls.Add(anchor)
                    Else
                        cell.Controls.Add(New LiteralControl(Me.SelectMonthText))
                    End If
                Else
                    ' Use the day header style.
                    cell.CssClass = Me.DayHeaderStyle.CssClass
                End If

                row.Cells.Add(cell)
            End If

            ' Add the day names to the header.
            For Each day As System.DayOfWeek In days
                row.Cells.Add(Me.DayHeaderTableCell(day))
            Next

            Return row
        End Function

        '
        ' Returns a table cell containing a day name for the calendar day
        ' header.
        '
        Private Function DayHeaderTableCell(ByVal dayOfWeek As System.DayOfWeek) As TableCell
            ' Generate the day name text based on the specified format.
            Dim s As String
            If Me.DayNameFormat = DayNameFormat.[Short] Then
                s = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames(CInt(dayOfWeek))
            Else
                s = CultureInfo.CurrentCulture.DateTimeFormat.DayNames(CInt(dayOfWeek))
                If Me.DayNameFormat = DayNameFormat.FirstTwoLetters Then
                    s = s.Substring(0, 2)
                End If
                If Me.DayNameFormat = DayNameFormat.FirstLetter Then
                    s = s.Substring(0, 1)
                End If
            End If

            ' Create the cell, set the style and the text.
            Dim cell As New TableCell()
            cell.HorizontalAlign = HorizontalAlign.Center
            cell.MergeStyle(Me.DayHeaderStyle)
            cell.Text = s

            Return cell
        End Function

        '
        ' Determines the first day of the week based on the FirstDayOfWeek
        ' property setting.
        '
        Private Function GetFirstDayOfWeek() As System.DayOfWeek
            ' If the default value is specifed, use the system default.
            If Me.FirstDayOfWeek = FirstDayOfWeek.[Default] Then
                Return CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek
            Else
                Return DirectCast(Me.FirstDayOfWeek, DayOfWeek)
            End If
        End Function

        '
        ' Returns the date that should appear in the first day cell of the
        ' calendar display.
        '
        Private Function GetFirstCalendarDate() As DateTime
            ' Start with the first of the month.
            Dim [date] As New DateTime(Me.TargetDate.Year, Me.TargetDate.Month, 1)

            ' While that day does not fall on the first day of the week, move back.
            Dim firstDay As DayOfWeek = Me.GetFirstDayOfWeek()
            While [date].DayOfWeek <> firstDay
                [date] = [date].AddDays(-1)
            End While

            Return [date]
        End Function

        '
        ' Creates a CalendarDay instance for the given date.
        '
        ' This object is included in the DayRenderEventArgs passed to
        ' the DayRender event handler.
        '
        Private Function Day(ByVal [date] As DateTime) As CalendarDay
            Dim calday As New CalendarDay([date], [date].DayOfWeek = DayOfWeek.Saturday OrElse [date].DayOfWeek = DayOfWeek.Sunday, [date] = Me.TodaysDate, [date] = Me.SelectedDate, Not ([date].Month = Me.TargetDate.Month AndAlso [date].Year = Me.TargetDate.Year), [date].Day.ToString())

            ' Default the day to selectable.
            calday.IsSelectable = True

            Return calday
        End Function

        '
        ' Creates a TableCell control for the given calendar day.
        '
        ' Note: This object is included in the DayRenderEventArgs passed to
        ' the DayRender event handler.
        '
        Private Function Cell(ByVal day As CalendarDay) As TableCell
            Dim tbcell As New TableCell()
            tbcell.HorizontalAlign = HorizontalAlign.Center

            ' Add styling based on day flags.
            ' - Styles are applied per the precedence order used by the
            ' base Calendar control.
            ' - For CssClass, multiple class names may be added.
            Dim sb As New StringBuilder()
            Dim currdayofweek As MyDayOfWeek = DayOfWeekMapping(day.[Date].DayOfWeek)
            If ((Me.WeekEndDays And currdayofweek) = currdayofweek) And (Not day.IsOtherMonth) And (Not (Me.WeekendDayStyle Is Nothing)) Then
                tbcell.MergeStyle(Me.WeekendDayStyle)
                sb.AppendFormat(" {0}", Me.WeekendDayStyle.CssClass)
            End If
            If day.IsOtherMonth Then
                If day.IsWeekend Then
                    tbcell.ApplyStyle(Me.OtherMonthDayStyle)
                    sb = New StringBuilder()
                    sb.AppendFormat(" {0}", Me.OtherMonthDayStyle.CssClass)
                Else
                    tbcell.MergeStyle(Me.OtherMonthDayStyle)
                    sb.AppendFormat(" {0}", Me.OtherMonthDayStyle.CssClass)
                End If
            End If

            If Me.SelectedDates.Contains(day.[Date]) Then
                tbcell.MergeStyle(Me.SelectedDayStyle)
                sb.AppendFormat(" {0}", Me.SelectedDayStyle.CssClass)
            End If
            If day.IsToday Then
                tbcell.MergeStyle(Me.TodayDayStyle)
                sb.AppendFormat(" {0}", Me.TodayDayStyle.CssClass)
            End If

            tbcell.MergeStyle(Me.DayStyle)
            sb.AppendFormat(" {0}", Me.DayStyle.CssClass)

            Dim s As String = sb.ToString().Trim()
            If s.Length > 0 Then
                tbcell.CssClass = s
            End If

            ' Add a literal control to the cell using the day number for the
            ' text.
            tbcell.Controls.Add(New LiteralControl(day.DayNumberText))

            Return tbcell
        End Function

        '
        ' Returns true if the selection mode includes week selectors.
        '
        Private Shadows Function HasWeekSelectors(ByVal selectionMode As CalendarSelectionMode) As Boolean
            If selectionMode = CalendarSelectionMode.DayWeek OrElse selectionMode = CalendarSelectionMode.DayWeekMonth Then
                Return True
            Else
                Return False
            End If
        End Function
#End Region

#Region "Post back event handling"
        ' ====================================================================
        ' Functions for converting between DateTime and day count values.
        ' ====================================================================

        '
        ' Returns the number of days between the given DateTime value and the
        ' base date.
        '
        Private Function DayCountFromDate(ByVal [date] As DateTime) As Integer
            Return ([date] - DNNCalendar.DayCountBaseDate).Days
        End Function

        '
        ' Returns a DateTime value equal to the base date plus the given number
        ' of days.
        '
        Private Function DateFromDayCount(ByVal dayCount As Integer) As DateTime
            Return DNNCalendar.DayCountBaseDate.AddDays(dayCount)
        End Function

        ' ====================================================================
        ' Functions to save and load the nonselectable dates list.
        '
        ' A hidden form field is used to store this data rather than the
        ' view state because the nonselectable dates are not known until after
        ' the DayRender event has been raised for each day as the control is
        ' rendered.
        '
        ' To minimize the amount of data stored in that field, the dates are
        ' represented as day count values.
        ' ====================================================================

        '
        ' Saves a list of dates to the hidden form field.
        '
        Private Sub SaveNonselectableDates(ByVal dates As ArrayList)
            ' Build a string array by converting each date to a day count
            ' value.
            Dim list As String() = New String(dates.Count - 1) {}
            For i As Integer = 0 To list.Length - 1
                list(i) = Me.DayCountFromDate(DateTime.Parse(dates(i).ToString())).ToString()
            Next

            ' Get the hidden field name.
            Dim fieldName As String = Me.GetHiddenFieldName()

            ' For the field value, create a comma-separated list from the day
            ' count values.
            Dim fieldValue As String = HttpUtility.HtmlAttributeEncode([String].Join(",", list))

            ' Add the hidden form field to the page.
            'EVT-9313 this.Page.RegisterHiddenField(fieldName, fieldValue);
            Me.Page.ClientScript.RegisterHiddenField(fieldName, fieldValue)
        End Sub

        '
        ' Returns a list of dates stored in the hidden form field.
        '
        Private Function LoadNonselectableDates() As ArrayList
            ' Create an empty list.
            Dim dates As New ArrayList()

            ' Get the value stored in the hidden form field.
            Dim fieldName As String = Me.GetHiddenFieldName()
            Dim fieldValue As String = Me.Page.Request.Form(fieldName)

            ' If no dates were stored, return the empty list.
            If fieldValue Is Nothing Then
                Return dates
            End If

            ' Extract the individual day count values.
            Dim list As String() = fieldValue.Split(","c)

            ' Convert those values to dates and store them in an array list.
            For Each s As String In list
                dates.Add(Me.DateFromDayCount(Int32.Parse(s)))
            Next

            Return dates
        End Function

        '
        ' Returns the name of the hidden field used to store nonselectable
        ' dates on the form.
        '
        Private Function GetHiddenFieldName() As String
            ' Create a unique field name.
            Return [String].Format("{0}_NonselectableDates", Me.ClientID)
        End Function

        ' ====================================================================
        ' Implementation of the IPostBackEventHandler.RaisePostBackEvent
        ' event handler.
        ' ====================================================================

        ''' <summary>
        ''' Handles a post back event targeted at the control.
        ''' </summary>
        ''' <param name="eventArgument">
        ''' A <see cref="System.String"/> representing the event argument passed to the handler.
        ''' </param>
        Public Shadows Sub RaisePostBackEvent(ByVal eventArgument As String)
            ' Was the post back initiated by a previous or next month link?
            If eventArgument.StartsWith("V") Then
                Try
                    ' Save the current visible date.
                    Dim previousDate As DateTime = Me.TargetDate

                    ' Extract the day count from the argument and use it to
                    ' change the visible date.
                    Dim d As Integer = Int32.Parse(eventArgument.Substring(1))
                    Me.VisibleDate = Me.DateFromDayCount(d)

                    ' Raise the VisibleMonthChanged event.
                    OnVisibleMonthChanged(Me.VisibleDate, previousDate)
                Catch generatedExceptionName As Exception
                End Try
                Exit Sub
            End If

            ' Was the post back initiated by a month or week selector link?
            If eventArgument.StartsWith("R") Then
                Try
                    ' Extract the day count and number of days from the
                    ' argument.
                    Dim d As Integer = Int32.Parse(eventArgument.Substring(1, eventArgument.Length - 3))
                    Dim n As Integer = Int32.Parse(eventArgument.Substring(eventArgument.Length - 2))

                    ' Get the starting date.
                    Dim [date] As DateTime = Me.DateFromDayCount(d)

                    ' Reset the selected dates collection to include all the
                    ' dates in the given range.
                    Me.SelectedDates.Clear()
                    Me.SelectedDates.SelectRange([date], [date].AddDays(n - 1))

                    ' // If SelectAllInRange is false, remove any dates found
                    ' // in the nonselectable date list.
                    ' if (!this.SelectAllInRange)
                    ' {
                    ' ArrayList nonselectableDates = this.LoadNonselectableDates();
                    ' foreach(DateTime badDate in nonselectableDates)
                    ' this.SelectedDates.Remove(badDate);
                    ' }

                    ' Raise the SelectionChanged event.
                    OnSelectionChanged()
                Catch generatedExceptionName As Exception
                End Try
                Exit Sub
            End If

            ' The post back must have been initiated by a calendar day link.
            Try
                ' Get the day count from the argument.
                Dim d As Integer = Int32.Parse(eventArgument)

                ' Reset the selected dates collection to include only the
                ' newly selected date.
                Me.SelectedDates.Clear()
                Me.SelectedDates.Add(Me.DateFromDayCount(d))

                ' Raise the SelectionChanged event.
                OnSelectionChanged()
            Catch generatedExceptionName As Exception
            End Try
        End Sub
#End Region

    End Class
End Namespace