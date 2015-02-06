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

<CLSCompliant(True)> _
Public Enum ScheduleItemType As Integer
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Standard item in a schedule
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Item = 0
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Alternating standard item in a schedule
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    AlternatingItem
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Item in the range header column or row. In the derived ScheduleCalendar,
    ''' this is an item in the time header.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    RangeHeader
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Item in the title header column or row. In the derived ScheduleCalendar,
    ''' this is an item in the date header.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    TitleHeader
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Item in the optional date header column or row.
    ''' Only used in the derived ScheduleGeneral control.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    DateHeader
End Enum

Public Enum LayoutEnum
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' In Horizontal layout, the range values (times in ScheduleCalendar) are shown horizontally
    ''' in the first row, and the titles (dates in ScheduleCalendar) are shown vertically in the first column.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Horizontal = 0
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' In Vertical layout, the range values (times in ScheduleCalendar) are shown vertically
    ''' in the first column, and the titles (dates in ScheduleCalendar) are shown horizontally in the first row.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Vertical
End Enum

''' -----------------------------------------------------------------------------
''' Project	 : schedule
''' Class	 : BaseSchedule
'''
''' -----------------------------------------------------------------------------
''' <summary>
''' BaseSchedule is the base class for the ScheduleGeneral and ScheduleCalendar controls
''' </summary>
''' -----------------------------------------------------------------------------
#If NET_2_0 = 1 Then
    <ParseChildren(True), _
    ControlValueProperty("SelectedValue")> _
    Public MustInherit Class BaseSchedule
        Inherits CompositeDataBoundControl
#Else ' .NET 1.x
<ParseChildren(True)> _
Public MustInherit Class BaseSchedule
    Inherits WebControl
    Implements INamingContainer
#End If
    Implements IPostBackEventHandler

#Region "Private and protected properties"
#If Not NET_2_0 = 1 Then
    Private _dataSource As Object
#End If
    Private _items As ScheduleItemCollection
    Private _itemsArrayList As ArrayList

    Protected Table1 As Table

    Private _itemStyle As TableItemStyle
    Private _alternatingItemStyle As TableItemStyle
    Private _RangeHeaderStyle As TableItemStyle
    Private _TitleStyle As TableItemStyle
    Private _BackgroundStyle As TableItemStyle

    Private _emptySlotToolTip As String = "Click here to add data"

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' list with values to be shown in row header
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Protected Property arrRangeValues() As ArrayList
        Get
            Return CType(ViewState("arrRangeValues"), ArrayList)
        End Get
        Set(ByVal value As ArrayList)
            ViewState("arrRangeValues") = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Items is a collection object that contains the ScheduleItem objects
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Protected Overridable ReadOnly Property Items() As ScheduleItemCollection
        Get
            If _items Is Nothing Then
                If _itemsArrayList Is Nothing Then
                    EnsureChildControls()
                End If
                _items = New ScheduleItemCollection(_itemsArrayList)
            End If
            Return _items
        End Get
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Whether to show the EmptyDataTemplate or not when no data is found
    ''' </summary>
    ''' <remarks>
    ''' Overridden in ScheduleCalendar when FullTimeScale=True
    ''' </remarks>
    ''' -----------------------------------------------------------------------------
    Protected Overridable ReadOnly Property ShowEmptyDataTemplate() As Boolean
        Get
            Return True ' Overridden in ScheduleCalendar 
        End Get
    End Property

#End Region

#Region "Public properties"
    <Description("The direction in which the item ranges are shown."), _
    DefaultValue(LayoutEnum.Vertical), Bindable(False), Category("Appearance")> _
    Public Property Layout() As LayoutEnum
        Get
            Dim o As Object = ViewState("Layout")
            If Not (o Is Nothing) Then
                Return CType(o, LayoutEnum)
            End If
            Return LayoutEnum.Vertical
        End Get
        Set(ByVal Value As LayoutEnum)
            ViewState("Layout") = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' If IncludeEndValue is true, the event is shown including the end row or column
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("If true, the event is shown including the end row or column"), _
    DefaultValue(False), Bindable(False), Category("Behavior")> _
    Public Property IncludeEndValue() As Boolean
        Get
            Dim o As Object = ViewState("IncludeEndValue")
            If Not (o Is Nothing) Then
                Return CBool(o)
            End If
            Return False
        End Get
        Set(ByVal Value As Boolean)
            ViewState("IncludeEndValue") = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' If ShowValueMarks is true, value marks will be shown in the range header column or row.
    ''' Applied only when IncludeEndValue is false.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("If true, value marks will be shown in the range header column or row. Applied only when IncludeEndValue is false."), _
    DefaultValue(False), Bindable(False), Category("Behavior")> _
    Public Property ShowValueMarks() As Boolean
        Get
            Dim o As Object = ViewState("ShowValueMarks")
            If Not (o Is Nothing) Then
                Return CBool(o)
            End If
            Return False
        End Get
        Set(ByVal Value As Boolean)
            ViewState("ShowValueMarks") = Value
        End Set
    End Property

#If Not NET_2_0 = 1 Then

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The data source that is used to show items in the schedule
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("The data source that is used to show items in the schedule"), _
    Bindable(True), DefaultValue(Nothing, ""), _
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), _
    Category("Data")> _
    Public Property DataSource() As Object
        Get
            Return _dataSource
        End Get
        Set(ByVal Value As Object)
            If ((Value Is Nothing) OrElse (TypeOf Value Is DataTable) OrElse (TypeOf Value Is DataSet) OrElse (TypeOf Value Is DataView)) Then
                _dataSource = Value
            Else
                Throw New HttpException("The DataSource must be a DataTable, DataSet or DataView object")
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The table or view used for binding when a DataSet is used as a data source.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("The table or view used for binding when a DataSet is used as a data source."), _
    DefaultValue(""), Category("Data")> _
    Public Property DataMember() As String
        Get
            Dim o As Object = ViewState("DataMember")
            If Not (o Is Nothing) Then
                Return CStr(o)
            End If
            Return String.Empty
        End Get
        Set(ByVal Value As String)
            ViewState("DataMember") = Value
        End Set
    End Property
#End If
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The database field containing the start of the items.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("The database field containing the start of the items."), _
    Bindable(False), Category("Data")> _
    Public Property DataRangeStartField() As String
        Get
            Dim o As Object = ViewState("DataRangeStartField")
            If Not (o Is Nothing) Then Return CStr(o)
            Return ""
        End Get
        Set(ByVal Value As String)
            ViewState("DataRangeStartField") = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The database field containing the end of the items.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("The database field containing the end of the items."), _
    Bindable(False), Category("Data")> _
    Public Property DataRangeEndField() As String
        Get
            Dim o As Object = ViewState("DataRangeEndField")
            If Not (o Is Nothing) Then Return CStr(o)
            Return ""
        End Get
        Set(ByVal Value As String)
            ViewState("DataRangeEndField") = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The database field providing the titles.
    ''' In Calendar mode this field should be of type Date when TimeFieldsContainDate=false.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("The database field providing the titles. In Calendar mode this field should be of type Date when TimeFieldsContainDate=false."), _
    Bindable(False), Category("Data")> _
    Public Property TitleField() As String
        Get
            Dim o As Object = ViewState("TitleField")
            If Not (o Is Nothing) Then Return CStr(o)
            Return ""
        End Get
        Set(ByVal Value As String)
            ViewState("TitleField") = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' An optional database field providing the item styles (in the form of a css class name).
    ''' If not provided, then the ItemStyle property will be used for all items.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("Optional database field providing the item styles.  If not provided, then the ItemStyle property will be used for all items."), _
    Bindable(False), Category("Data")> _
    Public Property ItemStyleField() As String
        Get
            Dim o As Object = ViewState("ItemStyleField")
            If Not (o Is Nothing) Then Return CStr(o)
            Return ""
        End Get
        Set(ByVal Value As String)
            ViewState("ItemStyleField") = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Boolean value indicating if a full time scale should be shown.
    ''' If true, show a full time scale.
    ''' If false, show only the occurring values in the data source.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("If true, show a full time scale. If false, show only the occurring values in the data source."), _
    DefaultValue(False), Bindable(True), Category("Behavior")> _
    Public Property FullTimeScale() As Boolean
        Get
            Dim o As Object = ViewState("FullTimeScale")
            If Not (o Is Nothing) Then Return CBool(o)
            Return False
        End Get
        Set(ByVal Value As Boolean)
            ViewState("FullTimeScale") = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The number of minutes between each mark on the time scale. Only used when FullTimeScale is true.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("The number of minutes between each mark on the time scale. Only used when FullTimeScale is true."), _
     DefaultValue(60), Bindable(True), Category("Behavior")> _
     Public Property TimeScaleInterval() As Integer
        Get
            Dim o As Object = ViewState("TimeScaleInterval")
            If Not (o Is Nothing) Then Return CInt(o)
            Return 60
        End Get
        Set(ByVal Value As Integer)
            If (Value = 0) Then
                Throw New HttpException("TimeScaleInterval can not be 0")
            End If
            ViewState("TimeScaleInterval") = Value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The start of the time scale. Only used when FullTimeScale is true.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("The start of the time scale. Only used when FullTimeScale is true."), _
    DefaultValue("8:00:00"), Bindable(True), Category("Behavior")> _
    Public Property StartOfTimeScale() As TimeSpan
        Get
            Dim o As Object = ViewState("StartOfTimeScale")
            If Not (o Is Nothing) Then Return CType(o, TimeSpan)
            If (FullTimeScale) Then Return New TimeSpan(0, 0, 0) ' = 0:00
            Return New TimeSpan(8, 0, 0) ' = 8:00 AM
        End Get
        Set(ByVal Value As TimeSpan)
            If (Value.Days = 0) Then
                ViewState("StartOfTimeScale") = Value
            Else
                Throw New HttpException("StartOfTimeScale should be between 0:00:00 and 23:59:00")
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The end of the time scale. Only used when FullTimeScale is true.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("The end of the time scale. Only used when FullTimeScale is true."), _
    DefaultValue("17:00:00"), Bindable(True), Category("Behavior")> _
    Public Property EndOfTimeScale() As TimeSpan
        Get
            Dim o As Object = ViewState("EndOfTimeScale")
            If Not (o Is Nothing) Then Return CType(o, TimeSpan)
            If (FullTimeScale) Then Return New TimeSpan(1, 0, 0, 0) ' = 24:00
            Return New TimeSpan(17, 0, 0) ' = 5:00 PM
        End Get
        Set(ByVal Value As TimeSpan)
            If (Value.Days = 0 Or Value.Equals(New TimeSpan(1, 0, 0, 0))) Then
                ViewState("EndOfTimeScale") = Value
            Else
                Throw New HttpException("EndOfTimeScale should be between 0:00:00 and 1.00:00:00 (24h)")
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The cell padding of the rendered table.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Bindable(True), Category("Appearance"), DefaultValue(-1), _
       Description("The cell padding of the rendered table.")> _
    Public Overridable Property CellPadding() As Integer
        Get
            If ControlStyleCreated = False Then Return -1
            Return CType(ControlStyle, TableStyle).CellPadding
        End Get
        Set(ByVal Value As Integer)
            If (TypeOf ControlStyle Is TableStyle) Then
                CType(ControlStyle, TableStyle).CellPadding = Value
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The cell spacing of the rendered table.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Bindable(True), Category("Appearance"), DefaultValue(1), _
       Description("The cell spacing of the rendered table.")> _
    Public Overridable Property CellSpacing() As Integer
        Get
            If ControlStyleCreated = False Then Return 1
            Return CType(ControlStyle, TableStyle).CellSpacing
        End Get
        Set(ByVal Value As Integer)
            If (TypeOf ControlStyle Is TableStyle) Then
                CType(ControlStyle, TableStyle).CellSpacing = Value
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The grid lines to be shown in the rendered table.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Bindable(True), Category("Appearance"), _
       DefaultValue(System.Web.UI.WebControls.GridLines.None), _
       Description("The grid lines to be shown in the rendered table.")> _
    Public Overridable Property GridLines() As GridLines
        Get
            If ControlStyleCreated = False Then Return GridLines.None
            Return CType(ControlStyle, TableStyle).GridLines
        End Get
        Set(ByVal Value As GridLines)
            If (TypeOf ControlStyle Is TableStyle) Then
                CType(ControlStyle, TableStyle).GridLines = Value
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The horizontal alignment of text in the table.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Bindable(True), Category("Appearance"), _
       DefaultValue(System.Web.UI.WebControls.HorizontalAlign.NotSet), _
       Description("The horizontal alignment of text in the table.")> _
    Public Overridable Property HorizontalAlign() As HorizontalAlign
        Get
            If ControlStyleCreated = False Then Return HorizontalAlign.NotSet
            Return CType(ControlStyle, TableStyle).HorizontalAlign
        End Get
        Set(ByVal Value As HorizontalAlign)
            If (TypeOf ControlStyle Is TableStyle) Then
                CType(ControlStyle, TableStyle).HorizontalAlign = Value
            End If
        End Set
    End Property

