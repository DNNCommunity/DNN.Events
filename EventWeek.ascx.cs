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
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading;
    using System.Web.UI.WebControls;
    using DotNetNuke.Modules.Events.ScheduleControl;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using global::Components;
    using Microsoft.VisualBasic;
    using Telerik.Web.UI.Calendar;
    using FirstDayOfWeek = System.Web.UI.WebControls.FirstDayOfWeek;

    public partial class EventWeek : EventBase
    {
        #region Page Events

        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.LocalizeAll();

                this.SetupViewControls(this.EventIcons, this.EventIcons2, this.SelectCategory, this.SelectLocation,
                                       this.pnlDateControls);
                var initDate = this.SelectedDate.Date;
                this.dpGoToDate.SelectedDate = initDate;
                this.dpGoToDate.Calendar.FirstDayOfWeek = this.Settings.WeekStart;

                if (!this.IsPostBack)
                {
                    this.schWeek.StartDay = this._culture.DateTimeFormat.FirstDayOfWeek;
                    if (this.Settings.WeekStart != FirstDayOfWeek.Default)
                    {
                        this.schWeek.StartDay = (DayOfWeek) this.Settings.WeekStart;
                    }
                    if (this.Settings.Fulltimescale)
                    {
                        this.schWeek.FullTimeScale = true;
                    }
                    if (this.Settings.Includeendvalue)
                    {
                        this.schWeek.IncludeEndValue = true;
                    }
                    else
                    {
                        this.schWeek.IncludeEndValue = false;
                    }
                    if (this.Settings.Showvaluemarks)
                    {
                        this.schWeek.ShowValueMarks = true;
                    }
                    else
                    {
                        this.schWeek.ShowValueMarks = false;
                    }
                    this.BindPage(initDate);
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

        #region Private Variables

        private DateTime _dWeekStart;
        private ArrayList _selectedEvents;
        private readonly CultureInfo _culture = Thread.CurrentThread.CurrentCulture;

        #endregion

        #region Helper Methods

        private void LocalizeAll()
        {
            this.lnkToday.Text = Localization.GetString("lnkToday", this.LocalResourceFile);
            this.dpGoToDate.DatePopupButton.ToolTip =
                Localization.GetString("DatePickerTooltip", this.LocalResourceFile);
        }

        private void BindPage(DateTime dDate)
        {
            var dBegin = default(DateTime);
            var dEnd = default(DateTime);
            var sBegin = default(DateTime);
            var sEnd = default(DateTime);
            var objEventInfoHelper = new EventInfoHelper(this.ModuleId, this.TabId, this.PortalId, this.Settings);

            try
            {
                // Set Date Range
                if (this.Settings.WeekStart != FirstDayOfWeek.Default)
                {
                    this._dWeekStart =
                        dDate.AddDays(Convert.ToDouble(-(int) dDate.DayOfWeek + this.Settings.WeekStart));
                }
                else
                {
                    this._dWeekStart = dDate.AddDays(Convert.ToDouble(-(int) dDate.DayOfWeek));
                }
                if (((int) dDate.DayOfWeek < (int) this.Settings.WeekStart) &
                    (this.Settings.WeekStart != FirstDayOfWeek.Default))
                {
                    this._dWeekStart = this._dWeekStart.AddDays(-7);
                }
                this.lblWeekOf.Text = string.Format(Localization.GetString("capWeekEvent", this.LocalResourceFile),
                                                    DateAndTime.DatePart(
                                                        DateInterval.WeekOfYear, this._dWeekStart,
                                                        FirstWeekOfYearValue: FirstWeekOfYear.FirstFourDays),
                                                    this._dWeekStart.ToLongDateString());
                this.ViewState[this.ModuleId + "WeekOf"] = this._dWeekStart.ToShortDateString();

                // Allow 7 days for events that might start before beginning of week
                sBegin = this._dWeekStart;
                dBegin = DateAndTime.DateAdd(DateInterval.Day, -7, this._dWeekStart);
                sEnd = DateAndTime.DateAdd(DateInterval.Day, Convert.ToDouble(+7), this._dWeekStart);
                dEnd = sEnd;

                // Get Events/Sub-Calendar Events

                var getSubEvents = this.Settings.MasterEvent;
                this._selectedEvents =
                    objEventInfoHelper.GetEvents(dBegin, dEnd, getSubEvents, this.SelectCategory.SelectedCategory,
                                                 this.SelectLocation.SelectedLocation, this.GetUrlGroupId(),
                                                 this.GetUrlUserId());

                this._selectedEvents =
                    objEventInfoHelper.ConvertEventListToDisplayTimeZone(
                        this._selectedEvents, this.GetDisplayTimeZoneId());

                // Setup ScheduleGeneral
                // Create DataView
                var eventTable = new DataTable("Events");
                eventTable.Columns.Add("ID", Type.GetType("System.Int32"));
                eventTable.Columns.Add("CreatedByID", Type.GetType("System.Int32"));
                eventTable.Columns.Add("OwnerID", Type.GetType("System.Int32"));
                eventTable.Columns.Add("StartTime", Type.GetType("System.DateTime"));
                eventTable.Columns.Add("EndTime", Type.GetType("System.DateTime"));
                eventTable.Columns.Add("Icons", Type.GetType("System.String"));
                eventTable.Columns.Add("Task", Type.GetType("System.String"));
                eventTable.Columns.Add("Description", Type.GetType("System.String"));
                eventTable.Columns.Add("StartDateTime", Type.GetType("System.DateTime"));
                eventTable.Columns.Add("Duration", Type.GetType("System.Int32"));
                eventTable.Columns.Add("URL", Type.GetType("System.String"));
                eventTable.Columns.Add("Target", Type.GetType("System.String"));
                eventTable.Columns.Add("Tooltip", Type.GetType("System.String"));
                eventTable.Columns.Add("BackColor", Type.GetType("System.String"));

                if (this.Settings.Eventtooltipweek)
                {
                    this.toolTipManager.TargetControls.Clear();
                }

                var dgRow = default(DataRow);
                var objEvent = default(EventInfo);
                foreach (EventInfo tempLoopVar_objEvent in this._selectedEvents)
                {
                    objEvent = tempLoopVar_objEvent;
                    // If full enrollments should be hidden, ignore
                    if (this.HideFullEvent(objEvent))
                    {
                        continue;
                    }

                    if (objEvent.EventTimeEnd > sBegin && objEvent.EventTimeBegin < sEnd)
                    {
                        dgRow = eventTable.NewRow();
                        dgRow["ID"] = objEvent.EventID;
                        dgRow["CreatedByID"] = objEvent.CreatedByID;
                        dgRow["OwnerID"] = objEvent.OwnerID;
                        dgRow["StartTime"] = objEvent.EventTimeBegin;
                        if (!objEvent.AllDayEvent)
                        {
                            dgRow["EndTime"] = objEvent.EventTimeEnd;
                        }
                        else
                        {
                            // all day events are recorded as 23:59
                            dgRow["EndTime"] = objEvent.EventTimeEnd.AddMinutes(1);
                        }
                        //**** Add ModuleName if SubCalendar
                        var imagestring = "";
                        if (this.Settings.Eventimage && this.Settings.EventImageWeek
                            && objEvent.ImageURL != null && objEvent.ImageDisplay)
                        {
                            imagestring = this.ImageInfo(objEvent.ImageURL, objEvent.ImageHeight, objEvent.ImageWidth);
                        }

                        dgRow["BackColor"] = "";
                        var iconString = "";

                        var eventtext = this.CreateEventName(objEvent, this.Settings.Templates.txtWeekEventText);

                        if (!this.IsPrivateNotModerator || this.UserId == objEvent.OwnerID)
                        {
                            var forecolorstr = "";
                            var backcolorstr = "";
                            var blankstr = "";
                            if (objEvent.Color != "")
                            {
                                backcolorstr = "background-color: " + objEvent.Color + ";";
                                blankstr = "&nbsp;";
                                dgRow["BackColor"] = objEvent.Color;
                            }
                            if (objEvent.FontColor != "")
                            {
                                forecolorstr = "color: " + objEvent.FontColor + ";";
                            }
                            dgRow["Task"] = "<span style=\"" + backcolorstr + forecolorstr + "\">" + imagestring +
                                            blankstr + eventtext + blankstr + "</span>";

                            iconString =
                                this.CreateIconString(objEvent, this.Settings.IconWeekPrio, this.Settings.IconWeekRec,
                                                      this.Settings.IconWeekReminder, this.Settings.IconWeekEnroll);

                            // Get detail page url
                            dgRow["URL"] = objEventInfoHelper.DetailPageURL(objEvent);
                            if (objEvent.DetailPage && objEvent.DetailNewWin)
                            {
                                dgRow["Target"] = "_blank";
                            }
                        }
                        else
                        {
                            dgRow["Task"] = imagestring + eventtext;
                        }

                        dgRow["Icons"] = iconString;
                        dgRow["Description"] = objEvent.EventDesc;
                        dgRow["StartDateTime"] = objEvent.EventTimeBegin;
                        dgRow["Duration"] = objEvent.Duration;
                        if (this.Settings.Eventtooltipweek)
                        {
                            var isEvtEditor = this.IsEventEditor(objEvent, false);
                            dgRow["Tooltip"] =
                                this.ToolTipCreate(objEvent, this.Settings.Templates.txtTooltipTemplateTitle,
                                                   this.Settings.Templates.txtTooltipTemplateBody, isEvtEditor);
                        }


                        eventTable.Rows.Add(dgRow);
                    }
                }
                var dvEvent = new DataView(eventTable);

                this.schWeek.StartDate = this._dWeekStart;
                this.schWeek.DateFormatString = this.Settings.Templates.txtWeekTitleDate;
                this.schWeek.Weeks = 1;
                this.schWeek.DataSource = dvEvent;
                this.schWeek.DataBind();
            }
            catch
            { }
        }

        #endregion

        #region Links and Buttons

        protected void lnkNext_Click(object sender, EventArgs e)
        {
            var dDate = Convert.ToDateTime(Convert.ToDateTime(this.ViewState[this.ModuleId + "WeekOf"]).AddDays(7));
            this.SelectedDate = dDate.Date;
            this.dpGoToDate.SelectedDate = dDate.Date;
            this.SelectCategory.StoreCategories();
            this.SelectLocation.StoreLocations();
            this.BindPage(dDate);
        }

        protected void lnkPrev_Click(object sender, EventArgs e)
        {
            var dDate = Convert.ToDateTime(Convert.ToDateTime(this.ViewState[this.ModuleId + "WeekOf"]).AddDays(-7));
            this.SelectedDate = dDate.Date;
            this.dpGoToDate.SelectedDate = dDate.Date;
            this.SelectCategory.StoreCategories();
            this.SelectLocation.StoreLocations();
            this.BindPage(dDate);
        }

        protected void schWeek_ItemDataBound(object sender, ScheduleItemEventArgs e)
        {
            if ((e.Item.ItemType == ScheduleItemType.Item) | (e.Item.ItemType == ScheduleItemType.AlternatingItem))
            {
                var row = (DataRowView) e.Item.DataItem;
                var itemCell = (TableCell) e.Item.Parent;
                if (this.Settings.Eventtooltipweek)
                {
                    var tooltip = Convert.ToString(row["Tooltip"]);
                    itemCell.Attributes.Add("title", tooltip);
                    this.toolTipManager.TargetControls.Add(itemCell.ClientID, true);
                }
                if (this.IsPrivateNotModerator && !(this.UserId == Convert.ToInt32(row["OwnerID"])))
                {
                    itemCell.Style.Add("cursor", "text");
                    itemCell.Style.Add("text-decoration", "none");
                    itemCell.Attributes.Add("onclick", "javascript:return false;");
                }
                var backColor = Convert.ToString(row["BackColor"]);
                if (!string.IsNullOrEmpty(backColor))
                {
                    itemCell.BackColor = this.GetColor(backColor);
                }
            }
        }

        protected void SelectCategory_CategorySelected(object sender, CommandEventArgs e)
        {
            //Store the other selection(s) too.
            this.SelectLocation.StoreLocations();
            var dDate = Convert.ToDateTime(this.ViewState[this.ModuleId + "WeekOf"]);
            this.BindPage(dDate);
        }

        protected void SelectLocation_LocationSelected(object sender, CommandEventArgs e)
        {
            //Store the other selection(s) too.
            this.SelectCategory.StoreCategories();
            var dDate = Convert.ToDateTime(this.ViewState[this.ModuleId + "WeekOf"]);
            this.BindPage(dDate);
        }

        protected void lnkToday_Click(object sender, EventArgs e)
        {
            var dDate = DateTime.Now.Date;
            this.SelectedDate = dDate;
            this.dpGoToDate.SelectedDate = dDate;
            this.SelectCategory.StoreCategories();
            this.SelectLocation.StoreLocations();
            this.BindPage(dDate);
        }

        protected void dpGoToDate_SelectedDateChanged(object sender, SelectedDateChangedEventArgs e)
        {
            var dDate = Convert.ToDateTime(this.dpGoToDate.SelectedDate);
            this.SelectedDate = dDate;
            this.BindPage(dDate);
        }

        #endregion
    }
}