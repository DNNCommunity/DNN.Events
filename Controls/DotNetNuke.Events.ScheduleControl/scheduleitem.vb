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
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.ComponentModel   ' ToolboxItem

''' -----------------------------------------------------------------------------
''' Project	 : schedule
''' Class	 : ScheduleItem
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' The ScheduleItem class represents an item in the Schedule control.
''' It can either be an item in the schedule itself, or a header item.
''' </summary>
''' -----------------------------------------------------------------------------
<CLSCompliant(True), ToolboxItem(False)> _
    Public Class ScheduleItem
    Inherits WebControl
    Implements INamingContainer

    Private _dataSetIndex As Integer = -1
    Private _itemType As ScheduleItemType
    Private _dataItem As Object
#If NET_2_0 = 1 Then
        Private _dataKey As DataKey
#End If

    Public Property DataItem() As Object
        Get
            Return _dataItem
        End Get
        Set(ByVal Value As Object)
            _dataItem = Value
        End Set
    End Property

#If NET_2_0 = 1 Then
        Public Property DataKey() As DataKey
            Get
                Return _dataKey
            End Get
            Set(ByVal Value As DataKey)
                _dataKey = Value
            End Set
        End Property
#End If

    Public Overridable ReadOnly Property DataSetIndex() As Integer
        Get
            Return _dataSetIndex
        End Get
    End Property

    Public Overridable ReadOnly Property ItemType() As ScheduleItemType
        Get
            Return _itemType
        End Get
    End Property

    Public Sub New(ByVal dataSetIndex1 As Integer, ByVal itemType1 As ScheduleItemType)
        MyBase.New()
        Me._dataSetIndex = dataSetIndex1
        Me._itemType = itemType1
    End Sub

    Protected Overrides Function OnBubbleEvent(ByVal [source] As Object, ByVal e As EventArgs) As Boolean
        ' pass any command events to the Schedule control itself
        If TypeOf e Is CommandEventArgs Then
            Dim args As New ScheduleCommandEventArgs(Me, [source], CType(e, CommandEventArgs))
            RaiseBubbleEvent(Me, args)
            Return True
        End If
        Return False
    End Function 'OnBubbleEvent

End Class

