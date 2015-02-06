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

Imports System.Web.UI.WebControls

''' -----------------------------------------------------------------------------
''' Project	 : schedule
''' Class	 : ClickableTableCell
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' The ClickableTableCell class is used for causing postback when the user
''' clicks on empty slots
''' </summary>
    Public Class ClickableTableCell
        Inherits TableCell

        Private _row As Integer
        Private _column As Integer

        Public Property Row() As Integer
            Get
                Return _row
            End Get
            Set(ByVal value As Integer)
                _row = value
            End Set
        End Property

        Public Property Column() As Integer
            Get
                Return _column
            End Get
            Set(ByVal value As Integer)
                _column = value
            End Set
        End Property


        Public Sub New(ByVal newRow As Integer, ByVal newColumn As Integer)
            Me._row = newRow
            Me._column = newColumn
        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)
            If (Controls.Count > 0) Then Return ' don't allow clicking on cells that contain existing items
            Me.Style.Add("cursor", "hand") ' change the cursor: only works in Internet Explorer
            Dim scheduleControl As BaseSchedule = CType(Me.Parent.Parent.Parent, BaseSchedule)
            ' get tooltip from parent schedule control
            Me.ToolTip = scheduleControl.EmptySlotToolTip
            Dim eventArgument As String = _row & "-" & _column
        Me.Attributes("onclick") = Page.ClientScript.GetPostBackClientHyperlink(scheduleControl, eventArgument)
        End Sub
    End Class