#If NET_2_0 = 1 Then
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the data key value of the selected item in a Schedule control 
        ''' </summary>
        ''' -----------------------------------------------------------------------------
        <Browsable(False)> _
        Public Property SelectedValue() As Object
            Get
                Return ViewState("SelectedValue")
            End Get
            Set(ByVal value As Object)
                ViewState("SelectedValue") = value
            End Set
        End Property

        <Category("Data"), _
        TypeConverter(GetType(StringArrayConverter))> _
        Public Property DataKeyNames() As String()
            Get
                Dim o As Object = ViewState("DataKeyNames")
                If (o [IsNot] Nothing) Then Return CType(o, String())
                Dim emptyKeys(0) As String
                Return emptyKeys
            End Get
            Set(ByVal value As String())
                ViewState("DataKeyNames") = value
                'RequiresDataBinding = True
            End Set
        End Property

#End If

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' When true, clicking on an empty slot in the control will raize the OnEmptySlotClick event.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("When true, clicking on an empty slot in the control will raize the OnEmptySlotClick event."), _
    DefaultValue(False), Bindable(False), Category("Behavior")> _
    Public Property EnableEmptySlotClick() As Boolean
        Get
            Dim o As Object = ViewState("EnableEmptySlotClick")
            If Not (o Is Nothing) Then Return CBool(o)
            Return False
        End Get
        Set(ByVal value As Boolean)
            ViewState("EnableEmptySlotClick") = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ToolTip on empty slots. Only shown when EmptySlotClickEnabled is true.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("ToolTip on empty slots. Only shown when EmptySlotClickEnabled is true."), _
    DefaultValue("Click here to add data"), Bindable(False), Category("Behavior")> _
    Public Property EmptySlotToolTip() As String
        Get
            Return Me._emptySlotToolTip
        End Get
        Set(ByVal value As String)
            Me._emptySlotToolTip = value
        End Set
    End Property

    Protected Overrides ReadOnly Property TagKey() As System.Web.UI.HtmlTextWriterTag
        Get
            Return HtmlTextWriterTag.Div
        End Get
    End Property

#End Region

#Region "Styles"

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The style applied to schedule items.
    ''' </summary>

    <Description("The style applied to schedule items. "), _
        Bindable(False), Category("Style"), _
        NotifyParentProperty(True), _
        PersistenceMode(PersistenceMode.InnerProperty)> _
    Public Overridable ReadOnly Property ItemStyle() As TableItemStyle
        Get
            If _itemStyle Is Nothing Then
                _itemStyle = New TableItemStyle
                If IsTrackingViewState Then
                    CType(_itemStyle, IStateManager).TrackViewState()
                End If
            End If
            Return _itemStyle
        End Get
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The style applied to alternating schedule items.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("The style applied to alternating schedule items. "), _
        Bindable(False), Category("Style"), _
        NotifyParentProperty(True), _
        PersistenceMode(PersistenceMode.InnerProperty)> _
    Public Overridable ReadOnly Property AlternatingItemStyle() As TableItemStyle
        Get
            If _alternatingItemStyle Is Nothing Then
                _alternatingItemStyle = New TableItemStyle
                If IsTrackingViewState Then
                    CType(_alternatingItemStyle, IStateManager).TrackViewState()
                End If
            End If
            Return _alternatingItemStyle
        End Get
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The style applied to range header items.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("The style applied to range header items. "), _
        Bindable(False), Category("Style"), _
        NotifyParentProperty(True), _
        PersistenceMode(PersistenceMode.InnerProperty)> _
    Public Overridable ReadOnly Property RangeHeaderStyle() As TableItemStyle
        Get
            If _RangeHeaderStyle Is Nothing Then
                _RangeHeaderStyle = New TableItemStyle
                If IsTrackingViewState Then
                    CType(_RangeHeaderStyle, IStateManager).TrackViewState()
                End If
            End If
            Return _RangeHeaderStyle
        End Get
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The style applied to title items.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("The style applied to title items. "), _
        Bindable(False), Category("Style"), _
        NotifyParentProperty(True), _
        PersistenceMode(PersistenceMode.InnerProperty)> _
    Public Overridable ReadOnly Property TitleStyle() As TableItemStyle
        Get
            If _TitleStyle Is Nothing Then
                _TitleStyle = New TableItemStyle
                If IsTrackingViewState Then
                    CType(_TitleStyle, IStateManager).TrackViewState()
                End If
            End If
            Return _TitleStyle
        End Get
    End Property

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The style applied to the background cells.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Description("The style applied to the background cells. "), _
        Bindable(False), Category("Style"), _
        NotifyParentProperty(True), _
        PersistenceMode(PersistenceMode.InnerProperty)> _
    Public Overridable ReadOnly Property BackgroundStyle() As TableItemStyle
        Get
            If _BackgroundStyle Is Nothing Then
                _BackgroundStyle = New TableItemStyle
                If IsTrackingViewState Then
                    CType(_BackgroundStyle, IStateManager).TrackViewState()
                End If
            End If
            Return _BackgroundStyle
        End Get
    End Property

#End Region

#Region "Templates"

    Private _itemTemplate As ITemplate = Nothing

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Main template providing content for each regular item in the body of the schedule.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <TemplateContainer(GetType(ScheduleItem)), Browsable(False), _
        Description("The content to be shown in each regular item."), _
        PersistenceMode(PersistenceMode.InnerProperty)> _
    Public Property ItemTemplate() As ITemplate
        Get
            Return _itemTemplate
        End Get
        Set(ByVal Value As ITemplate)
            _itemTemplate = Value
#If NET_2_0 Then
                RequiresDataBinding = True
#End If
        End Set
    End Property

    Private _emptyDataTemplate As ITemplate = Nothing

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Optional template providing content to be shown when the data source is empty.
    ''' This template is not used by the ScheduleCalendar control when FullTimeScale=True.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <TemplateContainer(GetType(ScheduleItem)), Browsable(False), _
        Description("The content to be shown when the data source is empty. This template is not used by the ScheduleCalendar control when FullTimeScale=True."), _
        PersistenceMode(PersistenceMode.InnerProperty)> _
    Public Property EmptyDataTemplate() As ITemplate
        Get
            Return _emptyDataTemplate
        End Get
        Set(ByVal Value As ITemplate)
            _emptyDataTemplate = Value
#If NET_2_0 Then
                RequiresDataBinding = True
#End If
        End Set
    End Property
#End Region

#Region "Events"

    Protected Overridable Sub OnItemCommand(ByVal e As ScheduleCommandEventArgs)
        RaiseEvent ItemCommand(Me, e)
    End Sub

    Protected Overridable Sub OnItemCreated(ByVal e As ScheduleItemEventArgs)
        RaiseEvent ItemCreated(Me, e)
    End Sub

    Protected Overridable Sub OnItemDataBound(ByVal e As ScheduleItemEventArgs)
        RaiseEvent ItemDataBound(Me, e)
    End Sub

#If NET_2_0 = 1 Then
        Protected Overridable Sub OnSelectedIndexChanged(ByVal e As EventArgs)
            RaiseEvent SelectedIndexChanged(Me, e)
        End Sub
#End If

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Raised when EnableEmptySlotClick is true and the user clicks on an empty slot.
    ''' </summary>
    ''' <param name="e">Event argument with information about the cell that was clicked</param>
    ''' -----------------------------------------------------------------------------
    Protected Overridable Sub OnEmptySlotClick(ByVal e As ClickableTableCellEventArgs)
        RaiseEvent EmptySlotClick(Me, e)
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Raised when a CommandEvent occurs within an item.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' -----------------------------------------------------------------------------
    <Category("Action"), Description("Raised when a CommandEvent occurs within an item.")> _
    Public Event ItemCommand As ScheduleCommandEventHandler

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Raised when an item is created and is ready for customization.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' -----------------------------------------------------------------------------
    <Category("Behavior"), Description("Raised when an item is created and is ready for customization.")> _
    Public Event ItemCreated As ScheduleItemEventHandler

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Raised when an item is data-bound.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' -----------------------------------------------------------------------------
    <Category("Behavior"), Description("Raised when an item is data-bound.")> _
    Public Event ItemDataBound As ScheduleItemEventHandler

#If NET_2_0 = 1 Then
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Occurs when the SelectedIndex property has changed.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' -----------------------------------------------------------------------------
        <Category("Action"), Description("Occurs when the SelectedIndex property has changed.")> _
        Public Event SelectedIndexChanged As EventHandler
#End If

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Occurs when the user clicks on an empty slot.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' -----------------------------------------------------------------------------
    <Category("Action"), Description("Occurs when the user clicks on an empty slot.")> _
    Public Event EmptySlotClick As ClickableTableCellEventHandler

#End Region

#Region "Methods and Implementation"

    Protected Sub New()  ' hide constructor in this abstract class
    End Sub

#If Not NET_2_0 = 1 Then

    Protected Overrides Sub CreateChildControls()
        CheckVersion()
        Controls.Clear()
        If ((Not ViewState("RowCount") Is Nothing) Or (Not ViewState("Empty") Is Nothing)) Then
            ' Create the control hierarchy using the view state, not the data source.
            CreateControlHierarchy(False)
        Else
            _itemsArrayList = New ArrayList
            ClearChildViewState()
        End If
    End Sub

#Else
        'CreateChildControls without parameters is implemented by CompositeDataBoundControl

        Protected Overrides Function CreateChildControls(ByVal data As IEnumerable, ByVal dataBinding As Boolean) As Integer
            CheckVersion()

            Controls.Clear()
            If (dataBinding) Then
                _itemsArrayList = New ArrayList
                ClearChildViewState()
                CreateControlHierarchy(data)
            Else
                ' Create the control hierarchy using the view state, not the data source.
                CreateControlHierarchy(Nothing)
                'RequiresDataBinding = False
            End If
            Return _itemsArrayList.Count

        End Function
#End If

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Checks if the application is running on the proper version of ASP.NET.
    ''' If not, an exception is thrown.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Sub CheckVersion()
