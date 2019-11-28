<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditEvents.ascx.cs" Inherits="DotNetNuke.Modules.Events.EditEvents" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/texteditor.ascx" %>
<%@ Register TagPrefix="dnn" TagName="URL" Src="~/controls/URLControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" TagPrefix="dnn" %>
<%@ Register TagPrefix="evt" TagName="AddUser" Src="~/DesktopModules/Events/SubControls/EventUserGrid.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web.Deprecated" Namespace="DotNetNuke.Web.UI.WebControls" %>
<asp:Panel id="pnlEventsModuleEdit" runat="server">
<div class="dnnForm EditEvents" id="EditEvents">
<asp:ValidationSummary ID="valSummary" runat="server" CssClass="dnnFormMessage dnnFormValidationSummary"
                       EnableClientScript="true" DisplayMode="List"/>
<ul class="dnnAdminTabNav">
    <li>
        <a href="#EditBasic"><%= this.LocalizeString("BasicSettings") %></a>
    </li>
    <li>
        <a href="#EditAdvanced"><%= this.LocalizeString("AdvancedSettings") %></a>
    </li>
</ul>
<div class="EditBasic dnnClear" id="EditBasic">
    <fieldset class="dnnClear">
        <div id="divBasic">
            <div class="dnnFormItem">
                <dnn:Label id="lblTitle" cssclass="dnnFormRequired" runat="server"></dnn:Label>
                <asp:TextBox id="txtTitle" cssclass="dnnFormRequired NormalTextBox" runat="server" MaxLength="100" Columns="30" width="250px" Font-Size="8pt"></asp:TextBox>
                <asp:RequiredFieldValidator id="valRequiredTitle" runat="server" cssclass="dnnFormMessage dnnFormError" resourcekey="valRequiredTitle" ControlToValidate="txtTitle"></asp:RequiredFieldValidator>
            </div>
            <div class="dnnFormItem">
                <dnn:Label id="lblAllDayEvent" runat="server"></dnn:Label>
                <asp:CheckBox id="chkAllDayEvent" runat="server" cssclass="SubHead"></asp:CheckBox>
            </div>
            <div class="dnnFormItem">
                <dnn:Label id="lblStartDateTime" runat="server" cssclass="dnnFormRequired"></dnn:Label>
                <div>
                    <dnn:DNNDatePicker id="dpStartDate" runat="server" DateInput-CssClass="DateFormat" CssClass="DatePicker"></dnn:DNNDatePicker>
                    <div id="divStartTime" runat="server" class="TimePicker">
                        <dnn:DNNTimePicker id="tpStartTime" runat="server" DateInput-CssClass="DateFormat"></dnn:DNNTimePicker>
                    </div>
                    <asp:LinkButton id="btnCopyStartdate" resourcekey="btnCopyStartdate" runat="server" text="Copy to enddate." cssclass="CommandButton cmdDatePicker" BorderStyle="none" CausesValidation="False"/>
                    <asp:RequiredFieldValidator id="valRequiredStartDate" runat="server" Display="Dynamic" cssclass="dnnFormMessage dnnFormError" resourcekey="valRequiredStartDate" ControlToValidate="dpStartDate" EnableViewState="false" ValidationGroup="startdate" SetFocusOnError="True"></asp:RequiredFieldValidator>
                    <asp:CustomValidator id="valValidStartDate" runat="server" ControlToValidate="dpStartDate" cssclass="dnnFormMessage dnnFormError"
                                         EnableViewState="false" resourcekey="valValidStartDate" ValidateEmptyText="true">
                    </asp:CustomValidator>
                    <asp:CustomValidator id="valValidRecurStartDate" runat="server" cssclass="dnnFormMessage dnnFormError" resourcekey="valValidRecurStartDate"
                                         ControlToValidate="dpStartDate" Visible="False" EnableViewState="false">
                    </asp:CustomValidator>
                    <asp:CustomValidator id="valValidRecurStartDate2" runat="server" cssclass="dnnFormMessage dnnFormError" resourcekey="valValidRecurStartDate2"
                                         ControlToValidate="dpStartDate" Visible="False" EnableViewState="false">
                    </asp:CustomValidator>
                    <asp:CustomValidator id="valValidStartDate2" runat="server" cssclass="dnnFormMessage dnnFormError"
                                         ControlToValidate="dpStartDate" Visible="False" EnableViewState="false">
                    </asp:CustomValidator>
                    <asp:RequiredFieldValidator id="valRequiredStartTime" runat="server" Display="Dynamic" cssclass="dnnFormMessage dnnFormError" resourcekey="valRequiredStartTime"
                                                ControlToValidate="tpStartTime" EnableViewState="false">
                    </asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="valValidStartTime2" runat="server" SetFocusOnError="true"
                                         ControlToValidate="tpStartTime" ClientValidationFunction="ValidateTime" cssclass="dnnFormMessage dnnFormError" Text="*" OnServerValidate="valValidStartTime2_ServerValidate"/>
                    <asp:CustomValidator id="valConflict" runat="server" cssclass="dnnFormMessage dnnFormError"
                                         ControlToValidate="txtP1Every" Visible="False" EnableViewState="false">
                    </asp:CustomValidator>
                    <asp:CustomValidator id="valLocationConflict" runat="server" cssclass="dnnFormMessage dnnFormError"
                                         ControlToValidate="txtP1Every" Visible="False" EnableViewState="false">
                    </asp:CustomValidator>
                </div>
            </div>
            <div class="dnnFormItem">
                <dnn:Label id="lblEndDateTime" runat="server" cssclass="dnnFormRequired"></dnn:Label>
                <div>
                    <dnn:DNNDatePicker id="dpEndDate" runat="server" DateInput-CssClass="DateFormat" CssClass="DatePicker"></dnn:DNNDatePicker>
                    <div id="divEndTime" runat="server" class="TimePicker">
                        <dnn:DNNTimePicker id="tpEndTime" runat="server" DateInput-CssClass="DateFormat"></dnn:DNNTimePicker>
                    </div>
                </div>
                <asp:RequiredFieldValidator id="valRequiredEndDate" runat="server" Display="Dynamic" cssclass="dnnFormMessage dnnFormError" resourcekey="valRequiredEndDate" ControlToValidate="dpEndDate" EnableViewState="false"></asp:RequiredFieldValidator>
                <asp:CompareValidator id="valValidEndDate" Operator="GreaterThanEqual" Display="Dynamic" ControlToValidate="dpEndDate" cssclass="dnnFormMessage dnnFormError"
                                      type="Date" resourcekey="valValidEndDate" runat="server" ValidationGroup="enddate" SetFocusOnError="True" ControlToCompare="dpStartDate">
                </asp:CompareValidator>
                <asp:CustomValidator id="valValidEndTime" runat="server" Display="Dynamic" cssclass="dnnFormMessage dnnFormError" resourcekey="valValidEndTime" ControlToValidate="dpEndDate" EnableViewState="false"></asp:CustomValidator>
                <asp:RequiredFieldValidator id="valRequiredEndTime" runat="server" Display="Dynamic" cssclass="dnnFormMessage dnnFormError" resourcekey="valRequiredEndTime" ControlToValidate="tpEndTime" EnableViewState="false"></asp:RequiredFieldValidator>
                <asp:CustomValidator ID="valValidEndTime2" runat="server" SetFocusOnError="true"
                                     ControlToValidate="tpEndTime" ClientValidationFunction="ValidateTime" cssclass="dnnFormMessage dnnFormError" Text="*" OnServerValidate="valValidEndTime2_ServerValidate"/>
            </div>
            <div class="dnnFormItem">
                <dnn:Label id="lblDisplayEndDate" runat="server"></dnn:Label>
                <asp:CheckBox id="chkDisplayEndDate" runat="server" cssclass="SubHead"></asp:CheckBox>
            </div>
            <div class="dnnFormItem" id="trTimeZone" runat="server" visible="false">
                <dnn:Label id="lblTimeZone" runat="server"></dnn:Label>
                <dnn:DnnTimeZoneComboBox id="cboTimeZone" cssclass="NormalTextBox" runat="server" Font-Size="8pt"/>
            </div>
            <div class="dnnFormItem">
                <dnn:Label id="lblImportance" runat="server"></dnn:Label>
                <asp:DropDownList id="cmbImportance" runat="server" Font-Size="8pt" cssclass="NormalTextBox">
                    <asp:ListItem resourcekey="Low" Value="3">Low</asp:ListItem>
                    <asp:ListItem resourcekey="Normal" Value="2" Selected="True">Normal</asp:ListItem>
                    <asp:ListItem resourcekey="High" Value="1">High</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="dnnFormItem">
                <dnn:Label id="lblCategory" runat="server"></dnn:Label>
                <asp:DropDownList id="cmbCategory" runat="server" Font-Size="8pt" cssclass="NormalTextBox" width="194px"></asp:DropDownList>
            </div>
            <div class="dnnFormItem">
                <dnn:Label id="lblLocation" runat="server"></dnn:Label>
                <asp:DropDownList id="cmbLocation" runat="server" Font-Size="8pt" cssclass="NormalTextBox" width="194px"></asp:DropDownList>
            </div>
            <div class="dnnFormItem" id="trCustomField1" runat="server">
                <dnn:label id="lblCustomField1" runat="server"></dnn:label>
                <asp:textbox id="txtCustomField1" runat="server" cssclass="NormalTextBox" width="250px" Columns="30" maxlength="100"></asp:textbox>
            </div>
            <div class="dnnFormItem" id="trCustomField2" runat="server">
                <dnn:label id="lblCustomField2" runat="server"></dnn:label>
                <asp:textbox id="txtCustomField2" runat="server" cssclass="NormalTextBox" width="250px" Columns="30" maxlength="100"></asp:textbox>
            </div>
            <div class="dnnFormItem" id="trOwner" runat="server">
                <dnn:Label id="lblOwner" runat="server"></dnn:Label>
                <asp:DropDownList id="cmbOwner" runat="server" Font-Size="8pt" cssclass="NormalTextBox" width="194px"></asp:DropDownList>
            </div>
            <div class="dnnFormItem">
                <dnn:Label id="lblNotes" runat="server"></dnn:Label>
				<div  style="margin-left:150px">
                <dnn:TextEditor id="ftbDesktopText" runat="server" width="650" height="400"></dnn:TextEditor>
				</div>
            </div>
            <div class="dnnFormItem SummaryHeader" style="display:none;">
                <dnn:Label id="lblSummary" runat="server"></dnn:Label>
				<div  style="margin-left:150px">
                <dnn:TextEditor id="ftbSummary" runat="server" width="650" height="400"></dnn:TextEditor>
				</div>
            </div>
        </div>
    </fieldset>
