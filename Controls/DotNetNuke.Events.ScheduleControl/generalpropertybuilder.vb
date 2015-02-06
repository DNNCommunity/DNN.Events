'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2007
' by DotNetNuke Corporation
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
''' Class	 : GeneralPropertyBuilder
'''
''' -----------------------------------------------------------------------------
''' <summary>
''' GeneralPropertyBuilder provides an easy interface to set all the properties
''' for the GeneralCalendar control.
''' </summary>
''' -----------------------------------------------------------------------------
    Public Class GeneralPropertyBuilder
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
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents cbIncludeEndValue As System.Windows.Forms.CheckBox
        Friend WithEvents Label5 As System.Windows.Forms.Label
        Friend WithEvents Label6 As System.Windows.Forms.Label
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
        Friend WithEvents Label9 As System.Windows.Forms.Label
        Friend WithEvents tbTitleField As System.Windows.Forms.TextBox
        Friend WithEvents cbSeparateDateHeader As System.Windows.Forms.CheckBox
        Friend WithEvents tbTitleFormatString As System.Windows.Forms.TextBox
        Friend WithEvents tbDataRangeStartField As System.Windows.Forms.TextBox
        Friend WithEvents tbDataRangeEndField As System.Windows.Forms.TextBox
        Friend WithEvents tbRangeDataFormatString As System.Windows.Forms.TextBox
        Friend WithEvents tbDateHeaderFormatString As System.Windows.Forms.TextBox
        Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
        Friend WithEvents Label11 As System.Windows.Forms.Label
        Friend WithEvents cbShowValueMarks As System.Windows.Forms.CheckBox
        Friend WithEvents cbAutoSortTitles As System.Windows.Forms.CheckBox
        Friend WithEvents groupBox4 As System.Windows.Forms.GroupBox
        Friend WithEvents label14 As System.Windows.Forms.Label
        Friend WithEvents tbEmptySlotToolTip As System.Windows.Forms.TextBox
        Friend WithEvents cbEnableEmptySlotClick As System.Windows.Forms.CheckBox
        Friend WithEvents tbItemStyleField As System.Windows.Forms.TextBox
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Me.btnOK = New System.Windows.Forms.Button
            Me.btnCancel = New System.Windows.Forms.Button
            Me.Label1 = New System.Windows.Forms.Label
            Me.tbTitleField = New System.Windows.Forms.TextBox
            Me.tbDataRangeStartField = New System.Windows.Forms.TextBox
            Me.Label2 = New System.Windows.Forms.Label
            Me.tbDataRangeEndField = New System.Windows.Forms.TextBox
            Me.Label3 = New System.Windows.Forms.Label
            Me.cbIncludeEndValue = New System.Windows.Forms.CheckBox
            Me.cbSeparateDateHeader = New System.Windows.Forms.CheckBox
            Me.tbRangeDataFormatString = New System.Windows.Forms.TextBox
            Me.Label5 = New System.Windows.Forms.Label
            Me.tbTitleFormatString = New System.Windows.Forms.TextBox
            Me.Label6 = New System.Windows.Forms.Label
            Me.GroupBox1 = New System.Windows.Forms.GroupBox
            Me.cbAutoSortTitles = New System.Windows.Forms.CheckBox
            Me.Label11 = New System.Windows.Forms.Label
            Me.tbItemStyleField = New System.Windows.Forms.TextBox
            Me.tbDateHeaderFormatString = New System.Windows.Forms.TextBox
            Me.Label9 = New System.Windows.Forms.Label
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
            Me.cbShowValueMarks = New System.Windows.Forms.CheckBox
            Me.rbVertical = New System.Windows.Forms.RadioButton
            Me.rbHorizontal = New System.Windows.Forms.RadioButton
            Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
            Me.cbEnableEmptySlotClick = New System.Windows.Forms.CheckBox
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
            Me.btnOK.Location = New System.Drawing.Point(260, 464)
            Me.btnOK.Name = "btnOK"
            Me.btnOK.Size = New System.Drawing.Size(60, 23)
            Me.btnOK.TabIndex = 4
            Me.btnOK.Text = "OK"
            '
            'btnCancel
            '
            Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.btnCancel.Location = New System.Drawing.Point(324, 464)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(60, 23)
            Me.btnCancel.TabIndex = 5
            Me.btnCancel.Text = "Cancel"
            '
            'Label1
            '
            Me.Label1.Location = New System.Drawing.Point(12, 56)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(64, 16)
            Me.Label1.TabIndex = 0
            Me.Label1.Text = "Title field:"
            '
            'tbTitleField
            '
            Me.tbTitleField.Location = New System.Drawing.Point(12, 72)
            Me.tbTitleField.Name = "tbTitleField"
            Me.tbTitleField.Size = New System.Drawing.Size(168, 20)
            Me.tbTitleField.TabIndex = 1
            Me.ToolTip1.SetToolTip(Me.tbTitleField, "The database field providing the titles. In Calendar mode this field should be of" & _
                    " type Date when ")
            '
            'tbDataRangeStartField
            '
            Me.tbDataRangeStartField.Location = New System.Drawing.Point(12, 116)
            Me.tbDataRangeStartField.Name = "tbDataRangeStartField"
            Me.tbDataRangeStartField.Size = New System.Drawing.Size(168, 20)
            Me.tbDataRangeStartField.TabIndex = 5
            Me.ToolTip1.SetToolTip(Me.tbDataRangeStartField, "The database field containing the start of the items.")
            '
            'Label2
            '
            Me.Label2.Location = New System.Drawing.Point(12, 100)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(156, 16)
            Me.Label2.TabIndex = 4
            Me.Label2.Text = "Data range start field:"
            '
            'tbDataRangeEndField
            '
            Me.tbDataRangeEndField.Location = New System.Drawing.Point(12, 160)
            Me.tbDataRangeEndField.Name = "tbDataRangeEndField"
            Me.tbDataRangeEndField.Size = New System.Drawing.Size(168, 20)
            Me.tbDataRangeEndField.TabIndex = 9
            Me.ToolTip1.SetToolTip(Me.tbDataRangeEndField, "The database field containing the end of the items.")
            '
            'Label3
            '
            Me.Label3.Location = New System.Drawing.Point(12, 144)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(152, 12)
            Me.Label3.TabIndex = 8
            Me.Label3.Text = "Data range end field:"
            '
            'cbIncludeEndValue
            '
            Me.cbIncludeEndValue.Location = New System.Drawing.Point(28, 252)
            Me.cbIncludeEndValue.Name = "cbIncludeEndValue"
            Me.cbIncludeEndValue.Size = New System.Drawing.Size(116, 24)
            Me.cbIncludeEndValue.TabIndex = 1
            Me.cbIncludeEndValue.Text = "Include end value"
            Me.ToolTip1.SetToolTip(Me.cbIncludeEndValue, "If true, the event is shown including the end row or column")
            '
            'cbSeparateDateHeader
            '
            Me.cbSeparateDateHeader.Location = New System.Drawing.Point(16, 24)
            Me.cbSeparateDateHeader.Name = "cbSeparateDateHeader"
            Me.cbSeparateDateHeader.Size = New System.Drawing.Size(164, 24)
            Me.cbSeparateDateHeader.TabIndex = 13
            Me.cbSeparateDateHeader.Text = "Separate date header"
            Me.ToolTip1.SetToolTip(Me.cbSeparateDateHeader, "When true, a separate header will be added for the date. This requires DataRangeS" & _
                    "tartField and DataRangeEndField to be of type DateTime.")
            '
            'tbRangeDataFormatString
            '
            Me.tbRangeDataFormatString.Location = New System.Drawing.Point(192, 116)
            Me.tbRangeDataFormatString.Name = "tbRangeDataFormatString"
            Me.tbRangeDataFormatString.Size = New System.Drawing.Size(168, 20)
            Me.tbRangeDataFormatString.TabIndex = 7
            Me.ToolTip1.SetToolTip(Me.tbRangeDataFormatString, "The format used for the ranges if the RangeHeaderTemplate is missing, e.g. {0:hh:" & _
                    "mm}")
            '
            'Label5
            '
            Me.Label5.Location = New System.Drawing.Point(192, 100)
            Me.Label5.Name = "Label5"
            Me.Label5.Size = New System.Drawing.Size(160, 16)
            Me.Label5.TabIndex = 6
            Me.Label5.Text = "Range data format string:"
            '
            'tbTitleFormatString
            '
            Me.tbTitleFormatString.Location = New System.Drawing.Point(192, 72)
            Me.tbTitleFormatString.Name = "tbTitleFormatString"
            Me.tbTitleFormatString.Size = New System.Drawing.Size(168, 20)
            Me.tbTitleFormatString.TabIndex = 3
            Me.ToolTip1.SetToolTip(Me.tbTitleFormatString, "The format used for the title if the TitleTemplate is missing, e.g. {0:ddd d}")
            '
            'Label6
            '
            Me.Label6.Location = New System.Drawing.Point(192, 56)
            Me.Label6.Name = "Label6"
            Me.Label6.Size = New System.Drawing.Size(120, 16)
            Me.Label6.TabIndex = 2
            Me.Label6.Text = "Title format string:"
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.cbAutoSortTitles)
            Me.GroupBox1.Controls.Add(Me.Label11)
            Me.GroupBox1.Controls.Add(Me.tbItemStyleField)
            Me.GroupBox1.Controls.Add(Me.tbDateHeaderFormatString)
            Me.GroupBox1.Controls.Add(Me.Label9)
            Me.GroupBox1.Controls.Add(Me.Label1)
            Me.GroupBox1.Controls.Add(Me.tbTitleField)
            Me.GroupBox1.Controls.Add(Me.tbTitleFormatString)
            Me.GroupBox1.Controls.Add(Me.Label6)
            Me.GroupBox1.Controls.Add(Me.Label3)
            Me.GroupBox1.Controls.Add(Me.tbDataRangeStartField)
            Me.GroupBox1.Controls.Add(Me.tbRangeDataFormatString)
            Me.GroupBox1.Controls.Add(Me.Label5)
            Me.GroupBox1.Controls.Add(Me.Label2)
            Me.GroupBox1.Controls.Add(Me.tbDataRangeEndField)
            Me.GroupBox1.Controls.Add(Me.cbSeparateDateHeader)
            Me.GroupBox1.Location = New System.Drawing.Point(8, 12)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.Size = New System.Drawing.Size(376, 232)
            Me.GroupBox1.TabIndex = 0
            Me.GroupBox1.TabStop = False
            Me.GroupBox1.Text = "Data source fields"
            '
            'cbAutoSortTitles
            '
            Me.cbAutoSortTitles.Location = New System.Drawing.Point(193, 204)
            Me.cbAutoSortTitles.Name = "cbAutoSortTitles"
            Me.cbAutoSortTitles.Size = New System.Drawing.Size(116, 24)
            Me.cbAutoSortTitles.TabIndex = 18
            Me.cbAutoSortTitles.Text = "Auto sort titles"
            Me.ToolTip1.SetToolTip(Me.cbAutoSortTitles, "Sort titles automatically. When false, make sure the data source items are proper" & _
                    "ly sorted in advance")
            '
            'Label11
            '
            Me.Label11.Location = New System.Drawing.Point(12, 188)
            Me.Label11.Name = "Label11"
            Me.Label11.Size = New System.Drawing.Size(152, 12)
            Me.Label11.TabIndex = 16
            Me.Label11.Text = "Item style field"
            '
            'tbItemStyleField
            '
            Me.tbItemStyleField.Location = New System.Drawing.Point(12, 204)
            Me.tbItemStyleField.Name = "tbItemStyleField"
            Me.tbItemStyleField.Size = New System.Drawing.Size(168, 20)
            Me.tbItemStyleField.TabIndex = 17
            Me.ToolTip1.SetToolTip(Me.tbItemStyleField, "An optional database field providing the item styles (in the form of a css class " & _
                    "name)")
            '
            'tbDateHeaderFormatString
            '
            Me.tbDateHeaderFormatString.Location = New System.Drawing.Point(192, 160)
            Me.tbDateHeaderFormatString.Name = "tbDateHeaderFormatString"
            Me.tbDateHeaderFormatString.Size = New System.Drawing.Size(168, 20)
            Me.tbDateHeaderFormatString.TabIndex = 15
            Me.ToolTip1.SetToolTip(Me.tbDateHeaderFormatString, "The format used for the date header if SeparateDateHeader=true and the DateHeader" & _
                    "Template is missing, e.g. {0:dd/MM}")
            '
            'Label9
            '
            Me.Label9.Location = New System.Drawing.Point(192, 144)
            Me.Label9.Name = "Label9"
            Me.Label9.Size = New System.Drawing.Size(160, 16)
            Me.Label9.TabIndex = 14
            Me.Label9.Text = "Date header format string:"
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
            Me.GroupBox2.Location = New System.Drawing.Point(12, 328)
            Me.GroupBox2.Name = "GroupBox2"
            Me.GroupBox2.Size = New System.Drawing.Size(376, 84)
            Me.GroupBox2.TabIndex = 3
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
            Me.GroupBox3.Controls.Add(Me.cbShowValueMarks)
            Me.GroupBox3.Controls.Add(Me.rbVertical)
            Me.GroupBox3.Controls.Add(Me.rbHorizontal)
            Me.GroupBox3.Location = New System.Drawing.Point(12, 284)
            Me.GroupBox3.Name = "GroupBox3"
            Me.GroupBox3.Size = New System.Drawing.Size(376, 40)
            Me.GroupBox3.TabIndex = 2
            Me.GroupBox3.TabStop = False
            Me.GroupBox3.Text = "Layout"
            '
            'cbShowValueMarks
            '
            Me.cbShowValueMarks.Location = New System.Drawing.Point(256, 14)
            Me.cbShowValueMarks.Name = "cbShowValueMarks"
            Me.cbShowValueMarks.Size = New System.Drawing.Size(116, 24)
            Me.cbShowValueMarks.TabIndex = 6
            Me.cbShowValueMarks.Text = "Show value marks"
            Me.ToolTip1.SetToolTip(Me.cbShowValueMarks, "If true, lines will be added to mark range values.")
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
            'groupBox4
            '
            Me.groupBox4.Controls.Add(Me.label14)
            Me.groupBox4.Controls.Add(Me.tbEmptySlotToolTip)
            Me.groupBox4.Controls.Add(Me.cbEnableEmptySlotClick)
            Me.groupBox4.Location = New System.Drawing.Point(12, 416)
            Me.groupBox4.Name = "groupBox4"
            Me.groupBox4.Size = New System.Drawing.Size(376, 45)
            Me.groupBox4.TabIndex = 9
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
            'GeneralPropertyBuilder
            '
            Me.AcceptButton = Me.btnOK
            Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
            Me.CancelButton = Me.btnCancel
            Me.ClientSize = New System.Drawing.Size(394, 491)
            Me.ControlBox = False
            Me.Controls.Add(Me.groupBox4)
            Me.Controls.Add(Me.GroupBox3)
            Me.Controls.Add(Me.GroupBox2)
            Me.Controls.Add(Me.GroupBox1)
            Me.Controls.Add(Me.cbIncludeEndValue)
            Me.Controls.Add(Me.btnCancel)
            Me.Controls.Add(Me.btnOK)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.Name = "GeneralPropertyBuilder"
            Me.Text = "Properties"
            Me.GroupBox1.ResumeLayout(False)
            Me.GroupBox1.PerformLayout()
            Me.GroupBox2.ResumeLayout(False)
            Me.GroupBox2.PerformLayout()
            Me.GroupBox3.ResumeLayout(False)
            Me.groupBox4.ResumeLayout(False)
            Me.groupBox4.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

#End Region

        Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
            Me.DialogResult = DialogResult.OK
            Close()
        End Sub

        Private Sub cbFullTimeScale_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbFullTimeScale.CheckedChanged
            tbStartTimeScale.Enabled = cbFullTimeScale.Checked
            tbEndTimeScale.Enabled = cbFullTimeScale.Checked
            tbInterval.Enabled = cbFullTimeScale.Checked
        End Sub

        Private Sub cbSeparateDateHeader_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbSeparateDateHeader.CheckedChanged
            tbDateHeaderFormatString.Enabled = cbSeparateDateHeader.Checked
        End Sub

        Private Sub cbShowValueMarks_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbShowValueMarks.CheckedChanged
            If (cbShowValueMarks.Checked) Then cbIncludeEndValue.Checked = False
        End Sub

        Private Sub cbEnableEmptySlotClick_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbEnableEmptySlotClick.CheckedChanged
            Me.tbEmptySlotToolTip.Enabled = Me.cbEnableEmptySlotClick.Checked
        End Sub
    End Class