#If NET_2_0 = 1 Then
            If Environment.Version.Major < 2 Then
                Throw New HttpException("The Schedule controls require ASP.NET 2.0 or higher")
            End If
#End If
    End Sub

#If Not NET_2_0 = 1 Then

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Overrides Control.Databind.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Overrides Sub DataBind()
        OnDataBinding(EventArgs.Empty) ' See BaseDatalist control in mono
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Overrides Control.OnDatabinding.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Protected Overrides Sub OnDataBinding(ByVal e As EventArgs)
        MyBase.OnDataBinding(e)
        If Not DataSource Is Nothing Then
            Controls.Clear()    ' clear any existing child controls

            ' clear any previous viewstate for existing child controls
            ClearChildViewState()
            CreateControlHierarchy(True)

            ' prevent child controls from being created again
            ChildControlsCreated = True
            TrackViewState()
        End If
    End Sub
#End If

#If NET_2_0 Then
        Sub CreateControlHierarchy(ByVal data As IEnumerable)
#Else
    Sub CreateControlHierarchy(ByVal useDataSource As Boolean)
#End If

        If Not (_itemsArrayList Is Nothing) Then
            _itemsArrayList.Clear()
            _items = Nothing
        Else
            _itemsArrayList = New ArrayList
        End If

#If NET_2_0 Then
            If Not IsNothing(data) Then
#Else
        If useDataSource Then
#End If
            Dim strCheckConfiguration As String = CheckConfiguration()
            ' check if all the necessary properties are set
            If (strCheckConfiguration <> "") Then
                Throw New HttpException(strCheckConfiguration)
            End If

#If NET_2_0 Then
                Dim dv As DataView = GetDataView(data)
#Else
            Dim dv As DataView = GetDataView()