</div>
<div class="EditAdvanced dnnClear" id="EditAdvanced">
<fieldset class="dnnClear">
<div id="divAdvanced">
<asp:Panel id="pnlDetailPage" runat="server" width="100%">
    <hr/>
    <div class="dnnFormItem">
        <dnn:Label id="lblDetailPage" runat="server"></dnn:Label>
        <asp:CheckBox id="chkDetailPage" runat="server" cssclass="SubHead"></asp:CheckBox>
    </div>
    <div id="tblDetailPageDetail" runat="server">
        <div class="dnnFormItem">
            <dnn:Label id="lblDetailURL" runat="server"></dnn:Label>
            <div style="display: inline-block">
                <dnn:URL id="URLDetail" runat="server" width="300" showfiles="False" showurls="True" shownewwindow="True" showtrack="False"
                         showlog="False" urltype="U" showtabs="True">
                </dnn:URL>
            </div>
        </div>
    </div>
</asp:Panel>
<asp:Panel id="pnlReminder" runat="server" width="100%">
    <hr/>
    <div class="dnnFormItem">
        <dnn:Label id="lblSendReminder" runat="server"></dnn:Label>
        <asp:CheckBox id="chkReminder" runat="server" cssclass="SubHead"></asp:CheckBox>
    </div>
    <div id="tblReminderDetail" runat="server">
        <div class="dnnFormItem">
            <dnn:Label id="lblTimeBefore" cssclass="dnnFormRequired" runat="server"></dnn:Label>
            <asp:TextBox id="txtReminderTime" cssclass="dnnFormRequired NormalTextBox" runat="server" MaxLength="3" width="50" Font-Size="8pt">8</asp:TextBox>
            <asp:DropDownList id="ddlReminderTimeMeasurement" runat="server" Font-Size="8pt" cssclass="NormalTextBox">
                <asp:ListItem Value="m" resourcekey="Minutes">Minutes</asp:ListItem>
                <asp:ListItem Value="h" resourcekey="Hours" Selected="True">Hours</asp:ListItem>
                <asp:ListItem Value="d" resourcekey="Days">Days</asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator id="valReminderTime2" runat="server" cssclass="dnnFormMessage dnnFormError"
                                        ControlToValidate="txtReminderTime" Display="Dynamic">
            </asp:RequiredFieldValidator>
            <asp:RangeValidator id="valReminderTime" runat="server" ControlToValidate="txtReminderTime" Type="Integer" cssclass="dnnFormMessage dnnFormError" Display="Dynamic"></asp:RangeValidator>
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblEmailFrom" runat="server"></dnn:Label>
            <asp:TextBox id="txtReminderFrom" runat="server" Font-Size="8pt" cssclass="NormalTextBox" width="157px" MaxLength="100" Wrap="False"></asp:TextBox>
            <asp:RegularExpressionValidator id="valEmail" runat="server" ControlToValidate="txtReminderFrom" ErrorMessage="Valid Email Address required" resourcekey="valEmail"
                                            ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" cssclass="dnnFormMessage dnnFormError">
            </asp:RegularExpressionValidator>
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblNotifyEmailSubject" runat="server"></dnn:Label>
            <asp:TextBox id="txtSubject" runat="server" Font-Size="8pt" cssclass="NormalTextBox" width="60%" TextMode="MultiLine" MaxLength="300"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblNotifyEmailMessage" runat="server"></dnn:Label>
            <asp:TextBox id="txtReminder" cssclass="NormalTextBox" runat="server" Font-Size="8pt" width="60%" height="80px" TextMode="MultiLine" MaxLength="2048"></asp:TextBox>
        </div>
    </div>
