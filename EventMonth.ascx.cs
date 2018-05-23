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
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using DotNetNuke.Framework.JavaScriptLibraries;
    using DotNetNuke.Modules.Events.ScheduleControl.MonthControl;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using global::Components;
    using Microsoft.VisualBasic;
    using Telerik.Web.UI.Calendar;
    using DayRenderEventArgs = System.Web.UI.WebControls.DayRenderEventArgs;
    using FirstDayOfWeek = System.Web.UI.WebControls.FirstDayOfWeek;
    using Globals = DotNetNuke.Common.Globals;

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
            if (!this._pageBound)
            {
                this.BindDataGrid();
            }
        }

        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Be sure to load the required scripts always
                JavaScript.RequestRegistration(CommonJs.DnnPlugins);

                this.LocalizeAll();

                this.SetupViewControls(this.EventIcons, this.EventIcons2, this.SelectCategory, this.SelectLocation,
                                       this.pnlDateControls);

                this.dpGoToDate.SelectedDate = this.SelectedDate.Date;
                this.dpGoToDate.Calendar.FirstDayOfWeek = this.Settings.WeekStart;

                // Set Weekend Display
                if (this.Settings.Fridayweekend)
                {
                    this.EventCalendar.WeekEndDays = MyDayOfWeek.Friday | MyDayOfWeek.Saturday;
                }

                // Set 1st Day of Week
                this.EventCalendar.FirstDayOfWeek = (FirstDayOfWeek) this._culture.DateTimeFormat.FirstDayOfWeek;

                if (this.Settings.WeekStart != FirstDayOfWeek.Default)
                {
                    this.EventCalendar.FirstDayOfWeek = this.Settings.WeekStart;
                }

                // if 1st time on page...
                if (!this.Page.IsPostBack)
                {
                    this.EventCalendar.VisibleDate = Convert.ToDateTime(this.dpGoToDate.SelectedDate);
                    if (!this.Settings.Monthcellnoevents)
                    {
                        this.EventCalendar.SelectedDate = this.EventCalendar.VisibleDate;
                    }
                    this.BindDataGrid();
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
            this.InitializeComponent();
        }

        #endregion

        #region Helper Methods & Functions

        private void LocalizeAll()
        {
            this.lnkToday.Text = Localization.GetString("lnkToday", this.LocalResourceFile);
            this.dpGoToDate.DatePopupButton.ToolTip =
                Localization.GetString("DatePickerTooltip", this.LocalResourceFile);
        }

        private void BindDataGrid()
        {
            var startDate = default(DateTime); // Start View Date Events Range
            var endDate = default(DateTime); // End View Date Events Range
            var objEventInfoHelper = new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);

            this._pageBound = true;
            //****DO NOT CHANGE THE NEXT SECTION FOR ML CODING ****
            // Used Only to select view dates on Event Month View...
            var useDate = Convert.ToDateTime(this.dpGoToDate.SelectedDate);
            var initDate = new DateTime(useDate.Year, useDate.Month, 1);
            startDate = initDate.AddDays(-10); // Allow for Prev Month days in View
            // Load 2 months of events.  This used to load only the events for the current month,
            // but was changed so that events for multiple events can be displayed in the case when
            // the Event displays some days for the next month.
            endDate = Convert.ToDateTime(initDate.AddMonths(1).AddDays(10));

            var getSubEvents = this.Settings.MasterEvent;
            this._selectedEvents =
                objEventInfoHelper.GetEvents(startDate, endDate, getSubEvents, this.SelectCategory.SelectedCategory,
                                             this.SelectLocation.SelectedLocation, this.GetUrlGroupId(),
                                             this.GetUrlUserId());

            this._selectedEvents =
                objEventInfoHelper.ConvertEventListToDisplayTimeZone(this._selectedEvents, this.GetDisplayTimeZoneId());

            //Write current date to UI
            this.SelectedDate = Convert.ToDateTime(this.EventCalendar.VisibleDate);

            // Setup the Tooltip TargetControls because it doesn't work in DayRender!
            if (this.Settings.Eventtooltipmonth)
            {
                this.toolTipManager.TargetControls.Clear();
                if (this.Settings.Monthcellnoevents)
                {
                    var calcDate = startDate;
                    while (calcDate <= endDate)
                    {
                        this.toolTipManager.TargetControls.Add(
                            "ctlEvents_Mod_" + this.ModuleId + "_EventDate_" + calcDate.Date.ToString("yyyyMMMdd"),
                            true);
                        calcDate = calcDate.AddDays(1);
                    }
                }
                else
                {
                    foreach (EventInfo objEvent in this._selectedEvents)
                    {
                        var calcDate = objEvent.EventTimeBegin.Date;
                        while (calcDate <= objEvent.EventTimeEnd.Date)
                        {
                            this.toolTipManager.TargetControls.Add(
                                "ctlEvents_Mod_" + this.ModuleId + "_EventID_" + objEvent.EventID + "_EventDate_" +
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
            this._objEventInfoHelper = new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);

            // Get Events/Sub-Calendar Events
            var dayEvents = new ArrayList();
            var allDayEvents = default(ArrayList);
            allDayEvents = this._objEventInfoHelper.GetDateEvents(this._selectedEvents, e.Day.Date);
            allDayEvents.Sort(new EventInfoHelper.EventDateSort());

            foreach (EventInfo tempLoopVar_objEvent in allDayEvents)
            {
                objEvent = tempLoopVar_objEvent;
                //if day not in current (selected) Event month OR full enrollments should be hidden, ignore
                if ((this.Settings.ShowEventsAlways || e.Day.Date.Month == this.SelectedDate.Month)
                    && !this.HideFullEvent(objEvent))
                {
                    dayEvents.Add(objEvent);
                }
            }

            // If No Cell Event Display...
            if (this.Settings.Monthcellnoevents)
            {
                if (this.Settings.ShowEventsAlways == false && e.Day.IsOtherMonth)
                {
                    e.Cell.Text = "";
                    return;
                }

                if (dayEvents.Count > 0)
                {
                    e.Day.IsSelectable = true;

                    if (e.Day.Date == this.SelectedDate)
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

                    if (this.Settings.Eventtooltipmonth)
                    {
                        var themeCss = this.GetThemeSettings().CssClass;

                        var tmpToolTipTitle = this.Settings.Templates.txtTooltipTemplateTitleNT;
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
                                this.CreateEventName(
                                    objEvent,
                                    Convert.ToString(this.Settings.Templates.txtTooltipTemplateBodyNT
                                                         .Replace(Constants.vbLf, "").Replace(Constants.vbCr, "")));
                        }
                        e.Cell.Attributes.Add(
                            "title",
                            "<table class=\"" + themeCss + " Eventtooltiptable\"><tr><td class=\"" + themeCss +
                            " Eventtooltipheader\">" + tooltipTitle + "</td></tr><tr><td class=\"" + themeCss +
                            " Eventtooltipbody\">" + cellToolTip + "</td></tr></table>");
                        e.Cell.ID = "ctlEvents_Mod_" + this.ModuleId + "_EventDate_" + e.Day.Date.ToString("yyyyMMMdd");
                    }

                    var dailyLink = new HyperLink();
                    dailyLink.Text = string.Format(this.Settings.Templates.txtMonthDayEventCount, dayEvents.Count);
                    var socialGroupId = this.GetUrlGroupId();
                    var socialUserId = this.GetUrlUserId();
                    if (dayEvents.Count > 1)
                    {
                        if (this.Settings.Eventdaynewpage)
                        {
                            if (socialGroupId > 0)
                            {
                                dailyLink.NavigateUrl =
                                    this._objEventInfoHelper.AddSkinContainerControls(
                                        Globals.NavigateURL(this.TabId, "Day", "Mid=" + this.ModuleId,
                                                            "selecteddate=" +
                                                            Strings.Format(e.Day.Date, "yyyyMMdd"),
                                                            "groupid=" + socialGroupId), "?");
                            }
                            else if (socialUserId > 0)
                            {
                                dailyLink.NavigateUrl =
                                    this._objEventInfoHelper.AddSkinContainerControls(
                                        Globals.NavigateURL(this.TabId, "Day", "Mid=" + this.ModuleId,
                                                            "selecteddate=" +
                                                            Strings.Format(e.Day.Date, "yyyyMMdd"),
                                                            "userid=" + socialUserId), "?");
                            }
                            else
                            {
                                dailyLink.NavigateUrl =
                                    this._objEventInfoHelper.AddSkinContainerControls(
                                        Globals.NavigateURL(this.TabId, "Day", "Mid=" + this.ModuleId,
                                                            "selecteddate=" +
                                                            Strings.Format(e.Day.Date, "yyyyMMdd")), "?");
                            }
                        }
                        else
                        {
                            if (socialGroupId > 0)
                            {
                                dailyLink.NavigateUrl =
                                    Globals.NavigateURL(this.TabId, "", "ModuleID=" + this.ModuleId, "mctl=EventDay",
                                                        "selecteddate=" + Strings.Format(e.Day.Date, "yyyyMMdd"),
                                                        "groupid=" + socialGroupId);
                            }
                            else if (socialUserId > 0)
                            {
                                dailyLink.NavigateUrl =
                                    Globals.NavigateURL(this.TabId, "", "ModuleID=" + this.ModuleId, "mctl=EventDay",
                                                        "selecteddate=" + Strings.Format(e.Day.Date, "yyyyMMdd"),
                                                        "userid=" + socialUserId);
                            }
                            else
                            {
                                dailyLink.NavigateUrl =
                                    Globals.NavigateURL(this.TabId, "", "ModuleID=" + this.ModuleId, "mctl=EventDay",
                                                        "selecteddate=" + Strings.Format(e.Day.Date, "yyyyMMdd"));
                            }
                        }
                    }
                    else
                    {
                        // Get detail page url
                        dailyLink = this.GetDetailPageUrl((EventInfo) dayEvents[0], dailyLink);
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
            if (!this.Settings.Monthdayselect)
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
                if (this.Settings.Eventimage && this.Settings.EventImageMonth && objEvent.ImageURL != null &&
                    objEvent.ImageDisplay)
                {
                    dailyLink.Text = this.ImageInfo(objEvent.ImageURL, objEvent.ImageHeight, objEvent.ImageWidth);
                }

                if (this.Settings.Timeintitle)
                {
                    dailyLink.Text = dailyLink.Text + objEvent.EventTimeBegin.ToString("t") + " - ";
                }

                var eventtext = this.CreateEventName(objEvent, this.Settings.Templates.txtMonthEventText);
                dailyLink.Text = dailyLink.Text + eventtext.Trim();

                if (!this.IsPrivateNotModerator || this.UserId == objEvent.OwnerID)
                {
                    dailyLink.ForeColor = this.GetColor(objEvent.FontColor);
                    iconString = this.CreateIconString(objEvent, this.Settings.IconMonthPrio,
                                                       this.Settings.IconMonthRec, this.Settings.IconMonthReminder,
                                                       this.Settings.IconMonthEnroll);

                    // Get detail page url
                    dailyLink = this.GetDetailPageUrl(objEvent, dailyLink);
                }
                else
                {
                    dailyLink.Style.Add("cursor", "text");
                    dailyLink.Style.Add("text-decoration", "none");
                    dailyLink.Attributes.Add("onclick", "javascript:return false;");
                }

                // See If Description Tooltip to be added
                if (this.Settings.Eventtooltipmonth)
                {
                    var isEvtEditor = this.IsEventEditor(objEvent, false);
                    dailyLink.Attributes.Add(
                        "title",
                        this.ToolTipCreate(objEvent, this.Settings.Templates.txtTooltipTemplateTitle,
                                           this.Settings.Templates.txtTooltipTemplateBody, isEvtEditor));
                }

                // Capture Control Info & save
                using (var stringWrite = new StringWriter())
                {
                    using (var eventoutput = new HtmlTextWriter(stringWrite))
                    {
                        dailyLink.ID = "ctlEvents_Mod_" + this.ModuleId + "_EventID_" + objEvent.EventID +
                                       "_EventDate_" + e.Day.Date.ToString("yyyyMMMdd");
                        dailyLink.RenderControl(eventoutput);
                        if (objEvent.Color != null && (!this.IsPrivateNotModerator || this.UserId == objEvent.OwnerID))
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
            this.EventCalendar.VisibleDate = this.EventCalendar.SelectedDate;
            this.SelectedDate = Convert.ToDateTime(this.EventCalendar.SelectedDate.Date);
            var urlDate = Convert.ToString(this.EventCalendar.SelectedDate.Date.ToShortDateString());
            this.dpGoToDate.SelectedDate = this.SelectedDate.Date;
            if (this.Settings.Monthcellnoevents)
            {
                try
                {
                    this.EventCalendar.SelectedDate = new DateTime();
                    if (this.Settings.Eventdaynewpage)
                    {
                        var objEventInfoHelper =
                            new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);
                        this.Response.Redirect(
                            objEventInfoHelper.AddSkinContainerControls(
                                Globals.NavigateURL(this.TabId, "Day", "Mid=" + this.ModuleId,
                                                    "selecteddate=" + urlDate), "&"));
                    }
                    else
                    {
                        this.Response.Redirect(
                            Globals.NavigateURL(this.TabId, "", "ModuleID=" + this.ModuleId, "mctl=EventDay",
                                                "selecteddate=" + urlDate));
                    }
                }
                catch (Exception)
                { }
            }
            else
            {
                //fill grid with current selection's data
                this.BindDataGrid();
            }
        }

        protected void EventCalendar_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {
            //set selected date to first of month
            this.SelectedDate = e.NewDate;
            this.dpGoToDate.SelectedDate = e.NewDate.Date;
            if (!this.Settings.Monthcellnoevents)
            {
                this.EventCalendar.SelectedDate = e.NewDate;
            }
            this.SelectCategory.StoreCategories();
            this.SelectLocation.StoreLocations();
            //bind datagrid
            this.BindDataGrid();
        }

        private HyperLink GetDetailPageUrl(EventInfo objevent, HyperLink dailyLink)
        {
            // Get detail page url
            dailyLink.NavigateUrl = this._objEventInfoHelper.DetailPageURL(objevent);
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
            this.SelectedDate = DateTime.Now.Date;
            this.EventCalendar.VisibleDate = this.SelectedDate;
            this.dpGoToDate.SelectedDate = this.SelectedDate.Date;
            if (!this.Settings.Monthcellnoevents)
            {
                this.EventCalendar.SelectedDate = this.SelectedDate;
            }
            this.SelectCategory.StoreCategories();
            this.SelectLocation.StoreLocations();
            //fill grid with current selection's data
            this.BindDataGrid();
        }

        protected void SelectCategoryChanged(object sender, CommandEventArgs e)
        {
            //Store the other selection(s) too.
            this.SelectLocation.StoreLocations();
            this.BindDataGrid();
        }

        protected void SelectLocationChanged(object sender, CommandEventArgs e)
        {
            //Store the other selection(s) too.
            this.SelectCategory.StoreCategories();
            this.BindDataGrid();
        }

        protected void dpGoToDate_SelectedDateChanged(object sender, SelectedDateChangedEventArgs e)
        {
            var dDate = Convert.ToDateTime(this.dpGoToDate.SelectedDate);
            this.SelectedDate = dDate;
            this.EventCalendar.VisibleDate = dDate;
            if (!this.Settings.Monthcellnoevents)
            {
                this.EventCalendar.SelectedDate = dDate;
            }
            //fill grid with current selection's data
            this.BindDataGrid();
        }

        #endregion
    }
}