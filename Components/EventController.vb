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
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.Security
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Services.Mail
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Modules.Events.Components.Integration
Imports System.Globalization


Namespace DotNetNuke.Modules.Events

#Region "EventInfoHelper Class"
    Public Class EventInfoHelper
        Inherits PortalModuleBase
#Region "Public Functions and Methods"

        Private _lstEvents As New ArrayList

        ' public properties
        Public Property LstEvents() As ArrayList
            Get
                Return _lstEvents
            End Get
            Set(ByVal value As ArrayList)
                _lstEvents = value
            End Set
        End Property

        ' Module ID of Base Calendar (used for converting Sub-Calendar Event Time Zones)
        Private _baseModuleId As Integer = 0
        Private _baseTabId As Integer = 0
        Private _basePortalId As Integer = 0
        Private _baseSettings As EventModuleSettings
        Public Property BaseModuleID() As Integer
            Get
                Return _baseModuleId
            End Get
            Set(ByVal value As Integer)
                _baseModuleId = value
            End Set
        End Property

        Public Property BaseTabID() As Integer
            Get
                Return _baseTabId
            End Get
            Set(ByVal value As Integer)
                _baseTabId = value
            End Set
        End Property

        Public Property BasePortalID() As Integer
            Get
                Return _basePortalId
            End Get
            Set(ByVal value As Integer)
                _basePortalId = value
            End Set
        End Property

        Public Property BaseSettings() As EventModuleSettings
            Get
                Return _baseSettings
            End Get
            Set(ByVal value As EventModuleSettings)
                _baseSettings = value
            End Set
        End Property

        Public Sub New()
        End Sub

        Public Sub New(ByVal moduleID As Integer, ByVal settings As EventModuleSettings)
            BaseModuleID = moduleID
            BaseSettings = settings
        End Sub

        Public Sub New(ByVal moduleID As Integer, ByVal tabID As Integer, ByVal portalID As Integer, ByVal settings As EventModuleSettings)
            BaseModuleID = moduleID
            BaseTabID = tabID
            BasePortalID = portalID
            BaseSettings = settings
        End Sub

        Public Function GetDetailPageRealURL(ByVal eventID As Integer, ByVal socialGroupId As Integer, ByVal socialUserId As Integer) As String
            Return GetDetailPageRealURL(eventID, True, socialGroupId, socialUserId)
        End Function

        Public Function GetDetailPageRealURL(ByVal eventID As Integer, ByVal blSkinContainer As Boolean, ByVal socialGroupId As Integer, ByVal socialUserId As Integer) As String
            Dim url As String
            If BaseSettings.eventdetailnewpage Then
                If socialGroupId > 0 Then
                    url = NavigateURL(BaseTabID, "Details", "Mid=" & BaseModuleID.ToString, "ItemID=" & eventID.ToString(), "groupid=" & socialGroupId.ToString)
                ElseIf socialUserId > 0 Then
                    url = NavigateURL(BaseTabID, "Details", "Mid=" & BaseModuleID.ToString, "ItemID=" & eventID.ToString(), "userid=" & socialUserId.ToString)
                Else
                    url = NavigateURL(BaseTabID, "Details", "Mid=" & BaseModuleID.ToString, "ItemID=" & eventID.ToString())
                End If
                If blSkinContainer Then
                    url = AddSkinContainerControls(url, "?")
                End If
            Else
                If socialGroupId > 0 Then
                    url = NavigateURL(BaseTabID, "", "ModuleID=" & BaseModuleID.ToString, "ItemID=" & eventID.ToString(), "mctl=EventDetails", "groupid=" & socialGroupId.ToString)
                ElseIf socialUserId > 0 Then
                    url = NavigateURL(BaseTabID, "", "ModuleID=" & BaseModuleID.ToString, "ItemID=" & eventID.ToString(), "mctl=EventDetails", "userid=" & socialUserId.ToString)
                Else
                    url = NavigateURL(BaseTabID, "", "ModuleID=" & BaseModuleID.ToString, "ItemID=" & eventID.ToString(), "mctl=EventDetails")
                End If
            End If
            If InStr(1, url, "://") = 0 Then
                Dim domainurl As String = GetDomainURL()
                url = AddHTTP(domainurl) & url
            End If
            Return url
        End Function

        Public Function GetDomainURL() As String
            ' Dim domainurl As String = ps.PortalAlias.HTTPAlias
            Dim domainurl As String = HttpContext.Current.Request.ServerVariables("HTTP_HOST")
            If domainurl Is Nothing Then
                Dim ps As Entities.Portals.PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), Entities.Portals.PortalSettings)
                domainurl = ps.PortalAlias.HTTPAlias()
            End If
            If domainurl.IndexOf("/", StringComparison.Ordinal) > 0 Then
                domainurl = domainurl.Substring(0, domainurl.IndexOf("/", StringComparison.Ordinal))
            End If
            Return domainurl
        End Function

        Public Function AddSkinContainerControls(ByVal url As String, ByVal addchar As String) As String
            If Left(url, 10).ToLower = "javascript" Then
                Return url
            End If

            If BaseSettings.enablecontainerskin Then
                Dim strFriendlyUrls As String = Entities.Controllers.HostController.Instance.GetString("UseFriendlyUrls")
                If strFriendlyUrls = "N" Then
                    addchar = "&"
                End If
                Dim objCtlTab As New TabController
                Dim objTabInfo As TabInfo = objCtlTab.GetTab(BaseTabID, BasePortalID, False)
                Dim skinSrc As String = Nothing
                If Not objTabInfo Is Nothing Then
                    If Not objTabInfo.SkinSrc = "" Then
                        skinSrc = objTabInfo.SkinSrc
                        If Right(skinSrc, 5) = ".ascx" Then
                            skinSrc = Left(skinSrc, Len(skinSrc) - 5)
                        End If
                    End If
                End If
                Dim objCtlModule As New ModuleController
                Dim objModuleInfo As ModuleInfo = objCtlModule.GetModule(BaseModuleID, BaseTabID, False)
                Dim containerSrc As String = Nothing
                If Not objModuleInfo Is Nothing Then
                    If objModuleInfo.DisplayTitle Then
                        If Not objModuleInfo.ContainerSrc = "" Then
                            containerSrc = objModuleInfo.ContainerSrc
                        ElseIf Not objTabInfo.ContainerSrc = "" Then
                            containerSrc = objTabInfo.ContainerSrc
                        End If
                        If Not containerSrc Is Nothing Then
                            If Right(containerSrc, 5) = ".ascx" Then
                                containerSrc = Left(containerSrc, Len(containerSrc) - 5)
                            End If
                        End If
                    Else
                        containerSrc = "[G]Containers/_default/No+Container"
                    End If
                End If
                If Not containerSrc Is Nothing Then
                    url += addchar + "ContainerSrc=" & HttpUtility.HtmlEncode(containerSrc)
                    addchar = "&"
                End If
                If Not skinSrc Is Nothing Then
                    url += addchar + "SkinSrc=" & HttpUtility.HtmlEncode(skinSrc)
                End If
            End If
            Return url
        End Function

        Public Function HideFullEvent(ByVal objevent As EventInfo, ByVal blEventHideFullEnroll As Boolean, ByVal intuserid As Integer, ByVal blAuthenticated As Boolean) As Boolean
            If objevent.Signups And blEventHideFullEnroll And objevent.MaxEnrollment > 0 And _
                objevent.Enrolled >= objevent.MaxEnrollment And UserId <> objevent.OwnerID And _
                intuserid <> objevent.CreatedByID And intuserid <> objevent.RMOwnerID And Not IsModerator(blAuthenticated) Then
                Return True
            End If
            Return False
        End Function

        Public Function DetailPageURL(ByVal objEvent As EventInfo) As String
            Return DetailPageURL(objEvent, True)
        End Function

        Public Function DetailPageURL(ByVal objEvent As EventInfo, ByVal blSkinContainer As Boolean) As String
            Dim returnURL As String
            If objEvent.DetailPage Then

                If objEvent.DetailURL.StartsWith("http") Then
                    returnURL = objEvent.DetailURL
                Else
                    returnURL = Common.Globals.LinkClick(objEvent.DetailURL, BaseTabID, BaseModuleID, False)
                End If
            Else
                returnURL = GetDetailPageRealURL(objEvent.EventID, blSkinContainer, objEvent.SocialGroupId, objEvent.SocialUserId)
            End If
            Return returnURL

        End Function

        Public Function GetModerateUrl() As String
            Return NavigateURL(BaseTabID, "", "Mid=" & BaseModuleID.ToString, "mctl=EventModerate")
        End Function

        Public Function GetEditURL(ByVal itemid As Integer, ByVal socialGroupId As Integer, ByVal socialUserId As Integer) As String
            Return GetEditURL(itemid, socialGroupId, socialUserId, "Single")
        End Function

        Public Function GetEditURL(ByVal itemid As Integer, ByVal socialGroupId As Integer, ByVal socialUserId As Integer, ByVal editRecur As String) As String
            If socialGroupId > 0 Then
                Return AddSkinContainerControls(EditUrl("ItemID", itemid.ToString, "Edit", "Mid=" & BaseModuleID.ToString, "EditRecur=" & editRecur, "groupid=" & socialGroupId.ToString), "?")
            ElseIf socialUserId > 0 Then
                Return AddSkinContainerControls(EditUrl("ItemID", itemid.ToString, "Edit", "Mid=" & BaseModuleID.ToString, "EditRecur=" & editRecur, "userid=" & socialUserId.ToString), "?")
            Else
                Return AddSkinContainerControls(EditUrl("ItemID", itemid.ToString, "Edit", "Mid=" & BaseModuleID.ToString, "EditRecur=" & editRecur), "?")
            End If
        End Function

        ' Detemines if a EventInfo ArrayList already contains the EventInfo Object
        Public Function IsConflict(ByVal objEvent As EventInfo, ByVal objEvents As ArrayList, ByVal conflictDateChk As DateTime) As DateTime
            Dim objEvent1 As EventInfo
            Dim objEvent2 As EventInfo
            Dim eventTimeBegin1, eventTimeBegin2, eventTimeEnd1, eventTimeEnd2 As Date
            Dim locationConflict As Boolean = BaseSettings.locationconflict

            ' Handle Recurring Event Conflict Detection
            LstEvents = New ArrayList
            AddEvent(objEvent)

            'Convert both lists to common timezone
            objEvents = ConvertEventListToDisplayTimeZone(objEvents, BaseSettings.TimeZoneId)
            LstEvents = ConvertEventListToDisplayTimeZone(LstEvents, BaseSettings.TimeZoneId)

            For Each objEvent1 In LstEvents
                ' Take into account timezone offsets and length of event when deciding on conflicts
                eventTimeBegin1 = objEvent1.EventTimeBegin
                eventTimeEnd1 = objEvent1.EventTimeEnd
                For Each objEvent2 In objEvents
                    eventTimeBegin2 = objEvent2.EventTimeBegin
                    eventTimeEnd2 = objEvent2.EventTimeEnd
                    If ((eventTimeBegin1 >= eventTimeBegin2 And eventTimeBegin1 < eventTimeEnd2) Or _
                         (eventTimeBegin1 <= eventTimeBegin2 And eventTimeEnd1 > eventTimeBegin2)) _
                         And (objEvent1.EventID <> objEvent2.EventID) Then
                        If locationConflict Then
                            If objEvent1.Location > 0 And objEvent1.Location = objEvent2.Location Then
                                Return objEvent.EventTimeBegin
                            End If
                        Else
                            Return objEvent.EventTimeBegin
                        End If
                    End If
                Next
            Next
            Return conflictDateChk
        End Function

        ' Get Events (including SubModule Events, including categories, including locations)
        Public Function GetEvents(ByVal startDate As DateTime, ByVal endDate As DateTime, ByVal getSubEvents As Boolean, ByVal categoryIDs As ArrayList, ByVal locationIDs As ArrayList, ByVal socialGroupId As Integer, ByVal socialUserId As Integer) As ArrayList
            Return GetEvents(startDate, endDate, getSubEvents, categoryIDs, locationIDs, False, socialGroupId, socialUserId)
        End Function

        Public Function GetEvents(ByVal startDate As DateTime, ByVal endDate As DateTime, ByVal getSubEvents As Boolean, ByVal inCategoryIDs As ArrayList, ByVal inLocationIDs As ArrayList, ByVal isSearch As Boolean, ByVal socialGroupId As Integer, ByVal socialUserId As Integer) As ArrayList
            'Dim objEventInfoHelper As New EventInfoHelper(ModID)
            Dim objCtlMasterEvent As New EventMasterController
            Dim moduleIDs As String = BaseModuleID.ToString
            Try
                '*** See what Sub-Events/Calendars are included
                If getSubEvents Then
                    ' Get Assigned Sub Events and Bind to Grid
                    Dim subEvents As ArrayList
                    subEvents = objCtlMasterEvent.EventsMasterAssignedModules(BaseModuleID)
                    If Not (subEvents Is Nothing) Then
                        Dim myEnumerator As IEnumerator = subEvents.GetEnumerator()
                        Dim objSubEvent As EventMasterInfo
                        While myEnumerator.MoveNext()
                            objSubEvent = CType(myEnumerator.Current, EventMasterInfo)
                            If IsModuleViewer(objSubEvent.SubEventID) Or Not BaseSettings.Enforcesubcalperms Then
                                moduleIDs = moduleIDs + "," + objSubEvent.SubEventID.ToString
                            End If
                        End While
                    End If
                End If

                ' Adds Recurring Dates to EventInfo ArrayList for given Date Range
                Dim objEvents As EventInfo
                Dim ctrlEvent As New EventController
                _lstEvents = New ArrayList
                _lstEvents.Clear()

                Dim categoryIDs As String = CreateCategoryFilter(inCategoryIDs)
                Dim locationIDs As String = CreateLocationFilter(inLocationIDs)

                Dim newlstEvents As New ArrayList
                If BaseSettings.SocialGroupModule = EventModuleSettings.SocialModule.No Or IsSocialUserPublic(socialUserId) Or IsSocialGroupPublic(socialGroupId) Then
                    newlstEvents = ctrlEvent.EventsGetByRange(moduleIDs, startDate, endDate, categoryIDs, locationIDs, socialGroupId, socialUserId)
                End If

                For Each objEvents In newlstEvents
                    ' If the module is set for private events, then obfuscate the appropriate information
                    objEvents.IsPrivate = False
                    If BaseSettings.PrivateMessage <> "" Then
                        If isSearch Then
                            objEvents.EventName = BaseSettings.PrivateMessage
                            objEvents.EventDesc = ""
                            objEvents.Summary = ""
                            objEvents.IsPrivate = True
                        ElseIf Not UserId = objEvents.OwnerID And Not IsModerator(True) Then
                            objEvents.EventName = BaseSettings.PrivateMessage
                            objEvents.EventDesc = ""
                            objEvents.Summary = ""
                            objEvents.IsPrivate = True
                        End If
                    End If
                    objEvents.ModuleTitle = objCtlMasterEvent.GetModuleTitle(objEvents.ModuleID)
                    AddEvent(objEvents)
                Next
                LstEvents.Sort(New EventDateSort)

                Return LstEvents
            Catch ex As Exception
                Return New ArrayList
            End Try
        End Function

        ' Get Events for a Specifc Date and returns a EventInfo ArrayList
        Public Function GetDateEvents(ByVal selectedEvents As ArrayList, ByVal selectDate As Date) As ArrayList
            Dim newEventEvents As New ArrayList
            Dim itemid As Integer = 0
            ' Modified to account for multi-day events
            If Not (selectedEvents Is Nothing) Then
                For Each objEvent As EventInfo In selectedEvents
                    If objEvent.EventTimeBegin = selectDate Or _
                       (objEvent.EventTimeBegin.Date <= selectDate.Date And _
                        objEvent.EventTimeEnd.Date >= selectDate.Date) Then
                        If itemid <> objEvent.EventID Then
                            newEventEvents.Add(objEvent)
                        End If
                        itemid = objEvent.EventID
                    End If
                Next
            End If
            Return newEventEvents
        End Function

        Public Function CreateCategoryFilter(inCategoryIDs As ArrayList) As String
            Dim restrictedCategories As New ArrayList
            If BaseSettings.restrictcategories Then
                If inCategoryIDs.Item(0).ToString = "-1" Then
                    restrictedCategories = BaseSettings.ModuleCategoryIDs
                Else
                    For Each inCategory As Integer In inCategoryIDs
                        For Each category As Integer In BaseSettings.ModuleCategoryIDs
                            If category = inCategory Then
                                restrictedCategories.Add(category)
                            End If
                        Next
                    Next
                End If
            Else
                restrictedCategories = inCategoryIDs
            End If

            Dim categoryIDs As String = ""
            ' ReSharper disable LoopCanBeConvertedToQuery
            For Each category As String In restrictedCategories
                ' ReSharper restore LoopCanBeConvertedToQuery
                categoryIDs = categoryIDs + "," + category
            Next
            categoryIDs = Mid(categoryIDs, 2)

            Return categoryIDs
        End Function

        Public Function CreateLocationFilter(inLocationIDs As ArrayList) As String
            ' Because of method overloads without locations.
            If inLocationIDs Is Nothing Then inLocationIDs = New ArrayList({"-1"})

            Dim restrictedLocations As New ArrayList
            If BaseSettings.Restrictlocations Then
                If inLocationIDs.Item(0).ToString = "-1" Then
                    restrictedLocations = BaseSettings.ModuleLocationIDs
                Else
                    For Each inLocation As Integer In inLocationIDs
                        For Each location As Integer In BaseSettings.ModuleLocationIDs
                            If location = inLocation Then
                                restrictedLocations.Add(location)
                            End If
                        Next
                    Next
                End If
            Else
                restrictedLocations = inLocationIDs
            End If

            Dim locationIDs As String = ""
            ' ReSharper disable LoopCanBeConvertedToQuery
            For Each location As String In restrictedLocations
                ' ReSharper restore LoopCanBeConvertedToQuery
                locationIDs = locationIDs + "," + location
            Next
            locationIDs = Mid(locationIDs, 2)

            Return locationIDs
        End Function

        Public Function IsModuleViewer(ByVal subModuleID As Integer) As Boolean
            Try
                If Not PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString) Then
                    Dim objCtlModule As New ModuleController

                    Dim objModules As ArrayList = CType(objCtlModule.GetTabModulesByModule(subModuleID), ArrayList)
                    Dim objModule As ModuleInfo
                    For Each objModule In objModules
                        If Not objModule.InheritViewPermissions Then
                            Dim objCollModulePermission As ModulePermissionCollection
                            objCollModulePermission = ModulePermissionController.GetModulePermissions(subModuleID, objModule.TabID)
                            Return ModulePermissionController.HasModulePermission(objCollModulePermission, "VIEW")
                        Else
                            Dim objCollTabPermission As TabPermissionCollection
                            objCollTabPermission = TabPermissionController.GetTabPermissions(objModule.TabID, PortalId)
                            Return TabPermissionController.HasTabPermission(objCollTabPermission, "VIEW")
                        End If
                    Next
                    Return False
                Else
                    Return True
                End If

            Catch
            End Try
            Return False
        End Function

        Public Function IsSocialUserPublic(ByVal socialUserId As Integer) As Boolean
            If (BaseSettings.SocialGroupModule = EventModuleSettings.SocialModule.UserProfile And Not BaseSettings.SocialUserPrivate) Then
                Return True
            ElseIf (socialUserId = -1) Then
                Return False
            ElseIf (BaseSettings.SocialGroupModule = EventModuleSettings.SocialModule.UserProfile And BaseSettings.SocialUserPrivate And socialUserId = PortalSettings.UserInfo.UserID) Then
                Return True
            End If
            Return False
        End Function

        Public Function IsSocialGroupPublic(ByVal socialGroupId As Integer) As Boolean
            If (BaseSettings.SocialGroupModule = EventModuleSettings.SocialModule.SocialGroup And Not BaseSettings.SocialGroupSecurity = EventModuleSettings.SocialGroupPrivacy.PrivateToGroup) Then
                Return True
            ElseIf (socialGroupId = -1) Then
                Return False
            Else
                Dim objRoleCtl As New RoleController
                Dim objRoleInfo As RoleInfo = objRoleCtl.GetRoleById(socialGroupId, PortalSettings.PortalId)
                If objRoleInfo Is Nothing Then
                    Return False
                End If
                If (BaseSettings.SocialGroupModule = EventModuleSettings.SocialModule.SocialGroup And BaseSettings.SocialGroupSecurity = EventModuleSettings.SocialGroupPrivacy.PrivateToGroup _
                                    And PortalSettings.UserInfo.IsInRole(objRoleInfo.RoleName)) Then
                    Return True
                Else
                    Return False
                End If
            End If
        End Function

        Public Function IsModerator(ByVal blAuthenticated As Boolean) As Boolean
            If blAuthenticated Then
                Try
                    Dim mc As New ModuleController
                    Dim objMod As ModuleInfo
                    Dim mp As ModulePermissionCollection

                    objMod = mc.GetModule(BaseModuleID, BaseTabID, False)

                    If Not objMod Is Nothing Then
                        mp = objMod.ModulePermissions
                        Return ModulePermissionController.HasModulePermission(mp, "EVENTSMOD")
                    Else
                        Return False
                    End If

                Catch
                End Try
            End If
            Return False
        End Function

        Public Function GetEventModuleViewers() As ArrayList
            Dim objCtlModule As New ModuleController
            Dim objModule As ModuleInfo = objCtlModule.GetModule(BaseModuleID, BaseTabID, False)

            Dim lstUsers As New ArrayList
            Dim lstDeniedUsers As New ArrayList

            If Not objModule.InheritViewPermissions Then
                Dim objModulePermission As ModulePermissionInfo
                For Each objModulePermission In objModule.ModulePermissions
                    If objModulePermission.PermissionKey = "VIEW" Then
                        If objModulePermission.UserID < 0 Then
                            Dim roleName As String
                            Dim objCtlRole As New RoleController
                            If objModulePermission.RoleID < 0 Then
                                roleName = PortalSettings.RegisteredRoleName
                            Else
                                roleName = objModulePermission.RoleName
                            End If
                            Dim lstRoleUsers As ArrayList = CType(objCtlRole.GetUsersByRole(BasePortalID, roleName), ArrayList)
                            For Each objUser As UserInfo In lstRoleUsers
                                AddViewUserid(objUser.UserID, objModulePermission.AllowAccess, lstUsers, lstDeniedUsers)
                            Next
                        Else
                            AddViewUserid(objModulePermission.UserID, objModulePermission.AllowAccess, lstUsers, lstDeniedUsers)
                        End If
                    End If
                Next
            Else
                Dim objCollTabPermission As TabPermissionCollection
                objCollTabPermission = TabPermissionController.GetTabPermissions(BaseTabID, BasePortalID)
                Dim objTabPermission As TabPermissionInfo
                For Each objTabPermission In objCollTabPermission
                    If objTabPermission.PermissionKey = "VIEW" Then
                        If objTabPermission.UserID < 0 Then
                            Dim roleName As String
                            Dim objCtlRole As New RoleController
                            If objTabPermission.RoleID < 0 Then
                                roleName = PortalSettings.RegisteredRoleName
                            Else
                                roleName = objTabPermission.RoleName
                            End If
                            Dim lstRoleUsers As ArrayList = CType(objCtlRole.GetUsersByRole(BasePortalID, roleName), ArrayList)
                            For Each objUser As UserInfo In lstRoleUsers
                                AddViewUserid(objUser.UserID, objTabPermission.AllowAccess, lstUsers, lstDeniedUsers)
                            Next
                        Else
                            AddViewUserid(objTabPermission.UserID, objTabPermission.AllowAccess, lstUsers, lstDeniedUsers)
                        End If
                    End If
                Next
            End If
            Return lstUsers
        End Function

        Private Sub AddViewUserid(ByVal viewUserid As Integer, ByVal viewAllowAccess As Boolean, ByVal lstUsers As ArrayList, ByVal lstDeniedUsers As ArrayList)
            If lstUsers.Contains(viewUserid) And Not viewAllowAccess Then
                lstUsers.Remove(viewUserid)
            End If
            If lstDeniedUsers.Contains(viewUserid) Then
                Exit Sub
            End If
            If Not viewAllowAccess Then
                lstDeniedUsers.Add(viewUserid)
                Exit Sub
            End If
            If lstUsers.Contains(viewUserid) Then
                Exit Sub
            End If
            lstUsers.Add(viewUserid)
        End Sub

        Public Function ConvertDateTimeToTZ(ByVal fromDateTime As DateTime, ByVal fromTZ As Integer, ByVal toTZ As Integer) As DateTime
            Return fromDateTime.AddMinutes(Convert.ToDouble(toTZ) - Convert.ToDouble(fromTZ))
        End Function

        Public Function UserDisplayNameProfile(ByVal prfUserID As Integer, ByVal prfDisplayName As String, ByVal inLocalResourceFile As String) As EventUser
            Dim domainurl As String = GetDomainURL()
            UserDisplayNameProfile = New EventUser
            With UserDisplayNameProfile
                .UserID = prfUserID
                .DisplayName = prfDisplayName
                If .DisplayName <> "" Then
                    .ProfileURL = HttpUtility.HtmlEncode(Common.Globals.UserProfileURL(.UserID))
                    If Not .ProfileURL.ToLower.StartsWith("http://") And Not .ProfileURL.ToLower.StartsWith("https://") Then
                        .ProfileURL = AddHTTP(domainurl) & .ProfileURL
                    End If
                    .DisplayNameURL = String.Format("<a href=""{0}""{2}>{1}</a>", .ProfileURL, .DisplayName, " target=""_blank""")
                Else
                    .ProfileURL = NavigateURL()
                    .DisplayName = Localization.GetString("UserDeleted", inLocalResourceFile)
                    .DisplayNameURL = .DisplayName
                End If
            End With
            Return UserDisplayNameProfile
        End Function

        Public Function ConvertEventToDisplayTimeZone(ByVal moduleEvent As EventInfo, ByVal displayTimeZoneId As String) As EventInfo
            With moduleEvent
                Dim objEventTimeZoneUtilities As New EventTimeZoneUtilities
                Dim eventDateInfo As EventTimeZoneUtilities.DateInfo
                ' Convert to display timezone based on timezone event was stored in
                eventDateInfo = objEventTimeZoneUtilities.ConvertToDisplayTimeZone(.EventTimeBegin, .EventTimeZoneId, .PortalID, displayTimeZoneId)
                ' If it is an all day event then no need to adjust
                If Not moduleEvent.AllDayEvent Then
                    .EventTimeBegin = eventDateInfo.EventDate
                End If
                eventDateInfo = objEventTimeZoneUtilities.ConvertToDisplayTimeZone(.LastRecurrence, .EventTimeZoneId, .PortalID, displayTimeZoneId)
                ' If it is an all day event then no need to adjust
                If Not moduleEvent.AllDayEvent Then
                    .LastRecurrence = eventDateInfo.EventDate
                End If
                ' Store the new timezone so it can be used again if needed
                .EventTimeZoneId = eventDateInfo.EventTimeZoneId
                ' Convert to display timezone. If times have not been converted before, then OtherTimeZoneId will conatin "UTC" since these date are stored to DB in UTC
                eventDateInfo = objEventTimeZoneUtilities.ConvertToDisplayTimeZone(.CreatedDate, .OtherTimeZoneId, .PortalID, displayTimeZoneId)
                .CreatedDate = eventDateInfo.EventDate
                eventDateInfo = objEventTimeZoneUtilities.ConvertToDisplayTimeZone(.LastUpdatedAt, .OtherTimeZoneId, .PortalID, displayTimeZoneId)
                .LastUpdatedAt = eventDateInfo.EventDate
                ' Store the new timezone so it can be used again if needed
                .OtherTimeZoneId = eventDateInfo.EventTimeZoneId
            End With
            Return moduleEvent
        End Function

        Public Function ConvertEventListToDisplayTimeZone(ByVal moduleEvents As ArrayList, ByVal displayTimeZoneId As String) As ArrayList
            Dim outEvents As New ArrayList
            For Each objEvent As EventInfo In moduleEvents
                outEvents.Add(ConvertEventToDisplayTimeZone(objEvent, displayTimeZoneId))
            Next
            Return outEvents
        End Function

        ' DateTime Sort Class
        Public Class EventDateSort
            Implements IComparer

            Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
                Dim xdatetimeid As String
                Dim ydatetimeid As String
                Dim xEventInfo As EventInfo = CType(x, EventInfo)
                Dim yEventInfo As EventInfo = CType(y, EventInfo)

                Dim objEventTimeZoneUtilities As New EventTimeZoneUtilities
                xdatetimeid = Format(objEventTimeZoneUtilities.ConvertToUTCTimeZone(xEventInfo.EventTimeBegin, xEventInfo.EventTimeZoneId), "yyyyMMddHHmm") + Format(xEventInfo.EventID, "00000000")
                ydatetimeid = Format(objEventTimeZoneUtilities.ConvertToUTCTimeZone(yEventInfo.EventTimeBegin, yEventInfo.EventTimeZoneId), "yyyyMMddHHmm") + Format(yEventInfo.EventID, "00000000")
                Return CInt(xdatetimeid < ydatetimeid)
            End Function
        End Class


