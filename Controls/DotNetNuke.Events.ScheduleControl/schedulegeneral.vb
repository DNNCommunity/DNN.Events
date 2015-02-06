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
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Microsoft.VisualBasic   ' needed for IIf definition
Imports System.ComponentModel

''' -----------------------------------------------------------------------------
''' Project	 : schedule
''' Class	 : ScheduleGeneral
'''
''' -----------------------------------------------------------------------------
''' <summary>
''' The ScheduleGeneral web control is designed to represent a schedule in a general format.
''' </summary>
''' -----------------------------------------------------------------------------
<ParseChildren(True)> _
    Public Class ScheduleGeneral : Inherits BaseSchedule

#Region "Private and protected properties"
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' list with values to be shown in title header
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Protected Property arrTitleValues() As ArrayList
        Get
            Return CType(ViewState("arrTitleValues"), ArrayList)
        End Get
        Set(ByVal value As ArrayList)
            ViewState("arrTitleValues") = value
        End Set
    End Property
#End Region

#Region "Public properties"

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' When true, a separate header will be added for the date. 
    ''' This requires DataRangeStartField and DataRangeEndField to be of type DateTime.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("When true, a separate header will be added for the date. This requires DataRangeStartField and DataRangeEndField to be of type DateTime."), _
    DefaultValue(False), Bindable(False), Category("Behavior")> _
    Public Overridable Property SeparateDateHeader() As Boolean
        Get
            Dim o As Object = ViewState("SeparateDateHeader")
            If Not (o Is Nothing) Then
                Return CBool(o)
            End If
            Return False
        End Get
        Set(ByVal Value As Boolean)
            ViewState("SeparateDateHeader") = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The format used for the title if the TitleTemplate is missing, e.g. {0:ddd d}
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("The format used for the title if the TitleTemplate is missing, e.g. {0:ddd d}"), _
    DefaultValue(""), Category("Data")> _
    Public Property TitleDataFormatString() As String
        Get
            Dim o As Object = ViewState("TitleDataFormatString")
            If Not (o Is Nothing) Then Return CStr(o)
            Return String.Empty
        End Get
        Set(ByVal Value As String)
            ViewState("TitleDataFormatString") = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The format used for the ranges if the RangeHeaderTemplate is missing, e.g. {0:hh:mm}
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("The format used for the ranges if the RangeHeaderTemplate is missing, e.g. {0:hh:mm}"), _
    DefaultValue(""), Category("Data")> _
    Public Property RangeDataFormatString() As String
        Get
            Dim o As Object = ViewState("RangeDataFormatString")
            If Not (o Is Nothing) Then Return CStr(o)
            Return String.Empty
        End Get
        Set(ByVal Value As String)
            ViewState("RangeDataFormatString") = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The format used for the date header if SeparateDateHeader=True and the DateHeaderTemplate is missing, e.g. {0:dd/MM}
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("The format used for the date header if SeparateDateHeader=true and the DateHeaderTemplate is missing, e.g. {0:dd/MM}"), _
    DefaultValue(""), Category("Data")> _
    Public Property DateHeaderDataFormatString() As String
        Get
            Dim o As Object = ViewState("DateHeaderDataFormatString")
            If Not (o Is Nothing) Then Return CStr(o)
            Return String.Empty
        End Get
        Set(ByVal Value As String)
            ViewState("DateHeaderDataFormatString") = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' When true, titles will automatically be sorted alphabetically. 
    ''' When false, you may provide your own sorting order for the titles, but make sure 
    ''' that the items with the same titles are grouped together, and that for each title, 
    ''' the items are sorted on DataRangeStartField first and on DataRangeEndField next. 
    ''' (for example: if you want to sort on a field called "SortOrder", the 
    ''' DataRangeStartField is "StartTime", and the DataRangeEndField is "EndTime", 
    ''' use the sorting expression "ORDER BY SortOrder ASC, StartTime ASC, EndTime ASC") 
    ''' The default value for AutoSortTitles is true. 
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("When true, titles will automatically be sorted alphabetically. When false, the data source should be sorted properly before binding."), _
    DefaultValue(True), Bindable(False), Category("Behavior")> _
    Public Overridable Property AutoSortTitles() As Boolean
        Get
            Dim o As Object = ViewState("AutoSortTitles")
            If Not (o Is Nothing) Then
                Return CBool(o)
            End If
            Return True
        End Get
        Set(ByVal Value As Boolean)
            ViewState("AutoSortTitles") = Value
        End Set
    End Property

#End Region

#Region "Templates"

    Private _TitleTemplate As ITemplate
    Private _RangeHeaderTemplate As ITemplate
    Private _dateHeaderTemplate As ITemplate   ' only used when DisplayMode=General and SeparateDateHeader=True

    <TemplateContainer(GetType(ScheduleItem)), Browsable(False), _
     Description("The template used to create title header content."), _
     NotifyParentProperty(True), _
     PersistenceMode(PersistenceMode.InnerProperty)> _
    Public Property TitleTemplate() As ITemplate
        Get
            Return _TitleTemplate
        End Get
        Set(ByVal Value As ITemplate)
            _TitleTemplate = Value
        End Set
    End Property

    <TemplateContainer(GetType(ScheduleItem)), Browsable(False), _
     Description("The template used to create range header content."), _
     PersistenceMode(PersistenceMode.InnerProperty)> _
    Public Property RangeHeaderTemplate() As ITemplate
        Get
            Return _RangeHeaderTemplate
        End Get
        Set(ByVal Value As ITemplate)
            _RangeHeaderTemplate = Value
        End Set
    End Property

    <TemplateContainer(GetType(ScheduleItem)), Browsable(False), _
     Description("The template used to create date header content."), _
     PersistenceMode(PersistenceMode.InnerProperty)> _
    Public Property DateHeaderTemplate() As ITemplate
        Get
            Return _dateHeaderTemplate
        End Get
        Set(ByVal Value As ITemplate)
            _dateHeaderTemplate = Value
        End Set
    End Property

#End Region

#Region "Methods"
    ' Check if all properties are set to make the control work
    Overrides Function CheckConfiguration() As String
        If (TitleField = "") Then
            Return "The TitleField property is not set"
        End If
        If (DataRangeStartField = "") Then
            Return "The DataRangeStartField property is not set"
        End If
        If (DataRangeEndField = "") Then
            Return "The DataRangeEndField property is not set"
        End If
        Return String.Empty
    End Function
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' create the list with all range header values (Start or End)
    ''' </summary>
    ''' <param name="dv"></param>
    ''' -----------------------------------------------------------------------------
    Overrides Sub FillRangeValueArray(ByRef dv As DataView)
        arrRangeValues = New ArrayList
        Dim strOldSort As String = dv.Sort
        If (FullTimeScale) Then
            Dim tsInc As New TimeSpan(0, TimeScaleInterval, 0)
            If (dv.Count = 0) Then Return ' empty database
            dv.Sort = DataRangeStartField & " ASC "
            ' Nulls are allowed (for creating titles without content) but will not show up
            Dim i As Integer = 0
            While (i < dv.Count AndAlso IsDBNull(dv.Item(i).Item(DataRangeStartField)))
                i = i + 1
            End While
            If (i >= dv.Count) Then Return
            Dim dt1 As DateTime = CType(dv.Item(i).Item(DataRangeStartField), DateTime) ' first start time in dataview
            dv.Sort = DataRangeEndField & " DESC "
            i = 0
            While (IsDBNull(dv.Item(i).Item(DataRangeStartField)))
                i = i + 1
            End While
            Dim dt2 As DateTime = CType(dv.Item(i).Item(DataRangeEndField), DateTime) ' last end time in dataview
            ' add incrementing times to the array
            While dt1 <= dt2
                Dim t As TimeSpan = StartOfTimeScale
                While TimeSpan.Compare(t, EndOfTimeScale) < 0
                    Dim dt As DateTime = New DateTime(dt1.Year, dt1.Month, dt1.Day, t.Hours, t.Minutes, 0)
                    arrRangeValues.Add(dt)
                    t = t.Add(tsInc)
                End While
                ' Add the end of the timescale as well to make sure it's there
                ' e.g. in the case of EndOfTimeScale=23:59 and TimeScaleInterval=1440, this is imperative
                Dim dtEnd As DateTime = New DateTime(dt1.Year, dt1.Month, dt1.Day, EndOfTimeScale.Hours, EndOfTimeScale.Minutes, 0)
                arrRangeValues.Add(dtEnd)
                dt1 = dt1.AddDays(1)
            End While
        Else   ' Not FullTimeScale
            ' Just add the times from the dataview
            Dim j As Integer
            For j = 0 To dv.Count - 1
                ' Nulls are allowed (for creating titles without content) but will not show up
                If (Not IsDBNull(dv.Item(j).Item(DataRangeStartField))) Then
                    Dim t1 As Object = dv.Item(j).Item(DataRangeStartField)
                    Dim t2 As Object = dv.Item(j).Item(DataRangeEndField)
                    arrRangeValues.Add(t1)
                    arrRangeValues.Add(t2)
                End If
            Next j
        End If

        arrRangeValues.Sort()
        RemoveDoubles(arrRangeValues)
        dv.Sort = strOldSort
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' create the list with all titles
    ''' </summary>
    ''' <param name="dv"></param>
    ''' -----------------------------------------------------------------------------
    Overrides Sub FillTitleValueArray(ByRef dv As DataView)
        arrTitleValues = New ArrayList
        Dim i As Integer
        For i = 0 To dv.Count - 1
            Dim val As Object = dv.Item(i).Item(TitleField)
            arrTitleValues.Add(val)
        Next i
        If (AutoSortTitles) Then
            arrTitleValues.Sort()
        End If
        RemoveDoubles(arrTitleValues)
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Add date headers to the table when SeparateDateHeader=True
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Overrides Sub AddDateHeaderData()
        If (Not SeparateDateHeader) Then Return
        ' merge all cells having the same date in the first (date) header
        If (arrRangeValues.Count = 0) Then Return
        If (Not TypeOf arrRangeValues(0) Is DateTime AndAlso Not TypeOf arrRangeValues(0) Is Date) Then
            Throw New HttpException("If SeparateDateHeader is true, then DataRangeStartField " & _
                " and DataRangeEndField need to be of type DateTime")
        End If
        If (Layout = LayoutEnum.Horizontal) Then
            ' In horizontal mode, add an extra row for date headers
            Table1.Rows.AddAt(0, New TableRow())
        End If
        Table1.Rows(0).Cells.AddAt(0, New TableHeaderCell)
        Table1.Rows(0).Cells(0).ApplyStyle(TitleStyle)

        Dim prevRangeValue As DateTime = CDate(arrRangeValues(arrRangeValues.Count - 1))
        Dim prevStartValueIndex As Integer = arrRangeValues.Count
        If (Not IncludeEndValue And ShowValueMarks) Then prevStartValueIndex = arrRangeValues.Count * 2 - 1

        Dim i As Integer
        For i = arrRangeValues.Count - 1 To 0 Step -1
            Dim arrRangeValue As DateTime = CDate(arrRangeValues(i))
            If (arrRangeValue.Date <> prevRangeValue.Date) Then
                ' this value has another date than the previous one (the one below or to the right)
                ' add a cell below or to the right which spans the cells that have the same date
                Dim ValueIndexOfNextCell As Integer = i + 2 ' add 1 for the title cell and 1 because it's the next cell, not this one
                Dim Span As Integer = prevStartValueIndex - ValueIndexOfNextCell + 1

                If (Not IncludeEndValue And ShowValueMarks) Then
                    ValueIndexOfNextCell = i * 2 + 3
                    Span = prevStartValueIndex - ValueIndexOfNextCell + 2
                End If

                Dim cell As TableCell
                If (Layout = LayoutEnum.Vertical) Then
                    Table1.Rows(ValueIndexOfNextCell).Cells.AddAt(0, New TableHeaderCell())
                    cell = Table1.Rows(ValueIndexOfNextCell).Cells(0)
                    cell.RowSpan = Span
                Else ' Horizontal
                    Table1.Rows(0).Cells.AddAt(1, New TableHeaderCell())
                    cell = Table1.Rows(0).Cells(1)
                    cell.ColumnSpan = Span
                End If
                cell.ApplyStyle(RangeHeaderStyle)
                prevRangeValue = arrRangeValue
                prevStartValueIndex = i + 1
                If (Not IncludeEndValue And ShowValueMarks) Then prevStartValueIndex = i * 2 + 1
            End If
        Next i
        ' finish by adding the first cell also
        Dim cell0 As TableCell
        Dim Span0 As Integer = prevStartValueIndex
        If (Not IncludeEndValue And ShowValueMarks) Then Span0 += 1

        If (Layout = LayoutEnum.Vertical) Then
            Table1.Rows(1).Cells.AddAt(0, New TableHeaderCell())
            cell0 = Table1.Rows(1).Cells(0)
            cell0.RowSpan = Span0
        Else ' Horizontal
            Table1.Rows(0).Cells.AddAt(1, New TableHeaderCell())
            cell0 = Table1.Rows(0).Cells(1)
            cell0.ColumnSpan = Span0
        End If
        cell0.ApplyStyle(RangeHeaderStyle)

        ' iterate arrRangeValues in forward direction creating a new item for each data item
        ' forward because it has to be in the same order as after postback, and there it's\
        ' much easier if it's forward
        Dim cellIndex As Integer = 1
        i = 0
        While i < arrRangeValues.Count
            Dim arrRangeValue As DateTime = CDate(arrRangeValues(i))
            CreateItem(cellIndex, 0, ScheduleItemType.DateHeader, True, arrRangeValue, -1)
            Dim Span As Integer = GetValueSpan(cellIndex, 0)
            If (Span = 0) Then Span = 1
            If Layout = LayoutEnum.Horizontal Then
                cellIndex += 1
            Else
                cellIndex += Span
            End If
            If (Not IncludeEndValue And ShowValueMarks) Then
                i += Span \ 2
            Else
                i += Span
            End If
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
        ' Find the title index by matching with the title values array
        Dim k As Integer
        For k = 0 To arrTitleValues.Count - 1
            If (arrTitleValues(k).ToString() = objTitleValue.ToString()) Then
                Return k + 1
            End If
        Next k
        Return -1
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
        If (IsDBNull(objRangeValue)) Then Return -1
        Dim RangeValueIndex As Integer = -1
        ' Find range value index by matching with range values array
        If (FullTimeScale AndAlso Not TypeOf objRangeValue Is DateTime) Then
            Throw New HttpException("The range field should be of type DateTime when FullTimeScale is set to true")
        End If
        Dim k As Integer
        If (FullTimeScale) Then
            Dim Dobj As DateTime = CType(objRangeValue, DateTime)
            ' if no match is found, use the index of the EndOfTimeScale value
            RangeValueIndex = arrRangeValues.Count
            For k = 0 To arrRangeValues.Count - 1
                Dim Dk As DateTime = CType(arrRangeValues(k), DateTime)
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
            ' find the matching value in arrRangeValues
            For k = 0 To arrRangeValues.Count - 1
                If (arrRangeValues(k).ToString() = objRangeValue.ToString()) Then
                    RangeValueIndex = k + 1
                    Exit For
                End If
            Next k
        End If
        If (Not IncludeEndValue And ShowValueMarks) Then
            If (Layout = LayoutEnum.Vertical) Then
                ' Each item spans two rows
                Return RangeValueIndex * 2
            Else
                ' Add one cell for the empty background cell on the left
                Return RangeValueIndex + 1
            End If
        Else
            Return RangeValueIndex
        End If
    End Function

    Protected Overrides Function GetTitleCount() As Integer
        Return arrTitleValues.Count
    End Function

    Overrides Function GetRangeHeaderIndex() As Integer
        ' when SeparateDateHeader=True, the first index (column or row) is the date header,
        ' the second (column or row) contains the range values
        Return CInt(IIf(SeparateDateHeader, 1, 0))
    End Function

    Overrides Sub AddTitleHeaderData()
        Dim nTitles As Integer = GetTitleCount()

        ' iterate arrTitleValues creating a new item for each data item
        Dim titleIndex As Integer
        For titleIndex = 1 To nTitles
            Dim obj As Object = arrTitleValues.Item(titleIndex - 1)
            CreateItem(0, titleIndex, ScheduleItemType.TitleHeader, True, obj, -1)
        Next titleIndex
    End Sub

    Public Overrides Function GetSortOrder() As String
        ' make sure the data is processed in the right order: from bottom right up to top left.
        If (AutoSortTitles) Then
            Return TitleField & " ASC, " & DataRangeStartField & " ASC, " & DataRangeEndField & " ASC"
        Else
            Return ""  ' leave sort order as it is when AutoSortTitles=False
        End If
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
                If TitleDataFormatString.Length = 0 Then
                    retVal = value.ToString()
                Else
                    retVal = String.Format(TitleDataFormatString, value)
                End If
            ElseIf type = ScheduleItemType.RangeHeader Then
                If RangeDataFormatString.Length = 0 Then
                    retVal = value.ToString()
                Else
                    retVal = String.Format(RangeDataFormatString, value)
                End If
            ElseIf type = ScheduleItemType.DateHeader Then
                If DateHeaderDataFormatString.Length = 0 Then
                    retVal = value.ToString()
                Else
                    retVal = String.Format(DateHeaderDataFormatString, value)
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
                Return RangeHeaderTemplate
            Case ScheduleItemType.TitleHeader
                Return TitleTemplate
            Case ScheduleItemType.DateHeader
                Return DateHeaderTemplate
            Case ScheduleItemType.Item
                Return ItemTemplate
            Case ScheduleItemType.AlternatingItem
                Return ItemTemplate
        End Select
        Return Nothing
    End Function

    Protected Overrides Function GetStyle(ByVal type As ScheduleItemType) As TableItemStyle
        ' handle DateHeader, which is not handled in the base class
        If (type = ScheduleItemType.DateHeader) Then
            Return RangeHeaderStyle
        End If
        Return MyBase.GetStyle(type)
    End Function

    '''' -----------------------------------------------------------------------------
    '''' <summary>
    '''' Calculate the title (data source value) given the cell index
    '''' </summary>
    '''' <param name="titleIndex">Title index of the cell</param>
    '''' <returns>Object containing the title</returns>
    '''' -----------------------------------------------------------------------------
    Public Overrides Function CalculateTitle(ByVal titleIndex As Integer, ByVal cellIndex As Integer) As Object
        Return arrTitleValues(titleIndex - 1)
    End Function

#End Region

End Class

