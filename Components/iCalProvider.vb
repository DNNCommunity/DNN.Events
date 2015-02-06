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
Imports System
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Security
Imports System.Globalization
Imports System.Text
Imports System.Collections.Generic


Namespace DotNetNuke.Modules.Events
#Region "vEvent Class"

    Public Class VEvent : Implements IHttpHandler

#Region " Private Properties"
        Private _oContext As HttpContext
        Private _blOwnerEmail As Boolean = False
        Private _blAnonOwnerEmail As Boolean = False
        Private ReadOnly _sExdate As New StringBuilder
        Private _objEventInfoHelper As EventInfoHelper
        Private _blSeries As Boolean = False
        Private _blEventSignup As Boolean = False
        Private _blEnrolleeEmail As Boolean = False
        Private _blAnonEmail As Boolean = False
        Private _blViewEmail As Boolean = False
        Private _blEditEmail As Boolean = False
        Private _blImages As Boolean = False
        Private _iUserid As Integer = -1
        Private _domainName As String = ""
        Private _portalurl As String = ""
        Private _timeZoneStart As DateTime
        Private _timeZoneEnd As DateTime
        Private _iCalURLAppend As String = ""
        Private _iCalDefaultImage As String = ""
        Private _icsFilename As String = ""
        Private _settings As EventModuleSettings = Nothing
        Private ReadOnly _vTimeZoneIds As New ArrayList

#End Region

#Region "Constructor"
        Sub New()
        End Sub

        Sub New(ByVal series As Boolean, ByVal context As HttpContext)
            _blSeries = series
            _oContext = context
        End Sub
#End Region

#Region " Properties"
        ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
            Get
                Return True
            End Get
        End Property

#End Region