</asp:Panel>
<asp:Panel id="pnlImage" runat="server" width="100%">
    <hr/>
    <div class="dnnFormItem">
        <dnn:Label id="lblDisplayImage" runat="server"></dnn:Label>
        <asp:CheckBox id="chkDisplayImage" runat="server" cssclass="SubHead"></asp:CheckBox>
    </div>
    <div id="tblImageURL" runat="server">
        <div class="dnnFormItem">
            <dnn:Label id="lblImageURL" runat="server"></dnn:Label>
            <div style="display: inline-block">
                <dnn:URL id="ctlURL" runat="server" width="300" showfiles="True" showurls="True" shownewwindow="False" showtrack="False" showlog="False" urltype="F" showtabs="False"></dnn:URL>
            </div>
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblWidth" runat="server"></dnn:Label>
            <asp:TextBox id="txtWidth" runat="server" cssclass="NormalTextBox" Columns="50" width="37px"></asp:TextBox>
            <asp:RegularExpressionValidator id="valWidth" runat="server" ControlToValidate="txtWidth" ErrorMessage="Width must be a valid Integer or Blank" resourcekey="valWidth"
                                            ValidationExpression="^[1-9]+[0-9]*$" cssclass="dnnFormMessage dnnFormError">
            </asp:RegularExpressionValidator>
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblHeight" runat="server"></dnn:Label>
            <asp:TextBox id="txtHeight" runat="server" cssclass="NormalTextBox" Columns="50" width="37px"></asp:TextBox>
            <asp:RegularExpressionValidator id="valHeight" runat="server" ControlToValidate="txtHeight" ErrorMessage="Height must be a valid Integer or Blank" resourcekey="valHeight"
                                            ValidationExpression="^[1-9]+[0-9]*$" cssclass="dnnFormMessage dnnFormError">
            </asp:RegularExpressionValidator>
        </div>
    </div>