#End If
            Controls.Clear()
            If ((dv Is Nothing OrElse dv.Count = 0) And ShowEmptyDataTemplate) Then
                RenderEmptyDataTemplate()
                Return
            End If
            ' clear any existing child controls
            Table1 = New Table
            Controls.Add(Table1)
            Table1.CopyBaseAttributes(Me)
            If ControlStyleCreated Then Table1.ApplyStyle(ControlStyle)

            PreprocessData(dv)

            FillRangeValueArray(dv)
            FillTitleValueArray(dv)

            CreateEmptyTable()

            AddRangeHeaderData()
            AddTitleHeaderData()

            AddData(dv)

            If (Not IncludeEndValue And ShowValueMarks) Then AddRangeValueMarks()

            AddDateHeaderData()

            ' Save information for use in round trips (enough to re-create the control tree).
            If (Not HttpContext.Current Is Nothing) Then
                SaveControlTree()
            End If
        Else ' Not useDataSource
            LoadControlTree()   ' Recreate the control tree from viewstate
        End If
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Check if all properties are set to make the control work
    ''' Override this function in derived versions.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------------
    MustOverride Function CheckConfiguration() As String

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Create the list with all range header values (Start or End)
    ''' </summary>
    ''' <param name="dv"></param>
    ''' -----------------------------------------------------------------------------
    MustOverride Sub FillRangeValueArray(ByRef dv As DataView)

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Remove any doubles from the ArrayList and 
    ''' </summary>
    ''' <param name="arr">The ArrayList object</param>
    ''' -----------------------------------------------------------------------------
    Shared Sub RemoveDoubles(ByRef arr As ArrayList)
        Dim count As Integer = arr.Count
        Dim i As Integer
        For i = count - 1 To 1 Step -1
            If (arr(i).ToString() = arr(i - 1).ToString()) Then
                arr.RemoveAt(i)
            End If
        Next i
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Pre-process the data in the dataview
    ''' Currently only used in ScheduleCalendar to split the items that span midnight
    ''' </summary>
    ''' <param name="dv">The DataView object containing the data</param>
    ''' -----------------------------------------------------------------------------
    Overridable Sub PreprocessData(ByRef dv As DataView)
        ' override to add usefull processing if any
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Create the list with all the title values
    ''' </summary>
    ''' <param name="dvView">The DataView object containing the data</param>
    ''' -----------------------------------------------------------------------------
    Overridable Sub FillTitleValueArray(ByRef dvView As DataView)
        ' Override in ScheduleGeneral to fill title array
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Get the number of times the control should repeat the schedule
    ''' (every time with new headers)
    ''' </summary>
    ''' <returns>An integer value indicating the repetition count</returns>
    ''' <remarks>
    ''' Overridden in ScheduleCalendar to return NumberOfRepetitions (usually number of weeks)
    ''' </remarks>
    ''' -----------------------------------------------------------------------------
    Overridable Function GetRepetitionCount() As Integer
        Return 1
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Create a Table control with the right number of rows and columns.
    ''' The actual content is added later.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Sub CreateEmptyTable()
        Dim col, row, iWeek As Integer ' counters

        Dim nTitles As Integer = GetTitleCount()

        If (Layout = LayoutEnum.Vertical) Then
            For iWeek = 1 To GetRepetitionCount()
                ' Add a row of title header cells
                Table1.Rows.Add(New TableRow)
                Dim tr0 As TableRow = Table1.Rows(Table1.Rows.Count - 1)
                For col = 1 To nTitles + 1
                    tr0.Cells.Add(New TableHeaderCell)
                    tr0.Cells(tr0.Cells.Count - 1).ApplyStyle(TitleStyle)
                    tr0.Cells(tr0.Cells.Count - 1).Text = "&nbsp;"
                Next col

                Dim nRows As Integer
                If (Not IncludeEndValue And ShowValueMarks) Then
                    ' Create 2 rows for each value allowing for a value mark to be added in between
                    nRows = arrRangeValues.Count * 2
                Else
                    nRows = arrRangeValues.Count
                End If

                For row = 1 To nRows
                    ' add a cell for the range header
                    Table1.Rows.Add(New TableRow)
                    Dim tr As TableRow = Table1.Rows(Table1.Rows.Count - 1)
                    ' add a cell for the header column
                    tr.Cells.Add(New TableHeaderCell)
                    Dim cell0 As TableCell = tr.Cells(tr.Cells.Count - 1)
                    cell0.ApplyStyle(RangeHeaderStyle)
                    cell0.Text = "&nbsp;"

                    ' If no value marks are needed add cells to all the rows
                    ' Else show only the even rows and the first row

                    If (IncludeEndValue Or Not ShowValueMarks Or row Mod 2 = 0 Or row = 1) Then

                        ' add a cell for each normal column
                        For col = 1 To nTitles
                            If EnableEmptySlotClick And _
                                    (Not (ShowValueMarks And Not IncludeEndValue And _
                                    (row = 1 Or row = nRows))) Then
                                tr.Cells.Add(New ClickableTableCell(Table1.Rows.Count - 1, col))
                            Else
                                tr.Cells.Add(New TableCell)
                            End If
                            Dim cell As TableCell = tr.Cells(tr.Cells.Count - 1)
                            cell.ApplyStyle(BackgroundStyle)

                            If (Not IncludeEndValue And ShowValueMarks) Then
                                If (row > 1 And row < nRows) Then
                                    ' the first and last row only have a span of 1
                                    cell.Text = "&nbsp;"
                                    cell.RowSpan = 2
                                End If
                            Else
                                cell.Text = "&nbsp;"
                            End If
                        Next col
                    End If
                Next row
            Next iWeek
        Else   ' Horizontal
            Dim nColumnsForRangeHeaders As Integer = arrRangeValues.Count
            If (Not IncludeEndValue And ShowValueMarks) Then
                ' Create 2 columns for each value allowing for a value mark to be added in between
                nColumnsForRangeHeaders = arrRangeValues.Count * 2
            End If

            ' In Horizontal layout, ignore repetition count: show 1 week only
            ' Add range header cell
            Table1.Rows.Add(New TableRow)
            Dim tr0 As TableRow = Table1.Rows(0)
            tr0.Cells.Add(New TableHeaderCell)
            tr0.Cells(0).ApplyStyle(TitleStyle)
            tr0.Cells(0).Text = "&nbsp;"
            For col = 1 To nColumnsForRangeHeaders
                tr0.Cells.Add(New TableHeaderCell)
                Dim cell As TableCell = tr0.Cells(col)
                cell.ApplyStyle(RangeHeaderStyle)
                cell.Text = "&nbsp;"
            Next col

            Dim nColumns As Integer = arrRangeValues.Count
            If (Not IncludeEndValue And ShowValueMarks) Then
                ' Extra column to allow the range headers to sit on the separation
                ' When for example there are 4 values, we make 5 columns
                ' 1 startcolumn, 3 columns sitting each between 2 values, and an end column
                nColumns += 1
            End If

            For row = 1 To nTitles
                Table1.Rows.Add(New TableRow)
                Dim tr As TableRow = Table1.Rows(row)
                ' add a cell for the title header
                tr.Cells.Add(New TableHeaderCell)
                tr.Cells(0).ApplyStyle(TitleStyle)
                tr.Cells(0).Text = "&nbsp;"

                ' add a cell for each column
                For col = 1 To nColumns
                    If EnableEmptySlotClick And _
                            (Not (Not IncludeEndValue And ShowValueMarks And _
                            (col = 1 Or col = nColumns))) Then
                        tr.Cells.Add(New ClickableTableCell(row, col))
                    Else
                        tr.Cells.Add(New TableCell)
                    End If
                    Dim cell As TableCell = tr.Cells(col)
                    cell.ApplyStyle(BackgroundStyle)
                    cell.Text = "&nbsp;"

                    If (Not IncludeEndValue And ShowValueMarks And col > 1 And col < nColumns) Then
                        ' the first and last column only have a span of 1
                        cell.ColumnSpan = 2
                    End If
                Next col
            Next row
        End If

    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Add date headers to the table when SeparateDateHeader=True
    ''' Overridden only in ScheduleGeneral to add date header data
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Overridable Sub AddDateHeaderData()
        ' Override in ScheduleGeneral to add date header data
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Iterate the arrRangeValues array, creating a new header item for each data item
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Sub AddRangeHeaderData()
        Dim cellsPerWeek As Integer = 1 + arrRangeValues.Count
        If (Not IncludeEndValue And ShowValueMarks) Then
            cellsPerWeek = 1 + arrRangeValues.Count * 2
        End If

        Dim j As Integer
        For j = 0 To GetRepetitionCount() - 1
            Dim i As Integer
            For i = 0 To arrRangeValues.Count - 1
                Dim obj As Object = arrRangeValues.Item(i)

                Dim rangeValueIndex As Integer = i + 1 + j * cellsPerWeek
                If (Not IncludeEndValue And ShowValueMarks) Then
                    rangeValueIndex = i * 2 + 1 + j * cellsPerWeek
                End If

                CreateItem(rangeValueIndex, 0, ScheduleItemType.RangeHeader, True, obj, -1)
            Next i
        Next j
    End Sub

    MustOverride Sub AddTitleHeaderData()

    MustOverride Function GetSortOrder() As String

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Add the actual items from the data source to the body of the table
    ''' </summary>
    ''' <param name="dvView">The data view that contains the data items</param>
    ''' -----------------------------------------------------------------------------
    Sub AddData(ByVal dvView As DataView)

        If (ItemTemplate Is Nothing) Then
            Throw New HttpException("The ItemTemplate is missing.")
        End If

        If (dvView Is Nothing) Then Return

        Dim strSortOrder As String = GetSortOrder()
        If (strSortOrder <> "") Then dvView.Sort = strSortOrder

        ' iterate DataSource creating a new item for each data item
        Dim prevTitleIndex As Integer = -1
        Dim prevStartCellIndex As Integer = -1

        Dim i As Integer
        ' make sure the data is processed in the right order: from bottom right up to top left.
        For i = dvView.Count - 1 To 0 Step -1

            Dim drv As DataRowView = dvView.Item(i)
            ' this dataRowView of data will be one entry in the schedule

            ' Let's find out where it should be displayed
            ' Instead of column or row numbers, we use titleIndex and valueIndex
            ' These can be used in the same way, whether layout is horizontal or vertical
            ' titleIndex is the row number in Horizontal mode and the column number in vertical mode
            ' valueIndex is the row number in Vertical mode and the column number in Horizontal mode
            ' Both start at 0
            Dim objTitleField As Object = drv(GetTitleField())
            Dim titleIndex As Integer = CalculateTitleIndex(objTitleField)
            If (titleIndex < 1) Then
                Exit For   ' since titleIndex is descending, and this one is too low already, skip all the rest too
            End If

            Dim objStartValue As Object = drv(DataRangeStartField)
            Dim objEndValue As Object = drv(DataRangeEndField)
            Dim startCellIndex As Integer = CalculateRangeCellIndex(objStartValue, objTitleField, False)
            Dim endCellIndex As Integer = CalculateRangeCellIndex(objEndValue, objTitleField, True)
            If (startCellIndex > -1 And endCellIndex > -1) Then  ' if not out of range
                If (Not IncludeEndValue) Then endCellIndex = endCellIndex - 1
                Dim Span As Integer = endCellIndex - startCellIndex + 1
                If Span = 0 Then Span = 1

                Dim maxStartCellIndex As Integer
                If (Layout = LayoutEnum.Vertical) Then
                    maxStartCellIndex = Table1.Rows.Count - 1
                Else ' Horizontal
                    If (titleIndex >= Table1.Rows.Count) Then
                        ' make sure nothing is added in this case
                        maxStartCellIndex = -2
                    Else
                        maxStartCellIndex = Table1.Rows(titleIndex).Cells.Count - 1
                    End If
                End If
                If (startCellIndex > 0 AndAlso startCellIndex <= maxStartCellIndex) Then

                    Dim cellIndex As Integer = startCellIndex
                    If (titleIndex = prevTitleIndex AndAlso startCellIndex + Span > prevStartCellIndex) Then
                        ' this cell is overlapping with the previous one (the one below or to the right)
                        ' prevStartValue is the starting cell index of the previous item
                        ' (in vertical layout the one below and in horizontal layout the one to the right)
                        ' this index is higher than this cell's index
                        ' (because the previous starting cell is under or to the right of this starting cell)
                        ' that's because we work from bottom right to top left
                        ' split the column or row that's overlapping so that we can show both contents
                        ' the last value to split is the end value of this item
                        SplitTitle(titleIndex, startCellIndex, endCellIndex)
                    End If
                    ' create new content
                    Dim type As ScheduleItemType = ScheduleItemType.Item
                    If (i Mod 2 = 1) Then type = ScheduleItemType.AlternatingItem
                    ' use the index in _itemsArrayList as dataSetIndex
                    If (Layout = LayoutEnum.Vertical OrElse _
                            cellIndex <= Table1.Rows(titleIndex).Cells.Count - 1) Then
                        Dim Item As ScheduleItem = CreateItem(cellIndex, titleIndex, type, True, drv, _itemsArrayList.Count)
                        MergeCells(cellIndex, titleIndex, Span)
                        _itemsArrayList.Add(Item)
                    End If
                    ' save location for next item to compare with
                    prevTitleIndex = titleIndex
                    prevStartCellIndex = startCellIndex
                End If
            End If
        Next i

    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Add range value marks to indicate the range values
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Protected Sub AddRangeValueMarks()
        If (Layout = LayoutEnum.Vertical) Then
            ' Insert an extra column for range marks after the first column

            Dim rowsPerWeek As Integer = 1 + arrRangeValues.Count * 2
            Dim j As Integer
            For j = 0 To GetRepetitionCount() - 1
                ' First insert a cell in the title row
                Dim rangeValueIndex As Integer = j * rowsPerWeek
                Table1.Rows(rangeValueIndex).Cells.AddAt(1, New TableHeaderCell)
                Dim tc1 As TableCell = Table1.Rows(rangeValueIndex).Cells(1)
                tc1.Text = "&nbsp;"
                tc1.ApplyStyle(TitleStyle)

                Dim i As Integer
                For i = 1 To rowsPerWeek - 1
                    rangeValueIndex = i + j * rowsPerWeek

                    Table1.Rows(rangeValueIndex).Cells.AddAt(1, New TableHeaderCell)
                    Dim tc As TableCell = Table1.Rows(rangeValueIndex).Cells(1)
                    tc.Text = "&nbsp;"
                    Dim tc0 As TableCell = Table1.Rows(rangeValueIndex).Cells(0)
                Next i
                For i = 1 To rowsPerWeek - 1 Step 2
                    ' each rangeheader spans 2 rows (over the rangeheader mark)
                    ' merge these cells over 2 rows in the first column
                    rangeValueIndex = i + j * rowsPerWeek
                    MergeCells(rangeValueIndex, 0, 2)
                Next i
            Next j
        Else  ' Horizontal
            Dim tr0 As TableRow = Table1.Rows(0)
            ' Add new row for rangevalue marks
            Table1.Rows.AddAt(1, New TableRow)
            Dim tr1 As TableRow = Table1.Rows(1)

            ' title column
            tr1.Cells.Add(New TableHeaderCell)
            Dim tc1 As TableCell = tr1.Cells(0)
            tc1.Text = "&nbsp;"
            tc1.ApplyStyle(TitleStyle)

            Dim nColumns As Integer = arrRangeValues.Count * 2 + 1
            Dim i As Integer
            For i = 1 To nColumns - 1
                Dim tc0 As TableCell = tr0.Cells(i)
                tr1.Cells.Add(New TableHeaderCell)
                Dim tc As TableCell = tr1.Cells(i)
                tc.Text = "&nbsp;"
            Next i
            ' iterate the cells in reverse order for merging
            For i = nColumns - 2 To 1 Step -2
                ' each rangeheader spans 2 rows (over the rangeheader mark)
                ' merge these cells over 2 columns in the first column
                MergeCells(i, 0, 2)
            Next i
        End If
    End Sub

    Protected Overridable Function GetTitleField() As String
        ' overridden in ScheduleCalendar
        Return TitleField
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
    ''' <param name="isEndValue">False if we're calculating the range value index for the 
    ''' start of the item, True if it's the end</param>
    ''' <returns>The range cell index</returns>
    ''' -----------------------------------------------------------------------------
    Protected MustOverride Function CalculateRangeCellIndex(ByVal objRangeValue As Object, ByVal objTitleValue As Object, _
            ByVal isEndValue As Boolean) As Integer

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Calculate the TitleIndex in the table, given the objTitleValue
    ''' </summary>
    ''' <param name="objTitleValue">The title value from the data source</param>
    ''' <returns>The title index</returns>
    ''' -----------------------------------------------------------------------------
    Protected MustOverride Function CalculateTitleIndex(ByVal objTitleValue As Object) As Integer

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Merge cells starting at startCellIndex
    ''' </summary>
    ''' <param name="startCellIndex">Index of the first cell to merge.</param>
    ''' <param name="titleIndex">Row or column containing the adjacent cells.</param>
    ''' <param name="Span">Number of columns or rows that the newly merged cell should span.</param>
    ''' <returns>The newly merged cell if successful. Nothing when merging fails.</returns>
    ''' <remarks>
    ''' In horizontal layout, merging may start at 0
    ''' </remarks>
    ''' -----------------------------------------------------------------------------
    Function MergeCells(ByVal startCellIndex As Integer, ByVal titleIndex As Integer, ByVal span As Integer) As TableCell
        If (titleIndex > GetTitleCount()) Then Return Nothing

        Dim minValueIndex As Integer = CInt(IIf(Layout = LayoutEnum.Horizontal, 0, 1))
        If (startCellIndex < minValueIndex) Then Return Nothing
        If (span < 2) Then Return Nothing

        Dim maxValueIndex As Integer
        If (Layout = LayoutEnum.Horizontal) Then
            maxValueIndex = Table1.Rows(0).Cells.Count - 1
        Else  ' Vertical
            maxValueIndex = Table1.Rows.Count - 1
        End If

        If (startCellIndex > maxValueIndex) Then Return Nothing

        If (startCellIndex + span - 1 > maxValueIndex) Then
            span = maxValueIndex - startCellIndex + 1
        End If

        Try
            If (Not IncludeEndValue And ShowValueMarks And _
                    Layout = LayoutEnum.Horizontal And titleIndex > 0) Then
                ' in this case every item spans 2 columns
                span = span * 2
            End If

            If (span <= GetValueSpan(startCellIndex, titleIndex)) Then Return Nothing
            RemoveCells(startCellIndex, startCellIndex + span, titleIndex)
            ' change span property to extend the cell
            SetValueSpan(startCellIndex, titleIndex, span)
            Return GetCell(startCellIndex, titleIndex)
        Catch ex As Exception
            If (Not HttpContext.Current Is Nothing) Then Context.Trace.Warn(ex.Message)
        End Try
        Return Nothing
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Removes all the cells from startValueIndex to endValueIndex, except the first cell
    ''' </summary>
    ''' <param name="startCellIndex">Index of the first cell to remove.</param>
    ''' <param name="endCellIndex">Index of the last cell to remove.</param>
    ''' <param name="titleIndex">Row or column containing the adjacent cells.</param>
    ''' -----------------------------------------------------------------------------
    Private Sub RemoveCells(ByVal startCellIndex As Integer, ByVal endCellIndex As Integer, ByVal titleIndex As Integer)
        ' When cells are merged in HTML, it is done by increasing the span of the first cell,
        ' and removing the other cells.
        Dim prevSpan As Integer = GetValueSpan(startCellIndex, titleIndex)

        Dim valueCellIndex As Integer = startCellIndex + prevSpan
        While valueCellIndex < endCellIndex
            Dim nextCellIndex As Integer
            If Layout = LayoutEnum.Vertical Then
                nextCellIndex = valueCellIndex
            Else
                nextCellIndex = startCellIndex + 1 ' use +1, not +prevSpan
            End If
            prevSpan = GetValueSpan(nextCellIndex, titleIndex)
            RemoveCell(nextCellIndex, titleIndex)
            valueCellIndex += prevSpan
        End While
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Removes the cell at cellValueIndex
    ''' </summary>
    ''' <param name="cellValueIndex">Cell value index of the cell to be removed.</param>
    ''' <param name="titleIndex">Title index of the cell to be removed.</param>
    ''' -----------------------------------------------------------------------------
    Private Sub RemoveCell(ByVal cellValueIndex As Integer, ByVal titleIndex As Integer)
        If Layout = LayoutEnum.Vertical Then
            Table1.Rows(cellValueIndex).Cells.RemoveAt(titleIndex)
        Else  ' Horizontal
            Table1.Rows(titleIndex).Cells.RemoveAt(cellValueIndex)
        End If
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' in vertical layout, get the column span. In horizontal layout, get the row span
    ''' </summary>
    ''' <param name="valueIndex">Value index of the cell</param>
    ''' <param name="titleIndex">Title index of the cell</param>
    ''' <returns>Integer value indicating the span width</returns>
    ''' -----------------------------------------------------------------------------
    Private Function GetSplitSpan(ByVal valueIndex As Integer, ByVal titleIndex As Integer) As Integer
        If (Layout = LayoutEnum.Vertical) Then
            GetSplitSpan = Table1.Rows(valueIndex).Cells(titleIndex).ColumnSpan
        Else
            GetSplitSpan = Table1.Rows(titleIndex).Cells(valueIndex).RowSpan
        End If
        If GetSplitSpan = 0 Then GetSplitSpan = 1
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Check if the cells at titleIndex are already spanning over multiple rows or columns
    ''' for the given range. The span must be the same over that range.
    ''' </summary>
    ''' <param name="titleIndex">Title index of the range</param>
    ''' <param name="firstCellIndex"></param>
    ''' <param name="lastCellIndex"></param>
    ''' <returns>True or false</returns>
    ''' -----------------------------------------------------------------------------
    Private Function IsRangeAlreadySpanning(ByVal titleIndex As Integer, ByVal firstCellIndex As Integer, _
            ByVal lastCellIndex As Integer) As Boolean
        Dim SplitSpan As Integer = GetSplitSpan(firstCellIndex, titleIndex)
        If (SplitSpan = 1) Then Return False
        If (Layout = LayoutEnum.Vertical) Then ' v1.6.0.4 bugfix
            Dim row As Integer = firstCellIndex + GetValueSpan(firstCellIndex, titleIndex)
            While row <= lastCellIndex
                If (GetSplitSpan(row, titleIndex) <> SplitSpan) Then Return False
                row = row + GetValueSpan(row, titleIndex)
            End While
            ' the last cell of the range must also end at startOfRange
            ' if not, we can't split the range without adding a new column or row
            If (row > lastCellIndex + 1) Then Return False
        Else ' Horizontal
            Dim cellIndex As Integer = firstCellIndex + 1 ' v1.6.0.3 bugfix
            For cellIndex = firstCellIndex + 1 To lastCellIndex
                If (GetSplitSpan(cellIndex, titleIndex) <> SplitSpan) Then Return False
            Next
        End If
        Return True
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Split a column or row in 2 in order to allow the display of overlapping items.
    ''' </summary>
    ''' <param name="titleIndex">Title index of the cells to be split</param>
    ''' <param name="firstCellIndexToSplit">Index of the first cell in the range to split</param>
    ''' <param name="lastCellIndexToSplit">Index of the last cell in the range to split</param>
    ''' -----------------------------------------------------------------------------
    Private Sub SplitTitle(ByVal titleIndex As Integer, ByVal firstCellIndexToSplit As Integer, _
            ByVal lastCellIndexToSplit As Integer)
        ' To show overlapping items, we simply split the entire column or row in 2,
        ' except when it was already split before.
        ' Only the overlapping items are actually shown in 2 columns or rows, the other items
        ' are shown sitting on both columns or rows.
        ' This works well only when there are not yet any split columns or rows to the left of or above this one
        ' If we work from right to left and bottom to top, this should be OK
        '
        ' Because of the way tables work in HTML, the approach is different for each layout.
        ' In vertical layout, we'll split the column by adding cells in several rows.
        ' In horizontal layout, we'll add a new row, and add several cells to it.
        Dim cellsPerWeek As Integer = 1 + arrRangeValues.Count  ' including one cell for title headers

        If (Not IncludeEndValue And ShowValueMarks) Then
            If (Layout = LayoutEnum.Horizontal) Then
                ' Extra column to allow the range headers to sit on the separation
                ' When for example there are 4 values, we make 6 columns
                ' 1 title column, 1 start column, 3 columns (each sitting between 2 values), 
                ' and an end column
                cellsPerWeek = 2 + arrRangeValues.Count
            Else
                ' Each item takes 2 rows 
                cellsPerWeek = 1 + arrRangeValues.Count * 2
            End If
        End If

        Dim firstCellIndexInThisWeek As Integer = (firstCellIndexToSplit \ cellsPerWeek) * cellsPerWeek
        Dim lastCellIndexInThisWeek As Integer = firstCellIndexInThisWeek + cellsPerWeek - 1

        Dim SplitEntireColumnOrRow As Boolean = Not IsRangeAlreadySpanning(titleIndex, firstCellIndexToSplit, lastCellIndexToSplit)

        Dim newRow As TableRow = Nothing ' only used in horizontal layout if SplitEntireColumnOrRow=True
        If (SplitEntireColumnOrRow And Layout = LayoutEnum.Horizontal) Then
            ' insert a new row
            Table1.Rows.AddAt(titleIndex, New TableRow)
            newRow = Table1.Rows(titleIndex)
            titleIndex = titleIndex + 1
        End If

        If Layout = LayoutEnum.Vertical Then
            ' in vertical layout, realCellIndex is the same as valueIndex
            Dim row As Integer = firstCellIndexInThisWeek
            While row <= lastCellIndexInThisWeek
                ' first, get the span of the original cell
                Dim originalCellSpan As Integer = GetValueSpan(row, titleIndex)

                If (row >= firstCellIndexToSplit AndAlso row <= lastCellIndexToSplit) Then
                    ' split the cell in this row by inserting a new cell
                    Dim ctc0 As New ClickableTableCell(row, titleIndex)
                    Table1.Rows(row).Cells.AddAt(titleIndex, ctc0)
                    Dim tc As TableCell = Table1.Rows(row).Cells(titleIndex)
                    tc.RowSpan = originalCellSpan
                    If (Not SplitEntireColumnOrRow) Then
                        ' no extra column was added, only an extra cell
                        ' therefore: reduce the columnspan of the original cell
                        Dim tcOriginal As TableCell = Table1.Rows(row).Cells(titleIndex + 1)
                        tc.ColumnSpan = tcOriginal.ColumnSpan - 1
                        tcOriginal.ColumnSpan = 1
                    Else ' SplitEntireColumnOrRow
                        If (row + originalCellSpan - 1 > lastCellIndexToSplit) Then
                            ' previous item went below this one, so split the remainder too
                            ' this is done by inserting another empty cell
                            tc.RowSpan = lastCellIndexToSplit - row + 1
                            ' split the remainder as well
                            Dim ctc As New ClickableTableCell(lastCellIndexToSplit + 1, titleIndex)
                            Table1.Rows(lastCellIndexToSplit + 1).Cells.AddAt(titleIndex, ctc)
                            Dim tc2 As TableCell = Table1.Rows(lastCellIndexToSplit + 1).Cells(titleIndex)
                            tc2.ApplyStyle(BackgroundStyle)
                            tc2.Text = "&nbsp;"
                            tc2.RowSpan = row + originalCellSpan - 1 - lastCellIndexToSplit
                        End If
                    End If
                Else ' this cell should not be split
                    If (SplitEntireColumnOrRow) Then
                        ' we have added an extra column, but this cell should not be split,
                        ' it should only be spread over the extra column 
                        Dim tc As TableCell = Table1.Rows(row).Cells(titleIndex)
                        Dim ColumnSpan As Integer = tc.ColumnSpan
                        If ColumnSpan = 0 Then ColumnSpan = 1
                        tc.ColumnSpan = ColumnSpan + 1
                    End If
                End If
                row = row + originalCellSpan   ' skip as many rows as this span
            End While
        Else ' Horizontal
            ' in horizontal layout, valueIndex is still the index referring to the list of values
            '   realCellIndex indicates the index (offset) of the cell in the TableRow
            '   this is not the same, when a cell has a horizontal span of 2, the next cell will have
            '   a realCellIndex that is 1 higher, but a valueIndex that is 2 higher.
            Dim valueIndex As Integer = firstCellIndexInThisWeek
            Dim realCellIndex As Integer = valueIndex
            While valueIndex <= lastCellIndexInThisWeek
                ' first, get the span of the original cell
                Dim originalCellSpan As Integer = GetValueSpan(realCellIndex, titleIndex)
                Dim originalValueSpan As Integer = originalCellSpan
                If (Not IncludeEndValue And ShowValueMarks) Then
                    ' in this case a normal item already has a span of 2, but it's only 1 cell
                    originalValueSpan = originalCellSpan \ 2
                    If (originalValueSpan = 0) Then originalValueSpan = 1
                End If

                If (valueIndex >= firstCellIndexToSplit AndAlso valueIndex <= lastCellIndexToSplit) Then
                    If (Not SplitEntireColumnOrRow) Then
                        ' the current cell is already spanning several rows
                        ' there is no need to add another row, just split the current cell
                        ' over the rows that it's already occupying.
                        Dim thisRow As TableRow = Table1.Rows(titleIndex)
                        Dim SplitSpan As Integer = thisRow.Cells(valueIndex).RowSpan
                        Dim lastRowInSpan As TableRow = Table1.Rows(titleIndex + SplitSpan - 1)
                        ' move the current cell down to the last row of the rows that are being spanned
                        ' it should become the first cell in that row
                        lastRowInSpan.Cells.AddAt(0, thisRow.Cells(realCellIndex))
                        lastRowInSpan.Cells(0).RowSpan = 1
                        ' add a new cell in this row
                        Dim ctc As New ClickableTableCell(titleIndex, valueIndex)
                        thisRow.Cells.AddAt(realCellIndex, ctc)
                        thisRow.Cells(realCellIndex).ColumnSpan = originalCellSpan
                        thisRow.Cells(realCellIndex).RowSpan = SplitSpan - 1
                    Else  ' SplitEntireColumnOrRow
                        ' split the cell in this column by inserting a new cell
                        Dim ctc0 As New ClickableTableCell(titleIndex - 1, valueIndex)
                        newRow.Cells.Add(ctc0)
                        Dim tc As TableCell = newRow.Cells(newRow.Cells.Count - 1)
                        tc.ColumnSpan = originalCellSpan
                        If (valueIndex + originalValueSpan - 1 > lastCellIndexToSplit) Then
                            ' previous item ended further to the right than this one, so split the remainder too
                            ' this is done by inserting another empty cell
                            Dim newValueSpan As Integer = lastCellIndexToSplit - valueIndex + 1
                            If (Not IncludeEndValue And ShowValueMarks) Then
                                ' in this case a normal item has a span of 2
                                newValueSpan *= 2
                            End If

                            tc.ColumnSpan = newValueSpan
                            ' split the remainder as well
                            Dim ctc As New ClickableTableCell(titleIndex - 1, valueIndex + originalValueSpan - 1)
                            newRow.Cells.Add(ctc)
                            Dim tc2 As TableCell = newRow.Cells(newRow.Cells.Count - 1)
                            Dim newValueSpan2 As Integer = valueIndex + originalValueSpan - 1 - lastCellIndexToSplit
                            If (Not IncludeEndValue And ShowValueMarks) Then
                                ' in this case a normal item has a span of 2
                                newValueSpan2 *= 2
                            End If
                            tc2.ColumnSpan = newValueSpan2
                            tc2.ApplyStyle(BackgroundStyle)
                            tc2.Text = "&nbsp;"
                        End If
                    End If
                Else ' this cell should not be split
                    If (SplitEntireColumnOrRow) Then
                        ' we have added an extra row, but this cell should not be split,
                        ' it should only be spread over the extra row
                        ' move this cell up one row to the new row, and spread it over more rows
                        newRow.Cells.Add(Table1.Rows(titleIndex).Cells(realCellIndex))
                        realCellIndex = realCellIndex - 1  ' decrease cellindex, because one cell has been removed
                        ' the cell was moved, and it is currently the last cell of newRow
                        Dim tc2 As TableCell = newRow.Cells(newRow.Cells.Count - 1)
                        Dim RowSpan As Integer = tc2.RowSpan ' get its span
                        If RowSpan = 0 Then RowSpan = 1
                        tc2.RowSpan = RowSpan + 1 ' increase it
                    End If
                End If
                valueIndex = valueIndex + originalValueSpan   ' for the index, skip as many rows/columns as this span
                realCellIndex = realCellIndex + 1    ' if horizontal, just take the next cell
            End While
        End If

    End Sub

#If Not NET_2_0 = 1 Then

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Get the DataView from the DataSource and DataMember properties
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------------
    Function GetDataView() As DataView
        GetDataView = Nothing

        If (Not Me.DataSource Is Nothing) Then
            ' determine if the datasource is a DataSet, DataTable or DataView
            If (TypeOf Me.DataSource Is DataView) Then
                GetDataView = CType(Me.DataSource, DataView)
            ElseIf (TypeOf Me.DataSource Is DataTable) Then
                GetDataView = New DataView(CType(Me.DataSource, DataTable))
            ElseIf (TypeOf Me.DataSource Is DataSet) Then
                Dim ds As DataSet = CType(Me.DataSource, DataSet)
                If (Me.DataMember Is Nothing OrElse Me.DataMember = "") Then
                    ' if data member isn't supplied, default to the first table
                    GetDataView = New DataView(ds.Tables(0))
                Else
                    GetDataView = New DataView(ds.Tables(Me.DataMember))   ' if data member is supplied, use it
                End If
            End If
        End If
        ' throw an exception if there is a problem with the data source
        If (GetDataView Is Nothing) Then
            Throw New HttpException("Error finding the DataSource. " & _
                "Please check the DataSource and DataMember properties.")
        End If
    End Function

#Else ' NET_2_0=1

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Get the DataView from the DataSource and DataMember properties
        ''' </summary>
        ''' <returns>The DataView object</returns>
        ''' -----------------------------------------------------------------------------
        Function GetDataView(ByVal data As IEnumerable) As DataView
            GetDataView = Nothing

            Dim obj As Object = Nothing
            ' Use a trick to get the dataview quickly: get it from the first item
            For Each obj In data
                Dim drv As DataRowView = CType(obj, DataRowView)
                If (Not drv Is Nothing) Then
                    GetDataView = drv.DataView
                    Exit For
                End If
            Next
        End Function

