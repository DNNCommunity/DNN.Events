
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

Imports System.Linq
Imports DotNetNuke.Entities.Content.Common
Imports DotNetNuke.Entities.Content
Imports DotNetNuke.Common.Utilities

Namespace DotNetNuke.Modules.Events.Components.Integration

    Public Class Content

        ''' <summary>
        ''' This should only run after the Event exists in the data store. 
        ''' </summary>
        ''' <returns>The newly created ContentItemID from the data store.</returns>
        Friend Function CreateContentEventRecurMaster(objEventRecurMaster As EventRecurMasterInfo, tabId As Integer) As ContentItem

            Dim objContent As New ContentItem With { _
             .Content = objEventRecurMaster.EventDesc, _
             .ContentTypeId = GetContentTypeID(Constants.ContentEventRecurMasterTypeName), _
             .Indexed = False, _
             .ContentKey = "RecurItemID=" & objEventRecurMaster.RecurMasterID.ToString & "&mctl=EventDetails", _
             .ModuleID = objEventRecurMaster.ModuleID, _
             .TabID = tabId _
            }

            objContent.ContentItemId = Util.GetContentController().AddContentItem(objContent)

            Return objContent
        End Function

        ''' <summary>
        ''' This is used to update the content in the ContentItems table. Should be called when an Event is updated.
        ''' </summary>
        Friend Sub UpdateContentEventRecurMaster(objEventRecurMaster As EventRecurMasterInfo)
            Dim objContent As ContentItem = Util.GetContentController().GetContentItem(objEventRecurMaster.ContentItemID)

            If objContent Is Nothing Then
                Return
            End If
            objContent.Content = objEventRecurMaster.EventDesc
            objContent.ContentKey = "ItemID=" & objEventRecurMaster.RecurMasterID.ToString & "&mctl=EventDetails"

            Util.GetContentController().UpdateContentItem(objContent)

        End Sub

        ''' <summary>
        ''' This should only run after the Event exists in the data store. 
        ''' </summary>
        ''' <returns>The newly created ContentItemID from the data store.</returns>
        Friend Function CreateContentEvent(objEvent As EventInfo, tabId As Integer) As ContentItem

            Dim objContent As New ContentItem With { _
             .Content = objEvent.EventDesc, _
             .ContentTypeId = GetContentTypeID(Constants.ContentEventTypeName), _
             .Indexed = False, _
             .ContentKey = "ItemID=" & objEvent.EventID.ToString & "&mctl=EventDetails", _
             .ModuleID = objEvent.ModuleID, _
             .TabID = tabId _
            }

            objContent.ContentItemId = Util.GetContentController().AddContentItem(objContent)

            Return objContent
        End Function

        ''' <summary>
        ''' This is used to update the content in the ContentItems table. Should be called when an Event is updated.
        ''' </summary>
        Friend Sub UpdateContentEvent(objEvent As EventInfo)
            Dim objContent As ContentItem = Util.GetContentController().GetContentItem(objEvent.ContentItemID)

            If objContent Is Nothing Then
                Return
            End If
            objContent.Content = objEvent.EventDesc
            objContent.ContentKey = "ItemID=" & objEvent.EventID.ToString & "&mctl=EventDetails"

            Util.GetContentController().UpdateContentItem(objContent)

        End Sub

        ''' <summary>
        ''' This removes a content item associated with an event from the data store.
        ''' </summary>
        ''' <param name="contentItemId"></param>
        Friend Sub DeleteContentItem(contentItemId As Integer)
            If contentItemId <= Null.NullInteger Then
                Return
            End If
            Dim objContent As ContentItem = Util.GetContentController().GetContentItem(contentItemId)
            If objContent Is Nothing Then
                Return
            End If

            Util.GetContentController().DeleteContentItem(objContent)
        End Sub

        ''' <summary>
        ''' This is used to determine the ContentTypeID (part of the Core API) based on this module's content type. If the content type doesn't exist yet for the module, it is created.
        ''' </summary>
        ''' <returns>The primary key value (ContentTypeID) from the core API's Content Types table.</returns>
        Friend Shared Function GetContentTypeId(ByVal contentTypeConstant As String) As Integer
            Dim typeController As New ContentTypeController
            Dim colContentTypes As Generic.IEnumerable(Of ContentType) = (From t In typeController.GetContentTypes() Where t.ContentType = contentTypeConstant)
            Dim contentTypeId As Integer

            If colContentTypes.Any() Then
                Dim contentType As ContentType = colContentTypes.[Single]()
                contentTypeId = If(contentType Is Nothing, CreateContentType(contentTypeConstant), contentType.ContentTypeId)
            Else
                contentTypeId = CreateContentType(contentTypeConstant)
            End If

            Return contentTypeId
        End Function

#Region "Private Methods"

        ''' <summary>
        ''' Creates a Content Type (for taxonomy) in the data store.
        ''' </summary>
        ''' <returns>The primary key value of the new ContentType.</returns>
        Private Shared Function CreateContentType(ByVal contentTypeConstant As String) As Integer
            Dim typeController As New ContentTypeController
            Dim objContentType As New ContentType With { _
             .ContentType = contentTypeConstant _
            }

            Return typeController.AddContentType(objContentType)
        End Function

#End Region

    End Class
End Namespace