#End Region

#Region "Private Functions and Methods"
        ' Adds a new Recurring Event (not the real event) to the EventInfo ArrayList
        ' Sets Instance Date/Time and Converts to User Time Zone
        Private Sub AddEvent(ByVal objEvent As EventInfo)
            Dim objEvent2 As EventInfo = objEvent.Clone

            LstEvents.Add(objEvent2)
        End Sub

#End Region

    End Class
#End Region

#Region "EventController Class "

    <DNNtc.UpgradeEventMessage("01.01.01,04.00.02,04.01.00,05.02.00")> _
    <DNNtc.BusinessControllerClass()> _
    Public Class EventController
        Implements ISearchable, IUpgradeable

        Public Sub EventsDelete(ByVal eventID As Integer, ByVal moduleID As Integer, ByVal contentItemID As Integer)
            ' Dim cntTaxonomy As New Content
            ' cntTaxonomy.DeleteContentItem(ContentItemID)
            DataProvider.Instance().EventsDelete(eventID, moduleID)
        End Sub

        Public Function EventsGet(ByVal eventID As Integer, ByVal moduleID As Integer) As EventInfo
            Dim eventInfo As EventInfo = CType(CBO.FillObject(DataProvider.Instance().EventsGet(eventID, moduleID), GetType(EventInfo)), EventInfo)
            If Not eventInfo Is Nothing Then
                Dim objCtlMasterEvent As New EventMasterController
                eventInfo.ModuleTitle = objCtlMasterEvent.GetModuleTitle(eventInfo.ModuleID)
            End If
            Return eventInfo
        End Function

        Public Function EventsGetByRange(ByVal moduleIDs As String, ByVal beginDate As DateTime, ByVal endDate As DateTime, ByVal categoryIDs As String, ByVal locationIDs As String, ByVal socialGroupId As Integer, ByVal socialUserId As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().EventsGetByRange(moduleIDs, beginDate, endDate, categoryIDs, locationIDs, socialGroupId, socialUserId), GetType(EventInfo))
        End Function


        Public Function EventsSave(ByVal objEvent As EventInfo, ByVal saveOnly As Boolean, ByVal tabID As Integer, ByVal updateContent As Boolean) As EventInfo
            ' Dim cntTaxonomy As New Content
            ' If UpdateContent Then
            ' If Not objEvent.ContentItemID = Nothing And objEvent.ContentItemID <> 0 Then
            ' If Not objEvent.Cancelled Then
            ' cntTaxonomy.UpdateContentEvent(objEvent)
            ' Else
            ' cntTaxonomy.DeleteContentItem(objEvent.ContentItemID)
            ' objEvent.ContentItemID = Nothing
            ' End If
            ' End If
            ' End If

            If objEvent.Cancelled And objEvent.JournalItem Then
                Dim cntJournal As New Journal
                cntJournal.DeleteEvent(objEvent)
                objEvent.JournalItem = False
            End If

            Dim objEventOut As EventInfo = EventsSave(objEvent, saveOnly)

            ' If UpdateContent And objEvent.EventID = -1 Then
            ' If objEventOut.ContentItemID = 0 And Not SaveOnly And Not TabID = Nothing Then
            ' objEventOut.ContentItemID = cntTaxonomy.CreateContentEvent(objEventOut, TabID).ContentItemId
            ' EventsSave(objEventOut, SaveOnly)
            ' End If
            ' End If

            Return objEventOut
        End Function

        Public Function EventsSave(ByVal objEvent As EventInfo, ByVal saveOnly As Boolean) As EventInfo
            Return CType(CBO.FillObject(DataProvider.Instance().EventsSave(objEvent.PortalID, objEvent.EventID, objEvent.RecurMasterID, _
                            objEvent.ModuleID, _
                            objEvent.EventTimeBegin, objEvent.Duration, objEvent.EventName, _
                            objEvent.EventDesc, objEvent.Importance, _
                            objEvent.CreatedByID.ToString, objEvent.Notify, objEvent.Approved, objEvent.Signups, _
                            objEvent.MaxEnrollment, objEvent.EnrollRoleID, objEvent.EnrollFee, objEvent.EnrollType, _
                            objEvent.PayPalAccount, objEvent.Cancelled, _
                            objEvent.DetailPage, objEvent.DetailNewWin, objEvent.DetailURL, objEvent.ImageURL, objEvent.ImageType, objEvent.ImageWidth, objEvent.ImageHeight, _
                            objEvent.ImageDisplay, objEvent.Location, _
                            objEvent.Category, _
                            objEvent.Reminder, objEvent.SendReminder, objEvent.ReminderTime, _
                            objEvent.ReminderTimeMeasurement, objEvent.ReminderFrom, objEvent.SearchSubmitted, objEvent.CustomField1, objEvent.CustomField2, _
                            objEvent.EnrollListView, objEvent.DisplayEndDate, objEvent.AllDayEvent, objEvent.OwnerID, objEvent.LastUpdatedID, objEvent.OriginalDateBegin, _
                            objEvent.NewEventEmailSent, objEvent.AllowAnonEnroll, objEvent.ContentItemID, objEvent.JournalItem, objEvent.Summary, saveOnly), _
                            GetType(EventInfo)), EventInfo)
        End Function

        Public Function EventsModerateEvents(ByVal moduleID As Integer, ByVal socialGroupId As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().EventsModerateEvents(moduleID, socialGroupId), GetType(EventInfo))
        End Function

        Public Function EventsTimeZoneCount(ByVal moduleID As Integer) As Integer
            ' ReSharper disable RedundantCast
            Return CType(DataProvider.Instance().EventsTimeZoneCount(CInt(moduleID)), Integer)
            ' ReSharper restore RedundantCast
        End Function

        Public Sub EventsUpgrade(ByVal moduleVersion As String)
            DataProvider.Instance().EventsUpgrade(moduleVersion)
        End Sub

        Public Sub EventsCleanupExpired(ByVal portalId As Integer, ByVal moduleId As Integer)
            DataProvider.Instance().EventsCleanupExpired(portalId, moduleId)
        End Sub

        Public Function EventsGetRecurrences(ByVal recurMasterID As Integer, ByVal moduleID As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().EventsGetRecurrences(recurMasterID, moduleID), GetType(EventInfo))
        End Function


