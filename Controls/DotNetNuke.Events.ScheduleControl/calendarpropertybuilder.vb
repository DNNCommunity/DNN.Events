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

''' -----------------------------------------------------------------------------
''' Project	 : schedule
''' Class	 : CalendarPropertyBuilder
'''
''' -----------------------------------------------------------------------------
''' <summary>
''' CalendarPropertyBuilder provides an easy interface to set all the properties
''' for the ScheduleCalendar control.
''' </summary>
''' -----------------------------------------------------------------------------
    Public Class CalendarPropertyBuilder
        Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

        Public Sub New()
            MyBase.New()

            'This call is required by the Windows Form Designer.
            InitializeComponent()

            'Add any initialization after the InitializeComponent() call

        End Sub

        'Form overrides dispose to clean up the component list.
        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                If Not (components Is Nothing) Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        Friend WithEvents btnOK As System.Windows.Forms.Button
        Friend WithEvents btnCancel As System.Windows.Forms.Button
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents tbDateField As System.Windows.Forms.TextBox
        Friend WithEvents tbStartTimeField As System.Windows.Forms.TextBox
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents tbEndTimeField As System.Windows.Forms.TextBox
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents cbIncludeEndValue As System.Windows.Forms.CheckBox
        Friend WithEvents Label5 As System.Windows.Forms.Label
        Friend WithEvents Label6 As System.Windows.Forms.Label
        Friend WithEvents tbTimeFormatString As System.Windows.Forms.TextBox
        Friend WithEvents tbDateFormatString As System.Windows.Forms.TextBox
        Friend WithEvents cbTimeFieldsContainDate As System.Windows.Forms.CheckBox
        Friend WithEvents tbNumberOfRepetitions As System.Windows.Forms.TextBox
        Friend WithEvents Label9 As System.Windows.Forms.Label
        Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
        Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
        Friend WithEvents cbFullTimeScale As System.Windows.Forms.CheckBox
        Friend WithEvents tbStartTimeScale As System.Windows.Forms.TextBox
        Friend WithEvents Label4 As System.Windows.Forms.Label
        Friend WithEvents tbEndTimeScale As System.Windows.Forms.TextBox
        Friend WithEvents Label7 As System.Windows.Forms.Label
        Friend WithEvents tbInterval As System.Windows.Forms.TextBox
        Friend WithEvents Label8 As System.Windows.Forms.Label
        Friend WithEvents Label10 As System.Windows.Forms.Label
        Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
        Friend WithEvents rbHorizontal As System.Windows.Forms.RadioButton
        Friend WithEvents rbVertical As System.Windows.Forms.RadioButton
        Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
        Friend WithEvents Label11 As System.Windows.Forms.Label
        Friend WithEvents cbShowValueMarks As System.Windows.Forms.CheckBox
        Friend WithEvents tbNumberOfDays As System.Windows.Forms.TextBox
        Friend WithEvents Label12 As System.Windows.Forms.Label
        Friend WithEvents Label13 As System.Windows.Forms.Label
        Friend WithEvents cbStartDay As System.Windows.Forms.ComboBox
        Private WithEvents groupBox4 As System.Windows.Forms.GroupBox
        Friend WithEvents label14 As System.Windows.Forms.Label
        Friend WithEvents tbEmptySlotToolTip As System.Windows.Forms.TextBox
        Friend WithEvents cbEnableEmptySlotClick As System.Windows.Forms.CheckBox
        Friend WithEvents tbItemStyleField As System.Windows.Forms.TextBox
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Me.btnOK = New System.Windows.Forms.Button
            Me.btnCancel = New System.Windows.Forms.Button
            Me.Label1 = New System.Windows.Forms.Label
            Me.tbDateField = New System.Windows.Forms.TextBox
            Me.tbStartTimeField = New System.Windows.Forms.TextBox
            Me.Label2 = New System.Windows.Forms.Label
            Me.tbEndTimeField = New System.Windows.Forms.TextBox
            Me.Label3 = New System.Windows.Forms.Label
            Me.cbIncludeEndValue = New System.Windows.Forms.CheckBox
            Me.cbTimeFieldsContainDate = New System.Windows.Forms.CheckBox
            Me.tbNumberOfRepetitions = New System.Windows.Forms.TextBox
            Me.Label9 = New System.Windows.Forms.Label
            Me.tbTimeFormatString = New System.Windows.Forms.TextBox
            Me.Label5 = New System.Windows.Forms.Label
            Me.tbDateFormatString = New System.Windows.Forms.TextBox
            Me.Label6 = New System.Windows.Forms.Label
            Me.GroupBox1 = New System.Windows.Forms.GroupBox
            Me.Label11 = New System.Windows.Forms.Label
            Me.tbItemStyleField = New System.Windows.Forms.TextBox
            Me.GroupBox2 = New System.Windows.Forms.GroupBox
            Me.Label10 = New System.Windows.Forms.Label
            Me.tbInterval = New System.Windows.Forms.TextBox
            Me.Label8 = New System.Windows.Forms.Label
            Me.tbEndTimeScale = New System.Windows.Forms.TextBox
            Me.Label7 = New System.Windows.Forms.Label
            Me.tbStartTimeScale = New System.Windows.Forms.TextBox
            Me.Label4 = New System.Windows.Forms.Label
            Me.cbFullTimeScale = New System.Windows.Forms.CheckBox
            Me.GroupBox3 = New System.Windows.Forms.GroupBox
            Me.rbVertical = New System.Windows.Forms.RadioButton
            Me.rbHorizontal = New System.Windows.Forms.RadioButton
            Me.cbShowValueMarks = New System.Windows.Forms.CheckBox
            Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
            Me.tbNumberOfDays = New System.Windows.Forms.TextBox
            Me.cbStartDay = New System.Windows.Forms.ComboBox
            Me.cbEnableEmptySlotClick = New System.Windows.Forms.CheckBox
            Me.Label12 = New System.Windows.Forms.Label
            Me.Label13 = New System.Windows.Forms.Label
            Me.groupBox4 = New System.Windows.Forms.GroupBox
            Me.label14 = New System.Windows.Forms.Label
            Me.tbEmptySlotToolTip = New System.Windows.Forms.TextBox
            Me.GroupBox1.SuspendLayout()
            Me.GroupBox2.SuspendLayout()
            Me.GroupBox3.SuspendLayout()
            Me.groupBox4.SuspendLayout()
            Me.SuspendLayout()
            '
            'btnOK
            '
            Me.btnOK.Location = New System.Drawing.Point(260, 492)
            Me.btnOK.Name = "btnOK"
            Me.btnOK.Size = New System.Drawing.Size(60, 23)
            Me.btnOK.TabIndex = 5
            Me.btnOK.Text = "OK"
            '
            'btnCancel
            '
            Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.btnCancel.Location = New System.Drawing.Point(324, 492)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(60, 23)
            Me.btnCancel.TabIndex = 6
            Me.btnCancel.Text = "Cancel"
            '
            'Label1
            '
            Me.Label1.Location = New System.Drawing.Point(12, 56)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(64, 16)
            Me.Label1.TabIndex = 0
            Me.Label1.Text = "Date field:"
            '
            'tbDateField
            '
            Me.tbDateField.Location = New System.Drawing.Point(12, 72)
            Me.tbDateField.Name = "tbDateField"
            Me.tbDateField.Size = New System.Drawing.Size(168, 20)
            Me.tbDateField.TabIndex = 1
            Me.ToolTip1.SetToolTip(Me.tbDateField, "The database field providing the dates. Ignored when TimeFieldsContainDate=true. " & _
                    "When TimeFieldsContainDate=false, this field should be of type Date.")
            '
            'tbStartTimeField
            '
            Me.tbStartTimeField.Location = New System.Drawing.Point(12, 116)
            Me.tbStartTimeField.Name = "tbStartTimeField"
            Me.tbStartTimeField.Size = New System.Drawing.Size(168, 20)
            Me.tbStartTimeField.TabIndex = 5
            Me.ToolTip1.SetToolTip(Me.tbStartTimeField, "The database field containing the start time of the events. This field should als" & _
                    "o contain the date when TimeFieldsContainDate=true")
            '
            'Label2
            '
            Me.Label2.Location = New System.Drawing.Point(12, 100)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(84, 16)
            Me.Label2.TabIndex = 4
            Me.Label2.Text = "Start time field:"
            '
            'tbEndTimeField
            '
            Me.tbEndTimeField.Location = New System.Drawing.Point(12, 160)
            Me.tbEndTimeField.Name = "tbEndTimeField"
            Me.tbEndTimeField.Size = New System.Drawing.Size(168, 20)
            Me.tbEndTimeField.TabIndex = 9
            Me.ToolTip1.SetToolTip(Me.tbEndTimeField, "The database field containing the end time of the events. This field should also " & _
                    "contain the date when TimeFieldsContainDate=true")
            '
            'Label3
            '
            Me.Label3.Location = New System.Drawing.Point(12, 144)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(84, 12)
            Me.Label3.TabIndex = 8
            Me.Label3.Text = "End time field:"
            '
            'cbIncludeEndValue
            '
            Me.cbIncludeEndValue.Location = New System.Drawing.Point(268, 279)
            Me.cbIncludeEndValue.Name = "cbIncludeEndValue"
            Me.cbIncludeEndValue.Size = New System.Drawing.Size(116, 24)
            Me.cbIncludeEndValue.TabIndex = 2
            Me.cbIncludeEndValue.Text = "Include end value"
            Me.ToolTip1.SetToolTip(Me.cbIncludeEndValue, "If true, the event is shown including the end row or column")
            '
            'cbTimeFieldsContainDate
            '
            Me.cbTimeFieldsContainDate.Location = New System.Drawing.Point(16, 24)
            Me.cbTimeFieldsContainDate.Name = "cbTimeFieldsContainDate"
            Me.cbTimeFieldsContainDate.Size = New System.Drawing.Size(164, 24)
            Me.cbTimeFieldsContainDate.TabIndex = 13
            Me.cbTimeFieldsContainDate.Text = "Time fields contain date"
            Me.ToolTip1.SetToolTip(Me.cbTimeFieldsContainDate, "Indicates whether the time fields (StartTimeField and EndTimeField) contain the d" & _
                    "ate as well. When true, this allows midnight spanning for calendar events. When " & _
                    "false, the DateField contains the date.")
            '
            'tbNumberOfRepetitions
            '
            Me.tbNumberOfRepetitions.Location = New System.Drawing.Point(340, 253)
            Me.tbNumberOfRepetitions.Name = "tbNumberOfRepetitions"
            Me.tbNumberOfRepetitions.Size = New System.Drawing.Size(44, 20)
            Me.tbNumberOfRepetitions.TabIndex = 1
            Me.ToolTip1.SetToolTip(Me.tbNumberOfRepetitions, "The NumberOfRepetitions to show at a time. Only used in Vertical layout.")
            '
            'Label9
            '
            Me.Label9.Location = New System.Drawing.Point(203, 255)
            Me.Label9.Name = "Label9"
            Me.Label9.Size = New System.Drawing.Size(131, 16)
            Me.Label9.TabIndex = 1
            Me.Label9.Text = "Number of repetitions:"
            Me.Label9.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'tbTimeFormatString
            '
            Me.tbTimeFormatString.Location = New System.Drawing.Point(192, 116)
            Me.tbTimeFormatString.Name = "tbTimeFormatString"
            Me.tbTimeFormatString.Size = New System.Drawing.Size(168, 20)
            Me.tbTimeFormatString.TabIndex = 7
            Me.ToolTip1.SetToolTip(Me.tbTimeFormatString, "The format used for the times if the TimeTemplate is missing, e.g. {0:hh:mm}")
            '
            'Label5
            '
            Me.Label5.Location = New System.Drawing.Point(192, 100)
            Me.Label5.Name = "Label5"
            Me.Label5.Size = New System.Drawing.Size(120, 16)
            Me.Label5.TabIndex = 6
            Me.Label5.Text = "Time format string:"
            '
            'tbDateFormatString
            '
            Me.tbDateFormatString.Location = New System.Drawing.Point(192, 72)
            Me.tbDateFormatString.Name = "tbDateFormatString"
            Me.tbDateFormatString.Size = New System.Drawing.Size(168, 20)
            Me.tbDateFormatString.TabIndex = 3
            Me.ToolTip1.SetToolTip(Me.tbDateFormatString, "The format used for the date if the DateTemplate is missing, e.g. {0:ddd d}")
            '
            'Label6
            '
            Me.Label6.Location = New System.Drawing.Point(192, 56)
            Me.Label6.Name = "Label6"
            Me.Label6.Size = New System.Drawing.Size(120, 16)
            Me.Label6.TabIndex = 2
            Me.Label6.Text = "Date format string:"
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.Label11)
            Me.GroupBox1.Controls.Add(Me.tbItemStyleField)
            Me.GroupBox1.Controls.Add(Me.Label1)
            Me.GroupBox1.Controls.Add(Me.tbDateField)
            Me.GroupBox1.Controls.Add(Me.tbDateFormatString)
            Me.GroupBox1.Controls.Add(Me.Label6)
            Me.GroupBox1.Controls.Add(Me.Label3)
            Me.GroupBox1.Controls.Add(Me.tbStartTimeField)
            Me.GroupBox1.Controls.Add(Me.tbTimeFormatString)
            Me.GroupBox1.Controls.Add(Me.Label5)
            Me.GroupBox1.Controls.Add(Me.Label2)
            Me.GroupBox1.Controls.Add(Me.tbEndTimeField)
            Me.GroupBox1.Controls.Add(Me.cbTimeFieldsContainDate)
            Me.GroupBox1.Location = New System.Drawing.Point(8, 12)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.Size = New System.Drawing.Size(376, 232)
            Me.GroupBox1.TabIndex = 0
            Me.GroupBox1.TabStop = False
            Me.GroupBox1.Text = "Data source fields"
            '
            'Label11
            '
            Me.Label11.Location = New System.Drawing.Point(12, 188)
            Me.Label11.Name = "Label11"
            Me.Label11.Size = New System.Drawing.Size(84, 12)
            Me.Label11.TabIndex = 14
            Me.Label11.Text = "Item style field:"
            '
            'tbItemStyleField
            '
            Me.tbItemStyleField.Location = New System.Drawing.Point(12, 204)
            Me.tbItemStyleField.Name = "tbItemStyleField"
            Me.tbItemStyleField.Size = New System.Drawing.Size(168, 20)
            Me.tbItemStyleField.TabIndex = 15
            Me.ToolTip1.SetToolTip(Me.tbItemStyleField, "An optional database field providing the item styles (in the form of a css class " & _
                    "name)")
            '
            'GroupBox2
            '
            Me.GroupBox2.Controls.Add(Me.Label10)
            Me.GroupBox2.Controls.Add(Me.tbInterval)
            Me.GroupBox2.Controls.Add(Me.Label8)
            Me.GroupBox2.Controls.Add(Me.tbEndTimeScale)
            Me.GroupBox2.Controls.Add(Me.Label7)
            Me.GroupBox2.Controls.Add(Me.tbStartTimeScale)
            Me.GroupBox2.Controls.Add(Me.Label4)
            Me.GroupBox2.Controls.Add(Me.cbFullTimeScale)
            Me.GroupBox2.Location = New System.Drawing.Point(8, 353)
            Me.GroupBox2.Name = "GroupBox2"
            Me.GroupBox2.Size = New System.Drawing.Size(376, 84)
            Me.GroupBox2.TabIndex = 4
            Me.GroupBox2.TabStop = False
            Me.GroupBox2.Text = "Full time scale"
            '
            'Label10
            '
            Me.Label10.Location = New System.Drawing.Point(328, 56)
            Me.Label10.Name = "Label10"
            Me.Label10.Size = New System.Drawing.Size(24, 16)
            Me.Label10.TabIndex = 7
            Me.Label10.Text = "min."
            '
            'tbInterval
            '
            Me.tbInterval.Location = New System.Drawing.Point(284, 52)
            Me.tbInterval.Name = "tbInterval"
            Me.tbInterval.Size = New System.Drawing.Size(40, 20)
            Me.tbInterval.TabIndex = 6
            Me.ToolTip1.SetToolTip(Me.tbInterval, "The number of minutes between each mark on the time scale. Only used when FullTim" & _
                    "eScale is true.")
            '
            'Label8
            '
            Me.Label8.Location = New System.Drawing.Point(232, 56)
            Me.Label8.Name = "Label8"
            Me.Label8.Size = New System.Drawing.Size(48, 16)
            Me.Label8.TabIndex = 5
            Me.Label8.Text = "Interval:"
            '
            'tbEndTimeScale
            '
            Me.tbEndTimeScale.Location = New System.Drawing.Point(160, 52)
            Me.tbEndTimeScale.Name = "tbEndTimeScale"
            Me.tbEndTimeScale.Size = New System.Drawing.Size(60, 20)
            Me.tbEndTimeScale.TabIndex = 4
            Me.ToolTip1.SetToolTip(Me.tbEndTimeScale, "The end of the time scale. Only used when FullTimeScale is true.")
            '
            'Label7
            '
            Me.Label7.Location = New System.Drawing.Point(124, 56)
            Me.Label7.Name = "Label7"
            Me.Label7.Size = New System.Drawing.Size(32, 16)
            Me.Label7.TabIndex = 3
            Me.Label7.Text = "End:"
            '
            'tbStartTimeScale
            '
            Me.tbStartTimeScale.Location = New System.Drawing.Point(52, 52)
            Me.tbStartTimeScale.Name = "tbStartTimeScale"
            Me.tbStartTimeScale.Size = New System.Drawing.Size(60, 20)
            Me.tbStartTimeScale.TabIndex = 2
            Me.ToolTip1.SetToolTip(Me.tbStartTimeScale, "The start of the time scale. Only used when FullTimeScale is true.")
            '
            'Label4
            '
            Me.Label4.Location = New System.Drawing.Point(12, 56)
            Me.Label4.Name = "Label4"
            Me.Label4.Size = New System.Drawing.Size(36, 16)
            Me.Label4.TabIndex = 1
            Me.Label4.Text = "Start:"
            '
            'cbFullTimeScale
            '
            Me.cbFullTimeScale.Location = New System.Drawing.Point(12, 24)
            Me.cbFullTimeScale.Name = "cbFullTimeScale"
            Me.cbFullTimeScale.Size = New System.Drawing.Size(148, 24)
            Me.cbFullTimeScale.TabIndex = 0
            Me.cbFullTimeScale.Text = "Use full time scale"
            Me.ToolTip1.SetToolTip(Me.cbFullTimeScale, "If true, show a full time scale. If false, show only the occurring values in the " & _
                    "data source.")
            '
            'GroupBox3
            '
            Me.GroupBox3.Controls.Add(Me.rbVertical)
            Me.GroupBox3.Controls.Add(Me.rbHorizontal)
            Me.GroupBox3.Controls.Add(Me.cbShowValueMarks)
            Me.GroupBox3.Location = New System.Drawing.Point(8, 309)
            Me.GroupBox3.Name = "GroupBox3"
            Me.GroupBox3.Size = New System.Drawing.Size(376, 40)
            Me.GroupBox3.TabIndex = 3
            Me.GroupBox3.TabStop = False
            Me.GroupBox3.Text = "Layout"
            '
            'rbVertical
            '
            Me.rbVertical.Location = New System.Drawing.Point(96, 16)
            Me.rbVertical.Name = "rbVertical"
            Me.rbVertical.Size = New System.Drawing.Size(68, 20)
            Me.rbVertical.TabIndex = 1
            Me.rbVertical.Text = "Vertical"
            Me.ToolTip1.SetToolTip(Me.rbVertical, "The direction in which the item ranges are shown.")
            '
            'rbHorizontal
            '
            Me.rbHorizontal.Location = New System.Drawing.Point(16, 16)
            Me.rbHorizontal.Name = "rbHorizontal"
            Me.rbHorizontal.Size = New System.Drawing.Size(76, 20)
            Me.rbHorizontal.TabIndex = 0
            Me.rbHorizontal.Text = "Horizontal"
            Me.ToolTip1.SetToolTip(Me.rbHorizontal, "The direction in which the item ranges are shown.")
            '
            'cbShowValueMarks
            '
            Me.cbShowValueMarks.Location = New System.Drawing.Point(254, 15)
            Me.cbShowValueMarks.Name = "cbShowValueMarks"
            Me.cbShowValueMarks.Size = New System.Drawing.Size(116, 24)
            Me.cbShowValueMarks.TabIndex = 7
            Me.cbShowValueMarks.Text = "Show value marks"
            Me.ToolTip1.SetToolTip(Me.cbShowValueMarks, "If true, lines will be added to mark range values.")
            '
            'tbNumberOfDays
            '
            Me.tbNumberOfDays.Location = New System.Drawing.Point(122, 251)
            Me.tbNumberOfDays.Name = "tbNumberOfDays"
            Me.tbNumberOfDays.Size = New System.Drawing.Size(45, 20)
            Me.tbNumberOfDays.TabIndex = 8
            Me.ToolTip1.SetToolTip(Me.tbNumberOfDays, "The number of days to show in one repetition")
            '
            'cbStartDay
            '
            Me.cbStartDay.Items.AddRange(New Object() {"Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"})
            Me.cbStartDay.Location = New System.Drawing.Point(69, 278)
            Me.cbStartDay.Name = "cbStartDay"
            Me.cbStartDay.Size = New System.Drawing.Size(121, 21)
            Me.cbStartDay.TabIndex = 11
            Me.ToolTip1.SetToolTip(Me.cbStartDay, "Day of the week to start the calendar with. Only used when Number of days equals " & _
                    "7.")
            '
            'cbEnableEmptySlotClick
            '
            Me.cbEnableEmptySlotClick.Location = New System.Drawing.Point(13, 19)
            Me.cbEnableEmptySlotClick.Name = "cbEnableEmptySlotClick"
            Me.cbEnableEmptySlotClick.Size = New System.Drawing.Size(94, 17)
            Me.cbEnableEmptySlotClick.TabIndex = 0
            Me.cbEnableEmptySlotClick.Text = "Enable clicking"
            Me.ToolTip1.SetToolTip(Me.cbEnableEmptySlotClick, "When checked, clicking an empty slot in the calendar will raize the EmptySlotClic" & _
                    "ked event")
            '
            'Label12
            '
            Me.Label12.Location = New System.Drawing.Point(9, 254)
            Me.Label12.Name = "Label12"
            Me.Label12.Size = New System.Drawing.Size(107, 20)
            Me.Label12.TabIndex = 9
            Me.Label12.Text = "Number of days:"
            Me.Label12.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'Label13
            '
            Me.Label13.Location = New System.Drawing.Point(9, 279)
            Me.Label13.Name = "Label13"
            Me.Label13.Size = New System.Drawing.Size(54, 20)
            Me.Label13.TabIndex = 10
            Me.Label13.Text = "Start on:"
            Me.Label13.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'groupBox4
            '
            Me.groupBox4.Controls.Add(Me.label14)
            Me.groupBox4.Controls.Add(Me.tbEmptySlotToolTip)
            Me.groupBox4.Controls.Add(Me.cbEnableEmptySlotClick)
            Me.groupBox4.Location = New System.Drawing.Point(8, 441)
            Me.groupBox4.Name = "groupBox4"
            Me.groupBox4.Size = New System.Drawing.Size(376, 45)
            Me.groupBox4.TabIndex = 8
            Me.groupBox4.TabStop = False
            Me.groupBox4.Text = "Empty slots"
            '
            'label14
            '
            Me.label14.Location = New System.Drawing.Point(124, 20)
            Me.label14.Name = "label14"
            Me.label14.Size = New System.Drawing.Size(42, 16)
            Me.label14.TabIndex = 6
            Me.label14.Text = "Tooltip:"
            '
            'tbEmptySlotToolTip
            '
            Me.tbEmptySlotToolTip.Location = New System.Drawing.Point(172, 16)
            Me.tbEmptySlotToolTip.Name = "tbEmptySlotToolTip"
            Me.tbEmptySlotToolTip.Size = New System.Drawing.Size(198, 20)
            Me.tbEmptySlotToolTip.TabIndex = 1
            '
            'CalendarPropertyBuilder
            '
            Me.AcceptButton = Me.btnOK
            Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
            Me.CancelButton = Me.btnCancel
            Me.ClientSize = New System.Drawing.Size(394, 520)
            Me.ControlBox = False
            Me.Controls.Add(Me.groupBox4)
            Me.Controls.Add(Me.cbStartDay)
            Me.Controls.Add(Me.Label13)
            Me.Controls.Add(Me.Label12)
            Me.Controls.Add(Me.tbNumberOfDays)
            Me.Controls.Add(Me.GroupBox3)
            Me.Controls.Add(Me.GroupBox2)
            Me.Controls.Add(Me.GroupBox1)
            Me.Controls.Add(Me.tbNumberOfRepetitions)
            Me.Controls.Add(Me.cbIncludeEndValue)
            Me.Controls.Add(Me.Label9)
            Me.Controls.Add(Me.btnCancel)
            Me.Controls.Add(Me.btnOK)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.Name = "CalendarPropertyBuilder"
            Me.Text = "Properties"
            Me.GroupBox1.ResumeLayout(False)
            Me.GroupBox1.PerformLayout()
            Me.GroupBox2.ResumeLayout(False)
            Me.GroupBox2.PerformLayout()
            Me.GroupBox3.ResumeLayout(False)
            Me.groupBox4.ResumeLayout(False)
            Me.groupBox4.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

