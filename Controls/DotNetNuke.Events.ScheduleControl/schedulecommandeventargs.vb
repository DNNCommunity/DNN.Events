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
Imports System.Web.UI.WebControls

''' -----------------------------------------------------------------------------
''' Project	 : schedule
''' Class	 : ScheduleItemEventArgs
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' The ScheduleItemEventArgs class can be used with the OnItemCommand event of the Schedule controls
''' </summary>
<CLSCompliant(True)> _
    Public NotInheritable Class ScheduleCommandEventArgs
    Inherits CommandEventArgs

    Private _item As ScheduleItem
    Private _commandSource As Object

    Public Sub New(ByVal item As ScheduleItem, ByVal commandSource As Object, ByVal originalArgs As CommandEventArgs)
        MyBase.New(originalArgs)
        Me._item = item
        Me._commandSource = commandSource
    End Sub

    Public ReadOnly Property Item() As ScheduleItem
        Get
            Return _item
        End Get
    End Property

    Public ReadOnly Property CommandSource() As Object
        Get
            Return _commandSource
        End Get
    End Property
End Class

    Public Delegate Sub ScheduleCommandEventHandler(ByVal sender As Object, ByVal e As ScheduleCommandEventArgs)