#Region "Optional Interfaces"
        '*** Implement ISearchable
        Public Function GetSearchItems(ByVal modInfo As ModuleInfo) As Services.Search.SearchItemInfoCollection Implements ISearchable.GetSearchItems
            Dim ems As New EventModuleSettings
            Dim settings As EventModuleSettings = ems.GetEventModuleSettings(modInfo.ModuleID, Nothing)

            Dim objPortals As New Entities.Portals.PortalController
            Dim objPortal As Entities.Portals.PortalInfo
            objPortal = objPortals.GetPortal(modInfo.PortalID)
            ' Set Thread Culture for Portal Default Culture (for Dates/Times Formatting)
            Dim lang As String = objPortal.DefaultLanguage
            Threading.Thread.CurrentThread.CurrentCulture = New CultureInfo(lang, False)
            Dim searchItemCollection As New Services.Search.SearchItemInfoCollection

            Try
                If settings.Eventsearch Then
                    ' Get Date Recurrences from 6 months prior to Current Date and 1 year out
                    Dim objEventInfoHelper As New EventInfoHelper(modInfo.ModuleID, settings)
                    Dim categoryIDs As New ArrayList
                    categoryIDs.Add("-1")
                    Dim locationIDs As New ArrayList
                    locationIDs.Add("-1")
                    Dim lstEvents As ArrayList = objEventInfoHelper.GetEvents(Date.UtcNow.AddMonths(-6), Date.UtcNow.AddYears(1), False, categoryIDs, locationIDs, True, -1, -1)

                    Dim portalTimeZoneId As String = Entities.Portals.PortalController.GetPortalSetting("TimeZone", modInfo.PortalID, String.Empty)
                    lstEvents = objEventInfoHelper.ConvertEventListToDisplayTimeZone(lstEvents, portalTimeZoneId)

                    For Each objEvent As EventInfo In lstEvents
                        Dim searchItem As Services.Search.SearchItemInfo
                        With objEvent
                            ' Item Title
                            Dim strTitle As String = HttpUtility.HtmlDecode(.ModuleTitle & ": " & .EventName & ", " & .EventTimeBegin.ToString)
                            ' Displayed Description
                            Dim strDescription As String = HtmlUtils.Shorten(HtmlUtils.StripTags(HttpUtility.HtmlDecode(.EventDesc), False), 255, "...")
                            ' Search Items
                            Dim strContent As String = HttpUtility.HtmlDecode(.ModuleTitle & " " & .EventName & " " & .EventTimeBegin.ToString & " " & .EventDesc)
                            ' Added to Link
                            Dim strGUID As String = HttpUtility.HtmlDecode("ModuleID=" & .ModuleID.ToString() & "&ItemID=" & .EventID.ToString() & "&mctl=EventDetails")
                            ' Unique Item Key
                            Dim strUnique As String = "Event: " & .EventID.ToString & ", Date:" & .EventTimeBegin.ToString

                            searchItem = New Services.Search.SearchItemInfo
                            searchItem.Title = strTitle
                            searchItem.PubDate = .LastUpdatedAt
                            searchItem.Description = strDescription
                            searchItem.Author = .LastUpdatedID
                            searchItem.ModuleId = modInfo.ModuleID
                            searchItem.SearchKey = strUnique
                            searchItem.Content = strContent
                            searchItem.GUID = strGUID

                            searchItemCollection.Add(searchItem)
                        End With
                    Next
                End If
                Return searchItemCollection
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        'Public Function ExportModule(ByVal ModuleID As Integer) As String Implements Entities.Modules.IPortable.ExportModule
        '    Dim strXML As String = ""
        '    Dim objEventCtl As New EventController

        '    Dim arrEvents As ArrayList = objEventCtl.EventsGetByRange(ModuleID.ToString(), DateTime.Now, DateTime.Now.AddYears(1), "")
        '    If arrEvents.Count <> 0 Then
        '        strXML += "<events>"
        '        Dim objEvent As EventInfo
        '        For Each objEvent In arrEvents
        '            strXML += "<event>"
        '            strXML += "<description>" & XMLEncode(objEvent.EventDesc) & "</description>"
        '            strXML += "<datetime>" & XMLEncode(objEvent.EventTimeBegin.ToString) & "</datetime>"
        '            strXML += "<title>" & XMLEncode(objEvent.EventName) & "</title>"
        '            strXML += "<icon>" & XMLEncode(objEvent.IconFile) & "</icon>"
        '            strXML += "<occursevery>" & XMLEncode(objEvent.Every.ToString) & "</occursevery>"
        '            strXML += "<alttext>" & XMLEncode(objEvent.AltText) & "</alttext>"
        '            strXML += "<expires>" & XMLEncode(objEvent.ExpireDate.ToString) & "</expires>"
        '            strXML += "<maxWidth>" & XMLEncode(objEvent.MaxWidth.ToString) & "</maxWidth>"
        '            strXML += "<period>" & XMLEncode(objEvent.Period) & "</period>"
        '            strXML += "</event>"
        '        Next
        '        strXML += "</events>"
        '    End If

        '    Return strXML
        'End Function

        'Public Sub ImportModule(ByVal ModuleID As Integer, ByVal Content As String, ByVal Version As String, ByVal UserId As Integer) Implements Entities.Modules.IPortable.ImportModule
        '    Dim xmlEvent As XmlNode
        '    Dim xmlEvents As XmlNode = GetContent(Content, "events")
        '    For Each xmlEvent In xmlEvents.SelectNodes("event")
        '        Dim objEvent As New EventInfo
        '        objEvent.ModuleID = ModuleID
        '        objEvent.Description = xmlEvent.Item("description").InnerText
        '        objEvent.DateTime = Date.Parse(xmlEvent.Item("datetime").InnerText)
        '        objEvent.Title = xmlEvent.Item("title").InnerText
        '        objEvent.IconFile = ImportUrl(ModuleID, xmlEvent.Item("icon").InnerText)
        '        objEvent.Every = Integer.Parse(xmlEvent.Item("occursevery").InnerText)
        '        objEvent.AltText = xmlEvent.Item("alttext").InnerText
        '        objEvent.ExpireDate = Date.Parse(xmlEvent.Item("expires").InnerText)
        '        objEvent.MaxWidth = Integer.Parse(xmlEvent.Item("maxWidth").InnerText)
        '        objEvent.Period = xmlEvent.Item("period").InnerText
        '        objEvent.CreatedByUser = UserId.ToString
        '        AddEvent(objEvent)
        '    Next
        'End Sub

        Public Function UpgradeModule(ByVal version As String) As String Implements IUpgradeable.UpgradeModule
            Dim rtnMessage As String = "Events Module Updated: " & version
            Try

                ' Create Lists and Schedule - they should always exist
                CreateListsAndSchedule()

                'Lookup DesktopModuleID
                Dim objDesktopModule As DesktopModuleInfo
                objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", 0)

                If Not objDesktopModule Is Nothing Then
                    Dim objModuleDefinition As ModuleDefinitionInfo
                    'Lookup ModuleDefID
                    objModuleDefinition = ModuleDefinitionController.GetModuleDefinitionByFriendlyName("Events", objDesktopModule.DesktopModuleID)

                    If Not objModuleDefinition Is Nothing Then
                        Dim objModuleControlInfo As ModuleControlInfo
                        'Lookup ModuleControlID
                        objModuleControlInfo = ModuleControlController.GetModuleControlByControlKey("Import", objModuleDefinition.ModuleDefID)
                        If Not objModuleControlInfo Is Nothing Then
                            'Remove Import control key
                            ModuleControlController.DeleteModuleControl(objModuleControlInfo.ModuleControlID)
                        End If
                        ' ReSharper disable RedundantAssignment
                        objModuleControlInfo = Nothing
                        ' ReSharper restore RedundantAssignment
                        objModuleControlInfo = ModuleControlController.GetModuleControlByControlKey("TZUpdate", objModuleDefinition.ModuleDefID)
                        If Not objModuleControlInfo Is Nothing Then
                            'Remove TZUpdate control key
                            ModuleControlController.DeleteModuleControl(objModuleControlInfo.ModuleControlID)
                        End If
                    End If
                End If

                If version = "04.00.02" Then
                    ' Copy moderators from ModuleSettings to ModulePermissions
                    Dim objEventCtl As New EventController
                    objEventCtl.EventsUpgrade(version)
                End If

                If version = "04.01.00" Then
                    ' Upgrade recurring events
                    Dim blAllOk As Boolean
                    blAllOk = UpgradeRecurringEvents()
                    EventsUpgrade(version)
                    If blAllOk Then
                        rtnMessage = "Events Module Updated: " & version + " --> All Events Upgraded"
                    Else
                        rtnMessage = "Events Module Updated: " & version + " --> Not All Events Upgraded - Check database for errors"
                    End If
                End If

                If version = "05.02.00" Then
                    ' ReSharper disable UnusedVariable
                    Dim result As Boolean = ConvertEditPermissions()
                    ' ReSharper restore UnusedVariable
                End If

            Catch ex As Exception
                LogException(ex)

                Return "Events Module Updated - Exception: " & version & " - Message: " & ex.Message.ToString
            End Try
            Return rtnMessage
        End Function

        Public Sub CreateListsAndSchedule()
            ' Create schedule
            Dim objEventNotificationController As New EventNotificationController
            objEventNotificationController.InstallEventSchedule()

            ' Add TimeInterval List entries
            Dim ctlLists As New Lists.ListController
            Dim colThreadStatus As Generic.IEnumerable(Of Lists.ListEntryInfo) = ctlLists.GetListEntryInfoItems("TimeInterval")
            If colThreadStatus.Count = 0 Then
                AddLists()
            End If

        End Sub

        Private Function UpgradeRecurringEvents() As Boolean
            Dim returnStr As Boolean = True

            Dim objPortals As New Entities.Portals.PortalController
            Dim objPortal As Entities.Portals.PortalInfo
            Dim objModules As New ModuleController
            Dim objModule As ModuleInfo

            Dim lstportals As ArrayList = objPortals.GetPortals()
            For Each objPortal In lstportals
                Dim objDesktopModule As DesktopModuleInfo
                objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", objPortal.PortalID)
                Dim folderName As String = objDesktopModule.FolderName
                Dim templateSourceDirectory As String = Common.Globals.ApplicationPath
                Dim localResourceFile As String = templateSourceDirectory & "/DesktopModules/" & folderName & "/" & Localization.LocalResourceDirectory & "/EventSettings.ascx.resx"

                Dim lstModules As ArrayList = objModules.GetModulesByDefinition(objPortal.PortalID, objDesktopModule.FriendlyName)
                For Each objModule In lstModules
                    ' This check for objModule = Nothing because of error in DNN 5.0.0 in GetModulesByDefinition
                    If objModule Is Nothing Then
                        Continue For
                    End If
                    Dim ems As New EventModuleSettings
                    Dim settings As EventModuleSettings = ems.GetEventModuleSettings(objModule.ModuleID, Nothing)

                    Dim maxRecurrences As String = settings.maxrecurrences.ToString

                    If Not UpgradeRecurringEventModule(objModule.ModuleID, CType(settings.RecurDummy, Integer), maxRecurrences, localResourceFile) Then
                        returnStr = False
                    End If
                Next
            Next

            Return returnStr
        End Function

        Public Function UpgradeRecurringEventModule(ByVal moduleID As Integer, ByVal recurMasterID As Integer, ByVal maxRecurrences As String, ByVal localResourceFile As String) As Boolean
            If recurMasterID = 99999 Then
                Return True
            End If
            Dim objCtlEventRecurMaster As New EventRecurMasterController
            Dim objCtlEvent As New EventController
            Dim objEvent, objEventNew As EventInfo

            Dim lstEvents As ArrayList
            lstEvents = objCtlEvent.EventsGetRecurrences(recurMasterID, moduleID)

            For Each objEvent In lstEvents
                Dim objEventRecurMaster As New EventRecurMasterInfo
                With objEventRecurMaster
                    .RecurMasterID = -1
                    .ModuleID = objEvent.ModuleID
                    .PortalID = objEvent.PortalID
                    .DTSTART = objEvent.EventTimeBegin
                    .Duration = CType(objEvent.Duration, String) + "M"
                    ' ReSharper disable VBWarnings::BC40008
                    .Until = objEvent.EventDateEnd
                    ' ReSharper restore VBWarnings::BC40008
                    .EventName = objEvent.EventName
                    .EventDesc = objEvent.EventDesc
                    .Importance = CType(objEvent.Importance, EventRecurMasterInfo.Priority)
                    .Notify = objEvent.Notify
                    .Approved = objEvent.Approved
                    .Signups = objEvent.Signups
                    .MaxEnrollment = objEvent.MaxEnrollment
                    .EnrollRoleID = objEvent.EnrollRoleID
                    .EnrollFee = objEvent.EnrollFee
                    .EnrollType = objEvent.EnrollType
                    .Enrolled = objEvent.Enrolled
                    .PayPalAccount = objEvent.PayPalAccount
                    .DetailPage = objEvent.DetailPage
                    .DetailNewWin = objEvent.DetailNewWin
                    .DetailURL = objEvent.DetailURL
                    .ImageURL = objEvent.ImageURL
                    .ImageType = objEvent.ImageType
                    .ImageWidth = objEvent.ImageWidth
                    .ImageHeight = objEvent.ImageHeight
                    .ImageDisplay = objEvent.ImageDisplay
                    .Location = objEvent.Location
                    .Category = objEvent.Category
                    .Reminder = objEvent.Reminder
                    .SendReminder = objEvent.SendReminder
                    .ReminderTime = objEvent.ReminderTime
                    .ReminderTimeMeasurement = objEvent.ReminderTimeMeasurement
                    .ReminderFrom = objEvent.ReminderFrom
                    .CustomField1 = objEvent.CustomField1
                    .CustomField2 = objEvent.CustomField2
                    .EnrollListView = objEvent.EnrollListView
                    .DisplayEndDate = objEvent.DisplayEndDate
                    .AllDayEvent = objEvent.AllDayEvent
                    .OwnerID = objEvent.OwnerID
                    .CreatedByID = objEvent.CreatedByID
                    .UpdatedByID = objEvent.CreatedByID
                    .AllowAnonEnroll = False
                    .ContentItemID = 0
                    .SocialGroupID = 0
                    .SocialUserID = 0
                    .Summary = Nothing
                    Dim objCtlUsers As New UserController
                    Dim objUserInfo As UserInfo = objCtlUsers.GetUser(objEvent.PortalID, objEvent.CreatedByID)
                    If Not objUserInfo Is Nothing Then
                        .CultureName = objUserInfo.Profile.PreferredLocale
                    End If
                    If objUserInfo Is Nothing Or .CultureName = "" Then
                        Dim objCtlPortal As New Entities.Portals.PortalController
                        Dim objPortalinfo As Entities.Portals.PortalInfo = objCtlPortal.GetPortal(objEvent.PortalID)
                        .CultureName = objPortalinfo.DefaultLanguage
                    End If

                    .RRULE = CreateRRULE(objEvent, .CultureName)
                End With
                Dim lstEventsNew As ArrayList

                lstEventsNew = objCtlEventRecurMaster.CreateEventRecurrences(objEventRecurMaster, objEvent.Duration, maxRecurrences)
                objEventRecurMaster = objCtlEventRecurMaster.EventsRecurMasterSave(objEventRecurMaster)

                ' If no events generated, mark original as cancelled and link to new recurmaster - Non Destructive
                If lstEventsNew.Count = 0 Then
                    objEvent.Cancelled = True
                    objEvent.RecurMasterID = objEventRecurMaster.RecurMasterID
                    objCtlEvent.EventsSave(objEvent, True)
                End If

                Dim i As Integer = 0
                For Each objEventNew In lstEventsNew
                    i = i + 1
                    If i = 1 Then
                        objEventNew.EventID = objEvent.EventID
                    End If
                    objEventNew.RecurMasterID = objEventRecurMaster.RecurMasterID
                    objEventNew.Cancelled = objEvent.Cancelled
                    objEventNew.SearchSubmitted = objEvent.SearchSubmitted
                    ' ReSharper disable RedundantAssignment
                    objEventNew = objCtlEvent.EventsSave(objEventNew, True)
                    ' ReSharper restore RedundantAssignment
                Next
            Next
            lstEvents.Clear()
            lstEvents = objCtlEvent.EventsGetRecurrences(recurMasterID, moduleID)
            If lstEvents.Count = 0 Then
                objCtlEventRecurMaster.EventsRecurMasterDelete(recurMasterID, moduleID)
                Dim ems As New EventModuleSettings
                Dim settings As EventModuleSettings = ems.GetEventModuleSettings(moduleID, localResourceFile)
                settings.RecurDummy = "99999"
                settings.SaveSettings(moduleID)
                Return True
            Else
                Return False
            End If
        End Function

        Private Function CreateRRULE(ByVal objEvent As EventInfo, ByVal cultureName As String) As String
            Dim rrule As String = ""
            Dim strWkst As String
            Dim culture As New CultureInfo(cultureName, False)
            strWkst = "SU"
            If (culture.DateTimeFormat.FirstDayOfWeek <> DayOfWeek.Sunday) Then
                strWkst = "MO"
            End If
            ' ReSharper disable VBWarnings::BC40008
            Select Case Trim(objEvent.RepeatType)
                Case "N"
                    rrule = ""
                Case "P1"
                    Select Case Trim(objEvent.Period)
                        Case "D"
                            rrule = "FREQ=DAILY"
                        Case "W"
                            rrule = "FREQ=WEEKLY;WKST=" + strWkst
                        Case "M"
                            rrule = "FREQ=MONTHLY"
                        Case "Y"
                            rrule = "FREQ=YEARLY"
                    End Select
                    rrule = rrule + ";INTERVAL=" + objEvent.Every.ToString
                Case "W1"
                    rrule = "FREQ=WEEKLY;WKST=" + strWkst + ";INTERVAL=" + objEvent.Every.ToString + ";BYDAY="
                    If CType(Mid(objEvent.Period, 1, 1), Boolean) Then
                        rrule = rrule + "SU,"
                    End If
                    If CType(Mid(objEvent.Period, 2, 1), Boolean) Then
                        rrule = rrule + "MO,"
                    End If
                    If CType(Mid(objEvent.Period, 3, 1), Boolean) Then
                        rrule = rrule + "TU,"
                    End If
                    If CType(Mid(objEvent.Period, 4, 1), Boolean) Then
                        rrule = rrule + "WE,"
                    End If
                    If CType(Mid(objEvent.Period, 5, 1), Boolean) Then
                        rrule = rrule + "TH,"
                    End If
                    If CType(Mid(objEvent.Period, 6, 1), Boolean) Then
                        rrule = rrule + "FR,"
                    End If
                    If CType(Mid(objEvent.Period, 7, 1), Boolean) Then
                        rrule = rrule + "SA,"
                    End If
                    rrule = Left(rrule, Len(rrule) - 1)
                Case "M1"
                    rrule = "FREQ=MONTHLY;INTERVAL=1;BYDAY="
                    Dim strWeek As String
                    If objEvent.Every < 5 Then
                        strWeek = "+" + Convert.ToString(objEvent.Every)
                    Else
                        strWeek = "-1"
                    End If
                    rrule = rrule + strWeek

                    Dim strDay As String = ""
                    Select Case Trim(objEvent.Period)
                        Case "0"
                            strDay = "SU"
                        Case "1"
                            strDay = "MO"
                        Case "2"
                            strDay = "TU"
                        Case "3"
                            strDay = "WE"
                        Case "4"
                            strDay = "TH"
                        Case "5"
                            strDay = "FR"
                        Case "6"
                            strDay = "SA"
                    End Select
                    rrule = rrule + strDay
                Case "M2"
                    rrule = "FREQ=MONTHLY;INTERVAL=" + objEvent.Every.ToString + ";BYMONTHDAY=+" + Trim(objEvent.Period)
                Case "Y1"
                    Dim uiculture As CultureInfo = Threading.Thread.CurrentThread.CurrentCulture
                    Dim usculture As New CultureInfo("en-US", False)
                    Threading.Thread.CurrentThread.CurrentCulture = usculture
                    Dim yearDate As DateTime = Convert.ToDateTime(objEvent.Period)
                    ' ReSharper restore VBWarnings::BC40008
                    Threading.Thread.CurrentThread.CurrentCulture = uiculture
                    rrule = "FREQ=YEARLY;INTERVAL=1;BYMONTH=" + Convert.ToString(yearDate.Month) + ";BYMONTHDAY=+" + Convert.ToString(yearDate.Day)
            End Select
            Return rrule
        End Function

        Private Function ConvertEditPermissions() As Boolean
            Dim returnStr As Boolean = True

            Dim objPortals As New Entities.Portals.PortalController
            Dim objPortal As Entities.Portals.PortalInfo
            Dim objModules As New ModuleController
            Dim objModule As ModuleInfo

            Dim lstportals As ArrayList = objPortals.GetPortals()
            For Each objPortal In lstportals
                Dim objDesktopModule As DesktopModuleInfo
                objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", objPortal.PortalID)

                Dim lstModules As ArrayList = objModules.GetModulesByDefinition(objPortal.PortalID, objDesktopModule.FriendlyName)
                For Each objModule In lstModules
                    ' This check for objModule = Nothing because of error in DNN 5.0.0 in GetModulesByDefinition
                    If objModule Is Nothing Then
                        Continue For
                    End If

                    If Not ConvertEditPermissionsModule(objModule.ModuleID, objModule.TabID) Then
                        returnStr = False
                    End If
                Next
            Next
            Return returnStr
        End Function

        Private Shared Function ConvertEditPermissionsModule(ByVal moduleID As Integer, ByVal tabID As Integer) As Boolean
            Dim returnStr As Boolean
            Dim arrRoles As New ArrayList
            Dim arrUsers As New ArrayList

            Dim objPermission As ModulePermissionInfo
            Dim objPermissionController As New PermissionController

            Dim objModules As New ModuleController
            ' Get existing module permissions
            Dim objModule As ModuleInfo = objModules.GetModule(moduleID, tabID)
            Dim objModulePermissions2 As New ModulePermissionCollection

            For Each perm As ModulePermissionInfo In objModule.ModulePermissions
                If (perm.PermissionKey = "EDIT" And perm.AllowAccess) Then
                    objModulePermissions2.Add(perm)
                    If perm.UserID >= 0 Then
                        arrUsers.Add(perm.UserID)
                    Else
                        arrRoles.Add(perm.RoleID)
                    End If
                End If
            Next
            For Each perm As ModulePermissionInfo In objModulePermissions2
                If perm.RoleName <> "Administrators" Then
                    objModule.ModulePermissions.Remove(perm)
                End If
            Next

            Dim objEditPermissions As ArrayList = objPermissionController.GetPermissionByCodeAndKey("EVENTS_MODULE", "EVENTSEDT")
            Dim objEditPermission As PermissionInfo = CType(objEditPermissions.Item(0), PermissionInfo)

            For Each iRoleID As Integer In arrRoles
                objPermission = New ModulePermissionInfo
                objPermission.RoleID = iRoleID
                objPermission.ModuleID = moduleID
                objPermission.PermissionKey = objEditPermission.PermissionKey
                objPermission.PermissionName = objEditPermission.PermissionName
                objPermission.PermissionCode = objEditPermission.PermissionCode
                objPermission.PermissionID = objEditPermission.PermissionID
                objPermission.AllowAccess = True
                objModule.ModulePermissions.Add(objPermission)
            Next
            For Each iUserID As Integer In arrUsers
                objPermission = New ModulePermissionInfo
                objPermission.UserID = iUserID
                objPermission.ModuleID = moduleID
                objPermission.PermissionKey = objEditPermission.PermissionKey
                objPermission.PermissionName = objEditPermission.PermissionName
                objPermission.PermissionCode = objEditPermission.PermissionCode
                objPermission.PermissionID = objEditPermission.PermissionID
                objPermission.AllowAccess = True
                objModule.ModulePermissions.Add(objPermission)
            Next

            ModulePermissionController.SaveModulePermissions(objModule)
            returnStr = True
            Return returnStr
        End Function
        Private Sub AddLists()
            Dim objDesktopModule As DesktopModuleInfo
            objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", 0)
            If objDesktopModule Is Nothing Then
                Exit Sub
            End If
            Dim objModuleDefinition As ModuleDefinitionInfo
            objModuleDefinition = ModuleDefinitionController.GetModuleDefinitionByFriendlyName("Events", objDesktopModule.DesktopModuleID)
            If objModuleDefinition Is Nothing Then
                Exit Sub
            End If

            Dim moduleDefId As Integer = objModuleDefinition.ModuleDefID

            Dim ctlLists As New Lists.ListController
            'description is missing, not needed

            'ThreadStatus
            Dim objList As New Lists.ListEntryInfo
            objList.ListName = "TimeInterval"
            objList.Value = "5"
            objList.Text = "5"
            objList.SortOrder = 1
            objList.ParentID = 0
            objList.Level = 0
            objList.DefinitionID = moduleDefId
            ctlLists.AddListEntry(objList)

            objList.ListName = "TimeInterval"
            objList.Value = "10"
            objList.Text = "10"
            objList.SortOrder = 2
            objList.ParentID = 0
            objList.Level = 0
            objList.DefinitionID = moduleDefId
            ctlLists.AddListEntry(objList)

            objList.ListName = "TimeInterval"
            objList.Value = "15"
            objList.Text = "15"
            objList.SortOrder = 3
            objList.ParentID = 0
            objList.Level = 0
            objList.DefinitionID = moduleDefId
            ctlLists.AddListEntry(objList)

            objList.ListName = "TimeInterval"
            objList.Value = "20"
            objList.Text = "20"
            objList.SortOrder = 4
            objList.ParentID = 0
            objList.Level = 0
            objList.DefinitionID = moduleDefId
            ctlLists.AddListEntry(objList)

            objList.ListName = "TimeInterval"
            objList.Value = "30"
            objList.Text = "30"
            objList.SortOrder = 5
            objList.ParentID = 0
            objList.Level = 0
            objList.DefinitionID = moduleDefId
            ctlLists.AddListEntry(objList)

            objList.ListName = "TimeInterval"
            objList.Value = "60"
            objList.Text = "60"
            objList.SortOrder = 6
            objList.ParentID = 0
            objList.Level = 0
            objList.DefinitionID = moduleDefId
            ctlLists.AddListEntry(objList)


            objList.ListName = "TimeInterval"
            objList.Value = "120"
            objList.Text = "120"
            objList.SortOrder = 7
            objList.ParentID = 0
            objList.Level = 0
            objList.DefinitionID = moduleDefId
            ctlLists.AddListEntry(objList)


            objList.ListName = "TimeInterval"
            objList.Value = "240"
            objList.Text = "240"
            objList.SortOrder = 8
            objList.ParentID = 0
            objList.Level = 0
            objList.DefinitionID = moduleDefId
            ctlLists.AddListEntry(objList)


            objList.ListName = "TimeInterval"
            objList.Value = "360"
            objList.Text = "360"
            objList.SortOrder = 9
            objList.ParentID = 0
            objList.Level = 0
            objList.DefinitionID = moduleDefId
            ctlLists.AddListEntry(objList)

            objList.ListName = "TimeInterval"
            objList.Value = "720"
            objList.Text = "720"
            objList.SortOrder = 10
            objList.ParentID = 0
            objList.Level = 0
            objList.DefinitionID = moduleDefId
            ctlLists.AddListEntry(objList)

            objList.ListName = "TimeInterval"
            objList.Value = "1440"
            objList.Text = "1440"
            objList.SortOrder = 11
            objList.ParentID = 0
            objList.Level = 0
            objList.DefinitionID = moduleDefId
            ctlLists.AddListEntry(objList)


        End Sub
