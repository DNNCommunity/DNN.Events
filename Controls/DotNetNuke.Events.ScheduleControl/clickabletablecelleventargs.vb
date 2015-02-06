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

''' -----------------------------------------------------------------------------
''' Project	 : schedule
''' Class	 : ClickableTableCellEventArgs
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' The ClickableTableCellEventArgs class is used when the user clicks on an empty slot
''' </summary>
    Public NotInheritable Class ClickableTableCellEventArgs
        Inherits EventArgs

        Private _title As Object
        Private _rangeStartValue As Object
        Private _rangeEndValue As Object

        Public Sub New(ByVal newTitle As Object, ByVal newRangeStartValue As Object, _
        ByVal newRangeEndValue As Object)
            Me._title = newTitle
            Me._rangeStartValue = newRangeStartValue
            Me._rangeEndValue = newRangeEndValue
        End Sub

        Public ReadOnly Property Title() As Object
            Get
                Return _title
            End Get
        End Property

        Public ReadOnly Property RangeStartValue() As Object
            Get
                Return _rangeStartValue
            End Get
        End Property

        Public ReadOnly Property RangeEndValue() As Object
            Get
                Return _rangeEndValue
            End Get
        End Property
    End Class

    Public Delegate Sub ClickableTableCellEventHandler(ByVal sender As Object, ByVal e As ClickableTableCellEventArgs)
