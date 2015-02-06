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
Option Strict On

Imports System
Imports System.Data
Imports System.Collections
Imports System.Web   ' HttpException
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.ComponentModel

''' -----------------------------------------------------------------------------
''' Project	 : schedule
''' Class	 : ScheduleCalendar
'''
''' -----------------------------------------------------------------------------
''' <summary>
''' The ScheduleCalendar web control is designed to represent a schedule in a calendar format.
''' </summary>
''' -----------------------------------------------------------------------------
<CLSCompliant(True), ParseChildren(True)> _
    Public Class ScheduleCalendar : Inherits BaseSchedule

#Region "Properties"
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Whether to show the EmptyDataTemplate or not when no data is found
    ''' </summary>
    ''' <remarks>
    ''' Overrides default value (True)
    ''' </remarks>
    ''' -----------------------------------------------------------------------------
    Protected Overrides ReadOnly Property ShowEmptyDataTemplate() As Boolean
        Get
            ' When FullTimeScale=True, an empty calendar is shown, so there's no need for a replacement
            Return Not FullTimeScale
        End Get
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The database field containing the start time of the events. This field should also contain the date when TimeFieldsContainDate=true
    ''' </summary>
    ''' <remarks>
    ''' StartTimeField replaces DataRangeStartField for ScheduleCalendar
    ''' </remarks>
    ''' -----------------------------------------------------------------------------
    <Description("The database field containing the start time of the events. This field should also contain the date when TimeFieldsContainDate=true"), _
    Bindable(False), Category("Data")> _
    Public Property StartTimeField() As String
        Get
            Return MyBase.DataRangeStartField
        End Get
        Set(ByVal Value As String)
            MyBase.DataRangeStartField = Value
        End Set
    End Property

    ' Hide DataRangeStartField. For ScheduleCalendar, it's called StartTimeField
    <Browsable(False), Obsolete("The DataRangeStartField property is obsolete"), _
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Shadows Property DataRangeStartField() As String
        Get
            Return MyBase.DataRangeStartField
        End Get
        Set(ByVal Value As String)
            MyBase.DataRangeStartField = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The database field containing the end time of the events. This field should also contain the date when TimeFieldsContainDate=true
    ''' </summary>
    ''' <remarks>
    ''' EndTimeField replaces DataRangeEndField for ScheduleCalendar
    ''' </remarks>
    ''' -----------------------------------------------------------------------------
    <Description("The database field containing the end time of the events. This field should also contain the date when TimeFieldsContainDate=true"), _
    Bindable(False), Category("Data")> _
    Public Property EndTimeField() As String
        Get
            Return MyBase.DataRangeEndField
        End Get
        Set(ByVal Value As String)
            MyBase.DataRangeEndField = Value
        End Set
    End Property

    ' Hide DataRangeEndField. For ScheduleCalendar, it's called EndTimeField
    <Browsable(False), Obsolete("The DataRangeEndField property is obsolete"), _
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Shadows Property DataRangeEndField() As String
        Get
            Return MyBase.DataRangeEndField
        End Get
        Set(ByVal Value As String)
            MyBase.DataRangeEndField = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The database field providing the dates. Ignored when TimeFieldsContainDate=true. When TimeFieldsContainDate=false, this field should be of type Date
    ''' </summary>
    ''' <remarks>
    ''' DateField replaces TitleField for ScheduleCalendar
    ''' </remarks>
    ''' -----------------------------------------------------------------------------
    <Description("The database field providing the dates. Ignored when TimeFieldsContainDate=true. When TimeFieldsContainDate=false, this field should be of type Date."), _
    Bindable(False), Category("Data")> _
    Public Property DateField() As String
        Get
            Return MyBase.TitleField
        End Get
        Set(ByVal Value As String)
            MyBase.TitleField = Value
        End Set
    End Property

    ' Hide TitleField. For ScheduleCalendar, it's called DateField
    <Browsable(False), Obsolete("The TitleField property is obsolete"), _
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Shadows Property TitleField() As String
        Get
            Return MyBase.TitleField
        End Get
        Set(ByVal Value As String)
            MyBase.TitleField = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The first date to display. 
    ''' The calendar will start on this date, if not overridden by the 
    ''' StartDay and NumberOfDays settings.
    ''' The default value is the date at the time of display.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("The first date to display. The calendar will start on this date, if not overridden by the StartDay and NumberOfDays settings."), _
    Bindable(True), Category("Behavior")> _
    Public Property StartDate() As Date
        Get
            Dim o As Object = ViewState("StartDate")
            If Not (o Is Nothing) Then Return CDate(o)
            Return DateTime.Today() ' default value
        End Get
        Set(ByVal Value As Date)
            ViewState("StartDate") = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The first day of the week to display. 
    ''' The calendar will start on this day of the week.
    ''' This value is used only when NumberOfDays equals 7.
    ''' The default is Monday.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("The first day of the week to display. The calendar will start on this day of the week. This value is used only when NumberOfDays equals 7."), _
    DefaultValue(DayOfWeek.Monday), Bindable(True), Category("Behavior")> _
    Public Property StartDay() As DayOfWeek
        Get
            Dim o As Object = ViewState("StartDay")
            If Not (o Is Nothing) Then Return CType(o, DayOfWeek)
            Return DayOfWeek.Monday ' default value
        End Get
        Set(ByVal Value As DayOfWeek)
            ViewState("StartDay") = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The number of days to display. 
    ''' This number may be repeated multiple times in Vertical layout when the NumberOfRepetitions 
    ''' property is greater than 1.
    ''' De default value is 7 (weekly calendar).
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("The number of days to display. This number may be repeated multiple times in Vertical layout when the NumberOfRepetitions property is greater than 1."), _
    DefaultValue(7), Bindable(True), Category("Behavior")> _
    Public Property NumberOfDays() As Integer
        Get
            Dim o As Object = ViewState("NumberOfDays")
            If Not (o Is Nothing) Then Return CInt(o)
            Return 7 ' default value
        End Get
        Set(ByVal Value As Integer)
            ViewState("NumberOfDays") = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The number of repetitions to show at a time. Only used in Vertical layout.
    ''' Especially useful if you want to show several weeks in the calendar, one
    ''' below the other.
    ''' This property replaces the Weeks property starting from version 1.6.1.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("The number of repetitions to show at a time. Only used in Vertical layout."), _
    DefaultValue(1), Bindable(True), Category("Behavior")> _
    Public Property NumberOfRepetitions() As Integer
        Get
            ' in horizontal layout, only 1 week is supported
            If (Layout = LayoutEnum.Horizontal) Then Return 1
            Dim o As Object = ViewState("NumberOfRepetitions")
            If Not (o Is Nothing) Then
                Dim w As Integer = CInt(o)
                If (w <= 0) Then Return 1
                Return w
            End If
            Return 1
        End Get
        Set(ByVal Value As Integer)
            ViewState("NumberOfRepetitions") = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Obsolete since version 1.6.1. Use the NumberOfRepetitions property instead.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("The number of weeks to show at a time. Only used in Vertical layout."), _
    Browsable(False), Bindable(True)> _
    Public Property Weeks() As Integer
        Get
            Return NumberOfRepetitions
        End Get
        Set(ByVal Value As Integer)
            NumberOfRepetitions = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The format used for the date if the DateTemplate is missing, e.g. {0:ddd d}
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("The format used for the date if the DateTemplate is missing, e.g. {0:ddd d}"), _
    DefaultValue(""), Category("Data")> _
    Public Property DateFormatString() As String
        Get
            Dim o As Object = ViewState("DateFormatString")
            If Not (o Is Nothing) Then Return CStr(o)
            Return String.Empty
        End Get
        Set(ByVal Value As String)
            ViewState("DateFormatString") = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The format used for the time if the TimeTemplate is missing, e.g. {0:hh:mm}
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("The format used for the times if the TimeTemplate is missing, e.g. {0:hh:mm}"), _
    DefaultValue(""), Category("Data")> _
    Public Property TimeFormatString() As String
        Get
            Dim o As Object = ViewState("TimeFormatString")
            If Not (o Is Nothing) Then Return CStr(o)
            Return String.Empty
        End Get
        Set(ByVal Value As String)
            ViewState("TimeFormatString") = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Indicates whether the time fields (StartTimeField and EndTimeField) contain the date as well. Whe true, this allows midnight spanning for calendar events. When false, the DateField contains the date.
    ''' </summary>
    ''' <remarks>
    ''' TimeFieldsContainDate replaces UseTitleFieldAsDate for ScheduleCalendar
    ''' </remarks>
    ''' -----------------------------------------------------------------------------
    <Description("Indicates whether the time fields (StartTimeField and EndTimeField) contain the date as well. When true, this allows midnight spanning for calendar events. When false, the DateField contains the date."), _
    DefaultValue(False), Bindable(True), Category("Data")> _
    Public Property TimeFieldsContainDate() As Boolean
        Get
            Dim o As Object = ViewState("TimeFieldsContainDate")
            If Not (o Is Nothing) Then Return CBool(o)
            Return False
        End Get
        Set(ByVal Value As Boolean)
            ViewState("TimeFieldsContainDate") = Value
        End Set
    End Property