#End Region

    End Class
#End Region

#Region "EventMasterController Class "
    Public Class EventMasterController

        Public Sub EventsMasterDelete(ByVal masterID As Integer, ByVal moduleID As Integer)
            DataProvider.Instance().EventsMasterDelete(masterID, moduleID)
        End Sub

        Public Function EventsMasterGet(ByVal moduleID As Integer, ByVal subEventID As Integer) As EventMasterInfo
            Dim eventMasterInfo As EventMasterInfo = CType(CBO.FillObject(DataProvider.Instance().EventsMasterGet(moduleID, subEventID), GetType(EventMasterInfo)), EventMasterInfo)
            If Not eventMasterInfo Is Nothing Then
                eventMasterInfo.SubEventTitle = GetModuleTitle(eventMasterInfo.SubEventID)
            End If
            Return eventMasterInfo
        End Function

        Public Function EventsMasterAssignedModules(ByVal moduleID As Integer) As ArrayList
            Dim assignedModules As ArrayList = CBO.FillCollection(DataProvider.Instance().EventsMasterAssignedModules(moduleID), GetType(EventMasterInfo))
            Dim objEventMasterInfo As EventMasterInfo
            For Each objEventMasterInfo In assignedModules
                objEventMasterInfo.SubEventTitle = GetModuleTitle(objEventMasterInfo.SubEventID)
            Next
            assignedModules.Sort(New ModuleListSort)
            Return assignedModules
        End Function

        Public Function EventsMasterAvailableModules(ByVal portalID As Integer, ByVal moduleID As Integer) As ArrayList
            Dim availableModules As ArrayList = CBO.FillCollection(DataProvider.Instance().EventsMasterAvailableModules(portalID, moduleID), GetType(EventMasterInfo))
            Dim objEventMasterInfo As EventMasterInfo
            For Each objEventMasterInfo In availableModules
                objEventMasterInfo.SubEventTitle = GetModuleTitle(objEventMasterInfo.SubEventID)
            Next
            availableModules.Sort(New ModuleListSort)
            Return availableModules
        End Function

        Public Function EventsMasterSave(ByVal objEventMaster As EventMasterInfo) As EventMasterInfo
            Return CType(CBO.FillObject(DataProvider.Instance().EventsMasterSave(objEventMaster.MasterID, objEventMaster.ModuleID, objEventMaster.SubEventID), GetType(EventMasterInfo)), EventMasterInfo)
        End Function

        Public Function GetModuleTitle(ByVal intModuleID As Integer) As String
            Dim cacheKey As String = "EventsModuleTitle" & intModuleID.ToString
            Dim moduleTitle As String = CType(Common.Utilities.DataCache.GetCache(cacheKey), String)
            If moduleTitle Is Nothing Then
                Dim objModuleController As New ModuleController
                Dim objModuleTabs As ArrayList = CType(objModuleController.GetTabModulesByModule(intModuleID), ArrayList)
                Dim objModuleInfo As ModuleInfo
                Dim intTabID As Integer = 0
                For Each objModuleInfo In objModuleTabs
                    If objModuleInfo.TabID < intTabID Or intTabID = 0 Then
                        moduleTitle = Common.Utilities.HtmlUtils.StripTags(objModuleInfo.ModuleTitle, False)
                        intTabID = objModuleInfo.TabID
                    End If
                Next
                Common.Utilities.DataCache.SetCache(cacheKey, moduleTitle)
            End If
            Return moduleTitle
        End Function

    End Class
