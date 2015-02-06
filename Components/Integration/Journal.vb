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
Imports DotNetNuke.Services.Journal
Imports System.Collections.Generic

Namespace DotNetNuke.Modules.Events.Components.Integration
    Public Class Journal


#Region "Internal Methods"

        ''' <summary>
        ''' Informs the core journal that an event has been created.
        ''' </summary>
        ''' <param name="objEvent"></param>
        ''' <param name="tabId"></param>
        ''' <param name="url"></param>
        ''' <param name="imageurl"></param>
        Public Sub NewEvent(ByVal objEvent As EventInfo, ByVal tabId As Integer, url As String, ByVal imageurl As String)
            Dim objectKey As String = Constants.ContentEventTypeName + "_" + Constants.JournalEventCreateTypeName & "_" & String.Format("{0}:{1}", objEvent.ModuleID, objEvent.EventID)
            Dim ji As JournalItem = JournalController.Instance.GetJournalItemByKey(objEvent.PortalID, objectKey)

            If (ji IsNot Nothing) Then
                JournalController.Instance.DeleteJournalItemByKey(objEvent.PortalID, objectKey)
            End If

            Dim securitySet As String = "E,"
            Dim socialGroupId As Integer = Nothing
            If objEvent.SocialGroupId > 0 Then
                securitySet = ","
                socialGroupId = objEvent.SocialGroupId
            ElseIf objEvent.SocialUserId > 0 Then
                securitySet = ","
            End If

            ji = New JournalItem() With { _
             .PortalId = objEvent.PortalID, _
             .ProfileId = objEvent.OwnerID, _
             .UserId = objEvent.CreatedByID, _
             .ContentItemId = objEvent.ContentItemID, _
             .Title = objEvent.EventName, _
             .ItemData = New ItemData() With {.Url = url, .ImageUrl = imageurl}, _
             .Summary = objEvent.Summary, _
             .Body = objEvent.EventDesc, _
             .JournalTypeId = GetEventCreateJournalTypeId(objEvent.PortalID), _
             .ObjectKey = objectKey, _
             .SecuritySet = securitySet, _
             .SocialGroupId = socialGroupId _
            }

            JournalController.Instance.SaveJournalItem(ji, tabId)
        End Sub

        ''' <summary>
        ''' Informs the core journal that an event has been deleted.
        ''' </summary>
        ''' <param name="objEvent"></param>
        Public Sub DeleteEvent(ByVal objEvent As EventInfo)
            Dim objectKey As String = Constants.ContentEventTypeName + "_" + Constants.JournalEventCreateTypeName & "_" & String.Format("{0}:{1}", objEvent.ModuleID, objEvent.EventID)
            JournalController.Instance.DeleteJournalItemByKey(objEvent.PortalID, objectKey)
        End Sub

        ''' <summary>
        ''' Informs the core journal that a user has enrolled to attend an event.
        ''' </summary>
        ''' <param name="objEventSignup"></param>
        ''' <param name="objEvent"></param>
        ''' <param name="tabId"></param>
        ''' <param name="url"></param>
        ''' <param name="userid"></param>
        Public Sub NewEnrollment(ByVal objEventSignup As EventSignupsInfo, ByVal objEvent As EventInfo, ByVal tabId As Integer, url As String, ByVal userid As Integer)
            Dim objectKey As String = Constants.ContentEventTypeName + "_" + Constants.JournalEventAttendTypeName & "_" & String.Format("{0}:{1};{2}", objEventSignup.ModuleID, objEventSignup.EventID, objEventSignup.SignupID)
            Dim ji As JournalItem = JournalController.Instance.GetJournalItemByKey(objEvent.PortalID, objectKey)

            If (ji IsNot Nothing) Then
                JournalController.Instance.DeleteJournalItemByKey(objEvent.PortalID, objectKey)
            End If

            Dim securitySet As String = "E,"
            Dim socialGroupId As Integer = Nothing
            If objEvent.SocialGroupId > 0 Then
                securitySet = ","
                socialGroupId = objEvent.SocialGroupId
            End If

            ji = New JournalItem() With { _
             .PortalId = objEvent.PortalID, _
             .ProfileId = objEventSignup.UserID, _
             .UserId = userid, _
             .ContentItemId = Nothing, _
             .Title = objEvent.EventName, _
             .ItemData = New ItemData() With {.Url = url}, _
             .Summary = Nothing, _
             .Body = Nothing, _
             .JournalTypeId = GetEventAttendJournalTypeID(objEvent.PortalID), _
             .ObjectKey = objectKey, _
             .SecuritySet = securitySet, _
             .SocialGroupId = socialGroupId _
            }

            JournalController.Instance.SaveJournalItem(ji, tabId)
        End Sub


        ''' <summary>
        ''' Informs the core journal that a user has unenrolled from attend an event.
        ''' </summary>
        ''' <param name="moduleId"></param>
        ''' <param name="eventId"></param>
        ''' <param name="signupId"></param>
        ''' <param name="portalId"></param>
        Public Sub DeleteEnrollment(ByVal moduleId As Integer, eventId As Integer, signupId As Integer, ByVal portalId As Integer)
            Dim objectKey As String = Constants.ContentEventTypeName + "_" + Constants.JournalEventAttendTypeName & "_" & String.Format("{0}:{1};{2}", moduleId, eventId, signupId)
            JournalController.Instance.DeleteJournalItemByKey(portalId, objectKey)
        End Sub


#End Region

#Region "Private Methods"

        ''' <summary>
        ''' Returns a journal type associated with new event creation (using one of the core built in journal types).
        ''' </summary>
        ''' <param name="portalId"></param>
        ''' <returns></returns>
        Private Shared Function GetEventCreateJournalTypeId(portalId As Integer) As Integer
            Dim colJournalTypes As IEnumerable(Of JournalTypeInfo) = (From t In JournalController.Instance.GetJournalTypes(portalId) Where t.JournalType = Constants.JournalEventCreateTypeName)
            Dim journalTypeId As Integer

            Dim journalTypeInfos As IEnumerable(Of JournalTypeInfo) = If(TryCast(colJournalTypes, JournalTypeInfo()), colJournalTypes.ToArray())
            If journalTypeInfos.Any() Then
                Dim journalType As JournalTypeInfo = journalTypeInfos.[Single]()
                journalTypeId = journalType.JournalTypeId
            Else
                journalTypeId = 21
            End If

            Return journalTypeId
        End Function

        ''' <summary>
        ''' Returns a journal type associated with user event enrollment (using one of the core built in journal types).
        ''' </summary>
        ''' <param name="portalId"></param>
        ''' <returns></returns>
        Private Shared Function GetEventAttendJournalTypeId(portalId As Integer) As Integer
            Dim colJournalTypes As IEnumerable(Of JournalTypeInfo) = (From t In JournalController.Instance.GetJournalTypes(portalId) Where t.JournalType = Constants.JournalEventAttendTypeName)
            Dim journalTypeId As Integer

            Dim journalTypeInfos As IEnumerable(Of JournalTypeInfo) = If(TryCast(colJournalTypes, JournalTypeInfo()), colJournalTypes.ToArray())
            If journalTypeInfos.Any() Then
                Dim journalType As JournalTypeInfo = journalTypeInfos.[Single]()
                journalTypeId = journalType.JournalTypeId
            Else
                journalTypeId = 22
            End If

            Return journalTypeId
        End Function


#End Region

    End Class
End Namespace
