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
Imports System.Text
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Security.Roles



Namespace DotNetNuke.Modules.Events

#Region "TokenReplaceController Class"
    ''' <summary>
    ''' Replaces the tokens that are defined for the Event e-mails and views
    ''' </summary>
    Public Class TokenReplaceControllerClass

#Region "Member Variables"
        Private _localResourceFile As String
        Private _moduleId As Int32
#End Region

#Region "Constructor"
        Sub New(ByVal moduleID As Integer, ByVal localResourceFile As String)
            Me.ModuleID = ModuleID
            Me.LocalResourceFile = localResourceFile
        End Sub
#End Region

#Region "Properties"
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
#End Region

#Region "Methods"
        ''' <summary>
        ''' Replace tokens in sourcetext with eventspecific token
        ''' </summary>
        Public Function TokenReplaceEvent(ByVal eventInfo As EventInfo, ByVal sourceText As String) As String
            Return TokenReplaceEvent(eventInfo, sourceText, Nothing, False)
        End Function
        Public Function TokenReplaceEvent(ByVal eventInfo As EventInfo, ByVal sourceText As String, ByVal eventSignupsInfo As EventSignupsInfo) As String
            Return TokenReplaceEvent(eventInfo, sourceText, eventSignupsInfo, False)
        End Function
        Public Function TokenReplaceEvent(ByVal eventInfo As EventInfo, ByVal sourceText As String, ByVal addsubmodulename As Boolean) As String
            Return TokenReplaceEvent(eventInfo, sourceText, Nothing, addsubmodulename)
        End Function
        Public Function TokenReplaceEvent(ByVal eventInfo As EventInfo, ByVal sourceText As String, ByVal eventSignupsInfo As EventSignupsInfo, ByVal addsubmodulename As Boolean) As String
            Return TokenReplaceEvent(eventInfo, sourceText, eventSignupsInfo, addsubmodulename, False)
        End Function
        Public Function TokenReplaceEvent(ByVal eventInfo As EventInfo, ByVal sourceText As String, ByVal eventSignupsInfo As EventSignupsInfo, ByVal addsubmodulename As Boolean, ByVal isEventEditor As Boolean) As String

            Dim dict As New Generic.Dictionary(Of String, Object)

            Dim cacheKey As String = "EventsFolderName" & ModuleID.ToString
            Dim folderName As String = CType(Common.Utilities.DataCache.GetCache(cacheKey), String)
            If folderName Is Nothing Then
                folderName = DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", eventInfo.PortalID).FolderName
                Common.Utilities.DataCache.SetCache(cacheKey, folderName)
            End If

            'Module settings
            Dim ems As New EventModuleSettings
            Dim settings As EventModuleSettings = ems.GetEventModuleSettings(ModuleID, LocalResourceFile)


            Dim trn As New Services.Tokens.TokenReplace(Services.Tokens.Scope.DefaultSettings, ModuleID)

            'Parameter processing
            sourceText = TokenParameters(sourceText, eventInfo, settings)

            'title
            If Not dict.ContainsKey("title") Then
                If addsubmodulename And eventInfo.ModuleTitle <> Nothing Then
                    'Eventname and moduletitle
                    dict.Add("title", String.Format("{0} ({1})", eventInfo.EventName, Trim(eventInfo.ModuleTitle)))
                Else
                    'Just eventname
                    dict.Add("title", eventInfo.EventName)
                End If
            End If

            'submodule name
            If eventInfo.ModuleTitle <> Nothing And ModuleID <> eventInfo.ModuleID Then
                dict.Add("subcalendarname", String.Format("({0})", Trim(eventInfo.ModuleTitle)))
                dict.Add("subcalendarnameclean", Trim(eventInfo.ModuleTitle))
            End If

            'alldayeventtext
            dict.Add("alldayeventtext", Localization.GetString("TokenAllDayEventText", LocalResourceFile))

            'startdatelabel
            dict.Add("startdatelabel", Localization.GetString("TokenStartdateLabel", LocalResourceFile))

            'startdate
            sourceText = TokenReplaceDate(sourceText, "event", "startdate", eventInfo.EventTimeBegin)

            'enddatelabel
            dict.Add("enddatelabel", Localization.GetString("TokenEnddateLabel", LocalResourceFile))

            'enddate
            sourceText = TokenReplaceDate(sourceText, "event", "enddate", eventInfo.EventTimeEnd)

            'timezone
            ' Added a try/catch since dnn core can failing getting timezone info
            Try
                Dim value As String = TimeZoneInfo.FindSystemTimeZoneById(eventInfo.EventTimeZoneId).DisplayName
                dict.Add("timezone", value)
            Catch
                dict.Add("timezone", "")
            End Try

            'Duration
            dict.Add("durationdayslabel", Localization.GetString("TokenDurationDaysLabel", LocalResourceFile))
            dict.Add("durationdays", CType(Int(eventInfo.Duration / 1440 + 1), Integer))

            'descriptionlabel
            dict.Add("descriptionlabel", Localization.GetString("TokenDescriptionLabel", LocalResourceFile))

            'description
            If Not dict.ContainsKey("description") Then
                dict.Add("description", HttpUtility.HtmlDecode(eventInfo.EventDesc))
            End If
            sourceText = TokenLength(sourceText, "event", "description", dict)

            'categorylabel
            If Not eventInfo.CategoryName Is Nothing Then
                dict.Add("categorylabel", Localization.GetString("TokenCategoryLabel", LocalResourceFile))
            Else
                dict.Add("categorylabel", "")
            End If

            'category, categoryfontcolor, categorybackcolor
            If Not eventInfo.CategoryName Is Nothing Then
                If eventInfo.Color.Length > 0 Then
                    dict.Add("category", String.Format("<div style='background-color:{1};color:{2}'>{0}</div>", eventInfo.CategoryName, eventInfo.Color, eventInfo.FontColor))
                    dict.Add("categoryfontcolor", eventInfo.FontColor)
                    dict.Add("categorybackcolor", eventInfo.Color)
                Else
                    dict.Add("category", eventInfo.CategoryName)
                End If
                dict.Add("categoryname", eventInfo.CategoryName)
            Else
                dict.Add("category", "")
                dict.Add("categoryname", "")
            End If

            'locationlabel
            If Not eventInfo.LocationName Is Nothing Then
                dict.Add("locationlabel", Localization.GetString("TokenLocationLabel", LocalResourceFile))
            Else
                dict.Add("locationlabel", "")
            End If

            'location, locationurl
            If Not eventInfo.MapURL Is Nothing And eventInfo.MapURL <> "" Then
                dict.Add("location", String.Format("<a href='{1}'>{0}</a>", eventInfo.LocationName, eventInfo.MapURL))
                dict.Add("locationurl", eventInfo.MapURL)
            Else
                dict.Add("location", eventInfo.LocationName)
                dict.Add("locationurl", "")
            End If

            'locationname
            dict.Add("locationname", eventInfo.LocationName)

            'Other location properties.
            Dim eventLocation As New EventLocationInfo
            eventLocation = New EventLocationController().EventsLocationGet(eventInfo.Location, eventInfo.PortalID)
            If eventLocation IsNot Nothing Then
                dict.Add("locationaddresslabel", Localization.GetString("TokenLocationAddressLabel", LocalResourceFile))
                dict.Add("locationstreet", eventLocation.Street)
                dict.Add("locationpostalcode", eventLocation.PostalCode)
                dict.Add("locationcity", eventLocation.City)
                dict.Add("locationregion", eventLocation.Region)
                dict.Add("locationcountry", eventLocation.Country)
            Else
                dict.Add("locationaddresslabel", "")
                dict.Add("locationstreet", "")
                dict.Add("locationpostalcode", "")
                dict.Add("locationcity", "")
                dict.Add("locationregion", "")
                dict.Add("locationcountry", "")
            End If

            'customfield1
            If settings.EventsCustomField1 Then
                dict.Add("customfield1", eventInfo.CustomField1)
            Else : dict.Add("customfield1", "")
            End If

            'customfield1label
            If settings.EventsCustomField1 Then
                dict.Add("customfield1label", Localization.GetString("TokenCustomField1Label", LocalResourceFile))
            Else : dict.Add("customfield1label", "")
            End If

            'customfield2
            If settings.EventsCustomField2 Then
                dict.Add("customfield2", eventInfo.CustomField2)
            Else : dict.Add("customfield2", "")
            End If

            'customfield2label
            If settings.EventsCustomField2 Then
                dict.Add("customfield2label", Localization.GetString("TokenCustomField2Label", LocalResourceFile))
            Else : dict.Add("customfield2label", "")
            End If

            'descriptionlabel
            dict.Add("summarylabel", Localization.GetString("TokenSummaryLabel", LocalResourceFile))

            'description
            If Not dict.ContainsKey("summary") Then
                dict.Add("summary", HttpUtility.HtmlDecode(eventInfo.Summary))
            End If
            sourceText = TokenLength(sourceText, "event", "summary", dict)

            'eventid
            dict.Add("eventid", eventInfo.EventID)

            'eventmoduleid
            dict.Add("eventmoduleid", eventInfo.ModuleID)


            'Createddate
            'TokenCreatedOnLabel.Text   on	
            dict.Add("createddatelabel", Localization.GetString("TokenCreatedOnLabel", LocalResourceFile))
            sourceText = TokenReplaceDate(sourceText, "event", "createddate", eventInfo.CreatedDate)

            'LastUpdateddate
            'TokenLastUpdatedOnLabel.Text   Last updated on	
            dict.Add("lastupdateddatelabel", Localization.GetString("TokenLastUpdatedOnLabel", LocalResourceFile))
            sourceText = TokenReplaceDate(sourceText, "event", "lastupdateddate", eventInfo.LastUpdatedAt)

            If settings.Eventsignup And eventInfo.Signups Then
                'maxenrollmentslabel
                'maxenrollments
                dict.Add("maxenrollmentslabel", Localization.GetString("TokenMaxEnrollmentsLabel", LocalResourceFile))
                If eventInfo.MaxEnrollment > 0 Then
                    dict.Add("maxenrollments", eventInfo.MaxEnrollment.ToString)
                Else
                    dict.Add("maxenrollments", Localization.GetString("Unlimited", LocalResourceFile))
                End If

                'noenrollmentslabel
                'noenrollments
                dict.Add("noenrollmentslabel", Localization.GetString("TokenNoEnrollmentsLabel", LocalResourceFile))
                dict.Add("noenrollments", eventInfo.Enrolled.ToString)

                'novacancieslabel
                'novacancies
                dict.Add("novacancieslabel", Localization.GetString("TokenNoVacanciesLabel", LocalResourceFile))
                If eventInfo.MaxEnrollment > 0 Then
                    dict.Add("novacancies", (eventInfo.MaxEnrollment - eventInfo.Enrolled).ToString)
                Else
                    dict.Add("novacancies", Localization.GetString("Unlimited", LocalResourceFile))
                End If
            Else
                dict.Add("maxenrollmentslabel", "")
                dict.Add("maxenrollments", "")
                dict.Add("noenrollmentslabel", "")
                dict.Add("noenrollments", "")
            End If

            If eventInfo.SocialGroupId > 0 Then
                'groupnamelabel
                'groupname
                Dim roleController As New RoleController
                Dim rolename As String = roleController.GetRole(eventInfo.SocialGroupId, eventInfo.PortalID).RoleName
                dict.Add("socialgrouprolenamelabel", Localization.GetString("TokenSocialGroupRoleNameLabel", LocalResourceFile))
                dict.Add("socialgrouprolename", rolename)
                dict.Add("socialgrouproleid", eventInfo.SocialGroupId.ToString)
            End If

            If eventInfo.SocialUserUserName IsNot Nothing Then
                'socialuserusernamelabel
                'socialuserusername
                dict.Add("socialuserusernamelabel", Localization.GetString("TokenSocialUserUserNameLabel", LocalResourceFile))
                dict.Add("socialuserusername", eventInfo.SocialUserUserName)
            End If

            If eventInfo.SocialUserDisplayName IsNot Nothing Then
                'socialuserdisplaynamelabel
                'socialuserdisplayname
                dict.Add("socialuserdisplaynamelabel", Localization.GetString("TokenSocialUserDisplayNameLabel", LocalResourceFile))
                dict.Add("socialuserdisplayname", eventInfo.SocialUserDisplayName)
            End If

            ' Process Event Signups Info is passed
            If eventSignupsInfo IsNot Nothing Then
                'signupuserid
                dict.Add("signupuserid", eventSignupsInfo.UserID)

                'signupusername, ...
                If eventSignupsInfo.UserID <> -1 Then
                    dict.Add("signupusername", eventSignupsInfo.UserName)
                    dict.Add("signupuserfirstname", "")
                    dict.Add("signupuserlastname", "")
                    dict.Add("signupuseremail", "")
                    dict.Add("signupuserstreet", "")
                    dict.Add("signupuserpostalcode", "")
                    dict.Add("signupusercity", "")
                    dict.Add("signupuserregion", "")
                    dict.Add("signupusercountry", "")
                    dict.Add("signupusercompany", "")
                    dict.Add("signupuserjobtitle", "")
                    dict.Add("signupuserrefnumber", "")
                Else
                    dict.Add("signupusername", eventSignupsInfo.AnonName)
                    dict.Add("signupuserfirstname", eventSignupsInfo.FirstName)
                    dict.Add("signupuserlastname", eventSignupsInfo.LastName)
                    dict.Add("signupuseremail", eventSignupsInfo.AnonEmail)
                    dict.Add("signupuserstreet", eventSignupsInfo.Street)
                    dict.Add("signupuserpostalcode", eventSignupsInfo.PostalCode)
                    dict.Add("signupusercity", eventSignupsInfo.City)
                    dict.Add("signupuserregion", eventSignupsInfo.Region)
                    dict.Add("signupusercountry", eventSignupsInfo.Country)
                    dict.Add("signupusercompany", eventSignupsInfo.Company)
                    dict.Add("signupuserjobtitle", eventSignupsInfo.JobTitle)
                    dict.Add("signupuserrefnumber", eventSignupsInfo.ReferenceNumber)
                End If

                'signupdatelabel
                dict.Add("signupdatelabel", Localization.GetString("TokenSignupdateLabel", LocalResourceFile))

                'signupdate
                sourceText = TokenReplaceDate(sourceText, "event", "signupdate", eventSignupsInfo.PayPalPaymentDate)

                'noenroleeslabel
                dict.Add("noenroleeslabel", Localization.GetString("TokenNoenroleesLabel", LocalResourceFile))

                'noenrolees
                dict.Add("noenrolees", eventSignupsInfo.NoEnrolees)
            End If

            'Custom/external enrollment page
            If settings.EnrollmentPageAllowed Then
                dict.Add("enrollmentdefaulturl", settings.EnrollmentPageDefaultUrl)
            Else
                dict.Add("enrollmentdefaulturl", "")
            End If

            'try and get the portalsettings. When in scheduled mode (EventNotifications)  
            'and no permissions, these will not be available and will error
            Dim ps As PortalSettings = Nothing
            Try
                ps = CType(HttpContext.Current.Items("PortalSettings"), Entities.Portals.PortalSettings)
            Catch exc As Exception
            End Try
            If Not ps Is Nothing Then
                ' add tokens for items that use PortalSettings
                TokenReplacewithPortalSettings(ps, eventInfo, settings, dict, folderName, sourceText, isEventEditor)
            End If

            Return trn.ReplaceEnvironmentTokens(sourceText, dict, "event")
        End Function

        Private Function TokenLength(ByVal sourceText As String, ByVal customCaption As String, ByVal customToken As String, ByVal dict As Generic.Dictionary(Of String, Object)) As String
            Dim trn As New Services.Tokens.TokenReplace(Services.Tokens.Scope.DefaultSettings, ModuleID)
            Dim tokenText As String = trn.ReplaceEnvironmentTokens("[" + customCaption + ":" + customToken + "]", dict, customCaption)
            Do While InStr(sourceText, "[" + customCaption + ":" + customToken + "]") > 0 Or InStr(sourceText, "[" + customCaption + ":" + customToken + "|") > 0
                With GetTokenFormat(sourceText, customToken, customCaption)
                    If .tokenfound Then
                        If .formatstring <> "" Then
                            sourceText = Replace(sourceText, "[" + customCaption + ":" + customToken + "|" + .formatstring + "]", HtmlUtils.Shorten(tokenText, CInt(.formatstring), "..."))
                        Else
                            sourceText = Replace(sourceText, "[" + customCaption + ":" + customToken + "]", tokenText)
                        End If
                    End If
                End With
            Loop
            Return sourceText
        End Function

        Private Function TokenReplaceDate(ByVal sourceText As String, ByVal customCaption As String, ByVal customToken As String, ByVal customDate As DateTime) As String

            Dim tokenDate As DateTime = customDate.Date.AddMinutes(customDate.TimeOfDay.TotalMinutes)
            Do While InStr(sourceText, "[" + customCaption + ":" + customToken + "]") > 0 Or InStr(sourceText, "[" + customCaption + ":" + customToken + "|") > 0
                With GetTokenFormat(sourceText, customToken, customCaption)
                    If .tokenfound Then
                        If .formatstring <> "" Then
                            sourceText = Replace(sourceText, "[" + customCaption + ":" + customToken + "|" + .formatstring + "]", String.Format("{0:" & .formatstring & "}", tokenDate))
                        Else
                            sourceText = Replace(sourceText, "[" + customCaption + ":" + customToken + "]", String.Format("{0:f}", tokenDate))
                        End If
                    End If
                End With
            Loop

            Return sourceText

        End Function

        Public Function TokenParameters(ByVal sourceText As String, ByVal eventInfo As EventInfo, ByVal settings As EventModuleSettings) As String

            If eventInfo.AllDayEvent Then
                sourceText = TokenOneParameter(sourceText, "ALLDAYEVENT", True)
                sourceText = TokenOneParameter(sourceText, "NOTALLDAYEVENT", False)
            Else
                sourceText = TokenOneParameter(sourceText, "ALLDAYEVENT", False)
                sourceText = TokenOneParameter(sourceText, "NOTALLDAYEVENT", True)
            End If
            If eventInfo.DisplayEndDate Then
                sourceText = TokenOneParameter(sourceText, "DISPLAYENDDATE", True)
            Else
                sourceText = TokenOneParameter(sourceText, "DISPLAYENDDATE", False)
            End If

            Dim eventimagesetting As Object = Settings.eventimage
            Dim eventimagebool As Boolean
            If Boolean.TryParse(CStr(eventimagesetting), eventimagebool) AndAlso (eventimagebool And eventInfo.ImageDisplay) Then
                sourceText = TokenOneParameter(sourceText, "IFHASIMAGE", True)
                sourceText = TokenOneParameter(sourceText, "IFNOTHASIMAGE", False)
            Else
                sourceText = TokenOneParameter(sourceText, "IFHASIMAGE", False)
                sourceText = TokenOneParameter(sourceText, "IFNOTHASIMAGE", True)
            End If

            If eventInfo.Category > 0 Then
                sourceText = TokenOneParameter(sourceText, "IFHASCATEGORY", True)
            Else
                sourceText = TokenOneParameter(sourceText, "IFHASCATEGORY", False)
            End If
            If eventInfo.Location > 0 Then
                sourceText = TokenOneParameter(sourceText, "IFHASLOCATION", True)
            Else
                sourceText = TokenOneParameter(sourceText, "IFHASLOCATION", False)
            End If
            If eventInfo.MapURL <> "" Then
                sourceText = TokenOneParameter(sourceText, "IFHASLOCATIONURL", True)
            Else
                sourceText = TokenOneParameter(sourceText, "IFHASLOCATIONURL", False)
            End If
            If eventInfo.MapURL = "" Then
                sourceText = TokenOneParameter(sourceText, "IFNOTHASLOCATIONURL", True)
            Else
                sourceText = TokenOneParameter(sourceText, "IFNOTHASLOCATIONURL", False)
            End If
            If Settings.eventsignup And eventInfo.Signups Then
                sourceText = TokenOneParameter(sourceText, "IFALLOWSENROLLMENTS", True)
            Else
                sourceText = TokenOneParameter(sourceText, "IFALLOWSENROLLMENTS", False)
            End If
            If Settings.EventsCustomField1 Then
                sourceText = TokenOneParameter(sourceText, "DISPLAYCUSTOMFIELD1", True)
            Else
                sourceText = TokenOneParameter(sourceText, "DISPLAYCUSTOMFIELD1", False)
            End If
            If Settings.EventsCustomField2 Then
                sourceText = TokenOneParameter(sourceText, "DISPLAYCUSTOMFIELD2", True)
            Else
                sourceText = TokenOneParameter(sourceText, "DISPLAYCUSTOMFIELD2", False)
            End If
            If Settings.DetailPageAllowed And eventInfo.DetailPage Then
                sourceText = TokenOneParameter(sourceText, "CUSTOMDETAILPAGE", True)
            Else
                sourceText = TokenOneParameter(sourceText, "CUSTOMDETAILPAGE", False)
            End If
            If eventInfo.EventTimeBegin.Date = eventInfo.EventTimeEnd.Date Then  'one day event... 
                sourceText = TokenOneParameter(sourceText, "ONEDAYEVENT", True)
                sourceText = TokenOneParameter(sourceText, "NOTONEDAYEVENT", False)
            Else
                sourceText = TokenOneParameter(sourceText, "ONEDAYEVENT", False)
                sourceText = TokenOneParameter(sourceText, "NOTONEDAYEVENT", True)
            End If
            If eventInfo.RRULE <> "" Then  'recurring event
                sourceText = TokenOneParameter(sourceText, "RECURRINGEVENT", True)
                sourceText = TokenOneParameter(sourceText, "NOTRECURRINGEVENT", False)
            Else
                sourceText = TokenOneParameter(sourceText, "RECURRINGEVENT", False)
                sourceText = TokenOneParameter(sourceText, "NOTRECURRINGEVENT", True)
            End If
            If eventInfo.IsPrivate Then  'Is private event
                sourceText = TokenOneParameter(sourceText, "PRIVATE", True)
                sourceText = TokenOneParameter(sourceText, "NOTPRIVATE", False)
            Else
                sourceText = TokenOneParameter(sourceText, "PRIVATE", False)
                sourceText = TokenOneParameter(sourceText, "NOTPRIVATE", True)
            End If
            If Settings.tzdisplay Then
                sourceText = TokenOneParameter(sourceText, "IFTIMEZONEDISPLAY", True)
            Else
                sourceText = TokenOneParameter(sourceText, "IFTIMEZONEDISPLAY", False)
            End If
            If eventInfo.Duration > 1440 Then
                sourceText = TokenOneParameter(sourceText, "IFMULTIDAY", True)
                sourceText = TokenOneParameter(sourceText, "IFNOTMULTIDAY", False)
            Else
                sourceText = TokenOneParameter(sourceText, "IFMULTIDAY", False)
                sourceText = TokenOneParameter(sourceText, "IFNOTMULTIDAY", True)
            End If

            If sourceText.Contains("[HASROLE_") Or sourceText.Contains("[HASNOTROLE_") Then
                Dim role As RoleInfo
                Dim roleController As New RoleController
                Dim userRolesIList As Generic.IList(Of UserRoleInfo)
                Dim userRoles As New ArrayList
                If Not UserController.GetCurrentUserInfo() Is Nothing Then
                    userRolesIList = roleController.GetUserRoles(UserController.GetCurrentUserInfo, True)
                    ' ReSharper disable NotAccessedVariable
                    Dim i As Integer = 0
                    ' ReSharper restore NotAccessedVariable
                    For Each userRole As UserRoleInfo In userRolesIList
                        userRoles.Add(userRole.RoleName)
                        i += 1
                    Next
                End If
                For Each role In roleController.GetPortalRoles(eventInfo.PortalID)
                    sourceText = TokenOneParameter(sourceText, "HASROLE_" + role.RoleName, userRoles.Contains(role.RoleName))
                    sourceText = TokenOneParameter(sourceText, "HASNOTROLE_" + role.RoleName, Not userRoles.Contains(role.RoleName))
                Next
            End If

            If eventInfo.Summary <> "" Then
                sourceText = TokenOneParameter(sourceText, "IFHASSUMMARY", True)
            Else
                sourceText = TokenOneParameter(sourceText, "IFHASSUMMARY", False)
            End If
            If eventInfo.Summary = "" Then
                sourceText = TokenOneParameter(sourceText, "IFNOTHASSUMMARY", True)
            Else
                sourceText = TokenOneParameter(sourceText, "IFNOTHASSUMMARY", False)
            End If

            If (sourceText.Contains("[IFENROLED]") Or sourceText.Contains("[IFNOTENROLED]")) Then
                Dim blEnroled As Boolean = False
                Dim blNotEnroled As Boolean = False
                If eventInfo.Signups Then
                    blNotEnroled = True
                    If Not UserController.GetCurrentUserInfo() Is Nothing Then
                        Dim signupsController As New EventSignupsController
                        Dim signupInfo As EventSignupsInfo = signupsController.EventsSignupsGetUser(eventInfo.EventID, UserController.GetCurrentUserInfo().UserID, eventInfo.ModuleID)
                        If Not signupInfo Is Nothing Then
                            blEnroled = signupInfo.Approved
                            blNotEnroled = Not signupInfo.Approved
                        End If
                    End If
                End If
                sourceText = TokenOneParameter(sourceText, "IFENROLED", blEnroled)
                sourceText = TokenOneParameter(sourceText, "IFNOTENROLED", blNotEnroled)
            End If

            If eventInfo.SocialUserId > 0 Then
                sourceText = TokenOneParameter(sourceText, "IFISSOCIALUSER", True)
            Else
                sourceText = TokenOneParameter(sourceText, "IFISSOCIALUSER", False)
            End If

            If eventInfo.SocialGroupId > 0 Then
                sourceText = TokenOneParameter(sourceText, "IFISSOCIALGROUP", True)
            Else
                sourceText = TokenOneParameter(sourceText, "IFISSOCIALGROUP", False)
            End If

            If eventInfo.MaxEnrollment > 0 AndAlso eventInfo.Enrolled >= eventInfo.MaxEnrollment Then
                sourceText = TokenOneParameter(sourceText, "IFISFULL", True)
            Else
                sourceText = TokenOneParameter(sourceText, "IFISFULL", False)
            End If
            If (eventInfo.MaxEnrollment > 0 AndAlso eventInfo.Enrolled < eventInfo.MaxEnrollment) OrElse eventInfo.MaxEnrollment = 0 Then
                sourceText = TokenOneParameter(sourceText, "IFNOTISFULL", True)
            Else
                sourceText = TokenOneParameter(sourceText, "IFNOTISFULL", False)
            End If

            Return sourceText
        End Function

        Public Function TokenOneParameter(ByVal sourceText As String, ByVal parameterName As String, ByVal parameterKeep As Boolean) As String
            Dim sourceTextOut As New StringBuilder
            If parameterKeep Then
                sourceTextOut.Insert(0, sourceText)
                sourceTextOut.Replace("[" + parameterName + "]", "")
                sourceTextOut.Replace("[/" + parameterName + "]", "")
            Else
                Dim rgx As New RegularExpressions.Regex("\[" + parameterName + "][.\s\S]*?\[/" + parameterName + "]")
                sourceTextOut.Insert(0, rgx.Replace(sourceText, ""))
            End If
            Return sourceTextOut.ToString
        End Function

        Private Sub TokenReplacewithPortalSettings(ByVal ps As PortalSettings, ByVal eventInfo As EventInfo, ByVal settings As EventModuleSettings, ByVal dict As Generic.Dictionary(Of String, Object), ByVal folderName As String, ByVal sourceText As String, ByVal isEventEditor As Boolean)
            'Build URL for event images
            Dim eventInfoHelper As New EventInfoHelper(ModuleID, ps.ActiveTab.TabID, eventInfo.PortalID, settings)

            ' Dim portalurl As String = ps.PortalAlias.HTTPAlias
            ' Dim domainurl As String = ps.PortalAlias.HTTPAlias
            Dim domainurl As String = eventInfoHelper.GetDomainURL()
            Dim portalurl As String = domainurl
            If ps.PortalAlias.HTTPAlias.IndexOf("/", StringComparison.Ordinal) > 0 Then
                portalurl = portalurl & Common.Globals.ApplicationPath
            End If
            Dim imagepath As String = AddHTTP(String.Format("{0}/DesktopModules/{1}/Images/", portalurl, folderName))

            'eventimage
            If settings.Eventimage And eventInfo.ImageDisplay Then
                Dim imageSrc As String = eventInfo.ImageURL

                If eventInfo.ImageURL.StartsWith("FileID=") Then
                    Dim fileId As Integer = Integer.Parse(eventInfo.ImageURL.Substring(7))
                    Dim objFileInfo As Services.FileSystem.IFileInfo = Services.FileSystem.FileManager.Instance.GetFile(fileId)
                    If Not objFileInfo Is Nothing Then
                        imageSrc = objFileInfo.Folder + objFileInfo.FileName
                        If InStr(1, imageSrc, "://") = 0 Then
                            Dim pi As New Entities.Portals.PortalController
                            imageSrc = AddHTTP(String.Format("{0}/{1}/{2}", portalurl, pi.GetPortal(eventInfo.PortalID).HomeDirectory, imageSrc))
                        End If
                    End If
                End If

                If eventInfo.ImageWidth > 0 And eventInfo.ImageHeight > 0 Then
                    dict.Add("eventimage", String.Format("<img src='{0}' alt='' width='{1}' height='{2}' />", imageSrc, Unit.Pixel(eventInfo.ImageWidth), Unit.Pixel(eventInfo.ImageHeight)))
                    Dim thumbWidth As Integer = eventInfo.ImageWidth
                    Dim thumbHeight As Integer = eventInfo.ImageHeight
                    If eventInfo.ImageHeight > settings.MaxThumbHeight Then
                        thumbHeight = settings.MaxThumbHeight
                        thumbWidth = CInt(eventInfo.ImageWidth * settings.MaxThumbHeight / eventInfo.ImageHeight)
                    End If
                    If thumbWidth > settings.MaxThumbWidth Then
                        thumbWidth = settings.MaxThumbWidth
                        thumbHeight = CInt(eventInfo.ImageHeight * settings.MaxThumbWidth / eventInfo.ImageWidth)
                    End If
                    dict.Add("eventthumb", String.Format("<img src='{0}' alt='' width='{1}' height='{2}' />", imageSrc, Unit.Pixel(thumbWidth), Unit.Pixel(thumbHeight)))
                Else
                    dict.Add("eventimage", String.Format("<img src='{0}' alt='' />", imageSrc))
                    dict.Add("eventthumb", String.Format("<img src='{0}' alt='' />", imageSrc))
                End If
                dict.Add("imageurl", imageSrc)
            Else
                '??
                dict.Add("eventimage", "")
                dict.Add("eventthumb", "")
                dict.Add("imageurl", "")
            End If

            'importancelabel
            dict.Add("importancelabel", Localization.GetString("TokenImporatanceLabel", LocalResourceFile))

            'importance, importanceicon
            Dim result As String = "<img src='{0}{1}' class=""{4}"" alt='{2}' /> {3}"
            Select Case eventInfo.Importance
                Case eventInfo.Priority.High
                    dict.Add("importance", String.Format(result, imagepath, "HighPrio.gif", Localization.GetString("HighPrio", LocalResourceFile), Localization.GetString("HighPrio", LocalResourceFile), "EventIconHigh"))
                    dict.Add("importanceicon", String.Format(result, imagepath, "HighPrio.gif", Localization.GetString("HighPrio", LocalResourceFile), "", "EventIconHigh"))
                Case eventInfo.Priority.Low
                    dict.Add("importance", String.Format(result, imagepath, "LowPrio.gif", Localization.GetString("LowPrio", LocalResourceFile), Localization.GetString("LowPrio", LocalResourceFile), "EventIconLow"))
                    dict.Add("importanceicon", String.Format(result, imagepath, "LowPrio.gif", Localization.GetString("HighPrio", LocalResourceFile), "", "EventIconLow"))
                Case eventInfo.Priority.Medium
                    dict.Add("importance", Localization.GetString("NormPrio", LocalResourceFile))
                    dict.Add("importanceicon", "")
            End Select

            'reminderlabel
            dict.Add("reminderlabel", Localization.GetString("TokenReminderLabel", LocalResourceFile))

            'reminder, remindericon
            Dim img As String
            If Not sourceText Is Nothing Then
                If (sourceText.Contains("[event:reminder]") Or sourceText.Contains("[event:remindericon]")) Then
                    Dim notificationInfo As String = ""
                    img = ""
                    Dim userEmail As String = Entities.Users.UserController.GetCurrentUserInfo().Email
                    If eventInfo.SendReminder And HttpContext.Current.Request.IsAuthenticated Then
                        Dim objEventNotificationController As EventNotificationController = New EventNotificationController
                        notificationInfo = objEventNotificationController.NotifyInfo(eventInfo.EventID, userEmail, eventInfo.ModuleID, LocalResourceFile, eventInfo.EventTimeZoneId)
                    End If
                    If eventInfo.SendReminder And HttpContext.Current.Request.IsAuthenticated And notificationInfo <> "" Then
                        img = String.Format("<img src='{0}bell.gif' class=""{2}"" alt='{1}' />", imagepath, Localization.GetString("ReminderEnabled", LocalResourceFile) + ": " + notificationInfo, "EventIconRem")
                    ElseIf eventInfo.SendReminder And (settings.notifyanon Or HttpContext.Current.Request.IsAuthenticated) Then
                        img = String.Format("<img src='{0}bell.gif' class=""{2}"" alt='{1}' />", imagepath, Localization.GetString("ReminderEnabled", LocalResourceFile), "EventIconRem")
                    End If
                    dict.Add("reminder", notificationInfo)
                    dict.Add("remindericon", img)
                End If
            End If
            'enrollicon
            Dim objEventSignupsController As New EventSignupsController
            img = ""
            If objEventSignupsController.DisplayEnrollIcon(eventInfo) And settings.eventsignup Then
                If eventInfo.MaxEnrollment = 0 Or eventInfo.Enrolled < eventInfo.MaxEnrollment Then
                    img = String.Format("<img src='{0}enroll.gif' class=""{2}"" alt='{1}' />", imagepath, Localization.GetString("EnrollEnabled", LocalResourceFile), "EventIconEnroll")
                Else
                    img = String.Format("<img src='{0}EnrollFull.gif' class=""{2}"" alt='{1}' />", imagepath, Localization.GetString("EnrollFull", LocalResourceFile), "EventIconEnrollFull")
                End If
            End If
            dict.Add("enrollicon", img)

            'recurringlabel
            dict.Add("recurringlabel", Localization.GetString("TokenRecurranceLabel", LocalResourceFile))

            'recurring, recurringicon
            Dim objEventRRULE As EventRRULEInfo
            Dim objCtlEventRecurMaster As New EventRecurMasterController
            objEventRRULE = objCtlEventRecurMaster.DecomposeRRULE(eventInfo.RRULE, eventInfo.EventTimeBegin)
            result = objCtlEventRecurMaster.RecurrenceText(objEventRRULE, LocalResourceFile, Threading.Thread.CurrentThread.CurrentCulture, eventInfo.EventTimeBegin)
            img = ""
            If eventInfo.RRULE <> "" Then
                img = String.Format("<img src='{0}rec.gif' class=""{2}"" alt='{1}' />", imagepath, Localization.GetString("RecurringEvent", LocalResourceFile), "EventIconRec")
                result = img & " " & result & " " & objCtlEventRecurMaster.RecurrenceInfo(eventInfo, LocalResourceFile)
            End If
            dict.Add("recurring", result)
            dict.Add("recurringicon", img)

            'titleurl
            Dim eventurl As String = eventInfoHelper.DetailPageURL(eventInfo)
            dict.Add("eventurl", eventurl)
            If eventInfo.DetailPage And eventInfo.DetailNewWin Then
                dict.Add("titleurl", "<a href=""" & eventurl & """ target=""_blank"">" & eventInfo.EventName & "</a>")
            Else
                dict.Add("titleurl", "<a href=""" & eventurl & """>" & eventInfo.EventName & "</a>")
            End If

            'View page url
            If settings.DetailPageAllowed And eventInfo.DetailPage Then
                Dim strUserID As String = Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString
                Dim userID As Integer = -1
                If IsNumeric(strUserID) Then
                    userID = CInt(strUserID)
                End If
                Dim blAuthenticated As Boolean = False
                If userID > -1 Then
                    blAuthenticated = True
                End If
                If eventInfo.CreatedByID = userID Or eventInfo.OwnerID = userID Or eventInfo.RmOwnerID = userID Or _
                    eventInfoHelper.IsModerator(blAuthenticated) = True Or _
                    Security.PortalSecurity.IsInRole(ps.AdministratorRoleName.ToString) Then
                    Dim imgurl As String = Entities.Icons.IconController.IconURL("View")
                    img = String.Format("<a href='{0}'><img src='{1}' border=""0"" alt=""{2}"" title=""{2}"" /></a>", eventInfoHelper.GetDetailPageRealURL(eventInfo.EventID, eventInfo.SocialGroupId, eventInfo.SocialUserId), imgurl, Localization.GetString("ViewEvent", LocalResourceFile))
                    dict.Add("viewicon", img)
                End If
            Else
                dict.Add("viewicon", "")
            End If

            'Createdby
            'TokenCreatedByLabel.Text   Created by, Created by ID, Created by Link
            Dim objEventUser As EventUser = eventInfoHelper.UserDisplayNameProfile(eventInfo.CreatedByID, eventInfo.CreatedBy, LocalResourceFile)
            dict.Add("createdbylabel", Localization.GetString("TokenCreatedByLabel", LocalResourceFile))
            dict.Add("createdby", objEventUser.DisplayName)
            dict.Add("createdbyid", objEventUser.UserID)
            dict.Add("createdbyurl", objEventUser.ProfileURL)
            dict.Add("createdbyprofile", objEventUser.DisplayNameURL)

            'ownedby
            'TokenOwnedByLabel.Text   Owned by, OwnerID, Owned by Link
            objEventUser = eventInfoHelper.UserDisplayNameProfile(eventInfo.OwnerID, eventInfo.OwnerName, LocalResourceFile)
            dict.Add("ownedbylabel", Localization.GetString("TokenOwnedByLabel", LocalResourceFile))
            dict.Add("ownedby", objEventUser.DisplayName)
            dict.Add("ownedbyid", objEventUser.UserID)
            dict.Add("ownedbyurl", objEventUser.ProfileURL)
            dict.Add("ownedbyprofile", objEventUser.DisplayNameURL)

            'LastUpdatedby
            'TokenLastUpdatedByLabel.Text   Last updated by, Last updated ID, Last update by ID
            objEventUser = eventInfoHelper.UserDisplayNameProfile(eventInfo.LastUpdatedID, eventInfo.LastUpdatedBy, LocalResourceFile)
            dict.Add("lastupdatedbylabel", Localization.GetString("TokenLastUpdatedByLabel", LocalResourceFile))
            dict.Add("lastupdatedby", objEventUser.DisplayName)
            dict.Add("lastupdatedbyid", objEventUser.UserID)
            dict.Add("lastupdatedbyurl", objEventUser.ProfileURL)
            dict.Add("lastupdatedbyprofile", objEventUser.DisplayNameURL)

            If settings.eventsignup And eventInfo.Signups Then
                'enrollfeelabel
                'enrollfee
                dict.Add("enrollfeelabel", Localization.GetString("TokenEnrollFeeLabel", LocalResourceFile))
                If eventInfo.EnrollType = "PAID" Then
                    Dim tokenEnrollFeePaid As String = Replace(Localization.GetString("TokenEnrollFeePaid", LocalResourceFile), "{0}", "{0:#0.00}")
                    dict.Add("enrollfee", String.Format(tokenEnrollFeePaid, eventInfo.EnrollFee, ps.Currency))
                Else
                    dict.Add("enrollfee", Localization.GetString("TokenEnrollFeeFree", LocalResourceFile))
                End If
            Else
                dict.Add("enrollfeelabel", "")
                dict.Add("enrollfee", "")
            End If

            If settings.moderateall Then
                dict.Add("moderateurl", eventInfoHelper.GetModerateUrl)
            Else
                dict.Add("moderateurl", "")
            End If

            'edit button
            If IsEventEditor Then
                Dim imgurl As String = Entities.Icons.IconController.IconURL("Edit")
                img = String.Format("<a href=""{3}""><img src='{0}' class=""{2}"" alt='{1}' title='{1}' /></a>", imgurl, Localization.GetString("EditEvent", LocalResourceFile), "EventIconEdit", eventInfoHelper.GetEditURL(eventInfo.EventID, eventInfo.SocialGroupId, eventInfo.SocialUserId))
                dict.Add("editbutton", img)
            Else
                dict.Add("editbutton", "")
            End If
        End Sub
#End Region

#Region "Helperfunctions"
        Private Structure GetTokenFormatResult
            Public Formatstring As String
            Public Tokenfound As Boolean
        End Structure

        Private Function GetTokenFormat(ByVal tokenstring As String, ByVal token As String, ByVal customcaption As String) As GetTokenFormatResult
            Dim search1 As String = String.Format("[{0}:{1}]", customcaption, token)
            Dim search2 As String = String.Format("[{0}:{1}|", customcaption, token)
            Dim starttoken1 As Int32 = tokenstring.IndexOf(search1, StringComparison.Ordinal)
            Dim starttoken2 As Int32 = tokenstring.IndexOf(search2, StringComparison.Ordinal)
            If starttoken1 = -1 And starttoken2 = -1 Then
                'Not found
                Return New GetTokenFormatResult() With {.formatstring = Nothing, .tokenfound = False}
            End If

            Dim result As New GetTokenFormatResult
            result.tokenfound = True
            If starttoken1 = -1 Then
                Dim endtoken As Int32 = tokenstring.Substring(starttoken2).IndexOf("]", StringComparison.Ordinal)
                result.formatstring = (tokenstring.Substring(starttoken2 + search2.Length, endtoken - search2.Length))
            Else
                result.formatstring = ""
            End If

            Return result
        End Function

#End Region
    End Class
#End Region

End Namespace