#End Region

#Region "Comparer Class"
    Public Class ModuleListSort
        Implements IComparer

        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
            Dim xdisplayname As String
            Dim ydisplayname As String

            xdisplayname = CType(x, EventMasterInfo).SubEventTitle
            ydisplayname = CType(y, EventMasterInfo).SubEventTitle
            Dim c As New CaseInsensitiveComparer
            Return c.Compare(xdisplayname, ydisplayname)
        End Function
    End Class

#End Region


#Region "EventSignupsController Class"

    ' EventSignupsController class made public in order to accommodate an external enrollment page.
    Public Class EventSignupsController

        Public Sub EventsSignupsDelete(ByVal signupID As Integer, ByVal moduleID As Integer)
            DataProvider.Instance().EventsSignupsDelete(signupID, moduleID)
        End Sub

        Public Function EventsSignupsGet(ByVal signupID As Integer, ByVal moduleID As Integer, ByVal ppipn As Boolean) As EventSignupsInfo
            Return CType(CBO.FillObject(DataProvider.Instance().EventsSignupsGet(signupID, moduleID, ppipn), GetType(EventSignupsInfo)), EventSignupsInfo)
        End Function

        Public Function EventsSignupsGetEvent(ByVal eventID As Integer, ByVal moduleID As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().EventsSignupsGetEvent(eventID, moduleID), GetType(EventSignupsInfo))
        End Function

        Public Function EventsSignupsGetEventRecurMaster(ByVal recurMasterID As Integer, ByVal moduleID As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().EventsSignupsGetEventRecurMaster(recurMasterID, moduleID), GetType(EventSignupsInfo))
        End Function

        Public Function EventsSignupsMyEnrollments(ByVal moduleID As Integer, ByVal userID As Integer, ByVal socialGroupId As Integer, ByVal categoryIDs As String, ByVal beginDate As DateTime, _
            ByVal endDate As DateTime) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().EventsSignupsMyEnrollments(moduleID, userID, socialGroupId, categoryIDs, beginDate, endDate), GetType(EventSignupsInfo))
        End Function

        Public Function EventsSignupsGetUser(ByVal eventID As Integer, ByVal userID As Integer, ByVal moduleID As Integer) As EventSignupsInfo
            Return CType(CBO.FillObject(DataProvider.Instance().EventsSignupsGetUser(eventID, userID, moduleID), GetType(EventSignupsInfo)), EventSignupsInfo)
        End Function

        Public Function EventsSignupsGetAnonUser(ByVal eventID As Integer, ByVal anonEmail As String, ByVal moduleID As Integer) As EventSignupsInfo
            Return CType(CBO.FillObject(DataProvider.Instance().EventsSignupsGetAnonUser(eventID, anonEmail, moduleID), GetType(EventSignupsInfo)), EventSignupsInfo)
        End Function

        Public Function EventsSignupsSave(ByVal objEventSignup As EventSignupsInfo) As EventSignupsInfo
            Return CType(CBO.FillObject(DataProvider.Instance().EventsSignupsSave(objEventSignup.EventID, objEventSignup.SignupID, objEventSignup.ModuleID, objEventSignup.UserID, objEventSignup.Approved, _
            objEventSignup.PayPalStatus, objEventSignup.PayPalReason, objEventSignup.PayPalTransID, objEventSignup.PayPalPayerID, _
            objEventSignup.PayPalPayerStatus, objEventSignup.PayPalRecieverEmail, objEventSignup.PayPalUserEmail, _
            objEventSignup.PayPalPayerEmail, objEventSignup.PayPalFirstName, objEventSignup.PayPalLastName, objEventSignup.PayPalAddress, _
            objEventSignup.PayPalCity, objEventSignup.PayPalState, objEventSignup.PayPalZip, objEventSignup.PayPalCountry, _
            objEventSignup.PayPalCurrency, objEventSignup.PayPalPaymentDate, objEventSignup.PayPalAmount, objEventSignup.PayPalFee, _
            objEventSignup.NoEnrolees, objEventSignup.AnonEmail, objEventSignup.AnonName, objEventSignup.AnonTelephone, _
            objEventSignup.AnonCulture, objEventSignup.AnonTimeZoneId, objEventSignup.FirstName, objEventSignup.LastName, _
            objEventSignup.Company, objEventSignup.JobTitle, objEventSignup.ReferenceNumber, _
            objEventSignup.Street, objEventSignup.PostalCode, objEventSignup.City, objEventSignup.Region, objEventSignup.Country), _
            GetType(EventSignupsInfo)), EventSignupsInfo)
        End Function

        Public Function EventsModerateSignups(ByVal moduleID As Integer, ByVal socialGroupId As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().EventsModerateSignups(moduleID, socialGroupId), GetType(EventSignupsInfo))
        End Function

        Public Function DisplayEnrollIcon(ByVal eventInfo As EventInfo) As Boolean
            Dim objEventTimeZoneUtilities As New EventTimeZoneUtilities
            If eventInfo.EventTimeBegin < objEventTimeZoneUtilities.ConvertFromUTCToModuleTimeZone(Date.UtcNow, eventInfo.EventTimeZoneId) Then
                Return False
            End If

            If Not eventInfo.Signups Then
                Return False
            End If

            If eventInfo.EnrollRoleID = Nothing Or eventInfo.EnrollRoleID = -1 Then
                Return True
            Else
                If IsEnrollRole(eventInfo.EnrollRoleID, eventInfo.PortalID) Then
                    Return True
                End If
            End If

            Return False
        End Function

        Public Function IsEnrollRole(ByVal roleID As Integer, ByVal portalId As Integer) As Boolean
            Try
                ' ReSharper disable RedundantAssignment
                Dim roleName As String = ""
                ' ReSharper restore RedundantAssignment
                If roleID <> -1 Then
                    Dim enrollRoleInfo As RoleInfo
                    Dim enrollRoleCntrl As New RoleController
                    enrollRoleInfo = enrollRoleCntrl.GetRole(roleID, portalId)
                    roleName = enrollRoleInfo.RoleName
                    Return PortalSecurity.IsInRole(roleName)
                End If
            Catch
            End Try
            Return False
        End Function


    End Class

#End Region

#Region "EventPPErrorLogController Class"

    Public Class EventPpErrorLogController
        Public Function EventsPpErrorLogAdd(ByVal objEventPpErrorLog As EventPPErrorLogInfo) As EventPPErrorLogInfo
            Return CType(CBO.FillObject(DataProvider.Instance().EventsPpErrorLogAdd(objEventPpErrorLog.SignupID, _
            objEventPpErrorLog.PayPalStatus, objEventPpErrorLog.PayPalReason, objEventPpErrorLog.PayPalTransID, objEventPpErrorLog.PayPalPayerID, _
            objEventPpErrorLog.PayPalPayerStatus, objEventPpErrorLog.PayPalRecieverEmail, objEventPpErrorLog.PayPalUserEmail, _
            objEventPpErrorLog.PayPalPayerEmail, objEventPpErrorLog.PayPalFirstName, objEventPpErrorLog.PayPalLastName, objEventPpErrorLog.PayPalAddress, _
            objEventPpErrorLog.PayPalCity, objEventPpErrorLog.PayPalState, objEventPpErrorLog.PayPalZip, objEventPpErrorLog.PayPalCountry, _
            objEventPpErrorLog.PayPalCurrency, objEventPpErrorLog.PayPalPaymentDate, objEventPpErrorLog.PayPalAmount, objEventPpErrorLog.PayPalFee _
            ), GetType(EventPPErrorLogInfo)), EventPPErrorLogInfo)
        End Function

    End Class
#End Region

#Region "EventCategoryController Class"
    Public Class EventCategoryController
        Public Sub EventsCategoryDelete(ByVal category As Integer, ByVal portalID As Integer)
            DataProvider.Instance().EventsCategoryDelete(category, portalID)
        End Sub

        Public Function EventCategoryGet(ByVal category As Integer, ByVal portalID As Integer) As EventCategoryInfo
            Return CType(CBO.FillObject(DataProvider.Instance().EventsCategoryGet(category, portalID), GetType(EventCategoryInfo)), EventCategoryInfo)
        End Function

        Public Function EventCategoryGetByName(ByVal categoryName As String, ByVal portalID As Integer) As EventCategoryInfo
            Return CType(CBO.FillObject(DataProvider.Instance().EventsCategoryGetByName(categoryName, portalID), GetType(EventCategoryInfo)), EventCategoryInfo)
        End Function

        Public Function EventsCategoryList(ByVal portalID As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().EventsCategoryList(portalID), GetType(EventCategoryInfo))
        End Function

        Public Function EventsCategorySave(ByVal objEventCategory As EventCategoryInfo) As EventCategoryInfo
            Return CType(CBO.FillObject(DataProvider.Instance().EventsCategorySave(objEventCategory.PortalID, _
                objEventCategory.Category, objEventCategory.CategoryName, objEventCategory.Color, objEventCategory.FontColor), _
                GetType(EventCategoryInfo)), EventCategoryInfo)
        End Function
    End Class
#End Region