#End Region

        Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        Me.DialogResult = Windows.Forms.DialogResult.OK
            Close()
        End Sub

        Private Sub cbFullTimeScale_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbFullTimeScale.CheckedChanged
            tbStartTimeScale.Enabled = cbFullTimeScale.Checked
            tbEndTimeScale.Enabled = cbFullTimeScale.Checked
            tbInterval.Enabled = cbFullTimeScale.Checked
        End Sub

        Private Sub cbTimeFieldsContainDate_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbTimeFieldsContainDate.CheckedChanged
            tbDateField.Enabled = Not cbTimeFieldsContainDate.Checked
        End Sub

        Private Sub rbHorizontal_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbHorizontal.CheckedChanged
            If (rbHorizontal.Checked) Then
                tbNumberOfRepetitions.Text = "1"
                tbNumberOfRepetitions.Enabled = False
            Else
                tbNumberOfRepetitions.Enabled = True
            End If
        End Sub

        Private Sub cbShowValueMarks_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbShowValueMarks.CheckedChanged
            If (cbShowValueMarks.Checked) Then cbIncludeEndValue.Checked = False
        End Sub

        Private Sub tbNumberOfDays_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbNumberOfDays.TextChanged
            cbStartDay.Enabled = (tbNumberOfDays.Text = "7") ' startday is only used on weekly calendar
        End Sub

        Private Sub cbEnableEmptySlotClick_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbEnableEmptySlotClick.CheckedChanged
            Me.tbEmptySlotToolTip.Enabled = Me.cbEnableEmptySlotClick.Checked
        End Sub
    End Class