</asp:Panel>
<asp:Panel id="pnlRecurring" runat="server" width="100%">
    <hr/>
    <div class="dnnFormItem">
        <dnn:Label id="lblRecEvent" runat="server"></dnn:Label>
        <asp:CheckBox id="chkReccuring" runat="server" cssclass="SubHead"></asp:CheckBox> <br/>
        <div id="divRepeatTypeN" style="display: none;">
            <input id="rblRepeatTypeN" type="radio" checked="True" value="N" name="rblRepeatType" runat="server"/>
        </div>
    </div>
    <div id="tblRecurringDetails" runat="server">
        <div class="dnnFormItem">
            <dnn:Label id="lblEndDate" runat="server" cssclass="dnnFormRequired"></dnn:Label>
            <div style="display: inline-block">
                <asp:Label id="lblMaxRecurrences" runat="server" cssclass="Normal">Maximum 1000 event occurences.</asp:Label>
                <br/>
                <dnn:DnnDatePicker id="dpRecurEndDate" AutoPostBack="false" runat="server"></dnn:DnnDatePicker>
                <asp:RequiredFieldValidator id="valRequiredRecurEndDate" runat="server"
                                            ControlToValidate="dpRecurEndDate" cssclass="dnnFormMessage dnnFormError" Display="Dynamic"
                                            EnableClientScript="False" EnableViewState="false"
                                            resourcekey="valRequiredRecurEndDate">
                </asp:RequiredFieldValidator>
                <asp:CustomValidator id="valValidRecurEndDate" runat="server"
                                     ControlToValidate="dpRecurEndDate" cssclass="dnnFormMessage dnnFormError"
                                     EnableViewState="false" resourcekey="valValidRecurEndDate" OnServerValidate="valValidRecurEndDate_ServerValidate">
                </asp:CustomValidator>
                <asp:CustomValidator id="valValidRecurEndDate2" runat="server"
                                     ControlToValidate="dpRecurEndDate" cssclass="dnnFormMessage dnnFormError" EnableViewState="false"
                                     resourcekey="valValidRecurEndDate2" Visible="False">
                </asp:CustomValidator>
            </div>
        </div>
        <div class="dnnFormItem evtRadioTop">
            <dnn:Label id="lblPeriodicEvent" runat="server"></dnn:Label>
            <input id="rblRepeatTypeP1" type="radio" value="P1" name="rblRepeatType" runat="server"/>
            <div id="tblDetailP1" runat="server">
                <asp:Label id="lblEvery" runat="server" cssclass="SubHead dnnFormRequired" resourcekey="lblEvery">Repeated every:</asp:Label>
                <asp:TextBox id="txtP1Every" runat="server" Columns="3" cssclass="dnnFormRequired NormalTextBox evtShortInputFloat" MaxLength="3">1</asp:TextBox>
                <asp:DropDownList id="cmbP1Period" runat="server" cssclass="NormalTextBox">
                    <asp:ListItem resourcekey="Days" Selected="True" Value="D">Day(s)</asp:ListItem>
                    <asp:ListItem resourcekey="Weeks" Value="W">Week(s)</asp:ListItem>
                    <asp:ListItem resourcekey="Months" Value="M">Month(s)</asp:ListItem>
                    <asp:ListItem resourcekey="Years" Value="Y">Year(s)</asp:ListItem>
                </asp:DropDownList>
                <asp:RangeValidator id="valP1Every" runat="server"
                                    ControlToValidate="txtP1Every" MinimumValue="1" Display="Dynamic" MaximumValue="999" Type="Integer" cssclass="dnnFormMessage dnnFormError" EnableViewState="false"
                                    resourcekey="valP1Every" EnableClientScript="False">
                </asp:RangeValidator>
                <asp:RequiredFieldValidator id="valP1Every2" runat="server" ControlToValidate="txtP1Every" resourcekey="valP1Every"
                                            cssclass="dnnFormMessage dnnFormError" EnableViewState="false" EnableClientScript="False" Display="Dynamic">
                </asp:RequiredFieldValidator>
            </div>
        </div>
        <div class="dnnFormItem evtRadioTop">
            <dnn:Label id="lblWeeklyEvent" runat="server"></dnn:Label>
            <input id="rblRepeatTypeW1" type="radio" value="W1" name="rblRepeatType" runat="server"/>
            <div id="tblDetailW1" runat="server">
                <div>
                    <asp:Label id="lblRepetitionWeek" runat="server" cssclass="dnnFormRequired SubHead" resourcekey="lblRepetitionWeek">Repetition frequency (weeks):</asp:Label>
                    <asp:TextBox id="txtW1Every" runat="server" Columns="3" cssclass="dnnFormRequired NormalTextBox evtShortInputFloat" MaxLength="2">1</asp:TextBox>
                    <asp:RangeValidator id="valW1Day" runat="server" Display="Dynamic"
                                        ControlToValidate="txtW1Every" MinimumValue="1" MaximumValue="99" Type="Integer" cssclass="dnnFormMessage dnnFormError" EnableViewState="false"
                                        resourcekey="valW1Day" EnableClientScript="False">
                    </asp:RangeValidator>
                    <asp:RequiredFieldValidator id="valW1Day2" runat="server" ControlToValidate="txtW1Every" resourcekey="valW1Day"
                                                cssclass="dnnFormMessage dnnFormError" EnableViewState="false" EnableClientScript="False" Display="Dynamic">
                    </asp:RequiredFieldValidator>
                </div>
                <div>
                    <asp:Label id="lblWeekDays" runat="server" cssclass="SubHead" resourcekey="lblWeekDays"></asp:Label>&nbsp;&nbsp;
                    <asp:CheckBox id="chkW1Sun" runat="server" cssclass="evtRecWeekDays" Text="Sun"/>
                    <asp:CheckBox id="chkW1Mon" runat="server" cssclass="evtRecWeekDays" Text="Mon"/>
                    <asp:CheckBox id="chkW1Tue" runat="server" cssclass="evtRecWeekDays" Text="Tue"/>
                    <asp:CheckBox id="chkW1Wed" runat="server" cssclass="evtRecWeekDays" Text="Wed"/>
                    <asp:CheckBox id="chkW1Thu" runat="server" cssclass="evtRecWeekDays" Text="Thu"/>
                    <asp:CheckBox id="chkW1Fri" runat="server" cssclass="evtRecWeekDays" Text="Fri"/>
                    <asp:CheckBox id="chkW1Sat" runat="server" cssclass="evtRecWeekDays" Text="Sat"/>
                    <asp:CheckBox id="chkW1Sun2" runat="server" cssclass="evtRecWeekDays" Text="Sun"/>
                    <asp:CustomValidator id="valW1Day3" runat="server" cssclass="dnnFormMessage dnnFormError"
                                         ControlToValidate="txtW1Every" Visible="False" EnableViewState="false">
                    </asp:CustomValidator>
                </div>
            </div>
        </div>
        <div class="dnnFormItem evtRadioTop">
            <dnn:Label id="lblMonthlyEvent" runat="server"></dnn:Label>
            <input id="rblRepeatTypeM" type="radio" value="M" name="rblRepeatType" runat="server"/>
            <div id="tblDetailM1" runat="server">
                <div>
                    <input id="rblRepeatTypeM1" runat="server" name="rblRepeatTypeMM" type="radio" value="M1"/>
                    <asp:Label id="lblRepeatedOn1" runat="server" cssclass="SubHead"
                               resourcekey="lblRepeatedOn1">
                        Repeated on:
                    </asp:Label>
                    <asp:DropDownList id="cmbM1Every" runat="server" width="79px">
                        <asp:ListItem resourcekey="First" Selected="True" Value="1">First</asp:ListItem>
                        <asp:ListItem resourcekey="Second" Value="2">Second</asp:ListItem>
                        <asp:ListItem resourcekey="Third" Value="3">Third</asp:ListItem>
                        <asp:ListItem resourcekey="Fourth" Value="4">Fourth</asp:ListItem>
                        <asp:ListItem resourcekey="Last" Value="5">Last</asp:ListItem>
                    </asp:DropDownList>
                    <asp:DropDownList id="cmbM1Period" runat="server" width="86px">
                        <asp:ListItem Value="0">Sunday</asp:ListItem>
                        <asp:ListItem Value="1">Monday</asp:ListItem>
                        <asp:ListItem Value="2">Tuesday</asp:ListItem>
                        <asp:ListItem Value="3">Wednesday</asp:ListItem>
                        <asp:ListItem Value="4">Thursday</asp:ListItem>
                        <asp:ListItem Value="5">Friday</asp:ListItem>
                        <asp:ListItem Value="6">Saturday</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div>
                    <input id="rblRepeatTypeM2" runat="server" name="rblRepeatTypeMM" type="radio" value="M2"/>
                    <asp:Label id="lblRepeatedOn2" runat="server" cssclass="SubHead"
                               resourcekey="lblRepeatedOn2">
                        Repeated on day:
                    </asp:Label>
                    <asp:DropDownList id="cmbM2Period" runat="server" width="58px">
                        <asp:ListItem resourcekey="01" Selected="True" Value="1">1st</asp:ListItem>
                        <asp:ListItem resourcekey="02" Value="2">2nd</asp:ListItem>
                        <asp:ListItem resourcekey="03" Value="3">3rd</asp:ListItem>
                        <asp:ListItem resourcekey="04" Value="4">4th</asp:ListItem>
                        <asp:ListItem resourcekey="05" Value="5">5th</asp:ListItem>
                        <asp:ListItem resourcekey="06" Value="6">6th</asp:ListItem>
                        <asp:ListItem resourcekey="07" Value="7">7th</asp:ListItem>
                        <asp:ListItem resourcekey="08" Value="8">8th</asp:ListItem>
                        <asp:ListItem resourcekey="09" Value="9">9th</asp:ListItem>
                        <asp:ListItem resourcekey="10" Value="10">10th</asp:ListItem>
                        <asp:ListItem resourcekey="11" Value="11">11th</asp:ListItem>
                        <asp:ListItem resourcekey="12" Value="12">12th</asp:ListItem>
                        <asp:ListItem resourcekey="13" Value="13">13th</asp:ListItem>
                        <asp:ListItem resourcekey="14" Value="14">14th</asp:ListItem>
                        <asp:ListItem resourcekey="15" Value="15">15th</asp:ListItem>
                        <asp:ListItem resourcekey="16" Value="16">16th</asp:ListItem>
                        <asp:ListItem resourcekey="17" Value="17">17th</asp:ListItem>
                        <asp:ListItem resourcekey="18" Value="18">18th</asp:ListItem>
                        <asp:ListItem resourcekey="19" Value="19">19th</asp:ListItem>
                        <asp:ListItem resourcekey="20" Value="20">20th</asp:ListItem>
                        <asp:ListItem resourcekey="21" Value="21">21st</asp:ListItem>
                        <asp:ListItem resourcekey="22" Value="22">22nd</asp:ListItem>
                        <asp:ListItem resourcekey="23" Value="23">23rd</asp:ListItem>
                        <asp:ListItem resourcekey="24" Value="24">24th</asp:ListItem>
                        <asp:ListItem resourcekey="25" Value="25">25th</asp:ListItem>
                        <asp:ListItem resourcekey="26" Value="26">26th</asp:ListItem>
                        <asp:ListItem resourcekey="27" Value="27">27th</asp:ListItem>
                        <asp:ListItem resourcekey="28" Value="28">28th</asp:ListItem>
                        <asp:ListItem resourcekey="29" Value="29">29th</asp:ListItem>
                        <asp:ListItem resourcekey="30" Value="30">30th</asp:ListItem>
                        <asp:ListItem resourcekey="31" Value="31">31st</asp:ListItem>
                    </asp:DropDownList><br/>
                </div>
                <div>
                    <asp:Label id="lblRepetitionMonth" runat="server" cssclass="dnnFormRequired SubHead" resourcekey="lblRepetitionMonth">Repetition frequency (months)</asp:Label>
                    <asp:TextBox id="txtMEvery" runat="server" Columns="3" cssclass="dnnFormRequired NormalTextBox evtShortInputFloat" MaxLength="2">1</asp:TextBox>
                    <asp:RangeValidator id="valM2Every" runat="server" Display="Dynamic"
                                        ControlToValidate="txtMEvery" MinimumValue="1" MaximumValue="99" Type="Integer" cssclass="dnnFormMessage dnnFormError" EnableViewState="false"
                                        resourcekey="valM2Every" EnableClientScript="false">
                    </asp:RangeValidator>
                    <asp:RequiredFieldValidator id="valM2Every2" runat="server" ControlToValidate="txtMEvery" resourcekey="valM2Every"
                                                cssclass="dnnFormMessage dnnFormError" EnableViewState="false" EnableClientScript="False" Display="Dynamic">
                    </asp:RequiredFieldValidator>
                </div>
            </div>
        </div>
        <div class="dnnFormItem evtRadioTop">
            <dnn:Label id="lblYearlyEvent" runat="server"></dnn:Label>
            <input id="rblRepeatTypeY1" type="radio" value="Y1" name="rblRepeatType" runat="server"/>
            <div id="tblDetailY1" runat="server">
                <asp:Label id="lblRepeatOnDate" runat="server" cssclass="dnnFormRequired SubHead" resourcekey="lblRepeatOnDate">Repeated on this date:</asp:Label>
                <dnn:DnnDatePicker id="dpY1Period" AutoPostBack="false" runat="server"></dnn:DnnDatePicker>
                <asp:RequiredFieldValidator id="valRequiredYearEventDate" runat="server"
                                            ControlToValidate="dpY1Period" cssclass="dnnFormMessage dnnFormError" EnableViewState="false"
                                            ErrorMessage="Invalid Annual Repeat Date" Display="Dynamic" EnableClientScript="False" resourcekey="valRequiredYearEventDate">
                </asp:RequiredFieldValidator>
                <asp:CompareValidator id="valValidYearEventDate" runat="server" ControlToValidate="dpY1Period" ControlToCompare="dpStartDate" resourcekey="valValidYearEventDate"
                                      cssclass="dnnFormMessage dnnFormError" EnableClientScript="False" EnableViewState="false" Display="Dynamic" Visible="false" Operator="GreaterThanEqual" Type="Date">
                </asp:CompareValidator>
            </div>
        </div>
    </div>