#Region "EventLocationController Class"
    Public Class EventLocationController
        Public Sub EventsLocationDelete(ByVal location As Integer, ByVal portalID As Integer)
            DataProvider.Instance().EventsLocationDelete(location, portalID)
        End Sub

        Public Function EventsLocationGet(ByVal location As Integer, ByVal portalID As Integer) As EventLocationInfo
            Return CType(CBO.FillObject(DataProvider.Instance().EventsLocationGet(location, portalID), GetType(EventLocationInfo)), EventLocationInfo)
        End Function

        Public Function EventsLocationGetByName(ByVal locationName As String, ByVal portalID As Integer) As EventLocationInfo
            Return CType(CBO.FillObject(DataProvider.Instance().EventsLocationGetByName(locationName, portalID), GetType(EventLocationInfo)), EventLocationInfo)
        End Function

        Public Function EventsLocationList(ByVal portalID As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().EventsLocationList(portalID), GetType(EventLocationInfo))
        End Function

        Public Function EventsLocationSave(ByVal objEventLocation As EventLocationInfo) As EventLocationInfo
            Return CType(CBO.FillObject(DataProvider.Instance().EventsLocationSave(objEventLocation.PortalID, _
                objEventLocation.Location, objEventLocation.LocationName, objEventLocation.MapURL, _
                objEventLocation.Street, objEventLocation.PostalCode, objEventLocation.City, _
                objEventLocation.Region, objEventLocation.Country), _
                GetType(EventLocationInfo)), EventLocationInfo)
        End Function
    End Class
#End Region

#Region "EventNotificationController Class"
    Public Class EventNotificationController
        Public Sub EventsNotificationTimeChange(ByVal eventID As Integer, ByVal eventTimeBegin As DateTime, ByVal moduleID As Integer)
            DataProvider.Instance().EventsNotificationTimeChange(eventID, eventTimeBegin, moduleID)
        End Sub

        Public Sub EventsNotificationDelete(ByVal deleteDate As DateTime)
            DataProvider.Instance().EventsNotificationDelete(deleteDate)
        End Sub

        Public Function EventsNotificationGet(ByVal eventID As Integer, ByVal userEmail As String, ByVal moduleID As Integer) As EventNotificationInfo
            Return CType(CBO.FillObject(DataProvider.Instance().EventsNotificationGet(eventID, userEmail, moduleID), GetType(EventNotificationInfo)), EventNotificationInfo)
        End Function

        Public Function EventsNotificationsToSend(ByVal notifyTime As DateTime) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().EventsNotificationsToSend(notifyTime), GetType(EventNotificationInfo))
        End Function

        Public Function EventsNotificationSave(ByVal objEventNotification As EventNotificationInfo) As EventNotificationInfo
            Return CType(CBO.FillObject(DataProvider.Instance().EventsNotificationSave(objEventNotification.NotificationID, objEventNotification.EventID, objEventNotification.PortalAliasID, objEventNotification.UserEmail, objEventNotification.NotificationSent, objEventNotification.NotifyByDateTime, objEventNotification.EventTimeBegin, objEventNotification.NotifyLanguage, objEventNotification.ModuleID, objEventNotification.TabID), _
                GetType(EventNotificationInfo)), EventNotificationInfo)
        End Function

        Public Function NotifyInfo(ByVal itemID As Integer, ByVal email As String, ByVal moduleID As Integer, ByVal localResourceFile As String, ByVal displayTimeZoneId As String) As String
            Dim notiinfo As String = ""
            Dim objEventNotification As EventNotificationInfo
            objEventNotification = EventsNotificationGet(itemID, email, moduleID)
            If Not objEventNotification Is Nothing Then
                Dim objEventTimeZoneUtilities As New EventTimeZoneUtilities
                Dim notifyDisplay As DateTime = objEventTimeZoneUtilities.ConvertFromUTCToDisplayTimeZone(objEventNotification.NotifyByDateTime, displayTimeZoneId).EventDate
                notiinfo = String.Format(Localization.GetString("lblReminderConfirmation", localResourceFile), notifyDisplay.ToString)
            End If
            Return notiinfo
        End Function

        ' Creates Event Schedule (if Not already installed)
        Public Sub InstallEventSchedule()
            Dim objScheduleItem As Services.Scheduling.ScheduleItem
            objScheduleItem = Services.Scheduling.SchedulingProvider.Instance().GetSchedule("DotNetNuke.Modules.Events.EventNotification, DotNetNuke.Modules.Events", Nothing)
            If objScheduleItem Is Nothing Then
                objScheduleItem = New Services.Scheduling.ScheduleItem
                objScheduleItem.TypeFullName = "DotNetNuke.Modules.Events.EventNotification, DotNetNuke.Modules.Events"
                objScheduleItem.TimeLapse = 1
                objScheduleItem.TimeLapseMeasurement = "h"
                objScheduleItem.RetryTimeLapse = 30
                objScheduleItem.RetryTimeLapseMeasurement = "m"
                objScheduleItem.RetainHistoryNum = 10
                objScheduleItem.Enabled = True
                objScheduleItem.ObjectDependencies = ""
                objScheduleItem.FriendlyName = "DNN Events"
                Services.Scheduling.SchedulingProvider.Instance().AddSchedule(objScheduleItem)
            Else
                objScheduleItem.FriendlyName = "DNN Events"
                Services.Scheduling.SchedulingProvider.Instance().UpdateSchedule(objScheduleItem)
            End If
        End Sub

    End Class
#End Region



