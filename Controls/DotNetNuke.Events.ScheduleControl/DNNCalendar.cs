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
    namespace MonthControl
    {
        using System;
        using System.Collections;
        using System.ComponentModel;
        using System.Drawing;
        using System.Globalization;
        using System.Text;
        using System.Web;
        using System.Web.UI;
        using System.Web.UI.HtmlControls;
        using System.Web.UI.WebControls;
        using Calendar = System.Web.UI.WebControls.Calendar;

        /// <summary>
        ///     Enumerator for Days of Week
        /// </summary>
        [Flags]
        public enum MyDayOfWeek
        {
            /// <summary>
            ///     Monday
            /// </summary>
            Monday = 1,

            /// <summary>
            ///     Tuesday
            /// </summary>
            Tuesday = 2,

            /// <summary>
            ///     Wednesday
            /// </summary>
            Wednesday = 4,

            /// <summary>
            ///     Thursday
            /// </summary>
            Thursday = 8,

            /// <summary>
            ///     Friday
            /// </summary>
            Friday = 16,

            /// <summary>
            ///     Saturday
            /// </summary>
            Saturday = 32,

            /// <summary>
            ///     Sunday
            /// </summary>
            Sunday = 64
        }

        /// <summary>
        ///     Fixes Web Calendar Control Style Issues
        /// </summary>
        [CLSCompliant(true)]
        [DefaultEvent("SelectionChanged")]
        [ToolboxBitmap(typeof(DNNCalendar), "DNNCalendar.bmp")]
        [ToolboxData("<{0}:DNNCalendar runat=\"server\"></{0}:DNNCalendar>")]
        public class DNNCalendar : Calendar
        {
            /// <summary>
            ///     Set WeekEnd days
            /// </summary>
            [Bindable(true)]
            [Browsable(true)]
            [Category("Appearance")]
            [Description("What days are considered Weekend Days")]
            [DefaultValue(MyDayOfWeek.Saturday | MyDayOfWeek.Sunday)]
            public MyDayOfWeek WeekEndDays
            {
                get
                    {
                        var wed = (MyDayOfWeek) ((int) MyDayOfWeek.Saturday | (int) MyDayOfWeek.Sunday);
                        var obj = this.ViewState["wed"];
                        if (obj != null)
                        {
                            wed = (MyDayOfWeek) obj;
                        }
                        return wed;
                    }
                set { this.ViewState["wed"] = value; }
            }

            // Mapping of built-in DayOfWeek and our custom MyDayOfWeek
            private MyDayOfWeek DayOfWeekMapping(DayOfWeek orig)
            {
                var ret = default(MyDayOfWeek);
                switch (orig)
                {
                    case DayOfWeek.Monday:
                        ret = MyDayOfWeek.Monday;
                        break;
                    case DayOfWeek.Tuesday:
                        ret = MyDayOfWeek.Tuesday;
                        break;
                    case DayOfWeek.Wednesday:
                        ret = MyDayOfWeek.Wednesday;
                        break;
                    case DayOfWeek.Thursday:
                        ret = MyDayOfWeek.Thursday;
                        break;
                    case DayOfWeek.Friday:
                        ret = MyDayOfWeek.Friday;
                        break;
                    case DayOfWeek.Saturday:
                        ret = MyDayOfWeek.Saturday;
                        break;
                    case DayOfWeek.Sunday:
                        ret = MyDayOfWeek.Sunday;
                        break;
                    default:
                        ret = MyDayOfWeek.Saturday;
                        break;
                }

                return ret;
            }

            #region Private properties

            // Gets the date that specifies the month to be displayed. This will
            // be VisibleDate unless that property is defaulted to
            // DateTime.MinValue, in which case TodaysDate is returned instead.
            private DateTime TargetDate
            {
                get
                    {
                        if (this.VisibleDate == DateTime.MinValue)
                        {
                            return this.TodaysDate;
                        }
                        return this.VisibleDate;
                    }
            }

            // This is the date used for creating day count values, i.e., the
            // number of days between some date and this one. These values are
            // used to create post back event arguments identical to those used
            // by the base Calendar class.
            private static readonly DateTime DayCountBaseDate = new DateTime(2000, 1, 1);

            #endregion

            #region Control rendering

            /// <summary>
            ///     This member overrides <see cref="System.Web.UI.Control.Render" />.
            /// </summary>
            protected override void Render(HtmlTextWriter output)
            {
                // Create the main table.
                var table = new Table();
                table.CellPadding = this.CellPadding;
                table.CellSpacing = this.CellSpacing;

                if (this.ShowGridLines)
                {
                    table.GridLines = GridLines.Both;
                }
                else
                {
                    table.GridLines = GridLines.None;
                }

                // If ShowTitle is true, add a row with the calendar title.
                if (this.ShowTitle)
                {
                    // Create a one-cell table row.
                    var row = new TableRow();
                    var cell = new TableCell();
                    if (this.HasWeekSelectors(this.SelectionMode))
                    {
                        cell.ColumnSpan = 8;
                    }
                    else
                    {
                        cell.ColumnSpan = 7;
                    }

                    // Apply styling.
                    cell.MergeStyle(this.TitleStyle);

                    // Add the title table to the cell.
                    cell.Controls.Add(this.TitleTable());
                    row.Cells.Add(cell);

                    // Add it to the table.
                    table.Rows.Add(row);
                }

                // If ShowDayHeader is true, add a row with the days header.
                if (this.ShowDayHeader)
                {
                    table.Rows.Add(this.DaysHeaderTableRow());
                }

                // Find the first date that will be visible on the calendar.
                var date = this.GetFirstCalendarDate();

                // Create a list for storing nonselectable dates.
                var nonselectableDates = new ArrayList();

                // Add rows for the dates (six rows are always displayed).
                for (var i = 0; i <= 5; i++)
                {
                    var row = new TableRow();

                    // Create a week selector, if needed.
                    if (this.HasWeekSelectors(this.SelectionMode))
                    {
                        var cell = new TableCell();
                        cell.HorizontalAlign = HorizontalAlign.Center;
                        cell.MergeStyle(this.SelectorStyle);

                        if (this.Enabled)
                        {
                            // Create the post back link.
                            var anchor = new HtmlAnchor();
                            var arg = string.Format("R{0}07", this.DayCountFromDate(date));
                            anchor.HRef = this.Page.ClientScript.GetPostBackClientHyperlink(this, arg);
                            anchor.Controls.Add(new LiteralControl(this.SelectWeekText));

                            // Add a color style to the anchor if it is explicitly
                            // set.
                            if (!this.SelectorStyle.ForeColor.IsEmpty)
                            {
                                anchor.Attributes.Add(
                                    "style", string.Format("color:{0}", this.SelectorStyle.ForeColor.Name));
                            }

                            cell.Controls.Add(anchor);
                        }
                        else
                        {
                            cell.Controls.Add(new LiteralControl(this.SelectWeekText));
                        }

                        row.Cells.Add(cell);
                    }

                    // Add the days (there are always seven days per row).
                    for (var j = 0; j <= 6; j++)
                    {
                        // Create a CalendarDay and a TableCell for the date.
                        var day = this.Day(date);
                        var cell = this.Cell(day);

                        // Raise the OnDayRender event.
                        this.OnDayRender(cell, day);

                        // If the day was marked nonselectable, add it to the list.
                        if (!day.IsSelectable)
                        {
                            nonselectableDates.Add(day.Date.ToShortDateString());
                        }

                        // If the day is selectable, and the selection mode allows
                        // it, convert the text to a link with post back.
                        if (this.Enabled && day.IsSelectable && this.SelectionMode != CalendarSelectionMode.None)
                        {
                            try
                            {
                                // Create the post back link.
                                var anchor = new HtmlAnchor();
                                var arg = Convert.ToString(this.DayCountFromDate(date).ToString());
                                anchor.HRef = this.Page.ClientScript.GetPostBackClientHyperlink(this, arg);

                                // Copy the existing text.
                                anchor.Controls.Add(new LiteralControl(((LiteralControl) cell.Controls[0]).Text));

                                // Add a color style to the anchor if it is
                                // explicitly set. Note that the style precedence
                                // follows that of the base Calendar control.
                                var s = "";
                                if (!this.DayStyle.ForeColor.IsEmpty)
                                {
                                    s = this.DayStyle.ForeColor.Name;
                                }
                                var currdayofweek = this.DayOfWeekMapping(day.Date.DayOfWeek);
                                if ((this.WeekEndDays & currdayofweek) == currdayofweek &&
                                    !this.WeekendDayStyle.ForeColor.IsEmpty)
                                {
                                    s = this.WeekendDayStyle.ForeColor.Name;
                                }
                                if (day.IsOtherMonth && !this.OtherMonthDayStyle.ForeColor.IsEmpty)
                                {
                                    s = this.OtherMonthDayStyle.ForeColor.Name;
                                }
                                if (day.IsToday && !this.TodayDayStyle.ForeColor.IsEmpty)
                                {
                                    s = this.TodayDayStyle.ForeColor.Name;
                                }
                                if (this.SelectedDates.Contains(day.Date) && !this.SelectedDayStyle.ForeColor.IsEmpty)
                                {
                                    s = this.SelectedDayStyle.ForeColor.Name;
                                }
                                if (s.Length > 0)
                                {
                                    anchor.Attributes.Add("style", string.Format("color:{0}", s));
                                }

                                // Replace the literal control in the cell with
                                // the anchor.
                                cell.Controls.RemoveAt(0);
                                cell.Controls.AddAt(0, anchor);
                            }
                            catch (Exception)
                            { }
                        }

                        // Add the cell to the current table row.
                        row.Cells.Add(cell);

                        // Bump the date.
                        date = date.AddDays(1);
                    }

                    // Add the row.
                    table.Rows.Add(row);
                }

                // Save the list of nonselectable dates.
                if (nonselectableDates.Count > 0)
                {
                    this.SaveNonselectableDates(nonselectableDates);
                }

                // Apply styling.
                this.AddAttributesToRender(output);

                // Render the table.
                table.RenderControl(output);
            }

            // ====================================================================
            // Helper functions for rendering the control.
            // ====================================================================

            //
            // Generates a Table control for the calendar title.
            //
            private Table TitleTable()
            {
                // Create a table row.
                var row = new TableRow();
                var cell = default(TableCell);
                var anchor = default(HtmlAnchor);
                var date = default(DateTime);
                var text = "";

                // Add a table cell with the previous month.
                if (this.ShowNextPrevMonth)
                {
                    cell = new TableCell();
                    cell.MergeStyle(this.NextPrevStyle);
                    cell.Style.Add("width", "15%");

                    // Find the first of the previous month, needed for post back
                    // processing.
                    try
                    {
                        date = new DateTime(this.TargetDate.Year, this.TargetDate.Month, 1).AddMonths(-1);
                    }
                    catch (Exception)
                    {
                        date = this.TargetDate;
                    }

                    // Get the previous month text.
                    if (this.NextPrevFormat == NextPrevFormat.CustomText)
                    {
                        text = this.PrevMonthText;
                    }
                    else
                    {
                        if (this.NextPrevFormat == NextPrevFormat.ShortMonth)
                        {
                            text = date.ToString("MMM");
                        }
                        else
                        {
                            text = date.ToString("MMMM");
                        }
                    }

                    if (this.Enabled)
                    {
                        // Create the post back link.
                        anchor = new HtmlAnchor();
                        var arg = string.Format("V{0}", this.DayCountFromDate(date));
                        anchor.HRef = this.Page.ClientScript.GetPostBackClientHyperlink(this, arg);
                        anchor.Controls.Add(new LiteralControl(text));

                        // Add a color style to the anchor if it is explicitly
                        // set.
                        if (!this.NextPrevStyle.ForeColor.IsEmpty)
                        {
                            anchor.Attributes.Add(
                                "style", string.Format("color:{0}", this.NextPrevStyle.ForeColor.Name));
                        }

                        // Add the link to the cell.
                        // }
                        cell.Controls.Add(anchor);
                    }
                    else
                    {
                        cell.Controls.Add(new LiteralControl(text));
                    }

                    row.Cells.Add(cell);
                }

                // Add a table cell for the title text.
                cell = new TableCell();
                cell.HorizontalAlign = HorizontalAlign.Center;
                cell.VerticalAlign = VerticalAlign.Middle;
                if (this.ShowNextPrevMonth)
                {
                    cell.Style.Add("width", "70%");
                }
                if (this.TitleFormat == TitleFormat.Month)
                {
                    cell.Text = this.TargetDate.ToString("MMMM");
                }
                else
                {
                    cell.Text = Convert.ToString(this.TargetDate.ToString("y").Replace(", ", " "));
                }
                row.Cells.Add(cell);

                // Add a table cell for the next month.
                if (this.ShowNextPrevMonth)
                {
                    cell = new TableCell();
                    cell.HorizontalAlign = HorizontalAlign.Right;
                    cell.MergeStyle(this.NextPrevStyle);
                    cell.Style.Add("width", "15%");

                    // Find the first of the next month, needed for post back
                    // processing.
                    try
                    {
                        date = new DateTime(this.TargetDate.Year, this.TargetDate.Month, 1).AddMonths(1);
                    }
                    catch (Exception)
                    {
                        date = this.TargetDate;
                    }

                    // Get the next month text.
                    if (this.NextPrevFormat == NextPrevFormat.CustomText)
                    {
                        text = this.NextMonthText;
                    }
                    else
                    {
                        if (this.NextPrevFormat == NextPrevFormat.ShortMonth)
                        {
                            text = date.ToString("MMM");
                        }
                        else
                        {
                            text = date.ToString("MMMM");
                        }
                    }

                    if (this.Enabled)
                    {
                        // Create the post back link.
                        anchor = new HtmlAnchor();
                        var arg = string.Format("V{0}", this.DayCountFromDate(date));
                        anchor.HRef = this.Page.ClientScript.GetPostBackClientHyperlink(this, arg);
                        anchor.Controls.Add(new LiteralControl(text));

                        // Add a color style to the anchor if it is explicitly
                        // set.
                        if (!this.NextPrevStyle.ForeColor.IsEmpty)
                        {
                            anchor.Attributes.Add(
                                "style", string.Format("color:{0}", this.NextPrevStyle.ForeColor.Name));
                        }

                        // Add the link to the cell.
                        // }
                        cell.Controls.Add(anchor);
                    }
                    else
                    {
                        cell.Controls.Add(new LiteralControl(text));
                    }

                    row.Cells.Add(cell);
                }

                // Create the table and add the title row to it.
                var table = new Table();
                table.CellPadding = 0;
                table.CellSpacing = 0;
                table.Attributes.Add("style", "width:100%;height:100%");
                table.Rows.Add(row);

                return table;
            }

            //
            // Generates a TableRow control for the calendar days header.
            //
            private TableRow DaysHeaderTableRow()
            {
                // Create the table row.
                var row = new TableRow();

                // Create an array of days.
                DayOfWeek[] days =
                    {
                        DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                        DayOfWeek.Thursday, DayOfWeek.Friday,
                        DayOfWeek.Saturday
                    };

                // Adjust the array to get the specified starting day at the first index.
                var first = this.GetFirstDayOfWeek();
                while (days[0] != first)
                {
                    var temp = days[0];
                    for (var i = 0; i <= days.Length - 2; i++)
                    {
                        days[i] = days[i + 1];
                    }
                    days[days.Length - 1] = temp;
                }

                // Add a month selector column, if needed.
                if (this.HasWeekSelectors(this.SelectionMode))
                {
                    var cell = new TableCell();
                    cell.HorizontalAlign = HorizontalAlign.Center;

                    // If months are selectable, create the selector.
                    if (this.SelectionMode == CalendarSelectionMode.DayWeekMonth)
                    {
                        // Find the first of the month.
                        var date = new DateTime(this.TargetDate.Year, this.TargetDate.Month, 1);

                        // Use the selector style.
                        cell.MergeStyle(this.SelectorStyle);

                        // Create the post back link.
                        if (this.Enabled)
                        {
                            var anchor = new HtmlAnchor();
                            var arg = string.Format("R{0}{1}", this.DayCountFromDate(date),
                                                    DateTime.DaysInMonth(date.Year, date.Month));
                            anchor.HRef = this.Page.ClientScript.GetPostBackClientHyperlink(this, arg);
                            anchor.Controls.Add(new LiteralControl(this.SelectMonthText));

                            // Add a color style to the anchor if it is explicitly
                            // set.
                            if (!this.SelectorStyle.ForeColor.IsEmpty)
                            {
                                anchor.Attributes.Add(
                                    "style", string.Format("color:{0}", this.SelectorStyle.ForeColor.Name));
                            }

                            cell.Controls.Add(anchor);
                        }
                        else
                        {
                            cell.Controls.Add(new LiteralControl(this.SelectMonthText));
                        }
                    }
                    else
                    {
                        // Use the day header style.
                        cell.CssClass = this.DayHeaderStyle.CssClass;
                    }

                    row.Cells.Add(cell);
                }

                // Add the day names to the header.
                foreach (var day in days)
                {
                    row.Cells.Add(this.DayHeaderTableCell(day));
                }

                return row;
            }

            //
            // Returns a table cell containing a day name for the calendar day
            // header.
            //
            private TableCell DayHeaderTableCell(DayOfWeek dayOfWeek)
            {
                // Generate the day name text based on the specified format.
                var s = "";
                if (this.DayNameFormat == DayNameFormat.Short)
                {
                    s = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[Convert.ToInt32(dayOfWeek)];
                }
                else
                {
                    s = CultureInfo.CurrentCulture.DateTimeFormat.DayNames[Convert.ToInt32(dayOfWeek)];
                    if (this.DayNameFormat == DayNameFormat.FirstTwoLetters)
                    {
                        s = s.Substring(0, 2);
                    }
                    if (this.DayNameFormat == DayNameFormat.FirstLetter)
                    {
                        s = s.Substring(0, 1);
                    }
                }

                // Create the cell, set the style and the text.
                var cell = new TableCell();
                cell.HorizontalAlign = HorizontalAlign.Center;
                cell.MergeStyle(this.DayHeaderStyle);
                cell.Text = s;

                return cell;
            }

            //
            // Determines the first day of the week based on the FirstDayOfWeek
            // property setting.
            //
            private DayOfWeek GetFirstDayOfWeek()
            {
                // If the default value is specifed, use the system default.
                if (this.FirstDayOfWeek == FirstDayOfWeek.Default)
                {
                    return CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
                }
                return (DayOfWeek) this.FirstDayOfWeek;
            }

            //
            // Returns the date that should appear in the first day cell of the
            // calendar display.
            //
            private DateTime GetFirstCalendarDate()
            {
                // Start with the first of the month.
                var date = new DateTime(this.TargetDate.Year, this.TargetDate.Month, 1);

                // While that day does not fall on the first day of the week, move back.
                var firstDay = this.GetFirstDayOfWeek();
                while (date.DayOfWeek != firstDay)
                {
                    date = date.AddDays(-1);
                }

                return date;
            }

            //
            // Creates a CalendarDay instance for the given date.
            //
            // This object is included in the DayRenderEventArgs passed to
            // the DayRender event handler.
            //
            private CalendarDay Day(DateTime date)
            {
                var calday =
                    new CalendarDay(date, date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday,
                                    date == this.TodaysDate, date == this.SelectedDate,
                                    !(date.Month == this.TargetDate.Month && date.Year == this.TargetDate.Year),
                                    date.Day.ToString());

                // Default the day to selectable.
                calday.IsSelectable = true;

                return calday;
            }

            //
            // Creates a TableCell control for the given calendar day.
            //
            // Note: This object is included in the DayRenderEventArgs passed to
            // the DayRender event handler.
            //
            private TableCell Cell(CalendarDay day)
            {
                var tbcell = new TableCell();
                tbcell.HorizontalAlign = HorizontalAlign.Center;

                // Add styling based on day flags.
                // - Styles are applied per the precedence order used by the
                // base Calendar control.
                // - For CssClass, multiple class names may be added.
                var sb = new StringBuilder();
                var currdayofweek = this.DayOfWeekMapping(day.Date.DayOfWeek);
                if ((this.WeekEndDays & currdayofweek) == currdayofweek && !day.IsOtherMonth &&
                    !ReferenceEquals(this.WeekendDayStyle, null))
                {
                    tbcell.MergeStyle(this.WeekendDayStyle);
                    sb.AppendFormat(" {0}", this.WeekendDayStyle.CssClass);
                }
                if (day.IsOtherMonth)
                {
                    if (day.IsWeekend)
                    {
                        tbcell.ApplyStyle(this.OtherMonthDayStyle);
                        sb = new StringBuilder();
                        sb.AppendFormat(" {0}", this.OtherMonthDayStyle.CssClass);
                    }
                    else
                    {
                        tbcell.MergeStyle(this.OtherMonthDayStyle);
                        sb.AppendFormat(" {0}", this.OtherMonthDayStyle.CssClass);
                    }
                }

                if (this.SelectedDates.Contains(day.Date))
                {
                    tbcell.MergeStyle(this.SelectedDayStyle);
                    sb.AppendFormat(" {0}", this.SelectedDayStyle.CssClass);
                }
                if (day.IsToday)
                {
                    tbcell.MergeStyle(this.TodayDayStyle);
                    sb.AppendFormat(" {0}", this.TodayDayStyle.CssClass);
                }

                tbcell.MergeStyle(this.DayStyle);
                sb.AppendFormat(" {0}", this.DayStyle.CssClass);

                var s = Convert.ToString(sb.ToString().Trim());
                if (s.Length > 0)
                {
                    tbcell.CssClass = s;
                }

                // Add a literal control to the cell using the day number for the
                // text.
                tbcell.Controls.Add(new LiteralControl(day.DayNumberText));

                return tbcell;
            }

            //
            // Returns true if the selection mode includes week selectors.
            //
            private new bool HasWeekSelectors(CalendarSelectionMode selectionMode)
            {
                if (selectionMode == CalendarSelectionMode.DayWeek ||
                    selectionMode == CalendarSelectionMode.DayWeekMonth)
                {
                    return true;
                }
                return false;
            }

            #endregion

            #region Post back event handling

            // ====================================================================
            // Functions for converting between DateTime and day count values.
            // ====================================================================

            //
            // Returns the number of days between the given DateTime value and the
            // base date.
            //
            private int DayCountFromDate(DateTime date)
            {
                return (date - DayCountBaseDate).Days;
            }

            //
            // Returns a DateTime value equal to the base date plus the given number
            // of days.
            //
            private DateTime DateFromDayCount(int dayCount)
            {
                return DayCountBaseDate.AddDays(dayCount);
            }

            // ====================================================================
            // Functions to save and load the nonselectable dates list.
            //
            // A hidden form field is used to store this data rather than the
            // view state because the nonselectable dates are not known until after
            // the DayRender event has been raised for each day as the control is
            // rendered.
            //
            // To minimize the amount of data stored in that field, the dates are
            // represented as day count values.
            // ====================================================================

            //
            // Saves a list of dates to the hidden form field.
            //
            private void SaveNonselectableDates(ArrayList dates)
            {
                // Build a string array by converting each date to a day count
                // value.
                var list = new string[dates.Count - 1 + 1];
                for (var i = 0; i <= list.Length - 1; i++)
                {
                    list[i] = Convert.ToString(
                        this.DayCountFromDate(DateTime.Parse(Convert.ToString(dates[i].ToString()))).ToString());
                }

                // Get the hidden field name.
                var fieldName = this.GetHiddenFieldName();

                // For the field value, create a comma-separated list from the day
                // count values.
                var fieldValue = HttpUtility.HtmlAttributeEncode(string.Join(",", list));

                // Add the hidden form field to the page.
                //EVT-9313 this.Page.RegisterHiddenField(fieldName, fieldValue);
                this.Page.ClientScript.RegisterHiddenField(fieldName, fieldValue);
            }

            //
            // Returns a list of dates stored in the hidden form field.
            //
            private ArrayList LoadNonselectableDates()
            {
                // Create an empty list.
                var dates = new ArrayList();

                // Get the value stored in the hidden form field.
                var fieldName = this.GetHiddenFieldName();
                var fieldValue = this.Page.Request.Form[fieldName];

                // If no dates were stored, return the empty list.
                if (ReferenceEquals(fieldValue, null))
                {
                    return dates;
                }

                // Extract the individual day count values.
                var list = fieldValue.Split(',');

                // Convert those values to dates and store them in an array list.
                foreach (var s in list)
                {
                    dates.Add(this.DateFromDayCount(int.Parse(s)));
                }

                return dates;
            }

            //
            // Returns the name of the hidden field used to store nonselectable
            // dates on the form.
            //
            private string GetHiddenFieldName()
            {
                // Create a unique field name.
                return string.Format("{0}_NonselectableDates", this.ClientID);
            }

            // ====================================================================
            // Implementation of the IPostBackEventHandler.RaisePostBackEvent
            // event handler.
            // ====================================================================

            /// <summary>
            ///     Handles a post back event targeted at the control.
            /// </summary>
            /// <param name="eventArgument">
            ///     A <see cref="System.String" /> representing the event argument passed to the handler.
            /// </param>
            public new void RaisePostBackEvent(string eventArgument)
            {
                // Was the post back initiated by a previous or next month link
                if (eventArgument.StartsWith("V"))
                {
                    try
                    {
                        // Save the current visible date.
                        var previousDate = this.TargetDate;

                        // Extract the day count from the argument and use it to
                        // change the visible date.
                        var d = int.Parse(eventArgument.Substring(1));
                        this.VisibleDate = this.DateFromDayCount(d);

                        // Raise the VisibleMonthChanged event.
                        this.OnVisibleMonthChanged(this.VisibleDate, previousDate);
                    }
                    catch (Exception)
                    { }
                    return;
                }

                // Was the post back initiated by a month or week selector link
                if (eventArgument.StartsWith("R"))
                {
                    try
                    {
                        // Extract the day count and number of days from the
                        // argument.
                        var d = int.Parse(eventArgument.Substring(1, eventArgument.Length - 3));
                        var n = int.Parse(eventArgument.Substring(eventArgument.Length - 2));

                        // Get the starting date.
                        var date = this.DateFromDayCount(d);

                        // Reset the selected dates collection to include all the
                        // dates in the given range.
                        this.SelectedDates.Clear();
                        this.SelectedDates.SelectRange(date, date.AddDays(n - 1));

                        // // If SelectAllInRange is false, remove any dates found
                        // // in the nonselectable date list.
                        // if (!this.SelectAllInRange)
                        // {
                        // ArrayList nonselectableDates = this.LoadNonselectableDates();
                        // foreach(DateTime badDate in nonselectableDates)
                        // this.SelectedDates.Remove(badDate);
                        // }

                        // Raise the SelectionChanged event.
                        this.OnSelectionChanged();
                    }
                    catch (Exception)
                    { }
                    return;
                }

                // The post back must have been initiated by a calendar day link.
                try
                {
                    // Get the day count from the argument.
                    var d = int.Parse(eventArgument);

                    // Reset the selected dates collection to include only the
                    // newly selected date.
                    this.SelectedDates.Clear();
                    this.SelectedDates.Add(this.DateFromDayCount(d));

                    // Raise the SelectionChanged event.
                    this.OnSelectionChanged();
                }
                catch (Exception)
                { }
            }

            #endregion
        }
    }
}