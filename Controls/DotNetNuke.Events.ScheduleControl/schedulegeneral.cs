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
    using Microsoft.VisualBasic;

    /// -----------------------------------------------------------------------------
    /// Project	 : schedule
    /// Class	 : ScheduleGeneral
    /// 
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     The ScheduleGeneral web control is designed to represent a schedule in a general format.
    /// </summary>
    /// -----------------------------------------------------------------------------
    [ParseChildren(true)]
    public class ScheduleGeneral : BaseSchedule
    {
        #region Private and protected properties

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     list with values to be shown in title header
        /// </summary>
        /// -----------------------------------------------------------------------------
        protected ArrayList arrTitleValues
        {
            get { return (ArrayList) this.ViewState["arrTitleValues"]; }
            set { this.ViewState["arrTitleValues"] = value; }
        }

        #endregion

        #region Public properties

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     When true, a separate header will be added for the date.
        ///     This requires DataRangeStartField and DataRangeEndField to be of type DateTime.
        /// </summary>
        /// -----------------------------------------------------------------------------
        [Description(
            "When true, a separate header will be added for the date. This requires DataRangeStartField and DataRangeEndField to be of type DateTime.")]
        [DefaultValue(false)]
        [Bindable(false)]
        [Category("Behavior")]
        public virtual bool SeparateDateHeader
        {
            get
                {
                    var o = this.ViewState["SeparateDateHeader"];
                    if (!ReferenceEquals(o, null))
                    {
                        return Convert.ToBoolean(o);
                    }
                    return false;
                }
            set { this.ViewState["SeparateDateHeader"] = value; }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     The format used for the title if the TitleTemplate is missing, e.g. {0:ddd d}
        /// </summary>
        /// -----------------------------------------------------------------------------
        [Description("The format used for the title if the TitleTemplate is missing, e.g. {0:ddd d}")]
        [DefaultValue("")]
        [Category("Data")]
        public string TitleDataFormatString
        {
            get
                {
                    var o = this.ViewState["TitleDataFormatString"];
                    if (!ReferenceEquals(o, null))
                    {
                        return Convert.ToString(o);
                    }
                    return string.Empty;
                }
            set { this.ViewState["TitleDataFormatString"] = value; }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     The format used for the ranges if the RangeHeaderTemplate is missing, e.g. {0:hh:mm}
        /// </summary>
        /// -----------------------------------------------------------------------------
        [Description("The format used for the ranges if the RangeHeaderTemplate is missing, e.g. {0:hh:mm}")]
        [DefaultValue("")]
        [Category("Data")]
        public string RangeDataFormatString
        {
            get
                {
                    var o = this.ViewState["RangeDataFormatString"];
                    if (!ReferenceEquals(o, null))
                    {
                        return Convert.ToString(o);
                    }
                    return string.Empty;
                }
            set { this.ViewState["RangeDataFormatString"] = value; }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     The format used for the date header if SeparateDateHeader=True and the DateHeaderTemplate is missing, e.g.
        ///     {0:dd/MM}
        /// </summary>
        /// -----------------------------------------------------------------------------
        [Description(
            "The format used for the date header if SeparateDateHeader=true and the DateHeaderTemplate is missing, e.g. {0:dd/MM}")]
        [DefaultValue("")]
        [Category("Data")]
        public string DateHeaderDataFormatString
        {
            get
                {
                    var o = this.ViewState["DateHeaderDataFormatString"];
                    if (!ReferenceEquals(o, null))
                    {
                        return Convert.ToString(o);
                    }
                    return string.Empty;
                }
            set { this.ViewState["DateHeaderDataFormatString"] = value; }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     When true, titles will automatically be sorted alphabetically.
        ///     When false, you may provide your own sorting order for the titles, but make sure
        ///     that the items with the same titles are grouped together, and that for each title,
        ///     the items are sorted on DataRangeStartField first and on DataRangeEndField next.
        ///     (for example: if you want to sort on a field called "SortOrder", the
        ///     DataRangeStartField is "StartTime", and the DataRangeEndField is "EndTime",
        ///     use the sorting expression "ORDER BY SortOrder ASC, StartTime ASC, EndTime ASC")
        ///     The default value for AutoSortTitles is true.
        /// </summary>
        /// -----------------------------------------------------------------------------
        [Description(
            "When true, titles will automatically be sorted alphabetically. When false, the data source should be sorted properly before binding.")]
        [DefaultValue(true)]
        [Bindable(false)]
        [Category("Behavior")]
        public virtual bool AutoSortTitles
        {
            get
                {
                    var o = this.ViewState["AutoSortTitles"];
                    if (!ReferenceEquals(o, null))
                    {
                        return Convert.ToBoolean(o);
                    }
                    return true;
                }
            set { this.ViewState["AutoSortTitles"] = value; }
        }

        #endregion

        #region Templates

        [TemplateContainer(typeof(ScheduleItem))]
        [Browsable(false)]
        [Description("The template used to create title header content.")]
        [NotifyParentProperty(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate TitleTemplate { get; set; }

        [TemplateContainer(typeof(ScheduleItem))]
        [Browsable(false)]
        [Description("The template used to create range header content.")]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate RangeHeaderTemplate { get; set; }

        [TemplateContainer(typeof(ScheduleItem))]
        [Browsable(false)]
        [Description("The template used to create date header content.")]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate DateHeaderTemplate { get; set; }

        #endregion

        #region Methods

        // Check if all properties are set to make the control work
        public override string CheckConfiguration()
        {
            if (this.TitleField == "")
            {
                return "The TitleField property is not set";
            }
            if (this.DataRangeStartField == "")
            {
                return "The DataRangeStartField property is not set";
            }
            if (this.DataRangeEndField == "")
            {
                return "The DataRangeEndField property is not set";
            }
            return string.Empty;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     create the list with all range header values (Start or End)
        /// </summary>
        /// <param name="dv"></param>
        /// -----------------------------------------------------------------------------
        public override void FillRangeValueArray(ref DataView dv)
        {
            this.arrRangeValues = new ArrayList();
            var strOldSort = dv.Sort;
            if (this.FullTimeScale)
            {
                var tsInc = new TimeSpan(0, this.TimeScaleInterval, 0);
                if (dv.Count == 0)
                {
                    return; // empty database
                }
                dv.Sort = this.DataRangeStartField + " ASC ";
                // Nulls are allowed (for creating titles without content) but will not show up
                var i = 0;
                while (i < dv.Count && Information.IsDBNull(dv[i][this.DataRangeStartField]))
                {
                    i++;
                }
                if (i >= dv.Count)
                {
                    return;
                }
                var dt1 = Convert.ToDateTime(dv[i][this.DataRangeStartField]); // first start time in dataview
                dv.Sort = this.DataRangeEndField + " DESC ";
                i = 0;
                while (Information.IsDBNull(dv[i][this.DataRangeStartField]))
                {
                    i++;
                }
                var dt2 = Convert.ToDateTime(dv[i][this.DataRangeEndField]); // last end time in dataview
                // add incrementing times to the array
                while (dt1 <= dt2)
                {
                    var t = this.StartOfTimeScale;
                    while (TimeSpan.Compare(t, this.EndOfTimeScale) < 0)
                    {
                        var dt = new DateTime(dt1.Year, dt1.Month, dt1.Day, t.Hours, t.Minutes, 0);
                        this.arrRangeValues.Add(dt);
                        t = t.Add(tsInc);
                    }
                    // Add the end of the timescale as well to make sure it's there
                    // e.g. in the case of EndOfTimeScale=23:59 and TimeScaleInterval=1440, this is imperative
                    var dtEnd = new DateTime(dt1.Year, dt1.Month, dt1.Day, this.EndOfTimeScale.Hours,
                                             this.EndOfTimeScale.Minutes, 0);
                    this.arrRangeValues.Add(dtEnd);
                    dt1 = dt1.AddDays(1);
                }
            }
            else // Not FullTimeScale
            {
                // Just add the times from the dataview
                var j = 0;
                for (j = 0; j <= dv.Count - 1; j++)
                {
                    // Nulls are allowed (for creating titles without content) but will not show up
                    if (!Information.IsDBNull(dv[j][this.DataRangeStartField]))
                    {
                        var t1 = dv[j][this.DataRangeStartField];
                        var t2 = dv[j][this.DataRangeEndField];
                        this.arrRangeValues.Add(t1);
                        this.arrRangeValues.Add(t2);
                    }
                }
            }

            this.arrRangeValues.Sort();
            RemoveDoubles(this.arrRangeValues);
            dv.Sort = strOldSort;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     create the list with all titles
        /// </summary>
        /// <param name="dv"></param>
        /// -----------------------------------------------------------------------------
        public override void FillTitleValueArray(ref DataView dv)
        {
            this.arrTitleValues = new ArrayList();
            var i = 0;
            for (i = 0; i <= dv.Count - 1; i++)
            {
                var val = dv[i][this.TitleField];
                this.arrTitleValues.Add(val);
            }
            if (this.AutoSortTitles)
            {
                this.arrTitleValues.Sort();
            }
            RemoveDoubles(this.arrTitleValues);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///     Add date headers to the table when SeparateDateHeader=True
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void AddDateHeaderData()
        {
            if (!this.SeparateDateHeader)
            {
                return;
            }
            // merge all cells having the same date in the first (date) header
            if (this.arrRangeValues.Count == 0)
            {
                return;
            }
            if (!(this.arrRangeValues[0] is DateTime) && !(this.arrRangeValues[0] is DateTime))
            {
                throw new HttpException("If SeparateDateHeader is true, then DataRangeStartField " +
                                        " and DataRangeEndField need to be of type DateTime");
            }
            if (this.Layout == LayoutEnum.Horizontal)
            {
                // In horizontal mode, add an extra row for date headers
                this.Table1.Rows.AddAt(0, new TableRow());
            }
            this.Table1.Rows[0].Cells.AddAt(0, new TableHeaderCell());
            this.Table1.Rows[0].Cells[0].ApplyStyle(this.TitleStyle);

            var prevRangeValue = Convert.ToDateTime(this.arrRangeValues[this.arrRangeValues.Count - 1]);
            var prevStartValueIndex = this.arrRangeValues.Count;
            if (!this.IncludeEndValue && this.ShowValueMarks)
            {
                prevStartValueIndex = this.arrRangeValues.Count * 2 - 1;
            }

            var i = 0;
            for (i = this.arrRangeValues.Count - 1; i >= 0; i--)
            {
                var arrRangeValue = Convert.ToDateTime(this.arrRangeValues[i]);
                if (arrRangeValue.Date != prevRangeValue.Date)
                {
                    // this value has another date than the previous one (the one below or to the right)
                    // add a cell below or to the right which spans the cells that have the same date
                    var ValueIndexOfNextCell =
                        i + 2; // add 1 for the title cell and 1 because it's the next cell, not this one
                    var Span = prevStartValueIndex - ValueIndexOfNextCell + 1;

                    if (!this.IncludeEndValue && this.ShowValueMarks)
                    {
                        ValueIndexOfNextCell = i * 2 + 3;
                        Span = prevStartValueIndex - ValueIndexOfNextCell + 2;
                    }

                    var cell = default(TableCell);
                    if (this.Layout == LayoutEnum.Vertical)
                    {
                        this.Table1.Rows[ValueIndexOfNextCell].Cells.AddAt(0, new TableHeaderCell());
                        cell = this.Table1.Rows[ValueIndexOfNextCell].Cells[0];
                        cell.RowSpan = Span;
                    }
                    else // Horizontal
                    {
                        this.Table1.Rows[0].Cells.AddAt(1, new TableHeaderCell());
                        cell = this.Table1.Rows[0].Cells[1];
                        cell.ColumnSpan = Span;
                    }
                    cell.ApplyStyle(this.RangeHeaderStyle);
                    prevRangeValue = arrRangeValue;
                    prevStartValueIndex = i + 1;
                    if (!this.IncludeEndValue && this.ShowValueMarks)
                    {
                        prevStartValueIndex = i * 2 + 1;
                    }
                }
            }
            // finish by adding the first cell also
            var cell0 = default(TableCell);
            var Span0 = prevStartValueIndex;
            if (!this.IncludeEndValue && this.ShowValueMarks)
            {
                Span0++;
            }

            if (this.Layout == LayoutEnum.Vertical)
            {
                this.Table1.Rows[1].Cells.AddAt(0, new TableHeaderCell());
                cell0 = this.Table1.Rows[1].Cells[0];
                cell0.RowSpan = Span0;
            }
            else // Horizontal
            {
                this.Table1.Rows[0].Cells.AddAt(1, new TableHeaderCell());
                cell0 = this.Table1.Rows[0].Cells[1];
                cell0.ColumnSpan = Span0;
            }
            cell0.ApplyStyle(this.RangeHeaderStyle);

            // iterate arrRangeValues in forward direction creating a new item for each data item
            // forward because it has to be in the same order as after postback, and there it's\
            // much easier if it's forward
            var cellIndex = 1;
            i = 0;
            while (i < this.arrRangeValues.Count)
            {
                var arrRangeValue = Convert.ToDateTime(this.arrRangeValues[i]);
                this.CreateItem(cellIndex, 0, ScheduleItemType.DateHeader, true, arrRangeValue, -1);
                var Span = this.GetValueSpan(cellIndex, 0);
                if (Span == 0)
                {
                    Span = 1;
                }
                if (this.Layout == LayoutEnum.Horizontal)
                {
                    cellIndex++;
                }
                else
                {
                    cellIndex += Span;
                }
                if (!this.IncludeEndValue && this.ShowValueMarks)
                {
                    i += Span / 2;
                }
                else
                {
                    i += Span;
                }
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
            // Find the title index by matching with the title values array
            var k = 0;
            for (k = 0; k <= this.arrTitleValues.Count - 1; k++)
            {
                if (this.arrTitleValues[k].ToString() == objTitleValue.ToString())
                {
                    return k + 1;
                }
            }
            return -1;
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
            if (Information.IsDBNull(objRangeValue))
            {
                return -1;
            }
            var RangeValueIndex = -1;
            // Find range value index by matching with range values array
            if (this.FullTimeScale && !(objRangeValue is DateTime))
            {
                throw new HttpException("The range field should be of type DateTime when FullTimeScale is set to true");
            }
            var k = 0;
            if (this.FullTimeScale)
            {
                var Dobj = Convert.ToDateTime(objRangeValue);
                // if no match is found, use the index of the EndOfTimeScale value
                RangeValueIndex = this.arrRangeValues.Count;
                for (k = 0; k <= this.arrRangeValues.Count - 1; k++)
                {
                    var Dk = Convert.ToDateTime(this.arrRangeValues[k]);
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
            if (!this.IncludeEndValue && this.ShowValueMarks)
            {
                if (this.Layout == LayoutEnum.Vertical)
                {
                    // Each item spans two rows
                    return RangeValueIndex * 2;
                }
                // Add one cell for the empty background cell on the left
                return RangeValueIndex + 1;
            }
            return RangeValueIndex;
        }

        protected override int GetTitleCount()
        {
            return this.arrTitleValues.Count;
        }

        public override int GetRangeHeaderIndex()
        {
            // when SeparateDateHeader=True, the first index (column or row) is the date header,
            // the second (column or row) contains the range values
            return Convert.ToInt32(this.SeparateDateHeader ? 1 : 0);
        }

        public override void AddTitleHeaderData()
        {
            var nTitles = this.GetTitleCount();

            // iterate arrTitleValues creating a new item for each data item
            var titleIndex = 0;
            for (titleIndex = 1; titleIndex <= nTitles; titleIndex++)
            {
                var obj = this.arrTitleValues[titleIndex - 1];
                this.CreateItem(0, titleIndex, ScheduleItemType.TitleHeader, true, obj, -1);
            }
        }

        public override string GetSortOrder()
        {
            // make sure the data is processed in the right order: from bottom right up to top left.
            if (this.AutoSortTitles)
            {
                return this.TitleField + " ASC, " + this.DataRangeStartField + " ASC, " + this.DataRangeEndField +
                       " ASC";
            }
            return ""; // leave sort order as it is when AutoSortTitles=False
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
                    if (this.TitleDataFormatString.Length == 0)
                    {
                        retVal = value.ToString();
                    }
                    else
                    {
                        retVal = string.Format(this.TitleDataFormatString, value);
                    }
                }
                else if (type == ScheduleItemType.RangeHeader)
                {
                    if (this.RangeDataFormatString.Length == 0)
                    {
                        retVal = value.ToString();
                    }
                    else
                    {
                        retVal = string.Format(this.RangeDataFormatString, value);
                    }
                }
                else if (type == ScheduleItemType.DateHeader)
                {
                    if (this.DateHeaderDataFormatString.Length == 0)
                    {
                        retVal = value.ToString();
                    }
                    else
                    {
                        retVal = string.Format(this.DateHeaderDataFormatString, value);
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
                    return this.RangeHeaderTemplate;
                case ScheduleItemType.TitleHeader:
                    return this.TitleTemplate;
                case ScheduleItemType.DateHeader:
                    return this.DateHeaderTemplate;
                case ScheduleItemType.Item:
                    return this.ItemTemplate;
                case ScheduleItemType.AlternatingItem:
                    return this.ItemTemplate;
            }
            return null;
        }

        protected override TableItemStyle GetStyle(ScheduleItemType type)
        {
            // handle DateHeader, which is not handled in the base class
            if (type == ScheduleItemType.DateHeader)
            {
                return this.RangeHeaderStyle;
            }
            return base.GetStyle(type);
        }

        /// ' -----------------------------------------------------------------------------
        /// '
        /// <summary>
        ///     ' Calculate the title (data source value) given the cell index
        ///     '
        /// </summary>
        /// '
        /// <param name="titleIndex">Title index of the cell</param>
        /// '
        /// <returns>Object containing the title</returns>
        /// ' -----------------------------------------------------------------------------
        public override dynamic CalculateTitle(int titleIndex, int cellIndex)
        {
            return this.arrTitleValues[titleIndex - 1];
        }

        #endregion
    }
}