</asp:Panel>
<asp:Panel id="pnlEnroll" runat="server" width="100%">
    <hr/>
    <div class="dnnFormItem">
        <dnn:Label id="lblAllowErollment" runat="server"></dnn:Label>
        <asp:CheckBox id="chkSignups" AutoPostBack="True" runat="server" cssclass="SubHead" Visible="False"></asp:CheckBox>
    </div>
    <div id="tblEnrollmentDetails" runat="server">
        <div id="trAllowAnonEnroll" runat="server">
            <div class="dnnFormItem">
                <dnn:Label id="plAllowAnonEnroll" text="AllowAnonEnroll:" runat="server" controlname="chkAllowAnonEnroll"></dnn:Label>
                <asp:CheckBox id="chkAllowAnonEnroll" runat="server" Checked="False"></asp:CheckBox>
            </div>
        </div>
        <div id="trTypeOfEnrollment" runat="server" style="display: block;">
            <div class="dnnFormItem">
                <dnn:Label id="lblTypeOfEnrollment" runat="server"></dnn:Label>
                <div style="display: inline-block; width: 60%;">
                    <div>
                        <input id="rblFree" type="radio" checked="True" value="FREE" name="rblEnrollType" runat="server"/>&nbsp;
                        <asp:Label id="lblFree" resourcekey="lblFree" runat="server" cssclass="SubHead">Free</asp:Label><asp:Label id="lblModerated"
                                                                                                                                   resourcekey="lblModerated.Text" runat="server" cssclass="SubHead">
                            (Moderated)
                        </asp:Label>
                    </div>
                    <div>
                        <input id="rblPaid" type="radio" value="PAID" name="rblEnrollType" runat="server"/>&nbsp;
                        <asp:Label id="lblPaidFee" resourcekey="lblPaidFee" runat="server" cssclass="dnnFormRequired SubHead">Paid Fee:</asp:Label>&nbsp;
                        <asp:TextBox id="txtEnrollFee" runat="server" Font-Size="8pt" cssclass="dnnFormRequired NormalTextBox evtShortInputFloat"
                                     MaxLength="8" Wrap="False">
                        </asp:TextBox>
                        <asp:Label id="lblTotalCurrency" runat="server" cssclass="SubHead"></asp:Label>
                        <asp:RequiredFieldValidator id="valBadFee" runat="server" cssclass="dnnFormMessage dnnFormError" ErrorMessage="Numeric fee > 0.00 required for Paid Enrollment" resourcekey="valBadFee"
                                                    ControlToValidate="txtEnrollFee" Visible="False" EnableViewState="false">
                        </asp:RequiredFieldValidator>
                    </div>
                </div>
            </div>
        </div>
        <div id="trPayPalAccount" runat="server">
            <div class="dnnFormItem">
                <dnn:Label id="lblPayPalAccount" runat="server" cssclass="dnnFormRequired"></dnn:Label>
                <asp:TextBox id="txtPayPalAccount" runat="server" Font-Size="8pt" cssclass="dnnFormRequired NormalTextBox" width="147px" MaxLength="100" Wrap="False"></asp:TextBox>
                <asp:RequiredFieldValidator id="valPayPalAccount" runat="server" cssclass="dnnFormMessage dnnFormError" ErrorMessage="PayPal Account required for Paid Enrollment" resourcekey="valPayPalAccount" ControlToValidate="txtPayPalAccount"
                                            Visible="False" EnableViewState="false">
                </asp:RequiredFieldValidator>
            </div>
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblMaxEnrollment" runat="server" cssclass="dnnFormRequired"></dnn:Label>
            <asp:TextBox id="txtMaxEnrollment" runat="server" cssclass="dnnFormRequired NormalTextBox evtShortInput" MaxLength="3">0</asp:TextBox>&nbsp;
            <asp:Label id="lblCurrentEnrolled" resourcekey="lblCurrentEnrolled" runat="server" cssclass="SubHead">Currently Enrolled:</asp:Label>&nbsp;
            <asp:TextBox id="txtEnrolled" runat="server" cssclass="SubHead evtShortInputFloat" MaxLength="3" ReadOnly="True" BorderStyle="None">0</asp:TextBox>
            <asp:RequiredFieldValidator id="valMaxEnrollment2" runat="server" cssclass="dnnFormMessage dnnFormError" resourcekey="valMaxEnrollment" ControlToValidate="txtMaxEnrollment"></asp:RequiredFieldValidator>
            <asp:RangeValidator id="valMaxEnrollment" runat="server" resourcekey="valMaxEnrollment"
                                ControlToValidate="txtMaxEnrollment" MinimumValue="0" MaximumValue="9999" Display="Dynamic" Type="Integer" cssclass="dnnFormMessage dnnFormError"
                                EnableClientScript="False">
            </asp:RangeValidator>
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblEnrollListView" text="Enroll List on Detail View:" runat="server" controlname="chkEnrollListView"></dnn:Label>
            <asp:CheckBox id="chkEnrollListView" runat="server" Checked="False"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label id="lblEnrollmentRole" runat="server"></dnn:Label>
            <div style="display: inline-block">
                <asp:DropDownList id="ddEnrollRoles" AutoPostBack="True" runat="server" Font-Size="8pt" width="214px"></asp:DropDownList>
                <asp:Label id="lblEnrollRoleNote" resourcekey="lblEnrollRoleNote" runat="server" cssclass="SubHead">(select "None" for All Registered)</asp:Label>
            </div>
        </div>
        <div class="dnnFormItem" id="divAddUser" runat="server">
            <dnn:Label id="lblRegUsers" runat="server"></dnn:Label>
            <div style="display: inline-block">
                <evt:AddUser ID="grdAddUser" runat="server" OnAddSelectedUsers="grdAddUser_AddSelectedUsers"/>
                <div id="divNoEnrolees" runat="server" visible="false">
                    <asp:Label id="lblNoEnrolee" runat="server" resourcekey="lblNoEnrolee">No. of Enrolees</asp:Label>&nbsp;
                    <asp:TextBox id="txtNoEnrolees" runat="server" cssclass="NormalTextBox evtShortInputFloat" Font-Size="8pt" MaxLength="3">1</asp:TextBox>&nbsp;
                    <asp:Label id="lblMaxNoEnrolees" runat="server">(Max 1)</asp:Label>&nbsp;
                    <asp:RangeValidator id="valNoEnrolees" runat="server"
                                        ControlToValidate="txtNoEnrolees" MinimumValue="1" MaximumValue="1" Display="Dynamic" Type="Integer" cssclass="dnnFormMessage dnnFormError"
                                        EnableClientScript="False">
                    </asp:RangeValidator>
                </div>
            </div>
        </div>
        <div class="dnnFormItem">
            <asp:Label id="lblEnrolledUsers" resourcekey="lblEnrolledUsers" runat="server" style="font-weight: bold">Enrolled Users</asp:Label>
            <asp:DataGrid id="grdEnrollment" runat="server" AutoGenerateColumns="False" CssClass="EditEnrollGrid"
                          DataKeyField="SignupID" GridLines="None" Visible="False" width="100%">
                <EditItemStyle VerticalAlign="Bottom"></EditItemStyle>
                <AlternatingItemStyle CssClass="EditEnrollGridAlternate"></AlternatingItemStyle>
                <ItemStyle VerticalAlign="Top"></ItemStyle>
                <Columns>
                    <asp:TemplateColumn HeaderText="Select">
                        <HeaderStyle CssClass="EditEnrollGridHeader"></HeaderStyle>
                        <ItemStyle CssClass="EditEnrollSelect"></ItemStyle>
                        <ItemTemplate>
                            <asp:CheckBox id="chkSelect" runat="server"></asp:CheckBox>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:BoundColumn DataField="EnrollUserName" HeaderText="UserName">
                        <HeaderStyle CssClass="EditEnrollGridHeader"></HeaderStyle>
                        <ItemStyle CssClass="EditEnrollUser"></ItemStyle>
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="EnrollDisplayName" HeaderText="DisplayName">
                        <HeaderStyle CssClass="EditEnrollGridHeader"></HeaderStyle>
                        <ItemStyle CssClass="EditEnrollDisplay"></ItemStyle>
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="EnrollEmail" HeaderText="Email">
                        <HeaderStyle CssClass="EditEnrollGridHeader"></HeaderStyle>
                        <ItemStyle CssClass="EditEnrollEmail"></ItemStyle>
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="Enrollphone" HeaderText="Phone">
                        <HeaderStyle CssClass="EditEnrollGridHeader"></HeaderStyle>
                        <ItemStyle CssClass="EditEnrollPhone"></ItemStyle>
                    </asp:BoundColumn>
                    <asp:TemplateColumn headerText="EnrollApproved">
                        <HeaderStyle CssClass="EditEnrollGridHeader"></HeaderStyle>
                        <ItemStyle CssClass="EditEnrollApproved"></ItemStyle>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkEnrollApproved" runat="server" Enabled="false" Checked='<%# DataBinder.Eval(Container.DataItem, "EnrollApproved") %>'/>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:BoundColumn DataField="EnrollNo" HeaderText="Qty">
                        <HeaderStyle CssClass="EditEnrollGridHeader"></HeaderStyle>
                        <ItemStyle CssClass="EditEnrollNo"></ItemStyle>
                    </asp:BoundColumn>
                    <asp:TemplateColumn HeaderText="Event Date">
                        <HeaderStyle CssClass="EditEnrollGridHeader"></HeaderStyle>
                        <ItemStyle CssClass="EditEnrollDate"></ItemStyle>
                        <ItemTemplate>
                            <asp:Label id="lblEventBegin" runat="server" Text='<%# string.Format(DataBinder.Eval(Container.DataItem, "EnrollTimeBegin", "{0:d}")) %>'>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>
            <dnn:CommandButton id="lnkSelectedEmail" IconKey="Email" resourcekey="lnkSelectedEmail" runat="server" cssclass="CommandButton"/>&nbsp;&nbsp;
            <dnn:CommandButton id="lnkSelectedDelete" IconKey="Delete" resourcekey="lnkSelectedDelete" runat="server" cssclass="CommandButton"/>
        </div>
    </div>
