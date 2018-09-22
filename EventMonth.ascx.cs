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


using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Components;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Modules.Events.ScheduleControl.MonthControl;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using Microsoft.VisualBasic;
using Telerik.Web.UI.Calendar;
using DayRenderEventArgs = System.Web.UI.WebControls.DayRenderEventArgs;
using FirstDayOfWeek = System.Web.UI.WebControls.FirstDayOfWeek;
using Globals = DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.Events
{
    public partial class EventMonth : EventBase
    {
        #region Private Variables

        private bool _pageBound;
        private ArrayList _selectedEvents;
        private readonly CultureInfo _culture = Thread.CurrentThread.CurrentCulture;
        private EventInfoHelper _objEventInfoHelper;

        #endregion

        #region Event Handlers

        private void Page_PreRender(object sender, EventArgs e)
        {
            // This handles the case where the same cell date is selected twice
            if (!_pageBound)
            {
                BindDataGrid();
            }
        }

        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Be sure to load the required scripts always
                JavaScript.RequestRegistration(CommonJs.DnnPlugins);

                LocalizeAll();

                SetupViewControls(EventIcons, EventIcons2, SelectCategory, SelectLocation,
                                       pnlDateControls);

                dpGoToDate.SelectedDate = SelectedDate.Date;
                dpGoToDate.Calendar.FirstDayOfWeek = Settings.WeekStart;

                // Set Weekend Display
                if (Settings.Fridayweekend)
                {
                    EventCalendar.WeekEndDays = MyDayOfWeek.Friday | MyDayOfWeek.Saturday;
                }

                // Set 1st Day of Week
                EventCalendar.FirstDayOfWeek = (FirstDayOfWeek) _culture.DateTimeFormat.FirstDayOfWeek;

                if (Settings.WeekStart != FirstDayOfWeek.Default)
                {
                    EventCalendar.FirstDayOfWeek = Settings.WeekStart;
                }

                // if 1st time on page...
                if (!Page.IsPostBack)
                {
                    EventCalendar.VisibleDate = Convert.ToDateTime(dpGoToDate.SelectedDate);
                    if (!Settings.Monthcellnoevents)
                    {
                        EventCalendar.SelectedDate = EventCalendar.VisibleDate;
                    }
                    BindDataGrid();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        #region  Web Form Designer Generated Code

        //This call is required by the Web Form Designer.
        [DebuggerStepThrough]
        private void InitializeComponent()
        { }

        private void Page_Init(object sender, EventArgs e)
        {
            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }

        #endregion

        #region Helper Methods & Functions

        private void LocalizeAll()
        {
            lnkToday.Text = Localization.GetString("lnkToday", LocalResourceFile);
            dpGoToDate.DatePopupButton.ToolTip =
                Localization.GetString("DatePickerTooltip", LocalResourceFile);
        }

        private void BindDataGrid()
        {
            var startDate = default(DateTime); // Start View Date Events Range
            var endDate = default(DateTime); // End View Date Events Range
            var objEventInfoHelper = new EventInfoHelper(ModuleId, TabId, PortalId, Settings);

            _pageBound = true;
            //****DO NOT CHANGE THE NEXT SECTION FOR ML CODING ****
            // Used Only to select view dates on Event Month View...
            var useDate = Convert.ToDateTime(dpGoToDate.SelectedDate);
            var initDate = new DateTime(useDate.Year, useDate.Month, 1);
            startDate = initDate.AddDays(-10); // Allow for Prev Month days in View
            // Load 2 months of events.  This used to load only the events for the current month,
            // but was changed so that events for multiple events can be displayed in the case when
            // the Event displays some days for the next month.
            endDate = Convert.ToDateTime(initDate.AddMonths(1).AddDays(10));

            var getSubEvents = Settings.MasterEvent;
            _selectedEvents =
                objEventInfoHelper.GetEvents(startDate, endDate, getSubEvents, SelectCategory.SelectedCategory,
                                             SelectLocation.SelectedLocation, GetUrlGroupId(),
                                             GetUrlUserId());

            _selectedEvents =
                objEventInfoHelper.ConvertEventListToDisplayTimeZone(_selectedEvents, GetDisplayTimeZoneId());

            //Write current date to UI
            SelectedDate = Convert.ToDateTime(EventCalendar.VisibleDate);

            // Setup the Tooltip TargetControls because it doesn't work in DayRender!
            if (Settings.Eventtooltipmonth)
            {
                toolTipManager.TargetControls.Clear();
                if (Settings.Monthcellnoevents)
                {
                    var calcDate = startDate;
                    while (calcDate <= endDate)
                    {
                        toolTipManager.TargetControls.Add(
                            "ctlEvents_Mod_" + ModuleId + "_EventDate_" + calcDate.Date.ToString("yyyyMMMdd"),
                            true);
                        calcDate = calcDate.AddDays(1);
                    }
                }
                else
                {
                    foreach (EventInfo objEvent in _selectedEvents)
                    {
                        var calcDate = objEvent.EventTimeBegin.Date;
                        while (calcDate <= objEvent.EventTimeEnd.Date)
                        {
                            toolTipManager.TargetControls.Add(
                                "ctlEvents_Mod_" + ModuleId + "_EventID_" + objEvent.EventID + "_EventDate_" +
                                calcDate.Date.ToString("yyyyMMMdd"), true);
                            calcDate = calcDate.AddDays(1);
                        }
                    }
                }
            }
        }

        #endregion

        #region Event Event Grid Methods and Functions

        /// <summary>
        ///     Render each day in the event (i.e. Cells)
        /// </summary>
        protected void EventCalendar_DayRender(object sender, DayRenderEventArgs e)
        {
            var objEvent = default(EventInfo);
            var cellcontrol = new LiteralControl();
            _objEventInfoHelper = new EventInfoHelper(ModuleId, TabId, PortalId, Settings);

            // Get Events/Sub-Calendar Events
            var dayEvents = new ArrayList();
            var allDayEvents = default(ArrayList);
            allDayEvents = _objEventInfoHelper.GetDateEvents(_selectedEvents, e.Day.Date);
            allDayEvents.Sort(new EventInfoHelper.EventDateSort());

            foreach (EventInfo tempLoopVar_objEvent in allDayEvents)
            {
                objEvent = tempLoopVar_objEvent;
                //if day not in current (selected) Event month OR full enrollments should be hidden, ignore
                if ((Settings.ShowEventsAlways || e.Day.Date.Month == SelectedDate.Month)
                    && !HideFullEvent(objEvent))
                {
                    dayEvents.Add(objEvent);
                }
            }

            // If No Cell Event Display...
            if (Settings.Monthcellnoevents)
            {
                if (Settings.ShowEventsAlways == false && e.Day.IsOtherMonth)
                {
                    e.Cell.Text = "";
                    return;
                }

                if (dayEvents.Count > 0)
                {
                    e.Day.IsSelectable = true;

                    if (e.Day.Date == SelectedDate)
                    {
                        e.Cell.CssClass = "EventSelectedDay";
                    }
                    else
                    {
                        if (e.Day.IsWeekend)
                        {
                            e.Cell.CssClass = "EventWeekendDayEvents";
                        }
                        else
                        {
                            e.Cell.CssClass = "EventDayEvents";
                        }
                    }

                    if (Settings.Eventtooltipmonth)
                    {
                        var themeCss = GetThemeSettings().CssClass;

                        var tmpToolTipTitle = Settings.Templates.txtTooltipTemplateTitleNT;
                        if (tmpToolTipTitle.IndexOf("{0}") + 1 > 0)
                        {
                            tmpToolTipTitle = tmpToolTipTitle.Replace("{0}", "{0:d}");
                        }
                        var tooltipTitle =
                            Convert.ToString(HttpUtility.HtmlDecode(string.Format(tmpToolTipTitle, e.Day.Date))
                                                        .Replace(Environment.NewLine, ""));
                        var cellToolTip = ""; //Holds control generated tooltip

                        foreach (EventInfo tempLoopVar_objEvent in dayEvents)
                        {
                            objEvent = tempLoopVar_objEvent;
                            //Add horizontal row to seperate the eventdescriptions
                            if (!string.IsNullOrEmpty(cellToolTip))
                            {
                                cellToolTip = cellToolTip + "<hr/>";
                            }

                            cellToolTip +=
                                CreateEventName(
                                    objEvent,
                                    Convert.ToString(Settings.Templates.txtTooltipTemplateBodyNT
                                                         .Replace(Constants.vbLf, "").Replace(Constants.vbCr, "")));
                        }
                        e.Cell.Attributes.Add(
                            "title",
                            "<table class=\"" + themeCss + " Eventtooltiptable\"><tr><td class=\"" + themeCss +
                            " Eventtooltipheader\">" + tooltipTitle + "</td></tr><tr><td class=\"" + themeCss +
                            " Eventtooltipbody\">" + cellToolTip + "</td></tr></table>");
                        e.Cell.ID = "ctlEvents_Mod_" + ModuleId + "_EventDate_" + e.Day.Date.ToString("yyyyMMMdd");
                    }

                    var dailyLink = new HyperLink();
                    dailyLink.Text = string.Format(Settings.Templates.txtMonthDayEventCount, dayEvents.Count);
                    var socialGroupId = GetUrlGroupId();
                    var socialUserId = GetUrlUserId();
                    if (dayEvents.Count > 1)
                    {
                        if (Settings.Eventdaynewpage)
                        {
                            if (socialGroupId > 0)
                            {
                                dailyLink.NavigateUrl =
                                    _objEventInfoHelper.AddSkinContainerControls(
                                        Globals.NavigateURL(TabId, "Day", "Mid=" + ModuleId,
                                                            "selecteddate=" +
                                                            Strings.Format(e.Day.Date, "yyyyMMdd"),
                                                            "groupid=" + socialGroupId), "?");
                            }
                            else if (socialUserId > 0)
                            {
                                dailyLink.NavigateUrl =
                                    _objEventInfoHelper.AddSkinContainerControls(
                                        Globals.NavigateURL(TabId, "Day", "Mid=" + ModuleId,
                                                            "selecteddate=" +
                                                            Strings.Format(e.Day.Date, "yyyyMMdd"),
                                                            "userid=" + socialUserId), "?");
                            }
                            else
                            {
                                dailyLink.NavigateUrl =
                                    _objEventInfoHelper.AddSkinContainerControls(
                                        Globals.NavigateURL(TabId, "Day", "Mid=" + ModuleId,
                                                            "selecteddate=" +
                                                            Strings.Format(e.Day.Date, "yyyyMMdd")), "?");
                            }
                        }
                        else
                        {
                            if (socialGroupId > 0)
                            {
                                dailyLink.NavigateUrl =
                                    Globals.NavigateURL(TabId, "", "ModuleID=" + ModuleId, "mctl=EventDay",
                                                        "selecteddate=" + Strings.Format(e.Day.Date, "yyyyMMdd"),
                                                        "groupid=" + socialGroupId);
                            }
                            else if (socialUserId > 0)
                            {
                                dailyLink.NavigateUrl =
                                    Globals.NavigateURL(TabId, "", "ModuleID=" + ModuleId, "mctl=EventDay",
                                                        "selecteddate=" + Strings.Format(e.Day.Date, "yyyyMMdd"),
                                                        "userid=" + socialUserId);
                            }
                            else
                            {
                                dailyLink.NavigateUrl =
                                    Globals.NavigateURL(TabId, "", "ModuleID=" + ModuleId, "mctl=EventDay",
                                                        "selecteddate=" + Strings.Format(e.Day.Date, "yyyyMMdd"));
                            }
                        }
                    }
                    else
                    {
                        // Get detail page url
                        dailyLink = GetDetailPageUrl((EventInfo) dayEvents[0], dailyLink);
                    }
                    using (var stringWrite = new StringWriter())
                    {
                        using (var eventoutput = new HtmlTextWriter(stringWrite))
                        {
                            dailyLink.RenderControl(eventoutput);
                            cellcontrol.Text = "<div class='EventDayScroll'>" + stringWrite + "</div>";
                            e.Cell.Controls.Add(cellcontrol);
                        }
                    }
                }
                else
                {
                    e.Day.IsSelectable = false;
                }
                return;
            }

            //Make day unselectable
            if (!Settings.Monthdayselect)
            {
                e.Day.IsSelectable = false;
            }

            //loop through records and render if startDate = current day and is not null
            var celldata = ""; // Holds Control Generated HTML

            foreach (EventInfo tempLoopVar_objEvent in dayEvents)
            {
                objEvent = tempLoopVar_objEvent;
                var dailyLink = new HyperLink();
                var iconString = "";

                // See if an Image is to be displayed for the Event
                if (Settings.Eventimage && Settings.EventImageMonth && objEvent.ImageURL != null &&
                    objEvent.ImageDisplay)
                {
                    dailyLink.Text = ImageInfo(objEvent.ImageURL, objEvent.ImageHeight, objEvent.ImageWidth);
                }

                if (Settings.Timeintitle)
                {
                    dailyLink.Text = dailyLink.Text + objEvent.EventTimeBegin.ToString("t") + " - ";
                }

                var eventtext = CreateEventName(objEvent, Settings.Templates.txtMonthEventText);
                dailyLink.Text = dailyLink.Text + eventtext.Trim();

                if (!IsPrivateNotModerator || UserId == objEvent.OwnerID)
                {
                    dailyLink.ForeColor = GetColor(objEvent.FontColor);
                    iconString = CreateIconString(objEvent, Settings.IconMonthPrio,
                                                       Settings.IconMonthRec, Settings.IconMonthReminder,
                                                       Settings.IconMonthEnroll);

                    // Get detail page url
                    dailyLink = GetDetailPageUrl(objEvent, dailyLink);
                }
                else
                {
                    dailyLink.Style.Add("cursor", "text");
                    dailyLink.Style.Add("text-decoration", "none");
                    dailyLink.Attributes.Add("onclick", "javascript:return false;");
                }

                // See If Description Tooltip to be added
                if (Settings.Eventtooltipmonth)
                {
                    var isEvtEditor = IsEventEditor(objEvent, false);
                    dailyLink.Attributes.Add(
                        "title",
                        ToolTipCreate(objEvent, Settings.Templates.txtTooltipTemplateTitle,
                                           Settings.Templates.txtTooltipTemplateBody, isEvtEditor));
                }

                // Capture Control Info & save
                using (var stringWrite = new StringWriter())
                {
                    using (var eventoutput = new HtmlTextWriter(stringWrite))
                    {
                        dailyLink.ID = "ctlEvents_Mod_" + ModuleId + "_EventID_" + objEvent.EventID +
                                       "_EventDate_" + e.Day.Date.ToString("yyyyMMMdd");
                        dailyLink.RenderControl(eventoutput);
                        if (objEvent.Color != null && (!IsPrivateNotModerator || UserId == objEvent.OwnerID))
                        {
                            celldata = celldata + "<div style=\"background-color: " + objEvent.Color + ";\">" +
                                       iconString + stringWrite + "</div>";
                        }
                        else
                        {
                            celldata = celldata + "<div>" + iconString + stringWrite + "</div>";
                        }
                    }
                }
            }

            // Add Literal Control Data to Cell w/DIV tag (in order to support scrolling in cell)
            cellcontrol.Text = "<div class='EventDayScroll'>" + celldata + "</div>";
            e.Cell.Controls.Add(cellcontrol);
        }


        protected void EventCalendar_SelectionChanged(object sender, EventArgs e)
        {
            EventCalendar.VisibleDate = EventCalendar.SelectedDate;
            SelectedDate = Convert.ToDateTime(EventCalendar.SelectedDate.Date);
            var urlDate = Convert.ToString(EventCalendar.SelectedDate.Date.ToShortDateString());
            dpGoToDate.SelectedDate = SelectedDate.Date;
            if (Settings.Monthcellnoevents)
            {
                try
                {
                    EventCalendar.SelectedDate = new DateTime();
                    if (Settings.Eventdaynewpage)
                    {
                        var objEventInfoHelper =
                            new EventInfoHelper(ModuleId, TabId, PortalId, Settings);
                        Response.Redirect(
                            objEventInfoHelper.AddSkinContainerControls(
                                Globals.NavigateURL(TabId, "Day", "Mid=" + ModuleId,
                                                    "selecteddate=" + urlDate), "&"));
                    }
                    else
                    {
                        Response.Redirect(
                            Globals.NavigateURL(TabId, "", "ModuleID=" + ModuleId, "mctl=EventDay",
                                                "selecteddate=" + urlDate));
                    }
                }
                catch (Exception)
                { }
            }
            else
            {
                //fill grid with current selection's data
                BindDataGrid();
            }
        }

        protected void EventCalendar_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {
            //set selected date to first of month
            SelectedDate = e.NewDate;
            dpGoToDate.SelectedDate = e.NewDate.Date;
            if (!Settings.Monthcellnoevents)
            {
                EventCalendar.SelectedDate = e.NewDate;
            }
            SelectCategory.StoreCategories();
            SelectLocation.StoreLocations();
            //bind datagrid
            BindDataGrid();
        }

        private HyperLink GetDetailPageUrl(EventInfo objevent, HyperLink dailyLink)
        {
            // Get detail page url
            dailyLink.NavigateUrl = _objEventInfoHelper.DetailPageURL(objevent);
            if (objevent.DetailPage && objevent.DetailNewWin)
            {
                dailyLink.Attributes.Add("target", "_blank");
            }
            return dailyLink;
        }

        #endregion

        #region Links and Buttons

        protected void lnkToday_Click(object sender, EventArgs e)
        {
            //set grid uneditable
            SelectedDate = DateTime.Now.Date;
            EventCalendar.VisibleDate = SelectedDate;
            dpGoToDate.SelectedDate = SelectedDate.Date;
            if (!Settings.Monthcellnoevents)
            {
                EventCalendar.SelectedDate = SelectedDate;
            }
            SelectCategory.StoreCategories();
            SelectLocation.StoreLocations();
            //fill grid with current selection's data
            BindDataGrid();
        }

        protected void SelectCategoryChanged(object sender, CommandEventArgs e)
        {
            //Store the other selection(s) too.
            SelectLocation.StoreLocations();
            BindDataGrid();
        }

        protected void SelectLocationChanged(object sender, CommandEventArgs e)
        {
            //Store the other selection(s) too.
            SelectCategory.StoreCategories();
            BindDataGrid();
        }

        protected void dpGoToDate_SelectedDateChanged(object sender, SelectedDateChangedEventArgs e)
        {
            var dDate = Convert.ToDateTime(dpGoToDate.SelectedDate);
            SelectedDate = dDate;
            EventCalendar.VisibleDate = dDate;
            if (!Settings.Monthcellnoevents)
            {
                EventCalendar.SelectedDate = dDate;
            }
            //fill grid with current selection's data
            BindDataGrid();
        }

        #endregion
    }
}