#Region " Public Methods"

        Public Sub ProcessVCalRequest(ByVal inContext As HttpContext) Implements IHttpHandler.ProcessRequest

            ' Is this a valid request ?
            Dim iItemID As Long = 0
            Dim iModuleID As Integer = 0
            Dim iTabID As Integer = 0
            Dim iCategoryName As String = ""
            Dim iCategoryID As Integer = -1
            Dim iSocialGroupId As Integer = -1
            Dim calname As String = ""

            ' Get the scope information
            _oContext = inContext
            If (iItemID = 0) And (_oContext.Request.QueryString("itemID") = "") Then
                Exit Sub
            End If
            If Not _oContext.Request.QueryString("itemID") = "" Then
                iItemID = CType(_oContext.Request.QueryString("itemID"), Long)
            End If

            If (iModuleID = 0) And (_oContext.Request.QueryString("Mid") = "") Then
                Exit Sub
            End If
            If Not _oContext.Request.QueryString("Mid") = "" Then
                iModuleID = CType(_oContext.Request.QueryString("Mid"), Integer)
            End If

            If (iTabID = 0) And (_oContext.Request.QueryString("TabId") = "") Then
                Exit Sub
            End If
            If Not _oContext.Request.QueryString("TabId") = "" Then
                iTabID = CType(_oContext.Request.QueryString("TabId"), Integer)
            End If

            If iItemID > 0 And (_oContext.Request.QueryString("Series") = "") Then
                Exit Sub
            Else
                _blSeries = False
            End If
            If Not _oContext.Request.QueryString("Series") = "" Then
                _blSeries = CType(_oContext.Request.QueryString("series"), Boolean)
            End If

            If Not _oContext.Request.QueryString("CategoryName") = "" Then
                iCategoryName = _oContext.Request.QueryString("CategoryName")
                Dim objSecurity As New PortalSecurity
                iCategoryName = objSecurity.InputFilter(iCategoryName, PortalSecurity.FilterFlag.NoSQL)
            End If
            If Not HttpContext.Current.Request.QueryString("CategoryID") = "" Then
                iCategoryID = CType(_oContext.Request.QueryString("CategoryID"), Integer)
            End If

            If Not _oContext.Request.QueryString("groupid") = "" Then
                iSocialGroupId = CType(_oContext.Request.QueryString("groupid"), Integer)
            End If

            If Not HttpContext.Current.Request.QueryString("Calname") = "" Then
                calname = _oContext.Request.QueryString("Calname")
            End If

            Dim iCal As String
            iCal = CreateiCal(iTabID, iModuleID, iItemID, iSocialGroupId, iCategoryName, iCategoryID, calname)

            ' Stream The vCalendar 
            Dim oStream As HttpResponse
            oStream = _oContext.Response
            oStream.ContentEncoding = Encoding.UTF8
            oStream.ContentType = "text/Calendar"
            oStream.AppendHeader("Content-Disposition", "filename=" & HttpUtility.UrlEncode(_icsFilename) & ".ics")
            oStream.Write(iCal)

        End Sub

        Public Function CreateiCal(ByVal iTabID As Integer, ByVal iModuleID As Integer, ByVal iItemID As Long, ByVal socialGroupId As Integer, Optional ByVal iCategoryName As String = "", Optional ByVal iCategoryID As Integer = -1, Optional ByVal iLocationName As String = "", Optional ByVal iLocationID As Integer = -1, Optional ByVal calname As String = "") As String
            ' Get relevant module settings
            Dim ems As New EventModuleSettings
            _settings = ems.GetEventModuleSettings(iModuleID, Nothing)

            ' Set up for this module
            Dim portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            _objEventInfoHelper = New EventInfoHelper(iModuleID, iTabID, portalSettings.PortalId, _settings)
            _portalurl = _objEventInfoHelper.GetDomainURL()
            If portalSettings.PortalAlias.HTTPAlias.IndexOf("/", StringComparison.Ordinal) > 0 Then
                _portalurl = _portalurl & Common.Globals.ApplicationPath
            End If

            _blOwnerEmail = _settings.Exportowneremail
            If _blOwnerEmail Then
                _blAnonOwnerEmail = _settings.Exportanonowneremail
            End If
            _blEventSignup = _settings.Eventsignup
            If _settings.EnrollAnonFields.LastIndexOf("03", StringComparison.Ordinal) > -1 Then
                _blAnonEmail = True
            End If
            If _settings.EnrollViewFields.LastIndexOf("03", StringComparison.Ordinal) > -1 Then
                _blViewEmail = True
            End If
            If _settings.EnrollEditFields.LastIndexOf("03", StringComparison.Ordinal) > -1 Then
                _blEditEmail = True
            End If

            Dim socialUserId As Integer = -1
            If _settings.SocialGroupModule = EventModuleSettings.SocialModule.UserProfile Then
                socialUserId = portalSettings.UserId
            End If

            _blImages = _settings.Eventimage
            _iUserid = portalSettings.UserId
            _domainName = portalSettings.PortalAlias.HTTPAlias
            Dim iCalDaysBefore As Integer = _settings.IcalDaysBefore
            Dim iCalDaysAfter As Integer = _settings.IcalDaysAfter
            If Not HttpContext.Current.Request.QueryString("DaysBefore") = "" Then
                iCalDaysBefore = CType(HttpContext.Current.Request.QueryString("DaysBefore"), Integer)
            End If
            If Not HttpContext.Current.Request.QueryString("DaysAfter") = "" Then
                iCalDaysAfter = CType(HttpContext.Current.Request.QueryString("DaysAfter"), Integer)
            End If


            _iCalURLAppend = _settings.IcalURLAppend
            _iCalDefaultImage = Mid(_settings.IcalDefaultImage, 7)

            ' Lookup DesktopModuleID
            Dim objDesktopModule As DesktopModuleInfo
            objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", portalSettings.PortalId)

            ' Build the filename
            Dim objCtlModule As New ModuleController
            Dim objModuleInfo As ModuleInfo = objCtlModule.GetModule(iModuleID, iTabID, False)
            _icsFilename = Common.Utilities.HtmlUtils.StripTags(objModuleInfo.ModuleTitle, False)

            ' Get the event that is being viewed
            Dim oEvent As New EventInfo
            Dim oCntrl As New EventController
            If iItemID > 0 Then
                oEvent = oCntrl.EventsGet(CInt(iItemID), CInt(iModuleID))
            End If

            Dim vEvents As New StringBuilder
            If _blSeries And iItemID = 0 Then
                ' Not supported yet
            ElseIf _blSeries And iItemID > 0 Then
                ' Process the series
                Dim oEventRecurMaster As EventRecurMasterInfo
                Dim oCntrlRecurMaster As New EventRecurMasterController
                oEventRecurMaster = oCntrlRecurMaster.EventsRecurMasterGet(oEvent.RecurMasterID, oEvent.ModuleID)
                vEvents.Append(CreateRecurvEvent(oEventRecurMaster))

                _icsFilename = oEventRecurMaster.EventName
            ElseIf Not _blSeries And iItemID = 0 Then
                ' Process all events for the module
                Dim categoryIDs As New ArrayList
                If _settings.Enablecategories = EventModuleSettings.DisplayCategories.DoNotDisplay Then
                    categoryIDs = _settings.ModuleCategoryIDs
                    iCategoryName = ""
                End If
                If iCategoryName <> "" Then
                    Dim oCntrlEventCategory As New EventCategoryController
                    Dim oEventCategory As EventCategoryInfo = oCntrlEventCategory.EventCategoryGetByName(iCategoryName, portalSettings.PortalId)
                    If Not oEventCategory Is Nothing Then
                        categoryIDs.Add(oEventCategory.Category)
                    End If
                End If
                If categoryIDs.Count = 0 Then
                    categoryIDs.Add("-1")
                End If
                Dim locationIDs As New ArrayList
                If _settings.Enablelocations = EventModuleSettings.DisplayLocations.DoNotDisplay Then
                    locationIDs = _settings.ModuleLocationIDs
                    iLocationName = ""
                End If
                If iLocationName <> "" Then
                    Dim oCntrlEventLocation As New EventLocationController
                    Dim oEventLocation As EventLocationInfo = oCntrlEventLocation.EventsLocationGetByName(iLocationName, portalSettings.PortalId)
                    If Not oEventLocation Is Nothing Then
                        locationIDs.Add(oEventLocation.Location)
                    End If
                End If
                If locationIDs.Count = 0 Then
                    locationIDs.Add("-1")
                End If

                Dim objEventTimeZoneUtilities As New EventTimeZoneUtilities
                Dim moduleDateNow As DateTime = objEventTimeZoneUtilities.ConvertFromUTCToModuleTimeZone(Date.UtcNow, _settings.TimeZoneId)
                Dim startdate As DateTime = DateAdd(DateInterval.Day, 0 - iCalDaysBefore - 1, moduleDateNow)
                Dim enddate As DateTime = DateAdd(DateInterval.Day, iCalDaysAfter + 1, moduleDateNow)
                Dim lstEvents As ArrayList
                lstEvents = _objEventInfoHelper.GetEvents(startdate, enddate, _settings.MasterEvent, categoryIDs, locationIDs, socialGroupId, socialUserId)
                For Each oEvent In lstEvents
                    Dim utcEventTimeBegin As DateTime = objEventTimeZoneUtilities.ConvertToUTCTimeZone(oEvent.EventTimeBegin, oEvent.EventTimeZoneId)
                    Dim utcEventTimeEnd As DateTime = objEventTimeZoneUtilities.ConvertToUTCTimeZone(oEvent.EventTimeEnd, oEvent.EventTimeZoneId)
                    Dim utcStart As DateTime = DateAdd(DateInterval.Day, 0 - iCalDaysBefore, Date.UtcNow)
                    Dim utcEnd As DateTime = DateAdd(DateInterval.Day, iCalDaysAfter, Date.UtcNow)
                    If utcEventTimeEnd > utcStart And utcEventTimeBegin < utcEnd Then
                        vEvents.Append(CreatevEvent(oEvent, oEvent.EventTimeBegin, False, False))
                    End If
                Next
            Else
                ' Process the single event
                vEvents.Append(CreatevEvent(oEvent, oEvent.EventTimeBegin, False, False))
                _icsFilename = oEvent.EventName
            End If

            ' Create the initial VCALENDAR
            Dim vCalendar As New StringBuilder
            vCalendar.Append("BEGIN:VCALENDAR" & Environment.NewLine)
            vCalendar.Append("VERSION:2.0" & Environment.NewLine)
            vCalendar.Append(FoldText("PRODID:-//DNN//" & objDesktopModule.FriendlyName & " " & objDesktopModule.Version & "//EN") & Environment.NewLine)
            vCalendar.Append("CALSCALE:GREGORIAN" & Environment.NewLine)
            vCalendar.Append("METHOD:PUBLISH" & Environment.NewLine)
            If calname <> "" Then
                vCalendar.Append("X-WR-CALNAME:" & calname & Environment.NewLine)
            ElseIf _settings.IcalIncludeCalname Then
                vCalendar.Append("X-WR-CALNAME:" & _icsFilename & Environment.NewLine)
            End If

            ' Create the VTIMEZONE
            If _timeZoneStart = Nothing Then
                _timeZoneStart = Now()
                _timeZoneEnd = DateAdd(DateInterval.Minute, 30, _timeZoneStart)
            End If

            vCalendar.Append(CreatevTimezones(_timeZoneStart, _timeZoneEnd))

            ' Output the events
            vCalendar.Append(vEvents.ToString)

            ' Close off the VCALENDAR
            vCalendar.Append("END:VCALENDAR" & Environment.NewLine)

            Return vCalendar.ToString

        End Function

        Private Function CreatevTimezones(ByVal dtStartDate As DateTime, ByVal dtEndDate As DateTime) As String
            Dim vTimezone As New StringBuilder
            For Each vTimeZoneId As String In _vTimeZoneIds
                Dim tzi As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(vTimeZoneId)
                Dim adjustments() As TimeZoneInfo.AdjustmentRule = tzi.GetAdjustmentRules()

                With vTimezone
                    .Append("BEGIN:VTIMEZONE" & Environment.NewLine)
                    .Append("TZID:" & tzi.Id & Environment.NewLine)

                    If adjustments.Count > 0 Then
                        ' Identify last DST change before start of recurrences
                        Dim intYear As Integer
                        Dim lastDSTStartDate As New DateTime
                        Dim lastDSTEndDate As New DateTime
                        For intYear = dtStartDate.Year - 1 To dtEndDate.Year Step 1
                            Dim yearDayLight As YearDayLight = GetAdjustment(adjustments, intYear)
                            Dim daylightStart As DateTime = yearDayLight.StartDate
                            Dim daylightEnd As DateTime = yearDayLight.EndDate
                            If (lastDSTStartDate = Nothing Or daylightStart > lastDSTStartDate) And daylightStart < dtStartDate Then
                                lastDSTStartDate = daylightStart
                            End If
                            If (lastDSTEndDate = Nothing Or daylightEnd > lastDSTEndDate) And daylightEnd < dtStartDate Then
                                lastDSTEndDate = daylightEnd
                            End If
                        Next
                        For intYear = dtStartDate.Year - 1 To dtEndDate.Year Step 1
                            Dim yearDayLight As YearDayLight = GetAdjustment(adjustments, intYear)
                            Dim daylightStart As DateTime = yearDayLight.StartDate
                            Dim daylightEnd As DateTime = yearDayLight.EndDate

                            Dim iByDay As Double = Int(daylightEnd.Day / 7) + 1
                            Dim sDayOfWeek As String = ""
                            Select Case daylightEnd.DayOfWeek
                                Case DayOfWeek.Sunday
                                    sDayOfWeek = "SU"
                                Case DayOfWeek.Monday
                                    sDayOfWeek = "MO"
                                Case DayOfWeek.Tuesday
                                    sDayOfWeek = "TU"
                                Case DayOfWeek.Wednesday
                                    sDayOfWeek = "WE"
                                Case DayOfWeek.Thursday
                                    sDayOfWeek = "TH"
                                Case DayOfWeek.Friday
                                    sDayOfWeek = "FR"
                                Case DayOfWeek.Saturday
                                    sDayOfWeek = "SA"
                            End Select

                            Dim sByDay As String = iByDay.ToString & sDayOfWeek
                            Dim sByMonth As String = daylightEnd.Month.ToString

                            ' Allow for timezone with no DST, and don't include Timezones after end
                            If daylightStart.Year > 1 And daylightStart < dtEndDate And Not daylightStart < lastDSTStartDate Then
                                Dim dtFrom As String = FormatTZTime(tzi.GetUtcOffset(DateAdd(DateInterval.Day, -2, daylightStart)))
                                Dim dtTo As String = FormatTZTime(tzi.GetUtcOffset(DateAdd(DateInterval.Day, +2, daylightStart)))
                                .Append("BEGIN:DAYLIGHT" & Environment.NewLine)
                                .Append("TZOFFSETFROM:" & dtFrom & Environment.NewLine)
                                .Append("TZOFFSETTO:" & dtTo & Environment.NewLine)
                                .Append("DTSTART:" & CreateTZIDDate(daylightStart, daylightStart, False, False, tzi.Id) & Environment.NewLine)
                                .Append("END:DAYLIGHT" & Environment.NewLine)
                            End If

                            ' Allow for timezone with no DST, and don't include Timezones after end
                            If daylightEnd.Year > 1 And daylightEnd < dtEndDate And Not daylightEnd < lastDSTEndDate Then
                                Dim dtFrom As String = FormatTZTime(tzi.GetUtcOffset(DateAdd(DateInterval.Day, -2, daylightEnd)))
                                Dim dtTo As String = FormatTZTime(tzi.GetUtcOffset(DateAdd(DateInterval.Day, +2, daylightEnd)))
                                .Append("BEGIN:STANDARD" & Environment.NewLine)
                                .Append(FoldText("RRULE:FREQ=YEARLY;INTERVAL=1;BYMONTH=" & sByMonth & ";BYDAY=" & sByDay & ";COUNT=1") & Environment.NewLine)
                                .Append("TZOFFSETFROM:" & dtFrom & Environment.NewLine)
                                .Append("TZOFFSETTO:" & dtTo & Environment.NewLine)
                                .Append("DTSTART:" & CreateTZIDDate(daylightEnd, daylightEnd, False, False, tzi.Id) & Environment.NewLine)
                                .Append("END:STANDARD" & Environment.NewLine)
                            End If

                            If Not daylightStart.Year > 1 Then
                                .Append(CreatevTimezone1601(tzi))
                                Exit For
                            End If
                        Next
                    Else
                        .Append(CreatevTimezone1601(tzi))
                    End If
                    .Append("END:VTIMEZONE" & Environment.NewLine)
                End With
            Next
            Return vTimezone.ToString
        End Function

        Private Function GetAdjustment(ByVal adjustments As IEnumerable(Of TimeZoneInfo.AdjustmentRule), _
                               ByVal year As Integer) As YearDayLight
            ' Iterate adjustment rules for time zone
            For Each adjustment As TimeZoneInfo.AdjustmentRule In adjustments
                ' Determine if this adjustment rule covers year desired
                If adjustment.DateStart.Year <= year And adjustment.DateEnd.Year >= year Then
                    Dim startTransition, endTransition As TimeZoneInfo.TransitionTime
                    Dim yearDayLight As New YearDayLight
                    startTransition = adjustment.DaylightTransitionStart
                    yearDayLight.StartDate = ProcessAdjustmentDate(startTransition, year)
                    endTransition = adjustment.DaylightTransitionEnd
                    yearDayLight.EndDate = ProcessAdjustmentDate(endTransition, year)
                    yearDayLight.Delta = CInt(adjustment.DaylightDelta.TotalMinutes)
                    Return yearDayLight
                End If
            Next
            Return Nothing
        End Function

        Private Function ProcessAdjustmentDate(ByVal processTransition As TimeZoneInfo.TransitionTime, ByVal year As Integer) As DateTime
            Dim transitionDay As Integer
            If processTransition.IsFixedDateRule Then
                transitionDay = processTransition.Day
            Else
                ' For non-fixed date rules, get local calendar
                Static cal As Calendar = CultureInfo.CurrentCulture.Calendar

                ' Get first day of week for transition
                ' For example, the 3rd week starts no earlier than the 15th of the month
                Dim startOfWeek As Integer = processTransition.Week * 7 - 6
                ' What day of the week does the month start on?
                Dim firstDayOfWeek As Integer = cal.GetDayOfWeek(New Date(Year, processTransition.Month, 1))
                ' Determine how much start date has to be adjusted
                Dim changeDayOfWeek As Integer = processTransition.DayOfWeek

                If firstDayOfWeek <= changeDayOfWeek Then
                    transitionDay = startOfWeek + (changeDayOfWeek - firstDayOfWeek)
                Else
                    transitionDay = startOfWeek + (7 - firstDayOfWeek + changeDayOfWeek)
                End If
                ' Adjust for months with no fifth week
                If transitionDay > cal.GetDaysInMonth(Year, processTransition.Month) Then
                    transitionDay -= 7
                End If
            End If
            Return Date.ParseExact(String.Format("{0:0000}/{1:00}/{2:00} {3:HH:mm}", Year, processTransition.Month, transitionDay, processTransition.TimeOfDay), "yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture)
        End Function

        Private Function CreatevTimezone1601(ByVal tzi As TimeZoneInfo) As String
            Dim vTimezone1601 As New StringBuilder
            Dim invCuluture As CultureInfo = CultureInfo.InvariantCulture
            Dim dt1601Date As DateTime = Date.ParseExact("01/01/1601 00:00:00", "MM/dd/yyyy HH:mm:ss", invCuluture)
            Dim dtTo As String = FormatTZTime(tzi.GetUtcOffset(dt1601Date))
            Dim dtFrom As String = FormatTZTime(tzi.GetUtcOffset(dt1601Date))
            With vTimezone1601
                .Append("BEGIN:STANDARD" & Environment.NewLine)
                .Append("TZOFFSETFROM:" & dtFrom & Environment.NewLine)
                .Append("TZOFFSETTO:" & dtTo & Environment.NewLine)
                .Append("DTSTART:" & CreateTZIDDate(dt1601Date, dt1601Date, False, False, tzi.Id) & Environment.NewLine)
                .Append("END:STANDARD" & Environment.NewLine)
            End With
            Return vTimezone1601.ToString
        End Function

        Private Function CreateRecurvEvent(ByVal oEventRecurMaster As EventRecurMasterInfo) As String
            Dim vEvent As New StringBuilder

            Dim lstEvents As ArrayList
            Dim oEvent As EventInfo
            Dim oCntrl As New EventController
            lstEvents = oCntrl.EventsGetRecurrences(oEventRecurMaster.RecurMasterID, oEventRecurMaster.ModuleID)
            ' Create the single VEVENT
            For Each oEvent In lstEvents
                If oEvent.EventTimeBegin.TimeOfDay = oEventRecurMaster.Dtstart.TimeOfDay And _
                   CStr(oEvent.Duration) & "M" = oEventRecurMaster.Duration And _
                   oEvent.EventName = oEventRecurMaster.EventName And _
                   oEvent.EventDesc = oEventRecurMaster.EventDesc And _
                   oEvent.OwnerID = oEventRecurMaster.OwnerID And _
                   oEvent.Location = oEventRecurMaster.Location And _
                   oEvent.Importance = oEventRecurMaster.Importance And _
                   oEvent.SendReminder = oEventRecurMaster.SendReminder And _
                   oEvent.ReminderTime = oEventRecurMaster.ReminderTime And _
                   oEvent.AllDayEvent = oEventRecurMaster.AllDayEvent And _
                   oEvent.ReminderTimeMeasurement = oEventRecurMaster.ReminderTimeMeasurement And _
                   oEvent.Cancelled = False And _
                   (oEvent.Enrolled = 0 Or Not _blEventSignup Or Not oEvent.EnrollListView Or Not oEvent.Signups) Then
                    If Not _blImages Or _
                       (_blImages And oEvent.ImageDisplay = oEventRecurMaster.ImageDisplay And oEvent.ImageURL = oEventRecurMaster.ImageURL) Then
                        Continue For
                    End If
                End If
                vEvent.Append(CreatevEvent(oEvent, oEventRecurMaster.Dtstart, False, oEventRecurMaster.AllDayEvent))
            Next

            If lstEvents.Count = 0 Then
                Return ""
            End If
            Dim oFirstEvent As EventInfo = CType(lstEvents.Item(0), EventInfo)
            Dim objEventLocation As New EventLocationInfo
            Dim objCtlEventLocation As New EventLocationController

            If oEventRecurMaster.Location > 0 Then
                objEventLocation = objCtlEventLocation.EventsLocationGet(oEventRecurMaster.Location, oEventRecurMaster.PortalID)
            End If

            Dim objEventTimeZoneUtilities As New EventTimeZoneUtilities
            Dim recurUntil As DateTime = objEventTimeZoneUtilities.ConvertToUTCTimeZone(oEventRecurMaster.Until, oEventRecurMaster.EventTimeZoneId)

            ' Calculate timezone start/end dates
            Dim intDuration As Integer
            intDuration = CInt(Left(oEventRecurMaster.Duration, Len(oEventRecurMaster.Duration) - 1))
            If _timeZoneStart = Nothing Or _timeZoneStart > oEventRecurMaster.Dtstart Then
                _timeZoneStart = oEventRecurMaster.Dtstart
            End If
            If _timeZoneEnd = Nothing Or _timeZoneEnd < DateAdd(DateInterval.Minute, intDuration, oEventRecurMaster.Until) Then
                _timeZoneEnd = DateAdd(DateInterval.Minute, intDuration, oEventRecurMaster.Until)
            End If

            ' Build Item
            Dim objUsers As New UserController
            Dim objUser As UserInfo = objUsers.GetUser(oEventRecurMaster.PortalID, oEventRecurMaster.OwnerID)
            Dim creatoremail As String
            Dim creatoranonemail As String
            Dim creatorname As String
            If Not objUser Is Nothing Then
                creatoremail = ":MAILTO:" & objUser.Email
                creatoranonemail = ":MAILTO:" & objUser.FirstName & "." & objUser.LastName & "@no_email.com"
                creatorname = "CN=""" & objUser.DisplayName & """"
            Else
                Dim objPortals As New Entities.Portals.PortalController
                Dim objPortal As PortalInfo
                objPortal = objPortals.GetPortal(oEventRecurMaster.PortalID)
                creatoremail = ":MAILTO:" & objPortal.Email()
                creatoranonemail = ":MAILTO:" & "anonymous@no_email.com"
                creatorname = "CN=""Anonymous"""
            End If

            Dim sEmail As String
            If (_oContext.Request.IsAuthenticated And _blOwnerEmail) Or _blAnonOwnerEmail Then
                sEmail = FoldText("ORGANIZER;" & creatorname & creatoremail) & Environment.NewLine
            Else
                sEmail = FoldText("ORGANIZER;" & creatorname & creatoranonemail) & Environment.NewLine
            End If

            Dim aTimes As ArrayList
            Dim sStartTime As String
            Dim sEndTime As String
            Dim sDtStamp As String
            Dim sSequence As String

            aTimes = TimeFormat(oFirstEvent.OriginalDateBegin.Date + oEventRecurMaster.Dtstart.TimeOfDay, intDuration, oFirstEvent.EventTimeZoneId, recurUntil)
            If Not oEventRecurMaster.AllDayEvent Then
                sStartTime = "DTSTART;" & CStr(aTimes(0)) & Environment.NewLine
                sEndTime = "DTEND;" & CStr(aTimes(1)) & Environment.NewLine
            Else
                sStartTime = "DTSTART;VALUE=DATE:" & AllDayEventDate(oFirstEvent.OriginalDateBegin.Date) & Environment.NewLine
                '  +1 deals with use of 1439 minutes instead of 1440
                sEndTime = "DTEND;VALUE=DATE:" & AllDayEventDate(oFirstEvent.OriginalDateBegin.Date.AddMinutes(intDuration + 1)) & Environment.NewLine
            End If
            sDtStamp = "DTSTAMP:" + CreateTZIDDate(DateTime.UtcNow, DateTime.UtcNow, True, False, oEventRecurMaster.EventTimeZoneId) & Environment.NewLine
            sSequence = "SEQUENCE:" & CStr(oEventRecurMaster.Sequence) & Environment.NewLine

            Dim sLocation As String = ""
            If oEventRecurMaster.Location > 0 Then
                If objEventLocation.MapURL <> "" And _settings.IcalURLInLocation Then
                    sLocation = objEventLocation.LocationName & " - " & objEventLocation.MapURL
                Else
                    sLocation = objEventLocation.LocationName
                End If
                sLocation = FoldText("LOCATION:" & CreateText(sLocation)) & Environment.NewLine
            End If
            Dim sDescription As String = CreateDescription(oEventRecurMaster.EventDesc)
            Dim altDescription As String = CreateAltDescription(oEventRecurMaster.EventDesc)
            Dim sSummary As String = FoldText("SUMMARY:" & CreateText(oEventRecurMaster.EventName)) & Environment.NewLine
            Dim sPriority As String = "PRIORITY:" & Priority(oEventRecurMaster.Importance) & Environment.NewLine

            Dim sURL As String = FoldText("URL:" & _objEventInfoHelper.DetailPageURL(oFirstEvent, False) & _iCalURLAppend) & Environment.NewLine

            With vEvent
                .Append("BEGIN:VEVENT" & Environment.NewLine)
                Dim strUID As String = String.Format("{0:00000}", oEventRecurMaster.ModuleID) & String.Format("{0:0000000}", oEventRecurMaster.RecurMasterID)
                .Append("UID:DNNEvent" & strUID & "@" & _domainName & Environment.NewLine)
                .Append(sSequence)
                If oEventRecurMaster.RRULE <> "" Then
                    .Append("RRULE:" & oEventRecurMaster.RRULE & ";" & "UNTIL=" & CStr(aTimes(2)) & Environment.NewLine)
                End If
                If _sExdate.ToString <> "" Then
                    .Append(_sExdate.ToString)
                End If
                .Append(sStartTime)
                .Append(sEndTime)
                .Append(sDtStamp)
                .Append(sURL)
                .Append(sEmail)
                .Append(sDescription)
                .Append(sSummary)
                .Append(altDescription)
                .Append(sPriority)
                .Append(sLocation)

                Dim iMinutes As Integer = 0
                If oEventRecurMaster.SendReminder Then
                    Select Case oEventRecurMaster.ReminderTimeMeasurement
                        Case "d"
                            iMinutes = oEventRecurMaster.ReminderTime * 60 * 24
                        Case "h"
                            iMinutes = oEventRecurMaster.ReminderTime * 60
                        Case "m"
                            iMinutes = oEventRecurMaster.ReminderTime
                    End Select
                End If

                .Append("CLASS:PUBLIC" & Environment.NewLine)
                .Append("CREATED:" & CreateTZIDDate(oEventRecurMaster.CreatedDate, oEventRecurMaster.CreatedDate, True, False, oEventRecurMaster.EventTimeZoneId) & Environment.NewLine)
                .Append("LAST-MODIFIED:" & CreateTZIDDate(oEventRecurMaster.UpdatedDate, oEventRecurMaster.UpdatedDate, True, False, oEventRecurMaster.EventTimeZoneId) & Environment.NewLine)
                If oEventRecurMaster.SendReminder Then
                    .Append("BEGIN:VALARM" & Environment.NewLine)
                    .Append("TRIGGER:-PT" & iMinutes.ToString & "M" & Environment.NewLine)
                    .Append("ACTION:DISPLAY" & Environment.NewLine)
                    .Append("DESCRIPTION:Reminder" & Environment.NewLine)
                    .Append("END:VALARM" & Environment.NewLine)
                End If

                If _blImages And oEventRecurMaster.ImageDisplay Then
                    .Append(FoldText("ATTACH:" & GetImageUrl(oEventRecurMaster.ImageURL, oEventRecurMaster.PortalID)) & Environment.NewLine)
                ElseIf _blImages And _iCalDefaultImage <> "" Then
                    .Append(FoldText("ATTACH:" & GetImageUrl(_iCalDefaultImage, oEventRecurMaster.PortalID)) & Environment.NewLine)
                End If

                .Append("TRANSP:OPAQUE" & Environment.NewLine)

                .Append("END:VEVENT" & Environment.NewLine)
            End With

            Return vEvent.ToString

        End Function

        Private Function CreatevEvent(ByVal oEvent As EventInfo, ByVal dtstart As DateTime, ByVal blURLOnly As Boolean, ByVal blAllDay As Boolean) As String

            If Not _vTimeZoneIds.Contains(oEvent.EventTimeZoneId) Then
                _vTimeZoneIds.Add(oEvent.EventTimeZoneId)
            End If

            oEvent.OriginalDateBegin = oEvent.OriginalDateBegin.Date + dtstart.TimeOfDay

            ' Calculate timezone start/end dates
            If _timeZoneStart = Nothing Or _timeZoneStart > oEvent.EventTimeBegin Then
                _timeZoneStart = oEvent.EventTimeBegin
            End If
            If _timeZoneEnd = Nothing Or _timeZoneEnd < DateAdd(DateInterval.Minute, oEvent.Duration, oEvent.EventTimeBegin) Then
                _timeZoneEnd = DateAdd(DateInterval.Minute, oEvent.Duration, oEvent.EventTimeBegin)
            End If

            ' Build Item
            Dim vEvent As New StringBuilder

            Dim objUsers As New UserController
            Dim objUser As UserInfo = objUsers.GetUser(oEvent.PortalID, oEvent.OwnerID)
            Dim creatoremail As String
            Dim creatoranonemail As String
            Dim creatorname As String
            If Not objUser Is Nothing Then
                creatoremail = ":MAILTO:" & objUser.Email
                creatoranonemail = ":MAILTO:" & objUser.FirstName & "." & objUser.LastName & "@no_email.com"
                creatorname = "CN=""" & objUser.DisplayName & """"
            Else
                Dim objPortals As New Entities.Portals.PortalController
                Dim objPortal As Entities.Portals.PortalInfo
                objPortal = objPortals.GetPortal(oEvent.PortalID)
                creatoremail = ":MAILTO:" & objPortal.Email()
                creatoranonemail = ":MAILTO:" & "anonymous@no_email.com"
                creatorname = "CN=""Anonymous"""
            End If

            Dim sEmail As String
            If (_oContext.Request.IsAuthenticated And _blOwnerEmail) Or _blAnonOwnerEmail Then
                sEmail = FoldText("ORGANIZER;" & creatorname & creatoremail) & Environment.NewLine
            Else
                sEmail = FoldText("ORGANIZER;" & creatorname & creatoranonemail) & Environment.NewLine
            End If

            Dim aTimes As ArrayList
            Dim sStartTime As String
            Dim sEndTime As String
            Dim sDtStamp As String = ""
            Dim sSequence As String

            aTimes = TimeFormat(oEvent.EventTimeBegin, oEvent.Duration, oEvent.EventTimeZoneId, , oEvent.OriginalDateBegin)
            If oEvent.Cancelled Then
                _sExdate.Append("EXDATE;" & CStr(aTimes(3)) & Environment.NewLine)
                Return ""
            End If

            If Not oEvent.AllDayEvent Then
                sStartTime = "DTSTART;" & CStr(aTimes(0)) & Environment.NewLine
                sEndTime = "DTEND;" & CStr(aTimes(1)) & Environment.NewLine
            Else
                sStartTime = "DTSTART;VALUE=DATE:" & AllDayEventDate(oEvent.EventTimeBegin.Date) & Environment.NewLine
                '  +1 deals with use of 1439 minutes instead of 1440
                sEndTime = "DTEND;VALUE=DATE:" & AllDayEventDate(oEvent.EventTimeBegin.Date.AddMinutes(oEvent.Duration + 1)) & Environment.NewLine
            End If
            If Not _blSeries Then
                sDtStamp = "DTSTAMP:" + CreateTZIDDate(DateTime.UtcNow, DateTime.UtcNow, True, False, oEvent.EventTimeZoneId) & Environment.NewLine
            End If
            sSequence = "SEQUENCE:" & CStr(oEvent.Sequence) & Environment.NewLine

            Dim sLocation As String = ""
            If oEvent.Location > 0 Then
                If oEvent.MapURL <> "" And _settings.IcalURLInLocation Then
                    sLocation = oEvent.LocationName & " - " & oEvent.MapURL
                Else
                    sLocation = oEvent.LocationName
                End If
                sLocation = FoldText("LOCATION:" & CreateText(sLocation)) & Environment.NewLine
            End If
            Dim sDescription As String = CreateDescription(oEvent.EventDesc)
            Dim altDescription As String = CreateAltDescription(oEvent.EventDesc)
            Dim sSummary As String = FoldText("SUMMARY:" & CreateText(oEvent.EventName)) & Environment.NewLine
            Dim sPriority As String = "PRIORITY:" & Priority(oEvent.Importance) & Environment.NewLine

            Dim sURL As String = FoldText("URL:" & _objEventInfoHelper.DetailPageURL(oEvent, False) & _iCalURLAppend) & Environment.NewLine

            With vEvent
                .Append("BEGIN:VEVENT" & Environment.NewLine)
                Dim strUID As String = String.Format("{0:00000}", oEvent.ModuleID) & String.Format("{0:0000000}", oEvent.RecurMasterID)
                If Not _blSeries Then
                    strUID += String.Format("{0:0000000}", oEvent.EventID)
                End If
                .Append("UID:DNNEvent" & strUID & "@" & _domainName & Environment.NewLine)
                .Append(sSequence)
                If _blSeries Then
                    If Not blAllDay Then
                        .Append("RECURRENCE-ID;" & CStr(aTimes(3)) & Environment.NewLine)
                    Else
                        .Append("RECURRENCE-ID;VALUE=DATE:" & AllDayEventDate(oEvent.OriginalDateBegin) & Environment.NewLine)
                    End If
                End If
                .Append(sStartTime)
                .Append(sEndTime)
                .Append(sDtStamp)
                .Append(sURL)
                If Not blURLOnly Then
                    .Append(sEmail)
                    .Append(sDescription)
                    .Append(sSummary)
                    .Append(altDescription)
                    .Append(sPriority)
                    .Append(sLocation)

                    If _blEventSignup And oEvent.EnrollListView And oEvent.Signups And oEvent.Enrolled > 0 Then
                        _blEnrolleeEmail = False
                        If (IsModerator() Or _
                           (oEvent.CreatedByID = _iUserid Or oEvent.RmOwnerID = _iUserid Or oEvent.OwnerID = _iUserid)) And _
                           _blEditEmail Then
                            _blEnrolleeEmail = True
                        End If
                        If _oContext.Request.IsAuthenticated And _
                           (_blViewEmail Or _blAnonEmail) Then
                            _blEnrolleeEmail = True
                        End If
                        If Not _oContext.Request.IsAuthenticated And _
                           _blAnonEmail Then
                            _blEnrolleeEmail = True
                        End If
                        .Append(CreateAttendee(oEvent))
                    End If

                    Dim iMinutes As Integer = 0
                    If oEvent.SendReminder Then
                        Select Case oEvent.ReminderTimeMeasurement
                            Case "d"
                                iMinutes = oEvent.ReminderTime * 60 * 24
                            Case "h"
                                iMinutes = oEvent.ReminderTime * 60
                            Case "m"
                                iMinutes = oEvent.ReminderTime
                        End Select
                    End If

                    .Append("CLASS:PUBLIC" & Environment.NewLine)
                    .Append("CREATED:" & CreateTZIDDate(oEvent.CreatedDate, oEvent.CreatedDate, True, False, oEvent.EventTimeZoneId) & Environment.NewLine)
                    .Append("LAST-MODIFIED:" & CreateTZIDDate(oEvent.LastUpdatedAt, oEvent.LastUpdatedAt, True, False, oEvent.EventTimeZoneId) & Environment.NewLine)
                    If oEvent.SendReminder Then
                        .Append("BEGIN:VALARM" & Environment.NewLine)
                        .Append("TRIGGER:-PT" & iMinutes.ToString & "M" & Environment.NewLine)
                        .Append("ACTION:DISPLAY" & Environment.NewLine)
                        .Append("DESCRIPTION:Reminder" & Environment.NewLine)
                        .Append("END:VALARM" & Environment.NewLine)
                    End If

                    If _blImages And oEvent.ImageDisplay Then
                        .Append(FoldText("ATTACH:" & GetImageUrl(oEvent.ImageURL, oEvent.PortalID)) & Environment.NewLine)
                    ElseIf _blImages And _iCalDefaultImage <> "" Then
                        .Append(FoldText("ATTACH:" & GetImageUrl(_iCalDefaultImage, oEvent.PortalID)) & Environment.NewLine)
                    End If
                End If

                .Append("TRANSP:OPAQUE" & Environment.NewLine)

                .Append("END:VEVENT" & Environment.NewLine)
            End With

            Return vEvent.ToString
        End Function

        Private Function CreateAttendee(ByVal oEvent As EventInfo) As String
            Dim attendees As New StringBuilder
            Dim oSignups As ArrayList
            Dim oSignup As EventSignupsInfo
            Dim oCtlEventSignups As New EventSignupsController
            Dim objUsers As New UserController
            oSignups = oCtlEventSignups.EventsSignupsGetEvent(oEvent.EventID, oEvent.ModuleID)
            For Each oSignup In oSignups
                Dim objUser As UserInfo = objUsers.GetUser(oEvent.PortalID, oSignup.UserID)
                Dim attendeeemail As String
                Dim attendeeanonemail As String
                Dim attendeename As String
                Dim sPartStat As String = "ACCEPTED"
                If Not oSignup.Approved Then
                    sPartStat = "NEEDS-ACTION"
                End If
                If Not objUser Is Nothing Then
                    attendeeemail = ":MAILTO:" & objUser.Email
                    attendeeanonemail = ":MAILTO:" & objUser.FirstName & "." & objUser.LastName & "@no_email.com"
                    attendeename = "CN=""" & objUser.DisplayName & """"
                Else
                    attendeeemail = ":MAILTO:" & "anonymous@no_email.com"
                    attendeeanonemail = ":MAILTO:" & "anonymous@no_email.com"
                    attendeename = "CN=""Anonymous-" & oSignup.UserID.ToString & """"
                End If
                Dim sAttendee As String
                If _blEnrolleeEmail Then
                    sAttendee = "ATTENDEE;ROLE=REQ-PARTICIPANT;PARTSTAT=" & sPartStat & ";" & attendeename & attendeeemail & Environment.NewLine
                Else
                    sAttendee = "ATTENDEE;ROLE=REQ-PARTICIPANT;PARTSTAT=" & sPartStat & ";" & attendeename & attendeeanonemail & Environment.NewLine
                End If
                attendees.Append(sAttendee)
            Next
            Return attendees.ToString
        End Function
#End Region

#Region " Private Support Functions"
        Private Shared Function TimeFormat(ByVal dBeginDateTime As Date, ByVal iDuration As Integer, ByVal timeZoneId As String, Optional ByVal dUntil As DateTime = Nothing, Optional ByVal dOriginal As DateTime = Nothing) As ArrayList
            Dim aTimes As New ArrayList
            Dim eDate As DateTime
            Dim tempDateTime As DateTime = dBeginDateTime.Date + dBeginDateTime.TimeOfDay

            'Begin Time Format
            aTimes.Add(CreateTZIDDate(dBeginDateTime, dBeginDateTime, False, True, timeZoneId))

            'End Time Format
            eDate = tempDateTime.AddMinutes(iDuration)
            aTimes.Add(CreateTZIDDate(eDate, eDate, False, True, timeZoneId))

            'Until Time Format
            Dim invCuluture As CultureInfo = CultureInfo.InvariantCulture
            aTimes.Add(CreateTZIDDate(dUntil, Date.ParseExact("01/01/2002 23:59:59", "MM/dd/yyyy HH:mm:ss", invCuluture), True, False, timeZoneId))

            'Original Time Format
            aTimes.Add(CreateTZIDDate(dOriginal, dOriginal, False, True, timeZoneId))

            Return aTimes
        End Function

        Private Shared Function CreateTZIDDate(ByVal dDate As DateTime, ByVal dTime As DateTime, ByVal blUTC As Boolean, ByVal blTZAdd As Boolean, ByVal timeZoneId As String) As String
            Dim sTime As New StringBuilder
            If blTZAdd Then
                sTime.Append("TZID=""" & timeZoneId & """:")
            End If
            sTime.Append(dDate.Year.ToString)
            sTime.Append(Format(dDate.Month, "0#").ToString)
            sTime.Append(Format(dDate.Day, "0#").ToString)
            sTime.Append("T")
            sTime.Append(Format(dTime.Hour, "0#").ToString)
            sTime.Append(Format(dTime.Minute, "0#").ToString)
            sTime.Append(Format(dTime.Second, "0#").ToString)
            If blUTC Then
                sTime.Append("Z")
            End If
            Return sTime.ToString
        End Function

        Private Shared Function AllDayEventDate(ByVal dDate As DateTime) As String
            Dim sDate As New StringBuilder
            sDate.Append(dDate.Year.ToString)
            sDate.Append(Format(dDate.Month, "0#").ToString)
            sDate.Append(Format(dDate.Day, "0#").ToString)
            Return sDate.ToString
        End Function

        Private Function Priority(ByVal importance As Integer) As String
            Select Case Importance
                Case 1
                    Return "1"
                Case 3
                    Return "9"
                Case Else
                    Return "5"
            End Select
        End Function

        Private Function CreateAltDescription(ByVal eventDesc As String) As String
            Dim altDescription As String = "X-ALT-DESC;FMTTYPE=text/html:<!DOCTYPE HTML PUBLIC "" -//W3C//DTD HTML 3.2//EN"">\n"
            altDescription += "<HTML>\n"
            altDescription += "<HEAD>\n"
            altDescription += "<META NAME=""Generator"" CONTENT=""DNN Events Module"">\n"
            altDescription += "<TITLE></TITLE>\n"
            altDescription += "</HEAD>\n"
            altDescription += "<BODY>\n"
            altDescription += HtmlUtils.StripWhiteSpace(HttpUtility.HtmlDecode(eventDesc), True).Replace(Environment.NewLine, "") & "\n"
            altDescription += "</BODY>\n"
            altDescription += "</HTML>"
            altDescription = FoldText(altDescription) & Environment.NewLine

            Return altDescription
        End Function

        Private Function CreateDescription(ByVal eventDesc As String) As String
            Dim sDescription As String = "DESCRIPTION:"
            Const descriptionLength As Integer = 1950
            Dim tmpDesc As String = CreateText(eventDesc)
            tmpDesc = HtmlUtils.Shorten(tmpDesc, descriptionLength, "...")
            sDescription = FoldText(sDescription & tmpDesc & "\n") & Environment.NewLine
            Return sDescription
        End Function

        Private Function CreateText(ByVal eventText As String) As String
            Dim tmpText As String = HtmlUtils.StripTags(HttpUtility.HtmlDecode(eventText), False)
            ' Double decode, for things that were encoded by RadEditor
            tmpText = HttpUtility.HtmlDecode(tmpText)
            tmpText = tmpText.Replace("\", "\\")
            tmpText = tmpText.Replace(",", "\,")
            tmpText = tmpText.Replace(";", "\;")
            tmpText = tmpText.Replace(Environment.NewLine, "\n")
            Return tmpText
        End Function

        Private Function FormatTZTime(ByVal dtTimeSpan As TimeSpan) As String
            Dim dtSign As String = "+"
            If dtTimeSpan.Hours < 0 Then
                dtSign = "-"
            End If
            Return dtSign & Format(Math.Abs(dtTimeSpan.Hours), "0#") & Format(Math.Abs(dtTimeSpan.Minutes), "0#")
        End Function

        Private Function IsModerator() As Boolean
            Return _objEventInfoHelper.IsModerator(True)
        End Function

        Private Function GetImageUrl(ByVal imageURL As String, ByVal portalID As Integer) As String
            Dim imageSrc As String = imageURL

            If imageURL.StartsWith("FileID=") Then
                Dim fileId As Integer = Integer.Parse(imageURL.Substring(7))
                Dim objFileInfo As Services.FileSystem.IFileInfo = Services.FileSystem.FileManager.Instance.GetFile(fileId)
                If Not objFileInfo Is Nothing Then
                    imageSrc = objFileInfo.Folder + objFileInfo.FileName
                    If InStr(1, imageSrc, "://") = 0 Then
                        Dim pi As New Entities.Portals.PortalController
                        imageSrc = AddHTTP(String.Format("{0}/{1}/{2}", _portalurl, pi.GetPortal(PortalID).HomeDirectory, imageSrc))
                    End If
                End If
            End If

            Return imageSrc

        End Function

        Private Function FoldText(ByVal inText As String) As String
            Dim outText As String = ""
            Do While Len(inText) > 75
                outText = outText & Left(inText, 75) & Environment.NewLine & " "
                inText = Mid(inText, 76)
            Loop
            outText = outText & inText
            Return outText
        End Function

        Private Class YearDayLight
            Private _startDate As DateTime
            Property StartDate() As DateTime
                Get
                    Return _StartDate
                End Get
                Set(ByVal value As DateTime)
                    _startDate = Value
                End Set
            End Property
            Private _endDate As DateTime
            Property EndDate() As DateTime
                Get
                    Return _EndDate
                End Get
                Set(ByVal value As DateTime)
                    _endDate = Value
                End Set
            End Property
            Private _delta As Integer
            Property Delta() As Integer
                ' ReSharper disable UnusedMember.Local
                Get
                    ' ReSharper restore UnusedMember.Local
                    Return _delta
                End Get
                Set(ByVal value As Integer)
                    _delta = Value
                End Set
            End Property

        End Class

#End Region

    End Class
#End Region

End Namespace