</asp:Panel>
<asp:Panel id="pnlEventEmailRole" runat="server" width="100%">
    <hr/>
    <div class="dnnFormItem">
        <dnn:Label id="lblEventEmailChk" runat="server"></dnn:Label>
        <asp:CheckBox id="chkEventEmailChk" runat="server" cssclass="SubHead"></asp:CheckBox>
    </div>
    <div id="tblEventEmailRoleDetail" runat="server">
        <div class="dnnFormItem">
            <dnn:Label id="lblEventEmailRole" runat="server"></dnn:Label>
            <asp:DropDownList id="ddEventEmailRoles" AutoPostBack="False" runat="server" Font-Size="8pt" width="214px"></asp:DropDownList>
        </div>
    </div>
</asp:Panel>
<div id="tblEventEmail" runat="server">
    <hr/>
    <asp:Label id="lblEventEmail" runat="server" resourcekey="lblEventEmail" cssclass="Head"></asp:Label>
    <div class="dnnFormItem">
        <dnn:Label id="lblEventEmailFrom" runat="server"></dnn:Label>
        <asp:TextBox id="txtEventEmailFrom" runat="server" Font-Size="8pt" cssclass="NormalTextBox" width="157px" MaxLength="100" Wrap="False"></asp:TextBox>
        <asp:RegularExpressionValidator id="RegularExpressionValidator1" runat="server" ControlToValidate="txtEventEmailFrom" ErrorMessage="Valid Email Address required" resourcekey="valEmail"
                                        ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" cssclass="dnnFormMessage dnnFormError">
        </asp:RegularExpressionValidator>
    </div>
    <div class="dnnFormItem">
        <dnn:Label id="lblEventEmailSubject" runat="server"></dnn:Label>
        <asp:TextBox id="txtEventEmailSubject" runat="server" Font-Size="8pt" cssclass="NormalTextBox" width="60%" TextMode="MultiLine" MaxLength="300"></asp:TextBox>
    </div>
    <div class="dnnFormItem">
        <dnn:Label id="lblEventEmailBody" runat="server"></dnn:Label>
        <asp:TextBox id="txtEventEmailBody" cssclass="NormalTextBox" runat="server" Font-Size="8pt" width="60%" height="80px" TextMode="MultiLine" MaxLength="2048"></asp:TextBox>
    </div>