#End If

    ' Override to get the corresponding templates
    Protected MustOverride Function GetTemplate(ByVal type As ScheduleItemType) As ITemplate

    Protected Overridable Function GetStyle(ByVal type As ScheduleItemType) As TableItemStyle
        ' Override to add additional styles (such as the style for DateHeaders in ScheduleGeneral)
        GetStyle = Nothing
        Select Case type
            Case ScheduleItemType.RangeHeader
                Return RangeHeaderStyle
            Case ScheduleItemType.TitleHeader
                Return TitleStyle
            Case ScheduleItemType.Item
                Return ItemStyle
            Case ScheduleItemType.AlternatingItem
                GetStyle = New TableItemStyle
                If (Not _itemStyle Is Nothing) Then
                    GetStyle.MergeWith(ItemStyle)
                End If
                If (Not _alternatingItemStyle Is Nothing) Then
                    GetStyle.CopyFrom(AlternatingItemStyle)
                End If
        End Select
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Instantiate item using the corresponding template (if any)
    ''' </summary>
    ''' <param name="item">ScheduleItem to instantiate</param>
    ''' <param name="cell">corresponding table cell</param>
    ''' -----------------------------------------------------------------------------
    Private Sub InstantiateItem(ByVal item As ScheduleItem, ByVal cell As TableCell)
        Dim template As ITemplate = GetTemplate(item.ItemType)
        If (Not template Is Nothing) Then
            template.InstantiateIn(item)   ' initialize item from template
        Else
            If (item.ItemType = ScheduleItemType.Item Or item.ItemType = ScheduleItemType.AlternatingItem) Then
                ' this exception should never fire: another should already have fired in AddData()
                Throw New HttpException("The ItemTemplate is missing")
            End If
            ' no template provided, just show data item
            Dim value As Object = item.DataItem  ' for header items, DataItem is just a String or a Date
            Dim lit As New Literal
            If (Not value Is Nothing) Then
                lit.Text = FormatDataValue(value, item.ItemType)
            Else
                ' On postback, viewstate should keep the contents, but it fails to do so ***
                ' The headers stay blank on postback if no template is provided.
                ' For now, to make it work on postback too, make sure you provide all the templates
                lit.Text = "&nbsp;"
            End If
            item.Controls.Add(lit)
        End If
        Dim myStyle As TableItemStyle = GetStyle(item.ItemType)
        If (Not myStyle Is Nothing) Then cell.ApplyStyle(myStyle)

    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' when there's no template, try to present the data in a reasonable format
    ''' </summary>
    ''' <param name="value">Value of the item</param>
    ''' <param name="type">Type of the item</param>
    ''' <returns>A formatted string</returns>
    ''' -----------------------------------------------------------------------------
    Protected MustOverride Function FormatDataValue(ByVal value As Object, ByVal type As ScheduleItemType) As String

    ''' <summary>
    ''' Create the item
    ''' </summary>
    ''' <param name="cellValueIndex">Cell value index of the item</param>
    ''' <param name="titleIndex">Title index of the item</param>
    ''' <param name="itemType">Type of the item</param>
    ''' <param name="dataBind">Whether databinding should be performed</param>
    ''' <param name="dataItem">Data item of the item</param>
    ''' <param name="dataSetIndex">Index of the item in the dataset</param>
    ''' <returns>The newly created ScheduleItem</returns>
    ''' -----------------------------------------------------------------------------
    Protected Function CreateItem(ByVal cellValueIndex As Integer, ByVal titleIndex As Integer, ByVal itemType As ScheduleItemType, _
        ByVal dataBind As Boolean, ByVal dataItem As Object, ByVal dataSetIndex As Integer) As ScheduleItem

        Dim item As New ScheduleItem(dataSetIndex, itemType)
        If (dataBind) Then
            item.DataItem = dataItem
