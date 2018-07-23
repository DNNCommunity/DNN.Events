#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2018
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//

#endregion


namespace DotNetNuke.Modules.Events
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web;
    using DotNetNuke.Common.Utilities;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Entities.Users;
    using DotNetNuke.Security;
    using DotNetNuke.Services.FileSystem;
    using DotNetNuke.Services.Localization;
    using global::Components;
    using Microsoft.VisualBasic;
    using Calendar = System.Globalization.Calendar;
    using Globals = DotNetNuke.Common.Globals;

    #region vEvent Class

    public class VEvent : IHttpHandler
    {
        #region  Properties

        public bool IsReusable => true;

        #endregion

        #region  Private Properties

        private int _moduleID;
        private HttpContext _oContext;
        private bool _blOwnerEmail;
        private bool _blAnonOwnerEmail;
        private readonly StringBuilder _sExdate = new StringBuilder();
        private EventInfoHelper _objEventInfoHelper;
        private bool _blSeries;
        private bool _blEventSignup;
        private bool _blEnrolleeEmail;
        private bool _blAnonEmail;
        private bool _blViewEmail;
        private bool _blEditEmail;
        private bool _blImages;
        private int _iUserid = -1;
        private string _domainName = "";
        private string _portalurl = "";
        private DateTime _timeZoneStart;
        private DateTime _timeZoneEnd;
        private string _iCalURLAppend = "";
        private string _iCalDefaultImage = "";
        private string _icsFilename = "";
        private EventModuleSettings _settings;
        private readonly ArrayList _vTimeZoneIds = new ArrayList();

        #endregion

        #region Constructor

        public VEvent()
        { }

        public VEvent(bool series, HttpContext context)
        {
            this._blSeries = series;
            this._oContext = context;
        }

        #endregion

        #region  Public Methods

        void IHttpHandler.ProcessRequest(HttpContext inContext)
        {
            this.ProcessVCalRequest(inContext);
        }

        public void ProcessVCalRequest(HttpContext inContext)
        {
            // Is this a valid request
            long iItemID = 0;
            var iModuleID = 0;
            var iTabID = 0;
            var iCategoryName = "";
            var iCategoryID = -1;
            var iSocialGroupId = -1;
            var calname = "";
            this._moduleID = 0;

            // Get the scope information
            this._oContext = inContext;
            if (iItemID == 0 && this._oContext.Request.QueryString["itemID"] == "")
            {
                return;
            }
            if (!(this._oContext.Request.QueryString["itemID"] == ""))
            {
                iItemID = Convert.ToInt64(this._oContext.Request.QueryString["itemID"]);
            }

            if (iModuleID == 0 && this._oContext.Request.QueryString["Mid"] == "")
            {
                return;
            }
            if (!(this._oContext.Request.QueryString["Mid"] == ""))
            {
                iModuleID = Convert.ToInt32(this._oContext.Request.QueryString["Mid"]);
            }

            //Save this in the private property value
            this._moduleID = iModuleID;

            if (iTabID == 0 && this._oContext.Request.QueryString["TabId"] == "")
            {
                return;
            }
            if (!(this._oContext.Request.QueryString["TabId"] == ""))
            {
                iTabID = Convert.ToInt32(this._oContext.Request.QueryString["TabId"]);
            }

            if (iItemID > 0 && this._oContext.Request.QueryString["Series"] == "")
            {
                return;
            }
            this._blSeries = false;
            if (!(this._oContext.Request.QueryString["Series"] == ""))
            {
                this._blSeries = Convert.ToBoolean(this._oContext.Request.QueryString["series"]);
            }

            if (!(this._oContext.Request.QueryString["CategoryName"] == ""))
            {
                iCategoryName = this._oContext.Request.QueryString["CategoryName"];
                var objSecurity = new PortalSecurity();
                iCategoryName = objSecurity.InputFilter(iCategoryName, PortalSecurity.FilterFlag.NoSQL);
            }
            if (!(HttpContext.Current.Request.QueryString["CategoryID"] == ""))
            {
                iCategoryID = Convert.ToInt32(this._oContext.Request.QueryString["CategoryID"]);
            }

            if (!(this._oContext.Request.QueryString["groupid"] == ""))
            {
                iSocialGroupId = Convert.ToInt32(this._oContext.Request.QueryString["groupid"]);
            }

            if (!(HttpContext.Current.Request.QueryString["Calname"] == ""))
            {
                calname = this._oContext.Request.QueryString["Calname"];
            }

            var iCal = "";
            iCal = this.CreateiCal(iTabID, iModuleID, iItemID, iSocialGroupId, iCategoryName, iCategoryID, calname);

            // Stream The vCalendar
            var oStream = default(HttpResponse);
            oStream = this._oContext.Response;
            oStream.ContentEncoding = Encoding.UTF8;
            oStream.ContentType = "text/Calendar";
            oStream.AppendHeader("Content-Disposition",
                                 "filename=" + HttpUtility.UrlEncode(this._icsFilename) + ".ics");
            oStream.Write(iCal);
        }

        public string CreateiCal(int iTabID, int iModuleID, long iItemID, int socialGroupId, string iCategoryName = "",
                                 int iCategoryID = -1, string iLocationName = "", int iLocationID = -1,
                                 string calname = "")
        {
            // Get relevant module settings

            this._settings = EventModuleSettings.GetEventModuleSettings(iModuleID, null);

            // Set up for this module
            var portalSettings = (PortalSettings) HttpContext.Current.Items["PortalSettings"];
            this._objEventInfoHelper = new EventInfoHelper(iModuleID, iTabID, portalSettings.PortalId, this._settings);
            this._portalurl = this._objEventInfoHelper.GetDomainURL();
            if (portalSettings.PortalAlias.HTTPAlias.IndexOf("/", StringComparison.Ordinal) > 0)
            {
                this._portalurl = this._portalurl + Globals.ApplicationPath;
            }

            this._blOwnerEmail = this._settings.Exportowneremail;
            if (this._blOwnerEmail)
            {
                this._blAnonOwnerEmail = this._settings.Exportanonowneremail;
            }
            this._blEventSignup = this._settings.Eventsignup;
            if (this._settings.EnrollAnonFields.LastIndexOf("03", StringComparison.Ordinal) > -1)
            {
                this._blAnonEmail = true;
            }
            if (this._settings.EnrollViewFields.LastIndexOf("03", StringComparison.Ordinal) > -1)
            {
                this._blViewEmail = true;
            }
            if (this._settings.EnrollEditFields.LastIndexOf("03", StringComparison.Ordinal) > -1)
            {
                this._blEditEmail = true;
            }

            var socialUserId = -1;
            if (this._settings.SocialGroupModule == EventModuleSettings.SocialModule.UserProfile)
            {
                socialUserId = portalSettings.UserId;
            }

            this._blImages = this._settings.Eventimage;
            this._iUserid = portalSettings.UserId;
            this._domainName = portalSettings.PortalAlias.HTTPAlias;
            var iCalDaysBefore = this._settings.IcalDaysBefore;
            var iCalDaysAfter = this._settings.IcalDaysAfter;
            if (!(HttpContext.Current.Request.QueryString["DaysBefore"] == ""))
            {
                iCalDaysBefore = Convert.ToInt32(HttpContext.Current.Request.QueryString["DaysBefore"]);
            }
            if (!(HttpContext.Current.Request.QueryString["DaysAfter"] == ""))
            {
                iCalDaysAfter = Convert.ToInt32(HttpContext.Current.Request.QueryString["DaysAfter"]);
            }


            this._iCalURLAppend = this._settings.IcalURLAppend;
            this._iCalDefaultImage = this._settings.IcalDefaultImage.Substring(6);

            // Lookup DesktopModuleID
            var objDesktopModule = default(DesktopModuleInfo);
            objDesktopModule =
                DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", portalSettings.PortalId);

            // Build the filename
            var objCtlModule = new ModuleController();
            var objModuleInfo = objCtlModule.GetModule(iModuleID, iTabID, false);
            this._icsFilename = HtmlUtils.StripTags(objModuleInfo.ModuleTitle, false);

            // Get the event that is being viewed
            var oEvent = new EventInfo();
            var oCntrl = new EventController();
            if (iItemID > 0)
            {
                oEvent = oCntrl.EventsGet((int) iItemID, iModuleID);
            }

            var vEvents = new StringBuilder();
            if (this._blSeries && iItemID == 0)
            {
                // Not supported yet
            }
            else if (this._blSeries && iItemID > 0)
            {
                // Process the series
                var oEventRecurMaster = default(EventRecurMasterInfo);
                var oCntrlRecurMaster = new EventRecurMasterController();
                oEventRecurMaster = oCntrlRecurMaster.EventsRecurMasterGet(oEvent.RecurMasterID, oEvent.ModuleID);
                vEvents.Append(this.CreateRecurvEvent(oEventRecurMaster));

                this._icsFilename = oEventRecurMaster.EventName;
            }
            else if (!this._blSeries && iItemID == 0)
            {
                // Process all events for the module
                var categoryIDs = new ArrayList();
                if (this._settings.Enablecategories == EventModuleSettings.DisplayCategories.DoNotDisplay)
                {
                    categoryIDs = this._settings.ModuleCategoryIDs;
                    iCategoryName = "";
                }
                if (iCategoryName != "")
                {
                    var oCntrlEventCategory = new EventCategoryController();
                    var oEventCategory =
                        oCntrlEventCategory.EventCategoryGetByName(iCategoryName, portalSettings.PortalId);
                    if (!ReferenceEquals(oEventCategory, null))
                    {
                        categoryIDs.Add(oEventCategory.Category);
                    }
                }
                if (categoryIDs.Count == 0)
                {
                    categoryIDs.Add("-1");
                }
                var locationIDs = new ArrayList();
                if (this._settings.Enablelocations == EventModuleSettings.DisplayLocations.DoNotDisplay)
                {
                    locationIDs = this._settings.ModuleLocationIDs;
                    iLocationName = "";
                }
                if (iLocationName != "")
                {
                    var oCntrlEventLocation = new EventLocationController();
                    var oEventLocation =
                        oCntrlEventLocation.EventsLocationGetByName(iLocationName, portalSettings.PortalId);
                    if (!ReferenceEquals(oEventLocation, null))
                    {
                        locationIDs.Add(oEventLocation.Location);
                    }
                }
                if (locationIDs.Count == 0)
                {
                    locationIDs.Add("-1");
                }

                var objEventTimeZoneUtilities = new EventTimeZoneUtilities();
                var moduleDateNow =
                    objEventTimeZoneUtilities
                        .ConvertFromUTCToModuleTimeZone(DateTime.UtcNow, this._settings.TimeZoneId);
                var startdate = DateAndTime.DateAdd(DateInterval.Day, 0 - iCalDaysBefore - 1, moduleDateNow);
                var enddate = DateAndTime.DateAdd(DateInterval.Day, iCalDaysAfter + 1, moduleDateNow);
                var lstEvents = default(ArrayList);
                lstEvents = this._objEventInfoHelper.GetEvents(startdate, enddate, this._settings.MasterEvent,
                                                               categoryIDs, locationIDs, socialGroupId, socialUserId);
                foreach (EventInfo tempLoopVar_oEvent in lstEvents)
                {
                    oEvent = tempLoopVar_oEvent;
                    var utcEventTimeBegin =
                        objEventTimeZoneUtilities.ConvertToUTCTimeZone(oEvent.EventTimeBegin, oEvent.EventTimeZoneId);
                    var utcEventTimeEnd =
                        objEventTimeZoneUtilities.ConvertToUTCTimeZone(oEvent.EventTimeEnd, oEvent.EventTimeZoneId);
                    var utcStart = DateAndTime.DateAdd(DateInterval.Day, 0 - iCalDaysBefore, DateTime.UtcNow);
                    var utcEnd = DateAndTime.DateAdd(DateInterval.Day, iCalDaysAfter, DateTime.UtcNow);
                    if (utcEventTimeEnd > utcStart && utcEventTimeBegin < utcEnd)
                    {
                        vEvents.Append(this.CreatevEvent(oEvent, oEvent.EventTimeBegin, false, false));
                    }
                }
            }
            else
            {
                // Process the single event
                vEvents.Append(this.CreatevEvent(oEvent, oEvent.EventTimeBegin, false, false));
                this._icsFilename = oEvent.EventName;
            }

            // Create the initial VCALENDAR
            var vCalendar = new StringBuilder();
            vCalendar.Append("BEGIN:VCALENDAR" + Environment.NewLine);
            vCalendar.Append("VERSION:2.0" + Environment.NewLine);
            vCalendar.Append(this.FoldText("PRODID:-//DNN//" + objDesktopModule.FriendlyName + " " +
                                           objDesktopModule.Version + "//EN") + Environment.NewLine);
            vCalendar.Append("CALSCALE:GREGORIAN" + Environment.NewLine);
            vCalendar.Append("METHOD:PUBLISH" + Environment.NewLine);
            if (calname != "")
            {
                vCalendar.Append("X-WR-CALNAME:" + calname + Environment.NewLine);
            }
            else if (this._settings.IcalIncludeCalname)
            {
                vCalendar.Append("X-WR-CALNAME:" + this._icsFilename + Environment.NewLine);
            }

            // Create the VTIMEZONE
            if (this._timeZoneStart == DateTime.MinValue)
            {
                this._timeZoneStart = DateTime.Now;
                this._timeZoneEnd = DateAndTime.DateAdd(DateInterval.Minute, 30, this._timeZoneStart);
            }

            vCalendar.Append(this.CreatevTimezones(this._timeZoneStart, this._timeZoneEnd));

            // Output the events
            vCalendar.Append(vEvents);

            // Close off the VCALENDAR
            vCalendar.Append("END:VCALENDAR" + Environment.NewLine);

            return vCalendar.ToString();
        }

        private string CreatevTimezones(DateTime dtStartDate, DateTime dtEndDate)
        {
            var vTimezone = new StringBuilder();
            foreach (string vTimeZoneId in this._vTimeZoneIds)
            {
                var tzi = TimeZoneInfo.FindSystemTimeZoneById(vTimeZoneId);
                var adjustments = tzi.GetAdjustmentRules();

                vTimezone.Append("BEGIN:VTIMEZONE" + Environment.NewLine);
                vTimezone.Append("TZID:" + tzi.Id + Environment.NewLine);

                if (adjustments.Any())
                {
                    // Identify last DST change before start of recurrences
                    var intYear = 0;
                    var lastDSTStartDate = new DateTime();
                    var lastDSTEndDate = new DateTime();
                    for (intYear = dtStartDate.Year - 1; intYear <= dtEndDate.Year; intYear++)
                    {
                        var yearDayLight = this.GetAdjustment(adjustments, intYear);
                        var daylightStart = yearDayLight.StartDate;
                        var daylightEnd = yearDayLight.EndDate;
                        if ((lastDSTStartDate == DateTime.MinValue || daylightStart > lastDSTStartDate) &&
                            daylightStart < dtStartDate)
                        {
                            lastDSTStartDate = daylightStart;
                        }
                        if ((lastDSTEndDate == DateTime.MinValue || daylightEnd > lastDSTEndDate) &&
                            daylightEnd < dtStartDate)
                        {
                            lastDSTEndDate = daylightEnd;
                        }
                    }
                    for (intYear = dtStartDate.Year - 1; intYear <= dtEndDate.Year; intYear++)
                    {
                        var yearDayLight = this.GetAdjustment(adjustments, intYear);
                        var daylightStart = yearDayLight.StartDate;
                        var daylightEnd = yearDayLight.EndDate;

                        var iByDay = Conversion.Int((double) daylightEnd.Day / 7) + 1;
                        var sDayOfWeek = "";
                        switch (daylightEnd.DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                sDayOfWeek = "SU";
                                break;
                            case DayOfWeek.Monday:
                                sDayOfWeek = "MO";
                                break;
                            case DayOfWeek.Tuesday:
                                sDayOfWeek = "TU";
                                break;
                            case DayOfWeek.Wednesday:
                                sDayOfWeek = "WE";
                                break;
                            case DayOfWeek.Thursday:
                                sDayOfWeek = "TH";
                                break;
                            case DayOfWeek.Friday:
                                sDayOfWeek = "FR";
                                break;
                            case DayOfWeek.Saturday:
                                sDayOfWeek = "SA";
                                break;
                        }

                        var sByDay = iByDay + sDayOfWeek;
                        var sByMonth = daylightEnd.Month.ToString();

                        // Allow for timezone with no DST, and don't include Timezones after end
                        if (daylightStart.Year > 1 && daylightStart < dtEndDate && !(daylightStart < lastDSTStartDate))
                        {
                            var dtFrom =
                                this.FormatTZTime(
                                    tzi.GetUtcOffset(DateAndTime.DateAdd(DateInterval.Day, -2, daylightStart)));
                            var dtTo = this.FormatTZTime(
                                tzi.GetUtcOffset(
                                    DateAndTime.DateAdd(DateInterval.Day, Convert.ToDouble(+2), daylightStart)));
                            vTimezone.Append("BEGIN:DAYLIGHT" + Environment.NewLine);
                            vTimezone.Append("TZOFFSETFROM:" + dtFrom + Environment.NewLine);
                            vTimezone.Append("TZOFFSETTO:" + dtTo + Environment.NewLine);
                            vTimezone.Append("DTSTART:" +
                                             CreateTZIDDate(daylightStart, daylightStart, false, false, tzi.Id) +
                                             Environment.NewLine);
                            vTimezone.Append("END:DAYLIGHT" + Environment.NewLine);
                        }

                        // Allow for timezone with no DST, and don't include Timezones after end
                        if (daylightEnd.Year > 1 && daylightEnd < dtEndDate && !(daylightEnd < lastDSTEndDate))
                        {
                            var dtFrom =
                                this.FormatTZTime(
                                    tzi.GetUtcOffset(DateAndTime.DateAdd(DateInterval.Day, -2, daylightEnd)));
                            var dtTo = this.FormatTZTime(
                                tzi.GetUtcOffset(
                                    DateAndTime.DateAdd(DateInterval.Day, Convert.ToDouble(+2), daylightEnd)));
                            vTimezone.Append("BEGIN:STANDARD" + Environment.NewLine);
                            vTimezone.Append(this.FoldText("RRULE:FREQ=YEARLY;INTERVAL=1;BYMONTH=" + sByMonth +
                                                           ";BYDAY=" + sByDay + ";COUNT=1") + Environment.NewLine);
                            vTimezone.Append("TZOFFSETFROM:" + dtFrom + Environment.NewLine);
                            vTimezone.Append("TZOFFSETTO:" + dtTo + Environment.NewLine);
                            vTimezone.Append("DTSTART:" +
                                             CreateTZIDDate(daylightEnd, daylightEnd, false, false, tzi.Id) +
                                             Environment.NewLine);
                            vTimezone.Append("END:STANDARD" + Environment.NewLine);
                        }

                        if (!(daylightStart.Year > 1))
                        {
                            vTimezone.Append(this.CreatevTimezone1601(tzi));
                            break;
                        }
                    }
                }
                else
                {
                    vTimezone.Append(this.CreatevTimezone1601(tzi));
                }
                vTimezone.Append("END:VTIMEZONE" + Environment.NewLine);
            }
            return vTimezone.ToString();
        }

        private YearDayLight GetAdjustment(IEnumerable<TimeZoneInfo.AdjustmentRule> adjustments, int year)
        {
            // Iterate adjustment rules for time zone
            foreach (var adjustment in adjustments)
            {
                // Determine if this adjustment rule covers year desired
                if ((adjustment.DateStart.Year <= year) & (adjustment.DateEnd.Year >= year))
                {
                    var startTransition = default(TimeZoneInfo.TransitionTime);
                    var endTransition = default(TimeZoneInfo.TransitionTime);
                    var yearDayLight = new YearDayLight();
                    startTransition = adjustment.DaylightTransitionStart;
                    yearDayLight.StartDate = this.ProcessAdjustmentDate(startTransition, year);
                    endTransition = adjustment.DaylightTransitionEnd;
                    yearDayLight.EndDate = this.ProcessAdjustmentDate(endTransition, year);
                    yearDayLight.Delta = Convert.ToInt32(adjustment.DaylightDelta.TotalMinutes);
                    return yearDayLight;
                }
            }
            return null;
        }

        // VBConversions Note: Former VB static variables moved to class level because they aren't supported in C#.
        private readonly Calendar ProcessAdjustmentDate_cal = CultureInfo.CurrentCulture.Calendar;

        private DateTime ProcessAdjustmentDate(TimeZoneInfo.TransitionTime processTransition, int year)
        {
            var transitionDay = 0;
            if (processTransition.IsFixedDateRule)
            {
                transitionDay = processTransition.Day;
            }
            else
            {
                // For non-fixed date rules, get local calendar
                // static Calendar cal = CultureInfo.CurrentCulture.Calendar; VBConversions Note: Static variable moved to class level and renamed ProcessAdjustmentDate_cal. Local static variables are not supported in C#.

                // Get first day of week for transition
                // For example, the 3rd week starts no earlier than the 15th of the month
                var startOfWeek = processTransition.Week * 7 - 6;
                // What day of the week does the month start on
                var firstDayOfWeek =
                    (int) this.ProcessAdjustmentDate_cal.GetDayOfWeek(new DateTime(year, processTransition.Month, 1));
                // Determine how much start date has to be adjusted
                var changeDayOfWeek = (int) processTransition.DayOfWeek;

                if (firstDayOfWeek <= changeDayOfWeek)
                {
                    transitionDay = startOfWeek + (changeDayOfWeek - firstDayOfWeek);
                }
                else
                {
                    transitionDay = startOfWeek + (7 - firstDayOfWeek) + changeDayOfWeek;
                }
                // Adjust for months with no fifth week
                if (transitionDay > this.ProcessAdjustmentDate_cal.GetDaysInMonth(year, processTransition.Month))
                {
                    transitionDay -= 7;
                }
            }
            return DateTime.ParseExact(
                string.Format("{0:0000}/{1:00}/{2:00} {3:HH:mm}", year, processTransition.Month, transitionDay,
                              processTransition.TimeOfDay), "yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture);
        }

        private string CreatevTimezone1601(TimeZoneInfo tzi)
        {
            var vTimezone1601 = new StringBuilder();
            var invCuluture = CultureInfo.InvariantCulture;
            var dt1601Date = DateTime.ParseExact("01/01/1601 00:00:00", "MM/dd/yyyy HH:mm:ss", invCuluture);
            var dtTo = this.FormatTZTime(tzi.GetUtcOffset(dt1601Date));
            var dtFrom = this.FormatTZTime(tzi.GetUtcOffset(dt1601Date));
            vTimezone1601.Append("BEGIN:STANDARD" + Environment.NewLine);
            vTimezone1601.Append("TZOFFSETFROM:" + dtFrom + Environment.NewLine);
            vTimezone1601.Append("TZOFFSETTO:" + dtTo + Environment.NewLine);
            vTimezone1601.Append("DTSTART:" + CreateTZIDDate(dt1601Date, dt1601Date, false, false, tzi.Id) +
                                 Environment.NewLine);
            vTimezone1601.Append("END:STANDARD" + Environment.NewLine);
            return vTimezone1601.ToString();
        }

        private string CreateRecurvEvent(EventRecurMasterInfo oEventRecurMaster)
        {
            var vEvent = new StringBuilder();

            var lstEvents = default(ArrayList);
            var oEvent = default(EventInfo);
            var oCntrl = new EventController();
            lstEvents = oCntrl.EventsGetRecurrences(oEventRecurMaster.RecurMasterID, oEventRecurMaster.ModuleID);
            // Create the single VEVENT
            foreach (EventInfo tempLoopVar_oEvent in lstEvents)
            {
                oEvent = tempLoopVar_oEvent;
                if (oEvent.EventTimeBegin.TimeOfDay == oEventRecurMaster.Dtstart.TimeOfDay &&
                    Convert.ToString(oEvent.Duration) + "M" == oEventRecurMaster.Duration &&
                    oEvent.EventName == oEventRecurMaster.EventName &&
                    oEvent.EventDesc == oEventRecurMaster.EventDesc &&
                    (oEvent.OwnerID == oEventRecurMaster.OwnerID) &
                    (oEvent.Location == oEventRecurMaster.Location) &&
                    (int) oEvent.Importance == (int) oEventRecurMaster.Importance &&
                    oEvent.SendReminder == oEventRecurMaster.SendReminder &&
                    oEvent.ReminderTime == oEventRecurMaster.ReminderTime &&
                    oEvent.AllDayEvent == oEventRecurMaster.AllDayEvent &&
                    oEvent.ReminderTimeMeasurement == oEventRecurMaster.ReminderTimeMeasurement &&
                    oEvent.Cancelled == false &&
                    (oEvent.Enrolled == 0 || !this._blEventSignup || !oEvent.EnrollListView || !oEvent.Signups))
                {
                    if (!this._blImages ||
                        this._blImages && oEvent.ImageDisplay == oEventRecurMaster.ImageDisplay &&
                        oEvent.ImageURL == oEventRecurMaster.ImageURL)
                    {
                        continue;
                    }
                }
                vEvent.Append(
                    this.CreatevEvent(oEvent, oEventRecurMaster.Dtstart, false, oEventRecurMaster.AllDayEvent));
            }

            if (lstEvents.Count == 0)
            {
                return "";
            }
            var oFirstEvent = (EventInfo) lstEvents[0];
            var objEventLocation = new EventLocationInfo();
            var objCtlEventLocation = new EventLocationController();

            if (oEventRecurMaster.Location > 0)
            {
                objEventLocation =
                    objCtlEventLocation.EventsLocationGet(oEventRecurMaster.Location, oEventRecurMaster.PortalID);
            }

            var objEventTimeZoneUtilities = new EventTimeZoneUtilities();
            var recurUntil =
                objEventTimeZoneUtilities.ConvertToUTCTimeZone(oEventRecurMaster.Until,
                                                               oEventRecurMaster.EventTimeZoneId);

            // Calculate timezone start/end dates
            var intDuration = 0;
            intDuration = int.Parse(oEventRecurMaster.Duration.Substring(0, oEventRecurMaster.Duration.Length - 1));
            if (this._timeZoneStart == DateTime.MinValue || this._timeZoneStart > oEventRecurMaster.Dtstart)
            {
                this._timeZoneStart = oEventRecurMaster.Dtstart;
            }
            if (this._timeZoneEnd == DateTime.MinValue || this._timeZoneEnd <
                DateAndTime.DateAdd(DateInterval.Minute, intDuration, oEventRecurMaster.Until))
            {
                this._timeZoneEnd = DateAndTime.DateAdd(DateInterval.Minute, intDuration, oEventRecurMaster.Until);
            }

            // Build Item
            var objUsers = new UserController();
            var objUser = objUsers.GetUser(oEventRecurMaster.PortalID, oEventRecurMaster.OwnerID);
            var creatoremail = "";
            var creatoranonemail = "";
            var creatorname = "";
            if (!ReferenceEquals(objUser, null))
            {
                creatoremail = ":MAILTO:" + objUser.Email;
                creatoranonemail = ":MAILTO:" + objUser.FirstName + "." + objUser.LastName + "@no_email.com";
                creatorname = "CN=\"" + objUser.DisplayName + "\"";
            }
            else
            {
                var objPortals = new PortalController();
                var objPortal = default(PortalInfo);
                objPortal = objPortals.GetPortal(oEventRecurMaster.PortalID);
                creatoremail = ":MAILTO:" + objPortal.Email;
                creatoranonemail = ":MAILTO:" + "anonymous@no_email.com";
                creatorname = "CN=\"Anonymous\"";
            }

            var sEmail = "";
            if (this._oContext.Request.IsAuthenticated && this._blOwnerEmail || this._blAnonOwnerEmail)
            {
                sEmail = this.FoldText("ORGANIZER;" + creatorname + creatoremail) + Environment.NewLine;
            }
            else
            {
                sEmail = this.FoldText("ORGANIZER;" + creatorname + creatoranonemail) + Environment.NewLine;
            }

            var aTimes = default(ArrayList);
            var sStartTime = "";
            var sEndTime = "";
            var sDtStamp = "";
            var sSequence = "";

            aTimes = TimeFormat(oFirstEvent.OriginalDateBegin.Date + oEventRecurMaster.Dtstart.TimeOfDay, intDuration,
                                oFirstEvent.EventTimeZoneId, recurUntil);
            if (!oEventRecurMaster.AllDayEvent)
            {
                sStartTime = "DTSTART;" + Convert.ToString(aTimes[0]) + Environment.NewLine;
                sEndTime = "DTEND;" + Convert.ToString(aTimes[1]) + Environment.NewLine;
            }
            else
            {
                sStartTime = "DTSTART;VALUE=DATE:" + AllDayEventDate(oFirstEvent.OriginalDateBegin.Date) +
                             Environment.NewLine;
                //  +1 deals with use of 1439 minutes instead of 1440
                sEndTime = "DTEND;VALUE=DATE:" +
                           AllDayEventDate(oFirstEvent.OriginalDateBegin.Date.AddMinutes(intDuration + 1)) +
                           Environment.NewLine;
            }
            sDtStamp = "DTSTAMP:" +
                       CreateTZIDDate(DateTime.UtcNow, DateTime.UtcNow, true, false,
                                      oEventRecurMaster.EventTimeZoneId) + Environment.NewLine;
            sSequence = "SEQUENCE:" + Convert.ToString(oEventRecurMaster.Sequence) + Environment.NewLine;

            var sLocation = "";
            if (oEventRecurMaster.Location > 0)
            {
                if (objEventLocation.MapURL != "" && this._settings.IcalURLInLocation)
                {
                    sLocation = objEventLocation.LocationName + " - " + objEventLocation.MapURL;
                }
                else
                {
                    sLocation = objEventLocation.LocationName;
                }
                sLocation = this.FoldText("LOCATION:" + this.CreateText(sLocation)) + Environment.NewLine;
            }
            var sDescription = this.CreateDescription(oEventRecurMaster.EventDesc);
            var altDescription = this.CreateAltDescription(oEventRecurMaster.EventDesc);

            // ToDo: HIER!
            // Make up the LocalResourceFile value
            var templateSourceDirectory = Globals.ApplicationPath;
            var localResourceFile = templateSourceDirectory + "/DesktopModules/Events/" +
                                    Localization.LocalResourceDirectory + "/SharedResources.ascx.resx";
            var tcc = new TokenReplaceControllerClass(this._moduleID, localResourceFile);

            var tmpSummary = this._settings.Templates.EventiCalSubject;
            //tmpSummary = tcc.TokenReplaceEvent(oEventRecurMaster, tmpSummary)
            //Dim sSummary As String = FoldText("SUMMARY:" & CreateText(oEventRecurMaster.EventName)) & Environment.NewLine

            var sSummary = this.FoldText("SUMMARY-RM:" + this.CreateText(oEventRecurMaster.EventName)) +
                           Environment.NewLine;
            var sPriority = "PRIORITY:" + this.Priority((int) oEventRecurMaster.Importance) + Environment.NewLine;

            var sURL = this.FoldText("URL:" + this._objEventInfoHelper.DetailPageURL(oFirstEvent, false) +
                                     this._iCalURLAppend) + Environment.NewLine;

            vEvent.Append("BEGIN:VEVENT" + Environment.NewLine);
            var strUID = string.Format("{0:00000}", oEventRecurMaster.ModuleID) +
                         string.Format("{0:0000000}", oEventRecurMaster.RecurMasterID);
            vEvent.Append("UID:DNNEvent" + strUID + "@" + this._domainName + Environment.NewLine);
            vEvent.Append(sSequence);
            if (oEventRecurMaster.RRULE != "")
            {
                vEvent.Append("RRULE:" + oEventRecurMaster.RRULE + ";" + "UNTIL=" + Convert.ToString(aTimes[2]) +
                              Environment.NewLine);
            }
            if (this._sExdate.ToString() != "")
            {
                vEvent.Append(this._sExdate);
            }
            vEvent.Append(sStartTime);
            vEvent.Append(sEndTime);
            vEvent.Append(sDtStamp);
            vEvent.Append(sURL);
            vEvent.Append(sEmail);
            vEvent.Append(sDescription);
            vEvent.Append(sSummary);
            vEvent.Append(altDescription);
            vEvent.Append(sPriority);
            vEvent.Append(sLocation);

            var iMinutes = 0;
            if (oEventRecurMaster.SendReminder)
            {
                switch (oEventRecurMaster.ReminderTimeMeasurement)
                {
                    case "d":
                        iMinutes = oEventRecurMaster.ReminderTime * 60 * 24;
                        break;
                    case "h":
                        iMinutes = oEventRecurMaster.ReminderTime * 60;
                        break;
                    case "m":
                        iMinutes = oEventRecurMaster.ReminderTime;
                        break;
                }
            }

            vEvent.Append("CLASS:PUBLIC" + Environment.NewLine);
            vEvent.Append("CREATED:" +
                          CreateTZIDDate(oEventRecurMaster.CreatedDate, oEventRecurMaster.CreatedDate, true, false,
                                         oEventRecurMaster.EventTimeZoneId) + Environment.NewLine);
            vEvent.Append("LAST-MODIFIED:" +
                          CreateTZIDDate(oEventRecurMaster.UpdatedDate, oEventRecurMaster.UpdatedDate, true, false,
                                         oEventRecurMaster.EventTimeZoneId) + Environment.NewLine);
            if (oEventRecurMaster.SendReminder)
            {
                vEvent.Append("BEGIN:VALARM" + Environment.NewLine);
                vEvent.Append("TRIGGER:-PT" + iMinutes + "M" + Environment.NewLine);
                vEvent.Append("ACTION:DISPLAY" + Environment.NewLine);
                vEvent.Append("DESCRIPTION:Reminder" + Environment.NewLine);
                vEvent.Append("END:VALARM" + Environment.NewLine);
            }

            if (this._blImages && oEventRecurMaster.ImageDisplay)
            {
                vEvent.Append(
                    this.FoldText(
                        "ATTACH:" + this.GetImageUrl(oEventRecurMaster.ImageURL, oEventRecurMaster.PortalID)) +
                    Environment.NewLine);
            }
            else if (this._blImages && !string.IsNullOrEmpty(this._iCalDefaultImage))
            {
                vEvent.Append(
                    this.FoldText("ATTACH:" + this.GetImageUrl(this._iCalDefaultImage, oEventRecurMaster.PortalID)) +
                    Environment.NewLine);
            }

            vEvent.Append("TRANSP:OPAQUE" + Environment.NewLine);

            vEvent.Append("END:VEVENT" + Environment.NewLine);

            return vEvent.ToString();
        }

        private string CreatevEvent(EventInfo oEvent, DateTime dtstart, bool blURLOnly, bool blAllDay)
        {
            if (!this._vTimeZoneIds.Contains(oEvent.EventTimeZoneId))
            {
                this._vTimeZoneIds.Add(oEvent.EventTimeZoneId);
            }

            oEvent.OriginalDateBegin = oEvent.OriginalDateBegin.Date + dtstart.TimeOfDay;

            // Calculate timezone start/end dates
            if (this._timeZoneStart == DateTime.MinValue || this._timeZoneStart > oEvent.EventTimeBegin)
            {
                this._timeZoneStart = oEvent.EventTimeBegin;
            }
            if (this._timeZoneEnd == DateTime.MinValue || this._timeZoneEnd <
                DateAndTime.DateAdd(DateInterval.Minute, oEvent.Duration, oEvent.EventTimeBegin))
            {
                this._timeZoneEnd = DateAndTime.DateAdd(DateInterval.Minute, oEvent.Duration, oEvent.EventTimeBegin);
            }

            // Build Item
            var vEvent = new StringBuilder();

            var objUsers = new UserController();
            var objUser = objUsers.GetUser(oEvent.PortalID, oEvent.OwnerID);
            var creatoremail = "";
            var creatoranonemail = "";
            var creatorname = "";
            if (!ReferenceEquals(objUser, null))
            {
                creatoremail = ":MAILTO:" + objUser.Email;
                creatoranonemail = ":MAILTO:" + objUser.FirstName + "." + objUser.LastName + "@no_email.com";
                creatorname = "CN=\"" + objUser.DisplayName + "\"";
            }
            else
            {
                var objPortals = new PortalController();
                var objPortal = default(PortalInfo);
                objPortal = objPortals.GetPortal(oEvent.PortalID);
                creatoremail = ":MAILTO:" + objPortal.Email;
                creatoranonemail = ":MAILTO:" + "anonymous@no_email.com";
                creatorname = "CN=\"Anonymous\"";
            }

            var sEmail = "";
            if (this._oContext.Request.IsAuthenticated && this._blOwnerEmail || this._blAnonOwnerEmail)
            {
                sEmail = this.FoldText("ORGANIZER;" + creatorname + creatoremail) + Environment.NewLine;
            }
            else
            {
                sEmail = this.FoldText("ORGANIZER;" + creatorname + creatoranonemail) + Environment.NewLine;
            }

            var aTimes = default(ArrayList);
            var sStartTime = "";
            var sEndTime = "";
            var sDtStamp = "";
            var sSequence = "";

            aTimes = TimeFormat(oEvent.EventTimeBegin, oEvent.Duration, oEvent.EventTimeZoneId,
                                dOriginal: oEvent.OriginalDateBegin);
            if (oEvent.Cancelled)
            {
                this._sExdate.Append("EXDATE;" + Convert.ToString(aTimes[3]) + Environment.NewLine);
                return "";
            }

            if (!oEvent.AllDayEvent)
            {
                sStartTime = "DTSTART;" + Convert.ToString(aTimes[0]) + Environment.NewLine;
                sEndTime = "DTEND;" + Convert.ToString(aTimes[1]) + Environment.NewLine;
            }
            else
            {
                sStartTime = "DTSTART;VALUE=DATE:" + AllDayEventDate(oEvent.EventTimeBegin.Date) + Environment.NewLine;
                //  +1 deals with use of 1439 minutes instead of 1440
                sEndTime = "DTEND;VALUE=DATE:" +
                           AllDayEventDate(oEvent.EventTimeBegin.Date.AddMinutes(oEvent.Duration + 1)) +
                           Environment.NewLine;
            }
            if (!this._blSeries)
            {
                sDtStamp = "DTSTAMP:" +
                           CreateTZIDDate(DateTime.UtcNow, DateTime.UtcNow, true, false, oEvent.EventTimeZoneId) +
                           Environment.NewLine;
            }
            sSequence = "SEQUENCE:" + Convert.ToString(oEvent.Sequence) + Environment.NewLine;

            var sLocation = "";
            if (oEvent.Location > 0)
            {
                if (oEvent.MapURL != "" && this._settings.IcalURLInLocation)
                {
                    sLocation = oEvent.LocationName + " - " + oEvent.MapURL;
                }
                else
                {
                    sLocation = oEvent.LocationName;
                }
                sLocation = this.FoldText("LOCATION:" + this.CreateText(sLocation)) + Environment.NewLine;
            }

            var sDescription = this.CreateDescription(oEvent.EventDesc);
            var altDescription = this.CreateAltDescription(oEvent.EventDesc);


            //Create the templated version of the summary
            var templateSourceDirectory = Globals.ApplicationPath;
            var localResourceFile = templateSourceDirectory + "/DesktopModules/Events/" +
                                    Localization.LocalResourceDirectory + "/SharedResources.ascx.resx";
            var tcc = new TokenReplaceControllerClass(this._moduleID, localResourceFile);
            var tmpSummary = this._settings.Templates.EventiCalSubject;
            tmpSummary = tcc.TokenReplaceEvent(oEvent, tmpSummary);

            var sSummary = this.FoldText("SUMMARY:" + this.CreateText(tmpSummary)) + Environment.NewLine;

            var sPriority = "PRIORITY:" + this.Priority((int) oEvent.Importance) + Environment.NewLine;

            var sURL = this.FoldText("URL:" + this._objEventInfoHelper.DetailPageURL(oEvent, false) +
                                     this._iCalURLAppend) + Environment.NewLine;

            vEvent.Append("BEGIN:VEVENT" + Environment.NewLine);
            var strUID = string.Format("{0:00000}", oEvent.ModuleID) +
                         string.Format("{0:0000000}", oEvent.RecurMasterID);
            if (!this._blSeries)
            {
                strUID += string.Format("{0:0000000}", oEvent.EventID);
            }
            vEvent.Append("UID:DNNEvent" + strUID + "@" + this._domainName + Environment.NewLine);
            vEvent.Append(sSequence);
            if (this._blSeries)
            {
                if (!blAllDay)
                {
                    vEvent.Append("RECURRENCE-ID;" + Convert.ToString(aTimes[3]) + Environment.NewLine);
                }
                else
                {
                    vEvent.Append("RECURRENCE-ID;VALUE=DATE:" + AllDayEventDate(oEvent.OriginalDateBegin) +
                                  Environment.NewLine);
                }
            }
            vEvent.Append(sStartTime);
            vEvent.Append(sEndTime);
            vEvent.Append(sDtStamp);
            vEvent.Append(sURL);
            if (!blURLOnly)
            {
                vEvent.Append(sEmail);
                vEvent.Append(sDescription);
                vEvent.Append(sSummary);
                vEvent.Append(altDescription);
                vEvent.Append(sPriority);
                vEvent.Append(sLocation);

                if (this._blEventSignup && oEvent.EnrollListView && oEvent.Signups && oEvent.Enrolled > 0)
                {
                    this._blEnrolleeEmail = false;
                    if ((this.IsModerator() || (oEvent.CreatedByID == this._iUserid) |
                         (oEvent.RmOwnerID == this._iUserid) | (oEvent.OwnerID == this._iUserid)) && this._blEditEmail)
                    {
                        this._blEnrolleeEmail = true;
                    }
                    if (this._oContext.Request.IsAuthenticated && (this._blViewEmail || this._blAnonEmail))
                    {
                        this._blEnrolleeEmail = true;
                    }
                    if (!this._oContext.Request.IsAuthenticated && this._blAnonEmail)
                    {
                        this._blEnrolleeEmail = true;
                    }
                    vEvent.Append(this.CreateAttendee(oEvent));
                }

                var iMinutes = 0;
                if (oEvent.SendReminder)
                {
                    switch (oEvent.ReminderTimeMeasurement)
                    {
                        case "d":
                            iMinutes = oEvent.ReminderTime * 60 * 24;
                            break;
                        case "h":
                            iMinutes = oEvent.ReminderTime * 60;
                            break;
                        case "m":
                            iMinutes = oEvent.ReminderTime;
                            break;
                    }
                }

                vEvent.Append("CLASS:PUBLIC" + Environment.NewLine);
                vEvent.Append("CREATED:" +
                              CreateTZIDDate(oEvent.CreatedDate, oEvent.CreatedDate, true, false,
                                             oEvent.EventTimeZoneId) + Environment.NewLine);
                vEvent.Append("LAST-MODIFIED:" +
                              CreateTZIDDate(oEvent.LastUpdatedAt, oEvent.LastUpdatedAt, true, false,
                                             oEvent.EventTimeZoneId) + Environment.NewLine);
                if (oEvent.SendReminder)
                {
                    vEvent.Append("BEGIN:VALARM" + Environment.NewLine);
                    vEvent.Append("TRIGGER:-PT" + iMinutes + "M" + Environment.NewLine);
                    vEvent.Append("ACTION:DISPLAY" + Environment.NewLine);
                    vEvent.Append("DESCRIPTION:Reminder" + Environment.NewLine);
                    vEvent.Append("END:VALARM" + Environment.NewLine);
                }

                if (this._blImages && oEvent.ImageDisplay)
                {
                    vEvent.Append(this.FoldText("ATTACH:" + this.GetImageUrl(oEvent.ImageURL, oEvent.PortalID)) +
                                  Environment.NewLine);
                }
                else if (this._blImages && !string.IsNullOrEmpty(this._iCalDefaultImage))
                {
                    vEvent.Append(this.FoldText("ATTACH:" + this.GetImageUrl(this._iCalDefaultImage, oEvent.PortalID)) +
                                  Environment.NewLine);
                }
            }

            vEvent.Append("TRANSP:OPAQUE" + Environment.NewLine);

            vEvent.Append("END:VEVENT" + Environment.NewLine);

            return vEvent.ToString();
        }

        private string CreateAttendee(EventInfo oEvent)
        {
            var attendees = new StringBuilder();
            var oSignups = default(ArrayList);
            var oSignup = default(EventSignupsInfo);
            var oCtlEventSignups = new EventSignupsController();
            var objUsers = new UserController();
            oSignups = oCtlEventSignups.EventsSignupsGetEvent(oEvent.EventID, oEvent.ModuleID);
            foreach (EventSignupsInfo tempLoopVar_oSignup in oSignups)
            {
                oSignup = tempLoopVar_oSignup;
                var objUser = objUsers.GetUser(oEvent.PortalID, oSignup.UserID);
                var attendeeemail = "";
                var attendeeanonemail = "";
                var attendeename = "";
                var sPartStat = "ACCEPTED";
                if (!oSignup.Approved)
                {
                    sPartStat = "NEEDS-ACTION";
                }
                if (!ReferenceEquals(objUser, null))
                {
                    attendeeemail = ":MAILTO:" + objUser.Email;
                    attendeeanonemail = ":MAILTO:" + objUser.FirstName + "." + objUser.LastName + "@no_email.com";
                    attendeename = "CN=\"" + objUser.DisplayName + "\"";
                }
                else
                {
                    attendeeemail = ":MAILTO:" + "anonymous@no_email.com";
                    attendeeanonemail = ":MAILTO:" + "anonymous@no_email.com";
                    attendeename = "CN=\"Anonymous-" + oSignup.UserID + "\"";
                }
                var sAttendee = "";
                if (this._blEnrolleeEmail)
                {
                    sAttendee = "ATTENDEE;ROLE=REQ-PARTICIPANT;PARTSTAT=" + sPartStat + ";" + attendeename +
                                attendeeemail + Environment.NewLine;
                }
                else
                {
                    sAttendee = "ATTENDEE;ROLE=REQ-PARTICIPANT;PARTSTAT=" + sPartStat + ";" + attendeename +
                                attendeeanonemail + Environment.NewLine;
                }
                attendees.Append(sAttendee);
            }
            return attendees.ToString();
        }

        #endregion

        #region  Private Support Functions

        private static ArrayList TimeFormat(DateTime dBeginDateTime, int iDuration, string timeZoneId,
                                            DateTime dUntil = default(DateTime), DateTime dOriginal = default(DateTime))
        {
            // VBConversions Note: dUntil assigned to default value below, since optional parameter values must be static and C# doesn't support date literals.
            if (dUntil == default(DateTime))
            {
                dUntil = DateTime.MinValue;
            }

            // VBConversions Note: dOriginal assigned to default value below, since optional parameter values must be static and C# doesn't support date literals.
            if (dOriginal == default(DateTime))
            {
                dOriginal = DateTime.MinValue;
            }

            var aTimes = new ArrayList();
            var eDate = default(DateTime);
            var tempDateTime = dBeginDateTime.Date + dBeginDateTime.TimeOfDay;

            //Begin Time Format
            aTimes.Add(CreateTZIDDate(dBeginDateTime, dBeginDateTime, false, true, timeZoneId));

            //End Time Format
            eDate = tempDateTime.AddMinutes(iDuration);
            aTimes.Add(CreateTZIDDate(eDate, eDate, false, true, timeZoneId));

            //Until Time Format
            var invCuluture = CultureInfo.InvariantCulture;
            aTimes.Add(CreateTZIDDate(
                           dUntil, DateTime.ParseExact("01/01/2002 23:59:59", "MM/dd/yyyy HH:mm:ss", invCuluture), true,
                           false, timeZoneId));

            //Original Time Format
            aTimes.Add(CreateTZIDDate(dOriginal, dOriginal, false, true, timeZoneId));

            return aTimes;
        }

        private static string CreateTZIDDate(DateTime dDate, DateTime dTime, bool blUTC, bool blTZAdd,
                                             string timeZoneId)
        {
            var sTime = new StringBuilder();
            if (blTZAdd)
            {
                sTime.Append("TZID=\"" + timeZoneId + "\":");
            }
            sTime.Append(dDate.Year.ToString());
            sTime.Append(Strings.Format(dDate.Month, "0#"));
            sTime.Append(Strings.Format(dDate.Day, "0#"));
            sTime.Append("T");
            sTime.Append(Strings.Format(dTime.Hour, "0#"));
            sTime.Append(Strings.Format(dTime.Minute, "0#"));
            sTime.Append(Strings.Format(dTime.Second, "0#"));
            if (blUTC)
            {
                sTime.Append("Z");
            }
            return sTime.ToString();
        }

        private static string AllDayEventDate(DateTime dDate)
        {
            var sDate = new StringBuilder();
            sDate.Append(dDate.Year.ToString());
            sDate.Append(Strings.Format(dDate.Month, "0#"));
            sDate.Append(Strings.Format(dDate.Day, "0#"));
            return sDate.ToString();
        }

        private string Priority(int importance)
        {
            switch (importance)
            {
                case 1:
                    return "1";
                case 3:
                    return "9";
                default:
                    return "5";
            }
        }

        private string CreateAltDescription(string eventDesc)
        {
            var altDescription = "X-ALT-DESC;FMTTYPE=text/html:<!DOCTYPE HTML PUBLIC \" -//W3C//DTD HTML 3.2//EN\">\\n";
            altDescription += "<HTML>\\n";
            altDescription += "<HEAD>\\n";
            altDescription += "<META NAME=\"Generator\" CONTENT=\"DNN Events Module\">\\n";
            altDescription += "<TITLE></TITLE>\\n";
            altDescription += "</HEAD>\\n";
            altDescription += "<BODY>\\n";
            altDescription += HtmlUtils.StripWhiteSpace(HttpUtility.HtmlDecode(eventDesc), true)
                                       .Replace(Environment.NewLine, "") + "\\n";
            altDescription += "</BODY>\\n";
            altDescription += "</HTML>";
            altDescription = this.FoldText(altDescription) + Environment.NewLine;

            return altDescription;
        }

        private string CreateDescription(string eventDesc)
        {
            var sDescription = "DESCRIPTION:";
            const int descriptionLength = 1950;
            var tmpDesc = this.CreateText(eventDesc);
            tmpDesc = HtmlUtils.Shorten(tmpDesc, descriptionLength, "...");
            sDescription = this.FoldText(sDescription + tmpDesc + "\\n") + Environment.NewLine;
            return sDescription;
        }

        private string CreateText(string eventText)
        {
            var tmpText = HtmlUtils.StripTags(HttpUtility.HtmlDecode(eventText), false);
            // Double decode, for things that were encoded by RadEditor
            tmpText = HttpUtility.HtmlDecode(tmpText);
            tmpText = tmpText.Replace("\\", "\\\\");
            tmpText = tmpText.Replace(",", "\\,");
            tmpText = tmpText.Replace(";", "\\;");
            tmpText = tmpText.Replace(Environment.NewLine, "\\n");
            return tmpText;
        }

        private string FormatTZTime(TimeSpan dtTimeSpan)
        {
            var dtSign = "+";
            if (dtTimeSpan.Hours < 0)
            {
                dtSign = "-";
            }
            return dtSign + Strings.Format(Math.Abs(dtTimeSpan.Hours), "0#") +
                   Strings.Format(Math.Abs(dtTimeSpan.Minutes), "0#");
        }

        private bool IsModerator()
        {
            return this._objEventInfoHelper.IsModerator(true);
        }

        private string GetImageUrl(string imageURL, int portalID)
        {
            var imageSrc = imageURL;

            if (imageURL.StartsWith("FileID="))
            {
                var fileId = int.Parse(imageURL.Substring(7));
                var objFileInfo = FileManager.Instance.GetFile(fileId);
                if (!ReferenceEquals(objFileInfo, null))
                {
                    imageSrc = Convert.ToString(objFileInfo.Folder + objFileInfo.FileName);
                    if (imageSrc.IndexOf("://") + 1 == 0)
                    {
                        var pi = new PortalController();
                        imageSrc = Globals.AddHTTP(
                            string.Format("{0}/{1}/{2}", this._portalurl, pi.GetPortal(portalID).HomeDirectory,
                                          imageSrc));
                    }
                }
            }

            return imageSrc;
        }

        private string FoldText(string inText)
        {
            var outText = "";
            while (inText.Length > 75)
            {
                outText = outText + inText.Substring(0, 75) + Environment.NewLine + " ";
                inText = inText.Substring(75);
            }
            outText = outText + inText;
            return outText;
        }

        private class YearDayLight
        {
            public DateTime StartDate { get; set; }

            public DateTime EndDate { get; set; }

            public int Delta
            {
                // ReSharper disable UnusedMember.Local
                get;
                set;
            }
        }

        #endregion
    }

    #endregion
}