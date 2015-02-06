'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2007
' by DotNetNuke Corporation
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

Imports System.ComponentModel
Imports System.ComponentModel.Design

''' -----------------------------------------------------------------------------
''' Project	 : schedule
''' Class	 : ScheduleDesignerActionList
'''
''' -----------------------------------------------------------------------------
''' <summary>
''' ScheduleDesignerActionList adds items to the smart menu tag of the Schedule controls
''' at design time
''' </summary>
''' -----------------------------------------------------------------------------
    Public Class ScheduleDesignerActionList
        Inherits DesignerActionList

        Private myScheduleControl As BaseSchedule
        Private myDesigner As ScheduleDesigner
        Private designerActionSvc As DesignerActionService = Nothing

        Public Sub New(ByVal component As IComponent, ByVal designer As ScheduleDesigner)

            MyBase.New(component)
            Me.myScheduleControl = CType(component, BaseSchedule)
            Me.myDesigner = designer

            ' Cache a reference to DesignerActionService, so the
            ' DesigneractionList can be refreshed.
            Me.designerActionSvc = CType(GetService(GetType(DesignerActionService)), DesignerActionService)
        End Sub

        Public Sub ShowPropertyBuilder()
            myDesigner.OnPropertyBuilder(Nothing, EventArgs.Empty)
        End Sub

        Public Overrides Function GetSortedActionItems() As DesignerActionItemCollection
            Dim items As DesignerActionItemCollection = New DesignerActionItemCollection()
            items.Add(New DesignerActionMethodItem(Me, "ShowPropertyBuilder", "Property Builder...", True))
            Return items
        End Function

    End Class