#End Region

#Region "Styles"
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The style applied to time header items.
    ''' </summary>
    ''' <remarks>
    ''' TimeStyle replaces RangeHeaderStyle for ScheduleCalendar
    ''' </remarks>
    ''' -----------------------------------------------------------------------------
    <Description("The style applied to time header items. "), _
    Bindable(False), Category("Style"), _
     NotifyParentProperty(True), _
     PersistenceMode(PersistenceMode.InnerProperty)> _
    Public Overridable ReadOnly Property TimeStyle() As TableItemStyle
        Get
            Return MyBase.RangeHeaderStyle
        End Get
    End Property

    ' Hide RangeHeaderStyle. For ScheduleCalendar, it's replaced with TimeStyle
    <Browsable(False), Obsolete("The RangeHeaderStyle property is obsolete. Use TimeStyle instead"), _
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Shadows ReadOnly Property RangeHeaderStyle() As TableItemStyle
        Get
            Return MyBase.RangeHeaderStyle
        End Get
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The style applied to date header items.
    ''' </summary>
    ''' <remarks>
    ''' DateStyle replaces TitleStyle for ScheduleCalendar
    ''' </remarks>
    ''' -----------------------------------------------------------------------------
    <Description("The style applied to date header items. "), _
    Bindable(False), Category("Style"), _
     NotifyParentProperty(True), _
     PersistenceMode(PersistenceMode.InnerProperty)> _
    Public Overridable ReadOnly Property DateStyle() As TableItemStyle
        Get
            Return MyBase.TitleStyle
        End Get
    End Property

    ' Hide TitleStyle. For ScheduleCalendar, it's replaced with DateStyle
    <Browsable(False), Obsolete("The TitleStyle property is obsolete. Use DateStyle instead."), _
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Shadows ReadOnly Property TitleStyle() As TableItemStyle
        Get
            Return MyBase.TitleStyle
        End Get
    End Property