#If NET_2_0 Then
                If (itemType = ScheduleItemType.Item Or itemType = ScheduleItemType.AlternatingItem) Then
                    If (DataKeyNames(0) [IsNot] Nothing) Then
                        Dim dk As New DataKey(CreateItemDataKey(item), DataKeyNames)
                        item.DataKey = dk
                    End If
                    ' Don't allow the framework to attribute an automatic ID
                    ' It may not be the same after postback
                    ' which will prevent postback events from firing
                    item.ID = "item" & dataSetIndex
                End If
#End If
        End If
        Dim cell As TableCell = GetCell(cellValueIndex, titleIndex)
        InstantiateItem(item, cell)

        If (item.ItemType = ScheduleItemType.Item Or item.ItemType = ScheduleItemType.AlternatingItem) Then
            If (ItemStyleField <> "" And Not IsNothing(dataItem)) Then
                Dim drv As DataRowView = CType(dataItem, DataRowView)
                Dim objItemStyle As Object = drv.Item(ItemStyleField)
                If (Not IsDBNull(objItemStyle)) Then
                    Dim style As New TableItemStyle
                    style.CssClass = CStr(objItemStyle)
                    cell.ApplyStyle(style)
                End If
            End If
        End If

        Dim e As New ScheduleItemEventArgs(item)
        OnItemCreated(e)
        GetCell(cellValueIndex, titleIndex).Controls.Add(item)

        If (dataBind) Then
            item.DataBind()
            OnItemDataBound(e)
            'item.DataItem = Nothing
        End If
        Return item
    End Function

    Protected MustOverride Function GetTitleCount() As Integer

    Overridable Function GetRangeHeaderIndex() As Integer
        ' overridden in ScheduleGeneral when using Date Headers
        Return 0
    End Function

    Private Function GetMinTitleIndex() As Integer
        If (Not IncludeEndValue And ShowValueMarks) Then
            Return GetRangeHeaderIndex() + 2
        Else
            Return GetRangeHeaderIndex() + 1
        End If
    End Function

    Private Function GetMaxTitleIndex() As Integer
        Return GetMinTitleIndex() + GetTitleCount() - 1
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' In vertical layout, get the row span. In horizontal layout, get the column span
    ''' </summary>
    ''' <param name="cellValueIndex">Cell value index of the item</param>
    ''' <param name="titleIndex">Title index of the item</param>
    ''' <returns>The integer value of the span</returns>
    ''' -----------------------------------------------------------------------------
    Protected Function GetValueSpan(ByVal cellValueIndex As Integer, ByVal titleIndex As Integer) As Integer
        If (Layout = LayoutEnum.Vertical) Then
            GetValueSpan = Table1.Rows(cellValueIndex).Cells(titleIndex).RowSpan
        Else
            GetValueSpan = Table1.Rows(titleIndex).Cells(cellValueIndex).ColumnSpan
        End If
        If GetValueSpan = 0 Then GetValueSpan = 1
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' in vertical layout, set the row span. In horizontal layout, set the column span
    ''' </summary>
    ''' <param name="cellValueIndex">Cell value index of the item</param>
    ''' <param name="titleIndex">Title index of the item</param>
    ''' <param name="span">The new span value</param>
    ''' -----------------------------------------------------------------------------
    Private Sub SetValueSpan(ByVal cellValueIndex As Integer, ByVal titleIndex As Integer, ByVal span As Integer)
        If (Layout = LayoutEnum.Vertical) Then
            Table1.Rows(cellValueIndex).Cells(titleIndex).RowSpan = span
        Else
            Table1.Rows(titleIndex).Cells(cellValueIndex).ColumnSpan = span
        End If
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Get the cell at the given indexes.
    ''' </summary>
    ''' <param name="cellValueIndex">Cell value index of the item</param>
    ''' <param name="titleIndex">Title index of the item</param>
    ''' <returns>The TableCell object of the corresponding cell</returns>
    ''' -----------------------------------------------------------------------------
    Private Function GetCell(ByVal cellValueIndex As Integer, ByVal titleIndex As Integer) As TableCell
        If (Layout = LayoutEnum.Vertical) Then
            GetCell = Table1.Rows(cellValueIndex).Cells(titleIndex)
        Else
            GetCell = Table1.Rows(titleIndex).Cells(cellValueIndex)
        End If
    End Function

#If NET_2_0 = 1 Then
        Private Function CreateItemDataKey(ByVal item As ScheduleItem) As IOrderedDictionary
            If Me.DesignMode Then Return Nothing
            If DataKeyNames.Length = 0 OrElse DataKeyNames(0) Is Nothing Then Return Nothing
            If item.DataItem Is Nothing Then Return Nothing
            Dim props As PropertyDescriptorCollection = TypeDescriptor.GetProperties(item.DataItem)
            Dim cachedKeyProperties(DataKeyNames.Length - 1) As PropertyDescriptor
            For n As Integer = 0 To DataKeyNames.Length - 1
                Dim p As PropertyDescriptor = props(DataKeyNames(n))
                If (p Is Nothing) Then
                    Throw New InvalidOperationException("Property '" & DataKeyNames(n) & _
                        "' not found in object of type " & item.DataItem.GetType().ToString())
                End If
                cachedKeyProperties(n) = p
            Next

            Dim dic As New OrderedDictionary()
            For Each p As PropertyDescriptor In cachedKeyProperties
                dic(p.Name) = p.GetValue(item.DataItem)
            Next p
            Return dic
        End Function
#End If

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Show the EmptyDataTemplate when the data source is empty
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Private Sub RenderEmptyDataTemplate()
        If (Not EmptyDataTemplate Is Nothing) Then
            Dim plh As New PlaceHolder
            EmptyDataTemplate.InstantiateIn(plh)   ' initialize from template
            Controls.Add(plh)
            ViewState("Empty") = True ' raize a flag
        End If
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Handle events raised by children by overriding OnBubbleEvent.
    ''' </summary>
    ''' <param name="source">the source of the event</param>
    ''' <param name="e">event data</param>
    ''' <returns>true if the event has been handled</returns>
    ''' -----------------------------------------------------------------------------
    Protected Overrides Function OnBubbleEvent(ByVal source As Object, ByVal e As EventArgs) As Boolean
        If TypeOf e Is ScheduleCommandEventArgs Then
            Dim ce As ScheduleCommandEventArgs = CType(e, ScheduleCommandEventArgs)
            OnItemCommand(ce)
            Dim cmdName As String = ce.CommandName.ToLower()
