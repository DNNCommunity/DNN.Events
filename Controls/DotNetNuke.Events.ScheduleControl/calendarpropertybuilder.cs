namespace DotNetNuke.Modules.Events.ScheduleControl
{
    using System;
    using System.Windows.Forms;

    //
    // DotNetNuke® - http://www.dnnsoftware.com
    // Copyright (c) 2002-2013
    // by DNNCorp
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

    /// -----------------------------------------------------------------------------
    /// Project	 : schedule
    /// Class	 : CalendarPropertyBuilder
    /// 
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///     CalendarPropertyBuilder provides an easy interface to set all the properties
    ///     for the ScheduleCalendar control.
    /// </summary>
    /// -----------------------------------------------------------------------------
    public class CalendarPropertyBuilder : Form
    {
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cbFullTimeScale_CheckedChanged(object sender, EventArgs e)
        {
            this.tbStartTimeScale.Enabled = this.cbFullTimeScale.Checked;
            this.tbEndTimeScale.Enabled = this.cbFullTimeScale.Checked;
            this.tbInterval.Enabled = this.cbFullTimeScale.Checked;
        }

        private void cbTimeFieldsContainDate_CheckedChanged(object sender, EventArgs e)
        {
            this.tbDateField.Enabled = !this.cbTimeFieldsContainDate.Checked;
        }

        private void rbHorizontal_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rbHorizontal.Checked)
            {
                this.tbNumberOfRepetitions.Text = "1";
                this.tbNumberOfRepetitions.Enabled = false;
            }
            else
            {
                this.tbNumberOfRepetitions.Enabled = true;
            }
        }

        private void cbShowValueMarks_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbShowValueMarks.Checked)
            {
                this.cbIncludeEndValue.Checked = false;
            }
        }

        private void tbNumberOfDays_TextChanged(object sender, EventArgs e)
        {
            this.cbStartDay.Enabled = this.tbNumberOfDays.Text == "7"; // startday is only used on weekly calendar
        }

        private void cbEnableEmptySlotClick_CheckedChanged(object sender, EventArgs e)
        {
            this.tbEmptySlotToolTip.Enabled = this.cbEnableEmptySlotClick.Checked;
        }

        #region  Windows Form Designer generated code

        public CalendarPropertyBuilder()
        {
            //This call is required by the Windows Form Designer.
            InitializeComponent();

            //Add any initialization after the InitializeComponent() call
        }

        //Form overrides dispose to clean up the component list.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!(ReferenceEquals(components, null)))
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        //Required by the Windows Form Designer
        private System.ComponentModel.Container components = null;

        //NOTE: The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.
        //Do not modify it using the code editor.
        internal System.Windows.Forms.Button btnOK;

        internal System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.TextBox tbDateField;
        internal System.Windows.Forms.TextBox tbStartTimeField;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.TextBox tbEndTimeField;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.CheckBox cbIncludeEndValue;
        internal System.Windows.Forms.Label Label5;
        internal System.Windows.Forms.Label Label6;
        internal System.Windows.Forms.TextBox tbTimeFormatString;
        internal System.Windows.Forms.TextBox tbDateFormatString;
        internal System.Windows.Forms.CheckBox cbTimeFieldsContainDate;
        internal System.Windows.Forms.TextBox tbNumberOfRepetitions;
        internal System.Windows.Forms.Label Label9;
        internal System.Windows.Forms.GroupBox GroupBox1;
        internal System.Windows.Forms.GroupBox GroupBox2;
        internal System.Windows.Forms.CheckBox cbFullTimeScale;
        internal System.Windows.Forms.TextBox tbStartTimeScale;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.TextBox tbEndTimeScale;
        internal System.Windows.Forms.Label Label7;
        internal System.Windows.Forms.TextBox tbInterval;
        internal System.Windows.Forms.Label Label8;
        internal System.Windows.Forms.Label Label10;
        internal System.Windows.Forms.GroupBox GroupBox3;
        internal System.Windows.Forms.RadioButton rbHorizontal;
        internal System.Windows.Forms.RadioButton rbVertical;
        internal System.Windows.Forms.ToolTip ToolTip1;
        internal System.Windows.Forms.Label Label11;
        internal System.Windows.Forms.CheckBox cbShowValueMarks;
        internal System.Windows.Forms.TextBox tbNumberOfDays;
        internal System.Windows.Forms.Label Label12;
        internal System.Windows.Forms.Label Label13;
        internal System.Windows.Forms.ComboBox cbStartDay;
        private System.Windows.Forms.GroupBox groupBox4;
        internal System.Windows.Forms.Label label14;
        internal System.Windows.Forms.TextBox tbEmptySlotToolTip;
        internal System.Windows.Forms.CheckBox cbEnableEmptySlotClick;
        internal System.Windows.Forms.TextBox tbItemStyleField;

        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            this.btnCancel = new System.Windows.Forms.Button();
            this.Label1 = new System.Windows.Forms.Label();
            this.tbDateField = new System.Windows.Forms.TextBox();
            this.tbStartTimeField = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.tbEndTimeField = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.cbIncludeEndValue = new System.Windows.Forms.CheckBox();
            this.cbTimeFieldsContainDate = new System.Windows.Forms.CheckBox();
            this.cbTimeFieldsContainDate.CheckedChanged +=
                new System.EventHandler(this.cbTimeFieldsContainDate_CheckedChanged);
            this.tbNumberOfRepetitions = new System.Windows.Forms.TextBox();
            this.Label9 = new System.Windows.Forms.Label();
            this.tbTimeFormatString = new System.Windows.Forms.TextBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.tbDateFormatString = new System.Windows.Forms.TextBox();
            this.Label6 = new System.Windows.Forms.Label();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.Label11 = new System.Windows.Forms.Label();
            this.tbItemStyleField = new System.Windows.Forms.TextBox();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.Label10 = new System.Windows.Forms.Label();
            this.tbInterval = new System.Windows.Forms.TextBox();
            this.Label8 = new System.Windows.Forms.Label();
            this.tbEndTimeScale = new System.Windows.Forms.TextBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.tbStartTimeScale = new System.Windows.Forms.TextBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.cbFullTimeScale = new System.Windows.Forms.CheckBox();
            this.cbFullTimeScale.CheckedChanged += new System.EventHandler(this.cbFullTimeScale_CheckedChanged);
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.rbVertical = new System.Windows.Forms.RadioButton();
            this.rbHorizontal = new System.Windows.Forms.RadioButton();
            this.rbHorizontal.CheckedChanged += new System.EventHandler(this.rbHorizontal_CheckedChanged);
            this.cbShowValueMarks = new System.Windows.Forms.CheckBox();
            this.cbShowValueMarks.CheckedChanged += new System.EventHandler(this.cbShowValueMarks_CheckedChanged);
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tbNumberOfDays = new System.Windows.Forms.TextBox();
            this.tbNumberOfDays.TextChanged += new System.EventHandler(this.tbNumberOfDays_TextChanged);
            this.cbStartDay = new System.Windows.Forms.ComboBox();
            this.cbEnableEmptySlotClick = new System.Windows.Forms.CheckBox();
            this.cbEnableEmptySlotClick.CheckedChanged +=
                new System.EventHandler(this.cbEnableEmptySlotClick_CheckedChanged);
            this.Label12 = new System.Windows.Forms.Label();
            this.Label13 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.tbEmptySlotToolTip = new System.Windows.Forms.TextBox();
            this.GroupBox1.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.GroupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            //
            //btnOK
            //
            this.btnOK.Location = new System.Drawing.Point(260, 492);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(60, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            //
            //btnCancel
            //
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(324, 492);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(60, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            //
            //Label1
            //
            this.Label1.Location = new System.Drawing.Point(12, 56);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(64, 16);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "Date field:";
            //
            //tbDateField
            //
            this.tbDateField.Location = new System.Drawing.Point(12, 72);
            this.tbDateField.Name = "tbDateField";
            this.tbDateField.Size = new System.Drawing.Size(168, 20);
            this.tbDateField.TabIndex = 1;
            this.ToolTip1.SetToolTip(this.tbDateField,
                                     "The database field providing the dates. Ignored when TimeFieldsContainDate=true. " +
                                     "When TimeFieldsContainDate=false, this field should be of type Date.");
            //
            //tbStartTimeField
            //
            this.tbStartTimeField.Location = new System.Drawing.Point(12, 116);
            this.tbStartTimeField.Name = "tbStartTimeField";
            this.tbStartTimeField.Size = new System.Drawing.Size(168, 20);
            this.tbStartTimeField.TabIndex = 5;
            this.ToolTip1.SetToolTip(this.tbStartTimeField,
                                     "The database field containing the start time of the events. This field should als" +
                                     "o contain the date when TimeFieldsContainDate=true");
            //
            //Label2
            //
            this.Label2.Location = new System.Drawing.Point(12, 100);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(84, 16);
            this.Label2.TabIndex = 4;
            this.Label2.Text = "Start time field:";
            //
            //tbEndTimeField
            //
            this.tbEndTimeField.Location = new System.Drawing.Point(12, 160);
            this.tbEndTimeField.Name = "tbEndTimeField";
            this.tbEndTimeField.Size = new System.Drawing.Size(168, 20);
            this.tbEndTimeField.TabIndex = 9;
            this.ToolTip1.SetToolTip(this.tbEndTimeField,
                                     "The database field containing the end time of the events. This field should also " +
                                     "contain the date when TimeFieldsContainDate=true");
            //
            //Label3
            //
            this.Label3.Location = new System.Drawing.Point(12, 144);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(84, 12);
            this.Label3.TabIndex = 8;
            this.Label3.Text = "End time field:";
            //
            //cbIncludeEndValue
            //
            this.cbIncludeEndValue.Location = new System.Drawing.Point(268, 279);
            this.cbIncludeEndValue.Name = "cbIncludeEndValue";
            this.cbIncludeEndValue.Size = new System.Drawing.Size(116, 24);
            this.cbIncludeEndValue.TabIndex = 2;
            this.cbIncludeEndValue.Text = "Include end value";
            this.ToolTip1.SetToolTip(this.cbIncludeEndValue,
                                     "If true, the event is shown including the end row or column");
            //
            //cbTimeFieldsContainDate
            //
            this.cbTimeFieldsContainDate.Location = new System.Drawing.Point(16, 24);
            this.cbTimeFieldsContainDate.Name = "cbTimeFieldsContainDate";
            this.cbTimeFieldsContainDate.Size = new System.Drawing.Size(164, 24);
            this.cbTimeFieldsContainDate.TabIndex = 13;
            this.cbTimeFieldsContainDate.Text = "Time fields contain date";
            this.ToolTip1.SetToolTip(this.cbTimeFieldsContainDate,
                                     "Indicates whether the time fields (StartTimeField and EndTimeField) contain the d" +
                                     "ate as well. When true, this allows midnight spanning for calendar events. When " +
                                     "false, the DateField contains the date.");
            //
            //tbNumberOfRepetitions
            //
            this.tbNumberOfRepetitions.Location = new System.Drawing.Point(340, 253);
            this.tbNumberOfRepetitions.Name = "tbNumberOfRepetitions";
            this.tbNumberOfRepetitions.Size = new System.Drawing.Size(44, 20);
            this.tbNumberOfRepetitions.TabIndex = 1;
            this.ToolTip1.SetToolTip(this.tbNumberOfRepetitions,
                                     "The NumberOfRepetitions to show at a time. Only used in Vertical layout.");
            //
            //Label9
            //
            this.Label9.Location = new System.Drawing.Point(203, 255);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(131, 16);
            this.Label9.TabIndex = 1;
            this.Label9.Text = "Number of repetitions:";
            this.Label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
            //
            //tbTimeFormatString
            //
            this.tbTimeFormatString.Location = new System.Drawing.Point(192, 116);
            this.tbTimeFormatString.Name = "tbTimeFormatString";
            this.tbTimeFormatString.Size = new System.Drawing.Size(168, 20);
            this.tbTimeFormatString.TabIndex = 7;
            this.ToolTip1.SetToolTip(this.tbTimeFormatString,
                                     "The format used for the times if the TimeTemplate is missing, e.g. {0:hh:mm}");
            //
            //Label5
            //
            this.Label5.Location = new System.Drawing.Point(192, 100);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(120, 16);
            this.Label5.TabIndex = 6;
            this.Label5.Text = "Time format string:";
            //
            //tbDateFormatString
            //
            this.tbDateFormatString.Location = new System.Drawing.Point(192, 72);
            this.tbDateFormatString.Name = "tbDateFormatString";
            this.tbDateFormatString.Size = new System.Drawing.Size(168, 20);
            this.tbDateFormatString.TabIndex = 3;
            this.ToolTip1.SetToolTip(this.tbDateFormatString,
                                     "The format used for the date if the DateTemplate is missing, e.g. {0:ddd d}");
            //
            //Label6
            //
            this.Label6.Location = new System.Drawing.Point(192, 56);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(120, 16);
            this.Label6.TabIndex = 2;
            this.Label6.Text = "Date format string:";
            //
            //GroupBox1
            //
            this.GroupBox1.Controls.Add(this.Label11);
            this.GroupBox1.Controls.Add(this.tbItemStyleField);
            this.GroupBox1.Controls.Add(this.Label1);
            this.GroupBox1.Controls.Add(this.tbDateField);
            this.GroupBox1.Controls.Add(this.tbDateFormatString);
            this.GroupBox1.Controls.Add(this.Label6);
            this.GroupBox1.Controls.Add(this.Label3);
            this.GroupBox1.Controls.Add(this.tbStartTimeField);
            this.GroupBox1.Controls.Add(this.tbTimeFormatString);
            this.GroupBox1.Controls.Add(this.Label5);
            this.GroupBox1.Controls.Add(this.Label2);
            this.GroupBox1.Controls.Add(this.tbEndTimeField);
            this.GroupBox1.Controls.Add(this.cbTimeFieldsContainDate);
            this.GroupBox1.Location = new System.Drawing.Point(8, 12);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(376, 232);
            this.GroupBox1.TabIndex = 0;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Data source fields";
            //
            //Label11
            //
            this.Label11.Location = new System.Drawing.Point(12, 188);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(84, 12);
            this.Label11.TabIndex = 14;
            this.Label11.Text = "Item style field:";
            //
            //tbItemStyleField
            //
            this.tbItemStyleField.Location = new System.Drawing.Point(12, 204);
            this.tbItemStyleField.Name = "tbItemStyleField";
            this.tbItemStyleField.Size = new System.Drawing.Size(168, 20);
            this.tbItemStyleField.TabIndex = 15;
            this.ToolTip1.SetToolTip(this.tbItemStyleField,
                                     "An optional database field providing the item styles (in the form of a css class " +
                                     "name)");
            //
            //GroupBox2
            //
            this.GroupBox2.Controls.Add(this.Label10);
            this.GroupBox2.Controls.Add(this.tbInterval);
            this.GroupBox2.Controls.Add(this.Label8);
            this.GroupBox2.Controls.Add(this.tbEndTimeScale);
            this.GroupBox2.Controls.Add(this.Label7);
            this.GroupBox2.Controls.Add(this.tbStartTimeScale);
            this.GroupBox2.Controls.Add(this.Label4);
            this.GroupBox2.Controls.Add(this.cbFullTimeScale);
            this.GroupBox2.Location = new System.Drawing.Point(8, 353);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(376, 84);
            this.GroupBox2.TabIndex = 4;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Full time scale";
            //
            //Label10
            //
            this.Label10.Location = new System.Drawing.Point(328, 56);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(24, 16);
            this.Label10.TabIndex = 7;
            this.Label10.Text = "min.";
            //
            //tbInterval
            //
            this.tbInterval.Location = new System.Drawing.Point(284, 52);
            this.tbInterval.Name = "tbInterval";
            this.tbInterval.Size = new System.Drawing.Size(40, 20);
            this.tbInterval.TabIndex = 6;
            this.ToolTip1.SetToolTip(this.tbInterval,
                                     "The number of minutes between each mark on the time scale. Only used when FullTim" +
                                     "eScale is true.");
            //
            //Label8
            //
            this.Label8.Location = new System.Drawing.Point(232, 56);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(48, 16);
            this.Label8.TabIndex = 5;
            this.Label8.Text = "Interval:";
            //
            //tbEndTimeScale
            //
            this.tbEndTimeScale.Location = new System.Drawing.Point(160, 52);
            this.tbEndTimeScale.Name = "tbEndTimeScale";
            this.tbEndTimeScale.Size = new System.Drawing.Size(60, 20);
            this.tbEndTimeScale.TabIndex = 4;
            this.ToolTip1.SetToolTip(this.tbEndTimeScale,
                                     "The end of the time scale. Only used when FullTimeScale is true.");
            //
            //Label7
            //
            this.Label7.Location = new System.Drawing.Point(124, 56);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(32, 16);
            this.Label7.TabIndex = 3;
            this.Label7.Text = "End:";
            //
            //tbStartTimeScale
            //
            this.tbStartTimeScale.Location = new System.Drawing.Point(52, 52);
            this.tbStartTimeScale.Name = "tbStartTimeScale";
            this.tbStartTimeScale.Size = new System.Drawing.Size(60, 20);
            this.tbStartTimeScale.TabIndex = 2;
            this.ToolTip1.SetToolTip(this.tbStartTimeScale,
                                     "The start of the time scale. Only used when FullTimeScale is true.");
            //
            //Label4
            //
            this.Label4.Location = new System.Drawing.Point(12, 56);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(36, 16);
            this.Label4.TabIndex = 1;
            this.Label4.Text = "Start:";
            //
            //cbFullTimeScale
            //
            this.cbFullTimeScale.Location = new System.Drawing.Point(12, 24);
            this.cbFullTimeScale.Name = "cbFullTimeScale";
            this.cbFullTimeScale.Size = new System.Drawing.Size(148, 24);
            this.cbFullTimeScale.TabIndex = 0;
            this.cbFullTimeScale.Text = "Use full time scale";
            this.ToolTip1.SetToolTip(this.cbFullTimeScale,
                                     "If true, show a full time scale. If false, show only the occurring values in the " +
                                     "data source.");
            //
            //GroupBox3
            //
            this.GroupBox3.Controls.Add(this.rbVertical);
            this.GroupBox3.Controls.Add(this.rbHorizontal);
            this.GroupBox3.Controls.Add(this.cbShowValueMarks);
            this.GroupBox3.Location = new System.Drawing.Point(8, 309);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Size = new System.Drawing.Size(376, 40);
            this.GroupBox3.TabIndex = 3;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Layout";
            //
            //rbVertical
            //
            this.rbVertical.Location = new System.Drawing.Point(96, 16);
            this.rbVertical.Name = "rbVertical";
            this.rbVertical.Size = new System.Drawing.Size(68, 20);
            this.rbVertical.TabIndex = 1;
            this.rbVertical.Text = "Vertical";
            this.ToolTip1.SetToolTip(this.rbVertical, "The direction in which the item ranges are shown.");
            //
            //rbHorizontal
            //
            this.rbHorizontal.Location = new System.Drawing.Point(16, 16);
            this.rbHorizontal.Name = "rbHorizontal";
            this.rbHorizontal.Size = new System.Drawing.Size(76, 20);
            this.rbHorizontal.TabIndex = 0;
            this.rbHorizontal.Text = "Horizontal";
            this.ToolTip1.SetToolTip(this.rbHorizontal, "The direction in which the item ranges are shown.");
            //
            //cbShowValueMarks
            //
            this.cbShowValueMarks.Location = new System.Drawing.Point(254, 15);
            this.cbShowValueMarks.Name = "cbShowValueMarks";
            this.cbShowValueMarks.Size = new System.Drawing.Size(116, 24);
            this.cbShowValueMarks.TabIndex = 7;
            this.cbShowValueMarks.Text = "Show value marks";
            this.ToolTip1.SetToolTip(this.cbShowValueMarks, "If true, lines will be added to mark range values.");
            //
            //tbNumberOfDays
            //
            this.tbNumberOfDays.Location = new System.Drawing.Point(122, 251);
            this.tbNumberOfDays.Name = "tbNumberOfDays";
            this.tbNumberOfDays.Size = new System.Drawing.Size(45, 20);
            this.tbNumberOfDays.TabIndex = 8;
            this.ToolTip1.SetToolTip(this.tbNumberOfDays, "The number of days to show in one repetition");
            //
            //cbStartDay
            //
            this.cbStartDay.Items.AddRange(new object[]
                                               {
                                                   "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday",
                                                   "Saturday"
                                               });
            this.cbStartDay.Location = new System.Drawing.Point(69, 278);
            this.cbStartDay.Name = "cbStartDay";
            this.cbStartDay.Size = new System.Drawing.Size(121, 21);
            this.cbStartDay.TabIndex = 11;
            this.ToolTip1.SetToolTip(this.cbStartDay,
                                     "Day of the week to start the calendar with. Only used when Number of days equals " +
                                     "7.");
            //
            //cbEnableEmptySlotClick
            //
            this.cbEnableEmptySlotClick.Location = new System.Drawing.Point(13, 19);
            this.cbEnableEmptySlotClick.Name = "cbEnableEmptySlotClick";
            this.cbEnableEmptySlotClick.Size = new System.Drawing.Size(94, 17);
            this.cbEnableEmptySlotClick.TabIndex = 0;
            this.cbEnableEmptySlotClick.Text = "Enable clicking";
            this.ToolTip1.SetToolTip(this.cbEnableEmptySlotClick,
                                     "When checked, clicking an empty slot in the calendar will raize the EmptySlotClic" +
                                     "ked event");
            //
            //Label12
            //
            this.Label12.Location = new System.Drawing.Point(9, 254);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(107, 20);
            this.Label12.TabIndex = 9;
            this.Label12.Text = "Number of days:";
            this.Label12.TextAlign = System.Drawing.ContentAlignment.TopRight;
            //
            //Label13
            //
            this.Label13.Location = new System.Drawing.Point(9, 279);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(54, 20);
            this.Label13.TabIndex = 10;
            this.Label13.Text = "Start on:";
            this.Label13.TextAlign = System.Drawing.ContentAlignment.TopRight;
            //
            //groupBox4
            //
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.tbEmptySlotToolTip);
            this.groupBox4.Controls.Add(this.cbEnableEmptySlotClick);
            this.groupBox4.Location = new System.Drawing.Point(8, 441);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(376, 45);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Empty slots";
            //
            //label14
            //
            this.label14.Location = new System.Drawing.Point(124, 20);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(42, 16);
            this.label14.TabIndex = 6;
            this.label14.Text = "Tooltip:";
            //
            //tbEmptySlotToolTip
            //
            this.tbEmptySlotToolTip.Location = new System.Drawing.Point(172, 16);
            this.tbEmptySlotToolTip.Name = "tbEmptySlotToolTip";
            this.tbEmptySlotToolTip.Size = new System.Drawing.Size(198, 20);
            this.tbEmptySlotToolTip.TabIndex = 1;
            //
            //CalendarPropertyBuilder
            //
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(394, 520);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.cbStartDay);
            this.Controls.Add(this.Label13);
            this.Controls.Add(this.Label12);
            this.Controls.Add(this.tbNumberOfDays);
            this.Controls.Add(this.GroupBox3);
            this.Controls.Add(this.GroupBox2);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this.tbNumberOfRepetitions);
            this.Controls.Add(this.cbIncludeEndValue);
            this.Controls.Add(this.Label9);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "CalendarPropertyBuilder";
            this.Text = "Properties";
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
            this.GroupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}