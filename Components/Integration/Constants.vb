
'
' DotNetNuke® - http://www.dnnsoftware.com
' Copyright (c) 2002-2012
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

Namespace DotNetNuke.Modules.Events.Components.Integration

    Public Class Constants

#Region "Misc."

        ''' <summary>
        ''' The name of the content type stored in the ContentTypes table of the core.
        ''' </summary>
        Public Const ContentEventRecurMasterTypeName As String = "DNN_Events_EventRecurMaster"
        Public Const ContentEventTypeName As String = "DNN_Events_Event"
        Public Const JournalEventCreateTypeName As String = "eventcreate"
        Public Const JournalEventAttendTypeName As String = "eventattend"

#End Region

    End Class
End Namespace

