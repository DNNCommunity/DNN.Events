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


namespace DotNetNuke.Modules.Events.ScheduleControl
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    /// -----------------------------------------------------------------------------
    /// Project	 : schedule
    /// Class	 : ScheduleCalendar
    /// 
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     The ScheduleCalendar web control is designed to represent a schedule in a calendar format.
    /// </summary>
    /// -----------------------------------------------------------------------------
    [CLSCompliant(true)]
    [ParseChildren(true)]
    public class ScheduleCalendar : BaseSchedule
    {
        #region Properties

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     Whether to show the EmptyDataTemplate or not when no data is found
        /// </summary>
        /// <remarks>
        ///     Overrides default value (True)
        /// </remarks>
        /// -----------------------------------------------------------------------------
        protected override bool ShowEmptyDataTemplate => !this.FullTimeScale;

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     The database field containing the start time of the events. This field should also contain the date when
        ///     TimeFieldsContainDate=true
        /// </summary>
        /// <remarks>
        ///     StartTimeField replaces DataRangeStartField for ScheduleCalendar
        /// </remarks>
        /// -----------------------------------------------------------------------------
        [Description(
            "The database field containing the start time of the events. This field should also contain the date when TimeFieldsContainDate=true")]
        [Bindable(false)]
        [Category("Data")]
        public string StartTimeField
        {
            get { return base.DataRangeStartField; }
            set { base.DataRangeStartField = value; }
        }

        // Hide DataRangeStartField. For ScheduleCalendar, it's called StartTimeField
        [Browsable(false)]
        [Obsolete("The DataRangeStartField property is obsolete")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new string DataRangeStartField
        {
            get { return base.DataRangeStartField; }
            set { base.DataRangeStartField = value; }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     The database field containing the end time of the events. This field should also contain the date when
        ///     TimeFieldsContainDate=true
        /// </summary>
        /// <remarks>
        ///     EndTimeField replaces DataRangeEndField for ScheduleCalendar
        /// </remarks>
        /// -----------------------------------------------------------------------------
        [Description(
            "The database field containing the end time of the events. This field should also contain the date when TimeFieldsContainDate=true")]
        [Bindable(false)]
        [Category("Data")]
        public string EndTimeField
        {
            get { return base.DataRangeEndField; }
            set { base.DataRangeEndField = value; }
        }

        // Hide DataRangeEndField. For ScheduleCalendar, it's called EndTimeField
        [Browsable(false)]
        [Obsolete("The DataRangeEndField property is obsolete")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new string DataRangeEndField
        {
            get { return base.DataRangeEndField; }
            set { base.DataRangeEndField = value; }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     The database field providing the dates. Ignored when TimeFieldsContainDate=true. When TimeFieldsContainDate=false,
        ///     this field should be of type Date
        /// </summary>
        /// <remarks>
        ///     DateField replaces TitleField for ScheduleCalendar
        /// </remarks>
        /// -----------------------------------------------------------------------------
        [Description(
            "The database field providing the dates. Ignored when TimeFieldsContainDate=true. When TimeFieldsContainDate=false, this field should be of type Date.")]
        [Bindable(false)]
        [Category("Data")]
        public string DateField
        {
            get { return base.TitleField; }
            set { base.TitleField = value; }
        }

        // Hide TitleField. For ScheduleCalendar, it's called DateField
        [Browsable(false)]
        [Obsolete("The TitleField property is obsolete")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new string TitleField
        {
            get { return base.TitleField; }
            set { base.TitleField = value; }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     The first date to display.
        ///     The calendar will start on this date, if not overridden by the
        ///     StartDay and NumberOfDays settings.
        ///     The default value is the date at the time of display.
        /// </summary>
        /// -----------------------------------------------------------------------------
        [Description(
            "The first date to display. The calendar will start on this date, if not overridden by the StartDay and NumberOfDays settings.")]
        [Bindable(true)]
        [Category("Behavior")]
        public DateTime StartDate
        {
            get
                {
                    var o = this.ViewState["StartDate"];
                    if (!ReferenceEquals(o, null))
                    {
                        return Convert.ToDateTime(o);
                    }
                    return DateTime.Today; // default value
                }
            set { this.ViewState["StartDate"] = value; }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     The first day of the week to display.
        ///     The calendar will start on this day of the week.
        ///     This value is used only when NumberOfDays equals 7.
        ///     The default is Monday.
        /// </summary>
        /// -----------------------------------------------------------------------------
        [Description(
            "The first day of the week to display. The calendar will start on this day of the week. This value is used only when NumberOfDays equals 7.")]
        [DefaultValue(DayOfWeek.Monday)]
        [Bindable(true)]
        [Category("Behavior")]
        public DayOfWeek StartDay
        {
            get
                {
                    var o = this.ViewState["StartDay"];
                    if (!ReferenceEquals(o, null))
                    {
                        return (DayOfWeek) o;
                    }
                    return DayOfWeek.Monday; // default value
                }
            set { this.ViewState["StartDay"] = value; }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     The number of days to display.
        ///     This number may be repeated multiple times in Vertical layout when the NumberOfRepetitions
        ///     property is greater than 1.
        ///     De default value is 7 (weekly calendar).
        /// </summary>
        /// -----------------------------------------------------------------------------
        [Description(
            "The number of days to display. This number may be repeated multiple times in Vertical layout when the NumberOfRepetitions property is greater than 1.")]
        [DefaultValue(7)]
        [Bindable(true)]
        [Category("Behavior")]
        public int NumberOfDays
        {
            get
                {
                    var o = this.ViewState["NumberOfDays"];
                    if (!ReferenceEquals(o, null))
                    {
                        return Convert.ToInt32(o);
                    }
                    return 7; // default value
                }
            set { this.ViewState["NumberOfDays"] = value; }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     The number of repetitions to show at a time. Only used in Vertical layout.
        ///     Especially useful if you want to show several weeks in the calendar, one
        ///     below the other.
        ///     This property replaces the Weeks property starting from version 1.6.1.
        /// </summary>
        /// -----------------------------------------------------------------------------
        [Description("The number of repetitions to show at a time. Only used in Vertical layout.")]
        [DefaultValue(1)]
        [Bindable(true)]
        [Category("Behavior")]
        public int NumberOfRepetitions
        {
            get
                {
                    // in horizontal layout, only 1 week is supported
                    if (this.Layout == LayoutEnum.Horizontal)
                    {
                        return 1;
                    }
                    var o = this.ViewState["NumberOfRepetitions"];
                    if (!ReferenceEquals(o, null))
                    {
                        var w = Convert.ToInt32(o);
                        if (w <= 0)
                        {
                            return 1;
                        }
                        return w;
                    }
                    return 1;
                }
            set { this.ViewState["NumberOfRepetitions"] = value; }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     Obsolete since version 1.6.1. Use the NumberOfRepetitions property instead.
        /// </summary>
        /// -----------------------------------------------------------------------------
        [Description("The number of weeks to show at a time. Only used in Vertical layout.")]
        [Browsable(false)]
        [Bindable(true)]
        public int Weeks
        {
            get { return this.NumberOfRepetitions; }
            set { this.NumberOfRepetitions = value; }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     The format used for the date if the DateTemplate is missing, e.g. {0:ddd d}
        /// </summary>
        /// -----------------------------------------------------------------------------
        [Description("The format used for the date if the DateTemplate is missing, e.g. {0:ddd d}")]
        [DefaultValue("")]
        [Category("Data")]
        public string DateFormatString
        {
            get
                {
                    var o = this.ViewState["DateFormatString"];
                    if (!ReferenceEquals(o, null))
                    {
                        return Convert.ToString(o);
                    }
                    return string.Empty;
                }
            set { this.ViewState["DateFormatString"] = value; }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     The format used for the time if the TimeTemplate is missing, e.g. {0:hh:mm}
        /// </summary>
        /// -----------------------------------------------------------------------------
        [Description("The format used for the times if the TimeTemplate is missing, e.g. {0:hh:mm}")]
        [DefaultValue("")]
        [Category("Data")]
        public string TimeFormatString
        {
            get
                {
                    var o = this.ViewState["TimeFormatString"];
                    if (!ReferenceEquals(o, null))
                    {
                        return Convert.ToString(o);
                    }
                    return string.Empty;
                }
            set { this.ViewState["TimeFormatString"] = value; }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     Indicates whether the time fields (StartTimeField and EndTimeField) contain the date as well. Whe true, this allows
        ///     midnight spanning for calendar events. When false, the DateField contains the date.
        /// </summary>
        /// <remarks>
        ///     TimeFieldsContainDate replaces UseTitleFieldAsDate for ScheduleCalendar
        /// </remarks>
        /// -----------------------------------------------------------------------------
        [Description(
            "Indicates whether the time fields (StartTimeField and EndTimeField) contain the date as well. When true, this allows midnight spanning for calendar events. When false, the DateField contains the date.")]
        [DefaultValue(false)]
        [Bindable(true)]
        [Category("Data")]
        public bool TimeFieldsContainDate
        {
            get
                {
                    var o = this.ViewState["TimeFieldsContainDate"];
                    if (!ReferenceEquals(o, null))
                    {
                        return Convert.ToBoolean(o);
                    }
                    return false;
                }
            set { this.ViewState["TimeFieldsContainDate"] = value; }
        }

        #endregion

        #region Styles

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     The style applied to time header items.
        /// </summary>
        /// <remarks>
        ///     TimeStyle replaces RangeHeaderStyle for ScheduleCalendar
        /// </remarks>
        /// -----------------------------------------------------------------------------
        [Description("The style applied to time header items. ")]
        [Bindable(false)]
        [Category("Style")]
        [NotifyParentProperty(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual TableItemStyle TimeStyle => base.RangeHeaderStyle;

        // Hide RangeHeaderStyle. For ScheduleCalendar, it's replaced with TimeStyle
        [Browsable(false)]
        [Obsolete("The RangeHeaderStyle property is obsolete. Use TimeStyle instead")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new TableItemStyle RangeHeaderStyle => base.RangeHeaderStyle;

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     The style applied to date header items.
        /// </summary>
        /// <remarks>
        ///     DateStyle replaces TitleStyle for ScheduleCalendar
        /// </remarks>
        /// -----------------------------------------------------------------------------
        [Description("The style applied to date header items. ")]
        [Bindable(false)]
        [Category("Style")]
        [NotifyParentProperty(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual TableItemStyle DateStyle => base.TitleStyle;

        // Hide TitleStyle. For ScheduleCalendar, it's replaced with DateStyle
        [Browsable(false)]
        [Obsolete("The TitleStyle property is obsolete. Use DateStyle instead.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new TableItemStyle TitleStyle => base.TitleStyle;

        #endregion

        #region Templates

        // DateTemplate replaces TitleTemplate for ScheduleCalendar
        [TemplateContainer(typeof(ScheduleItem))]
        [Browsable(false)]
        [Description("The template used to create date header content.")]
        [NotifyParentProperty(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate DateTemplate { get; set; }

        // TimeTemplate replaces RangeHeaderTemplate for ScheduleCalendar
        [TemplateContainer(typeof(ScheduleItem))]
        [Browsable(false)]
        [Description("The template used to create time header content.")]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate TimeTemplate { get; set; }

        #endregion

        #region Methods

        // Check if all properties are set to make the control work
        public override string CheckConfiguration()
        {
            if (!this.TimeFieldsContainDate && this.DateField == "")
            {
                return
                    "Either the DateField property must have a non blank value, or TimeFieldsContainDate must be true";
            }
            if (this.StartTimeField == "")
            {
                return "The StartTimeField property is not set";
            }
            if (this.EndTimeField == "")
            {
                return "The EndTimeField property is not set";
            }
            return string.Empty;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     create the list with all times (Start or End)
        /// </summary>
        /// <param name="dv"></param>
        /// -----------------------------------------------------------------------------
        public override void FillRangeValueArray(ref DataView dv)
        {
            this.arrRangeValues = new ArrayList();
            if (this.FullTimeScale)
            {
                var tsInc = new TimeSpan(0, this.TimeScaleInterval, 0);
                // ignore data, just fill the time scale
                // add incrementing times to the array
                var t = this.StartOfTimeScale;
                while (TimeSpan.Compare(t, this.EndOfTimeScale) < 0)
                {
                    // use DateTime objects for easy display
                    var dt = new DateTime(1, 1, 1, t.Hours, t.Minutes, 0);
                    this.arrRangeValues.Add(dt);
                    t = t.Add(tsInc);
                }
                // Add the end of the timescale as well to make sure it's there
                // e.g. in the case of EndOfTimeScale=23:59 and TimeScaleInterval=1440, this is imperative
                var dtEnd = new DateTime(1, 1, 1 + this.EndOfTimeScale.Days, this.EndOfTimeScale.Hours,
                                         this.EndOfTimeScale.Minutes, 0);
                this.arrRangeValues.Add(dtEnd);
            }
            else // Not FullTimeScale
            {
                // Just add the times from the dataview
                var j = 0;
                for (j = 0; j <= dv.Count - 1; j++)
                {
                    var t1 = dv[j][this.StartTimeField];
                    var t2 = dv[j][this.EndTimeField];
                    if (!this.TimeFieldsContainDate)
                    {
                        this.arrRangeValues.Add(t1);
                        this.arrRangeValues.Add(t2);
                    }
                    else // TimeFieldsContainDate
                    {
                        // both t1 and t2 should be of type DateTime now
                        if (!(t1 is DateTime))
                        {
                            throw new HttpException(
                                "When TimeFieldsContainDate=True, StartTimeField should be of type DateTime");
                        }
                        var dt1 = Convert.ToDateTime(t1);
                        if (!(t2 is DateTime))
                        {
                            throw new HttpException(
                                "When TimeFieldsContainDate=True, EndTimeField should be of type DateTime");
                        }
                        var dt2 = Convert.ToDateTime(t2);
                        // remove date part, only store time part in array
                        this.arrRangeValues.Add(new DateTime(1, 1, 1, dt1.Hour, dt1.Minute, dt1.Second));
                        if ((dt2.Hour > 0) | (dt2.Minute > 0) | (dt2.Second > 0))
                        {
                            this.arrRangeValues.Add(new DateTime(1, 1, 1, dt2.Hour, dt2.Minute, dt2.Second));
                        }
                        else
                        {
                            // if the end is 0:00:00 hour, insert 24:00:00 hour instead
                            this.arrRangeValues.Add(new DateTime(1, 1, 2, 0, 0, 0));
                        }
                    }
                }
            }

            this.arrRangeValues.Sort();
            RemoveDoubles(this.arrRangeValues);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     When TimeFieldsContainDate=True, items could span over midnight, even several days.
        ///     Split them.
        /// </summary>
        /// <param name="dv"></param>
        /// <remarks>Used to be called "SplitItemsThatSpanMidnight"</remarks>
        /// -----------------------------------------------------------------------------
        public override void PreprocessData(ref DataView dv)
        {
            this.ShiftStartDate();
            if (!this.TimeFieldsContainDate)
            {
                return;
            }
            if (ReferenceEquals(dv, null))
            {
                return; // added in v2.1.0.9
            }
            var j = 0;
            var count = dv.Count;
            while (j < count)
            {
                var drv = dv[j];
                var dtStartValue = Convert.ToDateTime(drv[this.StartTimeField]);
                var dtEndValue = Convert.ToDateTime(drv[this.EndTimeField]);
                var dateStart = new DateTime(dtStartValue.Year, dtStartValue.Month, dtStartValue.Day);
                var dateEnd = new DateTime(dtEndValue.Year, dtEndValue.Month, dtEndValue.Day);
                if ((dtEndValue.Hour == 0) & (dtEndValue.Minute == 0) & (dtEndValue.Second == 0))
                {
                    // when it ends at 0:00:00 hour, it's representing 24:00 hours of the previous day
                    dateEnd = dateEnd.AddDays(-1);
                }
                // Check if the item spans midnight. If so, split it.
                if (dateStart < dateEnd)
                {
                    // the item spans midnight. We'll truncate the item first, so that it only
                    // covers the last day, and we'll add new items for the other day(s) in the loop below.
                    if (this.FullTimeScale)
                    {
                        // truncate the item by setting its start time to StartOfTimeScale
                        drv[this.StartTimeField] =
                            new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, this.StartOfTimeScale.Hours,
                                         this.StartOfTimeScale.Minutes, this.StartOfTimeScale.Seconds);
                    }
                    else
                    {
                        // truncate the item by setting its start time to 0:00:00 hours
                        drv[this.StartTimeField] = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 0, 0, 0);
                    }
                }
                while (dateStart < dateEnd)
                {
                    // If the item spans midnight once, create an additional item for the first day.
                    // If it spans midnight several times, create additional items for each day.
                    var drvNew = dv.AddNew();
                    var i = 0;
                    for (i = 0; i <= dv.Table.Columns.Count - 1; i++)
                    {
                        drvNew[i] = drv[i]; // copy columns one by one
                    }
                    drvNew[this.StartTimeField] = dtStartValue;
                    if (this.FullTimeScale)
                    {
                        // set the end time to the EndOfTimeScale value
                        var dateEnd2 = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day,
                                                    this.EndOfTimeScale.Hours, this.EndOfTimeScale.Minutes,
                                                    this.EndOfTimeScale.Seconds);
                        if (this.EndOfTimeScale.Equals(new TimeSpan(1, 0, 0, 0)))
                        {
                            // EndOfTimeScale is 24:00 hours. Set the end at 0:00 AM of the next day.
                            // We'll catch this case later and show the proper value.
                            dateEnd2.AddDays(1);
                        }
                        drvNew[this.EndTimeField] = dateEnd2;
                    }
                    else
                    {
                        // Set the end time to 24:00 hours. This is 0:00 AM of the next day.
                        // We'll catch this case later and show the proper value.
                        drvNew[this.EndTimeField] =
                            new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0).AddDays(1);
                    }
                    drvNew.EndEdit();
                    dateStart = dateStart.AddDays(1);
                    if (this.FullTimeScale)
                    {
                        // next item should start at StartOfTimeScale
                        dtStartValue = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day,
                                                    this.StartOfTimeScale.Hours, this.StartOfTimeScale.Minutes,
                                                    this.StartOfTimeScale.Seconds);
                    }
                    else
                    {
                        // next item should start at 0:00:00 hour
                        dtStartValue = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0);
                    }
                }
                j++;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     calculate the TitleIndex in the table, given the objTitleValue
        /// </summary>
        /// <param name="objTitleValue"></param>
        /// <returns></returns>
        /// -----------------------------------------------------------------------------
        protected override int CalculateTitleIndex(object objTitleValue)
        {
            var returnValue = 0;
            returnValue = -1;
            if (!(objTitleValue is DateTime) && !(objTitleValue is DateTime))
            {
                throw new HttpException("DateField should be of type Date or DateTime in Calendar mode");
            }
            var dtDate = Convert.ToDateTime(objTitleValue);
            // remove time part, if any
            dtDate = new DateTime(dtDate.Year, dtDate.Month, dtDate.Day, 0, 0, 0);
            returnValue =
                Convert.ToInt32(dtDate.Subtract(this.StartDate.Date).Days % this.NumberOfDays +
                                1); // fix courtesy of Anthony Main
            return returnValue;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     Calculate the range cell index in the table, given the objRangeValue and the objTitleValue
        ///     The values refer to the real cell index in the table, taking into account whether cells are
        ///     spanning over value marks (in horizontal mode)
        ///     In vertical layout, the result is the real row index in the table
        ///     In horizontal layout, the result is the real cell index in the row (before any merging
        ///     of cells due to value spanning)
        /// </summary>
        /// <param name="objRangeValue">The range value from the data source</param>
        /// <param name="objTitleValue">The title value from the data source</param>
        /// <param name="isEndValue">
        ///     False if we're calculating the range value index for the start of the item, True if it's the
        ///     end
        /// </param>
        /// <returns>The range cell index</returns>
        /// -----------------------------------------------------------------------------
        protected override int CalculateRangeCellIndex(object objRangeValue, object objTitleValue, bool isEndValue)
        {
            var returnValue = 0;
            var RangeValueIndex = -1;
            // Find range value index by matching with range values array
            if (this.FullTimeScale && !(objRangeValue is DateTime))
            {
                throw new HttpException("The time field should be of type DateTime when FullTimeScale is set to true");
            }
            if (this.TimeFieldsContainDate && !(objRangeValue is DateTime))
            {
                throw new HttpException(
                    "The time field should be of type DateTime when TimeFieldsContainDate is set to true");
            }
            var k = 0;
            if (this.FullTimeScale)
            {
                var Dobj = Convert.ToDateTime(objRangeValue);
                // omit the date part for comparison
                Dobj = new DateTime(1, 1, 1, Dobj.Hour, Dobj.Minute, Dobj.Second);
                // if no match is found, use the index of the EndOfTimeScale value
                RangeValueIndex = this.arrRangeValues.Count;
                for (k = 0; k <= this.arrRangeValues.Count - 1; k++)
                {
                    var Dk = Convert.ToDateTime(this.arrRangeValues[k]);
                    Dk = new DateTime(1, 1, 1, Dk.Hour, Dk.Minute, Dk.Second); // omit date part
                    if (Dobj < Dk && k == 0 && isEndValue)
                    {
                        // ends before start of time scale
                        return -1;
                    }
                    if (Dobj >= Dk && k == this.arrRangeValues.Count - 1 && !isEndValue)
                    {
                        // starts at or after end of time scale
                        return -1;
                    }
                    if (Dobj <= Dk)
                    {
                        if (k == 0 && isEndValue)
                        {
                            // This can happen when the end value is 24:00:00, which will
                            // match with the value 0:00:00 and give k=0
                            // Instead of the value k=0, use k=arrRangeValues.Count-1
                            RangeValueIndex = this.arrRangeValues.Count;
                        }
                        else
                        {
                            RangeValueIndex = k + 1;
                        }
                        break;
                    }
                }
            }
            else // Not FullTimeScale
            {
                if (!this.TimeFieldsContainDate)
                {
                    // find the matching value in arrRangeValues
                    for (k = 0; k <= this.arrRangeValues.Count - 1; k++)
                    {
                        if (this.arrRangeValues[k].ToString() == objRangeValue.ToString())
                        {
                            RangeValueIndex = k + 1;
                            break;
                        }
                    }
                }
                else
                {
                    // TimeFieldsContainDate=True
                    var Dobj = Convert.ToDateTime(objRangeValue);
                    // omit the date part for comparison
                    Dobj = new DateTime(1, 1, 1, Dobj.Hour, Dobj.Minute, Dobj.Second);
                    for (k = 0; k <= this.arrRangeValues.Count - 1; k++)
                    {
                        var Dk = Convert.ToDateTime(this.arrRangeValues[k]);
                        Dk = new DateTime(1, 1, 1, Dk.Hour, Dk.Minute, Dk.Second); // omit date part
                        if (Dobj == Dk)
                        {
                            if (k == 0 && isEndValue)
                            {
                                // This can happen when the end value is 24:00:00, which will
                                // match with the value 0:00:00, and give k=0
                                // Instead of the value k=0, use k=arrRangeValues.Count-1
                                RangeValueIndex = this.arrRangeValues.Count;
                            }
                            else
                            {
                                RangeValueIndex = k + 1;
                            }
                            break;
                        }
                    }
                }
            }

            if (!this.IncludeEndValue && this.ShowValueMarks)
            {
                if (this.Layout == LayoutEnum.Vertical)
                {
                    // Each item spans two rows
                    returnValue = RangeValueIndex * 2;
                }
                else
                {
                    // Add one cell for the empty background cell on the left
                    returnValue = RangeValueIndex + 1;
                }
            }
            else
            {
                returnValue = RangeValueIndex;
            }

            // The valueindex that we found corresponds with an item in the first week.
            // If the item is in another week, modify the valueindex accordingly.
            // To find out, check the date of the item
            var dtDate = default(DateTime);
            if (!this.TimeFieldsContainDate)
            {
                // use objTitleValue for the date
                if (!(objTitleValue is DateTime) && !(objTitleValue is DateTime))
                {
                    throw new HttpException(
                        "The date field should be of type Date or DateTime in Calendar mode when TimeFieldsContainDate=false.");
                }
                dtDate = Convert.ToDateTime(objTitleValue);
            }
            else
            {
                // use objRangeValue for the date
                var Dobj = Convert.ToDateTime(objRangeValue);
                dtDate = new DateTime(Dobj.Year, Dobj.Month, Dobj.Day);
                if (isEndValue && (Dobj.Hour == 0) & (Dobj.Minute == 0) & (Dobj.Second == 0))
                {
                    // when it's the end of the item and the time = 0:00 hours,
                    // it's representing 24:00 hours of the previous day
                    dtDate = dtDate.AddDays(-1);
                }
            }
            // if dtDate is more than NumberOfDays after StartDate, add additional rows
            var rowsPerWeek = 1 + this.arrRangeValues.Count;
            if (!this.IncludeEndValue && this.ShowValueMarks)
            {
                rowsPerWeek = 1 + this.arrRangeValues.Count * 2;
            }

            returnValue = Convert.ToInt32(returnValue +
                                          dtDate.Subtract(this.StartDate).Days / this.NumberOfDays * rowsPerWeek);
            return returnValue;
        }

        protected override int GetTitleCount()
        {
            return this.NumberOfDays; // make a title cell for every NumberOfDays
        }

        public override void AddTitleHeaderData()
        {
            var nTitles = this.GetTitleCount();

            // iterate arrTitleValues creating a new item for each data item
            var titleIndex = 0;
            for (titleIndex = 1; titleIndex <= nTitles; titleIndex++)
            {
                var iWeek = 0;
                for (iWeek = 0; iWeek <= this.NumberOfRepetitions - 1; iWeek++)
                {
                    object obj = this.StartDate.AddDays(titleIndex - 1 + iWeek * this.NumberOfDays);
                    var rowsPerWeek = 1 + this.arrRangeValues.Count;
                    if (!this.IncludeEndValue && this.ShowValueMarks)
                    {
                        rowsPerWeek = 1 + this.arrRangeValues.Count * 2;
                    }
                    var rangeIndex = iWeek * rowsPerWeek;
                    this.CreateItem(rangeIndex, titleIndex, ScheduleItemType.TitleHeader, true, obj, -1);
                }
            }
        }

        protected override string GetTitleField()
        {
            if (this.TimeFieldsContainDate)
            {
                // When TimeFieldsContainDate=true, use StartTimeField as Title
                return this.StartTimeField;
            }
            return this.DateField;
        }

        public void ShiftStartDate()
        {
            if (this.NumberOfDays != 7)
            {
                return; // change the start date only for a weekly calendar
            }
            // for any StartDate set by the user, shift it to the previous day indicated by the StartDay property
            // (by default, this will be the previous Monday)
            this.StartDate = this.StartDate.AddDays(Convert.ToInt32(this.StartDay) -
                                                    Convert.ToInt32(this.StartDate.DayOfWeek));
            if (Convert.ToInt32(this.StartDay) > Convert.ToInt32(this.StartDate.DayOfWeek))
            {
                this.StartDate = this.StartDate.AddDays(-7);
            }
            // StartDate should be on the day of the week indicated by StartDay now
        }

        public override string GetSortOrder()
        {
            // make sure the data is processed in the right order: from bottom right up to top left.
            if (!this.TimeFieldsContainDate)
            {
                return this.DateField + " ASC, " + this.StartTimeField + " ASC, " + this.EndTimeField + " ASC";
            }
            // In Calendar Mode with TimeFieldsContainDate=True, there is no DateField
            return this.StartTimeField + " ASC, " + this.EndTimeField + " ASC";
        }

        public override int GetRepetitionCount()
        {
            return this.NumberOfRepetitions;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     when there's no template, try to present the data in a reasonable format
        /// </summary>
        /// <param name="value">Value of the item</param>
        /// <param name="type">Type of the item</param>
        /// <returns>A formatted string</returns>
        /// -----------------------------------------------------------------------------
        protected override string FormatDataValue(object value, ScheduleItemType type)
        {
            var retVal = string.Empty;
            if (!ReferenceEquals(value, null))
            {
                if (type == ScheduleItemType.TitleHeader)
                {
                    if (this.DateFormatString.Length == 0)
                    {
                        retVal = value.ToString();
                    }
                    else
                    {
                        retVal = string.Format(this.DateFormatString, value);
                    }
                }
                else if (type == ScheduleItemType.RangeHeader)
                {
                    if (this.TimeFormatString.Length == 0)
                    {
                        retVal = value.ToString();
                    }
                    else
                    {
                        retVal = string.Format(this.TimeFormatString, value);
                    }
                }
                else
                {
                    retVal = value.ToString();
                }
            }
            return retVal;
        }

        protected override ITemplate GetTemplate(ScheduleItemType type)
        {
            switch (type)
            {
                case ScheduleItemType.RangeHeader:
                    return this.TimeTemplate;
                case ScheduleItemType.TitleHeader:
                    return this.DateTemplate;
                case ScheduleItemType.Item:
                    return this.ItemTemplate;
                case ScheduleItemType.AlternatingItem:
                    return this.ItemTemplate;
            }
            return null;
        }

        /// ' -----------------------------------------------------------------------------
        /// '
        /// <summary>
        ///     ' Calculate the title value given the cell index
        ///     '
        /// </summary>
        /// '
        /// <param name="titleIndex">Title index of the cell</param>
        /// '
        /// <returns>Object containing the title</returns>
        /// ' -----------------------------------------------------------------------------
        public override dynamic CalculateTitle(int titleIndex, int cellIndex)
        {
            var cellsPerWeek = 0;
            if (!this.IncludeEndValue && this.ShowValueMarks)
            {
                cellsPerWeek = this.arrRangeValues.Count * 2 + 1;
            }
            else
            {
                cellsPerWeek = this.arrRangeValues.Count + 1;
            }
            var week = cellIndex / cellsPerWeek;
            return this.StartDate.AddDays(titleIndex - 1 + week * 7);
        }

        #endregion
    }
}