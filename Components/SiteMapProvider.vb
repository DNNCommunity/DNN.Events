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
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.Sitemap

Namespace DotNetNuke.Modules.Events.Providers

    Public Class Sitemap
        Inherits SitemapProvider
#Region "Public Methods"
        Public Overrides Function GetUrls(ByVal portalID As Integer, ByVal ps As PortalSettings, ByVal version As String) As Generic.List(Of SitemapUrl)
            Dim sitemapUrls As New Generic.List(Of SitemapUrl)

            Dim objDesktopModule As Entities.Modules.DesktopModuleInfo
            objDesktopModule = Entities.Modules.DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", portalID)

            Dim objModules As New Entities.Modules.ModuleController
            Dim objModule As Entities.Modules.ModuleInfo
            Dim lstModules As ArrayList = objModules.GetModulesByDefinition(portalID, objDesktopModule.FriendlyName)
            Dim moduleIds As New ArrayList
            Dim visibleModuleIds As New ArrayList
            Dim visibleTabModuleIds As New ArrayList
            For Each objModule In lstModules
                Dim objTabPermissions As Security.Permissions.TabPermissionCollection = Security.Permissions.TabPermissionController.GetTabPermissions(objModule.TabID, portalID)
                Dim objTabPermission As Security.Permissions.TabPermissionInfo
                For Each objTabPermission In objTabPermissions
                    If objTabPermission.PermissionKey = "VIEW" And objTabPermission.RoleName <> "" And objTabPermission.AllowAccess And (objTabPermission.RoleID = -1 Or objTabPermission.RoleID = -3) Then
                        If objModule.InheritViewPermissions Then
                            visibleTabModuleIds.Add("Tab" & objModule.TabID.ToString & "Mod" & objModule.ModuleID.ToString)
                            visibleModuleIds.Add(objModule.ModuleID)
                            Exit For
                        Else
                            Dim objModulePermission As Security.Permissions.ModulePermissionInfo
                            ' ReSharper disable LoopCanBeConvertedToQuery
                            For Each objModulePermission In objModule.ModulePermissions
                                ' ReSharper restore LoopCanBeConvertedToQuery
                                If objModulePermission.PermissionKey = "VIEW" And objModulePermission.RoleName <> "" And objModulePermission.AllowAccess And (objModulePermission.RoleID = -1 Or objModulePermission.RoleID = -3) Then
                                    visibleTabModuleIds.Add("Tab" & objModule.TabID.ToString & "Mod" & objModule.ModuleID.ToString)
                                    visibleModuleIds.Add(objModule.ModuleID)
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                Next
            Next
            For Each objModule In lstModules
                ' This check for objModule = Nothing because of error in DNN 5.0.0 in GetModulesByDefinition
                If objModule Is Nothing Then
                    Continue For
                End If
                If objModule.IsDeleted Then
                    Continue For
                End If
                If moduleIds.Contains(objModule.ModuleID) Then
                    Continue For
                End If
                If Not visibleTabModuleIds.Contains("Tab" & objModule.TabID.ToString & "Mod" & objModule.ModuleID.ToString) Then
                    Continue For
                End If
                moduleIds.Add(objModule.ModuleID)

                Dim ems As New EventModuleSettings
                Dim settings As EventModuleSettings = ems.GetEventModuleSettings(objModule.ModuleID, Nothing)
                If Not settings.enableSitemap Then
                    Continue For
                End If
                If settings.SocialGroupModule = EventModuleSettings.SocialModule.UserProfile Then
                    Continue For
                End If

                Dim iCategoryIDs As New ArrayList
                If settings.enablecategories = EventModuleSettings.DisplayCategories.DoNotDisplay Then
                    iCategoryIDs = settings.ModuleCategoryIDs
                Else
                    iCategoryIDs.Add("-1")
                End If
                Dim ilocationIDs As New ArrayList
                If settings.Enablelocations = EventModuleSettings.DisplayLocations.DoNotDisplay Then
                    ilocationIDs = settings.ModuleLocationIDs
                Else
                    ilocationIDs.Add("-1")
                End If

                Dim objEventTimeZoneUtilities As New EventTimeZoneUtilities
                Dim currDate As DateTime = objEventTimeZoneUtilities.ConvertFromUTCToModuleTimeZone(Date.UtcNow, settings.TimeZoneId)
                Dim dtStartDate As DateTime = DateAdd(DateInterval.Day, -settings.siteMapDaysBefore, currDate)
                Dim dtEndDate As DateTime = DateAdd(DateInterval.Day, settings.siteMapDaysAfter, currDate)

                Dim objEventInfoHelper As New EventInfoHelper(objModule.ModuleID, objModule.TabID, portalID, settings)
                Dim lstevents As ArrayList
                lstevents = objEventInfoHelper.GetEvents(dtStartDate, dtEndDate, settings.MasterEvent, iCategoryIDs, ilocationIDs, -1, -1)

                Dim objEvent As EventInfo
                For Each objEvent In lstevents
                    If settings.enforcesubcalperms And Not visibleModuleIds.Contains(objEvent.ModuleID) Then
                        Continue For
                    End If
                    Dim pageUrl As New SitemapUrl
                    With pageUrl
                        .Url = objEventInfoHelper.DetailPageURL(objEvent, False)
                        .Priority = settings.siteMapPriority
                        .LastModified = objEvent.LastUpdatedAt
                        .ChangeFrequency = SitemapChangeFrequency.Daily
                    End With
                    sitemapUrls.Add(pageUrl)
                Next
            Next

            Return sitemapUrls
        End Function

#End Region

    End Class

End Namespace