#Region "EventRecurMasterController Class "
    Public Class EventRecurMasterController
        Public Sub EventsRecurMasterDelete(ByVal recurMasterID As Integer, ByVal moduleID As Integer)

            ' Delete ContentItems for the Events
            Dim objCtlEvent As New EventController
            Dim lstEvents As ArrayList = objCtlEvent.EventsGetRecurrences(recurMasterID, moduleID)
            ' Dim cntTaxonomy As New Content
            Dim cntJournal As New Journal
            For Each objEvent As EventInfo In lstEvents
                ' cntTaxonomy.DeleteContentItem(objEvent.ContentItemID)
                If objEvent.JournalItem Then
                    cntJournal.DeleteEvent(objEvent)
                End If
            Next

            ' Delete ContentItem for recurmaster
            ' Dim objCtlEventRecurMaster As New EventRecurMasterController
            ' Dim objEventRecurMaster As EventRecurMasterInfo = objCtlEventRecurMaster.EventsRecurMasterGet(RecurMasterID, ModuleID)
            ' If Not objEventRecurMaster Is Nothing Then
            ' cntTaxonomy.DeleteContentItem(objEventRecurMaster.ContentItemID)
            ' End If

            ' Delete recurmaster
            DataProvider.Instance().EventsRecurMasterDelete(recurMasterID, moduleID)
        End Sub

        Public Function EventsRecurMasterGet(ByVal recurMasterID As Integer, ByVal moduleID As Integer) As EventRecurMasterInfo
            Return CType(CBO.FillObject(DataProvider.Instance().EventsRecurMasterGet(recurMasterID, moduleID), GetType(EventRecurMasterInfo)), EventRecurMasterInfo)
        End Function

        Public Function EventsRecurMasterSave(ByVal objEventRecurMaster As EventRecurMasterInfo, ByVal tabId As Integer, ByVal updateContent As Boolean) As EventRecurMasterInfo
            ' Dim cntTaxonomy As New Content
            ' If UpdateContent Then
            ' If Not objEventRecurMaster.ContentItemID = Nothing And objEventRecurMaster.ContentItemID <> 0 Then
            ' cntTaxonomy.UpdateContentEventRecurMaster(objEventRecurMaster)
            ' End If
            ' End If

            Dim objEventRecurMasterOut As EventRecurMasterInfo = EventsRecurMasterSave(objEventRecurMaster)

            ' If UpdateContent And objEventRecurMaster.RecurMasterID = -1 Then
            ' If objEventRecurMasterOut.ContentItemID = 0 And Not TabId = Nothing Then
            ' objEventRecurMasterOut.ContentItemID = cntTaxonomy.CreateContentEventRecurMaster(objEventRecurMasterOut, TabId).ContentItemId
            ' EventsRecurMasterSave(objEventRecurMasterOut)
            ' End If
            ' End If

            Return objEventRecurMasterOut

        End Function


        Public Function EventsRecurMasterSave(ByVal objEventRecurMaster As EventRecurMasterInfo) As EventRecurMasterInfo
            Return CType(CBO.FillObject(DataProvider.Instance().EventsRecurMasterSave(objEventRecurMaster.RecurMasterID, _
                    objEventRecurMaster.ModuleID, objEventRecurMaster.PortalID, objEventRecurMaster.RRULE, objEventRecurMaster.DTSTART, objEventRecurMaster.Duration, _
                    objEventRecurMaster.Until, objEventRecurMaster.EventName, objEventRecurMaster.EventDesc, objEventRecurMaster.Importance, _
                    objEventRecurMaster.Notify, objEventRecurMaster.Approved, objEventRecurMaster.Signups, objEventRecurMaster.MaxEnrollment, _
                    objEventRecurMaster.EnrollRoleID, objEventRecurMaster.EnrollFee, objEventRecurMaster.EnrollType, objEventRecurMaster.PayPalAccount, _
                    objEventRecurMaster.DetailPage, objEventRecurMaster.DetailNewWin, objEventRecurMaster.DetailURL, objEventRecurMaster.ImageURL, objEventRecurMaster.ImageType, objEventRecurMaster.ImageWidth, _
                    objEventRecurMaster.ImageHeight, objEventRecurMaster.ImageDisplay, objEventRecurMaster.Location, objEventRecurMaster.Category, _
                    objEventRecurMaster.Reminder, objEventRecurMaster.SendReminder, objEventRecurMaster.ReminderTime, _
                    objEventRecurMaster.ReminderTimeMeasurement, objEventRecurMaster.ReminderFrom, objEventRecurMaster.CustomField1, _
                    objEventRecurMaster.CustomField2, objEventRecurMaster.EnrollListView, objEventRecurMaster.DisplayEndDate, _
                    objEventRecurMaster.AllDayEvent, objEventRecurMaster.CultureName, objEventRecurMaster.OwnerID, objEventRecurMaster.CreatedByID, _
                    objEventRecurMaster.UpdatedByID, objEventRecurMaster.EventTimeZoneId, objEventRecurMaster.AllowAnonEnroll, objEventRecurMaster.ContentItemID, _
                    objEventRecurMaster.SocialGroupID, objEventRecurMaster.SocialUserID, objEventRecurMaster.Summary), GetType(EventRecurMasterInfo)), EventRecurMasterInfo)
        End Function

        Public Function EventsRecurMasterModerate(ByVal moduleID As Integer, ByVal socialGroupId As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().EventsRecurMasterModerate(moduleID, socialGroupId), GetType(EventRecurMasterInfo))
        End Function

        Public Function DecomposeRRULE(ByVal strRRULE As String, ByVal dtStart As DateTime) As EventRRULEInfo
            Dim objEventRRULE As New EventRRULEInfo
            With objEventRRULE
                Dim intEqual, intKeyEnd As Integer
                Dim strKey, strValue As String
                strRRULE = strRRULE + ";"
                .FreqBasic = False
                While Len(strRRULE) > 1
                    intEqual = InStr(strRRULE, "=")
                    intKeyEnd = InStr(strRRULE, ";")
                    strKey = Left(strRRULE, intEqual - 1)
                    strValue = Mid(strRRULE, intEqual + 1, intKeyEnd - intEqual - 1)
                    Select Case strKey
                        Case "FREQ"
                            .Freq = strValue
                        Case "WKST"
                            .WKST = strValue
                        Case "INTERVAL"
                            .Interval = CInt(strValue)
                        Case "BYDAY"
                            .ByDay = strValue
                            If IsNumeric(Left(strValue, 2)) Then
                                If InStr(strValue, "SU") > 0 Then
                                    .Su = True
                                    .SuNo = CInt(Left(strValue, 2))
                                End If
                                If InStr(strValue, "MO") > 0 Then
                                    .Mo = True
                                    .MoNo = CInt(Left(strValue, 2))
                                End If
                                If InStr(strValue, "TU") > 0 Then
                                    .Tu = True
                                    .TuNo = CInt(Left(strValue, 2))
                                End If
                                If InStr(strValue, "WE") > 0 Then
                                    .We = True
                                    .WeNo = CInt(Left(strValue, 2))
                                End If
                                If InStr(strValue, "TH") > 0 Then
                                    .Th = True
                                    .ThNo = CInt(Left(strValue, 2))
                                End If
                                If InStr(strValue, "FR") > 0 Then
                                    .Fr = True
                                    .FrNo = CInt(Left(strValue, 2))
                                End If
                                If InStr(strValue, "SA") > 0 Then
                                    .Sa = True
                                    .SaNo = CInt(Left(strValue, 2))
                                End If
                            Else
                                If InStr(strValue, "SU") > 0 Then
                                    .Su = True
                                    .SuNo = Nothing
                                End If
                                If InStr(strValue, "MO") > 0 Then
                                    .Mo = True
                                    .MoNo = Nothing
                                End If
                                If InStr(strValue, "TU") > 0 Then
                                    .Tu = True
                                    .TuNo = Nothing
                                End If
                                If InStr(strValue, "WE") > 0 Then
                                    .We = True
                                    .WeNo = Nothing
                                End If
                                If InStr(strValue, "TH") > 0 Then
                                    .Th = True
                                    .ThNo = Nothing
                                End If
                                If InStr(strValue, "FR") > 0 Then
                                    .Fr = True
                                    .FrNo = Nothing
                                End If
                                If InStr(strValue, "SA") > 0 Then
                                    .Sa = True
                                    .SaNo = Nothing
                                End If
                            End If
                        Case "BYMONTH"
                            .ByMonth = CInt(strValue)
                        Case "BYMONTHDAY"
                            .ByMonthDay = CInt(strValue)
                    End Select
                    strRRULE = Mid(strRRULE, intKeyEnd + 1)
                End While
                Select Case .Freq
                    Case "YEARLY"
                        If .ByMonth = 0 And .ByMonthDay = 0 Then
                            .FreqBasic = True
                        End If
                        If .ByMonth = 0 Then
                            .ByMonth = dtStart.Month
                        End If
                        If .ByMonthDay = 0 Then
                            .ByMonthDay = dtStart.Day
                        End If
                    Case "MONTHLY"
                        If .ByMonthDay = 0 And IsNothing(.ByDay) Then
                            .ByMonthDay = dtStart.Day
                            .FreqBasic = True
                        End If
                    Case "WEEKLY"
                        If IsNothing(.ByDay) Then
                            .FreqBasic = True
                            Dim dtdow As DayOfWeek = dtStart.DayOfWeek
                            Select Case dtdow
                                Case DayOfWeek.Sunday
                                    .Su = True
                                    .SuNo = Nothing
                                    .ByDay = "SU"
                                Case DayOfWeek.Monday
                                    .Mo = True
                                    .MoNo = Nothing
                                    .ByDay = "MO"
                                Case DayOfWeek.Tuesday
                                    .Tu = True
                                    .TuNo = Nothing
                                    .ByDay = "TU"
                                Case DayOfWeek.Wednesday
                                    .We = True
                                    .WeNo = Nothing
                                    .ByDay = "WE"
                                Case DayOfWeek.Thursday
                                    .Th = True
                                    .ThNo = Nothing
                                    .ByDay = "TH"
                                Case DayOfWeek.Friday
                                    .Fr = True
                                    .FrNo = Nothing
                                    .ByDay = "FR"
                                Case DayOfWeek.Saturday
                                    .Sa = True
                                    .SaNo = Nothing
                                    .ByDay = "SA"
                            End Select
                        End If
                End Select
            End With
            Return objEventRRULE
        End Function

        Public Function CreateEventRecurrences(ByVal objEventRecurMaster As EventRecurMasterInfo, ByVal intDuration As Integer, ByVal maxRecurrences As String) As ArrayList
            Dim dtStart As DateTime = objEventRecurMaster.DTSTART
            Dim objCtlEventRecurMaster As New EventRecurMasterController
            Dim objEventRRULE As EventRRULEInfo = objCtlEventRecurMaster.DecomposeRRULE(objEventRecurMaster.RRULE, objEventRecurMaster.DTSTART)
            Dim lstEvents As New ArrayList
            Dim nextDate As DateTime = dtStart.Date
            Dim blAddDate As Boolean = True
            While nextDate <= objEventRecurMaster.Until
                If objEventRRULE.ByMonth <> 0 And nextDate.Month <> objEventRRULE.ByMonth Then
                    blAddDate = False
                End If
                If objEventRRULE.ByMonthDay <> 0 And nextDate.Day <> objEventRRULE.ByMonthDay Then
                    blAddDate = False
                End If
                If Not IsNothing(objEventRRULE.ByDay) Then
                    Dim dtdow As DayOfWeek = nextDate.DayOfWeek
                    Select Case dtdow
                        Case DayOfWeek.Sunday
                            blAddDate = CheckWeekday(nextDate, objEventRRULE.Su, objEventRRULE.SuNo, blAddDate)
                        Case DayOfWeek.Monday
                            blAddDate = CheckWeekday(nextDate, objEventRRULE.Mo, objEventRRULE.MoNo, blAddDate)
                        Case DayOfWeek.Tuesday
                            blAddDate = CheckWeekday(nextDate, objEventRRULE.Tu, objEventRRULE.TuNo, blAddDate)
                        Case DayOfWeek.Wednesday
                            blAddDate = CheckWeekday(nextDate, objEventRRULE.We, objEventRRULE.WeNo, blAddDate)
                        Case DayOfWeek.Thursday
                            blAddDate = CheckWeekday(nextDate, objEventRRULE.Th, objEventRRULE.ThNo, blAddDate)
                        Case DayOfWeek.Friday
                            blAddDate = CheckWeekday(nextDate, objEventRRULE.Fr, objEventRRULE.FrNo, blAddDate)
                        Case DayOfWeek.Saturday
                            blAddDate = CheckWeekday(nextDate, objEventRRULE.Sa, objEventRRULE.SaNo, blAddDate)
                    End Select
                End If

                If blAddDate = True Then
                    Select Case objEventRRULE.Freq
                        Case "YEARLY"
                            Dim intYear As Integer = CInt(DateDiff(DateInterval.Year, dtStart.Date, nextDate))
                            ' ReSharper disable CompareOfFloatsByEqualityOperator
                            If intYear / objEventRRULE.Interval <> CInt(intYear / objEventRRULE.Interval) Then
                                ' ReSharper restore CompareOfFloatsByEqualityOperator
                                blAddDate = False
                            End If
                        Case "MONTHLY"
                            Dim intMonth As Integer = CInt(DateDiff(DateInterval.Month, dtStart.Date, nextDate))
                            ' ReSharper disable CompareOfFloatsByEqualityOperator
                            If intMonth / objEventRRULE.Interval <> CInt(intMonth / objEventRRULE.Interval) Then
                                ' ReSharper restore CompareOfFloatsByEqualityOperator
                                blAddDate = False
                            End If
                        Case "WEEKLY"
                            Dim fdow As Microsoft.VisualBasic.FirstDayOfWeek
                            If objEventRRULE.WKST = "SU" Then
                                fdow = Microsoft.VisualBasic.FirstDayOfWeek.Sunday
                            Else
                                fdow = Microsoft.VisualBasic.FirstDayOfWeek.Monday
                            End If
                            Dim intWeek As Integer = CInt(DateDiff(DateInterval.WeekOfYear, dtStart.Date, nextDate, fdow))
                            ' ReSharper disable CompareOfFloatsByEqualityOperator
                            If intWeek / objEventRRULE.Interval <> CInt(intWeek / objEventRRULE.Interval) Then
                                ' ReSharper restore CompareOfFloatsByEqualityOperator
                                blAddDate = False
                            End If
                        Case "DAILY"
                            Dim intDay As Integer = CInt(DateDiff(DateInterval.Day, dtStart.Date, nextDate))
                            ' ReSharper disable CompareOfFloatsByEqualityOperator
                            If intDay / objEventRRULE.Interval <> CInt(intDay / objEventRRULE.Interval) Then
                                ' ReSharper restore CompareOfFloatsByEqualityOperator
                                blAddDate = False
                            End If
                    End Select
                End If
                If blAddDate = True Then
                    Dim objEvent As New EventInfo
                    objEvent.EventTimeBegin = nextDate.Date + dtStart.TimeOfDay
                    objEvent.Duration = intDuration
                    objEvent.OriginalDateBegin = nextDate.Date
                    With objEventRecurMaster
                        objEvent.EventID = -1
                        objEvent.RecurMasterID = .RecurMasterID
                        objEvent.ModuleID = .ModuleID
                        objEvent.PortalID = .PortalID
                        objEvent.EventName = .EventName
                        objEvent.EventDesc = .EventDesc
                        objEvent.Importance = CType(.Importance, EventInfo.Priority)
                        objEvent.Notify = .Notify
                        objEvent.Approved = .Approved
                        objEvent.Signups = .Signups
                        objEvent.AllowAnonEnroll = .AllowAnonEnroll
                        objEvent.JournalItem = .JournalItem
                        objEvent.MaxEnrollment = .MaxEnrollment
                        objEvent.EnrollRoleID = .EnrollRoleID
                        objEvent.EnrollFee = .EnrollFee
                        objEvent.EnrollType = .EnrollType
                        objEvent.PayPalAccount = .PayPalAccount
                        objEvent.DetailPage = .DetailPage
                        objEvent.DetailNewWin = .DetailNewWin
                        objEvent.DetailURL = .DetailURL
                        objEvent.ImageURL = .ImageURL
                        objEvent.ImageType = .ImageType
                        objEvent.ImageWidth = .ImageWidth
                        objEvent.ImageHeight = .ImageHeight
                        objEvent.ImageDisplay = .ImageDisplay
                        objEvent.Location = .Location
                        objEvent.Category = .Category
                        objEvent.Reminder = .Reminder
                        objEvent.SendReminder = .SendReminder
                        objEvent.ReminderTime = .ReminderTime
                        objEvent.ReminderTimeMeasurement = .ReminderTimeMeasurement
                        objEvent.ReminderFrom = .ReminderFrom
                        objEvent.CustomField1 = .CustomField1
                        objEvent.CustomField2 = .CustomField2
                        objEvent.EnrollListView = .EnrollListView
                        objEvent.DisplayEndDate = .DisplayEndDate
                        objEvent.AllDayEvent = .AllDayEvent
                        objEvent.OwnerID = .OwnerID
                        objEvent.CreatedByID = .CreatedByID
                        objEvent.LastUpdatedID = .UpdatedByID
                        objEvent.Cancelled = False
                        objEvent.SearchSubmitted = False
                        objEvent.NewEventEmailSent = objEvent.Approved
                        objEvent.SocialGroupId = .SocialGroupID
                        objEvent.SocialUserId = .SocialUserID
                        objEvent.Summary = .Summary
                        objEvent.UpdateStatus = "Add"
                    End With
                    lstEvents.Add(objEvent)
                    If maxRecurrences <> "" Then
                        If lstEvents.Count = CInt(maxRecurrences) Then
                            objEventRecurMaster.Until = objEvent.EventTimeBegin.Date
                            Exit While
                        End If
                    End If
                End If
                nextDate = DateAdd(DateInterval.Day, 1, nextDate)
                blAddDate = True
            End While
            Return lstEvents
        End Function

        Private Function CheckWeekday(ByVal dtDate As DateTime, ByVal blDay As Boolean, ByVal intDayNo As Integer, ByVal blAddDate As Boolean) As Boolean
            If blAddDate = False Then
                Return False
            End If

            If Not blDay Then
                Return False
            End If

            If IsNothing(intDayNo) Or intDayNo = 0 Then
                Return True
            End If

            If intDayNo > 0 Then
                ' ReSharper disable CompareOfFloatsByEqualityOperator
                If Int((dtDate.Day - 1) / 7) = intDayNo - 1 Then
                    ' ReSharper restore CompareOfFloatsByEqualityOperator
                    Return True
                End If
            Else
                Dim intDaysInMonth As Integer
                intDaysInMonth = DateTime.DaysInMonth(dtDate.Year, dtDate.Month)
                ' ReSharper disable CompareOfFloatsByEqualityOperator
                If Int((intDaysInMonth - dtDate.Day) / 7) = (intDayNo * -1) - 1 Then
                    ' ReSharper restore CompareOfFloatsByEqualityOperator
                    Return True
                End If
            End If

            Return False

        End Function

        Public Function RepeatType(ByVal objEventRRULE As EventRRULEInfo) As String
            Dim strRepeatType As String = "N"
            Select Case objEventRRULE.Freq
                Case "DAILY"
                    strRepeatType = "P1"
                Case "WEEKLY"
                    If objEventRRULE.FreqBasic Then
                        strRepeatType = "P1"
                    Else
                        strRepeatType = "W1"
                    End If
                Case "MONTHLY"
                    If objEventRRULE.FreqBasic Then
                        strRepeatType = "P1"
                    ElseIf Not IsNothing(objEventRRULE.ByDay) Then
                        strRepeatType = "M1"
                    Else
                        strRepeatType = "M2"
                    End If
                Case "YEARLY"
                    If objEventRRULE.FreqBasic Then
                        strRepeatType = "P1"
                    Else
                        strRepeatType = "Y1"
                    End If
            End Select
            Return strRepeatType
        End Function

        Public Function RecurrenceText(ByVal objEventRRULE As EventRRULEInfo, ByVal localResourceFile As String, ByVal culture As CultureInfo, ByVal eventDateBegin As DateTime) As String
            Dim lblEventText As String = ""
            Dim strRepeatType As String = RepeatType(objEventRRULE)
            Select Case strRepeatType
                Case "N"
                    lblEventText = Localization.GetString("OneTimeEvent", localResourceFile)
                Case "P1"
                    Dim txtEvent As String = ""
                    Select Case Trim(Left(objEventRRULE.Freq, 1))
                        Case "D"
                            txtEvent = Localization.GetString("EveryXDays", localResourceFile)
                        Case "W"
                            txtEvent = Localization.GetString("EveryXWeeks", localResourceFile)
                        Case "M"
                            txtEvent = Localization.GetString("EveryXMonths", localResourceFile)
                        Case "Y"
                            txtEvent = Localization.GetString("EveryXYears", localResourceFile)
                    End Select
                    lblEventText = String.Format(txtEvent, objEventRRULE.Interval.ToString)
                Case "W1"
                    Dim txtdays As String = ""
                    If objEventRRULE.Su Then
                        txtdays += culture.DateTimeFormat.DayNames(DayOfWeek.Sunday)
                    End If
                    If objEventRRULE.Mo Then
                        If Trim(txtdays).Length > 0 Then
                            txtdays += ", "
                        End If
                        txtdays += culture.DateTimeFormat.DayNames(DayOfWeek.Monday)
                    End If
                    If objEventRRULE.Tu Then
                        If Trim(txtdays).Length > 0 Then
                            txtdays += ", "
                        End If
                        txtdays += culture.DateTimeFormat.DayNames(DayOfWeek.Tuesday)
                    End If
                    If objEventRRULE.We Then
                        If Trim(txtdays).Length > 0 Then
                            txtdays += ", "
                        End If
                        txtdays += culture.DateTimeFormat.DayNames(DayOfWeek.Wednesday)
                    End If
                    If objEventRRULE.Th Then
                        If Trim(txtdays).Length > 0 Then
                            txtdays += ", "
                        End If
                        txtdays += culture.DateTimeFormat.DayNames(DayOfWeek.Thursday)
                    End If
                    If objEventRRULE.Fr Then
                        If Trim(txtdays).Length > 0 Then
                            txtdays += ", "
                        End If
                        txtdays += culture.DateTimeFormat.DayNames(DayOfWeek.Friday)
                    End If
                    If objEventRRULE.Sa Then
                        If Trim(txtdays).Length > 0 Then
                            txtdays += ", "
                        End If
                        txtdays += culture.DateTimeFormat.DayNames(DayOfWeek.Saturday)
                    End If
                    lblEventText = String.Format(Localization.GetString("RecurringWeeksOn", localResourceFile), objEventRRULE.Interval.ToString, txtdays)
                Case "M1"
                    Dim txtDay As String = ""
                    Dim txtWeek As String = ""
                    Dim intEvery As Integer
                    If objEventRRULE.Su Then
                        txtDay = culture.DateTimeFormat.DayNames(DayOfWeek.Sunday)
                        intEvery = objEventRRULE.SuNo
                    End If
                    If objEventRRULE.Mo Then
                        txtDay = culture.DateTimeFormat.DayNames(DayOfWeek.Monday)
                        intEvery = objEventRRULE.MoNo()
                    End If
                    If objEventRRULE.Tu Then
                        txtDay = culture.DateTimeFormat.DayNames(DayOfWeek.Tuesday)
                        intEvery = objEventRRULE.TuNo
                    End If
                    If objEventRRULE.We Then
                        txtDay = culture.DateTimeFormat.DayNames(DayOfWeek.Wednesday)
                        intEvery = objEventRRULE.WeNo
                    End If
                    If objEventRRULE.Th Then
                        txtDay = culture.DateTimeFormat.DayNames(DayOfWeek.Thursday)
                        intEvery = objEventRRULE.ThNo
                    End If
                    If objEventRRULE.Fr Then
                        txtDay = culture.DateTimeFormat.DayNames(DayOfWeek.Friday)
                        intEvery = objEventRRULE.FrNo
                    End If
                    If objEventRRULE.Sa Then
                        txtDay = culture.DateTimeFormat.DayNames(DayOfWeek.Saturday)
                        intEvery = objEventRRULE.SaNo
                    End If
                    Select Case Trim(CType(intEvery, String))
                        Case "1"
                            txtWeek = Localization.GetString("First", localResourceFile)
                        Case "2"
                            txtWeek = Localization.GetString("Second", localResourceFile)
                        Case "3"
                            txtWeek = Localization.GetString("Third", localResourceFile)
                        Case "4"
                            txtWeek = Localization.GetString("Fourth", localResourceFile)
                        Case "-1"
                            txtWeek = Localization.GetString("Last", localResourceFile)
                    End Select
                    If objEventRRULE.Interval = 1 Then
                        lblEventText = String.Format(Localization.GetString("RecurringInMonth", localResourceFile), txtWeek, txtDay)
                    Else
                        lblEventText = String.Format(Localization.GetString("RecurringEveryMonth2", localResourceFile), Trim(objEventRRULE.Interval.ToString), txtWeek, txtDay)
                    End If
                Case "M2"
                    lblEventText = String.Format(Localization.GetString("RecurringEveryMonth", localResourceFile), Trim(objEventRRULE.Interval.ToString), Localization.GetString(CType(objEventRRULE.ByMonthDay, String), localResourceFile))
                Case "Y1"
                    lblEventText = String.Format(Localization.GetString("RecurringYearsOn", localResourceFile), eventDateBegin)

            End Select
            Return lblEventText
        End Function

        Public Function RecurrenceInfo(ByVal objEvent As EventInfo, ByVal localResourceFile As String) As String
            Dim recinfo As String
            recinfo = String.Format(Localization.GetString("RecurringUntil", localResourceFile), objEvent.LastRecurrence.ToString("d"), objEvent.NoOfRecurrences.ToString)
            Return recinfo
        End Function

    End Class