</div>
</div>
</fieldset>
</div>
<div class="dnnClear"> </div>
<ul class="dnnActions dnnClear">
    <li>
        <asp:LinkButton OnClick="updateButton_Click" id="updateButton" runat="server" cssclass="dnnPrimaryAction"/>
    </li>
    <li>
        <asp:LinkButton OnClick="cancelButton_Click" id="cancelButton" runat="server" cssclass="dnnSecondaryAction" resourcekey="cancelButton" CausesValidation="False"/>
    </li>
    <li>
        <asp:LinkButton OnClick="deleteButton_Click" id="deleteButton" runat="server" cssclass="dnnSecondaryAction" CausesValidation="False"/>
    </li>
    <li>
        <asp:LinkButton OnClick="copyButton_Click" id="copyButton" runat="server" cssclass="dnnSecondaryAction"/>
    </li>
</ul>
<asp:Panel id="pnlAudit" Visible="false" HorizontalAlign="Left" runat="server" width="100%">
    <hr/>
    <span class="Normal">
        <asp:Label id="lblCreatedBy" runat="server" resourcekey="lblCreatedBy" cssclass="SubHead" Visible="False">Created by:</asp:Label>&nbsp;
        <asp:Label id="CreatedBy" runat="server" cssclass="SubHead"></asp:Label>&nbsp;
        <asp:Label id="lblOn" runat="server" resourcekey="lblOn" cssclass="SubHead">on</asp:Label>&nbsp;
        <asp:Label id="CreatedDate" runat="server" cssclass="SubHead"></asp:Label>
    </span>
</asp:Panel>
</div>
</asp:Panel>

<script language="javascript" type="text/javascript">

    /*globals jQuery, window, Sys */
    (function($, sys) {
        function setupEditEvents() {
            $('#EditEvents').dnnTabs();
        }

        $(document).ready(function() {
            setupEditEvents();
            sys.WebForms.PageRequestManager.getInstance().add_endRequest(function() {
                setupEditEvents();
            });
        });
    }(jQuery, window.Sys));
</script>