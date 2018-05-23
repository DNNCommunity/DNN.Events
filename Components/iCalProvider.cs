using DotNetNuke.Entities.Users;
using Microsoft.VisualBasic;
using System.Collections;
using DotNetNuke.Common.Utilities;
using System.Web;
using DotNetNuke.Services.Localization;
using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using System.Linq;


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
	    using global::Components;

	    #region vEvent Class
		
		public class VEvent : IHttpHandler
		{
			
#region  Private Properties
			private int _moduleID;
			private HttpContext _oContext;
			private bool _blOwnerEmail = false;
			private bool _blAnonOwnerEmail = false;
			private StringBuilder _sExdate = new StringBuilder();
			private EventInfoHelper _objEventInfoHelper;
			private bool _blSeries = false;
			private bool _blEventSignup = false;
			private bool _blEnrolleeEmail = false;
			private bool _blAnonEmail = false;
			private bool _blViewEmail = false;
			private bool _blEditEmail = false;
			private bool _blImages = false;
			private int _iUserid = -1;
			private string _domainName = "";
			private string _portalurl = "";
			private DateTime _timeZoneStart;
			private DateTime _timeZoneEnd;
			private string _iCalURLAppend = "";
			private string _iCalDefaultImage = "";
			private string _icsFilename = "";
			private EventModuleSettings _settings = null;
			private ArrayList _vTimeZoneIds = new ArrayList();
			
#endregion
			
#region Constructor
			public VEvent()
			{
			}
			
			public VEvent(bool series, HttpContext context)
			{
				_blSeries = series;
				_oContext = context;
			}
#endregion
			
#region  Properties
			public bool IsReusable
			{
				get
				{
					return true;
				}
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
				int iModuleID = 0;
				int iTabID = 0;
				string iCategoryName = "";
				int iCategoryID = -1;
				int iSocialGroupId = -1;
				string calname = "";
				_moduleID = 0;
				
				// Get the scope information
				_oContext = inContext;
				if ((iItemID == 0) && (_oContext.Request.QueryString["itemID"] == ""))
				{
					return;
				}
				if (!(_oContext.Request.QueryString["itemID"] == ""))
				{
					iItemID = System.Convert.ToInt64(_oContext.Request.QueryString["itemID"]);
				}
				
				if ((iModuleID == 0) && (_oContext.Request.QueryString["Mid"] == ""))
				{
					return;
				}
				if (!(_oContext.Request.QueryString["Mid"] == ""))
				{
					iModuleID = System.Convert.ToInt32(_oContext.Request.QueryString["Mid"]);
				}
				
				//Save this in the private property value
				_moduleID = iModuleID;
				
				if ((iTabID == 0) && (_oContext.Request.QueryString["TabId"] == ""))
				{
					return;
				}
				if (!(_oContext.Request.QueryString["TabId"] == ""))
				{
					iTabID = System.Convert.ToInt32(_oContext.Request.QueryString["TabId"]);
				}
				
				if (iItemID > 0 && (_oContext.Request.QueryString["Series"] == ""))
				{
					return;
				}
				else
				{
					_blSeries = false;
				}
				if (!(_oContext.Request.QueryString["Series"] == ""))
				{
					_blSeries = System.Convert.ToBoolean(_oContext.Request.QueryString["series"]);
				}
				
				if (!(_oContext.Request.QueryString["CategoryName"] == ""))
				{
					iCategoryName = _oContext.Request.QueryString["CategoryName"];
					PortalSecurity objSecurity = new PortalSecurity();
					iCategoryName = objSecurity.InputFilter(iCategoryName, PortalSecurity.FilterFlag.NoSQL);
				}
				if (!(HttpContext.Current.Request.QueryString["CategoryID"] == ""))
				{
					iCategoryID = System.Convert.ToInt32(_oContext.Request.QueryString["CategoryID"]);
				}
				
				if (!(_oContext.Request.QueryString["groupid"] == ""))
				{
					iSocialGroupId = System.Convert.ToInt32(_oContext.Request.QueryString["groupid"]);
				}
				
				if (!(HttpContext.Current.Request.QueryString["Calname"] == ""))
				{
					calname = _oContext.Request.QueryString["Calname"];
				}
				
				string iCal = "";
				iCal = CreateiCal(iTabID, iModuleID, iItemID, iSocialGroupId, iCategoryName, iCategoryID, calname);
				
				// Stream The vCalendar
				HttpResponse oStream = default(HttpResponse);
				oStream = _oContext.Response;
				oStream.ContentEncoding = Encoding.UTF8;
				oStream.ContentType = "text/Calendar";
				oStream.AppendHeader("Content-Disposition", "filename=" + HttpUtility.UrlEncode(_icsFilename) +".ics");
				oStream.Write(iCal);
				
			}
			
			public string CreateiCal(int iTabID, int iModuleID, long iItemID, int socialGroupId, string iCategoryName = "", int iCategoryID = -1, string iLocationName = "", int iLocationID = -1, string calname = "")
			{
				// Get relevant module settings
				
				_settings = EventModuleSettings.GetEventModuleSettings(iModuleID, null);
				
				// Set up for this module
				PortalSettings portalSettings = (PortalSettings) (HttpContext.Current.Items["PortalSettings"]);
				_objEventInfoHelper = new EventInfoHelper(iModuleID, iTabID, portalSettings.PortalId, _settings);
				_portalurl = _objEventInfoHelper.GetDomainURL();
				if (portalSettings.PortalAlias.HTTPAlias.IndexOf("/", StringComparison.Ordinal) > 0)
				{
					_portalurl = _portalurl + Common.Globals.ApplicationPath;
				}
				
				_blOwnerEmail = _settings.Exportowneremail;
				if (_blOwnerEmail)
				{
					_blAnonOwnerEmail = _settings.Exportanonowneremail;
				}
				_blEventSignup = _settings.Eventsignup;
				if (_settings.EnrollAnonFields.LastIndexOf("03", StringComparison.Ordinal) > -1)
				{
					_blAnonEmail = true;
				}
				if (_settings.EnrollViewFields.LastIndexOf("03", StringComparison.Ordinal) > -1)
				{
					_blViewEmail = true;
				}
				if (_settings.EnrollEditFields.LastIndexOf("03", StringComparison.Ordinal) > -1)
				{
					_blEditEmail = true;
				}
				
				int socialUserId = -1;
				if (_settings.SocialGroupModule == EventModuleSettings.SocialModule.UserProfile)
				{
					socialUserId = portalSettings.UserId;
				}
				
				_blImages = _settings.Eventimage;
				_iUserid = portalSettings.UserId;
				_domainName = portalSettings.PortalAlias.HTTPAlias;
				int iCalDaysBefore = _settings.IcalDaysBefore;
				int iCalDaysAfter = _settings.IcalDaysAfter;
				if (!(HttpContext.Current.Request.QueryString["DaysBefore"] == ""))
				{
					iCalDaysBefore = System.Convert.ToInt32(HttpContext.Current.Request.QueryString["DaysBefore"]);
				}
				if (!(HttpContext.Current.Request.QueryString["DaysAfter"] == ""))
				{
					iCalDaysAfter = System.Convert.ToInt32(HttpContext.Current.Request.QueryString["DaysAfter"]);
				}
				
				
				_iCalURLAppend = _settings.IcalURLAppend;
				_iCalDefaultImage = _settings.IcalDefaultImage.Substring(6);
				
				// Lookup DesktopModuleID
				DesktopModuleInfo objDesktopModule = default(DesktopModuleInfo);
				objDesktopModule = DesktopModuleController.GetDesktopModuleByModuleName("DNN_Events", portalSettings.PortalId);
				
				// Build the filename
				ModuleController objCtlModule = new ModuleController();
				ModuleInfo objModuleInfo = objCtlModule.GetModule(iModuleID, iTabID, false);
				_icsFilename = Common.Utilities.HtmlUtils.StripTags(objModuleInfo.ModuleTitle, false);
				
				// Get the event that is being viewed
				EventInfo oEvent = new EventInfo();
				EventController oCntrl = new EventController();
				if (iItemID > 0)
				{
					oEvent = oCntrl.EventsGet((int) iItemID, iModuleID);
				}
				
				StringBuilder vEvents = new StringBuilder();
				if (_blSeries && iItemID == 0)
				{
					// Not supported yet
				}
				else if (_blSeries && iItemID > 0)
				{
					// Process the series
					EventRecurMasterInfo oEventRecurMaster = default(EventRecurMasterInfo);
					EventRecurMasterController oCntrlRecurMaster = new EventRecurMasterController();
					oEventRecurMaster = oCntrlRecurMaster.EventsRecurMasterGet(oEvent.RecurMasterID, oEvent.ModuleID);
					vEvents.Append(CreateRecurvEvent(oEventRecurMaster));
					
					_icsFilename = oEventRecurMaster.EventName;
				}
				else if (!_blSeries && iItemID == 0)
				{
					// Process all events for the module
					ArrayList categoryIDs = new ArrayList();
					if (_settings.Enablecategories == EventModuleSettings.DisplayCategories.DoNotDisplay)
					{
						categoryIDs = _settings.ModuleCategoryIDs;
						iCategoryName = "";
					}
					if (iCategoryName != "")
					{
						EventCategoryController oCntrlEventCategory = new EventCategoryController();
						EventCategoryInfo oEventCategory = oCntrlEventCategory.EventCategoryGetByName(iCategoryName, portalSettings.PortalId);
						if (!ReferenceEquals(oEventCategory, null))
						{
							categoryIDs.Add(oEventCategory.Category);
						}
					}
					if (categoryIDs.Count == 0)
					{
						categoryIDs.Add("-1");
					}
					ArrayList locationIDs = new ArrayList();
					if (_settings.Enablelocations == EventModuleSettings.DisplayLocations.DoNotDisplay)
					{
						locationIDs = _settings.ModuleLocationIDs;
						iLocationName = "";
					}
					if (iLocationName != "")
					{
						EventLocationController oCntrlEventLocation = new EventLocationController();
						EventLocationInfo oEventLocation = oCntrlEventLocation.EventsLocationGetByName(iLocationName, portalSettings.PortalId);
						if (!ReferenceEquals(oEventLocation, null))
						{
							locationIDs.Add(oEventLocation.Location);
						}
					}
					if (locationIDs.Count == 0)
					{
						locationIDs.Add("-1");
					}
					
					EventTimeZoneUtilities objEventTimeZoneUtilities = new EventTimeZoneUtilities();
					DateTime moduleDateNow = objEventTimeZoneUtilities.ConvertFromUTCToModuleTimeZone(DateTime.UtcNow, _settings.TimeZoneId);
					DateTime startdate = DateAndTime.DateAdd(DateInterval.Day, 0 - iCalDaysBefore - 1, moduleDateNow);
					DateTime enddate = DateAndTime.DateAdd(DateInterval.Day, iCalDaysAfter + 1, moduleDateNow);
					ArrayList lstEvents = default(ArrayList);
					lstEvents = _objEventInfoHelper.GetEvents(startdate, enddate, _settings.MasterEvent, categoryIDs, locationIDs, socialGroupId, socialUserId);
					foreach (EventInfo tempLoopVar_oEvent in lstEvents)
					{
						oEvent = tempLoopVar_oEvent;
						DateTime utcEventTimeBegin = objEventTimeZoneUtilities.ConvertToUTCTimeZone(oEvent.EventTimeBegin, oEvent.EventTimeZoneId);
						DateTime utcEventTimeEnd = objEventTimeZoneUtilities.ConvertToUTCTimeZone(oEvent.EventTimeEnd, oEvent.EventTimeZoneId);
						DateTime utcStart = DateAndTime.DateAdd(DateInterval.Day, 0 - iCalDaysBefore, DateTime.UtcNow);
						DateTime utcEnd = DateAndTime.DateAdd(DateInterval.Day, iCalDaysAfter, DateTime.UtcNow);
						if (utcEventTimeEnd > utcStart && utcEventTimeBegin < utcEnd)
						{
							vEvents.Append(CreatevEvent(oEvent, oEvent.EventTimeBegin, false, false));
						}
					}
				}
				else
				{
					// Process the single event
					vEvents.Append(CreatevEvent(oEvent, oEvent.EventTimeBegin, false, false));
					_icsFilename = oEvent.EventName;
				}
				
				// Create the initial VCALENDAR
				StringBuilder vCalendar = new StringBuilder();
				vCalendar.Append("BEGIN:VCALENDAR" + Environment.NewLine);
				vCalendar.Append("VERSION:2.0" + Environment.NewLine);
				vCalendar.Append(FoldText("PRODID:-//DNN//" + objDesktopModule.FriendlyName + " " + objDesktopModule.Version + "//EN") + Environment.NewLine);
				vCalendar.Append("CALSCALE:GREGORIAN" + Environment.NewLine);
				vCalendar.Append("METHOD:PUBLISH" + Environment.NewLine);
				if (calname != "")
				{
					vCalendar.Append("X-WR-CALNAME:" + calname + Environment.NewLine);
				}
				else if (_settings.IcalIncludeCalname)
				{
					vCalendar.Append("X-WR-CALNAME:" + _icsFilename + Environment.NewLine);
				}
				
				// Create the VTIMEZONE
				if (_timeZoneStart == DateTime.MinValue)
				{
					_timeZoneStart = DateTime.Now;
					_timeZoneEnd = DateAndTime.DateAdd(DateInterval.Minute, 30, _timeZoneStart);
				}
				
				vCalendar.Append(CreatevTimezones(_timeZoneStart, _timeZoneEnd));
				
				// Output the events
				vCalendar.Append(vEvents.ToString());
				
				// Close off the VCALENDAR
				vCalendar.Append("END:VCALENDAR" + Environment.NewLine);
				
				return vCalendar.ToString();
				
			}
			
			private string CreatevTimezones(DateTime dtStartDate, DateTime dtEndDate)
			{
				StringBuilder vTimezone = new StringBuilder();
				foreach (string vTimeZoneId in _vTimeZoneIds)
				{
					TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(vTimeZoneId);
					TimeZoneInfo.AdjustmentRule[] adjustments = tzi.GetAdjustmentRules();
					
					vTimezone.Append("BEGIN:VTIMEZONE" + Environment.NewLine);
					vTimezone.Append("TZID:" + tzi.Id + Environment.NewLine);
					
					if (adjustments.Any())
					{
						// Identify last DST change before start of recurrences
						int intYear = 0;
						DateTime lastDSTStartDate = new DateTime();
						DateTime lastDSTEndDate = new DateTime();
						for (intYear = dtStartDate.Year - 1; intYear <= dtEndDate.Year; intYear++)
						{
							YearDayLight yearDayLight = GetAdjustment(adjustments, intYear);
							DateTime daylightStart = yearDayLight.StartDate;
							DateTime daylightEnd = yearDayLight.EndDate;
							if ((lastDSTStartDate == DateTime.MinValue || daylightStart > lastDSTStartDate) && daylightStart < dtStartDate)
							{
								lastDSTStartDate = daylightStart;
							}
							if ((lastDSTEndDate == DateTime.MinValue || daylightEnd > lastDSTEndDate) && daylightEnd < dtStartDate)
							{
								lastDSTEndDate = daylightEnd;
							}
						}
						for (intYear = dtStartDate.Year - 1; intYear <= dtEndDate.Year; intYear++)
						{
							YearDayLight yearDayLight = GetAdjustment(adjustments, intYear);
							DateTime daylightStart = yearDayLight.StartDate;
							DateTime daylightEnd = yearDayLight.EndDate;
							
							double iByDay = Conversion.Int((double) daylightEnd.Day / 7) + 1;
							string sDayOfWeek = "";
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
							
							string sByDay = iByDay.ToString() + sDayOfWeek;
							string sByMonth = daylightEnd.Month.ToString();
							
							// Allow for timezone with no DST, and don't include Timezones after end
							if (daylightStart.Year > 1 && daylightStart < dtEndDate && !(daylightStart < lastDSTStartDate))
							{
								string dtFrom = FormatTZTime(tzi.GetUtcOffset(DateAndTime.DateAdd(DateInterval.Day, -2, daylightStart)));
								string dtTo = FormatTZTime(tzi.GetUtcOffset(DateAndTime.DateAdd(DateInterval.Day, System.Convert.ToDouble(+ 2), daylightStart)));
								vTimezone.Append("BEGIN:DAYLIGHT" + Environment.NewLine);
								vTimezone.Append("TZOFFSETFROM:" + dtFrom + Environment.NewLine);
								vTimezone.Append("TZOFFSETTO:" + dtTo + Environment.NewLine);
								vTimezone.Append("DTSTART:" + CreateTZIDDate(daylightStart, daylightStart, false, false, tzi.Id) + Environment.NewLine);
								vTimezone.Append("END:DAYLIGHT" + Environment.NewLine);
							}
							
							// Allow for timezone with no DST, and don't include Timezones after end
							if (daylightEnd.Year > 1 && daylightEnd < dtEndDate && !(daylightEnd < lastDSTEndDate))
							{
								string dtFrom = FormatTZTime(tzi.GetUtcOffset(DateAndTime.DateAdd(DateInterval.Day, -2, daylightEnd)));
								string dtTo = FormatTZTime(tzi.GetUtcOffset(DateAndTime.DateAdd(DateInterval.Day, System.Convert.ToDouble(+ 2), daylightEnd)));
								vTimezone.Append("BEGIN:STANDARD" + Environment.NewLine);
								vTimezone.Append(FoldText("RRULE:FREQ=YEARLY;INTERVAL=1;BYMONTH=" + sByMonth + ";BYDAY=" + sByDay + ";COUNT=1") + Environment.NewLine);
								vTimezone.Append("TZOFFSETFROM:" + dtFrom + Environment.NewLine);
								vTimezone.Append("TZOFFSETTO:" + dtTo + Environment.NewLine);
								vTimezone.Append("DTSTART:" + CreateTZIDDate(daylightEnd, daylightEnd, false, false, tzi.Id) + Environment.NewLine);
								vTimezone.Append("END:STANDARD" + Environment.NewLine);
							}
							
							if (!(daylightStart.Year > 1))
							{
								vTimezone.Append(CreatevTimezone1601(tzi));
								break;
							}
						}
					}
					else
					{
						vTimezone.Append(CreatevTimezone1601(tzi));
					}
					vTimezone.Append("END:VTIMEZONE" + Environment.NewLine);
				}
				return vTimezone.ToString();
			}
			
			private YearDayLight GetAdjustment(IEnumerable<System.TimeZoneInfo.AdjustmentRule> adjustments, int year)
				{
				// Iterate adjustment rules for time zone
				foreach (TimeZoneInfo.AdjustmentRule adjustment in adjustments)
				{
					// Determine if this adjustment rule covers year desired
					if (adjustment.DateStart.Year <= year & adjustment.DateEnd.Year >= year)
					{
						TimeZoneInfo.TransitionTime startTransition = default(TimeZoneInfo.TransitionTime);
						TimeZoneInfo.TransitionTime endTransition = default(TimeZoneInfo.TransitionTime);
						YearDayLight yearDayLight = new YearDayLight();
						startTransition = adjustment.DaylightTransitionStart;
						yearDayLight.StartDate = ProcessAdjustmentDate(startTransition, year);
						endTransition = adjustment.DaylightTransitionEnd;
						yearDayLight.EndDate = ProcessAdjustmentDate(endTransition, year);
						yearDayLight.Delta = System.Convert.ToInt32(adjustment.DaylightDelta.TotalMinutes);
						return yearDayLight;
					}
				}
				return null;
			}
			
			// VBConversions Note: Former VB static variables moved to class level because they aren't supported in C#.
			private System.Globalization.Calendar ProcessAdjustmentDate_cal = CultureInfo.CurrentCulture.Calendar;
			
			private DateTime ProcessAdjustmentDate(TimeZoneInfo.TransitionTime processTransition, int year)
			{
				int transitionDay = 0;
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
					int startOfWeek = processTransition.Week * 7 - 6;
					// What day of the week does the month start on
					int firstDayOfWeek = (int) (ProcessAdjustmentDate_cal.GetDayOfWeek(new DateTime(year, processTransition.Month, 1)));
					// Determine how much start date has to be adjusted
					int changeDayOfWeek = (int) processTransition.DayOfWeek;
					
					if (firstDayOfWeek <= changeDayOfWeek)
					{
						transitionDay = startOfWeek + (changeDayOfWeek - firstDayOfWeek);
					}
					else
					{
						transitionDay = startOfWeek + (7 - firstDayOfWeek + changeDayOfWeek);
					}
					// Adjust for months with no fifth week
					if (transitionDay > ProcessAdjustmentDate_cal.GetDaysInMonth(year, processTransition.Month))
					{
						transitionDay -= 7;
					}
				}
				return DateTime.ParseExact(string.Format("{0:0000}/{1:00}/{2:00} {3:HH:mm}", year, processTransition.Month, transitionDay, processTransition.TimeOfDay), "yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture);
			}
			
			private string CreatevTimezone1601(TimeZoneInfo tzi)
			{
				StringBuilder vTimezone1601 = new StringBuilder();
				CultureInfo invCuluture = CultureInfo.InvariantCulture;
				DateTime dt1601Date = DateTime.ParseExact("01/01/1601 00:00:00", "MM/dd/yyyy HH:mm:ss", invCuluture);
				string dtTo = FormatTZTime(tzi.GetUtcOffset(dt1601Date));
				string dtFrom = FormatTZTime(tzi.GetUtcOffset(dt1601Date));
				vTimezone1601.Append("BEGIN:STANDARD" + Environment.NewLine);
				vTimezone1601.Append("TZOFFSETFROM:" + dtFrom + Environment.NewLine);
				vTimezone1601.Append("TZOFFSETTO:" + dtTo + Environment.NewLine);
				vTimezone1601.Append("DTSTART:" + CreateTZIDDate(dt1601Date, dt1601Date, false, false, tzi.Id) + Environment.NewLine);
				vTimezone1601.Append("END:STANDARD" + Environment.NewLine);
				return vTimezone1601.ToString();
			}
			
			private string CreateRecurvEvent(EventRecurMasterInfo oEventRecurMaster)
			{
				StringBuilder vEvent = new StringBuilder();
				
				ArrayList lstEvents = default(ArrayList);
				EventInfo oEvent = default(EventInfo);
				EventController oCntrl = new EventController();
				lstEvents = oCntrl.EventsGetRecurrences(oEventRecurMaster.RecurMasterID, oEventRecurMaster.ModuleID);
				// Create the single VEVENT
				foreach (EventInfo tempLoopVar_oEvent in lstEvents)
				{
					oEvent = tempLoopVar_oEvent;
					if (oEvent.EventTimeBegin.TimeOfDay == oEventRecurMaster.Dtstart.TimeOfDay && Convert.ToString(oEvent.Duration) + "M" == oEventRecurMaster.Duration &&
							oEvent.EventName == oEventRecurMaster.EventName &&
							oEvent.EventDesc == oEventRecurMaster.EventDesc &&
							oEvent.OwnerID == oEventRecurMaster.OwnerID &
							oEvent.Location == oEventRecurMaster.Location &&
							(int)oEvent.Importance == (int)oEventRecurMaster.Importance &&
							oEvent.SendReminder == oEventRecurMaster.SendReminder &&
							oEvent.ReminderTime == oEventRecurMaster.ReminderTime &&
							oEvent.AllDayEvent == oEventRecurMaster.AllDayEvent &&
							oEvent.ReminderTimeMeasurement == oEventRecurMaster.ReminderTimeMeasurement &&
							oEvent.Cancelled == false &&
							(oEvent.Enrolled == 0 || !_blEventSignup || !oEvent.EnrollListView || !oEvent.Signups))
							{
							if (!_blImages ||
									(_blImages && oEvent.ImageDisplay == oEventRecurMaster.ImageDisplay && oEvent.ImageURL == oEventRecurMaster.ImageURL))
									{
									continue;
						}
					}
					vEvent.Append(CreatevEvent(oEvent, oEventRecurMaster.Dtstart, false, oEventRecurMaster.AllDayEvent));
				}
				
				if (lstEvents.Count == 0)
				{
					return "";
				}
				EventInfo oFirstEvent = (EventInfo) (lstEvents[0]);
				EventLocationInfo objEventLocation = new EventLocationInfo();
				EventLocationController objCtlEventLocation = new EventLocationController();
				
				if (oEventRecurMaster.Location > 0)
				{
					objEventLocation = objCtlEventLocation.EventsLocationGet(oEventRecurMaster.Location, oEventRecurMaster.PortalID);
				}
				
				EventTimeZoneUtilities objEventTimeZoneUtilities = new EventTimeZoneUtilities();
				DateTime recurUntil = objEventTimeZoneUtilities.ConvertToUTCTimeZone(oEventRecurMaster.Until, oEventRecurMaster.EventTimeZoneId);
				
				// Calculate timezone start/end dates
				int intDuration = 0;
				intDuration = int.Parse(oEventRecurMaster.Duration.Substring(0, oEventRecurMaster.Duration.Length - 1));
				if (_timeZoneStart == DateTime.MinValue || _timeZoneStart > oEventRecurMaster.Dtstart)
				{
					_timeZoneStart = oEventRecurMaster.Dtstart;
				}
				if (_timeZoneEnd == DateTime.MinValue || _timeZoneEnd < DateAndTime.DateAdd(DateInterval.Minute, intDuration, oEventRecurMaster.Until))
				{
					_timeZoneEnd = DateAndTime.DateAdd(DateInterval.Minute, intDuration, oEventRecurMaster.Until);
				}
				
				// Build Item
				UserController objUsers = new UserController();
				UserInfo objUser = objUsers.GetUser(oEventRecurMaster.PortalID, oEventRecurMaster.OwnerID);
				string creatoremail = "";
				string creatoranonemail = "";
				string creatorname = "";
				if (!ReferenceEquals(objUser, null))
				{
					creatoremail = ":MAILTO:" + objUser.Email;
					creatoranonemail = ":MAILTO:" + objUser.FirstName +"." + objUser.LastName + "@no_email.com";
					creatorname = "CN=\"" + objUser.DisplayName + "\"";
				}
				else
				{
					Entities.Portals.PortalController objPortals = new Entities.Portals.PortalController();
					PortalInfo objPortal = default(PortalInfo);
					objPortal = objPortals.GetPortal(oEventRecurMaster.PortalID);
					creatoremail = ":MAILTO:" + objPortal.Email;
					creatoranonemail = ":MAILTO:" + "anonymous@no_email.com";
					creatorname = "CN=\"Anonymous\"";
				}
				
				string sEmail = "";
				if ((_oContext.Request.IsAuthenticated && _blOwnerEmail) || _blAnonOwnerEmail)
				{
					sEmail = FoldText("ORGANIZER;" + creatorname + creatoremail) + Environment.NewLine;
				}
				else
				{
					sEmail = FoldText("ORGANIZER;" + creatorname + creatoranonemail) + Environment.NewLine;
				}
				
				ArrayList aTimes = default(ArrayList);
				string sStartTime = "";
				string sEndTime = "";
				string sDtStamp = "";
				string sSequence = "";
				
				aTimes = TimeFormat(oFirstEvent.OriginalDateBegin.Date + oEventRecurMaster.Dtstart.TimeOfDay, intDuration, oFirstEvent.EventTimeZoneId, recurUntil);
				if (!oEventRecurMaster.AllDayEvent)
				{
					sStartTime = "DTSTART;" + Convert.ToString(aTimes[0]) + Environment.NewLine;
					sEndTime = "DTEND;" + Convert.ToString(aTimes[1]) + Environment.NewLine;
				}
				else
				{
					sStartTime = "DTSTART;VALUE=DATE:" + AllDayEventDate(oFirstEvent.OriginalDateBegin.Date) + Environment.NewLine;
					//  +1 deals with use of 1439 minutes instead of 1440
					sEndTime = "DTEND;VALUE=DATE:" + AllDayEventDate(oFirstEvent.OriginalDateBegin.Date.AddMinutes(intDuration + 1)) + Environment.NewLine;
				}
				sDtStamp = "DTSTAMP:" + CreateTZIDDate(DateTime.UtcNow, DateTime.UtcNow, true, false, oEventRecurMaster.EventTimeZoneId) + Environment.NewLine;
				sSequence = "SEQUENCE:" + Convert.ToString(oEventRecurMaster.Sequence) + Environment.NewLine;
				
				string sLocation = "";
				if (oEventRecurMaster.Location > 0)
				{
					if (objEventLocation.MapURL != "" && _settings.IcalURLInLocation)
					{
						sLocation = objEventLocation.LocationName + " - " + objEventLocation.MapURL;
					}
					else
					{
						sLocation = objEventLocation.LocationName;
					}
					sLocation = FoldText("LOCATION:" + CreateText(sLocation)) + Environment.NewLine;
				}
				string sDescription = CreateDescription(oEventRecurMaster.EventDesc);
				string altDescription = CreateAltDescription(oEventRecurMaster.EventDesc);
				
				// ToDo: HIER!
				// Make up the LocalResourceFile value
				string templateSourceDirectory = Common.Globals.ApplicationPath;
				string localResourceFile = templateSourceDirectory + "/DesktopModules/Events/" + Localization.LocalResourceDirectory + "/SharedResources.ascx.resx";
				TokenReplaceControllerClass tcc = new TokenReplaceControllerClass(_moduleID, localResourceFile);
				
				string tmpSummary = _settings.Templates.EventiCalSubject;
				//tmpSummary = tcc.TokenReplaceEvent(oEventRecurMaster, tmpSummary)
				//Dim sSummary As String = FoldText("SUMMARY:" & CreateText(oEventRecurMaster.EventName)) & Environment.NewLine
				
				string sSummary = FoldText("SUMMARY-RM:" + CreateText(oEventRecurMaster.EventName)) + Environment.NewLine;
				string sPriority = "PRIORITY:" + Priority((System.Int32) oEventRecurMaster.Importance) + Environment.NewLine;
				
				string sURL = FoldText("URL:" + _objEventInfoHelper.DetailPageURL(oFirstEvent, false) + _iCalURLAppend) + Environment.NewLine;
				
				vEvent.Append("BEGIN:VEVENT" + Environment.NewLine);
				string strUID = string.Format("{0:00000}", oEventRecurMaster.ModuleID) + string.Format("{0:0000000}", oEventRecurMaster.RecurMasterID);
				vEvent.Append("UID:DNNEvent" + strUID + "@" + _domainName + Environment.NewLine);
				vEvent.Append(sSequence);
				if (oEventRecurMaster.RRULE != "")
				{
					vEvent.Append("RRULE:" + oEventRecurMaster.RRULE + ";" + "UNTIL=" + Convert.ToString(aTimes[2]) + Environment.NewLine);
				}
				if (_sExdate.ToString() != "")
				{
					vEvent.Append(_sExdate.ToString());
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
				
				int iMinutes = 0;
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
				vEvent.Append("CREATED:" + CreateTZIDDate(oEventRecurMaster.CreatedDate, oEventRecurMaster.CreatedDate, true, false, oEventRecurMaster.EventTimeZoneId) + Environment.NewLine);
				vEvent.Append("LAST-MODIFIED:" + CreateTZIDDate(oEventRecurMaster.UpdatedDate, oEventRecurMaster.UpdatedDate, true, false, oEventRecurMaster.EventTimeZoneId) + Environment.NewLine);
				if (oEventRecurMaster.SendReminder)
				{
					vEvent.Append("BEGIN:VALARM" + Environment.NewLine);
					vEvent.Append("TRIGGER:-PT" + iMinutes.ToString() + "M" + Environment.NewLine);
					vEvent.Append("ACTION:DISPLAY" + Environment.NewLine);
					vEvent.Append("DESCRIPTION:Reminder" + Environment.NewLine);
					vEvent.Append("END:VALARM" + Environment.NewLine);
				}
				
				if (_blImages && oEventRecurMaster.ImageDisplay)
				{
					vEvent.Append(FoldText("ATTACH:" + GetImageUrl(oEventRecurMaster.ImageURL, oEventRecurMaster.PortalID)) + Environment.NewLine);
				}
				else if (_blImages && !string.IsNullOrEmpty(_iCalDefaultImage))
				{
					vEvent.Append(FoldText("ATTACH:" + GetImageUrl(_iCalDefaultImage, oEventRecurMaster.PortalID)) + Environment.NewLine);
				}
				
				vEvent.Append("TRANSP:OPAQUE" + Environment.NewLine);
				
				vEvent.Append("END:VEVENT" + Environment.NewLine);
				
				return vEvent.ToString();
				
			}
			
			private string CreatevEvent(EventInfo oEvent, DateTime dtstart, bool blURLOnly, bool blAllDay)
			{
				
				if (!_vTimeZoneIds.Contains(oEvent.EventTimeZoneId))
				{
					_vTimeZoneIds.Add(oEvent.EventTimeZoneId);
				}
				
				oEvent.OriginalDateBegin = oEvent.OriginalDateBegin.Date + dtstart.TimeOfDay;
				
				// Calculate timezone start/end dates
				if (_timeZoneStart == DateTime.MinValue || _timeZoneStart > oEvent.EventTimeBegin)
				{
					_timeZoneStart = oEvent.EventTimeBegin;
				}
				if (_timeZoneEnd == DateTime.MinValue || _timeZoneEnd < DateAndTime.DateAdd(DateInterval.Minute, oEvent.Duration, oEvent.EventTimeBegin))
				{
					_timeZoneEnd = DateAndTime.DateAdd(DateInterval.Minute, oEvent.Duration, oEvent.EventTimeBegin);
				}
				
				// Build Item
				StringBuilder vEvent = new StringBuilder();
				
				UserController objUsers = new UserController();
				UserInfo objUser = objUsers.GetUser(oEvent.PortalID, oEvent.OwnerID);
				string creatoremail = "";
				string creatoranonemail = "";
				string creatorname = "";
				if (!ReferenceEquals(objUser, null))
				{
					creatoremail = ":MAILTO:" + objUser.Email;
					creatoranonemail = ":MAILTO:" + objUser.FirstName +"." + objUser.LastName + "@no_email.com";
					creatorname = "CN=\"" + objUser.DisplayName + "\"";
				}
				else
				{
					Entities.Portals.PortalController objPortals = new Entities.Portals.PortalController();
					Entities.Portals.PortalInfo objPortal = default(Entities.Portals.PortalInfo);
					objPortal = objPortals.GetPortal(oEvent.PortalID);
					creatoremail = ":MAILTO:" + objPortal.Email;
					creatoranonemail = ":MAILTO:" + "anonymous@no_email.com";
					creatorname = "CN=\"Anonymous\"";
				}
				
				string sEmail = "";
				if ((_oContext.Request.IsAuthenticated && _blOwnerEmail) || _blAnonOwnerEmail)
				{
					sEmail = FoldText("ORGANIZER;" + creatorname + creatoremail) + Environment.NewLine;
				}
				else
				{
					sEmail = FoldText("ORGANIZER;" + creatorname + creatoranonemail) + Environment.NewLine;
				}
				
				ArrayList aTimes = default(ArrayList);
				string sStartTime = "";
				string sEndTime = "";
				string sDtStamp = "";
				string sSequence = "";
				
				aTimes = TimeFormat(dBeginDateTime: oEvent.EventTimeBegin, iDuration: oEvent.Duration, timeZoneId: oEvent.EventTimeZoneId, dOriginal: oEvent.OriginalDateBegin);
				if (oEvent.Cancelled)
				{
					_sExdate.Append("EXDATE;" + Convert.ToString(aTimes[3]) + Environment.NewLine);
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
					sEndTime = "DTEND;VALUE=DATE:" + AllDayEventDate(oEvent.EventTimeBegin.Date.AddMinutes(oEvent.Duration + 1)) + Environment.NewLine;
				}
				if (!_blSeries)
				{
					sDtStamp = "DTSTAMP:" + CreateTZIDDate(DateTime.UtcNow, DateTime.UtcNow, true, false, oEvent.EventTimeZoneId) + Environment.NewLine;
				}
				sSequence = "SEQUENCE:" + Convert.ToString(oEvent.Sequence) + Environment.NewLine;
				
				string sLocation = "";
				if (oEvent.Location > 0)
				{
					if (oEvent.MapURL != "" && _settings.IcalURLInLocation)
					{
						sLocation = oEvent.LocationName + " - " + oEvent.MapURL;
					}
					else
					{
						sLocation = oEvent.LocationName;
					}
					sLocation = FoldText("LOCATION:" + CreateText(sLocation)) + Environment.NewLine;
				}
				
				string sDescription = CreateDescription(oEvent.EventDesc);
				string altDescription = CreateAltDescription(oEvent.EventDesc);
				
				
				//Create the templated version of the summary
				string templateSourceDirectory = Common.Globals.ApplicationPath;
				string localResourceFile = templateSourceDirectory + "/DesktopModules/Events/" + Localization.LocalResourceDirectory + "/SharedResources.ascx.resx";
				TokenReplaceControllerClass tcc = new TokenReplaceControllerClass(_moduleID, localResourceFile);
				string tmpSummary = _settings.Templates.EventiCalSubject;
				tmpSummary = tcc.TokenReplaceEvent(oEvent, tmpSummary);
				
				string sSummary = FoldText("SUMMARY:" + CreateText(tmpSummary)) + Environment.NewLine;
				
				string sPriority = "PRIORITY:" + Priority((System.Int32) oEvent.Importance) + Environment.NewLine;
				
				string sURL = FoldText("URL:" + _objEventInfoHelper.DetailPageURL(oEvent, false) + _iCalURLAppend) + Environment.NewLine;
				
				vEvent.Append("BEGIN:VEVENT" + Environment.NewLine);
				string strUID = string.Format("{0:00000}", oEvent.ModuleID) + string.Format("{0:0000000}", oEvent.RecurMasterID);
				if (!_blSeries)
				{
					strUID += string.Format("{0:0000000}", oEvent.EventID);
				}
				vEvent.Append("UID:DNNEvent" + strUID + "@" + _domainName + Environment.NewLine);
				vEvent.Append(sSequence);
				if (_blSeries)
				{
					if (!blAllDay)
					{
						vEvent.Append("RECURRENCE-ID;" + Convert.ToString(aTimes[3]) + Environment.NewLine);
					}
					else
					{
						vEvent.Append("RECURRENCE-ID;VALUE=DATE:" + AllDayEventDate(oEvent.OriginalDateBegin) + Environment.NewLine);
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
					
					if (_blEventSignup && oEvent.EnrollListView && oEvent.Signups && oEvent.Enrolled > 0)
					{
						_blEnrolleeEmail = false;
						if ((IsModerator() || (oEvent.CreatedByID == _iUserid | oEvent.RmOwnerID == _iUserid | oEvent.OwnerID == _iUserid)) && _blEditEmail)
						{
							_blEnrolleeEmail = true;
						}
						if (_oContext.Request.IsAuthenticated && (_blViewEmail || _blAnonEmail))
						{
							_blEnrolleeEmail = true;
						}
						if (!_oContext.Request.IsAuthenticated && _blAnonEmail)
						{
							_blEnrolleeEmail = true;
						}
						vEvent.Append(CreateAttendee(oEvent));
					}
					
					int iMinutes = 0;
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
					vEvent.Append("CREATED:" + CreateTZIDDate(oEvent.CreatedDate, oEvent.CreatedDate, true, false, oEvent.EventTimeZoneId) + Environment.NewLine);
					vEvent.Append("LAST-MODIFIED:" + CreateTZIDDate(oEvent.LastUpdatedAt, oEvent.LastUpdatedAt, true, false, oEvent.EventTimeZoneId) + Environment.NewLine);
					if (oEvent.SendReminder)
					{
						vEvent.Append("BEGIN:VALARM" + Environment.NewLine);
						vEvent.Append("TRIGGER:-PT" + iMinutes.ToString() + "M" + Environment.NewLine);
						vEvent.Append("ACTION:DISPLAY" + Environment.NewLine);
						vEvent.Append("DESCRIPTION:Reminder" + Environment.NewLine);
						vEvent.Append("END:VALARM" + Environment.NewLine);
					}
					
					if (_blImages && oEvent.ImageDisplay)
					{
						vEvent.Append(FoldText("ATTACH:" + GetImageUrl(oEvent.ImageURL, oEvent.PortalID)) + Environment.NewLine);
					}
					else if (_blImages && !string.IsNullOrEmpty(_iCalDefaultImage))
					{
						vEvent.Append(FoldText("ATTACH:" + GetImageUrl(_iCalDefaultImage, oEvent.PortalID)) + Environment.NewLine);
					}
				}
				
				vEvent.Append("TRANSP:OPAQUE" + Environment.NewLine);
				
				vEvent.Append("END:VEVENT" + Environment.NewLine);
				
				return vEvent.ToString();
			}
			
			private string CreateAttendee(EventInfo oEvent)
			{
				StringBuilder attendees = new StringBuilder();
				ArrayList oSignups = default(ArrayList);
				EventSignupsInfo oSignup = default(EventSignupsInfo);
				EventSignupsController oCtlEventSignups = new EventSignupsController();
				UserController objUsers = new UserController();
				oSignups = oCtlEventSignups.EventsSignupsGetEvent(oEvent.EventID, oEvent.ModuleID);
				foreach (EventSignupsInfo tempLoopVar_oSignup in oSignups)
				{
					oSignup = tempLoopVar_oSignup;
					UserInfo objUser = objUsers.GetUser(oEvent.PortalID, oSignup.UserID);
					string attendeeemail = "";
					string attendeeanonemail = "";
					string attendeename = "";
					string sPartStat = "ACCEPTED";
					if (!oSignup.Approved)
					{
						sPartStat = "NEEDS-ACTION";
					}
					if (!ReferenceEquals(objUser, null))
					{
						attendeeemail = ":MAILTO:" + objUser.Email;
						attendeeanonemail = ":MAILTO:" + objUser.FirstName +"." + objUser.LastName + "@no_email.com";
						attendeename = "CN=\"" + objUser.DisplayName + "\"";
					}
					else
					{
						attendeeemail = ":MAILTO:" + "anonymous@no_email.com";
						attendeeanonemail = ":MAILTO:" + "anonymous@no_email.com";
						attendeename = "CN=\"Anonymous-" + oSignup.UserID.ToString() + "\"";
					}
					string sAttendee = "";
					if (_blEnrolleeEmail)
					{
						sAttendee = "ATTENDEE;ROLE=REQ-PARTICIPANT;PARTSTAT=" + sPartStat + ";" + attendeename + attendeeemail + Environment.NewLine;
					}
					else
					{
						sAttendee = "ATTENDEE;ROLE=REQ-PARTICIPANT;PARTSTAT=" + sPartStat + ";" + attendeename + attendeeanonemail + Environment.NewLine;
					}
					attendees.Append(sAttendee);
				}
				return attendees.ToString();
			}
#endregion
			
#region  Private Support Functions
			private static ArrayList TimeFormat(DateTime dBeginDateTime, int iDuration, string timeZoneId, DateTime dUntil = default(DateTime), DateTime dOriginal = default(DateTime))
			{
				// VBConversions Note: dUntil assigned to default value below, since optional parameter values must be static and C# doesn't support date literals.
				if (dUntil == default(DateTime))
					dUntil = DateTime.MinValue;
				
				// VBConversions Note: dOriginal assigned to default value below, since optional parameter values must be static and C# doesn't support date literals.
				if (dOriginal == default(DateTime))
					dOriginal = DateTime.MinValue;
				
				ArrayList aTimes = new ArrayList();
				DateTime eDate = default(DateTime);
				DateTime tempDateTime = dBeginDateTime.Date + dBeginDateTime.TimeOfDay;
				
				//Begin Time Format
				aTimes.Add(CreateTZIDDate(dBeginDateTime, dBeginDateTime, false, true, timeZoneId));
				
				//End Time Format
				eDate = tempDateTime.AddMinutes(iDuration);
				aTimes.Add(CreateTZIDDate(eDate, eDate, false, true, timeZoneId));
				
				//Until Time Format
				CultureInfo invCuluture = CultureInfo.InvariantCulture;
				aTimes.Add(CreateTZIDDate(dUntil, DateTime.ParseExact("01/01/2002 23:59:59", "MM/dd/yyyy HH:mm:ss", invCuluture), true, false, timeZoneId));
				
				//Original Time Format
				aTimes.Add(CreateTZIDDate(dOriginal, dOriginal, false, true, timeZoneId));
				
				return aTimes;
			}
			
			private static string CreateTZIDDate(DateTime dDate, DateTime dTime, bool blUTC, bool blTZAdd, string timeZoneId)
			{
				StringBuilder sTime = new StringBuilder();
				if (blTZAdd)
				{
					sTime.Append("TZID=\"" + timeZoneId + "\":");
				}
				sTime.Append(dDate.Year.ToString());
				sTime.Append(Strings.Format(dDate.Month, "0#").ToString());
				sTime.Append(Strings.Format(dDate.Day, "0#").ToString());
				sTime.Append("T");
				sTime.Append(Strings.Format(dTime.Hour, "0#").ToString());
				sTime.Append(Strings.Format(dTime.Minute, "0#").ToString());
				sTime.Append(Strings.Format(dTime.Second, "0#").ToString());
				if (blUTC)
				{
					sTime.Append("Z");
				}
				return sTime.ToString();
			}
			
			private static string AllDayEventDate(DateTime dDate)
			{
				StringBuilder sDate = new StringBuilder();
				sDate.Append(dDate.Year.ToString());
				sDate.Append(Strings.Format(dDate.Month, "0#").ToString());
				sDate.Append(Strings.Format(dDate.Day, "0#").ToString());
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
				string altDescription = "X-ALT-DESC;FMTTYPE=text/html:<!DOCTYPE HTML PUBLIC \" -//W3C//DTD HTML 3.2//EN\">\\n";
				altDescription += "<HTML>\\n";
				altDescription += "<HEAD>\\n";
				altDescription += "<META NAME=\"Generator\" CONTENT=\"DNN Events Module\">\\n";
				altDescription += "<TITLE></TITLE>\\n";
				altDescription += "</HEAD>\\n";
				altDescription += "<BODY>\\n";
				altDescription += HtmlUtils.StripWhiteSpace(HttpUtility.HtmlDecode(eventDesc), true).Replace(Environment.NewLine, "") + "\\n";
				altDescription += "</BODY>\\n";
				altDescription += "</HTML>";
				altDescription = FoldText(altDescription) + Environment.NewLine;
				
				return altDescription;
			}
			
			private string CreateDescription(string eventDesc)
			{
				string sDescription = "DESCRIPTION:";
				const int descriptionLength = 1950;
				string tmpDesc = CreateText(eventDesc);
				tmpDesc = HtmlUtils.Shorten(tmpDesc, descriptionLength, "...");
				sDescription = FoldText(sDescription + tmpDesc + "\\n") + Environment.NewLine;
				return sDescription;
			}
			
			private string CreateText(string eventText)
			{
				string tmpText = HtmlUtils.StripTags(HttpUtility.HtmlDecode(eventText), false);
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
				string dtSign = "+";
				if (dtTimeSpan.Hours < 0)
				{
					dtSign = "-";
				}
				return dtSign + Strings.Format(Math.Abs(dtTimeSpan.Hours), "0#") + Strings.Format(Math.Abs(dtTimeSpan.Minutes), "0#");
			}
			
			private bool IsModerator()
			{
				return _objEventInfoHelper.IsModerator(true);
			}
			
			private string GetImageUrl(string imageURL, int portalID)
			{
				string imageSrc = imageURL;
				
				if (imageURL.StartsWith("FileID="))
				{
					int fileId = int.Parse(imageURL.Substring(7));
					Services.FileSystem.IFileInfo objFileInfo = Services.FileSystem.FileManager.Instance.GetFile(fileId);
					if (!ReferenceEquals(objFileInfo, null))
					{
						imageSrc = System.Convert.ToString(objFileInfo.Folder + objFileInfo.FileName);
						if (imageSrc.IndexOf("://") + 1 == 0)
						{
							Entities.Portals.PortalController pi = new Entities.Portals.PortalController();
							imageSrc = DotNetNuke.Common.Globals.AddHTTP(string.Format("{0}/{1}/{2}", _portalurl, pi.GetPortal(portalID).HomeDirectory, imageSrc));
						}
					}
				}
				
				return imageSrc;
				
			}
			
			private string FoldText(string inText)
			{
				string outText = "";
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
				private DateTime _startDate;
				public DateTime StartDate
				{
					get
					{
						return _startDate;
					}
					set
					{
						_startDate = value;
					}
				}
				private DateTime _endDate;
				public DateTime EndDate
				{
					get
					{
						return _endDate;
					}
					set
					{
						_endDate = value;
					}
				}
				private int _delta;
				public int Delta
				{
					// ReSharper disable UnusedMember.Local
					get
					{
						// ReSharper restore UnusedMember.Local
						return _delta;
					}
					set
					{
						_delta = value;
					}
				}
				
			}
			
#endregion
			
		}
#endregion
		
	}
	
	