#End Region

#Region "EventSubscriptionController Class"
    Public Class EventSubscriptionController
        Public Sub EventsSubscriptionDeleteUser(ByVal userID As Integer, ByVal moduleID As Integer)
            DataProvider.Instance().EventsSubscriptionDeleteUser(userID, moduleID)
        End Sub

        Public Function EventsSubscriptionGetUser(ByVal userID As Integer, ByVal moduleID As Integer) As EventSubscriptionInfo
            Return CType(CBO.FillObject(DataProvider.Instance().EventsSubscriptionGetUser(userID, moduleID), GetType(EventSubscriptionInfo)), EventSubscriptionInfo)
        End Function

        Public Function EventsSubscriptionGetModule(ByVal moduleID As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().EventsSubscriptionGetModule(moduleID), GetType(EventSubscriptionInfo))
        End Function

        Public Function EventsSubscriptionGetSubModule(ByVal moduleID As Integer) As ArrayList
            Return CBO.FillCollection(DataProvider.Instance().EventsSubscriptionGetSubModule(moduleID), GetType(EventSubscriptionInfo))
        End Function

        Public Function EventsSubscriptionSave(ByVal objEventSubscription As EventSubscriptionInfo) As EventSubscriptionInfo
            Return CType(CBO.FillObject(DataProvider.Instance().EventsSubscriptionSave(objEventSubscription.SubscriptionID, _
                objEventSubscription.ModuleID, objEventSubscription.PortalID, objEventSubscription.UserID), _
                GetType(EventSubscriptionInfo)), EventSubscriptionInfo)
        End Function
    End Class
#End Region

#Region "EventEmails Class"
    Public Class EventEmails
#Region "Member Variables"
        Private _localResourceFile As String
        Private _cultureName As String
        Private _moduleId As Int32
        Private _portalId As Int32
        Private _currculture As CultureInfo
#End Region

#Region "Constructor"
        Sub New(ByVal portalID As Integer, ByVal moduleID As Integer, ByVal localResourceFile As String)
            Me.PortalID = portalID
            Me.ModuleID = moduleID
            Me.LocalResourceFile = localResourceFile
        End Sub
        Sub New(ByVal portalID As Integer, ByVal moduleID As Integer, ByVal localResourceFile As String, ByVal cultureName As String)
            Me.PortalID = portalID
            Me.ModuleID = moduleID
            Me.LocalResourceFile = localResourceFile
            Me.CultureName = cultureName
        End Sub
#End Region

#Region "Properties"
        Private Property PortalID() As Integer
            Get
                Return _portalId
            End Get
            Set(ByVal value As Integer)
                _portalId = value
            End Set
        End Property

        Private Property ModuleID() As Integer
            Get
                Return _moduleId
            End Get
            Set(ByVal value As Integer)
                _moduleId = value
            End Set
        End Property

        Private Overloads Property LocalResourceFile() As String
            Get
                Return _localResourceFile
            End Get
            Set(ByVal value As String)
                _localResourceFile = value
            End Set
        End Property

        Private Property CultureName() As String
            Get
                Return _cultureName
            End Get
            Set(ByVal value As String)
                _cultureName = value
            End Set
        End Property

#End Region


#Region "Methods"
        Public Sub SendEmails(ByVal objEventEmailInfo As EventEmailInfo, ByVal objEvent As EventInfo)
            SendEmails(objEventEmailInfo, objEvent, Nothing, Nothing, Nothing, True)
        End Sub

        Public Sub SendEmails(ByVal objEventEmailInfo As EventEmailInfo, ByVal objEvent As EventInfo, ByVal objEventSignups As EventSignupsInfo)
            SendEmails(objEventEmailInfo, objEvent, objEventSignups, Nothing, Nothing, True)
        End Sub

        Public Sub SendEmails(ByVal objEventEmailInfo As EventEmailInfo, ByVal objEvent As EventInfo, ByVal objEventSignups As EventSignupsInfo, ByVal blTokenReplace As Boolean)
            SendEmails(objEventEmailInfo, objEvent, objEventSignups, Nothing, Nothing, blTokenReplace)
        End Sub

        Public Sub SendEmails(ByVal objEventEmailInfo As EventEmailInfo, ByVal objEvent As EventInfo, ByVal attachments As Generic.List(Of Net.Mail.Attachment))
            SendEmails(objEventEmailInfo, objEvent, Nothing, attachments, Nothing, True)
        End Sub

        Public Sub SendEmails(ByVal objEventEmailInfo As EventEmailInfo, ByVal objEvent As EventInfo, ByVal domainurl As String)
            SendEmails(objEventEmailInfo, objEvent, Nothing, Nothing, domainurl, True)
        End Sub

        Public Sub SendEmails(ByVal objEventEmailInfo As EventEmailInfo, ByVal objEvent As EventInfo, ByVal objEventSignups As EventSignupsInfo, ByVal attachments As Generic.List(Of Net.Mail.Attachment), ByVal domainurl As String, ByVal blTokenReplace As Boolean)
            _currculture = Threading.Thread.CurrentThread.CurrentCulture

            With objEventEmailInfo
                Dim userEmail As String
                Dim itemNo As Integer = 0

                Dim ems As New EventModuleSettings
                Dim settings As EventModuleSettings = ems.GetEventModuleSettings(ModuleID, LocalResourceFile)
                Dim objEventBase As New EventBase
                Dim displayTimeZoneId As String = objEventBase.GetDisplayTimeZoneId(settings, objEvent.PortalID, "User")

                For Each userEmail In .UserEmails
                    Dim usedTimeZoneId As String = displayTimeZoneId
                    If displayTimeZoneId = "User" Then
                        usedTimeZoneId = .UserTimeZoneIds.Item(itemNo).ToString
                    End If
                    SendSingleEmail(userEmail, .UserLocales.Item(itemNo), objEvent, .txtEmailSubject, .txtEmailBody, .txtEmailFrom(), objEventSignups, attachments, domainurl, usedTimeZoneId, blTokenReplace)
                    itemNo += 1
                Next

                Dim userID As Integer
                For Each userID In .UserIDs
                    Dim objCtlUser As New UserController
                    Dim objUser As UserInfo = objCtlUser.GetUser(PortalID, userID)

                    If Not objUser Is Nothing Then
                        Dim usedTimeZoneId As String = displayTimeZoneId
                        If displayTimeZoneId = "User" Then
                            usedTimeZoneId = objUser.Profile.PreferredTimeZone.Id
                        End If
                        SendSingleEmail(objUser.Email, objUser.Profile.PreferredLocale, objEvent, .txtEmailSubject, .txtEmailBody, .txtEmailFrom(), objEventSignups, attachments, domainurl, usedTimeZoneId, blTokenReplace)
                    End If
                Next
            End With

            Threading.Thread.CurrentThread.CurrentCulture = _currculture

        End Sub

        Private Sub SendSingleEmail(ByVal userEmail As String, ByVal userLocale As Object, ByVal objEvent As EventInfo, ByVal txtEmailSubject As String, ByVal txtEmailBody As String, _
                                    ByVal txtEmailFrom As String, ByVal objEventSignups As EventSignupsInfo, ByVal attachments As Generic.List(Of Net.Mail.Attachment), ByVal domainurl As String, _
                                    ByVal displayTimeZoneId As String, ByVal blTokenReplace As Boolean)
            Dim tcc As TokenReplaceControllerClass
            Dim subject As String
            Dim body As String
            ' ReSharper disable RedundantAssignment
            Dim bodyformat As String = "html"
            ' ReSharper restore RedundantAssignment
            ChangeLocale(userLocale)

            Dim objEventInfoHelper As New EventInfoHelper(ModuleID, Nothing)
            objEvent = objEventInfoHelper.ConvertEventToDisplayTimeZone(objEvent.Clone, displayTimeZoneId)
            Dim objUsedSignup As New EventSignupsInfo
            If Not objEventSignups Is Nothing Then
                objUsedSignup = objEventSignups.Clone
                Dim objEventTimeZoneUtilities As New EventTimeZoneUtilities
                objUsedSignup.PayPalPaymentDate = objEventTimeZoneUtilities.ConvertFromUTCToDisplayTimeZone( _
                    objUsedSignup.PayPalPaymentDate, displayTimeZoneId).EventDate
            End If

            If blTokenReplace Then
                tcc = New TokenReplaceControllerClass(ModuleID, LocalResourceFile)
                subject = tcc.TokenReplaceEvent(objEvent, txtEmailSubject, objUsedSignup)
                body = tcc.TokenReplaceEvent(objEvent, txtEmailBody, objUsedSignup)
            Else
                subject = txtEmailSubject
                body = txtEmailBody
            End If
            body = AddHost(body, domainurl)
            bodyformat = GetBodyFormat(body)
            If bodyformat = "text" Then
                body = HtmlUtils.StripTags(body, True)
            End If
            If Not attachments Is Nothing Then
                Dim smtpEnableSsl As Boolean = False
                If Entities.Controllers.HostController.Instance.GetString("SMTPEnableSSL") = "Y" Then
                    smtpEnableSsl = True
                End If
                Mail.SendMail(txtEmailFrom, userEmail, "", "", "", MailPriority.Normal, subject, MailFormat.Html, Text.Encoding.UTF8, body, attachments, "", "", "", "", smtpEnableSsl)
            Else
                Mail.SendMail(txtEmailFrom, userEmail, "", subject, body, "", bodyformat, "", "", "", "")
            End If
        End Sub

        Private Function GetBodyFormat(ByVal body As String) As String
            Dim bodyformat As String
            Dim ems As New EventModuleSettings
            Dim settings As EventModuleSettings = ems.GetEventModuleSettings(ModuleID, LocalResourceFile)

            bodyformat = settings.HTMLEmail
            If bodyformat = "auto" Then
                If Common.Utilities.HtmlUtils.IsHtml(body) Then
                    bodyformat = "html"
                Else
                    bodyformat = "text"
                End If
            End If
            Return bodyformat
        End Function

        Private Sub ChangeLocale(ByVal userLocale As Object)
            Dim strLocale As String = ""
            Threading.Thread.CurrentThread.CurrentCulture = _currculture
            If Not userLocale Is Nothing Then
                strLocale = userLocale.ToString
            End If
            If Not userLocale Is Nothing And strLocale <> "" Then
                Dim userculture As New CultureInfo(strLocale, False)
                Threading.Thread.CurrentThread.CurrentCulture = userculture
            ElseIf Not CultureName = Nothing Then
                Dim userculture As New CultureInfo(CultureName, False)
                Threading.Thread.CurrentThread.CurrentCulture = userculture
            End If

        End Sub

        Private Function AddHost(ByVal content As String, ByVal domainurl As String) As String

            If domainurl Is Nothing Then
                Dim objEventInfoHelper As New EventInfoHelper()
                domainurl = objEventInfoHelper.GetDomainURL()
            End If
            domainurl = AddHTTP(domainurl)

            Dim txtContent As String = content
            If domainurl <> "" Then
                txtContent = Replace(txtContent, "src=""/", "src=""" & domainurl & "/", , , CompareMethod.Text)
                txtContent = Replace(txtContent, "href=""/", "href=""" & domainurl & "/", , , CompareMethod.Text)
            End If
            Return txtContent
        End Function

#End Region
    End Class
#End Region

#Region "TimeZone Utilities Class"
    Public Class EventTimeZoneUtilities
        Public Function ConvertFromUTCToDisplayTimeZone(ByVal utcDate As DateTime, ByVal displayTimeZoneId As String) As DateInfo
            Dim displayInfo As New DateInfo
            displayInfo.EventDate = utcDate
            displayInfo.EventTimeZoneId = "UTC"
            Try
                Dim displayTimeZone As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(displayTimeZoneId)
                displayInfo.EventTimeZoneId = displayTimeZone.Id
                displayInfo.EventDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, displayTimeZone)
            Catch ex As Exception
                Return displayInfo
            End Try
            Return displayInfo
        End Function

        Public Function ConvertToDisplayTimeZone(ByVal inDate As DateTime, ByVal inTimeZoneId As String, ByVal portalId As Integer, ByVal displayTimeZoneId As String) As DateInfo
            Dim displayDateInfo As New DateInfo
            If inTimeZoneId = displayTimeZoneId Then
                displayDateInfo.EventDate = inDate
                displayDateInfo.EventTimeZoneId = inTimeZoneId
                Return displayDateInfo
            End If
            Dim utcDate As DateTime = inDate
            If inTimeZoneId <> "UTC" Then
                utcDate = ConvertToUTCTimeZone(inDate, inTimeZoneId)
            End If

            displayDateInfo.EventDate = utcDate
            displayDateInfo.EventTimeZoneId = displayTimeZoneId
            If displayTimeZoneId <> "UTC" Then
                displayDateInfo = ConvertFromUTCToDisplayTimeZone(utcDate, displayTimeZoneId)
            End If
            Return displayDateInfo
        End Function

        Public Function ConvertToUTCTimeZone(ByVal inDate As DateTime, ByVal inTimeZoneId As String) As DateTime
            Dim utcDate As DateTime
            Try
                Dim inTimeZone As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(inTimeZoneId)
                utcDate = TimeZoneInfo.ConvertTimeToUtc(inDate, inTimeZone)
            Catch ex As Exception
                Return inDate
            End Try
            Return utcDate
        End Function

        Public Function ConvertFromUTCToModuleTimeZone(ByVal utcDate As DateTime, ByVal moduleTimeZoneId As String) As DateTime
            Dim moduleDate As DateTime
            Try
                Dim moduleTimeZone As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(moduleTimeZoneId)
                moduleDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, moduleTimeZone)
            Catch ex As Exception
                Return utcDate
            End Try
            Return moduleDate
        End Function

        Public Class DateInfo
            Private _eventDate As DateTime
            Private _eventTimeZoneId As String

            Property EventDate() As DateTime
                Get
                    Return _EventDate
                End Get
                Set(ByVal value As DateTime)
                    _eventDate = Value
                End Set
            End Property

            Property EventTimeZoneId() As String
                Get
                    Return _EventTimeZoneId
                End Get
                Set(ByVal value As String)
                    _eventTimeZoneId = Value
                End Set
            End Property

        End Class
    End Class

#End Region

End Namespace
