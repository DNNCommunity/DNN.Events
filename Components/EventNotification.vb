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
Imports System.Collections
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Modules




Namespace DotNetNuke.Modules.Events
    Public Class EventNotification
        Inherits Services.Scheduling.SchedulerClient

#Region "Constructors"
        Public Sub New(ByVal objScheduleHistoryItem As Services.Scheduling.ScheduleHistoryItem)
            MyBase.new()
            ScheduleHistoryItem = objScheduleHistoryItem
        End Sub
#End Region

#Region "Methods"
        Public Overrides Sub DoWork()
            Try
                'notification that the event is progressing
                Progressing()        'OPTIONAL

                Dim returnStrCleanup As String
                returnStrCleanup = CleanupExpired()
                If returnStrCleanup <> "" Then
                    ScheduleHistoryItem.AddLogNote("<br />" & returnStrCleanup & "<br />")
                End If

                Dim returnStr As String
                returnStr = SendEventNotifications()
                ScheduleHistoryItem.AddLogNote(returnStr)

                ScheduleHistoryItem.Succeeded = True     'REQUIRED

            Catch exc As Exception      'REQUIRED
                ScheduleHistoryItem.Succeeded = False    'REQUIRED
                ScheduleHistoryItem.AddLogNote("Event Notification failed." + exc.ToString + Status)    'OPTIONAL
                'notification that we have errored
                Errored(exc)         'REQUIRED
                'log the exception
                LogException(exc)       'OPTIONAL
            End Try
        End Sub

        Private Function SendEventNotifications() As String
            Dim objEvents As EventController = New EventController
            Dim objEvent As EventInfo
            Dim objEventNotifications As EventNotificationController = New EventNotificationController
            Dim notifyEvents As ArrayList
            Dim returnStr As String = "Event Notification completed."

            Status = "Sending Event Notifications"

            '***  All Event Notifications are stored in UTC internally.  
            notifyEvents = objEventNotifications.EventsNotificationsToSend(Date.UtcNow)

            For Each objNotification As EventNotificationInfo In notifyEvents
                'Get the Associated Event
                objEvent = objEvents.EventsGet(objNotification.EventID, objNotification.ModuleID)

                If Not (objEvent Is Nothing) Then
                    ' Setup PortalSettings
                    Dim portalSettings As PortalSettings = CreatePortalSettings(objNotification.PortalAliasID, objNotification.TabID)
                    Dim folderName As String = DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", objEvent.PortalID).FolderName

                    If Not portalSettings Is Nothing Then
                        ' Make up the LocalResourceFile value
                        Dim templateSourceDirectory As String = Common.Globals.ApplicationPath
                        Dim localResourceFile As String = templateSourceDirectory & "/DesktopModules/" & folderName & "/" & Localization.LocalResourceDirectory & "/EventNotification.ascx.resx"

                        ' Send the email
                        Dim objEventEmailInfo As New EventEmailInfo
                        Dim objEventEmail As New EventEmails(portalSettings.PortalId, objNotification.ModuleID, localResourceFile, objNotification.NotifyLanguage)
                        objEventEmailInfo.TxtEmailSubject = objEvent.Notify
                        objEventEmailInfo.TxtEmailBody = objEvent.Reminder
                        objEventEmailInfo.TxtEmailFrom() = objEvent.ReminderFrom
                        objEventEmailInfo.UserEmails.Add(objNotification.UserEmail)
                        objEventEmailInfo.UserLocales.Add(objNotification.NotifyLanguage)
                        objEventEmailInfo.UserTimeZoneIds.Add(objEvent.EventTimeZoneId)
                        Dim domainurl As String = portalSettings.PortalAlias.HTTPAlias
                        If domainurl.IndexOf("/", StringComparison.Ordinal) > 0 Then
                            domainurl = domainurl.Substring(0, domainurl.IndexOf("/", StringComparison.Ordinal))
                        End If

                        objEventEmail.SendEmails(objEventEmailInfo, objEvent, domainurl)
                    End If

                    '*** Update Notification (so we don't send again)
                    objNotification.NotificationSent = True
                    objEventNotifications.EventsNotificationSave(objNotification)
                    returnStr = "Event Notification completed. " + notifyEvents.Count.ToString + " Notification(s) sent!"
                End If
            Next

            '**** Delete Expired EventNotifications (older than 30 days)
            Dim endDate As DateTime = Date.UtcNow.AddDays(-30)
            objEventNotifications.EventsNotificationDelete(endDate)

            Status = "Event Notifications Sent Successfully"
            ScheduleHistoryItem.Succeeded = True
            Return returnStr

        End Function

        Private Function CleanupExpired() As String
            Dim returnStr As String = "Event Cleanup completed."
            Dim noDeletedEvents As Integer = 0

            Status = "Performing Event Cleanup"

            Dim objDesktopModule As DesktopModuleInfo
            objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", 0)

            If objDesktopModule Is Nothing Then
                Return "Module Definition 'DNN_Events' not found. Cleanup cannot be performed."
            End If

            Status = "Performing Event Cleanup: Dummy"
            Status = "Performing Event Cleanup:" + objDesktopModule.FriendlyName

            Dim objPortals As New Entities.Portals.PortalController
            Dim objPortal As Entities.Portals.PortalInfo
            Dim objModules As New ModuleController
            Dim objModule As ModuleInfo

            Dim lstportals As ArrayList = objPortals.GetPortals()
            For Each objPortal In lstportals
                Status = "Performing Event Cleanup:" + objDesktopModule.FriendlyName + " PortalID: Dummy"
                Status = "Performing Event Cleanup:" + objDesktopModule.FriendlyName + " PortalID:" + objPortal.PortalID.ToString

                Dim lstModules As ArrayList = objModules.GetModulesByDefinition(objPortal.PortalID, objDesktopModule.FriendlyName)
                For Each objModule In lstModules
                    ' This check for objModule = Nothing because of error in DNN 5.0.0 in GetModulesByDefinition
                    If objModule Is Nothing Then
                        Continue For
                    End If
                    Status = "Performing Event Cleanup:" + objDesktopModule.FriendlyName + " PortalID:" + objPortal.PortalID.ToString + " ModuleID: Dummy"
                    Status = "Performing Event Cleanup:" + objDesktopModule.FriendlyName + " PortalID:" + objPortal.PortalID.ToString + " ModuleID:" + objModule.ModuleID.ToString

                    Dim ems As New EventModuleSettings
                    Dim settings As EventModuleSettings = ems.GetEventModuleSettings(objModule.ModuleID, Nothing)
                    If settings.expireevents <> "" Then
                        Status = "Performing Event Cleanup:" + objDesktopModule.FriendlyName + " PortalID:" + objPortal.PortalID.ToString + " ModuleID:" + objModule.ModuleID.ToString + " IN PROGRESS"

                        Dim objEventCtl As New EventController
                        Dim expireDate As DateTime = DateAdd(DateInterval.Day, -CType(settings.expireevents, Integer), Date.UtcNow)
                        Dim startdate As DateTime = expireDate.AddYears(-10)
                        Dim endDate As DateTime = expireDate.AddDays(1)
                        Dim objEventInfoHelper As New EventInfoHelper(objModule.ModuleID, settings)
                        Dim categoryIDs As New ArrayList
                        categoryIDs.Add("-1")
                        Dim locationIDs As New ArrayList
                        locationIDs.Add("-1")
                        Dim lstEvents As ArrayList = objEventInfoHelper.GetEvents(startdate, endDate, False, categoryIDs, locationIDs, -1, -1)

                        Dim objEventTimeZoneUtilities As New EventTimeZoneUtilities
                        For Each objEvent As EventInfo In lstEvents
                            Dim eventTime As DateTime = objEventTimeZoneUtilities.ConvertToUTCTimeZone(objEvent.EventTimeEnd, objEvent.EventTimeZoneId)
                            If eventTime < expireDate Then
                                objEvent.Cancelled = True
                                objEventCtl.EventsSave(objEvent, True, Nothing, True)
                                noDeletedEvents += 1
                                returnStr = "Event Cleanup completed. " + noDeletedEvents.ToString + " Events deleted."
                            End If
                        Next
                        objEventCtl.EventsCleanupExpired(objModule.PortalID, objModule.ModuleID)
                    End If
                Next
            Next
            Status = "Cleanup complete"
            Return returnStr
        End Function

        Public Function CreatePortalSettings(ByVal portalAliasID As Integer, ByVal tabID As Integer) As PortalSettings
            Dim cacheKey As String = "EventsPortalSettings" & portalAliasID.ToString
            Dim ps As New PortalSettings
            Dim pscache As PortalSettings = CType(Common.Utilities.DataCache.GetCache(cacheKey), PortalSettings)

            If pscache Is Nothing Then
                ' Setup PortalSettings
                Dim objPortalAliases As PortalAliasController = New PortalAliasController
                Dim objPortalAlias As PortalAliasInfo

                objPortalAlias = objPortalAliases.GetPortalAliasByPortalAliasID(portalAliasID)
                If objPortalAlias Is Nothing Then
                    Return Nothing
                End If
                Dim portalController As New PortalController
                Dim portal As Entities.Portals.PortalInfo = portalController.GetPortal(objPortalAlias.PortalID)


                With ps
                    .PortalAlias = objPortalAlias
                    .PortalId = portal.PortalID
                    .PortalName = portal.PortalName
                    .LogoFile = portal.LogoFile
                    .FooterText = portal.FooterText
                    .ExpiryDate = portal.ExpiryDate
                    .UserRegistration = portal.UserRegistration
                    .BannerAdvertising = portal.BannerAdvertising
                    .Currency = portal.Currency
                    .AdministratorId = portal.AdministratorId
                    .Email = portal.Email
                    .HostFee = portal.HostFee
                    .HostSpace = portal.HostSpace
                    .PageQuota = portal.PageQuota
                    .UserQuota = portal.UserQuota
                    .AdministratorRoleId = portal.AdministratorRoleId
                    .AdministratorRoleName = portal.AdministratorRoleName
                    .RegisteredRoleId = portal.RegisteredRoleId
                    .RegisteredRoleName = portal.RegisteredRoleName
                    .Description = portal.Description
                    .KeyWords = portal.KeyWords
                    .BackgroundFile = portal.BackgroundFile
                    .GUID = portal.GUID
                    .SiteLogHistory = portal.SiteLogHistory
                    .AdminTabId = portal.AdminTabId
                    .SuperTabId = portal.SuperTabId
                    .SplashTabId = portal.SuperTabId
                    .HomeTabId = portal.HomeTabId
                    .LoginTabId = portal.LoginTabId
                    .UserTabId = portal.UserTabId
                    .DefaultLanguage = portal.DefaultLanguage
                    .Pages = portal.Pages
                    .Users = portal.Users
                    If .HostSpace = Nothing Then
                        .HostSpace = 0
                    End If
                    If .DefaultLanguage Is Nothing Then
                        .DefaultLanguage = Localization.SystemLocale
                    End If
                    If .TimeZone Is Nothing Then
                        .TimeZone = TimeZoneInfo.FindSystemTimeZoneById(Localization.SystemTimeZone)
                    End If
                    .HomeDirectory = Common.Globals.ApplicationPath + "/" + portal.HomeDirectory + "/"
                End With
                Common.Utilities.DataCache.SetCache(cacheKey, ps)
            Else
                ps = pscache
            End If
            Dim tabController As New Entities.Tabs.TabController
            Dim tab As Entities.Tabs.TabInfo = tabController.GetTab(TabID, ps.PortalId, False)
            If tab Is Nothing Then
                Return Nothing
            End If
            ps.ActiveTab = tab

            Try
                ' Now to put it into the HTTPContext
                If HttpContext.Current Is Nothing Then
                    Dim page As String = ps.PortalAlias.HTTPAlias
                    Dim query As String = String.Empty
                    Dim output As IO.TextWriter = Nothing
                    Dim workerrequest As Hosting.SimpleWorkerRequest = New Hosting.SimpleWorkerRequest(page, query, output)
                    HttpContext.Current = New HttpContext(workerrequest)
                    HttpContext.Current.Items.Add("PortalSettings", ps)
                Else
                    HttpContext.Current.Items.Remove("PortalSettings")
                    HttpContext.Current.Items.Add("PortalSettings", ps)
                End If
            Catch exc As Exception
            End Try

            Return ps
        End Function
#End Region

    End Class

End Namespace