#If NET_2_0 = 1 Then
                If (cmdName = "select") Then
                    Try
                        SelectedValue = ce.Item.DataKey.Value
                    Catch ex As Exception
                        Throw New InvalidOperationException("Invalid DataKey. Check the DataKeyNames property")
                    End Try
                    OnSelectedIndexChanged(EventArgs.Empty)
                End If
#End If
        End If
        Return True
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' RaisePostBackEvent is called when the user clicks on an empty slot
    ''' (see clickabletablecell.vb)
    ''' </summary>
    ''' <param name="eventArgument">String with the event argument.
    ''' Contains something like "3-7", where 3 is the row and 7 the column </param>
    ''' -----------------------------------------------------------------------------
    Public Sub RaisePostBackEvent(ByVal eventArgument As String) _
               Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
        ' get column and row number from eventArgument
        Dim args() As String = eventArgument.Split("-"c)
        Dim row As Integer = CInt(args(0))
        Dim column As Integer = CInt(args(1))
        Dim Title, RangeStartValue, RangeEndValue As Object
        Dim RangeStartValueIndex As Integer = CalculateRangeValueIndex(row, column)
        If (Layout = LayoutEnum.Horizontal) Then
            Title = CalculateTitle(row, column)
        Else
            Title = CalculateTitle(column, row)
        End If
        RangeStartValue = arrRangeValues(RangeStartValueIndex) 'CalculateRangeValue(row - 1)
        Dim RangeEndValueIndex As Integer = RangeStartValueIndex + 1
        If (RangeEndValueIndex >= arrRangeValues.Count) Then RangeEndValueIndex = arrRangeValues.Count - 1
        RangeEndValue = arrRangeValues(RangeEndValueIndex) 'CalculateRangeValue(endRow)
        ' remove any selected value
#If NET_2_0 = 1 Then
            Me.SelectedValue = Nothing
#End If
        Dim ctcea As New ClickableTableCellEventArgs(Title, RangeStartValue, RangeEndValue)
        OnEmptySlotClick(ctcea)
    End Sub

    '''' -----------------------------------------------------------------------------
    '''' <summary>
    '''' Calculate the title value given the cell index
    '''' </summary>
    '''' <param name="titleIndex">Title index of the cell</param>
    '''' <returns>Object containing the title</returns>
    '''' -----------------------------------------------------------------------------
    Public MustOverride Function CalculateTitle(ByVal titleIndex As Integer, ByVal cellIndex As Integer) As Object

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Calculate the range value index given the cell index
    ''' </summary>
    ''' <param name="row">Row of the cell</param>
    ''' <param name="column">Column of the cell</param>
    ''' <returns>Integer containing the range value index</returns>
    ''' -----------------------------------------------------------------------------
    Public Function CalculateRangeValueIndex(ByVal row As Integer, ByVal column As Integer) As Integer
        If (Layout = LayoutEnum.Horizontal) Then
            Return column - 1 - CInt(IIf(Not IncludeEndValue And ShowValueMarks, 1, 0))
        Else
            Dim rowsPerWeek As Integer = CalculateRowsPerRepetition()
            Dim RangeValueIndex As Integer
            If (Not IncludeEndValue And ShowValueMarks) Then
                RangeValueIndex = (row Mod rowsPerWeek) \ 2 - 1
            Else
                RangeValueIndex = (row Mod rowsPerWeek) - 1
            End If
            If (RangeValueIndex < 0) Then RangeValueIndex = 0
            Return RangeValueIndex
        End If
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Calculate the number of rows per repetition
    ''' </summary>
    ''' <returns>Integer containing the number of rows</returns>
    ''' -----------------------------------------------------------------------------
    Public Function CalculateRowsPerRepetition() As Integer
        If (Not IncludeEndValue And ShowValueMarks) Then
            CalculateRowsPerRepetition = arrRangeValues.Count * 2 + 1
        Else
            CalculateRowsPerRepetition = arrRangeValues.Count + 1
        End If
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Save information for use in round trips (enough to re-create the control tree).
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Private Sub SaveControlTree()
        If (Items.Count = 0) Then Return
        Dim arrCellCount(Table1.Rows.Count) As Integer   ' number of cells in each row
        Dim arrHeaderCount(Table1.Rows.Count) As Integer ' number of header cells in each row
        Dim arrItemCols(Items.Count) As Integer
        Dim arrItemRows(Items.Count) As Integer
#If NET_2_0 = 1 Then
            Dim arrDataKeys(Items.Count) As IOrderedDictionary
#End If
        Dim nCells As Integer = Table1.Rows.Count * Table1.Rows(0).Cells.Count
        If (Table1.Rows.Count > 0 AndAlso Table1.Rows(1).Cells.Count > Table1.Rows(0).Cells.Count) Then
            nCells = Table1.Rows.Count * Table1.Rows(1).Cells.Count
        End If
        Dim arrClickRows(nCells) As Integer ' row of each clickable cell
        Dim arrClickColumns(nCells) As Integer ' column of each clickable cell

        Dim row, col As Integer
        Dim ClickableCellCount As Integer = 0
        For row = 0 To Table1.Rows.Count - 1
            Dim cellsInThisRow As Integer = Table1.Rows(row).Cells.Count
            arrCellCount(row) = cellsInThisRow
            For col = 0 To cellsInThisRow - 1
                If (TypeOf Table1.Rows(row).Cells(col) Is TableHeaderCell) Then
                    arrHeaderCount(row) = col + 1 ' will continue to increase until normal cells start
                End If
                If (EnableEmptySlotClick AndAlso _
                        TypeOf Table1.Rows(row).Cells(col) Is ClickableTableCell) Then
                    Dim ccell As ClickableTableCell = CType(Table1.Rows(row).Cells(col), ClickableTableCell)
                    arrClickColumns(ClickableCellCount) = ccell.Column
                    arrClickRows(ClickableCellCount) = ccell.Row
                    ClickableCellCount += 1
                End If
                Dim cell As TableCell = Table1.Rows(row).Cells(col)
                If (cell.HasControls()) Then
                    If (TypeOf cell.Controls(0) Is ScheduleItem) Then
                        Dim item As ScheduleItem = CType(cell.Controls(0), ScheduleItem)
                        Dim dataSetIndex As Integer = item.DataSetIndex
                        If (dataSetIndex >= 0) Then ' body item
                            arrItemRows(dataSetIndex) = row
                            arrItemCols(dataSetIndex) = col
#If NET_2_0 = 1 Then
                                arrDataKeys(dataSetIndex) = CreateItemDataKey(item)
#End If
                        End If
                    End If
                End If
            Next col
        Next row
        ViewState("RowCount") = Table1.Rows.Count   ' number of rows
        ViewState("ItemCount") = Items.Count        ' number of items in datasource
        ViewState("arrCellCount") = arrCellCount    ' number of cells in each row
        ViewState("arrHeaderCount") = arrHeaderCount ' number of header cells in each row
        ViewState("arrItemCols") = arrItemCols      ' column index of each item
        ViewState("arrItemRows") = arrItemRows      ' row index of each item
#If NET_2_0 = 1 Then
            ViewState("arrDataKeys") = arrDataKeys      ' row index of each item
#End If
        If (EnableEmptySlotClick) Then
            ReDim Preserve arrClickRows(ClickableCellCount)    ' reduce viewstate
            ViewState("arrClickRows") = arrClickRows           ' row of each clickable cell
            ReDim Preserve arrClickColumns(ClickableCellCount)
            ViewState("arrClickColumns") = arrClickColumns     ' column of each clickable cell
        End If
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Recreate the control tree from viewstate
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Private Sub LoadControlTree()
        Controls.Clear()
        If (CBool(ViewState("Empty")) = True) Then
            ' empty control: use EmptyDataTemplate
            Dim plh As New PlaceHolder
            EmptyDataTemplate.InstantiateIn(plh)
            Controls.Add(plh)
            Return
        End If
        ' When the control is empty, and EmptyDataTemplate does not exist:
        If (ViewState("RowCount") Is Nothing) Then Return
        Dim RowCount As Integer = CInt(ViewState("RowCount"))
        Dim ItemCount As Integer = CInt(ViewState("ItemCount"))
        Dim arrCellCount() As Integer = CType(ViewState("arrCellCount"), Integer())    ' number of cells in each row
        Dim arrHeaderCount() As Integer = CType(ViewState("arrHeaderCount"), Integer()) ' number of row header cells in each row
        Dim arrItemCols() As Integer = CType(ViewState("arrItemCols"), Integer())      ' column index of each data item
        Dim arrItemRows() As Integer = CType(ViewState("arrItemRows"), Integer())      ' row index of each data item
#If NET_2_0 = 1 Then
            Dim arrDataKeys() As IOrderedDictionary = CType(ViewState("arrDataKeys"), IOrderedDictionary())      ' row index of each data item
