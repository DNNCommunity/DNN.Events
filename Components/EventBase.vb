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
Imports DotNetNuke.Security
Imports System.Globalization
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Modules.Events.Components.Integration

Namespace DotNetNuke.Modules.Events
    Public Class EventBase
        Inherits PortalModuleBase

#Region "Properties"
        Private _selectedDate As Date
        Private _currculture As CultureInfo
        Public Property SelectedDate() As Date
            Get
                Dim objEventTimeZoneUtilities As New EventTimeZoneUtilities
                Dim currDateInfo As EventTimeZoneUtilities.DateInfo = objEventTimeZoneUtilities.ConvertFromUTCToDisplayTimeZone(Date.UtcNow, GetDisplayTimeZoneId())
                Try
                    _currculture = Threading.Thread.CurrentThread.CurrentCulture
                    If _selectedDate.Year = 1 Then
                        If Not Request.Params("selecteddate") = Nothing Then
                            Dim strDate As String = Request.Params("selecteddate")
                            If IsDate(strDate) Then
                                _selectedDate = CType(strDate, Date)
                            Else
                                Dim invCulture As CultureInfo = CultureInfo.InvariantCulture
                                Try
                                    _selectedDate = Date.ParseExact(strDate, "yyyyMMdd", invCulture)
                                Catch ex As Exception
                                    _selectedDate = currDateInfo.EventDate
                                End Try
                            End If
                        ElseIf Request.Cookies("DNNEvents") Is Nothing Then
                            _selectedDate = currDateInfo.EventDate
                        ElseIf Request.Cookies("DNNEvents")("EventSelectedDate" & ModuleId) Is Nothing Then
                            _selectedDate = currDateInfo.EventDate
                        Else
                            Dim cookieDate As String = Request.Cookies("DNNEvents")("EventSelectedDate" & ModuleId)
                            Threading.Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US", False)
                            If CType(cookieDate, Date).Year = 1 Then
                                _selectedDate = currDateInfo.EventDate
                            Else
                                _selectedDate = CType(cookieDate, Date)
                            End If
                            Threading.Thread.CurrentThread.CurrentCulture = _currculture
                        End If
                    End If
                    Return _selectedDate
                Catch exc As Exception
                    Threading.Thread.CurrentThread.CurrentCulture = _currculture
                    _selectedDate = currDateInfo.EventDate
                    Return _selectedDate
                End Try
            End Get
            Set(ByVal value As Date)
                If IsDate(value) Then
                    _selectedDate = CType(value.ToShortDateString, Date)
                Else
                    Dim objEventTimeZoneUtilities As New EventTimeZoneUtilities
                    _selectedDate = objEventTimeZoneUtilities.ConvertFromUTCToDisplayTimeZone(Date.UtcNow, GetDisplayTimeZoneId()).EventDate
                End If
                If _selectedDate.Year = 1 Then
                    Dim objEventTimeZoneUtilities As New EventTimeZoneUtilities
                    _selectedDate = objEventTimeZoneUtilities.ConvertFromUTCToDisplayTimeZone(Date.UtcNow, GetDisplayTimeZoneId()).EventDate
                End If
                _currculture = Threading.Thread.CurrentThread.CurrentCulture
                Threading.Thread.CurrentThread.CurrentCulture = New CultureInfo("en-US", False)
                Response.Cookies("DNNEvents")("EventSelectedDate" & ModuleId) = _selectedDate.ToShortDateString
                Response.Cookies("DNNEvents").Expires = DateTime.Now.AddMinutes(2)
                Response.Cookies("DNNEvents").Path = "/"
                Threading.Thread.CurrentThread.CurrentCulture = _currculture
            End Set
        End Property

        Public ReadOnly Property BasePage() As CDefault
            Get
                Return CType(Page, CDefault)
            End Get
        End Property

#End Region