#End Region

#Region "Templates"

    Private _DateTemplate As ITemplate
    Private _TimeTemplate As ITemplate

    ' DateTemplate replaces TitleTemplate for ScheduleCalendar
    <TemplateContainer(GetType(ScheduleItem)), Browsable(False), _
     Description("The template used to create date header content."), _
     NotifyParentProperty(True), _
     PersistenceMode(PersistenceMode.InnerProperty)> _
    Public Property DateTemplate() As ITemplate
        Get
            Return _DateTemplate
        End Get
        Set(ByVal Value As ITemplate)
            _DateTemplate = Value
        End Set
    End Property

    ' TimeTemplate replaces RangeHeaderTemplate for ScheduleCalendar
    <TemplateContainer(GetType(ScheduleItem)), Browsable(False), _
        Description("The template used to create time header content."), _
        PersistenceMode(PersistenceMode.InnerProperty)> _
    Public Property TimeTemplate() As ITemplate
        Get
            Return _TimeTemplate
        End Get
        Set(ByVal Value As ITemplate)
            _TimeTemplate = Value
        End Set
    End Property

#End Region

#Region "Methods"

    ' Check if all properties are set to make the control work
    Overrides Function CheckConfiguration() As String
        If (Not TimeFieldsContainDate AndAlso DateField = "") Then
            Return "Either the DateField property must have a non blank value, or TimeFieldsContainDate must be true"
        End If
        If (StartTimeField = "") Then
            Return "The StartTimeField property is not set"
        End If
        If (EndTimeField = "") Then
            Return "The EndTimeField property is not set"
        End If
        Return String.Empty
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' create the list with all times (Start or End)
    ''' </summary>
    ''' <param name="dv"></param>
    ''' -----------------------------------------------------------------------------
    Overrides Sub FillRangeValueArray(ByRef dv As DataView)
        arrRangeValues = New ArrayList
        If (FullTimeScale) Then
            Dim tsInc As New TimeSpan(0, TimeScaleInterval, 0)
            ' ignore data, just fill the time scale
            ' add incrementing times to the array
            Dim t As TimeSpan = StartOfTimeScale
            While TimeSpan.Compare(t, EndOfTimeScale) < 0
                ' use DateTime objects for easy display
                Dim dt As DateTime = New DateTime(1, 1, 1, t.Hours, t.Minutes, 0)
                arrRangeValues.Add(dt)
                t = t.Add(tsInc)
            End While
            ' Add the end of the timescale as well to make sure it's there
            ' e.g. in the case of EndOfTimeScale=23:59 and TimeScaleInterval=1440, this is imperative
            Dim dtEnd As DateTime = New DateTime(1, 1, 1 + EndOfTimeScale.Days, EndOfTimeScale.Hours, EndOfTimeScale.Minutes, 0)
            arrRangeValues.Add(dtEnd)
        Else   ' Not FullTimeScale
            ' Just add the times from the dataview
            Dim j As Integer
            For j = 0 To dv.Count - 1
                Dim t1 As Object = dv.Item(j).Item(StartTimeField)
                Dim t2 As Object = dv.Item(j).Item(EndTimeField)
                If (Not TimeFieldsContainDate) Then
                    arrRangeValues.Add(t1)
                    arrRangeValues.Add(t2)
                Else ' TimeFieldsContainDate
                    ' both t1 and t2 should be of type DateTime now
                    If (Not TypeOf t1 Is DateTime) Then
                        Throw New HttpException("When TimeFieldsContainDate=True, StartTimeField should be of type DateTime")
                    End If
                    Dim dt1 As DateTime = CType(t1, DateTime)
                    If (Not TypeOf t2 Is DateTime) Then
                        Throw New HttpException("When TimeFieldsContainDate=True, EndTimeField should be of type DateTime")
                    End If
                    Dim dt2 As DateTime = CType(t2, DateTime)
                    ' remove date part, only store time part in array
                    arrRangeValues.Add(New DateTime(1, 1, 1, dt1.Hour, dt1.Minute, dt1.Second))
                    If (dt2.Hour > 0 Or dt2.Minute > 0 Or dt2.Second > 0) Then
                        arrRangeValues.Add(New DateTime(1, 1, 1, dt2.Hour, dt2.Minute, dt2.Second))
                    Else
                        ' if the end is 0:00:00 hour, insert 24:00:00 hour instead
                        arrRangeValues.Add(New DateTime(1, 1, 2, 0, 0, 0))
                    End If
                End If
            Next j
        End If

        arrRangeValues.Sort()
        RemoveDoubles(arrRangeValues)

    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' When TimeFieldsContainDate=True, items could span over midnight, even several days.
    ''' Split them.
    ''' </summary>
    ''' <param name="dv"></param>
    ''' <remarks>Used to be called "SplitItemsThatSpanMidnight"</remarks>
    ''' -----------------------------------------------------------------------------
    Overrides Sub PreprocessData(ByRef dv As DataView)
        ShiftStartDate()
        If (Not TimeFieldsContainDate) Then Return
        If (dv Is Nothing) Then Return ' added in v2.1.0.9
        Dim j As Integer = 0
        Dim count As Integer = dv.Count
        While (j < count)
            Dim drv As DataRowView = dv.Item(j)
            Dim dtStartValue As DateTime = CType(drv(StartTimeField), DateTime)
            Dim dtEndValue As DateTime = CType(drv(EndTimeField), DateTime)
            Dim dateStart As DateTime = New DateTime(dtStartValue.Year, dtStartValue.Month, dtStartValue.Day)
            Dim dateEnd As DateTime = New DateTime(dtEndValue.Year, dtEndValue.Month, dtEndValue.Day)
            If (dtEndValue.Hour = 0 And dtEndValue.Minute = 0 And dtEndValue.Second = 0) Then
                ' when it ends at 0:00:00 hour, it's representing 24:00 hours of the previous day
                dateEnd = dateEnd.AddDays(-1)
            End If
            ' Check if the item spans midnight. If so, split it.
            If (dateStart < dateEnd) Then
                ' the item spans midnight. We'll truncate the item first, so that it only
                ' covers the last day, and we'll add new items for the other day(s) in the loop below.
                If (FullTimeScale) Then
                    ' truncate the item by setting its start time to StartOfTimeScale
                    drv(StartTimeField) = New DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, _
                        StartOfTimeScale.Hours, StartOfTimeScale.Minutes, StartOfTimeScale.Seconds)
                Else
                    ' truncate the item by setting its start time to 0:00:00 hours
                    drv(StartTimeField) = New DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 0, 0, 0)
                End If
            End If
            While (dateStart < dateEnd)
                ' If the item spans midnight once, create an additional item for the first day.
                ' If it spans midnight several times, create additional items for each day.
                Dim drvNew As DataRowView = dv.AddNew()
                Dim i As Integer
                For i = 0 To dv.Table.Columns.Count - 1
                    drvNew(i) = drv(i)  ' copy columns one by one
                Next i
                drvNew(StartTimeField) = dtStartValue
                If (FullTimeScale) Then
                    ' set the end time to the EndOfTimeScale value
                    Dim dateEnd2 As New DateTime(dateStart.Year, dateStart.Month, dateStart.Day, _
                        EndOfTimeScale.Hours, EndOfTimeScale.Minutes, EndOfTimeScale.Seconds)
                    If (EndOfTimeScale.Equals(New TimeSpan(1, 0, 0, 0))) Then
                        ' EndOfTimeScale is 24:00 hours. Set the end at 0:00 AM of the next day.
                        ' We'll catch this case later and show the proper value.
                        dateEnd2.AddDays(1)
                    End If
                    drvNew(EndTimeField) = dateEnd2
                Else
                    ' Set the end time to 24:00 hours. This is 0:00 AM of the next day.
                    ' We'll catch this case later and show the proper value.
                    drvNew(EndTimeField) = New DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0).AddDays(1)
                End If
                drvNew.EndEdit()
                dateStart = dateStart.AddDays(1)
                If (FullTimeScale) Then
                    ' next item should start at StartOfTimeScale
                    dtStartValue = New DateTime(dateStart.Year, dateStart.Month, dateStart.Day, _
                        StartOfTimeScale.Hours, StartOfTimeScale.Minutes, StartOfTimeScale.Seconds)
                Else
                    ' next item should start at 0:00:00 hour
                    dtStartValue = New DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0)
                End If
            End While
            j = j + 1
        End While
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' calculate the TitleIndex in the table, given the objTitleValue
    ''' </summary>
    ''' <param name="objTitleValue"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------------
    Protected Overrides Function CalculateTitleIndex(ByVal objTitleValue As Object) As Integer
        CalculateTitleIndex = -1
        If (Not TypeOf objTitleValue Is Date AndAlso Not TypeOf objTitleValue Is DateTime) Then
            Throw New HttpException("DateField should be of type Date or DateTime in Calendar mode")
        End If
        Dim dtDate As DateTime = CDate(objTitleValue)
        ' remove time part, if any
        dtDate = New DateTime(dtDate.Year, dtDate.Month, dtDate.Day, 0, 0, 0)
        CalculateTitleIndex = (((dtDate.Subtract(StartDate.Date)).Days) Mod NumberOfDays) + 1 ' fix courtesy of Anthony Main
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Calculate the range cell index in the table, given the objRangeValue and the objTitleValue
    ''' The values refer to the real cell index in the table, taking into account whether cells are 
    ''' spanning over value marks (in horizontal mode)
    ''' In vertical layout, the result is the real row index in the table
    ''' In horizontal layout, the result is the real cell index in the row (before any merging 
    ''' of cells due to value spanning) 
    ''' </summary>
    ''' <param name="objRangeValue">The range value from the data source</param>
    ''' <param name="objTitleValue">The title value from the data source</param>
    ''' <param name="isEndValue">False if we're calculating the range value index for the start of the item, True if it's the end</param>
    ''' <returns>The range cell index</returns>
    ''' -----------------------------------------------------------------------------
    Protected Overrides Function CalculateRangeCellIndex(ByVal objRangeValue As Object, ByVal objTitleValue As Object, _
            ByVal isEndValue As Boolean) As Integer
        Dim RangeValueIndex As Integer = -1
        ' Find range value index by matching with range values array
        If (FullTimeScale AndAlso Not TypeOf objRangeValue Is DateTime) Then
            Throw New HttpException("The time field should be of type DateTime when FullTimeScale is set to true")
        End If
        If (TimeFieldsContainDate AndAlso Not TypeOf objRangeValue Is DateTime) Then
            Throw New HttpException("The time field should be of type DateTime when TimeFieldsContainDate is set to true")
        End If
        Dim k As Integer
        If (FullTimeScale) Then
            Dim Dobj As DateTime = CType(objRangeValue, DateTime)
            ' omit the date part for comparison
            Dobj = New DateTime(1, 1, 1, Dobj.Hour, Dobj.Minute, Dobj.Second)
            ' if no match is found, use the index of the EndOfTimeScale value
            RangeValueIndex = arrRangeValues.Count
            For k = 0 To arrRangeValues.Count - 1
                Dim Dk As DateTime = CType(arrRangeValues(k), DateTime)
                Dk = New DateTime(1, 1, 1, Dk.Hour, Dk.Minute, Dk.Second)  ' omit date part
                If (Dobj < Dk And k = 0 And isEndValue) Then
                    ' ends before start of time scale
                    Return -1
                End If
                If (Dobj >= Dk And k = arrRangeValues.Count - 1 And Not isEndValue) Then
                    ' starts at or after end of time scale
                    Return -1
                End If
                If (Dobj <= Dk) Then
                    If (k = 0 And isEndValue) Then
                        ' This can happen when the end value is 24:00:00, which will
                        ' match with the value 0:00:00 and give k=0
                        ' Instead of the value k=0, use k=arrRangeValues.Count-1
                        RangeValueIndex = arrRangeValues.Count
                    Else
                        RangeValueIndex = k + 1
                    End If
                    Exit For
                End If
            Next k
        Else ' Not FullTimeScale
            If (Not TimeFieldsContainDate) Then
                ' find the matching value in arrRangeValues
                For k = 0 To arrRangeValues.Count - 1
                    If (arrRangeValues(k).ToString() = objRangeValue.ToString()) Then
                        RangeValueIndex = k + 1
                        Exit For
                    End If
                Next k
            Else
                ' TimeFieldsContainDate=True
                Dim Dobj As DateTime = CType(objRangeValue, DateTime)
                ' omit the date part for comparison
                Dobj = New DateTime(1, 1, 1, Dobj.Hour, Dobj.Minute, Dobj.Second)
                For k = 0 To arrRangeValues.Count - 1
                    Dim Dk As DateTime = CType(arrRangeValues(k), DateTime)
                    Dk = New DateTime(1, 1, 1, Dk.Hour, Dk.Minute, Dk.Second) ' omit date part
                    If (Dobj = Dk) Then
                        If (k = 0 And isEndValue) Then
                            ' This can happen when the end value is 24:00:00, which will
                            ' match with the value 0:00:00, and give k=0
                            ' Instead of the value k=0, use k=arrRangeValues.Count-1
                            RangeValueIndex = arrRangeValues.Count
                        Else
                            RangeValueIndex = k + 1
                        End If
                        Exit For
                    End If
                Next k
            End If
        End If

        If (Not IncludeEndValue And ShowValueMarks) Then
            If (Layout = LayoutEnum.Vertical) Then
                ' Each item spans two rows
                CalculateRangeCellIndex = RangeValueIndex * 2
            Else
                ' Add one cell for the empty background cell on the left
                CalculateRangeCellIndex = RangeValueIndex + 1
            End If
        Else
            CalculateRangeCellIndex = RangeValueIndex
        End If

        ' The valueindex that we found corresponds with an item in the first week.
        ' If the item is in another week, modify the valueindex accordingly.
        ' To find out, check the date of the item
        Dim dtDate As DateTime
        If (Not TimeFieldsContainDate) Then
            ' use objTitleValue for the date
            If (Not TypeOf objTitleValue Is Date AndAlso Not TypeOf objTitleValue Is DateTime) Then
                Throw New HttpException("The date field should be of type Date or DateTime in Calendar mode when TimeFieldsContainDate=false.")
            End If
            dtDate = CDate(objTitleValue)
        Else
            ' use objRangeValue for the date
            Dim Dobj As DateTime = CType(objRangeValue, DateTime)
            dtDate = New DateTime(Dobj.Year, Dobj.Month, Dobj.Day)
            If (isEndValue And Dobj.Hour = 0 And Dobj.Minute = 0 And Dobj.Second = 0) Then
                ' when it's the end of the item and the time = 0:00 hours,
                ' it's representing 24:00 hours of the previous day
                dtDate = dtDate.AddDays(-1)
            End If
        End If
        ' if dtDate is more than NumberOfDays after StartDate, add additional rows
        Dim rowsPerWeek As Integer = 1 + arrRangeValues.Count
        If (Not IncludeEndValue And ShowValueMarks) Then rowsPerWeek = 1 + arrRangeValues.Count * 2

        CalculateRangeCellIndex = CalculateRangeCellIndex + ((dtDate.Subtract(StartDate)).Days \ NumberOfDays) * rowsPerWeek
    End Function

    Protected Overrides Function GetTitleCount() As Integer
        Return NumberOfDays       ' make a title cell for every NumberOfDays
    End Function

    Overrides Sub AddTitleHeaderData()
        Dim nTitles As Integer = GetTitleCount()

        ' iterate arrTitleValues creating a new item for each data item
        Dim titleIndex As Integer
        For titleIndex = 1 To nTitles

            Dim iWeek As Integer
            For iWeek = 0 To NumberOfRepetitions - 1
                Dim obj As Object = StartDate.AddDays(titleIndex - 1 + iWeek * NumberOfDays)
                Dim rowsPerWeek As Integer = 1 + arrRangeValues.Count
                If (Not IncludeEndValue And ShowValueMarks) Then rowsPerWeek = 1 + arrRangeValues.Count * 2
                Dim rangeIndex As Integer = iWeek * rowsPerWeek
                CreateItem(rangeIndex, titleIndex, ScheduleItemType.TitleHeader, True, obj, -1)
            Next iWeek

        Next titleIndex

    End Sub

    Protected Overrides Function GetTitleField() As String
        If (TimeFieldsContainDate) Then
            ' When TimeFieldsContainDate=true, use StartTimeField as Title
            Return StartTimeField
        Else
            Return DateField
        End If
    End Function

    Public Sub ShiftStartDate()
        If (NumberOfDays <> 7) Then Return ' change the start date only for a weekly calendar
        ' for any StartDate set by the user, shift it to the previous day indicated by the StartDay property
        ' (by default, this will be the previous Monday)
        StartDate = StartDate.AddDays(CInt(StartDay) - CInt(StartDate.DayOfWeek()))
        If (CInt(StartDay) > CInt(StartDate.DayOfWeek())) Then
            StartDate = StartDate.AddDays(-7)
        End If
        ' StartDate should be on the day of the week indicated by StartDay now
    End Sub

    Public Overrides Function GetSortOrder() As String
        ' make sure the data is processed in the right order: from bottom right up to top left.
        If (Not TimeFieldsContainDate) Then
            Return DateField & " ASC, " & StartTimeField & " ASC, " & EndTimeField & " ASC"
        Else
            ' In Calendar Mode with TimeFieldsContainDate=True, there is no DateField
            Return StartTimeField & " ASC, " & EndTimeField & " ASC"
        End If
    End Function

    Overrides Function GetRepetitionCount() As Integer
        Return NumberOfRepetitions
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' when there's no template, try to present the data in a reasonable format
    ''' </summary>
    ''' <param name="value">Value of the item</param>
    ''' <param name="type">Type of the item</param>
    ''' <returns>A formatted string</returns>
    ''' -----------------------------------------------------------------------------
    Protected Overrides Function FormatDataValue(ByVal value As Object, ByVal type As ScheduleItemType) As String
        Dim retVal As String = String.Empty
        If (Not value Is Nothing) Then
            If type = ScheduleItemType.TitleHeader Then
                If DateFormatString.Length = 0 Then
                    retVal = value.ToString()
                Else
                    retVal = String.Format(DateFormatString, value)
                End If
            ElseIf type = ScheduleItemType.RangeHeader Then
                If TimeFormatString.Length = 0 Then
                    retVal = value.ToString()
                Else
                    retVal = String.Format(TimeFormatString, value)
                End If
            Else
                retVal = value.ToString()
            End If
        End If
        Return retVal
    End Function

    Protected Overrides Function GetTemplate(ByVal type As ScheduleItemType) As ITemplate
        Select Case type
            Case ScheduleItemType.RangeHeader
                Return TimeTemplate
            Case ScheduleItemType.TitleHeader
                Return DateTemplate
            Case ScheduleItemType.Item
                Return ItemTemplate
            Case ScheduleItemType.AlternatingItem
                Return ItemTemplate
        End Select
        Return Nothing
    End Function

    '''' -----------------------------------------------------------------------------
    '''' <summary>
    '''' Calculate the title value given the cell index
    '''' </summary>
    '''' <param name="titleIndex">Title index of the cell</param>
    '''' <returns>Object containing the title</returns>
    '''' -----------------------------------------------------------------------------
    Public Overrides Function CalculateTitle(ByVal titleIndex As Integer, ByVal cellIndex As Integer) As Object
        Dim cellsPerWeek As Integer
        If (Not IncludeEndValue And ShowValueMarks) Then
            cellsPerWeek = arrRangeValues.Count * 2 + 1
        Else
            cellsPerWeek = arrRangeValues.Count + 1
        End If
        Dim week As Integer = cellIndex \ cellsPerWeek
        Return StartDate.AddDays(titleIndex - 1 + week * 7)
    End Function

#End Region

End Class
