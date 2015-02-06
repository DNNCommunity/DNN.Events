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
Imports System.IO
Imports System.Xml
Imports System.Text
Imports System.Globalization
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Security

Namespace DotNetNuke.Modules.Events
    Partial Class EventRSS
        Inherits Page

#Region "Private Variables"
        Private _moduleID As Integer
        Private _tabID As Integer
        Private _portalID As Integer
        Private _settings As EventModuleSettings
        Private _userinfo As UserInfo
        Private Const NsPre As String = "e"
        Private Const NsFull As String = "http://www.dnnsoftware.com/dnnevents"
#End Region
#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region

#Region "Event Handlers"
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim iDaysBefore, iDaysAfter, iMax, iOwnerID, iLocationID, iImportance As Integer
            Dim categoryIDs As New ArrayList
            Dim locationIDs As New ArrayList
            Dim iGroupId As Integer = -1
            Dim iUserId As Integer = -1
            Dim iCategoryName As String = ""
            Dim iLocationName As String = ""
            Dim iOwnerName As String = ""
            Dim txtPriority As String = ""
            If Not HttpContext.Current.Request.QueryString("Mid") = "" Then
                _moduleID = CType(HttpContext.Current.Request.QueryString("mid"), Integer)
            Else
                Response.Redirect(NavigateURL(), True)
            End If
            If Not HttpContext.Current.Request.QueryString("tabid") = "" Then
                _tabID = CType(HttpContext.Current.Request.QueryString("tabid"), Integer)
            Else
                Response.Redirect(NavigateURL(), True)
            End If

            Dim localResourceFile As String = TemplateSourceDirectory & "/" & Localization.LocalResourceDirectory & "/EventRSS.aspx.resx"

            If Not HttpContext.Current.Request.QueryString("CategoryName") = "" Then
                iCategoryName = HttpContext.Current.Request.QueryString("CategoryName")
                Dim objSecurity As New PortalSecurity
                iCategoryName = objSecurity.InputFilter(iCategoryName, PortalSecurity.FilterFlag.NoSQL)
            End If
            If Not HttpContext.Current.Request.QueryString("CategoryID") = "" Then
                categoryIDs.Add(CType(HttpContext.Current.Request.QueryString("CategoryID"), Integer))
            End If
            If Not HttpContext.Current.Request.QueryString("LocationName") = "" Then
                iLocationName = HttpContext.Current.Request.QueryString("LocationName")
                Dim objSecurity As New PortalSecurity
                iLocationName = objSecurity.InputFilter(iLocationName, PortalSecurity.FilterFlag.NoSQL)
            End If
            If Not HttpContext.Current.Request.QueryString("LocationID") = "" Then
                locationIDs.Add(CType(HttpContext.Current.Request.QueryString("LocationID"), Integer))
            End If
            If Not HttpContext.Current.Request.QueryString("groupid") = "" Then
                iGroupId = CType(HttpContext.Current.Request.QueryString("groupid"), Integer)
            End If
            If Not HttpContext.Current.Request.QueryString("DaysBefore") = "" Then
                iDaysBefore = CType(HttpContext.Current.Request.QueryString("DaysBefore"), Integer)
            End If
            If Not HttpContext.Current.Request.QueryString("DaysAfter") = "" Then
                iDaysAfter = CType(HttpContext.Current.Request.QueryString("DaysAfter"), Integer)
            End If
            If Not HttpContext.Current.Request.QueryString("MaxNumber") = "" Then
                iMax = CType(HttpContext.Current.Request.QueryString("MaxNumber"), Integer)
            End If
            If Not HttpContext.Current.Request.QueryString("OwnerName") = "" Then
                iOwnerName = HttpContext.Current.Request.QueryString("OwnerName")
            End If
            If Not HttpContext.Current.Request.QueryString("OwnerID") = "" Then
                iOwnerID = CType(HttpContext.Current.Request.QueryString("OwnerID"), Integer)
            End If
            If Not HttpContext.Current.Request.QueryString("LocationName") = "" Then
                iLocationName = HttpContext.Current.Request.QueryString("LocationName")
            End If
            If Not HttpContext.Current.Request.QueryString("LocationID") = "" Then
                iLocationID = CType(HttpContext.Current.Request.QueryString("LocationID"), Integer)
            End If
            If Not HttpContext.Current.Request.QueryString("Priority") = "" Then
                Dim iPriority As String
                iPriority = HttpContext.Current.Request.QueryString("Priority")
                Dim lHigh, lMedium, lLow As String
                lHigh = Localization.GetString("High", localResourceFile)
                lMedium = Localization.GetString("Normal", localResourceFile)
                lLow = Localization.GetString("Low", localResourceFile)

                txtPriority = "Medium"
                Select Case iPriority
                    Case lHigh
                        txtPriority = "High"
                    Case lMedium
                        txtPriority = "Medium"
                    Case lLow
                        txtPriority = "Low"
                    Case "High"
                        txtPriority = "High"
                    Case "Normal"
                        txtPriority = "Medium"
                    Case "Low"
                        txtPriority = "Low"
                End Select
            End If
            If Not HttpContext.Current.Request.QueryString("Importance") = "" Then
                iImportance = CType(HttpContext.Current.Request.QueryString("Importance"), Integer)
            End If

            Dim portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            _portalID = portalSettings.PortalId
            _userinfo = CType(HttpContext.Current.Items("UserInfo"), UserInfo)
            If Not portalSettings Is Nothing Then
                If portalSettings.DefaultLanguage <> "" Then
                    Dim userculture As New CultureInfo(portalSettings.DefaultLanguage, False)
                    Threading.Thread.CurrentThread.CurrentCulture = userculture
                End If
            End If
            If _userinfo.UserID > 0 Then
                If _userinfo.Profile.PreferredLocale <> "" Then
                    Dim userculture As New CultureInfo(_userinfo.Profile.PreferredLocale, False)
                    Threading.Thread.CurrentThread.CurrentCulture = userculture
                End If
            End If

            Dim ems As New EventModuleSettings
            _settings = ems.GetEventModuleSettings(_moduleID, localResourceFile)

            If _settings.Enablecategories = EventModuleSettings.DisplayCategories.DoNotDisplay Then
                categoryIDs = _settings.ModuleCategoryIDs
                iCategoryName = ""
            End If
            If iCategoryName <> "" Then
                Dim oCntrlEventCategory As New EventCategoryController
                Dim oEventCategory As EventCategoryInfo = oCntrlEventCategory.EventCategoryGetByName(iCategoryName, _portalID)
                If Not oEventCategory Is Nothing Then
                    categoryIDs.Add(oEventCategory.Category)
                End If
            End If
            If _settings.Enablelocations = EventModuleSettings.DisplayLocations.DoNotDisplay Then
                locationIDs = _settings.ModuleLocationIDs
                iLocationName = ""
            End If
            If iLocationName <> "" Then
                Dim oCntrlEventLocation As New EventLocationController
                Dim oEventLocation As EventLocationInfo = oCntrlEventLocation.EventsLocationGetByName(iLocationName, _portalID)
                If Not oEventLocation Is Nothing Then
                    locationIDs.Add(oEventLocation.Location)
                End If
            End If

            If Not _settings.RSSEnable Then
                Response.Redirect(NavigateURL(), True)
            End If

            If _settings.SocialGroupModule = EventModuleSettings.SocialModule.UserProfile Then
                iUserId = _userinfo.UserID
            End If
            Dim getSubEvents As Boolean = _settings.MasterEvent

            Dim dtEndDate As DateTime
            If HttpContext.Current.Request.QueryString("DaysAfter") = "" And _
               HttpContext.Current.Request.QueryString("DaysBefore") = "" Then
                iDaysAfter = _settings.RSSDays
            End If
            Dim objEventTimeZoneUtilities As New EventTimeZoneUtilities
            Dim currDate As DateTime = objEventTimeZoneUtilities.ConvertFromUTCToModuleTimeZone(Date.UtcNow, _settings.TimeZoneId)

            dtEndDate = DateAdd(DateInterval.Day, iDaysAfter, currDate).Date

            Dim dtStartDate As DateTime
            dtStartDate = DateAdd(DateInterval.Day, -iDaysBefore, currDate).Date

            Dim txtFeedRootTitle, txtFeedRootDescription, txtRSSDateField As String
            txtFeedRootTitle = _settings.RSSTitle
            txtFeedRootDescription = _settings.RSSDesc
            txtRSSDateField = _settings.RSSDateField

            Response.ContentType = "text/xml"
            Response.ContentEncoding = Encoding.UTF8


            Using sw As New StringWriter
                Using writer As New XmlTextWriter(sw)
                    '                Dim writer As XmlTextWriter = New XmlTextWriter(sw)
                    writer.Formatting = Formatting.Indented
                    writer.Indentation = 4

                    writer.WriteStartElement("rss")
                    writer.WriteAttributeString("version", "2.0")
                    writer.WriteAttributeString("xmlns:wfw", "http://wellformedweb.org/CommentAPI/")
                    writer.WriteAttributeString("xmlns:slash", "http://purl.org/rss/1.0/modules/slash/")
                    writer.WriteAttributeString("xmlns:dc", "http://purl.org/dc/elements/1.1/")
                    writer.WriteAttributeString("xmlns:trackback", "http://madskills.com/public/xml/rss/module/trackback/")
                    writer.WriteAttributeString("xmlns:atom", "http://www.w3.org/2005/Atom")
                    writer.WriteAttributeString("xmlns", NsPre, Nothing, NsFull)

                    writer.WriteStartElement("channel")

                    writer.WriteStartElement("atom:link")
                    writer.WriteAttributeString("href", HttpContext.Current.Request.Url.AbsoluteUri)
                    writer.WriteAttributeString("rel", "self")
                    writer.WriteAttributeString("type", "application/rss+xml")
                    writer.WriteEndElement()

                    writer.WriteElementString("title", portalSettings.PortalName & " - " & txtFeedRootTitle)

                    If (portalSettings.PortalAlias.HTTPAlias.IndexOf("http://", StringComparison.Ordinal) = -1) And (portalSettings.PortalAlias.HTTPAlias.IndexOf("https://", StringComparison.Ordinal) = -1) Then
                        writer.WriteElementString("link", AddHTTP(portalSettings.PortalAlias.HTTPAlias))
                    Else
                        writer.WriteElementString("link", portalSettings.PortalAlias.HTTPAlias)
                    End If

                    writer.WriteElementString("description", txtFeedRootDescription)
                    writer.WriteElementString("ttl", "60")

                    Dim objEventInfoHelper As New EventInfoHelper(_moduleID, _tabID, _portalID, _settings)
                    Dim lstEvents As ArrayList
                    Dim tcc As New TokenReplaceControllerClass(_moduleID, localResourceFile)
                    Dim tmpTitle As String = _settings.Templates.txtRSSTitle
                    Dim tmpDescription As String = _settings.Templates.txtRSSDescription
                    If categoryIDs.Count = 0 Then
                        categoryIDs.Add("-1")
                    End If
                    If locationIDs.Count = 0 Then
                        locationIDs.Add("-1")
                    End If

                    lstEvents = objEventInfoHelper.GetEvents(dtStartDate, dtEndDate, getSubEvents, categoryIDs, locationIDs, iGroupId, iUserId)

                    Dim objEventBase As New EventBase
                    Dim displayTimeZoneId As String = objEventBase.GetDisplayTimeZoneId(_settings, _portalID)

                    Dim rssCount As Integer = 0
                    For Each objEvent As EventInfo In lstEvents
                        If CInt(categoryIDs.Item(0)) = 0 And objEvent.Category <> CInt(categoryIDs.Item(0)) Then
                            Continue For
                        End If
                        If CInt(locationIDs.Item(0)) = 0 And objEvent.Location <> CInt(locationIDs.Item(0)) Then
                            Continue For
                        End If
                        If iOwnerID > 0 And objEvent.OwnerID <> iOwnerID Then
                            Continue For
                        End If
                        If iOwnerName <> "" And objEvent.OwnerName <> iOwnerName Then
                            Continue For
                        End If
                        If iLocationID > 0 And objEvent.Location <> iLocationID Then
                            Continue For
                        End If
                        If iLocationName <> "" And objEvent.LocationName <> iLocationName Then
                            Continue For
                        End If
                        If iImportance > 0 And objEvent.Importance <> iImportance Then
                            Continue For
                        End If
                        If txtPriority <> "" And objEvent.Importance.ToString <> txtPriority Then
                            Continue For
                        End If

                        ' If full enrollments should be hidden, ignore
                        If HideFullEvent(objEvent) Then
                            Continue For
                        End If

                        Dim pubDate As DateTime
                        Dim pubTimeZoneId As String = ""
                        Select Case txtRSSDateField
                            Case "UPDATEDDATE"
                                pubDate = objEvent.LastUpdatedAt
                                pubTimeZoneId = objEvent.OtherTimeZoneId
                            Case "CREATIONDATE"
                                pubDate = objEvent.CreatedDate
                                pubTimeZoneId = objEvent.OtherTimeZoneId
                            Case "EVENTDATE"
                                pubDate = objEvent.EventTimeBegin
                                pubTimeZoneId = objEvent.EventTimeZoneId
                        End Select

                        objEvent = objEventInfoHelper.ConvertEventToDisplayTimeZone(objEvent, displayTimeZoneId)

                        writer.WriteStartElement("item")
                        Dim eventTitle As String = tcc.TokenReplaceEvent(objEvent, tmpTitle)
                        writer.WriteElementString("title", eventTitle)

                        Dim eventDescription As String = tcc.TokenReplaceEvent(objEvent, tmpDescription)
                        Dim txtDescription As String = HttpUtility.HtmlDecode(eventDescription)
                        writer.WriteElementString("description", txtDescription)

                        Dim txtURL As String = objEventInfoHelper.DetailPageURL(objEvent)
                        writer.WriteElementString("link", txtURL)
                        writer.WriteElementString("guid", txtURL)

                        writer.WriteElementString("pubDate", GetRFC822Date(pubDate, pubTimeZoneId))

                        writer.WriteElementString("dc:creator", objEvent.OwnerName)

                        If objEvent.Category > 0 And Not IsNothing(objEvent.Category) Then
                            writer.WriteElementString("category", objEvent.CategoryName)
                        End If
                        If objEvent.Location > 0 And Not IsNothing(objEvent.Location) Then
                            writer.WriteElementString("category", objEvent.LocationName)
                        End If
                        If objEvent.Importance <> 2 Then
                            Dim strImportance As String = Localization.GetString(objEvent.Importance.ToString & "Prio", localResourceFile)
                            writer.WriteElementString("category", strImportance)
                        End If

                        ' specific event data
                        writer.WriteElementString(NsPre, "AllDayEvent", Nothing, objEvent.AllDayEvent.ToString)
                        writer.WriteElementString(NsPre, "Approved", Nothing, objEvent.Approved.ToString)
                        writer.WriteElementString(NsPre, "Cancelled", Nothing, objEvent.Cancelled.ToString)
                        writer.WriteElementString(NsPre, "Category", Nothing, objEvent.CategoryName)
                        'writer.WriteElementString(NsPre, "Location", Nothing, objEvent.LocationName)
                        writer.WriteElementString(NsPre, "DetailURL", Nothing, objEvent.DetailURL)
                        writer.WriteElementString(NsPre, "EventTimeBegin", Nothing, objEvent.EventTimeBegin.ToString("yyyy-MM-dd HH:mm:ss"))
                        writer.WriteElementString(NsPre, "EventTimeZoneId", Nothing, objEvent.EventTimeZoneId)
                        writer.WriteElementString(NsPre, "Duration", Nothing, objEvent.Duration.ToString)
                        writer.WriteElementString(NsPre, "ImageURL", Nothing, objEvent.ImageURL)
                        writer.WriteElementString(NsPre, "LocationName", Nothing, objEvent.LocationName)
                        writer.WriteElementString(NsPre, "OriginalDateBegin", Nothing, objEvent.OriginalDateBegin.ToString("yyyy-MM-dd HH:mm:ss"))
                        writer.WriteElementString(NsPre, "Signups", Nothing, objEvent.Signups.ToString)
                        writer.WriteElementString(NsPre, "OtherTimeZoneId", Nothing, objEvent.OtherTimeZoneId)

                        writer.WriteEndElement()

                        rssCount += 1
                        If iMax > 0 And rssCount = iMax Then
                            Exit For
                        End If
                    Next

                    writer.WriteEndElement()
                    writer.WriteEndElement()

                    Response.Write(sw.ToString)
                End Using
            End Using

        End Sub

        Private Function HideFullEvent(ByVal objevent As EventInfo) As Boolean
            Dim objEventInfoHelper As New EventInfoHelper(_moduleID, _tabID, _portalID, _settings)
            Return objEventInfoHelper.HideFullEvent(objevent, _settings.Eventhidefullenroll(), _userinfo.UserID, Request.IsAuthenticated)
        End Function

        Private Shared Function GetRFC822Date([date] As DateTime, inTimeZoneId As String) As String
            Dim inTimeZone As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(inTimeZoneId)
            Dim offset As TimeSpan = inTimeZone.GetUtcOffset([date])
            Dim timeZone1 As String
            If offset.Hours >= 0 Then
                timeZone1 = "+"
            Else
                timeZone1 = ""
            End If
            timeZone1 += offset.Hours.ToString("00") & offset.Minutes.ToString("00")
            Return [date].ToString("ddd, dd MMM yyyy HH:mm:ss " & timeZone1, CultureInfo.InvariantCulture)
        End Function

#End Region

    End Class
End Namespace