#Region "Public Routines"
        Public Function IsModerator() As Boolean
            If PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString) Then
                Return True
            End If
            Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
            Return objEventInfoHelper.IsModerator(Request.IsAuthenticated)
        End Function

        Public Function IsModuleEditor() As Boolean
            Dim blHasBasePermissions As Boolean = False
            Try
                Dim mc As New ModuleController
                Dim objMod As ModuleInfo
                Dim mp As Permissions.ModulePermissionCollection

                objMod = mc.GetModule(ModuleId, TabId, False)

                If Not objMod Is Nothing Then
                    mp = objMod.ModulePermissions
                    If Permissions.ModulePermissionController.HasModulePermission(mp, "EVENTSEDT") Then
                        blHasBasePermissions = True
                    ElseIf Permissions.ModulePermissionController.HasModulePermission(mp, "EDIT") Then
                        blHasBasePermissions = True
                    End If
                End If

            Catch
            End Try
            If blHasBasePermissions And Settings.SocialGroupModule = EventModuleSettings.SocialModule.SocialGroup And Settings.SocialGroupSecurity <> EventModuleSettings.SocialGroupPrivacy.OpenToAll Then
                Dim socialGroupID As Integer = GetUrlGroupId()
                If socialGroupID > -1 Then
                    Dim objRoleCtl As New RoleController
                    Dim objRoleInfo As RoleInfo = objRoleCtl.GetRole(socialGroupID, PortalSettings.PortalId)
                    If Not objRoleInfo Is Nothing Then
                        If Not PortalSettings.UserInfo.IsInRole(objRoleInfo.RoleName) Then
                            Return False
                        End If
                    End If
                End If
            End If
            Return blHasBasePermissions
        End Function

        Public Function IsEventEditor(ByVal objEvent As EventInfo, ByVal blMasterOwner As Boolean) As Boolean
            Try
                If (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString) = True) Or _
                   (IsModuleEditor() And (objEvent.CreatedByID = UserId Or (objEvent.OwnerID = UserId And Not blMasterOwner) Or objEvent.RMOwnerID = UserId)) Or _
                   (IsModerator() = True) Then
                    Return True
                Else
                    Return False
                End If
            Catch
            End Try
            Return False
        End Function

        Public ReadOnly Property IsPrivateNotModerator() As Boolean
            Get
                If Settings.PrivateMessage <> "" And Not IsModerator() Then
                    IsPrivateNotModerator = True
                Else
                    IsPrivateNotModerator = False
                End If
            End Get
        End Property

        Public Function IsCategoryEditor() As Boolean
            If Request.IsAuthenticated Then
                If PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString) Then
                    Return True
                End If
                Try
                    Dim mc As New ModuleController
                    Dim objMod As ModuleInfo
                    Dim mp As Permissions.ModulePermissionCollection

                    objMod = mc.GetModule(ModuleId, TabId, False)

                    If Not objMod Is Nothing Then
                        mp = objMod.ModulePermissions
                        Return Permissions.ModulePermissionController.HasModulePermission(mp, "EVENTSCAT")
                    Else
                        Return False
                    End If

                Catch
                End Try
            End If
            Return False
        End Function

        Public Function IsLocationEditor() As Boolean
            If Request.IsAuthenticated Then
                If PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString) Then
                    Return True
                End If
                Try
                    Dim mc As New ModuleController
                    Dim objMod As ModuleInfo
                    Dim mp As Permissions.ModulePermissionCollection

                    objMod = mc.GetModule(ModuleId, TabId, False)

                    If Not objMod Is Nothing Then
                        mp = objMod.ModulePermissions
                        Return Permissions.ModulePermissionController.HasModulePermission(mp, "EVENTSLOC")
                    Else
                        Return False
                    End If

                Catch
                End Try
            End If
            Return False
        End Function

        Public Function IsSettingsEditor() As Boolean
            If Request.IsAuthenticated Then
                If PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName.ToString) Then
                    Return True
                End If
                Try
                    Dim mc As New ModuleController
                    Dim objMod As ModuleInfo
                    Dim mp As Permissions.ModulePermissionCollection

                    objMod = mc.GetModule(ModuleId, TabId, False)

                    If Not objMod Is Nothing Then
                        mp = objMod.ModulePermissions
                        Return Permissions.ModulePermissionController.HasModulePermission(mp, "EVENTSSET")
                    Else
                        Return False
                    End If

                Catch
                End Try
            End If
            Return False
        End Function

        Public Function GetModerators() As ArrayList
            Dim moderators As New ArrayList
            Dim objCollModulePermission As Permissions.ModulePermissionCollection
            objCollModulePermission = Permissions.ModulePermissionController.GetModulePermissions(ModuleId, TabId)
            Dim objModulePermission As Permissions.ModulePermissionInfo

            ' To cope with host users or someone who is no longer an editor!!

            For Each objModulePermission In objCollModulePermission
                If objModulePermission.PermissionKey = "EVENTSMOD" Then
                    If objModulePermission.UserID < 0 Then
                        Dim objCtlRole As New RoleController
                        If objModulePermission.RoleID <> PortalSettings.AdministratorRoleId Then
                            Dim lstUsers As ArrayList = objCtlRole.GetUsersByRoleName(PortalId, objModulePermission.RoleName)
                            Dim objUser As UserInfo
                            For Each objUser In lstUsers
                                If Not moderators.Contains(objUser) Then
                                    moderators.Add(objUser)
                                End If
                            Next
                        End If
                    Else
                        Dim objUserCtl As New UserController
                        Dim objUser As UserInfo = objUserCtl.GetUser(PortalId, objModulePermission.UserID)
                        If Not objUser Is Nothing Then
                            If Not moderators.Contains(objUser.Email) Then
                                moderators.Add(objUser.Email)
                            End If
                        End If
                    End If
                End If
            Next
            Return moderators
        End Function

        Public Function ImportanceColor(ByVal iImportance As Integer) As Color
            Select Case iImportance
                Case 1
                    Return Color.Red
                Case 2
                    Return Color.Blue
                Case 3
                    Return Color.Black
            End Select
        End Function

        Public Function GetColor(ByVal categoryColor As String) As Color
            Dim cc As ColorConverter = New ColorConverter
            Return CType(cc.ConvertFromString(categoryColor), Color)
        End Function

        ''' <summary>
        ''' Attach a theme css to the supplied panel
        ''' </summary>
        ''' <param name="ctlPnlTheme"></param>
        ''' <remarks></remarks>
        Public Sub SetTheme(ByVal ctlPnlTheme As System.Web.UI.WebControls.Panel)
            Dim themeSettings As ThemeSetting = GetThemeSettings()

            Dim cssLink As New HtmlLink()
            With cssLink
                .Href = themeSettings.ThemeFile
                .Attributes.Add("rel", "stylesheet")
                .Attributes.Add("type", "text/css")
            End With
            Dim added As Boolean = False
            For Each pagecontrol As Object In Page.Header.Controls
                If TypeOf (pagecontrol) Is System.Web.UI.WebControls.PlaceHolder Then
                    Dim placeholder As System.Web.UI.WebControls.PlaceHolder = CType(pagecontrol, System.Web.UI.WebControls.PlaceHolder)
                    If placeholder.ID = "CSS" Then
                        Dim insertat As Integer = 1
                        For Each placeholdercontrol As Object In placeholder.Controls
                            If TypeOf (placeholdercontrol) Is HtmlLink Then
                                Dim htmllink As HtmlLink = CType(placeholdercontrol, HtmlLink)
                                If htmllink.Href.ToLower.Contains("desktopmodules/events/module.css") Then
                                    placeholder.Controls.AddAt(insertat, cssLink)
                                    added = True
                                    Exit For
                                End If
                            End If
                            insertat += 1
                        Next
                        If added Then
                            Exit For
                        End If
                    End If
                End If
            Next
            If Not added Then
                Page.Header.Controls.Add(cssLink)
            End If

            ctlPnlTheme.CssClass = themeSettings.CssClass
        End Sub

        Public Function GetThemeSettings() As ThemeSetting
            Dim themeSettings As New ThemeSetting
            If themeSettings.ValidateSetting(Settings.EventTheme) = False Then
                themeSettings.ReadSetting(Settings.EventThemeDefault, PortalId)
            ElseIf Settings.EventTheme <> "" Then
                themeSettings.ReadSetting(Settings.EventTheme, PortalId)
            End If
            Return themeSettings
        End Function

        Public Sub AddFacebookMetaTags()
            If Settings.FBAdmins <> "" Then
                Dim fbMeta As New HtmlMeta
                fbMeta.Name = "fb:admins"
                fbMeta.Content = Settings.FBAdmins
                Page.Header.Controls.Add(fbMeta)
            End If

            If Settings.FBAppID <> "" Then
                Dim fbMeta As New HtmlMeta
                fbMeta.Name = "fb:app_id"
                fbMeta.Content = Settings.FBAppID
                Page.Header.Controls.Add(fbMeta)
            End If

        End Sub

        Public Function ImageInfo(ByVal imageUrl As String, ByVal imageHeight As Integer, ByVal imageWidth As Integer) As String
            Dim imagestring As String = ""
            Dim imageSrc As String
            If imageUrl.StartsWith("FileID=") Then
                Dim objFile As IFileInfo
                Dim fileId As Integer = Integer.Parse(imageUrl.Substring(7))
                objFile = FileManager.Instance.GetFile(fileId)
                If Not objFile Is Nothing Then
                    imageSrc = objFile.Folder + objFile.FileName.Replace(" ", "%20")
                    If InStr(1, imageSrc, "://") = 0 Then
                        imageSrc = PortalSettings.HomeDirectory & imageSrc
                    End If
                    imagestring = ConvertToThumb(imageSrc, imageWidth, imageHeight)
                End If
            ElseIf imageUrl.StartsWith("http") Then
                imageSrc = imageUrl
                imagestring = ConvertToThumb(imageSrc, imageWidth, imageHeight)
            End If
            Return imagestring
        End Function

        Private Function ConvertToThumb(ByVal imageSrc As String, ByVal imageWidth As Integer, ByVal imageHeight As Integer) As String
            Dim imagestring As String
            If imageWidth > 0 And imageHeight > 0 Then
                Dim thumbWidth As Integer = imageWidth
                Dim thumbHeight As Integer = imageHeight
                If imageHeight > Settings.MaxThumbHeight Then
                    thumbHeight = Settings.MaxThumbHeight
                    thumbWidth = CInt(imageWidth * Settings.MaxThumbHeight / imageHeight)
                End If
                If thumbWidth > Settings.MaxThumbWidth Then
                    thumbWidth = Settings.MaxThumbWidth
                    thumbHeight = CInt(imageHeight * Settings.MaxThumbWidth / imageWidth)
                End If
                imagestring = "<img src=""" + imageSrc + """ border=""0"" width=""" & thumbWidth.ToString & """ height=""" & thumbHeight.ToString & """ align=""middle"" alt="""" /><br />"
            Else
                imagestring = "<img src=""" + imageSrc + """ border=""0"" align=""middle"" alt="""" /><br />"
            End If
            Return imagestring
        End Function

        Public Function DetailPageEdit(ByVal objEvent As EventInfo) As String
            Dim editString As String = ""
            If IsEventEditor(objEvent, False) Then
                Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
                Dim imgurl As String = Entities.Icons.IconController.IconURL("View")
                editString = "<a href='" + objEventInfoHelper.GetDetailPageRealURL(objEvent.EventID, objEvent.SocialGroupId, objEvent.SocialUserId) + "'><img src=""" + imgurl + """ border=""0"" alt=""" + Localization.GetString("ViewEvent", LocalResourceFile) + """ title=""" + Localization.GetString("ViewEvent", LocalResourceFile) + """ /></a>"
            End If
            Return editString
        End Function

        'EVT-4499 Redirect to login page with url parameter returnurl
        Public Sub RedirectToLogin()

            Dim returnUrl As String = HttpContext.Current.Request.RawUrl
            If returnUrl.IndexOf("?returnurl=", StringComparison.Ordinal) <> -1 Then
                returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("?returnurl=", StringComparison.Ordinal))
            End If
            returnUrl = HttpUtility.UrlEncode(returnUrl)

            Response.Redirect(LoginURL(returnUrl, (Request.QueryString("override") IsNot Nothing)), True)

        End Sub

        Public Sub SetUpIconBar(ByVal eventIcons As EventIcons, ByVal eventIcons2 As EventIcons)
            eventIcons.Visible = False
            eventIcons2.Visible = False
            eventIcons.ModuleConfiguration = ModuleConfiguration.Clone
            eventIcons2.ModuleConfiguration = ModuleConfiguration.Clone
            Select Case Settings.IconBar
                Case "TOP"
                    eventIcons.Visible = True
                Case "BOTTOM"
                    eventIcons2.Visible = True
            End Select
        End Sub

        Public Sub SendNewEventEmails(ByVal objEvent As EventInfo)
            If Not objEvent.Approved Then
                Return
            End If
            Dim objEventEmailInfo As New EventEmailInfo
            Dim objEventEmail As New EventEmails(PortalId, ModuleId, LocalResourceFile, CType(Page, PageBase).PageCulture.Name)
            objEventEmailInfo.txtEmailSubject = Settings.Templates.txtNewEventEmailSubject
            objEventEmailInfo.txtEmailBody = Settings.Templates.txtNewEventEmailMessage
            objEventEmailInfo.txtEmailFrom() = Settings.StandardEmail
            Select Case Settings.neweventemails
                Case "Subscribe"
                    ' Email Subscribed Users

                    Dim objEventSubscriptionController As New EventSubscriptionController
                    Dim lstSubscriptions As ArrayList
                    Dim objEventSubscription As EventSubscriptionInfo
                    lstSubscriptions = objEventSubscriptionController.EventsSubscriptionGetSubModule(ModuleId)

                    If lstSubscriptions.Count = 0 Then
                        Return
                    End If

                    Dim objEventInfo As New EventInfoHelper(ModuleId, TabId, PortalId, Nothing)
                    Dim lstusers As ArrayList = objEventInfo.GetEventModuleViewers

                    For Each objEventSubscription In lstSubscriptions
                        If Not lstusers.Contains(objEventSubscription.UserID) Then
                            Dim objCtlUser As New UserController
                            Dim objUser As UserInfo = objCtlUser.GetUser(PortalId, objEventSubscription.UserID)
                            If Not objUser Is Nothing And objUser.IsSuperUser Then
                                objEventEmailInfo.UserEmails.Add(objUser.Email)
                                objEventEmailInfo.UserLocales.Add(objUser.Profile.PreferredLocale)
                                objEventEmailInfo.UserTimeZoneIds.Add(objUser.Profile.PreferredTimeZone.Id)
                            Else
                                Continue For
                            End If
                        Else
                            objEventEmailInfo.UserIDs.Add(objEventSubscription.UserID)
                        End If

                    Next
                Case "Role"
                    ' Email users in role
                    EventEmailAddRoleUsers(Settings.neweventemailrole, objEventEmailInfo)
                Case Else
                    Return
            End Select
            objEventEmail.SendEmails(objEventEmailInfo, objEvent)

        End Sub

        Public Sub EventEmailAddRoleUsers(ByVal roleId As Integer, ByVal objEventEmailInfo As EventEmailInfo)
            Dim objRoleController As New RoleController
            Dim objRole As RoleInfo = objRoleController.GetRole(roleId, PortalId)
            If Not objRole Is Nothing Then
                Dim lstUsers As ArrayList = objRoleController.GetUsersByRoleName(PortalId, objRole.RoleName)
                For Each objUser As UserInfo In lstUsers
                    objEventEmailInfo.UserEmails.Add(objUser.Email)
                    objEventEmailInfo.UserLocales.Add(objUser.Profile.PreferredLocale)
                    objEventEmailInfo.UserTimeZoneIds.Add(objUser.Profile.PreferredTimeZone.Id)
                Next
            End If
        End Sub

        Public Sub CreateNewEventJournal(ByVal objEvent As EventInfo)
            If Not Settings.JournalIntegration Then
                Exit Sub
            End If

            If objEvent.Approved Then
                Dim cntJournal As New Journal
                Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
                Dim url As String = objEventInfoHelper.DetailPageURL(objEvent)
                Dim imageSrc As String = Nothing
                If Settings.eventimage And objEvent.ImageDisplay Then
                    Dim portalurl As String = objEventInfoHelper.GetDomainURL()
                    If PortalSettings.PortalAlias.HTTPAlias.IndexOf("/", StringComparison.Ordinal) > 0 Then
                        portalurl = portalurl & Common.Globals.ApplicationPath
                    End If
                    imageSrc = objEvent.ImageURL
                    If objEvent.ImageURL.StartsWith("FileID=") Then
                        Dim fileId As Integer = Integer.Parse(objEvent.ImageURL.Substring(7))
                        Dim objFileInfo As IFileInfo = FileManager.Instance.GetFile(fileId)
                        If Not objFileInfo Is Nothing Then
                            imageSrc = objFileInfo.Folder + objFileInfo.FileName
                            If InStr(1, imageSrc, "://") = 0 Then
                                Dim pi As New Entities.Portals.PortalController
                                imageSrc = AddHTTP(String.Format("{0}/{1}/{2}", portalurl, pi.GetPortal(objEvent.PortalID).HomeDirectory, imageSrc))
                            End If
                        End If
                    End If
                End If

                cntJournal.NewEvent(objEvent, TabId, url, imageSrc)

                ' Update event to show it has an associated JournalItem
                Dim cntEvent As New EventController
                objEvent.JournalItem = True
                cntEvent.EventsSave(objEvent, True, TabId, False)
            End If
        End Sub

        Public Sub CreateEnrollmentJournal(ByVal objEventSignup As EventSignupsInfo, ByVal objEvent As EventInfo, ByVal enrollSettings As EventModuleSettings)
            If Not enrollSettings.JournalIntegration Then
                Exit Sub
            End If

            If objEventSignup.Approved And objEventSignup.UserID > -1 Then
                Dim modTab As Integer = TabId
                If modTab = -1 Then
                    Dim cntModule As New ModuleController
                    modTab = cntModule.GetModule(objEvent.ModuleID).TabID
                End If

                Dim objEventInfoHelper As New EventInfoHelper(objEvent.ModuleID, modTab, objEvent.PortalID, enrollSettings)
                Dim url As String = objEventInfoHelper.DetailPageURL(objEvent)

                Dim creatorUserid As Integer = UserId
                If creatorUserid = -1 Then
                    creatorUserid = objEventSignup.UserID
                End If

                Dim cntJournal As New Journal
                cntJournal.NewEnrollment(objEventSignup, objEvent, modTab, url, creatorUserid)
            End If
        End Sub

        Public Function CreateEnrollment(ByVal objEventSignup As EventSignupsInfo, ByVal objEvent As EventInfo) As EventSignupsInfo
            Return CreateEnrollment(objEventSignup, objEvent, Settings)
        End Function

        Public Function CreateEnrollment(ByVal objEventSignup As EventSignupsInfo, ByVal objEvent As EventInfo, ByVal enrollSettings As EventModuleSettings) As EventSignupsInfo
            Dim objCtlEventSignups As New EventSignupsController
            If objEventSignup.SignupID = 0 Then
                objEventSignup = objCtlEventSignups.EventsSignupsSave(objEventSignup)
            Else
                objCtlEventSignups.EventsSignupsSave(objEventSignup)
            End If
            CreateEnrollmentJournal(objEventSignup, objEvent, enrollSettings)
            Return objEventSignup
        End Function

        Public Sub DeleteEnrollment(ByVal signupId As Integer, ByVal inModuleId As Integer, ByVal eventId As Integer)
            Dim objCtlEventSignups As New EventSignupsController
            objCtlEventSignups.EventsSignupsDelete(signupId, inModuleId)
            Dim cntJournal As New Journal
            cntJournal.DeleteEnrollment(inModuleId, eventId, signupId, PortalId)
        End Sub

        Public Function ToolTipCreate(ByVal objEvent As EventInfo, ByVal templateTitle As String, ByVal templateBody As String, ByVal isEvtEditor As Boolean) As String
            Dim themeCss As String = GetThemeSettings().CssClass

            Dim tr As New TokenReplaceControllerClass(ModuleId, LocalResourceFile)

            ' Add sub module name if a sub-calendar
            Dim blAddSubModuleName As Boolean = False
            If objEvent.ModuleID <> ModuleId And objEvent.ModuleTitle <> Nothing And Settings.addsubmodulename Then
                blAddSubModuleName = True
            End If

            Dim tooltipTitle As String = tr.TokenReplaceEvent(objEvent, templateTitle.Replace(vbLf, "").Replace(vbCr, ""), blAddSubModuleName)
            Dim tooltipBody As String = tr.TokenReplaceEvent(objEvent, templateBody.Replace(vbLf, "").Replace(vbCr, ""), Nothing, False, isEvtEditor)

            ' Shorten to maximum length
            Dim intTooltipLength As Integer = Settings.eventtooltiplength
            tooltipBody = HtmlUtils.Shorten(HttpUtility.HtmlDecode(tooltipBody).Replace(Environment.NewLine, " "), intTooltipLength, "...").Replace("[", "&#91;").Replace("]", "&#93;")
            Dim tooltip As String = "<table class=""" & themeCss & " Eventtooltiptable"" cellspacing=""0""><tr><td class=""" & themeCss & " Eventtooltipheader"">" + tooltipTitle + "</td></tr><tr><td class=""" & themeCss & " Eventtooltipbody"">" + tooltipBody + "</td></tr></table>"
            Return tooltip

        End Function

        Public Function EnrolmentColumns(ByVal eventInfo As EventInfo, ByVal enrollListView As Boolean) As String
            Dim txtColumns As String = ""
            If Settings.eventsignup And enrollListView Then
                If IsEventEditor(eventInfo, False) Then
                    If Settings.EnrollEditFields <> "" Or Settings.EnrollViewFields <> "" Or Settings.EnrollAnonFields <> "" Then
                        txtColumns = Settings.EnrollEditFields + ";" + Settings.EnrollViewFields + ";" + Settings.EnrollAnonFields
                    End If
                ElseIf Request.IsAuthenticated Then
                    If Settings.EnrollViewFields <> "" Or Settings.EnrollAnonFields <> "" Then
                        txtColumns = Settings.EnrollViewFields + ";" + Settings.EnrollAnonFields
                    End If
                Else
                    If Settings.EnrollAnonFields <> "" Then
                        txtColumns = Settings.EnrollAnonFields
                    End If
                End If
            End If
            txtColumns = txtColumns.Replace("01", "UserName")
            txtColumns = txtColumns.Replace("02", "DisplayName")
            txtColumns = txtColumns.Replace("03", "Email")
            txtColumns = txtColumns.Replace("04", "Phone")
            txtColumns = txtColumns.Replace("05", "Approved")
            txtColumns = txtColumns.Replace("06", "Qty")

            Return txtColumns

        End Function

        Public Sub CreateThemeDirectory()
            'Create theme folder if needed
            Dim pc As New Entities.Portals.PortalController
            With pc.GetPortal(PortalId)
                Dim eventSkinPath As String = String.Format("{0}\DNNEvents\Themes", .HomeDirectoryMapPath)
                If Not IO.Directory.Exists(eventSkinPath) Then
                    IO.Directory.CreateDirectory(eventSkinPath)
                End If
            End With
        End Sub

        Public Function HideFullEvent(ByVal objevent As EventInfo) As Boolean
            Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
            Return objEventInfoHelper.HideFullEvent(objevent, Settings.eventhidefullenroll(), UserId, Request.IsAuthenticated)
        End Function

        Public Function CreateIconString(ByVal objEvent As EventInfo, ByVal iconPrio As Boolean, ByVal iconRec As Boolean, ByVal iconReminder As Boolean, ByVal iconEnroll As Boolean) As String
            Dim objCtlEventRecurMaster As New EventRecurMasterController
            Dim iconString As String = ""
            If iconPrio Then
                Select Case objEvent.Importance.ToString
                    Case "High"
                        iconString = "<img src=""" + ResolveUrl("Images/HighPrio.gif") + """ class=""EventIconHigh"" alt=""" + Localization.GetString("HighPrio", LocalResourceFile) + """ title=""" + Localization.GetString("HighPrio", LocalResourceFile) + """ /> "
                    Case "Low"
                        iconString = "<img src=""" + ResolveUrl("Images/LowPrio.gif") + """ class=""EventIconLow"" alt=""" + Localization.GetString("LowPrio", LocalResourceFile) + """ title=""" + Localization.GetString("LowPrio", LocalResourceFile) + """ /> "
                End Select
            End If
            If objEvent.RRULE <> "" And iconRec Then
                iconString = iconString + "<img src=""" + ResolveUrl("Images/rec.gif") + """ class=""EventIconRec"" alt=""" + Localization.GetString("RecurringEvent", LocalResourceFile) + ": " + objCtlEventRecurMaster.RecurrenceInfo(objEvent, LocalResourceFile) + """ title=""" + Localization.GetString("RecurringEvent", LocalResourceFile) + ": " + objCtlEventRecurMaster.RecurrenceInfo(objEvent, LocalResourceFile) + """ /> "
            End If

            Dim notificationInfo As String = ""
            If objEvent.SendReminder And iconReminder And Request.IsAuthenticated() Then
                Dim objEventNotificationController As EventNotificationController = New EventNotificationController
                notificationInfo = objEventNotificationController.NotifyInfo(objEvent.EventID, UserInfo.Email, objEvent.ModuleID, LocalResourceFile, GetDisplayTimeZoneId())
            End If
            If objEvent.SendReminder And iconReminder And Request.IsAuthenticated And notificationInfo <> "" Then
                iconString = iconString + "<img src=""" + ResolveUrl("Images/bell.gif") + """ class=""EventIconRem"" alt=""" + Localization.GetString("ReminderEnabled", LocalResourceFile) + ": " + notificationInfo + """ title=""" + Localization.GetString("ReminderEnabled", LocalResourceFile) + ": " + notificationInfo + """ /> "
            ElseIf objEvent.SendReminder And iconReminder And (Settings.notifyanon Or Request.IsAuthenticated) Then
                iconString = iconString + "<img src=""" + ResolveUrl("Images/bell.gif") + """ class=""EventIconRem"" alt=""" + Localization.GetString("ReminderEnabled", LocalResourceFile) + """ title=""" + Localization.GetString("ReminderEnabled", LocalResourceFile) + """ /> "
            End If

            Dim objEventSignupsController As New EventSignupsController
            If iconEnroll And objEventSignupsController.DisplayEnrollIcon(objEvent) And Settings.eventsignup Then
                If objEvent.MaxEnrollment = 0 Or objEvent.Enrolled < objEvent.MaxEnrollment Then
                    iconString = iconString + "<img src=""" + ResolveUrl("Images/enroll.gif") + """ class=""EventIconEnroll"" alt=""" + Localization.GetString("EnrollEnabled", LocalResourceFile) + """ title=""" + Localization.GetString("EnrollEnabled", LocalResourceFile) + """ /> "
                Else
                    iconString = iconString + "<img src=""" + ResolveUrl("Images/EnrollFull.gif") + """ class=""EventIconEnrollFull"" alt=""" + Localization.GetString("EnrollFull", LocalResourceFile) + """ title=""" + Localization.GetString("EnrollFull", LocalResourceFile) + """ /> "
                End If
            End If
            If objEvent.DetailPage = True Then
                iconString = iconString + DetailPageEdit(objEvent)
            End If
            Return iconString
        End Function

        Public Sub SetupViewControls(ByVal eventIcons As EventIcons, ByVal eventIcons2 As EventIcons, ByVal selectCategory As SelectCategory, ByVal selectLocation As SelectLocation, Optional ByVal pnlDateControls As System.Web.UI.WebControls.Panel = Nothing)
            ' Disable Top Navigation
            If Not pnlDateControls Is Nothing And Settings.DisableEventnav Then
                pnlDateControls.Visible = False
            End If

            ' Setup Icon Bar for use
            SetUpIconBar(eventIcons, eventIcons2)

            ' Category Configuration and Settings.
            selectCategory.ModuleConfiguration = ModuleConfiguration.Clone

            ' Disable Category Select
            If Settings.Enablecategories = EventModuleSettings.DisplayCategories.DoNotDisplay Or _
             (IsPrivateNotModerator And _
             Not IsCategoryEditor()) Then
                selectCategory.Visible = False
            End If

            ' Location Configuration and Settings.
            selectLocation.ModuleConfiguration = ModuleConfiguration.Clone

            ' Disable Location Select
            If Settings.Enablelocations = EventModuleSettings.DisplayLocations.DoNotDisplay Or _
             (IsPrivateNotModerator And _
             Not IsLocationEditor()) Then
                SelectLocation.Visible = False
            End If
        End Sub

        Public Function CreateEventName(ByVal objEvent As EventInfo, Optional ByVal template As String = Nothing) As String
            Dim isEvtEditor As Boolean = IsEventEditor(objEvent, False)

            Dim blAddSubModuleName As Boolean = False
            If objEvent.ModuleID <> ModuleId And objEvent.ModuleTitle <> Nothing And Settings.addsubmodulename Then
                blAddSubModuleName = True
            End If
            Dim tcc As New TokenReplaceControllerClass(ModuleId, LocalResourceFile)
            Return tcc.TokenReplaceEvent(objEvent, template, Nothing, blAddSubModuleName, isEvtEditor)
        End Function

        Public Function Get_ListView_Events(ByVal categoryIDs As ArrayList, ByVal locationIDs As ArrayList) As ArrayList
            Dim moduleStartDate As DateTime     ' Start View Date Events Range
            Dim moduleEndDate As DateTime       ' End View Date Events Range
            Dim displayStartDate As DateTime    ' Start View Date Events Range
            Dim displayEndDate As DateTime      ' End View Date Events Range
            Dim noEvents As Integer

            ' Set Date Range
            Dim moduleDate, displayDate As DateTime
            If Settings.ListViewUseTime Then
                moduleDate = ModuleNow()
                displayDate = DisplayNow()
            Else
                moduleDate = ModuleNow.Date
                displayDate = DisplayNow.Date
            End If
            Dim numDays As Integer = Settings.EventsListEventDays
            If Settings.EventsListSelectType = "DAYS" Then
                '****DO NOT CHANGE THE NEXT SECTION FOR ML CODING ****
                ' Used Only to select view dates on Event Month View..
                moduleStartDate = moduleDate.AddDays((Settings.EventsListBeforeDays + 1) * -1)
                moduleEndDate = moduleDate.AddDays((Settings.EventsListAfterDays + 1) * 1)
                displayStartDate = displayDate.AddDays(Settings.EventsListBeforeDays * -1)
                displayEndDate = displayDate.AddDays(Settings.EventsListAfterDays * 1)
            Else
                noEvents = Settings.EventsListNumEvents
                moduleStartDate = moduleDate.AddDays(-1)
                moduleEndDate = moduleDate.AddDays(numDays + 1)
                displayStartDate = displayDate
                displayEndDate = displayDate.AddDays(numDays)
            End If

            Dim getSubEvents As Boolean = Settings.MasterEvent

            Dim objEvent, lstEvent As EventInfo
            Dim objEventInfoHelper As New EventInfoHelper(ModuleId, TabId, PortalId, Settings)
            Dim lstEvents As ArrayList
            Dim selectedEvents As New ArrayList
            lstEvents = objEventInfoHelper.GetEvents(moduleStartDate, moduleEndDate, getSubEvents, categoryIDs, locationIDs, GetUrlGroupId, GetUrlUserId)

            lstEvents = objEventInfoHelper.ConvertEventListToDisplayTimeZone(lstEvents, GetDisplayTimeZoneId())

            For Each objEvent In lstEvents
                ' If full enrollments should be hidden, ignore
                If HideFullEvent(objEvent) Then
                    Continue For
                End If
                If objEvent.EventTimeEnd < displayStartDate Or _
                    objEvent.EventTimeBegin > displayEndDate Then
                    Continue For
                End If

                Dim blAddEvent As Boolean = True
                If Settings.Collapserecurring Then
                    For Each lstEvent In selectedEvents
                        If lstEvent.RecurMasterID = objEvent.RecurMasterID Then
                            blAddEvent = False
                        End If
                    Next
                End If
                If blAddEvent Then
                    selectedEvents.Add(objEvent)
                End If

                If Settings.EventsListSelectType = "EVENTS" And _
                    selectedEvents.Count >= noEvents Then
                    Exit For
                End If
            Next
            Return selectedEvents
        End Function

        Public Function ModuleNow() As DateTime
            Dim objEventTimeZoneUtilities As New EventTimeZoneUtilities
            Return objEventTimeZoneUtilities.ConvertFromUTCToModuleTimeZone(Date.UtcNow, Settings.TimeZoneId)
        End Function

        Public Function DisplayNow() As DateTime
            Dim objEventTimeZoneUtilities As New EventTimeZoneUtilities
            Return objEventTimeZoneUtilities.ConvertFromUTCToDisplayTimeZone(Date.UtcNow, GetDisplayTimeZoneId()).EventDate
        End Function

        Public Function GetDisplayTimeZoneId() As String
            Return GetDisplayTimeZoneId(Settings, PortalId)
        End Function

        Public Function GetDisplayTimeZoneId(ByVal modSettings As EventModuleSettings, ByVal modPortalid As Integer) As String
            Return GetDisplayTimeZoneId(modSettings, modPortalid, Nothing)
        End Function

        Public Function GetDisplayTimeZoneId(ByVal modSettings As EventModuleSettings, ByVal modPortalid As Integer, ByVal userTimeZoneId As String) As String
            Dim displayTimeToneId As String = ""

            'Try Primary
            Select Case modSettings.PrimaryTimeZone
                Case EventModuleSettings.TimeZones.UserTZ
                    If Not userTimeZoneId Is Nothing Then
                        displayTimeToneId = userTimeZoneId
                    Else
                        displayTimeToneId = GetUserTimeZoneId()
                    End If
                Case EventModuleSettings.TimeZones.ModuleTZ
                    displayTimeToneId = GetModuleTimeZoneId(modSettings)
                Case EventModuleSettings.TimeZones.PortalTZ
                    displayTimeToneId = GetPortalTimeZoneId()
            End Select

            ' Try Secondary
            If displayTimeToneId = "" Then
                Select Case modSettings.SecondaryTimeZone
                    Case EventModuleSettings.TimeZones.UserTZ
                        If Not userTimeZoneId Is Nothing Then
                            displayTimeToneId = userTimeZoneId
                        Else
                            displayTimeToneId = GetUserTimeZoneId()
                        End If
                    Case EventModuleSettings.TimeZones.ModuleTZ
                        displayTimeToneId = GetModuleTimeZoneId(modSettings)
                    Case EventModuleSettings.TimeZones.PortalTZ
                        displayTimeToneId = GetPortalTimeZoneId()
                End Select
            End If

            ' If all else fails use Portal
            If displayTimeToneId = "" Then
                displayTimeToneId = GetPortalTimeZoneId()
            End If

            Return displayTimeToneId
        End Function

        Public Function GetListSortExpression(ByVal columnName As String) As EventListObject.SortFilter
            Dim sortExpression As EventListObject.SortFilter = EventListObject.SortFilter.EventDateBegin
            Select Case columnName
                Case "CategoryName"
                    sortExpression = EventListObject.SortFilter.CategoryName
                Case "CustomField1"
                    sortExpression = EventListObject.SortFilter.CustomField1
                Case "CustomField2"
                    sortExpression = EventListObject.SortFilter.CustomField2
                Case "Description"
                    sortExpression = EventListObject.SortFilter.Description
                Case "Duration"
                    sortExpression = EventListObject.SortFilter.Duration
                Case "EventDateBegin"
                    sortExpression = EventListObject.SortFilter.EventDateBegin
                Case "EventDateEnd"
                    sortExpression = EventListObject.SortFilter.EventDateEnd
                Case "EventName"
                    sortExpression = EventListObject.SortFilter.EventName
                Case "LocationName"
                    sortExpression = EventListObject.SortFilter.LocationName
                Case "EventID"
                    sortExpression = EventListObject.SortFilter.EventID
            End Select
            Return sortExpression
        End Function

        Public Function GetSignupsSortExpression(ByVal columnName As String) As EventSignupsInfo.SortFilter
            Dim sortExpression As EventSignupsInfo.SortFilter = EventSignupsInfo.SortFilter.EventTimeBegin
            Select Case columnName
                Case "EventID"
                    sortExpression = EventSignupsInfo.SortFilter.EventID
                Case "Duration"
                    sortExpression = EventSignupsInfo.SortFilter.Duration
                Case "EventTimeBegin"
                    sortExpression = EventSignupsInfo.SortFilter.EventTimeBegin
                Case "EventTimeEnd"
                    sortExpression = EventSignupsInfo.SortFilter.EventTimeEnd
                Case "EventName"
                    sortExpression = EventSignupsInfo.SortFilter.EventName
                Case "Approved"
                    sortExpression = EventSignupsInfo.SortFilter.Approved
            End Select
            Return sortExpression
        End Function

        Public Function GetUrlGroupId() As Integer
            Dim socialGroupId As Integer = -1
            If Not HttpContext.Current.Request.QueryString("groupid") = "" And Settings.SocialGroupModule = EventModuleSettings.SocialModule.SocialGroup Then
                socialGroupId = CType(HttpContext.Current.Request.QueryString("groupid"), Integer)
            End If
            Return socialGroupId
        End Function

        Public Function GetUrlUserId() As Integer
            Dim socialUserId As Integer = -1
            If Not HttpContext.Current.Request.QueryString("userid") = "" And Settings.SocialGroupModule = EventModuleSettings.SocialModule.UserProfile Then
                socialUserId = CType(HttpContext.Current.Request.QueryString("Userid"), Integer)
            End If
            Return socialUserId
        End Function

        Public Sub StorePrevPageInViewState()
            If Not Request.UrlReferrer Is Nothing Then
                ViewState("prevPage") = Request.UrlReferrer.ToString
            Else
                ViewState("prevPage") = GetSocialNavigateUrl()
            End If
        End Sub

        Public Function GetStoredPrevPage() As String
            Return ViewState("prevPage").ToString
        End Function

        Public Function GetSocialNavigateUrl() As String
            Dim socialGroupId As Integer = GetUrlGroupId()
            Dim socialUserId As Integer = GetUrlUserId()
            If socialGroupId > 0 Then
                Return NavigateURL(TabId, "", "groupid=" & socialGroupId.ToString)
            ElseIf socialUserId > 0 Then
                Return NavigateURL(TabId, "", "userid=" & socialUserId.ToString)
            Else
                Return NavigateURL()
            End If
        End Function
#End Region

#Region "Private Routines"
        Private Function GetUserTimeZoneId() As String
            If HttpContext.Current.Request.IsAuthenticated Then
                Dim objUser As UserInfo = UserController.GetCurrentUserInfo
                Dim authUserTimeZone As TimeZoneInfo = objUser.Profile.PreferredTimeZone
                Return authUserTimeZone.Id
            End If
            Return ""
        End Function

        Private Function GetModuleTimeZoneId(ByVal modSettings As EventModuleSettings) As String
            If Not modSettings.TimeZoneId Is Nothing Then
                Return modSettings.TimeZoneId
            End If
            Return ""
        End Function

        Private Function GetPortalTimeZoneId() As String
            Dim portalTimeZoneId As String
            If HttpContext.Current Is Nothing Then
                portalTimeZoneId = Entities.Portals.PortalController.GetPortalSetting("TimeZone", PortalId, String.Empty)
            Else
                portalTimeZoneId = Entities.Portals.PortalController.GetCurrentPortalSettings.TimeZone.Id
            End If
            Return portalTimeZoneId
        End Function

#End Region

#Region " Shadow PMB Settings "

#Region " Variables "

        Private _settings As EventModuleSettings

#End Region

#Region " Properties "
        Public Shadows Property Settings() As EventModuleSettings
            Get
                If _settings Is Nothing Then
                    Dim ems As New EventModuleSettings
                    _settings = ems.GetEventModuleSettings(ModuleId, LocalResourceFile)
                End If
                Return _settings
            End Get
            Set(ByVal value As EventModuleSettings)
                _settings = value
            End Set
        End Property

#End Region

#End Region
    End Class
End Namespace