#End If
        Dim arrClickRows() As Integer = Nothing
        Dim arrClickColumns() As Integer = Nothing
        If (EnableEmptySlotClick) Then
            arrClickRows = CType(ViewState("arrClickRows"), Integer()) ' row of each clickable cell
            arrClickColumns = CType(ViewState("arrClickColumns"), Integer()) ' column of each clickable cell
        End If
        ' clear any existing child controls
        Table1 = New Table
        Controls.Add(Table1)

        Dim week, row, col As Integer
        Dim ClickableCellCount As Integer = 0
        Dim rowsPerRepetition As Integer = RowCount \ GetRepetitionCount() ' Only used in vertical layout

        ' first recreate the table
        For row = 0 To RowCount - 1
            Table1.Rows.Add(New TableRow)
            Dim tr As TableRow = Table1.Rows(Table1.Rows.Count - 1)
            For col = 0 To arrCellCount(row) - 1
                If (col < arrHeaderCount(row)) Then
                    ' create row header cells for this week
                    tr.Cells.Add(New TableHeaderCell)
                Else
                    If (EnableEmptySlotClick) Then
                        Dim isClickableCell As Boolean = True
                        If (ShowValueMarks And Not IncludeEndValue) Then
                            If (Layout = LayoutEnum.Horizontal And arrHeaderCount(row) > 0) Then
                                ' first and last columns should not be clickable
                                ' when the title is split over several rows, only check the first row
                                If (col = arrHeaderCount(row)) Then isClickableCell = False
                                If (col = arrCellCount(row) - 1) Then isClickableCell = False
                            Else ' Vertical
                                Dim rowInThisRepetition As Integer = row Mod rowsPerRepetition
                                If (rowInThisRepetition = 1) Then isClickableCell = False
                                If (rowInThisRepetition = rowsPerRepetition - 1) Then isClickableCell = False
                            End If
                        End If
                        If (isClickableCell) Then
                            ' col may differ from the real column because of cells
                            ' spanning several rows
                            ' We need the real column here for the EmptySlotClick to work
                            ' It's too complicated to calculate, therefore, we use ViewState
                            Dim fixedColumn As Integer = arrClickColumns(ClickableCellCount)
                            Dim fixedRow As Integer = arrClickRows(ClickableCellCount)
                            tr.Cells.Add(New ClickableTableCell(fixedRow, fixedColumn))
                            ClickableCellCount += 1
                        Else
                            tr.Cells.Add(New TableCell)
                        End If
                    Else
                        tr.Cells.Add(New TableCell)
                    End If
                End If
            Next col
        Next row

        ' now add the items
        ' it's imperative that we do it in the same order as before the postback
        ' 1) RangeHeader items
        ' 2) TitleHeader items
        ' 3) Normal Items
        ' 4) DateHeader items (if any)
        '
        ' add RangeHeader items
        If (Layout = LayoutEnum.Horizontal) Then
            ' create range header items in the first or second row 
            Dim rangeHeaderRow As Integer = GetRangeHeaderIndex()
            For col = 1 To arrCellCount(rangeHeaderRow) - 1
                CreateItem(col, rangeHeaderRow, ScheduleItemType.RangeHeader, False, Nothing, -1)
            Next col
        Else ' Layout = LayoutEnum.Vertical
            For week = 0 To GetRepetitionCount() - 1
                ' create range header items of this repetition in the first or second column 
                Dim minRangeHeaderRow As Integer = week * rowsPerRepetition + 1
                Dim maxRangeHeaderRow As Integer = week * rowsPerRepetition + rowsPerRepetition - 1
                Dim iStep As Integer = CInt(IIf(Not IncludeEndValue And ShowValueMarks, 2, 1))

                For row = minRangeHeaderRow To maxRangeHeaderRow Step iStep
                    col = arrHeaderCount(row) - 1  ' the range header column is the right-most column of the header columns
                    If (Not IncludeEndValue And ShowValueMarks) Then
                        ' in this case, the range header column is the 2nd to the right of the header columns
                        col = col - 1
                    End If

                    If (col >= 0) Then CreateItem(row, col, ScheduleItemType.RangeHeader, False, Nothing, -1)
                Next row
            Next week
        End If

        ' add Title Header items
        If (Layout = LayoutEnum.Vertical) Then
            For col = GetMinTitleIndex() To arrCellCount(0) - 1
                For week = 0 To GetRepetitionCount() - 1
                    ' create title header items in the first row of the week
                    Dim titleHeaderRow As Integer = week * rowsPerRepetition
                    CreateItem(titleHeaderRow, col, ScheduleItemType.TitleHeader, False, Nothing, -1)
                Next week
            Next col
        Else ' Layout = LayoutEnum.Horizontal
            ' create title header items in the first column
            For row = GetMinTitleIndex() To RowCount - 1
                ' titles may be merged over rows. Only the first row of a title contains the item
                ' So check if there is a title in this row
                If (arrHeaderCount(row) = 1) Then CreateItem(0, row, ScheduleItemType.TitleHeader, False, Nothing, -1)
            Next row

        End If

        ' add the (non-header) items
        Dim i As Integer
        For i = 0 To ItemCount - 1
            ' reconstruct the items
            row = arrItemRows(i)
            col = arrItemCols(i)
            Dim item As ScheduleItem
            If (i Mod 2 = 1) Then
                If (Layout = LayoutEnum.Vertical) Then
                    item = CreateItem(row, col, ScheduleItemType.AlternatingItem, False, Nothing, -1)
                Else
                    item = CreateItem(col, row, ScheduleItemType.AlternatingItem, False, Nothing, -1)
                End If
            Else
                If (Layout = LayoutEnum.Vertical) Then
                    item = CreateItem(row, col, ScheduleItemType.Item, False, Nothing, -1)
                Else
                    item = CreateItem(col, row, ScheduleItemType.Item, False, Nothing, -1)
                End If
            End If
#If NET_2_0 = 1 Then
                item.DataKey = New DataKey(arrDataKeys(i), DataKeyNames)
                ' set the same ID as before the postback
                item.ID = "item" & i
#End If
        Next i

        If (GetRangeHeaderIndex() = 1) Then
            ' add DateHeader items
            If (Layout = LayoutEnum.Horizontal) Then
                ' first row is the date header row
                For col = 1 To arrCellCount(0) - 1
                    CreateItem(col, 0, ScheduleItemType.RangeHeader, False, Nothing, -1)
                Next col
            Else ' Layout = LayoutEnum.Vertical
                For week = 0 To GetRepetitionCount() - 1
                    ' create date header items in the first column
                    Dim minDateHeaderRow As Integer = week * rowsPerRepetition + 1
                    Dim maxDateHeaderRow As Integer = week * rowsPerRepetition + rowsPerRepetition - 1
                    For row = minDateHeaderRow To maxDateHeaderRow
                        ' date headers may be merged over columns.
                        ' Only the first column of a date header contains the item
                        ' So check if there is a title in this column
                        Dim headersInDateHeaderRow As Integer = 2
                        If (Not IncludeEndValue And ShowValueMarks) Then
                            headersInDateHeaderRow = 3
                        End If

                        If arrHeaderCount(row) = headersInDateHeaderRow Then
                            ' first column contains a date header
                            CreateItem(row, 0, ScheduleItemType.RangeHeader, False, Nothing, -1)
                        End If
                    Next row
                Next
            End If
        End If

    End Sub

    Protected Overrides Function CreateControlStyle() As Style
        ' Since the Schedule control renders an HTML table,
        ' an instance of the TableStyle class is used as the control style.
        Dim mystyle As New TableStyle(ViewState)
        ' Set up default initial state.
        'mystyle.CellSpacing = 0
        mystyle.CellPadding = -1
        mystyle.GridLines = System.Web.UI.WebControls.GridLines.None
        Return mystyle
    End Function

#If Not NET_2_0 = 1 Then
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Customized state management to handle saving state of contained objects.
    ''' </summary>
    ''' <param name="savedState"></param>
    ''' -----------------------------------------------------------------------------
    Protected Overrides Sub LoadViewState(ByVal savedState As Object)
        If Not (savedState Is Nothing) Then
            Dim myState As Object() = CType(savedState, Object())

            If Not (myState(0) Is Nothing) Then
                MyBase.LoadViewState(myState(0))
            End If
            If Not (myState(1) Is Nothing) Then
                CType(ItemStyle, IStateManager).LoadViewState(myState(1))
            End If
            If Not (myState(2) Is Nothing) Then
                CType(AlternatingItemStyle, IStateManager).LoadViewState(myState(2))
            End If
            If Not (myState(3) Is Nothing) Then
                CType(RangeHeaderStyle, IStateManager).LoadViewState(myState(3))
            End If
            If Not (myState(4) Is Nothing) Then
                CType(TitleStyle, IStateManager).LoadViewState(myState(4))
            End If
            If Not (myState(5) Is Nothing) Then
                CType(BackgroundStyle, IStateManager).LoadViewState(myState(5))
            End If
        End If
    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Customize state management to handle saving state of contained objects such as styles.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------------
    Protected Overrides Function SaveViewState() As Object
        Dim baseState As Object = MyBase.SaveViewState()
        Dim itemStyleState As Object
        Dim alternatingItemStyleState As Object
        Dim RangeHeaderStyleState As Object
        Dim TitleStyleState As Object
        Dim BackgroundStyleState As Object

        If Not (_itemStyle Is Nothing) Then
            itemStyleState = CType(_itemStyle, IStateManager).SaveViewState()
        Else
            itemStyleState = Nothing
        End If
        If Not (_alternatingItemStyle Is Nothing) Then
            alternatingItemStyleState = CType(_alternatingItemStyle, IStateManager).SaveViewState()
        Else
            alternatingItemStyleState = Nothing
        End If
        If Not (_RangeHeaderStyle Is Nothing) Then
            RangeHeaderStyleState = CType(_RangeHeaderStyle, IStateManager).SaveViewState()
        Else
            RangeHeaderStyleState = Nothing
        End If
        If Not (_TitleStyle Is Nothing) Then
            TitleStyleState = CType(_TitleStyle, IStateManager).SaveViewState()
        Else
            TitleStyleState = Nothing
        End If
        If Not (_BackgroundStyle Is Nothing) Then
            BackgroundStyleState = CType(_BackgroundStyle, IStateManager).SaveViewState()
        Else
            BackgroundStyleState = Nothing
        End If

        Dim myState(6) As Object
        myState(0) = baseState
        myState(1) = itemStyleState
        myState(2) = alternatingItemStyleState
        myState(3) = RangeHeaderStyleState
        myState(4) = TitleStyleState
        myState(5) = BackgroundStyleState
        Return myState
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Customize state management to handle saving state of contained objects such as styles.
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Protected Overrides Sub TrackViewState()
        MyBase.TrackViewState()

        If Not (_itemStyle Is Nothing) Then
            CType(_itemStyle, IStateManager).TrackViewState()
        End If
        If Not (_alternatingItemStyle Is Nothing) Then
            CType(_alternatingItemStyle, IStateManager).TrackViewState()
        End If
        If Not (_RangeHeaderStyle Is Nothing) Then
            CType(_RangeHeaderStyle, IStateManager).TrackViewState()
        End If
        If Not (_TitleStyle Is Nothing) Then
            CType(_TitleStyle, IStateManager).TrackViewState()
        End If
        If Not (_BackgroundStyle Is Nothing) Then
            CType(_BackgroundStyle, IStateManager).TrackViewState()
        End If
    End Sub
#End If

#End Region

End Class