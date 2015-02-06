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
Imports System.Collections

''' -----------------------------------------------------------------------------
''' Project	 : schedule
''' Class	 : ScheduleItemCollection
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' The ScheduleItemCollection class represents a collection of ScheduleItem objects.
''' </summary>
''' -----------------------------------------------------------------------------
    Public Class ScheduleItemCollection
        Implements ICollection, IEnumerable

        Private items As ArrayList

        Public Sub New(ByVal items As ArrayList)
            Me.items = items
        End Sub

        Public ReadOnly Property Count() As Integer Implements ICollection.Count
            Get
                Return items.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property IsSynchronized() As Boolean Implements ICollection.IsSynchronized
            Get
                Return False
            End Get
        End Property

        Default Public ReadOnly Property Item(ByVal index As Integer) As ScheduleItem
            Get
                Return CType(items(index), ScheduleItem)
            End Get
        End Property

        Public ReadOnly Property SyncRoot() As Object Implements ICollection.SyncRoot
            Get
                Return Me
            End Get
        End Property

        Public Sub CopyTo(ByVal array As Array, ByVal index As Integer) Implements ICollection.CopyTo
            Dim current As ScheduleItem
            For Each current In Me
                array.SetValue(current, index)
                index = index + 1
            Next current
        End Sub

        Public Function GetEnumerator() As IEnumerator Implements ICollection.GetEnumerator
            Return items.GetEnumerator()
        End Function

    End Class 'ScheduleItemCollection

